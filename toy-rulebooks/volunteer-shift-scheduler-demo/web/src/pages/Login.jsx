import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../api.js';
import { setIdentity } from '../auth.js';

export default function Login() {
  const navigate = useNavigate();
  const [identities, setIdentities] = useState(null);

  useEffect(() => {
    api.identities().then(setIdentities);
  }, []);

  const pick = (ident) => {
    setIdentity(ident);
    if (ident.kind === 'volunteer') navigate('/my');
    else if (ident.kind === 'viewer') navigate('/');
    else navigate('/');
  };

  if (!identities) return <div className="login-wrap"><div className="card">Loading…</div></div>;

  return (
    <div className="login-wrap">
      <h2>Sign in</h2>
      <p className="muted small">Dev login — pick an identity to continue.</p>
      <div className="card">
        <div className="identities">
          <button
            className="identity-btn"
            onClick={() => pick({ kind: 'coordinator', label: identities.coordinator.label })}
          >
            <span><strong>Coordinator</strong><br /><span className="muted small">Full schedule edit</span></span>
            <span className="role-tag">coordinator</span>
          </button>
          {identities.volunteers.map((v) => (
            <button
              key={v.id}
              className="identity-btn"
              onClick={() => pick({ kind: 'volunteer', id: v.id, label: v.label })}
            >
              <span>{v.label.replace(' (volunteer)', '')}<br /><span className="muted small">{v.email}</span></span>
              <span className="role-tag">volunteer</span>
            </button>
          ))}
          <button
            className="identity-btn"
            onClick={() => pick({ kind: 'viewer', label: identities.viewer.label })}
          >
            <span><strong>Board / Viewer</strong><br /><span className="muted small">Read-only operational picture</span></span>
            <span className="role-tag">viewer</span>
          </button>
        </div>
      </div>
    </div>
  );
}
