// Two-step download UX: clicking "Rulebook.xlsx" opens a modal that streams
// the refresh script's stdout via SSE from /api/rulebook.xlsx/prepare, then
// auto-triggers the actual /api/rulebook.xlsx download when the build is done.

import { useRef, useState } from "react";

type Phase = "idle" | "running" | "done" | "fail";

export function RulebookDownload() {
  const [open, setOpen] = useState(false);
  const [phase, setPhase] = useState<Phase>("idle");
  const [log, setLog] = useState<string[]>([]);
  const [errMsg, setErrMsg] = useState<string | null>(null);
  const esRef = useRef<EventSource | null>(null);

  const start = () => {
    setOpen(true);
    setPhase("running");
    setLog([]);
    setErrMsg(null);

    const es = new EventSource("/api/rulebook.xlsx/prepare");
    esRef.current = es;

    es.addEventListener("log", (e) => {
      const line = (e as MessageEvent).data as string;
      setLog((prev) => [...prev, line]);
    });

    es.addEventListener("done", () => {
      es.close();
      esRef.current = null;
      setPhase("done");
      // Trigger the actual file download.
      const a = document.createElement("a");
      a.href = "/api/rulebook.xlsx";
      a.download = "customer-crm-rulebook.xlsx";
      document.body.appendChild(a);
      a.click();
      a.remove();
    });

    es.addEventListener("fail", (e) => {
      es.close();
      esRef.current = null;
      setPhase("fail");
      try {
        const d = JSON.parse((e as MessageEvent).data);
        setErrMsg(d.message ?? "Unknown error");
      } catch {
        setErrMsg("Unknown error");
      }
    });

    // EventSource auto-reconnects on transport-level errors. Once the server
    // closes the stream (after done/fail), the browser fires onerror. We
    // already handle final state via custom events, so just close here to
    // suppress the reconnect loop.
    es.onerror = () => {
      if (phase === "running" && es.readyState === EventSource.CLOSED) {
        // Connection dropped before we got done/fail.
        setPhase((p) => (p === "running" ? "fail" : p));
        setErrMsg((m) => m ?? "Lost connection to server.");
      }
      es.close();
    };
  };

  const close = () => {
    if (phase === "running" && esRef.current) {
      esRef.current.close();
      esRef.current = null;
    }
    setOpen(false);
  };

  const lastLine = log[log.length - 1] ?? "Working…";

  return (
    <>
      <button
        className="rulebook-download"
        onClick={start}
        disabled={phase === "running"}
        title="Refresh from Postgres and download the rulebook as Excel"
      >
        ⬇ Rulebook.xlsx
      </button>

      {open && (
        <div
          className="rulebook-overlay"
          onClick={() => phase !== "running" && close()}
        >
          <div className="rulebook-modal" onClick={(e) => e.stopPropagation()}>
            <div className="rulebook-modal-head">
              <h3>
                {phase === "running" && "Preparing rulebook.xlsx…"}
                {phase === "done" && "✅ Rulebook ready — download started"}
                {phase === "fail" && "❌ Refresh failed"}
              </h3>
            </div>

            <div className={`rulebook-progress rulebook-progress-${phase}`}>
              <div className="rulebook-progress-bar" />
            </div>

            <div className="rulebook-status">{lastLine}</div>

            <details className="rulebook-log-details">
              <summary>Log ({log.length} lines)</summary>
              <pre className="rulebook-log">{log.join("\n")}</pre>
            </details>

            {errMsg && <p className="rulebook-error">{errMsg}</p>}

            <div className="rulebook-modal-actions">
              {phase === "done" && (
                <a className="rulebook-redownload" href="/api/rulebook.xlsx">
                  Re-download
                </a>
              )}
              <button onClick={close} disabled={phase === "running"}>
                {phase === "running" ? "Working…" : "Close"}
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
