# Migration Plan

## Phase 0 — Freeze

1. Verify the SHA-256 of `source-artifacts/fermat_witness_lab_v21.zip`.
2. Run the v21 verifier from the extracted source artifacts.
3. Record the expected state: 571 loops, 113 proof-trace rows, 305 invariant rows, one contradiction, seven active foundation kernels.

## Phase 1 — Install the global theorem catalog

1. Place this payload in the intended `effortless-math` repo or temporarily under `rulebook-examples/effortless-math`.
2. Read the host repository's root `CLAUDE.md` before changing paths or scripts.
3. Treat `effortless-rulebook/effortless-math-rulebook.json` as the only semantic source of truth.
4. Run `effortless build` with `ERB_DOMAIN=effortless-math`.
5. Repair rulebook-shape incompatibilities in the rulebook, never by adding a parallel hand-coded evaluator.

## Phase 2 — Reproduce the v21 answer key

The generated Postgres/SQLite projection must reproduce:

- all 571 `Loops` rows;
- all 113 `ProofFacts` rows;
- all 305 `InvariantChecks` rows;
- all seven foundation dependencies;
- exactly one derived contradiction;
- no critical invariant failure.

Do not mark migration complete until a blank rebuild reproduces these counts and stable hashes for the semantic rows.

## Phase 3 — Install provider domains

Each provider folder already contains an initial `theorem-contract.json`. Convert each into the host repo's preferred per-domain rulebook shape while keeping the global theorem catalog authoritative for cross-domain edges.

The provider contracts should remain thin at first. They need:

- theorem statement;
- input/hypothesis contract;
- output/conclusion contract;
- current proof status;
- source frontier;
- expected witness schemas;
- provider fact IDs;
- consumers;
- next target.

## Phase 4 — Cross-domain certificate resolver

Add a generic resolver that:

1. loads a provider certificate by theorem ID and version;
2. validates it against `schemas/provider-certificate.schema.json`;
3. verifies artifact hashes;
4. binds consumer hypotheses explicitly;
5. refuses to upgrade the consumer if any load-bearing provider conclusion is absent or imported;
6. triggers a full conformance rebuild.

This resolver belongs in shared orchestration, not in FLT-specific app code.

## Phase 5 — Projections

Initial projections:

- Postgres/SQLite theorem graph;
- Python verifier;
- RuleSpeak narrative;
- OWL/SHACL ontology;
- ExplainDAG;
- CSV/XLSX obligation ledger;
- Lean/Coq/Isabelle declaration skeletons only.

Do not claim generated proof terms until a proof-assistant kernel actually checks them.

## Phase 6 — Provider loops

Pick one provider theorem—recommended first: `universal-level-lowering` or `analytic-prime-distribution`—and run the same witnessed loop process. Export versioned provider certificates into the catalog. Rebuild FLT after every semantic change.
