---
name: scoring-model
description: "Scoring model covering ChromaDB collections, fusion formula, and embedding synthesis. Use when implementing or modifying the scoring pipeline: indexer embedding synthesis, scorer collection queries, ranker fusion formula, or export score breakdowns."
---

# Scoring Model — Collections, Formula & Embedding Synthesis

## When This Skill Applies

When implementing or modifying the scoring pipeline: indexer embedding synthesis,
scorer collection queries, ranker fusion formula, or export score breakdowns.

---

## The Five ChromaDB Collections

| Collection | Documents | Built By | Score |
|---|---|---|---|
| `resume` | Resume chunked by `##` section | `index_resume()` | `fit_score` |
| `role_archetypes` | One doc per archetype (synthesized) | `index_archetypes()` | `archetype_score` |
| `global_positive_signals` | One doc per rubric dimension with `signals_positive` | `index_global_positive_signals()` | `culture_score` |
| `negative_signals` | One doc per rubric dimension + archetype negatives | `index_negative_signals()` | `negative_score` |
| `decisions` | Past JDs labeled yes/no/maybe | `decide` / `review` commands | `history_score` |

### Two Orthogonal Scoring Axes

- **Right kind of role** — `role_archetypes` (archetype_score) + `resume` (fit_score)
- **Right kind of environment** — `global_positive_signals` (culture_score) + `negative_signals` (negative_score)

---

## Embedding Synthesis

Archetype, positive signal, and negative signal embeddings are synthesized from
TOML fields, not used raw. See [embedding-synthesis.md](references/embedding-synthesis.md)
for code patterns, the what-gets-embedded decision table, and non-semantic score details.

### Global Positive Signal Synthesis

One document per rubric dimension that has `signals_positive`:

```python
# Pseudocode — see indexer.py for actual implementation
for dim in rubric["dimensions"]:
    if signals := dim.get("signals_positive"):
        text = f"{dim_name}: " + ", ".join(signals)
        # → one ChromaDB document with metadata {"source": dim_name}
```

Key rules:
- Only `signals_positive` arrays reach the embedding model
- `description`, `minimum_target`, `weight_*` fields are **never** embedded
- Dimensions without `signals_positive` produce no document (no error)

---

## Score Formula

```python
final_score = (
    archetype_weight  * archetype_score  +   # 0.5 default
    fit_weight        * fit_score        +   # 0.3 default
    culture_weight    * culture_score    +   # 0.2 default
    history_weight    * history_score    +   # 0.2 default
    comp_weight       * comp_score       -   # 0.15 default
    negative_weight   * negative_score       # 0.4 default
) * (0.0 if disqualified else 1.0)
```

### Rules

- Weights are read from `settings.toml`, never hardcoded
- Weights do NOT need to sum to 1.0 — this is a weighted composite, not a probability
- If `final_score <= 0.0`, floor it at `0.0` (clean discard)
- A disqualified role always scores `0.0` regardless of component values
- Roles below `min_score_threshold` (default 0.45) are excluded from output
- `negative_score` has **inverted semantics**: high similarity = bad outcome
- `culture_score` has **positive semantics**: high similarity = good outcome

---

## Score Explanation Format

```
Archetype: 0.81 · Fit: 0.74 · Culture: 0.65 · History: 0.62 · Comp: 0.70 · Negative: 0.12 · Not disqualified
```

All six components shown. Separator is `·` (middle dot), not `|` (breaks Markdown tables).

---

## Missing Collection Behavior

| Collection | Missing Behavior |
|---|---|
| `resume` | Auto-indexed before scoring via `_ensure_indexed()` |
| `role_archetypes` | Auto-indexed before scoring via `_ensure_indexed()` |
| `global_positive_signals` | Returns `0.0` culture_score — not an error |
| `negative_signals` | Returns `0.0` (no penalty) — not an error |
| `decisions` | Returns `0.0` history_score — not an error |

---

## Config Fields

### `settings.toml` — `[scoring]` section

| Field | Default | Range | Notes |
|---|---|---|---|
| `archetype_weight` | 0.5 | [0.0, 1.0] | Role-type axis |
| `fit_weight` | 0.3 | [0.0, 1.0] | Role-type axis |
| `culture_weight` | 0.2 | [0.0, 1.0] | Environment axis |
| `history_weight` | 0.2 | [0.0, 1.0] | Learned preferences |
| `comp_weight` | 0.15 | [0.0, 1.0] | Compensation signal |
| `negative_weight` | 0.4 | [0.0, 1.0] | Environment axis (penalty) |

### CLI Flags

- `--archetypes-only` rebuilds `role_archetypes`, `global_positive_signals`, and `negative_signals` (all three)
