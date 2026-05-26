import { useLocation, useNavigate, useParams } from "react-router-dom";
import * as Icons from "lucide-react";
import { api } from "../lib/api.js";
import { toast } from "../lib/toast.js";
import DomainSwitcher from "../components/DomainSwitcher.jsx";

// `mode` is "viewer" | "developer" | "admin"
// `requiresDomain` is true when this layout's routes always have a :domain
// `domainOptional` means the layout has BOTH domain and no-domain routes
export default function RoleTopBar({
  mode,
  accent,
  label,
  icon = "Circle",
  requiresDomain = false,
  domainOptional = false,
  me,
  projects,
  reload,
  onToggleSidebar,
}) {
  const location = useLocation();
  const navigate = useNavigate();
  const params   = useParams();
  const Icon = Icons[icon] || Icons.Circle;

  const activeDomain = params.domain || projects?.active || null;
  const activeProj   = (projects?.projects || []).find((p) => p.id === activeDomain);

  const switchDomain = async (newId) => {
    if (!newId) return;
    try {
      // Tell the server too — it still drives rulebook loading from active-domain.txt
      await api.post(`/api/projects/${encodeURIComponent(newId)}/activate`);
      await reload();
      // Swap only the :domain segment of the URL, preserving the remainder.
      // If the current URL has no domain yet (e.g. /developer), drop into the
      // new domain's landing instead.
      let next;
      if (params.domain) {
        next = location.pathname.replace(
          new RegExp(`^/${mode}/${params.domain}(/|$)`),
          `/${mode}/${newId}$1`,
        );
      } else {
        next = `/${mode}/${newId}`;
      }
      navigate(next);
      toast(`Switched to ${newId}`, "ok");
    } catch (err) { toast("Switch failed: " + err.message, "error"); }
  };

  const goRolePicker = () => navigate("/");

  const showDomainBlock = requiresDomain || (domainOptional && !!params.domain);

  return (
    <div className="topbar" style={{ ["--role-accent"]: accent }}>
      <button className="hamburger" onClick={onToggleSidebar} aria-label="Toggle navigation">
        <Icons.Menu size={20} />
      </button>

      <div className="role-brand" onClick={goRolePicker} title="Change role">
        <span className="role-mark" style={{ background: accent }}>
          <Icon size={14} />
        </span>
        <span className="role-name">{label}</span>
      </div>

      {showDomainBlock && (
        <div className="domain-block">
          <span className="domain-sep">›</span>
          <div className="domain-info">
            <span className="domain-name">{activeProj?.displayName || activeProj?.name || activeDomain || "—"}</span>
            <DomainSwitcher
              projects={projects?.projects || []}
              activeId={activeDomain}
              onPick={switchDomain}
            />
          </div>
        </div>
      )}

      <div className="topbar-spacer" />

      <div className="topbar-user">
        {me?.DisplayName && (
          <span className="topbar-user-name">{me.DisplayName}</span>
        )}
        <button className="btn-icon" onClick={goRolePicker} title="Change role">
          <Icons.LogOut size={16} />
        </button>
      </div>
    </div>
  );
}
