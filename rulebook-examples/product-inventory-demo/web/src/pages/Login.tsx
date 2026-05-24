import { useEffect, useState } from 'react';
import { api } from '../lib/api';

interface User {
  email: string;
  role: string;
}

interface LoginProps {
  onLogin: (user: User) => void;
}

export default function Login({ onLogin }: LoginProps) {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      try {
        const res = await fetch('/api/dev-users');
        const data = await res.json();
        setUsers(data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    }
    load();
  }, []);

  async function handleLogin(email: string) {
    localStorage.setItem('email', email);
    const res = await api('/api/me');
    const user = await res.json();
    onLogin(user);
  }

  if (loading) return <div className="container"><p>Loading users...</p></div>;

  const grouped = users.reduce((acc, u) => {
    if (!acc[u.role]) acc[u.role] = [];
    acc[u.role].push(u);
    return acc;
  }, {} as Record<string, User[]>);

  return (
    <div className="container" style={{ maxWidth: '400px', margin: '4rem auto', textAlign: 'center' }}>
      <h1>Product Inventory Demo</h1>
      <p>Select your role to sign in</p>
      {Object.entries(grouped).map(([role, roleUsers]) => (
        <div key={role} style={{ marginBottom: '2rem' }}>
          <h3 style={{ fontSize: '0.9rem', color: '#666', marginBottom: '1rem' }}>{role}</h3>
          {roleUsers.map((u) => (
            <button
              key={u.email}
              onClick={() => handleLogin(u.email)}
              style={{
                width: '100%',
                padding: '1rem',
                marginBottom: '0.5rem',
                background: '#0066cc',
                color: 'white',
                border: 'none',
                borderRadius: '4px',
                cursor: 'pointer',
                fontSize: '0.95rem',
              }}
            >
              {u.email}
            </button>
          ))}
        </div>
      ))}
    </div>
  );
}
