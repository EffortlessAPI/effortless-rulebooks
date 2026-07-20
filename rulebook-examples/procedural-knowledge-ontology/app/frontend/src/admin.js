// ============================================================================
// Admin — the witness layer looking at itself.
//
// Four panels: the test board, the witness board, the loops-and-questions
// browser, and a provenance tracer. Every number here was computed by Postgres
// from the rulebook's own formulas; this module renders readings, it does not
// derive them.
// ============================================================================

const esc = (s) =>
  String(s ?? "").replace(/[&<>"']/g, (c) =>
    ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));

const api = async (path, opts) => {
  const r = await fetch(path, opts);
  if (!r.ok) {
    const body = await r.json().catch(() => ({}));
    throw new Error(body.error || `${r.status} ${r.statusText}`);
  }
  return r.json();
};

const OUTCOME_CLASS = { PASS: "pass", WARN: "warn", FAIL: "fail", SKIP: "pend" };
const VERDICT_CLASS = {
  discriminates: "pass",
  "always true": "warn",
  "never true": "warn",
  empty: "pend",
};

// ---------- state ----------
let board = null;
let witnesses = null;
let loopData = null;
let filters = { kind: "", outcome: "", q: "" };
let wfilter = "";
let running = false;

export const ADMIN_TABS = {
  board: "Test Board",
  witnesses: "Witness Board",
  loops: "Loops & Questions",
  trace: "Provenance",
};

export async function loadAdmin() {
  [board, witnesses, loopData] = await Promise.all([
    api("/api/admin/board"),
    api("/api/admin/witnesses"),
    api("/api/admin/loops"),
  ]);
}

export function adminCount(tab) {
  if (!board) return null;
  if (tab === "board") return board.totals.total;
  if (tab === "witnesses") return witnesses?.length ?? null;
  if (tab === "loops") return loopData?.questions.length ?? null;
  return null;
}

// ---------- 1. the test board ----------
function statTile(label, value, cls) {
  return `<div class="atile ${cls || ""}">
    <div class="atile-n">${esc(value)}</div>
    <div class="atile-l">${esc(label)}</div>
  </div>`;
}

export function viewBoard() {
  if (!board) return `<div class="card" style="padding:24px">Loading…</div>`;
  const t = board.totals;
  const green = board.isGreen;

  return `
  <div class="abanner ${green ? "is-green" : "is-red"}">
    <div>
      <div class="abanner-t">${green ? "Board is green" : `${t.blocking_fail} blocking failure${t.blocking_fail === 1 ? "" : "s"}`}</div>
      <div class="abanner-s">${green
        ? "No check that would mean the model is asserting something false is failing."
        : "A blocking failure means the model is stating something untrue."}</div>
    </div>
    <button class="abtn" id="runsuite" ${running ? "disabled" : ""}>
      ${running ? "Running…" : "Run suite"}
    </button>
  </div>

  <div class="atiles">
    ${statTile("checks", t.total)}
    ${statTile("pass", t.pass, "pass")}
    ${statTile("warn", t.warn, "warn")}
    ${statTile("fail", t.fail, t.fail ? "fail" : "")}
    ${statTile("skip", t.skip, "pend")}
  </div>
  <p class="muted asub">Warnings are advisory: a witness that cannot fire against
  this seed data is not necessarily wrong, and scoring it red would only create
  pressure to fabricate rows until it went green. Last run
  ${t.last_run_at ? esc(new Date(t.last_run_at).toLocaleString()) : "never"}.</p>

  <div class="card asect">
    <h3 class="ah">Suites</h3>
    <table class="atable">
      <thead><tr><th>Suite</th><th class="n">Checks</th><th class="n">Pass</th><th class="n">Blocking fails</th><th>Status</th></tr></thead>
      <tbody>${board.suites.map((s) => `
        <tr><td>${esc(s.label)}</td>
          <td class="n mono">${esc(s.test_count)}</td>
          <td class="n mono">${esc(s.pass_count)}</td>
          <td class="n mono">${esc(s.blocking_fail_count)}</td>
          <td><span class="pill ${s.is_green ? "pass" : "fail"}">${s.is_green ? "green" : "red"}</span></td>
        </tr>`).join("")}
      </tbody>
    </table>
  </div>

  <div class="card asect">
    <h3 class="ah">By check kind</h3>
    <table class="atable">
      <thead><tr><th>Kind</th><th class="n">Total</th><th class="n">Pass</th><th class="n">Warn</th><th class="n">Fail</th><th>Coverage</th></tr></thead>
      <tbody>${board.byKind.map((k) => {
        const pct = k.total ? Math.round((k.pass / k.total) * 100) : 0;
        return `<tr>
          <td><button class="alink" data-kind="${esc(k.test_kind)}">${esc(k.test_kind)}</button></td>
          <td class="n mono">${k.total}</td>
          <td class="n mono">${k.pass}</td>
          <td class="n mono">${k.warn || ""}</td>
          <td class="n mono ${k.fail ? "isfail" : ""}">${k.fail || ""}</td>
          <td><div class="bar"><span style="width:${pct}%"></span></div></td>
        </tr>`;
      }).join("")}
      </tbody>
    </table>
  </div>

  <div class="card asect">
    <h3 class="ah">Checks</h3>
    <div class="afilters">
      <input id="tq" class="ainput" placeholder="Filter by subject…" value="${esc(filters.q)}">
      <select id="tkind" class="ainput">
        <option value="">every kind</option>
        ${board.byKind.map((k) => `<option value="${esc(k.test_kind)}"${filters.kind === k.test_kind ? " selected" : ""}>${esc(k.test_kind)}</option>`).join("")}
      </select>
      <select id="toutcome" class="ainput">
        ${["", "FAIL", "WARN", "PASS", "SKIP"].map((o) =>
          `<option value="${o}"${filters.outcome === o ? " selected" : ""}>${o || "every outcome"}</option>`).join("")}
      </select>
    </div>
    <div id="tlist" class="alist">Loading…</div>
  </div>`;
}

async function refreshChecks() {
  const el = document.getElementById("tlist");
  if (!el) return;
  const qs = new URLSearchParams();
  if (filters.kind) qs.set("kind", filters.kind);
  if (filters.outcome) qs.set("outcome", filters.outcome);
  if (filters.q) qs.set("q", filters.q);
  try {
    const rows = await api(`/api/admin/tests?${qs}`);
    const shown = rows.slice(0, 300);
    el.innerHTML = `
      <p class="muted asub">${rows.length} check${rows.length === 1 ? "" : "s"}${rows.length > 300 ? " (showing 300)" : ""}</p>
      ${shown.map((r) => `
        <div class="arow">
          <span class="pill ${OUTCOME_CLASS[r.last_outcome] || "pend"}">${esc(r.last_outcome || "—")}</span>
          <div class="arow-b">
            <div class="arow-t mono">${esc(r.subject)}</div>
            <div class="arow-a">${esc(r.assertion)}</div>
            ${r.last_detail ? `<div class="arow-d mono">${esc(r.last_detail)}</div>` : ""}
            ${r.defends_question ? `<div class="arow-q">defends <button class="alink" data-q="${esc(r.defends_question)}">${esc(r.defends_question)}</button></div>` : ""}
          </div>
          ${r.severity === "blocking" ? `<span class="tagb">blocking</span>` : `<span class="taga">advisory</span>`}
        </div>`).join("")}`;
  } catch (err) {
    el.innerHTML = `<p class="afail">${esc(err.message)}</p>`;
  }
}

// ---------- 2. the witness board ----------
export function viewWitnesses() {
  if (!witnesses) return `<div class="card" style="padding:24px">Loading…</div>`;
  const counts = witnesses.reduce((a, w) => ((a[w.verdict] = (a[w.verdict] || 0) + 1), a), {});
  const q = wfilter.toLowerCase();
  const shown = witnesses.filter(
    (w) => !q || `${w.table}.${w.col}`.toLowerCase().includes(q) || w.verdict.includes(q));

  const groups = new Map();
  for (const w of shown) {
    if (!groups.has(w.table)) groups.set(w.table, []);
    groups.get(w.table).push(w);
  }

  return `
  <div class="atiles">
    ${statTile("witness columns", witnesses.length)}
    ${statTile("discriminate", counts.discriminates || 0, "pass")}
    ${statTile("always true", counts["always true"] || 0, "warn")}
    ${statTile("never true", counts["never true"] || 0, "warn")}
    ${statTile("no rows", counts.empty || 0, "pend")}
  </div>
  <p class="muted asub">A witness that reads the same value on every row states nothing
  about the procedures — it looks like a working column and is not evidence. These are
  reported, not hidden, and they are not failures: several are legitimately uniform
  (every seeded requirement really is blocking).</p>

  <div class="card asect">
    <div class="afilters"><input id="wq" class="ainput" placeholder="Filter witnesses…" value="${esc(wfilter)}"></div>
    ${[...groups.entries()].map(([table, ws]) => `
      <div class="agroup">
        <h4 class="agh mono">${esc(table)} <span class="muted">${ws.length}</span></h4>
        <table class="atable">
          <tbody>${ws.map((w) => `
            <tr>
              <td class="mono"><button class="alink" data-trace="${esc(w.table)}.${esc(w.col)}">${esc(w.col)}</button></td>
              <td class="n mono" style="width:120px">${w.t} / ${w.total}</td>
              <td style="width:180px"><div class="bar"><span class="${VERDICT_CLASS[w.verdict]}" style="width:${w.total ? Math.round((w.t / w.total) * 100) : 0}%"></span></div></td>
              <td style="width:130px"><span class="pill ${VERDICT_CLASS[w.verdict]}">${esc(w.verdict)}</span></td>
              <td>${w.question ? `<button class="alink small" data-q="${esc(w.question)}">${esc(w.question)}</button>` : `<span class="muted small">pre-existing</span>`}</td>
            </tr>`).join("")}
          </tbody>
        </table>
      </div>`).join("")}
  </div>`;
}

// ---------- 3. loops and questions ----------
export function viewLoops() {
  if (!loopData) return `<div class="card" style="padding:24px">Loading…</div>`;
  const roleLabel = Object.fromEntries(loopData.roles.map((r) => [r.role_id, r.label || r.role_id]));

  return `
  <div class="atiles">
    ${statTile("loops", loopData.loops.length)}
    ${statTile("questions", loopData.questions.length)}
    ${statTile("roles asking", new Set(loopData.questions.map((q) => q.asking_role)).size)}
    ${statTile("predicates invented", loopData.questions.reduce((a, q) => a + q.predicateCount, 0))}
  </div>

  ${loopData.loops.map((l) => {
    const qs = loopData.questions.filter((q) => q.witness_loop === l.witness_loop_id);
    const byRole = new Map();
    for (const q of qs) {
      if (!byRole.has(q.asking_role)) byRole.set(q.asking_role, []);
      byRole.get(q.asking_role).push(q);
    }
    return `
    <div class="card asect">
      <h3 class="ah">${esc(l.name)}</h3>
      <p class="prose aprem">${esc(l.premise)}</p>
      <div class="atiles small">
        ${statTile("questions", l.question_count)}
        ${statTile("fields after", l.fields_after ?? "—")}
        ${statTile("witnessed after", l.witnessed_after ?? "—")}
      </div>
      ${[...byRole.entries()].map(([rid, rqs]) => `
        <div class="agroup">
          <h4 class="agh">${esc(roleLabel[rid] || rid)} <span class="muted mono">${esc(rid)}</span></h4>
          ${rqs.map((q) => `
            <div class="aq">
              <div class="aq-t">${esc(q.question_text)}</div>
              ${q.why_it_matters ? `<div class="aq-w">${esc(q.why_it_matters)}</div>` : ""}
              <div class="aq-m">
                <span class="tagb">${q.predicateCount} predicate${q.predicateCount === 1 ? "" : "s"}</span>
                ${q.answerable_before ? `<span class="taga">answerable before</span>` : `<span class="taga">newly askable</span>`}
              </div>
              ${q.witnessed_answer ? `<div class="aq-a mono">${esc(q.witnessed_answer)}</div>` : ""}
            </div>`).join("")}
        </div>`).join("")}
    </div>`;
  }).join("")}`;
}

// ---------- 4. provenance ----------
let traceTarget = "";
let traceData = null;
let traceErr = "";

export function viewTrace() {
  return `
  <div class="card asect">
    <h3 class="ah">Trace a field to the question that motivated it</h3>
    <p class="muted asub">Every derived field in this model exists because a named role
    asked something. Enter <span class="mono">Table.Field</span> to walk the chain back.</p>
    <div class="afilters">
      <input id="trq" class="ainput" style="flex:1" placeholder="StepExecutions.ViolatesSeparationOfDuties"
             value="${esc(traceTarget)}">
      <button class="abtn" id="trgo">Trace</button>
    </div>
    ${traceErr ? `<p class="afail">${esc(traceErr)}</p>` : ""}
    ${traceData ? renderTrace(traceData) : ""}
  </div>`;
}

function renderTrace(d) {
  const f = d.field;
  return `
  <div class="achain">
    <div class="anode">
      <div class="anode-k">Field</div>
      <div class="anode-t mono">${esc(f.rulebook_field_id)}</div>
      <div class="anode-b">
        <span class="taga">${esc(f.field_type)}</span>
        <span class="taga">${esc(f.datatype)}</span>
        ${f.is_witness ? `<span class="tagb">witness</span>` : `<span class="taga">pre-existing</span>`}
      </div>
      ${f.formula ? `<div class="aformula mono">${esc(f.formula)}</div>` : ""}
      ${d.reading ? `<div class="anode-r">computed on ${d.reading.populated} of ${d.reading.total} rows</div>` : ""}
    </div>
    ${d.question ? `
    <div class="aarrow">motivated by</div>
    <div class="anode">
      <div class="anode-k">Question</div>
      <div class="anode-t">${esc(d.question.question_text)}</div>
      ${d.question.why_it_matters ? `<div class="anode-w">${esc(d.question.why_it_matters)}</div>` : ""}
      ${d.question.witnessed_answer ? `<div class="aformula mono">${esc(d.question.witnessed_answer)}</div>` : ""}
    </div>
    <div class="aarrow">asked by</div>
    <div class="anode">
      <div class="anode-k">Role</div>
      <div class="anode-t">${esc(d.role?.label || d.question.asking_role)}</div>
      ${d.role?.responsibility ? `<div class="anode-w">${esc(d.role.responsibility)}</div>` : ""}
    </div>
    <div class="aarrow">in</div>
    <div class="anode">
      <div class="anode-k">Loop</div>
      <div class="anode-t">${esc(d.loop?.name || d.question.witness_loop)}</div>
      ${d.loop?.premise ? `<div class="anode-w">${esc(d.loop.premise)}</div>` : ""}
    </div>
    ${d.siblings?.length > 1 ? `
    <div class="aarrow">alongside</div>
    <div class="anode">
      <div class="anode-k">${d.siblings.length - 1} sibling predicate${d.siblings.length === 2 ? "" : "s"} for the same question</div>
      <div class="asibs">${d.siblings
        .filter((s) => s.rulebook_field_id !== f.rulebook_field_id)
        .map((s) => `<button class="alink small mono" data-trace="${esc(s.target_table)}.${esc(s.field_name)}">${esc(s.target_table)}.${esc(s.field_name)}</button>`)
        .join("")}</div>
    </div>` : ""}` : `
    <div class="aarrow">provenance</div>
    <div class="anode">
      <div class="anode-t">This field predates the witness loops.</div>
      <div class="anode-w">It carries no motivating question, because inventing one
      retroactively would be fabrication.</div>
    </div>`}
  </div>`;
}

async function doTrace(target) {
  traceTarget = target;
  traceErr = "";
  traceData = null;
  const [table, field] = target.split(".");
  if (!table || !field) {
    traceErr = "Enter it as Table.Field, e.g. StepExecutions.IsLate";
    return;
  }
  try {
    traceData = await api(`/api/admin/provenance/${encodeURIComponent(table)}/${encodeURIComponent(field)}`);
  } catch (err) {
    traceErr = err.message;
  }
}

// ---------- wiring ----------
export function wireAdmin(tab, rerender) {
  if (tab === "board") {
    refreshChecks();
    const run = document.getElementById("runsuite");
    if (run) run.onclick = async () => {
      running = true; rerender();
      try {
        await api("/api/admin/tests/run", { method: "POST" });
        await loadAdmin();
      } finally {
        running = false; rerender();
      }
    };
    const q = document.getElementById("tq");
    if (q) q.oninput = (e) => { filters.q = e.target.value; refreshChecks(); };
    const k = document.getElementById("tkind");
    if (k) k.onchange = (e) => { filters.kind = e.target.value; refreshChecks(); };
    const o = document.getElementById("toutcome");
    if (o) o.onchange = (e) => { filters.outcome = e.target.value; refreshChecks(); };
  }

  if (tab === "witnesses") {
    const w = document.getElementById("wq");
    if (w) w.oninput = (e) => {
      wfilter = e.target.value;
      const el = document.getElementById("view");
      const pos = e.target.selectionStart;
      el.innerHTML = viewWitnesses();
      wireAdmin("witnesses", rerender);
      const nw = document.getElementById("wq");
      if (nw) { nw.focus(); nw.setSelectionRange(pos, pos); }
    };
  }

  if (tab === "trace") {
    const go = async () => {
      await doTrace(document.getElementById("trq").value.trim());
      rerender();
    };
    const b = document.getElementById("trgo");
    if (b) b.onclick = go;
    const i = document.getElementById("trq");
    if (i) i.onkeydown = (e) => { if (e.key === "Enter") go(); };
  }

  // Cross-panel links: any element carrying data-trace jumps to the tracer.
  for (const el of document.querySelectorAll("[data-trace]")) {
    el.onclick = async () => {
      await doTrace(el.dataset.trace);
      rerender("trace");
    };
  }
  for (const el of document.querySelectorAll("[data-kind]")) {
    el.onclick = () => { filters.kind = el.dataset.kind; filters.outcome = ""; rerender("board"); };
  }
}
