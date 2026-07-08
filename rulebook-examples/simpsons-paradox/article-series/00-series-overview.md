# Trust the Artifact, Not the Autocomplete — Article Series Overview

## Series Premise

> LLMs are useful for proposing patterns, names, and abstractions, but the valid
> knowledge is the executable artifact: the rulebook, formulas, invariant checks,
> loop history, and reproducible outputs. The uploaded summary explicitly separates
> theorem-level conclusions, instrument-level findings, and provisional corpus
> observations, with the rulebook JSON as the SSoT.

## Umbrella Title

**Trust the Artifact, Not the Autocomplete**

## The Ten Articles

| # | File | Subtitle | Audience |
|---|------|----------|----------|
| 1 | 01-general-public.md | A Public Guide to Auditable AI | General public / magazine |
| 2 | 02-executive.md | A Boardroom Guide to Governed AI Knowledge | CEOs, boards, strategy leaders |
| 3 | 03-developer.md | A Developer Guide to Rulebooks, Builds, and Invariants | Engineers, platform teams |
| 4 | 04-data-scientist.md | A Data Scientist's Guide to Predicate Invention | Data scientists, ML researchers |
| 5 | 05-knowledge-governance.md | A Knowledge Governance Guide to Institutional Memory | CDOs, data stewards, enterprise architects |
| 6 | 06-legal-compliance.md | A Compliance Guide to Machine-Generated Claims | Lawyers, regulators, auditors |
| 7 | 07-product-ops.md | An Operations Guide to Repeatable AI Workflows | Product managers, operations leaders |
| 8 | 08-high-school.md | A Student's Guide to Rumors, Receipts, and Reasoning | Students, teachers, curious nontechnical readers |
| 9 | 09-young-child.md | A Story for Children About Guessing and Checking | Ages 7–10 |
| 10 | 10-phd-ontologist.md | An Ontologist's Guide to Executable Vocabulary Evolution | PhD ontologists, semantic web researchers |

## Suggested Publishing Order

Publish by reach, not by difficulty:

1. General public (broad thesis)
2. Executive / board (organizational stakes)
3. Developer (technical pattern)
4. Data scientist (research validation)
5. Knowledge governance (institutional memory)
6. Legal / compliance (due process framing)
7. Product / ops (repeatable workflows)
8. High school / education (accessible pedagogy)
9. Young child (simplest metaphor)
10. PhD ontologist (deep theory)

## Source Repository

All concrete details sourced from:
`rulebook-examples/simpsons-paradox/effortless-rulebook/simpsons-paradox-rulebook.json`

Key artifacts:
- **Loops table**: 85 loop iterations complete (loops 86–90 next; 97–107 queued)
- **InvariantChecks table**: 25 critical algebraic assertions (all FailCount=0) + 5 warning-only
- **TreatmentRankings**: derived fields with actual formula strings; 86 manifest sign-flips across 238 studies
- **Conclusions table**: 35 rows; 5 witnessed theorems + 1 candidate; epistemic-tier separation (theorem / discovery-finding / corpus-pattern-superseded)
- **ConfounderIdentities / StratumVariableIdentityMaps / IdentityDomainCells**: 19 confounding archetypes; cross-study identity maps; domain × identity flip-rate matrix (loops 80–85)
- **ModelSummary**: N=238 corpus self-portrait; 19 confounder archetypes; 24 active discovery hypotheses
- **owl/reason.py**: cross-substrate conformance testing
- **IngestionProtocol / StudyImportTemplate**: 17-item adapter contract, 6-step encoding checklist

Key findings to draw from (as of loop-85):
- **Perfect balance theorem (pending loop-86)**: 118 studies with `ArmSizeRatio=0.5` AND `MaxStratumImbalance=0.0` — zero sign-flips. Minimum imbalance at any flip: 0.033 (flu-vaccine-elderly).
- **Purity inversion (pending loop-87)**: collider/selection identities have the *highest* mean signal purity (0.916–1.000) yet require `do-not-condition` policy — the data looks cleanest where naive stratification is most harmful.
- **Naming fragility**: `id-institutional-unit` spans 28 distinct normalized variable names across the corpus (department, school, hospital_incidence_level, precinct_base_rate, grade, task_type…) — quantified explanation for why paradox replication fails across fields.
- **Domain modulation**: `id-disease-severity` flips 53% of the time in medicine, 0% in epidemiology and legal — same mechanism, radically different behavior by domain.
- **PolicyDefault is empirically exact**: every `stratify-*` identity has `sign_flips == stratify_changes_answer` across all studies — the policy never advises stratification when the answer doesn't change.
