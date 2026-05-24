import { ReactNode } from "react";
import { NavLink } from "react-router-dom";
import { Me } from "./App";
import { DagToggle } from "./explainer-dag";

interface Props { me: Me; onLogout: () => void; children: ReactNode }

const therapistNav = [
  { to: "/", label: "Dashboard" },
  { to: "/clients", label: "Clients" },
  { to: "/sessions", label: "Sessions" },
  { to: "/goals", label: "Goals" },
];

export default function Shell({ me, onLogout, children }: Props) {
  const nav = me.role === "therapist" ? therapistNav : [{ to: "/", label: "Home" }];
  return (
    <div className="shell">
      <aside className="sidebar">
        <div className="brand">Therapist Helper</div>
        <nav>
          {nav.map((n) => (
            <NavLink key={n.to} to={n.to} end={n.to === "/"}>
              {n.label}
            </NavLink>
          ))}
        </nav>
        <div className="me">
          <div className="me-name">{me.full_name}</div>
          <div className="me-role">{me.role}</div>
          <button onClick={onLogout}>Switch user</button>
        </div>
      </aside>
      <main className="main">
        <div style={{ display: "flex", justifyContent: "flex-end", marginBottom: ".5rem" }}>
          <DagToggle />
        </div>
        {children}
      </main>
    </div>
  );
}
