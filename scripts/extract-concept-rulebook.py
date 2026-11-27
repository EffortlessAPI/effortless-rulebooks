#!/usr/bin/env python3
"""A build step for a project whose rulebook is in the ontology form: write its
concept rulebook beside the step.

The hub of such a project (rulebook-examples/talismans-southpark-ontology) holds
things, properties and statements as rows. Its concept rulebook is the same
ontology as ordinary tables (a table of individuals, a table per class, a table
or a column per property, ERBTables and ERBFields): the file the generated
rulebook editor, Excel and a reader open when they want the concepts and not the
triples. orchestration/ontology_form.py is the one mapping; this step only calls
its `extract` on the hub and writes `<slug>-concepts.json` into the folder the
step runs in.

It runs after compile-rulebook, because the mapping reads which facts are in
effect from the values that step bakes into the hub; it computes nothing.

Like scripts/reset-project-db.py it is committed executable and takes no
arguments (the CLI re-parses every `-exec` token as one of its own options): the
project is the one whose build runs it, found by walking up from the step's
working directory. An edit made in the concept rulebook comes back into the hub
through `ontology_form.py inject <concepts> <hub out> --base <hub>` (the project
app's "Load the concept rulebook file").
"""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "orchestration"))
from erb_project import project_dir_of_build, rulebook_path  # noqa: E402
import ontology_form  # noqa: E402


def concept_file(project: Path, folder: Path) -> Path:
    return folder / f"{project.name}-concepts.json"


def main() -> None:
    if len(sys.argv) > 1:
        raise SystemExit("extract-concept-rulebook.py takes no arguments; it writes the concept "
                         "rulebook of the project it is run inside, into the folder it is run in.")
    folder = Path.cwd().resolve()
    project = project_dir_of_build(folder)
    if folder == project or folder == project / "effortless-rulebook":
        raise SystemExit(f"Run this step in a folder of its own (concept-rulebook/), not in {folder}: "
                         f"the concept rulebook is a second document, never beside or over the hub.")
    hub = rulebook_path(project.name, project)
    try:
        concepts = ontology_form.extract(ontology_form.read_rulebook(hub))
    except ontology_form.OntologyFormError as error:
        raise SystemExit(f"{hub.relative_to(project)}: {error}")
    target = concept_file(project, folder)
    ontology_form.write_rulebook(concepts, target)
    tables = [name for name, value in concepts.items() if ontology_form.is_table(value)]
    print(f"[concept-rulebook] {target.relative_to(project)}: {len(tables)} tables from {hub.relative_to(project)}")


if __name__ == "__main__":
    main()
