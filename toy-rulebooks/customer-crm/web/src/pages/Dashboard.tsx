import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { DagCell } from "../explainer-dag";

interface Customer {
  customers_id: string;
  name: string;
  email: string;
  first_name: string | null;
  last_name: string | null;
  full_name: string | null;
}

interface JetModel {
  jet_models_id: string;
  model_code: string;
  total_units_ordered: string | null;
  total_revenue: string | null;
}

const fmtM = (n: string | number | null) =>
  n == null ? "—" : `$${Number(n).toFixed(1)}M`;

export function Dashboard() {
  const { data, loading } = useApi<Customer[]>("/api/customers");
  const { data: jets } = useApi<JetModel[]>("/api/jet-models");
  if (loading || !data) return <div className="loading">Loading…</div>;

  const totalUnits = (jets ?? []).reduce(
    (sum, j) => sum + Number(j.total_units_ordered ?? 0),
    0
  );
  const totalRevenue = (jets ?? []).reduce(
    (sum, j) => sum + Number(j.total_revenue ?? 0),
    0
  );

  return (
    <div className="page">
      <h1>Dashboard</h1>
      <div className="cards">
        <div className="card">
          <div className="card-label">Total customers</div>
          <div className="card-value">{data.length}</div>
        </div>
        <div className="card">
          <div className="card-label">FCS units sold</div>
          <div className="card-value">{totalUnits}</div>
        </div>
        <div className="card">
          <div className="card-label">FCS revenue</div>
          <div className="card-value">{fmtM(totalRevenue)}</div>
        </div>
      </div>

      {jets && jets.length > 0 && (
        <>
          <h2>Revenue by jet model</h2>
          <table className="grid">
            <thead>
              <tr>
                <th>Model</th>
                <th style={{ textAlign: "right" }}>Units</th>
                <th style={{ textAlign: "right" }}>Revenue</th>
              </tr>
            </thead>
            <tbody>
              {jets.map((j) => (
                <tr key={j.jet_models_id}>
                  <td>
                    <Link to={`/jet-models/${encodeURIComponent(j.jet_models_id)}`}>
                      {j.model_code}
                    </Link>
                  </td>
                  <td style={{ textAlign: "right" }}>
                    <DagCell table="JetModels" field="TotalUnitsOrdered">
                      {j.total_units_ordered ?? 0}
                    </DagCell>
                  </td>
                  <td style={{ textAlign: "right" }}>
                    <DagCell table="JetModels" field="TotalRevenue">
                      <strong>{fmtM(j.total_revenue)}</strong>
                    </DagCell>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </>
      )}

      <h2>Recent customers</h2>
      <table className="grid">
        <thead>
          <tr>
            <th>Full name</th>
            <th>Email</th>
          </tr>
        </thead>
        <tbody>
          {data.slice(0, 5).map((c) => (
            <tr key={c.customers_id}>
              <td>
                <DagCell table="Customers" field="FullName">
                  <Link to={`/customers/${encodeURIComponent(c.customers_id)}`}>
                    {c.full_name}
                  </Link>
                </DagCell>
              </td>
              <td>{c.email}</td>
            </tr>
          ))}
        </tbody>
      </table>
      <p className="muted">
        Tip: toggle <em>Explain</em> in the top-right, then click any
        calculated value to see how it was derived.
      </p>
    </div>
  );
}
