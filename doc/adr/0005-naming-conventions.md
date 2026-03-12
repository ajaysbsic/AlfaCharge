# ADR 0005: Naming Convention Normalization

## Status
Proposed

## Context
Codebase currently contains inconsistent prefixes (`AlfaCharge` vs `AlphaCharge`) and folder naming patterns (`Versioned Handlers` vs `Versioned_Handlers`).

## Decision
Normalize naming in a staged migration:
1. Freeze new additions to canonical names.
2. Rename folders/namespaces with compatibility shims where required.
3. Remove shims after one release cycle.

## Consequences
- Positive: lower maintenance friction and fewer navigation errors.
- Negative: temporary churn in refs and PR size.
