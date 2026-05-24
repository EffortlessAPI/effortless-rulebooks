import { useCallback, useEffect, useState } from "react";
import RowEditor from "./RowEditor";

const API = "/api";

export default function TableView({ meta }) {
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(null); // null | "new" | row object
  const [error, setError] = useState(null);

  const allCols = [...meta.columns, ...meta.viewExtra];

  const load = useCallback(() => {
    setLoading(true);
    fetch(`${API}/${meta.key}`)
      .then((r) => r.json())
      .then((data) => {
        setRows(Array.isArray(data) ? data : []);
        setError(null);
      })
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [meta.key]);

  useEffect(() => {
    load();
  }, [load]);

  const handleSave = async (row) => {
    const isNew = editing === "new";
    const url = isNew
      ? `${API}/${meta.key}`
      : `${API}/${meta.key}/${row[meta.pk]}`;
    const method = isNew ? "POST" : "PUT";
    const res = await fetch(url, {
      method,
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(row),
    });
    if (!res.ok) {
      const body = await res.json().catch(() => ({}));
      alert(body.error || "Save failed");
      return;
    }
    setEditing(null);
    load();
  };

  const handleDelete = async (id) => {
    if (!confirm("Delete this row?")) return;
    const res = await fetch(`${API}/${meta.key}/${id}`, { method: "DELETE" });
    if (!res.ok) {
      const body = await res.json().catch(() => ({}));
      alert(body.error || "Delete failed");
      return;
    }
    load();
  };

  if (editing !== null) {
    const initial =
      editing === "new"
        ? Object.fromEntries(meta.columns.map((c) => [c, ""]))
        : editing;
    return (
      <RowEditor
        meta={meta}
        row={initial}
        isNew={editing === "new"}
        onSave={handleSave}
        onCancel={() => setEditing(null)}
      />
    );
  }

  return (
    <div>
      <div className="table-header">
        <h2>{meta.label}</h2>
        <button className="btn btn-primary" onClick={() => setEditing("new")}>
          + New
        </button>
      </div>
      {error && <p className="error">{error}</p>}
      {loading ? (
        <p>Loading...</p>
      ) : rows.length === 0 ? (
        <p className="empty">No rows yet.</p>
      ) : (
        <div className="table-scroll">
          <table>
            <thead>
              <tr>
                {allCols.map((c) => (
                  <th key={c}>{formatCol(c)}</th>
                ))}
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((row) => (
                <tr key={row[meta.pk]}>
                  {allCols.map((c) => (
                    <td key={c}>{renderCell(row[c])}</td>
                  ))}
                  <td className="actions">
                    <button
                      className="btn btn-sm"
                      onClick={() => setEditing(row)}
                    >
                      Edit
                    </button>
                    <button
                      className="btn btn-sm btn-danger"
                      onClick={() => handleDelete(row[meta.pk])}
                    >
                      Del
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

function formatCol(col) {
  return col
    .replace(/_/g, " ")
    .replace(/\b\w/g, (c) => c.toUpperCase());
}

function renderCell(value) {
  if (value === null || value === undefined) return <span className="null">null</span>;
  if (typeof value === "boolean") return value ? "Yes" : "No";
  const s = String(value);
  if (s.length > 120) return s.slice(0, 120) + "...";
  return s;
}
