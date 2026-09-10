
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AuthorityBoundaries")]
    public class AuthorityBoundaryBase : SoAEntityBase
    {
        [Key]
        public string AuthorityBoundaryId { get; set; }

        // Formula Name (rulebook: ={{ForbiddenAgentKind}} & " may not " & {{ForbiddenDecisionKind}})
        public string? Name
        {
            get => this.ForbiddenAgentKind + " may not " + this.ForbiddenDecisionKind; set { }
        }

        public string? ForbiddenAgentKind { get; set; }
        public string? ForbiddenDecisionKind { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string? Status { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula IsCurrentlyBinding (rulebook: =AND({{Status}} = "Approved", {{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        public bool? IsCurrentlyBinding
        {
            get => AND(this.Status = "Approved", this.ValidFrom <= this.AsOfInstant, OR(this.ValidTo = "", this.ValidTo > this.AsOfInstant)); set { }
        }

        // Formula RatifyingFragmentIsValid (rulebook: =INDEX(KnowledgeFragments!{{IsCurrentlyValid}}, MATCH({{RatifiedByKnowledgeFragment}}, KnowledgeFragments!{{KnowledgeFragmentId}}, 0)))
        public bool? RatifyingFragmentIsValid
        {
            get => INDEX(KnowledgeFragments!this.IsCurrentlyValid, MATCH(this.RatifiedByKnowledgeFragment, KnowledgeFragments!this.KnowledgeFragmentId, 0)); set { }
        }

        // Formula StepWhenBinding (rulebook: =IF({{IsCurrentlyBinding}}, {{Step}}, ""))
        public string? StepWhenBinding
        {
            get => IF(this.IsCurrentlyBinding, this.Step, ""); set { }
        }

        // Formula BoundaryMatchKey (rulebook: ={{Step}} & "|" & {{ForbiddenAgentKind}} & "|" & {{ForbiddenDecisionKind}})
        public string? BoundaryMatchKey
        {
            get => this.Step + "|" + this.ForbiddenAgentKind + "|" + this.ForbiddenDecisionKind; set { }
        }

        // Formula ViolationCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{BoundaryMatchKey}}, {{BoundaryMatchKey}}))
        public decimal? ViolationCount
        {
            get => COUNTIFS(AgentDecisionRecords!this.BoundaryMatchKey, this.BoundaryMatchKey); set { }
        }

        // Formula IsUntested (rulebook: =AND({{IsCurrentlyBinding}}, {{ViolationCount}} = 0))
        public bool? IsUntested
        {
            get => AND(this.IsCurrentlyBinding, this.ViolationCount = 0); set { }
        }

        // Formula HasRatifyingFragment (rulebook: ={{RatifiedByKnowledgeFragment}} <> "")
        public bool? HasRatifyingFragment
        {
            get => this.RatifiedByKnowledgeFragment <> ""; set { }
        }

        // Formula IsUnwarranted (rulebook: =AND({{IsCurrentlyBinding}}, OR(NOT({{HasRatifyingFragment}}), NOT({{RatifyingFragmentIsValid}}))))
        public bool? IsUnwarranted
        {
            get => AND(this.IsCurrentlyBinding, OR(NOT(this.HasRatifyingFragment), NOT(this.RatifyingFragmentIsValid))); set { }
        }

        // Formula RatifyingFragmentIsOverdue (rulebook: =INDEX(KnowledgeFragments!{{IsOverdueForReview}}, MATCH({{RatifiedByKnowledgeFragment}}, KnowledgeFragments!{{KnowledgeFragmentId}}, 0)))
        public bool? RatifyingFragmentIsOverdue
        {
            get => INDEX(KnowledgeFragments!this.IsOverdueForReview, MATCH(this.RatifiedByKnowledgeFragment, KnowledgeFragments!this.KnowledgeFragmentId, 0)); set { }
        }

        // Formula RatifyingFragmentIsSingleWitness (rulebook: =INDEX(KnowledgeFragments!{{IsFromSingleWitness}}, MATCH({{RatifiedByKnowledgeFragment}}, KnowledgeFragments!{{KnowledgeFragmentId}}, 0)))
        public bool? RatifyingFragmentIsSingleWitness
        {
            get => INDEX(KnowledgeFragments!this.IsFromSingleWitness, MATCH(this.RatifiedByKnowledgeFragment, KnowledgeFragments!this.KnowledgeFragmentId, 0)); set { }
        }

        // Formula WarrantIsThin (rulebook: =AND({{IsCurrentlyBinding}}, OR({{RatifyingFragmentIsOverdue}}, {{RatifyingFragmentIsSingleWitness}})))
        public bool? WarrantIsThin
        {
            get => AND(this.IsCurrentlyBinding, OR(this.RatifyingFragmentIsOverdue, this.RatifyingFragmentIsSingleWitness)); set { }
        }

        // Formula IsUnwarrantedAndUntested (rulebook: =AND({{IsUnwarranted}}, {{IsUntested}}))
        public bool? IsUnwarrantedAndUntested
        {
            get => AND(this.IsUnwarranted, this.IsUntested); set { }
        }

        // Formula UnwarrantedBoundaryStepKey (rulebook: =IF({{IsUnwarranted}}, {{Step}}, ""))
        public string? UnwarrantedBoundaryStepKey
        {
            get => IF(this.IsUnwarranted, this.Step, ""); set { }
        }

        // Formula RatifyingFragmentKey (rulebook: =IF({{IsCurrentlyBinding}}, {{RatifiedByKnowledgeFragment}}, ""))
        public string? RatifyingFragmentKey
        {
            get => IF(this.IsCurrentlyBinding, this.RatifiedByKnowledgeFragment, ""); set { }
        }

        // Formula RatifyingFragmentStatus (rulebook: =INDEX(KnowledgeFragments!{{Status}}, MATCH({{RatifiedByKnowledgeFragment}}, KnowledgeFragments!{{KnowledgeFragmentId}}, 0)))
        public string? RatifyingFragmentStatus
        {
            get => INDEX(KnowledgeFragments!this.Status, MATCH(this.RatifiedByKnowledgeFragment, KnowledgeFragments!this.KnowledgeFragmentId, 0)); set { }
        }

        // Formula RatificationLapsed (rulebook: =AND({{HasRatifyingFragment}}, NOT({{RatifyingFragmentIsValid}})))
        public bool? RatificationLapsed
        {
            get => AND(this.HasRatifyingFragment, NOT(this.RatifyingFragmentIsValid)); set { }
        }

        // Formula BindsDespiteLapsedRatification (rulebook: =AND({{IsCurrentlyBinding}}, {{RatificationLapsed}}))
        public bool? BindsDespiteLapsedRatification
        {
            get => AND(this.IsCurrentlyBinding, this.RatificationLapsed); set { }
        }

        // Formula IsUngroundedAndUntested (rulebook: =AND({{BindsDespiteLapsedRatification}}, {{IsUntested}}))
        public bool? IsUngroundedAndUntested
        {
            get => AND(this.BindsDespiteLapsedRatification, this.IsUntested); set { }
        }

        // Formula ConstrainedRoleAssignmentKey (rulebook: =IF({{BindsDespiteLapsedRatification}}, {{AuthorityRole}}, ""))
        public string? ConstrainedRoleAssignmentKey
        {
            get => IF(this.BindsDespiteLapsedRatification, this.AuthorityRole, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Step { get; set; }
        public string? RatifiedByKnowledgeFragment { get; set; }
        public string? EnforcingRequirement { get; set; }
        public string? AuthorityRole { get; set; }
        public string? EvaluationContext { get; set; }

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

        private KnowledgeFragment _knowledgeFragment;

        [ForeignKey("RatifiedByKnowledgeFragment")]
        public virtual KnowledgeFragment KnowledgeFragment
        {
            get
            {
                if (_knowledgeFragment == null && !string.IsNullOrEmpty(RatifiedByKnowledgeFragment))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragment - no database context is set. RatifiedByKnowledgeFragment: " + RatifiedByKnowledgeFragment + ".");
                        }
                        return null;
                    }
                    _knowledgeFragment = Context.KnowledgeFragments.Find(RatifiedByKnowledgeFragment);
                    if (_knowledgeFragment != null)
                    {
                        Context.Attach(_knowledgeFragment);
                    }
                }
                return _knowledgeFragment;
            }
            set
            {
                if (_knowledgeFragment != value)
                {
                    _knowledgeFragment = value;
                    RatifiedByKnowledgeFragment = _knowledgeFragment == null ? default : _knowledgeFragment.KnowledgeFragmentId;
                }
            }
        }

        private Requirement _requirement;

        [ForeignKey("EnforcingRequirement")]
        public virtual Requirement Requirement
        {
            get
            {
                if (_requirement == null && !string.IsNullOrEmpty(EnforcingRequirement))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Requirement - no database context is set. EnforcingRequirement: " + EnforcingRequirement + ".");
                        }
                        return null;
                    }
                    _requirement = Context.Requirements.Find(EnforcingRequirement);
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
                    EnforcingRequirement = _requirement == null ? default : _requirement.RequirementId;
                }
            }
        }

        private Role _role;

        [ForeignKey("AuthorityRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(AuthorityRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AuthorityRole: " + AuthorityRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(AuthorityRole);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    AuthorityRole = _role == null ? default : _role.RoleId;
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
            _ = this.Step;
            _ = this.KnowledgeFragment;
            _ = this.Requirement;
            _ = this.Role;
            _ = this.EvaluationContext;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
