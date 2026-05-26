import { useState, useEffect } from "react";
import { api } from "../../lib/api.js";
import { toast } from "../../lib/toast.js";
import ScreenHeader from "../../components/ScreenHeader.jsx";

export default function AddToolScreen({ screen }) {
  const [catalog, setCatalog]     = useState(null);
  const [installed, setInstalled] = useState({ transpilers: [] });
  const [busy, setBusy]           = useState(null);

  const load = async () => {
    setCatalog(await api.get("/api/tools/catalog"));
    setInstalled(await api.get("/api/tools/installed"));
  };
  useEffect(() => { load(); }, []);

  const install = async (tool) => {
    setBusy(tool.ToolId);
    try {
      const r = await api.post("/api/tools/install", { installUrl: tool.InstallUrl, outputPath: tool.OutputPath });
      toast(r.ok ? `Installed ${tool.Name}` : "Install failed", r.ok ? "ok" : "error");
      await load();
    } catch (e) { toast(e.message, "error"); }
    setBusy(null);
  };

  if (!catalog) return <div>Loading catalog…</div>;

  const installedNames = new Set((installed.transpilers || []).map((t) => t.Name?.toLowerCase().replace(/-/g, "")));

  return (
    <>
      <ScreenHeader screen={screen} />
      <div className="muted small" style={{ marginBottom: 12 }}>
        Catalog: {(catalog.fromRulebook || []).length} tools ·
        live proxy: {catalog.liveProxy?.transpilers ? Object.keys(catalog.liveProxy.transpilers).length : "offline"} routes ·
        installed: {(installed.transpilers || []).length}
      </div>
      <div className="cards">
        {(catalog.fromRulebook || []).map((t) => {
          const already = installedNames.has(t.Name?.toLowerCase().replace(/-/g, ""));
          return (
            <div key={t.ToolId} className="card">
              <h3>{t.Category}</h3>
              <div className="big" style={{ fontSize: 15 }}>{t.Name}</div>
              <div className="sub">{t.Description}</div>
              <div className="muted small mono" style={{ marginTop: 6, wordBreak: "break-all" }}>{t.InstallUrl}</div>
              <div className="muted small">→ {t.OutputPath}</div>
              <div style={{ marginTop: 10 }}>
                {already
                  ? <span className="pill good">installed</span>
                  : <button className="btn" disabled={busy === t.ToolId} onClick={() => install(t)}>
                      {busy === t.ToolId ? "Installing…" : "Install"}
                    </button>}
              </div>
            </div>
          );
        })}
      </div>
    </>
  );
}
