    set_frontier(rb, "frontier-gridville-inferred-edge-set", "CLOSED", 589, "CLOSED_BY_DEDUPLICATED_LOCAL_BOUND_SUPPORT_UNION")
    after = f"Derived {len(selected_edges)} unique edges {selected_edges} at total cost {total:g}; no CandidateTour/TourStop/TourLeg row was an antecedent."
    finish_loop(rb, contract, 589, status="CLOSED", after=after, result=after)
    contract["Acceptance"]["GridvilleInferredEdgeCount"] = len(selected_edges)
    save(rb, contract); validate(); commit("TSP loop 589: derive Gridville edge set")


def instance_stops(rb: dict[str, Any], instance: str) -> list[dict[str, Any]]:
    return [r for r in rows(rb, "InstanceStops") if r["TSPInstance"] == instance and r["IsRequired"]]


def members_for(rb: dict[str, Any], set_id: str) -> list[dict[str, Any]]:
    return [r for r in rows(rb, "TSPDerivedEdgeSetMembers") if r["DerivedEdgeSet"] == set_id]


def graph_certificate(rb: dict[str, Any], set_id: str) -> tuple[dict[str, int], list[list[str]], list[tuple[str, str, str, int]]]:
    edge_map = by_id(rb, "TravelEdges")
    set_row = by_id(rb, "TSPDerivedEdgeSets")[set_id]
    stops = [r["InstanceStopId"] for r in instance_stops(rb, set_row["TSPInstance"])]
    adjacency: dict[str, list[tuple[str, str]]] = {s: [] for s in stops}
    for m in members_for(rb, set_id):
        e = edge_map[m["TravelEdge"]]; a, b = e["FromStop"], e["ToStop"]
        adjacency[a].append((b, e["TravelEdgeId"])); adjacency[b].append((a, e["TravelEdgeId"]))
    degrees = {s: len(adjacency[s]) for s in stops}
    unseen = set(stops); comps: list[list[str]] = []
    while unseen:
        root = min(unseen); q = deque([root]); unseen.remove(root); comp = []
        while q:
            cur = q.popleft(); comp.append(cur)
            for nxt, _ in adjacency[cur]:
                if nxt in unseen: unseen.remove(nxt); q.append(nxt)
        comps.append(sorted(comp))
    root = min(stops); q = deque([(root, 0)]); seen = {root}; tree: list[tuple[str, str, str, int]] = []
    while q:
        cur, depth = q.popleft()
        for nxt, eid in sorted(adjacency[cur]):
            if nxt not in seen:
                seen.add(nxt); tree.append((cur, nxt, eid, depth + 1)); q.append((nxt, depth + 1))
    return degrees, comps, tree


def loop_590() -> None:
    rb = load_json(RULEBOOK); contract = load_json(CONTRACT); set_id = "edge-set-gridville-local-bound-union"
    degrees, comps, tree = graph_certificate(rb, set_id)
    degree_violations = sum(v != 2 for v in degrees.values())
    proper_subtours = sum(1 for comp in comps if len(comp) < len(degrees) and all(degrees[s] == 2 for s in comp))
    set_row = by_id(rb, "TSPDerivedEdgeSets")[set_id]
    set_row.update({"DegreeViolationCount": degree_violations, "ConnectedComponentCount": len(comps), "ProperSubtourCount": proper_subtours, "Status": "CONNECTED_DEGREE_TWO" if degree_violations == 0 and len(comps) == 1 and proper_subtours == 0 else "CONTRADICTED"})
    upsert_rows(rb["TSPEdgeSetStopDegrees"], "TSPEdgeSetStopDegreeId", [{"TSPEdgeSetStopDegreeId": f"degree-{set_id}-{s}", "DerivedEdgeSet": set_id, "InstanceStop": s, "SelectedDegree": d} for s, d in sorted(degrees.items())])
    upsert_rows(rb["TSPSpanningTreeEdges"], "TSPSpanningTreeEdgeId", [{"TSPSpann