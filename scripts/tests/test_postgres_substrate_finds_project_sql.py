#!/usr/bin/env python3
"""The effortless-postgres substrate regenerates the project's SQL where the project keeps it.

A project's rulebook-to-postgres output is wherever its step's RelativePath says:
postgres-bootstrap/, postgres/ or effortless-postgres/ (erb_project.postgres_output_dir reads
it). execution-substrates/effortless-postgres/inject-substrate.sh insisted on
$ERB_DOMAIN_DIR/postgres-bootstrap and refused every other project ("FATAL: ... its
postgres-bootstrap/ subdir is missing"). Once grading ran each substrate's injector
(2026-11-27), that took effortless-math, naive-set-theory, lazr-coulombs-law,
ross-style-business-rules, veritasium and causal-autoimmune-architecture from 100% to an error.
The script now asks erb_project.py --postgres-dir.

    python3 scripts/tests/test_postgres_substrate_finds_project_sql.py        (or pytest)
"""
import subprocess
import sys
import unittest
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
INJECT = REPO / "execution-substrates" / "effortless-postgres" / "inject-substrate.sh"


def postgres_dir(slug: str) -> str:
    return subprocess.run([sys.executable, str(REPO / "scripts" / "erb_project.py"), slug, "--postgres-dir"],
                          capture_output=True, text=True, check=True).stdout.strip()


class WhereTheSqlIs(unittest.TestCase):
    def test_a_project_that_keeps_it_in_postgres(self):
        self.assertEqual(postgres_dir("naive-set-theory"), str(REPO / "rulebook-examples" / "naive-set-theory" / "postgres"))

    def test_a_project_that_keeps_it_in_postgres_bootstrap(self):
        self.assertEqual(postgres_dir("star-trek"), str(REPO / "toy-rulebooks" / "star-trek" / "postgres-bootstrap"))

    def test_the_substrate_asks_erb_project_and_names_no_folder_of_its_own(self):
        script = INJECT.read_text(encoding="utf-8")
        self.assertIn("erb_project.py", script)
        self.assertIn("--postgres-dir", script)
        self.assertNotIn("$ERB_DOMAIN_DIR/postgres-bootstrap", script)


if __name__ == "__main__":
    unittest.main()
