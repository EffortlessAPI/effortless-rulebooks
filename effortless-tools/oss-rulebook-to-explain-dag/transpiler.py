#!/usr/bin/env python3
"""oss-rulebook-to-explain-dag — CLI-hosted local tool wrapping the repo's injector."""
import pathlib, sys

sys.path.insert(0, str(pathlib.Path(__file__).resolve().parents[2] / "orchestration"))
from local_tool_shim import run_injector

run_injector("explain-dag/inject-into-explain-dag.py")
