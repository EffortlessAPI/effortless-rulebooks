import type { Study, Stratum, StratumSummary, TreatmentRanking, Treatment, ModelSummary, AllocationSweepRow, SweepStudySummary, SyntheticPhaseRow, PhaseDiagramSummary, CorpusCatalogSummary, CandidateStudyRow, DomainExpansionTarget, StudyImportTemplateStep, CaseCell, SandboxEvaluateResult, Conclusion, DiscoveryHypothesis, DiscoveryFinding, InvariantCheck } from './types';

async function get<T>(path: string): Promise<T> {
  const res = await fetch(path);
  if (!res.ok) throw new Error(`GET ${path} → ${res.status}`);
  return res.json() as Promise<T>;
}

async function post<T>(path: string, body: unknown): Promise<T> {
  const res = await fetch(path, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({})) as { error?: string };
    throw new Error(err.error ?? `POST ${path} → ${res.status}`);
  }
  return res.json() as Promise<T>;
}

export const api = {
  studies: () => get<Study[]>('/api/studies'),
  study: (id: string) => get<Study>(`/api/studies/${id}`),
  caseCells: (study: string) => get<CaseCell[]>(`/api/case-cells?study=${encodeURIComponent(study)}`),
  sandboxEvaluate: (study: string, cells: Pick<CaseCell, 'stratum_label' | 'treatment_label' | 'successes' | 'cases'>[]) =>
    post<SandboxEvaluateResult>('/api/sandbox/evaluate', { study, cells }),
  strata: (study?: string) =>
    get<Stratum[]>(study ? `/api/strata?study=${study}` : '/api/strata'),
  stratumSummaries: (study?: string) =>
    get<StratumSummary[]>(
      study ? `/api/stratum-summaries?study=${study}` : '/api/stratum-summaries'
    ),
  treatmentRankings: (study?: string) =>
    get<TreatmentRanking[]>(
      study ? `/api/treatment-rankings?study=${study}` : '/api/treatment-rankings'
    ),
  treatments: (study?: string) =>
    get<Treatment[]>(study ? `/api/treatments?study=${study}` : '/api/treatments'),
  modelSummary: () => get<ModelSummary>('/api/model-summary'),
  sweep: (study?: string) =>
    get<AllocationSweepRow[]>(study ? `/api/sweep?study=${study}` : '/api/sweep'),
  sweepSummary: () => get<SweepStudySummary[]>('/api/sweep-summary'),
  syntheticPhase: () => get<SyntheticPhaseRow[]>('/api/synthetic-phase'),
  phaseDiagramSummary: () => get<PhaseDiagramSummary>('/api/phase-diagram-summary'),
  corpusCatalogSummary: () => get<CorpusCatalogSummary>('/api/corpus-catalog-summary'),
  candidateStudyCatalog: () => get<CandidateStudyRow[]>('/api/candidate-study-catalog'),
  domainExpansionTargets: () => get<DomainExpansionTarget[]>('/api/domain-expansion-targets'),
  studyImportTemplate: () => get<StudyImportTemplateStep[]>('/api/study-import-template'),
  conclusions: () => get<Conclusion[]>('/api/conclusions'),
  discoveryHypotheses: () => get<DiscoveryHypothesis[]>('/api/discovery-hypotheses'),
  discoveryFindings: () => get<DiscoveryFinding[]>('/api/discovery-findings'),
  invariantChecks: () => get<InvariantCheck[]>('/api/invariant-checks'),
};
