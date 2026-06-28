import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getProducts, createProduct, deleteProduct } from './api';

const inputStyle = { width: '100%', padding: '8px 12px', borderRadius: '7px', border: '1px solid #d1d5db', fontSize: '0.95rem', outline: 'none', boxSizing: 'border-box' };
const labelStyle = { display: 'block', fontSize: '0.85rem', fontWeight: 600, marginBottom: '5px', color: '#374151' };

function fmt(n) { return n != null ? '$' + parseFloat(n).toFixed(2) : '—'; }

export default function ProductListPage() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showAdd, setShowAdd] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [deleteError, setDeleteError] = useState(null);

  async function load() {
    try { setLoading(true); setProducts(await getProducts()); setError(null); }
    catch (e) { setError(e.message); } finally { setLoading(false); }
  }

  useEffect(() => { load(); }, []);

  async function handleAdd(formData) {
    setSaving(true);
    try { await createProduct(formData); setShowAdd(false); await load(); }
    catch (e) { alert(e.message); } finally { setSaving(false); }
  }

  async function handleDelete(id) {
    setDeleteError(null);
    try { await deleteProduct(id); setDeleteTarget(null); await load(); }
    catch (e) { setDeleteError(e.message); }
  }

  return (
    <div style={{ maxWidth: '1000px', margin: '0 auto', padding: '28px 16px' }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '22px' }}>
        <div>
          <h1 style={{ fontSize: '1.55rem', fontWeight: 700, color: '#1a1a1a' }}>Products</h1>
          <p style={{ color: '#6b7280', fontSize: '0.88rem', marginTop: '2px' }}>Manage product catalog</p>
        </div>
        <button onClick={() => setShowAdd(true)} style={addBtn}>+ Add Product</button>
      </div>

      {loading && <p style={{ color: '#6b7280' }}>Loading…</p>}
      {error && <p style={{ color: '#dc2626' }}>Error: {error}</p>}

      {showAdd && (
        <Modal title="Add Product" onClose={() => setShowAdd(false)}>
          <ProductForm onSave={handleAdd} onCancel={() => setShowAdd(false)} saving={saving} />
        </Modal>
      )}

      {deleteTarget && (
        <Modal title="Delete Product?" onClose={() => { setDeleteTarget(null); setDeleteError(null); }} small>
          {deleteError ? (
            <div style={{ padding: '10px 14px', background: '#fef2f2', border: '1px solid #fca5a5', borderRadius: '7px', color: '#991b1b', marginBottom: '16px', fontSize: '0.88rem' }}>
              ⚠️ {deleteError}
            </div>
          ) : (
            <p style={{ color: '#374151', marginBottom: '20px' }}>
              Delete <strong>{deleteTarget.display_name || deleteTarget.sku}</strong>? This cannot be undone.
            </p>
          )}
          <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
            <button onClick={() => { setDeleteTarget(null); setDeleteError(null); }} style={cancelBtn}>{deleteError ? 'Close' : 'Cancel'}</button>
            {!deleteError && <button onClick={() => handleDelete(deleteTarget.product_id)} style={redBtn}>Delete</button>}
          </div>
        </Modal>
      )}

      {!loading && !error && (
        <div style={{ background: '#fff', borderRadius: '12px', boxShadow: '0 1px 4px rgba(0,0,0,0.07)', overflow: 'hidden' }}>
          {products.length === 0 ? (
            <p style={{ padding: '32px', textAlign: 'center', color: '#6b7280' }}>No products yet.</p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>SKU</th>
                  <th style={th}>Display Name</th>
                  <th style={th}>Unit Price</th>
                  <th style={th}>Cost</th>
                  <th style={th}>Margin</th>
                  <th style={th}>Stock</th>
                  <th style={th}>Active</th>
                  <th style={{ ...th, width: '90px' }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {products.map(p => (
                  <tr key={p.product_id} style={{ borderBottom: '1px solid #e5e7eb', background: p.is_active ? '#fff' : '#fafafa' }}>
                    <td style={td}>
                      <Link to={`/products/${encodeURIComponent(p.product_id)}`} style={{ fontWeight: 700, color: '#1a1a1a', textDecoration: 'none', fontFamily: 'monospace', fontSize: '0.9rem' }}>
                        {p.sku}
                      </Link>
                    </td>
                    <td style={td}>
                      {p.display_name || <span style={{ color: '#d1d5db' }}>—</span>}
                      {p.is_high_margin && (
                        <span style={{ marginLeft: '6px', fontSize: '0.68rem', padding: '1px 7px', borderRadius: '8px', background: '#fef3c7', color: '#92400e', border: '1px solid #fcd34d', fontWeight: 700, letterSpacing: '0.04em' }}>
                          ⭐ HM
                        </span>
                      )}
                    </td>
                    <td style={{ ...td, fontWeight: 600 }}>{fmt(p.unit_price)}</td>
                    <td style={{ ...td, color: '#6b7280' }}>{fmt(p.cost)}</td>
                    <td style={{ ...td, fontWeight: 600, color: p.is_high_margin ? '#b45309' : '#374151' }}>
                      {p.margin != null ? (parseFloat(p.margin) * 100).toFixed(1) + '%' : '—'}
                    </td>
                    <td style={{ ...td, textAlign: 'center', color: p.stock_quantity === 0 ? '#dc2626' : '#374151', fontWeight: p.stock_quantity === 0 ? 700 : 400 }}>
                      {p.stock_quantity ?? '—'}
                    </td>
                    <td style={td}>
                      {p.is_active
                        ? <span style={{ ...badge, background: '#f0fdf4', color: '#16a34a', borderColor: '#86efac' }}>✓ Active</span>
                        : <span style={{ ...badge, background: '#f3f4f6', color: '#9ca3af', borderColor: '#e5e7eb' }}>Inactive</span>}
                    </td>
                    <td style={td}>
                      <div style={{ display: 'flex', gap: '6px' }}>
                        <Link to={`/products/${encodeURIComponent(p.product_id)}`} style={editLinkStyle}>Edit</Link>
                        <button onClick={() => { setDeleteTarget(p); setDeleteError(null); }} style={delBtnStyle}>Del</button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      )}
    </div>
  );
}

function ProductForm({ initial, onSave, onCancel, saving }) {
  const [sku, setSku] = useState(initial?.sku || '');
  const [displayName, setDisplayName] = useState(initial?.display_name || '');
  const [description, setDescription] = useState(initial?.description || '');
  const [unitPrice, setUnitPrice] = useState(initial?.unit_price ?? '');
  const [cost, setCost] = useState(initial?.cost ?? '');
  const [stockQuantity, setStockQuantity] = useState(initial?.stock_quantity ?? '');
  const [isActive, setIsActive] = useState(initial ? (initial.is_active !== false) : true);

  function handleSubmit(e) {
    e.preventDefault();
    if (!sku.trim()) return;
    onSave({ sku: sku.trim().toUpperCase(), display_name: displayName.trim() || null, description: description.trim() || null, unit_price: unitPrice !== '' ? parseFloat(unitPrice) : null, cost: cost !== '' ? parseFloat(cost) : null, stock_quantity: stockQuantity !== '' ? parseInt(stockQuantity) : null, is_active: isActive });
  }

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '13px' }}>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>SKU <span style={{ color: '#dc2626' }}>*</span></label>
          <input style={inputStyle} value={sku} onChange={e => setSku(e.target.value.toUpperCase())} placeholder="WIDGET-001" required autoFocus />
        </div>
        <div>
          <label style={labelStyle}>Display Name</label>
          <input style={inputStyle} value={displayName} onChange={e => setDisplayName(e.target.value)} placeholder="Standard Widget" />
        </div>
      </div>
      <div>
        <label style={labelStyle}>Description</label>
        <textarea style={{ ...inputStyle, minHeight: '60px', resize: 'vertical' }} value={description} onChange={e => setDescription(e.target.value)} placeholder="Product description…" />
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Unit Price</label>
          <input style={inputStyle} type="number" step="0.01" min="0" value={unitPrice} onChange={e => setUnitPrice(e.target.value)} placeholder="0.00" />
        </div>
        <div>
          <label style={labelStyle}>Cost</label>
          <input style={inputStyle} type="number" step="0.01" min="0" value={cost} onChange={e => setCost(e.target.value)} placeholder="0.00" />
        </div>
        <div>
          <label style={labelStyle}>Stock Qty</label>
          <input style={inputStyle} type="number" min="0" value={stockQuantity} onChange={e => setStockQuantity(e.target.value)} placeholder="—" />
        </div>
      </div>
      <label style={{ display: 'flex', alignItems: 'center', gap: '10px', cursor: 'pointer' }}>
        <input type="checkbox" checked={isActive} onChange={e => setIsActive(e.target.checked)} style={{ width: '18px', height: '18px', cursor: 'pointer' }} />
        <span style={{ fontSize: '0.92rem', fontWeight: 600, color: '#374151' }}>Active (sellable)</span>
      </label>
      <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end', marginTop: '4px' }}>
        {onCancel && <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>}
        <button type="submit" disabled={saving || !sku.trim()} style={saveBtn(saving || !sku.trim())}>{saving ? 'Saving…' : 'Save'}</button>
      </div>
    </form>
  );
}

function Modal({ title, children, onClose, small }) {
  return (
    <div style={modalOverlay}>
      <div style={{ ...modalBox, maxWidth: small ? '420px' : '540px' }}>
        <h2 style={{ marginBottom: '18px', fontSize: '1.1rem', fontWeight: 700 }}>{title}</h2>
        {children}
      </div>
    </div>
  );
}

const th = { padding: '11px 14px', textAlign: 'left', fontSize: '0.77rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em' };
const td = { padding: '12px 14px', fontSize: '0.92rem', verticalAlign: 'middle' };
const badge = { display: 'inline-block', padding: '2px 8px', borderRadius: '10px', fontWeight: 600, fontSize: '0.8rem', border: '1px solid' };
const addBtn = { padding: '10px 22px', background: '#059669', color: '#fff', border: 'none', borderRadius: '8px', fontWeight: 700, fontSize: '0.95rem', cursor: 'pointer' };
const editLinkStyle = { padding: '4px 10px', borderRadius: '5px', background: '#eff6ff', color: '#1e40af', fontSize: '0.8rem', fontWeight: 600, border: '1px solid #bfdbfe', textDecoration: 'none' };
const delBtnStyle = { padding: '4px 10px', borderRadius: '5px', background: '#fff', color: '#dc2626', fontSize: '0.8rem', fontWeight: 600, border: '1px solid #fca5a5', cursor: 'pointer' };
const cancelBtn = { padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer' };
const redBtn = { padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' };
function saveBtn(disabled) { return { padding: '8px 22px', borderRadius: '7px', border: 'none', background: disabled ? '#e5e7eb' : '#059669', color: disabled ? '#9ca3af' : '#fff', fontSize: '0.9rem', fontWeight: 600, cursor: disabled ? 'not-allowed' : 'pointer' }; }
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };
