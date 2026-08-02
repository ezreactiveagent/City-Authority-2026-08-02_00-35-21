# Vertical Slice Data Defaults

## 1. Purpose

[08 — Vertical Slice Specification, Section 12](08_Vertical_Slice_Specification.md#12-systems-the-slice-still-needs-numbers-for) identifies two remaining numeric gaps beyond Service Coverage (resolved in [09](09_Service_Coverage_and_Response_Model.md)): a Developer Interest placeholder constant and a Court Outcomes penalty range. This report fixes both, and supplies the concrete department and region data needed for [08 §14](08_Vertical_Slice_Specification.md#14-suggested-build-order) Step 1 ("Fixed region + department data — no gameplay yet, just data existing").

As with Report 09, these are starting defaults for playtesting, not final balance. Every number here is a tunable constant, not a rule.

## 2. Developer Interest Defaults

Per [02 §6](02_Land_Development_Housing_and_Infrastructure.md), rejecting all proposals for a listed parcel reduces property-specific developer interest, which can later recover. [08 §4](08_Vertical_Slice_Specification.md#4-development-system-limited) only requires this to function for the slice's single rejection path — no incentive packages, no relisting-loop tuning.

- **Scale**: Developer Interest is a 0–100 score, property-specific, starting at 100 when a parcel is first listed.
- **Rejection penalty**: rejecting both proposals applies a flat **-40** (proposed default).
- **Recovery**: **+5 per in-game week** while the parcel remains listed and dormant, flat and passive. The richer recovery model in [07 §4](07_Open_Decisions_and_Expansion_Backlog.md) (city growth, infrastructure, reputation, incentives) is not required for the slice.
- **Floor**: interest cannot drop below **10**, so a rejected parcel is never permanently dead — this only matters if the slice is replayed or extended past the single scenario in its Definition of Done.

## 3. Court Outcome Penalty Range

The slice's one case type is the emergency condemnation / demolition payment dispute ([02 §13](02_Land_Development_Housing_and_Infrastructure.md), [03 §21](03_Departments_Emergency_Management_and_Courts.md)), arising directly from the scripted emergency's condemnation per [08 §7](08_Vertical_Slice_Specification.md#7-court-case).

Per [03 §21](03_Departments_Emergency_Management_and_Courts.md), the engine defines valid outcomes and a penalty range; the judge's AI personality selects and explains within them, per the restricted-outcome pattern in [06 §4–§5](06_AI_LLM_and_Simulation_Architecture.md). Cost basis is the condemned structure's existing assessed demolition/rebuild value (already tracked via construction value, [01 §10](01_Game_Vision_and_Core_Structure.md)) — call it **C**. This report does not redefine C, only the share of it each outcome assigns.

| Outcome | City Share of C | Owner/Developer Share of C |
|---|---|---|
| City pays | 100% | 0% |
| Owner/developer pays | 0% | 100% |
| Split responsibility | 25% / 50% / 75% (three fixed increments only) | remainder |
| Initial assignment upheld | Whatever the Emergency Commander assigned at condemnation time ([02 §13](02_Land_Development_Housing_and_Infrastructure.md)) stands unchanged | — |

Restricting "Split" to three fixed increments (25/75, 50/50, 75/25) rather than a continuous range keeps the ruling auditable and reproducible per [06 §9](06_AI_LLM_and_Simulation_Architecture.md) — the judge's personality and the case facts influence which of these four outcomes (and which split increment) is chosen, not an open-ended percentage.

## 4. Department Roster (Slice Instance)

Concrete minimum data for the five departments required by [08 §5](08_Vertical_Slice_Specification.md#5-departments-included), sized so Fire alone is sufficient to trigger the coverage tradeoff required by [09 §7](09_Service_Coverage_and_Response_Model.md#7-slice-specific-application) without requiring a second station.

| Department | Facility | Units/Staff | Fixed Policy | Role in Slice |
|---|---|---|---|---|
| Fire | 1 station | 1 engine crew | Standard | Responds to the scripted emergency ([08 §6](08_Vertical_Slice_Specification.md#6-the-one-emergency)). Its home coverage spans both the incident's location and one adjacent district within the Reduced band ([09 §3](09_Service_Coverage_and_Response_Model.md#3-travel-time-bands)); dispatching it to the incident drops the adjacent district's coverage per [09 §7](09_Service_Coverage_and_Response_Model.md#7-slice-specific-application). |
| Police | 1 precinct | 1 patrol unit | Standard | Supports perimeter/evacuation control alongside Fire per [08 §5](08_Vertical_Slice_Specification.md#5-departments-included). Not required to trigger the coverage effect — Fire alone satisfies it. |
| Inspection | 1 office | 1 inspector | Standard | Produces the pre-incident condition record ([03 §4](03_Departments_Emergency_Management_and_Courts.md)) that the condemnation and its court case reference. Does not respond during the emergency itself. |
| Education | 1 school | Fixed small enrollment and staff, kept below the 80% capacity-penalty threshold ([04 §5](04_Citizens_Education_Employment_and_Neighborhood_Risk.md)) | Standard | Exists only to satisfy the five-department scope requirement in [08 §5](08_Vertical_Slice_Specification.md#5-departments-included). No mechanical interaction with the emergency, court case, or media coverage. |
| Court | 1 courthouse | 1 preassigned judge | Standard | Resolves the condemnation dispute per Section 3 above. |

## 5. Region Preset (Slice Instance)

[01 §8](01_Game_Vision_and_Core_Structure.md) and [08 §3](08_Vertical_Slice_Specification.md#3-region-and-mayor) require the regional data schema to exist but only one filled instance. This section fixes only the fields that other slice systems' numbers actually depend on; visual identity, climate, and environmental-pressure content are art/narrative concerns outside this report's scope.

| Field | Slice Default |
|---|---|
| Zoning categories available | Single-family and Mixed-use only, per [08 §4](08_Vertical_Slice_Specification.md#4-development-system-limited) |
| Base land value index | 1.0 (baseline multiplier — the Developer Interest scale in Section 2 assumes this) |
| Travel-time bands | Uses the citywide defaults from [09 §3](09_Service_Coverage_and_Response_Model.md#3-travel-time-bands) unmodified — no region-specific band multiplier |
| Disaster/emergency generation | Not applicable — the slice's one emergency is scripted directly per [08 §6](08_Vertical_Slice_Specification.md#6-the-one-emergency), not spawned by a regional frequency model |

## 6. Open Follow-Ups

Not required for the slice, but flagged for later tuning passes, per [07 §4](07_Open_Decisions_and_Expansion_Backlog.md):

- The full Developer Interest model — incentive strength, hard-mode timer reset, probability of improved offers, media-specific rejection suspicion.
- Court penalty ranges for case types other than the emergency condemnation dispute (criminal cases, zoning disputes, city lawsuits, liability claims).
- Region-specific numeric variation beyond this single flat instance (climate multipliers, economic tendencies, environmental pressure), per [09 §8](09_Service_Coverage_and_Response_Model.md#8-open-follow-ups).
