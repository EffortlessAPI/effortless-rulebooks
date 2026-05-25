import { api, setEmail } from "./lib/api";
import { useApi } from "./lib/useApi";
import { DevUser } from "./types";

export default function Login({ onLogin }: { onLogin: () => void }) {
  const { data, error, loading } = useApi<DevUser[]>("/api/dev-users");
  if (loading) return <div style={{ padding: 40 }}>Loading…</div>;
  if (error) return <div style={{ padding: 40, color: "#b3261e" }}>Cannot reach API: {error}</div>;

  const groups: Record<string, DevUser[]> = { trainer: [], client: [], admin: [] };
  for (const u of data ?? []) groups[u.role]?.push(u);

  const pick = (u: DevUser) => {
    setEmail(u.email);
    onLogin();
  };

  return (
    <div className="login-screen">
      <h1>Gym Trainer Invoicing</h1>
      <p className="muted">
        Dev login. Pick an identity to sign in as — this is a demo, no password required.
      </p>
      {(["trainer", "client", "admin"] as const).map((role) =>
        groups[role].length === 0 ? null : (
          <div key={role}>
            <div className="login-group-title">{role}s</div>
            <div className="login-list">
              {groups[role].map((u) => (
                <button key={u.user_id} className="login-btn" onClick={() => pick(u)}>
                  <span>
                    <span className="name">{u.display_name}</span>
                    <br />
                    <span className="email">{u.email}</span>
                  </span>
                  <span className={`role-tag ${role}`}>{role}</span>
                </button>
              ))}
            </div>
          </div>
        )
      )}
    </div>
  );
}
