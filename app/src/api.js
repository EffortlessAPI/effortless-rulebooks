// Every read goes through the generated, view-backed API (GET /api/tables/:table),
// which serves vw_<table> rows. Calculated columns are rendered, never recomputed.

const cache = new Map();

export class ApiError extends Error {
  constructor(message, { status, table } = {}) {
    super(message);
    this.status = status;
    this.table = table;
  }
}

export async function fetchTable(table) {
  if (cache.has(table)) return cache.get(table);
  const promise = (async () => {
    const response = await fetch(`/api/tables/${table}`);
    if (!response.ok) {
      cache.delete(table);
      throw new ApiError(`GET /api/tables/${table} failed with HTTP ${response.status}`, {
        status: response.status,
        table,
      });
    }
    const body = await response.json();
    if (!Array.isArray(body.rows) || !Array.isArray(body.fields) || !body.pkField) {
      cache.delete(table);
      throw new ApiError(`GET /api/tables/${table} returned an unexpected shape (expected fields[], pkField, rows[])`, { table });
    }
    return body;
  })();
  cache.set(table, promise);
  return promise;
}

// Drop cached tables so the next read goes back to the views.
export function invalidate(...tables) {
  for (const table of tables) cache.delete(table);
}

// Close a finding by hand (fixed | accepted-exception). The explorer's dev server
// writes the rulebook JSON through scripts/mark-finding-fixed.py and PATCHes the
// editor's base table; every derived score is re-read from the views afterwards.
export async function setFindingStatus(id, status) {
  const response = await fetch(`/__findings/${encodeURIComponent(id)}/status`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ status }),
  });
  const body = await response.json();
  if (!response.ok || !body.ok) throw new ApiError(body.error || `closing ${id} failed with HTTP ${response.status}`, { status: response.status, table: "ConsistencyFindings" });
  invalidate("ConsistencyFindings", "ConsistencyRules", "RulebookDomains", "ProjectMetadata");
  return body;
}

// Trigger a new conformance harness run for a project and stream its progress.
// The explorer's dev server shells out to scripts/run-conformance.py (which
// invokes the existing orchestration/test-orchestrator.py, records
// ConformanceRuns/ConformanceResults rows in the rulebook, generates the
// aggregate orchestration-report.html, then runs `effortless build` so the
// views pick them up) and streams its stdout/stderr back as Server-Sent
// Events — see conformanceRunPlugin in vite.config.js. `onLog` is called with
// each line as it arrives; the returned promise resolves with the final
// summary ({ run_id, substrates, report_path }) once the run finishes.
export async function runConformance(slug, onLog) {
  const response = await fetch(`/__conformance/${encodeURIComponent(slug)}/run`, { method: "POST" });
  if (!response.ok || !response.body) {
    throw new ApiError(`running conformance for ${slug} failed with HTTP ${response.status}`, { status: response.status, table: "ConformanceRuns" });
  }

  const reader = response.body.getReader();
  const decoder = new TextDecoder();
  let buffer = "";
  let result = null;

  while (true) {
    const { done, value } = await reader.read();
    if (done) break;
    buffer += decoder.decode(value, { stream: true });
    const events = buffer.split("\n\n");
    buffer = events.pop(); // last chunk may be incomplete
    for (const raw of events) {
      const eventMatch = /^event: (\w+)\ndata: (.*)$/s.exec(raw);
      if (!eventMatch) continue;
      const [, event, dataRaw] = eventMatch;
      const data = JSON.parse(dataRaw);
      if (event === "log") {
        onLog?.(data.line);
      } else if (event === "done") {
        result = data;
      }
    }
  }

  if (!result || !result.ok) {
    throw new ApiError(result?.error || `running conformance for ${slug} did not report a result`, { table: "ConformanceRuns" });
  }
  invalidate("ConformanceRuns", "ConformanceResults", "RulebookDomains");
  return result;
}

export async function fetchRows(table) {
  return (await fetchTable(table)).rows;
}

export async function fetchOne(table, pkColumn, value) {
  const rows = await fetchRows(table);
  const matches = rows.filter((row) => String(row[pkColumn]) === String(value));
  if (matches.length !== 1) {
    throw new ApiError(
      `${table}: expected exactly one row with ${pkColumn} = ${JSON.stringify(value)}, found ${matches.length}`,
      { table },
    );
  }
  return matches[0];
}

export async function fetchDocs() {
  const response = await fetch("/api/docs");
  if (!response.ok) throw new ApiError(`GET /api/docs failed with HTTP ${response.status}`, { status: response.status });
  return response.json();
}

export async function fetchViewHealth() {
  const response = await fetch("/api/view-health");
  if (!response.ok) throw new ApiError(`GET /api/view-health failed with HTTP ${response.status}`, { status: response.status });
  return response.json();
}

// Ask the explorer's own dev server to probe a modeled localhost URL.
export async function probe(url) {
  const response = await fetch(`/__probe?url=${encodeURIComponent(url)}`);
  const body = await response.json();
  if (!response.ok) throw new ApiError(body.error || `probe failed with HTTP ${response.status}`, { status: response.status });
  return body;
}

// Postgres numerics arrive as strings; coerce only for display.
export function num(value) {
  if (value === null || value === undefined || value === "") return null;
  const n = Number(value);
  return Number.isNaN(n) ? null : n;
}

// Reverse-relationship columns arrive as "id-a, id-b" strings.
export function idList(value) {
  if (!value) return [];
  return String(value)
    .split(",")
    .map((s) => s.trim())
    .filter(Boolean);
}

// ---------------------------------------------------------------------------
// Corpus runs. The runner is detached and owns its own live status.json, so the
// UI polls rather than holding a stream: a fan-out survives a page reload, two
// people can watch the same run, and closing the tab does not kill it.
// ---------------------------------------------------------------------------
async function corpus(path, init) {
  const response = await fetch(`/__corpus${path}`, init);
  const body = await response.json();
  if (!response.ok || !body.ok) {
    throw new ApiError(body.error || `/__corpus${path} failed with HTTP ${response.status}`, {
      status: response.status,
      table: "CorpusRuns",
    });
  }
  return body;
}

// Launch a fan-out. `mode` is "full" or "build-only"; `kinds` filters by declared
// Kind; `only` restricts to named slugs. Resolves with the new run's id.
export async function startCorpusRun({ mode = "full", kinds = [], only = [], note = "" } = {}) {
  const body = await corpus("/run", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ mode, kinds, only, note }),
  });
  return body.run_id;
}

// The live status document for one run, or the newest one ("latest").
// Returns null when no fan-out has ever been launched.
export async function fetchCorpusStatus(runId = "latest") {
  return (await corpus(`/${runId}`)).status;
}

export async function fetchCorpusRuns() {
  return (await corpus("/runs")).runs;
}

export async function stopCorpusRun(runId) {
  return corpus(`/${runId}/stop`, { method: "POST" });
}

// A finished fan-out has appended its rows to the rulebook and rebuilt the root,
// so every corpus view is stale until these are dropped.
export function invalidateCorpusTables() {
  invalidate("CorpusRuns", "CorpusDomainRuns", "TestSuites", "RulebookDomains", "ConformanceRuns", "ConformanceResults");
}
