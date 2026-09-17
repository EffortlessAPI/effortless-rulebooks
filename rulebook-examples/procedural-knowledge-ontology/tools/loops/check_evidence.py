#!/usr/bin/env python3
"""Check that every evidence target a theme names exists in a rulebook (default: its probe copy).

Usage: tools/loops/check_evidence.py <theme-name> [rulebook.json]
The default rulebook is the scratch copy the probe built: $TMPDIR/pko-probe-<theme>/effortless-rulebook/...
"""
import importlib.util
import json
import sys
import tempfile
from pathlib import Path

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE.parent))
import article_claims  # noqa: E402
import article_evidence  # noqa: E402

theme = sys.argv[1]
rb_path = Path(sys.argv[2]) if len(sys.argv) > 2 else (
    Path(tempfile.gettempdir()) / f"pko-probe-{theme}" / "effortless-rulebook" / "procedural-knowledge-ontology-rulebook.json")
rb = json.loads(rb_path.read_text())
spec = importlib.util.spec_from_file_location("ev", HERE / f"evidence_{theme}.py")
mod = importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)

tables = {t for t, v in rb.items() if isinstance(v, dict) and "schema" in v}
targets = {
    "Field": {f"{t}.{f['name']}" for t in tables for f in rb[t]["schema"]},
    "Table": tables,
    "RoleQuestion": {r["RoleQuestionId"] for r in rb["RoleQuestions"]["data"]},
    "OntologyProfile": {r["OntologyProfileId"] for r in rb["OntologyProfiles"]["data"]},
    "KnowledgeMethod": {m[0] for m in article_evidence.METHODS} | {r["KnowledgeMethodId"] for r in rb.get("KnowledgeMethods", {}).get("data", [])},
    "Procedure": {r["ProcedureId"] for r in rb["Procedures"]["data"]},
}
claims = {c[0]: c for c in article_claims.CLAIMS}
themes_mod = importlib.util.spec_from_file_location("themes", HERE / "themes.py")
tm = importlib.util.module_from_spec(themes_mod)
themes_mod.loader.exec_module(tm)
mine = {c[0] for c in article_claims.CLAIMS if tm.LOOP_OF[tm.theme_of(c)][1] == theme}
problems = []
for cid, items in mod.EVIDENCE.items():
    if cid not in claims:
        problems.append(f"{cid}: not a claim")
    elif cid not in mine:
        problems.append(f"{cid}: belongs to another theme")
    for kind, target, why in items:
        if kind not in targets:
            problems.append(f"{cid}: unknown kind {kind}")
        elif target not in targets[kind]:
            problems.append(f"{cid}: {kind} {target} does not exist")
        if not why.strip():
            problems.append(f"{cid}: empty justification")
uncovered = sorted(mine - set(mod.EVIDENCE))
print(f"{theme}: {len(mod.EVIDENCE)} claims with evidence, {len(uncovered)} without, {len(problems)} problems")
for x in problems:
    print("  ", x)
if uncovered:
    print("  no evidence:", ", ".join(uncovered))
sys.exit(1 if problems else 0)
