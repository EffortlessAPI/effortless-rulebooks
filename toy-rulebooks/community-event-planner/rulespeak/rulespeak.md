# 📘 Community Event Planner — RuleSpeak®

_Venues, events, speakers, attendees with capacity, scheduling, and attendance-forecast cascades._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Venue** | A venue is identified by its name. | — |
| Name | The same as its venue ID. | _Display name for the venue_ |
| Location | A defined attribute. | _Physical location/address_ |
| Capacity | A defined attribute. | _Total seating capacity_ |
| **Speaker** | A speaker is identified by its name. | — |
| Name | The same as its speaker ID. | _Display name for the speaker_ |
| Availability Start | A defined attribute. | _Start of speaker availability window_ |
| Availability End | A defined attribute. | _End of speaker availability window_ |
| Assignment Count | The number of assignments related to the speaker. | _Total number of event assignments for this speaker_ |
| Is Overbooked | True when the assignment count is greater than 3. | _Is speaker assigned to more than 3 events (workload alert)?_ |
| **Event** | An event is identified by its name and is related to a venue. | — |
| Name | The same as its event ID. | _Display name for the event_ |
| Venue | A defined attribute. | _Related venue_ |
| Venue Name | The venue. | _Looked up venue name_ |
| Venue Capacity | Taken from the linked venue. | _Looked up venue capacity_ |
| Event Date | A defined attribute. | _Event date and start time_ |
| Duration Minutes | A defined attribute. | _Event duration in minutes_ |
| Category | A defined attribute. | _Event category or track (e.g. Tech, Community, Learning)_ |
| Event End | The same as its event date. | _Calculated end time_ |
| Total Speakers Assigned | The number of assignments related to the event. | _Count of speakers assigned to this event_ |
| Has Speakers | True when the total speakers assigned is greater than 0. | _Is at least one speaker assigned?_ |
| Booked Capacity | The number of the event's RSV ps that have a status of “confirmed”. | _Total attendees booked for this event_ |
| Available Capacity | Computed as the venue capacity minus the booked capacity. | _Remaining capacity after bookings_ |
| At Capacity | True when the available capacity is at most 0. | _Is event at or over capacity?_ |
| Venue Conflict Count | The number of events related to the event. | _Count of other events at same venue within 30 min window_ |
| Has Venue Conflict | True when the venue conflict count is greater than 1. | _Are there venue scheduling conflicts?_ |
| Event Status | Determined by priority: “ready” if all of the following hold: the speakers flag is set; the at capacity flag is not set; and the venue conflict flag is not set; in all other cases, “issues”. | _Event readiness status_ |
| Registration Close Days Before Event | A defined attribute. | _Number of days before the event that registration closes_ |
| Registration Deadline | Computed as the event date minus the registration close days before event. | _Calculated registration deadline (EventDate minus RegistrationCloseDaysBeforeEvent)_ |
| Is Registration Open | True when the registration deadline is greater than the current date and time. | _Is registration still open (before deadline)?_ |
| Capacity Headroom | Computed as the available capacity minus the booked capacity. | _Available capacity minus booked attendees_ |
| Has Headroom for Overbooking | True when the capacity headroom is at least the booked capacity times 0.2. | _Is there enough headroom to safely overbook by 20%?_ |
| Recommended Overbooking Factor | Determined by priority: 1.2 if the headroom for overbooking flag is set; in all other cases, 1.0. | _Safe overbooking multiplier (if headroom allows)_ |
| **Assignment** | An assignment is identified by its name and is related to an event (its event ref) and a speaker (its speaker ref). | — |
| Name | The same as its assignment ID. | _Display name_ |
| Event Ref | A defined attribute. | _Related event_ |
| Event Name | The event ref. | _Looked up event name_ |
| Speaker Ref | A defined attribute. | _Related speaker_ |
| Speaker Name | The speaker ref. | _Looked up speaker name_ |
| Event Date | Taken from the linked event ref. | _Looked up event date_ |
| Speaker Avail Start | The availability start of the assignment's speaker ref. | _Looked up speaker availability start_ |
| Speaker Avail End | The availability end of the assignment's speaker ref. | _Looked up speaker availability end_ |
| Is Available | True when all of the following hold: the event date is at least the speaker avail start and the event date is at most the speaker avail end. | _Is speaker available for this event?_ |
| **Attendee** | An attendee is identified by its name. | — |
| Name | The same as its attendee ID. | _Display name for the attendee_ |
| **RSV P** | An RSV p is identified by its name and is related to an event (its event ref) and an attendee (its attendee ref). | — |
| Name | The same as its RSVP ID. | _Display name_ |
| Event Ref | A defined attribute. | _Related event_ |
| Event Name | The event ref. | _Looked up event name_ |
| Attendee Ref | A defined attribute. | _Related attendee_ |
| Attendee Name | The attendee ref. | _Looked up attendee name_ |
| Status | A defined attribute. | _RSVP status (confirmed, waitlist, declined)_ |

## 2 Fact Types

- an **event** references exactly one **venue**
- an **assignment** references exactly one **event**
- an **assignment** references exactly one **speaker**
- an **RSV p** references exactly one **event**
- an **RSV p** references exactly one **attendee**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A venue **must** have a capacity.
- An event **must** reference exactly one venue.
- An event **must** have an event date, a duration minutes, and a registration close days before event.
- An assignment **must** reference exactly one event as its event ref.
- An assignment **must** reference exactly one speaker as its speaker ref.
- An RSV p **must** reference exactly one event as its event ref.
- An RSV p **must** reference exactly one attendee as its attendee ref.
- An RSV p **must** have a status.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A venue's name is the same as its venue ID. |
| **DR-2 Name** | A speaker's name is the same as its speaker ID. |
| **DR-3 Assignment Count** | A speaker's assignment count is the number of assignments related to the speaker. |
| **DR-4 Is Overbooked** | A speaker is considered overbooked if the assignment count is greater than 3. |
| **DR-5 Name** | An event's name is the same as its event ID. |
| **DR-6 Venue Name** | An event's venue name is the venue. |
| **DR-7 Venue Capacity** | An event's venue capacity — taken from the linked venue. |
| **DR-8 Event End** | An event's event end is the same as its event date. |
| **DR-9 Total Speakers Assigned** | An event's total speakers assigned is the number of assignments related to the event. |
| **DR-10 Has Speakers** | An event is considered to have a speakers if the total speakers assigned is greater than 0. |
| **DR-11 Booked Capacity** | An event's booked capacity is the number of the event's RSV ps that have a status of “confirmed”. |
| **DR-12 Available Capacity** | An event's available capacity is computed as the venue capacity minus the booked capacity. |
| **DR-13 At Capacity** | An event is flagged at capacity if the available capacity is at most 0. |
| **DR-14 Venue Conflict Count** | An event's venue conflict count is the number of events related to the event. |
| **DR-15 Has Venue Conflict** | An event is considered to have a venue conflict if the venue conflict count is greater than 1. |
| **DR-16 Event Status** | The event's event status is determined by the following priority:<br>1. “ready”, if all of the following hold: the speakers flag is set; the at capacity flag is not set; and the venue conflict flag is not set;<br>2. in all other cases, “issues”. |
| **DR-17 Registration Deadline** | An event's registration deadline is computed as the event date minus the registration close days before event. |
| **DR-18 Is Registration Open** | An event is considered registration-open if the registration deadline is greater than the current date and time. |
| **DR-19 Capacity Headroom** | An event's capacity headroom is computed as the available capacity minus the booked capacity. |
| **DR-20 Has Headroom for Overbooking** | An event is considered to have headroom for overbooking if the capacity headroom is at least the booked capacity times 0.2. |
| **DR-21 Recommended Overbooking Factor** | The event's recommended overbooking factor is determined by the following priority:<br>1. 1.2, if the headroom for overbooking flag is set;<br>2. in all other cases, 1.0. |
| **DR-22 Name** | An assignment's name is the same as its assignment ID. |
| **DR-23 Event Name** | An assignment's event name is the event ref. |
| **DR-24 Speaker Name** | An assignment's speaker name is the speaker ref. |
| **DR-25 Event Date** | An assignment's event date — taken from the linked event ref. |
| **DR-26 Speaker Avail Start** | An assignment's speaker avail start is the availability start of the assignment's speaker ref. |
| **DR-27 Speaker Avail End** | An assignment's speaker avail end is the availability end of the assignment's speaker ref. |
| **DR-28 Is Available** | An assignment is considered available if all of the following hold: the event date is at least the speaker avail start and the event date is at most the speaker avail end. |
| **DR-29 Name** | An attendee's name is the same as its attendee ID. |
| **DR-30 Name** | An RSV p's name is the same as its RSVP ID. |
| **DR-31 Event Name** | An RSV p's event name is the event ref. |
| **DR-32 Attendee Name** | An RSV p's attendee name is the attendee ref. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Venues.Name** | formula | `VenueId` |
| **Speakers.Name** | formula | `SpeakerId` |
| **Speakers.AssignmentCount** | rollup | `Count(Assignments via SpeakerRef)` |
| **Speakers.IsOverbooked** | formula | `AssignmentCount > 3` |
| **Events.Name** | formula | `EventId` |
| **Events.VenueName** | lookup | `Venue` |
| **Events.VenueCapacity** | lookup | `Lookup(Venues.Capacity via Venue)` |
| **Events.EventEnd** | formula | `EventDate` |
| **Events.TotalSpeakersAssigned** | rollup | `Count(Assignments via EventRef)` |
| **Events.HasSpeakers** | formula | `TotalSpeakersAssigned > 0` |
| **Events.BookedCapacity** | rollup | `Count(RSVPs via EventRef)` |
| **Events.AvailableCapacity** | formula | `VenueCapacity - BookedCapacity` |
| **Events.AtCapacity** | formula | `AvailableCapacity <= 0` |
| **Events.VenueConflictCount** | rollup | `Count(Events via Venue)` |
| **Events.HasVenueConflict** | formula | `VenueConflictCount > 1` |
| **Events.EventStatus** | formula | `If(And(HasSpeakers, Not(AtCapacity), Not(HasVenueConflict)), "ready", "issues")` |
| **Events.RegistrationDeadline** | formula | `EventDate - RegistrationCloseDaysBeforeEvent` |
| **Events.IsRegistrationOpen** | formula | `RegistrationDeadline > Now()` |
| **Events.CapacityHeadroom** | formula | `AvailableCapacity - BookedCapacity` |
| **Events.HasHeadroomForOverbooking** | formula | `CapacityHeadroom >= BookedCapacity * 0.2` |
| **Events.RecommendedOverbookingFactor** | formula | `If(HasHeadroomForOverbooking, 1.2, 1.0)` |
| **Assignments.Name** | formula | `AssignmentId` |
| **Assignments.EventName** | lookup | `EventRef` |
| **Assignments.SpeakerName** | lookup | `SpeakerRef` |
| **Assignments.EventDate** | lookup | `Lookup(Events.EventDate via EventRef)` |
| **Assignments.SpeakerAvailStart** | lookup | `Lookup(Speakers.AvailabilityStart via SpeakerRef)` |
| **Assignments.SpeakerAvailEnd** | lookup | `Lookup(Speakers.AvailabilityEnd via SpeakerRef)` |
| **Assignments.IsAvailable** | formula | `And(EventDate >= SpeakerAvailStart, EventDate <= SpeakerAvailEnd)` |
| **Attendees.Name** | formula | `AttendeeId` |
| **RSVPs.Name** | formula | `RSVPId` |
| **RSVPs.EventName** | lookup | `EventRef` |
| **RSVPs.AttendeeName** | lookup | `AttendeeRef` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
