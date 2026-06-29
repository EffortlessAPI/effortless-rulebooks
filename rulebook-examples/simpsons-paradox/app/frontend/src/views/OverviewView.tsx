import { useEffect, useMemo, useRef, useState } from 'react';
import { Chart } from 'chart.js/auto';
import { api } from '../api';
import type { ModelSummary, Study, TreatmentRanking } from '../types';
import {
  TYPE_COLORS,
  buildCanonicalPlaneChart,
  buildPurityChart,
  buildRecoveryChart,
  buildScreeningChart,
} from './overviewCharts';
import './Overview.css';

const TYPE_INSIGHTS = [
  {
    key: 'A',
    typeClass: 'type-a',
    title: 'Full reversal',
    desc: 'Pooled winner is wrong in every stratum. Stratify immediately — the aggregate lies.',
    filter: (r: TreatmentRanking) => r.distortion_type === 'A',
  },
  {
    key: 'B',
    typeClass: 'type-b',
    title: 'Sign flip',
    desc: 'Direction reverses but not unanimously. Investigate the confounder before trusting pooled rates.',
    filter: (r: TreatmentRanking) => r.distortion_type === 'B',
  },
  {
    key: 'C',
    typeClass: 'type-c',
    title: 'Magnitude distortion',
    desc: 'Winner direction holds, but allocation inflates or compresses the effect size.',
    filter: (r: TreatmentRanking) => r.distortion_type.startsWith('C'),
  },
  {
    key: 'D',
    typeClass: 'type-d',
    title: 'Trustworthy pooled',
    desc: 'Allocation is balanced enough — the naive aggregate is safe to report.',
    filter: (r: TreatmentRanking) => r.distortion_type === 'D',
  },
] as const;

function n(v: number | string | null | undefined, dp = 3): string {
  if (v == null || v === '') return '—';
  return Number(v).toFixed(dp);
}

function pct(v: number | null | undefined): string {
  if (v == null) return '—';
  return (Number(v) * 100).toFixed(1) + '%';
}

function typeBadge(type: string | null | undefined) {
  const cls = type?.startsWith('C') ? 'badge badge-type-c' : `badge badge-type-${type?.toLowerCase() ?? 'd'}`;
  return <span className={cls}>Type {type}</span>;
}

function tierPill(tier: string | null | undefined) {
  if (!tier) return null;
  return <span className={`tier-pill ${tier.toLowerCase()}`}>{tier}</span>;
}

export function OverviewView() {
  const [studies, setStudies] = useState<Study[]>([]);
  const [rankings, setRankings] = useState<TreatmentRanking[]>([]);
  const [summary, setSummary] = useState<ModelSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [typeFilter, setTypeFilter] = useState<string | null>(null);
  const [tierFilter, setTierFilter] = useState<string | null>(null);
  const [search, setSearch] = useState('');
  const [expanded, setExpanded] = useState<string | null>(null);
  const [pdfLoading, setPdfLoading] = useState(false);
  const [pdfError, setPdfError] = useState<string | null>(null);

  const canonicalRef = useRef<HTMLCanvasElement>(null);
  const finding1Ref = useRef<HTMLCanvasElement>(null);
  const finding2Ref = useRef<HTMLCanvasElement>(null);
  const finding3Ref = useRef<HTMLCanvasElement>(null);
  const chartsRef = useRef<(Chart | null)[]>([]);

  useEffect(() => {
    Promise.all([api.studies(), api.treatmentRankings(), api.modelSummary()])
      .then(([s, r, m]) => { setStudies(s); setRankings(r); setSummary(m); })
      .finally(() => setLoading(false));
  }, []);

  const studyById = useMemo(
    () => Object.fromEntries(studies.map(s => [s.study_id, s])),
    [studies],
  );


  const spotlight = useMemo(
    () => [...rankings]
      .sort((a, b) => Number(b.paradox_strength) - Number(a.paradox_strength))
      .slice(0, 6),
    [rankings],
  );

  const filteredStudies = useMemo(() => {
    const q = search.trim().toLowerCase();
    return studies.filter(s => {
      const r = rankings.find(x => x.study === s.study_id);
      if (typeFilter && r?.distortion_type !== typeFilter && !(typeFilter === 'C' && r?.distortion_type.startsWith('C'))) return false;
      if (tierFilter && r?.screening_tier !== tierFilter) return false;
      if (!q) return true;
      return (
        s.study_id.toLowerCase().includes(q) ||
        (s.title ?? '').toLowerCase().includes(q) ||
        (s.domain ?? '').toLowerCase().includes(q)
      );
    });
  }, [studies, rankings, typeFilter, tierFilter, search]);

  useEffect(() => {
    if (!rankings.length) return;
    chartsRef.current.forEach(c => c?.destroy());
    const built: Chart[] = [];
    if (canonicalRef.current) built.push(buildCanonicalPlaneChart(canonicalRef.current, rankings, studyById));
    if (finding1Ref.current) built.push(buildRecoveryChart(finding1Ref.current, rankings, studyById));
    if (finding2Ref.current) built.push(buildScreeningChart(finding2Ref.current, rankings, studyById));
    if (finding3Ref.current) built.push(buildPurityChart(finding3Ref.current, rankings, studyById));
    chartsRef.current = built;
    return () => { chartsRef.current.forEach(c => c?.destroy()); chartsRef.current = []; };
  }, [rankings, studyById]);

  async function handleDownloadSummary() {
    setPdfError(null);
    setPdfLoading(true);
    try {
      await api.downloadSummaryPdf();
    } catch (err) {
      setPdfError(err instanceof Error ? err.message : String(err));
    } finally {
      setPdfLoading(false);
    }
  }

  if (loading) return <div className="loading">Loading…</div>;

  const total = summary?.study_count ?? studies.length;
  const dangerCount = Number(summary?.danger_tier_count ?? 0);
  const signFlipCount = rankings.filter(r => r.is_sign_flip).length;

  return (
    <div className="overview">
      <div className="overview-hero">
        <div className="overview-hero-inner">
          <div className="overview-eyebrow">Simpson's Paradox · {total} witnessed studies</div>
          <h1 className="overview-headline">
            <em>{dangerCount} of {total}</em> pooled conclusions would mislead you.
          </h1>
          <p className="overview-sub">
            Three derived facts fall out of the same arithmetic: severity tracks allocation bias,
            corrected gaps diverge from pooled gaps on sign-flips, and signal purity flags when
            the aggregate is mostly noise.
          </p>
          <div className="overview-hero-actions">
            <button
              type="button"
              className="summary-pdf-btn"
              onClick={handleDownloadSummary}
              disabled={pdfLoading}
            >
              {pdfLoading ? 'Building PDF…' : 'Download summary PDF'}
            </button>
            <span className="summary-pdf-hint">
              Conclusions, discovery findings, invariant health, and GitHub evidence links — not the full rulebook.
            </span>
          </div>
          {pdfError && <p className="summary-pdf-error">{pdfError}</p>}
        </div>
      </div>

      <div className="kpi-strip">
        <div className="kpi-tile danger">
          <div className="kpi-value danger">{dangerCount}</div>
          <div className="kpi-label">DANGER tier<br />Pooled sign is wrong</div>
        </div>
        <div className="kpi-tile caution">
          <div className="kpi-value caution">{summary?.caution_tier_count ?? '—'}</div>
          <div className="kpi-label">CAUTION tier<br />Magnitude distorted</div>
        </div>
        <div className="kpi-tile safe">
          <div className="kpi-value safe">{summary?.safe_tier_count ?? '—'}</div>
          <div className="kpi-label">SAFE tier<br />Pooling trustworthy</div>
        </div>
        <div className="kpi-tile info">
          <div className="kpi-value info">{signFlipCount}</div>
          <div className="kpi-label">Sign-flips<br />Type A: {summary?.type_a_count ?? '—'} unanimous</div>
        </div>
      </div>

      {/* Anchor visualization — the original good one */}
      <div className="canonical-plane-panel">
        <div className="finding-badge">Signature view</div>
        <h2 className="canonical-title">Allocation distortion plane</h2>
        <p className="chart-caption">
          Each dot is a study. X = how far allocation bends the pooled signal;
          Y = paradox strength (pooled gap × reversal intensity).
          Sign-flip studies rise from the origin toward the upper-right — severity tracks bias.
          Color = distortion type.
        </p>
        <div className="chart-canvas-wrap chart-canvas-wrap-hero">
          <canvas ref={canonicalRef} />
        </div>
      </div>

      {/* Three key findings — additional 2D views, not replacements */}
      <div className="findings-section-header">
        <h2 className="section-title">Three findings from the corpus</h2>
        <p className="section-sub">Same 90+ studies, three other lenses — the plane above is unchanged.</p>
      </div>

      <div className="findings-grid">
        <div className="finding-panel">
          <div className="finding-num">1</div>
          <h3 className="finding-title">Reversal recovery is visible</h3>
          <p className="finding-blurb">
            Where sign-flip occurs, the pooled gap (x) and stratum-corrected gap (y) disagree.
            Points off the diagonal are studies where pooling picked the wrong direction.
          </p>
          <div className="chart-canvas-wrap chart-canvas-wrap-finding">
            <canvas ref={finding1Ref} />
          </div>
        </div>

        <div className="finding-panel">
          <div className="finding-num">2</div>
          <h3 className="finding-title">Screening separates safe from dangerous</h3>
          <p className="finding-blurb">
            Paradox strength vs allocation distortion, colored by screening tier.
            DANGER studies cluster upper-right; SAFE studies hug the origin.
          </p>
          <div className="chart-canvas-wrap chart-canvas-wrap-finding">
            <canvas ref={finding2Ref} />
          </div>
        </div>

        <div className="finding-panel">
          <div className="finding-num">3</div>
          <h3 className="finding-title">Signal purity marks suspect aggregates</h3>
          <p className="finding-blurb">
            When purity drops below 0.5, allocation noise contributes more than half the pooled
            signal. Sign-flip studies (red/purple) sit low; many safe studies sit high.
          </p>
          <div className="chart-canvas-wrap chart-canvas-wrap-finding">
            <canvas ref={finding3Ref} />
          </div>
        </div>
      </div>

      <div className="insight-grid">
        {TYPE_INSIGHTS.map(ins => {
          const count = rankings.filter(ins.filter).length;
          const active = typeFilter === ins.key;
          return (
            <div
              key={ins.key}
              className={`insight-card ${ins.typeClass}${active ? ' active' : ''}`}
              onClick={() => setTypeFilter(active ? null : ins.key)}
              role="button"
              tabIndex={0}
              onKeyDown={e => e.key === 'Enter' && setTypeFilter(active ? null : ins.key)}
            >
              <div className="insight-type">TYPE {ins.key}</div>
              <div className="insight-title">{ins.title}</div>
              <div className="insight-desc">{ins.desc}</div>
              <div className="insight-count">{count}</div>
            </div>
          );
        })}
      </div>

      <div className="spotlight-section">
        <div className="section-header">
          <div>
            <div className="section-title">Most misleading studies</div>
            <div className="section-sub">Ranked by paradox strength — click to jump to full detail below</div>
          </div>
        </div>
        <div className="spotlight-list">
          {spotlight.map((r, i) => {
            const study = studyById[r.study];
            return (
              <div
                key={r.treatment_ranking_id}
                className="spotlight-row"
                onClick={() => {
                  setExpanded(r.study);
                  setSearch('');
                  setTypeFilter(null);
                  setTierFilter(null);
                  document.getElementById(`study-${r.study}`)?.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }}
              >
                <div className={`spotlight-rank${i < 3 ? ' top' : ''}`}>{i + 1}</div>
                <div>
                  <div className="spotlight-name">{study?.title ?? r.study}</div>
                  <div className="spotlight-meta">{r.study} · {study?.domain ?? '—'}</div>
                </div>
                <div className="spotlight-metrics">
                  <div className="spotlight-metric">
                    <span className="val" style={{ color: TYPE_COLORS[r.distortion_type] ?? '#ff7b72' }}>
                      {n(r.paradox_strength, 4)}
                    </span>
                    <span className="lbl">Strength</span>
                  </div>
                  <div className="spotlight-metric">
                    <span className="val">{n(r.allocation_distortion)}</span>
                    <span className="lbl">Distortion</span>
                  </div>
                  <div className="spotlight-metric">
                    <span className="val">{n(r.signal_purity, 3)}</span>
                    <span className="lbl">Purity</span>
                  </div>
                </div>
                <div className="study-row-badges">
                  {typeBadge(r.distortion_type)}
                  {tierPill(r.screening_tier)}
                </div>
              </div>
            );
          })}
        </div>
      </div>

      <div className="study-explorer">
        <div className="section-header">
          <div className="section-title">All studies</div>
          <span className="explorer-count">{filteredStudies.length} of {total}</span>
        </div>

        <div className="explorer-toolbar">
          <input
            className="explorer-search"
            placeholder="Search by name, domain, or ID…"
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
          {(['DANGER', 'CAUTION', 'SAFE'] as const).map(t => (
            <button
              key={t}
              type="button"
              className={`filter-chip tier-${t.toLowerCase()}${tierFilter === t ? ' active' : ''}`}
              onClick={() => setTierFilter(tierFilter === t ? null : t)}
            >
              {t}
            </button>
          ))}
          {(typeFilter || tierFilter || search) && (
            <button
              type="button"
              className="filter-chip"
              onClick={() => { setTypeFilter(null); setTierFilter(null); setSearch(''); }}
            >
              Clear filters
            </button>
          )}
        </div>

        {filteredStudies.map(s => {
          const r = rankings.find(x => x.study === s.study_id);
          const isOpen = expanded === s.study_id;
          return (
            <div className="study-row-compact" key={s.study_id} id={`study-${s.study_id}`}>
              <div
                className="study-row-header"
                onClick={() => setExpanded(isOpen ? null : s.study_id)}
              >
                <div>
                  <div className="study-row-title">{s.title || s.name}</div>
                  <div className="study-row-source">{s.source}</div>
                </div>
                <div className="study-row-badges">
                  {r && typeBadge(r.distortion_type)}
                  {r && tierPill(r.screening_tier)}
                </div>
                <span className={`study-row-chevron${isOpen ? ' open' : ''}`}>▶</span>
              </div>
              {isOpen && r && (
                <div className="study-row-detail">
                  <div className="detail-kv"><span className="dk">Pooled winner</span><span className="dv">{r.pooled_winner}</span></div>
                  <div className="detail-kv"><span className="dk">Per-stratum winner</span><span className="dv">{r.per_stratum_winner}</span></div>
                  <div className="detail-kv"><span className="dk">Paradox strength</span><span className="dv">{n(r.paradox_strength, 4)}</span></div>
                  <div className="detail-kv"><span className="dk">Sign flip</span><span className="dv" style={{ color: r.is_sign_flip ? '#ff7b72' : '#7ee787' }}>{r.is_sign_flip ? 'YES' : 'no'}</span></div>
                  <div className="detail-kv"><span className="dk">Allocation distortion</span><span className="dv">{n(r.allocation_distortion)}</span></div>
                  <div className="detail-kv"><span className="dk">Distortion ratio</span><span className="dv">{n(r.distortion_ratio)}</span></div>
                  <div className="detail-kv"><span className="dk">Signal purity</span><span className="dv" style={{ color: r.signal_purity != null && r.signal_purity < 0.5 ? '#ff7b72' : '#7ee787' }}>{pct(r.signal_purity)}</span></div>
                  <div className="detail-kv"><span className="dk">Domain · year</span><span className="dv">{s.domain ?? '—'} · {s.publication_year ?? '—'}</span></div>
                  {r.policy_implication && (
                    <div style={{ gridColumn: '1 / -1', fontSize: 12, color: '#8b949e', marginTop: 4, lineHeight: 1.5 }}>
                      {r.policy_implication}
                    </div>
                  )}
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
