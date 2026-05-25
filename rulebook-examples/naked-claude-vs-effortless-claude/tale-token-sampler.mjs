#!/usr/bin/env node
// tale-token-sampler.mjs — background ccusage poller.
// Every <interval> seconds, runs `cc-usage --json` from inside the
// experiment dir, parses the JSON, and appends one line to .tale/tokens.jsonl.
//
// Usage:
//   node tale-token-sampler.mjs --dir <experiment-dir> --interval <seconds>

import { spawnSync } from "node:child_process";
import { appendFileSync, existsSync, mkdirSync } from "node:fs";
import { join } from "node:path";

// ---------- arg parsing ----------
const argv = process.argv.slice(2);
function getFlag(name) {
    const i = argv.indexOf(name);
    return i >= 0 ? argv[i + 1] : null;
}

const expDir = getFlag("--dir");
const interval = parseInt(getFlag("--interval") || "30", 10);

if (!expDir || !existsSync(expDir)) {
    console.error(`[sampler] --dir is required and must exist (got: ${expDir})`);
    process.exit(2);
}
if (!Number.isFinite(interval) || interval <= 0) {
    console.error(`[sampler] invalid --interval: ${interval}`);
    process.exit(2);
}

const taleDir = join(expDir, ".tale");
mkdirSync(taleDir, { recursive: true });
const jsonlPath = join(taleDir, "tokens.jsonl");

const startTime = Date.now();
let stopping = false;

function logErr(msg) {
    console.error(`[sampler] ${new Date().toISOString()} ${msg}`);
}

function sampleOnce() {
    const t0 = Date.now();
    const r = spawnSync("cc-usage", ["--json"], {
        cwd: expDir,
        encoding: "utf8",
        timeout: 15000,
        killSignal: "SIGKILL",
    });

    const wallclock = new Date().toISOString();
    const elapsed_s = Math.round((t0 - startTime) / 1000);

    if (r.error) {
        logErr(`cc-usage spawn error: ${r.error.message}`);
        return;
    }
    if (r.signal) {
        logErr(`cc-usage killed by signal ${r.signal} (timeout?)`);
        return;
    }
    if (r.status !== 0) {
        logErr(`cc-usage exit ${r.status}: ${(r.stderr || "").trim()}`);
        return;
    }

    let parsed;
    try {
        parsed = JSON.parse(r.stdout);
    } catch (e) {
        logErr(`cc-usage JSON parse error: ${e.message}`);
        return;
    }

    const line = JSON.stringify({
        t: wallclock,
        elapsed_s,
        total: parsed.total ?? null,
    }) + "\n";

    try {
        appendFileSync(jsonlPath, line);
    } catch (e) {
        logErr(`append failed: ${e.message}`);
    }
}

function loop() {
    if (stopping) return;
    const t0 = Date.now();
    sampleOnce();
    const elapsed = Date.now() - t0;
    const sleep = Math.max(0, interval * 1000 - elapsed);
    setTimeout(loop, sleep);
}

function shutdown(sig) {
    stopping = true;
    console.error(`[sampler] received ${sig}, exiting`);
    process.exit(0);
}
process.on("SIGTERM", () => shutdown("SIGTERM"));
process.on("SIGINT",  () => shutdown("SIGINT"));

console.error(`[sampler] starting, dir=${expDir} interval=${interval}s`);
loop();
