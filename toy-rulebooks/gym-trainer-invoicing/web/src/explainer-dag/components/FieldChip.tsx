// Clickable chip that references a field. Color-coded by field type.
// Hover surfaces a mini-definition card. Click navigates via the host-provided
// FieldLink (see lib/routingContext.tsx).

import type { FieldNode, FieldType } from "../lib/types.ts";
import { humanizeField } from "../lib/renderEnglish.ts";
import { typeGlyph, typeTone } from "./TypeBadge.tsx";
import { useFieldLink } from "../lib/routingContext.tsx";

interface Props {
  table: string;
  field: string;
  // Known metadata about the referenced field, if we have it.
  node?: FieldNode;
  // Render variant. "chip" is the formula-inline rendering; "pill" is the
  // upstream/downstream card-list rendering with more space.
  variant?: "chip" | "pill";
  // How to display the foreign table prefix on the chip.
  //   "auto"  → show the table only when it differs from `pageTable`
  //   "always"→ always render `Table.Field`
  //   "never" → never render the table (legacy behavior)
  showTable?: "auto" | "always" | "never";
  // The table being explained on the current page. Required when
  // showTable === "auto" so cross-table refs aren't visually flattened.
  pageTable?: string;
}

export function FieldChip({
  table,
  field,
  node,
  variant = "chip",
  showTable = "auto",
  pageTable,
}: Props): JSX.Element {
  const FieldLink = useFieldLink();
  const type: FieldType = node?.type ?? "raw";
  const tone = typeTone(type);
  const glyph = typeGlyph(type);
  const label = humanizeField(field);

  const renderTable =
    showTable === "always" ||
    (showTable === "auto" && !!table && !!pageTable && table !== pageTable);

  return (
    <FieldLink
      table={table}
      field={field}
      className={`fc fc-${tone} fc-${variant}`}
    >
      <span className="fc-glyph">{glyph}</span>
      {renderTable && <span className="fc-table">{table}.</span>}
      <span className="fc-name">{label}</span>
    </FieldLink>
  );
}
