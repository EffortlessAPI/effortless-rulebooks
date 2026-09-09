#!/usr/bin/env python3
"""Shared per-project phases used by both the single-project conformance path
(scripts/run-conformance.py) and the corpus fan-out (scripts/run-corpus.py).

There is exactly one implementation of "build this project" and "reset this
project's database" so the explorer's per-project button and the corpus runner
cannot drift apart.

Runnable standalone for one project:
    python3 scripts/erb_project.py <slug> --build --reset-db
"""

from __future__ import annotations

import argparse
import json
import os
import re
import subprocess
import sys
from collections import OrderedDict
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent


def find_domain_dir(slug: str) -> Path:
    """rulebook-examples/ first, then toy-rulebooks/ — the same resolution
    orchestrate.sh's find_domain_dir() and test-orchestrator.py both use."""
    if slug == "root":
        return REPO_ROOT
    examples_dir = REPO_ROOT / "rulebook-examples" / slug
    toy_dir = REPO_ROOT / "toy-rulebooks" / slug
    if examples_dir.is_dir():
        return examples_dir
    if toy_dir.is_dir():
        return toy_dir
    raise SystemExit(
        f"project slug {slug!r} not found under rulebook-examples/ or toy-rulebooks/ "
        f"(checked {examples_dir} and {toy_dir})")


def db_name(slug: str) -> str:
    """The per-project database name. Derived from the slug the same way
    orchestrate.sh derives it, so DATABASE_URL only ever OVERRIDES this value —
    it never selects a different database behind the caller's back."""
    return f"erb_{slug.replace('-', '_')}"


def database_url(slug: str) -> str:
    return os.environ.get("DATABASE_URL") or f"postgresql://postgres@localhost:5432/{db_name(slug)}"


def rulebook_path(slug: str, domain_dir: Path) -> Path:
    """The hub for this project. Both filenames the protocol allows are valid
    (<slug>-rulebook.json or effortless-rulebook.json); the folder disambiguates.
    Ambiguity and absence are both hard errors."""
    rb_dir = domain_dir / "effortless-rulebook"
    if not rb_dir.is_dir():
        raise SystemExit(f"{rb_dir} does not exist — {slug} is not a governed Effortless project.")
    named = rb_dir / f"{slug}-rulebook.json"
    canonical = rb_dir / "effortless-rulebook.json"
    found = [p for p in (named, canonical) if p.is_file()]
    if not found:
        raise SystemExit(f"neither {named.name} nor {canonical.name} exists in {rb_dir}")
    if len(found) == 2:
        raise SystemExit(f"{rb_dir} contains BOTH {named.name} and {canonical.name} — ambiguous hub, fix the project.")
    return found[0]


PIN_KEYS = ("LastUrl", "LastVersionUsed")


def unpin(effortless_json: Path, log) -> int:
    """Drop every transpiler version pin from one manifest, returning how many
    keys were removed.

    The CLI writes LastUrl/LastVersionUsed after each build and RESOLVES TO THEM
    on the next one instead of re-resolving the bare tool name, so they are a
    de-facto pin that rots as soon as a new version ships. It offers no opt-out,
    and it re-adds them on every build — so this cannot be a one-time cleanup.
    Stripping them immediately before each build is what keeps the repo's
    "nothing is pinned, in any project" rule true in practice: every build
    resolves [latest], and the pin the CLI writes back is only ever a record of
    what [latest] was at that moment, never a stale host the NEXT build follows.
    """
    manifest = json.loads(effortless_json.read_text(encoding="utf-8"), object_pairs_hook=OrderedDict)
    transpilers = manifest.get("ProjectTranspilers")
    if transpilers is None:
        raise SystemExit(f"{effortless_json} has no ProjectTranspilers key — refusing to guess at its shape.")
    removed = 0
    for entry in transpilers:
        for key in PIN_KEYS:
            if key in entry:
                del entry[key]
                removed += 1
    if removed:
        effortless_json.write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
        log(f"[unpin] dropped {removed} version pin(s) so this build resolves [latest]")
    return removed


def build_project(slug: str, domain_dir: Path, log) -> None:
    """`effortless build` in the project directory: the whole pipeline declared
    in that project's effortless.json. Unpins first (see unpin()). Raises
    SystemExit on failure."""
    effortless_json = domain_dir / "effortless.json"
    if not effortless_json.is_file():
        raise SystemExit(f"{effortless_json} does not exist — {slug} has no build pipeline.")
    unpin(effortless_json, log)
    log(f"[build] effortless build in {domain_dir.relative_to(REPO_ROOT) if domain_dir != REPO_ROOT else '.'}")
    run_streaming(["effortless", "build"], cwd=domain_dir, log=log)


def reset_db(slug: str, domain_dir: Path, log) -> str:
    """Create the per-project database if needed, then apply the freshly
    generated destructive dev-reset script. Returns the database URL used.

    reset-rulebook-db.sh is regenerated by every build and routinely comes back
    without its executable bit, so it is always invoked as `bash <script>`."""
    script = domain_dir / "postgres-bootstrap" / "reset-rulebook-db.sh"
    if not script.is_file():
        raise SystemExit(
            f"{script} does not exist. rulebook-to-postgres generates it on every build — "
            f"build {slug} before resetting its database.")
    url = database_url(slug)
    name = db_name(slug)
    log(f"[db] createdb {name} (no-op when it already exists)")
    subprocess.run(["createdb", name], capture_output=True, text=True)  # already-exists is not an error here
    log(f"[db] bash postgres-bootstrap/reset-rulebook-db.sh against {name}")
    run_streaming(["bash", str(script), url], cwd=domain_dir, log=log)
    return url


# Lines worth quoting back as "what went wrong", most specific first. The last
# line of a failing `effortless build` is usually a stray brace from a pretty-
# printed proxy error, so taking it verbatim produces "exited 255: }" — true and
# useless. These patterns pull the line a human would have pointed at.
ERROR_PATTERNS = [
    re.compile(r"^\[cli\]\s+FAILED:\s*(?P<detail>.+)$"),
    re.compile(r"^\[cli\]\s+BUILD FAILED\s*[-—]\s*(?P<detail>.+)$"),
    re.compile(r"^(?P<detail>\*\*\* TRANSPILER ERROR \*\*\*.*)$"),
    re.compile(r"^(?P<detail>ERROR:\s+.+)$"),
    re.compile(r"^(?P<detail>FATAL:?\s+.+)$"),
    re.compile(r"^(?P<detail>\w*(?:Error|Exception):\s+.+)$"),
    re.compile(r"^(?P<detail>psql:.*(?:ERROR|FATAL).*)$"),
]


def first_error(lines: list[str], fallback: str) -> str:
    """The most useful error line in a failed command's output.

    Scans in pattern priority order rather than line order: a run that prints a
    generic `ERROR:` early and the specific `[cli] FAILED: <transpiler>` late
    should be summarised by the specific one. Falls back to the caller's last
    line only when nothing matched — that is a formatting choice, not a
    substitute for a failure, and the full log is always on disk either way.
    """
    for pattern in ERROR_PATTERNS:
        for line in lines:
            match = pattern.match(line.strip())
            if match:
                return match.group("detail").strip()[:400]
    return fallback[:400]


def run_streaming(cmd: list[str], cwd: Path, log, env: dict | None = None) -> None:
    """Run a command, forwarding every output line to `log` as it arrives.
    Raises SystemExit with the most useful error line on a nonzero exit — no
    partial success is ever reported as success."""
    proc = subprocess.Popen(
        cmd, cwd=str(cwd), env=env,
        stdout=subprocess.PIPE, stderr=subprocess.STDOUT,
        text=True, bufsize=1)
    lines, last = [], ""
    for line in proc.stdout:
        line = line.rstrip("\n")
        if line:
            last = line
            lines.append(line)
        log(line)
    code = proc.wait()
    if code != 0:
        raise SystemExit(f"{cmd[0]} exited {code}: {first_error(lines, last)}")


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("slug")
    ap.add_argument("--build", action="store_true", help="run `effortless build` in the project directory")
    ap.add_argument("--reset-db", action="store_true", help="createdb + postgres-bootstrap/reset-rulebook-db.sh")
    args = ap.parse_args()
    if not (args.build or args.reset_db):
        ap.error("nothing to do — pass --build and/or --reset-db")

    def log(line):
        print(line, flush=True)

    domain_dir = find_domain_dir(args.slug)
    if args.build:
        build_project(args.slug, domain_dir, log)
    if args.reset_db:
        reset_db(args.slug, domain_dir, log)


if __name__ == "__main__":
    main()
