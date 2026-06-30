import { useEffect, useState } from 'react';
import { api } from '../api';
import { ViewDagScan } from '../components/DagValue';
import { Cell, TrCell, TrTierPill, TrTypeBadge } from '../components/dag-display';
import type { TreatmentRanking } from '../types';

const TIER_COLORS: Record<string, { bg: string; text: string; border: string }> = {
  DANGER:  { bg: '#fff0f0', text: '#c00', border: '#fcc' },
  CAUTION: { bg: '#fffbe6', text: '#854d0e', border: '#fde68a' },
  SAFE:    { bg: '#f0fff4', text: '#166534', border: '#bbf7d0' },
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
    padding: '6px 10px', textAlign: 'left', cursor: 'pointer', userSelect: 'none',
    background: sortCol === col ? '#e8eaf0' : '#f4f4f6', fontWeight: 600, fontSize: 12,
    borderBottom: '2px solid #ddd', whiteSpace: 'nowrap',
  });

  if (loading) return <div style={{ padding: '2rem', color: '#888' }}>Loading…</div>;

  return (
    <div style={{ padding: '1.5rem', maxWidth: 1000 }}>
      <h2 style={{ margin: '0 0 .25rem' }}>Instrument Dashboard</h2>
      <p style={{ color: '#666', fontSize: 14, margin: '0 0 1.25rem' }}>
        Screening tier for each study from DistortionType.
        <strong style={{ color: '#c00' }}> DANGER</strong> = pooled sign is wrong (Types A & B).
        <strong style={{ color: '#854d0e' }}> CAUTION</strong> = direction correct, magnitude distorted (Type C).
        <strong style={{ color: '#166534' }}> SAFE</strong> = pooled trustworthy (Type D).
      </p>

      <div style={{ display: 'flex', gap: 12, marginBottom: '1.25rem', flexWrap: 'wrap' }}>
        {(['DANGER', 'CAUTION', 'SAFE'] as const).map(tier => {
          const c = TIER_COLORS[tier];
          return (
            <div key={tier} style={{
              padding: '10px 18px', borderRadius: 8,
              background: c.bg, color: c.text, border: `1px solid ${c.border}`,
              fontWeight: 700, fontSize: 14,
            }}>
              {tier}: {tierCounts[tier]} / {rows.length}
            </div>
          );
        })}
      </div>

      <div style={{ overflowX: 'auto', borderRadius: 8, border: '1px solid #ddd' }}>
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
            {sorted.map((r, i) => {
              const tier = r.screening_tier;
              const c = tier ? TIER_COLORS[tier] : { bg: 'transparent', text: '#333', border: '#ddd' };
              const dr = Number(r.distortion_ratio);
              return (
                <tr key={r.study} style={{ background: i % 2 === 0 ? '#fafafa' : '#fff' }}>
                  <td style={{ padding: '7px 10px', fontWeight: 500 }}>{r.study}</td>
                  <td style={{ padding: '7px 10px', textAlign: 'center' }}>
                    <TrTypeBadge type={r.distortion_type} />
                  </td>
                  <td style={{ padding: '7px 10px', textAlign: 'center' }}>
                    <TrTierPill tier={tier} />
                  </td>
                  <td style={{
                    padding: '7px 10px', textAlign: 'right', fontFamily: 'monospace',
                    color: dr < -1 ? '#800' : dr < 0 ? '#c44' : dr === 1 ? '#166534' : '#555',
                    fontWeight: 600,
                  }}>
                    <TrCell col="distortion_ratio">{ratio(r.distortion_ratio)}</TrCell>
                  </td>
                  <td style={{ padding: '7px 10px', textAlign: 'right', fontFamily: 'monospace' }}>
                    <TrCell col="allocation_distortion">{Number(r.allocation_distortion).toFixed(4)}</TrCell>
                  </td>
                  <td style={{ padding: '7px 10px', textAlign: 'right', color: '#166534', fontFamily: 'monospace' }}>
                    <TrCell col="weighted_stratum_gap_sum">{pct(r.weighted_stratum_gap_sum)}</TrCell>
                  </td>
                  <td style={{ padding: '7px 10px', textAlign: 'right', fontFamily: 'monospace' }}>
                    <TrCell col="signed_pooled_gap">{pct(r.signed_pooled_gap)}</TrCell>
                  </td>
                  <td style={{ padding: '7px 10px', fontSize: 11, color: c.text, maxWidth: 240 }}>
                    <TrCell col="policy_implication">{r.policy_implication}</TrCell>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      <div style={{
        marginTop: '1.25rem', padding: '14px 18px', background: '#f8f8fc',
        borderRadius: 8, border: '1px solid #ddd', fontSize: 13,
      }}>
        <strong>Distortion ratio:</strong>{' '}
        <code style={{ background: '#ece', padding: '2px 6px', borderRadius: 3 }}>
          DistortionRatio = SignedPooledGap / CorrectedGap
        </code>
        <br />
        <span style={{ color: '#666', marginTop: 6, display: 'block' }}>
          Ratio &lt; −1 → Type B · Ratio ∈ (−1, 0) → Type A ·
          Ratio &gt; 0 → Type C/D · Ratio = 1 → Type D (safe)
        </span>
      </div>
      <ViewDagScan ready={!loading} deps={[rows, sortCol, sortDir]} />
    </div>
  );
}
