# ADR 0004: Mobile Integration Mode

## Status
Proposed

## Context
`AlfaGrid` currently mixes local JSON-driven flows with framework networking abstractions. This creates ambiguity around whether it is demo-first or backend-integrated.

## Decision
Short term: preserve current local-data behavior.
Mid term: align key journeys to backend APIs and retire unused generic networking abstractions if not needed.

## Consequences
- Positive: avoids accidental production assumptions.
- Negative: temporary dual-mode complexity.
