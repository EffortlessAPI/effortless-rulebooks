import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { getInvoice, updateInvoice, deleteInvoice, createLineItem, updateLineItem, deleteLineItem, createPayment, updatePayment, deletePayment, getProducts } from './api';

function fmt(n) { return n != null ? '$' + parseFloat(n).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : '—'; }
function fmtDate(d) { return d ? new Date(d).toLocaleDateString() : '—'; }
function fmtPct(p) { return p != null && p > 0 ? `${(p * 100).toFixed(0)}%` : '—'; }
const inp = { width: '100%', padding: '7px 10px', borderRadius: '6px', border: '1px solid #d1d5db', fontSize: '0.9rem', outline: 'none', boxSizing: 'border-box' };
const lbl = { display: 'block', fontSize: '0.8rem', fontWeight: 600, marginBottom: '4px', color: '#374151' };

function PayBadge({ label }) {
  const colors = { Paid: ['#f0fdf4','#16a34a','#86efac'], Partial: ['#fffbeb','#d97706','#fcd34d'], Unpaid: ['#fef2f2','#dc2626','#fca5a5'] };
  const [bg, color, border] = colors[label] || ['#f3f4f6','#374151','#d1d5db'];
  return <span style={{ fontSize: '0.82rem', padding: '3px 10px', borderRadius: '10px', background: bg, color, border: `1px solid ${border}`, fontWeight: 700 }}>{label || '—'}</span>;
}

export default function InvoiceDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [invoice, setInvoice] = useState(null);
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleteConfirm, setDeleteConfirm] = useState(false);
  const [deleteError, setDeleteError] = useState(null);
  // Line item editing
  const [editingLi, setEditingLi] = useState(null);
  const [liForm, setLiForm] = useState({});
  const [liSaving, setLiSaving] = useState(false);
  // Payment editing
  const [editingPmt, setEditingPmt] = useState(null);
  const [pmtForm, setPmtForm] = useState({});
  const [pmtSaving, setPmtSaving] = useState(false);

  async function load() {
    try {
      setLoading(true);
      const [i, p] = await Promise.all([getInvoice(id), getProducts()]);
      setInvoice(i); setProducts(p); setError(null);
    } catch (e) { setError(e.message); } finally { setLoading(false); }
  }
  useEffect(() => { load(); }, [id]);

  async function handleSaveInvoice(formData) {
    setSaving(true);
    try { await updateInvoice(id, formData); await load(); setEditing(false); }
    catch (e) { alert(e.message); } finally { setSaving(false); }
  }

  async function handleDeleteInvoice() {
    setDeleteError(null);
    try { await deleteInvoice(id); navigate('/invoices'); }
    catch (e) { setDeleteError(e.message); }
  }

  // Line item actions
  function startAddLi() { setLiForm({ product: '', quantity: 1, unit_price: '', discount_percent: 0, notes: '' }); setEditingLi('new'); }
  function startEditLi(li) { setLiForm({ product: li.product, quantity: li.quantity, unit_price: li.unit_price, discount_percent: li.discount_percent || 0, notes: li.notes || '' }); setEditingLi(li.invoice_line_item_id); }

  async function handleSaveLi() {
    setLiSaving(true);
    try {
      if (editingLi === 'new') await createLineItem(id, liForm);
      else await updateLineItem(editingLi, liForm);
      setEditingLi(null); await load();
    } catch (e) { alert(e.message); } finally { setLiSaving(false); }
  }

  async function handleDeleteLi(liId) {
    if (!confirm('Delete this line item?')) return;
    try { await deleteLineItem(liId); await load(); }
    catch (e) { alert(e.message); }
  }

  // Payment actions
  function startAddPmt() {
    setPmtForm({ payment_date: new Date().toISOString().split('T')[0], amount: '', payment_method: 'CreditCard', payment_status: 'Completed', transaction_id: '', notes: '' });
    setEditingPmt('new');
  }
  function startEditPmt(pmt) {
    setPmtForm({
      payment_date: pmt.payment_date ? new Date(pmt.payment_date).toISOString().split('T')[0] : '',
      amount: pmt.amount, payment_method: pmt.payment_method || '',
      payment_status: pmt.payment_status || '', transaction_id: pmt.transaction_id || '', notes: pmt.notes || ''
    });
    setEditingPmt(pmt.payment_id);
  }

  async function handleSavePmt() {
    setPmtSaving(true);
    try {
      if (editingPmt === 'new') await createPayment(id, pmtForm);
      else await updatePayment(editingPmt, pmtForm);
      setEditingPmt(null); await load();
    } catch (e) { alert(e.message); } finally { setPmtSaving(false); }
  }

  async function handleDeletePmt(pmtId) {
    if (!confirm('Delete this payment?')) return;
    try { await deletePayment(pmtId); await load(); }
    catch (e) { alert(e.message); }
  }

  if (loading) return <div style={page}><p style={{ color: '#6b7280' }}>Loading…</p></div>;
  if (error) return <div style={page}><p style={{ color: '#dc2626' }}>Error: {error}</p></div>;
  if (!invoice) return null;

  const lineItems = invoice.line_item_list || [];
  const payments = invoice.payment_list || [];

  return (
    <div style={page}>
      <div style={{ maxWidth: '900px', margin: '0 auto' }}>
        <Link to="/invoices" style={backLink}>← Back to invoices</Link>

        {/* Invoice header card */}
        <div style={{ background: '#fff', borderRadius: '14px', padding: '24px 28px', boxShadow: '0 1px 6px rgba(0,0,0,0.08)', marginBottom: '20px', borderLeft: '4px solid #0891b2' }}>
          {!editing ? (
            <>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '18px' }}>
                <div>
                  <div style={{ fontSize: '0.8rem', color: '#9ca3af', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.05em', marginBottom: '4px' }}>Invoice</div>
                  <h1 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: '6px', display: 'flex', alignItems: 'center', gap: '10px' }}>
                    #{invoice.invoice_number}
                    {invoice.is_big_order && (
                      <span style={{ fontSize: '0.72rem', padding: '3px 10px', borderRadius: '10px', background: '#fef3c7', color: '#92400e', border: '1px solid #fcd34d', fontWeight: 800, letterSpacing: '0.04em' }}>
                        💰 BIG ORDER
                      </span>
                    )}
                  </h1>
                  <div style={{ display: 'flex', gap: '8px', alignItems: 'center', flexWrap: 'wrap' }}>
                    <Link to={`/clients/${encodeURIComponent(invoice.client)}`} style={{ color: '#4f46e5', fontWeight: 600, textDecoration: 'none', fontSize: '0.9rem' }}>
                      {invoice.client_name || invoice.client}
                    </Link>
                    {invoice.client_company_name && invoice.client_company_name !== invoice.client_name && (
                      <span style={{ fontSize: '0.82rem', color: '#6b7280' }}>({invoice.client_company_name})</span>
                    )}
                    <span style={{ color: '#d1d5db' }}>•</span>
                    <span style={{ fontSize: '0.85rem', color: '#6b7280' }}>{fmtDate(invoice.order_date)}</span>
                    <span style={{ fontSize: '0.82rem', padding: '2px 8px', borderRadius: '8px', background: '#f3f4f6', border: '1px solid #d1d5db', color: '#374151' }}>{invoice.order_status}</span>
                    <PayBadge label={invoice.payment_status_label} />
                  </div>
                  {(invoice.client_email || invoice.client_phone) && (
                    <div style={{ display: 'flex', gap: '14px', marginTop: '6px', fontSize: '0.82rem', color: '#6b7280' }}>
                      {invoice.client_email && <span>✉ {invoice.client_email}</span>}
                      {invoice.client_phone && <span>☎ {invoice.client_phone}</span>}
                    </div>
                  )}
                </div>
                <div style={{ display: 'flex', gap: '8px' }}>
                  <button onClick={() => setEditing(true)} style={editBtn}>Edit</button>
                  <button onClick={() => { setDeleteConfirm(true); setDeleteError(null); }} style={deleteBtn}>Delete</button>
                </div>
              </div>

              {/* Money summary */}
              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(6,1fr)', gap: '12px', padding: '14px 0', borderTop: '1px solid #f3f4f6', borderBottom: '1px solid #f3f4f6', marginBottom: '14px' }}>
                {[['Items', invoice.item_count ?? 0], ['Subtotal', fmt(invoice.sub_total)], ['Tax', fmt(invoice.tax_amount)], ['Total', fmt(invoice.invoice_total)], ['Paid', fmt(invoice.total_paid)], ['Due', fmt(invoice.amount_due)]].map(([label, val]) => (
                  <div key={label}>
                    <div style={{ fontSize: '0.72rem', fontWeight: 700, color: '#9ca3af', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: '3px' }}>{label}</div>
                    <div style={{ fontSize: '1.05rem', fontWeight: 700, color: label === 'Due' && parseFloat(invoice.amount_due) > 0 ? '#dc2626' : '#374151' }}>{val}</div>
                  </div>
                ))}
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '14px', fontSize: '0.88rem', color: '#6b7280' }}>
                {invoice.notes && <div><span style={{ fontWeight: 600, color: '#374151' }}>Notes: </span>{invoice.notes}</div>}
                <div><span style={{ fontWeight: 600, color: '#374151' }}>Tax Rate: </span>{invoice.tax_rate != null ? `${(invoice.tax_rate * 100).toFixed(1)}%` : '—'}</div>
                {invoice.shipping_address && <div><span style={{ fontWeight: 600, color: '#374151' }}>Ship to: </span><span style={{ whiteSpace: 'pre-wrap' }}>{invoice.shipping_address}</span></div>}
                {invoice.billing_address && <div><span style={{ fontWeight: 600, color: '#374151' }}>Bill to: </span><span style={{ whiteSpace: 'pre-wrap' }}>{invoice.billing_address}</span></div>}
              </div>
            </>
          ) : (
            <>
              <h2 style={{ marginBottom: '18px', fontSize: '1.1rem', fontWeight: 700 }}>Edit Invoice #{invoice.invoice_number}</h2>
              <InvoiceEditForm initial={invoice} onSave={handleSaveInvoice} onCancel={() => setEditing(false)} saving={saving} />
            </>
          )}
        </div>

        {/* Line Items */}
        <div style={{ background: '#fff', borderRadius: '14px', padding: '22px 28px', boxShadow: '0 1px 6px rgba(0,0,0,0.06)', marginBottom: '20px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '14px' }}>
            <h2 style={{ fontSize: '1rem', fontWeight: 700, color: '#374151' }}>
              Line Items
              <span style={{ marginLeft: '8px', background: '#dbeafe', color: '#1e40af', padding: '2px 9px', borderRadius: '10px', fontSize: '0.8rem', fontWeight: 700 }}>{lineItems.length}</span>
            </h2>
            <button onClick={startAddLi} style={{ padding: '6px 14px', background: '#0891b2', color: '#fff', border: 'none', borderRadius: '6px', fontWeight: 600, fontSize: '0.85rem', cursor: 'pointer' }}>+ Add Line</button>
          </div>

          {editingLi === 'new' && (
            <LineItemForm form={liForm} setForm={setLiForm} products={products} onSave={handleSaveLi} onCancel={() => setEditingLi(null)} saving={liSaving} />
          )}

          {lineItems.length === 0 && editingLi !== 'new' && (
            <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>No line items yet. Add one above.</p>
          )}

          {lineItems.length > 0 && (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>#</th>
                  <th style={th}>Product</th>
                  <th style={th}>Qty</th>
                  <th style={th}>Unit Price</th>
                  <th style={th}>Discount</th>
                  <th style={th}>Pre-disc</th>
                  <th style={th}>Subtotal</th>
                  <th style={th}>Notes</th>
                  <th style={{ ...th, width: '80px' }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {lineItems.map(li => (
                  <>
                    <tr key={li.invoice_line_item_id} style={{ borderBottom: editingLi === li.invoice_line_item_id ? 'none' : '1px solid #e5e7eb' }}>
                      <td style={td}>{li.line_number}</td>
                      <td style={td}>
                        <Link to={`/products/${encodeURIComponent(li.product)}`} style={{ color: '#4f46e5', textDecoration: 'none', fontWeight: 600 }}>
                          {li.product_display_name || li.product}
                        </Link>
                        <div style={{ fontSize: '0.75rem', color: '#9ca3af', fontFamily: 'monospace' }}>{li.product_sku}</div>
                      </td>
                      <td style={td}>{li.quantity}</td>
                      <td style={td}>{fmt(li.unit_price)}</td>
                      <td style={td}>{fmtPct(li.discount_percent)}</td>
                      <td style={{ ...td, color: '#6b7280' }}>{fmt(li.pre_discount)}</td>
                      <td style={{ ...td, fontWeight: 700 }}>{fmt(li.sub_total)}</td>
                      <td style={{ ...td, color: '#6b7280', fontSize: '0.82rem', maxWidth: '120px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{li.notes || '—'}</td>
                      <td style={td}>
                        <div style={{ display: 'flex', gap: '4px' }}>
                          <button onClick={() => startEditLi(li)} style={smEditBtn}>Edit</button>
                          <button onClick={() => handleDeleteLi(li.invoice_line_item_id)} style={smDelBtn}>Del</button>
                        </div>
                      </td>
                    </tr>
                    {editingLi === li.invoice_line_item_id && (
                      <tr key={`edit-${li.invoice_line_item_id}`} style={{ borderBottom: '1px solid #e5e7eb' }}>
                        <td colSpan={9} style={{ padding: '14px 16px', background: '#f9fafb' }}>
                          <LineItemForm form={liForm} setForm={setLiForm} products={products} onSave={handleSaveLi} onCancel={() => setEditingLi(null)} saving={liSaving} />
                        </td>
                      </tr>
                    )}
                  </>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {/* Payments */}
        <div style={{ background: '#fff', borderRadius: '14px', padding: '22px 28px', boxShadow: '0 1px 6px rgba(0,0,0,0.06)' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '14px' }}>
            <h2 style={{ fontSize: '1rem', fontWeight: 700, color: '#374151' }}>
              Payments
              <span style={{ marginLeft: '8px', background: '#dbeafe', color: '#1e40af', padding: '2px 9px', borderRadius: '10px', fontSize: '0.8rem', fontWeight: 700 }}>{payments.length}</span>
            </h2>
            <button onClick={startAddPmt} style={{ padding: '6px 14px', background: '#7c3aed', color: '#fff', border: 'none', borderRadius: '6px', fontWeight: 600, fontSize: '0.85rem', cursor: 'pointer' }}>+ Record Payment</button>
          </div>

          {editingPmt === 'new' && (
            <PaymentForm form={pmtForm} setForm={setPmtForm} onSave={handleSavePmt} onCancel={() => setEditingPmt(null)} saving={pmtSaving} />
          )}

          {payments.length === 0 && editingPmt !== 'new' && (
            <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>No payments yet.</p>
          )}

          {payments.length > 0 && (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>#</th>
                  <th style={th}>Date</th>
                  <th style={th}>Amount</th>
                  <th style={th}>Method</th>
                  <th style={th}>Status</th>
                  <th style={th}>Completed Amt</th>
                  <th style={th}>Transaction ID</th>
                  <th style={th}>Notes</th>
                  <th style={{ ...th, width: '80px' }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {payments.map(p => (
                  <>
                    <tr key={p.payment_id} style={{ borderBottom: editingPmt === p.payment_id ? 'none' : '1px solid #e5e7eb', background: p.payment_status === 'Failed' || p.payment_status === 'Cancelled' ? '#fafafa' : '#fff' }}>
                      <td style={td}>{p.payment_number}</td>
                      <td style={{ ...td, fontSize: '0.85rem' }}>{fmtDate(p.payment_date)}</td>
                      <td style={{ ...td, fontWeight: 600 }}>{fmt(p.amount)}</td>
                      <td style={{ ...td, fontSize: '0.85rem', color: '#6b7280' }}>{p.payment_method || '—'}</td>
                      <td style={td}>
                        <span style={{ fontSize: '0.8rem', padding: '2px 8px', borderRadius: '8px', background: p.is_completed ? '#f0fdf4' : '#fef2f2', color: p.is_completed ? '#16a34a' : '#dc2626', border: `1px solid ${p.is_completed ? '#86efac' : '#fca5a5'}`, fontWeight: 600 }}>
                          {p.payment_status}
                        </span>
                      </td>
                      <td style={{ ...td, fontWeight: 600, color: parseFloat(p.completed_amount || 0) > 0 ? '#16a34a' : '#9ca3af' }}>{fmt(p.completed_amount)}</td>
                      <td style={{ ...td, fontSize: '0.78rem', fontFamily: 'monospace', color: '#6b7280', maxWidth: '120px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{p.transaction_id || '—'}</td>
                      <td style={{ ...td, color: '#6b7280', fontSize: '0.82rem', maxWidth: '120px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{p.notes || '—'}</td>
                      <td style={td}>
                        <div style={{ display: 'flex', gap: '4px' }}>
                          <button onClick={() => startEditPmt(p)} style={smEditBtn}>Edit</button>
                          <button onClick={() => handleDeletePmt(p.payment_id)} style={smDelBtn}>Del</button>
                        </div>
                      </td>
                    </tr>
                    {editingPmt === p.payment_id && (
                      <tr key={`edit-${p.payment_id}`} style={{ borderBottom: '1px solid #e5e7eb' }}>
                        <td colSpan={9} style={{ padding: '14px 16px', background: '#f9fafb' }}>
                          <PaymentForm form={pmtForm} setForm={setPmtForm} onSave={handleSavePmt} onCancel={() => setEditingPmt(null)} saving={pmtSaving} />
                        </td>
                      </tr>
                    )}
                  </>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {/* Delete confirm */}
        {deleteConfirm && (
          <div style={modalOverlay}>
            <div style={modalBox}>
              <h2 style={{ marginBottom: '12px', fontSize: '1.1rem', fontWeight: 700 }}>Delete Invoice?</h2>
              {deleteError ? (
                <div style={{ padding: '10px 14px', background: '#fef2f2', border: '1px solid #fca5a5', borderRadius: '7px', color: '#991b1b', marginBottom: '16px', fontSize: '0.88rem' }}>⚠️ {deleteError}</div>
              ) : (
                <p style={{ color: '#374151', marginBottom: '20px' }}>
                  Delete invoice <strong>#{invoice.invoice_number}</strong>? All line items and payments will also be deleted. This cannot be undone.
                </p>
              )}
              <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
                <button onClick={() => { setDeleteConfirm(false); setDeleteError(null); }} style={cancelBtn}>{deleteError ? 'Close' : 'Cancel'}</button>
                {!deleteError && <button onClick={handleDeleteInvoice} style={{ padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' }}>Delete</button>}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

function LineItemForm({ form, setForm, products, onSave, onCancel, saving }) {
  const prd = products.find(p => p.product_id === form.product);
  return (
    <div style={{ background: '#ecfeff', border: '1px solid #a5f3fc', borderRadius: '8px', padding: '14px 16px', marginBottom: '4px' }}>
      <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr 1fr 1fr 2fr', gap: '10px', alignItems: 'end' }}>
        <div>
          <label style={lbl}>Product</label>
          <select style={{ ...inp, background: '#fff' }} value={form.product} onChange={e => {
            const p = products.find(x => x.product_id === e.target.value);
            setForm(f => ({ ...f, product: e.target.value, unit_price: p ? p.unit_price : f.unit_price }));
          }}>
            <option value="">— Select —</option>
            {products.filter(p => p.is_active).map(p => <option key={p.product_id} value={p.product_id}>{p.display_name} ({p.sku})</option>)}
          </select>
        </div>
        <div>
          <label style={lbl}>Qty</label>
          <input style={inp} type="number" min="1" value={form.quantity} onChange={e => setForm(f => ({ ...f, quantity: parseInt(e.target.value) || 1 }))} />
        </div>
        <div>
          <label style={lbl}>Unit Price</label>
          <input style={inp} type="number" step="0.01" min="0" value={form.unit_price} onChange={e => setForm(f => ({ ...f, unit_price: e.target.value }))} placeholder={prd ? prd.unit_price : ''} />
        </div>
        <div>
          <label style={lbl}>Discount</label>
          <input style={inp} type="number" step="0.01" min="0" max="1" value={form.discount_percent} onChange={e => setForm(f => ({ ...f, discount_percent: parseFloat(e.target.value) || 0 }))} placeholder="0" />
        </div>
        <div>
          <label style={lbl}>Notes</label>
          <input style={inp} value={form.notes} onChange={e => setForm(f => ({ ...f, notes: e.target.value }))} placeholder="Optional notes…" />
        </div>
      </div>
      <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end', marginTop: '10px' }}>
        <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>
        <button type="button" onClick={onSave} disabled={saving || !form.product} style={saveBtn(saving || !form.product)}>{saving ? 'Saving…' : 'Save Line'}</button>
      </div>
    </div>
  );
}

function PaymentForm({ form, setForm, onSave, onCancel, saving }) {
  return (
    <div style={{ background: '#faf5ff', border: '1px solid #d8b4fe', borderRadius: '8px', padding: '14px 16px', marginBottom: '4px' }}>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr 1fr 2fr', gap: '10px', alignItems: 'end' }}>
        <div>
          <label style={lbl}>Date</label>
          <input style={inp} type="date" value={form.payment_date} onChange={e => setForm(f => ({ ...f, payment_date: e.target.value }))} />
        </div>
        <div>
          <label style={lbl}>Amount</label>
          <input style={inp} type="number" step="0.01" min="0" value={form.amount} onChange={e => setForm(f => ({ ...f, amount: e.target.value }))} />
        </div>
        <div>
          <label style={lbl}>Method</label>
          <select style={{ ...inp, background: '#fff' }} value={form.payment_method} onChange={e => setForm(f => ({ ...f, payment_method: e.target.value }))}>
            {['CreditCard','BankTransfer','Cash','Check','PayPal','Other'].map(m => <option key={m} value={m}>{m}</option>)}
          </select>
        </div>
        <div>
          <label style={lbl}>Status</label>
          <select style={{ ...inp, background: '#fff' }} value={form.payment_status} onChange={e => setForm(f => ({ ...f, payment_status: e.target.value }))}>
            {['Pending','Completed','Failed','Refunded','Cancelled'].map(s => <option key={s} value={s}>{s}</option>)}
          </select>
        </div>
        <div>
          <label style={lbl}>Transaction ID</label>
          <input style={inp} value={form.transaction_id} onChange={e => setForm(f => ({ ...f, transaction_id: e.target.value }))} placeholder="External ref…" />
        </div>
      </div>
      <div style={{ marginTop: '10px' }}>
        <label style={lbl}>Notes</label>
        <input style={{ ...inp, width: '100%' }} value={form.notes} onChange={e => setForm(f => ({ ...f, notes: e.target.value }))} placeholder="Optional notes…" />
      </div>
      <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end', marginTop: '10px' }}>
        <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>
        <button type="button" onClick={onSave} disabled={saving || !form.amount} style={saveBtn(saving || !form.amount)}>{saving ? 'Saving…' : 'Save Payment'}</button>
      </div>
    </div>
  );
}

function InvoiceEditForm({ initial, onSave, onCancel, saving }) {
  const [orderStatus, setOrderStatus] = useState(initial?.order_status || 'New');
  const [orderDate, setOrderDate] = useState(initial?.order_date ? new Date(initial.order_date).toISOString().split('T')[0] : '');
  const [taxRate, setTaxRate] = useState(initial?.tax_rate ?? 0);
  const [shippingAddress, setShippingAddress] = useState(initial?.shipping_address || '');
  const [billingAddress, setBillingAddress] = useState(initial?.billing_address || '');
  const [notes, setNotes] = useState(initial?.notes || '');

  function handleSubmit(e) {
    e.preventDefault();
    onSave({ order_status: orderStatus, order_date: orderDate || null, tax_rate: parseFloat(taxRate) || 0, shipping_address: shippingAddress || null, billing_address: billingAddress || null, notes: notes || null });
  }

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '13px' }}>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '12px' }}>
        <div>
          <label style={lbl}>Invoice Status</label>
          <select style={{ ...inp, background: '#fff' }} value={orderStatus} onChange={e => setOrderStatus(e.target.value)}>
            {['New','Pending','Processing','Shipped','Delivered','Cancelled','Returned'].map(s => <option key={s} value={s}>{s}</option>)}
          </select>
        </div>
        <div>
          <label style={lbl}>Invoice Date</label>
          <input style={inp} type="date" value={orderDate} onChange={e => setOrderDate(e.target.value)} />
        </div>
        <div>
          <label style={lbl}>Tax Rate</label>
          <input style={inp} type="number" step="0.001" min="0" max="1" value={taxRate} onChange={e => setTaxRate(e.target.value)} />
        </div>
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div>
          <label style={lbl}>Shipping Address</label>
          <textarea style={{ ...inp, minHeight: '60px', resize: 'vertical' }} value={shippingAddress} onChange={e => setShippingAddress(e.target.value)} />
        </div>
        <div>
          <label style={lbl}>Billing Address</label>
          <textarea style={{ ...inp, minHeight: '60px', resize: 'vertical' }} value={billingAddress} onChange={e => setBillingAddress(e.target.value)} />
        </div>
      </div>
      <div>
        <label style={lbl}>Notes</label>
        <textarea style={{ ...inp, minHeight: '60px', resize: 'vertical' }} value={notes} onChange={e => setNotes(e.target.value)} />
      </div>
      <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
        <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>
        <button type="submit" disabled={saving} style={saveBtn(saving)}>{saving ? 'Saving…' : 'Save'}</button>
      </div>
    </form>
  );
}

const page = { maxWidth: '960px', margin: '0 auto', padding: '28px 16px' };
const backLink = { color: '#4f46e5', fontWeight: 600, fontSize: '0.9rem', display: 'inline-flex', alignItems: 'center', gap: '4px', marginBottom: '20px', textDecoration: 'none' };
const th = { padding: '9px 12px', textAlign: 'left', fontSize: '0.73rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em' };
const td = { padding: '10px 12px', fontSize: '0.88rem', verticalAlign: 'middle' };
const editBtn = { padding: '7px 16px', borderRadius: '7px', border: '1px solid #c7d2fe', background: '#eff6ff', color: '#1e40af', fontWeight: 600, fontSize: '0.88rem', cursor: 'pointer' };
const deleteBtn = { padding: '7px 14px', borderRadius: '7px', border: '1px solid #fca5a5', background: '#fff', color: '#dc2626', fontWeight: 600, fontSize: '0.88rem', cursor: 'pointer' };
const smEditBtn = { padding: '3px 8px', borderRadius: '4px', background: '#eff6ff', color: '#1e40af', fontSize: '0.76rem', fontWeight: 600, border: '1px solid #bfdbfe', cursor: 'pointer' };
const smDelBtn = { padding: '3px 8px', borderRadius: '4px', background: '#fff', color: '#dc2626', fontSize: '0.76rem', fontWeight: 600, border: '1px solid #fca5a5', cursor: 'pointer' };
const cancelBtn = { padding: '7px 16px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.88rem', fontWeight: 600, cursor: 'pointer' };
function saveBtn(disabled) { return { padding: '7px 18px', borderRadius: '7px', border: 'none', background: disabled ? '#e5e7eb' : '#0891b2', color: disabled ? '#9ca3af' : '#fff', fontSize: '0.88rem', fontWeight: 600, cursor: disabled ? 'not-allowed' : 'pointer' }; }
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', maxWidth: '450px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };
