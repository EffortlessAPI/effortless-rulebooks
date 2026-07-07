import { useEffect, useState } from 'react';
import { api } from '../api';
import { ViewDagScan } from '../components/DagValue';
import { TrCell, TrTierPill, TrTypeBadge } from '../components/dag-display';
import type { TreatmentRanking } from '../types';

const TIER_STYLES: Record<string, { color: string; bg: string; border: string }> = {
  DANGER:  { color: '#ff7b72', bg: '#2d1515', border: '#6e3030' },
  CAUTION: { color: '#e3b341', bg: '#2a2210', border: '#5c4a1a' },
  SAFE:    { color: '#7ee787', bg: '#152a1a', border: '#1a3d2a' },
};

function ratio(n: number | null | undefined): string {
  if (n == null) return '—';
  const v = Number(n);
  return (v >= 0 ? '+' : '') + v.toFixed(3);
}

function pct(n: number | null | undefined): string {
  if (n == null) return '—';
  return (Number(n) * 100).toFixed(1) + '%';
}

export function InstrumentDashboardView() {
  const [rows, setRows] = useState<TreatmentRanking[]>([]);
  const [loading, setLoading] = useState(true);
  const [sortCol, setSortCol] = useState<'distortion_ratio' | 'allocation_distortion' | 'study'>('distortion_ratio');
  const [sortDir, setSortDir] = useState<'asc' | 'desc'>('desc');

  useEffect(() => {
    api.treatmentRankings().then(setRows).finally(() => setLoading(false));
  }, []);

  function toggleSort(col: typeof sortCol) {
    if (sortCol === col) setSortDir(d => d === 'asc' ? 'desc' : 'asc');
    else { setSortCol(col); setSortDir('desc'); }
  }

  const sorted = [...rows].sort((a, b) => {
    const va = sortCol === 'study' ? a.study : Number(a[sortCol] ?? 0);
    const vb = sortCol === 'study' ? b.study : Number(b[sortCol] ?? 0);
    if (typeof va === 'string') return sortDir === 'asc' ? va.localeCompare(vb as string) : (vb as string).localeCompare(va);
    return sortDir === 'asc' ? (va as number) - (vb as number) : (vb as number) - (va as number);
  });

  const tierCounts = { DANGER: 0, CAUTION: 0, SAFE: 0 } as Record<string, number>;
  rows.forEach(r => { if (r.screening_tier) tierCounts[r.screening_tier]++; });

  const thStyle = (col: typeof sortCol): React.CSSProperties => ({
    padding: '6px 10px',
    textAlign: 'left',
    cursor: 'pointer',
    userSelect: 'none',
    color: sortCol === col ? '#e6edf3' : '#8b949e',
    fontWeight: 600,
    fontSize: 12,
    borderBottom: '2px solid #30363d',
    whiteSpace: 'nowrap',
  });

  if (loading) return <div className="loading">Loading…</div>;

  return (
    <div>
      <div className="page-title">Instrument Dashboard</div>
      <div className="page-desc">
        Screening tier for each study from DistortionType.
        <strong style={{ color: '#ff7b72' }}> DANGER</strong> = pooled sign is wrong (Types A &amp; B).
        <strong style={{ color: '#e3b341' }}> CAUTION</strong> = direction correct, magnitude distorted (Type C).
        <strong style={{ color: '#7ee787' }}> SAFE</strong> = pooled trustworthy (Type D).
      </div>

      <div style={{ display: 'flex', gap: 12, marginBottom: 16, flexWrap: 'wrap' }}>
        {(['DANGER', 'CAUTION', 'SAFE'] as const).map(tier => {
          const s = TIER_STYLES[tier];
          return (
            <div
              key={tier}
              style={{
                padding: '10px 18px',
                borderRadius: 8,
                background: s.bg,
                color: s.color,
                border: `1px solid ${s.border}`,
                fontWeight: 700,
                fontSize: 14,
              }}
            >
              {tier}: {tierCounts[tier]} / {rows.length}
            </div>
          );
        })}
      </div>

      <div className="card">
        <h3>Study screening table</h3>
        <div style={{ overflowX: 'auto' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
            <thead>
              <tr>
                <th style={thStyle('study')} onClick={() => toggleSort('study')}>
                  Study {sortCol === 'study' ? (sortDir === 'asc' ? '▲' : '▼') : ''}
                </th>
                <th style={{ ...thStyle('study'), cursor: 'default' }}>Type</th>
                <th style={{ ...thStyle('study'), cursor: 'default' }}>Tier</th>
                <th style={thStyle('distortion_ratio')} onClick={() => toggleSort('distortion_ratio')}>
                  DistortionRatio {sortCol === 'distortion_ratio' ? (sortDir === 'asc' ? '▲' : '▼') : ''}
                </th>
                <th style={thStyle('allocation_distortion')} onClick={() => toggleSort('allocation_distortion')}>
                  AllocDistortion {sortCol === 'allocation_distortion' ? (sortDir === 'asc' ? '▲' : '▼') : ''}
                </th>
                <th style={{ ...thStyle('study'), cursor: 'default' }}>CorrectedGap</th>
                <th style={{ ...thStyle('study'), cursor: 'default' }}>PooledGap</th>
                <th style={{ ...thStyle('study'), cursor: 'default' }}>Policy</th>
              </tr>
            </thead>
            <tbody>
              {sorted.map(r => {
                const tier = r.screening_tier;
                const tierStyle = tier ? TIER_STYLES[tier] : null;
                const dr = Number(r.distortion_ratio);
                return (
                  <tr key={r.study} style={{ borderBottom: '1px solid #21262d' }}>
                    <td style={{ padding: '7px 10px', fontWeight: 500 }}>{r.study}</td>
                    <td style={{ padding: '7px 10px', textAlign: 'center' }}>
                      <TrTypeBadge type={r.distortion_type} />
                    </td>
                    <td style={{ padding: '7px 10px', textAlign: 'center' }}>
                      <TrTierPill tier={tier} />
                    </td>
                    <td style={{
                      padding: '7px 10px',
                      textAlign: 'right',
                      fontFamily: 'monospace',
                      color: dr < -1 ? '#ff7b72' : dr < 0 ? '#d2a8ff' : dr === 1 ? '#7ee787' : '#8b949e',
                      fontWeight: 600,
                    }}>
                      <TrCell col="distortion_ratio">{ratio(r.distortion_ratio)}</TrCell>
                    </td>
                    <td style={{ padding: '7px 10px', textAlign: 'right', fontFamily: 'monospace', fontVariantNumeric: 'tabular-nums' }}>
                      <TrCell col="allocation_distortion">{Number(r.allocation_distortion).toFixed(4)}</TrCell>
                    </td>
                    <td style={{ padding: '7px 10px', textAlign: 'right', color: '#7ee787', fontFamily: 'monospace', fontVariantNumeric: 'tabular-nums' }}>
                      <TrCell col="weighted_stratum_gap_sum">{pct(r.weighted_stratum_gap_sum)}</TrCell>
                    </td>
                    <td style={{ padding: '7px 10px', textAlign: 'right', fontFamily: 'monospace', fontVariantNumeric: 'tabular-nums' }}>
                      <TrCell col="signed_pooled_gap">{pct(r.signed_pooled_gap)}</TrCell>
                    </td>
                    <td style={{
                      padding: '7px 10px',
                      fontSize: 11,
                      color: tierStyle?.color ?? '#8b949e',
                      maxWidth: 240,
                    }}>
                      <TrCell col="policy_implication">{r.policy_implication}</TrCell>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </div>

      <div className="card" style={{ fontSize: 13, lineHeight: 1.5 }}>
        <strong>Distortion ratio:</strong>{' '}
        <code style={{
          background: '#21262d',
          color: '#79c0ff',
          padding: '2px 6px',
          borderRadius: 3,
          border: '1px solid #30363d',
        }}>
          DistortionRatio = SignedPooledGap / CorrectedGap
        </code>
        <div className="page-desc" style={{ marginTop: 8, marginBottom: 0 }}>
          Ratio &lt; −1 → Type B · Ratio ∈ (−1, 0) → Type A ·
          Ratio &gt; 0 → Type C/D · Ratio = 1 → Type D (safe)
        </div>
      </div>
      <ViewDagScan ready={!loading} deps={[rows, sortCol, sortDir]} />
    </div>
  );
}
