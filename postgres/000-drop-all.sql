-- ============================================================================
-- 000-drop-all.sql - Drop all views, functions, and tables
-- ============================================================================
-- Run this before recreating the schema to ensure a clean slate
-- ============================================================================

\echo '=== Starting 000-drop-all.sql ==='

-- Step 1: Drop all views
\echo ''
\echo '--- Step 1: Dropping all views ---'

SELECT 'Views to drop: ' || COUNT(*)::text AS info
FROM information_schema.views
WHERE table_schema = 'public';

DO $$
DECLARE
    r RECORD;
    drop_count INTEGER := 0;
BEGIN
    FOR r IN (SELECT table_name FROM information_schema.views WHERE table_schema = 'public')
    LOOP
        RAISE WARNING 'Dropping view: %', r.table_name;
        EXECUTE 'DROP VIEW IF EXISTS public.' || quote_ident(r.table_name) || ' CASCADE';
        drop_count := drop_count + 1;
    END LOOP;
    RAISE WARNING 'Total views dropped: %', drop_count;
END $$;

SELECT 'Views remaining: ' || COUNT(*)::text AS info
FROM information_schema.views
WHERE table_schema = 'public';

-- Step 2: Drop all user-defined functions
\echo ''
\echo '--- Step 2: Dropping all functions ---'

SELECT 'Functions to drop: ' || COUNT(*)::text AS info
FROM pg_proc p
JOIN pg_namespace ns ON p.pronamespace = ns.oid
WHERE ns.nspname = 'public'
  AND p.prokind = 'f';

DO $$
DECLARE
    r RECORD;
    drop_count INTEGER := 0;
    drop_sql TEXT;
BEGIN
    FOR r IN (
        SELECT ns.nspname AS schema_name,
               p.proname AS function_name,
               pg_get_function_identity_arguments(p.oid) AS args
        FROM pg_proc p
        JOIN pg_namespace ns ON p.pronamespace = ns.oid
        WHERE ns.nspname = 'public'
          AND p.prokind = 'f'
    )
    LOOP
        drop_sql := 'DROP FUNCTION IF EXISTS public.' || quote_ident(r.function_name) || '(' || r.args || ') CASCADE';
        RAISE WARNING 'Executing: %', drop_sql;
        EXECUTE drop_sql;
        drop_count := drop_count + 1;
    END LOOP;
    RAISE WARNING 'Total functions dropped: %', drop_count;
END $$;

SELECT 'Functions remaining: ' || COUNT(*)::text AS info
FROM pg_proc p
JOIN pg_namespace ns ON p.pronamespace = ns.oid
WHERE ns.nspname = 'public'
  AND p.prokind = 'f';

-- Step 3: Drop all tables
\echo ''
\echo '--- Step 3: Dropping all tables ---'

SELECT 'Tables to drop: ' || COUNT(*)::text AS info
FROM pg_tables
WHERE schemaname = 'public';

DO $$
DECLARE
    r RECORD;
    drop_count INTEGER := 0;
    drop_sql TEXT;
BEGIN
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public')
    LOOP
        drop_sql := 'DROP TABLE IF EXISTS public.' || quote_ident(r.tablename) || ' CASCADE';
        RAISE WARNING 'Executing: %', drop_sql;
        EXECUTE drop_sql;
        drop_count := drop_count + 1;
    END LOOP;
    RAISE WARNING 'Total tables dropped: %', drop_count;
END $$;

SELECT 'Tables remaining: ' || COUNT(*)::text AS info
FROM pg_tables
WHERE schemaname = 'public';

\echo ''
\echo '=== Completed 000-drop-all.sql ==='
