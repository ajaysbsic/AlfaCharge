# AlfaCharge

AlfaCharge is an EV charging platform built with .NET, combining:
- Backend API and OCPP runtime (`AlfaCharge.Api` + `AlfaCharge.OcppServer`)
- Admin portal (`AlfaCharge.Admin`)
- Domain and Infrastructure libraries (`AlfaCharge.Domain`, `AlfaCharge.Infrastructure`)
- Mobile app (`AlfaGrid`, .NET MAUI)

## Solution Structure

- `Alfacharge.sln`: Primary solution file
- `AlfaCharge.Api/`: ASP.NET Core Web API host and OCPP runtime integration
- `AlfaCharge.OcppServer/`: OCPP protocol server library and handlers
- `AlfaCharge.Admin/`: Blazor Server admin web app
- `AlfaCharge.Domain/`: Domain entities/models/contracts
- `AlfaCharge.Infrastructure/`: EF Core database, services, middleware
- `AlfaGrid/`: .NET MAUI mobile application (Android/Windows targets)
- `doc/`: Architecture analysis, roadmap, and ADRs
- `eng/`: Engineering scripts (including architecture guardrails)

## Prerequisites

- .NET SDK 10.0 (for Admin and MAUI projects)
- .NET SDK 8.0 (for API/OCPP projects)
- Visual Studio 2022/2026 with:
  - ASP.NET and web development workload
  - .NET Multi-platform App UI (MAUI) workload
  - Android SDK + emulator (for mobile testing)
- SQL Server instance configured via `appsettings.Development.json`

## Restore and Build

```powershell
dotnet restore .\Alfacharge.sln
dotnet build .\Alfacharge.sln -c Debug
```

## Run Components

### 1. Backend API + OCPP runtime

`AlfaCharge.OcppServer` is hosted inside the API process.

```powershell
dotnet run --project .\AlfaCharge.Api\AlfaCharge.Api.csproj --launch-profile https
```

Default endpoints:
- `https://localhost:7188`
- `http://localhost:5100`

Swagger:
- `https://localhost:7188/swagger`
- `http://localhost:5100/swagger`

### 2. Admin Portal

```powershell
dotnet run --project .\AlfaCharge.Admin\AlfaCharge.Admin.csproj --launch-profile https
```

Default endpoints:
- `https://localhost:7294`
- `http://localhost:5122`

### 3. Mobile App (Android)

```powershell
dotnet build .\AlfaGrid\AlfaGrid.csproj -t:Run -f net10.0-android
```

## Architecture Guardrails

Run the repository guardrail checks:

```powershell
.\eng\architecture-guardrails.ps1
```

## Documentation

- `doc/Architecture-Codebase-Analysis.md`
- `doc/Architecture-Remediation-Roadmap.md`
- `doc/adr/` (architecture decisions)
- `OCPP_IMPLEMENTATION_GUIDE.md`
- `CSMS_PROJECT_DOCUMENTATION.md`

## Notes

- Keep project naming consistent with `AlfaCharge.*`.
- Avoid introducing references that violate layer boundaries; use `eng/architecture-guardrails.ps1` and CI checks.
