import { useEffect, useState } from 'react';
import { Link, Navigate, Route, Routes, useNavigate, useParams } from 'react-router-dom';
import Login from './Login';
import Shell from './Shell';
import Dashboard from './pages/Dashboard';
import Intelligences from './pages/Intelligences';
import IntelligenceDetail from './pages/IntelligenceDetail';
import Capabilities from './pages/Capabilities';
import AssessmentsMatrix from './pages/AssessmentsMatrix';
import Placeholder from './pages/Placeholder';
import { api, getEmail, setEmail } from './lib/api';
import { FieldDag, RoutingContext } from './explainer-dag';
import './explainer-dag/dag.css';
import type { DevUser } from './types';

function useDagRouting() {
  const navigate = useNavigate();
  return {
    FieldLink: ({
      table,
      field,
      className,
      children,
    }: {
      table: string;
      field: string;
      className?: string;
      children: React.ReactNode;
    }) => (
      <Link to={`/dag/${table}/${field}`} className={className}>
        {children}
      </Link>
    ),
    onBack: () => navigate(-1),
    navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
  };
}

function DagRoute() {
  const { table = '', field = '' } = useParams();
  return <FieldDag table={table} field={field} routing={useDagRouting()} />;
}

export default function App() {
  const [me, setMe] = useState<DevUser | null>(null);
  const [loaded, setLoaded] = useState(false);

  useEffect(() => {
    const email = getEmail();
    if (!email) {
      setLoaded(true);
      return;
    }
    api<DevUser>('/me')
      .then((u) => setMe(u))
      .catch(() => setEmail(null))
      .finally(() => setLoaded(true));
  }, []);

  if (!loaded) return null;
  if (!me) {
    return (
      <Routes>
        <Route path="*" element={<Login />} />
      </Routes>
    );
  }

  return (
    <DagRoutingProvider>
      <Routes>
        <Route element={<Shell me={me} />}>
          {me.role === 'researcher' ? (
            <>
              <Route index element={<Dashboard />} />
              <Route path="intelligences" element={<Intelligences />} />
              <Route path="intelligences/:id" element={<IntelligenceDetail />} />
              <Route path="capabilities" element={<Capabilities />} />
              <Route path="assessments" element={<AssessmentsMatrix />} />
            </>
          ) : (
            <Route index element={<Placeholder me={me} />} />
          )}
          <Route path="dag/:table/:field" element={<DagRoute />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Route>
      </Routes>
    </DagRoutingProvider>
  );
}

function DagRoutingProvider({ children }: { children: React.ReactNode }) {
  const value = useDagRouting();
  return <RoutingContext.Provider value={value}>{children}</RoutingContext.Provider>;
}
