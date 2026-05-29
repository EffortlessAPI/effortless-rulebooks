-- Override: calc_covenants_has_active_waiver
-- Field: Covenants.HasActiveWaiver
-- Rulebook formula: =NOT(COALESCE({{CurrentWaiverThrough}}, "") = "")
--
-- WHY THIS OVERRIDE EXISTS
-- The rulebook formula models "is blank" with the text idiom COALESCE(x, "") = "".
-- CurrentWaiverThrough is a `timestamptz` column, and rulebook-to-postgres
-- (v2026.05.29.1925) rendered it literally as:
--
--     COALESCE((SELECT current_waiver_through::timestamptz ...), '')
--
-- Postgres must coerce the '' literal to the COALESCE common type (timestamptz),
-- and ''::timestamptz raises 22007 (DateTimeParseError) at PLAN time — so the
-- generated function errors on EVERY row, regardless of data. (This is a
-- transpiler bug, not a data bug: the column holds proper NULLs.)
--
-- For a datetime, "blank" == NULL. Express the same intent type-correctly.
-- init-db.sh applies function-overrides/*.sql AFTER 02-create-functions.sql,
-- so this CREATE OR REPLACE wins and survives rebuilds of the generated files.
CREATE OR REPLACE FUNCTION calc_covenants_has_active_waiver(p_covenants_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT ((SELECT current_waiver_through
             FROM covenants
            WHERE covenants_id = p_covenants_id) IS NOT NULL)::boolean;
$$ LANGUAGE sql STABLE;
