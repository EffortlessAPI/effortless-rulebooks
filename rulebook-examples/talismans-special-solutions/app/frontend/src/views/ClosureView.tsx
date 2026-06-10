import React, { useMemo, useState, useRef } from "react";
import { kindOfType } from "../model";
import { DagCell } from "../explainer-dag";
import type { Situation, Handlers, Step, PrecedenceEdge, ClosurePair } from "../types";

// ===========================================================================
// CLOSURE VIEW — the transitive-closure "side trick", made visible and editable.
//
// THE TRICK: you assert only a handful of DIRECT "this step comes before that
// step" facts (the StepPrecedence edges). You never say "1 precedes 5". But the
// reasoner materializes ntwf:precedesStep as a TRANSITIVE relation, so 1→5 (and
// every other reachable pair) appears for free. This view shows BOTH layers at
// once and lets you EDIT the asserted layer by drag & drop — then watch the
// inferred layer recompute.
//
//   • The CHAIN (top): each step is a node with a draggable "→ then" handle.
//     Drag the handle from step A and drop it on step B to assert "A precedes B"
//     (writes a StepPrecedence row). Drop a node's own edge chip to remove it.
//   • The CLOSURE MATRIX (bottom): rows = from-step, cols = to-step. A SOLID
//     cell is an edge you asserted; a GHOST cell is one the reasoner INFERRED by
//     transitivity. The never-asserted 1→5 is a ghost cell — the headline.
//
// We compute NO precedence here. `sit.closure.pairs` already carries every pair
// with its is_inferred flag straight from the reasoner / the closure view. The
// only thing this view derives is presentation (which cell, which handle).
// ===========================================================================

interface ClosureViewProps {
  sit: Situation;
  handlers: Handlers;
}

export function ClosureView({ sit, handlers }: ClosureViewProps) {
  const steps = sit.steps;
  const byId = useMemo(() => Object.fromEntries(steps.map((s) => [s.id, s])) as Record<string, Step>, [steps]);
  const posOf = (id: string | null): number | string => byId[id ?? ""]?.position ?? "?";

  // Asserted edges (raw StepPrecedence rows) — directly editable.
  const asserted = sit.edges; // [{id, from, to}]
  const assertedSet = useMemo(
    () => new Set(asserted.map((e) => e.from + "→" + e.to)),
    [asserted]
  );

  // The full reasoned closure, split into the two layers.
  const pairs: ClosurePair[] = sit.closure?.pairs || [];
  const inferredPairs = pairs.filter((p) => p.is_inferred);

  // drag state: which step we're dragging an ordering-handle FROM
  const [dragFrom, setDragFrom] = useState<string | null>(null);
  const [hoverTo, setHoverTo] = useState<string | null>(null);

  // Assert "from precedes to" if it's a new, legal, non-self edge.
  const tryAssert = (from: string | null, to: string | null) => {
    setDragFrom(null);
    setHoverTo(null);
    if (!from || !to || from === to) return;
    if (assertedSet.has(from + "→" + to)) return;        // already a direct fact
    // Refuse a back-edge that would contradict the existing closure (a cycle):
    // if `to` already precedes `from` in the closure, dropping from→to makes a
    // loop. The reasoner would still expand it, but a DAG is the contract here.
    const reverseInClosure = pairs.some((p) => p.from_id === to && p.to_id === from);
    if (reverseInClosure) {
      // soft-refuse: flash nothing, just no-op (a cycle isn't a valid ordering)
      return;
    }
    handlers.addEdge(from, to);
  };

  return (
    <div className="closure">
      <div className="cl-intro">
        <div className="cl-intro-lead">
          You only ever state <b>direct</b> "comes before" facts. The reasoner makes
          <code>precedesStep</code> <b>transitive</b>, so every <i>reachable</i> pair is
          inferred for free — including pairs you never typed.
        </div>
        <div className="cl-counts">
          {/* asserted + inferred are leaf aggregations (no derivation graph of
              their own); the TOTAL is the explainable calc — clicking it shows
              both counts feeding it. */}
          <span className="cl-count asserted"><b>{sit.closure?.asserted ?? asserted.length}</b> asserted</span>
          <span className="cl-plus">+</span>
          <span className="cl-count inferred"><b>{sit.closure?.inferred ?? inferredPairs.length}</b> inferred</span>
          <span className="cl-eq">=</span>
          <span className="cl-count total">
            <DagCell table="Workflows" field="CountOfPrecedenceClosurePairs">
              <b>{sit.closure?.count ?? pairs.length}</b> total ordered pairs
            </DagCell>
          </span>
        </div>
      </div>

      {/* ---- THE CHAIN: drag a step's "then →" handle onto another step ---- */}
      <div className="cl-chainwrap">
        <div className="cl-section-label">
          Order the steps — drag a step's <span className="cl-handle-demo">then →</span> handle onto a later step
        </div>
        <div className="cl-chain"
          onDragEnd={() => { setDragFrom(null); setHoverTo(null); }}
        >
          {steps.map((s, i) => {
            const kind = kindOfType(s.executingAgentType);
            const isHover = hoverTo === s.id && dragFrom && dragFrom !== s.id;
            const isSource = dragFrom === s.id;
            return (
              <React.Fragment key={s.id}>
                <div
                  className={"cl-step k-" + kind + (isHover ? " drophot" : "") + (isSource ? " dragging" : "")}
                  onDragOver={(e: React.DragEvent<HTMLDivElement>) => { if (dragFrom && dragFrom !== s.id) { e.preventDefault(); setHoverTo(s.id); } }}
                  onDragLeave={() => setHoverTo((h) => (h === s.id ? null : h))}
                  onDrop={(e: React.DragEvent<HTMLDivElement>) => { e.preventDefault(); tryAssert(dragFrom, s.id); }}
                >
                  <div className="cl-step-pos">{s.position}</div>
                  <div className="cl-step-title">{s.title}</div>
                  {isHover && <div className="cl-drop-cue">drop → assert {posOf(dragFrom)} precedes {s.position}</div>}
                  {/* the draggable "then →" ordering handle */}
                  <div
                    className="cl-handle"
                    draggable
                    onDragStart={(e: React.DragEvent<HTMLDivElement>) => { e.dataTransfer.effectAllowed = "link"; setDragFrom(s.id); }}
                    title={`drag to set what comes after "${s.title}"`}
                  >
                    then →
                  </div>
                </div>
                {i < steps.length - 1 && <div className="cl-chain-gap" />}
              </React.Fragment>
            );
          })}
        </div>

        {/* asserted edges as removable chips — these are the ONLY facts in play */}
        <div className="cl-asserted-row">
          <span className="cl-asserted-label">asserted edges (raw facts):</span>
          {asserted.length === 0 && <span className="muted">none — drag a handle above to add one</span>}
          {asserted
            .slice()
            .sort((a, b) => (posOf(a.from) as number) - (posOf(b.from) as number) || (posOf(a.to) as number) - (posOf(b.to) as number))
            .map((e) => (
              <span key={e.id} className="cl-edge-chip">
                {posOf(e.from)} → {posOf(e.to)}
                <button className="cl-edge-x" title="remove this asserted edge"
                  onClick={() => handlers.removeEdge(e.id)}>×</button>
              </span>
            ))}
        </div>
      </div>

      {/* ---- THE CLOSURE MATRIX: solid = asserted, ghost = inferred -------- */}
      <ClosureMatrix steps={steps} pairs={pairs} assertedSet={assertedSet} posOf={posOf} />

      <p className="cl-foot muted">
        Solid cells are edges you <b>asserted</b>. Ghost cells the reasoner
        <b> inferred</b> by transitivity — you never stated them. Remove edge
        {" "}{firstChainHint(asserted, posOf)} and watch a whole column of ghosts vanish:
        the closure is recomputed, not stored.
      </p>
    </div>
  );
}

interface ClosureMatrixProps {
  steps: Step[];
  pairs: ClosurePair[];
  assertedSet: Set<string>;
  posOf: (id: string | null) => number | string;
}

// The N×N reachability grid. One glance shows the two layers and the headline
// inference (the corner pair you never asserted).
function ClosureMatrix({ steps, pairs, assertedSet, posOf }: ClosureMatrixProps) {
  const ids = steps.map((s) => s.id);
  const lookup = useMemo(() => {
    const m: Record<string, "inferred" | "asserted"> = {};
    for (const p of pairs) m[p.from_id + "→" + p.to_id] = p.is_inferred ? "inferred" : "asserted";
    return m;
  }, [pairs]);

  return (
    <div className="cl-matrixwrap">
      <div className="cl-section-label">Closure matrix — does row-step precede col-step?</div>
      <div className="cl-matrix" style={{ gridTemplateColumns: `auto repeat(${steps.length}, 1fr)` }}>
        {/* header row */}
        <div className="cl-mcell cl-mcorner">from ╲ to</div>
        {steps.map((s) => (
          <div key={"h" + s.id} className="cl-mcell cl-mhead">{s.position}</div>
        ))}
        {/* body rows */}
        {steps.map((rs) => (
          <React.Fragment key={"r" + rs.id}>
            <div className="cl-mcell cl-mrow" title={rs.title}>{rs.position}. <span className="cl-mrow-t">{rs.title}</span></div>
            {ids.map((toId) => {
              const key = rs.id + "→" + toId;
              const state = rs.id === toId ? "self" : (lookup[key] || "none");
              const cls = "cl-mcell cl-cell " + state;
              const title =
                state === "asserted" ? `asserted: ${rs.position} → ${posOf(toId)}`
                : state === "inferred" ? `inferred (transitive): ${rs.position} → ${posOf(toId)} — never asserted`
                : state === "self" ? "—"
                : "not ordered";
              return (
                <div key={key} className={cls} title={title}>
                  {state === "asserted" && <span className="cl-mark solid">●</span>}
                  {state === "inferred" && <span className="cl-mark ghost">○</span>}
                  {state === "self" && <span className="cl-mark self">·</span>}
                </div>
              );
            })}
          </React.Fragment>
        ))}
      </div>
      <div className="cl-legend">
        <span className="cl-leg"><span className="cl-mark solid">●</span> asserted (a fact you stated)</span>
        <span className="cl-leg"><span className="cl-mark ghost">○</span> inferred (reasoner, by transitivity)</span>
      </div>
    </div>
  );
}

// Pick a representative asserted edge to name in the footer hint.
function firstChainHint(asserted: PrecedenceEdge[], posOf: (id: string | null) => number | string): string {
  if (!asserted.length) return "";
  const e = asserted
    .slice()
    .sort((a, b) => (posOf(a.from) as number) - (posOf(b.from) as number))[0];
  return `${posOf(e.from)}→${posOf(e.to)}`;
}
