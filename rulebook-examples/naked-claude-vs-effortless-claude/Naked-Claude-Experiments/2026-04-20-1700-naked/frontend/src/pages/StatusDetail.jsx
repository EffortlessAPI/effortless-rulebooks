import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchStatus, deleteStatus } from '../api.js';
import StatusBadge from '../components/StatusBadge.jsx';

export default function StatusDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError]     = useState('');

  useEffect(() => {
    fetchStatus(id)
      .then(setStatus)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [id]);

  const handleDelete = async () => {
    if ((status.clients || []).length > 0) {
      alert(
        `Cannot delete "${status.name}" — ${status.clients.length} client(s) are assigned to it. ` +
        `Reassign them first.`
      );
      return;
    }
    if (!window.confirm(`Delete status "${status.name}"?`)) return;
    try {
      await deleteStatus(id);
      navigate('/statuses');
    } catch (err) {
      alert(err.message);
    }
  };

  if (loading) return <p>Loading…</p>;
  if (error)   return <p className="error-msg">{error}</p>;
  if (!status) return null;

  return (
    <div className="page-card">
      <Link className="back-link" to="/statuses">← Back to statuses</Link>

      <h2>
        <StatusBadge name={status.name} isBlocking={status.is_blocking} />
        {status.is_blocking && <span className="blocking-badge" style={{ marginLeft: 10 }}>Blocking</span>}
      </h2>

      <div className="detail-field">
        <div className="field-label">Description</div>
        <div className="field-value">{status.description || <em style={{ color: '#aaa' }}>No description</em>}</div>
      </div>

      <div className="detail-field">
        <div className="field-label">Blocking?</div>
        <div className="field-value" style={{ fontWeight: 600, color: status.is_blocking ? '#c0392b' : '#27ae60' }}>
          {status.is_blocking
            ? 'Yes — clients in this status are stopped'
            : 'No — clients in this status are progressing normally'}
        </div>
      </div>

      <div className="detail-field">
        <div className="field-label">Slug</div>
        <div className="field-value" style={{ fontFamily: 'monospace', color: '#888' }}>{status.slug}</div>
      </div>

      <div className="detail-field">
        <div className="field-label">
          Clients in this status ({(status.clients || []).length})
        </div>
        <div className="field-value">
          {(status.clients || []).length === 0 ? (
            <em style={{ color: '#aaa' }}>No clients assigned</em>
          ) : (
            <ul className="status-customer-list">
              {status.clients.map((c) => (
                <li key={c.id}>
                  <Link to={`/clients/${c.id}`} className="back-link" style={{ fontSize: 14 }}>
                    {c.name}
                  </Link>
                  {c.notes && <span style={{ color: '#888', marginLeft: 8, fontSize: 13 }}>— {c.notes}</span>}
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>

      <div className="form-actions">
        <button className="btn-primary" onClick={() => navigate(`/statuses/${id}/edit`)}>
          Edit
        </button>
        <button className="btn-danger" onClick={handleDelete}>
          Delete
        </button>
        <button className="btn-secondary" onClick={() => navigate('/statuses')}>
          Cancel
        </button>
      </div>
    </div>
  );
}
