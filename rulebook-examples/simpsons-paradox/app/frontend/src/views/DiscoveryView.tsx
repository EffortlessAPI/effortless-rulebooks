import { useEffect, useMemo, useState } from 'react';
import { api } from '../api';
import { Cell } from '../components/dag-display';
import type {
  DiscoveryFinding,
  DiscoveryHypothesis,
  ModelSummary,
  TreatmentRanking,
} from '../types';
import { formatObservedMetric } from '../../../shared/epistemic-framing';
import './Conclusions.css';

function flipMode(r: TreatmentRanking): string {
  if (r.is_sign_flip) return 'manifest';
  if (r.is_latent_only_flip) return 'latent-only';
  return 'stable';
}

export function DiscoveryView() {
  const [hypotheses, setHypotheses] = useState<DiscoveryHypothesis[]>([]);
  const [findings, setFindings] = useState<DiscoveryFinding[]>([]);
  const [rankings, setRankings] = useState<TreatmentRanking[]>([]);
  const [summary, setSummary] = useState<ModelSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [roleFilter, setRoleFilter] = useState<string>('causal-sweep');

  useEffect(() => {
    Promise.all([
      api.discoveryHypotheses(),
      api.discoveryFindings(),
      api.treatmentRankings(),
      api.modelSummary(),
    ])
      .then(([h, f, r, s]) => {
        setHypotheses(h);
        setFindings(f);
        setRankings(r);
        setSummary(s);
      })
      .finally(() => setLoading(false));
  }, []);

  const findingsByHypothesis = useMemo(() => {
    const map = new Map<string, DiscoveryFinding>();
    findings.forEach(f => map.set(f.hypothesis_id, f));
    return map;
  }, [findings]);

  const loop62Hypotheses = hypotheses.filter(h => h.registered_in_loop === 'loop-62');

  const filteredStudies = useMemo(() => {
    if (roleFilter === 'all') return rankings;
    if (roleFilter === 'confounder') {
      return rankings.filter(r => r.stratum_causal_role === 'confounder');
    }
    if (roleFilter === 'collider-selection') {
      return rankings.filter(r =>
        r.stratum_causal_role === 'collider' || r.stratum_causal_role === 'selection',
      );
    }
    return rankings.filter(r =>
      r.stratum_causal_role === 'confounder'
      || r.stratum_causal_role === 'collider'
      || r.stratum_causal_role === 'selection',
    );
  }, [rankings, roleFilter]);

  if (loading) return <div className="loading">Loading discovery research…</div>;

  return (
    <div className="conclusions-page">
      <h1 className="page-title">Discovery Research</h1>
      <p className="page-desc">
        Pre-registered <code>DiscoveryHypotheses</code> tested against live{' '}
        <code>vw_*</code> views. Loop-62 research sweep: does{' '}
        <code>StratumCausalRole</code> predict manifest sign-flips vs latent-only sweep
        flips?
      </p>

      <div className="conclusions-stats">
        <div className="card stat-card">
          <h3>Confounder sign-flips</h3>
          <div className="stat-big">
            <Cell table="ModelSummary" col="confounder_sign_flip_count">
              {summary?.confounder_sign_flip_count ?? '—'}
            </Cell>
          </div>
          <div className="stat-caption">manifest (IsSignFlip)</div>
        </div>
        <div className="card stat-card">
          <h3>Confounder latent-only</h3>
          <div className="stat-big">
            <Cell table="ModelSummary" col="confounder_latent_only_count">
              {summary?.confounder_latent_only_count ?? '—'}
            </Cell>
          </div>
          <div className="stat-caption">Type-D sweep sensitivity (not H-causal-manifest criterion)</div>
        </div>
        <div className="card stat-card">
          <h3>Collider/selection</h3>
          <div className="stat-big">
            <Cell table="ModelSummary" col="collider_selection_count">
              {summary?.collider_selection_count ?? '—'}
            </Cell>
          </div>
          <div className="stat-caption">
            manifest{' '}
            <Cell table="ModelSummary" col="collider_selection_manifest_count">
              {summary?.collider_selection_manifest_count ?? '—'}
            </Cell>
            {' · '}
            latent{' '}
            <Cell table="ModelSummary" col="collider_selection_latent_only_count">
              {summary?.collider_selection_latent_only_count ?? '—'}
            </Cell>
          </div>
        </div>
      </div>

      <section className="card">
        <h2>Loop-62 hypotheses</h2>
        <div className="discovery-grid">
          {loop62Hypotheses.map(h => {
            const f = findingsByHypothesis.get(h.hypothesis_id);
            const pass = f?.is_confirmed === true;
            return (
              <div
                key={h.hypothesis_id}
                className={`discovery-card ${pass ? 'discovery-pass' : 'discovery-fail'}`}
              >
                <div className="discovery-card-head">
                  <span className="discovery-id">{h.hypothesis_id}</span>
                  <span className={`badge ${pass ? 'badge-type-c' : 'badge-reversal'}`}>
                    {pass ? 'PASS' : 'FAIL'}
                  </span>
                </div>
                <p className="discovery-statement">{h.statement}</p>
                <div className="discovery-meta">
                  <span>Expected: {h.expected_outcome}</span>
                  {f?.observed_metric && (
                    <span>
                      Observed:{' '}
                      <Cell table="DiscoveryFindings" col="observed_metric">
                        {formatObservedMetric(f.observed_metric)}
                      </Cell>
                    </span>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      </section>

      <section className="card">
        <div className="conclusions-list-head">
          <h2>Study breakdown by causal role</h2>
          <select
            className="filter-select"
            value={roleFilter}
            onChange={e => setRoleFilter(e.target.value)}
          >
            <option value="causal-sweep">Confounder + collider/selection</option>
            <option value="confounder">Confounder only</option>
            <option value="collider-selection">Collider / selection only</option>
            <option value="all">All studies</option>
          </select>
        </div>
        <div className="discovery-table-wrap">
          <table className="discovery-table">
            <thead>
              <tr>
                <th>Study</th>
                <th>Causal role</th>
                <th>Flip mode</th>
                <th>Type</th>
                <th>IsSignFlip</th>
                <th>Crosses zero</th>
              </tr>
            </thead>
            <tbody>
              {filteredStudies.map(r => (
                <tr key={r.treatment_ranking_id}>
                  <td>{r.study}</td>
                  <td>{r.stratum_causal_role ?? '—'}</td>
                  <td>{flipMode(r)}</td>
                  <td>{r.distortion_type}</td>
                  <td>{r.is_sign_flip ? 'yes' : 'no'}</td>
                  <td>{r.pooled_gap_crosses_zero ? 'yes' : 'no'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}
