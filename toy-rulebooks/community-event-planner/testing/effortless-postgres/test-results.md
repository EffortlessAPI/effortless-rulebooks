# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 182 |
| Passed | 161 |
| Failed | 21 |
| Score | 88.5% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 86 | 93 | 92.5% |
| Lookup (INDEX/MATCH) | 58 | 72 | 80.6% |
| Aggregation (COUNTIFS/SUMIFS) | 17 | 17 | 100.0% |

## Results by Entity

### venues

- Fields: 3/3 (100.0%)
- Computed columns: name

### speakers

- Fields: 15/15 (100.0%)
- Computed columns: name, assignment_count, is_overbooked

### events

- Fields: 68/68 (100.0%)
- Computed columns: name, venue_name, venue_capacity, event_end, total_speakers_assigned, has_speakers, booked_capacity, available_capacity, at_capacity, venue_conflict_count, has_venue_conflict, event_status, registration_deadline, is_registration_open, capacity_headroom, has_headroom_for_overbooking, recommended_overbooking_factor

### assignments

- Fields: 70/70 (100.0%)
- Computed columns: name, event_name, speaker_name, event_date, speaker_avail_start, speaker_avail_end, is_available

### attendees

- Fields: 5/5 (100.0%)
- Computed columns: name

### rsv_ps

- Fields: 0/21 (0.0%)
- Computed columns: name, event_name, attendee_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rsvp-1 | name | None | rsvp-1 |
| rsvp-1 | event_name | None | tech-talk-1 |
| rsvp-1 | attendee_name | None | user1@attendees.local |
| rsvp-2 | name | None | rsvp-2 |
| rsvp-2 | event_name | None | tech-talk-1 |
| rsvp-2 | attendee_name | None | user2@attendees.local |
| rsvp-3 | name | None | rsvp-3 |
| rsvp-3 | event_name | None | workshop-1 |
| rsvp-3 | attendee_name | None | user1@attendees.local |
| rsvp-4 | name | None | rsvp-4 |
| rsvp-4 | event_name | None | workshop-1 |
| rsvp-4 | attendee_name | None | user3@attendees.local |
| rsvp-5 | name | None | rsvp-5 |
| rsvp-5 | event_name | None | panel-1 |
| rsvp-5 | attendee_name | None | user4@attendees.local |
| rsvp-6 | name | None | rsvp-6 |
| rsvp-6 | event_name | None | panel-1 |
| rsvp-6 | attendee_name | None | user5@attendees.local |
| rsvp-7 | name | None | rsvp-7 |
| rsvp-7 | event_name | None | meetup-1 |
| ... | ... | (1 more) | ... |
