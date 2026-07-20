// ============================================================================
// Explorer — every field, everywhere.
//
// The curated screens show the ~80 columns that tell the procedural story. The
// model carries 1521 fields, 925 of them derived. This module is the answer to
// "where does everything else live".
//
// It is DERIVED, like everything else here. Nothing below enumerates a table,
// a column, or a formula: it renders whatever /api/explore returns, which is
// itself built from vw_rulebook_fields. Add a field to the rulebook, rebuild,
// and it shows up — with its formula, its inputs, and the question that
// motivated it — without editing this file.
//
// THE VIEW IS STILL THE CONTRACT. Every value rendered here is a column the
// substrate computed. The `formula` strings are shown as EVIDENCE, never
// evaluated. The `inputs` are other columns, fetched by the API, not derived
// in this file.
// ============================================================================

const esc = (s) =>
  String(s ?? "").replace(/[&<>"']/g, (c) =>
    ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));

const api = async (path) => {
  const r = await fetch(path);
  if (!r.ok) {
    const body = await r.json().catch(() => ({}));
    throw new Error(body.error || `${r.status} ${r.statusText}`);
  }
  return r.json();
};

export const EXPLORE_TABS = {
  inferences: "Every Inference",
  tables: "All Tables",
  record: "Record Inspector",
};

// Field-kind vocabulary, used for the colour key everywhere in this module.
const KIND = {
  raw: { label: "raw", cls: "k-raw", note: "stored — entered, not computed" },
  calculated: { label: "calculated", cls: "k-calc", note: "computed by formula from other fields" },
  lookup: { label: "lookup", cls: "k-look", note: "pulled across a relationship" },
  aggregation: { label: "aggregation", cls: "k-agg", note: "rolled up from child rows" },
  relationship: { label: "relationship", cls: "k-rel", note: "a foreign key" },
  unmapped: { label: "unmapped", cls: "k-un", note: "on the view, not in the catalog" },
};
const kindOf = (k) => KIND[k] || KIND.unmapped;

// ---------- state ----------
let inf = null;                 // /api/explore/inferences
let ifilters = { kind: "", table: "", q: "", witness: false };
let tables = null;              // /api/explore/tables
let tableSel = null;            // { table, view, pk, columns, rows }
let record = null;              // /api/explore/row/...
let recordErr = "";
let recTarget = { table: "", id: "" };
let rowFilter = "";

export async function loadExplore() {
  [inf, tables] = await Promise.all([
    api("/api/explore/inferences"),
    api("/api/explore/tables"),
  ]);
}

export function exploreCount(tab) {
  if (tab === "inferences") return inf?.total ?? null;
  if (tab === "tables") return tables?.length ?? null;
  return null;
}

// ---------- shared bits ----------
const kindPill = (k) =>
  `<span class="kpill ${kindOf(k).cls}">${esc(kindOf(k).label)}</span>`;

// A value, rendered by what it IS. Booleans are the witnesses, so they get the
// strongest treatment — a witness reading false is as much a finding as true.
function val(v, datatype) {
  if (v === null || v === undefined || v === "")
    return `<span class="vnull">—</span>`;
  if (typeof v === "boolean" || datatype === "boolean") {
    const t = v === true || v === "t" || v === "true";
    return `<span class="vbool ${t ? "is-t" : "is-f"}">${t ? "true" : "false"}</span>`;
  }
  if (typeof v === "number") return `<span class="vnum mono">${esc(v)}</span>`;
  const s = String(v);
  if (s.length > 140) return `<span class="vstr" title="${esc(s)}">${esc(s.slice(0, 140))}…</span>`;
  return `<span class="vstr">${esc(s)}</span>`;
}

// The formula, shown as evidence. Field references are highlighted so the
// inputs listed underneath can be matched to their place in the expression.
function formulaHtml(f) {
  if (!f) return "";
  return `<code class="xformula">${esc(f).replace(
    /(?:([A-Za-z_][A-Za-z0-9_]*)\s*!\s*)?\{\{\s*([A-Za-z0-9_]+)\s*\}\}/g,
    (_m, t, fld) =>
      `<span class="fref">${t ? `<span class="fref-t">${esc(t)}!</span>` : ""}${esc(fld)}</span>`
  )}</code>`;
}

// The input list under an inference: the raw values it was computed from.
function inputsHtml(inputs) {
  if (!inputs?.length) return "";
  return `<div class="xinputs">
    <div class="xinputs-h">computed from</div>
    ${inputs.map((i) => {
      let v;
      if (i.aggregate)
        v = `<span class="vagg">many rows — see ${esc(i.table)}</span>`;
      else if (i.unavailable)
        v = `<span class="vnull">${esc(i.unavailable)}</span>`;
      else v = val(i.value, i.datatype);
      return `<div class="xinput">
        <span class="xinput-k mono">${
          i.isLocal ? "" : `<span class="fref-t">${esc(i.table)}!</span>`
        }${esc(i.field)}</span>
        ${i.fieldType ? kindPill(i.fieldType) : ""}
        <span class="xinput-v">${v}</span>
        ${i.via ? `<span class="xinput-via mono">via ${esc(i.via)}</span>` : ""}
        ${i.isDerived ? `<span class="xchain">itself derived</span>` : ""}
      </div>`;
    }).join("")}
  </div>`;
}

// ---------- 1. every inference ----------
export function viewInferences() {
  if (!inf) return `<div class="card" style="padding:24px">Loading…</div>`;

  const tableOpts = [...new Set(inf.fields.map((f) => f.table))].sort();

  return `
  <div class="page-head">
    <div class="eyebrow">Explorer</div>
    <h2>Every Inference</h2>
    <p>The model states ${inf.rawTotal} facts and infers ${inf.total} more from them.
    Every inferred field is here, with the formula that defines it and the fields
    it reads. Nothing on this screen was computed by the browser — each formula
    was executed by Postgres, and what you see is the column it wrote.</p>
  </div>

  <div class="atiles">
    <div class="atile"><div class="atile-n">${inf.rawTotal}</div><div class="atile-l">raw fields</div></div>
    <div class="atile pass"><div class="atile-n">${inf.total}</div><div class="atile-l">inferred</div></div>
    ${inf.byKind.map((k) => `
      <div class="atile"><div class="atile-n">${k.n}</div><div class="atile-l">${esc(k.kind)}</div></div>`).join("")}
  </div>
  <p class="muted asub">Ratio ${(inf.total / Math.max(inf.rawTotal, 1)).toFixed(2)}:1 — the
  model derives more than it stores. That is the point: each inference is a claim
  the substrate can be held to, not a value someone typed.</p>

  ${inf.unresolved?.length ? `
  <div class="notice"><span class="ni">Drift</span><div>
    ${inf.unresolved.length} catalog field${inf.unresolved.length === 1 ? "" : "s"}
    ${inf.unresolved.length === 1 ? "has" : "have"} no matching column in the views:
    <span class="mono">${inf.unresolved.slice(0, 8).map(esc).join(", ")}</span>${inf.unresolved.length > 8 ? " …" : ""}.
    The rulebook declares them; the substrate has not been rebuilt. This is reported
    rather than hidden — a field with no column cannot be shown a value.
  </div></div>` : ""}

  <div class="card asect">
    <div class="afilters">
      <input id="xq" class="ainput" style="flex:1" placeholder="Search field name or formula…" value="${esc(ifilters.q)}">
      <select id="xkind" class="ainput">
        <option value="">every kind</option>
        ${inf.byKind.map((k) => `<option value="${esc(k.kind)}"${ifilters.kind === k.kind ? " selected" : ""}>${esc(k.kind)} (${k.n})</option>`).join("")}
      </select>
      <select id="xtable" class="ainput">
        <option value="">every table</option>
        ${tableOpts.map((t) => `<option value="${esc(t)}"${ifilters.table === t ? " selected" : ""}>${esc(t)}</option>`).join("")}
      </select>
      <label class="xcheck"><input type="checkbox" id="xwit" ${ifilters.witness ? "checked" : ""}> witnesses only</label>
    </div>
    <div id="xlist" class="xlist">Loading…</div>
  </div>`;
}

async function refreshInferences() {
  const el = document.getElementById("xlist");
  if (!el) return;
  const qs = new URLSearchParams();
  if (ifilters.kind) qs.set("kind", ifilters.kind);
  if (ifilters.table) qs.set("table", ifilters.table);
  if (ifilters.q) qs.set("q", ifilters.q);
  if (ifilters.witness) qs.set("witness", "true");
  try {
    const d = await api(`/api/explore/inferences?${qs}`);
    el.innerHTML = `
      <p class="muted asub">${d.matched} of ${d.total} inferred fields${d.truncated ? " (showing 600)" : ""}</p>
      ${d.fields.map((f) => `
        <div class="xrow">
          <div class="xrow-h">
            <button class="alink mono xrow-t" data-record="${esc(f.table)}">${esc(f.table)}.<b>${esc(f.field)}</b></button>
            ${kindPill(f.fieldType)}
            ${f.isWitness ? `<span class="tagb">witness</span>` : ""}
            <span class="muted small mono">${esc(f.datatype ?? "")}</span>
          </div>
          ${formulaHtml(f.formula)}
          ${f.deps?.length ? `<div class="xdeps">reads
            ${f.deps.map((d2) => `<span class="xdep mono">${d2.isLocal ? "" : `<span class="fref-t">${esc(d2.table)}!</span>`}${esc(d2.field)}</span>`).join("")}
          </div>` : ""}
          ${f.question ? `<div class="xq-line">answers <button class="alink small" data-q="${esc(f.question)}">${esc(f.question)}</button></div>` : ""}
        </div>`).join("")}`;
  } catch (err) {
    el.innerHTML = `<p class="afail">${esc(err.message)}</p>`;
  }
}

// ---------- 2. all tables ----------
export function viewTables() {
  if (!tables) return `<div class="card" style="padding:24px">Loading…</div>`;

  if (tableSel) return viewOneTable();

  const tot = tables.reduce((a, t) => ({
    total: a.total + t.total, derived: a.derived + t.derived, rows: a.rows + (t.rowCount ?? 0),
  }), { total: 0, derived: 0, rows: 0 });

  return `
  <div class="page-head">
    <div class="eyebrow">Explorer</div>
    <h2>All Tables</h2>
    <p>Every table in the model, including the ${tables.filter((t) => t.total > 40).length}
    wide ones the curated screens only sample. Open any table to see all of its
    columns and rows — raw and inferred side by side.</p>
  </div>

  <div class="atiles">
    <div class="atile"><div class="atile-n">${tables.length}</div><div class="atile-l">tables</div></div>
    <div class="atile"><div class="atile-n">${tot.total}</div><div class="atile-l">fields</div></div>
    <div class="atile pass"><div class="atile-n">${tot.derived}</div><div class="atile-l">inferred</div></div>
    <div class="atile"><div class="atile-n">${tot.rows}</div><div class="atile-l">rows</div></div>
  </div>

  <div class="card asect">
    <table class="atable xtables">
      <thead><tr>
        <th>Table</th><th class="n">Rows</th><th class="n">Fields</th>
        <th>Composition</th><th class="n">Witnesses</th>
      </tr></thead>
      <tbody>${tables.map((t) => {
        const seg = (n, cls) => n ? `<span class="${cls}" style="flex:${n}" title="${n} ${cls.slice(2)}"></span>` : "";
        return `<tr>
          <td>${t.hasView
              ? `<button class="alink mono" data-table="${esc(t.table)}"><b>${esc(t.table)}</b></button>`
              : `<span class="mono">${esc(t.table)}</span>
                 <span class="taga" title="The transpiler emits no view for rulebook infrastructure tables — this is by design, not a failed build.">infrastructure</span>`}</td>
          <td class="n mono">${t.rowCount ?? "—"}</td>
          <td class="n mono">${t.total}</td>
          <td><div class="xbar">
            ${seg(t.raw, "k-raw")}${seg(t.calculated, "k-calc")}${seg(t.lookup, "k-look")}${seg(t.aggregation, "k-agg")}${seg(t.relationship, "k-rel")}
          </div></td>
          <td class="n mono">${t.witnesses || ""}</td>
        </tr>`;
      }).join("")}</tbody>
    </table>
    <div class="xkey">
      ${Object.entries(KIND).filter(([k]) => k !== "unmapped").map(([k, v]) =>
        `<span class="xkey-i"><span class="xkey-s ${v.cls}"></span>${esc(v.label)} — <span class="muted">${esc(v.note)}</span></span>`).join("")}
    </div>
  </div>`;
}

// One table: the full column catalog, then every row.
function viewOneTable() {
  const t = tableSel;
  const q = rowFilter.toLowerCase();
  const rows = q
    ? t.rows.filter((r) => Object.values(r).some((v) => String(v ?? "").toLowerCase().includes(q)))
    : t.rows;
  const derived = t.columns.filter((c) => c.isDerived);

  return `
  <div class="page-head">
    <button class="alink" data-back="tables">← all tables</button>
    <h2 class="mono">${esc(t.table)}</h2>
    <p>${t.columns.length} columns — ${t.columns.length - derived.length} raw,
    ${derived.length} inferred — over ${t.rows.length} rows.
    Read from <span class="mono">${esc(t.view)}</span>.</p>
  </div>

  <div class="card asect">
    <h3 class="ah">Columns</h3>
    <div class="xcols">
      ${t.columns.map((c) => `
        <div class="xcol ${c.isDerived ? "is-d" : ""}">
          <div class="xcol-h">
            <span class="mono xcol-n">${esc(c.field)}</span>
            ${kindPill(c.fieldType)}
            ${c.isWitness ? `<span class="tagb">witness</span>` : ""}
          </div>
          ${formulaHtml(c.formula)}
          ${c.deps?.length ? `<div class="xdeps">reads
            ${c.deps.map((d) => `<span class="xdep mono">${d.isLocal ? "" : `<span class="fref-t">${esc(d.table)}!</span>`}${esc(d.field)}</span>`).join("")}
          </div>` : ""}
          ${c.question ? `<div class="xq-line">answers <button class="alink small" data-q="${esc(c.question)}">${esc(c.question)}</button></div>` : ""}
        </div>`).join("")}
    </div>
  </div>

  <div class="card asect">
    <h3 class="ah">Rows</h3>
    <div class="afilters">
      <input id="xrq" class="ainput" style="flex:1" placeholder="Filter rows…" value="${esc(rowFilter)}">
      <span class="muted small">${rows.length} of ${t.rows.length}</span>
    </div>
    <div class="scroll-x xgrid-wrap">
      <table class="atable xgrid">
        <thead><tr>${t.columns.map((c) =>
          `<th class="${c.isDerived ? "is-d" : ""}" title="${esc(c.formula || kindOf(c.fieldType).note)}">
            <span class="mono">${esc(c.field)}</span>
            <span class="xgrid-k ${kindOf(c.fieldType).cls}"></span>
          </th>`).join("")}</tr></thead>
        <tbody>${rows.slice(0, 200).map((r) => `
          <tr>${t.columns.map((c, i) => `
            <td class="${c.isDerived ? "is-d" : ""}">
              ${i === 0
                ? `<button class="alink mono" data-record="${esc(t.table)}" data-id="${esc(r[c.column])}">${esc(r[c.column])}</button>`
                : c.isDerived
                  ? `<button class="xcell" data-cell="${esc(t.table)}|${esc(r[t.pk])}|${esc(c.column)}">${val(r[c.column], c.datatype)}</button>`
                  : val(r[c.column], c.datatype)}
            </td>`).join("")}</tr>`).join("")}
        </tbody>
      </table>
    </div>
    ${rows.length > 200 ? `<p class="muted asub">Showing 200 of ${rows.length} rows.</p>` : ""}
    <p class="muted asub">Shaded columns are inferred. Click any inferred cell to
    see the formula and the values it was computed from.</p>
  </div>`;
}

// ---------- 3. record inspector ----------
export function viewRecord() {
  const head = `
  <div class="page-head">
    <div class="eyebrow">Explorer</div>
    <h2>Record Inspector</h2>
    <p>One row, every field it has. Raw values first, then each inference with
    the inputs it read — so a wide table like
    <span class="mono">StepExecutions</span> (109 columns) is fully visible
    instead of sampled.</p>
  </div>
  <div class="card asect">
    <div class="afilters">
      <select id="rectable" class="ainput">
        <option value="">choose a table…</option>
        ${(tables ?? []).filter((t) => t.hasView && t.rowCount).map((t) =>
          `<option value="${esc(t.table)}"${recTarget.table === t.table ? " selected" : ""}>${esc(t.table)} (${t.rowCount})</option>`).join("")}
      </select>
      <select id="recid" class="ainput" style="flex:1">
        ${recIdOptions()}
      </select>
    </div>
    ${recordErr ? `<p class="afail">${esc(recordErr)}</p>` : ""}
  </div>`;

  if (!record) return head;

  const raw = record.fields.filter((f) => !f.isDerived);
  const der = record.fields.filter((f) => f.isDerived);
  const wit = der.filter((f) => f.isWitness);

  return head + `
  <div class="atiles">
    <div class="atile"><div class="atile-n">${record.fields.length}</div><div class="atile-l">fields on this row</div></div>
    <div class="atile"><div class="atile-n">${raw.length}</div><div class="atile-l">raw</div></div>
    <div class="atile pass"><div class="atile-n">${der.length}</div><div class="atile-l">inferred</div></div>
    <div class="atile"><div class="atile-n">${wit.length}</div><div class="atile-l">witnesses</div></div>
  </div>

  <div class="card asect">
    <h3 class="ah">Stated — the ${raw.length} values this row carries</h3>
    <p class="muted asub">Everything below was entered or loaded. Nothing here was computed.</p>
    <div class="xfields">
      ${raw.map((f) => `
        <div class="xfield">
          <div class="xfield-n mono">${esc(f.field)} ${kindPill(f.fieldType)}</div>
          <div class="xfield-v">${val(f.value, f.datatype)}</div>
        </div>`).join("")}
    </div>
  </div>

  <div class="card asect">
    <h3 class="ah">Inferred — the ${der.length} values the model computed</h3>
    <p class="muted asub">Each one shows its formula and the current value of every
    field that formula reads. Follow an input that is itself derived to keep walking back.</p>
    ${der.map((f) => `
      <div class="xinf ${f.isWitness ? "is-w" : ""}">
        <div class="xinf-h">
          <span class="mono xinf-n">${esc(f.field)}</span>
          ${kindPill(f.fieldType)}
          ${f.isWitness ? `<span class="tagb">witness</span>` : ""}
          <span class="xinf-v">${val(f.value, f.datatype)}</span>
        </div>
        ${formulaHtml(f.formula)}
        ${inputsHtml(f.inputs)}
        ${f.question ? `<div class="xq-line">answers <button class="alink small" data-q="${esc(f.question)}">${esc(f.question)}</button></div>` : ""}
      </div>`).join("")}
  </div>`;
}

let recIds = [];
function recIdOptions() {
  if (!recIds.length) return `<option value="">—</option>`;
  return recIds.map((id) =>
    `<option value="${esc(id)}"${recTarget.id === id ? " selected" : ""}>${esc(id)}</option>`).join("");
}

async function loadRecIds(table) {
  const d = await api(`/api/explore/table/${encodeURIComponent(table)}`);
  recIds = d.rows.map((r) => r[d.pk]);
}

// Open a row in the record inspector. Callable from ANY screen — the popover
// offers "open the whole record" on curated pages too, where wireExplore has
// never run. rerender() is main.js's goTo, which switches into the Explorer
// role if the caller was somewhere else.
async function openRecord(table, id, rerender) {
  recTarget = { table, id: id ?? "" };
  recordErr = "";
  try {
    await loadRecIds(table);
    if (!recTarget.id && recIds.length) recTarget.id = recIds[0];
    if (recTarget.id) await loadRecord(table, recTarget.id);
  } catch (err) {
    recordErr = err.message;
  }
  closePop();
  rerender("record");
}

async function loadRecord(table, id) {
  recordErr = "";
  record = null;
  try {
    record = await api(`/api/explore/row/${encodeURIComponent(table)}/${encodeURIComponent(id)}`);
  } catch (err) {
    recordErr = err.message;
  }
}

// ---------- the cell popover ----------
// Available on ANY screen. Attaches to elements carrying data-cell.
let pop = null;
function closePop() {
  pop?.remove();
  pop = null;
}
async function openPop(anchor, table, id, column, rerender = null) {
  closePop();
  pop = document.createElement("div");
  pop.className = "xpop";
  pop.innerHTML = `<div class="xpop-load">Reading ${esc(column)}…</div>`;
  document.body.appendChild(pop);
  place(anchor);

  try {
    const d = await api(
      `/api/explore/cell/${encodeURIComponent(table)}/${encodeURIComponent(id)}/${encodeURIComponent(column)}`);
    pop.innerHTML = `
      <div class="xpop-h">
        <span class="mono">${esc(d.table)}.${esc(d.field)}</span>
        ${kindPill(d.fieldType)}
        ${d.isWitness ? `<span class="tagb">witness</span>` : ""}
        <button class="x-btn xpop-x" aria-label="Close">×</button>
      </div>
      <div class="xpop-v">${val(d.value, d.datatype)}</div>
      ${d.isDerived ? `
        ${formulaHtml(d.formula)}
        ${inputsHtml(d.inputs)}
        ${rerender ? `<div class="xpop-f">
          <button class="alink small" id="xpop-rec">open the whole record →</button>
        </div>` : ""}`
        : `<p class="muted small">A stored value. Nothing computed it.</p>`}
      ${d.question ? `<div class="xq-line">answers <button class="alink small" data-q="${esc(d.question)}">${esc(d.question)}</button></div>` : ""}`;
    pop.querySelector(".xpop-x").onclick = closePop;
    const rec = pop.querySelector("#xpop-rec");
    if (rec) rec.onclick = () => openRecord(d.table, d.id, rerender);
    place(anchor);
  } catch (err) {
    pop.innerHTML = `<p class="afail">${esc(err.message)}</p>`;
  }
}
function place(anchor) {
  if (!pop) return;
  const r = anchor.getBoundingClientRect();
  const pw = Math.min(420, window.innerWidth - 24);
  pop.style.width = `${pw}px`;
  let left = r.left + window.scrollX;
  left = Math.min(left, window.scrollX + window.innerWidth - pw - 12);
  left = Math.max(left, window.scrollX + 12);
  pop.style.left = `${left}px`;
  const below = window.innerHeight - r.bottom;
  const ph = pop.offsetHeight;
  pop.style.top = below > ph + 16 || below > r.top
    ? `${r.bottom + window.scrollY + 6}px`
    : `${r.top + window.scrollY - ph - 6}px`;
}
document.addEventListener("keydown", (e) => { if (e.key === "Escape") closePop(); });
document.addEventListener("click", (e) => {
  if (pop && !pop.contains(e.target) && !e.target.closest("[data-cell]")) closePop();
});

// ---------- wiring ----------
export function wireExplore(tab, rerender) {
  if (tab === "inferences") {
    refreshInferences();
    const q = document.getElementById("xq");
    if (q) q.oninput = (e) => {
      ifilters.q = e.target.value;
      clearTimeout(q._t);
      q._t = setTimeout(refreshInferences, 150);
    };
    const k = document.getElementById("xkind");
    if (k) k.onchange = (e) => { ifilters.kind = e.target.value; refreshInferences(); };
    const t = document.getElementById("xtable");
    if (t) t.onchange = (e) => { ifilters.table = e.target.value; refreshInferences(); };
    const w = document.getElementById("xwit");
    if (w) w.onchange = (e) => { ifilters.witness = e.target.checked; refreshInferences(); };
  }

  if (tab === "tables" && tableSel) {
    const rq = document.getElementById("xrq");
    if (rq) rq.oninput = (e) => {
      rowFilter = e.target.value;
      const pos = e.target.selectionStart;
      document.getElementById("view").innerHTML = viewTables();
      wireExplore("tables", rerender);
      const n = document.getElementById("xrq");
      if (n) { n.focus(); n.setSelectionRange(pos, pos); }
    };
  }

  if (tab === "record") {
    const t = document.getElementById("rectable");
    if (t) t.onchange = async (e) => {
      recTarget = { table: e.target.value, id: "" };
      record = null; recordErr = "";
      if (recTarget.table) {
        try {
          await loadRecIds(recTarget.table);
          if (recIds.length) {
            recTarget.id = recIds[0];
            await loadRecord(recTarget.table, recTarget.id);
          }
        } catch (err) { recordErr = err.message; }
      }
      rerender();
    };
    const i = document.getElementById("recid");
    if (i) i.onchange = async (e) => {
      recTarget.id = e.target.value;
      await loadRecord(recTarget.table, recTarget.id);
      rerender();
    };
  }

  // --- cross-screen links, live on every tab ---
  for (const el of document.querySelectorAll("[data-table]")) {
    el.onclick = async () => {
      try {
        tableSel = await api(`/api/explore/table/${encodeURIComponent(el.dataset.table)}`);
        rowFilter = "";
      } catch (err) { alert(err.message); return; }
      rerender("tables");
    };
  }
  for (const el of document.querySelectorAll("[data-back]")) {
    el.onclick = () => { tableSel = null; rerender("tables"); };
  }
  for (const el of document.querySelectorAll("[data-record]")) {
    el.onclick = () => openRecord(el.dataset.record, el.dataset.id, rerender);
  }
  for (const el of document.querySelectorAll("[data-cell]")) {
    el.onclick = (e) => {
      e.stopPropagation();
      const [table, id, column] = el.dataset.cell.split("|");
      openPop(el, table, id, column);
    };
  }
}

// Exposed so the curated screens can offer the same popover without importing
// the whole module's state. `rerender` is main.js's goTo — it is what lets the
// popover's "open the whole record" link carry an Operator into the Explorer.
export function wireCells(root = document, rerender = null) {
  for (const el of root.querySelectorAll("[data-cell]")) {
    el.onclick = (e) => {
      e.stopPropagation();
      const [table, id, column] = el.dataset.cell.split("|");
      openPop(el, table, id, column, rerender);
    };
  }
}
