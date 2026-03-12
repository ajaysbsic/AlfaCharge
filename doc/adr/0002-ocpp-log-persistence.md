# ADR 0002: OCPP Log Persistence Strategy

## Status
Accepted

## Context
Previous OCPP logging wrote each frame immediately with `SaveChangesAsync`, increasing DB write pressure and latency under high message throughput.

## Decision
Use a buffered writer with background batch flush.

## Consequences
- Positive: lower write amplification, less protocol-path DB contention.
- Negative: in-memory buffer introduces small risk window on abrupt process termination.
- Mitigation: short flush interval and shutdown drain logic.

## Initial Implementation
`BatchedOcppLogWriter` introduced in API and registered as `IOcppLogWriter` + hosted service.
