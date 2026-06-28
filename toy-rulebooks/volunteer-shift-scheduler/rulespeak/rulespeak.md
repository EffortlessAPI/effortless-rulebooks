# 📘 Volunteer Shift Scheduler — RuleSpeak

_Coverage status, volunteer load (under/ok/over), and event-level A–F staffing grade all fall out automatically._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Event** | An event tracked by the business. |
| Shift Count | Total shifts attached to this event. |
| Total Slots Needed | Sum of needed-count across all shifts in this event. |
| Total Slots Filled | Sum of filled-count across all shifts in this event. |
| Coverage Percent | TotalSlotsFilled / TotalSlotsNeeded (0..1). |
| Staffing Grade | A-F letter grade from CoveragePercent. A: >=95%, B: >=85%, C: >=70%, D: >=50%, F: below. |
| **Volunteer** | A volunteer tracked by the business. |
| Assignment Count | Number of shifts this volunteer is assigned to. |
| Assigned Hours | Sum of DurationHours across this volunteer's assigned shifts. |
| Load Status | under \| ok \| over. Under: <50% of MaxHours. Over: >100%. Otherwise ok. |
| **Shift** | A shift tracked by the business. |
| Event Name | The name of the shift's event. |
| Filled Count | Number of volunteers currently assigned. |
| Coverage Status | understaffed \| covered \| overstaffed. |
| **Assignment** | An assignment tracked by the business. |
| Volunteer Name | The name of the assignment's volunteer. |
| Shift Name | The name of the assignment's shift. |
| Shift Duration Hours | Pulled through to support per-volunteer AssignedHours aggregation. |

## 2 Fact Types

- a **shift** may reference one **event**
- an **assignment** may reference one **volunteer**
- an **assignment** may reference one **shift**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Shift Count** | An event's shift count is the number of shifts related to the event. |
| **DR-2 Total Slots Needed** | An event's total slots needed is the total needed count across the shifts related to the event. |
| **DR-3 Total Slots Filled** | An event's total slots filled is the total filled count across the shifts related to the event. |
| **DR-4 Coverage Percent** | The event's coverage percent is determined by the following priority:<br>1. `TotalSlotsFilled/TotalSlotsNeeded`, if the total slots needed is greater than 0;<br>2. otherwise 0. |
| **DR-5 Staffing Grade** | The event's staffing grade is determined by the following priority:<br>1. the literal “A”, if the coverage percent is at least 0.95;<br>2. the literal “B”, if the coverage percent is at least 0.85;<br>3. the literal “C”, if the coverage percent is at least 0.70;<br>4. the literal “D”, if the coverage percent is at least 0.50;<br>5. otherwise the literal “F”. |
| **DR-6 Assignment Count** | A volunteer's assignment count is the number of assignments related to the volunteer. |
| **DR-7 Assigned Hours** | A volunteer's assigned hours is the total shift duration hours across the assignments related to the volunteer. |
| **DR-8 Load Status** | The volunteer's load status is determined by the following priority:<br>1. `If(AssignedHours>MaxHours, "over", If(AssignedHours<(MaxHours*0.5), "under", "ok"))`, if the max hours is greater than 0;<br>2. otherwise the literal “under”. |
| **DR-9 Event Name** | A shift's event name is the name of the shift's event. |
| **DR-10 Filled Count** | A shift's filled count is the number of assignments related to the shift. |
| **DR-11 Coverage Status** | The shift's coverage status is determined by the following priority:<br>1. the literal “understaffed”, if the filled count is less than the needed count;<br>2. the literal “overstaffed”, if the filled count is greater than the needed count;<br>3. otherwise the literal “covered”. |
| **DR-12 Volunteer Name** | An assignment's volunteer name is the name of the assignment's volunteer. |
| **DR-13 Shift Name** | An assignment's shift name is the name of the assignment's shift. |
| **DR-14 Shift Duration Hours** | An assignment's shift duration hours is the duration hours of the assignment's shift. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Events.ShiftCount** | rollup | `Count(Shifts via Event)` |
| **Events.TotalSlotsNeeded** | rollup | `Sum(Shifts.NeededCount via Event)` |
| **Events.TotalSlotsFilled** | rollup | `Sum(Shifts.FilledCount via Event)` |
| **Events.CoveragePercent** | formula | `If(TotalSlotsNeeded>0, TotalSlotsFilled/TotalSlotsNeeded, 0)` |
| **Events.StaffingGrade** | formula | `If(CoveragePercent>=0.95, "A", If(CoveragePercent>=0.85, "B", If(CoveragePercent>=0.70, "C", If(CoveragePercent>=0.50, "D", "F"))))` |
| **Volunteers.AssignmentCount** | rollup | `Count(Assignments via Volunteer)` |
| **Volunteers.AssignedHours** | rollup | `Sum(Assignments.ShiftDurationHours via Volunteer)` |
| **Volunteers.LoadStatus** | formula | `If(MaxHours>0, If(AssignedHours>MaxHours, "over", If(AssignedHours<(MaxHours*0.5), "under", "ok")), "under")` |
| **Shifts.EventName** | lookup | `Lookup(Events.Name via Event)` |
| **Shifts.FilledCount** | rollup | `Count(Assignments via Shift)` |
| **Shifts.CoverageStatus** | formula | `If(FilledCount<NeededCount, "understaffed", If(FilledCount>NeededCount, "overstaffed", "covered"))` |
| **Assignments.VolunteerName** | lookup | `Lookup(Volunteers.Name via Volunteer)` |
| **Assignments.ShiftName** | lookup | `Lookup(Shifts.Name via Shift)` |
| **Assignments.ShiftDurationHours** | lookup | `Lookup(Shifts.DurationHours via Shift)` |
