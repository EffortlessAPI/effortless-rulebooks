import React from "react";

// ===========================================================================
// ChainRibbon — ONE primitive for every transitive-closure chain in the model.
//
// Three relationships in this rulebook are the SAME construct: a `closure` field
// over adjacent edges, materialized as vw_*_closure(from_id,to_id,is_inferred).
//   • WorkflowSteps     precedesStep      (vw_step_precedence_closure)
//   • Roles             delegatesTo       (vw_roles_closure)
//   • WorkflowArtifacts wasDerivedFrom    (vw_workflow_artifacts_closure)
// They used to each have their own bespoke surface (a matrix, a tier chart,
// nothing). This component renders any of them identically: a left→right ribbon
// of nodes with a connector between each adjacent pair. A connector is SOLID
// when the link is asserted, GHOST when it only exists by transitive inference,
// and a GAP when it's missing — a broken chain. A missing connector can carry an
// inline fix button, so the trip→see→fix loop is the same everywhere.
//
// This computes NOTHING about the domain. The caller passes nodes already in
// chain order and the per-connector link state (read from the closure view / the
// raw adjacency). The ribbon only draws it.
// ===========================================================================

export interface RibbonNode {
  id: string;
  label: string;        // badge text — a letter, "#3", or an icon
  title: string;        // the node's full name
  kind?: string;        // maps to a k-<kind> color class (ai/human/pipeline/step/artifact/role)
  color?: string;       // explicit badge color (overrides kind), for nominal labels
  sub?: string;         // small secondary line under the title
  flag?: string;        // annotation chip, e.g. "⚑ origin"
  origin?: boolean;     // true = chain head (no parent); styles the node as a root
}

export type LinkState = "asserted" | "inferred" | "missing";

export interface RibbonLink {
  state: LinkState;
  caption?: string;        // tiny text on the connector ("derived from", "then →")
  onFix?: () => void;      // present on a `missing` link → renders the inline fix button
  fixLabel?: string;       // e.g. "↺ restore link"
}

interface ChainRibbonProps {
  nodes: RibbonNode[];
  links: RibbonLink[];     // exactly nodes.length - 1 entries (one per adjacent pair)
  // The closure pair counts (asserted + inferred = total), shown as the same
  // "4 asserted + 6 inferred = 10 pairs" line the Closure tab uses.
  counts?: { asserted: number; inferred: number; total: number };
  relationLabel?: string;  // human name of the relation, e.g. "wasDerivedFrom"
  brokenNote?: string;     // shown as a warning banner when the chain is split
}

export function ChainRibbon({ nodes, links, counts, relationLabel, brokenNote }: ChainRibbonProps) {
  return (
    <div className="ribbon">
      {counts && (
        <div className="rb-counts">
          <span className="rb-count asserted"><b>{counts.asserted}</b> asserted</span>
          <span className="rb-plus">+</span>
          <span className="rb-count inferred"><b>{counts.inferred}</b> inferred</span>
          <span className="rb-eq">=</span>
          <span className="rb-count total"><b>{counts.total}</b> reachable pairs</span>
          {relationLabel && <span className="rb-rel muted">over <code>{relationLabel}</code></span>}
        </div>
      )}

      {brokenNote && <div className="rb-broken">⚠ {brokenNote}</div>}

      <div className="rb-track">
        {nodes.map((n, i) => (
          <React.Fragment key={n.id}>
            <div
              className={"rb-node k-" + (n.kind || "step") + (n.origin ? " origin" : "")}
              title={n.title}
            >
              <span
                className="rb-badge"
                style={n.color ? { background: n.color, borderColor: n.color, color: "#19102e" } : undefined}
              >
                {n.label}
              </span>
              <span className="rb-node-body">
                <span className="rb-node-title">{n.title}</span>
                {n.sub && <span className="rb-node-sub muted">{n.sub}</span>}
              </span>
              {n.flag && <span className="rb-flag">{n.flag}</span>}
            </div>

            {/* connector to the next node (one fewer than node count) */}
            {i < nodes.length - 1 && <Connector link={links[i]} />}
          </React.Fragment>
        ))}
      </div>
    </div>
  );
}

function Connector({ link }: { link: RibbonLink | undefined }) {
  const state = link?.state ?? "missing";
  return (
    <div className={"rb-link " + state} title={connectorTitle(state, link?.caption)}>
      <span className="rb-link-line" />
      {link?.caption && state !== "missing" && <span className="rb-link-cap muted">{link.caption}</span>}
      {state === "missing" && (
        <span className="rb-gap">
          <span className="rb-gap-x">✂</span>
          {link?.onFix && (
            <button className="rb-fix" onClick={link.onFix} title="repair this broken link">
              {link.fixLabel || "↺ restore link"}
            </button>
          )}
        </span>
      )}
    </div>
  );
}

function connectorTitle(state: LinkState, caption?: string): string {
  const c = caption ? `${caption}: ` : "";
  return state === "asserted"
    ? `${c}asserted link (a fact you stated)`
    : state === "inferred"
      ? `${c}inferred by transitivity — never asserted`
      : `${c}MISSING — the chain is broken here`;
}
