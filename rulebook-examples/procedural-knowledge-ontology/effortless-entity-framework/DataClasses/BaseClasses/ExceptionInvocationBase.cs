
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
    [Table("ExceptionInvocations")]
    public class ExceptionInvocationBase : SoAEntityBase
    {
        [Key]
        public string ExceptionInvocationId { get; set; }

        // Formula Name (rulebook: ={{StepExecution}} & " / " & {{Exception}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.StepExecution)), F.S(" / "), F.TextOr(F.Of(this.Exception))))); set { }
        }

        public DateTimeOffset? InvokedAt { get; set; }
        public string? HandlingApplied { get; set; }
        // Formula ExpectedHandling (rulebook: =INDEX(Exceptions!{{Handling}}, MATCH({{Exception}}, Exceptions!{{ExceptionId}}, 0)))
        [NotMapped]
        public string? ExpectedHandling
        {
            get => F.AsString(F.Memo(this, "ExpectedHandling", () => F.Lookup<Exception>(this, "Exceptions", "ExceptionId", __c => __c.Exceptions, __r => F.Of(__r.ExceptionId), F.Of(this.Exception), __r => F.Of(__r.Handling), () => F.Of(new Exception().Handling)))); set { }
        }

        // Formula RequiredApprovalRole (rulebook: =INDEX(Exceptions!{{ApprovalRole}}, MATCH({{Exception}}, Exceptions!{{ExceptionId}}, 0)))
        [NotMapped]
        public string? RequiredApprovalRole
        {
            get => F.AsString(F.Memo(this, "RequiredApprovalRole", () => F.Lookup<Exception>(this, "Exceptions", "ExceptionId", __c => __c.Exceptions, __r => F.Of(__r.ExceptionId), F.Of(this.Exception), __r => F.Of(__r.ApprovalRole), () => F.Of(new Exception().ApprovalRole)))); set { }
        }

        // Formula RequiredApprovalRoleHolder (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{RequiredApprovalRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? RequiredApprovalRoleHolder
        {
            get => F.AsString(F.Memo(this, "RequiredApprovalRoleHolder", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.RequiredApprovalRole), __r => F.Of(__r.CurrentAgent), () => F.Of(new Role().CurrentAgent)))); set { }
        }

        // Formula ApprovalRoleMatches (rulebook: ={{ApprovedByAgent}} = {{RequiredApprovalRoleHolder}})
        [NotMapped]
        public bool? ApprovalRoleMatches
        {
            get => F.AsBool(F.Memo(this, "ApprovalRoleMatches", () => F.Eq(F.Nullif(F.Of(this.ApprovedByAgent)), F.Of(this.RequiredApprovalRoleHolder)))); set { }
        }

        // Formula IsApproved (rulebook: ={{ApprovedByAgent}} <> "")
        [NotMapped]
        public bool? IsApproved
        {
            get => F.AsBool(F.Memo(this, "IsApproved", () => F.IsNotBlank(F.Of(this.ApprovedByAgent)))); set { }
        }

        // Formula IsImproperlyApproved (rulebook: =OR(NOT({{IsApproved}}), NOT({{ApprovalRoleMatches}})))
        [NotMapped]
        public bool? IsImproperlyApproved
        {
            get => F.AsBool(F.Memo(this, "IsImproperlyApproved", () => F.Or(F.Bool3(F.Not(F.Bool3(F.Of(this.IsApproved)))), F.Bool3(F.Not(F.Bool3(F.Of(this.ApprovalRoleMatches))))))); set { }
        }

        // Formula InvokerAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{InvokedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? InvokerAgentKind
        {
            get => F.AsString(F.Memo(this, "InvokerAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.InvokedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula InvokerAlsoPreparedKey (rulebook: ={{ParentProcedureExecution}} & "|" & {{ApprovedByAgent}})
        [NotMapped]
        public string? InvokerAlsoPreparedKey
        {
            get => F.AsString(F.Memo(this, "InvokerAlsoPreparedKey", () => F.Concat(F.TextOr(F.Of(this.ParentProcedureExecution)), F.S("|"), F.TextOr(F.Of(this.ApprovedByAgent))))); set { }
        }

        // Formula ParentProcedureExecution (rulebook: =INDEX(StepExecutions!{{ProcedureExecution}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? ParentProcedureExecution
        {
            get => F.AsString(F.Memo(this, "ParentProcedureExecution", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.ProcedureExecution), () => F.Of(new StepExecution().ProcedureExecution)))); set { }
        }

        // Formula ApproverPreparedCount (rulebook: =COUNTIFS(StepExecutions!{{PreparerAgentKey}}, {{InvokerAlsoPreparedKey}}))
        [NotMapped]
        public decimal? ApproverPreparedCount
        {
            get => F.AsDecimal(F.Memo(this, "ApproverPreparedCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.PreparerAgentKey), F.Of(this.InvokerAlsoPreparedKey)))))); set { }
        }

        // Formula DelegatedToPreparer (rulebook: ={{ApproverPreparedCount}} > 0)
        [NotMapped]
        public bool? DelegatedToPreparer
        {
            get => F.AsBool(F.Memo(this, "DelegatedToPreparer", () => F.Cmp(F.Of(this.ApproverPreparedCount), ">", F.I(0)))); set { }
        }

        // Formula IsUngovernedInvocation (rulebook: =OR({{IsImproperlyApproved}}, {{DelegatedToPreparer}}))
        [NotMapped]
        public bool? IsUngovernedInvocation
        {
            get => F.AsBool(F.Memo(this, "IsUngovernedInvocation", () => F.Or(F.Bool3(F.Of(this.IsImproperlyApproved)), F.Bool3(F.Of(this.DelegatedToPreparer))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? Exception { get; set; }
        public string? InvokedByAgent { get; set; }
        public string? ApprovedByAgent { get; set; }

        private StepExecution _stepExecutionRef;

        [ForeignKey("StepExecution")]
        public virtual StepExecution StepExecutionRef
        {
            get
            {
                if (_stepExecutionRef == null && !string.IsNullOrEmpty(StepExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecutionRef - no database context is set. StepExecution: " + StepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecutionRef = base.SoAContext.StepExecutions.Find(StepExecution);
                    if (_stepExecutionRef != null)
                    {
                        base.SoAContext.Attach(_stepExecutionRef);
                    }
                }
                return _stepExecutionRef;
            }
            set
            {
                if (_stepExecutionRef != value)
                {
                    _stepExecutionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepExecutionRef != null)
                    {
                        StepExecution = _stepExecutionRef.StepExecutionId;
                    }
                }
            }
        }

        private Exception _exceptionRef;

        [ForeignKey("Exception")]
        public virtual Exception ExceptionRef
        {
            get
            {
                if (_exceptionRef == null && !string.IsNullOrEmpty(Exception))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExceptionRef - no database context is set. Exception: " + Exception + ".");
                        }
                        return null;
                    }
                    _exceptionRef = base.SoAContext.Exceptions.Find(Exception);
                    if (_exceptionRef != null)
                    {
                        base.SoAContext.Attach(_exceptionRef);
                    }
                }
                return _exceptionRef;
            }
            set
            {
                if (_exceptionRef != value)
                {
                    _exceptionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_exceptionRef != null)
                    {
                        Exception = _exceptionRef.ExceptionId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("InvokedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(InvokedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. InvokedByAgent: " + InvokedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(InvokedByAgent);
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
                        InvokedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("ApprovedByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(ApprovedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. ApprovedByAgent: " + ApprovedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(ApprovedByAgent);
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
                        ApprovedByAgent = _agentRef.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecutionRef;
            _ = this.ExceptionRef;
            _ = this.Agent;
            _ = this.AgentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
