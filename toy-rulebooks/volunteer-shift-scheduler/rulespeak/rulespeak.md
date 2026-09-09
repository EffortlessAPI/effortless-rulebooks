# 📘 Volunteer Shift Scheduler — RuleSpeak®

_Coverage status, volunteer load (under/ok/over), and event-level A–F staffing grade all fall out automatically._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Event** | An event is identified by its name. | — |
| Name | A defined attribute. | _Display name of the event._ |
| Event Date | A defined attribute. | — |
| Shift Count | The number of shifts related to the event. | _Total shifts attached to this event._ |
| Total Slots Needed | The total needed count across the shifts related to the event. | _Sum of needed-count across all shifts in this event._ |
| Total Slots Filled | The total filled count across the shifts related to the event. | _Sum of filled-count across all shifts in this event._ |
| Coverage Percent | Determined by priority: the total slots filled divided by the total slots needed if the total slots needed is greater than 0; in all other cases, 0. | _TotalSlotsFilled / TotalSlotsNeeded (0..1)._ |
| Staffing Grade | Determined by priority: “A” if the coverage percent is at least 0.95; “B” if the coverage percent is at least 0.85; “C” if the coverage percent is at least 0.70; “D” if the coverage percent is at least 0.50; in all other cases, “F”. | _A-F letter grade from CoveragePercent. A: >=95%, B: >=85%, C: >=70%, D: >=50%, F: below._ |
| **Volunteer** | A volunteer is identified by its name. | — |
| Name | Computed as the first name, followed by a space, followed by the last name. | _Display name._ |
| First Name | A defined attribute. | — |
| Last Name | A defined attribute. | — |
| Email Address | A defined attribute. | _Used as the click-to-login identity._ |
| Reliability Score | A defined attribute. | _0..1 — coordinator's confidence the volunteer will show up. Drives per-shift coverage weighting._ |
| Max Hours | A defined attribute. | _Volunteer's stated maximum hours for this scheduling period._ |
| Assignment Count | The number of assignments related to the volunteer. | _Number of shifts this volunteer is assigned to._ |
| Assigned Hours | The total shift duration hours across the assignments related to the volunteer. | _Sum of DurationHours across this volunteer's assigned shifts._ |
| Load Status | Determined by priority: “over” if the assigned hours is greater than the max hours, in all other cases “under” if the assigned hours is less than the max hours times 0.5, in all other cases “ok” if the max hours is greater than 0; in all other cases, “under”. | _under \| ok \| over. Under: <50% of MaxHours. Over: >100%. Otherwise ok._ |
| **Shift** | A shift is identified by its name and is related to optionally an event. | — |
| Name | A defined attribute. | _Short label (e.g. "Setup Morning")._ |
| Event | A defined attribute. | _Parent event._ |
| Event Name | Taken from the linked event. | — |
| Required Skill | A defined attribute. | _Free-text skill tag (e.g. "setup", "intake", "food-handler")._ |
| Duration Hours | A defined attribute. | _Length of the shift in hours; drives per-volunteer load._ |
| Needed Count | A defined attribute. | _How many volunteers the coordinator wants on this shift._ |
| Filled Count | The number of assignments related to the shift. | _Number of volunteers currently assigned._ |
| Coverage Status | Determined by priority: “understaffed” if the filled count is less than the needed count; “overstaffed” if the filled count is greater than the needed count; in all other cases, “covered”. | _understaffed \| covered \| overstaffed._ |
| **Assignment** | An assignment is identified by its name and is related to optionally a volunteer and optionally a shift. | — |
| Name | Computed as the volunteer name, followed by “ @ ”, followed by the shift name. | _Display label (VolunteerName @ ShiftName)._ |
| Volunteer | A defined attribute. | — |
| Shift | A defined attribute. | — |
| Volunteer Name | Taken from the linked volunteer. | — |
| Shift Name | Taken from the linked shift. | — |
| Shift Duration Hours | Taken from the linked shift. | _Pulled through to support per-volunteer AssignedHours aggregation._ |

## 2 Fact Types

- a **shift** may reference one **event**
- an **assignment** may reference one **volunteer**
- an **assignment** may reference one **shift**

## 3 Operative Rules

_No operative rules yet. Required fields and foreign keys imply structural
`must`-rules automatically; to declare semantic obligations (`must` / `must not` / `should`), add a **Constraints** table whose rows point at
boolean calculated fields. See the tool README for the column contract._

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Shift Count** | An event's shift count is the number of shifts related to the event. |
| **DR-2 Total Slots Needed** | An event's total slots needed is the total needed count across the shifts related to the event. |
| **DR-3 Total Slots Filled** | An event's total slots filled is the total filled count across the shifts related to the event. |
| **DR-4 Coverage Percent** | The event's coverage percent is determined by the following priority:<br>1. the total slots filled divided by the total slots needed, if the total slots needed is greater than 0;<br>2. in all other cases, 0. |
| **DR-5 Staffing Grade** | The event's staffing grade is determined by the following priority:<br>1. “A”, if the coverage percent is at least 0.95;<br>2. “B”, if the coverage percent is at least 0.85;<br>3. “C”, if the coverage percent is at least 0.70;<br>4. “D”, if the coverage percent is at least 0.50;<br>5. in all other cases, “F”. |
| **DR-6 Name** | A volunteer's name is computed as the first name, followed by a space, followed by the last name. |
| **DR-7 Assignment Count** | A volunteer's assignment count is the number of assignments related to the volunteer. |
| **DR-8 Assigned Hours** | A volunteer's assigned hours is the total shift duration hours across the assignments related to the volunteer. |
| **DR-9 Load Status** | The volunteer's load status is determined by the following priority:<br>1. “over” if the assigned hours is greater than the max hours, in all other cases “under” if the assigned hours is less than the max hours times 0.5, in all other cases “ok”, if the max hours is greater than 0;<br>2. in all other cases, “under”. |
| **DR-10 Event Name** | A shift's event name — taken from the linked event. |
| **DR-11 Filled Count** | A shift's filled count is the number of assignments related to the shift. |
| **DR-12 Coverage Status** | The shift's coverage status is determined by the following priority:<br>1. “understaffed”, if the filled count is less than the needed count;<br>2. “overstaffed”, if the filled count is greater than the needed count;<br>3. in all other cases, “covered”. |
| **DR-13 Name** | An assignment's name is computed as the volunteer name, followed by “ @ ”, followed by the shift name. |
| **DR-14 Volunteer Name** | An assignment's volunteer name — taken from the linked volunteer. |
| **DR-15 Shift Name** | An assignment's shift name — taken from the linked shift. |
| **DR-16 Shift Duration Hours** | An assignment's shift duration hours — taken from the linked shift. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Events.ShiftCount** | rollup | `Count(Shifts via Event)` |
| **Events.TotalSlotsNeeded** | rollup | `Sum(Shifts.NeededCount via Event)` |
| **Events.TotalSlotsFilled** | rollup | `Sum(Shifts.FilledCount via Event)` |
| **Events.CoveragePercent** | formula | `If(TotalSlotsNeeded > 0, TotalSlotsFilled / TotalSlotsNeeded, 0)` |
| **Events.StaffingGrade** | formula | `If(CoveragePercent >= 0.95, "A", If(CoveragePercent >= 0.85, "B", If(CoveragePercent >= 0.70, "C", If(CoveragePercent >= 0.50, "D", "F"))))` |
| **Volunteers.Name** | formula | `FirstName & " " & LastName` |
| **Volunteers.AssignmentCount** | rollup | `Count(Assignments via Volunteer)` |
| **Volunteers.AssignedHours** | rollup | `Sum(Assignments.ShiftDurationHours via Volunteer)` |
| **Volunteers.LoadStatus** | formula | `If(MaxHours > 0, If(AssignedHours > MaxHours, "over", If(AssignedHours < MaxHours * 0.5, "under", "ok")), "under")` |
| **Shifts.EventName** | lookup | `Lookup(Events.Name via Event)` |
| **Shifts.FilledCount** | rollup | `Count(Assignments via Shift)` |
| **Shifts.CoverageStatus** | formula | `If(FilledCount < NeededCount, "understaffed", If(FilledCount > NeededCount, "overstaffed", "covered"))` |
| **Assignments.Name** | formula | `VolunteerName & " @ " & ShiftName` |
| **Assignments.VolunteerName** | lookup | `Lookup(Volunteers.Name via Volunteer)` |
| **Assignments.ShiftName** | lookup | `Lookup(Shifts.Name via Shift)` |
| **Assignments.ShiftDurationHours** | lookup | `Lookup(Shifts.DurationHours via Shift)` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
