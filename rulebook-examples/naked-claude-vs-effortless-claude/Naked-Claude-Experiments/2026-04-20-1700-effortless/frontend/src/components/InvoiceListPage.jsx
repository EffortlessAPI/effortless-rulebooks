import { useState, useEffect } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { getInvoices, createInvoice, deleteInvoice, getClients } from './api';

function fmt(n) { return n != null ? '$' + parseFloat(n).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : '—'; }

export default function InvoiceListPage() {
  const [invoices, setInvoices] = useState([]);
  const [clients, setClients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showAdd, setShowAdd] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [deleteError, setDeleteError] = useState(null);
  const [searchParams] = useSearchParams();
  const filterClient = searchParams.get('client');

  async function load() {
    try {
      setLoading(true);
      const [i, c] = await Promise.all([getInvoices(), getClients()]);
      setInvoices(i);
      setClients(c);
      setError(null);
    } catch (e) { setError(e.message); } finally { setLoading(false); }
  }
  useEffect(() => { load(); }, []);

  // If redirected from client page with ?client=..., open add form
  useEffect(() => { if (filterClient) setShowAdd(true); }, [filterClient]);

  async function handleAdd(formData) {
    setSaving(true);
    try { await createInvoice(formData); setShowAdd(false); await load(); }
    catch (e) { alert(e.message); } finally { setSaving(false); }
  }

  async function handleDelete(id) {
    setDeleteError(null);
    try { await deleteInvoice(id); setDeleteTarget(null); await load(); }
    catch (e) { setDeleteError(e.message); }
  }

  return (
    <div style={{ maxWidth: '1100px', margin: '0 auto', padding: '28px 16px' }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '22px' }}>
        <div>
          <h1 style={{ fontSize: '1.55rem', fontWeight: 700, color: '#1a1a1a' }}>Invoices</h1>
          <p style={{ color: '#6b7280', fontSize: '0.88rem', marginTop: '2px' }}>All client invoices, most recent first</p>
        </div>
        <button onClick={() => setShowAdd(true)} style={addBtn}>+ New Invoice</button>
      </div>

      {loading && <p style={{ color: '#6b7280' }}>Loading…</p>}
      {error && <p style={{ color: '#dc2626' }}>Error: {error}</p>}

      {showAdd && (
        <Modal title="New Invoice" onClose={() => setShowAdd(false)}>
          <InvoiceForm clients={clients} onSave={handleAdd} onCancel={() => setShowAdd(false)} saving={saving} initialClient={filterClient} />
        </Modal>
      )}

      {deleteTarget && (
        <Modal title="Delete Invoice?" onClose={() => { setDeleteTarget(null); setDeleteError(null); }} small>
          {deleteError ? (
            <div style={{ padding: '10px 14px', background: '#fef2f2', border: '1px solid #fca5a5', borderRadius: '7px', color: '#991b1b', marginBottom: '16px', fontSize: '0.88rem' }}>⚠️ {deleteError}</div>
          ) : (
            <p style={{ color: '#374151', marginBottom: '20px' }}>
              Delete invoice <strong>#{deleteTarget.invoice_number}</strong> for <strong>{deleteTarget.client_name || deleteTarget.client}</strong>?
              <span style={{ display: 'block', marginTop: '8px', fontSize: '0.85rem', color: '#dc2626' }}>This will also delete all line items and payments for this invoice.</span>
            </p>
          )}
          <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
            <button onClick={() => { setDeleteTarget(null); setDeleteError(null); }} style={cancelBtn}>{deleteError ? 'Close' : 'Cancel'}</button>
            {!deleteError && <button onClick={() => handleDelete(deleteTarget.invoice_id)} style={redBtn}>Delete</button>}
          </div>
        </Modal>
      )}

      {!loading && !error && (
        <div style={{ background: '#fff', borderRadius: '12px', boxShadow: '0 1px 4px rgba(0,0,0,0.07)', overflow: 'hidden' }}>
          {invoices.length === 0 ? (
            <p style={{ padding: '32px', textAlign: 'center', color: '#6b7280' }}>No invoices yet.</p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>Invoice #</th>
                  <th style={th}>Client</th>
                  <th style={th}>Date</th>
                  <th style={th}>Status</th>
                  <th style={th}>Items</th>
                  <th style={th}>Total</th>
                  <th style={th}>Paid</th>
                  <th style={th}>Due</th>
                  <th style={th}>Payment</th>
                  <th style={{ ...th, width: '80px' }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {invoices.map(i => (
                  <tr key={i.invoice_id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={td}>
                      <Link to={`/invoices/${encodeURIComponent(i.invoice_id)}`} style={{ fontWeight: 700, color: '#4f46e5', textDecoration: 'none' }}>#{i.invoice_number}</Link>
                      {i.is_big_order && (
                        <span style={{ marginLeft: '6px', fontSize: '0.68rem', padding: '1px 7px', borderRadius: '8px', background: '#fef3c7', color: '#92400e', border: '1px solid #fcd34d', fontWeight: 700, letterSpacing: '0.04em' }} title="Big order (subtotal > $350)">
                          💰 BIG
                        </span>
                      )}
                    </td>
                    <td style={td}>
                      <Link to={`/clients/${encodeURIComponent(i.client)}`} style={{ color: '#374151', textDecoration: 'none', fontWeight: 500 }}>{i.client_name || i.client}</Link>
                    </td>
                    <td style={{ ...td, fontSize: '0.85rem', color: '#6b7280' }}>{i.order_date ? new Date(i.order_date).toLocaleDateString() : '—'}</td>
                    <td style={td}><span style={{ fontSize: '0.82rem', padding: '2px 8px', borderRadius: '8px', background: '#f3f4f6', border: '1px solid #d1d5db', color: '#374151' }}>{i.order_status || '—'}</span></td>
                    <td style={{ ...td, textAlign: 'center' }}>{i.item_count ?? 0}</td>
                    <td style={{ ...td, fontWeight: 600 }}>{fmt(i.invoice_total)}</td>
                    <td style={td}>{fmt(i.total_paid)}</td>
                    <td style={{ ...td, color: parseFloat(i.amount_due) > 0 ? '#dc2626' : '#16a34a', fontWeight: 600 }}>{fmt(i.amount_due)}</td>
                    <td style={td}><PayBadge label={i.payment_status_label} /></td>
                    <td style={td}>
                      <div style={{ display: 'flex', gap: '6px' }}>
                        <Link to={`/invoices/${encodeURIComponent(i.invoice_id)}`} style={editLinkStyle}>View</Link>
                        <button onClick={() => { setDeleteTarget(i); setDeleteError(null); }} style={delBtnStyle}>Del</button>
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

function InvoiceForm({ clients, onSave, onCancel, saving, initialClient }) {
  const inputStyle = { width: '100%', padding: '8px 12px', borderRadius: '7px', border: '1px solid #d1d5db', fontSize: '0.95rem', outline: 'none', boxSizing: 'border-box' };
  const labelStyle = { display: 'block', fontSize: '0.85rem', fontWeight: 600, marginBottom: '5px', color: '#374151' };
  const [client, setClient] = useState(initialClient || '');
  const [invoiceNumber, setInvoiceNumber] = useState('');
  const [orderDate, setOrderDate] = useState(new Date().toISOString().split('T')[0]);
  const [orderStatus, setOrderStatus] = useState('New');
  const [taxRate, setTaxRate] = useState('0');
  const [notes, setNotes] = useState('');

  function handleSubmit(e) {
    e.preventDefault();
    if (!client || !invoiceNumber.trim()) return;
    onSave({ client, invoice_number: invoiceNumber.trim(), order_date: orderDate || null, order_status: orderStatus, tax_rate: parseFloat(taxRate) || 0, notes: notes.trim() || null });
  }

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '13px' }}>
      <div>
        <label style={labelStyle}>Client <span style={{ color: '#dc2626' }}>*</span></label>
        <select style={{ ...inputStyle, cursor: 'pointer', background: '#fff' }} value={client} onChange={e => setClient(e.target.value)} required>
          <option value="">— Select client —</option>
          {clients.map(c => <option key={c.client_id} value={c.client_id}>{c.name}{c.company_name ? ` (${c.company_name})` : ''}</option>)}
        </select>
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Invoice Number <span style={{ color: '#dc2626' }}>*</span></label>
          <input style={inputStyle} value={invoiceNumber} onChange={e => setInvoiceNumber(e.target.value)} placeholder="e.g. 1001" required />
        </div>
        <div>
          <label style={labelStyle}>Invoice Date</label>
          <input style={inputStyle} type="date" value={orderDate} onChange={e => setOrderDate(e.target.value)} />
        </div>
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Invoice Status</label>
          <select style={{ ...inputStyle, cursor: 'pointer', background: '#fff' }} value={orderStatus} onChange={e => setOrderStatus(e.target.value)}>
            {['New','Pending','Processing','Shipped','Delivered','Cancelled','Returned'].map(s => <option key={s} value={s}>{s}</option>)}
          </select>
        </div>
        <div>
          <label style={labelStyle}>Tax Rate (e.g. 0.085)</label>
          <input style={inputStyle} type="number" step="0.001" min="0" max="1" value={taxRate} onChange={e => setTaxRate(e.target.value)} placeholder="0.085" />
        </div>
      </div>
      <div>
        <label style={labelStyle}>Notes</label>
        <textarea style={{ ...inputStyle, minHeight: '60px', resize: 'vertical' }} value={notes} onChange={e => setNotes(e.target.value)} />
      </div>
      <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
        {onCancel && <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>}
        <button type="submit" disabled={saving || !client || !invoiceNumber.trim()} style={saveBtn(saving || !client || !invoiceNumber.trim())}>{saving ? 'Saving…' : 'Create Invoice'}</button>
      </div>
    </form>
  );
}

function PayBadge({ label }) {
  const colors = { Paid: ['#f0fdf4','#16a34a','#86efac'], Partial: ['#fffbeb','#d97706','#fcd34d'], Unpaid: ['#fef2f2','#dc2626','#fca5a5'] };
  const [bg, color, border] = colors[label] || ['#f3f4f6','#374151','#d1d5db'];
  return <span style={{ fontSize: '0.8rem', padding: '2px 9px', borderRadius: '10px', background: bg, color, border: `1px solid ${border}`, fontWeight: 700 }}>{label || '—'}</span>;
}

function Modal({ title, children, onClose, small }) {
  return (
    <div style={modalOverlay}>
      <div style={{ ...modalBox, maxWidth: small ? '420px' : '560px' }}>
        <h2 style={{ marginBottom: '18px', fontSize: '1.1rem', fontWeight: 700 }}>{title}</h2>
        {children}
      </div>
    </div>
  );
}

const th = { padding: '11px 12px', textAlign: 'left', fontSize: '0.76rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em' };
const td = { padding: '11px 12px', fontSize: '0.9rem', verticalAlign: 'middle' };
const addBtn = { padding: '10px 22px', background: '#0891b2', color: '#fff', border: 'none', borderRadius: '8px', fontWeight: 700, fontSize: '0.95rem', cursor: 'pointer' };
const editLinkStyle = { padding: '4px 10px', borderRadius: '5px', background: '#eff6ff', color: '#1e40af', fontSize: '0.8rem', fontWeight: 600, border: '1px solid #bfdbfe', textDecoration: 'none' };
const delBtnStyle = { padding: '4px 10px', borderRadius: '5px', background: '#fff', color: '#dc2626', fontSize: '0.8rem', fontWeight: 600, border: '1px solid #fca5a5', cursor: 'pointer' };
const cancelBtn = { padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer' };
const redBtn = { padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' };
function saveBtn(disabled) { return { padding: '8px 22px', borderRadius: '7px', border: 'none', background: disabled ? '#e5e7eb' : '#0891b2', color: disabled ? '#9ca3af' : '#fff', fontSize: '0.9rem', fontWeight: 600, cursor: disabled ? 'not-allowed' : 'pointer' }; }
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };
