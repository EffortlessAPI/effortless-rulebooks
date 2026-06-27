import { useEffect, useRef, useState } from 'react';
import { Chart } from 'chart.js/auto';
import { api } from '../api';
import type { Study, StratumSummary, TreatmentRanking } from '../types';
import { StudySelector } from '../components/StudySelector';

function pct(n: number | null) {
  return n == null ? '—' : (Number(n) * 100).toFixed(1) + '%';
}

export function WeightsView() {
  const [studies, setStudies] = useState<Study[]>([]);
  const [selected, setSelected] = useState('');
  const [summaries, setSummaries] = useState<StratumSummary[]>([]);
  const [ranking, setRanking] = useState<TreatmentRanking | null>(null);
  const [loading, setLoading] = useState(true);
  const canvasARef = useRef<HTMLCanvasElement>(null);
  const canvasBRef = useRef<HTMLCanvasElement>(null);
  const chartARef = useRef<Chart | null>(null);
  const chartBRef = useRef<Chart | null>(null);

  useEffect(() => {
    api.studies().then(s => {
      setStudies(s);
      if (s.length) setSelected(s[0].study_id);
    });
  }, []);

  useEffect(() => {
    if (!selected) return;
    setLoading(true);
    Promise.all([api.stratumSummaries(selected), api.treatmentRankings(selected)])
      .then(([sums, ranks]) => { setSummaries(sums); setRanking(ranks[0] ?? null); })
      .finally(() => setLoading(false));
  }, [selected]);

  function buildWeightChart(
    canvas: HTMLCanvasElement | null,
    chartRef: React.MutableRefObject<Chart | null>,
    rowsA: StratumSummary[],
    treatLabel: string,
    allocKey: 'allocation_fraction_a' | 'allocation_fraction_b',
    color: string
  ) {
    if (!canvas || !rowsA.length) return;
    if (chartRef.current) chartRef.current.destroy();
    const labels = rowsA.map(s => s.stratum_label);
    chartRef.current = new Chart(canvas, {
      type: 'bar',
      data: {
        labels,
        datasets: [
          {
            label: `Allocation fraction (${treatLabel})`,
            data: rowsA.map(s => Number(s[allocKey])),
            backgroundColor: color + '99',
            borderColor: color,
            borderWidth: 1,
            yAxisID: 'yAlloc',
          },
          {
            label: 'Stratum success rate',
            data: rowsA.map(s => Number(s.stratum_success_rate)),
            type: 'line',
            borderColor: '#f0a83088',
            borderWidth: 2,
            pointRadius: 4,
            yAxisID: 'yRate',
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          yAlloc: {
            type: 'linear',
            position: 'left',
            min: 0,
            title: { display: true, text: 'Alloc fraction', color: '#8b949e' },
            ticks: { color: '#8b949e', callback: v => `${(Number(v) * 100).toFixed(0)}%` },
            grid: { color: '#21262d' },
          },
          yRate: {
            type: 'linear',
            position: 'right',
            min: 0,
            max: 1,
            title: { display: true, text: 'Stratum rate', color: '#8b949e' },
            ticks: { color: '#8b949e', callback: v => `${(Number(v) * 100).toFixed(0)}%` },
            grid: { drawOnChartArea: false },
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
  }

  useEffect(() => {
    if (!summaries.length || !ranking) return;
    const rowsA = summaries.filter(s => s.treatment_label === ranking.treatment_a);
    const rowsB = summaries.filter(s => s.treatment_label === ranking.treatment_b);
    buildWeightChart(canvasARef.current, chartARef, rowsA, ranking.treatment_a, 'allocation_fraction_a', '#58a6ff');
    buildWeightChart(canvasBRef.current, chartBRef, rowsB, ranking.treatment_b, 'allocation_fraction_b', '#d2a8ff');
  }, [summaries, ranking]);

  return (
    <div>
      <div className="page-title">Allocation Weights</div>
      <div className="page-desc">
        How each treatment's cases are distributed across strata (bars) vs the per-stratum
        success rate (line). When a treatment concentrates in low-success strata, its pooled
        rate is dragged down — even if it wins in every stratum individually.
      </div>

      <StudySelector studies={studies} selected={selected} onChange={s => setSelected(s)} />

      {loading && <div className="loading">Loading…</div>}

      {!loading && ranking && (
        <>
          <div className="two-col">
            <div className="card">
              <h3>Treatment {ranking.treatment_a} allocation</h3>
              <div style={{ height: 260 }}><canvas ref={canvasARef} /></div>
            </div>
            <div className="card">
              <h3>Treatment {ranking.treatment_b} allocation</h3>
              <div style={{ height: 260 }}><canvas ref={canvasBRef} /></div>
            </div>
          </div>

          <div className="card">
            <h3>Allocation bias detail</h3>
            <div className="page-desc" style={{ marginBottom: 12 }}>
              AllocationBias = fractionA − fractionB per stratum. A negative value in a
              low-success stratum means Treatment A is under-represented in the hard cases —
              making A look worse in the pooled view than it actually is in any individual stratum.
            </div>
            <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
              <thead>
                <tr style={{ borderBottom: '1px solid #30363d', color: '#8b949e' }}>
                  <th style={{ textAlign: 'left', padding: '6px 8px' }}>Stratum</th>
                  <th style={{ textAlign: 'right', padding: '6px 8px' }}>Stratum rate</th>
                  <th style={{ textAlign: 'right', padding: '6px 8px' }}>Alloc A</th>
                  <th style={{ textAlign: 'right', padding: '6px 8px' }}>Alloc B</th>
                  <th style={{ textAlign: 'right', padding: '6px 8px' }}>Bias (A−B)</th>
                  <th style={{ textAlign: 'right', padding: '6px 8px' }}>Stratum %</th>
                  <th style={{ textAlign: 'right', padding: '6px 8px' }}>Wtd gap</th>
                </tr>
              </thead>
              <tbody>
                {summaries
                  .filter(s => s.treatment_label === ranking.treatment_a)
                  .map(s => {
                    const matchB = summaries.find(
                      b => b.stratum_label === s.stratum_label && b.treatment_label === ranking.treatment_b
                    );
                    return (
                      <tr key={s.stratum_summary_id} style={{ borderBottom: '1px solid #21262d' }}>
                        <td style={{ padding: '6px 8px' }}>{s.stratum_label}</td>
                        <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                          {pct(s.stratum_success_rate)}
                        </td>
                        <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums', color: '#58a6ff' }}>
                          {pct(s.allocation_fraction_a)}
                        </td>
                        <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums', color: '#d2a8ff' }}>
                          {matchB ? pct(matchB.allocation_fraction_b) : '—'}
                        </td>
                        <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums',
                          color: Number(s.allocation_bias) < 0 ? '#ff7b72' : '#7ee787' }}>
                          {Number(s.allocation_bias).toFixed(3)}
                        </td>
                        <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                          {pct(s.stratum_fraction)}
                        </td>
                        <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                          {Number(s.weighted_stratum_gap).toFixed(4)}
                        </td>
                      </tr>
                    );
                  })}
              </tbody>
            </table>
          </div>

          <div className="card">
            <h3>Mechanism equation check</h3>
            <div className="stat-row">
              <span className="stat-label">Pooled rate A (direct)</span>
              <span className="stat-value">{pct(ranking.pooled_rate_a)}</span>
            </div>
            <div className="stat-row">
              <span className="stat-label">Pooled rate A (from weights)</span>
              <span className="stat-value">{pct((ranking as unknown as Record<string, number>).pooled_rate_from_weights_a)}</span>
            </div>
            <div className="stat-row">
              <span className="stat-label">WeightedStratumGapSum (equal-weight signal)</span>
              <span className="stat-value">{Number(ranking.weighted_stratum_gap_sum).toFixed(4)}</span>
            </div>
            <div className="stat-row">
              <span className="stat-label">SignedPooledGap (actual signal)</span>
              <span className="stat-value">{Number(ranking.signed_pooled_gap).toFixed(4)}</span>
            </div>
            <div className="stat-row">
              <span className="stat-label">AllocationDistortion</span>
              <span className="stat-value">{Number(ranking.allocation_distortion).toFixed(4)}</span>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
