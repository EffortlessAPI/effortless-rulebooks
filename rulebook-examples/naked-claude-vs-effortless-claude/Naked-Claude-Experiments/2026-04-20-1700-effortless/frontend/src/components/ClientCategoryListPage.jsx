import { useState, useEffect } from 'react';
import { getClientCategories, createClientCategory, updateClientCategory, deleteClientCategory } from './api';

function fmtPct(d) {
  if (d == null) return '—';
  const n = parseFloat(d);
  if (!isFinite(n)) return '—';
  return `${(n * 100).toFixed(n < 0.1 ? 1 : 0)}%`;
}

export default function ClientCategoryListPage() {
  const [cats, setCats] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [editing, setEditing] = useState(null); // id or 'new'
  const [form, setForm] = useState({ name: '', notes: '', discount: '' });
  const [saving, setSaving] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [deleteError, setDeleteError] = useState(null);

  async function load() {
    try {
      setLoading(true);
      setCats(await getClientCategories());
      setError(null);
    } catch (e) { setError(e.message); } finally { setLoading(false); }
  }
  useEffect(() => { load(); }, []);

  function startNew() {
    setEditing('new');
    setForm({ name: '', notes: '', discount: '' });
  }
  function startEdit(c) {
    setEditing(c.client_categorie_id);
    setForm({ name: c.name || '', notes: c.notes || '', discount: c.discount != null ? c.discount : '' });
  }
  function cancel() {
    setEditing(null);
    setForm({ name: '', notes: '', discount: '' });
  }

  async function save() {
    if (!form.name.trim()) return;
    setSaving(true);
    try {
      const payload = {
        name: form.name.trim(),
        notes: form.notes.trim() || null,
        discount: form.discount === '' ? 0 : parseFloat(form.discount)
      };
      if (editing === 'new') await createClientCategory(payload);
      else await updateClientCategory(editing, payload);
      cancel();
      await load();
    } catch (e) { alert(e.message); } finally { setSaving(false); }
  }

  async function doDelete(id) {
    setDeleteError(null);
    try {
      await deleteClientCategory(id);
      setDeleteTarget(null);
      await load();
    } catch (e) { setDeleteError(e.message); }
  }

  return (
    <div style={{ maxWidth: '800px', margin: '0 auto', padding: '28px 16px' }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '22px' }}>
        <div>
          <h1 style={{ fontSize: '1.55rem', fontWeight: 700, color: '#1a1a1a' }}>Client Categories</h1>
          <p style={{ color: '#6b7280', fontSize: '0.88rem', marginTop: '2px' }}>
            Category labels and the discount applied to clients in each category
          </p>
        </div>
        {editing == null && (
          <button onClick={startNew} style={primaryBtn}>+ Add Category</button>
        )}
      </div>

      {loading && <p style={{ color: '#6b7280' }}>Loading…</p>}
      {error && <p style={{ color: '#dc2626' }}>Error: {error}</p>}

      {!loading && !error && (
        <div style={{ background: '#fff', borderRadius: '12px', boxShadow: '0 1px 4px rgba(0,0,0,0.07)', overflow: 'hidden' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <thead>
              <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                <th style={th}>Name</th>
                <th style={th}>Discount</th>
                <th style={th}>Clients</th>
                <th style={th}>Notes</th>
                <th style={{ ...th, width: '130px' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {editing === 'new' && (
                <EditRow form={form} setForm={setForm} saving={saving} onSave={save} onCancel={cancel} />
              )}
              {cats.map(c => (
                editing === c.client_categorie_id ? (
                  <EditRow key={c.client_categorie_id} form={form} setForm={setForm} saving={saving} onSave={save} onCancel={cancel} />
                ) : (
                  <tr key={c.client_categorie_id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={td}>
                      <div style={{ fontWeight: 600, color: '#1a1a1a' }}>{c.name}</div>
                      <div style={{ fontSize: '0.78rem', color: '#9ca3af', fontFamily: 'monospace', marginTop: '2px' }}>{c.client_categorie_id}</div>
                    </td>
                    <td style={td}>
                      <span style={{
                        padding: '2px 10px', borderRadius: '10px',
                        background: parseFloat(c.discount) > 0 ? '#f0fdf4' : '#f3f4f6',
                        color: parseFloat(c.discount) > 0 ? '#166534' : '#6b7280',
                        fontWeight: 700, fontSize: '0.85rem',
                        border: `1px solid ${parseFloat(c.discount) > 0 ? '#86efac' : '#d1d5db'}`
                      }}>{fmtPct(c.discount)}</span>
                    </td>
                    <td style={td}>
                      <span style={{
                        background: c.client_count > 0 ? '#dbeafe' : '#f3f4f6',
                        color: c.client_count > 0 ? '#1e40af' : '#9ca3af',
                        padding: '2px 9px', borderRadius: '10px', fontWeight: 700, fontSize: '0.82rem'
                      }}>{c.client_count}</span>
                    </td>
                    <td style={{ ...td, color: '#6b7280', fontSize: '0.87rem' }}>{c.notes || <span style={{ color: '#d1d5db' }}>—</span>}</td>
                    <td style={td}>
                      <div style={{ display: 'flex', gap: '6px' }}>
                        <button onClick={() => startEdit(c)} style={smallBtn('#eff6ff', '#1e40af', '#bfdbfe')}>Edit</button>
                        <button onClick={() => { setDeleteTarget(c); setDeleteError(null); }} style={smallBtn('#fff', '#dc2626', '#fca5a5')}>Del</button>
                      </div>
                    </td>
                  </tr>
                )
              ))}
              {cats.length === 0 && editing !== 'new' && (
                <tr><td colSpan={5} style={{ padding: '32px', textAlign: 'center', color: '#6b7280' }}>No categories yet.</td></tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      {deleteTarget && (
        <div style={modalOverlay}>
          <div style={modalBox}>
            <h2 style={{ marginBottom: '12px', fontSize: '1.1rem', fontWeight: 700 }}>Delete Category?</h2>
            {deleteError ? (
              <div style={{ padding: '10px 14px', background: '#fef2f2', border: '1px solid #fca5a5', borderRadius: '7px', color: '#991b1b', marginBottom: '16px', fontSize: '0.88rem' }}>
                ⚠️ {deleteError}
              </div>
            ) : (
              <p style={{ color: '#374151', marginBottom: '20px' }}>
                Delete <strong>{deleteTarget.name}</strong>? This cannot be undone.
              </p>
            )}
            <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
              <button onClick={() => { setDeleteTarget(null); setDeleteError(null); }} style={cancelBtn}>
                {deleteError ? 'Close' : 'Cancel'}
              </button>
              {!deleteError && (
                <button onClick={() => doDelete(deleteTarget.client_categorie_id)} style={redBtn}>Delete</button>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

function EditRow({ form, setForm, saving, onSave, onCancel }) {
  return (
    <tr style={{ borderBottom: '1px solid #e5e7eb', background: '#fefce8' }}>
      <td style={td}>
        <input
          style={inpStyle}
          value={form.name}
          onChange={e => setForm({ ...form, name: e.target.value })}
          placeholder="Category name"
          autoFocus
        />
      </td>
      <td style={td}>
        <input
          style={{ ...inpStyle, width: '80px' }}
          type="number"
          step="0.01"
          min="0"
          max="1"
          value={form.discount}
          onChange={e => setForm({ ...form, discount: e.target.value })}
          placeholder="0.00"
        />
        <span style={{ marginLeft: '6px', fontSize: '0.78rem', color: '#9ca3af' }}>(0–1)</span>
      </td>
      <td style={td}>—</td>
      <td style={td}>
        <input
          style={inpStyle}
          value={form.notes}
          onChange={e => setForm({ ...form, notes: e.target.value })}
          placeholder="Notes"
        />
      </td>
      <td style={td}>
        <div style={{ display: 'flex', gap: '6px' }}>
          <button onClick={onSave} disabled={saving || !form.name.trim()} style={smallBtn('#4f46e5', '#fff', '#4f46e5')}>
            {saving ? '…' : 'Save'}
          </button>
          <button onClick={onCancel} style={smallBtn('#fff', '#6b7280', '#d1d5db')}>Cancel</button>
        </div>
      </td>
    </tr>
  );
}

const th = { padding: '11px 16px', textAlign: 'left', fontSize: '0.78rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em' };
const td = { padding: '12px 16px', fontSize: '0.92rem', verticalAlign: 'middle' };
const inpStyle = { padding: '6px 10px', borderRadius: '6px', border: '1px solid #d1d5db', fontSize: '0.9rem', outline: 'none' };
const primaryBtn = { padding: '10px 22px', background: '#4f46e5', color: '#fff', border: 'none', borderRadius: '8px', fontWeight: 700, fontSize: '0.95rem', cursor: 'pointer', boxShadow: '0 1px 4px rgba(79,70,229,0.3)' };
const cancelBtn = { padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer' };
const redBtn = { padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' };
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', maxWidth: '420px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };

function smallBtn(bg, color, border) {
  return {
    padding: '4px 12px', borderRadius: '5px', background: bg, color,
    fontSize: '0.8rem', fontWeight: 600, border: `1px solid ${border}`, cursor: 'pointer'
  };
}
