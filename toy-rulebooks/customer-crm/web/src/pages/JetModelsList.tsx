import { Link } from "react-router-dom";
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

const fmtM = (n: string | number | null) =>
  n == null ? "—" : `$${Number(n).toFixed(1)}M`;

export function JetModelsList() {
  const { data, loading } = useApi<JetModel[]>("/api/jet-models");
  if (loading || !data) return <div className="loading">Loading…</div>;

  return (
    <div className="page">
      <h1>Jet Models</h1>
      <p className="muted">
        Airframe catalog. FCS variants link here; revenue rolls up from order lines.
      </p>
      <table className="grid">
        <thead>
          <tr>
            <th>Model</th>
            <th>Manufacturer</th>
            <th style={{ textAlign: "right" }}>Gen</th>
            <th>5th-gen</th>
            <th style={{ textAlign: "right" }}>FCS variants</th>
            <th style={{ textAlign: "right" }}>Units ordered</th>
            <th style={{ textAlign: "right" }}>Revenue ($M)</th>
          </tr>
        </thead>
        <tbody>
          {data.map((j) => (
            <tr key={j.jet_models_id}>
              <td>
                <Link to={`/jet-models/${encodeURIComponent(j.jet_models_id)}`}>
                  {j.model_code}
                </Link>
              </td>
              <td>{j.manufacturer}</td>
              <td style={{ textAlign: "right" }}>{j.generation}</td>
              <td>
                <DagCell table="JetModels" field="IsFifthGen">
                  {j.is_fifth_gen ? "✅" : "—"}
                </DagCell>
              </td>
              <td style={{ textAlign: "right" }}>
                <DagCell table="JetModels" field="FCSVariantCount">
                  {j.fcs_variant_count ?? 0}
                </DagCell>
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
    </div>
  );
}
