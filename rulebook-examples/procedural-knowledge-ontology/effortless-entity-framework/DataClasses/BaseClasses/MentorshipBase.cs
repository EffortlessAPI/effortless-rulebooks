
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
    [Table("Mentorships")]
    public class MentorshipBase : SoAEntityBase
    {
        [Key]
        public string MentorshipId { get; set; }

        // Formula Name (rulebook: ={{MentorAgent}} & " -> " & {{LearnerAgent}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.MentorAgent)), F.S(" -> "), F.Text(F.Of(this.LearnerAgent))))); set { }
        }

        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        public string? LearningObjective { get; set; }
        public string? EvidenceOfCompletion { get; set; }
        public string? SemanticTypeIri { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        public string? MentorshipForm { get; set; }
        public string? EmployerWorkerObligation { get; set; }
        public decimal? ExpectedWeeklyHours { get; set; }
        // Formula IsActive (rulebook: =AND({{ValidFrom}} <> "", {{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} >= {{AsOfInstant}})))
        [NotMapped]
        public bool? IsActive
        {
            get => F.AsBool(F.Memo(this, "IsActive", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ValidFrom))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidFrom)), "<=", F.Of(this.AsOfInstant))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ValidTo))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidTo)), ">=", F.Of(this.AsOfInstant)))))))); set { }
        }

        // Formula DaysSinceStarted (rulebook: =IF({{ValidFrom}} = "", 99999, DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days")))
        [NotMapped]
        public int? DaysSinceStarted
        {
            get => F.AsInt(F.Memo(this, "DaysSinceStarted", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.ValidFrom)))) ? F.I(99999) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.ValidFrom), F.S("days")))))); set { }
        }

        // Formula IsRecentApprenticeship (rulebook: =AND({{MentorshipForm}} = "Apprenticeship", {{DaysSinceStarted}} >= 0, {{DaysSinceStarted}} <= 1095))
        [NotMapped]
        public bool? IsRecentApprenticeship
        {
            get => F.AsBool(F.Memo(this, "IsRecentApprenticeship", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.MentorshipForm)), F.S("Apprenticeship"))), F.Bool3(F.Cmp(F.Of(this.DaysSinceStarted), ">=", F.I(0))), F.Bool3(F.Cmp(F.Of(this.DaysSinceStarted), "<=", F.I(1095)))))); set { }
        }

        // Formula CommunityLabel (rulebook: =INDEX(CommunitiesOfPractice!{{Label}}, MATCH({{CommunityOfPractice}}, CommunitiesOfPractice!{{CommunityOfPracticeId}}, 0)))
        [NotMapped]
        public string? CommunityLabel
        {
            get => F.AsString(F.Memo(this, "CommunityLabel", () => F.Lookup<CommunitiesOfPractice>(this, "CommunitiesOfPractice", "CommunityOfPracticeId", __c => __c.CommunitiesOfPractice, __r => F.Of(__r.CommunityOfPracticeId), F.Of(this.CommunityOfPractice), __r => F.Of(__r.Label), () => F.Of(new CommunitiesOfPractice().Label)))); set { }
        }


        public string? CommunityOfPractice { get; set; }
        public string? MentorAgent { get; set; }
        public string? LearnerAgent { get; set; }
        public string? EvaluationContext { get; set; }

        private CommunitiesOfPractice _communitiesOfPractice;

        [ForeignKey("CommunityOfPractice")]
        public virtual CommunitiesOfPractice CommunitiesOfPractice
        {
            get
            {
                if (_communitiesOfPractice == null && !string.IsNullOrEmpty(CommunityOfPractice))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunitiesOfPractice - no database context is set. CommunityOfPractice: " + CommunityOfPractice + ".");
                        }
                        return null;
                    }
                    _communitiesOfPractice = base.SoAContext.CommunitiesOfPractice.Find(CommunityOfPractice);
                    if (_communitiesOfPractice != null)
                    {
                        base.SoAContext.Attach(_communitiesOfPractice);
                    }
                }
                return _communitiesOfPractice;
            }
            set
            {
                if (_communitiesOfPractice != value)
                {
                    _communitiesOfPractice = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_communitiesOfPractice != null)
                    {
                        CommunityOfPractice = _communitiesOfPractice.CommunityOfPracticeId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("MentorAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(MentorAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. MentorAgent: " + MentorAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(MentorAgent);
                    if (_agent != null)
                    {
                        base.SoAContext.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agent != null)
                    {
                        MentorAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("LearnerAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(LearnerAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. LearnerAgent: " + LearnerAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(LearnerAgent);
                    if (_agentRef != null)
                    {
                        base.SoAContext.Attach(_agentRef);
                    }
                }
                return _agentRef;
            }
            set
            {
                if (_agentRef != value)
                {
                    _agentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRef != null)
                    {
                        LearnerAgent = _agentRef.AgentId;
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
            _ = this.CommunitiesOfPractice;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
