import { useEffect, useState } from "react";

interface DevUser {
  email: string;
  full_name: string | null;
  role: string;
}

export function Login({ onPick }: { onPick: (email: string) => void }) {
  const [users, setUsers] = useState<DevUser[]>([]);
  const [err, setErr] = useState<string | null>(null);

  useEffect(() => {
    fetch("/api/dev-users")
      .then((r) => r.json())
      .then(setUsers)
      .catch((e) => setErr(String(e)));
  }, []);

  const byRole = users.reduce<Record<string, DevUser[]>>((acc, u) => {
    (acc[u.role] ||= []).push(u);
    return acc;
  }, {});

  return (
    <div className="login">
      <div className="login-card">
        <h1>Customer CRM Demo</h1>
        <p className="muted">
          Pick an identity to sign in. <code>admin</code> is the fully-wired
          primary role; <code>viewer</code> is a placeholder.
        </p>
        {err && <div className="error">{err}</div>}
        {Object.entries(byRole).map(([role, list]) => (
          <div key={role} className="role-group">
            <h2>{role}</h2>
            <ul>
              {list.map((u) => (
                <li key={u.email}>
                  <button onClick={() => onPick(u.email)}>
                    <strong>{u.full_name || u.email}</strong>
                    <span className="muted"> &nbsp;{u.email}</span>
                  </button>
                </li>
              ))}
            </ul>
          </div>
        ))}
      </div>
    </div>
  );
}
