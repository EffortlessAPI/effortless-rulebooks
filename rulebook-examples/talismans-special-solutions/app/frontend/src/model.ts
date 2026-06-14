// ===========================================================================
// The "situation" — one normalized view of the reasoned world that all three
// dashboard projections (Flow, Org, Graph) derive from. Same mathematical
// object, three lenses. We compute the shared structures ONCE here so the views
// stay thin and can't drift from each other.
//
// IMPORTANT: this file derives PRESENTATION structure (indices, an org tree,
// graph node/edge lists) from data the reasoner already computed. It never
// computes a business fact (an agent's type, a step's violation, staleness) —
// those arrive pre-derived in the story payload. (Same doctrine as the backend:
// the reasoner is the source of truth; we arrange.)
// ===========================================================================

import type {
  Agent,
  AgentKind,
  DelegationEntry,
  GraphEdge,
  GraphNode,
  OrgNode,
  RoleRow,
  Situation,
  Story,
} from "./types";

interface KindMeta {
  icon: string;
  label: string;
  color: string;
  cls: string;
}

export const KIND: Record<AgentKind, KindMeta> = {
  human: { icon: "🧑", label: "Human", color: "var(--human)", cls: "k-human" },
  ai: { icon: "🤖", label: "AI", color: "var(--ai)", cls: "k-ai" },
  pipeline: { icon: "⚙️", label: "Pipeline", color: "var(--pipeline)", cls: "k-pipeline" },
  unknown: { icon: "•", label: "—", color: "var(--muted)", cls: "" },
};

export function kindOfType(t: string): AgentKind {
  return t === "AIAgent" ? "ai"
    : t === "AutomatedPipeline" ? "pipeline"
    : t === "HumanAgent" ? "human"
    : "unknown";
}

export function initials(name: string): string {
  if (!name) return "?";
  return name.split(/[\s-]+/).filter(Boolean).slice(0, 2).map((w) => w[0].toUpperCase()).join("");
}

// ---- per-agent avatar -----------------------------------------------------
// Every agent gets its OWN face, so Maria reads differently from James at a
// glance — the role is clear from the person filling it, not just a generic 🧑.
// Deterministic from the agent id (offline, stable across reloads, no service):
// a distinct face from a kind-appropriate palette plus a distinct ring hue. Two
// people can share a face only if their hues also collide, which the 0–359 hue
// spread makes vanishingly unlikely for a demo-sized cast.
function hashStr(s: string): number {
  let h = 2166136261;
  for (let i = 0; i < s.length; i++) {
    h ^= s.charCodeAt(i);
    h = Math.imul(h, 16777619);
  }
  return h >>> 0;
}
const HUMAN_FACES = ["👩‍💼", "👨‍💼", "👩‍🔬", "👨‍💻", "👩‍⚖️", "🧑‍🚀", "👨‍🏫", "👩‍🔧", "🧔", "👩‍🦰", "👨‍🦱", "👩‍🦳"];
const AI_FACES = ["🤖", "👾", "🦾"];
const PIPELINE_FACES = ["⚙️", "🛠️", "🔁"];

export interface AvatarLook {
  face: string;
  hue: number;
}

export function avatarFor(agent?: Agent | null, type?: string): AvatarLook | null {
  if (!agent) return null;
  const kind: AgentKind = agent.kind || (type ? kindOfType(type) : "unknown");
  const pool = kind === "ai" ? AI_FACES : kind === "pipeline" ? PIPELINE_FACES : kind === "human" ? HUMAN_FACES : null;
  if (!pool) return null;
  const h = hashStr(agent.id || agent.name || "?");
  return { face: pool[h % pool.length], hue: h % 360 };
}

// The escalation ladder ABOVE a role (its VP, then CTO, …), read straight off
// the reasoned org tree the situation already built. Empty when the role
// escalates to no one. Lets a card say "↑ VP of Engineering → CTO" so the chain
// of authority around each person is always legible.
export function escalationAncestors(sit: Situation, roleId: string): { id: string; name: string }[] {
  for (const chain of sit.orgTree) {
    const idx = chain.findIndex((n) => n.id === roleId);
    if (idx >= 0) return chain.slice(idx + 1);
  }
  return [];
}

const shortId = (id: string): string => (id || "").replace(/^prod-deploy-/, "").replace(/^ntwf-/, "");

// Build the normalized situation from the raw /api/story payload.
export function buildSituation(story: Story): Situation {
  const agentById: Record<string, Agent> = {};
  for (const a of story.options.agents) agentById[a.id] = a;

  const roleById: Record<string, RoleRow> = {};
  for (const r of story.roles) roleById[r.id] = r;

  // step -> the role that owns it, and the agent that fills that role (already
  // resolved in story.steps). Also a reverse index: agent -> steps they touch.
  const stepsByAgent: Record<string, number[]> = {};
  for (const s of story.steps) {
    if (s.agent) (stepsByAgent[s.agent.id] ||= []).push(s.position);
  }

  // Direct (asserted) precedence edges, normalized from raw rows.
  const directEdges = story.edges.map((e) => ({ from: e.from, to: e.to, id: e.id }));

  // The org / delegation tree: roots are roles nobody delegates *to*; children
  // are the single next hop. story.delegation gives, per role, its full
  // transitive reach; we recover the DIRECT next hop (the closest target).
  const orgTree = buildOrgTree(story.delegation, roleById);

  // ---- graph projection: typed nodes + edges -----------------------------
  const nodes: GraphNode[] = [];
  const edges: GraphEdge[] = [];
  const addNode = (
    id: string,
    type: GraphNode["type"],
    label: string,
    extra: Partial<GraphNode> = {},
  ): number => nodes.push({ id, type, label, ...extra });

  // step nodes
  for (const s of story.steps) {
    addNode("step:" + s.id, "step", `${s.position}. ${s.title}`, {
      position: s.position,
      isGate: s.isApprovalGate,
      violation: s.consistencyViolation,
      agentKind: kindOfType(s.executingAgentType),
    });
  }
  // agent nodes (only those that fill a role used by a step, to keep it legible)
  const usedAgents = new Set(story.steps.map((s) => s.agent?.id).filter(Boolean));
  for (const a of story.options.agents) {
    if (usedAgents.has(a.id)) addNode("agent:" + a.id, "agent", a.name, { kind: a.kind });
  }
  // precedence edges (asserted only, in the graph — closure is shown elsewhere)
  for (const e of directEdges) {
    edges.push({ from: "step:" + e.from, to: "step:" + e.to, type: "precedes" });
  }
  // assignment edges: step -> its filling agent
  for (const s of story.steps) {
    if (s.agent) edges.push({ from: "agent:" + s.agent.id, to: "step:" + s.id, type: "executes" });
  }
  // dataset nodes + consumption edges (CQ8). A DCAT dataset is a first-class node
  // on the board so the question "what datasets does the review consume, and
  // which AI processed them?" is VISIBLE: the dataset feeds the step, the step's
  // agent (an AI) is one hop up. A dataset consumed by NO step renders as an
  // orphan — which is exactly what the "detach the risk dataset" simulate leaves
  // behind: you watch the edge break, not a string change in a card.
  for (const d of story.datasets) {
    addNode("dataset:" + d.id, "dataset", d.title, { orphan: d.consumedBySteps.length === 0 });
    for (const c of d.consumedBySteps) {
      edges.push({ from: "dataset:" + d.id, to: "step:" + c.id, type: "consumes" });
    }
  }

  return {
    company: story.company,
    workflow: story.workflow,
    closure: story.closure,
    derivationClosure: story.derivationClosure ?? null,
    steps: story.steps,
    stepFacts: Object.fromEntries(story.stepFacts.map((f) => [f.id, f])),
    roles: story.roles,
    departments: story.departments,
    departmentById: Object.fromEntries((story.departments || []).map((d) => [d.id, d])),
    roleById,
    agentById,
    team: story.team,
    options: story.options,
    edges: directEdges,
    stepsByAgent,
    orgTree,
    graph: { nodes, edges },
    delegation: story.delegation,
    artifacts: story.artifacts,
    datasets: story.datasets,
    competencyQuestions: story.competencyQuestions,
    reasoned_triples: story.reasoned_triples,
    short: shortId,
  };
}

// Recover a delegation tree (root -> ... -> leaf) from the transitive reach map.
// For each role we keep only its CLOSEST target (the one with the largest
// onward reach is the immediate next hop in a linear chain).
function buildOrgTree(
  delegation: Record<string, DelegationEntry>,
  roleById: Record<string, RoleRow>,
): OrgNode[][] {
  const nextHop: Record<string, string> = {}; // roleId -> immediate next roleId
  for (const [rid, info] of Object.entries(delegation || {})) {
    const targets = info.to || [];
    if (!targets.length) continue;
    // immediate next = the target that itself reaches the most others
    const reach = (id: string): number => (delegation[id]?.to?.length || 0);
    const sorted = [...targets].sort((a, b) => reach(b.id) - reach(a.id));
    nextHop[rid] = sorted[0].id;
  }
  // roots = roles that delegate but are not delegated-to by anyone in nextHop
  const targeted = new Set(Object.values(nextHop));
  const roots = Object.keys(nextHop).filter((r) => !targeted.has(r));
  // Build linear chains from each root.
  const chains = roots.map((root) => {
    const chain = [root];
    let cur = root;
    const seen = new Set([root]);
    while (nextHop[cur] && !seen.has(nextHop[cur])) {
      cur = nextHop[cur];
      seen.add(cur);
      chain.push(cur);
    }
    return chain.map((id) => ({ id, name: roleById[id]?.name || id }));
  });
  return chains;
}
