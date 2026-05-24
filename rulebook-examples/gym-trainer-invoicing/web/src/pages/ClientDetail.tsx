import { Link, useParams } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { Client, Invoice, Session } from "../types";
import { money, date } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

export default function ClientDetail() {
  const { id } = useParams();
  const { data: client } = useApi<Client>(`/api/clients/${id}`);
  const { data: invoices } = useApi<Invoice[]>(`/api/invoices`);
  const { data: sessions } = useApi<Session[]>(`/api/sessions`);

  if (!client) return <div>Loading…</div>;
  const inv = (invoices ?? []).filter((i) => i.client === id);
  const sess = (sessions ?? []).filter((s) => s.client === id);

  return (
    <>
      <p><Link to="/clients">← All clients</Link></p>
      <h2><DagCell table="Clients" field="FullName">{client.full_name}</DagCell></h2>
      <p className="muted"><DagCell table="Clients" field="Email">{client.email}</DagCell> · trainer: <DagCell table="Clients" field="TrainerName">{client.trainer_name}</DagCell> (<DagCell table="Clients" field="TrainerHourlyRate">${client.trainer_hourly_rate}</DagCell>/hr)</p>

      <div className="cards">
        <div className="card"><div className="label">Sessions</div><div className="value"><DagCell table="Clients" field="SessionCount">{client.session_count}</DagCell></div></div>
        <div className="card"><div className="label">Invoiced</div><div className="value"><DagCell table="Clients" field="TotalInvoiced">{money(client.total_invoiced)}</DagCell></div></div>
        <div className="card"><div className="label">Outstanding</div><div className="value"><DagCell table="Clients" field="OutstandingBalance">{money(client.outstanding_balance)}</DagCell></div></div>
        <div className="card"><div className="label">Overdue invoices</div><div className="value"><DagCell table="Clients" field="OverdueCount">{client.overdue_count}</DagCell></div></div>
      </div>

      <div className="panel">
        <h3>Invoices</h3>
        <table>
          <thead><tr><th>Invoice</th><th>Issued</th><th>Due</th><th className="right">Days late</th><th className="right">Late fee</th><th className="right">Total</th><th className="right">Balance</th><th>Status</th></tr></thead>
          <tbody>
            {inv.map((i) => (
              <tr key={i.invoice_id}>
                <td><DagCell table="Invoices" field="Name"><Link to={`/invoices/${i.invoice_id}`}>{i.name}</Link></DagCell></td>
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
        <h3>Sessions</h3>
        <table>
          <thead><tr><th>Session</th><th>Date</th><th className="right">Hours</th><th className="right">Rate</th><th className="right">Line total</th><th>Invoice</th></tr></thead>
          <tbody>
            {sess.map((s) => (
              <tr key={s.session_id}>
                <td><DagCell table="Sessions" field="Name">{s.name}</DagCell><div className="muted" style={{ fontSize: 12 }}><DagCell table="Sessions" field="Notes">{s.notes}</DagCell></div></td>
                <td><DagCell table="Sessions" field="SessionDate">{date(s.session_date)}</DagCell></td>
                <td className="right"><DagCell table="Sessions" field="DurationHours">{s.duration_hours}</DagCell></td>
                <td className="right"><DagCell table="Sessions" field="EffectiveRate">{money(s.effective_rate)}</DagCell></td>
                <td className="right"><DagCell table="Sessions" field="LineTotal">{money(s.line_total)}</DagCell></td>
                <td><DagCell table="Sessions" field="InvoiceName">{s.invoice_name ? <Link to={`/invoices/${s.invoice}`}>{s.invoice_name}</Link> : <span className="muted">unbilled</span>}</DagCell></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
