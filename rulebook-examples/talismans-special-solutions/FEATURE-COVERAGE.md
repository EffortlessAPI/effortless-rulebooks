# NTWF Feature Coverage — Ontology Series (Parts I–IV) → Rulebook → UI

**What this is.** A complete inventory of every feature the four-part *Intentional Arrangement*
ontology series defines (Jessica Talisman, NTWF workflow ontology), cross-referenced against (a) the
**rulebook** (`effortless-rulebook/talismans-special-solutions-rulebook.json`, the SSoT) and (b) the
**UI** that demonstrates it. The goal is to know, for every element of the original plan, whether a
user can *see it work* in the app — not just whether it exists in the data.

**Date:** 2026-06-12 · **Model is unified I–IV** (the "Part IV" provenance seam was removed; commits
`f6ca8d6`, `aec8984`, `53bbe22`). 19 business tables; 1 continuous model.

## The two UI surfaces (this is the whole crux)

There are **two distinct UIs**, and a feature can be "in the UI" in two very different senses:

1. **Release Console** (`/console/flow|graph|closure`) — the *pedagogical* surface. Purpose-built,
   interactive, "edit a fact → watch the substrate recompute the board" demonstrations. This is
   where a feature is *demonstrated*, not just shown.
2. **Explainer-DAG browser** (`/dag`, `/dag/:table`, `/dag/:table/:field`) — the *generic* surface.
   It lists **every table in the rulebook** with full row data and a clickable derivation graph for
   every calculated field. So **all 19 tables are technically visible here as data** — but visibility
   in the DAG browser is *not* a pedagogical demonstration of the concept.

**Status legend** (graded against the Console, the pedagogical bar):

- ✅ **Present** — purpose-built, interactive demonstration in the Console.
- 🟡 **Needs improvement** — partially demonstrated (shown but not interactive, or a sub-aspect missing).
- 🟦 **Data-only** — exists in the rulebook and is browsable in the `/dag` Explainer, but has **no
  purpose-built Console demonstration**. The fact is computable and inspectable; the *story* isn't told.
- ❌ **Missing** — no UI demonstration of this element anywhere beyond raw row data.

---

## Part I — *The Architecture of Meaning* (what an ontology IS)

Part I is conceptual. Its "features" are claims the demo is meant to *embody* and make legible:
that this is a real ontology (classes + properties + axioms + reasoning), built on reused W3C
standards, that produces genuine inferences — not a schema, taxonomy, or config file.

| # | Feature / claim from Part I | Rulebook | UI status | Where / gap |
|---|---|---|---|---|
| I-1 | **Genuine inference is shown** (a new fact the reasoner derived, not asserted) | ✅ closure tables | ✅ **Present** | Closure view: asserted ● vs inferred ○; the never-asserted 1→5 pair is the headline |
| I-2 | **Reuse-before-invent / standards provenance** (PROV-O, FOAF, DCAT, Dublin Core, Schema.org, SKOS as building blocks) | ✅ (alignments live in OWL substrate) | ❌ **Missing** | Nothing in the UI tells the user *which* external standard each class/field comes from. No "this column = `prov:wasDerivedFrom`" mapping anywhere |
| I-3 | **RDFS vs OWL expressiveness** (disjointness, transitivity, functional props — things a schema can't say) | ✅ (disjoint agent types, transitive precedence, functional fillers) | 🟡 **Needs improvement** | Transitivity is demonstrated (closure); disjointness is *enforced* (consistency violation) but never *explained* as "these classes are `owl:disjointWith`" |
| I-4 | **"It's a model, not a schema/taxonomy/config"** — the multi-substrate equivalence is the proof | ✅ | ✅ **Present** | Conformance chip (reasoner vs Postgres agree N/N) + engine switch on the login/sync gate |
| I-5 | **Symbolic AI / "the ontology reasons, the substrate computes"** (the app computes nothing) | n/a (architectural) | ✅ **Present** | "reasoning…" veil on every edit; footer states db.json = facts, OWL+SHACL = computation |
| I-6 | **URIs/IRIs as global identifiers for things** | ✅ (`Iri` calc field on every table) | 🟦 **Data-only** | IRIs exist and render in `/dag` field pages, but no Console surface frames "every row is a globally-addressable individual" |

---

## Part II — *Design Heuristics* (the 6 heuristics + CQs + standards survey)

| # | Feature from Part II | Rulebook | UI status | Where / gap |
|---|---|---|---|---|
| II-1 | **Heuristic 2 — Role/Agent separation** (steps→roles→agents; reassign = one edge) | ✅ Roles.FilledBy*, Steps.AssignedRole | ✅ **Present** | Reassign popover: change the filler, board recomputes; the whole "one triple changes" story |
| II-2 | **Three disjoint agent types** (Human / AI / Pipeline) | ✅ 3 tables, mutually disjoint | ✅ **Present** | Agent-kind icons (🧑/🤖/⚙️) everywhere; AgentMix counts; reassign offers all three |
| II-3 | **`requiresHumanApproval` as a logical constraint** (human-only roles; violation if AI fills) | ✅ Steps.RequiresHumanApproval + ApprovalConsistencyViolation | ✅ **Present** | Toggle the flag / assign an AI to a human-gate → "⚠ rule broken" fires; confirm-dialog warns first |
| II-4 | **Heuristic 4 — domain/range as inference, not constraint** (the silent mis-typing trap) | ✅ (modelVersion domain = AIAgent, etc.) | ❌ **Missing** | Suite-4 "inject an error, watch a wrong inference" is the article's most instructive moment — no UI re-creates it (e.g. "apply modelVersion to a human → reasoner now thinks they're an AI") |
| II-5 | **The 8 Competency Questions as the contract** (CQ1–CQ8) | ✅ (every CQ has backing fields/views) | 🟡 **Needs improvement** | The app *answers* most CQs implicitly, but there is no **CQ panel** that lists the 8 questions and shows each one's live answer + pass/fail. This is the single highest-value pedagogical gap (see roundup below) |
| II-6 | **SKOS controlled vocabularies** (WorkflowStatus, AgentCapability schemes) | ✅ WorkflowStatusConcepts, AgentCapabilityConcepts | 🟦 **Data-only** | Both concept schemes exist & are browsable in `/dag`, but neither is demonstrated: `workflowStatus` isn't shown as a SKOS-grounded picker, capabilities never appear on roles |
| II-7 | **Heuristic 1 — standards reuse / "mostly assembly"** | ✅ | ❌ **Missing** | Same gap as I-2: no UI shows the borrowed-vs-custom term boundary |
| II-8 | **Heuristic 6 — documentation (label/comment) before axioms** | ✅ Roles.Label/Comment; concept Definition/ScopeNote | 🟦 **Data-only** | Labels/comments/definitions exist as fields; visible in `/dag`; not surfaced as hovercards/tooltips in the Console |

---

## Part III — *From Framework to Formal Model* (the build: 9 classes, 13 obj props, 5 data props, ABox, 104 tests)

This is the densest part. Every custom term and the validation suite.

### Classes (9)

| Class | Rulebook table | UI status | Where / gap |
|---|---|---|---|
| `ntwf:Workflow` (⊑ prov:Plan, schema:CreativeWork) | ✅ Workflows | ✅ **Present** | The whole console is one workflow; title/verdict/staleness all shown |
| `ntwf:WorkflowStep` (⊑ prov:Activity) | ✅ WorkflowSteps | ✅ **Present** | Step cards in Flow; nodes in Graph/Closure |
| `ntwf:ApprovalGate` (⊑ WorkflowStep, double-typed) | ✅ ApprovalGates | 🟡 **Needs improvement** | Gate shows as a 🔒 badge; but `EscalationThresholdHours`, `GateRole`, `GateApproverHuman` are **not shown** — the gate's defining properties are invisible |
| `ntwf:WorkflowArtifact` (⊑ prov:Entity, schema:CreativeWork) | ✅ WorkflowArtifacts (5-row chain) | 🟦 **Data-only** | The 5-artifact `wasDerivedFrom` provenance chain is fully modeled & browsable in `/dag`, but **never drawn** in the Console (no provenance lens). High-value gap |
| `ntwf:Role` (root class, disjoint w/ Step & Artifact) | ✅ Roles | ✅ **Present** | Reassign popover, role labels on steps |
| `ntwf:HumanAgent` (⊑ foaf:Person) | ✅ HumanAgents | ✅ **Present** | Team, avatars, reassign |
| `ntwf:AIAgent` (⊑ prov:SoftwareAgent) | ✅ AIAgents | 🟡 **Needs improvement** | AI agents shown; but `ModelVersion` / `DeployedOn` (the audit-grade identity Part IV leans on) **not surfaced** |
| `ntwf:AutomatedPipeline` (⊑ prov:SoftwareAgent, schema:SoftwareApplication) | ✅ AutomatedPipelines | ✅ **Present** | Pipeline kind icon + reassign |
| `ntwf:Department` (⊑ schema:Organization) | ✅ Departments | 🟦 **Data-only** | Departments exist; `OwnedBy` resolves on steps (IsLegalOwned/IsEngineeringOwned are computed) — but **no department/org view**. OrgView.tsx exists but is **dead/unrouted code** |

### Object properties (13)

| Property | Rulebook | UI status | Where / gap |
|---|---|---|---|
| `hasStep` / `isStepOf` (inverse pair) | ✅ Workflow↔Steps | ✅ **Present** | Steps belong to the workflow throughout |
| `precedesStep` (**transitive**) + closure | ✅ StepPrecedence + closure view | ✅ **Present** | **The flagship.** Closure view: assert 4 edges → 10-pair closure, asserted vs inferred |
| `assignedRole` (functional) | ✅ Steps.AssignedRole | ✅ **Present** | Role label on each step |
| `filledBy` (the change-mgmt triple) | ✅ Roles.FilledBy{Human,AI,Pipeline} | ✅ **Present** | Reassign popover — *the* one-edge-changes demo |
| `ownedBy` (role→department) | ✅ Roles.OwnedBy | 🟦 **Data-only** | Computed (IsLegalOwned etc.) but not shown; feeds CQ7 which has no panel |
| `delegatesTo` (**transitive**, escalation chain) + closure | ✅ Roles.DelegatesTo + DelegationClosure | 🟦 **Data-only** | Chain (Release Mgr→VP Eng→CTO) is modeled, closured, and **delivered to the frontend** (`story.delegation`), but the only view that draws it (OrgView) is **unrouted**. So it's computed-and-shipped but **not on screen** |
| `producesArtifact` / `isProducedBy` (inverse) | ✅ WorkflowArtifacts.ProducedByStep | 🟦 **Data-only** | Part of the un-drawn provenance chain |
| `requiresArtifact` | ✅ WorkflowArtifacts.RequiredBySteps | 🟦 **Data-only** | Same |
| `consumesDataset` (→ dcat:Dataset) | ✅ Steps.ConsumesDataset, Datasets | 🟦 **Data-only** | Dataset is in `options.datasets` but **never rendered**; CQ8 ("what datasets does the review consume") has no surface |
| `hasCapability` (→ SKOS concept) | ✅ Roles.HasCapability | 🟦 **Data-only** | Capability tags never shown on roles |
| `workflowStatus` (→ SKOS concept, functional) | ✅ Workflows.WorkflowStatus | 🟡 **Needs improvement** | Status is carried in the payload but the verdict shows *compliance*, not *lifecycle status*; no SKOS-grounded status pill/picker |
| `wasAttributedTo` (artifact→agent, PROV) | ✅ Artifacts.AttributedTo{Human,AI,Pipeline} | 🟦 **Data-only** | Drives AIAgents.CountAttributedArtifacts / CountImpactedWorkflows (the Part-IV "blast radius") — computed, not drawn |
| `wasDerivedFrom` (artifact lineage, PROV) | ✅ Artifacts.DerivedFromArtifact | 🟦 **Data-only** | The 4 lineage links — modeled, not drawn |

### Datatype properties (5)

| Property | Rulebook | UI status | Where / gap |
|---|---|---|---|
| `sequencePosition` (functional int) | ✅ | ✅ **Present** | Step number on every card/node |
| `requiresHumanApproval` (bool) | ✅ | ✅ **Present** | Badge + toggle |
| `modelVersion` (AI version string) | ✅ AIAgents.ModelVersion | ❌ **Missing** | Not shown anywhere in Console; the audit-version story is invisible |
| `stepDurationMinutes` | ✅ | ✅ **Present** | TimeBudgetBar segments (draggable) |
| `escalationThresholdHours` (gate only) | ✅ ApprovalGates.EscalationThresholdHours | ❌ **Missing** | The gate's escalation threshold is never shown — and it's the natural bridge to the delegation chain |

### The ABox + the 104-test validation suite (Suites 1–4)

| Feature | Rulebook | UI status | Where / gap |
|---|---|---|---|
| ABox exercises **every** class/prop (1 workflow, 5 steps, 3 agent types, 7 roles, 5 artifacts, 1 dataset, 1 gate) | ✅ | 🟡 **Needs improvement** | Seed data exists and is complete; but the *claim* "this single example touches every term" is never made visible (no coverage map) |
| **Suite 2** — OWL-RL closure (433 → 800+ triples; 23 inferences) | ✅ | 🟡 **Needs improvement** | `reasoned_triples` count is shown in the footer; the *expansion* (asserted→entailed) is only shown for precedence, not for type inferences |
| **Suite 3** — the 8 CQs as queries, all passing | ✅ | 🟡 **Needs improvement** | See II-5 — **no CQ scoreboard.** CQ1/CQ3 partially demonstrated; CQ2/4/5/6/7/8 have no dedicated answer surface |
| **Suite 4** — inject-an-error → detect inconsistency (disjointness, functional sameAs, domain mis-inference) | ✅ (HasConsistencyViolation, HasExactlyOneFiller, CountRolesWithBadFillerCardinality exist) | ❌ **Missing** | Only the human-approval consistency violation is demonstrable. The disjoint-type violation, the functional-`assignedRole` double-assignment `sameAs`, and the domain-mis-inference traps are **not** demonstrable in the UI |

---

## Part IV — *Governance, Maintenance, and AI*

| # | Feature from Part IV | Rulebook | UI status | Where / gap |
|---|---|---|---|---|
| IV-1 | **Stale-workflow query** (CQ5: `dct:modified` > 12mo) | ✅ Workflows.IsStale/MonthsSinceModified | ✅ **Present** | StalenessBar — draggable, with the renewal/threshold leading edge |
| IV-2 | **Stale-AND-has-AI** (the higher-compliance-risk join) | ✅ Workflows.IsStaleAndHasAIAgent | ✅ **Present** | Folded into the compliance verdict |
| IV-3 | **Steward vs Authority governance roles** | ✅ GovernanceRoles (2 rows, CanApproveChanges) | 🟦 **Data-only** | Browsable in `/dag`; no governance panel in the Console |
| IV-4 | **Semantic versioning + ChangeLog** (patch/minor/major, breaking vs back-compat) | ✅ ChangeLog (IsBreakingChange, IsBackwardCompatible) | 🟦 **Data-only** | Two version events modeled & browsable; no version-history timeline in the Console |
| IV-5 | **External-dependency reconciliation** (`foaf:name owl:sameAs ntwf:name`, DCAT v2→v3) | ✅ VocabularyReconciliations (2 rows) | 🟦 **Data-only** | Modeled & browsable; not demonstrated |
| IV-6 | **Bitemporal `filledBy` history** (don't delete the old triple — timestamp & retain; "who filled this on 2026-03-01?") | ✅ RoleAssignments (ValidFrom/ValidTo, WasActiveAsOfAuditDate, IsAgentTypeChange, RequiresComplianceAudit) | 🟦 **Data-only** | Fully modeled (6 assignment rows, audit-date logic) and browsable — but **no time-travel UI**. The "as-of date" slider that would make this sing does not exist |
| IV-7 | **AI-system-turnover audit** (agent-type change AI→Human triggers compliance audit) | ✅ RoleAssignments.RequiresComplianceAudit + Workflows.CountComplianceAuditChanges | 🟦 **Data-only** | Computed; no surface |
| IV-8 | **NTWF graph as AI-system registry / "blast radius"** (upgrade risk-classifier → which artifacts/steps/workflows are impacted?) | ✅ AIAgents.CountAttributedArtifacts / CountImpactedWorkflows | 🟦 **Data-only** | The blast-radius numbers are **computed** but never shown; this is a flagship Part-IV demo with no UI |
| IV-9 | **ModelVersion as audit anchor** | ✅ AIAgents.ModelVersion/DeployedOn | ❌ **Missing** | See III datatype props — not surfaced |
| IV-10 | **Ontology-is-ground-truth / GraphRAG-ready** (URIs legible, DC labels, provenance traceable) | ✅ (IRIs, titles, identifiers everywhere) | 🟡 **Needs improvement** | The substrate exists; no UI frames it as "this is what an LLM would query" |

---

## The gap list — features that need a UI to reach 100% of the series

Ordered by pedagogical value (highest first). Each is a concrete thing to build in the **Console**
(the `/dag` browser already shows all the underlying data).

1. **Competency-Question scoreboard** (CQ1–CQ8). A panel listing the 8 leadership questions, each
   with its live answer and a ✓. This is the article's literal "acceptance test suite" and is the
   single biggest missing surface. *(covers II-5, III-Suite-3, and ties together CQ2/4/5/6/7/8)*
2. **Provenance / artifact-lineage lens** — draw the 5-artifact `wasDerivedFrom` chain with
   `wasAttributedTo` agent links. *(covers III WorkflowArtifact, producesArtifact, wasDerivedFrom, wasAttributedTo; answers CQ4)*
3. **Delegation/escalation view** — route `OrgView.tsx` (currently **dead code**) or build a fresh
   one; show the Release Mgr→VP Eng→CTO chain with asserted vs inferred (delegatesTo closure).
   *(covers III delegatesTo; answers CQ6; pairs with the gate's escalation threshold)*
4. **"Blast radius" / AI-system-registry panel** — pick an AI agent, show CountAttributedArtifacts +
   CountImpactedWorkflows + the downstream steps/workflows. *(covers IV-8, IV-9, modelVersion)*
5. **Approval-gate detail** — surface `EscalationThresholdHours`, `GateRole`, `GateApproverHuman` on
   the gate (today it's just a 🔒 badge). *(covers III ApprovalGate properties; answers CQ2 fully)*
6. **Bitemporal "as-of date" time-travel** — a date slider that re-filters RoleAssignments by
   ValidFrom/ValidTo and shows who filled each role then; flag agent-type changes needing audit.
   *(covers IV-6, IV-7 — the entire governance-history story)*
7. **Inject-an-error / consistency lab** (Suite 4) — let the user create a disjoint-type individual,
   a double-`assignedRole`, or a domain-mis-inference and watch the reasoner flag it. *(covers II-4, III-Suite-4)*
8. **Dataset (DCAT) surface** — show which steps consume which datasets, with distribution URL.
   *(covers III consumesDataset; answers CQ8)*
9. **SKOS vocabulary surfaces** — `workflowStatus` as a grounded status pill/picker; `hasCapability`
   tags on roles; concept definitions/scope-notes as hovercards. *(covers II-6, II-8, III hasCapability/workflowStatus)*
10. **Department / cross-cutting view** — which workflows touch both Engineering and Legal, and at
    which steps. *(covers III Department, ownedBy; answers CQ7)*
11. **Governance panel** — Steward vs Authority, the ChangeLog version timeline (breaking vs
    back-compat), and the vocabulary reconciliations. *(covers IV-3, IV-4, IV-5)*
12. **Standards-provenance overlay** — for any column/field, show the borrowed term it maps to
    (`prov:wasDerivedFrom`, `foaf:name`, `dcat:Dataset`, …) vs custom `ntwf:`. *(covers I-2, II-7, the "mostly assembly" claim)*
13. **Type-inference expansion** — visualize the OWL-RL closure beyond precedence: an individual's
    asserted type → entailed supertypes (HumanAgent→foaf:Person→prov:Agent). *(covers I-3, III-Suite-2)*
14. **ModelVersion / DeployedOn on AI agents** — small, but currently invisible. *(covers III-modelVersion, IV-9)*

### Scorecard

| Bucket | ✅ Present | 🟡 Needs improvement | 🟦 Data-only (browsable, not demonstrated) | ❌ Missing |
|---|---|---|---|---|
| Count (of graded rows above) | ~14 | ~9 | ~19 | ~7 |

**Bottom line.** The **rulebook is complete** — every class, property, axiom, CQ, governance and
provenance element from all four parts is modeled, computed, and at minimum *browsable* in the
`/dag` Explainer. The **Console** demonstrates the workflow spine brilliantly (role/agent separation,
the three agent types, the human-approval rule, transitive precedence closure, staleness,
time-budget, multi-substrate conformance). What it does **not yet demonstrate** is the
**provenance/PROV layer, the delegation chain (built but unrouted), the DCAT/SKOS layers, and the
entire Part-IV governance/bitemporal/AI-registry story** — all of which exist as data but have no
purpose-built, interactive "watch it work" surface. Items 1–6 above close the highest-value gaps.
