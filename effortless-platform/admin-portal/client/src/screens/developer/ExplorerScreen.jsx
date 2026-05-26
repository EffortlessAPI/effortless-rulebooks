import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { api } from "../../lib/api.js";
import { toast } from "../../lib/toast.js";
import ScreenHeader from "../../components/ScreenHeader.jsx";

const QUICK_QUERIES = [
  { label: "List schemas",
    sql: "SELECT schema_name FROM information_schema.schemata ORDER BY schema_name;" },
  { label: "Row counts (public)",
    sql: "SELECT relname AS table, n_live_tup AS rows FROM pg_stat_user_tables ORDER BY n_live_tup DESC;" },
];

export default function ExplorerScreen({ screen }) {
  const { domain } = useParams();
  const [tables, setTables] = useState([]);
  const [database, setDatabase] = useState("");
  const [selected, setSelected] = useState(null);
  const [sql, setSql] = useState("SELECT 1;");
  const [result, setResult] = useState(null);
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    api.get("/api/tech/postgres/tables").then((r) => {
      setTables(r.tables || []);
      setDatabase(r.database || "");
    }).catch((e) => toast(e.message, "error"));
  }, [domain]);

  const runQuery = async (q) => {
    const text = q ?? sql;
    setBusy(true);
    try {
      const r = await api.post("/api/tech/postgres/query", { sql: text });
      setResult(r);
    } catch (e) {
      setResult({ error: e.message });
    } finally {
      setBusy(false);
    }
  };

  const preview = (t) => {
    const q = `SELECT * FROM "${t.table_schema}"."${t.table_name}" LIMIT 100;`;
    setSql(q);
    setSelected(`${t.table_schema}.${t.table_name}`);
    runQuery(q);
  };

  return (
    <>
      <ScreenHeader screen={screen} />
      <div className="story-banner" style={{ borderLeftColor: "#b48cff" }}>
        <b>{database || "erb_*"}</b> · explore the generated database for <b>{domain}</b>
      </div>

      <div className="explorer">
        <div className="explorer-left">
          <div className="explorer-section-title">Tables ({tables.length})</div>
          <div className="explorer-table-list">
            {tables.map((t) => {
              const key = `${t.table_schema}.${t.table_name}`;
              return (
                <div
                  key={key}
                  className={`list-item ${selected === key ? "active" : ""}`}
                  onClick={() => preview(t)}
                >
                  <div className="name mono">{t.table_name}</div>
                  <div className="meta">{t.table_schema}</div>
                </div>
              );
            })}
          </div>
        </div>

        <div className="explorer-right">
          <div className="explorer-quick-row">
            {QUICK_QUERIES.map((q) => (
              <button key={q.label} className="btn secondary" onClick={() => { setSql(q.sql); runQuery(q.sql); }}>
                {q.label}
              </button>
            ))}
          </div>

          <textarea
            className="explorer-sql"
            rows={6}
            value={sql}
            onChange={(e) => setSql(e.target.value)}
            spellCheck={false}
          />
          <div style={{ marginTop: 8 }}>
            <button className="btn" disabled={busy} onClick={() => runQuery()}>
              {busy ? "Running…" : "Run query"}
            </button>
            <span className="muted small" style={{ marginLeft: 12 }}>
              Server returns the first 500 rows.
            </span>
          </div>

          <div className="explorer-result">
            {!result && <div className="muted small">No query run yet.</div>}
            {result?.error && <pre className="editor" style={{ color: "var(--bad)" }}>{result.error}</pre>}
            {result?.rows && (
              <>
                <div className="muted small">
                  {result.command} · {result.rowCount} rows · returning {result.rows.length}
                </div>
                {result.rows.length > 0 && (
                  <div className="inline-grid-wrap">
                    <table className="grid">
                      <thead>
                        <tr>{result.fields.map((f) => <th key={f}>{f}</th>)}</tr>
                      </thead>
                      <tbody>
                        {result.rows.map((row, i) => (
                          <tr key={i}>
                            {result.fields.map((f) => (
                              <td key={f}>{formatVal(row[f])}</td>
                            ))}
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
              </>
            )}
          </div>
        </div>
      </div>
    </>
  );
}

function formatVal(v) {
  if (v === null || v === undefined) return <span className="muted">∅</span>;
  if (typeof v === "boolean") return v ? "✓" : "✗";
  if (typeof v === "object") return JSON.stringify(v);
  return String(v);
}
