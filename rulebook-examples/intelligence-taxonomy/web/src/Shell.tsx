import { Link, NavLink, Outlet } from 'react-router-dom';
import { DagToggle } from './explainer-dag';
import { navFor } from './nav';
import { setEmail } from './lib/api';
import type { DevUser } from './types';

export default function Shell({ me }: { me: DevUser }) {
  const items = navFor(me.role);
  const onSignOut = () => {
    setEmail(null);
    window.location.href = '/';
  };
  return (
    <div className="app-shell">
      <aside className="sidebar">
        <Link to="/" className="brand">
          <div className="brand-title">Taxonomy of Intelligence</div>
          <div className="brand-sub">Effortless demo · {me.role}</div>
        </Link>
        <nav className="nav">
          {items.map((i) => (
            <NavLink
              key={i.to}
              to={i.to}
              end={i.to === '/'}
              className={({ isActive }) => (isActive ? 'nav-item active' : 'nav-item')}
            >
              {i.label}
            </NavLink>
          ))}
        </nav>
        <div className="who">
          <div className="who-name">{me.name}</div>
          <div className="who-email">{me.email}</div>
          <button className="signout" onClick={onSignOut}>
            Sign out
          </button>
        </div>
      </aside>
      <main className="content">
        <div className="content-top">
          <DagToggle />
        </div>
        <div className="content-body">
          <Outlet />
        </div>
      </main>
    </div>
  );
}
