#!/usr/bin/env bash
# =============================================================================
# clone-skills.sh — DERIVED ARTIFACT — DO NOT EDIT BY HAND
# =============================================================================
# Source of truth: ClaudeSkills table in
#   effortless-platform/effortless-rulebook/effortless-rulebook.json
# Registered as: "clone-skills" transpiler in effortless-platform/effortless.json
#
# What this does:
#   Reads the ClaudeSkills table from the platform rulebook and pulls a live
#   copy of each skill's SKILL.md from the effortless-claude GitHub repo into
#   docs/skills/<name>/SKILL.md. Each mirrored file carries a DO-NOT-EDIT
#   header. Re-run to update.
#
# Usage:
#   ./docs/skills/clone-skills.sh                      # from repo root
#   cd effortless-platform && effortless clone-skills  # via build pipeline
# =============================================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
RULEBOOK="$REPO_ROOT/effortless-platform/effortless-rulebook/effortless-rulebook.json"
SKILLS_DIR="$REPO_ROOT/docs/skills"

if [[ ! -f "$RULEBOOK" ]]; then
  echo "ERROR: Rulebook not found at expected path: $RULEBOOK" >&2
  exit 1
fi

export RULEBOOK SKILLS_DIR

echo "==> Cloning skills from effortless-claude into docs/skills/ ..."

python3 - "$RULEBOOK" "$SKILLS_DIR" << 'PYEOF'
import json, os, sys, urllib.request, urllib.error

rulebook_path, skills_dir = sys.argv[1], sys.argv[2]

rb = json.load(open(rulebook_path))
skills_data = rb.get('ClaudeSkills', {}).get('data', [])

if not skills_data:
    print("ERROR: ClaudeSkills table is empty or missing in rulebook.", file=sys.stderr)
    sys.exit(1)

ok = fail = 0
for skill in skills_data:
    name = skill['Name']
    clone_url = skill['CloneUrl']
    local_path = os.path.join(skills_dir, name, 'SKILL.md')

    os.makedirs(os.path.dirname(local_path), exist_ok=True)

    try:
        with urllib.request.urlopen(clone_url, timeout=15) as resp:
            raw = resp.read().decode('utf-8')
    except urllib.error.HTTPError as e:
        print(f"  SKIP {name}: HTTP {e.code} — {clone_url}")
        fail += 1
        continue
    except Exception as e:
        print(f"  SKIP {name}: {e}")
        fail += 1
        continue

    header = (
        "<!-- DERIVED ARTIFACT — DO NOT EDIT BY HAND -->\n"
        f"<!-- Source: {clone_url} -->\n"
        "<!-- Mirrored by: docs/skills/clone-skills.sh -->\n"
        "<!-- Update: cd effortless-platform && effortless clone-skills -->\n\n"
    )

    with open(local_path, 'w') as f:
        f.write(header + raw)

    print(f"  OK  {name}")
    ok += 1

print(f"\n==> {ok} skills cloned, {fail} skipped.")
if fail > 0:
    sys.exit(1)
PYEOF
