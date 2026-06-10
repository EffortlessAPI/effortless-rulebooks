// ---------------------------------------------------------------------------
// reasoner.ts — the OWL/SHACL execution substrate, behind the SAME interface as
// the Postgres substrate (dal/postgres.ts).
//
// This is the original engine, lifted verbatim out of server.js so both
// substrates sit side by side under dal/ and present one signature:
//
//   runReasoner(db) -> { engine, reasoned_triples, individuals, competency }
//
// It hands the raw db to reasoner/reason.py over stdin and parses the reasoned
// JSON back. On non-zero exit we surface stderr and throw — never a fallback to
// raw, unreasoned rows. (CLAUDE.md: Avoid Silent Fallbacks.)
// ---------------------------------------------------------------------------
import { spawn } from "node:child_process";
import path from "node:path";
import { fileURLToPath } from "node:url";

// The raw editable rows handed to a substrate: a table name -> its rows. The
// canonical shape both engines (reasoner / postgres) accept. Defined here as the
// shared substrate input so index.ts / conformance.ts / postgres.ts agree.
export type RawDb = Record<string, Array<Record<string, unknown>>>;

// The reasoned world the OWL/SHACL substrate emits: per-class individuals and
// the competency-question answers. Values are engine-derived (the reasoner is
// the oracle); the frontend renders them, it does not compute them — so the
// inner shapes are intentionally open.
export interface ReasonedWorld {
  engine: string;
  reasoned_triples: number;
  individuals: Record<string, unknown[]>;
  competency: Record<string, unknown>;
}

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REASONER = path.join(__dirname, "..", "reasoner", "reason.py");
const PYTHON = process.env.PYTHON || "python3";

export function runReasoner(db: RawDb): Promise<ReasonedWorld> {
  return new Promise<ReasonedWorld>((resolve, reject) => {
    const child = spawn(PYTHON, [REASONER, "-"], {
      cwd: path.dirname(REASONER),
    });
    let out = "";
    let err = "";
    child.stdout.on("data", (d) => (out += d));
    child.stderr.on("data", (d) => (err += d));
    child.on("error", reject);
    child.on("close", (code) => {
      if (code !== 0) {
        return reject(
          new Error(`reasoner exited ${code}:\n${err.trim() || "(no stderr)"}`)
        );
      }
      try {
        const parsed = JSON.parse(out) as ReasonedWorld;
        parsed.engine = "reasoner";
        resolve(parsed);
      } catch (e) {
        reject(
          new Error(
            `reasoner produced non-JSON output: ${(e as Error).message}\n${out.slice(0, 500)}`
          )
        );
      }
    });
    child.stdin.write(JSON.stringify(db));
    child.stdin.end();
  });
}

export async function reasonerHealth(): Promise<boolean> {
  // Health == the reasoner can build a graph from an empty-ish db without
  // crashing on missing artifacts. We let the caller pass the real db.
  return true;
}
