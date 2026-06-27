import { useEffect, useState } from 'react';
import { api } from '../api';
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
        Rollup across all studies — how many exhibit each distortion type, the average
        paradox strength, and which studies the unanimity vs sign-flip definitions disagree on.
      </div>

      <div className="three-col" style={{ marginBottom: 16 }}>
        <div className="card" style={{ textAlign: 'center' }}>
          <div style={{ fontSize: 36, fontWeight: 700, color: '#ff7b72' }}>{summary.reversal_count}</div>
          <div style={{ color: '#8b949e', fontSize: 13, marginTop: 4 }}>Full reversals (unanimity)</div>
        </div>
        <div className="card" style={{ textAlign: 'center' }}>
          <div style={{ fontSize: 36, fontWeight: 700, color: '#d2a8ff' }}>{summary.type_b_count}</div>
          <div style={{ color: '#8b949e', fontSize: 13, marginTop: 4 }}>Sign-flip only (v2 extends)</div>
        </div>
        <div className="card" style={{ textAlign: 'center' }}>
          <div style={{ fontSize: 36, fontWeight: 700, color: '#58a6ff' }}>{summary.study_count}</div>
          <div style={{ color: '#8b949e', fontSize: 13, marginTop: 4 }}>Total studies</div>
        </div>
      </div>

      <div className="two-col">
        <div className="card">
          <h3>Distortion type distribution</h3>
          {(['A', 'B', 'C', 'D'] as const).map(t => {
            const count = summary[`type_${t.toLowerCase()}_count` as keyof ModelSummary] as number;
            const pct = summary.study_count ? Math.round(count / summary.study_count * 100) : 0;
            return (
              <div key={t} style={{ marginBottom: 12 }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}>
                  <span>
                    <span className={`badge badge-type-${t.toLowerCase()}`}>Type {t}</span>
                    &nbsp;&nbsp;
                    <span style={{ fontSize: 12, color: '#8b949e' }}>
                      {t === 'A' ? 'Full reversal + sign flip' :
                        t === 'B' ? 'Sign flip, partial reversal' :
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
            <span className="stat-value">{num(summary.avg_paradox_strength)}</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Total paradox strength</span>
            <span className="stat-value">{num(summary.total_paradox_strength)}</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Explained (reversal + confounder)</span>
            <span className="stat-value">{summary.explained_count}</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Partial paradoxes</span>
            <span className="stat-value">{summary.partial_count}</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Zero paradox strength</span>
            <span className="stat-value">{summary.zero_strength_count}</span>
          </div>
        </div>
      </div>

      <div className="card">
        <h3>Definition delta — studies where unanimity ≠ sign-flip</h3>
        <div className="page-desc" style={{ marginBottom: 12 }}>
          These studies satisfy IsReversal_v2 (sign-flip criterion) but NOT IsReversal
          (unanimity criterion). They are the empirical proof that the unanimity criterion
          is a strict subset of the broader definition.
        </div>
        {rankings.filter(r => r.definition_delta).length === 0 ? (
          <div style={{ color: '#8b949e', fontSize: 13 }}>No definition delta in current dataset.</div>
        ) : (
          <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
            <thead>
              <tr style={{ borderBottom: '1px solid #30363d', color: '#8b949e' }}>
                <th style={{ textAlign: 'left', padding: '6px 8px' }}>Study</th>
                <th style={{ textAlign: 'center', padding: '6px 8px' }}>IsReversal</th>
                <th style={{ textAlign: 'center', padding: '6px 8px' }}>IsSignFlip</th>
                <th style={{ textAlign: 'center', padding: '6px 8px' }}>Type</th>
                <th style={{ textAlign: 'right', padding: '6px 8px' }}>Reversal intensity</th>
                <th style={{ textAlign: 'right', padding: '6px 8px' }}>Alloc distortion</th>
              </tr>
            </thead>
            <tbody>
              {rankings.filter(r => r.definition_delta).map(r => (
                <tr key={r.treatment_ranking_id} style={{ borderBottom: '1px solid #21262d' }}>
                  <td style={{ padding: '6px 8px' }}>{r.study}</td>
                  <td style={{ padding: '6px 8px', textAlign: 'center', color: '#6e7681' }}>no</td>
                  <td style={{ padding: '6px 8px', textAlign: 'center', color: '#d2a8ff' }}>YES</td>
                  <td style={{ padding: '6px 8px', textAlign: 'center' }}>
                    <span className={`badge badge-type-${r.distortion_type?.toLowerCase()}`}>
                      {r.distortion_type}
                    </span>
                  </td>
                  <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                    {(Number(r.reversal_intensity) * 100).toFixed(0)}%
                  </td>
                  <td style={{ padding: '6px 8px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                    {Number(r.allocation_distortion).toFixed(4)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <div className="card">
        <h3>All studies — full ranking table</h3>
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 12 }}>
          <thead>
            <tr style={{ borderBottom: '1px solid #30363d', color: '#8b949e' }}>
              <th style={{ textAlign: 'left', padding: '6px 6px' }}>Study</th>
              <th style={{ textAlign: 'center', padding: '6px 6px' }}>Type</th>
              <th style={{ textAlign: 'center', padding: '6px 6px' }}>Reversal</th>
              <th style={{ textAlign: 'center', padding: '6px 6px' }}>SignFlip</th>
              <th style={{ textAlign: 'right', padding: '6px 6px' }}>Strength</th>
              <th style={{ textAlign: 'right', padding: '6px 6px' }}>Intensity</th>
              <th style={{ textAlign: 'right', padding: '6px 6px' }}>Distortion</th>
              <th style={{ textAlign: 'left', padding: '6px 6px' }}>Subtype</th>
            </tr>
          </thead>
          <tbody>
            {rankings.map(r => (
              <tr key={r.treatment_ranking_id} style={{ borderBottom: '1px solid #21262d' }}>
                <td style={{ padding: '6px 6px' }}>{r.study}</td>
                <td style={{ padding: '6px 6px', textAlign: 'center' }}>
                  <span className={`badge badge-type-${r.distortion_type?.toLowerCase()}`}>{r.distortion_type}</span>
                </td>
                <td style={{ padding: '6px 6px', textAlign: 'center',
                  color: r.is_reversal ? '#ff7b72' : '#6e7681' }}>
                  {r.is_reversal ? 'YES' : 'no'}
                </td>
                <td style={{ padding: '6px 6px', textAlign: 'center',
                  color: r.is_sign_flip ? '#d2a8ff' : '#6e7681' }}>
                  {r.is_sign_flip ? 'YES' : 'no'}
                </td>
                <td style={{ padding: '6px 6px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                  {num(r.paradox_strength)}
                </td>
                <td style={{ padding: '6px 6px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                  {(Number(r.reversal_intensity) * 100).toFixed(0)}%
                </td>
                <td style={{ padding: '6px 6px', textAlign: 'right', fontVariantNumeric: 'tabular-nums' }}>
                  {num(r.allocation_distortion)}
                </td>
                <td style={{ padding: '6px 6px', color: '#8b949e', fontSize: 11 }}>
                  {r.strict_reversal_subtype}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
