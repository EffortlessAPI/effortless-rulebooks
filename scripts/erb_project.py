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
import codecs
import json
import os
import re
import select
import signal
import subprocess
import sys
import time
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


AREAS = ("rulebook-examples", "toy-rulebooks")


def project_dir_of_build(start: Path) -> Path:
    """The project folder holding the effortless.json of the build running in
    `start`: the nearest folder at or above `start` directly under an area. A
    build's `-exec` step takes no arguments, so this is how it knows its project."""
    start = start.resolve()
    for d in (start, *start.parents):
        if d.parent.name in AREAS and d.parent.parent == REPO_ROOT:
            if not (d / "effortless.json").is_file():
                raise SystemExit(f"{d} is a project folder with no effortless.json — nothing declares this build.")
            return d
        if d == REPO_ROOT:
            break
    raise SystemExit(
        f"{start} is not inside a project under {', '.join(AREAS)} — "
        f"this step works on the project whose build runs it.")


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


PIN_KEYS = ("LastUrl", "LastVersionUsed", "PinnedVersion")


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


# A build that has not finished after this long is stuck, not slow: the largest
# project (procedural-knowledge-ontology) builds in a few minutes. On 2026-11-27 an
# older CLI crashed reading a step's error reply and its process never exited; the
# corpus runner waited on it for three hours with 34 projects still to build.
BUILD_TIME_LIMIT_SECONDS = 30 * 60

# The CLI's own summary of a build in which some step failed.
FAILED_STEP = re.compile(r"^\[cli\]\s+FAILED:\s*(?P<step>.+)$")
FAILED_SUMMARY = re.compile(r"^\[cli\]\s+BUILD (?:FAILED|FINISHED WITH \d+ FAILED STEP)")


def failed_steps(lines: list[str]) -> list[str]:
    """The steps a build's output says failed ("oss-rulebook-to-english  (/english)")."""
    steps = [m.group("step").strip() for m in (FAILED_STEP.match(l.strip()) for l in lines) if m]
    if not steps and any(FAILED_SUMMARY.match(l.strip()) for l in lines):
        steps = ["a step (the CLI did not name it)"]
    return steps


def build_project(slug: str, domain_dir: Path, log, time_limit: float = BUILD_TIME_LIMIT_SECONDS) -> None:
    """`effortless build` in the project directory: the whole pipeline declared
    in that project's effortless.json. Unpins first (see unpin()). Raises
    SystemExit on failure.

    Every step runs even when one fails (-continueOnError): a step that cannot run
    on this machine (oss-rulebook-to-english with no OPENAI_API_KEY) used to stop
    the build there and leave the nine steps after it, Postgres among them,
    unbuilt. The build still FAILS, naming every failed step: with
    -continueOnError the CLI exits 0, so its own summary is what is read."""
    effortless_json = domain_dir / "effortless.json"
    if not effortless_json.is_file():
        raise SystemExit(f"{effortless_json} does not exist — {slug} has no build pipeline.")
    unpin(effortless_json, log)
    log(f"[build] effortless build in {domain_dir.relative_to(REPO_ROOT) if domain_dir != REPO_ROOT else '.'}")
    lines = run_streaming(["effortless", "build", "-continueOnError"], cwd=domain_dir, log=log,
                          time_limit=time_limit)
    failed = failed_steps(lines)
    if failed:
        raise SystemExit(f"build finished with {len(failed)} failed step(s): {'; '.join(failed)}")


def postgres_output_dir(domain_dir: Path) -> Path | None:
    """Where this project's rulebook-to-postgres output lands, read off the
    transpiler's own RelativePath in effortless.json.

    There is no single conventional directory: the corpus uses postgres-bootstrap/,
    postgres/ and effortless-postgres/. Hardcoding one made four projects look
    like database failures in the 2026-09-09 baseline when they had simply put
    their SQL somewhere else. Returns None when the project registers no enabled
    rulebook-to-postgres step — that project has no database to reset, which is a
    skip, not a failure.
    """
    manifest_path = domain_dir / "effortless.json"
    if not manifest_path.is_file():
        raise SystemExit(f"{manifest_path} does not exist — cannot locate the project's Postgres output.")
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    transpilers = manifest.get("ProjectTranspilers")
    if transpilers is None:
        raise SystemExit(f"{manifest_path} has no ProjectTranspilers key — refusing to guess at its shape.")
    matches = [t for t in transpilers
               if "rulebook-to-postgres" in t.get("CommandLine", "") and not t.get("IsDisabled")]
    if not matches:
        return None
    rels = {t.get("RelativePath") or "/" for t in matches}
    if len(rels) > 1:
        raise SystemExit(
            f"{manifest_path} declares rulebook-to-postgres with conflicting RelativePaths {sorted(rels)} — "
            f"ambiguous output directory, fix the manifest.")
    return domain_dir / rels.pop().lstrip("/")


def reset_db(slug: str, domain_dir: Path, log) -> str | None:
    """Recreate the per-project database (only reset it in place when
    DATABASE_URL names one), then apply the freshly generated destructive
    dev-reset script. Returns the database URL used, or
    None when the project has no Postgres substrate to reset.

    reset-rulebook-db.sh is regenerated by every build and routinely comes back
    without its executable bit, so it is always invoked as `bash <script>`."""
    output_dir = postgres_output_dir(domain_dir)
    if output_dir is None:
        log("[db] no enabled rulebook-to-postgres step — this project has no database to reset")
        return None
    script = output_dir / "reset-rulebook-db.sh"
    if not script.is_file():
        raise SystemExit(
            f"{script} does not exist. rulebook-to-postgres is enabled for {slug} with output "
            f"{output_dir.name}/, so its build should have generated it — check the build log.")
    url = database_url(slug)
    name = db_name(slug)
    if os.environ.get("DATABASE_URL"):
        # A database someone named is theirs: reset it, never drop it.
        log("[db] DATABASE_URL is set: resetting that database in place, not recreating it")
    else:
        # The project's own database is derived, like the root's (init-root-db.sh):
        # recreate it. The generated SQL says CREATE OR REPLACE FUNCTION, which
        # Postgres refuses once a function's return type changed, and 01-*.sql never
        # drops a table, so a database an older build left behind cannot be trusted.
        admin = "postgresql://postgres@localhost:5432/postgres"
        log(f"[db] recreating {name}")
        for sql in (f"DROP DATABASE IF EXISTS {name} WITH (FORCE);", f"CREATE DATABASE {name};"):
            run_streaming(["psql", "-v", "ON_ERROR_STOP=1", "-Atqc", sql, admin], cwd=domain_dir, log=log)
    log(f"[db] bash {output_dir.name}/reset-rulebook-db.sh against {name}")
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


LINE_BREAK = re.compile(r"\r\n|\r|\n")


def run_streaming(cmd: list[str], cwd: Path, log, env: dict | None = None,
                  time_limit: float | None = None) -> list[str]:
    """Run a command, forwarding every output line to `log` as it arrives, and
    return its non-blank lines. Raises SystemExit with the most useful error line
    on a nonzero exit — no partial success is ever reported as success.

    With `time_limit` (seconds), a command still running then is stopped (its
    whole process group) and reported as a failure. The output is read with a
    deadline rather than line by line, because a stuck process can hold the pipe
    open without writing to it, and even past SIGKILL (the 2026-11-27 CLI sat in
    the kernel's exit path, unkillable, for hours): waiting for the pipe to close
    would wait for ever."""
    proc = subprocess.Popen(
        cmd, cwd=str(cwd), env=env,
        stdout=subprocess.PIPE, stderr=subprocess.STDOUT,
        start_new_session=time_limit is not None)
    deadline = None if time_limit is None else time.monotonic() + time_limit
    decoder = codecs.getincrementaldecoder("utf-8")("replace")
    fd = proc.stdout.fileno()
    lines, last, pending = [], "", ""

    def emit(line: str) -> None:
        nonlocal last
        if line:
            last = line
            lines.append(line)
        log(line)

    timed_out = False
    while True:
        wait = None if deadline is None else deadline - time.monotonic()
        if wait is not None and wait <= 0:
            timed_out = True
            break
        ready, _, _ = select.select([fd], [], [], wait)
        if not ready:
            continue
        chunk = os.read(fd, 65536)
        if not chunk:
            break
        pending += decoder.decode(chunk)
        # A "\r" at the end may be the first half of a "\r\n" still to come.
        held = "\r" if pending.endswith("\r") else ""
        parts = LINE_BREAK.split(pending[:len(pending) - len(held)])
        pending = parts.pop() + held
        for part in parts:
            emit(part)
    pending += decoder.decode(b"", final=True)
    if pending.rstrip("\r"):
        emit(pending.rstrip("\r"))

    if timed_out:
        try:
            os.killpg(proc.pid, signal.SIGKILL)
        except (ProcessLookupError, PermissionError):
            pass
        try:
            proc.wait(timeout=10)
        except subprocess.TimeoutExpired:
            pass
        proc.stdout.close()
        minutes = f"{time_limit / 60:g} minute(s)" if time_limit >= 60 else f"{time_limit:g} second(s)"
        raise SystemExit(f"{cmd[0]} was still running after {minutes} and was stopped "
                         f"(last output: {first_error(lines, last)})")
    proc.stdout.close()
    code = proc.wait()
    if code != 0:
        raise SystemExit(f"{cmd[0]} exited {code}: {first_error(lines, last)}")
    return lines


def main() -> None:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("slug")
    ap.add_argument("--build", action="store_true", help="run `effortless build` in the project directory")
    ap.add_argument("--reset-db", action="store_true", help="recreate erb_<slug> + postgres-bootstrap/reset-rulebook-db.sh")
    ap.add_argument("--postgres-dir", action="store_true",
                    help="print the folder the project's rulebook-to-postgres step writes to, and exit")
    args = ap.parse_args()
    if args.postgres_dir:
        out = postgres_output_dir(find_domain_dir(args.slug))
        if out is None:
            raise SystemExit(f"{args.slug} has no enabled rulebook-to-postgres step")
        print(out)
        return
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
