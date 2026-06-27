import { useEffect, useRef, useState } from 'react';
import { Chart } from 'chart.js/auto';

// Replicates the Postgres view DAG in the browser (for live hypothesis testing)
// This intentionally does NOT call the API — it's a scratchpad for intuition.

interface Cell { successes: number; cases: number; }
interface Stratum { label: string; a: Cell; b: Cell; }

function computeSandbox(strata: Stratum[]) {
  let totalA = 0, totalB = 0, successA = 0, successB = 0;
  for (const s of strata) {
    totalA += s.a.cases;
    totalB += s.b.cases;
    successA += s.a.successes;
    successB += s.b.successes;
  }
  const pooledA = totalA ? successA / totalA : 0;
  const pooledB = totalB ? successB / totalB : 0;
  const pooledWinner = pooledA > pooledB ? 'A' : pooledB > pooledA ? 'B' : 'tie';
  const totalCases = totalA + totalB;

  let wgapSum = 0;
  let strataWonByLoser = 0;
  const strataDetail = strata.map(s => {
    const rA = s.a.cases ? s.a.successes / s.a.cases : 0;
    const rB = s.b.cases ? s.b.successes / s.b.cases : 0;
    const winner = rA > rB ? 'A' : rB > rA ? 'B' : 'tie';
    const gap = rA - rB;
    const frac = totalCases ? (s.a.cases + s.b.cases) / totalCases : 0;
    wgapSum += gap * frac;
    if (winner !== pooledWinner && winner !== 'tie') strataWonByLoser++;
    return { label: s.label, rA, rB, winner, gap, frac };
  });

  const signedPooledGap = pooledA - pooledB;
  const isSignFlip = wgapSum !== 0 && signedPooledGap !== 0 && Math.sign(wgapSum) !== Math.sign(signedPooledGap);
  const isReversal = strataDetail.every(s => s.winner !== pooledWinner && s.winner !== 'tie');
  const reversalIntensity = strata.length ? strataWonByLoser / strata.length : 0;
  const paradoxStrength = Math.abs(signedPooledGap) * reversalIntensity;
  const allocDistortion = Math.abs(wgapSum - signedPooledGap);

  return { pooledA, pooledB, pooledWinner, isReversal, isSignFlip, paradoxStrength, allocDistortion, strataDetail };
}

const PRESETS = [
  {
    label: 'Kidney stones (full reversal)',
    strata: [
      { label: 'Small', a: { successes: 81, cases: 87 }, b: { successes: 234, cases: 270 } },
      { label: 'Large', a: { successes: 192, cases: 263 }, b: { successes: 55, cases: 80 } },
    ],
  },
  {
    label: 'Berkeley (sign-flip, no unanimity)',
    strata: [
      { label: 'Dept A', a: { successes: 512, cases: 825 }, b: { successes: 89, cases: 108 } },
      { label: 'Dept B', a: { successes: 353, cases: 560 }, b: { successes: 17, cases: 25 } },
      { label: 'Dept C', a: { successes: 120, cases: 325 }, b: { successes: 202, cases: 593 } },
      { label: 'Dept D', a: { successes: 138, cases: 417 }, b: { successes: 131, cases: 375 } },
    ],
  },
  {
    label: 'Neutral (no paradox)',
    strata: [
      { label: 'Group 1', a: { successes: 60, cases: 100 }, b: { successes: 30, cases: 100 } },
      { label: 'Group 2', a: { successes: 20, cases: 100 }, b: { successes: 10, cases: 100 } },
    ],
  },
];

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
  const [strata, setStrata] = useState<Stratum[]>(PRESETS[0].strata);
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const chartRef = useRef<Chart | null>(null);

  const result = computeSandbox(strata);

  const updateCell = (si: number, trt: 'a' | 'b', field: 'successes' | 'cases', val: number) => {
    setStrata(prev => prev.map((s, i) => {
      if (i !== si) return s;
      const cell = { ...s[trt], [field]: val };
      if (field === 'cases' && cell.successes > val) cell.successes = val;
      return { ...s, [trt]: cell };
    }));
  };

  useEffect(() => {
    if (!canvasRef.current) return;
    if (chartRef.current) chartRef.current.destroy();
    const labels = [...result.strataDetail.map(s => `${s.label} A`), ...result.strataDetail.map(s => `${s.label} B`), 'Pooled A', 'Pooled B'];
    const data = [
      ...result.strataDetail.map(s => s.rA),
      ...result.strataDetail.map(s => s.rB),
      result.pooledA,
      result.pooledB,
    ];
    const colors = [
      ...result.strataDetail.map(() => '#58a6ff99'),
      ...result.strataDetail.map(() => '#d2a8ff99'),
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
  }, [strata]);

  return (
    <div>
      <div className="page-title">Interactive Sandbox</div>
      <div className="page-desc">
        Adjust successes and cases for each stratum and treatment to see how the paradox
        appears and disappears. This runs the same DAG as the Postgres views — entirely in
        the browser. Use the presets to start from known examples.
      </div>

      <div style={{ display: 'flex', gap: 10, marginBottom: 20, flexWrap: 'wrap' }}>
        {PRESETS.map(p => (
          <button
            key={p.label}
            onClick={() => setStrata(p.strata)}
            style={{
              background: '#21262d', border: '1px solid #30363d', borderRadius: 6,
              color: '#e6edf3', padding: '6px 14px', cursor: 'pointer', fontSize: 13,
            }}
          >
            {p.label}
          </button>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16, marginBottom: 16 }}>
        {strata.map((s, si) => (
          <div className="card" key={si}>
            <h3>Stratum: {s.label}</h3>
            <div style={{ marginBottom: 10, fontWeight: 600, fontSize: 12, color: '#58a6ff' }}>Treatment A</div>
            <SliderRow label="Successes A" field={`a-suc-${si}`} value={s.a.successes} max={s.a.cases}
              onChange={v => updateCell(si, 'a', 'successes', v)} />
            <SliderRow label="Cases A" field={`a-cas-${si}`} value={s.a.cases} max={2000}
              onChange={v => updateCell(si, 'a', 'cases', v)} />
            <div style={{ marginTop: 10, marginBottom: 10, fontWeight: 600, fontSize: 12, color: '#d2a8ff' }}>Treatment B</div>
            <SliderRow label="Successes B" field={`b-suc-${si}`} value={s.b.successes} max={s.b.cases}
              onChange={v => updateCell(si, 'b', 'successes', v)} />
            <SliderRow label="Cases B" field={`b-cas-${si}`} value={s.b.cases} max={2000}
              onChange={v => updateCell(si, 'b', 'cases', v)} />
            <div style={{ marginTop: 8, fontSize: 12, color: '#8b949e' }}>
              Rate A: <strong style={{ color: '#58a6ff' }}>
                {s.a.cases ? (s.a.successes / s.a.cases * 100).toFixed(1) : '—'}%
              </strong>
              {'  '}
              Rate B: <strong style={{ color: '#d2a8ff' }}>
                {s.b.cases ? (s.b.successes / s.b.cases * 100).toFixed(1) : '—'}%
              </strong>
            </div>
          </div>
        ))}
      </div>

      <div className="card">
        <h3>Live rates</h3>
        <div style={{ height: 240 }}><canvas ref={canvasRef} /></div>
      </div>

      <div className="two-col">
        <div className="card">
          <h3>Derived facts</h3>
          <div className="stat-row">
            <span className="stat-label">Pooled rate A</span>
            <span className="stat-value">{(result.pooledA * 100).toFixed(1)}%</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Pooled rate B</span>
            <span className="stat-value">{(result.pooledB * 100).toFixed(1)}%</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Pooled winner</span>
            <span className="stat-value">{result.pooledWinner}</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">IsReversal (unanimity)</span>
            <span className="stat-value" style={{ color: result.isReversal ? '#ff7b72' : '#7ee787' }}>
              {result.isReversal ? 'YES' : 'no'}
            </span>
          </div>
          <div className="stat-row">
            <span className="stat-label">IsSignFlip</span>
            <span className="stat-value" style={{ color: result.isSignFlip ? '#d2a8ff' : '#7ee787' }}>
              {result.isSignFlip ? 'YES' : 'no'}
            </span>
          </div>
        </div>
        <div className="card">
          <h3>Continuous measures</h3>
          <div className="stat-row">
            <span className="stat-label">Paradox strength</span>
            <span className="stat-value">{result.paradoxStrength.toFixed(4)}</span>
          </div>
          <div className="stat-row">
            <span className="stat-label">Allocation distortion</span>
            <span className="stat-value">{result.allocDistortion.toFixed(4)}</span>
          </div>
          {result.strataDetail.map(s => (
            <div className="stat-row" key={s.label}>
              <span className="stat-label">{s.label} winner</span>
              <span className="stat-value" style={{ color: '#58a6ff' }}>{s.winner}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
