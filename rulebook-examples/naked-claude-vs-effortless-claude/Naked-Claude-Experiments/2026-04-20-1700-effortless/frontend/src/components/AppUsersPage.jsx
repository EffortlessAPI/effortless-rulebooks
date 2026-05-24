import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getAppUsers, createAppUser, updateAppUser, deleteAppUser } from './api';

const inputStyle = { width: '100%', padding: '8px 12px', borderRadius: '7px', border: '1px solid #d1d5db', fontSize: '0.95rem', outline: 'none', boxSizing: 'border-box' };
const labelStyle = { display: 'block', fontSize: '0.85rem', fontWeight: 600, marginBottom: '5px', color: '#374151' };

export default function AppUsersPage() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showAdd, setShowAdd] = useState(false);
  const [editTarget, setEditTarget] = useState(null);
  const [saving, setSaving] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [deleteError, setDeleteError] = useState(null);

  async function load() {
    try { setLoading(true); setUsers(await getAppUsers()); setError(null); }
    catch (e) { setError(e.message); } finally { setLoading(false); }
  }
  useEffect(() => { load(); }, []);

  async function handleAdd(data) {
    setSaving(true);
    try { await createAppUser(data); setShowAdd(false); await load(); }
    catch (e) { alert(e.message); } finally { setSaving(false); }
  }

  async function handleEdit(data) {
    setSaving(true);
    try { await updateAppUser(editTarget.app_user_id, data); setEditTarget(null); await load(); }
    catch (e) { alert(e.message); } finally { setSaving(false); }
  }

  async function handleDelete(id) {
    setDeleteError(null);
    try { await deleteAppUser(id); setDeleteTarget(null); await load(); }
    catch (e) { setDeleteError(e.message); }
  }

  return (
    <div style={{ maxWidth: '1000px', margin: '0 auto', padding: '28px 16px' }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '22px' }}>
        <div>
          <h1 style={{ fontSize: '1.55rem', fontWeight: 700, color: '#1a1a1a' }}>App Users</h1>
          <p style={{ color: '#6b7280', fontSize: '0.88rem', marginTop: '2px' }}>People who approve clients and adjust inventory</p>
        </div>
        <button onClick={() => setShowAdd(true)} style={addBtn}>+ Add User</button>
      </div>

      {loading && <p style={{ color: '#6b7280' }}>Loading…</p>}
      {error && <p style={{ color: '#dc2626' }}>Error: {error}</p>}

      {(showAdd || editTarget) && (
        <Modal title={editTarget ? 'Edit User' : 'Add User'} small={false}>
          <AppUserForm
            initial={editTarget}
            onSave={editTarget ? handleEdit : handleAdd}
            onCancel={() => { setShowAdd(false); setEditTarget(null); }}
            saving={saving}
          />
        </Modal>
      )}

      {deleteTarget && (
        <Modal title="Delete User?" small>
          {deleteError ? (
            <div style={errBox}>⚠️ {deleteError}</div>
          ) : (
            <p style={{ color: '#374151', marginBottom: '20px' }}>
              Delete <strong>{deleteTarget.contact_name}</strong>? This cannot be undone.
            </p>
          )}
          <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
            <button onClick={() => { setDeleteTarget(null); setDeleteError(null); }} style={cancelBtn}>
              {deleteError ? 'Close' : 'Cancel'}
            </button>
            {!deleteError && <button onClick={() => handleDelete(deleteTarget.app_user_id)} style={redBtn}>Delete</button>}
          </div>
        </Modal>
      )}

      {!loading && !error && (
        <div style={{ background: '#fff', borderRadius: '12px', boxShadow: '0 1px 4px rgba(0,0,0,0.07)', overflow: 'hidden' }}>
          {users.length === 0 ? (
            <p style={{ padding: '32px', textAlign: 'center', color: '#6b7280' }}>
              No app users yet. Add one to start approving clients.
            </p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>Name</th>
                  <th style={th}>Email</th>
                  <th style={th}>Phone</th>
                  <th style={th}>Role</th>
                  <th style={th}>Approvals</th>
                  <th style={{ ...th, width: '100px' }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {users.map(u => (
                  <tr key={u.app_user_id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={td}>
                      <div style={{ fontWeight: 600, color: '#1a1a1a' }}>{u.contact_name || '—'}</div>
                      <div style={{ fontFamily: 'monospace', fontSize: '0.72rem', color: '#9ca3af' }}>{u.app_user_id}</div>
                    </td>
                    <td style={{ ...td, fontSize: '0.88rem', color: '#374151' }}>{u.email_address || '—'}</td>
                    <td style={{ ...td, fontSize: '0.88rem', color: '#6b7280' }}>{u.phone_number || '—'}</td>
                    <td style={td}>
                      {u.role ? (
                        <span style={{ fontSize: '0.8rem', padding: '2px 10px', borderRadius: '10px', background: '#eef2ff', color: '#4338ca', border: '1px solid #c7d2fe', fontWeight: 600 }}>
                          {u.role}
                        </span>
                      ) : <span style={{ color: '#d1d5db' }}>—</span>}
                    </td>
                    <td style={td}>
                      <Link to={`/approvals?user=${encodeURIComponent(u.app_user_id)}`} style={{
                        background: u.approval_count > 0 ? '#dbeafe' : '#f3f4f6',
                        color: u.approval_count > 0 ? '#1e40af' : '#9ca3af',
                        padding: '2px 10px', borderRadius: '10px', fontWeight: 700, fontSize: '0.8rem',
                        textDecoration: 'none'
                      }}>{u.approval_count}</Link>
                    </td>
                    <td style={td}>
                      <div style={{ display: 'flex', gap: '6px' }}>
                        <button onClick={() => setEditTarget(u)} style={editLinkStyle}>Edit</button>
                        <button onClick={() => { setDeleteTarget(u); setDeleteError(null); }} style={delBtnStyle}>Del</button>
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

function AppUserForm({ initial, onSave, onCancel, saving }) {
  const [contactName, setContactName] = useState(initial?.contact_name || '');
  const [emailAddress, setEmailAddress] = useState(initial?.email_address || '');
  const [phoneNumber, setPhoneNumber] = useState(initial?.phone_number || '');
  const [role, setRole] = useState(initial?.role || '');
  const [notes, setNotes] = useState(initial?.notes || '');

  function handleSubmit(e) {
    e.preventDefault();
    if (!contactName.trim() || !emailAddress.trim()) return;
    onSave({
      contact_name: contactName.trim(),
      email_address: emailAddress.trim(),
      phone_number: phoneNumber.trim() || null,
      role: role.trim() || null,
      notes: notes.trim() || null
    });
  }

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '13px' }}>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Name <span style={{ color: '#dc2626' }}>*</span></label>
          <input style={inputStyle} value={contactName} onChange={e => setContactName(e.target.value)} placeholder="Jane Doe" required autoFocus />
        </div>
        <div>
          <label style={labelStyle}>Role</label>
          <input style={inputStyle} value={role} onChange={e => setRole(e.target.value)} placeholder="Admin, Manager…" />
        </div>
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
        <div>
          <label style={labelStyle}>Email <span style={{ color: '#dc2626' }}>*</span></label>
          <input style={inputStyle} type="email" value={emailAddress} onChange={e => setEmailAddress(e.target.value)} placeholder="jane@example.com" required />
        </div>
        <div>
          <label style={labelStyle}>Phone</label>
          <input style={inputStyle} value={phoneNumber} onChange={e => setPhoneNumber(e.target.value)} placeholder="+1 555…" />
        </div>
      </div>
      <div>
        <label style={labelStyle}>Notes</label>
        <textarea style={{ ...inputStyle, minHeight: '60px', resize: 'vertical' }} value={notes} onChange={e => setNotes(e.target.value)} />
      </div>
      <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
        <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>
        <button type="submit" disabled={saving || !contactName.trim() || !emailAddress.trim()} style={saveBtn(saving || !contactName.trim() || !emailAddress.trim())}>
          {saving ? 'Saving…' : 'Save'}
        </button>
      </div>
    </form>
  );
}

function Modal({ title, children, small }) {
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
const addBtn = { padding: '10px 22px', background: '#4338ca', color: '#fff', border: 'none', borderRadius: '8px', fontWeight: 700, fontSize: '0.95rem', cursor: 'pointer' };
const editLinkStyle = { padding: '4px 10px', borderRadius: '5px', background: '#eff6ff', color: '#1e40af', fontSize: '0.8rem', fontWeight: 600, border: '1px solid #bfdbfe', cursor: 'pointer' };
const delBtnStyle = { padding: '4px 10px', borderRadius: '5px', background: '#fff', color: '#dc2626', fontSize: '0.8rem', fontWeight: 600, border: '1px solid #fca5a5', cursor: 'pointer' };
const cancelBtn = { padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer' };
const redBtn = { padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' };
function saveBtn(disabled) { return { padding: '8px 22px', borderRadius: '7px', border: 'none', background: disabled ? '#e5e7eb' : '#4338ca', color: disabled ? '#9ca3af' : '#fff', fontSize: '0.9rem', fontWeight: 600, cursor: disabled ? 'not-allowed' : 'pointer' }; }
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };
const errBox = { padding: '10px 14px', background: '#fef2f2', border: '1px solid #fca5a5', borderRadius: '7px', color: '#991b1b', marginBottom: '16px', fontSize: '0.88rem' };
