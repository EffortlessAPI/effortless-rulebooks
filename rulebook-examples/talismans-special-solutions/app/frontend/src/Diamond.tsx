import React from "react";
import type { StoreId } from "./types";

// ===========================================================================
// Diamond.tsx — the source / engines / client drift visual. NOTHING is privileged
// among the three STORES; the CLIENT is a fourth thing entirely (a reader, not a
// store).
//
// Four nodes, read bottom→top in the direction data actually flows:
//
//        ● Client            ← the app you're connected to (top apex)
//       / ╲                    reads exactly ONE engine (the lit wire)
//   Reasoner  Postgres        ← the two execution substrates (mid height)
//       ╲ /                     each DERIVED from the rulebook below
//        ◆ Rulebook          ← the single source of truth (bottom apex)
//
//   • Two DERIVE legs (rulebook→reasoner, rulebook→postgres) — the substrates are
//     built FROM the rulebook. This is the old "head & legs" relationship, just
//     drawn with the source at the BASE (everything is derived UP from it).
//   • Two WIRE edges (reasoner→client, postgres→client) — but only ONE is ever
//     lit: the engine the client is currently wired to (getBackend()). The lit
//     wire inherits that engine's drift state, so "you're about to read a STALE
//     engine" is visible on the board. The other wire is drawn faint/dashed:
//     available, not connected.
//
// The CLIENT is NOT a StoreId. It has no hash, no leg, no lastEditAt, no diff —
// it doesn't HAVE state, it DISPLAYS one engine's state. So drift/diff logic
// stays strictly 3-store (StoreId); the client rides along as an overlay with its
// own hover affordance (an engine picker that enters the app on that engine).
//
// Resting state of the three STORES shows the COMPUTED drift status — who is HEAD
// right now:
//   • all three agree            → the Rulebook is HEAD (the natural resting
//     state; the hub is authoritative because the legs match it)
//   • they disagree              → the most-recently-edited store is shown as
//     HEAD, but ONLY as a status read-out. Recency does not privilege it.
//
// There is NO selection. You HOVER a node to ask "if THIS store were
// authoritative, what would the others lose, and how would I push it?" (stores)
// or "which engine should the app read?" (client) — and you act straight from the
// hover. Move away and the box is gone.
//
// SyncPanel owns the data and hands this component a resolved view model:
//   nodeState(id) -> "head" | "lit" | "stale"     (resting drift status, stores)
//   status        -> the computed HEAD store id (drives the resting gold ring)
//   hovered       -> the store/client currently hovered (box + a hover ring)
//   legPlan       -> { left, right } leg-flow plan while a sync runs
//   labels        -> { id: { inSync, ago } }
//   syncing       -> a sync build is running (sparks animate)
//   connectedEngine -> which engine the client reads now (lights one wire)
//   onHover(id|null) -> pointer entered/left node `id`
//   disabled      -> hover is inert while a build runs
//   popover       -> React node floated next to the hovered node
// ===========================================================================

// `client` is an overlay id, distinct from the three real stores. It is NEVER a
// StoreId — it has no drift state of its own.
export type NodeId = StoreId | "client";

// The flow direction a leg's spark animates while a sync runs. "down-delayed" is
// a down-flow that waits for the up-flow to reach the hub first (CSS .flow-down-delayed).
export type LegFlow = "up" | "down" | "down-delayed";

interface NodeGeom {
  x: number;
  y: number;
  label: string;
  sub: string;
}

interface PopoverAnchor {
  top?: string;
  left?: string;
  right?: string;
  origin: "top" | "left" | "right";
}

interface LegDef {
  id: "left" | "right";
  from: StoreId;
  to: StoreId;
  engine: StoreId;
}

// A client wire: from an engine UP to the client apex. Only the connected one lights.
interface WireDef {
  id: "wire-left" | "wire-right";
  engine: StoreId;
}

interface NodeLabel {
  inSync?: boolean;
  ago?: string | null;
}

interface DiamondProps {
  nodeState: (id: StoreId) => "head" | "lit" | "stale";
  status: StoreId | null;
  hovered: NodeId | null;
  legPlan: Partial<Record<LegDef["id"], LegFlow | null>> | null;
  labels: Partial<Record<StoreId, NodeLabel>> | null;
  syncing: boolean;
  connectedEngine: StoreId;
  onHover: (id: NodeId | null) => void;
  disabled: boolean;
  popover: React.ReactNode;
}

// The SVG viewBox. Labels are positioned as a percentage of this same box so
// they land directly under each node, not in a re-laid-out grid below the svg.
const VIEW_W = 320;
const VIEW_H = 260;
// How far below a node's center (in viewBox units) the label's top sits — just
// past the largest node halo (rulebook r=26) so the text clears the circle.
const LABEL_DROP = 30;

// Node geometry in the 320×260 SVG viewBox. Diamond: rulebook at the BASE,
// engines mid-height left/right, client at the TOP apex.
export const NODES: Record<NodeId, NodeGeom> = {
  client:   { x: 160, y: 32,  label: "Client",   sub: "the app you're in" },
  reasoner: { x: 44,  y: 130, label: "Reasoner", sub: "db.json · OWL/SHACL" },
  postgres: { x: 276, y: 130, label: "Postgres", sub: "vw_* views" },
  rulebook: { x: 160, y: 226, label: "Rulebook", sub: "the source" },
};
// Where the hover box anchors, relative to the .diamond box. Biases toward the
// hovered node so it reads as "popping out of" it. `origin` drives
// transform-origin / horizontal alignment.
const POPOVER_ANCHOR: Record<NodeId, PopoverAnchor> = {
  client:   { top: "16%", left: "50%", origin: "top" },
  reasoner: { top: "40%", left: "-2%", origin: "left" },
  postgres: { top: "40%", right: "-2%", origin: "right" },
  rulebook: { top: "62%", left: "50%", origin: "top" },
};
// DERIVE legs: rulebook → each engine (the substrate is built from the source).
const LEGS: LegDef[] = [
  { id: "left", from: "rulebook", to: "reasoner", engine: "reasoner" },
  { id: "right", from: "rulebook", to: "postgres", engine: "postgres" },
];
// WIRE edges: each engine → client. Only the connected engine's wire lights.
const WIRES: WireDef[] = [
  { id: "wire-left", engine: "reasoner" },
  { id: "wire-right", engine: "postgres" },
];

export default function Diamond({
  nodeState, status, hovered, legPlan, labels, syncing,
  connectedEngine, onHover, disabled, popover,
}: DiamondProps) {
  const legClass = (leg: LegDef): string => {
    const f = legPlan?.[leg.id] || null;
    return [
      "dia-leg",
      `dia-leg-${leg.id}`,
      f ? `flow-${f}` : "",
      syncing ? "is-syncing" : "",
    ].filter(Boolean).join(" ");
  };

  const hover = (id: NodeId | null) => { if (!disabled && onHover) onHover(id); };
  const anchor = hovered ? POPOVER_ANCHOR[hovered] : null;

  return (
    <div className={`diamond ${syncing ? "is-syncing" : ""} status-${status || "none"} ${disabled ? "is-disabled" : ""}`}
         onMouseLeave={() => hover(null)}>
      <svg className="dia-svg" viewBox="0 0 320 260" role="img" aria-label="source / engines / client drift diamond">
        {/* DERIVE legs: rulebook (base) → each engine */}
        {LEGS.map((leg) => {
          const a = NODES[leg.from], b = NODES[leg.to];
          const f = legPlan?.[leg.id] || null;
          return (
            <g key={leg.id} className={legClass(leg)}>
              <line x1={a.x} y1={a.y} x2={b.x} y2={b.y} className="dia-leg-base" />
              <line x1={a.x} y1={a.y} x2={b.x} y2={b.y} className="dia-leg-glow" />
              {f && (
                <circle r="4.5" className="dia-spark">
                  <animateMotion dur="1.4s" repeatCount="indefinite"
                    keyPoints={f === "up" ? "1;0" : "0;1"} keyTimes="0;1"
                    path={`M${a.x},${a.y} L${b.x},${b.y}`} />
                </circle>
              )}
            </g>
          );
        })}

        {/* WIRE edges: each engine → client. Only the connected one is lit; it
            inherits that engine's drift state. The other is faint/dashed. */}
        {WIRES.map((w) => {
          const a = NODES[w.engine], b = NODES.client;
          const connected = w.engine === connectedEngine;
          const st = nodeState(w.engine); // lit wire borrows the engine's color
          return (
            <g key={w.id}
               className={`dia-wire dia-${w.id} ${connected ? `is-connected st-${st}` : "is-idle"}`}>
              <line x1={a.x} y1={a.y} x2={b.x} y2={b.y} className="dia-wire-line" />
              {connected && (
                <circle r="3.6" className="dia-wire-bead">
                  <animateMotion dur="2.2s" repeatCount="indefinite"
                    keyPoints="0;1" keyTimes="0;1"
                    path={`M${a.x},${a.y} L${b.x},${b.y}`} />
                </circle>
              )}
            </g>
          );
        })}

        {/* STORE nodes — hover to inspect/act; the resting ring marks computed HEAD */}
        {(["rulebook", "reasoner", "postgres"] as StoreId[]).map((id) => {
          const n = NODES[id];
          const st = nodeState(id);
          const isHead = id === status;
          const isHover = id === hovered;
          const r = id === "rulebook" ? 26 : 22;
          return (
            <g key={id}
               className={`dia-node dia-node-${id} st-${st} ${isHead ? "is-head" : ""} ${isHover ? "is-hover" : ""} ${disabled ? "" : "is-pickable"}`}
               transform={`translate(${n.x},${n.y})`}
               onMouseEnter={() => hover(id)}
               role="button" tabIndex={disabled ? -1 : 0}
               aria-label={`Inspect ${n.label} as authoritative`}
               onFocus={() => hover(id)} onBlur={() => hover(null)}>
              <circle r={r + 8} className="dia-node-hit" />
              <circle r={r} className="dia-node-halo" />
              <circle r={id === "rulebook" ? 19 : 16} className="dia-node-core" />
              {labels?.[id]?.inSync && <path className="dia-check" d="M-6,0 L-2,5 L7,-6" />}
              {isHead && <circle r={id === "rulebook" ? 23 : 19} className="dia-node-ring" />}
            </g>
          );
        })}

        {/* CLIENT node — an overlay reader, not a store. Hover picks which engine
            the app reads. It borrows the connected engine's drift color so a
            stale wiring is visible right on the apex. */}
        {(() => {
          const n = NODES.client;
          const st = nodeState(connectedEngine); // client mirrors the engine it reads
          const isHover = hovered === "client";
          return (
            <g className={`dia-node dia-node-client st-${st} ${isHover ? "is-hover" : ""} ${disabled ? "" : "is-pickable"}`}
               transform={`translate(${n.x},${n.y})`}
               onMouseEnter={() => hover("client")}
               role="button" tabIndex={disabled ? -1 : 0}
               aria-label="Choose which engine the app reads, and enter"
               onFocus={() => hover("client")} onBlur={() => hover(null)}>
              <circle r={26} className="dia-node-hit" />
              <circle r={18} className="dia-node-halo" />
              <rect x={-13} y={-11} width={26} height={22} rx={3.5} className="dia-client-screen" />
              <line x1={-5} y1={11} x2={5} y2={11} className="dia-client-stand" />
            </g>
          );
        })()}
      </svg>

      {/* labels pinned DIRECTLY under each node — absolutely placed in the same
          coordinate space as the SVG (percent of the 320×260 viewBox) so they
          sit on the diamond itself rather than re-drawing the structure below it.
          Still hoverable: hovering a label is the same as hovering its node. */}
      <div className="dia-labels">
        {(["reasoner", "client", "rulebook", "postgres"] as NodeId[]).map((id) => {
          const isClient = id === "client";
          const st = isClient ? nodeState(connectedEngine) : nodeState(id as StoreId);
          const meta: NodeLabel = !isClient ? (labels?.[id as StoreId] || {}) : {};
          const n = NODES[id];
          // node center as a fraction of the viewBox → percentage on the overlay.
          // sit the label just BELOW the node's halo (≈ node radius worth of drop).
          const leftPct = (n.x / VIEW_W) * 100;
          const topPct = ((n.y + LABEL_DROP) / VIEW_H) * 100;
          return (
            <div key={id}
              className={`dia-label dia-label-${id} st-${st} ${id === status ? "is-head" : ""} ${id === hovered ? "is-hover" : ""}`}
              style={{ left: `${leftPct}%`, top: `${topPct}%` }}
              onMouseEnter={() => hover(id)}>
              <span className="dia-label-name">{NODES[id].label}</span>
              <span className="dia-label-sub">{NODES[id].sub}</span>
              {isClient
                ? <span className="dia-badge-wired">▸ {NODES[connectedEngine].label}</span>
                : (id === status && <span className="dia-badge-head">HEAD</span>)}
              {!isClient && st === "stale" && id !== status && <span className="dia-badge-stale">trailing</span>}
              {!isClient && meta.ago && <span className="dia-label-ago">edited {meta.ago}</span>}
            </div>
          );
        })}
      </div>

      {/* the hover box, anchored to the hovered node. Present ONLY while hovering. */}
      {popover && anchor && (
        <div className={`dia-popover dia-popover-${hovered} origin-${anchor.origin}`} style={anchor as React.CSSProperties}
             onMouseEnter={() => hover(hovered)}>
          {popover}
        </div>
      )}

      {/* the reasoning pill — rides the legs while an engine→hub→engine sync runs */}
      {syncing && (
        <div className="dia-reasoning-pill">⟳ reasoning</div>
      )}
    </div>
  );
}
