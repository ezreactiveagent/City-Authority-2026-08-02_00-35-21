# Service Coverage and Response Model

## 1. Purpose

[03 §13–§14](03_Departments_Emergency_Management_and_Courts.md) and [07 §4](07_Open_Decisions_and_Expansion_Backlog.md) describe service coverage qualitatively (Adequate / Reduced / Uncovered, driven by travel time and physical unit commitment) but leave the numbers open. This report proposes a first-pass numeric model, scoped specifically to unblock the vertical slice in [08](08_Vertical_Slice_Specification.md).

These are starting defaults for playtesting, not final balance. Every number here should be treated as a tunable constant, not a rule.

## 2. Inputs

For a given department (Fire or Police, for the slice), coverage is computed per district or per point of interest from:

- **Travel time**: road-network distance from the nearest available unit's current position to the location, using the existing road graph rather than straight-line distance.
- **Unit availability**: whether a unit is uncommitted, committed-and-reassignable, or committed-and-unavailable per [03 §13](03_Departments_Emergency_Management_and_Courts.md).
- **Concurrent demand**: number of open calls currently competing for the same department's units.

## 3. Travel-Time Bands

Proposed default bands, in in-game minutes of travel time from the nearest uncommitted unit:

| Band | Travel Time |
|---|---|
| Adequate | 0–6 minutes |
| Reduced | 6–12 minutes |
| Uncovered | over 12 minutes, or no uncommitted unit exists |

These bands apply per department type. Fire and Police may use the same bands by default; a department-specific multiplier can be introduced later if playtesting shows one service needs tighter tolerances than the other.

## 4. Coverage State Resolution

For a given district, coverage state is the worse of:

1. The travel-time band from Section 3, evaluated against the nearest currently uncommitted unit.
2. A concurrent-demand check: if the number of open calls in or adjacent to the district exceeds the number of uncommitted units able to reach it within the Reduced band, downgrade one level (Adequate → Reduced, Reduced → Uncovered).

This means a district can be geographically inside the Adequate travel-time band and still show Reduced coverage if every nearby unit is already committed elsewhere — which is the mechanic [03 §14](03_Departments_Emergency_Management_and_Courts.md) calls for ("best-effort rather than guaranteeing perfect protection").

## 5. Response-Time Penalties

When an incident is actually dispatched, the resolved travel time (not just the band) determines outcome severity scaling:

- Under 6 minutes: baseline severity.
- 6–12 minutes: severity multiplier of approximately **1.25x** (proposed default) to losses/damage/risk calculated for that incident.
- Over 12 minutes: severity multiplier of approximately **1.6x** (proposed default).

These multipliers apply to whatever downstream loss calculation the incident type already defines (property damage, life-safety risk, etc.) — this report does not define those base values, only the coverage-driven multiplier applied to them.

## 6. Cross-Station Exposure

When a unit is reassigned outside its home district (per [03 §14](03_Departments_Emergency_Management_and_Courts.md)):

- The home district's coverage is recalculated immediately using Section 4, excluding the reassigned unit from the available pool.
- If this recalculation drops the home district to Uncovered, an Uncovered warning notification is generated for that district per [03 §5](03_Departments_Emergency_Management_and_Courts.md), separate from any notification about the original incident.
- The reassignment cannot be cancelled once the unit is en route, per the existing design rule in [03 §14](03_Departments_Emergency_Management_and_Courts.md); this report does not change that.

## 7. Slice-Specific Application

For the vertical slice ([08 §6](08_Vertical_Slice_Specification.md)), the scripted emergency should be sized so that:

- The primary incident location resolves to Adequate coverage if the department's single unit responds directly.
- Dispatching that unit drops at least one other district to Reduced or Uncovered per Section 6, producing the required coverage-effect warning.

This is the minimum configuration needed to demonstrate the tradeoff mechanic without requiring multiple stations or a citywide simulation.

## 8. Open Follow-Ups

Not required for the slice, but flagged for later tuning passes:

- Whether travel-time bands should vary by region preset (a rural region's "Adequate" may need a wider band than a dense urban core).
- Whether Police and Fire should use different band widths.
- How hospital and utility integration (flagged as open in [07 §2](07_Open_Decisions_and_Expansion_Backlog.md)) feeds into the same coverage model.
- Whether the concurrent-demand downgrade in Section 4 should scale by call severity rather than raw call count.
