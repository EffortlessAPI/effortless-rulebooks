import { useEffect, useRef, useState } from 'react';
import { Chart } from 'chart.js/auto';
import { api } from '../api';
import { ViewDagScan } from '../components/DagValue';
import { SsCell, TrCell } from '../components/dag-display';
import type { Study, StratumSummary, TreatmentRanking } from '../types';
import { StudySelector } from '../components/StudySelector';

function pct(n: number | null) {
  if (n == null) return '—';
  return (Number(n) * 100).toFixed(1) + '%';
}

function policyClass(p: string) {
  if (p?.includes('stratify')) return 'policy-stratify';
  if (p?.includes('investigate')) return 'policy-investigate';
  if (p?.includes('check')) return 'policy-check';
  return 'policy-trustworthy';
}

export function StratumView() {
  const [studies, setStudies] = useState<Study[]>([]);
  const [selected, setSelected] = useState('');
  const [summaries, setSummaries] = useState<StratumSummary[]>([]);
  const [ranking, setRanking] = useState<TreatmentRanking | null>(null);
  const [loading, setLoading] = useState(true);
  const rateCanvasRef = useRef<HTMLCanvasElement>(null);
  const rateChartRef = useRef<Chart | null>(null);

  useEffect(() => {
    api.studies().then(s => {
      setStudies(s);
      if (s.length) setSelected(s[0].study_id);
    });
  }, []);

  useEffect(() => {
    if (!selected) return;
    setLoading(true);
    Promise.all([
      api.stratumSummaries(selected),
      api.treatmentRankings(selected),
    ]).then(([sums, ranks]) => {
      setSummaries(sums);
      setRanking(ranks[0] ?? null);
    }).finally(() => setLoading(false));
  }, [selected]);

  useEffect(() => {
    if (!rateCanvasRef.current || !summaries.length || !ranking) return;
    if (rateChartRef.current) rateChartRef.current.destroy();

    const strataRows = summaries.filter(
      s => s.treatment_label === ranking.treatment_a
    );
    const labels = strataRows.map(s => s.stratum_label);

    rateChartRef.current = new Chart(rateCanvasRef.current, {
      type: 'bar',
      data: {
        labels,
        datasets: [
          {
            label: `Treatment ${ranking.treatment_a} (per stratum)`,
            data: strataRows.map(s => Number(s.stratum_rate_a)),
            backgroundColor: '#58a6ff99',
            borderColor: '#58a6ff',
            borderWidth: 1,
          },
          {
            label: `Treatment ${ranking.treatment_b} (per stratum)`,
            data: strataRows.map(s => Number(s.stratum_rate_b)),
            backgroundColor: '#d2a8ff99',
            borderColor: '#d2a8ff',
            borderWidth: 1,
          },
          {
            label: `${ranking.treatment_a} pooled`,
            data: Array(labels.length).fill(Number(ranking.pooled_rate_a)),
            type: 'line',
            borderColor: '#58a6ff',
            borderDash: [6, 3],
            borderWidth: 2,
            pointRadius: 0,
          },
          {
            label: `${ranking.treatment_b} pooled`,
            data: Array(labels.length).fill(Number(ranking.pooled_rate_b)),
            type: 'line',
            borderColor: '#d2a8ff',
            borderDash: [6, 3],
            borderWidth: 2,
            pointRadius: 0,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          y: {
            min: 0, max: 1,
            ticks: { color: '#8b949e', callback: v => `${(Number(v) * 100).toFixed(0)}%` },
            grid: { color: '#21262d' },
          },
          x: { ticks: { color: '#8b949e' }, grid: { color: '#21262d' } },
        },
        plugins: {
          legend: { labels: { color: '#8b949e', font: { size: 12 } } },
          tooltip: {
            callbacks: { label: ctx => `${ctx.dataset.label}: ${pct(ctx.parsed.y as number)}` },
          },
        },
      },
    });
  }, [summaries, ranking]);

  return (
    <div>
      <div className="page-title">Stratum Breakdown</div>
      <div className="page-desc">
        Per-stratum success rates for each treatment (bars) vs pooled rates (dashed lines).
        When bars unanimously favour one treatment but the dashed lines cross — that's the
        paradox made visual.
      </div>

      <StudySelector studies={studies} selected={selected} onChange={s => { setSelected(s); }} />

      {loading && <div className="loading">Loading…</div>}

      {!loading && ranking && (
        <>
          <div className={`policy-banner ${policyClass(ranking.policy_implication)}`}>
            <strong>Policy implication:</strong>{' '}
            <TrCell col="policy_implication">{ranking.policy_implication}</TrCell>
          </div>

          <div className="card">
            <h3>Per-stratum rates vs pooled</h3>
            <div style={{ height: 280 }}>
              <canvas ref={rateCanvasRef} />
            </div>
          </div>

          <div className="two-col">
            <div className="card">
              <h3>Reversal status</h3>
              <div className="stat-row">
                <span className="stat-label">Distortion type</span>
                <span className="stat-value">Type <TrCell col="distortion_type">{ranking.distortion_type}</TrCell></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">IsReversal (unanimity)</span>
                <span className="stat-value" style={{ color: ranking.is_reversal ? '#ff7b72' : '#7ee787' }}>
                  <TrCell col="is_reversal">{ranking.is_reversal ? 'YES' : 'no'}</TrCell>
                </span>
              </div>
              <div className="stat-row">
                <span className="stat-label">IsSignFlip</span>
                <span className="stat-value" style={{ color: ranking.is_sign_flip ? '#d2a8ff' : '#7ee787' }}>
                  <TrCell col="is_sign_flip">{ranking.is_sign_flip ? 'YES' : 'no'}</TrCell>
                </span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Reversal intensity</span>
                <span className="stat-value"><TrCell col="reversal_intensity">{(Number(ranking.reversal_intensity) * 100).toFixed(0)}%</TrCell></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Paradox strength</span>
                <span className="stat-value"><TrCell col="paradox_strength">{Number(ranking.paradox_strength).toFixed(4)}</TrCell></span>
              </div>
            </div>

            <div className="card">
              <h3>Pooled rates</h3>
              <div className="stat-row">
                <span className="stat-label">Treatment {ranking.treatment_a} pooled</span>
                <span className="stat-value"><TrCell col="pooled_rate_a">{pct(ranking.pooled_rate_a)}</TrCell></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Treatment {ranking.treatment_b} pooled</span>
                <span className="stat-value"><TrCell col="pooled_rate_b">{pct(ranking.pooled_rate_b)}</TrCell></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Pooled winner</span>
                <span className="stat-value"><TrCell col="pooled_winner">{ranking.pooled_winner}</TrCell></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Per-stratum winner</span>
                <span className="stat-value"><TrCell col="per_stratum_winner">{ranking.per_stratum_winner}</TrCell></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Strata count</span>
                <span className="stat-value"><TrCell col="stratum_count">{ranking.stratum_count}</TrCell></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Strata won by {ranking.treatment_a} / {ranking.treatment_b}</span>
                <span className="stat-value">
                  <TrCell col="strata_won_by_a">{ranking.strata_won_by_a}</TrCell>
                  {' / '}
                  <TrCell col="strata_won_by_b">{ranking.strata_won_by_b}</TrCell>
                </span>
              </div>
            </div>
          </div>

          <div className="card">
            <h3>Per-stratum detail</h3>
            <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
              <thead>
                <tr style={{ borderBottom: '1px solid #30363d', color: '#8b949e' }}>
                  <th style={{ textAlign: 'left', padding: '6px 8px' }}>Stratum</th>
                  <th style={{ textAlign: 'right', padding: '6px 8px' }}>Rate A</th>
                  <th style={{ textAlign: 'right', padding: '6px 8px' }}>Rate B</th>
                  <th style={{ textAlign: 'right', padding: '6px 8px' }}>Winner</th>
                  <th style={{ textAlign: 'right', padding: '6px 8px' }}>Stratum %</th>
                  <th style={{ textAlign: 'right', padding: '6px 8px' }}>Alloc bias</th>
                </tr>
              </thead>
              <tbody>
                {summaries
                  .filter(s => s.treatment_label === ranking.treatment_a)
                  .map(s => (
                    <tr key={s.stratum_summary_id} style={{ borderBottom: '1px solid #21262d' }}>
                      <td style={{ padding: '6px 8px' }}>{s.stratum_label}</td>
                      <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                        <SsCell col="stratum_rate_a">{pct(s.stratum_rate_a)}</SsCell>
                      </td>
                      <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                        <SsCell col="stratum_rate_b">{pct(s.stratum_rate_b)}</SsCell>
                      </td>
                      <td style={{ padding: '6px 8px', textAlign: 'right', color: '#58a6ff' }}>
                        <SsCell col="stratum_winner">{s.stratum_winner}</SsCell>
                      </td>
                      <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                        <SsCell col="stratum_fraction">{pct(s.stratum_fraction)}</SsCell>
                      </td>
                      <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums',
                        color: Number(s.allocation_bias) < 0 ? '#ff7b72' : '#7ee787' }}>
                        <SsCell col="allocation_bias">{Number(s.allocation_bias).toFixed(3)}</SsCell>
                      </td>
                    </tr>
                  ))}
              </tbody>
            </table>
          </div>
        </>
      )}
      <ViewDagScan ready={!loading && !!ranking} deps={[selected, summaries, ranking]} />
    </div>
  );
}
