import { Link } from "react-router-dom";
import type { Me } from "../App";
import { useApi } from "../lib/useApi";
import { num, dateOnly } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

interface Client {
  clients_id: string; client_name: string; therapist: string;
  session_count: number; avg_mood_rating: any; goal_count: number;
  avg_goal_progress: any;
  last_session_label: any; is_at_risk: boolean; status_label: string;
}

export default function Dashboard({ me }: { me: Me }) {
  const { data: clients } = useApi<Client[]>("/api/clients");
  const list = clients || [];
  const atRisk = list.filter((c) => c.is_at_risk);
  const avgMood = list.length ? list.reduce((s, c) => s + Number(c.avg_mood_rating || 0), 0) / list.length : 0;
  const avgProgress = list.length ? list.reduce((s, c) => s + Number(c.avg_goal_progress || 0), 0) / list.length : 0;

  return (
    <div>
      <h1>Dashboard</h1>
      <p className="muted">Logged in as {me.full_name}</p>

      <div className="stats">
        <Stat label="Active clients" value={String(list.length)} />
        <Stat label="At risk" value={String(atRisk.length)} tone={atRisk.length ? "warn" : "ok"} />
        <Stat label="Avg mood (1-10)" value={num(avgMood)} />
        <Stat label="Avg progress %" value={num(avgProgress, 1)} />
      </div>

      <h2>Your clients</h2>
      <table>
        <thead>
          <tr>
            <th>Client</th><th>Sessions</th><th>Last session</th>
            <th>Avg mood</th><th>Goals</th>
            <th>Avg progress %</th><th>Status</th>
          </tr>
        </thead>
        <tbody>
          {list.map((c) => (
            <tr key={c.clients_id}>
              <td><Link to={`/clients/${c.clients_id}`}>{c.client_name}</Link></td>
              <td><DagCell table="Clients" field="SessionCount">{c.session_count}</DagCell></td>
              <td><DagCell table="Clients" field="LastSessionLabel">{dateOnly(c.last_session_label)}</DagCell></td>
              <td><DagCell table="Clients" field="AvgMoodRating">{num(c.avg_mood_rating)}</DagCell></td>
              <td><DagCell table="Clients" field="GoalCount">{c.goal_count}</DagCell></td>
              <td><DagCell table="Clients" field="AvgGoalProgress">{num(c.avg_goal_progress, 1)}</DagCell></td>
              <td><DagCell table="Clients" field="StatusLabel">
                <span className={`badge ${c.is_at_risk ? "warn" : "ok"}`}>{c.status_label}</span>
              </DagCell></td>
            </tr>
          ))}
          {!list.length && <tr><td colSpan={7} className="muted">No clients yet.</td></tr>}
        </tbody>
      </table>
    </div>
  );
}

function Stat({ label, value, tone }: { label: string; value: string; tone?: "ok" | "warn" }) {
  return (
    <div className={`stat ${tone || ""}`}>
      <div className="stat-value">{value}</div>
      <div className="stat-label">{label}</div>
    </div>
  );
}
