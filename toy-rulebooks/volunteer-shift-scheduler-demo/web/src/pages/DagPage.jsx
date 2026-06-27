import { useEffect, useMemo, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { api } from '../api.js';

// /dag/:table/:field — true inference-graph viewer.
//
// Walks the `depends` chain back to raw columns. Live values come from the
// list endpoints (events, shifts, volunteers) so the user can plug in a
// concrete entity id and see THIS row's actual derivation, not a template.
//
// Layout: leftmost column = raw inputs; rightmost = the target field. Each
// node shows formula + (if a row is bound) the live value.

function nodeKind(node) {
  if (!node) return 'unknown';
  return node.kind || 'raw';
}

// BFS from `start` and build column-by-column layers so the graph renders
// left-to-right (raws at left, target at right).
function buildLayers(graph, startKey) {
  const seen = new Set();
  const depth = {}; // key -> depth (0 = the target)
  const stack = [{ key: startKey, d: 0 }];
  while (stack.length) {
    const { key, d } = stack.pop();
    if (seen.has(key)) {
      depth[key] = Math.max(depth[key] || 0, d);
      continue;
    }
    seen.add(key);
    depth[key] = d;
    const node = graph[key];
    if (!node) continue;
    for (const dep of node.depends || []) {
      stack.push({ key: dep, d: d + 1 });
    }
  }
  // group by depth (largest depth = raw, leftmost)
  const maxD = Math.max(0, ...Object.values(depth));
  const layers = [];
  for (let i = maxD; i >= 0; i--) {
    layers.push(Object.keys(depth).filter((k) => depth[k] === i));
  }
  return layers;
}

function fmtValue(v) {
  if (v === null) return 'null';
  if (v === undefined) return '—';
  if (typeof v === 'number') return Number.isInteger(v) ? String(v) : v.toFixed(3);
  if (typeof v === 'string') return v.length > 60 ? v.slice(0, 60) + '…' : v;
  if (v instanceof Date) return v.toLocaleString();
  if (typeof v === 'object') return JSON.stringify(v).slice(0, 60);
  return String(v);
}

// Pull a per-row value for a given DAG key. For `events.*`, we look in the
// computed payload; for `shifts.*` we look at the corresponding shift; for
// raw `events.X` we look at the column. Returns undefined when no row context.
function resolveValue(key, ctx) {
  const [table, field] = key.split('.');
  if (table === 'events') {
    const ev = ctx.event;
    if (!ev) return undefined;
    if (ev.computed && field in ev.computed) return ev.computed[field];
    if (field in ev) return ev[field];
    return undefined;
  }
  if (table === 'shifts') {
    if (!ctx.event) return undefined;
    // shifts table: show the *count* and a histogram of statuses
    const shifts = ctx.event.shifts || [];
    if (field === 'event_id') return ctx.event.id;
    if (shifts.length === 0) return undefined;
    if (field === 'coverage_status') {
      const counts = shifts.reduce((acc, s) => {
        const k = s.computed?.coverage_status || '?';
        acc[k] = (acc[k] || 0) + 1;
        return acc;
      }, {});
      return Object.entries(counts).map(([k, v]) => `${v}× ${k}`).join(', ');
    }
    if (field === 'required_count') return shifts.reduce((s, x) => s + x.required_count, 0) + ' (sum)';
    if (field === 'assigned_count') return shifts.reduce((s, x) => s + (x.computed?.assigned_count || 0), 0) + ' (sum)';
    if (field === 'duration_hours') return shifts.reduce((s, x) => s + (x.computed?.duration_hours || 0), 0).toFixed(2) + 'h (sum)';
    if (field === 'skill_match_pct') {
      const vals = shifts.map((s) => s.computed?.skill_match_pct).filter((v) => v != null);
      if (!vals.length) return '—';
      return (vals.reduce((a, b) => a + b, 0) / vals.length).toFixed(3) + ' (avg)';
    }
    return `${shifts.length} row(s)`;
  }
  if (table === 'volunteers') {
    return `${(ctx.volunteers || []).length} row(s)`;
  }
  if (table === 'assignments') {
    if (!ctx.event) return undefined;
    const shiftIds = new Set((ctx.event.shifts || []).map((s) => s.id));
    const count = (ctx.assignments || []).filter((a) => shiftIds.has(a.shift_id)).length;
    return `${count} row(s) (this event)`;
  }
  if (table === 'volunteer_skills') {
    return `${(ctx.volunteer_skills || []).length} row(s)`;
  }
  return undefined;
}

export default function DagPage() {
  const { table, field } = useParams();
  const targetKey = `${table}.${field}`;

  const [schema, setSchema] = useState(null);
  const [dag, setDag] = useState(null);
  const [events, setEvents] = useState([]);
  const [volunteers, setVolunteers] = useState([]);
  const [selectedEventId, setSelectedEventId] = useState(null);
  const [event, setEvent] = useState(null);

  useEffect(() => {
    api.schema().then((s) => {
      setSchema(s);
      // Build a graph mirror of what /api/schema exposes + raw column nodes.
      const graph = { ...s.computed };
      for (const [tname, t] of Object.entries(s.tables)) {
        for (const colName of Object.keys(t.columns)) {
          const key = `${tname}.${colName}`;
          if (!graph[key]) {
            graph[key] = {
              kind: 'raw',
              description: t.description,
              formula: null,
              excelFormula: null,
              sqlFormula: null,
              depends: [],
            };
          }
        }
      }
      setDag(graph);
    });
    api.events().then((evs) => {
      setEvents(evs);
      if (evs.length) setSelectedEventId(evs[0].id);
    });
    api.volunteers().then(setVolunteers);
  }, []);

  useEffect(() => {
    if (selectedEventId) {
      api.event(selectedEventId).then(setEvent);
    }
  }, [selectedEventId]);

  const layers = useMemo(() => {
    if (!dag) return null;
    return buildLayers(dag, targetKey);
  }, [dag, targetKey]);

  if (!dag || !layers) return <main>Loading DAG…</main>;
  const targetNode = dag[targetKey];
  if (!targetNode) {
    return (
      <main>
        <div className="card">
          <h2>Unknown field</h2>
          <p>No such field: <code>{targetKey}</code></p>
          <p><Link to="/">← Dashboard</Link></p>
        </div>
      </main>
    );
  }

  const ctx = { event, events, volunteers, assignments: event?.shifts?.flatMap((s) => s.assignments || []) || [], volunteer_skills: [] };

  return (
    <main>
      <div className="crumbs">
        <Link to="/">Events</Link> <span>›</span>
        <span>DAG · {table}.{field}</span>
      </div>

      <div className="card">
        <div className="dag-header">
          <div>
            <h2 style={{ marginBottom: 4 }}>
              <code>{table}.{field}</code>
            </h2>
            <div className="muted">{targetNode.description}</div>
          </div>
          <div className="dag-context">
            <label className="field">
              <span>Bind to event</span>
              <select
                value={selectedEventId || ''}
                onChange={(e) => setSelectedEventId(Number(e.target.value))}
              >
                {events.map((e) => (
                  <option key={e.id} value={e.id}>{e.name}</option>
                ))}
              </select>
            </label>
          </div>
        </div>

        <div className="dag-formula-card">
          <div className="small muted bold">FORMULA OF RECORD</div>
          <div className="dag-formula">{targetNode.formula || '— raw column —'}</div>
          {targetNode.excelFormula && (
            <div className="dag-subformula">
              <span className="muted small">Excel: </span><code>{targetNode.excelFormula}</code>
            </div>
          )}
          {targetNode.sqlFormula && (
            <div className="dag-subformula">
              <span className="muted small">SQL: </span><code>{targetNode.sqlFormula}</code>
            </div>
          )}
          {targetNode.constants && targetNode.constants.length > 0 && (
            <div className="dag-subformula">
              <span className="muted small">Constants: </span>
              {targetNode.constants.map((cname) => (
                <span key={cname} className="chip" style={{ marginLeft: 4 }}>
                  {cname} = {JSON.stringify(schema?.constants?.[cname])}
                </span>
              ))}
            </div>
          )}
        </div>

        <div className="dag-canvas">
          {layers.map((layer, i) => (
            <div key={i} className="dag-layer">
              <div className="dag-layer-label">
                {i === 0 ? 'raw inputs' :
                 i === layers.length - 1 ? 'target' :
                 `tier ${i}`}
              </div>
              {layer.map((key) => {
                const node = dag[key];
                if (!node) return null;
                const kind = nodeKind(node);
                const value = resolveValue(key, ctx);
                const isTarget = key === targetKey;
                return (
                  <div key={key} className={`dag-node dag-node-${kind} ${isTarget ? 'dag-target' : ''}`}>
                    <div className="dag-node-head">
                      <span className={`dag-pill dag-pill-${kind}`}>{kind}</span>
                      <code className="dag-key">{key}</code>
                    </div>
                    {node.formula && (
                      <div className="dag-node-formula">{node.formula}</div>
                    )}
                    {value !== undefined && (
                      <div className="dag-node-value">
                        <span className="muted small">= </span>
                        <strong>{fmtValue(value)}</strong>
                      </div>
                    )}
                    {kind !== 'raw' && (
                      <div className="dag-node-link">
                        <Link to={`/dag/${node.table || key.split('.')[0]}/${node.field || key.split('.')[1]}`}>
                          open →
                        </Link>
                      </div>
                    )}
                  </div>
                );
              })}
            </div>
          ))}
        </div>

        <div className="dag-legend">
          <span className="dag-pill dag-pill-raw">raw</span> raw column
          <span className="dag-pill dag-pill-lookup">lookup</span> cross-table read
          <span className="dag-pill dag-pill-aggregation">aggregation</span> SUM/COUNT/AVG/etc
          <span className="dag-pill dag-pill-calc">calc</span> arithmetic / branching
        </div>
      </div>
    </main>
  );
}
