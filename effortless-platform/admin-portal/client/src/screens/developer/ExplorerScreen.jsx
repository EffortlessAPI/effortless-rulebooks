// Effortless Explorer  (DOMAIN_UX_VISION.md §2)
// =============================================================================
// Step 2 surface: left-nav DAG driven by /api/explorer/tree, with each
// top-level entity expandable to reveal its child entities (the entities that
// have an FK pointing AT this one). Right pane is a stub — step 3 fills it
// with /api/explorer/node grids. URL path parsing also lives here so deep
// links (/explorer/<Entity>/<id>/...) round-trip cleanly.

import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../../lib/api.js";
import { toast } from "../../lib/toast.js";
import ScreenHeader from "../../components/ScreenHeader.jsx";

// Split the URL wildcard ("*" from /explorer/*) into decoded segments.
// Even-length paths (after the domain) are instance nodes per §2.6; odd-length
// are list nodes. Returned segments are exactly as authored in the rulebook
// (PascalCase entity names, Name-field values for ids).
function parsePathSegments(wildcard) {
  if (!wildcard) return [];
  return wildcard.split("/").filter(Boolean).map(decodeURIComponent);
}

function encodePathSegments(segments) {
  return segments.map(encodeURIComponent).join("/");
}

export default function ExplorerScreen({ screen }) {
  const navigate = useNavigate();
  const params   = useParams();
  const { domain } = params;
  const wildcard = params["*"] || "";
  const pathSegments = useMemo(() => parsePathSegments(wildcard), [wildcard]);

  const [tree, setTree]       = useState(null);
  const [expanded, setExpanded] = useState(() => new Set());
  const [error, setError]     = useState(null);

  useEffect(() => {
    setTree(null);
    setError(null);
    api.get(`/api/explorer/tree?domain=${encodeURIComponent(domain)}&maxDepth=1`)
      .then(setTree)
      .catch((e) => { setError(e.message); toast(e.message, "error"); });
  }, [domain]);

  // The current top-level entity (first segment) drives "selected" highlight
  // in the left rail.
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
        Walk the DAG for <b>{domain}</b>. Click an entity to list its rows; expand to
        see which other entities hang off it.
      </div>

      {error && (
        <div className="story-banner" style={{ borderLeftColor: "var(--bad)" }}>
          {error}
        </div>
      )}

      <div className="split">
        <div className="list-panel">
          {!tree && !error && <div className="muted small">Loading tree…</div>}
          {tree && tree.topLevel.length === 0 && (
            <div className="muted small">No entities in this rulebook.</div>
          )}
          {tree && tree.topLevel.map((node) => (
            <TreeRow
              key={node.entity}
              node={node}
              selected={selectedEntity === node.entity}
              expanded={expanded.has(node.entity)}
              onToggle={() => toggleExpand(node.entity)}
              onSelect={() => goTo([node.entity])}
              onSelectChild={(childEntity) => goTo([childEntity])}
            />
          ))}
        </div>

        <div className="detail-panel">
          <RightPane
            domain={domain}
            pathSegments={pathSegments}
            rulebookRevision={tree?.rulebookRevision || null}
          />
        </div>
      </div>
    </>
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

function RightPane({ domain, pathSegments, rulebookRevision }) {
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
  // Step 3 will render the actual list/instance grid here using
  // /api/explorer/node. For now: echo the path so the URL/path-parsing flow
  // is visible and testable end-to-end.
  return (
    <div>
      <h3 className="mono" style={{ marginTop: 0 }}>
        /{pathSegments.join("/")}
      </h3>
      <p className="muted small">
        Node detail (schema + data grid) lands in step 3.
      </p>
      <ol className="muted small" style={{ paddingLeft: 18 }}>
        {pathSegments.map((seg, i) => (
          <li key={i} className={i % 2 === 0 ? "" : "mono"}>
            {i % 2 === 0 ? `entity: ${seg}` : `instance: ${seg}`}
          </li>
        ))}
      </ol>
    </div>
  );
}
