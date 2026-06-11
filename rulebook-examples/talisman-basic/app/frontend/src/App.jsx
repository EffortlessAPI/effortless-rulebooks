import React, { useEffect, useState, useCallback, useRef } from "react";
import { api } from "./api.js";
import { buildSituation, KIND, kindOfType, initials } from "./model.js";
import { FlowView } from "./views/FlowView.jsx";
import { GraphView } from "./views/GraphView.jsx";
import { ClosureView } from "./views/ClosureView.jsx";
import { ReassignPopover } from "./Editable.jsx";

// ===========================================================================
// Talisman's Special Solutions — Release Console.
//
// A dashboard for ONE release. The top is the situation at a glance (the
// compliance verdict). The middle is the same reasoned model shown three ways
// (Flow / Org / Graph) — switch the lens, the object is the same. Everything
// with a dotted outline is editable IN PLACE: click an agent to reassign, a
// step to edit its facts, drag between steps to add ordering. Scenario buttons
// set several facts at once. EVERY edit writes a raw fact and the OWL/SHACL
// reasoner recomputes the whole board — the app computes nothing itself.
// ===========================================================================

const VIEWS = [
  { id: "flow", label: "Flow", hint: "the release, step by step" },
  { id: "graph", label: "Graph", hint: "the reasoned network" },
  { id: "closure", label: "Closure", hint: "assert order · watch it infer" },
];

export default function App({ headerRight = null }) {
  const [story, setStory] = useState(null);
  const [scenarios, setScenarios] = useState([]);
  const [error, setError] = useState(null);
  const [busy, setBusy] = useState(false);
  const [view, setView] = useState("flow");
  const [reassign, setReassign] = useState(null); // {roleId, anchorRect, requiresHuman} | null
  const [scenarioOpen, setScenarioOpen] = useState(false); // floating picker open?
  const reloadingRef = useRef(false);

  const load = useCallback(async () => {
    try {
      const [s, sc] = await Promise.all([api.story(), api.scenarios().catch(() => [])]);
      setStory(s);
      setScenarios(sc);
      setError(null);
    } catch (e) {
      setError(e.message);
    }
  }, []);
  useEffect(() => { load(); }, [load]);

  // Every mutation: run the write (which re-fires the reasoner server-side),
  // then reload the recomputed story. A short busy flag drives the "reasoning…"
  // pulse so you SEE the substrate working.
  const mutate = useCallback(async (writeFn) => {
    if (reloadingRef.current) return;
    reloadingRef.current = true;
    setBusy(true);
    try {
      await writeFn();
      await load();
    } catch (e) {
      setError(e.message);
    } finally {
      setBusy(false);
      reloadingRef.current = false;
    }
  }, [load]);

  if (error) return <BootError error={error} onRetry={load} />;
  if (!story) return <div className="loading">Reasoning over the workflow…</div>;

  const sit = buildSituation(story);

  // Editing handlers passed down to every view (so all lenses edit the same way).
  const handlers = {
    // `requiresHuman` is the step's RAW fact (requiresHumanApproval): when true,
    // the reasoner's consistency rule demands a human filler — the popover surfaces
    // that BEFORE you pick, and a non-human pick asks for confirmation.
    openReassign: (roleId, anchorRect, requiresHuman = false) =>
      setReassign({ roleId, anchorRect, requiresHuman: !!requiresHuman }),
    patchStep: (stepId, patch) => mutate(() => api.patchRow("WorkflowSteps", stepId, patch)),
    // Time budget (MaxPlanMinutes) is a FIXED fact (4h, seeded in the rulebook) —
    // no setter here; the plan fits or breaches by editing step durations.
    // Staleness knobs — both edit RAW facts; IsStale re-derives on re-reason.
    setStalenessThreshold: (months) =>
      mutate(() => api.patchRow("Workflows", sit.workflow.id, { stalenessThresholdMonths: months })),
    setModified: (isoDate) =>
      mutate(() => api.patchRow("Workflows", sit.workflow.id, { modified: isoDate })),
    addEdge: (from, to) => {
      const id = `prec-${sit.short(from).replace("step-", "")}-${sit.short(to).replace("step-", "")}`;
      return mutate(() => api.addRow("StepPrecedence", { stepPrecedenceId: id, fromStep: from, toStep: to }));
    },
    removeEdge: (edgeId) => mutate(() => api.deleteRow("StepPrecedence", edgeId)),
  };

  const doReassign = (roleId, agentId) => {
    const agent = sit.agentById[agentId];
    // If this step requires a human sign-off and you're picking a non-human,
    // ask first — the pick is still ALLOWED (the whole point is to watch the
    // reasoner flag it), but it will trip the consistency rule, so confirm intent.
    if (reassign?.requiresHuman && agent?.kind !== "human") {
      const ok = window.confirm(
        `“${sit.roleById[roleId]?.name || "This role"}” fills a step that requires a human sign-off.\n\n` +
        `Assigning ${agent?.name || "a non-human"} (${agent?.kind || "?"}) will break the ` +
        `consistency rule — the reasoner will flag the step ⚠ rule broken.\n\nAssign anyway?`
      );
      if (!ok) return; // leave the popover open so they can pick a human instead
    }
    const arms = { filledByHumanAgent: "", filledByAIAgent: "", filledByAutomatedPipeline: "" };
    if (agent?.kind === "human") arms.filledByHumanAgent = agentId;
    else if (agent?.kind === "ai") arms.filledByAIAgent = agentId;
    else if (agent?.kind === "pipeline") arms.filledByAutomatedPipeline = agentId;
    setReassign(null);
    mutate(() => api.patchRow("Roles", roleId, arms));
  };

  return (
    <div className={"console" + (busy ? " reasoning" : "")}>
      <TopBar sit={sit} busy={busy} headerRight={headerRight} />

      {/* The verdict is a fixed card in the top-right corner — always visible,
          pinned as you scroll. Right under the verdict sits the "Try a scenario"
          button, which opens the floating picker (scenarios come from the DB). */}
      <VerdictHeader
        verdict={sit.verdict}
        workflow={sit.workflow}
        hasScenarios={scenarios.length > 0}
        busy={busy}
        onOpenScenarios={() => setScenarioOpen(true)}
      />

      <div className="viewbar">
        <div className="tabs">
          {VIEWS.map((v) => (
            <button key={v.id} className={"tab " + (view === v.id ? "on" : "")} onClick={() => setView(v.id)}>
              {v.label}
              <span className="tab-hint">{v.hint}</span>
            </button>
          ))}
        </div>
      </div>

      {/* The step mix (how many steps an AI vs. a human runs) sits right above
          the step cards it summarizes. */}
      <AgentMix sit={sit} />

      <div className="stage">
        {view === "flow" && <FlowView sit={sit} handlers={handlers} />}
        {view === "graph" && <GraphView sit={sit} handlers={handlers} />}
        {view === "closure" && <ClosureView sit={sit} handlers={handlers} />}
      </div>

      {/* The two stacked status bars now live at the BOTTOM, just above the
          footer. They are read-outs of the verdict's inputs (staleness + time
          budget); their editable knobs drive what the verdict above recomputes. */}
      <StalenessBar
        workflow={sit.workflow}
        busy={busy}
        onSetModified={handlers.setModified}
      />
      <TimeBudgetBar sit={sit} workflow={sit.workflow} verdict={sit.verdict} handlers={handlers} busy={busy} />

      {/* The scenario picker is a floating popup, opened from the button under
          the verdict. Each choice sets several facts at once (and explains what
          it does); watch the whole board recompute. Scenarios are DB-driven. */}
      {scenarioOpen && (
        <ScenarioModal
          scenarios={scenarios}
          busy={busy}
          onApply={(name) => { setScenarioOpen(false); mutate(() => api.applyScenario(name)); }}
          onClose={() => setScenarioOpen(false)}
        />
      )}

      {/* While the reasoner runs, the whole board dims behind a pulsing veil so
          it's unmistakable that the substrate is recomputing (matches the
          top-right "reasoning…" pulse). */}
      {busy && (
        <div className="reasoning-veil" aria-hidden="true">
          <div className="rv-badge">✦ reasoning…</div>
        </div>
      )}

      {reassign && (
        <ReassignPopover
          roleId={reassign.roleId}
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

// ---- top bar: the brand on the left, the engine controls on the right -----
// `headerRight` is an optional slot (Root passes the <EngineBar/> into it) so the
// live-engine name, the conformance witness, and the engine switcher sit pinned
// in the top-right corner of the header rather than at the bottom of the page.
function TopBar({ sit, busy, headerRight }) {
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

// ---- agent mix: how many steps an AI vs. a human runs --------------------
// A small summary that sits directly above the step cards it counts. Both
// numbers are derived columns read from the view (countAISteps / countHumanSteps)
// — we never re-tally them here.
function AgentMix({ sit }) {
  const ai = sit.workflow.countAISteps ?? 0;
  const human = sit.workflow.countHumanSteps ?? 0;
  return (
    <div className="agentmix">
      <span className="am-item am-ai">🤖 <b>{ai}</b> AI {ai === 1 ? "step" : "steps"}</span>
      <span className="am-item am-human">🧑 <b>{human}</b> human {human === 1 ? "step" : "steps"}</span>
    </div>
  );
}

// ---- scenario picker (floating popup) -------------------------------------
// A modal launched from the button under the verdict. Each scenario sets several
// raw facts at once; the row shows its label + an explanation of what it does
// and what the reasoner will derive as a result. The whole list — labels, icons,
// explanations — comes from the backend (which reads the rulebook's `Scenarios`
// table). Nothing here is hardcoded; add a row to the rulebook and it shows up.
function ScenarioModal({ scenarios, onApply, onClose, busy }) {
  // Close on Escape; the backdrop click also closes.
  useEffect(() => {
    const onKey = (e) => { if (e.key === "Escape") onClose(); };
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
          board. Pick one and watch the verdict and the bars react.
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

// ---- the verdict, always visible ------------------------------------------
// THREE inputs decide the verdict, shown as one readable sentence:
//   (docs stale  AND  AI runs a step)   OR   (plan over its time budget)
// Each pill is lit only when that condition is currently true; the pills that
// are actually FORCING the current verdict get a "firing" highlight.
//
// Directly under the verdict sits the "Try a scenario" button that opens the
// floating picker — keeping the presets right next to the status they change.
// A tiny drag hook: grab the handle, move the card, drop it. Position is kept in
// viewport (fixed) coords and persisted to localStorage so it survives reloads.
// Returns pos=null until the user drags, so until then the card sits at its CSS
// default (top-right). Double-clicking the handle calls onReset -> back to null.
function useDraggable(storageKey) {
  const [pos, setPos] = useState(() => {
    try {
      const raw = localStorage.getItem(storageKey);
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;
    }
  });
  const [dragging, setDragging] = useState(false);
  const drag = useRef(null); // { dx, dy } pointer-offset within the card

  const onGrabHandle = useCallback((e) => {
    // ignore right-clicks; only start on the handle itself
    if (e.button !== 0) return;
    const card = e.currentTarget.closest(".verdict-header");
    if (!card) return;
    const rect = card.getBoundingClientRect();
    drag.current = { dx: e.clientX - rect.left, dy: e.clientY - rect.top };
    setDragging(true);
    e.preventDefault();
  }, []);

  useEffect(() => {
    if (!dragging) return;
    const onMove = (e) => {
      const { dx, dy } = drag.current || { dx: 0, dy: 0 };
      // clamp so the card can't be dragged fully off-screen (keep 40px in view)
      const maxLeft = window.innerWidth - 60;
      const maxTop = window.innerHeight - 40;
      const left = Math.max(0, Math.min(e.clientX - dx, maxLeft));
      const top = Math.max(0, Math.min(e.clientY - dy, maxTop));
      setPos({ top, left });
    };
    const onUp = () => {
      setDragging(false);
      setPos((p) => {
        try {
          if (p) localStorage.setItem(storageKey, JSON.stringify(p));
        } catch {}
        return p;
      });
    };
    window.addEventListener("mousemove", onMove);
    window.addEventListener("mouseup", onUp);
    return () => {
      window.removeEventListener("mousemove", onMove);
      window.removeEventListener("mouseup", onUp);
    };
  }, [dragging, storageKey]);

  const onReset = useCallback(() => {
    setPos(null);
    try {
      localStorage.removeItem(storageKey);
    } catch {}
  }, [storageKey]);

  return { pos, dragging, onGrabHandle, onReset };
}

function VerdictHeader({ verdict, workflow, hasScenarios, busy, onOpenScenarios }) {
  const risk = verdict.isAtComplianceRisk;
  const stale = verdict.isStale;
  const ai = verdict.hasAIExecutedStep;
  const over = verdict.isOverTimeBudget;
  const broken = verdict.hasConsistencyViolation;
  const staleAndAi = stale && ai;
  const { pos, dragging, onGrabHandle, onReset } = useDraggable("verdict-pos");
  // pos = null -> sit at the CSS-default top-right corner; once dragged we pin
  // it explicitly with top/left so it stays exactly where the user dropped it.
  const style = pos ? { top: pos.top, left: pos.left, right: "auto" } : undefined;
  return (
    <div
      className={"verdict-header " + (risk ? "risk" : "ok") + (dragging ? " dragging" : "")}
      style={style}
    >
      <div className="vh-drag" onMouseDown={onGrabHandle} title="Drag to move • double-click to reset" onDoubleClick={onReset}>
        <span className="vh-grip">⠿</span>
        <div className="vh-badge">{risk ? "⚠ AT COMPLIANCE RISK" : "✓ COMPLIANT"}</div>
      </div>
      <div className="vh-detail">
        <b>{workflow.title}</b> — {whyText(verdict)}
      </div>
      <div className="vh-rule">
        <span className={"rule-group " + (staleAndAi ? "firing" : "")}>
          <RulePill on={stale} label="docs stale" />
          <span className="vh-and">AND</span>
          <RulePill on={ai} label="AI runs a step" />
        </span>
        <span className="vh-or">OR</span>
        <RulePill on={over} label="over time budget" firing={over} />
        <span className="vh-or">OR</span>
        {/* the implicit "no broken rules" input — a consistency violation alone
            forces AT RISK, so COMPLIANT can never sit beside a broken rule */}
        <RulePill on={broken} label="a rule is broken" firing={broken} />
        <span className="vh-eq">→ {risk ? "AT RISK" : "ok"}</span>
      </div>
      {hasScenarios && (
        <button className="vh-scenario-btn" disabled={busy} onClick={onOpenScenarios}>
          ✦ Try a scenario…
        </button>
      )}
    </div>
  );
}

// Plain-language explanation of the CURRENT verdict — names exactly which
// condition(s) are forcing it, so the "why" is never a mystery.
function whyText(v) {
  const reasons = [];
  if (v.isStale && v.hasAIExecutedStep) reasons.push("docs are stale AND an AI runs a step");
  if (v.isOverTimeBudget)
    reasons.push(`the plan runs ${v.totalPlanMinutes} min, over its ${v.timeBudgetMinutes} min budget`);
  if (v.hasConsistencyViolation) {
    const n = v.consistencyViolationCount || 0;
    reasons.push(`${n} step${n === 1 ? "" : "s"} break the human-sign-off rule (an approval step isn't human-filled)`);
  }
  if (reasons.length) return "at risk because " + reasons.join("; and ");
  // compliant — explain what's keeping it safe (all three inputs are clear)
  if (v.hasAIExecutedStep && !v.isStale)
    return "an AI runs a step, but the docs are fresh, the plan is within budget, and no rule is broken — compliant";
  return "no stale-plus-AI risk, within the time budget, and no broken rules — compliant";
}

function RulePill({ on, label, firing }) {
  return (
    <span className={"rule-pill " + (on ? "on" : "off") + (firing ? " firing" : "")}>
      {on ? "✓" : "○"} {label}
    </span>
  );
}

// ---- the time-budget bar: the live total vs. the FIXED 4-hour budget -------
// The fill is COMPOSED of one colored segment per step, so you can see exactly
// how each step's duration builds the plan runtime. Each segment is RESIZABLE:
// drag the splitter on its right edge to change that step's duration right where
// you see its effect (no separate buttons). Green/yellow/red zoning is conveyed
// by the budget marker + overflow sliver, not the segment colors.
// The budget is a FIXED raw fact (MaxPlanMinutes = 240 / 4h, seeded in the
// rulebook); it is not user-adjustable. The plan fits or breaches by resizing a
// step's segment, not by moving the budget.
const DUR_SNAP = 15;   // drag snaps to 15-minute steps
const DUR_MIN = 15;    // a step can't go below one snap
const DUR_MAX = 240;   // a single step can't exceed the whole 4h budget
const snapDur = (m) => Math.max(DUR_MIN, Math.min(DUR_MAX, Math.round(m / DUR_SNAP) * DUR_SNAP));

function TimeBudgetBar({ sit, workflow, verdict, handlers, busy }) {
  const steps = sit.steps || [];
  const facts = sit.stepFacts || {};
  // Per-step minutes, read from the already-derived facts (the view is the
  // contract — we never recompute a duration here, only lay them side by side).
  // Each segment is colored by who runs the step (blue person / purple AI /
  // amber pipeline), mirroring the step cards, and labeled "Step #N".
  const segs = steps.map((s, i) => ({
    id: s.id,
    pos: s.position ?? i + 1,
    title: s.title,
    min: Number(facts[s.id]?.stepDurationMinutes ?? s.durationMinutes ?? 0),
    kind: kindOfType(s.executingAgentType),
  }));
  const budget = Number(workflow.maxPlanMinutes ?? verdict.timeBudgetMinutes ?? 0);
  const over = !!(workflow.isOverTimeBudget ?? verdict.isOverTimeBudget);

  const trackRef = useRef(null);
  // While dragging a splitter we hold a LIVE preview value for the dragged step
  // so the segment widths track the cursor without re-reasoning. On release we
  // commit ONE patch (one re-reason); the doctrine of "no mid-drag stuck slider"
  // is preserved — the reasoner only runs on the committed value.
  const [drag, setDrag] = useState(null); // { id, min } | null

  // Effective minutes for layout: the dragged step shows its live preview, all
  // others show their committed value.
  const effMin = (s) => (drag && drag.id === s.id ? drag.min : s.min);
  const previewTotal = segs.reduce((a, s) => a + effMin(s), 0);
  // FIXED axis: budget * 1.5 pins the budget marker at exactly two-thirds across
  // (budget / (budget*1.5) = 0.667), so "where it goes over budget" is a stable
  // landmark instead of sliding with the plan — same trick as the 12mo policy
  // line on the staleness slider. The axis only grows past that if the plan
  // itself exceeds 150% of budget (an extreme drag), so the segments stay honest.
  const axisMax = Math.max(budget * 1.5, previewTotal, 1);
  const pctOf = (m) => (m / axisMax) * 100;
  const budgetPct = pctOf(budget);
  const previewOver = previewTotal > budget;
  const zone = (drag ? previewOver : over) ? "red"
    : previewTotal / Math.max(budget, 1) >= 0.8 ? "yellow" : "green";

  // Drag math: convert a pixel delta on the track into a minute delta using the
  // track's pixel width and the current axis scale, then snap to 15 min.
  function startDrag(seg, e) {
    if (busy) return;             // reasoner is locked — don't start a resize
    e.preventDefault();
    const track = trackRef.current;
    if (!track) return;
    const trackPx = track.getBoundingClientRect().width;
    const minPerPx = axisMax / trackPx;
    const startX = e.clientX;
    const startMin = seg.min;

    const onMove = (ev) => {
      const dxMin = (ev.clientX - startX) * minPerPx;
      setDrag({ id: seg.id, min: snapDur(startMin + dxMin) });
    };
    const onUp = (ev) => {
      window.removeEventListener("mousemove", onMove);
      window.removeEventListener("mouseup", onUp);
      const dxMin = (ev.clientX - startX) * minPerPx;
      const next = snapDur(startMin + dxMin);
      setDrag(null);
      if (next !== startMin) handlers.patchStep(seg.id, { stepDurationMinutes: next });
    };
    window.addEventListener("mousemove", onMove);
    window.addEventListener("mouseup", onUp);
    setDrag({ id: seg.id, min: startMin });
  }

  return (
    <div className={"timebudget " + zone + (drag ? " dragging" : "")}>
      <div className="tb-head">
        <span className="tb-title">⏱ Plan runtime</span>
        <span className="tb-readout">
          <b>{previewTotal}</b> min of <b>{budget}</b> min budget
          {(drag ? previewOver : over)
            ? <span className="tb-flag"> · OVER by {previewTotal - budget} min → AT RISK</span>
            : <span className="tb-ok"> · within budget</span>}
          {drag && <span className="tb-dragnote"> · release to recompute</span>}
        </span>
      </div>

      <div className="tb-track" ref={trackRef}>
        {/* one segment per step, widths proportional to its minutes, colored by
            who runs it (mirrors the step cards) and labeled "Step #N". The
            splitter on each segment's right edge resizes that step. */}
        {segs.map((s) => {
          const m = effMin(s);
          if (m <= 0) return null;
          const isDragged = drag && drag.id === s.id;
          return (
            <div
              key={s.id}
              className={"tb-seg k-" + s.kind + (isDragged ? " dragged" : "")}
              style={{ width: pctOf(m) + "%" }}
              title={`Step #${s.pos}: ${s.title} — ${m} min (${KIND[s.kind].label}) · drag the edge to resize`}
            >
              <span className="tb-seg-num">Step #{s.pos}</span>
              {isDragged && <span className="tb-seg-live">{m}m</span>}
              {/* the splitter handle — drag to change this step's duration */}
              <span
                className={"tb-split" + (busy ? " disabled" : "")}
                onMouseDown={(e) => startDrag(s, e)}
                title={busy ? "reasoning…" : `resize Step #${s.pos} (15-min steps)`}
                role="separator"
                aria-label={`resize Step #${s.pos} duration`}
              />
            </div>
          );
        })}
        {/* the budget marker — everything to its right is over budget */}
        <div className={"tb-limit " + ((drag ? previewOver : over) ? "breached" : "")} style={{ left: budgetPct + "%" }} title={`budget: ${budget} min`}>
          <span className="tb-limit-label">{budget}m budget</span>
        </div>
      </div>

      {/* legend: which step is which, its minutes, and who runs it */}
      <div className="tb-legend">
        {segs.map((s) => (
          <span key={s.id} className="tb-legend-item">
            <span className={"tb-swatch k-" + s.kind} />
            <span className="tb-legend-pos">Step #{s.pos}</span>
            <span className="muted">{effMin(s)}m</span>
          </span>
        ))}
      </div>

      <div className="tb-budgetctl">
        <span className="tb-budgetnote muted">budget is fixed at 4h — drag a step's segment edge to resize it (snaps to 15 min); the plan fits or breaches</span>
      </div>
    </div>
  );
}

// ---- the staleness bar: compliance-doc review age vs. the policy window ----
// Models CQ5: a workflow's documentation is "stale" when it hasn't been reviewed
// (Modified / dct:modified) within its policy window (StalenessThresholdMonths).
// EVERYTHING shown is read from already-derived fields — monthsSinceModified and
// isStale come from the reasoner/Postgres; we never recompute staleness in JS
// (the view is the contract). The two knobs (Modified date, threshold months)
// edit RAW facts; isStale re-derives on the next read.
//
// Header: a gauge (months reviewed-ago, with the threshold marked). Expands to a
// timeline: Modified ──→ now, with the threshold line; the bar turns red once
// the gap crosses it.
// Discrete policy-window options (months). Stepped buttons (not a slider) for
// the same reason as the budget: one committed click per re-reason, and they
// don't feel stuck while the board is locked mid-reason.
// The review-age axis is FIXED at 0..18 months so the 12-month policy line
// always sits two-thirds across the bar (12/18 = 67%). The single knob is the
// review age: slide the thumb left of the policy line = within policy; drag it
// past = stale. The policy window itself is a fixed fact (12mo, seeded in the
// rulebook) shown as a static marker, not an adjustable control.
const REVIEW_AGE_MIN = 0;
const REVIEW_AGE_MAX = 18;
const REVIEW_AGE_SNAP = 1;   // slide in 1-month increments
const snapAge = (m) =>
  Math.max(REVIEW_AGE_MIN, Math.min(REVIEW_AGE_MAX, Math.round(m / REVIEW_AGE_SNAP) * REVIEW_AGE_SNAP));
function monthsAgoISO(n) {
  const d = new Date();
  d.setMonth(d.getMonth() - n);
  return d.toISOString().slice(0, 10) + "T00:00:00-05:00";
}

function StalenessBar({ workflow, busy, onSetModified }) {
  const [open, setOpen] = useState(false);
  const months = Number(workflow.monthsSinceModified ?? 0);
  const threshold = Number(workflow.stalenessThresholdMonths ?? 12);
  const stale = !!workflow.isStale;
  const modified = workflow.modified;

  // While dragging the thumb we hold a LIVE preview age so the bar tracks the
  // cursor without re-reasoning; on release we commit ONE patch (Modified =
  // today − ageMonths) and the reasoner runs once. Same doctrine as the budget
  // bar: no mid-drag stuck slider, the reasoner only runs on the committed value.
  const [drag, setDrag] = useState(null); // preview age in months | null
  const ageMonths = drag != null ? drag : months;

  // Fixed 0..18 axis: the policy line lands at threshold/18 (12 → 67%).
  const pctOf = (m) => Math.max(0, Math.min(m / REVIEW_AGE_MAX, 1)) * 100;
  const monthsPct = pctOf(ageMonths);
  const threshPct = pctOf(threshold);
  // The PREVIEW staleness while dragging (so colors flip the instant you cross
  // the line, before the reasoner confirms it on release).
  const previewStale = ageMonths > threshold;
  const zone = previewStale ? "red" : ageMonths >= threshold * 0.75 ? "yellow" : "green";

  const modifiedDate = modified ? String(modified).slice(0, 10) : "";
  const commitAge = (n) => onSetModified(monthsAgoISO(snapAge(n)));

  return (
    <div className={"staleness " + zone}>
      <div className="st-head" onClick={() => setOpen((o) => !o)} role="button">
        <span className="st-title">📄 Docs review age</span>
        <span className="st-readout">
          last reviewed <b>{ageMonths}</b> mo ago · policy window <b>{threshold}</b> mo
          {previewStale ? <span className="st-flag"> · STALE → feeds AT RISK</span>
                        : <span className="st-ok"> · within policy</span>}
        </span>
        <span className="st-expand">{open ? "▾ hide timeline" : "▸ show timeline"}</span>
      </div>

      {/* the slider: a 0..18-month track. The thumb is the review age; the policy
          line is fixed at 12mo (≈two-thirds across). Left of the line = within
          policy; drag the thumb past it and the docs go stale. */}
      <div className="st-slider" onClick={(e) => e.stopPropagation()}>
        <div className={"st-track " + zone} title={`${ageMonths} months since review; stale at >${threshold}`}>
          <div className={"st-fill " + zone} style={{ width: monthsPct + "%" }} />
          <div className="st-threshold" style={{ left: threshPct + "%" }}>
            <span className="st-threshold-label">{threshold}mo policy</span>
          </div>
          <input
            type="range"
            className="st-range"
            min={REVIEW_AGE_MIN}
            max={REVIEW_AGE_MAX}
            step={REVIEW_AGE_SNAP}
            value={ageMonths}
            disabled={busy}
            aria-label="Months since last review"
            // live preview while dragging (no re-reason); commit once on release
            onChange={(e) => setDrag(snapAge(Number(e.target.value)))}
            onMouseUp={(e) => { const n = snapAge(Number(e.target.value)); setDrag(null); if (n !== months) commitAge(n); }}
            onKeyUp={(e) => { const n = snapAge(Number(e.target.value)); setDrag(null); if (n !== months) commitAge(n); }}
            onBlur={() => setDrag(null)}
          />
          <div className="st-needle" style={{ left: monthsPct + "%" }}>
            <span className="st-needle-label">now ({ageMonths}mo)</span>
          </div>
        </div>
        <div className="st-scale">
          <span>0</span><span>now — drag to set months since review</span><span>{REVIEW_AGE_MAX}mo</span>
        </div>
      </div>

      {open && (
        <div className="st-timeline">
          <div className="st-tl-axis">
            <span className="st-tl-start">📝 reviewed<br /><b>{modifiedDate || "—"}</b></span>
            <span className="st-tl-arrow">────────── {ageMonths} months elapsed ──────────▶</span>
            <span className="st-tl-now">today</span>
          </div>
          <div className="st-tl-bar">
            <div className={"st-tl-fill " + zone} style={{ width: monthsPct + "%" }} />
            <div className="st-tl-line" style={{ left: threshPct + "%" }} title={`stale after ${threshold} months`} />
          </div>
          <div className="st-tl-caption muted">
            {previewStale
              ? `Past the ${threshold}-month review window — the docs are stale. Combined with an AI-executed step, that is the article's "AT RISK" verdict.`
              : `Inside the ${threshold}-month window — docs are current. They go stale in ${Math.max(0, threshold - ageMonths)} more month(s) — slide the thumb past the ${threshold}mo line to see it flip.`}
          </div>
        </div>
      )}

      <div className="st-ctls">
        <span className="st-ctlnote muted">
          Slide the thumb to set how long since the docs were reviewed. The{" "}
          <b>{threshold}-month policy</b> line is fixed — drag past it and the docs go stale.
        </span>
      </div>
    </div>
  );
}

function BootError({ error, onRetry }) {
  return (
    <div className="boot-error">
      <h2>Couldn't reach the reasoner</h2>
      <pre>{error}</pre>
      <p className="muted">
        Is the backend up, and has <code>effortless build</code> generated <code>owl/src/*.ttl</code>?
        The API surfaces the reasoner's error verbatim — no silent fallback. Try <code>./start.sh</code>.
      </p>
      <button className="btn" onClick={onRetry}>retry</button>
    </div>
  );
}

export { KIND, kindOfType, initials };
