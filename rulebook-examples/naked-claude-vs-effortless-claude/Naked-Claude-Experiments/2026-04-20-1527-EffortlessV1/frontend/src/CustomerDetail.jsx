import React, { useEffect, useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { fetchCustomer, updateCustomer, deleteCustomer } from './api';
import { colorToHex } from './colorUtils';
import CustomerModal from './CustomerModal';

export default function CustomerDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [customer, setCustomer] = useState(null);
  const [loading, setLoading] = useState(true);
  const [showEdit, setShowEdit] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState('');

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const c = await fetchCustomer(id);
      setCustomer(c);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => { load(); }, [load]);

  async function handleEdit(data) {
    await updateCustomer(id, data);
    setShowEdit(false);
    await load();
  }

  async function handleDelete() {
    if (!window.confirm(`Delete "${customer.name}"? This cannot be undone.`)) return;
    setDeleting(true);
    try {
      await deleteCustomer(id);
      navigate('/');
    } catch (err) {
      setError(err.message);
      setDeleting(false);
    }
  }

  if (loading) return <div className="container"><div className="loading">Loading…</div></div>;
  if (error) return <div className="container"><div className="error-msg">{error}</div></div>;
  if (!customer) return <div className="container"><div className="error-msg">Customer not found.</div></div>;

  return (
    <div className="container">
      <div className="back-link" onClick={() => navigate('/')}>← Back to list</div>
      <div className="detail-card">
        <div className="detail-header">
          <div>
            <div className="detail-title">{customer.name}</div>
            {customer.is_stopped && <div className="detail-stopped">⏸ Stopped</div>}
          </div>
        </div>

        <div className="detail-field">
          <label>Customer ID</label>
          <div className="value" style={{ fontFamily: 'monospace', color: '#555' }}>{customer.customer_id}</div>
        </div>

        <div className="detail-field">
          <label>Current Color</label>
          <div className="value">
            <div style={{ width: 16, height: 16, borderRadius: '50%', background: colorToHex(customer.current_color) }} />
            {customer.current_color || '—'}
          </div>
        </div>

        <div className="detail-field">
          <label>Stopped?</label>
          <div className="value">
            {customer.is_stopped
              ? <span style={{ color: '#15803d', fontWeight: 600 }}>Yes — color is Green</span>
              : <span style={{ color: '#6b7280' }}>No</span>}
          </div>
        </div>

        <div className="detail-field">
          <label>Notes</label>
          <div className="value" style={{ display: 'block', whiteSpace: 'pre-wrap', color: customer.notes ? '#1a1a1a' : '#9ca3af' }}>
            {customer.notes || 'No notes.'}
          </div>
        </div>

        <div className="detail-actions">
          <button className="btn btn-primary" onClick={() => setShowEdit(true)}>Edit</button>
          <button className="btn btn-danger" onClick={handleDelete} disabled={deleting}>
            {deleting ? 'Deleting…' : 'Delete'}
          </button>
        </div>
      </div>

      {showEdit && (
        <CustomerModal
          initial={customer}
          onSave={handleEdit}
          onClose={() => setShowEdit(false)}
        />
      )}
    </div>
  );
}
