/**
 * Witness: one cell's value for one row, with the values it came from, down to
 * the stored ones (ground truth).
 *
 * Nothing is evaluated here. Every value is computed by the project's own
 * generated TypeScript (rulebook-to-typescript: erb_runtime.ts + erb_sdk.ts,
 * the engine the conformance suite checks against Postgres), loaded from the
 * folder the host names in init({ typescript }) and run over the rulebook's
 * rows (or the rows the host hands over with setRows). The engine also says
 * which step of a priority rule applied (its own erbIf / erbSwitch are watched
 * while it computes the cell) and which rows an aggregation counted (its own
 * criterionMatches). The React explainer's hand-written evaluator
 * (lib/formulaEval.ts) is exactly what this file does NOT have.
 *
 * erb_runtime.ts and erb_sdk.ts are TypeScript written for Node. ts-strip.js
 * (sucrase, MIT) removes the types and turns the imports into require(), a
 * two-entry module table satisfies those, and the runtime's own dataset loop
 * runs in memory: the method of the site's live rulebook widget
 * (effortlessapi-www, www-rulebook/src/efx/live-rulebook/runtime.ts).
 *
 * No DOM here but the script tag that fetches ts-strip.js on first use.
 */
(function (global) {
  const R = () => global.EffortlessDagResolver;

  // rulebook-to-typescript's own to_snake_case (python/orchestration/shared.py).
  function toSnake(name) {
    return String(name).replace(/([^_])([A-Z][a-z]+)/g, "$1_$2").replace(/([a-z0-9])([A-Z])/g, "$1_$2").toLowerCase();
  }
  const loose = (s) => String(s).replace(/[^A-Za-z0-9]/g, "").toLowerCase();

  // The runtime keeps its dataset machinery to itself; one appended line hands it over.
  const INTERNALS = `
;exports.__erb = {
  Dataset: typeof Dataset !== "undefined" ? Dataset : undefined,
  outputRow: typeof outputRow !== "undefined" ? outputRow : undefined,
  newRow: typeof newRow !== "undefined" ? newRow : undefined,
  WRITERS: typeof WRITERS !== "undefined" ? WRITERS : undefined,
  materializeClosure: typeof materializeClosure !== "undefined" ? materializeClosure : undefined,
  criterionMatches: typeof criterionMatches !== "undefined" ? criterionMatches : undefined,
};`;

  class WitnessError extends Error {}

  let config = { typescript: null, basePath: null };
  let hostRows = null;          // { Table: [row, ...] } from setRows(); null = the rulebook's own rows
  let state = "idle";           // idle | loading | ready | error
  let error = null;
  let pending = null;
  let engine = null;            // { sdk, runtime, internals, specs }
  let data = null;              // { dataset, rows: Map(table -> [{ id, label, row (engine's), out }]) }

  function configure(next) {
    const before = config.typescript;
    config = Object.assign({}, config, next || {});
    if (config.typescript !== before) { state = "idle"; error = null; pending = null; engine = null; data = null; }
  }
  const enabled = () => !!config.typescript;

  // ── loading the engine ──────────────────────────────────────────────────
  function loadStripper() {
    if (global.EffortlessTsStrip) return Promise.resolve(global.EffortlessTsStrip);
    return new Promise((resolve, reject) => {
      const base = (config.basePath || "/rulebook-explainer-dag").replace(/\/$/, "");
      const s = global.document.createElement("script");
      s.src = `${base}/ts-strip.js`;
      s.onload = () => (global.EffortlessTsStrip ? resolve(global.EffortlessTsStrip) : reject(new WitnessError(`${s.src} loaded but holds no type-stripper`)));
      s.onerror = () => reject(new WitnessError(`${s.src} could not be loaded. It is written beside explainer-dag.js; serve the whole rulebook-explainer-dag folder.`));
      global.document.head.appendChild(s);
    });
  }

  async function fetchSource(name) {
    const url = `${String(config.typescript).replace(/\/$/, "")}/${name}`;
    let res;
    try { res = await global.fetch(url); }
    catch (e) { throw new WitnessError(`${url} could not be reached (${e.message}).`); }
    if (!res.ok) {
      throw new WitnessError(`${url} answered ${res.status}. The witness runs the project's generated TypeScript: add rulebook-to-typescript to effortless.json, write its output where the site serves files, and name that folder in init({ typescript }).`);
    }
    const text = await res.text();
    if (/^\s*<(!doctype|html)/i.test(text)) throw new WitnessError(`${url} answered a web page, not ${name}: the folder named in init({ typescript }) does not hold rulebook-to-typescript's output.`);
    return text;
  }

  async function buildEngine() {
    const [strip, runtimeTs, sdkTs] = await Promise.all([loadStripper(), fetchSource("erb_runtime.ts"), fetchSource("erb_sdk.ts")]);
    const sources = {};
    for (const [name, src] of [["erb_runtime", runtimeTs], ["erb_sdk", sdkTs]]) {
      try {
        sources[name] = strip.transform(src, { transforms: ["typescript", "imports"], filePath: `${name}.ts`, disableESTransforms: true }).code + (name === "erb_runtime" ? INTERNALS : "");
      } catch (e) { throw new WitnessError(`${name}.ts could not be read as TypeScript: ${e.message}`); }
    }
    const modules = {};
    // The runtime was written for Node and asks `process.env.FORMULA_NOW` for a fixed
    // "now" (the conformance runs set one). A browser has no `process`: without this,
    // every NOW() / TODAY() failed with "process is not defined". None set = the real now.
    const nodeProcess = { env: {}, stderr: { write() {} }, exit() { throw new Error("the generated runtime asked to exit"); } };
    const stub = (what) => new Proxy({}, { get: (_t, prop) => () => { throw new Error(`${what}.${String(prop)} is not available in the browser`); } });
    const load = (name) => {
      if (modules[name]) return modules[name];
      const module = { exports: {} };
      modules[name] = module.exports;
      const req = (spec) => {
        const m = /^\.\/(erb_runtime|erb_sdk)(?:\.js|\.ts)?$/.exec(spec);
        if (m) return load(m[1]);
        if (spec === "node:fs" || spec === "fs") return stub("fs");
        if (spec === "node:path" || spec === "path") return stub("path");
        throw new WitnessError(`${name}.ts requires ${spec}, which a browser does not have`);
      };
      new Function("require", "exports", "module", "process", sources[name])(req, module.exports, module, nodeProcess);
      return module.exports;
    };
    const sdk = load("erb_sdk");
    const runtime = load("erb_runtime");
    const internals = runtime.__erb || {};
    if (!Array.isArray(sdk.erbTables) || !sdk.erbTables.length) throw new WitnessError("erb_sdk.ts exports no erbTables: it is not rulebook-to-typescript's output (the simpleSyntax form cannot witness; use the full one).");
    if (!internals.Dataset || !internals.outputRow || !internals.newRow || !internals.WRITERS) {
      throw new WitnessError("erb_runtime.ts no longer has the Dataset / outputRow / newRow / WRITERS the witness drives: rulebook-to-typescript changed shape, and witness.js must follow it.");
    }
    return { sdk, runtime, internals, specs: sdk.erbTables };
  }

  // ── the rows ────────────────────────────────────────────────────────────
  function pkField(table) {
    const schema = (R().rulebook()[table] || {}).schema || [];
    const byName = schema.find((f) => /Id$/.test(f.name) && (f.type || "raw") === "raw");
    return (byName || schema[0] || {}).name || null;
  }
  // The engine's (snake_case) name of a rulebook field, by the generator's own rule,
  // then by letters alone for a name the rule and the rulebook spell apart.
  function engineField(spec, field) {
    const snake = toSnake(field);
    if (snake in spec.fields) return snake;
    const want = loose(field);
    return Object.keys(spec.fields).find((k) => loose(k) === want) || null;
  }
  function rulebookField(table, engineName) {
    const schema = (R().rulebook()[table] || {}).schema || [];
    const want = loose(engineName);
    const f = schema.find((x) => toSnake(x.name) === engineName) || schema.find((x) => loose(x.name) === want);
    return f ? f.name : null;
  }
  function inputRows(spec) {
    const given = hostRows && hostRows[spec.name];
    const rows = given || (R().rulebook()[spec.name] || {}).data || [];
    return rows.map((row) => {
      const out = {};
      for (const [k, v] of Object.entries(row || {})) {
        const key = k in spec.fields ? k : engineField(spec, k);
        if (key) out[key] = v;
      }
      return out;
    });
  }

  // erb_runtime.ts's erbRun, over rows in memory: lookups, aggregations and
  // calculations, pass after pass until nothing changes.
  function computeDataset() {
    const { sdk, runtime, internals, specs } = engine;
    const closures = sdk.erbClosures || [];
    const d = new internals.Dataset(specs);
    let derived = 0;
    for (const spec of specs) {
      d.rows.set(spec.file, inputRows(spec).map((m) => {
        const row = internals.newRow(spec);
        for (const [k, v] of Object.entries(m)) row[k] = internals.WRITERS[spec.fields[k]](runtime.fromJSON(v));
        return row;
      }));
      derived += spec.lookups.length + spec.aggregations.length;
    }
    if (closures.length && !internals.materializeClosure) throw new WitnessError("this rulebook needs closures, which this erb_runtime.ts cannot materialize in memory");
    for (const c of closures) d.closures.set(c.view, internals.materializeClosure(d, c));
    const maxPasses = derived + Number(sdk.calculatedFieldCount || 0) + 2;
    let previous = "";
    for (let pass = 1; ; pass++) {
      if (pass > maxPasses) throw new WitnessError(`the rows did not settle within ${maxPasses} passes: the field dependencies are cyclic`);
      d.blank = new Map();
      for (const c of closures) if (c.filter !== null) d.closures.set(c.view, internals.materializeClosure(d, c));
      for (const spec of specs) {
        const rows = d.rows.get(spec.file);
        for (const r of rows) delete r._erb_errors;
        for (const l of spec.lookups) d.computeLookup(l, spec, rows);
        for (const a of spec.aggregations) d.computeAggregation(a, spec, rows);
        for (const r of rows) spec.compute(r);
      }
      const current = specs.map((spec) => JSON.stringify(d.rows.get(spec.file).map((r) => internals.outputRow(r, spec)))).join("");
      if (current === previous) break;
      previous = current;
    }
    const rows = new Map();
    for (const spec of specs) {
      const pk = pkField(spec.name);
      const pkKey = pk && engineField(spec, pk);
      const nameKey = engineField(spec, "Name");
      rows.set(spec.name, d.rows.get(spec.file).map((row, i) => {
        const out = internals.outputRow(row, spec);
        const id = pkKey && out[pkKey] != null ? String(out[pkKey]) : String(i + 1);
        const label = nameKey && out[nameKey] != null && out[nameKey] !== "" ? String(out[nameKey]) : id;
        return { id, label, row, out, errors: row._erb_errors || {} };
      }));
    }
    return { dataset: d, rows };
  }

  function ready() {
    if (!enabled()) return Promise.reject(new WitnessError("no folder of generated TypeScript was named: init({ typescript: \"/rulebook-typescript\" })"));
    if (state === "ready") return Promise.resolve(true);
    if (pending) return pending;
    state = "loading";
    pending = (async () => {
      try {
        if (!engine) engine = await buildEngine();
        data = computeDataset();
        state = "ready"; error = null;
        return true;
      } catch (e) {
        state = "error"; error = e.message || String(e);
        throw e;
      } finally { pending = null; }
    })();
    return pending;
  }

  // Rows from the host (an app's live rows) in place of the rulebook's own:
  // { Table: [row, ...] }, keys as the rulebook or the database spells them.
  // Tables left out keep the rulebook's rows. null goes back to the rulebook's.
  function setRows(tables) {
    hostRows = tables || null;
    data = null;
    if (state === "ready" || state === "error") { state = "idle"; error = null; }
  }

  // ── reading the computed rows ───────────────────────────────────────────
  const specOf = (table) => engine.specs.find((s) => s.name === table) || null;
  function rowsOf(table) {
    if (state !== "ready") return [];
    return (data.rows.get(table) || []).map((r) => ({ id: r.id, label: r.label }));
  }
  function entry(table, rowId) {
    return (data.rows.get(table) || []).find((r) => r.id === String(rowId)) || null;
  }
  function valueOf(table, field, rowId) {
    const spec = specOf(table), e = entry(table, rowId);
    if (!spec || !e) return undefined;
    const key = engineField(spec, field);
    return key ? e.out[key] : undefined;
  }

  // ── which step of a priority rule applied: the engine's own erbIf / erbSwitch ──
  // erbIf(cond, then, otherwise) takes its branches as thunks, so watching which
  // thunk it calls says which branch it took. Calls made inside a branch are that
  // call's children; a ladder IF(c1, v1, IF(c2, v2, d)) is then the LAST call at
  // each level (anything before it at that level was inside its condition).
  function watchBranches(fn) {
    const runtime = engine.runtime;
    const origIf = runtime.erbIf, origSwitch = runtime.erbSwitch;
    const root = { children: [] };
    let cur = root;
    const inside = (node, thunk, mark) => () => {
      mark();
      const prev = cur; cur = node;
      try { return thunk(); } finally { cur = prev; }
    };
    if (typeof origIf === "function") {
      runtime.erbIf = function (cond, then, otherwise) {
        const node = { kind: "if", taken: null, children: [] };
        cur.children.push(node);
        return origIf(cond, inside(node, then, () => { node.taken = "then"; }), inside(node, otherwise, () => { node.taken = "else"; }));
      };
    }
    if (typeof origSwitch === "function") {
      runtime.erbSwitch = function (subject, pairs, otherwise) {
        const node = { kind: "switch", taken: null, children: [] };
        cur.children.push(node);
        const watched = (pairs || []).map(([pattern, result], i) => [pattern, inside(node, result, () => { node.taken = i; })]);
        return origSwitch(subject, watched, inside(node, otherwise, () => { node.taken = "else"; }));
      };
    }
    try { fn(); } catch { /* the value (and its error) is the dataset's; this run only watches */ }
    finally { runtime.erbIf = origIf; runtime.erbSwitch = origSwitch; }
    return root;
  }

  // The formula's ladder, from its parse: how many steps, and whether each ELSE is another IF.
  function ladderOf(formula) {
    const ast = R().tryParseFormula(formula);
    if (!ast || ast.kind !== "call") return null;
    const fn = String(ast.fn).toUpperCase();
    if (fn === "SWITCH") {
      const rest = ast.args.length - 1;
      const pairs = Math.floor(rest / 2);
      return { kind: "switch", steps: pairs + (rest % 2 === 1 ? 1 : 0), pairs, hasElse: rest % 2 === 1 };
    }
    if (fn !== "IF") return null;
    let ifs = 0, node = ast, hasElse = false;
    while (node && node.kind === "call" && String(node.fn).toUpperCase() === "IF") {
      ifs++;
      hasElse = node.args.length > 2;
      node = node.args[2];
    }
    return { kind: "if", steps: ifs + (hasElse ? 1 : 0), ifs, hasElse };
  }

  // { index (0-based step that applied, or null when none did), steps } or null when
  // the field is not a priority rule or the engine's calls do not have its shape.
  function firedStep(table, field, rowId) {
    if (state !== "ready") return null;
    const f = R().fieldOf(R().rulebook(), table, field);
    const e = entry(table, rowId);
    if (!f || !f.formula || !e) return null;
    const ladder = ladderOf(f.formula);
    const calc = engine.sdk[`calc${table}${field}`];
    if (!ladder || typeof calc !== "function") return null;
    const root = watchBranches(() => calc(e.row));
    if (ladder.kind === "switch") {
      const call = root.children.filter((c) => c.kind === "switch").pop();
      if (!call) return null;
      if (call.taken === "else" || call.taken === null) return { index: ladder.hasElse ? ladder.pairs : null, steps: ladder.steps };
      return { index: call.taken, steps: ladder.steps };
    }
    let level = root.children, step = 0;
    for (;;) {
      const call = level.filter((c) => c.kind === "if").pop();
      if (!call || call.taken === null) return null;
      if (call.taken === "then") return { index: step, steps: ladder.steps };
      step++;
      if (step >= ladder.ifs) return { index: ladder.hasElse ? step : null, steps: ladder.steps };
      level = call.children;
    }
  }

  // ── the witness of one cell ─────────────────────────────────────────────
  // A node says what the cell holds and names what it was made from (children),
  // each of which is a cell to witness in turn; a raw value has none.
  function matchingRows(agg, ownerSpec, record) {
    const d = data.dataset;
    if (agg.op === "COMPOSITE") {
      const seen = new Map();
      for (const part of agg.parts || []) for (const m of matchingRows(part, ownerSpec, record) || []) seen.set(`${m.table}\u0000${m.id}`, m);
      return Array.from(seen.values());
    }
    const targetSpec = d.byFile.get(agg.table);
    if (!targetSpec || d.closures.has(agg.table) || !engine.internals.criterionMatches) return null;
    const table = d.tableRows(agg.table);
    const ownerGet = d.accessor(ownerSpec);
    const all = data.rows.get(targetSpec.name) || [];
    const out = [];
    table.rows.forEach((row, i) => {
      const ok = (agg.criteria || []).every((c) => engine.internals.criterionMatches(c, table.get(row, c.range), () => ownerGet(record, c.field)));
      if (ok && all[i]) out.push({ table: targetSpec.name, id: all[i].id, label: all[i].label, field: agg.target ? rulebookField(targetSpec.name, agg.target) : null });
    });
    return out;
  }

  function witness(table, field, rowId) {
    if (state !== "ready") return null;
    const f = R().fieldOf(R().rulebook(), table, field);
    const spec = specOf(table), e = entry(table, rowId);
    if (!f || !spec) return { table, field, rowId, missing: `the generated TypeScript has no ${table}.${field}: it was built from another version of the rulebook. Run the build again.` };
    if (!e) return { table, field, rowId, missing: `${table} has no row ${JSON.stringify(String(rowId))}` };
    const key = engineField(spec, field);
    if (!key) return { table, field, rowId, missing: `the generated TypeScript has no ${table}.${field}: it was built from another version of the rulebook. Run the build again.` };
    const type = f.type || "raw";
    const node = {
      table, field, rowId: e.id, rowLabel: e.label, type, datatype: f.datatype || "string",
      value: e.out[key], error: e.errors[key] || null, children: [],
    };
    const cell = (t, fl, id, extra) => Object.assign({ table: t, field: fl, rowId: id }, extra || {});

    if (type === "raw") { node.groundTruth = true; return node; }

    if (type === "relationship") {
      node.groundTruth = true;
      const targetTable = f.RelatedTo || f.relatedTo || null;
      const target = targetTable && node.value != null ? entry(targetTable, node.value) : null;
      if (targetTable) node.pointsTo = { table: targetTable, id: node.value, label: target ? target.label : null };
      return node;
    }

    const lookup = spec.lookups.find((l) => l.field === key);
    if (lookup && !lookup.error) {
      const targetSpec = data.dataset.byFile.get(lookup.target);
      const keyField = rulebookField(table, lookup.key);
      const keyValue = e.out[lookup.key];
      const target = targetSpec && keyValue != null
        ? (data.rows.get(targetSpec.name) || []).find((r) => r.out[lookup.match] != null && String(r.out[lookup.match]) === String(keyValue))
        : null;
      node.via = { keyField, keyValue, table: targetSpec ? targetSpec.name : null, id: target ? target.id : null, label: target ? target.label : null };
      if (keyField) node.children.push(cell(table, keyField, e.id, { role: "key" }));
      if (target) {
        const ret = rulebookField(targetSpec.name, lookup.ret);
        if (ret) node.children.push(cell(targetSpec.name, ret, target.id, { role: "looked-up" }));
      }
      return node;
    }

    const agg = spec.aggregations.find((a) => a.field === key);
    if (agg && !agg.error) {
      const rows = matchingRows(agg, spec, e.row);
      node.over = { op: agg.op || null, rows: rows ? rows.length : null, table: rows && rows[0] ? rows[0].table : (data.dataset.byFile.get(agg.table) || {}).name || null };
      // The fields of THIS row the criteria compare against (the row's own key, usually).
      const own = new Set();
      const collect = (a) => { for (const c of a.criteria || []) if (c.kind === "field" && c.field) own.add(c.field); for (const p of a.parts || []) collect(p); };
      collect(agg);
      for (const k of own) { const name = rulebookField(table, k); if (name) node.children.push(cell(table, name, e.id, { role: "key" })); }
      node.counted = rows || null;   // [{ table, id, label, field }] : each a row, and the cell of it that was read (field null = the row itself)
      return node;
    }

    // calculated (and anything the engine computes row by row): the fields its formula names, in this row.
    const refs = (f.formula ? R().extractFormulaRefs(f.formula, table) : []).filter((r) => r.table === table);
    for (const r of refs) if (R().fieldOf(R().rulebook(), r.table, r.field)) node.children.push(cell(r.table, r.field, e.id));
    node.step = firedStep(table, field, e.id);
    return node;
  }

  global.EffortlessWitness = {
    configure, enabled, ready, setRows,
    state: () => state,
    error: () => error,
    rowsOf, valueOf, witness, firedStep, pkField,
  };
})(typeof window !== "undefined" ? window : globalThis);
