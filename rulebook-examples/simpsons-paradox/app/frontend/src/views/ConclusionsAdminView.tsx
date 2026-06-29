import { useEffect, useMemo, useState } from 'react';
import { api } from '../api';
import type {
  Conclusion,
  DiscoveryFinding,
  DiscoveryHypothesis,
  InvariantCheck,
  ModelSummary,
} from '../types';
import './Conclusions.css';

const CATEGORY_ORDER = [
  'theorem',
  'instrument',
  'domain',
  'methodology',
  'taxonomy',
  'open-question',
] as const;

const STATUS_COLORS: Record<string, string> = {
  witnessed: 'badge-type-c',
  planned: 'badge-sign-flip',
  open: 'badge-reversal',
};

function categoryLabel(cat: string): string {
  return cat.replace(/-/g, ' ');
}

export function ConclusionsAdminView() {
  const [conclusions, setConclusions] = useState<Conclusion[]>([]);
  const [hypotheses, setHypotheses] = useState<DiscoveryHypothesis[]>([]);
  const [findings, setFindings] = useState<DiscoveryFinding[]>([]);
  const [invariants, setInvariants] = useState<InvariantCheck[]>([]);
  const [summary, setSummary] = useState<ModelSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [categoryFilter, setCategoryFilter] = useState<string>('all');
  const [selectedId, setSelectedId] = useState<string | null>(null);

  useEffect(() => {
    Promise.all([
      api.conclusions(),
      api.discoveryHypotheses(),
      api.discoveryFindings(),
      api.invariantChecks(),
      api.modelSummary(),
    ])
      .then(([c, h, f, i, s]) => {
        setConclusions(c);
        setHypotheses(h);
        setFindings(f);
        setInvariants(i);
        setSummary(s);
        setSelectedId(prev => prev ?? c[c.length - 1]?.conclusion_id ?? null);
      })
      .finally(() => setLoading(false));
  }, []);

  const filtered = useMemo(() => {
    const rows =
      categoryFilter === 'all'
        ? conclusions
        : conclusions.filter(c => c.category === categoryFilter);
    return [...rows].sort((a, b) => {
      const ai = CATEGORY_ORDER.indexOf(a.category as (typeof CATEGORY_ORDER)[number]);
      const bi = CATEGORY_ORDER.indexOf(b.category as (typeof CATEGORY_ORDER)[number]);
      if (ai !== bi) return (ai === -1 ? 99 : ai) - (bi === -1 ? 99 : bi);
      return a.conclusion_id.localeCompare(b.conclusion_id);
    });
  }, [conclusions, categoryFilter]);

  const selected = conclusions.find(c => c.conclusion_id === selectedId) ?? null;
  const protectingInvariants = selected
    ? invariants.filter(i => i.protects_conclusion === selected.conclusion_id)
    : [];

  const findingsByHypothesis = useMemo(() => {
    const map = new Map<string, DiscoveryFinding>();
    findings.forEach(f => map.set(f.hypothesis_id, f));
    return map;
  }, [findings]);

  const witnessedCount = conclusions.filter(c => c.status === 'witnessed').length;

  if (loading) return <div className="loading">Loading conclusions…</div>;

  return (
    <div className="conclusions-page">
      <h1 className="page-title">Conclusions &amp; Findings</h1>
      <p className="page-desc">
        Formal epistemic claims tracked in the rulebook <code>Conclusions</code> table — findings
        witnessed across Leopold loops. Loop-61 adds pre-registered{' '}
        <code>DiscoveryHypotheses</code> / <code>DiscoveryFindings</code> with live PASS/FAIL
        from the corpus. SSoT is the rulebook JSON; this surface reads <code>vw_*</code> views only.
      </p>

      <div className="conclusions-stats three-col">
        <div className="card stat-card">
          <h3>Conclusions</h3>
          <div className="stat-big">{witnessedCount}<span> / {conclusions.length}</span></div>
          <div className="stat-caption">witnessed</div>
        </div>
        <div className="card stat-card">
          <h3>Discovery (loop-61)</h3>
          <div className="stat-big">
            {findings.filter(f => f.is_confirmed).length}
            <span> / {findings.length}</span>
          </div>
          <div className="stat-caption">hypotheses confirmed</div>
        </div>
        <div className="card stat-card">
          <h3>Latent Type-D</h3>
          <div className="stat-big">
            {summary?.latent_type_d_count != null
              ? `${Math.round(Number(summary.latent_type_d_fraction) * 1000) / 10}%`
              : '—'}
          </div>
          <div className="stat-caption">
            {summary?.latent_type_d_count ?? '—'} / {summary?.type_d_count ?? '—'} SAFE studies flip under sweep
          </div>
        </div>
      </div>

      <section className="card">
        <h2>Pre-registered discovery (loop-61)</h2>
        <p className="section-hint">
          Hypotheses registered before querying the 90+ study corpus. Each finding is computed live from ModelSummary.
        </p>
        <div className="discovery-grid">
          {hypotheses.map(h => {
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
                  {f?.observed_metric && <span>Observed: {f.observed_metric}</span>}
                </div>
              </div>
            );
          })}
        </div>
      </section>

      <div className="conclusions-layout">
        <section className="card conclusions-list-panel">
          <div className="conclusions-list-head">
            <h2>All conclusions</h2>
            <select
              className="filter-select"
              value={categoryFilter}
              onChange={e => setCategoryFilter(e.target.value)}
            >
              <option value="all">All categories</option>
              {CATEGORY_ORDER.map(cat => (
                <option key={cat} value={cat}>
                  {categoryLabel(cat)}
                </option>
              ))}
            </select>
          </div>
          <ul className="conclusions-list">
            {filtered.map(c => (
              <li key={c.conclusion_id}>
                <button
                  type="button"
                  className={`conclusion-row ${selectedId === c.conclusion_id ? 'selected' : ''}`}
                  onClick={() => setSelectedId(c.conclusion_id)}
                >
                  <span className={`badge ${STATUS_COLORS[c.status] ?? 'badge-neutral'}`}>
                    {c.status}
                  </span>
                  <span className="conclusion-cat">{categoryLabel(c.category)}</span>
                  <span className="conclusion-title">{c.title}</span>
                  {c.invariant_protecting_count != null && c.invariant_protecting_count > 0 && (
                    <span className="conclusion-inv-count">{c.invariant_protecting_count} inv</span>
                  )}
                </button>
              </li>
            ))}
          </ul>
        </section>

        <section className="card conclusion-detail-panel">
          {selected ? (
            <>
              <div className="detail-header">
                <code className="detail-id">{selected.conclusion_id}</code>
                <span className={`badge ${STATUS_COLORS[selected.status] ?? 'badge-neutral'}`}>
                  {selected.status}
                </span>
              </div>
              <h2 className="detail-title">{selected.title}</h2>
              <div className="detail-meta">
                <span>Category: {categoryLabel(selected.category)}</span>
                {selected.witnessed_in_loop && (
                  <span>Witnessed in: {selected.witnessed_in_loop}</span>
                )}
                {selected.target_loop && <span>Target loop: {selected.target_loop}</span>}
              </div>
              {(selected.witnessed_in_loop_commit_short || selected.witnessed_in_loop_git_tag) && (
                <div className="replay-block">
                  <h3>Discovery replay</h3>
                  <p className="replay-hint">
                    Check out the landing commit for this conclusion&apos;s loop, rebuild, and the
                    instrument state matches what was witnessed when the conclusion landed.
                  </p>
                  {selected.witnessed_in_loop_git_tag && (
                    <div>
                      <strong>Tag:</strong>{' '}
                      <code>git checkout {selected.witnessed_in_loop_git_tag}</code>
                    </div>
                  )}
                  {selected.witnessed_in_loop_commit_short && (
                    <div>
                      <strong>Commit:</strong>{' '}
                      <code>{selected.witnessed_in_loop_commit_short}</code>
                      {selected.witnessed_in_loop_commit_date && (
                        <span> ({selected.witnessed_in_loop_commit_date})</span>
                      )}
                    </div>
                  )}
                </div>
              )}
              <h3>Evidence</h3>
              <pre className="evidence-block">{selected.evidence ?? '(no evidence string)'}</pre>
              {protectingInvariants.length > 0 && (
                <>
                  <h3>Protecting invariants ({protectingInvariants.length})</h3>
                  <ul className="invariant-list">
                    {protectingInvariants.map(inv => (
                      <li key={inv.invariant_check_id}>
                        <span className={`badge ${inv.is_green ? 'badge-type-c' : 'badge-reversal'}`}>
                          {inv.status_label}
                        </span>
                        <strong>{inv.invariant_check_id}</strong>
                        <span>{inv.natural_language}</span>
                      </li>
                    ))}
                  </ul>
                </>
              )}
              <p className="edit-hint">
                To add or edit conclusions, update{' '}
                <code>effortless-rulebook/simpsons-paradox-rulebook.json</code> →{' '}
                <code>Conclusions</code> table, then <code>effortless build</code>.
              </p>
            </>
          ) : (
            <p className="loading">Select a conclusion</p>
          )}
        </section>
      </div>
    </div>
  );
}
