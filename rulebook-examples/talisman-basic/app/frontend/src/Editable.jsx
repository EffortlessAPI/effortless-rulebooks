import React, { useEffect, useRef } from "react";
import { KIND } from "./model.js";

// ===========================================================================
// Shared editable controls used by every view, so "what's editable" looks and
// behaves identically no matter which lens you're in.
// ===========================================================================

// An agent avatar. If `roleId` is given it's EDITABLE: clicking opens the
// reassign popover (via onReassign). Shows the agent-kind icon front-and-center
// (the icon IS the thing being regulated) inside a type-colored ring.
// `requiresHuman` (the step's raw requiresHumanApproval fact) is forwarded to
// the reassign popover so it can tell the user a human is expected BEFORE they
// pick — and a 🔒 hint on the ring signals it at a glance.
export function AgentAvatar({ agent, type, roleId, onReassign, size = 34, showName = false, requiresHuman = false }) {
  const kind = agent?.kind || (type ? typeKind(type) : "unknown");
  const k = KIND[kind] || KIND.unknown;
  const editable = !!roleId && !!onReassign;
  const ref = useRef(null);
  const onClick = (e) => {
    if (!editable) return;
    e.stopPropagation();
    onReassign(roleId, ref.current.getBoundingClientRect(), requiresHuman);
  };
  // A step that requires human sign-off but is NOT human-filled is currently in
  // conflict — flag the ring so the mismatch is visible before any click.
  const conflict = requiresHuman && kind !== "human";
  const title = editable
    ? `${agent ? agent.name : "unassigned"} — click to reassign` +
      (requiresHuman ? " · this step needs a 🧑 human sign-off" : "")
    : agent?.name;
  return (
    <button
      ref={ref}
      className={"avatar " + k.cls + (editable ? " editable" : "") + (conflict ? " conflict" : "")}
      style={{ width: size, height: size, fontSize: Math.round(size * 0.55) }}
      onClick={onClick}
      title={title}
      disabled={!editable}
    >
      <span className="avatar-kind">{k.icon}</span>
      {requiresHuman && <span className="avatar-req" title="this step requires a human sign-off">🔒</span>}
      {showName && <span className="avatar-name">{agent ? agent.name : "unassigned"}</span>}
    </button>
  );
}
function typeKind(t) {
  return t === "AIAgent" ? "ai" : t === "AutomatedPipeline" ? "pipeline" : t === "HumanAgent" ? "human" : "unknown";
}

// The reassign popover, anchored to the clicked avatar. Lists every agent
// grouped by kind; picking one PATCHes the role's filledBy* arm.
//
// `requiresHuman` (the step's raw requiresHumanApproval fact) drives the
// guidance: when set, a banner says a human is expected, the human group is
// marked "✓ allowed", and every non-human option is marked as a conflict with a
// hover tip explaining WHY it isn't allowed. The pick is still permitted (App
// confirms it) — the point is to show the expectation up front, not after the
// step turns red.
export function ReassignPopover({ role, agents, anchorRect, requiresHuman = false, onPick, onClose }) {
  const ref = useRef(null);
  useEffect(() => {
    const onDoc = (e) => { if (ref.current && !ref.current.contains(e.target)) onClose(); };
    const onEsc = (e) => e.key === "Escape" && onClose();
    document.addEventListener("mousedown", onDoc);
    document.addEventListener("keydown", onEsc);
    return () => { document.removeEventListener("mousedown", onDoc); document.removeEventListener("keydown", onEsc); };
  }, [onClose]);

  const current = role?.filledByHumanAgent || role?.filledByAIAgent || role?.filledByAutomatedPipeline || "";
  // position below the anchor, clamped to viewport
  const top = Math.min((anchorRect?.bottom || 100) + 8, window.innerHeight - 360);
  const left = Math.min(Math.max((anchorRect?.left || 100) - 80, 12), window.innerWidth - 280);

  const groups = [
    ["human", agents.filter((a) => a.kind === "human")],
    ["ai", agents.filter((a) => a.kind === "ai")],
    ["pipeline", agents.filter((a) => a.kind === "pipeline")],
  ];
  const conflictTip = "This step requires a human sign-off — assigning a non-human breaks the consistency rule (the reasoner flags it ⚠ rule broken).";
  return (
    <div className={"popover" + (requiresHuman ? " needs-human" : "")} ref={ref} style={{ top, left }}>
      <div className="popover-head">
        Who fills <b>{role?.name}</b>?
        <span className="popover-note">picking changes the role — every step using it re-resolves</span>
      </div>
      {requiresHuman && (
        <div className="popover-guide" role="note">
          🔒 This step <b>requires a human sign-off</b>. Pick someone from <b>🧑 Human</b> below.
          Non-human choices are still allowed, but they’ll trip the consistency rule.
        </div>
      )}
      <div className="popover-body">
        {groups.map(([kind, list]) => {
          if (!list.length) return null;
          const conflicts = requiresHuman && kind !== "human";
          const allowed = requiresHuman && kind === "human";
          return (
            <div className={"popover-group" + (conflicts ? " conflict" : "")} key={kind}>
              <div className="popover-group-label">
                {KIND[kind].icon} {KIND[kind].label}
                {allowed && <span className="pg-tag ok">✓ allowed</span>}
                {conflicts && <span className="pg-tag bad" title={conflictTip}>⚠ breaks the rule</span>}
              </div>
              {list.map((a) => (
                <button
                  key={a.id}
                  className={"popover-opt " + (a.id === current ? "current" : "") + (conflicts ? " conflict" : "")}
                  title={conflicts ? conflictTip : undefined}
                  onClick={() => onPick(a.id)}
                >
                  <span className={"opt-dot " + KIND[kind].cls} />
                  {a.name}
                  {conflicts && <span className="opt-warn" aria-hidden="true">⚠</span>}
                  {a.id === current && <span className="opt-check">current</span>}
                </button>
              ))}
            </div>
          );
        })}
      </div>
      <div className="popover-foot">
        Reassigning an approval step to a non-human will trip the consistency rule — the reasoner flags it.
      </div>
    </div>
  );
}

// A small inline toggle for a boolean step fact (e.g. requiresHumanApproval).
export function FactToggle({ on, label, onChange, busy }) {
  return (
    <label className={"fact-toggle " + (on ? "on" : "")}>
      <input type="checkbox" checked={!!on} disabled={busy} onChange={(e) => onChange(e.target.checked)} />
      <span>{label}</span>
    </label>
  );
}
