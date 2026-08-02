# Departments, Emergency Management, and Courts

## 1. Department Model

City services are constrained by actual capacity.

A department's performance depends on:

- Physical buildings
- Staff positions
- Vehicles
- Equipment
- Operating policy
- Throughput
- Road-network travel time
- Dispatch and coordination
- Current workload

Funding a policy does not guarantee that the department has enough capacity to execute it evenly.

## 2. Service Distance and Priority

Service coverage depends primarily on road-network distance.

Priority rules then determine who receives attention first when capacity is limited.

Possible department priorities include:

- Oldest request first
- Highest risk
- New development
- Highest property value
- Known hotspot
- Emergency severity
- District preference

The player can set district service priorities.

Unequal priorities may create:

- Neighborhood decline
- Political backlash
- Reputation damage
- Calls for more facilities or staff

## 3. Operating Policy Levels

Departments use a three-level operating policy.

Examples:

- Reactive / Standard / Proactive
- Limited / Standard / Expanded

Higher policy levels increase expected activity but also require enough staff, vehicles, equipment, and facilities.

A proactive department with inadequate capacity may maintain some areas extremely well while neglecting others.

## 4. Inspection Example

A rapidly growing city with one inspector may develop:

- Certificate of Occupancy backlogs
- Delayed openings
- Rushed inspections
- Missed safety problems
- Increased fire risk
- Structural collapse risk

This illustrates the intended relationship between policy ambition and operational capacity.

## 5. Notifications and City Log

Notifications use three levels:

- Informational
- Warning
- Critical

There is initially no notification filtering system.

All notices remain in a **City Log**.

A notification can include:

- Go to Location
- Open relevant department panel
- Acknowledge
- Act
- Ignore
- Pause and Inspect

Critical notifications should pause the game when the player chooses **Go to Location**.

## 6. Accountability for Follow-Through

The player may:

- Act on an alert
- Acknowledge it
- Ignore it

Acknowledging a warning without resolving it before an incident occurs can create greater accountability than never seeing the issue.

The final incident record should distinguish:

- Ignored warning
- Acknowledged but unresolved warning
- Action attempted
- Action completed
- Recommendation followed
- Recommendation rejected

## 7. Manager Recommendations

Department managers can produce popup recommendations.

Recommendations should show:

- Estimated outcome
- Confidence
- Required resources
- Expected tradeoffs

Recommendations may be wrong.

Manager specialty, available information, local knowledge, and department perspective influence recommendation quality.

## 8. Emergency Management Department

The Department of Emergency Management enables:

- Coordinated emergency priorities
- Automatic cross-department response changes
- City-specific readiness reports
- Better resource awareness
- Expanded emergency options

The game does not initially require the player to manually build detailed emergency plans.

A formal post-incident review system is deferred.

## 9. Emergency Department Manager

The player selects the Emergency Department Manager from profiles such as:

- General Manager
- Fire Specialist
- Law Specialist

The General Manager is the likely default.

Specialists provide stronger performance in their category and weaker recommendations outside it.

Examples:

- A Fire Specialist may request mutual aid or additional units early.
- A Law Specialist may incorrectly prioritize overtime, perimeter control, or enforcement during a wildfire.

The Manager serves a fixed term and cannot be replaced early.

The player receives advance notice before the term expires.

There is no initial warm-up period after appointment.

## 10. Incident Commander

The Incident Commander is a separate citywide position.

Unlocking the position requires prerequisites such as:

- Upgraded emergency headquarters
- Population threshold
- Department capacity
- Supporting departments
- Budget

Without an Incident Commander, the player usually receives approximately two manager-generated emergency choices.

With an Incident Commander, the player receives:

- Three or more choices
- More complete manual alternatives
- Cross-department coordination
- Broader resource awareness

The Commander knows citywide conditions but may know less local detail than a station manager.

## 11. Local Managers and Citywide Command

A station manager has stronger knowledge of:

- Local streets
- Local hazards
- Unit condition
- Neighborhood patterns
- Immediate response conditions

The Incident Commander has stronger knowledge of:

- Citywide coverage
- Competing emergencies
- Cross-station availability
- Supporting departments
- Overall exposure

Stations may have conflicting policies.

The Commander has final authority during a coordinated response.

Poor command decisions can create civil liability.

## 12. Dispatch and Coordination Requirements

Each department needs funded coordination capacity to fully participate in Commander overrides.

Relevant positions may include:

- Department coordinator
- Dispatcher
- Central emergency staff

Insufficient dispatch or coordination funding reduces:

- Information accuracy
- Response speed
- Cross-department execution
- Coverage forecasts

## 13. Physical Resource Commitment

Vehicles and crews are physically committed.

One fire truck cannot perform two assignments at once.

Sending multiple trucks to a major incident may leave other areas exposed.

Internal crew composition is abstracted. A fire truck represents the required crew rather than simulating every firefighter separately.

A large facility may still operate with only one vehicle if the budget is inadequate.

## 14. Reassignment and Coverage

Cross-station deployment can reduce local protection.

Once a unit is reassigned, the player cannot cancel the response while it is already underway.

Coverage forecasts use:

- **Adequate**
- **Reduced**
- **Uncovered**

Coverage is calculated from:

- Station availability
- Active calls
- Road travel
- Unit range
- Current reassignment

An Uncovered status triggers a warning.

The system should be best-effort rather than guaranteeing perfect protection.

## 15. Capital Requests

Departments request major assets through capital proposals.

Typical options:

1. Repair or extend existing equipment
2. Purchase a standard replacement
3. Build or purchase a larger expansion

Departments may request:

- Vehicles
- Fleet expansion
- Hospital upgrades
- Station expansion
- Court facilities
- Communications equipment

Routine purchases are approved by the City Manager.

Major purchases may require the Mayor.

Large or debt-funded purchases may require the City Council.

The Incident Commander cannot bypass capital approval. The Commander may only authorize overtime or deploy existing assets.

## 16. Court Department

The Court operates as a capacity-based city department.

Court performance depends on:

- Facilities
- Judges
- Staff
- Operating policy
- Case backlog
- Case complexity
- Priority rules

Court expansions increase throughput and may allow additional judges or specialization.

## 17. Court Case Types

Court overload can delay:

- Criminal cases
- Zoning disputes
- City lawsuits
- Developer appeals
- Emergency demolition payment disputes
- Liability claims

Delays can increase:

- Crime pressure
- Business uncertainty
- Development delays
- City legal costs
- Reputation risk

## 18. Court Operating Policy

The Court may use:

- Limited
- Standard
- Expanded

An aggressive operating policy without sufficient capacity creates uneven case processing.

Court priority options can follow the same general city framework:

- Oldest case
- Highest public-safety risk
- Largest financial exposure
- Development-related cases
- Criminal cases

## 19. Judge Selection

For leading judicial positions, the player may be presented with three judge profiles.

Visible profile information can include:

- Legal specialty
- General philosophy
- Public-safety focus
- Business-law experience
- Procedural strictness
- Liability caution
- Expected efficiency

Exact decision weights remain hidden.

Judges serve fixed terms and are not freely replaced.

## 20. Case Assignment

The player does not personally assign major cases to preferred judges.

Case assignment is determined by the Court system using factors such as:

- Judge authority
- Case type
- Availability
- Specialty
- Backlog

Judicial recusal is deferred as a later feature.

## 21. Court Outcomes

For emergency demolition payment disputes, valid outcomes may include:

- City pays
- Developer or owner pays
- Split responsibility
- Initial assignment upheld

The deterministic system defines:

- Known facts
- Evidence
- Valid outcomes
- Legal boundaries
- Penalty ranges
- Mechanical consequences

The judge's AI personality selects among valid outcomes and explains the ruling.

## 22. Court Publicity

Court rulings initially remain primarily legal and financial events.

As media coverage expands, major rulings can produce:

- News coverage
- Reputation effects
- Business-confidence changes
- Political pressure
- Increased scrutiny of city management
