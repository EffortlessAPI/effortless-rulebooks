import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { api } from "../lib/api";
import { DagCell } from "../explainer-dag";

interface Payment {
  payments_id: string;
  payment_number: string;
  order_id: string;
  payment_date: string;
  amount: string;
  method: string;
  order_number: string | null;
  order_date: string | null;
  order_total: string | null;
  order_amount_paid: string | null;
  order_balance: string | null;
  order_is_paid: boolean | null;
  order_customer: string | null;
  customer_first_name: string | null;
  customer_last_name: string | null;
  customer_full_name: string | null;
  customer_email: string | null;
}

const METHODS = ["card", "check", "cash", "wire", "ach"] as const;

const fmtMoney = (n: string | number | null) =>
  n == null
    ? "—"
    : Number(n).toLocaleString(undefined, { style: "currency", currency: "USD" });

export function PaymentDetail() {
  const { id = "" } = useParams();
  const navigate = useNavigate();
  const { data: payment, loading, reload } = useApi<Payment>(
    `/api/payments/${encodeURIComponent(id)}`
  );

  const [editing, setEditing] = useState(false);
  const [form, setForm] = useState({
    payment_number: "",
    payment_date: "",
    amount: "",
    method: "card",
  });

  if (loading || !payment) return <div className="loading">Loading…</div>;

  const startEdit = () => {
    setForm({
      payment_number: payment.payment_number,
      payment_date: payment.payment_date.slice(0, 10),
      amount: String(payment.amount),
      method: payment.method,
    });
    setEditing(true);
  };

  const save = async (e: React.FormEvent) => {
    e.preventDefault();
    await api(`/api/payments/${encodeURIComponent(payment.payments_id)}`, {
      method: "PATCH",
      body: JSON.stringify({
        payment_number: form.payment_number,
        payment_date: form.payment_date,
        amount: Number(form.amount),
        method: form.method,
      }),
    });
    setEditing(false);
    reload();
  };

  const remove = async () => {
    if (!confirm("Delete this payment?")) return;
    await api(`/api/payments/${encodeURIComponent(payment.payments_id)}`, {
      method: "DELETE",
    });
    navigate("/payments");
  };

  return (
    <div className="page">
      <p>
        <Link to="/payments">← Payments</Link>
      </p>

      <div className="page-header">
        <h1>Payment {payment.payment_number}</h1>
        {!editing && (
          <span>
            <button onClick={startEdit}>Edit payment</button>{" "}
            <button className="danger" onClick={remove}>
              Delete
            </button>
          </span>
        )}
      </div>

      <form className="edit-form" onSubmit={save}>
        <label>
          <span>Payment #</span>
          <input
            value={editing ? form.payment_number : payment.payment_number}
            disabled={!editing}
            onChange={(e) => setForm({ ...form, payment_number: e.target.value })}
          />
        </label>
        <label>
          <span>Date</span>
          <input
            type={editing ? "date" : "text"}
            value={editing ? form.payment_date : payment.payment_date.slice(0, 10)}
            disabled={!editing}
            onChange={(e) => setForm({ ...form, payment_date: e.target.value })}
          />
        </label>
        <label>
          <span>Amount</span>
          <input
            type={editing ? "number" : "text"}
            step="0.01"
            value={editing ? form.amount : fmtMoney(payment.amount)}
            disabled={!editing}
            onChange={(e) => setForm({ ...form, amount: e.target.value })}
          />
        </label>
        <label>
          <span>Method</span>
          {editing ? (
            <select
              value={form.method}
              onChange={(e) => setForm({ ...form, method: e.target.value })}
            >
              {METHODS.map((m) => (
                <option key={m} value={m}>
                  {m}
                </option>
              ))}
            </select>
          ) : (
            <input value={payment.method} disabled />
          )}
        </label>

        <label>
          <span>Order</span>
          <Link
            to={`/orders/${encodeURIComponent(payment.order_id)}`}
            className="field-link"
          >
            {payment.order_number ?? payment.order_id}
          </Link>
        </label>
        <label>
          <span>Customer</span>
          {payment.order_customer ? (
            <Link
              to={`/customers/${encodeURIComponent(payment.order_customer)}`}
              className="field-link"
            >
              {payment.customer_full_name ?? payment.customer_email ?? payment.order_customer}
            </Link>
          ) : (
            <input value="—" disabled />
          )}
        </label>

        <div className="calculated">
          <span>Order amount paid (aggregated)</span>
          <DagCell table="Orders" field="AmountPaid">
            <strong>{fmtMoney(payment.order_amount_paid)}</strong>
          </DagCell>
        </div>
        <div className="calculated">
          <span>Order balance (calculated)</span>
          <DagCell table="Orders" field="Balance">
            <strong>{fmtMoney(payment.order_balance)}</strong>
          </DagCell>
        </div>
        <div className="calculated">
          <span>Order paid in full (calculated)</span>
          <DagCell table="Orders" field="IsPaid">
            <strong>{payment.order_is_paid ? "✅ Paid" : "⛔ Unpaid"}</strong>
          </DagCell>
        </div>

        {editing && (
          <div className="form-actions">
            <button type="submit">Save</button>{" "}
            <button type="button" onClick={() => setEditing(false)}>
              Cancel
            </button>
          </div>
        )}
      </form>
    </div>
  );
}
