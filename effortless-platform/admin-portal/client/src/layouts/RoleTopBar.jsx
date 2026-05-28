import { useLocation, useNavigate, useParams } from "react-router-dom";
import * as Icons from "lucide-react";
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
  onToggleSidebar,
}) {
  const location = useLocation();
  const navigate = useNavigate();
  const params   = useParams();
  const Icon = Icons[icon] || Icons.Circle;

  const activeDomain = params.domain || projects?.active || null;
  const activeProj   = (projects?.projects || []).find((p) => p.id === activeDomain);

  const switchDomain = (newId) => {
    if (!newId) return;
    // Domain is a URL concern, not server-side state. Switching the dropdown
    // ONLY navigates — it does NOT POST /api/projects/:id/activate (which
    // would mutate orchestration/active-domain.txt, a shared scratchpad
    // used by CLI tools and this Claude conversation). Per CLAUDE.md
    // "`active-domain.txt` ≠ what this conversation is about", the UI uses
    // the URL as its source of truth and never touches that file.
    //
    // Known followup: per-domain Postgres pool. Until that lands, UI CRUD
    // writes (instance/schema PATCH/DELETE) still resolve against whatever
    // domain is in active-domain.txt — switching the dropdown does NOT
    // re-bind the pool. Pure read flows (Explorer tree/node/cell) are
    // already URL-driven and unaffected.
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
