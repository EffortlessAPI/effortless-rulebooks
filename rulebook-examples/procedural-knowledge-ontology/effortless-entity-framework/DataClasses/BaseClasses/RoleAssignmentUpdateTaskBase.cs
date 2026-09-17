
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
    [Table("RoleAssignmentUpdateTasks")]
    public class RoleAssignmentUpdateTaskBase : SoAEntityBase
    {
        [Key]
        public string RoleAssignmentUpdateTaskId { get; set; }

        // Formula Name (rulebook: ={{Role}} & " " & {{TriggerEvent}} & " @ " & {{TriggerOccurredAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Role)), F.S(" "), F.Text(F.Of(this.TriggerEvent)), F.S(" @ "), F.DatetimeText(F.Of(this.TriggerOccurredAt))))); set { }
        }

        public string? TriggerEvent { get; set; }
        public string? Reason { get; set; }
        public DateTimeOffset? TriggerOccurredAt { get; set; }
        public DateTimeOffset? TriggeredAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula PolicyTriggerOwnerRole (rulebook: =INDEX(AssignmentUpdatePolicies!{{TriggerOwnerRole}}, MATCH({{GoverningPolicy}}, AssignmentUpdatePolicies!{{AssignmentUpdatePolicyId}}, 0)))
        [NotMapped]
        public string? PolicyTriggerOwnerRole
        {
            get => F.AsString(F.Memo(this, "PolicyTriggerOwnerRole", () => F.Lookup<AssignmentUpdatePolicy>(this, "AssignmentUpdatePolicies", "AssignmentUpdatePolicyId", __c => __c.AssignmentUpdatePolicies, __r => F.Of(__r.AssignmentUpdatePolicyId), F.Of(this.GoverningPolicy), __r => F.Of(__r.TriggerOwnerRole), () => F.Of(new AssignmentUpdatePolicy().TriggerOwnerRole)))); set { }
        }

        // Formula TriggerOwnerCoverCount (rulebook: =INDEX(Roles!{{CurrentlyCoveredAssignmentCount}}, MATCH({{PolicyTriggerOwnerRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public int? TriggerOwnerCoverCount
        {
            get => F.AsInt(F.Memo(this, "TriggerOwnerCoverCount", () => F.Integer(F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.PolicyTriggerOwnerRole), __r => F.Of(__r.CurrentlyCoveredAssignmentCount), () => F.Of(new Role().CurrentlyCoveredAssignmentCount))))); set { }
        }

        // Formula LacksNamedTriggerOwner (rulebook: =OR({{PolicyTriggerOwnerRole}} = "", {{TriggerOwnerCoverCount}} = 0))
        [NotMapped]
        public bool? LacksNamedTriggerOwner
        {
            get => F.AsBool(F.Memo(this, "LacksNamedTriggerOwner", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.PolicyTriggerOwnerRole))), F.Bool3(F.Eq(F.Of(this.TriggerOwnerCoverCount), F.I(0)))))); set { }
        }

        // Formula PolicySlaHours (rulebook: =INDEX(AssignmentUpdatePolicies!{{UpdateSlaHours}}, MATCH({{GoverningPolicy}}, AssignmentUpdatePolicies!{{AssignmentUpdatePolicyId}}, 0)))
        [NotMapped]
        public int? PolicySlaHours
        {
            get => F.AsInt(F.Memo(this, "PolicySlaHours", () => F.Integer(F.Lookup<AssignmentUpdatePolicy>(this, "AssignmentUpdatePolicies", "AssignmentUpdatePolicyId", __c => __c.AssignmentUpdatePolicies, __r => F.Of(__r.AssignmentUpdatePolicyId), F.Of(this.GoverningPolicy), __r => F.Of(__r.UpdateSlaHours), () => F.Of(new AssignmentUpdatePolicy().UpdateSlaHours))))); set { }
        }

        // Formula ElapsedMinutes (rulebook: =IF({{CompletedAt}} = "", DATETIME_DIFF({{AsOfInstant}}, {{TriggerOccurredAt}}, "minutes"), DATETIME_DIFF({{CompletedAt}}, {{TriggerOccurredAt}}, "minutes")))
        [NotMapped]
        public decimal? ElapsedMinutes
        {
            get => F.AsDecimal(F.Memo(this, "ElapsedMinutes", () => (F.Truthy(F.Bool3(F.IsBlank(F.Of(this.CompletedAt)))) ? F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.TriggerOccurredAt), F.S("minutes")) : F.DatetimeDiff(F.Of(this.CompletedAt), F.Of(this.TriggerOccurredAt), F.S("minutes"))))); set { }
        }

        // Formula ExceededUpdateSla (rulebook: =OR({{PolicySlaHours}} = 0, {{ElapsedMinutes}} > ({{PolicySlaHours}} * 60)))
        [NotMapped]
        public bool? ExceededUpdateSla
        {
            get => F.AsBool(F.Memo(this, "ExceededUpdateSla", () => F.Or(F.Bool3(F.Eq(F.Of(this.PolicySlaHours), F.I(0))), F.Bool3(F.Cmp(F.Of(this.ElapsedMinutes), ">", F.Mul(F.Of(this.PolicySlaHours), F.I(60))))))); set { }
        }

        // Formula EndingRole (rulebook: =INDEX(RoleAssignments!{{Role}}, MATCH({{EndingAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public string? EndingRole
        {
            get => F.AsString(F.Memo(this, "EndingRole", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.EndingAssignment), __r => F.Of(__r.Role), () => F.Of(new RoleAssignment().Role)))); set { }
        }

        // Formula ReplacementRole (rulebook: =INDEX(RoleAssignments!{{Role}}, MATCH({{ReplacementAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public string? ReplacementRole
        {
            get => F.AsString(F.Memo(this, "ReplacementRole", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.ReplacementAssignment), __r => F.Of(__r.Role), () => F.Of(new RoleAssignment().Role)))); set { }
        }

        // Formula ChangedRoleInsteadOfAssignment (rulebook: =AND({{ReplacementAssignment}} <> "", {{ReplacementRole}} <> {{EndingRole}}))
        [NotMapped]
        public bool? ChangedRoleInsteadOfAssignment
        {
            get => F.AsBool(F.Memo(this, "ChangedRoleInsteadOfAssignment", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ReplacementAssignment))), F.Bool3(F.Ne(F.Of(this.ReplacementRole), F.Of(this.EndingRole)))))); set { }
        }

        // Formula DependentRunVersion (rulebook: =INDEX(ProcedureExecutions!{{ProcedureVersion}}, MATCH({{DependentExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        [NotMapped]
        public string? DependentRunVersion
        {
            get => F.AsString(F.Memo(this, "DependentRunVersion", () => F.Lookup<ProcedureExecution>(this, "ProcedureExecutions", "ProcedureExecutionId", __c => __c.ProcedureExecutions, __r => F.Of(__r.ProcedureExecutionId), F.Of(this.DependentExecution), __r => F.Of(__r.ProcedureVersion), () => F.Of(new ProcedureExecution().ProcedureVersion)))); set { }
        }

        // Formula DependentRunStartedAt (rulebook: =INDEX(ProcedureExecutions!{{StartedAt}}, MATCH({{DependentExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        [NotMapped]
        public DateTimeOffset? DependentRunStartedAt
        {
            get => F.AsDateTime(F.Memo(this, "DependentRunStartedAt", () => F.Lookup<ProcedureExecution>(this, "ProcedureExecutions", "ProcedureExecutionId", __c => __c.ProcedureExecutions, __r => F.Of(__r.ProcedureExecutionId), F.Of(this.DependentExecution), __r => F.Of(__r.StartedAt), () => F.Of(new ProcedureExecution().StartedAt)))); set { }
        }

        // Formula DependentRunRoleStepCount (rulebook: =COUNTIFS(Steps!{{ProcedureVersion}}, {{DependentRunVersion}}, Steps!{{AssignedRole}}, {{Role}}))
        [NotMapped]
        public int? DependentRunRoleStepCount
        {
            get => F.AsInt(F.Memo(this, "DependentRunRoleStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.DependentRunVersion)) && F.CritField(F.Of(__r.AssignedRole), F.Of(this.Role))))))); set { }
        }

        // Formula MissedNextDependentRun (rulebook: =AND({{DependentRunRoleStepCount}} > 0, {{DependentRunStartedAt}} > {{TriggerOccurredAt}}, OR({{CompletedAt}} = "", {{CompletedAt}} > {{DependentRunStartedAt}})))
        [NotMapped]
        public bool? MissedNextDependentRun
        {
            get => F.AsBool(F.Memo(this, "MissedNextDependentRun", () => F.And(F.Bool3(F.Cmp(F.Of(this.DependentRunRoleStepCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.DependentRunStartedAt), ">", F.Nullif(F.Of(this.TriggerOccurredAt)))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.CompletedAt))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.CompletedAt)), ">", F.Of(this.DependentRunStartedAt)))))))); set { }
        }

        // Formula FailedNoticeCount (rulebook: =COUNTIFS(AssignmentRoutedNotices!{{ProcedureExecution}}, {{DependentExecution}}, AssignmentRoutedNotices!{{NoticeRole}}, {{Role}}, AssignmentRoutedNotices!{{ReachedWrongPersonOrNobody}}, TRUE))
        [NotMapped]
        public int? FailedNoticeCount
        {
            get => F.AsInt(F.Memo(this, "FailedNoticeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AssignmentRoutedNotice>(base.SoAContext, "AssignmentRoutedNotices", __c => __c.AssignmentRoutedNotices), __r => F.CritField(F.Of(__r.ProcedureExecution), F.Of(this.DependentExecution)) && F.CritField(F.Of(__r.NoticeRole), F.Of(this.Role)) && F.CritLiteral(F.Of(__r.ReachedWrongPersonOrNobody), F.B(true))))))); set { }
        }

        // Formula StaleAssignmentBrokeRouting (rulebook: =AND({{MissedNextDependentRun}}, {{FailedNoticeCount}} > 0))
        [NotMapped]
        public bool? StaleAssignmentBrokeRouting
        {
            get => F.AsBool(F.Memo(this, "StaleAssignmentBrokeRouting", () => F.And(F.Bool3(F.Of(this.MissedNextDependentRun)), F.Bool3(F.Cmp(F.Of(this.FailedNoticeCount), ">", F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Role { get; set; }
        public string? GoverningPolicy { get; set; }
        public string? TriggeredByAgent { get; set; }
        public string? EndingAssignment { get; set; }
        public string? ReplacementAssignment { get; set; }
        public string? DependentExecution { get; set; }
        public string? EvaluationContext { get; set; }

        private Role _roleRef;

        [ForeignKey("Role")]
        public virtual Role RoleRef
        {
            get
            {
                if (_roleRef == null && !string.IsNullOrEmpty(Role))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleRef - no database context is set. Role: " + Role + ".");
                        }
                        return null;
                    }
                    _roleRef = base.SoAContext.Roles.Find(Role);
                    if (_roleRef != null)
                    {
                        base.SoAContext.Attach(_roleRef);
                    }
                }
                return _roleRef;
            }
            set
            {
                if (_roleRef != value)
                {
                    _roleRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleRef != null)
                    {
                        Role = _roleRef.RoleId;
                    }
                }
            }
        }

        private AssignmentUpdatePolicy _assignmentUpdatePolicy;

        [ForeignKey("GoverningPolicy")]
        public virtual AssignmentUpdatePolicy AssignmentUpdatePolicy
        {
            get
            {
                if (_assignmentUpdatePolicy == null && !string.IsNullOrEmpty(GoverningPolicy))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssignmentUpdatePolicy - no database context is set. GoverningPolicy: " + GoverningPolicy + ".");
                        }
                        return null;
                    }
                    _assignmentUpdatePolicy = base.SoAContext.AssignmentUpdatePolicies.Find(GoverningPolicy);
                    if (_assignmentUpdatePolicy != null)
                    {
                        base.SoAContext.Attach(_assignmentUpdatePolicy);
                    }
                }
                return _assignmentUpdatePolicy;
            }
            set
            {
                if (_assignmentUpdatePolicy != value)
                {
                    _assignmentUpdatePolicy = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_assignmentUpdatePolicy != null)
                    {
                        GoverningPolicy = _assignmentUpdatePolicy.AssignmentUpdatePolicyId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("TriggeredByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(TriggeredByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. TriggeredByAgent: " + TriggeredByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(TriggeredByAgent);
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
                        TriggeredByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private RoleAssignment _roleAssignment;

        [ForeignKey("EndingAssignment")]
        public virtual RoleAssignment RoleAssignment
        {
            get
            {
                if (_roleAssignment == null && !string.IsNullOrEmpty(EndingAssignment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignment - no database context is set. EndingAssignment: " + EndingAssignment + ".");
                        }
                        return null;
                    }
                    _roleAssignment = base.SoAContext.RoleAssignments.Find(EndingAssignment);
                    if (_roleAssignment != null)
                    {
                        base.SoAContext.Attach(_roleAssignment);
                    }
                }
                return _roleAssignment;
            }
            set
            {
                if (_roleAssignment != value)
                {
                    _roleAssignment = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleAssignment != null)
                    {
                        EndingAssignment = _roleAssignment.RoleAssignmentId;
                    }
                }
            }
        }

        private RoleAssignment _roleAssignmentRef;

        [ForeignKey("ReplacementAssignment")]
        public virtual RoleAssignment RoleAssignmentRef
        {
            get
            {
                if (_roleAssignmentRef == null && !string.IsNullOrEmpty(ReplacementAssignment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignmentRef - no database context is set. ReplacementAssignment: " + ReplacementAssignment + ".");
                        }
                        return null;
                    }
                    _roleAssignmentRef = base.SoAContext.RoleAssignments.Find(ReplacementAssignment);
                    if (_roleAssignmentRef != null)
                    {
                        base.SoAContext.Attach(_roleAssignmentRef);
                    }
                }
                return _roleAssignmentRef;
            }
            set
            {
                if (_roleAssignmentRef != value)
                {
                    _roleAssignmentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleAssignmentRef != null)
                    {
                        ReplacementAssignment = _roleAssignmentRef.RoleAssignmentId;
                    }
                }
            }
        }

        private ProcedureExecution _procedureExecution;

        [ForeignKey("DependentExecution")]
        public virtual ProcedureExecution ProcedureExecution
        {
            get
            {
                if (_procedureExecution == null && !string.IsNullOrEmpty(DependentExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecution - no database context is set. DependentExecution: " + DependentExecution + ".");
                        }
                        return null;
                    }
                    _procedureExecution = base.SoAContext.ProcedureExecutions.Find(DependentExecution);
                    if (_procedureExecution != null)
                    {
                        base.SoAContext.Attach(_procedureExecution);
                    }
                }
                return _procedureExecution;
            }
            set
            {
                if (_procedureExecution != value)
                {
                    _procedureExecution = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureExecution != null)
                    {
                        DependentExecution = _procedureExecution.ProcedureExecutionId;
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
            _ = this.RoleRef;
            _ = this.AssignmentUpdatePolicy;
            _ = this.Agent;
            _ = this.RoleAssignment;
            _ = this.RoleAssignmentRef;
            _ = this.ProcedureExecution;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
