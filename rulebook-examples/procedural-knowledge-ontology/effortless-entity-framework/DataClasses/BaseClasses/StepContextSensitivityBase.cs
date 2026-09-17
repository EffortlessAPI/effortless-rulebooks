
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
    [Table("StepContextSensitivities")]
    public class StepContextSensitivityBase : SoAEntityBase
    {
        [Key]
        public string StepContextSensitivityId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{ContextFactor}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(" / "), F.Text(F.Of(this.ContextFactor))))); set { }
        }

        public string? ContextFactor { get; set; }
        public string? Description { get; set; }
        public string? EffectOnSignificance { get; set; }
        // Formula UnscopedStepKey (rulebook: =IF({{ApplicabilityScope}} = "", {{Step}}, ""))
        [NotMapped]
        public string? UnscopedStepKey
        {
            get => F.AsString(F.Memo(this, "UnscopedStepKey", () => (F.Truthy(F.Bool3(F.IsBlank(F.Of(this.ApplicabilityScope)))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Step { get; set; }
        public string? ApplicabilityScope { get; set; }

        private Step _stepRef;

        [ForeignKey("Step")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(Step))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(Step);
                    if (_stepRef != null)
                    {
                        base.SoAContext.Attach(_stepRef);
                    }
                }
                return _stepRef;
            }
            set
            {
                if (_stepRef != value)
                {
                    _stepRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepRef != null)
                    {
                        Step = _stepRef.StepId;
                    }
                }
            }
        }

        private ApplicabilityScope _applicabilityScopeRef;

        [ForeignKey("ApplicabilityScope")]
        public virtual ApplicabilityScope ApplicabilityScopeRef
        {
            get
            {
                if (_applicabilityScopeRef == null && !string.IsNullOrEmpty(ApplicabilityScope))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApplicabilityScopeRef - no database context is set. ApplicabilityScope: " + ApplicabilityScope + ".");
                        }
                        return null;
                    }
                    _applicabilityScopeRef = base.SoAContext.ApplicabilityScopes.Find(ApplicabilityScope);
                    if (_applicabilityScopeRef != null)
                    {
                        base.SoAContext.Attach(_applicabilityScopeRef);
                    }
                }
                return _applicabilityScopeRef;
            }
            set
            {
                if (_applicabilityScopeRef != value)
                {
                    _applicabilityScopeRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_applicabilityScopeRef != null)
                    {
                        ApplicabilityScope = _applicabilityScopeRef.ApplicabilityScopeId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.ApplicabilityScopeRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
