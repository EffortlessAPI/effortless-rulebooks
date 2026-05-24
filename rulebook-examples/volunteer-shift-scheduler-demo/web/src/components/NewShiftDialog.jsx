import { useEffect, useState } from 'react';
import { api } from '../api.js';

function toLocalInput(d) {
  if (!d) return '';
  const dt = new Date(d);
  const pad = (n) => String(n).padStart(2, '0');
  return `${dt.getFullYear()}-${pad(dt.getMonth() + 1)}-${pad(dt.getDate())}T${pad(dt.getHours())}:${pad(dt.getMinutes())}`;
}

export default function NewShiftDialog({ eventId, eventDate, onClose, onSaved, initial }) {
  const [skills, setSkills] = useState([]);
  const [name, setName] = useState(initial?.name || '');
  const [starts_at, setStarts] = useState(
    initial ? toLocalInput(initial.starts_at) : `${eventDate?.slice(0, 10) || ''}T09:00`
  );
  const [ends_at, setEnds] = useState(
    initial ? toLocalInput(initial.ends_at) : `${eventDate?.slice(0, 10) || ''}T12:00`
  );
  const [required_count, setReq] = useState(initial?.required_count ?? 2);
  const [required_skill_id, setSkill] = useState(initial?.required_skill_id ?? '');
  const [saving, setSaving] = useState(false);
  const isEdit = !!initial;

  useEffect(() => { api.skills().then(setSkills); }, []);

  const save = async () => {
    setSaving(true);
    try {
      const payload = {
        name,
        starts_at,
        ends_at,
        required_count: Number(required_count),
        required_skill_id: required_skill_id ? Number(required_skill_id) : null,
      };
      if (isEdit) await api.updateShift(initial.id, payload);
      else await api.createShift(eventId, payload);
      onSaved();
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="explain-backdrop" onClick={onClose}>
      <div className="explain-card" onClick={(e) => e.stopPropagation()}>
        <header>
          <div className="bold">{isEdit ? 'Edit shift' : 'New shift'}</div>
          <button className="ghost" onClick={onClose}>✕</button>
        </header>
        <div className="body">
          <label className="field">
            <span>Name</span>
            <input value={name} onChange={(e) => setName(e.target.value)} autoFocus />
          </label>
          <div className="row" style={{ gap: 12 }}>
            <label className="field" style={{ flex: 1 }}>
              <span>Starts</span>
              <input type="datetime-local" value={starts_at} onChange={(e) => setStarts(e.target.value)} />
            </label>
            <label className="field" style={{ flex: 1 }}>
              <span>Ends</span>
              <input type="datetime-local" value={ends_at} onChange={(e) => setEnds(e.target.value)} />
            </label>
          </div>
          <div className="row" style={{ gap: 12 }}>
            <label className="field" style={{ flex: 1 }}>
              <span>Required count</span>
              <input type="number" min="0" value={required_count} onChange={(e) => setReq(e.target.value)} />
            </label>
            <label className="field" style={{ flex: 1 }}>
              <span>Required skill</span>
              <select value={required_skill_id} onChange={(e) => setSkill(e.target.value)}>
                <option value="">— none —</option>
                {skills.map((s) => (
                  <option key={s.id} value={s.id}>{s.name}</option>
                ))}
              </select>
            </label>
          </div>
          <div className="actions" style={{ justifyContent: 'flex-end' }}>
            <button onClick={onClose}>Cancel</button>
            <button className="primary" onClick={save} disabled={saving || !name || !starts_at || !ends_at}>
              {saving ? 'Saving…' : isEdit ? 'Save' : 'Create'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
