using AlfaCharge.Admin.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace AlfaCharge.Admin.Services;

/// <summary>
/// SignalR client for live session updates.
/// </summary>
public sealed class SessionsHubClient : IAsyncDisposable
{
    private readonly ILogger<SessionsHubClient> _logger;
    private readonly IConfiguration _configuration;
    private HubConnection? _connection;
    private CancellationTokenSource? _reconnectCts;

    public event Action<LiveSessionViewModel>? OnSessionUpdated;
    public event Action<string>? OnStationStatusChanged;
    public event Action<bool>? OnConnectionStateChanged;

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    public SessionsHubClient(ILogger<SessionsHubClient> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Start the SignalR connection.
    /// </summary>
    public async Task StartAsync(CancellationToken ct = default)
    {
        var hubUrl = _configuration["SignalRHubUrl"] ?? "https://localhost:7001/hub/ocpp";

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect(new RetryPolicy())
            .Build();

        _connection.Closed += OnClosed;
        _connection.Reconnecting += OnReconnecting;
        _connection.Reconnected += OnReconnected;

        // Register handlers
        _connection.On<LiveSessionViewModel>("SessionUpdated", session =>
        {
            _logger.LogDebug("Received session update for {ChargePointId}", session.ChargePointId);
            OnSessionUpdated?.Invoke(session);
        });

        _connection.On<string, string>("StationStatusChanged", (chargePointId, status) =>
        {
            _logger.LogDebug("Station {ChargePointId} status changed to {Status}", chargePointId, status);
            OnStationStatusChanged?.Invoke(chargePointId);
        });

        _reconnectCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        try
        {
            await _connection.StartAsync(_reconnectCts.Token);
            _logger.LogInformation("SignalR connection established");
            OnConnectionStateChanged?.Invoke(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to SignalR hub");
            OnConnectionStateChanged?.Invoke(false);
            _ = ReconnectWithBackoffAsync(_reconnectCts.Token);
        }
    }

    /// <summary>
    /// Join a charge point group to receive updates for that station.
    /// </summary>
    public async Task JoinChargePointGroupAsync(string chargePointId, CancellationToken ct = default)
    {
        if (_connection?.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("JoinChargePointGroup", chargePointId, ct);
        }
    }

    /// <summary>
    /// Leave a charge point group.
    /// </summary>
    public async Task LeaveChargePointGroupAsync(string chargePointId, CancellationToken ct = default)
    {
        if (_connection?.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("LeaveChargePointGroup", chargePointId, ct);
        }
    }

    /// <summary>
    /// Stop the SignalR connection.
    /// </summary>
    public async Task StopAsync()
    {
        _reconnectCts?.Cancel();

        if (_connection is not null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
            _connection = null;
        }

        OnConnectionStateChanged?.Invoke(false);
    }

    private Task OnClosed(Exception? exception)
    {
        if (exception is not null)
        {
            _logger.LogWarning(exception, "SignalR connection closed with error");
        }
        else
        {
            _logger.LogInformation("SignalR connection closed");
        }

        OnConnectionStateChanged?.Invoke(false);
        return Task.CompletedTask;
    }

    private Task OnReconnecting(Exception? exception)
    {
        _logger.LogWarning(exception, "SignalR reconnecting...");
        OnConnectionStateChanged?.Invoke(false);
        return Task.CompletedTask;
    }

    private Task OnReconnected(string? connectionId)
    {
        _logger.LogInformation("SignalR reconnected with connection ID: {ConnectionId}", connectionId);
        OnConnectionStateChanged?.Invoke(true);
        return Task.CompletedTask;
    }

    private async Task ReconnectWithBackoffAsync(CancellationToken ct)
    {
        var delays = new[] { 1000, 2000, 5000, 10000, 30000 };
        var attempt = 0;

        while (!ct.IsCancellationRequested && _connection?.State != HubConnectionState.Connected)
        {
            var delay = delays[Math.Min(attempt, delays.Length - 1)];
            _logger.LogInformation("Attempting reconnect in {Delay}ms (attempt {Attempt})", delay, attempt + 1);

            try
            {
                await Task.Delay(delay, ct);
                await _connection!.StartAsync(ct);
                _logger.LogInformation("Reconnection successful");
                OnConnectionStateChanged?.Invoke(true);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Reconnection attempt {Attempt} failed", attempt + 1);
                attempt++;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _reconnectCts?.Dispose();
    }

    private sealed class RetryPolicy : IRetryPolicy
    {
        private static readonly TimeSpan[] Delays =
        [
            TimeSpan.FromSeconds(0),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(30)
        ];

        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return retryContext.PreviousRetryCount < Delays.Length
                ? Delays[retryContext.PreviousRetryCount]
                : TimeSpan.FromSeconds(60);
        }
    }
}
