import { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import {
  fetchInvoice, deleteInvoice,
  fetchProducts,
  createInvoiceLineItem, updateInvoiceLineItem, deleteInvoiceLineItem,
  createPayment, updatePayment, deletePayment,
} from '../api.js';

const fmt = (n) => Number(n ?? 0).toLocaleString('en-US', { style: 'currency', currency: 'USD' });
const fmtDate = (d) => d ? new Date(d).toLocaleDateString() : '—';
const pct = (n) => `${(Number(n) * 100).toFixed(0)}%`;

function PayLabel({ label }) {
  const color = label === 'Paid' ? '#27ae60' : label === 'Unpaid' ? '#c0392b' : '#e67e22';
  return <span style={{ color, fontWeight: 700 }}>{label}</span>;
}

function Totals({ invoice }) {
  return (
    <div className="order-totals">
      <div className="totals-row"><span>Subtotal</span><span>{fmt(invoice.subtotal)}</span></div>
      <div className="totals-row"><span>Tax ({pct(invoice.tax_rate)})</span><span>{fmt(invoice.tax_amount)}</span></div>
      <div className="totals-row totals-total"><span>Invoice Total</span><span>{fmt(invoice.invoice_total)}</span></div>
      <div className="totals-row"><span>Total Paid</span><span style={{ color: '#27ae60' }}>{fmt(invoice.total_paid)}</span></div>
      <div className="totals-row totals-due" style={{ color: invoice.is_paid_in_full ? '#27ae60' : '#c0392b' }}>
        <span>Amount Due</span><span>{fmt(invoice.amount_due)}</span>
      </div>
      <div className="totals-row">
        <span>Payment Status</span><span><PayLabel label={invoice.payment_status_label} /></span>
      </div>
    </div>
  );
}

// ── Add/Edit Line Item form ────────────────────────────────────────────────────

function LineItemForm({ invoiceId, existing, products, onDone, onCancel }) {
  const isEdit = Boolean(existing);
  const [form, setForm] = useState({
    product_id: existing?.product_id ? String(existing.product_id) : '',
    quantity: existing ? String(existing.quantity) : '1',
    unit_price: existing ? String(existing.unit_price) : '',
    notes: existing?.notes || '',
  });
  const [saving, setSaving] = useState(false);
  const [error, setError]   = useState('');

  const set = (k) => (e) => setForm((f) => ({ ...f, [k]: e.target.value }));

  // Auto-fill unit_price from selected product
  const handleProductChange = (e) => {
    const pid = e.target.value;
    const prod = products.find((p) => String(p.id) === pid);
    setForm((f) => ({
      ...f,
      product_id: pid,
      unit_price: prod ? String(prod.unit_price) : f.unit_price,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.product_id) { setError('Product is required.'); return; }
    setSaving(true);
    setError('');
    try {
      const payload = {
        invoice_id: invoiceId,
        product_id: parseInt(form.product_id, 10),
        quantity: parseInt(form.quantity, 10) || 1,
        unit_price: parseFloat(form.unit_price) || 0,
        notes: form.notes,
      };
      if (isEdit) {
        await updateInvoiceLineItem(existing.id, payload);
      } else {
        await createInvoiceLineItem(payload);
      }
      onDone();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <tr className="inline-form-row">
      <td colSpan={8}>
        <form onSubmit={handleSubmit} style={{ display: 'flex', gap: 8, flexWrap: 'wrap', alignItems: 'flex-end', padding: '8px 4px' }}>
          <div style={{ flex: '2 1 180px' }}>
            <label style={{ fontSize: 11, color: '#888', display: 'block', marginBottom: 2 }}>Product</label>
            <select value={form.product_id} onChange={handleProductChange} required style={{ fontSize: 13 }}>
              <option value="">— Select —</option>
              {products.filter(p => p.is_active || (existing && p.id === existing.product_id)).map((p) => (
                <option key={p.id} value={String(p.id)}>{p.sku} — {p.display_name}</option>
              ))}
            </select>
          </div>
          <div style={{ flex: '0 0 70px' }}>
            <label style={{ fontSize: 11, color: '#888', display: 'block', marginBottom: 2 }}>Qty</label>
            <input type="number" min="1" value={form.quantity} onChange={set('quantity')} style={{ fontSize: 13 }} />
          </div>
          <div style={{ flex: '0 0 100px' }}>
            <label style={{ fontSize: 11, color: '#888', display: 'block', marginBottom: 2 }}>Unit Price ($)</label>
            <input type="number" step="0.01" min="0" value={form.unit_price} onChange={set('unit_price')} style={{ fontSize: 13 }} />
          </div>
          <div style={{ flex: '1 1 120px' }}>
            <label style={{ fontSize: 11, color: '#888', display: 'block', marginBottom: 2 }}>Notes</label>
            <input value={form.notes} onChange={set('notes')} style={{ fontSize: 13 }} />
          </div>
          <div style={{ display: 'flex', gap: 6 }}>
            <button type="submit" className="btn-primary btn-sm" disabled={saving}>
              {saving ? '…' : isEdit ? 'Save' : 'Add'}
            </button>
            <button type="button" className="btn-secondary btn-sm" onClick={onCancel}>Cancel</button>
          </div>
          {error && <span className="error-msg" style={{ width: '100%' }}>{error}</span>}
        </form>
      </td>
    </tr>
  );
}

// ── Add/Edit Payment form ─────────────────────────────────────────────────────

function PaymentForm({ invoiceId, existing, onDone, onCancel }) {
  const isEdit = Boolean(existing);
  const today = new Date().toISOString().slice(0, 10);
  const [form, setForm] = useState({
    payment_date: existing?.payment_date ? existing.payment_date.slice(0, 10) : today,
    amount: existing ? String(existing.amount) : '',
    payment_method: existing?.payment_method || 'CreditCard',
    payment_status: existing?.payment_status || 'Completed',
    transaction_id: existing?.transaction_id || '',
    notes: existing?.notes || '',
  });
  const [saving, setSaving] = useState(false);
  const [error, setError]   = useState('');

  const set = (k) => (e) => setForm((f) => ({ ...f, [k]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.amount || parseFloat(form.amount) === 0) { setError('Amount is required.'); return; }
    setSaving(true);
    setError('');
    try {
      const payload = {
        invoice_id: invoiceId,
        ...form,
        amount: parseFloat(form.amount),
        payment_date: form.payment_date || null,
      };
      if (isEdit) {
        await updatePayment(existing.id, payload);
      } else {
        await createPayment(payload);
      }
      onDone();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  };

  const METHODS = ['CreditCard', 'BankTransfer', 'Cash', 'Check', 'PayPal', 'Other'];
  const STATUSES = ['Pending', 'Completed', 'Failed', 'Refunded', 'Cancelled'];

  return (
    <tr className="inline-form-row">
      <td colSpan={8}>
        <form onSubmit={handleSubmit} style={{ display: 'flex', gap: 8, flexWrap: 'wrap', alignItems: 'flex-end', padding: '8px 4px' }}>
          <div style={{ flex: '0 0 130px' }}>
            <label style={{ fontSize: 11, color: '#888', display: 'block', marginBottom: 2 }}>Date</label>
            <input type="date" value={form.payment_date} onChange={set('payment_date')} style={{ fontSize: 13 }} />
          </div>
          <div style={{ flex: '0 0 110px' }}>
            <label style={{ fontSize: 11, color: '#888', display: 'block', marginBottom: 2 }}>Amount ($)</label>
            <input type="number" step="0.01" min="0.01" value={form.amount} onChange={set('amount')} required style={{ fontSize: 13 }} />
          </div>
          <div style={{ flex: '0 0 130px' }}>
            <label style={{ fontSize: 11, color: '#888', display: 'block', marginBottom: 2 }}>Method</label>
            <select value={form.payment_method} onChange={set('payment_method')} style={{ fontSize: 13 }}>
              {METHODS.map((m) => <option key={m} value={m}>{m}</option>)}
            </select>
          </div>
          <div style={{ flex: '0 0 120px' }}>
            <label style={{ fontSize: 11, color: '#888', display: 'block', marginBottom: 2 }}>Status</label>
            <select value={form.payment_status} onChange={set('payment_status')} style={{ fontSize: 13 }}>
              {STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
            </select>
          </div>
          <div style={{ flex: '1 1 140px' }}>
            <label style={{ fontSize: 11, color: '#888', display: 'block', marginBottom: 2 }}>Transaction ID</label>
            <input value={form.transaction_id} onChange={set('transaction_id')} placeholder="ch_xxxx" style={{ fontSize: 13 }} />
          </div>
          <div style={{ flex: '1 1 140px' }}>
            <label style={{ fontSize: 11, color: '#888', display: 'block', marginBottom: 2 }}>Notes</label>
            <input value={form.notes} onChange={set('notes')} style={{ fontSize: 13 }} />
          </div>
          <div style={{ display: 'flex', gap: 6 }}>
            <button type="submit" className="btn-primary btn-sm" disabled={saving}>
              {saving ? '…' : isEdit ? 'Save' : 'Add'}
            </button>
            <button type="button" className="btn-secondary btn-sm" onClick={onCancel}>Cancel</button>
          </div>
          {error && <span className="error-msg" style={{ width: '100%' }}>{error}</span>}
        </form>
      </td>
    </tr>
  );
}

// ── Main InvoiceDetail ────────────────────────────────────────────────────────

export default function InvoiceDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [invoice, setInvoice]   = useState(null);
  const [products, setProducts] = useState([]);
  const [loading, setLoading]   = useState(true);
  const [error, setError]       = useState('');

  // inline form state
  const [showAddLine, setShowAddLine] = useState(false);
  const [editLineId, setEditLineId]   = useState(null);
  const [showAddPay, setShowAddPay]   = useState(false);
  const [editPayId, setEditPayId]     = useState(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [inv, prods] = await Promise.all([fetchInvoice(id), fetchProducts()]);
      setInvoice(inv);
      setProducts(prods);
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => { load(); }, [load]);

  const handleDeleteInvoice = async () => {
    if (!window.confirm(`Delete invoice #${invoice.invoice_number}? This will also delete all its line items and payments.`)) return;
    try {
      await deleteInvoice(id);
      navigate('/invoices');
    } catch (err) {
      alert(err.message);
    }
  };

  const handleDeleteLine = async (li) => {
    if (!window.confirm(`Delete line item for ${li.product_name}?`)) return;
    try { await deleteInvoiceLineItem(li.id); load(); }
    catch (err) { alert(err.message); }
  };

  const handleDeletePayment = async (pay) => {
    if (!window.confirm(`Delete payment of ${fmt(pay.amount)}?`)) return;
    try { await deletePayment(pay.id); load(); }
    catch (err) { alert(err.message); }
  };

  const afterForm = () => {
    setShowAddLine(false); setEditLineId(null);
    setShowAddPay(false);  setEditPayId(null);
    load();
  };

  if (loading) return <p>Loading…</p>;
  if (error)   return <p className="error-msg">{error}</p>;
  if (!invoice) return null;

  const lineItems = invoice.line_items || [];
  const payments  = invoice.payments || [];

  return (
    <div className="page-card">
      <Link className="back-link" to="/invoices">← Back to invoices</Link>

      <h2>
        Invoice #{invoice.invoice_number}
        <span style={{ fontWeight: 400, fontSize: 16, color: '#666', marginLeft: 12 }}>
          — <Link to={`/clients/${invoice.client_id}`} className="link-plain">{invoice.client_name}</Link>
        </span>
      </h2>

      {/* Invoice meta */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 0 }}>
        <div className="detail-field">
          <div className="field-label">Date</div>
          <div className="field-value">{fmtDate(invoice.order_date)}</div>
        </div>
        <div className="detail-field">
          <div className="field-label">Status</div>
          <div className="field-value"><span className="tag">{invoice.order_status}</span></div>
        </div>
        <div className="detail-field">
          <div className="field-label">Tax Rate</div>
          <div className="field-value">{pct(invoice.tax_rate)}</div>
        </div>
        <div className="detail-field">
          <div className="field-label">Client Category</div>
          <div className="field-value">
            {invoice.client_category_name ? (
              <>
                <Link to={`/client-categories/${invoice.client_category_id}`} className="link-plain">
                  {invoice.client_category_name}
                </Link>
                <span className="tag" style={{ marginLeft: 8 }}>
                  {pct(invoice.client_category_discount)} discount
                </span>
              </>
            ) : (
              <em style={{ color: '#aaa' }}>None</em>
            )}
          </div>
        </div>
        <div className="detail-field">
          <div className="field-label">Billing Address</div>
          <div className="field-value" style={{ fontSize: 14 }}>{invoice.billing_address || '—'}</div>
        </div>
        <div className="detail-field">
          <div className="field-label">Shipping Address</div>
          <div className="field-value" style={{ fontSize: 14 }}>{invoice.shipping_address || '—'}</div>
        </div>
        <div className="detail-field">
          <div className="field-label">Slug</div>
          <div className="field-value" style={{ fontFamily: 'monospace', color: '#888', fontSize: 13 }}>{invoice.slug}</div>
        </div>
        {invoice.notes && (
          <div className="detail-field" style={{ gridColumn: '1 / -1' }}>
            <div className="field-label">Notes</div>
            <div className="field-value">{invoice.notes}</div>
          </div>
        )}
      </div>

      {/* Totals */}
      <Totals invoice={invoice} />

      {/* Line items */}
      <div style={{ marginTop: 24 }}>
        <div className="list-header">
          <h3 style={{ fontSize: 15 }}>Line Items ({lineItems.length}, {invoice.total_quantity} units)</h3>
          {!showAddLine && !editLineId && (
            <button className="btn-primary btn-sm" onClick={() => setShowAddLine(true)}>+ Add Line</button>
          )}
        </div>
        <table className="data-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Product</th>
              <th>Qty</th>
              <th>Unit Price</th>
              <th>Discount</th>
              <th>Pre-disc</th>
              <th>Disc Amt</th>
              <th>Subtotal</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {lineItems.map((li) => (
              editLineId === li.id ? (
                <LineItemForm
                  key={`edit-${li.id}`}
                  invoiceId={invoice.id}
                  existing={li}
                  products={products}
                  onDone={afterForm}
                  onCancel={() => setEditLineId(null)}
                />
              ) : (
                <tr key={li.id}>
                  <td>{li.line_number}</td>
                  <td>
                    <Link to={`/products/${li.product_id}`} className="link-mono">{li.product_sku}</Link>
                    <span style={{ color: '#666', marginLeft: 6, fontSize: 13 }}>{li.product_name}</span>
                  </td>
                  <td>{li.quantity}</td>
                  <td>{fmt(li.unit_price)}</td>
                  <td>{li.discount_percent > 0 ? pct(li.discount_percent) : '—'}</td>
                  <td>{fmt(li.pre_discount)}</td>
                  <td>{li.discount_percent > 0 ? fmt(li.discount_amount) : '—'}</td>
                  <td><strong>{fmt(li.subtotal)}</strong></td>
                  <td>
                    <div className="row-actions">
                      <button className="btn-secondary btn-sm" onClick={() => { setEditLineId(li.id); setShowAddLine(false); }}>Edit</button>
                      <button className="btn-danger btn-sm" onClick={() => handleDeleteLine(li)}>Del</button>
                    </div>
                  </td>
                </tr>
              )
            ))}
            {showAddLine && (
              <LineItemForm
                invoiceId={invoice.id}
                existing={null}
                products={products}
                onDone={afterForm}
                onCancel={() => setShowAddLine(false)}
              />
            )}
            {lineItems.length === 0 && !showAddLine && (
              <tr><td colSpan={9} style={{ color: '#aaa', textAlign: 'center', padding: 16 }}>No line items yet.</td></tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Payments */}
      <div style={{ marginTop: 24 }}>
        <div className="list-header">
          <h3 style={{ fontSize: 15 }}>Payments ({payments.length})</h3>
          {!showAddPay && !editPayId && (
            <button className="btn-primary btn-sm" onClick={() => setShowAddPay(true)}>+ Add Payment</button>
          )}
        </div>
        <table className="data-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Date</th>
              <th>Amount</th>
              <th>Method</th>
              <th>Status</th>
              <th>Completed Amt</th>
              <th>Transaction ID</th>
              <th>Notes</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {payments.map((pay) => (
              editPayId === pay.id ? (
                <PaymentForm
                  key={`edit-${pay.id}`}
                  invoiceId={invoice.id}
                  existing={pay}
                  onDone={afterForm}
                  onCancel={() => setEditPayId(null)}
                />
              ) : (
                <tr key={pay.id} className={pay.payment_status === 'Failed' ? 'row-failed' : pay.payment_status === 'Pending' ? 'row-pending' : ''}>
                  <td>{pay.payment_number}</td>
                  <td>{fmtDate(pay.payment_date)}</td>
                  <td>{fmt(pay.amount)}</td>
                  <td>{pay.payment_method}</td>
                  <td>
                    <span className={`tag tag-pay-${(pay.payment_status || '').toLowerCase()}`}>
                      {pay.payment_status}
                    </span>
                  </td>
                  <td style={{ color: pay.is_completed ? '#27ae60' : '#aaa' }}>
                    {fmt(pay.completed_amount)}
                  </td>
                  <td style={{ fontFamily: 'monospace', fontSize: 12, color: '#888' }}>
                    {pay.transaction_id || '—'}
                  </td>
                  <td style={{ fontSize: 13, color: '#666' }}>{pay.notes || '—'}</td>
                  <td>
                    <div className="row-actions">
                      <button className="btn-secondary btn-sm" onClick={() => { setEditPayId(pay.id); setShowAddPay(false); }}>Edit</button>
                      <button className="btn-danger btn-sm" onClick={() => handleDeletePayment(pay)}>Del</button>
                    </div>
                  </td>
                </tr>
              )
            ))}
            {showAddPay && (
              <PaymentForm
                invoiceId={invoice.id}
                existing={null}
                onDone={afterForm}
                onCancel={() => setShowAddPay(false)}
              />
            )}
            {payments.length === 0 && !showAddPay && (
              <tr><td colSpan={9} style={{ color: '#aaa', textAlign: 'center', padding: 16 }}>No payments yet.</td></tr>
            )}
          </tbody>
        </table>
      </div>

      <div className="form-actions">
        <button className="btn-primary" onClick={() => navigate(`/invoices/${id}/edit`)}>Edit Invoice</button>
        <button className="btn-danger" onClick={handleDeleteInvoice}>Delete Invoice</button>
        <button className="btn-secondary" onClick={() => navigate('/invoices')}>Back</button>
      </div>
    </div>
  );
}
