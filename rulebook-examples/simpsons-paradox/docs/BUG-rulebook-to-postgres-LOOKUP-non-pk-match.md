# Bug report: `rulebook-to-postgres` mistranslates `LOOKUP(...)` with a non-PK match column

**Target tool (make the fix here):**
`/Users/eejai42/effortlessapi-app-root/users/user_ee42ai73-18a9-47d5-8f99-954b00f6c041/my-projects/api.effortlessapi.com/Versioned-Stable-SSoTme-Tools/tools/effortless/rulebook-to-postgres/workload/Program.cs`

**Live repro project (a downstream consumer with a real broken field):**
`/Users/eejai42/effortlessapi-app-root/users/user_ee42ai73-18a9-47d5-8f99-954b00f6c041/my-projects/effortless-rulebooks/rulebook-examples/simpsons-paradox`

**Discovered by:** simpsons-paradox loop-92 / loop-93 (root-caused) → loop-98 (this report). The downstream project currently hand-writes a workaround for this in `effortless-postgres/03b-customize-views.sql` (a `vw_studies` override that re-derives these lookup columns via a real join) and `03c-customize-functions-post-views.sql`. When this transpiler bug is fixed, those hand-written overrides can eventually be deleted.

---

## Summary

The transpiler's `LOOKUP(...)` support only understands **"match by the target table's primary key."** When a rulebook formula uses `LOOKUP(key, Table[MatchCol], Table[ReturnCol])` where **`MatchCol` is NOT `Table`'s primary key** (e.g. `MatchCol` is a relationship/FK column on `Table` pointing back at the current table — a reverse lookup), the transpiler produces **syntactically valid but semantically wrong** SQL: a self-referential no-op subquery that passes the wrong ID into the downstream calc function, so the field silently resolves to `NULL` (or, worse, a coincidentally-matching wrong row).

There are **two manifestations of the same root cause** (LOOKUP's match-column name is parsed but then discarded, and the codegen unconditionally assumes PK-match):

- **Bug #1 (silent wrong answer):** `LOOKUP(...)` as a *whole formula* with a non-PK match column.
- **Bug #2 (`NULL::type` stub):** `LOOKUP(...)` as a *subexpression* inside `IF`/`AND`/`SWITCH`/nested `CASE` — throws `NotSupportedException`, which the outer catch turns into a `NULL::<type>` stub with a warning comment, nuking **every** branch of that formula, not just the LOOKUP one.

> ⚠️ Note: the **constant-key + PK-match** shape (e.g. `LOOKUP("simpsons-paradox-v1", ModelSummary[ModelSummaryId], ModelSummary[SomeCol])` where `ModelSummaryId` genuinely IS the PK) compiles **correctly today** — that is NOT the bug. The trigger is specifically **`Table[MatchCol]` where `MatchCol` is not the PK.**

---

## Confirmed live repro (already present in the simpsons-paradox project)

**Rulebook field** (`Studies.DistortionType`, from `effortless-rulebook/simpsons-paradox-rulebook.json`):

```json
{
  "name": "DistortionType",
  "type": "lookup",
  "datatype": "string",
  "formula": "=LOOKUP({{StudyId}}, TreatmentRankings[Study], TreatmentRankings[DistortionType])"
}
```

Key facts about the target table `TreatmentRankings`:
- Its **primary key** is `TreatmentRankingId` (`type: raw`).
- `Study` is a **`relationship`-typed FK** that points at `Studies` — it is **NOT** `TreatmentRankings`' PK.

So the formula means: *"find the one `TreatmentRankings` row whose `Study` FK equals this `Studies` row's `StudyId`, and return its `DistortionType`."* This is a single-row **reverse-FK** lookup.

**Generated (wrong) SQL** — from `effortless-postgres/02-create-functions.sql`:

```sql
CREATE OR REPLACE FUNCTION calc_studies_distortion_type(p_study_id TEXT)
RETURNS TEXT AS $$
  SELECT calc_treatment_rankings_distortion_type(
           (SELECT study_id FROM studies WHERE study_id = p_study_id)
         );
$$ LANGUAGE sql STABLE;
```

Why it's wrong:
- `(SELECT study_id FROM studies WHERE study_id = p_study_id)` is a no-op — it just returns `p_study_id` back.
- That `StudyId` value is then passed into `calc_treatment_rankings_distortion_type(p_treatment_ranking_id TEXT)` **as if it were a `TreatmentRankingId`**.
- Unless a Study's id and its TreatmentRanking's id happen to be identical strings, this returns `NULL` (no matching row). It never performs the intended `WHERE TreatmentRankings.Study = Studies.StudyId` join.

**Correct SQL it should have emitted** (shape):

```sql
CREATE OR REPLACE FUNCTION calc_studies_distortion_type(p_study_id TEXT)
RETURNS TEXT AS $$
  SELECT calc_treatment_rankings_distortion_type(
           (SELECT treatment_ranking_id FROM treatment_rankings
            WHERE study = p_study_id
            LIMIT 1)
         );
$$ LANGUAGE sql STABLE;
```

(i.e. resolve the matching `TreatmentRankings` row by its `Study` FK first, then feed *that row's PK* into the return-column calc function — or inline the return column directly.)

---

## Root cause — file & line references in `Program.cs`

> Line numbers are from the investigation snapshot; verify against current `Program.cs` before editing, they may have shifted.

### Bug #1 (whole-formula non-PK LOOKUP → self-lookup no-op)

**Trigger:** formula passes `IsWholeFormulaPlainLookup` (~line 1486) and `args[1]` = `Table[MatchCol]` where `MatchCol` ≠ target PK.

1. **`ParseLookupFormula` (~lines 1527–1598):** parses `args[1]` (`Table[MatchCol]`) only to extract the **table name** (`matchParts[0]`, ~line 1576). The **column name after `[` is discarded** and never stored. `ParsedIndexMatch` (~lines 4807–4820) has no field to hold it.
2. **Codegen (~lines 2444–2447):** `relationshipFieldName` (parsed from the LOOKUP's *first* argument) is looked up as a field **on the current table** (`table.Fields.FirstOrDefault(f => f.Name.Equals(relationshipFieldName...))`) — unconditionally assuming the classic "this table has an FK equal to the related table's PK" shape.
3. **~lines 2492–2495:** `relatedPkField` is found via `IsIdField` on the related table — unconditionally assuming the join target is the related table's **PK**, ignoring whatever `Table[MatchCol]` actually named.
4. **~line 2663 (non-calc) / ~2621+2623 (calc):** emits `WHERE {relatedPkField} = {fkExpression}` — joining on the related table's PK using a value from a same-named field on the current table, even when the formula explicitly said to match on a *different* column.

### Bug #2 (LOOKUP as a subexpression → `NULL::type` stub for the whole formula)

**Trigger:** any formula not matching `IsWholeFormulaPlainLookup` (~lines 1486–1516) and not `INDEX/MATCH`-shaped — i.e. LOOKUP nested inside `IF(...)`, `AND(...)`, `SWITCH(...)`, a `CASE` dispatch, etc.

1. Such formulas fall through to the general `ExcelToPostgresFormulaTranslator` (documented at ~lines 1012–1021).
2. That class's function-name dispatch switch (~lines 671–941 within the class) has **no `case "LOOKUP":`** → hits `default: throw new NotSupportedException($"Function '{funcName}' is not supported yet");`.
3. `TranslateExpression` is a single unguarded recursive descent (entry ~lines 78–91 of that class) with no per-subexpression try/catch, so the exception propagates to the single outer catch in the calc-function codegen loop.
4. That outer catch (~lines 2964–2979) replaces the **entire field's formula** (not just the LOOKUP subexpression) with:
   ```csharp
   formulaBody = $"/* WARNING: Formula translation failed: {ex.Message}\n   Original Airtable formula:\n   {escapedFormula}\n*/\nNULL::{returnType.ToLower()}";
   ```
   (~line 2978). One unsupported LOOKUP subexpression kills every branch of the `IF`/`CASE`, including branches that would have translated fine.

---

## Proposed fix (scoped, additive — not implemented)

1. **Extend `ParsedIndexMatch`** (~line 4807) with a `MatchField` (or `LookupMatchColumn`) string property.

2. **`ParseLookupFormula` (~lines 1527–1598):** capture the column name after `[` in `args[1]` into `MatchField` (currently only `matchParts[0]`, the table, is kept — capture `matchParts[1]` too, analogous to how `returnParts[1]` is already captured ~line 1583). Check `ParseIndexMatchFormula` for the same discard if INDEX/MATCH can name a non-PK match column.

3. **Codegen (~lines 2444–2497 and the `isReverseRelationship` logic ~2451–2482):** before assuming the classic forward-FK shape, test whether `parsed.MatchField` equals the related table's PK (`IsIdField`):
   - **PK match (current behavior):** keep the existing forward-FK codegen unchanged.
   - **Non-PK match (the bug):** emit a single-row reverse lookup —
     ```
     (SELECT {returnCol} FROM {relatedTable} WHERE {matchCol} = p_{pgPkField} LIMIT 1)
     ```
     reusing the machinery already present for the `isReverseRelationship` path (~lines 2574–2597), but as a scalar single-row subquery (LOOKUP expects one row, not a `string_agg` aggregate).

4. **`ExcelToPostgresFormulaTranslator`:** add a `case "LOOKUP":` in the function-dispatch switch (~near line 940, beside `default:`) that parses its 3 args the same way (ideally by extracting `ParseLookupFormula`'s arg-parsing into a shared helper) and emits the same correlated-subquery SQL shape as item 3. This fixes Bug #2 — LOOKUP nested inside `IF`/`SWITCH`/`AND` translates correctly instead of throwing and nuking the whole formula.

5. **Regression tests** should cover:
   - (a) constant-key LOOKUP with a **non-PK** match column;
   - (b) same-row-key LOOKUP with a non-PK / reverse-FK match column — **`simpsons-paradox`'s `Studies.DistortionType` is a ready-made real-world fixture**;
   - (c) LOOKUP nested inside an `IF(...)` / `CASE` dispatch with a non-PK match column.

---

## How to verify the fix against the live repro

1. Rebuild the tool and point the simpsons-paradox project at the freshly-built (unpublished) version:
   `effortless -setUrl rulebook-to-postgres=<versioned-url>` (undo with `-removeUrl`).
2. From `rulebook-examples/simpsons-paradox`, run `effortless build`, then inspect
   `effortless-postgres/02-create-functions.sql` → `calc_studies_distortion_type`. It should now
   resolve the `TreatmentRankings` row via `WHERE study = p_study_id` instead of the current
   `SELECT study_id FROM studies WHERE study_id = p_study_id` self-lookup.
3. `cd effortless-postgres && ./init-db.sh`, then confirm `SELECT study_id, distortion_type FROM vw_studies LIMIT 5;` returns non-null `distortion_type` values **directly from the native (non-overridden) `vw_studies`** — i.e. temporarily bypass/disable the `03b-customize-views.sql` `vw_studies` override to prove the native transpiler output is now correct on its own.
4. Once confirmed, the hand-written `vw_studies` LOOKUP-column override in `03b-customize-views.sql` (and the related `03c` entries) can be removed from the downstream project.
