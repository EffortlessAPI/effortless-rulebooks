import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link, useSearchParams } from 'react-router-dom';
import { fetchInvoice, fetchClients, createInvoice, updateInvoice } from '../api.js';

export default function InvoiceForm() {
  const { id } = useParams();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const [form, setForm] = useState({
    client_id: searchParams.get('client') || '',
    invoice_number: '',
    order_date: '',
    order_status: 'New',
    shipping_address: '',
    billing_address: '',
    tax_rate: '0.085',
    notes: '',
  });
  const [clients, setClients]  = useState([]);
  const [loading, setLoading]  = useState(true);
  const [saving, setSaving]    = useState(false);
  const [error, setError]      = useState('');

  useEffect(() => {
    const loads = [fetchClients()];
    if (isEdit) loads.push(fetchInvoice(id));
    Promise.all(loads)
      .then(([cls, inv]) => {
        setClients(cls);
        if (inv) {
          setForm({
            client_id: String(inv.client_id),
            invoice_number: inv.invoice_number || '',
            order_date: inv.order_date ? inv.order_date.slice(0, 10) : '',
            order_status: inv.order_status || 'New',
            shipping_address: inv.shipping_address || '',
            billing_address: inv.billing_address || '',
            tax_rate: String(inv.tax_rate || 0),
            notes: inv.notes || '',
          });
        }
      })
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [id, isEdit]);

  const set = (k) => (e) => setForm((f) => ({ ...f, [k]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.client_id) { setError('Client is required.'); return; }
    if (!form.invoice_number.trim()) { setError('Invoice number is required.'); return; }
    setSaving(true);
    setError('');
    try {
      const payload = {
        ...form,
        client_id: parseInt(form.client_id, 10),
        order_date: form.order_date || null,
        tax_rate: parseFloat(form.tax_rate) || 0,
      };
      if (isEdit) {
        await updateInvoice(id, payload);
        navigate(`/invoices/${id}`);
      } else {
        const created = await createInvoice(payload);
        navigate(`/invoices/${created.id}`);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  };

  const ORDER_STATUSES = ['New', 'Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled', 'Returned'];

  if (loading) return <p>Loading…</p>;

  return (
    <div className="page-card">
      <Link className="back-link" to={isEdit ? `/invoices/${id}` : '/invoices'}>
        ← {isEdit ? 'Back to invoice' : 'Back to invoices'}
      </Link>
      <h2>{isEdit ? 'Edit Invoice' : 'Add Invoice'}</h2>
      <form onSubmit={handleSubmit}>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '0 20px' }}>
          <div className="field">
            <label>Client *</label>
            <select value={form.client_id} onChange={set('client_id')} required>
              <option value="">— Select client —</option>
              {clients.map((c) => (
                <option key={c.id} value={String(c.id)}>{c.name}{c.company_name ? ` (${c.company_name})` : ''}</option>
              ))}
            </select>
          </div>
          <div className="field">
            <label>Invoice Number *</label>
            <input value={form.invoice_number} onChange={set('invoice_number')} placeholder="1001" required />
          </div>
          <div className="field">
            <label>Order Date</label>
            <input type="date" value={form.order_date} onChange={set('order_date')} />
          </div>
          <div className="field">
            <label>Order Status</label>
            <select value={form.order_status} onChange={set('order_status')}>
              {ORDER_STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
            </select>
          </div>
          <div className="field">
            <label>Tax Rate (e.g. 0.085 for 8.5%)</label>
            <input type="number" step="0.001" min="0" max="1" value={form.tax_rate} onChange={set('tax_rate')} placeholder="0.085" />
          </div>
          <div className="field" />
          <div className="field">
            <label>Billing Address</label>
            <textarea value={form.billing_address} onChange={set('billing_address')} rows={2} placeholder="123 Main St…" />
          </div>
          <div className="field">
            <label>Shipping Address</label>
            <textarea value={form.shipping_address} onChange={set('shipping_address')} rows={2} placeholder="123 Main St…" />
          </div>
        </div>
        <div className="field">
          <label>Notes</label>
          <textarea value={form.notes} onChange={set('notes')} placeholder="Any notes about this invoice…" />
        </div>
        {error && <p className="error-msg">{error}</p>}
        <div className="form-actions">
          <button type="submit" className="btn-primary" disabled={saving}>
            {saving ? 'Saving…' : isEdit ? 'Save Changes' : 'Add Invoice'}
          </button>
          <button type="button" className="btn-secondary"
            onClick={() => navigate(isEdit ? `/invoices/${id}` : '/invoices')}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
