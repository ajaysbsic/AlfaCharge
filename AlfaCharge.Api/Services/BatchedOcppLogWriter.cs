using System.Threading.Channels;
using AlfaCharge.Domain.Entities;
using AlfaCharge.Infrastructure.DB;
using AlfaCharge.OcppServer.Services;
using Microsoft.EntityFrameworkCore;

namespace AlfaCharge.Api.Services;

/// <summary>
/// Buffers OCPP frame logs in-memory and flushes them in batches to reduce DB write pressure.
/// </summary>
public sealed class BatchedOcppLogWriter : BackgroundService, IOcppLogWriter
{
    private readonly Channel<OCPPLog> _channel = Channel.CreateUnbounded<OCPPLog>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BatchedOcppLogWriter> _logger;

    private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(1);
    private const int BatchSize = 100;

    public BatchedOcppLogWriter(IServiceScopeFactory scopeFactory, ILogger<BatchedOcppLogWriter> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public Task LogAsync(
        string chargePointId,
        string direction,
        int messageTypeId,
        string? messageId,
        string? action,
        string payloadJson,
        string? resultCode = null)
    {
        var entry = new OCPPLog
        {
            Id = Guid.NewGuid(),
            ChargePointId = chargePointId,
            Direction = direction,
            MessageTypeId = messageTypeId,
            MessageId = messageId,
            Action = action,
            PayloadJson = payloadJson,
            ResultCode = resultCode,
            Timestamp = DateTimeOffset.UtcNow
        };

        // Never block protocol handling threads; fallback to async write if queue is momentarily busy.
        if (_channel.Writer.TryWrite(entry))
            return Task.CompletedTask;

        return _channel.Writer.WriteAsync(entry).AsTask();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var batch = new List<OCPPLog>(BatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var readTask = _channel.Reader.ReadAsync(stoppingToken).AsTask();
                var delayTask = Task.Delay(FlushInterval, stoppingToken);

                var completed = await Task.WhenAny(readTask, delayTask);
                if (completed == readTask)
                {
                    batch.Add(readTask.Result);

                    while (batch.Count < BatchSize && _channel.Reader.TryRead(out var item))
                    {
                        batch.Add(item);
                    }
                }

                if (batch.Count == 0)
                    continue;

                await FlushBatchAsync(batch, stoppingToken);
                batch.Clear();
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OCPP log writer loop");
            }
        }

        // Final drain on shutdown.
        while (_channel.Reader.TryRead(out var item))
        {
            batch.Add(item);
            if (batch.Count >= BatchSize)
            {
                await FlushBatchAsync(batch, CancellationToken.None);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await FlushBatchAsync(batch, CancellationToken.None);
        }
    }

    private async Task FlushBatchAsync(List<OCPPLog> batch, CancellationToken ct)
    {
        if (batch.Count == 0)
            return;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await db.OcppLogs.AddRangeAsync(batch, ct);
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to persist OCPP log batch of {Count}", batch.Count);
        }
    }
}
