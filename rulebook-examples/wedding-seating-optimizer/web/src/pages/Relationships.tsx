import { useApi } from "../lib/useApi";
import { api } from "../lib/api";
import { Relationship } from "../types";
import { num } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

const kindClass = (k: string) =>
  k === "must-not" ? "bad" : k === "avoid" ? "warn" : k === "loves" ? "good" : "muted";

export default function Relationships() {
  const { data, reload } = useApi<Relationship[]>("/api/relationships");

  async function setKind(rel_id: string, kind: string) {
    await api(`/api/relationships/${rel_id}`, { method: "PATCH", body: JSON.stringify({ kind }) });
    reload();
  }
  async function setWeight(rel_id: string, weight: number) {
    await api(`/api/relationships/${rel_id}`, { method: "PATCH", body: JSON.stringify({ weight }) });
    reload();
  }

  return (
    <div>
      <h2>Relationships</h2>
      <p className="muted">
        Pairwise affinities between guests. <code>must-not</code> pairs sitting together create a hard penalty;
        positive <code>Weight</code> rewards co-seating; negative <code>Weight</code> rewards seating apart.
      </p>
      <table>
        <thead>
          <tr>
            <th>Guest A</th><th>Guest B</th><th>Kind</th><th>Weight</th>
            <th>Same table?</th><th>Effective score</th><th>Satisfied?</th>
          </tr>
        </thead>
        <tbody>
          {data?.map((r) => (
            <tr key={r.rel_id} style={r.is_must_not_violation ? { background: "#fde8e8" } : undefined}>
              <td>{r.guest_a_name}</td>
              <td>{r.guest_b_name}</td>
              <td>
                <select value={r.kind} onChange={(e) => setKind(r.rel_id, e.target.value)}>
                  <option value="loves">loves</option>
                  <option value="prefers">prefers</option>
                  <option value="avoid">avoid</option>
                  <option value="must-not">must-not</option>
                </select>{" "}
                <span className={`pill ${kindClass(r.kind)}`}>{r.kind}</span>
              </td>
              <td>
                <input
                  type="number"
                  defaultValue={String(r.weight)}
                  onBlur={(e) => {
                    const w = Number(e.target.value);
                    if (!Number.isNaN(w) && w !== Number(r.weight)) setWeight(r.rel_id, w);
                  }}
                  style={{ width: 70 }}
                />
              </td>
              <td><DagCell table="Relationships" field="SameTable">{r.same_table ? "yes" : "no"}</DagCell></td>
              <td><DagCell table="Relationships" field="EffectiveScore">{num(r.effective_score)}</DagCell></td>
              <td><DagCell table="Relationships" field="IsSatisfied">{r.is_satisfied ? "yes" : "no"}</DagCell></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
