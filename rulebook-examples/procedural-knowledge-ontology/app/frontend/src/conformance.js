// ============================================================================
// Conformance — does every substrate compute the same answers?
//
// This rulebook is compiled into PostgreSQL, Python, Go, TypeScript, C#, an
// Excel workbook and an OWL reasoner. compile-rulebook bakes every derived value
// into the rulebook itself; those stored values are the answer keys, and the
// conformance harness grades every other substrate against them cell by cell.
//
// THE VIEW IS STILL THE CONTRACT. Scores, counts, "is perfect", "fully sampled"
// are columns on vw_conformance_*, computed from rows transcribed out of the
// harness by tools/record_conformance.py. The harness decided every pass and
// fail. Nothing below compares an expected value to an actual one.
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

export const CONFORMANCE_TABS = {
  "c-board": "Scoreboard",
  "c-grid": "Table × Substrate",
  "c-fields": "Disagreements",
};

// ---------- state ----------
let summary = null;   // /api/conformance
let grid = null;      // /api/conformance/grid
let fields = null;    // /api/conformance/fields
let fieldsKey = "";
let ffilter = { substrate: "", table: "", q: "" };
let gridAll = false;
let detail = null;    // /api/conformance/field/:id
let detailErr = "";

export async function loadConformance() {
  [summary, grid] = await Promise.all([api("/api/conformance"), api("/api/conformance/grid")]);
}

export function conformanceCount(tab) {
  if (!summary) return null;
  if (tab === "c-board") return summary.substrates.filter((s) => s.is_graded).length;
  if (tab === "c-grid") return grid?.tables.filter((t) => Number(t.disagreeing_substrate_count) > 0).length ?? null;
  // Rows the views produced, not a recount: node-pg returns numeric columns as
  // strings, so every numeric read in this module goes through Number().
  if (tab === "c-fields") return summary.substrates.reduce((n, s) => n + Number(s.disagreeing_field_count || 0), 0);
  return null;
}

// ---------- shared bits ----------
const pct = (n) => (n == null ? "—" : `${Number(n).toFixed(Number(n) === 100 ? 0 : 2)}%`);
const num = (n) => (n == null ? "—" : Number(n).toLocaleString("en-US"));
const graded = () => summary.substrates.filter((s) => s.is_graded);

function statTile(l, value, cls) {
  return `<div class="atile ${cls || ""}"><div class="atile-n">${esc(value)}</div><div class="atile-l">${esc(l)}</div></div>`;
}

function scoreBar(score, perfect) {
  const w = Math.max(0, Math.min(100, Number(score) || 0));
  return `<div class="bar cbar"><span class="${perfect ? "" : w >= 95 ? "warn" : "fail"}" style="width:${w}%"></span></div>`;
}

function statusPill(s) {
  if (Number(s.latest_harness_errors) > 0) return `<span class="pill fail">harness error</span>`;
  if (s.is_fully_conformant) return `<span class="pill pass">agrees everywhere</span>`;
  if (!Number(s.latest_cells_tested)) return `<span class="pill pend">not graded</span>`;
  return `<span class="pill warn">disagrees</span>`;
}

// ---------- 1. scoreboard ----------
export function viewConformanceBoard() {
  if (!summary) return `<div class="card" style="padding:24px">Loading…</div>`;
  const run = summary.latest;
  if (!run) {
    return `<div class="card" style="padding:26px">
      <h2 style="font-size:17px;margin-bottom:10px">No conformance run recorded yet</h2>
      <p class="prose">Run the harness, record it, and rebuild:</p>
      <p class="mono" style="font-size:13px">python3 ../../scripts/run-conformance.py procedural-knowledge-ontology --skip-build<br>
      python3 tools/record_conformance.py &amp;&amp; effortless build &amp;&amp; bash init-db.sh</p></div>`;
  }
  const author = summary.substrates.find((s) => s.role === "answer-key");
  const scoreOf = new Map(summary.scores.map((r) => [r.substrate, r]));

  return `
  <div class="abanner ${run.is_fully_conformant ? "is-green" : "is-red"}">
    <div>
      <div class="abanner-t">${run.is_fully_conformant
        ? "Every substrate agrees on every cell"
        : `${run.imperfect_substrate_count} of ${run.substrate_count} substrates disagree somewhere`}</div>
      <div class="abanner-s">${num(run.cells_passed)} of ${num(run.cells_tested)} graded cells agree
        (${pct(run.overall_score)}). Run <span class="mono">${esc(run.conformance_run_id)}</span>,
        ${esc(new Date(run.ran_on).toLocaleString())}, commit <span class="mono">${esc(run.rulebook_commit)}</span>.</div>
    </div>
  </div>

  <div class="atiles">
    ${statTile("substrates graded", run.substrate_count)}
    ${statTile("agree everywhere", run.perfect_substrate_count, "pass")}
    ${statTile("disagree", run.imperfect_substrate_count, Number(run.imperfect_substrate_count) ? "fail" : "")}
    ${statTile("cells graded", num(run.cells_tested))}
    ${statTile("cells disagreeing", num(run.cells_failed), Number(run.cells_failed) ? "warn" : "")}
  </div>

  ${author ? `<div class="card asect cexplain">
    <h3 class="ah">What “agree” means here</h3>
    <p class="prose">The answer key is written by <b class="mono">${esc(author.transpiler)}</b>
    (${esc(author.engine)}). ${esc(author.how_it_computes)}</p>
    <p class="muted" style="margin:0">A cell <i>disagrees</i> when a substrate's value differs from that
    stored value, or the substrate produced none. When several unrelated engines disagree with the key
    in exactly the same way, look hard at the key.</p>
  </div>` : ""}

  <div class="card asect">
    <h3 class="ah">Substrates</h3>
    <div class="ctable-wrap"><table class="atable ctable">
      <thead><tr><th>Substrate</th><th style="min-width:150px">Score</th><th class="n">Disagreeing cells</th>
        <th class="n">Calculated</th><th class="n">Lookup</th><th class="n">Aggregation</th>
        <th class="n">Fields</th><th class="n">Tables</th><th>Status</th></tr></thead>
      <tbody>${graded().map((s) => {
        const r = scoreOf.get(s.conformance_substrate_id) || {};
        return `<tr>
          <td><button class="alink" data-csub="${esc(s.conformance_substrate_id)}"><b>${esc(s.label)}</b></button>
            <div class="csub mono">${esc(s.transpiler)} → ${esc(s.output_folder)}</div>
            <div class="csub">${esc(s.engine)}</div>
            <details class="chow"><summary>how it computes</summary><p>${esc(s.how_it_computes)}</p>
              ${r.harness_error ? `<p class="isfail mono">${esc(r.harness_error)}</p>` : ""}</details></td>
          <td>${scoreBar(s.latest_score, s.is_fully_conformant)}<div class="mono cpct">${pct(s.latest_score)}</div></td>
          <td class="n mono ${Number(s.latest_cells_failed) ? "isfail" : ""}">${num(s.latest_cells_failed)}</td>
          <td class="n mono">${pct(r.calculated_score)}</td>
          <td class="n mono">${pct(r.lookup_score)}</td>
          <td class="n mono">${pct(r.aggregation_score)}</td>
          <td class="n mono">${num(s.disagreeing_field_count)}</td>
          <td class="n mono">${num(s.disagreeing_table_count)}</td>
          <td>${statusPill(s)}</td>
        </tr>`;
      }).join("")}</tbody>
    </table></div>
  </div>

  <div class="card asect">
    <h3 class="ah">Run history</h3>
    <div class="ctable-wrap"><table class="atable">
      <thead><tr><th>Run</th><th>When</th><th class="n">Substrates</th><th class="n">Agree everywhere</th>
        <th class="n">Overall</th><th>Commit</th><th>Notes</th></tr></thead>
      <tbody>${summary.runs.map((x) => `<tr>
        <td class="mono">${esc(x.conformance_run_id)}${x.is_latest ? ` <span class="pill pend">latest</span>` : ""}</td>
        <td>${esc(new Date(x.ran_on).toLocaleString())}</td>
        <td class="n mono">${x.substrate_count}</td>
        <td class="n mono">${x.perfect_substrate_count}</td>
        <td class="n mono">${pct(x.overall_score)}</td>
        <td class="mono">${esc(x.rulebook_commit)}</td>
        <td class="muted">${esc(x.notes ?? "")}</td></tr>`).join("")}</tbody>
    </table></div>
  </div>`;
}

// ---------- 2. table x substrate ----------
export function viewConformanceGrid() {
  if (!grid) return `<div class="card" style="padding:24px">Loading…</div>`;
  const subs = graded();
  const cell = new Map(grid.cells.map((c) => [`${c.substrate}|${c.rulebook_table}`, c]));
  const rows = grid.tables.filter((t) => gridAll || Number(t.disagreeing_substrate_count) > 0);
  const hidden = grid.tables.length - rows.length;

  return `
  <div class="card asect">
    <h3 class="ah">Where the substrates disagree</h3>
    <p class="muted" style="margin:-6px 0 14px">Each cell is one substrate on one table: the number of
    derived cells it got wrong. Click one to see which fields.
    ${hidden ? `${hidden} tables where every substrate agrees are hidden.` : ""}
    <button class="alink" id="cgridall">${gridAll ? "Hide tables that agree" : "Show all tables"}</button></p>
    <div class="ctable-wrap"><table class="atable cgrid">
      <thead><tr><th>Table</th><th>Area</th>${subs.map((s) => `<th class="n">${esc(s.label)}</th>`).join("")}</tr></thead>
      <tbody>${rows.map((t) => `<tr>
        <td class="mono">${esc(t.rulebook_table_id)}</td>
        <td class="muted">${esc(t.subject_area)}</td>
        ${subs.map((s) => {
          const c = cell.get(`${s.conformance_substrate_id}|${t.rulebook_table_id}`);
          if (!c) return `<td class="n"><span class="muted" title="not graded: no derived fields or no rows">·</span></td>`;
          const cls = c.is_perfect ? "ok" : c.is_missing_answer_file ? "miss" : Number(c.score) >= 95 ? "near" : "bad";
          return `<td class="n"><button class="cg ${cls}" data-cgsub="${esc(s.conformance_substrate_id)}" data-cgtable="${esc(t.rulebook_table_id)}"
            title="${esc(s.label)} on ${esc(t.rulebook_table_id)}: ${num(c.cells_passed)}/${num(c.cells_tested)} (${pct(c.score)})${c.is_missing_answer_file ? " — no answers produced" : ""}">
            ${c.is_perfect ? "✓" : num(c.cells_failed)}</button></td>`;
        }).join("")}
      </tr>`).join("")}</tbody>
    </table></div>
  </div>`;
}

// ---------- 3. disagreements ----------
function fieldsQuery() {
  const p = new URLSearchParams();
  for (const [k, v] of Object.entries(ffilter)) if (v) p.set(k, v);
  return p.toString();
}

function detailHtml() {
  if (detailErr) return `<div class="card asect"><p class="isfail">${esc(detailErr)}</p></div>`;
  if (!detail) return "";
  const d = detail;
  const f = d.field;
  const table = f.target_table;
  const bySub = new Map(d.disagreements.map((x) => [x.substrate, x]));
  // One row per record that any substrate got wrong. A substrate with no
  // FieldDisagreements row agreed on every cell of this field; one that has a
  // row but no sampled cell for this record is shown as "not sampled".
  const records = new Map();
  for (const c of d.cells) {
    if (!records.has(c.record_id)) records.set(c.record_id, { expected: c.expected_value, by: new Map() });
    records.get(c.record_id).by.set(c.substrate, c);
  }
  const subs = d.substrates;
  const show = (json) => {
    const v = JSON.parse(json);
    return v === null ? `<span class="muted">null</span>` : v === "" ? `<span class="muted">""</span>` : esc(typeof v === "string" ? v : JSON.stringify(v));
  };

  return `<div class="card asect cdetail">
    <div class="cdetail-head">
      <div><div class="eyebrow">${esc(f.field_type)} · ${esc(f.datatype)}</div>
        <h3 class="ah" style="margin:2px 0 8px">${esc(f.rulebook_field_id)}</h3></div>
      <button class="alink" id="cdclose">close</button>
    </div>
    ${f.formula ? `<pre class="cformula mono">${esc(f.formula)}</pre>` : ""}
    <div class="cverdicts">${subs.map((s) => {
      const x = bySub.get(s.conformance_substrate_id);
      return x
        ? `<span class="pill fail" title="${esc(x.dominant_reason)}">${esc(s.label)}: ${num(x.cells_failed)} wrong${x.is_fully_sampled ? "" : ` (${x.sampled_cell_count} shown)`}</span>`
        : `<span class="pill pass">${esc(s.label)}: agrees</span>`;
    }).join(" ")}</div>
    <div class="ctable-wrap"><table class="atable ccells">
      <thead><tr><th>Record</th><th>Answer key</th>${subs.map((s) => `<th>${esc(s.label)}</th>`).join("")}</tr></thead>
      <tbody>${[...records.entries()].map(([rid, r]) => `<tr>
        <td class="mono">${d.column
          ? `<button class="xcell" data-cell="${esc(table)}|${esc(rid)}|${esc(d.column)}" title="open the live Postgres value and its inputs">${esc(rid)}</button>`
          : esc(rid)}</td>
        <td class="mono">${show(r.expected)}</td>
        ${subs.map((s) => {
          const c = r.by.get(s.conformance_substrate_id);
          if (c) return `<td class="mono cwrong" title="${esc(c.reason)}">${show(c.actual_value)}</td>`;
          if (bySub.has(s.conformance_substrate_id)) return `<td class="muted">not sampled</td>`;
          return `<td class="cright">=</td>`;
        }).join("")}
      </tr>`).join("")}</tbody>
    </table></div>
  </div>`;
}

export function viewConformanceFields() {
  if (!summary) return `<div class="card" style="padding:24px">Loading…</div>`;
  const list = fields && fieldsKey === fieldsQuery() ? fields : null;
  return `
  ${detailHtml()}
  <div class="card asect">
    <h3 class="ah">Fields a substrate computes differently</h3>
    <div class="afilters">
      <select id="cfsub" class="ainput">
        <option value="">every substrate</option>
        ${graded().map((s) => `<option value="${esc(s.conformance_substrate_id)}"${ffilter.substrate === s.conformance_substrate_id ? " selected" : ""}>${esc(s.label)}</option>`).join("")}
      </select>
      <input id="cftable" class="ainput" placeholder="Table, e.g. FieldGrants" value="${esc(ffilter.table)}">
      <input id="cfq" class="ainput" placeholder="Filter by field…" value="${esc(ffilter.q)}">
    </div>
    ${!list ? `<p class="muted">Loading…</p>` : !list.length ? `<p class="muted">No disagreements match.</p>` : `
    <div class="ctable-wrap"><table class="atable">
      <thead><tr><th>Field</th><th>Class</th><th>Substrate</th><th class="n">Cells wrong</th><th>Most common reason</th></tr></thead>
      <tbody>${list.map((x) => `<tr>
        <td><button class="alink mono" data-cfield="${esc(x.rulebook_field)}">${esc(x.rulebook_field)}</button></td>
        <td><span class="kpill k-${x.field_class === "lookup" ? "look" : x.field_class === "aggregation" ? "agg" : "calc"}">${esc(x.field_class)}</span></td>
        <td>${esc(x.substrate_label)}</td>
        <td class="n mono isfail">${num(x.cells_failed)}</td>
        <td class="muted">${esc(x.dominant_reason)}</td></tr>`).join("")}</tbody>
    </table></div>`}
  </div>`;
}

// ---------- wiring ----------
// Filter typing and async loads repaint the view in place, keeping focus and
// scroll, the way admin.js does for its witness filter; a full render would
// jump to the top on every keystroke.
function repaint(goTo, rerender, wireCells) {
  const active = document.activeElement;
  const id = active?.id;
  const pos = active?.selectionStart;
  document.getElementById("view").innerHTML = viewConformanceFields();
  wireConformance("c-fields", goTo, rerender, wireCells);
  wireCells(document.getElementById("view"), goTo);
  const el = id && document.getElementById(id);
  if (el) { el.focus(); if (pos != null && el.setSelectionRange) el.setSelectionRange(pos, pos); }
}

async function openField(id, rerender) {
  detailErr = "";
  try {
    detail = await api(`/api/conformance/field/${encodeURIComponent(id)}`);
  } catch (err) {
    detail = null;
    detailErr = err.message;
  }
  rerender();
}

export function wireConformance(tab, goTo, rerender, wireCells) {
  const $ = (id) => document.getElementById(id);
  const toFields = (patch) => {
    ffilter = { substrate: "", table: "", q: "", ...patch };
    detail = null;
    goTo("c-fields");
  };

  if (tab === "c-board") {
    document.querySelectorAll("[data-csub]").forEach((b) => {
      b.onclick = () => toFields({ substrate: b.dataset.csub });
    });
  }

  if (tab === "c-grid") {
    $("cgridall").onclick = () => { gridAll = !gridAll; rerender(); };
    document.querySelectorAll("[data-cgsub]").forEach((b) => {
      b.onclick = () => toFields({ substrate: b.dataset.cgsub, table: b.dataset.cgtable });
    });
  }

  if (tab === "c-fields") {
    const key = fieldsQuery();
    if (fieldsKey !== key || !fields) {
      fieldsKey = key;
      fields = null;
      api(`/api/conformance/fields?${key}`).then((rows) => {
        if (fieldsKey === key) { fields = rows; repaint(goTo, rerender, wireCells); }
      }).catch((err) => { detailErr = err.message; repaint(goTo, rerender, wireCells); });
    }
    let timer = null;
    const refilter = () => {
      clearTimeout(timer);
      timer = setTimeout(() => {
        ffilter = { substrate: $("cfsub").value, table: $("cftable").value.trim(), q: $("cfq").value.trim() };
        repaint(goTo, rerender, wireCells);
      }, 250);
    };
    $("cfsub").onchange = refilter;
    $("cftable").oninput = refilter;
    $("cfq").oninput = refilter;
    document.querySelectorAll("[data-cfield]").forEach((b) => {
      b.onclick = () => openField(b.dataset.cfield, rerender);
    });
    if ($("cdclose")) $("cdclose").onclick = () => { detail = null; detailErr = ""; rerender(); };
  }
}
