import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { api } from '../api.js';
import GradeBadge from '../components/GradeBadge.jsx';
import Calc from '../components/Calc.jsx';
import NewEventDialog from '../components/NewEventDialog.jsx';

function fmtDate(d) {
  if (!d) return '';
  return new Date(d).toLocaleDateString(undefined, {
    weekday: 'short', month: 'short', day: 'numeric', year: 'numeric',
  });
}

export default function Dashboard({ readOnly = false }) {
  const navigate = useNavigate();
  const [events, setEvents] = useState(null);
  const [showNew, setShowNew] = useState(false);

  const load = () => api.events().then(setEvents);
  useEffect(() => { load(); }, []);

  if (!events) return <main>Loading…</main>;

  return (
    <main>
      <div className="toolbar">
        <div>
          <h2>Events</h2>
          <div className="muted small">
            {events.length} event{events.length === 1 ? '' : 's'} ·
            click a card to drill in · click any{' '}
            <span className="calc" style={{ cursor: 'default' }}>derived</span>{' '}
            value to see how it was computed
          </div>
        </div>
        {!readOnly && (
          <button className="primary" onClick={() => setShowNew(true)}>+ New event</button>
        )}
      </div>

      {events.length === 0 && (
        <div className="card" style={{ textAlign: 'center', padding: 40, color: 'var(--muted)' }}>
          No events yet.
          {!readOnly && <> Click <b>+ New event</b> to start.</>}
        </div>
      )}

      <div className="event-grid">
        {events.map((e) => (
          <div
            key={e.id}
            className={`card event-card grade-${e.computed.grade}`}
            onClick={() => navigate(`/events/${e.id}`)}
          >
            <div className="row1">
              <div>
                <div className="bold" style={{ fontSize: 16 }}>{e.name}</div>
                <div className="meta">{fmtDate(e.event_date)} · {e.location || 'no location'}</div>
              </div>
              <GradeBadge grade={e.computed.grade} />
            </div>
            <div className="stats">
              <div className="stat">
                <div className="v">
                  <Calc
                    value={`${Math.round(e.computed.coverage_pct * 100)}%`}
                    field="coverage_pct"
                    entity={e.computed}
                    entityKind="event"
                    entityLabel={e.name}
                  />
                </div>
                <div className="l">coverage</div>
              </div>
              <div className="stat">
                <div className="v">
                  <Calc
                    value={e.computed.understaffed_shift_count}
                    field="understaffed_shift_count"
                    entity={e.computed}
                    entityKind="event"
                    entityLabel={e.name}
                  />
                </div>
                <div className="l">under-staffed</div>
              </div>
              <div className="stat">
                <div className="v">
                  <Calc
                    value={e.computed.volunteer_count}
                    field="total_assigned"
                    entity={e.computed}
                    entityKind="event"
                    entityLabel={e.name}
                  />
                </div>
                <div className="l">volunteers</div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {showNew && (
        <NewEventDialog
          onClose={() => setShowNew(false)}
          onCreated={() => { setShowNew(false); load(); }}
        />
      )}
    </main>
  );
}
