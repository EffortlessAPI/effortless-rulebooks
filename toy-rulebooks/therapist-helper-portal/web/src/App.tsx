import React, { useEffect, useState } from "react";
import { Link, Navigate, Route, Routes, useNavigate, useParams } from "react-router-dom";
import { api, getEmail, clearEmail } from "./lib/api";
import Login from "./pages/Login";
import Shell from "./Shell";
import Dashboard from "./pages/Dashboard";
import Clients from "./pages/Clients";
import ClientDetail from "./pages/ClientDetail";
import Sessions from "./pages/Sessions";
import SessionDetail from "./pages/SessionDetail";
import Goals from "./pages/Goals";
import GoalDetail from "./pages/GoalDetail";
import Placeholder from "./pages/Placeholder";
import { FieldDag, RoutingContext } from "./explainer-dag";
import "./explainer-dag/dag.css";

function useDagRouting() {
  const navigate = useNavigate();
  return {
    FieldLink: ({ table, field, className, children }: {
      table: string; field: string; className?: string; children: React.ReactNode;
    }) => (
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

export interface Me { users_id: string; full_name: string; role: string }

export default function App() {
  const [me, setMe] = useState<Me | null>(null);
  const [ready, setReady] = useState(false);
  const dagRouting = useDagRouting();

  useEffect(() => {
    if (!getEmail()) { setReady(true); return; }
    api<Me>("/api/me")
      .then(setMe)
      .catch(() => { clearEmail(); setMe(null); })
      .finally(() => setReady(true));
  }, []);

  if (!ready) return <div className="loading">Loading…</div>;
  if (!me) return <Login onLogin={(m) => setMe(m)} />;

  const isTherapist = me.role === "therapist";

  return (
    <RoutingContext.Provider value={dagRouting}>
      <Shell me={me} onLogout={() => { clearEmail(); setMe(null); }}>
        <Routes>
          <Route path="/" element={isTherapist
            ? <Dashboard me={me} />
            : <Placeholder me={me} title="Home" />} />
          <Route path="/clients" element={isTherapist
            ? <Clients me={me} />
            : <Navigate to="/" replace />} />
          <Route path="/clients/:code" element={isTherapist
            ? <ClientDetail me={me} />
            : <Navigate to="/" replace />} />
          <Route path="/sessions" element={isTherapist
            ? <Sessions me={me} />
            : <Navigate to="/" replace />} />
          <Route path="/sessions/:code" element={isTherapist
            ? <SessionDetail me={me} />
            : <Navigate to="/" replace />} />
          <Route path="/goals" element={isTherapist
            ? <Goals me={me} />
            : <Navigate to="/" replace />} />
          <Route path="/goals/:code" element={isTherapist
            ? <GoalDetail me={me} />
            : <Navigate to="/" replace />} />
          <Route path="/dag/:table/:field" element={<DagRoute />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Shell>
    </RoutingContext.Provider>
  );
}
