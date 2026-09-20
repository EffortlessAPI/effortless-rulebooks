import { useEffect, useState } from "react";
import * as api from "../api";
import { useRows, useSession } from "../session";
import { ActionSheet, AppBar, Err, Loading, Tag, fmtDay, fmtTime, human } from "../ui/kit";

type Tab = "coverage" | "actions" | "context" | "story";
export default function Admin() {
  const [tab, setTab] = useState<Tab>("coverage");
  return (
    <>
      <AppBar title="Admin" eyebrow="administrator" />
      <div className="tabs">
        <button className="tab" aria-selected={tab === "coverage"} onClick={() => setTab("coverage")}>Where this came from</button>
        <button className="tab" aria-selected={tab === "actions"} onClick={() => setTab("actions")}>What each role may change</button>
        <button className="tab" aria-selected={tab === "context"} onClick={() => setTab("context")}>The date answers are judged against</button>
        <button className="tab" aria-selected={tab === "story"} onClick={() => setTab("story")}>Save · Reset</button>
        <span className="grow" /><a className="tab" href="http://localhost:5174" target="_blank" rel="noreferrer">Read-only register, explorer, conformance, access ↗</a>
      </div>
      <div className="content">{tab === "coverage" ? <Coverage /> : tab === "actions" ? <Actions /> : tab === "context" ? <Context /> : <Story />}</div>
    </>
  );
}

function Coverage() {
  const articles = useRows("source_articles", {}, "source_article_id"); const [sel, setSel] = useState<string | null>(null); const [only, setOnly] = useState(false);
  const claims = useRows(sel ? "article_claims" : null, only ? { source_article: sel ?? "", is_covered: false } : { source_article: sel ?? "" }, "article_claim_id");
  const [open, setOpen] = useState<string | null>(null);
  if (!articles.rows) return <Loading />;
  return (
    <>
      <p className="sub" style={{ margin: "6px 4px 14px", maxWidth: 860 }}>The model was built from five essays by Jessica Talisman. Each was broken into separate claims about what a system like this must represent, do, or answer, and the book has to prove each claim with data that can actually discriminate. Every claim below is our own paraphrase; nothing from the essays is reproduced.</p>
      <Err error={articles.error} />
      <div className="grid c3" style={{ gridTemplateColumns: "repeat(5, 1fr)" }}>{articles.rows.map((a) => (
        <button key={a.source_article_id} className={`card pressable ${a.is_fully_covered ? "edge-green" : "edge-amber"}`} style={{ marginBottom: 0, textAlign: "left", outline: sel === a.source_article_id ? "2px solid var(--blue)" : "none" }} onClick={() => setSel(a.source_article_id)}>
          <div className="big derived">{Number(a.coverage_percent).toFixed(1)}<span style={{ fontSize: 16 }}>%</span></div>
          <div className="sub" style={{ margin: "4px 0 8px" }}>{a.covered_claim_count} of {a.claim_count} claims proved</div><b style={{ fontSize: 13.5 }}>{a.title}</b><div className="sub">{a.author}</div></button>))}</div>
      {sel && (<><div className="row" style={{ margin: "18px 4px 8px" }}><div className="section grow" style={{ margin: 0 }}>Claims <span className="count">{claims.rows?.length ?? "·"}</span></div><button className="tab" aria-selected={only} onClick={() => setOnly(!only)}>Only the ones not yet proved</button></div>
        <div className="card" style={{ padding: 0 }}><table className="t"><tbody>{claims.rows?.map((c) => (<>
          <tr key={c.article_claim_id} className="click" onClick={() => setOpen(open === c.article_claim_id ? null : c.article_claim_id)}><td style={{ width: 110 }}><span className="mono">{c.section_ref}</span></td><td>{c.claim_text}</td><td style={{ width: 130 }}><Tag tone="grey">{String(c.claim_kind).replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase()}</Tag></td><td style={{ width: 120 }}>{c.is_covered ? <Tag tone="green">proved</Tag> : <Tag tone="solid-red">not yet proved</Tag>}</td></tr>
          {open === c.article_claim_id && <tr key={c.article_claim_id + "e"}><td /><td colSpan={3}><Evidence claim={c.article_claim_id} /></td></tr>}</>))}</tbody></table></div></>)}
      <p className="provenance">Procedural Knowledge Ontology 2.0.0 by Valentina Anita Carriero, Mario Scrocca, Ilaria Baroni, Antonia Azzini and Irene Celino (Cefriel), CC BY 4.0. This model aligns to PKO; it is not an official PKO distribution and no endorsement is implied.</p>
    </>);
}
function Evidence({ claim }: { claim: string }) {
  const ev = useRows("claim_evidence", { article_claim: claim });
  return (<>{ev.rows?.map((e) => <div key={e.claim_evidence_id} className="ground" style={{ background: e.is_valid ? "var(--green-wash)" : "var(--red-wash)" }}><code>{e.rulebook_field || e.rulebook_table || e.role_question || e.ontology_profile || e.knowledge_method || e.procedure}</code><span>{e.justification}{e.field_is_discriminating === false ? " (does not discriminate over the seed data)" : ""}</span></div>)}{ev.rows?.length === 0 && <span className="sub">No evidence offered yet.</span>}</>);
}

function Actions() {
  const actions = useRows("app_actions", {}, "story_episode,owning_role,app_action_id");
  return (<div className="card" style={{ padding: 0 }}><table className="t"><thead><tr><th>Role</th><th>May</th><th>Writes</th><th>Permitted by</th><th>Then the book re-works</th><th>Proven to refuse others?</th></tr></thead><tbody>
    {actions.rows?.map((a) => <tr key={a.app_action_id}><td>{human(a.owning_role)}</td><td><b>{a.label}</b><div className="sub">{a.description}</div></td><td><span className="mono">{a.operation} {a.target_table}</span><div className="sub">{a.input_field_count} columns</div></td>
      <td>{a.is_unpermitted ? <Tag tone="solid-red">nothing permits this</Tag> : <span className="mono">{a.policy}</span>}{a.policy_command_disagrees && <Tag tone="red">wrong command</Tag>}</td><td><span className="rule">{a.watched_field}</span></td><td>{a.is_unproven_write ? <Tag tone="amber">not yet</Tag> : <Tag tone="green">{a.policy_denial_test_count} tests</Tag>}</td></tr>)}</tbody></table></div>);
}

function Context() {
  const ctx = useRows("evaluation_contexts", { is_current: true }); const risk = useRows("know_how_carriers", { is_at_risk_of_imminent_loss: true });
  const leaving = useRows("know_how_carriers", { holder_agent: "tomas-reyes" }); const [edit, setEdit] = useState(false); const c = ctx.rows?.[0];
  if (!c) return <Loading />;
  return (<div className="grid c2"><div className="card"><div className="sub">Every time-dependent answer in this book is judged as of</div><div className="big fact" style={{ margin: "8px 0 4px", fontSize: 30 }}>{fmtTime(c.as_of_instant)}</div><p className="sub">{c.rationale}</p>
    <button className="btn accent" style={{ marginTop: 16 }} onClick={() => setEdit(true)}>Move the date</button>
    <p className="provenance">The date is a row in <code>EvaluationContexts</code>, not the wall clock, so the same question gives the same answer tomorrow. Moving it re-judges every freshness, overdue and departure answer at once.</p></div>
    <div><div className="section" style={{ marginTop: 0 }}>Skills at risk of imminent loss, as of that date <span className="count">{risk.rows?.length ?? "·"}</span></div>
      {risk.rows?.length === 0 && <div className="card edge-green"><b className="derived">Nothing is at risk.</b></div>}{risk.rows?.map((k) => <div key={k.know_how_carrier_id} className="card tight edge-red"><b>{k.topic}</b><div className="sub">{human(k.holder_agent)} · {k.days_until_holder_departure} days</div></div>)}
      <div className="section">Tomas Reyes</div>{leaving.rows?.map((k) => <div key={k.know_how_carrier_id} className={`card tight ${k.is_captured ? "edge-green" : ""}`}><b>{k.topic}</b><div className="row wrap" style={{ marginTop: 6 }}><Tag tone="grey">{k.days_until_holder_departure} days until departure</Tag><Tag tone={k.is_captured ? "green" : "grey"}>{k.is_captured ? "captured" : "not written down"}</Tag><Tag tone={k.transfer_count ? "green" : "grey"}>passed on {k.transfer_count}×</Tag></div></div>)}</div>
    {edit && <ActionSheet actionId="act-admin-move-instant" recordKey={c.evaluation_context_id} watch="khc12-tomas-bleed" onClose={() => setEdit(false)} onDone={() => location.reload()} />}</div>);
}

function Story() {
  const { bump, toast } = useSession();
  const [pending, setPending] = useState<{ added: api.Row[]; updated: api.Row[] } | null>(null); const [error, setError] = useState<string | null>(null);
  const [log, setLog] = useState<string[]>([]); const [busy, setBusy] = useState<string | null>(null);
  const load = () => api.unsaved().then(setPending).catch((e) => setError(e.message));
  useEffect(() => { load(); }, []);
  const run = async (what: "save" | "reset") => {
    setBusy(what); setLog([]);
    try { const ok = await api.streamPost(`/api/admin/story/${what}`, (l) => setLog((x) => [...x.slice(-200), l]));
      toast({ tone: ok ? "green" : "red", title: what === "save" ? (ok ? "Saved to the book" : "Save failed") : ok ? "Story reset" : "Reset failed" }); bump(); await load();
    } catch (e: any) { setError(e.message); } finally { setBusy(null); }
  };
  return (<div className="grid c2"><div><Err error={error} />
    <div className="card"><div className="h">Save to the book</div><p className="sub" style={{ margin: "6px 0 12px" }}>The app is a normal Postgres app: what people did stays in the database. It becomes part of the model only when it is saved into the rulebook. Rows are added, only writable columns are updated, nothing is deleted, and the data gets a version of its own.</p>
      {pending ? <><div className="row wrap" style={{ marginBottom: 12 }}><Tag tone={pending.added.length ? "amber" : "grey"}>{pending.added.length} rows not in the book</Tag><Tag tone={pending.updated.length ? "amber" : "grey"}>{pending.updated.length} rows changed</Tag></div>
        {[...pending.added.map((a) => `+ ${a.table}  ${a.key}`), ...pending.updated.map((u) => `~ ${u.table}  ${u.key}  ${Object.keys(u.changes).join(", ")}`)].slice(0, 14).map((l) => <div key={l} className="mono">{l}</div>)}</> : <Loading />}
      <button className="btn accent" style={{ marginTop: 14 }} disabled={!!busy || !pending || (!pending.added.length && !pending.updated.length)} onClick={() => run("save")}>{busy === "save" ? "Saving…" : "Save this session to the book"}</button></div>
    <div className="card"><div className="h">Reset the story</div><p className="sub" style={{ margin: "6px 0 12px" }}>Drops the database and reloads it from the rulebook, back to the modelled date. Anything not saved to the book is gone. Takes about half a minute.</p>
      <button className="btn danger" disabled={!!busy} onClick={() => run("reset")}>{busy === "reset" ? "Reloading from the rulebook…" : "Reset to the rulebook"}</button></div></div>
    <div className="card" style={{ background: "#0b1220", color: "#cbd5e1", minHeight: 300 }}><div className="mono" style={{ color: "#64748b", marginBottom: 8 }}>{busy ? `running: ${busy}` : "output"}</div>{log.slice(-28).map((l, i) => <div key={i} className="mono" style={{ color: "#cbd5e1", whiteSpace: "pre-wrap" }}>{l}</div>)}</div></div>);
}
