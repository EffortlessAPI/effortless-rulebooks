import { useState, useEffect } from "react";
import { api } from "../../lib/api.js";
import { toast } from "../../lib/toast.js";
import ScreenHeader from "../../components/ScreenHeader.jsx";

function formatCell(v) {
  if (v === null || v === undefined) return <span className="muted">—</span>;
  if (typeof v === "object") return JSON.stringify(v);
  return String(v);
}

export function TechPostgresScreen({ screen }) {
  const [tables, setTables] = useState([]);
  const [sql, setSql] = useState("SELECT entity_name, jsonb_array_length(data_json) AS rows FROM portal_rulebook_entities ORDER BY 1;");
  const [result, setResult] = useState(null);

  useEffect(() => {
    api.get("/api/tech/postgres/tables").then((r) => setTables(r.tables || [])).catch(() => {});
  }, []);

  const run = async () => {
    try { setResult(await api.post("/api/tech/postgres/query", { sql })); }
    catch (e) { toast(e.message, "error"); }
  };

  return (
    <>
      <ScreenHeader screen={screen} />
      <div className="muted small" style={{ marginBottom: 8 }}>Editor DB tables: {tables.length}</div>
      <table className="grid">
        <thead><tr><th>schema</th><th>table</th></tr></thead>
        <tbody>{tables.map((t, i) => <tr key={i}><td>{t.table_schema}</td><td>{t.table_name}</td></tr>)}</tbody>
      </table>
      <h4 style={{ marginTop: 20 }}>SQL</h4>
      <textarea rows={4} value={sql} onChange={(e) => setSql(e.target.value)} />
      <div style={{ marginTop: 8 }}><button className="btn" onClick={run}>Run query</button></div>
      {result && (
        <>
          <h4>Result ({result.rowCount} rows)</h4>
          {result.rows?.length ? (
            <div style={{ overflowX: "auto" }}>
              <table className="grid">
                <thead><tr>{result.fields.map((f) => <th key={f}>{f}</th>)}</tr></thead>
                <tbody>
                  {result.rows.map((r, i) => (
                    <tr key={i}>{result.fields.map((f) => <td key={f}>{formatCell(r[f])}</td>)}</tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : <div className="muted">No rows.</div>}
        </>
      )}
    </>
  );
}

export function TechProxyScreen({ screen }) {
  const [status, setStatus] = useState(null);
  useEffect(() => {
    api.get("/api/tech/proxy/status").then(setStatus).catch((e) => setStatus({ error: e.message }));
  }, []);
  return (
    <>
      <ScreenHeader screen={screen} />
      <div className="editor">{JSON.stringify(status, null, 2)}</div>
    </>
  );
}

export function TechFilesScreen({ screen }) {
  return (
    <>
      <ScreenHeader screen={screen} />
      <p className="muted">Filesystem browser is read-only. Consult Effortless Tools for generated output trees.</p>
    </>
  );
}

export function TechJsonScreen({ screen }) {
  const [text, setText] = useState("");
  const load = () => fetch("/api/tech/rulebook-json").then((r) => r.text()).then(setText);
  useEffect(() => { load(); }, []);

  const save = async () => {
    try {
      JSON.parse(text); // local syntax check
      if (!text || !text.trim()) throw new Error("nothing to save — textarea is empty");
      const res = await fetch("/api/tech/rulebook-json", {
        method: "PUT",
        headers: { "Content-Type": "text/plain" },
        body: text,
      });
      if (!res.ok) {
        let serverMsg = "";
        try { serverMsg = (await res.json()).error || ""; } catch { serverMsg = await res.text(); }
        throw new Error(`HTTP ${res.status}: ${serverMsg || "save rejected by server"}`);
      }
      toast("Saved rulebook JSON (and rebooted editor DB).");
    } catch (e) { toast("Invalid JSON or save failed: " + e.message, "error"); }
  };

  return (
    <>
      <ScreenHeader screen={screen} />
      <div style={{ display: "flex", gap: 8, marginBottom: 8 }}>
        <button className="btn secondary" onClick={load}>Reload from disk</button>
        <button className="btn" onClick={save}>Save</button>
      </div>
      <textarea rows={30} value={text} onChange={(e) => setText(e.target.value)} />
    </>
  );
}

export function TechResetScreen({ screen }) {
  const [busy, setBusy] = useState(false);
  const reset = async () => {
    if (!confirm("Drop the editor Postgres DB and rebuild from rulebook JSON?")) return;
    setBusy(true);
    try {
      await api.post("/api/tech/reset");
      toast("Editor DB rebuilt from rulebook JSON.");
    } catch (e) { toast(e.message, "error"); }
    setBusy(false);
  };
  return (
    <>
      <ScreenHeader screen={screen} />
      <div className="card" style={{ maxWidth: 480 }}>
        <h3>Safe operation</h3>
        <p>Drops the per-project editor Postgres database and re-bootstraps from the rulebook JSON on disk. The rulebook JSON is the durable SSoT — no business data is lost.</p>
        <button className="btn danger" disabled={busy} onClick={reset}>
          {busy ? "Resetting…" : "Reset editor DB now"}
        </button>
      </div>
    </>
  );
}
