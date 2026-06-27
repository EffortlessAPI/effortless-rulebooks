import { useState, useEffect, useCallback } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { fetchClientApprovals, deleteClientApproval } from '../api.js';

export default function ClientApprovalList() {
  const [rows, setRows]       = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  const load = useCallback(async () => {
    setLoading(true);
    try { setRows(await fetchClientApprovals()); }
    catch (e) { console.error(e); }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { load(); }, [load]);

  const handleDelete = async (a) => {
    if (!window.confirm(`Delete approval for "${a.client_name}"?`)) return;
    try { await deleteClientApproval(a.id); load(); }
    catch (err) { alert(err.message); }
  };

  const approved = rows.filter((r) => r.is_approved).length;
  const pending  = rows.length - approved;

  return (
    <>
      <div className="list-header">
        <h2>Client Approvals {!loading && `(${rows.length})`}</h2>
        <button className="btn-primary" onClick={() => navigate('/client-approvals/new')}>+ Add Approval</button>
      </div>
      {!loading && (
        <div style={{ marginBottom: 12, display: 'flex', gap: 10 }}>
          <span className="tag tag-green">{approved} Approved</span>
          <span className="tag tag-grey">{pending} Pending</span>
        </div>
      )}
      {loading ? <p>Loading…</p> : rows.length === 0 ? (
        <p style={{ color: '#888' }}>No approvals yet.</p>
      ) : (
        <table className="data-table">
          <thead>
            <tr>
              <th>Client</th>
              <th>Approved By</th>
              <th>Status</th>
              <th>Notes</th>
              <th>Created</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {rows.map((a) => (
              <tr key={a.id}>
                <td><Link to={`/clients/${a.client_id}`} className="link-plain">{a.client_name}</Link></td>
                <td>{a.approver_name
                    ? <Link to={`/app-users/${a.approved_by_user_id}`} className="link-plain">{a.approver_name}</Link>
                    : <em style={{ color: '#aaa' }}>—</em>}
                </td>
                <td>
                  <span className={`tag ${a.is_approved ? 'tag-green' : 'tag-grey'}`}>
                    {a.is_approved ? 'Approved' : 'Pending'}
                  </span>
                </td>
                <td>{a.notes || <em style={{ color: '#aaa' }}>—</em>}</td>
                <td>{a.created_at ? new Date(a.created_at).toLocaleDateString() : '—'}</td>
                <td>
                  <div className="row-actions">
                    <button className="btn-secondary btn-sm" onClick={() => navigate(`/client-approvals/${a.id}/edit`)}>Edit</button>
                    <button className="btn-danger btn-sm" onClick={() => handleDelete(a)}>Delete</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  );
}
