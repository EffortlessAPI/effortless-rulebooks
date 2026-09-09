# 📘 effortless-math — RuleSpeak®

_Executable theorem network starter. Fermat's Last Theorem is the deeply modeled flagship domain; seven foundation theorems begin as first-class provider contracts with imported universal content._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Domain** | A domain is identified by its name. | — |
| Name | The same as its domain ID. | _Display label, mirrors DomainId._ |
| Slug | A defined attribute. | _Slug_ |
| Title | A defined attribute. | _Title_ |
| Role | A defined attribute. | _Role_ |
| Status | A defined attribute. | _Status_ |
| Description | A defined attribute. | _Description_ |
| Canonical Theorem ID | A defined attribute. | _CanonicalTheoremId_ |
| Relative Path | A defined attribute. | _RelativePath_ |
| Is Flagship | True when an empty string. | _IsFlagship_ |
| **Proof Status** | A proof status is identified by its name. | — |
| Name | The same as its proof status ID. | _Name_ |
| Sort Order | A defined attribute. | _SortOrder_ |
| Is Imported | True when an empty string. | _IsImported_ |
| Is Derived | True when an empty string. | _IsDerived_ |
| Allows Imported Children | True when an empty string. | _AllowsImportedChildren_ |
| Is Terminal | True when an empty string. | _IsTerminal_ |
| Description | A defined attribute. | _Description_ |
| **Theorem** | A theorem is identified by its name. | — |
| Name | The same as its theorem ID. | _Name_ |
| Domain ID | A defined attribute. | _DomainId_ |
| Title | A defined attribute. | _Title_ |
| Theorem Kind | A defined attribute. | _TheoremKind_ |
| Statement | A defined attribute. | _Statement_ |
| Scope | A defined attribute. | _Scope_ |
| Proof Status ID | A defined attribute. | _ProofStatusId_ |
| Trust Boundary | A defined attribute. | _TrustBoundary_ |
| Active Import Count | A defined attribute. | _ActiveImportCount_ |
| Root Fact ID | A defined attribute. | _RootFactId_ |
| Contract Path | A defined attribute. | _ContractPath_ |
| Current Version | A defined attribute. | _CurrentVersion_ |
| Is Flagship | True when an empty string. | _IsFlagship_ |
| Next Target | A defined attribute. | _NextTarget_ |
| **Theorem Dependency** | A theorem dependency is identified by its name. | — |
| Name | The same as its dependency ID. | _Name_ |
| Consumer Theorem ID | A defined attribute. | _ConsumerTheoremId_ |
| Provider Theorem ID | A defined attribute. | _ProviderTheoremId_ |
| Required Conclusion | A defined attribute. | _RequiredConclusion_ |
| Proof Status ID | A defined attribute. | _ProofStatusId_ |
| Load Bearing | True when an empty string. | _LoadBearing_ |
| Is Imported | True when an empty string. | _IsImported_ |
| Is Shared Kernel | True when an empty string. | _IsSharedKernel_ |
| Provider Contract Path | A defined attribute. | _ProviderContractPath_ |
| Notes | A defined attribute. | _Notes_ |
| **Foundation Kernel** | A foundation kernel is identified by its name. | — |
| kernel order | A defined attribute. | _kernel_order_ |
| Name | The same as its kernel ID. | _Name_ |
| title | A defined attribute. | _title_ |
| mathematical scope | A defined attribute. | _mathematical_scope_ |
| input contract JSON | A defined attribute. | _input_contract_json_ |
| output contract JSON | A defined attribute. | _output_contract_json_ |
| finite compiler tables JSON | A defined attribute. | _finite_compiler_tables_json_ |
| source ID | A defined attribute. | _source_id_ |
| theorem content internalized | A defined attribute. | _theorem_content_internalized_ |
| status | A defined attribute. | _status_ |
| next target | A defined attribute. | _next_target_ |
| **Proof Fact** | A proof fact is identified by its name. | — |
| Name | The same as its fact ID. | _Name_ |
| Step No | A defined attribute. | _StepNo_ |
| Rule ID | A defined attribute. | _RuleId_ |
| Derived Fact | A defined attribute. | _DerivedFact_ |
| Antecedents JSON | A defined attribute. | _AntecedentsJson_ |
| Derivation Kind | A defined attribute. | _DerivationKind_ |
| Contradiction | True when an empty string. | _Contradiction_ |
| Notes | A defined attribute. | _Notes_ |
| Domain ID | A defined attribute. | _DomainId_ |
| **Loop** | A loop is identified by its name. | — |
| Name | The same as its loop ID. | _Name_ |
| Loop Order | A defined attribute. | _LoopOrder_ |
| Title | A defined attribute. | _Title_ |
| Status | A defined attribute. | _Status_ |
| New Concept | A defined attribute. | _NewConcept_ |
| Domain Question | A defined attribute. | _DomainQuestion_ |
| Witness Summary | A defined attribute. | _WitnessSummary_ |
| Next Frontier | A defined attribute. | _NextFrontier_ |
| Epistemic Tier | A defined attribute. | _EpistemicTier_ |
| Domain ID | A defined attribute. | _DomainId_ |
| **Invariant Check** | An invariant check is identified by its name. | — |
| Name | The same as its invariant check ID. | _Name_ |
| Description | A defined attribute. | _Description_ |
| Tier | A defined attribute. | _Tier_ |
| Pass Count | A defined attribute. | _PassCount_ |
| Fail Count | A defined attribute. | _FailCount_ |
| Universe Count | A defined attribute. | _UniverseCount_ |
| Status | A defined attribute. | _Status_ |
| Evidence | A defined attribute. | _Evidence_ |
| Protects Loop | A defined attribute. | _ProtectsLoop_ |
| Domain ID | A defined attribute. | _DomainId_ |
| **Source** | A source is identified by its name. | — |
| Name | The same as its source ID. | _Name_ |
| Citation | A defined attribute. | _Citation_ |
| Source URL | A defined attribute. | _SourceUrl_ |
| Source Kind | A defined attribute. | _SourceKind_ |
| Role | A defined attribute. | _Role_ |
| **Trust Boundary** | A trust boundary is identified by its name. | — |
| Name | The same as its trust boundary ID. | _Name_ |
| Theorem ID | A defined attribute. | _TheoremId_ |
| Boundary Kind | A defined attribute. | _BoundaryKind_ |
| Accepted External Claims | A defined attribute. | _AcceptedExternalClaims_ |
| Kernel or Substrate | A defined attribute. | _KernelOrSubstrate_ |
| Can Claim Zero Imports | True when an empty string. | _CanClaimZeroImports_ |
| Notes | A defined attribute. | _Notes_ |
| **Artifact Registry** | An artifact registry is identified by its name. | — |
| Name | The same as its artifact ID. | _Name_ |
| Domain ID | A defined attribute. | _DomainId_ |
| Artifact Kind | A defined attribute. | _ArtifactKind_ |
| Relative Path | A defined attribute. | _RelativePath_ |
| Version | A defined attribute. | _Version_ |
| Sha256 | A defined attribute. | _Sha256_ |
| Is Canonical Migration Source | True when an empty string. | _IsCanonicalMigrationSource_ |
| Notes | A defined attribute. | _Notes_ |
| **Migration Mapping** | A migration mapping is identified by its name. | — |
| Name | The same as its mapping ID. | _Name_ |
| Source Table | A defined attribute. | _SourceTable_ |
| Target Table | A defined attribute. | _TargetTable_ |
| Strategy | A defined attribute. | _Strategy_ |
| Notes | A defined attribute. | _Notes_ |
| **Project Roadmap** | A project roadmap is identified by its name. | — |
| Name | The same as its roadmap ID. | _Name_ |
| Sequence No | A defined attribute. | _SequenceNo_ |
| Title | A defined attribute. | _Title_ |
| Status | A defined attribute. | _Status_ |
| Acceptance Criteria | A defined attribute. | _AcceptanceCriteria_ |
| Depends on | A defined attribute. | _DependsOn_ |
| **Conclusion** | A conclusion is identified by its name. | — |
| Name | The same as its conclusion ID. | _Name_ |
| Category | A defined attribute. | _Category_ |
| Status | A defined attribute. | _Status_ |
| Report Tier | A defined attribute. | _ReportTier_ |
| Title | A defined attribute. | _Title_ |
| Evidence | A defined attribute. | _Evidence_ |
| Witnessed in Loop | A defined attribute. | _WitnessedInLoop_ |
| **Legacy Parent Audit** | A legacy parent audit is identified by its name. | — |
| edge order | A defined attribute. | _edge_order_ |
| Name | The same as its edge ID. | _Name_ |
| previous status | A defined attribute. | _previous_status_ |
| current status | A defined attribute. | _current_status_ |
| parent removed | A defined attribute. | _parent_removed_ |
| replacement kernel ID | A defined attribute. | _replacement_kernel_id_ |
| kernel shared with parent count | A defined attribute. | _kernel_shared_with_parent_count_ |
| theorem content fully internalized | A defined attribute. | _theorem_content_fully_internalized_ |
| status | A defined attribute. | _status_ |
| next target | A defined attribute. | _next_target_ |

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A domain **must** have a slug, a title, a role, a status, a description, a canonical theorem ID, and a relative path, and record whether it is a flagship.
- A proof status **must** have a sort order and a description, and record whether it is imported, whether it is derived, whether it is allows imported children, and whether it is a terminal.
- A theorem **must** have a domain ID, a title, a theorem kind, a statement, a scope, a proof status ID, a trust boundary, an active import count, a root fact ID, a contract path, a current version, and a next target, and record whether it is a flagship.
- A theorem dependency **must** have a consumer theorem ID, a provider theorem ID, a required conclusion, a proof status ID, a provider contract path, and a notes, and record whether it is load bearing, whether it is imported, and whether it is a shared kernel.
- A foundation kernel **must** have a kernel order, a title, a mathematical scope, an input contract JSON, an output contract JSON, a finite compiler tables JSON, a source ID, a theorem content internalized, a status, and a next target.
- A proof fact **must** have a step no, a rule ID, a derived fact, an antecedents JSON, a derivation kind, and a domain ID, and record whether it is contradiction.
- A loop **must** have a loop order, a title, a status, a new concept, a domain question, a witness summary, a next frontier, an epistemic tier, and a domain ID.
- An invariant check **must** have a description, a tier, a pass count, a fail count, an universe count, a status, an evidence, and a domain ID.
- A source **must** have a citation, a source URL, a source kind, and a role.
- A trust boundary **must** have a theorem ID, a boundary kind, an accepted external claims, a kernel or substrate, and a notes, and record whether it can claim zero imports.
- An artifact registry **must** have a domain ID, an artifact kind, a relative path, a version, a sha256, and a notes, and record whether it is a canonical migration source.
- A migration mapping **must** have a source table, a target table, a strategy, and a notes.
- A project roadmap **must** have a sequence no, a title, a status, an acceptance criteria, and a depends on.
- A conclusion **must** have a category, a status, a report tier, a title, an evidence, and a witnessed in loop.
- A legacy parent audit **must** have an edge order, a previous status, a current status, a parent removed, a replacement kernel ID, a kernel shared with parent count, a theorem content fully internalized, a status, and a next target.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A domain's name is the same as its domain ID. |
| **DR-2 Name** | A proof status's name is the same as its proof status ID. |
| **DR-3 Name** | A theorem's name is the same as its theorem ID. |
| **DR-4 Name** | A theorem dependency's name is the same as its dependency ID. |
| **DR-5 Name** | A foundation kernel's name is the same as its kernel ID. |
| **DR-6 Name** | A proof fact's name is the same as its fact ID. |
| **DR-7 Name** | A loop's name is the same as its loop ID. |
| **DR-8 Name** | An invariant check's name is the same as its invariant check ID. |
| **DR-9 Name** | A source's name is the same as its source ID. |
| **DR-10 Name** | A trust boundary's name is the same as its trust boundary ID. |
| **DR-11 Name** | An artifact registry's name is the same as its artifact ID. |
| **DR-12 Name** | A migration mapping's name is the same as its mapping ID. |
| **DR-13 Name** | A project roadmap's name is the same as its roadmap ID. |
| **DR-14 Name** | A conclusion's name is the same as its conclusion ID. |
| **DR-15 Name** | A legacy parent audit's name is the same as its edge ID. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Domains.Name** | formula | `DomainId` |
| **ProofStatuses.Name** | formula | `ProofStatusId` |
| **Theorems.Name** | formula | `TheoremId` |
| **TheoremDependencies.Name** | formula | `DependencyId` |
| **FoundationKernels.Name** | formula | `kernel_id` |
| **ProofFacts.Name** | formula | `FactId` |
| **Loops.Name** | formula | `LoopId` |
| **InvariantChecks.Name** | formula | `InvariantCheckId` |
| **Sources.Name** | formula | `SourceId` |
| **TrustBoundaries.Name** | formula | `TrustBoundaryId` |
| **ArtifactRegistry.Name** | formula | `ArtifactId` |
| **MigrationMappings.Name** | formula | `MappingId` |
| **ProjectRoadmap.Name** | formula | `RoadmapId` |
| **Conclusions.Name** | formula | `ConclusionId` |
| **LegacyParentAudits.Name** | formula | `edge_id` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
