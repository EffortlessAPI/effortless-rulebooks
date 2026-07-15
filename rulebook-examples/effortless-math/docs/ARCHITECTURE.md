# Architecture

## Goal

Build one executable theorem network whose canonical semantic asset is structural data. The network contains:

- one deeply modeled flagship application theorem: Fermat's Last Theorem;
- seven first-class provider theorems that begin as honest imported contracts;
- shared witness, provenance, status, and conformance primitives;
- mechanically generated projections into Postgres, Python, RuleSpeak, OWL, ExplainDAG, spreadsheets, and later proof-assistant skeletons.

## Two-level hub

```text
global theorem catalog
  ├── theorem contracts
  ├── provider/consumer dependencies
  └── proof-status and trust-boundary ledger

individual theorem domains
  ├── theorem-specific schemas and witnesses
  ├── proof-obligation DAG
  ├── imported child frontier
  └── provider certificate export
```

A provider theorem exports a versioned certificate. Consumers depend on the certificate contract, not on the provider's private tables.

## Why this is not one giant FLT rulebook

FLT consumes reusable mathematics. Analytic Chebotarev, Hilbert specialization, global duality, comparison, level lowering, and automorphy must be independently versionable providers. Otherwise every future theorem would duplicate those structures inside its own application database.

## Why this is still one mathematical object

The theorem catalog compiles the provider contracts and consumer edges into one global DAG. A provider status or version change must deterministically rebuild every dependent theorem trace.

## Recommended physical layout

```text
effortless-math/
├── effortless-rulebook/               # canonical global theorem-network SSoT
├── catalog/                           # generated/simplified network projections
├── domains/
│   ├── fermats-last-theorem/
│   ├── analytic-prime-distribution/
│   ├── hilbert-specialization/
│   ├── mazur-modular-curve-arithmetic/
│   ├── global-deformation-duality/
│   ├── modular-curve-cohomological-comparison/
│   ├── universal-level-lowering/
│   └── solvable-artin-automorphy/
├── schemas/                           # theorem/certificate exchange contracts
├── scripts/                           # migration and validation only
├── testing/                           # cross-domain conformance answer key
└── source-artifacts/                  # immutable legacy archives
```

## Status is data

The status model must preserve at least these distinctions:

```text
IMPORTED
DECOMPOSED
DERIVED_WITH_IMPORTED_CHILDREN
DERIVED_WITH_SHARED_KERNEL
FULLY_INTERNALIZED_FOR_SCOPE
FALSIFIED
SUPERSEDED
NOT_EVALUABLE
```

Never infer full internalization from parent removal, finite witness success, or a green build alone.

## Provider certificate flow

```text
provider theorem rulebook
  → provider certificate (versioned + content hashed)
  → global catalog validates contract
  → consumer theorem binds hypotheses
  → consumer proof trace rebuilds
  → all substrates must agree on status and conclusion
```
