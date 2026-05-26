import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../../lib/api.js";
import { toast } from "../../lib/toast.js";
import ScreenHeader from "../../components/ScreenHeader.jsx";

export default function SubstratesScreen({ screen, me }) {
  const navigate = useNavigate();
  const [subs, setSubs]       = useState([]);
  const [selected, setSelected] = useState(null);
  const canBuild = me?.role?.CanRunBuilds;

  useEffect(() => { api.get("/api/substrates").then(setSubs); }, []);

  const sel = subs.find((s) => s.SubstrateId === selected) || subs[0];

  const buildAll = async () => {
    toast("Building…");
    const r = await api.post("/api/build/all");
    toast(r.ok ? "Build complete" : "Build failed", r.ok ? "ok" : "error");
  };

  return (
    <>
      <ScreenHeader screen={screen} />
      <div className="story-banner" style={{ borderLeftColor: "#888" }}>
        All substrates are PEER projections of the rulebook — no substrate is the reference; conformance is the proof.
      </div>
      <div style={{ display: "flex", gap: 10, marginBottom: 12, alignItems: "center" }}>
        <span className="muted small">{subs.length} substrates registered</span>
        <div className="spacer" />
        <button className="btn secondary" onClick={() => navigate("/developer/tools/add")}>+ Add Tool</button>
        {canBuild && <button className="btn" onClick={buildAll}>Build all</button>}
      </div>
      <div className="split">
        <div className="list-panel">
          {subs.map((s) => (
            <div key={s.SubstrateId}
                 className={`list-item ${s.SubstrateId === sel?.SubstrateId ? "active" : ""}`}
                 onClick={() => setSelected(s.SubstrateId)}>
              <div className="name">{s.Name} <span className="tag">{s.Technology}</span></div>
              <div className="meta">
                {s.RelativePath}
                {s.Maturity && <span className="pill" style={{ marginLeft: 6 }}>{s.Maturity}</span>}
                {s.CanBeAnswerKey && <span className="pill good" style={{ marginLeft: 4 }}>answer-key</span>}
                {s.exists === false && <span className="pill warn" style={{ marginLeft: 4 }}>not built</span>}
              </div>
            </div>
          ))}
        </div>
        <div className="detail-panel">
          {sel ? (
            <>
              <h3 style={{ marginTop: 0 }}>{sel.Name}</h3>
              <div className="kv">
                <div className="k">Technology</div>            <div className="v">{sel.Technology}</div>
                <div className="k">Path</div>                  <div className="v mono">{sel.RelativePath}</div>
                <div className="k">Transpiler source</div>     <div className="v"><span className="pill">{sel.TranspilerSource || "—"}</span></div>
                <div className="k">Maturity</div>              <div className="v"><span className="pill">{sel.Maturity || "—"}</span></div>
                <div className="k">Expressive completeness</div><div className="v"><span className="pill">{sel.ExpressiveCompleteness || "—"}</span></div>
                <div className="k">Can be answer key?</div>    <div className="v">{sel.CanBeAnswerKey ? "yes" : "no"}</div>
                <div className="k">Status</div>                <div className="v">{sel.Status || "—"}</div>
              </div>
              <p>{sel.Description}</p>
            </>
          ) : <div className="muted">No substrate selected.</div>}
        </div>
      </div>
    </>
  );
}
