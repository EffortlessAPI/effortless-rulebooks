import { useState, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { api } from "../lib/api.js";
import { toast } from "../lib/toast.js";
import { AREAS, areaOf } from "../lib/nav.js";

export default function TopBar({ me, projects, rulebook, projectRulebook, reload }) {
  const location = useLocation();
  const navigate = useNavigate();
  const [users, setUsers]   = useState([]);
  const [roles, setRoles]   = useState([]);

  useEffect(() => {
    api.get("/api/users").then(({ users: u, roles: r }) => {
      setUsers(u || []);
      setRoles(r || []);
    }).catch(() => {});
  }, [me]);

  const area     = areaOf(location.pathname);
  const areaMeta = AREAS[area] || AREAS.main;
  const screens  = projectRulebook?.AppScreens?.data || [];
  const screen   = screens.find((s) => s.Path === location.pathname);
  const role     = roles.find((r) => r.RoleId === me?.RoleId);
  const activeProj = projects?.projects?.find((p) => p.id === projects?.active);

  const copyPath = () => {
    navigator.clipboard.writeText(location.pathname).catch(() => {});
    toast("Path copied", "ok");
  };

  const switchProject = async (e) => {
    try {
      await api.post(`/api/projects/${encodeURIComponent(e.target.value)}/activate`);
      toast("Switched project");
      await reload();
      navigate("/");
    } catch (err) { toast("Switch failed: " + err.message, "error"); }
  };

  const switchUser = async (e) => {
    await api.post("/api/me/switch", { userId: e.target.value });
    toast("Signed in as new user");
    navigate("/");
    await reload();
  };

  return (
    <div className="topbar">
      <div className="brand">ERB Admin</div>

      <div className="breadcrumb">
        <span className="area-pill" style={{ background: areaMeta.color, color: "#0a0f1c" }}>
          {areaMeta.label}
        </span>
        {screen && <span className="crumb-sep">›</span>}
        {screen && <span className="crumb-title">{screen.Title}</span>}
        <span className="crumb-path mono" onClick={copyPath} title="Click to copy">
          {location.pathname}
        </span>
      </div>

      <div className="spacer" />

      <div className="topbar-project">
        <span className="muted small">project:</span>
        <select value={projects?.active || ""} onChange={switchProject} style={{ width: 180 }}>
          {(projects?.projects || []).map((p) => (
            <option key={p.id} value={p.id}>{p.name}</option>
          ))}
        </select>
      </div>

      {role && (
        <span className="pill"
          style={{ background: role.ColorTheme || "#333", color: "#fff", cursor: "pointer" }}
          title={role.Tagline || ""}
          onClick={() => navigate(`/admin/users/roles?role=${encodeURIComponent(role.RoleId)}`)}>
          {role.Name}
        </span>
      )}

      <select value={me?.UserId || ""} onChange={switchUser} style={{ width: 180 }}>
        {users.map((u) => (
          <option key={u.UserId} value={u.UserId}>{u.DisplayName} ({u.Email})</option>
        ))}
      </select>
    </div>
  );
}
