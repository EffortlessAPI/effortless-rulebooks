import React from "react";

// ===========================================================================
// Triangle.jsx — the Head & Legs drift VISUAL (presentational only).
//
// The rulebook is the HEAD (apex). Its two LEGS drop into the engine stores:
//   left leg  → db.json (the OWL / SHACL reasoner)
//   right leg → the Postgres views
//
// This component does NO fetching and holds NO drift logic. SyncPanel owns the
// triangle/diff data and the active HEAD choice, and hands this component a
// fully-resolved view model:
//
//   nodeState(id) -> "ahead" | "lit" | "stale" | "diverged" | "head"
//   legPlan       -> { left: "down"|"up"|"down-delayed"|null, right: ... }
//   labels        -> { id: { ahead, stale, ago } }
//   focus         -> the store currently designated HEAD (drives the gold ring)
//   syncing       -> a sync build is running (sparks animate)
//
// Keeping it presentational means the SAME triangle renders both the recency
// SUGGESTION and a manual HEAD pick — the parent just changes the view model.
// ===========================================================================

// Node geometry in the 320×220 SVG viewBox. Head at top-center; legs at the base.
export const NODES = {
  rulebook: { x: 160, y: 34, label: "Rulebook", sub: "the hub · HEAD" },
  reasoner: { x: 44, y: 180, label: "Reasoner", sub: "db.json · OWL/SHACL" },
  postgres: { x: 276, y: 180, label: "Postgres", sub: "vw_* views" },
};
const LEGS = [
  { id: "left", from: "rulebook", to: "reasoner", engine: "reasoner" },
  { id: "right", from: "rulebook", to: "postgres", engine: "postgres" },
];

export default function Triangle({ nodeState, legPlan, labels, focus, syncing }) {
  const legClass = (leg) => {
    const f = legPlan?.[leg.id] || null;
    return [
      "tri-leg",
      `tri-leg-${leg.id}`,
      f ? `flow-${f}` : "",
      syncing ? "is-syncing" : "",
    ].filter(Boolean).join(" ");
  };

  return (
    <div className={`triangle ${syncing ? "is-syncing" : ""} focus-${focus || "none"}`}>
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

        {/* nodes */}
        {Object.entries(NODES).map(([id, n]) => {
          const st = nodeState(id);
          const isFocus = id === focus;
          return (
            <g key={id} className={`tri-node tri-node-${id} st-${st} ${isFocus ? "is-focus" : ""}`}
               transform={`translate(${n.x},${n.y})`}>
              <circle r={id === "rulebook" ? 26 : 22} className="tri-node-halo" />
              <circle r={id === "rulebook" ? 19 : 16} className="tri-node-core" />
              {labels?.[id]?.inSync && <path className="tri-check" d="M-6,0 L-2,5 L7,-6" />}
              {isFocus && <circle r={id === "rulebook" ? 23 : 19} className="tri-node-ring" />}
            </g>
          );
        })}
      </svg>

      {/* labels under the svg, aligned to the three nodes */}
      <div className="tri-labels">
        {["reasoner", "rulebook", "postgres"].map((id) => {
          const st = nodeState(id);
          const meta = labels?.[id] || {};
          return (
            <div key={id} className={`tri-label tri-label-${id} st-${st} ${id === focus ? "is-focus" : ""}`}>
              <span className="tri-label-name">{NODES[id].label}</span>
              <span className="tri-label-sub">{NODES[id].sub}</span>
              {id === focus && <span className="tri-badge-head">HEAD</span>}
              {st === "ahead" && id !== focus && <span className="tri-badge-ahead">ahead</span>}
              {st === "stale" && <span className="tri-badge-stale">trailing</span>}
              {meta.ago && <span className="tri-label-ago">edited {meta.ago}</span>}
            </div>
          );
        })}
      </div>

      {/* the reasoning pill — rides the legs while an engine→hub→engine sync runs */}
      {syncing && (
        <div className="tri-reasoning-pill">⟳ reasoning</div>
      )}
    </div>
  );
}
