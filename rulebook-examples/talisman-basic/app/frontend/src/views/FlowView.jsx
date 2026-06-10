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
  // This step REQUIRES a human sign-off (raw fact) — surface the expectation on
  // the card itself, before any edit, and forward it into the reassign popover.
  const requiresHuman = !!step.requiresHumanApproval;
  const needsHumanUnmet = requiresHuman && kind !== "human";
  const [gearOpen, setGearOpen] = useState(false);
  const [gearRect, setGearRect] = useState(null); // fixed anchor for the popover
  const cardRef = useRef(null);
  const gearRef = useRef(null);

  // Click outside (or Escape) closes the settings popover. The popover renders
  // OUTSIDE the card (fixed-position) so the card's overflow clip can't crop it,
  // so "outside" means: not in the card AND not in the popover itself.
  useEffect(() => {
    if (!gearOpen) return;
    const onDoc = (e) => {
      const inCard = cardRef.current && cardRef.current.contains(e.target);
      const inPop = e.target.closest && e.target.closest(".sc-pop");
      if (!inCard && !inPop) setGearOpen(false);
    };
    const onKey = (e) => { if (e.key === "Escape") setGearOpen(false); };
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
          <div className="sc-role">{step.role} · {KIND[kind].label}</div>
        </div>
      </div>

      {/* The PRECISE effect of who runs this step — always visible. */}
      <div className={"sc-effect " + (isAI ? "risk" : "safe")}>
        {isAI
          ? "🤖 AI runs this → counts toward risk (risk fires if docs are also stale)"
          : "🧑 not an AI → no AI-risk from this step"}
      </div>
      {/* This step expects a human but doesn't have one yet — explain the
          expectation BEFORE it turns into a hard violation. */}
      {requiresHuman && !step.consistencyViolation && (
        <div className="sc-effect needs">
          🔒 requires a human sign-off → assign a 🧑 human to keep it consistent
        </div>
      )}
      {step.consistencyViolation && (
        <div className="sc-effect broken">
          ⚠ requires human sign-off but isn't human-filled → consistency rule broken
        </div>
      )}

      {gearOpen && gearRect && (
        <div
          className="sc-pop"
          style={{
            position: "fixed",
            top: gearRect.bottom + 8,
            // right-align under the gear, clamped into the viewport
            left: Math.min(Math.max(gearRect.right - 230, 12), window.innerWidth - 242),
          }}
        >
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
