import { useEffect, useState } from "react";
import { Route, Routes, Navigate, useLocation, Link, useParams, useNavigate } from "react-router-dom";
import { api, clearEmail, getEmail } from "./lib/api";
import { Me } from "./types";
import Login from "./Login";
import Shell from "./Shell";
import Dashboard from "./pages/Dashboard";
import Tables from "./pages/Tables";
import TableDetail from "./pages/TableDetail";
import Guests from "./pages/Guests";
import GuestDetail from "./pages/GuestDetail";
import Relationships from "./pages/Relationships";
import Placeholder from "./pages/Placeholder";
import { FieldDag, RoutingContext } from "./explainer-dag";
import "./explainer-dag/dag.css";

function useDagRouting() {
  const navigate = useNavigate();
  return {
    FieldLink: ({ table, field, className, children }: { table: string; field: string; className?: string; children: React.ReactNode }) => (
      <Link to={`/dag/${table}/${field}`} className={className}>{children}</Link>
    ),
    onBack: () => navigate(-1),
    navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
  };
}

function DagRoute() {
  const { table = "", field = "" } = useParams();
  return <FieldDag table={table} field={field} routing={useDagRouting()} />;
}

export default function App() {
  const [me, setMe] = useState<Me | null>(null);
  const [loading, setLoading] = useState(true);
  const [bumpKey, setBumpKey] = useState(0);
  const loc = useLocation();
  const dagRouting = useDagRouting();

  useEffect(() => {
    const email = getEmail();
    if (!email) {
      setMe(null);
      setLoading(false);
      return;
    }
    setLoading(true);
    api<Me>("/api/me")
      .then(setMe)
      .catch(() => {
        clearEmail();
        setMe(null);
      })
      .finally(() => setLoading(false));
  }, [bumpKey]);

  if (loading) return <div style={{ padding: 40 }}>Loading…</div>;
  if (!me) return <Login onLogin={() => setBumpKey((k) => k + 1)} />;

  const logout = () => {
    clearEmail();
    setMe(null);
  };

  const isCoord = me.role === "coordinator";

  return (
    <RoutingContext.Provider value={dagRouting}>
    <Shell me={me} onLogout={logout}>
      <Routes>
        <Route path="/" element={isCoord ? <Dashboard /> : <Placeholder me={me} title="Home" />} />
        <Route path="/tables" element={<Tables />} />
        <Route path="/tables/:id" element={<TableDetail me={me} />} />
        <Route path="/guests" element={isCoord ? <Guests /> : <Placeholder me={me} title="Guests" />} />
        <Route path="/guests/:id" element={isCoord ? <GuestDetail /> : <Navigate to="/" replace />} />
        <Route path="/relationships" element={isCoord ? <Relationships /> : <Navigate to="/" replace />} />
        <Route path="/dag/:table/:field" element={<DagRoute />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
      <span style={{ display: "none" }}>{loc.pathname}</span>
    </Shell>
    </RoutingContext.Provider>
  );
}
