import type { ReactNode } from 'react';

/** Wraps a derived view value for the Effortless Explainer DAG (data-er-dag). */
export function DagValue({
  table,
  field,
  children,
  className,
}: {
  table: string;
  field: string;
  children: ReactNode;
  className?: string;
}) {
  return (
    <span data-er-dag={`${table}.${field}`} className={className}>
      {children}
    </span>
  );
}
