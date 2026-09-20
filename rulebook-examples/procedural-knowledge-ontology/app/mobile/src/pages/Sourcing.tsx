import { useState } from "react";
import * as api from "../api";
import { useRows, useSession } from "../session";
import { ActionSheet, AppBar, Err, Loading, Tag, fmtDay, human, yn } from "../ui/kit";

export default function Sourcing() {
  const { shell } = useSession();
  const mine = shell!.claims.organization;
  const [tab, setTab] = useState<"contracts" | "audit" | "people">("contracts");
  const [org, setOrg] = useState(mine);
  const orgs = useRows("organizations", { organization_type: "Internal" });
  const engagements = useRows("provider_engagements", { client_organization: org }, "-is_knowledge_access_unsecured,provider");
  const [clause, setClause] = useState<api.Row | null>(null); const [deliver, setDeliver] = useState<api.Row | null>(null);
  const clients = useRows("provider_engagements", {}, "client_organization");
  const clientOrgs = [...new Set((clients.rows || []).map((e) => e.client_organization))];

  return (
    <>
      <AppBar title="What We Still Know" />
      <div className="tabs">
        <button className="tab" aria-selected={tab === "contracts"} onClick={() => setTab("contracts")}>Contracts</button>
        <button className="tab" aria-selected={tab === "audit"} onClick={() => setTab("audit")}>What we need, and who holds it</button>
        <button className="tab" aria-selected={tab === "people"} onClick={() => setTab("people")}>The people whose job it is</button>
        <span className="grow" />
        {clientOrgs.map((o) => <button key={o} className="tab" aria-selected={org === o} onClick={() => setOrg(o)}>{human(o).replace("Acme", "ACME")}{o === mine ? " · mine" : ""}</button>)}
      </div>
      <div className="content">
        {tab === "contracts" && (<><Err error={engagements.error} />
          {!engagements.rows ? <Loading /> : engagements.rows.map((e) => <Engagement key={e.provider_engagement_id} e={e} mine={e.client_organization === mine} onClause={() => setClause(e)} onDeliver={() => setDeliver(e)} />)}
          {org !== mine && <p className="provenance">Sourcing reads every ACME organization's contracts. It may change only {human(mine)}'s: the row policy compares <code>client_organization</code> to the organization in your token.</p>}</>)}
        {tab === "audit" && <Audit org={org} />}
        {tab === "people" && <People />}
      </div>
      {clause && <ActionSheet actionId="act-sourcing-set-clause" recordKey={clause.provider_engagement_id} watch={clause.provider_engagement_id} initial={{ HasKnowledgeAccessClause: clause.has_knowledge_access_clause }} onClose={() => setClause(null)} />}
      {deliver && <ActionSheet actionId="act-sourcing-require-deliverable" context={{ ProviderEngagement: deliver.provider_engagement_id }} watch={deliver.provider_engagement_id} onClose={() => setDeliver(null)} />}
    </>
  );
}

function Engagement({ e, mine, onClause, onDeliver }: { e: api.Row; mine: boolean; onClause: () => void; onDeliver: () => void }) {
  const deliverables = useRows("knowledge_deliverables", { provider_engagement: e.provider_engagement_id });
  const fn = useRows("sourcing_functions", { sourcing_function_id: e.sourcing_function });
  const red = e.is_knowledge_access_unsecured; const amber = !red && (e.is_one_way_learning || e.lacks_knowledge_deliverables || (e.to_client_required_count > e.to_client_delivered_count));
  return (
    <div className={`card ${red ? "edge-red" : amber ? "edge-amber" : "edge-green"}`}>
      <div className="row top">
        <div className="grow"><div className="h">{human(e.provider)}</div><div className="sub">{String(fn.rows?.[0]?.name || "").split(": ")[1] || human(e.sourcing_function)} · since {fmtDay(e.started_at)} · {e.term_months} months · {String(e.status).toLowerCase()}</div></div>
        <div className="row wrap" style={{ justifyContent: "flex-end", maxWidth: 420 }}>
          {e.is_knowledge_access_unsecured && <Tag tone="solid-red">knowledge access unsecured</Tag>}{e.lacks_knowledge_deliverables && <Tag tone="red">no knowledge deliverables</Tag>}
          {e.is_one_way_learning && <Tag tone="amber">one way learning</Tag>}{e.to_client_required_count > e.to_client_delivered_count && <Tag tone="amber">required {e.to_client_required_count}, delivered {e.to_client_delivered_count}</Tag>}
          {e.obliges_knowledge_flow_back && <Tag tone="green">knowledge flows back</Tag>}
        </div>
      </div>
      <div className="grid c4" style={{ marginTop: 14 }}>
        <div><div className="sub">Who owns the documentation?</div><b className="fact">{String(e.documentation_ownership).replace("ProviderIP", "The provider").replace("ClientOwned", "ACME").replace("Joint", "Jointly")}</b></div>
        <div><div className="sub">Knowledge access clause?</div><b className="fact">{yn(e.has_knowledge_access_clause)}</b></div>
        <div><div className="sub">Knowledge deliverables required</div><b className="derived">{e.required_deliverable_count}</b></div>
        <div><div className="sub">Do we depend on what they know?</div><b className="derived">{e.relied_dependency_count > 0 ? `Yes, ${e.relied_dependency_count}` : "No"}</b></div>
      </div>
      {deliverables.rows && deliverables.rows.length > 0 && <div style={{ marginTop: 12 }}>{deliverables.rows.map((d) => <div key={d.knowledge_deliverable_id} className="row" style={{ padding: "6px 0", borderTop: "1px solid var(--line)" }}><span className="grow fact">{d.title}</span><span className="sub">{String(d.direction).replace("ToClient", "to ACME").replace("ToProvider", "to the provider")} · due {fmtDay(d.due_at)}</span>{d.is_delivered ? <Tag tone="green">delivered</Tag> : <Tag tone="amber">not yet delivered</Tag>}</div>)}</div>}
      {mine && <div className="row" style={{ marginTop: 14 }}><span className="grow" /><button className="btn sm ghost" onClick={onClause}>Edit contract</button><button className="btn sm accent" onClick={onDeliver}>Require a deliverable</button></div>}
    </div>);
}

function Audit({ org }: { org: string }) {
  const audits = useRows("knowledge_audits", { organization: org }, "-conducted_at");
  return (<>{audits.rows?.length === 0 && <div className="empty">No knowledge audit on file for {human(org)}.</div>}{audits.rows?.map((a) => <AuditCard key={a.knowledge_audit_id} a={a} />)}</>);
}
function AuditCard({ a }: { a: api.Row }) {
  const items = useRows("knowledge_audit_items", { knowledge_audit: a.knowledge_audit_id }, "-is_knowledge_dependency,-is_coverage_gap");
  const bar = (n: number, color: string) => <div className="bar"><i style={{ width: `${(n / 3) * 100}%`, background: color }} /></div>;
  return (
    <div className={`card ${a.treats_deficit_as_cost_problem ? "edge-red" : "edge-green"}`}>
      <div className="row top"><div className="grow"><div className="h">{a.title}</div><div className="sub">{fmtDay(a.conducted_at)} · run by {human(a.conducted_by_agent)} · asked about: <b>{String(a.problem_framing).replace(/([a-z])([A-Z])/g, "$1 $2").toLowerCase()}</b></div></div>
        {a.treats_deficit_as_cost_problem ? <Tag tone="solid-red">treats a knowledge deficit as a cost problem</Tag> : <Tag tone="green">asks what we need to know</Tag>}</div>
      <div className="grid c2" style={{ marginTop: 14 }}>{items.rows?.map((i) => (
        <div key={i.knowledge_audit_item_id} className={`card tight ${i.is_knowledge_dependency ? "edge-red" : i.is_coverage_gap ? "edge-black" : ""}`} style={{ marginBottom: 0, boxShadow: "inset 0 0 0 1.5px var(--line)" }}>
          <b>{i.knowledge_area}</b>
          <div className="lvl"><span>needed</span>{bar(i.needed_level, "var(--ink-3)")}<b>{i.needed_level}</b></div>
          <div className="lvl"><span>held inside ACME</span>{bar(i.held_internal_level, "var(--blue)")}<b className={i.held_internal_level < i.needed_level ? "cell-bad" : ""}>{i.held_internal_level}</b></div>
          <div className="lvl"><span>held by the provider</span>{bar(i.provider_held_level, "var(--amber)")}<b>{i.provider_held_level}</b></div>
          <div className="row wrap" style={{ marginTop: 8 }}>{i.is_knowledge_dependency && <Tag tone="red">we depend on them to know this</Tag>}{i.is_coverage_gap && <Tag tone="black">nobody knows this</Tag>}{i.is_single_team_silo && <Tag tone="amber">one team only</Tag>}</div>
        </div>))}</div>
    </div>);
}
function People() {
  const pos = useRows("knowledge_workforce_positions", {});
  return (<div className="card"><p className="sub" style={{ marginBottom: 10 }}>The jobs at ACME whose whole purpose is looking after knowledge. No tool does this without them.</p><table className="t"><thead><tr><th>Organization</th><th>Job</th><th>Status</th><th>Held by</th></tr></thead><tbody>
    {pos.rows?.map((p) => <tr key={p.knowledge_workforce_position_id}><td>{human(p.organization).replace("Acme", "ACME")}</td><td><b>{String(p.discipline).replace(/([a-z])([A-Z])/g, "$1 $2")}</b></td><td>{p.status === "Filled" ? <Tag tone="green">filled</Tag> : p.status === "Open" ? <Tag tone="amber">open</Tag> : <Tag tone="solid-red">not budgeted</Tag>}</td><td className="fact">{p.filled_by_agent ? human(p.filled_by_agent) : "—"}</td></tr>)}</tbody></table></div>);
}
