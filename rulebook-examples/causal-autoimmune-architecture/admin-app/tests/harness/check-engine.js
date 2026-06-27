// ============================================================================
//  check-engine.js — ONE source of truth for the witnessed harness.
// ============================================================================
//  Given a base URL of a running app, this evaluates EVERY oracle node against
//  the app's API and returns a flat, structured list of checks:
//
//    { category, level, label, status, witness, endpoint, field, expected, actual, detail }
//
//  status ∈ 'pass' | 'fail' | 'not_surfaced'
//    - pass         : app served the oracle value
//    - not_surfaced : app returned 501 (the node isn't wired yet — RED by design)
//    - fail         : app served a WRONG value (a real regression)
//
//  Consumed by BOTH:
//    - tests/harness/*.test.js  (node --test asserts status==='pass')
//    - GET /api/harness         (the browser renders the category→test tree)
//
//  This is what makes the harness the home screen AND the CI gate without
//  duplicating the oracle logic.
// ============================================================================

import {
  PATIENTS,
  KEYSTONE,
  GATES,
  PREDICTION_LEVEL,
  PREDICTION_WITNESS,
  INDIVIDUAL_LEVEL,
  MECHANISM_LEVEL,
  MECHANISM_WITNESS,
  EVIDENCE_ATOMS,
  REPLICATION_ATOMS,
  CONTROL_ATOMS,
  CALIBRATION_ATOMS,
  VARIANT_LEVEL,
  VARIANT_WITNESS,
  LIFECYCLE_PATHS,
  ROUTING,
  PROGRESSION_INDIVIDUALS,
  DISEASE_STATE,
  DISEASE_STATE_WITNESS,
  PROGRESSION_PATHS,
  TREATMENT_LINE,
  TREATMENT_LINE_WITNESS,
  TOL,
} from '../oracle/dag-oracle.js';
import { CONTRACT } from './contract.js';

const pByMech = (m) => PATIENTS.find((x) => x.mechanism === m);
const pByPred = (p) => PATIENTS.find((x) => x.prediction === p);

async function fetchJson(base, path) {
  const res = await fetch(`${base}${path}`);
  let body = null;
  try {
    body = await res.json();
  } catch {
    body = null;
  }
  return { status: res.status, body };
}

function numEq(a, b) {
  return Math.abs(Number(a) - Number(b)) <= TOL;
}

// Compare one served field to an expected value -> a check status.
function judgeScalar({ status, body }, field, expected) {
  if (status === 501) return { status: 'not_surfaced', actual: null, detail: 'endpoint returns 501 (not wired yet)' };
  if (status !== 200) return { status: 'fail', actual: null, detail: `HTTP ${status}` };
  if (!body || !Object.prototype.hasOwnProperty.call(body, field))
    return { status: 'fail', actual: undefined, detail: `200 but missing field "${field}"` };
  const actual = body[field];
  const ok =
    typeof expected === 'number' && !Number.isInteger(expected) ? numEq(actual, expected) : actual === expected;
  return { status: ok ? 'pass' : 'fail', actual, detail: ok ? '' : `expected ${JSON.stringify(expected)}, got ${JSON.stringify(actual)}` };
}

// Compare a derived count over a served LIST -> a check status.
function judgeListCount({ status, body }, predicate, expected) {
  if (status === 501) return { status: 'not_surfaced', actual: null, detail: 'endpoint returns 501 (not wired yet)' };
  if (status !== 200) return { status: 'fail', actual: null, detail: `HTTP ${status}` };
  const rows = Array.isArray(body) ? body : body?.rows;
  if (!Array.isArray(rows)) return { status: 'fail', actual: null, detail: '200 but body is not a list' };
  const actual = rows.filter(predicate).length;
  return { status: actual === expected ? 'pass' : 'fail', actual, detail: actual === expected ? '' : `expected ${expected}, got ${actual}` };
}

function mk(category, level, label, witness, endpoint, field, expected, judged) {
  return { category, level, label, witness, endpoint, field, expected, ...judged };
}

// ---------------------------------------------------------------------------
//  Run the full suite against a running app. Returns { checks, summary }.
// ---------------------------------------------------------------------------
export async function runHarness(base) {
  const checks = [];

  // ---- L7 KEYSTONE -------------------------------------------------------
  for (const p of PATIENTS) {
    const o = KEYSTONE[p.prediction];
    const ep = CONTRACT.keystone.endpoint(p);
    const r = await fetchJson(base, ep);
    checks.push(
      mk(
        'L7 · Keystone — IsClinicallyActionable',
        7,
        `${p.key} · ${p.name} (${p.ancestry}${p.holdout ? ', holdout' : ''}) ⇒ ${o.expected.is_clinically_actionable ? 'TRUE' : 'FALSE'} [${p.decidingGate}]`,
        o.witness,
        ep,
        'is_clinically_actionable',
        o.expected.is_clinically_actionable,
        judgeScalar(r, 'is_clinically_actionable', o.expected.is_clinically_actionable),
      ),
    );
  }

  // ---- L7 COHORT (set) ---------------------------------------------------
  {
    const ep = CONTRACT.cohort.endpoint();
    const r = await fetchJson(base, ep);
    let judged;
    if (r.status === 501) judged = { status: 'not_surfaced', actual: null, detail: 'endpoint returns 501 (not wired yet)' };
    else if (r.status !== 200) judged = { status: 'fail', actual: null, detail: `HTTP ${r.status}` };
    else {
      const rows = Array.isArray(r.body) ? r.body : r.body?.rows || [];
      const act = rows.filter((x) => x.is_clinically_actionable).map((x) => x.individual_prediction_id).sort();
      const exp = PATIENTS.filter((p) => p.actionable).map((p) => p.prediction).sort();
      const ok = rows.length === PATIENTS.length && JSON.stringify(act) === JSON.stringify(exp);
      judged = { status: ok ? 'pass' : 'fail', actual: act, detail: ok ? '' : `expected actionable set ${JSON.stringify(exp)} over ${PATIENTS.length} rows, got ${JSON.stringify(act)} over ${rows.length}` };
    }
    checks.push(
      mk('L7 · Cohort — keystone list', 7, 'Cohort lists all 7 with exactly {A,G} actionable', 'Every gate is exercised by both a passing and a failing patient; the whole oracle holds as a set.', ep, 'actionable-set', ['pred-a', 'pred-g'], judged),
    );
  }

  // ---- L6 GATES ----------------------------------------------------------
  const GATE_FIELDS = ['is_high_confidence_prediction', 'is_falsifiability_backed', 'is_ancestry_transport_safe'];
  for (const p of PATIENTS) {
    const g = GATES[p.prediction];
    const ep = CONTRACT.gates.endpoint(p);
    const r = await fetchJson(base, ep);
    for (const f of GATE_FIELDS) {
      checks.push(mk('L6 · Gates — the four keystone gates', 6, `${p.key} · ${p.name} · ${f} = ${g[f]}`, KEYSTONE[p.prediction].witness, ep, f, g[f], judgeScalar(r, f, g[f])));
    }
    // 4th gate: predicted_value sign
    let pvJudged;
    if (r.status === 501) pvJudged = { status: 'not_surfaced', actual: null, detail: 'endpoint returns 501 (not wired yet)' };
    else if (r.status !== 200) pvJudged = { status: 'fail', actual: null, detail: `HTTP ${r.status}` };
    else {
      const positive = Number(r.body?.predicted_value) > 0;
      pvJudged = { status: positive === g.predicted_value_positive ? 'pass' : 'fail', actual: r.body?.predicted_value, detail: positive === g.predicted_value_positive ? '' : `expected predicted_value ${g.predicted_value_positive ? '>0' : '=0'}` };
    }
    checks.push(mk('L6 · Gates — the four keystone gates', 6, `${p.key} · ${p.name} · predicted_value ${g.predicted_value_positive ? '> 0' : '= 0'}`, KEYSTONE[p.prediction].witness, ep, 'predicted_value', g.predicted_value_positive, pvJudged));
  }

  // ---- L5 PREDICTION SCALARS --------------------------------------------
  const PRED_FIELDS = [
    'individual_causal_mass', 'individual_confirmed_node_count', 'individual_cross_ancestry_node_count',
    'individual_has_cryptic_relatedness', 'predicted_value', 'count_bins', 'count_well_calibrated_bins',
    'well_calibrated_fraction', 'calibrated_uncertainty', 'rests_on_confirmed_mechanism',
    'has_spurious_correlation_flag', 'is_transportable_to_absent_ancestry', 'patient_stratification_tier',
  ];
  for (const p of PATIENTS) {
    const row = PREDICTION_LEVEL[p.prediction];
    const ep = CONTRACT.predictionLevel.endpoint(p);
    const r = await fetchJson(base, ep);
    for (const f of PRED_FIELDS) {
      checks.push(mk('L5 · Prediction scalars', 5, `${p.key} · ${p.name} · ${f} = ${row[f]}`, PREDICTION_WITNESS[f] || `derived scalar ${f}`, ep, f, row[f], judgeScalar(r, f, row[f])));
    }
  }

  // ---- L4 INDIVIDUAL -----------------------------------------------------
  const IND_FIELDS = ['count_confirmed_causal_nodes', 'count_cross_ancestry_confirmed_nodes', 'has_cryptic_relatedness_flag', 'is_ancestry_absent_from_training'];
  const IND_WITNESS = {
    count_confirmed_causal_nodes: 'COUNT of this individual\'s mechanisms where IsCausalArchitectureNode.',
    count_cross_ancestry_confirmed_nodes: 'COUNT of confirmed nodes that ALSO replicated in >=1 different ancestry.',
    has_cryptic_relatedness_flag: 'OBSERVATION: cryptic-relatedness leakage flag.',
    is_ancestry_absent_from_training: 'OBSERVATION: this ancestry is held out of training.',
  };
  for (const p of PATIENTS) {
    const row = INDIVIDUAL_LEVEL[p.individual];
    const ep = CONTRACT.individualLevel.endpoint(p);
    const r = await fetchJson(base, ep);
    for (const f of IND_FIELDS) {
      checks.push(mk('L4 · Individual rollups', 4, `${p.key} · ${p.name} · ${f} = ${row[f]}`, IND_WITNESS[f], ep, f, row[f], judgeScalar(r, f, row[f])));
    }
  }

  // ---- L3/L2 MECHANISM ---------------------------------------------------
  const MECH_FIELDS = [
    'count_qualified_evidence', 'count_modalities_supporting', 'count_intervention_targets', 'is_experimentally_falsifiable',
    'count_replications', 'count_concordant_replications', 'count_cross_ancestry_concordant', 'replicates_across_cohorts',
    'count_neg_control_tests', 'count_neg_control_survived', 'survives_negative_controls', 'is_spurious_derived',
    'causal_confidence', 'variant_is_causal_candidate', 'is_causal_architecture_node', 'is_ancestry_transportable',
  ];
  for (const p of PATIENTS) {
    const row = MECHANISM_LEVEL[p.mechanism];
    const ep = CONTRACT.mechanismLevel.endpoint(p);
    const r = await fetchJson(base, ep);
    for (const f of MECH_FIELDS) {
      checks.push(mk('L3/L2 · Mechanism — aggregations + node verdict', 3, `${p.key} · ${p.mechanism} · ${f} = ${row[f]}`, MECHANISM_WITNESS[f] || `aggregation/verdict ${f}`, ep, f, row[f], judgeScalar(r, f, row[f])));
    }
  }

  // ---- L1 ATOMS ----------------------------------------------------------
  for (const [m, o] of Object.entries(EVIDENCE_ATOMS)) {
    const p = pByMech(m); const ep = CONTRACT.evidenceAtoms.endpoint(p); const r = await fetchJson(base, ep);
    checks.push(mk('L1 · Atoms — evidence / replication / control / calibration', 1, `evidence · ${p.key} · ${m} · ${o.count_qualified} qualified`, o.witness, ep, 'is_qualified_evidence:count', o.count_qualified, judgeListCount(r, (x) => x.is_qualified_evidence === true, o.count_qualified)));
  }
  for (const [m, o] of Object.entries(REPLICATION_ATOMS)) {
    const p = pByMech(m); const ep = CONTRACT.replicationAtoms.endpoint(p); const r = await fetchJson(base, ep);
    checks.push(mk('L1 · Atoms — evidence / replication / control / calibration', 1, `replication · ${p.key} · ${m} · ${o.count_cross_ancestry_concordant} cross-ancestry concordant`, o.witness, ep, 'is_cross_ancestry_concordant:count', o.count_cross_ancestry_concordant, judgeListCount(r, (x) => x.is_cross_ancestry_concordant === true, o.count_cross_ancestry_concordant)));
  }
  for (const [m, o] of Object.entries(CONTROL_ATOMS)) {
    const p = pByMech(m); const ep = CONTRACT.controlAtoms.endpoint(p); const r = await fetchJson(base, ep);
    checks.push(mk('L1 · Atoms — evidence / replication / control / calibration', 1, `control · ${p.key} · ${m} · ${o.count_survived} survived`, o.witness, ep, 'is_survived:count', o.count_survived, judgeListCount(r, (x) => x.is_survived === true, o.count_survived)));
  }
  for (const [pred, o] of Object.entries(CALIBRATION_ATOMS)) {
    const p = pByPred(pred); const ep = CONTRACT.calibrationAtoms.endpoint(p); const r = await fetchJson(base, ep);
    checks.push(mk('L1 · Atoms — evidence / replication / control / calibration', 1, `calibration · ${p.key} · ${pred} · ${o.count_well_calibrated_bins} well-calibrated bins`, o.witness, ep, 'is_well_calibrated_bin:count', o.count_well_calibrated_bins, judgeListCount(r, (x) => x.is_well_calibrated_bin === true, o.count_well_calibrated_bins)));
  }

  // ---- L0/L1 VARIANT -----------------------------------------------------
  const VAR_FIELDS = ['allele_frequency', 'is_rare_variant', 'has_allele_specific_expression', 'is_causal_candidate'];
  for (const p of PATIENTS) {
    const row = VARIANT_LEVEL[p.variant];
    if (!row) continue;
    const ep = CONTRACT.variantLevel.endpoint(p);
    const r = await fetchJson(base, ep);
    for (const f of VAR_FIELDS) {
      checks.push(mk('L0/L1 · Variant — IsCausalCandidate', 0, `${p.key} · ${p.variant} · ${f} = ${row[f]}`, VARIANT_WITNESS, ep, f, row[f], judgeScalar(r, f, row[f])));
    }
  }

  // ---- L8 LIFECYCLE — each case's walk through the diagnosis state machine ----
  for (const p of PATIENTS) {
    const o = LIFECYCLE_PATHS[p.prediction];
    const ep = CONTRACT.lifecycle.endpoint(p);
    const r = await fetchJson(base, ep);
    let judged;
    if (r.status === 501) judged = { status: 'not_surfaced', actual: null, detail: 'endpoint returns 501 (not wired yet)' };
    else if (r.status !== 200) judged = { status: 'fail', actual: null, detail: `HTTP ${r.status}` };
    else {
      const states = Array.isArray(r.body?.states) ? r.body.states : null;
      const okPath = states && JSON.stringify(states) === JSON.stringify(o.states);
      const okTerminal = r.body?.terminal === o.terminal;
      const okKeystone = r.body?.is_clinically_actionable === o.is_clinically_actionable;
      const ok = okPath && okTerminal && okKeystone;
      judged = {
        status: ok ? 'pass' : 'fail',
        actual: states,
        detail: ok ? '' : `expected path ${JSON.stringify(o.states)} (terminal ${o.terminal}, actionable ${o.is_clinically_actionable}), got ${JSON.stringify(states)} (terminal ${r.body?.terminal}, actionable ${r.body?.is_clinically_actionable})`,
      };
    }
    checks.push(mk('L8 · Case lifecycle — walk to the diagnosis', 8, `${p.key} · ${p.name} · ${o.states.join(' → ')}`, o.witness, ep, 'states', o.states, judged));
  }

  // ---- L9 ROUTING — per-entity RelativePath ------------------------------
  for (const e of ROUTING.relativePaths) {
    const r = await fetchJson(base, e.endpoint);
    checks.push(mk('L9 · Routing — per-entity RelativePath', 9, `${e.endpoint} · ${e.field} = ${e.expected}`, 'RelativePath is a computed field that chains the parent entity\'s path; every entity is linkable by its own id.', e.endpoint, e.field, e.expected, judgeScalar(r, e.field, e.expected)));
  }

  // ---- L9 ROUTING — role-based nav trees ---------------------------------
  for (const [role, expectedTops] of Object.entries(ROUTING.navTrees)) {
    const ep = CONTRACT.routingTree.endpoint(role);
    const r = await fetchJson(base, ep);
    let judged;
    if (r.status === 501) judged = { status: 'not_surfaced', actual: null, detail: 'endpoint returns 501 (not wired yet)' };
    else if (r.status !== 200) judged = { status: 'fail', actual: null, detail: `HTTP ${r.status}` };
    else {
      const tops = (r.body?.tree || []).map((n) => n.route_key).sort();
      const exp = [...expectedTops].sort();
      const ok = JSON.stringify(tops) === JSON.stringify(exp);
      judged = { status: ok ? 'pass' : 'fail', actual: tops, detail: ok ? '' : `expected top-level routes ${JSON.stringify(exp)}, got ${JSON.stringify(tops)}` };
    }
    checks.push(mk('L9 · Routing — role-based nav trees', 9, `role ${role} sees top-level: ${expectedTops.join(', ')}`, 'RoleVisibility filters the nav tree; admin sees all trees, each clinician role sees its own workflow.', ep, 'tree-tops', expectedTops, judged));
  }

  // ---- L11 DISEASE STATE — the v2 disease-state simulator (per individual) ----
  const DISEASE_FIELDS = ['nephritis_progression_state_key', 'latest_sledai_score', 'activity_tier', 'is_disease_progressing'];
  for (const e of PROGRESSION_INDIVIDUALS) {
    const row = DISEASE_STATE[e.individual];
    if (!row) continue;
    const ep = CONTRACT.diseaseState.endpoint(e);
    const r = await fetchJson(base, ep);
    for (const f of DISEASE_FIELDS) {
      checks.push(mk('L11 · Disease-state simulator — derived state + activity', 11, `${e.key} · ${e.name} · ${f} = ${row[f]}`, DISEASE_STATE_WITNESS[f], ep, f, row[f], judgeScalar(r, f, row[f])));
    }
  }

  // ---- L11b PROGRESSION WALK — the lupus-nephritis machine path per individual ----
  for (const e of PROGRESSION_INDIVIDUALS) {
    const o = PROGRESSION_PATHS[e.individual];
    if (!o) continue;
    const ep = CONTRACT.progression.endpoint(e);
    const r = await fetchJson(base, ep);
    let judged;
    if (r.status === 501) judged = { status: 'not_surfaced', actual: null, detail: 'endpoint returns 501 (not wired yet)' };
    else if (r.status !== 200) judged = { status: 'fail', actual: null, detail: `HTTP ${r.status}` };
    else {
      const states = Array.isArray(r.body?.states) ? r.body.states : null;
      const okPath = states && JSON.stringify(states) === JSON.stringify(o.states);
      const okCurrent = r.body?.current === o.current;
      const ok = okPath && okCurrent;
      judged = { status: ok ? 'pass' : 'fail', actual: states, detail: ok ? '' : `expected path ${JSON.stringify(o.states)} (current ${o.current}), got ${JSON.stringify(states)} (current ${r.body?.current})` };
    }
    checks.push(mk('L11b · Disease progression — walk through the lupus-nephritis machine', 11, `${e.key} · ${e.name} · ${o.states.join(' → ')}`, `Derived from raw serology trajectories; current state ${o.current} is the IsCurrent occupancy, never hand-set.`, ep, 'states', o.states, judged));
  }

  // ---- L5d TREATMENT-LINE + the DISAGREEMENT counter-example (per prediction) ----
  const TL_FIELDS = ['recommended_treatment_line', 'treatment_line_deciding_factor', 'progression_vs_actionability_disagree'];
  for (const p of PATIENTS) {
    const row = TREATMENT_LINE[p.prediction];
    if (!row) continue;
    const ep = CONTRACT.treatmentLine.endpoint(p);
    const r = await fetchJson(base, ep);
    for (const f of TL_FIELDS) {
      checks.push(mk('L5d · Treatment-line selection + the disagreement counter-example', 5, `${p.key} · ${p.name} · ${f} = ${row[f]}`, TREATMENT_LINE_WITNESS[f], ep, f, row[f], judgeScalar(r, f, row[f])));
    }
  }

  const summary = {
    total: checks.length,
    pass: checks.filter((c) => c.status === 'pass').length,
    fail: checks.filter((c) => c.status === 'fail').length,
    not_surfaced: checks.filter((c) => c.status === 'not_surfaced').length,
  };
  return { checks, summary };
}

// Group checks by category for tree rendering.
export function groupByCategory(checks) {
  const map = new Map();
  for (const c of checks) {
    if (!map.has(c.category)) map.set(c.category, []);
    map.get(c.category).push(c);
  }
  // sort categories by level descending (L7 keystone first — the conclusion)
  return [...map.entries()]
    .map(([category, items]) => ({
      category,
      level: items[0].level,
      items,
      pass: items.filter((i) => i.status === 'pass').length,
      total: items.length,
    }))
    .sort((a, b) => b.level - a.level || a.category.localeCompare(b.category));
}
