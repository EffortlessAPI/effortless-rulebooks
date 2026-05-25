import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { Client, Invoice, Me } from "../types";
import { money, date } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

export default function Dashboard({ me }: { me: Me }) {
  const { data: clients } = useApi<Client[]>("/api/clients");
  const { data: invoices } = useApi<Invoice[]>("/api/invoices");

  const cs = clients ?? [];
  const inv = invoices ?? [];

  const outstanding = inv.reduce((a, i) => a + Number(i.balance || 0), 0);
  const overdueTotal = inv.filter((i) => i.status === "Overdue").reduce((a, i) => a + Number(i.balance || 0), 0);
  const billed = inv.reduce((a, i) => a + Number(i.total || 0), 0);
  const sessionCount = cs.reduce((a, c) => a + Number(c.session_count || 0), 0);

  return (
    <>
      <h2>Hi, {me.display_name.split(" ")[0]}</h2>
      <div className="cards">
        <div className="card"><div className="label">Active clients</div><div className="value">{cs.length}</div></div>
        <div className="card"><div className="label">Sessions logged</div><div className="value">{sessionCount}</div></div>
        <div className="card"><div className="label">Total billed</div><div className="value">{money(billed)}</div></div>
        <div className="card"><div className="label">Outstanding</div><div className="value">{money(outstanding)}</div></div>
        <div className="card"><div className="label">Overdue</div><div className="value" style={{ color: overdueTotal > 0 ? "#b3261e" : undefined }}>{money(overdueTotal)}</div></div>
      </div>

      <div className="panel">
        <h3>Recent invoices</h3>
        <table>
          <thead>
            <tr><th>Invoice</th><th>Client</th><th>Issued</th><th>Due</th><th className="right">Days late</th><th className="right">Late fee</th><th className="right">Total</th><th className="right">Balance</th><th>Status</th></tr>
          </thead>
          <tbody>
            {inv.slice(0, 8).map((i) => (
              <tr key={i.invoice_id}>
                <td><DagCell table="Invoices" field="Name"><Link to={`/invoices/${i.invoice_id}`}>{i.name}</Link></DagCell></td>
                <td><DagCell table="Invoices" field="ClientName">{i.client_name}</DagCell></td>
                <td><DagCell table="Invoices" field="IssueDate">{date(i.issue_date)}</DagCell></td>
                <td><DagCell table="Invoices" field="DueDate">{date(i.due_date)}</DagCell></td>
                <td className="right" style={{ color: i.days_past_due > 15 ? "#b3261e" : undefined }}><DagCell table="Invoices" field="DaysPastDue">{i.days_past_due > 0 ? i.days_past_due : "—"}</DagCell></td>
                <td className="right"><DagCell table="Invoices" field="LateFee">{Number(i.late_fee) > 0 ? money(i.late_fee) : "—"}</DagCell></td>
                <td className="right"><DagCell table="Invoices" field="Total">{money(i.total)}</DagCell></td>
                <td className="right"><DagCell table="Invoices" field="Balance">{money(i.balance)}</DagCell></td>
                <td><DagCell table="Invoices" field="Status"><span className={`status ${i.status}`}>{i.status}</span></DagCell></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="panel">
        <h3>Clients</h3>
        <table>
          <thead>
            <tr><th>Client</th><th className="right">Sessions</th><th className="right">Invoiced</th><th className="right">Outstanding</th><th>Status</th></tr>
          </thead>
          <tbody>
            {cs.map((c) => (
              <tr key={c.client_id}>
                <td><DagCell table="Clients" field="FullName"><Link to={`/clients/${c.client_id}`}>{c.full_name}</Link></DagCell><div className="muted" style={{ fontSize: 12 }}><DagCell table="Clients" field="Email">{c.email}</DagCell></div></td>
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
