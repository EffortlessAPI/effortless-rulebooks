import { useState } from "react";
import { useRows, useSession } from "../session";
import { AppBar, Err, Loading, human } from "../ui/kit";

/** A role with no hand-built home: every table in its schema, readable. Generated from the sign-in, not designed. */
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
        {table && (!data.rows ? <Loading /> : <div className="card" style={{ overflowX: "auto" }}><table className="t"><thead><tr>{cols.map((c) => <th key={c}>{c.replace(/_/g, " ")}</th>)}</tr></thead>
          <tbody>{data.rows.slice(0, 60).map((r, i) => <tr key={i}>{cols.map((c) => <td key={c}>{String(r[c] ?? "—").slice(0, 80)}</td>)}</tr>)}</tbody></table></div>)}
      </div>
    </>
  );
}
