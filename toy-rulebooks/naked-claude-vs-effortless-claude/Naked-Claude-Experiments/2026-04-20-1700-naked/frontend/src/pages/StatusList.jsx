import { useState, useEffect, useCallback } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { fetchStatuses, deleteStatus } from '../api.js';
import StatusBadge from '../components/StatusBadge.jsx';

export default function StatusList() {
  const [statuses, setStatuses] = useState([]);
  const [loading, setLoading]   = useState(true);
  const navigate = useNavigate();

  const load = useCallback(async () => {
    setLoading(true);
    try {
      setStatuses(await fetchStatuses());
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const handleDelete = async (status) => {
    if (status.client_count > 0) {
      alert(
        `Cannot delete "${status.name}" — ${status.client_count} client(s) are assigned to it. ` +
        `Reassign them first.`
      );
      return;
    }
    if (!window.confirm(`Delete status "${status.name}"?`)) return;
    try {
      await deleteStatus(status.id);
      load();
    } catch (err) {
      alert(err.message);
    }
  };

  return (
    <>
      <div className="list-header">
        <h2>Statuses {!loading && `(${statuses.length})`}</h2>
        <button className="btn-primary" onClick={() => navigate('/statuses/new')}>
          + Add Status
        </button>
      </div>

      {loading ? (
        <p>Loading statuses…</p>
      ) : statuses.length === 0 ? (
        <p style={{ color: '#888' }}>No statuses yet. Add one!</p>
      ) : (
        <div className="customer-list">
          {statuses.map((s) => (
            <div key={s.id} className={`customer-row status-row ${s.is_blocking ? 'status-blocking' : ''}`}>
              <StatusBadge name={s.name} isBlocking={s.is_blocking} />
              <div className="status-main">
                <Link className="customer-name-link" to={`/statuses/${s.id}`}>
                  {s.name}
                </Link>
                {s.is_blocking && <span className="blocking-badge">Blocking</span>}
              </div>
              <span className="customer-notes-preview">{s.description}</span>
              <span className="status-cust-count">
                {s.client_count} {s.client_count === 1 ? 'client' : 'clients'}
              </span>
              <div className="row-actions">
                <button className="btn-secondary btn-sm" onClick={() => navigate(`/statuses/${s.id}/edit`)}>
                  Edit
                </button>
                <button className="btn-danger btn-sm" onClick={() => handleDelete(s)}>
                  Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </>
  );
}
