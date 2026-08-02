# AI, LLM, and Simulation Architecture

## 1. Hybrid Design Principle

The game should combine:

- A deterministic simulation engine
- A constrained large language model layer

The simulation engine controls reality.

The LLM controls interpretation, personality, recommendations, and narrative expression.

## 2. Simulation Engine Responsibilities

The deterministic engine owns:

- Numeric values
- Citizen statistics
- Department capacity
- Travel time
- Coverage
- Budgets
- Evidence
- Known and unknown information
- Legal rules
- Valid choices
- Penalty ranges
- Mechanical consequences
- Development demand
- Risk calculations
- Reputation changes

The LLM must not invent simulation values or directly change the game state outside allowed outputs.

## 3. LLM Responsibilities

The LLM can generate:

- Department manager recommendations
- Judge reasoning
- Mayor responses
- Council responses
- Developer proposal descriptions
- News articles
- Citizen complaints
- Department reports
- Emergency summaries
- Historical references
- Natural-language explanations

## 4. Restricted Outcome Selection

For important decisions, the engine provides the LLM with a restricted set of valid outcomes.

Example court input:

- Relevant facts
- Admissible evidence
- Unknown facts
- Legal rules
- Valid rulings
- Minimum and maximum penalties
- Judge personality
- Required output format

The LLM chooses among valid results rather than inventing an entirely new legal outcome.

## 5. Judicial AI

Judges can have persistent tendencies such as:

- Public-safety focused
- Business friendly
- Strict proceduralist
- Liability cautious
- City friendly
- Unpredictable within limits

The player sees broad philosophy and specialty when selecting a judge.

Exact decision weights remain hidden.

Judge personality affects close or ambiguous cases, but it cannot override:

- Invalid outcomes
- Hard evidence
- Mandatory legal limits
- Engine-defined penalty boundaries

## 6. Manager AI

Department managers receive:

- Current department facts
- Known risks
- Available resources
- Specialty
- Confidence estimate
- Valid recommendation options

Their recommendations may be wrong because:

- Information is incomplete
- Their specialty creates bias
- Local and citywide perspectives differ
- Resources are unavailable
- Conditions change before execution

The recommendation system should not behave like a perfect strategy guide.

## 7. Developer AI

Developers can use AI to generate distinct proposal identities while remaining mechanically constrained.

The engine defines:

- Allowed zoning
- Density range
- Budget assumptions
- Infrastructure requirements
- Demand
- Risk
- Valid ownership forms

The LLM writes the proposal and gives it a coherent development strategy.

## 8. Media AI

Media stories can be generated from structured events.

Inputs may include:

- Event facts
- Outlet identity
- Coverage tier
- Known public information
- Government statements
- Court outcomes
- Historical context

The LLM produces the article, broadcast summary, or headline.

Mechanical reputation impact is still calculated by the engine.

## 9. Reproducibility

Important AI-driven decisions must remain stable after saving and reloading.

Store:

- Structured input
- Random seed
- Allowed outcomes
- Selected outcome
- Explanation
- Mechanical result
- Relevant personality state

Reloading should display the stored ruling or recommendation rather than asking the LLM to regenerate a potentially different result.

## 10. Output Validation

Every LLM output should be validated before entering the simulation.

Validation can check:

- Required format
- Valid outcome identifier
- Numeric range
- Forbidden state changes
- Missing fields
- Unsupported claims
- Contradictions with engine facts

Invalid output can be:

- Retried
- Repaired
- Replaced with a deterministic fallback

## 11. AI Failure Fallback

The game should remain playable without successful AI generation.

Fallbacks may include:

- Template recommendations
- Rules-engine rulings
- Prewritten news structures
- Generic proposal descriptions
- Simplified mayor responses

AI enriches the simulation but should not be a single point of failure.

## 12. Procedural City Generation

The procedural system may generate:

- Roads
- Parcels
- Easements
- Yards
- Fences
- Driveways
- Development footprints

Generation must obey:

- Zoning
- Road access
- Utility access
- Terrain
- Regional rules
- Density
- Setbacks
- Development quality
- Proposal specifications

## 13. Preliminary Engine Direction

The discussed engine candidates are:

- Godot 4 with C#
- Unity 6 with C#
- Unreal Engine

Preliminary assessment:

- **Unity 6 C#** is slightly favored for irregular runtime geometry, tooling, asset ecosystem, and commercial production.
- **Godot 4 C#** remains a strong option for a leaner, open-source project.
- **Unreal** is possible but may add complexity beyond the project's current needs.

**Decision: Unity 6 with C# is the selected engine for City Authority.** Godot 4 C# remains the documented fallback rationale above in case Unity-specific constraints force a reconsideration, but implementation work should proceed on Unity 6.

## 14. Technical Design Requirements

The architecture should support:

- Data-driven regional presets
- Deterministic simulation ticks
- Individual citizens with aggregate reporting
- Saveable AI decisions
- Modular department systems
- Procedural land development
- Multiple difficulty settings
- Historical city archives
- Clear separation between mechanics and narrative

## 15. Commercial Direction

The intended product is a paid commercial game.

An Early Access release is possible after the game becomes genuinely playable.

The roadmap should be clear but avoid exact public release dates that create unnecessary commitments.

Full development may involve professional developers rather than remaining a solo prototype.
