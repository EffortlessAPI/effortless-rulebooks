-- ============================================================================
-- CUSTOMIZE FUNCTIONS - User-defined functions customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main functions script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom functions changes will appear here:


-- Override: transpiler v2026.05.09.1738 mis-quotes a single-criterion COUNT
-- against the literally-named "Session" FK column on GoalUpdates, emitting
-- `WHERE NULLIF = (SELECT NULLIF(...))` instead of `WHERE "session" = ...`.
-- AVERAGEIFS / MAXIFS on the same column work fine, so this is local to COUNT.
CREATE OR REPLACE FUNCTION calc_sessions_update_count(p_sessions_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer FROM goal_updates WHERE "session" = p_sessions_id;
$$ LANGUAGE sql STABLE;
