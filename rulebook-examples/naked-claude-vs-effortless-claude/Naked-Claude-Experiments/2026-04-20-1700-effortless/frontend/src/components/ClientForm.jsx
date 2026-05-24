import { useState, useEffect } from 'react';
import { getStatuses, getClientCategories } from './api';

function fmtPct(d) {
  if (d == null) return '';
  const n = parseFloat(d);
  return isFinite(n) ? `${(n * 100).toFixed(n < 0.1 ? 1 : 0)}%` : '';
}

const inputStyle = {
  width: '100%',
  padding: '8px 12px',
  borderRadius: '7px',
  border: '1px solid #d1d5db',
  fontSize: '0.95rem',
  outline: 'none',
  boxSizing: 'border-box'
};

export default function ClientForm({ initial, onSave, onCancel, saving }) {
  const [name, setName] = useState(initial?.name || '');
  const [company_name, setCompanyName] = useState(initial?.company_name || '');
  const [email, setEmail] = useState(initial?.email || '');
  const [phone, setPhone] = useState(initial?.phone || '');
  const [billing_address, setBillingAddress] = useState(initial?.billing_address || '');
  const [shipping_address, setShippingAddress] = useState(initial?.shipping_address || '');
  const [notes, setNotes] = useState(initial?.notes || '');
  const [status, setStatus] = useState(initial?.status || '');
  const [category, setCategory] = useState(initial?.category || '');
  const [statuses, setStatuses] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loadingLookups, setLoadingLookups] = useState(true);

  useEffect(() => {
    Promise.all([
      getStatuses().catch(() => []),
      getClientCategories().catch(() => [])
    ]).then(([s, c]) => {
      setStatuses(s);
      setCategories(c);
    }).finally(() => setLoadingLookups(false));
  }, []);

  const selectedStatus = statuses.find(s => s.statuse_id === status);
  const willBeStopped = selectedStatus?.is_blocking === true;
  const selectedCategory = categories.find(c => c.client_categorie_id === category);

  function handleSubmit(e) {
    e.preventDefault();
    if (!name.trim()) return;
    onSave({
      name: name.trim(),
      company_name: company_name.trim() || null,
      email: email.trim() || null,
      phone: phone.trim() || null,
      billing_address: billing_address.trim() || null,
      shipping_address: shipping_address.trim() || null,
      notes: notes.trim() || null,
      status: status || null,
      category: category || null
    });
  }

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '14px' }}>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Name <span style={{ color: '#dc2626' }}>*</span></label>
          <input style={inputStyle} value={name} onChange={e => setName(e.target.value)} placeholder="Client name" required autoFocus />
        </div>
        <div>
          <label style={labelStyle}>Company</label>
          <input style={inputStyle} value={company_name} onChange={e => setCompanyName(e.target.value)} placeholder="Company name" />
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Email</label>
          <input style={inputStyle} type="email" value={email} onChange={e => setEmail(e.target.value)} placeholder="email@example.com" />
        </div>
        <div>
          <label style={labelStyle}>Phone</label>
          <input style={inputStyle} value={phone} onChange={e => setPhone(e.target.value)} placeholder="+1-555-000-0000" />
        </div>
      </div>

      <div>
        <label style={labelStyle}>Billing Address</label>
        <textarea style={{ ...inputStyle, minHeight: '60px', resize: 'vertical' }} value={billing_address} onChange={e => setBillingAddress(e.target.value)} placeholder="Billing address..." />
      </div>

      <div>
        <label style={labelStyle}>Shipping Address</label>
        <textarea style={{ ...inputStyle, minHeight: '60px', resize: 'vertical' }} value={shipping_address} onChange={e => setShippingAddress(e.target.value)} placeholder="Shipping address..." />
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Status</label>
          {loadingLookups ? (
            <p style={{ color: '#9ca3af', fontSize: '0.85rem' }}>Loading…</p>
          ) : (
            <select style={{ ...inputStyle, cursor: 'pointer', background: '#fff' }} value={status} onChange={e => setStatus(e.target.value)}>
              <option value="">— No status —</option>
              {statuses.map(s => (
                <option key={s.statuse_id} value={s.statuse_id}>
                  {s.display_name}{s.is_blocking ? ' 🔴' : ''}
                </option>
              ))}
            </select>
          )}
        </div>
        <div>
          <label style={labelStyle}>Category</label>
          {loadingLookups ? (
            <p style={{ color: '#9ca3af', fontSize: '0.85rem' }}>Loading…</p>
          ) : (
            <select style={{ ...inputStyle, cursor: 'pointer', background: '#fff' }} value={category} onChange={e => setCategory(e.target.value)}>
              <option value="">— No category —</option>
              {categories.map(c => (
                <option key={c.client_categorie_id} value={c.client_categorie_id}>
                  {c.name}{parseFloat(c.discount) > 0 ? ` (${fmtPct(c.discount)} off)` : ''}
                </option>
              ))}
            </select>
          )}
        </div>
      </div>

      {(selectedStatus || selectedCategory) && (
        <div style={{ display: 'grid', gridTemplateColumns: selectedStatus && selectedCategory ? '1fr 1fr' : '1fr', gap: '12px' }}>
          {selectedStatus && (
            <div style={{
              padding: '8px 12px', borderRadius: '7px',
              background: willBeStopped ? '#fef2f2' : '#f0fdf4',
              border: `1px solid ${willBeStopped ? '#fca5a5' : '#86efac'}`,
              fontSize: '0.83rem', color: willBeStopped ? '#991b1b' : '#166534'
            }}>
              {willBeStopped ? '🚫 This client will be marked as Stopped' : '✓ Not stopped'}
              {selectedStatus.description && (
                <div style={{ marginTop: '4px', color: '#6b7280', fontStyle: 'italic' }}>{selectedStatus.description}</div>
              )}
            </div>
          )}
          {selectedCategory && (
            <div style={{
              padding: '8px 12px', borderRadius: '7px',
              background: '#eff6ff', border: '1px solid #bfdbfe',
              fontSize: '0.83rem', color: '#1e40af'
            }}>
              {parseFloat(selectedCategory.discount) > 0
                ? `💰 ${fmtPct(selectedCategory.discount)} discount applies`
                : 'No discount for this category'}
              {selectedCategory.notes && (
                <div style={{ marginTop: '4px', color: '#6b7280', fontStyle: 'italic' }}>{selectedCategory.notes}</div>
              )}
            </div>
          )}
        </div>
      )}

      <div>
        <label style={labelStyle}>Notes</label>
        <textarea style={{ ...inputStyle, minHeight: '60px', resize: 'vertical' }} value={notes} onChange={e => setNotes(e.target.value)} placeholder="Optional notes..." />
      </div>

      <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end', marginTop: '4px' }}>
        {onCancel && (
          <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>
        )}
        <button type="submit" disabled={saving || !name.trim()} style={saveBtn(saving || !name.trim())}>
          {saving ? 'Saving…' : 'Save'}
        </button>
      </div>
    </form>
  );
}

const labelStyle = { display: 'block', fontSize: '0.85rem', fontWeight: 600, marginBottom: '5px', color: '#374151' };
const cancelBtn = {
  padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db',
  background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer'
};
function saveBtn(disabled) {
  return {
    padding: '8px 22px', borderRadius: '7px', border: 'none',
    background: disabled ? '#e5e7eb' : '#4f46e5',
    color: disabled ? '#9ca3af' : '#fff',
    fontSize: '0.9rem', fontWeight: 600, cursor: disabled ? 'not-allowed' : 'pointer'
  };
}
