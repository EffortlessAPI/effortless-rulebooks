import { NavLink } from "react-router-dom";
import { Me } from "./types";
import { DagToggle } from "./explainer-dag";

const coordNav = [
  { to: "/", label: "Dashboard", end: true },
  { to: "/tables", label: "Tables" },
  { to: "/guests", label: "Guests" },
  { to: "/relationships", label: "Relationships" },
];

const placeholderNav = [
  { to: "/", label: "Home", end: true },
  { to: "/tables", label: "Tables" },
];

export default function Shell({ me, onLogout, children }: { me: Me; onLogout: () => void; children: React.ReactNode }) {
  const nav = me.role === "coordinator" ? coordNav : placeholderNav;
  return (
    <div className="layout">
      <aside className="sidebar">
        <h1>
          Wedding Seating<span className="role-pill">{me.role}</span>
        </h1>
        <div className="who">
          {me.display_name}
          <br />
          <span style={{ color: "#6c7186" }}>{me.email}</span>
        </div>
        {nav.map((n) => (
          <NavLink key={n.to} to={n.to} end={n.end} className={({ isActive }) => (isActive ? "active" : "")}>
            {n.label}
          </NavLink>
        ))}
        <button className="logout" onClick={onLogout}>
          Sign out
        </button>
      </aside>
      <main className="main">
        <div style={{ display: "flex", justifyContent: "flex-end", marginBottom: 8 }}><DagToggle /></div>
        {children}
      </main>
    </div>
  );
}
