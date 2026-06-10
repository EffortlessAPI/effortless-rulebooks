import React, { useEffect, useRef, useState } from "react";
import { KIND, kindOfType } from "../model";
import { DagCell } from "../explainer-dag";
import type { Situation, Workflow, Verdict, Handlers, AgentKind } from "../types";

// ---- the time-budget bar: the live total vs. the FIXED 4-hour budget -------
// The fill is COMPOSED of one colored segment per step, so you can see exactly
// how each step's duration builds the plan runtime. Each segment is RESIZABLE:
// drag the splitter on its right edge to change that step's duration right where
// you see its effect (no separate buttons). Green/yellow/red zoning is conveyed
// by the budget marker + overflow sliver, not the segment colors.
// The budget is a FIXED raw fact (MaxPlanMinutes = 240 / 4h, seeded in the
// rulebook); it is not user-adjustable. The plan fits or breaches by resizing a
// step's segment, not by moving the budget.
const DUR_SNAP = 15;   // drag snaps to 15-minute steps
const DUR_MIN = 15;    // a step can't go below one snap
const DUR_MAX = 240;   // a single step can't exceed the whole 4h budget
const snapDur = (m: number): number =>
  Math.max(DUR_MIN, Math.min(DUR_MAX, Math.round(m / DUR_SNAP) * DUR_SNAP));

interface Seg {
  id: string;
  pos: number;
  title: string;
  min: number;
  kind: AgentKind;
}

interface TimeBudgetBarProps {
  sit: Situation;
  workflow: Workflow;
  verdict: Verdict;
  handlers: Handlers;
  busy: boolean;
}

export function TimeBudgetBar({ sit, workflow, verdict, handlers, busy }: TimeBudgetBarProps) {
  const steps = sit.steps || [];
  const facts = sit.stepFacts || {};
  // Per-step minutes, read from the already-derived facts (the view is the
  // contract — we never recompute a duration here, only lay them side by side).
  // Each segment is colored by who runs the step (blue person / purple AI /
  // amber pipeline), mirroring the step cards, and labeled "Step #N".
  const segs: Seg[] = steps.map((s, i) => ({
    id: s.id,
    pos: s.position ?? i + 1,
    title: s.title,
    min: Number(facts[s.id]?.stepDurationMinutes ?? s.durationMinutes ?? 0),
    kind: kindOfType(s.executingAgentType),
  }));
  const budget = Number(workflow.maxPlanMinutes ?? verdict.timeBudgetMinutes ?? 0);
  const over = !!(workflow.isOverTimeBudget ?? verdict.isOverTimeBudget);

  const trackRef = useRef<HTMLDivElement | null>(null);
  // While dragging a splitter we hold a LIVE preview value for the dragged step
  // so the segment widths track the cursor without re-reasoning. On release we
  // commit ONE patch (one re-reason); the doctrine of "no mid-drag stuck slider"
  // is preserved — the reasoner only runs on the committed value.
  const [drag, setDrag] = useState<{ id: string; min: number } | null>(null);
  // After release we PIN the resized step's value here through the whole reasoning
  // pass, so its segment stays where it was dropped instead of snapping back to
  // the stale fact while the substrate re-derives. Cleared once reasoning finishes.
  const [pending, setPending] = useState<{ id: string; min: number } | null>(null);

  // Reconcile: drop the pin once reasoning finishes (busy: true → false). The
  // freshly-reloaded facts are authoritative — the view is the contract.
  const wasBusy = useRef(busy);
  useEffect(() => {
    if (wasBusy.current && !busy) setPending(null); // reasoning just completed
    wasBusy.current = busy;
  }, [busy]);

  // Effective minutes for layout: live drag > committed-but-pending > the
  // substrate's committed value.
  const effMin = (s: Seg): number =>
    drag && drag.id === s.id ? drag.min
    : pending && pending.id === s.id ? pending.min
    : s.min;
  const previewTotal = segs.reduce((a, s) => a + effMin(s), 0);
  // FIXED axis: budget * 1.5 pins the budget marker at exactly two-thirds across
  // (budget / (budget*1.5) = 0.667), so "where it goes over budget" is a stable
  // landmark instead of sliding with the plan — same trick as the 12mo policy
  // line on the staleness slider. The axis only grows past that if the plan
  // itself exceeds 150% of budget (an extreme drag), so the segments stay honest.
  const axisMax = Math.max(budget * 1.5, previewTotal, 1);
  const pctOf = (m: number): number => (m / axisMax) * 100;
  const budgetPct = pctOf(budget);
  const previewOver = previewTotal > budget;
  // The thumb is "moved" during a live drag OR while a committed resize is still
  // reasoning; in both cases the OVER flag echoes previewTotal so it tracks the
  // pinned segment instead of the stale prop, then reconciles when reasoning ends.
  const moved = drag != null || pending != null;
  const zone = (moved ? previewOver : over) ? "red"
    : previewTotal / Math.max(budget, 1) >= 0.8 ? "yellow" : "green";

  // Drag math: convert a pixel delta on the track into a minute delta using the
  // track's pixel width and the current axis scale, then snap to 15 min.
  function startDrag(seg: Seg, e: React.MouseEvent) {
    if (busy) return;             // reasoner is locked — don't start a resize
    e.preventDefault();
    const track = trackRef.current;
    if (!track) return;
    const trackPx = track.getBoundingClientRect().width;
    const minPerPx = axisMax / trackPx;
    const startX = e.clientX;
    const startMin = seg.min;

    const onMove = (ev: MouseEvent) => {
      const dxMin = (ev.clientX - startX) * minPerPx;
      setDrag({ id: seg.id, min: snapDur(startMin + dxMin) });
    };
    const onUp = (ev: MouseEvent) => {
      window.removeEventListener("mousemove", onMove);
      window.removeEventListener("mouseup", onUp);
      const dxMin = (ev.clientX - startX) * minPerPx;
      const next = snapDur(startMin + dxMin);
      setDrag(null);
      if (next !== startMin) {
        // pin the dropped value so the segment stays put through reasoning
        setPending({ id: seg.id, min: next });
        handlers.patchStep(seg.id, { stepDurationMinutes: next });
      }
    };
    window.addEventListener("mousemove", onMove);
    window.addEventListener("mouseup", onUp);
    setDrag({ id: seg.id, min: startMin });
  }

  return (
    <div className={"timebudget " + zone + (drag ? " dragging" : "")}>
      <div className="tb-head">
        <span className="tb-title">⏱ Plan runtime</span>
        <span className="tb-readout">
          {/* the plan total is the derived Workflows.CountTotalPlanMinutes; the OVER
              flag is the derived Workflows.IsOverTimeBudget — wrap both so explainer
              mode can drill into the rollup and the boolean it feeds. */}
          <DagCell table="Workflows" field="CountTotalPlanMinutes"><b>{previewTotal}</b> min</DagCell>
          {" "}of <b>{budget}</b> min budget{" "}
          <DagCell table="Workflows" field="IsOverTimeBudget">
            {(moved ? previewOver : over)
              ? <span className="tb-flag"> · OVER by {previewTotal - budget} min → AT RISK</span>
              : <span className="tb-ok"> · within budget</span>}
          </DagCell>
          {drag && <span className="tb-dragnote"> · release to recompute</span>}
        </span>
        <span className="tb-budgetnote muted">budget is fixed at 4h — drag a step's segment edge to resize it (snaps to 15 min); the plan fits or breaches</span>
      </div>

      <div className="tb-track" ref={trackRef}>
        {/* one segment per step, widths proportional to its minutes, colored by
            who runs it (mirrors the step cards). Each segment now carries its
            whole identity inline — a large agent-type icon (human / AI / pipeline),
            the step #, and its minutes — so there's no separate legend row. The
            splitter on each segment's right edge resizes that step. */}
        {segs.map((s) => {
          const m = effMin(s);
          if (m <= 0) return null;
          const isDragged = drag && drag.id === s.id;
          const meta = KIND[s.kind];
          return (
            <div
              key={s.id}
              className={"tb-seg k-" + s.kind + (isDragged ? " dragged" : "")}
              style={{ width: pctOf(m) + "%" }}
              title={`Step #${s.pos}: ${s.title} — ${m} min (${meta.label}) · drag the edge to resize`}
            >
              {/* the big ¼" agent-type glyph: who runs this step */}
              <span className="tb-seg-icon" role="img" aria-label={meta.label}>{meta.icon}</span>
              <span className="tb-seg-meta">
                <span className="tb-seg-pos">Step #{s.pos}</span>
                <span className="tb-seg-min">{m}m</span>
              </span>
              {/* the splitter handle — drag to change this step's duration */}
              <span
                className={"tb-split" + (busy ? " disabled" : "")}
                onMouseDown={(e) => startDrag(s, e)}
                title={busy ? "reasoning…" : `resize Step #${s.pos} (15-min steps)`}
                role="separator"
                aria-label={`resize Step #${s.pos} duration`}
              />
            </div>
          );
        })}
        {/* the budget marker — everything to its right is over budget */}
        <div className={"tb-limit " + ((moved ? previewOver : over) ? "breached" : "")} style={{ left: budgetPct + "%" }} title={`budget: ${budget} min`}>
          <span className="tb-limit-label">{budget}m budget</span>
        </div>
      </div>
    </div>
  );
}
