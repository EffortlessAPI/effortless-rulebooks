-- dump-raw-facts.sql
--
-- Emits a single JSON object whose keys are PascalCase table names (matching
-- the rulebook's top-level keys) and whose values are arrays of row objects
-- pulled from the corresponding Postgres base table.
--
-- ── ARCHITECTURE NOTES ─────────────────────────────────────────────────────
-- Reads ONLY from base tables (the `01-*.sql` layer), NOT from `vw_*` views.
-- The explainer DAG re-derives every calculated / lookup / aggregation field
-- client-side from raw inputs — pulling pre-computed values would mask the
-- provenance story the DAG is meant to expose.
--
-- ── NAMING CONVENTION ──────────────────────────────────────────────────────
-- Rulebook table:   "Orders"     (PascalCase, plural)
-- Postgres base:     orders       (snake_case, lowercased, plural)
-- We rebuild the PascalCase key by initcap-ing each underscore segment.
--
-- ── OUTPUT ─────────────────────────────────────────────────────────────────
-- A single JSON object printed to stdout:
--   { "Orders": [ {...}, ... ], "Customers": [ {...}, ... ], ... }

\pset format unaligned
\pset tuples_only on

CREATE OR REPLACE FUNCTION pg_temp.erb_dump_table(tbl text)
RETURNS jsonb LANGUAGE plpgsql AS $$
DECLARE
  result jsonb;
BEGIN
  EXECUTE format(
    'SELECT coalesce(jsonb_agg(to_jsonb(t)), ''[]''::jsonb) FROM %I t',
    tbl
  ) INTO result;
  RETURN result;
END;
$$;

SELECT jsonb_object_agg(pascal_key, pg_temp.erb_dump_table(sql_name))::text
FROM (
  SELECT
    string_agg(initcap(part), '' ORDER BY ord) AS pascal_key,
    table_name                                  AS sql_name
  FROM (
    SELECT table_name
    FROM information_schema.tables
    WHERE table_schema = 'public'
      AND table_type   = 'BASE TABLE'
      AND table_name NOT LIKE 'vw_%'
      AND table_name NOT LIKE 'pg_%'
      AND table_name NOT LIKE 'sql_%'
      AND table_name NOT IN ('schema_migrations', 'ar_internal_metadata')
  ) bt,
  LATERAL (
    SELECT part, ord
    FROM unnest(string_to_array(bt.table_name, '_')) WITH ORDINALITY AS u(part, ord)
  ) parts
  GROUP BY table_name
) keyed;
