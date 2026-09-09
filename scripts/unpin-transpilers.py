#!/usr/bin/env python3
"""Strip every transpiler version pin from every effortless.json in the repo.

Three keys pin a transpiler, and they are not the same kind of thing.

`PinnedVersion` is a deliberate, hand-written pin — 8 of them across the corpus,
one of which (rulebook-to-react-explainer-dag v2026.05.20.0229 in
community-event-planner) pointed at a decommissioned workload and survived the
first unpinning pass because that pass only knew about the other two.

`LastUrl` and `LastVersionUsed` are written by the CLI after a build as a record
of what it resolved. They are not inert bookkeeping: the CLI resolves to the
recorded `LastUrl` on the next build rather than re-resolving the bare tool name,
so they are a de-facto pin — and one that rots. As of 2026-09-09 the corpus held
26 distinct pins, 10 of them pointing at decommissioned Control Plane workloads
that the CLI retries 10 x 6s before failing the build. Worse, the two fields
disagree: 18 projects recorded LastVersionUsed v2026.08.30.1537 alongside a
LastUrl for v2026-06-06-0219.

Doctrine (2026-09-09): nothing is pinned, in any project. Every transpiler
resolves by bare name to [latest] on every build. Removing these keys is how that
is enforced; the CLI rewrites them on the next build, so re-run this whenever the
corpus is unpinned again.

Usage:
    python3 scripts/unpin-transpilers.py            # report only
    python3 scripts/unpin-transpilers.py --write    # strip the keys
"""

from __future__ import annotations

import argparse
import json
from collections import OrderedDict
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent
PIN_KEYS = ("LastUrl", "LastVersionUsed", "PinnedVersion")


def project_manifests() -> list[Path]:
    paths = [REPO_ROOT / "effortless.json"]
    for area in ("toy-rulebooks", "rulebook-examples"):
        paths.extend(sorted((REPO_ROOT / area).glob("*/effortless.json")))
    missing = [p for p in paths if not p.is_file()]
    if missing:
        raise SystemExit(f"expected manifests are missing: {[str(p) for p in missing]}")
    return paths


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--write", action="store_true", help="actually strip the keys (default is a dry run)")
    args = ap.parse_args()

    total_pins = 0
    touched = 0
    for path in project_manifests():
        manifest = json.loads(path.read_text(encoding="utf-8"), object_pairs_hook=OrderedDict)
        transpilers = manifest.get("ProjectTranspilers")
        if transpilers is None:
            raise SystemExit(f"{path} has no ProjectTranspilers key — refusing to guess at its shape.")
        pins = 0
        for entry in transpilers:
            for key in PIN_KEYS:
                if key in entry:
                    del entry[key]
                    pins += 1
        if pins:
            total_pins += pins
            touched += 1
            rel = path.relative_to(REPO_ROOT)
            print(f"  {'stripped' if args.write else 'would strip'} {pins:>3} pin key(s)  {rel}")
            if args.write:
                path.write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")

    verb = "stripped" if args.write else "would strip"
    print(f"\n{verb} {total_pins} pin key(s) across {touched} manifest(s).")
    if not args.write:
        print("Dry run — pass --write to apply.")


if __name__ == "__main__":
    main()
