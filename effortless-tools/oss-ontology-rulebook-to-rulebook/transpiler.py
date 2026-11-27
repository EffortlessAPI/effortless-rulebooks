#!/usr/bin/env python3
"""oss-ontology-rulebook-to-rulebook: an ontology-form rulebook -> concept tables (orchestration/ontology_form.py)."""
import pathlib, sys

sys.path.insert(0, str(pathlib.Path(__file__).resolve().parents[2] / "orchestration"))
from local_tool_shim import run_rulebook_transform
from ontology_form import extract

# -i <name>.json  ->  <name>.concepts.json
run_rulebook_transform(extract, ".concepts.json")
