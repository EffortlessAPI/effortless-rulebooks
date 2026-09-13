
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
    [Table("RoleAssignments")]
    public class RoleAssignmentBase : SoAEntityBase
    {
        [Key]
        public string RoleAssignmentId { get; set; }

        // Formula Name (rulebook: ={{Role}} & " @ " & {{ValidFrom}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.Role)), F.S(" @ "), F.TimestamptzText(F.Of(this.ValidFrom))))); set { }
        }

        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula IsCurrent (rulebook: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        [NotMapped]
        public bool? IsCurrent
        {
            get => F.AsBool(F.Memo(this, "IsCurrent", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidFrom)), "<=", F.Of(this.AsOfInstant))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ValidTo))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidTo)), ">", F.Of(this.AsOfInstant)))))))); set { }
        }

        // Formula CurrentAgentKey (rulebook: =IF({{IsCurrent}}, {{Agent}}, ""))
        [NotMapped]
        public string? CurrentAgentKey
        {
            get => F.AsString(F.Memo(this, "CurrentAgentKey", () => (F.Truthy(F.Bool3(F.Of(this.IsCurrent))) ? F.Of(this.Agent) : F.S("")))); set { }
        }

        // Formula IsCurrentlyValid (rulebook: =AND({{Status}} = "Active", OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        [NotMapped]
        public bool? IsCurrentlyValid
        {
            get => F.AsBool(F.Memo(this, "IsCurrentlyValid", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Active"))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ValidTo))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidTo)), ">", F.Of(this.AsOfInstant)))))))); set { }
        }

        // Formula AgentRoleKey (rulebook: =IF({{IsCurrentlyValid}}, {{Agent}} & "|" & {{Role}}, ""))
        [NotMapped]
        public string? AgentRoleKey
        {
            get => F.AsString(F.Memo(this, "AgentRoleKey", () => (F.Truthy(F.Bool3(F.Of(this.IsCurrentlyValid))) ? F.Concat(F.TextOr(F.Of(this.Agent)), F.S("|"), F.TextOr(F.Of(this.Role))) : F.S("")))); set { }
        }

        // Formula HasDeparted (rulebook: =AND({{ValidTo}} <> "", {{ValidTo}} <= {{AsOfInstant}}))
        [NotMapped]
        public bool? HasDeparted
        {
            get => F.AsBool(F.Memo(this, "HasDeparted", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ValidTo))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidTo)), "<=", F.Of(this.AsOfInstant)))))); set { }
        }

        // Formula CoversNow (rulebook: =AND({{Status}} = "Active", {{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        [NotMapped]
        public bool? CoversNow
        {
            get => F.AsBool(F.Memo(this, "CoversNow", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Active"))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidFrom)), "<=", F.Of(this.AsOfInstant))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ValidTo))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidTo)), ">", F.Of(this.AsOfInstant)))))))); set { }
        }

        // Formula RoleWhenCovering (rulebook: =IF({{CoversNow}}, {{Role}}, ""))
        [NotMapped]
        public string? RoleWhenCovering
        {
            get => F.AsString(F.Memo(this, "RoleWhenCovering", () => (F.Truthy(F.Bool3(F.Of(this.CoversNow))) ? F.Of(this.Role) : F.S("")))); set { }
        }

        // Formula AgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{Agent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? AgentKind
        {
            get => F.AsString(F.Memo(this, "AgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.Agent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula IsNonHumanAssignment (rulebook: =NOT({{AgentKind}} = "Human"))
        [NotMapped]
        public bool? IsNonHumanAssignment
        {
            get => F.AsBool(F.Memo(this, "IsNonHumanAssignment", () => F.Not(F.Bool3(F.Eq(F.Of(this.AgentKind), F.S("Human")))))); set { }
        }

        // Formula PredecessorAgentKind (rulebook: =INDEX(RoleAssignments!{{AgentKind}}, MATCH({{SupersedesAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public string? PredecessorAgentKind
        {
            get => F.AsString(F.Memo(this, "PredecessorAgentKind", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.SupersedesAssignment), __r => F.Of(__r.AgentKind), () => F.Of(new RoleAssignment().AgentKind)))); set { }
        }

        // Formula IsHumanToNonHumanHandover (rulebook: =AND({{PredecessorAgentKind}} = "Human", {{IsNonHumanAssignment}}))
        [NotMapped]
        public bool? IsHumanToNonHumanHandover
        {
            get => F.AsBool(F.Memo(this, "IsHumanToNonHumanHandover", () => F.And(F.Bool3(F.Eq(F.Of(this.PredecessorAgentKind), F.S("Human"))), F.Bool3(F.Of(this.IsNonHumanAssignment))))); set { }
        }

        // Formula IsUnauthorizedNonHumanAssignment (rulebook: =AND({{IsNonHumanAssignment}}, NOT({{HasApprovingAuthority}})))
        [NotMapped]
        public bool? IsUnauthorizedNonHumanAssignment
        {
            get => F.AsBool(F.Memo(this, "IsUnauthorizedNonHumanAssignment", () => F.And(F.Bool3(F.Of(this.IsNonHumanAssignment)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasApprovingAuthority))))))); set { }
        }

        // Formula WasAuthorizedByChangeRequest (rulebook: =AND({{HasApprovingAuthority}}, {{AuthorizingChangeRequest}} <> ""))
        [NotMapped]
        public bool? WasAuthorizedByChangeRequest
        {
            get => F.AsBool(F.Memo(this, "WasAuthorizedByChangeRequest", () => F.And(F.Bool3(F.Of(this.HasApprovingAuthority)), F.Bool3(F.IsNotBlank(F.Of(this.AuthorizingChangeRequest)))))); set { }
        }

        // Formula DecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{RoleAssignmentWhenScored}}, {{RoleAssignmentId}}))
        [NotMapped]
        public decimal? DecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "DecisionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.RoleAssignmentWhenScored), F.Of(this.RoleAssignmentId)))))); set { }
        }

        // Formula OverriddenDecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{RoleAssignmentWhenOverridden}}, {{RoleAssignmentId}}))
        [NotMapped]
        public decimal? OverriddenDecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "OverriddenDecisionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.RoleAssignmentWhenOverridden), F.Of(this.RoleAssignmentId)))))); set { }
        }

        // Formula OverrideRatePercent (rulebook: =IF({{DecisionCount}} = 0, 0, ({{OverriddenDecisionCount}} * 100) / {{DecisionCount}}))
        [NotMapped]
        public decimal? OverrideRatePercent
        {
            get => F.AsDecimal(F.Memo(this, "OverrideRatePercent", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.DecisionCount), F.I(0)))) ? F.I(0) : F.Div(F.Mul(F.Of(this.OverriddenDecisionCount), F.I(100)), F.Of(this.DecisionCount))))); set { }
        }

        // Formula PredecessorOverrideRatePercent (rulebook: =INDEX(RoleAssignments!{{OverrideRatePercent}}, MATCH({{SupersedesAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public decimal? PredecessorOverrideRatePercent
        {
            get => F.AsDecimal(F.Memo(this, "PredecessorOverrideRatePercent", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.SupersedesAssignment), __r => F.Of(__r.OverrideRatePercent), () => F.Of(new RoleAssignment().OverrideRatePercent)))); set { }
        }

        // Formula QualityRegressedVsPredecessor (rulebook: =AND({{SupersedesAssignment}} <> "", {{OverrideRatePercent}} > {{PredecessorOverrideRatePercent}}))
        [NotMapped]
        public bool? QualityRegressedVsPredecessor
        {
            get => F.AsBool(F.Memo(this, "QualityRegressedVsPredecessor", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.SupersedesAssignment))), F.Bool3(F.Cmp(F.Of(this.OverrideRatePercent), ">", F.Of(this.PredecessorOverrideRatePercent)))))); set { }
        }

        // Formula DepartedRoleKey (rulebook: =IF({{HasDeparted}}, {{Role}}, ""))
        [NotMapped]
        public string? DepartedRoleKey
        {
            get => F.AsString(F.Memo(this, "DepartedRoleKey", () => (F.Truthy(F.Bool3(F.Of(this.HasDeparted))) ? F.Of(this.Role) : F.S("")))); set { }
        }

        public int? MinimumDecisionsForComparison { get; set; }
        // Formula PredecessorDecisionCount (rulebook: =INDEX(RoleAssignments!{{DecisionCount}}, MATCH({{SupersedesAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public decimal? PredecessorDecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "PredecessorDecisionCount", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.SupersedesAssignment), __r => F.Of(__r.DecisionCount), () => F.Of(new RoleAssignment().DecisionCount)))); set { }
        }

        // Formula HasSufficientSample (rulebook: ={{DecisionCount}} >= {{MinimumDecisionsForComparison}})
        [NotMapped]
        public bool? HasSufficientSample
        {
            get => F.AsBool(F.Memo(this, "HasSufficientSample", () => F.Cmp(F.Of(this.DecisionCount), ">=", F.Nullif(F.Of(this.MinimumDecisionsForComparison))))); set { }
        }

        // Formula PredecessorHasSufficientSample (rulebook: ={{PredecessorDecisionCount}} >= {{MinimumDecisionsForComparison}})
        [NotMapped]
        public bool? PredecessorHasSufficientSample
        {
            get => F.AsBool(F.Memo(this, "PredecessorHasSufficientSample", () => F.Cmp(F.Of(this.PredecessorDecisionCount), ">=", F.Nullif(F.Of(this.MinimumDecisionsForComparison))))); set { }
        }

        // Formula ComparisonIsEvidentiallySound (rulebook: =AND({{HasSufficientSample}}, {{PredecessorHasSufficientSample}}))
        [NotMapped]
        public bool? ComparisonIsEvidentiallySound
        {
            get => F.AsBool(F.Memo(this, "ComparisonIsEvidentiallySound", () => F.And(F.Bool3(F.Of(this.HasSufficientSample)), F.Bool3(F.Of(this.PredecessorHasSufficientSample))))); set { }
        }

        // Formula SingleOverrideSwingPercent (rulebook: =IF({{DecisionCount}} > 0, 100 / {{DecisionCount}}, 0))
        [NotMapped]
        public decimal? SingleOverrideSwingPercent
        {
            get => F.AsDecimal(F.Memo(this, "SingleOverrideSwingPercent", () => (F.Truthy(F.Bool3(F.Cmp(F.Of(this.DecisionCount), ">", F.I(0)))) ? F.Div(F.I(100), F.Of(this.DecisionCount)) : F.I(0)))); set { }
        }

        // Formula QualityVerdictIsUnsupported (rulebook: =AND(NOT({{ComparisonIsEvidentiallySound}}), NOT({{QualityRegressedVsPredecessor}})))
        [NotMapped]
        public bool? QualityVerdictIsUnsupported
        {
            get => F.AsBool(F.Memo(this, "QualityVerdictIsUnsupported", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.ComparisonIsEvidentiallySound)))), F.Bool3(F.Not(F.Bool3(F.Of(this.QualityRegressedVsPredecessor))))))); set { }
        }

        // Formula IsUnmeasuredAutomationHandover (rulebook: =AND({{IsHumanToNonHumanHandover}}, NOT({{ComparisonIsEvidentiallySound}})))
        [NotMapped]
        public bool? IsUnmeasuredAutomationHandover
        {
            get => F.AsBool(F.Memo(this, "IsUnmeasuredAutomationHandover", () => F.And(F.Bool3(F.Of(this.IsHumanToNonHumanHandover)), F.Bool3(F.Not(F.Bool3(F.Of(this.ComparisonIsEvidentiallySound))))))); set { }
        }

        // Formula ErrorCorrectionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{ErrorCorrectionRoleAssignmentKey}}, {{RoleAssignmentId}}))
        [NotMapped]
        public decimal? ErrorCorrectionCount
        {
            get => F.AsDecimal(F.Memo(this, "ErrorCorrectionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.ErrorCorrectionRoleAssignmentKey), F.Of(this.RoleAssignmentId)))))); set { }
        }

        // Formula ErrorRatePercent (rulebook: =IF({{DecisionCount}} > 0, {{ErrorCorrectionCount}} * 100 / {{DecisionCount}}, 0))
        [NotMapped]
        public decimal? ErrorRatePercent
        {
            get => F.AsDecimal(F.Memo(this, "ErrorRatePercent", () => (F.Truthy(F.Bool3(F.Cmp(F.Of(this.DecisionCount), ">", F.I(0)))) ? F.Div(F.Mul(F.Of(this.ErrorCorrectionCount), F.I(100)), F.Of(this.DecisionCount)) : F.I(0)))); set { }
        }

        public DateTimeOffset? AuthorizationDecidedAt { get; set; }
        public DateTimeOffset? AuthorizationReviewedAt { get; set; }
        public int? AuthorizationReviewCadenceDays { get; set; }
        // Formula HasDatedAuthorization (rulebook: =AND({{ApprovingAuthorityRole}} <> "", {{AuthorizationDecidedAt}} <> ""))
        [NotMapped]
        public bool? HasDatedAuthorization
        {
            get => F.AsBool(F.Memo(this, "HasDatedAuthorization", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ApprovingAuthorityRole))), F.Bool3(F.IsNotBlank(F.Of(this.AuthorizationDecidedAt)))))); set { }
        }

        // Formula DaysSinceAuthorizationReview (rulebook: =IF({{AuthorizationReviewedAt}} <> "", DATETIME_DIFF({{AsOfInstant}}, {{AuthorizationReviewedAt}}, "days"), DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days")))
        [NotMapped]
        public int? DaysSinceAuthorizationReview
        {
            get => F.AsInt(F.Memo(this, "DaysSinceAuthorizationReview", () => F.Integer((F.Truthy(F.Bool3(F.IsNotBlank(F.Of(this.AuthorizationReviewedAt)))) ? F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.AuthorizationReviewedAt), F.S("days")) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.ValidFrom), F.S("days")))))); set { }
        }

        // Formula AuthorizationIsOverdueForReview (rulebook: =AND({{AuthorizationReviewCadenceDays}} > 0, {{DaysSinceAuthorizationReview}} > {{AuthorizationReviewCadenceDays}}))
        [NotMapped]
        public bool? AuthorizationIsOverdueForReview
        {
            get => F.AsBool(F.Memo(this, "AuthorizationIsOverdueForReview", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.AuthorizationReviewCadenceDays)), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.DaysSinceAuthorizationReview), ">", F.Nullif(F.Of(this.AuthorizationReviewCadenceDays))))))); set { }
        }

        // Formula IsStandingUnreviewedAutomation (rulebook: =AND({{CoversNow}}, AND({{IsNonHumanAssignment}}, {{AuthorizationIsOverdueForReview}})))
        [NotMapped]
        public bool? IsStandingUnreviewedAutomation
        {
            get => F.AsBool(F.Memo(this, "IsStandingUnreviewedAutomation", () => F.And(F.Bool3(F.Of(this.CoversNow)), F.Bool3(F.And(F.Bool3(F.Of(this.IsNonHumanAssignment)), F.Bool3(F.Of(this.AuthorizationIsOverdueForReview))))))); set { }
        }

        // Formula IsUnconditionedAutomationHandover (rulebook: =AND({{IsHumanToNonHumanHandover}}, {{AuthorizationReviewCadenceDays}} = 0))
        [NotMapped]
        public bool? IsUnconditionedAutomationHandover
        {
            get => F.AsBool(F.Memo(this, "IsUnconditionedAutomationHandover", () => F.And(F.Bool3(F.Of(this.IsHumanToNonHumanHandover)), F.Bool3(F.Eq(F.Nullif(F.Of(this.AuthorizationReviewCadenceDays)), F.I(0)))))); set { }
        }

        public int? MaxTolerableErrorRatePercent { get; set; }
        // Formula ExceedsTolerableErrorRate (rulebook: =AND({{MaxTolerableErrorRatePercent}} > 0, {{ErrorRatePercent}} >= {{MaxTolerableErrorRatePercent}}))
        [NotMapped]
        public bool? ExceedsTolerableErrorRate
        {
            get => F.AsBool(F.Memo(this, "ExceedsTolerableErrorRate", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.MaxTolerableErrorRatePercent)), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.ErrorRatePercent), ">=", F.Nullif(F.Of(this.MaxTolerableErrorRatePercent))))))); set { }
        }

        // Formula BoundaryViolationCountForAssignment (rulebook: =COUNTIFS(AgentDecisionRecords!{{BoundaryViolationRoleAssignmentKey}}, {{RoleAssignmentId}}))
        [NotMapped]
        public decimal? BoundaryViolationCountForAssignment
        {
            get => F.AsDecimal(F.Memo(this, "BoundaryViolationCountForAssignment", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.BoundaryViolationRoleAssignmentKey), F.Of(this.RoleAssignmentId)))))); set { }
        }

        // Formula HasAnyBoundaryViolation (rulebook: =({{BoundaryViolationCountForAssignment}} > 0))
        [NotMapped]
        public bool? HasAnyBoundaryViolation
        {
            get => F.AsBool(F.Memo(this, "HasAnyBoundaryViolation", () => F.Cmp(F.Of(this.BoundaryViolationCountForAssignment), ">", F.I(0)))); set { }
        }

        // Formula HasUngroundedGoverningBoundary (rulebook: =INDEX(Roles!{{IsGovernedByLapsedAuthority}}, MATCH({{Role}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public bool? HasUngroundedGoverningBoundary
        {
            get => F.AsBool(F.Memo(this, "HasUngroundedGoverningBoundary", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.Role), __r => F.Of(__r.IsGovernedByLapsedAuthority), () => F.Of(new Role().IsGovernedByLapsedAuthority)))); set { }
        }

        // Formula SuspensionConditionMet (rulebook: =OR({{ExceedsTolerableErrorRate}}, OR({{HasAnyBoundaryViolation}}, {{HasUngroundedGoverningBoundary}})))
        [NotMapped]
        public bool? SuspensionConditionMet
        {
            get => F.AsBool(F.Memo(this, "SuspensionConditionMet", () => F.Or(F.Bool3(F.Of(this.ExceedsTolerableErrorRate)), F.Bool3(F.Or(F.Bool3(F.Of(this.HasAnyBoundaryViolation)), F.Bool3(F.Of(this.HasUngroundedGoverningBoundary))))))); set { }
        }

        // Formula IsOperatingUnderMetSuspensionCondition (rulebook: =AND({{SuspensionConditionMet}}, AND({{CoversNow}}, {{IsNonHumanAssignment}})))
        [NotMapped]
        public bool? IsOperatingUnderMetSuspensionCondition
        {
            get => F.AsBool(F.Memo(this, "IsOperatingUnderMetSuspensionCondition", () => F.And(F.Bool3(F.Of(this.SuspensionConditionMet)), F.Bool3(F.And(F.Bool3(F.Of(this.CoversNow)), F.Bool3(F.Of(this.IsNonHumanAssignment))))))); set { }
        }

        // Formula HasDeclaredSuspensionCondition (rulebook: ={{MaxTolerableErrorRatePercent}} > 0)
        [NotMapped]
        public bool? HasDeclaredSuspensionCondition
        {
            get => F.AsBool(F.Memo(this, "HasDeclaredSuspensionCondition", () => F.Cmp(F.Nullif(F.Of(this.MaxTolerableErrorRatePercent)), ">", F.I(0)))); set { }
        }

        // Formula HasApprovingAuthority (rulebook: ={{ApprovingAuthorityRole}} <> "")
        [NotMapped]
        public bool? HasApprovingAuthority
        {
            get => F.AsBool(F.Memo(this, "HasApprovingAuthority", () => F.IsNotBlank(F.Of(this.ApprovingAuthorityRole)))); set { }
        }

        // Formula HasAuthorizingChangeRequest (rulebook: ={{AuthorizingChangeRequest}} <> "")
        [NotMapped]
        public bool? HasAuthorizingChangeRequest
        {
            get => F.AsBool(F.Memo(this, "HasAuthorizingChangeRequest", () => F.IsNotBlank(F.Of(this.AuthorizingChangeRequest)))); set { }
        }

        public bool? IsEnforcementRole { get; set; }
        // Formula IsUnauthorizedEnforcementAgent (rulebook: =AND({{IsEnforcementRole}}, {{IsUnauthorizedNonHumanAssignment}}))
        [NotMapped]
        public bool? IsUnauthorizedEnforcementAgent
        {
            get => F.AsBool(F.Memo(this, "IsUnauthorizedEnforcementAgent", () => F.And(F.IsTrueV(F.Of(this.IsEnforcementRole)), F.Bool3(F.Of(this.IsUnauthorizedNonHumanAssignment))))); set { }
        }

        // Formula GovernanceEvidenceCount (rulebook: =IF({{HasApprovingAuthority}}, 1, 0) + IF({{HasAuthorizingChangeRequest}}, 1, 0))
        [NotMapped]
        public int? GovernanceEvidenceCount
        {
            get => F.AsInt(F.Memo(this, "GovernanceEvidenceCount", () => F.Integer(F.Add((F.Truthy(F.Bool3(F.Of(this.HasApprovingAuthority))) ? F.I(1) : F.I(0)), (F.Truthy(F.Bool3(F.Of(this.HasAuthorizingChangeRequest))) ? F.I(1) : F.I(0)))))); set { }
        }

        // Formula UnauthorizedEnforcementRoleKey (rulebook: =IF({{IsUnauthorizedNonHumanAssignment}}, {{Role}}, ""))
        [NotMapped]
        public string? UnauthorizedEnforcementRoleKey
        {
            get => F.AsString(F.Memo(this, "UnauthorizedEnforcementRoleKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnauthorizedNonHumanAssignment))) ? F.Of(this.Role) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Role { get; set; }
        public string? Agent { get; set; }
        public string? EvaluationContext { get; set; }
        public string? SupersedesAssignment { get; set; }
        public string? ApprovingAuthorityRole { get; set; }
        public string? AuthorizingChangeRequest { get; set; }

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

        private Agent _agentRef;

        [ForeignKey("Agent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(Agent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. Agent: " + Agent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(Agent);
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
                        Agent = _agentRef.AgentId;
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

        private RoleAssignment _roleAssignment;

        [ForeignKey("SupersedesAssignment")]
        public virtual RoleAssignment RoleAssignment
        {
            get
            {
                if (_roleAssignment == null && !string.IsNullOrEmpty(SupersedesAssignment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignment - no database context is set. SupersedesAssignment: " + SupersedesAssignment + ".");
                        }
                        return null;
                    }
                    _roleAssignment = base.SoAContext.RoleAssignments.Find(SupersedesAssignment);
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
                        SupersedesAssignment = _roleAssignment.RoleAssignmentId;
                    }
                }
            }
        }

        private Role _roleRefRef;

        [ForeignKey("ApprovingAuthorityRole")]
        public virtual Role RoleRefRef
        {
            get
            {
                if (_roleRefRef == null && !string.IsNullOrEmpty(ApprovingAuthorityRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleRefRef - no database context is set. ApprovingAuthorityRole: " + ApprovingAuthorityRole + ".");
                        }
                        return null;
                    }
                    _roleRefRef = base.SoAContext.Roles.Find(ApprovingAuthorityRole);
                    if (_roleRefRef != null)
                    {
                        base.SoAContext.Attach(_roleRefRef);
                    }
                }
                return _roleRefRef;
            }
            set
            {
                if (_roleRefRef != value)
                {
                    _roleRefRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleRefRef != null)
                    {
                        ApprovingAuthorityRole = _roleRefRef.RoleId;
                    }
                }
            }
        }

        private ChangeRequest _changeRequest;

        [ForeignKey("AuthorizingChangeRequest")]
        public virtual ChangeRequest ChangeRequest
        {
            get
            {
                if (_changeRequest == null && !string.IsNullOrEmpty(AuthorizingChangeRequest))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeRequest - no database context is set. AuthorizingChangeRequest: " + AuthorizingChangeRequest + ".");
                        }
                        return null;
                    }
                    _changeRequest = base.SoAContext.ChangeRequests.Find(AuthorizingChangeRequest);
                    if (_changeRequest != null)
                    {
                        base.SoAContext.Attach(_changeRequest);
                    }
                }
                return _changeRequest;
            }
            set
            {
                if (_changeRequest != value)
                {
                    _changeRequest = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_changeRequest != null)
                    {
                        AuthorizingChangeRequest = _changeRequest.ChangeRequestId;
                    }
                }
            }
        }

        private ObservableCollection<RoleAssignment> _roleAssignments;

        [InverseProperty("RoleAssignment")]
        public virtual ObservableCollection<RoleAssignment> RoleAssignments
        {
            get
            {
                if (_roleAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. RoleAssignmentId: " + this.RoleAssignmentId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignments.Where(x => x.SupersedesAssignment == this.RoleAssignmentId).ToList<RoleAssignment>();
                        _roleAssignments = new ObservableCollection<RoleAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roleAssignments.CollectionChanged += RoleAssignments_CollectionChanged;
                }
                return _roleAssignments;
            }
            private set
            {
                if (_roleAssignments != null)
                {
                    _roleAssignments.CollectionChanged -= RoleAssignments_CollectionChanged;
                }
                _roleAssignments = value;
                if (_roleAssignments != null)
                {
                    _roleAssignments.CollectionChanged += RoleAssignments_CollectionChanged;
                }
            }
        }

        private void RoleAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleAssignment>())
                {
                    item.SupersedesAssignment = this.RoleAssignmentId;
                }
            }
        }

        private ObservableCollection<AgentDecisionRecord> _agentDecisionRecords;

        [InverseProperty("RoleAssignment")]
        public virtual ObservableCollection<AgentDecisionRecord> AgentDecisionRecords
        {
            get
            {
                if (_agentDecisionRecords == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentDecisionRecords - no database context is set. RoleAssignmentId: " + this.RoleAssignmentId + ".");
                        }
                        _agentDecisionRecords = new ObservableCollection<AgentDecisionRecord>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentDecisionRecords.Where(x => x.UnderRoleAssignment == this.RoleAssignmentId).ToList<AgentDecisionRecord>();
                        _agentDecisionRecords = new ObservableCollection<AgentDecisionRecord>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _agentDecisionRecords.CollectionChanged += AgentDecisionRecords_CollectionChanged;
                }
                return _agentDecisionRecords;
            }
            private set
            {
                if (_agentDecisionRecords != null)
                {
                    _agentDecisionRecords.CollectionChanged -= AgentDecisionRecords_CollectionChanged;
                }
                _agentDecisionRecords = value;
                if (_agentDecisionRecords != null)
                {
                    _agentDecisionRecords.CollectionChanged += AgentDecisionRecords_CollectionChanged;
                }
            }
        }

        private void AgentDecisionRecords_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentDecisionRecord>())
                {
                    item.UnderRoleAssignment = this.RoleAssignmentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RoleRef;
            _ = this.AgentRef;
            _ = this.EvaluationContextRef;
            _ = this.RoleAssignment;
            _ = this.RoleRefRef;
            _ = this.ChangeRequest;
            _ = this.RoleAssignments;
            _ = this.AgentDecisionRecords;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
