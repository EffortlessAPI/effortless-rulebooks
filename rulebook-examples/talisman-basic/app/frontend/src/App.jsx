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
        onSetThreshold={handlers.setStalenessThreshold}
        onSetModified={handlers.setModified}
      />
      <TimeBudgetBar sit={sit} workflow={sit.workflow} verdict={sit.verdict} />

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
function VerdictHeader({ verdict, workflow, hasScenarios, busy, onOpenScenarios }) {
  const risk = verdict.isAtComplianceRisk;
  const stale = verdict.isStale;
  const ai = verdict.hasAIExecutedStep;
  const over = verdict.isOverTimeBudget;
  const broken = verdict.hasConsistencyViolation;
  const staleAndAi = stale && ai;
  return (
    <div className={"verdict-header " + (risk ? "risk" : "ok")}>
      <div className="vh-badge">{risk ? "⚠ AT COMPLIANCE RISK" : "✓ COMPLIANT"}</div>
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
// how each step's duration builds the plan runtime. Green/yellow/red zoning is
// conveyed by the budget marker + overflow sliver, not the segment colors.
// The budget is a FIXED raw fact (MaxPlanMinutes = 240 / 4h, seeded in the
// rulebook); it is not user-adjustable. The plan fits or breaches by editing a
// step's duration, not by moving the budget.
function TimeBudgetBar({ sit, workflow, verdict }) {
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
  const total = Number(workflow.totalPlanMinutes ?? verdict.totalPlanMinutes ?? segs.reduce((a, s) => a + s.min, 0));
  const budget = Number(workflow.maxPlanMinutes ?? verdict.timeBudgetMinutes ?? 0);
  const over = !!(workflow.isOverTimeBudget ?? verdict.isOverTimeBudget);
  // The track spans 0 .. max(total, budget) so both the bar and the budget
  // marker always fit and their crossing point is honest.
  const axisMax = Math.max(total, budget, 1);
  const pctOf = (m) => (m / axisMax) * 100;
  const budgetPct = pctOf(budget);
  const zone = over ? "red" : total / Math.max(budget, 1) >= 0.8 ? "yellow" : "green";

  return (
    <div className={"timebudget " + zone}>
      <div className="tb-head">
        <span className="tb-title">⏱ Plan runtime</span>
        <span className="tb-readout">
          <b>{total}</b> min of <b>{budget}</b> min budget
          {over ? <span className="tb-flag"> · OVER by {total - budget} min → AT RISK</span>
                : <span className="tb-ok"> · within budget</span>}
        </span>
      </div>

      <div className="tb-track">
        {/* one segment per step, widths proportional to its minutes, colored by
            who runs it (mirrors the step cards) and labeled "Step #N" */}
        {segs.map((s) => (
          s.min > 0 && (
            <div
              key={s.id}
              className={"tb-seg k-" + s.kind}
              style={{ width: pctOf(s.min) + "%" }}
              title={`Step #${s.pos}: ${s.title} — ${s.min} min (${KIND[s.kind].label})`}
            >
              <span className="tb-seg-num">Step #{s.pos}</span>
            </div>
          )
        ))}
        {/* the budget marker — everything to its right is over budget */}
        <div className={"tb-limit " + (over ? "breached" : "")} style={{ left: budgetPct + "%" }} title={`budget: ${budget} min`}>
          <span className="tb-limit-label">{budget}m budget</span>
        </div>
      </div>

      {/* legend: which step is which, its minutes, and who runs it */}
      <div className="tb-legend">
        {segs.map((s) => (
          <span key={s.id} className="tb-legend-item">
            <span className={"tb-swatch k-" + s.kind} />
            <span className="tb-legend-pos">Step #{s.pos}</span>
            <span className="muted">{s.min}m</span>
          </span>
        ))}
      </div>

      <div className="tb-budgetctl">
        <span className="tb-budgetnote muted">budget is fixed at 4h — change a step's duration to fit or breach</span>
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
const STALENESS_STEPS = [3, 6, 12, 18, 24];
// "Last reviewed N months ago" presets — one committed click each, like the
// budget/duration buttons. Stamps Modified to (today − N months).
const REVIEW_AGE_STEPS = [3, 6, 12, 18];
function monthsAgoISO(n) {
  const d = new Date();
  d.setMonth(d.getMonth() - n);
  return d.toISOString().slice(0, 10) + "T00:00:00-05:00";
}

function StalenessBar({ workflow, busy, onSetThreshold, onSetModified }) {
  const [open, setOpen] = useState(false);
  const months = Number(workflow.monthsSinceModified ?? 0);
  const threshold = Number(workflow.stalenessThresholdMonths ?? 12);
  const stale = !!workflow.isStale;
  const modified = workflow.modified;

  // Axis: 0 .. max(threshold, months) + headroom, so both the needle and the
  // threshold line always fit and the crossing is visible.
  const axisMax = Math.max(threshold, months, 1) * 1.25;
  const pctOf = (m) => Math.max(0, Math.min(m / axisMax, 1)) * 100;
  const monthsPct = pctOf(months);
  const threshPct = pctOf(threshold);
  const zone = stale ? "red" : months >= threshold * 0.75 ? "yellow" : "green";

  const modifiedDate = modified ? String(modified).slice(0, 10) : "";
  const setReviewedToday = () => {
    // "Mark docs reviewed" = stamp Modified with today's date (resets the clock).
    const today = new Date().toISOString().slice(0, 10) + "T00:00:00-05:00";
    onSetModified(today);
  };

  return (
    <div className={"staleness " + zone}>
      <div className="st-head" onClick={() => setOpen((o) => !o)} role="button">
        <span className="st-title">📄 Docs review age</span>
        <span className="st-readout">
          last reviewed <b>{months}</b> mo ago · policy window <b>{threshold}</b> mo
          {stale ? <span className="st-flag"> · STALE → feeds AT RISK</span>
                 : <span className="st-ok"> · within policy</span>}
        </span>
        <span className="st-expand">{open ? "▾ hide timeline" : "▸ show timeline"}</span>
      </div>

      {/* the gauge: a track from 0 to axisMax, fill = months-ago, threshold line */}
      <div className="st-track" title={`${months} months since review; stale at >${threshold}`}>
        <div className={"st-fill " + zone} style={{ width: monthsPct + "%" }} />
        <div className="st-threshold" style={{ left: threshPct + "%" }}>
          <span className="st-threshold-label">{threshold}mo policy</span>
        </div>
        <div className="st-needle" style={{ left: monthsPct + "%" }}>
          <span className="st-needle-label">now ({months}mo)</span>
        </div>
      </div>

      {open && (
        <div className="st-timeline">
          <div className="st-tl-axis">
            <span className="st-tl-start">📝 reviewed<br /><b>{modifiedDate || "—"}</b></span>
            <span className="st-tl-arrow">────────── {months} months elapsed ──────────▶</span>
            <span className="st-tl-now">today</span>
          </div>
          <div className="st-tl-bar">
            <div className={"st-tl-fill " + zone} style={{ width: monthsPct + "%" }} />
            <div className="st-tl-line" style={{ left: threshPct + "%" }} title={`stale after ${threshold} months`} />
          </div>
          <div className="st-tl-caption muted">
            {stale
              ? `Past the ${threshold}-month review window — the docs are stale. Combined with an AI-executed step, that is the article's "AT RISK" verdict.`
              : `Inside the ${threshold}-month window — docs are current. They go stale in ${Math.max(0, threshold - months)} more month(s) (or sooner if you tighten the policy).`}
          </div>
        </div>
      )}

      <div className="st-ctls">
        <span className="st-ctl">
          <span className="muted">last reviewed</span>
          <span className="st-steps">
            {REVIEW_AGE_STEPS.map((n) => (
              <button
                key={n}
                className={"st-step-btn " + (months === n ? "on" : "")}
                disabled={busy}
                onClick={() => onSetModified(monthsAgoISO(n))}
              >
                {n}mo ago
              </button>
            ))}
          </span>
        </span>
        <span className="st-ctl st-threshold-ctl">
          <span className="muted">policy window</span>
          <span className="st-steps">
            {STALENESS_STEPS.map((m) => (
              <button
                key={m}
                className={"st-step-btn " + (threshold === m ? "on" : "")}
                disabled={busy}
                onClick={() => onSetThreshold(m)}
              >
                {m}mo
              </button>
            ))}
          </span>
        </span>
        <span className="st-ctlnote muted">backdate the review date, or tighten the policy, to make it stale</span>
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
