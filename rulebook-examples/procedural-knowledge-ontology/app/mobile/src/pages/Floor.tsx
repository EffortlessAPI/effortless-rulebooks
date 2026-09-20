import { useState } from "react";
import * as api from "../api";
import { useRows, useSession } from "../session";
import { ActionSheet, AppBar, Err, Loading, Tag, fmtDay, human } from "../ui/kit";

export default function Floor() {
  const { shell } = useSession();
  const [tab, setTab] = useState<"decide" | "people" | "knowhow" | "levels">("decide");
  const changes = useRows("change_requests", { authority_role: shell!.claims.role }, "-requested_at");
  const onboarding = useRows("onboarding_records", {}, "-days_to_proficiency");
  const mentor = useRows("mentorships", {}, "valid_from");
  const gone = useRows("know_how_carriers", {}, "days_until_holder_departure");
  const levels = useRows("process_knowledge_levels", {});
  const diverge = useRows("workflow_view_divergences", {});
  const [decide, setDecide] = useState<api.Row | null>(null);
  const open = changes.rows?.filter((c) => !c.is_decided) ?? null;

  return (
    <>
      <AppBar title="The Floor, This Quarter" />
      <div className="tabs">
        <button className="tab" aria-selected={tab === "decide"} onClick={() => setTab("decide")}>Awaiting me<span className={`n ${open?.length ? "red" : ""}`}>{open?.length ?? "·"}</span></button>
        <button className="tab" aria-selected={tab === "people"} onClick={() => setTab("people")}>People</button>
        <button className="tab" aria-selected={tab === "knowhow"} onClick={() => setTab("knowhow")}>Know-how</button>
        <button className="tab" aria-selected={tab === "levels"} onClick={() => setTab("levels")}>Levels</button>
      </div>
      <div className="content">
        {tab === "decide" && (<><Err error={changes.error} />
          {!changes.rows ? <Loading /> : changes.rows.length === 0 ? <div className="empty">No decision has been handed to you.</div> : changes.rows.map((c) => (
            <div key={c.change_request_id} className={`card ${c.is_decided ? (c.is_open ? "edge-amber" : "edge-green") : "edge-amber"}`}>
              <div className="h">{c.title}</div><div className="sub">{human(c.procedure_version)} · requested by {human(c.requested_by_agent)}</div>
              <p className="sub" style={{ marginTop: 8, color: "var(--ink-2)" }}>{c.impact_assessment}</p>
              <div className="row wrap" style={{ marginTop: 10 }}><Tag tone={c.is_decided ? "green" : "amber"}>{c.status.replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase()}</Tag>
                {c.is_decided && <Tag tone={c.is_open ? "amber" : "green"}>{c.is_open ? "decided, not yet made" : "closed"}</Tag>}<span className="grow" />
                {!c.is_decided && <button className="btn sm accent" onClick={() => setDecide(c)}>Decide</button>}</div>
            </div>))}
          <div className="section">Two versions of one step</div>
          {diverge.rows?.map((d) => (
            <div key={d.workflow_view_divergence_id} className={`card ${d.is_reconciled ? "edge-green" : "edge-amber"}`}>
              <div className="sub" style={{ fontWeight: 700 }}>{human(d.step)}</div>
              <div className="quote" style={{ marginTop: 8 }}>“{d.view_a}”<span className="by">{human(d.holder_a)}</span></div>
              <div className="quote" style={{ marginTop: 8 }}>“{d.view_b}”<span className="by">{human(d.holder_b)}</span></div>
              {d.is_reconciled ? <p className="derived" style={{ marginTop: 10, fontWeight: 650 }}>Agreed {fmtDay(d.reconciled_at)}: {d.reconciled_statement}</p> : <div style={{ marginTop: 10 }}><Tag tone="amber">nobody has decided</Tag></div>}
            </div>))}
        </>)}
        {tab === "people" && (<>
          <div className="section">Days to become proficient</div>
          <div className="card">{onboarding.rows?.filter((o) => o.is_proficient).map((o) => (
            <div key={o.onboarding_record_id} style={{ marginBottom: 14 }}>
              <div className="row"><b className="grow">{human(o.new_starter)}</b><span className="big" style={{ fontSize: 22, color: o.is_starting_from_nothing ? "var(--red)" : o.used_captured_knowledge ? "var(--green)" : "var(--amber)" }}>{o.days_to_proficiency}</span></div>
              <div className="bar" style={{ margin: "6px 0" }}><i style={{ width: `${Math.min(100, o.days_to_proficiency)}%`, background: o.is_starting_from_nothing ? "var(--red)" : o.used_captured_knowledge ? "var(--green)" : "var(--amber)" }} /></div>
              <div className="sub">{human(o.procedure)} · {o.is_starting_from_nothing ? "started from nothing" : o.used_captured_knowledge ? "used captured knowledge" : "no captured knowledge used"}</div>
            </div>))}</div>
          <div className="section">Who is mentoring whom</div>
          {mentor.rows?.map((m) => <div key={m.mentorship_id} className="card tight"><div className="row"><b>{human(m.mentor_agent)}</b><span className="sub">→</span><b className="grow">{human(m.learner_agent)}</b>{m.is_active ? <Tag tone="green">active</Tag> : <Tag tone="grey">ended</Tag>}</div><div className="sub">{m.learning_objective}</div></div>)}
        </>)}
        {tab === "knowhow" && gone.rows?.map((k) => (
          <div key={k.know_how_carrier_id} className={`card tight ${k.is_held_only_by_departed ? "edge-black" : k.is_at_risk_of_imminent_loss ? "edge-red" : k.is_captured ? "edge-green" : ""}`}>
            <div className="h" style={{ fontSize: 15 }}>{k.topic}</div><div className="sub">{k.holder_agent ? `${human(k.holder_agent)} · ${k.holder_tenure_years} years` : human(k.carrier_kind)}</div>
            <div className="row wrap" style={{ marginTop: 8 }}>{k.is_held_only_by_departed && <Tag tone="black">held only by someone who has left</Tag>}{k.is_at_risk_of_imminent_loss && <Tag tone="solid-red">at risk of imminent loss</Tag>}
              <Tag tone={k.is_captured ? "green" : "grey"}>{k.is_captured ? "captured" : "not written down"}</Tag><Tag tone={k.transfer_count > 0 ? "green" : "grey"}>passed on {k.transfer_count}×</Tag></div>
          </div>))}
        {tab === "levels" && (<>
          <p className="sub" style={{ margin: "6px 4px 12px" }}>The same knowledge, at three heights. Each level needs a plan for capturing what is written down and what is not.</p>
          {levels.rows?.map((l) => (
            <div key={l.process_knowledge_level_id} className={`card ${l.lacks_capture_strategy_for_either_form ? "edge-red" : "edge-green"}`}>
              <div className="row"><div className="grow"><div className="h">{l.label}</div><div className="sub">{l.planning_horizon}</div></div>{l.lacks_capture_strategy_for_either_form ? <Tag tone="solid-red">no plan for one form</Tag> : <Tag tone="green">both forms planned</Tag>}</div>
              <dl className="kv" style={{ marginTop: 10 }}><dt>Plans for capturing the unwritten</dt><dd className={l.tacit_strategy_count ? "derived" : "bad"}>{l.tacit_strategy_count}</dd><dt>Plans for capturing the written</dt><dd className={l.explicit_strategy_count ? "derived" : "bad"}>{l.explicit_strategy_count}</dd></dl>
            </div>))}
        </>)}
      </div>
      {decide && <ActionSheet actionId="act-floor-decide" recordKey={decide.change_request_id} watch={decide.change_request_id} onClose={() => setDecide(null)} />}
    </>
  );
}
