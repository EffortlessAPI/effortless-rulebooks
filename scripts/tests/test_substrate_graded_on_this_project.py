#!/usr/bin/env python3
"""A substrate is graded on code generated for the project being graded.

Every shared substrate under execution-substrates/<name>/ keeps ONE generated copy
(python_only_erb_simulator.py, erb_sdk.go, the OWL files, ...), which belongs to
whichever project's injector ran there last. orchestrate.sh runs each substrate's
inject-substrate.sh (regenerate for the active project, then take-test.sh), but
test-orchestrator.run_substrate_test, which scripts/run-conformance.py and the
corpus runner use, ran take-test.sh alone. On 2026-11-27 acme-llc's python, golang
and owl scored 0/6 ("unknown entity 'customers'": the shared module was another
project's), and traffic-ticket-contest's python 73%. The grader now runs
inject-substrate.sh when the substrate has one, as orchestrate.sh does.

    python3 scripts/tests/test_substrate_graded_on_this_project.py        (or pytest)
"""
import importlib.util
import os
import shutil
import tempfile
import unittest
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]


def load_orchestrator():
    os.environ["ERB_DOMAIN"] = "acme-llc"
    spec = importlib.util.spec_from_file_location(
        "test_orchestrator_substrate_run", REPO / "orchestration" / "test-orchestrator.py")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class WhichScriptRuns(unittest.TestCase):
    def setUp(self):
        self.saved_domain = os.environ.get("ERB_DOMAIN")
        self.to = load_orchestrator()
        self.dir = Path(tempfile.mkdtemp(prefix="erb-substrates-"))
        self.saved_dir = self.to.SUBSTRATES_DIR
        self.to.SUBSTRATES_DIR = str(self.dir)

    def tearDown(self):
        self.to.SUBSTRATES_DIR = self.saved_dir
        shutil.rmtree(self.dir, ignore_errors=True)
        if self.saved_domain is None:
            os.environ.pop("ERB_DOMAIN", None)
        else:
            os.environ["ERB_DOMAIN"] = self.saved_domain

    def substrate(self, name: str, inject: bool):
        d = self.dir / name
        d.mkdir()
        (d / "take-test.sh").write_text(f'echo test >> "{d}/ran"\n')
        if inject:
            # What every inject-substrate.sh does: regenerate for ERB_DOMAIN, then test.
            (d / "inject-substrate.sh").write_text(
                f'echo "inject $ERB_DOMAIN" >> "{d}/ran"\nbash "{d}/take-test.sh"\n')
        return d

    def test_a_substrate_with_an_injector_is_regenerated_for_this_project_first(self):
        d = self.substrate("python", inject=True)
        ok, error, _, _ = self.to.run_substrate_test("python")
        self.assertTrue(ok, error)
        self.assertEqual((d / "ran").read_text().split("\n")[:2], ["inject acme-llc", "test"])

    def test_a_test_only_substrate_runs_its_test(self):
        d = self.substrate("yaml", inject=False)
        ok, error, _, _ = self.to.run_substrate_test("yaml")
        self.assertTrue(ok, error)
        self.assertEqual((d / "ran").read_text(), "test\n")

    def test_a_failed_injection_fails_the_substrate(self):
        d = self.substrate("golang", inject=True)
        (d / "inject-substrate.sh").write_text("echo 'go: build failed' >&2\nexit 1\n")
        ok, error, _, _ = self.to.run_substrate_test("golang")
        self.assertFalse(ok)
        self.assertIn("go: build failed", error)


if __name__ == "__main__":
    unittest.main()
