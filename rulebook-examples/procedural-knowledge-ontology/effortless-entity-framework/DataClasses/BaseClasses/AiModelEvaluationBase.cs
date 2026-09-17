
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
    [Table("AiModelEvaluations")]
    public class AiModelEvaluationBase : SoAEntityBase
    {
        [Key]
        public string AiModelEvaluationId { get; set; }

        // Formula Name (rulebook: ={{ModelVersion}} & " " & {{Metric}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ModelVersion)), F.S(" "), F.Text(F.Of(this.Metric))))); set { }
        }

        public DateTimeOffset? EvaluatedAt { get; set; }
        public string? EvaluationSuite { get; set; }
        public string? Metric { get; set; }
        public decimal? Score { get; set; }
        public decimal? PassThreshold { get; set; }
        // Formula Passed (rulebook: ={{Score}} >= {{PassThreshold}})
        [NotMapped]
        public bool? Passed
        {
            get => F.AsBool(F.Memo(this, "Passed", () => F.Cmp(F.Nullif(F.Of(this.Score)), ">=", F.Nullif(F.Of(this.PassThreshold))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ModelVersion { get; set; }

        private AiRegistryModelVersion _aiRegistryModelVersion;

        [ForeignKey("ModelVersion")]
        public virtual AiRegistryModelVersion AiRegistryModelVersion
        {
            get
            {
                if (_aiRegistryModelVersion == null && !string.IsNullOrEmpty(ModelVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiRegistryModelVersion - no database context is set. ModelVersion: " + ModelVersion + ".");
                        }
                        return null;
                    }
                    _aiRegistryModelVersion = base.SoAContext.AiRegistryModelVersions.Find(ModelVersion);
                    if (_aiRegistryModelVersion != null)
                    {
                        base.SoAContext.Attach(_aiRegistryModelVersion);
                    }
                }
                return _aiRegistryModelVersion;
            }
            set
            {
                if (_aiRegistryModelVersion != value)
                {
                    _aiRegistryModelVersion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_aiRegistryModelVersion != null)
                    {
                        ModelVersion = _aiRegistryModelVersion.AiRegistryModelVersionId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AiRegistryModelVersion;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
