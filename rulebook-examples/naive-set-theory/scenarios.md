# Mock Data Scenarios — Exercising Every Rule

Each scenario maps to a rule (R1–R12) and to membership facts the rulebook must reproduce.

## S1 — Empty set has no members (R8)
`a ∈ ∅` must evaluate **False**. The empty set contains nothing.

## S2 — Singleton membership (R9)
`a ∈ {a}` must evaluate **True**, and `{a} ≠ a` (a set is distinct from its member).

## S3 — Order & repetition are irrelevant (R3, R4)
`{a, b}` and `{b, a, a}` denote the same set; both yield `a ∈ S = True`, `b ∈ S = True`.

## S4 — Comprehension builds a set from a condition (R5, R6)
`Evens = { x | x is even }`; membership tracks the predicate, e.g. `two ∈ Evens = True`.

## S5 — A set may contain sets (R10)
`{a} ∈ {{a}} = True` — nesting is allowed.

## S6 — Self-membership is well-formed (R11)
`U ∈ U` is a legal question for a self-containing set `U`; it evaluates **True** when grounded.

## S7 — Negation propagates NULL (K3 / Strong Kleene)
`¬True = False`, `¬False = True`, `¬NULL = NULL`. This is the row that makes the loop rest.

## S8 — Restricted excluded middle
For the Russell fact, `φ ∨ ¬φ` with `φ = NULL` gives `NULL ∨ NULL = NULL` — not forced True. This is why the paradox dissolves.

## S9 — Russell's set is a legal definition (R6 + R11)
`R = { x | x ∉ x }` is admitted by comprehension; no rule forbids it.

## S10 — The Russell fact is ungrounded → NULL (R12)
Evaluation of `R ∈ R`:
- trial **True** → defining condition yields False → not stable → contradiction
- trial **False** → defining condition yields True → not stable → contradiction
- trial **NULL** → defining condition yields NULL → **stable fixed point**
Therefore `R ∈ R = NULL`.

## S11 — Grounded sets stay bivalent
`a ∈ {a}` and `a ∈ ∅` never go NULL; classical behavior is recovered everywhere a definition converges.

## S12 — Extensionality across three values (R2)
Two sets are identical iff every object has the *same* membership value in both, including both NULL.
