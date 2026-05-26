# THE_PLAN — Planar Unit-Distance Neighborhood (Curation Toward a Formal Rulebook)

> **Status.** Curation workspace. This document and the JSON/SQL sketches in
> the same folder are *not* the formal rulebook. They are the curation surface
> from which `effortless-rulebook/planar-unit-discovery-rulebook.json` will be
> bootstrapped. Anything substantive must end up captured here before the
> sketches are retired.

---

## 1. Premise — what this artifact is

The artifact is a **structural mirror** of the mathematical neighborhood around
the planar unit-distance problem. It is a typed, declarative, queryable object
that names the entities, relations, and derivations of that neighborhood as
first-class data.

It is explicitly **not**:

- a proof,
- a reasoning trace of how a result was found,
- a research workspace whose deliverable is "make the gaps visible,"
- a methodology document or paper.

The result we care about (Sawin, *An explicit lower bound for the unit
distance problem*, arXiv:2605.20579 — building on the OpenAI superlinear bound
that preceded it) has **already been discovered**. That discovery is **input**
to this work, not output. The work is to *describe* the discovered structure
in CMCC vocabulary so that it becomes a fixed point in a navigable object,
rather than a black-box result narrated in prose.

Phrased operationally: build a sufficiently dense declarative model of the
mathematical neighborhood — in a common vocabulary across the sub-domains —
such that the discovered result becomes a *locatable fixed point* in that
object, not a magic-trick output. The artifact is a digital mirror of the
mathematics. Build the object; the result emerges as a projection.

This is the same move the Veritasium power-laws rulebook
(`github.com/eejai42/veritasiums-power-laws-and-fractals`) makes for that
video, scaled up: prose-claim → typed data object whose every claim is a cell
with witnesses, whose every derivation is a formula in the DAG, and whose
behavior is recomputable identically in any conforming substrate.

The deeper thesis is that of "Triangles, Baseball, and Quantum
Superposition" — truth is an emergent property of structure. We do not
encode the *theorem*; we build the *object* whose structure makes the theorem
fall out as a projection. The same way the double-slit pattern emerges from
that paper's structural model without ever encoding the Schrödinger equation,
the unit-distance superlinear bound should be locatable in this object
without ever encoding "the proof" as a procedural sidecar.

## 2. Two predictions this artifact is testing

The work is worth doing because it tests two falsifiable predictions:

**Prediction A — Foundation side.** Once the neighborhood's intermediate
machinery is declared densely enough (every usually-unnamed object reified as
a first-class row, with formulas and FK reach), the AI-discovered result will
be a *short hop* from objects already in the DAG. The "long part of the hop"
will have been pre-paid by the structural work, not by the discoverer.

**Prediction B — Solution side.** Once the discovered result is *located* in
this object — represented as the fixed point it is, with all its witnesses
in the DAG — it stops being a black-box magic trick and becomes
obvious/uncontroversial/easy to see. Comprehensibility is decoupled from
discoverability (the e=mc² move: a fifth-grader can use the formula even
though no fifth-grader could derive it).

Both predictions fail loudly if false. Prediction A fails if, after the
neighborhood is densely populated, the bound is still many novel objects
away from the existing rows. Prediction B fails if, once mapped in,
mathematicians who walk the DAG still find the bound mysterious.

## 3. Describe, do not derive — the work discipline

The single most important framing rule for everything below:

> **Every row, field, formula, and relation in this rulebook is a description
> of something already known. The rulebook does not propose, conjecture,
> discover, or refine. It locates and names.**

Concrete consequences:

- **No `GapStatus`, no `candidate`, no `partial_candidate`, no
  `needs_direction_refinement` as data.** Those tags encode the modeler's
  epistemic state, not the structural object. The rulebook either has a row
  for an intermediate object (because the discovered result names one) or it
  does not. The curation notes (this document) may track what is and isn't
  yet loaded; the rulebook itself does not.
- **No agent-driven gap-extraction queries.** A query like "list route steps
  with `GapStatus in ('candidate', …)`" is a research instrument, not a
  structural property. It does not belong as a first-class artifact.
- **The proof content IS the data.** When the curation workflow says
  "populate the row for `OBJ_SHORT_VECTOR_RELATION`," it means: mine Sawin's
  paper (and the prior OpenAI bound it improves) for the actual short-vector
  relation those proofs use, and load it. Not "guess at what it might
  eventually be."
- **No speculative splits.** Phrases like "this object might later split into
  `controlled algebraic difference / norm-constrained difference class / …`"
  do not belong in the rulebook. If the discovered result already names a
  finer object, name that. Otherwise, do not invent placeholders.
- **No "do not add proof content yet."** The opposite: load the proof
  content as the first work, then see what structural pressure that puts on
  the existing types and relations.
- **The only acceptable modeler-added object: `OriginatedHere = true`.** If
  reading a paper surfaces an intermediate object the paper *uses without
  explicitly naming*, give it a row, but mark it `OriginatedHere = true` so
  it is auditably distinguishable from objects the papers named directly.
  This is description (the object is implicit in the proof), not discovery.
  It is **not** a license to speculate about objects the papers do not use.

### 3.1 The mirror contract — codified

Every row, formula, and relation in the rulebook must satisfy:

| Contract | What it forbids |
|---|---|
| **Rows are things that exist.** | No `Status: candidate`, `GapStatus: needs_direction_refinement`, or `AnchorRole: hidden_intermediate_candidate`. Either the object is in the source material and gets a row, or it isn't and gets no row. Epistemic state of the modeler does not belong in the data. |
| **Formulas are intrinsic properties of rows.** | No "tests for the agent to run on itself." `DistanceSquared` is a calculated field on a `PointPair`. It is not a self-check the agent performs. |
| **Relations live in the DAG by default.** | Reaching for a cyclic `REL_SEMANTIC_BRIDGE_TO` is a signal that a bridge object is missing. First move is to promote the bridge to a first-class row (§5), not to flip `IsDAGEdge=false`. The escape hatch is reserved for genuine cross-domain analogies with no intermediate object to name (e.g., the number-field ↔ function-field analogy) and must be justified per use. |
| **The rulebook is multi-substrate.** | Postgres-only is not a mirror — it is a schema with mathematical labels. At minimum one alternate substrate must compute the same field values, gated by an answer key (§14). |
| **The neighborhood is the unit, not the theorem.** | A single theorem anchor cannot validate the "dense enough that adjacent results are also locatable" claim. The rulebook hosts multiple co-located anchors that share the same intermediate machinery (§6). |

### 3.2 What is and is not in this neighborhood

**In scope.** Euclidean plane, finite point configurations, unit-distance
graphs, algebraic number theory as it bears on lattice constructions,
Minkowski embeddings, asymptotic combinatorics relevant to U(n), and the
specific incidence-geometry machinery (Szemerédi–Trotter, Elekes–Sharir,
Guth–Katz) the adjacent anchors require.

**Out of scope.**

- Category theory in general, model theory, and anything not present in a
  cited paper.
- Proving the theorem. The proof is in the papers; we describe it.
- Improving on the bound. We are not the discoverer.
- Refining any object name, signature, or edge direction by "agent
  inspection." If a refinement is needed, it comes from re-reading the
  source papers, not from the agent's own analysis.
- Marking anything as "future work" or "to be discovered." If we don't
  know it yet, we read the papers until we do. If the papers don't say,
  the row doesn't exist.

## 4. CMCC substrate alignment

The rulebook satisfies CMCC's five-primitive decomposition (SDLAF) on a
bitemporal ACID DAG. This is not negotiable — it is what makes the
substrate-equivalence claim hold (Section 13).

| CMCC primitive | Realization in this rulebook |
| -------------- | ---------------------------- |
| **Schema (S)** | Table/field definitions for `domains`, `object_types`, `relation_types`, `relation_type_signatures`, `property_types`, and the domain-typed tables (Section 7). |
| **Data (D)** | Rows in those tables: `math_objects`, `math_relations`, `property_values`, plus the specific seed objects of Section 10. |
| **Lookups (L)** | Lookup fields and `REL_LOOKUP_FROM` traversals — e.g. `TypeLabel`, `DomainLabel`, `RelationName`. |
| **Aggregations (A)** | `formula_definitions` with `FormulaKind = aggregation` — e.g. edge count over `point_pairs`, `U(n)` over configurations. |
| **Formulas (F)** | `formula_definitions` with `FormulaKind = calculated/predicate` — e.g. distance squared, unit-distance predicate, density-exponent estimate. |
| **Bi-temporality** | Every assertion table carries `valid_range tstzrange` (mathematical validity) and `tx_range tstzrange` (database belief). Past truth is never mutated. |
| **ACID DAG** | Postgres constraints + trigger-enforced acyclicity on the evaluation-edge subgraph (Section 5). FK relationships across domain-typed tables also acyclic. |

## 5. Semantic topology vs evaluation DAG

The mathematical neighborhood is genuinely cross-linked: geometry and graph
theory share objects, algebraic number theory feeds into lattice embeddings
which project back into the plane, analogies flow both ways. That topology
is real and the rulebook must accommodate it.

But the **evaluation layer** — the edges used for lookup, calculation,
aggregation, and dependency ordering — must be acyclic. Otherwise the
generated views/functions cannot be ordered and substrate-equivalence
breaks.

The mechanism:

- Relation types carry an `IsDAGEdge` boolean.
- `REL_DEFINED_BY`, `REL_COMPUTED_FROM`, `REL_LOOKUP_FROM`,
  `REL_AGGREGATES_OVER`, `REL_DEPENDS_ON` have `IsDAGEdge = true` and
  participate in the acyclicity trigger.
- `REL_SEMANTIC_BRIDGE_TO`, `REL_ANALOGOUS_TO` have `IsDAGEdge = false` and
  may participate in cycles in the semantic topology.

**Curation rule (correcting a v0.1 drift):** the *default* response to "I need
to connect X and Y across domains" is **not** to introduce a non-DAG semantic
bridge. The default is to ask whether the bridge is itself a first-class
intermediate **object** that has just not been named. If so, promote it to a
row with two acyclic FK reaches, and the bridge becomes a real entity
instead of a cyclic edge. This is the single most common case in this
neighborhood: e.g. `OBJ_UNIT_DISTANCE_GRAPH` *is* the bridge between
geometry and graph theory; it is a row, not an edge.

Non-DAG semantic bridges are reserved for genuinely irreducible cases — e.g.
the function-field/number-field analogy as a meta-statement *about* both,
where promoting it to an object would itself create a cycle.

## 6. Theorem anchors are co-locatable fixed points

The v0.1 sketch named a single theorem anchor
(`OBJ_THEOREM_SUPERLINEAR_UNIT_DISTANCE`). The rulebook is designed to hold
**many** such anchors in the same neighborhood, because the conjecture being
tested is that the foundation makes *adjacent* results also locatable.

The schema supports:

- A `T_THEOREM` object type and `REL_ANCHORS` relation that any theorem can
  use.
- An `IsTheoremAnchor` field on the math-object table.
- No structural assumption that there is exactly one anchor.

Anchors expected to be co-located in this neighborhood (loading order TBD,
listed here so the schema is not narrowed prematurely):

- `OBJ_THEOREM_SUPERLINEAR_UNIT_DISTANCE` — Sawin / OpenAI, *U(n)* > *n*<sup>1+c</sup>.
- `OBJ_THEOREM_ERDOS_DISTINCT_DISTANCES` — Guth–Katz resolution.
- `OBJ_THEOREM_SZEMEREDI_TROTTER` — point-line incidence bound (Spencer–Szemerédi–Trotter).
- `OBJ_THEOREM_CAYLEY_SALMON` — 27 lines on a cubic; used inside Guth–Katz.
- `OBJ_THEOREM_ELEKES_SHARIR` — incidences-on-ruled-surfaces reduction.

If the foundation is right, these share most of their intermediate machinery
with the unit-distance result. The rulebook schema must not prevent that
sharing.

## 7. The neighborhood — seven top-level domains

Preserved from v0.1. Each is a `domains` row.

| Domain ID  | Domain                            | Role                                                                       |
| ---------- | --------------------------------- | -------------------------------------------------------------------------- |
| `DOM_META` | Meta                              | Typing, relation signatures, field definitions, temporal governance.       |
| `DOM_EG`   | Euclidean geometry                | Plane, points, distance, unit-distance predicate.                          |
| `DOM_GT`   | Graph theory                      | Unit-distance graphs, edge counts, graph invariants.                       |
| `DOM_ANT`  | Algebraic number theory           | Number fields, rings of integers, norms, discriminants, prime ideals.      |
| `DOM_LE`   | Lattice embeddings                | Minkowski embeddings, lattices, Gram matrices, projections.                |
| `DOM_AC`   | Asymptotic combinatorics          | Asymptotic functions, exponents, lower bounds, construction families.      |
| `DOM_THM`  | Theorem anchors                   | Theorem objects and the fixed-point claims they anchor.                    |

## 8. The object spine

The structural backbone connecting the domains is:

```
Number field K
  → ring of integers O_K
  → Minkowski embedding
  → Minkowski lattice
  → Gram matrix / short-vector relation
  → planar projection
  → finite point configuration P ⊂ R²
  → unit-distance graph G(P)
  → edge count e(G(P))
  → maximum unit-distance function U(n)
  → superlinear lower bound (the anchored coordinate)
  → theorem anchor
```

This is not a proof route — it is the spine along which the relevant
intermediate objects are arranged. Some of these objects (e.g.
`OBJ_SHORT_VECTOR_RELATION`) are the "usually-unnamed intermediates" the
foundation prediction (Section 2A) is principally about.

## 9. Object types

Abstract metatypes — these populate `object_types` and form a typing
ancestry tree used by the relation-signature trigger.

```
T_MATH_OBJECT       (root)
T_CONCEPT
T_STRUCTURE
T_SET
T_FUNCTION
T_PREDICATE
T_INVARIANT
T_TRANSFORMATION
T_PROJECTION
T_CONSTRUCTION_FAMILY
T_THEOREM
T_BOUND
```

Domain specializations of the metatypes:

```
T_EUCLIDEAN_PLANE
T_POINT
T_POINT_SET
T_METRIC
T_DISTANCE_PREDICATE

T_GRAPH
T_GRAPH_INVARIANT
T_GRAPH_EMBEDDING

T_NUMBER_FIELD
T_RING
T_IDEAL
T_PRIME_IDEAL
T_FIELD_INVARIANT
T_FIELD_EMBEDDING

T_LATTICE
T_GRAM_MATRIX
T_MINKOWSKI_EMBEDDING

T_ASYMPTOTIC_FUNCTION
T_GROWTH_BOUND
T_SEMANTIC_BRIDGE
```

These are intentionally modest. They cover the objects the seed loading
(Section 11) needs to type. New types may be added as the AI's discovered
result names objects that don't fit existing types — but only because the
discovered result *requires* them, not as anticipatory padding.

## 10. Relation types

```
REL_DEFINED_BY                   (IsDAGEdge=true)
REL_DEPENDS_ON                   (IsDAGEdge=true)
REL_INDUCES                      (IsDAGEdge=true)
REL_HAS_INVARIANT                (IsDAGEdge=true)
REL_PRESERVES                    (IsDAGEdge=true)
REL_EMBEDS_IN                    (IsDAGEdge=true)
REL_PROJECTS_TO                  (IsDAGEdge=true)
REL_REALIZES                     (IsDAGEdge=true)
REL_COUNTS                       (IsDAGEdge=true)
REL_BOUNDS                       (IsDAGEdge=true)
REL_ASYMPTOTIC_LOWER_BOUND_FOR   (IsDAGEdge=true)
REL_COMPUTED_FROM                (IsDAGEdge=true)
REL_LOOKUP_FROM                  (IsDAGEdge=true)
REL_AGGREGATES_OVER              (IsDAGEdge=true)
REL_ANCHORS                      (IsDAGEdge=true)

REL_SEMANTIC_BRIDGE_TO           (IsDAGEdge=false)  ← reserve for irreducible cases
REL_ANALOGOUS_TO                 (IsDAGEdge=false)  ← reserve for irreducible cases
```

Relation types carry signatures (rows in `relation_type_signatures`)
enforced by the `assert_relation_signature()` trigger. Example signatures:

```
T_POINT_SET            --REL_INDUCES--                  → T_GRAPH
T_RING                 --REL_EMBEDS_IN--                → T_LATTICE
T_LATTICE              --REL_PROJECTS_TO--              → T_POINT_SET
T_CONSTRUCTION_FAMILY  --REL_REALIZES--                 → T_GROWTH_BOUND
T_GROWTH_BOUND         --REL_ASYMPTOTIC_LOWER_BOUND_FOR → T_ASYMPTOTIC_FUNCTION
T_THEOREM              --REL_ANCHORS--                  → T_MATH_OBJECT
```

Signature mismatches fail at write time, not at query time.

## 11. Seed objects

These are the first rows to load. They are grouped by domain. Each entry is
the object's identifier in `math_object_identities`; the version, property
values, and relation assertions are loaded into the corresponding tables.

**Euclidean geometry (DOM_EG)**

```
OBJ_EUCLIDEAN_PLANE_R2
OBJ_FINITE_POINT_SET
OBJ_EUCLIDEAN_DISTANCE
OBJ_UNIT_DISTANCE_PREDICATE
OBJ_UNIT_DISTANCE_PAIR
```

Conceptual flow: distance function → unit-distance predicate → unit-distance
pair → counted geometric atom.

**Graph theory (DOM_GT)**

```
OBJ_UNIT_DISTANCE_GRAPH
OBJ_GRAPH_EDGE_COUNT
```

Bridge: P ⊂ R² induces G(P); edge in G(P) iff d(p,q)=1; edge count =
number of unit-distance pairs. `OBJ_UNIT_DISTANCE_GRAPH` is itself the
bridge object between `DOM_EG` and `DOM_GT` (see Section 5).

**Asymptotic combinatorics (DOM_AC)**

```
OBJ_UNIT_DISTANCE_MAX_FUNCTION         ← U(n) = max_{|P|=n} e(G(P))
OBJ_SUPERLINEAR_LOWER_BOUND
OBJ_EXPLICIT_N_1_014_BOUND             ← Sawin's explicit coordinate
```

**Algebraic number theory (DOM_ANT)**

```
OBJ_NUMBER_FIELD_K
OBJ_RING_OF_INTEGERS_OK
OBJ_FIELD_DEGREE
OBJ_FIELD_DISCRIMINANT
OBJ_SMALL_NORM_PRIME_IDEALS
OBJ_FIELD_NORM
OBJ_GOLOD_SHAFAREVICH_CRITERION
```

**Lattice embeddings (DOM_LE)**

```
OBJ_MINKOWSKI_EMBEDDING
OBJ_MINKOWSKI_LATTICE
OBJ_GRAM_MATRIX
OBJ_SHORT_VECTOR_RELATION         ← the principal hidden intermediate
OBJ_PLANAR_PROJECTION
```

`OBJ_SHORT_VECTOR_RELATION` is the intermediate the conjecture says should
become *much* sharper once Sawin's paper is mined for its actual structure.
It is loaded into the rulebook as whatever the paper defines it to be —
not as a placeholder name awaiting refinement.

**Theorem anchors (DOM_THM)**

```
OBJ_THEOREM_SUPERLINEAR_UNIT_DISTANCE        ← anchors OBJ_SUPERLINEAR_LOWER_BOUND
(future anchors per Section 6 — Erdős distinct-distances, Szemerédi–Trotter, etc. —
 share the schema and load without redesign)
```

## 12. Semantic routes

Routes are ordered paths through the spine. They live in `semantic_routes`
and `semantic_route_steps`. They are descriptive of the structure, not
prescriptive of how to derive it.

The top-level route is `RTE_FULL_SEMANTIC_SPINE`, decomposing into three
sub-routes:

**Route 1 — geometry → graph**

```
finite point set
  --REL_INDUCES-->            unit-distance graph
  --REL_DEFINED_BY-->         unit-distance predicate
  --REL_COUNTS-->             graph edge count
```

**Route 2 — algebra → lattice → plane**

```
number field K
  --REL_HAS_INVARIANT-->      small-norm prime ideals
ring of integers O_K
  --REL_EMBEDS_IN-->          Minkowski lattice
Minkowski lattice
  --REL_HAS_INVARIANT-->      Gram matrix
Gram matrix
  --REL_COMPUTED_FROM-->      short-vector relation
Minkowski lattice
  --REL_PROJECTS_TO-->        finite planar point set
```

**Route 3 — local counts → asymptotic theorem**

```
graph edge count
  --REL_AGGREGATES_OVER-->    growth sequence
construction family
  --REL_REALIZES-->           superlinear lower bound
superlinear lower bound
  --REL_ASYMPTOTIC_LOWER_BOUND_FOR-->  U(n)
theorem
  --REL_ANCHORS-->            superlinear lower bound
```

Routes are not annotated with `GapStatus`. If a route step's target object
is not yet loaded as a row, the route is incomplete and that fact is visible
without an instrumentation field — the FK simply has no target. Curation
notes (this document, Section 15) track what remains to be loaded.

## 13. Formula layer

Formulas are intrinsic properties of rows in the domain-typed tables, not
self-tests for an agent. Every formula below is a calculated, lookup, or
aggregation field on a specific row class.

```
F_DISTANCE_SQUARED          on PointPair
=POWER({{x2}}-{{x1}},2)+POWER({{y2}}-{{y1}},2)

F_IS_UNIT_DISTANCE          on PointPair (predicate)
=ABS({{DistanceSquared}}-1) <= {{Tolerance}}

F_INDUCED_EDGE_COUNT        on PointConfiguration (aggregation)
=COUNTROWS(FILTER(point_pairs, {{IsUnitDistance}}=TRUE))

F_DENSITY_EXPONENT_ESTIMATE on PointConfiguration
=LOG({{EdgeCount}})/LOG({{PointCount}})

F_MINKOWSKI_GRAM_DISTANCE   on LatticePair
=TRANSPOSE({{delta_vector}}) * {{GramMatrix}} * {{delta_vector}}
```

**Note on `F_UNIT_DISTANCE_MAX`.** The v0.1 sketch had
`=MAX(FILTER(configurations, {{PointCount}}={{n}}).{{EdgeCount}})`. This is
fine as a definition of `U(n)` over a *loaded* set of configurations — but
*U(n)* in the mathematical sense is the max over **all** configurations of
size *n*, which is not enumerable for general *n*. The formula should be
understood as an empirical-on-the-loaded-rows aggregation, with the true
*U(n)* represented as a separate `OBJ_UNIT_DISTANCE_MAX_FUNCTION` whose
asymptotic behavior is anchored by the bound objects, not computed by row
aggregation. This is one of the open curation decisions (Section 15).

## 14. Substrate equivalence and the answer-key gate

This is the most important section the v0.1 sketch was missing. Without it,
the rulebook is a Postgres schema with mathematical labels — it loses the
CMCC claim that the model is the invariant.

The rulebook must compile to **at least two structurally-different
substrates** and produce field-for-field-identical outputs for the same
inputs, gated by an `answer-key.json`.

Initial substrate targets (matching the Veritasium-rulebook template):

- **PostgreSQL** — generated tables + views + functions from the rulebook.
  The reference implementation for formula coverage.
- **Sage (preferred) or Python** — the secondary substrate. Sage is the
  strong candidate because it can do the algebraic-number-theory and
  lattice work (number fields, rings of integers, Minkowski embeddings,
  Gram matrices) directly; Python is acceptable if a Sage dependency is
  too heavy.
- **Go** — generated structs + computation functions per row (optional for
  v0.1).

Per-substrate runners read the same loaded data, recompute every calculated
field and aggregation, and write outputs that are diffed against
`answer-key.json`. Mismatch → that substrate's transpiler is wrong (not
the rulebook).

The answer key for v0.1 should at minimum cover:

- distance-squared and `IsUnitDistance` over a known small point set,
- induced edge counts on small configurations,
- a fixed *U(n)* value for *n* small enough to be exhaustively computed
  (and clearly labeled as empirical, distinct from the asymptotic
  `OBJ_UNIT_DISTANCE_MAX_FUNCTION`),
- Gram-matrix distances on a fixed Minkowski lattice example,
- a registered numerical witness for `OBJ_EXPLICIT_N_1_014_BOUND` for one
  specific *n* (the structural location of the bound; the proof that it
  holds is in `REL_ANCHORS`, not in a substrate).

Without this section, prediction A and B (Section 2) are not testable —
because the "object" claim degenerates to "a schema someone wrote." The
multi-substrate gate is what makes the object real.

## 15. Open curation decisions

These are decisions the curator needs to make before the formal rulebook is
bootstrapped. They are not "gaps in the mathematics"; they are choices
about *how* to model already-known content.

1. **Generic-graph vs domain-typed tables.** The v0.1 SQL sketch models math
   objects as a generic graph (`cmcc_math_object_identities`,
   `cmcc_math_object_versions`, `cmcc_relation_assertions`, etc.). The
   Veritasium-template approach is the opposite: specific domain-typed
   tables (`Points`, `PointPairs`, `PointConfigurations`,
   `UnitDistanceGraphs`, `NumberFields`, `MinkowskiLattices`, etc.) with
   typed columns and field-level formulas.

   The mirror framing favors the domain-typed approach — calculated values
   live as field formulas, lookups are FK reaches, and substrate generators
   produce typed code per table. The generic-graph approach is meta-mode
   (a rulebook *describing* a rulebook) and runs into formula-interpretation
   problems at substrate generation time.

   Decision needed: pick one and rewrite the SQL sketch accordingly. The
   meta-typing (relation signatures, type ancestry) can be a small auxiliary
   layer on top of the domain-typed tables if needed.

2. **Scope of `OBJ_UNIT_DISTANCE_MAX_FUNCTION` as a computed vs anchored
   object.** See Section 13's note. The most defensible answer is two
   distinct objects: an empirical-aggregation version computed over loaded
   configurations, and an asymptotic version represented by its anchored
   lower-bound objects. Decision needed: confirm and split.

3. **How many anchors to seed in v0.1.** Section 6 lists five candidate
   theorem anchors that should be co-locatable. v0.1 only needs the
   unit-distance anchor *loaded*, but the schema must already support the
   others without redesign. Decision needed: confirm v0.1 ships with one
   anchor loaded and the schema validated against the others as a
   non-loaded conformance check.

4. **What "loaded into the rulebook" means for Sawin's paper.** The work
   that turns the discovery into data: extract every named lemma, every
   intermediate construction, every short-vector relation invocation, every
   norm/discriminant condition, and load them as rows. Decision needed: a
   curation pass over the paper that produces an explicit row-list before
   the formal rulebook is generated.

5. **Whether `cmcc_*` table names survive into the formal rulebook.** The
   formal effortless rulebook convention is PascalCase singular table names
   on the rulebook side, with the substrate transpilers producing
   substrate-idiomatic identifiers. The `cmcc_*` names in the SQL sketch are
   exploratory. Decision needed: drop them in favor of the convention.

## 16. Structural integrity checks (replaces the v0.1 "model_tests")

The v0.1 sketch listed five model tests. Test #5 ("explicit gaps") is
removed — it is research instrumentation, not a structural check.

The remaining checks, all of which the substrate generators must enforce
before any conformance run:

1. **Relation-signature validity.** Every row in `relation_assertions` must
   satisfy a registered `relation_type_signature`. Enforced by trigger at
   write time.
2. **Evaluation-DAG acyclicity.** All `IsDAGEdge = true` relation assertions
   form an acyclic graph. Enforced by trigger.
3. **Theorem-anchor reachability.** For every theorem in `DOM_THM`, there
   exists a path from the loaded domain objects to the theorem via
   `IsDAGEdge = true` relations. Failure here is a *structural* finding (the
   schema does not yet reach the anchor through DAG edges), distinct from
   "the proof is incomplete." The schema's reachability is the property
   being checked.
4. **Formula dependency topological sort.** The union of
   `REL_COMPUTED_FROM`, `REL_LOOKUP_FROM`, `REL_AGGREGATES_OVER`, and
   `formula_definitions.DependsOnObjectIDs` must be topologically sortable.
   This is the precondition for view generation.
5. **Substrate conformance** (new). For every loaded row, every calculated
   and aggregated field must match across all configured substrates,
   diffed against `answer-key.json`. This is the gate that makes the
   rulebook the invariant.
6. **No epistemic-state columns** (linting). No row in any domain-typed
   table contains `Status`, `GapStatus`, `*Candidate*`, or similar
   modeler-epistemic markers. Enforces the describe-do-not-derive rule
   (§3) at the storage layer, not just by author discipline.

There is no "find the gaps" check. Theorem-anchor reachability (#3) is
the only "is the rulebook complete" signal; if an anchor is unreachable
from raw objects via DAG edges, the fix is to re-read the papers, not to
add an instrumented `GapStatus` column. "Find the gaps" is a research
instrument, not a structural property of the data.

## 17. Postgres skeleton (sketch)

The v0.1 Postgres tables, preserved for reference; the actual generated SQL
will come from `effortless-rulebook/planar-unit-discovery-rulebook.json`
via the rulebook-to-postgres transpiler and will follow effortless naming
conventions (not the `cmcc_*` prefixes used in the sketch).

```
contexts
domains
object_types
relation_types
relation_type_signatures
type_ancestry

math_object_identities
math_object_versions          (carries valid_range, tx_range)

relation_assertions           (carries valid_range, tx_range; triggers: signature, DAG)

property_types
property_value_assertions     (carries valid_range, tx_range)
formula_definitions

semantic_routes
semantic_route_steps
```

Triggers:

```
assert_relation_signature()   — checks every assertion against a signature row
assert_no_dag_cycle()         — checks IsDAGEdge=true subgraph for cycles
```

Bi-temporality: every assertion-style table carries `valid_range tstzrange`
(mathematical validity) and `tx_range tstzrange` (database belief).

## 18. Curation workflow (what to do, in order)

1. **Resolve open curation decisions** (Section 15). In particular: choose
   domain-typed tables, lock in the `U(n)` split, drop the `cmcc_*` prefix.
2. **Mine Sawin's paper** (arXiv:2605.20579) and the prior OpenAI bound
   for every named intermediate object, construction, lemma, and relation.
   Produce a row-list, dataset by dataset, against the seed objects of
   Section 11. New types (Section 9) and new relations (Section 10) only as
   the paper forces them.
3. **Author the formal rulebook JSON** at
   `effortless-rulebook/planar-unit-discovery-rulebook.json` using the
   loaded row-list.
4. **Wire substrates and `answer-key.json`** (Section 14). At minimum
   Postgres + Python; Go is desirable but optional for v0.1.
5. **Run conformance**, fix any disagreement on the substrate side, never
   on the rulebook side.
6. **Validate prediction A and B** (Section 2) by inspecting the loaded
   object: how many hops from the existing rows is the bound? Is the
   structural location of the bound such that someone walking the DAG
   arrives at it without needing the paper's prose?
7. **Co-locate the next anchor** (Section 6). The Erdős distinct-distances
   neighborhood reuses most of the spine; add it and re-run conformance.
   The conjecture's "neighborhood" claim is validated here, not at step 6.

## 19. References

- Sawin, *An explicit lower bound for the unit distance problem*,
  arXiv:2605.20579.
- OpenAI (cited in Sawin's abstract): the unspecified-exponent superlinear
  bound preceding the explicit *n*<sup>1.014</sup> coordinate.
- `github.com/eejai42/veritasiums-power-laws-and-fractals` — canonical
  template for "narrative-claim → typed data object with multi-substrate
  conformance."
- "Triangles, Baseball, and Quantum Superposition" (EJ Alexandra) — the
  structural-truth thesis: double-slit results emerge from structure
  without ever encoding the Schrödinger equation. The methodological
  template for this work.
- CMCC executive summary and substrate matrix: see the `effortless-cmcc`
  and `effortless-rulebooks` skills in this environment.

## 20. Handoff (literal instruction block)

When delegating execution to an agent, give the agent this file and the
cited papers, and the following instructions verbatim:

> 1. Build the rulebook at
>    `effortless-rulebook/planar-unit-discovery-rulebook.json`.
>
> 2. Extract every named object, relation, formula, and intermediate quantity
>    from the cited papers and add as rows. For intermediates the papers use
>    but do not explicitly name, set `OriginatedHere = true` (see §3). Do not
>    invent placeholders for objects the papers do not use.
>
> 3. Add adjacent-anchor rows (Erdős distinct-distances, Szemerédi–Trotter /
>    Spencer–Szemerédi–Trotter, Elekes–Sharir, optionally Cayley–Salmon /
>    Guth–Katz). Load enough of each to test shared-intermediate-object
>    reachability (§6).
>
> 4. For every cross-domain relation, decide row-or-analogy. Default to row;
>    keep `IsDAGEdge=false` only for genuinely irreducible analogies (§5).
>
> 5. Build the substrate plan (§14): Postgres primary, Sage (preferred) or
>    Python secondary, wire ExplainDAG, generate the English projection,
>    populate `answer-key.json`.
>
> 6. Implement the structural integrity checks in §16. They MUST pass before
>    this is considered done.
>
> Do NOT add `Status` / `GapStatus` / `Candidate` markers to any row.
> Do NOT speculate about objects not in the source papers.
> Do NOT "refine" anything by inspection — re-read the papers if uncertain.
> The deliverable is a structural mirror, not a research workspace.

**Closing rule.** If the agent asks the user a question that is answerable
by reading the source papers, the agent has misunderstood the assignment.

---

## Appendix A — what changed from the v0.1 sketch, and why

This appendix exists so nothing substantive from the v0.1 plan is silently
lost during the curation rewrite.

**Preserved (with the same intent):**

- Seven-domain decomposition (Section 7).
- Object spine (Section 8).
- Object types and metatypes (Section 9).
- Relation types and their signatures (Section 10).
- Seed object list (Section 11).
- Three-route semantic spine (Section 12).
- Formula sketches (Section 13).
- Bitemporal `valid_range` / `tx_range` design.
- ACID-DAG-with-signature-and-cycle triggers.
- Semantic-topology / evaluation-DAG split (Section 5).
- Theorem-as-fixed-coordinate framing (Section 6).
- `OBJ_SHORT_VECTOR_RELATION` as the principal load target (Section 11).

**Removed and why:**

- `GapStatus`, `candidate`, `partial_candidate`,
  `candidate_direction_check`, `needs_direction_refinement`,
  `candidate_hidden_intermediate`, and the "first agent should query gap
  rows" pattern. These encode the modeler's epistemic state, not the
  structural object. Curation notes track what is and isn't loaded; the
  rulebook does not.
- The "do not add proof content yet" instruction. Inverted: the proof
  content is the input data to load first.
- Speculative splits of `OBJ_SHORT_VECTOR_RELATION` into invented finer
  names. Replaced with: load the actual finer objects Sawin's paper names.
- Framing of formulas as "tests an agent runs to check its own
  consistency." Replaced with: intrinsic properties of typed rows.
- The single-anchor assumption baked into the schema. Replaced with
  explicit multi-anchor support (Section 6).
- The "the agent's deliverable is a queryable semantic map whose gaps are
  visible" framing. Replaced with: the deliverable is a multi-substrate
  conforming object that locates the AI-discovered result as a fixed point
  (Sections 1, 2, 14).

**Added that the v0.1 sketch did not have:**

- Explicit substrate-equivalence + answer-key requirement (Section 14).
- Explicit multi-anchor co-location design (Section 6).
- Explicit predictions A and B that this artifact is testing (Section 2).
- The "describe, do not derive" work discipline as a first-class rule
  (Section 3).
- Reference to the Veritasium-rulebook as the template and to the
  structural-truth paper as the methodological grounding (Section 1, 19).
- Open curation decisions enumerated for resolution before formal
  bootstrap (Section 15).
- Curation workflow with explicit ordering (Section 18).

**Folded in from the root prescriptive `THE_PLAN.md` (previously a
separate, uncommitted file at `rulebook-examples/planar-unit-discovery/THE_PLAN.md`):**

- The "digital mirror" / "locatable fixed point, not magic-trick output"
  operative phrasing (Section 1).
- `OriginatedHere = true` as the *only* acceptable form of modeler-added
  object — used when a paper's proof uses an intermediate without
  explicitly naming it (Section 3).
- The mirror contract codified as a five-row table (Section 3.1),
  consolidating rules previously scattered across §§3, 5, 6, 14.
- An explicit enumeration of what *is* and *is not* in the mathematical
  neighborhood — including the meta-out-of-scope items "proving the
  theorem," "improving on the bound," and "refining by agent
  inspection" (Section 3.2).
- Spencer–Szemerédi–Trotter explicitly named alongside Szemerédi–Trotter
  (Section 6).
- Sage promoted to *preferred* secondary substrate (over Python) because
  it handles algebraic-number-theory and lattice work directly
  (Section 14).
- A "no `find-the-gaps` check" rule, plus a linting check (#6) for
  epistemic-state columns at the storage layer (Section 16).
- A literal handoff instruction block (Section 20) including the
  closing rule: "If the agent asks the user a question that is
  answerable by reading the source papers, the agent has misunderstood
  the assignment."

The root file is removed; this document is now the single home for the
plan.
