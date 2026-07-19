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
an imported dependency
!= an internal frontier obligation
!= a kernel assumption
!= residual search

and

a supplied route is structurally valid
!= the route is optimal
!= a solver found the route
!= route-discovery search was eliminated
!= P = NP
```

Use **imported dependency** only when the domain consumes an external provider conclusion. Use **frontier obligation** for an open semantic edge inside this domain, typed as `INFERENCE_OBLIGATION`, `CERTIFICATE_OBLIGATION`, `SUBSTRATE_OBLIGATION`, `GENERALIZATION_OBLIGATION`, or `RESIDUAL_SEARCH`.

`CandidateTours.IsHamiltonianCycleWitness` derives from global transition coverage. `CandidateTours.IsOptimalityProved` may become true only from a passing finite-instance certificate; the current Gridville certificate does not generalize to arbitrary TSP instances.

## Search doctrine

Search is the final stage, not the first. Every inference rule must declare:

- soundness;
- completeness;
- runtime;
- memory;
- applicability;
- certificate type.

`SearchMetrics` must state exactly how many branches remain. The initial baseline is intentionally 12 -> 12, or 0% eliminated.

## Current loops

```text
577  City / Neighborhood / Address hierarchy
578  finite weighted graph normalization
579  ordered route witness
580  initial route certificate + negative witness
581  residual-search baseline
582  typed frontier-obligation ledger
583  global one-in/one-out cycle coverage
584  canonical unordered edge-pair completeness
585  degree-two local-to-global lower bound
586  Gridville finite optimality by bound equality
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

The remaining nearby obligations are:

1. commission the generated Postgres projection and record artifact hashes;
2. reconstruct the cycle from inferred structural edges rather than consuming a supplied route;
3. add a non-tight lower-bound fixture so unresolved optimality remains explicitly open;
4. implement degree-two forced-edge closure only after admissibility pruning creates a genuine degree-two stop.

The success metric remains measured semantic closure and replayable certificates, not merely finding a route.
