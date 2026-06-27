#!/usr/bin/env bash
# ============================================================================
# export-hydrated-rulebook.sh
# ============================================================================
# Reads all computed views from the live simpsons_paradox DB and writes
# effortless-rulebook/simpsons-paradox-hydrated.json — a version of the
# rulebook where every calculated/aggregation field is populated with its
# actual DB-computed value.
#
# Run automatically at the end of init-db.sh, or standalone:
#   ./effortless-postgres/export-hydrated-rulebook.sh
#   DATABASE_URL=postgresql://... ./effortless-postgres/export-hydrated-rulebook.sh
#
# Output is checked into git so the computed state is reproducible and
# queryable by anyone without a running Postgres instance.
# ============================================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
DEFAULT_CONN="postgresql://postgres@localhost:5432/simpsons_paradox"
DATABASE_URL="${DATABASE_URL:-${1:-$DEFAULT_CONN}}"
OUT="$PROJECT_DIR/effortless-rulebook/simpsons-paradox-hydrated.json"

echo "[export-hydrated] source: $DATABASE_URL"
echo "[export-hydrated] output: $OUT"

# Helper: run a psql query and return JSON
q() { psql "$DATABASE_URL" -At -c "$1"; }

# ── Studies ──────────────────────────────────────────────────────────────────
STUDIES=$(psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.\"StudyId\")
FROM (
  SELECT
    study_id        AS \"StudyId\",
    name            AS \"Name\",
    title           AS \"Title\",
    source          AS \"Source\",
    source_url      AS \"SourceUrl\",
    total_cases     AS \"TotalCases\",
    cell_count      AS \"CellCount\"
  FROM vw_studies
) r;
")

# ── Treatments ───────────────────────────────────────────────────────────────
TREATMENTS=$(psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.\"TreatmentId\")
FROM (
  SELECT
    treatment_id        AS \"TreatmentId\",
    name                AS \"Name\",
    study               AS \"Study\",
    treatment_label     AS \"TreatmentLabel\",
    description         AS \"Description\",
    total_cases         AS \"TotalCases\",
    total_successes     AS \"TotalSuccesses\",
    pooled_success_rate AS \"PooledSuccessRate\"
  FROM vw_treatments
) r;
")

# ── Strata ───────────────────────────────────────────────────────────────────
STRATA=$(psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.\"StratumId\")
FROM (
  SELECT
    stratum_id    AS \"StratumId\",
    name          AS \"Name\",
    study         AS \"Study\",
    stratum_label AS \"StratumLabel\",
    description   AS \"Description\",
    total_cases   AS \"TotalCases\"
  FROM vw_strata
) r;
")

# ── CaseCells ────────────────────────────────────────────────────────────────
CASE_CELLS=$(psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.\"CaseCellId\")
FROM (
  SELECT
    case_cell_id               AS \"CaseCellId\",
    name                       AS \"Name\",
    study                      AS \"Study\",
    stratum_label              AS \"StratumLabel\",
    treatment_label            AS \"TreatmentLabel\",
    successes                  AS \"Successes\",
    cases                      AS \"Cases\",
    cell_success_rate          AS \"CellSuccessRate\",
    total_cases_for_treatment  AS \"TotalCasesForTreatment\",
    treatment_exposure_fraction AS \"TreatmentExposureFraction\"
  FROM vw_case_cells
) r;
")

# ── StratumSummaries ─────────────────────────────────────────────────────────
STRATUM_SUMMARIES=$(psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.\"StratumSummaryId\")
FROM (
  SELECT
    stratum_summary_id     AS \"StratumSummaryId\",
    name                   AS \"Name\",
    study                  AS \"Study\",
    stratum_label          AS \"StratumLabel\",
    treatment_label        AS \"TreatmentLabel\",
    stratum_successes      AS \"StratumSuccesses\",
    stratum_cases          AS \"StratumCases\",
    stratum_success_rate   AS \"StratumSuccessRate\",
    stratum_successes_a    AS \"StratumSuccessesA\",
    stratum_cases_a        AS \"StratumCasesA\",
    stratum_rate_a         AS \"StratumRateA\",
    stratum_successes_b    AS \"StratumSuccessesB\",
    stratum_cases_b        AS \"StratumCasesB\",
    stratum_rate_b         AS \"StratumRateB\",
    stratum_winner         AS \"StratumWinner\",
    study_total_cases      AS \"StudyTotalCases\",
    stratum_total_cases    AS \"StratumTotalCases\",
    stratum_fraction       AS \"StratumFraction\",
    weighted_stratum_rate  AS \"WeightedStratumRate\",
    treatment_a_cases_here AS \"TreatmentACasesHere\",
    treatment_b_cases_here AS \"TreatmentBCasesHere\",
    treatment_a_total_cases AS \"TreatmentATotalCases\",
    treatment_b_total_cases AS \"TreatmentBTotalCases\",
    allocation_fraction_a  AS \"AllocationFractionA\",
    allocation_fraction_b  AS \"AllocationFractionB\",
    allocation_bias        AS \"AllocationBias\",
    stratum_gap            AS \"StratumGap\",
    weighted_stratum_gap   AS \"WeightedStratumGap\"
  FROM vw_stratum_summaries
) r;
")

# ── StratumVariables ─────────────────────────────────────────────────────────
STRATUM_VARIABLES=$(psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.\"StratumVariableId\")
FROM (
  SELECT
    stratum_variable_id           AS \"StratumVariableId\",
    name                          AS \"Name\",
    study                         AS \"Study\",
    variable_name                 AS \"VariableName\",
    causal_role                   AS \"CausalRole\",
    affects_treatment_assignment  AS \"AffectsTreatmentAssignment\",
    affects_outcome               AS \"AffectsOutcome\",
    is_confounder                 AS \"IsConfounder\",
    mechanism_note                AS \"MechanismNote\"
  FROM vw_stratum_variables
) r;
")

# ── TreatmentRankings ────────────────────────────────────────────────────────
TREATMENT_RANKINGS=$(psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.\"AllocationDistortion\" DESC)
FROM (
  SELECT
    treatment_ranking_id        AS \"TreatmentRankingId\",
    name                        AS \"Name\",
    study                       AS \"Study\",
    treatment_a                 AS \"TreatmentA\",
    treatment_b                 AS \"TreatmentB\",
    total_cases_a               AS \"TotalCasesA\",
    total_successes_a           AS \"TotalSuccessesA\",
    pooled_rate_a               AS \"PooledRateA\",
    total_cases_b               AS \"TotalCasesB\",
    total_successes_b           AS \"TotalSuccessesB\",
    pooled_rate_b               AS \"PooledRateB\",
    pooled_winner               AS \"PooledWinner\",
    stratum_count               AS \"StratumCount\",
    strata_won_by_a             AS \"StrataWonByA\",
    strata_won_by_b             AS \"StrataWonByB\",
    per_stratum_winner          AS \"PerStratumWinner\",
    is_reversal                 AS \"IsReversal\",
    confounders_in_study        AS \"ConfoundersInStudy\",
    is_paradox_explained        AS \"IsParadoxExplained\",
    pooled_gap                  AS \"PooledGap\",
    strata_won_by_loser         AS \"StrataWonByLoser\",
    paradox_strength            AS \"ParadoxStrength\",
    pooled_rate_from_weights_a  AS \"PooledRateFromWeightsA\",
    pooled_rate_from_weights_b  AS \"PooledRateFromWeightsB\",
    reversal_intensity          AS \"ReversalIntensity\",
    threshold_margin            AS \"ThresholdMargin\",
    signed_pooled_gap           AS \"SignedPooledGap\",
    weighted_stratum_gap_sum    AS \"WeightedStratumGapSum\",
    is_sign_flip                AS \"IsSignFlip\",
    allocation_distortion       AS \"AllocationDistortion\",
    distortion_type             AS \"DistortionType\",
    policy_implication          AS \"PolicyImplication\"
  FROM vw_treatment_rankings
) r;
")

# ── ModelSummary ─────────────────────────────────────────────────────────────
MODEL_SUMMARY=$(psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r))
FROM (
  SELECT
    model_summary_id       AS \"ModelSummaryId\",
    name                   AS \"Name\",
    reversal_count         AS \"ReversalCount\",
    non_reversal_count     AS \"NonReversalCount\",
    study_count            AS \"StudyCount\",
    explained_count        AS \"ExplainedCount\",
    zero_strength_count    AS \"ZeroStrengthCount\",
    partial_count          AS \"PartialCount\",
    total_paradox_strength AS \"TotalParadoxStrength\",
    avg_paradox_strength   AS \"AvgParadoxStrength\"
  FROM vw_model_summary
) r;
")

# ── Loops (raw — no computed fields) ─────────────────────────────────────────
LOOPS=$(psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.\"LoopId\")
FROM (
  SELECT
    loop_id         AS \"LoopId\",
    name            AS \"Name\",
    title           AS \"Title\",
    status          AS \"Status\",
    new_concept     AS \"NewConcept\",
    domain_question AS \"DomainQuestion\",
    mock_data_note  AS \"MockDataNote\",
    next_suggestion AS \"NextSuggestion\"
  FROM vw_loops
) r;
")

# ── Assemble ─────────────────────────────────────────────────────────────────
EXPORTED_AT="$(date -u +%Y-%m-%dT%H:%M:%SZ)"

python3 - <<PYEOF
import json, sys

studies            = json.loads('''$STUDIES''')
treatments         = json.loads('''$TREATMENTS''')
strata             = json.loads('''$STRATA''')
case_cells         = json.loads('''$CASE_CELLS''')
stratum_summaries  = json.loads('''$STRATUM_SUMMARIES''')
stratum_variables  = json.loads('''$STRATUM_VARIABLES''')
treatment_rankings = json.loads('''$TREATMENT_RANKINGS''')
model_summary      = json.loads('''$MODEL_SUMMARY''')
loops              = json.loads('''$LOOPS''')

hydrated = {
    "\$schema": "https://example.com/cmcc-schema/v1",
    "Name": "simpsons-paradox",
    "_hydrated": {
        "exported_at": "$EXPORTED_AT",
        "source_db": "$DATABASE_URL",
        "note": "All calculated and aggregation fields are DB-computed values from vw_* views. Raw fields match the rulebook SSoT. Real study data lives in effortless-postgres/05b-customize-data.sql."
    },
    "Loops":             {"data": loops},
    "Studies":           {"data": studies},
    "Treatments":        {"data": treatments},
    "Strata":            {"data": strata},
    "CaseCells":         {"data": case_cells},
    "StratumSummaries":  {"data": stratum_summaries},
    "StratumVariables":  {"data": stratum_variables},
    "TreatmentRankings": {"data": treatment_rankings},
    "ModelSummary":      {"data": model_summary},
}

with open("$OUT", "w") as f:
    json.dump(hydrated, f, indent=2, default=str)

print(f"[export-hydrated] wrote {len(json.dumps(hydrated))} bytes")
print(f"  Studies:           {len(studies)}")
print(f"  TreatmentRankings: {len(treatment_rankings)}")
print(f"  StratumSummaries:  {len(stratum_summaries)}")
PYEOF

echo "[export-hydrated] done → $OUT"
