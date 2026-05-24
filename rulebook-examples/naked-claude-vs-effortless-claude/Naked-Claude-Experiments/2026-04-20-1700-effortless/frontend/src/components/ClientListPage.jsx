import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getClients, createClient, deleteClient } from './api';
import Summary from './Summary';
import ClientForm from './ClientForm';

export default function ClientListPage() {
  const [clients, setClients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showAdd, setShowAdd] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleteConfirm, setDeleteConfirm] = useState(null);

  async function load() {
    try {
      setLoading(true);
      const data = await getClients();
      setClients(data);
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
      await createClient(formData);
      setShowAdd(false);
      await load();
      if (Summary.refresh) Summary.refresh();
    } catch (e) {
      alert(e.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(id) {
    try {
      await deleteClient(id);
      setDeleteConfirm(null);
      await load();
      if (Summary.refresh) Summary.refresh();
    } catch (e) {
      alert(e.message);
    }
  }

  return (
    <div style={{ maxWidth: '900px', margin: '0 auto', padding: '28px 16px' }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '22px' }}>
        <div>
          <h1 style={{ fontSize: '1.55rem', fontWeight: 700, color: '#1a1a1a' }}>Clients</h1>
          <p style={{ color: '#6b7280', fontSize: '0.88rem', marginTop: '2px' }}>
            Manage clients and track who's stopped
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
          + Add Client
        </button>
      </div>

      {loading && <p style={{ color: '#6b7280' }}>Loading…</p>}
      {error && <p style={{ color: '#dc2626' }}>Error: {error}</p>}

      {!loading && !error && <Summary />}

      {showAdd && (
        <Modal title="Add Client" onClose={() => setShowAdd(false)}>
          <ClientForm onSave={handleAdd} onCancel={() => setShowAdd(false)} saving={saving} />
        </Modal>
      )}

      {deleteConfirm && (
        <Modal title="Delete Client?" onClose={() => setDeleteConfirm(null)} small>
          <p style={{ color: '#374151', marginBottom: '20px' }}>
            Are you sure you want to delete <strong>{deleteConfirm.name}</strong>? This cannot be undone.
          </p>
          <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
            <button onClick={() => setDeleteConfirm(null)} style={cancelBtn}>Cancel</button>
            <button
              onClick={() => handleDelete(deleteConfirm.client_id)}
              style={redBtn}
            >
              Delete
            </button>
          </div>
        </Modal>
      )}

      {!loading && !error && (
        <div style={{ background: '#fff', borderRadius: '12px', boxShadow: '0 1px 4px rgba(0,0,0,0.07)', overflow: 'hidden' }}>
          {clients.length === 0 ? (
            <p style={{ padding: '32px', textAlign: 'center', color: '#6b7280' }}>No clients yet. Add one above.</p>
          ) : (
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                  <th style={th}>Name / Company</th>
                  <th style={th}>Contact</th>
                  <th style={th}>Category</th>
                  <th style={th}>Status</th>
                  <th style={th}>State</th>
                  <th style={{ ...th, minWidth: '140px' }}>Notes</th>
                  <th style={{ ...th, width: '90px' }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {clients.map(c => (
                  <tr
                    key={c.client_id}
                    style={{
                      borderBottom: '1px solid #e5e7eb',
                      background: c.is_stopped ? '#fef2f2' : '#fff',
                      transition: 'background 0.1s'
                    }}
                  >
                    <td style={td}>
                      <Link
                        to={`/clients/${encodeURIComponent(c.client_id)}`}
                        style={{ fontWeight: 600, color: '#1a1a1a', textDecoration: 'none' }}
                      >
                        {c.name}
                      </Link>
                      {c.company_name && (
                        <div style={{ fontSize: '0.78rem', color: '#6b7280', marginTop: '2px' }}>{c.company_name}</div>
                      )}
                    </td>
                    <td style={{ ...td, fontSize: '0.85rem' }}>
                      {c.email && <div style={{ color: '#374151' }}>{c.email}</div>}
                      {c.phone && <div style={{ color: '#6b7280' }}>{c.phone}</div>}
                      {!c.email && !c.phone && <span style={{ color: '#d1d5db' }}>—</span>}
                    </td>
                    <td style={td}>
                      {c.category_name ? (
                        <span style={{
                          display: 'inline-block',
                          padding: '2px 10px',
                          borderRadius: '10px',
                          background: parseFloat(c.category_discount) > 0 ? '#f0fdf4' : '#eff6ff',
                          color: parseFloat(c.category_discount) > 0 ? '#166534' : '#1e40af',
                          fontWeight: 600,
                          fontSize: '0.82rem',
                          border: `1px solid ${parseFloat(c.category_discount) > 0 ? '#86efac' : '#bfdbfe'}`
                        }}>
                          {c.category_name}
                          {parseFloat(c.category_discount) > 0 && (
                            <span style={{ marginLeft: '6px', opacity: 0.8, fontWeight: 700 }}>
                              −{(parseFloat(c.category_discount) * 100).toFixed(0)}%
                            </span>
                          )}
                        </span>
                      ) : (
                        <span style={{ color: '#d1d5db', fontSize: '0.85rem' }}>—</span>
                      )}
                    </td>
                    <td style={td}>
                      {c.status_display_name ? (
                        <span
                          title={c.status_description || ''}
                          style={{
                            display: 'inline-block',
                            padding: '2px 10px',
                            borderRadius: '10px',
                            background: c.is_stopped ? '#fef2f2' : '#f3f4f6',
                            color: c.is_stopped ? '#991b1b' : '#374151',
                            fontWeight: 600,
                            fontSize: '0.82rem',
                            border: `1px solid ${c.is_stopped ? '#fca5a5' : '#d1d5db'}`,
                            cursor: 'help'
                          }}
                        >
                          {c.status_display_name}
                        </span>
                      ) : (
                        <span style={{ color: '#d1d5db', fontSize: '0.85rem' }}>—</span>
                      )}
                    </td>
                    <td style={td}>
                      {c.is_stopped
                        ? <span style={stoppedBadge}>🚫 Stopped</span>
                        : <span style={okBadge}>✓ OK</span>}
                    </td>
                    <td style={{ ...td, color: '#6b7280', fontSize: '0.87rem', maxWidth: '160px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                      {c.notes || <span style={{ color: '#d1d5db' }}>—</span>}
                    </td>
                    <td style={td}>
                      <div style={{ display: 'flex', gap: '6px' }}>
                        <Link
                          to={`/clients/${encodeURIComponent(c.client_id)}`}
                          style={{ padding: '4px 10px', borderRadius: '5px', background: '#eff6ff', color: '#1e40af', fontSize: '0.8rem', fontWeight: 600, border: '1px solid #bfdbfe', textDecoration: 'none' }}
                        >
                          Edit
                        </Link>
                        <button
                          onClick={() => setDeleteConfirm(c)}
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
      <div style={{ ...modalBox, maxWidth: small ? '380px' : '480px' }}>
        <h2 style={{ marginBottom: '18px', fontSize: '1.1rem', fontWeight: 700 }}>{title}</h2>
        {children}
      </div>
    </div>
  );
}

const th = { padding: '11px 16px', textAlign: 'left', fontSize: '0.78rem', fontWeight: 700, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.04em' };
const td = { padding: '12px 16px', fontSize: '0.92rem', verticalAlign: 'middle' };

const stoppedBadge = {
  display: 'inline-block', padding: '3px 10px', borderRadius: '12px',
  background: '#fef2f2', color: '#991b1b', fontWeight: 700, fontSize: '0.82rem', border: '1px solid #fca5a5'
};
const okBadge = {
  display: 'inline-block', padding: '3px 10px', borderRadius: '12px',
  background: '#f0fdf4', color: '#166534', fontWeight: 600, fontSize: '0.82rem', border: '1px solid #86efac'
};
const modalOverlay = { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.35)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100 };
const modalBox = { background: '#fff', borderRadius: '14px', padding: '28px 28px 22px', width: '100%', boxShadow: '0 8px 32px rgba(0,0,0,0.18)' };
const cancelBtn = { padding: '8px 18px', borderRadius: '7px', border: '1px solid #d1d5db', background: '#fff', color: '#374151', fontSize: '0.9rem', fontWeight: 600, cursor: 'pointer' };
const redBtn = { padding: '8px 20px', borderRadius: '7px', border: 'none', background: '#dc2626', color: '#fff', fontWeight: 700, fontSize: '0.9rem', cursor: 'pointer' };
