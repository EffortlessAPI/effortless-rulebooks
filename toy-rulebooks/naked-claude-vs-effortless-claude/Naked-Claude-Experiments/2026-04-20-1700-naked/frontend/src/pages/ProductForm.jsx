import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchProduct, createProduct, updateProduct } from '../api.js';

export default function ProductForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const [form, setForm] = useState({
    sku: '', display_name: '', description: '',
    unit_price: '', cost: '', stock_quantity: '', is_active: true,
  });
  const [loading, setLoading] = useState(isEdit);
  const [saving, setSaving]   = useState(false);
  const [error, setError]     = useState('');

  useEffect(() => {
    if (!isEdit) return;
    fetchProduct(id)
      .then((p) => setForm({
        sku: p.sku || '',
        display_name: p.display_name || '',
        description: p.description || '',
        unit_price: p.unit_price !== null ? String(p.unit_price) : '',
        cost: p.cost !== null ? String(p.cost) : '',
        stock_quantity: p.stock_quantity !== null ? String(p.stock_quantity) : '',
        is_active: p.is_active,
      }))
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [id, isEdit]);

  const set = (k) => (e) => setForm((f) => ({ ...f, [k]: e.target.value }));
  const setBool = (k) => (e) => setForm((f) => ({ ...f, [k]: e.target.checked }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.sku.trim()) { setError('SKU is required.'); return; }
    setSaving(true);
    setError('');
    try {
      const payload = {
        sku: form.sku.trim(),
        display_name: form.display_name,
        description: form.description,
        unit_price: parseFloat(form.unit_price) || 0,
        cost: parseFloat(form.cost) || 0,
        stock_quantity: form.stock_quantity === '' ? null : parseInt(form.stock_quantity, 10),
        is_active: form.is_active,
      };
      if (isEdit) {
        await updateProduct(id, payload);
        navigate(`/products/${id}`);
      } else {
        const created = await createProduct(payload);
        navigate(`/products/${created.id}`);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <p>Loading…</p>;

  return (
    <div className="page-card">
      <Link className="back-link" to={isEdit ? `/products/${id}` : '/products'}>
        ← {isEdit ? 'Back to product' : 'Back to products'}
      </Link>
      <h2>{isEdit ? 'Edit Product' : 'Add Product'}</h2>
      <form onSubmit={handleSubmit}>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '0 20px' }}>
          <div className="field">
            <label>SKU *</label>
            <input value={form.sku} onChange={set('sku')} placeholder="WIDGET-001" required />
          </div>
          <div className="field">
            <label>Display Name</label>
            <input value={form.display_name} onChange={set('display_name')} placeholder="Standard Widget" />
          </div>
          <div className="field">
            <label>Unit Price ($)</label>
            <input type="number" step="0.01" min="0" value={form.unit_price} onChange={set('unit_price')} placeholder="19.99" />
          </div>
          <div className="field">
            <label>Cost ($)</label>
            <input type="number" step="0.01" min="0" value={form.cost} onChange={set('cost')} placeholder="7.50" />
          </div>
          <div className="field">
            <label>Stock Quantity</label>
            <input type="number" min="0" value={form.stock_quantity} onChange={set('stock_quantity')} placeholder="Leave blank if unknown" />
          </div>
          <div className="field" style={{ justifyContent: 'flex-end' }}>
            <label className="checkbox-label" style={{ marginTop: 24 }}>
              <input
                type="checkbox"
                checked={form.is_active}
                onChange={setBool('is_active')}
                style={{ width: 'auto', marginRight: 8 }}
              />
              Active (available for ordering)
            </label>
          </div>
        </div>
        <div className="field">
          <label>Description</label>
          <textarea value={form.description} onChange={set('description')} placeholder="Long-form product description…" />
        </div>
        {error && <p className="error-msg">{error}</p>}
        <div className="form-actions">
          <button type="submit" className="btn-primary" disabled={saving}>
            {saving ? 'Saving…' : isEdit ? 'Save Changes' : 'Add Product'}
          </button>
          <button type="button" className="btn-secondary"
            onClick={() => navigate(isEdit ? `/products/${id}` : '/products')}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
