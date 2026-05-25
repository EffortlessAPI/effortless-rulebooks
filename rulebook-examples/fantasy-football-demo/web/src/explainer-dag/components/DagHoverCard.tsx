// Hover-triggered micro page for a DagCell's field. Shows the same key details
// that the full /dag/:table/:field page shows (type badge, description, the
// one-line function in English and as a formula, immediate inputs), so a
// presenter can point to each piece during a demo without the card vanishing.
//
// Sticky-hover behavior: the visible card is wrapped in a transparent "safety
// zone" so the mouse can travel ~15px between the trigger glyph and the card
// without dismissing. A short close-delay (220ms) on mouseleave gives the same
// pedagogical breathing room.

import { useMemo } from "react";
import type { DagResponse, FieldNode } from "../lib/types.ts";
import { humanizeField, renderEnglish } from "../lib/renderEnglish.ts";
import { tryParseFormula } from "../lib/formula.ts";
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

        {dag.description && (
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

        {english && (
          <span className="dag-hovercard-english">
            <span className="dag-hovercard-label">In English</span>
            <span className="dag-hovercard-english-text">{english}.</span>
          </span>
        )}

        {dag.formula && (
          <span className="dag-hovercard-formula">
            <span className="dag-hovercard-label">As a formula</span>
            <span className="dag-hovercard-formula-code">
              <FormulaText formula={dag.formula} table={dag.table} lookup={upstreamLookup} />
            </span>
          </span>
        )}

        {dag.upstream.length > 0 && (
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
