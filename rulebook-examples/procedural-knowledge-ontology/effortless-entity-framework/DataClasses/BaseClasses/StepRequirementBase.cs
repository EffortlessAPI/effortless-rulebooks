
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
    [Table("StepRequirements")]
    public class StepRequirementBase : SoAEntityBase
    {
        [Key]
        public string StepRequirementId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{Requirement}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(" / "), F.Text(F.Of(this.Requirement))))); set { }
        }

        // Formula RequirementIsBlocking (rulebook: =INDEX(Requirements!{{IsBlocking}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        [NotMapped]
        public bool? RequirementIsBlocking
        {
            get => F.AsBool(F.Memo(this, "RequirementIsBlocking", () => F.Lookup<Requirement>(this, "Requirements", "RequirementId", __c => __c.Requirements, __r => F.Of(__r.RequirementId), F.Of(this.Requirement), __r => F.Of(__r.IsBlocking), () => F.Of(new Requirement().IsBlocking)))); set { }
        }

        // Formula BlockingStepKey (rulebook: =IF({{RequirementIsBlocking}}, {{Step}}, ""))
        [NotMapped]
        public string? BlockingStepKey
        {
            get => F.AsString(F.Memo(this, "BlockingStepKey", () => (F.Truthy(F.Bool3(F.Of(this.RequirementIsBlocking))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula StepWhenBlocking (rulebook: =IF({{RequirementIsBlocking}}, {{Step}}, ""))
        [NotMapped]
        public string? StepWhenBlocking
        {
            get => F.AsString(F.Memo(this, "StepWhenBlocking", () => (F.Truthy(F.Bool3(F.Of(this.RequirementIsBlocking))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula RequirementLacksWitness (rulebook: =INDEX(Requirements!{{IsUnwitnessedBlockingControl}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        [NotMapped]
        public bool? RequirementLacksWitness
        {
            get => F.AsBool(F.Memo(this, "RequirementLacksWitness", () => F.Lookup<Requirement>(this, "Requirements", "RequirementId", __c => __c.Requirements, __r => F.Of(__r.RequirementId), F.Of(this.Requirement), __r => F.Of(__r.IsUnwitnessedBlockingControl), () => F.Of(new Requirement().IsUnwitnessedBlockingControl)))); set { }
        }

        // Formula UnwitnessedStepKey (rulebook: =IF({{RequirementLacksWitness}}, {{Step}}, ""))
        [NotMapped]
        public string? UnwitnessedStepKey
        {
            get => F.AsString(F.Memo(this, "UnwitnessedStepKey", () => (F.Truthy(F.Bool3(F.Of(this.RequirementLacksWitness))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula SatisfactionCountForBinding (rulebook: =COUNTIFS(RequirementSatisfactions!{{BindingKey}}, {{StepRequirementId}}))
        [NotMapped]
        public decimal? SatisfactionCountForBinding
        {
            get => F.AsDecimal(F.Memo(this, "SatisfactionCountForBinding", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.BindingKey), F.Of(this.StepRequirementId)))))); set { }
        }

        // Formula BindingWasEverExercised (rulebook: ={{SatisfactionCountForBinding}} > 0)
        [NotMapped]
        public bool? BindingWasEverExercised
        {
            get => F.AsBool(F.Memo(this, "BindingWasEverExercised", () => F.Cmp(F.Of(this.SatisfactionCountForBinding), ">", F.I(0)))); set { }
        }

        // Formula IsUnexercisedBlockingBinding (rulebook: =AND({{RequirementIsBlocking}}, NOT({{BindingWasEverExercised}})))
        [NotMapped]
        public bool? IsUnexercisedBlockingBinding
        {
            get => F.AsBool(F.Memo(this, "IsUnexercisedBlockingBinding", () => F.And(F.Bool3(F.Of(this.RequirementIsBlocking)), F.Bool3(F.Not(F.Bool3(F.Of(this.BindingWasEverExercised))))))); set { }
        }

        // Formula UnexercisedBindingRequirementKey (rulebook: =IF({{IsUnexercisedBlockingBinding}}, {{Requirement}}, ""))
        [NotMapped]
        public string? UnexercisedBindingRequirementKey
        {
            get => F.AsString(F.Memo(this, "UnexercisedBindingRequirementKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnexercisedBlockingBinding))) ? F.Of(this.Requirement) : F.S("")))); set { }
        }

        // Formula RequirementIsRegulatory (rulebook: =INDEX(Requirements!{{IsRegulatoryRequirement}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        [NotMapped]
        public bool? RequirementIsRegulatory
        {
            get => F.AsBool(F.Memo(this, "RequirementIsRegulatory", () => F.Lookup<Requirement>(this, "Requirements", "RequirementId", __c => __c.Requirements, __r => F.Of(__r.RequirementId), F.Of(this.Requirement), __r => F.Of(__r.IsRegulatoryRequirement), () => F.Of(new Requirement().IsRegulatoryRequirement)))); set { }
        }


        public string? Step { get; set; }
        public string? Requirement { get; set; }

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

        private Requirement _requirementRef;

        [ForeignKey("Requirement")]
        public virtual Requirement RequirementRef
        {
            get
            {
                if (_requirementRef == null && !string.IsNullOrEmpty(Requirement))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequirementRef - no database context is set. Requirement: " + Requirement + ".");
                        }
                        return null;
                    }
                    _requirementRef = base.SoAContext.Requirements.Find(Requirement);
                    if (_requirementRef != null)
                    {
                        base.SoAContext.Attach(_requirementRef);
                    }
                }
                return _requirementRef;
            }
            set
            {
                if (_requirementRef != value)
                {
                    _requirementRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_requirementRef != null)
                    {
                        Requirement = _requirementRef.RequirementId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.RequirementRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
