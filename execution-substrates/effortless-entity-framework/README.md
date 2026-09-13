# Effortless Entity Framework Execution Substrate

This substrate runs the C# Entity Framework Core model that the commercial `rulebook-to-entity-framework` transpiler generates for each project (registered in that project's `effortless.json` with `RelativePath: /effortless-entity-framework`). The generator turns every calculated, lookup and aggregation field into a C# computed property that evaluates through its own formula runtime (`DataClasses/EfFormulaFns.cs`) over a real `DbContext` (`DataClasses/SoAEFContext.cs`).

## How it works

1. `take-test.sh` builds `EffortlessEntityFrameworkRunner.csproj` with `-p:EF_TOOL_DIR=$ERB_DOMAIN_DIR/effortless-entity-framework`. The csproj globs that project's `DataClasses/**/*.cs` (the real `SoAEFContext` included) and `SqlOnAir/DotNet/Lib/DataExtensions.cs`, and references EF Core 8 with the SQLite provider (plus the SQL Server provider, which the generated `OnConfiguring` names but the runner never uses). Nothing is copied or patched.
2. `Program.cs` builds the generated `SoAEFContext` against a throwaway SQLite file, then:
   - **seeds** every `$ERB_TESTING_DIR/blank-tests/<entity>.json` row with parameterized `INSERT`s on the connection (FK enforcement off). Tables and columns come from the EF model; blank-test keys are matched with an exact port of `orchestration/shared.py`'s `to_snake_case`;
   - **loads** every table into the context and sets `FreezeComputedValues`, so each computed property is evaluated once;
   - **reads** every `[NotMapped]` computed property for each blank-test row (looked up by primary key) and writes `$ERB_TESTING_DIR/effortless-entity-framework/test-answers/<entity>.json` with the blank test's keys, raw values echoed and derived values filled in.
3. `grade-and-record.py` grades the answers.

This mirrors the tool's own author-side harness (`Versioned-Stable-SSoTme-Tools/tools/effortless/rulebook-to-entity-framework/conformance/`), except that it seeds from the blank tests instead of the rulebook's `data`.

## Failure behaviour

- A blank-test file with no entity in the EF model, a value that cannot be converted to its property's type, or a row that was seeded but not loaded stops the run.
- A computed property whose getter throws is written as `null` (so the grader counts it) and listed in an `ERROR:` block at the end of the run log.
- A stale `DataClasses/SoAEntityBase.cs` fails the build with an explicit message. The tool writes that file only once (overwrite-Never), so a project first built by an older tool version keeps a base class whose context member is `Context`, while the current getters read `SoAContext`. Delete the file and re-run `effortless build` in the project.
