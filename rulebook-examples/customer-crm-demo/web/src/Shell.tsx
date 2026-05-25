import { NavLink } from "react-router-dom";
import { DagToggle } from "./explainer-dag";
import { RulebookDownload } from "./RulebookDownload";

interface Me {
  email: string;
  full_name: string | null;
  role: string;
}

export function Shell({
  me,
  onLogout,
  children,
}: {
  me: Me;
  onLogout: () => void;
  children: React.ReactNode;
}) {
  const navItems =
    me.role === "admin"
      ? [
          { to: "/", label: "Dashboard", end: true },
          { to: "/customers", label: "Customers" },
          { to: "/orders", label: "Orders" },
          { to: "/payments", label: "Payments" },
          { to: "/jet-models", label: "Jet Models" },
          { to: "/flight-control-systems", label: "Flight Control Systems" },
        ]
      : [{ to: "/", label: "Home", end: true }];

  return (
    <div className="shell">
      <aside className="sidebar">
        <h1>Customer CRM</h1>
        <nav>
          {navItems.map((n) => (
            <NavLink key={n.to} to={n.to} end={n.end}>
              {n.label}
            </NavLink>
          ))}
        </nav>
        <div className="sidebar-bottom">
          <DagToggle />
        </div>
      </aside>
      <div className="main">
        <header className="topbar">
          <div className="me">
            <strong>{me.full_name || me.email}</strong>{" "}
            <span className="role">{me.role}</span>
          </div>
          <div className="topbar-actions">
            <RulebookDownload />
            <button onClick={onLogout}>Switch identity</button>
          </div>
        </header>
        <main className="content">{children}</main>
      </div>
    </div>
  );
}
