import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { api } from '../api.js';
import Calc from '../components/Calc.jsx';
import StatusPill from '../components/StatusPill.jsx';
import NewShiftDialog from '../components/NewShiftDialog.jsx';

function fmtFull(d) {
  if (!d) return '';
  return new Date(d).toLocaleString(undefined, {
    weekday: 'short', month: 'short', day: 'numeric',
    hour: 'numeric', minute: '2-digit',
  });
}

export default function ShiftDetail({ readOnly = false }) {
  const { eventId, shiftId } = useParams();
  const navigate = useNavigate();
  const [bundle, setBundle] = useState(null);
  const [eventName, setEventName] = useState('');
  const [allVols, setAllVols] = useState([]);
  const [pick, setPick] = useState('');
  const [showEdit, setShowEdit] = useState(false);

  const load = () => api.shift(shiftId).then(setBundle);

  useEffect(() => {
    load();
    api.event(eventId).then((e) => setEventName(e.name));
    api.volunteers().then(setAllVols);
  }, [shiftId, eventId]);

  if (!bundle) return <main>Loading…</main>;
  const c = bundle.computed;

  const assign = async () => {
    if (!pick) return;
    try {
      await api.assign(shiftId, Number(pick));
      setPick('');
      load();
    } catch (e) {
      alert(e.message);
    }
  };

  const unassign = async (assignmentId) => {
    await api.unassign(assignmentId);
    load();
  };

  const onDelete = async () => {
    if (!confirm(`Delete shift "${bundle.name}"?`)) return;
    await api.deleteShift(shiftId);
    navigate(`/events/${eventId}`);
  };

  const assignedIds = new Set(bundle.assignments.map((a) => a.volunteer_id));
  const unassignedVols = allVols.filter((v) => !assignedIds.has(v.id));

  return (
    <main>
      <div className="crumbs">
        <Link to="/">Events</Link> <span>›</span>
        <Link to={`/events/${eventId}`}>{eventName}</Link> <span>›</span>
        <span>{bundle.name}</span>
      </div>

      <div className="card">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', gap: 16 }}>
          <div>
            <h2 style={{ marginBottom: 4 }}>{bundle.name}</h2>
            <div className="muted">
              {fmtFull(bundle.starts_at)} → {fmtFull(bundle.ends_at)}
            </div>
            <div className="muted small" style={{ marginTop: 4 }}>
              requires {bundle.required_count} {bundle.required_count === 1 ? 'volunteer' : 'volunteers'}
              {bundle.required_skill_name ? ` · skill: ${bundle.required_skill_name}` : ' · no skill required'}
            </div>
          </div>
          {!readOnly && (
            <div className="actions">
              <button onClick={() => setShowEdit(true)}>Edit</button>
              <button className="danger" onClick={onDelete}>Delete</button>
            </div>
          )}
        </div>

        <div className="event-stats">
          <div className="stat">
            <div className="v">
              <Calc value={c.assigned_count} field="assigned_count" entity={c} entityKind="shift" entityLabel={bundle.name} />
              <span className="muted"> / {bundle.required_count}</span>
            </div>
            <div className="l">assigned / required</div>
          </div>
          <div className="stat">
            <div className="v">
              <Calc value={<StatusPill status={c.coverage_status} />} field="coverage_status" entity={c} entityKind="shift" entityLabel={bundle.name} />
            </div>
            <div className="l">coverage status</div>
          </div>
          <div className="stat">
            <div className="v">
              <Calc value={`${c.duration_hours}h`} field="duration_hours" entity={c} entityKind="shift" entityLabel={bundle.name} />
            </div>
            <div className="l">duration</div>
          </div>
          <div className="stat">
            <div className="v">
              <Calc value={c.weighted_reliability.toFixed(2)} field="weighted_reliability" entity={c} entityKind="shift" entityLabel={bundle.name} />
            </div>
            <div className="l">reliability</div>
          </div>
          {c.skill_match_pct !== null && (
            <div className="stat">
              <div className="v">
                <Calc value={`${Math.round(c.skill_match_pct * 100)}%`} field="skill_match_pct" entity={c} entityKind="shift" entityLabel={bundle.name} />
              </div>
              <div className="l">skill match</div>
            </div>
          )}
        </div>
      </div>

      <div style={{ marginTop: 24 }}>
        <h3>Assigned volunteers</h3>
        <div className="card">
          {bundle.assignments.length === 0 && (
            <div className="muted" style={{ textAlign: 'center', padding: 12 }}>
              No volunteers assigned yet.
            </div>
          )}
          <div>
            {bundle.assignments.map((a) => (
              <span key={a.assignment_id} className="chip">
                <span>{a.volunteer.name}</span>
                <span className="muted small">
                  · {Number(a.volunteer.reliability_score).toFixed(2)}
                </span>
                {!readOnly && (
                  <button className="x" onClick={() => unassign(a.assignment_id)} title="Remove">×</button>
                )}
              </span>
            ))}
          </div>
          {!readOnly && (
            <div className="row" style={{ marginTop: 14, alignItems: 'end' }}>
              <label className="field" style={{ flex: 1, marginBottom: 0 }}>
                <span>Add volunteer</span>
                <select value={pick} onChange={(e) => setPick(e.target.value)}>
                  <option value="">— pick a volunteer —</option>
                  {unassignedVols.map((v) => {
                    const hasSkill = bundle.required_skill_id == null
                      || v.skills.some((s) => s.id === bundle.required_skill_id);
                    return (
                      <option key={v.id} value={v.id}>
                        {v.name} {hasSkill ? '' : '(no required skill)'} · rel {Number(v.reliability_score).toFixed(2)}
                      </option>
                    );
                  })}
                </select>
              </label>
              <button className="primary" onClick={assign} disabled={!pick}>Assign</button>
            </div>
          )}
        </div>
      </div>

      {showEdit && (
        <NewShiftDialog
          eventId={eventId}
          initial={bundle}
          onClose={() => setShowEdit(false)}
          onSaved={() => { setShowEdit(false); load(); }}
        />
      )}
    </main>
  );
}
