import React, { useState } from "react";
import { KIND } from "../model";
import { AgentAvatar } from "../Editable";
import type { Situation, Handlers, Agent, Step, RoleRow, OrgNode } from "../types";

// ===========================================================================
// ORG VIEW — accountability. Two panels of the same reasoned model:
//   left  : the delegation hierarchy (who escalates to whom) — a chain the
//           reasoner completed (Release Mgr → VP → CTO), inferred links marked.
//   right : the role → agent assignment grid. Each row is a role (editable
//           avatar to reassign), and the steps that role owns light up. Hover a
//           role to highlight its steps in the step rail.
// ===========================================================================

interface OrgViewProps {
  sit: Situation;
  handlers?: Handlers;
}

export function OrgView({ sit, handlers }: OrgViewProps) {
  const [hoverRole, setHoverRole] = useState<string | null>(null);

  // role -> the steps it owns (by position)
  const stepsByRole: Record<string, number[]> = {};
  for (const s of sit.steps) (stepsByRole[s.roleId] ||= []).push(s.position);

  return (
    <div className="org">
      <div className="org-tree">
        <div className="panel-label">Escalation hierarchy</div>
        {sit.orgTree.map((chain: OrgNode[], ci: number) => (
          <div className="chain" key={ci}>
            {chain.map((node: OrgNode, i: number) => (
              <React.Fragment key={node.id}>
                <div
                  className={"org-node " + (hoverRole === node.id ? "hot" : "")}
                  onMouseEnter={() => setHoverRole(node.id)}
                  onMouseLeave={() => setHoverRole(null)}
                >
                  <AgentAvatar
                    agent={fillerAgent(sit, node.id)}
                    roleId={node.id}
                    onReassign={handlers!.openReassign}
                    size={30}
                  />
                  <div className="org-node-text">
                    <div className="org-role">{node.name}</div>
                    <div className="org-agent muted">{fillerAgent(sit, node.id)?.name || "unassigned"}</div>
                  </div>
                  {sit.roleById[node.id]?.escalationViolation && (
                    <button
                      className="org-escalation-broken"
                      onClick={(e) => handlers!.openEscalation(node.id, e.currentTarget.getBoundingClientRect())}
                      title="This role owns a gate but has no escalation target — open the org chart to fix it"
                    >⚠ no escalation</button>
                  )}
                </div>
                {i < chain.length - 1 && (
                  <button
                    type="button"
                    className="org-up org-up-edit"
                    onClick={(e) => handlers!.openEscalation(node.id, e.currentTarget.getBoundingClientRect())}
                    title={`Change who ${node.name} escalates to`}
                  >
                    ↑ escalates to <span className="org-up-pen">✎</span>
                  </button>
                )}
              </React.Fragment>
            ))}
          </div>
        ))}
        <p className="org-note muted">
          Only adjacent links were written down; the reasoner closes the chain so the bottom role can reach the top.
        </p>
      </div>

      <div className="org-grid">
        <div className="panel-label">Role → who fills it → steps they own</div>
        <table className="assign">
          <thead>
            <tr><th>Role</th><th>Filled by</th><th>Owns steps</th></tr>
          </thead>
          <tbody>
            {sit.roles.map((r: RoleRow) => {
              const owns = stepsByRole[r.id] || [];
              if (!owns.length && !isLeadership(r.id)) {
                // still show, but mark as not-on-this-workflow
              }
              return (
                <tr
                  key={r.id}
                  className={hoverRole === r.id ? "hot" : ""}
                  onMouseEnter={() => setHoverRole(r.id)}
                  onMouseLeave={() => setHoverRole(null)}
                >
                  <td className="role-cell">{r.name}</td>
                  <td className="filler-cell">
                    <AgentAvatar
                      agent={fillerAgent(sit, r.id)}
                      roleId={r.id}
                      onReassign={handlers!.openReassign}
                      size={28}
                      showName
                    />
                  </td>
                  <td className="steps-cell">
                    {owns.length ? owns.map((p: number) => <span className="step-chip" key={p}>#{p}</span>)
                      : <span className="muted">— none —</span>}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      {/* the step rail across the bottom, lighting up the hovered role's steps */}
      <div className="org-steprail">
        {sit.steps.map((s: Step) => (
          <div
            key={s.id}
            className={
              "rail-step k-" + (s.executingAgentType === "AIAgent" ? "ai" : s.executingAgentType === "AutomatedPipeline" ? "pipeline" : "human") +
              (hoverRole && s.roleId === hoverRole ? " lit" : "") +
              (hoverRole && s.roleId !== hoverRole ? " dim" : "")
            }
          >
            <span className="rail-pos">{s.position}</span>
            <span className="rail-title">{s.title}</span>
          </div>
        ))}
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
function isLeadership(roleId: string): boolean {
  return roleId.includes("vp-") || roleId.includes("cto");
}
