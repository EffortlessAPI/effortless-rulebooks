// ============================================================================
//  diagnosis.js — the REAL PAYOFF: a doctor-style writeup per patient.
// ============================================================================
//  Given a prediction id, this reads the FULL inference chain the app surfaces
//  (raw facts -> derivations -> the four gates -> keystone) and renders a
//  narrative a clinician would expect:
//
//    "The patient presented with ... tests xyz confirmed abc ... therefore C."
//
//  It needs no UI: a passing run produces this artifact. Markdown today; once
//  the inference endpoints are green, the same text renders to diagnosis.pdf.
//
//  IMPORTANT: this module NEVER computes the conclusion. It consumes the derived
//  fields from vw_* (via the route layer) as opaque truth, and only arranges
//  them into prose. If a field isn't surfaced yet, the writeup says so honestly.
// ============================================================================

import { query } from './db.js';

const yn = (b) => (b === true ? 'YES' : b === false ? 'no' : '—');
const passfail = (b) => (b === true ? 'PASS' : b === false ? 'FAIL' : 'unknown');
const num = (n, d = 2) => (n == null ? '—' : Number(n).toFixed(d));
// Panel 2→1 provenance pointer: render the literal span of the case narrative
// this leaf's raw value was extracted from. Lets a reader check the EXTRACTION
// was faithful, independently of trusting the derived verdict.
const src = (q) => (q ? `\n  - ↳ _from the case:_ “${q}”` : '');
// Honest three-state render of the transport gate (consumes the derived
// TransportGateStatus). A vacuous in-training pass is shown as n/a, never PASS,
// so the writeup never implies transport evidence the keystone never used.
const transportStatus = (s) =>
  s === 'NotApplicable' ? 'n/a — in-training ancestry (gate did not bite)'
  : s === 'PASS-tested' ? 'PASS (tested) — measured cross-ancestry transport'
  : s === 'FAIL' ? 'FAIL — holdout ancestry with no cross-ancestry transport'
  : 'unknown';

// Map the four gates to a plain-English deciding reason.
function decidingReason(p) {
  if (p.is_clinically_actionable) {
    return p.is_ancestry_holdout
      ? 'All gates pass — and, despite this ancestry being held out of training, the carried mechanism showed **measured cross-ancestry replication**, so the prediction transports.'
      : 'All four gates pass: the prediction rests on a confident, falsifiable, non-spurious mechanism, is well-calibrated, and (in-training) is transport-safe.';
  }
  // find the single failing gate, in the order the keystone ANDs them
  if (p.is_high_confidence_prediction === false) {
    if (p.calibrated_uncertainty != null && Number(p.calibrated_uncertainty) < 0.7 && p.has_spurious_correlation_flag === false)
      return '**Calibration gate FAILED:** held-out reliability coverage was insufficient (CalibratedUncertainty < 0.7), even though the mechanism is confirmed.';
    if (p.rests_on_confirmed_mechanism === false)
      return '**Anti-spurious gate FAILED:** the prediction rests on no validated causal mechanism (the candidate failed replication-sign and/or a negative control).';
    if (p.individual_has_cryptic_relatedness === true)
      return '**Anti-spurious gate FAILED (leakage):** cryptic relatedness flags the prediction as potentially confounded, despite a valid mechanism and good calibration.';
    return '**High-confidence gate FAILED** (calibration or anti-spurious).';
  }
  if (p.is_falsifiability_backed === false)
    return '**Falsifiability gate FAILED:** the mechanism has no perturbable intervention target, so it is not experimentally falsifiable.';
  if (p.is_ancestry_transport_safe === false)
    return '**Ancestry-transport gate FAILED:** this ancestry is out-of-distribution and the carried mechanism was never replicated in a different ancestry (no measured cross-ancestry invariance).';
  if (Number(p.predicted_value) <= 0)
    return '**Magnitude gate FAILED:** no mechanism-derived predicted value (rests on no confirmed causal mass).';
  return 'Not actionable (deciding gate undetermined from surfaced fields).';
}

// Map the derived severity gate (chained to the onset mechanism gates) to a
// plain-English deciding reason. Consumes the derived fields as opaque truth.
function severityReason(p) {
  switch (p.severity_deciding_factor) {
    case 'HighSeverityOnConfirmedMechanism':
      return p.is_clinically_actionable
        ? 'A high-severity phenotype on a confirmed, non-spurious mechanism — and the onset prediction is itself actionable.'
        : '**Severity is actionable even though the onset prediction is not:** the mechanism is confirmed and non-spurious, so the high-severity claim stands on its own (the onset verdict failed for a *different* reason).';
    case 'NotHighSeverity':
      return `Not severity-actionable: the maximum clinical severity (**${num(p.predicted_severity_value, 0)}**, tier ${p.severity_tier}) does not clear the high-severity threshold (>7).`;
    case 'NoValidatedMechanism':
      return '**Severity gate blocked by the mechanism chain:** the severity score is high, but it rests on no validated causal mechanism — so a high severity number alone is not actionable.';
    case 'SpuriousFlag':
      return '**Severity gate blocked by the mechanism chain:** severe and on a confirmed mechanism, but a spurious-correlation (cryptic-relatedness) flag means the prediction may be confounded.';
    default:
      return 'Severity actionability undetermined from surfaced fields.';
  }
}

// Map the derived treatment-response gate (grounded in a mechanism match) to a
// plain-English deciding reason. Consumes the derived fields as opaque truth.
function treatmentReason(p) {
  switch (p.treatment_response_deciding_factor) {
    case 'EffectiveOnConfirmedMechanism':
      return p.is_clinically_actionable
        ? 'An effective therapy (Complete/Partial, no adverse effect) that targets the **confirmed** causal mechanism — the response is mechanistically grounded.'
        : '**Treatment response is actionable even though the onset prediction is not:** the therapy responds and targets the confirmed mechanism, so the response prediction stands on its own (onset failed for a *different* reason).';
    case 'NoEffectiveTreatmentOnMechanism':
      return '**No effective treatment on the confirmed mechanism:** the therapy targets a confirmed mechanism, but it was adverse or produced no response — so a response is not predicted.';
    case 'NoConfirmedMechanism':
      return '**Treatment-response gate blocked by the mechanism match:** the therapy targets a mechanism that failed validation (not a confirmed causal node), so any apparent response is not mechanistically grounded.';
    default:
      return 'Treatment-response actionability undetermined from surfaced fields.';
  }
}

// Build a structured diagnosis object from live views (read-only, opaque truth).
export async function buildDiagnosis(predictionId) {
  const preds = await query('SELECT * FROM vw_individual_predictions WHERE individual_prediction_id = $1', [predictionId]);
  if (!preds.length) return null;
  const p = preds[0];

  // Panel 1 (the input case text) lives on the individual, not the prediction.
  // Attach it onto p so the renderer can open with it.
  const inds = await query('SELECT case_narrative FROM vw_individuals WHERE individual_id = $1', [p.individual]);
  if (inds.length) p.case_narrative = inds[0].case_narrative;

  const mechs = await query('SELECT * FROM vw_causal_mechanisms WHERE individual = $1', [p.individual]);
  const variants = await query('SELECT * FROM vw_genomic_variants WHERE individual = $1', [p.individual]);
  const bins = await query('SELECT * FROM vw_calibration_bins WHERE individual_prediction = $1 ORDER BY calibration_bin_id', [predictionId]);

  // child rows for the (typically one) mechanism
  let evidence = [], reps = [], controls = [];
  if (mechs.length) {
    const cm = mechs[0].causal_mechanism_id;
    evidence = await query('SELECT * FROM vw_evidence_items WHERE causal_mechanism = $1 ORDER BY evidence_item_id', [cm]);
    reps = await query('SELECT * FROM vw_cohort_replications WHERE causal_mechanism = $1 ORDER BY cohort_replication_id', [cm]);
    controls = await query('SELECT * FROM vw_negative_control_tests WHERE causal_mechanism = $1 ORDER BY negative_control_test_id', [cm]);
  }

  return { p, mechs, variants, bins, evidence, reps, controls };
}

// ----------------------------------------------------------------------------
//  buildWitness — the THREE-PANEL shape for the interactive UI.
//    panel1: the case narrative (the LLM's raw input)
//    panel2: a flat list of extracted leaf facts, each with the literal
//            sourceQuote span it came from (and whether it's a synthetic leaf)
//    panel3: the conclusion (keystone + deciding reason + the four gates)
//  The UI highlights a fact's sourceQuote inside panel1 on hover/click — so a
//  human checks the EXTRACTION (panel2 ↔ panel1) independently of the VERDICT.
// ----------------------------------------------------------------------------
export async function buildWitness(predictionId) {
  const d = await buildDiagnosis(predictionId);
  if (!d) return null;
  const { p, mechs, variants, bins, evidence, reps, controls } = d;
  const m = mechs[0];

  const facts = [];
  const push = (id, group, label, value, row) =>
    facts.push({ id, group, label, value, sourceQuote: row.source_quote || null, isSynthetic: row.is_synthetic_leaf === true });

  for (const v of variants)
    push(v.genomic_variant_id, 'variant', `${v.genomic_variant_id} — allele frequency / ASE`,
      `AF ${num(v.allele_frequency, 4)} (${v.is_rare_variant ? 'rare' : 'common'}), ASE ${yn(v.has_allele_specific_expression)} ⇒ candidate ${yn(v.is_causal_candidate)}`, v);
  for (const e of evidence)
    push(e.evidence_item_id, 'evidence', `${e.evidence_item_id} — effect / SE`,
      `ZStat ${num(e.z_stat)} ⇒ qualified ${yn(e.is_qualified_evidence)}`, e);
  for (const r of reps)
    push(r.cohort_replication_id, 'replication', `${r.cohort_replication_id} — ${r.replication_ancestry_label} cohort`,
      `p=${num(r.replication_p_value, 3)}, sign ${r.replication_effect_sign > 0 ? '+' : r.replication_effect_sign < 0 ? '−' : '0'} ⇒ cross-ancestry ${yn(r.is_cross_ancestry_concordant)}`, r);
  for (const c of controls)
    push(c.negative_control_test_id, 'control', `${c.negative_control_test_id} — permutation control`,
      `perm ${num(c.permutation_effect_size, 3)} vs ±${num(c.null_threshold, 2)} ⇒ survived ${yn(c.is_survived)}`, c);
  for (const b of bins)
    push(b.calibration_bin_id, 'calibration', `${b.calibration_bin_id} — reliability bin`,
      `band ${num(b.predicted_probability_band, 2)}, observed ${num(b.observed_event_rate, 2)}, coverage ${b.coverage_count}`, b);

  return {
    predictionId,
    panel1: { individual: p.individual, ancestry: p.individual_ancestry_label, isHoldout: p.is_ancestry_holdout, narrative: p.case_narrative || null },
    panel2: facts,
    panel3: {
      isClinicallyActionable: p.is_clinically_actionable,
      decidingReason: decidingReason(p),
      stratificationTier: p.patient_stratification_tier,
      gates: [
        { label: 'Causally grounded (magnitude)', pass: Number(p.predicted_value) > 0, detail: `PredictedValue ${num(p.predicted_value)}` },
        { label: 'Well calibrated ∧ not spurious', pass: p.is_high_confidence_prediction, detail: `CalibratedUncertainty ${num(p.calibrated_uncertainty)}` },
        { label: 'Falsifiability-backed', pass: p.is_falsifiability_backed, detail: m ? `${m.count_intervention_targets} intervention target(s)` : '—' },
        { label: 'Ancestry-transport-safe', pass: p.is_ancestry_transport_safe, detail: transportStatus(p.transport_gate_status) },
      ],
    },
  };
}

// Render the structured diagnosis to Markdown.
export function renderMarkdown(d) {
  if (!d) return null;
  const { p, mechs, variants, bins, evidence, reps, controls } = d;
  const m = mechs[0];
  const L = [];

  L.push(`# Diagnosis Report — ${p.individual}`);
  L.push('');
  L.push('> **Demonstration of inference structure, not validated clinical decision support.**');
  L.push(`> Generated from the derived inference chain (\`vw_*\`). Conclusion is computed, never entered.`);
  L.push('> 🧪-marked rows are **synthetic leaves** (LLM-produced transparent test results for this invented case); every other value is **derived** by formula. The trust boundary is a line in the DAG. Each leaf carries a ↳ pointer back into the **case text** (Panel 1) it was extracted from, so the extraction is checkable independently of the verdict.');
  L.push('');
  L.push(`**Prediction:** ${p.individual_prediction_id} · **Disease:** ${p.autoimmune_disease || '—'} · **Type:** ${p.prediction_type || '—'}`);
  L.push(`**Ancestry:** ${p.individual_ancestry_label} ${p.is_ancestry_holdout ? '(held out of training)' : '(in training)'}`);
  L.push('');

  // ---- PANEL 1: the input case text (what the LLM was handed) ----
  // The whole point of the three-panel witness: this is the messy NL input.
  // Every raw observation below is extracted from THIS text, and each carries a
  // ↳ pointer back into it — so a human can check the extraction was faithful
  // SEPARATELY from trusting the verdict. That is what stops the conclusion
  // from being "a hallucination laundered through a deterministic function".
  if (p.case_narrative) {
    L.push('## Presenting case (Panel 1 — the LLM\'s raw input)');
    L.push('');
    L.push('> ' + String(p.case_narrative).replace(/\n+/g, '\n> '));
    L.push('');
    L.push('_Below, every extracted fact carries a ↳ pointer back into this text (Panel 2). '
      + 'The extraction is checkable here; the derivation (the gates) is checkable in the Assessment. '
      + 'The two halves are independent — which is the point._');
    L.push('');
  }

  // ---- PANEL 2: PRESENTATION (raw facts extracted from the case) ----
  L.push('## Presentation (raw observations — Panel 2)');
  if (variants.length) {
    for (const v of variants) {
      L.push(`- **Genomic variant ${v.genomic_variant_id}:** allele frequency ${num(v.allele_frequency, 4)} ` +
        `(${v.is_rare_variant ? 'rare' : 'common'}), allele-specific expression ${yn(v.has_allele_specific_expression)} ` +
        `⇒ candidate causal variant: **${yn(v.is_causal_candidate)}**.` + src(v.source_quote));
    }
  } else {
    L.push('- No genomic variants on record.');
  }
  L.push(`- Cryptic-relatedness leakage flag: **${yn(p.individual_has_cryptic_relatedness)}**.`);
  L.push('');

  // ---- DIAGNOSTIC WORKUP (the tests) ----
  L.push('## Diagnostic workup (tests run)');
  if (m) {
    L.push(`### Mechanism ${m.causal_mechanism_id}`);
    L.push('');
    L.push('**Evidence assays:**');
    if (evidence.length) {
      for (const e of evidence) {
        const leaf = e.is_synthetic_leaf ? ' 🧪_synthetic_' : '';
        L.push(`- ${e.evidence_item_id}:${leaf} ZStat ${num(e.z_stat)} ` +
          `(high-quality assay ${yn(e.assay_is_high_quality)}, confound-controlled ${yn(e.is_confound_controlled)}, ` +
          `cross-modality ${yn(e.is_cross_modality)}) ⇒ qualified: **${yn(e.is_qualified_evidence)}**.` + src(e.source_quote));
        if (e.represents_assay_modality)
          L.push(`  - _represents:_ ${e.represents_assay_modality}` +
            `${e.identification_assumption ? ` — _valid under:_ ${e.identification_assumption}` : ''}`);
      }
      L.push(`- ⇒ ${m.count_qualified_evidence} qualified evidence rows across ${m.count_modalities_supporting} modalities.`);
    } else L.push('- (none)');
    L.push('');
    L.push('**Cross-cohort replication:**');
    for (const r of reps) {
      L.push(`- ${r.cohort_replication_id}: ${r.replication_ancestry_label} cohort, ` +
        `p=${num(r.replication_p_value, 3)}, sign ${r.replication_effect_sign > 0 ? '+' : r.replication_effect_sign < 0 ? '−' : '0'} ⇒ ` +
        `replicated@sig ${yn(r.replicated_at_nominal_sig)}, cross-ancestry-concordant **${yn(r.is_cross_ancestry_concordant)}**.` + src(r.source_quote));
    }
    L.push(`- ⇒ ${m.count_concordant_replications}/${m.count_replications} concordant; ${m.count_cross_ancestry_concordant} cross-ancestry concordant ⇒ replicates-across-cohorts: **${yn(m.replicates_across_cohorts)}**.`);
    L.push('');
    L.push('**Negative controls:**');
    for (const c of controls) {
      L.push(`- ${c.negative_control_test_id}: permutation effect ${num(c.permutation_effect_size, 3)} vs null ±${num(c.null_threshold, 2)} ⇒ survived: **${yn(c.is_survived)}**.` + src(c.source_quote));
    }
    L.push(`- ⇒ ${m.count_neg_control_survived}/${m.count_neg_control_tests} survived ⇒ survives-negative-controls: **${yn(m.survives_negative_controls)}**.`);
    L.push('');
    L.push('**Falsifiability:** ' + `${m.count_intervention_targets} intervention target(s) ⇒ experimentally falsifiable: **${yn(m.is_experimentally_falsifiable)}**.`);
    L.push('');
    L.push(`**Mechanism verdict:** causal confidence ${num(m.causal_confidence)} (threshold 0.70), spurious ${yn(m.is_spurious_derived)} ⇒ ` +
      `**IsCausalArchitectureNode = ${yn(m.is_causal_architecture_node)}**` +
      `${m.is_causal_architecture_node ? `, ancestry-transportable ${yn(m.is_ancestry_transportable)}` : ''}.`);
  } else {
    L.push('- No causal mechanisms on record for this individual.');
  }
  L.push('');
  L.push('**Calibration (reliability bins):** ' +
    `${p.count_well_calibrated_bins}/${p.count_bins} bins well-calibrated ` +
    `(coverage≥20 & |gap|≤0.1) ⇒ CalibratedUncertainty **${num(p.calibrated_uncertainty)}** (threshold 0.70).` +
    src(bins.find((b) => b.source_quote)?.source_quote));
  L.push('');

  // ---- ASSESSMENT (the four gates) ----
  L.push('## Assessment (the four gates)');
  L.push('');
  L.push('| Gate | Result |');
  L.push('|---|---|');
  L.push(`| Causally grounded (mechanism-derived magnitude) | **${passfail(Number(p.predicted_value) > 0)}** (PredictedValue ${num(p.predicted_value)}) |`);
  L.push(`| Well calibrated ∧ not spurious | **${passfail(p.is_high_confidence_prediction)}** |`);
  L.push(`| Falsifiability-backed | **${passfail(p.is_falsifiability_backed)}** |`);
  L.push(`| Ancestry-transport-safe | **${transportStatus(p.transport_gate_status)}** |`);
  L.push('');

  // ---- CONCLUSION (keystone) ----
  L.push('## Conclusion');
  L.push('');
  L.push(`**Stratification tier:** ${p.patient_stratification_tier}`);
  L.push('');
  L.push(`### IsClinicallyActionable: ${p.is_clinically_actionable ? '✅ **TRUE**' : '⛔ **FALSE**'}`);
  L.push('');
  L.push(decidingReason(p));
  L.push('');

  // ---- SECOND PREDICTION: severity (Loop 4) ----
  // A separate derived prediction grounded in ClinicalPhenotypes.SeverityScore,
  // chained to the onset mechanism gates so it can't fire on a debunked mechanism.
  // It is independent of the onset verdict — pred-b is the marquee: onset FALSE,
  // severity TRUE. Consumed as opaque truth from the view.
  if (p.severity_tier != null) {
    L.push('## Severity prediction (second derived prediction)');
    L.push('');
    L.push(`**Max clinical severity:** ${num(p.predicted_severity_value, 0)} ⇒ tier **${p.severity_tier}**`);
    L.push('');
    L.push(`### IsSeverityActionable: ${p.is_severity_actionable ? '✅ **TRUE**' : '⛔ **FALSE**'}  ·  _(${p.severity_deciding_factor})_`);
    L.push('');
    L.push(severityReason(p));
    L.push('');
    L.push('_Severity is grounded in `ClinicalPhenotypes.SeverityScore` and gated on the SAME mechanism gates as onset (rests-on-confirmed-mechanism ∧ not-spurious) — so a high severity number alone is never actionable on a debunked mechanism._');
    L.push('');
  }

  // ---- THIRD PREDICTION: treatment response (Loop 5) ----
  // Grounded in a MECHANISM MATCH: a Treatment targets a CausalMechanism (FK), and
  // a response is predicted only when that mechanism is a confirmed causal node AND
  // the therapy is effective (Complete/Partial, not adverse). Independent of onset
  // and severity — pred-b is the marquee again (onset FALSE, treatment-response TRUE).
  if (p.treatment_response_deciding_factor != null) {
    L.push('## Treatment-response prediction (third derived prediction)');
    L.push('');
    L.push(`### IsTreatmentResponseActionable: ${p.is_treatment_response_actionable ? '✅ **TRUE**' : '⛔ **FALSE**'}  ·  _(${p.treatment_response_deciding_factor})_`);
    L.push('');
    L.push(treatmentReason(p));
    L.push('');
    L.push('_Treatment response is grounded in a **mechanism match**: the treatment\'s `TargetsMechanism` must be a confirmed causal-architecture node, and the therapy must be effective (`IsEffectiveTreatment`). A drug aimed at a debunked mechanism — or one that was adverse or didn\'t respond — is never predicted to respond._');
    L.push('');
  }

  L.push('---');
  L.push('_This report is the output of the witnessed inference chain. Every value above is derived from raw observations through the DAG in `bootstrap/derivation-spec.md`; none of the conclusions were entered by hand._');
  return L.join('\n');
}
