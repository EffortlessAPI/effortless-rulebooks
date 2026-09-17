
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
    [Table("StakeholderQuestions")]
    public class StakeholderQuestionBase : SoAEntityBase
    {
        [Key]
        public string StakeholderQuestionId { get; set; }

        // Formula Name (rulebook: =LEFT({{QuestionText}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Left(F.Of(this.QuestionText), F.I(60)))); set { }
        }

        public string? Channel { get; set; }
        public string? UseCase { get; set; }
        public string? QuestionText { get; set; }
        public DateTimeOffset? AskedAt { get; set; }
        public DateTimeOffset? AnsweredAt { get; set; }
        public string? TriageOutcome { get; set; }
        // Formula TriagerKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{TriagedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? TriagerKind
        {
            get => F.AsString(F.Memo(this, "TriagerKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.TriagedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula ResultRoute (rulebook: =INDEX(ModelChangeRequests!{{Route}}, MATCH({{ResultingChangeRequest}}, ModelChangeRequests!{{ModelChangeRequestId}}, 0)))
        [NotMapped]
        public string? ResultRoute
        {
            get => F.AsString(F.Memo(this, "ResultRoute", () => F.Lookup<ModelChangeRequest>(this, "ModelChangeRequests", "ModelChangeRequestId", __c => __c.ModelChangeRequests, __r => F.Of(__r.ModelChangeRequestId), F.Of(this.ResultingChangeRequest), __r => F.Of(__r.Route), () => F.Of(new ModelChangeRequest().Route)))); set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula DaysOpen (rulebook: =IF({{AnsweredAt}} = "", DATETIME_DIFF({{AsOfInstant}}, {{AskedAt}}, "days"), DATETIME_DIFF({{AnsweredAt}}, {{AskedAt}}, "days")))
        [NotMapped]
        public int? DaysOpen
        {
            get => F.AsInt(F.Memo(this, "DaysOpen", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.AnsweredAt)))) ? F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.AskedAt), F.S("days")) : F.DatetimeDiff(F.Of(this.AnsweredAt), F.Of(this.AskedAt), F.S("days")))))); set { }
        }

        // Formula IsUnansweredPastDue (rulebook: =AND({{AnsweredAt}} = "", {{DaysOpen}} > 14))
        [NotMapped]
        public bool? IsUnansweredPastDue
        {
            get => F.AsBool(F.Memo(this, "IsUnansweredPastDue", () => F.And(F.Bool3(F.IsBlank(F.Of(this.AnsweredAt))), F.Bool3(F.Cmp(F.Of(this.DaysOpen), ">", F.I(14)))))); set { }
        }

        // Formula IsUnanswerableToday (rulebook: ={{AnsweringRoleQuestion}} = "")
        [NotMapped]
        public bool? IsUnanswerableToday
        {
            get => F.AsBool(F.Memo(this, "IsUnanswerableToday", () => F.IsBlank(F.Of(this.AnsweringRoleQuestion)))); set { }
        }

        // Formula IsUntriagedUnanswerable (rulebook: =AND({{IsUnanswerableToday}}, {{TriageOutcome}} = ""))
        [NotMapped]
        public bool? IsUntriagedUnanswerable
        {
            get => F.AsBool(F.Memo(this, "IsUntriagedUnanswerable", () => F.And(F.Bool3(F.Of(this.IsUnanswerableToday)), F.Bool3(F.IsBlank(F.Of(this.TriageOutcome)))))); set { }
        }

        // Formula NeedsStructuralChange (rulebook: ={{TriageOutcome}} = "StructuralChange")
        [NotMapped]
        public bool? NeedsStructuralChange
        {
            get => F.AsBool(F.Memo(this, "NeedsStructuralChange", () => F.Eq(F.Nullif(F.Of(this.TriageOutcome)), F.S("StructuralChange")))); set { }
        }

        // Formula UnanswerableWithoutScopeRequest (rulebook: =AND({{IsUnanswerableToday}}, {{ResultingChangeRequest}} = ""))
        [NotMapped]
        public bool? UnanswerableWithoutScopeRequest
        {
            get => F.AsBool(F.Memo(this, "UnanswerableWithoutScopeRequest", () => F.And(F.Bool3(F.Of(this.IsUnanswerableToday)), F.Bool3(F.IsBlank(F.Of(this.ResultingChangeRequest)))))); set { }
        }

        // Formula IsMisroutedAfterTriage (rulebook: =AND({{ResultingChangeRequest}} <> "", OR(AND({{TriageOutcome}} = "MoreData", {{ResultRoute}} <> "DataOperations"), AND({{TriageOutcome}} <> "MoreData", {{TriageOutcome}} <> "", {{ResultRoute}} <> "Governance"))))
        [NotMapped]
        public bool? IsMisroutedAfterTriage
        {
            get => F.AsBool(F.Memo(this, "IsMisroutedAfterTriage", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ResultingChangeRequest))), F.Bool3(F.Or(F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.TriageOutcome)), F.S("MoreData"))), F.Bool3(F.Ne(F.Of(this.ResultRoute), F.S("DataOperations"))))), F.Bool3(F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.TriageOutcome)), F.S("MoreData"))), F.Bool3(F.IsNotBlank(F.Of(this.TriageOutcome))), F.Bool3(F.Ne(F.Of(this.ResultRoute), F.S("Governance")))))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? AskedByAgent { get; set; }
        public string? AnsweringRoleQuestion { get; set; }
        public string? AnsweredByAgent { get; set; }
        public string? TriagedByAgent { get; set; }
        public string? ResultingChangeRequest { get; set; }
        public string? EvaluationContext { get; set; }

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

        private Agent _agent;

        [ForeignKey("AskedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(AskedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. AskedByAgent: " + AskedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(AskedByAgent);
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
                        AskedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private RoleQuestion _roleQuestion;

        [ForeignKey("AnsweringRoleQuestion")]
        public virtual RoleQuestion RoleQuestion
        {
            get
            {
                if (_roleQuestion == null && !string.IsNullOrEmpty(AnsweringRoleQuestion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestion - no database context is set. AnsweringRoleQuestion: " + AnsweringRoleQuestion + ".");
                        }
                        return null;
                    }
                    _roleQuestion = base.SoAContext.RoleQuestions.Find(AnsweringRoleQuestion);
                    if (_roleQuestion != null)
                    {
                        base.SoAContext.Attach(_roleQuestion);
                    }
                }
                return _roleQuestion;
            }
            set
            {
                if (_roleQuestion != value)
                {
                    _roleQuestion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleQuestion != null)
                    {
                        AnsweringRoleQuestion = _roleQuestion.RoleQuestionId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("AnsweredByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(AnsweredByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. AnsweredByAgent: " + AnsweredByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(AnsweredByAgent);
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
                        AnsweredByAgent = _agentRef.AgentId;
                    }
                }
            }
        }

        private Agent _agentRefRef;

        [ForeignKey("TriagedByAgent")]
        public virtual Agent AgentRefRef
        {
            get
            {
                if (_agentRefRef == null && !string.IsNullOrEmpty(TriagedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRefRef - no database context is set. TriagedByAgent: " + TriagedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRefRef = base.SoAContext.Agents.Find(TriagedByAgent);
                    if (_agentRefRef != null)
                    {
                        base.SoAContext.Attach(_agentRefRef);
                    }
                }
                return _agentRefRef;
            }
            set
            {
                if (_agentRefRef != value)
                {
                    _agentRefRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRefRef != null)
                    {
                        TriagedByAgent = _agentRefRef.AgentId;
                    }
                }
            }
        }

        private ModelChangeRequest _modelChangeRequest;

        [ForeignKey("ResultingChangeRequest")]
        public virtual ModelChangeRequest ModelChangeRequest
        {
            get
            {
                if (_modelChangeRequest == null && !string.IsNullOrEmpty(ResultingChangeRequest))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelChangeRequest - no database context is set. ResultingChangeRequest: " + ResultingChangeRequest + ".");
                        }
                        return null;
                    }
                    _modelChangeRequest = base.SoAContext.ModelChangeRequests.Find(ResultingChangeRequest);
                    if (_modelChangeRequest != null)
                    {
                        base.SoAContext.Attach(_modelChangeRequest);
                    }
                }
                return _modelChangeRequest;
            }
            set
            {
                if (_modelChangeRequest != value)
                {
                    _modelChangeRequest = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_modelChangeRequest != null)
                    {
                        ResultingChangeRequest = _modelChangeRequest.ModelChangeRequestId;
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
            _ = this.GovernedModelRef;
            _ = this.Agent;
            _ = this.RoleQuestion;
            _ = this.AgentRef;
            _ = this.AgentRefRef;
            _ = this.ModelChangeRequest;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
