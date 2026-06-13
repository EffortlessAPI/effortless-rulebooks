// ===========================================================================
// Shared frontend types — the shapes the Express API returns (server.js) and
// the normalized "situation" model.js derives from them.
//
// These mirror server.js exactly. The frontend NEVER computes a derived field;
// these types describe what the chosen engine already computed and the API
// arranged. Keep them in sync with /api/story, /api/triangle, /api/diff,
// /api/conformance. (Doctrine: the view IS the contract — we read it.)
// ===========================================================================

export type AgentKind = "human" | "ai" | "pipeline" | "unknown";
export type StoreId = "rulebook" | "reasoner" | "postgres";
export type ExecutingAgentType =
  | "HumanAgent"
  | "AIAgent"
  | "AutomatedPipeline"
  | string;

export interface Agent {
  id: string;
  name: string;
  kind?: AgentKind;
}

export interface TeamMember {
  id: string;
  name: string;
}

export interface Team {
  humans: TeamMember[];
  ais: TeamMember[];
  pipelines: TeamMember[];
}

// One step as arranged by /api/story (already-derived fields, narrative order).
export interface Step {
  id: string;
  position: number;
  title: string;
  roleId: string;
  requiresHumanApproval: boolean;
  executingAgentType: ExecutingAgentType;
  role: string;
  agent: Agent | null;
  durationMinutes: number;
  isApprovalGate: boolean;
  gateName: string | null;
  // Gate-only properties (null on non-gate steps). Resolved to labels server-side.
  escalationThresholdHours: number | null;
  gateRole: string | null;
  gateApproverHuman: string | null;
  consistencyViolation: boolean;
  precedes: string[];
}

// The raw step facts the inline editor needs (separate from narrative `steps`).
export interface StepFact {
  id: string;
  title: string;
  position: number;
  assignedRole: string;
  requiresHumanApproval: boolean;
  stepDurationMinutes: number;
  consumesDataset: string | null;
}

export interface RoleRow {
  id: string;
  name: string;
  fillerType: string;
  filledByHumanAgent: string | null;
  filledByAIAgent: string | null;
  filledByAutomatedPipeline: string | null;
}

export interface PrecedenceEdge {
  id: string;
  from: string;
  to: string;
}

export interface Workflow {
  id: string;
  title: string;
  status: string;
  countAISteps: number;
  countHumanSteps: number;
  totalPlanMinutes: number;
  maxPlanMinutes: number;
  isOverTimeBudget: boolean;
  modified: string;
  monthsSinceModified: number;
  stalenessThresholdMonths: number;
  isStale: boolean;
}

export interface Verdict {
  workflowTitle: string;
  monthsSinceReview: number;
  isStale: boolean;
  aiStepCount: number;
  hasAIExecutedStep: boolean;
  totalPlanMinutes: number;
  timeBudgetMinutes: number;
  isOverTimeBudget: boolean;
  hasConsistencyViolation: boolean;
  consistencyViolationCount: number;
  isAtComplianceRisk: boolean;
  statement: string;
}

export interface DelegationEntry {
  from: string;
  to: { id: string; name: string }[];
}

// One ordered pair in the transitive closure (reasoner / vw_*_closure).
export interface ClosurePair {
  from_id: string;
  to_id: string;
  is_inferred: boolean;
  hop_distance?: number;
}

export interface Closure {
  pairs: ClosurePair[];
  asserted: number;
  inferred: number;
  count: number;
}

export interface StoryOptions {
  agents: Agent[];
  roles: { id: string; name: string }[];
  steps: { id: string; title: string; position: number }[];
  datasets: { id: string; name: string }[];
}

// The full /api/story payload.
export interface Story {
  company: string;
  workflow: Workflow;
  team: Team;
  steps: Step[];
  stepFacts: StepFact[];
  roles: RoleRow[];
  edges: PrecedenceEdge[];
  options: StoryOptions;
  delegation: Record<string, DelegationEntry>;
  closure: Closure;
  verdict: Verdict;
  engine: string;
  reasoned_triples: number;
}

// ---- the normalized situation (model.ts) ----------------------------------
export interface GraphNode {
  id: string;
  type: "step" | "agent";
  label: string;
  position?: number;
  isGate?: boolean;
  violation?: boolean;
  agentKind?: AgentKind;
  kind?: AgentKind;
}

export interface GraphEdge {
  from: string;
  to: string;
  type: "precedes" | "executes";
}

export interface OrgNode {
  id: string;
  name: string;
}

export interface Situation {
  company: string;
  workflow: Workflow;
  verdict: Verdict;
  closure: Closure;
  steps: Step[];
  stepFacts: Record<string, StepFact>;
  roles: RoleRow[];
  roleById: Record<string, RoleRow>;
  agentById: Record<string, Agent>;
  team: Team;
  options: StoryOptions;
  edges: PrecedenceEdge[];
  stepsByAgent: Record<string, number[]>;
  orgTree: OrgNode[][];
  graph: { nodes: GraphNode[]; edges: GraphEdge[] };
  reasoned_triples: number;
  short: (id: string) => string;
}

// ---- the editing handlers passed down to every view -----------------------
export interface Handlers {
  openReassign: (roleId: string, anchorRect: DOMRect, requiresHuman?: boolean) => void;
  patchStep: (stepId: string, patch: Record<string, unknown>) => void;
  setStalenessThreshold: (months: number) => void;
  setModified: (isoDate: string) => void;
  addEdge: (from: string, to: string) => void;
  removeEdge: (edgeId: string) => void;
}

// ---- triangle / diff / conformance (login + sync station) ------------------
export interface TriangleLeg {
  inSyncWithReasoner?: boolean;
  inSyncWithPostgres?: boolean;
  inSyncWithRulebook?: boolean;
  lastEditAt: string | null;
}

export interface TriangleResponse {
  aheadOf: StoreId | "diverged" | null;
  action: string | null;
  hashes: Record<StoreId, string>;
  legs: Record<StoreId, TriangleLeg>;
}

export interface DiffField {
  field: string;
  other: unknown;
  head: unknown;
}

export interface DiffRow {
  pk: string;
  kind: "changed" | "added" | "removed";
  fields: DiffField[];
}

export interface DiffTable {
  table: string;
  rows: DiffRow[];
}

export interface DiffAgainst {
  tables: DiffTable[];
  changedFields: number;
  changedRows: number;
}

export interface DiffResponse {
  head: StoreId;
  files: Record<StoreId, string>;
  against: Partial<Record<StoreId, DiffAgainst>>;
  hashes: Record<StoreId, string>;
}

export interface ConformanceDiff {
  kind: "value" | "representation" | "presence";
  class: string;
  pk: string;
  field: string;
  ftype: string;
  reasoner: unknown;
  postgres: unknown;
}

export interface ConformanceResponse {
  summary: {
    agree: boolean;
    valueDiffs: number;
    representationDiffs: number;
    presenceDiffs: number;
    identical: number;
    fieldsCompared: number;
  };
  diffs: ConformanceDiff[];
}

export interface BackendDescriptor {
  id: string;
  label: string;
}

export interface Scenario {
  id: string;
  label: string;
  icon: string;
  explanation: string;
  isReset: boolean;
}

// One SSE event streamed by a control action (Reset / Rebuild / Sync) — see
// runControl in api.ts and control.ts on the server.
export interface ControlEvent {
  type: string;
  label?: string;
  line?: string;
  action?: string;
  error?: string;
}
