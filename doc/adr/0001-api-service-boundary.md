# ADR 0001: API Service Boundary Standard

## Status
Accepted

## Context
Controllers in `AlfaCharge.Api` used mixed patterns: direct `ApplicationDbContext` access and service abstractions. This created inconsistent layering and reduced testability.

## Decision
Adopt a standard where controller actions delegate business/query logic to application services. Controllers should handle HTTP concerns only (routing, validation, status codes, response shaping).

## Consequences
- Positive: improved testability, clearer boundaries, easier refactoring.
- Negative: more service classes and DI registrations.
- Migration: perform incrementally controller-by-controller.

## Initial Implementation
`MetricsController` now delegates to `IMetricsQueryService` / `MetricsQueryService`.
