import React, { useMemo, useState, useEffect, useRef } from "react";
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

// Nominal label colors — like the colors in a graph-coloring problem, the
// specific hues carry no meaning; they're just a second way to tell cards apart.
const LABEL_COLORS = ["#b692ff", "#ffc14d", "#5ad1c4", "#ff8fa3", "#7fb2ff", "#c0e35a", "#ff9d5c", "#d29bff"];

interface ClosureViewProps {
  sit: Situation;
  handlers: Handlers;
}

export function ClosureView({ sit, handlers }: ClosureViewProps) {
  const steps = sit.steps;
  const byId = useMemo(() => Object.fromEntries(steps.map((s) => [s.id, s])) as Record<string, Step>, [steps]);

  // NOMINAL LABELS vs. DERIVED RANK vs. REAL STRUCTURE.
  // `position` (s.position) is the derived SequencePosition (PrecedingStepCount + 1)
  // — it TIES (all 1 when nothing is ordered) and reshuffles as you assert edges,
  // so it can't name a card. We give each card a LABEL (a letter + a color)
  // instead. But the label is purely NOMINAL — like the colors in the 3-coloring
  // problem, any bijection works; only the precedence *relation* is real.
  //
  // `order` is a client-only assignment of step ids → label slot (A=0, B=1, …).
  // It is FROZEN once set, so labels stay glued to their cards while you wire edges
  // (stable identity). Two buttons act on it:
  //   • Randomize  — scramble the assignment (proves labels carry no meaning;
  //                  the closure matrix keeps the exact same pattern, just renamed).
  //   • Relabel    — re-assign A,B,C… straight down the CURRENT flow order. Once the
  //                  steps are fully wired, the flow order is a clean 1..n, so this
  //                  lands you on A,B,C,D,E matching the sequence — but only then.
  const displayOrder = useMemo(() => steps.map((s) => s.id), [steps]); // backend sorts by SequencePosition

  const [order, setOrder] = useState<string[] | null>(null);
  // Freeze to the current flow order once steps exist (and re-baseline if the step
  // SET changes shape — added/removed). A pure rewire keeps the same length, so the
  // frozen assignment survives and labels don't jump while you drag.
  useEffect(() => {
    setOrder((prev) => (prev && prev.length === steps.length ? prev : displayOrder.slice()));
  }, [displayOrder, steps.length]);

  const effectiveOrder = order && order.length === steps.length ? order : displayOrder;
  const slotOf = useMemo(() => {
    const m: Record<string, number> = {};
    effectiveOrder.forEach((id, i) => { m[id] = i; });
    return m;
  }, [effectiveOrder]);
  const letOf = (id: string | null): string => (id != null && slotOf[id] != null ? String.fromCharCode(65 + slotOf[id]) : "?");
  const colorOf = (id: string | null): string => (id != null && slotOf[id] != null ? LABEL_COLORS[slotOf[id] % LABEL_COLORS.length] : "transparent");

  const randomizeLabels = () => {
    const a = effectiveOrder.slice();
    for (let i = a.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [a[i], a[j]] = [a[j], a[i]];
    }
    setOrder(a);
  };
  // Relabel A,B,C… down the current flow order (NOT a revert to any "original").
  const relabelInOrder = () => setOrder(displayOrder.slice());

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
        <div className="cl-section-label cl-section-label--row">
          <span>Order the steps — drag a step's <span className="cl-handle-demo">then →</span> handle onto a later step</span>
          <span className="cl-label-tools">
            <span className="cl-label-note">labels are nominal (like graph-coloring) →</span>
            <button className="cl-label-btn" onClick={randomizeLabels} title="Scramble the label assignment client-side — the closure structure is invariant">⤮ Randomize</button>
            <button className="cl-label-btn cl-label-btn--ghost" onClick={relabelInOrder} title="Re-assign A,B,C… straight down the current flow order — once fully wired, that's a clean A→E">↧ Relabel A→E in order</button>
          </span>
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
                  <div className="cl-step-pos" title="nominal label — identifies this card; reassignable, no structural meaning"
                    style={{ background: colorOf(s.id), borderColor: colorOf(s.id), color: "#19102e" }}>{letOf(s.id)}</div>
                  <div className="cl-step-rank" title="derived SequencePosition — recomputed from the closure">pos {s.position}</div>
                  <div className="cl-step-title">{s.title}</div>
                  {isHover && <div className="cl-drop-cue">drop → assert {letOf(dragFrom)} precedes {letOf(s.id)}</div>}
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
            .sort((a, b) => letOf(a.from).localeCompare(letOf(b.from)) || letOf(a.to).localeCompare(letOf(b.to)))
            .map((e) => (
              <span key={e.id} className="cl-edge-chip">
                {letOf(e.from)} → {letOf(e.to)}
                <button className="cl-edge-x" title="remove this asserted edge"
                  onClick={() => handlers.removeEdge(e.id)}>×</button>
              </span>
            ))}
        </div>
      </div>

      {/* ---- THE CLOSURE MATRIX: solid = asserted, ghost = inferred -------- */}
      <ClosureMatrix steps={steps} pairs={pairs} letOf={letOf} colorOf={colorOf} />

      <p className="cl-foot muted">
        Solid cells are edges you <b>asserted</b>. Ghost cells the reasoner
        <b> inferred</b> by transitivity — you never stated them. Remove edge
        {" "}{firstChainHint(asserted, letOf)} and watch a whole column of ghosts vanish:
        the closure is recomputed, not stored. <b>Randomize the labels</b> and this
        whole grid keeps its shape — the labels are nominal, only the ordering is real.
      </p>
    </div>
  );
}

interface ClosureMatrixProps {
  steps: Step[];
  pairs: ClosurePair[];
  letOf: (id: string | null) => string;
  colorOf: (id: string | null) => string;
}

// The N×N reachability grid. One glance shows the two layers and the headline
// inference (the corner pair you never asserted).
function ClosureMatrix({ steps, pairs, letOf, colorOf }: ClosureMatrixProps) {
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
          <div key={"h" + s.id} className="cl-mcell cl-mhead">
            <span className="cl-mlabel" style={{ background: colorOf(s.id), color: "#19102e" }}>{letOf(s.id)}</span>
          </div>
        ))}
        {/* body rows */}
        {steps.map((rs) => (
          <React.Fragment key={"r" + rs.id}>
            <div className="cl-mcell cl-mrow" title={rs.title}>
              <span className="cl-mlabel" style={{ background: colorOf(rs.id), color: "#19102e" }}>{letOf(rs.id)}</span>
              <span className="cl-mrow-t">{rs.title}</span>
            </div>
            {ids.map((toId) => {
              const key = rs.id + "→" + toId;
              const state = rs.id === toId ? "self" : (lookup[key] || "none");
              const cls = "cl-mcell cl-cell " + state;
              const title =
                state === "asserted" ? `asserted: ${letOf(rs.id)} → ${letOf(toId)}`
                : state === "inferred" ? `inferred (transitive): ${letOf(rs.id)} → ${letOf(toId)} — never asserted`
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
function firstChainHint(asserted: PrecedenceEdge[], letOf: (id: string | null) => string): string {
  if (!asserted.length) return "";
  const e = asserted
    .slice()
    .sort((a, b) => letOf(a.from).localeCompare(letOf(b.from)))[0];
  return `${letOf(e.from)}→${letOf(e.to)}`;
}
