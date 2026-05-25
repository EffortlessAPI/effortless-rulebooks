// The DAG explorer page. The host app routes /dag/:table/:field to this
// component, passing `table` and `field` as props, plus optional routing
// callbacks (FieldLink, onBack).
//
// Reads like a dictionary entry: this cell's one-line function, with each
// vocabulary word linked to its own one-line function, all the way down to
// ground truth.

import { useMemo } from "react";
import type { ReactNode } from "react";
import { resolveDag } from "../lib/dagResolver.ts";
import type { DagResponse, FieldNode, FieldType } from "../lib/types.ts";
import { humanizeField, renderEnglish } from "../lib/renderEnglish.ts";
import { tryParseFormula } from "../lib/formula.ts";
import { FormulaText } from "../components/FormulaText.tsx";
import { FieldChip } from "../components/FieldChip.tsx";
import { TypeBadge, typeGlyph, typeTone } from "../components/TypeBadge.tsx";
import {
  RoutingContext,
  useFieldLink,
  useOnBack,
} from "../lib/routingContext.tsx";
import type {
  ExplainerDagRouting,
  FieldLinkProps,
} from "../lib/routingContext.tsx";

export type { ExplainerDagRouting, FieldLinkProps };

export interface FieldDagProps {
  table: string;
  field: string;
  routing?: ExplainerDagRouting;
}

export function FieldDag(props: FieldDagProps): JSX.Element {
  const { table, field, routing } = props;
  const merged = useMemo(() => mergeRouting(routing), [routing]);
  return (
    <RoutingContext.Provider value={merged}>
      <FieldDagInner table={table} field={field} />
    </RoutingContext.Provider>
  );
}

function mergeRouting(r: ExplainerDagRouting | undefined): Required<ExplainerDagRouting> {
  const defaultFieldLink = (p: FieldLinkProps) => (
    <a
      href={`#/dag/${encodeURIComponent(p.table)}/${encodeURIComponent(p.field)}`}
      className={p.className}
    >
      {p.children}
    </a>
  );
  const defaultNavigate = (t: string, f: string) => {
    if (typeof window !== "undefined") {
      window.location.hash = `#/dag/${encodeURIComponent(t)}/${encodeURIComponent(f)}`;
    }
  };
  const defaultOnBack = () => {
    if (typeof window !== "undefined") window.history.back();
  };
  return {
    FieldLink: r?.FieldLink ?? defaultFieldLink,
    onBack: r?.onBack ?? defaultOnBack,
    navigate: r?.navigate ?? defaultNavigate,
  };
}

function FieldDagInner({ table, field }: { table: string; field: string }): JSX.Element {
  const dag = useMemo<DagResponse | null>(() => resolveDag(table, field), [table, field]);

  const upstreamLookup = useMemo(() => {
    if (!dag) return {};
    const m: Record<string, FieldNode> = {};
    for (const n of dag.upstream) m[`${n.table}.${n.field}`] = n;
    m[`${dag.table}.${dag.field}`] = dag;
    return m;
  }, [dag]);

  const englishSentence = useMemo(() => {
    if (!dag?.formula) return null;
    const ast = tryParseFormula(dag.formula);
    if (!ast) return null;
    try {
      return renderEnglish(ast);
    } catch { return null; }
  }, [dag?.formula]);

  if (!dag) {
    return (
      <div className="dag-page">
        <Breadcrumb table={table} field={field} />
        <div className="muted">Field not found in rulebook.</div>
      </div>
    );
  }

  return (
    <div className="dag-page">
      <Breadcrumb table={dag.table} field={dag.field} />
      <Hero dag={dag} />

      {dag.type === "raw" || dag.type === "relationship" ? (
        <GroundTruthCard datatype={dag.datatype} type={dag.type} />
      ) : dag.formula ? (
        <FormulaCard
          formula={dag.formula}
          english={englishSentence}
          table={dag.table}
          lookup={upstreamLookup}
        />
      ) : null}

      {dag.upstream.length > 0 && <InputsSection upstream={dag.upstream} />}

      <ConsumersSection direct={dag.downstream} transitive={dag.consumerTransitive} />

      {dag.leaves.length > 0 && (
        <LeavesSection leaves={dag.leaves} />
      )}
    </div>
  );
}

// ─────────────────────────────────────────────────────────────────────────

function Breadcrumb({ table, field }: { table: string; field: string }): JSX.Element {
  const onBack = useOnBack();
  return (
    <div className="dag-crumb">
      <button type="button" className="dag-back" onClick={() => onBack()}>← back</button>
      <span className="dag-crumb-sep">·</span>
      <span className="dag-crumb-table">{table}</span>
      <span className="dag-crumb-sep">›</span>
      <span className="dag-crumb-field">{field}</span>
    </div>
  );
}

function Hero({ dag }: { dag: DagResponse }): JSX.Element {
  const tone = typeTone(dag.type);
  return (
    <header className={`dag-hero dag-hero-${tone}`}>
      <div className="dag-hero-meta">
        <TypeBadge type={dag.type} />
        {dag.depth > 0 && (
          <span className="dag-pill dag-pill-depth">
            {dag.depth === 1 ? "1 hop from ground truth" : `${dag.depth} hops from ground truth`}
          </span>
        )}
        <span className="dag-pill">{dag.datatype}</span>
        {dag.nullable && <span className="dag-pill dag-pill-muted">nullable</span>}
      </div>
      <h1 className="dag-title">{dag.field}</h1>
      <p className="dag-subtitle">
        <span className="dag-subtitle-table">{dag.table}</span>
        {" · "}
        <span className="dag-subtitle-human">{humanizeField(dag.field)}</span>
      </p>

      {dag.description && (
        <p className="dag-description">{dag.description}</p>
      )}
    </header>
  );
}

function GroundTruthCard({ datatype, type }: { datatype: string; type: FieldType }): JSX.Element {
  return (
    <section className="dag-card dag-gt">
      <div className="dag-gt-glyph">{typeGlyph(type)}</div>
      <div>
        <h3 className="dag-gt-title">
          {type === "relationship" ? "Relationship pointer" : "Ground truth"}
        </h3>
        <p className="dag-gt-body">
          {type === "relationship" ? (
            <>This field is a pointer to another table. It is not derived from
              anything else — it&rsquo;s the link the rest of the DAG hangs off.</>
          ) : (
            <>This cell is a leaf. It is <em>written</em> directly to the
              database (datatype <code>{datatype}</code>) and is not computed
              from anything else. Everything else upstream of this row
              eventually traces back to values like this one.</>
          )}
        </p>
      </div>
    </section>
  );
}

function FormulaCard({
  formula, english, table, lookup,
}: {
  formula: string;
  english: string | null;
  table: string;
  lookup: Record<string, FieldNode>;
}): JSX.Element {
  return (
    <section className="dag-card dag-formula">
      <h3 className="dag-card-title">This cell is the value of one one-line function</h3>

      {english && (
        <div className="dag-english">
          <span className="dag-english-label">In English</span>
          <p className="dag-english-text">{english}.</p>
        </div>
      )}

      <div className="dag-formula-syntax">
        <span className="dag-formula-label">As a formula</span>
        <div className="dag-formula-code">
          <FormulaText formula={formula} table={table} lookup={lookup} />
        </div>
      </div>

      <p className="dag-formula-hint muted">
        Each chip is itself a one-line function. Click any chip to drill in.
      </p>
    </section>
  );
}

function InputsSection({ upstream }: { upstream: FieldNode[] }): JSX.Element {
  return (
    <section className="dag-section">
      <h2 className="dag-section-title">
        ▼ Inputs <span className="dag-count">{upstream.length}</span>
      </h2>
      <p className="dag-section-lead muted">Fields this one-line function calls.</p>
      <div className="dag-input-grid">
        {upstream.map((u) => (
          <InputCard key={`${u.table}.${u.field}`} node={u} />
        ))}
      </div>
    </section>
  );
}

function InputCard({ node }: { node: FieldNode }): JSX.Element {
  const tone = typeTone(node.type);
  const FieldLink = useFieldLink();
  const english = useMemo(() => {
    if (!node.formula) return null;
    const ast = tryParseFormula(node.formula);
    if (!ast) return null;
    try { return renderEnglish(ast); } catch { return null; }
  }, [node.formula]);

  return (
    <FieldLink
      table={node.table}
      field={node.field}
      className={`dag-input dag-input-${tone}`}
    >
      <span className="dag-input-head">
        <TypeBadge type={node.type} size="sm" />
        <span className="dag-input-name">{node.field}</span>
        <span className="dag-input-table">{node.table}</span>
      </span>
      {english && <span className="dag-input-english">{english}.</span>}
      {!english && node.description && <span className="dag-input-desc">{node.description}</span>}
      {!english && !node.description && node.type === "raw" && (
        <span className="dag-input-desc muted">Ground truth · datatype {node.datatype}</span>
      )}
    </FieldLink>
  );
}

function ConsumersSection({
  direct, transitive,
}: {
  direct: FieldNode[];
  transitive: { table: string; field: string }[];
}): JSX.Element {
  const transitiveOnly = transitive.filter(
    (t) => !direct.some((d) => d.table === t.table && d.field === t.field),
  );

  if (direct.length === 0 && transitive.length === 0) {
    return (
      <section className="dag-section">
        <h2 className="dag-section-title">▼ Consumers <span className="dag-count">0</span></h2>
        <p className="muted">Nothing else in the rulebook references this cell yet.</p>
      </section>
    );
  }

  return (
    <section className="dag-section">
      <h2 className="dag-section-title">
        ▼ Consumers
        <span className="dag-count">{direct.length} direct</span>
        {transitiveOnly.length > 0 && (
          <span className="dag-count dag-count-muted">+{transitiveOnly.length} transitive</span>
        )}
      </h2>
      <p className="dag-section-lead muted">Fields whose one-line function calls this one.</p>

      {direct.length > 0 && (
        <div className="dag-pill-row">
          {direct.map((d) => (
            <FieldChip key={`${d.table}.${d.field}`} table={d.table} field={d.field} node={d} variant="pill" showTable="always" />
          ))}
        </div>
      )}

      {transitiveOnly.length > 0 && (
        <details className="dag-transitive">
          <summary>Show {transitiveOnly.length} transitive consumers (downstream of downstream)</summary>
          <div className="dag-pill-row dag-pill-row-faded">
            {transitiveOnly.map((t) => (
              <FieldChip key={`${t.table}.${t.field}`} table={t.table} field={t.field} variant="pill" showTable="always" />
            ))}
          </div>
        </details>
      )}
    </section>
  );
}

function LeavesSection({ leaves }: { leaves: FieldNode[] }): JSX.Element {
  return (
    <section className="dag-section dag-section-leaves">
      <h2 className="dag-section-title">
        ▼ Ground truth at the leaves <span className="dag-count">{leaves.length}</span>
      </h2>
      <p className="dag-section-lead muted">
        Ultimately, this cell&rsquo;s value depends on these raw values. Editing any
        of these and rebuilding propagates here automatically.
      </p>
      <div className="dag-pill-row">
        {leaves.map((l) => (
          <FieldChip key={`${l.table}.${l.field}`} table={l.table} field={l.field} node={l} variant="pill" showTable="always" />
        ))}
      </div>
    </section>
  );
}

// Returned for completeness; not used here since we removed row-value display.
// Kept exported so Phase B's row-value adapter can re-use it.
export function _formatScalar(v: unknown, datatype: string): string {
  if (v === null || v === undefined) return "—";
  if (typeof v === "boolean") return v ? "TRUE" : "FALSE";
  if (typeof v === "number") {
    if (datatype === "number" && Math.abs(v) >= 100) {
      return v.toLocaleString("en-US", { style: "currency", currency: "USD" });
    }
    return v.toLocaleString("en-US");
  }
  if (datatype === "date" && typeof v === "string") {
    return v.slice(0, 10);
  }
  return String(v);
}

// Silence unused-import warning for ReactNode in older TS configs.
type _RN = ReactNode;
