
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
    [Table("GovernanceStageControls")]
    public class GovernanceStageControlBase : SoAEntityBase
    {
        [Key]
        public string GovernanceStageControlId { get; set; }

        // Formula Name (rulebook: ={{GovernedModel}} & " " & {{Stage}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.GovernedModel)), F.S(" "), F.Text(F.Of(this.Stage))))); set { }
        }

        public string? Stage { get; set; }
        public string? Control { get; set; }
        public bool? IsContinuous { get; set; }
        // Formula IsContinuousPipelineStage (rulebook: =AND(OR({{Stage}} = "Collection", {{Stage}} = "Organization", {{Stage}} = "Encoding"), {{IsContinuous}}))
        [NotMapped]
        public bool? IsContinuousPipelineStage
        {
            get => F.AsBool(F.Memo(this, "IsContinuousPipelineStage", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Stage)), F.S("Collection"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Stage)), F.S("Organization"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Stage)), F.S("Encoding"))))), F.IsTrueV(F.Of(this.IsContinuous))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }

        private GovernedModel _governedModelRef;

        [ForeignKey("GovernedModel")]
        public virtual GovernedModel GovernedModelRef
        {
            get
            {
                if (_governedModelRef == null && !string.IsNullOrEmpty(GovernedModel))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModelRef - no database context is set. GovernedModel: " + GovernedModel + ".");
                        }
                        return null;
                    }
                    _governedModelRef = base.SoAContext.GovernedModels.Find(GovernedModel);
                    if (_governedModelRef != null)
                    {
                        base.SoAContext.Attach(_governedModelRef);
                    }
                }
                return _governedModelRef;
            }
            set
            {
                if (_governedModelRef != value)
                {
                    _governedModelRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_governedModelRef != null)
                    {
                        GovernedModel = _governedModelRef.GovernedModelId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
