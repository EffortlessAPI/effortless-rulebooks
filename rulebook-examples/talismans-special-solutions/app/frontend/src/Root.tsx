import React, { useState, useCallback, useEffect } from "react";
import { Routes, Route, Navigate, useNavigate, useParams, Link } from "react-router-dom";
import App from "./App";
import Login from "./Login";
import { api, setBackend, getBackend, exportXlsx } from "./api";
import { FieldDag, TablePage, TablesIndex, RoutingContext, mergeRouting } from "./explainer-dag";
import "./explainer-dag/dag.css";
import type { ConformanceResponse, ConformanceDiff } from "./types";

// ===========================================================================
// Root — the router around the release console.
//
// Routes:
//   /                       → Login / sync station (the gate)
//   /console        → redirect to /console/flow
//   /console/:view          → the App console (view = flow|graph|closure)
//   /dag/:table/:field      → the Explainer DAG full-screen derivation view
//   *                       → redirect to /
//
// The whole tree is wrapped in <RoutingContext.Provider> so every <DagCell>
// (in the console) and the <FieldDag> page resolve their links/navigation
// against react-router. Explainer-mode ("provenance glyphs on") is global
// state owned by the generated module (lib/dagPrefs) — no provider needed for
// it. The engine bar (which engine is live, the conformance witness, the way
// back to the sync station) rides into App's header via the headerRight slot.
// ===========================================================================

// Bridge the generated explainer-dag module's routing to react-router. Every
// navigational handler the module can ask for is wired EXPLICITLY here — we do
// not lean on the module's internal fallbacks, because those fall back to
// `window.location.hash` URLs (#/, #/dag/...) which a path router (BrowserRouter)
// silently ignores: the hash changes, the route doesn't, and the user bounces to
// the "/" root node. Wiring every handler to react-router keeps every header
// link (Home, the Tables/table breadcrumbs, field glyphs, back) on real paths.
//
// The explainer has its own three-level ontology browser, all under /dag:
//   /dag                → TablesIndex  (every table — the "Tables" crumb target)
//   /dag/:table         → TablePage    (one table's columns — the {table} crumb)
//   /dag/:table/:field  → FieldDag     (one column's full derivation)
// Each level links down to the next, exactly like fields-within-a-table. Home is
// the only action that LEAVES the explainer — back to the release console.
function useDagRouting() {
  const navigate = useNavigate();
  return {
    FieldLink: ({ table, field, className, children }: {
      table: string; field: string; className?: string; children: React.ReactNode;
    }) => (
      <Link to={`/dag/${table}/${field}`} className={className}>{children}</Link>
    ),
    TableLink: ({ table, className, children }: {
      table: string; className?: string; children: React.ReactNode;
    }) => (
      <Link to={`/dag/${table}`} className={className}>{children}</Link>
    ),
    onBack: () => navigate(-1),
    // Home leaves the explainer entirely — back to the console.
    onHome: () => navigate("/console/flow"),
    navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
    // navigateTable("") is the "Tables" index crumb → the TablesIndex page at
    // /dag; a real table goes to its /dag/:table column list.
    navigateTable: (t: string) => navigate(t ? `/dag/${t}` : "/dag"),
  };
}

// The /dag/:table/:field route — reads the params and hands them to the
// generated full-screen derivation page.
function DagRoute() {
  const { table = "", field = "" } = useParams();
  return <FieldDag table={table} field={field} routing={useDagRouting()} />;
}

// The /dag index — the tables landing page (every table in the rulebook). The
// "Tables" breadcrumb links here.
function IndexRoute() {
  return <TablesIndex routing={useDagRouting()} />;
}

// The /dag/:table route — the table's column list. The {table} breadcrumb on a
// field page links here, so this route must exist for that link to resolve to a
// real path instead of dead-ending at "/".
function TableRoute() {
  const { table = "" } = useParams();
  return <TablePage table={table} routing={useDagRouting()} />;
}

// The live conformance chip: runs both engines on the active data and shows
// whether they AGREE. We never hide a mismatch.
function ConformanceChip() {
  const [c, setC] = useState<ConformanceResponse | null>(null);
  const [open, setOpen] = useState(false);
  const [err, setErr] = useState<string | null>(null);

  const refresh = useCallback(() => {
    setErr(null);
    api.conformance().then(setC).catch((e) => setErr((e as Error).message));
  }, []);
  useEffect(() => { refresh(); }, [refresh]);

  if (err) return <span className="chip chip-err" title={err}>conformance: error</span>;
  if (!c) return <span className="chip">checking conformance…</span>;

  const { agree, valueDiffs, representationDiffs, presenceDiffs, identical, fieldsCompared } = c.summary;
  const valueRows: ConformanceDiff[] = (c.diffs || []).filter((d) => d.kind === "value");

  return (
    <span className="conformance">
      <button
        className={`chip ${agree ? "chip-ok" : "chip-warn"}`}
        onClick={() => setOpen((o) => !o)}
        title="Click to see field-by-field differences between the two engines"
        type="button"
      >
        {agree
          ? `✓ engines agree (${identical}/${fieldsCompared})`
          : `⚠ ${valueDiffs} field${valueDiffs === 1 ? "" : "s"} differ`}
      </button>
      {open && (
        <div className="conformance-panel">
          <div className="conformance-head">
            <strong>Reasoner vs Postgres</strong>
            <button onClick={refresh} type="button" className="mini-btn">re-check</button>
          </div>
          <div className="conformance-counts">
            <span>{identical} identical</span>
            <span className="cnt-value">{valueDiffs} value</span>
            <span className="cnt-rep">{representationDiffs} representation</span>
            <span className="cnt-pres">{presenceDiffs} presence</span>
          </div>
          {valueRows.length > 0 && (
            <table className="conformance-table">
              <thead>
                <tr><th>where</th><th>field</th><th>reasoner</th><th>postgres</th></tr>
              </thead>
              <tbody>
                {valueRows.slice(0, 40).map((d, i) => (
                  <tr key={i}>
                    <td>{d.class}.{d.pk}</td>
                    <td>{d.field} <em className="ftype">{d.ftype}</em></td>
                    <td className="v-rs">{String(d.reasoner)}</td>
                    <td className="v-pg">{String(d.postgres)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
          <p className="conformance-note">
            Both engines are generated from the same rulebook. A <strong>value</strong>{" "}
            difference on a calculated/aggregation field is a real bug in one of the
            generated substrates — surfaced, not hidden. Representation/presence
            differences are the same answer in a different shape.
          </p>
        </div>
      )}
    </span>
  );
}

// A download-to-Excel button: exports the CURRENT live DB state as a workbook
// (server reads the vw_* views and runs rulebook-to-xlsx). Shows a brief working
// state and surfaces any error inline rather than swallowing it.
function ExcelButton() {
  const [busy, setBusy] = useState(false);
  const [err, setErr] = useState<string | null>(null);
  const onClick = useCallback(async () => {
    if (busy) return;
    setBusy(true);
    setErr(null);
    try {
      await exportXlsx();
    } catch (e) {
      setErr((e as Error).message);
    } finally {
      setBusy(false);
    }
  }, [busy]);
  return (
    <button
      className={"mini-btn excel-btn" + (busy ? " busy" : "")}
      onClick={onClick}
      type="button"
      disabled={busy}
      title={err || "Export the current data to an Excel workbook (one sheet per table, all calculated columns included)"}
    >
      {busy ? "⏳ exporting…" : "⬇ Excel"}
    </button>
  );
}

// The engine controls, rendered into the top-right corner of App's header via
// the headerRight slot: which engine is live, the live conformance witness, the
// explain-mode toggle, the Excel export, and the way back to the sync station.
function EngineBar({ backend, onLeave }: { backend: string; onLeave: () => void }) {
  return (
    <div className="engine-bar">
      <span className="engine-bar-left">
        <span className="engine-bar-dot" />
        engine: <strong>{backend}</strong>
      </span>
      <span className="engine-bar-right">
        <ExcelButton />
        <ConformanceChip />
        <button className="mini-btn" onClick={onLeave} type="button">
          ⇆ sync / switch engine
        </button>
      </span>
    </div>
  );
}

// The console route — reads the active backend, mounts App keyed on it so a
// engine switch fully remounts (fresh load).
function ConsoleRoute() {
  const navigate = useNavigate();
  const backend = getBackend();
  return (
    <div className="rooted">
      <App
        key={backend}
        headerRight={<EngineBar backend={backend} onLeave={() => navigate("/")} />}
      />
    </div>
  );
}

// The login/sync gate — entering an engine stores the choice and routes to the
// console on that engine.
function LoginRoute() {
  const navigate = useNavigate();
  return (
    <Login
      onEnter={(b: string) => {
        setBackend(b);
        navigate("/console/flow");
      }}
    />
  );
}

export default function Root() {
  // mergeRouting fills the table-level handlers (TableLink / navigateTable /
  // onHome) the richer module wants, deriving them from our field-route wiring
  // so the Provider gets a complete Required<ExplainerDagRouting>.
  const routing = mergeRouting(useDagRouting());
  return (
    <RoutingContext.Provider value={routing}>
      <Routes>
        <Route path="/" element={<LoginRoute />} />
        <Route path="/console" element={<Navigate to="/console/flow" replace />} />
        <Route path="/console/:view" element={<ConsoleRoute />} />
        <Route path="/dag" element={<IndexRoute />} />
        <Route path="/dag/:table" element={<TableRoute />} />
        <Route path="/dag/:table/:field" element={<DagRoute />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </RoutingContext.Provider>
  );
}
