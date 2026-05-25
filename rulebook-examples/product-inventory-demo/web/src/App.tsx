import { useEffect, useState } from 'react';
import { Routes, Route, Navigate, useNavigate, useParams } from 'react-router-dom';
import { api } from './lib/api';
import Login from './pages/Login';
import Shell from './Shell';
import Dashboard from './pages/Dashboard';
import Products from './pages/Products';
import Transactions from './pages/Transactions';
import Placeholder from './pages/Placeholder';
import { FieldDag, RoutingContext } from './explainer-dag';
import './explainer-dag/dag.css';

interface User {
  email: string;
  role: string;
}

function useDagRouting() {
  const navigate = useNavigate();
  return {
    FieldLink: ({ table, field, className, children }: {
      table: string; field: string; className?: string; children: React.ReactNode;
    }) => (
      <button
        type="button"
        onClick={() => navigate(`/dag/${table}/${field}`)}
        className={className}
        style={{ background: 'none', border: 'none', padding: 0, cursor: 'pointer', color: 'inherit', font: 'inherit' }}
      >
        {children}
      </button>
    ),
    onBack: () => navigate(-1),
    navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
  };
}

function AppRoutes({ user, onLogout }: { user: User; onLogout: () => void }) {
  const dagRouting = useDagRouting();
  return (
    <RoutingContext.Provider value={dagRouting}>
      <Shell user={user} onLogout={onLogout}>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/products" element={user.role === 'WarehouseManager' ? <Products /> : <Placeholder title="Products" description="View and manage product inventory" />} />
          <Route path="/transactions" element={user.role === 'WarehouseManager' ? <Transactions /> : <Placeholder title="Transactions" description="View all inventory transactions" />} />
          <Route path="/dag/:table/:field" element={<DagRoute />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Shell>
    </RoutingContext.Provider>
  );
}

function DagRoute() {
  const { table = "", field = "" } = useParams();
  const navigate = useNavigate();
  return (
    <FieldDag
      table={table}
      field={field}
      routing={{
        FieldLink: ({ table, field, className, children }) => (
          <button
            type="button"
            onClick={() => navigate(`/dag/${table}/${field}`)}
            className={className}
            style={{ background: 'none', border: 'none', padding: 0, cursor: 'pointer', color: 'inherit' }}
          >
            {children}
          </button>
        ),
        navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
        onBack: () => navigate(-1),
      }}
    />
  );
}

export default function App() {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    checkAuth();
  }, []);

  async function checkAuth() {
    try {
      const response = await api('/api/me');
      if (response.ok) {
        setUser(await response.json());
      }
    } catch (err) {
      // Not logged in
    } finally {
      setLoading(false);
    }
  }

  async function logout() {
    setUser(null);
  }

  if (loading) {
    return <div className="container"><p>Loading...</p></div>;
  }

  if (!user) {
    return <Login onLogin={(u) => { setUser(u); }} />;
  }

  return <AppRoutes user={user} onLogout={logout} />;
}
