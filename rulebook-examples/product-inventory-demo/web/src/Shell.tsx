import { ReactNode } from 'react';
import { Link } from 'react-router-dom';
import { DagToggle } from './explainer-dag';

interface User {
  email: string;
  role: string;
}

interface ShellProps {
  user: User;
  onLogout: () => void;
  children: ReactNode;
}

export default function Shell({ user, onLogout, children }: ShellProps) {
  return (
    <div style={{ display: 'flex', minHeight: '100vh' }}>
      <nav style={{ width: '250px', background: '#2c3e50', color: 'white', padding: '2rem 0', position: 'fixed', height: '100vh', overflowY: 'auto' }}>
        <div style={{ padding: '0 1.5rem', marginBottom: '2rem' }}>
          <h2 style={{ fontSize: '1.2rem', margin: 0 }}>Inventory</h2>
          <p style={{ fontSize: '0.85rem', margin: '0.5rem 0 0 0', opacity: 0.8 }}>{user.role}</p>
          <p style={{ fontSize: '0.75rem', margin: '0.25rem 0 0 0', opacity: 0.6 }}>{user.email}</p>
        </div>

        <div style={{ marginBottom: '2rem' }}>
          {user.role === 'WarehouseManager' && (
            <>
              <Link to="/" style={{ display: 'block', padding: '0.75rem 1.5rem', color: 'white', textDecoration: 'none' }}>📊 Dashboard</Link>
              <Link to="/products" style={{ display: 'block', padding: '0.75rem 1.5rem', color: 'white', textDecoration: 'none' }}>📦 Products</Link>
              <Link to="/transactions" style={{ display: 'block', padding: '0.75rem 1.5rem', color: 'white', textDecoration: 'none' }}>📝 Transactions</Link>
            </>
          )}
          {user.role !== 'WarehouseManager' && (
            <Link to="/" style={{ display: 'block', padding: '0.75rem 1.5rem', color: 'white', textDecoration: 'none' }}>📊 Home</Link>
          )}
        </div>

        <button
          onClick={onLogout}
          style={{
            width: '90%',
            margin: '0 auto',
            display: 'block',
            padding: '0.75rem',
            background: '#e74c3c',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer',
            fontSize: '0.9rem',
          }}
        >
          Sign out
        </button>
      </nav>

      <main style={{ marginLeft: '250px', flex: 1, padding: '2rem' }}>
        <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: '1rem' }}>
          <DagToggle />
        </div>
        {children}
      </main>
    </div>
  );
}
