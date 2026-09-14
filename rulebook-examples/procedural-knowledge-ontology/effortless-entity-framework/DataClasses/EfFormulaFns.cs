// Its own namespace, with the usings inside it: a rulebook table becomes a class in
// SqlOnAir.DotNet.Lib.DataClasses, and a table named Exceptions or Environments would otherwise
// shadow the System type of the same name everywhere in this file.
namespace SqlOnAir.DotNet.Lib.DataClasses.Formulas
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Formula runtime for the generated computed-property getters. Every rulebook formula
    /// compiles to calls on this class, so there is one implementation of the dialect here.
    ///
    /// The semantics are the Postgres oracle's, as rulebook-to-python and rulebook-to-go
    /// implement them: NULL propagates through comparisons and AND/OR/NOT, a raw text column
    /// reads through NULLIF(x, ''), a raw boolean is COALESCE(x, FALSE) in a boolean context,
    /// DATETIME_DIFF counts calendar days, CONCAT renders a datetime as timestamptz text, a
    /// field declared integer is cast the way ::integer casts. When a rule changes in those
    /// tools it changes here.
    ///
    /// Formulas compute in <see cref="V"/>, a dynamically typed value, because their operands
    /// are: NULL is a value of every type, a lookup returns whatever its target column holds,
    /// and an IF can mix branches. The entity properties stay typed.
    /// </summary>
    public static class EfFormulaFns
    {
        // ───────────────────────────── build parameters ─────────────────────────────
        //
        // The ERB build parameters (docs/ERB-BUILD-PARAMETERS.md) this SDK was generated
        // under, baked in by rulebook-to-entity-framework. The static constructor installs
        // them, so nothing here ever runs unconfigured. erb_runtime.py is the reference;
        // when a rule changes there, it changes here.

        static readonly Dictionary<string, string> GeneratedParameters = new Dictionary<string, string>
        {
            { "erbBlankLogic", "coerce" },
            { "erbDateDiff", "calendar" },
            { "erbDateTimeText", "iso8601" },
            { "erbTimezone", "UTC" },
            { "erbWholeNumber", "by-field-type" }
        };

        static readonly Dictionary<string, string[]> ParameterValues = new Dictionary<string, string[]>
        {
            { "erbDateDiff", new[] { "calendar", "elapsed" } },
            { "erbTimezone", new[] { "UTC" } }, // or any IANA zone name
            { "erbDateTimeText", new[] { "iso8601", "sql" } },
            { "erbBlankLogic", new[] { "coerce", "propagate" } },
            { "erbWholeNumber", new[] { "by-field-type", "integer", "decimal" } },
        };

        static IReadOnlyDictionary<string, string> Params;
        static TimeZoneInfo Zone;

        static EfFormulaFns() { Configure(GeneratedParameters); }

        public static void Configure(IReadOnlyDictionary<string, string> parameters)
        {
            foreach (var name in parameters.Keys)
                if (!ParameterValues.ContainsKey(name))
                    throw new InvalidOperationException($"EfFormulaFns.Configure: unknown ERB build parameter '{name}'");
            foreach (var (name, allowed) in ParameterValues)
            {
                if (!parameters.TryGetValue(name, out var value))
                    throw new InvalidOperationException($"EfFormulaFns.Configure: missing ERB build parameter '{name}'");
                if (name != "erbTimezone" && !allowed.Contains(value))
                    throw new InvalidOperationException($"EfFormulaFns.Configure: {name}='{value}' is not one of [{string.Join(", ", allowed)}]");
            }
            if (parameters["erbWholeNumber"] != "by-field-type")
                throw new InvalidOperationException($"EfFormulaFns.Configure: an entity property is typed by its declared datatype; erbWholeNumber must be by-field-type, not '{parameters["erbWholeNumber"]}'");
            var zoneName = parameters["erbTimezone"];
            try
            {
                Zone = zoneName == "UTC" ? TimeZoneInfo.Utc : TimeZoneInfo.FindSystemTimeZoneById(zoneName);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"EfFormulaFns.Configure: erbTimezone='{zoneName}' cannot be resolved: {ex.Message}", ex);
            }
            Params = new Dictionary<string, string>(parameters);
        }

        public static IReadOnlyDictionary<string, string> Parameters => Params;

        static string Param(string name) => Params[name];
        static bool CoerceBlanks => Param("erbBlankLogic") == "coerce";

        /// <summary>The instant v names, expressed in erbTimezone. A value carrying no offset is
        /// taken to be in erbTimezone.</summary>
        static DateTimeOffset InZone(V v)
        {
            switch (v.K)
            {
                case VK.Time:
                    if (v.Zoned) return TimeZoneInfo.ConvertTime(v.T, Zone);
                    return WallInZone(v.T.DateTime);
                case VK.Str:
                    var s = v.S.Trim();
                    if (ParseIso(s, out var t, out var hasZone))
                        return hasZone ? TimeZoneInfo.ConvertTime(t, Zone) : WallInZone(t.DateTime);
                    if (s.Length >= 10 && DateTime.TryParseExact(s.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                        return WallInZone(d);
                    break;
            }
            throw new InvalidOperationException($"cannot read {PyRepr(v)} as a datetime");
        }

        static DateTimeOffset WallInZone(DateTime wall)
        {
            var unspecified = DateTime.SpecifyKind(wall, DateTimeKind.Unspecified);
            return new DateTimeOffset(unspecified, Zone.GetUtcOffset(unspecified));
        }

        // ───────────────────────────── values ─────────────────────────────

        public enum VK { Null, Bool, Num, Str, Time }

        public readonly struct V
        {
            public readonly VK K;
            public readonly bool B;
            public readonly double N;
            public readonly bool IsInt;
            public readonly string S;
            public readonly DateTimeOffset T;
            public readonly bool Zoned;

            public V(VK k, bool b = false, double n = 0, bool isInt = false, string s = null,
                     DateTimeOffset t = default, bool zoned = false)
            {
                K = k; B = b; N = n; IsInt = isInt; S = s; T = t; Zoned = zoned;
            }

            public override string ToString() => PyRepr(this);
        }

        public static readonly V Null = new V(VK.Null);
        public static V B(bool b) => new V(VK.Bool, b: b);
        public static V S(string s) => s == null ? Null : new V(VK.Str, s: s);
        public static V I(long n) => new V(VK.Num, n: n, isInt: true);
        public static V D(double n) => new V(VK.Num, n: n);
        static V Wall(DateTime t) => new V(VK.Time, t: new DateTimeOffset(DateTime.SpecifyKind(t, DateTimeKind.Unspecified), TimeSpan.Zero));
        static V Zoned(DateTimeOffset t) => new V(VK.Time, t: t, zoned: true);

        public static V Of(string v) => S(v);
        public static V Of(bool? v) => v.HasValue ? B(v.Value) : Null;
        public static V Of(int? v) => v.HasValue ? I(v.Value) : Null;
        public static V Of(long? v) => v.HasValue ? I(v.Value) : Null;
        public static V Of(double? v) => v.HasValue ? NumberValue(v.Value) : Null;
        public static V Of(decimal? v) => v.HasValue ? NumberValue((double)v.Value) : Null;
        public static V Of(DateTimeOffset? v) => v.HasValue ? Zoned(v.Value) : Null;
        public static V Of(DateTime? v) => v.HasValue ? Wall(v.Value) : Null;
        public static V Of(DateOnly? v) => v.HasValue ? S(v.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)) : Null;
        public static V Of(Guid? v) => v.HasValue ? S(v.Value.ToString()) : Null;
        public static V Of(V v) => v;

        static V NumberValue(double n) =>
            n == Math.Truncate(n) && Math.Abs(n) < 1e15 ? I((long)n) : D(n);

        // ───────────────────────────── typed boundary ─────────────────────────────

        public static string AsString(V v) => v.K switch
        {
            VK.Null => null,
            VK.Str => v.S,
            _ => PyStr(v)
        };

        public static bool? AsBool(V v)
        {
            switch (v.K)
            {
                case VK.Null: return null;
                case VK.Bool: return v.B;
                case VK.Num: return v.N != 0;
                case VK.Str:
                    var s = v.S.Trim().ToLowerInvariant();
                    if (s == "true") return true;
                    if (s == "false") return false;
                    if (s == "") return null;
                    break;
            }
            throw new InvalidCastException($"cannot store {PyRepr(v)} in a boolean property");
        }

        public static double? AsDouble(V v)
        {
            switch (v.K)
            {
                case VK.Null: return null;
                case VK.Num: return v.N;
                case VK.Bool: return v.B ? 1 : 0;
                case VK.Str:
                    if (v.S.Trim() == "") return null;
                    if (ToNumber(v, out var n)) return n.N;
                    break;
            }
            throw new InvalidCastException($"cannot store {PyRepr(v)} in a numeric property");
        }

        public static decimal? AsDecimal(V v) { var d = AsDouble(v); return d.HasValue ? (decimal?)(decimal)d.Value : null; }
        public static long? AsLong(V v) { var d = AsDouble(v); return d.HasValue ? (long?)(long)RoundDecimal(d.Value, 0, false) : null; }
        public static int? AsInt(V v) { var d = AsDouble(v); return d.HasValue ? (int?)(int)RoundDecimal(d.Value, 0, false) : null; }

        public static DateTimeOffset? AsDateTime(V v)
        {
            switch (v.K)
            {
                case VK.Null: return null;
                case VK.Time: return v.T;
                case VK.Str:
                    if (v.S.Trim() == "") return null;
                    if (ParseIso(v.S, out var t, out _)) return t;
                    break;
            }
            throw new InvalidCastException($"cannot store {PyRepr(v)} in a datetime property");
        }

        public static DateOnly? AsDate(V v)
        {
            var t = AsDateTime(v);
            return t.HasValue ? DateOnly.FromDateTime(t.Value.DateTime) : null;
        }

        public static Guid? AsGuid(V v) => v.K == VK.Null ? null : Guid.Parse(PyStr(v));

        // ───────────────────────────── text rendering ─────────────────────────────

        /// <summary>str(x) as the reference implementation renders it.</summary>
        public static string PyStr(V v)
        {
            switch (v.K)
            {
                case VK.Null: return "None";
                case VK.Bool: return v.B ? "True" : "False";
                case VK.Num: return v.IsInt ? ((long)v.N).ToString(CultureInfo.InvariantCulture) : PyFloat(v.N);
                case VK.Str: return v.S;
                case VK.Time:
                    var s = v.T.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    var micros = v.T.Ticks % TimeSpan.TicksPerSecond / 10;
                    if (micros != 0) s += "." + micros.ToString("D6", CultureInfo.InvariantCulture);
                    if (v.Zoned) s += v.T.ToString("zzz", CultureInfo.InvariantCulture);
                    return s;
            }
            return "";
        }

        public static string PyRepr(V v) => v.K == VK.Str ? "'" + v.S + "'" : PyStr(v);

        static string PyFloat(double f)
        {
            if (double.IsPositiveInfinity(f)) return "inf";
            if (double.IsNegativeInfinity(f)) return "-inf";
            if (double.IsNaN(f)) return "nan";
            var r = f.ToString("R", CultureInfo.InvariantCulture);
            if (r.Contains('E'))
            {
                var exp = Math.Floor(Math.Log10(Math.Abs(f)));
                if (exp >= -4 && exp < 16)
                    r = ((decimal)f).ToString(CultureInfo.InvariantCulture);
                else
                {
                    var parts = r.Split('E');
                    var e = int.Parse(parts[1], CultureInfo.InvariantCulture);
                    return $"{parts[0]}e{(e < 0 ? "-" : "+")}{Math.Abs(e):D2}";
                }
            }
            if (!r.Contains('.')) r += ".0";
            return r;
        }

        /// <summary>Python truthiness, where compiled code reads <c>x or ""</c>.</summary>
        public static bool Truthy(V v) => v.K switch
        {
            VK.Null => false,
            VK.Bool => v.B,
            VK.Num => v.N != 0,
            VK.Str => v.S.Length > 0,
            _ => true
        };

        /// <summary><c>str(x or "")</c></summary>
        public static V TextOr(V v) => Truthy(v) ? S(PyStr(v)) : S("");

        /// <summary><c>str(x if x is not None else "")</c></summary>
        public static V TextNotNull(V v) => v.K == VK.Null ? S("") : S(PyStr(v));

        public static V Concat(params V[] parts)
        {
            var sb = new StringBuilder();
            foreach (var p in parts) sb.Append(p.S);
            return S(sb.ToString());
        }

        // ───────────────────────────── numbers ─────────────────────────────

        static readonly Regex IntText = new Regex(@"^[+-]?\d+$", RegexOptions.Compiled);

        static bool ToNumber(V v, out V number)
        {
            number = Null;
            if (v.K == VK.Num) { number = v; return true; }
            if (v.K != VK.Str) return false;
            var s = v.S.Trim();
            if (s.Length == 0) return false;
            if (IntText.IsMatch(s) && long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i)) { number = I(i); return true; }
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)) { number = D(d); return true; }
            return false;
        }

        static V NumOrZero(V v) => ToNumber(v, out var n) ? n : I(0);

        static V Arith(V a, V b, char op)
        {
            var n = op switch { '+' => a.N + b.N, '-' => a.N - b.N, _ => a.N * b.N };
            return new V(VK.Num, n: n, isInt: a.IsInt && b.IsInt);
        }

        static V OrZero(V v)
        {
            if (!Truthy(v)) return I(0);
            if (v.K != VK.Num) throw new InvalidOperationException($"expected a number, got {PyRepr(v)}");
            return v;
        }

        public static V Neg(V v) { var x = OrZero(v); return new V(VK.Num, n: -x.N, isInt: x.IsInt); }

        public static V Add(V a, V b)
        {
            var ad = DateOrNone(a, out var at);
            var bd = DateOrNone(b, out var bt);
            if (ad && !bd) return Wall(at + DaysDelta(b));
            if (bd && !ad) return Wall(bt + DaysDelta(a));
            return Arith(NumOrZero(a), NumOrZero(b), '+');
        }

        public static V Sub(V a, V b)
        {
            if (DateOrNone(a, out var at) && !DateOrNone(b, out _)) return Wall(at - DaysDelta(b));
            return Arith(NumOrZero(a), NumOrZero(b), '-');
        }

        public static V Mul(V a, V b) => Arith(NumOrZero(a), NumOrZero(b), '*');

        public static V Div(V a, V b)
        {
            var d = NumOrZero(b);
            return d.N == 0 ? Null : D(NumOrZero(a).N / d.N);
        }

        static TimeSpan DaysDelta(V v)
        {
            if (!Truthy(v)) return TimeSpan.Zero;
            return ToNumber(v, out var n) ? TimeSpan.FromDays(n.N) : TimeSpan.Zero;
        }

        /// <summary>Decimal(str(x)).quantize(10^-digits): half away from zero, or toward +inf.</summary>
        static double RoundDecimal(double f, int digits, bool ceiling)
        {
            var d = decimal.Parse(f.ToString("R", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
            var scale = 1m;
            for (int i = 0; i < Math.Abs(digits); i++) scale *= 10m;
            decimal shifted = digits >= 0 ? d * scale : d / scale;
            shifted = ceiling ? Math.Ceiling(shifted) : Math.Round(shifted, 0, MidpointRounding.AwayFromZero);
            return (double)(digits >= 0 ? shifted / scale : shifted * scale);
        }

        static int Digits(V v)
        {
            if (!Truthy(v) && v.K != VK.Num) return 0;
            if (!ToNumber(v, out var n)) throw new InvalidOperationException($"invalid digits {PyRepr(v)}");
            return (int)n.N;
        }

        public static V Round(V v, V digits)
        {
            if (v.K == VK.Null || (v.K == VK.Str && v.S == "")) return Null;
            if (!ToNumber(v, out var n)) throw new InvalidOperationException($"cannot ROUND {PyRepr(v)}");
            return D(RoundDecimal(n.N, Digits(digits), false));
        }

        public static V Roundup(V v, V digits)
        {
            if (v.K == VK.Null || (v.K == VK.Str && v.S == "")) return Null;
            if (!ToNumber(v, out var n)) throw new InvalidOperationException($"cannot ROUNDUP {PyRepr(v)}");
            return D(RoundDecimal(n.N, Digits(digits), true));
        }

        /// <summary>The oracle's ::integer cast on a field declared integer.</summary>
        public static V Integer(V v)
        {
            if (v.K == VK.Null || v.K == VK.Bool) return v;
            return ToNumber(v, out var n) ? I((long)RoundDecimal(n.N, 0, false)) : v;
        }

        public static V Abs(V v) { var x = OrZero(v); return new V(VK.Num, n: Math.Abs(x.N), isInt: x.IsInt); }
        public static V Power(V b, V e) { var x = OrZero(b); var y = OrZero(e); return new V(VK.Num, n: Math.Pow(x.N, y.N), isInt: x.IsInt && y.IsInt && y.N >= 0); }
        public static V Sqrt(V v) => D(Math.Sqrt(OrZero(v).N));
        public static V Tan(V v) => D(Math.Tan(OrZero(v).N));
        public static V Log10(V v) => D(Math.Log10(OrZero(v).N));
        public static V Log(V v, V b) => D(Math.Log(OrZero(v).N) / Math.Log(OrZero(b).N));
        public static V Pi() => D(Math.PI);

        public static V MaxMin(bool max, params V[] args)
        {
            var best = OrZero(args[0]);
            foreach (var a in args.Skip(1))
            {
                var x = OrZero(a);
                if (max ? x.N > best.N : x.N < best.N) best = x;
            }
            return best;
        }

        public static V Sum(params V[] args)
        {
            var total = I(0);
            foreach (var a in args) total = Arith(total, NumOrZero(a), '+');
            return total;
        }

        // ───────────────────────────── logic ─────────────────────────────

        public static V Bool3(V v) => v.K == VK.Null || v.K == VK.Bool ? v : B(false);
        public static V IsTrueV(V v) => B(v.K == VK.Bool && v.B);
        public static V HasValue(V v) => B(!Blank(v));

        /// <summary>AND: under erbBlankLogic=coerce a blank operand is FALSE; under propagate FALSE
        /// if any operand is FALSE, else NULL if any is NULL, else TRUE.</summary>
        public static V And(params V[] values)
        {
            if (values.Any(v => v.K == VK.Bool && !v.B)) return B(false);
            if (values.Any(v => v.K == VK.Null)) return CoerceBlanks ? B(false) : Null;
            return B(true);
        }

        public static V Or(params V[] values)
        {
            if (values.Any(v => v.K == VK.Bool && v.B)) return B(true);
            if (values.Any(v => v.K == VK.Null)) return CoerceBlanks ? B(false) : Null;
            return B(false);
        }

        /// <summary>NOT: coerce makes NOT(blank) TRUE; propagate keeps it NULL.</summary>
        public static V Not(V v) => v.K == VK.Null ? (CoerceBlanks ? B(true) : Null) : B(!Truthy(v));

        static bool Blank(V v) => v.K == VK.Null || (v.K == VK.Str && v.S.Length == 0);
        public static V IsBlank(V v) => B(Blank(v));
        public static V IsNotBlank(V v) => B(!Blank(v));
        public static V Nullif(V v) => v.K == VK.Str && v.S.Length == 0 ? Null : v;

        static bool NumericLike(V v, out double n)
        {
            n = 0;
            if (v.K == VK.Num) { n = v.N; return true; }
            if (v.K == VK.Bool) { n = v.B ? 1 : 0; return true; }
            return false;
        }

        /// <summary>Python ==.</summary>
        public static bool PyEqual(V a, V b)
        {
            if (NumericLike(a, out var an) && NumericLike(b, out var bn)) return an == bn;
            if (a.K != b.K) return false;
            return a.K switch
            {
                VK.Null => true,
                VK.Str => a.S == b.S,
                VK.Time => a.T == b.T,
                _ => false
            };
        }

        /// <summary>Under coerce a blank compared against <paramref name="other"/> takes the other
        /// side's zero: FALSE against a boolean, 0 against a number, "" otherwise.</summary>
        static V BlankAsZeroOf(V other)
        {
            if (other.K == VK.Bool) return B(false);
            if (ToNumber(other, out _)) return I(0);
            return S("");
        }

        /// <summary>Resolves blank operands per erbBlankLogic: true with the pair to compare, or
        /// false when the blank must propagate as NULL.</summary>
        static bool CoercedPair(ref V a, ref V b)
        {
            if (a.K == VK.Null && b.K == VK.Null)
            {
                if (!CoerceBlanks) return false;
                a = S(""); b = S(""); return true;
            }
            if (a.K == VK.Null || b.K == VK.Null)
            {
                if (!CoerceBlanks) return false;
                if (a.K == VK.Null) a = BlankAsZeroOf(b); else b = BlankAsZeroOf(a);
            }
            return true;
        }

        public static V Eq(V a, V b) => CoercedPair(ref a, ref b) ? B(PyEqual(a, b)) : Null;
        public static V Ne(V a, V b) => CoercedPair(ref a, ref b) ? B(!PyEqual(a, b)) : Null;

        /// <summary>Numbers and numeric strings as numbers, two strings as strings, two
        /// booleans or two datetimes in order; a blank is NULL under propagate and the other
        /// side's zero under coerce; anything else is FALSE.</summary>
        public static V Cmp(V a, string op, V b)
        {
            if (!CoercedPair(ref a, ref b)) return Null;
            int c;
            if (ToNumber(a, out var an) && ToNumber(b, out var bn)) c = an.N.CompareTo(bn.N);
            else if (a.K == VK.Str && b.K == VK.Str) c = string.CompareOrdinal(a.S, b.S);
            else if (a.K == VK.Bool && b.K == VK.Bool) c = a.B.CompareTo(b.B);
            else if (a.K == VK.Time && b.K == VK.Time) c = a.T.CompareTo(b.T);
            else return B(false);
            return op switch
            {
                "<" => B(c < 0),
                "<=" => B(c <= 0),
                ">" => B(c > 0),
                _ => B(c >= 0)
            };
        }

        public static V Coalesce(params V[] values)
        {
            foreach (var v in values) if (!Blank(v)) return v;
            return Null;
        }

        public static V Try(Func<V> main, Func<V> fallback)
        {
            try { return main(); }
            catch (Exception) { return fallback(); }
        }

        public static V IsError(Func<V> main)
        {
            try { main(); return B(false); }
            catch (Exception) { return B(true); }
        }

        public static V Switch(V test, Func<V> otherwise, params (V Key, Func<V> Value)[] cases)
        {
            foreach (var (key, value) in cases)
                if (Truthy(Eq(test, key))) return value();
            return otherwise();
        }

        // ───────────────────────────── strings ─────────────────────────────

        static string TextOperand(V v, string fn)
        {
            if (!Truthy(v)) return "";
            if (v.K != VK.Str) throw new InvalidOperationException($"{fn} expects text, got {PyRepr(v)}");
            return v.S;
        }

        static int CountOperand(V v, string fn)
        {
            if (!Truthy(v)) return 0;
            if (v.K != VK.Num || !v.IsInt) throw new InvalidOperationException($"{fn} expects an integer count, got {PyRepr(v)}");
            return (int)v.N;
        }

        static int[] CodePoints(string s)
        {
            var list = new List<int>(s.Length);
            for (int i = 0; i < s.Length; i += char.IsSurrogatePair(s, i) ? 2 : 1) list.Add(char.ConvertToUtf32(s, i));
            return list.ToArray();
        }

        static string FromCodePoints(IEnumerable<int> cps) => string.Concat(cps.Select(char.ConvertFromUtf32));

        public static V Lower(V v) => S(TextOperand(v, "LOWER").ToLowerInvariant());
        public static V Upper(V v) => S(TextOperand(v, "UPPER").ToUpperInvariant());
        public static V Trim(V v) => S(TextOperand(v, "TRIM").Trim(' '));
        public static V Len(V v) => I(CodePoints(TextOperand(v, "LEN")).Length);

        public static V Left(V text, V count)
        {
            var cps = CodePoints(TextOperand(text, "LEFT"));
            var n = CountOperand(count, "LEFT");
            return S(FromCodePoints(cps.Take(Math.Max(0, n))));
        }

        public static V Right(V text, V count)
        {
            var cps = CodePoints(TextOperand(text, "RIGHT"));
            var n = CountOperand(count, "RIGHT");
            return n <= 0 ? S("") : S(FromCodePoints(cps.Skip(Math.Max(0, cps.Length - n))));
        }

        public static V Mid(V text, V start, V count)
        {
            var cps = CodePoints(TextOperand(text, "MID"));
            var n = CountOperand(count, "MID");
            var s = Truthy(start) ? (int)NumOrZero(start).N : 1;
            return n <= 0 ? S("") : S(FromCodePoints(cps.Skip(Math.Max(0, s - 1)).Take(n)));
        }

        public static V Substitute(V text, V old, V replacement)
        {
            if (old.K != VK.Str || replacement.K != VK.Str) throw new InvalidOperationException("SUBSTITUTE expects text arguments");
            var t = TextOperand(text, "SUBSTITUTE");
            return S(old.S.Length == 0 ? t : t.Replace(old.S, replacement.S, StringComparison.Ordinal));
        }

        public static V Find(V needle, V haystack)
        {
            if (needle.K == VK.Null || haystack.K == VK.Null) return Null;
            var h = PyStr(haystack);
            var i = h.IndexOf(PyStr(needle), StringComparison.Ordinal);
            return i < 0 ? I(0) : I(CodePoints(h.Substring(0, i)).Length + 1);
        }

        public static V Cast(V v) => Truthy(v) ? S(PyStr(v)) : S("");

        public static V Text(V v, V format)
        {
            if (v.K == VK.Null) return S("");
            var fmt = format.K == VK.Str ? format.S : "";
            if (v.K == VK.Time) return S(FormatDate(v.T, fmt));
            if (ToNumber(v, out var n) && fmt.Length > 0)
            {
                int decimals = fmt.Contains('.') ? fmt.Length - fmt.IndexOf('.') - 1 : 0;
                return S(n.N.ToString("F" + decimals, CultureInfo.InvariantCulture));
            }
            return S(PyStr(v));
        }

        // ───────────────────────────── dates ─────────────────────────────

        static readonly Regex Iso = new Regex(
            @"^(\d{4})-(\d{2})-(\d{2})(?:[T ](\d{2})(?::(\d{2})(?::(\d{2})(?:[.,](\d+))?)?)?)?\s*(Z|[+-]\d{2}(?::?\d{2}(?::?\d{2})?)?)?$",
            RegexOptions.Compiled);

        static bool ParseIso(string s, out DateTimeOffset t, out bool hasZone)
        {
            t = default; hasZone = false;
            var m = Iso.Match(s.Trim());
            if (!m.Success) return false;
            int G(int i) => m.Groups[i].Success && m.Groups[i].Value.Length > 0 ? int.Parse(m.Groups[i].Value, CultureInfo.InvariantCulture) : 0;
            var ticks = 0L;
            if (m.Groups[7].Success) ticks = long.Parse((m.Groups[7].Value + "0000000").Substring(0, 7), CultureInfo.InvariantCulture);
            var offset = TimeSpan.Zero;
            if (m.Groups[8].Success && m.Groups[8].Value.Length > 0)
            {
                hasZone = true;
                var z = m.Groups[8].Value;
                if (z != "Z")
                {
                    var digits = z.Substring(1).Replace(":", "");
                    var span = new TimeSpan(int.Parse(digits.Substring(0, 2), CultureInfo.InvariantCulture),
                                            digits.Length >= 4 ? int.Parse(digits.Substring(2, 2), CultureInfo.InvariantCulture) : 0,
                                            digits.Length >= 6 ? int.Parse(digits.Substring(4, 2), CultureInfo.InvariantCulture) : 0);
                    offset = z[0] == '-' ? -span : span;
                }
            }
            t = new DateTimeOffset(G(1), G(2), G(3), G(4), G(5), G(6), offset).AddTicks(ticks);
            return true;
        }

        static readonly Regex DatePrefix = new Regex(@"^\d{4}-\d{2}-\d{2}", RegexOptions.Compiled);

        static DateTime WallClock(DateTimeOffset t) => t.DateTime;

        static bool DateOrNone(V v, out DateTime t)
        {
            t = default;
            if (v.K != VK.Str) return false;
            var s = v.S.Trim();
            if (!DatePrefix.IsMatch(s)) return false;
            if (ParseIso(s, out var parsed, out _)) { t = WallClock(parsed); return true; }
            return DateTime.TryParseExact(s.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out t);
        }

        /// <summary>The wall-clock reading of v in erbTimezone (date arithmetic's operand).</summary>
        static DateTime CoerceDatetime(V v) => InZone(v).DateTime;

        /// <summary>DATETIME_DIFF(end, start, unit) per erbDateDiff: hour, minute and second are
        /// exact elapsed (possibly fractional) in both modes; month and year are whole calendar
        /// months (AGE()) in both. calendar: day is calendar dates in erbTimezone subtracted,
        /// week is day/7 truncated. elapsed: day is seconds/86400 truncated, week
        /// seconds/604800 truncated.</summary>
        public static V DatetimeDiff(V end, V start, V unitValue)
        {
            var unit = Truthy(unitValue) ? PyStr(unitValue).ToLowerInvariant().TrimEnd('s') : "day";
            if (Blank(end) || Blank(start)) return Null;
            var ez = InZone(end);
            var sz = InZone(start);
            var e = ez.DateTime;
            var s = sz.DateTime;
            var seconds = (ez - sz).TotalSeconds;
            var calendar = Param("erbDateDiff") == "calendar";
            long CalendarDays() => (long)Math.Round((e.Date - s.Date).TotalDays);
            switch (unit)
            {
                case "hour": return D(seconds / 3600);
                case "minute": return D(seconds / 60);
                case "second": return D(seconds);
                case "day": return I(calendar ? CalendarDays() : (long)Math.Truncate(seconds / 86400));
                case "week": return I(calendar ? (long)Math.Truncate(CalendarDays() / 7.0) : (long)Math.Truncate(seconds / 604800));
                case "month":
                case "year":
                    var months = (e.Year - s.Year) * 12 + e.Month - s.Month;
                    var ec = (e.Day, e.Hour, e.Minute, e.Second);
                    var sc = (s.Day, s.Hour, s.Minute, s.Second);
                    if (ec.CompareTo(sc) < 0) months--;
                    return unit == "month" ? I(months) : I((long)Math.Truncate(months / 12.0));
            }
            throw new InvalidOperationException($"DATETIME_DIFF: unsupported unit '{unit}'");
        }

        /// <summary>NOW()/TODAY(), an instant in erbTimezone. FORMULA_NOW pins it.</summary>
        public static V Now()
        {
            var overrideText = Environment.GetEnvironmentVariable("FORMULA_NOW");
            if (!string.IsNullOrEmpty(overrideText))
            {
                if (!ParseIso(overrideText, out _, out _))
                    throw new InvalidOperationException($"FORMULA_NOW '{overrideText}' is not an ISO-8601 datetime");
                return Zoned(InZone(S(overrideText)));
            }
            return Zoned(TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, Zone));
        }

        public static V DateAdd(V date, V amount, V unitValue)
        {
            if (date.K == VK.Null) return Null;
            var d = CoerceDatetime(date);
            var n = (int)NumOrZero(amount).N;
            var unit = Truthy(unitValue) ? PyStr(unitValue).Trim().ToLowerInvariant().TrimEnd('s') : "day";
            var r = unit switch
            {
                "year" => d.AddYears(n),
                "month" => d.AddMonths(n),
                "week" => d.AddDays(n * 7),
                "hour" => d.AddHours(n),
                "minute" => d.AddMinutes(n),
                "second" => d.AddSeconds(n),
                _ => d.AddDays(n)
            };
            return Wall(r);
        }

        public static V DatetimeFormat(V date, V format)
        {
            if (date.K == VK.Null) return Null;
            var t = date.K == VK.Time ? date.T : new DateTimeOffset(CoerceDatetime(date), TimeSpan.Zero);
            return S(FormatDate(t, format.K == VK.Str ? format.S : ""));
        }

        static string FormatDate(DateTimeOffset dt, string fmt)
        {
            if (string.IsNullOrEmpty(fmt)) return dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var net = fmt.Replace("YYYY", "yyyy").Replace("YY", "yy").Replace("DD", "dd");
            return dt.ToString(net, CultureInfo.InvariantCulture);
        }

        static readonly Regex DateOnlyText = new Regex(@"^\d{4}-\d{2}-\d{2}$", RegexOptions.Compiled);

        /// <summary>A datetime rendered into text per erbDateTimeText, in erbTimezone:
        /// iso8601 = <c>2026-04-03T14:00:00+00:00</c>, sql = <c>2026-04-03 14:00:00+00</c>. A
        /// date-only value renders as YYYY-MM-DD; a blank as "".</summary>
        public static V DatetimeText(V v)
        {
            if (Blank(v)) return S("");
            if (v.K == VK.Str && DateOnlyText.IsMatch(v.S.Trim())) return S(v.S.Trim());
            if (v.K != VK.Time && v.K != VK.Str) throw new InvalidOperationException($"cannot render {PyRepr(v)} as a datetime");
            var t = InZone(v);
            var iso = Param("erbDateTimeText") == "iso8601";
            var s = t.ToString(iso ? "yyyy-MM-dd'T'HH:mm:ss" : "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            var micros = t.Ticks % TimeSpan.TicksPerSecond / 10;
            if (micros != 0) s += ("." + micros.ToString("D6", CultureInfo.InvariantCulture)).TrimEnd('0');
            var off = t.Offset;
            var sign = off < TimeSpan.Zero ? "-" : "+";
            var hh = Math.Abs(off.Hours).ToString("D2", CultureInfo.InvariantCulture);
            var mm = Math.Abs(off.Minutes).ToString("D2", CultureInfo.InvariantCulture);
            if (iso) s += sign + hh + ":" + mm;
            else
            {
                s += sign + hh;
                if (off.Minutes != 0) s += ":" + mm;
            }
            return S(s);
        }

        /// <summary>A computed <see cref="DateTimeOffset"/> as it is written into JSON: per
        /// erbDateTimeText, as the reference implementation's erb_json_value writes it.</summary>
        public static string DatetimeJson(DateTimeOffset t) => DatetimeText(Zoned(t)).S;

        /// <summary>A non-datetime CONCAT / <c>&amp;</c> operand as text: a blank contributes
        /// nothing, a number renders in its shortest exact form with no trailing .0, a boolean as
        /// true/false, a datetime per erbDateTimeText.</summary>
        public static V Text(V v)
        {
            switch (v.K)
            {
                case VK.Null: return S("");
                case VK.Bool: return S(v.B ? "true" : "false");
                case VK.Num:
                    if (v.IsInt || (v.N == Math.Truncate(v.N) && Math.Abs(v.N) < 1e15))
                        return S(((long)v.N).ToString(CultureInfo.InvariantCulture));
                    return S(PyFloat(v.N));
                case VK.Time: return DatetimeText(v);
            }
            return S(v.S);
        }

        // ───────────────────────────── snapshot memoization ─────────────────────────────

        /// <summary>
        /// The computed values of a frozen context. Enabled by
        /// <c>SoAEFContext.FreezeComputedValues = true</c>, which declares the tracked rows a
        /// read-only snapshot: every computed property is then evaluated once per row, and every
        /// row set once per table. Unfreezing — or freezing again — discards it. Without a frozen
        /// context nothing is cached and every read recomputes from the current rows.
        /// </summary>
        public sealed class Snapshot
        {
            internal readonly Dictionary<object, Dictionary<string, V>> Values = new(ReferenceEqualityComparer.Instance);
            internal readonly HashSet<(object, string)> InProgress = new();
            internal readonly Dictionary<string, object> Rows = new();
            internal readonly Dictionary<string, object> Indexes = new();
        }

        public static V Memo(SoAEntityBase entity, string field, Func<V> compute)
        {
            var snapshot = entity.SoAContext?.ComputedSnapshot;
            if (snapshot == null) return compute();
            if (!snapshot.Values.TryGetValue(entity, out var fields))
                snapshot.Values[entity] = fields = new Dictionary<string, V>();
            if (fields.TryGetValue(field, out var cached)) return cached;
            if (!snapshot.InProgress.Add((entity, field)))
                throw new InvalidOperationException($"{entity.GetType().Name}.{field} depends on itself");
            try
            {
                var value = compute();
                fields[field] = value;
                return value;
            }
            finally
            {
                snapshot.InProgress.Remove((entity, field));
            }
        }

        public static List<T> Rows<T>(SoAEFContext context, string table, Func<SoAEFContext, DbSet<T>> set) where T : class
        {
            var snapshot = context.ComputedSnapshot;
            if (snapshot != null && snapshot.Rows.TryGetValue(table, out var cached)) return (List<T>)cached;
            var dbSet = set(context);
            var rows = dbSet.Local.Count > 0 ? dbSet.Local.ToList() : dbSet.ToList();
            if (snapshot != null) snapshot.Rows[table] = rows;
            return rows;
        }

        // ───────────────────────────── lookups ─────────────────────────────

        static string LookupKey(V v) => v.K switch
        {
            VK.Str => "s:" + v.S,
            VK.Num => "n:" + v.N.ToString("R", CultureInfo.InvariantCulture),
            VK.Bool => v.B ? "n:1" : "n:0",
            VK.Time => "t:" + v.T.UtcTicks.ToString(CultureInfo.InvariantCulture),
            _ => ""
        };

        /// <summary>
        /// INDEX(Target!Return, MATCH(key, Target!Match, 0)). No match — or no context to look
        /// in — renders <paramref name="blank"/>: the target field evaluated on an all-NULL row,
        /// as a LEFT JOIN evaluates a calculated column (literal text in its formula survives).
        /// </summary>
        public static V Lookup<T>(SoAEntityBase entity, string table, string matchColumn, Func<SoAEFContext, DbSet<T>> set,
                                  Func<T, V> match, V key, Func<T, V> ret, Func<V> blank) where T : class
        {
            var context = entity.SoAContext;
            if (context == null || key.K == VK.Null) return blank();
            var indexName = table + "." + matchColumn;
            Dictionary<string, T> index = null;
            var snapshot = context.ComputedSnapshot;
            if (snapshot != null && snapshot.Indexes.TryGetValue(indexName, out var cached)) index = (Dictionary<string, T>)cached;
            if (index == null)
            {
                index = new Dictionary<string, T>();
                foreach (var row in Rows(context, table, set))
                {
                    var pk = match(row);
                    if (pk.K != VK.Null) index[LookupKey(pk)] = row;
                }
                if (snapshot != null) snapshot.Indexes[indexName] = index;
            }
            return index.TryGetValue(LookupKey(key), out var found) ? ret(found) : blank();
        }

        // ───────────────────────────── aggregations ─────────────────────────────

        static V BlankAsEmpty(V v) => v.K == VK.Null ? S("") : v;

        /// <summary>A criterion that names a field of the record being computed.</summary>
        public static bool CritField(V cell, V recordValue) => PyEqual(BlankAsEmpty(cell), BlankAsEmpty(recordValue));

        public static bool CritLiteral(V cell, V literal) => PyEqual(cell, literal);

        public static bool CritOp(V cell, string op, V target)
        {
            if (Blank(cell)) return false;
            double c;
            if (cell.K == VK.Bool) c = cell.B ? 1 : 0;
            else if (ToNumber(cell, out var n)) c = n.N;
            else return false;
            if (!ToNumber(target, out var t)) return false;
            return op switch
            {
                ">" => c > t.N,
                ">=" => c >= t.N,
                "<" => c < t.N,
                "<=" => c <= t.N,
                "<>" => c != t.N,
                _ => c == t.N
            };
        }

        public static V CountIfs<T>(List<T> rows, Func<T, bool> matches) => I(rows.Count(matches));

        public static V SumIfs<T>(List<T> rows, Func<T, bool> matches, Func<T, V> value, Func<V, V> suffix)
        {
            var numbers = Numbers(rows, matches, value, suffix);
            return numbers.Count == 0 ? I(0) : D(numbers.Sum());
        }

        public static V AverageIfs<T>(List<T> rows, Func<T, bool> matches, Func<T, V> value)
        {
            var numbers = Numbers(rows, matches, value, null);
            return numbers.Count == 0 ? Null : D(numbers.Sum() / numbers.Count);
        }

        static List<double> Numbers<T>(List<T> rows, Func<T, bool> matches, Func<T, V> value, Func<V, V> suffix)
        {
            var numbers = new List<double>();
            foreach (var row in rows.Where(matches))
            {
                var raw = value(row);
                if (Blank(raw)) continue;
                V number;
                if (raw.K == VK.Bool) number = D(raw.B ? 1 : 0);
                else if (ToNumber(raw, out var n)) number = D(n.N);
                else continue;
                if (suffix != null) number = suffix(number);
                numbers.Add(number.N);
            }
            return numbers;
        }

        public static V ExtremeIfs<T>(bool max, List<T> rows, Func<T, bool> matches, Func<T, V> value)
        {
            var values = rows.Where(matches).Select(value).Where(v => !Blank(v)).ToList();
            if (values.Count == 0) return Null;
            var best = values[0];
            foreach (var v in values.Skip(1))
                if (max ? Less(best, v) : Less(v, best)) best = v;
            return best;
        }

        static bool Less(V a, V b)
        {
            if (NumericLike(a, out var an) && NumericLike(b, out var bn)) return an < bn;
            if (a.K == VK.Str && b.K == VK.Str) return string.CompareOrdinal(a.S, b.S) < 0;
            if (a.K == VK.Time && b.K == VK.Time) return a.T < b.T;
            throw new InvalidOperationException($"MIN/MAX over values that cannot be ordered: {PyRepr(a)} and {PyRepr(b)}");
        }

        // ───────────────────────────── transitive closure ─────────────────────────────

        /// <summary>One row of vw_&lt;edge&gt;_closure: a reachable pair, the shortest derivation's
        /// length, and whether no directly-asserted edge states the pair.</summary>
        public sealed class ClosureRow
        {
            public string FromId { get; init; } = "";
            public string ToId { get; init; } = "";
            public int? HopDistance { get; init; }
            public bool? IsInferred { get; init; }
        }

        /// <summary>
        /// Materializes a closure view over an edge table's rows: every (from, to) pair reachable
        /// in one or more edges, computed natively here (reading another substrate's closure would
        /// make this substrate's score a measure of that one). A node on a cycle reaches itself.
        /// An empty endpoint is not an edge. <paramref name="include"/> is the closure's edge
        /// filter; an unfiltered closure passes every row.
        /// </summary>
        public static List<ClosureRow> Closure<T>(SoAEFContext context, string view, string table,
                                                 Func<SoAEFContext, DbSet<T>> set, Func<T, V> from, Func<T, V> to,
                                                 Func<T, bool> include) where T : class
        {
            var snapshot = context.ComputedSnapshot;
            if (snapshot != null && snapshot.Rows.TryGetValue(view, out var cached)) return (List<ClosureRow>)cached;

            var asserted = new HashSet<(string, string)>();
            var adjacency = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            var origins = new List<string>();
            foreach (var row in Rows(context, table, set))
            {
                if (!include(row)) continue;
                var f = from(row);
                var t = to(row);
                if (Blank(f) || Blank(t)) continue;
                var src = PyStr(f);
                var dst = PyStr(t);
                asserted.Add((src, dst));
                if (!adjacency.TryGetValue(src, out var outs))
                {
                    adjacency[src] = outs = new List<string>();
                    origins.Add(src);
                }
                outs.Add(dst);
            }

            var shortest = new Dictionary<(string, string), int>();
            foreach (var origin in origins)
            {
                // Breadth-first, so the first arrival at a node is its shortest derivation.
                // Interior nodes are never revisited, so a cyclic edge set terminates; arriving
                // back at the origin records (origin, origin).
                var frontier = new List<(string Node, HashSet<string> Path)>
                {
                    (origin, new HashSet<string>(StringComparer.Ordinal) { origin })
                };
                for (int hop = 1; frontier.Count > 0; hop++)
                {
                    var next = new List<(string Node, HashSet<string> Path)>();
                    foreach (var (node, path) in frontier)
                    {
                        if (!adjacency.TryGetValue(node, out var neighbors)) continue;
                        foreach (var neighbor in neighbors)
                        {
                            shortest.TryAdd((origin, neighbor), hop);
                            if (path.Contains(neighbor)) continue;
                            next.Add((neighbor, new HashSet<string>(path, StringComparer.Ordinal) { neighbor }));
                        }
                    }
                    frontier = next;
                }
            }

            var rows = shortest
                .Select(kv => new ClosureRow
                {
                    FromId = kv.Key.Item1,
                    ToId = kv.Key.Item2,
                    HopDistance = kv.Value,
                    IsInferred = !asserted.Contains(kv.Key),
                })
                .ToList();
            if (snapshot != null) snapshot.Rows[view] = rows;
            return rows;
        }
    }
}
