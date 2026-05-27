#!/usr/bin/env python3
"""
Migrate the platform rulebook's `_meta` essay keys into first-class tables.

The platform rulebook
(`effortless-platform/effortless-rulebook/effortless-rulebook.json`)
has a structurally different `_meta` from the demo rulebooks. Its keys are
narrative essays (`_ProjectGoal`, `_ArchitecturalHighlight`, ...). The user
wants each of these promoted to its own first-class table — no leading
underscores, no `__meta__` key-value bucket — so they show up as ordinary
tables in queries and the admin portal.

Mapping (each becomes a single-row table with `{Name, Description}` schema):

    _CMCC_Summary           -> CMCCSummary
    _ProjectGoal            -> ProjectGoal
    _ArchitecturalHighlight -> ArchitecturalHighlight
    _WriteThroughInvariant  -> WriteThroughInvariant
    _PortalCliParity        -> PortalCliParity
    _BootstrapStory         -> BootstrapStory
    _DeveloperJourney       -> DeveloperJourney
    _ResilienceClaim        -> ResilienceClaim

Idempotent: a table that already exists is left untouched.
The `_meta` object is removed when migration succeeds.
"""

from __future__ import annotations

import json
import os
import sys

REPO_ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
PLATFORM_RULEBOOK = os.path.join(
    REPO_ROOT,
    "effortless-platform",
    "effortless-rulebook",
    "effortless-rulebook.json",
)

TABLE_FOR_META_KEY: dict[str, dict[str, str]] = {
    "_CMCC_Summary": {
        "table": "CMCCSummary",
        "description": "One-line CMCC framing for the platform rulebook itself: what it is, what it generates, and what conformance it claims.",
    },
    "_ProjectGoal": {
        "table": "ProjectGoal",
        "description": "The primary goal of the platform: the load-bearing claim the project is built to demonstrate.",
    },
    "_ArchitecturalHighlight": {
        "table": "ArchitecturalHighlight",
        "description": "The architectural pattern that makes the platform work: input spokes write to the rulebook, output spokes read from it, conformance binds them.",
    },
    "_WriteThroughInvariant": {
        "table": "WriteThroughInvariant",
        "description": "The portal-vs-rulebook write-through invariant: Postgres is the live editor, JSON is the durable SSoT, every save writes to both.",
    },
    "_PortalCliParity": {
        "table": "PortalCliParity",
        "description": "The portal and the `./start.sh --cli` interface are peer interfaces to the same effortless.json pipeline; portal behaviour cannot drift from CLI behaviour.",
    },
    "_BootstrapStory": {
        "table": "BootstrapStory",
        "description": "The cold-start story: from `git clone` to a running portal with a default user landing on Home.",
    },
    "_DeveloperJourney": {
        "table": "DeveloperJourney",
        "description": "The intended developer path through the portal: Home → project → Rulebook → Substrates → Builds → Tests → Input Spokes → Users.",
    },
    "_ResilienceClaim": {
        "table": "ResilienceClaim",
        "description": "The resilience claim: dropping the Postgres editor at any time is safe, because `./start.sh` rebuilds it from JSON.",
    },
}

NARRATIVE_SCHEMA = [
    {
        "name": "Name",
        "datatype": "string",
        "type": "raw",
        "nullable": False,
        "Description": "Identifier for this narrative entry (single-row tables use 'primary').",
    },
    {
        "name": "Description",
        "datatype": "string",
        "type": "raw",
        "nullable": False,
        "Description": "The full narrative content for this concept.",
    },
]


def main() -> int:
    if not os.path.exists(PLATFORM_RULEBOOK):
        print(f"Platform rulebook not found at {PLATFORM_RULEBOOK}", file=sys.stderr)
        return 1

    with open(PLATFORM_RULEBOOK, "r", encoding="utf-8") as fp:
        data = json.load(fp)

    meta = data.pop("_meta", None)
    if not meta:
        print("No _meta object on platform rulebook; nothing to migrate.")
        return 0

    created: list[str] = []
    skipped_existing: list[str] = []
    unmapped: list[str] = []

    for meta_key, value in meta.items():
        mapping = TABLE_FOR_META_KEY.get(meta_key)
        if mapping is None:
            unmapped.append(meta_key)
            continue
        table_name = mapping["table"]
        if table_name in data:
            skipped_existing.append(table_name)
            continue
        if not isinstance(value, str):
            print(
                f"  WARN  {meta_key}: expected string, got {type(value).__name__}; serializing as JSON.",
                file=sys.stderr,
            )
            value = json.dumps(value, ensure_ascii=False)
        data[table_name] = {
            "Description": mapping["description"],
            "important": False,
            "schema": NARRATIVE_SCHEMA,
            "data": [{"Name": "primary", "Description": value}],
        }
        created.append(table_name)

    if unmapped:
        print(
            f"  ERROR  Unmapped meta keys (refusing to drop _meta): {unmapped}",
            file=sys.stderr,
        )
        return 2

    with open(PLATFORM_RULEBOOK, "w", encoding="utf-8") as fp:
        json.dump(data, fp, indent=2, ensure_ascii=False)
        fp.write("\n")

    print(f"Migrated platform rulebook _meta.")
    print(f"  Created tables ({len(created)}): {created}")
    if skipped_existing:
        print(f"  Skipped (already existed): {skipped_existing}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
