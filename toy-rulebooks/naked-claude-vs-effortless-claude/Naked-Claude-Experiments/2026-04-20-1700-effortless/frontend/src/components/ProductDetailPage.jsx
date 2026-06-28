import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { getProduct, updateProduct, deleteProduct, createInventoryAdjustment, deleteInventoryAdjustment } from './api';

const ADJ_TYPES = ['Correction', 'Addition', 'Removal', 'Shrinkage', 'Damage', 'Transfer'];
function fmtDate(d) { if (!d) return '—'; try { return new Date(d).toLocaleDateString(); } catch { return d; } }

const inputStyle = { width: '100%', padding: '8px 12px', borderRadius: '7px', border: '1px solid #d1d5db', fontSize: '0.95rem', outline: 'none', boxSizing: 'border-box' };
const labelStyle = { display: 'block', fontSize: '0.85rem', fontWeight: 600, marginBottom: '5px', color: '#374151' };
function fmt(n) { return n != null ? '$' + parseFloat(n).toFixed(2) : '—'; }

export default function ProductDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleteConfirm, setDeleteConfirm] = useState(false);
  const [deleteError, setDeleteError] = useState(null);
  const [showAdjForm, setShowAdjForm] = useState(false);
  const [adjSaving, setAdjSaving] = useState(false);

  async function load() {
    try { setLoading(true); setProduct(await getProduct(id)); setError(null); }
    catch (e) { setError(e.message); } finally { setLoading(false); }
  }
  useEffect(() => { load(); }, [id]);

  async function handleSave(formData) {
    setSaving(true);
    try { await updateProduct(id, formData); await load(); setEditing(false); }
    catch (e) { alert(e.message); } finally { setSaving(false); }
  }

  async function handleDelete() {
    setDeleteError(null);
    try { await deleteProduct(id); navigate('/products'); }
    catch (e) { setDeleteError(e.message); }
  }

  async function handleAddAdjustment(data) {
    setAdjSaving(true);
    try { await createInventoryAdjustment({ ...data, product: id }); setShowAdjForm(false); await load(); }
    catch (e) { alert(e.message); } finally { setAdjSaving(false); }
  }

  async function handleDeleteAdjustment(adjId) {
    if (!confirm('Delete this adjustment? Stock level will recalculate.')) return;
    try { await deleteInventoryAdjustment(adjId); await load(); }
    catch (e) { alert(e.message); }
  }

  if (loading) return <div style={page}><p style={{ color: '#6b7280' }}>Loading…</p></div>;
  if (error) return <div style={page}><p style={{ color: '#dc2626' }}>Error: {error}</p></div>;
  if (!product) return null;

  const lineItems = product.line_item_list || [];
  const adjustments = product.adjustment_list || [];

  return (
    <div style={page}>
      <div style={{ maxWidth: '820px', margin: '0 auto' }}>
        <Link to="/products" style={backLink}>← Back to products</Link>

        <div style={{ background: '#fff', borderRadius: '14px', padding: '28px', boxShadow: '0 1px 6px rgba(0,0,0,0.08)', borderLeft: product.is_active ? '4px solid #059669' : '4px solid #d1d5db', marginBottom: '20px' }}>
          {!editing ? (
            <>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '16px' }}>
                <div>
                  <div style={{ fontFamily: 'monospace', fontSize: '0.9rem', color: '#4f46e5', fontWeight: 700, marginBottom: '4px' }}>{product.sku}</div>
                  <h1 style={{ fontSize: '1.4rem', fontWeight: 700, marginBottom: '8px' }}>{product.display_name || product.sku}</h1>
                  <div style={{ display: 'flex', gap: '6px', flexWrap: 'wrap' }}>
                    <span style={{ fontSize: '0.82rem', padding: '2px 10px', borderRadius: '10px', background: product.is_active ? '#f0fdf4' : '#f3f4f6', color: product.is_active ? '#16a34a' : '#9ca3af', border: `1px solid ${product.is_active ? '#86efac' : '#e5e7eb'}`, fontWeight: 600 }}>
                      {product.is_active ? '✓ Active' : 'Inactive'}
                    </span>
                    {product.is_high_margin && (
                      <span style={{ fontSize: '0.78rem', padding: '2px 10px', borderRadius: '10px', background: '#fef3c7', color: '#92400e', border: '1px solid #fcd34d', fontWeight: 700 }}>
                        ⭐ HIGH MARGIN
                      </span>
                    )}
                  </div>
                </div>
                <div style={{ display: 'flex', gap: '8px' }}>
                  <button onClick={() => setEditing(true)} style={editBtn}>Edit</button>
                  <button onClick={() => { setDeleteConfirm(true); setDeleteError(null); }} style={deleteBtn}>Delete</button>
                </div>
              </div>

              {product.description && (
                <div style={{ borderTop: '1px solid #f3f4f6', paddingTop: '14px', marginBottom: '14px' }}>
                  <label style={fieldLabel}>Description</label>
                  <p style={{ color: '#374151', fontSize: '0.92rem', lineHeight: '1.55' }}>{product.description}</p>
                </div>
              )}

              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3,1fr)', gap: '16px', borderTop: '1px solid #f3f4f6', paddingTop: '14px' }}>
                <div>
                  <label style={fieldLabel}>Unit Price</label>
                  <div style={{ fontSize: '1.4rem', fontWeight: 700, color: '#374151' }}>{fmt(product.unit_price)}</div>
                </div>
                <div>
                  <label style={fieldLabel}>Cost</label>
                  <div style={{ fontSize: '1.4rem', fontWeight: 700, color: '#6b7280' }}>{fmt(product.cost)}</div>
                </div>
                <div>
                  <label style={fieldLabel}>Stock</label>
                  <div style={{ fontSize: '1.4rem', fontWeight: 700, color: product.stock_quantity === 0 ? '#dc2626' : '#374151' }}>
                    {product.stock_quantity ?? '—'}
                    {product.stock_quantity === 0 && <span style={{ fontSize: '0.75rem', marginLeft: '6px', color: '#dc2626' }}>OUT OF STOCK</span>}
                  </div>
                </div>
              </div>
              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3,1fr)', gap: '16px', borderTop: '1px solid #f3f4f6', paddingTop: '14px', marginTop: '14px' }}>
                <div>
                  <label style={fieldLabel}>Profit (per unit)</label>
                  <div style={{ fontSize: '1.2rem', fontWeight: 700, color: parseFloat(product.profit) > 0 ? '#059669' : '#6b7280' }}>{fmt(product.profit)}</div>
                </div>
                <div>
                  <label style={fieldLabel}>Margin</label>
                  <div style={{ fontSize: '1.2rem', fontWeight: 700, color: product.is_high_margin ? '#b45309' : '#374151' }}>
                    {product.margin != null ? (parseFloat(product.margin) * 100).toFixed(1) + '%' : '—'}
                  </div>
                </div>
                <div>
                  <label style={fieldLabel}>COGS (stock × cost)</label>
                  <div style={{ fontSize: '1.2rem', fontWeight: 700, color: '#6b7280' }}>{fmt(product.cogs)}</div>
                </div>
              </div>
            </>
          ) : (
            <>
              <h2 style={{ marginBottom: '18px', fontSize: '1.15rem', fontWeight: 700 }}>Edit Product</h2>
              <ProductEditForm initial={product} onSave={handleSave} onCancel={() => setEditing(false)} saving={saving} />
            </>
          )}
        </div>

        {/* Inventory adjustments */}
        <div style={{ background: '#fff', borderRadius: '14px', padding: '22px 28px', boxShadow: '0 1px 6px rgba(0,0,0,0.06)', marginBottom: '20px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '14px' }}>
            <h2 style={{ fontSize: '1rem', fontWeight: 700, color: '#374151' }}>
              Inventory Adjustments
              <span style={{ marginLeft: '8px', background: '#cffafe', color: '#0e7490', padding: '2px 9px', borderRadius: '10px', fontSize: '0.8rem', fontWeight: 700 }}>{adjustments.length}</span>
            </h2>
            <button onClick={() => setShowAdjForm(true)} style={{ padding: '6px 14px', borderRadius: '7px', border: 'none', background: '#0891b2', color: '#fff', fontWeight: 700, fontSize: '0.85rem', cursor: 'pointer' }}>+ Adjust Stock</button>
          </div>
          {adjustments.length === 0 ? (
            <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>No adjustments yet. Current stock: <strong>{product.stock_quantity ?? 0}</strong></p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>Date</th>
                  <th style={th}>Type</th>
                  <th style={{ ...th, textAlign: 'right' }}>Qty</th>
                  <th style={th}>Reason</th>
                  <th style={th}>By</th>
                  <th style={th}>Notes</th>
                  <th style={{ ...th, width: '50px' }}></th>
                </tr>
              </thead>
              <tbody>
                {adjustments.map(a => (
                  <tr key={a.inventory_adjustment_id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={td}>{fmtDate(a.date)}</td>
                    <td style={td}><span style={{ fontSize: '0.78rem', padding: '2px 8px', borderRadius: '10px', background: '#eff6ff', color: '#1e40af', border: '1px solid #bfdbfe', fontWeight: 600 }}>{a.adjustment_type || '—'}</span></td>
                    <td style={{ ...td, textAlign: 'right', fontWeight: 700, color: a.quantity < 0 ? '#dc2626' : '#15803d' }}>{a.quantity > 0 ? '+' : ''}{a.quantity}</td>
                    <td style={{ ...td, fontSize: '0.87rem' }}>{a.reason || '—'}</td>
                    <td style={{ ...td, fontSize: '0.87rem', color: '#6b7280' }}>{a.adjusted_by || '—'}</td>
                    <td style={{ ...td, fontSize: '0.85rem', color: '#6b7280', maxWidth: '180px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{a.notes || ''}</td>
                    <td style={td}>
                      <button onClick={() => handleDeleteAdjustment(a.inventory_adjustment_id)} style={{ padding: '3px 9px', borderRadius: '5px', background: '#fff', color: '#dc2626', fontSize: '0.78rem', fontWeight: 600, border: '1px solid #fca5a5', cursor: 'pointer' }}>Del</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {/* Line items */}
        <div style={{ background: '#fff', borderRadius: '14px', padding: '22px 28px', boxShadow: '0 1px 6px rgba(0,0,0,0.06)' }}>
          <h2 style={{ fontSize: '1rem', fontWeight: 700, marginBottom: '14px', color: '#374151' }}>
            Invoice Line Items
            <span style={{ marginLeft: '8px', background: '#dbeafe', color: '#1e40af', padding: '2px 9px', borderRadius: '10px', fontSize: '0.8rem', fontWeight: 700 }}>{lineItems.length}</span>
          </h2>
          {lineItems.length === 0 ? (
            <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>No line items reference this product yet.</p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>Invoice</th>
                  <th style={th}>Qty</th>
                  <th style={th}>Unit Price</th>
                  <th style={th}>Discount</th>
                  <th style={th}>Subtotal</th>
                  <th style={th}>Invoice Status</th>
                </tr>
              </thead>
              <tbody>
                {lineItems.map(li => (
                  <tr key={li.invoice_line_item_id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={td}>
                      <Link to={`/invoices/${encodeURIComponent(li.invoice)}`} style={{ fontWeight: 600, color: '#4f46e5', textDecoration: 'none' }}>
                        #{li.invoice_number}
                      </Link>
                      <div style={{ fontSize: '0.75rem', color: '#9ca3af' }}>{li.client}</div>
                    </td>
                    <td style={td}>{li.quantity}</td>
                    <td style={td}>{fmt(li.unit_price)}</td>
                    <td style={td}>{li.discount_percent > 0 ? `${(li.discount_percent * 100).toFixed(0)}%` : '—'}</td>
                    <td style={{ ...td, fontWeight: 600 }}>{fmt(li.sub_total)}</td>
                    <td style={td}><span style={{ fontSize: '0.82rem', padding: '2px 8px', borderRadius: '8px', background: '#f3f4f6', border: '1px solid #d1d5db' }}>{li.order_status || '—'}</span></td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {showAdjForm && (
          <div style={modalOverlay}>
            <div style={modalBox}>
              <h2 style={{ marginBottom: '18px', fontSize: '1.1rem', fontWeight: 700 }}>Adjust Stock — {product.display_name || product.sku}</h2>
              <p style={{ fontSize: '0.85rem', color: '#6b7280', marginBottom: '14px' }}>Current stock: <strong>{product.stock_quantity ?? 0}</strong></p>
              <AdjustmentInlineForm onSave={handleAddAdjustment} onCancel={() => setShowAdjForm(false)} saving={adjSaving} />
            </div>
          </div>
        )}

        {deleteConfirm && (
          <div style={modalOverlay}>
            <div style={modalBox}>
              <h2 style={{ marginBottom: '12px', fontSize: '1.1rem', fontWeight: 700 }}>Delete Product?</h2>
              {deleteError ? (
                <div style={{ padding: '10px 14px', background: '#fef2f2', border: '1px solid #fca5a5', borderRadius: '7px', color: '#991b1b', marginBottom: '16px', fontSize: '0.88rem' }}>⚠️ {deleteError}</div>
              ) : (
                <p style={{ color: '#374151', marginBottom: '20px' }}>Delete <strong>{product.display_name || product.sku}</strong>? This cannot be undone.</p>
              )}
              <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
                <button onClick={() => { setDeleteConfirm(false); setDeleteError(null); }} style={cancelBtn}>{deleteError ? 'Close' : 'Cancel'}</button>
                {!deleteError && <button onClick={handleDelete} style={{ padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' }}>Delete</button>}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

function AdjustmentInlineForm({ onSave, onCancel, saving }) {
  const [type, setType] = useState('Addition');
  const [quantity, setQuantity] = useState('');
  const [reason, setReason] = useState('');
  const [notes, setNotes] = useState('');
  const [adjustedBy, setAdjustedBy] = useState('');
  const [date, setDate] = useState(new Date().toISOString().slice(0, 10));

  function handleSubmit(e) {
    e.preventDefault();
    if (quantity === '') return;
    onSave({
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
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Type</label>
          <select style={inputStyle} value={type} onChange={e => setType(e.target.value)}>
            {ADJ_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
          </select>
        </div>
        <div>
          <label style={labelStyle}>Quantity</label>
          <input style={inputStyle} type="number" value={quantity} onChange={e => setQuantity(e.target.value)} placeholder="e.g. 100" required autoFocus />
        </div>
        <div>
          <label style={labelStyle}>Date</label>
          <input style={inputStyle} type="date" value={date} onChange={e => setDate(e.target.value)} />
        </div>
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Reason</label>
          <input style={inputStyle} value={reason} onChange={e => setReason(e.target.value)} placeholder="Restock, Damage…" />
        </div>
        <div>
          <label style={labelStyle}>Adjusted By</label>
          <input style={inputStyle} value={adjustedBy} onChange={e => setAdjustedBy(e.target.value)} placeholder="name" />
        </div>
      </div>
      <div>
        <label style={labelStyle}>Notes</label>
        <textarea style={{ ...inputStyle, minHeight: '50px', resize: 'vertical' }} value={notes} onChange={e => setNotes(e.target.value)} />
      </div>
      <div style={{ background: '#f9fafb', borderRadius: '7px', padding: '10px 14px', fontSize: '0.82rem', color: '#6b7280' }}>
        Stock is the sum of all adjustment quantities. Use negative values to remove stock.
      </div>
      <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
        <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>
        <button type="submit" disabled={saving || quantity === ''} style={saveBtn(saving || quantity === '')}>{saving ? 'Saving…' : 'Save'}</button>
      </div>
    </form>
  );
}

function ProductEditForm({ initial, onSave, onCancel, saving }) {
  const [sku, setSku] = useState(initial?.sku || '');
  const [displayName, setDisplayName] = useState(initial?.display_name || '');
  const [description, setDescription] = useState(initial?.description || '');
  const [unitPrice, setUnitPrice] = useState(initial?.unit_price ?? '');
  const [cost, setCost] = useState(initial?.cost ?? '');
  const [isActive, setIsActive] = useState(initial ? (initial.is_active !== false) : true);

  function handleSubmit(e) {
    e.preventDefault();
    onSave({ sku: sku.trim().toUpperCase(), display_name: displayName.trim() || null, description: description.trim() || null, unit_price: unitPrice !== '' ? parseFloat(unitPrice) : null, cost: cost !== '' ? parseFloat(cost) : null, is_active: isActive });
  }

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '13px' }}>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div><label style={labelStyle}>SKU</label><input style={inputStyle} value={sku} onChange={e => setSku(e.target.value.toUpperCase())} /></div>
        <div><label style={labelStyle}>Display Name</label><input style={inputStyle} value={displayName} onChange={e => setDisplayName(e.target.value)} /></div>
      </div>
      <div><label style={labelStyle}>Description</label><textarea style={{ ...inputStyle, minHeight: '60px', resize: 'vertical' }} value={description} onChange={e => setDescription(e.target.value)} /></div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div><label style={labelStyle}>Unit Price</label><input style={inputStyle} type="number" step="0.01" min="0" value={unitPrice} onChange={e => setUnitPrice(e.target.value)} /></div>
        <div><label style={labelStyle}>Cost</label><input style={inputStyle} type="number" step="0.01" min="0" value={cost} onChange={e => setCost(e.target.value)} /></div>
      </div>
      <div style={{ fontSize: '0.82rem', color: '#6b7280', background: '#f9fafb', padding: '10px 14px', borderRadius: '7px' }}>
        Stock qty is derived from inventory adjustments — use "+ Adjust Stock" below to change it.
      </div>
      <label style={{ display: 'flex', alignItems: 'center', gap: '10px', cursor: 'pointer' }}>
        <input type="checkbox" checked={isActive} onChange={e => setIsActive(e.target.checked)} style={{ width: '18px', height: '18px', cursor: 'pointer' }} />
        <span style={{ fontSize: '0.92rem', fontWeight: 600, color: '#374151' }}>Active (sellable)</span>
      </label>
      <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
        <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>
        <button type="submit" disabled={saving} style={saveBtn(saving)}>{saving ? 'Saving…' : 'Save'}</button>
      </div>
    </form>
  );
}

const page = { maxWidth: '900px', margin: '0 auto', padding: '28px 16px' };
const fieldLabel = { display: 'block', fontSize: '0.75rem', fontWeight: 700, color: '#9ca3af', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: '5px' };
const th = { padding: '9px 14px', textAlign: 'left', fontSize: '0.75rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em' };
const td = { padding: '10px 14px', fontSize: '0.9rem', verticalAlign: 'middle' };
const backLink = { color: '#4f46e5', fontWeight: 600, fontSize: '0.9rem', display: 'inline-flex', alignItems: 'center', gap: '4px', marginBottom: '20px', textDecoration: 'none' };
const editBtn = { padding: '7px 16px', borderRadius: '7px', border: '1px solid #c7d2fe', background: '#eff6ff', color: '#1e40af', fontWeight: 600, fontSize: '0.88rem', cursor: 'pointer' };
const deleteBtn = { padding: '7px 14px', borderRadius: '7px', border: '1px solid #fca5a5', background: '#fff', color: '#dc2626', fontWeight: 600, fontSize: '0.88rem', cursor: 'pointer' };
const cancelBtn = { padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer' };
function saveBtn(disabled) { return { padding: '8px 22px', borderRadius: '7px', border: 'none', background: disabled ? '#e5e7eb' : '#059669', color: disabled ? '#9ca3af' : '#fff', fontSize: '0.9rem', fontWeight: 600, cursor: disabled ? 'not-allowed' : 'pointer' }; }
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', maxWidth: '420px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };
