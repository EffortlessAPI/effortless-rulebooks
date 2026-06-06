import { useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import { api, EntityMeta, Row } from "../lib/api";
import { useAsync } from "../lib/useApi";
import { DagCell } from "../explainer-dag";

const TITLES: Record<string, string> = {
  "symmetry-groups": "Symmetry Groups",
  prototiles: "Prototiles",
  tilings: "Tilings",
  "tiling-prototiles": "Tiling ↔ Prototiles",
  "vertex-figures": "Vertex Figures",
  regions: "Regions",
  placements: "Placements",
};

/** PascalCase rulebook table name per entity key (for the explainer DAG). */
const RB_TABLE: Record<string, string> = {
  "symmetry-groups": "SymmetryGroups",
  prototiles: "Prototiles",
  tilings: "Tilings",
  "tiling-prototiles": "TilingPrototiles",
  "vertex-figures": "VertexFigures",
  regions: "Regions",
  placements: "Placements",
};

/** snake_case view column -> PascalCase rulebook field name. */
function toPascal(snake: string): string {
  return snake
    .split("_")
    .map((w) => w.charAt(0).toUpperCase() + w.slice(1))
    .join("");
}

function cellText(v: any): string {
  if (v === null || v === undefined) return "";
  if (typeof v === "boolean") return v ? "yes" : "no";
  if (typeof v === "number") return Number.isInteger(v) ? String(v) : v.toFixed(4);
  return String(v);
}

export default function EntityPage() {
  const { entity = "" } = useParams();
  const meta = useAsync(() => api.entities(), []);
  const rows = useAsync(() => api.list(entity), [entity]);
  const [msg, setMsg] = useState<string | null>(null);
  const [err, setErr] = useState<string | null>(null);
  const [showNew, setShowNew] = useState(false);

  const em: EntityMeta | undefined = meta.data?.[entity];

  if (meta.error) return <div className="page"><div className="error">{meta.error}</div></div>;
  if (rows.error) return <div className="page"><div className="error">{rows.error}</div></div>;
  if (!em || rows.loading || meta.loading) return <div className="page">Loading…</div>;

  const columns = em.columns;
  const editableCols = em.editable.filter((c) => c !== em.pk);

  async function saveCell(id: string, col: string, raw: string, type: string) {
    setErr(null);
    setMsg(null);
    try {
      const value = coerce(raw, type);
      await api.patch(entity, id, { [col]: value });
      setMsg(`Updated ${col} on ${id} — derived values recomputed.`);
      rows.reload();
    } catch (e: any) {
      setErr(e.message);
    }
  }

  async function del(id: string) {
    if (!confirm(`Delete ${id}?`)) return;
    setErr(null);
    try {
      await api.remove(entity, id);
      setMsg(`Deleted ${id}.`);
      rows.reload();
    } catch (e: any) {
      setErr(e.message);
    }
  }

  return (
    <div className="page">
      <div className="page-head">
        <h1>{TITLES[entity] ?? entity}</h1>
        <button className="btn" onClick={() => setShowNew((s) => !s)}>
          {showNew ? "Cancel" : "+ New row"}
        </button>
      </div>
      <p className="muted">
        Editable columns (the raw inputs) are highlighted. Everything else is a
        derived value read from <code>{em.view}</code> — change an input and watch
        it recompute on save.
      </p>

      {msg && <div className="toast ok">{msg}</div>}
      {err && <div className="toast no">{err}</div>}

      {showNew && (
        <NewRowForm
          em={em}
          onCreated={() => {
            setShowNew(false);
            setMsg("Row created.");
            rows.reload();
          }}
          onError={setErr}
        />
      )}

      <div className="table-wrap">
        <table className="grid">
          <thead>
            <tr>
              {columns.map((c) => (
                <th key={c.name} className={c.editable ? "editable-col" : ""}>
                  {c.name}
                  {c.editable && <span className="edit-dot" title="editable raw input">●</span>}
                </th>
              ))}
              <th></th>
            </tr>
          </thead>
          <tbody>
            {(rows.data ?? []).map((row: Row) => (
              <tr key={row[em.pk]}>
                {columns.map((c) => (
                  <td key={c.name} className={c.editable ? "editable-col" : "derived-col"}>
                    {c.editable && c.name !== em.pk ? (
                      <EditableCell
                        value={row[c.name]}
                        type={c.type}
                        onSave={(raw) => saveCell(row[em.pk], c.name, raw, c.type)}
                      />
                    ) : c.editable ? (
                      // the raw PK — a calculated formula? No; PK is raw. Show plain.
                      cellText(row[c.name])
                    ) : (
                      <DagCell table={RB_TABLE[entity]} field={toPascal(c.name)}>
                        {cellText(row[c.name])}
                      </DagCell>
                    )}
                  </td>
                ))}
                <td>
                  <button className="btn-link danger" onClick={() => del(row[em.pk])}>
                    delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

function coerce(raw: string, type: string): any {
  if (type === "boolean") return raw === "true" || raw === "yes" || raw === "1";
  if (type === "integer" || type === "numeric" || type === "double precision") {
    if (raw.trim() === "") return null;
    return Number(raw);
  }
  return raw;
}

function EditableCell({
  value,
  type,
  onSave,
}: {
  value: any;
  type: string;
  onSave: (raw: string) => void;
}) {
  const initial = type === "boolean" ? (value ? "true" : "false") : value ?? "";
  const [v, setV] = useState<string>(String(initial));
  const dirty = String(initial) !== v;

  if (type === "boolean") {
    return (
      <select
        className="cell-input"
        value={v}
        onChange={(e) => {
          setV(e.target.value);
          onSave(e.target.value);
        }}
      >
        <option value="true">yes</option>
        <option value="false">no</option>
      </select>
    );
  }

  return (
    <span className="cell-edit">
      <input
        className="cell-input"
        value={v}
        onChange={(e) => setV(e.target.value)}
        onKeyDown={(e) => {
          if (e.key === "Enter") onSave(v);
        }}
      />
      {dirty && (
        <button className="btn-mini" onClick={() => onSave(v)}>
          save
        </button>
      )}
    </span>
  );
}

function NewRowForm({
  em,
  onCreated,
  onError,
}: {
  em: EntityMeta;
  onCreated: () => void;
  onError: (m: string) => void;
}) {
  const fields = useMemo(
    () => em.columns.filter((c) => em.editable.includes(c.name)),
    [em],
  );
  const [form, setForm] = useState<Row>({});

  async function submit() {
    try {
      const body: Row = {};
      for (const f of fields) {
        if (form[f.name] !== undefined && form[f.name] !== "")
          body[f.name] = coerce(String(form[f.name]), f.type);
      }
      await api.create(em.key, body);
      onCreated();
    } catch (e: any) {
      onError(e.message);
    }
  }

  return (
    <div className="new-row">
      {fields.map((f) => (
        <label key={f.name} className="field">
          <span>
            {f.name}
            {em.required.includes(f.name) && <b className="req"> *</b>}
          </span>
          {f.type === "boolean" ? (
            <select
              value={form[f.name] ?? "false"}
              onChange={(e) => setForm({ ...form, [f.name]: e.target.value })}
            >
              <option value="false">no</option>
              <option value="true">yes</option>
            </select>
          ) : (
            <input
              value={form[f.name] ?? ""}
              onChange={(e) => setForm({ ...form, [f.name]: e.target.value })}
            />
          )}
        </label>
      ))}
      <button className="btn" onClick={submit}>
        Create
      </button>
    </div>
  );
}
