# Vertical Slice Specification

## 1. Purpose

This report converts the scope sketch in [07 — Open Decisions and Expansion Backlog, Section 5G](07_Open_Decisions_and_Expansion_Backlog.md) into a concrete build target.

The vertical slice is the smallest playable city that still demonstrates the core gameplay identity described in [01 — Game Vision and Core Structure](01_Game_Vision_and_Core_Structure.md): capacity-constrained departments, delayed consequences, an accountable decision record, and an explainable outcome.

The slice is a build target, not a design document. Every system it touches must reach a working, testable state — not just a described one. Systems not listed here should not be started until the slice is playable end to end.

## 2. Scope Summary

The slice contains exactly:

- One region preset
- One Mayor personality
- A limited private-development system (one proposal cycle)
- Five departments: Fire, Police, Inspection, Education, Court
- One scripted emergency
- One court case (arising from the emergency)
- One newspaper outlet
- One developer dispute
- A complete failure-or-success accountability report at the end of the scenario

If a system is not required to make one of these ten items work, it is out of scope for the slice, regardless of how developed its design report already is.

## 3. Region and Mayor

- One regional preset from [01 §8](01_Game_Vision_and_Core_Structure.md), fully data-driven per the existing design intent, but only one instance needs to exist. Building the regional data schema is in scope; building a second region is not.
- One Mayor personality, AI-driven per [01 §3](01_Game_Vision_and_Core_Structure.md) and [06 §3](06_AI_LLM_and_Simulation_Architecture.md). The Mayor must be able to approve or reject at least one major expenditure and react to the emergency and the court ruling.
- No Mayor selection screen or roster of alternate personalities is required.

## 4. Development System (Limited)

- Land listing, zoning designation, and a single private-development proposal cycle per [02 §1–§7](02_Land_Development_Housing_and_Infrastructure.md).
- Exactly two competing proposals are generated for the one listed parcel, not three, to reduce content requirements.
- The player can approve one proposal or reject both. Rejection triggers the relisting and interest-decay behavior from [02 §6](02_Land_Development_Housing_and_Infrastructure.md) at a placeholder decay rate (see Report 09 candidates in Section 12 below).
- City-built development, redevelopment, infrastructure sale, and incentive-package tiers are out of scope for the slice.

## 5. Departments Included

Fire, Police, Inspection, Education, and Court, per [03](03_Departments_Emergency_Management_and_Courts.md). Each department needs, at minimum:

- One facility
- A fixed small staff/vehicle count (no hiring, procurement, or capital-request flow required)
- A single operating-policy level (the three-level policy system from [03 §3](03_Departments_Emergency_Management_and_Courts.md) is not required for the slice; a fixed policy is sufficient)
- Enough simulated capacity to visibly bottleneck at least once during the scripted emergency

No Emergency Management Department, Incident Commander, dispatch/coordination staffing, or manager-personality selection ([03 §8–§12](03_Departments_Emergency_Management_and_Courts.md)) is required. The scripted emergency can be handled by the Fire and Police departments directly with a fixed General-Manager-equivalent recommendation source.

## 6. The One Emergency

- A single scripted incident (e.g., a structure fire with a life-safety risk) sized so that the fixed department capacity from Section 5 is insufficient to respond perfectly.
- The incident must generate at least one Warning-level and one Critical-level notification per [03 §5](03_Departments_Emergency_Management_and_Courts.md), and the player's response (act, acknowledge, ignore) must be recorded per [03 §6](03_Departments_Emergency_Management_and_Courts.md).
- The incident must produce a coverage effect (Adequate / Reduced / Uncovered per [03 §14](03_Departments_Emergency_Management_and_Courts.md)) somewhere in the city, even if only for one other district, to prove the tradeoff mechanic works.
- No mutual aid, cross-station reassignment cancellation edge cases, or Incident Commander authority are required.

## 7. Court Case

- Exactly one case type is required: the emergency condemnation / demolition payment dispute described in [02 §13](02_Land_Development_Housing_and_Infrastructure.md) and [03 §21](03_Departments_Emergency_Management_and_Courts.md), since it is a direct consequence of the one scripted emergency.
- One judge, with one visible personality profile per [03 §19](03_Departments_Emergency_Management_and_Courts.md) and [06 §5](06_AI_LLM_and_Simulation_Architecture.md). No judge selection roster is required — the judge can be preassigned.
- The ruling must be produced through the restricted-outcome pattern from [06 §4–§5](06_AI_LLM_and_Simulation_Architecture.md): engine-defined valid outcomes and penalty range, LLM selects and explains within that range, result is stored for reproducibility.
- No appeals, case backlog, or additional case types are required.

## 8. Media Coverage

- One newspaper outlet only, per [05 §4](05_Reputation_Media_Politics_and_Accountability.md). Radio and television are out of scope for the slice.
- The outlet must generate at least two stories: one covering the emergency response, one covering the court ruling. Both should follow the structured-event-to-article pattern from [06 §8](06_AI_LLM_and_Simulation_Architecture.md).
- No outlet personality drift, competing outlets, or city-owned media are required.

## 9. Developer Dispute

- The single rejected-or-approved proposal from Section 4 is the "developer dispute." A separate, additional dispute system is not required.
- If the player rejects both proposals, the resulting developer-interest penalty and any negative PR per [02 §11](02_Land_Development_Housing_and_Infrastructure.md) should be visible in the City Log and, if material, referenced in the newspaper's coverage.

## 10. Failure and Accountability Report

- The scenario must be completable in either a "successful" or "failed" state. Full catastrophic-failure destruction math per [01 §5](01_Game_Vision_and_Core_Structure.md) is not required — the slice can use a scenario-scoped pass/fail condition tied to the one emergency (e.g., whether the life-safety threat was contained).
- Regardless of outcome, the end-of-scenario report must be generated from the accountability record described in [05 §13](05_Reputation_Media_Politics_and_Accountability.md) and [03 §6](03_Departments_Emergency_Management_and_Courts.md): warnings issued, acknowledgments, actions taken, the court ruling, and media coverage, in the categories used by the eventual Final Failure Report ([01 §6](01_Game_Vision_and_Core_Structure.md)).
- Historical archiving ([01 §7](01_Game_Vision_and_Core_Structure.md)) is not required — generating the report once, on demand, is sufficient.

## 11. Explicitly Out of Scope for the Slice

- Individual citizen-level simulation ([04](04_Citizens_Education_Employment_and_Neighborhood_Risk.md)). The slice can use a small fixed or placeholder population; the per-citizen risk/education/employment model is the single largest technical risk in the whole design and should be prototyped separately, not inside the slice. A narrow exception — a handful of hand-authored named citizens with no simulation loop, tied directly to the condemned structure and the coverage-tradeoff district — is scoped in by [11 §4](11_Vertical_Slice_Implementation_Approach.md#4-minimal-named-citizen-set-reopens-part-of-11); the full model here still is not.
- Procedural parcel/road generation ([02 §9](02_Land_Development_Housing_and_Infrastructure.md), [06 §12](06_AI_LLM_and_Simulation_Architecture.md)). The slice's one parcel can be hand-placed.
- Reputation subscore breakdown ([05 §1](05_Reputation_Media_Politics_and_Accountability.md)) beyond whatever single number the report needs.
- Incentive packages, capital requests, judge/manager rosters, multiple regions, City Council approval tier, and everything listed as Deferred in [07 §3](07_Open_Decisions_and_Expansion_Backlog.md).

## 12. Systems the Slice Still Needs Numbers For

Building the slice will force early answers on a subset of [07 §4](07_Open_Decisions_and_Expansion_Backlog.md):

- **Service Coverage** — required immediately, since the one emergency's bottleneck and coverage effect depend on it. See [09 — Service Coverage and Response Model](09_Service_Coverage_and_Response_Model.md) for a first-pass numeric model.
- **Developer Interest** (decay/recovery rate) — needed only if the player rejects both proposals in Section 4; a single placeholder constant is sufficient for the slice. See [10 — Vertical Slice Data Defaults](10_Vertical_Slice_Data_Defaults.md).
- **Court Outcomes** (penalty range for the one case type) — needed for Section 7; a single placeholder range is sufficient for the slice. See [10 — Vertical Slice Data Defaults](10_Vertical_Slice_Data_Defaults.md).

Report 10 also supplies the concrete department roster and region-preset data needed to begin Section 14's Step 1.

Education, citizen risk, media reach at scale, and catastrophic-failure weighting are not required to be tuned for the slice per Section 11.

## 13. Definition of Done

The slice is complete when a single playthrough can, without developer intervention:

1. Load the one region and Mayor.
2. Present the one development listing and resolve it (approve or reject).
3. Trigger the scripted emergency and produce at least one Warning and one Critical notification.
4. Show a visible capacity bottleneck and a non-Adequate coverage state somewhere in the city.
5. Generate the court case, produce a ruling within the engine-defined range, and store it for reproducibility.
6. Generate two newspaper stories referencing the emergency and the ruling.
7. Reach a pass/fail scenario end state and generate an accountability report covering all of the above.
8. Reload a save made mid-scenario and confirm the court ruling and any other stored AI decisions do not change.

**Item 8 is deferred to a second pass for the first working build; see [11 §5](11_Vertical_Slice_Implementation_Approach.md#5-definition-of-done-revised-amends-13).**

## 14. Suggested Build Order

1. Fixed region + department data (Sections 3, 5) — no gameplay yet, just data existing.
2. Scripted emergency + coverage model (Section 6, backed by Report 09) — proves the capacity-bottleneck mechanic.
3. Accountability logging (Section 10) — build this early since every later system needs to write to it, not bolt it on at the end.
4. Court case + restricted-outcome AI pattern (Section 7) — proves the engine/LLM boundary from Report 06 actually holds.
5. Development listing + proposal cycle (Sections 4, 9) — proves the private-actor negotiation loop.
6. Newspaper coverage (Section 8) — last, since it only summarizes events the other systems already produced.
7. Final report generation (Section 10) — assembled from the accountability log built in step 3.

Step 3 before step 2 is a deliberate reversal of narrative order: the accountability record is infrastructure that every other slice system depends on, and retrofitting it after the fact risks silently missing the exact "ignored vs. acknowledged vs. acted on" distinction the whole design identity depends on.

This order is a reasonable sequence for standing up each subsystem, but the subsystems should be wired together as a branching decision-flow graph feeding the shared accountability log, not as a hardcoded call chain — see [11 §3](11_Vertical_Slice_Implementation_Approach.md#3-deferred-llm-integration-amends-69) and [11 §2](11_Vertical_Slice_Implementation_Approach.md#2-decision-flow-structure-amends-14). Steps 4 (court case) and 6 (newspaper) should use the deterministic template stubs in [11 §3](11_Vertical_Slice_Implementation_Approach.md#3-deferred-llm-integration-amends-69) for the first pass rather than live LLM calls.
