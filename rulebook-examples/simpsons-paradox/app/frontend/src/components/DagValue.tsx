import { useLayoutEffect } from 'react';
import type { ReactNode } from 'react';

/** snake_case view column → PascalCase rulebook field */
export function snakeToPascal(snake: string): string {
  return snake
    .split('_')
    .filter(Boolean)
    .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
    .join('');
}

type DagValueProps = {
  /** Rulebook table name (PascalCase) */
  table: string;
  /** Rulebook field name (PascalCase) — use this OR col */
  field?: string;
  /** View column name (snake_case) — converted to PascalCase automatically */
  col?: string;
  children: ReactNode;
  className?: string;
  /** Skip provenance (raw seed / UI chrome only) */
  raw?: boolean;
};

/** Wraps a derived view value for the Effortless Explainer DAG (data-er-dag). */
export function DagValue({ table, field, col, children, className, raw }: DagValueProps) {
  if (raw) return <span className={className}>{children}</span>;
  const f = field ?? (col ? snakeToPascal(col) : '');
  if (!f) return <span className={className}>{children}</span>;
  return (
    <span data-er-dag={`${table}.${f}`} className={className}>
      {children}
    </span>
  );
}

/** Shorthand alias used throughout views */
export const V = DagValue;

/**
 * Re-run enhanceCells after async view data paints. Without this, the explainer
 * only scans once at app mount while pages still show "Loading…".
 */
export function ViewDagScan({
  ready = true,
  deps = [],
}: {
  ready?: boolean;
  deps?: unknown[];
}) {
  useLayoutEffect(() => {
    if (!ready) return;
    window.EffortlessExplainer?.enhanceCells(document);
  }, [ready, ...deps]); // eslint-disable-line react-hooks/exhaustive-deps
  return null;
}
