# AlfaCharge CSMS - Project Documentation

## Overview

AlfaCharge is a **Charging Station Management System (CSMS)** built on .NET that manages EV charging infrastructure using the OCPP (Open Charge Point Protocol) standard. The system supports both OCPP 1.6 and OCPP 2.0.1 protocols.

---

## Project Structure

```
D:\Projects\Alfacharge\
??? AlfaCharge.Domain/          # Domain entities, enums, and models
??? AlfaCharge.Infrastructure/  # Database context, services, repositories
??? AlfaCharge.Api/             # REST API for external integrations
??? AlfaCharge.Admin/           # Blazor Server Admin Portal (MudBlazor UI)
??? AlfaCharge.OcppServer/     # OCPP WebSocket server (1.6 & 2.0.1)
```

---

## What Has Been Completed

### Phase 1: Core OCPP Server Infrastructure ?

#### 1.1 WebSocket Server
- **OcppConnectionManager** - Manages active WebSocket connections
- **OcppWebSocketMiddleware** - Handles WebSocket upgrade and routing
- Protocol version detection (OCPP 1.6 / 2.0.1)
- Message routing and dispatching

#### 1.2 OCPP 1.6 Message Handlers
| Handler | Status | Description |
|---------|--------|-------------|
| BootNotification | ? | Charge point registration |
| Heartbeat | ? | Keep-alive mechanism |
| StatusNotification | ? | Connector status updates |
| Authorize | ? | RFID/token authorization |
| StartTransaction | ? | Charging session start |
| StopTransaction | ? | Charging session end |
| MeterValues | ? | Energy consumption data |
| DataTransfer | ? | Vendor-specific messaging |
| DiagnosticsStatusNotification | ? | Diagnostics upload status |
| FirmwareStatusNotification | ? | Firmware update status |

#### 1.3 OCPP 2.0.1 Message Handlers
| Handler | Status | Description |
|---------|--------|-------------|
| BootNotification | ? | Charge point registration |
| Heartbeat | ? | Keep-alive mechanism |
| StatusNotification | ? | Connector status updates |
| Authorize | ? | Token authorization |
| TransactionEvent | ? | Transaction lifecycle events |
| MeterValues | ? | Energy consumption data |

---

### Phase 2: Domain Entities & Database ?

#### 2.1 Core Entities
| Entity | Description |
|--------|-------------|
| ChargePoint | Charging station registration data |
| Connector | Individual charging connectors |
| Location | Physical location of charge points |
| ChargingTransaction | Charging session records |
| IdTag / RfidCard | Authorization tokens |
| OcppLog | OCPP message audit trail |
| OcppJob | Async job queue (firmware, diagnostics) |
| MeterValue / SampledValue | Energy metering data |

#### 2.2 Enums
- `ConnectorStatus` - Available, Charging, Unavailable, Faulted
- `ConnectorType` - Type1, Type2, CCS, CHAdeMO, etc.
- `OcppJobType` - FirmwareUpdate16, Diagnostics16, Log201, FirmwareUpdate201
- `OcppJobStatus` - Created, Accepted, Running, Succeeded, Failed, Rejected, Canceled
- `Standard` - OCPP connector standards

#### 2.3 Infrastructure Services
| Service | Description |
|---------|-------------|
| BootNotificationService | Handle charge point registration |
| LocationServices | CRUD for locations |
| ApplicationDbContext | EF Core database context |

---

### Phase 3: REST API (AlfaCharge.Api) ?

#### 3.1 Controllers
| Controller | Endpoints | Description |
|------------|-----------|-------------|
| ChargePointController | GET, POST /api/cp | Connected charge points, OCPP commands |
| LocationController | CRUD /api/location | Location management |
| MetricsController | GET /api/metrics/* | Dashboard metrics & charts |
| StationController | CRUD /api/station | Station management |
| UserController | CRUD /api/user | User management |
| RfidController | CRUD /api/rfid | RFID card management |
| OcppLogController | GET /api/ocpp-logs | OCPP message logs |

#### 3.2 OCPP Remote Commands (via ChargePointController)
| Command | OCPP 1.6 | OCPP 2.0.1 |
|---------|----------|------------|
| Ping (DataTransfer) | ? | ? |
| ChangeAvailability | ? | ? |
| Reset | ? | ? |
| RemoteStart | ? | ? |
| RemoteStop | ? | ? |

#### 3.3 DTOs
- `StationDtos` - Station list/detail DTOs
- `LocationDtos` - Location DTOs
- `UserDtos` - User management DTOs
- `RfidDtos` - RFID card DTOs
- `OcppActionDtos` - Remote command DTOs
- `MetricsDtos` - Dashboard metrics DTOs
- `PagingQueryDto` - Pagination support

---

### Phase 4: Admin Portal (AlfaCharge.Admin) ?

#### 4.1 Technology Stack
- **Blazor Server** (.NET 10)
- **MudBlazor** UI Component Library
- **HttpClient** for API communication

#### 4.2 Pages Implemented
| Module | Pages | Features |
|--------|-------|----------|
| Dashboard | Home.razor | KPI cards, charts (sessions, energy, OCPP traffic, errors) |
| Stations | StationsList, StationDetail, StationEdit, StationActions | List, CRUD, remote commands |
| Locations | LocationsList, LocationEdit | List, CRUD |
| Users | UsersList, UserEdit | List, CRUD, password reset |
| RFID | RfidList, RfidEdit | List, CRUD |
| OCPP Logs | OcppLogs | Log viewer with filters |

#### 4.3 Shared Components
| Component | Description |
|-----------|-------------|
| ConfirmDialog | Confirmation dialogs |
| RemoteStartDialog | Remote start transaction dialog |
| RemoteStopDialog | Remote stop transaction dialog |
| ResetPasswordDialog | User password reset dialog |
| OcppLogDetailDialog | OCPP log JSON viewer |
| NavMenu | Navigation sidebar |
| MainLayout | Main application layout |

#### 4.4 Services (Client-side)
| Service | Description |
|---------|-------------|
| ApiClient | Base HTTP client with error handling |
| StationService | Station API calls |
| LocationService | Location API calls |
| UserService | User API calls |
| RfidService | RFID card API calls |
| OcppLogClient | OCPP logs API calls |
| OcppAdminClient | OCPP remote commands |
| MetricsService | Dashboard metrics API calls |
| SessionsHubClient | SignalR for live sessions |

#### 4.5 Models
- ViewModels for UI display
- PagingModels for pagination
- DashboardModels for metrics
- ApiResult for API responses
- OcppActionModels for commands

---

### Phase 5: Bug Fixes & Configuration ?

#### 5.1 Compilation Fixes
- Fixed sealed class inheritance issues (StationListDto, PagingRequest, StationViewModel)
- Removed duplicate DTO files (RemoteStartDto, RemoteStopDto)
- Fixed MudBlazor dialog type (`IMudDialogInstance` ? `MudDialogInstance`)
- Fixed enum comparisons in MetricsController
- Fixed ambiguous type references
- Fixed missing MudBlazor icon (`Monitoring` ? `Analytics`)

#### 5.2 Configuration Fixes
- Updated Admin portal API base URL (7001 ? 7188)
- Removed duplicate MudBlazor providers from App.razor

---

## Configuration

### AlfaCharge.Admin/appsettings.json
```json
{
  "ApiBaseUrl": "https://localhost:7188",
  "SignalRHubUrl": "https://localhost:7188/hub/ocpp"
}
```

### API Launch Settings
- **HTTPS**: `https://localhost:7188`
- **HTTP**: `http://localhost:5100`

---

## Running the Application

### Prerequisites
1. .NET 8 or .NET 10 SDK
2. SQL Server (or configured database)
3. Visual Studio 2022+

### Multi-Project Startup
1. Right-click Solution ? **Set Startup Projects...**
2. Select **Multiple startup projects**
3. Set both `AlfaCharge.Api` and `AlfaCharge.Admin` to **Start**
4. Press F5

---

## Future Development Plans

### Phase 6: Enhanced OCPP Commands ??

#### 6.1 OCPP 1.6 Commands to Implement
| Command | Priority | Description |
|---------|----------|-------------|
| GetConfiguration | High | Read charge point configuration |
| ChangeConfiguration | High | Modify charge point settings |
| ClearCache | Medium | Clear authorization cache |
| UnlockConnector | High | Emergency connector unlock |
| GetDiagnostics | Medium | Request diagnostics upload |
| UpdateFirmware | Medium | Initiate firmware update |
| TriggerMessage | Medium | Request specific messages |
| ReserveNow | Low | Make a reservation |
| CancelReservation | Low | Cancel reservation |
| SendLocalList | Low | Update local auth list |
| GetLocalListVersion | Low | Check auth list version |
| SetChargingProfile | Medium | Smart charging profiles |
| ClearChargingProfile | Medium | Remove charging profiles |
| GetCompositeSchedule | Low | Get charging schedule |

#### 6.2 OCPP 2.0.1 Commands to Implement
| Command | Priority | Description |
|---------|----------|-------------|
| GetVariables | High | Read device variables |
| SetVariables | High | Set device variables |
| GetBaseReport | Medium | Full device configuration |
| SetNetworkProfile | Medium | Network configuration |
| ClearCache | Medium | Clear caches |
| UnlockConnector | High | Unlock connector |
| TriggerMessage | Medium | Request messages |
| GetLog | Medium | Request log upload |
| UpdateFirmware | Medium | Firmware update |
| PublishFirmware | Medium | Publish to local controller |
| ReserveNow | Low | Reserve EVSE |
| CancelReservation | Low | Cancel reservation |
| RequestStartTransaction | High | Already implemented |
| RequestStopTransaction | High | Already implemented |
| SetChargingProfile | Medium | Smart charging |
| GetChargingProfiles | Medium | Get active profiles |
| ClearChargingProfile | Medium | Remove profiles |
| GetTransactionStatus | Low | Check transaction status |
| CostUpdated | Low | Display cost updates |
| CustomerInformation | Low | Customer data request |

---

### Phase 7: Smart Charging ??

#### 7.1 Features
- **Load Balancing** - Distribute power across connectors
- **Time-of-Use Pricing** - Schedule charging for off-peak hours
- **Solar Integration** - Charge when renewable energy available
- **Fleet Management** - Priority charging for fleet vehicles
- **Demand Response** - Grid operator load shedding

#### 7.2 Database Entities
```
ChargingProfile
??? Id
??? ChargePointId
??? StackLevel
??? ChargingProfilePurpose (ChargePointMaxProfile, TxDefaultProfile, TxProfile)
??? ChargingProfileKind (Absolute, Recurring, Relative)
??? RecurrencyKind (Daily, Weekly)
??? ValidFrom / ValidTo
??? ChargingSchedule[]

ChargingSchedule
??? Id
??? ChargingProfileId
??? Duration
??? StartSchedule
??? ChargingRateUnit (W, A)
??? ChargingSchedulePeriod[]

ChargingSchedulePeriod
??? StartPeriod
??? Limit
??? NumberPhases
```

#### 7.3 Admin UI
- Charging profile editor
- Schedule visualization (Gantt chart)
- Load monitoring dashboard
- Power distribution charts

---

### Phase 8: Reservations System ??

#### 8.1 Features
- Create/cancel reservations via Admin portal
- Mobile app reservation support
- Reservation expiry handling
- Conflict detection

#### 8.2 Database Entities
```
Reservation
??? Id
??? ChargePointId
??? ConnectorId
??? ExpiryDateTime
??? IdTag
??? ReservationId (OCPP)
??? Status (Pending, Active, Used, Cancelled, Expired)
??? CreatedAt
??? UsedAt
```

#### 8.3 Admin UI
- Reservation calendar view
- Create/cancel reservations
- Reservation history

---

### Phase 9: Billing & Payments ??

#### 9.1 Features
- **Tariff Management** - Per-kWh, per-minute, flat fee, time-of-use
- **CDR Generation** - Charge Detail Records per OCPI
- **Invoice Generation** - PDF invoices
- **Payment Integration** - Stripe, PayPal, etc.
- **Prepaid Balance** - User wallet system

#### 9.2 Database Entities
```
Tariff
??? Id
??? Name
??? Currency
??? Elements[] (energy, time, flat, parking)
??? ValidFrom / ValidTo
??? Restrictions (day_of_week, min_kwh, etc.)

CDR (Charge Detail Record)
??? Id
??? TransactionId
??? StartDateTime / EndDateTime
??? TotalEnergy
??? TotalTime
??? TotalParkingTime
??? TariffId
??? TotalCost
??? Currency
??? ChargingPeriods[]

Invoice
??? Id
??? UserId
??? InvoiceNumber
??? IssuedDate
??? DueDate
??? TotalAmount
??? Status (Draft, Sent, Paid, Overdue)
??? LineItems[]

UserWallet
??? UserId
??? Balance
??? Currency
??? Transactions[]
```

#### 9.3 Admin UI
- Tariff editor
- CDR viewer/export
- Invoice management
- Payment history
- Revenue dashboard

---

### Phase 10: OCPI Integration ??

**OCPI (Open Charge Point Interface)** enables roaming between different CPO (Charge Point Operator) networks.

#### 10.1 OCPI Modules
| Module | Description |
|--------|-------------|
| Credentials | Authentication between parties |
| Locations | Share location/EVSE data |
| Sessions | Real-time session sharing |
| CDRs | Charge detail records |
| Tariffs | Pricing information |
| Tokens | Authorization tokens |
| Commands | Remote commands (START_SESSION, etc.) |

#### 10.2 Features
- **Hub Connection** - Connect to OCPI hubs (Hubject, e-clearing, etc.)
- **Peer-to-Peer** - Direct CPO connections
- **eMSP Role** - Act as e-Mobility Service Provider
- **CPO Role** - Act as Charge Point Operator

---

### Phase 11: Mobile App API ??

#### 11.1 Endpoints
| Endpoint | Description |
|----------|-------------|
| POST /api/mobile/auth/login | User login |
| POST /api/mobile/auth/register | User registration |
| GET /api/mobile/locations | Find nearby chargers |
| GET /api/mobile/locations/{id} | Location details |
| POST /api/mobile/sessions/start | Start charging |
| POST /api/mobile/sessions/stop | Stop charging |
| GET /api/mobile/sessions/active | Current session |
| GET /api/mobile/sessions/history | Past sessions |
| GET /api/mobile/wallet/balance | Wallet balance |
| POST /api/mobile/wallet/topup | Add funds |
| GET /api/mobile/invoices | User invoices |

#### 11.2 Features
- JWT authentication
- Push notifications (FCM/APNS)
- QR code scanning for stations
- Real-time session updates via SignalR
- Apple Pay / Google Pay integration

---

### Phase 12: Analytics & Reporting ??

#### 12.1 Dashboards
- **Operations Dashboard** - Real-time station status
- **Revenue Dashboard** - Income by station/location/time
- **Utilization Dashboard** - Usage patterns, peak hours
- **Reliability Dashboard** - Uptime, error rates

#### 12.2 Reports
| Report | Description |
|--------|-------------|
| Station Utilization | Usage % by station/connector |
| Energy Consumption | kWh by period/location |
| Revenue Report | Income breakdown |
| Error Report | Failures and error codes |
| User Activity | Sessions per user |
| Peak Hours | Busiest charging times |

#### 12.3 Export Formats
- PDF reports
- Excel/CSV export
- Scheduled email reports

---

### Phase 13: Notifications & Alerts ??

#### 13.1 Alert Types
| Alert | Trigger |
|-------|---------|
| Station Offline | No heartbeat for X minutes |
| Connector Fault | Status = Faulted |
| Transaction Anomaly | Unusual energy consumption |
| Low Utilization | Station idle for X hours |
| Firmware Available | New firmware detected |
| Certificate Expiry | SSL/TLS cert expiring |

#### 13.2 Notification Channels
- Email
- SMS
- Push notification
- Slack/Teams webhook
- In-app notifications

#### 13.3 Admin UI
- Alert rules configuration
- Notification preferences
- Alert history

---

### Phase 14: Multi-Tenancy ??

#### 14.1 Features
- **Tenant Isolation** - Separate data per tenant
- **White-Label** - Custom branding per tenant
- **Role-Based Access** - Tenant admin, operator, viewer
- **Tenant Dashboard** - Dedicated dashboards

#### 14.2 Architecture
```
Tenant
??? Id
??? Name
??? Subdomain
??? Branding (logo, colors)
??? Settings
??? Users[]

All entities include:
??? TenantId (FK)
```

---

### Phase 15: Advanced Security ??

#### 15.1 Features
- **ISO 15118** - Plug & Charge support
- **Certificate Management** - Automated cert rotation
- **OCPP Security Profiles** - Profile 1, 2, 3
- **Audit Logging** - All admin actions logged
- **MFA** - Multi-factor authentication
- **API Key Management** - For integrations

---

### Phase 16: Hardware Integration ??

#### 16.1 Direct Integrations
| Vendor | Protocol |
|--------|----------|
| ABB | OCPP + Vendor extensions |
| ChargePoint | OCPP + Proprietary |
| EVBox | OCPP |
| Schneider Electric | OCPP |
| Siemens | OCPP |
| Tritium | OCPP |
| Delta | OCPP |

#### 16.2 Features
- Vendor-specific DataTransfer handling
- Firmware repository management
- Device provisioning workflow
- Hardware inventory management

---

## Technology Recommendations

### Current Stack
| Layer | Technology |
|-------|------------|
| Frontend | Blazor Server + MudBlazor |
| API | ASP.NET Core Web API |
| Database | SQL Server / PostgreSQL |
| ORM | Entity Framework Core |
| Real-time | SignalR |
| OCPP | Custom WebSocket implementation |

### Recommended Additions
| Need | Technology |
|------|------------|
| Caching | Redis |
| Message Queue | RabbitMQ / Azure Service Bus |
| Search | Elasticsearch |
| Monitoring | Application Insights / Prometheus + Grafana |
| CI/CD | Azure DevOps / GitHub Actions |
| Container | Docker + Kubernetes |
| API Gateway | Azure API Management / Kong |

---

## Development Priorities

### Immediate (Next Sprint)
1. ? Validate Admin Portal functionality
2. Add missing OCPP commands (GetConfiguration, ChangeConfiguration)
3. Implement UnlockConnector
4. Add real-time station status updates via SignalR

### Short-term (1-2 months)
1. Smart charging profiles
2. Reservation system
3. Basic tariff management
4. CDR generation

### Medium-term (3-6 months)
1. OCPI integration
2. Mobile app API
3. Billing & payments
4. Advanced analytics

### Long-term (6-12 months)
1. Multi-tenancy
2. ISO 15118 Plug & Charge
3. White-label solution
4. Enterprise features

---

## Contact & Resources

### OCPP Resources
- [Open Charge Alliance](https://www.openchargealliance.org/)
- [OCPP 1.6 Specification](https://www.openchargealliance.org/protocols/ocpp-16/)
- [OCPP 2.0.1 Specification](https://www.openchargealliance.org/protocols/ocpp-201/)

### OCPI Resources
- [OCPI Protocol](https://evroaming.org/ocpi/)
- [OCPI GitHub](https://github.com/ocpi/ocpi)

---

*Last Updated: January 2026*
*Version: 1.0*
