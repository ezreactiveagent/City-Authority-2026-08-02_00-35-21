# CLAUDE.md

Guidance for Claude Code when working in this repository.

## What this repo is

City-Authority (CA) is now a **Unity 6 C# project** (this file lives at the project root, alongside `Assets/`, `Packages/`, `ProjectSettings/`) that also carries its own design documentation as the numbered Markdown reports below. The reports remain the source of truth for design intent; implementation of the vertical slice (Report 08) is underway — see "Current status" below for what exists in code.

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

- **Engine: Unity 6 with C#**, decided (Report 06 §13). Godot 4 C# remains documented as the fallback rationale if Unity-specific constraints force a reconsideration, but implementation targets Unity 6.
- The active build target is the **vertical slice** (Report 08), built per its suggested build order (08 §14, amended by 11 §2-§3). **All seven build-order steps are implemented, and Slice v1's save/load reproducibility (08 §13 item 8) has since landed too — the full Report 08 definition of done is complete.**

  | Step | System | Code |
  |---|---|---|
  | 1 | Fixed region + department data | `Assets/Scripts/CityAuthority/Data/` (RegionPreset, DepartmentDefinition, MayorPersonality, District, SliceConfig, …) |
  | 2 | Scripted emergency + coverage model | `Assets/Scripts/CityAuthority/Emergency/` (CoverageResolver, EmergencyIncidentRuntime, DepartmentCoverageState, ResponseTimePenalty) |
  | 3 | Accountability logging | `Assets/Scripts/CityAuthority/Accountability/` (CityLog, AccountabilityEvent, IAccountabilityRecorder) — the shared log every later system writes into |
  | 4 | Court case + restricted-outcome AI pattern | `Assets/Scripts/CityAuthority/Court/` (CourtOutcomeCatalog, JudicialRulingSelector, CondemnationCaseRuntime) — deterministic stub standing in for a future LLM call, per 11 §3 |
  | 5 | Development listing + proposal cycle | `Assets/Scripts/CityAuthority/Development/` (DeveloperInterest, DevelopmentProposalCycleRuntime) |
  | 6 | Newspaper coverage | `Assets/Scripts/CityAuthority/Media/` (NewsArticleGenerator, NewspaperCoverageRuntime) |
  | 7 | Final report generation | `Assets/Scripts/CityAuthority/Report/` (ScenarioOutcomeResolver, AccountabilityReport, FinalReportGenerator) — scenario-scoped pass/fail tied to whether the emergency's life-safety threat was contained, plus an end-of-scenario report assembled by querying the Step 3 City Log and reading the Court/Media systems' own stored records |
  | 8 (Slice v1) | Save/load reproducibility | `Assets/Scripts/CityAuthority/SaveLoad/` (ScenarioSaveData + friends, ScenarioSaveService, SaveFileIO) — captures every runtime system to a JsonUtility-serializable file and restores it via additive `Restore()` factories on each runtime class, never replaying anything; the court ruling, published articles, and log entries all come back as stored data. The one exception is the final report, which is a pure recomputation over the restored state rather than also being persisted, since it's deterministic |

  All eight are wired together in `Assets/Data/Slice/SliceConfig_Default.asset` and drivable by hand via the bare-bones `EmergencyDebugPanel` (`Assets/Scripts/CityAuthority/DebugUI/`), attached to a GameObject in `Assets/Scenes/SampleScene.unity` — press Play and use its buttons to walk the whole scenario (emergency → condemnation → ruling → development decision → newspaper → final report), plus Save/Load buttons that round-trip the full scenario through a JSON file at `Application.persistentDataPath`. Each step has EditMode tests under `Assets/Tests/Editor/`.

- Full citizen-level simulation (Report 04) beyond the 11 §4 named-citizen carve-out (already used for the court case's claimants — `Assets/Scripts/CityAuthority/Data/Citizen.cs`), procedural parcel/road generation, and most numeric tuning in Report 07 §4 remain explicitly deferred — don't scope-creep implementation work into them unless asked.
- A known Unity/MCP quirk: `AssetDatabase.SaveAssets()` (the global save) reliably trips a false-positive "immutable package asset altered" warning that blocks the MCP bridge. Use `AssetDatabase.SaveAssetIfDirty(obj)` per-object instead when authoring `.asset` instances via script.
- This repo previously existed as a docs-only folder separate from the Unity project; the two were merged (docs committed on top of the Unity scaffold) and the Unity project folder is now the single canonical repo. If you ever find a second `City-Authority*` folder with just the numbered reports and no `Assets/`, it's a stale leftover — don't use it.
- **The Unity Editor's MCP bridge is bound to `/Users/ezrasystems/City Authority/`, a separate checkout from whatever git worktree a session is actually editing.** `Application.dataPath` in that Editor resolves there, not to the worktree path. Editing files only in the worktree and then calling Unity MCP tools (compile/test/play) silently verifies against that *other*, unmodified checkout — it will report stale/pre-existing results with no indication anything is wrong (e.g. a passing test run that never saw your new files, or a "missing method" compile error against code that isn't actually missing). Before trusting any Unity MCP verification, `cp` the changed/new files (including generating `.meta` files by letting Unity `AssetDatabase.Refresh` them there, then copying those `.meta` files back) into `/Users/ezrasystems/City Authority/`, refresh, and verify there — then copy the resulting `.meta` files back into the worktree before committing, since new assets need real Unity-generated `.meta` files to be committed correctly.
- **Play mode state in that Editor can survive an "exit play mode" call made in an earlier turn if you re-enter play mode without re-confirming `EditorApplication.isPlaying` settled to `false` first.** Symptom: counts/state that should start at zero (log entries, published articles) show leftover values from a previous session's scenario walkthrough. Before trusting any fresh Play mode scenario run, explicitly check `isPlaying` is `false`, wait, re-enter, and spot-check that panel state (e.g. `Events.Count`) is actually zero before proceeding.
- Repo work in this environment has historically been drafted in a sandboxed session without push access, then applied/pushed from a machine with GitHub Desktop credentials — but push access has been confirmed working directly from this environment for Steps 1-6. Check push access before assuming a commit will reach `origin` if that stops being true.
