// Floating top-right pill that toggles whether the ƒ provenance glyphs are
// visible on cells. Double-click on cells still works either way.

import { useGlyphsOn, setGlyphsOn } from "../lib/dagPrefs.ts";

export function DagToggle(): JSX.Element {
  const on = useGlyphsOn();
  return (
    <button
      type="button"
      className={`dag-toggle ${on ? "is-on" : "is-off"}`}
      onClick={() => setGlyphsOn(!on)}
      title={
        on
          ? "Hide ƒ provenance glyphs. Double-click on cells still opens the DAG."
          : "Show ƒ provenance glyphs on cells with explorable provenance."
      }
      aria-pressed={on}
    >
      <span className="dag-toggle-glyph">ƒ</span>
      <span className="dag-toggle-label">{on ? "Provenance · ON" : "Provenance · off"}</span>
      <span className="dag-toggle-knob" aria-hidden="true" />
    </button>
  );
}
