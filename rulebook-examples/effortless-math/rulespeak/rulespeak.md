# 📘 effortless-math — Docs

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

<details open>
<summary>Example data</summary>

| Name ƒ | Slug | Title | Role | Status | Description | Canonical Theorem ID | Relative Path | Is Flagship |
|---|---|---|---|---|---|---|---|---|
| domain-fermats-last-theorem | fermats-last-theorem | Fermat's Last Theorem | flagship-consumer | deeply-modeled | Flagship theorem network migrated from Fermat Witness Lab v21. | fermats-last-theorem | domains/fermats-last-theorem | true |
| domain-analytic-prime-distribution | analytic-prime-distribution | Analytic Prime Distribution / Chebotarev | provider-theorem | foundation-stub | Chebotarev density for finite Galois quotients. | analytic-prime-distribution | domains/analytic-prime-distribution | false |
| domain-hilbert-specialization | hilbert-specialization | Hilbert Specialization | provider-theorem | foundation-stub | Hilbert irreducibility with simultaneous weak approximation. | hilbert-specialization | domains/hilbert-specialization | false |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Proof Status** | A proof status is identified by its name. | — |
| Name | The same as its proof status ID. | _Name_ |
| Sort Order | A defined attribute. | _SortOrder_ |
| Is Imported | True when an empty string. | _IsImported_ |
| Is Derived | True when an empty string. | _IsDerived_ |
| Allows Imported Children | True when an empty string. | _AllowsImportedChildren_ |
| Is Terminal | True when an empty string. | _IsTerminal_ |
| Description | A defined attribute. | _Description_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Sort Order | Is Imported | Is Derived | Allows Imported Children | Is Terminal | Description |
|---|---|---|---|---|---|---|
| PLANNED | 10 | false | false | true | false | The theorem or edge has a declared contract but no active proof object yet. |
| IMPORTED | 20 | true | false | true | false | The conclusion is accepted from an external theorem or trusted adapter. |
| DECOMPOSED | 30 | true | false | true | false | A theorem-sized import has been split into typed child obligations. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Domain ID | Title | Theorem Kind | Statement | Scope | Proof Status ID | Trust Boundary | Active Import Count | Root Fact ID | Contract Path | Current Version | Is Flagship | Next Target |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| fermats-last-theorem | domain-fermats-last-theorem | Fermat's Last Theorem | application-theorem | For every integer n>2, there are no nonzero integers x,y,z with x^n+y^n=z^n. | The current instrument models the prime-exponent p>=5 Frey/Wiles/Ribet route and its reductions. | DERIVED_WITH_IMPORTED_CHILDREN | Seven foundation provider theorems remain imported in v21. | 7 | contradiction | domains/fermats-last-theorem/theorem-contract.json | 21.0.0-migration | true | Replace provider imports one theorem repository at a time while rebuilding the complete theorem graph. |
| analytic-prime-distribution | domain-analytic-prime-distribution | Analytic Prime Distribution / Chebotarev | foundation-provider | Chebotarev density for finite Galois quotients. | Provider contract imported from the v21 foundation frontier. | IMPORTED | External foundation theorem; finite compiler interfaces are present but the universal theorem is not internalized. | 1 | analytic_chebotarev_density_available | domains/analytic-prime-distribution/theorem-contract.json | 0.1.0 | false | Internalize the analytic prime-ideal theorem and Artin-L-function argument. |
| hilbert-specialization | domain-hilbert-specialization | Hilbert Specialization | foundation-provider | Hilbert irreducibility with simultaneous weak approximation. | Provider contract imported from the v21 foundation frontier. | IMPORTED | External foundation theorem; finite compiler interfaces are present but the universal theorem is not internalized. | 1 | hilbert_irreducibility_weak_approximation_available | domains/hilbert-specialization/theorem-contract.json | 0.1.0 | false | Internalize universal thin-set avoidance. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Consumer Theorem ID | Provider Theorem ID | Required Conclusion | Proof Status ID | Load Bearing | Is Imported | Is Shared Kernel | Provider Contract Path | Notes |
|---|---|---|---|---|---|---|---|---|---|
| dep-flt-analytic-prime-distribution | fermats-last-theorem | analytic-prime-distribution | positive density and representatives in every class | IMPORTED | true | true | false | domains/analytic-prime-distribution/theorem-contract.json | Chebotarev density for finite Galois quotients. |
| dep-flt-hilbert-specialization | fermats-last-theorem | hilbert-specialization | rational specialization outside thin sets satisfying every local open | IMPORTED | true | true | false | domains/hilbert-specialization/theorem-contract.json | Hilbert irreducibility with simultaneous weak approximation. |
| dep-flt-mazur-modular-curve-arithmetic | fermats-last-theorem | mazur-modular-curve-arithmetic | rational point forced to the cusp; rational isogeny exclusion | IMPORTED | true | true | false | domains/mazur-modular-curve-arithmetic/theorem-contract.json | Formal immersion at the cusp plus finiteness of the Eisenstein quotient. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| kernel order | Name ƒ | title | mathematical scope | input contract JSON | output contract JSON | finite compiler tables JSON | source ID | theorem content internalized | status | next target |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | kernel-prime-distribution-v21 | Analytic prime distribution | Chebotarev density for finite Galois quotients. | ["finite quotient", "complete conjugacy classes", "Frobenius classifier"] | ["positive density and representatives in every class"] | ["finite_galois_class_profiles_v19", "prime_count_density_stages_v19"] | src-prime-distribution-kernel-v21 | 0 | ACTIVE-FOUNDATION-KERNEL | Internalize the analytic prime-ideal theorem and Artin-L-function argument. |
| 2 | kernel-hilbert-specialization-v21 | Hilbert specialization | Hilbert irreducibility with simultaneous weak approximation. | ["rational parameter space", "thin bad covers", "local open conditions"] | ["rational specialization outside thin sets satisfying every local open"] | ["thin_set_profiles_v19", "local_open_constraints_v19", "crt_specialization_certificates_v19"] | src-hilbert-specialization-kernel-v21 | 0 | ACTIVE-FOUNDATION-KERNEL | Internalize universal thin-set avoidance. |
| 3 | kernel-mazur-formal-immersion-v21 | Mazur modular-curve arithmetic | Formal immersion at the cusp plus finiteness of the Eisenstein quotient. | ["cyclic isogeny degree ledger", "cusp specialization", "tangent rank"] | ["rational point forced to the cusp", "rational isogeny exclusion"] | ["cyclic_isogeny_degree_ledger_v19", "formal_immersion_tangent_certificates_v19", "frey_local_specialization_profiles_v19"] | src-mazur-modular-curve-kernel-v21 | 0 | ACTIVE-FOUNDATION-KERNEL | Internalize the Eisenstein quotient and formal-immersion theorem. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Step No | Rule ID | Derived Fact | Antecedents JSON | Derivation Kind | Contradiction | Notes | Domain ID |
|---|---|---|---|---|---|---|---|---|
| proof-step-001 | 1 | v10-01 | normalized_signed_flt_solution | ["hypothetical_primitive_flt_solution_p_ge_5"] | elementary | false | Normalization. | domain-fermats-last-theorem |
| proof-step-002 | 2 | v10-02 | frey_curve_exists | ["normalized_signed_flt_solution"] | definition | false | Frey construction. | domain-fermats-last-theorem |
| proof-step-003 | 3 | v10-03 | frey_minimal_model_exists | ["normalized_signed_flt_solution"] | algebraic | false | Integral minimal model. | domain-fermats-last-theorem |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Loop Order | Title | Status | New Concept | Domain Question | Witness Summary | Next Frontier | Epistemic Tier | Domain ID |
|---|---|---|---|---|---|---|---|---|---|
| loop-01 | 1 | Equation space | complete | FermatEquationCase | What counts as a nontrivial integer solution of x^n + y^n = z^n? | Positive controls for n=2 and near-misses for n>2 are stored as explicit rows. | Add signed and projective normalizations. | definition | domain-fermats-last-theorem |
| loop-02 | 2 | Exponent reduction | complete | ExponentReductionRule | Why is it enough to handle exponent 4 and odd prime exponents? | Composite exponents reduce to a prime divisor; powers of two reduce to exponent 4. | Machine-check reduction paths for arbitrary n. | symbolic-rule | domain-fermats-last-theorem |
| loop-03 | 3 | Primitive normalization | complete | PrimitiveTriple | Can every counterexample be divided to gcd(a,b,c)=1? | Primitive normalization and pairwise-coprimality rules are registered. | Add ideal-theoretic normalization later. | symbolic-rule | domain-fermats-last-theorem |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| Name ƒ | Description | Tier | Pass Count | Fail Count | Universe Count | Status | Evidence | Protects Loop | Domain ID |
|---|---|---|---|---|---|---|---|---|---|
| inv-v13-hecke-compatibility | Every Hecke Frobenius stage reduces compatibly. | adic-certificate | 8 | 0 | 8 | PASS | Live rows from hecke_pseudocharacter_compatibility_v13. | loop-223 | domain-fermats-last-theorem |
| inv-v13-stage-compatibility | Every adjacent representation stage reduces compatibly. | adic-certificate | 6 | 0 | 6 | PASS | Live rows from representation_stage_compatibility_v13. | loop-220 | domain-fermats-last-theorem |
| inv-v14-module-compat | All module stages reduce compatibly. | adic-certificate | 6 | 0 | 6 | PASS | Live rows from module_stage_compatibility_v14. | loop-237 | domain-fermats-last-theorem |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Source** | A source is identified by its name. | — |
| Name | The same as its source ID. | _Name_ |
| Citation | A defined attribute. | _Citation_ |
| Source URL | A defined attribute. | _SourceUrl_ |
| Source Kind | A defined attribute. | _SourceKind_ |
| Role | A defined attribute. | _Role_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Citation | Source URL | Source Kind | Role |
|---|---|---|---|---|
| src-a2-theta-v20 | The A2 lattice theta series and its weight-one modular transformation law. | https://people.math.rochester.edu/faculty/doug/otherpapers/Schoeneberg.pdf | classical-monograph | Explicit weight-one Eisenstein/theta series congruent to one modulo 3. |
| src-brauer-nesbitt-v17 | Brauer–Nesbitt semisimplification uniqueness from character data. | https://math.berkeley.edu/~ribet/Articles/ps.pdf | primary-paper-context | Complete class traces determine the semisimplification. |
| src-buzzard-finite-flat | K. Buzzard, Finite flat group schemes course notes. | https://www.ma.imperial.ac.uk/~buzzard/maths/research/notes/finite_flat_group_schemes_course.pdf | course-notes | Finite-flat Hopf algebra foundations. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Trust Boundary** | A trust boundary is identified by its name. | — |
| Name | The same as its trust boundary ID. | _Name_ |
| Theorem ID | A defined attribute. | _TheoremId_ |
| Boundary Kind | A defined attribute. | _BoundaryKind_ |
| Accepted External Claims | A defined attribute. | _AcceptedExternalClaims_ |
| Kernel or Substrate | A defined attribute. | _KernelOrSubstrate_ |
| Can Claim Zero Imports | True when an empty string. | _CanClaimZeroImports_ |
| Notes | A defined attribute. | _Notes_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Theorem ID | Boundary Kind | Accepted External Claims | Kernel or Substrate | Can Claim Zero Imports | Notes |
|---|---|---|---|---|---|---|
| trust-fermats-last-theorem | fermats-last-theorem | application-frontier | Seven foundation provider theorems remain imported in v21. | ERB rulebook + conformance harness + mathematical provider contract | false | Zero imports is always relative to an explicitly declared logical and execution trust boundary. |
| trust-analytic-prime-distribution | analytic-prime-distribution | foundation-theorem | External foundation theorem; finite compiler interfaces are present but the universal theorem is not internalized. | ERB rulebook + conformance harness + mathematical provider contract | false | Zero imports is always relative to an explicitly declared logical and execution trust boundary. |
| trust-hilbert-specialization | hilbert-specialization | foundation-theorem | External foundation theorem; finite compiler interfaces are present but the universal theorem is not internalized. | ERB rulebook + conformance harness + mathematical provider contract | false | Zero imports is always relative to an explicitly declared logical and execution trust boundary. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Artifact Registry** | An artifact registry is identified by its name. | — |
| Name | The same as its artifact ID. | _Name_ |
| Domain ID | A defined attribute. | _DomainId_ |
| Artifact Kind | A defined attribute. | _ArtifactKind_ |
| Relative Path | A defined attribute. | _RelativePath_ |
| Version | A defined attribute. | _Version_ |
| Sha256 | A defined attribute. | _Sha256_ |
| Is Canonical Migration Source | True when an empty string. | _IsCanonicalMigrationSource_ |
| Notes | A defined attribute. | _Notes_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Domain ID | Artifact Kind | Relative Path | Version | Sha256 | Is Canonical Migration Source | Notes |
|---|---|---|---|---|---|---|---|
| v21-witness-report-v21-md | domain-fermats-last-theorem | md | domains/fermats-last-theorem/source-artifacts/WITNESS_REPORT_V21.md | 21.0.0 | a69503387b772aa4eee47f5e4c624764968343f1fc05367cadabff671a443f6a | true | Preserved from the v21 witness lab. Do not silently rewrite or delete during migration. |
| v21-final-nine-attempt-v21-md | domain-fermats-last-theorem | md | domains/fermats-last-theorem/source-artifacts/FINAL_NINE_ATTEMPT_V21.md | 21.0.0 | d5a246b8b44d25216f351de59e445e5f46629814ac6c7b9c0ef230ace2596534 | false | Preserved from the v21 witness lab. Do not silently rewrite or delete during migration. |
| v21-active-import-surface-v21-csv | domain-fermats-last-theorem | csv | domains/fermats-last-theorem/source-artifacts/active_import_surface_v21.csv | 21.0.0 | 5b0eb5ea5d4668178110f02a0ac3f3e60cd8a0a3ff149a838c53aaff0e195b2f | false | Preserved from the v21 witness lab. Do not silently rewrite or delete during migration. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Migration Mapping** | A migration mapping is identified by its name. | — |
| Name | The same as its mapping ID. | _Name_ |
| Source Table | A defined attribute. | _SourceTable_ |
| Target Table | A defined attribute. | _TargetTable_ |
| Strategy | A defined attribute. | _Strategy_ |
| Notes | A defined attribute. | _Notes_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Source Table | Target Table | Strategy | Notes |
|---|---|---|---|---|
| map-foundation-kernels | foundation_kernels_v21 | FoundationKernels | direct-row-copy | Retain kernel IDs and status fields. |
| map-proof-trace | proof_trace | ProofFacts | one-row-per-derived-fact | Retain rule IDs, antecedents, derivation kinds, and contradiction flag. |
| map-loops | loops | Loops | direct-row-copy | All 571 loops are historical domain data. |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Project Roadmap** | A project roadmap is identified by its name. | — |
| Name | The same as its roadmap ID. | _Name_ |
| Sequence No | A defined attribute. | _SequenceNo_ |
| Title | A defined attribute. | _Title_ |
| Status | A defined attribute. | _Status_ |
| Acceptance Criteria | A defined attribute. | _AcceptanceCriteria_ |
| Depends on | A defined attribute. | _DependsOn_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Sequence No | Title | Status | Acceptance Criteria | Depends on |
|---|---|---|---|---|---|
| phase-0-freeze | 0 | Freeze and verify v21 | complete | Source ZIP and SQLite hashes preserved; v21 verifier passes. | — |
| phase-1-catalog | 1 | Install theorem catalog rulebook | planned | Effortless build creates Postgres, RuleSpeak, OWL, and ExplainDAG projections from the new SSoT. | phase-0-freeze |
| phase-2-flt-migration | 2 | Migrate FLT flagship domain | planned | 571 loops, 113 proof facts, 305 invariants, and seven provider dependencies rebuild from blank. | phase-1-catalog |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Conclusion** | A conclusion is identified by its name. | — |
| Name | The same as its conclusion ID. | _Name_ |
| Category | A defined attribute. | _Category_ |
| Status | A defined attribute. | _Status_ |
| Report Tier | A defined attribute. | _ReportTier_ |
| Title | A defined attribute. | _Title_ |
| Evidence | A defined attribute. | _Evidence_ |
| Witnessed in Loop | A defined attribute. | _WitnessedInLoop_ |

<details open>
<summary>Example data</summary>

| Name ƒ | Category | Status | Report Tier | Title | Evidence | Witnessed in Loop |
|---|---|---|---|---|---|---|
| conc-theorem-network-created | instrument | witnessed | critical | FLT and seven foundation kernels are represented as one cross-theorem provider/consumer network. | The catalog contains eight theorem contracts and seven load-bearing FLT provider dependencies. | migration-bootstrap |
| conc-flt-not-zero-import | methodology | witnessed | critical | The migrated FLT object remains conditional on seven foundation providers. | v21 contains one contradiction trace and seven active foundation-kernel imports; no provider is marked fully internalized. | migration-bootstrap |
| conc-nna-first-zero-import-theorems | methodology | witnessed | critical | The natural-number-arithmetic domain contributes the network's first FULLY_INTERNALIZED_FOR_SCOPE, zero-import theorems. | Four theorems (+, -, *, /) reach FULLY_INTERNALIZED_FOR_SCOPE with ActiveImportCount=0, backed by a Peano kernel with theorem_content_internalized=1 and a TrustBoundary with CanClaimZeroImports=true. | loop-576 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
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

<details open>
<summary>Example data</summary>

| edge order | Name ƒ | previous status | current status | parent removed | replacement kernel ID | kernel shared with parent count | theorem content fully internalized | status | next target |
|---|---|---|---|---|---|---|---|---|---|
| 1 | import-analytic-chebotarev-density-v19 | IMPORTED | DERIVED-FROM-FOUNDATION-KERNEL | 1 | kernel-prime-distribution-v21 | 1 | 0 | PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS | Internalize the analytic prime-ideal theorem and Artin-L-function argument. |
| 2 | import-hilbert-irreducibility-weak-approximation-v19 | IMPORTED | DERIVED-FROM-FOUNDATION-KERNEL | 1 | kernel-hilbert-specialization-v21 | 1 | 0 | PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS | Internalize universal thin-set avoidance. |
| 3 | import-mazur-formal-immersion-eisenstein-v19 | IMPORTED | DERIVED-FROM-FOUNDATION-KERNEL | 1 | kernel-mazur-formal-immersion-v21 | 1 | 0 | PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS | Internalize the Eisenstein quotient and formal-immersion theorem. |

_ƒ marks a computed column._

</details>

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A domain **must** have a slug; a title; a role; a status; a description; a canonical theorem ID; and a relative path, and record whether it is a flagship.
- A proof status **must** have a sort order and a description, and record whether it is imported; whether it is derived; whether it is allows imported children; and whether it is a terminal.
- A theorem **must** have a domain ID; a title; a theorem kind; a statement; a scope; a proof status ID; a trust boundary; an active import count; a root fact ID; a contract path; a current version; and a next target, and record whether it is a flagship.
- A theorem dependency **must** have a consumer theorem ID; a provider theorem ID; a required conclusion; a proof status ID; a provider contract path; and a notes, and record whether it is load bearing; whether it is imported; and whether it is a shared kernel.
- A foundation kernel **must** have a kernel order; a title; a mathematical scope; an input contract JSON; an output contract JSON; a finite compiler tables JSON; a source ID; a theorem content internalized; a status; and a next target.
- A proof fact **must** have a step no; a rule ID; a derived fact; an antecedents JSON; a derivation kind; and a domain ID, and record whether it is contradiction.
- A loop **must** have a loop order; a title; a status; a new concept; a domain question; a witness summary; a next frontier; an epistemic tier; and a domain ID.
- An invariant check **must** have a description; a tier; a pass count; a fail count; an universe count; a status; an evidence; and a domain ID.
- A source **must** have a citation; a source URL; a source kind; and a role.
- A trust boundary **must** have a theorem ID; a boundary kind; an accepted external claims; a kernel or substrate; and a notes, and record whether it can claim zero imports.
- An artifact registry **must** have a domain ID; an artifact kind; a relative path; a version; a sha256; and a notes, and record whether it is a canonical migration source.
- A migration mapping **must** have a source table; a target table; a strategy; and a notes.
- A project roadmap **must** have a sequence no; a title; a status; an acceptance criteria; and a depends on.
- A conclusion **must** have a category; a status; a report tier; a title; an evidence; and a witnessed in loop.
- A legacy parent audit **must** have an edge order; a previous status; a current status; a parent removed; a replacement kernel ID; a kernel shared with parent count; a theorem content fully internalized; a status; and a next target.

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

_The expression column is the rule's definition in Docs notation —
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

> **RuleSpeak®** — the declarative business-rule notation created by **Ronald G. Ross**, following the conventions of **SBVR** — is coming soon. Until then this page shows the regular documentation. [www.RonRoss.info](https://www.RonRoss.info)
