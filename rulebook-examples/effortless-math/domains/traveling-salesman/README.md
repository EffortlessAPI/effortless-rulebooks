# Traveling Salesman — Semantic Geometry Starter

**Status:** Research program  
**Version:** 0.3.1  
**Current finite claim:** Gridville's five-stop cycle is reconstructed from inferred structural edges with `CandidateUsedAsAntecedent=false`, uses zero branch decisions, and is optimal for its declared finite instance by a witnessed degree-two lower-bound equality. No general solver or complexity claim is made.

The canonical object is not a solver and not a route. It is an executable dependency graph describing the city, selected stops, canonical weighted edges, supplied and reconstructed route witnesses, typed frontier obligations, local inference certificates, finite optimality certificates, constraint closure, neighborhood boundary states, and residual search.

## Sixty-thousand-foot object

```text
City
└── Neighborhood
    └── Address
        └── InstanceStop
            ├── TravelEdge
            ├── TourStop / TourLeg
            ├── LocalDegreeBound
            │     └── IncidentDominanceCheck
            ├── EdgeState / EdgeSupport
            └── ClusterBoundaryState

TSPInstance
├── InstanceLowerBound
├── DerivedEdgeSet
│   ├── ConnectedDegreeTwoCertificate
│   └── RouteReconstruction
├── CandidateTour
│   └── OptimalityCertificate
├── ConstraintRound / ConstraintDecision
├── FrontierObligation
└── SearchCertificate
```

`InstanceStops` selects addresses into a particular problem instance. `TravelEdges` makes connectivity first-class. A supplied candidate route is represented by ordered `TourStops` and edge-bound `TourLegs`; reconstructed routes are projected independently from certified inferred edge sets.

## Vocabulary

The pieces still to close are generally **frontier obligations**, not imported edges.

| Term | Meaning |
|---|---|
| Imported dependency | An external provider conclusion consumed without internal derivation in this domain. |
| Frontier obligation | Any open semantic edge, typed as inference, certificate, substrate, generalization, or residual search. |
| Kernel assumption | Trusted primitive input semantics recorded at the declared trust boundary. |
| Residual search | Explicit ambiguity remaining after all represented deterministic obligations reach closure. |

The active imported-dependency count is **zero**. Generated Postgres is a commissioned `SUBSTRATE_OBLIGATION`; the mathematical route-reconstruction obligation is also closed.

## Loops 577–596

| Loops | Closure |
|---:|---|
| 577–581 | City hierarchy, weighted graph, supplied route witness, validity, and search baseline. |
| 582–586 | Typed frontier, global cycle coverage, pair completeness, degree-two lower bound, and finite Gridville optimality. |
| 587 | Generated Postgres commissioning, database initialization, and Python/Postgres conformance. |
| 588–591 | Inference application spine, inferred edge union, connected degree-two certificate, and route reconstruction. |
| 592 | Derived search certificate. |
| 593 | Non-tight twin-triangles counterexample. |
| 594–595 | Degree-two forcing and forbidden-edge propagation. |
| 596 | Neighborhood boundary-state contraction. |

Every loop from 587 through 596 was first recorded as `PLANNED` with its before-state and closure criterion, then updated in the same canonical row with its after-state and completion disposition.

## Negative certificates

The rulebook rejects the duplicate-stop route:

```text
A → B → C → B → E → A
```

because `B` is duplicated and `D` is omitted. It also rejects a candidate with five unique visits and five locally legal legs but a duplicated `A→B` transition and missing `E→A` closure. A count-preserving graph with duplicated `A-B` and omitted `B-C` is rejected by canonical pair multiplicity.

Twin triangles provides the stronger counterexample:

```text
certified degree-two lower bound = 6
feasible candidate cost          = 24
selected components              = 2
proper subtours                  = 2
optimality proved                = false
```

Thus a sound local lower bound is not silently upgraded into tightness, connectivity, reconstruction, or optimality.

## Degree-two lower-bound geometry

Every Hamiltonian cycle uses two incident edges at every stop. The witnessed two cheapest incident costs for Gridville are:

```text
A: 2 + 3 = 5
B: 2 + 3 = 5
C: 3 + 3 = 6
D: 3 + 3 = 6
E: 3 + 3 = 6
```

Their sum is `28`. Every tour edge is counted at both endpoints, so every tour costs at least `28 / 2 = 14`. The represented Gridville cycle costs `14`, proving finite-instance optimality.

## Route reconstruction and search accounting

The local-bound supports derive these five distinct edges without consuming a candidate route:

```text
A-B, B-C, C-D, D-E, E-A
```

The rulebook certifies degree two at every required stop, one connected component, a four-edge spanning tree, and zero proper subtours. Deterministic ordering from the depot reconstructs:

```text
A → B → C → D → E → A
CandidateUsedAsAntecedent=false
```

The derived route-discovery accounting is:

```text
initial symmetry-reduced route classes   12
surviving reconstructed route classes     1
route-class contraction               12 → 1
branch decisions                           0
backtracks                                 0
residual ambiguity                         0
```

This is a finite structural result, not a claim about the general complexity of TSP.

## Constraint closure and neighborhood contraction

A sparse fixture executes degree-two forcing and selects five ring edges with no branch decisions. The next closure round forbids one chord by degree saturation and one edge by proper-subtour prevention, each with an explicit reason code.

For each three-stop twin-triangle neighborhood:

```text
6 directed internal orders → 3 undirected boundary states
```

For the paired composition:

```text
36 raw combinations → 9 contracted combinations
36 → 9
```

The contraction certificate is scoped to the represented symmetric three-stop clusters.

## Run

From this directory:

```bash
./start.sh validate   # canonical rulebook + Python substrate
./start.sh all        # validate -> effortless build -> Postgres -> conformance -> show
```

The local database is `erb_traveling_salesman`, unless `TSP_DB` explicitly overrides it.

## Honest frontier

The nearest open obligations are stronger component/crossing lower bounds, general neighborhood-state soundness beyond symmetric three-stop clusters, and explicit branching only after deterministic closure leaves positive residual ambiguity.

---

## Local transpiler bus (`localhost:4242`)

> **All 13 local transpilers live on `localhost:4242`.** Once you run
> `./start.sh` from the repo root, the ssotme-proxy exposes every repo-local
> transpiler — `rulebook-to-postgres`, `rulebook-to-python`, `rulebook-to-golang`,
> `rulebook-to-cobol`, `rulebook-to-owl`, and more — as first-class `ssotme://`
> routes any `effortless build` can call.

## Loops 587–596 — inference geometry and contraction

The rulebook records every loop first as PLANNED with a before-state and closure criterion, then preserves the after-state in the same row. Gridville is reconstructed from inferred edges with zero branch decisions; twin triangles preserves a sound non-tight lower bound and yields the first finite neighborhood boundary-state contraction certificate.

**Postgres commissioning: CLOSED.** Attempts 1–9 remain explicit blocked or failed execution rows. Retry 10 installed the pinned EffortlessAPI/cli archive with npm, generated Postgres and RuleSpeak, initialized `erb_traveling_salesman`, passed the complete Python/Postgres conformance surface, normalized PostgreSQL NUMERIC text through `Decimal`, and durably sealed the canonical certificate. Generated artifact hashes and all available failure/success transcripts are retained.
