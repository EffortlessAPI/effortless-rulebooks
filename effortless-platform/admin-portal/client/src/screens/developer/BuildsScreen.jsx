import { useState, useEffect } from "react";
import { api } from "../../lib/api.js";
import { toast } from "../../lib/toast.js";
import ScreenHeader from "../../components/ScreenHeader.jsx";

export default function BuildsScreen({ screen, me }) {
  const [installed, setInstalled] = useState({ transpilers: [] });
  const [last, setLast] = useState(null);
  const [busy, setBusy] = useState(false);
  const canBuild = me?.role?.CanRunBuilds;

  useEffect(() => { api.get("/api/tools/installed").then(setInstalled); }, []);

  const run = async () => {
    setBusy(true);
    toast("Building all…");
    try {
      const r = await api.post("/api/build/all");
      setLast(r);
      toast(r.ok ? "Build complete" : "Build failed", r.ok ? "ok" : "error");
    } finally { setBusy(false); }
  };

  return (
    <>
      <ScreenHeader screen={screen} />
      <div style={{ display: "flex", gap: 10, marginBottom: 12, alignItems: "center" }}>
        <span className="muted small">Installed transpilers: {(installed.transpilers || []).length}</span>
        <div className="spacer" />
        {canBuild && <button className="btn" onClick={run} disabled={busy}>{busy ? "Building…" : "Trigger build"}</button>}
      </div>
      <table className="grid">
        <thead><tr><th>name</th><th>path</th><th>disabled</th><th>pinned</th></tr></thead>
        <tbody>
          {(installed.transpilers || []).map((t, i) => (
            <tr key={i}>
              <td>{t.Name}</td>
              <td className="mono">{t.RelativePath}</td>
              <td>{String(t.IsDisabled || false)}</td>
              <td>{t.PinnedVersion || ""}</td>
            </tr>
          ))}
        </tbody>
      </table>
      {last && (
        <>
          <h4>Last build output</h4>
          <div className="editor">{last.stdout}{last.stderr ? "\n--- stderr ---\n" + last.stderr : ""}</div>
        </>
      )}
    </>
  );
}
