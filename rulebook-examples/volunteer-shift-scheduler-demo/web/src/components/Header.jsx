import { NavLink, useNavigate } from 'react-router-dom';
import { clearIdentity } from '../auth.js';

export default function Header({ identity }) {
  const navigate = useNavigate();
  const onLogout = () => {
    clearIdentity();
    navigate('/login');
  };
  return (
    <header className="topbar">
      <h1>Volunteer Shift Scheduler</h1>
      <nav>
        {identity?.kind === 'coordinator' && (
          <>
            <NavLink to="/" end className={({ isActive }) => (isActive ? 'active' : '')}>
              Dashboard
            </NavLink>
            <NavLink to="/volunteers" className={({ isActive }) => (isActive ? 'active' : '')}>
              Volunteers
            </NavLink>
            <NavLink to="/rules" className={({ isActive }) => (isActive ? 'active' : '')}>
              Rules
            </NavLink>
          </>
        )}
        {identity?.kind === 'viewer' && (
          <NavLink to="/rules" className={({ isActive }) => (isActive ? 'active' : '')}>
            Rules
          </NavLink>
        )}
        {identity?.kind === 'volunteer' && (
          <NavLink to="/my" className={({ isActive }) => (isActive ? 'active' : '')}>
            My Shifts
          </NavLink>
        )}
        {identity?.kind === 'viewer' && (
          <NavLink to="/" end className={({ isActive }) => (isActive ? 'active' : '')}>
            Dashboard (read-only)
          </NavLink>
        )}
      </nav>
      <div className="identity">
        <span className="small">signed in as</span>
        <span className="pill">{identity?.label || '—'}</span>
        <a href="/api/schema.xlsx" className="small" title="Download schema + data as Excel">
          schema.xlsx
        </a>
        <button className="ghost small" onClick={onLogout}>
          Switch identity
        </button>
      </div>
    </header>
  );
}
