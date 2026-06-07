// ---------------------------------------------------------------------------
// control.js — the control plane (Reset / Rebuild / Rebuild-from-X).
//
// server.js is about reading and writing ROWS. This file is about running
// BUILDS. The login page's control buttons each shell out to an orchestration
// step and stream its stdout/stderr back to the browser as Server-Sent Events,
// so the user watches the rebuild happen live (and sees the real error, in full,
// if a step fails — never a swallowed exit code; see CLAUDE.md "Avoid Silent
// Fallbacks").
//
// The three actions, in terms of the engine-owned stores (Model B; see
// dal/store.js) and the rulebook hub:
//
//   Reset            Return the WHOLE demo to its pristine starting point. The
//                    user chose git as the backup: we `git checkout` the
//                    rulebook back to its last committed state (discarding any
//                    Rebuild-from-X data edits AND any hand-edits), rebuild every
//                    substrate from that pristine rulebook, then reseed both
//                    stores. Safe to run anytime to snap back to known-good.
//
//   Rebuild-from-X   Round-trip engine X's CURRENT store through the hub:
//                    export X's rows INTO the rulebook (export-cli X) →
//                    `effortless build` (regenerate every substrate from the
//                    now-updated rulebook) → reseed BOTH stores from it
//                    (init-db.sh + export-cli seed-dbjson). This is how an edit
//                    made in one engine becomes the shared truth and relocks the
//                    other engine. The rulebook IS written here — by design.
//
//   Rebuild          `effortless build` from the rulebook exactly as it sits on
//                    disk (no export step), then reseed both stores. Used after
//                    a hand-edit to the rulebook JSON.
//
// Every action ends with BOTH stores reseeded from the one rulebook, so the
// login page's drift indicator goes quiet (the whole point of the round-trip).
// ---------------------------------------------------------------------------
import { spawn } from "node:child_process";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { isValidBackend } from "./dal/index.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

// Anchors. The backend lives at app/backend/; the project root (where
// effortless.json + start.sh live) is two levels up; postgres-bootstrap and the
// export CLI hang off known fixed paths under it.
const BACKEND_DIR = __dirname;
const PROJECT_ROOT = path.join(__dirname, "..", "..");
const EXPORT_CLI = path.join(BACKEND_DIR, "dal", "export-cli.js");
const INIT_DB = path.join(PROJECT_ROOT, "postgres-bootstrap", "init-db.sh");
// The rulebook, relative to the project root, for the git-restore Reset step.
const RULEBOOK_REL = path.join(
  "effortless-rulebook",
  "talisman-basic-rulebook.json"
);

// --- SSE plumbing ----------------------------------------------------------
// One response, many events. We send {type:"log"} lines as a step runs, a
// {type:"step"} marker when each step starts, and exactly one terminal event —
// {type:"done"} on success or {type:"error"} with the failing step + exit code.
// We NEVER end with "done" if a step failed; the failure is the message.
function sse(res) {
  res.writeHead(200, {
    "Content-Type": "text/event-stream",
    "Cache-Control": "no-cache, no-transform",
    Connection: "keep-alive",
    "X-Accel-Buffering": "no", // disable proxy buffering so events flush live
  });
  // An initial comment flushes headers immediately past any buffering layer.
  res.write(": open\n\n");
  const send = (obj) => res.write(`data: ${JSON.stringify(obj)}\n\n`);
  return { send };
}

// Run one command, streaming stdout+stderr as {type:"log"} events. Resolves on
// exit 0, REJECTS on any non-zero exit or spawn error — so the caller's chain
// stops at the first failure and we report it verbatim. No exit code is ever
// swallowed.
function runStep(send, { label, cmd, args, cwd, env }) {
  return new Promise((resolve, reject) => {
    send({ type: "step", label });
    send({ type: "log", line: `$ ${cmd} ${args.join(" ")}` });
    const child = spawn(cmd, args, {
      cwd: cwd || PROJECT_ROOT,
      env: { ...process.env, ...(env || {}) },
    });
    const pump = (buf) => {
      for (const line of buf.toString().split(/\r?\n/)) {
        if (line.length) send({ type: "log", line });
      }
    };
    child.stdout.on("data", pump);
    child.stderr.on("data", pump);
    child.on("error", (e) =>
      reject(new Error(`${label}: failed to start (${e.message})`))
    );
    child.on("close", (code) => {
      if (code === 0) {
        send({ type: "log", line: `✓ ${label}` });
        resolve();
      } else {
        reject(new Error(`${label}: exited with code ${code}`));
      }
    });
  });
}

// Run a list of steps in order over an open SSE stream, ending with exactly one
// terminal event. A rejected step stops the chain and surfaces as {type:"error"}.
async function runSequence(res, action, steps) {
  const { send } = sse(res);
  try {
    for (const step of steps) await runStep(send, step);
    send({ type: "done", action });
  } catch (e) {
    // Surface the REAL failure — failing step name + message. The browser shows
    // it; we do not pretend the rebuild succeeded.
    send({ type: "error", action, error: e.message });
  } finally {
    res.end();
  }
}

// --- the named step builders ----------------------------------------------
// Each returns a {label, cmd, args, cwd, env} descriptor for runStep.

const node = (verb, label) => ({
  label,
  cmd: process.execPath, // the same node that's running this server
  args: [EXPORT_CLI, verb],
  cwd: BACKEND_DIR,
});

const stepExportFrom = (engine) =>
  node(engine, `export ${engine} store → rulebook`);
const stepSeedDbJson = () => node("seed-dbjson", "reseed db.json from rulebook");

const stepEffortlessBuild = () => ({
  label: "effortless build",
  cmd: "effortless",
  args: ["build"],
  cwd: PROJECT_ROOT,
});

// init-db.sh with drop_all semantics is what the dev loop uses (see init-db.sh /
// CLAUDE.md "every build resets it"). We run it as the Postgres reseed step.
const stepInitDb = () => ({
  label: "reseed Postgres store (init-db.sh)",
  cmd: "bash",
  args: [INIT_DB],
  cwd: PROJECT_ROOT,
});

// Reset's backup story (user's choice): restore the rulebook to its last
// committed state via git. `git checkout -- <path>` only touches that one file;
// it leaves every other working-tree change alone. We run from PROJECT_ROOT
// (inside the repo) and let git resolve the path relative to the repo root.
const stepGitRestoreRulebook = () => ({
  label: "git-restore rulebook to committed baseline",
  cmd: "git",
  args: ["checkout", "--", RULEBOOK_REL],
  cwd: PROJECT_ROOT,
});

// --- routes ----------------------------------------------------------------
export function registerControlRoutes(app) {
  // The actions the login page renders a button for. Kept here so the UI never
  // hardcodes the list; it asks the server what's available.
  app.get("/api/control/actions", (_req, res) => {
    res.json({
      actions: [
        { id: "reset", method: "POST", path: "/api/control/reset",
          label: "Reset to pristine baseline",
          desc: "git-restore the rulebook, rebuild both engines, reseed both stores." },
        { id: "rebuild", method: "POST", path: "/api/control/rebuild",
          label: "Rebuild",
          desc: "Build from the rulebook as-is, then reseed both stores." },
        { id: "rebuild-from-reasoner", method: "POST", path: "/api/control/rebuild-from/reasoner",
          label: "Rebuild from Reasoner",
          desc: "Export db.json → rulebook → build → reseed both (Postgres overwritten)." },
        { id: "rebuild-from-postgres", method: "POST", path: "/api/control/rebuild-from/postgres",
          label: "Rebuild from Postgres",
          desc: "Export Postgres tables → rulebook → build → reseed both (reasoner overwritten)." },
      ],
    });
  });

  // Reset: restore the rulebook to its committed baseline (git), rebuild every
  // substrate from that pristine rulebook, then reseed BOTH stores. This is the
  // "snap the whole demo back to known-good" button. The git step discards any
  // data edits a prior Rebuild-from-X wrote into the rulebook, so we MUST
  // rebuild afterward to regenerate the SQL/OWL from the restored rulebook.
  app.post("/api/control/reset", (_req, res) => {
    runSequence(res, "reset", [
      stepGitRestoreRulebook(),
      stepEffortlessBuild(),
      stepInitDb(),
      stepSeedDbJson(),
    ]);
  });

  // Rebuild-from-X: round-trip engine X's current store through the hub.
  // /api/control/rebuild-from/reasoner or /postgres.
  app.post("/api/control/rebuild-from/:engine", (req, res) => {
    const engine = req.params.engine;
    // Validate against the SAME backend registry the rest of the server uses —
    // an unknown engine is a 400, not a guess at which store to export.
    if (!isValidBackend(engine)) {
      return res
        .status(400)
        .json({ error: `unknown engine: ${engine}` });
    }
    runSequence(res, `rebuild-from-${engine}`, [
      stepExportFrom(engine),
      stepEffortlessBuild(),
      stepInitDb(),
      stepSeedDbJson(),
    ]);
  });

  // Rebuild: build from the rulebook exactly as it sits on disk (after a
  // hand-edit), then reseed both stores. No export step — the rulebook is
  // already the intended truth.
  app.post("/api/control/rebuild", (_req, res) => {
    runSequence(res, "rebuild", [
      stepEffortlessBuild(),
      stepInitDb(),
      stepSeedDbJson(),
    ]);
  });
}
