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

## Anti-pattern to avoid

Don't paper over the discovery bug by **silently defaulting** to a substrate when none is found. That's the exact "silent fallback" `CLAUDE.md` calls out: it would hide the next rename mismatch too, and turn a loud `FAIL: init-db.sh missing` into a quiet `0% pass`. Always fail loudly with the canonical expected path; only add to the alias table when the new mapping is intentional.

---

## Where to put new "structure failure" entries

If you hit a different flavor of the same bug, append a new section to this file rather than starting a new doc. The point of this file is one stop for "I have weird path/discovery output — is this familiar?"
