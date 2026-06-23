# Normalized Schema (rough, pre-JSON)

A DAG of nine tables. Arrows are foreign keys (child → parent, many-to-one). Every `Name` is a slug formula over a `Label` raw field, per convention. Inference fields are tagged 1°/2°/3° by DAG depth.

```
Debaters ─────┐ ┌──────── Worldviews
   ▲          │ │              ▲
   │ (championedBy)            │ (fieldsWorldview)
Worldviews ───┘                │
   ▲                           │
   │                           │
Arguments ──(advancedBy)──▶ Debaters
   ▲   ▲                       ▲
   │   │                       │
Premises Evidence              │ (introducedBy / citedBy / madeBy)
   (supportsArgument)          │
                               │
Concepts ──(introducedBy)──▶ Debaters
   ▲                           ▲
   │ (touchesConcept)          │
Claims ──(madeBy)──────────────┘
   ▲   │
   │   └(supportsArgument)──▶ Arguments
Rebuttals ──(rebutsClaim)──▶ Claims
            (madeBy)────────▶ Debaters

Thinkers ──(citedBy)───────▶ Debaters
Quotations ─(speaker)──────▶ Thinkers
            (citedBy)──────▶ Debaters
```

## Tables

**Debaters** — the participants and the moderator.
- `Label` (string, raw) · `Name` (string, calc slug)
- `Side` (string, raw — affirmative / negative / moderator)
- `WorldviewDefended` (relationship → Worldviews, nullable)
- `Description` (string, raw)
- `ArgumentCount` (integer, agg) — *1° COUNTIFS Arguments.AdvancedBy*
- `ClaimCount` (integer, agg) — *1°*
- `ThinkersCited` (integer, agg) — *1°*

**Worldviews** — theism, deism, atheism, anti-theism, agnosticism, materialism, humanism, Christianity.
- `Label`/`Name` · `AffirmsGod` (boolean, raw) · `ChampionedBy` (rel → Debaters, nullable) · `Description`
- `ArgumentCount` (integer, agg) — *1°*

**Arguments** — the 13 named arguments.
- `Label`/`Name` · `ArgumentType` (string, raw — primary / supporting / counter)
- `AdvancedBy` (rel → Debaters) · `FieldsWorldview` (rel → Worldviews)
- `Conclusion` (string, raw) · `Description`
- `PremiseCount` (integer, agg) — *1°* · `EvidenceCount` (integer, agg) — *1°*
- `ClaimCount` (integer, agg) — *1°*
- `IsFullyDeveloped` (boolean, calc) — *2° PremiseCount ≥ 2*
- `TotalRebuttals` (integer, agg) — *2° SUMIFS over Claims.RebuttalCount*
- `IsContested` (boolean, calc) — *3° TotalRebuttals > 0*

**Premises** — the ordered steps inside an argument.
- `Label`/`Name` · `SupportsArgument` (rel → Arguments) · `Ordinal` (integer, raw)
- `Statement` (string, raw) · `Description`

**Evidence** — empirical items (SURGE, DNA-info, red-shift, etc.).
- `Label`/`Name` · `SupportsArgument` (rel → Arguments)
- `EvidenceKind` (string, raw — observational / theoretical) · `Description`

**Concepts** — the philosophical concepts raised.
- `Label`/`Name` · `Category` (string, raw — cosmology / design / moral / theological / method / epistemic)
- `IntroducedBy` (rel → Debaters, nullable) · `Description`
- `ClaimCount` (integer, agg) — *1°*

**Claims** — specific assertions.
- `Label`/`Name` · `MadeBy` (rel → Debaters) · `Phase` (string, raw — opening/rebuttal/cross-ex/closing)
- `SupportsArgument` (rel → Arguments, nullable) · `TouchesConcept` (rel → Concepts, nullable)
- `Description`
- `RebuttalCount` (integer, agg) — *1°* · `IsRebutted` (boolean, calc) — *2° RebuttalCount > 0*

**Rebuttals** — responses to claims.
- `Label`/`Name` · `MadeBy` (rel → Debaters) · `RebutsClaim` (rel → Claims) · `Description`

**Thinkers** — cited authorities.
- `Label`/`Name` · `Field` (string, raw — physics/biology/philosophy/etc.) · `Era` (string, raw)
- `CitedBy` (rel → Debaters, nullable) · `CitedAsHostileWitness` (boolean, raw) · `Description`
- `QuotationCount` (integer, agg) — *1°*

**Quotations** — paraphrased citations (never long verbatim — copyright).
- `Label`/`Name` · `Speaker` (rel → Thinkers) · `CitedBy` (rel → Debaters)
- `Gist` (string, raw — paraphrase) · `Description`

## DAG depth check
- 1° (depend only on raw): all the COUNTIFS, IsRebutted's input.
- 2°: `Argument.IsFullyDeveloped`, `Claim.IsRebutted`, `Argument.TotalRebuttals`.
- 3°: `Argument.IsContested` (depends on TotalRebuttals which depends on Claim.RebuttalCount).
No cycles; every relationship is many-to-one. ✓
