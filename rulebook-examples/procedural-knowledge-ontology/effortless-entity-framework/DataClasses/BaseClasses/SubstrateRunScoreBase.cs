
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;
using F = SqlOnAir.DotNet.Lib.DataClasses.Formulas.EfFormulaFns;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("SubstrateRunScores")]
    public class SubstrateRunScoreBase : SoAEntityBase
    {
        [Key]
        public string SubstrateRunScoreId { get; set; }

        // Formula Name (rulebook: =CONCAT({{Run}}, " / ", {{Substrate}}))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Run)), F.S(" / "), F.Text(F.Of(this.Substrate))))); set { }
        }

        public string? HarnessError { get; set; }
        public decimal? DurationSeconds { get; set; }
        public decimal? CellsTested { get; set; }
        public decimal? CellsPassed { get; set; }
        public decimal? CalculatedTested { get; set; }
        public decimal? CalculatedPassed { get; set; }
        public decimal? LookupTested { get; set; }
        public decimal? LookupPassed { get; set; }
        public decimal? AggregationTested { get; set; }
        public decimal? AggregationPassed { get; set; }
        // Formula CellsFailed (rulebook: ={{CellsTested}} - {{CellsPassed}})
        [NotMapped]
        public decimal? CellsFailed
        {
            get => F.AsDecimal(F.Memo(this, "CellsFailed", () => F.Sub(F.Of(this.CellsTested), F.Of(this.CellsPassed)))); set { }
        }

        // Formula Score (rulebook: =IF({{CellsTested}} = 0, 0, ROUND(100 * {{CellsPassed}} / {{CellsTested}}, 2)))
        [NotMapped]
        public decimal? Score
        {
            get => F.AsDecimal(F.Memo(this, "Score", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.CellsTested)), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.CellsPassed)), F.Of(this.CellsTested)), F.I(2))))); set { }
        }

        // Formula CalculatedScore (rulebook: =IF({{CalculatedTested}} = 0, 0, ROUND(100 * {{CalculatedPassed}} / {{CalculatedTested}}, 2)))
        [NotMapped]
        public decimal? CalculatedScore
        {
            get => F.AsDecimal(F.Memo(this, "CalculatedScore", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.CalculatedTested)), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.CalculatedPassed)), F.Of(this.CalculatedTested)), F.I(2))))); set { }
        }

        // Formula LookupScore (rulebook: =IF({{LookupTested}} = 0, 0, ROUND(100 * {{LookupPassed}} / {{LookupTested}}, 2)))
        [NotMapped]
        public decimal? LookupScore
        {
            get => F.AsDecimal(F.Memo(this, "LookupScore", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.LookupTested)), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.LookupPassed)), F.Of(this.LookupTested)), F.I(2))))); set { }
        }

        // Formula AggregationScore (rulebook: =IF({{AggregationTested}} = 0, 0, ROUND(100 * {{AggregationPassed}} / {{AggregationTested}}, 2)))
        [NotMapped]
        public decimal? AggregationScore
        {
            get => F.AsDecimal(F.Memo(this, "AggregationScore", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.AggregationTested)), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.AggregationPassed)), F.Of(this.AggregationTested)), F.I(2))))); set { }
        }

        // Formula IsPerfect (rulebook: =AND({{HarnessError}} = "", {{CellsTested}} > 0, {{CellsFailed}} = 0))
        [NotMapped]
        public bool? IsPerfect
        {
            get => F.AsBool(F.Memo(this, "IsPerfect", () => F.And(F.Bool3(F.IsBlank(F.Of(this.HarnessError))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.CellsTested)), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.CellsFailed), F.I(0)))))); set { }
        }

        // Formula PerfectRunKey (rulebook: =IF({{IsPerfect}}, {{Run}}, ""))
        [NotMapped]
        public string? PerfectRunKey
        {
            get => F.AsString(F.Memo(this, "PerfectRunKey", () => (F.Truthy(F.Bool3(F.Of(this.IsPerfect))) ? F.Of(this.Run) : F.S("")))); set { }
        }

        // Formula IsInLatestRun (rulebook: =INDEX(ConformanceRuns!{{IsLatest}}, MATCH({{Run}}, ConformanceRuns!{{ConformanceRunId}}, 0)))
        [NotMapped]
        public bool? IsInLatestRun
        {
            get => F.AsBool(F.Memo(this, "IsInLatestRun", () => F.Lookup<ConformanceRun>(this, "ConformanceRuns", "ConformanceRunId", __c => __c.ConformanceRuns, __r => F.Of(__r.ConformanceRunId), F.Of(this.Run), __r => F.Of(__r.IsLatest), () => F.Of(new ConformanceRun().IsLatest)))); set { }
        }

        // Formula LatestCellsTested (rulebook: =IF({{IsInLatestRun}}, {{CellsTested}}, 0))
        [NotMapped]
        public decimal? LatestCellsTested
        {
            get => F.AsDecimal(F.Memo(this, "LatestCellsTested", () => (F.Truthy(F.Bool3(F.Of(this.IsInLatestRun))) ? F.Of(this.CellsTested) : F.I(0)))); set { }
        }

        // Formula LatestCellsPassed (rulebook: =IF({{IsInLatestRun}}, {{CellsPassed}}, 0))
        [NotMapped]
        public decimal? LatestCellsPassed
        {
            get => F.AsDecimal(F.Memo(this, "LatestCellsPassed", () => (F.Truthy(F.Bool3(F.Of(this.IsInLatestRun))) ? F.Of(this.CellsPassed) : F.I(0)))); set { }
        }

        // Formula LatestErrorFlag (rulebook: =IF(AND({{IsInLatestRun}}, {{HarnessError}} <> ""), 1, 0))
        [NotMapped]
        public decimal? LatestErrorFlag
        {
            get => F.AsDecimal(F.Memo(this, "LatestErrorFlag", () => (F.Truthy(F.Bool3(F.And(F.Bool3(F.Of(this.IsInLatestRun)), F.Bool3(F.IsNotBlank(F.Of(this.HarnessError)))))) ? F.I(1) : F.I(0)))); set { }
        }

        // Formula SubstrateLabel (rulebook: =INDEX(ConformanceSubstrates!{{Label}}, MATCH({{Substrate}}, ConformanceSubstrates!{{ConformanceSubstrateId}}, 0)))
        [NotMapped]
        public string? SubstrateLabel
        {
            get => F.AsString(F.Memo(this, "SubstrateLabel", () => F.Lookup<ConformanceSubstrate>(this, "ConformanceSubstrates", "ConformanceSubstrateId", __c => __c.ConformanceSubstrates, __r => F.Of(__r.ConformanceSubstrateId), F.Of(this.Substrate), __r => F.Of(__r.Label), () => F.Of(new ConformanceSubstrate().Label)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Run { get; set; }
        public string? Substrate { get; set; }

        private ConformanceRun _conformanceRun;

        [ForeignKey("Run")]
        public virtual ConformanceRun ConformanceRun
        {
            get
            {
                if (_conformanceRun == null && !string.IsNullOrEmpty(Run))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConformanceRun - no database context is set. Run: " + Run + ".");
                        }
                        return null;
                    }
                    _conformanceRun = base.SoAContext.ConformanceRuns.Find(Run);
                    if (_conformanceRun != null)
                    {
                        base.SoAContext.Attach(_conformanceRun);
                    }
                }
                return _conformanceRun;
            }
            set
            {
                if (_conformanceRun != value)
                {
                    _conformanceRun = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_conformanceRun != null)
                    {
                        Run = _conformanceRun.ConformanceRunId;
                    }
                }
            }
        }

        private ConformanceSubstrate _conformanceSubstrate;

        [ForeignKey("Substrate")]
        public virtual ConformanceSubstrate ConformanceSubstrate
        {
            get
            {
                if (_conformanceSubstrate == null && !string.IsNullOrEmpty(Substrate))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConformanceSubstrate - no database context is set. Substrate: " + Substrate + ".");
                        }
                        return null;
                    }
                    _conformanceSubstrate = base.SoAContext.ConformanceSubstrates.Find(Substrate);
                    if (_conformanceSubstrate != null)
                    {
                        base.SoAContext.Attach(_conformanceSubstrate);
                    }
                }
                return _conformanceSubstrate;
            }
            set
            {
                if (_conformanceSubstrate != value)
                {
                    _conformanceSubstrate = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_conformanceSubstrate != null)
                    {
                        Substrate = _conformanceSubstrate.ConformanceSubstrateId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ConformanceRun;
            _ = this.ConformanceSubstrate;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
