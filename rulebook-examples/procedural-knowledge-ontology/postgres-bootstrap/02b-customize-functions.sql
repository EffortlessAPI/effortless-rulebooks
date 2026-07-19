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

-- ---------------------------------------------------------------------------
-- Repair: ActualDurationMinutes must round to an integer, not cast via text.
--
-- The rulebook is correct:
--   ActualDurationMinutes | datatype "integer" | calculated
--   =IF({{EndedAt}} = "", 0, DATETIME_DIFF({{EndedAt}}, {{StartedAt}}, "minutes"))
--
-- But rulebook-to-postgres compiled DATETIME_DIFF(...,"minutes") to
--   EXTRACT(EPOCH FROM (ended_at - started_at)) / 60
-- which yields NUMERIC, then round-tripped it through ::text before ::integer:
--
--   ((EXTRACT(EPOCH FROM (...)) / 60))::text ... ::integer
--
-- Postgres will cast numeric->integer (rounding), but it will NOT cast the
-- TEXT '4.0000000000000000' to integer. So the generated function raises:
--
--   ERROR: invalid input syntax for type integer: "4.0000000000000000"
--
-- That error fires on ANY select touching the column, which means vw_step_executions
-- -- the single most important view in this domain -- was completely unreadable,
-- and IsLate (which calls this function) was dead too.
--
-- Fix: same formula, same semantics, without the text round-trip. ROUND() makes
-- the integer conversion explicit rather than implicit, so a 4.5-minute step
-- rounds rather than silently truncating.
-- ---------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION calc_step_executions_actual_duration_minutes(p_step_execution_id TEXT)
RETURNS INTEGER AS $$
  SELECT CASE
    WHEN se.ended_at IS NULL THEN 0
    ELSE ROUND(EXTRACT(EPOCH FROM (se.ended_at::timestamptz - se.started_at::timestamptz)) / 60.0)::integer
  END
  FROM step_executions se
  WHERE se.step_execution_id = p_step_execution_id;
$$ LANGUAGE sql STABLE;

