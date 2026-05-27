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

export default function ExplorerScreen({ screen }) {
  const navigate = useNavigate();
  const params   = useParams();
  const { domain } = params;
  const wildcard = params["*"] || "";
  const pathSegments = useMemo(() => parsePathSegments(wildcard), [wildcard]);

  const [tree, setTree]         = useState(null);
  const [expanded, setExpanded] = useState(() => new Set());
  const [node, setNode]         = useState(null);
  const [nodeError, setNodeError] = useState(null);
  const [nodeLoading, setNodeLoading] = useState(false);
  const [page, setPage]         = useState(0);
  const [treeError, setTreeError] = useState(null);

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
  }, [wildcard, page, domain]); // eslint-disable-line react-hooks/exhaustive-deps

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

      <Breadcrumbs domain={domain} segments={pathSegments} onCrumb={goTo} />

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

function RightPane({ pathSegments, node, nodeError, nodeLoading, page, setPage, goTo, rulebookRevision }) {
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
    return <ListView node={node} page={page} setPage={setPage} pathSegments={pathSegments} goTo={goTo} />;
  }
  if (node.kind === "instance") {
    return <InstanceView node={node} pathSegments={pathSegments} goTo={goTo} />;
  }
  return <div className="muted small">Unknown node kind: {node.kind}</div>;
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

function ListView({ node, page, setPage, pathSegments, goTo }) {
  const cols = node.schema || [];
  const rows = node.rows || [];
  const totalPages = Math.max(1, Math.ceil((node.totalCount || 0) / (node.pageSize || 50)));

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
      </h3>

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

function InstanceView({ node, pathSegments, goTo }) {
  const cols = node.schema || [];
  const row  = node.row || {};
  return (
    <>
      <h3 className="mono" style={{ marginTop: 0 }}>
        {node.entity} / <span style={{ color: "var(--accent)" }}>{node.id}</span>
        <span className="muted small" style={{ marginLeft: 8, fontWeight: 400 }}>
          {node.pk} = <span className="mono">{String(node.pkValue ?? "")}</span>
        </span>
      </h3>

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
          {cols.map((c) => (
            <tr key={c.name}>
              <td>
                <div>{c.name}</div>
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
              <td>{formatCell(row[c.name])}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}
