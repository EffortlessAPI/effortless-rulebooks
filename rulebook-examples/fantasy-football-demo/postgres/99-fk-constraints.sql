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

-- RosterAssignments
ALTER TABLE roster_assignments DROP CONSTRAINT IF EXISTS fk_roster_assignments_roster;
ALTER TABLE roster_assignments ADD CONSTRAINT fk_roster_assignments_roster
  FOREIGN KEY (roster) REFERENCES rosters (roster_id);
ALTER TABLE roster_assignments DROP CONSTRAINT IF EXISTS fk_roster_assignments_player;
ALTER TABLE roster_assignments ADD CONSTRAINT fk_roster_assignments_player
  FOREIGN KEY (player) REFERENCES players (player_id);

-- Matchups
ALTER TABLE matchups DROP CONSTRAINT IF EXISTS fk_matchups_team1;
ALTER TABLE matchups ADD CONSTRAINT fk_matchups_team1
  FOREIGN KEY (team1) REFERENCES rosters (roster_id);
ALTER TABLE matchups DROP CONSTRAINT IF EXISTS fk_matchups_team2;
ALTER TABLE matchups ADD CONSTRAINT fk_matchups_team2
  FOREIGN KEY (team2) REFERENCES rosters (roster_id);

-- 4 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
