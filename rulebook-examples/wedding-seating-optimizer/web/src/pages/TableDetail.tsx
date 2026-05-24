import { useParams, Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { Table, Guest, Me } from "../types";
import { num } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

export default function TableDetail({ me }: { me: Me }) {
  const { id } = useParams();
  const { data } = useApi<{ table: Table; guests: Guest[] }>(id ? `/api/tables/${id}` : null);
  if (!data) return <div>Loading…</div>;
  const isCoord = me.role === "coordinator";

  const moodClass = (m: string) => (m === "Happy" ? "good" : m === "Neutral" ? "muted" : "bad");

  return (
    <div>
      <h2>
        {data.table.label}
        <span className={`pill ${data.table.over_capacity ? "bad" : "muted"}`} style={{ marginLeft: 12 }}>
          <DagCell table="Tables" field="HeadCount">{data.table.head_count}</DagCell>
          {" "}/ {data.table.seats}
        </span>
      </h2>

      <div className="stat-grid">
        <div className="stat-tile">
          <div className="label">Happiness</div>
          <div className="value"><DagCell table="Tables" field="Happiness">{num(data.table.happiness)}</DagCell></div>
        </div>
        <div className="stat-tile">
          <div className="label">Raw happiness</div>
          <div className="value"><DagCell table="Tables" field="RawHappiness">{num(data.table.raw_happiness)}</DagCell></div>
        </div>
        <div className="stat-tile">
          <div className="label">Violations</div>
          <div className="value"><DagCell table="Tables" field="ViolationCount">{data.table.violation_count}</DagCell></div>
        </div>
        <div className="stat-tile">
          <div className="label">Grade</div>
          <div className="value"><DagCell table="Tables" field="Grade">{data.table.grade}</DagCell></div>
        </div>
      </div>

      <h3 style={{ marginTop: 32 }}>Seated guests</h3>
      <table>
        <thead>
          <tr><th>Name</th><th>Side</th><th>Dietary</th><th>Satisfaction</th><th>Mood</th></tr>
        </thead>
        <tbody>
          {data.guests.map((g) => (
            <tr key={g.guest_id}>
              <td>{isCoord ? <Link to={`/guests/${g.guest_id}`}>{g.full_name}</Link> : g.full_name}</td>
              <td>{g.side}</td>
              <td>{g.dietary}</td>
              <td><DagCell table="Guests" field="Satisfaction">{num(g.satisfaction)}</DagCell></td>
              <td><DagCell table="Guests" field="Mood"><span className={`pill ${moodClass(g.mood)}`}>{g.mood}</span></DagCell></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
