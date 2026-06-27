import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { Client } from "../types";
import { money } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

export default function Clients() {
  const { data, loading } = useApi<Client[]>("/api/clients");
  if (loading) return <div>Loading…</div>;
  const cs = data ?? [];
  return (
    <>
      <h2>Clients</h2>
      <div className="panel">
        <table>
          <thead>
            <tr><th>Client</th><th>Email</th><th className="right">Sessions</th><th className="right">Invoiced</th><th className="right">Outstanding</th><th>Status</th></tr>
          </thead>
          <tbody>
            {cs.map((c) => (
              <tr key={c.client_id}>
                <td><DagCell table="Clients" field="FullName"><Link to={`/clients/${c.client_id}`}>{c.full_name}</Link></DagCell></td>
                <td className="muted"><DagCell table="Clients" field="Email">{c.email}</DagCell></td>
                <td className="right"><DagCell table="Clients" field="SessionCount">{c.session_count}</DagCell></td>
                <td className="right"><DagCell table="Clients" field="TotalInvoiced">{money(c.total_invoiced)}</DagCell></td>
                <td className="right"><DagCell table="Clients" field="OutstandingBalance">{money(c.outstanding_balance)}</DagCell></td>
                <td><DagCell table="Clients" field="Status"><span className={`status ${c.status.replace(/ /g, "")}`}>{c.status}</span></DagCell></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
