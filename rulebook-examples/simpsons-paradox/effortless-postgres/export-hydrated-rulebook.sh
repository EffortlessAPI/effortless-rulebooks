#!/usr/bin/env bash
# ============================================================================
# export-hydrated-rulebook.sh
# ============================================================================
# Reads all computed views from the live simpsons_paradox DB and writes the
# computed field values back into the rulebook JSON SSoT in-place.
#
# The rulebook is always "complete" — raw inputs and computed outputs live
# together in the same document. This script re-witnesses the computed fields
# after any build so the rulebook reflects the actual DB state.
#
# Run automatically at the end of init-db.sh, or standalone:
#   ./effortless-postgres/export-hydrated-rulebook.sh
#   DATABASE_URL=postgresql://... ./effortless-postgres/export-hydrated-rulebook.sh
# ============================================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
DEFAULT_CONN="postgresql://postgres@localhost:5432/simpsons_paradox"
DATABASE_URL="${DATABASE_URL:-${1:-$DEFAULT_CONN}}"
RULEBOOK="$PROJECT_DIR/effortless-rulebook/simpsons-paradox-rulebook.json"

echo "[re-hydrate] source: $DATABASE_URL"
echo "[re-hydrate] rulebook: $RULEBOOK"

TMPDIR_RH="$(mktemp -d)"
trap 'rm -rf "$TMPDIR_RH"' EXIT

# ── fetch each view into a temp file (avoids single-quote issues in heredocs) -

psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.study_id) FROM (
  SELECT study_id, name, title, source, source_url, total_cases, cell_count
  FROM vw_studies) r;" > "$TMPDIR_RH/studies.json"

psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.treatment_id) FROM (
  SELECT treatment_id, name, study, treatment_label, description,
         total_cases, total_successes, pooled_success_rate
  FROM vw_treatments) r;" > "$TMPDIR_RH/treatments.json"

psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.stratum_id) FROM (
  SELECT stratum_id, name, study, stratum_label, description, total_cases
  FROM vw_strata) r;" > "$TMPDIR_RH/strata.json"

psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.case_cell_id) FROM (
  SELECT case_cell_id, name, study, stratum_label, treatment_label,
         successes, cases, cell_success_rate,
         total_cases_for_treatment, treatment_exposure_fraction
  FROM vw_case_cells) r;" > "$TMPDIR_RH/case_cells.json"

psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.stratum_summary_id) FROM (
  SELECT stratum_summary_id, name, study, stratum_label, treatment_label,
         stratum_successes, stratum_cases, stratum_success_rate,
         stratum_successes_a, stratum_cases_a, stratum_rate_a,
         stratum_successes_b, stratum_cases_b, stratum_rate_b,
         stratum_winner, study_total_cases, stratum_total_cases,
         stratum_fraction, weighted_stratum_rate,
         treatment_a_cases_here, treatment_b_cases_here,
         treatment_a_total_cases, treatment_b_total_cases,
         allocation_fraction_a, allocation_fraction_b, allocation_bias,
         stratum_gap, weighted_stratum_gap
  FROM vw_stratum_summaries) r;" > "$TMPDIR_RH/stratum_summaries.json"

psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.stratum_variable_id) FROM (
  SELECT stratum_variable_id, name, study, variable_name, causal_role,
         affects_treatment_assignment, affects_outcome, is_confounder,
         mechanism_note
  FROM vw_stratum_variables) r;" > "$TMPDIR_RH/stratum_variables.json"

psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r) ORDER BY r.treatment_ranking_id) FROM (
  SELECT treatment_ranking_id, name, study, treatment_a, treatment_b,
         total_cases_a, total_successes_a, pooled_rate_a,
         total_cases_b, total_successes_b, pooled_rate_b,
         pooled_winner, stratum_count,
         strata_won_by_a, strata_won_by_b, per_stratum_winner,
         is_reversal, confounders_in_study, is_paradox_explained,
         is_paradox_explained_v2, pooled_gap, strata_won_by_loser,
         paradox_strength, pooled_rate_from_weights_a, pooled_rate_from_weights_b,
         reversal_intensity, threshold_margin, signed_pooled_gap,
         weighted_stratum_gap_sum, is_sign_flip, allocation_distortion,
         distortion_type, policy_implication,
         is_reversal_v2, definition_delta, strict_reversal_subtype
  FROM vw_treatment_rankings) r;" > "$TMPDIR_RH/treatment_rankings.json"

psql "$DATABASE_URL" -At -c "
SELECT json_agg(row_to_json(r)) FROM (
  SELECT model_summary_id, name,
         reversal_count, non_reversal_count, study_count,
         explained_count, zero_strength_count, partial_count,
         total_paradox_strength, avg_paradox_strength,
         type_a_count, type_b_count, type_c_count, type_d_count,
         type_a_fraction, distortion_taxonomy_coverage,
         reversal_count_v2, non_reversal_count_v2, extended_reversal_count,
         explained_count_v2, distortion_only_count
  FROM vw_model_summary) r;" > "$TMPDIR_RH/model_summary.json"

# ── merge computed values back into the rulebook ──────────────────────────────

python3 - "$TMPDIR_RH" "$RULEBOOK" <<'PYEOF'
import json, re, sys

tmpdir, rulebook_path = sys.argv[1], sys.argv[2]

def load(name):
    return json.load(open(f"{tmpdir}/{name}.json"))

COL = {
    'study_id': 'StudyId', 'name': 'Name', 'title': 'Title', 'source': 'Source',
    'source_url': 'SourceUrl', 'total_cases': 'TotalCases', 'cell_count': 'CellCount',
    'treatment_id': 'TreatmentId', 'study': 'Study', 'treatment_label': 'TreatmentLabel',
    'description': 'Description', 'total_successes': 'TotalSuccesses',
    'pooled_success_rate': 'PooledSuccessRate',
    'stratum_id': 'StratumId', 'stratum_label': 'StratumLabel',
    'case_cell_id': 'CaseCellId', 'successes': 'Successes', 'cases': 'Cases',
    'cell_success_rate': 'CellSuccessRate',
    'total_cases_for_treatment': 'TotalCasesForTreatment',
    'treatment_exposure_fraction': 'TreatmentExposureFraction',
    'stratum_summary_id': 'StratumSummaryId',
    'stratum_successes': 'StratumSuccesses', 'stratum_cases': 'StratumCases',
    'stratum_success_rate': 'StratumSuccessRate',
    'stratum_successes_a': 'StratumSuccessesA', 'stratum_cases_a': 'StratumCasesA',
    'stratum_rate_a': 'StratumRateA',
    'stratum_successes_b': 'StratumSuccessesB', 'stratum_cases_b': 'StratumCasesB',
    'stratum_rate_b': 'StratumRateB', 'stratum_winner': 'StratumWinner',
    'study_total_cases': 'StudyTotalCases', 'stratum_total_cases': 'StratumTotalCases',
    'stratum_fraction': 'StratumFraction', 'weighted_stratum_rate': 'WeightedStratumRate',
    'treatment_a_cases_here': 'TreatmentACasesHere',
    'treatment_b_cases_here': 'TreatmentBCasesHere',
    'treatment_a_total_cases': 'TreatmentATotalCases',
    'treatment_b_total_cases': 'TreatmentBTotalCases',
    'allocation_fraction_a': 'AllocationFractionA',
    'allocation_fraction_b': 'AllocationFractionB',
    'allocation_bias': 'AllocationBias',
    'stratum_gap': 'StratumGap', 'weighted_stratum_gap': 'WeightedStratumGap',
    'stratum_variable_id': 'StratumVariableId', 'variable_name': 'VariableName',
    'causal_role': 'CausalRole',
    'affects_treatment_assignment': 'AffectsTreatmentAssignment',
    'affects_outcome': 'AffectsOutcome', 'is_confounder': 'IsConfounder',
    'mechanism_note': 'MechanismNote',
    'treatment_ranking_id': 'TreatmentRankingId',
    'treatment_a': 'TreatmentA', 'treatment_b': 'TreatmentB',
    'total_cases_a': 'TotalCasesA', 'total_successes_a': 'TotalSuccessesA',
    'pooled_rate_a': 'PooledRateA',
    'total_cases_b': 'TotalCasesB', 'total_successes_b': 'TotalSuccessesB',
    'pooled_rate_b': 'PooledRateB', 'pooled_winner': 'PooledWinner',
    'stratum_count': 'StratumCount',
    'strata_won_by_a': 'StrataWonByA', 'strata_won_by_b': 'StrataWonByB',
    'per_stratum_winner': 'PerStratumWinner',
    'is_reversal': 'IsReversal', 'confounders_in_study': 'ConfoundersInStudy',
    'is_paradox_explained': 'IsParadoxExplained',
    'is_paradox_explained_v2': 'IsParadoxExplained_v2',
    'pooled_gap': 'PooledGap', 'strata_won_by_loser': 'StrataWonByLoser',
    'paradox_strength': 'ParadoxStrength',
    'pooled_rate_from_weights_a': 'PooledRateFromWeightsA',
    'pooled_rate_from_weights_b': 'PooledRateFromWeightsB',
    'reversal_intensity': 'ReversalIntensity', 'threshold_margin': 'ThresholdMargin',
    'signed_pooled_gap': 'SignedPooledGap',
    'weighted_stratum_gap_sum': 'WeightedStratumGapSum',
    'is_sign_flip': 'IsSignFlip', 'allocation_distortion': 'AllocationDistortion',
    'distortion_type': 'DistortionType', 'policy_implication': 'PolicyImplication',
    'is_reversal_v2': 'IsReversal_v2', 'definition_delta': 'DefinitionDelta',
    'strict_reversal_subtype': 'StrictReversalSubtype',
    'model_summary_id': 'ModelSummaryId',
    'reversal_count': 'ReversalCount', 'non_reversal_count': 'NonReversalCount',
    'study_count': 'StudyCount', 'explained_count': 'ExplainedCount',
    'zero_strength_count': 'ZeroStrengthCount', 'partial_count': 'PartialCount',
    'total_paradox_strength': 'TotalParadoxStrength',
    'avg_paradox_strength': 'AvgParadoxStrength',
    'type_a_count': 'TypeACount', 'type_b_count': 'TypeBCount',
    'type_c_count': 'TypeCCount', 'type_d_count': 'TypeDCount',
    'type_a_fraction': 'TypeAFraction',
    'distortion_taxonomy_coverage': 'DistortionTaxonomyCoverage',
    'reversal_count_v2': 'ReversalCountV2',
    'non_reversal_count_v2': 'NonReversalCountV2',
    'extended_reversal_count': 'ExtendedReversalCount',
    'explained_count_v2': 'ExplainedCountV2',
    'distortion_only_count': 'DistortionOnlyCount',
}

def rename(row):
    return {COL.get(k, k): v for k, v in row.items()}

TABLE_UPDATES = [
    ('Studies',           'StudyId',            [rename(r) for r in load('studies')]),
    ('Treatments',        'TreatmentId',        [rename(r) for r in load('treatments')]),
    ('Strata',            'StratumId',          [rename(r) for r in load('strata')]),
    ('CaseCells',         'CaseCellId',         [rename(r) for r in load('case_cells')]),
    ('StratumSummaries',  'StratumSummaryId',   [rename(r) for r in load('stratum_summaries')]),
    ('StratumVariables',  'StratumVariableId',  [rename(r) for r in load('stratum_variables')]),
    ('TreatmentRankings', 'TreatmentRankingId', [rename(r) for r in load('treatment_rankings')]),
    ('ModelSummary',      'ModelSummaryId',     [rename(r) for r in load('model_summary')]),
]

rb = json.load(open(rulebook_path))

for rb_table, pk, db_rows in TABLE_UPDATES:
    db_by_pk = {r[pk]: r for r in db_rows}
    updated = 0
    for row in rb[rb_table]['data']:
        key = row[pk]
        if key in db_by_pk:
            row.update(db_by_pk[key])
            updated += 1
    print(f"  {rb_table}: {updated} rows re-witnessed")

# write back with compact data rows (one object per line per CLAUDE.md)
raw = json.dumps(rb, indent=2, ensure_ascii=False)
lines = []
in_data = False
brace_depth = 0
row_buf = []
src = raw.splitlines()
i = 0
while i < len(src):
    line = src[i]
    stripped = line.lstrip()
    if not in_data:
        lines.append(line)
        if re.search(r'"data"\s*:\s*\[', line):
            in_data = True
            brace_depth = 0
    else:
        if stripped.startswith('{'):
            brace_depth = stripped.count('{') - stripped.count('}')
            row_buf = [stripped]
            if brace_depth == 0:
                indent = ' ' * (len(line) - len(stripped))
                lines.append(indent + row_buf[0])
                row_buf = []
        elif brace_depth > 0:
            brace_depth += stripped.count('{') - stripped.count('}')
            row_buf.append(stripped)
            if brace_depth == 0:
                back = i - len(row_buf) + 1
                first = src[back] if back >= 0 else src[i]
                indent = ' ' * (len(first) - len(first.lstrip()))
                compact = re.sub(r'\s+', ' ', ' '.join(row_buf))
                lines.append(indent + compact)
                row_buf = []
        elif stripped.startswith(']'):
            in_data = False
            lines.append(line)
        else:
            lines.append(line)
    i += 1

open(rulebook_path, 'w').write('\n'.join(lines) + '\n')
PYEOF

echo "[re-hydrate] done"
