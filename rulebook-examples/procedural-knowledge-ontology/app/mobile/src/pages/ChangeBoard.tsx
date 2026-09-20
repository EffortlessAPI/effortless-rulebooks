import { useState } from "react";
import { useRows } from "../session";
import { AppBar, Err, ExplainerNote, Explains, Loading, Tag, Th, Val, Why, fmtDay, human, useQuickAction } from "../ui/kit";

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
      <div><ExplainerNote /><div className="section">Words that moved <span className="count">{words.rows?.length ?? "·"}</span></div>
        {words.rows?.map((w) => <Explains key={w.term_meaning_change_id} t="term_meaning_changes"><div className="card edge-amber"><div className="row"><b className="grow" style={{ fontSize: 17 }}>“{String(w.vocabulary_term).replace(/^vt-/, "")}”</b>{w.is_structural_change && <Tag tone="amber" f="is_structural_change">structural change</Tag>}<Tag tone="grey" f="span_days">{w.span_days} days</Tag></div>
          <div className="quote" style={{ marginTop: 10, color: "var(--ink-3)", borderColor: "var(--line-2)" }}>{w.prior_meaning}<span className="by">meant this from {fmtDay(w.prior_meaning_since)}<Why f="prior_meaning_since" /></span></div>
          <div className="quote" style={{ marginTop: 8 }}>{w.new_meaning}<span className="by">means this since {fmtDay(w.changed_at)}<Why f="changed_at" /> · recorded by {human(w.recorded_by_agent)}</span></div></div></Explains>)}
        <div className="section">Standards that moved underneath <span className="count">{deps.rows?.length ?? "·"}</span></div>
        {deps.rows?.map((d) => <Explains key={d.external_dependency_revision_id} t="external_dependency_revisions"><div className="card tight edge-amber"><div className="row"><b className="grow">{d.revision_label}</b><span className="big derived" style={{ fontSize: 22 }}>{d.affected_mapping_count}</span><Why f="affected_mapping_count" /></div><div className="sub">{human(d.ontology_profile)} · {String(d.revision_kind).replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase()}<Why f="revision_kind" /> · published {fmtDay(d.published_at)} · mappings affected</div></div></Explains>)}</div>
      <div><div className="section">Requests to grow <span className="count">{grow.rows?.length ?? "·"}</span></div>
        {grow.rows?.map((g) => <Explains key={g.model_expansion_request_id} t="model_expansion_requests"><div className="card tight edge-amber"><b>{g.workflow_description}</b><div className="row wrap" style={{ marginTop: 6 }}><Tag tone="grey" f="requesting_organization">{human(g.requesting_organization)}</Tag><Tag tone={g.fit_decision === "FitsExistingSchema" ? "green" : "purple"} f="fit_decision">{g.fit_decision === "FitsExistingSchema" ? "fits the existing tables" : "needs new terms"}</Tag><span className="sub">decided by {human(g.decided_by_agent)}</span></div></div></Explains>)}
        <div className="section">Agents that changed version</div>
        {agents.rows?.map((a) => <Explains key={a.role_assignment_id} t="role_assignments"><div className="card tight"><div className="row"><b className="grow">{human(a.agent)}</b>{a.is_current ? <Tag tone="green" f="is_current">holds the role now</Tag> : <Tag tone="grey" f="valid_to">ended {fmtDay(a.valid_to)}</Tag>}</div><div className="sub">{human(a.role)} · from {fmtDay(a.valid_from)}<Why f="valid_from" /></div></div></Explains>)}
        <p className="provenance">None of these is an emergency. All four happen every quarter.</p></div>
    </div>);
}

function Changes() {
  const list = useRows("model_change_requests", {}, "model_change_request_id"); const quick = useQuickAction();
  if (!list.rows) return <Loading />;
  return (<><ExplainerNote /><Err error={list.error} /><div className="grid c2">{list.rows.map((c) => {
    const red = c.authority_review_skipped || c.steward_own_change_unreviewed || c.accepted_without_test_run; const returned = c.status === "ReturnedForClarification";
    return (<Explains key={c.model_change_request_id} t="model_change_requests"><div className={`card ${returned ? "" : red ? "edge-red" : "edge-green"}`} style={{ marginBottom: 0 }}>
      <div className="row top"><div className="grow"><div className="h">{c.title}</div><div className="sub">{c.stated_need}<Why f="stated_need" /></div></div><Tag tone={c.status === "Deployed" ? "green" : returned ? "grey" : "amber"} f="status">{String(c.status).replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase()}</Tag></div>
      <div className="row wrap" style={{ marginTop: 10 }}>
        {c.motivating_question ? <Tag tone="blue" f="motivating_question">tied to a question</Tag> : <Tag tone="red" f="motivating_question">no question behind it</Tag>}
        {c.coverage_checked_at && <Tag tone="green" f="coverage_checked_at">coverage checked</Tag>}{c.has_authority_review && <Tag tone="green" f="has_authority_review">authority review · {human(c.authority_agent)}</Tag>}
        {c.integrity_check_count > 0 && <Tag tone="green" f="integrity_check_count">integrity checks · {c.integrity_check_count}</Tag>}
        {c.steward_own_change_unreviewed && <Tag tone="solid-red" f="steward_own_change_unreviewed">steward approved it alone</Tag>}{c.authority_review_skipped && <Tag tone="red" f="authority_review_skipped">authority review skipped</Tag>}
        {c.accepted_without_test_run && <Tag tone="red" f="accepted_without_test_run">accepted without a test run</Tag>}{c.missed_altered_inferences && <Tag tone="red" f="missed_altered_inferences">altered {c.post_deploy_inference_count} inference, not assessed</Tag>}
      </div>
      {c.impact_assessment && <p className="sub" style={{ marginTop: 8 }}>{c.impact_assessment}<Why f="impact_assessment" /></p>}
      {c.authority_review_skipped && c.status !== "Deployed" && <div className="row" style={{ marginTop: 12 }}><span className="grow sub">Not shipped yet. A review can still happen before it does.</span><button className="btn sm accent" onClick={() => quick("act-authority-record-review", { key: c.model_change_request_id, watch: c.model_change_request_id })}>Record authority review</button></div>}
    </div></Explains>); })}</div></>);
}

function Questions() {
  const runs = useRows("competency_question_runs", {}, "cq_set_entry,ran_at"); const reviews = useRows("competency_question_reviews", {}, "-reviewed_at");
  const releases = useRows("rulebook_releases", {}, "issued_at");
  if (!runs.rows || !releases.rows) return <Loading />;
  const cols = releases.rows.filter((r) => runs.rows!.some((x) => x.rulebook_release === r.rulebook_release_id));
  const qs = [...new Set(runs.rows.map((r) => r.cq_set_entry))];
  return (<div className="card"><p className="sub" style={{ marginBottom: 10 }}>The book was built from questions real roles needed answered. They are kept as its test suite: one row for each question, one column for each release.</p>
    <ExplainerNote />
    <Explains t="competency_question_runs"><table className="t"><thead><tr><Th f="cq_set_entry">Question</Th>{cols.map((c) => <Th key={c.rulebook_release_id} t="rulebook_releases" f="rulebook_version">{c.rulebook_version}</Th>)}</tr></thead><tbody>
      {qs.map((q) => <tr key={q}><td><b>{String(q).replace("cqe-pko-", "question ")}</b></td>{cols.map((c) => { const r = runs.rows!.find((x) => x.cq_set_entry === q && x.rulebook_release === c.rulebook_release_id);
        return <td key={c.rulebook_release_id}>{!r ? <span className="cell-na">·</span> : !r.was_answerable ? <Tag tone="grey" f="was_answerable">could not be answered</Tag> : r.answer_outcome === "Correct" ? <span className="cell-ok">✓ correct<Why f="answer_outcome" /></span> : <Tag tone="solid-red" f="answer_outcome">✗ wrong · regression</Tag>}</td>; })}</tr>)}
    </tbody></table></Explains>
    <div className="section" style={{ marginLeft: 0 }}>Reviews</div>{reviews.rows?.map((r) => <div key={r.competency_question_review_id} className="sub" style={{ padding: "4px 0" }}><b>{fmtDay(r.reviewed_at)}</b> · {human(r.reviewed_by_agent)} · {r.outcome}<Why t="competency_question_reviews" f="outcome" /></div>)}</div>);
}

function Releases() {
  const releases = useRows("rulebook_releases", {}, "issued_at"); const data = useRows("instance_data_versions", {}, "snapshot_at");
  return (<div className="grid c2"><div><ExplainerNote /><div className="section">The model: changes need an authority</div>{releases.rows?.map((r) => <Explains key={r.rulebook_release_id} t="rulebook_releases"><div className={`card tight ${r.is_current ? "edge-green" : ""}`}><div className="row"><b className="grow" style={{ fontSize: 17 }}>{r.rulebook_version}<Why f="rulebook_version" /></b>{r.declared_scale && <Tag tone={r.declared_scale === "Major" ? "red" : r.declared_scale === "Minor" ? "purple" : "grey"} f="declared_scale">{String(r.declared_scale).toLowerCase()}</Tag>}{r.is_increment_inconsistent_with_scale && <Tag tone="solid-red" f="is_increment_inconsistent_with_scale">number does not match the scale</Tag>}</div><div className="sub">{fmtDay(r.issued_at)} · {r.changelog}</div></div></Explains>)}</div>
    <div><div className="section">The data: versioned separately, and much faster</div>{data.rows?.map((d) => <Explains key={d.instance_data_version_id} t="instance_data_versions"><div className="card tight"><div className="row"><b className="grow fact">{d.data_version_label}<Why f="data_version_label" /></b><Tag tone="grey" f="conforms_to_release">conforms to {String(d.conforms_to_release).replace("pko-release-", "")}</Tag></div><div className="sub">{fmtDay(d.snapshot_at)}</div></div></Explains>)}
      <p className="provenance">Recording that a technician finished a step is data: it needs a sign-in, not an authority. Changing a rule is the model: it needs a review and a release. Saving a session from the app adds a data version here.</p></div></div>);
}
