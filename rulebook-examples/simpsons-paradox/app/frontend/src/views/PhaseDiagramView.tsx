import { useEffect, useMemo, useState } from 'react';
import { api } from '../api';
import { ViewDagScan } from '../components/DagValue';
import { Cell } from '../components/dag-display';
import type { PhaseDiagramSummary, SyntheticPhaseRow } from '../types';

const TYPE_COLORS: Record<string, string> = {
  A: '#e05',
  B: '#e80',
  'C+': '#07b',
  'C-': '#609',
  D: '#080',
};

const TYPE_LABELS: Record<string, string> = {
  A: 'Full reversal',
  B: 'Partial sign-flip',
  'C+': 'Amplification',
  'C-': 'Compression',
  D: 'Neutral',
};

function fmt(n: unknown, dp: number): string {
  return Number(n).toFixed(dp);
}

function fmtPp(n: unknown, dp = 0): string {
  return `${fmt(Number(n) * 100, dp)}pp`;
}

export function PhaseDiagramView() {
  const [summary, setSummary] = useState<PhaseDiagramSummary | null>(null);
  const [rows, setRows] = useState<SyntheticPhaseRow[]>([]);
  const [typeFilter, setTypeFilter] = useState<string>('all');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setError(null);
    Promise.all([api.phaseDiagramSummary(), api.syntheticPhase()])
      .then(([s, r]) => {
        setSummary(s);
        setRows(r);
      })
      .catch(err => {
        setError(err instanceof Error ? err.message : String(err));
      })
      .finally(() => setLoading(false));
  }, []);

  const filtered = useMemo(
    () =>
      typeFilter === 'all'
        ? rows
        : rows.filter(r => r.phase_distortion_type === typeFilter),
    [rows, typeFilter]
  );

  if (loading) return <div className="loading">Loading…</div>;
  if (error) return <div className="error">{error}</div>;
  if (!summary) return <div className="error">No phase diagram summary found.</div>;

  const types = ['A', 'B', 'C+', 'C-', 'D'] as const;
  const counts: Record<string, number> = {
    A: summary.phase_type_a_count,
    B: summary.phase_type_b_count,
    'C+': summary.phase_type_c_plus_count,
    'C-': summary.phase_type_c_minus_count,
    D: summary.phase_type_d_count,
  };

  return (
    <div style={{ padding: '1.5rem', maxWidth: 960 }}>
      <h2 style={{ margin: '0 0 .25rem' }}>Synthetic Phase Diagram</h2>
      <p style={{ color: '#888', margin: '0 0 1.25rem', fontSize: 14 }}>
        240 parameter points at controlled (StratumFraction, StratumGap, AllocationBias)
        coordinates. Hard stratum base rate 0.45, easy stratum 0.75. Each point derives
        its <strong>DistortionType</strong> from the same geometric formulas as real studies —
        proving the five-cell taxonomy is populated across parameter space.
      </p>

      <div
        style={{
          padding: '12px 16px',
          borderRadius: 8,
          marginBottom: '1.25rem',
          background: summary.all_five_types_present ? '#e6f4ea' : '#fce8e6',
          border: `1px solid ${summary.all_five_types_present ? '#34a853' : '#ea4335'}`,
          fontSize: 14,
        }}
      >
        <strong><Cell table="PhaseDiagramSummary" col="phase_witness_note">{summary.phase_witness_note}</Cell></strong>
        <div style={{ marginTop: 4, color: '#555' }}>
          <Cell table="PhaseDiagramSummary" col="phase_taxonomy_witness">{summary.phase_taxonomy_witness}</Cell>
        </div>
      </div>

      <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap', marginBottom: '1.25rem' }}>
        {types.map(t => (
          <button
            key={t}
            onClick={() => setTypeFilter(typeFilter === t ? 'all' : t)}
            style={{
              padding: '8px 14px',
              borderRadius: 6,
              border: typeFilter === t ? `2px solid ${TYPE_COLORS[t]}` : '2px solid #ddd',
              background: '#fff',
              cursor: 'pointer',
              fontSize: 13,
            }}
          >
            <span style={{ color: TYPE_COLORS[t], fontWeight: 700 }}>{t}</span>
            <span style={{ color: '#888', marginLeft: 6 }}>
              <Cell table="PhaseDiagramSummary" col={`phase_type_${t === 'C+' ? 'c_plus' : t === 'C-' ? 'c_minus' : t.toLowerCase()}_count`}>
                {counts[t]}
              </Cell>
            </span>
          </button>
        ))}
        {typeFilter !== 'all' && (
          <button
            onClick={() => setTypeFilter('all')}
            style={{ padding: '8px 12px', borderRadius: 6, border: '1px solid #ccc', background: '#f5f5f5', fontSize: 13 }}
          >
            Clear filter
          </button>
        )}
      </div>

      <div style={{ overflowX: 'auto', border: '1px solid #e0e0e0', borderRadius: 8 }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
          <thead>
            <tr style={{ background: '#f6f8fa', textAlign: 'left' }}>
              <th style={{ padding: '8px 10px' }}>Type</th>
              <th style={{ padding: '8px 10px' }}>w</th>
              <th style={{ padding: '8px 10px' }}>g₁</th>
              <th style={{ padding: '8px 10px' }}>g₂</th>
              <th style={{ padding: '8px 10px' }}>bias</th>
              <th style={{ padding: '8px 10px' }}>Pooled gap</th>
              <th style={{ padding: '8px 10px' }}>Corrected</th>
              <th style={{ padding: '8px 10px' }}>Distortion</th>
            </tr>
          </thead>
          <tbody>
            {filtered.slice(0, 80).map(r => (
              <tr key={r.phase_id} style={{ borderTop: '1px solid #eee' }}>
                <td style={{ padding: '6px 10px' }}>
                  <span style={{ color: TYPE_COLORS[r.phase_distortion_type], fontWeight: 600 }}>
                    <Cell table="SyntheticPhase" col="phase_distortion_type">{r.phase_distortion_type}</Cell>
                  </span>
                  <span style={{ color: '#aaa', fontSize: 11, marginLeft: 4 }}>
                    {TYPE_LABELS[r.phase_distortion_type]}
                  </span>
                </td>
                <td style={{ padding: '6px 10px' }}>
                  <Cell table="SyntheticPhase" col="param_stratum_fraction">{fmt(r.param_stratum_fraction, 2)}</Cell>
                </td>
                <td style={{ padding: '6px 10px' }}>
                  <Cell table="SyntheticPhase" col="param_stratum_gap1">{fmtPp(r.param_stratum_gap1)}</Cell>
                </td>
                <td style={{ padding: '6px 10px' }}>
                  <Cell table="SyntheticPhase" col="param_stratum_gap2">{fmtPp(r.param_stratum_gap2)}</Cell>
                </td>
                <td style={{ padding: '6px 10px' }}>
                  <Cell table="SyntheticPhase" col="param_allocation_bias">{fmt(r.param_allocation_bias, 2)}</Cell>
                </td>
                <td style={{ padding: '6px 10px' }}>
                  <Cell table="SyntheticPhase" col="phase_signed_pooled_gap">{fmtPp(r.phase_signed_pooled_gap, 2)}</Cell>
                </td>
                <td style={{ padding: '6px 10px' }}>
                  <Cell table="SyntheticPhase" col="phase_corrected_gap">{fmtPp(r.phase_corrected_gap, 2)}</Cell>
                </td>
                <td style={{ padding: '6px 10px' }}>
                  <Cell table="SyntheticPhase" col="phase_allocation_distortion">{fmtPp(r.phase_allocation_distortion, 2)}</Cell>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {filtered.length > 80 && (
          <p style={{ padding: '8px 12px', margin: 0, color: '#888', fontSize: 12 }}>
            Showing 80 of {filtered.length} grid points{ typeFilter !== 'all' ? ` (type ${typeFilter})` : '' }.
          </p>
        )}
      </div>
      <ViewDagScan ready={!loading} deps={[filtered, typeFilter, summary]} />
    </div>
  );
}
