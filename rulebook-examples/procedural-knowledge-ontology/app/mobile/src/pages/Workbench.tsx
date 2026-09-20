import { useState } from "react";
import * as api from "../api";
import { useRows } from "../session";
import { ActionSheet, AppBar, Err, Loading, Tag, fmtDay, fmtTime, human, useQuickAction, yn } from "../ui/kit";

type Tab = "board" | "collect" | "organize" | "encode" | "searches" | "changes";
const PROCEDURE = "lockout-tagout"; const VERSION = "loto-v2.0.0";

export default function Workbench() {
  const [tab, setTab] = useState<Tab>("board");
  const board = useRows("know_how_carriers", {}, "-is_at_risk_of_imminent_loss,-is_held_only_by_departed,days_until_holder_departure");
  const risk = board.rows?.some((k) => k.is_at_risk_of_imminent_loss);
  return (
    <>
      <AppBar title="Capture Workbench" />
      <div className="tabs">
        <button className="tab" aria-selected={tab === "board"} onClick={() => setTab("board")}>Know-how<span className={`n ${risk ? "red" : ""}`}>{board.rows?.length ?? "·"}</span></button>
        <button className="tab" aria-selected={tab === "collect"} onClick={() => setTab("collect")}>1 · Collect</button>
        <button className="tab" aria-selected={tab === "organize"} onClick={() => setTab("organize")}>2 · Organize</button>
        <button className="tab" aria-selected={tab === "encode"} onClick={() => setTab("encode")}>3 · Encode</button>
        <button className="tab" aria-selected={tab === "searches"} onClick={() => setTab("searches")}>Searches that failed</button>
        <button className="tab" aria-selected={tab === "changes"} onClick={() => setTab("changes")}>Decided changes</button>
      </div>
      <div className="content">
        {tab === "board" && <Board board={board} />}
        {tab === "collect" && <Collect />}
        {tab === "organize" && <Organize />}
        {tab === "encode" && <Encode />}
        {tab === "searches" && <Searches />}
        {tab === "changes" && <Changes />}
      </div>
    </>
  );
}

function Board({ board }: { board: ReturnType<typeof useRows> }) {
  const [write, setWrite] = useState<api.Row | null>(null); const [hand, setHand] = useState<api.Row | null>(null);
  if (!board.rows) return <Loading />;
  return (
    <>
      <p className="sub" style={{ margin: "6px 4px 14px", maxWidth: 760 }}>One card for each skill at ACME that lives in a person or a place. Red can still be saved. A black band is already too late.</p>
      <Err error={board.error} />
      <div className="grid c3">
        {board.rows.map((k) => (
          <div key={k.know_how_carrier_id} className="card" style={{ marginBottom: 0 }}>
            {k.is_at_risk_of_imminent_loss ? <div className="banner red">● At risk of imminent loss · {k.days_until_holder_departure} days</div>
              : k.is_held_only_by_departed ? <div className="banner black">Held only by someone who has left</div>
              : k.is_captured ? <div className="banner green">✓ Captured</div> : null}
            <div className="h">{k.topic}</div>
            <div className="sub" style={{ marginBottom: 10 }}>{k.holder_agent ? `${human(k.holder_agent)} · ${k.holder_tenure_years} years` : `${k.carrier_kind} · ${human(k.holder_facility)}`}</div>
            <dl className="kv">
              <dt>In the written procedure</dt><dd className="fact">{yn(k.is_in_written_procedure)}</dd>
              <dt>Repository entries</dt><dd className={k.repository_entry_count ? "derived" : "bad"}>{k.repository_entry_count}</dd>
              <dt>Captured</dt><dd className={k.is_captured ? "derived" : "bad"}>{yn(k.is_captured)}</dd>
              <dt>Passed on to</dt><dd className={k.transfer_count ? "derived" : "bad"}>{k.transfer_count}</dd>
              {k.holder_departure_at && <><dt>Holder departs</dt><dd className="fact">{fmtDay(k.holder_departure_at)}</dd></>}
            </dl>
            {k.holder_is_still_engaged && <div className="row" style={{ marginTop: 12 }}>
              <button className="btn sm accent grow" onClick={() => setWrite(k)}>Write this down</button>
              <button className="btn sm ghost grow" onClick={() => setHand(k)}>Record a hand-over</button></div>}
          </div>))}
      </div>
      {write && <ActionSheet actionId="act-ke-write-down" context={{ KnowHow: write.know_how_carrier_id, Procedure: write.procedure }} watch={write.know_how_carrier_id}
        initial={{ SourceExpert: write.holder_agent }} onClose={() => setWrite(null)} />}
      {hand && <ActionSheet actionId="act-ke-hand-over" context={{ KnowHow: hand.know_how_carrier_id, FromAgent: hand.holder_agent, CommunityOfPractice: hand.community_of_practice }}
        watch={hand.know_how_carrier_id} choiceFilter={(_f, r) => r.agent_kind === "Human" && r.agent_id !== hand.holder_agent && r.organization === hand.organization} onClose={() => setHand(null)} />}
    </>
  );
}

function Collect() {
  const sessions = useRows("elicitation_sessions", { procedure_version: VERSION }, "started_at");
  const gaps = useRows("knowledge_gaps", { procedure_version: VERSION }, "identified_at");
  const brokers = useRows("knowledge_broker_links", {});
  const [sel, setSel] = useState<string | null>(null);
  const s = sessions.rows?.find((x) => x.elicitation_session_id === sel) || sessions.rows?.[0];
  return (
    <>
      <div className="section">Sessions on lockout/tagout <span className="count">{sessions.rows?.length ?? "·"}</span></div>
      <Err error={sessions.error} />
      <div className="timeline">{sessions.rows?.map((x) => (
        <button key={x.elicitation_session_id} className="chip" aria-pressed={s?.elicitation_session_id === x.elicitation_session_id} onClick={() => setSel(x.elicitation_session_id)}>
          <b>{x.observer_stance === "LegitimatePeripheralParticipation" ? "Working alongside" : human(x.method).replace(/([a-z])([A-Z])/g, "$1 $2")}</b><span>{fmtDay(x.started_at)} · {human(x.practitioner_agent)}</span></button>))}</div>
      {s && <Session s={s} />}
      <div className="section">People who will not tell, and other gaps</div>
      <div className="grid c3">{gaps.rows?.map((g) => (
        <div key={g.knowledge_gap_id} className={`card ${g.status === "Resolved" ? "edge-green" : g.status === "Open" ? "edge-red" : "edge-amber"}`} style={{ marginBottom: 0 }}>
          <div className="row wrap" style={{ marginBottom: 8 }}><Tag tone={g.status === "Resolved" ? "green" : g.status === "Open" ? "red" : "amber"}>{g.status.toLowerCase()}</Tag>{g.gap_cause && <Tag tone="grey">{g.gap_cause.toLowerCase()}</Tag>}<Tag tone="grey">{g.severity.toLowerCase()}</Tag></div>
          <div className="quote" style={{ color: "var(--ink)" , borderColor: "var(--line-2)"}}>{g.statement}</div>
          {g.siloed_within && <p className="sub" style={{ marginTop: 8 }}>Kept within: <b>{g.siloed_within}</b></p>}
          {g.resolution_plan && <p className="sub" style={{ marginTop: 4 }}>Plan: {g.resolution_plan}</p>}
          {g.codified_as_fragment && <p className="derived" style={{ marginTop: 6, fontWeight: 650, fontSize: 13 }}>Drawn out by {human(g.drawn_out_by_session)} and written down.</p>}
        </div>))}</div>
      <div className="section">Who people really ask</div>
      <div className="card"><table className="t"><thead><tr><th>Who asks</th><th>Goes to</th><th>About</th><th>How often</th></tr></thead><tbody>
        {brokers.rows?.map((b) => <tr key={b.knowledge_broker_link_id}><td>{human(b.seeker)}</td><td><b>{human(b.broker)}</b>{b.is_at_risk_reliance && <> <Tag tone="red">broker has left</Tag></>}</td><td>{human(String(b.topic).replace(/^vt-/, ""))}</td><td>{b.frequency}</td></tr>)}
      </tbody></table><p className="provenance">None of these lines is on an organization chart. A person several others depend on is a knowledge broker, and a single point of failure.</p></div>
    </>
  );
}

function Session({ s }: { s: api.Row }) {
  const id = s.elicitation_session_id;
  const fragments = useRows("knowledge_fragments", { elicitation_session: id });
  const incidents = useRows("critical_incidents", { elicitation_session: id });
  const rungs = useRows("concept_ladder_rungs", { elicitation_session: id }, "-ladder_level");
  const grid = useRows("repertory_grid_constructs", { elicitation_session: id });
  const divergences = useRows("workflow_view_divergences", { elicitation_session: id });
  return (
    <div className="card">
      <div className="row top"><div className="grow"><div className="h">{s.summary}</div><div className="sub">{fmtTime(s.started_at)} to {new Date(s.ended_at).toLocaleTimeString("en-US", { hour: "numeric", minute: "2-digit" })} · practitioner {human(s.practitioner_agent)} · facilitator {human(s.facilitator_agent)} · {String(s.setting || "").replace("InSitu", "on the floor")}</div></div>
        <div style={{ textAlign: "right" }}><div className="big derived">{s.valid_fragments_produced ?? 0}</div><div className="sub">usable pieces of knowledge</div></div></div>
      {incidents.rows?.map((i) => (<div key={i.critical_incident_id} style={{ marginTop: 14 }}><Tag tone={i.outcome === "WentBadly" ? "red" : "green"}>{fmtTime(i.occurred_at)} · {String(i.outcome).replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase()}</Tag>
        <div className="quote" style={{ marginTop: 8 }}>“{i.account}”<span className="by">told by {human(i.narrator)}</span></div>{i.revealed_judgment && <p className="derived" style={{ fontWeight: 650, marginTop: 8 }}>What it revealed: {i.revealed_judgment}</p>}</div>))}
      {rungs.rows && rungs.rows.length > 0 && <div style={{ marginTop: 14, maxWidth: 560 }}>{["loto-04b", "loto-05"].map((step) => { const rs = rungs.rows!.filter((r) => r.step === step); if (!rs.length) return null; return (<div key={step} style={{ marginBottom: 14 }}>
        {rs.filter((r) => r.ladder_level > 0).map((r) => <div key={r.concept_ladder_rung_id} className={`rung ${r.is_ultimate_goal ? "value" : ""}`} style={{ width: `${100 - r.ladder_level * 8}%` }}><small>why? · {r.rung_kind}</small>{r.statement}</div>)}
        <div className="rung start"><small>the step</small>{human(step)}</div>
        {rs.filter((r) => r.ladder_level < 0).map((r) => <div key={r.concept_ladder_rung_id} className="rung" style={{ width: "92%" }}><small>how? · {r.rung_kind}</small>{r.statement}</div>)}</div>); })}</div>}
      {grid.rows && grid.rows.length > 0 && <table className="t" style={{ marginTop: 14 }}><thead><tr><th>How {human(s.practitioner_agent).split(" ")[0]} tells the jobs apart</th><th>Said without being asked</th><th>Actually separates the jobs</th></tr></thead><tbody>
        {grid.rows.map((g) => <tr key={g.repertory_grid_construct_id} style={!g.separates_situations ? { opacity: .55 } : undefined}><td className="fact"><b>{g.pole_a}</b> / {g.pole_b}</td><td className={g.was_stated_unprompted ? "cell-ok" : "cell-na"}>{yn(g.was_stated_unprompted)}</td><td className={g.separates_situations ? "cell-ok" : "cell-bad"}>{yn(g.separates_situations)}</td></tr>)}</tbody></table>}
      {divergences.rows?.map((d) => (<div key={d.workflow_view_divergence_id} style={{ marginTop: 14 }}><div className="sub" style={{ fontWeight: 700 }}>{human(d.step)}: two versions</div>
        <div className="grid c2" style={{ marginTop: 6 }}><div className="quote">“{d.view_a}”<span className="by">{human(d.holder_a)}</span></div><div className="quote">“{d.view_b}”<span className="by">{human(d.holder_b)}</span></div></div>
        {d.is_reconciled ? <p className="derived" style={{ fontWeight: 650, marginTop: 8 }}>Agreed: {d.reconciled_statement}</p> : <div style={{ marginTop: 8 }}><Tag tone="amber">not reconciled</Tag></div>}</div>))}
      {fragments.rows && fragments.rows.length > 0 && <><div className="section" style={{ marginLeft: 0 }}>What it produced</div>{fragments.rows.map((f) => <div key={f.knowledge_fragment_id} className="quote" style={{ marginBottom: 8 }}>“{f.statement}”<span className="by">{f.knowledge_form} · {f.status.toLowerCase()} · attached to {human(f.step)}</span></div>)}</>}
    </div>);
}

function Organize() {
  const terms = useRows("vocabulary_terms", {}, "vocabulary,pref_label");
  const variants = useRows("term_label_variants", {});
  const lenses = useRows("stakeholder_lenses", {}, "-granularity_rank");
  const [q, setQ] = useState("");
  // The search is a WHERE on the label table, not a filter in the browser: the vocabulary answers.
  const found = useRows(q.trim() ? "term_label_variants" : null, { wording: q.trim().toLowerCase() });
  const hit = q.trim() ? found.rows : null;
  return (
    <div className="grid c2">
      <div>
        <div className="section">One word for one thing</div>
        <div className="card"><label className="field" style={{ marginBottom: 8 }}><span>Search the vocabulary the way people actually say it</span><input type="text" placeholder="try: loto" value={q} onChange={(e) => setQ(e.target.value)} /></label>
          {hit && (hit.length ? hit.map((v) => <div key={v.term_label_variant_id} className="row" style={{ padding: "6px 2px" }}><b className="derived grow">{v.term_pref_label}</b><span className="sub">{v.label_kind === "alt" ? `matched alternative label: ${v.wording}` : "preferred label"}</span></div>) : <div className="sub">No term carries that wording. That is a gap to report.</div>)}</div>
        {terms.rows?.filter((t) => t.vocabulary === "voc-lockout-activities").map((t) => (
          <div key={t.vocabulary_term_id} className="card tight"><div className="row"><b className="fact grow">{t.pref_label}</b><Tag tone="grey">preferred</Tag></div>
            <div className="row wrap" style={{ marginTop: 6 }}>{variants.rows?.filter((v) => v.vocabulary_term === t.vocabulary_term_id && v.label_kind === "alt").map((v) => <Tag key={v.term_label_variant_id} tone="blue">also said: {v.wording}</Tag>)}</div>
            {t.definition && <p className="sub" style={{ marginTop: 6 }}>{t.definition}</p>}</div>))}
      </div>
      <div>
        <div className="section">One record, many readers <span className="count">{lenses.rows?.length ?? "·"}</span></div>
        {lenses.rows?.map((l) => (<div key={l.stakeholder_lens_id} className="card tight"><div className="row"><b className="grow">{l.label}</b><Tag tone="purple">{String(l.required_granularity).toLowerCase()} level</Tag><Tag tone="grey">{String(l.preferred_form).toLowerCase()}</Tag></div>
          <div className="row wrap" style={{ marginTop: 6 }}>{l.needs_step_guidance && <Tag tone="blue">step guidance</Tag>}{l.needs_metrics && <Tag tone="blue">metrics</Tag>}{l.needs_exception_handling && <Tag tone="blue">exceptions</Tag>}{l.needs_compliance_evidence && <Tag tone="blue">evidence</Tag>}{l.needs_structured_constraints && <Tag tone="blue">structured constraints</Tag>}</div></div>))}
        <p className="provenance">The same knowledge, shown at a different height and in a different form for whoever signed in. It is written once.</p>
      </div>
    </div>);
}

function Encode() {
  const steps = useRows("steps", { procedure_version: VERSION }, "step_number");
  const [sel, setSel] = useState("loto-06"); const [adding, setAdding] = useState(false);
  const conditions = useRows("step_conditions", { step: sel }); const decisions = useRows("decision_points", { step: sel });
  const failures = useRows("failure_modes", { step: sel }); const cues = useRows("step_cues", { step: sel });
  const fragments = useRows("knowledge_fragments", { step: sel }); const traces = useRows("knowledge_traces", { step: sel });
  return (
    <div className="split">
      <div className="pane card" style={{ padding: 8 }}>{steps.rows?.map((s) => <button key={s.step_id} className="person" style={{ boxShadow: "none", marginBottom: 2, background: sel === s.step_id ? "var(--blue-wash)" : "transparent", marginLeft: s.parent_step ? 16 : 0, width: s.parent_step ? "calc(100% - 16px)" : "100%" }} onClick={() => setSel(s.step_id)}><span className="grow"><span className="nm" style={{ fontSize: 14 }}>{s.step_number} · {s.title}</span></span></button>)}</div>
      <div>
        <div className="row" style={{ margin: "4px 4px 12px" }}><div className="grow"><div className="h" style={{ fontSize: 20 }}>{human(sel)}, opened up</div><div className="sub">Written so that a machine can use it as well as a person.</div></div><button className="btn sm accent" onClick={() => setAdding(true)}>＋ Add a warning sign</button></div>
        <div className="grid c2">
          <div className="card" style={{ marginBottom: 0 }}><div className="section" style={{ margin: "0 0 8px" }}>Conditions</div>{conditions.rows?.length ? conditions.rows.map((c) => <p key={c.step_condition_id} style={{ marginBottom: 6 }}><Tag tone="purple">{c.condition_kind === "Postcondition" ? "true afterwards" : c.condition_kind === "Precondition" ? "true first" : "stays true"}</Tag> {c.statement}</p>) : <span className="sub">None recorded.</span>}</div>
          <div className="card" style={{ marginBottom: 0 }}><div className="section" style={{ margin: "0 0 8px" }}>Decision points</div>{decisions.rows?.length ? decisions.rows.map((d) => <p key={d.decision_point_id} style={{ marginBottom: 6 }}>{d.question || String(d.name).split(": ").slice(1).join(": ")}</p>) : <span className="sub">None recorded.</span>}</div>
          <div className="card" style={{ marginBottom: 0 }}><div className="section" style={{ margin: "0 0 8px" }}>Failure modes</div>{failures.rows?.length ? failures.rows.map((f) => <div key={f.failure_mode_id} style={{ marginBottom: 10 }}><b>{f.description}</b><div className="sub">Response: {f.response}</div>{f.escalates_to_vacant_role && <Tag tone="red">escalates to a role nobody holds</Tag>}</div>) : <span className="sub">None recorded.</span>}</div>
          <div className="card" style={{ marginBottom: 0 }}><div className="section" style={{ margin: "0 0 8px" }}>Warning signs</div>{cues.rows?.length ? cues.rows.map((c) => <div key={c.step_cue_id} className={`cue ${c.signals_incomplete_step ? "danger" : ""}`} style={{ marginTop: 0, marginBottom: 8 }}><div className="what">{c.description}</div><div className="row wrap" style={{ marginTop: 6 }}>{c.signals_incomplete_step && <Tag tone="red">step not finished</Tag>}{c.requires_escalation && <Tag tone="amber">escalate to {human(c.escalate_to_role)}</Tag>}{c.is_unanswerable_sign && <Tag tone="solid-red">no recorded response</Tag>}</div></div>) : <span className="sub">None recorded.</span>}</div>
        </div>
        <div className="section">Knowledge attached here, and where it came from</div>
        {fragments.rows?.map((f) => <div key={f.knowledge_fragment_id} className="card tight"><div className="quote">“{f.statement}”<span className="by">{f.knowledge_form} · {f.status.toLowerCase()} · from {human(f.source_agent)} · session {human(f.elicitation_session)}</span></div></div>)}
        {traces.rows?.map((t) => <div key={t.knowledge_trace_id} className="card tight"><div className="sub"><b>{t.traced_aspect}</b> traced to {human(t.source_material)} ({String(t.source_material_kind || "").toLowerCase()}) · derived by {human(t.derived_by_agent)}{t.validated_by_agent ? ` · validated by ${human(t.validated_by_agent)}` : ""}</div>{t.source_statement && <div className="quote" style={{ marginTop: 6 }}>“{t.source_statement}”</div>}</div>)}
      </div>
      {adding && <ActionSheet actionId="act-ke-add-warning-sign" context={{ Step: sel }} watch={sel} choiceFilter={(_f, r) => !!r.current_agent} onClose={() => setAdding(false)} />}
    </div>);
}

function Searches() {
  const searches = useRows("knowledge_search_events", {}, "-searched_at");
  return (<div className="card"><p className="sub" style={{ marginBottom: 10 }}>Every failed search is somebody telling the book what it is missing.</p><table className="t"><thead><tr><th>Who</th><th>When</th><th>Typed</th><th>Where</th><th>What happened</th></tr></thead><tbody>
    {searches.rows?.map((s) => <tr key={s.knowledge_search_event_id}><td>{human(s.searched_by_agent)}</td><td>{fmtTime(s.searched_at)}</td><td className="fact"><b>{s.query_text}</b></td><td>{String(s.channel).replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase()}</td>
      <td>{s.found_nothing_useful ? <Tag tone="red">found nothing useful{s.seconds_before_abandoning ? ` · gave up after ${s.seconds_before_abandoning}s` : ""}</Tag> : <Tag tone="green">opened a result · {s.dwell_seconds}s</Tag>}{s.linked_usability_barrier && <div className="sub" style={{ marginTop: 4 }}>{s.linked_usability_barrier}</div>}</td></tr>)}
  </tbody></table></div>);
}

function Changes() {
  const changes = useRows("change_requests", { is_decided: true }, "-decided_at");
  const quick = useQuickAction();
  return (<><p className="sub" style={{ margin: "6px 4px 12px" }}>Deciding a change and making it are two things. A decided request stays open until the steward has actually made it.</p>
    <div className="grid c2">{changes.rows?.map((c) => (<div key={c.change_request_id} className={`card ${c.is_open ? "edge-amber" : "edge-green"}`} style={{ marginBottom: 0 }}>
      <div className="h">{c.title}</div><div className="sub">{human(c.procedure_version)} · {c.status.toLowerCase()} {fmtDay(c.decided_at)}</div>
      <div className="row wrap" style={{ marginTop: 10 }}><Tag tone={c.is_open ? "amber" : "green"}>{c.is_open ? "open: decided, not yet made" : "closed"}</Tag><span className="grow" />
        {c.is_open && <button className="btn sm accent" onClick={() => quick("act-ke-mark-implemented", { key: c.change_request_id, watch: c.change_request_id })}>Mark implemented</button>}</div></div>))}</div></>);
}
