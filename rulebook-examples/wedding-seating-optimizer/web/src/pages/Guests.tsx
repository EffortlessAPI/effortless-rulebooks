import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { Guest, Table } from "../types";
import { num } from "../lib/fmt";
import { api } from "../lib/api";
import { DagCell } from "../explainer-dag";

const moodClass = (m: string) => (m === "Happy" ? "good" : m === "Neutral" ? "muted" : "bad");

export default function Guests() {
  const { data: guests, reload } = useApi<Guest[]>("/api/guests");
  const { data: tables } = useApi<Table[]>("/api/tables");

  async function reassign(guest_id: string, table_id: string) {
    await api(`/api/guests/${guest_id}`, {
      method: "PATCH",
      body: JSON.stringify({ assigned_table: table_id }),
    });
    reload();
  }

  return (
    <div>
      <h2>Guests</h2>
      <p className="muted">
        Change a guest's table to watch every dependent number recompute. The dropdown
        writes one raw column (<code>assigned_table</code>); the DAG handles the rest.
      </p>
      <table>
        <thead>
          <tr>
            <th>Name</th><th>Side</th><th>Dietary</th><th>Lang</th><th>Table</th>
            <th>Satisfaction</th><th>Mood</th>
          </tr>
        </thead>
        <tbody>
          {guests?.map((g) => (
            <tr key={g.guest_id}>
              <td><Link to={`/guests/${g.guest_id}`}>{g.full_name}</Link></td>
              <td>{g.side}</td>
              <td>{g.dietary}</td>
              <td>{g.language}</td>
              <td>
                <select
                  value={g.assigned_table ?? ""}
                  onChange={(e) => reassign(g.guest_id, e.target.value)}
                >
                  <option value="">— unassigned —</option>
                  {tables?.map((t) => (
                    <option key={t.table_id} value={t.table_id}>{t.label}</option>
                  ))}
                </select>
              </td>
              <td><DagCell table="Guests" field="Satisfaction">{num(g.satisfaction)}</DagCell></td>
              <td><DagCell table="Guests" field="Mood"><span className={`pill ${moodClass(g.mood)}`}>{g.mood}</span></DagCell></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
