import { useState, useEffect, useCallback } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { fetchInventoryAdjustments, deleteInventoryAdjustment, fetchProducts } from '../api.js';

const TYPE_CLASS = { Addition: 'tag-green', Removal: 'tag-red', Correction: 'tag' };

export default function InventoryList() {
  const [adjustments, setAdjustments] = useState([]);
  const [products, setProducts]       = useState([]);
  const [loading, setLoading]         = useState(true);
  const [filter, setFilter]           = useState('');
  const navigate = useNavigate();

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [a, p] = await Promise.all([fetchInventoryAdjustments(), fetchProducts()]);
      setAdjustments(a);
      setProducts(p);
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { load(); }, [load]);

  const handleDelete = async (ia) => {
    if (!window.confirm(`Delete ${ia.adjustment_type} of ${ia.quantity} for ${ia.product_sku}?`)) return;
    try { await deleteInventoryAdjustment(ia.id); load(); }
    catch (err) { alert(err.message); }
  };

  const shown = filter ? adjustments.filter((a) => String(a.product_id) === filter) : adjustments;

  return (
    <>
      <div className="list-header">
        <h2>Inventory Adjustments {!loading && `(${shown.length}${filter ? ` of ${adjustments.length}` : ''})`}</h2>
        <button className="btn-primary" onClick={() => navigate('/inventory/new')}>+ Add Adjustment</button>
      </div>

      {!loading && products.length > 0 && (
        <div className="inventory-overview">
          {products.map((p) => (
            <div key={p.id} className="inv-card">
              <Link to={`/products/${p.id}`} className="link-mono" style={{ fontSize: 12 }}>{p.sku}</Link>
              <div className="inv-on-hand">{p.stock_on_hand ?? 0}</div>
              <div className="inv-label">on hand</div>
              {p.adjustment_count > 0 && (
                <div className="inv-meta">{p.adjustment_count} adj</div>
              )}
            </div>
          ))}
        </div>
      )}

      <div style={{ margin: '12px 0' }}>
        <label style={{ fontSize: 13, marginRight: 8 }}>Filter by product:</label>
        <select value={filter} onChange={(e) => setFilter(e.target.value)}>
          <option value="">All products</option>
          {products.map((p) => (<option key={p.id} value={p.id}>{p.sku} — {p.display_name}</option>))}
        </select>
      </div>

      {loading ? <p>Loading…</p> : shown.length === 0 ? (
        <p style={{ color: '#888' }}>No adjustments yet.</p>
      ) : (
        <table className="data-table">
          <thead>
            <tr>
              <th>Date</th>
              <th>Product</th>
              <th>Type</th>
              <th>Qty</th>
              <th>Δ</th>
              <th>Reason</th>
              <th>By</th>
              <th>Notes</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {shown.map((a) => (
              <tr key={a.id}>
                <td>{new Date(a.adjustment_date).toLocaleDateString()}</td>
                <td><Link to={`/products/${a.product_id}`} className="link-mono">{a.product_sku}</Link></td>
                <td><span className={`tag ${TYPE_CLASS[a.adjustment_type] || 'tag'}`}>{a.adjustment_type}</span></td>
                <td>{a.quantity}</td>
                <td style={{ fontWeight: 600, color: a.signed_quantity < 0 ? '#c33' : '#262' }}>
                  {a.signed_quantity > 0 ? '+' : ''}{a.signed_quantity}
                </td>
                <td>{a.reason}</td>
                <td>{a.adjusted_by || <em style={{ color: '#aaa' }}>—</em>}</td>
                <td>{a.notes || <em style={{ color: '#aaa' }}>—</em>}</td>
                <td>
                  <div className="row-actions">
                    <button className="btn-secondary btn-sm" onClick={() => navigate(`/inventory/${a.id}/edit`)}>Edit</button>
                    <button className="btn-danger btn-sm" onClick={() => handleDelete(a)}>Delete</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  );
}
