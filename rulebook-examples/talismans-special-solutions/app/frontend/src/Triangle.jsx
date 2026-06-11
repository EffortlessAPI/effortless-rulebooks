import React from "react";

// ===========================================================================
// Triangle.jsx — the Head & Legs drift visual, now the ONLY control.
//
// The rulebook is the HEAD (apex). Its two LEGS drop into the engine stores:
//   left leg  → db.json (the OWL / SHACL reasoner)
//   right leg → the Postgres views
//
// This component is presentational + one interaction: the three nodes are
// CLICKABLE. Clicking a node tells the parent "make this store HEAD" — that's
// the entire former HEAD-selector, folded into the picture. SyncPanel owns the
// drift data and hands this component a fully-resolved view model plus an
// onPick(store) callback; the floaty box of diffs + push directions is rendered
// by the parent as `popover` and positioned next to the focused node here.
//
//   nodeState(id) -> "head" | "lit" | "stale" | "diverged"
//   legPlan       -> { left: "down"|"up"|"down-delayed"|null, right: ... }
//   labels        -> { id: { inSync, ago } }
//   focus         -> the store currently designated HEAD (drives the gold ring)
//   syncing       -> a sync build is running (sparks animate)
//   onPick(id)    -> user clicked node `id` to make it HEAD
//   disabled      -> clicks are inert while a build runs
//   popover       -> React node floated next to the focused store
// ===========================================================================

// Node geometry in the 320×220 SVG viewBox. Head at top-center; legs at the base.
export const NODES = {
  rulebook: { x: 160, y: 34, label: "Rulebook", sub: "the hub · HEAD" },
  reasoner: { x: 44, y: 180, label: "Reasoner", sub: "db.json · OWL/SHACL" },
  postgres: { x: 276, y: 180, label: "Postgres", sub: "vw_* views" },
};
// Where the floaty box anchors, relative to the .triangle box. The box overlays
// the lower half of the picture and biases toward the focused store, so it reads
// as "popping out of" that node. SyncPanel renders the SVG (~250px tall) above;
// the box starts a little below the apex so the head node stays visible. The
// `origin` drives transform-origin / horizontal alignment in CSS.
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
  nodeState, legPlan, labels, focus, syncing,
  onPick, disabled, popover,
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

  const pick = (id) => { if (!disabled && onPick) onPick(id); };
  const anchor = focus ? POPOVER_ANCHOR[focus] : null;

  return (
    <div className={`triangle ${syncing ? "is-syncing" : ""} focus-${focus || "none"} ${disabled ? "is-disabled" : ""}`}>
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

        {/* nodes — clickable to set HEAD */}
        {Object.entries(NODES).map(([id, n]) => {
          const st = nodeState(id);
          const isFocus = id === focus;
          const r = id === "rulebook" ? 26 : 22;
          return (
            <g key={id}
               className={`tri-node tri-node-${id} st-${st} ${isFocus ? "is-focus" : ""} ${disabled ? "" : "is-pickable"}`}
               transform={`translate(${n.x},${n.y})`}
               onClick={() => pick(id)}
               role="button" tabIndex={disabled ? -1 : 0}
               aria-label={`Make ${n.label} the authoritative HEAD`}
               onKeyDown={(e) => { if (e.key === "Enter" || e.key === " ") { e.preventDefault(); pick(id); } }}>
              {/* a roomy invisible hit target so the whole node is easy to click */}
              <circle r={r + 8} className="tri-node-hit" />
              <circle r={r} className="tri-node-halo" />
              <circle r={id === "rulebook" ? 19 : 16} className="tri-node-core" />
              {labels?.[id]?.inSync && <path className="tri-check" d="M-6,0 L-2,5 L7,-6" />}
              {isFocus && <circle r={id === "rulebook" ? 23 : 19} className="tri-node-ring" />}
            </g>
          );
        })}
      </svg>

      {/* labels under the svg, aligned to the three nodes — also clickable */}
      <div className="tri-labels">
        {["reasoner", "rulebook", "postgres"].map((id) => {
          const st = nodeState(id);
          const meta = labels?.[id] || {};
          return (
            <button key={id} type="button" disabled={disabled}
              className={`tri-label tri-label-${id} st-${st} ${id === focus ? "is-focus" : ""}`}
              onClick={() => pick(id)}>
              <span className="tri-label-name">{NODES[id].label}</span>
              <span className="tri-label-sub">{NODES[id].sub}</span>
              {id === focus && <span className="tri-badge-head">HEAD</span>}
              {st === "stale" && id !== focus && <span className="tri-badge-stale">trailing</span>}
              {meta.ago && <span className="tri-label-ago">edited {meta.ago}</span>}
            </button>
          );
        })}
      </div>

      {/* the floaty box: diffs + directional pushes, anchored to the focused node */}
      {popover && anchor && (
        <div className={`tri-popover tri-popover-${focus} origin-${anchor.origin}`} style={anchor}>
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
