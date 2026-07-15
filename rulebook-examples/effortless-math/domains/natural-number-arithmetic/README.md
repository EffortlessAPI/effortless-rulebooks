# Natural-Number Arithmetic (Peano) — Internalized Foundation Domain

This folder is the network's first **fully internalized, zero-import** domain. Where Fermat's Last Theorem is `DERIVED_WITH_IMPORTED_CHILDREN` (seven providers still imported) and the seven provider theorems are `IMPORTED`, every result here is `FULLY_INTERNALIZED_FOR_SCOPE` with `ActiveImportCount = 0`.

Its purpose is not the results — everyone already believes `2 + 2 = 4`. Its purpose is to prove that the *same rulebook genes* that model FLT also model elementary arithmetic **with no special-case schema**, and to give the proof-status machinery its first positive instance of a zero-import, fully-internalized theorem above an explicitly declared kernel.

## Trust boundary

`PeanoNaturalNumberKernel` — the trusted base is exactly:

- the `Zero` constructor,
- the `Successor` constructor,
- the recursive definitions of the four operations (see the contract).

Nothing above that line is imported. `CanClaimZeroImports = true` here **because the child frontier is closed** — not because finite examples happen to pass. That is the distinction the domain `CLAUDE.md` insists on.

## The four theorems (loops 572–575)

| Loop | Theorem | Statement | Rules used | Reuses |
|---|---|---|---|---|
| 572 | `natural-number-addition-2-plus-2-equals-4` | `NaturalAdd(2,2) = 4` | add-zero-left, add-succ-left | — |
| 573 | `natural-number-subtraction-4-minus-2-equals-2` | `NaturalSub(4,2) = 2` | sub-n-zero, sub-succ-succ | — |
| 574 | `natural-number-multiplication-2-times-2-equals-4` | `NaturalMul(2,2) = 4` | mul-zero-left, mul-succ-left | addition (572) |
| 575 | `natural-number-division-4-div-2-equals-2` | `NaturalDiv(4,2) = 2` | div-base, div-sub-step | subtraction (573) |

Multiplication reduces *through* the already-internalized addition, and division *through* the already-internalized subtraction. That internal reuse is a dependency **inside** the kernel boundary — it is not an import, and it is recorded as antecedent references in `ProofFacts`, never as a `TheoremDependencies` row.

## Kernel

- **Kernel ID:** `kernel-peano-naturals-v1`
- **`theorem_content_internalized`:** `1` — the first kernel in the network with internalized content (the seven FLT foundation kernels are all `0`).
- **Numerals:** `0=Z`, `1=S(Z)`, `2=S(S(Z))`, `3=S(S(S(Z)))`, `4=S(S(S(S(Z))))`. Numerals are structural abbreviations, not primitive constants.

## Witnessed derivations

Each result is a step-by-step reduction to a numeral normal form, recorded in `ProofFacts` (22 steps across the four operations). The addition trace is the canonical example:

```
NaturalAdd(S(S(Z)), S(S(Z)))          expand numeral 2
S(NaturalAdd(S(Z), S(S(Z))))          add-succ-left
S(S(NaturalAdd(Z, S(S(Z)))))          add-succ-left
S(S(S(S(Z))))                         add-zero-left
Numeral(4)                            recognize numeral   ∎
```

## Invariants

`InvariantChecks` in the `peano-reduction-certificate` tier protect this domain, including:

- **determinism** — each reducible term has at most one applicable next rule;
- **termination** — every recursive step strictly lowers a structural measure (successor count of the recursing operand);
- **no self-import** — no derivation references its own goal fact as an antecedent;
- **zero imports above the kernel** — all four theorems carry `ActiveImportCount = 0` and appear in no `TheoremDependencies` row;
- per-operation **normal-form** checks (`= 4`, `= 2`).

## Status rule (same guardrail as every other domain)

Do not promote any of these theorems past their **stated scope**. `2 + 2 = 4` is internalized; *addition is commutative* is a different, not-yet-modeled theorem. A finite calibration or a green invariant does not establish a universal theorem — the universal inference must be represented and its child frontier closed. See the domain `NextTarget` for the intended generalizations (associativity, commutativity, distributivity, ordering, the division algorithm).
