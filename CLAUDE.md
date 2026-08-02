# CLAUDE.md

Guidance for Claude Code when working in this repository.

## What this repo is

City-Authority (CA) is a **design-documentation repo for an unbuilt game**, not a codebase. There is no source code, build system, or test suite yet — everything here is Markdown design reports. Do not assume any engine, framework, or file structure beyond what's described below; none has been committed to.

The game: a sandbox city-management simulation where the player is the **City Manager** (not an all-powerful mayor/planner), balancing department capacity, private development, emergencies, courts, media, and political oversight under a Mayor and City Council.

## Repo structure

- `00_Project_Index.md` — table of contents and summary for every report. **Update this whenever a report is added or its scope changes.**
- `01_Game_Vision_and_Core_Structure.md` — player role, sandbox structure, regional presets, catastrophic failure/game-over, historical archive.
- `02_Land_Development_Housing_and_Infrastructure.md` — zoning, private/city development proposals, developer interest, infrastructure ownership, redevelopment.
- `03_Departments_Emergency_Management_and_Courts.md` — department capacity model, notifications/City Log, Emergency Management, Incident Commander, capital requests, courts and judges.
- `04_Citizens_Education_Employment_and_Neighborhood_Risk.md` — individual citizen simulation, education tiers, employment matching, risk/crime clustering, neighborhood recovery.
- `05_Reputation_Media_Politics_and_Accountability.md` — reputation subscores, newspaper/radio/TV tiers, editorial independence, political approval tiers, the accountability record.
- `06_AI_LLM_and_Simulation_Architecture.md` — the hybrid deterministic-engine + LLM design (see below), reproducibility, validation, engine candidates.
- `07_Open_Decisions_and_Expansion_Backlog.md` — deferred features, systems that still need numeric tuning, suggested next deep-dive reports, design guardrails.
- `08_Vertical_Slice_Specification.md` — **the current build target**: the minimum playable city (one region, one Mayor, five departments, one scripted emergency, one court case, one newspaper, one developer dispute, an accountability report). Read this first if implementation work starts.
- `09_Service_Coverage_and_Response_Model.md` — first-pass numeric defaults (travel-time bands, coverage-state resolution, response-time multipliers) needed to make the vertical slice's emergency system actually implementable.
- `10_Vertical_Slice_Data_Defaults.md` — fixes the remaining vertical-slice numeric gaps (developer interest penalty/recovery, court penalty range for the emergency condemnation dispute) and supplies the concrete department roster and region-preset data needed to start building.

Reports are numbered and meant to be read roughly in order; `00_Project_Index.md §Suggested Review Order` gives targeted entry points per topic.

## Conventions for editing reports

- Cross-reference other reports with relative Markdown links and section numbers, e.g. `[03 §14](03_Departments_Emergency_Management_and_Courts.md)`, not bare prose references.
- Numbers/thresholds proposed outside of Report 09 are provisional design intent, not final balance — phrase additions the same way (e.g., "proposed default," not "the value is").
- When a report resolves or supersedes an item in `07_Open_Decisions_and_Expansion_Backlog.md`, cross-link it from that section rather than deleting the backlog entry (see how Reports 08/09 were linked in as "Resolved in ...").
- New top-level reports get the next sequential number and must be added to `00_Project_Index.md` (both the report list and the Suggested Review Order section).

## Core design pillars (from Report 01 §13 / 00 §Current Design Pillars)

1. The player manages rather than directly controls everything.
2. Private actors remain independent and may work against city interests.
3. Capacity, staffing, infrastructure, and distance determine actual service quality — funding a policy doesn't guarantee the department can execute it.
4. Decisions create delayed and sometimes uncertain consequences.
5. **The simulation engine controls facts and mechanics; AI creates interpretation and personality** — this is the load-bearing architectural boundary (see below).
6. The city develops organically rather than through rigid painted zoning grids.
7. Failure should produce an explainable historical record, not merely a score screen.

## The engine/AI boundary (Report 06)

This is the most important constraint for any future implementation work:

- **The deterministic engine owns all facts**: numeric values, citizen stats, department capacity, travel time/coverage, budgets, evidence, legal rules, valid choices, penalty ranges, mechanical consequences.
- **The LLM only interprets and narrates**: manager recommendations, judge reasoning, mayor/council responses, developer proposal flavor text, news articles, citizen complaints — always constrained to a restricted, engine-provided set of valid outcomes.
- The LLM must never invent simulation values or change game state outside its allowed outputs. Every LLM output must be validated before entering the simulation (format, valid outcome ID, numeric range, no contradictions with engine facts) — invalid output gets retried, repaired, or replaced with a deterministic fallback.
- AI-driven decisions (court rulings, recommendations) must be **stored and reproducible**: reloading a save must show the stored result, never re-generate a potentially different one.
- The game must remain playable if AI generation fails entirely (template fallbacks exist for every AI-driven surface).

If asked to design or implement any AI-touching system, preserve this boundary — it's the thing every other design report assumes holds.

## Current status / what to work on

- **No code exists yet.** The project is pre-implementation.
- **Engine: Unity 6 with C#**, decided (Report 06 §13). Godot 4 C# remains documented as the fallback rationale if Unity-specific constraints force a reconsideration, but implementation should target Unity 6.
- The active build target is the **vertical slice** (Report 08) — the smallest playable loop that proves the core identity (capacity bottleneck → delayed consequence → accountability record → explainable outcome). Its suggested build order deliberately builds accountability logging (step 3) *before* the emergency/coverage system (step 2 is designed, but logging infra should land early) since every later system writes to it. Report 10 supplies the concrete numbers and data (department roster, region preset, developer interest, court penalty range) needed to actually start Step 1.
- Full citizen-level simulation (Report 04), procedural parcel/road generation, and most numeric tuning in Report 07 §4 are explicitly deferred past the slice — don't scope-creep implementation work into them unless asked.
- Repo work in this environment (`ezreactiveagent/City-Authority`) has historically been drafted in a sandboxed session without push access, then applied/pushed from a machine with GitHub Desktop credentials. Check push access before assuming a commit will reach `origin`.
