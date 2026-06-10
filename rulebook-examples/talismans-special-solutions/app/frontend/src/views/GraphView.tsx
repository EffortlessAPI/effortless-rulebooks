import React, { useMemo, useState } from "react";
import { KIND } from "../model";
import { DagCell } from "../explainer-dag";
import type { Situation, Handlers, GraphNode, GraphEdge } from "../types";

// ===========================================================================
// GRAPH VIEW — the reasoned network itself, made legible.
//
// Nodes: steps (bottom row, in order) and the agents that execute them (top).
// Edges: agent —executes→ step (assignment) and step —precedes→ step (order).
// Click any node to focus it: its direct connections light up, the rest dim,
// so you can SEE "this step is run by that agent, and precedes these steps."
// Deterministic layout (no physics lib) — stable, readable, no jitter.
// ===========================================================================

const W = 860, H = 460;
const STEP_Y = 330, AGENT_Y = 90;

type Point = { x: number; y: number };

interface GraphViewProps {
  sit: Situation;
  handlers: Handlers;
}

export function GraphView({ sit, handlers }: GraphViewProps) {
  const [focus, setFocus] = useState<string | null>(null);

  const layout = useMemo(() => positions(sit), [sit]);

  // adjacency for focus highlighting
  const neighbors = useMemo(() => {
    const m: Record<string, Set<string>> = {};
    for (const e of sit.graph.edges) {
      (m[e.from] ||= new Set()).add(e.to);
      (m[e.to] ||= new Set()).add(e.from);
    }
    return m;
  }, [sit]);

  const isLit = (id: string) => !focus || id === focus || neighbors[focus]?.has(id);
  const edgeLit = (e: GraphEdge) => !focus || e.from === focus || e.to === focus;

  return (
    <div className="graph">
      <div className="graph-hint muted">
        Click a node to trace it. Agents <b>execute</b> steps; steps <b>precede</b> steps.
        Every edge here is a triple the reasoner holds.
      </div>
      <svg viewBox={`0 0 ${W} ${H}`} className="graph-svg" onClick={() => setFocus(null)}>
        <defs>
          <marker id="arrow" markerWidth="9" markerHeight="9" refX="7" refY="3" orient="auto">
            <path d="M0,0 L7,3 L0,6 Z" fill="var(--muted)" />
          </marker>
        </defs>

        {/* edges */}
        {sit.graph.edges.map((e, i) => {
          const a = layout[e.from], b = layout[e.to];
          if (!a || !b) return null;
          const lit = edgeLit(e);
          return (
            <line
              key={i}
              x1={a.x} y1={a.y} x2={b.x} y2={b.y}
              className={"gedge " + e.type + (lit ? " lit" : " dim")}
              markerEnd={e.type === "precedes" ? "url(#arrow)" : undefined}
            />
          );
        })}

        {/* nodes */}
        {sit.graph.nodes.map((n) => {
          const p = layout[n.id];
          if (!p) return null;
          const lit = isLit(n.id);
          return (
            <GNode key={n.id} node={n} x={p.x} y={p.y} lit={lit} focused={focus === n.id}
              onClick={(e) => { e.stopPropagation(); setFocus(focus === n.id ? null : n.id); }} />
          );
        })}
      </svg>

      {focus && <FocusPanel sit={sit} focus={focus} />}
    </div>
  );
}

interface GNodeProps {
  node: GraphNode;
  x: number;
  y: number;
  lit: boolean;
  focused: boolean;
  onClick: (e: React.MouseEvent<SVGGElement>) => void;
}

function GNode({ node, x, y, lit, focused, onClick }: GNodeProps) {
  if (node.type === "step") {
    const k = KIND[node.agentKind ?? "unknown"] || KIND.unknown;
    return (
      <g className={"gnode step " + (lit ? "lit" : "dim") + (focused ? " focused" : "")} transform={`translate(${x},${y})`} onClick={onClick}>
        <rect x={-46} y={-26} width={92} height={52} rx={9}
          className={"gnode-box k-" + node.agentKind + (node.violation ? " violation" : "") + (node.isGate ? " gate" : "")} />
        <text className="gnode-pos" y={-6}>{node.position}{node.isGate ? " 🔒" : ""}</text>
        <text className="gnode-label" y={12}>{truncate(node.label.replace(/^\d+\.\s*/, ""), 14)}</text>
      </g>
    );
  }
  // agent
  const k = KIND[node.kind ?? "unknown"] || KIND.unknown;
  return (
    <g className={"gnode agent " + (lit ? "lit" : "dim") + (focused ? " focused" : "")} transform={`translate(${x},${y})`} onClick={onClick}>
      <circle r={24} className={"gnode-circle k-" + node.kind} />
      <text className="gnode-icon" y={6}>{k.icon}</text>
      <text className="gnode-aname" y={42}>{truncate(node.label, 16)}</text>
    </g>
  );
}

interface FocusPanelProps {
  sit: Situation;
  focus: string;
}

function FocusPanel({ sit, focus }: FocusPanelProps) {
  const node = sit.graph.nodes.find((n) => n.id === focus);
  if (!node) return null;
  const out = sit.graph.edges.filter((e) => e.from === focus);
  const inc = sit.graph.edges.filter((e) => e.to === focus);
  const nm = (id: string) => sit.graph.nodes.find((n) => n.id === id)?.label || id;
  return (
    <div className="focus-panel">
      <div className="fp-title">{node.label}</div>
      {inc.map((e, i) => <div className="fp-edge" key={"i" + i}>← {nm(e.from)} <span className="fp-rel">{e.type}</span></div>)}
      {out.map((e, i) => <div className="fp-edge" key={"o" + i}><span className="fp-rel">{e.type}</span> → {nm(e.to)}</div>)}
      {/* For a step node, its agent type is reasoner-derived (role→agent→type) —
          surface it as an explainable cell that drills into the derivation. */}
      {node.type === "step" && (
        <div className="fp-edge fp-derived">
          <DagCell table="WorkflowSteps" field="ExecutingAgentType" block>
            executed by: <b>{KIND[node.agentKind ?? "unknown"].label}</b>
          </DagCell>
        </div>
      )}
    </div>
  );
}

// deterministic positions: steps spread across the bottom; each agent sits
// above the (first) step it executes, others fanned across the top.
function positions(sit: Situation): Record<string, Point> {
  const pos: Record<string, Point> = {};
  const steps = sit.steps;
  const n = steps.length;
  const pad = 90;
  const span = (W - pad * 2);
  steps.forEach((s, i) => {
    pos["step:" + s.id] = { x: pad + (n === 1 ? span / 2 : (span * i) / (n - 1)), y: STEP_Y };
  });
  // agents: place above their executed step; if multiple steps, average x.
  const agentNodes = sit.graph.nodes.filter((nd) => nd.type === "agent");
  agentNodes.forEach((a) => {
    const xs = sit.graph.edges
      .filter((e) => e.from === a.id && e.type === "executes")
      .map((e) => pos[e.to]?.x).filter((v): v is number => v != null);
    const x = xs.length ? xs.reduce((p, c) => p + c, 0) / xs.length : W / 2;
    pos[a.id] = { x: clamp(x, pad, W - pad), y: AGENT_Y };
  });
  // de-overlap agents that landed too close
  spread(agentNodes.map((a) => pos[a.id]).filter(Boolean), 64);
  return pos;
}
function spread(points: Point[], min: number) {
  points.sort((a, b) => a.x - b.x);
  for (let i = 1; i < points.length; i++) {
    if (points[i].x - points[i - 1].x < min) points[i].x = points[i - 1].x + min;
  }
}
const clamp = (v: number, lo: number, hi: number) => Math.max(lo, Math.min(hi, v));
const truncate = (s: string, n: number) => (s.length > n ? s.slice(0, n - 1) + "…" : s);
