#!/usr/bin/env python3
"""A project build runs every step, fails when any step failed, and ends.

Three things went wrong in one corpus run on 2026-11-27:

1. oss-rulebook-to-english cannot run without OPENAI_API_KEY, and the CLI stopped
   every build at it: step 9 of 18, so Postgres, the explainer, RuleSpeak and
   Entity Framework were never rebuilt for any project with an English step.
   erb_project.build_project now passes -continueOnError.
2. With -continueOnError the CLI exits 0 even when a step failed ("BUILD FINISHED
   WITH 1 FAILED STEP(S)"), so the build is failed from the CLI's own summary.
3. An older CLI crashed reading a step's error reply and its process never exited
   (it sat in the kernel's exit path, unkillable). The runner waited on its output
   for three hours. A build now has a time limit, and the limit holds even while
   something the build started keeps the output pipe open.

    python3 scripts/tests/test_build_runs_every_step.py        (or pytest)
"""
import json
import os
import shutil
import signal
import sys
import tempfile
import textwrap
import time
import unittest
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "scripts"))
import erb_project  # noqa: E402
from erb_project import build_project  # noqa: E402

ENGLISH_FAILED = """\
 **** /english: oss-rulebook-to-english ****
[oss-rulebook-to-english]   No specification.md exists and LLM is unreachable.
 **** /postgres-bootstrap: rulebook-to-postgres ****
[cli] BUILD FINISHED WITH 1 FAILED STEP(S) — 17 step(s) succeeded and are usable.
[cli]   FAILED: oss-rulebook-to-english  (/english)
[cli]     error:   Local tool 'oss-rulebook-to-english' exited with code 2.
"""


class FakeCli(unittest.TestCase):
    """An `effortless` on PATH that records its arguments and plays a script."""

    def setUp(self):
        self.tmp = Path(tempfile.mkdtemp(prefix="erb-build-"))
        self.bin = self.tmp / "bin"
        self.bin.mkdir()
        self.project = self.tmp / "project"
        self.project.mkdir()
        (self.project / "effortless.json").write_text(json.dumps({"ProjectTranspilers": []}))
        self.args_file = self.tmp / "args"
        self.saved_path = os.environ["PATH"]
        os.environ["PATH"] = f"{self.bin}{os.pathsep}{self.saved_path}"
        self.saved_root = erb_project.REPO_ROOT
        erb_project.REPO_ROOT = self.tmp  # build_project logs the folder relative to the repo
        self.lines = []

    def tearDown(self):
        os.environ["PATH"] = self.saved_path
        erb_project.REPO_ROOT = self.saved_root
        pid_file = self.tmp / "holder.pid"
        if pid_file.exists():
            try:
                os.kill(int(pid_file.read_text()), signal.SIGKILL)
            except (ProcessLookupError, ValueError):
                pass
        shutil.rmtree(self.tmp, ignore_errors=True)

    def cli(self, body: str):
        script = self.bin / "effortless"
        script.write_text(f'#!/bin/bash\necho "$@" > "{self.args_file}"\n{body}\n')
        script.chmod(0o755)

    def build(self, **kw):
        build_project("probe", self.project, self.lines.append, **kw)


class OneFailedStep(FakeCli):
    def test_every_step_is_asked_for(self):
        self.cli("exit 0")
        self.build()
        self.assertIn("-continueOnError", self.args_file.read_text().split())

    def test_a_failed_step_fails_the_build_though_the_cli_exits_0(self):
        self.cli(f"cat <<'EOF'\n{ENGLISH_FAILED}EOF\nexit 0")
        with self.assertRaises(SystemExit) as caught:
            self.build()
        self.assertIn("oss-rulebook-to-english", str(caught.exception))
        self.assertIn("1 failed step", str(caught.exception))

    def test_a_clean_build_passes(self):
        self.cli("echo '[cli] BUILD SUCCEEDED — 18 step(s).'\nexit 0")
        self.build()
        self.assertIn("[cli] BUILD SUCCEEDED — 18 step(s).", self.lines)

    def test_a_cli_that_exits_nonzero_still_fails(self):
        self.cli("echo '[cli]   FAILED: rulebook-to-postgres  (/postgres)'\nexit 255")
        with self.assertRaises(SystemExit) as caught:
            self.build()
        self.assertIn("rulebook-to-postgres", str(caught.exception))


class AStuckBuild(FakeCli):
    def test_is_stopped_at_its_time_limit(self):
        self.cli("echo started\nsleep 60")
        began = time.monotonic()
        with self.assertRaises(SystemExit) as caught:
            self.build(time_limit=1)
        self.assertLess(time.monotonic() - began, 15)
        self.assertIn("still running after 1 second", str(caught.exception))
        self.assertIn("started", self.lines)

    def test_is_stopped_even_while_something_unkillable_holds_its_output(self):
        # A process in a session of its own keeps the output pipe open after the
        # build's process group is killed: what the crashed CLI's stuck process did.
        holder = textwrap.dedent(f"""\
            python3 -c 'import os, time
            os.setsid()
            open("{self.tmp}/holder.pid", "w").write(str(os.getpid()))
            time.sleep(60)' &
            echo started
            sleep 60""")
        self.cli(holder)
        began = time.monotonic()
        with self.assertRaises(SystemExit) as caught:
            self.build(time_limit=1)
        self.assertLess(time.monotonic() - began, 15, "the build waited for the pipe to close")
        self.assertIn("still running after", str(caught.exception))


if __name__ == "__main__":
    unittest.main()
