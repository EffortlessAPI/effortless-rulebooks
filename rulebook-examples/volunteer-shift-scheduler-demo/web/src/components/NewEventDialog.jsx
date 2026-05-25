import { useState } from 'react';
import { api } from '../api.js';

export default function NewEventDialog({ onClose, onCreated, initial }) {
  const [name, setName] = useState(initial?.name || '');
  const [location, setLocation] = useState(initial?.location || '');
  const [event_date, setDate] = useState(
    initial?.event_date ? initial.event_date.slice(0, 10) : ''
  );
  const [target, setTarget] = useState(initial?.target_hours_per_volunteer ?? 4);
  const [saving, setSaving] = useState(false);
  const isEdit = !!initial;

  const save = async () => {
    setSaving(true);
    try {
      if (isEdit) {
        await api.updateEvent(initial.id, {
          name, location, event_date, target_hours_per_volunteer: Number(target),
        });
      } else {
        await api.createEvent({
          name, location, event_date, target_hours_per_volunteer: Number(target),
        });
      }
      onCreated();
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="explain-backdrop" onClick={onClose}>
      <div className="explain-card" onClick={(e) => e.stopPropagation()}>
        <header>
          <div className="bold">{isEdit ? 'Edit event' : 'New event'}</div>
          <button className="ghost" onClick={onClose}>✕</button>
        </header>
        <div className="body">
          <label className="field">
            <span>Name</span>
            <input value={name} onChange={(e) => setName(e.target.value)} autoFocus />
          </label>
          <label className="field">
            <span>Location</span>
            <input value={location} onChange={(e) => setLocation(e.target.value)} />
          </label>
          <label className="field">
            <span>Date</span>
            <input type="date" value={event_date} onChange={(e) => setDate(e.target.value)} />
          </label>
          <label className="field">
            <span>Target hours per volunteer</span>
            <input
              type="number"
              min="0"
              step="0.5"
              value={target}
              onChange={(e) => setTarget(e.target.value)}
            />
          </label>
          <div className="actions" style={{ justifyContent: 'flex-end' }}>
            <button onClick={onClose}>Cancel</button>
            <button
              className="primary"
              onClick={save}
              disabled={saving || !name || !event_date}
            >
              {saving ? 'Saving…' : isEdit ? 'Save' : 'Create'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
