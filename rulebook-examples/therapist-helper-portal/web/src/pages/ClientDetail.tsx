import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { api } from "../lib/api";
import { num, dateOnly } from "../lib/fmt";
import type { Me } from "../App";
import { DagCell } from "../explainer-dag";

export default function ClientDetail(_: { me: Me }) {
  const { code } = useParams<{ code: string }>();
  const { data: client, reload: reloadC } = useApi<any>(`/api/clients/${code}`);
  const { data: goals, reload: reloadG } = useApi<any[]>(`/api/goals?client=${code}`);
  const { data: sessions, reload: reloadS } = useApi<any[]>(`/api/sessions?client=${code}`);

  function reloadAll() { reloadC(); reloadG(); reloadS(); }

  if (!client) return <div>Loading…</div>;
  return (
    <div>
      <p><Link to="/clients">← Clients</Link></p>
      <h1>{client.client_name}</h1>
      <p className="muted">Therapist: {client.therapist_name || client.therapist} · last session {dateOnly(client.last_session_label)}</p>
      <div className="stats">
        <DagCell table="Clients" field="SessionCount"><Stat label="Sessions" value={String(client.session_count)} /></DagCell>
        <DagCell table="Clients" field="AvgMoodRating"><Stat label="Avg mood" value={num(client.avg_mood_rating)} /></DagCell>
        <DagCell table="Clients" field="AvgGoalProgress"><Stat label="Goal progress %" value={num(client.avg_goal_progress, 1)} /></DagCell>
        <DagCell table="Clients" field="StatusLabel"><Stat label="Status" value={client.status_label} tone={client.is_at_risk ? "warn" : "ok"} /></DagCell>
      </div>

      <h2>Goals</h2>
      <table>
        <thead><tr><th>Title</th><th>Target</th><th>Avg achieved</th><th>Latest</th><th>Gap</th><th>Progress %</th><th>Status</th><th></th></tr></thead>
        <tbody>
          {(goals || []).map((g) => (
            <GoalRow key={g.goals_id} goal={g} onSaved={reloadAll} />
          ))}
        </tbody>
      </table>
      <AddGoalForClient clientId={client.clients_id} onSaved={reloadAll} />

      <h2>Sessions</h2>
      <table>
        <thead><tr><th>Date</th><th>Duration</th><th>Mood (editable)</th><th>Updates</th><th>Avg score</th><th>Productive?</th><th>Notes</th><th></th></tr></thead>
        <tbody>
          {(sessions || []).map((s) => (
            <SessionRow key={s.sessions_id} session={s} onSaved={reloadAll} />
          ))}
        </tbody>
      </table>
      <AddSessionForClient clientId={client.clients_id} onSaved={reloadAll} />
    </div>
  );
}

function AddGoalForClient({ clientId, onSaved }: { clientId: string; onSaved: () => void }) {
  const [title, setTitle] = useState("");
  const [target, setTarget] = useState("7");
  const [saving, setSaving] = useState(false);
  async function add() {
    if (!title) return;
    setSaving(true);
    try {
      await api(`/api/goals`, {
        method: "POST",
        body: JSON.stringify({
          client: clientId, title,
          target_score: target ? Number(target) : null,
        }),
      });
      setTitle("");
      onSaved();
    } finally { setSaving(false); }
  }
  return (
    <div style={{ display: "flex", gap: 8, alignItems: "center", margin: "8px 0 16px" }}>
      <input type="text" placeholder="New goal title" value={title}
             onChange={(e) => setTitle(e.target.value)} style={{ flex: 1, minWidth: 200 }} />
      <input type="number" step="0.5" placeholder="target" value={target}
             onChange={(e) => setTarget(e.target.value)} style={{ width: 70 }} />
      <button onClick={add} disabled={saving || !title}>{saving ? "Adding…" : "Add goal"}</button>
    </div>
  );
}

function AddSessionForClient({ clientId, onSaved }: { clientId: string; onSaved: () => void }) {
  const today = new Date().toISOString().slice(0, 10);
  const [date, setDate] = useState(today);
  const [duration, setDuration] = useState("50");
  const [mood, setMood] = useState("7");
  const [notes, setNotes] = useState("");
  const [saving, setSaving] = useState(false);
  async function add() {
    if (!date) return;
    setSaving(true);
    try {
      await api(`/api/sessions`, {
        method: "POST",
        body: JSON.stringify({
          client: clientId,
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
    <div style={{ display: "flex", gap: 8, alignItems: "center", flexWrap: "wrap", margin: "8px 0 16px" }}>
      <input type="date" value={date} onChange={(e) => setDate(e.target.value)} />
      <input type="number" min={1} placeholder="min" value={duration}
             onChange={(e) => setDuration(e.target.value)} style={{ width: 70 }} />
      <input type="number" min={1} max={10} placeholder="mood" value={mood}
             onChange={(e) => setMood(e.target.value)} style={{ width: 60 }} />
      <input type="text" placeholder="notes" value={notes}
             onChange={(e) => setNotes(e.target.value)} style={{ flex: 1, minWidth: 180 }} />
      <button onClick={add} disabled={saving || !date}>{saving ? "Adding…" : "Add session"}</button>
    </div>
  );
}

function Stat({ label, value, tone }: { label: string; value: string; tone?: "ok" | "warn" }) {
  return <div className={`stat ${tone || ""}`}>
    <div className="stat-value">{value}</div>
    <div className="stat-label">{label}</div>
  </div>;
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

function SessionRow({ session, onSaved }: { session: any; onSaved: () => void }) {
  const [mood, setMood] = useState(String(session.mood_rating));
  const [saving, setSaving] = useState(false);
  async function save() {
    setSaving(true);
    try {
      await api(`/api/sessions/${session.sessions_id}`, { method: "PATCH", body: JSON.stringify({ mood_rating: Number(mood) }) });
      onSaved();
    } finally { setSaving(false); }
  }
  return (
    <tr>
      <td><Link to={`/sessions/${session.sessions_id}`}>{dateOnly(session.session_label)}</Link></td>
      <td>{session.duration_minutes} min</td>
      <td><input type="number" min={1} max={10} value={mood} onChange={(e) => setMood(e.target.value)} style={{ width: 50 }} /></td>
      <td><DagCell table="Sessions" field="UpdateCount">{session.update_count}</DagCell></td>
      <td><DagCell table="Sessions" field="AvgScoreAchieved">{num(session.avg_score_achieved)}</DagCell></td>
      <td><DagCell table="Sessions" field="StatusLabel">
        <span className={`badge ${session.is_productive ? "ok" : "warn"}`}>{session.status_label}</span>
      </DagCell></td>
      <td className="notes">{session.notes}</td>
      <td><button onClick={save} disabled={saving || mood === String(session.mood_rating)}>{saving ? "…" : "Save"}</button></td>
    </tr>
  );
}
