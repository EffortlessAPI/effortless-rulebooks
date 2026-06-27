import { useState } from "react";
import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { api } from "../lib/api";
import { DagCell } from "../explainer-dag";

interface Order {
  orders_id: string;
  order_number: string;
  order_date: string;
  customer: string;
  customer_first_name: string | null;
  customer_last_name: string | null;
  customer_email: string | null;
  total: string;
  amount_paid: string | null;
  balance: string | null;
  is_paid: boolean | null;
}

interface Customer {
  customers_id: string;
  name: string;
  email: string;
  full_name: string | null;
}

const fmtMoney = (n: string | number | null) =>
  n == null
    ? "—"
    : Number(n).toLocaleString(undefined, { style: "currency", currency: "USD" });

export function OrdersList() {
  const { data, loading, reload } = useApi<Order[]>("/api/orders");
  const { data: customers } = useApi<Customer[]>("/api/customers");
  const [adding, setAdding] = useState(false);
  const today = new Date().toISOString().slice(0, 10);
  const [form, setForm] = useState({
    order_number: "",
    order_date: today,
    customer: "",
    total: "",
  });

  if (loading || !data) return <div className="loading">Loading…</div>;

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    await api("/api/orders", {
      method: "POST",
      body: JSON.stringify({ ...form, total: Number(form.total) }),
    });
    setForm({ order_number: "", order_date: today, customer: "", total: "" });
    setAdding(false);
    reload();
  };

  const remove = async (id: string) => {
    if (!confirm("Delete this order?")) return;
    await api(`/api/orders/${encodeURIComponent(id)}`, { method: "DELETE" });
    reload();
  };

  return (
    <div className="page">
      <div className="page-header">
        <h1>Orders</h1>
        <button onClick={() => setAdding((v) => !v)}>
          {adding ? "Cancel" : "+ New order"}
        </button>
      </div>

      {adding && (
        <form className="add-form" onSubmit={submit}>
          <input
            placeholder="Order number (e.g. ORD-1100)"
            value={form.order_number}
            onChange={(e) => setForm({ ...form, order_number: e.target.value })}
            required
          />
          <input
            type="date"
            value={form.order_date}
            onChange={(e) => setForm({ ...form, order_date: e.target.value })}
            required
          />
          <select
            value={form.customer}
            onChange={(e) => setForm({ ...form, customer: e.target.value })}
            required
          >
            <option value="">— customer —</option>
            {(customers ?? []).map((c) => (
              <option key={c.customers_id} value={c.name}>
                {c.full_name || c.email}
              </option>
            ))}
          </select>
          <input
            type="number"
            step="0.01"
            placeholder="Total"
            value={form.total}
            onChange={(e) => setForm({ ...form, total: e.target.value })}
            required
          />
          <button type="submit">Create</button>
        </form>
      )}

      <table className="grid">
        <thead>
          <tr>
            <th>Order #</th>
            <th>Date</th>
            <th>First</th>
            <th>Last</th>
            <th>Email</th>
            <th style={{ textAlign: "right" }}>Total</th>
            <th style={{ textAlign: "right" }}>Paid</th>
            <th style={{ textAlign: "right" }}>Balance</th>
            <th>Status</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {data.map((o) => (
            <tr key={o.orders_id}>
              <td>
                <Link to={`/orders/${encodeURIComponent(o.orders_id)}`}>
                  {o.order_number}
                </Link>
              </td>
              <td>{o.order_date}</td>
              <td>{o.customer_first_name}</td>
              <td>{o.customer_last_name}</td>
              <td>{o.customer_email}</td>
              <td style={{ textAlign: "right" }}>{fmtMoney(o.total)}</td>
              <td style={{ textAlign: "right" }}>
                <DagCell table="Orders" field="AmountPaid">
                  {fmtMoney(o.amount_paid)}
                </DagCell>
              </td>
              <td style={{ textAlign: "right" }}>
                <DagCell table="Orders" field="Balance">
                  {fmtMoney(o.balance)}
                </DagCell>
              </td>
              <td>
                <DagCell table="Orders" field="IsPaid">
                  {o.is_paid ? "✅" : "⛔"}
                </DagCell>
              </td>
              <td>
                <button className="danger" onClick={() => remove(o.orders_id)}>
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <p className="muted">
        Add or delete an order, then visit <em>Customers</em> — the{" "}
        <DagCell table="Customers" field="LifetimeSales">
          <strong>Lifetime sales</strong>
        </DagCell>{" "}
        rollup and{" "}
        <DagCell table="Customers" field="IsVIP">
          <strong>VIP</strong>
        </DagCell>{" "}
        flag recompute automatically.
      </p>
    </div>
  );
}
