import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { api } from "../lib/api";
import { num, dateOnly } from "../lib/fmt";
import type { Me } from "../App";
import { DagCell } from "../explainer-dag";

export default function SessionDetail({ me }: { me: Me }) {
  const { code } = useParams<{ code: string }>();
  const { data: sessions, reload: rs } = useApi<any[]>(`/api/sessions?`);
  const session = (sessions || []).find((s) => s.sessions_id === code);
  const { data: updates, reload: ru } = useApi<any[]>(`/api/goal-updates?session=${code}`);
  const { data: clientGoals, reload: rcg } = useApi<any[]>(
    session ? `/api/goals?client=${session.client}` : null
  );

  if (!session) return <div>Loading…</div>;
  const canEdit = me.role === "therapist";
  const usedGoalIds = new Set((updates || []).map((u) => u.goal));
  const availableGoals = (clientGoals || []).filter((g) => !usedGoalIds.has(g.goals_id));
  function reloadUpdates() { ru(); rcg(); rs(); }
  return (
    <div>
      <p><Link to="/sessions">← Sessions</Link></p>
      <h1>Session {dateOnly(session.session_label)}</h1>
      <p className="muted">
        <Link to={`/clients/${session.client}`}>{session.client_name}</Link>
      </p>

      <div className="stats">
        <DagCell table="Sessions" field="UpdateCount"><div className="stat"><div className="stat-value">{session.update_count}</div><div className="stat-label">Goal updates</div></div></DagCell>
        <DagCell table="Sessions" field="AvgScoreAchieved"><div className="stat"><div className="stat-value">{num(session.avg_score_achieved)}</div><div className="stat-label">Avg score</div></div></DagCell>
        <DagCell table="Sessions" field="StatusLabel"><div className={`stat ${session.is_productive ? "ok" : "warn"}`}><div className="stat-value">{session.status_label}</div><div className="stat-label">Session type</div></div></DagCell>
      </div>

      {canEdit
        ? <SessionEditor session={session} onSaved={rs} />
        : (
          <>
            <p className="muted">{session.duration_minutes} min · mood {session.mood_rating}/10</p>
            {session.notes && <p>{session.notes}</p>}
          </>
        )}

      <h2>Goal updates this session</h2>
      <table>
        <thead><tr><th>Goal</th><th>Target</th><th>Score (editable)</th><th></th></tr></thead>
        <tbody>
          {(updates || []).map((u) => (
            <UpdateRow key={u.goal_updates_id} u={u} onSaved={reloadUpdates} />
          ))}
          {!updates?.length && <tr><td colSpan={4} className="muted">No goal updates for this session.</td></tr>}
        </tbody>
      </table>

      {canEdit && (
        <AddUpdate
          sessionId={session.sessions_id}
          availableGoals={availableGoals}
          onSaved={reloadUpdates}
        />
      )}
    </div>
  );
}

function SessionEditor({ session, onSaved }: { session: any; onSaved: () => void }) {
  const [mood, setMood] = useState(String(session.mood_rating ?? ""));
  const [duration, setDuration] = useState(String(session.duration_minutes ?? ""));
  const [notes, setNotes] = useState(String(session.notes ?? ""));
  const [saving, setSaving] = useState(false);
  const dirty =
    mood !== String(session.mood_rating ?? "") ||
    duration !== String(session.duration_minutes ?? "") ||
    notes !== String(session.notes ?? "");
  async function save() {
    setSaving(true);
    try {
      await api(`/api/sessions/${session.sessions_id}`, {
        method: "PATCH",
        body: JSON.stringify({
          mood_rating: Number(mood),
          duration_minutes: Number(duration),
          notes,
        }),
      });
      onSaved();
    } finally { setSaving(false); }
  }
  return (
    <div style={{ display: "grid", gap: 8, maxWidth: 480, marginBottom: 16 }}>
      <label>
        Mood (0–10){" "}
        <input type="number" step="1" min={0} max={10} value={mood}
               onChange={(e) => setMood(e.target.value)} style={{ width: 60 }} />
      </label>
      <label>
        Duration (min){" "}
        <input type="number" step="5" min={0} value={duration}
               onChange={(e) => setDuration(e.target.value)} style={{ width: 80 }} />
      </label>
      <label style={{ display: "grid", gap: 4 }}>
        Notes
        <textarea rows={3} value={notes} onChange={(e) => setNotes(e.target.value)} />
      </label>
      <div>
        <button onClick={save} disabled={saving || !dirty}>
          {saving ? "Saving…" : "Save session"}
        </button>
      </div>
    </div>
  );
}

function AddUpdate({ sessionId, availableGoals, onSaved }:
  { sessionId: string; availableGoals: any[]; onSaved: () => void }) {
  const [goal, setGoal] = useState("");
  const [score, setScore] = useState("");
  const [saving, setSaving] = useState(false);
  if (!availableGoals.length) {
    return <p className="muted" style={{ marginTop: 12 }}>All this client's goals already have updates for this session.</p>;
  }
  async function add() {
    if (!goal || score === "") return;
    setSaving(true);
    try {
      await api(`/api/goal-updates`, {
        method: "POST",
        body: JSON.stringify({ goal, session: sessionId, score_achieved: Number(score) }),
      });
      setGoal(""); setScore("");
      onSaved();
    } finally { setSaving(false); }
  }
  return (
    <div style={{ display: "flex", gap: 8, alignItems: "center", marginTop: 12 }}>
      <select value={goal} onChange={(e) => setGoal(e.target.value)}>
        <option value="">Add update for goal…</option>
        {availableGoals.map((g) => (
          <option key={g.goals_id} value={g.goals_id}>{g.title}</option>
        ))}
      </select>
      <input type="number" step="0.5" min={0} max={10} placeholder="score"
             value={score} onChange={(e) => setScore(e.target.value)} style={{ width: 70 }} />
      <button onClick={add} disabled={saving || !goal || score === ""}>
        {saving ? "Adding…" : "Add update"}
      </button>
    </div>
  );
}

function UpdateRow({ u, onSaved }: { u: any; onSaved: () => void }) {
  const [score, setScore] = useState(String(u.score_achieved));
  const [saving, setSaving] = useState(false);
  async function save() {
    setSaving(true);
    try {
      await api(`/api/goal-updates/${u.goal_updates_id}`, {
        method: "PATCH",
        body: JSON.stringify({ score_achieved: Number(score) }),
      });
      onSaved();
    } finally { setSaving(false); }
  }
  return (
    <tr>
      <td>{u.goal_title}</td>
      <td>{num(u.goal_target_score, 1)}</td>
      <td>
        <input type="number" step="0.5" min={0} max={10} value={score}
               onChange={(e) => setScore(e.target.value)} style={{ width: 60 }} />
      </td>
      <td>
        <button onClick={save} disabled={saving || score === String(u.score_achieved)}>
          {saving ? "…" : "Save"}
        </button>
      </td>
    </tr>
  );
}
