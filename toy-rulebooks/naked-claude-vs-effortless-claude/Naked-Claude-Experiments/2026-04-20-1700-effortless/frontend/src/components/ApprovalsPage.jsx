import { useState, useEffect } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { getApprovals, createApproval, updateApproval, deleteApproval, getClients, getAppUsers } from './api';

const inputStyle = { width: '100%', padding: '8px 12px', borderRadius: '7px', border: '1px solid #d1d5db', fontSize: '0.95rem', outline: 'none', boxSizing: 'border-box' };
const labelStyle = { display: 'block', fontSize: '0.85rem', fontWeight: 600, marginBottom: '5px', color: '#374151' };

export default function ApprovalsPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const filter = searchParams.get('status') || 'all'; // all | pending | approved
  const userFilter = searchParams.get('user');

  const [approvals, setApprovals] = useState([]);
  const [clients, setClients] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showAdd, setShowAdd] = useState(false);
  const [editTarget, setEditTarget] = useState(null);
  const [saving, setSaving] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);

  async function load() {
    try {
      setLoading(true);
      const status = filter !== 'all' ? filter : undefined;
      const [a, c, u] = await Promise.all([
        getApprovals({ status }),
        getClients(),
        getAppUsers()
      ]);
      const rows = userFilter ? a.filter(r => r.approved_by === userFilter) : a;
      setApprovals(rows);
      setClients(c);
      setUsers(u);
      setError(null);
    } catch (e) { setError(e.message); } finally { setLoading(false); }
  }
  useEffect(() => { load(); }, [filter, userFilter]);

  async function handleAdd(data) {
    setSaving(true);
    try { await createApproval(data); setShowAdd(false); await load(); }
    catch (e) { alert(e.message); } finally { setSaving(false); }
  }

  async function handleEdit(data) {
    setSaving(true);
    try { await updateApproval(editTarget.client_approval_id, data); setEditTarget(null); await load(); }
    catch (e) { alert(e.message); } finally { setSaving(false); }
  }

  async function handleDelete(id) {
    try { await deleteApproval(id); setDeleteTarget(null); await load(); }
    catch (e) { alert(e.message); }
  }

  function setFilter(f) {
    const next = new URLSearchParams(searchParams);
    if (f === 'all') next.delete('status'); else next.set('status', f);
    setSearchParams(next);
  }

  const activeUser = userFilter ? users.find(u => u.app_user_id === userFilter) : null;

  return (
    <div style={{ maxWidth: '1100px', margin: '0 auto', padding: '28px 16px' }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '18px' }}>
        <div>
          <h1 style={{ fontSize: '1.55rem', fontWeight: 700, color: '#1a1a1a' }}>Client Approvals</h1>
          <p style={{ color: '#6b7280', fontSize: '0.88rem', marginTop: '2px' }}>
            Approval queue — assign, review, and close out.
            {activeUser && <span> Filtered to <strong>{activeUser.contact_name}</strong>. <Link to="/approvals" style={{ color: '#4f46e5', marginLeft: '6px' }}>Clear</Link></span>}
          </p>
        </div>
        <button onClick={() => setShowAdd(true)} style={addBtn}>+ New Approval</button>
      </div>

      {/* Filter tabs */}
      <div style={{ display: 'flex', gap: '6px', marginBottom: '16px' }}>
        {[['all', 'All'], ['pending', 'Pending'], ['approved', 'Approved']].map(([k, label]) => (
          <button key={k} onClick={() => setFilter(k)} style={{
            padding: '6px 14px', borderRadius: '7px',
            border: filter === k ? '1px solid #4f46e5' : '1px solid #d1d5db',
            background: filter === k ? '#eef2ff' : '#fff',
            color: filter === k ? '#4338ca' : '#374151',
            fontWeight: 600, fontSize: '0.85rem', cursor: 'pointer'
          }}>{label}</button>
        ))}
      </div>

      {loading && <p style={{ color: '#6b7280' }}>Loading…</p>}
      {error && <p style={{ color: '#dc2626' }}>Error: {error}</p>}

      {(showAdd || editTarget) && (
        <Modal title={editTarget ? 'Edit Approval' : 'New Approval'}>
          <ApprovalForm
            initial={editTarget}
            clients={clients}
            users={users}
            onSave={editTarget ? handleEdit : handleAdd}
            onCancel={() => { setShowAdd(false); setEditTarget(null); }}
            saving={saving}
          />
        </Modal>
      )}

      {deleteTarget && (
        <Modal title="Delete Approval?" small>
          <p style={{ color: '#374151', marginBottom: '20px' }}>
            Delete approval <strong>{deleteTarget.name}</strong>? This cannot be undone.
          </p>
          <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
            <button onClick={() => setDeleteTarget(null)} style={cancelBtn}>Cancel</button>
            <button onClick={() => handleDelete(deleteTarget.client_approval_id)} style={redBtn}>Delete</button>
          </div>
        </Modal>
      )}

      {!loading && !error && (
        <div style={{ background: '#fff', borderRadius: '12px', boxShadow: '0 1px 4px rgba(0,0,0,0.07)', overflow: 'hidden' }}>
          {approvals.length === 0 ? (
            <p style={{ padding: '32px', textAlign: 'center', color: '#6b7280' }}>
              No {filter !== 'all' ? filter : ''} approvals.
            </p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>Client</th>
                  <th style={th}>Client Email</th>
                  <th style={th}>Approved By</th>
                  <th style={th}>Status</th>
                  <th style={th}>Notes</th>
                  <th style={{ ...th, width: '100px' }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {approvals.map(a => (
                  <tr key={a.client_approval_id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={td}>
                      <Link to={`/clients/${encodeURIComponent(a.client)}`} style={{ fontWeight: 600, color: '#4f46e5', textDecoration: 'none' }}>
                        {a.client_name || a.client}
                      </Link>
                      {a.client_category && (
                        <div style={{ fontSize: '0.75rem', color: '#6b7280', marginTop: '2px' }}>{a.client_category}</div>
                      )}
                    </td>
                    <td style={{ ...td, fontSize: '0.85rem', color: '#6b7280' }}>{a.client_email || '—'}</td>
                    <td style={td}>
                      {a.approved_by ? (
                        <Link to={`/app-users`} style={{ fontWeight: 600, color: '#4338ca', textDecoration: 'none' }}>
                          {a.approved_by_contact_name || a.approved_by}
                          {a.approved_by_role && <span style={{ fontSize: '0.75rem', color: '#9ca3af', marginLeft: '6px' }}>({a.approved_by_role})</span>}
                        </Link>
                      ) : <span style={{ color: '#9ca3af', fontStyle: 'italic' }}>Unassigned</span>}
                    </td>
                    <td style={td}>
                      {a.is_approved ? (
                        <span style={apvdBadge}>✓ APPROVED</span>
                      ) : (
                        <span style={pendBadge}>⏳ PENDING</span>
                      )}
                    </td>
                    <td style={{ ...td, fontSize: '0.85rem', color: '#6b7280', maxWidth: '200px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{a.notes || '—'}</td>
                    <td style={td}>
                      <div style={{ display: 'flex', gap: '6px' }}>
                        <button onClick={() => setEditTarget(a)} style={editLinkStyle}>Edit</button>
                        <button onClick={() => setDeleteTarget(a)} style={delBtnStyle}>Del</button>
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

export function ApprovalForm({ initial, clients, users, onSave, onCancel, saving, lockedClient }) {
  const [client, setClient] = useState(initial?.client || lockedClient || '');
  const [approvedBy, setApprovedBy] = useState(initial?.approved_by || '');
  const [notes, setNotes] = useState(initial?.notes || '');

  function handleSubmit(e) {
    e.preventDefault();
    if (!client) return;
    onSave({
      client,
      approved_by: approvedBy || null,
      notes: notes.trim() || null
    });
  }

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '13px' }}>
      <div>
        <label style={labelStyle}>Client <span style={{ color: '#dc2626' }}>*</span></label>
        <select style={{ ...inputStyle, background: lockedClient || initial ? '#f3f4f6' : '#fff' }} value={client} onChange={e => setClient(e.target.value)} disabled={!!lockedClient || !!initial} required>
          <option value="">— Select client —</option>
          {clients.map(c => <option key={c.client_id} value={c.client_id}>{c.name}{c.company_name ? ` (${c.company_name})` : ''}</option>)}
        </select>
      </div>
      <div>
        <label style={labelStyle}>Approved By <span style={{ color: '#9ca3af', fontWeight: 400 }}>(blank = pending)</span></label>
        <select style={{ ...inputStyle, background: '#fff' }} value={approvedBy} onChange={e => setApprovedBy(e.target.value)}>
          <option value="">— Leave pending —</option>
          {users.map(u => <option key={u.app_user_id} value={u.app_user_id}>{u.contact_name} {u.role ? `— ${u.role}` : ''}</option>)}
        </select>
      </div>
      <div>
        <label style={labelStyle}>Notes</label>
        <textarea style={{ ...inputStyle, minHeight: '60px', resize: 'vertical' }} value={notes} onChange={e => setNotes(e.target.value)} placeholder="Context, sign-off conditions…" />
      </div>
      <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
        <button type="button" onClick={onCancel} style={cancelBtn}>Cancel</button>
        <button type="submit" disabled={saving || !client} style={saveBtn(saving || !client)}>
          {saving ? 'Saving…' : 'Save'}
        </button>
      </div>
    </form>
  );
}

function Modal({ title, children, small }) {
  return (
    <div style={modalOverlay}>
      <div style={{ ...modalBox, maxWidth: small ? '420px' : '520px' }}>
        <h2 style={{ marginBottom: '18px', fontSize: '1.1rem', fontWeight: 700 }}>{title}</h2>
        {children}
      </div>
    </div>
  );
}

const th = { padding: '11px 14px', textAlign: 'left', fontSize: '0.77rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em' };
const td = { padding: '12px 14px', fontSize: '0.92rem', verticalAlign: 'middle' };
const addBtn = { padding: '10px 22px', background: '#059669', color: '#fff', border: 'none', borderRadius: '8px', fontWeight: 700, fontSize: '0.95rem', cursor: 'pointer' };
const editLinkStyle = { padding: '4px 10px', borderRadius: '5px', background: '#eff6ff', color: '#1e40af', fontSize: '0.8rem', fontWeight: 600, border: '1px solid #bfdbfe', cursor: 'pointer' };
const delBtnStyle = { padding: '4px 10px', borderRadius: '5px', background: '#fff', color: '#dc2626', fontSize: '0.8rem', fontWeight: 600, border: '1px solid #fca5a5', cursor: 'pointer' };
const cancelBtn = { padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer' };
const redBtn = { padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' };
function saveBtn(disabled) { return { padding: '8px 22px', borderRadius: '7px', border: 'none', background: disabled ? '#e5e7eb' : '#059669', color: disabled ? '#9ca3af' : '#fff', fontSize: '0.9rem', fontWeight: 600, cursor: disabled ? 'not-allowed' : 'pointer' }; }
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };
const apvdBadge = { display: 'inline-block', fontSize: '0.78rem', padding: '3px 10px', borderRadius: '10px', background: '#f0fdf4', color: '#166534', border: '1px solid #86efac', fontWeight: 700, letterSpacing: '0.04em' };
const pendBadge = { display: 'inline-block', fontSize: '0.78rem', padding: '3px 10px', borderRadius: '10px', background: '#fffbeb', color: '#b45309', border: '1px solid #fcd34d', fontWeight: 700, letterSpacing: '0.04em' };
