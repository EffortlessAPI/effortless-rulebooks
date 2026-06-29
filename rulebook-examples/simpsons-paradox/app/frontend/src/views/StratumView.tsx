import { useEffect, useRef, useState } from 'react';
import { Chart } from 'chart.js/auto';
import { api } from '../api';
import { DagValue } from '../components/DagValue';
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

    // One data point per stratum (use TreatmentA sentinel rows only)
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
          <div
            className={`policy-banner ${policyClass(ranking.policy_implication)}`}
          >
            <strong>Policy implication:</strong>{' '}
            <DagValue table="TreatmentRankings" field="PolicyImplication">{ranking.policy_implication}</DagValue>
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
                <span className="stat-value">Type <DagValue table="TreatmentRankings" field="DistortionType">{ranking.distortion_type}</DagValue></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">IsReversal (unanimity)</span>
                <span className="stat-value" style={{ color: ranking.is_reversal ? '#ff7b72' : '#7ee787' }}>
                  {ranking.is_reversal ? 'YES' : 'no'}
                </span>
              </div>
              <div className="stat-row">
                <span className="stat-label">IsSignFlip</span>
                <span className="stat-value" style={{ color: ranking.is_sign_flip ? '#d2a8ff' : '#7ee787' }}>
                  {ranking.is_sign_flip ? 'YES' : 'no'}
                </span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Reversal intensity</span>
                <span className="stat-value">{(Number(ranking.reversal_intensity) * 100).toFixed(0)}%</span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Paradox strength</span>
                <span className="stat-value">{Number(ranking.paradox_strength).toFixed(4)}</span>
              </div>
            </div>

            <div className="card">
              <h3>Pooled rates</h3>
              <div className="stat-row">
                <span className="stat-label">Treatment {ranking.treatment_a} pooled</span>
                <span className="stat-value"><DagValue table="TreatmentRankings" field="PooledRateA">{pct(ranking.pooled_rate_a)}</DagValue></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Treatment {ranking.treatment_b} pooled</span>
                <span className="stat-value"><DagValue table="TreatmentRankings" field="PooledRateB">{pct(ranking.pooled_rate_b)}</DagValue></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Pooled winner</span>
                <span className="stat-value">{ranking.pooled_winner}</span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Per-stratum winner</span>
                <span className="stat-value">{ranking.per_stratum_winner}</span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Strata count</span>
                <span className="stat-value">{ranking.stratum_count}</span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Strata won by {ranking.treatment_a} / {ranking.treatment_b}</span>
                <span className="stat-value">{ranking.strata_won_by_a} / {ranking.strata_won_by_b}</span>
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
                        <DagValue table="StratumSummaries" field="StratumRateA">{pct(s.stratum_rate_a)}</DagValue>
                      </td>
                      <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                        {pct(s.stratum_rate_b)}
                      </td>
                      <td style={{ padding: '6px 8px', textAlign: 'right', color: '#58a6ff' }}>
                        {s.stratum_winner}
                      </td>
                      <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                        {pct(s.stratum_fraction)}
                      </td>
                      <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums',
                        color: Number(s.allocation_bias) < 0 ? '#ff7b72' : '#7ee787' }}>
                        <DagValue table="StratumSummaries" field="AllocationBias">{Number(s.allocation_bias).toFixed(3)}</DagValue>
                      </td>
                    </tr>
                  ))}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  );
}
