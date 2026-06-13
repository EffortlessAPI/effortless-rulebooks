import React, { useState } from "react";
import { KIND } from "../model";
import { AgentAvatar } from "../Editable";
import type { Situation, Handlers, Agent, Step, RoleRow } from "../types";

// ===========================================================================
// ORG VIEW — one coherent org chart, ordered by the WORKFLOW.
//
// The spine is the release itself: roles are listed in the order of the step
// they own (1, 2, 3, 4, 5). Where a role escalates, its escalation ladder
// branches off to the right (Release Manager → VP → CTO). Roles that own no
// steps (VP, CTO) are never their own row — they only appear as the ladder that
// hangs off the role that escalates to them. So the layout reads exactly like
// the workflow: 1, 2, 3→VP→CTO, 4, 5.
//
// Every role on the canvas is fully assignable:
//   • avatar           — click to set who fills the role (human / AI / pipeline)
//   • escalation edge  — the connector between two cards (and the trailing pill on
//                        the top of a ladder) edits who a role escalates to. There
//                        is exactly one editor per role's outgoing edge, and it is
//                        ALWAYS present — you can never get stuck unable to add one.
//   • step ownership   — click a step in the rail to move it to another role.
//
// Order is DETERMINISTIC (owned-step position, then name) so nothing jumps when
// the story reloads. The app computes no business facts — it reads the reasoned
// delegation reach (sit.delegation) and arranges it.
// ===========================================================================

interface OrgViewProps {
  sit: Situation;
  handlers?: Handlers;
}

interface ChainNode { id: string; ref?: boolean } // ref = a step-owner shown compactly (has its own row)
interface StepMenu { stepId: string; anchorRect: DOMRect }

export function OrgView({ sit, handlers }: OrgViewProps) {
  // Pinned selection (click) wins over transient hover so the highlight stays put.
  const [sel, setSel] = useState<string | null>(null);
  const [hover, setHover] = useState<string | null>(null);
  const [stepMenu, setStepMenu] = useState<StepMenu | null>(null);
  const active = hover || sel;

  // role -> the steps it owns (in workflow order)
  const stepsByRole: Record<string, Step[]> = {};
  for (const s of sit.steps) (stepsByRole[s.roleId] ||= []).push(s);
  for (const k of Object.keys(stepsByRole)) stepsByRole[k].sort((a, b) => a.position - b.position);
  const ownsSteps = (rid: string): boolean => (stepsByRole[rid]?.length || 0) > 0;
  const minStep = (rid: string): number => stepsByRole[rid]?.[0]?.position ?? Infinity;

  // Immediate escalation parent: of everyone a role can reach (the reasoner's
  // transitive delegatesTo), the one that itself reaches the most others is the
  // closest hop up. (Same heuristic the closure uses.)
  const reach = (id: string): number => sit.delegation[id]?.to?.length || 0;
  const parentOf = (rid: string): string | null => {
    const targets = sit.delegation[rid]?.to || [];
    if (!targets.length) return null;
    return [...targets].sort((a, b) => reach(b.id) - reach(a.id))[0].id;
  };

  // The escalation ladder starting at a role: itself, then up through ancestors.
  // Stop (and mark as a compact ref) if we reach another step-owner — it has its
  // own row, so we don't expand its ladder again here.
  const ladderFrom = (startId: string): ChainNode[] => {
    const out: ChainNode[] = [{ id: startId }];
    const seen = new Set([startId]);
    let cur = parentOf(startId);
    while (cur && !seen.has(cur)) {
      seen.add(cur);
      if (ownsSteps(cur)) { out.push({ id: cur, ref: true }); break; }
      out.push({ id: cur });
      cur = parentOf(cur);
    }
    return out;
  };

  // PRIMARY rows: every role that owns a step, ordered by that step. Each row is
  // the role + its escalation ladder.
  const primary = sit.roles
    .filter((r) => ownsSteps(r.id))
    .sort((a, b) => minStep(a.id) - minStep(b.id) || a.name.localeCompare(b.name));
  const rows = primary.map((r) => ladderFrom(r.id));

  // Roles that appear in some ladder (so we don't also list them as orphans).
  const inLadder = new Set<string>();
  rows.forEach((row) => row.forEach((n) => inLadder.add(n.id)));
  // ORPHANS: own no steps and not part of any ladder — render at the bottom so
  // they're still assignable / wireable, but they're not in the workflow yet.
  const orphans = sit.roles.filter((r) => !inLadder.has(r.id));

  // The active role's escalation path (itself + ancestors), to light the ladder.
  const pathUp = new Set<string>();
  if (active) {
    let cur: string | null = active;
    const seen = new Set<string>();
    while (cur && !seen.has(cur)) { pathUp.add(cur); seen.add(cur); cur = parentOf(cur); }
  }

  // ---- pieces ------------------------------------------------------------
  const roleCard = (rid: string) => {
    const r = sit.roleById[rid];
    if (!r) return null;
    const owns = stepsByRole[rid] || [];
    const isActive = active === rid;
    const onPath = pathUp.has(rid) && !isActive;
    const ownsGate = (r.fillsApprovalGate || 0) > 0;
    return (
      <div
        className={"ox-card" + (isActive ? " sel" : "") + (onPath ? " onpath" : "") + (ownsGate ? " gate" : "")}
        onMouseEnter={() => setHover(rid)}
        onMouseLeave={() => setHover(null)}
        onClick={() => setSel((s) => (s === rid ? null : rid))}
      >
        <div className="ox-card-top">
          <AgentAvatar agent={fillerAgent(sit, rid)} roleId={rid} onReassign={handlers!.openReassign} size={34} />
          <div className="ox-card-id">
            <div className="ox-role">
              {r.name}
              {ownsGate && <span className="ox-gate-badge" title="owns an approval gate">⚖ gate</span>}
            </div>
            <div className="ox-filler muted">{fillerAgent(sit, rid)?.name || "unassigned"}</div>
          </div>
        </div>
        <div className="ox-card-steps">
          {owns.length
            ? owns.map((s) => (
                <span key={s.id} className={"ox-stepchip k-" + agentKind(s)} title={s.title}>#{s.position} {s.title}</span>
              ))
            : <span className="ox-nosteps muted">owns no steps</span>}
        </div>
      </div>
    );
  };

  // A compact reference to a step-owner that lives in its own row.
  const refCard = (rid: string) => {
    const r = sit.roleById[rid];
    const pos = minStep(rid);
    return (
      <button
        className={"ox-ref" + (pathUp.has(rid) ? " onpath" : "")}
        onClick={() => setSel(rid)}
        title={`${r?.name} owns step #${pos} — its row is above`}
      >
        {r?.name} <span className="muted">· step #{pos} ↑</span>
      </button>
    );
  };

  // The editable escalation edge of `rid` (who it escalates to). Rendered as the
  // connector when there's a next card, or as a trailing pill at the top of a
  // ladder / on a role with no escalation set.
  const escEditor = (rid: string, parentId: string | null, asConnector: boolean) => {
    const r = sit.roleById[rid];
    const parent = parentId ? sit.roleById[parentId] : null;
    const cls = asConnector ? "ox-conn" : "ox-trail";
    const state = r?.escalationViolation ? " broken" : parent ? " set" : " none";
    return (
      <button
        type="button"
        className={cls + state}
        onClick={(e) => { e.stopPropagation(); handlers!.openEscalation(rid, e.currentTarget.getBoundingClientRect()); }}
        title={parent ? `Change who ${r?.name} escalates to` : `Set who ${r?.name} escalates to`}
      >
        {r?.escalationViolation
          ? <>⚠ no escalation — fix</>
          : parent
            ? <><span className="ox-arrow">↑ escalates to</span> <span className="ox-pen">✎</span></>
            : <><span className="ox-set">+ set escalation</span></>}
      </button>
    );
  };

  // Render one ladder row: card, edge, card, edge, … then a trailing editor on
  // the apex (so the chain can always be extended upward).
  const renderRow = (chain: ChainNode[], key: string | number) => (
    <div className="ox-row" key={key}>
      {chain.map((node, i) => {
        const next = chain[i + 1];
        return (
          <React.Fragment key={node.id}>
            {node.ref ? refCard(node.id) : roleCard(node.id)}
            {!node.ref && (next
              ? escEditor(node.id, next.id, true)
              : escEditor(node.id, null, false))}
          </React.Fragment>
        );
      })}
    </div>
  );

  return (
    <div className="orgx">
      <div className="ox-help muted">
        Roles are listed in <b>workflow order</b> — by the step each one owns. Where a role escalates, its ladder
        branches to the right. <b>Click the avatar</b> to set who fills a role · <b>click an escalation bar</b> to
        set who it escalates to · <b>click a step below</b> to move it to another role.
      </div>

      <div className="ox-rows">
        {rows.map((chain, i) => renderRow(chain, i))}
      </div>

      {orphans.length > 0 && (
        <div className="ox-orphans">
          <div className="ox-orphans-label">Not in the workflow yet — own no steps, escalate to no one</div>
          <div className="ox-rows">
            {orphans.map((r) => renderRow(ladderFrom(r.id), "orphan-" + r.id))}
          </div>
        </div>
      )}

      {/* the step rail — every step in workflow order; lit by the active role */}
      <div className="ox-rail">
        <div className="ox-rail-label muted">Release steps — click one to move it to another role</div>
        <div className="ox-rail-row">
          {sit.steps.map((s) => {
            const lit = active && s.roleId === active;
            const dim = active && s.roleId !== active;
            return (
              <button
                key={s.id}
                className={"ox-railstep k-" + agentKind(s) + (lit ? " lit" : "") + (dim ? " dim" : "")}
                onClick={(e) => setStepMenu({ stepId: s.id, anchorRect: e.currentTarget.getBoundingClientRect() })}
                onMouseEnter={() => setHover(s.roleId)}
                onMouseLeave={() => setHover(null)}
                title={`${s.title} — owned by ${s.role}; click to reassign`}
              >
                <span className="ox-rail-pos">{s.position}</span>
                <span className="ox-rail-body">
                  <span className="ox-rail-title">{s.title}</span>
                  <span className="ox-rail-meta muted">{KIND[agentKind(s)].icon} {s.role}{s.agent ? ` · ${s.agent.name}` : ""}</span>
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
          onPick={(roleId) => { setStepMenu(null); handlers!.patchStep(stepMenu.stepId, { assignedRole: roleId }); }}
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
