import { useState } from "react";
import { Link } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { api } from "../lib/api";
import { dateOnly, num } from "../lib/fmt";
import type { Me } from "../App";
import { DagCell } from "../explainer-dag";

export default function Sessions(_: { me: Me }) {
  const { data, reload } = useApi<any[]>("/api/sessions");
  const { data: clients, reload: reloadClients } = useApi<any[]>("/api/clients");
  const list = data || [];
  function reloadAll() { reload(); reloadClients(); }
  return (
    <div>
      <h1>Sessions</h1>
      <AddSession clients={clients || []} onSaved={reloadAll} />
      <table>
        <thead><tr><th>Date</th><th>Client</th><th>Duration (min)</th><th>Mood (1-10)</th><th>Updates</th><th>Avg score</th><th>Productive?</th><th>Notes</th><th></th></tr></thead>
        <tbody>
          {list.map((s) => <SessionRow key={s.sessions_id} session={s} onSaved={reload} />)}
        </tbody>
      </table>
    </div>
  );
}

function AddSession({ clients, onSaved }: { clients: any[]; onSaved: () => void }) {
  const today = new Date().toISOString().slice(0, 10);
  const [client, setClient] = useState("");
  const [date, setDate] = useState(today);
  const [duration, setDuration] = useState("50");
  const [mood, setMood] = useState("7");
  const [notes, setNotes] = useState("");
  const [saving, setSaving] = useState(false);
  async function add() {
    if (!client || !date) return;
    setSaving(true);
    try {
      await api(`/api/sessions`, {
        method: "POST",
        body: JSON.stringify({
          client,
          session_label: date,
          duration_minutes: duration ? Number(duration) : null,
          mood_rating: mood ? Number(mood) : null,
          notes,
        }),
      });
      setNotes("");
      onSaved();
    } finally { setSaving(false); }
  }
  return (
    <div style={{ display: "flex", gap: 8, alignItems: "center", flexWrap: "wrap", margin: "12px 0" }}>
      <select value={client} onChange={(e) => setClient(e.target.value)}>
        <option value="">Client…</option>
        {clients.map((c) => <option key={c.clients_id} value={c.clients_id}>{c.client_name}</option>)}
      </select>
      <input type="date" value={date} onChange={(e) => setDate(e.target.value)} />
      <input type="number" min={1} placeholder="min" value={duration}
             onChange={(e) => setDuration(e.target.value)} style={{ width: 70 }} />
      <input type="number" min={1} max={10} placeholder="mood" value={mood}
             onChange={(e) => setMood(e.target.value)} style={{ width: 60 }} />
      <input type="text" placeholder="notes" value={notes}
             onChange={(e) => setNotes(e.target.value)} style={{ flex: 1, minWidth: 180 }} />
      <button onClick={add} disabled={saving || !client || !date}>
        {saving ? "Adding…" : "Add session"}
      </button>
    </div>
  );
}

function SessionRow({ session, onSaved }: { session: any; onSaved: () => void }) {
  const [mood, setMood] = useState(String(session.mood_rating));
  const [duration, setDuration] = useState(String(session.duration_minutes ?? ""));
  const [notes, setNotes] = useState(String(session.notes ?? ""));
  const [saving, setSaving] = useState(false);
  const dirty =
    mood !== String(session.mood_rating) ||
    duration !== String(session.duration_minutes ?? "") ||
    notes !== String(session.notes ?? "");
  async function save() {
    setSaving(true);
    try {
      const body: any = { mood_rating: Number(mood), notes };
      if (duration !== "") body.duration_minutes = Number(duration);
      await api(`/api/sessions/${session.sessions_id}`, { method: "PATCH", body: JSON.stringify(body) });
      onSaved();
    } finally { setSaving(false); }
  }
  return (
    <tr>
      <td><Link to={`/sessions/${session.sessions_id}`}>{dateOnly(session.session_label)}</Link></td>
      <td><Link to={`/clients/${session.client}`}>{session.client_name}</Link></td>
      <td><input type="number" min={1} value={duration} onChange={(e) => setDuration(e.target.value)} style={{ width: 60 }} /></td>
      <td><input type="number" min={1} max={10} value={mood} onChange={(e) => setMood(e.target.value)} style={{ width: 50 }} /></td>
      <td><DagCell table="Sessions" field="UpdateCount">{session.update_count}</DagCell></td>
      <td><DagCell table="Sessions" field="AvgScoreAchieved">{num(session.avg_score_achieved)}</DagCell></td>
      <td><DagCell table="Sessions" field="StatusLabel">
        <span className={`badge ${session.is_productive ? "ok" : "warn"}`}>{session.status_label}</span>
      </DagCell></td>
      <td><input type="text" value={notes} onChange={(e) => setNotes(e.target.value)} style={{ width: "100%", minWidth: 180 }} /></td>
      <td><button onClick={save} disabled={saving || !dirty}>{saving ? "…" : "Save"}</button></td>
    </tr>
  );
}
