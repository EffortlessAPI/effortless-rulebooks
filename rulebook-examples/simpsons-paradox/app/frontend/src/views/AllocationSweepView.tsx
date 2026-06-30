import { useState, useEffect, useMemo } from 'react';
import { api } from '../api';
import { ViewDagScan } from '../components/DagValue';
import { Cell } from '../components/dag-display';
import type { AllocationSweepRow, SweepStudySummary } from '../types';

const TYPE_COLORS: Record<string, string> = {
  A: '#e05',
  B: '#e80',
  'C+': '#07b',
  'C-': '#609',
  C: '#07b',
  D: '#080',
};

function pct(n: number | null | undefined): string {
  if (n == null) return '—';
  return (n * 100).toFixed(2) + '%';
}

function pp(n: number | null | undefined): string {
  if (n == null) return '—';
  const sign = n >= 0 ? '+' : '';
  return sign + (n * 100).toFixed(2) + 'pp';
}

export function AllocationSweepView() {
  const [studyId, setStudyId] = useState('kidney-1986');
  const [typeFilter, setTypeFilter] = useState<string>('all');
  const [rows, setRows] = useState<AllocationSweepRow[]>([]);
  const [summaries, setSummaries] = useState<SweepStudySummary[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.sweepSummary().then(setSummaries);
  }, []);

  const filteredSummaries = useMemo(() => {
    const list =
      typeFilter === 'all'
        ? summaries
        : summaries.filter(s => s.distortion_type_label === typeFilter);
    return [...list].sort((a, b) =>
      a.sweep_study_id.localeCompare(b.sweep_study_id)
    );
  }, [summaries, typeFilter]);

  useEffect(() => {
    if (!summaries.length) return;
    const ids = new Set(filteredSummaries.map(s => s.sweep_study_id));
    if (!ids.has(studyId) && filteredSummaries.length) {
      setStudyId(filteredSummaries[0].sweep_study_id);
    }
  }, [summaries, filteredSummaries, studyId]);

  useEffect(() => {
    if (!studyId) return;
    setLoading(true);
    api.sweep(studyId).then(setRows).finally(() => setLoading(false));
  }, [studyId]);

  const summary = summaries.find(s => s.sweep_study_id === studyId);
  const correctedGap = rows[0]?.sweep_corrected_gap ?? null;
  const originalRow = rows.find(r => r.is_original);

  const pooledMin = summary?.sweep_pooled_gap_min ?? 0;
  const pooledMax = summary?.sweep_pooled_gap_max ?? 0;
  const pooledSpan = pooledMax - pooledMin || 1;

  function barX(gap: number): number {
    return ((gap - pooledMin) / pooledSpan) * 100;
  }

  const zeroX = barX(0);
  const cgX = correctedGap != null ? barX(correctedGap) : null;

  const passCount = summaries.filter(
    s => Number(s.sweep_corrected_gap_range) < 0.0001
  ).length;

  return (
    <div style={{ padding: '1.5rem', maxWidth: 960 }}>
      <h2 style={{ margin: '0 0 .25rem' }}>Allocation Invariance Sweep</h2>
      <p style={{ color: '#888', margin: '0 0 1.25rem', fontSize: 14 }}>
        Hold stratum totals and per-stratum success rates fixed. Vary only the treatment
        allocation within the most confounded stratum. <strong>CorrectedGap is flat.
        PooledGap wanders.</strong> Corpus-wide: {summaries.length} studies × 10 fractions.
      </p>

      {/* Type filter */}
      <div style={{ display: 'flex', gap: 8, marginBottom: '0.75rem', flexWrap: 'wrap' }}>
        {['all', 'A', 'B', 'C+', 'C-', 'D'].map(t => (
          <button
            key={t}
            type="button"
            onClick={() => setTypeFilter(t)}
            style={{
              padding: '4px 12px',
              borderRadius: 16,
              border: typeFilter === t ? '2px solid #1a73e8' : '1px solid #ccc',
              background: typeFilter === t ? '#e8f0fe' : '#fff',
              cursor: 'pointer',
              fontSize: 12,
            }}
          >
            {t === 'all' ? 'All types' : `Type ${t}`}
          </button>
        ))}
      </div>

      {/* Study selector */}
      <div
        style={{
          display: 'flex',
          gap: 8,
          marginBottom: '1.25rem',
          flexWrap: 'wrap',
          maxHeight: 120,
          overflowY: 'auto',
          padding: '4px 0',
        }}
      >
        {filteredSummaries.map(s => (
          <button
            key={s.sweep_study_id}
            type="button"
            onClick={() => setStudyId(s.sweep_study_id)}
            style={{
              padding: '6px 12px',
              borderRadius: 6,
              border:
                studyId === s.sweep_study_id
                  ? `2px solid ${TYPE_COLORS[s.distortion_type_label] ?? '#888'}`
                  : '2px solid #ddd',
              background: studyId === s.sweep_study_id ? '#f0f4ff' : '#fff',
              cursor: 'pointer',
              fontSize: 12,
              textAlign: 'left',
            }}
          >
            <span
              style={{
                background: TYPE_COLORS[s.distortion_type_label] ?? '#888',
                color: '#fff',
                borderRadius: 3,
                padding: '0 4px',
                fontSize: 10,
                fontWeight: 700,
                marginRight: 6,
              }}
            >
              <Cell table="SweepStudySummary" col="distortion_type_label">{s.distortion_type_label}</Cell>
            </span>
            {s.sweep_study_id}
          </button>
        ))}
      </div>

      {loading && <div className="loading">Loading…</div>}

      {!loading && summary && (
        <>
          <div
            style={{
              display: 'grid',
              gridTemplateColumns: '1fr 1fr',
              gap: 12,
              marginBottom: '1.25rem',
            }}
          >
            <div
              style={{
                padding: '10px 14px',
                borderRadius: 8,
                background: Number(summary.sweep_corrected_gap_range) < 0.0001 ? '#e6f4ea' : '#fce8e6',
                border: `1px solid ${Number(summary.sweep_corrected_gap_range) < 0.0001 ? '#34a853' : '#ea4335'}`,
                fontSize: 13,
              }}
            >
              <strong><Cell table="SweepStudySummary" col="invariant_witness">{summary.invariant_witness}</Cell></strong>
              <div style={{ marginTop: 4, color: '#555' }}>
                CG range ={' '}
                <Cell table="SweepStudySummary" col="sweep_corrected_gap_range">
                  {(Number(summary.sweep_corrected_gap_range) * 100).toFixed(8)}pp
                </Cell>
              </div>
            </div>
            <div
              style={{
                padding: '10px 14px',
                borderRadius: 8,
                background: summary.pooled_gap_crosses_zero ? '#fef7e0' : '#f1f3f4',
                border: '1px solid #ccc',
                fontSize: 13,
              }}
            >
              {summary.pooled_gap_crosses_zero
                ? <>
                    PooledGap crosses zero — range{' '}
                    <Cell table="SweepStudySummary" col="sweep_pooled_gap_range">
                      {pp(Number(summary.sweep_pooled_gap_range))}
                    </Cell>
                  </>
                : <>
                    PooledGap range{' '}
                    <Cell table="SweepStudySummary" col="sweep_pooled_gap_range">
                      {pp(Number(summary.sweep_pooled_gap_range))}
                    </Cell>{' '}
                    (no sign flip in sweep)
                  </>}
            </div>
          </div>

          <p style={{ color: '#666', fontSize: 13, marginBottom: '1rem' }}>
            Sweeping allocation in stratum{' '}
            <strong><Cell table="SweepStudySummary" col="sweep_stratum_label">{summary.sweep_stratum_label ?? '—'}</Cell></strong>.
            All other stratum totals and per-cell rates are fixed. CorrectedGap constant ={' '}
            <strong><Cell table="SweepStudySummary" col="corrected_gap_constant">{pct(correctedGap)}</Cell></strong>
            {originalRow && (
              <>
                {' '}
                · Original allocation ≈ <strong>{pct(originalRow.alloc_fraction_a)}</strong>
                {originalRow.is_original ? ' ★' : ''}
              </>
            )}
          </p>

          {/* Chart */}
          <div
            style={{
              background: '#f8f8f8',
              borderRadius: 10,
              padding: '1rem',
              marginBottom: '1.25rem',
            }}
          >
            <div style={{ fontSize: 12, color: '#888', marginBottom: 8 }}>
              Gap (treatment A − treatment B) · left = B wins, right = A wins
            </div>
            <div style={{ position: 'relative', height: rows.length * 26 + 40 }}>
              <div
                style={{
                  position: 'absolute',
                  left: `${zeroX}%`,
                  top: 0,
                  bottom: 0,
                  width: 1,
                  background: '#999',
                }}
              />
              {cgX != null && (
                <div
                  style={{
                    position: 'absolute',
                    left: `${cgX}%`,
                    top: 0,
                    bottom: 24,
                    width: 2,
                    background: '#0a0',
                    opacity: 0.7,
                  }}
                >
                  <span
                    style={{
                      position: 'absolute',
                      bottom: -20,
                      left: -30,
                      fontSize: 10,
                      color: '#0a0',
                      whiteSpace: 'nowrap',
                      fontWeight: 600,
                    }}
                  >
                    CG = <Cell table="AllocationSweep" col="sweep_corrected_gap">{pct(correctedGap)}</Cell>
                  </span>
                </div>
              )}
              <div
                style={{
                  position: 'absolute',
                  left: `${zeroX}%`,
                  bottom: 6,
                  transform: 'translateX(-50%)',
                  fontSize: 10,
                  color: '#999',
                }}
              >
                0
              </div>

              {rows.map((row, i) => {
                const pooled = Number(row.sweep_pooled_gap);
                const barLeft = Math.min(zeroX, barX(pooled));
                const barRight = Math.max(zeroX, barX(pooled));
                const isOrig = row.is_original;
                return (
                  <div
                    key={row.sweep_id}
                    style={{
                      position: 'absolute',
                      top: i * 26,
                      left: 0,
                      right: 0,
                      height: 22,
                      display: 'flex',
                      alignItems: 'center',
                    }}
                  >
                    <span
                      style={{
                        width: 36,
                        fontSize: 11,
                        color: isOrig ? '#000' : '#666',
                        fontWeight: isOrig ? 700 : 400,
                      }}
                    >
                      {Math.round(row.alloc_fraction_a * 100)}%
                    </span>
                    <div style={{ flex: 1, position: 'relative', height: 14 }}>
                      <div
                        style={{
                          position: 'absolute',
                          left: `${barLeft}%`,
                          width: `${barRight - barLeft}%`,
                          height: '100%',
                          background: pooled >= 0 ? '#4a90e2' : '#e05',
                          borderRadius: 2,
                          opacity: isOrig ? 1 : 0.75,
                          outline: isOrig ? '2px solid #fbbc04' : 'none',
                        }}
                      />
                    </div>
                    <span
                      style={{
                        width: 64,
                        fontSize: 11,
                        textAlign: 'right',
                        color: pooled >= 0 ? '#4a90e2' : '#e05',
                      }}
                    >
                      <Cell table="AllocationSweep" col="sweep_pooled_gap">{pp(pooled)}</Cell>
                    </span>
                  </div>
                );
              })}
            </div>
            <div style={{ marginTop: 8, fontSize: 11, color: '#888' }}>
              <span
                style={{
                  display: 'inline-block',
                  width: 12,
                  height: 8,
                  background: '#4a90e2',
                  borderRadius: 1,
                  marginRight: 4,
                }}
              />
              A wins
              <span
                style={{
                  display: 'inline-block',
                  width: 12,
                  height: 8,
                  background: '#e05',
                  borderRadius: 1,
                  margin: '0 4px 0 16px',
                }}
              />
              B wins
              <span
                style={{
                  display: 'inline-block',
                  width: 3,
                  height: 12,
                  background: '#0a0',
                  marginRight: 4,
                  verticalAlign: 'middle',
                }}
              />
              CorrectedGap (constant)
              <span
                style={{
                  display: 'inline-block',
                  width: 1,
                  height: 12,
                  background: '#999',
                  margin: '0 4px',
                  verticalAlign: 'middle',
                }}
              />
              zero
            </div>
          </div>

          <details style={{ marginBottom: '1rem' }}>
            <summary style={{ cursor: 'pointer', fontSize: 13, color: '#666' }}>
              Numeric data table
            </summary>
            <table
              style={{
                width: '100%',
                borderCollapse: 'collapse',
                fontSize: 12,
                marginTop: 8,
              }}
            >
              <thead>
                <tr style={{ background: '#f0f0f0' }}>
                  <th style={{ padding: '4px 8px', textAlign: 'left' }}>f(A)</th>
                  <th style={{ padding: '4px 8px', textAlign: 'right' }}>PooledRateA</th>
                  <th style={{ padding: '4px 8px', textAlign: 'right' }}>PooledRateB</th>
                  <th style={{ padding: '4px 8px', textAlign: 'right' }}>PooledGap</th>
                  <th style={{ padding: '4px 8px', textAlign: 'right' }}>CorrectedGap</th>
                  <th style={{ padding: '4px 8px', textAlign: 'right' }}>Distortion</th>
                </tr>
              </thead>
              <tbody>
                {rows.map(row => (
                  <tr
                    key={row.sweep_id}
                    style={{
                      background: row.is_original ? '#fffbe6' : 'transparent',
                      fontWeight: row.is_original ? 600 : 400,
                    }}
                  >
                    <td style={{ padding: '3px 8px' }}>
                      {Math.round(row.alloc_fraction_a * 100)}%
                      {row.is_original ? ' ★' : ''}
                    </td>
                    <td style={{ padding: '3px 8px', textAlign: 'right' }}>
                      <Cell table="AllocationSweep" col="sweep_pooled_rate_a">{pct(row.sweep_pooled_rate_a)}</Cell>
                    </td>
                    <td style={{ padding: '3px 8px', textAlign: 'right' }}>
                      <Cell table="AllocationSweep" col="sweep_pooled_rate_b">{pct(row.sweep_pooled_rate_b)}</Cell>
                    </td>
                    <td
                      style={{
                        padding: '3px 8px',
                        textAlign: 'right',
                        color: Number(row.sweep_pooled_gap) >= 0 ? '#4a90e2' : '#e05',
                      }}
                    >
                      <Cell table="AllocationSweep" col="sweep_pooled_gap">{pp(Number(row.sweep_pooled_gap))}</Cell>
                    </td>
                    <td style={{ padding: '3px 8px', textAlign: 'right', color: '#0a0' }}>
                      <Cell table="AllocationSweep" col="sweep_corrected_gap">{pp(Number(row.sweep_corrected_gap))}</Cell>
                    </td>
                    <td style={{ padding: '3px 8px', textAlign: 'right', color: '#888' }}>
                      <Cell table="AllocationSweep" col="allocation_distortion_witness">
                        {pp(Number(row.allocation_distortion_witness))}
                      </Cell>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </details>

          <details open>
            <summary style={{ cursor: 'pointer', fontSize: 13, color: '#666' }}>
              Invariant witness — all {summaries.length} studies ({passCount} pass CorrectedGap
              invariance)
            </summary>
            <table
              style={{
                width: '100%',
                borderCollapse: 'collapse',
                fontSize: 12,
                marginTop: 8,
              }}
            >
              <thead>
                <tr style={{ background: '#f0f0f0' }}>
                  <th style={{ padding: '4px 8px', textAlign: 'left' }}>Study</th>
                  <th style={{ padding: '4px 8px', textAlign: 'left' }}>Stratum</th>
                  <th style={{ padding: '4px 8px', textAlign: 'center' }}>Type</th>
                  <th style={{ padding: '4px 8px', textAlign: 'right' }}>CG constant</th>
                  <th style={{ padding: '4px 8px', textAlign: 'right' }}>CG range</th>
                  <th style={{ padding: '4px 8px', textAlign: 'right' }}>PG range</th>
                  <th style={{ padding: '4px 8px', textAlign: 'center' }}>Sign flip?</th>
                </tr>
              </thead>
              <tbody>
                {summaries.map(s => (
                  <tr
                    key={s.sweep_study_id}
                    style={{
                      background: s.sweep_study_id === studyId ? '#f0f4ff' : 'transparent',
                      cursor: 'pointer',
                    }}
                    onClick={() => setStudyId(s.sweep_study_id)}
                  >
                    <td style={{ padding: '3px 8px' }}>{s.sweep_study_id}</td>
                    <td style={{ padding: '3px 8px', fontFamily: 'monospace', fontSize: 11 }}>
                      <Cell table="SweepStudySummary" col="sweep_stratum_label">{s.sweep_stratum_label}</Cell>
                    </td>
                    <td style={{ padding: '3px 8px', textAlign: 'center' }}>
                      <span
                        style={{
                          background: TYPE_COLORS[s.distortion_type_label] ?? '#888',
                          color: '#fff',
                          borderRadius: 3,
                          padding: '1px 6px',
                          fontSize: 11,
                          fontWeight: 700,
                        }}
                      >
                        <Cell table="SweepStudySummary" col="distortion_type_label">{s.distortion_type_label}</Cell>
                      </span>
                    </td>
                    <td style={{ padding: '3px 8px', textAlign: 'right', color: '#0a0' }}>
                      <Cell table="SweepStudySummary" col="corrected_gap_constant">{pp(Number(s.corrected_gap_constant))}</Cell>
                    </td>
                    <td
                      style={{
                        padding: '3px 8px',
                        textAlign: 'right',
                        color: Number(s.sweep_corrected_gap_range) < 0.0001 ? '#0a0' : '#e05',
                      }}
                    >
                      <Cell table="SweepStudySummary" col="sweep_corrected_gap_range">
                        {(Number(s.sweep_corrected_gap_range) * 100).toFixed(8)}pp
                      </Cell>
                    </td>
                    <td style={{ padding: '3px 8px', textAlign: 'right' }}>
                      <Cell table="SweepStudySummary" col="sweep_pooled_gap_range">{pp(Number(s.sweep_pooled_gap_range))}</Cell>
                    </td>
                    <td style={{ padding: '3px 8px', textAlign: 'center' }}>
                      <Cell table="SweepStudySummary" col="pooled_gap_crosses_zero">
                        {s.pooled_gap_crosses_zero ? '✓' : '—'}
                      </Cell>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </details>
        </>
      )}
      <ViewDagScan ready={!loading} deps={[rows, summaries, studyId, typeFilter, summary]} />
    </div>
  );
}
