import { useParams, Link } from "react-router-dom";
import { useState, useEffect } from "react";
import { useApi } from "../lib/useApi";
import { Guest, Table } from "../types";
import { api } from "../lib/api";
import { num } from "../lib/fmt";
import { DagCell } from "../explainer-dag";

export default function GuestDetail() {
  const { id } = useParams();
  const { data: guest, reload } = useApi<Guest>(id ? `/api/guests/${id}` : null);
  const { data: tables } = useApi<Table[]>("/api/tables");
  const [draft, setDraft] = useState<Partial<Guest>>({});

  useEffect(() => { if (guest) setDraft({}); }, [guest?.guest_id]);

  if (!guest) return <div>Loading…</div>;
  const v = (k: keyof Guest): any => (k in draft ? (draft as any)[k] : (guest as any)[k]);
  const set = (k: keyof Guest, val: any) => setDraft((d) => ({ ...d, [k]: val }));

  async function save() {
    await api(`/api/guests/${id}`, { method: "PATCH", body: JSON.stringify(draft) });
    reload();
  }

  return (
    <div>
      <h2>{guest.full_name}</h2>
      <p className="muted">
        Seated at: <Link to={`/tables/${guest.assigned_table}`}>{guest.table_label ?? "— unassigned —"}</Link>
      </p>

      <div className="stat-grid">
        <div className="stat-tile">
          <div className="label">Satisfaction</div>
          <div className="value"><DagCell table="Guests" field="Satisfaction">{num(guest.satisfaction)}</DagCell></div>
        </div>
        <div className="stat-tile">
          <div className="label">Mood</div>
          <div className="value"><DagCell table="Guests" field="Mood">{guest.mood}</DagCell></div>
        </div>
      </div>

      <h3 style={{ marginTop: 32 }}>Edit</h3>
      <div className="form-grid">
        <label>Full name<input value={v("full_name") ?? ""} onChange={(e) => set("full_name", e.target.value)} /></label>
        <label>Side
          <select value={v("side") ?? ""} onChange={(e) => set("side", e.target.value)}>
            <option value="bride">bride</option>
            <option value="groom">groom</option>
            <option value="both">both</option>
          </select>
        </label>
        <label>Dietary
          <select value={v("dietary") ?? ""} onChange={(e) => set("dietary", e.target.value)}>
            <option value="none">none</option>
            <option value="veg">veg</option>
            <option value="vegan">vegan</option>
            <option value="gf">gluten-free</option>
            <option value="kosher">kosher</option>
          </select>
        </label>
        <label>Language
          <select value={v("language") ?? ""} onChange={(e) => set("language", e.target.value)}>
            <option value="en">en</option>
            <option value="es">es</option>
            <option value="fr">fr</option>
          </select>
        </label>
        <label>Age band
          <select value={v("age_band") ?? ""} onChange={(e) => set("age_band", e.target.value)}>
            <option value="child">child</option>
            <option value="adult">adult</option>
            <option value="senior">senior</option>
          </select>
        </label>
        <label>Assigned table
          <select value={v("assigned_table") ?? ""} onChange={(e) => set("assigned_table", e.target.value)}>
            <option value="">— unassigned —</option>
            {tables?.map((t) => <option key={t.table_id} value={t.table_id}>{t.label}</option>)}
          </select>
        </label>
      </div>
      <div style={{ marginTop: 16 }}>
        <button className="primary" onClick={save} disabled={Object.keys(draft).length === 0}>Save</button>
      </div>
    </div>
  );
}
