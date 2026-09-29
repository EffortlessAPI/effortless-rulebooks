import json
import os
import sys

import pytest

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
for p in ("tools", "substrates/transparent", "substrates/interpreter", "substrates/tangled"):
    sys.path.insert(0, os.path.join(ROOT, p))

import r0lang as L  # noqa: E402


@pytest.fixture(scope="session")
def root():
    return ROOT


@pytest.fixture(scope="session")
def spec():
    return L.load(os.path.join(ROOT, "r0", "spec.json"))


@pytest.fixture(scope="session")
def inputs():
    out = {}
    d = os.path.join(ROOT, "r0", "inputs")
    for name in sorted(os.listdir(d)):
        with open(os.path.join(d, name)) as f:
            inp = json.load(f)
        out[inp["name"]] = inp
    return out


@pytest.fixture(scope="session")
def transparent_module(spec, tmp_path_factory):
    import transpile
    path = tmp_path_factory.mktemp("gen") / "generated.py"
    path.write_text(transpile.transpile(spec))
    return str(path)


@pytest.fixture(scope="session")
def tangled_module(transparent_module, tmp_path_factory):
    import make_tangled
    path = tmp_path_factory.mktemp("gen") / "tangled.py"
    path.write_text(make_tangled.tangle(open(transparent_module).read()))
    return str(path)
