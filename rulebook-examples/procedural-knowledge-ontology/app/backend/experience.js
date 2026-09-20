// ---------------------------------------------------------------------------
// The role experiences: reads, ACTIONS, the Copilot, and the story controls.
//
// Reads    SELECT * FROM <the caller's own schema>.<table>, with equality
//          filters. The schema is the caller's whole world; a table or column
//          outside it does not exist for them. Nothing is filtered here.
//
// Actions  There is no endpoint per action. An action is a vw_app_actions row
//          plus its vw_app_action_fields rows: the base table, the operation,
//          and where each column's value comes from (typed, picked, fixed by
//          the action, or stamped by the server). This file executes those rows
//          AS the caller. Whether the write is allowed is decided by Postgres:
//          the RLS policy opens the row, the column grant opens the columns.
//          A refusal comes back as the database's own error.
//
// Time     Writes are stamped from the modelled instant (EvaluationContexts),
//          never the wall clock. Every Days... column in the model is judged
//          against that instant; a wall-clock stamp would land months after it.
// ---------------------------------------------------------------------------
import { spawn } from "node:child_process";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { adminQuery, asPrincipal } from "./db.js";
import { mintClaimsFor } from "./auth.js";

const HERE = path.dirname(fileURLToPath(import.meta.url));
const ROOT = path.resolve(HERE, "../..");
const IRI = "https://effortlessapi.github.io/effortless-rulebooks/ns/pko-extension#";

const snake = (n) => n.replace(/(?<!^)(?=[A-Z])/g, "_").toLowerCase().replace(/_+/g, "_");
const ident = (s) => {
  if (!/^[a-z][a-z0-9_]*$/.test(s)) {
    const e = new Error(`bad identifier: ${s}`); e.status = 400; throw e;
  }
  return s;
};
const fail = (status, error, detail) => Object.assign(new Error(detail || error), { status, code: error });

// Minutes added to the modelled instant so rows written in one sitting keep
// their order. Reset with the story.
let tick = 0;
async function stampInstant() {
  tick += 1;
  const { rows } = await adminQuery(
    `SELECT (as_of_instant + make_interval(mins => $1))::timestamptz AS at
       FROM vw_evaluation_contexts WHERE is_current`, [tick]);
  if (rows.length !== 1) throw fail(500, "no_current_context", "expected exactly one current EvaluationContexts row");
  return rows[0].at;
}

const PREFIX = {
  ProcedureExecutions: "exec", StepExecutions: "se", CueObservations: "obs",
  AssistantAnswers: "ans", KnowledgeRepositoryEntries: "kre", KnowledgeTransfers: "kt",
  StepCues: "cue", KnowledgeDeliverables: "kd",
};
const newId = (table, agent) =>
  `${PREFIX[table] || snake(table)}-app-${agent}-${Date.now().toString(36)}`;

async function loadAction(actionId) {
  const [{ rows: a }, { rows: f }] = await Promise.all([
    adminQuery(`SELECT a.*, t.physical_table, t.physical_view
                  FROM vw_app_actions a JOIN vw_rulebook_tables t ON t.rulebook_table_id = a.target_table
                 WHERE a.app_action_id = $1`, [actionId]),
    adminQuery(`SELECT af.*, rf.field_name, rf.datatype
                  FROM vw_app_action_fields af JOIN vw_rulebook_fields rf ON rf.rulebook_field_id = af.target_field
                 WHERE af.app_action = $1 ORDER BY af.sort_order`, [actionId]),
  ]);
  if (a.length !== 1) throw fail(404, "no_such_action", actionId);
  if (!a[0].physical_table) throw fail(500, "no_physical_table", `${a[0].target_table} has no PhysicalTable`);
  return { action: a[0], fields: f };
}

async function primaryKey(physicalTable) {
  const { rows } = await adminQuery(
    `SELECT column_name FROM information_schema.columns
      WHERE table_schema='public' AND table_name=$1 AND ordinal_position=1`, [physicalTable]);
  if (!rows.length) throw fail(500, "no_such_table", physicalTable);
  return rows[0].column_name;
}

// What the server stamps. Each is named in AppActionFields.FixedValue.
async function serverValue(what, ctx) {
  const { claims, action, body, client } = ctx;
  switch (what) {
    case "id": return newId(action.target_table, claims.agent);
    case "agent": return claims.agent;
    case "instant": return ctx.instant;
    case "iri": return IRI + action.target_table.replace(/ies$/, "y").replace(/s$/, "");
    case "title": {
      const { rows } = await client.query(`SELECT name FROM machines WHERE machine_id = $1`,
        [body.values?.ExecutedOnMachine]);
      if (!rows.length) throw fail(400, "unknown_machine", String(body.values?.ExecutedOnMachine));
      return `Lockout of ${rows[0].name.toLowerCase()}, ${String(body.values?.Shift || "").toLowerCase()} shift`;
    }
    case "previous": {
      const { rows } = await client.query(
        `SELECT step_execution_id FROM step_executions WHERE procedure_execution = $1
          ORDER BY started_at DESC, step_execution_id DESC LIMIT 1`, [body.context?.ProcedureExecution]);
      return rows[0]?.step_execution_id ?? null;
    }
    case "cue-escalation-holder": {
      // The person holding the role this warning sign says it must go to.
      const { rows } = await adminQuery(
        `SELECT r.current_agent, c.escalate_to_role
           FROM vw_cue_observations o JOIN vw_step_cues c ON c.step_cue_id = o.step_cue
           LEFT JOIN vw_roles r ON r.role_id = c.escalate_to_role
          WHERE o.cue_observation_id = $1`, [body.key]);
      if (!rows.length) throw fail(404, "no_such_observation", body.key);
      if (!rows[0].current_agent) {
        throw fail(409, "escalation_role_vacant",
          `${rows[0].escalate_to_role || "(no role)"} has no current holder to escalate to`);
      }
      return rows[0].current_agent;
    }
    case "answer": return ctx.answer?.[ctx.fieldName] ?? null;
    default: throw fail(500, "unknown_server_stamp", what);
  }
}

function coerce(v, datatype) {
  if (v === "" || v === undefined) return null;
  if (datatype === "boolean") return v === true || v === "true";
  return v;
}

/** Execute one AppActions row as the caller. Returns the watched value before and after. */
export async function runAction(claims, actionId, body, extra = {}) {
  const { action, fields } = await loadAction(actionId);
  if (action.owning_role !== claims.role) {
    // A courtesy, so the refusal names the reason. The database would refuse anyway.
    throw fail(403, "not_your_action", `${action.label} belongs to ${action.owning_role}, you are ${claims.role}`);
  }
  const pk = await primaryKey(action.physical_table);
  const instant = await stampInstant();
  const watch = body.watch && action.watched_field ? await watchSpec(action.watched_field, body.watch) : null;

  return asPrincipal(claims, async (client) => {
    const before = watch ? await readWatched(client, watch) : null;
    const ctx = { claims, action, body, client, instant, answer: extra.answer };
    const cols = [], vals = [];
    for (const f of fields) {
      const col = snake(f.field_name);
      let v;
      if (f.input_kind === "fixed") v = f.fixed_value;
      else if (f.input_kind === "server") v = await serverValue(f.fixed_value, { ...ctx, fieldName: f.field_name });
      else if (f.input_kind === "context") {
        v = body.context?.[f.field_name];
        if (v == null || v === "") throw fail(400, "missing_context", `${action.label} needs ${f.field_name}`);
      } else v = body.values?.[f.field_name];
      cols.push(ident(col)); vals.push(coerce(v, f.datatype));
    }

    let key = body.key;
    if (action.operation === "INSERT") {
      key = vals[cols.indexOf(pk)];
      // No RETURNING: it would apply the SELECT policy to a row the derived
      // organization lookup cannot see yet, and refuse a legitimate insert.
      await client.query(
        `INSERT INTO public.${ident(action.physical_table)} (${cols.join(", ")})
         VALUES (${cols.map((_, i) => `$${i + 1}`).join(", ")})`, vals);
    } else {
      if (!key) throw fail(400, "missing_key", `${action.label} updates a row; say which`);
      const r = await client.query(
        `UPDATE public.${ident(action.physical_table)}
            SET ${cols.map((c, i) => `${c} = $${i + 1}`).join(", ")}
          WHERE ${ident(pk)} = $${cols.length + 1}`, [...vals, key]);
      if (r.rowCount === 0) {
        throw fail(403, "refused_by_row_policy",
          `No row ${key} that ${claims.principal} may ${action.label.toLowerCase()}. The row policy decides this, not the app.`);
      }
    }
    const after = watch ? await readWatched(client, watch) : null;
    return { action: action.app_action_id, label: action.label, table: action.target_table, key, instant,
             watched: watch ? { ...watch.public, before, after, changed: before !== after } : null };
  });
}

async function watchSpec(watchedField, watchKey) {
  const [table, field] = watchedField.split(".");
  const { rows } = await adminQuery(
    `SELECT physical_table FROM vw_rulebook_tables WHERE rulebook_table_id = $1`, [table]);
  if (!rows[0]?.physical_table) throw fail(500, "no_physical_table", table);
  const pk = await primaryKey(rows[0].physical_table);
  return { view: rows[0].physical_table, column: snake(field), pk, key: watchKey,
           public: { table, field, key: watchKey } };
}
async function readWatched(client, w) {
  // Through the caller's own schema: the watched value is one they can see.
  const { rows } = await client.query(
    `SELECT ${ident(w.column)} AS v FROM ${ident(w.view)} WHERE ${ident(w.pk)} = $1`, [w.key]);
  return rows.length ? rows[0].v : null;
}

// ---------------------------------------------------------------------------
// The Plant Copilot. No language model. It answers AS its own principal, from
// its own narrow schema, by structured query, and returns the rows it used.
// ---------------------------------------------------------------------------
const COPILOT_USER = "user-plant-copilot-1-2";
const COPILOT_PRINCIPAL = "principal-plant-assistant";

async function copilot(fn) {
  const claims = await mintClaimsFor(COPILOT_USER, COPILOT_PRINCIPAL);
  return asPrincipal(claims, fn);
}
const stepLabel = (s) => `step ${s.step_number}, ${s.title}`;

/** The questions the book can answer about one step execution (or a bare step). */
export async function copilotQuestions({ stepId, stepExecutionId }) {
  return copilot(async (c) => {
    const { rows: cues } = await c.query(
      `SELECT step_cue_id, operator_question FROM step_cues
        WHERE step = $1 AND operator_question <> '' ORDER BY step_cue_id`, [stepId]);
    return [
      { id: "next", text: "What comes next?" },
      ...cues.map((q) => ({ id: `cue:${q.step_cue_id}`, text: q.operator_question })),
      { id: "energy", text: "Which energy sources does this machine have?" },
    ].map((q) => ({ ...q, stepId, stepExecutionId }));
  });
}

export async function copilotAnswer({ questionId, stepId, stepExecutionId, machineId }) {
  return copilot(async (c) => {
    const one = async (sql, p) => (await c.query(sql, p)).rows[0];
    const step = await one(`SELECT step_id, step_number, title, procedure_version FROM steps WHERE step_id = $1`, [stepId]);
    if (!step) throw fail(404, "step_not_visible_to_copilot", stepId);
    const se = stepExecutionId
      ? await one(`SELECT step_execution_id, is_blocked_by_observed_cue FROM step_executions WHERE step_execution_id = $1`, [stepExecutionId])
      : null;
    const { rows: outs } = await c.query(
      `SELECT t.step_transition_id, t.transition_kind, t.condition, s.step_id, s.step_number, s.title
         FROM step_transitions t JOIN steps s ON s.step_id = t.to_step
        WHERE t.from_step = $1 ORDER BY t.priority`, [stepId]);
    const fallback = outs.find((t) => t.transition_kind === "Fallback");
    const normal = outs.find((t) => t.transition_kind === "Next");
    const g = (table, key, why) => ({ table, key, why });

    if (questionId === "next") {
      if (se?.is_blocked_by_observed_cue) {
        if (!fallback) throw fail(409, "blocked_with_no_fallback", stepId);
        return { question: "What comes next?", topic: "Sequence", safety: true, next: fallback.step_id,
          text: `Not ${normal ? stepLabel(normal) : "the next step"}. A warning sign recorded on this step means it is not finished. The only way forward is ${stepLabel(fallback)}.`,
          groundings: [g("StepExecutions", se.step_execution_id, "a warning sign was recorded on this step run"),
                       g("StepTransitions", fallback.step_transition_id, `the fallback path: ${fallback.condition}`)] };
      }
      if (!normal) throw fail(409, "no_next_step", stepId);
      return { question: "What comes next?", topic: "Sequence", safety: false, next: normal.step_id,
        text: `Next is ${stepLabel(normal)}.`,
        groundings: [g("StepTransitions", normal.step_transition_id, `the normal path: ${normal.condition}`)] };
    }

    if (questionId.startsWith("cue:")) {
      const cue = await one(
        `SELECT step_cue_id, description, operator_question, signals_incomplete_step, requires_escalation,
                escalate_to_role, signals_failure_mode, failure_mode_response
           FROM step_cues WHERE step_cue_id = $1 AND step = $2`, [questionId.slice(4), stepId]);
      if (!cue) throw fail(404, "no_such_warning_sign", questionId);
      if (!cue.signals_incomplete_step) {
        return { question: cue.operator_question, topic: "Verification", safety: false, next: normal?.step_id ?? null,
          text: `Yes. "${cue.description}" is what this step expects to see.`,
          groundings: [g("StepCues", cue.step_cue_id, "recorded as a sign the step went as intended")] };
      }
      if (!cue.failure_mode_response) {
        return { question: cue.operator_question, topic: "Safety", safety: true, next: fallback?.step_id ?? null,
          text: `That means this step is not finished, and the book records no response for it. Stop, keep every lock on, and ask the safety officer.`,
          groundings: [g("StepCues", cue.step_cue_id, "a sign the step is not finished, linked to no failure mode")] };
      }
      return { question: cue.operator_question, topic: "Safety", safety: true, next: fallback?.step_id ?? null,
        text: `No. This step is not finished. ${cue.failure_mode_response}` +
              (fallback ? ` The way forward is ${stepLabel(fallback)}.` : ""),
        groundings: [g("StepCues", cue.step_cue_id, cue.description),
                     g("FailureModes", cue.signals_failure_mode, "the recorded response to what this sign means"),
                     ...(fallback ? [g("StepTransitions", fallback.step_transition_id, `the fallback path: ${fallback.condition}`)] : [])] };
    }

    if (questionId === "energy") {
      if (!machineId) throw fail(400, "missing_machine", "which machine?");
      const { rows } = await c.query(
        `SELECT machine_energy_source_id, energy_source FROM machine_energy_sources WHERE machine = $1 ORDER BY 1`, [machineId]);
      if (!rows.length) throw fail(404, "no_energy_sources_on_record", machineId);
      const names = rows.map((r) => r.energy_source.replace(/Energy$/, "").toLowerCase());
      return { question: "Which energy sources does this machine have?", topic: "Isolation", safety: false, next: null,
        text: `${rows.length} on record: ${names.join(", ")}. Each one is isolated and locked separately.`,
        groundings: rows.map((r) => g("MachineEnergySources", r.machine_energy_source_id, r.energy_source)) };
    }
    throw fail(400, "unknown_question", questionId);
  });
}

// ---------------------------------------------------------------------------
export function mountExperience(app, { requireAuth, requireAdmin, h }) {
  const wrap = (fn) => h(async (req, res) => {
    try { await fn(req, res); } catch (e) {
      if (e.status) return res.status(e.status).json({ error: e.code || "error", detail: e.message });
      // Postgres refusals are the security model speaking. Pass them through verbatim.
      if (e.code === "42501") return res.status(403).json({ error: "refused_by_database", detail: e.message });
      throw e;
    }
  });

  // Who am I here: my profile, my actions, the instant, the tables I can see.
  app.get("/api/app/shell", requireAuth, wrap(async (req, res) => {
    const [profile, actions, fields, ctx] = await Promise.all([
      adminQuery(`SELECT * FROM vw_app_role_profiles WHERE role = $1`, [req.claims.role]),
      adminQuery(`SELECT * FROM vw_app_actions WHERE owning_role = $1 ORDER BY app_action_id`, [req.claims.role]),
      adminQuery(`SELECT af.*, rf.field_name, rf.datatype FROM vw_app_action_fields af
                    JOIN vw_app_actions a ON a.app_action_id = af.app_action
                    JOIN vw_rulebook_fields rf ON rf.rulebook_field_id = af.target_field
                   WHERE a.owning_role = $1 ORDER BY af.app_action, af.sort_order`, [req.claims.role]),
      adminQuery(`SELECT evaluation_context_id, label, as_of_instant FROM vw_evaluation_contexts WHERE is_current`),
    ]);
    const tables = await asPrincipal(req.claims, async (c, p) =>
      (await c.query(`SELECT table_name FROM information_schema.views WHERE table_schema = $1 ORDER BY 1`,
        [p.schema_name])).rows.map((r) => r.table_name));
    const { icon_png_base64: _drop, ...prof } = profile.rows[0] || {};
    res.json({ claims: req.claims, profile: profile.rows[0] ? prof : null,
      actions: actions.rows.map((a) => ({ ...a, fields: fields.rows.filter((f) => f.app_action === a.app_action_id) })),
      context: ctx.rows[0], tables });
  }));

  // Rows from MY schema, with equality filters (?col=value) and ?order=col[,col].
  app.get("/api/app/rows/:table", requireAuth, wrap(async (req, res) => {
    const t = ident(String(req.params.table));
    const { order, limit, ...filters } = req.query;
    const cols = Object.keys(filters).map(ident);
    const orderBy = order ? String(order).split(",").map((c) =>
      c.startsWith("-") ? `${ident(c.slice(1))} DESC` : ident(c)).join(", ") : null;
    try {
      const rows = await asPrincipal(req.claims, async (c, p) => (await c.query(
        `SELECT * FROM ${p.schema_name}.${t}` +
        (cols.length ? ` WHERE ${cols.map((c2, i) => `${c2}::text = $${i + 1}`).join(" AND ")}` : "") +
        (orderBy ? ` ORDER BY ${orderBy}` : "") + ` LIMIT ${Math.min(Number(limit) || 1000, 5000)}`,
        cols.map((k) => String(filters[k])))).rows);
      // Role views list their columns alphabetically, so the key is named, not assumed to be first.
      res.json({ table: t, pk: await primaryKey(t), count: rows.length, rows });
    } catch (e) {
      if (/does not exist/i.test(e.message)) {
        return res.status(404).json({ error: "not_in_your_schema", detail: e.message });
      }
      throw e;
    }
  }));

  app.post("/api/app/action/:actionId", requireAuth, wrap(async (req, res) => {
    res.json(await runAction(req.claims, String(req.params.actionId), req.body || {}));
  }));

  app.get("/api/app/copilot/questions", requireAuth, wrap(async (req, res) => {
    res.json(await copilotQuestions({ stepId: String(req.query.step || ""), stepExecutionId: req.query.stepExecution || null }));
  }));
  // Ask: the Copilot answers as itself; the answer is then recorded against the asker.
  app.post("/api/app/copilot/ask", requireAuth, wrap(async (req, res) => {
    const b = req.body || {};
    const a = await copilotAnswer(b);
    let recorded = null;
    if (b.stepExecutionId) {
      recorded = await runAction(req.claims, "act-tech-ask", { context: { StepExecution: b.stepExecutionId } }, {
        answer: { QuestionTopic: a.topic, QuestionText: a.question, AnswerText: a.text,
                  AssumedCurrentStep: b.stepId, AssertedNextStep: a.next, RaisedSafetyConcern: a.safety },
      });
    }
    res.json({ ...a, answeredBy: "plant-copilot-1-2", retrievalMode: "StructuredQuery", recorded });
  }));

  // ---- the story: reset it, or save it into the book --------------------------------
  const stream = (res, cmd, args, after) => {
    res.writeHead(200, { "Content-Type": "text/event-stream", "Cache-Control": "no-cache", Connection: "keep-alive" });
    const send = (event, data) => res.write(`event: ${event}\ndata: ${JSON.stringify(data)}\n\n`);
    const child = spawn(cmd, args, { cwd: ROOT });
    const lines = (buf) => String(buf).split("\n").filter(Boolean).forEach((line) => send("line", { line }));
    child.stdout.on("data", lines); child.stderr.on("data", lines);
    child.on("close", (code) => { after?.(code); send("done", { ok: code === 0, code }); res.end(); });
  };
  app.post("/api/admin/story/reset", requireAuth, requireAdmin, (_req, res) =>
    stream(res, "bash", ["init-db.sh"], (code) => { if (code === 0) tick = 0; }));
  app.post("/api/admin/story/save", requireAuth, requireAdmin, (_req, res) =>
    stream(res, "python3", ["tools/save_session_to_book.py"]));
  app.get("/api/admin/story/unsaved", requireAuth, requireAdmin, wrap(async (_req, res) => {
    const child = spawn("python3", ["tools/save_session_to_book.py", "--dry-run", "--json"], { cwd: ROOT });
    let out = "", err = "";
    child.stdout.on("data", (d) => (out += d)); child.stderr.on("data", (d) => (err += d));
    await new Promise((r) => child.on("close", r));
    if (!out.trim()) throw fail(500, "save_tool_failed", err);
    res.json(JSON.parse(out));
  }));
}
