
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
    [Table("AgentDecisionRecords")]
    public class AgentDecisionRecordBase : SoAEntityBase
    {
        [Key]
        public string AgentDecisionRecordId { get; set; }

        // Formula Name (rulebook: ={{DecidingAgent}} & ": " & LEFT({{DecisionSummary}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.DecidingAgent)), F.S(": "), F.TextNotNull(F.Left(F.Of(this.DecisionSummary), F.I(60)))))); set { }
        }

        public string? DecisionKind { get; set; }
        public string? DecisionSummary { get; set; }
        public DateTimeOffset? DecidedAt { get; set; }
        public string? MaterialityBand { get; set; }
        public string? HumanDisposition { get; set; }
        public DateTimeOffset? ReviewedAt { get; set; }
        // Formula WasOverridden (rulebook: =OR({{HumanDisposition}} = "Corrected", {{HumanDisposition}} = "Reversed"))
        [NotMapped]
        public bool? WasOverridden
        {
            get => F.AsBool(F.Memo(this, "WasOverridden", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.HumanDisposition)), F.S("Corrected"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.HumanDisposition)), F.S("Reversed")))))); set { }
        }

        // Formula WasReviewed (rulebook: =AND({{HumanDisposition}} <> "", {{HumanDisposition}} <> "NotReviewed"))
        [NotMapped]
        public bool? WasReviewed
        {
            get => F.AsBool(F.Memo(this, "WasReviewed", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.HumanDisposition))), F.Bool3(F.Ne(F.Nullif(F.Of(this.HumanDisposition)), F.S("NotReviewed")))))); set { }
        }

        // Formula DecidingAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{DecidingAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? DecidingAgentKind
        {
            get => F.AsString(F.Memo(this, "DecidingAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.DecidingAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula DecidingAgentWhenOverridden (rulebook: =IF({{WasOverridden}}, {{DecidingAgent}}, ""))
        [NotMapped]
        public string? DecidingAgentWhenOverridden
        {
            get => F.AsString(F.Memo(this, "DecidingAgentWhenOverridden", () => (F.Truthy(F.Bool3(F.Of(this.WasOverridden))) ? F.Of(this.DecidingAgent) : F.S("")))); set { }
        }

        // Formula RoleAssignmentWhenScored (rulebook: =IF({{UnderRoleAssignment}} <> "", {{UnderRoleAssignment}}, ""))
        [NotMapped]
        public string? RoleAssignmentWhenScored
        {
            get => F.AsString(F.Memo(this, "RoleAssignmentWhenScored", () => (F.Truthy(F.Bool3(F.IsNotBlank(F.Of(this.UnderRoleAssignment)))) ? F.Of(this.UnderRoleAssignment) : F.S("")))); set { }
        }

        // Formula RoleAssignmentWhenOverridden (rulebook: =IF({{WasOverridden}}, {{UnderRoleAssignment}}, ""))
        [NotMapped]
        public string? RoleAssignmentWhenOverridden
        {
            get => F.AsString(F.Memo(this, "RoleAssignmentWhenOverridden", () => (F.Truthy(F.Bool3(F.Of(this.WasOverridden))) ? F.Of(this.UnderRoleAssignment) : F.S("")))); set { }
        }

        // Formula StepOfDecision (rulebook: =INDEX(StepExecutions!{{Step}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? StepOfDecision
        {
            get => F.AsString(F.Memo(this, "StepOfDecision", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.Step), () => F.Of(new StepExecution().Step)))); set { }
        }

        // Formula BoundaryMatchKey (rulebook: ={{StepOfDecision}} & "|" & {{DecidingAgentKind}} & "|" & {{DecisionKind}})
        [NotMapped]
        public string? BoundaryMatchKey
        {
            get => F.AsString(F.Memo(this, "BoundaryMatchKey", () => F.Concat(F.TextOr(F.Of(this.StepOfDecision)), F.S("|"), F.TextOr(F.Of(this.DecidingAgentKind)), F.S("|"), F.TextOr(F.Of(this.DecisionKind))))); set { }
        }

        // Formula MatchingBoundaryCount (rulebook: =COUNTIFS(AuthorityBoundaries!{{BoundaryMatchKey}}, {{BoundaryMatchKey}}))
        [NotMapped]
        public decimal? MatchingBoundaryCount
        {
            get => F.AsDecimal(F.Memo(this, "MatchingBoundaryCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AuthorityBoundary>(base.SoAContext, "AuthorityBoundaries", __c => __c.AuthorityBoundaries), __r => F.CritField(F.Of(__r.BoundaryMatchKey), F.Of(this.BoundaryMatchKey)))))); set { }
        }

        // Formula ViolatedAuthorityBoundary (rulebook: ={{MatchingBoundaryCount}} > 0)
        [NotMapped]
        public bool? ViolatedAuthorityBoundary
        {
            get => F.AsBool(F.Memo(this, "ViolatedAuthorityBoundary", () => F.Cmp(F.Of(this.MatchingBoundaryCount), ">", F.I(0)))); set { }
        }

        // Formula ReviewerAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{ReviewedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? ReviewerAgentKind
        {
            get => F.AsString(F.Memo(this, "ReviewerAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.ReviewedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula HasHumanConfirmation (rulebook: =AND({{ReviewerAgentKind}} = "Human", {{HumanDisposition}} <> "", {{HumanDisposition}} <> "NotReviewed"))
        [NotMapped]
        public bool? HasHumanConfirmation
        {
            get => F.AsBool(F.Memo(this, "HasHumanConfirmation", () => F.And(F.Bool3(F.Eq(F.Of(this.ReviewerAgentKind), F.S("Human"))), F.Bool3(F.IsNotBlank(F.Of(this.HumanDisposition))), F.Bool3(F.Ne(F.Nullif(F.Of(this.HumanDisposition)), F.S("NotReviewed")))))); set { }
        }

        // Formula NeedsHumanConfirmation (rulebook: =AND(NOT({{DecidingAgentKind}} = "Human"), OR({{MaterialityBand}} = "Material", {{MaterialityBand}} = "Escalated")))
        [NotMapped]
        public bool? NeedsHumanConfirmation
        {
            get => F.AsBool(F.Memo(this, "NeedsHumanConfirmation", () => F.And(F.Bool3(F.Not(F.Bool3(F.Eq(F.Of(this.DecidingAgentKind), F.S("Human"))))), F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.MaterialityBand)), F.S("Material"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.MaterialityBand)), F.S("Escalated")))))))); set { }
        }

        // Formula IsUnconfirmedNonHumanDecision (rulebook: =AND({{NeedsHumanConfirmation}}, NOT({{HasHumanConfirmation}})))
        [NotMapped]
        public bool? IsUnconfirmedNonHumanDecision
        {
            get => F.AsBool(F.Memo(this, "IsUnconfirmedNonHumanDecision", () => F.And(F.Bool3(F.Of(this.NeedsHumanConfirmation)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasHumanConfirmation))))))); set { }
        }

        // Formula StepExecutionWhenUnconfirmed (rulebook: =IF({{IsUnconfirmedNonHumanDecision}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? StepExecutionWhenUnconfirmed
        {
            get => F.AsString(F.Memo(this, "StepExecutionWhenUnconfirmed", () => (F.Truthy(F.Bool3(F.Of(this.IsUnconfirmedNonHumanDecision))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        // Formula AgentWhenBoundaryViolated (rulebook: =IF({{ViolatedAuthorityBoundary}}, {{DecidingAgent}}, ""))
        [NotMapped]
        public string? AgentWhenBoundaryViolated
        {
            get => F.AsString(F.Memo(this, "AgentWhenBoundaryViolated", () => (F.Truthy(F.Bool3(F.Of(this.ViolatedAuthorityBoundary))) ? F.Of(this.DecidingAgent) : F.S("")))); set { }
        }

        // Formula ReviewLatencyMinutes (rulebook: =IF({{ReviewedAt}} = "", 0, DATETIME_DIFF({{ReviewedAt}}, {{DecidedAt}}, "minutes")))
        [NotMapped]
        public decimal? ReviewLatencyMinutes
        {
            get => F.AsDecimal(F.Memo(this, "ReviewLatencyMinutes", () => (F.Truthy(F.Bool3(F.IsBlank(F.Of(this.ReviewedAt)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.ReviewedAt), F.Of(this.DecidedAt), F.S("minutes"))))); set { }
        }

        // Formula IsDraftKind (rulebook: =OR({{DecisionKind}} = "Draft", {{DecisionKind}} = "Commitment"))
        [NotMapped]
        public bool? IsDraftKind
        {
            get => F.AsBool(F.Memo(this, "IsDraftKind", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.DecisionKind)), F.S("Draft"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.DecisionKind)), F.S("Commitment")))))); set { }
        }

        // Formula AgentWhenDraftOverridden (rulebook: =IF(AND({{IsDraftKind}}, {{WasOverridden}}), {{DecidingAgent}}, ""))
        [NotMapped]
        public string? AgentWhenDraftOverridden
        {
            get => F.AsString(F.Memo(this, "AgentWhenDraftOverridden", () => (F.Truthy(F.Bool3(F.And(F.Bool3(F.Of(this.IsDraftKind)), F.Bool3(F.Of(this.WasOverridden))))) ? F.Of(this.DecidingAgent) : F.S("")))); set { }
        }

        // Formula AgentWhenDraft (rulebook: =IF({{IsDraftKind}}, {{DecidingAgent}}, ""))
        [NotMapped]
        public string? AgentWhenDraft
        {
            get => F.AsString(F.Memo(this, "AgentWhenDraft", () => (F.Truthy(F.Bool3(F.Of(this.IsDraftKind))) ? F.Of(this.DecidingAgent) : F.S("")))); set { }
        }

        public string? OverrideReasonKind { get; set; }
        // Formula IsErrorCorrection (rulebook: =AND({{WasOverridden}}, {{OverrideReasonKind}} = "ErrorCorrection"))
        [NotMapped]
        public bool? IsErrorCorrection
        {
            get => F.AsBool(F.Memo(this, "IsErrorCorrection", () => F.And(F.Bool3(F.Of(this.WasOverridden)), F.Bool3(F.Eq(F.Nullif(F.Of(this.OverrideReasonKind)), F.S("ErrorCorrection")))))); set { }
        }

        // Formula IsReservedJudgmentOverride (rulebook: =AND({{WasOverridden}}, {{OverrideReasonKind}} = "JudgmentReserved"))
        [NotMapped]
        public bool? IsReservedJudgmentOverride
        {
            get => F.AsBool(F.Memo(this, "IsReservedJudgmentOverride", () => F.And(F.Bool3(F.Of(this.WasOverridden)), F.Bool3(F.Eq(F.Nullif(F.Of(this.OverrideReasonKind)), F.S("JudgmentReserved")))))); set { }
        }

        // Formula OverrideReasonIsRecorded (rulebook: =AND({{WasOverridden}}, {{OverrideReasonKind}} <> ""))
        [NotMapped]
        public bool? OverrideReasonIsRecorded
        {
            get => F.AsBool(F.Memo(this, "OverrideReasonIsRecorded", () => F.And(F.Bool3(F.Of(this.WasOverridden)), F.Bool3(F.IsNotBlank(F.Of(this.OverrideReasonKind)))))); set { }
        }

        // Formula IsUnexplainedOverride (rulebook: =AND({{WasOverridden}}, NOT({{OverrideReasonIsRecorded}})))
        [NotMapped]
        public bool? IsUnexplainedOverride
        {
            get => F.AsBool(F.Memo(this, "IsUnexplainedOverride", () => F.And(F.Bool3(F.Of(this.WasOverridden)), F.Bool3(F.Not(F.Bool3(F.Of(this.OverrideReasonIsRecorded))))))); set { }
        }

        // Formula ErrorCorrectionRoleAssignmentKey (rulebook: =IF({{IsErrorCorrection}}, {{UnderRoleAssignment}}, ""))
        [NotMapped]
        public string? ErrorCorrectionRoleAssignmentKey
        {
            get => F.AsString(F.Memo(this, "ErrorCorrectionRoleAssignmentKey", () => (F.Truthy(F.Bool3(F.Of(this.IsErrorCorrection))) ? F.Of(this.UnderRoleAssignment) : F.S("")))); set { }
        }

        // Formula BoundaryViolationRoleAssignmentKey (rulebook: =IF({{ViolatedAuthorityBoundary}}, {{UnderRoleAssignment}}, ""))
        [NotMapped]
        public string? BoundaryViolationRoleAssignmentKey
        {
            get => F.AsString(F.Memo(this, "BoundaryViolationRoleAssignmentKey", () => (F.Truthy(F.Bool3(F.Of(this.ViolatedAuthorityBoundary))) ? F.Of(this.UnderRoleAssignment) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? DecidingAgent { get; set; }
        public string? ReviewedByAgent { get; set; }
        public string? UnderRoleAssignment { get; set; }

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

        private Agent _agent;

        [ForeignKey("DecidingAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(DecidingAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. DecidingAgent: " + DecidingAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(DecidingAgent);
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
                        DecidingAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("ReviewedByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(ReviewedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. ReviewedByAgent: " + ReviewedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(ReviewedByAgent);
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
                        ReviewedByAgent = _agentRef.AgentId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignment - no database context is set. UnderRoleAssignment: " + UnderRoleAssignment + ".");
                        }
                        return null;
                    }
                    _roleAssignment = base.SoAContext.RoleAssignments.Find(UnderRoleAssignment);
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
                        UnderRoleAssignment = _roleAssignment.RoleAssignmentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecutionRef;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.RoleAssignment;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
