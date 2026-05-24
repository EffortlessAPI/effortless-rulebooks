import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { Invoice, Me, Session } from "../types";
import { api } from "../lib/api";
import { money, date, dateInput } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

export default function InvoiceDetail({ me }: { me: Me }) {
  const { id } = useParams();
  const { data, loading, reload } = useApi<{ invoice: Invoice; sessions: Session[] }>(`/api/invoices/${id}`);
  const [editing, setEditing] = useState(false);

  if (loading || !data) return <div>Loading…</div>;
  const { invoice: i, sessions } = data;
  const canEdit = me.role === "trainer";

  return (
    <>
      <p><Link to="/invoices">← All invoices</Link></p>
      <h2><DagCell table="Invoices" field="Name">{i.name}</DagCell> — <DagCell table="Invoices" field="ClientName">{i.client_name}</DagCell> <DagCell table="Invoices" field="Status"><span className={`status ${i.status}`} style={{ verticalAlign: "middle", marginLeft: 8 }}>{i.status}</span></DagCell></h2>
      <p className="muted">trainer: <DagCell table="Invoices" field="TrainerName">{i.trainer_name}</DagCell> · issued <DagCell table="Invoices" field="IssueDate">{date(i.issue_date)}</DagCell> · due <DagCell table="Invoices" field="DueDate">{date(i.due_date)}</DagCell></p>

      <div className="row">
        <div className="col panel">
          <h3>Totals (calculated)</h3>
          <table>
            <tbody>
              <tr><td>Subtotal (sum of sessions)</td><td className="right"><DagCell table="Invoices" field="Subtotal">{money(i.subtotal)}</DagCell></td></tr>
              <tr><td>Tax (<DagCell table="Invoices" field="TaxRate">{(Number(i.tax_rate) * 100).toFixed(1)}%</DagCell>)</td><td className="right"><DagCell table="Invoices" field="TaxAmount">{money(i.tax_amount)}</DagCell></td></tr>
              {i.days_past_due > 0 && (
                <tr>
                  <td>Late fee <span className="muted">(<DagCell table="Invoices" field="DaysPastDue">{i.days_past_due} days past due</DagCell>{Number(i.late_fee) > 0 ? "" : ", ≤15"})</span></td>
                  <td className="right" style={{ color: Number(i.late_fee) > 0 ? "#b3261e" : undefined }}><DagCell table="Invoices" field="LateFee">{money(i.late_fee)}</DagCell></td>
                </tr>
              )}
              <tr><td>Discount</td><td className="right"><DagCell table="Invoices" field="DiscountAmount">−{money(i.discount_amount)}</DagCell></td></tr>
              <tr><td><b>Total</b></td><td className="right"><DagCell table="Invoices" field="Total"><b>{money(i.total)}</b></DagCell></td></tr>
              <tr><td>Paid</td><td className="right"><DagCell table="Invoices" field="PaidAmount">{money(i.paid_amount)}</DagCell></td></tr>
              <tr><td><b>Balance</b></td><td className="right"><DagCell table="Invoices" field="Balance"><b>{money(i.balance)}</b></DagCell></td></tr>
            </tbody>
          </table>
        </div>

        <div className="col panel">
          <h3>Editable (raw fields)</h3>
          {canEdit && !editing && <button className="primary" onClick={() => setEditing(true)}>Edit invoice</button>}
          {!canEdit && <p className="muted">Read-only for {me.role} role.</p>}
          {editing && <EditForm invoice={i} onDone={() => { setEditing(false); reload(); }} onCancel={() => setEditing(false)} />}
        </div>
      </div>

      <div className="panel">
        <h3>Sessions on this invoice</h3>
        <table>
          <thead><tr><th>Session</th><th>Date</th><th className="right">Hours</th><th className="right">Rate</th><th className="right">Line total</th></tr></thead>
          <tbody>
            {sessions.map((s) => (
              <tr key={s.session_id}>
                <td><DagCell table="Sessions" field="Name">{s.name}</DagCell><div className="muted" style={{ fontSize: 12 }}><DagCell table="Sessions" field="Notes">{s.notes}</DagCell></div></td>
                <td><DagCell table="Sessions" field="SessionDate">{date(s.session_date)}</DagCell></td>
                <td className="right"><DagCell table="Sessions" field="DurationHours">{s.duration_hours}</DagCell></td>
                <td className="right"><DagCell table="Sessions" field="EffectiveRate">{money(s.effective_rate)}</DagCell></td>
                <td className="right"><DagCell table="Sessions" field="LineTotal">{money(s.line_total)}</DagCell></td>
              </tr>
            ))}
            <tr><td colSpan={4} className="right"><b>Subtotal</b></td><td className="right"><b>{money(i.subtotal)}</b></td></tr>
          </tbody>
        </table>
      </div>
    </>
  );
}

function EditForm({ invoice, onDone, onCancel }: { invoice: Invoice; onDone: () => void; onCancel: () => void }) {
  const [tax, setTax] = useState(String(invoice.tax_rate));
  const [discount, setDiscount] = useState(String(invoice.discount_amount));
  const [paid, setPaid] = useState(String(invoice.paid_amount));
  const [issue, setIssue] = useState(dateInput(invoice.issue_date));
  const [due, setDue] = useState(dateInput(invoice.due_date));
  const [busy, setBusy] = useState(false);

  const save = async () => {
    setBusy(true);
    try {
      await api(`/api/invoices/${invoice.invoice_id}`, {
        method: "PATCH",
        body: JSON.stringify({
          tax_rate: Number(tax),
          discount_amount: Number(discount),
          paid_amount: Number(paid),
          issue_date: issue,
          due_date: due,
        }),
      });
      onDone();
    } catch (e: any) {
      alert(e.message);
      setBusy(false);
    }
  };

  return (
    <div>
      <div className="field"><label>Issue date</label><input type="date" value={issue} onChange={(e) => setIssue(e.target.value)} /></div>
      <div className="field"><label>Due date</label><input type="date" value={due} onChange={(e) => setDue(e.target.value)} /></div>
      <div className="field"><label>Tax rate (decimal, e.g. 0.08)</label><input value={tax} onChange={(e) => setTax(e.target.value)} /></div>
      <div className="field"><label>Discount amount</label><input value={discount} onChange={(e) => setDiscount(e.target.value)} /></div>
      <div className="field"><label>Paid amount</label><input value={paid} onChange={(e) => setPaid(e.target.value)} /></div>
      <div className="actions">
        <button className="primary" onClick={save} disabled={busy}>Save</button>
        <button onClick={onCancel} disabled={busy}>Cancel</button>
      </div>
    </div>
  );
}
