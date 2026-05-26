import { useEffect, useState } from "react";
import {
  Link,
  Navigate,
  Route,
  Routes,
  useNavigate,
  useParams,
} from "react-router-dom";
import { api, getEmail, setEmail } from "./lib/api";
import { Login } from "./pages/Login";
import { CustomersList } from "./pages/CustomersList";
import { CustomerDetail } from "./pages/CustomerDetail";
import { OrdersList } from "./pages/OrdersList";
import { OrderDetail } from "./pages/OrderDetail";
import { PaymentsList } from "./pages/PaymentsList";
import { PaymentDetail } from "./pages/PaymentDetail";
import { Dashboard } from "./pages/Dashboard";
import { JetModelsList } from "./pages/JetModelsList";
import { JetModelDetail } from "./pages/JetModelDetail";
import { FCSList } from "./pages/FCSList";
import { FCSDetail } from "./pages/FCSDetail";
import { Placeholder } from "./pages/Placeholder";
import { Shell } from "./Shell";
import { FieldDag, RoutingContext } from "./explainer-dag";
import "./explainer-dag/dag.css";

interface Me {
  email: string;
  first_name: string | null;
  last_name: string | null;
  full_name: string | null;
  role: string;
}

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
  const { table = "", field = "" } = useParams();
  return <FieldDag table={table} field={field} routing={useDagRouting()} />;
}

export default function App() {
  const [me, setMe] = useState<Me | null>(null);
  const [loading, setLoading] = useState(true);
  const routing = useDagRouting();

  useEffect(() => {
    if (!getEmail()) {
      setLoading(false);
      return;
    }
    api<Me>("/api/me")
      .then(setMe)
      .catch(() => setEmail(null))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="loading">Loading…</div>;
  if (!me)
    return (
      <Login
        onPick={async (email) => {
          setEmail(email);
          const m = await api<Me>("/api/me");
          setMe(m);
        }}
      />
    );

  const logout = () => {
    setEmail(null);
    setMe(null);
  };

  return (
    <RoutingContext.Provider value={routing}>
      <Shell me={me} onLogout={logout}>
        <Routes>
          {me.role === "admin" ? (
            <>
              <Route path="/" element={<Dashboard />} />
              <Route path="/customers" element={<CustomersList />} />
              <Route path="/customers/:id" element={<CustomerDetail />} />
              <Route path="/orders" element={<OrdersList />} />
              <Route path="/orders/:id" element={<OrderDetail />} />
              <Route path="/payments" element={<PaymentsList />} />
              <Route path="/payments/:id" element={<PaymentDetail />} />
              <Route path="/jet-models" element={<JetModelsList />} />
              <Route path="/jet-models/:id" element={<JetModelDetail />} />
              <Route path="/flight-control-systems" element={<FCSList />} />
              <Route path="/flight-control-systems/:id" element={<FCSDetail />} />
            </>
          ) : (
            <>
              <Route
                path="/"
                element={
                  <Placeholder
                    role={me.role}
                    title="Viewer Home"
                    body="In a full build, viewers would see a read-only roster of customers. For this demo, the admin role is the fully-wired primary role — switch identities from the top-right to try it."
                  />
                }
              />
              <Route path="/customers" element={<Navigate to="/" replace />} />
              <Route path="/customers/:id" element={<Navigate to="/" replace />} />
              <Route path="/orders" element={<Navigate to="/" replace />} />
              <Route path="/orders/:id" element={<Navigate to="/" replace />} />
              <Route path="/payments" element={<Navigate to="/" replace />} />
              <Route path="/payments/:id" element={<Navigate to="/" replace />} />
              <Route path="/jet-models" element={<Navigate to="/" replace />} />
              <Route path="/jet-models/:id" element={<Navigate to="/" replace />} />
              <Route path="/flight-control-systems" element={<Navigate to="/" replace />} />
              <Route path="/flight-control-systems/:id" element={<Navigate to="/" replace />} />
            </>
          )}
          <Route path="/dag/:table/:field" element={<DagRoute />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Shell>
    </RoutingContext.Provider>
  );
}
