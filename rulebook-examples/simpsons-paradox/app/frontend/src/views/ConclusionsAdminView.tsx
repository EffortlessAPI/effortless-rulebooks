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

  function median(vals: number[]): number {
    if (!vals.length) return 0;
    const s = [...vals].sort((a, b) => a - b);
    const mid = Math.floor(s.length / 2);
    return s.length % 2 ? s[mid] : (s[mid - 1] + s[mid]) / 2;
  }

  const abCompare = useMemo(() => {
    const aRows = rankings.filter(r => r.distortion_type === 'A');
    const bRows = rankings.filter(r => r.distortion_type === 'B');
    const aUnanimous = aRows.filter(r => r.is_stratum_unanimous).length;
    const bUnanimous = bRows.filter(r => r.is_stratum_unanimous).length;
    const aMedianPurity = median(aRows.filter(r => r.signal_purity != null).map(r => Number(r.signal_purity)));
    const bMedianPurity = median(bRows.filter(r => r.signal_purity != null).map(r => Number(r.signal_purity)));
    return { aRows, bRows, aUnanimous, bUnanimous, aMedianPurity, bMedianPurity };
  }, [rankings]);

  // Finding 1 right panel: causal role × outcome, flattened to 9 named rows (matches artifact layout).
  const causalModeRows = useMemo(() => {
    const find = (role: string, key: 'manifest' | 'latent' | 'stable') =>
      causalRoleBreakdown.find(r => r.role === role)?.[key] ?? 0;
    const label = (role: string) => role.charAt(0).toUpperCase() + role.slice(1);
    return (['confounder', 'collider', 'selection'] as const).flatMap(role => ([
      { label: `${label(role)} → manifest flip`, n: find(role, 'manifest'), color: TYPE_COLORS.A },
      { label: `${label(role)} → latent only`, n: find(role, 'latent'), color: TYPE_COLORS['C+'] },
      { label: `${label(role)} → stable`, n: find(role, 'stable'), color: TYPE_COLORS.D },
    ]));
  }, [causalRoleBreakdown]);

  // Finding 3 right panel: C+ studies — high distortion, zero sign-flip.
  const cPlusTable = useMemo(
    () => rankings
      .filter(r => r.distortion_type === 'C+')
      .map(r => ({
        study: r.study,
        purity: Number(r.signal_purity ?? 0),
        distortion_ratio: Number(r.distortion_ratio),
        pooled_gap: Number(r.pooled_gap),
        corrected_gap: Math.abs(Number(r.weighted_stratum_gap_sum)),
      }))
      .sort((a, b) => b.distortion_ratio - a.distortion_ratio),
    [rankings],
  );

  // Finding 4: full domain breakdown with percentages (no truncation).
  const domainBreakdownPct = useMemo(
    () => domainBreakdown.map(d => ({
      ...d,
      manifestPct: d.total ? d.manifest / d.total : 0,
      latentPct: d.total ? d.latent / d.total : 0,
      stablePct: d.total ? d.stable / d.total : 0,
    })),
    [domainBreakdown],
  );

  // Finding 5: ranked fragility list for "invisible fragile" Type D studies (purity=1, distortion=0, sweep>0).
  const invisibleFragileRanked = useMemo(
    () => rankings
      .filter(r => r.is_sweep_fragile && Number(r.allocation_distortion) === 0)
      .map(r => ({ study: r.study, fragility: Number(r.allocation_fragility ?? 0) }))
      .sort((a, b) => b.fragility - a.fragility)
      .slice(0, 12),
    [rankings],
  );

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

        {/* Finding 1 — causal role × outcome */}
        <div className="finding-2col">
          <div className="finding-panel">
            <span className="finding-badge">Finding 1 · Novel</span>
            <h3 className="finding-title">Collider / selection never manifests a sign flip</h3>
            <p className="finding-blurb">Causal role × manifest vs latent reversal</p>
            <div className="side-stack">
              {causalRoleBreakdown.map(r => {
                const total = r.manifest + r.latent + r.stable;
                return (
                  <div key={r.role} className="nf-stacked-row">
                    <span className="nf-bar-label" style={{ textAlign: 'left' }}>{r.role}</span>
                    <div className="nf-stacked-track">
                      <div className="nf-stacked-seg" style={{ flex: r.manifest || 0.0001, background: TYPE_COLORS.A, minWidth: r.manifest ? 2 : 0 }} title={`manifest: ${r.manifest}`} />
                      <div className="nf-stacked-seg" style={{ flex: r.latent || 0.0001, background: TYPE_COLORS['C+'], minWidth: r.latent ? 2 : 0 }} title={`latent: ${r.latent}`} />
                      <div className="nf-stacked-seg" style={{ flex: r.stable || 0.0001, background: TYPE_COLORS.D, minWidth: r.stable ? 2 : 0 }} title={`stable: ${r.stable}`} />
                    </div>
                    <span className="nf-bar-val" style={{ textAlign: 'right' }}>{total}</span>
                  </div>
                );
              })}
            </div>
            <div className="nf-legend">
              <span className="nf-legend-item"><span className="nf-legend-swatch" style={{ background: TYPE_COLORS.A }} />Manifest flip</span>
              <span className="nf-legend-item"><span className="nf-legend-swatch" style={{ background: TYPE_COLORS['C+'] }} />Latent only</span>
              <span className="nf-legend-item"><span className="nf-legend-swatch" style={{ background: TYPE_COLORS.D }} />Stable</span>
            </div>
            <p className="finding-blurb">
              Real-world corpus only ({causalRoleBreakdown.reduce((n, r) => n + r.manifest + r.latent + r.stable, 0)}{' '}
              studies; excludes synthetic boundary-probe studies — see{' '}
              <code>H-collider-no-manifest-theorem</code> below). Collider and selection studies
              are exclusively latent or stable — they can appear fragile under sweep but never
              actually flip at observed allocation.
            </p>
          </div>

          <div className="finding-panel">
            <h3 className="finding-title" style={{ visibility: 'hidden', height: 0, margin: 0 }}>&nbsp;</h3>
            <p className="finding-blurb"><strong>Causal role → reversal mode (count)</strong></p>
            <div className="nf-bar-list">
              {causalModeRows.map(row => {
                const max = Math.max(1, ...causalModeRows.map(r => r.n));
                return (
                  <div key={row.label} className="nf-bar-row wide-label">
                    <span className="nf-bar-label">{row.label}</span>
                    <div className="nf-bar-track">
                      <div className="nf-bar-fill" style={{ width: `${(row.n / max) * 100}%`, background: row.color }} />
                    </div>
                    <span className="nf-bar-val">{row.n}</span>
                  </div>
                );
              })}
            </div>
            <p className="finding-blurb" style={{ marginTop: 10, color: '#8b949e' }}>
              Theorem candidate: conditioning on a collider biases the estimate but does not
              reverse its sign under the 2×K encoding.
            </p>
          </div>
        </div>

        {/* Finding 2 — effect size vs Type-D fragility */}
        <div className="finding-2col">
          <div className="finding-panel">
            <span className="finding-badge">Finding 2 · Novel</span>
            <h3 className="finding-title">Effect size is the strongest predictor of Type-D fragility</h3>
            <p className="finding-blurb">Pooled gap vs latent-flip potential (Type D)</p>
            <div className="nf-legend">
              <span className="nf-legend-item"><span className="nf-legend-swatch" style={{ background: TYPE_COLORS.A }} />Latent-fragile</span>
              <span className="nf-legend-item"><span className="nf-legend-swatch" style={{ background: TYPE_COLORS.D }} />Allocation-stable</span>
            </div>
            <div className="chart-canvas-wrap chart-canvas-wrap-finding">
              <canvas ref={typeDFragilityRef} />
            </div>
            <p className="finding-blurb" style={{ color: '#8b949e' }}>
              Stable D studies cluster at larger pooled gaps; fragile D studies concentrate where
              the observed effect is tiny, even when purity = 1.0.
            </p>
          </div>

          <div className="finding-panel">
            <p className="finding-blurb"><strong>Effect size bucket → latent-flip rate (Type D)</strong></p>
            <div className="nf-bar-list">
              {effectSizeBuckets.map(b => (
                <div key={b.label} className="nf-bar-row">
                  <span className="nf-bar-label">{b.label}</span>
                  <div className="nf-bar-track">
                    <div className="nf-bar-fill" style={{ width: `${b.pct * 100}%`, background: TYPE_COLORS.A }} />
                  </div>
                  <span className="nf-bar-val">{Math.round(b.pct * 100)}% ({b.fragile}/{b.n})</span>
                </div>
              ))}
            </div>
            <p className="finding-blurb" style={{ marginTop: 10, color: '#8b949e' }}>
              Effect size is a near-perfect separator between fragile and stable Type-D studies —
              not part of any published taxonomy we're aware of.
            </p>
          </div>
        </div>

        {/* Finding 3 — signal purity necessary but not sufficient */}
        <div className="finding-2col">
          <div className="finding-panel">
            <span className="finding-badge" style={{ color: '#7ee787' }}>Finding 3 · Theorem (proven)</span>
            <h3 className="finding-title">Signal purity is necessary but not sufficient for a flip</h3>
            <p className="finding-blurb">Signal purity by distortion type</p>
            <div className="nf-strip-chart">
              <div className="nf-strip-threshold" style={{ bottom: '50%' }}>
                <span className="nf-strip-threshold-label">purity = 0.5</span>
              </div>
              {(['A', 'B', 'C-', 'C+', 'D'] as const).map(t => (
                <div key={t} className="nf-strip-col">
                  {rankings.filter(r => r.distortion_type === t && r.signal_purity != null).map(r => (
                    <div
                      key={r.study}
                      className="nf-strip-dot"
                      style={{
                        bottom: `${Math.max(0, Math.min(100, Number(r.signal_purity) * 100))}%`,
                        background: TYPE_COLORS[t],
                        opacity: 0.75,
                      }}
                      title={`${r.study}: purity ${Number(r.signal_purity).toFixed(3)}`}
                    />
                  ))}
                  <span className="nf-strip-axis-label" style={{ color: TYPE_COLORS[t] }}>{t}</span>
                </div>
              ))}
            </div>
            <p className="finding-blurb" style={{ color: '#8b949e' }}>
              All manifest reversals (A, B) have purity below 0.5 — proven invariant. C+ studies
              also show low purity but don't flip: their corrected gap stays non-zero. The full
              condition is purity &lt; 0.5 AND |corrected gap| ≈ 0.
            </p>
          </div>

          <div className="finding-panel">
            <p className="finding-blurb"><strong>C+ studies: high distortion, zero sign-flip — why?</strong></p>
            <div className="nf-table-wrap">
              <table className="nf-table">
                <thead>
                  <tr>
                    <th>Study</th>
                    <th>Purity</th>
                    <th>Distortion</th>
                    <th>Pooled gap</th>
                    <th>|Corrected|</th>
                  </tr>
                </thead>
                <tbody>
                  {cPlusTable.map(r => (
                    <tr key={r.study}>
                      <td>{r.study}</td>
                      <td>{(r.purity * 100).toFixed(1)}%</td>
                      <td style={{ color: TYPE_COLORS['C+'] }}>{r.distortion_ratio.toFixed(1)}×</td>
                      <td>{(r.pooled_gap * 100).toFixed(1)}%</td>
                      <td>{(r.corrected_gap * 100).toFixed(1)}%</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <p className="finding-blurb" style={{ marginTop: 10, color: '#8b949e' }}>
              Amplification ≠ reversal: distortion ratio can be an order of magnitude off while
              both pooled and corrected gaps still point the same direction.
            </p>
          </div>
        </div>

        {/* Finding 4 — domain risk profile */}
        <div className="finding-panel finding-panel-wide" style={{ marginBottom: 28 }}>
          <span className="finding-badge">Finding 4 · Novel</span>
          <h3 className="finding-title">Domain risk profile: manifest · latent-fragile · stable — by domain</h3>
          <div className="nf-legend">
            <span className="nf-legend-item"><span className="nf-legend-swatch" style={{ background: TYPE_COLORS.A }} />Manifest wrong</span>
            <span className="nf-legend-item"><span className="nf-legend-swatch" style={{ background: TYPE_COLORS['C+'] }} />Latent fragile</span>
            <span className="nf-legend-item"><span className="nf-legend-swatch" style={{ background: TYPE_COLORS.D }} />Stable</span>
          </div>
          <div className="side-stack">
            {domainBreakdownPct.map(d => (
              <div key={d.domain} className="nf-stacked-row" style={{ gridTemplateColumns: '110px 1fr 40px' }}>
                <span className="nf-bar-label" style={{ textAlign: 'left' }}>{d.domain}</span>
                <div className="nf-stacked-track" style={{ height: 20 }}>
                  {d.manifest > 0 && (
                    <div className="nf-stacked-seg" style={{ flex: d.manifest, background: TYPE_COLORS.A }} title={`${Math.round(d.manifestPct * 100)}% manifest`} />
                  )}
                  {d.latent > 0 && (
                    <div className="nf-stacked-seg" style={{ flex: d.latent, background: TYPE_COLORS['C+'] }} title={`${Math.round(d.latentPct * 100)}% latent-fragile`} />
                  )}
                  {d.stable > 0 && (
                    <div className="nf-stacked-seg" style={{ flex: d.stable, background: TYPE_COLORS.D }} title={`${Math.round(d.stablePct * 100)}% stable`} />
                  )}
                </div>
                <span className="nf-bar-val" style={{ textAlign: 'right' }}>n={d.total}</span>
              </div>
            ))}
          </div>
          <p className="finding-blurb" style={{ marginTop: 10, color: '#8b949e' }}>
            {(() => {
              const sorted = [...domainBreakdownPct].sort((a, b) => b.latentPct - a.latentPct);
              const top = sorted[0];
              const zeroManifest = domainBreakdownPct.filter(d => d.manifest === 0).map(d => d.domain);
              return top
                ? `${top.domain} has the highest latent-fragile rate (${Math.round(top.latentPct * 100)}%). ${
                    zeroManifest.length ? `Zero manifest flips in: ${zeroManifest.join(', ')}.` : ''
                  }`
                : '';
            })()}
          </p>
        </div>

        {/* Finding 5 — invisible-fragile Type D */}
        <div className="finding-2col">
          <div className="finding-panel">
            <span className="finding-badge">Finding 5 · Novel</span>
            <h3 className="finding-title">Ultra-fragile "invisible" Type D studies</h3>
            <p className="finding-blurb">
              Allocation fragility (sweep range ÷ pooled gap) — Type D studies with
              signal_purity = 1.0 and allocation_distortion = 0
            </p>
            <div className="nf-bar-list">
              {invisibleFragileRanked.map(r => {
                const max = Math.max(1, ...invisibleFragileRanked.map(x => x.fragility));
                return (
                  <div key={r.study} className="nf-bar-row wide-label">
                    <span className="nf-bar-label">{r.study}</span>
                    <div className="nf-bar-track">
                      <div className="nf-bar-fill" style={{ width: `${(r.fragility / max) * 100}%`, background: TYPE_COLORS['C+'] }} />
                    </div>
                    <span className="nf-bar-val">{r.fragility.toFixed(1)}×</span>
                  </div>
                );
              })}
            </div>
            <p className="finding-blurb" style={{ marginTop: 10, color: '#8b949e' }}>
              <TrCell col="allocation_distortion">{invisibleFragileCount}</TrCell> studies look
              perfectly safe by every published measure (purity 1.0, distortion 0) yet are
              flagged <code>is_sweep_fragile</code> — undetectable by existing distortion metrics.
            </p>
          </div>

          <div className="finding-panel">
            <p className="finding-blurb"><strong>Allocation distortion vs sweep range (all types)</strong></p>
            <div className="chart-canvas-wrap chart-canvas-wrap-finding">
              <canvas ref={invisibleFragileRef} />
            </div>
            <p className="finding-blurb" style={{ color: '#8b949e' }}>
              The cluster at distortion≈0, sweep&gt;0 is the "invisible fragile" group — undetectable
              by allocation-distortion metrics but highly reweighting-sensitive.
            </p>
          </div>
        </div>

        {/* Finding 6 — A vs B structural distinction */}
        <div className="finding-panel finding-panel-wide">
          <span className="finding-badge">Finding 6 · Novel characterization</span>
          <h3 className="finding-title">A vs B is a structural distinction (unanimous vs heterogeneous strata)</h3>
          <p className="finding-blurb">
            Type A: strata agree, pool overrides them · Type B: strata disagree, pool picks the wrong one
          </p>
          <div className="nf-compare-grid">
            <div className="nf-compare-card" style={{ borderLeft: `3px solid ${TYPE_COLORS.A}` }}>
              <h4 style={{ color: TYPE_COLORS.A }}>Type A — allocation-driven</h4>
              <div className="nf-compare-stat">
                <div className="nf-cs-val">{abCompare.aRows.length}</div>
                <div className="nf-cs-label">studies</div>
              </div>
              <div className="nf-compare-stat">
                <div className="nf-cs-val">{abCompare.aUnanimous}</div>
                <div className="nf-cs-label">unanimous strata — all agree on direction</div>
              </div>
              <div className="nf-compare-stat">
                <div className="nf-cs-val">{Math.round(abCompare.aMedianPurity * 100)}%</div>
                <div className="nf-cs-label">median purity (lower = more distorted)</div>
              </div>
              <div className="nf-compare-mech">
                <strong>Mechanism</strong>
                Every stratum agrees; the error is purely in the pooling weights.
              </div>
              <div className="nf-compare-mech">
                <strong>Policy</strong>
                Stratify immediately. The corrected winner is unambiguous.
              </div>
            </div>
            <div className="nf-compare-card" style={{ borderLeft: `3px solid ${TYPE_COLORS.B}` }}>
              <h4 style={{ color: TYPE_COLORS.B }}>Type B — heterogeneity-driven</h4>
              <div className="nf-compare-stat">
                <div className="nf-cs-val">{abCompare.bRows.length}</div>
                <div className="nf-cs-label">studies</div>
              </div>
              <div className="nf-compare-stat">
                <div className="nf-cs-val">{abCompare.bUnanimous}</div>
                <div className="nf-cs-label">unanimous strata — most strata disagree</div>
              </div>
              <div className="nf-compare-stat">
                <div className="nf-cs-val">{Math.round(abCompare.bMedianPurity * 100)}%</div>
                <div className="nf-cs-label">median purity (lower = more distorted)</div>
              </div>
              <div className="nf-compare-mech">
                <strong>Mechanism</strong>
                Strata themselves contradict each other; the pooled result favors whichever
                stratum happens to be overweighted.
              </div>
              <div className="nf-compare-mech">
                <strong>Policy</strong>
                Investigate before acting — causal role is contested or confounded by
                heterogeneous subpopulation effects.
              </div>
            </div>
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
