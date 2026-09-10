
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ProcessMiningRuns")]
    public class ProcessMiningRunBase : SoAEntityBase
    {
        [Key]
        public string ProcessMiningRunId { get; set; }

        // Formula Name (rulebook: ={{EventLogSource}} & " / " & {{MinedAt}})
        public string? Name
        {
            get => this.EventLogSource + " / " + this.MinedAt; set { }
        }

        public string? EventLogSource { get; set; }
        public DateTime? MinedAt { get; set; }
        public int? DiscoveredVariantCount { get; set; }
        public int? ConformingVariantCount { get; set; }
        public string? DeviationDescription { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula ConformanceRate (rulebook: =IF({{DiscoveredVariantCount}} = 0, 0, {{ConformingVariantCount}} / {{DiscoveredVariantCount}}))
        public decimal? ConformanceRate
        {
            get => IF(this.DiscoveredVariantCount = 0, 0, this.ConformingVariantCount / this.DiscoveredVariantCount); set { }
        }

        // Formula IsConformant (rulebook: ={{ConformanceRate}} >= 0.8)
        public bool? IsConformant
        {
            get => this.ConformanceRate >= 0.8; set { }
        }

        // Formula HasMajorDriftFromDocumentation (rulebook: ={{ConformanceRate}} < 0.5)
        public bool? HasMajorDriftFromDocumentation
        {
            get => this.ConformanceRate < 0.5; set { }
        }

        // Formula DaysSinceMined (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{MinedAt}}, "days"))
        public int? DaysSinceMined
        {
            get => DATETIME_DIFF(this.AsOfInstant, this.MinedAt, "days"); set { }
        }

        // Formula IsStaleMiningEvidence (rulebook: ={{DaysSinceMined}} > 180)
        public bool? IsStaleMiningEvidence
        {
            get => this.DaysSinceMined > 180; set { }
        }

        // Formula ProcedureVersionIsLive (rulebook: =INDEX(ProcedureVersions!{{IsLive}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        public bool? ProcedureVersionIsLive
        {
            get => INDEX(ProcedureVersions!this.IsLive, MATCH(this.ProcedureVersion, ProcedureVersions!this.ProcedureVersionId, 0)); set { }
        }

        // Formula IsDriftOnLiveVersion (rulebook: =AND({{HasMajorDriftFromDocumentation}}, {{ProcedureVersionIsLive}}))
        public bool? IsDriftOnLiveVersion
        {
            get => AND(this.HasMajorDriftFromDocumentation, this.ProcedureVersionIsLive); set { }
        }

        // Formula DriftedMiningRunKey (rulebook: =IF({{IsDriftOnLiveVersion}}, {{ProcedureVersion}}, ""))
        public string? DriftedMiningRunKey
        {
            get => IF(this.IsDriftOnLiveVersion, this.ProcedureVersion, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? EvaluationContext { get; set; }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = Context.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        Context.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    ProcedureVersion = _procedureVersion == null ? default : _procedureVersion.ProcedureVersionId;
                }
            }
        }

        private EvaluationContext _evaluationContext;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContext
        {
            get
            {
                if (_evaluationContext == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContext - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContext = Context.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContext != null)
                    {
                        Context.Attach(_evaluationContext);
                    }
                }
                return _evaluationContext;
            }
            set
            {
                if (_evaluationContext != value)
                {
                    _evaluationContext = value;
                    EvaluationContext = _evaluationContext == null ? default : _evaluationContext.EvaluationContextId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.EvaluationContext;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
