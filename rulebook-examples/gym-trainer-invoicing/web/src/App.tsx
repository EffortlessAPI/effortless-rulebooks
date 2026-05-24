import { useEffect, useState } from "react";
import { Route, Routes, Navigate, useLocation, Link, useParams, useNavigate } from "react-router-dom";
import { api, clearEmail, getEmail } from "./lib/api";
import { Me } from "./types";
import Login from "./Login";
import Shell from "./Shell";
import Dashboard from "./pages/Dashboard";
import Clients from "./pages/Clients";
import ClientDetail from "./pages/ClientDetail";
import Sessions from "./pages/Sessions";
import Invoices from "./pages/Invoices";
import InvoiceDetail from "./pages/InvoiceDetail";
import Settings from "./pages/Settings";
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

  const isTrainer = me.role === "trainer";

  return (
    <RoutingContext.Provider value={dagRouting}>
    <Shell me={me} onLogout={logout}>
      <Routes>
        <Route path="/" element={isTrainer ? <Dashboard me={me} /> : <Placeholder me={me} title="Home" />} />
        <Route path="/clients" element={isTrainer ? <Clients /> : <Placeholder me={me} title="Clients" />} />
        <Route path="/clients/:id" element={isTrainer ? <ClientDetail /> : <Navigate to="/" replace />} />
        <Route path="/sessions" element={isTrainer ? <Sessions /> : <Placeholder me={me} title="Sessions" />} />
        <Route path="/invoices" element={<Invoices me={me} />} />
        <Route path="/invoices/:id" element={<InvoiceDetail me={me} />} />
        <Route path="/settings" element={isTrainer ? <Settings me={me} /> : <Navigate to="/" replace />} />
        <Route path="/dag/:table/:field" element={<DagRoute />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
      {/* tiny location debug suppressor */}
      <span style={{ display: "none" }}>{loc.pathname}</span>
    </Shell>
    </RoutingContext.Provider>
  );
}
