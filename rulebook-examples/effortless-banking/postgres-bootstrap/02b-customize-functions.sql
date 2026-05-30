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

-- NOTE: The cross-table FK lookup bridges that previously lived here are no longer
-- needed. As of rulebook-to-postgres v2026.05.29.1853 (pk_type, default "string"),
-- the transpiler keeps FK values consistent with the PK they reference, so the
-- generated 02-create-functions.sql joins resolve correctly on their own.
