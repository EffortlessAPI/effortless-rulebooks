import { NavLink } from 'react-router-dom';

export default function Nav() {
  return (
    <nav style={{
      background: '#1e1b4b',
      padding: '0 24px',
      display: 'flex',
      alignItems: 'center',
      gap: '0',
      boxShadow: '0 2px 8px rgba(0,0,0,0.15)'
    }}>
      <span style={{ color: '#a5b4fc', fontWeight: 800, fontSize: '1rem', marginRight: '24px', letterSpacing: '0.02em' }}>
        🧾 Invoice Tracker
      </span>
      <NavLink to="/" end style={({ isActive }) => navStyle(isActive)}>
        Clients
      </NavLink>
      <NavLink to="/client-categories" style={({ isActive }) => navStyle(isActive)}>
        Categories
      </NavLink>
      <NavLink to="/statuses" style={({ isActive }) => navStyle(isActive)}>
        Statuses
      </NavLink>
      <NavLink to="/products" style={({ isActive }) => navStyle(isActive)}>
        Products
      </NavLink>
      <NavLink to="/inventory" style={({ isActive }) => navStyle(isActive)}>
        Inventory
      </NavLink>
      <NavLink to="/invoices" style={({ isActive }) => navStyle(isActive)}>
        Invoices
      </NavLink>
      <NavLink to="/payments" style={({ isActive }) => navStyle(isActive)}>
        Payments
      </NavLink>
      <NavLink to="/approvals" style={({ isActive }) => navStyle(isActive)}>
        Approvals
      </NavLink>
      <NavLink to="/app-users" style={({ isActive }) => navStyle(isActive)}>
        Users
      </NavLink>
    </nav>
  );
}

function navStyle(isActive) {
  return {
    display: 'inline-block',
    padding: '14px 20px',
    color: isActive ? '#fff' : '#a5b4fc',
    fontWeight: isActive ? 700 : 500,
    fontSize: '0.92rem',
    textDecoration: 'none',
    borderBottom: isActive ? '3px solid #818cf8' : '3px solid transparent',
    transition: 'all 0.15s'
  };
}
