// ===========================================================================
// The "situation" — one normalized view of the reasoned world that all three
// dashboard projections (Flow, Org, Graph) derive from. Same mathematical
// object, three lenses. We compute the shared structures ONCE here so the views
// stay thin and can't drift from each other.
//
// IMPORTANT: this file derives PRESENTATION structure (indices, an org tree,
// graph node/edge lists) from data the reasoner already computed. It never
// computes a business fact (an agent's type, a step's violation, the verdict) —
// those arrive pre-derived in the story payload. (Same doctrine as the backend:
// the reasoner is the source of truth; we arrange.)
// ===========================================================================

export const KIND = {
  human: { icon: "🧑", label: "Human", color: "var(--human)", cls: "k-human" },
  ai: { icon: "🤖", label: "AI", color: "var(--ai)", cls: "k-ai" },
  pipeline: { icon: "⚙️", label: "Pipeline", color: "var(--pipeline)", cls: "k-pipeline" },
  unknown: { icon: "•", label: "—", color: "var(--muted)", cls: "" },
};

export function kindOfType(t) {
  return t === "AIAgent" ? "ai"
    : t === "AutomatedPipeline" ? "pipeline"
    : t === "HumanAgent" ? "human"
    : "unknown";
}

export function initials(name) {
  if (!name) return "?";
  return name.split(/[\s-]+/).filter(Boolean).slice(0, 2).map((w) => w[0].toUpperCase()).join("");
}

const shortId = (id) => (id || "").replace(/^prod-deploy-/, "").replace(/^ntwf-/, "");

// Build the normalized situation from the raw /api/story payload.
export function buildSituation(story) {
  const agentById = {};
  for (const a of story.options.agents) agentById[a.id] = a;

  const roleById = {};
  for (const r of story.roles) roleById[r.id] = r;

  // step -> the role that owns it, and the agent that fills that role (already
  // resolved in story.steps). Also a reverse index: agent -> steps they touch.
  const stepsByAgent = {};
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
  const nodes = [];
  const edges = [];
  const addNode = (id, type, label, extra = {}) => nodes.push({ id, type, label, ...extra });

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

  return {
    company: story.company,
    workflow: story.workflow,
    verdict: story.verdict,
    closure: story.closure,
    steps: story.steps,
    stepFacts: Object.fromEntries(story.stepFacts.map((f) => [f.id, f])),
    roles: story.roles,
    roleById,
    agentById,
    team: story.team,
    options: story.options,
    edges: directEdges,
    stepsByAgent,
    orgTree,
    graph: { nodes, edges },
    reasoned_triples: story.reasoned_triples,
    short: shortId,
  };
}

// Recover a delegation tree (root -> ... -> leaf) from the transitive reach map.
// For each role we keep only its CLOSEST target (the one with the largest
// onward reach is the immediate next hop in a linear chain).
function buildOrgTree(delegation, roleById) {
  const nextHop = {}; // roleId -> immediate next roleId
  for (const [rid, info] of Object.entries(delegation || {})) {
    const targets = info.to || [];
    if (!targets.length) continue;
    // immediate next = the target that itself reaches the most others
    const reach = (id) => (delegation[id]?.to?.length || 0);
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
