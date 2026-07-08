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

-- (loop-90) Studies.PublicationYear/Domain/IsSynthetic were previously
-- duplicated here via ADD COLUMN IF NOT EXISTS. Confirmed dead: the native
-- transpiler output (01-drop-and-create-tables.sql) already emits these
-- columns directly from the rulebook's Studies schema. Removed.

-- (loop-90) The _erb_tr_metrics / _erb_sp_metrics bespoke cache tables that
-- lived here have been eliminated. Every column they held is now a real
-- calculated/lookup/aggregation field on TreatmentRankings, ModelSummary, or
-- SyntheticPhase in the rulebook, computed live by the native
-- 02-create-functions.sql — see effortless-rulebook/simpsons-paradox-rulebook.json.
