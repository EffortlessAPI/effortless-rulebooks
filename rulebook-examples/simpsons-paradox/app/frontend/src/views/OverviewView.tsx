import { useEffect, useRef, useState } from 'react';
import { Chart } from 'chart.js/auto';
import { api } from '../api';
import type { Study, TreatmentRanking } from '../types';

function typeBadge(r: TreatmentRanking) {
  const cls = `badge badge-type-${r.distortion_type?.toLowerCase() ?? 'd'}`;
  return <span className={cls}>Type {r.distortion_type}</span>;
}

function pct(n: number | null) {
  if (n == null) return '—';
  return (Number(n) * 100).toFixed(1) + '%';
}

function num(n: number | null, dp = 3) {
  if (n == null) return '—';
  return Number(n).toFixed(dp);
}

export function OverviewView() {
  const [studies, setStudies] = useState<Study[]>([]);
  const [rankings, setRankings] = useState<TreatmentRanking[]>([]);
  const [loading, setLoading] = useState(true);
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const chartRef = useRef<Chart | null>(null);

  useEffect(() => {
    Promise.all([api.studies(), api.treatmentRankings()])
      .then(([s, r]) => { setStudies(s); setRankings(r); })
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    if (!canvasRef.current || !rankings.length) return;
    if (chartRef.current) chartRef.current.destroy();

    const typeColors: Record<string, string> = {
      A: '#ff7b72', B: '#d2a8ff', C: '#7ee787', D: '#6e7681',
    };

    const datasets = ['A', 'B', 'C', 'D'].map(t => ({
      label: `Type ${t}`,
      data: rankings
        .filter(r => r.distortion_type === t)
        .map(r => ({
          x: Number(r.allocation_distortion),
          y: Number(r.paradox_strength),
          label: r.study,
        })),
      backgroundColor: typeColors[t] + 'cc',
      pointRadius: 8,
      pointHoverRadius: 10,
    }));

    chartRef.current = new Chart(canvasRef.current, {
      type: 'scatter',
      data: { datasets },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { labels: { color: '#8b949e', font: { size: 12 } } },
          tooltip: {
            callbacks: {
              label: ctx => {
                const d = ctx.raw as { x: number; y: number; label: string };
                return `${d.label}  distortion=${d.x.toFixed(3)}  strength=${d.y.toFixed(3)}`;
              },
            },
          },
        },
        scales: {
          x: {
            title: { display: true, text: 'Allocation Distortion', color: '#8b949e' },
            ticks: { color: '#8b949e' },
            grid: { color: '#21262d' },
          },
          y: {
            title: { display: true, text: 'Paradox Strength', color: '#8b949e' },
            ticks: { color: '#8b949e' },
            grid: { color: '#21262d' },
          },
        },
      },
    });
  }, [rankings]);

  if (loading) return <div className="loading">Loading…</div>;

  const rankByStudy = Object.fromEntries(rankings.map(r => [r.study, r]));

  return (
    <div>
      <div className="page-title">Study Overview</div>
      <div className="page-desc">
        Each study plotted on the allocation-distortion plane. X axis = how far allocation
        bends the pooled signal; Y axis = paradox strength (pooled gap × reversal intensity).
        Type A = full sign-flip reversal · B = partial sign-flip · C = distortion without
        flip · D = neutral.
      </div>

      <div className="card" style={{ marginBottom: 20 }}>
        <h3>Allocation distortion plane</h3>
        <div style={{ height: 320 }}>
          <canvas ref={canvasRef} />
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))', gap: 14 }}>
        {studies.map(s => {
          const r = rankByStudy[s.study_id];
          return (
            <div className="card" key={s.study_id}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 8 }}>
                <div>
                  <div style={{ fontWeight: 600, fontSize: 14 }}>{s.title || s.name}</div>
                  <div style={{ color: '#8b949e', fontSize: 12, marginTop: 2 }}>{s.source}</div>
                </div>
                {r && typeBadge(r)}
              </div>
              {r && <>
                <div className="stat-row">
                  <span className="stat-label">Pooled winner</span>
                  <span className="stat-value">{r.pooled_winner}</span>
                </div>
                <div className="stat-row">
                  <span className="stat-label">Per-stratum winner</span>
                  <span className="stat-value">{r.per_stratum_winner}</span>
                </div>
                <div className="stat-row">
                  <span className="stat-label">IsReversal (unanimity)</span>
                  <span className="stat-value" style={{ color: r.is_reversal ? '#ff7b72' : '#7ee787' }}>
                    {r.is_reversal ? 'YES' : 'no'}
                  </span>
                </div>
                <div className="stat-row">
                  <span className="stat-label">IsReversal v2 (sign-flip)</span>
                  <span className="stat-value" style={{ color: r.is_reversal_v2 ? '#d2a8ff' : '#7ee787' }}>
                    {r.is_reversal_v2 ? 'YES' : 'no'}
                  </span>
                </div>
                <div className="stat-row">
                  <span className="stat-label">Allocation distortion</span>
                  <span className="stat-value">{num(r.allocation_distortion)}</span>
                </div>
                <div className="stat-row">
                  <span className="stat-label">Paradox strength</span>
                  <span className="stat-value">{num(r.paradox_strength)}</span>
                </div>
                <div className="stat-row">
                  <span className="stat-label">Pooled rate A / B</span>
                  <span className="stat-value">{pct(r.pooled_rate_a)} / {pct(r.pooled_rate_b)}</span>
                </div>
              </>}
            </div>
          );
        })}
      </div>
    </div>
  );
}
