#!/usr/bin/env bash
# Rebuild the substrate from the rulebook and report every witness's distribution.
#
# The acceptance bar for a witness is NON-VACUOUS: it must be able to fire.
# A boolean that is all-true or all-false over the seed data states nothing
# about the procedures, so this script flags those explicitly rather than
# letting a green build imply the model says something it does not.
#
# Usage: tools/verify_witnesses.sh [--no-build]
set -euo pipefail

cd "$(dirname "$0")/.."
export ERB_DOMAIN=procedural-knowledge-ontology
export PGDATABASE="erb_${ERB_DOMAIN//-/_}"

if [ "${1:-}" != "--no-build" ]; then
  echo "==> effortless build"
  (cd postgres-bootstrap && effortless build) >/tmp/erb-build.log 2>&1 || {
    echo "BUILD FAILED — last 20 lines:"; tail -20 /tmp/erb-build.log; exit 1; }
  grep -oE "Generated [0-9]+ calculation/lookup functions" /tmp/erb-build.log | tail -1
fi

# Load from scratch. Re-running against a populated DB emits a wall of
# "already exists" NOTICEs that make real errors easy to miss.
echo "==> reload (dropdb + createdb)"
dropdb --if-exists "$PGDATABASE" >/dev/null 2>&1 || true
createdb "$PGDATABASE"
./postgres-bootstrap/init-db.sh >/tmp/erb-init.log 2>&1 || true

# Count only true psql errors, not NOTICEs.
errs=$(grep -cE "^psql:.*: ERROR:" /tmp/erb-init.log || true)
if [ "$errs" -gt 0 ]; then
  echo "LOAD FAILED — $errs error(s):"
  grep -E "^psql:.*: ERROR:" -A3 /tmp/erb-init.log | head -30
  exit 1
fi
echo "load clean"

echo
echo "==> witness distributions (vacuous = cannot discriminate)"
psql -qtA -v ON_ERROR_STOP=1 <<'SQL'
DO $$
DECLARE
  r RECORD; t INT; f INT; n INT; total INT; verdict TEXT; vac INT := 0; ok INT := 0;
BEGIN
  FOR r IN
    SELECT c.table_name, c.column_name
    FROM information_schema.columns c
    WHERE c.table_schema='public' AND c.table_name LIKE 'vw\_%'
      AND c.data_type='boolean'
    ORDER BY c.table_name, c.column_name
  LOOP
    EXECUTE format(
      'SELECT count(*) FILTER (WHERE %I), count(*) FILTER (WHERE NOT %I),
              count(*) FILTER (WHERE %I IS NULL), count(*) FROM %I',
      r.column_name, r.column_name, r.column_name, r.table_name)
      INTO t, f, n, total;
    IF total = 0 THEN verdict := 'EMPTY';
    ELSIF t = 0 THEN verdict := 'VACUOUS(never true)';
    ELSIF f = 0 AND n = 0 THEN verdict := 'VACUOUS(always true)';
    ELSE verdict := 'ok';
    END IF;
    IF verdict = 'ok' THEN ok := ok + 1; ELSE vac := vac + 1; END IF;
    RAISE NOTICE '% %-34s t=% f=% null=% [%]',
      rpad(replace(r.table_name,'vw_',''), 26), r.column_name, t, f, n, verdict;
  END LOOP;
  RAISE NOTICE '--- % discriminating, % vacuous/empty ---', ok, vac;
END $$;
SQL

echo
echo "==> field catalog"
python3 tools/reconcile_field_catalog.py --check || {
  echo "catalog drifted — run: python3 tools/reconcile_field_catalog.py"; exit 1; }
psql -qtA -c "SELECT '  fields=' || count(*) || '  derived=' || count(*) FILTER (WHERE is_derived)
              || '  witnessed=' || count(*) FILTER (WHERE is_witness) FROM vw_rulebook_fields"
