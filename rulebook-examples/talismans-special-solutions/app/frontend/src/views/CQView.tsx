import React, { useMemo, useState } from "react";
import { DagCell, DagToggle } from "../explainer-dag";
import { resolveCq, type CqRow } from "../console/cqs";
import type { Situation, CompetencyQuestion } from "../types";

// ===========================================================================
// CQ VIEW — the Competency-Question scoreboard (the article's acceptance suite).
//
// This is the RIGHT RAIL of the console: it rides beside the Flow/Graph/Closure
// lens at all times, so every edit on the left re-answers the leadership
// questions in place. The competency questions are FIRST-CLASS ROWS in the
// rulebook (`CompetencyQuestions`), so this panel is a projection of the model
// like every other lens — not a hardcoded list. Each card shows the question,
// the LIVE answer the substrate computed, and ✓/✗ against the rulebook's
// asserted expectation. The answer is wrapped in a <DagCell> pinned to the exact
// field that produces it (CompetencyQuestions.TargetTable/TargetField), so a
// click opens its full derivation in /dag. The header **"show only failing"**
// switch filters to the CQs whose live answer no longer satisfies the
// expectation — which an edit elsewhere in the console (reassign the gate, drag
// the staleness bar) can cause. The explainer toggle and the scenario picker
// (the controls that used to live in the old verdict box) live in this panel's
// action row. Nothing here computes an answer; see cqs.ts.
// ===========================================================================

interface CQViewProps {
  sit: Situation;
  hasScenarios?: boolean;
  busy?: boolean;
  // CQ ids that just changed answer from a Simulate click → flash their cards.
  flashed?: Set<string>;
  // Apply a CQ card's SimulateScenario (the minimal raw-fact edit that moves it).
  onSimulate?: (scenarioId: string) => void;
  onOpenScenarios?: () => void;
  // Open the escalation org-chart popup for the role with a broken escalation
  // (used by the CQ6 card when EscalationViolation is set).
  onFixEscalation?: (roleId: string, anchorRect: DOMRect) => void;
  // Re-attach a detached DCAT dataset to a consuming step (the CQ8 fix). Sets
  // the raw WorkflowSteps.ConsumesDataset FK; the substrate recomputes the
  // (derived) Datasets.ConsumedBySteps inverse and CQ8 re-answers itself.
  onReattachDataset?: (stepId: string, datasetId: string) => void;
}

export function CQView({ sit, hasScenarios = false, busy = false, flashed, onSimulate, onOpenScenarios, onFixEscalation, onReattachDataset }: CQViewProps) {
  // The role (if any) whose escalation is broken — the derived witness column,
  // read, not recomputed. Drives the CQ6 card's "fix on org chart" badge.
  const escalationRoleId = sit.roles.find((r) => r.escalationViolation)?.id;
  // The dataset (if any) consumed by NO step — what the CQ8 simulate leaves
  // behind. Its presence drives the CQ8 card's "re-attach" picker (the fix).
  const orphanDataset = sit.datasets.find((d) => d.consumedBySteps.length === 0);
  // The picker offers only PROPER consumers: steps whose role actually carries the
  // dataset-processing capability (cap-risk-analysis). You can't re-attach the risk
  // dataset to a step that can't process it (e.g. the deployment-health step) — that
  // would leave CQ8 red. Keeping the dataset on a proper step also means the detach
  // simulate (which clears that step's ConsumesDataset) always has a target.
  const DATASET_PROCESSING_CAP = "cap-risk-analysis";
  const reattachSteps = sit.steps
    .filter((s) => sit.roleById[s.roleId]?.hasCapability === DATASET_PROCESSING_CAP)
    .map((s) => ({ id: s.id, position: s.position, title: s.title }));
  const [onlyFailing, setOnlyFailing] = useState(false);
  const [open, setOpen] = useState<Record<string, boolean>>({});

  const results = useMemo(
    () =>
      [...sit.competencyQuestions]
        .sort((a, b) => (a.sortOrder ?? a.number) - (b.sortOrder ?? b.number))
        .map((cq) => ({ cq, res: resolveCq(sit, cq) })),
    [sit],
  );

  const passing = results.filter((r) => r.res.ok).length;
  const total = results.length;
  const allGreen = passing === total;
  const shown = onlyFailing ? results.filter((r) => !r.res.ok) : results;

  return (
    <div className="cqboard rail">
      <div className="cq-intro">
        <div className="cq-intro-lead">
          The article's acceptance suite — the leadership questions the model must answer. Each
          question is a row in the rulebook's <code>CompetencyQuestions</code> table; the answer
          below is read straight from the field that computes it, never recomputed here. Click any
          answer to open its derivation.
        </div>
        <div className="cq-intro-bar">
          <span className={"cq-score " + (allGreen ? "all" : "some")}>
            <b>{passing}</b>/{total} answered
          </span>
          <label className="cq-switch">
            <input
              type="checkbox"
              checked={onlyFailing}
              onChange={(e) => setOnlyFailing(e.target.checked)}
            />
            <span className="cq-switch-track"><span className="cq-switch-thumb" /></span>
            show only failing
          </label>
        </div>
        {/* The controls that used to sit in the verdict box: the explainer
            toggle (makes every computed value clickable into /dag) and the
            scenario picker (sets several raw facts, then re-reasons). */}
        <div className="cq-actions">
          <DagToggle />
          {hasScenarios && onOpenScenarios && (
            <button className="cq-scenario-btn" disabled={busy} onClick={onOpenScenarios}>
              ✦ Try a scenario…
            </button>
          )}
        </div>
      </div>

      <div className="cq-list">
        {shown.map(({ cq, res }) => (
          <CqCard
            key={cq.id}
            cq={cq}
            answer={res.answer}
            ok={res.ok}
            rows={res.rows}
            open={!!open[cq.id]}
            flash={!!flashed?.has(cq.id)}
            busy={busy}
            onSimulate={onSimulate}
            escalationRoleId={escalationRoleId}
            onFixEscalation={onFixEscalation}
            orphanDatasetId={orphanDataset?.id}
            orphanDatasetTitle={orphanDataset?.title}
            reattachSteps={reattachSteps}
            onReattachDataset={onReattachDataset}
            onToggle={() => setOpen((o) => ({ ...o, [cq.id]: !o[cq.id] }))}
          />
        ))}
        {shown.length === 0 && (
          <div className="cq-empty">Every competency question is answered — nothing failing.</div>
        )}
      </div>

      <p className="cq-foot muted">
        ✓ means the substrate's live answer matches the rulebook's asserted expectation. Edit a raw
        fact elsewhere in the console — reassign the approval gate, drag the staleness bar past the
        threshold — and watch a CQ flip to ✗ here. The questions live in the rulebook; the answers
        come from the same engine the rest of the console reads.
      </p>
    </div>
  );
}

interface CqCardProps {
  cq: CompetencyQuestion;
  answer: string;
  ok: boolean;
  rows: CqRow[];
  open: boolean;
  flash?: boolean;
  busy?: boolean;
  onSimulate?: (scenarioId: string) => void;
  // Set when SOME role has a broken escalation; only the cq-6 card uses it.
  escalationRoleId?: string;
  onFixEscalation?: (roleId: string, anchorRect: DOMRect) => void;
  // Set when a dataset is consumed by no step (CQ8 detached); only cq-8 uses it.
  orphanDatasetId?: string;
  orphanDatasetTitle?: string;
  reattachSteps?: { id: string; position: number; title: string }[];
  onReattachDataset?: (stepId: string, datasetId: string) => void;
  onToggle: () => void;
}

function CqCard({ cq, answer, ok, rows, open, flash = false, busy = false, onSimulate, escalationRoleId, onFixEscalation, orphanDatasetId, orphanDatasetTitle, reattachSteps, onReattachDataset, onToggle }: CqCardProps) {
  const canSimulate = !!cq.simulateScenario && !!onSimulate;
  // CQ6 is the escalation question; when a role's escalation is broken, offer to
  // open the org-chart popup right here and fix it.
  const canFixEscalation = cq.id === "cq-6" && !!escalationRoleId && !!onFixEscalation;
  // CQ8 is the dataset question; when the dataset is detached (consumed by no
  // step), offer to re-attach it to a step right here — the FIX, symmetric to
  // the simulate. Picking a step sets WorkflowSteps.ConsumesDataset.
  const canReattachDataset = cq.id === "cq-8" && !!orphanDatasetId && !!onReattachDataset;
  return (
    <div className={"cq-card " + (ok ? "ok" : "fail") + (flash ? " flash" : "")}>
      <div className="cq-num">CQ{cq.number}</div>

      <div className="cq-main">
        <div className="cq-q">{cq.questionText}</div>

        <div className="cq-answer-row">
          {/* The live answer, pinned to the field that computed it → click for /dag. */}
          <DagCell table={cq.targetTable} field={cq.targetField}>
            <span className="cq-answer">{answer}</span>
          </DagCell>
          {canSimulate && (
            <button
              className="cq-simulate"
              disabled={busy}
              onClick={() => onSimulate!(cq.simulateScenario!)}
              title="Apply the minimal raw-fact edit that moves this question's answer, then watch the substrate re-answer live"
            >
              ▶ simulate
            </button>
          )}
          {canFixEscalation && (
            <button
              className="cq-fix-escalation"
              disabled={busy}
              onClick={(e) => onFixEscalation!(escalationRoleId!, e.currentTarget.getBoundingClientRect())}
              title="A role that owns an approval gate has no one to escalate to — open the org chart and fix the chain"
            >
              ⚠ fix escalation
            </button>
          )}
          {canReattachDataset && (
            <select
              className="cq-reattach"
              disabled={busy}
              value=""
              onChange={(e) => e.target.value && onReattachDataset!(e.target.value, orphanDatasetId!)}
              title={`Re-attach “${orphanDatasetTitle}” to a step — sets WorkflowSteps.ConsumesDataset; the substrate re-derives ConsumedBySteps and CQ8 turns green`}
            >
              <option value="">📦 re-attach to…</option>
              {[...(reattachSteps || [])].sort((a, b) => a.position - b.position).map((s) => (
                <option key={s.id} value={s.id}>{s.position}. {s.title}</option>
              ))}
            </select>
          )}
          <button
            className="cq-disclose"
            onClick={onToggle}
            aria-expanded={open}
            title={open ? "hide the backing rows" : "show the backing rows"}
          >
            {open ? "−" : "+"} trace
          </button>
        </div>

        {open && (
          <div className="cq-detail">
            <div className="cq-expected">
              <span className="cq-expected-label">expected</span> {cq.expectedAnswer}
            </div>
            <ul className="cq-rows">
              {rows.map((r, i) => (
                <li key={i} className={"cq-row " + (r.kind ? "k-" + r.kind : "")}>
                  <span className="cq-row-label">{r.label}</span>
                  {r.sub && <span className="cq-row-sub">{r.sub}</span>}
                </li>
              ))}
            </ul>
            {cq.explanation && <div className="cq-why">{cq.explanation}</div>}
            <div className="cq-target muted">
              answered by <code>{cq.targetTable}.{cq.targetField}</code>
            </div>
          </div>
        )}
      </div>

      <div className={"cq-mark " + (ok ? "ok" : "fail")} title={ok ? "answered — matches expectation" : "does not match the asserted expectation"}>
        {ok ? "✓" : "✗"}
      </div>
    </div>
  );
}
