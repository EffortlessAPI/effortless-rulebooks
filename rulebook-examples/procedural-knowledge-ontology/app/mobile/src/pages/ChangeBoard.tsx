import { useState } from "react";
import { useRows } from "../session";
import { AppBar, Err, Loading, Tag, fmtDay, human, useQuickAction } from "../ui/kit";

export default function ChangeBoard() {
  const [tab, setTab] = useState<"stale" | "changes" | "questions" | "releases">("stale");
  return (
    <>
      <AppBar title="Change Board" />
      <div className="tabs">
        <button className="tab" aria-selected={tab === "stale"} onClick={() => setTab("stale")}>Four ways a book goes stale</button>
        <button className="tab" aria-selected={tab === "changes"} onClick={() => setTab("changes")}>Changes to the model</button>
        <button className="tab" aria-selected={tab === "questions"} onClick={() => setTab("questions")}>Questions it must still answer</button>
        <button className="tab" aria-selected={tab === "releases"} onClick={() => setTab("releases")}>Versions</button>
      </div>
      <div className="content">{tab === "stale" ? <Stale /> : tab === "changes" ? <Changes /> : tab === "questions" ? <Questions /> : <Releases />}</div>
    </>
  );
}

function Stale() {
  const words = useRows("term_meaning_changes", {}, "-changed_at"); const grow = useRows("model_expansion_requests", {}, "-requested_at");
  const deps = useRows("external_dependency_revisions", {}, "-affected_mapping_count"); const agents = useRows("role_assignments", { agent_kind: "AIAgent" }, "role,valid_from");
  return (
    <div className="grid c2">
      <div><div className="section">Words that moved <span className="count">{words.rows?.length ?? "·"}</span></div>
        {words.rows?.map((w) => <div key={w.term_meaning_change_id} className="card edge-amber"><div className="row"><b className="grow" style={{ fontSize: 17 }}>“{String(w.vocabulary_term).replace(/^vt-/, "")}”</b>{w.is_structural_change && <Tag tone="amber">structural change</Tag>}<Tag tone="grey">{w.span_days} days</Tag></div>
          <div className="quote" style={{ marginTop: 10, color: "var(--ink-3)", borderColor: "var(--line-2)" }}>{w.prior_meaning}<span className="by">meant this from {fmtDay(w.prior_meaning_since)}</span></div>
          <div className="quote" style={{ marginTop: 8 }}>{w.new_meaning}<span className="by">means this since {fmtDay(w.changed_at)} · recorded by {human(w.recorded_by_agent)}</span></div></div>)}
        <div className="section">Standards that moved underneath <span className="count">{deps.rows?.length ?? "·"}</span></div>
        {deps.rows?.map((d) => <div key={d.external_dependency_revision_id} className="card tight edge-amber"><div className="row"><b className="grow">{d.revision_label}</b><span className="big derived" style={{ fontSize: 22 }}>{d.affected_mapping_count}</span></div><div className="sub">{human(d.ontology_profile)} · {String(d.revision_kind).replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase()} · published {fmtDay(d.published_at)} · mappings affected</div></div>)}</div>
      <div><div className="section">Requests to grow <span className="count">{grow.rows?.length ?? "·"}</span></div>
        {grow.rows?.map((g) => <div key={g.model_expansion_request_id} className="card tight edge-amber"><b>{g.workflow_description}</b><div className="row wrap" style={{ marginTop: 6 }}><Tag tone="grey">{human(g.requesting_organization)}</Tag><Tag tone={g.fit_decision === "FitsExistingSchema" ? "green" : "purple"}>{g.fit_decision === "FitsExistingSchema" ? "fits the existing tables" : "needs new terms"}</Tag><span className="sub">decided by {human(g.decided_by_agent)}</span></div></div>)}
        <div className="section">Agents that changed version</div>
        {agents.rows?.map((a) => <div key={a.role_assignment_id} className="card tight"><div className="row"><b className="grow">{human(a.agent)}</b>{a.is_current ? <Tag tone="green">holds the role now</Tag> : <Tag tone="grey">ended {fmtDay(a.valid_to)}</Tag>}</div><div className="sub">{human(a.role)} · from {fmtDay(a.valid_from)}</div></div>)}
        <p className="provenance">None of these is an emergency. All four happen every quarter.</p></div>
    </div>);
}

function Changes() {
  const list = useRows("model_change_requests", {}, "model_change_request_id"); const quick = useQuickAction();
  if (!list.rows) return <Loading />;
  return (<><Err error={list.error} /><div className="grid c2">{list.rows.map((c) => {
    const red = c.authority_review_skipped || c.steward_own_change_unreviewed || c.accepted_without_test_run; const returned = c.status === "ReturnedForClarification";
    return (<div key={c.model_change_request_id} className={`card ${returned ? "" : red ? "edge-red" : "edge-green"}`} style={{ marginBottom: 0 }}>
      <div className="row top"><div className="grow"><div className="h">{c.title}</div><div className="sub">{c.stated_need}</div></div><Tag tone={c.status === "Deployed" ? "green" : returned ? "grey" : "amber"}>{String(c.status).replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase()}</Tag></div>
      <div className="row wrap" style={{ marginTop: 10 }}>
        {c.motivating_question ? <Tag tone="blue">tied to a question</Tag> : <Tag tone="red">no question behind it</Tag>}
        {c.coverage_checked_at && <Tag tone="green">coverage checked</Tag>}{c.has_authority_review && <Tag tone="green">authority review · {human(c.authority_agent)}</Tag>}
        {c.integrity_check_count > 0 && <Tag tone="green">integrity checks · {c.integrity_check_count}</Tag>}
        {c.steward_own_change_unreviewed && <Tag tone="solid-red">steward approved it alone</Tag>}{c.authority_review_skipped && <Tag tone="red">authority review skipped</Tag>}
        {c.accepted_without_test_run && <Tag tone="red">accepted without a test run</Tag>}{c.missed_altered_inferences && <Tag tone="red">altered {c.post_deploy_inference_count} inference, not assessed</Tag>}
      </div>
      {c.impact_assessment && <p className="sub" style={{ marginTop: 8 }}>{c.impact_assessment}</p>}
      {c.authority_review_skipped && c.status !== "Deployed" && <div className="row" style={{ marginTop: 12 }}><span className="grow sub">Not shipped yet. A review can still happen before it does.</span><button className="btn sm accent" onClick={() => quick("act-authority-record-review", { key: c.model_change_request_id, watch: c.model_change_request_id })}>Record authority review</button></div>}
    </div>); })}</div></>);
}

function Questions() {
  const runs = useRows("competency_question_runs", {}, "cq_set_entry,ran_at"); const reviews = useRows("competency_question_reviews", {}, "-reviewed_at");
  const releases = useRows("rulebook_releases", {}, "issued_at");
  if (!runs.rows || !releases.rows) return <Loading />;
  const cols = releases.rows.filter((r) => runs.rows!.some((x) => x.rulebook_release === r.rulebook_release_id));
  const qs = [...new Set(runs.rows.map((r) => r.cq_set_entry))];
  return (<div className="card"><p className="sub" style={{ marginBottom: 10 }}>The book was built from questions real roles needed answered. They are kept as its test suite: one row for each question, one column for each release.</p>
    <table className="t"><thead><tr><th>Question</th>{cols.map((c) => <th key={c.rulebook_release_id}>{c.rulebook_version}</th>)}</tr></thead><tbody>
      {qs.map((q) => <tr key={q}><td><b>{String(q).replace("cqe-pko-", "question ")}</b></td>{cols.map((c) => { const r = runs.rows!.find((x) => x.cq_set_entry === q && x.rulebook_release === c.rulebook_release_id);
        return <td key={c.rulebook_release_id}>{!r ? <span className="cell-na">·</span> : !r.was_answerable ? <Tag tone="grey">could not be answered</Tag> : r.answer_outcome === "Correct" ? <span className="cell-ok">✓ correct</span> : <Tag tone="solid-red">✗ wrong · regression</Tag>}</td>; })}</tr>)}
    </tbody></table>
    <div className="section" style={{ marginLeft: 0 }}>Reviews</div>{reviews.rows?.map((r) => <div key={r.competency_question_review_id} className="sub" style={{ padding: "4px 0" }}><b>{fmtDay(r.reviewed_at)}</b> · {human(r.reviewed_by_agent)} · {r.outcome}</div>)}</div>);
}

function Releases() {
  const releases = useRows("rulebook_releases", {}, "issued_at"); const data = useRows("instance_data_versions", {}, "snapshot_at");
  return (<div className="grid c2"><div><div className="section">The model: changes need an authority</div>{releases.rows?.map((r) => <div key={r.rulebook_release_id} className={`card tight ${r.is_current ? "edge-green" : ""}`}><div className="row"><b className="grow" style={{ fontSize: 17 }}>{r.rulebook_version}</b>{r.declared_scale && <Tag tone={r.declared_scale === "Major" ? "red" : r.declared_scale === "Minor" ? "purple" : "grey"}>{String(r.declared_scale).toLowerCase()}</Tag>}{r.is_increment_inconsistent_with_scale && <Tag tone="solid-red">number does not match the scale</Tag>}</div><div className="sub">{fmtDay(r.issued_at)} · {r.changelog}</div></div>)}</div>
    <div><div className="section">The data: versioned separately, and much faster</div>{data.rows?.map((d) => <div key={d.instance_data_version_id} className="card tight"><div className="row"><b className="grow fact">{d.data_version_label}</b><Tag tone="grey">conforms to {String(d.conforms_to_release).replace("pko-release-", "")}</Tag></div><div className="sub">{fmtDay(d.snapshot_at)}</div></div>)}
      <p className="provenance">Recording that a technician finished a step is data: it needs a sign-in, not an authority. Changing a rule is the model: it needs a review and a release. Saving a session from the app adds a data version here.</p></div></div>);
}
