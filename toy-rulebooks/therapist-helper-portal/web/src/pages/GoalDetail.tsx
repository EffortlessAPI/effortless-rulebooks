import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useApi } from "../lib/useApi";
import { api } from "../lib/api";
import { num, dateOnly } from "../lib/fmt";
import type { Me } from "../App";
import { DagCell } from "../explainer-dag";

export default function GoalDetail(_: { me: Me }) {
  const { code } = useParams<{ code: string }>();
  const { data: goals, reload: rg } = useApi<any[]>(`/api/goals`);
  const goal = (goals || []).find((g) => g.goals_id === code);
  const { data: updates, reload: ru } = useApi<any[]>(`/api/goal-updates?goal=${code}`);

  if (!goal) return <div>Loading…</div>;
  function reloadAll() { rg(); ru(); }

  return (
    <div>
      <p><Link to="/goals">← Goals</Link></p>
      <h1>{goal.title}</h1>
      <p className="muted">
        <Link to={`/clients/${goal.client}`}>{goal.client_name}</Link>
      </p>

      <div className="stats">
        <DagCell table="Goals" field="AvgScoreAchieved"><Stat label="Avg achieved" value={num(goal.avg_score_achieved)} /></DagCell>
        <DagCell table="Goals" field="LatestScore"><Stat label="Latest" value={num(goal.latest_score, 1)} /></DagCell>
        <DagCell table="Goals" field="RemainingGap"><Stat label="Gap" value={num(goal.remaining_gap)} /></DagCell>
        <DagCell table="Goals" field="ProgressPct"><Stat label="Progress %" value={num(goal.progress_pct, 1)} /></DagCell>
        <DagCell table="Goals" field="IsOnTrack">
          <Stat label="Status" value={goal.is_on_track ? "On track" : "Behind"} tone={goal.is_on_track ? "ok" : "warn"} />
        </DagCell>
      </div>

      <GoalEditor goal={goal} onSaved={reloadAll} />

      <h2>Goal updates</h2>
      <table>
        <thead><tr><th>Session</th><th>Target</th><th>Score (editable)</th><th></th></tr></thead>
        <tbody>
          {(updates || []).map((u) => (
            <UpdateRow key={u.goal_updates_id} u={u} onSaved={reloadAll} />
          ))}
          {!updates?.length && <tr><td colSpan={4} className="muted">No updates for this goal yet.</td></tr>}
        </tbody>
      </table>
    </div>
  );
}

function Stat({ label, value, tone }: { label: string; value: string; tone?: "ok" | "warn" }) {
  return <div className={`stat ${tone || ""}`}>
    <div className="stat-value">{value}</div>
    <div className="stat-label">{label}</div>
  </div>;
}

function GoalEditor({ goal, onSaved }: { goal: any; onSaved: () => void }) {
  const [title, setTitle] = useState(String(goal.title ?? ""));
  const [target, setTarget] = useState(String(goal.target_score ?? ""));
  const [saving, setSaving] = useState(false);
  const dirty =
    title !== String(goal.title ?? "") ||
    target !== String(goal.target_score ?? "");
  async function save() {
    setSaving(true);
    try {
      await api(`/api/goals/${goal.goals_id}`, { method: "PATCH", body: JSON.stringify({ title, target_score: Number(target) }) });
      onSaved();
    } finally { setSaving(false); }
  }
  return (
    <div style={{ display: "grid", gap: 8, maxWidth: 480, margin: "12px 0" }}>
      <label style={{ display: "grid", gap: 4 }}>
        Title
        <input value={title} onChange={(e) => setTitle(e.target.value)} />
      </label>
      <label>
        Target{" "}
        <input type="number" step="0.5" value={target}
               onChange={(e) => setTarget(e.target.value)} style={{ width: 80 }} />
      </label>
      <div>
        <button onClick={save} disabled={saving || !dirty}>
          {saving ? "Saving…" : "Save goal"}
        </button>
      </div>
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
      <td>
        {u.session
          ? <Link to={`/sessions/${u.session}`}>{dateOnly(u.session_label)}</Link>
          : <span className="muted">—</span>}
      </td>
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
