# Vertical Slice Implementation Approach

## 1. Purpose

[08 — Vertical Slice Specification](08_Vertical_Slice_Specification.md) defines *what* the slice contains. This report amends *how* it should actually be built, based on the real conditions of the build: a solo developer, working at a casual, undeadlined pace, who wants to see the design's branching-consequence identity hold up before investing in AI integration or full citizen simulation.

It changes four things about Report 08's approach without changing the slice's content scope (Section 2's ten items remain the target):

- Reframes [08 §14](08_Vertical_Slice_Specification.md#14-suggested-build-order)'s build order as a decision-flow graph rather than a strict linear pipeline (Section 3 below).
- Defers all LLM calls required by [08 §6–§9](08_Vertical_Slice_Specification.md) in favor of the deterministic template fallbacks already described in [06 §11](06_AI_LLM_and_Simulation_Architecture.md), keeping the restricted-outcome architecture as the drop-in future integration point (Section 4).
- Reopens a narrow slice of [08 §11](08_Vertical_Slice_Specification.md#11-explicitly-out-of-scope-for-the-slice)'s citizen-simulation exclusion: a handful of hand-authored named citizens tied directly to the slice's existing content, not the full model from [04](04_Citizens_Education_Employment_and_Neighborhood_Risk.md) (Section 5).
- Defers [08 §13](08_Vertical_Slice_Specification.md#13-definition-of-done)'s save/load reproducibility check to a second pass, so a first working build doesn't block on it (Section 6).

This report supersedes nothing in Report 08 outright — it narrows what "done" means for a first working pass, and Report 08's full definition remains the eventual target.

## 2. Decision-Flow Structure (Amends §14)

[08 §14](08_Vertical_Slice_Specification.md#14-suggested-build-order)'s seven steps remain a reasonable order for standing up subsystems one at a time — that part is unchanged. What this section amends is how those subsystems talk to each other once built: not as a hardcoded call sequence (system 2 calls system 4 calls system 6), but as a shared graph of decision points and consequence edges, all of which write to the accountability log built in step 3.

Every player-facing decision in the slice is a node; every mechanical result it triggers is an edge into the accountability log and, where applicable, into another node:

| Decision Node | Branches | Consequence Edges |
|---|---|---|
| Development proposal ([08 §4](08_Vertical_Slice_Specification.md#4-development-system-limited)) | Approve one / Reject both | Approve → proposal proceeds, no further branching. Reject → Developer Interest −40 ([10 §2](10_Vertical_Slice_Data_Defaults.md#2-developer-interest-defaults)) → City Log entry → if material, referenced in newspaper coverage ([08 §9](08_Vertical_Slice_Specification.md#9-developer-dispute)). |
| Emergency warning response ([08 §6](08_Vertical_Slice_Specification.md#6-the-one-emergency), [03 §6](03_Departments_Emergency_Management_and_Courts.md)) | Act / Acknowledge / Ignore | Act → unit dispatched → coverage state resolved ([09 §4, §7](09_Service_Coverage_and_Response_Model.md)) → response-time severity multiplier applied ([09 §5](09_Service_Coverage_and_Response_Model.md#5-response-time-penalties)). Acknowledge/Ignore → recorded without capacity commitment → higher severity outcome. All three branches write an entry to the accountability log regardless of which is chosen. |
| Coverage tradeoff ([09 §6](09_Service_Coverage_and_Response_Model.md#6-cross-station-exposure)) | — (automatic once a unit is dispatched) | Home district coverage recalculated → if it drops to Uncovered, a second notification is generated, independent of the original incident notification. |
| Condemnation ([02 §13](02_Land_Development_Housing_and_Infrastructure.md), [03 §21](03_Departments_Emergency_Management_and_Courts.md)) | — (triggered by incident severity crossing the condemnation threshold) | Opens the court case node below. |
| Court ruling ([08 §7](08_Vertical_Slice_Specification.md#7-court-case), [10 §3](10_Vertical_Slice_Data_Defaults.md#3-court-outcome-penalty-range)) | City pays / Owner pays / Split (25/50/75) / Initial assignment upheld | Selected outcome → City Log entry → referenced in the newspaper's ruling story ([08 §8](08_Vertical_Slice_Specification.md#8-media-coverage)). |
| Scenario end ([08 §10](08_Vertical_Slice_Specification.md#10-failure-and-accountability-report)) | Pass / Fail | Aggregates every prior node's outcome, not just the terminal incident-containment result, into the final accountability report. |

The practical implication for the build: the accountability log (step 3) should expose a single "record an event" interface that every other node's consequence edges call into, rather than each system knowing about and calling the next system directly. This is what makes the "web/bracket" structure real in code — nodes don't need to know what's downstream of them, they just report outcomes, and the accountability report at the end is a read of the accumulated graph rather than a hand-assembled summary. This also keeps the build order in [08 §14](08_Vertical_Slice_Specification.md#14-suggested-build-order) valid: you can still build and test each node in isolation, as long as it writes to the shared log interface from the start.

## 3. Deferred LLM Integration (Amends §6–§9)

[06 §11 "AI Failure Fallback"](06_AI_LLM_and_Simulation_Architecture.md) already describes exactly the mode this slice should launch in: "The game should remain playable without successful AI generation." For the first build, treat that fallback path as the *primary* path, not a degraded one. Concretely:

- **Court ruling** ([08 §7](08_Vertical_Slice_Specification.md#7-court-case)): the restricted-outcome pattern from [06 §4](06_AI_LLM_and_Simulation_Architecture.md#4-restricted-outcome-selection) still applies — the engine still defines the four valid outcomes and the penalty range from [10 §3](10_Vertical_Slice_Data_Defaults.md#3-court-outcome-penalty-range). Instead of an LLM selecting among them, use a deterministic rule keyed to the judge's personality tag ([06 §5](06_AI_LLM_and_Simulation_Architecture.md#5-judicial-ai)) — e.g. a "public-safety focused" judge biases toward City pays or a higher owner share; a "business friendly" judge biases the other way — paired with a canned explanation string per outcome ID ("rules-engine rulings" and "prewritten news structures," per [06 §11](06_AI_LLM_and_Simulation_Architecture.md#11-ai-failure-fallback)).
- **Newspaper stories** ([08 §8](08_Vertical_Slice_Specification.md#8-media-coverage)): use prewritten article templates with slots for the structured event facts (incident location, coverage state, ruling outcome), rather than an LLM-generated article.
- **Mayor reactions** ([08 §3](08_Vertical_Slice_Specification.md#3-region-and-mayor)): use the "simplified mayor responses" fallback named in [06 §11](06_AI_LLM_and_Simulation_Architecture.md#11-ai-failure-fallback) — a small set of canned reaction lines keyed to outcome type (approved/rejected proposal, contained/uncontained emergency, ruling direction).

Keep the interface shape identical to what an LLM call would eventually receive and return: structured input in (facts, valid outcomes, personality tag), `{selected outcome, explanation}` out, validated the same way [06 §10](06_AI_LLM_and_Simulation_Architecture.md#10-output-validation) describes. Store the same fields [06 §9](06_AI_LLM_and_Simulation_Architecture.md#9-reproducibility) requires (structured input, allowed outcomes, selected outcome, explanation, mechanical result, personality state) even though the "selection" is a deterministic rule for now, not a sampled generation. Because deterministic output is valid by construction, the [06 §10](06_AI_LLM_and_Simulation_Architecture.md#10-output-validation) validation gate has nothing to reject in this mode — but keep the call site shaped so a real LLM call can be substituted later without changing any downstream consumer of `{selected outcome, explanation}`.

This does not change the engine/AI boundary itself ([06 §2–§3](06_AI_LLM_and_Simulation_Architecture.md)) — it just means the "AI" side of that boundary is temporarily filled by rules and templates instead of a model call, which the existing design already anticipates as a legitimate operating mode, not a workaround.

## 4. Minimal Named-Citizen Set (Reopens part of §11)

[08 §11](08_Vertical_Slice_Specification.md#11-explicitly-out-of-scope-for-the-slice) excludes individual citizen simulation entirely, calling the full model in [04](04_Citizens_Education_Employment_and_Neighborhood_Risk.md) "the single largest technical risk in the whole design" and directing it to be prototyped separately. That risk assessment still stands — this section does not reopen the full model (education tiers, employment matching, risk clustering, migration all remain deferred per [08 §11](08_Vertical_Slice_Specification.md#11-explicitly-out-of-scope-for-the-slice)).

What it adds instead is a small, hand-authored set of named citizens — no simulation loop, no aggregation, no clustering — carrying only the subset of [04 §1](04_Citizens_Education_Employment_and_Neighborhood_Risk.md#1-individual-citizen-simulation)'s variable list that the slice's existing content already touches:

| Attribute | Source | Why it's included |
|---|---|---|
| Name | — | Gives the accountability report and newspaper stories a concrete stake instead of an abstract population count. |
| Residence (which parcel) | [02](02_Land_Development_Housing_and_Infrastructure.md) | Ties the citizen to either the condemned structure or the district affected by the coverage tradeoff. |
| Housing status (owner / renter) | [04 §1](04_Citizens_Education_Employment_and_Neighborhood_Risk.md#1-individual-citizen-simulation) | Determines whether they're a claimant in the condemnation court case ([02 §13](02_Land_Development_Housing_and_Infrastructure.md)). |
| Satisfaction, Trust | [04 §1](04_Citizens_Education_Employment_and_Neighborhood_Risk.md#1-individual-citizen-simulation) | Gives the accountability report something to move in response to the court ruling and coverage outcome (e.g. "Trust declined among residents of [district] following the ruling"). |

Proposed slice population: 2–3 named residents of the condemned structure (the court case's claimants) plus 1–2 named residents of the district affected by the coverage tradeoff in [09 §7](09_Service_Coverage_and_Response_Model.md#7-slice-specific-application). Every other district and every other citizen remains an unmodeled population number, exactly as [08 §11](08_Vertical_Slice_Specification.md#11-explicitly-out-of-scope-for-the-slice) already assumes.

This does not resolve [07 §5E](07_Open_Decisions_and_Expansion_Backlog.md)'s "Citizen Data Model" deep-dive — that remains open for the full per-citizen simulation. This section only establishes that a handful of named citizens, scoped to what the slice's other systems already produce, is in scope for the first build.

## 5. Definition of Done, Revised (Amends §13)

[08 §13](08_Vertical_Slice_Specification.md#13-definition-of-done) lists eight checks for a complete playthrough. For a first working pass, split them into two tiers:

**Slice v0 (first working build) — items 1–7 of [08 §13](08_Vertical_Slice_Specification.md#13-definition-of-done) unchanged:**

1. Load the one region and Mayor.
2. Present the one development listing and resolve it.
3. Trigger the scripted emergency and produce at least one Warning and one Critical notification.
4. Show a visible capacity bottleneck and a non-Adequate coverage state somewhere in the city.
5. Generate the court case, produce a ruling within the engine-defined range (via the deterministic stub in Section 3 above), and store it.
6. Generate two newspaper stories (via templates, per Section 3) referencing the emergency and the ruling.
7. Reach a pass/fail scenario end state and generate an accountability report covering all of the above.

**Slice v1 (deferred) — item 8:**

8. Reload a save made mid-scenario and confirm the court ruling and any other stored decisions do not change.

Deferring item 8 doesn't require throwing away the storage work — Section 3 already asks for the same structured-input/selected-outcome/explanation storage [06 §9](06_AI_LLM_and_Simulation_Architecture.md#9-reproducibility) calls for, so v0 already does the work v1's check would verify. v1 just isn't a gate on calling the first build "done."

## 6. What Remains Unchanged

Everything else in [08](08_Vertical_Slice_Specification.md) holds: one region, one Mayor, five departments, one scripted emergency, one court case, one newspaper outlet, one developer dispute, the [08 §11](08_Vertical_Slice_Specification.md#11-explicitly-out-of-scope-for-the-slice) exclusions apart from the narrow citizen carve-out in Section 4, and the numeric defaults in [09](09_Service_Coverage_and_Response_Model.md) and [10](10_Vertical_Slice_Data_Defaults.md) — both already framed as "starting defaults for playtesting, not final balance," which matches building against them now and tuning later rather than validating them up front.
