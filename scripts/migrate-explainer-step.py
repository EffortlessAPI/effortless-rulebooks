#!/usr/bin/env python3
"""Put every governed project on the one explainer tool, rulebook-to-explainer-dag.

2026-11-22: rulebook-to-react-explainer-dag is retired (it is one call to
rulebook-to-explainer-dag with react=true), and every project carries the explainer,
which is the repo's most particular differentiator. This rewrites each project's
`effortless.json`, and nothing else:

  * a step that names rulebook-to-react-explainer-dag (by name, or by a pinned cpln URL)
    becomes `rulebook-to-explainer-dag ... -p react=true`, same path, same input;
  * a project with no explainer step gets one that writes the static explainer to
    /explainer (explainer/rulebook-explainer-dag/pages/index.html, a real page that
    opens from the file system or any static server);
  * a project already on the new tool is left alone.

The version a step last used (LastVersionUsed / LastUrl) is the CLI's own history, not
a pin; it is dropped on a rewritten step so the next build records the new tool's.

Usage:
    python3 scripts/migrate-explainer-step.py                 # report what would change
    python3 scripts/migrate-explainer-step.py --write         # change effortless.json files
    python3 scripts/migrate-explainer-step.py --write --only customer-crm,star-trek
    python3 scripts/migrate-explainer-step.py --write --generate   # and run each changed step once, as a build would

--generate runs the step's own command line (`effortless rulebook-to-explainer-dag ...`) in the step's
folder, so the files exist without a full project build (no database, no editor). For a React step it first
removes the files the retired tool wrote (lib/, components/, pages/, embedded-*.ts under explainer-dag/);
the module that replaces them is `explainer-dag/shim.tsx` over `explainer-dag/vendor/`.
"""
import argparse
import glob
import json
import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
STEP_NAME = "rulebooktoexplainerdag"
NEW_TOOL = "rulebook-to-explainer-dag"
OLD_TOOL = "rulebook-to-react-explainer-dag"


def is_old(step):
    cmd = step.get("CommandLine", "")
    return cmd.split(" ", 1)[0] == OLD_TOOL or (
        cmd.startswith("http") and "react-exp" in cmd.split(" ", 1)[0]
    ) or step.get("Name") == "rulebooktoreactexplainerdag"


def is_new(step):
    return step.get("CommandLine", "").split(" ", 1)[0] == NEW_TOOL


def args_of(cmd):
    parts = cmd.split(" ", 1)
    return parts[1] if len(parts) > 1 else ""


def hub_for(project_dir):
    """The project's rulebook, as written in effortless.json's own steps (the folder is /effortless-rulebook)."""
    folder = os.path.join(project_dir, "effortless-rulebook")
    cands = sorted(glob.glob(os.path.join(folder, "*-rulebook.json")) + glob.glob(os.path.join(folder, "effortless-rulebook.json")))
    cands = [c for c in cands if not c.endswith((".derived-schema.json", ".derived-data.json"))]
    return os.path.basename(cands[0]) if cands else None


def migrate(project_dir, write):
    path = os.path.join(project_dir, "effortless.json")
    if not os.path.exists(path):
        return None
    raw = open(path, encoding="utf-8").read()
    doc = json.loads(raw)
    steps = doc.get("ProjectTranspilers")
    if not steps:
        return ("skip", "no ProjectTranspilers")
    if any(is_new(s) and not s.get("IsDisabled") for s in steps) and not any(is_old(s) for s in steps):
        return ("ok", "already on rulebook-to-explainer-dag")

    changed = None
    for s in steps:
        if is_old(s):
            rest = args_of(s["CommandLine"])
            if "react=true" not in rest:
                rest = (rest + " -p react=true").strip()
            s["Name"] = STEP_NAME
            s["CommandLine"] = f"{NEW_TOOL} {rest}"
            s.pop("LastVersionUsed", None)
            s.pop("LastUrl", None)
            s.pop("_HeadPointerNote", None)
            s.pop("Description", None)
            changed = ("react", s["RelativePath"])
    if changed is None:
        hub = hub_for(project_dir)
        if not hub:
            return ("skip", "no rulebook in effortless-rulebook/")
        step = {
            "IsSSoTTranspiler": False,
            "Name": STEP_NAME,
            "RelativePath": "/explainer",
            "CommandLine": f"{NEW_TOOL} -i ../effortless-rulebook/{hub}",
            "IsDisabled": False,
        }
        # After the rulebook's docs step when there is one, else last.
        at = next((i + 1 for i, s in enumerate(steps) if s.get("Name") == "rulebooktorulespeak"), len(steps))
        steps.insert(at, step)
        changed = ("static", "/explainer")

    if write:
        out = json.dumps(doc, indent=2, ensure_ascii=False) + ("\n" if raw.endswith("\n") else "")
        open(path, "w", encoding="utf-8").write(out)
    return changed


STALE = ["lib", "components", "pages", "embedded-rulebook.ts", "embedded-rulespeak.ts"]


def generate(project_dir, step_dir, kind):
    """Run the step as a build would, in its own folder. Raises if the CLI fails."""
    target = os.path.join(project_dir, step_dir.lstrip("/"))
    if kind == "react":
        for name in STALE:
            p = os.path.join(target, "explainer-dag", name)
            if os.path.isdir(p):
                shutil.rmtree(p)
            elif os.path.exists(p):
                os.remove(p)
    os.makedirs(target, exist_ok=True)
    doc = json.load(open(os.path.join(project_dir, "effortless.json"), encoding="utf-8"))
    step = next(s for s in doc["ProjectTranspilers"] if s.get("Name") == STEP_NAME)
    cmd = ["effortless"] + step["CommandLine"].split(" ")
    r = subprocess.run(cmd, cwd=target, capture_output=True, text=True)
    if r.returncode != 0:
        raise RuntimeError(f"{' '.join(cmd)} failed in {target} ({r.returncode}):\n{r.stdout[-800:]}\n{r.stderr[-800:]}")
    # A generated folder with a file too big for git (GitHub refuses 100 MB; 5 MB is where a
    # diff stops being readable) is not committed: the build writes it, as for any generated output.
    big = max((os.path.getsize(os.path.join(r_, f)) for r_, _, fs in os.walk(target) for f in fs), default=0)
    if big > 5 * 1024 * 1024 and kind == "static":
        ignore = os.path.join(project_dir, ".gitignore")
        line = step_dir.rstrip("/") + "/"
        have = open(ignore, encoding="utf-8").read() if os.path.exists(ignore) else ""
        if line not in have.split("\n"):
            open(ignore, "a", encoding="utf-8").write(("" if have.endswith("\n") or not have else "\n") + line + "\n")
        print(f"    largest file {big / 1048576:.0f} MB: {line} is generated, not committed (added to .gitignore)", flush=True)
    marker = "explainer-dag/shim.tsx" if kind == "react" else "rulebook-explainer-dag/pages/index.html"
    if not os.path.exists(os.path.join(target, marker)):
        raise RuntimeError(f"the step ran but {marker} is not in {target}:\n{r.stdout[-800:]}")


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--write", action="store_true")
    ap.add_argument("--generate", action="store_true", help="also run each changed step once (needs --write)")
    ap.add_argument("--only", help="comma-separated project slugs")
    a = ap.parse_args()
    only = set(a.only.split(",")) if a.only else None
    dirs = sorted(glob.glob(os.path.join(ROOT, "rulebook-examples", "*")) + glob.glob(os.path.join(ROOT, "toy-rulebooks", "*")))
    for d in dirs:
        slug = os.path.basename(d)
        if only and slug not in only:
            continue
        r = migrate(d, a.write)
        if r is None:
            continue
        print(f"{slug:48s} {r[0]:7s} {r[1]}", flush=True)
        if a.generate and r[0] in ("react", "static"):
            if not a.write:
                sys.exit("--generate needs --write")
            generate(d, r[1], r[0])
    if not a.write:
        print("\n(report only: pass --write to change effortless.json)")


if __name__ == "__main__":
    sys.exit(main())
