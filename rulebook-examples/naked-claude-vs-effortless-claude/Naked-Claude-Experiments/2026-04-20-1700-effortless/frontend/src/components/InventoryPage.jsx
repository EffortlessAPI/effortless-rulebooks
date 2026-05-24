import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import {
  getInventoryAdjustments,
  createInventoryAdjustment,
  deleteInventoryAdjustment,
  getProducts
} from './api';

const ADJ_TYPES = ['Correction', 'Addition', 'Removal', 'Shrinkage', 'Damage', 'Transfer'];

const inputStyle = { width: '100%', padding: '8px 12px', borderRadius: '7px', border: '1px solid #d1d5db', fontSize: '0.95rem', outline: 'none', boxSizing: 'border-box' };
const labelStyle = { display: 'block', fontSize: '0.85rem', fontWeight: 600, marginBottom: '5px', color: '#374151' };

function fmtDate(d) {
  if (!d) return '—';
  try { return new Date(d).toLocaleDateString(); } catch { return d; }
}

export default function InventoryPage() {
  const [adjustments, setAdjustments] = useState([]);
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showAdd, setShowAdd] = useState(false);
  const [saving, setSaving] = useState(false);
  const [filterProduct, setFilterProduct] = useState('');
  const [deleteTarget, setDeleteTarget] = useState(null);

  async function load() {
    try {
      setLoading(true);
      const [adj, prods] = await Promise.all([
        getInventoryAdjustments(filterProduct || undefined),
        getProducts()
      ]);
      setAdjustments(adj);
      setProducts(prods);
      setError(null);
    } catch (e) { setError(e.message); } finally { setLoading(false); }
  }

  useEffect(() => { load(); }, [filterProduct]);

  async function handleAdd(data) {
    setSaving(true);
    try { await createInventoryAdjustment(data); setShowAdd(false); await load(); }
    catch (e) { alert(e.message); } finally { setSaving(false); }
  }

  async function handleDelete(id) {
    try { await deleteInventoryAdjustment(id); setDeleteTarget(null); await load(); }
    catch (e) { alert(e.message); }
  }

  const totalStockValue = products.reduce(
    (sum, p) => sum + (p.stock_quantity || 0) * (p.cost || 0), 0
  );
  const outOfStock = products.filter(p => p.is_active && p.stock_quantity === 0).length;
  const lowStock = products.filter(p => p.is_active && p.stock_quantity > 0 && p.stock_quantity < 50).length;

  return (
    <div style={{ maxWidth: '1100px', margin: '0 auto', padding: '28px 16px' }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '22px' }}>
        <div>
          <h1 style={{ fontSize: '1.55rem', fontWeight: 700, color: '#1a1a1a' }}>Inventory</h1>
          <p style={{ color: '#6b7280', fontSize: '0.88rem', marginTop: '2px' }}>Stock levels & adjustment history</p>
        </div>
        <button onClick={() => setShowAdd(true)} style={addBtn}>+ New Adjustment</button>
      </div>

      {/* KPI cards */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '12px', marginBottom: '20px' }}>
        <Kpi label="Total Products" value={products.length} color="#1e40af" bg="#eff6ff" />
        <Kpi label="Inventory Value" value={'$' + totalStockValue.toFixed(2)} color="#15803d" bg="#f0fdf4" />
        <Kpi label="Out of Stock" value={outOfStock} color={outOfStock > 0 ? '#dc2626' : '#6b7280'} bg={outOfStock > 0 ? '#fef2f2' : '#f9fafb'} />
        <Kpi label="Low Stock (<50)" value={lowStock} color={lowStock > 0 ? '#d97706' : '#6b7280'} bg={lowStock > 0 ? '#fffbeb' : '#f9fafb'} />
      </div>

      {/* Stock levels by product */}
      <section style={{ background: '#fff', borderRadius: '12px', boxShadow: '0 1px 4px rgba(0,0,0,0.07)', overflow: 'hidden', marginBottom: '22px' }}>
        <div style={sectionHeader}>Current Stock Levels</div>
        {products.length === 0 ? (
          <p style={{ padding: '24px', textAlign: 'center', color: '#6b7280' }}>No products.</p>
        ) : (
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <thead>
              <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                <th style={th}>SKU</th>
                <th style={th}>Product</th>
                <th style={{ ...th, textAlign: 'right' }}>Stock</th>
                <th style={{ ...th, textAlign: 'right' }}>Unit Cost</th>
                <th style={{ ...th, textAlign: 'right' }}>COGS</th>
                <th style={th}>Status</th>
              </tr>
            </thead>
            <tbody>
              {products.map(p => (
                <tr key={p.product_id} style={{ borderBottom: '1px solid #e5e7eb', opacity: p.is_active ? 1 : 0.5 }}>
                  <td style={td}>
                    <Link to={`/products/${encodeURIComponent(p.product_id)}`} style={{ fontFamily: 'monospace', fontWeight: 700, color: '#4f46e5', textDecoration: 'none', fontSize: '0.88rem' }}>
                      {p.sku}
                    </Link>
                  </td>
                  <td style={td}>{p.display_name || p.sku}</td>
                  <td style={{ ...td, textAlign: 'right', fontWeight: 700, color: p.stock_quantity === 0 ? '#dc2626' : p.stock_quantity < 50 ? '#d97706' : '#15803d' }}>
                    {p.stock_quantity ?? 0}
                  </td>
                  <td style={{ ...td, textAlign: 'right', color: '#6b7280' }}>
                    {p.cost != null ? '$' + parseFloat(p.cost).toFixed(2) : '—'}
                  </td>
                  <td style={{ ...td, textAlign: 'right', fontWeight: 600 }}>
                    {p.cogs != null ? '$' + parseFloat(p.cogs).toFixed(2) : '—'}
                  </td>
                  <td style={td}>
                    {p.stock_quantity === 0
                      ? <span style={{ ...badge, background: '#fef2f2', color: '#dc2626', borderColor: '#fca5a5' }}>Out of stock</span>
                      : p.stock_quantity < 50
                        ? <span style={{ ...badge, background: '#fffbeb', color: '#d97706', borderColor: '#fcd34d' }}>Low</span>
                        : <span style={{ ...badge, background: '#f0fdf4', color: '#15803d', borderColor: '#86efac' }}>OK</span>}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      {/* Adjustment history */}
      <section style={{ background: '#fff', borderRadius: '12px', boxShadow: '0 1px 4px rgba(0,0,0,0.07)', overflow: 'hidden' }}>
        <div style={{ ...sectionHeader, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <span>Adjustment History</span>
          <select
            value={filterProduct}
            onChange={e => setFilterProduct(e.target.value)}
            style={{ padding: '5px 10px', borderRadius: '6px', border: '1px solid #d1d5db', fontSize: '0.85rem', background: '#fff' }}
          >
            <option value="">All products</option>
            {products.map(p => (
              <option key={p.product_id} value={p.product_id}>
                {p.sku} — {p.display_name || p.sku}
              </option>
            ))}
          </select>
        </div>
        {loading && <p style={{ padding: '24px', color: '#6b7280' }}>Loading…</p>}
        {error && <p style={{ padding: '24px', color: '#dc2626' }}>Error: {error}</p>}
        {!loading && !error && (
          adjustments.length === 0 ? (
            <p style={{ padding: '24px', textAlign: 'center', color: '#6b7280' }}>No adjustments yet.</p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>Date</th>
                  <th style={th}>Product</th>
                  <th style={th}>Type</th>
                  <th style={{ ...th, textAlign: 'right' }}>Qty</th>
                  <th style={th}>Reason</th>
                  <th style={th}>By</th>
                  <th style={th}>Notes</th>
                  <th style={{ ...th, width: '60px' }}></th>
                </tr>
              </thead>
              <tbody>
                {adjustments.map(a => (
                  <tr key={a.inventory_adjustment_id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={td}>{fmtDate(a.date)}</td>
                    <td style={td}>
                      <Link to={`/products/${encodeURIComponent(a.product)}`} style={{ color: '#4f46e5', fontWeight: 600, textDecoration: 'none' }}>
                        {a.product_sku || a.product}
                      </Link>
                      {a.product_display_name && (
                        <div style={{ fontSize: '0.75rem', color: '#9ca3af' }}>{a.product_display_name}</div>
                      )}
                    </td>
                    <td style={td}>
                      <span style={typeBadge(a.adjustment_type)}>{a.adjustment_type || '—'}</span>
                    </td>
                    <td style={{ ...td, textAlign: 'right', fontWeight: 700, color: a.quantity < 0 ? '#dc2626' : '#15803d' }}>
                      {a.quantity > 0 ? '+' : ''}{a.quantity}
                    </td>
                    <td style={{ ...td, fontSize: '0.87rem', color: '#4b5563' }}>{a.reason || '—'}</td>
                    <td style={{ ...td, fontSize: '0.87rem', color: '#6b7280' }}>{a.adjusted_by || '—'}</td>
                    <td style={{ ...td, fontSize: '0.85rem', color: '#6b7280', maxWidth: '180px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                      {a.notes || ''}
                    </td>
                    <td style={td}>
                      <button onClick={() => setDeleteTarget(a)} style={delBtnStyle}>Del</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )
        )}
      </section>

      {showAdd && (
        <Modal title="New Inventory Adjustment" onClose={() => setShowAdd(false)}>
          <AdjustmentForm products={products} onSave={handleAdd} onCancel={() => setShowAdd(false)} saving={saving} />
        </Modal>
      )}

      {deleteTarget && (
        <Modal title="Delete Adjustment?" onClose={() => setDeleteTarget(null)} small>
          <p style={{ color: '#374151', marginBottom: '20px' }}>
            Delete this adjustment? Stock level will recalculate.
          </p>
          <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
            <button onClick={() => setDeleteTarget(null)} style={cancelBtn}>Cancel</button>
            <button onClick={() => handleDelete(deleteTarget.inventory_adjustment_id)} style={redBtn}>Delete</button>
          </div>
        </Modal>
      )}
    </div>
  );
}

function AdjustmentForm({ products, onSave, onCancel, saving }) {
  const [product, setProduct] = useState('');
  const [type, setType] = useState('Addition');
  const [quantity, setQuantity] = useState('');
  const [reason, setReason] = useState('');
  const [notes, setNotes] = useState('');
  const [adjustedBy, setAdjustedBy] = useState('');
  const [date, setDate] = useState(new Date().toISOString().slice(0, 10));

  function handleSubmit(e) {
    e.preventDefault();
    if (!product || quantity === '' || !type) return;
    onSave({
      product,
      adjustment_type: type,
      quantity: parseInt(quantity),
      reason: reason.trim() || null,
      notes: notes.trim() || null,
      adjusted_by: adjustedBy.trim() || null,
      date
    });
  }

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '13px' }}>
      <div>
        <label style={labelStyle}>Product <span style={{ color: '#dc2626' }}>*</span></label>
        <select style={inputStyle} value={product} onChange={e => setProduct(e.target.value)} required>
          <option value="">— Select product —</option>
          {products.map(p => (
            <option key={p.product_id} value={p.product_id}>
              {p.sku} — {p.display_name || p.sku} (stock: {p.stock_quantity ?? 0})
            </option>
          ))}
        </select>
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Type <span style={{ color: '#dc2626' }}>*</span></label>
          <select style={inputStyle} value={type} onChange={e => setType(e.target.value)} required>
            {ADJ_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
          </select>
        </div>
        <div>
          <label style={labelStyle}>Quantity <span style={{ color: '#dc2626' }}>*</span></label>
          <input style={inputStyle} type="number" value={quantity} onChange={e => setQuantity(e.target.value)} placeholder="e.g. 100" required />
        </div>
        <div>
          <label style={labelStyle}>Date</label>
          <input style={inputStyle} type="date" value={date} onChange={e => setDate(e.target.value)} />
        </div>
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Reason</label>
          <input style={inputStyle} value={reason} onChange={e => setReason(e.target.value)} placeholder="Restock, Damage, Transfer…" />
        </div>
        <div>
          <label style={labelStyle}>Adjusted By</label>
          <input style={inputStyle} value={adjustedBy} onChange={e => setAdjustedBy(e.target.value)} placeholder="name" />
        </div>
      </div>
      <div>
        <label style={labelStyle}>Notes</label>
        <textarea style={{ ...inputStyle, minHeight: '60px', resize: 'vertical' }} value={notes} onChange={e => setNotes(e.target.value)} />
      </div>
      <div style={{ background: '#f9fafb', borderRadius: '7px', padding: '10px 14px', fontSize: '0.83rem', color: '#6b7280' }}>
        Stock is derived from the sum of all adjustment quantities. Positive quantities add stock, negative subtract.
      </div>
      <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end', marginTop: '4px' }}>
        <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>
        <button type="submit" disabled={saving || !product || quantity === ''} style={saveBtn(saving || !product || quantity === '')}>
          {saving ? 'Saving…' : 'Save'}
        </button>
      </div>
    </form>
  );
}

function Modal({ title, children, onClose, small }) {
  return (
    <div style={modalOverlay} onClick={e => { if (e.target === e.currentTarget) onClose(); }}>
      <div style={{ ...modalBox, maxWidth: small ? '420px' : '560px' }}>
        <h2 style={{ marginBottom: '18px', fontSize: '1.1rem', fontWeight: 700 }}>{title}</h2>
        {children}
      </div>
    </div>
  );
}

function Kpi({ label, value, color, bg }) {
  return (
    <div style={{ background: bg, borderRadius: '10px', padding: '14px 16px', border: '1px solid #e5e7eb' }}>
      <div style={{ fontSize: '0.72rem', color: '#6b7280', fontWeight: 700, textTransform: 'uppercase', letterSpacing: '0.05em' }}>{label}</div>
      <div style={{ fontSize: '1.4rem', fontWeight: 700, color, marginTop: '4px' }}>{value}</div>
    </div>
  );
}

function typeBadge(t) {
  const c = {
    Correction: ['#eff6ff', '#1e40af', '#bfdbfe'],
    Addition:   ['#f0fdf4', '#15803d', '#86efac'],
    Removal:    ['#fef2f2', '#dc2626', '#fca5a5'],
    Shrinkage:  ['#fef2f2', '#dc2626', '#fca5a5'],
    Damage:     ['#fef2f2', '#dc2626', '#fca5a5'],
    Transfer:   ['#f5f3ff', '#6d28d9', '#c4b5fd']
  }[t] || ['#f3f4f6', '#374151', '#d1d5db'];
  return { ...badge, background: c[0], color: c[1], borderColor: c[2] };
}

const th = { padding: '11px 14px', textAlign: 'left', fontSize: '0.77rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em' };
const td = { padding: '11px 14px', fontSize: '0.9rem', verticalAlign: 'middle' };
const badge = { display: 'inline-block', padding: '2px 9px', borderRadius: '10px', fontWeight: 600, fontSize: '0.78rem', border: '1px solid' };
const sectionHeader = { padding: '13px 18px', borderBottom: '1px solid #e5e7eb', background: '#fafafa', fontWeight: 700, fontSize: '0.95rem', color: '#374151' };
const addBtn = { padding: '10px 22px', background: '#0891b2', color: '#fff', border: 'none', borderRadius: '8px', fontWeight: 700, fontSize: '0.95rem', cursor: 'pointer' };
const delBtnStyle = { padding: '3px 9px', borderRadius: '5px', background: '#fff', color: '#dc2626', fontSize: '0.78rem', fontWeight: 600, border: '1px solid #fca5a5', cursor: 'pointer' };
const cancelBtn = { padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer' };
const redBtn = { padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' };
function saveBtn(disabled) { return { padding: '8px 22px', borderRadius: '7px', border: 'none', background: disabled ? '#e5e7eb' : '#0891b2', color: disabled ? '#9ca3af' : '#fff', fontSize: '0.9rem', fontWeight: 600, cursor: disabled ? 'not-allowed' : 'pointer' }; }
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };
