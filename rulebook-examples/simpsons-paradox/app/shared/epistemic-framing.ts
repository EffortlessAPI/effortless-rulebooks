/** Epistemic tier labels — shared by PDF export and /conclusions UI. */

export const CONSISTENCY_CHECK_HYPOTHESES = new Set(['H-purity']);

/** Corpus hypotheses that PASS only because their filtered universe is empty by construction
 *  (control studies were selected for absence of paradox, so they were never encoded with
 *  paired treatment arms — zero TreatmentRankings rows means zero opportunity to sign-flip).
 *  observed_metric already self-labels these "vacuous" at the SQL layer (see 03c). Excluding
 *  them from the corpus-hypothesis confirmed/total ratio stops a true-by-construction PASS from
 *  inflating a headline confirmation count that is supposed to mean "survived a real test." */
export const VACUOUS_HYPOTHESES = new Set([
  'H-control-type-d-predominance',
  'H-control-no-manifest-flip',
  'H-control-safety-corridor',
]);

export function isVacuousHypothesis(hypothesisId: string): boolean {
  return VACUOUS_HYPOTHESES.has(hypothesisId);
}

/** Corpus-pattern-superseded hypotheses retired from the live DAG at loop-79 after loop-78
 *  witnessed them FAIL at N=238 (expansion wave 3). Removed from DiscoveryHypotheses/
 *  DiscoveryFindings so the instrument doesn't carry dead vocabulary forward, but their FAIL
 *  evidence is preserved verbatim in conc-32's Evidence field — this is the permanent record,
 *  not a live query. A report that silently drops these six from view (rather than showing
 *  historical/FAIL) is indistinguishable from cherry-picking, even though nothing was hidden
 *  from the rulebook itself. Kept as a fixed historical list, not re-derived each build, because
 *  loop-79 already closed the book on it; if a future loop supersedes a NEW hypothesis, add it
 *  here alongside its own conc-* evidence row. */
export interface SupersededHypothesis {
  hypothesisId: string;
  observedMetric: string;
}

export const SUPERSEDED_HYPOTHESES: SupersededHypothesis[] = [
  { hypothesisId: 'H-econ-zero', observedMetric: 'flips=10 (was 0 pre-expansion)' },
  { hypothesisId: 'H-domain-dist', observedMetric: 'epi<edu (direction reversed)' },
  { hypothesisId: 'H-catalog-flip-prediction', observedMetric: 'flipPred=54.1% (was 25.4%)' },
  { hypothesisId: 'H-domain-flip-geometry-controlled', observedMetric: 'econHighImbFlips=10 (was 0)' },
  { hypothesisId: 'H-econ-encoding-selection', observedMetric: 'mismatch=50% (was 100%)' },
  { hypothesisId: 'H-domain-profiles-stable', observedMetric: 'econFlipRate=25.6% (was 0%)' },
];

export const SUPERSEDED_HYPOTHESES_CONCLUSION_ID = 'conc-32-expansion-wave-3-supersession';

export const SWEEP_CONTRACT =
  'Allocation sweep: 10 steps per study, f ∈ [0.05, 0.95] (SweepStudyConfig). ' +
  'Latent flip = Type D at observed allocation + PooledGapCrossesZero somewhere in that range — ' +
  'not necessarily at observed allocation.';

export const SCOPE_BOUNDARY_TITLE = 'Scope boundary (conc-09)';

export const SCOPE_BOUNDARY_TEXT =
  'This rulebook is a high-fidelity measurement instrument, not an oracle of truth. It classifies ' +
  'allocation geometry precisely and repeatably — sign flips, distortion ratios, signal purity — the ' +
  'same way a thermometer measures temperature. It does not adjudicate causal truth. Causal ' +
  'interpretation is external, gated by AdjustmentAppropriate, and rests on CausalRole, the one ' +
  'hand/LLM-encoded field in the DAG. Berkeley: highest AllocationDistortion (0.193) but ' +
  'CausalRole=contested — geometry and confounding are not the same thing, and the instrument only ' +
  'measures the former.';

export const LIMITS_TEXT =
  'Theorem conclusions follow from definitions (regression checks for the transpiler). ' +
  'Domain conclusions and corpus discovery findings are statistics on a curated convenience sample — ' +
  'directional, not inferential. See conc-14 before treating cross-domain patterns as laws.';

export const INVARIANT_DEFINITIONS_TITLE = 'How to read "InvariantChecks" (read before the pass count)';

export const INVARIANT_DEFINITIONS_TEXT =
  'This instrument is a measurement device, not an oracle: it classifies allocation geometry ' +
  'precisely and repeatably, the way a thermometer classifies temperature — it does not adjudicate ' +
  'whether a study’s causal story is correct. "Invariant" in this rulebook means "the transpiler ' +
  'output is self-consistent with its own definitions," not "an independent empirical hypothesis ' +
  'survived a test." Three different things get called an invariant here, and conflating them is the ' +
  'single easiest way to overstate this corpus:';

export const INVARIANT_DEFINITION_TAUTOLOGICAL =
  'Definitional (cannot fail by construction). DistortionType, IsSignFlip, AllocationDirection, and ' +
  'SignalPurity are all calculated from the same three numbers (WeightedStratumGapSum, SignedPooledGap, ' +
  'CorrectedGap, AllocationDistortion). An invariant like "DistortionType ∈ {A,B} → IsSignFlip=TRUE" ' +
  'restates one algebraic fact in a second vocabulary — it is a regression check that the transpiler ' +
  'did not corrupt a formula, not a finding about the world. Most of the 32 rows in InvariantChecks are ' +
  'this kind.';

export const INVARIANT_DEFINITION_JUDGMENT =
  'Judgment-crossing (the closest thing to a substantive check). CausalRole on StratumVariables ' +
  '(confounder / mediator / collider / selection / contested / unknown) is the one hand/LLM-encoded ' +
  'field in the DAG — schema type "raw," not derived from geometry. Invariants that reference ' +
  'StratumCausalRole, ConfounderIdentity, or IsParadoxExplained (e.g. inv-explained-sign-flip-confounder, ' +
  'inv-collider-no-manifest) are checking that the geometric result is consistent with that human ' +
  'judgment call — the only place a real counterexample could show up. The rulebook does not currently ' +
  'record who made each CausalRole call or by what protocol; treat these classifications as reviewed ' +
  'labels, not as independently verified ground truth.';

export const INVARIANT_DEFINITION_VACUOUS =
  'Vacuous (PassCount=0). A handful of invariants (e.g. inv-type-a-unanimous, ' +
  'inv-confounder-signflip-explained, inv-collider-no-manifest) currently have zero rows in their ' +
  'filtered universe. "0 pass / 0 fail" means no study in this corpus falls into that category yet — ' +
  'it is true the way "all unicorns are blue" is true, and proves nothing until a qualifying study exists.';

/** Theorem-category conclusions whose claim is conditional on CausalRole / ConfounderIdentity /
 *  StratumCausalRole being correctly annotated — the one hand/LLM-encoded, unaudited-protocol field
 *  in the DAG. These are true given a correct annotation, not true by algebra alone, so they must
 *  not sit in the same "proved by construction" bucket as pure-geometry theorems (e.g. conc-12,
 *  conc-20, conc-28, conc-36, which reference only IsSignFlip/SignalPurity/CorrectedGap/ArmSizeRatio/
 *  MaxStratumImbalance — no causal-role dependency). conc-30's own title says "given correct
 *  causal-role annotation" — that precondition is the reason it belongs here, not in "proved."
 *  Kept as an explicit allowlist so new theorem conclusions default to "proved" and must be
 *  deliberately added here, not the reverse. */
export const CAUSAL_ROLE_CONDITIONAL_THEOREMS = new Set([
  'conc-29-explained-confounder-theorem',
  'conc-30-collider-no-manifest-theorem',
  'conc-34-latent-predominant-family',
  'conc-35-unexplained-identity-sink-theorem',
  'conc-37-purity-inversion-theorem',
]);

export function isCausalRoleConditionalTheorem(conclusionId: string): boolean {
  return CAUSAL_ROLE_CONDITIONAL_THEOREMS.has(conclusionId);
}

export const CATEGORY_ORDER = [
  'corpus-theorem',
  'instrument',
  'taxonomy',
  'methodology',
  'open-question',
  'domain',
] as const;

export const CATEGORY_PDF_LABEL: Record<string, string> = {
  'corpus-theorem': 'Proved (by construction)',
  instrument: 'Established in instrument',
  taxonomy: 'Taxonomy',
  methodology: 'Methodology',
  'open-question': 'Scope / open question',
  domain: 'Observed in this corpus',
};

export const CONDITIONAL_THEOREM_PDF_LABEL =
  'Proved, conditional on CausalRole annotation (not pure algebra)';

export const DOMAIN_CORPUS_CAVEATS: Record<string, string> = {
  'conc-15-latent-flip-potential':
    'Sensitivity over sweep range f ∈ [0.05, 0.95]; not a claim about realistic reallocation alone.',
  'conc-17-economics-flip-free':
    'This corpus only (n=8 economics studies); see conc-14 for inferential limits.',
};

export function isConsistencyCheck(hypothesisId: string): boolean {
  return CONSISTENCY_CHECK_HYPOTHESES.has(hypothesisId);
}

/** Invariant IDs that cross into the one hand/LLM-encoded field (CausalRole) rather than
 *  restating pure geometry. Kept as an explicit allowlist so new invariants default to
 *  "definitional" and must be deliberately added here, not the reverse. */
export const JUDGMENT_CROSSING_INVARIANTS = new Set([
  'inv-explained-sign-flip-confounder',
  'inv-confounder-signflip-explained',
  'inv-collider-no-manifest',
  'inv-unexplained-identity-sink',
  'inv-collider-purity-dominance',
]);

export type InvariantKind = 'judgment' | 'vacuous' | 'definitional';

export function classifyInvariant(row: {
  invariant_check_id: string;
  pass_count: number;
  fail_count: number;
}): InvariantKind {
  if (JUDGMENT_CROSSING_INVARIANTS.has(row.invariant_check_id)) return 'judgment';
  if (Number(row.pass_count) === 0 && Number(row.fail_count) === 0) return 'vacuous';
  return 'definitional';
}

export interface InvariantKindCounts {
  definitional: number;
  judgment: number;
  vacuous: number;
  total: number;
}

export function invariantKindCounts(
  rows: Array<{ invariant_check_id: string; pass_count: number; fail_count: number }>,
): InvariantKindCounts {
  const counts = { definitional: 0, judgment: 0, vacuous: 0, total: rows.length };
  for (const r of rows) counts[classifyInvariant(r)]++;
  return counts;
}

export function conclusionPdfLabel(
  category: string | null | undefined,
  conclusionId?: string,
): string {
  if (conclusionId && isCausalRoleConditionalTheorem(conclusionId)) {
    return CONDITIONAL_THEOREM_PDF_LABEL;
  }
  if (!category) return '(no category)';
  return CATEGORY_PDF_LABEL[category] ?? category.replace(/-/g, ' ');
}

/** Theorems whose claim follows directly from how SignalPurity/CorrectedGap/AllocationDistortion
 *  are DEFINED (see INVARIANT_DEFINITION_TAUTOLOGICAL) — not from CausalRole, and not an
 *  independent empirical regularity. Corpus-scale testing of these is a transpiler regression
 *  check confirming the algebra wasn't broken, not new evidence for the claim itself. */
export const PURE_ALGEBRA_DEFINITIONAL_THEOREMS = new Set([
  'conc-12-signal-purity-theorem',
  'conc-20-signal-purity-ceiling',
  'conc-28-corrected-gap-invariance-theorem',
]);

export function domainCaveat(conclusionId: string): string | undefined {
  if (isCausalRoleConditionalTheorem(conclusionId)) {
    return 'Conditional on CausalRole being correctly annotated — the one hand/LLM-encoded field ' +
      'in the DAG, with no recorded protocol for who assigned each label or how. True given a ' +
      'correct annotation; not true by algebra alone. See "How to read InvariantChecks."';
  }
  if (PURE_ALGEBRA_DEFINITIONAL_THEOREMS.has(conclusionId)) {
    return 'Follows directly from how SignalPurity/CorrectedGap/AllocationDistortion are defined ' +
      '— corpus-scale testing here is a regression check that the transpiler did not corrupt the ' +
      'formula, not independent empirical confirmation of a separate discovery.';
  }
  if (conclusionId.startsWith('conc-17') || conclusionId === 'conc-17-economics-flip-free') {
    return DOMAIN_CORPUS_CAVEATS['conc-17-economics-flip-free'];
  }
  if (conclusionId === 'conc-15-latent-flip-potential') {
    return DOMAIN_CORPUS_CAVEATS['conc-15-latent-flip-potential'];
  }
  if (conclusionId.includes('domain') || conclusionId.endsWith('-flip-free')) {
    return 'Corpus-specific pattern; see conc-14.';
  }
  return undefined;
}

export interface ConclusionTierCounts {
  proved: number;
  provedConditional: number;
  instrument: number;
  corpus: number;
  total: number;
}

export function tierConclusionCounts(
  rows: Array<{ conclusion_id: string; category: string | null; status: string | null }>,
): ConclusionTierCounts {
  const witnessed = rows.filter(r => r.status === 'witnessed');
  const theorems = witnessed.filter(r => r.category === 'corpus-theorem');
  const provedConditional = theorems.filter(r =>
    isCausalRoleConditionalTheorem(r.conclusion_id),
  ).length;
  const proved = theorems.length - provedConditional;
  const instrument = witnessed.filter(
    r =>
      r.category === 'instrument' ||
      r.category === 'taxonomy' ||
      r.category === 'methodology' ||
      r.category === 'open-question',
  ).length;
  const corpus = witnessed.filter(r => r.category === 'domain').length;
  return { proved, provedConditional, instrument, corpus, total: witnessed.length };
}

/** Round numeric literals in observed_metric strings (e.g. epi=0.049454… → epi=0.049). */
export function formatObservedMetric(raw: string | null | undefined, dp = 3): string {
  if (!raw) return '—';
  return raw.replace(/(-?\d+\.\d+)/g, m => {
    const n = Number(m);
    if (!Number.isFinite(n)) return m;
    return n.toFixed(dp);
  });
}
