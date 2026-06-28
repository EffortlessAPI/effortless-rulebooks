import { useEffect, useState } from 'react';
import { api } from '../api.js';

function fmtFull(d) {
  if (!d) return '';
  return new Date(d).toLocaleString(undefined, {
    weekday: 'short', month: 'short', day: 'numeric',
    hour: 'numeric', minute: '2-digit',
  });
}

export default function VolunteerHome({ identity }) {
  const [shifts, setShifts] = useState(null);
  useEffect(() => {
    api.volunteerShifts(identity.id).then(setShifts);
  }, [identity.id]);
  if (!shifts) return <main>Loading…</main>;

  return (
    <main>
      <h2>My Shifts</h2>
      <p className="muted small">
        Hi {identity.label.replace(' (volunteer)', '')} — these are the shifts you're assigned to.
      </p>

      {shifts.length === 0 && (
        <div className="card" style={{ textAlign: 'center', padding: 30, color: 'var(--muted)' }}>
          You aren't assigned to any shifts yet. The coordinator will reach out.
        </div>
      )}

      <div className="shifts-list">
        {shifts.map((s) => (
          <div key={s.id} className="card">
            <div className="row1" style={{ display: 'flex', justifyContent: 'space-between' }}>
              <div>
                <div className="bold">{s.name}</div>
                <div className="muted small">{s.event_name} · {s.event_location || ''}</div>
              </div>
              <div className="small muted" style={{ textAlign: 'right' }}>
                {fmtFull(s.starts_at)}<br />→ {fmtFull(s.ends_at)}
              </div>
            </div>
            {s.required_skill_name && (
              <div className="small muted" style={{ marginTop: 8 }}>
                Skill: {s.required_skill_name}
              </div>
            )}
          </div>
        ))}
      </div>

      <div className="stub" style={{ marginTop: 24 }}>
        <h3 style={{ marginTop: 0 }}>Volunteer view — stub</h3>
        <p>
          A full volunteer view would also let you:
        </p>
        <ul>
          <li>Confirm or decline each shift</li>
          <li>Swap shifts with another volunteer</li>
          <li>Update your availability and skills</li>
          <li>See a calendar export (.ics) of your shifts</li>
        </ul>
        <p className="small">
          Out of scope for the demo — the cascade lives on the coordinator side.
        </p>
      </div>
    </main>
  );
}
