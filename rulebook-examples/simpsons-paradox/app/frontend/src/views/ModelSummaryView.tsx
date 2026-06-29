import { useEffect, useState } from 'react';
import { api } from '../api';
import { DagValue } from '../components/DagValue';
import type { ModelSummary, TreatmentRanking } from '../types';

function num(n: number | null, dp = 4) {
  return n == null ? '—' : Number(n).toFixed(dp);
}

export function ModelSummaryView() {
  const [summary, setSummary] = useState<ModelSummary | null>(null);
  const [rankings, setRankings] = useState<TreatmentRanking[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([api.modelSummary(), api.treatmentRankings()])
      .then(([s, r]) => { setSummary(s); setRankings(r); })
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="loading">Loading…</div>;
  if (!summary) return <div className="error">No model summary found.</div>;

  return (
    <div>
      <div className="page-title">Model Summary</div>
      <div className="page-desc">
        Rollup across all studies — how many exhibit each distortion type and the average
        paradox strength. IsReversal is the sign-flip criterion (same as IsSignFlip).
        Type A vs Type B captures unanimous vs partial stratum disagreement.
        SignalPurity below 0.5 means allocation noise exceeds true signal on average.
      </div>

      <div className="three-col" style={{ marginBottom: 16 }}>
        <div className="card" style={{ textAlign: 'center' }}>
          <div style={{ fontSize: 36, fontWeight: 700, color: '#ff7b72' }}>
            <DagValue table="ModelSummary" field="ReversalCount">{summary.reversal_count}</DagValue>
          </div>
          <div style={{ color: '#8b949e', fontSize: 13, marginTop: 4 }}>Sign-flip reversals</div>
        </div>
        <div className="card" style={{ textAlign: 'center' }}>
          <div style={{ fontSize: 36, fontWeight: 700, color: '#d2a8ff' }}>
            <DagValue table="ModelSummary" field="TypeBCount">{summary.type_b_count}</DagValue>
          </div>
          <div style={{ color: '#8b949e', fontSize: 13, marginTop: 4 }}>Type B (partial stratum)</div>
        </div>
        <div className="card" style={{ textAlign: 'center' }}>
          <div style={{ fontSize: 36, fontWeight: 700, color: '#58a6ff' }}>
            <DagValue table="ModelSummary" field="StudyCount">{summary.study_count}</DagValue>
          </div>
          <div style={{ color: '#8b949e', fontSize: 13, marginTop: 4 }}>Total studies</div>
        </div>
      </div>

      <div className="two-col">
        <div className="card">
          <h3>Distortion type distribution</h3>
          {(['A', 'B', 'C', 'D'] as const).map(t => {
            const count = t === 'C'
              ? Number(summary.type_c_plus_count ?? 0) + Number(summary.type_c_minus_count ?? 0)
              : summary[`type_${t.toLowerCase()}_count` as keyof ModelSummary] as number;
            const pct = summary.study_count ? Math.round(count / summary.study_count * 100) : 0;
            return (
              <div key={t} style={{ marginBottom: 12 }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}>
                  <span>
                    <span className={`badge badge-type-${t.toLowerCase()}`}>Type {t}</span>
                    &nbsp;&nbsp;
                    <span style={{ fontSize: 12, color: '#8b949e' }}>
                      {t === 'A' ? 'Unanimous sign flip' :
                        t === 'B' ? 'Sign flip, partial stratum' :
                          t === 'C' ? 'Distortion, no sign flip' : 'Neutral / trustworthy'}
                    </span>
                  </span>
                  <span style={{ fontWeight: 600 }}>{count}</span>
                </div>
                <div style={{ background: '#21262d', borderRadius: 4, height: 6 }}>
                  <div style={{
                    height: '100%', borderRadius: 4,
                    width: `${pct}%`,
                    background: t === 'A' ? '#ff7b72' : t === 'B' ? '#d2a8ff' : t === 'C' ? '#7ee787' : '#6e7681',
                  }} />
                </div>
              </div>
            );
          })}
        </div>

        <div className="card">
          <h3>Continuous measures</h3>
          <div className="stat-row">
            <span className="stat-label">Average paradox strength</span>
            <span className="stat-value"><DagValue table="ModelSummary" field="AvgParadoxStrength">{num(summary.avg_paradox_strength)}</DagValue></span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Avg signal purity</span>
            <span className="stat-value"><DagValue table="ModelSummary" field="AvgSignalPurity">{num(summary.avg_signal_purity)}</DagValue></span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Avg purity (reversal)</span>
            <span className="stat-value">{num(summary.avg_signal_purity_reversal)}</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Avg purity (non-reversal)</span>
            <span className="stat-value">{num(summary.avg_signal_purity_non_reversal)}</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Signal purity gap</span>
            <span className="stat-value">{num(summary.signal_purity_gap)}</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Total paradox strength</span>
            <span className="stat-value">{num(summary.total_paradox_strength)}</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Explained (reversal + confounder)</span>
            <span className="stat-value">{summary.explained_count}</span>
          </div>
        </div>
      </div>

      <div className="card">
        <h3>All studies — full ranking table</h3>
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 12 }}>
          <thead>
            <tr style={{ borderBottom: '1px solid #30363d', color: '#8b949e' }}>
              <th style={{ textAlign: 'left', padding: '6px 6px' }}>Study</th>
              <th style={{ textAlign: 'center', padding: '6px 6px' }}>Type</th>
              <th style={{ textAlign: 'center', padding: '6px 6px' }}>Tier</th>
              <th style={{ textAlign: 'center', padding: '6px 6px' }}>Reversal</th>
              <th style={{ textAlign: 'right', padding: '6px 6px' }}>Strength</th>
              <th style={{ textAlign: 'right', padding: '6px 6px' }}>Signal purity</th>
              <th style={{ textAlign: 'right', padding: '6px 6px' }}>Intensity</th>
              <th style={{ textAlign: 'right', padding: '6px 6px' }}>Distortion</th>
            </tr>
          </thead>
          <tbody>
            {rankings.map(r => (
              <tr key={r.treatment_ranking_id} style={{ borderBottom: '1px solid #21262d' }}>
                <td style={{ padding: '6px 6px' }}>{r.study}</td>
                <td style={{ padding: '6px 6px', textAlign: 'center' }}>
                  <span className={`badge badge-type-${r.distortion_type?.toLowerCase()}`}>
                    <DagValue table="TreatmentRankings" field="DistortionType">{r.distortion_type}</DagValue>
                  </span>
                </td>
                <td style={{ padding: '6px 6px', textAlign: 'center', fontSize: 11 }}>{r.screening_tier ?? '—'}</td>
                <td style={{ padding: '6px 6px', textAlign: 'center',
                  color: r.is_reversal ? '#ff7b72' : '#6e7681' }}>
                  {r.is_reversal ? 'YES' : 'no'}
                </td>
                <td style={{ padding: '6px 6px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                  <DagValue table="TreatmentRankings" field="ParadoxStrength">{num(r.paradox_strength)}</DagValue>
                </td>
                <td style={{ padding: '6px 6px', textAlign: 'right', fontVariantNumeric: 'tabular-nums', color: r.signal_purity != null && r.signal_purity < 0.5 ? '#ff7b72' : '#7ee787' }}>
                  <DagValue table="TreatmentRankings" field="SignalPurity">{num(r.signal_purity, 3)}</DagValue>
                </td>
                <td style={{ padding: '6px 6px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                  {(Number(r.reversal_intensity) * 100).toFixed(0)}%
                </td>
                <td style={{ padding: '6px 6px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                  <DagValue table="TreatmentRankings" field="AllocationDistortion">{num(r.allocation_distortion)}</DagValue>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
