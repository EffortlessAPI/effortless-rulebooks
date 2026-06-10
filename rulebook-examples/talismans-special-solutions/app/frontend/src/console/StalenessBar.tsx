import React, { useEffect, useRef, useState } from "react";
import { DagCell } from "../explainer-dag";
import type { Workflow } from "../types";

// ---- the staleness bar: compliance-doc review age vs. the review policy ----
// Models CQ5 directly: the docs go stale the instant the review age passes the
// policy line. There is no renewal window or grace period — staleness fires at
// the policy date itself:
//     IsStale = MonthsSinceModified > StalenessThresholdMonths
// The policy line IS the staleness boundary.
//
// EVERYTHING shown is read from already-derived / raw fields — monthsSinceModified,
// isStale and stalenessThresholdMonths all come from the reasoner/Postgres view.
// `previewStale` is ONLY a mid-drag echo of the rulebook's IsStale boundary so colors
// flip the instant the cursor crosses it; on release the substrate re-derives the real
// isStale and the bar reconciles to it.
//
// The review-age axis is FIXED at 0..18 months so the 12-month policy line always
// sits two-thirds across the bar (12/18 = 67%).
const REVIEW_AGE_MIN = 0;
const REVIEW_AGE_MAX = 18;
const REVIEW_AGE_SNAP = 1;   // slide in 1-month increments
const snapAge = (m: number): number =>
  Math.max(REVIEW_AGE_MIN, Math.min(REVIEW_AGE_MAX, Math.round(m / REVIEW_AGE_SNAP) * REVIEW_AGE_SNAP));
// Produce a Modified date that the substrate reads back as EXACTLY n whole months.
// The reasoner's formula (rules.shacl.ttl) is:
//   monthsSince = (YEAR(NOW)-YEAR(mod))*12 + (MONTH(NOW)-MONTH(mod))
//                 - IF(DAY(NOW) < DAY(mod), 1, 0)
// so the round-trip is lossless ONLY when DAY(mod) == DAY(NOW) (the IF term is 0).
// Two things matter:
//   1. Build the date from LOCAL Y/M/D — toISOString() converts to UTC and can
//      bump the day across a tz boundary, making DAY(mod) > DAY(NOW) and silently
//      subtracting a month (the "slides back 1 month" bug).
//   2. Keep the SAME day-of-month as today, clamped to the target month's length
//      so month-end days (29-31) can't overflow into the next month.
function monthsAgoISO(n: number): string {
  const today = new Date();
  const y = today.getFullYear();
  const m = today.getMonth() - n;            // may go negative; Date normalizes below
  const day = today.getDate();
  // last day of the target month — clamp `day` so e.g. Mar 31 → Feb 28, not Mar 3
  const lastDay = new Date(y, m + 1, 0).getDate();
  const d = new Date(y, m, Math.min(day, lastDay));
  const pad = (x: number) => String(x).padStart(2, "0");
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T00:00:00-05:00`;
}

interface StalenessBarProps {
  workflow: Workflow;
  busy: boolean;
  onSetModified: (isoDate: string) => void;
}

export function StalenessBar({ workflow, busy, onSetModified }: StalenessBarProps) {
  // The view is the contract: render EXACTLY what the substrate derived. The
  // staleness boundary is the policy line itself — no window to draw, no defaults
  // to invent. (CLAUDE.md: Avoid Silent Fallbacks — never paint a plausible value
  // over a real null.)
  const months = Number(workflow.monthsSinceModified ?? 0);
  const threshold = Number(workflow.stalenessThresholdMonths ?? 0);
  const trackRef = useRef<HTMLDivElement | null>(null);

  // While dragging the thumb we hold a LIVE preview age so the bar tracks the
  // cursor without re-reasoning; on release we commit ONE patch (Modified =
  // today − ageMonths) and the reasoner runs once.
  const [drag, setDrag] = useState<number | null>(null); // preview age in months | null
  // After release we keep the dropped value PINNED here through the whole
  // reasoning pass, so the thumb stays where it was dropped instead of snapping
  // back to the stale prop while the substrate re-derives. It's cleared once
  // reasoning finishes (see the effect below).
  const [pending, setPending] = useState<number | null>(null);
  // Precedence: live drag > committed-but-pending > the substrate's value.
  const ageMonths = drag != null ? drag : pending != null ? pending : months;

  // Reconcile: drop the pin once reasoning finishes (busy: true → false). The
  // freshly-reloaded prop is now authoritative — the view is the contract — so we
  // snap to whatever the substrate actually derived. (We can't gate on
  // months === pending: monthsAgoISO → DATETIME_DIFF can legitimately round to a
  // neighboring whole month, and a strict match would pin the thumb forever.)
  const wasBusy = useRef(busy);
  useEffect(() => {
    if (wasBusy.current && !busy) setPending(null); // reasoning just completed
    wasBusy.current = busy;
  }, [busy]);

  // Fixed 0..18 axis: the policy line lands at threshold/18 (12 → 67%).
  const pctOf = (m: number): number => Math.max(0, Math.min(m / REVIEW_AGE_MAX, 1)) * 100;
  const monthsPct = pctOf(ageMonths);
  const threshPct = pctOf(threshold);
  // The view is the contract: at REST, the STALE state comes straight from the
  // substrate's derived IsStale (workflow.isStale) — never recomputed in JS — so the
  // bar can never disagree with the verdict pill above. While the thumb is moved
  // (live drag OR committed-but-still-reasoning) we echo the computed boundary
  // (age > policy line) so the color tracks the thumb instead of snapping back to
  // the stale prop.
  const moved = drag != null || pending != null;
  const previewStale = moved ? ageMonths > threshold : !!workflow.isStale;
  // Past the policy line is STALE → red. Before it is green. (No intermediate
  // grace band — staleness fires the instant the review comes due.)
  const zone = previewStale ? "red" : "green";

  // Commit: pin the dropped value so the thumb stays put through reasoning, drop
  // the live-drag preview, and fire the single re-reason.
  const commitAge = (n: number) => {
    const snapped = snapAge(n);
    setPending(snapped);
    setDrag(null);
    onSetModified(monthsAgoISO(snapped));
  };

  return (
    <div className={"staleness " + zone}>
      <div className="st-head">
        <span className="st-title">📄 Docs review age</span>
        <span className="st-readout">
          {/* review age is the derived Workflows.MonthsSinceModified; the STALE
              flag is the derived Workflows.IsStale — wrap both so explainer mode
              can drill into how the reasoner computed each. */}
          last reviewed{" "}
          <DagCell table="Workflows" field="MonthsSinceModified"><b>{ageMonths}</b> mo ago</DagCell>
          {" "}· <b>{threshold}</b> mo policy{" "}
          <DagCell table="Workflows" field="IsStale">
            {previewStale ? <span className="st-flag"> · STALE → feeds AT RISK</span>
                          : <span className="st-ok"> · current</span>}
          </DagCell>
        </span>
      </div>

      {/* the slider: a 0..18-month track. The thumb is the review age. The policy
          line is fixed at 12mo (≈two-thirds across) and IS the staleness boundary —
          IsStale flips the instant the thumb crosses it. */}
      <div className="st-slider" onClick={(e) => e.stopPropagation()}>
        <div
          ref={trackRef}
          className={"st-track " + zone}
          title={`${ageMonths} months since review; stale once age crosses the ${threshold}mo policy line`}
        >
          <div className={"st-fill " + zone} style={{ width: monthsPct + "%" }} />
          {/* policy line: where IsStale flips */}
          <div className="st-threshold" style={{ left: threshPct + "%" }}>
            <span className="st-threshold-label">{threshold}mo policy</span>
          </div>
          <input
            type="range"
            className="st-range"
            min={REVIEW_AGE_MIN}
            max={REVIEW_AGE_MAX}
            step={REVIEW_AGE_SNAP}
            value={ageMonths}
            disabled={busy}
            aria-label="Months since last review"
            // live preview while dragging (no re-reason); commit once on release
            onChange={(e) => setDrag(snapAge(Number(e.target.value)))}
            onMouseUp={(e) => { const n = snapAge(Number((e.target as HTMLInputElement).value)); if (n !== months) commitAge(n); else setDrag(null); }}
            onKeyUp={(e) => { const n = snapAge(Number((e.target as HTMLInputElement).value)); if (n !== months) commitAge(n); else setDrag(null); }}
            onBlur={() => setDrag(null)}
          />
          <div className="st-needle" style={{ left: monthsPct + "%" }}>
            <span className="st-needle-label">now ({ageMonths}mo)</span>
          </div>
        </div>
        <div className="st-scale">
          <span>0</span><span>now — drag the thumb for review age</span><span>{REVIEW_AGE_MAX}mo</span>
        </div>
      </div>

      <div className="st-ctls">
        <span className="st-ctlnote muted">
          Slide the thumb to set how long since the docs were reviewed. The{" "}
          <b>{threshold}-month policy</b> line is the staleness boundary: the docs go
          stale the instant the review age passes it.
        </span>
      </div>
    </div>
  );
}
