# Article coverage — every claim in the source articles, witnessed by the model

This is the durable plan for loops 5 onward. Read it first after any context loss.

## The goal, and why it is stated as a number

The rulebook was bootstrapped from five articles by Jessica Talisman (kept locally,
never committed, under `bootstrap/git-ignored-articles/`):

| Id | Article |
|---|---|
| `pkm-1` | Intentional Arrangement — Process Knowledge Management, Part I: Accounting for How We Work |
| `pkm-2` | Part II: Collection Development and Organizing Principles |
| `pkm-3` | Part III: How We Lost Our Way |
| `pkm-4` | Part IV: From Theory to Practice, The Procedural Knowledge Ontology |
| `ont-4` | Ontology Series, Part IV: Governance, Maintenance, and AI |

The target is **100% coverage of every article**. "Coverage" is not an opinion in
prose. It is a rulebook formula over three tables, computed by every substrate:

```
SourceArticles  <- ArticleClaims  <- ClaimEvidence -> RulebookFields / RulebookTables /
 CoveragePercent    IsCovered         IsValid          RoleQuestions / OntologyProfiles /
                                                       KnowledgeMethods / Procedures
```

One `ArticleClaims` row per thing an article says a process-knowledge system must
represent, do, or answer. Claims are **paraphrased with a section reference** — the
articles are paid content and are never quoted into git.

## What counts as evidence — the bar is in the formula, not in this file

`ClaimEvidence.IsValid` decides. Every evidence row needs a non-blank
`Justification`, and then the evidence must be of the kind the claim demands:

| ClaimKind | Valid only when |
|---|---|
| `Concept` | a table with rows, or a field with at least one **substantive** value (not blank, not false, not zero) |
| `Prescription` | a **witness**: a field invented for an answered role question that **discriminates** over the seed data (at least two distinct values) |
| `Illustration` | same as Prescription — the point the example illustrates is witnessed |
| `CompetencyQuestion` | a role question that is answered and carries a substrate-computed `WitnessedAnswer` |
| `Standard` | an ontology profile with at least one semantic mapping, or a knowledge method applied by a seeded row (`ElicitationSessions.Method` or `MethodApplications`) |
| `Scenario` | a procedure with at least one recorded execution |

Every claim is **atomic**: one valid evidence row must prove the whole claim.

"Substantive", "discriminates", and "has rows" are **measured in Postgres** by
`tools/measure_catalog.py` and written back to `RulebookFields` / `RulebookTables`
(the substrate is the oracle; nothing is computed in Python). A table with zero rows,
an all-false boolean, or an all-zero count cannot cover anything.

Substrate agreement is reported beside coverage, not folded into it:
`SourceArticles.AgreedCoveragePercent` also excludes claims whose only evidence is a
field some graded substrate disagrees on. The spreadsheet substrate has no closure
engine, so gating coverage on it would cap coverage for reasons unrelated to the
articles.

## Anti-party-trick rules

1. A claim is never covered by a field whose justification does not say how that
   field answers that claim. `generated/article-coverage.md` projects every claim,
   its evidence and its justification so a skeptic can audit each link.
2. No claim is reclassified to a weaker kind to make it pass. A prescription needs
   a witness that can fire.
3. Every new witness follows the existing loop protocol: role question, predicate,
   seeded violation proving it fires, remediation in model, violating rows kept.
4. New tables lead with `<Entity>Id`. No `IIF`. No `ISBLANK`. Blank checks are bare
   `{{X}} <> ""`. Write the rulebook with `indent=1, ensure_ascii=False`.

## Scenario decisions

- A third procedure family, **lockout/tagout at a fictional plant**, tells Part IV's
  story as the article tells it: normative procedure, variations and exceptions,
  technician executions, elicitation from veteran workers.
- A fourth family, **production deployment with a release approval gate and an AI
  risk classifier**, carries the governance essay's worked example: role/agent
  decoupling, escalation backup, agent model versions, upgrade blast radius.
- Organizations stay fictional (`acme-*`). No real company is represented as data.
- Part V of the PKM series is not in the bootstrap folder, so it is not in scope.

## Cycle

```bash
python3 tools/check_rulebook_integrity.py
effortless build && bash init-db.sh          # compute
python3 tools/measure_catalog.py             # write measurements back
python3 tools/extract_computed_answers.py    # write witnessed answers back
python3 tools/reconcile_field_catalog.py
effortless build && bash init-db.sh          # recompute coverage over measurements
psql -d erb_procedural_knowledge_ontology -c "select * from vw_source_articles"
```

## How the build-out is organized

- `tools/article_claims.py` — the 900 claims. Audited against the article text by one
  independent reader per article (splits, missing claims, wrong kinds, quotations removed).
- `tools/loops/themes.py` — assigns every claim to one theme; writes `claims_<theme>.txt`.
- `tools/loops/CONTRACT.md` — the rules every theme author follows.
- `tools/loops/loopNN_<theme>.py` — one spec per theme, applied by `tools/apply_loop_spec.py`.
- `tools/loops/evidence_<theme>.py` — that theme's evidence rows; the ledger merges them all.
- `tools/loops/probe_specs.py` — validates specs on a scratch copy with Postgres only
  (seconds, not the seven-minute project build). Must exit 0 before a spec is applied.
- `tools/add_article_coverage_ledger.py` — owns the ledger tables; re-run after claims or
  evidence change. `tools/measure_catalog.py` — the measurement write-back.

| Theme | Loop | Spec | Claims | State (2026-09-17) |
|---|---|---|---|---|
| A foundation | loop-06 | `loop06_foundation.py` | 192 | done, 192 with evidence |
| B collection (elicitation, tacit) | loop-07 | `loop07_collection.py` | 77 | done, 77 |
| C organizing | loop-08 | `loop08_organizing.py` | 109 | done, 109 |
| D encoding | loop-09 | `loop09_encoding.py` | 162 | done, 160 (pkm1-i06, pkm4-c12 uncovered) |
| E governance | loop-10 | `loop10_governance.py` | 190 | done, 187 (pkm1-s09, pkm4-i07, pkm4-s13 uncovered) |
| F sourcing | loop-11 | `loop11_sourcing.py` | 61 | done, 61 |
| G social | loop-12 | `loop12_social.py` | 60 | done, 60 |
| H provenance | loop-13 | `loop13_provenance.py` | 42 | done, 42 |
| I gaps | loop-14 | `loop14_gaps.py` | 7 | done, 7 |

Theme B was split into B, G and H after its first author twice exceeded one response's
output cap. Claims pkm4-p23 and pkm4-s17 were split into atomic claims during the merge
review (900 claims in total).

## Merge sequence (serial, after every theme's probe exits 0)

Specs depend on each other's roles and rows, so they are applied in this order, not by
loop number:

```bash
SPECS="tools/loops/loop06_foundation.py tools/loops/loop08_organizing.py tools/loops/loop09_encoding.py \
  tools/loops/loop10_governance.py tools/loops/loop11_sourcing.py tools/loops/loop07_collection.py \
  tools/loops/loop12_social.py tools/loops/loop13_provenance.py tools/loops/loop14_gaps.py"
python3 tools/loops/probe_specs.py --name all $SPECS      # must exit 0
python3 tools/apply_loop_spec.py $SPECS
python3 tools/add_article_coverage_ledger.py
python3 tools/reconcile_field_catalog.py
tools/coverage_cycle.sh          # build, load, measure, rebuild, report coverage per article
```

Target set by the user on 2026-09-15: 98% coverage of the article series, first pass,
no backward compatibility required.

## State after the merge (2026-09-17)

Full project cycle (two builds, measurement, extraction) on the merged rulebook: 260 tables,
5,058 catalog fields, integrity clean, `./start.sh validate` clean, 0 vacuous witnesses.

| Article | Claims | Covered | Coverage |
|---|---|---|---|
| `ont-4` | 230 | 230 | 100.0 |
| `pkm-1` | 110 | 108 | 98.2 |
| `pkm-2` | 283 | 283 | 100.0 |
| `pkm-3` | 99 | 99 | 100.0 |
| `pkm-4` | 178 | 175 | 98.3 |
| **All** | 900 | 895 | 99.4 |

Five honest gaps, left uncovered rather than stretched:

- `pkm1-i06` — the historical record-keeping technologies illustration; nothing in the model is about them.
- `pkm1-s09` — the enterprise-architecture precedent; no framework profile is modeled.
- `pkm4-c12` — three kinds of scarce knowledge; only two are represented.
- `pkm4-i07` — a named EU research project; no data about it is seeded.
- `pkm4-s13` — Linked Data principles; extension terms use `urn:` identifiers, not dereferenceable IRIs.

`agreed_pct` still reads the previous conformance run's `FieldDisagreements`, which predates
the loops 6-14 fields. It becomes meaningful only after a fresh conformance run.

Traps found at merge are recorded in `tools/loops/CONTRACT.md` and guarded where possible:
closure endpoints must be relationships (`apply_loop_spec.py --check`), the relationship graph
must be a DAG (the probe runs the validator), one aggregation per field, and a derived empty
string is not blank. The remote `compile-rulebook` and `rulebook-to-owl` time out on a rulebook
this size; the cycle was run against both tools started locally with temporary
`effortless -setUrl` overrides, removed afterwards.
