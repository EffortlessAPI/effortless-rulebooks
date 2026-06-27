import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { Table } from "../types";
import { num } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

export default function Tables() {
  const { data } = useApi<Table[]>("/api/tables");
  return (
    <div>
      <h2>Tables</h2>
      <table>
        <thead>
          <tr>
            <th>Label</th><th>Seats</th><th>Head count</th><th>Open</th><th>Happiness</th><th>Grade</th>
          </tr>
        </thead>
        <tbody>
          {data?.map((t) => (
            <tr key={t.table_id}>
              <td><Link to={`/tables/${t.table_id}`}>{t.label}</Link></td>
              <td>{t.seats}</td>
              <td><DagCell table="Tables" field="HeadCount">{t.head_count}</DagCell></td>
              <td><DagCell table="Tables" field="OpenSeats">{t.open_seats}</DagCell></td>
              <td><DagCell table="Tables" field="Happiness">{num(t.happiness)}</DagCell></td>
              <td><DagCell table="Tables" field="Grade">{t.grade}</DagCell></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
