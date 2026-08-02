# City Manager Simulation — Design Report Index

## Purpose

This report set organizes the design discussion into focused systems that can be expanded independently later. It reflects decisions and ideas discussed through **Question 150**.

The game is a sandbox city-management simulation in which the player serves as the **City Manager** rather than an all-powerful mayor or planner. The city grows through a mix of public construction, private development proposals, department operations, political oversight, legal accountability, media coverage, and individual citizen behavior.

## Reports

### [01 — Game Vision and Core Structure](01_Game_Vision_and_Core_Structure.md)

Covers the player role, sandbox structure, mayor relationship, city growth philosophy, regional presets, game-over conditions, and the overall distinction from conventional city builders.

### [02 — Land, Development, Housing, and Infrastructure](02_Land_Development_Housing_and_Infrastructure.md)

Covers land acquisition, zoning, private development proposals, public construction, city-owned housing, developer interest, redevelopment, infrastructure ownership, and neighborhood condition.

### [03 — Departments, Emergency Management, and Courts](03_Departments_Emergency_Management_and_Courts.md)

Covers department capacity, staffing, vehicles, service priorities, emergency command, station coverage, capital requests, court operations, judges, liability, and case outcomes.

### [04 — Citizens, Education, Employment, and Neighborhood Risk](04_Citizens_Education_Employment_and_Neighborhood_Risk.md)

Covers individual citizen simulation, education tiers, workforce matching, unemployment, overqualification, risk factors, crime, litter, trust, neighborhood clustering, and recovery.

### [05 — Reputation, Media, Politics, and Accountability](05_Reputation_Media_Politics_and_Accountability.md)

Covers reputation scoring, local news, newspaper/radio/television progression, private and city-owned outlets, freedom of speech, repeated media rejections, mayor/council approvals, and public accountability.

### [06 — AI, LLM, and Simulation Architecture](06_AI_LLM_and_Simulation_Architecture.md)

Covers the hybrid deterministic simulation and LLM design, restricted outcome generation, persistent personalities, reproducibility, procedural city generation, and preliminary engine direction.

### [07 — Open Decisions and Expansion Backlog](07_Open_Decisions_and_Expansion_Backlog.md)

Lists deferred systems, unresolved implementation questions, later-game ideas, and suggested topics for the next design sessions.

### [08 — Vertical Slice Specification](08_Vertical_Slice_Specification.md)

Defines the minimum playable city — one region, one Mayor, five departments, one emergency, one court case, one media outlet, one developer dispute, and a complete accountability report — as a concrete build target, with explicit scope boundaries and a suggested build order.

### [09 — Service Coverage and Response Model](09_Service_Coverage_and_Response_Model.md)

Proposes first-pass numeric defaults for travel-time bands, coverage-state resolution, response-time severity multipliers, and cross-station exposure, scoped to unblock the vertical slice's emergency system.

### [10 — Vertical Slice Data Defaults](10_Vertical_Slice_Data_Defaults.md)

Fixes the remaining vertical-slice numeric gaps — developer interest penalty/recovery and the court penalty range for the emergency condemnation dispute — and supplies the concrete department roster and region-preset data needed to start building.

### [11 — Vertical Slice Implementation Approach](11_Vertical_Slice_Implementation_Approach.md)

Amends how the slice gets built for a solo, casual-pace first pass: reframes the build order as a branching decision-flow graph feeding a shared accountability log, defers LLM calls in favor of the deterministic template fallbacks already described in Report 06, reopens a narrow hand-authored named-citizen set, and splits the Definition of Done into a first-pass tier and a deferred save/load-reproducibility tier.

## Current Design Pillars

1. **The player manages rather than directly controls everything.**
2. **Private actors remain independent and may work against city interests.**
3. **Capacity, staffing, infrastructure, and distance determine actual service quality.**
4. **Decisions create delayed and sometimes uncertain consequences.**
5. **The simulation engine controls facts and mechanics; AI creates interpretation and personality.**
6. **The city develops organically rather than through rigid painted grids.**
7. **Failure should produce an explainable historical record, not merely a score screen.**

## Suggested Review Order

For a complete pass, read Reports 01 through 10 in order.

For targeted development:

- Start with **Report 02** for procedural land and development systems.
- Start with **Report 03** for the operational city-management gameplay loop.
- Start with **Report 06** for technical architecture and AI boundaries.
- Start with **Report 07** before resuming the one-question design process.
- Start with **Report 08** to see what should actually get built first.
- Start with **Report 10** for the concrete numbers and data needed to begin Report 08's build order.
- Start with **Report 11** for how to actually sequence and build the slice as a solo, casual-pace first pass.
