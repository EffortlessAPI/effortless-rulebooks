# Ontology, Parts I–III: The Architecture of Meaning
**Jessica Talisman — Intentional Arrangement (Feb–Mar 2026)**

*Summary prepared for the Talisman's Special Solutions ERB rulebook*

---

## Overview

This three-part series demystifies ontologies — what they are, how to engineer them, and how to validate them. The worked example is NTWF: a workflow-management ontology for a fictional 1,100-person technology company called Special Solutions, whose processes involve human specialists, AI agents, and automated pipelines operating side by side. The NTWF ontology is the domain modeled in the Talisman's Special Solutions ERB rulebook.

---

## Part I — What Is an Ontology

The article opens by cataloguing why the term "ontology" is so widely misunderstood: it straddles philosophy and computer science; it functions simultaneously as a model, a language, and a specification; and marketers have diluted it by applying it to flat term lists, YAML configs, and database schemas.

Tom Gruber's 1993 definition anchors the series: an ontology is *"a formal, explicit specification of a shared conceptualization."* Every word matters — **formal** (machine-readable), **explicit** (stated, not implied), **specification** (a defined model), **shared** (intended for a community), **conceptualization** (a model of what exists and how it is organized).

The Semantic Web stack provides the substrate. **RDF** is the grammar (subject–predicate–object triples over URIs). **RDFS** adds class hierarchies, subclass inference, and domain/range constraints. **OWL** adds disjointness, cardinality, property characteristics, and class constructors. **SKOS** provides controlled vocabularies and lightweight hierarchies for human-navigable classification without full logical commitment.

The critical distinction is *ontological commitment*: a formal claim that certain kinds of things exist and that they relate to one another in defined ways. By this test the article evaluates Dublin Core, SKOS, FOAF, PROV-O, DCAT, and Schema.org — each makes genuine ontological commitments at varying levels of expressiveness and therefore qualifies as an ontology, despite not all being expressed in full OWL.

The **Ontology Spectrum** (McGuinness 2003) maps knowledge structures on a gradient from flat controlled vocabularies to richly axiomatized logical models. The **Ontology Pipeline®** describes the engineering stages from vocabulary → taxonomy → thesaurus → ontology → knowledge graph. The two frameworks are complementary: the Spectrum tells you where you are; the Pipeline tells you how to build.

---

## Part II — Design Heuristics and Methodology

The domain is introduced: Special Solutions needs its accumulated process knowledge to be formally queryable, across workflows that involve human specialists, AI agents, and automated pipelines with different accountability structures.

**Competency Questions (CQs)** are the ontology's contract. Before writing a single triple, natural-language questions define the scope: *"Which steps are executed by AI agents?" "Can an AI agent act in a delegation chain?" "What datasets does the quarterly business review consume?"* Every class, property, and axiom must trace back to at least one CQ. Anything that traces to none is unnecessary complexity.

### Six Design Heuristics

1. **Reuse before you invent.** Borrow from PROV-O, FOAF, Dublin Core, DCAT, Schema.org, SKOS. A well-designed domain ontology is mostly assembly of existing standards.

2. **Separate roles from agents.** "Release Manager" is a role; "Maria Gonzalez" is a person who fills it today. Wire steps to roles, roles to agents. When personnel change, one triple changes; the workflow structure is untouched. This extends to AI agents: "Risk Analysis Agent" is a role, not an identity.

3. **Use SKOS for classification, RDFS/OWL for commitment.** SKOS concepts organize and label; RDFS/OWL class declarations carry logical consequences that reasoners act on.

4. **Domain and range are inference rules, not constraints.** In RDFS, if a property's domain is `WorkflowStep` and you attach it to something else, the reasoner *infers* that something else is a `WorkflowStep` — it does not reject the triple. Run a test ABox early to surface these silent type-promotion errors before they propagate.

5. **Model for the open world.** Absence of a fact means unknown, not false. Add `owl:disjointWith` or `owl:FunctionalProperty` only where a violation represents a real logical error.

6. **Document as you build.** Write the label and definition before the axiom. Difficulty writing a definition is a signal the concept boundary is not yet understood.

### The Three-Box Architecture as Design Sequence

- **CBox first** — namespaces, license, documentation conventions, SKOS controlled vocabularies.
- **TBox second** — class hierarchy before properties before constraints.
- **ABox third (but not last)** — a representative test instance set run through a reasoner to validate every class, property, and axiom before deployment.

### Standards Survey for NTWF

| Standard | Contribution |
|---|---|
| PROV-O | `prov:Plan` → Workflow, `prov:Activity` → WorkflowStep, `prov:Entity` → artifact |
| FOAF | `foaf:Agent` / `foaf:Person` → agent hierarchy root and human agents |
| Schema.org | `schema:Organization` → Department, `schema:SoftwareApplication` → pipelines |
| DCAT | `dcat:Dataset` / `dcat:Distribution` → dataset consumption tracing |
| Dublin Core | `dct:title`, `dct:modified`, `dct:identifier` → workflow metadata and stale-workflow queries |

Nine custom terms are justified only where the standards survey returns nothing adequate.

---

## Part III — The Build and Validation

### Classes

The NTWF vocabulary has nine classes in two groups.

**Workflow structure:**

| Class | Superclass(es) | Notes |
|---|---|---|
| `Workflow` | `prov:Plan`, `schema:CreativeWork` | Dual nature: process spec and organizational record |
| `WorkflowStep` | `prov:Activity` | First-class entity, individually addressable |
| `ApprovalGate` | `WorkflowStep` | Adds `requiresHumanApproval` and `escalationThresholdHours`; double-typed in ABox for SPARQL compatibility |
| `WorkflowArtifact` | `prov:Entity`, `schema:CreativeWork` | Carries provenance chain via `prov:wasDerivedFrom` |

**Actors and organizational context:**

| Class | Superclass(es) | Notes |
|---|---|---|
| `Role` | *(custom root)* | No adequate standard match; disjoint with WorkflowStep and WorkflowArtifact |
| `HumanAgent` | `foaf:Person`, `prov:Agent` | Only type that may fill roles where `requiresHumanApproval` is true |
| `AIAgent` | `prov:SoftwareAgent` | Adds `modelVersion` datatype property for audit trail |
| `AutomatedPipeline` | `prov:SoftwareAgent`, `schema:SoftwareApplication` | Distinguished from AIAgent by deterministic vs. probabilistic output semantics |
| `Department` | `schema:Organization` | First-class entity enabling cross-department intersection queries |

All three agent types are declared **mutually disjoint**. A data-entry error that double-types an individual produces a detectable inconsistency rather than silently corrupting downstream queries.

### Key Object Properties

| Property | Type | Purpose |
|---|---|---|
| `hasStep` / `isStepOf` | inverse pair | Workflow to steps |
| `precedesStep` | `owl:TransitiveProperty` | Four asserted triples; OWL-RL closure produces all ten non-adjacent ordering pairs, including step 1 precedes step 5 (never asserted) |
| `assignedRole` | `owl:FunctionalProperty` | One role per step; double-assignment causes the reasoner to merge roles via `owl:sameAs`, revealing the error |
| `filledBy` | — | Role to agent; the single change-management triple for personnel or AI upgrades |
| `delegatesTo` | — | Role to fallback role in escalation chain, traversable via SPARQL property path |
| `consumesDataset` | — | Step to `dcat:Dataset`, kept separate from `requiresArtifact` to preserve DCAT metadata semantics |

**Five datatype properties** carry literal values: `sequencePosition` (integer, functional), `requiresHumanApproval` (boolean), `modelVersion` (string on AIAgent), `stepDurationMinutes`, `escalationThresholdHours` (on ApprovalGate).

### Validation

The ABox exercises every term: five workflow steps, three agent types, six roles, five artifacts forming a provenance chain via `prov:wasDerivedFrom`, one DCAT dataset, one approval gate. Validation runs **104 tests** in four suites using `rdflib` and `owlrl`. The baseline graph has 433 triples; after OWL-RL closure it expands to over 800. Suite 4 injects deliberate errors to confirm detection patterns — disjointness violations, functional-property double-assignments, Heuristic 4 domain-inference traps — all surface correctly.

The metadata layer — Dublin Core properties spread across all three boxes — is the integration surface. Workflows carry `dct:modified` values sourced from a document management system; a single SPARQL query over those values answers *"which workflows involve AI agents and haven't been reviewed in twelve months?"* without any bespoke integration. This is the article's central demonstration: when organizational data is expressed through shared standard vocabularies, the ontology's inference machinery becomes directly useful to business stakeholders.

---

## Mapping to the Talisman's Special Solutions Rulebook

The Talisman's Special Solutions ERB rulebook replicates the core NTWF domain as relational tables:

| Rulebook Table | Ontology Counterpart |
|---|---|
| `Workflows` | `prov:Plan` / `schema:CreativeWork` |
| `WorkflowSteps` | `prov:Activity`, with `sequencePosition` |
| `ApprovalGates` | `ntwf:ApprovalGate` subtype — a gate row specializes one `WorkflowStep` via a 1:1 FK (class-table inheritance form of `rdfs:subClassOf`), carrying only `escalationThresholdHours` |
| `StepPrecedence` | First-class step→step ordering junction (`FromStep`/`ToStep`), one row per asserted `ntwf:precedesStep` edge |
| `Roles` | `ntwf:Role` (assignedRole / filledBy / ownedBy / delegatesTo) |
| `Departments` | `ntwf:Department` (`schema:Organization`) |
| `HumanAgents` | `ntwf:HumanAgent` (`foaf:Person`) |
| `AIAgents` | `ntwf:AIAgent` (`prov:SoftwareAgent` + `modelVersion`) |
| `AutomatedPipelines` | `ntwf:AutomatedPipeline` (`prov:SoftwareAgent` + `schema:SoftwareApplication`) |

The role-agent separation heuristic is structurally enforced: `WorkflowSteps` point to `Roles`; `Roles` point to the three agent tables through a polymorphic `filledBy` relationship (with exactly one arm set per role, mirroring the functional property + disjointness). The `StepPrecedence` junction mirrors the transitive `precedesStep` property as first-class step→step edges, enabling path queries over the step sequence (4 asserted edges → 10-pair closure). `ApprovalGates` is modeled as a true subtype: each gate row shares identity with the `WorkflowStep` it specializes rather than collapsing two nodes into one. The three agent types are kept as distinct tables, reflecting the ontology's disjointness declarations and their different accountability structures.
