import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchAppUser, deleteAppUser } from '../api.js';

export default function AppUserDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [user, setUser]       = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError]     = useState('');

  useEffect(() => {
    fetchAppUser(id)
      .then(setUser)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [id]);

  const handleDelete = async () => {
    if (!window.confirm(`Delete user "${user.contact_name || user.email}"?`)) return;
    try { await deleteAppUser(id); navigate('/app-users'); }
    catch (err) { alert(err.message); }
  };

  if (loading) return <p>Loading…</p>;
  if (error)   return <p className="error-msg">{error}</p>;
  if (!user)   return null;

  return (
    <div className="page-card">
      <Link className="back-link" to="/app-users">← Back to users</Link>
      <h2>
        {user.contact_name || user.email}
        <span className="tag" style={{ marginLeft: 10 }}>{user.role}</span>
      </h2>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 0 }}>
        <div className="detail-field"><div className="field-label">Contact Name</div><div className="field-value">{user.contact_name || <em>—</em>}</div></div>
        <div className="detail-field"><div className="field-label">Email</div><div className="field-value">{user.email || <em>—</em>}</div></div>
        <div className="detail-field"><div className="field-label">Phone</div><div className="field-value">{user.phone || <em>—</em>}</div></div>
        <div className="detail-field"><div className="field-label">Role</div><div className="field-value">{user.role}</div></div>
        <div className="detail-field"><div className="field-label">Slug</div><div className="field-value" style={{ fontFamily: 'monospace', color: '#888' }}>{user.slug}</div></div>
      </div>

      {user.notes && (
        <div className="detail-field">
          <div className="field-label">Notes</div>
          <div className="field-value">{user.notes}</div>
        </div>
      )}

      <div style={{ marginTop: 24 }}>
        <h3 style={{ fontSize: 15, marginBottom: 12 }}>
          Approvals granted by this user ({(user.approvals || []).length})
        </h3>
        {(user.approvals || []).length === 0 ? (
          <p style={{ color: '#888', fontSize: 14 }}>No approvals yet.</p>
        ) : (
          <table className="data-table">
            <thead>
              <tr><th>Client</th><th>Status</th><th>Notes</th><th>Created</th></tr>
            </thead>
            <tbody>
              {user.approvals.map((a) => (
                <tr key={a.id}>
                  <td><Link to={`/clients/${a.client_id}`} className="link-plain">{a.client_name}</Link></td>
                  <td>
                    <span className={`tag ${a.is_approved ? 'tag-green' : 'tag-grey'}`}>
                      {a.is_approved ? 'Approved' : 'Pending'}
                    </span>
                  </td>
                  <td>{a.notes || <em style={{ color: '#aaa' }}>—</em>}</td>
                  <td>{a.created_at ? new Date(a.created_at).toLocaleDateString() : '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <div className="form-actions">
        <button className="btn-primary" onClick={() => navigate(`/app-users/${id}/edit`)}>Edit</button>
        <button className="btn-danger" onClick={handleDelete}>Delete</button>
        <button className="btn-secondary" onClick={() => navigate('/app-users')}>Back</button>
      </div>
    </div>
  );
}
