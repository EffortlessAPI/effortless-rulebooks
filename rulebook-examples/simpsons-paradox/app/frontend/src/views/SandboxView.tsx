import { useCallback, useEffect, useRef, useState } from 'react';
import { Chart } from 'chart.js/auto';
import { api } from '../api';
import { V, ViewDagScan } from '../components/DagValue';
import { SsCell, TrCell } from '../components/dag-display';
import type { CaseCell, StratumSummary, TreatmentRanking } from '../types';

const PRESET_STUDIES = ['kidney-1986', 'berkeley-1973', 'balanced-synthetic'];

interface EditableCell {
  stratum_label: string;
  treatment_label: string;
  successes: number;
  cases: number;
}

function groupCellsByStratum(cells: CaseCell[]) {
  const strata = new Map<string, { label: string; cells: EditableCell[] }>();
  for (const c of cells) {
    if (!strata.has(c.stratum_label)) {
      strata.set(c.stratum_label, { label: c.stratum_label, cells: [] });
    }
    strata.get(c.stratum_label)!.cells.push({
      stratum_label: c.stratum_label,
      treatment_label: c.treatment_label,
      successes: c.successes,
      cases: c.cases,
    });
  }
  return Array.from(strata.values());
}

function pct(n: number | null | undefined) {
  if (n == null) return '—';
  return (Number(n) * 100).toFixed(1) + '%';
}

function SliderRow({
  label,
  field,
  value,
  max,
  onChange,
}: {
  label: string;
  field: string;
  value: number;
  max: number;
  onChange: (v: number) => void;
}) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 6 }}>
      <label style={{ width: 140, fontSize: 12, color: '#8b949e' }} htmlFor={field}>{label}</label>
      <input
        id={field}
        type="range"
        min={0}
        max={max}
        value={value}
        onChange={e => onChange(Number(e.target.value))}
        style={{ flex: 1 }}
      />
      <span style={{ width: 36, textAlign: 'right', fontSize: 12, fontVariantNumeric: 'tabular-nums', color: '#58a6ff' }}>
        {value}
      </span>
    </div>
  );
}

export function SandboxView() {
  const [selectedStudy, setSelectedStudy] = useState(PRESET_STUDIES[0]);
  const [presetLabels, setPresetLabels] = useState<Record<string, string>>({});
  const [cells, setCells] = useState<EditableCell[]>([]);
  const [ranking, setRanking] = useState<TreatmentRanking | null>(null);
  const [summaries, setSummaries] = useState<StratumSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const chartRef = useRef<Chart | null>(null);
  const evaluateTimer = useRef<ReturnType<typeof setTimeout> | null>(null);

  const loadStudy = useCallback(async (studyId: string) => {
    setLoading(true);
    setError(null);
    try {
      const [rawCells, ranks, sums] = await Promise.all([
        api.caseCells(studyId),
        api.treatmentRankings(studyId),
        api.stratumSummaries(studyId),
      ]);
      setCells(rawCells.map(c => ({
        stratum_label: c.stratum_label,
        treatment_label: c.treatment_label,
        successes: c.successes,
        cases: c.cases,
      })));
      setRanking(ranks[0] ?? null);
      setSummaries(sums);
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e));
    } finally {
      setLoading(false);
    }
  }, []);

  const evaluate = useCallback(async (studyId: string, nextCells: EditableCell[]) => {
    try {
      const result = await api.sandboxEvaluate(studyId, nextCells);
      setRanking(result.ranking);
      setSummaries(result.summaries);
      setError(null);
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e));
    }
  }, []);

  useEffect(() => {
    api.studies().then(studies => {
      const labels: Record<string, string> = {};
      for (const s of studies) {
        if (PRESET_STUDIES.includes(s.study_id)) {
          labels[s.study_id] = s.title || s.name;
        }
      }
      setPresetLabels(labels);
    });
    loadStudy(PRESET_STUDIES[0]);
  }, [loadStudy]);

  const scheduleEvaluate = useCallback((studyId: string, nextCells: EditableCell[]) => {
    if (evaluateTimer.current) clearTimeout(evaluateTimer.current);
    evaluateTimer.current = setTimeout(() => evaluate(studyId, nextCells), 150);
  }, [evaluate]);

  const updateCell = (stratumLabel: string, treatmentLabel: string, field: 'successes' | 'cases', val: number) => {
    setCells(prev => {
      const next = prev.map(c => {
        if (c.stratum_label !== stratumLabel || c.treatment_label !== treatmentLabel) return c;
        const updated = { ...c, [field]: val };
        if (field === 'cases' && updated.successes > val) updated.successes = val;
        return updated;
      });
      scheduleEvaluate(selectedStudy, next);
      return next;
    });
  };

  const strataGroups = groupCellsByStratum(cells.map(c => ({ ...c, case_cell_id: '', study: selectedStudy, cell_success_rate: 0 })));

  useEffect(() => {
    if (!canvasRef.current || !ranking || !summaries.length) return;
    if (chartRef.current) chartRef.current.destroy();

    const strataRows = summaries.filter(s => s.treatment_label === ranking.treatment_a);
    const labels = [
      ...strataRows.map(s => `${s.stratum_label} ${ranking.treatment_a}`),
      ...strataRows.map(s => `${s.stratum_label} ${ranking.treatment_b}`),
      `Pooled ${ranking.treatment_a}`,
      `Pooled ${ranking.treatment_b}`,
    ];
    const data = [
      ...strataRows.map(s => Number(s.stratum_rate_a)),
      ...strataRows.map(s => Number(s.stratum_rate_b)),
      Number(ranking.pooled_rate_a),
      Number(ranking.pooled_rate_b),
    ];
    const colors = [
      ...strataRows.map(() => '#58a6ff99'),
      ...strataRows.map(() => '#d2a8ff99'),
      '#58a6ff',
      '#d2a8ff',
    ];

    chartRef.current = new Chart(canvasRef.current, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Rate',
          data,
          backgroundColor: colors,
          borderColor: colors.map(c => c.replace('99', '')),
          borderWidth: 1,
        }],
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
          x: { ticks: { color: '#8b949e', font: { size: 11 } }, grid: { color: '#21262d' } },
        },
        plugins: {
          legend: { display: false },
          tooltip: { callbacks: { label: ctx => `${(Number(ctx.parsed.y) * 100).toFixed(1)}%` } },
        },
      },
    });
  }, [ranking, summaries]);

  return (
    <div>
      <div className="page-title">Interactive Sandbox</div>
      <div className="page-desc">
        Adjust successes and cases for each stratum and treatment to see how the paradox
        appears and disappears. Derived facts come live from Postgres views — the same DAG
        as the rest of the app. Presets load real studies from the database.
      </div>

      <div style={{ display: 'flex', gap: 10, marginBottom: 20, flexWrap: 'wrap' }}>
        {PRESET_STUDIES.map(id => (
          <button
            key={id}
            onClick={() => { setSelectedStudy(id); loadStudy(id); }}
            style={{
              background: selectedStudy === id ? '#30363d' : '#21262d',
              border: `1px solid ${selectedStudy === id ? '#58a6ff' : '#30363d'}`,
              borderRadius: 6,
              color: '#e6edf3', padding: '6px 14px', cursor: 'pointer', fontSize: 13,
            }}
          >
            {presetLabels[id] ?? id}
          </button>
        ))}
      </div>

      {loading && <div className="loading">Loading…</div>}
      {error && <div className="card" style={{ color: '#ff7b72', marginBottom: 16 }}>{error}</div>}

      {!loading && ranking && (
        <>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16, marginBottom: 16 }}>
            {strataGroups.map(group => {
              const sorted = [...group.cells].sort((a, b) => a.treatment_label.localeCompare(b.treatment_label));
              return (
                <div className="card" key={group.label}>
                  <h3>Stratum: {group.label}</h3>
                  {sorted.map((cell, ci) => {
                    const color = ci === 0 ? '#58a6ff' : '#d2a8ff';
                    return (
                      <div key={cell.treatment_label}>
                        <div style={{ marginBottom: 10, fontWeight: 600, fontSize: 12, color }}>
                          Treatment {cell.treatment_label}
                        </div>
                        <SliderRow
                          label={`Successes ${cell.treatment_label}`}
                          field={`${group.label}-suc-${cell.treatment_label}`}
                          value={cell.successes}
                          max={cell.cases}
                          onChange={v => updateCell(group.label, cell.treatment_label, 'successes', v)}
                        />
                        <SliderRow
                          label={`Cases ${cell.treatment_label}`}
                          field={`${group.label}-cas-${cell.treatment_label}`}
                          value={cell.cases}
                          max={2000}
                          onChange={v => updateCell(group.label, cell.treatment_label, 'cases', v)}
                        />
                        <div style={{ marginBottom: 12, fontSize: 12, color: '#8b949e' }}>
                          Rate: <strong style={{ color }}>
                            <V table="CaseCells" col="cell_success_rate">
                              {cell.cases ? (cell.successes / cell.cases * 100).toFixed(1) : '—'}%
                            </V>
                          </strong>
                        </div>
                      </div>
                    );
                  })}
                </div>
              );
            })}
          </div>

          <div className="card">
            <h3>Live rates</h3>
            <div style={{ height: 240 }}><canvas ref={canvasRef} /></div>
          </div>

          <div className="two-col">
            <div className="card">
              <h3>Derived facts</h3>
              <div className="stat-row">
                <span className="stat-label">Pooled rate {ranking.treatment_a}</span>
                <span className="stat-value"><TrCell col="pooled_rate_a">{pct(ranking.pooled_rate_a)}</TrCell></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Pooled rate {ranking.treatment_b}</span>
                <span className="stat-value"><TrCell col="pooled_rate_b">{pct(ranking.pooled_rate_b)}</TrCell></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Pooled winner</span>
                <span className="stat-value"><TrCell col="pooled_winner">{ranking.pooled_winner}</TrCell></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">IsReversal (unanimity)</span>
                <span className="stat-value" style={{ color: ranking.is_reversal ? '#ff7b72' : '#7ee787' }}>
                  <TrCell col="is_reversal">{ranking.is_reversal ? 'YES' : 'no'}</TrCell>
                </span>
              </div>
              <div className="stat-row">
                <span className="stat-label">IsSignFlip</span>
                <span className="stat-value" style={{ color: ranking.is_sign_flip ? '#d2a8ff' : '#7ee787' }}>
                  <TrCell col="is_sign_flip">{ranking.is_sign_flip ? 'YES' : 'no'}</TrCell>
                </span>
              </div>
            </div>
            <div className="card">
              <h3>Continuous measures</h3>
              <div className="stat-row">
                <span className="stat-label">Paradox strength</span>
                <span className="stat-value"><TrCell col="paradox_strength">{Number(ranking.paradox_strength).toFixed(4)}</TrCell></span>
              </div>
              <div className="stat-row">
                <span className="stat-label">Allocation distortion</span>
                <span className="stat-value"><TrCell col="allocation_distortion">{Number(ranking.allocation_distortion).toFixed(4)}</TrCell></span>
              </div>
              {summaries
                .filter(s => s.treatment_label === ranking.treatment_a)
                .map(s => (
                  <div className="stat-row" key={s.stratum_summary_id}>
                    <span className="stat-label">{s.stratum_label} winner</span>
                    <span className="stat-value" style={{ color: '#58a6ff' }}>
                      <SsCell col="stratum_winner">{s.stratum_winner}</SsCell>
                    </span>
                  </div>
                ))}
            </div>
          </div>
        </>
      )}
      <ViewDagScan ready={!loading && !!ranking} deps={[selectedStudy, ranking, summaries, cells]} />
    </div>
  );
}
