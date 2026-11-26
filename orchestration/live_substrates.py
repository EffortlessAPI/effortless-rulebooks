#!/usr/bin/env python3
"""Run an execution substrate on LIVE rows, and compare substrates' answers.

The conformance harness (test-orchestrator.py) grades every substrate on the rows
a rulebook stores. An app that lets a person change rows needs the same
substrates on the rows as they are NOW: to compute its screen with one of them,
and to show that they still agree. This is that, built from the harness's own
parts so there is one of each rule:

  - a substrate is fed the way the harness feeds it: blank tests in
    $ERB_TESTING_DIR/blank-tests, answers out of
    $ERB_TESTING_DIR/<substrate>/test-answers (the generated runner's contract);
  - the OWL substrate is run by execution-substrates/owl/take-test.py's own
    functions (rules in sh:order over a SPARQL store, values read back through
    manifest.json), on individuals written here from the live rows;
  - two values are equal when test-orchestrator.compare_values says so.

Nothing is written into the project: every run uses a temporary directory.

  evaluate  --project-dir DIR --substrate NAME --rows FILE [--out FILE]
  compare   --project-dir DIR --answers NAME=FILE NAME=FILE ... [--out FILE]

--rows FILE:  {"Table": [{"Field": value, ...}, ...]}  stored (raw and
              relationship) fields under the rulebook's own field names.
evaluate out: {"substrate", "elapsedMs", "tables": {"Table": [{every field}]}}
compare out:  {"substrates", "cells", "differing": [{table, key, field,
              datatype, values: {substrate: value}}], "missingRows": [...]}

Substrates: effortless-typescript, effortless-python, effortless-owl.
(effortless-postgres answers come from the database the app keeps; hand them to
`compare` as an answers file.)
"""
from __future__ import annotations

import argparse
import importlib.util
import json
import os
import subprocess
import sys
import tempfile
import time
from pathlib import Path
from urllib.parse import quote

REPO_ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(REPO_ROOT / "orchestration"))
sys.path.insert(0, str(REPO_ROOT / "scripts"))

from shared import to_snake_case  # noqa: E402  (the harness's own naming rule)
import erb_project  # noqa: E402  (the one place a project's rulebook is located)

STORED_TYPES = {"raw", "relationship"}
RUN_TIMEOUT_SECONDS = 180  # a hung substrate is reported, never waited on forever


class LiveSubstrateError(RuntimeError):
    pass


def load_rulebook(project_dir: Path) -> dict:
    path = erb_project.rulebook_path(project_dir.name, project_dir)
    with open(path, encoding="utf-8") as handle:
        return json.load(handle)


def tables_of(rulebook: dict) -> dict:
    return {name: table for name, table in rulebook.items() if isinstance(table, dict) and "schema" in table}


def key_field(table: dict) -> str:
    """The table's key: its first non-nullable field (the ERB convention the
    harness's discover_primary_key applies), else its first field."""
    for field in table["schema"]:
        if field.get("nullable") is False:
            return field["name"]
    return table["schema"][0]["name"]


def stored_fields(table: dict) -> list[str]:
    return [f["name"] for f in table["schema"] if f.get("type", "raw") in STORED_TYPES]


def check_rows(rulebook: dict, rows: dict) -> None:
    known = tables_of(rulebook)
    unknown = sorted(set(rows) - set(known))
    if unknown:
        raise LiveSubstrateError(f"--rows names tables the rulebook does not have: {unknown}")
    missing = sorted(set(known) - set(rows))
    if missing:
        raise LiveSubstrateError(f"--rows is missing tables the rulebook has: {missing}")


def run(cmd: list[str], cwd: Path, env: dict | None = None) -> str:
    try:
        done = subprocess.run(cmd, cwd=cwd, env=env, capture_output=True, text=True, timeout=RUN_TIMEOUT_SECONDS)
    except subprocess.TimeoutExpired:
        raise LiveSubstrateError(f"{' '.join(cmd)} in {cwd} did not finish within {RUN_TIMEOUT_SECONDS}s")
    if done.returncode != 0:
        raise LiveSubstrateError(f"{' '.join(cmd)} in {cwd} exited {done.returncode}:\n{(done.stdout + done.stderr)[-3000:]}")
    return done.stdout


# ----------------------------------------------------------------------------
# Substrates that follow the generated runner's file contract
# ----------------------------------------------------------------------------

def write_blank_tests(rulebook: dict, rows: dict, testing_dir: Path) -> None:
    """The harness's blank tests for these rows: every field under its snake_case
    name, stored fields holding their value, every computed field null."""
    blank_dir = testing_dir / "blank-tests"
    blank_dir.mkdir(parents=True)
    for name, table in tables_of(rulebook).items():
        stored = set(stored_fields(table))
        fields = [f["name"] for f in table["schema"]]
        out = [{to_snake_case(f): (row.get(f) if f in stored else None) for f in fields} for row in rows[name]]
        (blank_dir / f"{to_snake_case(name)}.json").write_text(json.dumps(out, ensure_ascii=False), encoding="utf-8")


def read_answers(rulebook: dict, rows: dict, answers_dir: Path, substrate: str) -> dict:
    result = {}
    for name, table in tables_of(rulebook).items():
        path = answers_dir / f"{to_snake_case(name)}.json"
        if not path.exists():
            if rows[name]:
                raise LiveSubstrateError(f"{substrate} wrote no answers for {name} ({path.name})")
            result[name] = []
            continue
        answered = json.loads(path.read_text(encoding="utf-8"))
        fields = [f["name"] for f in table["schema"]]
        out = []
        for row in answered:
            errors = row.get("_erb_errors")
            if errors:
                raise LiveSubstrateError(f"{substrate} could not compute {name} fields {errors}")
            absent = [f for f in fields if to_snake_case(f) not in row]
            if absent:
                raise LiveSubstrateError(f"{substrate} answered {name} without fields {absent}")
            out.append({f: row[to_snake_case(f)] for f in fields})
        result[name] = out
    return result


def ensure_typescript_built(folder: Path) -> None:
    """The harness's own steps (effortless-typescript/take-test.sh), run only when
    the generated source is newer than what was compiled from it."""
    if not (folder / "erb_sdk.ts").exists():
        raise LiveSubstrateError(f"{folder}/erb_sdk.ts is missing: the project has no rulebook-to-typescript step at /effortless-typescript, or it has not been built")
    built = folder / "dist" / "main.js"
    newest = max(p.stat().st_mtime for p in folder.glob("*.ts"))
    if built.exists() and built.stat().st_mtime >= newest:
        return
    if not (folder / "node_modules").exists():
        run(["npm", "install", "--no-audit", "--no-fund", "--silent"], folder)
    run(["npx", "tsc", "-p", "."], folder)


def evaluate_file_substrate(project_dir: Path, substrate: str, rulebook: dict, rows: dict) -> dict:
    folder = project_dir / substrate
    if substrate == "effortless-typescript":
        ensure_typescript_built(folder)
        cmd = ["node", "dist/main.js"]
    elif substrate == "effortless-python":
        if not (folder / "main.py").exists():
            raise LiveSubstrateError(f"{folder}/main.py is missing: the project has no rulebook-to-python step at /effortless-python, or it has not been built")
        cmd = [sys.executable, "main.py"]
    else:
        raise LiveSubstrateError(f"{substrate} does not follow the generated runner's file contract")
    with tempfile.TemporaryDirectory(prefix="live-substrate-") as tmp:
        testing = Path(tmp)
        write_blank_tests(rulebook, rows, testing)
        env = dict(os.environ, ERB_TESTING_DIR=str(testing), ERB_SUBSTRATE_NAME=substrate, PYTHONDONTWRITEBYTECODE="1")
        run(cmd, folder, env)
        return read_answers(rulebook, rows, testing / substrate / "test-answers", substrate)


# ----------------------------------------------------------------------------
# OWL: the harness's runner over individuals written from the live rows
# ----------------------------------------------------------------------------

XSD = "http://www.w3.org/2001/XMLSchema#"
RDF_TYPE = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type"


def _owl_runner():
    spec = importlib.util.spec_from_file_location("owl_take_test", REPO_ROOT / "execution-substrates" / "owl" / "take-test.py")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def _nt_literal(value, datatype: str) -> str | None:
    if isinstance(value, bool) or datatype == "boolean":
        if isinstance(value, str):
            value = value.strip().lower() in ("true", "t", "1", "yes")
        return f'"{"true" if value else "false"}"^^<{XSD}boolean>'
    if datatype == "integer":
        return f'"{int(value)}"^^<{XSD}integer>'
    if datatype in ("number", "decimal", "float"):
        return f'"{value}"^^<{XSD}decimal>'
    text = str(value)
    escaped = text.replace("\\", "\\\\").replace('"', '\\"').replace("\n", "\\n").replace("\r", "\\r").replace("\t", "\\t")
    return f'"{escaped}"'


def individuals_from_rows(rulebook: dict, rows: dict, manifest: dict) -> str:
    """N-Triples for the live rows in the shape rulebook-to-owl writes them: one
    individual per row, typed by its table's class, carrying erb:key and each
    stored field under the property manifest.json names. A relationship points
    at the individual of the row it names. The IRI is this function's own
    business (the runner finds rows by class and key, never by IRI)."""
    tables = tables_of(rulebook)
    base = manifest["ontology"]["dataNamespace"]
    key_property = manifest["keyProperty"]

    def iri(table_name: str, key) -> str:
        return f"<{base}row/{quote(table_name, safe='')}/{quote(str(key), safe='')}>"

    lines = []
    for name, names in manifest["tables"].items():
        if name not in tables:
            raise LiveSubstrateError(f"manifest.json names table {name!r}, which the rulebook does not have: the OWL files are from another build")
        table = tables[name]
        key_name = key_field(table)
        fields = {f["name"]: f for f in table["schema"]}
        for index, row in enumerate(rows[name]):
            key = row.get(key_name)
            if key is None or str(key).strip() == "":
                raise LiveSubstrateError(f"{name}[{index}] has no value in its key field {key_name}")
            subject = iri(name, key)
            lines.append(f"{subject} <{RDF_TYPE}> <{names['class']}> .")
            lines.append(f"{subject} <{key_property}> {_nt_literal(str(key), 'string')} .")
            for field_name in stored_fields(table):
                value = row.get(field_name)
                if value is None or value == "":
                    continue
                prop = names["fields"].get(field_name)
                if prop is None:
                    raise LiveSubstrateError(f"manifest.json has no property for {name}.{field_name}: the OWL files are from another build")
                field = fields[field_name]
                if field.get("type") == "relationship":
                    target = field.get("RelatedTo")
                    if target not in tables:
                        raise LiveSubstrateError(f"{name}.{field_name} relates to {target!r}, which is not a table")
                    lines.append(f"{subject} <{prop}> {iri(target, value)} .")
                else:
                    lines.append(f"{subject} <{prop}> {_nt_literal(value, field.get('datatype', 'string'))} .")
    return "\n".join(lines) + "\n"


def evaluate_owl(project_dir: Path, rulebook: dict, rows: dict) -> dict:
    src = project_dir / "effortless-owl" / "src"
    for required in ("ontology.owl", "rules.shacl.ttl", "manifest.json"):
        if not (src / required).exists():
            raise LiveSubstrateError(f"{src / required} is missing: the project has no rulebook-to-owl step at /effortless-owl, or it has not been built")
    runner = _owl_runner()
    import pyoxigraph
    from rdflib import Graph

    manifest = runner.load_manifest(src)
    store = pyoxigraph.Store()
    store.load(path=str(src / "ontology.owl"), format=pyoxigraph.RdfFormat.TURTLE)
    store.load(individuals_from_rows(rulebook, rows, manifest).encode("utf-8"), format=pyoxigraph.RdfFormat.N_TRIPLES)
    rules_graph = Graph()
    rules_graph.parse(src / "rules.shacl.ttl", format="turtle")
    runner.run_shacl_reasoning(store, runner.load_sparql_rules(rules_graph))
    graph = Graph()
    graph.parse(data=store.dump(format=pyoxigraph.RdfFormat.N_TRIPLES, from_graph=pyoxigraph.DefaultGraph()), format="nt")

    result = {}
    for name, table in tables_of(rulebook).items():
        fields = [f["name"] for f in table["schema"]]
        if name not in manifest["tables"]:
            # A table the OWL build leaves out (the transpiler-ignored __meta__): nothing is computed for it.
            result[name] = None
            continue
        answered = runner.extract_entity_results(graph, name, table["schema"], rows[name], manifest)
        result[name] = [{f: row.get(runner.camel_to_snake(f)) for f in fields} for row in answered]
    return result


# ----------------------------------------------------------------------------
# evaluate / compare
# ----------------------------------------------------------------------------

def evaluate(project_dir: Path, substrate: str, rows: dict) -> dict:
    rulebook = load_rulebook(project_dir)
    check_rows(rulebook, rows)
    started = time.time()
    if substrate == "effortless-owl":
        tables = evaluate_owl(project_dir, rulebook, rows)
    else:
        tables = evaluate_file_substrate(project_dir, substrate, rulebook, rows)
    return {"substrate": substrate, "elapsedMs": round((time.time() - started) * 1000), "tables": tables}


def _compare_values(project_dir: Path):
    """test-orchestrator.compare_values: the one rule for when two substrates'
    values are the same value. The harness resolves its project at import, so it
    is told which one first."""
    os.environ["ERB_DOMAIN"] = project_dir.name
    spec = importlib.util.spec_from_file_location("test_orchestrator", REPO_ROOT / "orchestration" / "test-orchestrator.py")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module.compare_values


def compare(project_dir: Path, answers: dict) -> dict:
    """answers: {substrate: {"Table": [rows]}}. Every computed cell is compared
    across every pair of substrates that computed its table, in both directions
    (compare_values is an expected-versus-actual test, and no substrate here is
    the expected one)."""
    if len(answers) < 2:
        raise LiveSubstrateError("compare needs the answers of at least two substrates")
    rulebook = load_rulebook(project_dir)
    same = _compare_values(project_dir)
    names = list(answers)
    cells = 0
    differing, missing_rows = [], []
    for table_name, table in tables_of(rulebook).items():
        key_name = key_field(table)
        computed = [f for f in table["schema"] if f.get("type", "raw") not in STORED_TYPES]
        present = [n for n in names if answers[n].get(table_name) is not None]
        if len(present) < 2 or not computed:
            continue
        by_key = {n: {str(row.get(key_name)): row for row in answers[n][table_name]} for n in present}
        for key in by_key[present[0]]:
            absent = [n for n in present if key not in by_key[n]]
            if absent:
                missing_rows.append({"table": table_name, "key": key, "missingFrom": absent})
                continue
            for field in computed:
                cells += 1
                values = {n: by_key[n][key].get(field["name"]) for n in present}
                datatype = field.get("datatype")
                agree = all(
                    same(values[a], values[b], datatype) and same(values[b], values[a], datatype)
                    for i, a in enumerate(present) for b in present[i + 1:]
                )
                if not agree:
                    differing.append({"table": table_name, "key": key, "field": field["name"], "datatype": datatype, "values": values})
        for n in present[1:]:
            for key in by_key[n]:
                if key not in by_key[present[0]]:
                    missing_rows.append({"table": table_name, "key": key, "missingFrom": [present[0]]})
    return {"substrates": names, "cells": cells, "differing": differing, "missingRows": missing_rows}


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    sub = parser.add_subparsers(dest="command", required=True)
    ev = sub.add_parser("evaluate")
    ev.add_argument("--project-dir", required=True, type=Path)
    ev.add_argument("--substrate", required=True)
    ev.add_argument("--rows", required=True, type=Path)
    ev.add_argument("--out", type=Path)
    cp = sub.add_parser("compare")
    cp.add_argument("--project-dir", required=True, type=Path)
    cp.add_argument("--answers", required=True, nargs="+", metavar="NAME=FILE")
    cp.add_argument("--out", type=Path)
    args = parser.parse_args()

    project_dir = args.project_dir.resolve(strict=True)
    try:
        if args.command == "evaluate":
            rows = json.loads(args.rows.read_text(encoding="utf-8"))
            result = evaluate(project_dir, args.substrate, rows)
        else:
            answers = {}
            for item in args.answers:
                name, _, file = item.partition("=")
                if not file:
                    raise LiveSubstrateError(f"--answers takes NAME=FILE, got {item!r}")
                loaded = json.loads(Path(file).read_text(encoding="utf-8"))
                answers[name] = loaded["tables"] if "tables" in loaded else loaded
            result = compare(project_dir, answers)
    except LiveSubstrateError as error:
        print(f"ERROR: {error}", file=sys.stderr)
        raise SystemExit(1)
    text = json.dumps(result, ensure_ascii=False)
    if args.out:
        args.out.write_text(text, encoding="utf-8")
    else:
        print(text)


if __name__ == "__main__":
    main()
