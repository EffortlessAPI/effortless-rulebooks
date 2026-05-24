-- ============================================================================
-- 99-fk-constraints.sql — FK CONSTRAINTS (off by default)
-- ============================================================================
-- Demos must never fail on FK violations, so init-db.sh SKIPS this file
-- unless EFFORTLESS_ENFORCE_FKS=true is set in the environment.
--
--   EFFORTLESS_ENFORCE_FKS=true bash init-db.sh    # apply constraints
--   bash init-db.sh                                # leave them documented but unenforced
--
-- The rulebook always documents the FK relationships, and 01-drop-and-create-tables.sql
-- always installs the supporting indexes inline. This file just declares the actual
-- enforcement. Idempotent: every constraint is dropped if present, then added.
-- ============================================================================

-- Events
ALTER TABLE events DROP CONSTRAINT IF EXISTS fk_events_venue;
ALTER TABLE events ADD CONSTRAINT fk_events_venue
  FOREIGN KEY (venue) REFERENCES venues (venue_id);

-- Assignments
ALTER TABLE assignments DROP CONSTRAINT IF EXISTS fk_assignments_event_ref;
ALTER TABLE assignments ADD CONSTRAINT fk_assignments_event_ref
  FOREIGN KEY (event_ref) REFERENCES events (event_id);
ALTER TABLE assignments DROP CONSTRAINT IF EXISTS fk_assignments_speaker_ref;
ALTER TABLE assignments ADD CONSTRAINT fk_assignments_speaker_ref
  FOREIGN KEY (speaker_ref) REFERENCES speakers (speaker_id);

-- RSVPs
ALTER TABLE rsvps DROP CONSTRAINT IF EXISTS fk_rsvps_event_ref;
ALTER TABLE rsvps ADD CONSTRAINT fk_rsvps_event_ref
  FOREIGN KEY (event_ref) REFERENCES events (event_id);
ALTER TABLE rsvps DROP CONSTRAINT IF EXISTS fk_rsvps_attendee_ref;
ALTER TABLE rsvps ADD CONSTRAINT fk_rsvps_attendee_ref
  FOREIGN KEY (attendee_ref) REFERENCES attendees (attendee_id);

-- 5 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
