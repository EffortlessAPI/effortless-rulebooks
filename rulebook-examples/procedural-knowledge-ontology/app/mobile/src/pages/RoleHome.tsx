import { useState } from "react";
import { useRows, useSession } from "../session";
import { AppBar, Err, ExplainerNote, Loading, Th, human } from "../ui/kit";

/**
 * A role with no hand-built home: every table in its schema, readable. Generated
 * from the sign-in, not designed — and because the columns come from the rows the
 * role's own view returned, every one of them is a real column the catalog can
 * explain. Nothing here is named by hand, so the explainers cannot fall behind
 * the model: a field added to the rulebook appears with its provenance attached.
 */
export default function RoleHome() {
  const { shell } = useSession();
  const [table, setTable] = useState<string | null>(null);
  const data = useRows(table, {});
  const cols = data.rows?.[0] ? Object.keys(data.rows[0]).filter((c) => c !== "semantic_type_iri").slice(0, 7) : [];
  return (
    <>
      <AppBar title={shell!.profile?.home_title || shell!.claims.principal_label} />
      <div className="content">
        {shell!.profile?.pitch && <div className="card"><div className="quote">“{shell!.profile.pitch}”<span className="by">what this role is here to do</span></div>
          <p className="provenance">This role has no hand-built experience yet, so this page is generated from the {shell!.tables.length} tables in its schema. The full console for it is the administrator's read-only register.</p></div>}
        <div className="row wrap" style={{ gap: 6, marginBottom: 12 }}>{shell!.tables.map((t) => <button key={t} className="tab" aria-selected={table === t} onClick={() => setTable(t)}>{human(t.replace(/_/g, "-"))}</button>)}</div>
        <Err error={data.error} />
        {table && <ExplainerNote />}
        {table && (!data.rows ? <Loading /> : <div className="card" style={{ overflowX: "auto" }}><table className="t"><thead><tr>{cols.map((c) => <Th key={c} t={table} f={c}>{c.replace(/_/g, " ")}</Th>)}</tr></thead>
          <tbody>{data.rows.slice(0, 60).map((r, i) => <tr key={i}>{cols.map((c) => <td key={c}>{String(r[c] ?? "—").slice(0, 80)}</td>)}</tr>)}</tbody></table></div>)}
      </div>
    </>
  );
}
