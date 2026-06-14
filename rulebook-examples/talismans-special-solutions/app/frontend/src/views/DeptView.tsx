import React, { useState } from "react";
import { KIND } from "../model";
import { AgentAvatar } from "../Editable";
import { DagCell } from "../explainer-dag";
import type { Situation, Handlers, Agent, Step, RoleRow, Department } from "../types";

// ===========================================================================
// DEPT VIEW — the cross-department lens (answers CQ7).
//
// "Does the release involve BOTH Engineering and Legal?" The Flow/Org boards
// show WHO does each step; this one shows WHICH DEPARTMENT each step belongs to,
// and therefore makes the article's cross-cutting question concrete:
//
//   • what the departments ARE     — each Department (schema:Organization) is a
//                                     titled column, drawn from the Departments
//                                     table, not a bare FK id.
//   • which steps are Legal vs Eng  — every step is filed under the column of its
//                                     OWNING department and tinted that hue. A
//                                     step's department is NOT a raw fact: it is
//                                     the department of the ROLE it's assigned to
//                                     (Step → AssignedRole → Role.OwnedBy →
//                                     Department). We show that whole chain.
//   • how it's chosen / modified    — the editable fact is the role's department
//                                     chip (Roles.OwnedBy). Click it, pick another
//                                     department, and every step that role owns
//                                     moves columns — and CQ7 re-answers live.
//   • how it's flagged              — the header is the CQ7 verdict itself
//                                     (InvolvesEngineeringAndLegal), drillable via
//                                     the explainer DAG, plus the two input counts.
//
// DOCTRINE: the app computes NO ownership here. owningDepartment / isLegalOwned /
// isEngineeringOwned arrive pre-computed per step from the chosen substrate; the
// counts and the AND() verdict arrive on the workflow. We only group and tint.
// (Switch the backend to Postgres and a wrong lookup there shows up as steps in
// the "wrong" column — a visible conformance diff, not a hidden one.)
// ===========================================================================

interface DeptViewProps {
  sit: Situation;
  handlers?: Handlers;
}

interface DeptMenu {
  roleId: string;
  anchorRect: DOMRect;
}

// Stable, legible hues for the two named departments; anything else falls back to
// a neutral so an added department still renders (just without a bespoke color).
function deptHue(d: Department | null | undefined): string {
  const key = (d?.id || "") + " " + (d?.displayName || "") + " " + (d?.title || "");
  if (/legal/i.test(key)) return "#f0719b";        // rose = Legal
  if (/eng/i.test(key)) return "#3fb6a8";          // teal = Engineering
  return "var(--muted)";
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

export function DeptView({ sit, handlers }: DeptViewProps) {
  const [deptMenu, setDeptMenu] = useState<DeptMenu | null>(null);
  const [hover, setHover] = useState<string | null>(null); // hovered department id

  const w = sit.workflow;
  const both = !!w.involvesEngineeringAndLegal;

  // role -> the steps it owns (workflow order)
  const stepsByRole: Record<string, Step[]> = {};
  for (const s of sit.steps) (stepsByRole[s.roleId] ||= []).push(s);
  for (const k of Object.keys(stepsByRole)) stepsByRole[k].sort((a, b) => a.position - b.position);

  // Bucket roles under the department that owns them (Roles.OwnedBy). A role with
  // no ownedBy lands in a synthetic "unassigned" column so it's never hidden.
  const UNASSIGNED = "__none__";
  const cols: { dept: Department | null; id: string; roles: RoleRow[] }[] = [];
  const colIndex: Record<string, number> = {};
  for (const d of sit.departments) {
    colIndex[d.id] = cols.length;
    cols.push({ dept: d, id: d.id, roles: [] });
  }
  for (const r of sit.roles) {
    const did = r.ownedBy && colIndex[r.ownedBy] != null ? r.ownedBy : UNASSIGNED;
    if (colIndex[did] == null) { colIndex[did] = cols.length; cols.push({ dept: null, id: UNASSIGNED, roles: [] }); }
    cols[colIndex[did]].roles.push(r);
  }
  for (const c of cols) c.roles.sort((a, b) => a.name.localeCompare(b.name));

  // Per-column step count, lifted from the substrate's per-step booleans (never
  // recounted from the role list — the substrate is the oracle).
  const stepsInDept = (deptId: string): Step[] =>
    sit.steps.filter((s) => s.owningDepartment === deptId);

  const renderRole = (r: RoleRow, hue: string) => {
    const owns = stepsByRole[r.id] || [];
    const dept = r.ownedBy ? sit.departmentById[r.ownedBy] : null;
    return (
      <div key={r.id} className="dx-role">
        <div className="dx-role-top">
          <AgentAvatar agent={fillerAgent(sit, r.id)} roleId={r.id} onReassign={handlers!.openReassign} size={28} />
          <div className="dx-role-id">
            <div className="dx-role-name">{r.name}</div>
            {/* the editable department fact — Roles.OwnedBy */}
            <button
              type="button"
              className="dx-deptchip"
              style={{ borderColor: hue, color: hue }}
              onClick={(e) => { e.stopPropagation(); setDeptMenu({ roleId: r.id, anchorRect: e.currentTarget.getBoundingClientRect() }); }}
              title={`This role is owned by ${dept?.title || "no department"} — click to move it`}
            >
              🏛 {dept?.title || "no department"} <span className="dx-pen">✎</span>
            </button>
          </div>
        </div>
        <div className="dx-role-steps">
          {owns.length
            ? owns.map((s) => (
                <span
                  key={s.id}
                  className={"dx-stepchip k-" + agentKind(s)}
                  style={{ borderLeftColor: hue }}
                  title={`${s.title} — ${KIND[agentKind(s)].label}-executed; owned (via this role) by ${dept?.title || "—"}`}
                >
                  <span className="dx-step-pos">#{s.position}</span> {s.title}
                </span>
              ))
            : <span className="dx-nosteps muted">owns no steps</span>}
        </div>
      </div>
    );
  };

  return (
    <div className="deptx">
      {/* CQ7 verdict — the flag itself, drillable. */}
      <DagCell table="Workflows" field="InvolvesEngineeringAndLegal">
        <div className={"dx-verdict " + (both ? "yes" : "no")}>
          <div className="dx-verdict-q">Does this release involve <b>both Engineering and Legal</b>?</div>
          <div className="dx-verdict-a">
            {both ? "✓ Yes" : "✗ No"} —{" "}
            <span style={{ color: deptHue(sit.departmentById["ntwf-engineering"]) }}>
              {w.countEngineeringOwnedSteps} Engineering
            </span>{" "}+{" "}
            <span style={{ color: deptHue(sit.departmentById["ntwf-legal-dept"]) }}>
              {w.countLegalOwnedSteps} Legal
            </span>{" "}step(s)
          </div>
        </div>
      </DagCell>

      <div className="dx-help muted">
        A step's department is inherited: <b>Step → assigned Role → Role.OwnedBy → Department</b>.
        Click a role's <span className="dx-help-chip">department chip</span> to move it — every step it
        owns changes column and CQ7 re-answers. The department coloring and the counts come straight
        from the substrate (switch the backend on the login page to see if Postgres and the reasoner agree).
      </div>

      <div className="dx-cols">
        {cols.map((c) => {
          const hue = deptHue(c.dept);
          const stepCount = c.dept ? stepsInDept(c.dept.id).length : 0;
          const isHover = hover === c.id;
          return (
            <div
              key={c.id}
              className={"dx-col" + (isHover ? " hover" : "")}
              style={{ borderTopColor: hue }}
              onMouseEnter={() => setHover(c.id)}
              onMouseLeave={() => setHover(null)}
            >
              <div className="dx-col-head" style={{ color: hue }}>
                <span className="dx-col-dot" style={{ background: hue }} />
                <span className="dx-col-title">{c.dept ? c.dept.title : "No department"}</span>
                {c.dept && (
                  <span className="dx-col-count">
                    {stepCount} step{stepCount === 1 ? "" : "s"} · {c.roles.length} role{c.roles.length === 1 ? "" : "s"}
                  </span>
                )}
              </div>
              {c.dept?.displayName && (
                <div className="dx-col-sub muted">schema:Organization · <code>{c.dept.displayName}</code></div>
              )}
              <div className="dx-col-body">
                {c.roles.length
                  ? c.roles.map((r) => renderRole(r, hue))
                  : <div className="dx-col-empty muted">no roles owned by this department</div>}
              </div>
            </div>
          );
        })}
      </div>

      {deptMenu && (
        <DeptMenu
          role={sit.roleById[deptMenu.roleId]}
          departments={sit.departments}
          anchorRect={deptMenu.anchorRect}
          onPick={(deptId) => { setDeptMenu(null); handlers!.setOwnedBy(deptMenu.roleId, deptId); }}
          onClose={() => setDeptMenu(null)}
        />
      )}
    </div>
  );
}

// A compact department picker, anchored to the clicked chip. Picking writes
// Roles.OwnedBy; the substrate recomputes every owned step's department + CQ7.
function DeptMenu({
  role, departments, anchorRect, onPick, onClose,
}: {
  role: RoleRow;
  departments: Department[];
  anchorRect: DOMRect;
  onPick: (deptId: string) => void;
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
  const top = Math.min((anchorRect.bottom || 100) + 8, window.innerHeight - 260);
  const left = Math.min(Math.max(anchorRect.left, MARGIN), window.innerWidth - 280);
  const sorted = [...departments].sort((a, b) => a.title.localeCompare(b.title));

  return (
    <div className="popover dx-deptmenu" ref={ref} style={{ top, left }}>
      <div className="popover-head">Which department owns <b>{role.name}</b>?</div>
      <div className="popover-body">
        {sorted.map((d) => (
          <button
            key={d.id}
            className={"popover-opt" + (d.id === role.ownedBy ? " current" : "")}
            onClick={() => onPick(d.id)}
          >
            <span className="opt-dot" style={{ background: deptHue(d) }} />
            {d.title}
            {d.id === role.ownedBy && <span className="opt-check">current</span>}
          </button>
        ))}
      </div>
      <div className="popover-foot">
        Sets <code>{role.name} → OwnedBy</code>; the substrate recomputes each owned step's department and CQ7.
      </div>
    </div>
  );
}
