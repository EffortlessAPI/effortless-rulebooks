"""NEGATIVE CONTROL 2: the tangled substrate.

Takes the transparent module and routes every predicate read inside a rule
through one dynamic dispatcher, `_dispatch(name, S)`, which looks the
target function up by name.  The computation is unchanged and the answer
key still passes.  But the dependency of each rule on the relations it
reads is now a string inside a call to a helper, not a call edge: condition
2 (dependency preservation) fails, and condition 6 fails because the helper
resolves its callee by name at run time (an evaluator in miniature).  This is the small-scale shape of the
"dependency flooding" obstacle in paper section 23: constructor images
exist, yet the structure between them has been hidden.

Usage:  python make_tangled.py transparent_module.py out.py
"""
import re
import sys

HELPER = '''

def _dispatch(name, S):
    """Universal lookup: every rule's reads go through here."""
    return globals()["c_pred_" + name](S)
'''


def tangle(src: str) -> str:
    out = []
    in_rule = False
    for line in src.splitlines():
        if line.startswith("def "):
            in_rule = line.startswith("def c_rule_")
        if in_rule and not line.startswith("def "):
            line = re.sub(r"c_pred_([A-Za-z0-9_]+)\(S\)", r'_dispatch("\1", S)', line)
        out.append(line)
    return "\n".join(out) + HELPER


if __name__ == "__main__":
    with open(sys.argv[1]) as f:
        src = f.read()
    with open(sys.argv[2], "w") as f:
        f.write(tangle(src))
    print(f"wrote {sys.argv[2]}")
