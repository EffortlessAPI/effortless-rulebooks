
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ExceptionInvocations")]
    public class ExceptionInvocationBase : SoAEntityBase
    {
        [Key]
        public string ExceptionInvocationId { get; set; }

        // Formula Name (rulebook: ={{StepExecution}} & " / " & {{Exception}})
        public string? Name
        {
            get => this.StepExecution + " / " + this.Exception; set { }
        }

        public DateTime? InvokedAt { get; set; }
        public string? HandlingApplied { get; set; }
        // Formula ExpectedHandling (rulebook: =INDEX(Exceptions!{{Handling}}, MATCH({{Exception}}, Exceptions!{{ExceptionId}}, 0)))
        public string? ExpectedHandling
        {
            get => INDEX(Exceptions!this.Handling, MATCH(this.Exception, Exceptions!this.ExceptionId, 0)); set { }
        }

        // Formula RequiredApprovalRole (rulebook: =INDEX(Exceptions!{{ApprovalRole}}, MATCH({{Exception}}, Exceptions!{{ExceptionId}}, 0)))
        public string? RequiredApprovalRole
        {
            get => INDEX(Exceptions!this.ApprovalRole, MATCH(this.Exception, Exceptions!this.ExceptionId, 0)); set { }
        }

        // Formula RequiredApprovalRoleHolder (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{RequiredApprovalRole}}, Roles!{{RoleId}}, 0)))
        public string? RequiredApprovalRoleHolder
        {
            get => INDEX(Roles!this.CurrentAgent, MATCH(this.RequiredApprovalRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula ApprovalRoleMatches (rulebook: ={{ApprovedByAgent}} = {{RequiredApprovalRoleHolder}})
        public bool? ApprovalRoleMatches
        {
            get => this.ApprovedByAgent = this.RequiredApprovalRoleHolder; set { }
        }

        // Formula IsApproved (rulebook: ={{ApprovedByAgent}} <> "")
        public bool? IsApproved
        {
            get => this.ApprovedByAgent <> ""; set { }
        }

        // Formula IsImproperlyApproved (rulebook: =OR(NOT({{IsApproved}}), NOT({{ApprovalRoleMatches}})))
        public bool? IsImproperlyApproved
        {
            get => OR(NOT(this.IsApproved), NOT(this.ApprovalRoleMatches)); set { }
        }

        // Formula InvokerAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{InvokedByAgent}}, Agents!{{AgentId}}, 0)))
        public string? InvokerAgentKind
        {
            get => INDEX(Agents!this.AgentKind, MATCH(this.InvokedByAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula InvokerAlsoPreparedKey (rulebook: ={{ParentProcedureExecution}} & "|" & {{ApprovedByAgent}})
        public string? InvokerAlsoPreparedKey
        {
            get => this.ParentProcedureExecution + "|" + this.ApprovedByAgent; set { }
        }

        // Formula ParentProcedureExecution (rulebook: =INDEX(StepExecutions!{{ProcedureExecution}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        public string? ParentProcedureExecution
        {
            get => INDEX(StepExecutions!this.ProcedureExecution, MATCH(this.StepExecution, StepExecutions!this.StepExecutionId, 0)); set { }
        }

        // Formula ApproverPreparedCount (rulebook: =COUNTIFS(StepExecutions!{{PreparerAgentKey}}, {{InvokerAlsoPreparedKey}}))
        public decimal? ApproverPreparedCount
        {
            get => COUNTIFS(StepExecutions!this.PreparerAgentKey, this.InvokerAlsoPreparedKey); set { }
        }

        // Formula DelegatedToPreparer (rulebook: ={{ApproverPreparedCount}} > 0)
        public bool? DelegatedToPreparer
        {
            get => this.ApproverPreparedCount > 0; set { }
        }

        // Formula IsUngovernedInvocation (rulebook: =OR({{IsImproperlyApproved}}, {{DelegatedToPreparer}}))
        public bool? IsUngovernedInvocation
        {
            get => OR(this.IsImproperlyApproved, this.DelegatedToPreparer); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? Exception { get; set; }
        public string? InvokedByAgent { get; set; }
        public string? ApprovedByAgent { get; set; }

        private StepExecution _stepExecution;

        [ForeignKey("StepExecution")]
        public virtual StepExecution StepExecution
        {
            get
            {
                if (_stepExecution == null && !string.IsNullOrEmpty(StepExecution))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecution - no database context is set. StepExecution: " + StepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecution = Context.StepExecutions.Find(StepExecution);
                    if (_stepExecution != null)
                    {
                        Context.Attach(_stepExecution);
                    }
                }
                return _stepExecution;
            }
            set
            {
                if (_stepExecution != value)
                {
                    _stepExecution = value;
                    StepExecution = _stepExecution == null ? default : _stepExecution.StepExecutionId;
                }
            }
        }

        private Exception _exception;

        [ForeignKey("Exception")]
        public virtual Exception Exception
        {
            get
            {
                if (_exception == null && !string.IsNullOrEmpty(Exception))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Exception - no database context is set. Exception: " + Exception + ".");
                        }
                        return null;
                    }
                    _exception = Context.Exceptions.Find(Exception);
                    if (_exception != null)
                    {
                        Context.Attach(_exception);
                    }
                }
                return _exception;
            }
            set
            {
                if (_exception != value)
                {
                    _exception = value;
                    Exception = _exception == null ? default : _exception.ExceptionId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. InvokedByAgent: " + InvokedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(InvokedByAgent);
                    if (_agent != null)
                    {
                        Context.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    InvokedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private Agent _agent;

        [ForeignKey("ApprovedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ApprovedByAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ApprovedByAgent: " + ApprovedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(ApprovedByAgent);
                    if (_agent != null)
                    {
                        Context.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    ApprovedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecution;
            _ = this.Exception;
            _ = this.Agent;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
