# TSP Persistent Exact-Provider Assistance — Loops 1033–1532

## Provider-local finite result

A preregistered campaign compared the same one-thread HiGHS exact binary degree-two plus iterative subtour-elimination formulation in two runtime placements:

- **COLD_REBUILD:** construct a fresh native model for every weight snapshot and retain no proof artifacts.
- **PERSISTENT_START:** retain the fixed-topology native model and accepted SEC rows, update only edge objectives, and install the previous independently verified tour as a MIP start.

The rulebook and Postgres remain the intended canonical editing, orchestration, state-machine, verification, and acceptance surfaces. The numerical provider is purpose-built `highspy` 1.12.0.

**Evidence status in this repository:** provider-local archive summary. These values have not yet been ingested into the canonical loop-710 rulebook and rebuilt through the current Effortless/Postgres gate.

### Combined preregistered evidence

```text
paired scopes                         27
  generated 60-address cities        20
  named TSPLIB topologies             7
weight snapshots                     155
paired wins                       27 / 27
paired losses                          0
objective disagreements                0
provider timeouts                      0

cold end-to-end elapsed          88.107806 s
assisted end-to-end elapsed      31.757388 s
ratio of totals                      2.774×
elapsed reduction                   63.956%
geometric-mean speedup               2.785×
95% bootstrap interval       [2.500×, 3.106×]
minimum paired speedup                1.761×
maximum paired speedup                5.057×

MILP solves                       827 → 321
new SECs generated              4,286 → 836
```

## Generated-city holdout

Twenty unseen 60-address cities, six changing traffic snapshots per city, and three repetitions per configuration:

```text
20 / 20 city wins
49.912287 s → 17.741706 s
2.976× geometric-mean speedup
95% CI [2.641×, 3.347×]
657 → 251 MILP solves
3,691 → 701 newly generated SECs
```

## External-topology replication

Seven preregistered named TSPLIB topologies—`ulysses22`, `bayg29`, `bays29`, `att48`, `berlin52`, `st70`, and `kroA100`—each across five deterministic weight snapshots:

```text
7 / 7 topology wins
38.195519 s → 14.015682 s
2.304× geometric-mean speedup
95% CI [1.933×, 2.776×]
170 → 70 MILP solves
595 → 135 newly generated SECs
```

## Attribution

Development-only factorial ablation:

```text
cold rebuild                       22.628011 s   1.000×
previous verified tour only        24.121867 s   0.938×
accepted SEC memory only           10.816139 s   2.092×
SEC memory + prior tour, rebuild    8.155882 s   2.774×
persistent model, no tour start     9.139868 s   2.476×
persistent model + prior tour       8.041352 s   2.814×
```

Most of the represented gain comes from reusing independently accepted proof cuts. The previous tour helps when paired with those cuts. Model persistence adds a smaller further contribution on the development cohort.

## Corrected closure of the earlier four cases

A timing bug in loops 933–1032 reset the clock before each MILP call, so the old elapsed-time headline and original four-regression diagnosis are withdrawn. With corrected end-to-end timing, nine paired repetitions on each case produced:

```text
7004-day1   1.382×   9 / 9 wins
7006-day0   1.430×   9 / 9 wins
7006-day3   1.090×   9 / 9 wins
7009-day0   1.764×   9 / 9 wins
```

Aggregate: **36 paired wins, 0 losses**. That local selector was later falsified on unseen cities and was not promoted as the primary policy.

## Orchestration state machines

Solve lifecycle:

```text
DECLARED → PLANNED → PREPARED → PROVIDER_BOUND → MODEL_READY
→ RUNNING → CANDIDATE → VERIFIED → ACCEPTED
```

Artifact lifecycle:

```text
PROPOSED → VALIDATED → ADMITTED → COMPILED → INJECTED
→ MEASURED → RETAINED / RETIRED / REJECTED
```

Provider output is never accepted merely because the provider claims optimality. The orchestration layer independently reconstructs each tour, verifies coverage and connectedness, recomputes its objective, and seals an evidence digest.

## Claim boundary

This is a provider-local finite repeated-topology exact-solver-assistance result from one recorded environment. It is not a universal TSP speedup theorem, a cross-machine leaderboard, a claim against Concorde, polynomial normalization, or `P = NP`. No new semantic atom, operator, or active glossary term was introduced in the 500-loop campaign.
