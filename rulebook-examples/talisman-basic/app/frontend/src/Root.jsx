import React, { useState, useCallback, useEffect } from "react";
import App from "./App.jsx";
import Login from "./Login.jsx";
import { api, getBackend, setBackend } from "./api.js";

// ===========================================================================
// Root — the gate around the (unchanged) release console.
//
// Until you "enter" from the login page, we show Login. After entering we show
// a slim engine bar (which engine is live, the live conformance witness, a way
// back to the sync station) ABOVE the unmodified <App/>. App.jsx and every view
// are untouched — they render whatever the active engine computed. Switching
// engines is just changing the stored backend + remounting App.
// ===========================================================================

// The live conformance chip: runs both engines on the active data and shows
// whether they AGREE. This is the headline — a backend toggle that proves (or
// disproves) the two generated substrates compute the same answers, and links
// to exactly which fields differ. We never hide a mismatch.
function ConformanceChip() {
  const [c, setC] = useState(null);
  const [open, setOpen] = useState(false);
  const [err, setErr] = useState(null);

  const refresh = useCallback(() => {
    setErr(null);
    api.conformance().then(setC).catch((e) => setErr(e.message));
  }, []);
  useEffect(() => { refresh(); }, [refresh]);

  if (err) return <span className="chip chip-err" title={err}>conformance: error</span>;
  if (!c) return <span className="chip">checking conformance…</span>;

  const { agree, valueDiffs, representationDiffs, presenceDiffs, identical, fieldsCompared } = c.summary;
  const valueRows = (c.diffs || []).filter((d) => d.kind === "value");

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

function EngineBar({ backend, onLeave }) {
  return (
    <div className="engine-bar">
      <span className="engine-bar-left">
        <span className="engine-bar-dot" />
        engine: <strong>{backend}</strong>
      </span>
      <span className="engine-bar-right">
        <ConformanceChip />
        <button className="mini-btn" onClick={onLeave} type="button">
          ⇆ sync / switch engine
        </button>
      </span>
    </div>
  );
}

export default function Root() {
  // `entered` gates the console; `backend` is the active engine. A remount key
  // forces App to fully reload when the engine changes.
  const [entered, setEntered] = useState(false);
  const [backend, setBackendState] = useState(getBackend());

  if (!entered) {
    return (
      <Login
        onEnter={(b) => {
          setBackend(b);
          setBackendState(b);
          setEntered(true);
        }}
      />
    );
  }

  return (
    <div className="rooted">
      <EngineBar backend={backend} onLeave={() => setEntered(false)} />
      {/* key on backend so App fully remounts (fresh load) when the engine changes */}
      <App key={backend} />
    </div>
  );
}
