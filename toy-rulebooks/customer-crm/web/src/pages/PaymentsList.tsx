import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
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

const fmtMoney = (n: string | number | null) =>
  n == null
    ? "—"
    : Number(n).toLocaleString(undefined, { style: "currency", currency: "USD" });

const fmtDate = (s: string | null) => (s ? s.slice(0, 10) : "—");

export function PaymentsList() {
  const { data, loading } = useApi<Payment[]>("/api/payments");
  if (loading || !data) return <div className="loading">Loading…</div>;

  return (
    <div className="page">
      <div className="page-header">
        <h1>Payments</h1>
      </div>

      <table className="grid">
        <thead>
          <tr>
            <th>Payment #</th>
            <th>Date</th>
            <th style={{ textAlign: "right" }}>Amount</th>
            <th>Method</th>
            <th>Order #</th>
            <th>Customer</th>
            <th style={{ textAlign: "right" }}>Order total</th>
            <th style={{ textAlign: "right" }}>Order paid</th>
            <th style={{ textAlign: "right" }}>Order balance</th>
            <th>Order status</th>
          </tr>
        </thead>
        <tbody>
          {data.map((p) => (
            <tr key={p.payments_id}>
              <td>
                <Link to={`/payments/${encodeURIComponent(p.payments_id)}`}>
                  {p.payment_number}
                </Link>
              </td>
              <td>{fmtDate(p.payment_date)}</td>
              <td style={{ textAlign: "right" }}>{fmtMoney(p.amount)}</td>
              <td>{p.method}</td>
              <td>
                <Link to={`/orders/${encodeURIComponent(p.order_id)}`}>
                  {p.order_number ?? p.order_id}
                </Link>
              </td>
              <td>{p.customer_full_name ?? p.customer_email ?? "—"}</td>
              <td style={{ textAlign: "right" }}>{fmtMoney(p.order_total)}</td>
              <td style={{ textAlign: "right" }}>
                <DagCell table="Orders" field="AmountPaid">
                  {fmtMoney(p.order_amount_paid)}
                </DagCell>
              </td>
              <td style={{ textAlign: "right" }}>
                <DagCell table="Orders" field="Balance">
                  {fmtMoney(p.order_balance)}
                </DagCell>
              </td>
              <td>
                <DagCell table="Orders" field="IsPaid">
                  {p.order_is_paid ? "✅ Paid" : "⛔ Unpaid"}
                </DagCell>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <p className="muted">
        Each payment is joined to its order — click a payment's order # to add,
        edit, or delete payments and watch the order's calculated fields
        recompute via the rulebook DAG.
      </p>
    </div>
  );
}
