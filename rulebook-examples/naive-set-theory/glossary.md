# Glossary — Three-Valued Naïve Set Theory

Formal vocabulary for the platform, grouped by area. Every term below appears in the rulebook.

## Sets & Membership

- **Set** — A collection of distinct objects, determined entirely by its members. The basic entity of the theory.
- **Member** — An object that belongs to a set. In this universe every member is itself a set.
- **Membership** — The relation asserting that an object is a member of a set, written `x ∈ S`. Here it is three-valued rather than two-valued.
- **Self-membership** — The case where a set is a member of itself (`S ∈ S`), permitted by the naïve rules and central to Russell's construction.
- **Comprehension** — The rule that any condition you can state determines a set whose members are exactly the objects satisfying it.
- **Singleton** — A set with exactly one member; distinct from that member.
- **Empty set** — The set with no members, `∅`.
- **Contain** — Informal verb for the membership relation: a set contains its members.
- **Collection** — The intuitive notion a set formalizes; an unordered grouping of objects.
- **Extensionality** — The rule that a set is fully determined by its members: two sets with the same membership profile are identical.
- **Order** — The arrangement of listed members, which carries no information (`{a,b} = {b,a}`).
- **Repetition** — Listing a member more than once, which adds nothing (`{a,a} = {a}`).

## Truth & Logic

- **Truth value** — The outcome of evaluating a membership question or condition: True, False, or NULL.
- **True** — The classical affirmative value.
- **False** — The classical negative value.
- **NULL** — The third value: "does not resolve to True or False." Undefined, ungrounded, no fixed point. A first-class outcome, not an error.
- **Bivalent / Bivalence** — The assumption that every question resolves to exactly one of True or False. This platform rejects forced bivalence.
- **Two-valued** — A logic admitting only True and False; the classical setting where Russell's paradox is fatal.
- **Three-valued** — A logic admitting True, False, and NULL; the setting this platform adopts.
- **Classical** — The standard two-valued logic; recovered as the special case where no NULL arises.
- **Partial** — A semantics in which some facts may be undefined; membership here is partial.
- **Semantics** — The system assigning meaning (truth values) to membership questions and conditions.

## Connectives (Strong Kleene)

- **Strong Kleene** — The three-valued logic (a.k.a. **K3**) whose connective tables define how NULL propagates.
- **K3** — Shorthand for Strong Kleene three-valued logic.
- **Connective** — A logical operator combining truth values: negation, conjunction, or disjunction.
- **Negation** — The unary connective `¬`; flips True/False and maps NULL to NULL.
- **Conjunction** — The binary connective `∧` (AND), evaluated by the K3 table.
- **Disjunction** — The binary connective `∨` (OR), evaluated by the K3 table.
- **Truth table** — The data defining a connective's output for every combination of input truth values.

## Evaluation & Fixed Points

- **Evaluation** — The act of computing a membership question's truth value via the grounding procedure.
- **Fixed point** — A value assignment that, fed back into the definition, reproduces itself; the stable outcome of evaluation.
- **Kripke** — Saul Kripke's fixed-point construction for grounded truth, the model this platform borrows.
- **Grounded** — A membership fact that settles to True or False through the evaluation procedure.
- **Ungrounded** — A membership fact that never settles; it remains NULL.
- **Converge** — To settle on a stable truth value during evaluation; failure to converge yields NULL.
- **Undefined** — The condition of an ungrounded fact: neither True nor False.
- **Self-referential** — A definition whose evaluation depends on its own value; the source of non-convergence.
- **Loop** — A self-referential evaluation cycle that may or may not settle.
- **Definition** — The condition (predicate) that specifies a set's members.
- **Condition** — A logical expression deciding membership for each candidate object.
- **Predicate** — A condition expressed as a property an object may or may not have.

## Russell & The Missing Rule

- **Russell set** — `R = { x | x ∉ x }`, the set of all sets that do not contain themselves.
- **Russell's paradox** — The classical contradiction arising when asking whether the Russell set contains itself.
- **Contradiction** — A forced clash between True and False; what bivalence produces for `R ∈ R` and what NULL avoids.
- **Rule** — One of the structural laws governing sets; the platform has twelve.
- **Rule 12** — The missing rule: a membership question whose definition cannot settle returns NULL.
- **CMCC** — Conceptual Model Completeness Conjecture: every definable property has an evaluation outcome, and "does not converge" is a legitimate one.
- **Paraconsistent** — Logics that tolerate some contradictions without collapse; the sibling "glut" route to this platform's "gap" (NULL) route.
- **Declarative / Declarable** — A system where properties are stated as definitions and evaluated, rather than imperatively computed.
