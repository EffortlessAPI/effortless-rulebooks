import React, { useMemo, useState } from "react";
import { DagCell } from "../explainer-dag";
import { resolveCq, type CqRow } from "../console/cqs";
import type { Situation, CompetencyQuestion } from "../types";

// ===========================================================================
// CQ VIEW — the Competency-Question scoreboard (the article's acceptance suite).
//
// The eight competency questions are FIRST-CLASS ROWS in the rulebook
// (`CompetencyQuestions`), so this lens is a projection of the model like every
// other lens — not a hardcoded list. Each card shows the question, the LIVE
// answer the substrate computed, and ✓/✗ against the rulebook's asserted
// expectation. The answer is wrapped in a <DagCell> pinned to the exact field
// that produces it (CompetencyQuestions.TargetTable/TargetField), so a click
// opens its full derivation in /dag. The header **"show only failing"** switch
// filters to the CQs whose live answer no longer satisfies the expectation —
// which an edit elsewhere in the console (reassign the gate, drag the staleness
// bar) can cause. Nothing here computes an answer; see cqs.ts.
// ===========================================================================

interface CQViewProps {
  sit: Situation;
}

export function CQView({ sit }: CQViewProps) {
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
    <div className="cqboard">
      <div className="cq-intro">
        <div className="cq-intro-lead">
          The article's acceptance suite — the eight leadership questions the model must answer.
          Each question is a row in the rulebook's <code>CompetencyQuestions</code> table; the
          answer below is read straight from the field that computes it, never recomputed here.
          Click any answer to open its derivation.
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
  onToggle: () => void;
}

function CqCard({ cq, answer, ok, rows, open, onToggle }: CqCardProps) {
  return (
    <div className={"cq-card " + (ok ? "ok" : "fail")}>
      <div className="cq-num">CQ{cq.number}</div>

      <div className="cq-main">
        <div className="cq-q">{cq.questionText}</div>

        <div className="cq-answer-row">
          {/* The live answer, pinned to the field that computed it → click for /dag. */}
          <DagCell table={cq.targetTable} field={cq.targetField}>
            <span className="cq-answer">{answer}</span>
          </DagCell>
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
