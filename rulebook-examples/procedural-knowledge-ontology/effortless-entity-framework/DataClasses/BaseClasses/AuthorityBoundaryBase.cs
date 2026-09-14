
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
    [Table("AuthorityBoundaries")]
    public class AuthorityBoundaryBase : SoAEntityBase
    {
        [Key]
        public string AuthorityBoundaryId { get; set; }

        // Formula Name (rulebook: ={{ForbiddenAgentKind}} & " may not " & {{ForbiddenDecisionKind}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ForbiddenAgentKind)), F.S(" may not "), F.Text(F.Of(this.ForbiddenDecisionKind))))); set { }
        }

        public string? ForbiddenAgentKind { get; set; }
        public string? ForbiddenDecisionKind { get; set; }
        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        public string? Status { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula IsCurrentlyBinding (rulebook: =AND({{Status}} = "Approved", {{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        [NotMapped]
        public bool? IsCurrentlyBinding
        {
            get => F.AsBool(F.Memo(this, "IsCurrentlyBinding", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved"))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidFrom)), "<=", F.Of(this.AsOfInstant))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ValidTo))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidTo)), ">", F.Of(this.AsOfInstant)))))))); set { }
        }

        // Formula RatifyingFragmentIsValid (rulebook: =INDEX(KnowledgeFragments!{{IsCurrentlyValid}}, MATCH({{RatifiedByKnowledgeFragment}}, KnowledgeFragments!{{KnowledgeFragmentId}}, 0)))
        [NotMapped]
        public bool? RatifyingFragmentIsValid
        {
            get => F.AsBool(F.Memo(this, "RatifyingFragmentIsValid", () => F.Lookup<KnowledgeFragment>(this, "KnowledgeFragments", "KnowledgeFragmentId", __c => __c.KnowledgeFragments, __r => F.Of(__r.KnowledgeFragmentId), F.Of(this.RatifiedByKnowledgeFragment), __r => F.Of(__r.IsCurrentlyValid), () => F.Of(new KnowledgeFragment().IsCurrentlyValid)))); set { }
        }

        // Formula StepWhenBinding (rulebook: =IF({{IsCurrentlyBinding}}, {{Step}}, ""))
        [NotMapped]
        public string? StepWhenBinding
        {
            get => F.AsString(F.Memo(this, "StepWhenBinding", () => (F.Truthy(F.Bool3(F.Of(this.IsCurrentlyBinding))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula BoundaryMatchKey (rulebook: ={{Step}} & "|" & {{ForbiddenAgentKind}} & "|" & {{ForbiddenDecisionKind}})
        [NotMapped]
        public string? BoundaryMatchKey
        {
            get => F.AsString(F.Memo(this, "BoundaryMatchKey", () => F.Concat(F.Text(F.Of(this.Step)), F.S("|"), F.Text(F.Of(this.ForbiddenAgentKind)), F.S("|"), F.Text(F.Of(this.ForbiddenDecisionKind))))); set { }
        }

        // Formula ViolationCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{BoundaryMatchKey}}, {{BoundaryMatchKey}}))
        [NotMapped]
        public decimal? ViolationCount
        {
            get => F.AsDecimal(F.Memo(this, "ViolationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.BoundaryMatchKey), F.Of(this.BoundaryMatchKey)))))); set { }
        }

        // Formula IsUntested (rulebook: =AND({{IsCurrentlyBinding}}, {{ViolationCount}} = 0))
        [NotMapped]
        public bool? IsUntested
        {
            get => F.AsBool(F.Memo(this, "IsUntested", () => F.And(F.Bool3(F.Of(this.IsCurrentlyBinding)), F.Bool3(F.Eq(F.Of(this.ViolationCount), F.I(0)))))); set { }
        }

        // Formula HasRatifyingFragment (rulebook: ={{RatifiedByKnowledgeFragment}} <> "")
        [NotMapped]
        public bool? HasRatifyingFragment
        {
            get => F.AsBool(F.Memo(this, "HasRatifyingFragment", () => F.IsNotBlank(F.Of(this.RatifiedByKnowledgeFragment)))); set { }
        }

        // Formula IsUnwarranted (rulebook: =AND({{IsCurrentlyBinding}}, OR(NOT({{HasRatifyingFragment}}), NOT({{RatifyingFragmentIsValid}}))))
        [NotMapped]
        public bool? IsUnwarranted
        {
            get => F.AsBool(F.Memo(this, "IsUnwarranted", () => F.And(F.Bool3(F.Of(this.IsCurrentlyBinding)), F.Bool3(F.Or(F.Bool3(F.Not(F.Bool3(F.Of(this.HasRatifyingFragment)))), F.Bool3(F.Not(F.Bool3(F.Of(this.RatifyingFragmentIsValid))))))))); set { }
        }

        // Formula RatifyingFragmentIsOverdue (rulebook: =INDEX(KnowledgeFragments!{{IsOverdueForReview}}, MATCH({{RatifiedByKnowledgeFragment}}, KnowledgeFragments!{{KnowledgeFragmentId}}, 0)))
        [NotMapped]
        public bool? RatifyingFragmentIsOverdue
        {
            get => F.AsBool(F.Memo(this, "RatifyingFragmentIsOverdue", () => F.Lookup<KnowledgeFragment>(this, "KnowledgeFragments", "KnowledgeFragmentId", __c => __c.KnowledgeFragments, __r => F.Of(__r.KnowledgeFragmentId), F.Of(this.RatifiedByKnowledgeFragment), __r => F.Of(__r.IsOverdueForReview), () => F.Of(new KnowledgeFragment().IsOverdueForReview)))); set { }
        }

        // Formula RatifyingFragmentIsSingleWitness (rulebook: =INDEX(KnowledgeFragments!{{IsFromSingleWitness}}, MATCH({{RatifiedByKnowledgeFragment}}, KnowledgeFragments!{{KnowledgeFragmentId}}, 0)))
        [NotMapped]
        public bool? RatifyingFragmentIsSingleWitness
        {
            get => F.AsBool(F.Memo(this, "RatifyingFragmentIsSingleWitness", () => F.Lookup<KnowledgeFragment>(this, "KnowledgeFragments", "KnowledgeFragmentId", __c => __c.KnowledgeFragments, __r => F.Of(__r.KnowledgeFragmentId), F.Of(this.RatifiedByKnowledgeFragment), __r => F.Of(__r.IsFromSingleWitness), () => F.Of(new KnowledgeFragment().IsFromSingleWitness)))); set { }
        }

        // Formula WarrantIsThin (rulebook: =AND({{IsCurrentlyBinding}}, OR({{RatifyingFragmentIsOverdue}}, {{RatifyingFragmentIsSingleWitness}})))
        [NotMapped]
        public bool? WarrantIsThin
        {
            get => F.AsBool(F.Memo(this, "WarrantIsThin", () => F.And(F.Bool3(F.Of(this.IsCurrentlyBinding)), F.Bool3(F.Or(F.Bool3(F.Of(this.RatifyingFragmentIsOverdue)), F.Bool3(F.Of(this.RatifyingFragmentIsSingleWitness))))))); set { }
        }

        // Formula IsUnwarrantedAndUntested (rulebook: =AND({{IsUnwarranted}}, {{IsUntested}}))
        [NotMapped]
        public bool? IsUnwarrantedAndUntested
        {
            get => F.AsBool(F.Memo(this, "IsUnwarrantedAndUntested", () => F.And(F.Bool3(F.Of(this.IsUnwarranted)), F.Bool3(F.Of(this.IsUntested))))); set { }
        }

        // Formula UnwarrantedBoundaryStepKey (rulebook: =IF({{IsUnwarranted}}, {{Step}}, ""))
        [NotMapped]
        public string? UnwarrantedBoundaryStepKey
        {
            get => F.AsString(F.Memo(this, "UnwarrantedBoundaryStepKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnwarranted))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula RatifyingFragmentKey (rulebook: =IF({{IsCurrentlyBinding}}, {{RatifiedByKnowledgeFragment}}, ""))
        [NotMapped]
        public string? RatifyingFragmentKey
        {
            get => F.AsString(F.Memo(this, "RatifyingFragmentKey", () => (F.Truthy(F.Bool3(F.Of(this.IsCurrentlyBinding))) ? F.Of(this.RatifiedByKnowledgeFragment) : F.S("")))); set { }
        }

        // Formula RatifyingFragmentStatus (rulebook: =INDEX(KnowledgeFragments!{{Status}}, MATCH({{RatifiedByKnowledgeFragment}}, KnowledgeFragments!{{KnowledgeFragmentId}}, 0)))
        [NotMapped]
        public string? RatifyingFragmentStatus
        {
            get => F.AsString(F.Memo(this, "RatifyingFragmentStatus", () => F.Lookup<KnowledgeFragment>(this, "KnowledgeFragments", "KnowledgeFragmentId", __c => __c.KnowledgeFragments, __r => F.Of(__r.KnowledgeFragmentId), F.Of(this.RatifiedByKnowledgeFragment), __r => F.Of(__r.Status), () => F.Of(new KnowledgeFragment().Status)))); set { }
        }

        // Formula RatificationLapsed (rulebook: =AND({{HasRatifyingFragment}}, NOT({{RatifyingFragmentIsValid}})))
        [NotMapped]
        public bool? RatificationLapsed
        {
            get => F.AsBool(F.Memo(this, "RatificationLapsed", () => F.And(F.Bool3(F.Of(this.HasRatifyingFragment)), F.Bool3(F.Not(F.Bool3(F.Of(this.RatifyingFragmentIsValid))))))); set { }
        }

        // Formula BindsDespiteLapsedRatification (rulebook: =AND({{IsCurrentlyBinding}}, {{RatificationLapsed}}))
        [NotMapped]
        public bool? BindsDespiteLapsedRatification
        {
            get => F.AsBool(F.Memo(this, "BindsDespiteLapsedRatification", () => F.And(F.Bool3(F.Of(this.IsCurrentlyBinding)), F.Bool3(F.Of(this.RatificationLapsed))))); set { }
        }

        // Formula IsUngroundedAndUntested (rulebook: =AND({{BindsDespiteLapsedRatification}}, {{IsUntested}}))
        [NotMapped]
        public bool? IsUngroundedAndUntested
        {
            get => F.AsBool(F.Memo(this, "IsUngroundedAndUntested", () => F.And(F.Bool3(F.Of(this.BindsDespiteLapsedRatification)), F.Bool3(F.Of(this.IsUntested))))); set { }
        }

        // Formula ConstrainedRoleAssignmentKey (rulebook: =IF({{BindsDespiteLapsedRatification}}, {{AuthorityRole}}, ""))
        [NotMapped]
        public string? ConstrainedRoleAssignmentKey
        {
            get => F.AsString(F.Memo(this, "ConstrainedRoleAssignmentKey", () => (F.Truthy(F.Bool3(F.Of(this.BindsDespiteLapsedRatification))) ? F.Of(this.AuthorityRole) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Step { get; set; }
        public string? RatifiedByKnowledgeFragment { get; set; }
        public string? EnforcingRequirement { get; set; }
        public string? AuthorityRole { get; set; }
        public string? EvaluationContext { get; set; }

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

        private KnowledgeFragment _knowledgeFragment;

        [ForeignKey("RatifiedByKnowledgeFragment")]
        public virtual KnowledgeFragment KnowledgeFragment
        {
            get
            {
                if (_knowledgeFragment == null && !string.IsNullOrEmpty(RatifiedByKnowledgeFragment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragment - no database context is set. RatifiedByKnowledgeFragment: " + RatifiedByKnowledgeFragment + ".");
                        }
                        return null;
                    }
                    _knowledgeFragment = base.SoAContext.KnowledgeFragments.Find(RatifiedByKnowledgeFragment);
                    if (_knowledgeFragment != null)
                    {
                        base.SoAContext.Attach(_knowledgeFragment);
                    }
                }
                return _knowledgeFragment;
            }
            set
            {
                if (_knowledgeFragment != value)
                {
                    _knowledgeFragment = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeFragment != null)
                    {
                        RatifiedByKnowledgeFragment = _knowledgeFragment.KnowledgeFragmentId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Requirement - no database context is set. EnforcingRequirement: " + EnforcingRequirement + ".");
                        }
                        return null;
                    }
                    _requirement = base.SoAContext.Requirements.Find(EnforcingRequirement);
                    if (_requirement != null)
                    {
                        base.SoAContext.Attach(_requirement);
                    }
                }
                return _requirement;
            }
            set
            {
                if (_requirement != value)
                {
                    _requirement = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_requirement != null)
                    {
                        EnforcingRequirement = _requirement.RequirementId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AuthorityRole: " + AuthorityRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(AuthorityRole);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        AuthorityRole = _role.RoleId;
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
            _ = this.StepRef;
            _ = this.KnowledgeFragment;
            _ = this.Requirement;
            _ = this.Role;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
