import { useState } from "react";
import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { api } from "../lib/api";
import { num } from "../lib/fmt";
import type { Me } from "../App";
import { DagCell } from "../explainer-dag";

export default function Goals(_: { me: Me }) {
  const { data, reload } = useApi<any[]>("/api/goals");
  const { data: clients } = useApi<any[]>("/api/clients");
  const list = data || [];
  return (
    <div>
      <h1>Goals</h1>
      <AddGoal clients={clients || []} onSaved={reload} />
      <table>
        <thead><tr><th>Title</th><th>Client</th><th>Target</th><th>Avg</th><th>Latest</th><th>Gap</th><th>Progress %</th><th>Status</th><th></th></tr></thead>
        <tbody>
          {list.map((g) => <GoalRow key={g.goals_id} goal={g} onSaved={reload} />)}
        </tbody>
      </table>
    </div>
  );
}

function AddGoal({ clients, onSaved }: { clients: any[]; onSaved: () => void }) {
  const [client, setClient] = useState("");
  const [title, setTitle] = useState("");
  const [target, setTarget] = useState("7");
  const [saving, setSaving] = useState(false);
  async function add() {
    if (!client || !title) return;
    setSaving(true);
    try {
      await api(`/api/goals`, {
        method: "POST",
        body: JSON.stringify({
          client, title,
          target_score: target ? Number(target) : null,
        }),
      });
      setTitle("");
      onSaved();
    } finally { setSaving(false); }
  }
  return (
    <div style={{ display: "flex", gap: 8, alignItems: "center", flexWrap: "wrap", margin: "12px 0" }}>
      <select value={client} onChange={(e) => setClient(e.target.value)}>
        <option value="">Client…</option>
        {clients.map((c) => <option key={c.clients_id} value={c.clients_id}>{c.client_name}</option>)}
      </select>
      <input type="text" placeholder="Title" value={title}
             onChange={(e) => setTitle(e.target.value)} style={{ flex: 1, minWidth: 180 }} />
      <input type="number" step="0.5" placeholder="target" value={target}
             onChange={(e) => setTarget(e.target.value)} style={{ width: 70 }} />
      <button onClick={add} disabled={saving || !client || !title}>
        {saving ? "Adding…" : "Add goal"}
      </button>
    </div>
  );
}

function GoalRow({ goal, onSaved }: { goal: any; onSaved: () => void }) {
  const [target, setTarget] = useState(String(goal.target_score));
  const [saving, setSaving] = useState(false);
  const dirty = target !== String(goal.target_score);
  async function save() {
    setSaving(true);
    try {
      await api(`/api/goals/${goal.goals_id}`, { method: "PATCH", body: JSON.stringify({ target_score: Number(target) }) });
      onSaved();
    } finally { setSaving(false); }
  }
  return (
    <tr>
      <td><Link to={`/goals/${goal.goals_id}`}>{goal.title}</Link></td>
      <td><Link to={`/clients/${goal.client}`}>{goal.client_name}</Link></td>
      <td><input type="number" step="0.5" value={target} onChange={(e) => setTarget(e.target.value)} style={{ width: 60 }} /></td>
      <td><DagCell table="Goals" field="AvgScoreAchieved">{num(goal.avg_score_achieved)}</DagCell></td>
      <td><DagCell table="Goals" field="LatestScore">{num(goal.latest_score, 1)}</DagCell></td>
      <td><DagCell table="Goals" field="RemainingGap">{num(goal.remaining_gap)}</DagCell></td>
      <td><DagCell table="Goals" field="ProgressPct">{num(goal.progress_pct, 1)}</DagCell></td>
      <td><DagCell table="Goals" field="IsOnTrack">{goal.is_on_track
        ? <span className="badge ok">On track</span>
        : <span className="badge warn">Behind</span>}</DagCell></td>
      <td><button onClick={save} disabled={saving || !dirty}>{saving ? "…" : "Save"}</button></td>
    </tr>
  );
}
