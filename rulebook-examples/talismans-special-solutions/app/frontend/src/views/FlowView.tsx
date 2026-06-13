import React, { useState, useRef, useEffect } from "react";
import { KIND, kindOfType, escalationAncestors } from "../model";
import { AgentAvatar, FactToggle } from "../Editable";
import { DagCell } from "../explainer-dag";
import type { AgentKind, Situation, Step, StepFact, Handlers } from "../types";

// ===========================================================================
// FLOW VIEW — the release as a left→right pipeline of step cards.
//
// This is the COMPLIANCE story. Each card makes the effect of every edit
// explicit: who runs the step, and whether that contributes to risk.
//
// Edits here re-answer the competency questions on the right:
//   • AI runs a step → reassign an agent (the avatar) to flip CQ3 / HasAIAgentStep
// (Staleness — CQ5 — is the workflow's Modified date, set on the staleness bar.)
// ===========================================================================

interface FlowViewProps {
  sit: Situation;
  handlers: Handlers;
}

export function FlowView({ sit, handlers }: FlowViewProps) {
  // ORDER AMBIGUITY (correlates with CQ1). This left→right sequence is only a
  // faithful "the order" when the precedence closure is TOTAL — every pair of
  // steps comparable, i.e. n·(n−1)/2 closure pairs. When it's a partial order,
  // steps tie on SequencePosition and this row is just one arbitrary
  // linearization. Same check as CQ1's resolver, surfaced where the order is
  // actually drawn — so a red CQ1 lines up with a flagged dashboard.
  const steps = sit.steps;
  const n = steps.length;
  const pairs = sit.closure?.count ?? 0;
  const needed = (n * (n - 1)) / 2;
  const isTotal = n > 0 && pairs === needed;
  const posCounts: Record<number, number> = {};
  for (const s of steps) posCounts[s.position] = (posCounts[s.position] || 0) + 1;
  const tiedCount = steps.filter((s) => posCounts[s.position] > 1).length;

  // DCAT dataset INPUTS, per consuming step. A dataset is NOT a step — it is an
  // input consumed BY a step (CQ8). So it renders as a categorically-DIFFERENT
  // box (a stack of papers, not a step card) that FEEDS its consuming step from
  // the left of the track — never as a peer in the ordered sequence (that would
  // conflate dcat:Dataset with WorkflowStep and pollute the CQ1 order). Driven by
  // the SAME reasoned consumedBySteps the Graph node and CQ8 read; detaching it
  // (the simulate) empties this map → the feeder box vanishes, so the visible cut
  // shows up here in the Flow lens too.
  const inputByStep: Record<string, { id: string; title: string }> = {};
  for (const d of sit.datasets) {
    for (const c of d.consumedBySteps) inputByStep[c.id] = { id: d.id, title: d.title };
  }

  return (
    <div className="flow">
      {!isTotal && n > 0 && (
        <div className="flow-ambiguous" role="alert">
          <b>⚠ This order is not determined.</b> The precedence closure is a{" "}
          <b>partial</b> order ({pairs}/{needed} pairs) — {tiedCount} step(s) tie on position,
          so the sequence below is just one arbitrary arrangement. Finish wiring the steps in the{" "}
          <b>Closure</b> tab (reach {needed} pairs) to make this the real order.{" "}
          <span className="muted">This is why CQ1 is failing.</span>
        </div>
      )}
      <div className="flow-track">
        {steps.map((s, i) => {
          const next = steps[i + 1];
          const ambiguousGap = !!next && s.position === next.position; // same rank → not ordered
          const input = inputByStep[s.id];
          return (
            <React.Fragment key={s.id}>
              {/* A dataset that this step consumes feeds IN from the left — a
                  categorically different box (stack of papers), not a step card. */}
              {input && (
                <>
                  <DatasetFeeder id={input.id} title={input.title} />
                  <div className="flow-arrow flow-arrow--feeds" title="feeds into (DCAT dataset consumed by this step)">⇢</div>
                </>
              )}
              {/* One column = the agent NODE on top (the old graph's circle),
                  a short stalk, then the step's REAL card directly beneath it.
                  The agent and the step it runs are the SAME column now — there is
                  no separate graph drawing the five steps a second time. */}
              <div className="sc-col">
                <StepAgentNode
                  step={s}
                  handlers={handlers}
                  escalatesTo={escalationAncestors(sit, s.roleId).map((a) => a.name)}
                  departmentTitle={s.owningDepartment ? sit.departmentById[s.owningDepartment]?.title ?? null : null}
                />
                <div className="sc-connector" aria-hidden="true" />
                <StepCard
                  step={s}
                  fact={sit.stepFacts[s.id] || {}}
                  handlers={handlers}
                  tied={posCounts[s.position] > 1}
                  escalationViolation={!!sit.roleById[s.roleId]?.escalationViolation}
                />
              </div>
              {i < steps.length - 1 && (
                <div className={"flow-arrow" + (ambiguousGap ? " flow-arrow--ambiguous" : "")}
                  title={ambiguousGap ? "these two steps share a position — their order is NOT asserted" : "comes before"}>
                  {ambiguousGap ? "∥?" : "→"}
                </div>
              )}
            </React.Fragment>
          );
        })}
      </div>
      <p className="flow-foot muted">
        Reassign a step to an 🤖 AI to add AI-risk · open a step's ⚙ for its sign-off · the competency
        answers on the right recompute on every edit.
      </p>
    </div>
  );
}

// A DCAT dataset rendered as a categorically-different "box" — a stack of papers,
// NOT a step card. It feeds the step to its right. The layered look (the .ds-stack
// sheets behind the face) is what signals "this is data, a record set" at a glance,
// so it can never be misread as a step in the ordered sequence. Pinned to
// Datasets.ConsumedBySteps so a click opens its derivation (and CQ8 lives there).
function DatasetFeeder({ id, title }: { id: string; title: string }) {
  return (
    <div className="flow-dataset" title={`DCAT dataset · input to the next step (${id})`}>
      <span className="ds-stack ds-stack-3" />
      <span className="ds-stack ds-stack-2" />
      <div className="ds-face">
        <div className="ds-kicker">📚 input dataset</div>
        <DagCell table="Datasets" field="ConsumedBySteps" block>
          <div className="ds-title">{title}</div>
        </DagCell>
        <div className="ds-sub">DCAT · consumed by →</div>
      </div>
    </div>
  );
}

interface StepCardProps {
  step: Step;
  fact: StepFact | Record<string, never>;
  handlers: Handlers;
  tied?: boolean;
  // The gate role's derived EscalationViolation — drives the ⚠ badge on the gate.
  escalationViolation?: boolean;
}

// Department hue (matches the Dept lens + graph hover).
function deptHue(title?: string | null): string {
  const s = title || "";
  if (/legal/i.test(s)) return "#f0719b";
  if (/eng/i.test(s)) return "#3fb6a8";
  return "var(--muted)";
}

// The agent NODE that sits on top of a step's card — this IS what the old graph's
// top row of circles was, but now it's wired straight to the real card below it
// (a short stalk), so the five steps are never drawn twice. The distinct face is
// the "who"; the name sits under it; role / department / escalation pop on hover.
// Clicking the avatar reassigns (the edit path is unchanged).
function StepAgentNode({
  step, handlers, escalatesTo, departmentTitle,
}: {
  step: Step;
  handlers: Handlers;
  escalatesTo: string[];
  departmentTitle?: string | null;
}) {
  const [hov, setHov] = useState<{ x: number; y: number } | null>(null);
  const kind: AgentKind = kindOfType(step.executingAgentType);
  const requiresHuman = !!step.requiresHumanApproval;
  return (
    <div
      className="sc-agentnode"
      onMouseEnter={(e) => setHov({ x: e.clientX, y: e.clientY })}
      onMouseMove={(e) => setHov({ x: e.clientX, y: e.clientY })}
      onMouseLeave={() => setHov(null)}
    >
      <AgentAvatar
        agent={step.agent}
        type={step.executingAgentType}
        roleId={step.roleId}
        onReassign={handlers.openReassign}
        requiresHuman={requiresHuman}
        size={50}
      />
      <div className="sc-agentname" title={step.agent ? step.agent.name : "unassigned"}>
        {step.agent ? step.agent.name : "unassigned"}
      </div>
      {hov && (
        <div
          className="graph-hovercard"
          style={{ left: Math.min(hov.x + 16, window.innerWidth - 260), top: Math.min(hov.y + 16, window.innerHeight - 170) }}
        >
          <div className="gh-title">{step.agent ? step.agent.name : "unassigned"}</div>
          <div className="gh-sub muted">{KIND[kind].icon} {KIND[kind].label} · fills {step.role}</div>
          <div className="gh-row">
            department{" "}
            <span className="gh-pill" style={{ borderColor: deptHue(departmentTitle), color: deptHue(departmentTitle) }}>
              🏛 {departmentTitle || "—"}
            </span>
          </div>
          {escalatesTo.length > 0 && (
            <div className="gh-row muted">↑ escalates to {escalatesTo.join(" → ")}</div>
          )}
          <div className="gh-row muted">click to reassign</div>
        </div>
      )}
    </div>
  );
}

function StepCard({ step, fact, handlers, tied, escalationViolation = false }: StepCardProps) {
  const kind: AgentKind = kindOfType(step.executingAgentType);
  const isAI = kind === "ai";
  // This step REQUIRES a human sign-off (raw fact) — surface the expectation on
  // the card itself, before any edit, and forward it into the reassign popover.
  const requiresHuman = !!step.requiresHumanApproval;
  const needsHumanUnmet = requiresHuman && kind !== "human";
  const [gearOpen, setGearOpen] = useState<boolean>(false);
  const [gearRect, setGearRect] = useState<DOMRect | null>(null); // fixed anchor for the popover
  const cardRef = useRef<HTMLDivElement | null>(null);
  const gearRef = useRef<HTMLButtonElement | null>(null);

  // Click outside (or Escape) closes the settings popover. The popover renders
  // OUTSIDE the card (fixed-position) so the card's overflow clip can't crop it,
  // so "outside" means: not in the card AND not in the popover itself.
  useEffect(() => {
    if (!gearOpen) return;
    const onDoc = (e: MouseEvent) => {
      const target = e.target as Node | null;
      const inCard = cardRef.current && target && cardRef.current.contains(target);
      const el = e.target as Element | null;
      const inPop = el && el.closest && el.closest(".sc-pop");
      if (!inCard && !inPop) setGearOpen(false);
    };
    const onKey = (e: KeyboardEvent) => { if (e.key === "Escape") setGearOpen(false); };
    // The popover is anchored at open-time; if the page or the flow-track scrolls
    // it would detach from the gear, so close it (same feel as the reassign popover).
    const onScroll = () => setGearOpen(false);
    document.addEventListener("mousedown", onDoc);
    document.addEventListener("keydown", onKey);
    window.addEventListener("scroll", onScroll, true);
    return () => {
      document.removeEventListener("mousedown", onDoc);
      document.removeEventListener("keydown", onKey);
      window.removeEventListener("scroll", onScroll, true);
    };
  }, [gearOpen]);

  const toggleGear = () => {
    setGearOpen((o) => {
      const next = !o;
      if (next && gearRef.current) setGearRect(gearRef.current.getBoundingClientRect());
      return next;
    });
  };

  return (
    <div
      ref={cardRef}
      className={
        "stepcard k-" + kind +
        (step.isApprovalGate ? " gate" : "") +
        (step.consistencyViolation ? " violation" : "") +
        (needsHumanUnmet ? " needs-human" : "") +
        (tied ? " tied" : "") +
        (isAI ? " ai-risk" : "")
      }
    >
      {/* gear opens a popover with the editable knobs (duration + sign-off),
          keeping the card itself a clean read-only summary at a glance */}
      <button
        ref={gearRef}
        className={"sc-gear " + (gearOpen ? "on" : "")}
        title="step settings"
        onClick={toggleGear}
      >
        ⚙
      </button>

      {/* The "who" is the agent node + stalk ABOVE this card, so the card body is
          just the step itself: badges (the only flags worth a word) + title. */}
      <div className="sc-top">
        <span className={"sc-pos" + (tied ? " tied" : "")} title={tied ? "another step shares this position — order not determined" : undefined}>{step.position}{tied ? " ⚠" : ""}</span>
        {step.isApprovalGate && <span className="sc-gate">🔒 gate</span>}
        {requiresHuman && !step.consistencyViolation && (
          <span className="sc-needs" title="this step requires a human sign-off">🔒 needs human</span>
        )}
        {step.consistencyViolation && <span className="sc-violation">⚠ rule broken</span>}
      </div>
      <div className="sc-title">{step.title}</div>

      {/* Gate-only: the one gate-specific contract worth a line — its escalation
          window. (Who owns/approves it is the avatar + its hover, so those rows
          are gone.) The escalation-broken witness stays: it's an actionable fix. */}
      {step.isApprovalGate && (
        <div className="sc-gate-detail">
          <div className="sc-gate-row">
            <span className="sc-gate-k">⏱ escalates after</span>
            <DagCell table="ApprovalGates" field="EscalationThresholdHours">
              <span className="sc-gate-v">
                {step.escalationThresholdHours != null ? `${step.escalationThresholdHours} h` : "—"}
              </span>
            </DagCell>
          </div>
          {/* The derived EscalationViolation witness: this gate's role has no
              delegatesTo target, so a stalled gate can't escalate. Click to open
              the org chart and restore the chain. */}
          {escalationViolation && (
            <button
              className="sc-escalation-broken"
              onClick={(e) => handlers.openEscalation(step.roleId, e.currentTarget.getBoundingClientRect())}
              title="This gate has no escalation target — open the org chart to fix the chain"
            >
              <DagCell table="Roles" field="EscalationViolation" block>
                ⚠ escalation broken — no one to escalate to → fix
              </DagCell>
            </button>
          )}
        </div>
      )}

      {/* The only derived effects worth a chip: AI-risk (the unique fact the
          badges DON'T already flag) and the hard violation (kept drillable so
          explainer mode still shows how the rule fired). "needs human" and the
          "not-an-AI / safe" line are gone — the badges above already say it, and
          the sentences just doubled the word count. */}
      {(isAI || step.consistencyViolation) && (
        <div className="sc-chips">
          {isAI && (
            <DagCell table="WorkflowSteps" field="IsExecutedByAI">
              <span className="sc-chip risk" title="AI runs this → counts toward risk (fires if docs are also stale)">🤖 AI-risk</span>
            </DagCell>
          )}
          {step.consistencyViolation && (
            <DagCell table="WorkflowSteps" field="ApprovalConsistencyViolation">
              <span className="sc-chip broken" title="requires human sign-off but isn't human-filled → consistency rule broken">⚠ rule broken</span>
            </DagCell>
          )}
        </div>
      )}

      {gearOpen && gearRect && (
        <div
          className="sc-pop"
          style={{
            position: "fixed",
            // Prefer below the gear; if that would run off the BOTTOM edge, flip
            // above it; then clamp into [12, innerHeight-12-popH] so it can never
            // be clipped by the bottom (or top) of the screen. (~150px popover.)
            top: (() => {
              const POP_H = 150, M = 12;
              const below = gearRect.bottom + 8;
              const above = gearRect.top - 8 - POP_H;
              const t = below + POP_H > window.innerHeight - M
                ? (above >= M ? above : window.innerHeight - M - POP_H)
                : below;
              return Math.max(M, t);
            })(),
            // right-align under the gear, clamped into the viewport
            left: Math.min(Math.max(gearRect.right - 230, 12), window.innerWidth - 242),
          }}
        >
          <div className="sc-pop-section">
            <FactToggle
              on={(fact as StepFact).requiresHumanApproval}
              label="requires human sign-off"
              onChange={(v: boolean) => handlers.patchStep(step.id, { requiresHumanApproval: v })}
            />
          </div>
          {/* Duration is no longer edited here — drag a segment's edge on the
              Plan-runtime bar at the top to resize a step right where you see
              its effect on the plan runtime. */}
          <div className="sc-pop-divider" />
          <div className="sc-pop-section">
            <span className="muted sc-pop-note">
              ⏱ duration is set by dragging the step's segment on the time bar above
            </span>
          </div>
        </div>
      )}
    </div>
  );
}
