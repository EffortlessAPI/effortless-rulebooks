import { useEffect, useMemo, useState } from "react";
import * as api from "../api";
import { useRows, useSession } from "../session";
import { ActionSheet, AppBar, Err, Loading, Sheet, Tag, fmtDay, fmtTime, human, useQuickAction, yn } from "../ui/kit";

const LIVE_VERSION = "loto-v2.0.0"; // the version act-tech-start-lockout runs; AppActionFields fixes it

export default function Lockout() {
  const { shell } = useSession();
  const me = shell!.claims.agent;
  const [tab, setTab] = useState<"runs" | "procedure" | "know" | "copilot">("runs");
  const [runId, setRunId] = useState<string | null>(null);
  const runs = useRows("procedure_executions", { executed_by_agent: me }, "-started_at");
  const knowHow = useRows("know_how_carriers", { holder_agent: me }, "-is_at_risk_of_imminent_loss");
  const answers = useRows("assistant_answers", { asked_by_agent: me }, "-asked_at");
  const atRisk = (knowHow.rows || []).some((k) => k.is_at_risk_of_imminent_loss);

  if (runId) return <Runner runId={runId} onBack={() => setRunId(null)} />;
  return (
    <>
      <AppBar title="My Lockout" />
      <div className="tabs" role="tablist">
        <button className="tab" aria-selected={tab === "runs"} onClick={() => setTab("runs")}>My runs<span className="n">{runs.rows?.length ?? "·"}</span></button>
        <button className="tab" aria-selected={tab === "procedure"} onClick={() => setTab("procedure")}>The procedure</button>
        <button className="tab" aria-selected={tab === "know"} onClick={() => setTab("know")}>What I know<span className={`n ${atRisk ? "red" : ""}`}>{knowHow.rows?.length ?? "·"}</span></button>
        <button className="tab" aria-selected={tab === "copilot"} onClick={() => setTab("copilot")}>Copilot<span className="n">{answers.rows?.length ?? "·"}</span></button>
      </div>
      <div className="content">
        {tab === "runs" && <Runs runs={runs} onOpen={setRunId} />}
        {tab === "procedure" && <Lane version={LIVE_VERSION} />}
        {tab === "know" && <KnowHow list={knowHow} />}
        {tab === "copilot" && <AnswerHistory list={answers} />}
      </div>
    </>
  );
}

function Runs({ runs, onOpen }: { runs: ReturnType<typeof useRows>; onOpen: (id: string) => void }) {
  const [starting, setStarting] = useState(false);
  const quick = useQuickAction();
  return (
    <>
      <button className="btn accent" onClick={() => setStarting(true)}>＋ Start a lockout</button>
      <div className="section">My lockouts</div>
      <Err error={runs.error} />
      {!runs.rows ? <Loading /> : runs.rows.length === 0 ? <div className="empty">No lockouts in your name yet.</div> : runs.rows.map((r) => (
        <div key={r.procedure_execution_id} className={`card pressable tight ${r.execution_status === "InProgress" ? "edge-amber" : r.execution_status === "Completed" ? "edge-green" : ""}`} onClick={() => onOpen(r.procedure_execution_id)}>
          <div className="row"><div className="grow"><div className="h">{r.title || r.name}</div><div className="sub">{fmtTime(r.started_at)} · {human(r.executed_on_machine)} · {r.shift || "—"} shift</div></div>
            <Tag tone={r.execution_status === "Completed" ? "green" : r.execution_status === "InProgress" ? "amber" : "grey"}>{r.execution_status.replace("InProgress", "In progress")}</Tag></div>
          {r.observations && <div className="quote" style={{ marginTop: 10 }}>{r.observations}</div>}
        </div>
      ))}
      {starting && <ActionSheet actionId="act-tech-start-lockout" submitLabel="Start" onClose={() => setStarting(false)}
        onDone={async (r) => { await quick("act-tech-begin-step", { context: { ProcedureExecution: r.key, Step: "loto-01" } }, { silent: true }); onOpen(r.key); }} />}
    </>
  );
}

function KnowHow({ list }: { list: ReturnType<typeof useRows> }) {
  if (!list.rows) return <Loading />;
  return (
    <>
      <p className="sub" style={{ margin: "4px 4px 12px" }}>Know-how is skill that lives in a person. The register keeps a card for each one, and works out which are about to be lost.</p>
      <Err error={list.error} />
      {list.rows.map((k) => (
        <div key={k.know_how_carrier_id} className="card">
          {k.is_at_risk_of_imminent_loss && <div className="banner red">● At risk of imminent loss</div>}
          {!k.is_at_risk_of_imminent_loss && k.is_captured && <div className="banner green">✓ Captured in the repository</div>}
          <div className="h" style={{ marginBottom: 10 }}>{k.topic}</div>
          <dl className="kv">
            <dt>Held by</dt><dd className="fact">{human(k.holder_agent)}</dd>
            <dt>Since</dt><dd className="fact">{new Date(k.held_since).getFullYear()}</dd>
            <dt>In the written procedure</dt><dd className="fact">{yn(k.is_in_written_procedure)}</dd>
            <dt>Passed on to anyone</dt><dd className={k.transfer_count > 0 ? "derived" : "bad"}>{k.transfer_count} {k.transfer_count === 1 ? "time" : "times"}</dd>
            <dt>Written into the repository</dt><dd className={k.repository_entry_count > 0 ? "derived" : "bad"}>{k.repository_entry_count} {k.repository_entry_count === 1 ? "time" : "times"}</dd>
            {k.holder_departure_at && <><dt>Holder departs</dt><dd className="fact">{fmtDay(k.holder_departure_at)}</dd>
              <dt>Days until departure</dt><dd className={k.is_at_risk_of_imminent_loss ? "bad" : "derived"}>{k.days_until_holder_departure}</dd></>}
          </dl>
          <p className="provenance">Blue is what somebody recorded. Green and red are worked out by the rulebook from those facts; nobody typed the banner.</p>
        </div>
      ))}
    </>
  );
}

function AnswerHistory({ list }: { list: ReturnType<typeof useRows> }) {
  if (!list.rows) return <Loading />;
  if (!list.rows.length) return <div className="empty">You have not asked the Copilot anything yet.</div>;
  return (<>{list.rows.map((a) => (
    <div key={a.assistant_answer_id} className={`answer ${a.task_outcome === "Failed" ? "safety" : ""}`}>
      <div className="row wrap" style={{ marginBottom: 6 }}><span className="sub">{fmtTime(a.asked_at)} · {human(a.context_step || a.assumed_current_step)}</span>
        <span className="grow" />{a.model_did_the_reasoning ? <Tag tone="red">language model did the reasoning</Tag> : <Tag tone="purple">{a.derivation_performed_by === "StructuredQuery" ? "structured query" : a.derivation_performed_by}</Tag>}</div>
      <div className="q">“{a.question_text}”</div>
      <div className="a" style={a.task_outcome === "Failed" ? { color: "var(--red)" } : undefined}>{a.answer_text}</div>
      <div className="row wrap" style={{ marginTop: 10 }}>
        {a.lost_track_of_state && <Tag tone="red">lost track of which step</Tag>}{a.contradicts_shared_model && <Tag tone="red">contradicts the procedure</Tag>}
        {a.was_correct === true && <Tag tone="green">reviewed: correct</Tag>}{a.task_outcome && <Tag tone={a.task_outcome === "Failed" ? "solid-red" : "green"}>task {a.task_outcome.toLowerCase()}</Tag>}
        <Tag tone="grey">{a.grounding_count} rows cited</Tag>
      </div>
      {a.documented_inaccuracy && <p className="sub" style={{ marginTop: 8 }}>{a.documented_inaccuracy}</p>}
    </div>))}</>);
}

// ------------------------------------------------------------------------------------------
// The lane: the procedure as steps. With a run it is a runner; without one it is for reading.
// ------------------------------------------------------------------------------------------
function Lane({ version, run, execs, onChanged }: { version: string; run?: api.Row; execs?: api.Row[]; onChanged?: () => void }) {
  const steps = useRows("steps", { procedure_version: version }, "step_number");
  const [peek, setPeek] = useState<string | null>(null);
  const open = execs?.find((e) => e.execution_status === "InProgress");
  useEffect(() => { if (open) document.getElementById(`step-${open.step}`)?.scrollIntoView({ block: "center", behavior: "smooth" }); }, [open?.step]);
  if (!steps.rows) return <Loading />;
  return (
    <div className="lane">
      <Err error={steps.error} />
      {steps.rows.map((s) => {
        const mine = (execs || []).filter((e) => e.step === s.step_id);
        const done = mine.find((e) => e.execution_status === "Completed");
        const isOpen = open?.step === s.step_id;
        const cls = ["step", s.parent_step ? "child" : "", isOpen ? (open?.is_blocked_by_observed_cue ? "open blocked" : "open") : done ? "done" : "", !run && peek === s.step_id ? "open" : ""].join(" ");
        return (
          <div key={s.step_id} id={`step-${s.step_id}`} className={cls}>
            <div className="num">{done && !isOpen ? "✓" : s.step_number}</div>
            <div className="body" onClick={() => !run && setPeek(peek === s.step_id ? null : s.step_id)} style={!run ? { cursor: "pointer" } : undefined}>
              <div className="title">{s.title}</div>
              {done && !isOpen && <div className="stamp">{human(done.executed_by_agent)} · {fmtTime(done.ended_at)}{done.verification_result === "WARN" ? " · left by a fallback" : ""}</div>}
              {(isOpen || (!run && peek === s.step_id)) && <StepBody step={s} run={run} exec={isOpen ? open : undefined} onChanged={onChanged} />}
            </div>
          </div>);
      })}
    </div>
  );
}

function StepBody({ step, run, exec, onChanged }: { step: api.Row; run?: api.Row; exec?: api.Row; onChanged?: () => void }) {
  const id = step.step_id;
  const conditions = useRows("step_conditions", { step: id });
  const locks = useRows("step_lock_requirements", { step: id });
  const cues = useRows("step_cues", { step: id });
  const fragments = useRows("knowledge_fragments", { step: id, status: "Approved" });
  const minds = useRows("expert_cognitions", { step: id });
  const values = useRows("concept_ladder_rungs", { step: id, rung_kind: "Value" });
  const outs = useRows("step_transitions", { from_step: id }, "priority");
  const energy = useRows(step.step_number === "01" && run?.executed_on_machine ? "machine_energy_sources" : null, { machine: run?.executed_on_machine ?? "" });
  const seen = useRows(exec ? "cue_observations" : null, { step_execution: exec?.step_execution_id ?? "" });
  const allSteps = useRows("steps", { procedure_version: step.procedure_version });
  const quick = useQuickAction();
  const [busy, setBusy] = useState(false); const [closing, setClosing] = useState(false);
  const label = (sid: string) => { const t = allSteps.rows?.find((x) => x.step_id === sid); return t ? `${t.step_number}  ${t.title}` : sid; };

  // The way forward is the step's outgoing transitions. Once a warning sign that means "not finished"
  // has been recorded (StepExecutions.IsBlockedByObservedCue, a rulebook formula), only a fallback is offered.
  const ways = (outs.rows || []).filter((t) => !exec?.is_blocked_by_observed_cue || t.transition_kind === "Fallback");
  const go = async (t: api.Row) => {
    if (!exec || !run) return; setBusy(true);
    const ok = await quick("act-tech-complete-step", { key: exec.step_execution_id, context: { VerificationResult: t.transition_kind === "Next" ? "PASS" : "WARN" } }, { silent: true });
    if (ok) await quick("act-tech-begin-step", { context: { ProcedureExecution: run.procedure_execution_id, Step: t.to_step } }, { silent: true });
    setBusy(false); onChanged?.();
  };

  return (
    <div className="stack" style={{ marginTop: 8 }} onClick={(e) => e.stopPropagation()}>
      {step.instruction && <p className="sub" style={{ color: "var(--ink-2)" }}>{step.instruction}</p>}
      {minds.rows?.map((m) => <div key={m.expert_cognition_id} className="quote">“{m.statement}”<span className="by">{human(m.agent)} · how an expert thinks about this step</span></div>)}
      {values.rows?.map((v) => <div key={v.concept_ladder_rung_id} className="quote">“{v.statement}”<span className="by">why this step exists</span></div>)}
      {energy.rows && energy.rows.length > 0 && <div className="row wrap">{energy.rows.map((e) => <Tag key={e.machine_energy_source_id} tone="blue">⚡ {e.energy_source.replace(/Energy$/, "")}</Tag>)}</div>}
      {locks.rows && locks.rows.length > 0 && <div className="row wrap">{locks.rows.map((l) => <Tag key={l.step_lock_requirement_id} tone="grey">🔒 {human(l.lock_device).replace(/([a-z])([A-Z])/g, "$1 $2")}</Tag>)}</div>}
      {conditions.rows?.map((c) => <div key={c.step_condition_id} className="sub"><b style={{ color: "var(--ink-2)" }}>{c.condition_kind === "Postcondition" ? "Must be true afterwards" : c.condition_kind === "Precondition" ? "Must be true first" : "Must stay true"}:</b> {c.statement}</div>)}
      {fragments.rows?.map((f) => <div key={f.knowledge_fragment_id} className="quote">“{f.statement}”<span className="by">{f.knowledge_form === "Tacit" ? "what the veterans know" : f.knowledge_form.replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase()} · from {human(f.source_agent)}</span></div>)}
      {cues.rows?.map((c) => {
        const obs = seen.rows?.find((o) => o.step_cue === c.step_cue_id);
        return (
          <div key={c.step_cue_id} className={`cue ${c.signals_incomplete_step ? "danger" : ""} ${obs ? "seen" : ""}`}>
            <div className="row top"><div className="grow"><div className="sub" style={{ fontWeight: 700, color: c.signals_incomplete_step ? "var(--red)" : "var(--green)" }}>{c.signals_incomplete_step ? "WARNING SIGN" : "WHAT YOU SHOULD SEE"} · {c.cue_kind.toLowerCase()}</div><div className="what">{c.description}</div></div>
              {exec && !obs && c.signals_incomplete_step && <button className="btn sm danger" disabled={busy} onClick={() => quick("act-tech-observe-cue", { context: { StepExecution: exec.step_execution_id, StepCue: c.step_cue_id }, watch: exec.step_execution_id }).then(() => onChanged?.())}>I see this</button>}</div>
            {c.signals_incomplete_step && <div className="row wrap" style={{ marginTop: 8 }}><Tag tone="red">means the step is not finished</Tag>{c.requires_escalation && <Tag tone="amber">goes to {human(c.escalate_to_role)}</Tag>}</div>}
            {obs && <div className="row" style={{ marginTop: 10 }}><span className="grow sub">Recorded {fmtTime(obs.observed_at)}.{obs.was_escalated ? ` Sent to ${human(obs.escalated_to_agent)}${obs.acknowledged_at ? ", acknowledged." : ", not yet acknowledged."}` : ""}</span>
              {!obs.was_escalated && c.requires_escalation && <button className="btn sm amber" onClick={() => quick("act-tech-escalate", { key: obs.cue_observation_id, watch: obs.cue_observation_id })}>Escalate</button>}</div>}
          </div>);
      })}
      {exec && run && (
        <div className="nexts">
          {exec.is_blocked_by_observed_cue && <div className="unsaved" style={{ background: "var(--red-wash)", color: "var(--red)", margin: 0 }}>A warning sign on this step means it is not finished. The normal next step is not offered.</div>}
          {ways.map((t) => (
            <button key={t.step_transition_id} className={`nextbtn ${t.transition_kind === "Fallback" ? "fallback" : t.transition_kind === "Alternative" ? "alt" : ""}`} disabled={busy} onClick={() => go(t)}>
              <span className="grow">{t.transition_kind === "Next" ? "Done → " : t.transition_kind === "Fallback" ? "Fallback → " : "Or → "}{label(t.to_step)}<small>{t.condition}</small></span>
            </button>))}
          {outs.rows && outs.rows.length === 0 && <button className="btn accent" onClick={() => setClosing(true)}>Finish this run</button>}
        </div>)}
      {closing && exec && run && <ActionSheet actionId="act-tech-close-run" recordKey={run.procedure_execution_id} onClose={() => setClosing(false)}
        onDone={() => quick("act-tech-complete-step", { key: exec.step_execution_id, context: { VerificationResult: "PASS" } }).then(() => onChanged?.())} />}
    </div>
  );
}

function Runner({ runId, onBack }: { runId: string; onBack: () => void }) {
  const run = useRows("procedure_executions", { procedure_execution_id: runId });
  const execs = useRows("step_executions", { procedure_execution: runId }, "started_at");
  const [ask, setAsk] = useState(false); const [view, setView] = useState<"run" | "ledger">("run");
  const r = run.rows?.[0]; const open = execs.rows?.find((e) => e.execution_status === "InProgress");
  if (!r) return <><AppBar title="My Lockout" /><div className="content"><Err error={run.error} /><Loading /></div></>;
  return (
    <>
      <AppBar title={human(r.executed_on_machine)} eyebrow={`${r.execution_status.replace("InProgress", "In progress")} · ${r.shift || ""} shift`} />
      <div className="tabs"><button className="tab" onClick={onBack}>‹ My runs</button>
        <button className="tab" aria-selected={view === "run"} onClick={() => setView("run")}>The run</button>
        <button className="tab" aria-selected={view === "ledger"} onClick={() => setView("ledger")}>Procedure and what happened</button></div>
      <div className="content" style={{ paddingBottom: 96 }}>
        <Err error={execs.error} />
        {view === "run" ? <Lane version={r.procedure_version} run={r} execs={execs.rows || []} /> : <Ledger run={r} execs={execs.rows || []} />}
      </div>
      {open && view === "run" && <button className="fab" onClick={() => setAsk(true)}>✦ Ask the Copilot</button>}
      {ask && open && <Ask stepId={open.step} exec={open} machine={r.executed_on_machine} onClose={() => setAsk(false)} />}
    </>
  );
}

function Ledger({ run, execs }: { run: api.Row; execs: api.Row[] }) {
  const steps = useRows("steps", { procedure_version: run.procedure_version }, "step_number");
  return (
    <div className="card">
      <div className="row" style={{ marginBottom: 10 }}><div className="grow"><div className="h">The procedure, and what happened</div><div className="sub">Two records, kept apart on purpose and compared step by step.</div></div></div>
      <table className="t"><thead><tr><th style={{ color: "var(--blue)" }}>The procedure says</th><th style={{ color: "var(--amber)" }}>What happened</th></tr></thead><tbody>
        {steps.rows?.map((s) => { const e = execs.filter((x) => x.step === s.step_id); return (
          <tr key={s.step_id}><td><b>{s.step_number}</b> {s.title}</td><td>{e.length === 0 ? <span className="cell-na">not carried out</span> : e.map((x) => (
            <div key={x.step_execution_id}>{x.execution_status === "Completed" ? fmtTime(x.ended_at) : "in progress"} {x.verification_result === "WARN" && <Tag tone="amber">left by a fallback</Tag>} {x.is_blocked_by_observed_cue && <Tag tone="red">warning sign seen</Tag>} {x.is_out_of_specified_order && <Tag tone="red">out of order</Tag>}</div>))}</td></tr>); })}
      </tbody></table>
    </div>);
}

function Ask({ stepId, exec, machine, onClose }: { stepId: string; exec: api.Row; machine: string; onClose: () => void }) {
  const { bump } = useSession();
  const [qs, setQs] = useState<{ id: string; text: string }[] | null>(null);
  const [answer, setAnswer] = useState<api.Answer | null>(null); const [error, setError] = useState<string | null>(null);
  useEffect(() => { api.copilotQuestions(stepId, exec.step_execution_id).then(setQs).catch((e) => setError(e.message)); }, [stepId, exec.step_execution_id]);
  const ask = (id: string) => api.copilotAsk({ questionId: id, stepId, stepExecutionId: exec.step_execution_id, machineId: machine }).then((a) => { setAnswer(a); bump(); }).catch((e) => setError(e.message));
  return (
    <Sheet title="Ask the Copilot" desc="These are the questions the book can answer about the step you are on." onClose={onClose}>
      <Err error={error} />
      {!answer && (qs ? qs.map((q) => <button key={q.id} className="btn ghost" style={{ marginBottom: 8, justifyContent: "flex-start", textAlign: "left", height: "auto", padding: "12px 14px", color: "var(--blue)" }} onClick={() => ask(q.id)}>{q.text}</button>) : <Loading />)}
      {answer && (
        <div className={`answer ${answer.safety ? "safety" : ""}`} style={{ marginTop: 0 }}>
          <div className="q">“{answer.question}”</div><div className="a" style={answer.safety ? { color: "var(--red)" } : undefined}>{answer.text}</div>
          <div className="section" style={{ margin: "14px 0 4px" }}>The rows it used <span className="count">{answer.groundings.length}</span></div>
          {answer.groundings.map((g) => <div key={g.table + g.key} className="ground"><code>{g.table} · {g.key}</code><span>{g.why}</span></div>)}
          <div className="row wrap" style={{ marginTop: 12 }}><Tag tone="purple">structured query</Tag><Tag tone="grey">no language model</Tag><Tag tone="grey">answered as {answer.answeredBy}</Tag></div>
        </div>)}
      {answer && <button className="btn ghost" style={{ marginTop: 12 }} onClick={() => setAnswer(null)}>Ask something else</button>}
      <p className="provenance">The Copilot signs in as its own database role with a narrower schema than yours. It reads the same steps, warning signs and paths you do, and cannot read anything it was never granted.</p>
    </Sheet>);
}
