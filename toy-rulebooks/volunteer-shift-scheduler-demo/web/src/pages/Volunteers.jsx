import { useEffect, useState } from 'react';
import { api } from '../api.js';

function VolunteerForm({ initial, skills, onClose, onSaved }) {
  const [name, setName] = useState(initial?.name || '');
  const [email, setEmail] = useState(initial?.email || '');
  const [reliability_score, setRel] = useState(initial?.reliability_score ?? 0.85);
  const [max_hours, setMax] = useState(initial?.max_hours ?? 8);
  const [skill_ids, setSkillIds] = useState(initial?.skills?.map((s) => s.id) || []);
  const [saving, setSaving] = useState(false);
  const isEdit = !!initial;

  const toggleSkill = (id) =>
    setSkillIds((cur) => (cur.includes(id) ? cur.filter((x) => x !== id) : [...cur, id]));

  const save = async () => {
    setSaving(true);
    try {
      const payload = {
        name, email,
        reliability_score: Number(reliability_score),
        max_hours: Number(max_hours),
        skill_ids,
      };
      if (isEdit) await api.updateVolunteer(initial.id, payload);
      else await api.createVolunteer(payload);
      onSaved();
    } catch (e) {
      alert(e.message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="explain-backdrop" onClick={onClose}>
      <div className="explain-card" onClick={(e) => e.stopPropagation()}>
        <header>
          <div className="bold">{isEdit ? 'Edit volunteer' : 'New volunteer'}</div>
          <button className="ghost" onClick={onClose}>✕</button>
        </header>
        <div className="body">
          <label className="field">
            <span>Name</span>
            <input value={name} onChange={(e) => setName(e.target.value)} autoFocus />
          </label>
          <label className="field">
            <span>Email</span>
            <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
          </label>
          <div className="row" style={{ gap: 12 }}>
            <label className="field" style={{ flex: 1 }}>
              <span>Reliability (0–1)</span>
              <input type="number" min="0" max="1" step="0.01" value={reliability_score} onChange={(e) => setRel(e.target.value)} />
            </label>
            <label className="field" style={{ flex: 1 }}>
              <span>Max hours</span>
              <input type="number" min="0" step="0.5" value={max_hours} onChange={(e) => setMax(e.target.value)} />
            </label>
          </div>
          <div className="field">
            <span>Skills</span>
            <div className="skill-pick">
              {skills.map((s) => (
                <label key={s.id}>
                  <input
                    type="checkbox"
                    checked={skill_ids.includes(s.id)}
                    onChange={() => toggleSkill(s.id)}
                  />
                  {s.name}
                </label>
              ))}
            </div>
          </div>
          <div className="actions" style={{ justifyContent: 'flex-end' }}>
            <button onClick={onClose}>Cancel</button>
            <button className="primary" onClick={save} disabled={saving || !name || !email}>
              {saving ? 'Saving…' : isEdit ? 'Save' : 'Create'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default function Volunteers() {
  const [vols, setVols] = useState(null);
  const [skills, setSkills] = useState([]);
  const [edit, setEdit] = useState(null);
  const [creating, setCreating] = useState(false);

  const load = () => api.volunteers().then(setVols);
  useEffect(() => {
    load();
    api.skills().then(setSkills);
  }, []);

  if (!vols) return <main>Loading…</main>;

  const onDelete = async (v) => {
    if (!confirm(`Delete ${v.name}? This removes their assignments.`)) return;
    await api.deleteVolunteer(v.id);
    load();
  };

  return (
    <main>
      <div className="toolbar">
        <h2>Volunteers</h2>
        <button className="primary" onClick={() => setCreating(true)}>+ New volunteer</button>
      </div>
      <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
        <table className="vols-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Reliability</th>
              <th>Max hours</th>
              <th>Skills</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {vols.map((v) => (
              <tr key={v.id}>
                <td><strong>{v.name}</strong></td>
                <td className="muted">{v.email}</td>
                <td>{Number(v.reliability_score).toFixed(2)}</td>
                <td>{Number(v.max_hours).toFixed(1)}</td>
                <td className="small">
                  {v.skills.map((s) => s.name).join(', ') || <span className="muted">—</span>}
                </td>
                <td>
                  <div className="actions" style={{ justifyContent: 'flex-end' }}>
                    <button onClick={() => setEdit(v)}>Edit</button>
                    <button className="danger" onClick={() => onDelete(v)}>Delete</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {creating && (
        <VolunteerForm
          skills={skills}
          onClose={() => setCreating(false)}
          onSaved={() => { setCreating(false); load(); }}
        />
      )}
      {edit && (
        <VolunteerForm
          initial={edit}
          skills={skills}
          onClose={() => setEdit(null)}
          onSaved={() => { setEdit(null); load(); }}
        />
      )}
    </main>
  );
}
