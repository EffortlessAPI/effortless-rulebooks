# Traveling Salesman — Semantic Geometry Starter

**Status:** Research program  
**Version:** 0.1.0  
**Current claim:** explicit tours can be validated structurally; optimality is not yet proved.

This domain starts the Traveling Salesman stress test outside pure theorem decomposition. The canonical object is not a solver and not a route. It is an executable dependency graph describing the city, the selected stops, the weighted graph, supplied route witnesses, inference contracts, invariants, and residual search.

## Sixty-thousand-foot object

```text
City
└── Neighborhood
    └── Address
        └── InstanceStop
            ├── TravelEdge
            ├── TourStop
            └── TourLeg
```

`InstanceStops` selects addresses into a particular problem instance. `TravelEdges` promotes address-to-address connectivity into a first-class entity, so there is no hidden many-to-many relation. A candidate route is represented by ordered `TourStops` and edge-bound `TourLegs`.

## Initial loops

| Loop | Closure |
|---:|---|
| 577 | One city, three neighborhoods, and five addresses form an acyclic spatial hierarchy. |
| 578 | The five stops produce ten canonical undirected edges, matching `n(n-1)/2`. |
| 579 | A route becomes an ordered structural witness rather than an opaque list. |
| 580 | Postgres derives that `A-B-C-D-E-A` is valid and rejects `A-B-C-B-E-A`; neither is promoted to an optimality proof. |
| 581 | Search is measured before inference: 12 symmetry-reduced route classes remain, so semantic elimination is currently 0%. |

## Seed fixture

The initial instance is `tsp-gridville-5`.

```text
A  depot      Downtown
B  market     Downtown
C  north      North Hills
D  hill       North Hills
E  south      South Market
```

The reference route is:

```text
A -> B -> C -> D -> E -> A
```

Its represented cost is `14`. The deliberately invalid witness is:

```text
A -> B -> C -> B -> E -> A
```

It also has five ordered rows and five legal edge traversals, but it repeats `B` and omits `D`. The rulebook must reject it. This is the first negative certificate.

## What Postgres derives

The generated views expose:

```text
vw_tsp_instances.is_complete_undirected_graph
vw_travel_edges.is_admissible
vw_candidate_tours.total_travel_cost
vw_candidate_tours.is_hamiltonian_cycle_witness
vw_candidate_tours.is_optimality_proved
vw_tsp_invariant_checks.is_passing
vw_search_metrics.search_elimination_pct
```

Application or test code reads these columns. It does not reimplement their formulas.

## Run

From this directory:

```bash
./start.sh validate   # rulebook + Python substrate only
./start.sh all        # validate -> effortless build -> Postgres -> conformance -> show
```

Individual stages:

```bash
./start.sh build
./start.sh db
./start.sh test
./start.sh show
./start.sh contract
./start.sh stop
```

The local database is `erb_traveling_salesman`, unless `TSP_DB` explicitly overrides it.

## Conformance boundary

`scripts/reference_model.py` is a peer execution substrate. It consumes only raw rulebook rows and independently evaluates graph completeness and supplied-tour validity. `testing/take-test.py` compares those results with generated Postgres `vw_*` rows.

Expected first acceptance state:

```text
13 domain tables
1 city
3 neighborhoods
5 addresses
5 required instance stops
10 canonical travel edges
2 candidate tours
2/2 rulebook invariant rows passing
reference route: valid=true, cost=14, optimality=false
negative route: valid=false, cost=17, optimality=false
search elimination: 0% (12 -> 12)
```

## Honest frontier

Nothing here claims that the reference route is optimal, that a general solver exists, or that complexity has changed. The next edge to close is `tsp-rule-degree-two-forcing`, followed by forbidden-edge propagation, symmetry collapse, neighborhood boundary states, component contraction, subtour exclusion, lower bounds, and only then residual branching.
