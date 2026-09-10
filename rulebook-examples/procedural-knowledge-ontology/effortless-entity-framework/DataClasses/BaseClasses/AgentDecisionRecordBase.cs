
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AgentDecisionRecords")]
    public class AgentDecisionRecordBase : SoAEntityBase
    {
        [Key]
        public string AgentDecisionRecordId { get; set; }

        // Formula Name (rulebook: ={{DecidingAgent}} & ": " & LEFT({{DecisionSummary}}, 60))
        public string? Name
        {
            get => this.DecidingAgent + ": " + LEFT(this.DecisionSummary, 60); set { }
        }

        public string? DecisionKind { get; set; }
        public string? DecisionSummary { get; set; }
        public DateTime? DecidedAt { get; set; }
        public string? MaterialityBand { get; set; }
        public string? HumanDisposition { get; set; }
        public DateTime? ReviewedAt { get; set; }
        // Formula WasOverridden (rulebook: =OR({{HumanDisposition}} = "Corrected", {{HumanDisposition}} = "Reversed"))
        public bool? WasOverridden
        {
            get => OR(this.HumanDisposition = "Corrected", this.HumanDisposition = "Reversed"); set { }
        }

        // Formula WasReviewed (rulebook: =AND({{HumanDisposition}} <> "", {{HumanDisposition}} <> "NotReviewed"))
        public bool? WasReviewed
        {
            get => AND(this.HumanDisposition <> "", this.HumanDisposition <> "NotReviewed"); set { }
        }

        // Formula DecidingAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{DecidingAgent}}, Agents!{{AgentId}}, 0)))
        public string? DecidingAgentKind
        {
            get => INDEX(Agents!this.AgentKind, MATCH(this.DecidingAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula DecidingAgentWhenOverridden (rulebook: =IF({{WasOverridden}}, {{DecidingAgent}}, ""))
        public string? DecidingAgentWhenOverridden
        {
            get => IF(this.WasOverridden, this.DecidingAgent, ""); set { }
        }

        // Formula RoleAssignmentWhenScored (rulebook: =IF({{UnderRoleAssignment}} <> "", {{UnderRoleAssignment}}, ""))
        public string? RoleAssignmentWhenScored
        {
            get => IF(this.UnderRoleAssignment <> "", this.UnderRoleAssignment, ""); set { }
        }

        // Formula RoleAssignmentWhenOverridden (rulebook: =IF({{WasOverridden}}, {{UnderRoleAssignment}}, ""))
        public string? RoleAssignmentWhenOverridden
        {
            get => IF(this.WasOverridden, this.UnderRoleAssignment, ""); set { }
        }

        // Formula StepOfDecision (rulebook: =INDEX(StepExecutions!{{Step}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        public string? StepOfDecision
        {
            get => INDEX(StepExecutions!this.Step, MATCH(this.StepExecution, StepExecutions!this.StepExecutionId, 0)); set { }
        }

        // Formula BoundaryMatchKey (rulebook: ={{StepOfDecision}} & "|" & {{DecidingAgentKind}} & "|" & {{DecisionKind}})
        public string? BoundaryMatchKey
        {
            get => this.StepOfDecision + "|" + this.DecidingAgentKind + "|" + this.DecisionKind; set { }
        }

        // Formula MatchingBoundaryCount (rulebook: =COUNTIFS(AuthorityBoundaries!{{BoundaryMatchKey}}, {{BoundaryMatchKey}}))
        public decimal? MatchingBoundaryCount
        {
            get => COUNTIFS(AuthorityBoundaries!this.BoundaryMatchKey, this.BoundaryMatchKey); set { }
        }

        // Formula ViolatedAuthorityBoundary (rulebook: ={{MatchingBoundaryCount}} > 0)
        public bool? ViolatedAuthorityBoundary
        {
            get => this.MatchingBoundaryCount > 0; set { }
        }

        // Formula ReviewerAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{ReviewedByAgent}}, Agents!{{AgentId}}, 0)))
        public string? ReviewerAgentKind
        {
            get => INDEX(Agents!this.AgentKind, MATCH(this.ReviewedByAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula HasHumanConfirmation (rulebook: =AND({{ReviewerAgentKind}} = "Human", {{HumanDisposition}} <> "", {{HumanDisposition}} <> "NotReviewed"))
        public bool? HasHumanConfirmation
        {
            get => AND(this.ReviewerAgentKind = "Human", this.HumanDisposition <> "", this.HumanDisposition <> "NotReviewed"); set { }
        }

        // Formula NeedsHumanConfirmation (rulebook: =AND(NOT({{DecidingAgentKind}} = "Human"), OR({{MaterialityBand}} = "Material", {{MaterialityBand}} = "Escalated")))
        public bool? NeedsHumanConfirmation
        {
            get => AND(NOT(this.DecidingAgentKind = "Human"), OR(this.MaterialityBand = "Material", this.MaterialityBand = "Escalated")); set { }
        }

        // Formula IsUnconfirmedNonHumanDecision (rulebook: =AND({{NeedsHumanConfirmation}}, NOT({{HasHumanConfirmation}})))
        public bool? IsUnconfirmedNonHumanDecision
        {
            get => AND(this.NeedsHumanConfirmation, NOT(this.HasHumanConfirmation)); set { }
        }

        // Formula StepExecutionWhenUnconfirmed (rulebook: =IF({{IsUnconfirmedNonHumanDecision}}, {{StepExecution}}, ""))
        public string? StepExecutionWhenUnconfirmed
        {
            get => IF(this.IsUnconfirmedNonHumanDecision, this.StepExecution, ""); set { }
        }

        // Formula AgentWhenBoundaryViolated (rulebook: =IF({{ViolatedAuthorityBoundary}}, {{DecidingAgent}}, ""))
        public string? AgentWhenBoundaryViolated
        {
            get => IF(this.ViolatedAuthorityBoundary, this.DecidingAgent, ""); set { }
        }

        // Formula ReviewLatencyMinutes (rulebook: =IF({{ReviewedAt}} = "", 0, DATETIME_DIFF({{ReviewedAt}}, {{DecidedAt}}, "minutes")))
        public decimal? ReviewLatencyMinutes
        {
            get => IF(this.ReviewedAt = "", 0, DATETIME_DIFF(this.ReviewedAt, this.DecidedAt, "minutes")); set { }
        }

        // Formula IsDraftKind (rulebook: =OR({{DecisionKind}} = "Draft", {{DecisionKind}} = "Commitment"))
        public bool? IsDraftKind
        {
            get => OR(this.DecisionKind = "Draft", this.DecisionKind = "Commitment"); set { }
        }

        // Formula AgentWhenDraftOverridden (rulebook: =IF(AND({{IsDraftKind}}, {{WasOverridden}}), {{DecidingAgent}}, ""))
        public string? AgentWhenDraftOverridden
        {
            get => IF(AND(this.IsDraftKind, this.WasOverridden), this.DecidingAgent, ""); set { }
        }

        // Formula AgentWhenDraft (rulebook: =IF({{IsDraftKind}}, {{DecidingAgent}}, ""))
        public string? AgentWhenDraft
        {
            get => IF(this.IsDraftKind, this.DecidingAgent, ""); set { }
        }

        public string? OverrideReasonKind { get; set; }
        // Formula IsErrorCorrection (rulebook: =AND({{WasOverridden}}, {{OverrideReasonKind}} = "ErrorCorrection"))
        public bool? IsErrorCorrection
        {
            get => AND(this.WasOverridden, this.OverrideReasonKind = "ErrorCorrection"); set { }
        }

        // Formula IsReservedJudgmentOverride (rulebook: =AND({{WasOverridden}}, {{OverrideReasonKind}} = "JudgmentReserved"))
        public bool? IsReservedJudgmentOverride
        {
            get => AND(this.WasOverridden, this.OverrideReasonKind = "JudgmentReserved"); set { }
        }

        // Formula OverrideReasonIsRecorded (rulebook: =AND({{WasOverridden}}, {{OverrideReasonKind}} <> ""))
        public bool? OverrideReasonIsRecorded
        {
            get => AND(this.WasOverridden, this.OverrideReasonKind <> ""); set { }
        }

        // Formula IsUnexplainedOverride (rulebook: =AND({{WasOverridden}}, NOT({{OverrideReasonIsRecorded}})))
        public bool? IsUnexplainedOverride
        {
            get => AND(this.WasOverridden, NOT(this.OverrideReasonIsRecorded)); set { }
        }

        // Formula ErrorCorrectionRoleAssignmentKey (rulebook: =IF({{IsErrorCorrection}}, {{UnderRoleAssignment}}, ""))
        public string? ErrorCorrectionRoleAssignmentKey
        {
            get => IF(this.IsErrorCorrection, this.UnderRoleAssignment, ""); set { }
        }

        // Formula BoundaryViolationRoleAssignmentKey (rulebook: =IF({{ViolatedAuthorityBoundary}}, {{UnderRoleAssignment}}, ""))
        public string? BoundaryViolationRoleAssignmentKey
        {
            get => IF(this.ViolatedAuthorityBoundary, this.UnderRoleAssignment, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? DecidingAgent { get; set; }
        public string? ReviewedByAgent { get; set; }
        public string? UnderRoleAssignment { get; set; }

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

        private Agent _agent;

        [ForeignKey("DecidingAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(DecidingAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. DecidingAgent: " + DecidingAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(DecidingAgent);
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
                    DecidingAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private Agent _agent;

        [ForeignKey("ReviewedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ReviewedByAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ReviewedByAgent: " + ReviewedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(ReviewedByAgent);
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
                    ReviewedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private RoleAssignment _roleAssignment;

        [ForeignKey("UnderRoleAssignment")]
        public virtual RoleAssignment RoleAssignment
        {
            get
            {
                if (_roleAssignment == null && !string.IsNullOrEmpty(UnderRoleAssignment))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignment - no database context is set. UnderRoleAssignment: " + UnderRoleAssignment + ".");
                        }
                        return null;
                    }
                    _roleAssignment = Context.RoleAssignments.Find(UnderRoleAssignment);
                    if (_roleAssignment != null)
                    {
                        Context.Attach(_roleAssignment);
                    }
                }
                return _roleAssignment;
            }
            set
            {
                if (_roleAssignment != value)
                {
                    _roleAssignment = value;
                    UnderRoleAssignment = _roleAssignment == null ? default : _roleAssignment.RoleAssignmentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecution;
            _ = this.Agent;
            _ = this.Agent;
            _ = this.RoleAssignment;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
