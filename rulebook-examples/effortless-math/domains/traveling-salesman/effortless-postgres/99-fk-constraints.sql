-- ============================================================================
-- 99-fk-constraints.sql — FK CONSTRAINTS (off by default)
-- ============================================================================
-- Demos must never fail on FK violations, so init-db.sh SKIPS this file
-- unless EFFORTLESS_ENFORCE_FKS=true is set in the environment.
--
--   EFFORTLESS_ENFORCE_FKS=true bash init-db.sh    # apply constraints
--   bash init-db.sh                                # leave them documented but unenforced
--
-- The rulebook always documents the FK relationships, and 01-drop-and-create-tables.sql
-- always installs the supporting indexes inline. This file just declares the actual
-- enforcement. Idempotent: every constraint is dropped if present, then added.
-- ============================================================================

-- Cities
ALTER TABLE cities DROP CONSTRAINT IF EXISTS fk_cities_display_name;
ALTER TABLE cities ADD CONSTRAINT fk_cities_display_name
  FOREIGN KEY (display_name) REFERENCES cities (city_id);

-- Neighborhoods
ALTER TABLE neighborhoods DROP CONSTRAINT IF EXISTS fk_neighborhoods_city;
ALTER TABLE neighborhoods ADD CONSTRAINT fk_neighborhoods_city
  FOREIGN KEY (city) REFERENCES cities (city_id);

-- Addresses
ALTER TABLE addresses DROP CONSTRAINT IF EXISTS fk_addresses_neighborhood;
ALTER TABLE addresses ADD CONSTRAINT fk_addresses_neighborhood
  FOREIGN KEY (neighborhood) REFERENCES neighborhoods (neighborhood_id);

-- TSPInstances
ALTER TABLE tsp_instances DROP CONSTRAINT IF EXISTS fk_tsp_instances_city;
ALTER TABLE tsp_instances ADD CONSTRAINT fk_tsp_instances_city
  FOREIGN KEY (city) REFERENCES cities (city_id);
ALTER TABLE tsp_instances DROP CONSTRAINT IF EXISTS fk_tsp_instances_depot_address;
ALTER TABLE tsp_instances ADD CONSTRAINT fk_tsp_instances_depot_address
  FOREIGN KEY (depot_address) REFERENCES addresses (address_id);

-- TSPGraphInvariantChecks
ALTER TABLE tsp_graph_invariant_checks DROP CONSTRAINT IF EXISTS fk_tsp_graph_invariant_checks_tsp_instance;
ALTER TABLE tsp_graph_invariant_checks ADD CONSTRAINT fk_tsp_graph_invariant_checks_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);

-- InstanceStops
ALTER TABLE instance_stops DROP CONSTRAINT IF EXISTS fk_instance_stops_tsp_instance;
ALTER TABLE instance_stops ADD CONSTRAINT fk_instance_stops_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);
ALTER TABLE instance_stops DROP CONSTRAINT IF EXISTS fk_instance_stops_address;
ALTER TABLE instance_stops ADD CONSTRAINT fk_instance_stops_address
  FOREIGN KEY (address) REFERENCES addresses (address_id);

-- TravelEdges
ALTER TABLE travel_edges DROP CONSTRAINT IF EXISTS fk_travel_edges_tsp_instance;
ALTER TABLE travel_edges ADD CONSTRAINT fk_travel_edges_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);
ALTER TABLE travel_edges DROP CONSTRAINT IF EXISTS fk_travel_edges_from_stop;
ALTER TABLE travel_edges ADD CONSTRAINT fk_travel_edges_from_stop
  FOREIGN KEY (from_stop) REFERENCES instance_stops (instance_stop_id);
ALTER TABLE travel_edges DROP CONSTRAINT IF EXISTS fk_travel_edges_to_stop;
ALTER TABLE travel_edges ADD CONSTRAINT fk_travel_edges_to_stop
  FOREIGN KEY (to_stop) REFERENCES instance_stops (instance_stop_id);

-- CandidateTours
ALTER TABLE candidate_tours DROP CONSTRAINT IF EXISTS fk_candidate_tours_tsp_instance;
ALTER TABLE candidate_tours ADD CONSTRAINT fk_candidate_tours_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);

-- TourStops
ALTER TABLE tour_stops DROP CONSTRAINT IF EXISTS fk_tour_stops_candidate_tour;
ALTER TABLE tour_stops ADD CONSTRAINT fk_tour_stops_candidate_tour
  FOREIGN KEY (candidate_tour) REFERENCES candidate_tours (candidate_tour_id);
ALTER TABLE tour_stops DROP CONSTRAINT IF EXISTS fk_tour_stops_instance_stop;
ALTER TABLE tour_stops ADD CONSTRAINT fk_tour_stops_instance_stop
  FOREIGN KEY (instance_stop) REFERENCES instance_stops (instance_stop_id);

-- TourLegs
ALTER TABLE tour_legs DROP CONSTRAINT IF EXISTS fk_tour_legs_candidate_tour;
ALTER TABLE tour_legs ADD CONSTRAINT fk_tour_legs_candidate_tour
  FOREIGN KEY (candidate_tour) REFERENCES candidate_tours (candidate_tour_id);
ALTER TABLE tour_legs DROP CONSTRAINT IF EXISTS fk_tour_legs_from_tour_stop;
ALTER TABLE tour_legs ADD CONSTRAINT fk_tour_legs_from_tour_stop
  FOREIGN KEY (from_tour_stop) REFERENCES tour_stops (tour_stop_id);
ALTER TABLE tour_legs DROP CONSTRAINT IF EXISTS fk_tour_legs_to_tour_stop;
ALTER TABLE tour_legs ADD CONSTRAINT fk_tour_legs_to_tour_stop
  FOREIGN KEY (to_tour_stop) REFERENCES tour_stops (tour_stop_id);
ALTER TABLE tour_legs DROP CONSTRAINT IF EXISTS fk_tour_legs_travel_edge;
ALTER TABLE tour_legs ADD CONSTRAINT fk_tour_legs_travel_edge
  FOREIGN KEY (travel_edge) REFERENCES travel_edges (travel_edge_id);

-- LocalDegreeBounds
ALTER TABLE local_degree_bounds DROP CONSTRAINT IF EXISTS fk_local_degree_bounds_tsp_instance;
ALTER TABLE local_degree_bounds ADD CONSTRAINT fk_local_degree_bounds_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);
ALTER TABLE local_degree_bounds DROP CONSTRAINT IF EXISTS fk_local_degree_bounds_instance_stop;
ALTER TABLE local_degree_bounds ADD CONSTRAINT fk_local_degree_bounds_instance_stop
  FOREIGN KEY (instance_stop) REFERENCES instance_stops (instance_stop_id);
ALTER TABLE local_degree_bounds DROP CONSTRAINT IF EXISTS fk_local_degree_bounds_first_edge;
ALTER TABLE local_degree_bounds ADD CONSTRAINT fk_local_degree_bounds_first_edge
  FOREIGN KEY (first_edge) REFERENCES travel_edges (travel_edge_id);
ALTER TABLE local_degree_bounds DROP CONSTRAINT IF EXISTS fk_local_degree_bounds_second_edge;
ALTER TABLE local_degree_bounds ADD CONSTRAINT fk_local_degree_bounds_second_edge
  FOREIGN KEY (second_edge) REFERENCES travel_edges (travel_edge_id);

-- IncidentDominanceChecks
ALTER TABLE incident_dominance_checks DROP CONSTRAINT IF EXISTS fk_incident_dominance_checks_local_degree_bound;
ALTER TABLE incident_dominance_checks ADD CONSTRAINT fk_incident_dominance_checks_local_degree_bound
  FOREIGN KEY (local_degree_bound) REFERENCES local_degree_bounds (local_degree_bound_id);
ALTER TABLE incident_dominance_checks DROP CONSTRAINT IF EXISTS fk_incident_dominance_checks_other_edge;
ALTER TABLE incident_dominance_checks ADD CONSTRAINT fk_incident_dominance_checks_other_edge
  FOREIGN KEY (other_edge) REFERENCES travel_edges (travel_edge_id);

-- InstanceLowerBounds
ALTER TABLE instance_lower_bounds DROP CONSTRAINT IF EXISTS fk_instance_lower_bounds_tsp_instance;
ALTER TABLE instance_lower_bounds ADD CONSTRAINT fk_instance_lower_bounds_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);

-- OptimalityCertificates
ALTER TABLE optimality_certificates DROP CONSTRAINT IF EXISTS fk_optimality_certificates_candidate_tour;
ALTER TABLE optimality_certificates ADD CONSTRAINT fk_optimality_certificates_candidate_tour
  FOREIGN KEY (candidate_tour) REFERENCES candidate_tours (candidate_tour_id);
ALTER TABLE optimality_certificates DROP CONSTRAINT IF EXISTS fk_optimality_certificates_instance_lower_bound;
ALTER TABLE optimality_certificates ADD CONSTRAINT fk_optimality_certificates_instance_lower_bound
  FOREIGN KEY (instance_lower_bound) REFERENCES instance_lower_bounds (instance_lower_bound_id);

-- TSPExecutionRuns
ALTER TABLE tsp_execution_runs DROP CONSTRAINT IF EXISTS fk_tsp_execution_runs_tsp_loop;
ALTER TABLE tsp_execution_runs ADD CONSTRAINT fk_tsp_execution_runs_tsp_loop
  FOREIGN KEY (tsp_loop) REFERENCES tsp_loops (tsp_loop_id);

-- TSPArtifacts
ALTER TABLE tsp_artifacts DROP CONSTRAINT IF EXISTS fk_tsp_artifacts_execution_run;
ALTER TABLE tsp_artifacts ADD CONSTRAINT fk_tsp_artifacts_execution_run
  FOREIGN KEY (execution_run) REFERENCES tsp_execution_runs (tsp_execution_run_id);

-- TSPConformanceChecks
ALTER TABLE tsp_conformance_checks DROP CONSTRAINT IF EXISTS fk_tsp_conformance_checks_execution_run;
ALTER TABLE tsp_conformance_checks ADD CONSTRAINT fk_tsp_conformance_checks_execution_run
  FOREIGN KEY (execution_run) REFERENCES tsp_execution_runs (tsp_execution_run_id);

-- TSPInferenceStates
ALTER TABLE tsp_inference_states DROP CONSTRAINT IF EXISTS fk_tsp_inference_states_tsp_instance;
ALTER TABLE tsp_inference_states ADD CONSTRAINT fk_tsp_inference_states_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);
ALTER TABLE tsp_inference_states DROP CONSTRAINT IF EXISTS fk_tsp_inference_states_tsp_loop;
ALTER TABLE tsp_inference_states ADD CONSTRAINT fk_tsp_inference_states_tsp_loop
  FOREIGN KEY (tsp_loop) REFERENCES tsp_loops (tsp_loop_id);

-- TSPInferenceApplications
ALTER TABLE tsp_inference_applications DROP CONSTRAINT IF EXISTS fk_tsp_inference_applications_inference_state;
ALTER TABLE tsp_inference_applications ADD CONSTRAINT fk_tsp_inference_applications_inference_state
  FOREIGN KEY (inference_state) REFERENCES tsp_inference_states (tsp_inference_state_id);
ALTER TABLE tsp_inference_applications DROP CONSTRAINT IF EXISTS fk_tsp_inference_applications_inference_rule;
ALTER TABLE tsp_inference_applications ADD CONSTRAINT fk_tsp_inference_applications_inference_rule
  FOREIGN KEY (inference_rule) REFERENCES tsp_inference_rules (tsp_inference_rule_id);
ALTER TABLE tsp_inference_applications DROP CONSTRAINT IF EXISTS fk_tsp_inference_applications_tsp_loop;
ALTER TABLE tsp_inference_applications ADD CONSTRAINT fk_tsp_inference_applications_tsp_loop
  FOREIGN KEY (tsp_loop) REFERENCES tsp_loops (tsp_loop_id);

-- TSPInferenceAntecedents
ALTER TABLE tsp_inference_antecedents DROP CONSTRAINT IF EXISTS fk_tsp_inference_antecedents_inference_application;
ALTER TABLE tsp_inference_antecedents ADD CONSTRAINT fk_tsp_inference_antecedents_inference_application
  FOREIGN KEY (inference_application) REFERENCES tsp_inference_applications (tsp_inference_application_id);

-- TSPEdgeStates
ALTER TABLE tsp_edge_states DROP CONSTRAINT IF EXISTS fk_tsp_edge_states_inference_state;
ALTER TABLE tsp_edge_states ADD CONSTRAINT fk_tsp_edge_states_inference_state
  FOREIGN KEY (inference_state) REFERENCES tsp_inference_states (tsp_inference_state_id);
ALTER TABLE tsp_edge_states DROP CONSTRAINT IF EXISTS fk_tsp_edge_states_travel_edge;
ALTER TABLE tsp_edge_states ADD CONSTRAINT fk_tsp_edge_states_travel_edge
  FOREIGN KEY (travel_edge) REFERENCES travel_edges (travel_edge_id);
ALTER TABLE tsp_edge_states DROP CONSTRAINT IF EXISTS fk_tsp_edge_states_inference_application;
ALTER TABLE tsp_edge_states ADD CONSTRAINT fk_tsp_edge_states_inference_application
  FOREIGN KEY (inference_application) REFERENCES tsp_inference_applications (tsp_inference_application_id);

-- TSPEdgeSupports
ALTER TABLE tsp_edge_supports DROP CONSTRAINT IF EXISTS fk_tsp_edge_supports_edge_state;
ALTER TABLE tsp_edge_supports ADD CONSTRAINT fk_tsp_edge_supports_edge_state
  FOREIGN KEY (edge_state) REFERENCES tsp_edge_states (tsp_edge_state_id);
ALTER TABLE tsp_edge_supports DROP CONSTRAINT IF EXISTS fk_tsp_edge_supports_local_degree_bound;
ALTER TABLE tsp_edge_supports ADD CONSTRAINT fk_tsp_edge_supports_local_degree_bound
  FOREIGN KEY (local_degree_bound) REFERENCES local_degree_bounds (local_degree_bound_id);
ALTER TABLE tsp_edge_supports DROP CONSTRAINT IF EXISTS fk_tsp_edge_supports_inference_application;
ALTER TABLE tsp_edge_supports ADD CONSTRAINT fk_tsp_edge_supports_inference_application
  FOREIGN KEY (inference_application) REFERENCES tsp_inference_applications (tsp_inference_application_id);

-- TSPDerivedEdgeSets
ALTER TABLE tsp_derived_edge_sets DROP CONSTRAINT IF EXISTS fk_tsp_derived_edge_sets_tsp_instance;
ALTER TABLE tsp_derived_edge_sets ADD CONSTRAINT fk_tsp_derived_edge_sets_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);
ALTER TABLE tsp_derived_edge_sets DROP CONSTRAINT IF EXISTS fk_tsp_derived_edge_sets_inference_state;
ALTER TABLE tsp_derived_edge_sets ADD CONSTRAINT fk_tsp_derived_edge_sets_inference_state
  FOREIGN KEY (inference_state) REFERENCES tsp_inference_states (tsp_inference_state_id);

-- TSPDerivedEdgeSetMembers
ALTER TABLE tsp_derived_edge_set_members DROP CONSTRAINT IF EXISTS fk_tsp_derived_edge_set_members_derived_edge_set;
ALTER TABLE tsp_derived_edge_set_members ADD CONSTRAINT fk_tsp_derived_edge_set_members_derived_edge_set
  FOREIGN KEY (derived_edge_set) REFERENCES tsp_derived_edge_sets (tsp_derived_edge_set_id);
ALTER TABLE tsp_derived_edge_set_members DROP CONSTRAINT IF EXISTS fk_tsp_derived_edge_set_members_travel_edge;
ALTER TABLE tsp_derived_edge_set_members ADD CONSTRAINT fk_tsp_derived_edge_set_members_travel_edge
  FOREIGN KEY (travel_edge) REFERENCES travel_edges (travel_edge_id);

-- TSPEdgeSetStopDegrees
ALTER TABLE tsp_edge_set_stop_degrees DROP CONSTRAINT IF EXISTS fk_tsp_edge_set_stop_degrees_derived_edge_set;
ALTER TABLE tsp_edge_set_stop_degrees ADD CONSTRAINT fk_tsp_edge_set_stop_degrees_derived_edge_set
  FOREIGN KEY (derived_edge_set) REFERENCES tsp_derived_edge_sets (tsp_derived_edge_set_id);
ALTER TABLE tsp_edge_set_stop_degrees DROP CONSTRAINT IF EXISTS fk_tsp_edge_set_stop_degrees_instance_stop;
ALTER TABLE tsp_edge_set_stop_degrees ADD CONSTRAINT fk_tsp_edge_set_stop_degrees_instance_stop
  FOREIGN KEY (instance_stop) REFERENCES instance_stops (instance_stop_id);

-- TSPSpanningTreeEdges
ALTER TABLE tsp_spanning_tree_edges DROP CONSTRAINT IF EXISTS fk_tsp_spanning_tree_edges_derived_edge_set;
ALTER TABLE tsp_spanning_tree_edges ADD CONSTRAINT fk_tsp_spanning_tree_edges_derived_edge_set
  FOREIGN KEY (derived_edge_set) REFERENCES tsp_derived_edge_sets (tsp_derived_edge_set_id);
ALTER TABLE tsp_spanning_tree_edges DROP CONSTRAINT IF EXISTS fk_tsp_spanning_tree_edges_parent_stop;
ALTER TABLE tsp_spanning_tree_edges ADD CONSTRAINT fk_tsp_spanning_tree_edges_parent_stop
  FOREIGN KEY (parent_stop) REFERENCES instance_stops (instance_stop_id);
ALTER TABLE tsp_spanning_tree_edges DROP CONSTRAINT IF EXISTS fk_tsp_spanning_tree_edges_child_stop;
ALTER TABLE tsp_spanning_tree_edges ADD CONSTRAINT fk_tsp_spanning_tree_edges_child_stop
  FOREIGN KEY (child_stop) REFERENCES instance_stops (instance_stop_id);
ALTER TABLE tsp_spanning_tree_edges DROP CONSTRAINT IF EXISTS fk_tsp_spanning_tree_edges_travel_edge;
ALTER TABLE tsp_spanning_tree_edges ADD CONSTRAINT fk_tsp_spanning_tree_edges_travel_edge
  FOREIGN KEY (travel_edge) REFERENCES travel_edges (travel_edge_id);

-- TSPConnectedDegreeTwoCertificates
ALTER TABLE tsp_connected_degree_two_certificates DROP CONSTRAINT IF EXISTS fk_tsp_connected_degree_two_certificates_derived_edge_set;
ALTER TABLE tsp_connected_degree_two_certificates ADD CONSTRAINT fk_tsp_connected_degree_two_certificates_derived_edge_set
  FOREIGN KEY (derived_edge_set) REFERENCES tsp_derived_edge_sets (tsp_derived_edge_set_id);

-- TSPRouteReconstructions
ALTER TABLE tsp_route_reconstructions DROP CONSTRAINT IF EXISTS fk_tsp_route_reconstructions_tsp_instance;
ALTER TABLE tsp_route_reconstructions ADD CONSTRAINT fk_tsp_route_reconstructions_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);
ALTER TABLE tsp_route_reconstructions DROP CONSTRAINT IF EXISTS fk_tsp_route_reconstructions_derived_edge_set;
ALTER TABLE tsp_route_reconstructions ADD CONSTRAINT fk_tsp_route_reconstructions_derived_edge_set
  FOREIGN KEY (derived_edge_set) REFERENCES tsp_derived_edge_sets (tsp_derived_edge_set_id);
ALTER TABLE tsp_route_reconstructions DROP CONSTRAINT IF EXISTS fk_tsp_route_reconstructions_start_stop;
ALTER TABLE tsp_route_reconstructions ADD CONSTRAINT fk_tsp_route_reconstructions_start_stop
  FOREIGN KEY (start_stop) REFERENCES instance_stops (instance_stop_id);
ALTER TABLE tsp_route_reconstructions DROP CONSTRAINT IF EXISTS fk_tsp_route_reconstructions_comparison_candidate;
ALTER TABLE tsp_route_reconstructions ADD CONSTRAINT fk_tsp_route_reconstructions_comparison_candidate
  FOREIGN KEY (comparison_candidate) REFERENCES candidate_tours (candidate_tour_id);

-- TSPRouteReconstructionSteps
ALTER TABLE tsp_route_reconstruction_steps DROP CONSTRAINT IF EXISTS fk_tsp_route_reconstruction_steps_route_reconstruction;
ALTER TABLE tsp_route_reconstruction_steps ADD CONSTRAINT fk_tsp_route_reconstruction_steps_route_reconstruction
  FOREIGN KEY (route_reconstruction) REFERENCES tsp_route_reconstructions (tsp_route_reconstruction_id);
ALTER TABLE tsp_route_reconstruction_steps DROP CONSTRAINT IF EXISTS fk_tsp_route_reconstruction_steps_from_stop;
ALTER TABLE tsp_route_reconstruction_steps ADD CONSTRAINT fk_tsp_route_reconstruction_steps_from_stop
  FOREIGN KEY (from_stop) REFERENCES instance_stops (instance_stop_id);
ALTER TABLE tsp_route_reconstruction_steps DROP CONSTRAINT IF EXISTS fk_tsp_route_reconstruction_steps_to_stop;
ALTER TABLE tsp_route_reconstruction_steps ADD CONSTRAINT fk_tsp_route_reconstruction_steps_to_stop
  FOREIGN KEY (to_stop) REFERENCES instance_stops (instance_stop_id);
ALTER TABLE tsp_route_reconstruction_steps DROP CONSTRAINT IF EXISTS fk_tsp_route_reconstruction_steps_travel_edge;
ALTER TABLE tsp_route_reconstruction_steps ADD CONSTRAINT fk_tsp_route_reconstruction_steps_travel_edge
  FOREIGN KEY (travel_edge) REFERENCES travel_edges (travel_edge_id);

-- TSPSearchCertificates
ALTER TABLE tsp_search_certificates DROP CONSTRAINT IF EXISTS fk_tsp_search_certificates_tsp_instance;
ALTER TABLE tsp_search_certificates ADD CONSTRAINT fk_tsp_search_certificates_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);
ALTER TABLE tsp_search_certificates DROP CONSTRAINT IF EXISTS fk_tsp_search_certificates_tsp_loop;
ALTER TABLE tsp_search_certificates ADD CONSTRAINT fk_tsp_search_certificates_tsp_loop
  FOREIGN KEY (tsp_loop) REFERENCES tsp_loops (tsp_loop_id);
ALTER TABLE tsp_search_certificates DROP CONSTRAINT IF EXISTS fk_tsp_search_certificates_derived_edge_set;
ALTER TABLE tsp_search_certificates ADD CONSTRAINT fk_tsp_search_certificates_derived_edge_set
  FOREIGN KEY (derived_edge_set) REFERENCES tsp_derived_edge_sets (tsp_derived_edge_set_id);

-- TSPConstraintRounds
ALTER TABLE tsp_constraint_rounds DROP CONSTRAINT IF EXISTS fk_tsp_constraint_rounds_tsp_instance;
ALTER TABLE tsp_constraint_rounds ADD CONSTRAINT fk_tsp_constraint_rounds_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);
ALTER TABLE tsp_constraint_rounds DROP CONSTRAINT IF EXISTS fk_tsp_constraint_rounds_tsp_loop;
ALTER TABLE tsp_constraint_rounds ADD CONSTRAINT fk_tsp_constraint_rounds_tsp_loop
  FOREIGN KEY (tsp_loop) REFERENCES tsp_loops (tsp_loop_id);

-- TSPConstraintDecisions
ALTER TABLE tsp_constraint_decisions DROP CONSTRAINT IF EXISTS fk_tsp_constraint_decisions_constraint_round;
ALTER TABLE tsp_constraint_decisions ADD CONSTRAINT fk_tsp_constraint_decisions_constraint_round
  FOREIGN KEY (constraint_round) REFERENCES tsp_constraint_rounds (tsp_constraint_round_id);
ALTER TABLE tsp_constraint_decisions DROP CONSTRAINT IF EXISTS fk_tsp_constraint_decisions_travel_edge;
ALTER TABLE tsp_constraint_decisions ADD CONSTRAINT fk_tsp_constraint_decisions_travel_edge
  FOREIGN KEY (travel_edge) REFERENCES travel_edges (travel_edge_id);
ALTER TABLE tsp_constraint_decisions DROP CONSTRAINT IF EXISTS fk_tsp_constraint_decisions_instance_stop;
ALTER TABLE tsp_constraint_decisions ADD CONSTRAINT fk_tsp_constraint_decisions_instance_stop
  FOREIGN KEY (instance_stop) REFERENCES instance_stops (instance_stop_id);
ALTER TABLE tsp_constraint_decisions DROP CONSTRAINT IF EXISTS fk_tsp_constraint_decisions_inference_rule;
ALTER TABLE tsp_constraint_decisions ADD CONSTRAINT fk_tsp_constraint_decisions_inference_rule
  FOREIGN KEY (inference_rule) REFERENCES tsp_inference_rules (tsp_inference_rule_id);

-- TSPClusterBoundaryStates
ALTER TABLE tsp_cluster_boundary_states DROP CONSTRAINT IF EXISTS fk_tsp_cluster_boundary_states_tsp_instance;
ALTER TABLE tsp_cluster_boundary_states ADD CONSTRAINT fk_tsp_cluster_boundary_states_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);
ALTER TABLE tsp_cluster_boundary_states DROP CONSTRAINT IF EXISTS fk_tsp_cluster_boundary_states_neighborhood;
ALTER TABLE tsp_cluster_boundary_states ADD CONSTRAINT fk_tsp_cluster_boundary_states_neighborhood
  FOREIGN KEY (neighborhood) REFERENCES neighborhoods (neighborhood_id);
ALTER TABLE tsp_cluster_boundary_states DROP CONSTRAINT IF EXISTS fk_tsp_cluster_boundary_states_entry_stop;
ALTER TABLE tsp_cluster_boundary_states ADD CONSTRAINT fk_tsp_cluster_boundary_states_entry_stop
  FOREIGN KEY (entry_stop) REFERENCES instance_stops (instance_stop_id);
ALTER TABLE tsp_cluster_boundary_states DROP CONSTRAINT IF EXISTS fk_tsp_cluster_boundary_states_exit_stop;
ALTER TABLE tsp_cluster_boundary_states ADD CONSTRAINT fk_tsp_cluster_boundary_states_exit_stop
  FOREIGN KEY (exit_stop) REFERENCES instance_stops (instance_stop_id);
ALTER TABLE tsp_cluster_boundary_states DROP CONSTRAINT IF EXISTS fk_tsp_cluster_boundary_states_internal_via_stop;
ALTER TABLE tsp_cluster_boundary_states ADD CONSTRAINT fk_tsp_cluster_boundary_states_internal_via_stop
  FOREIGN KEY (internal_via_stop) REFERENCES instance_stops (instance_stop_id);

-- TSPClusterBoundaryStateMembers
ALTER TABLE tsp_cluster_boundary_state_members DROP CONSTRAINT IF EXISTS fk_tsp_cluster_boundary_state_members_cluster_boundary_state;
ALTER TABLE tsp_cluster_boundary_state_members ADD CONSTRAINT fk_tsp_cluster_boundary_state_members_cluster_boundary_state
  FOREIGN KEY (cluster_boundary_state) REFERENCES tsp_cluster_boundary_states (tsp_cluster_boundary_state_id);
ALTER TABLE tsp_cluster_boundary_state_members DROP CONSTRAINT IF EXISTS fk_tsp_cluster_boundary_state_members_travel_edge;
ALTER TABLE tsp_cluster_boundary_state_members ADD CONSTRAINT fk_tsp_cluster_boundary_state_members_travel_edge
  FOREIGN KEY (travel_edge) REFERENCES travel_edges (travel_edge_id);

-- TSPClusterContractionCertificates
ALTER TABLE tsp_cluster_contraction_certificates DROP CONSTRAINT IF EXISTS fk_tsp_cluster_contraction_certificates_tsp_instance;
ALTER TABLE tsp_cluster_contraction_certificates ADD CONSTRAINT fk_tsp_cluster_contraction_certificates_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);
ALTER TABLE tsp_cluster_contraction_certificates DROP CONSTRAINT IF EXISTS fk_tsp_cluster_contraction_certificates_neighborhood;
ALTER TABLE tsp_cluster_contraction_certificates ADD CONSTRAINT fk_tsp_cluster_contraction_certificates_neighborhood
  FOREIGN KEY (neighborhood) REFERENCES neighborhoods (neighborhood_id);

-- TSPFrontierObligations
ALTER TABLE tsp_frontier_obligations DROP CONSTRAINT IF EXISTS fk_tsp_frontier_obligations_inference_rule;
ALTER TABLE tsp_frontier_obligations ADD CONSTRAINT fk_tsp_frontier_obligations_inference_rule
  FOREIGN KEY (inference_rule) REFERENCES tsp_inference_rules (tsp_inference_rule_id);
ALTER TABLE tsp_frontier_obligations DROP CONSTRAINT IF EXISTS fk_tsp_frontier_obligations_opened_by_loop;
ALTER TABLE tsp_frontier_obligations ADD CONSTRAINT fk_tsp_frontier_obligations_opened_by_loop
  FOREIGN KEY (opened_by_loop) REFERENCES tsp_loops (tsp_loop_id);
ALTER TABLE tsp_frontier_obligations DROP CONSTRAINT IF EXISTS fk_tsp_frontier_obligations_closed_by_loop;
ALTER TABLE tsp_frontier_obligations ADD CONSTRAINT fk_tsp_frontier_obligations_closed_by_loop
  FOREIGN KEY (closed_by_loop) REFERENCES tsp_loops (tsp_loop_id);

-- TSPLoops
ALTER TABLE tsp_loops DROP CONSTRAINT IF EXISTS fk_tsp_loops_primary_inference_rule;
ALTER TABLE tsp_loops ADD CONSTRAINT fk_tsp_loops_primary_inference_rule
  FOREIGN KEY (primary_inference_rule) REFERENCES tsp_inference_rules (tsp_inference_rule_id);

-- SearchMetrics
ALTER TABLE search_metrics DROP CONSTRAINT IF EXISTS fk_search_metrics_tsp_instance;
ALTER TABLE search_metrics ADD CONSTRAINT fk_search_metrics_tsp_instance
  FOREIGN KEY (tsp_instance) REFERENCES tsp_instances (tsp_instance_id);
ALTER TABLE search_metrics DROP CONSTRAINT IF EXISTS fk_search_metrics_tsp_loop;
ALTER TABLE search_metrics ADD CONSTRAINT fk_search_metrics_tsp_loop
  FOREIGN KEY (tsp_loop) REFERENCES tsp_loops (tsp_loop_id);
ALTER TABLE search_metrics DROP CONSTRAINT IF EXISTS fk_search_metrics_candidate_tour;
ALTER TABLE search_metrics ADD CONSTRAINT fk_search_metrics_candidate_tour
  FOREIGN KEY (candidate_tour) REFERENCES candidate_tours (candidate_tour_id);

-- TSPInvariantChecks
ALTER TABLE tsp_invariant_checks DROP CONSTRAINT IF EXISTS fk_tsp_invariant_checks_candidate_tour;
ALTER TABLE tsp_invariant_checks ADD CONSTRAINT fk_tsp_invariant_checks_candidate_tour
  FOREIGN KEY (candidate_tour) REFERENCES candidate_tours (candidate_tour_id);

-- 87 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
