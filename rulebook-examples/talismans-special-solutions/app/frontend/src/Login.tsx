import React, { useEffect, useState, useRef, useCallback } from "react";
import { api, setBackend, runControl } from "./api";
import SyncPanel from "./SyncPanel";
import type { BackendDescriptor } from "./types";

// ===========================================================================
// Login / control page — now ONE interactive control.
//
// The whole gate is the DIAMOND (SyncPanel): the Rulebook source at the base,
// the two engines (Reasoner / Postgres) mid-height, and the Client app at the
// top apex, wired to exactly one engine. Everything that used to be stacked
// sections — pick-engine buttons, HEAD chips, the diff list, the directional
// sync buttons, AND the manual rebuild buttons — is folded into the picture.
// Nothing is privileged among the three STORES; the client is a reader, not a
// store:
//
//   • At rest, the diamond shows WHO IS HEAD among the stores — the Rulebook
//     when all three agree, otherwise the most-recently-edited store (a status
//     read-out only).
//   • HOVER a STORE node to ask "if THIS store were authoritative?" — a floaty
//     box appears with the field-level diff and the ONE push that makes it the
//     truth everywhere, plus a quarantined "reset-then-rebuild". Act straight
//     from the hover; move away and the box is gone.
//   • HOVER the CLIENT node to choose which engine the app reads — "Enter on
//     Reasoner / Postgres". The connected engine's wire is lit and carries its
//     drift color, so you can see you're about to read a STALE engine first.
//
// The build runs on the server and streams into the console below this control.
// App.jsx and the views are untouched — they render whatever engine computed.
// ===========================================================================

interface LoginProps {
  onEnter: (backendId: string) => void;
}

// The SSE event objects streamed by runControl during a build.
interface ControlEvent {
  type: string;
  label?: string;
  line?: string;
  action?: string;
  error?: string;
}

// A ready-to-run build descriptor (the shape SyncPanel hands up).
interface RunDescriptor {
  id?: string;
  path: string;
  body?: unknown;
  label: string;
}

// What SyncPanel emits via onRunSync — a descriptor keyed by `kind`.
interface SyncDescriptor {
  kind: string;
  path: string;
  body?: unknown;
  label: string;
}

type BuildResult = "done" | "error" | null;

export default function Login({ onEnter }: LoginProps) {
  const [backends, setBackends] = useState<BackendDescriptor[]>([]);
  const [log, setLog] = useState<string[]>([]);
  const [running, setRunning] = useState<string | null>(null); // action id/label while a build runs
  const [result, setResult] = useState<BuildResult>(null);    // "done" | "error" | null
  const [error, setError] = useState<string | null>(null);
  const [triKey, setTriKey] = useState<number>(0);        // bump to re-poll the triangle after a build
  const logRef = useRef<HTMLPreElement | null>(null);

  useEffect(() => {
    api.backends().then((b) => setBackends(b.backends || [])).catch((e) => setError(e.message));
  }, []);

  // Auto-scroll the streamed console.
  useEffect(() => {
    if (logRef.current) logRef.current.scrollTop = logRef.current.scrollHeight;
  }, [log]);

  const appendLog = useCallback((line: string) => setLog((l) => [...l, line]), []);

  // Launch the console on the chosen engine (handed up from the floaty box).
  const enter = useCallback((backendId: string) => {
    setBackend(backendId);
    onEnter(backendId);
  }, [onEnter]);

  // One streaming runner for every build descriptor the SyncPanel hands us
  // ({path, body, label}). `running` holds the active label (truthy = a build is
  // in flight); it streams into the shared console and re-polls the triangle/diff
  // when done so the picture relocks (or re-flags) live.
  const runDescriptor = useCallback(async ({ id, path, body = null, label }: RunDescriptor) => {
    if (running) return;
    setRunning(id || label);
    setResult(null);
    setError(null);
    setLog([`▶ ${label}`]);
    try {
      await runControl(path, (ev: ControlEvent) => {
        if (ev.type === "step") appendLog(`\n— ${ev.label} —`);
        else if (ev.type === "log") appendLog(ev.line ?? "");
        else if (ev.type === "done") { appendLog(`\n✓ ${ev.action} complete`); setResult("done"); }
        else if (ev.type === "error") { appendLog(`\n✗ ${ev.error}`); setResult("error"); }
      }, body);
    } catch (e) {
      appendLog(`\n✗ ${(e as Error).message}`);
      setResult("error");
    } finally {
      setRunning(null);
      setTriKey((k) => k + 1);
    }
  }, [running, appendLog]);

  // SyncPanel hands us a ready descriptor ({kind, path, body, label}); just run it.
  const runSyncDescriptor = useCallback((desc: SyncDescriptor) => {
    runDescriptor({ id: desc.kind, path: desc.path, body: desc.body, label: desc.label });
  }, [runDescriptor]);

  return (
    <div className="login">
      <div className="login-card">
        <div className="login-head">
          <h1>Talisman's Special Solutions — Release Console</h1>
          <p className="login-sub">
            One rulebook (the <strong>source</strong>, at the base), two execution
            engines derived from it (the OWL <strong>reasoner</strong> and the
            Postgres <strong>views</strong>), and your <strong>app</strong> at the
            top — wired to exactly one engine. The ring marks who's{" "}
            <strong>HEAD</strong> right now; none of the three is privileged.{" "}
            <em>Hover a store</em> to see what it would overwrite and push it,
            or <em>hover the app</em> to pick which engine to read and enter —
            straight from the hover.
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
