import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { api } from "../lib/api";
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

const ARCHITECTURES = ["fly-by-wire", "fly-by-light", "analog-hybrid"] as const;

const fmtM = (n: string | number | null) =>
  n == null ? "—" : `$${Number(n).toFixed(1)}M`;

export function FCSDetail() {
  const { id = "" } = useParams();
  const path = `/api/flight-control-systems/${encodeURIComponent(id)}`;
  const { data, loading, reload } = useApi<FCS>(path);

  const [editing, setEditing] = useState(false);
  const [form, setForm] = useState({
    architecture: "fly-by-wire",
    redundancy_channels: "3",
    unit_price: "0",
  });

  if (loading || !data) return <div className="loading">Loading…</div>;

  const startEdit = () => {
    setForm({
      architecture: data.architecture,
      redundancy_channels: String(data.redundancy_channels),
      unit_price: String(data.unit_price),
    });
    setEditing(true);
  };

  const save = async (e: React.FormEvent) => {
    e.preventDefault();
    await api(path, {
      method: "PATCH",
      body: JSON.stringify({
        architecture: form.architecture,
        redundancy_channels: Number(form.redundancy_channels),
        unit_price: Number(form.unit_price),
      }),
    });
    setEditing(false);
    reload();
  };

  return (
    <div className="page">
      <p>
        <Link to="/flight-control-systems">← Flight control systems</Link>
      </p>
      <div className="page-header">
        <h1>{data.fcs_code}</h1>
        {!editing && <button onClick={startEdit}>Edit</button>}
      </div>
      <p className="muted">
        Airframe:{" "}
        <Link to={`/jet-models/${encodeURIComponent(data.jet_model_id)}`}>
          {data.jet_model_id}
        </Link>{" "}
        · {data.jet_manufacturer} · Gen {data.jet_generation}
      </p>

      <form className="edit-form" onSubmit={save}>
        <label>
          <span>Architecture</span>
          {editing ? (
            <select
              value={form.architecture}
              onChange={(e) => setForm({ ...form, architecture: e.target.value })}
            >
              {ARCHITECTURES.map((a) => (
                <option key={a} value={a}>{a}</option>
              ))}
            </select>
          ) : (
            <input value={data.architecture} disabled />
          )}
        </label>
        <label>
          <span>Redundancy channels</span>
          <input
            type={editing ? "number" : "text"}
            value={editing ? form.redundancy_channels : data.redundancy_channels}
            disabled={!editing}
            onChange={(e) =>
              setForm({ ...form, redundancy_channels: e.target.value })
            }
          />
        </label>
        <label>
          <span>Unit price ($M)</span>
          <input
            type={editing ? "number" : "text"}
            step="0.1"
            value={editing ? form.unit_price : fmtM(data.unit_price)}
            disabled={!editing}
            onChange={(e) => setForm({ ...form, unit_price: e.target.value })}
          />
        </label>

        {editing && (
          <div className="form-actions">
            <button type="submit">Save</button>{" "}
            <button type="button" onClick={() => setEditing(false)}>Cancel</button>
          </div>
        )}

        <div className="calculated">
          <span>Triple-redundant (calculated)</span>
          <DagCell table="FlightControlSystems" field="IsTripleRedundant">
            <strong>{data.is_triple_redundant ? "✅" : "⛔"}</strong>
          </DagCell>
        </div>
        <div className="calculated">
          <span>Quad-redundant (calculated)</span>
          <DagCell table="FlightControlSystems" field="IsQuadRedundant">
            <strong>{data.is_quad_redundant ? "✅" : "⛔"}</strong>
          </DagCell>
        </div>
        <div className="calculated">
          <span>Meets 5th-gen spec (calculated)</span>
          <DagCell table="FlightControlSystems" field="MeetsFifthGenSpec">
            <strong>{data.meets_fifth_gen_spec ? "✅" : "⛔"}</strong>
          </DagCell>
        </div>
        <div className="calculated">
          <span>Units ordered (aggregated)</span>
          <DagCell table="FlightControlSystems" field="TotalUnitsOrdered">
            <strong>{data.total_units_ordered ?? 0}</strong>
          </DagCell>
        </div>
        <div className="calculated">
          <span>Total revenue (aggregated)</span>
          <DagCell table="FlightControlSystems" field="TotalRevenue">
            <strong>{fmtM(data.total_revenue)}</strong>
          </DagCell>
        </div>
      </form>
    </div>
  );
}
