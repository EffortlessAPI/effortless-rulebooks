import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { Invoice, Me } from "../types";
import { money, date } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

export default function Invoices({ me }: { me: Me }) {
  const { data, loading } = useApi<Invoice[]>("/api/invoices");
  if (loading) return <div>Loading…</div>;
  const inv = (data ?? []).filter((i) => me.role !== "client" || i.client === me.client_id);

  return (
    <>
      <h2>Invoices</h2>
      <div className="panel">
        <table>
          <thead>
            <tr><th>Invoice</th><th>Client</th><th>Issued</th><th>Due</th><th className="right">Days late</th><th className="right">Subtotal</th><th className="right">Tax</th><th className="right">Late fee</th><th className="right">Total</th><th className="right">Paid</th><th className="right">Balance</th><th>Status</th></tr>
          </thead>
          <tbody>
            {inv.map((i) => (
              <tr key={i.invoice_id}>
                <td><DagCell table="Invoices" field="Name"><Link to={`/invoices/${i.invoice_id}`}>{i.name}</Link></DagCell></td>
                <td><DagCell table="Invoices" field="ClientName">{i.client_name}</DagCell></td>
                <td><DagCell table="Invoices" field="IssueDate">{date(i.issue_date)}</DagCell></td>
                <td><DagCell table="Invoices" field="DueDate">{date(i.due_date)}</DagCell></td>
                <td className="right"><DagCell table="Invoices" field="DaysPastDue">{i.days_past_due > 0 ? i.days_past_due : "—"}</DagCell></td>
                <td className="right"><DagCell table="Invoices" field="Subtotal">{money(i.subtotal)}</DagCell></td>
                <td className="right"><DagCell table="Invoices" field="TaxAmount">{money(i.tax_amount)}</DagCell></td>
                <td className="right"><DagCell table="Invoices" field="LateFee">{Number(i.late_fee) > 0 ? money(i.late_fee) : "—"}</DagCell></td>
                <td className="right"><DagCell table="Invoices" field="Total"><b>{money(i.total)}</b></DagCell></td>
                <td className="right"><DagCell table="Invoices" field="PaidAmount">{money(i.paid_amount)}</DagCell></td>
                <td className="right"><DagCell table="Invoices" field="Balance">{money(i.balance)}</DagCell></td>
                <td><DagCell table="Invoices" field="Status"><span className={`status ${i.status}`}>{i.status}</span></DagCell></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
