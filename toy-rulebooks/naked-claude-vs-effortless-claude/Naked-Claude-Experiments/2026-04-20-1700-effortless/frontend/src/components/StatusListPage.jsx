import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getStatuses, createStatus, deleteStatus } from './api';
import StatusForm from './StatusForm';

export default function StatusListPage() {
  const [statuses, setStatuses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showAdd, setShowAdd] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [deleteError, setDeleteError] = useState(null);

  async function load() {
    try {
      setLoading(true);
      const data = await getStatuses();
      setStatuses(data);
      setError(null);
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { load(); }, []);

  async function handleAdd(formData) {
    setSaving(true);
    try {
      await createStatus(formData);
      setShowAdd(false);
      await load();
    } catch (e) {
      alert(e.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(id) {
    setDeleteError(null);
    try {
      await deleteStatus(id);
      setDeleteTarget(null);
      await load();
    } catch (e) {
      setDeleteError(e.message);
    }
  }

  return (
    <div style={{ maxWidth: '900px', margin: '0 auto', padding: '28px 16px' }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '22px' }}>
        <div>
          <h1 style={{ fontSize: '1.55rem', fontWeight: 700, color: '#1a1a1a' }}>Statuses</h1>
          <p style={{ color: '#6b7280', fontSize: '0.88rem', marginTop: '2px' }}>
            Manage statuses and their blocking flag
          </p>
        </div>
        <button
          onClick={() => setShowAdd(true)}
          style={{
            padding: '10px 22px',
            background: '#4f46e5',
            color: '#fff',
            border: 'none',
            borderRadius: '8px',
            fontWeight: 700,
            fontSize: '0.95rem',
            cursor: 'pointer',
            boxShadow: '0 1px 4px rgba(79,70,229,0.3)'
          }}
        >
          + Add Status
        </button>
      </div>

      {loading && <p style={{ color: '#6b7280' }}>Loading…</p>}
      {error && <p style={{ color: '#dc2626' }}>Error: {error}</p>}

      {showAdd && (
        <Modal title="Add Status" onClose={() => setShowAdd(false)}>
          <StatusForm onSave={handleAdd} onCancel={() => setShowAdd(false)} saving={saving} />
        </Modal>
      )}

      {deleteTarget && (
        <Modal title="Delete Status?" onClose={() => { setDeleteTarget(null); setDeleteError(null); }} small>
          {deleteError ? (
            <div style={{ padding: '10px 14px', background: '#fef2f2', border: '1px solid #fca5a5', borderRadius: '7px', color: '#991b1b', marginBottom: '16px', fontSize: '0.88rem' }}>
              ⚠️ {deleteError}
            </div>
          ) : (
            <p style={{ color: '#374151', marginBottom: '20px' }}>
              Are you sure you want to delete <strong>{deleteTarget.display_name}</strong>?
              {deleteTarget.client_count > 0 && (
                <span style={{ display: 'block', marginTop: '8px', color: '#dc2626', fontWeight: 600 }}>
                  ⚠️ {deleteTarget.client_count} client(s) are assigned to this status.
                </span>
              )}
            </p>
          )}
          {!deleteError && (
            <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
              <button onClick={() => { setDeleteTarget(null); setDeleteError(null); }} style={cancelBtn}>Cancel</button>
              {deleteTarget.client_count === 0 && (
                <button onClick={() => handleDelete(deleteTarget.statuse_id)} style={redBtn}>Delete</button>
              )}
            </div>
          )}
          {deleteError && (
            <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
              <button onClick={() => { setDeleteTarget(null); setDeleteError(null); }} style={cancelBtn}>Close</button>
            </div>
          )}
        </Modal>
      )}

      {!loading && !error && (
        <div style={{ background: '#fff', borderRadius: '12px', boxShadow: '0 1px 4px rgba(0,0,0,0.07)', overflow: 'hidden' }}>
          {statuses.length === 0 ? (
            <p style={{ padding: '32px', textAlign: 'center', color: '#6b7280' }}>No statuses yet. Add one above.</p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>Name</th>
                  <th style={th}>Description</th>
                  <th style={{ ...th, width: '100px' }}>Blocking</th>
                  <th style={{ ...th, width: '80px' }}>Clients</th>
                  <th style={{ ...th, width: '100px' }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {statuses.map(s => (
                  <tr key={s.statuse_id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={td}>
                      <Link
                        to={`/statuses/${encodeURIComponent(s.statuse_id)}`}
                        style={{ fontWeight: 700, color: '#1a1a1a', textDecoration: 'none' }}
                      >
                        {s.display_name}
                      </Link>
                      <div style={{ fontSize: '0.75rem', color: '#9ca3af', fontFamily: 'monospace', marginTop: '1px' }}>
                        {s.statuse_id}
                      </div>
                    </td>
                    <td style={{ ...td, color: '#6b7280', fontSize: '0.87rem', maxWidth: '280px' }}>
                      {s.description || <span style={{ color: '#d1d5db' }}>—</span>}
                    </td>
                    <td style={td}>
                      {s.is_blocking
                        ? <span style={{ ...badge, background: '#fef2f2', color: '#991b1b', borderColor: '#fca5a5' }}>🔴 Yes</span>
                        : <span style={{ ...badge, background: '#f0fdf4', color: '#166534', borderColor: '#86efac' }}>✓ No</span>
                      }
                    </td>
                    <td style={{ ...td, textAlign: 'center' }}>
                      <span style={{
                        fontWeight: 700,
                        color: s.client_count > 0 ? '#1e40af' : '#9ca3af',
                        fontSize: '0.95rem'
                      }}>
                        {s.client_count}
                      </span>
                    </td>
                    <td style={td}>
                      <div style={{ display: 'flex', gap: '6px' }}>
                        <Link
                          to={`/statuses/${encodeURIComponent(s.statuse_id)}`}
                          style={{ padding: '4px 10px', borderRadius: '5px', background: '#eff6ff', color: '#1e40af', fontSize: '0.8rem', fontWeight: 600, border: '1px solid #bfdbfe', textDecoration: 'none' }}
                        >
                          Edit
                        </Link>
                        <button
                          onClick={() => { setDeleteTarget(s); setDeleteError(null); }}
                          style={{ padding: '4px 10px', borderRadius: '5px', background: '#fff', color: '#dc2626', fontSize: '0.8rem', fontWeight: 600, border: '1px solid #fca5a5', cursor: 'pointer' }}
                        >
                          Del
                        </button>
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

function Modal({ title, children, onClose, small }) {
  return (
    <div style={modalOverlay}>
      <div style={{ ...modalBox, maxWidth: small ? '420px' : '500px' }}>
        <h2 style={{ marginBottom: '18px', fontSize: '1.1rem', fontWeight: 700 }}>{title}</h2>
        {children}
      </div>
    </div>
  );
}

const th = { padding: '11px 16px', textAlign: 'left', fontSize: '0.78rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em' };
const td = { padding: '12px 16px', fontSize: '0.92rem', verticalAlign: 'middle' };
const badge = { display: 'inline-block', padding: '3px 9px', borderRadius: '10px', fontWeight: 700, fontSize: '0.8rem', border: '1px solid' };
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };
const cancelBtn = { padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer' };
const redBtn = { padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' };
