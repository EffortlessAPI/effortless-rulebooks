import React, { useEffect, useState, useCallback, useRef } from "react";
import { useNavigate, useParams, Link } from "react-router-dom";
import { api } from "./api";
import { buildSituation, KIND, kindOfType, initials } from "./model";
import { FlowView } from "./views/FlowView";
import { GraphView } from "./views/GraphView";
import { ClosureView } from "./views/ClosureView";
import { CQView } from "./views/CQView";
import { ReassignPopover } from "./Editable";
import { StalenessBar } from "./console/StalenessBar";
import { resolveCq } from "./console/cqs";
import { DagCell } from "./explainer-dag";
import type { Story, Situation, Handlers, Scenario } from "./types";

// ===========================================================================
// Talisman's Special Solutions — Release Console.
//
// A dashboard for ONE release. The reasoned model is shown three ways
// (Flow / Graph / Closure) on the LEFT — switch the lens via the URL
// (/console/:view), the object is the same. The competency-question scoreboard
// rides on the RIGHT, always visible, so the leadership questions re-answer
// themselves live as you edit facts on the left. Everything with a dotted
// outline is editable IN PLACE. EVERY edit writes a raw fact and the OWL/SHACL
// (or Postgres) substrate recomputes the whole board — the app computes nothing
// itself.
//
// (Split out of the original 821-line App.jsx: the staleness bar and drag hook
// now live in console/ and hooks/.)
// ===========================================================================

export type ViewId = "flow" | "graph" | "closure";

const VIEWS: { id: ViewId; label: string; hint: string }[] = [
  { id: "flow", label: "Flow", hint: "the release, step by step" },
  { id: "graph", label: "Graph", hint: "the reasoned network" },
  { id: "closure", label: "Closure", hint: "assert order · watch it infer" },
];

interface ReassignState {
  roleId: string;
  anchorRect: DOMRect;
  requiresHuman: boolean;
}

interface AppProps {
  headerRight?: React.ReactNode;
}

export default function App({ headerRight = null }: AppProps) {
  // The active lens comes from the route (/console/:view); default to flow.
  const { view: viewParam } = useParams();
  const view: ViewId = (VIEWS.some((v) => v.id === viewParam) ? viewParam : "flow") as ViewId;
  const navigate = useNavigate();

  const [story, setStory] = useState<Story | null>(null);
  const [scenarios, setScenarios] = useState<Scenario[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);
  const [reassign, setReassign] = useState<ReassignState | null>(null);
  const [scenarioOpen, setScenarioOpen] = useState(false);
  // CQ ids that just changed answer as the result of a "Simulate" click — drives
  // the chip-flash so the user SEES which questions a scenario moved (one for an
  // isolated scenario, two for cq-2's ai-release-manager which also ripples cq-3).
  const [flashed, setFlashed] = useState<Set<string>>(new Set());
  const reloadingRef = useRef(false);

  const load = useCallback(async () => {
    try {
      const [s, sc] = await Promise.all([api.story(), api.scenarios().catch(() => [] as Scenario[])]);
      setStory(s);
      setScenarios(sc);
      setError(null);
    } catch (e) {
      setError((e as Error).message);
    }
  }, []);
  useEffect(() => { load(); }, [load]);

  // Every mutation: run the write (which re-fires the substrate server-side),
  // then reload the recomputed story. A short busy flag drives the "reasoning…"
  // pulse so you SEE the substrate working.
  const mutate = useCallback(async (writeFn: () => Promise<unknown>) => {
    if (reloadingRef.current) return;
    reloadingRef.current = true;
    setBusy(true);
    try {
      await writeFn();
      await load();
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setBusy(false);
      reloadingRef.current = false;
    }
  }, [load]);

  if (error) return <BootError error={error} onRetry={load} />;
  if (!story) return <div className="loading">Reasoning over the workflow…</div>;

  const sit: Situation = buildSituation(story);

  // Editing handlers passed down to every view (so all lenses edit the same way).
  const handlers: Handlers = {
    openReassign: (roleId, anchorRect, requiresHuman = false) =>
      setReassign({ roleId, anchorRect, requiresHuman: !!requiresHuman }),
    patchStep: (stepId, patch) => mutate(() => api.patchRow("WorkflowSteps", stepId, patch)),
    setStalenessThreshold: (months) =>
      mutate(() => api.patchRow("Workflows", sit.workflow.id, { stalenessThresholdMonths: months })),
    setModified: (isoDate) =>
      mutate(() => api.patchRow("Workflows", sit.workflow.id, { modified: isoDate })),
    addEdge: (from, to) => {
      const id = `prec-${sit.short(from).replace("step-", "")}-${sit.short(to).replace("step-", "")}`;
      mutate(() => api.addRow("StepPrecedence", { stepPrecedenceId: id, fromStep: from, toStep: to }));
    },
    removeEdge: (edgeId) => mutate(() => api.deleteRow("StepPrecedence", edgeId)),
  };

  const doReassign = (roleId: string, agentId: string) => {
    const agent = sit.agentById[agentId];
    if (reassign?.requiresHuman && agent?.kind !== "human") {
      const ok = window.confirm(
        `“${sit.roleById[roleId]?.name || "This role"}” fills a step that requires a human sign-off.\n\n` +
        `Assigning ${agent?.name || "a non-human"} (${agent?.kind || "?"}) will break the ` +
        `consistency rule — the reasoner will flag the step ⚠ rule broken.\n\nAssign anyway?`
      );
      if (!ok) return;
    }
    const arms: Record<string, string> = { filledByHumanAgent: "", filledByAIAgent: "", filledByAutomatedPipeline: "" };
    if (agent?.kind === "human") arms.filledByHumanAgent = agentId;
    else if (agent?.kind === "ai") arms.filledByAIAgent = agentId;
    else if (agent?.kind === "pipeline") arms.filledByAutomatedPipeline = agentId;
    setReassign(null);
    mutate(() => api.patchRow("Roles", roleId, arms));
  };

  // "Simulate" on a CQ card: snapshot every CQ's live answer, apply the card's
  // SimulateScenario, reload, then diff — flashing every CQ whose answer moved.
  // Isolated scenarios flash exactly one chip; cq-2's ai-release-manager flashes
  // two (the gate approver is itself a step executor, so cq-2 and cq-3 are
  // structurally coupled — the model won't let you move one without the other).
  const simulate = async (scenarioId: string) => {
    if (!scenarioId || reloadingRef.current) return;
    reloadingRef.current = true;
    setBusy(true);
    setFlashed(new Set());
    try {
      const before = new Map(sit.competencyQuestions.map((c) => [c.id, resolveCq(sit, c).answer]));
      await api.applyScenario(scenarioId);
      const s = await api.story();
      setStory(s);
      setError(null);
      const after = buildSituation(s);
      const moved = after.competencyQuestions
        .filter((c) => before.get(c.id) !== resolveCq(after, c).answer)
        .map((c) => c.id);
      setFlashed(new Set(moved));
      window.setTimeout(() => setFlashed(new Set()), 2800);
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setBusy(false);
      reloadingRef.current = false;
    }
  };

  return (
    <div className={"console" + (busy ? " reasoning" : "")}>
      <TopBar sit={sit} busy={busy} headerRight={headerRight} />

      <div className="viewbar">
        <div className="tabs">
          {VIEWS.map((v) => (
            <button
              key={v.id}
              className={"tab " + (view === v.id ? "on" : "")}
              onClick={() => navigate(`/console/${v.id}`)}
            >
              {v.label}
              <span className="tab-hint">{v.hint}</span>
            </button>
          ))}
        </div>
      </div>

      {/* The lens sits on the LEFT; the competency-question scoreboard pins to
          its RIGHT — always visible, so every edit on the left re-answers the
          leadership questions in place instead of on a separate tab. The CQ
          panel also hosts the explainer toggle and the scenario picker (the
          controls that used to live in the old verdict box). */}
      <div className="stage-row">
        <div className="stage">
          {/* The AgentMix (AI/human step counts) sits directly above the cards
              it summarizes, in the Flow lens only. */}
          {view === "flow" && (
            <>
              <AgentMix sit={sit} />
              <FlowView sit={sit} handlers={handlers} />
            </>
          )}
          {view === "graph" && <GraphView sit={sit} handlers={handlers} />}
          {view === "closure" && <ClosureView sit={sit} handlers={handlers} />}

          {/* The docs-review-age slider sits at the bottom of the board column,
              right under the step/role cards (it edits Modified → drives CQ5's
              IsStale, which re-answers live in the rail). */}
          <StalenessBar
            workflow={sit.workflow}
            busy={busy}
            onSetModified={handlers.setModified}
          />
        </div>
        <CQView
          sit={sit}
          hasScenarios={scenarios.length > 0}
          busy={busy}
          flashed={flashed}
          onSimulate={simulate}
          onOpenScenarios={() => setScenarioOpen(true)}
        />
      </div>

      {scenarioOpen && (
        <ScenarioModal
          scenarios={scenarios}
          busy={busy}
          onApply={(name) => { setScenarioOpen(false); mutate(() => api.applyScenario(name)); }}
          onClose={() => setScenarioOpen(false)}
        />
      )}

      {busy && (
        <div className="reasoning-veil" aria-hidden="true">
          <div className="rv-badge">✦ reasoning…</div>
        </div>
      )}

      {reassign && (
        <ReassignPopover
          role={sit.roleById[reassign.roleId]}
          agents={sit.options.agents}
          anchorRect={reassign.anchorRect}
          requiresHuman={reassign.requiresHuman}
          onPick={(agentId) => doReassign(reassign.roleId, agentId)}
          onClose={() => setReassign(null)}
        />
      )}

      <footer className="console-foot">
        <span className="dot" /> db.json holds the raw facts · OWL + SHACL is the computation ·
        this console only edits facts and renders what the reasoner returns ({sit.reasoned_triples?.toLocaleString?.()} triples)
      </footer>
    </div>
  );
}

// ---- top bar --------------------------------------------------------------
function TopBar({ sit, busy, headerRight }: { sit: Situation; busy: boolean; headerRight: React.ReactNode }) {
  return (
    <div className={"topbar " + (busy ? "busy" : "")}>
      <div className="brand">
        <span className="brand-mark">◈</span> {sit.company}
        <span className="brand-sub">Release Console</span>
      </div>
      {headerRight}
    </div>
  );
}

// ---- agent mix ------------------------------------------------------------
function AgentMix({ sit }: { sit: Situation }) {
  const ai = sit.workflow.countAISteps ?? 0;
  const human = sit.workflow.countHumanSteps ?? 0;
  // HasAIAgentStep is the explainable boolean the AI-step count drives; its DAG
  // shows CountAISteps as an input.
  // The human count is the derived CountHumanSteps rollup — no boolean of its
  // own, but the aggregation has a DAG worth drilling into, so wrap it too.
  return (
    <div className="agentmix">
      <span className="am-item am-ai">
        <DagCell table="Workflows" field="HasAIAgentStep">
          🤖 <b>{ai}</b> AI {ai === 1 ? "step" : "steps"}
        </DagCell>
      </span>
      <span className="am-item am-human">
        <DagCell table="Workflows" field="CountHumanSteps">
          🧑 <b>{human}</b> human {human === 1 ? "step" : "steps"}
        </DagCell>
      </span>
    </div>
  );
}

// ---- scenario picker (floating popup) -------------------------------------
function ScenarioModal({
  scenarios, onApply, onClose, busy,
}: {
  scenarios: Scenario[];
  onApply: (id: string) => void;
  onClose: () => void;
  busy: boolean;
}) {
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === "Escape") onClose(); };
    document.addEventListener("keydown", onKey);
    return () => document.removeEventListener("keydown", onKey);
  }, [onClose]);

  return (
    <div className="scenario-modal-backdrop" onClick={onClose}>
      <div className="scenario-modal" role="dialog" aria-modal="true" onClick={(e) => e.stopPropagation()}>
        <div className="sm-head">
          <div className="sm-title">Try a scenario</div>
          <button className="sm-close" onClick={onClose} aria-label="close">✕</button>
        </div>
        <p className="sm-intro muted">
          Each scenario sets several raw facts at once — then the reasoner recomputes the whole
          board. Pick one and watch the competency answers on the right react.
        </p>
        <div className="sm-list">
          {scenarios.map((s) => (
            <button
              key={s.id}
              className={"sm-item " + (s.isReset ? "reset" : "")}
              disabled={busy}
              onClick={() => onApply(s.id)}
            >
              <span className="sm-item-icon">{s.icon || "▷"}</span>
              <span className="sm-item-body">
                <span className="sm-item-label">{s.label}</span>
                {s.explanation && <span className="sm-item-explain">{s.explanation}</span>}
              </span>
            </button>
          ))}
        </div>
      </div>
    </div>
  );
}

function BootError({ error, onRetry }: { error: string; onRetry: () => void }) {
  return (
    <div className="boot-error">
      <h2>Couldn't reach the reasoner</h2>
      <pre>{error}</pre>
      <p className="muted">
        Is the backend up, and has <code>effortless build</code> generated <code>owl/src/*.ttl</code>?
        The API surfaces the reasoner's error verbatim — no silent fallback. Try <code>./start.sh</code>.{" "}
        <Link to="/">back to the sync station</Link>
      </p>
      <button className="btn" onClick={onRetry}>retry</button>
    </div>
  );
}

export { KIND, kindOfType, initials };
