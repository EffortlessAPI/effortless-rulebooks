import React, { useMemo, useState } from "react";
import { KIND, avatarFor } from "../model";
import { DagCell } from "../explainer-dag";
import type { Situation, Handlers, GraphNode, GraphEdge } from "../types";

// Department hue, matching the Dept lens (teal = Engineering, rose = Legal).
function deptHue(key: string | null | undefined): string {
  const s = key || "";
  if (/legal/i.test(s)) return "#f0719b";
  if (/eng/i.test(s)) return "#3fb6a8";
  return "var(--muted)";
}
const agentIdOf = (nodeId: string) => nodeId.replace(/^agent:/, "");
const stepIdOf = (nodeId: string) => nodeId.replace(/^step:/, "");

// ===========================================================================
// GRAPH VIEW — the reasoned network itself, made legible.
//
// Nodes: steps (bottom row, in order) and the agents that execute them (top).
// Edges: agent —executes→ step (assignment) and step —precedes→ step (order).
// Click any node to focus it: its direct connections light up, the rest dim,
// so you can SEE "this step is run by that agent, and precedes these steps."
// Deterministic layout (no physics lib) — stable, readable, no jitter.
// ===========================================================================

const W = 860, H = 500;
const STEP_Y = 300, AGENT_Y = 80, DATASET_Y = 440;

type Point = { x: number; y: number };

interface GraphViewProps {
  sit: Situation;
  handlers: Handlers;
}

export function GraphView({ sit, handlers }: GraphViewProps) {
  const [focus, setFocus] = useState<string | null>(null);
  // Hover surfaces the role/department details on the main graph without a
  // click: move over an agent or step and its card follows the cursor.
  const [hover, setHover] = useState<{ id: string; x: number; y: number } | null>(null);

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
        <b>Hover</b> any node for who/role/department · <b>click</b> to trace it. Agents
        <b> execute</b> steps; steps <b>precede</b> steps; a DCAT 📦 <b>dataset</b> feeds the step
        that consumes it (CQ8). Every edge here is a triple the reasoner holds.
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
              markerEnd={e.type === "precedes" || e.type === "consumes" ? "url(#arrow)" : undefined}
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
              onClick={(e) => { e.stopPropagation(); setFocus(focus === n.id ? null : n.id); }}
              onEnter={(e) => setHover({ id: n.id, x: e.clientX, y: e.clientY })}
              onMove={(e) => setHover((h) => (h && h.id === n.id ? { ...h, x: e.clientX, y: e.clientY } : h))}
              onLeave={() => setHover((h) => (h && h.id === n.id ? null : h))} />
          );
        })}
      </svg>

      {hover && <GraphHoverCard sit={sit} nodeId={hover.id} x={hover.x} y={hover.y} />}
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
  onEnter: (e: React.MouseEvent<SVGGElement>) => void;
  onMove: (e: React.MouseEvent<SVGGElement>) => void;
  onLeave: () => void;
}

function GNode({ node, x, y, lit, focused, onClick, onEnter, onMove, onLeave }: GNodeProps) {
  const hov = { onMouseEnter: onEnter, onMouseMove: onMove, onMouseLeave: onLeave };
  if (node.type === "step") {
    return (
      <g className={"gnode step " + (lit ? "lit" : "dim") + (focused ? " focused" : "")} transform={`translate(${x},${y})`} onClick={onClick} {...hov}>
        <rect x={-46} y={-26} width={92} height={52} rx={9}
          className={"gnode-box k-" + node.agentKind + (node.violation ? " violation" : "") + (node.isGate ? " gate" : "")} />
        <text className="gnode-pos" y={-6}>{node.position}{node.isGate ? " 🔒" : ""}</text>
        <text className="gnode-label" y={12}>{truncate(node.label.replace(/^\d+\.\s*/, ""), 14)}</text>
      </g>
    );
  }
  if (node.type === "dataset") {
    // A DCAT dataset: a first-class node so CQ8 ("what datasets does the review
    // consume, and which AI processed them?") is visible. Orphan = consumed by
    // no step (what the detach simulate leaves behind) → dashed + dimmed.
    return (
      <g className={"gnode dataset " + (lit ? "lit" : "dim") + (focused ? " focused" : "")} transform={`translate(${x},${y})`} onClick={onClick} {...hov}>
        <rect x={-52} y={-20} width={104} height={40} rx={7}
          className={"gnode-dataset" + (node.orphan ? " orphan" : "")} />
        <text className="gnode-dataset-icon" x={-38} y={5}>📦</text>
        <text className="gnode-dataset-label" x={4} y={1}>{truncate(node.label, 13)}</text>
        <text className="gnode-dataset-sub" x={4} y={13}>{node.orphan ? "consumed by no step" : "DCAT dataset"}</text>
      </g>
    );
  }
  // agent — show the PERSON's distinct face (so the two AIs read as 🤖, the
  // pipeline as ⚙️, and each human is unmistakable), not just the kind icon.
  const k = KIND[node.kind ?? "unknown"] || KIND.unknown;
  const look = avatarFor({ id: agentIdOf(node.id), name: node.label, kind: node.kind }, undefined);
  return (
    <g className={"gnode agent " + (lit ? "lit" : "dim") + (focused ? " focused" : "")} transform={`translate(${x},${y})`} onClick={onClick} {...hov}>
      <circle r={24} className={"gnode-circle k-" + node.kind} />
      <text className="gnode-icon" y={6}>{look ? look.face : k.icon}</text>
      <text className="gnode-aname" y={42}>{truncate(node.label, 16)}</text>
    </g>
  );
}

// The hover-card: who/what this node is, the role it fills (or that owns it),
// and the OWNING DEPARTMENT — so CQ7's Eng/Legal split is legible right on the
// main graph, not only in the Dept lens. Every value is read from the situation
// the substrate already computed; nothing is derived here.
function GraphHoverCard({ sit, nodeId, x, y }: { sit: Situation; nodeId: string; x: number; y: number }) {
  const node = sit.graph.nodes.find((n) => n.id === nodeId);
  if (!node) return null;
  const style: React.CSSProperties = {
    left: Math.min(x + 16, window.innerWidth - 260),
    top: Math.min(y + 16, window.innerHeight - 180),
  };

  let body: React.ReactNode = null;
  if (node.type === "agent") {
    const aid = agentIdOf(nodeId);
    const fills = sit.roles.filter(
      (r) => r.filledByHumanAgent === aid || r.filledByAIAgent === aid || r.filledByAutomatedPipeline === aid,
    );
    const kindLabel = KIND[node.kind ?? "unknown"].label;
    body = (
      <>
        <div className="gh-sub muted">{KIND[node.kind ?? "unknown"].icon} {kindLabel} agent</div>
        {fills.length === 0 && <div className="gh-row muted">fills no role in this release</div>}
        {fills.map((r) => {
          const dept = r.ownedBy ? sit.departmentById[r.ownedBy] : null;
          const hue = deptHue(r.ownedBy);
          const owns = sit.steps.filter((s) => s.roleId === r.id).sort((a, b) => a.position - b.position);
          return (
            <div className="gh-block" key={r.id}>
              <div className="gh-row">fills <b>{r.name}</b></div>
              <div className="gh-row">
                department <span className="gh-pill" style={{ borderColor: hue, color: hue }}>🏛 {dept?.title || "—"}</span>
              </div>
              {owns.length > 0 && (
                <div className="gh-row muted">executes {owns.map((s) => `#${s.position} ${s.title}`).join(" · ")}</div>
              )}
            </div>
          );
        })}
      </>
    );
  } else if (node.type === "step") {
    const s = sit.steps.find((st) => st.id === stepIdOf(nodeId));
    if (s) {
      const dept = s.owningDepartment ? sit.departmentById[s.owningDepartment] : null;
      const hue = deptHue(s.owningDepartment);
      body = (
        <>
          <div className="gh-sub muted">
            step #{s.position}{s.isApprovalGate ? " · 🔒 approval gate" : ""}
            {s.consistencyViolation ? " · ⚠ rule broken" : ""}
          </div>
          <div className="gh-row">role <b>{s.role}</b></div>
          <div className="gh-row">
            run by {KIND[(node.agentKind ?? "unknown")].icon} <b>{s.agent ? s.agent.name : "unassigned"}</b>
          </div>
          <div className="gh-row">
            department <span className="gh-pill" style={{ borderColor: hue, color: hue }}>🏛 {dept?.title || "—"}</span>
            {s.isLegalOwned ? " · Legal-owned" : s.isEngineeringOwned ? " · Engineering-owned" : ""}
          </div>
        </>
      );
    }
  } else {
    body = <div className="gh-sub muted">{node.orphan ? "DCAT dataset · consumed by no step" : "DCAT dataset"}</div>;
  }

  return (
    <div className="graph-hovercard" style={style}>
      <div className="gh-title">{node.label}</div>
      {body}
    </div>
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
  // datasets: a stable bottom row, evenly spread, INDEPENDENT of consumption —
  // so detaching one (it loses its consume-edge) makes the edge vanish while the
  // node stays put. That's the visible cut, not a node that jumps away.
  const dsNodes = sit.graph.nodes.filter((nd) => nd.type === "dataset");
  const dn = dsNodes.length;
  dsNodes.forEach((d, i) => {
    pos[d.id] = { x: pad + (dn === 1 ? span / 2 : (span * i) / (dn - 1)), y: DATASET_Y };
  });
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
