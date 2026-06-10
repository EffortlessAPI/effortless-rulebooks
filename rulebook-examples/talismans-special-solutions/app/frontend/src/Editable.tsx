import React, { useEffect, useLayoutEffect, useRef, useState } from "react";
import { KIND } from "./model";
import type { Agent, AgentKind, ExecutingAgentType, RoleRow, Handlers } from "./types";

// ===========================================================================
// Shared editable controls used by every view, so "what's editable" looks and
// behaves identically no matter which lens you're in.
// ===========================================================================

interface AgentAvatarProps {
  agent?: Agent | null;
  type?: ExecutingAgentType;
  roleId?: string;
  // matches Handlers.openReassign
  onReassign?: Handlers["openReassign"];
  size?: number;
  showName?: boolean;
  requiresHuman?: boolean;
}

// An agent avatar. If `roleId` is given it's EDITABLE: clicking opens the
// reassign popover (via onReassign). Shows the agent-kind icon front-and-center
// (the icon IS the thing being regulated) inside a type-colored ring.
// `requiresHuman` (the step's raw requiresHumanApproval fact) is forwarded to
// the reassign popover so it can tell the user a human is expected BEFORE they
// pick — and a 🔒 hint on the ring signals it at a glance.
export function AgentAvatar({ agent, type, roleId, onReassign, size = 34, showName = false, requiresHuman = false }: AgentAvatarProps) {
  const kind: AgentKind = agent?.kind || (type ? typeKind(type) : "unknown");
  const k = KIND[kind] || KIND.unknown;
  const editable = !!roleId && !!onReassign;
  const ref = useRef<HTMLButtonElement | null>(null);
  const onClick = (e: React.MouseEvent<HTMLButtonElement>) => {
    if (!editable) return;
    e.stopPropagation();
    onReassign!(roleId!, ref.current!.getBoundingClientRect(), requiresHuman);
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
function typeKind(t: ExecutingAgentType): AgentKind {
  return t === "AIAgent" ? "ai" : t === "AutomatedPipeline" ? "pipeline" : t === "HumanAgent" ? "human" : "unknown";
}

interface ReassignPopoverProps {
  role?: RoleRow | null;
  agents: Agent[];
  anchorRect?: DOMRect | null;
  requiresHuman?: boolean;
  onPick: (agentId: string) => void;
  onClose: () => void;
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
export function ReassignPopover({ role, agents, anchorRect, requiresHuman = false, onPick, onClose }: ReassignPopoverProps) {
  const ref = useRef<HTMLDivElement | null>(null);
  useEffect(() => {
    const onDoc = (e: MouseEvent) => { if (ref.current && !ref.current.contains(e.target as Node)) onClose(); };
    const onEsc = (e: KeyboardEvent) => e.key === "Escape" && onClose();
    document.addEventListener("mousedown", onDoc);
    document.addEventListener("keydown", onEsc);
    return () => { document.removeEventListener("mousedown", onDoc); document.removeEventListener("keydown", onEsc); };
  }, [onClose]);

  const current = role?.filledByHumanAgent || role?.filledByAIAgent || role?.filledByAutomatedPipeline || "";

  // The box grows to fit ALL agents (no internal scroll). Measure its real
  // height after render and place it so it stays fully on screen: below the
  // anchor when it fits, otherwise flipped above. No clamping into a fixed
  // budget, no scrolling — just enough room for the whole list.
  const MARGIN = 12;
  const left = Math.min(Math.max((anchorRect?.left || 100) - 80, MARGIN), window.innerWidth - 280);
  const [top, setTop] = useState<number>((anchorRect?.bottom || 100) + 8);
  useLayoutEffect(() => {
    const h = ref.current?.offsetHeight ?? 0;
    const below = (anchorRect?.bottom || 100) + 8;
    const above = (anchorRect?.top || 100) - 8 - h;
    // prefer below; if it would overflow the bottom, try flipping above; if
    // neither fits the full box, pin to whichever edge keeps the top visible.
    let next = below;
    if (below + h > window.innerHeight - MARGIN) {
      next = above >= MARGIN ? above : Math.max(MARGIN, window.innerHeight - MARGIN - h);
    }
    setTop(next);
  }, [anchorRect, role, agents, requiresHuman]);

  const groups: [AgentKind, Agent[]][] = [
    ["human", agents.filter((a) => a.kind === "human")],
    ["ai", agents.filter((a) => a.kind === "ai")],
    ["pipeline", agents.filter((a) => a.kind === "pipeline")],
  ];
  const conflictTip = "This step requires a human sign-off — assigning a non-human breaks the consistency rule (the reasoner flags it ⚠ rule broken).";
  return (
    <div className={"popover" + (requiresHuman ? " needs-human" : "")} ref={ref} style={{ top, left }}>
      <div className="popover-head">
        Who fills <b>{role?.name}</b>?
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

interface FactToggleProps {
  on?: boolean;
  label: string;
  onChange: (checked: boolean) => void;
  busy?: boolean;
}

// A small inline toggle for a boolean step fact (e.g. requiresHumanApproval).
export function FactToggle({ on, label, onChange, busy }: FactToggleProps) {
  return (
    <label className={"fact-toggle " + (on ? "on" : "")}>
      <input type="checkbox" checked={!!on} disabled={busy} onChange={(e: React.ChangeEvent<HTMLInputElement>) => onChange(e.target.checked)} />
      <span>{label}</span>
    </label>
  );
}
