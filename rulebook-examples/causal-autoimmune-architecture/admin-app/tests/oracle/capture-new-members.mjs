// capture-new-members.mjs — emit paste-ready oracle entries for the v2 Step-10
// cohort expansion (members H..L), CAPTURED LIVE from vw_* (the contract: the
// oracle is frozen from live views, never hand-written). Run with the app DB up:
//   node tests/oracle/capture-new-members.mjs
// Paste the printed blocks into dag-oracle.js.
import pg from 'pg';

const CONN = process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/causal_autoimmune';
const pool = new pg.Pool({ connectionString: CONN });
const q = async (sql, p = []) => (await pool.query(sql, p)).rows;

// new members: letter -> {ind, pred, mech, variant, template}
const NEW = [
  { key: 'H', ind: 'ind-h-yamamoto', pred: 'pred-h', mech: 'cm-h', variant: 'var-h-irf5', tmpl: 'a' },
  { key: 'I', ind: 'ind-i-conteh', pred: 'pred-i', mech: 'cm-i', variant: 'var-i-irf5', tmpl: 'd' },
  { key: 'J', ind: 'ind-j-brooks', pred: 'pred-j', mech: 'cm-j', variant: 'var-j-irf5', tmpl: 'b' },
  { key: 'K', ind: 'ind-k-nair', pred: 'pred-k', mech: 'cm-k', variant: 'var-k-il23r', tmpl: 'f' },
  { key: 'L', ind: 'ind-l-brandt', pred: 'pred-l', mech: 'cm-l', variant: 'var-l-il23r', tmpl: 'g' },
];

const j = (v) => JSON.stringify(v);
const obj = (row, fields) => '{ ' + fields.map((f) => `${f}: ${j(row[f])}`).join(', ') + ' }';

const PRED_FIELDS = ['individual_causal_mass', 'individual_confirmed_node_count', 'individual_cross_ancestry_node_count',
  'individual_has_cryptic_relatedness', 'predicted_value', 'count_bins', 'count_well_calibrated_bins',
  'well_calibrated_fraction', 'calibrated_uncertainty', 'rests_on_confirmed_mechanism',
  'has_spurious_correlation_flag', 'is_transportable_to_absent_ancestry', 'patient_stratification_tier'];
const GATE_FIELDS = ['is_high_confidence_prediction', 'is_falsifiability_backed', 'is_ancestry_transport_safe', 'predicted_value'];
const SEV_FIELDS = ['individual_max_severity_score', 'severity_tier', 'is_severity_actionable', 'severity_deciding_factor'];
const TREAT_FIELDS = ['is_treatment_response_actionable', 'treatment_response_deciding_factor'];
const IND_FIELDS = ['count_confirmed_causal_nodes', 'count_cross_ancestry_confirmed_nodes', 'has_cryptic_relatedness_flag', 'is_ancestry_absent_from_training'];
const MECH_FIELDS = ['count_qualified_evidence', 'count_modalities_supporting', 'count_intervention_targets', 'is_experimentally_falsifiable',
  'count_replications', 'count_concordant_replications', 'count_cross_ancestry_concordant', 'replicates_across_cohorts',
  'count_neg_control_tests', 'count_neg_control_survived', 'survives_negative_controls', 'is_spurious_derived',
  'causal_confidence', 'variant_is_causal_candidate', 'is_causal_architecture_node', 'is_ancestry_transportable'];
const VAR_FIELDS = ['allele_frequency', 'is_rare_variant', 'has_allele_specific_expression', 'is_causal_candidate'];
const DISEASE_FIELDS = ['nephritis_progression_state_key', 'latest_sledai_score', 'activity_tier', 'is_disease_progressing'];
const TL_FIELDS = ['recommended_treatment_line', 'treatment_line_deciding_factor', 'progression_vs_actionability_disagree'];

const pick = (row, fields) => Object.fromEntries(fields.map((f) => [f, row[f]]));

async function main() {
  const out = {};
  for (const m of NEW) {
    const pred = (await q('SELECT * FROM vw_individual_predictions WHERE individual_prediction_id=$1', [m.pred]))[0];
    const ind = (await q('SELECT * FROM vw_individuals WHERE individual_id=$1', [m.ind]))[0];
    const mech = (await q('SELECT * FROM vw_causal_mechanisms WHERE causal_mechanism_id=$1', [m.mech]))[0];
    const variant = (await q('SELECT * FROM vw_genomic_variants WHERE genomic_variant_id=$1', [m.variant]))[0];
    out[m.key] = { m, pred, ind, mech, variant };
  }

  console.log('\n// ===== PATIENTS rows (append to PATIENTS array) =====');
  for (const k of Object.keys(out)) {
    const { m, pred, ind } = out[k];
    console.log(`  { key: '${m.key}', individual: '${m.ind}', mechanism: '${m.mech}', prediction: '${m.pred}', variant: '${m.variant}', name: '${ind.given_name} ${ind.family_name}', ancestry: ${j(ind.ancestry_label)}, holdout: ${ind.is_ancestry_absent_from_training}, decidingGate: '${pred.deciding_gate}', actionable: ${pred.is_clinically_actionable} },`);
  }

  const block = (title, keyOf, fields, src) => {
    console.log(`\n// ===== ${title} =====`);
    for (const k of Object.keys(out)) {
      const row = out[k][src];
      console.log(`  '${keyOf(out[k].m)}': ${obj(pick(row, fields), fields)},`);
    }
  };
  block('KEYSTONE (is_clinically_actionable)', (m) => m.pred, ['is_clinically_actionable'], 'pred');
  block('GATES', (m) => m.pred, GATE_FIELDS, 'pred');
  block('PREDICTION_LEVEL', (m) => m.pred, PRED_FIELDS, 'pred');
  block('SEVERITY_LEVEL', (m) => m.pred, SEV_FIELDS, 'pred');
  block('TREATMENT_LEVEL', (m) => m.pred, TREAT_FIELDS, 'pred');
  block('TREATMENT_LINE', (m) => m.pred, TL_FIELDS, 'pred');
  block('INDIVIDUAL_LEVEL', (m) => m.ind, IND_FIELDS, 'ind');
  block('MECHANISM_LEVEL', (m) => m.mech, MECH_FIELDS, 'mech');
  block('VARIANT_LEVEL', (m) => m.variant, VAR_FIELDS, 'variant');
  block('DISEASE_STATE', (m) => m.ind, DISEASE_FIELDS, 'ind');

  // DECIDING_GATE (string)
  console.log('\n// ===== DECIDING_GATE =====');
  for (const k of Object.keys(out)) console.log(`  '${out[k].m.pred}': '${out[k].pred.deciding_gate}',`);

  // PROGRESSION_PATHS — needs the occupancy walk
  console.log('\n// ===== PROGRESSION_PATHS =====');
  for (const k of Object.keys(out)) {
    const m = out[k].m;
    const walk = await q(`SELECT state_key FROM vw_subject_state_instances WHERE state_machine='lupus-nephritis-progression' AND subject_id=$1 ORDER BY sequence_index`, [m.ind]);
    const states = walk.map((r) => r.state_key);
    console.log(`  '${m.ind}': { states: ${j(states)}, current: ${j(states[states.length - 1])} },`);
  }

  await pool.end();
}
main();
