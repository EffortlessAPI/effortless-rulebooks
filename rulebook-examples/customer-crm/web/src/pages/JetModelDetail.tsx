import { Link, useParams } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { DagCell } from "../explainer-dag";

interface JetModel {
  jet_models_id: string;
  name: string;
  model_code: string;
  manufacturer: string;
  generation: number;
  is_fifth_gen: boolean | null;
  fcs_variant_count: string | null;
  total_units_ordered: string | null;
  total_revenue: string | null;
}

interface FCS {
  flight_control_systems_id: string;
  fcs_code: string;
  architecture: string;
  redundancy_channels: number;
  unit_price: string;
  is_triple_redundant: boolean | null;
  is_quad_redundant: boolean | null;
  meets_fifth_gen_spec: boolean | null;
  total_units_ordered: string | null;
  total_revenue: string | null;
}

const fmtM = (n: string | number | null) =>
  n == null ? "—" : `$${Number(n).toFixed(1)}M`;

export function JetModelDetail() {
  const { id = "" } = useParams();
  const { data: jet, loading } = useApi<JetModel>(
    `/api/jet-models/${encodeURIComponent(id)}`
  );
  const { data: fcs } = useApi<FCS[]>(
    `/api/flight-control-systems?jet_model_id=${encodeURIComponent(id)}`
  );

  if (loading || !jet) return <div className="loading">Loading…</div>;

  return (
    <div className="page">
      <p>
        <Link to="/jet-models">← Jet models</Link>
      </p>
      <h1>{jet.model_code}</h1>
      <p className="muted">
        {jet.manufacturer} · Generation {jet.generation}
      </p>

      <div className="cards">
        <div className="card">
          <div className="card-label">FCS variants</div>
          <div className="card-value">
            <DagCell table="JetModels" field="FCSVariantCount">
              {jet.fcs_variant_count ?? 0}
            </DagCell>
          </div>
        </div>
        <div className="card">
          <div className="card-label">Units ordered</div>
          <div className="card-value">
            <DagCell table="JetModels" field="TotalUnitsOrdered">
              {jet.total_units_ordered ?? 0}
            </DagCell>
          </div>
        </div>
        <div className="card">
          <div className="card-label">Revenue</div>
          <div className="card-value">
            <DagCell table="JetModels" field="TotalRevenue">
              {fmtM(jet.total_revenue)}
            </DagCell>
          </div>
        </div>
      </div>

      <h2>FCS variants for this airframe</h2>
      <table className="grid">
        <thead>
          <tr>
            <th>Code</th>
            <th>Architecture</th>
            <th style={{ textAlign: "right" }}>Channels</th>
            <th style={{ textAlign: "right" }}>Unit price</th>
            <th>5th-gen spec</th>
            <th style={{ textAlign: "right" }}>Units</th>
            <th style={{ textAlign: "right" }}>Revenue</th>
          </tr>
        </thead>
        <tbody>
          {(fcs ?? []).map((f) => (
            <tr key={f.flight_control_systems_id}>
              <td>
                <Link
                  to={`/flight-control-systems/${encodeURIComponent(
                    f.flight_control_systems_id
                  )}`}
                >
                  {f.fcs_code}
                </Link>
              </td>
              <td>{f.architecture}</td>
              <td style={{ textAlign: "right" }}>{f.redundancy_channels}</td>
              <td style={{ textAlign: "right" }}>{fmtM(f.unit_price)}</td>
              <td>
                <DagCell table="FlightControlSystems" field="MeetsFifthGenSpec">
                  {f.meets_fifth_gen_spec ? "✅" : "—"}
                </DagCell>
              </td>
              <td style={{ textAlign: "right" }}>
                <DagCell table="FlightControlSystems" field="TotalUnitsOrdered">
                  {f.total_units_ordered ?? 0}
                </DagCell>
              </td>
              <td style={{ textAlign: "right" }}>
                <DagCell table="FlightControlSystems" field="TotalRevenue">
                  {fmtM(f.total_revenue)}
                </DagCell>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
