# Talisman BASIC — the NTWF Ontology

**A 14-table workflow ontology that tells one complete story: the Production Deployment Workflow.**

This rulebook models **NTWF**, the workflow-management ontology from Jessica Talisman's
[*Intentional Arrangement* / *Ontology, Parts I–III*](https://jessicatalisman.medium.com/) series.
The domain is *Special Solutions*, a fictional 1,100-person technology company whose processes
involve human specialists, AI agents, and automated pipelines side by side.

Rather than a pile of disconnected tutorial stubs, the seed data is **one curated worked example**
— the *Production Deployment Workflow* — that exercises every NTWF class, property, and competency
question with real, connected rows. (See `bootstrap/jessica-talisman-abstract.md` for a ~10 KB
summary of the source article.)

---

## What This Rulebook Demonstrates

- **14 interconnected tables** with foreign-key relationships (a DAG — no many-to-many)
- **All three disjoint agent types**: `HumanAgents`, `AIAgents`, `AutomatedPipelines`
- **Role–agent separation (Heuristic 2)**: steps assign to *Roles*, never directly to people/AI
- **Approval gate as a step subtype**: `ApprovalGates` specializes a `WorkflowStep` via a 1:1 FK
  (the relational form of `ntwf:ApprovalGate rdfs:subClassOf WorkflowStep`)
- **First-class step-to-step ordering**: `StepPrecedence` edges model `ntwf:precedesStep`
  (a transitive property — 4 asserted edges imply the full closure of 10 ordered pairs, incl. 1→5)
- **Delegation / escalation chain**: `Roles.DelegatesTo` (Release Manager → VP Engineering → CTO)
- **PROV provenance chain**: five `WorkflowArtifacts` linked by `DerivedFromArtifact`
- **DCAT dataset consumption**: a step consumes a `dcat:Dataset`, kept distinct from artifacts
- **SKOS controlled vocabularies**: workflow status and agent capability schemes
- **Aggregations & boolean derivations**: `COUNTIFS` rollups (incl. conditional counts over derived
  child fields) feeding workflow-level competency-question answers
- **Transitive closure**: `ntwf:precedesStep` and `ntwf:delegatesTo` are materialized as first-class
  closure views (`vw_step_precedence_closure`, `vw_roles_closure`) — the asserted edges plus every
  inferred reachability pair, each tagged `is_inferred` / `hop_distance`
- **Inference chains as derived fields**: role→agent lookups resolve each step's executing-agent
  *type*; a human-approval consistency witness and a disjointness/functional witness reproduce the
  ABox validation checks; staleness + "stale AND has-AI-step" reproduce the article's business query
- **The finale, as a first-class verdict**: `ComplianceVerdicts.IsAtComplianceRisk` is the single
  binary the article ends on — *"every stale workflow where an AI agent is executing a step and the
  compliance documentation hasn't been reviewed."* It crosses the three layers Talisman names —
  metadata (`dct:modified`), structure (step→role), accountability (`filledBy`→AIAgent) — and because
  every input is derived, the verdict **flips automatically** when you change a raw fact downstream:
  backdate the workflow's `Modified` past twelve months, *or* reassign the risk-analysis role from the
  AI agent to a human (one `filledBy` edge), and `IsAtComplianceRisk` recomputes four inference-hops away.

Every competency question in the article is answered by a **derived column or closure view** —
no sidecar code. To do that faithfully the model uses **load-bearing lookups** (`INDEX/MATCH`) where
the article relies on the role→agent indirection and the gate→role→approver chain. (Earlier revisions
were deliberately lookup-free; full article coverage made a small set of lookups load-bearing.)

---

## The Story: Production Deployment Workflow

Five ordered steps carry a release from request to post-deploy report:

| # | Step | Role | Filled by | Human approval? |
|---|------|------|-----------|-----------------|
| 1 | Initiate Deployment Request | Release Manager | Maria Gonzalez (human) | ✅ |
| 2 | AI Risk Assessment | Risk Analysis Agent | RiskAnalysis-AI (`risk-classifier-v2.4.1`) | ❌ |
| 3 | Legal Compliance Review & Release Authorization **(Approval Gate)** | Legal Compliance Reviewer | James Okafor (human) | ✅ |
| 4 | Automated Deployment Execution | CI/CD Executor | CI/CD Deploy Pipeline | ❌ |
| 5 | Post-Deployment Health Check & Report | Release Manager | Maria Gonzalez (human) | ✅ |

- **Ordering** (`StepPrecedence`): 1→2→3→4→5 asserted; transitive closure yields all 10 pairs.
- **Approval gate** (step 3): `EscalationThresholdHours = 4`.
- **Delegation**: Release Manager → VP Engineering (David Chen) → CTO (Sarah Kim).
- **Provenance**: risk report → legal clearance → release authorization → deployment log →
  post-deployment report.
- **Dataset**: *Q1 2026 Risk Metrics* (`DS-RISK-2026-Q1`) consumed by step 2.

---

## Entity Model

```
Workflows
    └── WorkflowSteps (many)            ntwf:hasStep / isStepOf
            ├── AssignedRole → Roles    ntwf:assignedRole (functional)
            ├── ConsumesDataset → Datasets
            ├── ProducesArtifacts ← WorkflowArtifacts.ProducedByStep
            ├── ApprovalGate ← ApprovalGates.WorkflowStep   (subtype, 1:1)
            └── Precedes / PrecededBy ← StepPrecedence       (step→step edges)

StepPrecedence (junction)
    ├── FromStep → WorkflowSteps        (predecessor)
    └── ToStep   → WorkflowSteps        (successor)        ntwf:precedesStep (transitive)

Roles
    ├── HasCapability → AgentCapabilityConcepts (SKOS)
    ├── FilledByHumanAgent → HumanAgents \
    ├── FilledByAIAgent → AIAgents        }  exactly ONE arm set (filledBy, functional + disjoint)
    ├── FilledByAutomatedPipeline → AutomatedPipelines /
    ├── OwnedBy → Departments           ntwf:ownedBy (functional)
    └── DelegatesTo → Roles (self-ref)  ntwf:delegatesTo (escalation)

WorkflowArtifacts
    ├── ProducedByStep → WorkflowSteps          prov:wasGeneratedBy
    ├── DerivedFromArtifact → WorkflowArtifacts prov:wasDerivedFrom (self-ref chain)
    └── AttributedTo{Human|AI|Pipeline}Agent    prov:wasAttributedTo

Workflows.WorkflowStatus → WorkflowStatusConcepts (SKOS)
```

---

## Tables Overview

| Table | Rows | Key Fields | Calculated Fields |
|-------|------|------------|-------------------|
| **Workflows** | 1 | Title, Description, Identifier, Modified | `Name` (slug), `CountOfNonProposedSteps`, `HasMoreThan1Step` |
| **WorkflowSteps** | 5 | SequencePosition, RequiresHumanApproval, StepDurationMinutes | `Name` (slug) |
| **ApprovalGates** | 1 | EscalationThresholdHours | `Name` (slug) |
| **StepPrecedence** | 4 | FromStep, ToStep | `Name` ("from → to") |
| **Roles** | 6 | Label, Comment | `Name` (slug) |
| **Departments** | 2 | Title | `Name` (slug) |
| **HumanAgents** | 4 | Name, Mbox | — |
| **AIAgents** | 1 | ModelVersion, Title | — |
| **AutomatedPipelines** | 1 | Description | — |
| **WorkflowStatusConcepts** | 4 | PrefLabel, Definition (SKOS) | — |
| **AgentCapabilityConcepts** | 5 | PrefLabel, Definition (SKOS) | — |
| **Datasets** | 1 | Title, Identifier, DistributionUrl (DCAT) | — |
| **WorkflowArtifacts** | 5 | Title, Created (PROV chain) | — |

(`__meta__` carries project-level metadata — tagline, motif, narrative, substrate list — as typed rows.)

---

## Competency Questions (acceptance criteria)

The curated ABox keeps all of these answerable:

- **CQ1** — Steps in order: walk `StepPrecedence` from step 1; closure gives 1→5.
- **CQ2** — Who approves a deployment: gate (step 3) → Legal Compliance Reviewer → James Okafor.
- **CQ3** — AI vs human steps: filter `RequiresHumanApproval` (1, 3, 5 human; 2, 4 not).
- **CQ4** — Artifact provenance + consumers: follow `DerivedFromArtifact` through all five artifacts.
- **CQ5** — Stale workflows: `Workflows.Modified` (Dublin Core `dct:modified`).
- **CQ6** — Escalation when a role is unavailable: traverse `DelegatesTo` (RM → VP Eng → CTO).
- **CQ7** — Workflows spanning Engineering **and** Legal: intersect `Roles.OwnedBy`.
- **CQ8** — Datasets the review consumes + which AI processed them: `ConsumesDataset` → step 2 → RiskAnalysis-AI.

> **Note on one small divergence from the article (for Jessica).** The article's worked
> example lists **six roles and one AI agent** (`risk-classifier-v2.4.1`), yet states that
> **CQ3 returns *two* AI-executed steps**. With a single AI agent those two facts can't both
> hold — one AI agent fills one role, which executes one step. To make CQ3 answer **2** as
> written, this rulebook adds a **seventh role** (`DeploymentHealth-Role`) and a **second AI
> agent** (`DeploymentHealth-AI`) for the post-deployment health report (step 5). We flag it
> here rather than quietly papering over it: the model matches every CQ *result* in the
> article; it just carries one role/agent more than the prose enumerates. Everything else is
> 1:1 with the named individuals.

---

## Key Formulas

### Slug generation (used across several tables)
```
=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
```
Generates: "Production Deployment" → "production-deployment"

### Step-count aggregation (`Workflows.CountOfNonProposedSteps`)
```
=COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}})
```
Counts how many steps belong to each workflow (no JOIN to write).

### Boolean derivation (`Workflows.HasMoreThan1Step`)
```
={{CountOfNonProposedSteps}} > 1
```
Reads a rollup and turns it into a flag — the smallest possible DAG (a rollup feeds a rule).

### Precedence-edge label (`StepPrecedence.Name`)
```
={{FromStep}} & " -> " & {{ToStep}}
```
Human-readable label for each step-to-step ordering edge.

---

## Design Patterns Demonstrated

### 1. Role–agent separation (Heuristic 2)
Steps point to **Roles**, never directly to agents. A personnel change or a model upgrade is a
single `filledBy` edge change; the workflow structure is untouched.

### 2. The three agent types are disjoint
`HumanAgent`, `AIAgent`, and `AutomatedPipeline` are separate tables (mutually disjoint classes).
Each role sets **exactly one** `FilledBy*` arm — matching `owl:FunctionalProperty` + disjointness.
A human-approval step may only be filled by a `HumanAgent`.

### 3. Approval gate as a step **subtype** (not a collapsed node)
`ApprovalGates` is its own first-class table. Each gate row *specializes* one `WorkflowStep` via a
1:1 FK (`WorkflowStep`) and adds only `EscalationThresholdHours`. This is class-table inheritance —
the relational form of `rdfs:subClassOf` — so the gate stays a distinct node in the DAG while still
being "a step."

### 4. First-class step-to-step ordering
`StepPrecedence` is a real junction (`FromStep → ToStep`), not a helper integer. Because
`ntwf:precedesStep` is transitive, the 4 asserted edges imply 10 ordered pairs (including the
never-asserted 1→5). Every edge is its own addressable node.

### 5. PROV provenance + DCAT datasets
Artifacts form a `wasDerivedFrom` chain and are `wasAttributedTo` their producing agent; datasets
are kept as `dcat:Dataset` separate from `prov:Entity` artifacts.

---

## Building & Testing

This is a **rulebook-first, Airtable-free** project. Edit
`effortless-rulebook/talisman-basic-rulebook.json` directly; everything else is derived.

```bash
effortless build
# or, from the repo root, the full generate + test + report pipeline:
ERB_DOMAIN=talisman-basic bash orchestration/orchestrate.sh
```

The build regenerates the Postgres substrate (`postgres-bootstrap/`), resets the dev database from
the rulebook seed data, regenerates answer keys, and grades every substrate for conformance — all
substrates must compute identically.

> **Dev is a clean slate every build.** All seed data is mock/demo data. The dev Postgres bootstrap
> runs `drop_all=true`, so `init-db.sh` resets the database back to exactly the rulebook seed on
> every build. In production the same rulebook would *add* rows, not drop — the only difference is
> the destructive reset. See `CLAUDE.md`.

---

## Files in This Directory

- `effortless-rulebook/talisman-basic-rulebook.json` — the rulebook (authoritative SSoT)
- `README.md` — this file
- `CLAUDE.md` — project rules for agents
- `bootstrap/jessica-talisman-abstract.md` — ~10 KB summary of the source article
- `postgres-bootstrap/` — generated Postgres schema, functions, views, seed data (do not hand-edit)
- `testing/` — generated answer keys and conformance results (do not hand-edit)

---

## Local transpiler bus (`localhost:4242`)

> **All 13 local transpilers live on `localhost:4242`.** Once you run
> `./start.sh` from the repo root, the ssotme-proxy exposes every repo-local
> transpiler — `rulebook-to-postgres`, `rulebook-to-python`, `rulebook-to-golang`,
> `rulebook-to-cobol`, `rulebook-to-owl`, and more — as first-class `ssotme://`
> routes any `effortless build` can call.
