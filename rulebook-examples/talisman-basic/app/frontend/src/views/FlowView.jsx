import React, { useState, useRef, useEffect } from "react";
import { KIND, kindOfType } from "../model.js";
import { AgentAvatar, FactToggle } from "../Editable.jsx";

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

export function FlowView({ sit, handlers }) {
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
        Reassign a step to an 🤖 AI to add AI-risk · open a step's ⚙ to change its duration or sign-off ·
        the verdict and the time bar above recompute on every edit.
      </p>
    </div>
  );
}

function StepCard({ step, fact, handlers }) {
  const kind = kindOfType(step.executingAgentType);
  const isAI = kind === "ai";
  const [gearOpen, setGearOpen] = useState(false);
  const cardRef = useRef(null);

  // Click outside (or Escape) closes the settings popover.
  useEffect(() => {
    if (!gearOpen) return;
    const onDoc = (e) => { if (cardRef.current && !cardRef.current.contains(e.target)) setGearOpen(false); };
    const onKey = (e) => { if (e.key === "Escape") setGearOpen(false); };
    document.addEventListener("mousedown", onDoc);
    document.addEventListener("keydown", onKey);
    return () => { document.removeEventListener("mousedown", onDoc); document.removeEventListener("keydown", onKey); };
  }, [gearOpen]);

  return (
    <div
      ref={cardRef}
      className={
        "stepcard k-" + kind +
        (step.isApprovalGate ? " gate" : "") +
        (step.consistencyViolation ? " violation" : "") +
        (isAI ? " ai-risk" : "")
      }
    >
      {/* gear opens a popover with the editable knobs (duration + sign-off),
          keeping the card itself a clean read-only summary at a glance */}
      <button
        className={"sc-gear " + (gearOpen ? "on" : "")}
        title="step settings"
        onClick={() => setGearOpen((o) => !o)}
      >
        ⚙
      </button>

      <div className="sc-top">
        <span className="sc-pos">{step.position}</span>
        {step.isApprovalGate && <span className="sc-gate">🔒 gate</span>}
        {step.consistencyViolation && <span className="sc-violation">⚠ rule broken</span>}
      </div>
      <div className="sc-title">{step.title}</div>

      <div className="sc-assign">
        <AgentAvatar
          agent={step.agent}
          type={step.executingAgentType}
          roleId={step.roleId}
          onReassign={handlers.openReassign}
          size={40}
        />
        <div className="sc-who">
          <div className="sc-agent">{step.agent ? step.agent.name : "unassigned"}</div>
          <div className="sc-role">{step.role} · {KIND[kind].label}</div>
        </div>
      </div>

      {/* The PRECISE effect of who runs this step — always visible. */}
      <div className={"sc-effect " + (isAI ? "risk" : "safe")}>
        {isAI
          ? "🤖 AI runs this → counts toward risk (risk fires if docs are also stale)"
          : "🧑 not an AI → no AI-risk from this step"}
      </div>
      {step.consistencyViolation && (
        <div className="sc-effect broken">
          ⚠ requires human sign-off but isn't human-filled → consistency rule broken
        </div>
      )}

      {gearOpen && (
        <div className="sc-pop">
          <div className="sc-pop-section">
            <DurationControl step={step} fact={fact} handlers={handlers} />
          </div>
          <div className="sc-pop-divider" />
          <div className="sc-pop-section">
            <FactToggle
              on={fact.requiresHumanApproval}
              label="requires human sign-off"
              onChange={(v) => handlers.patchStep(step.id, { requiresHumanApproval: v })}
            />
          </div>
        </div>
      )}
    </div>
  );
}

// Inline duration buttons. Each click is one committed value (one re-reason) —
// no mid-drag stuck-slider while the reasoner is locked. Changing a step's
// duration moves the workflow's total runtime (the top time-budget bar).
const DURATION_STEPS = [15, 30, 60, 120];
function DurationControl({ step, fact, handlers }) {
  const current = Number(fact.stepDurationMinutes ?? step.durationMinutes ?? 0);
  return (
    <div className="sc-duration">
      <div className="sc-dur-head">
        <span className="muted">duration</span>
        <span className="sc-dur-val">{current} min</span>
      </div>
      <span className="sc-dur-steps">
        {DURATION_STEPS.map((m) => (
          <button
            key={m}
            className={"sc-dur-btn " + (current === m ? "on" : "")}
            onClick={() => { if (current !== m) handlers.patchStep(step.id, { stepDurationMinutes: m }); }}
          >
            {m}m
          </button>
        ))}
      </span>
      <div className="sc-dur-note muted">adds to the plan's total runtime ↑</div>
    </div>
  );
}
