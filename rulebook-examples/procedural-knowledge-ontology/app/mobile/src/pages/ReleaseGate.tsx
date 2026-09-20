import { useState } from "react";
import { useRows } from "../session";
import { AppBar, Big, ExplainerNote, Explains, Tag, Why, fmtDay, human } from "../ui/kit";

export default function ReleaseGate() {
  const [tab, setTab] = useState<"upgrade" | "history" | "words">("upgrade");
  const assess = useRows("agent_upgrade_assessments", {}, "assessed_at"); const roles = useRows("role_assignments", {}, "role,valid_from");
  const words = useRows("term_meaning_changes", {}, "-changed_at"); const [on, setOn] = useState("2025-12-01");
  const at = new Date(on + "T12:00:00Z").getTime();
  const gate = ["change-risk-classifier", "release-manager", "lead-release-manager", "vp-engineering"];
  return (
    <>
      <AppBar title="Release Gate" />
      <div className="tabs"><button className="tab" aria-selected={tab === "upgrade"} onClick={() => setTab("upgrade")}>Blast radius</button><button className="tab" aria-selected={tab === "history"} onClick={() => setTab("history")}>Who held the role that day</button><button className="tab" aria-selected={tab === "words"} onClick={() => setTab("words")}>What we mean by “release”</button></div>
      <div className="content">
        {tab === "upgrade" && (<><p className="sub" style={{ margin: "6px 4px 14px", maxWidth: 780 }}>An AI agent is a role holder like anyone else. Before it is upgraded: everything it produced, every step that consumes that, and who owns those steps.</p>
          <ExplainerNote />
          <div className="grid c2">{assess.rows?.map((a) => (<Explains key={a.agent_upgrade_assessment_id} t="agent_upgrade_assessments"><div className={`card ${a.missed_traversed_impact ? "edge-red" : "edge-green"}`} style={{ marginBottom: 0 }}>
            <div className="row top"><div className="grow"><div className="h">{a.assessment_method === "ChangeTicket" ? "A person's change ticket" : "Walked from the book"}<Why f="assessment_method" /></div><div className="sub">{human(a.assessed_by_agent)} · {fmtDay(a.assessed_at)}</div></div>{a.missed_traversed_impact ? <Tag tone="solid-red" f="missed_traversed_impact">missed impact</Tag> : <Tag tone="green" f="missed_traversed_impact">complete</Tag>}</div>
            <div className="sub" style={{ marginTop: 10 }}>{human(a.current_agent)} → <b>{human(a.candidate_agent)}</b><Why f="candidate_agent" /></div>
            <div className="row" style={{ marginTop: 14, gap: 22 }}>
              <Big f="listed_affected_step_count" caption="steps listed" tone={a.missed_traversed_impact ? "var(--red)" : "var(--green)"}>{a.listed_affected_step_count}</Big>
              <Big f="traversed_downstream_step_count" caption="steps the book finds">{a.traversed_downstream_step_count}</Big>
              <Big f="attributed_artifact_count" caption="artifacts it produced">{a.attributed_artifact_count}</Big></div>
            {a.listed_affected_steps_note && <div className="quote" style={{ marginTop: 12 }}>{a.listed_affected_steps_note}<Why f="listed_affected_steps_note" /></div>}
          </div></Explains>))}</div><p className="provenance">The count the book finds is a closure over who produced what and which step consumes it (<code>vw_artifact_handoffs_closure</code>), not a list anyone maintains.</p></>)}
        {tab === "history" && (<><div className="card"><label className="field" style={{ marginBottom: 0, maxWidth: 320 }}><span>Who held each role on…</span><input type="text" value={on} onChange={(e) => setOn(e.target.value)} /></label></div>
          {gate.map((g) => { const rs = roles.rows?.filter((r) => r.role === g) || []; if (!rs.length) return null; return (<Explains key={g} t="role_assignments"><div className="card"><div className="h" style={{ marginBottom: 8 }}>{human(g)}</div>
            {rs.map((r) => { const held = new Date(r.valid_from).getTime() <= at && (!r.valid_to || new Date(r.valid_to).getTime() > at); return (
              <div key={r.role_assignment_id} className="row" style={{ padding: "8px 10px", borderRadius: 10, background: held ? "var(--green-wash)" : "transparent" }}><b className="grow">{human(r.agent)}<Why f="agent" /></b><span className="sub">{fmtDay(r.valid_from)} → {r.valid_to ? fmtDay(r.valid_to) : "now"}<Why f="valid_to" /></span>{held && <Tag tone="green">held it that day</Tag>}{!r.valid_to ? null : <Tag tone="grey" f="is_current">closed, never deleted</Tag>}</div>); })}</div></Explains>); })}
          <p className="provenance">When a role changes hands the old row is closed with an end date and a new one opened. Nothing is overwritten, so a date question stays answerable.</p></>)}
        {tab === "words" && <><ExplainerNote />{words.rows?.map((w) => <Explains key={w.term_meaning_change_id} t="term_meaning_changes"><div className="card edge-amber"><div className="row"><b className="grow" style={{ fontSize: 17 }}>“{String(w.vocabulary_term).replace(/^vt-/, "")}”</b><Tag tone="grey" f="span_days">{w.span_days} days apart</Tag></div>
          <div className="quote" style={{ marginTop: 10, color: "var(--ink-3)", borderColor: "var(--line-2)" }}>{w.prior_meaning}<span className="by">from {fmtDay(w.prior_meaning_since)}<Why f="prior_meaning_since" /></span></div><div className="quote" style={{ marginTop: 8 }}>{w.new_meaning}<span className="by">since {fmtDay(w.changed_at)} · recorded by {human(w.recorded_by_agent)}<Why f="recorded_by_agent" /></span></div></div></Explains>)}</>}
      </div>
    </>
  );
}
