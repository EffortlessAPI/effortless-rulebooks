import { useLocation, useNavigate, useParams } from "react-router-dom";
import * as Icons from "lucide-react";
import { useState } from "react";
import { roleMeetsMin } from "../lib/nav.js";

function NavIcon({ name }) {
  const I = Icons[name] || Icons.Circle;
  return <I size={15} />;
}

function resolvePath(template, params) {
  if (!template) return null;
  return template.replace(/:(\w+)/g, (_, k) => encodeURIComponent(params[k] ?? `:${k}`));
}

// Shared sidebar primitive. `area` is the NavArea the layout owns
// (viewer | developer | admin). Docs is always appended.
export default function RoleSidebar({ area, accent, projectRulebook, me, mobileOpen, onCloseMobile }) {
  const location = useLocation();
  const navigate = useNavigate();
  const params   = useParams();
  const [collapsed, setCollapsed] = useState({});

  const nav     = (projectRulebook?.AppNavigation?.data || []).slice().sort((a, b) => (a.Order ?? 0) - (b.Order ?? 0));
  const roles   = projectRulebook?.UserRoles?.data || [];
  const screens = projectRulebook?.AppScreens?.data || [];
  const myRole  = roles.find((r) => r.RoleId === me?.RoleId);

  const screenPath = (sid) => screens.find((s) => s.ScreenId === sid)?.Path || null;

  // Items live in this layout's area. Docs are appended as a separate block.
  const owned = nav.filter((n) => n.NavArea === area);
  const docs  = nav.filter((n) => n.NavArea === "docs");

  const renderSection = (items, sectionKey) => {
    const top = items.filter((n) => !n.ParentNavId);
    return top.map((n) => {
      const template = n.ScreenId ? screenPath(n.ScreenId) : null;
      const target   = resolvePath(template, params);
      const visible  = roleMeetsMin(myRole, n.MinRoleId, roles);
      if (!visible) return null;

      const kids = items
        .filter((c) => c.ParentNavId === n.NavId && roleMeetsMin(myRole, c.MinRoleId, roles));
      const isGroup = !n.ScreenId && kids.length > 0;
      const isCollapsed = collapsed[n.NavId];
      const isActive = template && location.pathname === target;

      const goto = (p) => {
        if (!p) return;
        navigate(p);
        onCloseMobile && onCloseMobile();
      };

      return (
        <div key={n.NavId}>
          <div
            className={`nav-item ${isActive ? "active" : ""} ${isGroup ? "group-header" : ""}`}
            onClick={() => {
              if (isGroup) setCollapsed({ ...collapsed, [n.NavId]: !isCollapsed });
              else goto(target);
            }}
          >
            <NavIcon name={n.Icon || "Circle"} />
            <span>{n.Label}</span>
            {isGroup && (
              <span style={{ marginLeft: "auto", color: "var(--muted)" }}>
                {isCollapsed ? "▸" : "▾"}
              </span>
            )}
          </div>
          {!isCollapsed && kids.map((c) => {
            const cTpl = screenPath(c.ScreenId);
            const cPath = resolvePath(cTpl, params);
            const cActive = cPath && location.pathname === cPath;
            return (
              <div
                key={c.NavId}
                className={`nav-item child ${cActive ? "active" : ""}`}
                onClick={() => goto(cPath)}
              >
                <NavIcon name={c.Icon || "Circle"} />
                <span>{c.Label}</span>
              </div>
            );
          })}
        </div>
      );
    });
  };

  return (
    <>
      <div className={`sidebar ${mobileOpen ? "mobile-open" : ""}`}>
        <div className="sidebar-area-strip" style={{ background: accent }} />
        {renderSection(owned, "owned")}
        {docs.length > 0 && (
          <div className="sidebar-divider">Reference</div>
        )}
        {renderSection(docs, "docs")}
        <div className="sidebar-footer">
          <span className="muted">rulebook is HEAD</span>
        </div>
      </div>
      {mobileOpen && <div className="sidebar-scrim" onClick={onCloseMobile} />}
    </>
  );
}
