#!/usr/bin/env python3
"""oss-rulebook-to-csv — CLI-hosted local tool wrapping the repo's injector."""
import pathlib, sys

sys.path.insert(0, str(pathlib.Path(__file__).resolve().parents[2] / "orchestration"))
from local_tool_shim import run_injector

run_injector("csv/inject-into-csv.py")
