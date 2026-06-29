// The ontology landing page: every table in the rulebook. Click a table to see
// its columns; from there click any column to walk its DAG. This is the "see the
// whole thing at a glance, then drill in" entry point.

import { useMemo } from "react";
import { listTables } from "../lib/dagResolver.ts";
import type { TableSummary } from "../lib/dagResolver.ts";
import { humanizeField } from "../lib/renderEnglish.ts";
import {
  RoutingContext,
  mergeRouting,
  useTableLink,
} from "../lib/routingContext.tsx";
import type { ExplainerDagRouting } from "../lib/routingContext.tsx";
import { ExplainerHeader } from "../components/ExplainerHeader.tsx";

export interface TablesIndexProps {
  routing?: ExplainerDagRouting;
}

export function TablesIndex({ routing }: TablesIndexProps): JSX.Element {
  const merged = useMemo(() => mergeRouting(routing), [routing]);
  return (
    <RoutingContext.Provider value={merged}>
      <TablesIndexInner />
    </RoutingContext.Provider>
  );
}

function TablesIndexInner(): JSX.Element {
  const tables = useMemo(() => listTables(), []);
  const TableLink = useTableLink();

  return (
    <div className="dag-page">
      <ExplainerHeader crumbs={["Tables"]} />

      <header className="dag-hero dag-hero-calc">
        <div className="dag-hero-meta">
          <span className="dag-pill">{tables.length} tables</span>
        </div>
        <h1 className="dag-title">The ontology</h1>
        <p className="dag-subtitle">
          <span className="dag-subtitle-human">
            Every table in the rulebook. Open one to see its columns, then click any
            column to trace how it is computed, all the way down to ground truth.
          </span>
        </p>
      </header>

      <section className="dag-section">
        <div className="dag-table-grid">
          {tables.map((t) => (
            <TableCardLink key={t.table} t={t} TableLink={TableLink} />
          ))}
        </div>
      </section>
    </div>
  );
}

function TableCardLink({
  t,
  TableLink,
}: {
  t: TableSummary;
  TableLink: ReturnType<typeof useTableLink>;
}): JSX.Element {
  return (
    <TableLink table={t.table} className="dag-table-card">
      <span className="dag-table-card-name">{t.table}</span>
      <span className="dag-table-card-human">{humanizeField(t.table)}</span>
      <span className="dag-table-card-counts">
        <span className="dag-pill dag-pill-muted">{t.fieldCount} columns</span>
        {t.derivedCount > 0 && (
          <span className="dag-pill dag-pill-calc">{t.derivedCount} derived</span>
        )}
        {t.relationshipCount > 0 && (
          <span className="dag-pill dag-pill-rel">{t.relationshipCount} links</span>
        )}
      </span>
      {t.description && <span className="dag-table-card-desc">{t.description}</span>}
    </TableLink>
  );
}
