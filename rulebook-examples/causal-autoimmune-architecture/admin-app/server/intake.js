// ============================================================================
//  intake.js — THE WRITE PATH (Loop 3). Raw facts in -> derived diagnosis out.
// ============================================================================
//  This is the knob-editing payoff: an intake payload of ONLY raw leaf
//  observations (the exact two things the LLM/human may produce — interpreted
//  intake facts + synthetic-but-transparent test results) is written, in ONE
//  transaction, to the BASE tables. We then re-read the derived panel from the
//  vw_* views and hand back the keystone + deciding gate + diagnosis.
//
//  The trust boundary is enforced structurally: this module inserts raw columns
//  ONLY. It never writes ZStat, IsQualifiedEvidence, CausalConfidence, a gate,
//  the keystone, or DecidingGate — every one of those is computed by the views.
//  If a caller tries to send a derived field, it is ignored (we whitelist raw
//  columns per table). So the conclusion is never entered; it is always derived.
//
//  One patient's leaf set spans 10 base tables:
//    Individual -> GenomicVariant -> OmicsAssays -> CausalMechanism
//      -> EvidenceItems -> CohortReplications -> NegativeControlTests
//      -> InterventionTargets -> IndividualPrediction -> CalibrationBins
//  All ids/FKs are derived from a single patient slug so the caller never has
//  to thread foreign keys by hand.
// ============================================================================

import { pool, query } from './db.js';
import { buildDiagnosis, renderMarkdown } from './diagnosis.js';

// ---- tiny helpers -----------------------------------------------------------
const slugify = (s) =>
  String(s || '')
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '');

const isBool = (v) => v === true || v === false;
const isNum = (v) => typeof v === 'number' && Number.isFinite(v);
const isStr = (v) => typeof v === 'string' && v.length > 0;

class IntakeError extends Error {
  constructor(message) { super(message); this.name = 'IntakeError'; }
}

// Insert one row from a whitelisted {col: val} map. Raw columns ONLY — this is
// where the trust boundary is enforced: derived fields can't be smuggled in.
async function insertRow(client, table, row) {
  const cols = Object.keys(row);
  const params = cols.map((_, i) => `$${i + 1}`);
  const sql = `INSERT INTO ${table} (${cols.join(', ')}) VALUES (${params.join(', ')})`;
  await client.query(sql, cols.map((c) => row[c]));
}

// ---- payload validation (raw leaves only) -----------------------------------
//  We validate shape/types so a bad intake fails LOUDLY before any write,
//  rather than silently producing a half-built patient. The error names the
//  exact leaf at fault — these are knobs, so the message points at the knob.
function validateIntake(p) {
  if (!p || typeof p !== 'object') throw new IntakeError('intake payload must be a JSON object');
  const ind = p.individual;
  if (!ind || typeof ind !== 'object') throw new IntakeError('intake.individual is required');
  if (!isStr(ind.given_name) || !isStr(ind.family_name))
    throw new IntakeError('individual.given_name and individual.family_name are required');
  if (!isStr(ind.ancestry_label)) throw new IntakeError('individual.ancestry_label is required');
  if (!isStr(ind.federated_dataset)) throw new IntakeError('individual.federated_dataset is required');
  if (!isBool(ind.is_ancestry_absent_from_training))
    throw new IntakeError('individual.is_ancestry_absent_from_training (bool) is required — it gates ancestry-transport');
  if (!isBool(ind.has_cryptic_relatedness_flag))
    throw new IntakeError('individual.has_cryptic_relatedness_flag (bool) is required — it gates the leakage/anti-spurious branch');

  const v = p.variant;
  if (!v || !isStr(v.variant_type) || !isNum(v.allele_frequency))
    throw new IntakeError('variant.variant_type and numeric variant.allele_frequency are required');

  if (!Array.isArray(p.assays) || p.assays.length === 0)
    throw new IntakeError('at least one omics assay is required (it carries measurement_error_score/batch — the quality leaf)');
  p.assays.forEach((a, i) => {
    if (!isStr(a.omics_modality)) throw new IntakeError(`assays[${i}].omics_modality is required`);
    if (!isNum(a.measurement_error_score)) throw new IntakeError(`assays[${i}].measurement_error_score (number) is required`);
  });

  const m = p.mechanism;
  if (!m || !isStr(m.mechanism_type)) throw new IntakeError('mechanism.mechanism_type is required');
  if (!isBool(m.has_pleiotropy)) throw new IntakeError('mechanism.has_pleiotropy (bool) is required');

  if (!Array.isArray(p.evidence) || p.evidence.length === 0)
    throw new IntakeError('at least one evidence item is required');
  p.evidence.forEach((e, i) => {
    if (!isNum(e.effect_size) || !isNum(e.standard_error))
      throw new IntakeError(`evidence[${i}] needs numeric effect_size and standard_error (the Z-stat leaves)`);
    if (!isStr(e.omics_assay_ref))
      throw new IntakeError(`evidence[${i}].omics_assay_ref must name one of the supplied assays (by 0-based index "a<n>" or its modality)`);
  });

  // replications / controls / intervention targets / calibration are optional
  // arrays — but their absence has derived consequences the caller should know:
  // no intervention target -> not falsifiable; <2 concordant repls -> spurious.
  for (const key of ['replications', 'controls', 'interventionTargets', 'calibration']) {
    if (p[key] != null && !Array.isArray(p[key])) throw new IntakeError(`intake.${key} must be an array when present`);
  }

  const pred = p.prediction;
  if (!pred || !isStr(pred.autoimmune_disease) || !isStr(pred.prediction_type))
    throw new IntakeError('prediction.autoimmune_disease and prediction.prediction_type are required');
}

// ============================================================================
//  ingestIntake(payload) — the whole write path, transactionally.
//  Returns { individualId, predictionId, prediction (derived panel), diagnosis }.
// ============================================================================
export async function ingestIntake(payload) {
  validateIntake(payload);

  const slug = slugify(`${payload.individual.family_name}-${payload.individual.given_name}`);
  if (!slug) throw new IntakeError('could not derive a slug from the patient name');

  // Refuse to clobber an existing patient — intake is create, not overwrite.
  const existing = await query('SELECT 1 FROM individuals WHERE individual_id = $1', [`ind-${slug}`]);
  if (existing.length) throw new IntakeError(`a patient with id 'ind-${slug}' already exists; pick a different name`);

  const indId = `ind-${slug}`;
  const varId = `var-${slug}`;
  const mechId = `cm-${slug}`;
  const predId = `pred-${slug}`;

  const client = await pool.connect();
  try {
    await client.query('BEGIN');

    // 1) Individual (intake facts) ------------------------------------------
    const ind = payload.individual;
    await insertRow(client, 'individuals', {
      individual_id: indId,
      given_name: ind.given_name,
      family_name: ind.family_name,
      ancestry_label: ind.ancestry_label,
      age_years: isNum(ind.age_years) ? ind.age_years : null,
      is_ancestry_absent_from_training: ind.is_ancestry_absent_from_training,
      federated_dataset: ind.federated_dataset,
      enrollment_date: isStr(ind.enrollment_date) ? ind.enrollment_date : null,
      has_cryptic_relatedness_flag: ind.has_cryptic_relatedness_flag,
    });

    // 2) GenomicVariant ------------------------------------------------------
    const v = payload.variant;
    await insertRow(client, 'genomic_variants', {
      genomic_variant_id: varId,
      variant_label: v.variant_label || varId,
      individual: indId,
      variant_type: v.variant_type,
      allele_frequency: v.allele_frequency,
      has_allele_specific_expression: isBool(v.has_allele_specific_expression) ? v.has_allele_specific_expression : false,
    });

    // 3) OmicsAssays (quality leaves) — build a ref map for evidence FKs -----
    const assayIdByRef = {};
    for (let i = 0; i < payload.assays.length; i++) {
      const a = payload.assays[i];
      const assayId = `assay-${slug}-${i}`;
      assayIdByRef[`a${i}`] = assayId;            // positional ref:  "a0", "a1"
      if (isStr(a.omics_modality)) assayIdByRef[a.omics_modality] = assayId; // modality ref
      await insertRow(client, 'omics_assays', {
        omics_assay_id: assayId,
        assay_label: a.assay_label || assayId,
        individual: indId,
        omics_modality: a.omics_modality,
        tissue: a.tissue || null,
        federated_dataset: a.federated_dataset || ind.federated_dataset,
        batch_id: a.batch_id || null,
        measurement_error_score: a.measurement_error_score,
        has_cell_state_specific_effect: isBool(a.has_cell_state_specific_effect) ? a.has_cell_state_specific_effect : false,
      });
    }

    // 4) CausalMechanism -----------------------------------------------------
    const m = payload.mechanism;
    await insertRow(client, 'causal_mechanisms', {
      causal_mechanism_id: mechId,
      mechanism_label: m.mechanism_label || mechId,
      individual: indId,
      genomic_variant: varId,
      environmental_exposure: m.environmental_exposure || '',
      mechanism_type: m.mechanism_type,
      has_pleiotropy: m.has_pleiotropy,
    });

    // 5) EvidenceItems (the test-result leaves) ------------------------------
    for (let i = 0; i < payload.evidence.length; i++) {
      const e = payload.evidence[i];
      const assayId = assayIdByRef[e.omics_assay_ref];
      if (!assayId) {
        throw new IntakeError(
          `evidence[${i}].omics_assay_ref '${e.omics_assay_ref}' matches no supplied assay (use "a0".."a${payload.assays.length - 1}" or a modality)`,
        );
      }
      await insertRow(client, 'evidence_items', {
        evidence_item_id: `ev-${slug}-${i}`,
        evidence_label: e.evidence_label || `ev-${slug}-${i}`,
        causal_mechanism: mechId,
        omics_assay: assayId,
        effect_size: e.effect_size,
        standard_error: e.standard_error,
        is_cross_modality: isBool(e.is_cross_modality) ? e.is_cross_modality : false,
        is_negative_control_arm: isBool(e.is_negative_control_arm) ? e.is_negative_control_arm : false,
        is_adjusted_for_ancestry_pcs: isBool(e.is_adjusted_for_ancestry_pcs) ? e.is_adjusted_for_ancestry_pcs : false,
        is_adjusted_for_batch: isBool(e.is_adjusted_for_batch) ? e.is_adjusted_for_batch : false,
        is_synthetic_leaf: isBool(e.is_synthetic_leaf) ? e.is_synthetic_leaf : true,
        represents_assay_modality: e.represents_assay_modality || null,
        identification_assumption: e.identification_assumption || null,
      });
    }

    // 6) CohortReplications (optional) ---------------------------------------
    const reps = payload.replications || [];
    for (let i = 0; i < reps.length; i++) {
      const r = reps[i];
      await insertRow(client, 'cohort_replications', {
        cohort_replication_id: `rep-${slug}-${i}`,
        replication_label: r.replication_label || `rep-${slug}-${i}`,
        causal_mechanism: mechId,
        federated_dataset: r.federated_dataset || null,
        replication_effect_sign: isNum(r.replication_effect_sign) ? r.replication_effect_sign : 0,
        replication_p_value: isNum(r.replication_p_value) ? r.replication_p_value : 1,
        replication_ancestry_label: r.replication_ancestry_label || null,
      });
    }

    // 7) NegativeControlTests (optional) -------------------------------------
    const controls = payload.controls || [];
    for (let i = 0; i < controls.length; i++) {
      const c = controls[i];
      await insertRow(client, 'negative_control_tests', {
        negative_control_test_id: `nct-${slug}-${i}`,
        control_label: c.control_label || `nct-${slug}-${i}`,
        causal_mechanism: mechId,
        test_kind: c.test_kind || 'permutation',
        permutation_effect_size: isNum(c.permutation_effect_size) ? c.permutation_effect_size : 0,
        null_threshold: isNum(c.null_threshold) ? c.null_threshold : 0.1,
      });
    }

    // 8) InterventionTargets (optional) — absence => not falsifiable ----------
    const targets = payload.interventionTargets || [];
    for (let i = 0; i < targets.length; i++) {
      const t = targets[i];
      await insertRow(client, 'intervention_targets', {
        intervention_target_id: `it-${slug}-${i}`,
        target_label: t.target_label || `it-${slug}-${i}`,
        causal_mechanism: mechId,
        autoimmune_disease: t.autoimmune_disease || payload.prediction.autoimmune_disease,
        therapy_class: t.therapy_class || null,
        is_validated: isBool(t.is_validated) ? t.is_validated : false,
      });
    }

    // 9) IndividualPrediction ------------------------------------------------
    const pred = payload.prediction;
    await insertRow(client, 'individual_predictions', {
      individual_prediction_id: predId,
      prediction_label: pred.prediction_label || predId,
      individual: indId,
      autoimmune_disease: pred.autoimmune_disease,
      prediction_type: pred.prediction_type,
    });

    // 10) CalibrationBins (optional) — reliability leaves ---------------------
    const bins = payload.calibration || [];
    for (let i = 0; i < bins.length; i++) {
      const b = bins[i];
      await insertRow(client, 'calibration_bins', {
        calibration_bin_id: `cb-${slug}-${i}`,
        bin_label: b.bin_label || `cb-${slug}-${i}`,
        individual_prediction: predId,
        predicted_probability_band: isNum(b.predicted_probability_band) ? b.predicted_probability_band : null,
        observed_event_rate: isNum(b.observed_event_rate) ? b.observed_event_rate : null,
        coverage_count: isNum(b.coverage_count) ? b.coverage_count : 0,
      });
    }

    await client.query('COMMIT');
  } catch (err) {
    await client.query('ROLLBACK');
    throw err;
  } finally {
    client.release();
  }

  // Re-read the DERIVED panel from the views (never base tables). The keystone,
  // deciding gate, and diagnosis are computed downstream of the leaves we wrote.
  const panel = await query('SELECT * FROM vw_individual_predictions WHERE individual_prediction_id = $1', [predId]);
  const diagnosis = await buildDiagnosis(predId);

  return {
    individualId: indId,
    predictionId: predId,
    prediction: panel[0] || null,
    diagnosis,
    diagnosisMarkdown: diagnosis ? renderMarkdown(diagnosis) : null,
  };
}

export { IntakeError };
