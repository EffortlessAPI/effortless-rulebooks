import type { ReactNode } from "react";
import { useRows } from "../session";
import { Err, Explains, Why } from "./kit";

/**
 * Register-wide totals (loop 17). They are columns of the one current EvaluationContexts row,
 * "the register, as judged at this instant", worked out by the rulebook. Nothing is counted here:
 * a sign-in that reads five fragments still reads the total for all twelve, because the total is a
 * value on a row it may read, not a count of the rows it can see.
 */
export function useRegister() {
  const r = useRows("evaluation_contexts", { is_current: true });
  return { row: r.rows?.[0] ?? null, error: r.error };
}

export type Total = { f: string; label: ReactNode; tone?: "derived" | "bad" | "fact" };

/** One strip of totals. `id` is what a page (and the series' shot plan) finds it by. */
export function Totals({ id, title, note, items, sticky }: { id: string; title: string; note?: ReactNode; items: Total[]; sticky?: boolean }) {
  const { row, error } = useRegister();
  return (
    <Explains t="evaluation_contexts">
      <div id={id} className={`totals ${sticky ? "sticky" : ""}`}>
        <div className="totals-title">{title}</div>
        <Err error={error} />
        <div className="totals-row">
          {items.map((i) => (
            <div key={i.f} className="total" data-total={i.f}>
              <b className={i.tone || "derived"}>{row ? row[i.f] : "·"}<Why f={i.f} /></b>
              <span>{i.label}</span>
            </div>))}
        </div>
        {note && <p className="totals-note">{note}</p>}
      </div>
    </Explains>
  );
}
