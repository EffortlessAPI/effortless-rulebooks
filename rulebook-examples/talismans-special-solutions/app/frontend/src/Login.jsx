import React, { useEffect, useState, useRef, useCallback } from "react";
import { api, setBackend, runControl } from "./api.js";
import SyncPanel from "./SyncPanel.jsx";

// ===========================================================================
// Login / control page — now ONE interactive control.
//
// The whole gate is the Head & Legs triangle (SyncPanel). Everything that used
// to be three stacked sections — pick-engine buttons, HEAD chips, the diff list,
// the directional sync buttons, AND the manual rebuild buttons — is folded into
// the picture:
//
//   • Click a node  → that store becomes HEAD (styling shows it).
//   • A floaty box pops next to the focused node with the field-level diff, the
//     directional pushes (◂ left leg / ▸ right leg / ▾ both / ▴ up into the hub),
//     a quarantined "reset-then-rebuild" (the git-reset-then-down 4th choice),
//     and — for an engine leg — a "Launch console on X →" button.
//
// Clicking either engine leg snaps everything back in line with that engine as
// HEAD; clicking the rulebook makes the hub authoritative. The build still runs
// on the server and streams into the console below this control. App.jsx and the
// views are untouched — they render whatever the chosen engine computed.
// ===========================================================================

export default function Login({ onEnter }) {
  const [backends, setBackends] = useState([]);
  const [log, setLog] = useState([]);
  const [running, setRunning] = useState(null); // action id/label while a build runs
  const [result, setResult] = useState(null);    // "done" | "error" | null
  const [error, setError] = useState(null);
  const [triKey, setTriKey] = useState(0);        // bump to re-poll the triangle after a build
  const logRef = useRef(null);

  useEffect(() => {
    api.backends().then((b) => setBackends(b.backends || [])).catch((e) => setError(e.message));
  }, []);

  // Auto-scroll the streamed console.
  useEffect(() => {
    if (logRef.current) logRef.current.scrollTop = logRef.current.scrollHeight;
  }, [log]);

  const appendLog = useCallback((line) => setLog((l) => [...l, line]), []);

  // Launch the console on the chosen engine (handed up from the floaty box).
  const enter = useCallback((backendId) => {
    setBackend(backendId);
    onEnter(backendId);
  }, [onEnter]);

  // One streaming runner for every build descriptor the SyncPanel hands us
  // ({path, body, label}). `running` holds the active label (truthy = a build is
  // in flight); it streams into the shared console and re-polls the triangle/diff
  // when done so the picture relocks (or re-flags) live.
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
      setTriKey((k) => k + 1);
    }
  }, [running, appendLog]);

  // SyncPanel hands us a ready descriptor ({kind, path, body, label}); just run it.
  const runSyncDescriptor = useCallback((desc) => {
    runDescriptor({ id: desc.kind, path: desc.path, body: desc.body, label: desc.label });
  }, [runDescriptor]);

  return (
    <div className="login">
      <div className="login-card">
        <div className="login-head">
          <h1>Talisman's Special Solutions — Release Console</h1>
          <p className="login-sub">
            One rulebook (the <strong>head</strong>), two execution engines (the{" "}
            <strong>legs</strong>): the OWL reasoner and the Postgres views.{" "}
            <em>Click a node to make it authoritative</em> — the floaty box shows what
            each other store would lose, lets you push the head left, right, down, or
            up, reset to baseline, and launch the console on either engine.
          </p>
        </div>

        {error && <div className="login-error">{error}</div>}

        <section className="login-section login-section-control">
          <SyncPanel
            onRunSync={runSyncDescriptor}
            running={running}
            refreshKey={triKey}
            backends={backends}
            onEnter={enter}
          />
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
