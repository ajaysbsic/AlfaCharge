# Architecture Remediation Roadmap

## Objective
Stabilize runtime behavior, reduce architectural coupling, and establish maintainable growth paths across API, OCPP server, admin portal, and mobile app.

## Prioritization Model
- `P0`: Production stability and correctness
- `P1`: Maintainability and architectural consistency
- `P2`: Scalability, hardening, and developer productivity

## Phase 1 - Runtime Stability (P0)

### 1. Fix DI composition gaps in API startup
Ensure all required interfaces are registered in `AlfaCharge.Api/Program.cs` for classes currently resolved by interface in controllers/factories.

Candidate registrations to verify/add:
- `IStatusNotificationHandler`
- `IOcpp16TransactionHandler`
- `IOcpp201TransactionHandler`
- `IConfigurationOps16`, `IConfigurationOps201`
- `IDiagnosticsFirmwareOps16`, `IDiagnosticsFirmwareOps201`
- `IOcppHandlerFactory` (or refactor controllers to avoid direct factory dependency)

Acceptance criteria:
- Application starts without DI resolution exceptions.
- OCPP frame routing paths execute end-to-end for supported actions.

### 2. Enable SignalR hub endpoint mapping
Add explicit hub mapping to expose the endpoint expected by admin (`/hub/ocpp` or configured equivalent).

Acceptance criteria:
- `SessionsHubClient` can connect successfully.
- Live events appear in admin for station/session status.

### 3. Remove or isolate placeholder endpoints
Address template/stub endpoints in:
- `DashboardController`
- `OCPPController`

Acceptance criteria:
- Either fully implemented or marked internal/non-routable/deleted.
- Public API surface contains only supported operations.

## Phase 2 - API Architecture Convergence (P1)

### 4. Standardize service boundary in API controllers
Current state is mixed (direct `DbContext` in some controllers, service abstractions in others).

Recommended target:
- Controllers depend only on application service interfaces.
- Business logic and query composition moved into service layer.

Suggested service partition:
- `IStationAdminService`
- `IUserAdminService`
- `IRfidAdminService`
- `IMetricsQueryService`
- `IOcppLogQueryService`

Acceptance criteria:
- Controllers become thin (validation + response shape only).
- Query/update logic testable without controller harness.

### 5. Complete or retire incomplete infrastructure services
`StationServices` and `OCPPServices` currently throw `NotImplementedException`.

Acceptance criteria:
- Methods implemented with integration tests, or interfaces removed if obsolete.
- No reachable endpoint depends on non-implemented method.

### 6. Normalize DTO strategy
Currently API DTOs and Admin view models are nearly duplicate structures.

Recommended options:
- Option A: Keep separate contracts but enforce explicit mapping and compatibility tests.
- Option B: Introduce shared contracts package for admin/API transport models.

Acceptance criteria:
- Breaking contract changes are detected in CI (snapshot or schema tests).

## Phase 3 - OCPP Domain Hardening (P1/P2)

### 7. Introduce OCPP command pipeline abstraction
Wrap direct `SendCallAsync` orchestration into a cohesive command service with:
- timeout policy
- retries where safe
- response normalization
- centralized error mapping

Acceptance criteria:
- Command behavior consistent across `ChargePointController` and admin action paths.

### 8. Improve OCPP log write throughput
Current `EfOcppLogWriter` writes each frame with immediate `SaveChangesAsync`.

Recommended approaches:
- buffered channel + background batch flush
- or batched unit-of-work per frame burst

Acceptance criteria:
- reduced DB write amplification
- no message loss under peak traffic test

### 9. Split protocol module from persistence concerns
Longer-term, reduce direct Infrastructure dependency from OCPP core.

Target:
- OCPP handlers depend on persistence interfaces, not concrete `ApplicationDbContext`.
- Persistence implementation remains in Infrastructure.

Acceptance criteria:
- OCPP core testable with in-memory mocks.

## Phase 4 - Naming, Structure, and Governance (P1)

### 10. Resolve naming and folder inconsistencies
Issues:
- `AlfaCharge` vs `AlphaCharge` project/namespace prefix split
- duplicate style: `Versioned Handlers` and `Versioned_Handlers`

Acceptance criteria:
- canonical naming standard documented
- folder/namespace alignment complete

### 11. Add architecture guardrails in CI
Add checks for:
- forbidden project reference directions
- no `NotImplementedException` in production paths
- optional namespace/folder linting

Acceptance criteria:
- CI fails on boundary violations.

## Phase 5 - Security and Configuration Hygiene (P0/P1)

### 12. Externalize sensitive mobile configuration
Some environment files contain hardcoded secrets/tokens and placeholder URLs.

Actions:
- move secrets to secure provider per environment
- rotate compromised values
- enforce `*.example` pattern for committed config

Acceptance criteria:
- no secrets in repository
- secure config loading documented

### 13. Add API auth/authz policy and endpoint review
`UseAuthorization` is present but policy model appears incomplete.

Actions:
- define authentication scheme
- add role/policy attributes for admin endpoints
- verify token issuance/validation path

Acceptance criteria:
- role-segregated access to admin operations
- unauthorized access tests in CI

## Phase 6 - Mobile Integration Alignment (P2)

### 14. Decide mobile integration target model
Current mobile app uses local JSON for key flows while framework contains API repository tooling.

Decision options:
- keep mock-first app for demos only
- or integrate with backend endpoints and remove dead network abstractions

Acceptance criteria:
- documented intended mode
- implementation aligned to decision

### 15. Rationalize framework networking stack
If backend integration is desired:
- validate Refit interfaces against current API routes
- remove legacy headers/assumptions not used by current backend

Acceptance criteria:
- end-to-end mobile API calls working in at least one environment.

## Execution Plan (Suggested)

### Sprint 1
- Phase 1 items 1-3
- Phase 5 item 12 (urgent secret hygiene)

### Sprint 2
- Phase 2 items 4-6
- Phase 5 item 13

### Sprint 3
- Phase 3 items 7-8
- Phase 4 item 10

### Sprint 4
- Phase 3 item 9
- Phase 4 item 11
- Phase 6 items 14-15

## Suggested KPIs
- API startup failures due to DI: `0`
- OCPP command success/error observability: `100%` actions logged with correlation IDs
- Mean DB writes per OCPP frame: reduced by target baseline percent
- Controller unit test coverage for admin paths: >= target threshold
- Secrets in repo scan findings: `0`

## Recommended ADR Topics
Create ADRs for:
1. Service-layer boundary standard in API
2. OCPP logging persistence model (sync vs buffered)
3. Shared contracts strategy for Admin/API
4. Mobile integration mode (mock-first vs backend-integrated)
5. Naming convention normalization (`Alfa` vs `Alpha`)
