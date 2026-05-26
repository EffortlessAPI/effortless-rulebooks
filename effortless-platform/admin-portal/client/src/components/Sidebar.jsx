import { useLocation, useNavigate } from "react-router-dom";
import * as Icons from "lucide-react";
import { AREAS, roleMeetsMin } from "../lib/nav.js";

const AREA_ORDER = ["main", "developer", "admin", "docs"];

function NavIcon({ name }) {
  const I = Icons[name] || Icons.Circle;
  return <I size={15} />;
}

export default function Sidebar({ projectRulebook, me }) {
  const location = useLocation();
  const navigate = useNavigate();

  const nav     = (projectRulebook?.AppNavigation?.data || []).slice().sort((a, b) => (a.Order ?? 0) - (b.Order ?? 0));
  const roles   = projectRulebook?.UserRoles?.data || [];
  const screens = projectRulebook?.AppScreens?.data || [];
  const myRole  = roles.find((r) => r.RoleId === me?.RoleId);

  const screenPath = (sid) => screens.find((s) => s.ScreenId === sid)?.Path || "/";
  const currentArea = AREA_ORDER.find((a) => location.pathname.startsWith("/" + a)) || "main";

  // Group top-level nav items by NavArea
  const byArea = {};
  for (const area of AREA_ORDER) byArea[area] = [];

  for (const n of nav) {
    if (n.ParentNavId) continue; // children handled inline
    if (!roleMeetsMin(myRole, n.MinRoleId, roles)) continue;
    const area = n.NavArea || "main";
    if (byArea[area]) byArea[area].push(n);
  }

  return (
    <div className="sidebar">
      {AREA_ORDER.map((area) => {
        const meta    = AREAS[area];
        const items   = byArea[area];
        const isActive = area === currentArea;

        return (
          <div key={area} className={`nav-area ${isActive ? "active-area" : ""}`}>
            <div className="nav-area-header" style={{ borderLeftColor: meta.color }}
                 onClick={() => {
                   // Jump to landing screen for this area
                   const first = items[0];
                   if (first?.ScreenId) navigate(screenPath(first.ScreenId));
                   else if (area === "docs") navigate("/docs/framing");
                 }}>
              <NavIcon name={meta.icon} />
              <span style={{ color: meta.color }}>{meta.label}</span>
            </div>

            {isActive && items.map((n) => {
              const target = n.ScreenId ? screenPath(n.ScreenId) : null;
              const isNavActive = target === location.pathname;
              const children = nav
                .filter((c) => c.ParentNavId === n.NavId)
                .filter((c) => roleMeetsMin(myRole, c.MinRoleId, roles));

              return (
                <div key={n.NavId}>
                  <div
                    className={`nav-item ${isNavActive ? "active" : ""} ${!n.ScreenId ? "group-label" : ""}`}
                    onClick={() => target && navigate(target)}>
                    <NavIcon name={n.Icon || "Circle"} />
                    <span>{n.Label}</span>
                  </div>
                  {children.map((c) => {
                    const cp = screenPath(c.ScreenId);
                    return (
                      <div key={c.NavId}
                           className={`nav-item child ${cp === location.pathname ? "active" : ""}`}
                           onClick={() => navigate(cp)}>
                        <NavIcon name={c.Icon || "Circle"} />
                        <span>{c.Label}</span>
                      </div>
                    );
                  })}
                </div>
              );
            })}
          </div>
        );
      })}

      <div className="sidebar-footer">rulebook is SSoT · postgres is editor</div>
    </div>
  );
}
