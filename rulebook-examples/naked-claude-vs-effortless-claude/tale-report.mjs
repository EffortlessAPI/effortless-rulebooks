#!/usr/bin/env node
// tale-report.mjs — turn one (or more) experiment dirs into a self-contained
// HTML report. No external deps; charts are hand-rolled SVG.
//
// Usage:
//   node tale-report.mjs --dir <experiment-dir>
//   node tale-report.mjs --compare <dir1> <dir2> [<dir3> ...]

import { existsSync, readFileSync, writeFileSync } from "node:fs";
import { basename, join, resolve } from "node:path";

// ---------- arg parsing ----------
const argv = process.argv.slice(2);

function getFlag(name) {
    const i = argv.indexOf(name);
    return i >= 0 ? argv[i + 1] : null;
}

const dir = getFlag("--dir");
const compareIdx = argv.indexOf("--compare");

if (!dir && compareIdx < 0) {
    console.error("Usage: node tale-report.mjs --dir <exp-dir>");
    console.error("       node tale-report.mjs --compare <dir1> <dir2> ...");
    process.exit(2);
}

// ---------- helpers ----------
function loadRun(expDir) {
    const taleDir = join(expDir, ".tale");
    const runPath = join(taleDir, "run.json");
    const jsonlPath = join(taleDir, "tokens.jsonl");

    const run = existsSync(runPath) ? JSON.parse(readFileSync(runPath, "utf8")) : {};
    const samples = [];
    if (existsSync(jsonlPath)) {
        const text = readFileSync(jsonlPath, "utf8");
        for (const line of text.split("\n")) {
            const trimmed = line.trim();
            if (!trimmed) continue;
            try { samples.push(JSON.parse(trimmed)); } catch {}
        }
    }
    return { expDir, run, samples };
}

function escapeHtml(s) {
    return String(s)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#39;");
}

function fmtNum(n) {
    if (n == null || !Number.isFinite(n)) return "—";
    return n.toLocaleString();
}

function fmtUsd(n) {
    if (n == null || !Number.isFinite(n)) return "—";
    return `$${n.toFixed(2)}`;
}

// ---------- chart primitives ----------
function svgLineChart({ width, height, padding, series, xLabel, yLabel, title }) {
    // series: [{ name, color, points: [[x,y],...], dash? }]
    const allPts = series.flatMap(s => s.points);
    if (allPts.length === 0) {
        return `<svg width="${width}" height="${height}"><text x="${width/2}" y="${height/2}" text-anchor="middle" fill="#888">no data</text></svg>`;
    }

    const xs = allPts.map(p => p[0]);
    const ys = allPts.map(p => p[1]);
    const xMin = 0;
    const xMax = Math.max(1, Math.max(...xs));
    const yMin = 0;
    const yMax = Math.max(1, Math.max(...ys));

    const innerW = width - padding.l - padding.r;
    const innerH = height - padding.t - padding.b;

    const sx = x => padding.l + (x - xMin) / (xMax - xMin) * innerW;
    const sy = y => padding.t + innerH - (y - yMin) / (yMax - yMin) * innerH;

    const yTicks = 5;
    const xTicks = 6;

    let svg = `<svg width="${width}" height="${height}" font-family="system-ui,sans-serif" font-size="11">`;
    svg += `<text x="${width/2}" y="16" text-anchor="middle" font-size="13" font-weight="600">${escapeHtml(title)}</text>`;

    // grid + y axis labels
    for (let i = 0; i <= yTicks; i++) {
        const v = yMin + (yMax - yMin) * (i / yTicks);
        const y = sy(v);
        svg += `<line x1="${padding.l}" y1="${y}" x2="${width - padding.r}" y2="${y}" stroke="#eee"/>`;
        svg += `<text x="${padding.l - 6}" y="${y + 3}" text-anchor="end" fill="#666">${fmtNum(Math.round(v))}</text>`;
    }
    // x axis labels
    for (let i = 0; i <= xTicks; i++) {
        const v = xMin + (xMax - xMin) * (i / xTicks);
        const x = sx(v);
        svg += `<text x="${x}" y="${height - padding.b + 14}" text-anchor="middle" fill="#666">${Math.round(v)}s</text>`;
    }

    // axes
    svg += `<line x1="${padding.l}" y1="${padding.t}" x2="${padding.l}" y2="${height - padding.b}" stroke="#888"/>`;
    svg += `<line x1="${padding.l}" y1="${height - padding.b}" x2="${width - padding.r}" y2="${height - padding.b}" stroke="#888"/>`;

    // axis labels
    svg += `<text x="${width/2}" y="${height - 4}" text-anchor="middle" fill="#444">${escapeHtml(xLabel)}</text>`;
    svg += `<text x="14" y="${height/2}" text-anchor="middle" fill="#444" transform="rotate(-90 14 ${height/2})">${escapeHtml(yLabel)}</text>`;

    // series
    for (const s of series) {
        if (s.points.length === 0) continue;
        const d = s.points.map((p, i) => `${i === 0 ? "M" : "L"}${sx(p[0]).toFixed(1)},${sy(p[1]).toFixed(1)}`).join(" ");
        const dash = s.dash ? `stroke-dasharray="${s.dash}"` : "";
        svg += `<path d="${d}" fill="none" stroke="${s.color}" stroke-width="2" ${dash}/>`;
        for (const p of s.points) {
            svg += `<circle cx="${sx(p[0]).toFixed(1)}" cy="${sy(p[1]).toFixed(1)}" r="2.5" fill="${s.color}"/>`;
        }
    }

    // legend
    let lx = padding.l + 10;
    const ly = padding.t + 14;
    for (const s of series) {
        svg += `<rect x="${lx}" y="${ly - 8}" width="10" height="10" fill="${s.color}"/>`;
        svg += `<text x="${lx + 14}" y="${ly}" fill="#333">${escapeHtml(s.name)}</text>`;
        lx += 14 + (s.name.length * 6.5) + 14;
    }

    svg += `</svg>`;
    return svg;
}

// ---------- single-run report ----------
function buildSingleReport({ expDir, run, samples }) {
    const label = run.label || basename(expDir);

    // Series: cumulative input, output, total
    const cumPoints = {
        input: [],
        output: [],
        total: [],
        cache_read: [],
        cache_creation: [],
    };
    const deltaPoints = { input: [], output: [], total: [] };

    let prev = null;
    for (const s of samples) {
        const t = s.elapsed_s;
        const tot = s.total?.tokens || {};
        cumPoints.input.push([t, tot.input ?? 0]);
        cumPoints.output.push([t, tot.output ?? 0]);
        cumPoints.total.push([t, tot.total ?? 0]);
        cumPoints.cache_read.push([t, tot.cache_read ?? 0]);
        cumPoints.cache_creation.push([t, tot.cache_creation ?? 0]);

        if (prev) {
            const dt = prev.total?.tokens || {};
            deltaPoints.input.push([t, Math.max(0, (tot.input ?? 0) - (dt.input ?? 0))]);
            deltaPoints.output.push([t, Math.max(0, (tot.output ?? 0) - (dt.output ?? 0))]);
            deltaPoints.total.push([t, Math.max(0, (tot.total ?? 0) - (dt.total ?? 0))]);
        }
        prev = s;
    }

    const last = samples[samples.length - 1];
    const lastTotals = last?.total?.tokens || {};
    const lastUsd = last?.total?.api_equivalent_value_usd;

    let durationSec = null;
    if (run.started_at && run.ended_at) {
        durationSec = Math.round((new Date(run.ended_at) - new Date(run.started_at)) / 1000);
    }

    const cumChart = svgLineChart({
        width: 900, height: 360, padding: { l: 70, r: 20, t: 30, b: 40 },
        title: "Cumulative tokens (from cc-usage)",
        xLabel: "elapsed seconds", yLabel: "tokens",
        series: [
            { name: "input",          color: "#1f77b4", points: cumPoints.input },
            { name: "output",         color: "#d62728", points: cumPoints.output },
            { name: "cache_read",     color: "#2ca02c", points: cumPoints.cache_read },
            { name: "cache_creation", color: "#ff7f0e", points: cumPoints.cache_creation },
            { name: "total",          color: "#000000", points: cumPoints.total, dash: "4 3" },
        ],
    });

    const deltaChart = svgLineChart({
        width: 900, height: 320, padding: { l: 70, r: 20, t: 30, b: 40 },
        title: "Tokens added per sample (delta)",
        xLabel: "elapsed seconds", yLabel: "tokens added",
        series: [
            { name: "input",  color: "#1f77b4", points: deltaPoints.input },
            { name: "output", color: "#d62728", points: deltaPoints.output },
            { name: "total",  color: "#000000", points: deltaPoints.total, dash: "4 3" },
        ],
    });

    const tableRows = samples.map(s => {
        const tot = s.total?.tokens || {};
        return `<tr>
            <td>${escapeHtml(s.t || "")}</td>
            <td>${s.elapsed_s}</td>
            <td>${fmtNum(tot.input)}</td>
            <td>${fmtNum(tot.output)}</td>
            <td>${fmtNum(tot.cache_read)}</td>
            <td>${fmtNum(tot.cache_creation)}</td>
            <td>${fmtNum(tot.total)}</td>
            <td>${fmtUsd(s.total?.api_equivalent_value_usd)}</td>
        </tr>`;
    }).join("\n");

    return `<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<title>Tale of Two Claudes — ${escapeHtml(label)}</title>
<style>
  body { font-family: system-ui, sans-serif; max-width: 980px; margin: 24px auto; padding: 0 20px; color: #222; }
  h1 { margin-bottom: 4px; }
  .sub { color: #666; margin-top: 0; }
  .grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 12px 24px; margin: 16px 0 24px; }
  .grid div { font-size: 14px; }
  .grid b { display: block; color: #555; font-weight: 500; font-size: 12px; text-transform: uppercase; }
  table { border-collapse: collapse; width: 100%; font-size: 12px; }
  th, td { border: 1px solid #ddd; padding: 4px 8px; text-align: right; }
  th { background: #f5f5f5; }
  th:first-child, td:first-child { text-align: left; }
  details { margin: 16px 0; }
  pre { background: #f5f5f5; padding: 12px; border-radius: 4px; overflow-x: auto; font-size: 12px; }
  footer { margin-top: 32px; color: #888; font-size: 12px; }
</style>
</head>
<body>
<h1>Tale of Two Claudes — ${escapeHtml(label)}</h1>
<p class="sub">${escapeHtml(run.flavor || "")} / ${escapeHtml(run.version || "")} · ${escapeHtml(expDir)}</p>

<div class="grid">
  <div><b>Started</b>${escapeHtml(run.started_at || "—")}</div>
  <div><b>Ended</b>${escapeHtml(run.ended_at || "—")}</div>
  <div><b>Duration</b>${durationSec != null ? durationSec + "s" : "—"}</div>
  <div><b>Exit code</b>${run.exit_code ?? "—"}${run.interrupted ? " (interrupted)" : ""}</div>
  <div><b>Samples</b>${samples.length}</div>
  <div><b>Base id</b>${escapeHtml(run.base_id || "—")}</div>
  <div><b>Total tokens</b>${fmtNum(lastTotals.total)}</div>
  <div><b>Input tokens</b>${fmtNum(lastTotals.input)}</div>
  <div><b>Output tokens</b>${fmtNum(lastTotals.output)}</div>
  <div><b>API-equivalent USD</b>${fmtUsd(lastUsd)}</div>
</div>

${cumChart}
${deltaChart}

<details>
  <summary>All samples (${samples.length})</summary>
  <table>
    <thead><tr>
      <th>timestamp</th><th>elapsed</th><th>input</th><th>output</th>
      <th>cache_read</th><th>cache_creation</th><th>total</th><th>usd</th>
    </tr></thead>
    <tbody>${tableRows}</tbody>
  </table>
</details>

<details>
  <summary>run.json</summary>
  <pre>${escapeHtml(JSON.stringify(run, null, 2))}</pre>
</details>

<footer>
  cc-usage reads from Claude Code's session logs on disk and may lag the live session by a few seconds.
</footer>
</body>
</html>`;
}

// ---------- comparison report ----------
function buildCompareReport(runs) {
    // colors: naked / effortless. dash by version.
    const colorFor = flavor => flavor === "naked" ? "#d62728" : "#1f77b4";
    const dashFor = version => version === "v1" ? null : version === "v2" ? "6 4" : version === "v3" ? "2 3" : "8 2 2 2";

    const series = runs.map(({ run, samples }) => {
        const points = samples.map(s => [s.elapsed_s, s.total?.tokens?.total ?? 0]);
        return {
            name: run.label || "unknown",
            color: colorFor(run.flavor),
            dash: dashFor(run.version),
            points,
        };
    });

    const chart = svgLineChart({
        width: 1000, height: 480, padding: { l: 80, r: 20, t: 30, b: 40 },
        title: "Cumulative tokens — comparison",
        xLabel: "elapsed seconds", yLabel: "tokens",
        series,
    });

    const rows = runs.map(({ run, samples }) => {
        const last = samples[samples.length - 1];
        const tot = last?.total?.tokens || {};
        return `<tr>
            <td>${escapeHtml(run.label || "—")}</td>
            <td>${escapeHtml(run.flavor || "")}</td>
            <td>${escapeHtml(run.version || "")}</td>
            <td>${samples.length}</td>
            <td>${fmtNum(tot.input)}</td>
            <td>${fmtNum(tot.output)}</td>
            <td>${fmtNum(tot.total)}</td>
            <td>${fmtUsd(last?.total?.api_equivalent_value_usd)}</td>
        </tr>`;
    }).join("\n");

    return `<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<title>Tale of Two Claudes — comparison</title>
<style>
  body { font-family: system-ui, sans-serif; max-width: 1080px; margin: 24px auto; padding: 0 20px; color: #222; }
  table { border-collapse: collapse; width: 100%; font-size: 13px; margin-top: 16px; }
  th, td { border: 1px solid #ddd; padding: 6px 10px; text-align: right; }
  th { background: #f5f5f5; }
  th:first-child, td:first-child, th:nth-child(2), td:nth-child(2), th:nth-child(3), td:nth-child(3) { text-align: left; }
</style>
</head>
<body>
<h1>Tale of Two Claudes — comparison</h1>
${chart}
<table>
  <thead><tr>
    <th>label</th><th>flavor</th><th>version</th><th>samples</th>
    <th>input</th><th>output</th><th>total</th><th>usd</th>
  </tr></thead>
  <tbody>${rows}</tbody>
</table>
</body>
</html>`;
}

// ---------- main ----------
if (compareIdx >= 0) {
    const dirs = argv.slice(compareIdx + 1).filter(a => !a.startsWith("--"));
    if (dirs.length === 0) {
        console.error("--compare requires at least one dir");
        process.exit(2);
    }
    const runs = dirs.map(d => loadRun(resolve(d)));
    const html = buildCompareReport(runs);
    const ts = new Date().toISOString().replace(/[:.]/g, "-");
    const out = resolve(`tale-comparison-${ts}.html`);
    writeFileSync(out, html);
    console.log(out);
} else {
    const expDir = resolve(dir);
    if (!existsSync(expDir)) {
        console.error(`Experiment dir not found: ${expDir}`);
        process.exit(1);
    }
    const data = loadRun(expDir);
    const html = buildSingleReport(data);
    const out = join(expDir, ".tale", "report.html");
    writeFileSync(out, html);
    console.log(out);
}
