import { useState } from "react";
import * as api from "../api";
import { useRows, useSession } from "../session";
import { Controls } from "../ui/controls";
import { ActionSheet, AppBar, Err, ExplainerNote, Explains, Fact, KV, Loading, Tag, Why, fmtTime, human, useQuickAction } from "../ui/kit";

export default function SafetyDesk() {
  const { shell } = useSession();
  const me = shell!.claims.agent; const myRole = shell!.claims.role;
  const [tab, setTab] = useState<"inbox" | "controls" | "insights" | "changes">("inbox");
  const waiting = useRows("cue_observations", { is_awaiting_acknowledgement: true, escalated_to_agent: me }, "-observed_at");
  const never = useRows("cue_observations", { is_unescalated_danger_cue: true }, "-observed_at");
  const breaches = useRows("condition_checks", { is_scored_breach: true }, "-checked_at");
  const insights = useRows("ai_insight_proposals", {}, "-proposed_at");
  const changes = useRows("change_requests", { authority_role: myRole, is_decided: false }, "-requested_at");
  const quick = useQuickAction();
  const [verdict, setVerdict] = useState<api.Row | null>(null);
  const [handUp, setHandUp] = useState<api.Row | null>(null); const [decide, setDecide] = useState<api.Row | null>(null);

  return (
    <>
      <AppBar title="Safety Desk" />
      <div className="tabs">
        <button className="tab" aria-selected={tab === "inbox"} onClick={() => setTab("inbox")}>Inbox<span className={`n ${waiting.rows?.length ? "red" : ""}`}>{waiting.rows?.length ?? "·"}</span></button>
        <button className="tab" aria-selected={tab === "controls"} onClick={() => setTab("controls")}>Controls</button>
        <button className="tab" aria-selected={tab === "insights"} onClick={() => setTab("insights")}>Insights<span className="n">{insights.rows?.length ?? "·"}</span></button>
        <button className="tab" aria-selected={tab === "changes"} onClick={() => setTab("changes")}>Decisions<span className="n">{changes.rows?.length ?? "·"}</span></button>
      </div>
      <div className="content">
        {tab === "inbox" && (<>
          <ExplainerNote />
          <div className="section">Rules broken, by the register's own count</div><Err error={breaches.error} />
          <p className="sub" style={{ margin: "0 4px 10px" }}>A check said no, and the rule that check exists to keep is one the register computes. Nobody had to look.</p>
          {!breaches.rows ? <Loading /> : breaches.rows.length === 0 ? <div id="no-breaches" className="empty">None the register can count yet. A rule is counted once a column computes it.</div> : breaches.rows.map((c) => <Breach key={c.condition_check_id} c={c} />)}
          <div className="section">Escalated to me</div><Err error={waiting.error} />
          {!waiting.rows ? <Loading /> : waiting.rows.length === 0 ? <div className="empty">Nothing is waiting on you.</div> : waiting.rows.map((o) => (
            <Observation key={o.cue_observation_id} o={o} tone="amber" action={<button className="btn sm accent" onClick={() => quick("act-safety-acknowledge", { key: o.cue_observation_id, watch: o.cue_observation_id })}>Acknowledge</button>} />))}
          <div className="section">Seen, and never escalated</div>
          <p className="sub" style={{ margin: "0 4px 10px" }}>A warning sign was recorded, it requires escalation, and nobody was told. The register does not need an injury to notice.</p>
          {never.rows?.map((o) => <Observation key={o.cue_observation_id} o={o} tone="red" />)}
        </>)}
        {tab === "controls" && <Controls mode="officer" />}
        {tab === "insights" && (<>
          <p className="sub" style={{ margin: "6px 4px 12px" }}>Patterns an AI agent found in the execution records. A machine proposes; a person checks it against the floor and commits.</p>
          <ExplainerNote />
          {insights.rows?.map((i) => (
            <Explains key={i.ai_insight_proposal_id} t="ai_insight_proposals">
            <div className={`card ${i.validation_verdict === "Valid" ? "edge-green" : i.validation_verdict ? "" : "edge-amber"}`}>
              <div className="row wrap" style={{ marginBottom: 8 }}><Tag tone="purple" f="proposing_agent">proposed by {human(i.proposing_agent)}</Tag><span className="sub">{fmtTime(i.proposed_at)}</span></div>
              <div className="quote">“{i.statement}”<Why f="statement" /></div>
              <div className="row wrap" style={{ marginTop: 10 }}>
                {i.validation_verdict ? <Tag tone={i.validation_verdict === "Valid" ? "green" : "grey"} f="validation_verdict">{i.validation_verdict.toLowerCase()} · {human(i.validated_by_agent)}</Tag> : <Tag tone="amber" f="validation_verdict">nobody has checked this</Tag>}
                {i.folded_into_change_request && <Tag tone="blue" f="folded_into_change_request">became a change request</Tag>}<span className="grow" />
                {!i.validation_verdict && <button className="btn sm" onClick={() => setVerdict(i)}>Record my verdict</button>}
              </div>
            </div></Explains>))}
        </>)}
        {tab === "changes" && (<>
          <p className="sub" style={{ margin: "6px 4px 12px" }}>Changes to procedures where you are the authority.</p>
          {changes.rows?.length === 0 && <div className="empty">No decisions are waiting on you.</div>}
          {changes.rows?.map((c) => (
            <Explains key={c.change_request_id} t="change_requests">
            <div className={`card ${c.requester_is_authority ? "edge-red" : "edge-amber"}`}>
              {c.requester_is_authority && <div className="banner red">● Requester is the authority<Why f="requester_is_authority" label="requester is the authority" /></div>}
              <div className="h">{c.title}</div><div className="sub">{human(c.procedure_version)} · {c.change_kind}<Why f="change_kind" /></div>
              <KV t="change_requests" style={{ marginTop: 10 }}>
                <Fact f="requested_by_agent" label="Requested by" tone="fact">{human(c.requested_by_agent)}</Fact>
                <Fact f="authority_agent" label="Decided by" tone="derived">{human(c.authority_agent)} <span className="sub">({c.authority_role_label})</span></Fact>
                <Fact f="days_pending" label="Waiting" tone="derived">{c.days_pending} days</Fact>
              </KV>
              <p className="sub" style={{ marginTop: 8 }}>{c.impact_assessment}<Why f="impact_assessment" /></p>
              <div className="row" style={{ marginTop: 12 }}>
                {c.requester_is_authority && c.requested_by_agent === me
                  ? <><span className="grow sub" style={{ color: "var(--red)", fontWeight: 650 }}>You raised this request, so you cannot decide it.</span><button className="btn sm" onClick={() => setHandUp(c)}>Hand the decision up</button></>
                  : <><span className="grow" /><button className="btn sm ghost" onClick={() => setHandUp(c)}>Hand up</button><button className="btn sm accent" onClick={() => setDecide(c)}>Decide</button></>}
              </div>
            </div></Explains>))}
        </>)}
      </div>
      {verdict && <ActionSheet actionId="act-safety-validate-insight" recordKey={verdict.ai_insight_proposal_id} watch={verdict.ai_insight_proposal_id} onClose={() => setVerdict(null)} />}
      {handUp && <ActionSheet actionId="act-safety-hand-up" recordKey={handUp.change_request_id} watch={handUp.change_request_id} onClose={() => setHandUp(null)}
        choiceFilter={(_f, r) => r.organization === shell!.claims.organization && r.role_id !== myRole && !!r.current_agent && r.current_agent_kind === "Human"} />}
      {decide && <ActionSheet actionId="act-safety-decide" recordKey={decide.change_request_id} watch={decide.change_request_id} onClose={() => setDecide(null)} />}
    </>
  );
}

// A breach the register counted itself (ConditionChecks.IsScoredBreach, loop 20): the check that said no,
// the rule it exists to keep, who and when, and whether the work went ahead regardless.
function Breach({ c }: { c: api.Row }) {
  const exec = useRows("step_executions", { step_execution_id: c.step_execution });
  const run = useRows(exec.rows?.[0] ? "procedure_executions" : null, { procedure_execution_id: exec.rows?.[0]?.procedure_execution ?? "" });
  const cond = useRows("step_conditions", { step_condition_id: c.step_condition });
  const e = exec.rows?.[0]; const r = run.rows?.[0];
  return (
    <Explains t="condition_checks">
    <div id={`breach-${c.condition_check_id}`} className="card edge-red">
      <div className="row"><div className="grow"><div className="h">{human(c.checked_by_agent)}<Why f="checked_by_agent" /></div><div className="sub">{fmtTime(c.checked_at)}{r ? ` · ${human(r.executed_on_machine)} · ${r.shift || ""} shift` : ""} · {human(e?.step)}</div></div>
        <Tag tone="red" f="is_scored_breach">rule broken</Tag></div>
      <div className="quote" style={{ marginTop: 10 }}>{c.enforced_requirement_statement}<span className="by">the rule<Why f="enforced_requirement_statement" /></span></div>
      <div className="cue danger" style={{ marginTop: 10 }}><div className="what">{cond.rows?.[0]?.statement}<Why t="step_conditions" f="statement" /></div><div className="sub" style={{ marginTop: 4 }}>The check: <b style={{ color: "var(--red)" }}>did not hold</b><Why f="held" /></div></div>
      {e && <div className="row wrap" style={{ marginTop: 10 }}>{e.proceeded_despite_failed_precondition
        ? <Tag tone="red" t="step_executions" f="proceeded_despite_failed_precondition">the work went ahead anyway</Tag>
        : e.is_blocked_by_failed_precondition ? <Tag tone="green" t="step_executions" f="is_blocked_by_failed_precondition">the step was not allowed to finish</Tag> : null}</div>}
    </div></Explains>);
}

function Observation({ o, tone, action }: { o: api.Row; tone: "amber" | "red"; action?: React.ReactNode }) {
  const cue = useRows("step_cues", { step_cue_id: o.step_cue });
  const exec = useRows("step_executions", { step_execution_id: o.step_execution });
  const run = useRows(exec.rows?.[0] ? "procedure_executions" : null, { procedure_execution_id: exec.rows?.[0]?.procedure_execution ?? "" });
  const c = cue.rows?.[0]; const r = run.rows?.[0];
  return (
    <Explains t="cue_observations">
    <div className={`card edge-${tone}`}>
      <div className="row"><div className="grow"><div className="h">{human(o.observed_by_agent)}<Why f="observed_by_agent" /></div><div className="sub">{fmtTime(o.observed_at)}{r ? ` · ${human(r.executed_on_machine)} · ${r.shift || ""} shift` : ""} · {human(exec.rows?.[0]?.step)}</div></div>
        <Tag tone={tone} f={tone === "red" ? "is_unescalated_danger_cue" : "is_awaiting_acknowledgement"}>{tone === "red" ? "seen, not escalated" : "waiting on you"}</Tag></div>
      {c && <div className="cue danger" style={{ marginTop: 10 }}><div className="what">{c.description}<Why t="step_cues" f="description" /></div></div>}
      {tone === "red" && r?.observations && <div className="quote" style={{ marginTop: 10 }}>{r.observations}<span className="by">the run's own note<Why t="procedure_executions" f="observations" /></span></div>}
      {action && <div className="row" style={{ marginTop: 12 }}><span className="grow" />{action}</div>}
    </div></Explains>);
}
