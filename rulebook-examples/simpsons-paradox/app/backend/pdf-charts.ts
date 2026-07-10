/** Server-side PNG renders of the four Overview charts, for embedding in the summary PDF.
 *  Math/config ported from app/frontend/src/views/overviewCharts.ts — same bounds/jitter/symlog
 *  logic, minus interactive-only bits (tooltips, hover) which don't fire in a static render. */
import { ChartJSNodeCanvas } from 'chartjs-node-canvas';
import type { ChartConfiguration } from 'chart.js';

export interface RankingRow {
  study: string;
  distortion_type: string;
  screening_tier: string;
  is_sign_flip: boolean;
  allocation_distortion: number;
  paradox_strength: number;
  signed_pooled_gap: number;
  weighted_stratum_gap_sum: number;
  signal_purity: number | null;
}

const TYPE_COLORS: Record<string, string> = {
  A: '#ff7b72',
  B: '#d2a8ff',
  C: '#7ee787',
  'C+': '#79c0ff',
  'C-': '#56d364',
  D: '#6e7681',
};

const TIER_COLORS: Record<string, string> = {
  DANGER: '#ff7b72',
  CAUTION: '#e3b341',
  SAFE: '#7ee787',
};

const SYMLOG_K = 0.012;

function symlog(v: number, k = SYMLOG_K): number {
  if (v <= 0) return 0;
  return Math.log1p(v / k);
}

function jitterOrigin(x: number, y: number, seed: string, yCutoff = 0.003): { x: number; y: number } {
  if (x > 0.004 || y > yCutoff) return { x, y };
  let h = 0;
  for (let i = 0; i < seed.length; i++) h = (h * 31 + seed.charCodeAt(i)) | 0;
  const angle = ((h & 0xffff) / 0xffff) * Math.PI * 2;
  const r = 0.003 + ((h >> 16) & 0xff) / 0xff * 0.006;
  return { x: x + Math.cos(angle) * r, y: Math.max(0, y + Math.sin(angle) * r * 0.5) };
}

function percentileCap(values: number[], p: number): number {
  if (!values.length) return 0.1;
  const sorted = [...values].sort((a, b) => a - b);
  const idx = Math.min(sorted.length - 1, Math.ceil(p * sorted.length) - 1);
  return sorted[Math.max(0, idx)];
}

const DARK_BG = '#0d1117';
const AXIS_COLOR = '#8b949e';
const GRID_COLOR = '#21262d';

function baseOptions(xTitle: string, yTitle: string, bounds: { xMin: number; xMax: number; yMin: number; yMax: number }) {
  return {
    responsive: false,
    animation: false as const,
    layout: { padding: { top: 12, right: 16, bottom: 8, left: 8 } },
    plugins: {
      legend: {
        position: 'bottom' as const,
        labels: { color: AXIS_COLOR, font: { size: 13 }, boxWidth: 10, padding: 14 },
      },
    },
    scales: {
      x: {
        title: { display: true, text: xTitle, color: AXIS_COLOR, font: { size: 13, weight: 'bold' as const } },
        ticks: { color: '#6e7681', maxTicksLimit: 7, font: { size: 11 } },
        grid: { color: GRID_COLOR },
        min: bounds.xMin,
        max: bounds.xMax,
      },
      y: {
        title: { display: true, text: yTitle, color: AXIS_COLOR, font: { size: 13, weight: 'bold' as const } },
        ticks: { color: '#6e7681', maxTicksLimit: 7, font: { size: 11 } },
        grid: { color: GRID_COLOR },
        min: bounds.yMin,
        max: bounds.yMax,
      },
    },
  };
}

function byTypeDatasets(rows: RankingRow[], point: (r: RankingRow) => { x: number; y: number }) {
  const types = [...new Set(rows.map(r => r.distortion_type))].sort();
  return types.map(t => {
    const color = TYPE_COLORS[t] ?? '#8b949e';
    const points = rows.filter(r => r.distortion_type === t).map(point);
    return {
      label: `Type ${t}`,
      data: points,
      backgroundColor: color + '99',
      borderColor: color,
      borderWidth: 1.25,
      pointRadius: 4,
    };
  });
}

const renderer = new ChartJSNodeCanvas({
  width: 900,
  height: 480,
  backgroundColour: DARK_BG,
  chartCallback: ChartJS => {
    ChartJS.defaults.font.family = 'Helvetica, Arial, sans-serif';
  },
});

async function render(config: ChartConfiguration<'scatter'>): Promise<Buffer> {
  return renderer.renderToBuffer(config as ChartConfiguration);
}

/** Chart 1 — Allocation distortion plane (hero chart): distortion type vs paradox strength. */
export async function renderPlaneChart(rows: RankingRow[]): Promise<Buffer> {
  const rawXs = rows.map(r => Number(r.allocation_distortion));
  const rawYs = rows.map(r => Number(r.paradox_strength));
  const capX = Math.max(percentileCap(rawXs, 0.92) * 1.1, 0.07);
  const capY = Math.max(percentileCap(rawYs, 0.92) * 1.1, 0.05);
  const bounds = { xMin: -0.006, xMax: capX, yMin: -0.003, yMax: capY };

  const datasets = byTypeDatasets(rows, r => {
    const rawX = Number(r.allocation_distortion);
    const rawY = Number(r.paradox_strength);
    let { x, y } = jitterOrigin(rawX, rawY, r.study);
    x = Math.min(x, capX * 0.992);
    y = Math.min(y, capY * 0.992);
    return { x, y };
  });

  return render({
    type: 'scatter',
    data: { datasets },
    options: {
      ...baseOptions('Allocation distortion →', 'Paradox strength →', bounds),
      plugins: {
        ...baseOptions('', '', bounds).plugins,
        title: {
          display: true,
          text: 'Allocation distortion vs. paradox strength, by distortion type',
          color: '#e6edf3',
          font: { size: 15, weight: 'bold' as const },
          padding: { bottom: 12 },
        },
      },
    },
    plugins: [{
      id: 'diagonalRise',
      beforeDraw(chart) {
        const { ctx, chartArea: { left, right, top, bottom } } = chart;
        const g = ctx.createLinearGradient(left, bottom, right, top);
        g.addColorStop(0, 'rgba(110, 118, 129, 0.07)');
        g.addColorStop(0.55, 'rgba(121, 192, 255, 0.04)');
        g.addColorStop(1, 'rgba(255, 123, 114, 0.09)');
        ctx.save();
        ctx.fillStyle = g;
        ctx.fillRect(left, top, right - left, bottom - top);
        ctx.restore();
      },
    }],
  });
}

/** Chart 2 — Reversal recovery: pooled gap vs. corrected (stratum-weighted) gap, sign-flips only. */
export async function renderRecoveryChart(rows: RankingRow[]): Promise<Buffer> {
  const flip = rows.filter(r => r.is_sign_flip);
  const datasets = byTypeDatasets(flip, r => ({
    x: Number(r.signed_pooled_gap),
    y: Number(r.weighted_stratum_gap_sum),
  }));
  const pts = datasets.flatMap(d => d.data);
  const all = [...pts.map(p => p.x), ...pts.map(p => p.y)];
  const lo = Math.min(...all) - 0.02;
  const hi = Math.max(...all) + 0.02;
  const bounds = { xMin: lo, xMax: hi, yMin: lo, yMax: hi };

  return render({
    type: 'scatter',
    data: { datasets },
    options: {
      ...baseOptions('Signed pooled gap →', 'Corrected gap (stratum-weighted) →', bounds),
      plugins: {
        ...baseOptions('', '', bounds).plugins,
        title: {
          display: true,
          text: 'Reversal recovery — pooled gap vs. corrected gap (sign-flip studies)',
          color: '#e6edf3',
          font: { size: 15, weight: 'bold' as const },
          padding: { bottom: 12 },
        },
      },
    },
    plugins: [{
      id: 'agreementLine',
      beforeDraw(chart) {
        const { ctx, scales: { x, y } } = chart;
        ctx.save();
        ctx.strokeStyle = '#484f58';
        ctx.setLineDash([5, 5]);
        ctx.lineWidth = 1;
        ctx.beginPath();
        ctx.moveTo(x.getPixelForValue(lo), y.getPixelForValue(lo));
        ctx.lineTo(x.getPixelForValue(hi), y.getPixelForValue(hi));
        ctx.stroke();
        ctx.setLineDash([]);
        ctx.fillStyle = '#6e7681';
        ctx.font = '500 11px Helvetica';
        ctx.textAlign = 'left';
        ctx.fillText('y = x  (pooled agrees with corrected)', x.getPixelForValue(lo) + 6, y.getPixelForValue(hi) + 16);
        ctx.restore();
      },
    }],
  });
}

/** Chart 3 — Screening tiers (DANGER/CAUTION/SAFE) on the same plane as Chart 1. */
export async function renderScreeningChart(rows: RankingRow[]): Promise<Buffer> {
  const rawXs = rows.map(r => Number(r.allocation_distortion));
  const rawYs = rows.map(r => Number(r.paradox_strength));
  const capX = Math.max(percentileCap(rawXs, 0.92) * 1.1, 0.07);
  const capY = Math.max(percentileCap(rawYs, 0.92) * 1.1, 0.05);
  const bounds = { xMin: -0.006, xMax: capX, yMin: -0.003, yMax: capY };

  const datasets = (['DANGER', 'CAUTION', 'SAFE'] as const).map(tier => {
    const color = TIER_COLORS[tier];
    const points = rows
      .filter(r => r.screening_tier === tier)
      .map(r => {
        const rawX = Number(r.allocation_distortion);
        const rawY = Number(r.paradox_strength);
        let { x, y } = jitterOrigin(rawX, rawY, r.study, 0.06);
        x = Math.min(x, capX * 0.992);
        y = Math.min(y, capY * 0.992);
        return { x, y };
      });
    return {
      label: tier,
      data: points,
      backgroundColor: color + '99',
      borderColor: color,
      borderWidth: 1.25,
      pointRadius: 4,
    };
  });

  return render({
    type: 'scatter',
    data: { datasets },
    options: {
      ...baseOptions('Allocation distortion →', 'Paradox strength →', bounds),
      plugins: {
        ...baseOptions('', '', bounds).plugins,
        title: {
          display: true,
          text: 'Screening tiers separate safe studies from dangerous ones',
          color: '#e6edf3',
          font: { size: 15, weight: 'bold' as const },
          padding: { bottom: 12 },
        },
      },
    },
  });
}

/** Chart 4 — Signal purity vs. paradox strength, with the purity=0.5 threshold marked. */
export async function renderPurityChart(rows: RankingRow[]): Promise<Buffer> {
  const withPurity = rows.filter(r => r.signal_purity != null);
  const rawXs = withPurity.map(r => Number(r.paradox_strength));
  const capX = Math.max(percentileCap(rawXs, 0.92) * 1.1, 0.07);
  const bounds = { xMin: -0.006, xMax: capX, yMin: -0.02, yMax: 1.08 };

  const datasets = byTypeDatasets(withPurity, r => {
    const rawX = Number(r.paradox_strength);
    const rawY = Number(r.signal_purity);
    let { x, y } = jitterOrigin(rawX, rawY, r.study, 0.04);
    x = Math.min(x, capX * 0.992);
    return { x, y };
  });

  return render({
    type: 'scatter',
    data: { datasets },
    options: {
      ...baseOptions('Paradox strength →', 'Signal purity →', bounds),
      plugins: {
        ...baseOptions('', '', bounds).plugins,
        title: {
          display: true,
          text: 'Signal purity marks suspect aggregates (below 0.5 = allocation noise dominates)',
          color: '#e6edf3',
          font: { size: 15, weight: 'bold' as const },
          padding: { bottom: 12 },
        },
      },
    },
    plugins: [{
      id: 'purityThreshold',
      beforeDraw(chart) {
        const { ctx, chartArea: { left, right }, scales: { y } } = chart;
        const y50 = y.getPixelForValue(0.5);
        ctx.save();
        ctx.setLineDash([4, 4]);
        ctx.strokeStyle = '#e3b34188';
        ctx.beginPath();
        ctx.moveTo(left, y50);
        ctx.lineTo(right, y50);
        ctx.stroke();
        ctx.fillStyle = '#e3b341';
        ctx.font = '500 11px Helvetica';
        ctx.textAlign = 'left';
        ctx.fillText('purity = 0.5 threshold', left + 8, y50 - 8);
        ctx.restore();
      },
    }],
  });
}
