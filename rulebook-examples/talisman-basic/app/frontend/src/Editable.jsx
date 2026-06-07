import React, { useEffect, useRef } from "react";
import { KIND, initials } from "./model.js";

// ===========================================================================
// Shared editable controls used by every view, so "what's editable" looks and
// behaves identically no matter which lens you're in.
// ===========================================================================

// An agent avatar. If `roleId` is given it's EDITABLE: clicking opens the
// reassign popover (via onReassign). Shows initials + a type-colored ring.
export function AgentAvatar({ agent, type, roleId, onReassign, size = 34, showName = false }) {
  const kind = agent?.kind || (type ? typeKind(type) : "unknown");
  const k = KIND[kind] || KIND.unknown;
  const editable = !!roleId && !!onReassign;
  const ref = useRef(null);
  const onClick = (e) => {
    if (!editable) return;
    e.stopPropagation();
    onReassign(roleId, ref.current.getBoundingClientRect());
  };
  return (
    <button
      ref={ref}
      className={"avatar " + k.cls + (editable ? " editable" : "")}
      style={{ width: size, height: size }}
      onClick={onClick}
      title={editable ? `${agent ? agent.name : "unassigned"} — click to reassign` : agent?.name}
      disabled={!editable}
    >
      <span className="avatar-init">{agent ? initials(agent.name) : "?"}</span>
      <span className="avatar-kind">{k.icon}</span>
      {showName && <span className="avatar-name">{agent ? agent.name : "unassigned"}</span>}
    </button>
  );
}
function typeKind(t) {
  return t === "AIAgent" ? "ai" : t === "AutomatedPipeline" ? "pipeline" : t === "HumanAgent" ? "human" : "unknown";
}

// The reassign popover, anchored to the clicked avatar. Lists every agent
// grouped by kind; picking one PATCHes the role's filledBy* arm.
export function ReassignPopover({ role, agents, anchorRect, onPick, onClose }) {
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
  const top = Math.min((anchorRect?.bottom || 100) + 8, window.innerHeight - 340);
  const left = Math.min(Math.max((anchorRect?.left || 100) - 80, 12), window.innerWidth - 280);

  const groups = [
    ["human", agents.filter((a) => a.kind === "human")],
    ["ai", agents.filter((a) => a.kind === "ai")],
    ["pipeline", agents.filter((a) => a.kind === "pipeline")],
  ];
  return (
    <div className="popover" ref={ref} style={{ top, left }}>
      <div className="popover-head">
        Who fills <b>{role?.name}</b>?
        <span className="popover-note">picking changes the role — every step using it re-resolves</span>
      </div>
      <div className="popover-body">
        {groups.map(([kind, list]) =>
          list.length ? (
            <div className="popover-group" key={kind}>
              <div className="popover-group-label">{KIND[kind].icon} {KIND[kind].label}</div>
              {list.map((a) => (
                <button key={a.id} className={"popover-opt " + (a.id === current ? "current" : "")} onClick={() => onPick(a.id)}>
                  <span className={"opt-dot " + KIND[kind].cls} />
                  {a.name}
                  {a.id === current && <span className="opt-check">current</span>}
                </button>
              ))}
            </div>
          ) : null
        )}
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
