#!/usr/bin/env python3
"""Bridge from the effortless CLI's local-tool script contract to the repo's injectors.

The CLI hosts every tool under ``effortless-tools/<name>/`` and hands a script
tool two directories: ``EFFORTLESS_INPUT_DIR``, already holding the request's
input fileset (the rulebook the build step named with ``-i``), and an empty
``EFFORTLESS_OUTPUT_DIR`` whose contents become the step's output FileSet.

The injectors under ``execution-substrates/`` predate that contract and speak
``ERB_RULEBOOK_PATH`` / ``ERB_OUTPUT_DIR``, so this translates one to the other.
It is the whole replacement for ssotme-proxy, which had to recover those same
two paths by finding the CLI process behind the inbound TCP connection and
reading its cwd -- the CLI now simply hands them over.
"""

import json
import os
import subprocess
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent
SUBSTRATES = REPO_ROOT / "execution-substrates"

# The injectors overwrite their output on every build, exactly as they did when
# ssotme-proxy wrote it straight to disk. A file the CLI collects with no
# declared mode is written once and NEVER overwritten, which would silently
# freeze every substrate after its first build, so the mode is declared here.
OVERWRITE_MODES = {"**/*": "Always"}


def _assert_oss_prefixed() -> None:
    """Every repo-local tool must be named oss-<tool>.

    The bare names belong to the commercial catalog as
    ``effortless/effortless/<tool>``; the prefix is the only thing stopping a
    repo-local route from shadowing one for every project on the machine. The
    retired ssotme-proxy enforced this at startup and refused to boot without
    it. The CLI's host knows nothing of the convention, so the check lives here
    and runs on every invocation -- it inspects the whole directory, not just
    the calling tool, so any tool call catches a mis-named sibling.
    """
    tools_dir = REPO_ROOT / "effortless-tools"
    offenders = sorted(
        d.name for d in tools_dir.iterdir()
        if d.is_dir() and not d.name.startswith("oss-")
    )
    if offenders:
        raise SystemExit(
            f"Repo-local tools missing the 'oss-' prefix: {offenders}. Rename "
            f"each to oss-<tool> so it can never shadow a commercial catalog "
            f"tool of the same bare name."
        )


def _require_dir(var: str) -> Path:
    raw = os.environ.get(var)
    if not raw:
        raise SystemExit(
            f"{var} is not set. This script is a local tool for the effortless "
            f"CLI and must be invoked by the CLI's local tool host "
            f"(`effortless serve`, or an ephemeral host during a build)."
        )
    path = Path(raw)
    if not path.is_dir():
        raise SystemExit(f"{var}={raw} is not a directory.")
    return path


def _rulebook_from(input_dir: Path, extra_names: set) -> Path:
    """The one rulebook the build step sent. Anything else is a hard error."""
    candidates = sorted(
        p for p in input_dir.rglob("*.json")
        if p.is_file() and p.name not in extra_names
    )
    if len(candidates) != 1:
        raise SystemExit(
            f"Expected exactly one input file in EFFORTLESS_INPUT_DIR={input_dir}, "
            f"found {len(candidates)}: {[c.name for c in candidates]}. The build "
            f"step must name its rulebook with `-i <path-to-rulebook.json>`."
        )
    return candidates[0]


def run_injector(relative_script: str, extra_inputs: dict | None = None) -> None:
    """Run one execution-substrate injector under the CLI's script contract.

    ``extra_inputs`` maps an environment variable to the file name of an input
    the injector needs besides the rulebook (``{"ERB_PG_RAW_DATA_PATH":
    ".pg-raw-data.json"}``). The build step sends each one with ``-i``; the CLI
    flattens every input to its bare file name in EFFORTLESS_INPUT_DIR, and a
    pattern that matched nothing arrives as no file at all, so a missing one is
    a hard error naming it.
    """
    _assert_oss_prefixed()
    injector = (SUBSTRATES / relative_script).resolve()
    if not injector.is_file():
        raise SystemExit(f"Injector not found: {injector}")

    extra_inputs = extra_inputs or {}
    input_dir = _require_dir("EFFORTLESS_INPUT_DIR")
    output_dir = _require_dir("EFFORTLESS_OUTPUT_DIR")
    rulebook = _rulebook_from(input_dir, set(extra_inputs.values()))

    env = os.environ.copy()
    env["ERB_RULEBOOK_PATH"] = str(rulebook)
    env["ERB_OUTPUT_DIR"] = str(output_dir)
    for var, file_name in extra_inputs.items():
        path = input_dir / file_name
        if not path.is_file():
            raise SystemExit(
                f"Required input '{file_name}' was not sent to "
                f"{os.environ.get('EFFORTLESS_TOOL_NAME', 'this tool')}. The build "
                f"step must name it with -i (comma-separated after the rulebook), "
                f"and the file must exist where that path points."
            )
        env[var] = str(path)

    result = subprocess.run(
        [sys.executable, str(injector)],
        cwd=str(output_dir),
        env=env,
    )
    if result.returncode != 0:
        raise SystemExit(result.returncode)

    (output_dir / "effortless-overwrite-modes.json").write_text(
        json.dumps(OVERWRITE_MODES, indent=2) + "\n", encoding="utf-8"
    )
