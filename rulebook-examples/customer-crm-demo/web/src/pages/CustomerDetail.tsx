import { useEffect, useState } from "react";
import { useNavigate, useParams, Link } from "react-router-dom";
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

interface Order {
  orders_id: string;
  order_number: string;
  order_date: string;
  total: string;
}

const fmtMoney = (n: string | number | null) =>
  n == null
    ? "—"
    : Number(n).toLocaleString(undefined, { style: "currency", currency: "USD" });

export function CustomerDetail() {
  const { id = "" } = useParams();
  const path = `/api/customers/${encodeURIComponent(id)}`;
  const { data, loading, reload } = useApi<Customer>(path);
  const ordersPath = data?.name
    ? `/api/orders?customer=${encodeURIComponent(data.name)}`
    : null;
  const { data: orders } = useApi<Order[]>(ordersPath);
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: "", first_name: "", last_name: "" });

  useEffect(() => {
    if (data)
      setForm({
        email: data.email || "",
        first_name: data.first_name || "",
        last_name: data.last_name || "",
      });
  }, [data]);

  if (loading || !data) return <div className="loading">Loading…</div>;

  const save = async (e: React.FormEvent) => {
    e.preventDefault();
    await api(path, { method: "PATCH", body: JSON.stringify(form) });
    reload();
  };

  const remove = async () => {
    if (!confirm("Delete this customer?")) return;
    await api(path, { method: "DELETE" });
    navigate("/customers");
  };

  return (
    <div className="page">
      <p>
        <Link to="/customers">← Customers</Link>
      </p>
      <h1>
        <DagCell table="Customers" field="FullName">
          {data.full_name}
        </DagCell>
      </h1>

      <form className="edit-form" onSubmit={save}>
        <label>
          <span>First name</span>
          <input
            value={form.first_name}
            onChange={(e) => setForm({ ...form, first_name: e.target.value })}
          />
        </label>
        <label>
          <span>Last name</span>
          <input
            value={form.last_name}
            onChange={(e) => setForm({ ...form, last_name: e.target.value })}
          />
        </label>
        <label>
          <span>Email</span>
          <input
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
          />
        </label>

        <div className="calculated">
          <span>Full name (calculated)</span>
          <DagCell table="Customers" field="FullName">
            <strong>{data.full_name}</strong>
          </DagCell>
        </div>

        <div className="calculated">
          <span>Lifetime sales (aggregated)</span>
          <DagCell table="Customers" field="LifetimeSales">
            <strong>{fmtMoney(data.lifetime_sales)}</strong>
          </DagCell>
        </div>

        <div className="calculated">
          <span>Unpaid orders (aggregated)</span>
          <DagCell table="Customers" field="UnpaidOrderCount">
            <strong>{data.unpaid_order_count ?? 0}</strong>
          </DagCell>
        </div>

        <div className="calculated">
          <span>Orders over $500 (aggregated)</span>
          <DagCell table="Customers" field="LargeOrderCount">
            <strong>{data.large_order_count ?? 0}</strong>
          </DagCell>
        </div>

        <div className="calculated">
          <span>VIP (calculated)</span>
          <DagCell table="Customers" field="IsVIP">
            <strong>{data.is_vip ? "⭐ VIP" : "no"}</strong>
          </DagCell>
        </div>

        <div className="form-actions">
          <button type="submit">Save</button>
          <button type="button" className="danger" onClick={remove}>
            Delete
          </button>
        </div>
      </form>

      <h2>Orders</h2>
      {orders && orders.length > 0 ? (
        <table className="grid">
          <thead>
            <tr>
              <th>Order #</th>
              <th>Date</th>
              <th style={{ textAlign: "right" }}>Total</th>
            </tr>
          </thead>
          <tbody>
            {orders.map((o) => (
              <tr key={o.orders_id}>
                <td>
                  <Link to={`/orders/${encodeURIComponent(o.orders_id)}`}>
                    {o.order_number}
                  </Link>
                </td>
                <td>{o.order_date}</td>
                <td style={{ textAlign: "right" }}>{fmtMoney(o.total)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p className="muted">No orders yet.</p>
      )}

      <p className="muted">
        Edit First or Last name, hit Save, and watch the calculated Full
        name update — it's derived by the rulebook, not stored. Toggle{" "}
        <em>Explain</em> in the top-right and click the Full name to see
        the inference graph.
      </p>
    </div>
  );
}
