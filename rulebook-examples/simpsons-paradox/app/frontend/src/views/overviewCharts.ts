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

/** Symlog scale constant — tuned for this corpus (most values &lt; 0.1). */
export const SYMLOG_K = 0.012;

export type ScaleMode = 'linear' | 'symlog';
export type RangeMode = 'focused' | 'full';

export interface ChartDisplayOptions {
  scaleMode?: ScaleMode;
  rangeMode?: RangeMode;
}

export interface PlaneChartMeta {
  clippedCount: number;
}

export function symlog(v: number, k = SYMLOG_K): number {
  if (v <= 0) return 0;
  return Math.log1p(v / k);
}

export function symexp(t: number, k = SYMLOG_K): number {
  if (t <= 0) return 0;
  return Math.expm1(t) * k;
}

export function jitterOrigin(x: number, y: number, seed: string, yCutoff = 0.003): { x: number; y: number } {
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

export interface DisplayBounds {
  xMin: number;
  xMax: number;
  yMin: number;
  yMax: number;
  capX: number;
  capY: number;
  clippedCount: number;
  scaleMode: ScaleMode;
}

function toDisplay(v: number, scaleMode: ScaleMode): number {
  return scaleMode === 'symlog' ? symlog(Math.max(0, v)) : v;
}

function formatAxisTick(v: string | number, scaleMode: ScaleMode, dpSmall = 3, dpLarge = 2): string {
  const n = Number(v);
  if (Number.isNaN(n) || n < 0) return '';
  const raw = scaleMode === 'symlog' ? symexp(n) : n;
  if (raw < 0) return '';
  return raw < 0.01 ? raw.toFixed(dpSmall) : raw.toFixed(dpLarge);
}

export function computePlaneBounds(
  rawXs: number[],
  rawYs: number[],
  options: ChartDisplayOptions = {},
): DisplayBounds {
  const rangeMode = options.rangeMode ?? 'focused';
  const scaleMode = options.scaleMode ?? 'linear';

  let xMin: number;
  let xMax: number;
  let yMin: number;
  let yMax: number;
  let capX: number;
  let capY: number;
  let clippedCount = 0;

  if (rangeMode === 'full') {
    const b = paddedBounds(rawXs, rawYs);
    ({ xMin, xMax, yMin, yMax } = b);
    capX = xMax;
    capY = yMax;
  } else {
    capX = Math.max(percentileCap(rawXs, 0.92) * 1.1, 0.07);
    capY = Math.max(percentileCap(rawYs, 0.92) * 1.1, 0.05);
    xMin = -0.006;
    yMin = -0.003;
    xMax = capX;
    yMax = capY;
    clippedCount = rawXs.filter((x, i) => x > capX || rawYs[i] > capY).length;
  }

  return {
    xMin: toDisplay(xMin, scaleMode),
    xMax: toDisplay(xMax, scaleMode),
    yMin: toDisplay(yMin, scaleMode),
    yMax: toDisplay(yMax, scaleMode),
    capX,
    capY,
    clippedCount,
    scaleMode,
  };
}

type PlanePoint = ScatterDataPoint & Record<string, unknown> & {
  label: string;
  _radius: number;
  clipped?: boolean;
};

function pointRadius(): number {
  return 4;
}

function mapPlanePoint(
  rawX: number,
  rawY: number,
  seed: string,
  bounds: DisplayBounds,
  rangeMode: RangeMode,
  yCutoff?: number,
): { x: number; y: number; clipped: boolean; rawX: number; rawY: number } {
  let { x, y } = jitterOrigin(rawX, rawY, seed, yCutoff);
  const clipped = rangeMode === 'focused' && (rawX > bounds.capX || rawY > bounds.capY);
  if (clipped) {
    x = Math.min(x, bounds.capX * 0.992);
    y = Math.min(y, bounds.capY * 0.992);
  }
  return {
    x: toDisplay(x, bounds.scaleMode),
    y: toDisplay(y, bounds.scaleMode),
    clipped,
    rawX,
    rawY,
  };
}

function byTypeDatasets(
  rankings: TreatmentRanking[],
  studyById: Record<string, Study>,
  mapPoint: (r: TreatmentRanking, study: Study | undefined) => PlanePoint,
) {
  const types = [...new Set(rankings.map(r => r.distortion_type))].sort();
  return types.map(t => {
    const color = TYPE_COLORS[t] ?? '#8b949e';
    const points = rankings
      .filter(r => r.distortion_type === t)
      .map(r => mapPoint(r, studyById[r.study]));
    return {
      label: `Type ${t}`,
      data: points,
      backgroundColor: points.map(p => (p.clipped ? 'transparent' : color + '66')),
      borderColor: points.map(p => color),
      borderWidth: points.map(p => (p.clipped ? 2 : 1.25)),
      pointRadius: points.map(p => (p.clipped ? 3.5 : p._radius)),
      pointHoverRadius: points.map(p => Math.min(8, (p.clipped ? 3.5 : p._radius) + 2)),
      pointStyle: points.map(p => (p.clipped ? 'rectRot' as const : 'circle' as const)),
    };
  });
}

function byTierDatasets(
  rankings: TreatmentRanking[],
  studyById: Record<string, Study>,
  mapPoint: (r: TreatmentRanking, study: Study | undefined) => PlanePoint,
) {
  return (['DANGER', 'CAUTION', 'SAFE'] as const).map(tier => {
    const color = TIER_COLORS[tier];
    const points = rankings
      .filter(r => r.screening_tier === tier)
      .map(r => mapPoint(r, studyById[r.study]));
    return {
      label: tier,
      data: points,
      backgroundColor: points.map(p => (p.clipped ? 'transparent' : color + '66')),
      borderColor: points.map(p => color),
      borderWidth: points.map(p => (p.clipped ? 2 : 1.25)),
      pointRadius: points.map(p => (p.clipped ? 3.5 : p._radius)),
      pointHoverRadius: points.map(p => Math.min(8, (p.clipped ? 3.5 : p._radius) + 2)),
      pointStyle: points.map(p => (p.clipped ? 'rectRot' as const : 'circle' as const)),
    };
  });
}

function baseScatterOptions(
  xTitle: string,
  yTitle: string,
  bounds: Pick<DisplayBounds, 'xMin' | 'xMax' | 'yMin' | 'yMax' | 'scaleMode'>,
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
        ticks: { color: '#6e7681', maxTicksLimit: 7, callback: xTick },
        grid: { color: '#21262d' },
        min: bounds.xMin,
        max: bounds.xMax,
      },
      y: {
        title: { display: true, text: yTitle, color: '#8b949e', font: { size: 11, weight: 'bold' as const } },
        ticks: { color: '#6e7681', maxTicksLimit: 7, callback: yTick },
        grid: { color: '#21262d' },
        min: bounds.yMin,
        max: bounds.yMax,
      },
    },
  };
}

function planeTickCallbacks(bounds: DisplayBounds) {
  const { scaleMode } = bounds;
  return {
    xTick: (v: string | number) => formatAxisTick(v, scaleMode, 3, 2),
    yTick: (v: string | number) => formatAxisTick(v, scaleMode, 3, 3),
  };
}

/** Canonical plane: allocation distortion × paradox strength (README contract). */
export function buildCanonicalPlaneChart(
  canvas: HTMLCanvasElement,
  rankings: TreatmentRanking[],
  studyById: Record<string, Study>,
  options: ChartDisplayOptions = {},
): { chart: Chart<'scatter'>; meta: PlaneChartMeta } {
  const rangeMode = options.rangeMode ?? 'focused';
  const rawXs = rankings.map(r => Number(r.allocation_distortion));
  const rawYs = rankings.map(r => Number(r.paradox_strength));
  const bounds = computePlaneBounds(rawXs, rawYs, options);
  const { xTick, yTick } = planeTickCallbacks(bounds);

  const datasets = byTypeDatasets(rankings, studyById, r => {
    const rawX = Number(r.allocation_distortion);
    const rawY = Number(r.paradox_strength);
    const mapped = mapPlanePoint(rawX, rawY, r.study, bounds, rangeMode);
    return {
      x: mapped.x,
      y: mapped.y,
      label: r.study,
      rawX: mapped.rawX,
      rawY: mapped.rawY,
      type: r.distortion_type,
      tier: r.screening_tier,
      signFlip: r.is_sign_flip,
      clipped: mapped.clipped,
      _radius: pointRadius(),
    };
  });

  const config: ChartConfiguration<'scatter'> = {
    type: 'scatter',
    data: { datasets },
    options: {
      ...baseScatterOptions('Allocation distortion →', 'Paradox strength →', bounds, xTick, yTick),
      plugins: {
        ...baseScatterOptions('', '', bounds, xTick, yTick).plugins,
        tooltip: {
          ...baseScatterOptions('', '', bounds, xTick, yTick).plugins!.tooltip,
          callbacks: {
            title: items => String((items[0]?.raw as PlanePoint)?.label ?? ''),
            label: ctx => {
              const d = ctx.raw as PlanePoint & {
                rawX: number; rawY: number; type: string; tier: string; signFlip: boolean; clipped?: boolean;
              };
              const lines = [
                `Type ${d.type} · ${d.tier}`,
                `Allocation distortion: ${d.rawX.toFixed(4)}`,
                `Paradox strength: ${d.rawY.toFixed(4)}`,
                d.signFlip ? 'Sign flip: yes' : 'Sign flip: no',
              ];
              if (d.clipped) lines.push('Pinned at edge — switch to Full range');
              return lines;
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
  return { chart: new Chart(canvas, config), meta: { clippedCount: bounds.clippedCount } };
}

/** Finding 1: pooled gap vs corrected (stratum-weighted) gap — reversal recovery. */
export function buildRecoveryChart(
  canvas: HTMLCanvasElement,
  rankings: TreatmentRanking[],
  studyById: Record<string, Study>,
): Chart<'scatter'> {
  const flip = rankings.filter(r => r.is_sign_flip);
  const datasets = byTypeDatasets(flip, studyById, r => {
    const rawX = Number(r.signed_pooled_gap);
    const rawY = Number(r.weighted_stratum_gap_sum);
    return {
      x: rawX, y: rawY, label: r.study, rawX, rawY,
      type: r.distortion_type,
      _radius: pointRadius(),
    };
  });

  const pts = datasets.flatMap(d => d.data);
  const xs = pts.map(p => Number(p.x));
  const ys = pts.map(p => Number(p.y));
  const all = [...xs, ...ys];
  const lo = Math.min(...all) - 0.02;
  const hi = Math.max(...all) + 0.02;
  const bounds = { xMin: lo, xMax: hi, yMin: lo, yMax: hi, scaleMode: 'linear' as const };

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
        const lineLo = bounds.xMin;
        const lineHi = bounds.xMax;
        ctx.save();
        ctx.strokeStyle = '#484f58';
        ctx.setLineDash([5, 5]);
        ctx.lineWidth = 1;
        ctx.beginPath();
        ctx.moveTo(x.getPixelForValue(lineLo), y.getPixelForValue(lineLo));
        ctx.lineTo(x.getPixelForValue(lineHi), y.getPixelForValue(lineHi));
        ctx.stroke();
        ctx.setLineDash([]);
        ctx.fillStyle = '#6e7681';
        ctx.font = '500 10px system-ui';
        ctx.textAlign = 'left';
        ctx.fillText('y = x  (pooled agrees with corrected)', x.getPixelForValue(lineLo) + 6, y.getPixelForValue(lineHi) + 14);
        ctx.restore();
      },
    }],
  };
  return new Chart(canvas, config);
}

/** Finding 2: screening tiers on allocation vs severity plane. */
export function buildScreeningChart(
  canvas: HTMLCanvasElement,
  rankings: TreatmentRanking[],
  studyById: Record<string, Study>,
  options: ChartDisplayOptions = {},
): Chart<'scatter'> {
  const rangeMode = options.rangeMode ?? 'focused';
  const rawXs = rankings.map(r => Number(r.allocation_distortion));
  const rawYs = rankings.map(r => Number(r.paradox_strength));
  const bounds = computePlaneBounds(rawXs, rawYs, { ...options, rangeMode });
  const { xTick, yTick } = planeTickCallbacks(bounds);

  const datasets = byTierDatasets(rankings, studyById, r => {
    const rawX = Number(r.allocation_distortion);
    const rawY = Number(r.paradox_strength);
    const mapped = mapPlanePoint(rawX, rawY, r.study, bounds, rangeMode, 0.06);
    return {
      x: mapped.x,
      y: mapped.y,
      label: r.study,
      rawX: mapped.rawX,
      rawY: mapped.rawY,
      tier: r.screening_tier,
      type: r.distortion_type,
      clipped: mapped.clipped,
      _radius: pointRadius(),
    };
  });

  const config: ChartConfiguration<'scatter'> = {
    type: 'scatter',
    data: { datasets },
    options: {
      ...baseScatterOptions('Allocation distortion →', 'Paradox strength →', bounds, xTick, yTick),
      plugins: {
        ...baseScatterOptions('', '', bounds, xTick, yTick).plugins,
        tooltip: {
          callbacks: {
            title: items => String((items[0]?.raw as PlanePoint)?.label ?? ''),
            label: ctx => {
              const d = ctx.raw as PlanePoint & { rawX: number; rawY: number; tier: string; type: string; clipped?: boolean };
              const lines = [
                `${d.tier} · Type ${d.type}`,
                `Distortion: ${d.rawX.toFixed(4)}`,
                `Strength: ${d.rawY.toFixed(4)}`,
              ];
              if (d.clipped) lines.push('Pinned at edge — switch to Full range');
              return lines;
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
  options: ChartDisplayOptions = {},
): Chart<'scatter'> {
  const withPurity = rankings.filter(r => r.signal_purity != null);
  const rangeMode = options.rangeMode ?? 'focused';
  const scaleMode = options.scaleMode ?? 'linear';
  const rawXs = withPurity.map(r => Number(r.paradox_strength));
  const rawYs = withPurity.map(r => Number(r.signal_purity));

  let capX: number;
  let clippedCount = 0;
  if (rangeMode === 'full') {
    capX = Math.max(...rawXs, 0.1);
  } else {
    capX = Math.max(percentileCap(rawXs, 0.92) * 1.1, 0.07);
    clippedCount = rawXs.filter(x => x > capX).length;
  }

  const bounds: DisplayBounds = {
    xMin: toDisplay(-0.006, scaleMode),
    xMax: toDisplay(capX, scaleMode),
    yMin: -0.02,
    yMax: 1.08,
    capX,
    capY: 1.08,
    clippedCount,
    scaleMode,
  };
  const xTick = (v: string | number) => formatAxisTick(v, scaleMode, 3, 3);
  const yTick = (v: string | number) => (Number(v) < 0 ? '' : Number(v).toFixed(2));

  const datasets = byTypeDatasets(withPurity, studyById, r => {
    const rawX = Number(r.paradox_strength);
    const rawY = Number(r.signal_purity);
    let { x, y } = jitterOrigin(rawX, rawY, r.study, 0.04);
    const clipped = rangeMode === 'focused' && rawX > capX;
    if (clipped) x = Math.min(x, capX * 0.992);
    return {
      x: toDisplay(x, scaleMode),
      y,
      label: r.study,
      rawX,
      rawY,
      type: r.distortion_type,
      pure: rawY < 0.5,
      clipped,
      _radius: pointRadius(),
    };
  });

  const config: ChartConfiguration<'scatter'> = {
    type: 'scatter',
    data: { datasets },
    options: {
      ...baseScatterOptions('Paradox strength →', 'Signal purity →', bounds, xTick, yTick),
      plugins: {
        ...baseScatterOptions('', '', bounds, xTick, yTick).plugins,
        tooltip: {
          callbacks: {
            title: items => String((items[0]?.raw as PlanePoint)?.label ?? ''),
            label: ctx => {
              const d = ctx.raw as PlanePoint & { rawX: number; rawY: number; type: string; pure: boolean; clipped?: boolean };
              const lines = [
                `Type ${d.type}`,
                `Paradox strength: ${d.rawX.toFixed(4)}`,
                `Signal purity: ${d.rawY.toFixed(3)}`,
                d.pure ? 'Below 0.5 — allocation noise dominates' : 'Pooled signal mostly real',
              ];
              if (d.clipped) lines.push('Pinned at edge — switch to Full range');
              return lines;
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
