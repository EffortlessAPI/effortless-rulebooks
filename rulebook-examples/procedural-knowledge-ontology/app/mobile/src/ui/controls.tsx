// The controls: every blocking requirement, with what the rulebook says about its health.
//
// A control's PASS marks, its evaluations and the failed checks of the conditions that enforce it
// are three different numbers, and the story of loop 18 is that they can disagree: the lockout's
// zero-energy control was passed six times and evaluated never, while the execution record held a
// failed check of "zero energy has been verified". Every number here is a column of vw_requirements;
// the lists are rows, never counted in the client.
import { useState } from "react";
import * as api from "../api";
import { useRows, useSession } from "../session";
import { ActionSheet, Err, Explains, Loading, Tag, Why, fmtTime, human } from "./kit";

const STATE_TONE: Record<string, "red" | "amber" | "green" | "grey"> = {
  Decorative: "red", Inoperative: "red", Asserted: "amber", Untested: "amber", Demonstrated: "green", Holding: "green",
};
const STATE_SAYS: Record<string, string> = {
  Decorative: "attached to no step at all",
  Inoperative: "attached to a step, never once evaluated",
  Asserted: "a person has evaluated it; nothing in the book computes it",
  Demonstrated: "a named column computes it, and it has been seen to fail",
  Holding: "computed, and has held across enough evaluations to mean something",
  Untested: "computed, and not yet evaluated often enough to mean anything",
};

export function Controls({ mode }: { mode: "officer" | "engineer" }) {
  const { shell } = useSession();
  const me = shell!.claims.agent;
  const all = useRows("requirements", { is_blocking: true }, "-is_breached_but_never_failed,-is_passed_but_never_evaluated,-recorded_pass_count,requirement_id");
  const [open, setOpen] = useState<string | null>(null);
  if (!all.rows) return <><Err error={all.error} /><Loading /></>;
  const mine = all.rows.filter((r) => r.accountable_agent === me);
  const rest = all.rows.filter((r) => r.accountable_agent !== me);
  const card = (r: api.Row) => <Control key={r.requirement_id} r={r} mode={mode} open={open === r.requirement_id} onToggle={() => setOpen(open === r.requirement_id ? null : r.requirement_id)} />;
  return (
    <div id="controls">
      <p className="sub" style={{ margin: "6px 4px 12px", maxWidth: 760 }}>Every blocking control in the book. A control that has never been seen to fail has never been seen to work, so a clean record means nothing until somebody has asked.</p>
      {mode === "officer" && <><div className="section">Controls you answer for</div>{mine.length === 0 && <div className="empty">None.</div>}{mine.map(card)}
        <div className="section">Everyone else's</div></>}
      {(mode === "officer" ? rest : all.rows).map(card)}
    </div>
  );
}

function Control({ r, mode, open, onToggle }: { r: api.Row; mode: "officer" | "engineer"; open: boolean; onToggle: () => void }) {
  const state = String(r.control_assurance_state || "");
  const tone = STATE_TONE[state] || "grey";
  return (
    <Explains t="requirements">
      <div id={`ctl-${r.requirement_id}`} data-state={state} className={`card control ${tone === "red" ? "edge-red" : tone === "amber" ? "edge-amber" : tone === "green" ? "edge-green" : ""}`}>
        {r.is_breached_but_never_failed && <div className="banner red breached">● The book holds a failed check of this control. Its own record says it has never failed.<Why f="is_breached_but_never_failed" label="breached, and never failed" /></div>}
        <div className="row top" onClick={onToggle} style={{ cursor: "pointer" }}>
          <div className="grow"><div className="h">{r.statement}<Why f="statement" /></div>
            <div className="sub">{human(r.requirement_type)} control{r.accountable_agent ? ` · ${human(r.accountable_agent)} answers for it` : ""}</div></div>
          <span className="state"><Tag tone={tone} f="control_assurance_state">{state}</Tag></span>
        </div>
        <div className="sub says" style={{ marginTop: 4 }}>{STATE_SAYS[state]}</div>
        <div className="grid c3 counts" style={{ marginTop: 12 }}>
          <div data-count="recorded_pass_count"><div className="big">{r.recorded_pass_count}</div><div className="sub">PASS marks on its steps<Why f="recorded_pass_count" label="PASS marks on its steps" /></div></div>
          <div data-count="satisfaction_record_count"><div className="big" style={{ color: r.satisfaction_record_count ? undefined : "var(--red)" }}>{r.satisfaction_record_count}</div><div className="sub">times anyone evaluated it<Why f="satisfaction_record_count" label="times anyone evaluated it" /></div></div>
          <div data-count="failed_check_count"><div className="big" style={{ color: r.failed_check_count ? "var(--red)" : undefined }}>{r.failed_check_count}</div><div className="sub">failed checks on record<Why f="failed_check_count" label="failed checks on record" /></div></div>
        </div>
        {r.attestation_exposure_note && <p className="sub exposure" style={{ marginTop: 10 }}>{r.attestation_exposure_note}<Why f="attestation_exposure_note" /></p>}
        {r.witness_field_name && <div className="row wrap witness" style={{ marginTop: 8 }}><Tag tone={r.derived_has_computed_witness ? "green" : "red"} f="derived_has_computed_witness">computed by {r.witness_field_name}</Tag>{r.witness_claim_is_unverified && <Tag tone="red" f="witness_claim_is_unverified">that column is not in the book</Tag>}</div>}
        {open && <Detail r={r} mode={mode} />}
      </div>
    </Explains>
  );
}

function Detail({ r, mode }: { r: api.Row; mode: "officer" | "engineer" }) {
  const { shell } = useSession();
  const id = r.requirement_id as string;
  const can = (a: string) => shell!.actions.some((x) => x.app_action_id === a);
  const conds = useRows("step_conditions", { enforces_requirement: id });
  const failed = useRows("condition_checks", { failed_check_requirement_key: id }, "checked_at");
  const evals = useRows("requirement_satisfactions", { requirement: id }, "evaluated_at");
  const bind = useRows("step_requirements", { requirement: id });
  const step = bind.rows?.[0]?.step as string | undefined;
  const runs = useRows(step ? "step_executions" : null, { step: step ?? "" }, "started_at");
  const [evaluate, setEvaluate] = useState<api.Row | null>(null); const [witness, setWitness] = useState(false);
  const statement = (c: string) => conds.rows?.find((x) => x.step_condition_id === c)?.statement || human(c);
  return (
    <div className="stack detail" style={{ marginTop: 14 }} onClick={(e) => e.stopPropagation()}>
      <div className="section" style={{ margin: 0 }}>Failed checks on record</div>
      <Explains t="condition_checks">
        {failed.rows?.length === 0 && <div className="empty">None.</div>}
        {failed.rows?.map((c) => (
          <div key={c.condition_check_id} id={`chk-${c.condition_check_id}`} className="cue danger failed-check">
            <div className="what">{statement(c.step_condition)}<Why f="step_condition" /></div>
            <div className="sub" style={{ marginTop: 4 }}>Checked {fmtTime(c.checked_at)} by {human(c.checked_by_agent)}: <b style={{ color: "var(--red)" }}>did not hold</b><Why f="held" /></div>
          </div>))}
      </Explains>
      <div className="section" style={{ margin: "6px 0 0" }}>Evaluations of the control itself</div>
      <Explains t="requirement_satisfactions">
        {evals.rows?.length === 0 && <div className="empty never">Never evaluated. Not once.</div>}
        {evals.rows?.map((s) => (
          <div key={s.requirement_satisfaction_id} className="card tight evaluation">
            <div className="row wrap"><Tag tone={s.is_fully_satisfied ? "green" : "red"} f="satisfaction_level">{String(s.satisfaction_level).replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase()}</Tag>
              <span className="sub">{human(s.evaluated_by_agent)} · {fmtTime(s.evaluated_at)}</span>
              {s.evaluator_is_step_executor && <Tag tone="amber" f="evaluator_is_step_executor">scored their own step</Tag>}
              {s.is_bare_assertion && <Tag tone="amber" f="is_bare_assertion">no evidence</Tag>}</div>
            {s.evidence && <div className="quote" style={{ marginTop: 8 }}>{s.evidence}<Why f="evidence" /></div>}
          </div>))}
      </Explains>
      {step && <>
        <div className="section" style={{ margin: "6px 0 0" }}>Every run of the step it guards</div>
        <Explains t="step_executions">
          {runs.rows?.map((x) => (
            <div key={x.step_execution_id} id={`run-${x.step_execution_id}`} className="row guarded-run" style={{ padding: "6px 0", borderTop: "1px solid var(--line)" }}>
              <span className="grow">{human(x.executed_by_agent)} · {fmtTime(x.ended_at || x.started_at)} · <b className={x.verification_result === "PASS" ? "derived" : ""}>{x.verification_result || "in progress"}</b><Why f="verification_result" /></span>
              {mode === "officer" && can("act-safety-evaluate-control") && x.execution_status === "Completed" && <button className="btn sm accent" onClick={() => setEvaluate(x)}>Evaluate this control</button>}
            </div>))}
        </Explains>
      </>}
      {mode === "engineer" && can("act-ke-name-witness") && !r.has_computed_witness && (
        <div className="row"><span className="grow sub">Nothing in the book computes this control yet.</span><button className="btn sm accent" onClick={() => setWitness(true)}>Name the column that computes it</button></div>)}
      {evaluate && <ActionSheet actionId="act-safety-evaluate-control" context={{ Requirement: id, StepExecution: evaluate.step_execution_id }} watch={id} onClose={() => setEvaluate(null)} />}
      {witness && <ActionSheet actionId="act-ke-name-witness" recordKey={id} watch={id} onClose={() => setWitness(false)} />}
    </div>
  );
}
