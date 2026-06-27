import { useState, useEffect } from 'react';
import { useParams, useNavigate, useSearchParams, Link } from 'react-router-dom';
import {
  fetchInventoryAdjustment, createInventoryAdjustment, updateInventoryAdjustment,
  fetchProducts,
} from '../api.js';

const REASONS = ['Purchase Order', 'Sales Order', 'Inventory Count', 'Damage', 'Return', 'Transfer', 'Other'];
const TYPES   = ['Addition', 'Removal', 'Correction'];

export default function InventoryForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [qs] = useSearchParams();
  const isEdit = Boolean(id);

  const [products, setProducts] = useState([]);
  const [form, setForm] = useState({
    product_id: qs.get('product_id') || '',
    adjustment_date: new Date().toISOString().slice(0, 10),
    adjustment_type: 'Addition',
    quantity: '1',
    reason: 'Purchase Order',
    adjusted_by: '',
    notes: '',
  });
  const [loading, setLoading] = useState(true);
  const [saving, setSaving]   = useState(false);
  const [error, setError]     = useState('');

  useEffect(() => {
    (async () => {
      try {
        const ps = await fetchProducts();
        setProducts(ps);
        if (isEdit) {
          const a = await fetchInventoryAdjustment(id);
          setForm({
            product_id: String(a.product_id),
            adjustment_date: a.adjustment_date ? new Date(a.adjustment_date).toISOString().slice(0, 10) : '',
            adjustment_type: a.adjustment_type,
            quantity: String(a.quantity),
            reason: a.reason,
            adjusted_by: a.adjusted_by || '',
            notes: a.notes || '',
          });
        }
      } catch (e) { setError(e.message); }
      finally { setLoading(false); }
    })();
  }, [id, isEdit]);

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.product_id) { setError('Product is required.'); return; }
    const qty = parseInt(form.quantity, 10);
    if (!Number.isFinite(qty) || qty < 0) { setError('Quantity must be a non-negative integer.'); return; }
    setSaving(true); setError('');
    const payload = {
      product_id: Number(form.product_id),
      adjustment_date: form.adjustment_date || null,
      adjustment_type: form.adjustment_type,
      quantity: qty,
      reason: form.reason,
      adjusted_by: form.adjusted_by,
      notes: form.notes,
    };
    try {
      if (isEdit) { await updateInventoryAdjustment(id, payload); }
      else        { await createInventoryAdjustment(payload); }
      navigate('/inventory');
    } catch (err) { setError(err.message); }
    finally { setSaving(false); }
  };

  if (loading) return <p>Loading…</p>;

  return (
    <div className="page-card">
      <Link className="back-link" to="/inventory">← Back to inventory</Link>
      <h2>{isEdit ? 'Edit Inventory Adjustment' : 'Add Inventory Adjustment'}</h2>
      <form onSubmit={handleSubmit}>
        <div className="field">
          <label htmlFor="product_id">Product *</label>
          <select id="product_id" value={form.product_id} onChange={set('product_id')} required disabled={isEdit}>
            <option value="">— Select product —</option>
            {products.map((p) => (
              <option key={p.id} value={p.id}>
                {p.sku} — {p.display_name} (on hand: {p.stock_on_hand ?? 0})
              </option>
            ))}
          </select>
        </div>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}>
          <div className="field">
            <label htmlFor="adjustment_type">Type *</label>
            <select id="adjustment_type" value={form.adjustment_type} onChange={set('adjustment_type')} required>
              {TYPES.map((t) => <option key={t} value={t}>{t}</option>)}
            </select>
          </div>
          <div className="field">
            <label htmlFor="quantity">Quantity *</label>
            <input id="quantity" type="number" min="0" step="1" value={form.quantity} onChange={set('quantity')} required />
          </div>
          <div className="field">
            <label htmlFor="reason">Reason *</label>
            <select id="reason" value={form.reason} onChange={set('reason')} required>
              {REASONS.map((r) => <option key={r} value={r}>{r}</option>)}
            </select>
          </div>
          <div className="field">
            <label htmlFor="adjustment_date">Date</label>
            <input id="adjustment_date" type="date" value={form.adjustment_date} onChange={set('adjustment_date')} />
          </div>
        </div>
        <div className="field">
          <label htmlFor="adjusted_by">Adjusted By</label>
          <input id="adjusted_by" value={form.adjusted_by} onChange={set('adjusted_by')} placeholder="Your name or initials" />
        </div>
        <div className="field">
          <label htmlFor="notes">Notes</label>
          <textarea id="notes" value={form.notes} onChange={set('notes')} />
        </div>
        <div style={{ fontSize: 13, color: '#666', marginBottom: 12 }}>
          <strong>Additions</strong> add to on-hand stock, <strong>Removals</strong> subtract,
          and <strong>Corrections</strong> apply as a positive audit adjustment.
        </div>
        {error && <p className="error-msg">{error}</p>}
        <div className="form-actions">
          <button type="submit" className="btn-primary" disabled={saving}>
            {saving ? 'Saving…' : isEdit ? 'Save Changes' : 'Add Adjustment'}
          </button>
          <button type="button" className="btn-secondary" onClick={() => navigate('/inventory')}>Cancel</button>
        </div>
      </form>
    </div>
  );
}
