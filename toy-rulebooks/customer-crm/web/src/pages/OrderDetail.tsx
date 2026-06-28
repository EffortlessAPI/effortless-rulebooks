import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { api } from "../lib/api";
import { DagCell } from "../explainer-dag";

interface Order {
  orders_id: string;
  name: string;
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
  fcs_subtotal: string | null;
  fcs_unit_count: string | null;
}

interface OrderLine {
  order_lines_id: string;
  line_number: string;
  order_id: string;
  fcs_id: string;
  quantity: string;
  fcs_unit_price: string | null;
  fcs_architecture: string | null;
  fcs_jet_model: string | null;
  fcs_meets_fifth_gen_spec: boolean | null;
  line_total: string | null;
}

interface FCSOption {
  flight_control_systems_id: string;
  fcs_code: string;
  jet_model_id: string;
  unit_price: string;
}

interface CustomerOption {
  customers_id: string;
  name: string;
  email: string;
  first_name: string | null;
  last_name: string | null;
}

interface Payment {
  payments_id: string;
  name: string;
  payment_number: string;
  order_id: string;
  payment_date: string;
  amount: string;
  method: string;
}

const METHODS = ["card", "check", "cash", "wire", "ach"] as const;

const fmtMoney = (n: string | number | null) =>
  n == null
    ? "—"
    : Number(n).toLocaleString(undefined, { style: "currency", currency: "USD" });

const newPaymentNumber = () => `PMT-${Date.now().toString().slice(-6)}`;
const newLineNumber = () => `OL-${Date.now().toString().slice(-6)}`;
const fmtM = (n: string | number | null) =>
  n == null ? "—" : `$${Number(n).toFixed(1)}M`;

export function OrderDetail() {
  const { id = "" } = useParams();
  const orderPath = `/api/orders/${encodeURIComponent(id)}`;
  const { data: order, loading, reload } = useApi<Order>(orderPath);
  const paymentsPath = order ? `/api/payments?order_id=${encodeURIComponent(order.name)}` : null;
  const { data: payments, reload: reloadPayments } = useApi<Payment[]>(paymentsPath);
  const { data: customers } = useApi<CustomerOption[]>("/api/customers");
  const linesPath = order ? `/api/order-lines?order_id=${encodeURIComponent(order.name)}` : null;
  const { data: lines, reload: reloadLines } = useApi<OrderLine[]>(linesPath);
  const { data: fcsOptions } = useApi<FCSOption[]>("/api/flight-control-systems");
  const [addingLine, setAddingLine] = useState(false);
  const [lineForm, setLineForm] = useState({ line_number: "", fcs_id: "", quantity: "1" });

  const [editingOrder, setEditingOrder] = useState(false);
  const [orderForm, setOrderForm] = useState({
    order_number: "",
    order_date: "",
    customer: "",
    total: "",
  });

  const today = new Date().toISOString().slice(0, 10);
  const [adding, setAdding] = useState(false);
  const [form, setForm] = useState({
    payment_number: "",
    payment_date: today,
    amount: "",
    method: "card",
  });
  const [editId, setEditId] = useState<string | null>(null);
  const [editForm, setEditForm] = useState({
    payment_number: "",
    payment_date: "",
    amount: "",
    method: "card",
  });

  if (loading || !order) return <div className="loading">Loading…</div>;

  const reloadAll = () => {
    reload();
    reloadPayments();
    reloadLines();
  };

  const submitLine = async (e: React.FormEvent) => {
    e.preventDefault();
    await api("/api/order-lines", {
      method: "POST",
      body: JSON.stringify({
        line_number: lineForm.line_number || newLineNumber(),
        order_id: order.name,
        fcs_id: lineForm.fcs_id,
        quantity: Number(lineForm.quantity),
      }),
    });
    setLineForm({ line_number: "", fcs_id: "", quantity: "1" });
    setAddingLine(false);
    reloadAll();
  };

  const removeLine = async (lid: string) => {
    if (!confirm("Delete this FCS line?")) return;
    await api(`/api/order-lines/${encodeURIComponent(lid)}`, { method: "DELETE" });
    reloadAll();
  };

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    await api("/api/payments", {
      method: "POST",
      body: JSON.stringify({
        payment_number: form.payment_number || newPaymentNumber(),
        order_id: order.name,
        payment_date: form.payment_date,
        amount: Number(form.amount),
        method: form.method,
      }),
    });
    setForm({ payment_number: "", payment_date: today, amount: "", method: "card" });
    setAdding(false);
    reloadAll();
  };

  const startEdit = (p: Payment) => {
    setEditId(p.payments_id);
    setEditForm({
      payment_number: p.payment_number,
      payment_date: p.payment_date,
      amount: String(p.amount),
      method: p.method,
    });
  };

  const saveEdit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editId) return;
    await api(`/api/payments/${encodeURIComponent(editId)}`, {
      method: "PATCH",
      body: JSON.stringify({
        payment_number: editForm.payment_number,
        payment_date: editForm.payment_date,
        amount: Number(editForm.amount),
        method: editForm.method,
      }),
    });
    setEditId(null);
    reloadAll();
  };

  const startEditOrder = () => {
    setOrderForm({
      order_number: order.order_number,
      order_date: order.order_date,
      customer: order.customer,
      total: String(order.total),
    });
    setEditingOrder(true);
  };

  const saveOrder = async (e: React.FormEvent) => {
    e.preventDefault();
    await api(`/api/orders/${encodeURIComponent(order.orders_id)}`, {
      method: "PATCH",
      body: JSON.stringify({
        order_number: orderForm.order_number,
        order_date: orderForm.order_date,
        customer: orderForm.customer,
        total: Number(orderForm.total),
      }),
    });
    setEditingOrder(false);
    reloadAll();
  };

  const remove = async (pid: string) => {
    if (!confirm("Delete this payment?")) return;
    await api(`/api/payments/${encodeURIComponent(pid)}`, { method: "DELETE" });
    reloadAll();
  };

  return (
    <div className="page">
      <p>
        <Link to="/orders">← Orders</Link>
      </p>
      <div className="page-header">
        <h1>Order {order.order_number}</h1>
        {!editingOrder && <button onClick={startEditOrder}>Edit order</button>}
      </div>

      <form className="edit-form" onSubmit={saveOrder}>
        <label>
          <span>Order #</span>
          <input
            value={editingOrder ? orderForm.order_number : order.order_number}
            disabled={!editingOrder}
            onChange={(e) => setOrderForm({ ...orderForm, order_number: e.target.value })}
          />
        </label>
        <label>
          <span>Date</span>
          <input
            type={editingOrder ? "date" : "text"}
            value={editingOrder ? orderForm.order_date : order.order_date}
            disabled={!editingOrder}
            onChange={(e) => setOrderForm({ ...orderForm, order_date: e.target.value })}
          />
        </label>
        <label>
          <span>Customer</span>
          {editingOrder ? (
            <select
              value={orderForm.customer}
              onChange={(e) => setOrderForm({ ...orderForm, customer: e.target.value })}
            >
              {(customers ?? []).map((c) => (
                <option key={c.customers_id} value={c.name}>
                  {c.first_name} {c.last_name} ({c.email})
                </option>
              ))}
            </select>
          ) : (
            <Link
              to={`/customers/${encodeURIComponent(order.customer)}`}
              className="field-link"
            >
              {order.customer_first_name} {order.customer_last_name} ({order.customer_email})
            </Link>
          )}
        </label>
        <label>
          <span>Total</span>
          <input
            type={editingOrder ? "number" : "text"}
            step="0.01"
            value={editingOrder ? orderForm.total : fmtMoney(order.total)}
            disabled={!editingOrder}
            onChange={(e) => setOrderForm({ ...orderForm, total: e.target.value })}
          />
        </label>

        {editingOrder && (
          <div className="form-actions">
            <button type="submit">Save</button>{" "}
            <button type="button" onClick={() => setEditingOrder(false)}>Cancel</button>
          </div>
        )}

        <div className="calculated">
          <span>Amount paid (aggregated)</span>
          <DagCell table="Orders" field="AmountPaid">
            <strong>{fmtMoney(order.amount_paid)}</strong>
          </DagCell>
        </div>

        <div className="calculated">
          <span>Balance (calculated)</span>
          <DagCell table="Orders" field="Balance">
            <strong>{fmtMoney(order.balance)}</strong>
          </DagCell>
        </div>

        <div className="calculated">
          <span>Paid in full (calculated)</span>
          <DagCell table="Orders" field="IsPaid">
            <strong>{order.is_paid ? "✅ Paid" : "⛔ Unpaid"}</strong>
          </DagCell>
        </div>

        <div className="calculated">
          <span>FCS subtotal (aggregated, $M)</span>
          <DagCell table="Orders" field="FCSSubtotal">
            <strong>{fmtM(order.fcs_subtotal)}</strong>
          </DagCell>
        </div>

        <div className="calculated">
          <span>FCS unit count (aggregated)</span>
          <DagCell table="Orders" field="FCSUnitCount">
            <strong>{order.fcs_unit_count ?? 0}</strong>
          </DagCell>
        </div>
      </form>

      <div className="page-header" style={{ marginTop: 24 }}>
        <h2>FCS line items</h2>
        <button onClick={() => setAddingLine((v) => !v)}>
          {addingLine ? "Cancel" : "+ New FCS line"}
        </button>
      </div>

      {addingLine && (
        <form className="add-form" onSubmit={submitLine}>
          <input
            placeholder="Line # (auto if blank)"
            value={lineForm.line_number}
            onChange={(e) => setLineForm({ ...lineForm, line_number: e.target.value })}
          />
          <select
            value={lineForm.fcs_id}
            onChange={(e) => setLineForm({ ...lineForm, fcs_id: e.target.value })}
            required
          >
            <option value="">— FCS variant —</option>
            {(fcsOptions ?? []).map((f) => (
              <option key={f.flight_control_systems_id} value={f.fcs_code}>
                {f.fcs_code} ({f.jet_model_id}, {fmtM(f.unit_price)})
              </option>
            ))}
          </select>
          <input
            type="number"
            min="1"
            placeholder="Quantity"
            value={lineForm.quantity}
            onChange={(e) => setLineForm({ ...lineForm, quantity: e.target.value })}
            required
          />
          <button type="submit">Add line</button>
        </form>
      )}

      {lines && lines.length > 0 ? (
        <table className="grid">
          <thead>
            <tr>
              <th>Line #</th>
              <th>FCS</th>
              <th>Jet</th>
              <th>Architecture</th>
              <th style={{ textAlign: "right" }}>Qty</th>
              <th style={{ textAlign: "right" }}>Unit price</th>
              <th style={{ textAlign: "right" }}>Line total</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {lines.map((l) => (
              <tr key={l.order_lines_id}>
                <td>{l.line_number}</td>
                <td>
                  <Link
                    to={`/flight-control-systems/${encodeURIComponent(l.fcs_id)}`}
                  >
                    {l.fcs_id}
                  </Link>
                </td>
                <td>{l.fcs_jet_model}</td>
                <td>{l.fcs_architecture}</td>
                <td style={{ textAlign: "right" }}>{l.quantity}</td>
                <td style={{ textAlign: "right" }}>{fmtM(l.fcs_unit_price)}</td>
                <td style={{ textAlign: "right" }}>
                  <DagCell table="OrderLines" field="LineTotal">
                    <strong>{fmtM(l.line_total)}</strong>
                  </DagCell>
                </td>
                <td>
                  <button className="danger" onClick={() => removeLine(l.order_lines_id)}>
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p className="muted">No FCS line items on this order yet.</p>
      )}

      <div className="page-header" style={{ marginTop: 24 }}>
        <h2>Payments</h2>
        <button onClick={() => setAdding((v) => !v)}>
          {adding ? "Cancel" : "+ New payment"}
        </button>
      </div>

      {adding && (
        <form className="add-form" onSubmit={submit}>
          <input
            placeholder="Payment # (auto if blank)"
            value={form.payment_number}
            onChange={(e) => setForm({ ...form, payment_number: e.target.value })}
          />
          <input
            type="date"
            value={form.payment_date}
            onChange={(e) => setForm({ ...form, payment_date: e.target.value })}
            required
          />
          <input
            type="number"
            step="0.01"
            placeholder="Amount"
            value={form.amount}
            onChange={(e) => setForm({ ...form, amount: e.target.value })}
            required
          />
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
          <button type="submit">Record</button>
        </form>
      )}

      {payments && payments.length > 0 ? (
        <table className="grid">
          <thead>
            <tr>
              <th>Payment #</th>
              <th>Date</th>
              <th style={{ textAlign: "right" }}>Amount</th>
              <th>Method</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {payments.map((p) =>
              editId === p.payments_id ? (
                <tr key={p.payments_id}>
                  <td>
                    <input
                      value={editForm.payment_number}
                      onChange={(e) =>
                        setEditForm({ ...editForm, payment_number: e.target.value })
                      }
                    />
                  </td>
                  <td>
                    <input
                      type="date"
                      value={editForm.payment_date}
                      onChange={(e) =>
                        setEditForm({ ...editForm, payment_date: e.target.value })
                      }
                    />
                  </td>
                  <td style={{ textAlign: "right" }}>
                    <input
                      type="number"
                      step="0.01"
                      value={editForm.amount}
                      onChange={(e) =>
                        setEditForm({ ...editForm, amount: e.target.value })
                      }
                    />
                  </td>
                  <td>
                    <select
                      value={editForm.method}
                      onChange={(e) =>
                        setEditForm({ ...editForm, method: e.target.value })
                      }
                    >
                      {METHODS.map((m) => (
                        <option key={m} value={m}>
                          {m}
                        </option>
                      ))}
                    </select>
                  </td>
                  <td>
                    <button onClick={saveEdit}>Save</button>{" "}
                    <button onClick={() => setEditId(null)}>Cancel</button>
                  </td>
                </tr>
              ) : (
                <tr key={p.payments_id}>
                  <td>
                    <Link to={`/payments/${encodeURIComponent(p.payments_id)}`}>
                      {p.payment_number}
                    </Link>
                  </td>
                  <td>{p.payment_date}</td>
                  <td style={{ textAlign: "right" }}>{fmtMoney(p.amount)}</td>
                  <td>{p.method}</td>
                  <td>
                    <button onClick={() => startEdit(p)}>Edit</button>{" "}
                    <button className="danger" onClick={() => remove(p.payments_id)}>
                      Delete
                    </button>
                  </td>
                </tr>
              )
            )}
          </tbody>
        </table>
      ) : (
        <p className="muted">No payments recorded for this order yet.</p>
      )}

      <p className="muted">
        Add, edit, or delete payments — the order's{" "}
        <DagCell table="Orders" field="AmountPaid">
          <strong>amount paid</strong>
        </DagCell>
        ,{" "}
        <DagCell table="Orders" field="Balance">
          <strong>balance</strong>
        </DagCell>
        , and{" "}
        <DagCell table="Orders" field="IsPaid">
          <strong>paid-in-full</strong>
        </DagCell>{" "}
        flag recompute automatically.
      </p>
    </div>
  );
}
