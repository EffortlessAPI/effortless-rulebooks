# Rulebook Examples

The canonical collection of example domains — each one demonstrating a specific facet of what a **rulebook as hub** makes possible.

The premise is always the same: one JSON file (the rulebook) is the single source of truth. Every other artifact — Postgres schema, views, Python classes, Go structs, OWL ontology, UI, tests — is mechanically derived from it. The examples below are chosen because each one proves a distinct piece of that claim.

---

## The high-quality examples

### `simpsons-paradox` — *scale, corpus depth, and witnessed invariants*

**96 published and synthetic studies** (medicine, epidemiology, law, sports, education, economics, social science) poured into a single rulebook entity model. Simpson's paradox **emerges** as a derived boolean — there is no `ReversalDetection` entity. `IsReversal`, `DistortionType` (five types A/B/C+/C−/D), `SignalPurity`, and `CorrectedWinner` all fall out of formulas declared in the rulebook.

What this demonstrates:
- **Corpus-scale derivation.** 960 allocation-sweep rows, 21 algebraic self-consistency invariants, all passing.
- **The paradox as data.** A philosophical puzzle becomes a computable, queryable fact.
- **Witnessed build history.** The `Loops` table (59 rows) is the plan — every loop documents what domain concept was introduced and what natural-language question it answers.
- **Full-stack derivation.** Rulebook → Postgres view chain (`vw_case_cells` → `vw_stratum_summaries` → `vw_treatment_rankings` → `vw_model_summary`) → interactive Vite explorer UI.

→ [simpsons-paradox/README.md](simpsons-paradox/README.md)

---

### `talismans-special-solutions` — *multi-substrate conformance, ontology generation, live sync*

Inspired by Jessica Talisman's four-part ontology series. One rulebook generates: a Postgres database, a Python reasoner, an OWL ontology with SHACL rules, a SPARQL endpoint, an explainer DAG, and a React admin portal — all deriving the same computed answers.

What this demonstrates:
- **Multi-substrate conformance.** Postgres and the OWL reasoner independently compute `IsStale`, `SequencePosition`, `EscalationViolation`, and all 8 competency questions — answers match.
- **Live schema-drift visualization.** A triangle widget shows which store (rulebook head, reasoner, Postgres) is "ahead" at any moment; sync is direction-aware.
- **Semantic web portability.** The same business rules are simultaneously a relational schema, an OWL class hierarchy, and a set of SHACL constraints — derived, not hand-translated.
- **Escalation state machine.** `EscalationViolation` is a computed boolean that gates an org-chart popup showing the exact broken link in the chain.

→ [talismans-special-solutions/README.md](talismans-special-solutions/README.md)

---

### `veritasium-power-laws-and-fractals` — *science communication meets reproducibility*

Built alongside the Veritasium *Power Laws* video. Turns the video's claims into a small, reproducible, multi-language lab where the stories can be tested, not just plotted. Zipf, earthquakes, forest fires, sandpiles, scale-free networks, Sierpinski, Koch — all encoded as rulebook entities with formulas.

What this demonstrates:
- **Science claims as executable structure.** The rulebook is the spec; Python, Postgres, and Go all run the same computation.
- **Targeted at science communicators.** A worked example for YouTubers, writers, and lecturers who need reproducibility, not just pretty plots.
- **Where power laws break.** The rulebook encodes the measurement models and the cases where the straight-line story stops being true.

→ [veritasium-power-laws-and-fractals/README.md](veritasium-power-laws-and-fractals/README.md)

---

### `causal-autoimmune-architecture` — *grand-challenge complexity, one falsifiable boolean*

A grand-challenge prompt — infer the complete causal architecture of a heterogeneous autoimmune disease from a million-person multi-omic cohort — reduced to a single calculated-field DAG. The verdict (`IsActionable`) is computed from raw observations alone; nothing is hand-entered.

What this demonstrates:
- **Transparency as architecture.** Every Z-statistic, every gate, and the keystone verdict are inspectable formulas — the judgment that would otherwise hide inside a model is externalized into editable cells.
- **The trust boundary is a line in the graph.** Everything above the raw leaves is a pure formula; everything below is a raw observation.
- **Disease state machine.** A patient's progression is derived from longitudinal labs as a computed state, not a stored flag.
- **Clinical review feedback loop.** The model was corrected by a clinical reviewer and the correction is encoded as structure, not as a comment.

→ [causal-autoimmune-architecture/README.md](causal-autoimmune-architecture/README.md)

---

### `traffic-ticket-contest` — *state machines and a rules engine, both as data*

A universally-understood case-management domain (driver gets a ticket, may pay or contest it), chosen precisely because the interesting part is fully visible — it is all in the rules.

What this demonstrates:
- **Four computed state machines.** `CitationStatus`, `ContestStatus`, `PaymentStatus`, `LicenseStatus` — all calculated fields, never stored-and-mutated. Always correct because always re-derived.
- **Regulations as rows.** Every knob in `Jurisdictions` (`DaysToRespond`, `LatePenaltyPct`, `PointSuspensionThreshold`, …) is a data row. Change a number and every downstream citation re-derives its due dates and statuses — no migration, no application code.
- **Multi-jurisdiction rules engine.** The same formula works across all jurisdictions because the jurisdiction's parameters are inputs to the formula, not hardcoded branches.

→ [traffic-ticket-contest/README.md](traffic-ticket-contest/README.md)

---

### `intelligence-taxonomy` — *a three-hop DAG in miniature*

A minimal POC: four agents (human, octopus, LLM, pocket calculator) classified by aggregating per-capability scores through a three-hop calculated-field DAG. Built to let a researcher edit any raw score live and watch the taxonomic class flip.

What this demonstrates:
- **The cascade in miniature.** L0 raw scores → L1 weighted scores → L2 aggregated totals → L3 taxonomy class. Change one weight, rebuild, the whole classification re-derives.
- **"The rules live in one place."** The taxonomy is contested by design — any weight or threshold is debatable. The point is that editing the debate requires touching exactly one file.
- **Live app included.** Edit a raw score, see the class flip. The app is generated from the rulebook.

→ [intelligence-taxonomy/README.md](intelligence-taxonomy/README.md)

---

### `tiling-the-plane` — *mathematical validity as a derived boolean*

A catalog of Euclidean plane tilings — which ones exist and *why* each one is valid — plus a generative engine that places tiles into a region and measures coverage.

What this demonstrates:
- **Mathematical truth as a formula.** `VertexFigures.IsValid = (AngleGapDeg <= 0.0001)` — the validity of a tiling is derived, not asserted. No one can mark a tiling valid by hand.
- **Two halves, one DAG.** Catalog (existence proofs via angle sums) and generative (coverage measurement via SUMIFS) share the same entity model and the same build.
- **Geometry as data.** A mathematical library expressed as a rulebook — extensible by adding rows, not by writing code.

→ [tiling-the-plane/README.md](tiling-the-plane/README.md)

---

### `naive-set-theory` — *formal mathematics in a rulebook*

Naive set theory encoded as a rulebook. Sets, membership, union, intersection, complement, power sets — all expressed as entities and formulas.

→ [naive-set-theory/](naive-set-theory/)

---

### `is-everything-a-language` — *philosophical ontology, formal predicates*

A philosophical meta-ontology exploring what qualifies as a "language" through formal predicates and logical argument. The question is interesting; the point is that the argument is expressible as inspectable structure.

→ [is-everything-a-language/README.md](is-everything-a-language/README.md)

---

### `ross-style-business-rules` — *the keystone case for "only if" vs "iff"*

Five business rules re-encoded from a hand-correction by Ronald Ross (the business-rules / SBVR figure). Built around one dataset row (`claim-D`) that proves why DR-5 must be "only if" and not "iff." The meaning is legible in the structure.

→ [ross-style-business-rules/README.md](ross-style-business-rules/README.md)

---

### `acme-llc` — *hello, formulas*

The smallest viable rulebook with a real calculated field: one table (`Customers`), two formulas (`Name = SUBSTITUTE(EmailAddress, "@", "-")`, `Initials = Left(FirstName,1) & Left(LastName,1)`). Start here.

→ [acme-llc/README.md](acme-llc/README.md)

---

### `customer-fullname` — *name derivation, string concatenation*

One step above `acme-llc`. A `FullName` calculated field from `FirstName` + `LastName`. Comes with execution substrates (Postgres, Python, Go) proving that all three compute the same value.

→ [customer-fullname/](customer-fullname/)

---

## What the examples collectively demonstrate

| Capability | Where it's most clearly shown |
|---|---|
| Corpus-scale derivation (hundreds of rows, all computed) | `simpsons-paradox` |
| Multi-substrate conformance (Postgres = Python = OWL = Go) | `talismans-special-solutions`, `customer-fullname` |
| State machines as calculated fields (never stored-and-mutated) | `traffic-ticket-contest`, `causal-autoimmune-architecture` |
| Regulations / parameters as data rows | `traffic-ticket-contest` |
| Formal invariants that break the build on violation | `simpsons-paradox` |
| Mathematical truth as a derived boolean | `tiling-the-plane`, `naive-set-theory` |
| Science claims as executable, multi-language structure | `veritasium-power-laws-and-fractals` |
| Philosophical / semantic ontology | `is-everything-a-language`, `intelligence-taxonomy` |
| Grand-challenge complexity, one falsifiable verdict | `causal-autoimmune-architecture` |
| Witnessed build history (loops table as the plan) | `simpsons-paradox` |
| Full-stack derivation (rulebook → DB → UI) | `simpsons-paradox`, `talismans-special-solutions`, `intelligence-taxonomy` |
| Minimal entry point | `acme-llc`, `customer-fullname` |

---

## Also in this folder

- `acme-corporation/` — fuller ACME variant with more tables
- `effortless-banking/` — accounts, transactions, ledger
- `effortless-rulebooks/` — meta-ontology: the ERB project describing itself
- `nakedclaude-v1` through `v4/` — Naked-Claude vs. Effortless-Claude comparison experiments
- `planar-unit-discovery/` — experimental scope

---

## The invariant

Every example here satisfies the same property: **if you deleted all the derived artifacts and kept only the rulebook JSON, a transpiler could regenerate everything else identically.** That property — not any specific substrate, not any specific domain — is what this collection is here to demonstrate.
