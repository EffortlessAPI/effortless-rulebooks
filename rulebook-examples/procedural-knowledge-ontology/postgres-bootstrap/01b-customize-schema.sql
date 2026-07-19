-- ============================================================================
-- CUSTOMIZE SCHEMA - User-defined schema customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main schema script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom schema changes will appear here:

-- ---------------------------------------------------------------------------
-- Repair: quiet-hours columns must be TEXT, not DATE.
--
-- The rulebook declares both fields as datatype "string":
--
--   { "name": "QuietHoursStart", "datatype": "string", "type": "raw", ... }
--   { "name": "QuietHoursEnd",   "datatype": "string", "type": "raw", ... }
--
-- but rulebook-to-postgres emitted `DATE` for them in 01-drop-and-create-tables.sql
-- (it appears to infer a temporal type from the field NAME rather than honoring
-- the declared datatype). The seeded values are wall-clock times of day —
-- '20:00', '08:00', '00:00' — which are not dates, so 05-insert-data.sql aborts:
--
--   ERROR: invalid input syntax for type date: "00:00"
--
-- The rulebook is the source of truth and it is already correct, so the fix
-- belongs here rather than in the rulebook. This runs after the generated
-- schema and before the data load, and is idempotent.
--
-- These are clock times with no date component and no timezone (they are
-- interpreted in the RECIPIENT's local timezone per the field description), so
-- TEXT is the honest column type — TIME would silently imply a UTC-anchorable
-- instant that the domain does not mean.
-- ---------------------------------------------------------------------------
-- On a re-run, vw_communication_policies (created by 03-create-views.sql in the
-- PREVIOUS run) still depends on these columns, and Postgres refuses to retype a
-- column a view selects. Dropping it here is safe and not destructive: the view
-- is generated, holds no data, and 03-create-views.sql recreates it later in
-- this very same init-db.sh run, after the retype has landed.
DROP VIEW IF EXISTS vw_communication_policies CASCADE;

ALTER TABLE communication_policies
    ALTER COLUMN quiet_hours_start TYPE TEXT USING quiet_hours_start::TEXT;
ALTER TABLE communication_policies
    ALTER COLUMN quiet_hours_end   TYPE TEXT USING quiet_hours_end::TEXT;

