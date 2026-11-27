#!/usr/bin/env python3
"""The database step of a project's `effortless build`: reset that project's
database from the SQL rulebook-to-postgres just generated.

Every project's build used to end with `-exec ./reset-rulebook-db.sh`, which
failed in two silent ways (found on star-trek, 2026-11-26):

1. The build regenerates reset-rulebook-db.sh, and a regenerated file comes
   back WITHOUT its executable bit (cr-20-02, a CLI defect). Whenever the
   script's content changed, the step printed `The system couldn't find the
   command "./reset-rulebook-db.sh"`, the build stopped right there, skipped
   every later step, and still exited 0.
2. The script's default database was `demo` (see fix-db-reset-step.py), not
   the `erb_<slug>` the app reads.

This file is committed with its executable bit and is never regenerated, so
`-exec` can always run it. It takes no arguments (the CLI re-parses every
`-exec` token as one of its own options): the project is the folder whose
effortless.json declares the build that is running, found by walking up from
the step's working directory. The work itself is erb_project.reset_db, the one
implementation the corpus runner and run-conformance.py also use: recreate the
database, then `bash reset-rulebook-db.sh <url>` against erb_<slug> (DATABASE_URL
overrides, as everywhere).

fix-db-reset-step.py points every project's step here.
"""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from erb_project import project_dir_of_build as project_dir, reset_db  # noqa: E402,F401


def main() -> None:
    if len(sys.argv) > 1:
        raise SystemExit("reset-project-db.py takes no arguments; it resets the project it is run inside.")
    d = project_dir(Path.cwd())
    url = reset_db(d.name, d, print)
    if url is None:
        raise SystemExit(f"{d.name} runs a database step but registers no enabled rulebook-to-postgres step.")


if __name__ == "__main__":
    main()
