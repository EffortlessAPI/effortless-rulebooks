
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
    [Table("ProcessMiningRuns")]
    public class ProcessMiningRunBase : SoAEntityBase
    {
        [Key]
        public string ProcessMiningRunId { get; set; }

        // Formula Name (rulebook: ={{EventLogSource}} & " / " & {{MinedAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.EventLogSource)), F.S(" / "), F.DatetimeText(F.Of(this.MinedAt))))); set { }
        }

        public string? EventLogSource { get; set; }
        public DateTimeOffset? MinedAt { get; set; }
        public int? DiscoveredVariantCount { get; set; }
        public int? ConformingVariantCount { get; set; }
        public string? DeviationDescription { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula ConformanceRate (rulebook: =IF({{DiscoveredVariantCount}} = 0, 0, {{ConformingVariantCount}} / {{DiscoveredVariantCount}}))
        [NotMapped]
        public decimal? ConformanceRate
        {
            get => F.AsDecimal(F.Memo(this, "ConformanceRate", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.DiscoveredVariantCount)), F.I(0)))) ? F.I(0) : F.Div(F.Of(this.ConformingVariantCount), F.Of(this.DiscoveredVariantCount))))); set { }
        }

        // Formula IsConformant (rulebook: ={{ConformanceRate}} >= 0.8)
        [NotMapped]
        public bool? IsConformant
        {
            get => F.AsBool(F.Memo(this, "IsConformant", () => F.Cmp(F.Of(this.ConformanceRate), ">=", F.D(0.8)))); set { }
        }

        // Formula HasMajorDriftFromDocumentation (rulebook: ={{ConformanceRate}} < 0.5)
        [NotMapped]
        public bool? HasMajorDriftFromDocumentation
        {
            get => F.AsBool(F.Memo(this, "HasMajorDriftFromDocumentation", () => F.Cmp(F.Of(this.ConformanceRate), "<", F.D(0.5)))); set { }
        }

        // Formula DaysSinceMined (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{MinedAt}}, "days"))
        [NotMapped]
        public int? DaysSinceMined
        {
            get => F.AsInt(F.Memo(this, "DaysSinceMined", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.MinedAt), F.S("days"))))); set { }
        }

        // Formula IsStaleMiningEvidence (rulebook: ={{DaysSinceMined}} > 180)
        [NotMapped]
        public bool? IsStaleMiningEvidence
        {
            get => F.AsBool(F.Memo(this, "IsStaleMiningEvidence", () => F.Cmp(F.Of(this.DaysSinceMined), ">", F.I(180)))); set { }
        }

        // Formula ProcedureVersionIsLive (rulebook: =INDEX(ProcedureVersions!{{IsLive}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public bool? ProcedureVersionIsLive
        {
            get => F.AsBool(F.Memo(this, "ProcedureVersionIsLive", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.IsLive), () => F.Of(new ProcedureVersion().IsLive)))); set { }
        }

        // Formula IsDriftOnLiveVersion (rulebook: =AND({{HasMajorDriftFromDocumentation}}, {{ProcedureVersionIsLive}}))
        [NotMapped]
        public bool? IsDriftOnLiveVersion
        {
            get => F.AsBool(F.Memo(this, "IsDriftOnLiveVersion", () => F.And(F.Bool3(F.Of(this.HasMajorDriftFromDocumentation)), F.Bool3(F.Of(this.ProcedureVersionIsLive))))); set { }
        }

        // Formula DriftedMiningRunKey (rulebook: =IF({{IsDriftOnLiveVersion}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? DriftedMiningRunKey
        {
            get => F.AsString(F.Memo(this, "DriftedMiningRunKey", () => (F.Truthy(F.Bool3(F.Of(this.IsDriftOnLiveVersion))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? EvaluationContext { get; set; }

        private ProcedureVersion _procedureVersionRef;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersionRef
        {
            get
            {
                if (_procedureVersionRef == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionRef - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersionRef = base.SoAContext.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersionRef != null)
                    {
                        base.SoAContext.Attach(_procedureVersionRef);
                    }
                }
                return _procedureVersionRef;
            }
            set
            {
                if (_procedureVersionRef != value)
                {
                    _procedureVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersionRef != null)
                    {
                        ProcedureVersion = _procedureVersionRef.ProcedureVersionId;
                    }
                }
            }
        }

        private EvaluationContext _evaluationContextRef;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContextRef
        {
            get
            {
                if (_evaluationContextRef == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContextRef - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContextRef = base.SoAContext.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContextRef != null)
                    {
                        base.SoAContext.Attach(_evaluationContextRef);
                    }
                }
                return _evaluationContextRef;
            }
            set
            {
                if (_evaluationContextRef != value)
                {
                    _evaluationContextRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_evaluationContextRef != null)
                    {
                        EvaluationContext = _evaluationContextRef.EvaluationContextId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
