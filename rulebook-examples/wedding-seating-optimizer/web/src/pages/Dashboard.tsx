import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { Table, PlanScore } from "../types";
import { num } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

const gradeClass = (g: string) =>
  g === "Great" ? "good" : g === "OK" ? "warn" : g === "Conflict" ? "bad" : g === "Over Capacity" ? "bad" : "muted";

export default function Dashboard() {
  const { data: score } = useApi<PlanScore>("/api/plan-score");
  const { data: tables } = useApi<Table[]>("/api/tables");

  return (
    <div>
      <h2>Plan overview</h2>
      <p className="muted">
        Every number below is wrapped in a DAG cell — hover for a peek, click for the
        full inference graph. Edit a guest's table assignment on the <Link to="/guests">Guests</Link> page
        to watch the cascade.
      </p>

      <div className="stat-grid">
        <DagCell table="Tables" field="Happiness" block>
          <div className="stat-tile">
            <div className="label">Total happiness</div>
            <div className="value">{num(score?.total_happiness ?? 0)}</div>
            <div className="sub">summed across all tables</div>
          </div>
        </DagCell>
        <DagCell table="Tables" field="OverCapacity" block>
          <div className="stat-tile">
            <div className="label">Over-capacity tables</div>
            <div className="value">{score?.over_capacity_count ?? 0}</div>
            <div className="sub">head_count &gt; seats</div>
          </div>
        </DagCell>
        <DagCell table="Relationships" field="IsMustNotViolation" block>
          <div className="stat-tile">
            <div className="label">Must-not violations</div>
            <div className="value" style={{ color: (score?.must_not_violations ?? 0) > 0 ? "#b3261e" : undefined }}>
              {score?.must_not_violations ?? 0}
            </div>
            <div className="sub">pairs that can't share a table</div>
          </div>
        </DagCell>
        <DagCell table="Relationships" field="IsSatisfied" block>
          <div className="stat-tile">
            <div className="label">Satisfied pairs</div>
            <div className="value">{score?.satisfied_pairs ?? 0} / {score?.total_pairs ?? 0}</div>
            <div className="sub">positive pairs co-seated; negative pairs apart</div>
          </div>
        </DagCell>
      </div>

      <h3 style={{ marginTop: 32 }}>Tables</h3>
      <table>
        <thead>
          <tr>
            <th>Label</th><th>Seats</th><th>Head count</th><th>Open</th><th>B/G sides</th><th>Skew</th><th>Happiness</th><th>Violations</th><th>Grade</th>
          </tr>
        </thead>
        <tbody>
          {tables?.map((t) => (
            <tr key={t.table_id}>
              <td><Link to={`/tables/${t.table_id}`}>{t.label}</Link></td>
              <td>{t.seats}</td>
              <td><DagCell table="Tables" field="HeadCount">{t.head_count}</DagCell></td>
              <td><DagCell table="Tables" field="OpenSeats">{t.open_seats}</DagCell></td>
              <td><DagCell table="Tables" field="BrideSideCount">{t.bride_side_count}</DagCell> / <DagCell table="Tables" field="GroomSideCount">{t.groom_side_count}</DagCell></td>
              <td><DagCell table="Tables" field="SideSkew"><span className={t.side_skew > 2 ? "pill warn" : ""}>{t.side_skew}</span></DagCell></td>
              <td><DagCell table="Tables" field="Happiness">{num(t.happiness)}</DagCell></td>
              <td><DagCell table="Tables" field="ViolationCount">{t.violation_count}</DagCell></td>
              <td><DagCell table="Tables" field="Grade"><span className={`pill ${gradeClass(t.grade)}`}>{t.grade}</span></DagCell></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
