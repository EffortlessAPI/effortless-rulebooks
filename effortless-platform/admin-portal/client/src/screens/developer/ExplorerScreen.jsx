// Effortless Explorer  (DOMAIN_UX_VISION.md §2)
// =============================================================================
// Left rail: DAG-shaped tree from /api/explorer/tree (top-level entities,
// each expandable to its child entities via inbound FKs).
// Right pane: /api/explorer/node — list view (odd-length path) or instance
// view (even-length path). Schema rides in the same payload as data so the
// column headers ARE the schema (hover for type/formula/description).
// URL encoding is §2.6: /<Entity>/<Name>/<ChildEntity>/<Name>... where the
// "id" segments are the entity's Name field (ERB closed-platform convention).

import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../../lib/api.js";
import { toast } from "../../lib/toast.js";
import ScreenHeader from "../../components/ScreenHeader.jsx";

function parsePathSegments(wildcard) {
  if (!wildcard) return [];
  return wildcard.split("/").filter(Boolean).map(decodeURIComponent);
}

function encodePathSegments(segments) {
  return segments.map(encodeURIComponent).join("/");
}

function formatCell(v) {
  if (v === null || v === undefined || v === "") return <span className="muted">∅</span>;
  if (typeof v === "boolean") return v ? "✓" : "✗";
  if (typeof v === "object") return <span className="mono small">{JSON.stringify(v)}</span>;
  return String(v);
}

export default function ExplorerScreen({ screen, me }) {
  const navigate = useNavigate();
  const params   = useParams();
  const { domain } = params;
  const wildcard = params["*"] || "";
  const pathSegments = useMemo(() => parsePathSegments(wildcard), [wildcard]);
  const canEdit = !!me?.role?.CanEditRulebook;

  const [tree, setTree]         = useState(null);
  const [expanded, setExpanded] = useState(() => new Set());
  const [node, setNode]         = useState(null);
  const [nodeError, setNodeError] = useState(null);
  const [nodeLoading, setNodeLoading] = useState(false);
  const [page, setPage]         = useState(0);
  const [treeError, setTreeError] = useState(null);
  const [refreshTick, setRefreshTick] = useState(0);
  const [rebuildId, setRebuildId] = useState(null);
  const refreshNode = () => setRefreshTick((t) => t + 1);
  const refreshAll = () => {
    setRefreshTick((t) => t + 1);
    api.get(`/api/explorer/tree?domain=${encodeURIComponent(domain)}&maxDepth=1`)
      .then(setTree).catch(() => {});
  };

  const triggerRebuild = async () => {
    try {
      const r = await api.post("/api/explorer/rebuild");
      setRebuildId(r.rebuildId);
    } catch (e) {
      toast(`Rebuild failed to start: ${e.message}`, "error");
    }
  };

  // Load tree on domain change
  useEffect(() => {
    setTree(null);
    setTreeError(null);
    api.get(`/api/explorer/tree?domain=${encodeURIComponent(domain)}&maxDepth=1`)
      .then(setTree)
      .catch((e) => { setTreeError(e.message); toast(e.message, "error"); });
  }, [domain]);

  // Load node on path change. Reset paging when path changes.
  useEffect(() => {
    setPage(0);
  }, [wildcard]);

  useEffect(() => {
    setNode(null);
    setNodeError(null);
    if (pathSegments.length === 0) return;
    setNodeLoading(true);
    const qs = new URLSearchParams({
      domain,
      path: pathSegments.map(encodeURIComponent).join("/"),
      page: String(page),
      pageSize: "50",
    });
    api.get(`/api/explorer/node?${qs.toString()}`)
      .then((r) => { setNode(r); })
      .catch((e) => { setNodeError(e.message); })
      .finally(() => setNodeLoading(false));
  }, [wildcard, page, domain, refreshTick]); // eslint-disable-line react-hooks/exhaustive-deps

  const selectedEntity = pathSegments[0] || null;

  const goTo = (segments) => {
    const tail = encodePathSegments(segments);
    navigate(tail ? `/developer/${domain}/explorer/${tail}` : `/developer/${domain}/explorer`);
  };

  const toggleExpand = (entityName) => {
    setExpanded((prev) => {
      const next = new Set(prev);
      if (next.has(entityName)) next.delete(entityName);
      else next.add(entityName);
      return next;
    });
  };

  return (
    <>
      <ScreenHeader screen={screen} />
      <div className="story-banner" style={{ borderLeftColor: "#6ea8fe" }}>
        Walk the DAG for <b>{domain}</b>. Click an entity to list its rows; click a
        row to see its instance + children.
      </div>

      {treeError && (
        <div className="story-banner" style={{ borderLeftColor: "var(--bad)" }}>
          {treeError}
        </div>
      )}

      <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
        <div style={{ flex: 1, minWidth: 0 }}>
          <Breadcrumbs domain={domain} segments={pathSegments} onCrumb={goTo} />
        </div>
        {canEdit && (
          <button
            className="btn secondary"
            onClick={triggerRebuild}
            title="Run 'effortless build' and tail the output"
            style={{ whiteSpace: "nowrap" }}
          >
            ↻ Rebuild
          </button>
        )}
      </div>

      {rebuildId && (
        <RebuildOverlay
          rebuildId={rebuildId}
          onClose={() => setRebuildId(null)}
          onSuccess={() => { setRebuildId(null); refreshAll(); }}
        />
      )}

      <div className="split">
        <div className="list-panel">
          {!tree && !treeError && <div className="muted small">Loading tree…</div>}
          {tree && tree.topLevel.length === 0 && (
            <div className="muted small">No entities in this rulebook.</div>
          )}
          {tree && tree.topLevel.map((n) => (
            <TreeRow
              key={n.entity}
              node={n}
              selected={selectedEntity === n.entity}
              expanded={expanded.has(n.entity)}
              onToggle={() => toggleExpand(n.entity)}
              onSelect={() => goTo([n.entity])}
              onSelectChild={(childEntity) => goTo([childEntity])}
            />
          ))}
        </div>

        <div className="detail-panel">
          <RightPane
            pathSegments={pathSegments}
            node={node}
            nodeError={nodeError}
            nodeLoading={nodeLoading}
            page={page}
            setPage={setPage}
            goTo={goTo}
            canEdit={canEdit}
            refreshNode={refreshNode}
            rulebookRevision={tree?.rulebookRevision || null}
          />
        </div>
      </div>
    </>
  );
}

function Breadcrumbs({ domain, segments, onCrumb }) {
  if (segments.length === 0) return null;
  return (
    <div className="muted small" style={{ marginTop: 8, marginBottom: 8 }}>
      <span className="clickable" onClick={() => onCrumb([])}>/explorer</span>
      {segments.map((s, i) => (
        <span key={i}>
          {" / "}
          <span
            className={i % 2 === 0 ? "mono clickable" : "clickable"}
            onClick={() => onCrumb(segments.slice(0, i + 1))}
          >
            {s}
          </span>
        </span>
      ))}
    </div>
  );
}

function TreeRow({ node, selected, expanded, onToggle, onSelect, onSelectChild }) {
  const hasChildren = node.children && node.children.length > 0;
  return (
    <div>
      <div
        className={`list-item ${selected ? "active" : ""}`}
        onClick={onSelect}
        style={{ display: "flex", alignItems: "center", gap: 6 }}
      >
        <span
          onClick={(e) => { e.stopPropagation(); onToggle(); }}
          style={{
            width: 14, textAlign: "center", cursor: hasChildren ? "pointer" : "default",
            opacity: hasChildren ? 1 : 0.25, userSelect: "none",
          }}
          title={hasChildren ? (expanded ? "Collapse" : "Expand") : "No child entities"}
        >
          {hasChildren ? (expanded ? "▾" : "▸") : "·"}
        </span>
        <div style={{ flex: 1, minWidth: 0 }}>
          <div className="name">
            {node.important && <span style={{ color: "var(--warn)", marginRight: 4 }}>★</span>}
            {node.entity}
          </div>
          <div className="meta">{node.rowCount} {node.rowCount === 1 ? "row" : "rows"}</div>
        </div>
      </div>
      {expanded && hasChildren && (
        <div style={{ paddingLeft: 26 }}>
          {node.children.map((child, i) => (
            <div
              key={`${child.entity}/${child.viaFk}/${i}`}
              className="list-item"
              onClick={() => onSelectChild(child.entity)}
              title={`${child.entity}.${child.viaFk} → ${node.entity}`}
            >
              <div className="name mono" style={{ fontSize: 12 }}>
                {child.entity}
              </div>
              <div className="meta">
                via <span className="mono">{child.viaFk}</span> · {child.rowCount} {child.rowCount === 1 ? "row" : "rows"}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

function RightPane({ pathSegments, node, nodeError, nodeLoading, page, setPage, goTo, canEdit, refreshNode, rulebookRevision }) {
  if (pathSegments.length === 0) {
    return (
      <div className="muted">
        <p>Select an entity from the left to start walking the DAG.</p>
        {rulebookRevision && (
          <p className="small">
            Rulebook revision: <span className="mono">{rulebookRevision}</span>
          </p>
        )}
      </div>
    );
  }
  if (nodeError) {
    return (
      <div className="story-banner" style={{ borderLeftColor: "var(--bad)" }}>
        {nodeError}
      </div>
    );
  }
  if (nodeLoading || !node) {
    return <div className="muted small">Loading…</div>;
  }
  if (node.kind === "list") {
    return <ListView node={node} page={page} setPage={setPage} pathSegments={pathSegments} goTo={goTo} canEdit={canEdit} refreshNode={refreshNode} />;
  }
  if (node.kind === "instance") {
    return <InstanceView node={node} pathSegments={pathSegments} goTo={goTo} canEdit={canEdit} refreshNode={refreshNode} />;
  }
  return <div className="muted small">Unknown node kind: {node.kind}</div>;
}

// Fields the server's classifyFieldsForWrite() will accept. Mirror that
// logic here so the client doesn't try to PATCH read-only fields.
function isWritableField(f) {
  if (!f) return false;
  if (f.type === "raw") return true;
  if (f.type === "relationship") {
    // Forward side only (heuristic mirrors server: no prefersSingleRecordLink=false).
    return f.prefersSingleRecordLink !== false;
  }
  return false;
}

function coerceValueForType(raw, datatype) {
  if (raw === "" || raw === null || raw === undefined) return null;
  if (datatype === "integer") {
    const n = parseInt(raw, 10);
    return Number.isNaN(n) ? raw : n;
  }
  if (datatype === "number" || datatype === "decimal" || datatype === "float") {
    const n = parseFloat(raw);
    return Number.isNaN(n) ? raw : n;
  }
  if (datatype === "boolean") {
    if (typeof raw === "boolean") return raw;
    return /^(true|yes|1)$/i.test(String(raw).trim());
  }
  return String(raw);
}

function FieldInput({ field, value, onChange }) {
  if (field.datatype === "boolean") {
    return (
      <input
        type="checkbox"
        checked={!!value}
        onChange={(e) => onChange(e.target.checked)}
      />
    );
  }
  if (field.datatype === "integer" || field.datatype === "number" || field.datatype === "decimal" || field.datatype === "float") {
    return (
      <input
        type="number"
        value={value ?? ""}
        onChange={(e) => onChange(e.target.value)}
        style={{ width: "100%" }}
      />
    );
  }
  if (field.datatype === "datetime" || field.datatype === "date") {
    return (
      <input
        type="text"
        value={value ?? ""}
        onChange={(e) => onChange(e.target.value)}
        placeholder={field.datatype === "date" ? "YYYY-MM-DD" : "YYYY-MM-DDTHH:MM:SSZ"}
        style={{ width: "100%" }}
      />
    );
  }
  return (
    <input
      type="text"
      value={value ?? ""}
      onChange={(e) => onChange(e.target.value)}
      style={{ width: "100%" }}
    />
  );
}

function SchemaHeaderCell({ field }) {
  // The column header IS the schema entry per §2.3. Hover shows the full
  // type / formula / description. Step 6 will add click-for-provenance.
  const parts = [];
  if (field.type) parts.push(`type: ${field.type}`);
  if (field.datatype) parts.push(`datatype: ${field.datatype}`);
  if (field.formula) parts.push(`formula: ${field.formula}`);
  if (field.Description) parts.push(`\n${field.Description}`);
  if (field.RelatedTo) parts.push(`→ ${field.RelatedTo}`);
  return (
    <th title={parts.join("  ·  ")}>
      <div>{field.name}</div>
      <div className="muted small" style={{ fontWeight: 400 }}>
        {field.type === "raw" ? field.datatype : field.type}
      </div>
    </th>
  );
}

function ListView({ node, page, setPage, pathSegments, goTo, canEdit, refreshNode }) {
  const cols = node.schema || [];
  const rows = node.rows || [];
  const totalPages = Math.max(1, Math.ceil((node.totalCount || 0) / (node.pageSize || 50)));
  const [showNewRow, setShowNewRow] = useState(false);

  return (
    <>
      <h3 className="mono" style={{ marginTop: 0 }}>
        {node.entity}
        <span className="muted small" style={{ marginLeft: 8, fontWeight: 400 }}>
          {node.totalCount} {node.totalCount === 1 ? "row" : "rows"}
          {node.scopedBy && (
            <> · scoped by {node.scopedBy.entity}{" "}
              <span className="mono">{node.scopedBy.id}</span>{" "}
              via <span className="mono">{node.scopedBy.viaFk}</span>
            </>
          )}
        </span>
        {canEdit && (
          <button
            className="btn secondary"
            style={{ marginLeft: 12 }}
            onClick={() => setShowNewRow((v) => !v)}
          >
            {showNewRow ? "× Cancel" : "+ New row"}
          </button>
        )}
      </h3>

      {showNewRow && canEdit && (
        <NewRowForm
          entity={node.entity}
          schema={cols}
          scopedBy={node.scopedBy}
          onCreated={() => { setShowNewRow(false); refreshNode(); }}
          onCancel={() => setShowNewRow(false)}
        />
      )}

      {rows.length === 0 ? (
        <div className="muted small">No rows.</div>
      ) : (
        <div style={{ overflowX: "auto" }}>
          <table className="grid">
            <thead>
              <tr>{cols.map((c) => <SchemaHeaderCell key={c.name} field={c} />)}</tr>
            </thead>
            <tbody>
              {rows.map((row, i) => (
                <tr
                  key={i}
                  className="clickable"
                  onClick={() => goTo([...pathSegments, String(row.Name ?? "")])}
                  title={`Open ${node.entity} / ${row.Name}`}
                >
                  {cols.map((c) => <td key={c.name}>{formatCell(row[c.name])}</td>)}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {totalPages > 1 && (
        <div className="muted small" style={{ marginTop: 8, display: "flex", gap: 8, alignItems: "center" }}>
          <button className="btn secondary" disabled={page === 0} onClick={() => setPage(page - 1)}>
            ← Prev
          </button>
          <span>Page {page + 1} of {totalPages}</span>
          <button className="btn secondary" disabled={page + 1 >= totalPages} onClick={() => setPage(page + 1)}>
            Next →
          </button>
        </div>
      )}
    </>
  );
}

function NewRowForm({ entity, schema, scopedBy, onCreated, onCancel }) {
  const writable = schema.filter(isWritableField);
  // Pre-seed the FK field if the URL ancestry implies one (e.g. creating a
  // Project under /Employees/Sarah Chen pre-fills ApprovedBy = sarah-chen).
  const [values, setValues] = useState(() => {
    const init = {};
    if (scopedBy?.viaFk) {
      // We have the parent's URL Name but not its PK value. Leave empty for
      // now — user supplies it (or a future server response can supply it).
      init[scopedBy.viaFk] = "";
    }
    return init;
  });
  const [busy, setBusy] = useState(false);
  const [err, setErr]   = useState(null);

  const submit = async () => {
    setBusy(true); setErr(null);
    try {
      const payload = {};
      for (const f of writable) {
        const v = values[f.name];
        if (v === undefined) continue;
        payload[f.name] = coerceValueForType(v, f.datatype);
      }
      await api.post(`/api/explorer/instance/${encodeURIComponent(entity)}`, payload);
      toast(`Created ${entity}/${payload.Name}`);
      onCreated();
    } catch (e) {
      setErr(e.message);
    } finally { setBusy(false); }
  };

  return (
    <div className="card" style={{ padding: 12, marginBottom: 12 }}>
      <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))", gap: 10 }}>
        {writable.map((f) => (
          <div key={f.name}>
            <label className="muted small" style={{ display: "block", marginBottom: 2 }}>
              {f.name}
              {f.nullable === false && <span style={{ color: "var(--bad)" }}> *</span>}
              {f.datatype && <span style={{ marginLeft: 4 }}>({f.datatype})</span>}
            </label>
            <FieldInput
              field={f}
              value={values[f.name]}
              onChange={(v) => setValues((s) => ({ ...s, [f.name]: v }))}
            />
          </div>
        ))}
      </div>
      {err && (
        <div className="story-banner" style={{ borderLeftColor: "var(--bad)", marginTop: 8 }}>{err}</div>
      )}
      <div style={{ marginTop: 10, display: "flex", gap: 8 }}>
        <button className="btn" disabled={busy} onClick={submit}>
          {busy ? "Creating…" : "Create"}
        </button>
        <button className="btn secondary" disabled={busy} onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
}

function InstanceView({ node, pathSegments, goTo, canEdit, refreshNode }) {
  const cols = node.schema || [];
  const row  = node.row || {};
  // Dirty map: only fields the user actually changed. Sending the unchanged
  // ones too would pass the server's classifier but is wasted writes.
  const [dirty, setDirty] = useState({});
  const [busy, setBusy]   = useState(false);
  const [err, setErr]     = useState(null);
  const [openCell, setOpenCell] = useState(null); // field name currently showing provenance

  // Reset dirty state + close any open provenance when the underlying row
  // changes (navigated to a new instance, or refreshed post-save).
  useEffect(() => { setDirty({}); setErr(null); setOpenCell(null); }, [node]);

  const isDirty = Object.keys(dirty).length > 0;

  const setField = (field, raw, datatype) => {
    setDirty((d) => ({ ...d, [field]: coerceValueForType(raw, datatype) }));
  };

  const save = async () => {
    setBusy(true); setErr(null);
    try {
      await api.patch(
        `/api/explorer/instance/${encodeURIComponent(node.entity)}/${encodeURIComponent(node.id)}`,
        dirty
      );
      toast("Saved (Postgres + rulebook JSON).");
      refreshNode();
    } catch (e) {
      setErr(e.message);
    } finally { setBusy(false); }
  };

  const del = async () => {
    if (!window.confirm(`Delete ${node.entity} / ${node.id}?`)) return;
    setBusy(true); setErr(null);
    try {
      await api.del(
        `/api/explorer/instance/${encodeURIComponent(node.entity)}/${encodeURIComponent(node.id)}`
      );
      toast(`Deleted ${node.entity}/${node.id}.`);
      goTo(pathSegments.slice(0, -1)); // back to the parent list
    } catch (e) {
      // Server returns 409 with referrer list when cascade is needed.
      if (e.referrers) {
        const yes = window.confirm(
          `${node.entity}/${node.id} has ${e.referrers.length} FK referrer(s). Cascade?`
        );
        if (yes) {
          try {
            await api.del(
              `/api/explorer/instance/${encodeURIComponent(node.entity)}/${encodeURIComponent(node.id)}?cascade=true`
            );
            toast(`Deleted ${node.entity}/${node.id} and ${e.referrers.length} referrer(s).`);
            goTo(pathSegments.slice(0, -1));
            return;
          } catch (e2) { setErr(e2.message); }
        }
      } else {
        setErr(e.message);
      }
    } finally { setBusy(false); }
  };

  return (
    <>
      <h3 className="mono" style={{ marginTop: 0 }}>
        {node.entity} / <span style={{ color: "var(--accent)" }}>{node.id}</span>
        <span className="muted small" style={{ marginLeft: 8, fontWeight: 400 }}>
          {node.pk} = <span className="mono">{String(node.pkValue ?? "")}</span>
        </span>
      </h3>

      {canEdit && (
        <div style={{ display: "flex", gap: 8, marginBottom: 12 }}>
          <button className="btn" disabled={!isDirty || busy} onClick={save}>
            {busy ? "Saving…" : isDirty ? `Save ${Object.keys(dirty).length} change(s)` : "Save"}
          </button>
          {isDirty && (
            <button className="btn secondary" disabled={busy} onClick={() => setDirty({})}>
              Reset
            </button>
          )}
          <button className="btn secondary" style={{ marginLeft: "auto", color: "var(--bad)" }} disabled={busy} onClick={del}>
            Delete row…
          </button>
        </div>
      )}

      {err && (
        <div className="story-banner" style={{ borderLeftColor: "var(--bad)" }}>{err}</div>
      )}

      {node.tabs && node.tabs.length > 0 && (
        <div style={{ display: "flex", gap: 6, flexWrap: "wrap", marginBottom: 14 }}>
          {node.tabs.map((t, i) => (
            <button
              key={`${t.entity}/${t.viaFk}/${i}`}
              className="btn secondary"
              onClick={() => goTo([...pathSegments, t.entity])}
              title={`${t.entity}.${t.viaFk} → ${node.entity}`}
            >
              {t.entity}
              <span className="muted small" style={{ marginLeft: 6 }}>
                {t.rowCount}
              </span>
            </button>
          ))}
        </div>
      )}

      <table className="grid">
        <thead><tr><th>field</th><th>type</th><th>value</th></tr></thead>
        <tbody>
          {cols.map((c) => {
            const writable = canEdit && isWritableField(c);
            const liveValue = c.name in dirty ? dirty[c.name] : row[c.name];
            // A cell has provenance to show whenever its field isn't plain
            // raw — calculated/lookup/aggregation/relationship all have a
            // story (formula text, target row, dependencies). Raw cells
            // skip the click target to keep the surface clean.
            const hasProvenance = c.type && c.type !== "raw";
            return (
              <tr key={c.name}>
                <td>
                  <div>
                    {c.name}
                    {c.name in dirty && (
                      <span className="muted small" style={{ marginLeft: 6, color: "var(--warn)" }}>•</span>
                    )}
                  </div>
                  {c.Description && (
                    <div className="muted small">{c.Description}</div>
                  )}
                </td>
                <td>
                  <span className="tag">{c.type || ""}</span>
                  {c.datatype && c.type === "raw" && (
                    <span className="muted small" style={{ marginLeft: 6 }}>{c.datatype}</span>
                  )}
                  {c.formula && (
                    <div className="mono small muted" style={{ marginTop: 4 }}>{c.formula}</div>
                  )}
                </td>
                <td
                  className={hasProvenance ? "clickable" : ""}
                  onClick={hasProvenance ? () => setOpenCell((cur) => cur === c.name ? null : c.name) : undefined}
                  title={hasProvenance ? "Click to see how this was computed" : undefined}
                >
                  {writable
                    ? <FieldInput field={c} value={liveValue} onChange={(v) => setField(c.name, v, c.datatype)} />
                    : formatCell(row[c.name])}
                  {hasProvenance && openCell === c.name && (
                    <CellProvenance
                      entity={node.entity}
                      id={node.id}
                      field={c.name}
                      onClose={() => setOpenCell(null)}
                      onNavigate={(segments) => { setOpenCell(null); goTo(segments); }}
                    />
                  )}
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </>
  );
}

function CellProvenance({ entity, id, field, onClose, onNavigate }) {
  const [data, setData] = useState(null);
  const [err, setErr]   = useState(null);
  useEffect(() => {
    setData(null); setErr(null);
    const qs = new URLSearchParams({
      entity, id, field,
    });
    api.get(`/api/explorer/cell?${qs.toString()}`)
      .then(setData)
      .catch((e) => setErr(e.message));
  }, [entity, id, field]);

  return (
    <div
      onClick={(e) => e.stopPropagation()}
      style={{
        marginTop: 8,
        padding: 10,
        background: "var(--panel-2)",
        border: "1px solid var(--border)",
        borderRadius: 6,
      }}
    >
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <strong className="mono small">{entity}.{field}</strong>
        <button className="btn secondary" onClick={onClose} style={{ padding: "2px 8px" }}>×</button>
      </div>
      {err && <div className="story-banner" style={{ borderLeftColor: "var(--bad)", marginTop: 6 }}>{err}</div>}
      {!data && !err && <div className="muted small" style={{ marginTop: 6 }}>Loading…</div>}
      {data && (
        <>
          <div className="muted small" style={{ marginTop: 6 }}>
            kind: <span className="tag">{data.kind}</span>
            {" · value: "}<span className="mono">{formatCell(data.value)}</span>
          </div>
          {data.formula && (
            <div className="mono small" style={{ marginTop: 6, padding: 6, background: "var(--panel)", borderRadius: 4 }}>
              {data.formula}
            </div>
          )}
          {data.explanation_rich && (
            <div
              className="small"
              style={{ marginTop: 6 }}
              dangerouslySetInnerHTML={{ __html: data.explanation_rich }}
            />
          )}
          {data.inputs && data.inputs.length > 0 && (
            <>
              <div className="muted small" style={{ marginTop: 8, fontWeight: 600 }}>Inputs</div>
              <ul style={{ margin: "4px 0 0 0", padding: "0 0 0 18px" }}>
                {data.inputs.map((inp, i) => (
                  <li key={i} className="small" style={{ marginTop: 2 }}>
                    {inp.entity ? (
                      <>
                        <span
                          className="mono clickable"
                          onClick={() => onNavigate([inp.entity, inp.id ?? ""])}
                          title={`Open ${inp.entity}/${inp.id || "…"}`}
                        >
                          {inp.entity}
                        </span>
                        {inp.id && (
                          <>
                            {" / "}
                            <span
                              className="mono clickable"
                              onClick={() => onNavigate([inp.entity, inp.id])}
                            >
                              {inp.id}
                            </span>
                          </>
                        )}
                        {inp.field && <>{"."}<span className="mono">{inp.field}</span></>}
                      </>
                    ) : (
                      <span className="mono">{inp.field}</span>
                    )}
                    {" · "}
                    <span className="tag" style={{ fontSize: 10 }}>{inp.kind}</span>
                    {inp.value !== undefined && (
                      <> {" = "}<span className="mono">{formatCell(inp.value)}</span></>
                    )}
                  </li>
                ))}
              </ul>
            </>
          )}
        </>
      )}
    </div>
  );
}

// =============================================================================
// RebuildOverlay  (DOMAIN_UX_VISION.md §2.7 + step 8)
// =============================================================================
// Opens when a /api/explorer/rebuild call (or a schema PATCH) returns a
// rebuildId. Tails the SSE stream emitted by /api/explorer/rebuild/:id/stream
// — each phase event becomes a line in a live log; on "done" the parent
// refreshes (tree + node); on "error" we leave the overlay open so the user
// can read the failure and decide how to recover.
//
// Per §2.8 decision 3, no auto-rollback. The recovery affordance is a copy-
// able `git checkout` command — explicit user action, never silent.

function RebuildOverlay({ rebuildId, onClose, onSuccess }) {
  const [events, setEvents] = useState([]);
  const [status, setStatus] = useState("pending");
  const [doneData, setDoneData] = useState(null);
  const [errData, setErrData] = useState(null);

  useEffect(() => {
    if (!rebuildId) return;
    const es = new EventSource(`/api/explorer/rebuild/${encodeURIComponent(rebuildId)}/stream`);
    es.addEventListener("phase", (e) => {
      try {
        const data = JSON.parse(e.data);
        setEvents((evs) => [...evs, { kind: "phase", data, t: Date.now() }]);
        setStatus("running");
      } catch {}
    });
    es.addEventListener("done", (e) => {
      try {
        const data = JSON.parse(e.data);
        setDoneData(data);
        setStatus("done");
      } catch {}
      es.close();
    });
    es.addEventListener("error", (e) => {
      // SSE 'error' events come in two flavors: server-emitted JSON (the
      // build failed loudly) and transport-level errors (network drop).
      // Differentiate by whether e.data is set.
      if (e.data) {
        try {
          const data = JSON.parse(e.data);
          setErrData(data);
          setStatus("error");
        } catch {
          setErrData({ msg: "stream parse error" });
          setStatus("error");
        }
      } else if (es.readyState === EventSource.CLOSED) {
        // Connection closed — if we haven't set done/error yet, leave the
        // overlay alone; the user can close manually.
      }
    });
    return () => es.close();
  }, [rebuildId]);

  // When the build succeeds, give the user one beat to see the green
  // "done" line, then let the parent refresh and close.
  useEffect(() => {
    if (status === "done") {
      const t = setTimeout(onSuccess, 600);
      return () => clearTimeout(t);
    }
  }, [status, onSuccess]);

  return (
    <div
      style={{
        position: "fixed", inset: 0, background: "rgba(0,0,0,0.55)", zIndex: 1000,
        display: "flex", alignItems: "center", justifyContent: "center", padding: 20,
      }}
      onClick={status === "error" || status === "done" ? onClose : undefined}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{
          background: "var(--panel)",
          border: "1px solid var(--border)",
          borderRadius: 8,
          width: "min(720px, 100%)",
          maxHeight: "80vh",
          display: "flex", flexDirection: "column",
        }}
      >
        <div style={{ padding: 12, borderBottom: "1px solid var(--border)", display: "flex", alignItems: "center", gap: 10 }}>
          <strong>
            {status === "pending" && "Starting rebuild…"}
            {status === "running" && "Rebuilding…"}
            {status === "done"    && "Rebuild complete"}
            {status === "error"   && "Rebuild failed"}
          </strong>
          <span className="mono small muted">{rebuildId}</span>
          <span style={{ flex: 1 }} />
          {(status === "done" || status === "error") && (
            <button className="btn secondary" onClick={onClose}>Close</button>
          )}
        </div>

        <div
          className="mono small"
          style={{ padding: 12, overflowY: "auto", flex: 1, background: "var(--panel-2)" }}
        >
          {events.length === 0 && status === "pending" && (
            <div className="muted">Connecting to stream…</div>
          )}
          {events.map((e, i) => (
            <div key={i} style={{ marginBottom: 2 }}>
              <span className="muted">[{e.data.phase}]</span> {e.data.msg}
            </div>
          ))}
          {status === "done" && doneData && (
            <div style={{ color: "var(--good)", marginTop: 6 }}>
              ✓ done in {doneData.durationMs}ms
            </div>
          )}
          {status === "error" && errData && (
            <div style={{ color: "var(--bad)", marginTop: 6 }}>
              ✗ {errData.code}: {errData.msg}
            </div>
          )}
        </div>

        {status === "error" && (
          <div style={{ padding: 12, borderTop: "1px solid var(--border)" }}>
            <div className="small" style={{ marginBottom: 8 }}>
              Per §2.8 decision 3, failed rebuilds aren't auto-rolled back —
              the rulebook JSON keeps the edit that triggered this failure.
              Inspect the diff or revert by hand:
            </div>
            <pre
              className="mono small"
              style={{
                padding: 8, background: "var(--panel-2)",
                border: "1px solid var(--border)", borderRadius: 4,
                margin: 0, overflowX: "auto",
              }}
            >git diff -- effortless-rulebook/
git checkout -- effortless-rulebook/   # to revert</pre>
          </div>
        )}
      </div>
    </div>
  );
}
