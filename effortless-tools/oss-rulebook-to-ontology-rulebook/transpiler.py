#!/usr/bin/env python3
"""oss-rulebook-to-ontology-rulebook: a rulebook -> its ontology form (orchestration/ontology_form.py)."""
import pathlib, sys

sys.path.insert(0, str(pathlib.Path(__file__).resolve().parents[2] / "orchestration"))
from local_tool_shim import run_rulebook_transform
from ontology_form import inject

# -i <slug>-rulebook.json  ->  <slug>-rulebook.ontology.json
run_rulebook_transform(inject, ".ontology.json")
