# ADR 0003: Admin/API Contract Strategy

## Status
Proposed

## Context
Admin view models and API DTOs have substantial shape overlap, which increases contract drift risk.

## Decision
Maintain separate API DTO and Admin view model types for now, but enforce explicit mapping and add compatibility tests in CI.

## Consequences
- Positive: UI autonomy preserved.
- Negative: duplicate model evolution cost.
- Future: re-evaluate shared contracts package if churn remains high.
