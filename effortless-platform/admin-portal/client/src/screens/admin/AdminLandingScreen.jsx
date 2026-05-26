import { useNavigate } from "react-router-dom";
import ScreenHeader from "../../components/ScreenHeader.jsx";

export default function AdminLandingScreen({ screen, projectRulebook }) {
  const navigate = useNavigate();
  const users  = projectRulebook?.AppUsers?.data || [];
  const roles  = projectRulebook?.UserRoles?.data || [];
  const perms  = projectRulebook?.AppPermissions?.data || [];
  const navs   = projectRulebook?.AppNavigation?.data || [];
  const screens = projectRulebook?.AppScreens?.data || [];

  const items = [
    { label: "Users",        count: users.length,   path: "/admin/users",       desc: "Portal user accounts and role assignments." },
    { label: "Roles",        count: roles.length,   path: "/admin/users/roles", desc: "Role definitions, personas, and capability flags." },
    { label: "Permissions",  count: perms.length,   path: "/admin/permissions", desc: "Resource/action/role permission matrix." },
    { label: "Navigation",   count: navs.length,    path: "/admin/navigation",  desc: "Sidebar nav items, areas, icons, and role gates." },
    { label: "Screens",      count: screens.length, path: "/admin/screens",     desc: "Screen definitions, paths, layouts, and roles." },
  ];

  return (
    <>
      <ScreenHeader screen={screen || { Title: "Admin", Story: "Portal configuration — users, roles, permissions, navigation, and screens. All changes write through to the project rulebook JSON." }} />
      <div className="cards">
        {items.map((it) => (
          <div key={it.label} className="card clickable" onClick={() => navigate(it.path)}>
            <h3>{it.label}</h3>
            <div className="big">{it.count}</div>
            <div className="sub">{it.desc}</div>
          </div>
        ))}
      </div>
    </>
  );
}
