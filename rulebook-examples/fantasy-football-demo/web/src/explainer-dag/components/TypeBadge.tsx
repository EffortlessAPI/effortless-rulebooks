// Colored badge for a field's type. Glyph + label.
import type { FieldType } from "../lib/types.ts";

const META: Record<FieldType, { glyph: string; label: string; tone: string }> = {
  raw:           { glyph: "●", label: "Ground truth",  tone: "raw" },
  calculated:    { glyph: "ƒ", label: "Calculated",    tone: "calc" },
  aggregation:   { glyph: "Σ", label: "Aggregation",   tone: "agg" },
  relationship:  { glyph: "⇢", label: "Relationship",  tone: "rel" },
  lookup:        { glyph: "↗", label: "Lookup",        tone: "lookup" },
};

export function typeGlyph(t: FieldType): string { return META[t].glyph; }
export function typeLabel(t: FieldType): string { return META[t].label; }
export function typeTone(t: FieldType): string  { return META[t].tone; }

export function TypeBadge({ type, size = "md" }: { type: FieldType; size?: "sm" | "md" }): JSX.Element {
  const m = META[type];
  return (
    <span className={`tb tb-${m.tone} tb-${size}`}>
      <span className="tb-glyph">{m.glyph}</span>
      <span className="tb-label">{m.label}</span>
    </span>
  );
}
