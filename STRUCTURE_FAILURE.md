# STRUCTURE_FAILURE.md — when a folder rename half-propagates

## What this is

A short post-mortem for one class of bug that has bitten this repo more than once: **a refactor renames a folder convention, but only some of the scripts that hardcode the old name get updated.** The build "succeeds," but downstream consumers silently look at the wrong path. Symptoms look like a test or data failure, but the actual cause is plumbing.

When you see the same shape of failure again — different folder, same story — fix it the same way.

---

## The canonical instance (Nov 2026)

**Symptom chain:**

1. `./start.sh` → `[B] BUILD` ran `rulebooktopostgres` and `execute` to completion.
2. Right after, conformance failed with `FAIL: per-domain init-db.sh missing for 'acme-corporation'. Expected: …/effortless-postgres/init-db.sh`. The file was actually at `…/postgres-bootstrap/init-db.sh`.
3. After patching that path, the build ran clean but `all-tests-results.md` reported **`Substrates Tested: 0` / Overall Score: 0.0%** — no tests ran, no errors.
4. After fixing substrate discovery, the build runs end-to-end and produces real conformance numbers (5/40, real logic mismatches, useful signal).

**Root cause:**

Commit `13a6926` ("rulebook-as-HEAD refactor") renamed the per-domain Postgres output folder from `postgres/` to `postgres-bootstrap/` inside each `rulebook-examples/<domain>/`. The per-domain `effortless.json` files were updated; three downstream consumers weren't:

| File | Wrong path it hardcoded | Fix |
|------|-------------------------|-----|
| `orchestration/orchestrate.sh:1061` | `…/effortless-postgres/init-db.sh` | `…/postgres-bootstrap/init-db.sh` |
| `orchestration/orchestrate.sh:1124` | `$ERB_DOMAIN_DIR/effortless-postgres/init-db.sh` | `$ERB_DOMAIN_DIR/postgres-bootstrap/init-db.sh` |
| `execution-substrates/effortless-postgres/inject-substrate.sh:20-28` | `$ERB_DOMAIN_DIR/postgres` | `$ERB_DOMAIN_DIR/postgres-bootstrap` |
| `orchestration/shared.py` `EFFORTLESS_ALIASES` table | knew `postgres → effortless-postgres` only | added `postgres-bootstrap → effortless-postgres` |

Note three different *wrong* names in three different files (`effortless-postgres`, `postgres`, just-missing). That's the tell — the wrong path looks plausible-but-different in every file, because each script crystallized whatever the name happened to be when it was written.

**Why the categories matter:**

- `execution-substrates/effortless-postgres/` is the **substrate** (test runner code, repo-level, one per technology).
- `rulebook-examples/<domain>/postgres-bootstrap/` is the **per-domain output** (where `rulebook-to-postgres` writes the SQL + `init-db.sh`).

These are different things. Scripts that confused the substrate name for the per-domain folder name (or vice versa) all broke in this episode.

---

## Diagnostic heuristic

You probably have this bug if **two or more of these are true** in one run:

1. `BUILD` reports each transpiler as `✓ OK` and prints `All transpilers complete.`
2. The next step fails with `… missing` for a file that *does* exist somewhere obvious.
3. OR the next step "succeeds" but the report says `Substrates Tested: 0` / `0% pass`.
4. Two scripts that should agree on a path disagree about it (grep finds the same concept under two or three names).

If symptoms 1 + 3 hit together, suspect substrate **discovery**, not substrate **execution**. The orchestrator silently skipped every substrate because the alias table didn't recognize a renamed folder.

---

## How to fix the next instance

When this pattern shows up around a different folder rename (e.g. `effortless-rulebook/` ↔ `rulebook/`, or `postgres-bootstrap/` ↔ `effortless-pg/`, or anything else):

1. **Identify the canonical name.** Whatever the active `effortless.json` files point to via `RelativePath` is authoritative — that's where the transpiler actually writes.
2. **Grep for the wrong names** across `.sh` and `.py`:
   ```
   grep -rn '<old-name>\|<wrong-but-plausible-name>' --include='*.sh' --include='*.py' .
   ```
3. **Update hardcoded paths** to the canonical name.
4. **Add an entry to `EFFORTLESS_ALIASES`** in `orchestration/shared.py:get_active_project_substrates` mapping the new per-domain folder name to the substrate folder name. Keep the legacy alias too so older domains still work — this is a real default-from-SSoT case, not a fallback. (See `CLAUDE.md` "Defaults derived from the SSoT are NOT fallbacks.")
5. **Verify discovery** with:
   ```
   python3 -c "from orchestration.shared import get_active_project_substrates; print(get_active_project_substrates('<domain>'))"
   ```
   The list must be non-empty AND every name must correspond to a directory under `execution-substrates/`.

---

## Second instance (May 2026, acme-corporation) — every `Actual` is `None`

**Symptom chain:**

1. `BUILD` runs to completion. `rulebooktopostgres` ✓ OK, `execute` ✓ OK, `All transpilers complete.`
2. `effortless-postgres` substrate reports `5 / 40 (12.5%) PARTIAL` — looks like real conformance signal.
3. But every entry in the failure table reads `Actual: None`. Not `Actual: <wrong-value>`. Just `None`. Across all 35 failures. For every entity. Including fields that obviously *do* compute in the DB (`role_name`, `count_of_employees`, `full_name`, etc.).
4. Manually running `psql -U postgres -d erb_acme_corporation -c "SELECT * FROM vw_employees"` returns correct rows with all calculated columns populated.
5. Manually running `python3 take-test.py` after the orchestrator finished produces correct `test-answers/*.json` files. So the substrate's code path WORKS — yet the orchestrator's run left `test-answers/` **empty**.

**Diagnosis:**

This is the same shape as the postgres-bootstrap bug but the failure mode is more insidious. The producer (`take-test.py`) and the consumer (`test-orchestrator.grade_substrate`) both compute their `test-answers/` path from `active-domain.txt`, but they read it at *different moments* inside the orchestration:

- `orchestrate.sh:1265` precomputes `test_answers_dir="$ERB_TESTING_DIR/$substrate/test-answers"` and clears it.
- The inject script runs `effortless -buildLocal`, `init-db.sh`, then `take-test.sh` → `take-test.py`.
- `take-test.py:36` re-reads `active-domain.txt` from disk.
- `test-orchestrator.py:59` reads `active-domain.txt` at module-load time, which is whenever the grader process started.

If anything between the orchestrator's start and the grader's grade-time changes `active-domain.txt` (or if the producer/consumer were started in different processes against different file states), the producer writes to one path and the consumer reads from another. Both look right when grepped individually; the bug only exists in the temporal gap.

**The false-positive tell:**

`grade_substrate` does NOT fail loudly when `test-answers/` is empty — it falls through `get_substrate_answers` returning `{}` and then scores every expected pair as `Actual: None`. The grader's `_is_nullish` helper deliberately treats `None == "" == 0 == False == "no"` as one equivalence class (so substrates can serialize empty values however they want). That means any field whose canonical answer happens to be nullish (`role_is_manager: False`, an empty string, a 0, etc.) **passes by accident** when the actual is missing.

That's how a 0% catastrophic plumbing failure shows up as `5 / 40 (12.5%) PARTIAL` — five answer-key values happened to be nullish, so the missing actual matched. **The score is a lie**; nothing was actually verified for that substrate.

**Diagnostic heuristic for this variant:**

You have THIS bug, not a real conformance bug, if:

1. The substrate's `test-answers/` directory exists but is empty (or missing files for entities that DO have answer-keys).
2. Every (or nearly every) failing row in the test-results table shows `Actual: None`, never a real-but-wrong value.
3. Running the substrate's `take-test.py` manually with the current `active-domain.txt` writes the files correctly.
4. The "passes" cluster on fields where the canonical value is null/false/0/empty.

If 2 + 4 hit, the score is almost certainly a false positive. Treat 12.5% as 0% until you can re-run and see populated `test-answers/`.

**The deeper fix (not just papering over):**

Add a missing-answers loudness layer to the grader:

- If `get_substrate_answers(substrate)` returns `{}` for a substrate whose `prepare_substrate_for_test` was called this run, fail with `FAIL: <substrate> wrote no test-answers — structural failure, NOT a logic mismatch.` Do not score.
- If `get_substrate_answers` returns answers for some entities but not others, fail per-entity instead of silently scoring all that entity's fields as `None`.
- Equivalently: refuse to grant `_is_nullish` equivalence credit when the *actual* came from a missing file, only when it came from a file that explicitly serialized null/empty. The grader needs to distinguish "substrate said NULL" from "substrate said nothing."

This converts the silent 12.5% into a loud `0 / 40 — substrate produced no answers`, matching `STRUCTURE_FAILURE.md`'s rule: when plumbing fails, fail noisily; never hand out partial credit derived from missing data.

---

## Generalizing the pattern

These two instances differ in their literal path but share their shape. Any time the repo has a **producer → consumer** pair that both compute the same path independently (from `active-domain.txt`, from a folder convention, from an `EFFORTLESS_ALIASES` table, from anything mutable), and the consumer silently degrades to a default when the producer didn't deliver, you can hit this. The "silently degrades" step is where the false-positive score comes from.

When you suspect this:

1. **Diff the producer's expected output path against the consumer's read path** — print both literally, side by side, from a fresh subprocess.
2. **Make the consumer fail loudly on missing producer output**, not just when the path is missing but when individual expected files are missing.
3. **Audit every place a downstream score could "accidentally pass"** — look for nullish-equivalence in graders, default-empty-dict returns, `try/except` blocks that swallow file-not-found, anything that could turn missing data into a benign-looking value.
4. When you find them, treat the discovery the same as the postgres-bootstrap rename: add a loud assertion at the boundary, and DO NOT hide it behind a fallback.

---

## Anti-pattern to avoid

Don't paper over the discovery bug by **silently defaulting** to a substrate when none is found. That's the exact "silent fallback" `CLAUDE.md` calls out: it would hide the next rename mismatch too, and turn a loud `FAIL: init-db.sh missing` into a quiet `0% pass`. Always fail loudly with the canonical expected path; only add to the alias table when the new mapping is intentional.

The same rule applies to graders: never let a missing `test-answers/<entity>.json` produce a row that *could* pass. Missing data is a structural failure; score it as such.

---

## Where to put new "structure failure" entries

If you hit a different flavor of the same bug, append a new section to this file rather than starting a new doc. The point of this file is one stop for "I have weird path/discovery output — is this familiar?"
