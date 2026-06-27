import type { Study, Stratum, StratumSummary, TreatmentRanking, Treatment, ModelSummary } from './types';

async function get<T>(path: string): Promise<T> {
  const res = await fetch(path);
  if (!res.ok) throw new Error(`GET ${path} → ${res.status}`);
  return res.json() as Promise<T>;
}

export const api = {
  studies: () => get<Study[]>('/api/studies'),
  study: (id: string) => get<Study>(`/api/studies/${id}`),
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
};
