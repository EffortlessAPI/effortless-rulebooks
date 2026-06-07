# 📘 Community Event Planner — RuleSpeak

_Venues, events, speakers, attendees with capacity, scheduling, and attendance-forecast cascades._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Venue** | A venue tracked by the business. |
| **Speaker** | A speaker tracked by the business. |
| Assignment Count | Total number of event assignments for this speaker |
| Is Overbooked | Is speaker assigned to more than 3 events (workload alert)? |
| **Event** | An event tracked by the business. |
| Venue Name | Looked up venue name |
| Venue Capacity | Looked up venue capacity |
| Event End | Calculated end time |
| Total Speakers Assigned | Count of speakers assigned to this event |
| Has Speakers | Is at least one speaker assigned? |
| Booked Capacity | Total attendees booked for this event |
| Available Capacity | Remaining capacity after bookings |
| At Capacity | Is event at or over capacity? |
| Venue Conflict Count | Count of other events at same venue within 30 min window |
| Has Venue Conflict | Are there venue scheduling conflicts? |
| Event Status | Event readiness status |
| Registration Deadline | Calculated registration deadline (EventDate minus RegistrationCloseDaysBeforeEvent) |
| Is Registration Open | Is registration still open (before deadline)? |
| Capacity Headroom | Available capacity minus booked attendees |
| Has Headroom for Overbooking | Is there enough headroom to safely overbook by 20%? |
| Recommended Overbooking Factor | Safe overbooking multiplier (if headroom allows) |
| **Assignment** | An assignment tracked by the business. |
| Event Name | Looked up event name |
| Speaker Name | Looked up speaker name |
| Event Date | Looked up event date |
| Speaker Avail Start | Looked up speaker availability start |
| Speaker Avail End | Looked up speaker availability end |
| Is Available | Is speaker available for this event? |
| **Attendee** | An attendee tracked by the business. |
| **RSV P** | An RSV p tracked by the business. |
| Event Name | Looked up event name |
| Attendee Name | Looked up attendee name |

## 2 Fact Types

- an **event** references exactly one **venue**
- an **assignment** references exactly one **event**
- an **assignment** references exactly one **speaker**
- an **RSV p** references exactly one **event**
- an **RSV p** references exactly one **attendee**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Assignment Count** | A speaker's assignment count is the number of assignments related to the speaker. |
| **DR-2 Is Overbooked** | A speaker is considered overbooked if the assignment count is greater than 3. |
| **DR-3 Venue Name** | An event's venue name is carried over from the related record (`Venue`). |
| **DR-4 Venue Capacity** | An event's venue capacity is the capacity of the event's venue. |
| **DR-5 Event End** | An event's event end is computed as the event date. |
| **DR-6 Total Speakers Assigned** | An event's total speakers assigned is the number of assignments related to the event. |
| **DR-7 Has Speakers** | An event is considered to have a speakers if the total speakers assigned is greater than 0. |
| **DR-8 Booked Capacity** | An event's booked capacity is rolled up from its related records (`Count(RSVPs.EventRef, EventId, RSVPs.Status, "confirmed")`). |
| **DR-9 Available Capacity** | An event's available capacity is computed as `VenueCapacity - BookedCapacity`. |
| **DR-10 At Capacity** | An event is flagged at capacity if the available capacity is at most 0. |
| **DR-11 Venue Conflict Count** | An event's venue conflict count is the number of events related to the event. |
| **DR-12 Has Venue Conflict** | An event is considered to have a venue conflict if the venue conflict count is greater than 1. |
| **DR-13 Event Status** | The event's event status is determined by the following priority:<br>1. the literal “ready”, if all of the following hold: the has speakers flag is set; it is not the case that the at capacity flag is set; and it is not the case that the has venue conflict flag is set;<br>2. otherwise the literal “issues”. |
| **DR-14 Registration Deadline** | An event's registration deadline is computed as `EventDate - RegistrationCloseDaysBeforeEvent`. |
| **DR-15 Is Registration Open** | An event is considered registration open if the registration deadline is greater than `Now()`. |
| **DR-16 Capacity Headroom** | An event's capacity headroom is computed as `AvailableCapacity - BookedCapacity`. |
| **DR-17 Has Headroom for Overbooking** | An event is considered to have headroom for overbooking if the capacity headroom is at least `(BookedCapacity * 0.2)`. |
| **DR-18 Recommended Overbooking Factor** | The event's recommended overbooking factor is determined by the following priority:<br>1. 1.2, if the has headroom for overbooking flag is set;<br>2. otherwise 1.0. |
| **DR-19 Event Name** | An assignment's event name is carried over from the related record (`EventRef`). |
| **DR-20 Speaker Name** | An assignment's speaker name is carried over from the related record (`SpeakerRef`). |
| **DR-21 Event Date** | An assignment's event date is the event date of the assignment's event ref. |
| **DR-22 Speaker Avail Start** | An assignment's speaker avail start is the availability start of the assignment's speaker ref. |
| **DR-23 Speaker Avail End** | An assignment's speaker avail end is the availability end of the assignment's speaker ref. |
| **DR-24 Is Available** | An assignment is considered an available if all of the following hold: the event date is at least the speaker avail start and the event date is at most the speaker avail end. |
| **DR-25 Event Name** | An RSV p's event name is carried over from the related record (`EventRef`). |
| **DR-26 Attendee Name** | An RSV p's attendee name is carried over from the related record (`AttendeeRef`). |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Speakers.AssignmentCount** | rollup | `Count(Assignments via SpeakerRef)` |
| **Speakers.IsOverbooked** | formula | `AssignmentCount > 3` |
| **Events.VenueName** | lookup | `Venue` |
| **Events.VenueCapacity** | lookup | `Lookup(Venues.Capacity via Venue)` |
| **Events.EventEnd** | formula | `EventDate` |
| **Events.TotalSpeakersAssigned** | rollup | `Count(Assignments via EventRef)` |
| **Events.HasSpeakers** | formula | `TotalSpeakersAssigned > 0` |
| **Events.BookedCapacity** | rollup | `Count(RSVPs.EventRef, EventId, RSVPs.Status, "confirmed")` |
| **Events.AvailableCapacity** | formula | `VenueCapacity - BookedCapacity` |
| **Events.AtCapacity** | formula | `AvailableCapacity <= 0` |
| **Events.VenueConflictCount** | rollup | `Count(Events via Venue)` |
| **Events.HasVenueConflict** | formula | `VenueConflictCount > 1` |
| **Events.EventStatus** | formula | `If(And(HasSpeakers, Not(AtCapacity), Not(HasVenueConflict)), "ready", "issues")` |
| **Events.RegistrationDeadline** | formula | `EventDate - RegistrationCloseDaysBeforeEvent` |
| **Events.IsRegistrationOpen** | formula | `RegistrationDeadline > Now()` |
| **Events.CapacityHeadroom** | formula | `AvailableCapacity - BookedCapacity` |
| **Events.HasHeadroomForOverbooking** | formula | `CapacityHeadroom >= (BookedCapacity * 0.2)` |
| **Events.RecommendedOverbookingFactor** | formula | `If(HasHeadroomForOverbooking, 1.2, 1.0)` |
| **Assignments.EventName** | lookup | `EventRef` |
| **Assignments.SpeakerName** | lookup | `SpeakerRef` |
| **Assignments.EventDate** | lookup | `Lookup(Events.EventDate via EventRef)` |
| **Assignments.SpeakerAvailStart** | lookup | `Lookup(Speakers.AvailabilityStart via SpeakerRef)` |
| **Assignments.SpeakerAvailEnd** | lookup | `Lookup(Speakers.AvailabilityEnd via SpeakerRef)` |
| **Assignments.IsAvailable** | formula | `And(EventDate >= SpeakerAvailStart, EventDate <= SpeakerAvailEnd)` |
| **RSVPs.EventName** | lookup | `EventRef` |
| **RSVPs.AttendeeName** | lookup | `AttendeeRef` |
