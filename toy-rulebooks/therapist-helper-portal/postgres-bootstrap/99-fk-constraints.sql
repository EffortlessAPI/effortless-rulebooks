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

-- Clients
ALTER TABLE clients DROP CONSTRAINT IF EXISTS fk_clients_therapist;
ALTER TABLE clients ADD CONSTRAINT fk_clients_therapist
  FOREIGN KEY (therapist) REFERENCES users (users_id);

-- Goals
ALTER TABLE goals DROP CONSTRAINT IF EXISTS fk_goals_client;
ALTER TABLE goals ADD CONSTRAINT fk_goals_client
  FOREIGN KEY (client) REFERENCES clients (clients_id);

-- Sessions
ALTER TABLE sessions DROP CONSTRAINT IF EXISTS fk_sessions_client;
ALTER TABLE sessions ADD CONSTRAINT fk_sessions_client
  FOREIGN KEY (client) REFERENCES clients (clients_id);

-- GoalUpdates
ALTER TABLE goal_updates DROP CONSTRAINT IF EXISTS fk_goal_updates_goal;
ALTER TABLE goal_updates ADD CONSTRAINT fk_goal_updates_goal
  FOREIGN KEY (goal) REFERENCES goals (goals_id);
ALTER TABLE goal_updates DROP CONSTRAINT IF EXISTS fk_goal_updates_session;
ALTER TABLE goal_updates ADD CONSTRAINT fk_goal_updates_session
  FOREIGN KEY ("session") REFERENCES sessions (sessions_id);

-- 5 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
