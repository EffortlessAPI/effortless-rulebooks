import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { DagCell } from "../explainer-dag";

interface FCS {
  flight_control_systems_id: string;
  fcs_code: string;
  jet_model_id: string;
  jet_manufacturer: string | null;
  jet_generation: number | null;
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

export function FCSList() {
  const { data, loading } = useApi<FCS[]>("/api/flight-control-systems");
  if (loading || !data) return <div className="loading">Loading…</div>;

  return (
    <div className="page">
      <h1>Flight Control Systems</h1>
      <p className="muted">
        Product variants by airframe. Edit a unit price or redundancy level and
        watch revenue and 5th-gen-spec flags recompute.
      </p>
      <table className="grid">
        <thead>
          <tr>
            <th>FCS code</th>
            <th>Jet</th>
            <th>Architecture</th>
            <th style={{ textAlign: "right" }}>Channels</th>
            <th style={{ textAlign: "right" }}>Unit price</th>
            <th>Triple</th>
            <th>Quad</th>
            <th>5th-gen spec</th>
            <th style={{ textAlign: "right" }}>Units</th>
            <th style={{ textAlign: "right" }}>Revenue</th>
          </tr>
        </thead>
        <tbody>
          {data.map((f) => (
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
              <td>
                <Link to={`/jet-models/${encodeURIComponent(f.jet_model_id)}`}>
                  {f.jet_model_id}
                </Link>
              </td>
              <td>{f.architecture}</td>
              <td style={{ textAlign: "right" }}>{f.redundancy_channels}</td>
              <td style={{ textAlign: "right" }}>{fmtM(f.unit_price)}</td>
              <td>
                <DagCell table="FlightControlSystems" field="IsTripleRedundant">
                  {f.is_triple_redundant ? "✅" : "—"}
                </DagCell>
              </td>
              <td>
                <DagCell table="FlightControlSystems" field="IsQuadRedundant">
                  {f.is_quad_redundant ? "✅" : "—"}
                </DagCell>
              </td>
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
                  <strong>{fmtM(f.total_revenue)}</strong>
                </DagCell>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
