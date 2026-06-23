# Mock Data & Scenarios

These scenarios validate that the normalized schema can answer real questions about the debate. Each maps to an inference (calculated/lookup/aggregation) the rulebook must support. The actual seed rows live in `effortless-rulebook.json` (Step 9).

### Scenario A — Premise counting (1st-order aggregation)
*"How many premises does the cosmological argument have?"*
The `Cosmological Argument` row should roll up its child `Premises` (a beginning exists → a beginning needs a beginner → the cause is spaceless/timeless/immaterial). Exercises `Argument.PremiseCount = COUNTIFS(Premises.SupportsArgument, this)`.

### Scenario B — Evidence lookup
*"What evidence backs the cosmological argument?"*
Five `Evidence` rows (the SURGE items) all point to `Cosmological Argument`. Exercises `Argument.EvidenceCount` and the Evidence→Argument FK.

### Scenario C — Rebuttal counting (1st-order)
*"Was Turek's fine-tuning claim rebutted?"*
Hitchens's Krauss/red-shift-increasing rebuttal points at the fine-tuning claim. Exercises `Claim.RebuttalCount` and `Claim.IsRebutted = RebuttalCount > 0`.

### Scenario D — Roll-up to argument (2nd-order)
*"Which arguments were contested?"*
Claims supporting the `Design Argument` accumulate rebuttals; the argument sums them. Exercises `Argument.TotalRebuttals = SUMIFS(Claims.RebuttalCount, Claims.SupportsArgument, this)` and `Argument.IsContested = TotalRebuttals > 0` — a 3rd-order chain (Rebuttal → Claim → Argument).

### Scenario E — Worldview attribution
*"How many arguments does each worldview field?"*
`Theism` should own Turek's seven arguments; `Atheism`/`Anti-theism` Hitchens's counters. Exercises `Worldview.ArgumentCount` and the Argument→Worldview FK.

### Scenario F — Thinker invocation, by side
*"Which thinkers did each debater press into service?"*
Jastrow, Hawking, Dawkins, Flew, Hoyle → Turek (often as hostile witnesses). Hume, Paley, Jefferson, Socrates, Smith, Krauss, Gould → Hitchens. Exercises the Thinker→Debater FK and `Debater.ThinkersCited`.

### Scenario G — Quotation provenance (lookup chain)
*"Who said the universe is 'like looking at God,' and who quoted them?"*
Smoot is the `Speaker`; Turek is the `CitedBy` debater. Exercises a two-hop lookup Quotation→Thinker and Quotation→Debater. (Quotation text is stored as paraphrase, never long verbatim.)

### Scenario H — Concept coverage
*"Which concepts did the moral exchange touch?"*
`is-ought problem`, `objective moral values`, `divine permission`, `conscience` all appear as `Concepts` linked from claims on both sides. Exercises `Concept.ClaimCount`.

---

**Representative seed counts (target):** ~5 Debaters/roles, 8 Worldviews, ~13 Arguments, ~20 Premises, ~6 Evidence (SURGE+), ~24 Concepts, ~16 Claims, ~10 Rebuttals, ~30 Thinkers, ~8 Quotations. Every vocabulary term from `vocabulary.txt` appears in at least one table name, field, description, or data value (Step 7 requirement).
