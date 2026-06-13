import React, { useMemo, useState, useEffect, useRef, useLayoutEffect } from "react";
import { kindOfType } from "../model";
import { DagCell } from "../explainer-dag";
import { LineageView } from "./LineageView";
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
//   • The LAYERED DAG (top): steps are laid out in 2D, like the org chart —
//     the x-axis is the derived SequencePosition (rank), so every step in the
//     same rank STACKS vertically in one column. Assert "A precedes B" (drag A's
//     "then →" handle onto B) and B's rank jumps to the next column: you literally
//     watch the inference push the card to the RIGHT. Asserted edges are drawn as
//     solid arrows across the columns. When nothing is ordered, all steps tie at
//     rank 1 and stack in a single column; a finished chain fans out 1→n columns.
//   • The CLOSURE MATRIX (middle): rows = from-step, cols = to-step. A SOLID
//     cell is an edge you asserted; a GHOST cell is one the reasoner INFERRED by
//     transitivity. The never-asserted 1→5 is a ghost cell — the headline.
//   • The ARTIFACT LINEAGE (bottom): the SAME transitive-closure machine pointed
//     at a different relation (wasDerivedFrom). It lived on its own "Lineage" tab,
//     but it's one story with step ordering (the artifacts are even sequenced by
//     their producer-step), so it now rides here, below the step closure.
//
// We compute NO precedence here. `sit.closure.pairs` already carries every pair
// with its is_inferred flag straight from the reasoner / the closure view. The
// only thing this view derives is presentation (which column, which cell, which
// edge endpoints to draw).
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
          <span>Order the steps — drag a step's <span className="cl-handle-demo">then →</span> handle onto another. Same-rank steps <b>stack</b>; asserting an edge pushes a step into the <b>next column to the right</b>.</span>
          <span className="cl-label-tools">
            <span className="cl-label-note">labels are nominal (like graph-coloring) →</span>
            <button className="cl-label-btn" onClick={randomizeLabels} title="Scramble the label assignment client-side — the closure structure is invariant">⤮ Randomize</button>
            <button className="cl-label-btn cl-label-btn--ghost" onClick={relabelInOrder} title="Re-assign A,B,C… straight down the current flow order — once fully wired, that's a clean A→E">↧ Relabel A→E in order</button>
          </span>
        </div>
        <StepDag
          steps={steps}
          letOf={letOf}
          colorOf={colorOf}
          asserted={asserted}
          dragFrom={dragFrom}
          setDragFrom={setDragFrom}
          hoverTo={hoverTo}
          setHoverTo={setHoverTo}
          onAssert={tryAssert}
        />

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

      {/* ---- SAME MACHINE, DIFFERENT RELATION: artifact lineage ----------- */}
      <div className="cl-lineage-section">
        <div className="cl-section-divider">
          <span className="cl-section-divider-label">
            ↓ the same transitive-closure machine — pointed at <code>wasDerivedFrom</code>
          </span>
        </div>
        <p className="cl-lineage-bridge muted">
          The step order above and the artifact lineage below are <b>one construct</b>: assert the
          adjacent links, the reasoner closes the reachable pairs. They're coupled, too — each artifact
          is sequenced by its <b>producer step</b>, so re-ordering steps above re-threads the chain below.
          That's why these used to be two tabs and are now one.
        </p>
        <LineageView sit={sit} handlers={handlers} embedded />
      </div>
    </div>
  );
}

// ===========================================================================
// STEP DAG — the step set laid out in 2D by derived rank (SequencePosition).
// Each column is one rank; steps that tie at a rank stack vertically in that
// column. Asserting "A precedes B" (drag A's "then →" onto B) bumps B's rank, so
// it slides into the next column — the inference is the layout moving. Asserted
// edges are drawn as solid arrows over the columns (measured from the live DOM,
// so they stay glued to the cards through reflow / reload). Computes nothing
// about precedence: the column index IS s.position, straight from the substrate.
// ===========================================================================
interface StepDagProps {
  steps: Step[];
  asserted: PrecedenceEdge[];
  letOf: (id: string | null) => string;
  colorOf: (id: string | null) => string;
  dragFrom: string | null;
  setDragFrom: (id: string | null) => void;
  hoverTo: string | null;
  setHoverTo: (fn: string | null | ((h: string | null) => string | null)) => void;
  onAssert: (from: string | null, to: string | null) => void;
}

interface Seg { x1: number; y1: number; x2: number; y2: number; key: string }

function StepDag({ steps, asserted, letOf, colorOf, dragFrom, setDragFrom, hoverTo, setHoverTo, onAssert }: StepDagProps) {
  // Group steps into columns by their derived rank (SequencePosition). Within a
  // column, order by the nominal label so the stack is stable across reloads.
  const columns = useMemo(() => {
    const byRank: Record<number, Step[]> = {};
    for (const s of steps) (byRank[s.position] ||= []).push(s);
    return Object.keys(byRank)
      .map(Number)
      .sort((a, b) => a - b)
      .map((rank) => ({
        rank,
        items: byRank[rank].slice().sort((a, b) => letOf(a.id).localeCompare(letOf(b.id))),
      }));
  }, [steps, letOf]);

  // A stable fingerprint of the actual card arrangement (ranks + the vertical
  // order within each column). Randomizing the nominal labels re-sorts a column
  // WITHOUT changing the container size — so the ResizeObserver wouldn't fire and
  // the arrows would go stale. Keying the measure effect on this string re-draws
  // the arrows the instant the layout rearranges, label-shuffle included.
  const layoutKey = useMemo(
    () => columns.map((c) => c.rank + ":" + c.items.map((s) => s.id).join(",")).join("|"),
    [columns],
  );

  const wrapRef = useRef<HTMLDivElement | null>(null);
  const cardRefs = useRef<Record<string, HTMLDivElement | null>>({});
  const [segs, setSegs] = useState<Seg[]>([]);
  const [dims, setDims] = useState<{ w: number; h: number }>({ w: 0, h: 0 });

  // Measure each asserted edge's endpoints from the live DOM and draw a line from
  // the source card's right edge to the target card's left edge. Re-runs whenever
  // the step set / asserted edges change, and on container resize.
  useLayoutEffect(() => {
    const measure = () => {
      const wrap = wrapRef.current;
      if (!wrap) return;
      const cr = wrap.getBoundingClientRect();
      const next: Seg[] = [];
      for (const e of asserted) {
        const a = cardRefs.current[e.from];
        const b = cardRefs.current[e.to];
        if (!a || !b) continue;
        const ar = a.getBoundingClientRect();
        const br = b.getBoundingClientRect();
        next.push({
          x1: ar.right - cr.left,
          y1: ar.top - cr.top + ar.height / 2,
          x2: br.left - cr.left,
          y2: br.top - cr.top + br.height / 2,
          key: e.id,
        });
      }
      setSegs((prev) => (sameSegs(prev, next) ? prev : next));
      setDims((prev) => {
        const w = wrap.scrollWidth, h = wrap.scrollHeight;
        return prev.w === w && prev.h === h ? prev : { w, h };
      });
    };
    measure();
    const ro = new ResizeObserver(measure);
    if (wrapRef.current) ro.observe(wrapRef.current);
    window.addEventListener("resize", measure);
    return () => { ro.disconnect(); window.removeEventListener("resize", measure); };
  }, [layoutKey, asserted]);

  return (
    <div className="cl-dag" ref={wrapRef} onDragEnd={() => { setDragFrom(null); setHoverTo(null); }}>
      {/* asserted-edge arrows, drawn over the columns */}
      <svg className="cl-dag-svg" width={dims.w || "100%"} height={dims.h || "100%"} aria-hidden="true">
        <defs>
          <marker id="cl-arrow" markerWidth="9" markerHeight="9" refX="7" refY="3" orient="auto">
            <path d="M0,0 L7,3 L0,6 Z" fill="var(--accent)" />
          </marker>
        </defs>
        {segs.map((s) => (
          <line key={s.key} x1={s.x1} y1={s.y1} x2={s.x2} y2={s.y2}
            className="cl-dagedge" markerEnd="url(#cl-arrow)" />
        ))}
      </svg>

      <div className="cl-dag-cols">
        {columns.map((col) => (
          <div className="cl-dag-col" key={col.rank}>
            <div className="cl-dag-colhead" title="derived SequencePosition — recomputed from the closure">
              pos {col.rank}{col.items.length > 1 ? <span className="cl-dag-tie"> · {col.items.length} tied</span> : null}
            </div>
            <div className="cl-dag-stack">
              {col.items.map((s) => {
                const kind = kindOfType(s.executingAgentType);
                const isHover = hoverTo === s.id && dragFrom && dragFrom !== s.id;
                const isSource = dragFrom === s.id;
                return (
                  <div
                    key={s.id}
                    ref={(el) => { cardRefs.current[s.id] = el; }}
                    className={"cl-step cl-dagcard k-" + kind + (isHover ? " drophot" : "") + (isSource ? " dragging" : "")}
                    onDragOver={(e: React.DragEvent<HTMLDivElement>) => { if (dragFrom && dragFrom !== s.id) { e.preventDefault(); setHoverTo(s.id); } }}
                    onDragLeave={() => setHoverTo((h) => (h === s.id ? null : h))}
                    onDrop={(e: React.DragEvent<HTMLDivElement>) => { e.preventDefault(); onAssert(dragFrom, s.id); }}
                  >
                    <div className="cl-step-pos" title="nominal label — identifies this card; reassignable, no structural meaning"
                      style={{ background: colorOf(s.id), borderColor: colorOf(s.id), color: "#19102e" }}>{letOf(s.id)}</div>
                    <div className="cl-step-title">{s.title}</div>
                    {isHover && <div className="cl-drop-cue">drop → assert {letOf(dragFrom)} precedes {letOf(s.id)}</div>}
                    <div
                      className="cl-handle"
                      draggable
                      onDragStart={(e: React.DragEvent<HTMLDivElement>) => { e.dataTransfer.effectAllowed = "link"; setDragFrom(s.id); }}
                      title={`drag to set what comes after "${s.title}"`}
                    >
                      then →
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

// Cheap structural equality for the segment list, so measure() doesn't thrash
// React state when nothing actually moved.
function sameSegs(a: Seg[], b: Seg[]): boolean {
  if (a.length !== b.length) return false;
  for (let i = 0; i < a.length; i++) {
    const p = a[i], q = b[i];
    if (p.key !== q.key || p.x1 !== q.x1 || p.y1 !== q.y1 || p.x2 !== q.x2 || p.y2 !== q.y2) return false;
  }
  return true;
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
