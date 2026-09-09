#!/usr/bin/env python3
"""Take postgres-calculated-to-rulebook out of every project's BUILD pipeline.

It caused 18 of the 38 failures in the 2026-09-09 corpus baseline, by failing on
a missing `.pg-raw-data.json`. The obvious fix — add the `pull-from-postgres.sh`
step that produces that file — was tried and it does make the build pass. It was
rejected anyway, because of what the spoke then does.

Its own Description claims "raws verified, never written". It is not true. One
build of toy-rulebooks/star-trek rewrote 1018 RAW datetime values in the
rulebook, round-tripping them through Postgres and baking the BUILDING MACHINE'S
timezone into the hub:

    Episodes.Airdate   '1966-09-01'           -> '1966-09-01T00:00:00-05:00'
    Ratings.CreatedAt  '2026-01-12T19:46:59Z' -> '2026-01-12T13:46:59-06:00'

That is a build mutating HEAD, and mutating it differently depending on where it
runs. rulebook-examples/traffic-ticket-contest had already reached the same
conclusion independently and disabled it there.

Disabling costs nothing, because the spoke's real, load-bearing use is NOT the
build pipeline. `postgres-bootstrap/regenerate-answer-keys.sh` invokes the
injector directly by path, with ERB_WRITE_COMPUTED=true, against a TEMP database
it drops afterwards — a deliberate answer-key refresh, which is what writing
computed values back into a rulebook is actually for. That path is untouched here.

Re-enable per project only once the transpiler honours its own "raws verified,
never written" contract.

Usage:
    python3 scripts/disable-reverse-sync-spoke.py            # report only
    python3 scripts/disable-reverse-sync-spoke.py --write
"""

from __future__ import annotations

import argparse
import json
from collections import OrderedDict
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent
SPOKE = "postgres-calculated-to-rulebook"
REASON = (
    "Reverse spoke: refresh rulebook derived-field display copies from Postgres views. "
    "DISABLED in the build pipeline 2026-09-09: despite claiming 'raws verified, never written' "
    "it rewrites raw datetime values with the building machine's timezone offset (1018 of them in "
    "one star-trek build), which makes every build mutate the rulebook hub and mutate it differently "
    "per machine. Its load-bearing use is postgres-bootstrap/regenerate-answer-keys.sh, which calls "
    "the injector directly with ERB_WRITE_COMPUTED=true against a temp database and is unaffected. "
    "Re-enable only when the transpiler honours its own raws-never-written contract."
)


def manifests() -> list[Path]:
    paths = [REPO_ROOT / "effortless.json"]
    for area in ("toy-rulebooks", "rulebook-examples"):
        paths.extend(sorted((REPO_ROOT / area).glob("*/effortless.json")))
    return [p for p in paths if p.is_file()]


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--write", action="store_true")
    args = ap.parse_args()

    disabled, already, absent = [], [], 0
    for path in manifests():
        project = path.parent.name
        manifest = json.loads(path.read_text(encoding="utf-8"), object_pairs_hook=OrderedDict)
        transpilers = manifest.get("ProjectTranspilers")
        if transpilers is None:
            raise SystemExit(f"{path} has no ProjectTranspilers key — refusing to guess at its shape.")

        entries = [e for e in transpilers if SPOKE in e.get("CommandLine", "")]
        if not entries:
            absent += 1
            continue
        changed = False
        for entry in entries:
            if entry.get("IsDisabled"):
                continue
            entry["IsDisabled"] = True
            entry["Description"] = REASON
            changed = True
        (disabled if changed else already).append(project)
        if changed and args.write:
            path.write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")

    verb = "disabled in" if args.write else "would disable in"
    print(f"{verb} {len(disabled)} project(s):")
    for p in sorted(disabled):
        print(f"    {p}")
    print(f"already disabled: {len(already)} {sorted(already)}")
    print(f"does not register the spoke: {absent}")
    if not args.write:
        print("\nDry run — pass --write to apply.")


if __name__ == "__main__":
    main()
