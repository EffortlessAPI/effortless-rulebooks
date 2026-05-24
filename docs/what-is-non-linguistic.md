# What Does "Non-Linguistic" Mean?

Most business-rule systems are linguistic: you write a sentence, a function, a
query, or a policy document. The meaning of the rule lives inside the syntax of
some language — English, Python, SQL, YAML — and the language is how you
communicate both to humans and to machines.

ERB is non-linguistic. The rules live in a **structured grid of named,
typed cells**. There is no sentence to parse. There is no grammar to learn.
There is no ambiguity introduced by word choice.

---

## What "linguistic" actually means

A linguistic representation depends on a reader sharing a grammar with a writer.
SQL's `CASE WHEN status = 'open' THEN 1 ELSE 0 END` only means anything if you
know SQL. The English equivalent — "count one if the status is open, otherwise
zero" — only means anything if you share English idioms with the author. Both are
linguistic: the meaning is encoded in a token sequence whose interpretation
depends on shared grammar.

Linguistic representations have a hidden cost: **the grammar is a layer of
indirection between the intended meaning and the machine.** Every time you want
a new substrate to consume the rule, you need a parser, an interpreter, or a
human who understands the source language.

---

## What ERB does instead

An ERB rulebook is a table. Each row is an entity. Each column is a typed slot.
The slots have fixed roles — `Name`, `DataType`, `Formula`, `IsCalculated`,
`ForeignKey` — and the meaning of each slot is fixed by the schema, not by what
the author chose to write in it.

A formula like `LOWER(SUBSTITUTE(DisplayName, " ", "-"))` appears in the
`Formula` column of a specific row. The **row** says which entity it belongs to.
The **column** says it is a formula, not a raw value, not a foreign key. The
**DataType** column says the result is a string. None of that is encoded in the
formula text itself — it is encoded in the *structure* of the table.

This is non-linguistic: the machine doesn't need to understand the formula text
to know it is a formula, which field it computes, or what type it returns. That
information is structural, not syntactic.

---

## The practical payoff

Because the rules are structural rather than linguistic, any substrate can
consume them mechanically:

- A Python transpiler reads the `Formula` column, parses the expression, and
  emits a `calc_*()` method. It never needs to understand what the formula
  *means* — just what slot it lives in.
- A Postgres transpiler reads the same column and emits a SQL function. Same
  input, different output, no grammar negotiation.
- An English transpiler reads the same row and produces a prose sentence. The
  sentence is linguistic; the source is not.

The grammar lives **only in the transpilers**, not in the rulebook itself. Add a
new transpiler (Go, COBOL, ARM64 assembly) and the same rulebook feeds it without
change, because the rulebook never encoded meaning in a language the new
transpiler couldn't read.

---

## Non-linguistic ≠ no formulas

ERB rulebooks do contain formula text — `IF({{Total}} > 1000, "large", "small")`
is a string. That string is linguistic in the narrow sense that it follows an
Excel-like grammar. But the *claim* ERB makes is at a higher level: the
**business intent** — "there is a calculated field on this entity, derived from
these other fields, using this formula, returning this type" — is encoded
structurally. The formula string is just a payload in a well-typed slot.

This is the same distinction as the difference between a JSON value and a JSON
schema. The value `"alice@example.com"` is opaque text; the schema says it is an
email address. ERB's structure is the schema; the formula text is the value.

---

## Why Airtable

Airtable's structured grid is a practical embodiment of this principle. Every
field you add to an Airtable table is a typed, named slot — not a free-text
field, not a configuration comment, not a line in a config file. The UI enforces
the structure. The exported JSON preserves it. The rulebook format is a
faithful, portable representation of that same structure.

This is also why the rulebook is readable by any frontier LLM without explanation:
the structure is self-describing. A row that says "FullName, formula, string,
`LastName & ', ' & FirstName`" is unambiguous regardless of whether you speak
SQL, Python, or English.

---

## See also

- [Rulebook is a complete spec](features/README.complete-spec.md) — the downstream consequence: any LLM or transpiler can answer questions about the domain from the rulebook alone
- [Hub-and-Spoke topology](features/README.hub-and-spoke.md) — why structural encoding makes the n-substrate problem tractable
- [ExplainDAG](features/README.explain-dag.md) — how the non-linguistic structure makes full inference tracing possible
- [CMCC / the 5 primitives](https://medium.com/effortlessapi/prove-me-wrong-every-idea-in-the-universe-melts-effortlessly-into-these-5-simple-primitives-87df9317e86e) — the theoretical claim that Schema, Data, Lookups, Aggregations, and Formulas are sufficient for any finitely-computable design-time semantic
