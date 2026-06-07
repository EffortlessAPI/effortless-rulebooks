// Hover-triggered micro page for a DagCell's field. Shows the SAME single
// dialect the full /dag/:table/:field page shows — RuleSpeak · English · Formula
// — following the same global narration choice (lib/dagPrefs), so the card and
// the page never disagree and a presenter sees exactly one explanation at a time.
//
// Sticky-hover behavior: the visible card is wrapped in a transparent "safety
// zone" so the mouse can travel ~15px between the trigger glyph and the card
// without dismissing. A short close-delay (220ms) on mouseleave gives the same
// pedagogical breathing room.

import { useMemo } from "react";
import type { ReactNode } from "react";
import type { DagResponse, FieldNode } from "../lib/types.ts";
import { humanizeField, renderEnglish } from "../lib/renderEnglish.ts";
import { tryParseFormula } from "../lib/formula.ts";
import {
  ruleSpeakForField,
  renderRuleRich,
  linkifyText,
} from "../lib/rulespeak.ts";
import type { RuleSpeakRef } from "../lib/rulespeak.ts";
import { useDocElements } from "../lib/dagPrefs.ts";
import { FormulaText } from "./FormulaText.tsx";
import { FieldChip } from "./FieldChip.tsx";
import { TypeBadge, typeTone } from "./TypeBadge.tsx";

interface Props {
  dag: DagResponse;
  onMouseEnter: () => void;
  onMouseLeave: () => void;
}

export function DagHoverCard({ dag, onMouseEnter, onMouseLeave }: Props): JSX.Element {
  const tone = typeTone(dag.type);
  // The hovercard is a compact 360px preview, so it shows ONE narration — the
  // first enabled in the gear (rulespeak → english → formula), defaulting to
  // RuleSpeak if all three are off. The roomy /dag page stacks all enabled ones.
  const docs = useDocElements();
  const mode: "rulespeak" | "english" | "formula" =
    docs.rulespeak ? "rulespeak" : docs.english ? "english" : docs.formula ? "formula" : "rulespeak";

  const ruleSpeak = useMemo(() => ruleSpeakForField(dag.table, dag.field), [dag.table, dag.field]);

  const upstreamLookup = useMemo(() => {
    const m: Record<string, FieldNode> = {};
    for (const n of dag.upstream) m[`${n.table}.${n.field}`] = n;
    m[`${dag.table}.${dag.field}`] = dag;
    return m;
  }, [dag]);

  const english = useMemo(() => {
    if (!dag.formula) return null;
    const ast = tryParseFormula(dag.formula);
    if (!ast) return null;
    try { return renderEnglish(ast); } catch { return null; }
  }, [dag.formula]);

  // The fields this rule references — drives clickable in-prose refs in the
  // RuleSpeak / English dialects (the formula dialect chips its own fields).
  // Same renderRef shape the full page uses, so a ref looks identical here.
  const refs = ruleSpeak?.refs ?? [];
  const renderRef = (r: RuleSpeakRef, matched: string, key: number) => (
    <FieldChip
      key={`ref-${key}`}
      table={r.table}
      field={r.field}
      node={upstreamLookup[`${r.table}.${r.field}`]}
      variant="inline"
      pageTable={dag.table}
    >
      {matched}
    </FieldChip>
  );

  return (
    <span
      className="dag-hovercard-shell"
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
    >
      <span className={`dag-hovercard dag-hovercard-${tone}`} role="dialog">
        <span className="dag-hovercard-head">
          <TypeBadge type={dag.type} size="sm" />
          <span className="dag-hovercard-where">
            <span className="dag-hovercard-table">{dag.table}</span>
            <span className="dag-hovercard-dot">·</span>
            <span className="dag-hovercard-field">{humanizeField(dag.field)}</span>
          </span>
        </span>

        {docs.desc && dag.description && (
          <span className="dag-hovercard-desc">{dag.description}</span>
        )}

        {dag.type === "raw" && (
          <span className="dag-hovercard-gt">
            Ground truth — written directly to the database
            {dag.datatype ? <> (datatype <code>{dag.datatype}</code>)</> : null}.
          </span>
        )}
        {dag.type === "relationship" && (
          <span className="dag-hovercard-gt">
            Relationship pointer — the link the rest of the DAG hangs off.
          </span>
        )}

        {/* The one dialect the global toggle selects — exactly one of RuleSpeak /
            English / Formula, never two at once, and ALWAYS the chosen one (the
            toggle must never feel dead). Raw / relationship cells have no rule to
            narrate, so this block is skipped for them. A formula-less roll-up has
            a RuleSpeak rule but no formula / English — those dialects say so. */}
        {(dag.type !== "raw" && dag.type !== "relationship") && (
          mode === "rulespeak"
          ? <HoverRuleSpeak ruleSpeak={ruleSpeak} refs={refs} renderRef={renderRef} />
          : mode === "english"
          ? <HoverEnglish english={english} refs={refs} renderRef={renderRef} />
          : <HoverFormula formula={dag.formula} table={dag.table} lookup={upstreamLookup} />)}

        {docs.inputs && dag.upstream.length > 0 && (
          <span className="dag-hovercard-inputs">
            <span className="dag-hovercard-label">
              Inputs <span className="dag-hovercard-count">{dag.upstream.length}</span>
            </span>
            <span className="dag-hovercard-input-row">
              {dag.upstream.map((u) => (
                <FieldChip
                  key={`${u.table}.${u.field}`}
                  table={u.table}
                  field={u.field}
                  node={u}
                  variant="pill"
                  showTable="always"
                />
              ))}
            </span>
          </span>
        )}

        <span className="dag-hovercard-hint">
          Double-click the cell to open the full DAG page.
        </span>
      </span>
    </span>
  );
}

// ── Dialect blocks — one shown at a time, matching the global narration mode ──

function HoverRuleSpeak({
  ruleSpeak, refs, renderRef,
}: {
  ruleSpeak: ReturnType<typeof ruleSpeakForField>;
  refs: RuleSpeakRef[];
  renderRef: (r: RuleSpeakRef, matched: string, key: number) => ReactNode;
}): JSX.Element {
  // The hover is a compact 360px preview, so it always shows the flat
  // single-sentence RuleSpeak rule (refs still clickable) — the full nested
  // priority/AND-OR outline is reserved for the roomy /dag page (RuleTree).
  return (
    <span className="dag-hovercard-english">
      <span className="dag-hovercard-label">In RuleSpeak</span>
      {ruleSpeak?.rule ? (
        <span className="dag-hovercard-english-text">{renderRuleRich(ruleSpeak.rule, refs, renderRef)}.</span>
      ) : (
        <span className="dag-hovercard-english-text dag-hovercard-muted">No declarative rule rendered.</span>
      )}
    </span>
  );
}

function HoverEnglish({
  english, refs, renderRef,
}: {
  english: string | null;
  refs: RuleSpeakRef[];
  renderRef: (r: RuleSpeakRef, matched: string, key: number) => ReactNode;
}): JSX.Element {
  return (
    <span className="dag-hovercard-english">
      <span className="dag-hovercard-label">In English</span>
      <span className={`dag-hovercard-english-text${english ? "" : " dag-hovercard-muted"}`}>
        {english
          ? <>{linkifyText(english, refs, renderRef)}.</>
          : "No formula — a structural roll-up. See RuleSpeak."}
      </span>
    </span>
  );
}

function HoverFormula({
  formula, table, lookup,
}: {
  formula: string | null;
  table: string;
  lookup: Record<string, FieldNode>;
}): JSX.Element {
  return (
    <span className="dag-hovercard-formula">
      <span className="dag-hovercard-label">As a formula</span>
      {formula ? (
        <span className="dag-hovercard-formula-code">
          <FormulaText formula={formula} table={table} lookup={lookup} />
        </span>
      ) : (
        <span className="dag-hovercard-english-text dag-hovercard-muted">
          No formula — a structural roll-up. See RuleSpeak.
        </span>
      )}
    </span>
  );
}
