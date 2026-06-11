import React from "react";

// ===========================================================================
// Triangle.jsx — the Head & Legs drift visual. NOTHING is privileged.
//
// The rulebook is the hub (apex). Its two LEGS drop into the engine stores:
//   left leg  → db.json (the OWL / SHACL reasoner)
//   right leg → the Postgres views
//
// Resting state shows the COMPUTED drift status — who is HEAD *right now*:
//   • all three agree            → the Rulebook is HEAD (the natural resting
//     state; the hub is authoritative because the legs match it)
//   • they disagree              → the most-recently-edited store is shown as
//     HEAD, but ONLY as a status read-out. Recency does not privilege it — the
//     whole point of this board is that any store can be made authoritative.
//
// There is NO selection. You HOVER a node to ask "if THIS store were
// authoritative, what would the others lose, and how would I push it?" — and you
// act straight from the hover. Move away and the box is gone, so it can never
// cover the other nodes (the old sticky box trapped you on one HEAD).
//
// SyncPanel owns the data and hands this component a resolved view model:
//   nodeState(id) -> "head" | "lit" | "stale"     (resting drift status)
//   status        -> the computed HEAD store id (drives the resting gold ring)
//   hovered       -> the store currently hovered (drives the box + a hover ring)
//   legPlan       -> { left, right } leg-flow plan while a sync runs
//   labels        -> { id: { inSync, ago } }
//   syncing       -> a sync build is running (sparks animate)
//   onHover(id|null) -> pointer entered/left node `id`
//   disabled      -> hover is inert while a build runs
//   popover       -> React node floated next to the hovered node
// ===========================================================================

// Node geometry in the 320×220 SVG viewBox. Head at top-center; legs at the base.
export const NODES = {
  rulebook: { x: 160, y: 34, label: "Rulebook", sub: "the hub" },
  reasoner: { x: 44, y: 180, label: "Reasoner", sub: "db.json · OWL/SHACL" },
  postgres: { x: 276, y: 180, label: "Postgres", sub: "vw_* views" },
};
// Where the hover box anchors, relative to the .triangle box. The box appears
// only while a node is hovered; it biases toward that node so it reads as
// "popping out of" it. `origin` drives transform-origin / horizontal alignment.
const POPOVER_ANCHOR = {
  rulebook: { top: "30%", left: "50%", origin: "top" },
  reasoner: { top: "44%", left: "-2%", origin: "left" },
  postgres: { top: "44%", right: "-2%", origin: "right" },
};
const LEGS = [
  { id: "left", from: "rulebook", to: "reasoner", engine: "reasoner" },
  { id: "right", from: "rulebook", to: "postgres", engine: "postgres" },
];

export default function Triangle({
  nodeState, status, hovered, legPlan, labels, syncing,
  onHover, disabled, popover,
}) {
  const legClass = (leg) => {
    const f = legPlan?.[leg.id] || null;
    return [
      "tri-leg",
      `tri-leg-${leg.id}`,
      f ? `flow-${f}` : "",
      syncing ? "is-syncing" : "",
    ].filter(Boolean).join(" ");
  };

  const hover = (id) => { if (!disabled && onHover) onHover(id); };
  const anchor = hovered ? POPOVER_ANCHOR[hovered] : null;

  return (
    <div className={`triangle ${syncing ? "is-syncing" : ""} status-${status || "none"} ${disabled ? "is-disabled" : ""}`}
         onMouseLeave={() => hover(null)}>
      <svg className="tri-svg" viewBox="0 0 320 220" role="img" aria-label="store drift triangle">
        {/* legs */}
        {LEGS.map((leg) => {
          const a = NODES[leg.from], b = NODES[leg.to];
          const f = legPlan?.[leg.id] || null;
          return (
            <g key={leg.id} className={legClass(leg)}>
              <line x1={a.x} y1={a.y} x2={b.x} y2={b.y} className="tri-leg-base" />
              <line x1={a.x} y1={a.y} x2={b.x} y2={b.y} className="tri-leg-glow" />
              {f && (
                <circle r="4.5" className="tri-spark">
                  <animateMotion dur="1.4s" repeatCount="indefinite"
                    keyPoints={f === "up" ? "1;0" : "0;1"} keyTimes="0;1"
                    path={`M${a.x},${a.y} L${b.x},${b.y}`} />
                </circle>
              )}
            </g>
          );
        })}

        {/* nodes — hover to inspect/act; the resting ring marks the computed HEAD */}
        {Object.entries(NODES).map(([id, n]) => {
          const st = nodeState(id);
          const isHead = id === status;
          const isHover = id === hovered;
          const r = id === "rulebook" ? 26 : 22;
          return (
            <g key={id}
               className={`tri-node tri-node-${id} st-${st} ${isHead ? "is-head" : ""} ${isHover ? "is-hover" : ""} ${disabled ? "" : "is-pickable"}`}
               transform={`translate(${n.x},${n.y})`}
               onMouseEnter={() => hover(id)}
               role="button" tabIndex={disabled ? -1 : 0}
               aria-label={`Inspect ${n.label} as authoritative`}
               onFocus={() => hover(id)} onBlur={() => hover(null)}>
              {/* a roomy invisible hit target so the whole node is easy to hover */}
              <circle r={r + 8} className="tri-node-hit" />
              <circle r={r} className="tri-node-halo" />
              <circle r={id === "rulebook" ? 19 : 16} className="tri-node-core" />
              {labels?.[id]?.inSync && <path className="tri-check" d="M-6,0 L-2,5 L7,-6" />}
              {isHead && <circle r={id === "rulebook" ? 23 : 19} className="tri-node-ring" />}
            </g>
          );
        })}
      </svg>

      {/* labels under the svg, aligned to the three nodes — also hoverable */}
      <div className="tri-labels">
        {["reasoner", "rulebook", "postgres"].map((id) => {
          const st = nodeState(id);
          const meta = labels?.[id] || {};
          return (
            <div key={id}
              className={`tri-label tri-label-${id} st-${st} ${id === status ? "is-head" : ""} ${id === hovered ? "is-hover" : ""}`}
              onMouseEnter={() => hover(id)}>
              <span className="tri-label-name">{NODES[id].label}</span>
              <span className="tri-label-sub">{NODES[id].sub}</span>
              {id === status && <span className="tri-badge-head">HEAD</span>}
              {st === "stale" && id !== status && <span className="tri-badge-stale">trailing</span>}
              {meta.ago && <span className="tri-label-ago">edited {meta.ago}</span>}
            </div>
          );
        })}
      </div>

      {/* the hover box: diffs + pushes (or just launch/reset when in sync),
          anchored to the hovered node. Present ONLY while hovering. */}
      {popover && anchor && (
        <div className={`tri-popover tri-popover-${hovered} origin-${anchor.origin}`} style={anchor}
             onMouseEnter={() => hover(hovered)}>
          {popover}
        </div>
      )}

      {/* the reasoning pill — rides the legs while an engine→hub→engine sync runs */}
      {syncing && (
        <div className="tri-reasoning-pill">⟳ reasoning</div>
      )}
    </div>
  );
}
