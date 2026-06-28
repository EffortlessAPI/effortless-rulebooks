// L10 — THE WRITE PATH (Loop 3). Raw facts in -> derived diagnosis out.
//
// Proves the keystone capability the whole app exists for: POST an intake of
// ONLY raw leaf observations, and the views derive the entire chain to the
// keystone with NO hand-entered conclusion. Then flip a single leaf knob and
// watch the verdict flip — the falsifiable/configurable property. Finally,
// prove the trust boundary: a smuggled derived conclusion is ignored, not honored.
//
// Each test cleans up the patient it creates (DELETE /api/individuals/:id), so
// the canonical 7-patient oracle is untouched and this suite is re-runnable.

import { test, before, after, describe } from 'node:test';
import assert from 'node:assert/strict';
import { startServer, stopServer, getJson } from './server-fixture.js';

// A fully-specified, ALL-GATES-PASS intake (raw leaves only — no conclusion).
const ACTIONABLE_INTAKE = {
  individual: {
    given_name: 'Harness', family_name: 'WritePath',
    ancestry_label: 'European', age_years: 40,
    is_ancestry_absent_from_training: false,
    federated_dataset: 'fed-europe', enrollment_date: '2026-06-19',
    has_cryptic_relatedness_flag: false,
  },
  variant: { variant_type: 'regulatory', allele_frequency: 0.04, has_allele_specific_expression: true },
  assays: [
    { omics_modality: 'rna-seq', tissue: 'blood', measurement_error_score: 0.05, batch_id: 'b1' },
    { omics_modality: 'atac-seq', tissue: 'blood', measurement_error_score: 0.06, batch_id: 'b1' },
    { omics_modality: 'proteomics', tissue: 'blood', measurement_error_score: 0.07, batch_id: 'b1' },
  ],
  mechanism: { mechanism_type: 'regulatory', has_pleiotropy: false },
  evidence: [
    { omics_assay_ref: 'a0', effect_size: 0.8, standard_error: 0.2, is_cross_modality: false, is_adjusted_for_ancestry_pcs: true, is_adjusted_for_batch: true },
    { omics_assay_ref: 'a1', effect_size: 0.7, standard_error: 0.2, is_cross_modality: true, is_adjusted_for_ancestry_pcs: true, is_adjusted_for_batch: true },
    { omics_assay_ref: 'a2', effect_size: 0.6, standard_error: 0.2, is_cross_modality: true, is_adjusted_for_ancestry_pcs: true, is_adjusted_for_batch: true },
  ],
  replications: [
    { replication_effect_sign: 1, replication_p_value: 0.01, replication_ancestry_label: 'East Asian', federated_dataset: 'fed-east-asia' },
    { replication_effect_sign: 1, replication_p_value: 0.02, replication_ancestry_label: 'African', federated_dataset: 'fed-west-africa' },
  ],
  controls: [
    { permutation_effect_size: 0.01, null_threshold: 0.1 },
    { permutation_effect_size: 0.02, null_threshold: 0.1 },
  ],
  interventionTargets: [{ therapy_class: 'jak-inhibitor', is_validated: true }],
  prediction: { autoimmune_disease: 'ra', prediction_type: 'onset-risk' },
  calibration: [
    { predicted_probability_band: 0.1, observed_event_rate: 0.09, coverage_count: 25 },
    { predicted_probability_band: 0.3, observed_event_rate: 0.31, coverage_count: 25 },
    { predicted_probability_band: 0.5, observed_event_rate: 0.52, coverage_count: 25 },
    { predicted_probability_band: 0.7, observed_event_rate: 0.69, coverage_count: 25 },
    { predicted_probability_band: 0.9, observed_event_rate: 0.91, coverage_count: 25 },
  ],
};

// POST helper that goes through the real Express app via the fixture's base URL.
async function postIntake(payload) {
  return getJson('/api/intake', { method: 'POST', body: JSON.stringify(payload), headers: { 'Content-Type': 'application/json' } });
}
async function del(id) {
  return getJson(`/api/individuals/${id}`, { method: 'DELETE' });
}

describe('L10 · WRITE PATH · raw facts in -> derived diagnosis out', () => {
  before(startServer);
  after(stopServer);

  test('intake of raw leaves derives an ACTIONABLE keystone (no conclusion entered)', async () => {
    const { status, body } = await postIntake(ACTIONABLE_INTAKE);
    try {
      assert.equal(status, 201, `expected 201, got ${status}: ${JSON.stringify(body)}`);
      assert.ok(body.prediction, 'response must include the derived prediction panel');
      assert.equal(body.prediction.is_clinically_actionable, true, 'keystone must derive TRUE from the leaves');
      assert.equal(body.prediction.deciding_gate, 'AllGatesPass', 'deciding gate must be AllGatesPass');
      assert.equal(body.prediction.is_high_confidence_prediction, true);
      assert.equal(body.prediction.is_falsifiability_backed, true);
      assert.equal(body.prediction.is_ancestry_transport_safe, true);
      assert.ok(Number(body.prediction.predicted_value) > 0, 'predicted_value must be > 0');
      assert.ok(body.diagnosisMarkdown && body.diagnosisMarkdown.length > 0, 'a diagnosis writeup must render');
    } finally {
      await del('ind-writepath-harness');
    }
  });

  test('flipping ONE leaf knob (remove intervention target) flips the verdict to NoValidatedMechanism', async () => {
    const noTarget = structuredClone(ACTIONABLE_INTAKE);
    noTarget.individual.family_name = 'WritePathNoTarget';
    noTarget.interventionTargets = [];
    const { status, body } = await postIntake(noTarget);
    try {
      assert.equal(status, 201, `expected 201, got ${status}: ${JSON.stringify(body)}`);
      assert.equal(body.prediction.is_clinically_actionable, false, 'no intervention target ⇒ not falsifiable ⇒ not actionable');
      assert.equal(body.prediction.is_falsifiability_backed, false);
      assert.equal(body.prediction.deciding_gate, 'NoValidatedMechanism');
    } finally {
      await del('ind-writepathnotarget-harness');
    }
  });

  test('TRUST BOUNDARY: a smuggled derived conclusion is ignored, not honored', async () => {
    const smuggle = structuredClone(ACTIONABLE_INTAKE);
    smuggle.individual.family_name = 'WritePathSmuggle';
    smuggle.interventionTargets = []; // would be NOT actionable on the merits
    smuggle.prediction.is_clinically_actionable = true; // ← the smuggle attempt
    smuggle.evidence[0].is_qualified_evidence = true; // ← another derived field
    const { status, body } = await postIntake(smuggle);
    try {
      assert.equal(status, 201, `expected 201, got ${status}: ${JSON.stringify(body)}`);
      assert.equal(body.prediction.is_clinically_actionable, false, 'derived conclusion must be re-derived (false), never accepted from the payload');
      assert.equal(body.prediction.deciding_gate, 'NoValidatedMechanism');
    } finally {
      await del('ind-writepathsmuggle-harness');
    }
  });

  test('a malformed intake is rejected (400) before any write, naming the bad knob', async () => {
    const bad = structuredClone(ACTIONABLE_INTAKE);
    bad.individual.family_name = 'WritePathBad';
    delete bad.individual.ancestry_label; // required leaf
    const { status, body } = await postIntake(bad);
    assert.equal(status, 400, 'missing required leaf must 400');
    assert.equal(body.error, 'invalid_intake');
    assert.match(body.detail, /ancestry_label/, 'error must name the offending knob');
    // nothing should have been written
    const probe = await getJson('/api/individuals/ind-writepathbad-harness');
    assert.equal(probe.status, 404, 'a rejected intake must write nothing');
  });
});
