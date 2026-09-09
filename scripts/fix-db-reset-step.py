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

Both are fixed here. The bit is set in the index as well as on disk, so a fresh
clone gets it — the CLI's clean-and-recreate pass still drops it on disk during a
build, which is a CLI defect (cr-20-02) this cannot fix, but a `git checkout`
restores it and the corpus runner's own db phase calls `bash` on the script
directly and is unaffected either way.

Usage:
    python3 scripts/fix-db-reset-step.py            # report only
    python3 scripts/fix-db-reset-step.py --write
"""

from __future__ import annotations

import argparse
import json
import subprocess
import sys
from collections import OrderedDict
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from erb_project import postgres_output_dir  # noqa: E402

REPO_ROOT = Path(__file__).resolve().parent.parent
RESET = "reset-rulebook-db.sh"


def manifests() -> list[Path]:
    paths = [REPO_ROOT / "effortless.json"]
    for area in ("toy-rulebooks", "rulebook-examples"):
        paths.extend(sorted((REPO_ROOT / area).glob("*/effortless.json")))
    return [p for p in paths if p.is_file()]


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--write", action="store_true")
    args = ap.parse_args()

    repointed, chmodded, ok = [], [], 0
    for path in manifests():
        project = path.parent.name
        manifest = json.loads(path.read_text(encoding="utf-8"), object_pairs_hook=OrderedDict)
        transpilers = manifest.get("ProjectTranspilers")
        if transpilers is None:
            raise SystemExit(f"{path} has no ProjectTranspilers key — refusing to guess at its shape.")

        steps = [t for t in transpilers
                 if RESET in t.get("CommandLine", "") and t.get("CommandLine", "").startswith("-exec")
                 and not t.get("IsDisabled")]
        if not steps:
            continue

        output_dir = postgres_output_dir(path.parent)
        if output_dir is None:
            print(f"  {project}: has a reset step but no enabled rulebook-to-postgres — leaving alone")
            continue
        want = "/" + output_dir.relative_to(path.parent).as_posix()

        changed = False
        for step in steps:
            if step.get("RelativePath") != want:
                print(f"  {project}: RelativePath {step.get('RelativePath')!r} -> {want!r}")
                step["RelativePath"] = want
                changed = True
        if changed:
            repointed.append(project)
            if args.write:
                path.write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")

        script = output_dir / RESET
        if script.is_file() and not script.stat().st_mode & 0o111:
            chmodded.append(project)
            print(f"  {project}: {script.relative_to(REPO_ROOT)} is not executable")
            if args.write:
                script.chmod(script.stat().st_mode | 0o755)
                # Record the bit in the index too, so a fresh clone gets it.
                subprocess.run(["git", "update-index", "--chmod=+x", str(script.relative_to(REPO_ROOT))],
                               cwd=REPO_ROOT, capture_output=True)
        if not changed and project not in chmodded:
            ok += 1

    verb = "fixed" if args.write else "would fix"
    print(f"\n{verb}: {len(repointed)} RelativePath(s), {len(chmodded)} executable bit(s); {ok} already correct")
    if not args.write:
        print("Dry run — pass --write to apply.")


if __name__ == "__main__":
    main()
