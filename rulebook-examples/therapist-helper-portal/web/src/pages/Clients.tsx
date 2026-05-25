import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { num } from "../lib/fmt";
import type { Me } from "../App";
import { DagCell } from "../explainer-dag";

export default function Clients(_: { me: Me }) {
  const { data } = useApi<any[]>("/api/clients");
  const list = data || [];
  return (
    <div>
      <h1>Clients</h1>
      <table>
        <thead><tr><th>Name</th><th>Sessions</th><th>Avg mood</th><th>Progress %</th><th>Status</th></tr></thead>
        <tbody>
          {list.map((c) => (
            <tr key={c.clients_id}>
              <td><Link to={`/clients/${c.clients_id}`}>{c.client_name}</Link></td>
              <td>{c.session_count}</td>
              <td>{num(c.avg_mood_rating)}</td>
              <td>{num(c.avg_goal_progress, 1)}</td>
              <td><DagCell table="Clients" field="StatusLabel">
                <span className={`badge ${c.is_at_risk ? "warn" : "ok"}`}>{c.status_label}</span>
              </DagCell></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
