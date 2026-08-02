# Open Decisions and Expansion Backlog

## 1. Purpose

This file separates settled direction from topics that remain deferred, incomplete, or ready for deeper design.

It can serve as the starting point when the one-question design process resumes.

## 2. High-Priority Open Decisions

### Procedural Development

- How are Domains drawn and edited?
- How does the player communicate street hierarchy?
- How much control does the player have over parcel subdivision?
- How are setbacks, easements, and driveways displayed?
- What happens when generated development fails to fit a site?
- Can private developers build roads inside a project?
- How does redevelopment preserve or replace existing road geometry?

### Department Simulation

- What exact positions exist in each department?
- How are salaries and hiring delays modeled?
- How are equipment condition and repair handled?
- How much staff detail should the player see?
- How do mutual-aid agreements work?
- How are hospitals and utilities integrated into emergency response?

### Courts

- How many judges can each court level support?
- What are the initial legal case categories?
- What evidence can become unknown, disputed, or unavailable?
- Can rulings be appealed?
- How is enforcement of court orders handled?
- How are damages and settlement offers calculated?
- When should judicial recusals be added?
- How should judge replacement and term expiration work?

### Media

- How are outlet personalities generated after opening?
- How does a city-owned outlet maintain or lose credibility?
- What determines whether a story is covered?
- Can competing outlets reduce or increase a story's impact?
- How does inaccurate reporting work?
- When do regional or national outlets enter the simulation?
- Can private outlets relocate, close, merge, or be purchased?

### Reputation

- What are the final reputation categories?
- What are the normal and maximum rates of change?
- How does reputation differ from trust?
- How are district-level views aggregated into city reputation?
- How much mechanical information should the player see?

## 3. Deferred Features

These were explicitly treated as possible later additions.

- Judicial recusal
- Formal post-incident reports as a routine system
- Forced condemnation outside immediate emergency danger
- Negotiation and counteroffers for development proposals
- Detailed media bias selection during proposal approval
- National or regional media
- Complex independent city communications
- Detailed individual emergency crews
- Department personnel leveling
- Burnout systems
- Historical cities directly modifying future mechanics

## 4. Systems That Need Numbers

The design direction exists, but implementation needs numeric tuning.

### Catastrophic Failure

- Destruction calculation
- Weight of occupied buildings
- Weight of critical infrastructure
- Population displacement thresholds
- Difficulty scaling

### Service Coverage

- Travel-time bands
- Vehicle range
- Adequate/Reduced/Uncovered thresholds
- Response-time penalties
- Cross-station exposure

**First-pass defaults proposed in [09 — Service Coverage and Response Model](09_Service_Coverage_and_Response_Model.md).** Vehicle range and region-specific band variation remain open (see Report 09 §8).

### Education

- School capacity penalties above 80%
- Strong penalty above 95%
- Staff-to-student ratios
- Education speed
- Dropout probability
- Education Center progression speed

### Citizen Risk

- Education weight
- Employment weight
- Housing weight
- Service-access weight
- Nonlinear clustering
- Maximum hotspot amplification
- Trust recovery rates

### Developer Interest

- Rejection penalty
- Recovery speed
- Incentive strength
- Hard-mode timer reset
- Probability of improved offers
- Media-specific rejection suspicion

**Rejection penalty and recovery speed given placeholder defaults in [10 — Vertical Slice Data Defaults](10_Vertical_Slice_Data_Defaults.md).** Incentive strength, hard-mode timer reset, probability of improved offers, and media-specific rejection suspicion remain open (see Report 10 §6).

### Court Outcomes

- Penalty range for the emergency condemnation / demolition payment dispute
- Penalty ranges for other case types (criminal, zoning disputes, city lawsuits, liability claims)

**A placeholder penalty range for the emergency condemnation dispute is proposed in [10 — Vertical Slice Data Defaults](10_Vertical_Slice_Data_Defaults.md).** Other case types remain open (see Report 10 §6).

### Media Reach

- Newspaper spread speed
- Radio reach
- Television reach
- Multiple-outlet stacking
- Story decay
- Reputation amplification

## 5. Suggested Next Deep-Dive Reports

Future design sessions could each focus on one document.

### A. Procedural City Form Specification

Define:

- Domain tools
- Road-generation rules
- Parcel generation
- Development footprints
- Terrain handling
- Redevelopment

### B. Department Roster and Capacity Model

Define every department's:

- Buildings
- Positions
- Vehicles
- Equipment
- Policies
- Priority options
- Failure modes

### C. Emergency Incident Framework

Define:

- Incident categories
- Escalation
- Recommendations
- Command options
- Coverage loss
- Liability
- Final incident records

### D. Court and Legal Rules Framework

Define:

- Case types
- Evidence packages
- Valid rulings
- Appeals
- Damages
- Judicial personalities
- Backlog

### E. Citizen Data Model

Define:

- Required individual attributes
- Update frequency
- Performance limits
- Household structure
- Migration
- Risk
- Education and employment

**A narrow, hand-authored named-citizen set (no simulation loop) is scoped into the vertical slice in [11 §4](11_Vertical_Slice_Implementation_Approach.md#4-minimal-named-citizen-set-reopens-part-of-11). The full per-citizen simulation described here remains open.**

### F. Media and Reputation Model

Define:

- Outlet creation
- Story selection
- Freedom-of-speech boundaries
- City communications
- Coverage reach
- Reputation formulas
- Public trust

### G. Vertical Slice Scope

Choose the minimum playable city containing:

- One region
- One Mayor type
- A limited development system
- Fire, police, inspection, education, and court departments
- One emergency
- Newspaper coverage
- One developer dispute
- A complete failure and accountability report

**Resolved in [08 — Vertical Slice Specification](08_Vertical_Slice_Specification.md).**

## 6. Recommended Next Question

When the one-question process resumes, a useful next question would be:

> Should repeated media-development rejections be judged only against comparable land-use applications, or should the game also track public statements and internal city reasoning to determine whether the pattern appears suppressive?

This follows directly from the most recent discussion without prematurely building a complete constitutional-law simulation.

## 7. Design Guardrails

Future additions should preserve these boundaries:

- The player should not control independent actors merely because they are inconvenient.
- Capacity should matter more than policy labels alone.
- AI must not invent simulation facts.
- Consequences should be explainable after the event.
- Complexity should create meaningful decisions rather than administrative busywork.
- Deferred systems should not block an initial playable version.
