import { useEffect, useState } from "react";
import { api, setEmail } from "../lib/api";
import type { Me } from "../App";

interface DevUser { users_id: string; full_name: string; role: string }

export default function Login({ onLogin }: { onLogin: (me: Me) => void }) {
  const [users, setUsers] = useState<DevUser[]>([]);
  const [err, setErr] = useState<string | null>(null);

  useEffect(() => {
    fetch("/api/dev-users")
      .then((r) => r.json())
      .then(setUsers)
      .catch((e) => setErr(String(e)));
  }, []);

  async function pick(u: DevUser) {
    setEmail(u.users_id);
    try {
      const me = await api<Me>("/api/me");
      onLogin(me);
    } catch (e: any) {
      setErr(String(e.message || e));
    }
  }

  const byRole: Record<string, DevUser[]> = {};
  users.forEach((u) => { (byRole[u.role] ||= []).push(u); });

  return (
    <div className="login">
      <h1>Therapist Helper Portal</h1>
      <p className="subtitle">Dev login — pick any identity below. No password.</p>
      {err && <div className="error">{err}</div>}
      {Object.entries(byRole).map(([role, list]) => (
        <section key={role}>
          <h2>{role}{role === "therapist" ? " (fully wired)" : " (placeholder)"}</h2>
          <ul>
            {list.map((u) => (
              <li key={u.users_id}>
                <button onClick={() => pick(u)}>
                  <strong>{u.full_name}</strong>
                  <span className="email">{u.users_id}</span>
                </button>
              </li>
            ))}
          </ul>
        </section>
      ))}
    </div>
  );
}
