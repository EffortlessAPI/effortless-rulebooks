# Traveling Salesman — Semantic Geometry Starter

**Status:** Research program  
**Version:** 0.2.0  
**Current finite claim:** the supplied Gridville cycle is structurally valid and optimal for its declared five-stop instance by a witnessed degree-two lower-bound equality. No general solver or complexity claim is made.

The canonical object is not a solver and not a route. It is an executable dependency graph describing the city, selected stops, canonical weighted edges, supplied route witnesses, typed frontier obligations, local inference certificates, finite optimality certificates, and residual search.

## Sixty-thousand-foot object

```text
City
└── Neighborhood
    └── Address
        └── InstanceStop
            ├── TravelEdge
            ├── TourStop
            ├── TourLeg
            └── LocalDegreeBound
                  └── IncidentDominanceCheck

TSPInstance
├── InstanceLowerBound
├── CandidateTour
│   └── OptimalityCertificate
├── FrontierObligation
└── SearchMetric
```

`InstanceStops` selects addresses into a particular problem instance. `TravelEdges` makes connectivity first-class. A candidate route is represented by ordered `TourStops` and edge-bound `TourLegs`; it is never an opaque list.

## Vocabulary

The pieces still to close are generally **frontier obligations**, not imported edges.

| Term | Meaning |
|---|---|
| Imported dependency | An external provider conclusion consumed without internal derivation in this domain. |
| Frontier obligation | Any open semantic edge, typed as inference, certificate, substrate, generalization, or residual search. |
| Kernel assumption | Trusted primitive input semantics recorded at the declared trust boundary. |
| Residual search | Explicit ambiguity remaining after all represented deterministic obligations reach closure. |

The current active imported-dependency count is **zero**. Live Postgres commissioning is an open `SUBSTRATE_OBLIGATION`; route reconstruction is an open `CERTIFICATE_OBLIGATION`.

## Loops 577–586

| Loop | Closure |
|---:|---|
| 577 | City → Neighborhood → Address hierarchy. |
| 578 | Five-stop graph normalized as ten canonical undirected edges. |
| 579 | Route represented as ordered stops plus edge-bound legs. |
| 580 | Reference route accepted; duplicate-stop route rejected. |
| 581 | Route-discovery baseline recorded as 12 → 12. |
| 582 | Frontier obligations typed separately from imported dependencies. |
| 583 | Every ordered visit must have exactly one incoming and outgoing transition. |
| 584 | Pair identity and multiplicity replace edge count as the completeness certificate. |
| 585 | Two cheapest incident edges at each stop produce a global degree-two lower bound. |
| 586 | Candidate cost 14 equals certified lower bound 14, proving finite-instance optimality. |

## Three negative certificates

The rulebook rejects:

```text
A → B → C → B → E → A
```

because `B` is duplicated and `D` is omitted.

It also rejects a more deceptive candidate with five unique visits and five individually legal legs:

```text
A→B, A→B, B→C, C→D, D→E
```

because `A→B` is duplicated and `E→A` is missing. Global one-in/one-out coverage is required.

Finally, `tsp-gridville-broken-3` has exactly `n(n−1)/2 = 3` edge rows but repeats `A-B` and omits `B-C`; canonical pair multiplicity rejects it. Count alone is not a graph-completeness proof.

## Degree-two lower-bound geometry

Every Hamiltonian cycle uses two incident edges at every stop. The witnessed two cheapest incident costs are:

```text
A: 2 + 3 = 5
B: 2 + 3 = 5
C: 3 + 3 = 6
D: 3 + 3 = 6
E: 3 + 3 = 6
```

Their sum is `28`. Every tour edge is counted at both endpoints, so every tour costs at least:

```text
28 / 2 = 14
```

The supplied cycle `A-B-C-D-E-A` has cost `14`. Bound equality therefore certifies it as optimal **for `tsp-gridville-5` only**.

## Search accounting

Two different questions remain separate:

```text
DISCOVER_ROUTE_WITHOUT_SUPPLIED_CANDIDATE       12 → 12   0% eliminated
VERIFY_OPTIMALITY_OF_SUPPLIED_CANDIDATE         12 → 0  100% enumeration avoided
```

The second result does not claim that the system discovered the route. It proves that exhaustive route comparison is unnecessary to verify this supplied candidate's optimality.

## Run

From this directory:

```bash
./start.sh validate   # canonical rulebook + Python substrate
./start.sh all        # validate -> effortless build -> Postgres -> conformance -> show
```

The local database is `erb_traveling_salesman`, unless `TSP_DB` explicitly overrides it.

## Honest frontier

The nearest open obligations are live Postgres commissioning, route reconstruction from inferred edge rows, a deliberately non-tight lower-bound fixture, degree-two forcing after pruning, neighborhood boundary-state contraction, subtour certificates, and explicit residual branching.

---

## Local transpiler bus (`localhost:4242`)

> **All 13 local transpilers live on `localhost:4242`.** Once you run
> `./start.sh` from the repo root, the ssotme-proxy exposes every repo-local
> transpiler — `rulebook-to-postgres`, `rulebook-to-python`, `rulebook-to-golang`,
> `rulebook-to-cobol`, `rulebook-to-owl`, and more — as first-class `ssotme://`
> routes any `effortless build` can call.

## Loops 587–596 — inference geometry and contraction

The rulebook records every loop first as PLANNED with a before-state and closure criterion, then preserves the after-state in the same row. Gridville is reconstructed from inferred edges with zero branch decisions; twin triangles preserves a sound non-tight lower bound and yields the first finite neighborhood boundary-state contraction certificate. Loop 587 remains honestly BLOCKED when live generated Postgres cannot be commissioned in the execution environment.
