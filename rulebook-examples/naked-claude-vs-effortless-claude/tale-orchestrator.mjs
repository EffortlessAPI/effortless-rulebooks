#!/usr/bin/env node
// tale-orchestrator.mjs — runs a matched pair of Naked vs Effortless Claude
// experiments side-by-side, with progressive phases (v1 → v2 → v3 specs).
//
// Interactive flow:
//   1. New test or continue existing?
//   2. If continuing — pick the timestamp shared by the pair.
//   3. Choose which phases to run (only forward progress allowed).
//
// State is tracked in orchestration-state.json per experiment pair.

import { execFileSync, spawn, spawnSync } from "node:child_process";
import { existsSync, mkdirSync, readFileSync, writeFileSync, copyFileSync, openSync, closeSync, readSync, unlinkSync, readdirSync, appendFileSync, statSync } from "node:fs";
import { dirname, join, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import { homedir } from "node:os";
import { createInterface } from "node:readline";

const __filename = fileURLToPath(import.meta.url);
const REPO_ROOT = dirname(resolve(__filename));

// ---------- logging ----------
const LOG_LEVELS = { DEBUG: 0, INFO: 1, WARN: 2, ERROR: 3 };
let LOG_LEVEL = LOG_LEVELS.DEBUG;

function log(level, tag, msg) {
    if (LOG_LEVELS[level] < LOG_LEVEL) return;
    const ts = new Date().toISOString().replace("T", " ").slice(0, 19);
    const prefix = `[${ts}] [${level.padEnd(5)}] [${tag}]`;
    const line = `${prefix} ${msg}`;
    if (level === "ERROR" || level === "WARN") {
        console.error(line);
    } else {
        console.log(line);
    }
}

function logDebug(tag, msg) { log("DEBUG", tag, msg); }
function logInfo(tag, msg)  { log("INFO",  tag, msg); }
function logWarn(tag, msg)  { log("WARN",  tag, msg); }
function logError(tag, msg) { log("ERROR", tag, msg); }

// ---------- interactive prompt helpers ----------
function createRl() {
    return createInterface({ input: process.stdin, output: process.stdout });
}

function ask(rl, question) {
    return new Promise(resolve => rl.question(question, resolve));
}

// ---------- config ----------
const TIMEOUT_MIN = parseInt(process.env.TALE_TIMEOUT_MIN || "45", 10);
const MODEL       = process.env.TALE_MODEL || null;
const DRY_RUN     = process.argv.includes("--dry-run");
const NO_CLAUDE   = process.argv.includes("--no-claude");
const NO_SAMPLER  = process.argv.includes("--no-sampler");

const parentDir = join(REPO_ROOT, "Naked-Claude-Experiments");

// ---------- phase / spec mapping ----------
const PHASES = [
    { phase: 1, spec: "v1_SPECIFICATION.md", tag: "phase-1-complete" },
    { phase: 2, spec: "v2_SPECIFICATION.md", tag: "phase-2-complete" },
    { phase: 3, spec: "v3_SPECIFICATION.md", tag: "phase-3-complete" },
    { phase: 4, spec: "v4_SPECIFICATION.md", tag: "phase-4-complete" },
];

// ---------- read base ids ----------
const effortlessJson = JSON.parse(readFileSync(join(REPO_ROOT, "effortless.json"), "utf8"));
function getBaseId(version) {
    const setting = (effortlessJson.ProjectSettings || []).find(p => p.Name === `${version}-base-id`);
    if (!setting) { logError("config", `No ${version}-base-id in effortless.json`); process.exit(1); }
    return setting.Value;
}

// ---------- state management ----------
function stateFilePath(ts) {
    return join(parentDir, `${ts}-state.json`);
}

function saveState(state) {
    mkdirSync(parentDir, { recursive: true });
    writeFileSync(stateFilePath(state.timestamp), JSON.stringify(state, null, 2));
}

function createFreshState(ts) {
    return {
        timestamp: ts,
        created_at: new Date().toISOString(),
        naked_dir: join(parentDir, `${ts}-naked`),
        effortless_dir: join(parentDir, `${ts}-effortless`),
        db_naked: `${ts}-naked`,
        db_effortless: `${ts}-effortless`,
        completed_phases: [],  // e.g. [1] means phase 1 is done
        phase_history: {},     // { "1": { started_at, ended_at, naked_exit, effortless_exit, ... } }
    };
}

// ---------- discover existing experiments ----------
function discoverExperiments() {
    if (!existsSync(parentDir)) return [];
    const entries = readdirSync(parentDir);

    // Find state files
    const stateFiles = entries.filter(e => e.endsWith("-state.json"));
    const experiments = [];

    for (const sf of stateFiles) {
        try {
            const state = JSON.parse(readFileSync(join(parentDir, sf), "utf8"));
            experiments.push(state);
        } catch { /* skip corrupt state files */ }
    }

    // Also discover experiments that don't have state files yet (legacy)
    const nakedDirs = entries.filter(e => e.endsWith("-naked") && !e.endsWith("-state.json"));
    for (const nd of nakedDirs) {
        const ts = nd.replace(/-naked$/, "");
        const efDir = `${ts}-effortless`;
        // Skip if we already have a state file for this timestamp
        if (experiments.some(e => e.timestamp === ts)) continue;
        // Only include if both dirs exist
        if (entries.includes(efDir)) {
            // Infer state from run.json files
            const state = createFreshState(ts);
            // Check what phases are complete by looking at run.json
            for (const flavor of ["naked", "effortless"]) {
                const dir = flavor === "naked" ? state.naked_dir : state.effortless_dir;
                const runJsonPath = join(dir, ".tale", "run.json");
                if (existsSync(runJsonPath)) {
                    try {
                        const runData = JSON.parse(readFileSync(runJsonPath, "utf8"));
                        const completedPhases = Object.keys(runData.phases || {})
                            .filter(p => runData.phases[p].exit_code === 0)
                            .map(Number);
                        // Both must have completed a phase for it to count
                        if (flavor === "naked") {
                            state._naked_phases = completedPhases;
                        } else {
                            state._effortless_phases = completedPhases;
                        }
                    } catch { /* skip */ }
                }
            }
            // Completed phases = intersection of both flavors
            if (state._naked_phases && state._effortless_phases) {
                state.completed_phases = state._naked_phases.filter(
                    p => state._effortless_phases.includes(p)
                ).sort((a, b) => a - b);
            }
            delete state._naked_phases;
            delete state._effortless_phases;
            // Save this inferred state so future runs find it
            saveState(state);
            experiments.push(state);
        }
    }

    return experiments.sort((a, b) => b.timestamp.localeCompare(a.timestamp));
}

// ---------- databases ----------
function dropAndCreateDb(dbName) {
    logInfo("db", `Dropping database '${dbName}' if exists...`);
    const drop = spawnSync("psql", ["-h", "localhost", "-U", "postgres", "-c", `DROP DATABASE IF EXISTS "${dbName}";`], {
        encoding: "utf8", timeout: 15000,
    });
    if (drop.status !== 0) {
        logWarn("db", `Drop failed (may not exist): ${(drop.stderr || "").trim()}`);
    } else {
        logInfo("db", `  ✓ Dropped '${dbName}'`);
    }

    logInfo("db", `Creating database '${dbName}'...`);
    const create = spawnSync("psql", ["-h", "localhost", "-U", "postgres", "-c", `CREATE DATABASE "${dbName}";`], {
        encoding: "utf8", timeout: 15000,
    });
    if (create.status !== 0) {
        logError("db", `Create failed: ${(create.stderr || "").trim()}`);
        return false;
    }
    logInfo("db", `  ✓ Created '${dbName}'`);
    return true;
}

// ---------- locate claude binary ----------
function findClaudeBin() {
    if (process.env.CLAUDE_BIN && existsSync(process.env.CLAUDE_BIN)) {
        return process.env.CLAUDE_BIN;
    }
    const which = spawnSync("which", ["claude"], { encoding: "utf8" });
    if (which.status === 0 && which.stdout.trim()) return which.stdout.trim();

    const extDir = join(homedir(), ".vscode", "extensions");
    if (existsSync(extDir)) {
        const candidates = readdirSync(extDir)
            .filter(n => n.startsWith("anthropic.claude-code-"))
            .sort()
            .reverse();
        for (const c of candidates) {
            const bin = join(extDir, c, "resources", "native-binary", "claude");
            if (existsSync(bin)) return bin;
        }
    }
    return null;
}

// ---------- token sampling ----------
class TokenSampler {
    constructor(expDir, label) {
        this.expDir = expDir;
        this.label = label;
        this.taleDir = join(expDir, ".tale");
        this.jsonlPath = join(this.taleDir, "tokens.jsonl");
        this.startTime = Date.now();
        this.timer = null;
        this.currentPhase = 0;
        this.sampleCount = 0;
        this.lastTotal = null;
        mkdirSync(this.taleDir, { recursive: true });
    }

    setPhase(phase) {
        this.currentPhase = phase;
        logInfo(`sampler:${this.label}`, `Phase set to ${phase}`);
    }

    start(intervalSec = 60) {
        logInfo(`sampler:${this.label}`, `Starting token sampler (interval=${intervalSec}s)`);
        this.timer = setInterval(() => this.sample(), intervalSec * 1000);
        this.sample();
    }

    stop() {
        if (this.timer) {
            clearInterval(this.timer);
            this.timer = null;
        }
        this.sample();
        logInfo(`sampler:${this.label}`, `Stopped. Total samples: ${this.sampleCount}`);
    }

    sample() {
        const t0 = Date.now();
        const r = spawnSync("cc-usage", ["--json"], {
            cwd: this.expDir,
            encoding: "utf8",
            timeout: 15000,
            killSignal: "SIGKILL",
        });

        const wallclock = new Date().toISOString();
        const elapsed_s = Math.round((t0 - this.startTime) / 1000);

        if (r.error) {
            logWarn(`sampler:${this.label}`, `cc-usage spawn error: ${r.error.message}`);
            return;
        }
        if (r.status !== 0) {
            logWarn(`sampler:${this.label}`, `cc-usage exit ${r.status}: ${(r.stderr || "").trim()}`);
            return;
        }

        let parsed;
        try {
            parsed = JSON.parse(r.stdout);
        } catch (e) {
            logWarn(`sampler:${this.label}`, `JSON parse error: ${e.message}`);
            return;
        }

        const total = parsed.total ?? null;
        const line = JSON.stringify({
            t: wallclock,
            elapsed_s,
            phase: this.currentPhase,
            total,
        }) + "\n";

        try {
            appendFileSync(this.jsonlPath, line);
            this.sampleCount++;
        } catch (e) {
            logError(`sampler:${this.label}`, `append failed: ${e.message}`);
            return;
        }

        if (total) {
            const inputTok = total.input_tokens || 0;
            const outputTok = total.output_tokens || 0;
            const cacheTok = total.cache_read_tokens || 0;
            const costUsd = total.total_cost_usd || 0;
            const delta = this.lastTotal
                ? ` (Δ input:+${inputTok - (this.lastTotal.input_tokens || 0)}, output:+${outputTok - (this.lastTotal.output_tokens || 0)})`
                : "";
            logInfo(`tokens:${this.label}`,
                `Phase ${this.currentPhase} | ` +
                `Input: ${inputTok.toLocaleString()} | Output: ${outputTok.toLocaleString()} | ` +
                `Cache: ${cacheTok.toLocaleString()} | ` +
                `Cost: $${costUsd.toFixed(4)}${delta}`
            );
            this.lastTotal = { ...total };
        }
    }

    getSampleCount() { return this.sampleCount; }

    getPhaseBreakdown() {
        try {
            const lines = readFileSync(this.jsonlPath, "utf8").split("\n").filter(l => l.trim());
            const phases = { 1: [], 2: [], 3: [], 4: [] };
            for (const line of lines) {
                const obj = JSON.parse(line);
                if (obj.phase && phases[obj.phase]) {
                    phases[obj.phase].push(obj);
                }
            }
            return phases;
        } catch { return { 1: [], 2: [], 3: [], 4: [] }; }
    }
}

// ---------- progress monitor ----------
class ProgressMonitor {
    constructor(logPath, label, phase) {
        this.logPath = logPath;
        this.label = label;
        this.phase = phase;
        this.timer = null;
        this.lastSize = 0;
        this.toolCallCount = 0;
        this.lastReportedTools = 0;
        this.startTime = Date.now();
    }

    start(intervalSec = 10) {
        this.timer = setInterval(() => this.check(), intervalSec * 1000);
    }

    stop() {
        if (this.timer) {
            clearInterval(this.timer);
            this.timer = null;
        }
    }

    check() {
        try {
            const st = statSync(this.logPath);
            if (st.size <= this.lastSize) return;

            const fd = openSync(this.logPath, "r");
            const buf = Buffer.alloc(st.size - this.lastSize);
            const bytesRead = readSync(fd, buf, 0, buf.length, this.lastSize);
            closeSync(fd);
            this.lastSize = st.size;

            const newText = buf.toString("utf8", 0, bytesRead);
            const toolMatches = newText.match(/tool_call|tool_use|function_call/gi);
            if (toolMatches) this.toolCallCount += toolMatches.length;

            if (this.toolCallCount - this.lastReportedTools >= 5) {
                const elapsed = Math.round((Date.now() - this.startTime) / 1000);
                const logKb = Math.round(st.size / 1024);
                logInfo(`progress:${this.label}`,
                    `Phase ${this.phase} | ${elapsed}s elapsed | ` +
                    `~${this.toolCallCount} tool calls | ` +
                    `log: ${logKb}KB`
                );
                this.lastReportedTools = this.toolCallCount;
            }
        } catch {
            // File may not exist yet
        }
    }
}

// ---------- build prompt for a phase ----------
function buildPrompt(flavor, phase, baseId, dbName, isFirstPhase) {
    const specFile = PHASES[phase - 1].spec;
    const lines = [];

    if (isFirstPhase) {
        lines.push(
            `The ${specFile} describes the airtable ${baseId}.`,
            `Please write this as a node/vite-react app. There is a local instance`,
            `of postgres on localhost, with a db called ${dbName}.`,
        );
    } else {
        lines.push(
            `PHASE ${phase}: The requirements have evolved. The new specification is in ${specFile}.`,
            `The airtable base is now ${baseId}.`,
            `Update the existing app to match the new specification.`,
            `The postgres database is '${dbName}' on localhost.`,
        );

        // For effortless flavor on phase 2+, tell Claude to rebuild and use the diff
        if (flavor === "effortless") {
            lines.push(
                ``,
                `EFFORTLESS UPGRADE WORKFLOW:`,
                `1. Update the baseId in effortless.json to ${baseId}:`,
                `   Run: sed -i '' 's/"Value": "app[^"]*"/"Value": "${baseId}"/' effortless.json`,
                `   (update ONLY the baseId setting, not the project name)`,
                `2. Run \`effortless build\` from the project root — this pulls the new`,
                `   rulebook from the updated Airtable base and regenerates all SQL.`,
                `3. Commit the build output: git add -A && git commit -m "effortless build: upgrade to ${specFile}"`,
                `4. Run: git diff HEAD~1 -- effortless-rulebook/effortless-rulebook.json`,
                `   This diff shows EXACTLY what changed in the schema — new tables,`,
                `   renamed fields, new calculated fields, removed columns, etc.`,
                `   This is 100% accurate and the most trustworthy input for your work.`,
                `5. Run init-db.sh to reload the database with the new schema.`,
                `6. Now update the app code to match the new schema surface.`,
                `   Only change code that touches the schema — column names, new views,`,
                `   removed tables. Never reimplement business logic that lives in the views.`,
            );
        }
    }

    lines.push(
        ``,
        `HARD CONSTRAINTS — apply to every action:`,
        `- DO NOT modify Airtable (no REST API writes, no OMNI, no Playwright).`,
        `  Treat the Airtable schema as read-only.`,
        `- DO NOT leave background processes running. No dev server left alive.`,
        ``,
        `REQUIRED DELIVERABLES:`,
        `- All source code written into the current directory.`,
        `- A start.sh at the repo root that brings the whole app up end-to-end`,
        `  (install deps, run any DB migrations, start backend + frontend).`,
        `  The grader will run ./start.sh to test the app — make it just work.`,
        ``,
        `When you are done, stop and exit. Do not leave anything running.`,
    );

    return lines.join("\n");
}

// ---------- run one phase of one flavor ----------
async function runPhase(flavor, phase, expDir, dbName, sampler) {
    const tag = `${flavor}:phase${phase}`;
    const baseId = getBaseId(`v${phase}`);
    const specSrc = join(REPO_ROOT, PHASES[phase - 1].spec);
    const isFirstPhase = phase === 1;

    logInfo(tag, `════════════════════════════════════════════════════`);
    logInfo(tag, `Starting phase ${phase} for ${flavor}`);
    logInfo(tag, `  Spec: ${specSrc}`);
    logInfo(tag, `  Base ID: ${baseId}`);
    logInfo(tag, `  DB: ${dbName}`);
    logInfo(tag, `════════════════════════════════════════════════════`);

    // Copy the spec file into the experiment dir
    if (!existsSync(specSrc)) {
        logError(tag, `Spec not found: ${specSrc}`);
        return { exitCode: 1, phase };
    }
    copyFileSync(specSrc, join(expDir, PHASES[phase - 1].spec));

    sampler.setPhase(phase);

    const prompt = buildPrompt(flavor, phase, baseId, dbName, isFirstPhase);
    logDebug(tag, `Prompt:\n${prompt}`);

    if (NO_CLAUDE) {
        logInfo(tag, `--no-claude: skipping CLI invocation for phase ${phase}`);
        return { exitCode: 0, phase };
    }

    const claudeBin = findClaudeBin();
    if (!claudeBin) {
        logError(tag, "Could not locate `claude` CLI");
        return { exitCode: 127, phase };
    }

    logInfo(tag, `Claude binary: ${claudeBin}`);
    logInfo(tag, `Launching Claude (timeout: ${TIMEOUT_MIN}min)...`);

    const taleDir = join(expDir, ".tale");
    mkdirSync(taleDir, { recursive: true });
    const claudeLogPath = join(taleDir, `claude-phase${phase}.log`);
    const claudeLogFd = openSync(claudeLogPath, "a");

    const claudeArgs = [
        "-p",
        "--bare",
        "--dangerously-skip-permissions",
        "--system-prompt-file", join(expDir, "CLAUDE.md"),
        "--add-dir", expDir,
        "--verbose",
    ];

    if (MODEL) claudeArgs.push("--model", MODEL);
    claudeArgs.push(prompt);

    const progressMonitor = new ProgressMonitor(claudeLogPath, `${flavor}:p${phase}`, phase);
    progressMonitor.start(10);

    const startTime = Date.now();
    const timeoutMs = TIMEOUT_MIN * 60 * 1000;

    const exitCode = await new Promise((resolve) => {
        const child = spawn(claudeBin, claudeArgs, {
            cwd: expDir,
            stdio: ["ignore", claudeLogFd, claudeLogFd],
        });

        const killTimer = setTimeout(() => {
            logError(tag, `Timeout after ${TIMEOUT_MIN}min — sending SIGTERM`);
            child.kill("SIGTERM");
            setTimeout(() => { try { child.kill("SIGKILL"); } catch {} }, 10000);
        }, timeoutMs);

        child.on("error", (err) => {
            clearTimeout(killTimer);
            logError(tag, `Failed to invoke claude: ${err.message}`);
            resolve(127);
        });

        child.on("close", (code, signal) => {
            clearTimeout(killTimer);
            if (signal) {
                logError(tag, `Claude killed by ${signal} (timeout after ${TIMEOUT_MIN}min)`);
                resolve(124);
            } else {
                resolve(code ?? 1);
            }
        });
    });

    closeSync(claudeLogFd);
    progressMonitor.stop();

    const elapsed = Math.round((Date.now() - startTime) / 1000);
    logInfo(tag, `Phase ${phase} complete. Exit code: ${exitCode}, elapsed: ${elapsed}s`);

    // Commit the phase results
    const phaseTag = PHASES[phase - 1].tag;
    try {
        execFileSync("git", ["add", "-A"], { cwd: expDir, stdio: "pipe" });
        execFileSync("git", ["commit", "-m", `Phase ${phase} complete (${flavor})`], { cwd: expDir, stdio: "pipe" });
        execFileSync("git", ["tag", phaseTag], { cwd: expDir, stdio: "pipe" });
        logInfo(tag, `  ✓ Committed and tagged: ${phaseTag}`);
    } catch (e) {
        logWarn(tag, `  Git commit/tag failed (maybe no changes): ${e.message}`);
    }

    sampler.sample();

    return { exitCode, phase, elapsed };
}

// ---------- scaffold an experiment directory ----------
function scaffoldExperiment(flavor, expDir, dbName) {
    const tag = `scaffold:${flavor}`;
    mkdirSync(expDir, { recursive: true });
    mkdirSync(join(expDir, ".tale"), { recursive: true });

    // Copy CLAUDE.md for this flavor
    let claudeMdSrc = join(REPO_ROOT, "test-orchestrator", `claude_${flavor}.md`);
    if (!existsSync(claudeMdSrc)) claudeMdSrc = join(REPO_ROOT, `claude_${flavor}.md`);
    if (!existsSync(claudeMdSrc)) {
        logError(tag, `claude_${flavor}.md not found`);
        return false;
    }
    const claudeMdContent = readFileSync(claudeMdSrc, "utf8").replace(/\{\{DB_NAME\}\}/g, dbName);
    writeFileSync(join(expDir, "CLAUDE.md"), claudeMdContent);
    writeFileSync(join(expDir, ".gitignore"), ".tale/\nnode_modules/\n");

    // Git init
    try {
        execFileSync("git", ["init", "-q"], { cwd: expDir, stdio: "pipe" });
        execFileSync("git", ["add", "-A"], { cwd: expDir, stdio: "pipe" });
        execFileSync("git", ["commit", "-q", "-m", `scaffold: ${flavor} experiment`], { cwd: expDir, stdio: "pipe" });
        logInfo(tag, `  ✓ Git initialized`);
    } catch (e) {
        logWarn(tag, `  Git init failed: ${e.message}`);
    }

    return true;
}

// ---------- run phases for both flavors in parallel ----------
async function runPhasePair(state, phase, samplers) {
    const tag = `orchestrator:phase${phase}`;
    logInfo(tag, `╔══════════════════════════════════════════════════════╗`);
    logInfo(tag, `║  Running Phase ${phase} for BOTH flavors in parallel      ║`);
    logInfo(tag, `╚══════════════════════════════════════════════════════╝`);

    const isFirstPhase = phase === 1;

    // For phase 1, create fresh DBs
    if (isFirstPhase && !state.completed_phases.includes(1)) {
        logInfo(tag, "Preparing databases...");
        if (!dropAndCreateDb(state.db_naked)) {
            logError(tag, `Failed to create ${state.db_naked}`);
            return null;
        }
        if (!dropAndCreateDb(state.db_effortless)) {
            logError(tag, `Failed to create ${state.db_effortless}`);
            return null;
        }
    }

    // Write run metadata
    for (const [flavor, dir] of [["naked", state.naked_dir], ["effortless", state.effortless_dir]]) {
        const taleDir = join(dir, ".tale");
        mkdirSync(taleDir, { recursive: true });
        const runMeta = {
            flavor,
            date: state.timestamp,
            started_at: new Date().toISOString(),
            experiment_dir: dir,
            database: flavor === "naked" ? state.db_naked : state.db_effortless,
            timeout_min: TIMEOUT_MIN,
            model: MODEL || "default",
            phases: {},
        };
        // Merge with existing run.json if present
        const runJsonPath = join(taleDir, "run.json");
        if (existsSync(runJsonPath)) {
            try {
                const existing = JSON.parse(readFileSync(runJsonPath, "utf8"));
                runMeta.phases = existing.phases || {};
            } catch { /* start fresh */ }
        }
        writeFileSync(runJsonPath, JSON.stringify(runMeta, null, 2));
    }

    const phaseStart = new Date().toISOString();

    // Launch both in parallel
    const [nakedResult, effortlessResult] = await Promise.all([
        runPhase("naked", phase, state.naked_dir, state.db_naked, samplers.naked),
        runPhase("effortless", phase, state.effortless_dir, state.db_effortless, samplers.effortless),
    ]);

    const phaseEnd = new Date().toISOString();

    // Update state
    if (!state.phase_history) state.phase_history = {};
    state.phase_history[phase] = {
        started_at: phaseStart,
        ended_at: phaseEnd,
        naked_exit: nakedResult.exitCode,
        naked_elapsed: nakedResult.elapsed || 0,
        effortless_exit: effortlessResult.exitCode,
        effortless_elapsed: effortlessResult.elapsed || 0,
    };

    // Mark phase complete (even if one failed — it ran)
    if (!state.completed_phases.includes(phase)) {
        state.completed_phases.push(phase);
        state.completed_phases.sort((a, b) => a - b);
    }
    saveState(state);

    // Update run.json files
    for (const [, dir, result] of [
        ["naked", state.naked_dir, nakedResult],
        ["effortless", state.effortless_dir, effortlessResult],
    ]) {
        const runJsonPath = join(dir, ".tale", "run.json");
        try {
            const runMeta = JSON.parse(readFileSync(runJsonPath, "utf8"));
            runMeta.phases[phase] = {
                started_at: phaseStart,
                ended_at: phaseEnd,
                exit_code: result.exitCode,
                elapsed_s: result.elapsed || 0,
            };
            runMeta.ended_at = phaseEnd;
            writeFileSync(runJsonPath, JSON.stringify(runMeta, null, 2));
        } catch { /* skip */ }
    }

    return { nakedResult, effortlessResult };
}

// ---------- compute timestamp ----------
function nowTimestamp() {
    const d = new Date();
    const pad = n => String(n).padStart(2, "0");
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}-${pad(d.getHours())}${pad(d.getMinutes())}`;
}

// ---------- interactive menu ----------
async function interactiveMenu() {
    const rl = createRl();

    console.log("");
    console.log("╔══════════════════════════════════════════════════════════════╗");
    console.log("║  Tale of Two Claudes — Orchestrator                         ║");
    console.log("╠══════════════════════════════════════════════════════════════╣");
    console.log("║  Every test runs BOTH naked + effortless in parallel.       ║");
    console.log("║  Phases always progress forward (1 → 2 → 3 → 4).           ║");
    console.log("╚══════════════════════════════════════════════════════════════╝");
    console.log("");

    const experiments = discoverExperiments();
    const continuable = experiments.filter(e => e.completed_phases.length > 0 && e.completed_phases.length < PHASES.length);

    // Step 1: New or continue?
    let choice;
    if (continuable.length === 0) {
        console.log("  No existing experiments to continue. Starting fresh.\n");
        choice = "new";
    } else {
        console.log("  (N) Start a NEW test");
        console.log("  (C) Continue an existing test\n");
        const ans = (await ask(rl, "  Choice [N/C]: ")).trim().toUpperCase();
        choice = ans === "C" ? "continue" : "new";
        console.log("");
    }

    let state;

    if (choice === "continue") {
        // Step 2: Pick the experiment
        console.log("  Existing experiments:\n");
        for (let i = 0; i < continuable.length; i++) {
            const e = continuable[i];
            const phases = e.completed_phases.join(", ");
            console.log(`    ${i + 1}) ${e.timestamp}  — completed phases: [${phases}]`);
        }
        console.log("");
        const pickStr = (await ask(rl, "  Pick experiment number: ")).trim();
        const pick = parseInt(pickStr, 10) - 1;
        if (pick < 0 || pick >= continuable.length) {
            logError("menu", "Invalid selection");
            rl.close();
            process.exit(1);
        }
        state = continuable[pick];
        console.log(`\n  Selected: ${state.timestamp}\n`);
    } else {
        // New experiment
        const ts = nowTimestamp();
        state = createFreshState(ts);
        console.log(`  New experiment: ${ts}\n`);
    }

    // Step 3: Which phases to run?
    const maxCompleted = state.completed_phases.length > 0
        ? Math.max(...state.completed_phases)
        : 0;
    const nextPhase = maxCompleted + 1;

    if (nextPhase > PHASES.length) {
        console.log(`  All ${PHASES.length} phases are already complete for this experiment!`);
        rl.close();
        process.exit(0);
    }

    const availableRanges = [];
    for (let end = nextPhase; end <= PHASES.length; end++) {
        const range = [];
        for (let p = nextPhase; p <= end; p++) range.push(p);
        availableRanges.push(range);
    }

    console.log("  Available phase runs:\n");
    for (let i = 0; i < availableRanges.length; i++) {
        const r = availableRanges[i];
        const label = r.length === 1 ? `Phase ${r[0]} only` : `Phases ${r[0]} → ${r[r.length - 1]}`;
        console.log(`    ${i + 1}) ${label}`);
    }
    console.log("");
    const rangeStr = (await ask(rl, "  Pick range: ")).trim();
    const rangeIdx = parseInt(rangeStr, 10) - 1;
    if (rangeIdx < 0 || rangeIdx >= availableRanges.length) {
        logError("menu", "Invalid selection");
        rl.close();
        process.exit(1);
    }
    const phasesToRun = availableRanges[rangeIdx];

    rl.close();

    return { state, phasesToRun };
}

// ---------- main ----------
async function main() {
    const { state, phasesToRun } = await interactiveMenu();

    console.log("");
    logInfo("orchestrator", `╔══════════════════════════════════════════════════════════════╗`);
    logInfo("orchestrator", `║  Experiment: ${state.timestamp.padEnd(47)} ║`);
    logInfo("orchestrator", `║  Phases:     ${phasesToRun.join(" → ").padEnd(47)} ║`);
    logInfo("orchestrator", `║  Timeout:    ${(TIMEOUT_MIN + "min per phase").padEnd(47)} ║`);
    logInfo("orchestrator", `║  Naked DB:   ${state.db_naked.padEnd(47)} ║`);
    logInfo("orchestrator", `║  Effort DB:  ${state.db_effortless.padEnd(47)} ║`);
    if (MODEL) logInfo("orchestrator", `║  Model:      ${MODEL.padEnd(47)} ║`);
    logInfo("orchestrator", `║  Naked dir:  ${state.naked_dir.slice(-47).padEnd(47)} ║`);
    logInfo("orchestrator", `║  Effort dir: ${state.effortless_dir.slice(-47).padEnd(47)} ║`);
    logInfo("orchestrator", `╚══════════════════════════════════════════════════════════════╝`);
    console.log("");

    if (DRY_RUN) {
        logInfo("orchestrator", "--dry-run: stopping here.");
        process.exit(0);
    }

    // Scaffold if this is a brand-new experiment (no phases completed yet)
    if (state.completed_phases.length === 0) {
        logInfo("orchestrator", "Scaffolding experiment directories...");
        mkdirSync(parentDir, { recursive: true });
        if (!scaffoldExperiment("naked", state.naked_dir, state.db_naked)) {
            process.exit(1);
        }
        if (!scaffoldExperiment("effortless", state.effortless_dir, state.db_effortless)) {
            process.exit(1);
        }
        saveState(state);
        console.log("");
    }

    // Start token samplers
    const samplers = {
        naked: new TokenSampler(state.naked_dir, "naked"),
        effortless: new TokenSampler(state.effortless_dir, "effortless"),
    };
    if (!NO_SAMPLER) {
        samplers.naked.start(30);
        samplers.effortless.start(30);
    }

    // Run each phase sequentially, both flavors in parallel per phase
    const allResults = [];
    for (const phase of phasesToRun) {
        const result = await runPhasePair(state, phase, samplers);
        if (!result) {
            logError("orchestrator", `Phase ${phase} setup failed. Aborting.`);
            break;
        }
        allResults.push({ phase, ...result });

        if (result.nakedResult.exitCode !== 0) {
            logWarn("orchestrator", `Phase ${phase} naked exited ${result.nakedResult.exitCode}, continuing...`);
        }
        if (result.effortlessResult.exitCode !== 0) {
            logWarn("orchestrator", `Phase ${phase} effortless exited ${result.effortlessResult.exitCode}, continuing...`);
        }
    }

    // Stop samplers
    samplers.naked.stop();
    samplers.effortless.stop();

    // ---------- final summary ----------
    console.log("");
    logInfo("orchestrator", `╔══════════════════════════════════════════════════════════════╗`);
    logInfo("orchestrator", `║  EXPERIMENT COMPLETE — ${state.timestamp.padEnd(37)} ║`);
    logInfo("orchestrator", `╠══════════════════════════════════════════════════════════════╣`);

    for (const r of allResults) {
        const nMark = r.nakedResult.exitCode === 0 ? "✓" : "✗";
        const eMark = r.effortlessResult.exitCode === 0 ? "✓" : "✗";
        logInfo("orchestrator", `║  Phase ${r.phase}: Naked ${nMark} (${r.nakedResult.elapsed || 0}s)  Effortless ${eMark} (${r.effortlessResult.elapsed || 0}s)`);
    }

    logInfo("orchestrator", `║  Completed phases: [${state.completed_phases.join(", ")}]`);
    logInfo("orchestrator", `╚══════════════════════════════════════════════════════════════╝`);

    // Generate reports
    const reportScript = join(REPO_ROOT, "tale-report.mjs");
    if (existsSync(reportScript)) {
        for (const dir of [state.naked_dir, state.effortless_dir]) {
            if (existsSync(join(dir, ".tale"))) {
                const r = spawnSync("node", [reportScript, "--dir", dir], { stdio: "inherit" });
                if (r.status === 0) {
                    logInfo("report", `Generated: ${join(dir, ".tale", "report.html")}`);
                }
            }
        }

        // Comparison report
        const r = spawnSync("node", [reportScript, "--compare", state.naked_dir, state.effortless_dir], { stdio: "inherit" });
        if (r.status === 0) {
            logInfo("report", `Comparison report generated`);
        }
    }

    // Final state save
    state.last_run_at = new Date().toISOString();
    saveState(state);

    // Exit with worst exit code
    const worstExit = allResults.reduce((worst, r) => {
        return Math.max(worst, r.nakedResult.exitCode, r.effortlessResult.exitCode);
    }, 0);

    process.exit(worstExit);
}

main().catch(e => {
    logError("orchestrator", `Unhandled error: ${e.message}`);
    console.error(e.stack);
    process.exit(1);
});
