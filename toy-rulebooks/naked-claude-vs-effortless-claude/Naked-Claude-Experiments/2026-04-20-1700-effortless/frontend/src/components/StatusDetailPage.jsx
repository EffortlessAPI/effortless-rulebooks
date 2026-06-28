import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { getStatus, updateStatus, deleteStatus } from './api';
import StatusForm from './StatusForm';

export default function StatusDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleteConfirm, setDeleteConfirm] = useState(false);
  const [deleteError, setDeleteError] = useState(null);

  async function load() {
    try {
      setLoading(true);
      const data = await getStatus(id);
      setStatus(data);
      setError(null);
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { load(); }, [id]);

  async function handleSave(formData) {
    setSaving(true);
    try {
      await updateStatus(id, formData);
      await load();
      setEditing(false);
    } catch (e) {
      alert(e.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    setDeleteError(null);
    try {
      await deleteStatus(id);
      navigate('/statuses');
    } catch (e) {
      setDeleteError(e.message);
    }
  }

  if (loading) return <div style={page}><p style={{ color: '#6b7280' }}>Loading…</p></div>;
  if (error)   return <div style={page}><p style={{ color: '#dc2626' }}>Error: {error}</p></div>;
  if (!status) return null;

  const clientList = status.client_list || [];

  return (
    <div style={page}>
      <div style={{ maxWidth: '640px', margin: '0 auto' }}>
        <Link to="/statuses" style={{ color: '#4f46e5', fontWeight: 600, fontSize: '0.9rem', display: 'inline-flex', alignItems: 'center', gap: '4px', marginBottom: '20px', textDecoration: 'none' }}>
          ← Back to statuses
        </Link>

        <div style={{
          background: '#fff',
          borderRadius: '14px',
          padding: '28px',
          boxShadow: '0 1px 6px rgba(0,0,0,0.08)',
          borderLeft: status.is_blocking ? '4px solid #dc2626' : '4px solid #16a34a',
          marginBottom: '20px'
        }}>
          {!editing ? (
            <>
              <div style={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', marginBottom: '16px' }}>
                <div>
                  <h1 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: '8px' }}>{status.display_name}</h1>
                  <div style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
                    <code style={{ fontSize: '0.8rem', color: '#9ca3af', background: '#f3f4f6', padding: '2px 8px', borderRadius: '5px' }}>{status.statuse_id}</code>
                    {status.is_blocking
                      ? <span style={blockBadge}>🔴 Blocking</span>
                      : <span style={okBadge}>✓ Non-blocking</span>}
                  </div>
                </div>
                <div style={{ display: 'flex', gap: '8px' }}>
                  <button onClick={() => setEditing(true)} style={editBtn}>Edit</button>
                  <button onClick={() => { setDeleteConfirm(true); setDeleteError(null); }} style={deleteBtn}>Delete</button>
                </div>
              </div>

              <div style={{ borderTop: '1px solid #f3f4f6', paddingTop: '14px' }}>
                <label style={fieldLabel}>Description</label>
                <p style={{ color: status.description ? '#374151' : '#d1d5db', fontSize: '0.93rem', lineHeight: '1.55' }}>
                  {status.description || 'No description.'}
                </p>
              </div>

              {status.is_blocking && (
                <div style={{ marginTop: '14px', padding: '10px 14px', background: '#fef2f2', border: '1px solid #fca5a5', borderRadius: '8px', fontSize: '0.85rem', color: '#991b1b' }}>
                  ⚠️ Clients assigned to this status will be marked as <strong>Stopped</strong>.
                  Changing this flag will immediately affect all clients in this status.
                </div>
              )}
            </>
          ) : (
            <>
              <h2 style={{ marginBottom: '18px', fontSize: '1.15rem', fontWeight: 700 }}>Edit Status</h2>
              <StatusForm
                initial={status}
                onSave={handleSave}
                onCancel={() => setEditing(false)}
                saving={saving}
              />
            </>
          )}
        </div>

        {/* Clients assigned to this status */}
        <div style={{ background: '#fff', borderRadius: '14px', padding: '22px 28px', boxShadow: '0 1px 6px rgba(0,0,0,0.06)' }}>
          <h2 style={{ fontSize: '1rem', fontWeight: 700, marginBottom: '14px', color: '#374151' }}>
            Clients in this status
            <span style={{ marginLeft: '8px', background: '#dbeafe', color: '#1e40af', padding: '2px 9px', borderRadius: '10px', fontSize: '0.8rem', fontWeight: 700 }}>
              {clientList.length}
            </span>
          </h2>
          {clientList.length === 0 ? (
            <p style={{ color: '#9ca3af', fontSize: '0.9rem' }}>No clients are currently assigned to this status.</p>
          ) : (
            <ul style={{ listStyle: 'none', padding: 0, margin: 0, display: 'flex', flexDirection: 'column', gap: '8px' }}>
              {clientList.map(c => (
                <li key={c.client_id} style={{
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'space-between',
                  padding: '10px 14px',
                  borderRadius: '8px',
                  background: c.is_stopped ? '#fef2f2' : '#f9fafb',
                  border: `1px solid ${c.is_stopped ? '#fca5a5' : '#e5e7eb'}`
                }}>
                  <div>
                    <Link
                      to={`/clients/${encodeURIComponent(c.client_id)}`}
                      style={{ fontWeight: 600, color: '#1a1a1a', textDecoration: 'none', fontSize: '0.92rem' }}
                    >
                      {c.name}
                    </Link>
                    {c.notes && <div style={{ fontSize: '0.8rem', color: '#6b7280', marginTop: '2px' }}>{c.notes}</div>}
                  </div>
                  {c.is_stopped && <span style={stoppedBadge}>🚫 Stopped</span>}
                </li>
              ))}
            </ul>
          )}
        </div>

        {deleteConfirm && (
          <div style={modalOverlay}>
            <div style={modalBox}>
              <h2 style={{ marginBottom: '12px', fontSize: '1.1rem', fontWeight: 700 }}>Delete Status?</h2>
              {deleteError ? (
                <div style={{ padding: '10px 14px', background: '#fef2f2', border: '1px solid #fca5a5', borderRadius: '7px', color: '#991b1b', marginBottom: '16px', fontSize: '0.88rem' }}>
                  ⚠️ {deleteError}
                </div>
              ) : (
                <p style={{ color: '#374151', marginBottom: '20px' }}>
                  Are you sure you want to delete <strong>{status.display_name}</strong>?
                  {clientList.length > 0 && (
                    <span style={{ display: 'block', marginTop: '8px', color: '#dc2626', fontWeight: 600 }}>
                      ⚠️ {clientList.length} client(s) are still assigned to this status. Reassign them first.
                    </span>
                  )}
                </p>
              )}
              <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
                <button onClick={() => { setDeleteConfirm(false); setDeleteError(null); }} style={cancelBtn}>
                  {deleteError ? 'Close' : 'Cancel'}
                </button>
                {!deleteError && clientList.length === 0 && (
                  <button onClick={handleDelete} style={{ padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' }}>
                    Delete
                  </button>
                )}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

const page = { maxWidth: '900px', margin: '0 auto', padding: '28px 16px' };
const fieldLabel = { display: 'block', fontSize: '0.75rem', fontWeight: 700, color: '#9ca3af', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: '5px' };
const blockBadge = { display: 'inline-block', padding: '3px 10px', borderRadius: '12px', background: '#fef2f2', color: '#991b1b', fontWeight: 700, fontSize: '0.82rem', border: '1px solid #fca5a5' };
const okBadge = { display: 'inline-block', padding: '3px 10px', borderRadius: '12px', background: '#f0fdf4', color: '#166534', fontWeight: 600, fontSize: '0.82rem', border: '1px solid #86efac' };
const stoppedBadge = { display: 'inline-block', padding: '3px 10px', borderRadius: '12px', background: '#fef2f2', color: '#991b1b', fontWeight: 700, fontSize: '0.82rem', border: '1px solid #fca5a5' };
const editBtn = { padding: '7px 16px', borderRadius: '7px', border: '1px solid #c7d2fe', background: '#eff6ff', color: '#1e40af', fontWeight: 600, fontSize: '0.88rem', cursor: 'pointer' };
const deleteBtn = { padding: '7px 14px', borderRadius: '7px', border: '1px solid #fca5a5', background: '#fff', color: '#dc2626', fontWeight: 600, fontSize: '0.88rem', cursor: 'pointer' };
const cancelBtn = { padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer' };
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', maxWidth: '420px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };
