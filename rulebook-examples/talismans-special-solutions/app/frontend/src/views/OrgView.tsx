import React, { useState } from "react";
import { KIND } from "../model";
import { AgentAvatar } from "../Editable";
import type { Situation, Handlers, Agent, Step, RoleRow } from "../types";

// ===========================================================================
// ORG VIEW — one coherent org chart for a release.
//
// This replaces the old two-disconnected-panels layout (a randomly-ordered
// table next to a chain that vanished when you cut the last edge). It is ONE
// canvas, and every role on it is a card you can fully assign:
//
//   • escalation chain   — roles are stacked in tiers, apex (escalates to no
//                          one) on top, leaves below, exactly like an org chart.
//                          Connectors are clickable to set / change / clear who
//                          a role escalates to. EVERY card always carries an
//                          escalation control, so you can never get stuck with
//                          "the panel disappeared and there's no way to add one."
//   • who fills the role — the avatar is editable (reassign human / AI / pipeline).
//   • what the role owns — the steps it owns are chips on the card; selecting a
//                          role lights its steps in the rail below and its
//                          escalation path above, so an edit's CONSEQUENCE is
//                          visible in one eyeful.
//   • step ownership     — click a step in the rail to move it to a different
//                          role (writes WorkflowSteps.assignedRole); the
//                          substrate then recomputes who executes it.
//
// Ordering is DETERMINISTIC (escalation depth, then name) so nothing jumps when
// the story reloads after an edit. The app computes no business facts — it reads
// the reasoned delegation reach (sit.delegation) and arranges it.
// ===========================================================================

interface OrgViewProps {
  sit: Situation;
  handlers?: Handlers;
}

interface StepMenu {
  stepId: string;
  anchorRect: DOMRect;
}

export function OrgView({ sit, handlers }: OrgViewProps) {
  // Pinned selection (click) wins over transient hover, so the highlight stays
  // put while you read it — the old version only had a flickery hover.
  const [sel, setSel] = useState<string | null>(null);
  const [hover, setHover] = useState<string | null>(null);
  const [stepMenu, setStepMenu] = useState<StepMenu | null>(null);
  const active = hover || sel;

  // role -> the steps it owns (in workflow order)
  const stepsByRole: Record<string, Step[]> = {};
  for (const s of sit.steps) (stepsByRole[s.roleId] ||= []).push(s);
  for (const k of Object.keys(stepsByRole)) stepsByRole[k].sort((a, b) => a.position - b.position);

  // Immediate escalation parent of a role: of everyone it can reach (the
  // reasoner's transitive delegatesTo), the one that itself reaches the most
  // others is the closest hop up the chain. (Same heuristic the closure uses.)
  const reach = (id: string): number => sit.delegation[id]?.to?.length || 0;
  const parentOf = (rid: string): string | null => {
    const targets = sit.delegation[rid]?.to || [];
    if (!targets.length) return null;
    return [...targets].sort((a, b) => reach(b.id) - reach(a.id))[0].id;
  };

  // Who escalates UP to a given role (its direct reports in org-chart terms).
  const hasReport: Record<string, boolean> = {};
  for (const r of sit.roles) { const p = parentOf(r.id); if (p) hasReport[p] = true; }

  // Depth from the apex: apex (no parent) = 0, each hop down +1. Drives the tiers.
  const depthMemo: Record<string, number> = {};
  const depthOf = (rid: string, seen = new Set<string>()): number => {
    if (rid in depthMemo) return depthMemo[rid];
    if (seen.has(rid)) return 0; // cycle guard
    seen.add(rid);
    const p = parentOf(rid);
    const d = p ? depthOf(p, seen) + 1 : 0;
    return (depthMemo[rid] = d);
  };

  // A role is part of the escalation graph if it escalates to someone or someone
  // escalates to it. Everything else is "unlinked" — own steps, no chain yet.
  const inChain = (r: RoleRow): boolean => !!parentOf(r.id) || !!hasReport[r.id];
  const chained = sit.roles.filter(inChain);
  const unlinked = sit.roles.filter((r) => !inChain(r));

  // Bucket chained roles into tiers by depth; sort each tier by name. Apex tier
  // (depth 0) renders first/top.
  const maxDepth = chained.reduce((m, r) => Math.max(m, depthOf(r.id)), 0);
  const tiers: RoleRow[][] = [];
  for (let d = 0; d <= maxDepth; d++) {
    const tier = chained
      .filter((r) => depthOf(r.id) === d)
      .sort((a, b) => a.name.localeCompare(b.name));
    if (tier.length) tiers.push(tier);
  }

  // The active role's escalation path (itself + every ancestor up to the apex),
  // so selecting a role lights the whole chain it would escalate through.
  const pathUp = new Set<string>();
  if (active) {
    let cur: string | null = active;
    const seen = new Set<string>();
    while (cur && !seen.has(cur)) { pathUp.add(cur); seen.add(cur); cur = parentOf(cur); }
  }

  const cardProps = (r: RoleRow) => ({
    onMouseEnter: () => setHover(r.id),
    onMouseLeave: () => setHover(null),
    onClick: () => setSel((s) => (s === r.id ? null : r.id)),
  });

  const renderCard = (r: RoleRow) => {
    const owns = stepsByRole[r.id] || [];
    const parentId = parentOf(r.id);
    const parent = parentId ? sit.roleById[parentId] : null;
    const isActive = active === r.id;
    const onPath = pathUp.has(r.id) && !isActive;
    const ownsGate = (r.fillsApprovalGate || 0) > 0;
    return (
      <div
        key={r.id}
        className={"ox-card" + (isActive ? " sel" : "") + (onPath ? " onpath" : "") + (ownsGate ? " gate" : "")}
        {...cardProps(r)}
      >
        <div className="ox-card-top">
          <AgentAvatar
            agent={fillerAgent(sit, r.id)}
            roleId={r.id}
            onReassign={handlers!.openReassign}
            size={34}
          />
          <div className="ox-card-id">
            <div className="ox-role">
              {r.name}
              {ownsGate && <span className="ox-gate-badge" title="owns an approval gate">⚖ gate</span>}
            </div>
            <div className="ox-filler muted">{fillerAgent(sit, r.id)?.name || "unassigned"}</div>
          </div>
        </div>

        <div className="ox-card-steps">
          {owns.length
            ? owns.map((s) => (
                <span key={s.id} className={"ox-stepchip k-" + agentClass(s)} title={s.title}>
                  #{s.position} {s.title}
                </span>
              ))
            : <span className="ox-nosteps muted">owns no steps</span>}
        </div>

        {/* escalation control — ALWAYS present, so a role is never un-editable */}
        <button
          type="button"
          className={"ox-esc" + (r.escalationViolation ? " broken" : parent ? " set" : " none")}
          onClick={(e) => { e.stopPropagation(); handlers!.openEscalation(r.id, e.currentTarget.getBoundingClientRect()); }}
          title={parent ? `Change who ${r.name} escalates to` : `Set who ${r.name} escalates to`}
        >
          {r.escalationViolation
            ? <>⚠ owns a gate but escalates to no one — fix</>
            : parent
              ? <>↑ escalates to <b>{parent.name}</b> <span className="ox-pen">✎</span></>
              : <>top of chain — <span className="ox-set">+ set escalation</span></>}
        </button>
      </div>
    );
  };

  return (
    <div className="orgx">
      <div className="ox-help muted">
        Every role is a card. <b>Click the avatar</b> to set who fills it · <b>click the escalation bar</b> to
        set who it escalates to · <b>click a step below</b> to move it to another role. Select a role to light its
        steps and the chain it escalates through.
      </div>

      <div className="ox-chart">
        {tiers.length === 0 && <div className="ox-empty muted">No escalation chain yet — wire one up from the roles below.</div>}
        {tiers.map((tier, ti) => (
          <React.Fragment key={ti}>
            {ti > 0 && <div className="ox-tier-gap" aria-hidden="true">↑</div>}
            <div className="ox-tier">{tier.map(renderCard)}</div>
          </React.Fragment>
        ))}
      </div>

      {unlinked.length > 0 && (
        <div className="ox-unlinked">
          <div className="ox-unlinked-label">
            Not in the escalation chain — own steps, but escalate to no one yet
          </div>
          <div className="ox-tier">{unlinked.map(renderCard)}</div>
        </div>
      )}

      {/* the step rail — every step, lit by the active role; click to reassign */}
      <div className="ox-rail">
        <div className="ox-rail-label muted">Release steps — click one to move it to another role</div>
        <div className="ox-rail-row">
          {sit.steps.map((s) => {
            const lit = active && s.roleId === active;
            const dim = active && s.roleId !== active;
            return (
              <button
                key={s.id}
                className={"ox-railstep k-" + agentClass(s) + (lit ? " lit" : "") + (dim ? " dim" : "")}
                onClick={(e) => setStepMenu({ stepId: s.id, anchorRect: e.currentTarget.getBoundingClientRect() })}
                onMouseEnter={() => setHover(s.roleId)}
                onMouseLeave={() => setHover(null)}
                title={`${s.title} — owned by ${s.role}; click to reassign`}
              >
                <span className="ox-rail-pos">{s.position}</span>
                <span className="ox-rail-body">
                  <span className="ox-rail-title">{s.title}</span>
                  <span className="ox-rail-meta muted">
                    {KIND[agentKind(s)].icon} {s.role}{s.agent ? ` · ${s.agent.name}` : ""}
                  </span>
                </span>
              </button>
            );
          })}
        </div>
      </div>

      {stepMenu && (
        <StepRoleMenu
          step={sit.steps.find((s) => s.id === stepMenu.stepId)!}
          roles={sit.roles}
          anchorRect={stepMenu.anchorRect}
          onPick={(roleId) => {
            setStepMenu(null);
            handlers!.patchStep(stepMenu.stepId, { assignedRole: roleId });
          }}
          onClose={() => setStepMenu(null)}
        />
      )}
    </div>
  );
}

// A compact role picker, anchored to the clicked rail step. Picking a role
// writes WorkflowSteps.assignedRole; the substrate recomputes the executor.
function StepRoleMenu({
  step, roles, anchorRect, onPick, onClose,
}: {
  step: Step;
  roles: RoleRow[];
  anchorRect: DOMRect;
  onPick: (roleId: string) => void;
  onClose: () => void;
}) {
  const ref = React.useRef<HTMLDivElement | null>(null);
  React.useEffect(() => {
    const onDoc = (e: MouseEvent) => { if (ref.current && !ref.current.contains(e.target as Node)) onClose(); };
    const onEsc = (e: KeyboardEvent) => e.key === "Escape" && onClose();
    document.addEventListener("mousedown", onDoc);
    document.addEventListener("keydown", onEsc);
    return () => { document.removeEventListener("mousedown", onDoc); document.removeEventListener("keydown", onEsc); };
  }, [onClose]);

  const MARGIN = 12;
  const top = Math.min((anchorRect.bottom || 100) + 8, window.innerHeight - 320);
  const left = Math.min(Math.max(anchorRect.left, MARGIN), window.innerWidth - 280);
  const sorted = [...roles].sort((a, b) => a.name.localeCompare(b.name));

  return (
    <div className="popover ox-rolemenu" ref={ref} style={{ top, left }}>
      <div className="popover-head">Who owns <b>{step.title}</b>?</div>
      <div className="popover-body">
        {sorted.map((r) => (
          <button
            key={r.id}
            className={"popover-opt" + (r.id === step.roleId ? " current" : "")}
            onClick={() => onPick(r.id)}
          >
            <span className="opt-dot k-role" />
            {r.name}
            {r.id === step.roleId && <span className="opt-check">current</span>}
          </button>
        ))}
      </div>
      <div className="popover-foot">
        Sets <code>{step.title} → assignedRole</code>; the substrate recomputes who executes it.
      </div>
    </div>
  );
}

function fillerAgent(sit: Situation, roleId: string): Agent | null {
  const r = sit.roleById[roleId];
  if (!r) return null;
  const id = r.filledByHumanAgent || r.filledByAIAgent || r.filledByAutomatedPipeline;
  return id ? sit.agentById[id] : null;
}
function agentKind(s: Step) {
  return s.executingAgentType === "AIAgent" ? "ai"
    : s.executingAgentType === "AutomatedPipeline" ? "pipeline"
    : "human";
}
function agentClass(s: Step): string {
  return agentKind(s);
}
