import React, { useEffect, useState } from 'react';
import { api } from '../api.js';
import { useAuth } from '../auth.jsx';
import { useNavigate } from 'react-router-dom';

export default function Login() {
  const [users, setUsers] = useState([]);
  const [err, setErr] = useState(null);
  const auth = useAuth();
  const nav = useNavigate();
  useEffect(() => { api.get('/api/auth/demo-users').then(setUsers).catch((e) => setErr(e.message)); }, []);

  const pick = async (u) => {
    try {
      await auth.login(u.email);
      nav('/');
    } catch (e) { setErr(e.message); }
  };

  return (
    <div className="login">
      <div style={{ textAlign: 'center' }}>
        <h1 style={{ margin: 0 }}>First Valley Bank</h1>
        <div className="muted">Small Business Banking — sign in as a demo user</div>
      </div>
      <div className="card">
        <div className="card-header">Demo users (simulated RLS)</div>
        {err && <div className="card-body" style={{ color: 'var(--danger)' }}>{err}</div>}
        {users.map((u) => (
          <div key={u.name} className="user-row" onClick={() => pick(u)}>
            <div>
              <div>{u.full_name}</div>
              <div className="role">{u.email}</div>
            </div>
            <span className={'badge ' + roleBadge(u.role)}>{u.role}</span>
          </div>
        ))}
      </div>
      <div className="muted" style={{ fontSize: 12 }}>
        Each user lands in a role-specific portal. The backend SETs <span className="mono">app.current_user_role</span>
        &nbsp;and <span className="mono">app.current_user_name</span> on every transaction.
      </div>
    </div>
  );
}

function roleBadge(role) {
  if (role === 'Admin') return 'danger';
  if (role === 'Underwriter') return 'warn';
  if (role === 'RM') return 'info';
  return 'ok';
}
