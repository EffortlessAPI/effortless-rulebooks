---
name: rescore-pipeline
description: "Rescore pipeline for re-scoring existing JDs from disk. Use when implementing or modifying the rescore CLI subcommand, Rescorer class, or JD file parsing back into JobListing objects."
---

# Rescore Pipeline — Re-Score Existing JDs From Disk

## When This Skill Applies

When implementing the `rescore` CLI subcommand and `Rescorer` class, or when
modifying how JD files are parsed back into `JobListing` objects.

---

## Purpose

Re-score existing JD files against current ChromaDB collections without running
a browser search. Use after re-indexing archetypes, adding negative signals, or
tuning weights. Completes in seconds instead of 10+ minutes.

---

## JD File Format (Input)

Files live in `output/jds/` as `{rank:03d}_{company_slug}_{title_slug}.md`. For full format
specification, parsing rules, and edge cases, see [jd-file-format.md](references/jd-file-format.md).

---

## Data Flow

```
output/jds/*.md
    → parse metadata + body → list[JobListing]
    → _ensure_indexed() (auto-index if collections empty)
    → for each listing:
        → scorer.score(full_text)           → ScoreResult (fit, archetype, history, negative)
        → comp_parser.parse(full_text)      → comp fields
        → compute_comp_score(comp_max)      → comp_score
    → ranker.rank(scored, embeddings)       → ranked list + summary
    → export: results.md, results.csv, jd files (with new scores/ranks)
```

### Decision Skipping

- Same as `search`: check `DecisionRecorder.get_decision(external_id)` before scoring
- Skip decided listings unless `--force-rescore` is passed
- Skipped count appears in run summary

---

## CLI Interface

```bash
# Re-score all undecided JDs
python -m jobsearch_rag rescore

# Re-score everything including decided
python -m jobsearch_rag rescore --force-rescore
```

### Exit Conditions

| Condition | Behavior |
|---|---|
| `output/jds/` missing | Print actionable message + exit |
| `output/jds/` empty | Print actionable message + exit |
| No valid JD files parsed | Print warning + exit |
| Collections empty | Auto-index before scoring (same as search) |

---

## Module Location

`pipeline/rescorer.py` — standalone module, not bolted onto `runner.py`.

Shares with runner:
- `Scorer`, `Ranker`, `Embedder`, `VectorStore`, `DecisionRecorder`
- `parse_compensation()`, `compute_comp_score()`
- All exporters (`MarkdownExporter`, `CSVExporter`, `JDFileExporter`)

Does NOT share:
- Browser session management
- Adapter layer
- Rate limiting / throttling

---

## `handle_rescore` in `cli.py`

Pattern mirrors `handle_search`:

1. `load_settings()`
2. Configure file logging
3. Create `Rescorer(settings)`
4. `asyncio.run(rescorer.run(force_rescore=args.force_rescore))`
5. Print summary (same format as search)
6. Run all three exporters
