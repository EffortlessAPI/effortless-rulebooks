import { useCallback, useEffect, useRef, useState } from "react";
import { Link } from "react-router-dom";
import {
  fetchCorpusRuns,
  fetchCorpusStatus,
  fetchRows,
  invalidateCorpusTables,
  num,
  startCorpusRun,
  stopCorpusRun,
} from "../api.js";
import { useTables } from "../hooks.js";
import { Async, DataTable, Panel, Pill, Progress, Stat } from "../components.jsx";

// The corpus page: the registered test corpus and its fan-out history.
//
// Two different sources feed it, on purpose.
//
//  * Everything HISTORICAL is a generated view column — TestSuites.RegistrationState,
//    CorpusRuns.GreenPercent, CorpusDomainRuns.FailingPhase, RulebookDomains.CorpusTestState.
//    None of it is recomputed here; a fan-out records its rows and rebuilds the root,
//    and these tables then say what happened.
//  * Everything LIVE comes from the running fan-out's own status.json, polled through
//    /__corpus. That run has not been recorded yet — there is nothing in the rulebook
//    to read — so the in-flight grid is the one thing on this page that is not a view.
//    It becomes view data the moment the run finishes.

const POLL_MS = 2000;
const PHASES = ["build", "db", "conformance"];

export function Corpus() {
  const [version, setVersion] = useState(0);
  const [live, setLive] = useState(null); // the running (or most recent) status.json
  const [runs, setRuns] = useState([]);
  const [launching, setLaunching] = useState(false);
  const [error, setError] = useState(null);
  const [mode, setMode] = useState("full");
  const [kind, setKind] = useState("");
  const [note, setNote] = useState("");
  const wasLive = useRef(false);

  const poll = useCallback(async () => {
    const [status, runList] = await Promise.all([fetchCorpusStatus("latest"), fetchCorpusRuns()]);
    setLive(status);
    setRuns(runList);
    // A run that just crossed the finish line has appended its rows and rebuilt
    // the root, so every view on this page is stale until we drop the cache.
    if (wasLive.current && status && !status.live) {
      invalidateCorpusTables();
      setVersion((v) => v + 1);
    }
    wasLive.current = Boolean(status?.live);
  }, []);

  useEffect(() => {
    let cancelled = false;
    const tick = () => poll().catch((e) => !cancelled && setError(e.message));
    tick();
    const id = setInterval(tick, POLL_MS);
    return () => {
      cancelled = true;
      clearInterval(id);
    };
  }, [poll]);

  const state = useTables(
    ["TestSuites", "CorpusRuns", "CorpusDomainRuns", "RulebookDomains"],
    async () => {
      const [suites, corpusRuns, domainRuns, domains] = await Promise.all([
        fetchRows("TestSuites"),
        fetchRows("CorpusRuns"),
        fetchRows("CorpusDomainRuns"),
        fetchRows("RulebookDomains"),
      ]);
      return {
        suites: [...suites].sort((a, b) => a.test_suite_id.localeCompare(b.test_suite_id)),
        corpusRuns: [...corpusRuns].sort((a, b) => String(b.started_on).localeCompare(String(a.started_on))),
        domainRuns,
        domains,
      };
    },
    version,
  );

  async function launch() {
    setLaunching(true);
    setError(null);
    try {
      await startCorpusRun({ mode, kinds: kind ? [kind] : [], note });
      wasLive.current = true;
      await poll();
    } catch (e) {
      setError(e.message);
    } finally {
      setLaunching(false);
    }
  }

  async function stop() {
    setError(null);
    try {
      await stopCorpusRun(live.run_id);
      await poll();
    } catch (e) {
      setError(e.message);
    }
  }

  return (
    <>
      <section className="hero compact">
        <p className="eyebrow">Health · corpus</p>
        <h1>Every registered suite, every fan-out, one queue to green</h1>
        <p className="summary">
          A suite runs because a <code>TestSuites</code> row declares it registered — never because a
          directory happened to contain answer keys. Each fan-out walks three phases per project (build,
          database reset, cross-substrate conformance) and records one <code>CorpusDomainRuns</code> row
          per project, so a red project is red for a named phase. Scores, green percentages and per-project
          standing are rulebook formulas read from the views.
        </p>
        <nav className="subnav">
          <Link to="/consistency">Consistency</Link>
          <Link to="/progress">Progress</Link>
          <Link to="/conformance">Conformance</Link>
          <Link className="active" to="/corpus">Corpus</Link>
        </nav>
      </section>

      {error && (
        <div className="panel error" role="alert">
          <h3>Corpus runner</h3>
          <p>{error}</p>
        </div>
      )}

      <LiveRun live={live} runs={runs} onStop={stop} />

      <Panel
        eyebrow="Run"
        title="Launch a fan-out"
        actions={
          <div className="filters">
            <label>
              Mode{" "}
              <select value={mode} onChange={(e) => setMode(e.target.value)}>
                <option value="full">full — build, reset, grade every substrate</option>
                <option value="build-only">build-only — build and reset, skip grading</option>
              </select>
            </label>
            <label>
              Scope{" "}
              <select value={kind} onChange={(e) => setKind(e.target.value)}>
                <option value="">every registered suite</option>
                <option value="root">root only</option>
                <option value="toy">toys only</option>
                <option value="example">examples only</option>
              </select>
            </label>
            <label>
              Note{" "}
              <input
                type="text"
                value={note}
                placeholder="why this run"
                onChange={(e) => setNote(e.target.value)}
              />
            </label>
            <button className="button" onClick={launch} disabled={launching || Boolean(live?.live)}>
              {live?.live ? "a run is in flight" : launching ? "starting…" : "Run corpus"}
            </button>
          </div>
        }
      >
        <p className="muted">
          Builds are sequential on purpose: every <code>effortless build</code> goes through the
          ssotme-proxy on <code>:4242</code>, and concurrent builds corrupt each other. The runner is
          detached — closing this tab does not stop it, and reopening this page reattaches to it.
        </p>
      </Panel>

      <Async state={state} what="the corpus">
        {({ suites, corpusRuns, domainRuns, domains }) => (
          <>
            <CorpusStanding suites={suites} corpusRuns={corpusRuns} domains={domains} />
            <Registry suites={suites} />
            <History corpusRuns={corpusRuns} domainRuns={domainRuns} />
          </>
        )}
      </Async>
    </>
  );
}

// The in-flight (or most recent) fan-out, straight from its status.json.
function LiveRun({ live, runs, onStop }) {
  if (!live) {
    return (
      <Panel eyebrow="In flight" title="No fan-out has been launched yet">
        <p className="muted">
          Run one below. Its progress appears here as it goes, and lands in the views when it finishes.
        </p>
      </Panel>
    );
  }

  const domains = Object.values(live.domains);
  const done = domains.filter((d) => d.state === "done");
  const green = done.filter((d) => !PHASES.some((p) => d[p] === "fail"));
  const percent = domains.length ? (100 * done.length) / domains.length : 0;
  const meta = runs.find((r) => r.run_id === live.run_id);

  return (
    <Panel
      eyebrow={live.live ? "In flight" : "Most recent fan-out"}
      title={live.run_id}
      actions={
        live.live ? (
          <button className="button danger" onClick={onStop}>
            Stop
          </button>
        ) : null
      }
    >
      <div className="stats">
        <Stat label="mode" value={live.mode} />
        <Stat label="targets" value={live.target_count} />
        <Stat label="finished" value={`${done.length}/${domains.length}`} />
        <Stat label="green so far" value={green.length} hint={`${done.length - green.length} red`} />
        <Stat
          label="state"
          value={live.live ? (live.current ?? live.phase ?? "running") : (live.phase ?? "finished")}
          hint={
            live.phase === "recording" ? "appending rows to the rulebook"
              : live.phase === "building-root" ? "rebuilding the root so the views recompute"
              : live.phase === "root-build-failed" ? "rows recorded, but the root build failed"
              : undefined
          }
        />
      </div>
      <Progress percent={percent} />
      {live.note && <p className="muted">{live.note}</p>}
      {meta && !live.live && !live.finished_on && (
        <p className="muted">
          This run's process is gone but it never wrote a finish time — it was stopped or killed part
          way. Its completed projects are still listed below; nothing was recorded to the rulebook.
        </p>
      )}
      <DataTable
        rows={domains}
        rowKey="slug"
        empty="No targets selected."
        columns={[
          { key: "slug", label: "Project", render: (r) => <strong>{r.slug}</strong> },
          { key: "kind", label: "Kind", render: (r) => <Pill value={r.kind} /> },
          { key: "build", label: "Build", render: (r) => <Pill value={r.build} /> },
          { key: "db", label: "DB", render: (r) => <Pill value={r.db} /> },
          { key: "conformance", label: "Conformance", render: (r) => <Pill value={r.conformance} /> },
          {
            key: "substrates",
            label: "Substrates",
            render: (r) =>
              r.substrates_tested == null ? "—" : `${r.substrates_passed}/${r.substrates_tested}`,
          },
          { key: "duration_seconds", label: "Seconds", render: (r) => r.duration_seconds ?? "—" },
          {
            key: "first_error",
            label: "First error",
            render: (r) => (r.first_error ? <code className="error-inline">{r.first_error}</code> : "—"),
          },
        ]}
      />
    </Panel>
  );
}

// Where the corpus stands right now — every number a view column.
function CorpusStanding({ suites, corpusRuns, domains }) {
  const latest = corpusRuns.find((r) => r.is_latest);
  const registered = suites.filter((s) => s.is_registered);
  const runnable = suites.filter((s) => s.is_runnable);
  const graded = suites.filter((s) => s.is_gradable);

  return (
    <Panel eyebrow="Standing" title="The corpus, as of the latest fan-out">
      <div className="stats">
        <Stat label="registered suites" value={registered.length} hint={`of ${suites.length} known`} />
        <Stat label="runnable" value={runnable.length} />
        <Stat label="already graded" value={graded.length} hint={`${runnable.length - graded.length} never exercised`} />
        <Stat
          label="green in latest run"
          value={latest ? latest.green_domain_count : "—"}
          hint={latest ? `of ${latest.domain_run_count} attempted` : "no fan-out recorded yet"}
        />
        <Stat
          label="built and graded green"
          value={latest ? latest.fully_green_domain_count : "—"}
          hint={latest ? `${num(latest.fully_green_percent)}% of attempted` : undefined}
        />
      </div>
      {latest && <Progress percent={num(latest.green_percent)} />}
      <DataTable
        rows={[...domains].sort((a, b) => a.slug.localeCompare(b.slug))}
        rowKey="domain_id"
        empty="No projects modeled."
        columns={[
          {
            key: "slug",
            label: "Project",
            render: (r) => <Link to={`/projects/${r.slug}`}>{r.domain_name}</Link>,
          },
          { key: "kind", label: "Kind", render: (r) => <Pill value={r.kind} /> },
          { key: "corpus_test_state", label: "Latest run", render: (r) => <Pill value={r.corpus_test_state} /> },
          { key: "corpus_attempt_count", label: "Attempts", render: (r) => r.corpus_attempt_count ?? 0 },
          { key: "test_suite_count", label: "Suites", render: (r) => r.test_suite_count ?? 0 },
          { key: "conformance_run_count", label: "Conformance runs", render: (r) => r.conformance_run_count ?? 0 },
        ]}
      />
    </Panel>
  );
}

function Registry({ suites }) {
  return (
    <Panel eyebrow="Registry" title="Registered test suites">
      <p className="muted">
        <code>IsRegistered</code> and <code>ExpectedSubstrateCount</code> are declarations — edit them in the
        rulebook. The witness columns are refreshed by <code>scripts/scan-test-suites.py</code> so
        <code>RegistrationState</code> can say why a registered suite is not ready, rather than the runner
        skipping it silently.
      </p>
      <DataTable
        rows={suites}
        rowKey="test_suite_id"
        empty="No suites registered."
        columns={[
          { key: "test_suite_id", label: "Suite", render: (r) => <code>{r.test_suite_id}</code> },
          { key: "suite_kind", label: "Kind", render: (r) => <Pill value={r.suite_kind} /> },
          { key: "domain_name", label: "Project", render: (r) => r.domain_name ?? "—" },
          { key: "is_registered", label: "Registered", render: (r) => <Pill value={Boolean(r.is_registered)} /> },
          { key: "registration_state", label: "State", render: (r) => <Pill value={r.registration_state} /> },
          { key: "answer_key_count", label: "Answer keys", render: (r) => r.answer_key_count ?? 0 },
          {
            key: "expected_substrate_count",
            label: "Expected substrates",
            render: (r) => r.expected_substrate_count ?? 0,
          },
          { key: "corpus_domain_run_count", label: "Runs", render: (r) => r.corpus_domain_run_count ?? 0 },
        ]}
      />
    </Panel>
  );
}

function History({ corpusRuns, domainRuns }) {
  const [openId, setOpenId] = useState(null);
  const open = corpusRuns.find((r) => r.corpus_run_id === openId);
  const rows = open
    ? domainRuns
        .filter((d) => d.corpus_run === open.corpus_run_id)
        .sort((a, b) => String(a.domain_name).localeCompare(String(b.domain_name)))
    : [];

  return (
    <Panel eyebrow="History" title="Recorded fan-outs">
      <DataTable
        rows={corpusRuns}
        rowKey="corpus_run_id"
        empty="No fan-out has been recorded to the rulebook yet."
        columns={[
          {
            key: "corpus_run_id",
            label: "Run",
            render: (r) => (
              <button className="linkbtn" onClick={() => setOpenId(openId === r.corpus_run_id ? null : r.corpus_run_id)}>
                {r.corpus_run_id}
              </button>
            ),
          },
          { key: "mode", label: "Mode", render: (r) => <Pill value={r.mode} /> },
          { key: "started_on", label: "Started", render: (r) => String(r.started_on ?? "").replace("T", " ") },
          { key: "overall_status", label: "Status", render: (r) => <Pill value={r.overall_status} /> },
          {
            key: "green",
            label: "Green",
            render: (r) => `${r.green_domain_count ?? 0}/${r.domain_run_count ?? 0}`,
          },
          { key: "green_percent", label: "%", render: (r) => <Progress percent={num(r.green_percent)} /> },
          { key: "build_failure_count", label: "Build fails", render: (r) => r.build_failure_count ?? 0 },
          { key: "conformance_failure_count", label: "Conformance fails", render: (r) => r.conformance_failure_count ?? 0 },
          { key: "is_latest", label: "Latest", render: (r) => <Pill value={Boolean(r.is_latest)} /> },
        ]}
      />
      {open && (
        <>
          <h3>{open.corpus_run_id}</h3>
          {open.notes && <p className="muted">{open.notes}</p>}
          <DataTable
            rows={rows}
            rowKey="corpus_domain_run_id"
            empty="This run recorded no projects."
            columns={[
              { key: "domain_name", label: "Project" },
              { key: "build_status", label: "Build", render: (r) => <Pill value={r.build_status} /> },
              { key: "db_status", label: "DB", render: (r) => <Pill value={r.db_status} /> },
              { key: "conformance_status", label: "Conformance", render: (r) => <Pill value={r.conformance_status} /> },
              { key: "conformance_outcome", label: "Outcome", render: (r) => <Pill value={r.conformance_outcome} /> },
              { key: "failing_phase", label: "Failed at", render: (r) => (r.failing_phase ? <Pill value={r.failing_phase} /> : "—") },
              {
                key: "substrates",
                label: "Substrates",
                render: (r) => (r.substrates_tested == null ? "—" : `${r.substrates_passed ?? 0}/${r.substrates_tested}`),
              },
              { key: "duration_seconds", label: "Seconds", render: (r) => r.duration_seconds ?? "—" },
              {
                key: "first_error",
                label: "First error",
                render: (r) => (r.first_error ? <code className="error-inline">{r.first_error}</code> : "—"),
              },
            ]}
          />
        </>
      )}
    </Panel>
  );
}
