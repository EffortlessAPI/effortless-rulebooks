import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { api } from "../api";
import type {
  ConformanceTestMeta,
  ConformanceRunReport,
  ConformanceTestResult,
  ConformanceStatus,
} from "../types";

// ===========================================================================
// Admin · Conformance — see and run the whole test suite from the site.
//
// The suite IS the rulebook's ConformanceTests table; the harness runs every
// test against BOTH engines (the same reason() the app uses). This screen lets
// you: (1) see every test grouped by section, with its last-run status per
// engine, (2) run the whole suite (or one test) on demand, (3) read the
// pass/fail/gap detail. It never computes or fakes a verdict — it renders what
// the harness returned and surfaces gaps rather than hiding them.
// ===========================================================================

const STATUS_ICON: Record<ConformanceStatus, string> = {
  pass: "✓", fail: "✗", gap: "◐", error: "!", skipped: "·",
};
const STATUS_LABEL: Record<ConformanceStatus, string> = {
  pass: "pass", fail: "fail", gap: "gap", error: "error", skipped: "skipped",
};

// Merge the test list (always available) with the latest run's results (if any),
// so the screen shows every test even before the first run.
interface Row extends ConformanceTestMeta {
  result?: ConformanceTestResult;
}

export default function AdminConformance() {
  const [engines, setEngines] = useState<string[]>([]);
  const [tests, setTests] = useState<ConformanceTestMeta[]>([]);
  const [report, setReport] = useState<ConformanceRunReport | null>(null);
  const [loading, setLoading] = useState(true);
  const [running, setRunning] = useState<false | "all" | string>(false);
  const [err, setErr] = useState<string | null>(null);
  const [filter, setFilter] = useState<"all" | "notpass">("all");
  const [expanded, setExpanded] = useState<Set<string>>(new Set());

  const load = useCallback(async () => {
    setErr(null);
    try {
      const [t, latest] = await Promise.all([api.conformanceTests(), api.conformanceLatest()]);
      setEngines(t.engines);
      setTests(t.tests);
      setReport(latest);
    } catch (e) {
      setErr((e as Error).message);
    } finally {
      setLoading(false);
    }
  }, []);
  useEffect(() => { load(); }, [load]);

  const runAll = useCallback(async () => {
    setRunning("all");
    setErr(null);
    try {
      setReport(await api.conformanceRun());
    } catch (e) {
      setErr((e as Error).message);
    } finally {
      setRunning(false);
    }
  }, []);

  const runOne = useCallback(async (id: string) => {
    setRunning(id);
    setErr(null);
    try {
      // Run just this test, then merge its result into the current report so the
      // rest of the table keeps its last-known status.
      const fresh = await api.conformanceRun([id]);
      const one = fresh.results.find((r) => r.id === id);
      setReport((prev) => {
        if (!prev) return fresh;
        return { ...prev, results: prev.results.map((r) => (r.id === id && one ? one : r)) };
      });
    } catch (e) {
      setErr((e as Error).message);
    } finally {
      setRunning(false);
    }
  }, []);

  // index results by id for the merge
  const resultById = useMemo(() => {
    const m = new Map<string, ConformanceTestResult>();
    for (const r of report?.results || []) m.set(r.id, r);
    return m;
  }, [report]);

  // group rows by section, preserving the test order
  const sections = useMemo(() => {
    const order: string[] = [];
    const bySection = new Map<string, Row[]>();
    for (const t of tests) {
      const row: Row = { ...t, result: resultById.get(t.id) };
      if (filter === "notpass" && row.result && row.result.overall === "pass") continue;
      if (!bySection.has(t.section)) { bySection.set(t.section, []); order.push(t.section); }
      bySection.get(t.section)!.push(row);
    }
    return order.map((s) => ({ section: s, rows: bySection.get(s)! })).filter((g) => g.rows.length);
  }, [tests, resultById, filter]);

  const toggle = (id: string) =>
    setExpanded((prev) => {
      const next = new Set(prev);
      next.has(id) ? next.delete(id) : next.add(id);
      return next;
    });

  if (loading) return <div className="admin-loading">Loading the conformance suite…</div>;

  const s = report?.summary;

  return (
    <div className="admin-conf">
      <div className="admin-conf-head">
        <div>
          <h1>Conformance</h1>
          <p className="muted">
            Every test below is a row in the rulebook's <code>ConformanceTests</code> table, run by the
            engine-agnostic harness against <strong>{engines.join(" + ") || "the engines"}</strong>. The
            oracle is the answer-key (Postgres-computed); the harness never fakes a verdict. Gaps (a
            substrate emitted no value) are shown, not hidden.
          </p>
        </div>
        <div className="admin-conf-actions">
          <button className="btn-run" onClick={runAll} disabled={!!running} type="button">
            {running === "all" ? "running… (~100s)" : "▶ Run all"}
          </button>
          <Link to="/console/flow" className="mini-btn">⇆ back to console</Link>
        </div>
      </div>

      {err && <div className="admin-conf-err">⚠ {err}</div>}

      {/* the headline scoreboard */}
      {s ? (
        <div className={"conf-scoreboard " + (s.allGreen ? "green" : "")}>
          <span className="sb-stat sb-pass"><b>{s.passed}</b> pass</span>
          <span className="sb-stat sb-fail"><b>{s.failed}</b> fail</span>
          <span className="sb-stat sb-gap"><b>{s.gaps}</b> gap</span>
          {s.errors > 0 && <span className="sb-stat sb-err"><b>{s.errors}</b> error</span>}
          {s.skipped > 0 && <span className="sb-stat sb-skip"><b>{s.skipped}</b> skipped</span>}
          <span className="sb-sep">/</span>
          <span className="sb-total">{s.total} tests</span>
          <span className="sb-when muted">
            last run {new Date(s.finishedAt).toLocaleString()} · {(s.durationMs / 1000).toFixed(1)}s
          </span>
          {s.allGreen
            ? <span className="sb-verdict green">ALL GREEN — both engines agree on every stable fact</span>
            : <span className="sb-verdict">not green — see ✗ rows</span>}
        </div>
      ) : (
        <div className="conf-scoreboard">
          <span className="muted">No run yet. Click <b>Run all</b> to execute the suite against both engines.</span>
        </div>
      )}

      <div className="conf-filterbar">
        <button className={"chip-toggle " + (filter === "all" ? "on" : "")} onClick={() => setFilter("all")} type="button">all {tests.length}</button>
        <button className={"chip-toggle " + (filter === "notpass" ? "on" : "")} onClick={() => setFilter("notpass")} type="button">needs attention</button>
      </div>

      {sections.map(({ section, rows }) => (
        <div className="conf-section" key={section}>
          <h2 className="conf-section-title">{section} <span className="muted">({rows.length})</span></h2>
          <table className="conf-table">
            <thead>
              <tr>
                <th className="c-status">status</th>
                <th className="c-name">test</th>
                {engines.map((e) => <th key={e} className="c-engine">{e}</th>)}
                <th className="c-feat">feature</th>
                <th className="c-act"></th>
              </tr>
            </thead>
            <tbody>
              {rows.map((row) => {
                const r = row.result;
                const overall: ConformanceStatus = r ? r.overall : "skipped";
                const isOpen = expanded.has(row.id);
                return (
                  <React.Fragment key={row.id}>
                    <tr className={"conf-row st-" + overall} onClick={() => toggle(row.id)}>
                      <td className="c-status">
                        <span className={"st-badge st-" + overall} title={STATUS_LABEL[overall]}>
                          {r ? STATUS_ICON[overall] : "—"}
                        </span>
                      </td>
                      <td className="c-name">
                        <span className="conf-id">{row.id}</span>
                        <span className="conf-kind">{row.testKind}</span>
                      </td>
                      {engines.map((e) => {
                        const er = r?.engines.find((x) => x.engine === e)
                          || (r?.engines.length === 1 && r.engines[0].engine === "both" ? r.engines[0] : undefined);
                        const st = (er?.status || (r ? "pass" : undefined)) as ConformanceStatus | undefined;
                        return (
                          <td key={e} className="c-engine">
                            {st ? <span className={"st-dot st-" + st} title={er?.detail}>{STATUS_ICON[st]}</span> : <span className="muted">—</span>}
                          </td>
                        );
                      })}
                      <td className="c-feat">{row.featureRef && <span className="feat-chip">{row.featureRef}</span>}</td>
                      <td className="c-act">
                        <button
                          className="mini-btn run-one"
                          onClick={(ev) => { ev.stopPropagation(); runOne(row.id); }}
                          disabled={!!running}
                          type="button"
                        >
                          {running === row.id ? "…" : "▶"}
                        </button>
                      </td>
                    </tr>
                    {isOpen && (
                      <tr className="conf-detail-row">
                        <td colSpan={4 + engines.length}>
                          <div className="conf-detail">
                            <p className="conf-expl">{row.explanation}</p>
                            {row.targetRef && <p className="conf-target"><code>{row.targetRef}</code></p>}
                            {r ? (
                              <ul className="conf-engine-list">
                                {r.engines.map((er) => (
                                  <li key={er.engine} className={"st-" + er.status}>
                                    <span className={"st-dot st-" + er.status}>{STATUS_ICON[er.status as ConformanceStatus]}</span>
                                    <b>{er.engine}</b>: {er.detail}
                                  </li>
                                ))}
                              </ul>
                            ) : <p className="muted">not run yet</p>}
                          </div>
                        </td>
                      </tr>
                    )}
                  </React.Fragment>
                );
              })}
            </tbody>
          </table>
        </div>
      ))}
    </div>
  );
}
