// ---------------------------------------------------------------------------
// control.ts — the control plane (Reset / Rebuild / Rebuild-from-X).
//
// server.ts is about reading and writing ROWS. This file is about running
// BUILDS. The login page's control buttons each shell out to an orchestration
// step and stream its stdout/stderr back to the browser as Server-Sent Events,
// so the user watches the rebuild happen live (and sees the real error, in full,
// if a step fails — never a swallowed exit code; see CLAUDE.md "Avoid Silent
// Fallbacks").
//
// The three actions, in terms of the engine-owned stores (Model B; see
// dal/store.ts) and the rulebook hub:
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
import type { Express, Request, Response } from "express";
import { isValidBackend } from "./dal/index";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

// Anchors. The backend lives at app/backend/; the project root (where
// effortless.json + start.sh live) is two levels up; postgres-bootstrap and the
// export CLI hang off known fixed paths under it.
const BACKEND_DIR = __dirname;
const PROJECT_ROOT = path.join(__dirname, "..", "..");
const EXPORT_CLI = path.join(BACKEND_DIR, "dal", "export-cli.ts");
const INIT_DB = path.join(PROJECT_ROOT, "postgres-bootstrap", "init-db.sh");
// The rulebook, relative to the project root, for the git-restore Reset step.
const RULEBOOK_REL = path.join(
  "effortless-rulebook",
  "talismans-special-solutions-rulebook.json"
);

// One step in a build sequence: a labeled child-process invocation, optionally
// with a working directory and extra environment overlaid on process.env.
interface Step {
  label: string;
  cmd: string;
  args: string[];
  cwd?: string;
  env?: NodeJS.ProcessEnv;
}

// The terminal/log events we stream over SSE.
type SseEvent =
  | { type: "step"; label: string }
  | { type: "log"; line: string }
  | { type: "done"; action: string }
  | { type: "error"; action: string; error: string };

// A planned sequence: the ordered steps plus the action id reported to the UI.
interface Plan {
  action: string;
  steps: Step[];
}

// --- SSE plumbing ----------------------------------------------------------
// One response, many events. We send {type:"log"} lines as a step runs, a
// {type:"step"} marker when each step starts, and exactly one terminal event —
// {type:"done"} on success or {type:"error"} with the failing step + exit code.
// We NEVER end with "done" if a step failed; the failure is the message.
function sse(res: Response): { send: (obj: SseEvent) => void } {
  res.writeHead(200, {
    "Content-Type": "text/event-stream",
    "Cache-Control": "no-cache, no-transform",
    Connection: "keep-alive",
    "X-Accel-Buffering": "no", // disable proxy buffering so events flush live
  });
  // An initial comment flushes headers immediately past any buffering layer.
  res.write(": open\n\n");
  const send = (obj: SseEvent) => res.write(`data: ${JSON.stringify(obj)}\n\n`);
  return { send };
}

// Run one command, streaming stdout+stderr as {type:"log"} events. Resolves on
// exit 0, REJECTS on any non-zero exit or spawn error — so the caller's chain
// stops at the first failure and we report it verbatim. No exit code is ever
// swallowed.
function runStep(
  send: (obj: SseEvent) => void,
  { label, cmd, args, cwd, env }: Step
): Promise<void> {
  return new Promise<void>((resolve, reject) => {
    send({ type: "step", label });
    send({ type: "log", line: `$ ${cmd} ${args.join(" ")}` });
    const child = spawn(cmd, args, {
      cwd: cwd || PROJECT_ROOT,
      env: { ...process.env, ...(env || {}) },
    });
    const pump = (buf: Buffer) => {
      for (const line of buf.toString().split(/\r?\n/)) {
        if (line.length) send({ type: "log", line });
      }
    };
    child.stdout.on("data", pump);
    child.stderr.on("data", pump);
    child.on("error", (e: Error) =>
      reject(new Error(`${label}: failed to start (${e.message})`))
    );
    child.on("close", (code: number | null) => {
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
async function runSequence(
  res: Response,
  action: string,
  steps: Step[]
): Promise<void> {
  const { send } = sse(res);
  try {
    for (const step of steps) await runStep(send, step);
    send({ type: "done", action });
  } catch (e) {
    // Surface the REAL failure — failing step name + message. The browser shows
    // it; we do not pretend the rebuild succeeded.
    send({ type: "error", action, error: (e as Error).message });
  } finally {
    res.end();
  }
}

// --- the named step builders ----------------------------------------------
// Each returns a {label, cmd, args, cwd, env} descriptor for runStep.

// export-cli is a .ts file. The server runs under `tsx` (see start.sh: `npx tsx
// server.ts`), but `process.execPath` is the underlying bare `node` binary — and
// plain node v20 cannot load `.ts` (ERR_UNKNOWN_FILE_EXTENSION). So spawn the
// CLI through the same node WITH the tsx loader registered (`--import tsx`),
// exactly the way start.sh runs the server. Resolving tsx by name lets node find
// it in the backend's node_modules.
const node = (verb: string, label: string): Step => ({
  label,
  cmd: process.execPath, // the node binary running this server…
  args: ["--import", "tsx", EXPORT_CLI, verb], // …+ tsx so it can load the .ts CLI
  cwd: BACKEND_DIR,
});

const stepExportFrom = (engine: string): Step =>
  node(engine, `export ${engine} store → rulebook`);
const stepSeedDbJson = (): Step =>
  node("seed-dbjson", "reseed db.json from rulebook");

const stepEffortlessBuild = (): Step => ({
  label: "effortless build",
  cmd: "effortless",
  args: ["build"],
  cwd: PROJECT_ROOT,
});

// init-db.sh with drop_all semantics is what the dev loop uses (see init-db.sh /
// CLAUDE.md "every build resets it"). We run it as the Postgres reseed step.
const stepInitDb = (): Step => ({
  label: "reseed Postgres store (init-db.sh)",
  cmd: "bash",
  args: [INIT_DB],
  cwd: PROJECT_ROOT,
});

// Reset's backup story (user's choice): restore the rulebook to its last
// committed state via git. `git checkout -- <path>` only touches that one file;
// it leaves every other working-tree change alone. We run from PROJECT_ROOT
// (inside the repo) and let git resolve the path relative to the repo root.
const stepGitRestoreRulebook = (): Step => ({
  label: "git-restore rulebook to committed baseline",
  cmd: "git",
  args: ["checkout", "--", RULEBOOK_REL],
  cwd: PROJECT_ROOT,
});

// --- routes ----------------------------------------------------------------
export function registerControlRoutes(app: Express): void {
  // The actions the login page renders a button for. Kept here so the UI never
  // hardcodes the list; it asks the server what's available.
  app.get("/api/control/actions", (_req: Request, res: Response) => {
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
  app.post("/api/control/reset", (_req: Request, res: Response) => {
    runSequence(res, "reset", [
      stepGitRestoreRulebook(),
      stepEffortlessBuild(),
      stepInitDb(),
      stepSeedDbJson(),
    ]);
  });

  // Rebuild-from-X: round-trip engine X's current store through the hub.
  // /api/control/rebuild-from/reasoner or /postgres.
  app.post("/api/control/rebuild-from/:engine", (req: Request, res: Response) => {
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
  app.post("/api/control/rebuild", (_req: Request, res: Response) => {
    runSequence(res, "rebuild", [
      stepEffortlessBuild(),
      stepInitDb(),
      stepSeedDbJson(),
    ]);
  });

  // -------------------------------------------------------------------------
  // Sync: "store FROM is authoritative — make the other two match it."
  //
  //   POST /api/control/sync   { from: "reasoner"|"postgres"|"rulebook" }
  //
  // ONE push per node (the login page offers exactly one button per store):
  // "make THIS store the truth everywhere." There is no partial-target fan-out,
  // because data only round-trips THROUGH the rulebook hub (no direct engine↔
  // engine copy — that would bypass the SSoT). A leg pushed to the other leg
  // necessarily rewrites the hub on the way, so any push lands all three stores
  // locked anyway; a partial target set was a distinction without a difference.
  //
  // The push is composed from the same three primitives the named actions use:
  //   1. export FROM → rulebook   (skip when from === "rulebook": the hub already
  //                                holds the authoritative rows)
  //   2. effortless build         (regenerate every substrate from the now-
  //                                authoritative rulebook)
  //   3. reseed the OTHER leg      (init-db.sh for postgres / seed-dbjson for the
  //                                reasoner). The source is never reseeded from
  //                                itself (it already IS authoritative; a redundant
  //                                round-trip could only differ if the export/build
  //                                lost something — which we'd want to SEE, not
  //                                paper over), and the rulebook "reseeds" itself
  //                                purely by being the build input. The next
  //                                /api/triangle read proves convergence.
  //
  // `targets` is still accepted for back-compat / explicit callers, but the UI
  // omits it now; planSync defaults it to "the other two stores."
  app.post("/api/control/sync", (req: Request, res: Response) => {
    const from = (req.body && req.body.from) || "";
    const targets = (req.body && Array.isArray(req.body.targets)) ? req.body.targets : null;
    let plan: Plan;
    try {
      plan = planSync(from, targets);
    } catch (e) {
      return res.status(400).json({ error: (e as Error).message });
    }
    runSequence(res, plan.action, plan.steps);
  });
}

// planSync — PURE step planner for /api/control/sync, factored out so it can be
// reasoned about and tested without executing a build. Given an authoritative
// source store, it returns the ordered list of step descriptors (export? →
// build → reseed the other leg) plus an action id. `targetsIn` is optional: the
// one-push-per-node UI omits it, so it defaults to "the other two stores" (make
// FROM the truth everywhere). An explicit list is still honored for back-compat.
// Throws on invalid input (the route turns that into a 400).
export function planSync(from: string, targetsIn?: string[]): Plan {
  const STORES = new Set(["rulebook", "reasoner", "postgres"]);
  if (!STORES.has(from)) {
    throw new Error(`sync 'from' must be one of rulebook|reasoner|postgres (got '${from}')`);
  }
  // Default: the other two stores — "make FROM authoritative everywhere." Every
  // push round-trips through the hub, so this always converges all three.
  const targets = (Array.isArray(targetsIn) && targetsIn.length)
    ? targetsIn.slice()
    : [...STORES].filter((s) => s !== from);
  for (const t of targets) {
    if (!STORES.has(t)) throw new Error(`sync target '${t}' is not a known store`);
  }

  const steps: Step[] = [];
  const exporting = from === "reasoner" || from === "postgres";
  if (exporting) {
    // Promote the authoritative engine's rows into the hub. Any push that moves a
    // leg's edits anywhere REWRITES the rulebook — so the rulebook is implicitly a
    // target; reflect that so the UI's "what got touched" is true.
    steps.push(stepExportFrom(from));
    if (!targets.includes("rulebook")) targets.push("rulebook");
  }
  // Always rebuild so every substrate is regenerated from the (possibly just-
  // updated) rulebook before we reseed any target from it.
  steps.push(stepEffortlessBuild());

  // Reseed each requested target EXCEPT the source (already authoritative) and
  // the rulebook (it "reseeds" itself purely by being the build input).
  const reseed = targets.filter((t) => t !== from && t !== "rulebook");
  if (reseed.includes("postgres")) steps.push(stepInitDb());
  if (reseed.includes("reasoner")) steps.push(stepSeedDbJson());

  return { action: `sync-${from}->${targets.join("+")}`, steps };
}
