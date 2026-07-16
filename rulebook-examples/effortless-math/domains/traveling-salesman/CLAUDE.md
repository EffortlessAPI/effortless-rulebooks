# CLAUDE.md — Traveling Salesman Research Domain

Read and obey the repository root `CLAUDE.md` and `../../CLAUDE.md` first. This file adds TSP-specific constraints.

## Category boundary

This is a mathematical optimization research domain. Do not add admin-portal, user, orchestration, or platform tables here.

## Canonical semantic asset

```text
effortless-rulebook/traveling-salesman-rulebook.json
```

The Postgres schema, functions, views, RuleSpeak narrative, Python execution, reports, and any future UI are projections. Never create a parallel route-status registry.

## Current scope

The initial scope is deliberately finite and narrow:

```text
City
  -> Neighborhood
      -> Address
          -> InstanceStop
              -> TravelEdge / TourStop / TourLeg
```

The fixture is one five-address symmetric complete weighted graph with a fixed depot.

## Status doctrine

Never conflate:

```text
a supplied route is structurally valid
!= the route is optimal
!= a solver found the route
!= search was eliminated
!= P = NP
```

`CandidateTours.IsHamiltonianCycleWitness` may become true from represented structure. `CandidateTours.IsOptimalityProved` remains false until an actual lower-bound/optimality certificate is represented and closed.

## Search doctrine

Search is the final stage, not the first. Every inference rule must declare:

- soundness;
- completeness;
- runtime;
- memory;
- applicability;
- certificate type.

`SearchMetrics` must state exactly how many branches remain. The initial baseline is intentionally 12 -> 12, or 0% eliminated.

## First loops

```text
577  City / Neighborhood / Address hierarchy
578  finite weighted graph normalization
579  ordered route witness
580  Postgres route certificate + negative witness
581  residual-search baseline
```

Do not renumber these rows.

## Postgres

The local database is:

```text
erb_traveling_salesman
```

Reads come only from generated `vw_*` views. Writes and seed loading go through base tables via generated SQL. Do not hand-edit generated `00`–`05` SQL files.

Use:

```bash
./start.sh validate
./start.sh all
```

The build must fail loudly if the rulebook, generated initializer, `effortless` CLI, `psql`, or formatter is missing.

## Immediate next frontier

Implement `tsp-rule-degree-two-forcing` as a witnessed closure rule. The success metric is not “found a route”; it is a measured reduction in `BranchCountAfter` with a replayable forced-edge certificate.
