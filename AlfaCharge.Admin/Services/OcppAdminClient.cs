using AlfaCharge.Admin.Models;

namespace AlfaCharge.Admin.Services;

/// <summary>
/// Client for executing OCPP actions on stations.
/// </summary>
public sealed class OcppAdminClient
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<OcppAdminClient> _logger;

    public OcppAdminClient(ApiClient apiClient, ILogger<OcppAdminClient> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Remote start a transaction.
    /// </summary>
    public async Task<ApiResult<OcppActionResult>> RemoteStartAsync(
        string chargePointId,
        RemoteStartRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing RemoteStart on {ChargePointId}", chargePointId);
        return await _apiClient.PostAsync<RemoteStartRequest, OcppActionResult>(
            $"/api/admin/stations/{chargePointId}/actions/remote-start", request, ct);
    }

    /// <summary>
    /// Remote stop a transaction.
    /// </summary>
    public async Task<ApiResult<OcppActionResult>> RemoteStopAsync(
        string chargePointId,
        RemoteStopRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing RemoteStop on {ChargePointId}", chargePointId);
        return await _apiClient.PostAsync<RemoteStopRequest, OcppActionResult>(
            $"/api/admin/stations/{chargePointId}/actions/remote-stop", request, ct);
    }

    /// <summary>
    /// Reset a station.
    /// </summary>
    public async Task<ApiResult<OcppActionResult>> ResetAsync(
        string chargePointId,
        ResetRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing Reset ({Type}) on {ChargePointId}", request.Type, chargePointId);
        return await _apiClient.PostAsync<ResetRequest, OcppActionResult>(
            $"/api/admin/stations/{chargePointId}/actions/reset", request, ct);
    }

    /// <summary>
    /// Trigger a message from the station.
    /// </summary>
    public async Task<ApiResult<OcppActionResult>> TriggerMessageAsync(
        string chargePointId,
        TriggerMessageRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing TriggerMessage ({Message}) on {ChargePointId}", 
            request.RequestedMessage, chargePointId);
        return await _apiClient.PostAsync<TriggerMessageRequest, OcppActionResult>(
            $"/api/admin/stations/{chargePointId}/actions/trigger-message", request, ct);
    }

    /// <summary>
    /// Get the local list version.
    /// </summary>
    public async Task<ApiResult<OcppActionResult>> GetLocalListVersionAsync(
        string chargePointId,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing GetLocalListVersion on {ChargePointId}", chargePointId);
        return await _apiClient.GetAsync<OcppActionResult>(
            $"/api/admin/stations/{chargePointId}/actions/local-list-version", ct);
    }

    /// <summary>
    /// Send local list to station.
    /// </summary>
    public async Task<ApiResult<OcppActionResult>> SendLocalListAsync(
        string chargePointId,
        SendLocalListRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing SendLocalList on {ChargePointId}", chargePointId);
        return await _apiClient.PostAsync<SendLocalListRequest, OcppActionResult>(
            $"/api/admin/stations/{chargePointId}/actions/send-local-list", request, ct);
    }

    /// <summary>
    /// Reserve a connector.
    /// </summary>
    public async Task<ApiResult<OcppActionResult>> ReserveNowAsync(
        string chargePointId,
        ReserveNowRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing ReserveNow on {ChargePointId}", chargePointId);
        return await _apiClient.PostAsync<ReserveNowRequest, OcppActionResult>(
            $"/api/admin/stations/{chargePointId}/actions/reserve-now", request, ct);
    }

    /// <summary>
    /// Cancel a reservation.
    /// </summary>
    public async Task<ApiResult<OcppActionResult>> CancelReservationAsync(
        string chargePointId,
        CancelReservationRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing CancelReservation on {ChargePointId}", chargePointId);
        return await _apiClient.PostAsync<CancelReservationRequest, OcppActionResult>(
            $"/api/admin/stations/{chargePointId}/actions/cancel-reservation", request, ct);
    }

    /// <summary>
    /// Clear charging profile.
    /// </summary>
    public async Task<ApiResult<OcppActionResult>> ClearChargingProfileAsync(
        string chargePointId,
        ClearChargingProfileRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing ClearChargingProfile on {ChargePointId}", chargePointId);
        return await _apiClient.PostAsync<ClearChargingProfileRequest, OcppActionResult>(
            $"/api/admin/stations/{chargePointId}/actions/clear-charging-profile", request, ct);
    }

    /// <summary>
    /// Set charging profile.
    /// </summary>
    public async Task<ApiResult<OcppActionResult>> SetChargingProfileAsync(
        string chargePointId,
        SetChargingProfileRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing SetChargingProfile on {ChargePointId}", chargePointId);
        return await _apiClient.PostAsync<SetChargingProfileRequest, OcppActionResult>(
            $"/api/admin/stations/{chargePointId}/actions/set-charging-profile", request, ct);
    }

    /// <summary>
    /// Get composite schedule.
    /// </summary>
    public async Task<ApiResult<OcppActionResult>> GetCompositeScheduleAsync(
        string chargePointId,
        GetCompositeScheduleRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Executing GetCompositeSchedule on {ChargePointId}", chargePointId);
        return await _apiClient.PostAsync<GetCompositeScheduleRequest, OcppActionResult>(
            $"/api/admin/stations/{chargePointId}/actions/get-composite-schedule", request, ct);
    }
}
