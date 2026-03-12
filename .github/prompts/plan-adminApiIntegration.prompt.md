# Admin–API Integration Plan

## Overview

Fix all mismatches between `AlfaCharge.Admin` (Blazor Server) and `AlfaCharge.Api` (ASP.NET Core) so the Admin portal communicates correctly with real API endpoints. Analysis is complete — 10 file edits required.

---

## Files to Edit

| # | File | Change |
|---|---|---|
| 1 | `AlfaCharge.Api/Controllers/LocationController.cs` | Full rewrite with EF Core, paged + `/all` endpoints, proper DTOs |
| 2 | `AlfaCharge.Api/DTO/MetricsDtos.cs` | Rename `HourlyData`→`Data`, `DailyData`→`Data`; add `ErrorsChartDto` |
| 3 | `AlfaCharge.Api/Services/IMetricsQueryService.cs` | `GetErrorStatsAsync` → returns `Task<ErrorsChartDto>` |
| 4 | `AlfaCharge.Api/Services/MetricsQueryService.cs` | Populate `.Data`; `GetErrorStatsAsync` maps to `ErrorsChartDto` |
| 5 | `AlfaCharge.Api/Controllers/MetricsController.cs` | `GetErrorStats` → `ActionResult<ErrorsChartDto>` |
| 6 | `AlfaCharge.Admin/appsettings.Development.json` | Add `ApiBaseUrl: http://localhost:5100`, `SignalRHubUrl: http://localhost:5100/hub/ocpp` |
| 7 | `AlfaCharge.Admin/Models/ViewModels.cs` | `LastLogin`→`LastLoginAt`; `LastUsed`→`LastUsedAt`; `LastUsedStation`→`LastUsedStationId` |
| 8 | `AlfaCharge.Admin/Components/Pages/Users/UsersList.razor` | `context.LastLogin` → `context.LastLoginAt` |
| 9 | `AlfaCharge.Admin/Components/Pages/Rfid/RfidList.razor` | `context.LastUsed`→`context.LastUsedAt`; `context.LastUsedStation`→`context.LastUsedStationId` |
| 10 | `AlfaCharge.Admin/Services/LocationService.cs` | `GetAllLocationsAsync()` URL → `/api/location/all` |

---

## Task 1 — Rewrite `LocationController.cs`

Replace the entire file with an EF-Core-based controller that:

- Injects `ApplicationDbContext` and `ILogger`
- `GET /api/location/Locations?page&pageSize&search&sortBy&sortDescending` → `PagedResultDto<LocationListDto>` with `StationCount`, `AvailableConnectors`, `ChargingConnectors` via group-join
- `GET /api/location/all` → `List<LocationListDto>` (no paging, for dropdown in `StationsList` page)
- `GET /api/location/{locationId}` → single `LocationListDto` (fix route param binding)
- `POST /api/location/AddLocation` → accept `LocationUpsertDto`, map to `Location` entity, return `LocationListDto`
- `PUT /api/location/UpdateLocation?id={locationId}` → accept `LocationUpsertDto`, update entity, return `NoContent`
- `DELETE /api/location/{locationId}` → return `NoContent`

**Why**: Current controller returns raw `Location` entities (no pagination, no DTOs, circular ref risk), `GET {id}` has param name mismatch (`locationId` in name but `{id}` in template), `PUT` body is `[FromBody] string value` (completely wrong), returns `bool` from DELETE.

**DbSet names** (from `ApplicationDbContext`): `Locations`, `ChargePoints`, `Connectors`

---

## Task 2 — Fix Metrics DTO Shape Mismatches

### `AlfaCharge.Api/DTO/MetricsDtos.cs`

Current problems:
- `SessionsChartDto` has `HourlyData` + `DailyData` — Admin `SessionsChartData` expects `.Data`
- `EnergyChartDto` has `DailyData` + `WeeklyData` — Admin `EnergyChartData` expects `.Data`
- No `ErrorsChartDto` — `GetErrorStats` returns `List<ErrorStatsDto>` but Admin expects `ErrorsChartData { List<ChartDataPoint> Data }`
- `OcppTrafficChartDto` has `InboundData`/`OutboundData` — **already matches** Admin's `OcppTrafficChartData` (no change needed)

Changes:
- Rename `SessionsChartDto.HourlyData` → `Data` (use hourly, last 24h)
- Rename `EnergyChartDto.DailyData` → `Data` (use daily, last 7 days)
- Add `ErrorsChartDto { List<ChartDataPointDto> Data }` projecting `ErrorCode → Label`, `Count → Value`

### `AlfaCharge.Api/Services/IMetricsQueryService.cs`

- Change `GetErrorStatsAsync` signature from `Task<List<ErrorStatsDto>>` → `Task<ErrorsChartDto>`

### `AlfaCharge.Api/Services/MetricsQueryService.cs`

- After DTO rename — populate `.Data` in `GetSessionsChartAsync` and `GetEnergyChartAsync`
- Change `GetErrorStatsAsync` to return `ErrorsChartDto`, mapping `ErrorCode → Label`, `Count → Value`

### `AlfaCharge.Api/Controllers/MetricsController.cs`

- Change `GetErrorStats` action return type from `ActionResult<List<ErrorStatsDto>>` → `ActionResult<ErrorsChartDto>`

---

## Task 3 — Fix Admin-Side Field Names and URLs

### `AlfaCharge.Admin/Models/ViewModels.cs`

Three property name mismatches (Admin uses `PropertyNameCaseInsensitive = true` so names must align):

| Property | Current | Correct (matches API JSON) |
|---|---|---|
| `UserViewModel.LastLogin` | `LastLogin` | `LastLoginAt` |
| `RfidCardViewModel.LastUsed` | `LastUsed` | `LastUsedAt` |
| `RfidCardViewModel.LastUsedStation` | `LastUsedStation` | `LastUsedStationId` |

### `AlfaCharge.Admin/Components/Pages/Users/UsersList.razor`

- `context.LastLogin` → `context.LastLoginAt`

### `AlfaCharge.Admin/Components/Pages/Rfid/RfidList.razor`

- `context.LastUsed` → `context.LastUsedAt`
- `context.LastUsedStation` → `context.LastUsedStationId`

### `AlfaCharge.Admin/Services/LocationService.cs`

- `GetAllLocationsAsync()` currently calls `/api/location/Locations` (paged endpoint) but expects `List<LocationViewModel>` — deserialization failure
- Change URL to `/api/location/all` (new flat endpoint added in Task 1)

### `AlfaCharge.Admin/appsettings.Development.json`

- Add to avoid HTTPS dev-cert trust failures in server-to-server calls:

```json
{
  "ApiBaseUrl": "http://localhost:5100",
  "SignalRHubUrl": "http://localhost:5100/hub/ocpp"
}
```

---

## Task 4 — Build and Rerun

```powershell
dotnet build .\Alfacharge.sln -c Debug
```

Kill any existing API/Admin processes, then restart:

```powershell
# API (background)
dotnet run --project .\AlfaCharge.Api\AlfaCharge.Api.csproj --launch-profile https

# Admin (background)
dotnet run --project .\AlfaCharge.Admin\AlfaCharge.Admin.csproj --launch-profile https
```

Smoke tests:

```
GET http://localhost:5100/api/location/Locations?page=1&pageSize=5   → 200 + paged items
GET http://localhost:5100/api/location/all                           → 200 + flat list
GET http://localhost:5100/api/metrics/dashboard                      → 200 + activeStations, chargingSessionsToday
GET http://localhost:5100/api/metrics/sessions                       → 200 + data array (not hourlyData)
GET http://localhost:5100/api/metrics/errors                         → 200 + { data: [...] }
GET http://localhost:5122                                            → Admin dashboard loads with real data
```

---

## Task 5 — Commit and Push

After verifying everything runs:

```powershell
git add -A
git commit -m "fix: integrate Admin portal with real API endpoints"
git push origin main
```

---

## Known-Good Files (No Changes Needed)

- `AlfaCharge.Admin/Services/ApiClient.cs` — robust HTTP wrapper, correct JSON options
- `AlfaCharge.Admin/Services/StationService.cs` — routes match `AdminStationsController`
- `AlfaCharge.Admin/Services/UserService.cs` — routes match `UsersController`
- `AlfaCharge.Admin/Services/RfidService.cs` — routes match `RfidController`
- `AlfaCharge.Admin/Services/OcppLogClient.cs` — routes match `OcppLogsController`
- `AlfaCharge.Admin/Services/OcppAdminClient.cs` — routes match OCPP action endpoints
- `AlfaCharge.Admin/Services/MetricsService.cs` — URLs correct, mismatch was on DTO shape (fixed in Task 2)
- `AlfaCharge.Admin/Services/SessionsHubClient.cs` — reads `SignalRHubUrl` from config
- `AlfaCharge.Api/Controllers/AdminStationsController.cs` — all CRUD + OCPP actions correct
- `AlfaCharge.Api/Controllers/UsersController.cs` — correct
- `AlfaCharge.Api/Controllers/RfidController.cs` — correct
- `AlfaCharge.Api/Controllers/OcppLogsController.cs` — correct

---

## Reference: DbSet Names (`ApplicationDbContext`)

`AppUsers`, `RfidCards`, `ChargePoints`, `Locations`, `Connectors`, `OcppLogs`, `ChargingTransactions`, `OcppJobs`

## Reference: API Base URLs

- API: `http://localhost:5100` / `https://localhost:7188`
- Admin: `http://localhost:5122` / `https://localhost:7294`
- SignalR Hub: `/hub/ocpp`
