#!/usr/bin/env python3
"""take-test.py for the effortless-owl execution substrate.

Reasons over the COMMERCIAL rulebook-to-owl output for the active domain
($ERB_DOMAIN_DIR/effortless-owl/src/) and writes test-answers for grading.

The licensed counterpart to the open-source `owl` substrate, mirroring how
effortless-python relates to python. The open-source `owl` substrate grades
its own repo-local injector (execution-substrates/owl/inject-into-owl.py), NOT
a project's owl/ transpiler output — so a project that registers rulebook-to-owl
and runs only `owl` has never had that tool graded at all.

Both emit the same effortless-ntwf: IRIs and SHACL-AF SPARQLRule shapes, so the
reasoning runner is shared rather than copied: this file imports owl/take-test.py
and hands it a different src dir and substrate name. The ABox the commercial tool
emits carries raw fields only, so every derived value is computed by the SPARQL
engine, never read back from the compiled rulebook.
"""

import importlib.util
import os
import sys
from pathlib import Path

domain_dir = os.environ.get("ERB_DOMAIN_DIR")
if not domain_dir or not Path(domain_dir).is_dir():
    raise SystemExit(
        f"ERB_DOMAIN_DIR is not set or does not exist (ERB_DOMAIN_DIR={domain_dir!r}). "
        "Invoke this substrate via the orchestrator."
    )

owl_runner_path = Path(__file__).resolve().parent.parent / "owl" / "take-test.py"
spec = importlib.util.spec_from_file_location("owl_take_test", owl_runner_path)
owl_runner = importlib.util.module_from_spec(spec)
spec.loader.exec_module(owl_runner)

owl_runner.main(
    Path(domain_dir) / "effortless-owl" / "src",
    "effortless-owl",
    "Register rulebook-to-owl in this project's effortless.json with "
    "RelativePath /effortless-owl and run `effortless build`.",
)
