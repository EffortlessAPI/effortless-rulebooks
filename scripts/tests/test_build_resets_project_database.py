#!/usr/bin/env python3
"""`effortless build` in a project must reset THAT project's database.

A project's build runs rulebook-to-postgres and then `-exec ./reset-rulebook-db.sh`.
The generated script's default connection is `.../<database>`, where <database> is
whatever the step passed as `-p database=`, and `demo` when it passed nothing. None
of the 38 projects passed it, so every plain `effortless build` reset a shared
database called `demo` while the project's app (and the corpus runner, via
scripts/erb_project.db_name) read `erb_<slug>`. The build reported green, the app
kept serving whatever an older build had left in `erb_<slug>`, and an edit to the
rulebook never reached the screen. star-trek's explorer is where it was noticed
(2026-11-26): the live database still held a months-old function for Series.Rating.

The same step also failed whenever the build regenerated reset-rulebook-db.sh:
the new file has no executable bit (cr-20-02), `-exec ./reset-rulebook-db.sh`
printed "couldn't find the command", and the build stopped there and exited 0.
It now runs scripts/reset-project-db.py, committed executable, never regenerated.

This holds on the repository as it is: every project whose build resets its
database names its own database on its rulebook-to-postgres step (the name
erb_project.db_name derives from the folder), and its database step runs the
shared script, by a path that resolves from the step's working directory.

    python3 scripts/tests/test_build_resets_project_database.py        (or pytest)
"""
import importlib.util
import json
import os
import shutil
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "scripts"))
from erb_project import db_name, reset_db  # noqa: E402

spec = importlib.util.spec_from_file_location("fix_db_reset_step", REPO / "scripts" / "fix-db-reset-step.py")
fix = importlib.util.module_from_spec(spec)
spec.loader.exec_module(fix)


def postgres_steps(manifest: dict) -> list[dict]:
    return [t for t in manifest["ProjectTranspilers"]
            if t.get("CommandLine", "").startswith("rulebook-to-postgres") and not t.get("IsDisabled")]


def runs_reset_script(manifest: dict) -> bool:
    return any(fix.is_database_step(t) for t in manifest["ProjectTranspilers"])


class ThisRepository(unittest.TestCase):
    def test_every_project_build_resets_its_own_database(self):
        wrong = []
        checked = 0
        for path in fix.manifests():
            manifest = json.loads(path.read_text(encoding="utf-8"))
            if not runs_reset_script(manifest):
                continue
            want = f"-p database={db_name(path.parent.name)}"
            for step in postgres_steps(manifest):
                checked += 1
                if fix.database_param(step["CommandLine"]) != db_name(path.parent.name):
                    wrong.append(f"{path.relative_to(REPO)}: {step['CommandLine']!r} lacks {want!r}")
        self.assertGreater(checked, 0, "found no project that runs reset-rulebook-db.sh — the test is looking in the wrong place")
        self.assertEqual(wrong, [], "\n".join(["builds that reset some other database:"] + wrong))


    def test_every_database_step_runs_the_shared_script(self):
        wrong = []
        for path in fix.manifests():
            manifest = json.loads(path.read_text(encoding="utf-8"))
            for step in manifest["ProjectTranspilers"]:
                if not fix.is_database_step(step):
                    continue
                target = step["CommandLine"].split(None, 1)[1].strip()
                cwd = path.parent / step["RelativePath"].lstrip("/")
                if (cwd / target).resolve() != fix.SHARED:
                    wrong.append(f"{path.relative_to(REPO)}: {step['CommandLine']!r} (from {step['RelativePath']})")
        self.assertEqual(wrong, [], "\n".join(["database steps that do not run scripts/reset-project-db.py:"] + wrong))

    def test_the_shared_script_is_executable_in_git(self):
        mode = subprocess.run(["git", "ls-files", "-s", "scripts/reset-project-db.py"], cwd=REPO,
                              capture_output=True, text=True, check=True).stdout.split(" ")[0]
        self.assertEqual(mode, "100755", "scripts/reset-project-db.py must be committed executable: -exec runs it directly")


class FindingTheProject(unittest.TestCase):
    def setUp(self):
        spec = importlib.util.spec_from_file_location("reset_project_db", REPO / "scripts" / "reset-project-db.py")
        self.mod = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(self.mod)

    def test_from_the_postgres_folder(self):
        d = REPO / "toy-rulebooks" / "star-trek"
        self.assertEqual(self.mod.project_dir(d / "postgres-bootstrap"), d)

    def test_outside_a_project_is_refused(self):
        with self.assertRaises(SystemExit):
            self.mod.project_dir(REPO / "scripts")


def local_postgres() -> bool:
    if shutil.which("psql") is None:
        return False
    probe = subprocess.run(["psql", "-Atqc", "select 1", "postgresql://postgres@localhost:5432/postgres"],
                           capture_output=True, text=True)
    return probe.returncode == 0


@unittest.skipUnless(local_postgres(), "needs psql and a Postgres on localhost:5432")
class ASecondResetAfterTheRulesChanged(unittest.TestCase):
    """The database step must work on a database an older build left behind.

    The generated 02-create-functions.sql says CREATE OR REPLACE FUNCTION, and
    Postgres refuses that when a function's return type changed ("cannot change
    return type of existing function"). reset_db only ran `createdb` (a no-op when
    the database exists), so the first rulebook change that turned a field from a
    number into text (procedural-knowledge-ontology, 2026-11-27:
    get_communication_policies_quiet_hours_start) stopped the corpus runner at its
    db phase and graded nothing. The root's init-root-db.sh already recreates its
    database for the same reason; a project's database is derived the same way.
    """
    SLUG = "reset-probe-for-tests"

    def setUp(self):
        self.saved_url = os.environ.pop("DATABASE_URL", None)
        self.dir = Path(tempfile.mkdtemp(prefix="erb-reset-"))
        (self.dir / "effortless.json").write_text(json.dumps({"ProjectTranspilers": [
            {"CommandLine": "rulebook-to-postgres -i x.json", "RelativePath": "/postgres-bootstrap"}]}))
        out = self.dir / "postgres-bootstrap"
        out.mkdir()
        # What the generated script does that matters here: CREATE OR REPLACE a
        # function whose return type comes from the rules.
        (out / "reset-rulebook-db.sh").write_text(
            'set -e\nRET=$(cat "$(dirname "$0")/return-type")\n'
            'psql "$1" -v ON_ERROR_STOP=1 -qc "CREATE OR REPLACE FUNCTION get_probe() RETURNS $RET '
            'LANGUAGE sql AS \\$\\$ SELECT NULL::$RET \\$\\$;"\n')
        self.lines = []

    def tearDown(self):
        subprocess.run(["psql", "-qc", f"DROP DATABASE IF EXISTS {db_name(self.SLUG)} WITH (FORCE)",
                        "postgresql://postgres@localhost:5432/postgres"], capture_output=True)
        shutil.rmtree(self.dir, ignore_errors=True)
        if self.saved_url is not None:
            os.environ["DATABASE_URL"] = self.saved_url

    def reset(self, return_type: str):
        (self.dir / "postgres-bootstrap" / "return-type").write_text(return_type)
        reset_db(self.SLUG, self.dir, self.lines.append)

    def test_a_function_whose_return_type_changed_is_reset_too(self):
        self.reset("numeric")
        try:
            self.reset("text")
        except SystemExit as e:
            self.fail(f"the second reset failed: {e}")
        returns = subprocess.run(
            ["psql", "-Atqc", "select pg_get_function_result('get_probe'::regproc)",
             f"postgresql://postgres@localhost:5432/{db_name(self.SLUG)}"],
            capture_output=True, text=True, check=True).stdout.strip()
        self.assertEqual(returns, "text")


class TheFix(unittest.TestCase):
    def test_adds_the_parameter(self):
        self.assertEqual(fix.with_database("rulebook-to-postgres -i a.json -p drop_all=true", "erb_x"),
                         "rulebook-to-postgres -i a.json -p drop_all=true -p database=erb_x")

    def test_replaces_a_wrong_one(self):
        self.assertEqual(fix.with_database("rulebook-to-postgres -i a.json -p database=demo -p stage=dev", "erb_x"),
                         "rulebook-to-postgres -i a.json -p database=erb_x -p stage=dev")

    def test_leaves_a_right_one(self):
        line = "rulebook-to-postgres -i a.json -p database=erb_x"
        self.assertEqual(fix.with_database(line, "erb_x"), line)


if __name__ == "__main__":
    unittest.main()
