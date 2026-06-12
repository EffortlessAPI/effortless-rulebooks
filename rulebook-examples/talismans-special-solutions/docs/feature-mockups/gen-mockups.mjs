// Generates one SVG mockup per FEATURE-TODO item, in the Release Console's real
// dark palette (from app/frontend/src/styles.css). Each mockup shows the
// proposed TOGGLE control + the before/after board state, so the to-do reads as
// a concrete visual plan. Pure string templating — no deps.
//
//   node gen-mockups.mjs
//
// then convert with rsvg-convert (see convert-all.sh).

import { writeFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { dirname, join } from "node:path";

const OUT = dirname(fileURLToPath(import.meta.url));

// ---- palette (mirrors styles.css :root) -----------------------------------
const C = {
  bg: "#0e1016", bg2: "#141823", panel: "#181c27", card: "#1e2330", card2: "#252b3a",
  line: "#2b3142", text: "#e8eaf0", muted: "#8a91a4",
  human: "#5aa9e6", ai: "#a78bfa", pipeline: "#ffc14d", inferred: "#ffc14d",
  accent: "#b692ff", ok: "#43c463", risk: "#f0653f",
};
const W = 1000, H = 620;
const FONT = `-apple-system, "Segoe UI", Roboto, sans-serif`;

// ---- tiny svg helpers -----------------------------------------------------
const esc = (s) => String(s).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
function rect(x, y, w, h, { fill = C.card, stroke = C.line, sw = 1, rx = 10, dash = null, opacity = 1 } = {}) {
  return `<rect x="${x}" y="${y}" width="${w}" height="${h}" rx="${rx}" fill="${fill}" stroke="${stroke}" stroke-width="${sw}"${dash ? ` stroke-dasharray="${dash}"` : ""} opacity="${opacity}"/>`;
}
function text(x, y, s, { fill = C.text, size = 14, weight = 400, anchor = "start", mono = false, op = 1 } = {}) {
  const f = mono ? `ui-monospace, "SF Mono", Menlo, monospace` : FONT;
  return `<text x="${x}" y="${y}" fill="${fill}" font-size="${size}" font-weight="${weight}" font-family='${f}' text-anchor="${anchor}" opacity="${op}">${esc(s)}</text>`;
}
function pill(x, y, s, { fill = C.card2, stroke = C.line, tc = C.text, size = 12, pad = 11 } = {}) {
  const w = pad * 2 + s.length * size * 0.56;
  return rect(x, y, w, 24, { fill, stroke, rx: 12 }) + text(x + pad, y + 16, s, { fill: tc, size, weight: 600 });
}
// a labeled toggle switch (the recurring "toggleable thing")
function toggle(x, y, label, on, { onColor = C.accent } = {}) {
  const tw = 42, th = 22, kx = on ? x + tw - 19 : x + 3;
  return [
    rect(x, y, tw, th, { rx: 11, fill: on ? onColor : C.card2, stroke: on ? onColor : C.line }),
    `<circle cx="${kx + 8}" cy="${y + 11}" r="8" fill="#fff"/>`,
    text(x + tw + 10, y + 16, label, { size: 13, weight: 600, fill: on ? C.text : C.muted }),
  ].join("");
}
function slider(x, y, w, frac, label) {
  const kx = x + w * frac;
  return [
    text(x, y - 10, label, { size: 12, weight: 600, fill: C.muted }),
    `<line x1="${x}" y1="${y}" x2="${x + w}" y2="${y}" stroke="${C.line}" stroke-width="4" stroke-linecap="round"/>`,
    `<line x1="${x}" y1="${y}" x2="${kx}" y2="${y}" stroke="${C.accent}" stroke-width="4" stroke-linecap="round"/>`,
    `<circle cx="${kx}" cy="${y}" r="9" fill="#fff" stroke="${C.accent}" stroke-width="2"/>`,
  ].join("");
}
const ICON = { human: "🧑", ai: "🤖", pipeline: "⚙️", gate: "🔒", warn: "⚠", check: "✓", cross: "✗" };

// node for graph-ish mockups
function node(cx, cy, label, { fill = C.card, stroke = C.line, tc = C.text, r = 30, sub = null } = {}) {
  return [
    `<circle cx="${cx}" cy="${cy}" r="${r}" fill="${fill}" stroke="${stroke}" stroke-width="2"/>`,
    text(cx, cy + (sub ? -1 : 5), label, { anchor: "middle", size: 13, weight: 700, fill: tc }),
    sub ? text(cx, cy + 13, sub, { anchor: "middle", size: 10, fill: C.muted }) : "",
  ].join("");
}
function edge(x1, y1, x2, y2, { color = C.line, dash = null, w = 2, arrow = true } = {}) {
  return `<line x1="${x1}" y1="${y1}" x2="${x2}" y2="${y2}" stroke="${color}" stroke-width="${w}"${dash ? ` stroke-dasharray="${dash}"` : ""} marker-end="${arrow ? "url(#arr)" : ""}"/>`;
}

// ---- console chrome shared by every mockup --------------------------------
function chrome(title, { tabs = ["Flow", "Graph", "Closure"], activeTab = null, extraTab = null, veil = false } = {}) {
  let s = "";
  // background
  s += rect(0, 0, W, H, { fill: C.bg, stroke: "none", rx: 0 });
  // topbar
  s += rect(0, 0, W, 52, { fill: C.bg2, stroke: C.line, rx: 0 });
  s += text(24, 33, "◈ Talisman's Special Solutions", { size: 16, weight: 700, fill: C.text });
  s += text(282, 33, "Release Console", { size: 12, weight: 600, fill: C.muted });
  // engine chip (top right) — the multi-substrate conformance toggle
  s += pill(W - 250, 14, "reasoner ⇄ postgres", { fill: C.card, stroke: C.accent, tc: C.accent });
  s += pill(W - 70, 14, "12/12 ✓", { fill: C.card, stroke: C.ok, tc: C.ok });
  // tab bar
  const allTabs = extraTab ? [...tabs, extraTab] : tabs;
  let tx = 24;
  for (const t of allTabs) {
    const on = (activeTab || tabs[0]) === t;
    const w = 22 + t.length * 9;
    s += rect(tx, 64, w, 32, { rx: 8, fill: on ? C.card2 : "transparent", stroke: on ? C.accent : C.line });
    s += text(tx + w / 2, 85, t, { anchor: "middle", size: 13, weight: 700, fill: on ? C.text : C.muted });
    if (t === extraTab) s += text(tx + w / 2, 112, "← NEW", { anchor: "middle", size: 9, weight: 800, fill: C.ok });
    tx += w + 8;
  }
  // title strip for the mockup
  s += text(24, 134, title, { size: 13, weight: 700, fill: C.accent });
  // footer
  s += text(24, H - 16, "db.json holds raw facts · OWL + SHACL is the computation · this console only edits facts & renders what the reasoner returns", { size: 10.5, fill: C.muted });
  if (veil) {
    s += rect(0, 52, W, H - 52, { fill: "#08090f", stroke: "none", rx: 0, opacity: 0.5 });
    s += pill(W / 2 - 70, H / 2 - 12, "✦ reasoning…", { fill: C.bg2, stroke: C.accent, tc: C.accent, size: 13 });
  }
  return s;
}

function svg(inner) {
  return `<svg xmlns="http://www.w3.org/2000/svg" width="${W}" height="${H}" viewBox="0 0 ${W} ${H}" font-family='${FONT}'>
<defs><marker id="arr" markerWidth="9" markerHeight="9" refX="7" refY="4.5" orient="auto"><path d="M0,0 L9,4.5 L0,9 Z" fill="${C.muted}"/></marker>
<marker id="arrInf" markerWidth="9" markerHeight="9" refX="7" refY="4.5" orient="auto"><path d="M0,0 L9,4.5 L0,9 Z" fill="${C.inferred}"/></marker></defs>
${inner}
</svg>`;
}

// caption box (the "what flips" label)
function caption(x, y, w, lines) {
  let s = rect(x, y, w, 24 + lines.length * 20, { fill: "#0c0e15", stroke: C.line, rx: 8 });
  lines.forEach((ln, i) => {
    const bold = ln.startsWith("*");
    s += text(x + 14, y + 22 + i * 20, bold ? ln.slice(1) : ln, { size: 12.5, fill: bold ? C.text : C.muted, weight: bold ? 700 : 400 });
  });
  return s;
}

// ===========================================================================
// the 14 mockups
// ===========================================================================
const M = {};

// 1. CQ scoreboard — new "CQs" lens tab
M["01-cq-scoreboard"] = () => {
  let s = chrome("Feature 1 · Competency-Question scoreboard — new lens tab /console/cq", { extraTab: "CQs", activeTab: "CQs" });
  s += toggle(720, 150, "show only failing", false);
  const cqs = [
    ["CQ1", "Who is assigned to each step?", "7 roles resolved", true],
    ["CQ2", "Which human approves the release gate?", "Maria Gonzalez", true],
    ["CQ3", "What order do steps run in?", "10-pair closure", true],
    ["CQ4", "Where did each artifact come from?", "5-artifact chain", true],
    ["CQ5", "Is this workflow stale (>12mo)?", "false (4mo)", true],
    ["CQ6", "Who does the gate role escalate to?", "VP Eng → CTO", true],
    ["CQ7", "Which steps cross Eng & Legal?", "step-2, step-4", true],
    ["CQ8", "What datasets does review consume?", "incident-log v3", true],
  ];
  let y = 182;
  for (const [id, q, a, ok] of cqs) {
    s += rect(24, y, 952, 42, { fill: C.card });
    s += pill(36, y + 9, id, { fill: C.card2, stroke: C.line, tc: C.accent });
    s += text(110, y + 27, q, { size: 13, weight: 600 });
    s += text(560, y + 27, a, { size: 12.5, fill: C.muted, mono: true });
    s += rect(930, y + 11, 22, 22, { rx: 6, fill: ok ? "#16341f" : "#2a1714", stroke: ok ? C.ok : C.risk });
    s += text(941, y + 27, ok ? ICON.check : ICON.cross, { anchor: "middle", size: 13, weight: 800, fill: ok ? C.ok : C.risk });
    y += 48;
  }
  return s;
};

// 2. Provenance lens
M["02-provenance-lens"] = () => {
  let s = chrome("Feature 2 · Provenance / artifact-lineage — new lens tab /console/provenance", { extraTab: "Provenance", activeTab: "Provenance" });
  s += toggle(740, 150, "asserted ↔ +derived", true);
  // wasDerivedFrom chain (5 artifacts left→right), with wasAttributedTo agent below each
  const xs = [110, 290, 470, 650, 830], cy = 300;
  const arts = ["design-doc", "code-pr", "build", "test-report", "release-pkg"];
  const ag = ["human", "ai", "pipeline", "ai", "pipeline"];
  for (let i = 0; i < 5; i++) {
    if (i < 4) s += edge(xs[i] + 34, cy, xs[i + 1] - 34, cy, { color: i >= 2 ? C.inferred : C.muted, dash: i >= 2 ? "5 4" : null });
    s += node(xs[i], cy, "A" + (i + 1), { stroke: C.accent, sub: arts[i] });
    // attributed-to agent below
    const col = C[ag[i]];
    s += `<circle cx="${xs[i]}" cy="${cy + 86}" r="16" fill="${C.card2}" stroke="${col}"/>`;
    s += text(xs[i], cy + 91, ICON[ag[i]], { anchor: "middle", size: 14 });
    s += edge(xs[i], cy + 30, xs[i], cy + 68, { color: col, w: 1.5, dash: "3 3" });
  }
  s += text(110, cy - 70, "wasDerivedFrom →", { size: 12, weight: 700, fill: C.muted });
  s += text(110, cy + 130, "↑ wasAttributedTo (agent)", { size: 12, weight: 700, fill: C.muted });
  s += pill(110, cy + 150, "● asserted", { fill: C.card, stroke: C.muted, tc: C.muted });
  s += pill(230, cy + 150, "○ derived (PROV)", { fill: C.card, stroke: C.inferred, tc: C.inferred });
  s += caption(24, 510, 952, ["*Answers CQ4. Read WorkflowArtifacts.DerivedFromArtifact + AttributedTo* — already computed & browsable in /dag; just draw the edges."]);
  return s;
};

// 3. Delegation / Org lens (route dead OrgView.tsx)
M["03-delegation-view"] = () => {
  let s = chrome("Feature 3 · Delegation / escalation — route the dead OrgView.tsx as /console/org", { extraTab: "Delegation", activeTab: "Delegation" });
  s += toggle(760, 150, "asserted ↔ closure", true);
  // Release Mgr -> VP Eng -> CTO with inferred RM->CTO
  const n = [[200, 240, "Release Mgr", "ntwf-release-manager"], [500, 360, "VP Eng", "ntwf-vp-eng"], [800, 240, "CTO", "ntwf-cto"]];
  s += edge(244, 262, 456, 338, { color: C.text, w: 2 });
  s += edge(544, 338, 756, 262, { color: C.text, w: 2 });
  // inferred RM -> CTO (the never-asserted hop)
  s += `<path d="M 240 215 Q 500 120 760 215" fill="none" stroke="${C.inferred}" stroke-width="2.5" stroke-dasharray="6 5" marker-end="url(#arrInf)"/>`;
  s += text(500, 130, "inferred: Release Mgr → CTO  (hop 2)", { anchor: "middle", size: 12, weight: 700, fill: C.inferred });
  for (const [x, y, lab, sub] of n) s += node(x, y, lab.split(" ").map(w => w[0]).join(""), { r: 38, stroke: C.human, sub });
  for (const [x, y, lab] of n) s += text(x, y + 60, lab, { anchor: "middle", size: 13, weight: 700 });
  s += pill(120, 470, "● asserted edge (delegatesTo)", { fill: C.card, stroke: C.muted, tc: C.muted });
  s += pill(420, 470, "○ inferred (delegatesTo closure)", { fill: C.card, stroke: C.inferred, tc: C.inferred });
  s += caption(24, 520, 952, ["*Nearly free — chain already ships as story.delegation.* Answers CQ6; pairs with Feature 5's escalation threshold."]);
  return s;
};

// 4. Blast radius panel
M["04-blast-radius"] = () => {
  let s = chrome("Feature 4 · 'Blast radius' / AI-system-registry — overlay on the Graph lens", { activeTab: "Graph" });
  s += toggle(700, 150, "highlight blast radius", true);
  // pick an AI agent
  s += rect(24, 180, 300, 250, { fill: C.card });
  s += text(40, 206, "Pick an AI agent", { size: 13, weight: 700, fill: C.muted });
  s += rect(40, 220, 268, 56, { fill: C.card2, stroke: C.ai });
  s += text(56, 244, "🤖 risk-classifier", { size: 14, weight: 700 });
  s += text(56, 263, "ModelVersion: v2.4.1 · DeployedOn: 2026-02-10", { size: 11, mono: true, fill: C.muted });
  s += rect(40, 290, 130, 64, { fill: "#19102e", stroke: C.ai });
  s += text(105, 318, "4", { anchor: "middle", size: 26, weight: 800, fill: C.ai });
  s += text(105, 340, "artifacts", { anchor: "middle", size: 11, fill: C.muted });
  s += rect(178, 290, 130, 64, { fill: "#19102e", stroke: C.ai });
  s += text(243, 318, "2", { anchor: "middle", size: 26, weight: 800, fill: C.ai });
  s += text(243, 340, "workflows", { anchor: "middle", size: 11, fill: C.muted });
  s += text(40, 386, "CountAttributedArtifacts /", { size: 11, fill: C.muted });
  s += text(40, 402, "CountImpactedWorkflows (computed)", { size: 11, fill: C.muted });
  // downstream graph highlighted
  const gx = 420;
  s += text(gx, 200, "downstream (highlighted)", { size: 12, weight: 700, fill: C.muted });
  const dn = [[gx + 40, 250], [gx + 220, 230], [gx + 220, 320], [gx + 400, 270], [gx + 400, 360]];
  s += edge(gx + 70, 250, gx + 192, 232, { color: C.ai, w: 2 });
  s += edge(gx + 70, 256, gx + 192, 314, { color: C.ai, w: 2 });
  s += edge(gx + 248, 232, gx + 372, 268, { color: C.ai, w: 2 });
  s += edge(gx + 248, 320, gx + 372, 356, { color: C.ai, w: 2 });
  const dl = ["agent", "artifact", "step", "wf", "wf"];
  dn.forEach(([x, y], i) => { s += node(x, y, dl[i], { r: 26, stroke: i === 0 ? C.ai : C.line, fill: i === 0 ? "#19102e" : C.card, tc: i === 0 ? C.ai : C.text }); });
  s += caption(24, 470, 952, ["*Covers IV-8/IV-9 + modelVersion — the flagship Part-IV demo with no UI today.*", "Highlight is a display filter over existing edges; the four counts are already computed on AIAgents."]);
  return s;
};

// 5. Approval-gate detail
M["05-gate-detail"] = () => {
  let s = chrome("Feature 5 · Approval-gate detail — expandable popover on the 🔒 gate badge", { activeTab: "Flow" });
  // flow strip with the gate badge
  const sx = 60, sy = 200;
  for (let i = 0; i < 5; i++) {
    const x = sx + i * 180;
    s += rect(x, sy, 150, 70, { fill: C.card, stroke: i === 2 ? C.pipeline : C.line });
    s += text(x + 14, sy + 26, "step " + (i + 1), { size: 13, weight: 700 });
    s += text(x + 14, sy + 48, ["plan", "build", "approve", "deploy", "verify"][i], { size: 11, fill: C.muted });
    if (i === 2) { s += rect(x + 110, sy + 8, 30, 20, { rx: 6, fill: "#2a2410", stroke: C.pipeline }); s += text(x + 125, sy + 23, ICON.gate, { anchor: "middle", size: 12 }); }
    if (i < 4) s += edge(x + 150, sy + 35, x + 180, sy + 35, { color: C.muted });
  }
  // popover expanded from the gate
  const px = 360, py = 300;
  s += `<path d="M ${px + 60} ${py} L ${px + 75} ${py - 16} L ${px + 90} ${py} Z" fill="${C.card2}" stroke="${C.pipeline}"/>`;
  s += rect(px, py, 320, 180, { fill: C.card2, stroke: C.pipeline });
  s += text(px + 18, py + 30, "🔒 Release Approval Gate", { size: 14, weight: 800 });
  const rows = [["EscalationThresholdHours", "4 h", true], ["GateRole", "Release Manager", false], ["GateApproverHuman", "Maria Gonzalez", false]];
  let ry = py + 58;
  for (const [k, v, edit] of rows) {
    s += text(px + 18, ry, k, { size: 12, fill: C.muted });
    if (edit) { s += rect(px + 220, ry - 16, 80, 24, { rx: 6, fill: C.card, stroke: C.accent, dash: "3 3" }); s += text(px + 260, ry, v, { anchor: "middle", size: 12, weight: 700, fill: C.accent }); }
    else s += text(px + 300, ry, v, { anchor: "end", size: 12.5, weight: 700, mono: true });
    ry += 40;
  }
  s += text(px + 18, ry + 4, "↕ drag to edit → re-fires the substrate", { size: 11, fill: C.accent });
  s += caption(24, 510, 952, ["*Answers CQ2 fully — values already resolved (see testing/owl/test-answers/approval_gates.json).*"]);
  return s;
};

// 6. Bitemporal as-of slider
M["06-asof-slider"] = () => {
  let s = chrome("Feature 6 · Bitemporal 'as-of date' time-travel — draggable date slider overlay", { activeTab: "Flow" });
  s += rect(60, 170, 880, 70, { fill: C.bg2, stroke: C.line });
  s += slider(110, 205, 700, 0.38, "As of date  →  2026-03-01");
  s += text(110, 230, "drag back in time → each role shows who filled it then (RoleAssignments ValidFrom/ValidTo)", { size: 11.5, fill: C.muted });
  // role rows showing different fillers as-of
  const rows = [
    ["Release Manager", "🧑 Maria Gonzalez", false],
    ["CI Operator", "🤖 deploy-bot v1.2  →  🧑 Sam Patel", true],
    ["QA Lead", "🧑 Aisha Khan", false],
  ];
  let y = 290;
  for (const [role, filler, audit] of rows) {
    s += rect(60, y, 880, 52, { fill: C.card, stroke: audit ? C.risk : C.line });
    s += text(78, y + 31, role, { size: 13, weight: 700 });
    s += text(330, y + 31, filler, { size: 13, fill: audit ? C.text : C.muted });
    if (audit) { s += rect(770, y + 14, 150, 24, { rx: 6, fill: "#2a1714", stroke: C.risk }); s += text(845, y + 30, "⚠ audit: AI→Human", { anchor: "middle", size: 11, weight: 700, fill: C.risk }); }
    y += 60;
  }
  s += caption(24, 500, 952, ["*Covers IV-6/IV-7 — the entire governance-history story.*", "Pass the slider date to the API; read back the substrate's WasActiveAsOfAuditDate — don't do the interval test in React."]);
  return s;
};

// 7. Consistency lab (inject error)
M["07-consistency-lab"] = () => {
  let s = chrome("Feature 7 · Inject-an-error / consistency lab (Suite 4) — 'lab mode' switch", { activeTab: "Graph" });
  s += toggle(700, 150, "lab mode", true, { onColor: C.risk });
  s += text(40, 200, "Inject a violation — watch the reasoner flag it:", { size: 13, weight: 700, fill: C.muted });
  const btns = [
    "Assign AI to human-only gate", "Create disjoint-type individual",
    "Double-assign a functional role", "Apply modelVersion to a human",
  ];
  let by = 220;
  btns.forEach((b, i) => {
    const x = 40 + (i % 2) * 470, yy = by + Math.floor(i / 2) * 64;
    s += rect(x, yy, 440, 50, { fill: C.card, stroke: C.risk, dash: "4 3" });
    s += text(x + 18, yy + 30, "⚠ " + b, { size: 13, weight: 600 });
  });
  // result banner
  s += rect(40, 360, 920, 80, { fill: "#2a1714", stroke: C.risk });
  s += text(60, 392, "✗ Reasoner: inconsistency detected", { size: 15, weight: 800, fill: C.risk });
  s += text(60, 416, "HasConsistencyViolation = true · CountRolesWithBadFillerCardinality = 1 · owl:disjointWith breached", { size: 12, mono: true, fill: C.text });
  s += pill(40, 460, "↺ reset to clean ABox", { fill: C.card, stroke: C.ok, tc: C.ok });
  s += caption(24, 500, 952, ["*Covers II-4 + III-Suite-4 — the article's most instructive moment.*", "Each injection is a raw-fact write via api.addRow/patchRow; reuses the existing scenario machinery."]);
  return s;
};

// 8. Dataset (DCAT) overlay
M["08-dataset-overlay"] = () => {
  let s = chrome("Feature 8 · Dataset (DCAT) surface — 'show datasets' switch on the Flow lens", { activeTab: "Flow" });
  s += toggle(720, 150, "show datasets", true);
  const sx = 60, sy = 230;
  const consumes = [false, false, false, true, true];
  for (let i = 0; i < 5; i++) {
    const x = sx + i * 180;
    s += rect(x, sy, 150, 70, { fill: C.card, stroke: C.line });
    s += text(x + 14, sy + 26, "step " + (i + 1), { size: 13, weight: 700 });
    s += text(x + 14, sy + 48, ["plan", "build", "approve", "review", "verify"][i], { size: 11, fill: C.muted });
    if (i < 4) s += edge(x + 150, sy + 35, x + 180, sy + 35, { color: C.muted });
    if (consumes[i]) {
      s += rect(x, sy + 86, 150, 50, { fill: "#0c2414", stroke: C.ok, dash: "4 3" });
      s += text(x + 75, sy + 106, "📊 incident-log", { anchor: "middle", size: 11.5, weight: 700, fill: C.ok });
      s += text(x + 75, sy + 124, "dcat:Dataset v3", { anchor: "middle", size: 10, mono: true, fill: C.muted });
      s += edge(x + 75, sy + 70, x + 75, sy + 86, { color: C.ok, w: 1.5, dash: "3 3" });
    }
  }
  s += caption(24, 470, 952, ["*Answers CQ8. Steps.ConsumesDataset + Datasets already in options.datasets — never rendered. Just render behind the switch.*"]);
  return s;
};

// 9. SKOS vocabulary surfaces
M["09-skos-vocab"] = () => {
  let s = chrome("Feature 9 · SKOS vocabularies — status pill/picker, capability tags, definition hovercards", { activeTab: "Flow" });
  // status picker in verdict header
  s += rect(620, 160, 360, 110, { fill: C.card });
  s += text(640, 186, "Workflow status (SKOS-grounded)", { size: 12, weight: 700, fill: C.muted });
  ["draft", "in-review", "approved", "deprecated"].forEach((st, i) => {
    const x = 640 + (i % 2) * 170, y = 200 + Math.floor(i / 2) * 32;
    const on = st === "approved";
    s += rect(x, y, 158, 26, { rx: 13, fill: on ? "#16341f" : C.card2, stroke: on ? C.ok : C.line });
    s += text(x + 79, y + 17, st, { anchor: "middle", size: 12, weight: 700, fill: on ? C.ok : C.muted });
  });
  s += text(640, 264, "writes Workflows.WorkflowStatus → recomputes", { size: 10.5, fill: C.accent });
  // capability tags on roles
  s += text(40, 186, "Capabilities on roles (hasCapability →)", { size: 12, weight: 700, fill: C.muted });
  s += toggle(40, 196, "show capabilities", true);
  const roles = [["Release Manager", ["approve", "deploy"]], ["QA Lead", ["test", "sign-off"]], ["CI Pipeline", ["build", "package"]]];
  let y = 250;
  for (const [r, caps] of roles) {
    s += rect(40, y, 540, 48, { fill: C.card });
    s += text(58, y + 30, r, { size: 13, weight: 700 });
    let cx = 280;
    for (const c of caps) { const w = pill(cx, y + 12, c, { fill: "#19102e", stroke: C.ai, tc: C.ai }); s += w; cx += 22 + c.length * 8; }
    y += 56;
  }
  // hovercard
  s += rect(280, 430, 360, 90, { fill: C.card2, stroke: C.accent });
  s += text(298, 456, "skos:definition — 'approve'", { size: 12, weight: 800, fill: C.accent });
  s += text(298, 478, "Authority to sign off a release for production.", { size: 11.5, fill: C.text });
  s += text(298, 498, "scopeNote: distinct from 'deploy' (mechanical push).", { size: 11, fill: C.muted });
  s += caption(24, 530, 952, ["*Covers II-6/II-8 + hasCapability/workflowStatus. Concept schemes already exist; reuse DagHoverCard for definitions.*"], );
  return s;
};

// 10. Department / cross-cutting
M["10-department-lens"] = () => {
  let s = chrome("Feature 10 · Department / cross-cutting — 'department lens' switch (color by owner)", { activeTab: "Flow" });
  s += toggle(700, 150, "department lens", true);
  s += pill(40, 175, "🟦 Engineering", { fill: C.card, stroke: C.human, tc: C.human });
  s += pill(200, 175, "🟪 Legal", { fill: C.card, stroke: C.ai, tc: C.ai });
  s += pill(320, 175, "⚠ touches both", { fill: C.card, stroke: C.risk, tc: C.risk });
  const sx = 60, sy = 240, owners = [C.human, C.human, C.risk, C.ai, C.risk];
  for (let i = 0; i < 5; i++) {
    const x = sx + i * 180, col = owners[i], both = col === C.risk;
    s += rect(x, sy, 150, 80, { fill: both ? "#2a1714" : C.card, stroke: col, sw: both ? 2.5 : 1.5 });
    s += text(x + 14, sy + 28, "step " + (i + 1), { size: 13, weight: 700 });
    s += text(x + 14, sy + 50, both ? "Eng + Legal" : (col === C.human ? "Engineering" : "Legal"), { size: 11, fill: col });
    if (both) s += text(x + 125, sy + 24, "⚠", { anchor: "middle", size: 14, fill: C.risk });
    if (i < 4) s += edge(x + 150, sy + 40, x + 180, sy + 40, { color: C.muted });
  }
  s += caption(24, 470, 952, ["*Answers CQ7. Color from the already-computed IsLegalOwned / IsEngineeringOwned booleans (ownedBy chain) — don't recompute ownership in React.*"]);
  return s;
};

// 11. Governance panel
M["11-governance-panel"] = () => {
  let s = chrome("Feature 11 · Governance panel — slide-out from a topbar 'Governance' button", { activeTab: "Flow" });
  s += pill(W - 420, 14, "Governance ▸", { fill: C.card2, stroke: C.accent, tc: C.accent });
  // slide-out panel on the right
  s += rect(540, 110, 440, 470, { fill: C.panel, stroke: C.accent });
  s += text(562, 142, "Governance", { size: 16, weight: 800 });
  // steward vs authority
  s += text(562, 176, "Steward vs Authority", { size: 12, weight: 700, fill: C.muted });
  s += rect(562, 186, 196, 50, { fill: C.card, stroke: C.line }); s += text(578, 208, "Steward", { size: 12, weight: 700 }); s += text(578, 226, "CanApproveChanges: ✗", { size: 11, mono: true, fill: C.muted });
  s += rect(770, 186, 190, 50, { fill: C.card, stroke: C.ok }); s += text(786, 208, "Authority", { size: 12, weight: 700 }); s += text(786, 226, "CanApproveChanges: ✓", { size: 11, mono: true, fill: C.ok });
  // changelog timeline with breaking/back-compat pills
  s += text(562, 274, "ChangeLog version timeline", { size: 12, weight: 700, fill: C.muted });
  const ev = [["v1.0.0", "back-compat", C.ok], ["v2.0.0", "breaking", C.risk]];
  let y = 290;
  for (const [v, tag, col] of ev) {
    s += `<circle cx="574" cy="${y + 18}" r="6" fill="${col}"/>`;
    s += text(594, y + 23, v, { size: 13, weight: 700, mono: true });
    s += pill(700, y + 5, tag, { fill: C.card, stroke: col, tc: col });
    y += 48;
  }
  // vocab reconciliations
  s += text(562, 408, "Vocabulary reconciliations", { size: 12, weight: 700, fill: C.muted });
  ["foaf:name owl:sameAs ntwf:name", "dcat v2 → v3"].forEach((r, i) => {
    s += rect(562, 420 + i * 38, 398, 30, { fill: C.card, stroke: C.line });
    s += text(578, 440 + i * 38, r, { size: 11.5, mono: true, fill: C.text });
  });
  s += caption(24, 300, 480, ["*Covers IV-3/IV-4/IV-5.*", "All three tables computed &", "browsable in /dag — render their", "rows; the only 'toggle' is the", "breaking/back-compat pill color."]);
  return s;
};

// 12. Standards-provenance overlay
M["12-standards-overlay"] = () => {
  let s = chrome("Feature 12 · Standards-provenance overlay — global switch annotating borrowed vs custom terms", { activeTab: "Flow" });
  s += toggle(640, 150, "standards overlay", true);
  s += pill(40, 178, "borrowed (W3C standard)", { fill: C.card, stroke: C.ok, tc: C.ok });
  s += pill(280, 178, "custom ntwf:", { fill: C.card, stroke: C.accent, tc: C.accent });
  const fields = [
    ["modified", "dct:modified", C.ok], ["wasDerivedFrom", "prov:wasDerivedFrom", C.ok],
    ["name", "foaf:name", C.ok], ["consumesDataset", "dcat:Dataset", C.ok],
    ["status", "skos:Concept", C.ok], ["escalationThresholdHours", "ntwf: (custom)", C.accent],
    ["precedesStep", "ntwf: (custom)", C.accent], ["filledBy", "ntwf: (custom)", C.accent],
  ];
  let y = 230;
  fields.forEach((f, i) => {
    const x = 40 + (i % 2) * 480, yy = y + Math.floor(i / 2) * 56;
    s += rect(x, yy, 450, 44, { fill: C.card, stroke: f[2], dash: f[2] === C.accent ? "5 4" : null });
    s += text(x + 16, yy + 28, f[0], { size: 13, weight: 700 });
    s += pill(x + 250, yy + 10, f[1], { fill: "#0c0e15", stroke: f[2], tc: f[2] });
  });
  s += caption(24, 520, 952, ["*Covers I-2/II-7 — the 'mostly assembly' claim. Project the field→standard map FROM the OWL alignments (rdfs:isDefinedBy / equivalentClass), not hand-typed React.*"]);
  return s;
};

// 13. Type-inference expansion
M["13-type-closure"] = () => {
  let s = chrome("Feature 13 · Type-inference expansion — 'type closure' sub-toggle inside the Closure lens", { activeTab: "Closure" });
  s += toggle(700, 150, "type closure", true);
  // an individual -> asserted type -> entailed supertypes
  s += node(140, 320, "Maria", { r: 34, stroke: C.human, sub: "individual" });
  const chain = [[360, 320, "HumanAgent", "ntwf:", true], [580, 320, "foaf:Person", "foaf:", false], [800, 320, "prov:Agent", "prov:", false]];
  s += edge(174, 320, 322, 320, { color: C.human, w: 2 });
  for (let i = 0; i < chain.length; i++) {
    const [x, y, lab, , asserted] = chain[i];
    if (i < 2) s += edge(x + 42, y, chain[i + 1][0] - 42, y, { color: C.inferred, dash: "6 5", arrow: true });
    s += node(x, y, "", { r: 42, stroke: asserted ? C.text : C.inferred });
    s += text(x, y - 2, lab, { anchor: "middle", size: 12, weight: 700 });
    s += text(x, y + 16, asserted ? "● asserted" : "○ inferred", { anchor: "middle", size: 10, fill: asserted ? C.muted : C.inferred });
  }
  s += text(360, 250, "rdf:type", { anchor: "middle", size: 11, fill: C.muted });
  s += text(470, 290, "rdfs:subClassOf", { anchor: "middle", size: 11, fill: C.inferred });
  s += text(690, 290, "rdfs:subClassOf", { anchor: "middle", size: 11, fill: C.inferred });
  s += caption(24, 470, 952, ["*Covers I-3 + III-Suite-2 — generalizes the closure idea beyond precedence.*", "OWL-RL already entails these triples; expose the per-individual type chain and render with the existing asserted/inferred styling."]);
  return s;
};

// 14. ModelVersion / DeployedOn
M["14-model-version"] = () => {
  let s = chrome("Feature 14 · ModelVersion / DeployedOn on AI agents — 'show model versions' switch", { activeTab: "Flow" });
  s += toggle(700, 150, "show model versions", true);
  const ais = [["risk-classifier", "v2.4.1", "2026-02-10"], ["test-gen", "v1.8.0", "2025-11-22"], ["doc-summarizer", "v3.0.2", "2026-04-01"]];
  let y = 220;
  for (const [name, ver, dep] of ais) {
    s += rect(60, y, 880, 70, { fill: C.card, stroke: C.ai });
    s += `<circle cx="100" cy="${y + 35}" r="20" fill="#19102e" stroke="${C.ai}"/>`;
    s += text(100, y + 41, "🤖", { anchor: "middle", size: 16 });
    s += text(140, y + 32, name, { size: 15, weight: 700 });
    s += text(140, y + 54, "AI agent (prov:SoftwareAgent)", { size: 11, fill: C.muted });
    s += pill(560, y + 14, "ModelVersion " + ver, { fill: "#19102e", stroke: C.ai, tc: C.ai });
    s += pill(560, y + 42, "DeployedOn " + dep, { fill: C.card2, stroke: C.line, tc: C.muted });
    y += 86;
  }
  s += caption(24, 500, 952, ["*Covers III modelVersion + IV-9 — small but currently invisible. Folds into Feature 4's blast-radius panel. AIAgents.ModelVersion/DeployedOn exist; add to payload & render.*"]);
  return s;
};

// ---- emit -----------------------------------------------------------------
let n = 0;
for (const [key, fn] of Object.entries(M)) {
  const file = join(OUT, key + ".svg");
  writeFileSync(file, svg(fn()), "utf8");
  console.log("wrote", key + ".svg");
  n++;
}
console.log(`\n${n} svg files written to ${OUT}`);
