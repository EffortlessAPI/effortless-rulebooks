import { NavLink, Outlet } from 'react-router-dom';
import './Shell.css';

export function Shell() {
  return (
    <div className="shell">
      <nav className="sidebar">
        <div className="sidebar-title">
          <span className="sidebar-logo">∫</span>
          Simpson's Paradox
        </div>
        <NavLink to="/overview" className={({ isActive }) => isActive ? 'nav-item active' : 'nav-item'}>
          Study Overview
        </NavLink>
        <NavLink to="/stratum" className={({ isActive }) => isActive ? 'nav-item active' : 'nav-item'}>
          Stratum Breakdown
        </NavLink>
        <NavLink to="/weights" className={({ isActive }) => isActive ? 'nav-item active' : 'nav-item'}>
          Allocation Weights
        </NavLink>
        <NavLink to="/sandbox" className={({ isActive }) => isActive ? 'nav-item active' : 'nav-item'}>
          Interactive Sandbox
        </NavLink>
        <NavLink to="/model" className={({ isActive }) => isActive ? 'nav-item active' : 'nav-item'}>
          Model Summary
        </NavLink>
        <div className="sidebar-sep" />
        <a
          className="nav-item nav-item-external"
          href="../simpsons-paradox-explorer.html"
          target="_blank"
          rel="noopener noreferrer"
        >
          ✉ Email-ready HTML
        </a>
      </nav>
      <main className="content">
        <Outlet />
      </main>
    </div>
  );
}
