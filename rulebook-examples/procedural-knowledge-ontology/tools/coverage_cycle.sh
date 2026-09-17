#!/usr/bin/env bash
# One article-coverage cycle: compute, measure, recompute, report.
#
#   integrity check -> effortless build -> load -> measure_catalog -> extract answers
#   -> reconcile catalog -> effortless build -> load -> coverage report
#
# Two builds are unavoidable: coverage formulas read measurements that only exist once
# the first build has been loaded into Postgres. Every step fails loudly; a green build
# with a translation failure or a load error stops the cycle. See ARTICLE-COVERAGE.md.
#
# Usage: tools/coverage_cycle.sh [--measure-only]
# When piping the output (| tee), run under `set -o pipefail` or the exit code is lost.
set -euo pipefail
cd "$(dirname "$0")/.."
export PGDATABASE=erb_procedural_knowledge_ontology
LOG_DIR="${TMPDIR:-/tmp}/pko-coverage-cycle"
mkdir -p "$LOG_DIR"

build_and_load() {
  local tag=$1
  echo "==> [$tag] effortless build"
  effortless build >"$LOG_DIR/$tag-build.log" 2>&1 || {
    echo "BUILD FAILED ($LOG_DIR/$tag-build.log):"; tail -30 "$LOG_DIR/$tag-build.log"; exit 1; }
  if grep -q "Formula translation failed" postgres-bootstrap/02-create-functions.sql; then
    echo "TRANSLATION FAILURES — these columns would silently read NULL:"
    grep -B6 "Formula translation failed" postgres-bootstrap/02-create-functions.sql \
      | grep -oE "FUNCTION calc_[a-z0-9_]+" | sort -u | sed 's/FUNCTION /  /'
    exit 1
  fi
  echo "==> [$tag] load database"
  bash init-db.sh >"$LOG_DIR/$tag-init.log" 2>&1 || {
    echo "LOAD FAILED ($LOG_DIR/$tag-init.log):"; grep -E "ERROR" -A3 "$LOG_DIR/$tag-init.log" | head -30; exit 1; }
  if grep -qE "^psql:.*ERROR:" "$LOG_DIR/$tag-init.log"; then
    echo "LOAD ERRORS ($LOG_DIR/$tag-init.log):"; grep -E "^psql:.*ERROR:" -A3 "$LOG_DIR/$tag-init.log" | head -30; exit 1
  fi
  echo "==> [$tag] probe every view"
  psql -X -qtA -v ON_ERROR_STOP=1 <<'SQL'
DO $$ DECLARE v record; BEGIN
  FOR v IN SELECT table_name FROM information_schema.views WHERE table_schema='public' AND table_name LIKE 'vw\_%' LOOP
    EXECUTE format('SELECT count(*) FROM %I', v.table_name);
  END LOOP; END $$;
SQL
}

python3 tools/check_rulebook_integrity.py
if [ "${1:-}" != "--measure-only" ]; then
  build_and_load first
fi
echo "==> measure catalog"
python3 tools/measure_catalog.py
echo "==> extract witnessed answers"
python3 tools/extract_computed_answers.py --loop 5 >"$LOG_DIR/extract.log"
python3 tools/reconcile_field_catalog.py >/dev/null
build_and_load second

echo
echo "==> article coverage"
psql -X -P pager=off -c "SELECT source_article_id AS article, claim_count AS claims,
  covered_claim_count AS covered, coverage_percent AS pct, agreed_coverage_percent AS agreed_pct
  FROM vw_source_articles ORDER BY source_article_id"
psql -X -P pager=off -c "SELECT claim_kind, count(*) AS claims, count(*) FILTER (WHERE is_covered) AS covered,
  count(*) FILTER (WHERE has_rejected_evidence) AS rejected
  FROM vw_article_claims GROUP BY claim_kind ORDER BY claim_kind"
echo
echo "==> audit projection"
python3 tools/project_article_coverage.py
