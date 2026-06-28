import { Chart, type ChartConfiguration, type ScatterDataPoint } from 'chart.js/auto';
import type { Study, TreatmentRanking } from '../types';

export const TYPE_COLORS: Record<string, string> = {
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

export function jitterOrigin(x: number, y: number, seed: string, yCutoff = 0.003): { x: number; y: number } {
  if (x > 0.004 || y > yCutoff) return { x, y };
  let h = 0;
  for (let i = 0; i < seed.length; i++) h = (h * 31 + seed.charCodeAt(i)) | 0;
  const angle = ((h & 0xffff) / 0xffff) * Math.PI * 2;
  const r = 0.003 + ((h >> 16) & 0xff) / 0xff * 0.006;
  return { x: x + Math.cos(angle) * r, y: Math.max(0, y + Math.sin(angle) * r * 0.5) };
}

export function paddedBounds(xs: number[], ys: number[], yMinFloor = -0.008, yMaxCap?: number) {
  const dataMinX = Math.min(...xs);
  const dataMaxX = Math.max(...xs);
  const dataMinY = Math.min(...ys);
  const dataMaxY = Math.max(...ys);
  const xSpan = Math.max(dataMaxX - dataMinX, 0.035);
  const ySpan = Math.max(dataMaxY - dataMinY, 0.015);
  const xPad = xSpan * 0.16 + 0.01;
  const yPad = ySpan * 0.2 + 0.008;
  return {
    xMin: Math.min(-0.018, dataMinX - xPad),
    xMax: dataMaxX + xPad,
    yMin: Math.min(yMinFloor, dataMinY - yPad),
    yMax: yMaxCap ?? dataMaxY + yPad,
  };
}

type PlanePoint = ScatterDataPoint & Record<string, unknown> & { label: string; _radius: number };

function byTypeDatasets(
  rankings: TreatmentRanking[],
  studyById: Record<string, Study>,
  mapPoint: (r: TreatmentRanking, study: Study | undefined) => PlanePoint,
) {
  const types = [...new Set(rankings.map(r => r.distortion_type))].sort();
  return types.map(t => {
    const points = rankings
      .filter(r => r.distortion_type === t)
      .map(r => mapPoint(r, studyById[r.study]));
    return {
      label: `Type ${t}`,
      data: points,
      backgroundColor: (TYPE_COLORS[t] ?? '#8b949e') + 'cc',
      borderColor: TYPE_COLORS[t] ?? '#8b949e',
      borderWidth: 1.5,
      pointRadius: points.map(p => p._radius),
      pointHoverRadius: points.map(p => Math.min(18, p._radius + 3)),
    };
  });
}

function byTierDatasets(
  rankings: TreatmentRanking[],
  studyById: Record<string, Study>,
  mapPoint: (r: TreatmentRanking, study: Study | undefined) => PlanePoint,
) {
  return (['DANGER', 'CAUTION', 'SAFE'] as const).map(tier => {
    const points = rankings
      .filter(r => r.screening_tier === tier)
      .map(r => mapPoint(r, studyById[r.study]));
    return {
      label: tier,
      data: points,
      backgroundColor: TIER_COLORS[tier] + 'cc',
      borderColor: TIER_COLORS[tier],
      borderWidth: 1.5,
      pointRadius: points.map(p => p._radius),
      pointHoverRadius: points.map(p => Math.min(18, p._radius + 3)),
    };
  });
}

function baseScatterOptions(
  xTitle: string,
  yTitle: string,
  bounds: ReturnType<typeof paddedBounds>,
  xTick: (v: string | number) => string,
  yTick: (v: string | number) => string,
) {
  return {
    responsive: true,
    maintainAspectRatio: false,
    layout: { padding: { top: 10, right: 14, bottom: 6, left: 6 } },
    plugins: {
      legend: {
        position: 'bottom' as const,
        labels: { color: '#8b949e', font: { size: 10 }, boxWidth: 8, padding: 12 },
      },
      tooltip: {
        backgroundColor: '#21262d',
        titleColor: '#e6edf3',
        bodyColor: '#8b949e',
        borderColor: '#30363d',
        borderWidth: 1,
        padding: 10,
      },
    },
    scales: {
      x: {
        title: { display: true, text: xTitle, color: '#8b949e', font: { size: 11, weight: 'bold' as const } },
        ticks: { color: '#6e7681', callback: xTick },
        grid: { color: '#21262d' },
        min: bounds.xMin,
        max: bounds.xMax,
      },
      y: {
        title: { display: true, text: yTitle, color: '#8b949e', font: { size: 11, weight: 'bold' as const } },
        ticks: { color: '#6e7681', callback: yTick },
        grid: { color: '#21262d' },
        min: bounds.yMin,
        max: bounds.yMax,
      },
    },
  };
}

function pointRadius(study: Study | undefined, base = 7): number {
  const cases = study?.total_cases ?? 100;
  return Math.min(14, base + Math.sqrt(cases) * 0.22);
}

/** Canonical plane: allocation distortion × paradox strength (README contract). */
export function buildCanonicalPlaneChart(
  canvas: HTMLCanvasElement,
  rankings: TreatmentRanking[],
  studyById: Record<string, Study>,
): Chart<'scatter'> {
  const datasets = byTypeDatasets(rankings, studyById, (r, study) => {
    const rawX = Number(r.allocation_distortion);
    const rawY = Number(r.paradox_strength);
    const { x, y } = jitterOrigin(rawX, rawY, r.study);
    return {
      x, y, label: r.study, rawX, rawY,
      type: r.distortion_type,
      tier: r.screening_tier,
      signFlip: r.is_sign_flip,
      _radius: pointRadius(study),
    };
  });

  const pts = datasets.flatMap(d => d.data);
  const bounds = paddedBounds(
    pts.map(p => Number(p.x)),
    pts.map(p => Number(p.y)),
  );

  const config: ChartConfiguration<'scatter'> = {
    type: 'scatter',
    data: { datasets },
    options: {
      ...baseScatterOptions(
        'Allocation distortion →',
        'Paradox strength →',
        bounds,
        v => Number(v) < 0 ? '' : Number(v).toFixed(2),
        v => Number(v) < 0 ? '' : Number(v).toFixed(3),
      ),
      plugins: {
        ...baseScatterOptions('', '', bounds, v => String(v), v => String(v)).plugins,
        tooltip: {
          ...baseScatterOptions('', '', bounds, v => String(v), v => String(v)).plugins!.tooltip,
          callbacks: {
            title: items => String((items[0]?.raw as PlanePoint)?.label ?? ''),
            label: ctx => {
              const d = ctx.raw as PlanePoint & { rawX: number; rawY: number; type: string; tier: string; signFlip: boolean };
              return [
                `Type ${d.type} · ${d.tier}`,
                `Allocation distortion: ${d.rawX.toFixed(4)}`,
                `Paradox strength: ${d.rawY.toFixed(4)}`,
                d.signFlip ? 'Sign flip: yes' : 'Sign flip: no',
              ];
            },
          },
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
  };
  return new Chart(canvas, config);
}

/** Finding 1: pooled gap vs corrected (stratum-weighted) gap — reversal recovery. */
export function buildRecoveryChart(
  canvas: HTMLCanvasElement,
  rankings: TreatmentRanking[],
  studyById: Record<string, Study>,
): Chart<'scatter'> {
  const flip = rankings.filter(r => r.is_sign_flip);
  const datasets = byTypeDatasets(flip, studyById, (r, study) => {
    const rawX = Number(r.signed_pooled_gap);
    const rawY = Number(r.weighted_stratum_gap_sum);
    return {
      x: rawX, y: rawY, label: r.study, rawX, rawY,
      type: r.distortion_type,
      _radius: pointRadius(study, 8),
    };
  });

  const pts = datasets.flatMap(d => d.data);
  const xs = pts.map(p => Number(p.x));
  const ys = pts.map(p => Number(p.y));
  const all = [...xs, ...ys];
  const lo = Math.min(...all) - 0.02;
  const hi = Math.max(...all) + 0.02;
  const bounds = { xMin: lo, xMax: hi, yMin: lo, yMax: hi };

  const config: ChartConfiguration<'scatter'> = {
    type: 'scatter',
    data: { datasets },
    options: {
      ...baseScatterOptions(
        'Signed pooled gap →',
        'Corrected gap (stratum-weighted) →',
        bounds,
        v => Number(v).toFixed(2),
        v => Number(v).toFixed(2),
      ),
      plugins: {
        ...baseScatterOptions('', '', bounds, v => String(v), v => String(v)).plugins,
        tooltip: {
          callbacks: {
            title: items => String((items[0]?.raw as PlanePoint)?.label ?? ''),
            label: ctx => {
              const d = ctx.raw as PlanePoint & { rawX: number; rawY: number; type: string };
              return [
                `Type ${d.type}`,
                `Pooled: ${(d.rawX * 100).toFixed(2)}pp`,
                `Corrected: ${(d.rawY * 100).toFixed(2)}pp`,
                'Off the diagonal = pooled misleads',
              ];
            },
          },
        },
      },
    },
    plugins: [{
      id: 'agreementLine',
      beforeDraw(chart) {
        const { ctx, scales: { x, y } } = chart;
        const lo = bounds.xMin;
        const hi = bounds.xMax;
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
        ctx.font = '500 10px system-ui';
        ctx.textAlign = 'left';
        ctx.fillText('y = x  (pooled agrees with corrected)', x.getPixelForValue(lo) + 6, y.getPixelForValue(hi) + 14);
        ctx.restore();
      },
    }],
  };
  return new Chart(canvas, config);
}

/** Finding 2: screening tiers on the instrument plane. */
export function buildScreeningChart(
  canvas: HTMLCanvasElement,
  rankings: TreatmentRanking[],
  studyById: Record<string, Study>,
): Chart<'scatter'> {
  const datasets = byTierDatasets(rankings, studyById, (r, study) => {
    const rawX = Number(r.allocation_distortion);
    const rawY = Number(r.instrument_score);
    const { x, y } = jitterOrigin(rawX, rawY, r.study, 0.06);
    return {
      x, y, label: r.study, rawX, rawY,
      tier: r.screening_tier,
      type: r.distortion_type,
      _radius: pointRadius(study, 7),
    };
  });

  const pts = datasets.flatMap(d => d.data);
  const bounds = paddedBounds(
    pts.map(p => Number(p.x)),
    pts.map(p => Number(p.y)),
    -0.15,
  );

  const config: ChartConfiguration<'scatter'> = {
    type: 'scatter',
    data: { datasets },
    options: {
      ...baseScatterOptions(
        'Allocation distortion →',
        'Instrument score →',
        bounds,
        v => Number(v) < 0 ? '' : Number(v).toFixed(2),
        v => Number(v) < 0 ? '' : Number(v).toFixed(1),
      ),
      plugins: {
        ...baseScatterOptions('', '', bounds, v => String(v), v => String(v)).plugins,
        tooltip: {
          callbacks: {
            title: items => String((items[0]?.raw as PlanePoint)?.label ?? ''),
            label: ctx => {
              const d = ctx.raw as PlanePoint & { rawX: number; rawY: number; tier: string; type: string };
              return [
                `${d.tier} · Type ${d.type}`,
                `Distortion: ${d.rawX.toFixed(4)}`,
                `Instrument score: ${d.rawY.toFixed(3)}`,
              ];
            },
          },
        },
      },
    },
  };
  return new Chart(canvas, config);
}

/** Finding 3: signal purity vs paradox strength — when pooling is mostly noise. */
export function buildPurityChart(
  canvas: HTMLCanvasElement,
  rankings: TreatmentRanking[],
  studyById: Record<string, Study>,
): Chart<'scatter'> {
  const withPurity = rankings.filter(r => r.signal_purity != null);
  const datasets = byTypeDatasets(withPurity, studyById, (r, study) => {
    const rawX = Number(r.paradox_strength);
    const rawY = Number(r.signal_purity);
    const { x, y } = jitterOrigin(rawX, rawY, r.study, 0.04);
    return {
      x, y, label: r.study, rawX, rawY,
      type: r.distortion_type,
      pure: rawY < 0.5,
      _radius: pointRadius(study, 7),
    };
  });

  const pts = datasets.flatMap(d => d.data);
  const bounds = paddedBounds(
    pts.map(p => Number(p.x)),
    pts.map(p => Number(p.y)),
    -0.02,
    1.08,
  );

  const config: ChartConfiguration<'scatter'> = {
    type: 'scatter',
    data: { datasets },
    options: {
      ...baseScatterOptions(
        'Paradox strength →',
        'Signal purity →',
        bounds,
        v => Number(v) < 0 ? '' : Number(v).toFixed(3),
        v => Number(v) < 0 ? '' : Number(v).toFixed(2),
      ),
      plugins: {
        ...baseScatterOptions('', '', bounds, v => String(v), v => String(v)).plugins,
        tooltip: {
          callbacks: {
            title: items => String((items[0]?.raw as PlanePoint)?.label ?? ''),
            label: ctx => {
              const d = ctx.raw as PlanePoint & { rawX: number; rawY: number; type: string; pure: boolean };
              return [
                `Type ${d.type}`,
                `Paradox strength: ${d.rawX.toFixed(4)}`,
                `Signal purity: ${d.rawY.toFixed(3)}`,
                d.pure ? 'Below 0.5 — allocation noise dominates' : 'Pooled signal mostly real',
              ];
            },
          },
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
        ctx.font = '500 10px system-ui';
        ctx.textAlign = 'left';
        ctx.fillText('purity = 0.5 threshold', left + 8, y50 - 6);
        ctx.restore();
      },
    }],
  };
  return new Chart(canvas, config);
}
