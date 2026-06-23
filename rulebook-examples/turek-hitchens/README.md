# Rulebook — *Does God Exist?* (Hitchens vs. Turek, VCU)

A formal **Effortless Rulebook (ERB)** built from the VCU debate transcript, capturing the debate's structure **and every philosophical concept raised**. Produced by running the transcript through the **Shadle steps** (the `effortless-bootstrap` pipeline), Rulebook-First.

## The Shadle steps, as applied here

| Step | Artifact | What it did |
|---|---|---|
| 1 · Word extraction | `bootstrap/words.txt` | 627 unique tokens pulled from the transcript text |
| 2 · Domain vocabulary | `bootstrap/vocabulary.txt` | trimmed to **142 domain terms** (philosophy of religion) |
| 3 · Glossary | `bootstrap/glossary.md` | every term defined, grouped by area |
| 4 · Narrative | `bootstrap/narrative.md` | prose using the full vocabulary, end-to-end |
| 5 · Mock data & scenarios | `bootstrap/mock-data/scenarios.md` | 8 scenarios exercising each inference rule |
| 6 · Normalized schema | `bootstrap/normalized-schema.md` | 9-table DAG design, FKs, depth-tagged inferences |
| 7 · Rulebook JSON | `effortless-rulebook/effortless-rulebook.json` | the hub — all 142 vocab terms present |
| 8 · Descriptions | (in the JSON) | every table and every field carries a `Description` |
| 9 · Seed data | (in the JSON) | 149 rows across 10 tables |
| 10 · Inference DAG | (in the JSON) | 1°/2°/3° calculated + aggregation fields |
| 11 · Leopold loop | — | ready for `effortless build` → Postgres + other substrates |

## The model (10 tables, 149 rows)

`Debaters` · `Worldviews` · `Arguments` · `Premises` · `Evidence` · `Concepts` · `Claims` · `Rebuttals` · `Thinkers` · `Quotations`

It is a clean DAG (1-to-many only, no cycles, no many-to-many). The derived layer chains three deep:
**Rebuttal → `Claim.RebuttalCount` (1°) → `Argument.TotalRebuttals` (2°) → `Argument.IsContested` (3°).**

## Validation (all passing)

- ✅ Valid JSON, ERB-conformant (PascalCase plural tables, `Name` = slug formula, `Description` on every field)
- ✅ Foreign-key referential integrity (every relationship resolves to a parent `Name`)
- ✅ Acyclic table-level dependency graph
- ✅ **142/142 vocabulary terms present** in the rulebook

## Derived scoreboard (computing the inference DAG)

| Argument | premises | evidence | developed | rebuttals | contested |
|---|--:|--:|:--:|--:|:--:|
| Cosmological | 3 | 5 | ✓ | 1 | ✓ |
| Teleological (parent) | 0 | 0 | – | 1 | ✓ |
| · Fine-Tuning | 3 | 0 | ✓ | 1 | ✓ |
| · Design of Life | 3 | 1 | ✓ | 1 | ✓ |
| Moral | 3 | 0 | ✓ | 2 | ✓ |
| From Reason | 0 | 0 | – | 1 | ✓ |
| From Mathematics | 0 | 0 | – | 0 | – |
| From Free Will | 0 | 0 | – | 0 | – |
| From Consciousness | 0 | 0 | – | 0 | – |
| Poor Design (H) | 2 | 1 | ✓ | 0 | – |
| Deism-Theism Gap (H) | 2 | 0 | ✓ | 1 | ✓ |
| Morality Without God (H) | 3 | 0 | ✓ | 1 | ✓ |
| Religion Poisons Everything (H) | 0 | 0 | – | 1 | ✓ |

Turek: 9 arguments, 8 claims, 16 thinkers cited · Hitchens: 4 arguments, 8 claims, 16 thinkers cited.
**Hostile witnesses** (cited against their own sympathies): Jastrow, Dawkins, Crick, Hoyle, Wickramasinghe, Weinberg, Provine, Dennett.

## Notes

- **Copyright:** `Quotations` store paraphrased *gists*, never long verbatim text.
- **Editorial:** the `data` is one defensible reading of the transcript (which premises belong to which argument, which claims drew rebuttals). It's meant to be tended — edit the rulebook and rebuild, the same as any ERB project.
