#!/usr/bin/env python3
"""Point each project's database-reset build step at the script it actually runs.

Two independent defects made 6 of 38 projects fail the 2026-09-09 baseline with
`The system couldn't find the command "./reset-rulebook-db.sh"`:

1. WRONG DIRECTORY. The `-exec ./reset-rulebook-db.sh` step declared
   RelativePath "/" — the project root — while rulebook-to-postgres writes the
   script into postgres/, effortless-postgres/ or postgres-bootstrap/ depending
   on the project. There is no conventional directory, so the step's
   RelativePath has to match the rulebook-to-postgres step's.

2. MISSING EXECUTABLE BIT. The CLI's `-exec` takes only the first token, so
   `-exec bash <script>` runs bare bash and blocks on stdin until the build is
   killed. Single-token `-exec ./script` is the only working form, and it needs
   the executable bit. The affected scripts were committed 100644 while the
   identical blob is 100755 in every project that works.

3. WRONG DATABASE (2026-11-26). rulebook-to-postgres bakes `-p database=<name>`
   into the generated script's default connection and uses `demo` when the
   step does not pass it. No project passed it, so every plain `effortless
   build` reset a shared `demo` database while the app and the corpus runner
   read `erb_<slug>` (erb_project.db_name): the build was green and the app
   never saw the rulebook change. Each project's rulebook-to-postgres step now
   names its own database. Guarded by
   scripts/tests/test_build_resets_project_database.py.

4. THE BIT AGAIN (2026-11-26). Fix 2 set the bit in git, but every build that
   changes the script's content regenerates it without the bit (cr-20-02, a
   CLI defect), so the step failed again on the very next such build, stopped
   the build there, and the build still exited 0. The step no longer runs the
   generated script at all: it runs scripts/reset-project-db.py, committed
   executable and never regenerated, which hands the work to
   erb_project.reset_db (`bash reset-rulebook-db.sh <url>`, the same code the
   corpus runner uses). Fix 1's RelativePath still matters: it is where that
   script starts looking for its project.

All four are fixed here. Guarded by scripts/tests/test_build_resets_project_database.py.

Usage:
    python3 scripts/fix-db-reset-step.py            # report only
    python3 scripts/fix-db-reset-step.py --write
"""

from __future__ import annotations

import argparse
import json
import os
import re
import sys
from collections import OrderedDict
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from erb_project import db_name, postgres_output_dir  # noqa: E402

REPO_ROOT = Path(__file__).resolve().parent.parent
RESET = "reset-rulebook-db.sh"
SHARED = REPO_ROOT / "scripts" / "reset-project-db.py"


def is_database_step(step: dict) -> bool:
    """An enabled `-exec` step that resets the project database, in either the
    old form (the generated reset-rulebook-db.sh) or the current one."""
    line = step.get("CommandLine", "")
    return (line.startswith("-exec") and not step.get("IsDisabled")
            and (RESET in line or SHARED.name in line))


def database_step_command(project_dir: Path, relative_path: str) -> str:
    """`-exec <path to scripts/reset-project-db.py>` relative to the step's
    working directory. Relative because the CLI re-parses a token starting
    with `/` as one of its own options."""
    cwd = project_dir / relative_path.lstrip("/")
    return f"-exec {os.path.relpath(SHARED, cwd)}"


def manifests() -> list[Path]:
    paths = [REPO_ROOT / "effortless.json"]
    for area in ("toy-rulebooks", "rulebook-examples"):
        paths.extend(sorted((REPO_ROOT / area).glob("*/effortless.json")))
    return [p for p in paths if p.is_file()]


DATABASE_PARAM = re.compile(r"(?<!\S)-p\s+database=(\S+)")


def database_param(command_line: str) -> str | None:
    """The database a rulebook-to-postgres command line names, or None."""
    m = DATABASE_PARAM.search(command_line)
    return m.group(1) if m else None


def with_database(command_line: str, database: str) -> str:
    """The command line naming `database`: an existing `-p database=` is
    replaced in place, a missing one is appended."""
    if DATABASE_PARAM.search(command_line):
        return DATABASE_PARAM.sub(f"-p database={database}", command_line, count=1)
    return f"{command_line} -p database={database}"


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--write", action="store_true")
    args = ap.parse_args()

    repointed, ok = [], 0
    for path in manifests():
        project = path.parent.name
        manifest = json.loads(path.read_text(encoding="utf-8"), object_pairs_hook=OrderedDict)
        transpilers = manifest.get("ProjectTranspilers")
        if transpilers is None:
            raise SystemExit(f"{path} has no ProjectTranspilers key — refusing to guess at its shape.")

        steps = [t for t in transpilers if is_database_step(t)]
        if not steps:
            continue

        output_dir = postgres_output_dir(path.parent)
        if output_dir is None:
            print(f"  {project}: has a reset step but no enabled rulebook-to-postgres — leaving alone")
            continue
        want = "/" + output_dir.relative_to(path.parent).as_posix()

        changed = False
        database = db_name(project)
        for step in transpilers:
            line = step.get("CommandLine", "")
            if not line.startswith("rulebook-to-postgres") or step.get("IsDisabled"):
                continue
            if database_param(line) != database:
                print(f"  {project}: rulebook-to-postgres resets {database_param(line) or 'demo'!r} -> {database!r}")
                step["CommandLine"] = with_database(line, database)
                changed = True
        for step in steps:
            if step.get("RelativePath") != want:
                print(f"  {project}: RelativePath {step.get('RelativePath')!r} -> {want!r}")
                step["RelativePath"] = want
                changed = True
            command = database_step_command(path.parent, want)
            if step["CommandLine"] != command:
                print(f"  {project}: {step['CommandLine']!r} -> {command!r}")
                step["CommandLine"] = command
                changed = True
        if changed:
            repointed.append(project)
            if args.write:
                path.write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")

        if not changed:
            ok += 1

    verb = "fixed" if args.write else "would fix"
    print(f"\n{verb}: {len(repointed)} manifest(s); {ok} already correct")
    if not args.write:
        print("Dry run — pass --write to apply.")


if __name__ == "__main__":
    main()
