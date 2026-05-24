import { useState } from "react";
import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { api } from "../lib/api";
import { DagCell } from "../explainer-dag";

interface Customer {
  customers_id: string;
  name: string;
  email: string;
  first_name: string | null;
  last_name: string | null;
  full_name: string | null;
  lifetime_sales: string | null;
  unpaid_order_count: string | number | null;
  large_order_count: string | number | null;
  is_vip: boolean | null;
}

const fmtMoney = (n: string | number | null) =>
  n == null
    ? "—"
    : Number(n).toLocaleString(undefined, { style: "currency", currency: "USD" });

export function CustomersList() {
  const { data, loading, reload } = useApi<Customer[]>("/api/customers");
  const [adding, setAdding] = useState(false);
  const [form, setForm] = useState({ email: "", first_name: "", last_name: "" });

  if (loading || !data) return <div className="loading">Loading…</div>;

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    await api("/api/customers", { method: "POST", body: JSON.stringify(form) });
    setForm({ email: "", first_name: "", last_name: "" });
    setAdding(false);
    reload();
  };

  return (
    <div className="page">
      <div className="page-header">
        <h1>Customers</h1>
        <button onClick={() => setAdding((v) => !v)}>
          {adding ? "Cancel" : "+ New customer"}
        </button>
      </div>

      {adding && (
        <form className="add-form" onSubmit={submit}>
          <input
            placeholder="Email"
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            required
          />
          <input
            placeholder="First name"
            value={form.first_name}
            onChange={(e) => setForm({ ...form, first_name: e.target.value })}
          />
          <input
            placeholder="Last name"
            value={form.last_name}
            onChange={(e) => setForm({ ...form, last_name: e.target.value })}
          />
          <button type="submit">Create</button>
        </form>
      )}

      <table className="grid">
        <thead>
          <tr>
            <th>Full name</th>
            <th>First</th>
            <th>Last</th>
            <th>Email</th>
            <th style={{ textAlign: "right" }}>Lifetime sales</th>
            <th style={{ textAlign: "right" }}>Unpaid orders</th>
            <th style={{ textAlign: "right" }}>Orders &gt; $500</th>
            <th>VIP</th>
          </tr>
        </thead>
        <tbody>
          {data.map((c) => (
            <tr key={c.customers_id}>
              <td>
                <DagCell table="Customers" field="FullName">
                  <Link to={`/customers/${encodeURIComponent(c.customers_id)}`}>
                    {c.full_name}
                  </Link>
                </DagCell>
              </td>
              <td>{c.first_name}</td>
              <td>{c.last_name}</td>
              <td>{c.email}</td>
              <td style={{ textAlign: "right" }}>
                <DagCell table="Customers" field="LifetimeSales">
                  {fmtMoney(c.lifetime_sales)}
                </DagCell>
              </td>
              <td style={{ textAlign: "right" }}>
                <DagCell table="Customers" field="UnpaidOrderCount">
                  {c.unpaid_order_count ?? 0}
                </DagCell>
              </td>
              <td style={{ textAlign: "right" }}>
                <DagCell table="Customers" field="LargeOrderCount">
                  {c.large_order_count ?? 0}
                </DagCell>
              </td>
              <td>
                <DagCell table="Customers" field="IsVIP">
                  {c.is_vip ? "⭐ VIP" : ""}
                </DagCell>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
