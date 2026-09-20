
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
    [Table("KnowledgeFragments")]
    public class KnowledgeFragmentBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeFragmentId { get; set; }

        // Formula Name (rulebook: ={{KnowledgeForm}} & ": " & LEFT({{Statement}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.KnowledgeForm)), F.S(": "), F.Text(F.Left(F.Of(this.Statement), F.I(60)))))); set { }
        }

        public string? KnowledgeForm { get; set; }
        public string? Statement { get; set; }
        public string? Confidence { get; set; }
        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        public string? Status { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula IsCurrentlyValid (rulebook: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}}), {{Status}} = "Approved"))
        [NotMapped]
        public bool? IsCurrentlyValid
        {
            get => F.AsBool(F.Memo(this, "IsCurrentlyValid", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidFrom)), "<=", F.Of(this.AsOfInstant))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ValidTo))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidTo)), ">", F.Of(this.AsOfInstant))))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved")))))); set { }
        }

        // Formula SourceAgentIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{SourceAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public bool? SourceAgentIsStillEngaged
        {
            get => F.AsBool(F.Memo(this, "SourceAgentIsStillEngaged", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.SourceAgent), __r => F.Of(__r.IsStillEngaged), () => F.Of(new Agent().IsStillEngaged)))); set { }
        }

        // Formula SourceAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{SourceAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? SourceAgentKind
        {
            get => F.AsString(F.Memo(this, "SourceAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.SourceAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula HasHumanSource (rulebook: ={{SourceAgentKind}} = "Human")
        [NotMapped]
        public bool? HasHumanSource
        {
            get => F.AsBool(F.Memo(this, "HasHumanSource", () => F.Eq(F.Of(this.SourceAgentKind), F.S("Human")))); set { }
        }

        // Formula HasOrphanedProvenance (rulebook: =AND({{IsCurrentlyValid}}, NOT({{SourceAgentIsStillEngaged}})))
        [NotMapped]
        public bool? HasOrphanedProvenance
        {
            get => F.AsBool(F.Memo(this, "HasOrphanedProvenance", () => F.And(F.Bool3(F.Of(this.IsCurrentlyValid)), F.Bool3(F.Not(F.Bool3(F.Of(this.SourceAgentIsStillEngaged))))))); set { }
        }

        // Formula IsUndefendableTacitClaim (rulebook: =AND({{HasOrphanedProvenance}}, OR({{KnowledgeForm}} = "Tacit", {{KnowledgeForm}} = "SituatedJudgment")))
        [NotMapped]
        public bool? IsUndefendableTacitClaim
        {
            get => F.AsBool(F.Memo(this, "IsUndefendableTacitClaim", () => F.And(F.Bool3(F.Of(this.HasOrphanedProvenance)), F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.KnowledgeForm)), F.S("Tacit"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.KnowledgeForm)), F.S("SituatedJudgment")))))))); set { }
        }

        // Formula IsApproved (rulebook: ={{Status}} = "Approved")
        [NotMapped]
        public bool? IsApproved
        {
            get => F.AsBool(F.Memo(this, "IsApproved", () => F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved")))); set { }
        }

        // Formula IsWithinValidityWindow (rulebook: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        [NotMapped]
        public bool? IsWithinValidityWindow
        {
            get => F.AsBool(F.Memo(this, "IsWithinValidityWindow", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidFrom)), "<=", F.Of(this.AsOfInstant))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ValidTo))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidTo)), ">", F.Of(this.AsOfInstant)))))))); set { }
        }

        // Formula IsReliedUpon (rulebook: =AND({{Step}} <> "", {{IsWithinValidityWindow}}))
        [NotMapped]
        public bool? IsReliedUpon
        {
            get => F.AsBool(F.Memo(this, "IsReliedUpon", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.Step))), F.Bool3(F.Of(this.IsWithinValidityWindow))))); set { }
        }

        // Formula StepProcedureVersionStatus (rulebook: =INDEX(Steps!{{ProcedureVersion}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? StepProcedureVersionStatus
        {
            get => F.AsString(F.Memo(this, "StepProcedureVersionStatus", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.ProcedureVersion), () => F.Of(new Step().ProcedureVersion)))); set { }
        }

        // Formula IsAttachedToLiveVersion (rulebook: =INDEX(ProcedureVersions!{{IsLive}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public bool? IsAttachedToLiveVersion
        {
            get => F.AsBool(F.Memo(this, "IsAttachedToLiveVersion", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.IsLive), () => F.Of(new ProcedureVersion().IsLive)))); set { }
        }

        // Formula IsUnapprovedButReliedOn (rulebook: =AND({{IsReliedUpon}}, {{IsAttachedToLiveVersion}}, NOT({{IsApproved}})))
        [NotMapped]
        public bool? IsUnapprovedButReliedOn
        {
            get => F.AsBool(F.Memo(this, "IsUnapprovedButReliedOn", () => F.And(F.Bool3(F.Of(this.IsReliedUpon)), F.Bool3(F.Of(this.IsAttachedToLiveVersion)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsApproved))))))); set { }
        }

        // Formula EvidenceAgeDays (rulebook: =INDEX(ElicitationSessions!{{DaysSinceElicited}}, MATCH({{ElicitationSession}}, ElicitationSessions!{{ElicitationSessionId}}, 0)))
        [NotMapped]
        public int? EvidenceAgeDays
        {
            get => F.AsInt(F.Memo(this, "EvidenceAgeDays", () => F.Integer(F.Lookup<ElicitationSession>(this, "ElicitationSessions", "ElicitationSessionId", __c => __c.ElicitationSessions, __r => F.Of(__r.ElicitationSessionId), F.Of(this.ElicitationSession), __r => F.Of(__r.DaysSinceElicited), () => F.Of(new ElicitationSession().DaysSinceElicited))))); set { }
        }

        // Formula HasRecordedElicitation (rulebook: ={{ElicitationSession}} <> "")
        [NotMapped]
        public bool? HasRecordedElicitation
        {
            get => F.AsBool(F.Memo(this, "HasRecordedElicitation", () => F.IsNotBlank(F.Of(this.ElicitationSession)))); set { }
        }

        // Formula IsFromSingleWitness (rulebook: =INDEX(ElicitationSessions!{{IsSingleWitnessMethod}}, MATCH({{ElicitationSession}}, ElicitationSessions!{{ElicitationSessionId}}, 0)))
        [NotMapped]
        public bool? IsFromSingleWitness
        {
            get => F.AsBool(F.Memo(this, "IsFromSingleWitness", () => F.Lookup<ElicitationSession>(this, "ElicitationSessions", "ElicitationSessionId", __c => __c.ElicitationSessions, __r => F.Of(__r.ElicitationSessionId), F.Of(this.ElicitationSession), __r => F.Of(__r.IsSingleWitnessMethod), () => F.Of(new ElicitationSession().IsSingleWitnessMethod)))); set { }
        }

        // Formula EvidenceExpiryDays (rulebook: =IF({{IsFromSingleWitness}}, 180, 365))
        [NotMapped]
        public int? EvidenceExpiryDays
        {
            get => F.AsInt(F.Memo(this, "EvidenceExpiryDays", () => F.Integer((F.Truthy(F.Bool3(F.Of(this.IsFromSingleWitness))) ? F.I(180) : F.I(365))))); set { }
        }

        // Formula EvidenceHasExpired (rulebook: =AND({{HasRecordedElicitation}}, {{EvidenceAgeDays}} > {{EvidenceExpiryDays}}))
        [NotMapped]
        public bool? EvidenceHasExpired
        {
            get => F.AsBool(F.Memo(this, "EvidenceHasExpired", () => F.And(F.Bool3(F.Of(this.HasRecordedElicitation)), F.Bool3(F.Cmp(F.Of(this.EvidenceAgeDays), ">", F.Of(this.EvidenceExpiryDays)))))); set { }
        }

        // Formula OwnerAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? OwnerAgent
        {
            get => F.AsString(F.Memo(this, "OwnerAgent", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.OwnerRole), __r => F.Of(__r.CurrentAgent), () => F.Of(new Role().CurrentAgent)))); set { }
        }

        // Formula IsAwaitingApproval (rulebook: ={{Status}} = "Reviewed")
        [NotMapped]
        public bool? IsAwaitingApproval
        {
            get => F.AsBool(F.Memo(this, "IsAwaitingApproval", () => F.Eq(F.Nullif(F.Of(this.Status)), F.S("Reviewed")))); set { }
        }

        // Formula OwnerIsMe (rulebook: ={{OwnerRole}} = "hr-policy-owner")
        [NotMapped]
        public bool? OwnerIsMe
        {
            get => F.AsBool(F.Memo(this, "OwnerIsMe", () => F.Eq(F.Nullif(F.Of(this.OwnerRole)), F.S("hr-policy-owner")))); set { }
        }

        // Formula IsMyUnfinishedApproval (rulebook: =AND({{OwnerIsMe}}, {{IsAwaitingApproval}}))
        [NotMapped]
        public bool? IsMyUnfinishedApproval
        {
            get => F.AsBool(F.Memo(this, "IsMyUnfinishedApproval", () => F.And(F.Bool3(F.Of(this.OwnerIsMe)), F.Bool3(F.Of(this.IsAwaitingApproval))))); set { }
        }

        // Formula IsInvokedByAnException (rulebook: =COUNTIFS(Exceptions!{{TriggerStep}}, KnowledgeFragments!{{Step}}))
        [NotMapped]
        public int? IsInvokedByAnException
        {
            get => F.AsInt(F.Memo(this, "IsInvokedByAnException", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Exception>(base.SoAContext, "Exceptions", __c => __c.Exceptions), __r => F.CritField(F.Of(__r.TriggerStep), F.Of(this.Step))))))); set { }
        }

        // Formula HasOperationalReliance (rulebook: ={{IsInvokedByAnException}} > 0)
        [NotMapped]
        public bool? HasOperationalReliance
        {
            get => F.AsBool(F.Memo(this, "HasOperationalReliance", () => F.Cmp(F.Of(this.IsInvokedByAnException), ">", F.I(0)))); set { }
        }

        // Formula IsUnapprovedAndOperationallyLive (rulebook: =AND({{IsMyUnfinishedApproval}}, {{HasOperationalReliance}}))
        [NotMapped]
        public bool? IsUnapprovedAndOperationallyLive
        {
            get => F.AsBool(F.Memo(this, "IsUnapprovedAndOperationallyLive", () => F.And(F.Bool3(F.Of(this.IsMyUnfinishedApproval)), F.Bool3(F.Of(this.HasOperationalReliance))))); set { }
        }

        // Formula AgeDays (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days"))
        [NotMapped]
        public int? AgeDays
        {
            get => F.AsInt(F.Memo(this, "AgeDays", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.ValidFrom), F.S("days"))))); set { }
        }

        // Formula IsLowConfidence (rulebook: =OR({{Confidence}} = "Medium", {{Confidence}} = "Low"))
        [NotMapped]
        public bool? IsLowConfidence
        {
            get => F.AsBool(F.Memo(this, "IsLowConfidence", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Confidence)), F.S("Medium"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Confidence)), F.S("Low")))))); set { }
        }

        // Formula OwningVersionCadenceDays (rulebook: =INDEX(ProcedureVersions!{{StewardReviewCadenceDays}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public int? OwningVersionCadenceDays
        {
            get => F.AsInt(F.Memo(this, "OwningVersionCadenceDays", () => F.Integer(F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.StewardReviewCadenceDays), () => F.Of(new ProcedureVersion().StewardReviewCadenceDays))))); set { }
        }

        // Formula ExceedsOwningCadence (rulebook: ={{AgeDays}} > {{OwningVersionCadenceDays}})
        [NotMapped]
        public bool? ExceedsOwningCadence
        {
            get => F.AsBool(F.Memo(this, "ExceedsOwningCadence", () => F.Cmp(F.Of(this.AgeDays), ">", F.Of(this.OwningVersionCadenceDays)))); set { }
        }

        // Formula IsAgingLowConfidenceClaim (rulebook: =AND({{ExceedsOwningCadence}}, {{IsLowConfidence}}))
        [NotMapped]
        public bool? IsAgingLowConfidenceClaim
        {
            get => F.AsBool(F.Memo(this, "IsAgingLowConfidenceClaim", () => F.And(F.Bool3(F.Of(this.ExceedsOwningCadence)), F.Bool3(F.Of(this.IsLowConfidence))))); set { }
        }

        // Formula OwnerRoleAgentKind (rulebook: =INDEX(Roles!{{CurrentAgentKind}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? OwnerRoleAgentKind
        {
            get => F.AsString(F.Memo(this, "OwnerRoleAgentKind", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.OwnerRole), __r => F.Of(__r.CurrentAgentKind), () => F.Of(new Role().CurrentAgentKind)))); set { }
        }

        // Formula IsHumanOwned (rulebook: ={{OwnerRoleAgentKind}} = "Human")
        [NotMapped]
        public bool? IsHumanOwned
        {
            get => F.AsBool(F.Memo(this, "IsHumanOwned", () => F.Eq(F.Of(this.OwnerRoleAgentKind), F.S("Human")))); set { }
        }

        // Formula IsAiValidatedByAi (rulebook: =AND(NOT({{SourceAgentKind}} = "Human"), NOT({{IsHumanOwned}})))
        [NotMapped]
        public bool? IsAiValidatedByAi
        {
            get => F.AsBool(F.Memo(this, "IsAiValidatedByAi", () => F.And(F.Bool3(F.Not(F.Bool3(F.Eq(F.Of(this.SourceAgentKind), F.S("Human"))))), F.Bool3(F.Not(F.Bool3(F.Of(this.IsHumanOwned))))))); set { }
        }

        // Formula ReviewCadenceDays (rulebook: =INDEX(ProcedureVersions!{{StewardReviewCadenceDays}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public int? ReviewCadenceDays
        {
            get => F.AsInt(F.Memo(this, "ReviewCadenceDays", () => F.Integer(F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.StewardReviewCadenceDays), () => F.Of(new ProcedureVersion().StewardReviewCadenceDays))))); set { }
        }

        // Formula IsOverdueForReview (rulebook: =AND({{IsCurrentlyValid}}, {{AgeDays}} > {{ReviewCadenceDays}}))
        [NotMapped]
        public bool? IsOverdueForReview
        {
            get => F.AsBool(F.Memo(this, "IsOverdueForReview", () => F.And(F.Bool3(F.Of(this.IsCurrentlyValid)), F.Bool3(F.Cmp(F.Of(this.AgeDays), ">", F.Of(this.ReviewCadenceDays)))))); set { }
        }

        // Formula PredatesCurrentRoleHolder (rulebook: =AND({{OwnerRoleAgentKind}} <> "", {{ValidFrom}} < {{OwnerRoleAssignmentValidFrom}}))
        [NotMapped]
        public bool? PredatesCurrentRoleHolder
        {
            get => F.AsBool(F.Memo(this, "PredatesCurrentRoleHolder", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.OwnerRoleAgentKind))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidFrom)), "<", F.Of(this.OwnerRoleAssignmentValidFrom)))))); set { }
        }

        // Formula OwnerRoleAssignmentValidFrom (rulebook: =INDEX(Roles!{{CurrentAssignmentValidFrom}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public DateTimeOffset? OwnerRoleAssignmentValidFrom
        {
            get => F.AsDateTime(F.Memo(this, "OwnerRoleAssignmentValidFrom", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.OwnerRole), __r => F.Of(__r.CurrentAssignmentValidFrom), () => F.Of(new Role().CurrentAssignmentValidFrom)))); set { }
        }

        public DateTimeOffset? LastReviewedAt { get; set; }
        // Formula FragilitySignalCount (rulebook: =IF({{IsFromSingleWitness}}, 1, 0) + IF({{IsOverdueForReview}}, 1, 0) + IF({{IsLowConfidence}}, 1, 0) + IF({{HasOperationalReliance}}, 1, 0))
        [NotMapped]
        public int? FragilitySignalCount
        {
            get => F.AsInt(F.Memo(this, "FragilitySignalCount", () => F.Integer(F.Add(F.Add(F.Add((F.Truthy(F.Bool3(F.Of(this.IsFromSingleWitness))) ? F.I(1) : F.I(0)), (F.Truthy(F.Bool3(F.Of(this.IsOverdueForReview))) ? F.I(1) : F.I(0))), (F.Truthy(F.Bool3(F.Of(this.IsLowConfidence))) ? F.I(1) : F.I(0))), (F.Truthy(F.Bool3(F.Of(this.HasOperationalReliance))) ? F.I(1) : F.I(0)))))); set { }
        }

        // Formula IsCompoundFragile (rulebook: ={{FragilitySignalCount}} >= 3)
        [NotMapped]
        public bool? IsCompoundFragile
        {
            get => F.AsBool(F.Memo(this, "IsCompoundFragile", () => F.Cmp(F.Of(this.FragilitySignalCount), ">=", F.I(3)))); set { }
        }

        // Formula IsSinglePointOfFailure (rulebook: =AND({{IsFromSingleWitness}}, {{HasOperationalReliance}}))
        [NotMapped]
        public bool? IsSinglePointOfFailure
        {
            get => F.AsBool(F.Memo(this, "IsSinglePointOfFailure", () => F.And(F.Bool3(F.Of(this.IsFromSingleWitness)), F.Bool3(F.Of(this.HasOperationalReliance))))); set { }
        }

        // Formula IsExpiringSinglePointOfFailure (rulebook: =AND({{IsSinglePointOfFailure}}, {{IsOverdueForReview}}))
        [NotMapped]
        public bool? IsExpiringSinglePointOfFailure
        {
            get => F.AsBool(F.Memo(this, "IsExpiringSinglePointOfFailure", () => F.And(F.Bool3(F.Of(this.IsSinglePointOfFailure)), F.Bool3(F.Of(this.IsOverdueForReview))))); set { }
        }

        // Formula CompoundFragileVersionKey (rulebook: =IF({{IsCompoundFragile}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? CompoundFragileVersionKey
        {
            get => F.AsString(F.Memo(this, "CompoundFragileVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsCompoundFragile))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula ValidFragmentSessionKey (rulebook: =IF({{IsCurrentlyValid}}, {{ElicitationSession}}, ""))
        [NotMapped]
        public string? ValidFragmentSessionKey
        {
            get => F.AsString(F.Memo(this, "ValidFragmentSessionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsCurrentlyValid))) ? F.Of(this.ElicitationSession) : F.S("")))); set { }
        }

        // Formula ConsumingStepIsSoftwareAssigned (rulebook: =INDEX(Steps!{{IsSoftwareAssigned}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? ConsumingStepIsSoftwareAssigned
        {
            get => F.AsBool(F.Memo(this, "ConsumingStepIsSoftwareAssigned", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.IsSoftwareAssigned), () => F.Of(new Step().IsSoftwareAssigned)))); set { }
        }

        // Formula ConsumingStepAgentKind (rulebook: =INDEX(Steps!{{AssignedAgentKind}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? ConsumingStepAgentKind
        {
            get => F.AsString(F.Memo(this, "ConsumingStepAgentKind", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.AssignedAgentKind), () => F.Of(new Step().AssignedAgentKind)))); set { }
        }

        // Formula IsUnapprovedAndMachineConsumed (rulebook: =AND({{IsUnapprovedButReliedOn}}, {{ConsumingStepIsSoftwareAssigned}}))
        [NotMapped]
        public bool? IsUnapprovedAndMachineConsumed
        {
            get => F.AsBool(F.Memo(this, "IsUnapprovedAndMachineConsumed", () => F.And(F.Bool3(F.Of(this.IsUnapprovedButReliedOn)), F.Bool3(F.Of(this.ConsumingStepIsSoftwareAssigned))))); set { }
        }

        // Formula IsUnapprovedAndHumanConsumed (rulebook: =AND({{IsUnapprovedButReliedOn}}, NOT({{ConsumingStepIsSoftwareAssigned}})))
        [NotMapped]
        public bool? IsUnapprovedAndHumanConsumed
        {
            get => F.AsBool(F.Memo(this, "IsUnapprovedAndHumanConsumed", () => F.And(F.Bool3(F.Of(this.IsUnapprovedButReliedOn)), F.Bool3(F.Not(F.Bool3(F.Of(this.ConsumingStepIsSoftwareAssigned))))))); set { }
        }

        // Formula MachineConsumedUnapprovedVersionKey (rulebook: =IF({{IsUnapprovedAndMachineConsumed}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? MachineConsumedUnapprovedVersionKey
        {
            get => F.AsString(F.Memo(this, "MachineConsumedUnapprovedVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnapprovedAndMachineConsumed))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula HasReviewRecord (rulebook: ={{LastReviewedAt}} <> "")
        [NotMapped]
        public bool? HasReviewRecord
        {
            get => F.AsBool(F.Memo(this, "HasReviewRecord", () => F.IsNotBlank(F.Of(this.LastReviewedAt)))); set { }
        }

        // Formula DaysSinceActualReview (rulebook: =IF({{HasReviewRecord}}, DATETIME_DIFF({{AsOfInstant}}, {{LastReviewedAt}}, "days"), 0))
        [NotMapped]
        public int? DaysSinceActualReview
        {
            get => F.AsInt(F.Memo(this, "DaysSinceActualReview", () => F.Integer((F.Truthy(F.Bool3(F.Of(this.HasReviewRecord))) ? F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.LastReviewedAt), F.S("days")) : F.I(0))))); set { }
        }

        // Formula IsUnreviewedSinceAuthoring (rulebook: =AND({{IsCurrentlyValid}}, NOT({{HasReviewRecord}})))
        [NotMapped]
        public bool? IsUnreviewedSinceAuthoring
        {
            get => F.AsBool(F.Memo(this, "IsUnreviewedSinceAuthoring", () => F.And(F.Bool3(F.Of(this.IsCurrentlyValid)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasReviewRecord))))))); set { }
        }

        // Formula IsGenuinelyOverdue (rulebook: =AND({{IsCurrentlyValid}}, {{HasReviewRecord}}, {{DaysSinceActualReview}} > {{ReviewCadenceDays}}))
        [NotMapped]
        public bool? IsGenuinelyOverdue
        {
            get => F.AsBool(F.Memo(this, "IsGenuinelyOverdue", () => F.And(F.Bool3(F.Of(this.IsCurrentlyValid)), F.Bool3(F.Of(this.HasReviewRecord)), F.Bool3(F.Cmp(F.Of(this.DaysSinceActualReview), ">", F.Of(this.ReviewCadenceDays)))))); set { }
        }

        // Formula ReviewRecencyIsInferred (rulebook: =AND({{IsOverdueForReview}}, NOT({{HasReviewRecord}})))
        [NotMapped]
        public bool? ReviewRecencyIsInferred
        {
            get => F.AsBool(F.Memo(this, "ReviewRecencyIsInferred", () => F.And(F.Bool3(F.Of(this.IsOverdueForReview)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasReviewRecord))))))); set { }
        }

        // Formula InferenceDisagreesWithRecord (rulebook: =AND({{HasReviewRecord}}, {{IsOverdueForReview}}, NOT({{IsGenuinelyOverdue}})))
        [NotMapped]
        public bool? InferenceDisagreesWithRecord
        {
            get => F.AsBool(F.Memo(this, "InferenceDisagreesWithRecord", () => F.And(F.Bool3(F.Of(this.HasReviewRecord)), F.Bool3(F.Of(this.IsOverdueForReview)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsGenuinelyOverdue))))))); set { }
        }

        // Formula GenuinelyOverdueVersionKey (rulebook: =IF({{IsGenuinelyOverdue}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? GenuinelyOverdueVersionKey
        {
            get => F.AsString(F.Memo(this, "GenuinelyOverdueVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsGenuinelyOverdue))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula RatifiedBoundaryCount (rulebook: =COUNTIFS(AuthorityBoundaries!{{RatifyingFragmentKey}}, {{KnowledgeFragmentId}}))
        [NotMapped]
        public decimal? RatifiedBoundaryCount
        {
            get => F.AsDecimal(F.Memo(this, "RatifiedBoundaryCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AuthorityBoundary>(base.SoAContext, "AuthorityBoundaries", __c => __c.AuthorityBoundaries), __r => F.CritField(F.Of(__r.RatifyingFragmentKey), F.Of(this.KnowledgeFragmentId)))))); set { }
        }

        // Formula RelianceSurfaceCount (rulebook: ={{IsInvokedByAnException}} + {{RatifiedBoundaryCount}})
        [NotMapped]
        public int? RelianceSurfaceCount
        {
            get => F.AsInt(F.Memo(this, "RelianceSurfaceCount", () => F.Integer(F.Add(F.Of(this.IsInvokedByAnException), F.Of(this.RatifiedBoundaryCount))))); set { }
        }

        // Formula DaysAwaitingMyApproval (rulebook: =IF({{IsMyUnfinishedApproval}}, DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days"), 0))
        [NotMapped]
        public int? DaysAwaitingMyApproval
        {
            get => F.AsInt(F.Memo(this, "DaysAwaitingMyApproval", () => F.Integer((F.Truthy(F.Bool3(F.Of(this.IsMyUnfinishedApproval))) ? F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.ValidFrom), F.S("days")) : F.I(0))))); set { }
        }

        // Formula IsHighBlastRadiusUnapproved (rulebook: =AND({{IsUnapprovedAndOperationallyLive}}, {{RelianceSurfaceCount}} > 1))
        [NotMapped]
        public bool? IsHighBlastRadiusUnapproved
        {
            get => F.AsBool(F.Memo(this, "IsHighBlastRadiusUnapproved", () => F.And(F.Bool3(F.Of(this.IsUnapprovedAndOperationallyLive)), F.Bool3(F.Cmp(F.Of(this.RelianceSurfaceCount), ">", F.I(1)))))); set { }
        }

        // Formula IsLongUnapproved (rulebook: =AND({{IsMyUnfinishedApproval}}, {{DaysAwaitingMyApproval}} > 30))
        [NotMapped]
        public bool? IsLongUnapproved
        {
            get => F.AsBool(F.Memo(this, "IsLongUnapproved", () => F.And(F.Bool3(F.Of(this.IsMyUnfinishedApproval)), F.Bool3(F.Cmp(F.Of(this.DaysAwaitingMyApproval), ">", F.I(30)))))); set { }
        }

        // Formula UnapprovedLoadBearingVersionKey (rulebook: =IF({{IsHighBlastRadiusUnapproved}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? UnapprovedLoadBearingVersionKey
        {
            get => F.AsString(F.Memo(this, "UnapprovedLoadBearingVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsHighBlastRadiusUnapproved))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula OwnerRoleIsVacated (rulebook: =INDEX(Roles!{{IsVacatedRole}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public bool? OwnerRoleIsVacated
        {
            get => F.AsBool(F.Memo(this, "OwnerRoleIsVacated", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.OwnerRole), __r => F.Of(__r.IsVacatedRole), () => F.Of(new Role().IsVacatedRole)))); set { }
        }

        // Formula IsOrphanedByRole (rulebook: =AND({{IsCurrentlyValid}}, {{OwnerRoleIsVacated}}))
        [NotMapped]
        public bool? IsOrphanedByRole
        {
            get => F.AsBool(F.Memo(this, "IsOrphanedByRole", () => F.And(F.Bool3(F.Of(this.IsCurrentlyValid)), F.Bool3(F.Of(this.OwnerRoleIsVacated))))); set { }
        }

        // Formula ValidFragmentVersionKey (rulebook: =IF({{IsCurrentlyValid}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? ValidFragmentVersionKey
        {
            get => F.AsString(F.Memo(this, "ValidFragmentVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsCurrentlyValid))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        public string? CognitiveBasis { get; set; }
        public int? TacitnessDegree { get; set; }
        public string? LostInTranslation { get; set; }
        public string? EncodedAs { get; set; }
        public string? StatedConditions { get; set; }
        // Formula IsFlattenedToBrittleRule (rulebook: =AND(OR({{KnowledgeForm}} = "Tacit", {{KnowledgeForm}} = "SituatedJudgment"), {{EncodedAs}} = "HardRule", {{StatedConditions}} = ""))
        [NotMapped]
        public bool? IsFlattenedToBrittleRule
        {
            get => F.AsBool(F.Memo(this, "IsFlattenedToBrittleRule", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.KnowledgeForm)), F.S("Tacit"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.KnowledgeForm)), F.S("SituatedJudgment"))))), F.Bool3(F.Eq(F.Nullif(F.Of(this.EncodedAs)), F.S("HardRule"))), F.Bool3(F.IsBlank(F.Of(this.StatedConditions)))))); set { }
        }

        // Formula CorroborationCount (rulebook: =COUNTIFS(FragmentCorroborations!{{KnowledgeFragment}}, {{KnowledgeFragmentId}}, FragmentCorroborations!{{Agrees}}, TRUE))
        [NotMapped]
        public int? CorroborationCount
        {
            get => F.AsInt(F.Memo(this, "CorroborationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<FragmentCorroboration>(base.SoAContext, "FragmentCorroborations", __c => __c.FragmentCorroborations), __r => F.CritField(F.Of(__r.KnowledgeFragment), F.Of(this.KnowledgeFragmentId)) && F.CritLiteral(F.Of(__r.Agrees), F.B(true))))))); set { }
        }

        // Formula RestsOnSingleDataPoint (rulebook: =AND({{Status}} = "Approved", {{CorroborationCount}} = 0))
        [NotMapped]
        public bool? RestsOnSingleDataPoint
        {
            get => F.AsBool(F.Memo(this, "RestsOnSingleDataPoint", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved"))), F.Bool3(F.Eq(F.Of(this.CorroborationCount), F.I(0)))))); set { }
        }

        // Formula OwnerOrganization (rulebook: =INDEX(ProcedureVersions!{{OwnerOrganization}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public string? OwnerOrganization
        {
            get => F.AsString(F.Memo(this, "OwnerOrganization", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.OwnerOrganization), () => F.Of(new ProcedureVersion().OwnerOrganization)))); set { }
        }


        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? ElicitationSession { get; set; }
        public string? SourceAgent { get; set; }
        public string? OwnerRole { get; set; }
        public string? EvaluationContext { get; set; }

        private ProcedureVersion _procedureVersionRef;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersionRef
        {
            get
            {
                if (_procedureVersionRef == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionRef - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersionRef = base.SoAContext.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersionRef != null)
                    {
                        base.SoAContext.Attach(_procedureVersionRef);
                    }
                }
                return _procedureVersionRef;
            }
            set
            {
                if (_procedureVersionRef != value)
                {
                    _procedureVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersionRef != null)
                    {
                        ProcedureVersion = _procedureVersionRef.ProcedureVersionId;
                    }
                }
            }
        }

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

        private ElicitationSession _elicitationSessionRef;

        [ForeignKey("ElicitationSession")]
        public virtual ElicitationSession ElicitationSessionRef
        {
            get
            {
                if (_elicitationSessionRef == null && !string.IsNullOrEmpty(ElicitationSession))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ElicitationSessionRef - no database context is set. ElicitationSession: " + ElicitationSession + ".");
                        }
                        return null;
                    }
                    _elicitationSessionRef = base.SoAContext.ElicitationSessions.Find(ElicitationSession);
                    if (_elicitationSessionRef != null)
                    {
                        base.SoAContext.Attach(_elicitationSessionRef);
                    }
                }
                return _elicitationSessionRef;
            }
            set
            {
                if (_elicitationSessionRef != value)
                {
                    _elicitationSessionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_elicitationSessionRef != null)
                    {
                        ElicitationSession = _elicitationSessionRef.ElicitationSessionId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("SourceAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(SourceAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. SourceAgent: " + SourceAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(SourceAgent);
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
                        SourceAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("OwnerRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(OwnerRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. OwnerRole: " + OwnerRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(OwnerRole);
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
                        OwnerRole = _role.RoleId;
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

        private ObservableCollection<KnowledgeGap> _knowledgeGaps;

        [InverseProperty("KnowledgeFragment")]
        public virtual ObservableCollection<KnowledgeGap> KnowledgeGaps
        {
            get
            {
                if (_knowledgeGaps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeGaps - no database context is set. KnowledgeFragmentId: " + this.KnowledgeFragmentId + ".");
                        }
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeGaps.Where(x => x.CodifiedAsFragment == this.KnowledgeFragmentId).ToList<KnowledgeGap>();
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeGaps.CollectionChanged += KnowledgeGaps_CollectionChanged;
                }
                return _knowledgeGaps;
            }
            private set
            {
                if (_knowledgeGaps != null)
                {
                    _knowledgeGaps.CollectionChanged -= KnowledgeGaps_CollectionChanged;
                }
                _knowledgeGaps = value;
                if (_knowledgeGaps != null)
                {
                    _knowledgeGaps.CollectionChanged += KnowledgeGaps_CollectionChanged;
                }
            }
        }

        private void KnowledgeGaps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeGap>())
                {
                    item.CodifiedAsFragment = this.KnowledgeFragmentId;
                }
            }
        }

        private ObservableCollection<AuthorityBoundary> _authorityBoundaries;

        [InverseProperty("KnowledgeFragment")]
        public virtual ObservableCollection<AuthorityBoundary> AuthorityBoundaries
        {
            get
            {
                if (_authorityBoundaries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorityBoundaries - no database context is set. KnowledgeFragmentId: " + this.KnowledgeFragmentId + ".");
                        }
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>();
                    }
                    else
                    {
                        var items = base.SoAContext.AuthorityBoundaries.Where(x => x.RatifiedByKnowledgeFragment == this.KnowledgeFragmentId).ToList<AuthorityBoundary>();
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _authorityBoundaries.CollectionChanged += AuthorityBoundaries_CollectionChanged;
                }
                return _authorityBoundaries;
            }
            private set
            {
                if (_authorityBoundaries != null)
                {
                    _authorityBoundaries.CollectionChanged -= AuthorityBoundaries_CollectionChanged;
                }
                _authorityBoundaries = value;
                if (_authorityBoundaries != null)
                {
                    _authorityBoundaries.CollectionChanged += AuthorityBoundaries_CollectionChanged;
                }
            }
        }

        private void AuthorityBoundaries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AuthorityBoundary>())
                {
                    item.RatifiedByKnowledgeFragment = this.KnowledgeFragmentId;
                }
            }
        }

        private ObservableCollection<ModelAnnotation> _modelAnnotations;

        [InverseProperty("KnowledgeFragment")]
        public virtual ObservableCollection<ModelAnnotation> ModelAnnotations
        {
            get
            {
                if (_modelAnnotations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelAnnotations - no database context is set. KnowledgeFragmentId: " + this.KnowledgeFragmentId + ".");
                        }
                        _modelAnnotations = new ObservableCollection<ModelAnnotation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelAnnotations.Where(x => x.PromotedToFragment == this.KnowledgeFragmentId).ToList<ModelAnnotation>();
                        _modelAnnotations = new ObservableCollection<ModelAnnotation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelAnnotations.CollectionChanged += ModelAnnotations_CollectionChanged;
                }
                return _modelAnnotations;
            }
            private set
            {
                if (_modelAnnotations != null)
                {
                    _modelAnnotations.CollectionChanged -= ModelAnnotations_CollectionChanged;
                }
                _modelAnnotations = value;
                if (_modelAnnotations != null)
                {
                    _modelAnnotations.CollectionChanged += ModelAnnotations_CollectionChanged;
                }
            }
        }

        private void ModelAnnotations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelAnnotation>())
                {
                    item.PromotedToFragment = this.KnowledgeFragmentId;
                }
            }
        }

        private ObservableCollection<CriticalIncident> _criticalIncidents;

        [InverseProperty("KnowledgeFragment")]
        public virtual ObservableCollection<CriticalIncident> CriticalIncidents
        {
            get
            {
                if (_criticalIncidents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CriticalIncidents - no database context is set. KnowledgeFragmentId: " + this.KnowledgeFragmentId + ".");
                        }
                        _criticalIncidents = new ObservableCollection<CriticalIncident>();
                    }
                    else
                    {
                        var items = base.SoAContext.CriticalIncidents.Where(x => x.JudgmentFragment == this.KnowledgeFragmentId).ToList<CriticalIncident>();
                        _criticalIncidents = new ObservableCollection<CriticalIncident>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _criticalIncidents.CollectionChanged += CriticalIncidents_CollectionChanged;
                }
                return _criticalIncidents;
            }
            private set
            {
                if (_criticalIncidents != null)
                {
                    _criticalIncidents.CollectionChanged -= CriticalIncidents_CollectionChanged;
                }
                _criticalIncidents = value;
                if (_criticalIncidents != null)
                {
                    _criticalIncidents.CollectionChanged += CriticalIncidents_CollectionChanged;
                }
            }
        }

        private void CriticalIncidents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CriticalIncident>())
                {
                    item.JudgmentFragment = this.KnowledgeFragmentId;
                }
            }
        }

        private ObservableCollection<ObservedAction> _observedActions;

        [InverseProperty("KnowledgeFragment")]
        public virtual ObservableCollection<ObservedAction> ObservedActions
        {
            get
            {
                if (_observedActions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ObservedActions - no database context is set. KnowledgeFragmentId: " + this.KnowledgeFragmentId + ".");
                        }
                        _observedActions = new ObservableCollection<ObservedAction>();
                    }
                    else
                    {
                        var items = base.SoAContext.ObservedActions.Where(x => x.CapturedAsFragment == this.KnowledgeFragmentId).ToList<ObservedAction>();
                        _observedActions = new ObservableCollection<ObservedAction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _observedActions.CollectionChanged += ObservedActions_CollectionChanged;
                }
                return _observedActions;
            }
            private set
            {
                if (_observedActions != null)
                {
                    _observedActions.CollectionChanged -= ObservedActions_CollectionChanged;
                }
                _observedActions = value;
                if (_observedActions != null)
                {
                    _observedActions.CollectionChanged += ObservedActions_CollectionChanged;
                }
            }
        }

        private void ObservedActions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ObservedAction>())
                {
                    item.CapturedAsFragment = this.KnowledgeFragmentId;
                }
            }
        }

        private ObservableCollection<WorkflowViewDivergence> _workflowViewDivergences;

        [InverseProperty("KnowledgeFragment")]
        public virtual ObservableCollection<WorkflowViewDivergence> WorkflowViewDivergences
        {
            get
            {
                if (_workflowViewDivergences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowViewDivergences - no database context is set. KnowledgeFragmentId: " + this.KnowledgeFragmentId + ".");
                        }
                        _workflowViewDivergences = new ObservableCollection<WorkflowViewDivergence>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowViewDivergences.Where(x => x.ReconciledIntoFragment == this.KnowledgeFragmentId).ToList<WorkflowViewDivergence>();
                        _workflowViewDivergences = new ObservableCollection<WorkflowViewDivergence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _workflowViewDivergences.CollectionChanged += WorkflowViewDivergences_CollectionChanged;
                }
                return _workflowViewDivergences;
            }
            private set
            {
                if (_workflowViewDivergences != null)
                {
                    _workflowViewDivergences.CollectionChanged -= WorkflowViewDivergences_CollectionChanged;
                }
                _workflowViewDivergences = value;
                if (_workflowViewDivergences != null)
                {
                    _workflowViewDivergences.CollectionChanged += WorkflowViewDivergences_CollectionChanged;
                }
            }
        }

        private void WorkflowViewDivergences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowViewDivergence>())
                {
                    item.ReconciledIntoFragment = this.KnowledgeFragmentId;
                }
            }
        }

        private ObservableCollection<KnowledgeConversion> _knowledgeConversions;

        [InverseProperty("KnowledgeFragment")]
        public virtual ObservableCollection<KnowledgeConversion> KnowledgeConversions
        {
            get
            {
                if (_knowledgeConversions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeConversions - no database context is set. KnowledgeFragmentId: " + this.KnowledgeFragmentId + ".");
                        }
                        _knowledgeConversions = new ObservableCollection<KnowledgeConversion>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeConversions.Where(x => x.ResultFragment == this.KnowledgeFragmentId).ToList<KnowledgeConversion>();
                        _knowledgeConversions = new ObservableCollection<KnowledgeConversion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeConversions.CollectionChanged += KnowledgeConversions_CollectionChanged;
                }
                return _knowledgeConversions;
            }
            private set
            {
                if (_knowledgeConversions != null)
                {
                    _knowledgeConversions.CollectionChanged -= KnowledgeConversions_CollectionChanged;
                }
                _knowledgeConversions = value;
                if (_knowledgeConversions != null)
                {
                    _knowledgeConversions.CollectionChanged += KnowledgeConversions_CollectionChanged;
                }
            }
        }

        private void KnowledgeConversions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeConversion>())
                {
                    item.ResultFragment = this.KnowledgeFragmentId;
                }
            }
        }

        private ObservableCollection<KnowledgeHolding> _knowledgeHoldings;

        [InverseProperty("KnowledgeFragment")]
        public virtual ObservableCollection<KnowledgeHolding> KnowledgeHoldings
        {
            get
            {
                if (_knowledgeHoldings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeHoldings - no database context is set. KnowledgeFragmentId: " + this.KnowledgeFragmentId + ".");
                        }
                        _knowledgeHoldings = new ObservableCollection<KnowledgeHolding>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeHoldings.Where(x => x.FormalizedAs == this.KnowledgeFragmentId).ToList<KnowledgeHolding>();
                        _knowledgeHoldings = new ObservableCollection<KnowledgeHolding>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeHoldings.CollectionChanged += KnowledgeHoldings_CollectionChanged;
                }
                return _knowledgeHoldings;
            }
            private set
            {
                if (_knowledgeHoldings != null)
                {
                    _knowledgeHoldings.CollectionChanged -= KnowledgeHoldings_CollectionChanged;
                }
                _knowledgeHoldings = value;
                if (_knowledgeHoldings != null)
                {
                    _knowledgeHoldings.CollectionChanged += KnowledgeHoldings_CollectionChanged;
                }
            }
        }

        private void KnowledgeHoldings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeHolding>())
                {
                    item.FormalizedAs = this.KnowledgeFragmentId;
                }
            }
        }

        private ObservableCollection<FragmentCorroboration> _fragmentCorroborations;

        [InverseProperty("KnowledgeFragmentRef")]
        public virtual ObservableCollection<FragmentCorroboration> FragmentCorroborations
        {
            get
            {
                if (_fragmentCorroborations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FragmentCorroborations - no database context is set. KnowledgeFragmentId: " + this.KnowledgeFragmentId + ".");
                        }
                        _fragmentCorroborations = new ObservableCollection<FragmentCorroboration>();
                    }
                    else
                    {
                        var items = base.SoAContext.FragmentCorroborations.Where(x => x.KnowledgeFragment == this.KnowledgeFragmentId).ToList<FragmentCorroboration>();
                        _fragmentCorroborations = new ObservableCollection<FragmentCorroboration>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fragmentCorroborations.CollectionChanged += FragmentCorroborations_CollectionChanged;
                }
                return _fragmentCorroborations;
            }
            private set
            {
                if (_fragmentCorroborations != null)
                {
                    _fragmentCorroborations.CollectionChanged -= FragmentCorroborations_CollectionChanged;
                }
                _fragmentCorroborations = value;
                if (_fragmentCorroborations != null)
                {
                    _fragmentCorroborations.CollectionChanged += FragmentCorroborations_CollectionChanged;
                }
            }
        }

        private void FragmentCorroborations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FragmentCorroboration>())
                {
                    item.KnowledgeFragment = this.KnowledgeFragmentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.StepRef;
            _ = this.ElicitationSessionRef;
            _ = this.Agent;
            _ = this.Role;
            _ = this.EvaluationContextRef;
            _ = this.KnowledgeGaps;
            _ = this.AuthorityBoundaries;
            _ = this.ModelAnnotations;
            _ = this.CriticalIncidents;
            _ = this.ObservedActions;
            _ = this.WorkflowViewDivergences;
            _ = this.KnowledgeConversions;
            _ = this.KnowledgeHoldings;
            _ = this.FragmentCorroborations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
