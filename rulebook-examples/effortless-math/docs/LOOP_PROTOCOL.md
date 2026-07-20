# Effortless Math loop protocol

A **loop** is exactly one semantic change to a canonical rulebook.

This definition is strict:

```text
canonical rulebook semantic object changed  => loop
canonical rulebook semantic object unchanged => not a loop
```

A loop may add, modify, or remove schema, rows, formulas, relationships, warrants, trust boundaries, status semantics, or other canonical rulebook content. A formatting-only rewrite is not a loop when the parsed JSON object is unchanged.

The following are not loops unless they also change the canonical rulebook:

- UI or UX work;
- generated SQL, RuleSpeak, application code, or other projections;
- solver/provider adapters;
- orchestration and workflow changes;
- build, packaging, dependency, or deployment repairs;
- tests, validators, replay scripts, benchmark runners, or reports;
- documentation-only changes.

Those changes remain important, but they are implementation, projection, validation, benchmark, reproducibility, or documentation work. They do not receive semantic loop numbers and do not increment a rulebook version.

## One canonical ledger

Each domain keeps one canonical loop ledger inside its rulebook. For the traveling-salesman domain, that ledger is the `TSPLoops` table in:

```text
rulebook-examples/effortless-math/domains/traveling-salesman/
  effortless-rulebook/traveling-salesman-rulebook.json
```

A semantic loop and its ledger row are one atomic rulebook change. Do not maintain a second authoritative loop list in Python, SQL, YAML, test JSON, campaign reports, Git history, or prose. Those artifacts may preserve evidence and projections, but they must identify the canonical `TSPLoops` row they support.

The ledger must preserve development order. Loop identifiers are immutable once published. Every row must state at least:

- the before-state;
- the semantic change;
- the closure criterion;
- the after-state;
- proof or claim boundaries;
- evidence artifacts;
- completion disposition;
- whether closure was contemporaneously built or later established by a consolidated replay.

## Closure and replay

Changing the rulebook creates a loop. It does not, by itself, close the loop.

A loop may be marked closed only when the cumulative rulebook state has passed the domain's required gates:

```text
canonical JSON validation
-> domain invariants
-> real Effortless generation
-> generated Postgres recreation
-> Python/Postgres peer conformance
-> loop-specific evidence replay
-> artifact and toolchain provenance capture
```

When historical loop commits were not individually built, do not invent contemporaneous evidence. Preserve that fact and close them through an explicit consolidated replay of the ordered cumulative state. The replay certificate must name the loop range, source commit, rulebook hash, generated tree hashes, toolchain identities, database version, provider versions, commands, and independent verification results.

## Commit messages

Semantic commits should name the loop and the conceptual change. Non-loop commits should name the actual work without a loop number.

Examples:

```text
TSP loop 604: add bound-sandwich semantics
Pin TSP solver and transpiler provenance
Repair TSP build workflow branch targeting
Reconcile TSP README with consolidated rulebook
```

The first example is a loop because the rulebook changes. The other examples are not loops unless their commits also modify the canonical rulebook's semantic object.
