import React, { useEffect, useLayoutEffect, useRef, useState } from "react";
import { KIND } from "./model";
import type { Agent, AgentKind, ExecutingAgentType, RoleRow, Handlers, Situation } from "./types";

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

interface EscalationPopoverProps {
  roleId: string;
  sit: Situation;
  anchorRect?: DOMRect | null;
  onPick: (targetRoleId: string) => void;
  onClose: () => void;
}

// The escalation org-chart popup, anchored to a clicked EscalationViolation
// badge. It shows the delegation hierarchy as the reasoner has it (sit.orgTree)
// with the offending role shown DETACHED — its delegatesTo edge is blank, so the
// chain dead-ends and the gate it owns has nowhere to escalate. Picking a
// next-hop role PATCHes Roles.delegatesTo; the substrate then re-closes the
// chain and EscalationViolation / CQ6 re-answer themselves. Nothing here
// computes the violation — it reads the derived `escalationViolation` column and
// the already-reasoned delegation reach (sit.delegation).
export function EscalationPopover({ roleId, sit, anchorRect, onPick, onClose }: EscalationPopoverProps) {
  const ref = useRef<HTMLDivElement | null>(null);
  useEffect(() => {
    const onDoc = (e: MouseEvent) => { if (ref.current && !ref.current.contains(e.target as Node)) onClose(); };
    const onEsc = (e: KeyboardEvent) => e.key === "Escape" && onClose();
    document.addEventListener("mousedown", onDoc);
    document.addEventListener("keydown", onEsc);
    return () => { document.removeEventListener("mousedown", onDoc); document.removeEventListener("keydown", onEsc); };
  }, [onClose]);

  const role = sit.roleById[roleId];
  // A candidate next hop is any role but self, excluding one whose escalation
  // already reaches this role (that pick would make a cycle). Roles that
  // themselves escalate upward (a non-empty reach) lead somewhere — list first.
  const reachesSelf = (from: string): boolean =>
    (sit.delegation[from]?.to || []).some((t) => t.id === roleId);
  const candidates = sit.roles
    .filter((r) => r.id !== roleId && !reachesSelf(r.id))
    .map((r) => ({ r, chain: sit.delegation[r.id]?.to || [] }))
    .sort((a, b) => b.chain.length - a.chain.length || a.r.name.localeCompare(b.r.name));

  // positioning mirrors ReassignPopover: prefer below the anchor, flip above if
  // it would overflow, never clip.
  const MARGIN = 12;
  const left = Math.min(Math.max((anchorRect?.left || 100) - 80, MARGIN), window.innerWidth - 340);
  const [top, setTop] = useState<number>((anchorRect?.bottom || 100) + 8);
  useLayoutEffect(() => {
    const h = ref.current?.offsetHeight ?? 0;
    const below = (anchorRect?.bottom || 100) + 8;
    const above = (anchorRect?.top || 100) - 8 - h;
    let next = below;
    if (below + h > window.innerHeight - MARGIN) next = above >= MARGIN ? above : Math.max(MARGIN, window.innerHeight - MARGIN - h);
    setTop(next);
  }, [anchorRect, roleId]);

  return (
    <div className="popover esc-popover" ref={ref} style={{ top, left }}>
      <div className="popover-head">
        ⚠ <b>{role?.name || roleId}</b> has no one to escalate to
      </div>
      <div className="popover-guide" role="note">
        This role owns an <b>approval gate</b>. When the gate stalls past its
        escalation window, authority must flow <b>up the delegation chain</b> — but
        the chain dead-ends here. Pick who it escalates to:
      </div>

      <div className="esc-orgchart">
        <div className="esc-orgchart-label">Escalation hierarchy</div>
        {sit.orgTree.map((chain, ci) => (
          <div className="esc-chain" key={ci}>
            {chain.map((n, i) => (
              <React.Fragment key={n.id}>
                <span className={"esc-node" + (n.id === roleId ? " broken" : "")}>{n.name}</span>
                {i < chain.length - 1 && <span className="esc-up">↑</span>}
              </React.Fragment>
            ))}
          </div>
        ))}
        {/* the offending role, shown detached: its edge was cut */}
        <div className="esc-chain broken-chain">
          <span className="esc-node broken">{role?.name}</span>
          <span className="esc-up dashed">↑ ?</span>
          <span className="esc-node ghost">— no target —</span>
        </div>
      </div>

      <div className="popover-body">
        <div className="popover-group-label">Delegate to</div>
        {candidates.map(({ r, chain }) => (
          <button key={r.id} className="popover-opt esc-opt" onClick={() => onPick(r.id)}>
            <span className="opt-dot k-role" />
            <span className="esc-opt-name">{r.name}</span>
            {chain.length > 0
              ? <span className="esc-leads">↑ {chain.map((t) => t.name).join(" → ")}</span>
              : <span className="esc-leads top">top of chain</span>}
          </button>
        ))}
      </div>
      <div className="popover-foot">
        Sets <code>{role?.name} → delegatesTo</code>; the reasoner re-closes the chain and CQ6 re-answers.
      </div>
    </div>
  );
}
