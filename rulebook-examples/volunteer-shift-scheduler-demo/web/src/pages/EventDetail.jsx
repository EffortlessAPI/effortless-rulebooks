import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { api } from '../api.js';
import Calc from '../components/Calc.jsx';
import GradeBadge from '../components/GradeBadge.jsx';
import StatusPill from '../components/StatusPill.jsx';
import NewShiftDialog from '../components/NewShiftDialog.jsx';
import NewEventDialog from '../components/NewEventDialog.jsx';

function fmtDate(d) {
  if (!d) return '';
  return new Date(d).toLocaleDateString(undefined, {
    weekday: 'long', month: 'long', day: 'numeric', year: 'numeric',
  });
}
function fmtTime(d) {
  return new Date(d).toLocaleTimeString(undefined, { hour: 'numeric', minute: '2-digit' });
}

export default function EventDetail({ readOnly = false }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const [bundle, setBundle] = useState(null);
  const [showNew, setShowNew] = useState(false);
  const [showEdit, setShowEdit] = useState(false);

  const load = () => api.event(id).then(setBundle);
  useEffect(() => { load(); }, [id]);

  if (!bundle) return <main>Loading…</main>;

  const onDeleteEvent = async () => {
    if (!confirm(`Delete event "${bundle.name}"? This removes all shifts and assignments.`)) return;
    await api.deleteEvent(id);
    navigate('/');
  };

  const c = bundle.computed;

  return (
    <main>
      <div className="crumbs">
        <Link to="/">Events</Link> <span>›</span> <span>{bundle.name}</span>
      </div>

      <div className={`card grade-${c.grade}`}>
        <div className="event-header">
          <div>
            <h2 style={{ marginBottom: 4 }}>{bundle.name}</h2>
            <div className="muted">{fmtDate(bundle.event_date)} · {bundle.location || 'no location'}</div>
            <div className="muted small" style={{ marginTop: 4 }}>
              target {bundle.target_hours_per_volunteer}h per volunteer
            </div>
          </div>
          <div className="big-grade">
            <div style={{ textAlign: 'right' }}>
              <div className="muted small">Staffing grade</div>
              <Calc
                value={<GradeBadge grade={c.grade} />}
                field="grade"
                entity={c}
                entityKind="event"
                entityLabel={bundle.name}
              />
            </div>
          </div>
        </div>

        <div className="event-stats">
          <div className="stat">
            <div className="v">
              <Calc value={`${Math.round(c.coverage_pct * 100)}%`} field="coverage_pct" entity={c} entityKind="event" entityLabel={bundle.name} />
            </div>
            <div className="l">coverage</div>
          </div>
          <div className="stat">
            <div className="v">
              <Calc value={c.total_assigned} field="total_assigned" entity={c} entityKind="event" entityLabel={bundle.name} />
              <span className="muted"> / </span>
              <Calc value={c.total_required} field="total_required" entity={c} entityKind="event" entityLabel={bundle.name} />
            </div>
            <div className="l">assigned / required</div>
          </div>
          <div className="stat">
            <div className="v">
              <Calc value={c.understaffed_shift_count} field="understaffed_shift_count" entity={c} entityKind="event" entityLabel={bundle.name} />
            </div>
            <div className="l">under-staffed shifts</div>
          </div>
          <div className="stat">
            <div className="v">
              <Calc value={c.fully_staffed_shift_count} field="fully_staffed_shift_count" entity={c} entityKind="event" entityLabel={bundle.name} />
            </div>
            <div className="l">fully staffed</div>
          </div>
          <div className="stat">
            <div className="v">
              <Calc value={c.overstaffed_shift_count} field="overstaffed_shift_count" entity={c} entityKind="event" entityLabel={bundle.name} />
            </div>
            <div className="l">over-staffed</div>
          </div>
          <div className="stat">
            <div className="v">
              <Calc value={c.avg_reliability.toFixed(2)} field="avg_reliability" entity={c} entityKind="event" entityLabel={bundle.name} />
            </div>
            <div className="l">avg reliability</div>
          </div>
          <div className="stat">
            <div className="v">
              <Calc value={`${Math.round(c.skill_match_avg * 100)}%`} field="skill_match_avg" entity={c} entityKind="event" entityLabel={bundle.name} />
            </div>
            <div className="l">skill match</div>
          </div>
          <div className="stat">
            <div className="v">{c.volunteer_count}</div>
            <div className="l">volunteers</div>
          </div>
        </div>

        {!readOnly && (
          <div className="actions">
            <button onClick={() => setShowEdit(true)}>Edit event</button>
            <button className="danger" onClick={onDeleteEvent}>Delete event</button>
          </div>
        )}
      </div>

      <div className="toolbar" style={{ marginTop: 24 }}>
        <h3>Shifts</h3>
        {!readOnly && <button className="primary" onClick={() => setShowNew(true)}>+ Add shift</button>}
      </div>
      <div className="shifts-list">
        {bundle.shifts.length === 0 && (
          <div className="card muted" style={{ textAlign: 'center', padding: 20 }}>
            No shifts yet.
          </div>
        )}
        {bundle.shifts.map((s) => {
          const sc = s.computed;
          return (
            <div
              key={s.id}
              className="shift-row"
              onClick={() => navigate(`/events/${id}/shifts/${s.id}`)}
            >
              <div>
                <div className="name">{s.name}</div>
                <div className="small muted">
                  {s.required_skill_name ? `requires ${s.required_skill_name}` : 'no skill required'}
                </div>
              </div>
              <div>
                <div className="small muted">{fmtTime(s.starts_at)} – {fmtTime(s.ends_at)}</div>
                <div className="small muted">
                  <Calc value={`${sc.duration_hours}h`} field="duration_hours" entity={sc} entityKind="shift" entityLabel={s.name} />
                </div>
              </div>
              <div>
                <div className="small muted">assigned</div>
                <div>
                  <Calc value={sc.assigned_count} field="assigned_count" entity={sc} entityKind="shift" entityLabel={s.name} />
                  <span className="muted"> / {s.required_count}</span>
                </div>
              </div>
              <div>
                <div className="small muted">coverage</div>
                <Calc
                  value={<StatusPill status={sc.coverage_status} />}
                  field="coverage_status"
                  entity={sc}
                  entityKind="shift"
                  entityLabel={s.name}
                />
              </div>
              <div>
                <div className="small muted">reliability</div>
                <Calc value={sc.weighted_reliability.toFixed(2)} field="weighted_reliability" entity={sc} entityKind="shift" entityLabel={s.name} />
              </div>
              <div className="small" style={{ color: 'var(--link)' }}>open →</div>
            </div>
          );
        })}
      </div>

      <div style={{ marginTop: 24 }}>
        <h3>Volunteer load</h3>
        <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
          <table className="vols-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Shifts</th>
                <th>Hours</th>
                <th>Load</th>
                <th>Reliability</th>
              </tr>
            </thead>
            <tbody>
              {bundle.volunteer_loads.length === 0 && (
                <tr><td colSpan={5} className="muted" style={{ padding: 16, textAlign: 'center' }}>No volunteers assigned yet.</td></tr>
              )}
              {bundle.volunteer_loads.map((vl) => (
                <tr key={vl.volunteer.id}>
                  <td><strong>{vl.volunteer.name}</strong></td>
                  <td>{vl.shift_count} ({vl.shifts.map(s => s.name).join(', ')})</td>
                  <td>
                    <Calc
                      value={`${vl.assigned_hours}h`}
                      field="assigned_hours"
                      entity={{ id: vl.volunteer.id, explain: vl.explain }}
                      entityKind="volunteer"
                      entityLabel={vl.volunteer.name}
                    />
                  </td>
                  <td>
                    <Calc
                      value={<StatusPill status={vl.load_status} />}
                      field="load_status"
                      entity={{ id: vl.volunteer.id, explain: vl.explain }}
                      entityKind="volunteer"
                      entityLabel={vl.volunteer.name}
                    />
                  </td>
                  <td>{Number(vl.volunteer.reliability_score).toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {showNew && (
        <NewShiftDialog
          eventId={id}
          eventDate={bundle.event_date}
          onClose={() => setShowNew(false)}
          onSaved={() => { setShowNew(false); load(); }}
        />
      )}
      {showEdit && (
        <NewEventDialog
          initial={bundle}
          onClose={() => setShowEdit(false)}
          onCreated={() => { setShowEdit(false); load(); }}
        />
      )}
    </main>
  );
}
