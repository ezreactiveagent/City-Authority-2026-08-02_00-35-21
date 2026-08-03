# ADR 001: Use Unity 6 with C# as the Game Engine

- **Status:** Accepted (written retroactively — the decision was already made and implemented; see [06 §13](06_AI_LLM_and_Simulation_Architecture.md#13-preliminary-engine-direction))
- **Date decided:** prior to 2026-08-02 (implementation on Unity 6 predates this record)
- **Date recorded:** 2026-08-03

## Context

City Authority needed a simulation/rendering engine capable of supporting irregular runtime geometry (organic parcels and roads rather than a painted zoning grid, per [01 §9](01_Game_Vision_and_Core_Structure.md#9-organic-city-form)), a hybrid deterministic-engine-plus-LLM architecture ([06 §1-§3](06_AI_LLM_and_Simulation_Architecture.md#1-hybrid-design-principle)), and a path to commercial release ([06 §15](06_AI_LLM_and_Simulation_Architecture.md#15-commercial-direction)).

Three candidates were considered in [06 §13](06_AI_LLM_and_Simulation_Architecture.md#13-preliminary-engine-direction):

- **Godot 4 with C#** — a strong, leaner, open-source option.
- **Unity 6 with C#** — favored for irregular runtime geometry, tooling, asset ecosystem, and commercial production.
- **Unreal Engine** — possible, but judged likely to add complexity beyond the project's current needs.

## Decision

**Unity 6 with C# is the selected engine for City Authority.** All vertical-slice implementation work (Reports 08–11, and the code under `Assets/`) targets Unity 6.

Godot 4 C# remains documented as the fallback rationale in [06 §13](06_AI_LLM_and_Simulation_Architecture.md#13-preliminary-engine-direction) in case Unity-specific constraints force a reconsideration, but no active work targets it.

## Consequences

- All engine-specific implementation guidance in `CLAUDE.md` (asset saving quirks, MCP bridge binding, Input System requirements, etc.) is Unity-specific and does not need to account for Godot in parallel.
- Platform-support questions that differ meaningfully between engines (e.g. Steam Deck compatibility) inherit Unity's characteristics rather than Godot's, whenever they're revisited.
- If Unity-specific constraints ever force a reconsideration, Godot 4 C# is the documented starting point for re-evaluation rather than an open-ended re-survey of engines.

## Related

- [06 §13 — Preliminary Engine Direction](06_AI_LLM_and_Simulation_Architecture.md#13-preliminary-engine-direction) (original decision prose)
- [12 §3 item 5](12_External_Review_and_Recommended_Next_Steps.md#3-open-questions-for-tegan-do-not-resolve-unilaterally) (raised the question of formalizing this as an ADR; confirmed yes by Tegan, 2026-08-03)
