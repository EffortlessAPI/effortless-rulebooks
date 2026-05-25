import { useEffect, useState } from 'react';
import { Navigate, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import { api } from './api.js';
import { getIdentity } from './auth.js';
import Header from './components/Header.jsx';
import { ExplainProvider } from './components/ExplainContext.jsx';
import Login from './pages/Login.jsx';
import Dashboard from './pages/Dashboard.jsx';
import EventDetail from './pages/EventDetail.jsx';
import ShiftDetail from './pages/ShiftDetail.jsx';
import Volunteers from './pages/Volunteers.jsx';
import VolunteerHome from './pages/VolunteerHome.jsx';
import ViewerStub from './pages/ViewerStub.jsx';
import DagPage from './pages/DagPage.jsx';
import RulesPage from './pages/RulesPage.jsx';

// resolveRef: parses 'kind:id[:field]' and returns an explain frame the
// overlay can render, so the user can drill upstream into the DAG.
async function resolveRef(ref) {
  const m = ref.match(/^([a-z]+):(\d+)(?::([a-z0-9_]+))?/i);
  if (!m) return null;
  const [, kind, idStr, field] = m;
  const id = Number(idStr);

  if (kind === 'event') {
    const ev = await api.event(id);
    const f = field || 'grade';
    if (!ev.computed.explain[f]) return null;
    return {
      title: `${ev.name} · ${f}`,
      kind: 'event',
      id,
      field: f,
      explain: ev.computed.explain[f],
    };
  }
  if (kind === 'shift') {
    const sh = await api.shift(id);
    const f = field || 'coverage_status';
    if (!sh.computed.explain[f]) return null;
    return {
      title: `${sh.name} · ${f}`,
      kind: 'shift',
      id,
      field: f,
      explain: sh.computed.explain[f],
    };
  }
  if (kind === 'volunteer') {
    const all = await api.volunteers();
    const v = all.find((x) => x.id === id);
    if (!v) return null;
    // Raw / non-derived values: synthesize an "explain" that just states the source.
    if (field === 'reliability_score') {
      return {
        title: `${v.name} · reliability_score`,
        kind: 'volunteer',
        id,
        field,
        explain: {
          formula: 'raw value from volunteers.reliability_score (set by coordinator)',
          inputs: [
            { label: 'volunteers.reliability_score', value: Number(v.reliability_score) },
          ],
          result: Number(v.reliability_score),
        },
      };
    }
    return {
      title: `${v.name}`,
      kind: 'volunteer',
      id,
      field: field || 'profile',
      explain: {
        formula: 'volunteer profile (raw fields)',
        inputs: [
          { label: 'name', value: v.name },
          { label: 'email', value: v.email },
          { label: 'reliability_score', value: Number(v.reliability_score) },
          { label: 'max_hours', value: Number(v.max_hours) },
          { label: 'skills', value: v.skills.map((s) => s.name).join(', ') || '—' },
        ],
        result: v.name,
      },
    };
  }
  return null;
}

export default function App() {
  const [identity, setIdent] = useState(() => getIdentity());
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const onStorage = () => setIdent(getIdentity());
    window.addEventListener('storage', onStorage);
    return () => window.removeEventListener('storage', onStorage);
  }, []);

  // Re-read identity each route change so logout/switch reflects immediately.
  useEffect(() => { setIdent(getIdentity()); }, [location.pathname]);

  if (!identity && location.pathname !== '/login') {
    return <Navigate to="/login" replace />;
  }

  return (
    <ExplainProvider resolveRef={resolveRef}>
      {identity && <Header identity={identity} />}
      <Routes>
        <Route path="/login" element={<Login />} />

        {/* Coordinator routes */}
        {identity?.kind === 'coordinator' && (
          <>
            <Route path="/" element={<Dashboard />} />
            <Route path="/events/:id" element={<EventDetail />} />
            <Route path="/events/:eventId/shifts/:shiftId" element={<ShiftDetail />} />
            <Route path="/volunteers" element={<Volunteers />} />
            <Route path="/dag/:table/:field" element={<DagPage />} />
            <Route path="/rules" element={<RulesPage />} />
          </>
        )}

        {/* Viewer (read-only) routes */}
        {identity?.kind === 'viewer' && (
          <>
            <Route
              path="/"
              element={
                <>
                  <Dashboard readOnly />
                  <main><ViewerStub /></main>
                </>
              }
            />
            <Route path="/events/:id" element={<EventDetail readOnly />} />
            <Route path="/events/:eventId/shifts/:shiftId" element={<ShiftDetail readOnly />} />
            <Route path="/dag/:table/:field" element={<DagPage />} />
            <Route path="/rules" element={<RulesPage />} />
          </>
        )}

        {/* Volunteer routes */}
        {identity?.kind === 'volunteer' && (
          <>
            <Route path="/my" element={<VolunteerHome identity={identity} />} />
            <Route path="/" element={<Navigate to="/my" replace />} />
          </>
        )}

        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </ExplainProvider>
  );
}
