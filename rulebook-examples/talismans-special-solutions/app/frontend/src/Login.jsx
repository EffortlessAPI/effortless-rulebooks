import React, { useEffect, useState, useRef, useCallback } from "react";
import { api, getBackend, setBackend, runControl } from "./api.js";
import SyncPanel from "./SyncPanel.jsx";

// ===========================================================================
// Login / control page — the "sync station".
//
// This is the gate ahead of the release console (App.jsx). It does two jobs:
//
//   1. Pick the execution substrate ("which rules compute the board"): the OWL
//      reasoner or the Postgres views. The choice is stored and attached to
//      every API call as a header — the entire release console is UNCHANGED and
//      unaware of which engine answered.
//
//   2. Drive the rebuild control plane: Reset / Rebuild / Rebuild-from-X. Each
//      shells out to a build on the server and STREAMS its console here, so you
//      watch both engines relock to the rulebook live.
//
// The two engines each own their own data store (Model B): editing on Postgres
// mutates the Postgres tables; editing on the reasoner mutates db.json. A
// "Rebuild from X" round-trips X's data through the rulebook hub and rebuilds
// both, so an edit made in one engine becomes the shared truth.
//
// 2x2-ready: the engine list comes from the server (/api/backends); when the
// off-diagonal cross-runs (same rules, the other DB's data) are exposed there,
// they show up here automatically as extra choices.
// ===========================================================================

export default function Login({ onEnter }) {
  const [backends, setBackends] = useState([]);
  const [chosen, setChosen] = useState(getBackend());
  const [actions, setActions] = useState([]);
  const [log, setLog] = useState([]);
  const [running, setRunning] = useState(null); // action id while a build runs
  const [result, setResult] = useState(null);    // "done" | "error" | null
  const [error, setError] = useState(null);
  const [triKey, setTriKey] = useState(0);        // bump to re-poll the triangle after a build
  const logRef = useRef(null);

  useEffect(() => {
    api.backends().then((b) => setBackends(b.backends || [])).catch((e) => setError(e.message));
    api.controlActions().then((a) => setActions(a.actions || [])).catch(() => {});
  }, []);

  // Auto-scroll the streamed console.
  useEffect(() => {
    if (logRef.current) logRef.current.scrollTop = logRef.current.scrollHeight;
  }, [log]);

  const appendLog = useCallback((line) => setLog((l) => [...l, line]), []);

  const enter = () => {
    setBackend(chosen);
    onEnter(chosen);
  };

  // One streaming runner for everything: the manual named actions AND the
  // SyncPanel's {path, body, label} descriptors. `running` holds the id/label of
  // the active build (truthy = a build is in flight); it streams into the shared
  // console and re-polls the triangle/diff when done.
  const runDescriptor = useCallback(async ({ id, path, body = null, label }) => {
    if (running) return;
    setRunning(id || label);
    setResult(null);
    setError(null);
    setLog([`▶ ${label}`]);
    try {
      await runControl(path, (ev) => {
        if (ev.type === "step") appendLog(`\n— ${ev.label} —`);
        else if (ev.type === "log") appendLog(ev.line);
        else if (ev.type === "done") { appendLog(`\n✓ ${ev.action} complete`); setResult("done"); }
        else if (ev.type === "error") { appendLog(`\n✗ ${ev.error}`); setResult("error"); }
      }, body);
    } catch (e) {
      appendLog(`\n✗ ${e.message}`);
      setResult("error");
    } finally {
      setRunning(null);
      // A build (success OR fail) may have moved the stores — re-poll the
      // triangle + diff so they relock (or re-flag) live.
      setTriKey((k) => k + 1);
    }
  }, [running, appendLog]);

  // Manual named-action buttons (Reset / Rebuild / Rebuild-from-X).
  const runAction = useCallback((action) => {
    runDescriptor({ id: action.id, path: action.path, body: null, label: action.label });
  }, [runDescriptor]);

  // SyncPanel hands us a ready descriptor ({path, body, label}); just run it.
  const runSyncDescriptor = useCallback((desc) => {
    runDescriptor({ id: desc.kind, path: desc.path, body: desc.body, label: desc.label });
  }, [runDescriptor]);

  return (
    <div className="login">
      <div className="login-card">
        <div className="login-head">
          <h1>Talisman's Special Solutions — Release Console</h1>
          <p className="login-sub">
            One rulebook, two execution substrates. Pick which engine computes the
            board, then walk in — the console is identical either way.
          </p>
        </div>

        {error && <div className="login-error">{error}</div>}

        <section className="login-section">
          <h2>1 · Choose the engine</h2>
          <div className="engine-grid">
            {backends.map((b) => (
              <button
                key={b.id}
                className={`engine-choice ${chosen === b.id ? "is-chosen" : ""}`}
                onClick={() => setChosen(b.id)}
                type="button"
              >
                <span className="engine-name">{b.label}</span>
                <span className="engine-id">{b.id}</span>
              </button>
            ))}
          </div>
          <button className="enter-btn" onClick={enter} disabled={!chosen || !!running}>
            Enter the console as <strong>{chosen}</strong> →
          </button>
        </section>

        <section className="login-section">
          <h2>2 · Sync the three stores</h2>
          <p className="login-hint">
            The rulebook is the <strong>head</strong>; the reasoner (db.json) and
            Postgres are its two <strong>legs</strong>. Pick which store is
            authoritative (HEAD) — the most-recently-edited one is suggested, but
            <em> any</em> store can win at any time. The diff shows exactly which
            values each other store would lose; then push HEAD into both, into one
            leg, or up into the hub.
          </p>
          <SyncPanel
            onRunSync={runSyncDescriptor}
            running={running}
            refreshKey={triKey}
          />
        </section>

        <section className="login-section">
          <h2>3 · Manual rebuild</h2>
          <p className="login-hint">
            The explicit named controls behind the sync station. A <em>Rebuild
            from X</em> exports that engine’s rows back into the rulebook and
            rebuilds <em>both</em>. <em>Reset</em> restores the rulebook to its
            committed baseline. Builds run on the server and stream below.
          </p>
          <div className="action-grid">
            {actions.map((a) => (
              <button
                key={a.id}
                className={`action-btn action-${a.id}`}
                onClick={() => runAction(a)}
                disabled={!!running}
                type="button"
                title={a.desc}
              >
                <span className="action-label">{a.label}</span>
                <span className="action-desc">{a.desc}</span>
              </button>
            ))}
          </div>
        </section>

        {(log.length > 0 || running) && (
          <section className="login-section">
            <h2>
              Build console
              {running && <span className="running-dot"> · running {running}…</span>}
              {result === "done" && <span className="ok-dot"> · done</span>}
              {result === "error" && <span className="err-dot"> · failed</span>}
            </h2>
            <pre className="build-console" ref={logRef}>
              {log.join("\n")}
            </pre>
          </section>
        )}
      </div>
    </div>
  );
}
