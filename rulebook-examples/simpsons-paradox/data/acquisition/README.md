# Corpus acquisition queue (out-of-channel)

This directory tracks **standalone data acquisition** for the 142 expansion-wave-1
catalog candidates added in loop-67. It is intentionally **outside** the Leopold
loops 68–72 encode path so bulk downloading does not collide with rulebook edits.

## Files

| File | Purpose |
|------|---------|
| `expansion-wave-1-manifest.json` | Master queue: per-study acquisition status, URLs tried, files saved |
| `../raw/<study-id>/` | Downloaded artifacts + `provenance.md` stub + `acquisition.json` sidecar |

## Run the acquisition loop

```bash
python3 scripts/acquisition-loop-expansion-wave-1.py
python3 scripts/acquisition-loop-expansion-wave-1.py --limit 10   # smoke test
python3 scripts/acquisition-loop-expansion-wave-1.py --retry-failed
python3 scripts/acquisition-loop-expansion-wave-1.py --id blau-kahn-2017
```

## Status vocabulary

| Status | Meaning |
|--------|---------|
| `pending` | Not yet attempted |
| `downloaded` | Open PDF or data file retrieved |
| `portal_manual` | Data portal landing page saved; manual extract required (IPUMS, BLS, …) |
| `landing_saved` | HTML saved but not a direct dataset |
| `metadata_only` | DOI/OpenAlex resolved; no open PDF |
| `manual_only` | No public URL — encode from published tables in paper |
| `blocked` | URLs exist but fetch failed (paywall/login) |

## Merge into rulebook (after loops 68–72)

**Merged 2026-07-07** via `python3 scripts/merge-acquisition-manifest.py`.

Mapping used:

- `downloaded` → `downloaded`
- `portal_manual`, `landing_saved`, `metadata_only`, `manual_only` → `manual_only`
- `blocked` → `blocked`

Re-merge after a fresh acquisition run:

```bash
python3 scripts/acquisition-loop-expansion-wave-1.py --retry-failed
python3 scripts/merge-acquisition-manifest.py
effortless build
```
