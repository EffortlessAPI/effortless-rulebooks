#!/usr/bin/env python3
"""
airtable substrate — the rulebook source-of-truth sync step.

Behaves differently from every other substrate: instead of generating code
and taking the conformance test, it refreshes the rulebook that all other
substrates consume. Concretely:

  1. Shows the current base (name + ID)
  2. Lists known bases (from orchestration/bases.json)
  3. Prompts for a selection — Enter keeps the current base, a number switches
  4. Runs rulebook-cache.py sync to pull the latest schema/data from Airtable

In CI / non-interactive mode it silently keeps the current base and syncs.

Environment:
  ERB_DOCKER_MODE=true  — treated as CI (non-interactive, offline sync)
  --ci                  — CLI flag, same effect as ERB_DOCKER_MODE
  --non-interactive     — keep current base, no prompt
"""

import os
import subprocess
import sys
from pathlib import Path

SCRIPT_DIR = Path(__file__).parent.resolve()
PROJECT_ROOT = SCRIPT_DIR.parent.parent
ORCHESTRATION_DIR = PROJECT_ROOT / "orchestration"
BASE_MANAGER = ORCHESTRATION_DIR / "base-manager.py"
RULEBOOK_CACHE = ORCHESTRATION_DIR / "rulebook-cache.py"

sys.path.insert(0, str(ORCHESTRATION_DIR))
import importlib.util

spec = importlib.util.spec_from_file_location("base_manager", BASE_MANAGER)
bm = importlib.util.module_from_spec(spec)
spec.loader.exec_module(bm)


CYAN = "\033[0;36m"
GREEN = "\033[0;32m"
YELLOW = "\033[1;33m"
RED = "\033[0;31m"
DIM = "\033[2m"
BOLD = "\033[1m"
WHITE = "\033[1;37m"
NC = "\033[0m"


def is_non_interactive() -> bool:
    if "--non-interactive" in sys.argv or "--ci" in sys.argv:
        return True
    if os.environ.get("ERB_DOCKER_MODE") == "true":
        return True
    if not sys.stdin.isatty():
        return True
    return False


def print_header():
    print()
    print(f"{BOLD}{CYAN}╔════════════════════════════════════════════════════════════╗{NC}")
    print(f"{BOLD}{CYAN}║{NC}            {BOLD}{WHITE}AIRTABLE — RULEBOOK SYNC{NC}                        {BOLD}{CYAN}║{NC}")
    print(f"{BOLD}{CYAN}╚════════════════════════════════════════════════════════════╝{NC}")
    print()


def show_bases(current_base_id: str, bases: list) -> None:
    print(f"  {DIM}Known bases:{NC}")
    print()
    for i, base in enumerate(bases, 1):
        marker = f" {GREEN}(active){NC}" if base["id"] == current_base_id else ""
        color = GREEN if base["id"] == current_base_id else CYAN
        print(f"  {color}[{i}]{NC} {base['name']}{marker}")
        print(f"      {DIM}{base['id']}{NC}")
    print()


SKIP_SYNC = object()  # sentinel returned when the user opts out of the pull


def prompt_for_base(current_base_id: str, bases: list):
    """Return base ID to sync, or SKIP_SYNC to bypass the pull entirely.

    - A number selects that base and pulls from it
    - Enter pulls from the current base
    - S skips the pull entirely (no switch, no sync)
    - N adds a new base by ID and pulls from it
    """
    count = len(bases)
    prompt = (
        f"  Select base [1-{count}] to pull, "
        f"{DIM}Enter=pull current, S=skip pull, N=add new{NC}: "
    )
    try:
        choice = input(prompt).strip()
    except EOFError:
        return SKIP_SYNC

    if choice == "":
        return current_base_id

    if choice.lower() in ("s", "q"):
        return SKIP_SYNC

    if choice.lower() == "n":
        try:
            new_id = input("  Enter new Airtable base ID (e.g., appXXXXX): ").strip()
        except EOFError:
            return SKIP_SYNC
        if not new_id:
            return SKIP_SYNC
        if not new_id.startswith("app"):
            print(f"{RED}  Invalid Base ID format (must start with 'app').{NC}")
            return SKIP_SYNC
        try:
            name = bm.fetch_base_name_or_fail(new_id)
            bm.add_or_update_base(new_id, name)
            return new_id
        except Exception as e:
            print(f"{RED}  Failed to add base: {e}{NC}")
            return SKIP_SYNC

    if choice.isdigit():
        idx = int(choice) - 1
        if 0 <= idx < count:
            return bases[idx]["id"]
        print(f"{RED}  Invalid selection: {choice}{NC}")
        return SKIP_SYNC

    print(f"{RED}  Invalid input: {choice}{NC}")
    return SKIP_SYNC


def select_and_sync(target_base_id: str, current_base_id: str) -> int:
    """Switch base (if changed) and run rulebook-cache.py sync."""
    if target_base_id != current_base_id:
        print()
        print(f"{YELLOW}  Switching base...{NC}")
        try:
            bm.select_base(target_base_id)
        except Exception as e:
            print(f"{RED}  Failed to switch base: {e}{NC}")
            return 1

    print()
    print(f"{YELLOW}  Syncing rulebook from Airtable...{NC}")
    print()

    cmd = [sys.executable, str(RULEBOOK_CACHE), "sync"]
    if is_non_interactive():
        cmd += ["--offline", "--non-interactive"]

    result = subprocess.run(cmd, cwd=str(PROJECT_ROOT))
    return result.returncode


def main() -> int:
    if "--clean" in sys.argv:
        # Nothing to clean for the airtable substrate — the rulebook is the
        # shared source of truth and is managed by rulebook-cache, not us.
        return 0

    print_header()

    # Make sure bases.json is populated with the current base
    try:
        bm.sync_bases()
    except Exception:
        # If Airtable is unreachable, keep going with whatever's in bases.json
        pass

    config = bm.load_ssotme_config()
    current_base_id = bm.get_setting(config, "baseId") or ""
    project_name = config.get("Name", "Unknown")
    bases = bm.get_bases_list(config)

    print(f"  Project: {WHITE}{project_name}{NC}")
    print(f"  Base ID: {WHITE}{current_base_id}{NC}")
    print()

    if is_non_interactive():
        print(f"  {DIM}Non-interactive mode — keeping current base and syncing.{NC}")
        return select_and_sync(current_base_id, current_base_id)

    if not bases:
        print(f"  {DIM}No known bases — syncing current base only.{NC}")
        return select_and_sync(current_base_id, current_base_id)

    show_bases(current_base_id, bases)
    target = prompt_for_base(current_base_id, bases)
    if target is SKIP_SYNC:
        print()
        print(f"  {DIM}Skipped — rulebook left unchanged.{NC}")
        return 0
    return select_and_sync(target, current_base_id)


if __name__ == "__main__":
    sys.exit(main())
