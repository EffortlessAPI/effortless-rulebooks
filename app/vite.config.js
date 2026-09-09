import { defineConfig } from "vite";
import path from "node:path";
import fs from "node:fs/promises";
import { statSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { execFile, spawn } from "node:child_process";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const EDITOR_API = "http://localhost:42441";

// Server-side reachability probe for modeled localhost services.
// The browser cannot probe other localhost ports itself (CORS), so the explorer
// asks its own dev server to try. Only http://localhost / 127.0.0.1 URLs are
// accepted; anything else is refused rather than guessed at.
function healthProbePlugin() {
  return {
    name: "erb-health-probe",
    configureServer(server) {
      server.middlewares.use("/__probe", async (req, res) => {
        const url = new URL(req.url, "http://localhost").searchParams.get("url");
        res.setHeader("Content-Type", "application/json");
        let target;
        try {
          target = new URL(url);
        } catch {
          res.statusCode = 400;
          res.end(JSON.stringify({ ok: false, error: "missing or malformed url" }));
          return;
        }
        if (target.protocol !== "http:" || !["localhost", "127.0.0.1"].includes(target.hostname)) {
          res.statusCode = 400;
          res.end(JSON.stringify({ ok: false, error: "only http://localhost URLs may be probed" }));
          return;
        }
        const controller = new AbortController();
        const timer = setTimeout(() => controller.abort(), 2000);
        try {
          const response = await fetch(target, { method: "GET", signal: controller.signal, redirect: "manual" });
          res.end(JSON.stringify({ ok: response.status < 500, status: response.status, url: target.href }));
        } catch (error) {
          res.end(JSON.stringify({ ok: false, error: error.name === "AbortError" ? "timeout" : String(error.message || error), url: target.href }));
        } finally {
          clearTimeout(timer);
        }
      });
    },
  };
}

// Serve the root's generated documents (progress report, RuleSpeak) read-only from
// the repository so the explorer can link to them without copying them.
const GENERATED = {
  "/generated/progress-report/": path.resolve(__dirname, "../progress-report/progress-report"),
  "/generated/rulespeak/": path.resolve(__dirname, "../rulespeak"),
};
const MIME = { ".html": "text/html; charset=utf-8", ".htm": "text/html; charset=utf-8", ".md": "text/markdown; charset=utf-8", ".json": "application/json", ".css": "text/css", ".js": "text/javascript", ".png": "image/png", ".svg": "image/svg+xml" };

function generatedFilesPlugin() {
  return {
    name: "erb-generated-files",
    configureServer(server) {
      server.middlewares.use(async (req, res, next) => {
        const pathname = new URL(req.url, "http://localhost").pathname;
        const prefix = Object.keys(GENERATED).find((p) => pathname.startsWith(p));
        if (!prefix) return next();
        const relative = decodeURIComponent(pathname.slice(prefix.length));
        const file = path.resolve(GENERATED[prefix], relative);
        if (!file.startsWith(GENERATED[prefix] + path.sep)) {
          res.statusCode = 400;
          res.end("path escapes the generated directory");
          return;
        }
        try {
          const data = await fs.readFile(file);
          res.setHeader("Content-Type", MIME[path.extname(file)] || "application/octet-stream");
          res.end(data);
        } catch (error) {
          res.statusCode = error.code === "ENOENT" ? 404 : 500;
          res.setHeader("Content-Type", "text/plain");
          res.end(`${error.code === "ENOENT" ? "generated file not found" : "could not read generated file"}: ${file}\nRun 'effortless build' at the repository root to regenerate it.`);
        }
      });
    },
  };
}

// Close a consistency finding from the work queue (US-036).
// The rulebook JSON is HEAD, so the write goes through scripts/mark-finding-fixed.py
// (the same diff-minimal path the CLI uses; it refuses non-open and scanner-derived
// findings). The editor's base table is then PATCHed so the live views recompute at
// once; the editor's own save-changes is NOT used because it rewrites the whole
// file (finding cr-21-01).
const REPO_ROOT = path.resolve(__dirname, "..");
const RULEBOOK = "effortless-rulebook/effortless-rulebook.json";
const FINDING_STATUSES = new Set(["fixed", "accepted-exception"]);

function readJsonBody(req) {
  return new Promise((resolve, reject) => {
    let raw = "";
    req.on("data", (chunk) => (raw += chunk));
    req.on("end", () => {
      try {
        resolve(raw ? JSON.parse(raw) : {});
      } catch (error) {
        reject(error);
      }
    });
    req.on("error", reject);
  });
}

function markFindingInRulebook(id, status) {
  return new Promise((resolve, reject) => {
    execFile(
      "python3",
      ["scripts/mark-finding-fixed.py", RULEBOOK, "--status", status, id],
      { cwd: REPO_ROOT },
      (error, stdout, stderr) => (error ? reject(new Error((stderr || stdout || error.message).trim())) : resolve(stdout.trim())),
    );
  });
}

function findingStatusPlugin() {
  return {
    name: "erb-finding-status",
    configureServer(server) {
      server.middlewares.use("/__findings", async (req, res) => {
        res.setHeader("Content-Type", "application/json");
        const match = /^\/([A-Za-z0-9-]+)\/status$/.exec(new URL(req.url, "http://localhost").pathname);
        if (req.method !== "POST" || !match) {
          res.statusCode = 404;
          res.end(JSON.stringify({ ok: false, error: "POST /__findings/<id>/status" }));
          return;
        }
        const id = match[1];
        let status;
        try {
          ({ status } = await readJsonBody(req));
        } catch (error) {
          res.statusCode = 400;
          res.end(JSON.stringify({ ok: false, error: `malformed JSON body: ${error.message}` }));
          return;
        }
        if (!FINDING_STATUSES.has(status)) {
          res.statusCode = 400;
          res.end(JSON.stringify({ ok: false, error: `status must be one of ${[...FINDING_STATUSES].join(", ")}` }));
          return;
        }
        try {
          await markFindingInRulebook(id, status);
        } catch (error) {
          res.statusCode = 400;
          res.end(JSON.stringify({ ok: false, error: error.message }));
          return;
        }
        const patch = await fetch(`${EDITOR_API}/api/tables/ConsistencyFindings/rows/${encodeURIComponent(id)}`, {
          method: "PATCH",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ Status: status }),
        }).catch((error) => ({ ok: false, status: 0, text: async () => String(error.message || error) }));
        if (!patch.ok) {
          res.statusCode = 502;
          res.end(JSON.stringify({ ok: false, error: `${RULEBOOK} now says ${id} is ${status}, but the editor API refused the base-table write (HTTP ${patch.status}): ${await patch.text()}` }));
          return;
        }
        res.end(JSON.stringify({ ok: true, id, status }));
      });
    },
  };
}

// Trigger a conformance harness run (cap-conformance-harness, promoted to a
// first-class explorer feature). Same shape as findingStatusPlugin: the write
// path is a repo script, not a route bolted onto the generated API. Streams
// scripts/run-conformance.py <slug> as Server-Sent Events so the UI can show
// live progress instead of blocking on one big response; that script shells
// out to the EXISTING harness (orchestration/test-orchestrator.py — not
// reimplemented here), records ConformanceRuns/ConformanceResults rows in the
// rulebook JSON, generates the aggregate orchestration-report.html, then runs
// `effortless build` so the views pick up the new rows. This can run for a
// while (every registered substrate), so no artificial timeout is imposed.
const SLUG_RE = /^[A-Za-z0-9-]+$/;

function findDomainDir(slug) {
  for (const base of ["rulebook-examples", "toy-rulebooks"]) {
    const dir = path.join(REPO_ROOT, base, slug);
    if (dirExistsSync(dir)) return dir;
  }
  return null;
}

function dirExistsSync(dir) {
  try {
    return statSync(dir).isDirectory();
  } catch {
    return false;
  }
}

function sseSend(res, event, data) {
  res.write(`event: ${event}\ndata: ${JSON.stringify(data)}\n\n`);
}

function conformanceRunPlugin() {
  return {
    name: "erb-conformance-run",
    configureServer(server) {
      server.middlewares.use("/__conformance", (req, res) => {
        const match = /^\/([A-Za-z0-9-]+)\/run$/.exec(new URL(req.url, "http://localhost").pathname);
        if (req.method !== "POST" || !match) {
          res.setHeader("Content-Type", "application/json");
          res.statusCode = 404;
          res.end(JSON.stringify({ ok: false, error: "POST /__conformance/<slug>/run" }));
          return;
        }
        const slug = match[1];
        if (!SLUG_RE.test(slug)) {
          res.setHeader("Content-Type", "application/json");
          res.statusCode = 400;
          res.end(JSON.stringify({ ok: false, error: "slug must match [A-Za-z0-9-]+" }));
          return;
        }

        res.writeHead(200, {
          "Content-Type": "text/event-stream",
          "Cache-Control": "no-cache",
          Connection: "keep-alive",
        });

        const child = spawn("python3", ["scripts/run-conformance.py", slug], { cwd: REPO_ROOT });
        let lastLine = "";

        const forwardLines = (chunk) => {
          for (const line of chunk.toString("utf-8").split("\n")) {
            if (!line) continue;
            lastLine = line;
            sseSend(res, "log", { line });
          }
        };
        child.stdout.on("data", forwardLines);
        child.stderr.on("data", forwardLines);

        child.on("error", (error) => {
          sseSend(res, "done", { ok: false, error: error.message });
          res.end();
        });
        child.on("close", (code) => {
          if (code !== 0) {
            sseSend(res, "done", { ok: false, error: `run-conformance.py exited ${code}: ${lastLine}` });
            res.end();
            return;
          }
          // The script's final stdout line is the JSON summary (run_id, substrates, report_path).
          let summary = null;
          try {
            summary = JSON.parse(lastLine);
          } catch {
            // fall through with summary=null; the UI still knows it succeeded
          }
          sseSend(res, "done", { ok: true, slug, ...summary });
          res.end();
        });

        req.on("close", () => child.kill());
      });
    },
  };
}

// Serve a domain's generated orchestration-report.html / per-substrate
// substrate-report.html files read-only, so the explorer can iframe them
// without copying them. Mirrors generatedFilesPlugin's escape-check.
function conformanceReportPlugin() {
  return {
    name: "erb-conformance-report",
    configureServer(server) {
      server.middlewares.use("/__conformance-report", async (req, res) => {
        const url = new URL(req.url, "http://localhost");
        const match = /^\/([A-Za-z0-9-]+)\/(aggregate|substrate\/[A-Za-z0-9_-]+)$/.exec(url.pathname);
        if (!match) {
          res.statusCode = 404;
          res.setHeader("Content-Type", "text/plain");
          res.end("GET /__conformance-report/<slug>/aggregate or /<slug>/substrate/<name>");
          return;
        }
        const [, slug, which] = match;
        const domainDir = findDomainDir(slug);
        if (!domainDir) {
          res.statusCode = 404;
          res.setHeader("Content-Type", "text/plain");
          res.end(`no project directory found for slug ${slug}`);
          return;
        }
        const file = which === "aggregate"
          ? path.join(domainDir, "orchestration-report.html")
          : path.join(REPO_ROOT, "execution-substrates", which.slice("substrate/".length), "substrate-report.html");
        try {
          const data = await fs.readFile(file);
          res.setHeader("Content-Type", "text/html; charset=utf-8");
          res.end(data);
        } catch (error) {
          res.statusCode = error.code === "ENOENT" ? 404 : 500;
          res.setHeader("Content-Type", "text/plain");
          res.end(`${error.code === "ENOENT" ? "report not found" : "could not read report"}: ${file}\nRun a conformance run for ${slug} first.`);
        }
      });
    },
  };
}

// ---------------------------------------------------------------------------
// Corpus runs — launch and monitor scripts/run-corpus.py across the whole corpus.
//
// Deliberately NOT server-sent events. A corpus fan-out takes many minutes, and
// tying it to one HTTP response would mean closing the tab kills the run and a
// second viewer cannot watch it. Instead the runner is spawned DETACHED and owns
// its own live artifact — orchestration/corpus-runs/<run-id>/status.json, written
// atomically after every phase transition. The endpoints below just launch it and
// read that file, so reload, reattach and multiple viewers all work for free, and
// the run keeps going either way.
// ---------------------------------------------------------------------------
const CORPUS_RUNS_DIR = path.join(REPO_ROOT, "orchestration", "corpus-runs");
const RUN_ID_RE = /^corpus-\d{8}-\d{6}$/;

function json(res, status, body) {
  res.statusCode = status;
  res.setHeader("Content-Type", "application/json");
  res.end(JSON.stringify(body));
}

async function readBody(req) {
  const chunks = [];
  for await (const chunk of req) chunks.push(chunk);
  if (!chunks.length) return {};
  return JSON.parse(Buffer.concat(chunks).toString("utf-8"));
}

async function listRunIds() {
  let entries;
  try {
    entries = await fs.readdir(CORPUS_RUNS_DIR, { withFileTypes: true });
  } catch (error) {
    if (error.code === "ENOENT") return []; // no fan-out has ever been launched
    throw error;
  }
  return entries.filter((e) => e.isDirectory() && RUN_ID_RE.test(e.name)).map((e) => e.name).sort().reverse();
}

async function readStatus(runId) {
  const file = path.join(CORPUS_RUNS_DIR, runId, "status.json");
  return JSON.parse(await fs.readFile(file, "utf-8"));
}

// A detached runner outlives this dev server, so "is it still going?" cannot be
// answered from a child handle. status.json records the runner's pid; signal 0
// asks the OS whether that process is still alive without touching it.
function pidAlive(pid) {
  if (!pid) return false;
  try {
    process.kill(pid, 0);
    return true;
  } catch (error) {
    return error.code === "EPERM"; // alive, just not ours to signal
  }
}

function corpusPlugin() {
  return {
    name: "erb-corpus",
    configureServer(server) {
      server.middlewares.use("/__corpus", async (req, res) => {
        const url = new URL(req.url, "http://localhost");
        try {
          // POST /__corpus/run  { mode, kinds?, only?, note? }
          if (req.method === "POST" && url.pathname === "/run") {
            const body = await readBody(req);
            const mode = body.mode === "build-only" ? "build-only" : "full";
            const args = ["scripts/run-corpus.py", "--mode", mode];
            for (const kind of body.kinds || []) {
              if (!["root", "toy", "example"].includes(kind)) return json(res, 400, { ok: false, error: `bad kind ${kind}` });
              args.push("--kind", kind);
            }
            if (body.only?.length) {
              const bad = body.only.filter((s) => !SLUG_RE.test(s));
              if (bad.length) return json(res, 400, { ok: false, error: `bad slug(s): ${bad.join(", ")}` });
              args.push("--only", body.only.join(","));
            }
            if (body.note) args.push("--note", String(body.note).slice(0, 500));

            // Refuse to start a second fan-out while one is live: every
            // `effortless build` goes through the ssotme-proxy on :4242 and
            // concurrent builds corrupt each other.
            for (const runId of await listRunIds()) {
              const status = await readStatus(runId).catch(() => null);
              if (status && !status.finished_on && pidAlive(status.pid)) {
                return json(res, 409, { ok: false, error: `corpus run ${runId} is still going (pid ${status.pid})`, run_id: runId });
              }
            }

            const before = new Set(await listRunIds());
            const logFile = await fs.open(path.join(REPO_ROOT, "orchestration", "corpus-runs.log"), "a");
            const child = spawn("python3", args, {
              cwd: REPO_ROOT,
              detached: true,
              stdio: ["ignore", logFile.fd, logFile.fd],
            });
            child.unref();

            // The runner creates its run directory within the first moments;
            // poll briefly for the new id rather than guessing the timestamp.
            for (let i = 0; i < 60; i += 1) {
              const fresh = (await listRunIds()).filter((id) => !before.has(id));
              if (fresh.length) {
                await logFile.close();
                return json(res, 200, { ok: true, run_id: fresh[0] });
              }
              await new Promise((r) => setTimeout(r, 250));
            }
            await logFile.close();
            return json(res, 500, { ok: false, error: "run-corpus.py did not create a run directory within 15s — check orchestration/corpus-runs.log" });
          }

          // GET /__corpus/runs — every fan-out on disk, newest first.
          if (req.method === "GET" && url.pathname === "/runs") {
            const runs = [];
            for (const runId of await listRunIds()) {
              const status = await readStatus(runId).catch(() => null);
              if (!status) continue;
              runs.push({
                run_id: status.run_id, mode: status.mode, note: status.note,
                started_on: status.started_on, finished_on: status.finished_on,
                target_count: status.target_count,
                live: !status.finished_on && pidAlive(status.pid),
              });
            }
            return json(res, 200, { ok: true, runs });
          }

          // GET /__corpus/latest | /__corpus/<run-id> — the full live status doc.
          const match = /^\/(latest|corpus-\d{8}-\d{6})$/.exec(url.pathname);
          if (req.method === "GET" && match) {
            let runId = match[1];
            if (runId === "latest") {
              const ids = await listRunIds();
              if (!ids.length) return json(res, 200, { ok: true, status: null });
              runId = ids[0];
            }
            const status = await readStatus(runId);
            return json(res, 200, { ok: true, status: { ...status, live: !status.finished_on && pidAlive(status.pid) } });
          }

          // POST /__corpus/<run-id>/stop — SIGTERM the detached runner.
          const stopMatch = /^\/(corpus-\d{8}-\d{6})\/stop$/.exec(url.pathname);
          if (req.method === "POST" && stopMatch) {
            const status = await readStatus(stopMatch[1]);
            if (status.finished_on || !pidAlive(status.pid)) {
              return json(res, 409, { ok: false, error: "that run is not going" });
            }
            process.kill(status.pid, "SIGTERM");
            return json(res, 200, { ok: true, run_id: status.run_id, signalled: status.pid });
          }

          return json(res, 404, { ok: false, error: "POST /__corpus/run | GET /__corpus/runs | GET /__corpus/latest | GET /__corpus/<run-id> | POST /__corpus/<run-id>/stop" });
        } catch (error) {
          return json(res, error.code === "ENOENT" ? 404 : 500, { ok: false, error: error.message });
        }
      });
    },
  };
}

export default defineConfig({
  plugins: [healthProbePlugin(), generatedFilesPlugin(), findingStatusPlugin(), conformanceRunPlugin(), conformanceReportPlugin(), corpusPlugin()],
  server: {
    proxy: {
      "/api": { target: EDITOR_API, changeOrigin: true },
    },
  },
});
