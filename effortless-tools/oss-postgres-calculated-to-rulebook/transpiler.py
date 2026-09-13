#!/usr/bin/env python3
"""oss-postgres-calculated-to-rulebook — CLI-hosted local tool wrapping the repo's injector."""
import pathlib, sys

sys.path.insert(0, str(pathlib.Path(__file__).resolve().parents[2] / "orchestration"))
from local_tool_shim import run_injector

# The step sits in /effortless-rulebook and sends both inputs:
#   -i <slug>-rulebook.json,../postgres-bootstrap/.pg-raw-data.json
# The updated rulebook comes back under its own name, so the CLI recognises the
# output as the input it was read from and applies it as an in-place upsert
# (written, never ledgered as generated, never deleted by a clean).
run_injector(
    "postgres-calculated-to-rulebook/inject-into-postgres-calculated-to-rulebook.py",
    extra_inputs={"ERB_PG_RAW_DATA_PATH": ".pg-raw-data.json"},
)
