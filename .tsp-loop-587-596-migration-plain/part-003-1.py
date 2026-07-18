    finish_loop(rb, contract, 587, status=status, after=after, result=after, disposition=disposition)
    contract["Acceptance"]["PostgresCommissioningStatus"] = status
    ATTEMPT.parent.mkdir(parents=True, exist_ok=True)
    write_json(ATTEMPT, {"status": status, "failure": failure, "has_effortless": has_effortless, "has_psql": has_psql, "output_tail": output})
    save(rb, contract); validate(); commit("TSP loop 587: record Postgres commissioning attempt", [ATTEMPT, DOMAIN / "effortless-postgres", DOMAIN / "rulespeak"])


def local_bounds_for(rb: dict[str, Any], instance: str) -> list[dict[str, Any]]:
    return [r for r in rows(rb, "LocalDegreeBounds") if r["TSPInstance"] == instance]


def loop_588() -> None:
    rb = load_json(RULEBOOK); contract = load_json(CONTRACT)
    state_id = "state-gridville-local-bound-selections"
    upsert_rows(rb["TSPInferenceStates"], "TSPInferenceStateId", [{"TSPInferenceStateId": state_id, "TSPInstance": "tsp-gridville-5", "TSPLoop": "tsp-loop-588", "StateKind": "LOCAL_BOUND_EDGE_SELECTIONS", "ParentStateId": None, "Status": "CLOSED", "Description": "Edges selected by explicit two-cheapest incident-edge witnesses; selection is not universal forcing."}])
    edge_to_state: dict[str, str] = {}
    apps = []; ants = []; supports = []
    for bound in local_bounds_for(rb, "tsp-gridville-5"):
        bid = bound["LocalDegreeBoundId"]; app = f"application-{bid}"
        apps.append({"TSPInferenceApplicationId": app, "InferenceState": state_id, "InferenceRule": "tsp-rule-inference-application-spine", "TSPLoop": "tsp-loop-588", "SubjectType": "LOCAL_DEGREE_BOUND", "SubjectId": bid, "ApplicabilityPassed": True, "Conclusion": "The two named incident edges are selected by this local lower-bound witness only.", "CertificateType": "local-bound-edge-selection"})
        ants.append({"TSPInferenceAntecedentId": f"antecedent-{bid}", "InferenceApplication": app, "AntecedentKind": "TWO_CHEAPEST_WITNESS", "AntecedentId": bid, "Statement": "LocalDegreeBounds.IsTwoCheapestWitness is certified by child dominance checks."})
        for edge in [bound["FirstEdge"], bound["SecondEdge"]]:
            edge_to_state.setdefault(edge, f"edge-state-gridville-{edge}")
            supports.append({"TSPEdgeSupportId": f"support-{bid}-{edge}", "EdgeState": edge_to_state[edge], "LocalDegreeBound": bid, "InferenceApplication": app, "SupportKind": "ENDPOINT_LOCAL_BOUND_SELECTION", "Statement": f"{bid} selects {edge}; this is support, not a universal forced-edge claim."})
    edge_states = [{"TSPEdgeStateId": sid, "InferenceState": state_id, "TravelEdge": edge, "DecisionStatus": "SELECTED", "EpistemicStatus": "SELECTED_BY_LOCAL_BOUND_WITNESS", "InferenceApplication": None} for edge, sid in sorted(edge_to_state.items())]
    upsert_rows(rb["TSPInferenceApplications"], "TSPInferenceApplicationId", apps)
    upsert_rows(rb["TSPInferenceAntecedents"], "TSPInferenceAntecedentId", ants)
    upsert_rows(rb["TSPEdgeStates"], "TSPEdgeStateId", edge_states)
    upsert_rows(rb["TSPEdgeSupports"], "TSPEdgeSupportId", supports)
    byapp = by_id(rb, "TSPInferenceApplications")
    for es in edge_states:
        first_support = next(s for s in supports if s["EdgeState"] == es["TSPEdgeStateId"])
        es["InferenceApplication"] = first_support["InferenceApplication"]
    set_frontier(rb, "frontier-inference-application-spine", "CLOSED", 588, "CLOSED_BY_FIRST_CLASS_APPLICATION_ANTECEDENT_STATE_AND_SUPPORT_ROWS")
    after = f"Represented {len(apps)} applications, {len(ants)} antecedents, {len(edge_states)} selected edge states, and {len(supports)} supports; no edge was labeled FORCED."
    finish_loop(rb, contract, 588, status="CLOSED", after=after, result=after)
    contract["Acceptance"]["InferenceApplications"] = len(apps); contract["Acceptance"]["SelectedNotForcedEdgeStates"] = len(edge_states)
    save(rb, contract); validate(); commit("TSP loop 588: add inference application spine")


def edge_rows(rb: dict[str, Any], instance: str) -> list[dict[str, Any]]:
    return [r for r in rows(rb, "TravelEdges") if r["TSPInstance"] == instance]


def loop_589() -> None:
    rb = load_json(RULEBOOK); contract = load_json(CONTRACT)
    state = "state-gridville-derived-edge-union"
    upsert_rows(rb["TSPInferenceStates"], "TSPInferenceStateId", [{"TSPInferenceStateId": state, "TSPInstance": "tsp-gridville-5", "TSPLoop": "tsp-loop-589", "StateKind": "DERIVED_EDGE_SET", "ParentStateId": "state-gridville-local-bound-selections", "Status": "CLOSED", "Description": "Deduplicated union of edges selected by local degree-bound support rows."}])
    support_rows = rows(rb, "TSPEdgeSupports")
    edge_state_map = by_id(rb, "TSPEdgeStates")
    selected_edges = sorted({edge_state_map[s["EdgeState"]]["TravelEdge"] for s in support_rows if edge_state_map[s["EdgeState"]]["InferenceState"] == "state-gridville-local-bound-selections"})
    edge_map = by_id(rb, "TravelEdges")
    total = sum(float(edge_map[e]["TravelCost"]) for e in selected_edges)
    set_id = "edge-set-gridville-local-bound-union"
    upsert_rows(rb["TSPDerivedEdgeSets"], "TSPDerivedEdgeSetId", [{"TSPDerivedEdgeSetId": set_id, "TSPInstance": "tsp-gridville-5", "InferenceState": state, "DerivationKind": "UNION_OF_LOCAL_DEGREE_BOUND_SELECTIONS", "EdgeCount": len(selected_edges), "RequiredStopCount": 5, "TotalCost": total, "DegreeViolationCount": None, "ConnectedComponentCount": None, "ProperSubtourCount": None, "Status": "DERIVED_UNCERTIFIED"}])
    members = []
    for e in selected_edges:
        count = sum(1 for s in support_rows if edge_state_map[s["EdgeState"]]["TravelEdge"] == e)
        members.append({"TSPDerivedEdgeSetMemberId": f"member-gridville-{e}", "DerivedEdgeSet": set_id, "TravelEdge": e, "SupportCount": count, "SelectedAtBothEndpoints": count == 2, "MemberStatus": "SELECTED"})
    upsert_rows(rb["TSPDerivedEdgeSetMembers"], "TSPDerivedEdgeSetMemberId", members)
