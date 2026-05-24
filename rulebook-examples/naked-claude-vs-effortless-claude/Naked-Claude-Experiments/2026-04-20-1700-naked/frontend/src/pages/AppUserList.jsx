import { useState, useEffect, useCallback } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { fetchAppUsers, deleteAppUser } from '../api.js';

const ROLE_CLASS = { Admin: 'tag-green', Manager: 'tag', Customer: 'tag-grey' };

export default function AppUserList() {
  const [users, setUsers]     = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  const load = useCallback(async () => {
    setLoading(true);
    try { setUsers(await fetchAppUsers()); }
    catch (e) { console.error(e); }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { load(); }, [load]);

  const handleDelete = async (u) => {
    if (!window.confirm(`Delete user "${u.contact_name || u.email}"?`)) return;
    try { await deleteAppUser(u.id); load(); }
    catch (err) { alert(err.message); }
  };

  return (
    <>
      <div className="list-header">
        <h2>App Users {!loading && `(${users.length})`}</h2>
        <button className="btn-primary" onClick={() => navigate('/app-users/new')}>+ Add User</button>
      </div>
      {loading ? <p>Loading…</p> : users.length === 0 ? (
        <p style={{ color: '#888' }}>No users yet.</p>
      ) : (
        <table className="data-table">
          <thead>
            <tr>
              <th>Contact Name</th>
              <th>Email</th>
              <th>Phone</th>
              <th>Role</th>
              <th>Approvals</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.id}>
                <td><Link to={`/app-users/${u.id}`} className="link-plain">{u.contact_name || <em>—</em>}</Link></td>
                <td>{u.email || '—'}</td>
                <td>{u.phone || '—'}</td>
                <td><span className={`tag ${ROLE_CLASS[u.role] || 'tag'}`}>{u.role}</span></td>
                <td>{u.approval_count}</td>
                <td>
                  <div className="row-actions">
                    <button className="btn-secondary btn-sm" onClick={() => navigate(`/app-users/${u.id}/edit`)}>Edit</button>
                    <button className="btn-danger btn-sm" onClick={() => handleDelete(u)}>Delete</button>
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
