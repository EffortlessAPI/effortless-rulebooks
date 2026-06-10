import React from "react";
import { DagCell, DagToggle } from "../explainer-dag";
import type { Verdict, Workflow } from "../types";

// ---- the verdict, always visible ------------------------------------------
// THREE inputs decide the verdict, shown as one readable sentence:
//   (docs stale  AND  AI runs a step)   OR   (plan over its time budget)
// Each pill is lit only when that condition is currently true; the pills that
// are actually FORCING the current verdict get a "firing" highlight.
//
// This is a FIXED card — it rides at the right end of the top summary strip,
// next to the Plan-runtime bar, sized to hold the whole AND/OR statement. It is
// no longer draggable. Directly under the verdict sits the "Try a scenario"
// button that opens the floating picker — keeping the presets right next to the
// status they change.

interface VerdictHeaderProps {
  verdict: Verdict;
  workflow: Workflow;
  hasScenarios: boolean;
  busy: boolean;
  onOpenScenarios: () => void;
}

export function VerdictHeader({ verdict, workflow, hasScenarios, busy, onOpenScenarios }: VerdictHeaderProps) {
  const risk = verdict.isAtComplianceRisk;
  const stale = verdict.isStale;
  const ai = verdict.hasAIExecutedStep;
  const over = verdict.isOverTimeBudget;
  const broken = verdict.hasConsistencyViolation;
  const staleAndAi = stale && ai;
  return (
    <div className={"verdict-header " + (risk ? "risk" : "ok")}>
      <div className="vh-drag">
        <div className="vh-badge">{risk ? "⚠ AT COMPLIANCE RISK" : "✓ COMPLIANT"}</div>
      </div>
      <div className="vh-detail">
        <b>{workflow.title}</b> — {whyText(verdict)}
      </div>
      {/* Each pill is a derived Workflows boolean — wrap it so explainer mode can
          drill into exactly how the rulebook computed it. The verdict equals
          IsStaleAndHasAIAgent OR IsOverTimeBudget OR HasConsistencyViolation. */}
      {/* Stacked boolean tree — one clause per line, so the structure
          ( stale AND ai ) OR over-budget OR rule-broken → verdict reads at a glance. */}
      <div className="vh-rule">
        <span className={"vh-clause rule-group " + (staleAndAi ? "firing" : "")}>
          <DagCell table="Workflows" field="IsStale"><RulePill on={stale} label="docs stale" /></DagCell>
          <span className="vh-and">AND</span>
          <DagCell table="Workflows" field="HasAIAgentStep"><RulePill on={ai} label="AI runs a step" /></DagCell>
        </span>
        <span className="vh-clause">
          <span className="vh-or">OR</span>
          <DagCell table="Workflows" field="IsOverTimeBudget"><RulePill on={over} label="over time budget" firing={over} /></DagCell>
        </span>
        {/* the implicit "no broken rules" input — a consistency violation alone
            forces AT RISK, so COMPLIANT can never sit beside a broken rule */}
        <span className="vh-clause">
          <span className="vh-or">OR</span>
          <DagCell table="Workflows" field="HasConsistencyViolation"><RulePill on={broken} label="a rule is broken" firing={broken} /></DagCell>
        </span>
        <span className="vh-eq">→ {risk ? "AT RISK" : "ok"}</span>
      </div>
      {/* The provenance/explainer toggle lives here, in the verdict box — turn it
          on and every pill above (and every calculated value on the board)
          becomes clickable, drilling into how the rulebook derived it. */}
      <div className="vh-actions">
        <DagToggle />
        {hasScenarios && (
          <button className="vh-scenario-btn" disabled={busy} onClick={onOpenScenarios}>
            ✦ Try a scenario…
          </button>
        )}
      </div>
    </div>
  );
}

// Plain-language explanation of the CURRENT verdict — names exactly which
// condition(s) are forcing it, so the "why" is never a mystery.
function whyText(v: Verdict): string {
  const reasons: string[] = [];
  if (v.isStale && v.hasAIExecutedStep) reasons.push("docs are stale AND an AI runs a step");
  if (v.isOverTimeBudget)
    reasons.push(`the plan runs ${v.totalPlanMinutes} min, over its ${v.timeBudgetMinutes} min budget`);
  if (v.hasConsistencyViolation) {
    const n = v.consistencyViolationCount || 0;
    reasons.push(`${n} step${n === 1 ? "" : "s"} break the human-sign-off rule (an approval step isn't human-filled)`);
  }
  if (reasons.length) return "at risk because " + reasons.join("; and ");
  // compliant — explain what's keeping it safe (all three inputs are clear)
  if (v.hasAIExecutedStep && !v.isStale)
    return "an AI runs a step, but the docs are fresh, the plan is within budget, and no rule is broken — compliant";
  return "no stale-plus-AI risk, within the time budget, and no broken rules — compliant";
}

interface RulePillProps {
  on: boolean;
  label: string;
  firing?: boolean;
}

function RulePill({ on, label, firing }: RulePillProps) {
  return (
    <span className={"rule-pill " + (on ? "on" : "off") + (firing ? " firing" : "")}>
      {on ? "✓" : "○"} {label}
    </span>
  );
}
