import { useEffect, useState } from 'react';
import { api } from '../api';
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

function score(n: number | null | undefined): string {
  if (n == null) return '—';
  return Number(n).toFixed(3);
}

function pct(n: number | null | undefined): string {
  if (n == null) return '—';
  return (Number(n) * 100).toFixed(1) + '%';
}

export function InstrumentDashboardView() {
  const [rows, setRows] = useState<TreatmentRanking[]>([]);
  const [loading, setLoading] = useState(true);
  const [sortCol, setSortCol] = useState<'instrument_score' | 'distortion_ratio' | 'study'>('instrument_score');
  const [sortDir, setSortDir] = useState<'asc' | 'desc'>('desc');

  useEffect(() => {
    api.treatmentRankings().then(setRows).finally(() => setLoading(false));
  }, []);

  function toggleSort(col: typeof sortCol) {
    if (sortCol === col) setSortDir(d => d === 'asc' ? 'desc' : 'asc');
    else { setSortCol(col); setSortDir('desc'); }
  }

  const sorted = [...rows].sort((a, b) => {
    const va = sortCol === 'study' ? a.study : Number((a as any)[sortCol] ?? 0);
    const vb = sortCol === 'study' ? b.study : Number((b as any)[sortCol] ?? 0);
    if (typeof va === 'string') return sortDir === 'asc' ? va.localeCompare(vb as string) : (vb as string).localeCompare(va);
    return sortDir === 'asc' ? (va as number) - (vb as number) : (vb as number) - (va as number);
  });

  const tierCounts = { DANGER: 0, CAUTION: 0, SAFE: 0 } as Record<string, number>;
  rows.forEach(r => { if ((r as any).screening_tier) tierCounts[(r as any).screening_tier]++; });

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
        Screening verdict for each study. <strong>InstrumentScore = |DistortionRatio − 1|</strong>.
        Zero = no distortion. Higher = more dangerous.
        <strong style={{ color: '#c00' }}> DANGER</strong> = pooled sign is wrong (Types A & B).
        <strong style={{ color: '#854d0e' }}> CAUTION</strong> = direction correct, magnitude distorted (Type C).
        <strong style={{ color: '#166534' }}> SAFE</strong> = pooled trustworthy (Type D).
      </p>

      {/* Tier summary badges */}
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

      {/* Main table */}
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
              <th style={thStyle('instrument_score')} onClick={() => toggleSort('instrument_score')}>
                InstrumentScore {sortCol === 'instrument_score' ? (sortDir === 'asc' ? '▲' : '▼') : ''}
              </th>
              <th style={{ ...thStyle('study'), cursor: 'default' }}>CorrectedGap</th>
              <th style={{ ...thStyle('study'), cursor: 'default' }}>PooledGap</th>
              <th style={{ ...thStyle('study'), cursor: 'default' }}>Verdict</th>
            </tr>
          </thead>
          <tbody>
            {sorted.map((r, i) => {
              const tier = (r as any).screening_tier as string | undefined;
              const c = tier ? TIER_COLORS[tier] : { bg: 'transparent', text: '#333', border: '#ddd' };
              const dr = Number((r as any).distortion_ratio);
              const isScore = Number((r as any).instrument_score);
              return (
                <tr key={r.study} style={{ background: i % 2 === 0 ? '#fafafa' : '#fff' }}>
                  <td style={{ padding: '7px 10px', fontWeight: 500 }}>{r.study}</td>
                  <td style={{ padding: '7px 10px', textAlign: 'center' }}>
                    <span style={{
                      display: 'inline-block', borderRadius: 4, padding: '1px 7px',
                      fontSize: 11, fontWeight: 700,
                      background: { A: '#fde', B: '#ede', C: '#def', D: '#dfd' }[r.distortion_type] ?? '#eee',
                      color: { A: '#c00', B: '#808', C: '#008', D: '#060' }[r.distortion_type] ?? '#444',
                    }}>
                      {r.distortion_type}
                    </span>
                  </td>
                  <td style={{ padding: '7px 10px', textAlign: 'center' }}>
                    {tier && (
                      <span style={{
                        display: 'inline-block', borderRadius: 4, padding: '2px 8px',
                        fontSize: 11, fontWeight: 700,
                        background: c.bg, color: c.text, border: `1px solid ${c.border}`,
                      }}>
                        {tier}
                      </span>
                    )}
                  </td>
                  <td style={{
                    padding: '7px 10px', textAlign: 'right', fontFamily: 'monospace',
                    color: dr < -1 ? '#800' : dr < 0 ? '#c44' : dr === 1 ? '#166534' : '#555',
                    fontWeight: 600,
                  }}>
                    {ratio((r as any).distortion_ratio)}
                  </td>
                  <td style={{ padding: '7px 10px', textAlign: 'right', position: 'relative' }}>
                    <div style={{
                      position: 'absolute', left: 4, top: '50%', transform: 'translateY(-50%)',
                      height: 14, width: `${Math.min(isScore / 5.5 * 80, 80)}%`,
                      background: tier === 'DANGER' ? '#fcc' : tier === 'CAUTION' ? '#fde68a' : '#bbf7d0',
                      borderRadius: 2, opacity: 0.6,
                    }} />
                    <span style={{ position: 'relative', fontFamily: 'monospace', fontWeight: 600 }}>
                      {score((r as any).instrument_score)}
                    </span>
                  </td>
                  <td style={{ padding: '7px 10px', textAlign: 'right', color: '#166534', fontFamily: 'monospace' }}>
                    {pct(r.weighted_stratum_gap_sum)}
                  </td>
                  <td style={{ padding: '7px 10px', textAlign: 'right', fontFamily: 'monospace' }}>
                    {pct(r.signed_pooled_gap)}
                  </td>
                  <td style={{ padding: '7px 10px', fontSize: 11, color: c.text, maxWidth: 240 }}>
                    {(r as any).screening_verdict}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      {/* Formula box */}
      <div style={{
        marginTop: '1.25rem', padding: '14px 18px', background: '#f8f8fc',
        borderRadius: 8, border: '1px solid #ddd', fontSize: 13,
      }}>
        <strong>Screening formula:</strong>{' '}
        <code style={{ background: '#ece', padding: '2px 6px', borderRadius: 3 }}>
          DistortionRatio = SignedPooledGap / CorrectedGap
        </code>
        {' '}·{' '}
        <code style={{ background: '#ece', padding: '2px 6px', borderRadius: 3 }}>
          InstrumentScore = |DistortionRatio − 1|
        </code>
        <br />
        <span style={{ color: '#666', marginTop: 6, display: 'block' }}>
          Ratio &lt; −1 → Type B (amplification paradox) · Ratio ∈ (−1, 0) → Type A (classic reversal) ·
          Ratio &gt; 0 → Type C/D (no sign flip) · Ratio = 1 exactly → Type D (safe)
        </span>
      </div>
    </div>
  );
}
