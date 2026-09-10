
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("StepRequirements")]
    public class StepRequirementBase : SoAEntityBase
    {
        [Key]
        public string StepRequirementId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{Requirement}})
        public string? Name
        {
            get => this.Step + " / " + this.Requirement; set { }
        }

        // Formula RequirementIsBlocking (rulebook: =INDEX(Requirements!{{IsBlocking}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        public bool? RequirementIsBlocking
        {
            get => INDEX(Requirements!this.IsBlocking, MATCH(this.Requirement, Requirements!this.RequirementId, 0)); set { }
        }

        // Formula BlockingStepKey (rulebook: =IF({{RequirementIsBlocking}}, {{Step}}, ""))
        public string? BlockingStepKey
        {
            get => IF(this.RequirementIsBlocking, this.Step, ""); set { }
        }

        // Formula StepWhenBlocking (rulebook: =IF({{RequirementIsBlocking}}, {{Step}}, ""))
        public string? StepWhenBlocking
        {
            get => IF(this.RequirementIsBlocking, this.Step, ""); set { }
        }

        // Formula RequirementLacksWitness (rulebook: =INDEX(Requirements!{{IsUnwitnessedBlockingControl}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        public bool? RequirementLacksWitness
        {
            get => INDEX(Requirements!this.IsUnwitnessedBlockingControl, MATCH(this.Requirement, Requirements!this.RequirementId, 0)); set { }
        }

        // Formula UnwitnessedStepKey (rulebook: =IF({{RequirementLacksWitness}}, {{Step}}, ""))
        public string? UnwitnessedStepKey
        {
            get => IF(this.RequirementLacksWitness, this.Step, ""); set { }
        }

        // Formula SatisfactionCountForBinding (rulebook: =COUNTIFS(RequirementSatisfactions!{{BindingKey}}, {{StepRequirementId}}))
        public decimal? SatisfactionCountForBinding
        {
            get => COUNTIFS(RequirementSatisfactions!this.BindingKey, this.StepRequirementId); set { }
        }

        // Formula BindingWasEverExercised (rulebook: ={{SatisfactionCountForBinding}} > 0)
        public bool? BindingWasEverExercised
        {
            get => this.SatisfactionCountForBinding > 0; set { }
        }

        // Formula IsUnexercisedBlockingBinding (rulebook: =AND({{RequirementIsBlocking}}, NOT({{BindingWasEverExercised}})))
        public bool? IsUnexercisedBlockingBinding
        {
            get => AND(this.RequirementIsBlocking, NOT(this.BindingWasEverExercised)); set { }
        }

        // Formula UnexercisedBindingRequirementKey (rulebook: =IF({{IsUnexercisedBlockingBinding}}, {{Requirement}}, ""))
        public string? UnexercisedBindingRequirementKey
        {
            get => IF(this.IsUnexercisedBlockingBinding, this.Requirement, ""); set { }
        }


        public string? Step { get; set; }
        public string? Requirement { get; set; }

        private Step _step;

        [ForeignKey("Step")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(Step))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _step = Context.Steps.Find(Step);
                    if (_step != null)
                    {
                        Context.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    Step = _step == null ? default : _step.StepId;
                }
            }
        }

        private Requirement _requirement;

        [ForeignKey("Requirement")]
        public virtual Requirement Requirement
        {
            get
            {
                if (_requirement == null && !string.IsNullOrEmpty(Requirement))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Requirement - no database context is set. Requirement: " + Requirement + ".");
                        }
                        return null;
                    }
                    _requirement = Context.Requirements.Find(Requirement);
                    if (_requirement != null)
                    {
                        Context.Attach(_requirement);
                    }
                }
                return _requirement;
            }
            set
            {
                if (_requirement != value)
                {
                    _requirement = value;
                    Requirement = _requirement == null ? default : _requirement.RequirementId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Step;
            _ = this.Requirement;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
