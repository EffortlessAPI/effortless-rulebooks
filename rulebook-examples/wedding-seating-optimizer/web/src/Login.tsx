import { setEmail } from "./lib/api";
import { useApi } from "./lib/useApi";
import { DevUser, Role } from "./types";

const ROLES: Role[] = ["coordinator", "bride", "groom"];

export default function Login({ onLogin }: { onLogin: () => void }) {
  const { data, error, loading } = useApi<DevUser[]>("/api/dev-users");
  if (loading) return <div style={{ padding: 40 }}>Loading…</div>;
  if (error) return <div style={{ padding: 40, color: "#b3261e" }}>Cannot reach API: {error}</div>;

  const groups: Record<Role, DevUser[]> = { coordinator: [], bride: [], groom: [] };
  for (const u of data ?? []) groups[u.role]?.push(u);

  const pick = (u: DevUser) => {
    setEmail(u.email);
    onLogin();
  };

  return (
    <div className="login-screen">
      <h1>Wedding Seating Optimizer</h1>
      <p className="muted">
        Dev login. Pick an identity to sign in as — this is a demo, no password required.
        The <b>coordinator</b> view is fully wired; bride/groom are placeholders.
      </p>
      {ROLES.map((role) =>
        groups[role].length === 0 ? null : (
          <div key={role}>
            <div className="login-group-title">{role}</div>
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
