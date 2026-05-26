// Render a rulebook formula as JSX: each {{ref}} becomes a FieldChip, the
// rest is operators / function names / literals. Falls back to plain text if
// the formula doesn't parse.

import { useMemo } from "react";
import { tryParseFormula } from "../lib/formula.ts";
import { renderJsx } from "../lib/renderJsx.tsx";
import { FieldChip } from "./FieldChip.tsx";
import type { FieldNode } from "../lib/types.ts";

interface Props {
  formula: string;
  // The current field's table — used to resolve same-table refs.
  table: string;
  // Map of "Table.Field" → FieldNode for chips we know the type of.
  lookup?: Record<string, FieldNode>;
}

export function FormulaText({ formula, table, lookup }: Props): JSX.Element {
  const ast = useMemo(() => tryParseFormula(formula), [formula]);
  if (!ast) {
    return <code className="fx-raw">{formula}</code>;
  }
  return (
    <span className="fx">
      <span className="fx-eq">= </span>
      {renderJsx(ast, {
        renderChip: (refTable, refField) => {
          const t = refTable ?? table;
          const k = `${t}.${refField}`;
          const node = lookup?.[k];
          return <FieldChip key={k} table={t} field={refField} node={node} variant="chip" pageTable={table} />;
        },
      })}
    </span>
  );
}
