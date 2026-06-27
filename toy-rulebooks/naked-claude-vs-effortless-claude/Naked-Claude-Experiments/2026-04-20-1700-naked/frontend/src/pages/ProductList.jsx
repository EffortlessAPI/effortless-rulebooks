import { useState, useEffect, useCallback } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { fetchProducts, deleteProduct } from '../api.js';

const fmt = (n) => Number(n ?? 0).toLocaleString('en-US', { style: 'currency', currency: 'USD' });

export default function ProductList() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading]   = useState(true);
  const navigate = useNavigate();

  const load = useCallback(async () => {
    setLoading(true);
    try { setProducts(await fetchProducts()); }
    catch (e) { console.error(e); }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { load(); }, [load]);

  const handleDelete = async (p) => {
    if (p.line_item_count > 0) {
      alert(`Cannot delete "${p.display_name}" — it appears on ${p.line_item_count} line item(s). Mark it inactive instead.`);
      return;
    }
    if (!window.confirm(`Delete product "${p.sku}"?`)) return;
    try { await deleteProduct(p.id); load(); }
    catch (err) { alert(err.message); }
  };

  return (
    <>
      <div className="list-header">
        <h2>Products {!loading && `(${products.length})`}</h2>
        <button className="btn-primary" onClick={() => navigate('/products/new')}>+ Add Product</button>
      </div>
      {loading ? <p>Loading…</p> : products.length === 0 ? (
        <p style={{ color: '#888' }}>No products yet.</p>
      ) : (
        <table className="data-table">
          <thead>
            <tr>
              <th>SKU</th>
              <th>Display Name</th>
              <th>Price</th>
              <th>Cost</th>
              <th>Margin</th>
              <th>On Hand</th>
              <th>Active</th>
              <th>Line Items</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {products.map((p) => {
              const marginPct = p.margin != null ? (Number(p.margin) * 100).toFixed(0) : '—';
              const onHand = p.stock_on_hand ?? 0;
              return (
                <tr key={p.id} className={!p.is_active ? 'row-inactive' : ''}>
                  <td><Link to={`/products/${p.id}`} className="link-mono">{p.sku}</Link></td>
                  <td>{p.display_name}</td>
                  <td>{fmt(p.unit_price)}</td>
                  <td>{fmt(p.cost)}</td>
                  <td>
                    {marginPct}%
                    {p.is_high_margin && <span className="tag tag-green" style={{ marginLeft: 6 }}>High</span>}
                  </td>
                  <td style={{ fontWeight: 600, color: onHand <= 0 ? '#c33' : onHand < 5 ? '#c60' : '#262' }}>
                    {onHand}
                  </td>
                  <td>
                    <span className={`tag ${p.is_active ? 'tag-green' : 'tag-grey'}`}>
                      {p.is_active ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td>{p.line_item_count}</td>
                  <td>
                    <div className="row-actions">
                      <button className="btn-secondary btn-sm" onClick={() => navigate(`/products/${p.id}/edit`)}>Edit</button>
                      <button className="btn-danger btn-sm" onClick={() => handleDelete(p)}>Delete</button>
                    </div>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      )}
    </>
  );
}
