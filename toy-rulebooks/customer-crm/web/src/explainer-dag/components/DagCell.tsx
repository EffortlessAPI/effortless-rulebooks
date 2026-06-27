// Wrap any value with provenance. Double-click → DAG page. When provenance is
// enabled (DagToggle on), a small type-coded glyph appears in the upper-right
// corner: ● ground truth, ↗ lookup, ƒ calculated, Σ aggregation, ⇢ relationship.
// Color and border reinforce the same distinction.
//
// Hovering the glyph opens a sticky "micro page" with the field's key details
// (English form, formula, inputs) so a presenter can point to each piece
// during a demo without the popup disappearing. The popup stays open as long
// as the mouse is over the glyph, the card, or the ~15px safety zone around
// it; leaving all three triggers a short close delay (~220ms).
//
// Navigation is supplied by the routing context (see lib/routingContext.tsx),
// so this component doesn't depend on any particular router.

import { useMemo, useRef, useState } from "react";
import type { ReactNode } from "react";
import { useGlyphsOn } from "../lib/dagPrefs.ts";
import { useFieldLink, useNavigateField } from "../lib/routingContext.tsx";
import { resolveDag } from "../lib/dagResolver.ts";
import { typeGlyph, typeTone } from "./TypeBadge.tsx";
import { DagHoverCard } from "./DagHoverCard.tsx";

interface Props {
  table: string;
  field: string;
  children: ReactNode;
  // Inline-block by default. Pass `block` if wrapping a stat tile or card.
  block?: boolean;
}

const CLOSE_DELAY_MS = 220;

export function DagCell({ table, field, children, block }: Props): JSX.Element {
  const navigate = useNavigateField();
  const FieldLink = useFieldLink();
  const glyphsOn = useGlyphsOn();
  const [hoverOpen, setHoverOpen] = useState(false);
  const closeTimer = useRef<number | null>(null);

  const dag = useMemo(
    () => (glyphsOn ? resolveDag(table, field) : null),
    [glyphsOn, table, field],
  );
  const type = dag?.type ?? "calculated";
  const tone = typeTone(type);
  const glyph = typeGlyph(type);

  const cancelClose = () => {
    if (closeTimer.current !== null) {
      window.clearTimeout(closeTimer.current);
      closeTimer.current = null;
    }
  };
  const openCard = () => {
    cancelClose();
    setHoverOpen(true);
  };
  const scheduleClose = () => {
    cancelClose();
    closeTimer.current = window.setTimeout(() => {
      setHoverOpen(false);
      closeTimer.current = null;
    }, CLOSE_DELAY_MS);
  };

  return (
    <span
      className={`dag-cell ${block ? "dag-cell-block" : ""} ${glyphsOn ? "" : "dag-cell-quiet"}`}
      onDoubleClick={(e) => {
        const target = e.target as HTMLElement;
        if (target.closest("a, button")) return;
        e.preventDefault();
        navigate(table, field);
      }}
      title={glyphsOn
        ? "Hover the badge to peek; double-click to open the full DAG page"
        : "Double-click to see this cell's provenance (toggle ƒ glyphs on top-right to show inline)"}
    >
      {children}
      {glyphsOn && (
        <span className="dag-cell-badge-wrap">
          <FieldLink
            table={table}
            field={field}
            className={`dag-cell-fx dag-cell-fx-${tone}`}
          >
            <span
              className="dag-cell-fx-glyph"
              onMouseEnter={openCard}
              onMouseLeave={scheduleClose}
            >
              {glyph}
            </span>
          </FieldLink>
          {hoverOpen && dag && (
            <DagHoverCard
              dag={dag}
              onMouseEnter={openCard}
              onMouseLeave={scheduleClose}
            />
          )}
        </span>
      )}
    </span>
  );
}
