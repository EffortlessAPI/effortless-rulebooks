import { useApi } from './lib/useApi';
import { setEmail } from './lib/api';
import type { DevUser } from './types';

export default function Login() {
  const { data: users, loading, error } = useApi<DevUser[]>('/dev-users');

  const choose = (email: string) => {
    setEmail(email);
    window.location.href = '/';
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <h1>Taxonomy of Intelligence</h1>
        <p className="lede">
          An Effortless demo. Pick an identity to sign in — no password. Three roles are seeded;
          the <strong>researcher</strong> role is fully wired and exercises the DAG.
        </p>
        {loading && <p>Loading dev users…</p>}
        {error && <p className="err">Failed to load dev users: {String(error)}</p>}
        <div className="login-users">
          {(users ?? []).map((u) => (
            <button key={u.email} className="login-user" onClick={() => choose(u.email)}>
              <div className="login-user-name">{u.name}</div>
              <div className="login-user-role">{u.role}</div>
              <div className="login-user-blurb">{u.blurb}</div>
            </button>
          ))}
        </div>
      </div>
    </div>
  );
}
