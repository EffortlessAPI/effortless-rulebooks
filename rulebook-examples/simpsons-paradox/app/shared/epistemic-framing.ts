/** Epistemic tier labels — shared by PDF export and /conclusions UI. */

export const CONSISTENCY_CHECK_HYPOTHESES = new Set(['H-purity']);

export const SWEEP_CONTRACT =
  'Allocation sweep: 10 steps per study, f ∈ [0.05, 0.95] (SweepStudyConfig). ' +
  'Latent flip = Type D at observed allocation + PooledGapCrossesZero somewhere in that range — ' +
  'not necessarily at observed allocation.';

export const SCOPE_BOUNDARY_TITLE = 'Scope boundary (conc-09)';

export const SCOPE_BOUNDARY_TEXT =
  'The instrument classifies allocation geometry only. Causal interpretation is external, ' +
  'gated by AdjustmentAppropriate. Berkeley: highest AllocationDistortion (0.193) but ' +
  'CausalRole=contested — geometry and confounding are not the same thing.';

export const LIMITS_TEXT =
  'Theorem conclusions follow from definitions (regression checks for the transpiler). ' +
  'Domain conclusions and corpus discovery findings are statistics on a curated convenience sample — ' +
  'directional, not inferential. See conc-14 before treating cross-domain patterns as laws.';

export const CATEGORY_ORDER = [
  'theorem',
  'instrument',
  'taxonomy',
  'methodology',
  'open-question',
  'domain',
] as const;

export const CATEGORY_PDF_LABEL: Record<string, string> = {
  theorem: 'Proved (by construction)',
  instrument: 'Established in instrument',
  taxonomy: 'Taxonomy',
  methodology: 'Methodology',
  'open-question': 'Scope / open question',
  domain: 'Observed in this corpus',
};

export const DOMAIN_CORPUS_CAVEATS: Record<string, string> = {
  'conc-15-latent-flip-potential':
    'Sensitivity over sweep range f ∈ [0.05, 0.95]; not a claim about realistic reallocation alone.',
  'conc-17-economics-flip-free':
    'This corpus only (n=8 economics studies); see conc-14 for inferential limits.',
};

export function isConsistencyCheck(hypothesisId: string): boolean {
  return CONSISTENCY_CHECK_HYPOTHESES.has(hypothesisId);
}

export function conclusionPdfLabel(category: string | null | undefined): string {
  if (!category) return '(no category)';
  return CATEGORY_PDF_LABEL[category] ?? category.replace(/-/g, ' ');
}

export function domainCaveat(conclusionId: string): string | undefined {
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
  instrument: number;
  corpus: number;
  total: number;
}

export function tierConclusionCounts(
  rows: Array<{ category: string | null; status: string | null }>,
): ConclusionTierCounts {
  const witnessed = rows.filter(r => r.status === 'witnessed');
  const proved = witnessed.filter(r => r.category === 'theorem').length;
  const instrument = witnessed.filter(
    r =>
      r.category === 'instrument' ||
      r.category === 'taxonomy' ||
      r.category === 'methodology' ||
      r.category === 'open-question',
  ).length;
  const corpus = witnessed.filter(r => r.category === 'domain').length;
  return { proved, instrument, corpus, total: witnessed.length };
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
