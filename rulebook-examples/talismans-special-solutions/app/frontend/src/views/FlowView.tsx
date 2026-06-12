import React, { useState, useRef, useEffect } from "react";
import { KIND, kindOfType } from "../model";
import { AgentAvatar, FactToggle } from "../Editable";
import { DagCell } from "../explainer-dag";
import type { AgentKind, Situation, Step, StepFact, Handlers } from "../types";

// ===========================================================================
// FLOW VIEW — the release as a left→right pipeline of step cards.
//
// This is the COMPLIANCE story. Each card makes the effect of every edit
// explicit: who runs the step (and whether that contributes to risk), and how
// long it takes (which feeds the time-budget bar at the top).
//
// Two of the three verdict inputs are driven from here:
//   • AI runs a step   → reassign an agent (the avatar) to flip it
//   • over time budget → change a step's duration; the time bar reacts
// (The third, "docs stale", is the workflow's Modified date.)
// ===========================================================================

interface FlowViewProps {
  sit: Situation;
  handlers: Handlers;
}

export function FlowView({ sit, handlers }: FlowViewProps) {
  return (
    <div className="flow">
      <div className="flow-track">
        {sit.steps.map((s, i) => (
          <React.Fragment key={s.id}>
            <StepCard
              step={s}
              fact={sit.stepFacts[s.id] || {}}
              handlers={handlers}
            />
            {i < sit.steps.length - 1 && <div className="flow-arrow">→</div>}
          </React.Fragment>
        ))}
      </div>
      <p className="flow-foot muted">
        Reassign a step to an 🤖 AI to add AI-risk · open a step's ⚙ for its sign-off · drag a step's
        segment edge on the Plan-runtime bar above to change its duration · the verdict and the bars recompute on every edit.
      </p>
    </div>
  );
}

interface StepCardProps {
  step: Step;
  fact: StepFact | Record<string, never>;
  handlers: Handlers;
}

function StepCard({ step, fact, handlers }: StepCardProps) {
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

      {/* badges live in their own row, with right padding reserved for the gear
          so a "rule broken" / "needs human" flag is never hidden behind it */}
      <div className="sc-top">
        <span className="sc-pos">{step.position}</span>
        {step.isApprovalGate && <span className="sc-gate">🔒 gate</span>}
        {requiresHuman && !step.consistencyViolation && (
          <span className="sc-needs" title="this step requires a human sign-off">🔒 needs human</span>
        )}
        {step.consistencyViolation && <span className="sc-violation">⚠ rule broken</span>}
      </div>
      <div className="sc-title">{step.title}</div>

      <div className="sc-assign">
        <AgentAvatar
          agent={step.agent}
          type={step.executingAgentType}
          roleId={step.roleId}
          onReassign={handlers.openReassign}
          requiresHuman={requiresHuman}
          size={40}
        />
        <div className="sc-who">
          <div className="sc-agent">{step.agent ? step.agent.name : "unassigned"}</div>
          {/* the kind label is the reasoner-resolved ExecutingAgentType (role→agent→type) */}
          <div className="sc-role">
            <DagCell table="WorkflowSteps" field="ExecutingAgentType" block>
              {step.role} · {KIND[kind].label}
            </DagCell>
          </div>
        </div>
      </div>

      {/* Gate-only: the approval gate's defining properties. Today the card only
          shows the 🔒 badge; surface the gate's actual contract — its escalation
          window, the role that owns it, and the human who signs off. These are
          ApprovalGates columns the reasoner resolved; we just render them. */}
      {step.isApprovalGate && (
        <div className="sc-gate-detail">
          <div className="sc-gate-row">
            <span className="sc-gate-k">escalation</span>
            <DagCell table="ApprovalGates" field="EscalationThresholdHours">
              <span className="sc-gate-v">
                {step.escalationThresholdHours != null ? `${step.escalationThresholdHours} h` : "—"}
              </span>
            </DagCell>
          </div>
          <div className="sc-gate-row">
            <span className="sc-gate-k">gate role</span>
            <span className="sc-gate-v">{step.gateRole ?? "—"}</span>
          </div>
          <div className="sc-gate-row">
            <span className="sc-gate-k">approver</span>
            <span className="sc-gate-v">{step.gateApproverHuman ?? "—"}</span>
          </div>
        </div>
      )}

      {/* The PRECISE effect of who runs this step — always visible. Driven by the
          derived IsExecutedByAI boolean. */}
      <div className={"sc-effect " + (isAI ? "risk" : "safe")}>
        <DagCell table="WorkflowSteps" field="IsExecutedByAI" block>
          {isAI
            ? "🤖 AI runs this → counts toward risk (risk fires if docs are also stale)"
            : "🧑 not an AI → no AI-risk from this step"}
        </DagCell>
      </div>
      {/* This step expects a human but doesn't have one yet — explain the
          expectation BEFORE it turns into a hard violation. Driven by the
          derived ApprovalIsHumanFilled boolean. */}
      {requiresHuman && !step.consistencyViolation && (
        <div className="sc-effect needs">
          <DagCell table="WorkflowSteps" field="ApprovalIsHumanFilled" block>
            🔒 requires a human sign-off → assign a 🧑 human to keep it consistent
          </DagCell>
        </div>
      )}
      {/* The hard violation IS the derived ApprovalConsistencyViolation boolean —
          wrap it so explainer mode shows exactly how the rule fired. */}
      {step.consistencyViolation && (
        <div className="sc-effect broken">
          <DagCell table="WorkflowSteps" field="ApprovalConsistencyViolation" block>
            ⚠ requires human sign-off but isn't human-filled → consistency rule broken
          </DagCell>
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
