// ===========================================================================
// conformance/run-cli.ts — headless conformance runner.
//
// Runs the entire ConformanceTests suite against every registered engine, with
// NO UI and NO running server required. Prints a colored summary and writes a
// timestamped JSON run-log under testing/conformance-runs/ (and updates
// latest.json) so a run is auditable after the fact and the admin UI can show
// run history without re-executing.
//
//   node --import tsx conformance/run-cli.ts            # run all, write log
//   node --import tsx conformance/run-cli.ts --only id1,id2
//   node --import tsx conformance/run-cli.ts --no-log   # don't write a log
//   node --import tsx conformance/run-cli.ts --quiet    # summary only
//
// Exit code: 0 iff allGreen (no fail, no error). Gaps do NOT fail the run — a
// gap is a surfaced coverage difference, not a wrong answer; CI can decide. The
// "engines drop lookup columns" known gap is reported, loudly, as a gap.
// ===========================================================================
import { writeFile, mkdir } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { ConformanceHarness, type RunReport, type TestResult } from "./harness";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const RUNS_DIR = path.join(__dirname, "..", "..", "..", "testing", "conformance-runs");

const C = {
  reset: "\x1b[0m", dim: "\x1b[2m", bold: "\x1b[1m",
  green: "\x1b[32m", red: "\x1b[31m", yellow: "\x1b[33m", cyan: "\x1b[36m", magenta: "\x1b[35m",
};
const ICON = { pass: "✓", fail: "✗", gap: "◐", error: "!", skipped: "·" } as const;
const COLOR = { pass: C.green, fail: C.red, gap: C.yellow, error: C.magenta, skipped: C.dim } as const;

function parseArgs(argv: string[]) {
  const a = { only: null as string[] | null, log: true, quiet: false };
  for (let i = 0; i < argv.length; i++) {
    if (argv[i] === "--only") a.only = (argv[++i] || "").split(",").map((s) => s.trim()).filter(Boolean);
    else if (argv[i] === "--no-log") a.log = false;
    else if (argv[i] === "--quiet") a.quiet = true;
  }
  return a;
}

function printRow(r: TestResult) {
  const c = COLOR[r.overall];
  const head = `${c}${ICON[r.overall]}${C.reset} ${C.bold}${r.id}${C.reset} ${C.dim}[${r.section}/${r.testKind}]${C.reset}`;
  console.log(head);
  console.log(`    ${C.dim}${r.displayName}${r.featureRef ? "  ·  " + r.featureRef : ""}${C.reset}`);
  for (const e of r.engines) {
    const ec = COLOR[e.status];
    console.log(`      ${ec}${ICON[e.status]} ${e.engine.padEnd(9)}${C.reset} ${e.detail}`);
  }
}

function printSummary(rep: RunReport) {
  const s = rep.summary;
  console.log("");
  console.log(`${C.bold}── Conformance summary ──${C.reset}`);
  console.log(`  engines : ${s.engines.join(", ")}`);
  console.log(`  ${C.green}pass ${s.passed}${C.reset}  ${C.red}fail ${s.failed}${C.reset}  ${C.yellow}gap ${s.gaps}${C.reset}  ${C.magenta}err ${s.errors}${C.reset}  ${C.dim}skip ${s.skipped}${C.reset}  / ${s.total}`);
  console.log(`  duration: ${s.durationMs} ms`);
  console.log(
    s.allGreen
      ? `  ${C.green}${C.bold}ALL GREEN — both engines reason about every stable fact identically.${C.reset}`
      : `  ${C.red}${C.bold}NOT GREEN — ${s.failed} fail / ${s.errors} error. See rows above.${C.reset}`
  );
  if (s.gaps > 0) console.log(`  ${C.yellow}${s.gaps} gap(s): a substrate didn't emit a value — surfaced, not hidden.${C.reset}`);
}

async function writeLog(rep: RunReport, isSubset: boolean): Promise<string> {
  await mkdir(RUNS_DIR, { recursive: true });
  // filename-safe timestamp from the ISO string (no Date.now needed)
  const stamp = rep.summary.startedAt.replace(/[:.]/g, "-");
  const file = path.join(RUNS_DIR, `run-${stamp}${isSubset ? `-subset-${rep.summary.total}` : ""}.json`);
  await writeFile(file, JSON.stringify(rep, null, 2) + "\n", "utf8");
  // latest.json is the canonical FULL baseline (the admin UI reads it). A --only
  // subset run is archived above but must NOT clobber it, or a refresh would show
  // just the subset.
  if (!isSubset) {
    await writeFile(path.join(RUNS_DIR, "latest.json"), JSON.stringify(rep, null, 2) + "\n", "utf8");
  }
  return file;
}

async function main() {
  const args = parseArgs(process.argv.slice(2));
  const harness = new ConformanceHarness();
  await harness.load();
  const rep = await harness.run({ only: args.only || undefined });

  if (!args.quiet) {
    console.log(`${C.cyan}${C.bold}Conformance run — ${rep.summary.total} test(s) over [${rep.summary.engines.join(", ")}]${C.reset}\n`);
    // show fails/errors/gaps first, then a compact pass tally
    const notable = rep.results.filter((r) => r.overall !== "pass" && r.overall !== "skipped");
    for (const r of notable) printRow(r);
    if (notable.length === 0) console.log(`  ${C.green}(every enabled test passed on every engine)${C.reset}`);
    else console.log(`\n  ${C.dim}(${rep.summary.passed} passing test(s) not shown — use --quiet=false to expand)${C.reset}`);
  }

  printSummary(rep);

  if (args.log) {
    const file = await writeLog(rep, !!args.only);
    console.log(`\n  ${C.dim}run-log: ${path.relative(process.cwd(), file)}${C.reset}`);
  }

  process.exit(rep.summary.allGreen ? 0 : 1);
}

main().catch((err) => {
  console.error(`${C.red}harness crashed:${C.reset}`, err);
  process.exit(2);
});
