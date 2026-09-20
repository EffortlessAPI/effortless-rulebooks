import { useState } from "react";
import { useRows } from "../session";
import { Totals } from "../ui/register";
import { AppBar, Err, ExplainerNote, Explains, Fact, KV, Loading, Tag, Why, fmtDay, human } from "../ui/kit";

/** The process steward's home: is the work done the way the book says, and where does the book stand against PKO. */
export default function Steward() {
  const [tab, setTab] = useState<"reality" | "standard">("reality");
  const runs = useRows("process_mining_runs", {}, "conformance_rate");
  return (
    <>
      <AppBar title="Plan Versus Reality" />
      <div className="tabs">
        <button className="tab" aria-selected={tab === "reality"} onClick={() => setTab("reality")}>Plan versus reality<span className="n">{runs.rows?.length ?? "·"}</span></button>
        <button className="tab" aria-selected={tab === "standard"} onClick={() => setTab("standard")}>Against the standard</button>
      </div>
      <div className="content">
        {tab === "reality" && (<>
          <p className="sub" style={{ margin: "6px 4px 12px", maxWidth: 860 }}>The procedure says how the work should be done. A system's own log says how it was done. Each card is one log replayed against one version of a procedure.</p>
          <ExplainerNote /><Err error={runs.error} />
          {!runs.rows ? <Loading /> : <div className="grid c2">{runs.rows.map((r) => (
            <Explains key={r.process_mining_run_id} t="process_mining_runs">
            <div id={`pmr-${r.process_mining_run_id}`} className={`card ${r.is_drift_on_live_version ? "edge-red" : r.is_conformant ? "edge-green" : "edge-amber"}`} style={{ marginBottom: 0 }}>
              {r.is_drift_on_live_version && <div className="banner red">● Drifting, on the version everyone still follows<Why f="is_drift_on_live_version" label="drift on a live version" /></div>}
              <div className="row top"><div className="grow"><div className="h">{human(r.procedure_version)}<Why f="procedure_version" /></div><div className="sub">{r.event_log_source}<Why f="event_log_source" /> · replayed {fmtDay(r.mined_at)}</div></div>
                <div className="big" style={{ color: r.is_drift_on_live_version ? "var(--red)" : r.is_conformant ? "var(--green)" : "var(--amber)" }}>{r.conformance_percent}<span style={{ fontSize: 16 }}>%</span><Why f="conformance_percent" label="percent that match the plan" /></div></div>
              <KV t="process_mining_runs" style={{ marginTop: 10 }}>
                <Fact f="discovered_variant_count" label="Paths people really took" tone="fact">{r.discovered_variant_count}</Fact>
                <Fact f="conforming_variant_count" label="Paths that match the plan" tone="fact">{r.conforming_variant_count}</Fact>
              </KV>
              {r.deviation_description && <div className="quote" style={{ marginTop: 10 }}>{r.deviation_description}<span className="by">what the log shows<Why f="deviation_description" /></span></div>}
              <div className="row wrap" style={{ marginTop: 10 }}>
                <Tag tone={r.procedure_version_is_live ? "blue" : "grey"} f="procedure_version_is_live">{r.procedure_version_is_live ? "the live version" : "not the live version"}</Tag>
                {r.has_undocumented_enacted_path && <Tag tone="amber" f="has_undocumented_enacted_path">a path nobody documented</Tag>}
                {r.is_deviation_unexplained_by_people && <Tag tone="amber" f="is_deviation_unexplained_by_people">nobody has been asked why</Tag>}
              </div>
            </div></Explains>))}</div>}
        </>)}
        {tab === "standard" && (<>
          <p className="sub" style={{ margin: "6px 4px 12px", maxWidth: 860 }}>This book is aligned to the Procedural Knowledge Ontology, an open standard. Every concept in it is mapped one of three ways, and an extension is always labelled as an extension so that nobody mistakes it for the standard.</p>
          <ExplainerNote />
          <Totals id="against-pko" title="Where this book stands against PKO" items={[
            { f: "exact_mapping_count", label: "map exactly onto a term PKO defines" },
            { f: "aligned_mapping_count", label: "map onto an older standard PKO reuses", tone: "fact" },
            { f: "extension_mapping_count", label: "are ACME's own extensions, labelled as extensions" }]} />
        </>)}
      </div>
    </>
  );
}
