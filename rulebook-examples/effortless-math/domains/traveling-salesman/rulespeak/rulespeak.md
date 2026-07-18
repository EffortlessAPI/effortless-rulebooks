# 📘 traveling-salesman — RuleSpeak

_Traveling Salesman research domain at city/neighborhood/address scale. The rulebook normalizes finite weighted city graphs, validates explicit ordered-tour witnesses, records typed frontier obligations, derives a degree-two lower bound, and certifies the supplied Gridville route as optimal for its declared finite instance without claiming a general solver._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **City** | Cities provide the outer spatial boundary for TSP instances. | — |
| Name | The same as its display name. | _Human-readable city label._ |
| **Neighborhood** | Neighborhoods partition a city into first-class spatial clusters. | — |
| Name | The same as its display name. | _Human-readable neighborhood label._ |
| City Name | Taken from the linked city. | _Display name of the containing city._ |
| **Address** | Addresses are visitable physical locations. Coordinates support visualization; edge costs remain explicit graph data. | — |
| Name | The same as its street label. | _Human-readable address label._ |
| Neighborhood Name | Taken from the linked neighborhood. | _Display name of the containing neighborhood._ |
| City | Taken from the linked neighborhood. | _City reached through the address's neighborhood._ |
| **TSP Instance** | One finite Traveling Salesman problem instance over required city addresses. | — |
| Name | The same as its display name. | _Human-readable instance label._ |
| Count of Stops | The number of instance stops related to the TSP instance. | _Number of normalized instance-stop rows._ |
| Count of Required Stops | The number of the TSP instance's instance stops that are required. | _Number of required visit stops._ |
| Count of Travel Edges | The number of travel edges related to the TSP instance. | _Number of canonical undirected travel edges._ |
| Count of Inadmissible Edges | The number of the TSP instance's travel edges that are not admissible. | _Edge rows failing canonical pair, endpoint, availability, or instance checks._ |
| Count of Non Unique Edge Pair Rows | The number of the TSP instance's travel edges that are not unique canonical pairs. | _Rows participating in a duplicated canonical unordered edge pair._ |
| Expected Undirected Edge Count | Computed as the count of stops times the count of stops minus 1 divided by 2. | _n(n-1)/2 expected edges for a complete undirected graph._ |
| Is Complete Undirected Graph | True when all of the following hold: the count of travel edges is the expected undirected edge count; the count of inadmissible edges is 0; and the count of non unique edge pair rows is 0. | _Whether the instance has the expected edge count, no inadmissible rows, and no duplicated unordered pair rows._ |
| **TSP Graph Invariant Check** | Graph-normalization acceptance checks, including a count-preserving duplicate-pair counterexample. | — |
| Name | The same as its display name. | _Human-readable invariant label._ |
| Actual Complete Undirected Graph | True when the TSP graph invariant check's TSP instance is a complete undirected graph. | _Derived graph-completeness status._ |
| Actual Travel Edge Count | The count of travel edges of the TSP graph invariant check's TSP instance. | _Derived raw edge-row count._ |
| Actual Non Unique Edge Pair Rows | The count of non unique edge pair rows of the TSP graph invariant check's TSP instance. | _Derived duplicated-pair row count._ |
| Is Passing | True when all of the following hold: the expected complete undirected graph is the actual complete undirected graph; the expected travel edge count is the actual travel edge count; and the expected non unique edge pair rows is the actual non unique edge pair rows. | _Whether all graph expectations agree with derived values._ |
| **Instance Stop** | Junction entity selecting addresses into a TSP instance without introducing a many-to-many relationship. | — |
| Instance City | Taken from the linked TSP instance. | _City declared by the TSP instance._ |
| Instance Depot Address | Taken from the linked TSP instance. | _Depot address declared by the TSP instance._ |
| Address Name | Taken from the linked address. | _Human-readable address label._ |
| Address City | Taken from the linked address. | _City reached through the selected address._ |
| Neighborhood | Taken from the linked address. | _Neighborhood of the selected address._ |
| X Coordinate | Taken from the linked address. | _X coordinate of the selected address._ |
| Y Coordinate | Taken from the linked address. | _Y coordinate of the selected address._ |
| Is Within Instance City | True when the address city is the instance city. | _Whether the selected address belongs to the instance city._ |
| Is Depot | True when the address is the instance depot address. | _Whether this stop is the instance depot._ |
| Count of Canonical Edges From | The number of travel edges related to the instance stop. | _Canonical edge rows whose first endpoint is this stop._ |
| Count of Canonical Edges to | The number of travel edges related to the instance stop. | _Canonical edge rows whose second endpoint is this stop._ |
| Count of Incident Edges | Computed as the count of canonical edges from plus the count of canonical edges to. | _Total canonical incident edge rows at this stop._ |
| Count of Admissible Edges From | The number of travel edges related to the instance stop. | _Admissible edge rows whose first endpoint is this stop._ |
| Count of Admissible Edges to | The number of travel edges related to the instance stop. | _Admissible edge rows whose second endpoint is this stop._ |
| Count of Admissible Incident Edges | Computed as the count of admissible edges from plus the count of admissible edges to. | _Total currently admissible edges incident to this stop._ |
| **Travel Edge** | Canonical undirected weighted edges between instance stops. The edge entity resolves the address-to-address many-to-many relation. | — |
| Expected Canonical Pair Key | Determined by priority: the from stop, followed by “|”, followed by the to stop if the from stop is less than the to stop; in all other cases, the to stop, followed by “|”, followed by the from stop. | _Endpoint-derived canonical pair key used to verify the supplied pair identity._ |
| Is Canonical Pair Key Correct | True when the canonical pair key is the expected canonical pair key. | _Whether CanonicalPairKey agrees with the sorted endpoint identities._ |
| Is Canonical Endpoint Order | True when the from stop is less than the to stop. | _Whether the stored FromStop/ToStop order is the declared canonical lexical order._ |
| Pair Multiplicity | The number of travel edges related to the travel edge. | _Number of edge rows in this instance carrying the same canonical unordered pair key._ |
| Is Unique Canonical Pair | True when the pair multiplicity is 1. | _Whether exactly one edge row represents this unordered stop pair in the instance._ |
| From TSP Instance | Taken from the linked from stop. | _Instance of the first endpoint._ |
| To TSP Instance | Taken from the linked to stop. | _Instance of the second endpoint._ |
| Is Self Loop | True when the from stop is the to stop. | _Whether both endpoints are the same stop._ |
| Is Within Instance | True when all of the following hold: the TSP instance is the from TSP instance and the TSP instance is the to TSP instance. | _Whether the edge and both endpoints belong to one instance._ |
| Is Admissible | True when all of the following hold: the available flag is set; the self loop flag is not set; the within instance flag is set; the canonical endpoint order flag is set; the canonical pair key correct flag is set; and the unique canonical pair flag is set. | _First-order edge admissibility including availability, instance membership, canonical endpoint order, correct pair identity, and pair uniqueness._ |
| **Candidate Tour** | Explicit candidate tours. Validity is derived; optimality is deliberately not inferred. | — |
| Name | The same as its display name. | _Human-readable tour label._ |
| Required Stop Count | The count of required stops of the candidate tour's TSP instance. | _Required stop count supplied by the instance._ |
| Count of Tour Stops | The number of tour stops related to the candidate tour. | _Number of ordered tour-stop rows._ |
| Count of Tour Legs | The number of tour legs related to the candidate tour. | _Number of ordered tour-leg rows._ |
| Count of Non Unique Visits | The number of the candidate tour's tour stops that are not unique visits. | _Tour-stop rows whose instance stop appears more than once._ |
| Count of Non Unique Sequences | The number of the candidate tour's tour stops that are not unique sequences. | _Tour-stop rows whose sequence position is duplicated._ |
| Count of Out of Instance Stops | The number of the candidate tour's tour stops that are not in-tour-instance. | _Tour stops that do not belong to the candidate's instance._ |
| Count of Non Required Stops | The number of the candidate tour's tour stops that are not stop is required. | _Tour stops not marked required by the instance._ |
| Count of Depot Stops | The number of the candidate tour's tour stops that are stop is depot. | _Number of depot visits in the ordered stop list._ |
| Count of Depot Position Violations | The number of the candidate tour's tour stops that are not depot at first positions. | _Depot rows not placed at sequence position one._ |
| Count of Invalid Legs | The number of the candidate tour's tour legs that are not valid. | _Tour legs failing membership, sequence, edge, or instance checks._ |
| Count of Cycle Degree Violations | The number of the candidate tour's tour stops that are not cycle-degree-satisfied. | _Ordered visits lacking exactly one incoming and one outgoing candidate leg._ |
| Count of Non Unique Leg Orders | The number of the candidate tour's tour legs that are not unique leg orders. | _Leg rows whose LegOrder is duplicated within the candidate._ |
| Total Travel Cost | The total travel cost across the tour legs related to the candidate tour. | _Sum of validated tour-leg costs._ |
| Is Hamiltonian Cycle Witness | True when all of the following hold: the count of tour stops is the required stop count; the count of tour legs is the required stop count; the count of non unique visits is 0; the count of non unique sequences is 0; the count of out of instance stops is 0; the count of non required stops is 0; the count of depot stops is 1; the count of depot position violations is 0; the count of invalid legs is 0; the count of cycle degree violations is 0; and the count of non unique leg orders is 0. | _Whether the supplied ordered rows form one globally covered Hamiltonian cycle for the declared instance._ |
| Count of Passing Optimality Certificates | The number of the candidate tour's optimality certificates that are passing. | _Passing finite-instance optimality certificates attached to this candidate._ |
| Is Optimality Proved | True when all of the following hold: the hamiltonian cycle witness flag is set and the count of passing optimality certificates is greater than 0. | _Whether at least one passing finite-instance optimality certificate closes this candidate's scope._ |
| Residual Claim | Determined by priority: “OPTIMAL_FOR_DECLARED_FINITE_INSTANCE” if the optimality proved flag is set; “VALID_TOUR_NOT_OPTIMALITY_PROOF” if the hamiltonian cycle witness flag is set; in all other cases, “INVALID_TOUR”. | _Honest current epistemic status of the candidate._ |
| **Tour Stop** | Ordered candidate-tour visits. Multiplicity and sequence uniqueness are derived from the row set. | — |
| Tour TSP Instance | Taken from the linked candidate tour. | _TSP instance owned by the candidate tour._ |
| Stop TSP Instance | Taken from the linked instance stop. | _TSP instance owning the visited stop._ |
| Stop is Required | True when the linked instance stop is required. | _Whether the visited stop is required._ |
| Stop is Depot | True when the tour stop's instance stop is a depot. | _Whether the visited stop is the depot._ |
| Is in Tour Instance | True when the tour TSP instance is the stop TSP instance. | _Whether the visited stop belongs to the candidate's instance._ |
| Visit Multiplicity | The number of tour stops related to the tour stop. | _Number of times this instance stop appears in the candidate._ |
| Sequence Multiplicity | The number of tour stops related to the tour stop. | _Number of rows using this sequence position in the candidate._ |
| Is Unique Visit | True when the visit multiplicity is 1. | _Whether this stop appears exactly once._ |
| Is Unique Sequence | True when the sequence multiplicity is 1. | _Whether this sequence position appears exactly once._ |
| Is Depot At First Position | True when at least one of the following holds: the stop is depot flag is not set or the sequence position is 1. | _Non-depot rows pass automatically; the depot must be position one._ |
| Count of Outgoing Legs | The number of tour legs related to the tour stop. | _Tour-leg rows in this candidate whose FromTourStop is this ordered visit._ |
| Count of Incoming Legs | The number of tour legs related to the tour stop. | _Tour-leg rows in this candidate whose ToTourStop is this ordered visit._ |
| Has Exactly One Outgoing Leg | True when the count of outgoing legs is 1. | _Whether this ordered visit has exactly one outgoing candidate leg._ |
| Has Exactly One Incoming Leg | True when the count of incoming legs is 1. | _Whether this ordered visit has exactly one incoming candidate leg._ |
| Is Cycle Degree Satisfied | True when all of the following hold: the exactly one outgoing leg flag is set and the exactly one incoming leg flag is set. | _Whether this ordered visit participates in exactly one incoming and one outgoing transition._ |
| **Tour Leg** | Ordered directed traversal legs bound to canonical undirected travel edges. | — |
| Tour TSP Instance | Taken from the linked candidate tour. | _TSP instance owned by the candidate tour._ |
| Required Stop Count | The count of required stops of the tour leg's tour TSP instance. | _Required stop count reached through the candidate's TSP instance and used to recognize the closing leg._ |
| From Tour Candidate | Taken from the linked from tour stop. | _Candidate tour owning the from visit._ |
| To Tour Candidate | Taken from the linked to tour stop. | _Candidate tour owning the to visit._ |
| From Instance Stop | Taken from the linked from tour stop. | _Instance stop at the start of the leg._ |
| To Instance Stop | Taken from the linked to tour stop. | _Instance stop at the end of the leg._ |
| From Sequence Position | Taken from the linked from tour stop. | _Sequence position at the start of the leg._ |
| To Sequence Position | Taken from the linked to tour stop. | _Sequence position at the end of the leg._ |
| Edge TSP Instance | Taken from the linked travel edge. | _TSP instance owning the selected edge._ |
| Edge From Stop | Taken from the linked travel edge. | _First canonical edge endpoint._ |
| Edge to Stop | Taken from the linked travel edge. | _Second canonical edge endpoint._ |
| Edge Travel Cost | Taken from the linked travel edge. | _Optimization cost of the selected edge._ |
| Edge is Admissible | True when the linked travel edge is admissible. | _Whether the selected edge passes first-order normalization._ |
| Leg Order Multiplicity | The number of tour legs related to the tour leg. | _Number of leg rows in this candidate carrying the same LegOrder._ |
| Is Unique Leg Order | True when the leg order multiplicity is 1. | _Whether this leg order appears exactly once in the candidate._ |
| Is Candidate Membership Valid | True when all of the following hold: the candidate tour is the from tour candidate and the candidate tour is the to tour candidate. | _Whether both ordered visits belong to this candidate._ |
| Is Sequence Transition | True when at least one of the following holds: the to sequence position is the from sequence position plus 1 or all of the following hold: the from sequence position is the required stop count and the to sequence position is 1. | _Whether the leg follows the next visit or closes the route from n back to one._ |
| Is Edge Endpoint Match | True when at least one of the following holds: all of the following hold: the from instance stop is the edge from stop and the to instance stop is the edge to stop or all of the following hold: the from instance stop is the edge to stop and the to instance stop is the edge from stop. | _Whether the directed leg traverses the selected undirected edge in either direction._ |
| Is in Tour Instance | True when the edge TSP instance is the tour TSP instance. | _Whether the edge belongs to the candidate's TSP instance._ |
| Is Valid | True when all of the following hold: the candidate membership valid flag is set; the sequence transition flag is set; the edge endpoint match flag is set; the in tour instance flag is set; the edge is admissible flag is set; and the unique leg order flag is set. | _Whether the leg passes candidate membership, sequence, endpoint, instance, admissibility, and unique-order checks._ |
| Travel Cost | Determined by priority: the edge travel cost if the valid flag is set; in all other cases, 0. | _Cost contributed by a valid leg; invalid legs contribute zero and are counted separately._ |
| **Local Degree Bound** | One degree-two lower-bound witness per required stop. Each row names two incident edges and proves all other incident edges are no cheaper than the second selected edge. | — |
| Stop TSP Instance | Taken from the linked instance stop. | _Instance owning the bounded stop._ |
| Stop Admissible Incident Edge Count | The count of admissible incident edges of the local degree bound's instance stop. | _Current admissible incident-edge count at the stop._ |
| First Edge From Stop | Taken from the linked first edge. | _First selected edge's canonical first endpoint._ |
| First Edge to Stop | Taken from the linked first edge. | _First selected edge's canonical second endpoint._ |
| Second Edge From Stop | Taken from the linked second edge. | _Second selected edge's canonical first endpoint._ |
| Second Edge to Stop | Taken from the linked second edge. | _Second selected edge's canonical second endpoint._ |
| First Edge Cost | The travel cost of the local degree bound's first edge. | _Cost of the first selected edge._ |
| Second Edge Cost | The travel cost of the local degree bound's second edge. | _Cost of the second selected edge._ |
| Is First Edge Incident | True when at least one of the following holds: the instance stop is the first edge from stop or the instance stop is the first edge to stop. | _Whether the first selected edge is incident to the bounded stop._ |
| Is Second Edge Incident | True when at least one of the following holds: the instance stop is the second edge from stop or the instance stop is the second edge to stop. | _Whether the second selected edge is incident to the bounded stop._ |
| Are Selected Edges Distinct | True when the first edge is not the second edge. | _Whether the two selected edge rows are distinct._ |
| Are Selected Costs Ordered | True when the first edge cost is at most the second edge cost. | _Whether the first selected edge is no more expensive than the second._ |
| Required Dominance Check Count | Computed as the stop admissible incident edge count minus 2. | _Number of other incident edges that must be compared against the second selected cost._ |
| Count of Dominance Checks | The number of incident dominance checks related to the local degree bound. | _Represented comparisons against other incident edges._ |
| Count of Failed Dominance Checks | The number of the local degree bound's incident dominance checks that are not passing. | _Dominance comparisons that fail incidence, distinctness, uniqueness, or cost ordering._ |
| Is Two Cheapest Witness | True when all of the following hold: the TSP instance is the stop TSP instance; the first edge incident flag is set; the second edge incident flag is set; the are selected edges distinct flag is set; the are selected costs ordered flag is set; the count of dominance checks is the required dominance check count; and the count of failed dominance checks is 0. | _Whether this row and its child comparisons certify the two cheapest admissible incident edges._ |
| Local Bound Cost | Determined by priority: the first edge cost plus the second edge cost if the two cheapest witness flag is set; in all other cases, 0. | _Certified sum of the two cheapest incident edge costs; zero when the witness is invalid._ |
| **Incident Dominance Check** | Witness rows proving every unselected incident edge is at least as expensive as the second edge named by a LocalDegreeBound. | — |
| Bound Stop | The instance stop of the incident dominance check's local degree bound. | _Stop bounded by the parent witness._ |
| First Selected Edge | Taken from the linked local degree bound. | _First edge selected by the parent witness._ |
| Second Selected Edge | Taken from the linked local degree bound. | _Second edge selected by the parent witness._ |
| Second Selected Edge Cost | Taken from the linked local degree bound. | _Cost of the second selected edge._ |
| Other Edge From Stop | Taken from the linked other edge. | _Other edge's first endpoint._ |
| Other Edge to Stop | Taken from the linked other edge. | _Other edge's second endpoint._ |
| Other Edge Cost | The travel cost of the incident dominance check's other edge. | _Cost of the unselected incident edge._ |
| Other Edge Multiplicity | The number of incident dominance checks related to the incident dominance check. | _Number of comparison rows in the same local witness naming this other edge._ |
| Is Other Edge Incident | True when at least one of the following holds: the bound stop is the other edge from stop or the bound stop is the other edge to stop. | _Whether the compared edge is incident to the bounded stop._ |
| Is Other Edge Unselected | True when all of the following hold: the other edge is not the first selected edge and the other edge is not the second selected edge. | _Whether the compared edge differs from both selected edges._ |
| Is Other Edge Unique | True when the other edge multiplicity is 1. | _Whether this unselected edge appears exactly once among the parent's comparisons._ |
| Is Dominated by Second Cost | True when the other edge cost is at least the second selected edge cost. | _Whether the unselected edge is no cheaper than the second selected edge._ |
| Is Passing | True when all of the following hold: the other edge incident flag is set; the other edge unselected flag is set; the other edge unique flag is set; and the dominated by second cost flag is set. | _Whether the comparison is incident, unselected, unique, and cost-dominated._ |
| **Instance Lower Bound** | Global lower bounds obtained by summing certified local degree-two bounds and dividing by two because each tour edge is incident to two stops. | — |
| Required Stop Count | The count of required stops of the instance lower bound's TSP instance. | _Required stop count of the instance._ |
| Count of Local Degree Bounds | The number of local degree bounds related to the instance lower bound. | _Local degree-bound rows represented for the instance._ |
| Count of Invalid Local Degree Bounds | The number of the instance lower bound's local degree bounds that are not two cheapest witnesses. | _Local degree-bound rows that do not carry a valid two-cheapest witness._ |
| Total Local Degree Bound Cost | The total local bound cost across the local degree bounds related to the instance lower bound. | _Sum of the local two-edge lower bounds across all required stops._ |
| Lower Bound Cost | Computed as the base lower bound cost plus the supplemental bound adjustment. | _Certified global tour lower bound after double-count correction._ |
| Is Certified | True when all of the following hold: the count of local degree bounds is the required stop count; the count of invalid local degree bounds is 0; and the supplemental bound certified flag is set. | _Whether every required stop has one valid local witness and the global double-counting aggregation is closed._ |
| Base Lower Bound Cost | Computed as the total local degree bound cost divided by 2. | _Degree-two base lower bound._ |
| Count of Certified Supplemental Terms | The number of the instance lower bound's TSP bound terms that count a toward adjustment and are certified. | _Certified terms contributing to the adjustment._ |
| Is Supplemental Bound Certified | True when the count of certified supplemental terms is the required supplemental term count. | _Whether the required repair terms are all certified._ |
| **TSP Bound Term** | Signed additive and constraint terms composing reusable lower-bound certificates. | — |
| Term Value | Computed as the quantity times the unit weight times the sign. | _Signed additive contribution._ |
| **Optimality Certificate** | Finite-instance optimality certificates. Passing requires a valid Hamiltonian witness, a certified lower bound for the same instance, and equality between candidate cost and lower bound. | — |
| Candidate TSP Instance | Taken from the linked candidate tour. | _Instance owned by the candidate._ |
| Lower Bound TSP Instance | Taken from the linked instance lower bound. | _Instance owned by the lower-bound certificate._ |
| Candidate Travel Cost | The total travel cost of the optimality certificate's candidate tour. | _Derived travel cost of the candidate._ |
| Lower Bound Cost | Taken from the linked instance lower bound. | _Certified lower-bound cost._ |
| Candidate is Hamiltonian Cycle Witness | True when the linked candidate tour is a hamiltonian cycle witness. | _Whether the candidate is a valid Hamiltonian-cycle witness._ |
| Lower Bound is Certified | True when the linked instance lower bound is certified. | _Whether the lower-bound construction is certified._ |
| Is Same Instance | True when the candidate TSP instance is the lower bound TSP instance. | _Whether candidate and lower bound refer to the same TSP instance._ |
| Is Bound Tight | True when the candidate travel cost is the lower bound cost. | _Whether the candidate cost equals the certified lower bound._ |
| Is Passing | True when all of the following hold: the candidate is hamiltonian cycle witness flag is set; the lower bound is certified flag is set; the same instance flag is set; and the bound tight flag is set. | _Whether bound equality certifies this candidate as optimal for the declared finite instance._ |
| Scope Claim | Determined by priority: “OPTIMAL_FOR_DECLARED_FINITE_INSTANCE” if the passing flag is set; in all other cases, “OPTIMALITY_NOT_CERTIFIED”. | _Honest scope of the certificate._ |
| **TSP Execution Run** | Execution attempts against a concrete substrate; blocked and failed runs remain first-class evidence. | — |
| Is Successful | True when all of the following hold: the build succeeded flag is set; the database initialized flag is set; and the conformance succeeded flag is set. | _Whether all three commissioning stages succeeded._ |
| **TSP Artifact** | Content-addressed artifacts emitted or expected from substrate execution. | — |
| **TSP Conformance Check** | Expected-versus-actual checks for a substrate run. | — |
| Is Passing | True when the status is “PASS”. | _Whether expected and actual agree._ |
| **TSP Inference State** | Named immutable snapshots of the current deterministic inference frontier. | — |
| **TSP Inference Application** | Individual applications of declared inference rules. | — |
| **TSP Inference Antecedent** | Antecedent facts consumed by one inference application. | — |
| **TSP Edge State** | Epistemically typed state of one edge in one inference snapshot. | — |
| Is Decided | True when it is not the case that the decision status is “UNKNOWN”. | _Whether the edge is no longer unknown._ |
| Commitment Rank | Determined by priority: 0 if the decision status is “UNKNOWN”; 1 if the decision status is “SELECTED”; 2 if at least one of the following holds: the decision status is “FORCED” or the decision status is “FORBIDDEN”; in all other cases, 3. | _Ordered strength in the edge commitment lattice._ |
| Commitment Polarity | Determined by priority: the negative of 1 if the decision status is “FORBIDDEN”; 0 if the decision status is “UNKNOWN”; in all other cases, 1. | _Positive inclusion, zero unknown, or negative exclusion._ |
| Necessity Scope | Determined by priority: “ALL_FEASIBLE_TOURS” if the decision status is “FORCED”; “NO_FEASIBLE_TOUR” if the decision status is “FORBIDDEN”; “CURRENT_WITNESS” if the decision status is “SELECTED”; in all other cases, “UNRESOLVED_OR_HISTORICAL”. | _Scope in which the commitment holds._ |
| Is Terminal Commitment | True when at least one of the following holds: the decision status is “FORCED”; the decision status is “FORBIDDEN”; the decision status is “CONTRADICTED”; or the decision status is “SUPERSEDED”. | _Whether deterministic closure no longer treats the edge as open._ |
| **TSP Edge Support** | Witness rows supporting an edge-state conclusion. | — |
| **TSP Derived Edge Set** | First-class edge sets derived from inference rows rather than supplied route order. | — |
| Is Connected Degree Two | True when all of the following hold: the degree violation count is 0; the connected component count is 1; and the proper subtour count is 0. | _Whether this is one connected degree-two cycle._ |
| **TSP Defect Profile** | Uniform incidence, connectivity, boundary, and cost defects for stops, instances, witnesses, and quotient nodes. | — |
| Incidence Defect | Computed as the absolute value of the required incidence minus the observed incidence. | _Absolute incidence deficit or excess._ |
| Connectivity Defect | Computed as the largest of the component count minus 1 and 0. | _Components beyond the required one._ |
| Boundary Defect | Computed as the largest of the required boundary crossings minus the observed boundary crossings and 0. | _Missing required boundary crossings._ |
| Cost Gap | Computed as the upper bound cost minus the lower bound cost. | _Upper minus lower value._ |
| Defect Vector | Computed as the incidence defect, followed by “|”, followed by the connectivity defect, followed by “|”, followed by the boundary defect, followed by “|”, followed by the cost gap. | _Canonical four-coordinate defect vector._ |
| **TSP Derived Edge Set Member** | Member edges and support counts for a derived edge set. | — |
| **TSP Edge Set Stop Degree** | Selected degree of each required stop in a derived edge set. | — |
| Is Degree Two | True when the selected degree is 2. | _Whether selected degree is two._ |
| **TSP Spanning Tree Edge** | Spanning-tree witness proving connectedness of a selected edge set. | — |
| **TSP Connected Degree Two Certificate** | Certificates that a derived edge set is one connected degree-two cycle. | — |
| Is Passing | True when all of the following hold: the edge count is the required stop count; the degree violation count is 0; the component count is 1; the proper subtour count is 0; and the spanning tree edge count is the required stop count minus 1. | _Whether the graph is one connected cycle._ |
| **TSP Route Reconstruction** | Ordered cycles reconstructed from certified edge sets. | — |
| Is Passing | True when all of the following hold: the reconstructed stop count is the required stop count; the reconstructed leg count is the required stop count; the candidate used as antecedent flag is not set; and the status is “RECONSTRUCTED”. | _Whether reconstruction closes without a supplied-route antecedent._ |
| **TSP Route Reconstruction Step** | Ordered traversal steps emitted from an inferred edge set. | — |
| **TSP Witness Normal Form** | Common semantic normal form for supplied, reconstructed, and contracted cycle or path witnesses. | — |
| Semantic Key | Computed as the witness shape, followed by “|”, followed by the TSP instance, followed by “|”, followed by the covered stop count, followed by “|”, followed by the required stop count, followed by “|”, followed by the edge count, followed by “|”, followed by the total cost, followed by “|”, followed by the incidence defect, followed by “|”, followed by the connectivity defect, followed by “|”, followed by the order defect, followed by “|”, followed by the boundary signature (a missing value counts as an empty string). ⚠︎ mechanical <!-- rulespeak:reword --> | _Provenance-independent witness signature._ |
| Is Valid | True when all of the following hold: the covered stop count is the required stop count; the incidence defect is 0; the connectivity defect is 0; the order defect is 0; and at least one of the following holds: all of the following hold: the witness shape is “CYCLE” and the edge count is the required stop count or all of the following hold: the witness shape is “PATH” and the edge count is the required stop count minus 1. | _Whether the normal form is a valid cycle or path._ |
| **TSP Search Certificate** | Derived accounting of route classes, branch decisions, backtracks, and residual ambiguity. | — |
| Route Class Elimination Pct | Determined by priority: 0 if the initial route class count is 0; in all other cases, the initial route class count minus the surviving route class count divided by the initial route class count times 100 rounded to 2 decimal place(s). | _Percentage of route classes removed._ |
| **TSP Constraint Round** | Fixed-point rounds for forced and forbidden edge propagation. | — |
| **TSP Constraint Decision** | Replayable forced or forbidden edge decisions with reason codes. | — |
| **TSP Cluster Boundary State** | Finite undirected entry/exit Hamiltonian-path states for a neighborhood cluster. | — |
| Boundary Signature | Computed as the neighborhood, followed by “|”, followed by the entry stop, followed by “|”, followed by the exit stop, followed by “|”, followed by the internal stop count, followed by “|”, followed by the internal path cost. | _Scope/entry/exit/coverage/cost signature._ |
| Semantic Quotient Key | Computed as the neighborhood, followed by “|”, followed by the entry stop, followed by “|”, followed by the exit stop, followed by “|”, followed by the internal stop count, followed by “|”, followed by the internal path cost. | _Equivalence key preserving boundary signature and cost._ |
| Is Fiber Minimum | True when all of the following hold: the quotient representative flag is set and the dominated flag is not set. | _Whether the state is the surviving minimum in its fiber._ |
| **TSP Cluster Boundary State Member** | Ordered internal edges supporting a cluster boundary state. | — |
| **TSP Cluster Contraction Certificate** | Finite-fixture certificates measuring neighborhood-state contraction. | — |
| **TSP Inference Rule** | Declared structural inference contracts, including soundness, completeness, runtime, memory, applicability, and certificate shape. | — |
| Name | The same as its display name. | _Human-readable inference-rule label._ |
| **TSP Frontier Obligation** | Typed ledger of open and closed semantic frontier obligations. Imported dependencies are used only when an external provider conclusion is actually consumed. | — |
| Name | The same as its display name. | _Human-readable obligation label._ |
| Is Imported Dependency | True when the obligation kind is “IMPORTED_DEPENDENCY”. | _Whether this row consumes an external provider conclusion._ |
| Is Closed | True when the status is “CLOSED”. | _Whether the frontier obligation is closed._ |
| **TSP Loop** | Named semantic loops that establish the initial Traveling Salesman architecture. | — |
| Name | The same as its display name. | _Human-readable loop label._ |
| **TSP Convergence Measurement** | Measurements testing whether surface concepts compress onto a stable predicate basis. | — |
| Semantic Compression Pct | Determined by priority: 0 if the surface concept count before is 0; in all other cases, the surface concept count before minus the primitive count after divided by the surface concept count before times 100 rounded to 2 decimal place(s). | _Surface-to-basis reduction percentage._ |
| **TSP Concept Registry** | Registry of primitive, derived, coined, and historical concepts with explicit basis expressions. | — |
| Is Current Basis Member | True when at least one of the following holds: the status is “ACTIVE_PRIMITIVE” or the status is “ACTIVE_OPERATOR”. | _Whether the row is an active atom or operator._ |
| **Search Metric** | Quantitative record of how much combinatorial search remains after each represented inference stage. | — |
| Search Elimination Pct | Determined by priority: 0 if the branch count before is 0; in all other cases, the branch count before minus the branch count after divided by the branch count before times 100 rounded to 2 decimal place(s). | _Percentage of branches removed without search._ |
| **TSP Invariant Check** | Rulebook-native acceptance checks comparing expected raw outcomes to derived candidate-tour fields. | — |
| Name | The same as its display name. | _Human-readable invariant label._ |
| Actual Hamiltonian Cycle Witness | True when the TSP invariant check's candidate tour is a hamiltonian cycle witness. | _Derived route-validity result from the candidate-tour view._ |
| Actual Optimality Proved | True when the linked candidate tour is optimality proved. | _Derived optimality-certificate status from the candidate-tour view._ |
| Actual Total Travel Cost | Taken from the linked candidate tour. | _Derived total tour cost from the candidate-tour view._ |
| Is Hamiltonian Status Correct | True when the expected hamiltonian cycle witness is the actual hamiltonian cycle witness. | _Whether expected and actual route validity agree._ |
| Is Optimality Status Correct | True when the expected optimality proved is the actual optimality proved. | _Whether expected and actual optimality status agree._ |
| Is Cost Correct | True when the expected total travel cost is the actual total travel cost. | _Whether expected and actual total cost agree._ |
| Is Passing | True when all of the following hold: the hamiltonian status correct flag is set; the optimality status correct flag is set; and the cost correct flag is set. | _Whether all represented expectations agree with derived values._ |

## 2 Fact Types

- a **neighborhood** references exactly one **city**
- an **address** references exactly one **neighborhood**
- a **TSP instance** references exactly one **city**
- a **TSP instance** references exactly one **address**
- a **TSP graph invariant check** references exactly one **TSP instance**
- an **instance stop** references exactly one **TSP instance**
- an **instance stop** references exactly one **address**
- a **travel edge** references exactly one **TSP instance**
- a **travel edge** references exactly one **instance stop**
- a **candidate tour** references exactly one **TSP instance**
- a **tour stop** references exactly one **candidate tour**
- a **tour stop** references exactly one **instance stop**
- a **tour leg** references exactly one **candidate tour**
- a **tour leg** references exactly one **tour stop**
- a **tour leg** references exactly one **travel edge**
- a **local degree bound** references exactly one **TSP instance**
- a **local degree bound** references exactly one **instance stop**
- a **local degree bound** references exactly one **travel edge**
- an **incident dominance check** references exactly one **local degree bound**
- an **incident dominance check** references exactly one **travel edge**
- an **instance lower bound** references exactly one **TSP instance**
- a **TSP bound term** references exactly one **instance lower bound**
- a **TSP bound term** references exactly one **TSP instance**
- a **TSP bound term** references exactly one **TSP loop**
- an **optimality certificate** references exactly one **candidate tour**
- an **optimality certificate** references exactly one **instance lower bound**
- a **TSP execution run** references exactly one **TSP loop**
- a **TSP artifact** references exactly one **TSP execution run**
- a **TSP conformance check** references exactly one **TSP execution run**
- a **TSP inference state** references exactly one **TSP instance**
- a **TSP inference state** references exactly one **TSP loop**
- a **TSP inference application** references exactly one **TSP inference state**
- a **TSP inference application** references exactly one **TSP inference rule**
- a **TSP inference application** references exactly one **TSP loop**
- a **TSP inference antecedent** references exactly one **TSP inference application**
- a **TSP edge state** references exactly one **TSP inference state**
- a **TSP edge state** references exactly one **travel edge**
- a **TSP edge state** may reference one **TSP inference application**
- a **TSP edge support** references exactly one **TSP edge state**
- a **TSP edge support** may reference one **local degree bound**
- a **TSP edge support** references exactly one **TSP inference application**
- a **TSP derived edge set** references exactly one **TSP instance**
- a **TSP derived edge set** references exactly one **TSP inference state**
- a **TSP defect profile** references exactly one **TSP instance**
- a **TSP defect profile** references exactly one **TSP loop**
- a **TSP derived edge set member** references exactly one **TSP derived edge set**
- a **TSP derived edge set member** references exactly one **travel edge**
- a **TSP edge set stop degree** references exactly one **TSP derived edge set**
- a **TSP edge set stop degree** references exactly one **instance stop**
- a **TSP spanning tree edge** references exactly one **TSP derived edge set**
- a **TSP spanning tree edge** references exactly one **instance stop**
- a **TSP spanning tree edge** references exactly one **travel edge**
- a **TSP connected degree two certificate** references exactly one **TSP derived edge set**
- a **TSP route reconstruction** references exactly one **TSP instance**
- a **TSP route reconstruction** references exactly one **TSP derived edge set**
- a **TSP route reconstruction** references exactly one **instance stop**
- a **TSP route reconstruction** may reference one **candidate tour**
- a **TSP route reconstruction step** references exactly one **TSP route reconstruction**
- a **TSP route reconstruction step** references exactly one **instance stop**
- a **TSP route reconstruction step** references exactly one **travel edge**
- a **TSP witness normal form** references exactly one **TSP instance**
- a **TSP witness normal form** references exactly one **TSP loop**
- a **TSP search certificate** references exactly one **TSP instance**
- a **TSP search certificate** references exactly one **TSP loop**
- a **TSP search certificate** may reference one **TSP derived edge set**
- a **TSP constraint round** references exactly one **TSP instance**
- a **TSP constraint round** references exactly one **TSP loop**
- a **TSP constraint decision** references exactly one **TSP constraint round**
- a **TSP constraint decision** references exactly one **travel edge**
- a **TSP constraint decision** may reference one **instance stop**
- a **TSP constraint decision** references exactly one **TSP inference rule**
- a **TSP cluster boundary state** references exactly one **TSP instance**
- a **TSP cluster boundary state** references exactly one **neighborhood**
- a **TSP cluster boundary state** references exactly one **instance stop**
- a **TSP cluster boundary state member** references exactly one **TSP cluster boundary state**
- a **TSP cluster boundary state member** references exactly one **travel edge**
- a **TSP cluster contraction certificate** references exactly one **TSP instance**
- a **TSP cluster contraction certificate** may reference one **neighborhood**
- a **TSP frontier obligation** may reference one **TSP inference rule**
- a **TSP frontier obligation** references exactly one **TSP loop**
- a **TSP loop** references exactly one **TSP inference rule**
- a **TSP convergence measurement** references exactly one **TSP loop**
- a **TSP concept registry** references exactly one **TSP loop**
- a **search metric** references exactly one **TSP instance**
- a **search metric** references exactly one **TSP loop**
- a **search metric** may reference one **candidate tour**
- a **TSP invariant check** references exactly one **candidate tour**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A city **must** have a display name, a country code, and a coordinate system.
- A neighborhood **must** reference exactly one city.
- A neighborhood **must** have a display name and a cluster kind.
- An address **must** reference exactly one neighborhood.
- An address **must** have a street label, a x coordinate, and a y coordinate, and record whether it is a depot candidate.
- A TSP instance **must** reference exactly one city.
- A TSP instance **must** reference exactly one address as its depot address.
- A TSP instance **must** have a display name, a distance model, a status, and a search policy, and record whether it is symmetric.
- A TSP graph invariant check **must** reference exactly one TSP instance.
- A TSP graph invariant check **must** have a display name, an expected travel edge count, and an expected non unique edge pair rows, and record whether it is expected complete undirected graph.
- An instance stop **must** reference exactly one TSP instance.
- An instance stop **must** reference exactly one address.
- An instance stop **must** record whether it is required.
- A travel edge **must** reference exactly one TSP instance.
- A travel edge **must** reference exactly one instance stop as its from stop.
- A travel edge **must** reference exactly one instance stop as its to stop.
- A travel edge **must** have a distance meters, a travel cost, an edge source, and a canonical pair key, and record whether it is available.
- A candidate tour **must** reference exactly one TSP instance.
- A candidate tour **must** have a display name, a candidate kind, a search branches explored, and a backtrack count.
- A tour stop **must** reference exactly one candidate tour.
- A tour stop **must** reference exactly one instance stop.
- A tour stop **must** have a sequence position.
- A tour leg **must** reference exactly one candidate tour.
- A tour leg **must** reference exactly one tour stop as its from tour stop.
- A tour leg **must** reference exactly one tour stop as its to tour stop.
- A tour leg **must** reference exactly one travel edge.
- A tour leg **must** have a leg order.
- A local degree bound **must** reference exactly one TSP instance.
- A local degree bound **must** reference exactly one instance stop.
- A local degree bound **must** reference exactly one travel edge as its first edge.
- A local degree bound **must** reference exactly one travel edge as its second edge.
- An incident dominance check **must** reference exactly one local degree bound.
- An incident dominance check **must** reference exactly one travel edge as its other edge.
- An instance lower bound **must** reference exactly one TSP instance.
- An instance lower bound **must** have a bound kind, a required supplemental term count, a supplemental bound adjustment, and a bound composition kind.
- A TSP bound term **must** reference exactly one instance lower bound as its bound certificate.
- A TSP bound term **must** reference exactly one TSP instance.
- A TSP bound term **must** reference exactly one TSP loop.
- A TSP bound term **must** have a term kind, a quantity, a unit weight, a sign, a constraint kind, a constraint value, and a justification, and record whether it is counts toward adjustment and whether it is certified.
- An optimality certificate **must** reference exactly one candidate tour.
- An optimality certificate **must** reference exactly one instance lower bound.
- A TSP execution run **must** reference exactly one TSP loop.
- A TSP execution run **must** have a substrate, an attempted at, a build command, and a status, and record whether it is build succeeded, whether it is database initialized, and whether it is conformance succeeded.
- A TSP artifact **must** reference exactly one TSP execution run as its execution run.
- A TSP artifact **must** have an artifact kind and a relative path, and record whether it is a present.
- A TSP conformance check **must** reference exactly one TSP execution run as its execution run.
- A TSP conformance check **must** have a check kind, an expected value, and a status.
- A TSP inference state **must** reference exactly one TSP instance.
- A TSP inference state **must** reference exactly one TSP loop.
- A TSP inference state **must** have a state kind, a status, and a description.
- A TSP inference application **must** reference exactly one TSP inference state as its inference state.
- A TSP inference application **must** reference exactly one TSP inference rule as its inference rule.
- A TSP inference application **must** reference exactly one TSP loop.
- A TSP inference application **must** have a subject type, a conclusion, and a certificate type, and record whether it is applicability passed.
- A TSP inference antecedent **must** reference exactly one TSP inference application as its inference application.
- A TSP inference antecedent **must** have an antecedent kind and a statement.
- A TSP edge state **must** reference exactly one TSP inference state as its inference state.
- A TSP edge state **must** reference exactly one travel edge.
- A TSP edge state **must** have a decision status and an epistemic status.
- A TSP edge support **must** reference exactly one TSP edge state as its edge state.
- A TSP edge support **must** reference exactly one TSP inference application as its inference application.
- A TSP edge support **must** have a support kind and a statement.
- A TSP derived edge set **must** reference exactly one TSP instance.
- A TSP derived edge set **must** reference exactly one TSP inference state as its inference state.
- A TSP derived edge set **must** have a derivation kind, an edge count, a required stop count, a total cost, and a status.
- A TSP defect profile **must** reference exactly one TSP instance.
- A TSP defect profile **must** reference exactly one TSP loop.
- A TSP defect profile **must** have a subject kind, a required incidence, an observed incidence, a component count, a required boundary crossings, an observed boundary crossings, a lower bound cost, an upper bound cost, and a status.
- A TSP derived edge set member **must** reference exactly one TSP derived edge set as its derived edge set.
- A TSP derived edge set member **must** reference exactly one travel edge.
- A TSP derived edge set member **must** have a support count and a member status, and record whether it is selected at both endpoints.
- A TSP edge set stop degree **must** reference exactly one TSP derived edge set as its derived edge set.
- A TSP edge set stop degree **must** reference exactly one instance stop.
- A TSP edge set stop degree **must** have a selected degree.
- A TSP spanning tree edge **must** reference exactly one TSP derived edge set as its derived edge set.
- A TSP spanning tree edge **must** reference exactly one instance stop as its parent stop.
- A TSP spanning tree edge **must** reference exactly one instance stop as its child stop.
- A TSP spanning tree edge **must** reference exactly one travel edge.
- A TSP spanning tree edge **must** have a depth.
- A TSP connected degree two certificate **must** reference exactly one TSP derived edge set as its derived edge set.
- A TSP connected degree two certificate **must** have a required stop count, an edge count, a degree violation count, a component count, a proper subtour count, and a spanning tree edge count.
- A TSP route reconstruction **must** reference exactly one TSP instance.
- A TSP route reconstruction **must** reference exactly one TSP derived edge set as its derived edge set.
- A TSP route reconstruction **must** reference exactly one instance stop as its start stop.
- A TSP route reconstruction **must** have an orientation rule, a required stop count, a reconstructed stop count, a reconstructed leg count, a total cost, and a status, and record whether it is candidate used as antecedent.
- A TSP route reconstruction step **must** reference exactly one TSP route reconstruction as its route reconstruction.
- A TSP route reconstruction step **must** reference exactly one instance stop as its from stop.
- A TSP route reconstruction step **must** reference exactly one instance stop as its to stop.
- A TSP route reconstruction step **must** reference exactly one travel edge.
- A TSP route reconstruction step **must** have a step order, and record whether it is a closing step.
- A TSP witness normal form **must** reference exactly one TSP instance.
- A TSP witness normal form **must** reference exactly one TSP loop.
- A TSP witness normal form **must** have a witness shape, an origin kind, a source kind, a covered stop count, a required stop count, an edge count, a total cost, an incidence defect, a connectivity defect, an order defect, and a provenance summary.
- A TSP search certificate **must** reference exactly one TSP instance.
- A TSP search certificate **must** reference exactly one TSP loop.
- A TSP search certificate **must** have a question kind, an initial route class count, a surviving route class count, a branch decision count, a backtrack count, a residual ambiguity count, a branching avoided pct, and a status.
- A TSP constraint round **must** reference exactly one TSP instance.
- A TSP constraint round **must** reference exactly one TSP loop.
- A TSP constraint round **must** have a round order, an input state, a forced decision count, a forbidden decision count, a branch decision count, and a status.
- A TSP constraint decision **must** reference exactly one TSP constraint round as its constraint round.
- A TSP constraint decision **must** reference exactly one travel edge.
- A TSP constraint decision **must** reference exactly one TSP inference rule as its inference rule.
- A TSP constraint decision **must** have a decision status, a reason code, and an antecedent summary, and record whether it is deterministic.
- A TSP cluster boundary state **must** reference exactly one TSP instance.
- A TSP cluster boundary state **must** reference exactly one neighborhood.
- A TSP cluster boundary state **must** reference exactly one instance stop as its entry stop.
- A TSP cluster boundary state **must** reference exactly one instance stop as its exit stop.
- A TSP cluster boundary state **must** reference exactly one instance stop as its internal via stop.
- A TSP cluster boundary state **must** have an internal path cost, an internal stop count, a status, and an orientation multiplicity, and record whether it is a hamiltonian path, whether it is dominated, and whether it is quotient representative.
- A TSP cluster boundary state member **must** reference exactly one TSP cluster boundary state as its cluster boundary state.
- A TSP cluster boundary state member **must** reference exactly one travel edge.
- A TSP cluster boundary state member **must** have a member order.
- A TSP cluster contraction certificate **must** reference exactly one TSP instance.
- A TSP cluster contraction certificate **must** have a scope kind, a raw internal order count, a surviving boundary state count, a reduction pct, a scope claim, an equivalence relation, and a quotient class count, and record whether it is passing.
- A TSP inference rule **must** have a display name, an inference layer, an implementation status, a soundness, a completeness, a runtime class, a memory class, an applicability, a certificate type, and a description.
- A TSP frontier obligation **must** reference exactly one TSP loop as its opened by loop.
- A TSP frontier obligation **must** have a display name, an obligation kind, a status, a trust disposition, a closure criterion, and a certificate type.
- A TSP loop **must** reference exactly one TSP inference rule as its primary inference rule.
- A TSP loop **must** have a loop order, a display name, a status, a new concept, a witness summary, and a next frontier.
- A TSP convergence measurement **must** reference exactly one TSP loop.
- A TSP convergence measurement **must** have a measurement kind, a surface concept count before, a primitive count after, a new primitive count, a derived alias count, a physical table count, a novel term, a prediction status, and a notes.
- A TSP concept registry **must** reference exactly one TSP loop as its introduced by loop.
- A TSP concept registry **must** have a display name, a concept kind, a basis expression, an arity, a source tables, and a status.
- A search metric **must** reference exactly one TSP instance.
- A search metric **must** reference exactly one TSP loop.
- A search metric **must** have a search question, a raw node count, a raw edge count, a forced edge count, a forbidden edge count, a symmetry class count, a component contraction count, a branch count before, a branch count after, a backtrack count, a residual ambiguity count, and a status.
- A TSP invariant check **must** reference exactly one candidate tour.
- A TSP invariant check **must** have a display name and an expected total travel cost, and record whether it is expected hamiltonian cycle witness and whether it is expected optimality proved.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A city's name is the same as its display name. |
| **DR-2 Name** | A neighborhood's name is the same as its display name. |
| **DR-3 City Name** | A neighborhood's city name — taken from the linked city. |
| **DR-4 Name** | An address's name is the same as its street label. |
| **DR-5 Neighborhood Name** | An address's neighborhood name — taken from the linked neighborhood. |
| **DR-6 City** | An address's city — taken from the linked neighborhood. |
| **DR-7 Name** | A TSP instance's name is the same as its display name. |
| **DR-8 Count of Stops** | A TSP instance's count of stops is the number of instance stops related to the TSP instance. |
| **DR-9 Count of Required Stops** | A TSP instance's count of required stops is the number of the TSP instance's instance stops that are required. |
| **DR-10 Count of Travel Edges** | A TSP instance's count of travel edges is the number of travel edges related to the TSP instance. |
| **DR-11 Count of Inadmissible Edges** | A TSP instance's count of inadmissible edges is the number of the TSP instance's travel edges that are not admissible. |
| **DR-12 Count of Non Unique Edge Pair Rows** | A TSP instance's count of non unique edge pair rows is the number of the TSP instance's travel edges that are not unique canonical pairs. |
| **DR-13 Expected Undirected Edge Count** | A TSP instance's expected undirected edge count is computed as the count of stops times the count of stops minus 1 divided by 2. |
| **DR-14 Is Complete Undirected Graph** | A TSP instance is considered a complete undirected graph if all of the following hold: the count of travel edges is the expected undirected edge count; the count of inadmissible edges is 0; and the count of non unique edge pair rows is 0. |
| **DR-15 Name** | A TSP graph invariant check's name is the same as its display name. |
| **DR-16 Actual Complete Undirected Graph** | A TSP graph invariant check's actual complete undirected graph is true when the TSP graph invariant check's TSP instance is a complete undirected graph. |
| **DR-17 Actual Travel Edge Count** | A TSP graph invariant check's actual travel edge count is the count of travel edges of the TSP graph invariant check's TSP instance. |
| **DR-18 Actual Non Unique Edge Pair Rows** | A TSP graph invariant check's actual non unique edge pair rows is the count of non unique edge pair rows of the TSP graph invariant check's TSP instance. |
| **DR-19 Is Passing** | A TSP graph invariant check is considered passing if all of the following hold: the expected complete undirected graph is the actual complete undirected graph; the expected travel edge count is the actual travel edge count; and the expected non unique edge pair rows is the actual non unique edge pair rows. |
| **DR-20 Instance City** | An instance stop's instance city — taken from the linked TSP instance. |
| **DR-21 Instance Depot Address** | An instance stop's instance depot address — taken from the linked TSP instance. |
| **DR-22 Address Name** | An instance stop's address name — taken from the linked address. |
| **DR-23 Address City** | An instance stop's address city — taken from the linked address. |
| **DR-24 Neighborhood** | An instance stop's neighborhood — taken from the linked address. |
| **DR-25 X Coordinate** | An instance stop's x coordinate — taken from the linked address. |
| **DR-26 Y Coordinate** | An instance stop's y coordinate — taken from the linked address. |
| **DR-27 Is Within Instance City** | An instance stop is considered a within instance city if the address city is the instance city. |
| **DR-28 Is Depot** | An instance stop is considered a depot if the address is the instance depot address. |
| **DR-29 Count of Canonical Edges From** | An instance stop's count of canonical edges from is the number of travel edges related to the instance stop. |
| **DR-30 Count of Canonical Edges to** | An instance stop's count of canonical edges to is the number of travel edges related to the instance stop. |
| **DR-31 Count of Incident Edges** | An instance stop's count of incident edges is computed as the count of canonical edges from plus the count of canonical edges to. |
| **DR-32 Count of Admissible Edges From** | An instance stop's count of admissible edges from is the number of travel edges related to the instance stop. |
| **DR-33 Count of Admissible Edges to** | An instance stop's count of admissible edges to is the number of travel edges related to the instance stop. |
| **DR-34 Count of Admissible Incident Edges** | An instance stop's count of admissible incident edges is computed as the count of admissible edges from plus the count of admissible edges to. |
| **DR-35 Expected Canonical Pair Key** | The travel edge's expected canonical pair key is determined by the following priority:<br>1. the from stop, followed by “|”, followed by the to stop, if the from stop is less than the to stop;<br>2. in all other cases, the to stop, followed by “|”, followed by the from stop. |
| **DR-36 Is Canonical Pair Key Correct** | A travel edge is considered a canonical pair key correct if the canonical pair key is the expected canonical pair key. |
| **DR-37 Is Canonical Endpoint Order** | A travel edge is considered a canonical endpoint order if the from stop is less than the to stop. |
| **DR-38 Pair Multiplicity** | A travel edge's pair multiplicity is the number of travel edges related to the travel edge. |
| **DR-39 Is Unique Canonical Pair** | A travel edge is considered a unique canonical pair if the pair multiplicity is 1. |
| **DR-40 From TSP Instance** | A travel edge's from TSP instance — taken from the linked from stop. |
| **DR-41 To TSP Instance** | A travel edge's to TSP instance — taken from the linked to stop. |
| **DR-42 Is Self Loop** | A travel edge is considered a self loop if the from stop is the to stop. |
| **DR-43 Is Within Instance** | A travel edge is considered a within instance if all of the following hold: the TSP instance is the from TSP instance and the TSP instance is the to TSP instance. |
| **DR-44 Is Admissible** | A travel edge is considered admissible if all of the following hold: the available flag is set; the self loop flag is not set; the within instance flag is set; the canonical endpoint order flag is set; the canonical pair key correct flag is set; and the unique canonical pair flag is set. |
| **DR-45 Name** | A candidate tour's name is the same as its display name. |
| **DR-46 Required Stop Count** | A candidate tour's required stop count is the count of required stops of the candidate tour's TSP instance. |
| **DR-47 Count of Tour Stops** | A candidate tour's count of tour stops is the number of tour stops related to the candidate tour. |
| **DR-48 Count of Tour Legs** | A candidate tour's count of tour legs is the number of tour legs related to the candidate tour. |
| **DR-49 Count of Non Unique Visits** | A candidate tour's count of non unique visits is the number of the candidate tour's tour stops that are not unique visits. |
| **DR-50 Count of Non Unique Sequences** | A candidate tour's count of non unique sequences is the number of the candidate tour's tour stops that are not unique sequences. |
| **DR-51 Count of Out of Instance Stops** | A candidate tour's count of out of instance stops is the number of the candidate tour's tour stops that are not in-tour-instance. |
| **DR-52 Count of Non Required Stops** | A candidate tour's count of non required stops is the number of the candidate tour's tour stops that are not stop is required. |
| **DR-53 Count of Depot Stops** | A candidate tour's count of depot stops is the number of the candidate tour's tour stops that are stop is depot. |
| **DR-54 Count of Depot Position Violations** | A candidate tour's count of depot position violations is the number of the candidate tour's tour stops that are not depot at first positions. |
| **DR-55 Count of Invalid Legs** | A candidate tour's count of invalid legs is the number of the candidate tour's tour legs that are not valid. |
| **DR-56 Count of Cycle Degree Violations** | A candidate tour's count of cycle degree violations is the number of the candidate tour's tour stops that are not cycle-degree-satisfied. |
| **DR-57 Count of Non Unique Leg Orders** | A candidate tour's count of non unique leg orders is the number of the candidate tour's tour legs that are not unique leg orders. |
| **DR-58 Total Travel Cost** | A candidate tour's total travel cost is the total travel cost across the tour legs related to the candidate tour. |
| **DR-59 Is Hamiltonian Cycle Witness** | A candidate tour is considered a hamiltonian cycle witness if all of the following hold: the count of tour stops is the required stop count; the count of tour legs is the required stop count; the count of non unique visits is 0; the count of non unique sequences is 0; the count of out of instance stops is 0; the count of non required stops is 0; the count of depot stops is 1; the count of depot position violations is 0; the count of invalid legs is 0; the count of cycle degree violations is 0; and the count of non unique leg orders is 0. |
| **DR-60 Count of Passing Optimality Certificates** | A candidate tour's count of passing optimality certificates is the number of the candidate tour's optimality certificates that are passing. |
| **DR-61 Is Optimality Proved** | A candidate tour is considered optimality-proved if all of the following hold: the hamiltonian cycle witness flag is set and the count of passing optimality certificates is greater than 0. |
| **DR-62 Residual Claim** | The candidate tour's residual claim is determined by the following priority:<br>1. “OPTIMAL_FOR_DECLARED_FINITE_INSTANCE”, if the optimality proved flag is set;<br>2. “VALID_TOUR_NOT_OPTIMALITY_PROOF”, if the hamiltonian cycle witness flag is set;<br>3. in all other cases, “INVALID_TOUR”. |
| **DR-63 Tour TSP Instance** | A tour stop's tour TSP instance — taken from the linked candidate tour. |
| **DR-64 Stop TSP Instance** | A tour stop's stop TSP instance — taken from the linked instance stop. |
| **DR-65 Stop is Required** | A tour stop's stop is required when the linked instance stop is required. |
| **DR-66 Stop is Depot** | A tour stop's stop is depot is true when the tour stop's instance stop is a depot. |
| **DR-67 Is in Tour Instance** | A tour stop is considered in-tour-instance if the tour TSP instance is the stop TSP instance. |
| **DR-68 Visit Multiplicity** | A tour stop's visit multiplicity is the number of tour stops related to the tour stop. |
| **DR-69 Sequence Multiplicity** | A tour stop's sequence multiplicity is the number of tour stops related to the tour stop. |
| **DR-70 Is Unique Visit** | A tour stop is considered a unique visit if the visit multiplicity is 1. |
| **DR-71 Is Unique Sequence** | A tour stop is considered a unique sequence if the sequence multiplicity is 1. |
| **DR-72 Is Depot At First Position** | A tour stop is considered a depot at first position if at least one of the following holds: the stop is depot flag is not set or the sequence position is 1. |
| **DR-73 Count of Outgoing Legs** | A tour stop's count of outgoing legs is the number of tour legs related to the tour stop. |
| **DR-74 Count of Incoming Legs** | A tour stop's count of incoming legs is the number of tour legs related to the tour stop. |
| **DR-75 Has Exactly One Outgoing Leg** | A tour stop is considered to have an exactly one outgoing leg if the count of outgoing legs is 1. |
| **DR-76 Has Exactly One Incoming Leg** | A tour stop is considered to have an exactly one incoming leg if the count of incoming legs is 1. |
| **DR-77 Is Cycle Degree Satisfied** | A tour stop is considered cycle-degree-satisfied if all of the following hold: the exactly one outgoing leg flag is set and the exactly one incoming leg flag is set. |
| **DR-78 Tour TSP Instance** | A tour leg's tour TSP instance — taken from the linked candidate tour. |
| **DR-79 Required Stop Count** | A tour leg's required stop count is the count of required stops of the tour leg's tour TSP instance. |
| **DR-80 From Tour Candidate** | A tour leg's from tour candidate — taken from the linked from tour stop. |
| **DR-81 To Tour Candidate** | A tour leg's to tour candidate — taken from the linked to tour stop. |
| **DR-82 From Instance Stop** | A tour leg's from instance stop — taken from the linked from tour stop. |
| **DR-83 To Instance Stop** | A tour leg's to instance stop — taken from the linked to tour stop. |
| **DR-84 From Sequence Position** | A tour leg's from sequence position — taken from the linked from tour stop. |
| **DR-85 To Sequence Position** | A tour leg's to sequence position — taken from the linked to tour stop. |
| **DR-86 Edge TSP Instance** | A tour leg's edge TSP instance — taken from the linked travel edge. |
| **DR-87 Edge From Stop** | A tour leg's edge from stop — taken from the linked travel edge. |
| **DR-88 Edge to Stop** | A tour leg's edge to stop — taken from the linked travel edge. |
| **DR-89 Edge Travel Cost** | A tour leg's edge travel cost — taken from the linked travel edge. |
| **DR-90 Edge is Admissible** | A tour leg's edge is admissible when the linked travel edge is admissible. |
| **DR-91 Leg Order Multiplicity** | A tour leg's leg order multiplicity is the number of tour legs related to the tour leg. |
| **DR-92 Is Unique Leg Order** | A tour leg is considered a unique leg order if the leg order multiplicity is 1. |
| **DR-93 Is Candidate Membership Valid** | A tour leg is considered candidate-membership-valid if all of the following hold: the candidate tour is the from tour candidate and the candidate tour is the to tour candidate. |
| **DR-94 Is Sequence Transition** | A tour leg is considered a sequence transition if at least one of the following holds: the to sequence position is the from sequence position plus 1 or all of the following hold: the from sequence position is the required stop count and the to sequence position is 1. |
| **DR-95 Is Edge Endpoint Match** | A tour leg is considered an edge endpoint match if at least one of the following holds: all of the following hold: the from instance stop is the edge from stop and the to instance stop is the edge to stop or all of the following hold: the from instance stop is the edge to stop and the to instance stop is the edge from stop. |
| **DR-96 Is in Tour Instance** | A tour leg is considered in-tour-instance if the edge TSP instance is the tour TSP instance. |
| **DR-97 Is Valid** | A tour leg is considered valid if all of the following hold: the candidate membership valid flag is set; the sequence transition flag is set; the edge endpoint match flag is set; the in tour instance flag is set; the edge is admissible flag is set; and the unique leg order flag is set. |
| **DR-98 Travel Cost** | The tour leg's travel cost is determined by the following priority:<br>1. the edge travel cost, if the valid flag is set;<br>2. in all other cases, 0. |
| **DR-99 Stop TSP Instance** | A local degree bound's stop TSP instance — taken from the linked instance stop. |
| **DR-100 Stop Admissible Incident Edge Count** | A local degree bound's stop admissible incident edge count is the count of admissible incident edges of the local degree bound's instance stop. |
| **DR-101 First Edge From Stop** | A local degree bound's first edge from stop — taken from the linked first edge. |
| **DR-102 First Edge to Stop** | A local degree bound's first edge to stop — taken from the linked first edge. |
| **DR-103 Second Edge From Stop** | A local degree bound's second edge from stop — taken from the linked second edge. |
| **DR-104 Second Edge to Stop** | A local degree bound's second edge to stop — taken from the linked second edge. |
| **DR-105 First Edge Cost** | A local degree bound's first edge cost is the travel cost of the local degree bound's first edge. |
| **DR-106 Second Edge Cost** | A local degree bound's second edge cost is the travel cost of the local degree bound's second edge. |
| **DR-107 Is First Edge Incident** | A local degree bound is considered a first edge incident if at least one of the following holds: the instance stop is the first edge from stop or the instance stop is the first edge to stop. |
| **DR-108 Is Second Edge Incident** | A local degree bound is considered a second edge incident if at least one of the following holds: the instance stop is the second edge from stop or the instance stop is the second edge to stop. |
| **DR-109 Are Selected Edges Distinct** | A local degree bound is considered a selected edges distinct if the first edge is not the second edge. |
| **DR-110 Are Selected Costs Ordered** | A local degree bound is considered selected-costs-ordered if the first edge cost is at most the second edge cost. |
| **DR-111 Required Dominance Check Count** | A local degree bound's required dominance check count is computed as the stop admissible incident edge count minus 2. |
| **DR-112 Count of Dominance Checks** | A local degree bound's count of dominance checks is the number of incident dominance checks related to the local degree bound. |
| **DR-113 Count of Failed Dominance Checks** | A local degree bound's count of failed dominance checks is the number of the local degree bound's incident dominance checks that are not passing. |
| **DR-114 Is Two Cheapest Witness** | A local degree bound is considered a two cheapest witness if all of the following hold: the TSP instance is the stop TSP instance; the first edge incident flag is set; the second edge incident flag is set; the are selected edges distinct flag is set; the are selected costs ordered flag is set; the count of dominance checks is the required dominance check count; and the count of failed dominance checks is 0. |
| **DR-115 Local Bound Cost** | The local degree bound's local bound cost is determined by the following priority:<br>1. the first edge cost plus the second edge cost, if the two cheapest witness flag is set;<br>2. in all other cases, 0. |
| **DR-116 Bound Stop** | An incident dominance check's bound stop is the instance stop of the incident dominance check's local degree bound. |
| **DR-117 First Selected Edge** | An incident dominance check's first selected edge — taken from the linked local degree bound. |
| **DR-118 Second Selected Edge** | An incident dominance check's second selected edge — taken from the linked local degree bound. |
| **DR-119 Second Selected Edge Cost** | An incident dominance check's second selected edge cost — taken from the linked local degree bound. |
| **DR-120 Other Edge From Stop** | An incident dominance check's other edge from stop — taken from the linked other edge. |
| **DR-121 Other Edge to Stop** | An incident dominance check's other edge to stop — taken from the linked other edge. |
| **DR-122 Other Edge Cost** | An incident dominance check's other edge cost is the travel cost of the incident dominance check's other edge. |
| **DR-123 Other Edge Multiplicity** | An incident dominance check's other edge multiplicity is the number of incident dominance checks related to the incident dominance check. |
| **DR-124 Is Other Edge Incident** | An incident dominance check is considered an other edge incident if at least one of the following holds: the bound stop is the other edge from stop or the bound stop is the other edge to stop. |
| **DR-125 Is Other Edge Unselected** | An incident dominance check is considered other-edge-unselected if all of the following hold: the other edge is not the first selected edge and the other edge is not the second selected edge. |
| **DR-126 Is Other Edge Unique** | An incident dominance check is considered an other edge unique if the other edge multiplicity is 1. |
| **DR-127 Is Dominated by Second Cost** | An incident dominance check is considered a dominated by second cost if the other edge cost is at least the second selected edge cost. |
| **DR-128 Is Passing** | An incident dominance check is considered passing if all of the following hold: the other edge incident flag is set; the other edge unselected flag is set; the other edge unique flag is set; and the dominated by second cost flag is set. |
| **DR-129 Required Stop Count** | An instance lower bound's required stop count is the count of required stops of the instance lower bound's TSP instance. |
| **DR-130 Count of Local Degree Bounds** | An instance lower bound's count of local degree bounds is the number of local degree bounds related to the instance lower bound. |
| **DR-131 Count of Invalid Local Degree Bounds** | An instance lower bound's count of invalid local degree bounds is the number of the instance lower bound's local degree bounds that are not two cheapest witnesses. |
| **DR-132 Total Local Degree Bound Cost** | An instance lower bound's total local degree bound cost is the total local bound cost across the local degree bounds related to the instance lower bound. |
| **DR-133 Lower Bound Cost** | An instance lower bound's lower bound cost is computed as the base lower bound cost plus the supplemental bound adjustment. |
| **DR-134 Is Certified** | An instance lower bound is considered certified if all of the following hold: the count of local degree bounds is the required stop count; the count of invalid local degree bounds is 0; and the supplemental bound certified flag is set. |
| **DR-135 Base Lower Bound Cost** | An instance lower bound's base lower bound cost is computed as the total local degree bound cost divided by 2. |
| **DR-136 Count of Certified Supplemental Terms** | An instance lower bound's count of certified supplemental terms is the number of the instance lower bound's TSP bound terms that count a toward adjustment and are certified. |
| **DR-137 Is Supplemental Bound Certified** | An instance lower bound is considered supplemental-bound-certified if the count of certified supplemental terms is the required supplemental term count. |
| **DR-138 Term Value** | A TSP bound term's term value is computed as the quantity times the unit weight times the sign. |
| **DR-139 Candidate TSP Instance** | An optimality certificate's candidate TSP instance — taken from the linked candidate tour. |
| **DR-140 Lower Bound TSP Instance** | An optimality certificate's lower bound TSP instance — taken from the linked instance lower bound. |
| **DR-141 Candidate Travel Cost** | An optimality certificate's candidate travel cost is the total travel cost of the optimality certificate's candidate tour. |
| **DR-142 Lower Bound Cost** | An optimality certificate's lower bound cost — taken from the linked instance lower bound. |
| **DR-143 Candidate is Hamiltonian Cycle Witness** | An optimality certificate's candidate is hamiltonian cycle witness when the linked candidate tour is a hamiltonian cycle witness. |
| **DR-144 Lower Bound is Certified** | An optimality certificate's lower bound is certified when the linked instance lower bound is certified. |
| **DR-145 Is Same Instance** | An optimality certificate is considered a same instance if the candidate TSP instance is the lower bound TSP instance. |
| **DR-146 Is Bound Tight** | An optimality certificate is considered a bound tight if the candidate travel cost is the lower bound cost. |
| **DR-147 Is Passing** | An optimality certificate is considered passing if all of the following hold: the candidate is hamiltonian cycle witness flag is set; the lower bound is certified flag is set; the same instance flag is set; and the bound tight flag is set. |
| **DR-148 Scope Claim** | The optimality certificate's scope claim is determined by the following priority:<br>1. “OPTIMAL_FOR_DECLARED_FINITE_INSTANCE”, if the passing flag is set;<br>2. in all other cases, “OPTIMALITY_NOT_CERTIFIED”. |
| **DR-149 Is Successful** | A TSP execution run is considered a successful if all of the following hold: the build succeeded flag is set; the database initialized flag is set; and the conformance succeeded flag is set. |
| **DR-150 Is Passing** | A TSP conformance check is considered passing if the status is “PASS”. |
| **DR-151 Is Decided** | A TSP edge state is considered decided if it is not the case that the decision status is “UNKNOWN”. |
| **DR-152 Commitment Rank** | The TSP edge state's commitment rank is determined by the following priority:<br>1. 0, if the decision status is “UNKNOWN”;<br>2. 1, if the decision status is “SELECTED”;<br>3. 2, if at least one of the following holds: the decision status is “FORCED” or the decision status is “FORBIDDEN”;<br>4. in all other cases, 3. |
| **DR-153 Commitment Polarity** | The TSP edge state's commitment polarity is determined by the following priority:<br>1. the negative of 1, if the decision status is “FORBIDDEN”;<br>2. 0, if the decision status is “UNKNOWN”;<br>3. in all other cases, 1. |
| **DR-154 Necessity Scope** | The TSP edge state's necessity scope is determined by the following priority:<br>1. “ALL_FEASIBLE_TOURS”, if the decision status is “FORCED”;<br>2. “NO_FEASIBLE_TOUR”, if the decision status is “FORBIDDEN”;<br>3. “CURRENT_WITNESS”, if the decision status is “SELECTED”;<br>4. in all other cases, “UNRESOLVED_OR_HISTORICAL”. |
| **DR-155 Is Terminal Commitment** | A TSP edge state is considered a terminal commitment if at least one of the following holds: the decision status is “FORCED”; the decision status is “FORBIDDEN”; the decision status is “CONTRADICTED”; or the decision status is “SUPERSEDED”. |
| **DR-156 Is Connected Degree Two** | A TSP derived edge set is considered a connected degree two if all of the following hold: the degree violation count is 0; the connected component count is 1; and the proper subtour count is 0. |
| **DR-157 Incidence Defect** | A TSP defect profile's incidence defect is computed as the absolute value of the required incidence minus the observed incidence. |
| **DR-158 Connectivity Defect** | A TSP defect profile's connectivity defect is computed as the largest of the component count minus 1 and 0. |
| **DR-159 Boundary Defect** | A TSP defect profile's boundary defect is computed as the largest of the required boundary crossings minus the observed boundary crossings and 0. |
| **DR-160 Cost Gap** | A TSP defect profile's cost gap is computed as the upper bound cost minus the lower bound cost. |
| **DR-161 Defect Vector** | A TSP defect profile's defect vector is computed as the incidence defect, followed by “|”, followed by the connectivity defect, followed by “|”, followed by the boundary defect, followed by “|”, followed by the cost gap. |
| **DR-162 Is Degree Two** | A TSP edge set stop degree is considered a degree two if the selected degree is 2. |
| **DR-163 Is Passing** | A TSP connected degree two certificate is considered passing if all of the following hold: the edge count is the required stop count; the degree violation count is 0; the component count is 1; the proper subtour count is 0; and the spanning tree edge count is the required stop count minus 1. |
| **DR-164 Is Passing** | A TSP route reconstruction is considered passing if all of the following hold: the reconstructed stop count is the required stop count; the reconstructed leg count is the required stop count; the candidate used as antecedent flag is not set; and the status is “RECONSTRUCTED”. |
| **DR-165 Semantic Key** | A TSP witness normal form's semantic key is computed as the witness shape, followed by “|”, followed by the TSP instance, followed by “|”, followed by the covered stop count, followed by “|”, followed by the required stop count, followed by “|”, followed by the edge count, followed by “|”, followed by the total cost, followed by “|”, followed by the incidence defect, followed by “|”, followed by the connectivity defect, followed by “|”, followed by the order defect, followed by “|”, followed by the boundary signature (a missing value counts as an empty string). ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-166 Is Valid** | A TSP witness normal form is considered valid if all of the following hold: the covered stop count is the required stop count; the incidence defect is 0; the connectivity defect is 0; the order defect is 0; and at least one of the following holds: all of the following hold: the witness shape is “CYCLE” and the edge count is the required stop count or all of the following hold: the witness shape is “PATH” and the edge count is the required stop count minus 1. |
| **DR-167 Route Class Elimination Pct** | The TSP search certificate's route class elimination pct is determined by the following priority:<br>1. 0, if the initial route class count is 0;<br>2. in all other cases, the initial route class count minus the surviving route class count divided by the initial route class count times 100 rounded to 2 decimal place(s). |
| **DR-168 Boundary Signature** | A TSP cluster boundary state's boundary signature is computed as the neighborhood, followed by “|”, followed by the entry stop, followed by “|”, followed by the exit stop, followed by “|”, followed by the internal stop count, followed by “|”, followed by the internal path cost. |
| **DR-169 Semantic Quotient Key** | A TSP cluster boundary state's semantic quotient key is computed as the neighborhood, followed by “|”, followed by the entry stop, followed by “|”, followed by the exit stop, followed by “|”, followed by the internal stop count, followed by “|”, followed by the internal path cost. |
| **DR-170 Is Fiber Minimum** | A TSP cluster boundary state is considered a fiber minimum if all of the following hold: the quotient representative flag is set and the dominated flag is not set. |
| **DR-171 Name** | A TSP inference rule's name is the same as its display name. |
| **DR-172 Name** | A TSP frontier obligation's name is the same as its display name. |
| **DR-173 Is Imported Dependency** | A TSP frontier obligation is considered an imported dependency if the obligation kind is “IMPORTED_DEPENDENCY”. |
| **DR-174 Is Closed** | A TSP frontier obligation is considered closed if the status is “CLOSED”. |
| **DR-175 Name** | A TSP loop's name is the same as its display name. |
| **DR-176 Semantic Compression Pct** | The TSP convergence measurement's semantic compression pct is determined by the following priority:<br>1. 0, if the surface concept count before is 0;<br>2. in all other cases, the surface concept count before minus the primitive count after divided by the surface concept count before times 100 rounded to 2 decimal place(s). |
| **DR-177 Is Current Basis Member** | A TSP concept registry is considered a current basis member if at least one of the following holds: the status is “ACTIVE_PRIMITIVE” or the status is “ACTIVE_OPERATOR”. |
| **DR-178 Search Elimination Pct** | The search metric's search elimination pct is determined by the following priority:<br>1. 0, if the branch count before is 0;<br>2. in all other cases, the branch count before minus the branch count after divided by the branch count before times 100 rounded to 2 decimal place(s). |
| **DR-179 Name** | A TSP invariant check's name is the same as its display name. |
| **DR-180 Actual Hamiltonian Cycle Witness** | A TSP invariant check's actual hamiltonian cycle witness is true when the TSP invariant check's candidate tour is a hamiltonian cycle witness. |
| **DR-181 Actual Optimality Proved** | A TSP invariant check's actual optimality proved when the linked candidate tour is optimality proved. |
| **DR-182 Actual Total Travel Cost** | A TSP invariant check's actual total travel cost — taken from the linked candidate tour. |
| **DR-183 Is Hamiltonian Status Correct** | A TSP invariant check is considered a hamiltonian status correct if the expected hamiltonian cycle witness is the actual hamiltonian cycle witness. |
| **DR-184 Is Optimality Status Correct** | A TSP invariant check is considered an optimality status correct if the expected optimality proved is the actual optimality proved. |
| **DR-185 Is Cost Correct** | A TSP invariant check is considered a cost correct if the expected total travel cost is the actual total travel cost. |
| **DR-186 Is Passing** | A TSP invariant check is considered passing if all of the following hold: the hamiltonian status correct flag is set; the optimality status correct flag is set; and the cost correct flag is set. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Cities.Name** | formula | `DisplayName` |
| **Neighborhoods.Name** | formula | `DisplayName` |
| **Neighborhoods.CityName** | lookup | `Lookup(Cities.Name via City)` |
| **Addresses.Name** | formula | `StreetLabel` |
| **Addresses.NeighborhoodName** | lookup | `Lookup(Neighborhoods.Name via Neighborhood)` |
| **Addresses.City** | lookup | `Lookup(Neighborhoods.City via Neighborhood)` |
| **TSPInstances.Name** | formula | `DisplayName` |
| **TSPInstances.CountOfStops** | rollup | `Count(InstanceStops via TSPInstance)` |
| **TSPInstances.CountOfRequiredStops** | rollup | `Count(InstanceStops via TSPInstance)` |
| **TSPInstances.CountOfTravelEdges** | rollup | `Count(TravelEdges via TSPInstance)` |
| **TSPInstances.CountOfInadmissibleEdges** | rollup | `Count(TravelEdges via TSPInstance)` |
| **TSPInstances.CountOfNonUniqueEdgePairRows** | rollup | `Count(TravelEdges via TSPInstance)` |
| **TSPInstances.ExpectedUndirectedEdgeCount** | formula | `CountOfStops * CountOfStops - 1 / 2` |
| **TSPInstances.IsCompleteUndirectedGraph** | formula | `And(CountOfTravelEdges = ExpectedUndirectedEdgeCount, CountOfInadmissibleEdges = 0, CountOfNonUniqueEdgePairRows = 0)` |
| **TSPGraphInvariantChecks.Name** | formula | `DisplayName` |
| **TSPGraphInvariantChecks.ActualCompleteUndirectedGraph** | lookup | `Lookup(TSPInstances.IsCompleteUndirectedGraph via TSPInstance)` |
| **TSPGraphInvariantChecks.ActualTravelEdgeCount** | lookup | `Lookup(TSPInstances.CountOfTravelEdges via TSPInstance)` |
| **TSPGraphInvariantChecks.ActualNonUniqueEdgePairRows** | lookup | `Lookup(TSPInstances.CountOfNonUniqueEdgePairRows via TSPInstance)` |
| **TSPGraphInvariantChecks.IsPassing** | formula | `And(ExpectedCompleteUndirectedGraph = ActualCompleteUndirectedGraph, ExpectedTravelEdgeCount = ActualTravelEdgeCount, ExpectedNonUniqueEdgePairRows = ActualNonUniqueEdgePairRows)` |
| **InstanceStops.InstanceCity** | lookup | `Lookup(TSPInstances.City via TSPInstance)` |
| **InstanceStops.InstanceDepotAddress** | lookup | `Lookup(TSPInstances.DepotAddress via TSPInstance)` |
| **InstanceStops.AddressName** | lookup | `Lookup(Addresses.Name via Address)` |
| **InstanceStops.AddressCity** | lookup | `Lookup(Addresses.City via Address)` |
| **InstanceStops.Neighborhood** | lookup | `Lookup(Addresses.Neighborhood via Address)` |
| **InstanceStops.XCoordinate** | lookup | `Lookup(Addresses.XCoordinate via Address)` |
| **InstanceStops.YCoordinate** | lookup | `Lookup(Addresses.YCoordinate via Address)` |
| **InstanceStops.IsWithinInstanceCity** | formula | `AddressCity = InstanceCity` |
| **InstanceStops.IsDepot** | formula | `Address = InstanceDepotAddress` |
| **InstanceStops.CountOfCanonicalEdgesFrom** | rollup | `Count(TravelEdges via TSPInstance)` |
| **InstanceStops.CountOfCanonicalEdgesTo** | rollup | `Count(TravelEdges via TSPInstance)` |
| **InstanceStops.CountOfIncidentEdges** | formula | `CountOfCanonicalEdgesFrom + CountOfCanonicalEdgesTo` |
| **InstanceStops.CountOfAdmissibleEdgesFrom** | rollup | `Count(TravelEdges via TSPInstance)` |
| **InstanceStops.CountOfAdmissibleEdgesTo** | rollup | `Count(TravelEdges via TSPInstance)` |
| **InstanceStops.CountOfAdmissibleIncidentEdges** | formula | `CountOfAdmissibleEdgesFrom + CountOfAdmissibleEdgesTo` |
| **TravelEdges.ExpectedCanonicalPairKey** | formula | `If(FromStop < ToStop, Concat(FromStop, "\|", ToStop), Concat(ToStop, "\|", FromStop))` |
| **TravelEdges.IsCanonicalPairKeyCorrect** | formula | `CanonicalPairKey = ExpectedCanonicalPairKey` |
| **TravelEdges.IsCanonicalEndpointOrder** | formula | `FromStop < ToStop` |
| **TravelEdges.PairMultiplicity** | rollup | `Count(TravelEdges via TSPInstance)` |
| **TravelEdges.IsUniqueCanonicalPair** | formula | `PairMultiplicity = 1` |
| **TravelEdges.FromTSPInstance** | lookup | `Lookup(InstanceStops.TSPInstance via FromStop)` |
| **TravelEdges.ToTSPInstance** | lookup | `Lookup(InstanceStops.TSPInstance via ToStop)` |
| **TravelEdges.IsSelfLoop** | formula | `FromStop = ToStop` |
| **TravelEdges.IsWithinInstance** | formula | `And(TSPInstance = FromTSPInstance, TSPInstance = ToTSPInstance)` |
| **TravelEdges.IsAdmissible** | formula | `And(IsAvailable, Not(IsSelfLoop), IsWithinInstance, IsCanonicalEndpointOrder, IsCanonicalPairKeyCorrect, IsUniqueCanonicalPair)` |
| **CandidateTours.Name** | formula | `DisplayName` |
| **CandidateTours.RequiredStopCount** | lookup | `Lookup(TSPInstances.CountOfRequiredStops via TSPInstance)` |
| **CandidateTours.CountOfTourStops** | rollup | `Count(TourStops via CandidateTour)` |
| **CandidateTours.CountOfTourLegs** | rollup | `Count(TourLegs via CandidateTour)` |
| **CandidateTours.CountOfNonUniqueVisits** | rollup | `Count(TourStops via CandidateTour)` |
| **CandidateTours.CountOfNonUniqueSequences** | rollup | `Count(TourStops via CandidateTour)` |
| **CandidateTours.CountOfOutOfInstanceStops** | rollup | `Count(TourStops via CandidateTour)` |
| **CandidateTours.CountOfNonRequiredStops** | rollup | `Count(TourStops via CandidateTour)` |
| **CandidateTours.CountOfDepotStops** | rollup | `Count(TourStops via CandidateTour)` |
| **CandidateTours.CountOfDepotPositionViolations** | rollup | `Count(TourStops via CandidateTour)` |
| **CandidateTours.CountOfInvalidLegs** | rollup | `Count(TourLegs via CandidateTour)` |
| **CandidateTours.CountOfCycleDegreeViolations** | rollup | `Count(TourStops via CandidateTour)` |
| **CandidateTours.CountOfNonUniqueLegOrders** | rollup | `Count(TourLegs via CandidateTour)` |
| **CandidateTours.TotalTravelCost** | rollup | `Sum(TourLegs.TravelCost via CandidateTour)` |
| **CandidateTours.IsHamiltonianCycleWitness** | formula | `And(CountOfTourStops = RequiredStopCount, CountOfTourLegs = RequiredStopCount, CountOfNonUniqueVisits = 0, CountOfNonUniqueSequences = 0, CountOfOutOfInstanceStops = 0, CountOfNonRequiredStops = 0, CountOfDepotStops = 1, CountOfDepotPositionViolations = 0, CountOfInvalidLegs = 0, CountOfCycleDegreeViolations = 0, CountOfNonUniqueLegOrders = 0)` |
| **CandidateTours.CountOfPassingOptimalityCertificates** | rollup | `Count(OptimalityCertificates via CandidateTour)` |
| **CandidateTours.IsOptimalityProved** | formula | `And(IsHamiltonianCycleWitness, CountOfPassingOptimalityCertificates > 0)` |
| **CandidateTours.ResidualClaim** | formula | `If(IsOptimalityProved, "OPTIMAL_FOR_DECLARED_FINITE_INSTANCE", If(IsHamiltonianCycleWitness, "VALID_TOUR_NOT_OPTIMALITY_PROOF", "INVALID_TOUR"))` |
| **TourStops.TourTSPInstance** | lookup | `Lookup(CandidateTours.TSPInstance via CandidateTour)` |
| **TourStops.StopTSPInstance** | lookup | `Lookup(InstanceStops.TSPInstance via InstanceStop)` |
| **TourStops.StopIsRequired** | lookup | `Lookup(InstanceStops.IsRequired via InstanceStop)` |
| **TourStops.StopIsDepot** | lookup | `Lookup(InstanceStops.IsDepot via InstanceStop)` |
| **TourStops.IsInTourInstance** | formula | `TourTSPInstance = StopTSPInstance` |
| **TourStops.VisitMultiplicity** | rollup | `Count(TourStops via CandidateTour)` |
| **TourStops.SequenceMultiplicity** | rollup | `Count(TourStops via CandidateTour)` |
| **TourStops.IsUniqueVisit** | formula | `VisitMultiplicity = 1` |
| **TourStops.IsUniqueSequence** | formula | `SequenceMultiplicity = 1` |
| **TourStops.IsDepotAtFirstPosition** | formula | `Or(Not(StopIsDepot), SequencePosition = 1)` |
| **TourStops.CountOfOutgoingLegs** | rollup | `Count(TourLegs via CandidateTour)` |
| **TourStops.CountOfIncomingLegs** | rollup | `Count(TourLegs via CandidateTour)` |
| **TourStops.HasExactlyOneOutgoingLeg** | formula | `CountOfOutgoingLegs = 1` |
| **TourStops.HasExactlyOneIncomingLeg** | formula | `CountOfIncomingLegs = 1` |
| **TourStops.IsCycleDegreeSatisfied** | formula | `And(HasExactlyOneOutgoingLeg, HasExactlyOneIncomingLeg)` |
| **TourLegs.TourTSPInstance** | lookup | `Lookup(CandidateTours.TSPInstance via CandidateTour)` |
| **TourLegs.RequiredStopCount** | lookup | `Lookup(TSPInstances.CountOfRequiredStops via TourTSPInstance)` |
| **TourLegs.FromTourCandidate** | lookup | `Lookup(TourStops.CandidateTour via FromTourStop)` |
| **TourLegs.ToTourCandidate** | lookup | `Lookup(TourStops.CandidateTour via ToTourStop)` |
| **TourLegs.FromInstanceStop** | lookup | `Lookup(TourStops.InstanceStop via FromTourStop)` |
| **TourLegs.ToInstanceStop** | lookup | `Lookup(TourStops.InstanceStop via ToTourStop)` |
| **TourLegs.FromSequencePosition** | lookup | `Lookup(TourStops.SequencePosition via FromTourStop)` |
| **TourLegs.ToSequencePosition** | lookup | `Lookup(TourStops.SequencePosition via ToTourStop)` |
| **TourLegs.EdgeTSPInstance** | lookup | `Lookup(TravelEdges.TSPInstance via TravelEdge)` |
| **TourLegs.EdgeFromStop** | lookup | `Lookup(TravelEdges.FromStop via TravelEdge)` |
| **TourLegs.EdgeToStop** | lookup | `Lookup(TravelEdges.ToStop via TravelEdge)` |
| **TourLegs.EdgeTravelCost** | lookup | `Lookup(TravelEdges.TravelCost via TravelEdge)` |
| **TourLegs.EdgeIsAdmissible** | lookup | `Lookup(TravelEdges.IsAdmissible via TravelEdge)` |
| **TourLegs.LegOrderMultiplicity** | rollup | `Count(TourLegs via CandidateTour)` |
| **TourLegs.IsUniqueLegOrder** | formula | `LegOrderMultiplicity = 1` |
| **TourLegs.IsCandidateMembershipValid** | formula | `And(CandidateTour = FromTourCandidate, CandidateTour = ToTourCandidate)` |
| **TourLegs.IsSequenceTransition** | formula | `Or(ToSequencePosition = FromSequencePosition + 1, And(FromSequencePosition = RequiredStopCount, ToSequencePosition = 1))` |
| **TourLegs.IsEdgeEndpointMatch** | formula | `Or(And(FromInstanceStop = EdgeFromStop, ToInstanceStop = EdgeToStop), And(FromInstanceStop = EdgeToStop, ToInstanceStop = EdgeFromStop))` |
| **TourLegs.IsInTourInstance** | formula | `EdgeTSPInstance = TourTSPInstance` |
| **TourLegs.IsValid** | formula | `And(IsCandidateMembershipValid, IsSequenceTransition, IsEdgeEndpointMatch, IsInTourInstance, EdgeIsAdmissible, IsUniqueLegOrder)` |
| **TourLegs.TravelCost** | formula | `If(IsValid, EdgeTravelCost, 0)` |
| **LocalDegreeBounds.StopTSPInstance** | lookup | `Lookup(InstanceStops.TSPInstance via InstanceStop)` |
| **LocalDegreeBounds.StopAdmissibleIncidentEdgeCount** | lookup | `Lookup(InstanceStops.CountOfAdmissibleIncidentEdges via InstanceStop)` |
| **LocalDegreeBounds.FirstEdgeFromStop** | lookup | `Lookup(TravelEdges.FromStop via FirstEdge)` |
| **LocalDegreeBounds.FirstEdgeToStop** | lookup | `Lookup(TravelEdges.ToStop via FirstEdge)` |
| **LocalDegreeBounds.SecondEdgeFromStop** | lookup | `Lookup(TravelEdges.FromStop via SecondEdge)` |
| **LocalDegreeBounds.SecondEdgeToStop** | lookup | `Lookup(TravelEdges.ToStop via SecondEdge)` |
| **LocalDegreeBounds.FirstEdgeCost** | lookup | `Lookup(TravelEdges.TravelCost via FirstEdge)` |
| **LocalDegreeBounds.SecondEdgeCost** | lookup | `Lookup(TravelEdges.TravelCost via SecondEdge)` |
| **LocalDegreeBounds.IsFirstEdgeIncident** | formula | `Or(InstanceStop = FirstEdgeFromStop, InstanceStop = FirstEdgeToStop)` |
| **LocalDegreeBounds.IsSecondEdgeIncident** | formula | `Or(InstanceStop = SecondEdgeFromStop, InstanceStop = SecondEdgeToStop)` |
| **LocalDegreeBounds.AreSelectedEdgesDistinct** | formula | `FirstEdge <> SecondEdge` |
| **LocalDegreeBounds.AreSelectedCostsOrdered** | formula | `FirstEdgeCost <= SecondEdgeCost` |
| **LocalDegreeBounds.RequiredDominanceCheckCount** | formula | `StopAdmissibleIncidentEdgeCount - 2` |
| **LocalDegreeBounds.CountOfDominanceChecks** | rollup | `Count(IncidentDominanceChecks via LocalDegreeBound)` |
| **LocalDegreeBounds.CountOfFailedDominanceChecks** | rollup | `Count(IncidentDominanceChecks via LocalDegreeBound)` |
| **LocalDegreeBounds.IsTwoCheapestWitness** | formula | `And(TSPInstance = StopTSPInstance, IsFirstEdgeIncident, IsSecondEdgeIncident, AreSelectedEdgesDistinct, AreSelectedCostsOrdered, CountOfDominanceChecks = RequiredDominanceCheckCount, CountOfFailedDominanceChecks = 0)` |
| **LocalDegreeBounds.LocalBoundCost** | formula | `If(IsTwoCheapestWitness, FirstEdgeCost + SecondEdgeCost, 0)` |
| **IncidentDominanceChecks.BoundStop** | lookup | `Lookup(LocalDegreeBounds.InstanceStop via LocalDegreeBound)` |
| **IncidentDominanceChecks.FirstSelectedEdge** | lookup | `Lookup(LocalDegreeBounds.FirstEdge via LocalDegreeBound)` |
| **IncidentDominanceChecks.SecondSelectedEdge** | lookup | `Lookup(LocalDegreeBounds.SecondEdge via LocalDegreeBound)` |
| **IncidentDominanceChecks.SecondSelectedEdgeCost** | lookup | `Lookup(LocalDegreeBounds.SecondEdgeCost via LocalDegreeBound)` |
| **IncidentDominanceChecks.OtherEdgeFromStop** | lookup | `Lookup(TravelEdges.FromStop via OtherEdge)` |
| **IncidentDominanceChecks.OtherEdgeToStop** | lookup | `Lookup(TravelEdges.ToStop via OtherEdge)` |
| **IncidentDominanceChecks.OtherEdgeCost** | lookup | `Lookup(TravelEdges.TravelCost via OtherEdge)` |
| **IncidentDominanceChecks.OtherEdgeMultiplicity** | rollup | `Count(IncidentDominanceChecks via LocalDegreeBound)` |
| **IncidentDominanceChecks.IsOtherEdgeIncident** | formula | `Or(BoundStop = OtherEdgeFromStop, BoundStop = OtherEdgeToStop)` |
| **IncidentDominanceChecks.IsOtherEdgeUnselected** | formula | `And(OtherEdge <> FirstSelectedEdge, OtherEdge <> SecondSelectedEdge)` |
| **IncidentDominanceChecks.IsOtherEdgeUnique** | formula | `OtherEdgeMultiplicity = 1` |
| **IncidentDominanceChecks.IsDominatedBySecondCost** | formula | `OtherEdgeCost >= SecondSelectedEdgeCost` |
| **IncidentDominanceChecks.IsPassing** | formula | `And(IsOtherEdgeIncident, IsOtherEdgeUnselected, IsOtherEdgeUnique, IsDominatedBySecondCost)` |
| **InstanceLowerBounds.RequiredStopCount** | lookup | `Lookup(TSPInstances.CountOfRequiredStops via TSPInstance)` |
| **InstanceLowerBounds.CountOfLocalDegreeBounds** | rollup | `Count(LocalDegreeBounds via TSPInstance)` |
| **InstanceLowerBounds.CountOfInvalidLocalDegreeBounds** | rollup | `Count(LocalDegreeBounds via TSPInstance)` |
| **InstanceLowerBounds.TotalLocalDegreeBoundCost** | rollup | `Sum(LocalDegreeBounds.LocalBoundCost via TSPInstance)` |
| **InstanceLowerBounds.LowerBoundCost** | formula | `BaseLowerBoundCost + SupplementalBoundAdjustment` |
| **InstanceLowerBounds.IsCertified** | formula | `And(CountOfLocalDegreeBounds = RequiredStopCount, CountOfInvalidLocalDegreeBounds = 0, IsSupplementalBoundCertified)` |
| **InstanceLowerBounds.BaseLowerBoundCost** | formula | `TotalLocalDegreeBoundCost / 2` |
| **InstanceLowerBounds.CountOfCertifiedSupplementalTerms** | rollup | `Count(TSPBoundTerms via BoundCertificate)` |
| **InstanceLowerBounds.IsSupplementalBoundCertified** | formula | `CountOfCertifiedSupplementalTerms = RequiredSupplementalTermCount` |
| **TSPBoundTerms.TermValue** | formula | `Quantity * UnitWeight * Sign` |
| **OptimalityCertificates.CandidateTSPInstance** | lookup | `Lookup(CandidateTours.TSPInstance via CandidateTour)` |
| **OptimalityCertificates.LowerBoundTSPInstance** | lookup | `Lookup(InstanceLowerBounds.TSPInstance via InstanceLowerBound)` |
| **OptimalityCertificates.CandidateTravelCost** | lookup | `Lookup(CandidateTours.TotalTravelCost via CandidateTour)` |
| **OptimalityCertificates.LowerBoundCost** | lookup | `Lookup(InstanceLowerBounds.LowerBoundCost via InstanceLowerBound)` |
| **OptimalityCertificates.CandidateIsHamiltonianCycleWitness** | lookup | `Lookup(CandidateTours.IsHamiltonianCycleWitness via CandidateTour)` |
| **OptimalityCertificates.LowerBoundIsCertified** | lookup | `Lookup(InstanceLowerBounds.IsCertified via InstanceLowerBound)` |
| **OptimalityCertificates.IsSameInstance** | formula | `CandidateTSPInstance = LowerBoundTSPInstance` |
| **OptimalityCertificates.IsBoundTight** | formula | `CandidateTravelCost = LowerBoundCost` |
| **OptimalityCertificates.IsPassing** | formula | `And(CandidateIsHamiltonianCycleWitness, LowerBoundIsCertified, IsSameInstance, IsBoundTight)` |
| **OptimalityCertificates.ScopeClaim** | formula | `If(IsPassing, "OPTIMAL_FOR_DECLARED_FINITE_INSTANCE", "OPTIMALITY_NOT_CERTIFIED")` |
| **TSPExecutionRuns.IsSuccessful** | formula | `And(BuildSucceeded, DatabaseInitialized, ConformanceSucceeded)` |
| **TSPConformanceChecks.IsPassing** | formula | `Status = "PASS"` |
| **TSPEdgeStates.IsDecided** | formula | `Not(DecisionStatus = "UNKNOWN")` |
| **TSPEdgeStates.CommitmentRank** | formula | `If(DecisionStatus = "UNKNOWN", 0, If(DecisionStatus = "SELECTED", 1, If(Or(DecisionStatus = "FORCED", DecisionStatus = "FORBIDDEN"), 2, 3)))` |
| **TSPEdgeStates.CommitmentPolarity** | formula | `If(DecisionStatus = "FORBIDDEN", -1, If(DecisionStatus = "UNKNOWN", 0, 1))` |
| **TSPEdgeStates.NecessityScope** | formula | `If(DecisionStatus = "FORCED", "ALL_FEASIBLE_TOURS", If(DecisionStatus = "FORBIDDEN", "NO_FEASIBLE_TOUR", If(DecisionStatus = "SELECTED", "CURRENT_WITNESS", "UNRESOLVED_OR_HISTORICAL")))` |
| **TSPEdgeStates.IsTerminalCommitment** | formula | `Or(DecisionStatus = "FORCED", DecisionStatus = "FORBIDDEN", DecisionStatus = "CONTRADICTED", DecisionStatus = "SUPERSEDED")` |
| **TSPDerivedEdgeSets.IsConnectedDegreeTwo** | formula | `And(DegreeViolationCount = 0, ConnectedComponentCount = 1, ProperSubtourCount = 0)` |
| **TSPDefectProfiles.IncidenceDefect** | formula | `Abs(RequiredIncidence - ObservedIncidence)` |
| **TSPDefectProfiles.ConnectivityDefect** | formula | `Max(ComponentCount - 1, 0)` |
| **TSPDefectProfiles.BoundaryDefect** | formula | `Max(RequiredBoundaryCrossings - ObservedBoundaryCrossings, 0)` |
| **TSPDefectProfiles.CostGap** | formula | `UpperBoundCost - LowerBoundCost` |
| **TSPDefectProfiles.DefectVector** | formula | `Concat(IncidenceDefect, "\|", ConnectivityDefect, "\|", BoundaryDefect, "\|", CostGap)` |
| **TSPEdgeSetStopDegrees.IsDegreeTwo** | formula | `SelectedDegree = 2` |
| **TSPConnectedDegreeTwoCertificates.IsPassing** | formula | `And(EdgeCount = RequiredStopCount, DegreeViolationCount = 0, ComponentCount = 1, ProperSubtourCount = 0, SpanningTreeEdgeCount = RequiredStopCount - 1)` |
| **TSPRouteReconstructions.IsPassing** | formula | `And(ReconstructedStopCount = RequiredStopCount, ReconstructedLegCount = RequiredStopCount, Not(CandidateUsedAsAntecedent), Status = "RECONSTRUCTED")` |
| **TSPWitnessNormalForms.SemanticKey** | formula | `Concat(WitnessShape, "\|", TSPInstance, "\|", CoveredStopCount, "\|", RequiredStopCount, "\|", EdgeCount, "\|", TotalCost, "\|", IncidenceDefect, "\|", ConnectivityDefect, "\|", OrderDefect, "\|", Coalesce(BoundarySignature, ""))` |
| **TSPWitnessNormalForms.IsValid** | formula | `And(CoveredStopCount = RequiredStopCount, IncidenceDefect = 0, ConnectivityDefect = 0, OrderDefect = 0, Or(And(WitnessShape = "CYCLE", EdgeCount = RequiredStopCount), And(WitnessShape = "PATH", EdgeCount = RequiredStopCount - 1)))` |
| **TSPSearchCertificates.RouteClassEliminationPct** | formula | `If(InitialRouteClassCount = 0, 0, Round(InitialRouteClassCount - SurvivingRouteClassCount / InitialRouteClassCount * 100, 2))` |
| **TSPClusterBoundaryStates.BoundarySignature** | formula | `Concat(Neighborhood, "\|", EntryStop, "\|", ExitStop, "\|", InternalStopCount, "\|", InternalPathCost)` |
| **TSPClusterBoundaryStates.SemanticQuotientKey** | formula | `Concat(Neighborhood, "\|", EntryStop, "\|", ExitStop, "\|", InternalStopCount, "\|", InternalPathCost)` |
| **TSPClusterBoundaryStates.IsFiberMinimum** | formula | `And(IsQuotientRepresentative, Not(IsDominated))` |
| **TSPInferenceRules.Name** | formula | `DisplayName` |
| **TSPFrontierObligations.Name** | formula | `DisplayName` |
| **TSPFrontierObligations.IsImportedDependency** | formula | `ObligationKind = "IMPORTED_DEPENDENCY"` |
| **TSPFrontierObligations.IsClosed** | formula | `Status = "CLOSED"` |
| **TSPLoops.Name** | formula | `DisplayName` |
| **TSPConvergenceMeasurements.SemanticCompressionPct** | formula | `If(SurfaceConceptCountBefore = 0, 0, Round(SurfaceConceptCountBefore - PrimitiveCountAfter / SurfaceConceptCountBefore * 100, 2))` |
| **TSPConceptRegistry.IsCurrentBasisMember** | formula | `Or(Status = "ACTIVE_PRIMITIVE", Status = "ACTIVE_OPERATOR")` |
| **SearchMetrics.SearchEliminationPct** | formula | `If(BranchCountBefore = 0, 0, Round(BranchCountBefore - BranchCountAfter / BranchCountBefore * 100, 2))` |
| **TSPInvariantChecks.Name** | formula | `DisplayName` |
| **TSPInvariantChecks.ActualHamiltonianCycleWitness** | lookup | `Lookup(CandidateTours.IsHamiltonianCycleWitness via CandidateTour)` |
| **TSPInvariantChecks.ActualOptimalityProved** | lookup | `Lookup(CandidateTours.IsOptimalityProved via CandidateTour)` |
| **TSPInvariantChecks.ActualTotalTravelCost** | lookup | `Lookup(CandidateTours.TotalTravelCost via CandidateTour)` |
| **TSPInvariantChecks.IsHamiltonianStatusCorrect** | formula | `ExpectedHamiltonianCycleWitness = ActualHamiltonianCycleWitness` |
| **TSPInvariantChecks.IsOptimalityStatusCorrect** | formula | `ExpectedOptimalityProved = ActualOptimalityProved` |
| **TSPInvariantChecks.IsCostCorrect** | formula | `ExpectedTotalTravelCost = ActualTotalTravelCost` |
| **TSPInvariantChecks.IsPassing** | formula | `And(IsHamiltonianStatusCorrect, IsOptimalityStatusCorrect, IsCostCorrect)` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
