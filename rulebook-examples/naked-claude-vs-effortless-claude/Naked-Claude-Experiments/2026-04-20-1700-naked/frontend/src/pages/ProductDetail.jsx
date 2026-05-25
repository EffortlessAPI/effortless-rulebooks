import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchProduct, deleteProduct } from '../api.js';

const fmt = (n) => Number(n ?? 0).toLocaleString('en-US', { style: 'currency', currency: 'USD' });
const fmtDate = (d) => d ? new Date(d).toLocaleDateString() : '—';

function Field({ label, value, mono }) {
  return (
    <div className="detail-field">
      <div className="field-label">{label}</div>
      <div className="field-value" style={mono ? { fontFamily: 'monospace', color: '#888' } : {}}>
        {value !== undefined && value !== null && value !== '' ? value : <em style={{ color: '#aaa' }}>—</em>}
      </div>
    </div>
  );
}

export default function ProductDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError]     = useState('');

  useEffect(() => {
    fetchProduct(id)
      .then(setProduct)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [id]);

  const handleDelete = async () => {
    if ((product.line_item_count || 0) > 0) {
      alert(`Cannot delete "${product.display_name}" — it appears on ${product.line_item_count} line item(s). Mark it inactive instead.`);
      return;
    }
    if (!window.confirm(`Delete product "${product.sku}"?`)) return;
    try {
      await deleteProduct(id);
      navigate('/products');
    } catch (err) {
      alert(err.message);
    }
  };

  if (loading) return <p>Loading…</p>;
  if (error)   return <p className="error-msg">{error}</p>;
  if (!product) return null;

  return (
    <div className="page-card">
      <Link className="back-link" to="/products">← Back to products</Link>
      <h2>
        <span style={{ fontFamily: 'monospace', fontSize: 18 }}>{product.sku}</span>
        {' — '}
        {product.display_name}
        {!product.is_active && <span className="tag tag-grey" style={{ marginLeft: 10 }}>Inactive</span>}
      </h2>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 0 }}>
        <Field label="SKU"          value={product.sku} mono />
        <Field label="Slug"         value={product.slug} mono />
        <Field label="Display Name" value={product.display_name} />
        <Field label="Active"       value={product.is_active ? 'Yes' : 'No'} />
        <Field label="Unit Price"   value={fmt(product.unit_price)} />
        <Field label="Cost"         value={fmt(product.cost)} />
        <Field label="Profit / unit" value={fmt(product.profit)} />
        <Field label="Margin" value={
          <>
            {product.margin != null ? `${(Number(product.margin) * 100).toFixed(1)}%` : '—'}
            {product.is_high_margin && <span className="tag tag-green" style={{ marginLeft: 8 }}>High margin</span>}
          </>
        } />
        <Field label="Stock on Hand" value={
          <span style={{ fontWeight: 600, color: (product.stock_on_hand ?? 0) <= 0 ? '#c33' : '#262' }}>
            {product.stock_on_hand ?? 0}
          </span>
        } />
        <Field label="Adjustments" value={product.adjustment_count ?? 0} />
        <Field label="Total Sold" value={product.total_sold_qty ?? 0} />
        <Field label="COGS" value={fmt(product.cogs)} />
      </div>

      {product.description && (
        <div className="detail-field">
          <div className="field-label">Description</div>
          <div className="field-value">{product.description}</div>
        </div>
      )}

      {/* Inventory adjustments inline */}
      <div style={{ marginTop: 24 }}>
        <h3 style={{ fontSize: 15, marginBottom: 12, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <span>Inventory Adjustments ({(product.inventory_adjustments || []).length})</span>
          <button className="btn-secondary btn-sm"
                  onClick={() => navigate(`/inventory/new?product_id=${product.id}`)}>
            + Add Adjustment
          </button>
        </h3>
        {(product.inventory_adjustments || []).length === 0 ? (
          <p style={{ color: '#888', fontSize: 14 }}>No inventory activity yet.</p>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Date</th><th>Type</th><th>Qty</th><th>Reason</th><th>By</th><th>Notes</th>
              </tr>
            </thead>
            <tbody>
              {product.inventory_adjustments.map((ia) => (
                <tr key={ia.id}>
                  <td>{fmtDate(ia.adjustment_date)}</td>
                  <td>
                    <span className={`tag ${
                      ia.adjustment_type === 'Addition' ? 'tag-green' :
                      ia.adjustment_type === 'Removal'  ? 'tag-red'   : 'tag'
                    }`}>{ia.adjustment_type}</span>
                  </td>
                  <td>{ia.quantity}</td>
                  <td>{ia.reason}</td>
                  <td>{ia.adjusted_by || <em style={{ color: '#aaa' }}>—</em>}</td>
                  <td>{ia.notes || <em style={{ color: '#aaa' }}>—</em>}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* Line items inline */}
      <div style={{ marginTop: 24 }}>
        <h3 style={{ fontSize: 15, marginBottom: 12 }}>
          Invoice Line Items ({(product.line_items || []).length})
        </h3>
        {(product.line_items || []).length === 0 ? (
          <p style={{ color: '#888', fontSize: 14 }}>This product hasn't been invoiced yet.</p>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Invoice</th>
                <th>Date</th>
                <th>Line #</th>
                <th>Qty</th>
                <th>Unit Price</th>
                <th>Discount</th>
                <th>Subtotal</th>
              </tr>
            </thead>
            <tbody>
              {product.line_items.map((li) => (
                <tr key={li.id}>
                  <td><Link to={`/invoices/${li.invoice_id}`} className="link-plain">{li.slug.replace(/-line-\d+$/, '')}</Link></td>
                  <td>—</td>
                  <td>{li.line_number}</td>
                  <td>{li.quantity}</td>
                  <td>{fmt(li.unit_price)}</td>
                  <td>{li.discount_percent > 0 ? `${(li.discount_percent * 100).toFixed(0)}%` : '—'}</td>
                  <td>{fmt(li.subtotal)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <div className="form-actions">
        <button className="btn-primary" onClick={() => navigate(`/products/${id}/edit`)}>Edit</button>
        <button className="btn-danger" onClick={handleDelete}>Delete</button>
        <button className="btn-secondary" onClick={() => navigate('/products')}>Back</button>
      </div>
    </div>
  );
}
