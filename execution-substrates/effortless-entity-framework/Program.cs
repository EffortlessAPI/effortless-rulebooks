using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
// Aliases, not a namespace import: the generated model can contain classes named
// Exception / Action, which would shadow the System types in this file.
using SoAEFContext = SqlOnAir.DotNet.Lib.DataClasses.SoAEFContext;

namespace EffortlessEntityFrameworkRunner;

/// <summary>
/// effortless-entity-framework substrate test runner.
///
/// Runs the generated EF model for real, mirroring the tool's own conformance harness
/// (rulebook-to-entity-framework/conformance/Program.cs):
///   1. Build the generated SoAEFContext against a throwaway SQLite file (its ctor calls
///      Database.EnsureCreated()).
///   2. SEED the raw facts from $ERB_TESTING_DIR/blank-tests/*.json with parameterized
///      INSERTs straight on the connection, FK enforcement off. Seeding through EF's change
///      tracker would fire the generated navigation getters mid-SaveChanges.
///   3. LOAD every table's rows into the context first, then freeze the context
///      (FreezeComputedValues) so each computed property is evaluated once.
///   4. READ: for every blank-test row, look the entity up by primary key and read every
///      computed ([NotMapped]) property; the generated getters compute the value through
///      EfFormulaFns. Write test-answers/&lt;entity&gt;.json with the blank tests' keys.
///
/// Entirely reflection-driven: tables, columns and keys come from the EF model, so no
/// per-entity wiring lives here.
/// </summary>
public static class Program
{
    public static int Main()
    {
        var testingDir = Environment.GetEnvironmentVariable("ERB_TESTING_DIR");
        if (string.IsNullOrEmpty(testingDir))
            throw new InvalidOperationException("ERB_TESTING_DIR must be set to the active project's testing directory.");
        var blankTestsDir = Path.Combine(testingDir, "blank-tests");
        if (!Directory.Exists(blankTestsDir))
            throw new DirectoryNotFoundException($"No blank tests at {blankTestsDir}");
        var testAnswersDir = Path.Combine(testingDir, "effortless-entity-framework", "test-answers");
        Directory.CreateDirectory(testAnswersDir);

        Console.WriteLine("Effortless-EntityFramework Substrate Test Runner");
        Console.WriteLine(new string('=', 50));

        var tmpDb = Path.Combine(Path.GetTempPath(), $"erb-ef-{Guid.NewGuid():N}.db");
        try
        {
            var options = new DbContextOptionsBuilder<SoAEFContext>().UseSqlite($"Data Source={tmpDb}").Options;
            SoAEFContext.ThrowErrorOnContextMissing = true;
            using var ctx = new SoAEFContext(options);
            return Run(ctx, blankTestsDir, testAnswersDir);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            if (File.Exists(tmpDb)) File.Delete(tmpDb);
        }
    }

    private sealed record Table(string FileName, IEntityType EfType, IProperty Pk,
        List<Dictionary<string, JsonElement>> Records);

    private static int Run(SoAEFContext ctx, string blankTestsDir, string testAnswersDir)
    {
        // Every entity type in the model, keyed by the snake_case of its table name — the same
        // name the blank-test files carry (RoleAssignments -> role_assignments.json).
        var byFileStem = new Dictionary<string, IEntityType>();
        foreach (var et in ctx.Model.GetEntityTypes())
        {
            var stem = ToSnakeCase(et.GetTableName() ?? throw new InvalidOperationException($"{et.ClrType.Name} has no table"));
            if (!byFileStem.TryAdd(stem, et))
                throw new InvalidOperationException($"Two entity types map to {stem}.json: {byFileStem[stem].ClrType.Name}, {et.ClrType.Name}");
        }
        Console.WriteLine($"EF model: {byFileStem.Count} entity types");

        var tables = new List<Table>();
        foreach (var file in Directory.GetFiles(blankTestsDir, "*.json").OrderBy(p => p, StringComparer.Ordinal))
        {
            var fileName = Path.GetFileName(file);
            if (fileName.StartsWith("_")) continue;
            var stem = Path.GetFileNameWithoutExtension(fileName);
            if (!byFileStem.TryGetValue(stem, out var efType))
                throw new InvalidOperationException($"blank-tests/{fileName} has no matching entity in the generated EF model");
            var pkProps = efType.FindPrimaryKey()?.Properties
                ?? throw new InvalidOperationException($"{efType.ClrType.Name} has no primary key");
            if (pkProps.Count != 1)
                throw new InvalidOperationException($"{efType.ClrType.Name} has a composite key; the runner expects one PK column");
            var records = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(File.ReadAllText(file))
                ?? throw new InvalidDataException($"blank-tests/{fileName} is not a JSON array");
            tables.Add(new Table(fileName, efType, pkProps[0], records));
        }

        // ---------- SEED raw facts ----------
        var connection = ctx.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) connection.Open();
        using (var pragma = connection.CreateCommand())
        {
            // Rows are inserted file by file, not in FK order; only computed reads follow.
            pragma.CommandText = "PRAGMA foreign_keys = OFF;";
            pragma.ExecuteNonQuery();
        }

        foreach (var t in tables)
        {
            var storeObject = StoreObjectIdentifier.Table(t.EfType.GetTableName()!, t.EfType.GetSchema());
            var cols = new List<(IProperty Prop, string Key, string Column)>();
            var seen = new Dictionary<string, string>();
            foreach (var p in t.EfType.GetProperties())
            {
                if (p.IsShadowProperty()) continue;
                var key = ToSnakeCase(p.Name);
                if (seen.TryGetValue(key, out var other))
                    throw new InvalidOperationException($"{t.EfType.ClrType.Name}.{p.Name} and .{other} both map to key '{key}'");
                seen[key] = p.Name;
                var column = p.GetColumnName(storeObject) ?? throw new InvalidOperationException($"{t.EfType.ClrType.Name}.{p.Name} has no column");
                cols.Add((p, key, column));
            }
            // Blank tests omit a null key, so a column absent from one row is normal; a column
            // absent from every row may also be a naming mismatch, so it is listed.
            if (t.Records.Count > 0)
            {
                var neverPresent = cols.Where(c => t.Records.All(r => !r.ContainsKey(c.Key))).Select(c => c.Key).ToList();
                if (neverPresent.Count > 0)
                    Console.WriteLine($"  note {t.FileName}: columns in no blank-test row, seeded NULL: {string.Join(", ", neverPresent)}");
            }

            var sql = $"INSERT INTO \"{t.EfType.GetTableName()}\" ({string.Join(", ", cols.Select(c => $"\"{c.Column}\""))}) " +
                      $"VALUES ({string.Join(", ", cols.Select((_, i) => $"@p{i}"))})";
            foreach (var record in t.Records)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                for (int i = 0; i < cols.Count; i++)
                {
                    var (prop, key, _) = cols[i];
                    object? clr = null;
                    if (record.TryGetValue(key, out var je))
                    {
                        try { clr = ToClr(je, prop.ClrType); }
                        catch (System.Exception ex)
                        {
                            throw new InvalidDataException($"{t.FileName}: cannot read '{key}' = {je.GetRawText()} as {prop.ClrType.Name}: {ex.Message}", ex);
                        }
                    }
                    var mapping = (RelationalTypeMapping)(prop.GetTypeMapping());
                    cmd.Parameters.Add(mapping.CreateParameter(cmd, $"@p{i}", clr, nullable: true));
                }
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine($"  seeded {t.FileName}: {t.Records.Count} rows");
        }

        // ---------- LOAD every table, then freeze ----------
        // Load every root before reading any computed value: the generated navigation getters
        // run nested queries and Attach the results, which must land on stable, tracked rows.
        var setMethod = typeof(DbContext).GetMethods()
            .First(m => m.Name == "Set" && m.IsGenericMethod && m.GetParameters().Length == 0);
        var toList = typeof(Enumerable).GetMethod("ToList")!;
        var loaded = new Dictionary<IEntityType, List<object>>();
        foreach (var et in ctx.Model.GetEntityTypes())
        {
            var set = setMethod.MakeGenericMethod(et.ClrType).Invoke(ctx, null)!;
            var list = (System.Collections.IEnumerable)toList.MakeGenericMethod(et.ClrType).Invoke(null, new[] { set })!;
            loaded[et] = list.Cast<object>().ToList();
        }
        foreach (var t in tables)
        {
            if (loaded[t.EfType].Count != t.Records.Count)
                throw new InvalidOperationException($"{t.FileName}: seeded {t.Records.Count} rows but EF loaded {loaded[t.EfType].Count}");
        }
        ctx.FreezeComputedValues = true;

        // ---------- READ computed properties ----------
        int totalRecords = 0, propertyErrors = 0;
        var errorsByField = new SortedDictionary<string, (int Count, string Sample)>(StringComparer.Ordinal);
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        foreach (var t in tables)
        {
            var clr = t.EfType.ClrType;
            var mappedKeys = t.EfType.GetProperties().Where(p => !p.IsShadowProperty()).Select(p => ToSnakeCase(p.Name)).ToHashSet();
            var computed = new Dictionary<string, PropertyInfo>();
            foreach (var pi in clr.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>() == null) continue;
                if (!pi.CanRead || pi.GetIndexParameters().Length > 0) continue;
                var key = ToSnakeCase(pi.Name);
                if (!computed.TryAdd(key, pi))
                    throw new InvalidOperationException($"{clr.Name}.{pi.Name} and .{computed[key].Name} both map to key '{key}'");
            }

            var pkGetter = t.Pk.PropertyInfo ?? throw new InvalidOperationException($"{clr.Name}.{t.Pk.Name} is not a CLR property");
            var byPk = new Dictionary<string, object>();
            foreach (var e in loaded[t.EfType])
            {
                var pk = Convert.ToString(pkGetter.GetValue(e), CultureInfo.InvariantCulture)
                    ?? throw new InvalidOperationException($"{clr.Name} row with a null primary key");
                if (!byPk.TryAdd(pk, e)) throw new InvalidOperationException($"{clr.Name}: duplicate primary key {pk}");
            }
            var pkKey = ToSnakeCase(t.Pk.Name);

            var unmatched = new HashSet<string>();
            var outRecords = new List<Dictionary<string, object?>>();
            foreach (var record in t.Records)
            {
                if (!record.TryGetValue(pkKey, out var pkJe) || pkJe.ValueKind == JsonValueKind.Null)
                    throw new InvalidDataException($"{t.FileName}: a row has no '{pkKey}'");
                var pkText = pkJe.ValueKind == JsonValueKind.String ? pkJe.GetString()! : pkJe.GetRawText();
                if (!byPk.TryGetValue(pkText, out var entity))
                    throw new InvalidOperationException($"{t.FileName}: row {pkText} was seeded but not loaded");

                var outRec = new Dictionary<string, object?>();
                foreach (var (key, je) in record)
                {
                    if (computed.TryGetValue(key, out var pi))
                    {
                        try
                        {
                            outRec[key] = pi.GetValue(entity);
                        }
                        catch (System.Exception ex)
                        {
                            // Written as null so the grader counts it; reported below.
                            outRec[key] = null;
                            propertyErrors++;
                            var inner = ex is TargetInvocationException { InnerException: not null } tie ? tie.InnerException! : ex;
                            var field = $"{clr.Name}.{pi.Name}";
                            errorsByField[field] = errorsByField.TryGetValue(field, out var prev)
                                ? (prev.Count + 1, prev.Sample)
                                : (1, $"{inner.GetType().Name}: {inner.Message}");
                        }
                    }
                    else
                    {
                        // Raw input column: echoed verbatim.
                        if (!mappedKeys.Contains(key)) unmatched.Add(key);
                        outRec[key] = JsonElementToObject(je);
                    }
                }
                outRecords.Add(outRec);
            }
            if (unmatched.Count > 0)
                Console.WriteLine($"  WARNING {t.FileName}: keys with no {clr.Name} property, echoed from the blank test: {string.Join(", ", unmatched.OrderBy(k => k))}");

            File.WriteAllText(Path.Combine(testAnswersDir, t.FileName), JsonSerializer.Serialize(outRecords, jsonOptions));
            Console.WriteLine($"  -> {t.FileName} ({clr.Name}): {outRecords.Count} records");
            totalRecords += outRecords.Count;
        }

        if (propertyErrors > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"ERROR: {propertyErrors} computed-property reads threw (written as null):");
            foreach (var (field, (count, sample)) in errorsByField)
                Console.WriteLine($"  {field}: {count}x  {sample}");
        }

        Console.WriteLine();
        Console.WriteLine($"effortless-entity-framework: Processed {tables.Count} entities, {totalRecords} total records");
        return 0;
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    /// <summary>Exact port of orchestration/shared.py to_snake_case, the blank tests' key naming.</summary>
    private static string ToSnakeCase(string name)
    {
        var s1 = Regex.Replace(name, "([^_])([A-Z][a-z]+)", "$1_$2");
        return Regex.Replace(s1, "([a-z0-9])([A-Z])", "$1_$2").ToLowerInvariant();
    }

    /// <summary>A blank-test JSON value as the CLR value of the mapped property.</summary>
    private static object? ToClr(JsonElement je, Type target)
    {
        var t = Nullable.GetUnderlyingType(target) ?? target;
        if (je.ValueKind == JsonValueKind.Null) return null;
        if (t == typeof(string)) return je.ValueKind == JsonValueKind.String ? je.GetString() : je.GetRawText();
        // A blank string is SQL NULL for every non-text column.
        if (je.ValueKind == JsonValueKind.String && je.GetString()!.Trim().Length == 0) return null;
        var inv = CultureInfo.InvariantCulture;
        if (t == typeof(bool))
        {
            if (je.ValueKind is JsonValueKind.True or JsonValueKind.False) return je.GetBoolean();
            throw new FormatException("not a JSON boolean");
        }
        if (t == typeof(int)) return je.ValueKind == JsonValueKind.String ? int.Parse(je.GetString()!, inv) : je.GetInt32();
        if (t == typeof(long)) return je.ValueKind == JsonValueKind.String ? long.Parse(je.GetString()!, inv) : je.GetInt64();
        if (t == typeof(decimal)) return je.ValueKind == JsonValueKind.String ? decimal.Parse(je.GetString()!, NumberStyles.Float, inv) : je.GetDecimal();
        if (t == typeof(double)) return je.ValueKind == JsonValueKind.String ? double.Parse(je.GetString()!, NumberStyles.Float, inv) : je.GetDouble();
        if (t == typeof(DateTimeOffset)) return DateTimeOffset.Parse(je.GetString()!, inv, DateTimeStyles.RoundtripKind);
        if (t == typeof(DateTime)) return DateTime.Parse(je.GetString()!, inv, DateTimeStyles.RoundtripKind);
        if (t == typeof(DateOnly)) return DateOnly.Parse(je.GetString()!, inv);
        if (t == typeof(Guid)) return Guid.Parse(je.GetString()!);
        throw new NotSupportedException($"no JSON conversion to {t.Name}");
    }

    private static object? JsonElementToObject(JsonElement je) => je.ValueKind switch
    {
        JsonValueKind.String => je.GetString(),
        JsonValueKind.Number => je.TryGetInt64(out var l) ? l : je.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        _ => je.Clone(),
    };
}
