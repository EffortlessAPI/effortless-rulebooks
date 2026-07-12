import { useEffect, useMemo, useRef, useState } from 'react';
import { Chart } from 'chart.js/auto';
import { api } from '../api';
import { ViewDagScan } from '../components/DagValue';
import { Cell, TrCell } from '../components/dag-display';
import type {
  Conclusion,
  DiscoveryFinding,
  DiscoveryHypothesis,
  InvariantCheck,
  ModelSummary,
  Study,
  TreatmentRanking,
} from '../types';
import {
  CATEGORY_ORDER,
  LIMITS_TEXT,
  SCOPE_BOUNDARY_TEXT,
  SCOPE_BOUNDARY_TITLE,
  SWEEP_CONTRACT,
  conclusionPdfLabel,
  domainCaveat,
  formatObservedMetric,
  isConsistencyCheck,
  tierConclusionCounts,
} from '../../../shared/epistemic-framing';
import { buildInvisibleFragileChart, buildPurityChart, buildTypeDFragilityChart, TYPE_COLORS } from './overviewCharts';
import './Conclusions.css';
import './Overview.css';

const STATUS_COLORS: Record<string, string> = {
  witnessed: 'badge-type-c',
  planned: 'badge-sign-flip',
  open: 'badge-reversal',
};

function categoryLabel(cat: string, conclusionId?: string): string {
  return conclusionPdfLabel(cat, conclusionId);
}

function DiscoveryCard({
  h,
  f,
  tierLabel,
}: {
  h: DiscoveryHypothesis;
  f: DiscoveryFinding | undefined;
  tierLabel: string;
}) {
  const pass = f?.is_confirmed === true;
  return (
    <div className={`discovery-card ${pass ? 'discovery-pass' : 'discovery-fail'}`}>
      <div className="discovery-card-head">
        <span className="discovery-id">{h.hypothesis_id}</span>
        <span className="discovery-tier">{tierLabel}</span>
        <span className={`badge ${pass ? 'badge-type-c' : 'badge-reversal'}`}>
          <Cell table="DiscoveryFindings" col="is_confirmed">{pass ? 'PASS' : 'FAIL'}</Cell>
        </span>
      </div>
      <p className="discovery-statement">{h.statement}</p>
      <div className="discovery-meta">
        <span>Expected: {h.expected_outcome}</span>
        {f?.observed_metric && (
          <span>Observed: <Cell table="DiscoveryFindings" col="observed_metric">{formatObservedMetric(f.observed_metric)}</Cell></span>
        )}
      </div>
    </div>
  );
}

const CAUSAL_ROLES = ['confounder', 'collider', 'selection', 'mediator', 'contested', 'unknown'] as const;

export function ConclusionsAdminView() {
  const [conclusions, setConclusions] = useState<Conclusion[]>([]);
  const [hypotheses, setHypotheses] = useState<DiscoveryHypothesis[]>([]);
  const [findings, setFindings] = useState<DiscoveryFinding[]>([]);
  const [invariants, setInvariants] = useState<InvariantCheck[]>([]);
  const [summary, setSummary] = useState<ModelSummary | null>(null);
  const [studies, setStudies] = useState<Study[]>([]);
  const [rankings, setRankings] = useState<TreatmentRanking[]>([]);
  const [loading, setLoading] = useState(true);
  const [categoryFilter, setCategoryFilter] = useState<string>('all');
  const [selectedId, setSelectedId] = useState<string | null>(null);

  const typeDFragilityRef = useRef<HTMLCanvasElement>(null);
  const purityRef = useRef<HTMLCanvasElement>(null);
  const invisibleFragileRef = useRef<HTMLCanvasElement>(null);
  const notableChartsRef = useRef<(Chart | null)[]>([]);

  useEffect(() => {
    Promise.all([
      api.conclusions(),
      api.discoveryHypotheses(),
      api.discoveryFindings(),
      api.invariantChecks(),
      api.modelSummary(),
      api.studies(),
      api.treatmentRankings(),
    ])
      .then(([c, h, f, i, s, st, tr]) => {
        setConclusions(c);
        setHypotheses(h);
        setFindings(f);
        setInvariants(i);
        setSummary(s);
        setStudies(st);
        setRankings(tr);
        setSelectedId(prev => prev ?? c[c.length - 1]?.conclusion_id ?? null);
      })
      .finally(() => setLoading(false));
  }, []);

  const studyById = useMemo(
    () => Object.fromEntries(studies.map(s => [s.study_id, s])),
    [studies],
  );

  useEffect(() => {
    if (!rankings.length) return;
    notableChartsRef.current.forEach(c => c?.destroy());
    const built: Chart[] = [];
    if (typeDFragilityRef.current) built.push(buildTypeDFragilityChart(typeDFragilityRef.current, rankings));
    if (purityRef.current) built.push(buildPurityChart(purityRef.current, rankings, studyById));
    if (invisibleFragileRef.current) built.push(buildInvisibleFragileChart(invisibleFragileRef.current, rankings, studyById));
    notableChartsRef.current = built;
    return () => { notableChartsRef.current.forEach(c => c?.destroy()); notableChartsRef.current = []; };
  }, [rankings, studyById]);

  const distortionCounts = useMemo(() => {
    const counts: Record<string, number> = { A: 0, B: 0, 'C-': 0, 'C+': 0, D: 0 };
    rankings.forEach(r => { if (r.distortion_type in counts) counts[r.distortion_type]++; });
    return counts;
  }, [rankings]);

  // Matches ModelSummary.ColliderSelectionCount / ColliderSelectionManifestCount scope:
  // real-world corpus only — synthetic boundary-probe studies (e.g. adversarial theorem
  // stress tests) are excluded from this specific corpus-level claim.
  const causalRoleBreakdown = useMemo(() => {
    const byRole: Record<string, { manifest: number; latent: number; stable: number }> = {};
    CAUSAL_ROLES.forEach(r => { byRole[r] = { manifest: 0, latent: 0, stable: 0 }; });
    rankings.filter(r => r.study_domain !== 'synthetic').forEach(r => {
      const role = r.stratum_causal_role && r.stratum_causal_role in byRole ? r.stratum_causal_role : 'unknown';
      if (r.is_sign_flip) byRole[role].manifest++;
      else if (r.is_latent_only_flip) byRole[role].latent++;
      else byRole[role].stable++;
    });
    return CAUSAL_ROLES.map(role => ({ role, ...byRole[role] })).filter(r => r.manifest + r.latent + r.stable > 0);
  }, [rankings]);

  const effectSizeBuckets = useMemo(() => {
    const buckets = [
      { label: '< 1%', min: 0, max: 0.01 },
      { label: '1–3%', min: 0.01, max: 0.03 },
      { label: '3–5%', min: 0.03, max: 0.05 },
      { label: '5–10%', min: 0.05, max: 0.10 },
      { label: '> 10%', min: 0.10, max: Infinity },
    ];
    const dRows = rankings.filter(r => r.distortion_type === 'D');
    return buckets.map(b => {
      const group = dRows.filter(r => Number(r.pooled_gap) >= b.min && Number(r.pooled_gap) < b.max);
      const fragile = group.filter(r => r.is_sweep_fragile).length;
      return { ...b, n: group.length, fragile, pct: group.length ? fragile / group.length : 0 };
    }).filter(b => b.n > 0);
  }, [rankings]);

  const domainBreakdown = useMemo(() => {
    const byDomain: Record<string, { manifest: number; latent: number; stable: number }> = {};
    rankings.forEach(r => {
      const domain = r.study_domain ?? 'unknown';
      if (!byDomain[domain]) byDomain[domain] = { manifest: 0, latent: 0, stable: 0 };
      if (r.is_sign_flip) byDomain[domain].manifest++;
      else if (r.is_latent_only_flip) byDomain[domain].latent++;
      else byDomain[domain].stable++;
    });
    return Object.entries(byDomain)
      .map(([domain, d]) => ({ domain, ...d, total: d.manifest + d.latent + d.stable }))
      .sort((a, b) => b.total - a.total);
  }, [rankings]);

  const invisibleFragileCount = useMemo(
    () => rankings.filter(r => r.is_sweep_fragile && Number(r.allocation_distortion) === 0).length,
    [rankings],
  );

  const abCompare = useMemo(() => {
    const aRows = rankings.filter(r => r.distortion_type === 'A');
    const bRows = rankings.filter(r => r.distortion_type === 'B');
    const aUnanimous = aRows.filter(r => r.is_stratum_unanimous).length;
    const bUnanimous = bRows.filter(r => r.is_stratum_unanimous).length;
    return { aRows, bRows, aUnanimous, bUnanimous };
  }, [rankings]);

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

  const tiers = tierConclusionCounts(conclusions);

  const consistencyHypotheses = hypotheses.filter(
    h => isConsistencyCheck(h.hypothesis_id) || h.epistemic_tier === 'consistency-check',
  );
  const corpusHypotheses = hypotheses.filter(
    h => !isConsistencyCheck(h.hypothesis_id) && h.epistemic_tier !== 'consistency-check',
  );
  const corpusConfirmed = corpusHypotheses.filter(
    h => findingsByHypothesis.get(h.hypothesis_id)?.is_confirmed === true,
  ).length;
  const consistencyConfirmed = consistencyHypotheses.filter(
    h => findingsByHypothesis.get(h.hypothesis_id)?.is_confirmed === true,
  ).length;

  if (loading) return <div className="loading">Loading conclusions…</div>;

  return (
    <div className="conclusions-page">
      <h1 className="page-title">Conclusions &amp; Findings</h1>
      <p className="page-desc">
        Formal epistemic claims in the rulebook <code>Conclusions</code> table — tiered by what
        kind of claim each row is (proved · instrument · corpus snapshot). Loop-61 adds
        pre-registered <code>DiscoveryHypotheses</code> split into consistency checks vs corpus
        hypotheses. SSoT is the rulebook JSON; this surface reads <code>vw_*</code> views only.
      </p>

      <section className="card scope-card">
        <h2>{SCOPE_BOUNDARY_TITLE}</h2>
        <p className="scope-text">{SCOPE_BOUNDARY_TEXT}</p>
        <p className="scope-text scope-muted">{LIMITS_TEXT}</p>
        <p className="scope-text scope-muted">{SWEEP_CONTRACT}</p>
      </section>

      <div className="conclusions-stats">
        <div className="card stat-card">
          <h3>Proved (theorem)</h3>
          <div className="stat-big">{tiers.proved}</div>
          <div className="stat-caption">by construction — pure algebra</div>
        </div>
        <div className="card stat-card">
          <h3>Proved, conditional</h3>
          <div className="stat-big">{tiers.provedConditional}</div>
          <div className="stat-caption">true given correct CausalRole annotation — not pure algebra</div>
        </div>
        <div className="card stat-card">
          <h3>Instrument &amp; scope</h3>
          <div className="stat-big">{tiers.instrument}</div>
          <div className="stat-caption">instrument · taxonomy · methodology · scope</div>
        </div>
        <div className="card stat-card">
          <h3>Corpus snapshot</h3>
          <div className="stat-big">{tiers.corpus}</div>
          <div className="stat-caption">domain — provisional (see conc-14)</div>
        </div>
        <div className="card stat-card">
          <h3>Corpus hypotheses</h3>
          <div className="stat-big">
            {corpusConfirmed}
            <span> / {corpusHypotheses.length}</span>
          </div>
          <div className="stat-caption">loop-61 contingent findings</div>
        </div>
        <div className="card stat-card">
          <h3>Consistency checks</h3>
          <div className="stat-big">
            {consistencyConfirmed}
            <span> / {consistencyHypotheses.length}</span>
          </div>
          <div className="stat-caption">definition-linked (e.g. H-purity)</div>
        </div>
        <div className="card stat-card">
          <h3>Latent Type-D</h3>
          <div className="stat-big">
            <Cell table="ModelSummary" col="latent_type_d_fraction">
              {summary?.latent_type_d_count != null
                ? `${Math.round(Number(summary.latent_type_d_fraction) * 1000) / 10}%`
                : '—'}
            </Cell>
          </div>
          <div className="stat-caption">
            <Cell table="ModelSummary" col="latent_type_d_count">{summary?.latent_type_d_count ?? '—'}</Cell> /{' '}
            <Cell table="ModelSummary" col="type_d_count">{summary?.type_d_count ?? '—'}</Cell> under sweep
            contract
          </div>
        </div>
      </div>

      <section className="card">
        <h2>Notable findings</h2>
        <p className="section-hint">
          Live cross-cuts over <code>vw_treatment_rankings</code> / <code>vw_studies</code> —
          computed on every page load, not baked-in numbers. Refresh the underlying data and
          these recompute automatically.
        </p>

        <div className="stat-row" style={{ display: 'flex', gap: 12, flexWrap: 'wrap', marginBottom: 20 }}>
          {(['A', 'B', 'C-', 'C+', 'D'] as const).map(t => (
            <div key={t} className="card stat-card" style={{ flex: 1, minWidth: 110 }}>
              <div className="stat-big" style={{ color: TYPE_COLORS[t] }}>
                <TrCell col="distortion_type">{distortionCounts[t]}</TrCell>
              </div>
              <div className="stat-caption">Type {t}</div>
            </div>
          ))}
        </div>

        <div className="findings-grid">
          <div className="finding-panel">
            <div className="finding-num">1</div>
            <span className="finding-badge">Novel</span>
            <h3 className="finding-title">Collider / selection never manifests a sign flip</h3>
            <p className="finding-blurb">
              Across {causalRoleBreakdown.reduce((n, r) => n + r.manifest + r.latent + r.stable, 0)}{' '}
              real-world studies (excludes synthetic boundary-probe studies — see{' '}
              <code>H-collider-no-manifest-theorem</code> below), causal role by outcome:
            </p>
            <div className="side-stack">
              {causalRoleBreakdown.map(r => {
                const total = r.manifest + r.latent + r.stable;
                return (
                  <div key={r.role} className="type-bar-row">
                    <div className="type-bar-header">
                      <span>{r.role}</span>
                      <span style={{ color: '#8b949e' }}>{total}</span>
                    </div>
                    <div style={{ display: 'flex', height: 10, borderRadius: 6, overflow: 'hidden', gap: 1 }}>
                      <div style={{ flex: r.manifest || 0.0001, background: TYPE_COLORS.A, minWidth: r.manifest ? 2 : 0 }} title={`manifest: ${r.manifest}`} />
                      <div style={{ flex: r.latent || 0.0001, background: TYPE_COLORS['C+'], minWidth: r.latent ? 2 : 0 }} title={`latent: ${r.latent}`} />
                      <div style={{ flex: r.stable || 0.0001, background: TYPE_COLORS.D, minWidth: r.stable ? 2 : 0 }} title={`stable: ${r.stable}`} />
                    </div>
                  </div>
                );
              })}
            </div>
            <p className="finding-blurb" style={{ marginTop: 10 }}>
              {(() => {
                const collider = causalRoleBreakdown.find(r => r.role === 'collider');
                const selection = causalRoleBreakdown.find(r => r.role === 'selection');
                const colliderManifest = (collider?.manifest ?? 0) + (selection?.manifest ?? 0);
                return colliderManifest === 0
                  ? 'Zero manifest flips among collider/selection studies in the current corpus — conditioning biases the estimate but does not reverse its sign.'
                  : `${colliderManifest} manifest flip(s) among collider/selection studies — this no-flip pattern does not hold on the current data.`;
              })()}
            </p>
          </div>

          <div className="finding-panel">
            <div className="finding-num">2</div>
            <span className="finding-badge">Novel</span>
            <h3 className="finding-title">Effect size predicts Type-D latent fragility</h3>
            <p className="finding-blurb">Pooled gap bucket → latent-fragile rate, Type D only:</p>
            <div className="side-stack">
              {effectSizeBuckets.map(b => (
                <div key={b.label} className="type-bar-row">
                  <div className="type-bar-header">
                    <span>{b.label}</span>
                    <span style={{ color: '#8b949e' }}>{b.fragile}/{b.n}</span>
                  </div>
                  <div className="type-bar-track">
                    <div className="type-bar-fill" style={{ width: `${b.pct * 100}%`, background: TYPE_COLORS.A }} />
                  </div>
                </div>
              ))}
            </div>
            <div className="chart-canvas-wrap chart-canvas-wrap-finding">
              <canvas ref={typeDFragilityRef} />
            </div>
          </div>

          <div className="finding-panel">
            <div className="finding-num">3</div>
            <span className="finding-badge" style={{ color: '#7ee787' }}>Theorem</span>
            <h3 className="finding-title">Signal purity is necessary but not sufficient for a flip</h3>
            <p className="finding-blurb">
              All manifest reversals (A, B) have purity below 0.5 — proven invariant. C+ studies
              also show low purity but don't flip: their corrected gap stays non-zero.
            </p>
            <div className="chart-canvas-wrap chart-canvas-wrap-finding">
              <canvas ref={purityRef} />
            </div>
          </div>

          <div className="finding-panel">
            <div className="finding-num">4</div>
            <span className="finding-badge">Novel</span>
            <h3 className="finding-title">Domain risk profile</h3>
            <p className="finding-blurb">Manifest / latent-fragile / stable, by domain:</p>
            <div className="side-stack" style={{ maxHeight: 220, overflowY: 'auto' }}>
              {domainBreakdown.map(d => (
                <div key={d.domain} className="type-bar-row">
                  <div className="type-bar-header">
                    <span>{d.domain}</span>
                    <span style={{ color: '#8b949e' }}>{d.total}</span>
                  </div>
                  <div style={{ display: 'flex', height: 10, borderRadius: 6, overflow: 'hidden', gap: 1 }}>
                    <div style={{ flex: d.manifest || 0.0001, background: TYPE_COLORS.A, minWidth: d.manifest ? 2 : 0 }} title={`manifest: ${d.manifest}`} />
                    <div style={{ flex: d.latent || 0.0001, background: TYPE_COLORS['C+'], minWidth: d.latent ? 2 : 0 }} title={`latent: ${d.latent}`} />
                    <div style={{ flex: d.stable || 0.0001, background: TYPE_COLORS.D, minWidth: d.stable ? 2 : 0 }} title={`stable: ${d.stable}`} />
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="finding-panel">
            <div className="finding-num">5</div>
            <span className="finding-badge">Novel</span>
            <h3 className="finding-title">Invisible-fragile Type D studies</h3>
            <p className="finding-blurb">
              <TrCell col="allocation_distortion">{invisibleFragileCount}</TrCell> studies have
              zero measured allocation distortion yet are flagged <code>is_sweep_fragile</code> —
              undetectable by existing distortion metrics but reweighting-sensitive.
            </p>
            <div className="chart-canvas-wrap chart-canvas-wrap-finding">
              <canvas ref={invisibleFragileRef} />
            </div>
          </div>

          <div className="finding-panel">
            <div className="finding-num">6</div>
            <span className="finding-badge">Novel characterization</span>
            <h3 className="finding-title">A vs B is a structural distinction</h3>
            <p className="finding-blurb">
              Type A ({abCompare.aRows.length} studies): {abCompare.aUnanimous} have unanimous
              stratum agreement — the pooled aggregate simply overrides them. Type B (
              {abCompare.bRows.length} studies): {abCompare.bUnanimous} unanimous, meaning most B
              studies have strata that disagree with each other, and pooling favors whichever
              stratum happens to be overweighted.
            </p>
            <p className="finding-blurb" style={{ color: '#8b949e' }}>
              Policy response differs: A warrants immediate stratification; B requires causal
              investigation of which stratum's estimate to trust first.
            </p>
          </div>
        </div>
      </section>

      <section className="card">
        <h2>Consistency checks (loop-61)</h2>
        <p className="section-hint">
          Confirm implementation matches stated algebra — same tier as conc-12. Not independent
          discoveries about nature.
        </p>
        <div className="discovery-grid">
          {consistencyHypotheses.map(h => (
            <DiscoveryCard
              key={h.hypothesis_id}
              h={h}
              f={findingsByHypothesis.get(h.hypothesis_id)}
              tierLabel="consistency"
            />
          ))}
        </div>
      </section>

      <section className="card">
        <h2>Corpus hypotheses (loop-61)</h2>
        <p className="section-hint">
          Contingent patterns on this convenience sample — directional only, not inferential.
          See conc-14.
        </p>
        <div className="discovery-grid">
          {corpusHypotheses.map(h => (
            <DiscoveryCard
              key={h.hypothesis_id}
              h={h}
              f={findingsByHypothesis.get(h.hypothesis_id)}
              tierLabel="corpus"
            />
          ))}
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
              {CATEGORY_ORDER.map((cat: (typeof CATEGORY_ORDER)[number]) => (
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
                    <Cell table="Conclusions" col="status">{c.status}</Cell>
                  </span>
                  <span className="conclusion-cat">{categoryLabel(c.category, c.conclusion_id)}</span>
                  <span className="conclusion-title">{c.title}</span>
                  {c.invariant_protecting_count != null && c.invariant_protecting_count > 0 && (
                    <span className="conclusion-inv-count">
                      <Cell table="Conclusions" col="invariant_protecting_count">{c.invariant_protecting_count}</Cell> inv
                    </span>
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
                  <Cell table="Conclusions" col="status">{selected.status}</Cell>
                </span>
              </div>
              <h2 className="detail-title">{selected.title}</h2>
              <div className="detail-meta">
                <span>Tier: {categoryLabel(selected.category, selected.conclusion_id)}</span>
                {selected.witnessed_in_loop && (
                  <span>Witnessed in: {selected.witnessed_in_loop}</span>
                )}
                {selected.target_loop && <span>Target loop: {selected.target_loop}</span>}
              </div>
              {domainCaveat(selected.conclusion_id) && (
                <p className="domain-caveat">{domainCaveat(selected.conclusion_id)}</p>
              )}
              {(selected.witnessed_in_loop_commit_short || selected.witnessed_in_loop_git_tag) && (
                <div className="replay-block">
                  <h3>Loop replay</h3>
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
                          <Cell table="InvariantChecks" col="status_label">{inv.status_label}</Cell>
                        </span>
                        <strong>{inv.invariant_check_id}</strong>
                        <span><Cell table="InvariantChecks" col="natural_language">{inv.natural_language}</Cell></span>
                        <span style={{ marginLeft: 8, color: '#8b949e', fontSize: 12 }}>
                          pass <Cell table="InvariantChecks" col="pass_count">{inv.pass_count}</Cell>
                          {' / fail '}
                          <Cell table="InvariantChecks" col="fail_count">{inv.fail_count}</Cell>
                        </span>
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
      <ViewDagScan ready={!loading} deps={[conclusions, hypotheses, findings, invariants, selectedId, categoryFilter, rankings, studies]} />
    </div>
  );
}
