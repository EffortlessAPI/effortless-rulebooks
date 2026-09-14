
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
    [Table("ProcedureVersions")]
    public class ProcedureVersionBase : SoAEntityBase
    {
        [Key]
        public string ProcedureVersionId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Title))); set { }
        }

        public string? VersionNumber { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset? IssuedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public string? NewVersionMotivation { get; set; }
        public string? ChangelogDescription { get; set; }
        public bool? IsCurrent { get; set; }
        // Formula CountOfSteps (rulebook: =COUNTIFS(Steps!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}))
        [NotMapped]
        public int? CountOfSteps
        {
            get => F.AsInt(F.Memo(this, "CountOfSteps", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula CountOfOpenKnowledgeGaps (rulebook: =COUNTIFS(KnowledgeGaps!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, KnowledgeGaps!{{Status}}, "Open"))
        [NotMapped]
        public int? CountOfOpenKnowledgeGaps
        {
            get => F.AsInt(F.Memo(this, "CountOfOpenKnowledgeGaps", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeGap>(base.SoAContext, "KnowledgeGaps", __c => __c.KnowledgeGaps), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.Status), F.S("Open"))))))); set { }
        }

        // Formula IsReadyForExecution (rulebook: =AND({{Status}} = "Approved", {{CountOfSteps}} > 0, {{CountOfOpenKnowledgeGaps}} = 0))
        [NotMapped]
        public bool? IsReadyForExecution
        {
            get => F.AsBool(F.Memo(this, "IsReadyForExecution", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved"))), F.Bool3(F.Cmp(F.Of(this.CountOfSteps), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.CountOfOpenKnowledgeGaps), F.I(0)))))); set { }
        }

        // Formula SpecifiedStepCount (rulebook: =COUNTIFS(Steps!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? SpecifiedStepCount
        {
            get => F.AsDecimal(F.Memo(this, "SpecifiedStepCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula OverdueReviewCount (rulebook: =COUNTIFS(ReviewEvents!{{OverdueVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? OverdueReviewCount
        {
            get => F.AsDecimal(F.Memo(this, "OverdueReviewCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ReviewEvent>(base.SoAContext, "ReviewEvents", __c => __c.ReviewEvents), __r => F.CritField(F.Of(__r.OverdueVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula OpenChangeRequestCount (rulebook: =COUNTIFS(ChangeRequests!{{OpenChangeVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? OpenChangeRequestCount
        {
            get => F.AsDecimal(F.Memo(this, "OpenChangeRequestCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeRequest>(base.SoAContext, "ChangeRequests", __c => __c.ChangeRequests), __r => F.CritField(F.Of(__r.OpenChangeVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula OpenHighSeverityGapCount (rulebook: =COUNTIFS(KnowledgeGaps!{{OpenGapVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? OpenHighSeverityGapCount
        {
            get => F.AsDecimal(F.Memo(this, "OpenHighSeverityGapCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeGap>(base.SoAContext, "KnowledgeGaps", __c => __c.KnowledgeGaps), __r => F.CritField(F.Of(__r.OpenGapVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula IsFitToExecute (rulebook: =AND({{Status}} = "Approved", {{OverdueReviewCount}} = 0, {{OpenChangeRequestCount}} = 0, {{OpenHighSeverityGapCount}} = 0))
        [NotMapped]
        public bool? IsFitToExecute
        {
            get => F.AsBool(F.Memo(this, "IsFitToExecute", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved"))), F.Bool3(F.Eq(F.Of(this.OverdueReviewCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.OpenChangeRequestCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.OpenHighSeverityGapCount), F.I(0)))))); set { }
        }

        // Formula StewardReviewCadenceDays (rulebook: =SUMIFS(StewardshipAssignments!{{ReviewCadenceDays}}, StewardshipAssignments!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? StewardReviewCadenceDays
        {
            get => F.AsDecimal(F.Memo(this, "StewardReviewCadenceDays", () => (base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<StewardshipAssignment>(base.SoAContext, "StewardshipAssignments", __c => __c.StewardshipAssignments), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)), __r => F.Of(__r.ReviewCadenceDays), null)))); set { }
        }

        // Formula CountOfStewardshipAssignments (rulebook: =COUNTIFS(StewardshipAssignments!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}))
        [NotMapped]
        public int? CountOfStewardshipAssignments
        {
            get => F.AsInt(F.Memo(this, "CountOfStewardshipAssignments", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StewardshipAssignment>(base.SoAContext, "StewardshipAssignments", __c => __c.StewardshipAssignments), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula HasAnySteward (rulebook: ={{CountOfStewardshipAssignments}} > 0)
        [NotMapped]
        public bool? HasAnySteward
        {
            get => F.AsBool(F.Memo(this, "HasAnySteward", () => F.Cmp(F.Of(this.CountOfStewardshipAssignments), ">", F.I(0)))); set { }
        }

        // Formula IsLive (rulebook: =OR({{Status}} = "Approved", {{Status}} = "Published"))
        [NotMapped]
        public bool? IsLive
        {
            get => F.AsBool(F.Memo(this, "IsLive", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Published")))))); set { }
        }

        // Formula IsUnstewarded (rulebook: =NOT({{HasAnySteward}}))
        [NotMapped]
        public bool? IsUnstewarded
        {
            get => F.AsBool(F.Memo(this, "IsUnstewarded", () => F.Not(F.Bool3(F.Of(this.HasAnySteward))))); set { }
        }

        // Formula IsLiveAndUnstewarded (rulebook: =AND({{IsLive}}, {{IsUnstewarded}}))
        [NotMapped]
        public bool? IsLiveAndUnstewarded
        {
            get => F.AsBool(F.Memo(this, "IsLiveAndUnstewarded", () => F.And(F.Bool3(F.Of(this.IsLive)), F.Bool3(F.Of(this.IsUnstewarded))))); set { }
        }

        // Formula CountOfOpenBlockingGaps (rulebook: =COUNTIFS(KnowledgeGaps!{{IsOpenAndBlocking}}, TRUE))
        [NotMapped]
        public int? CountOfOpenBlockingGaps
        {
            get => F.AsInt(F.Memo(this, "CountOfOpenBlockingGaps", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeGap>(base.SoAContext, "KnowledgeGaps", __c => __c.KnowledgeGaps), __r => F.CritLiteral(F.Of(__r.IsOpenAndBlocking), F.B(true))))))); set { }
        }

        // Formula HasOpenBlockingGap (rulebook: ={{CountOfOpenBlockingGaps}} > 0)
        [NotMapped]
        public bool? HasOpenBlockingGap
        {
            get => F.AsBool(F.Memo(this, "HasOpenBlockingGap", () => F.Cmp(F.Of(this.CountOfOpenBlockingGaps), ">", F.I(0)))); set { }
        }

        // Formula IsLiveWithBlockingGap (rulebook: =AND({{IsLive}}, {{HasOpenBlockingGap}}))
        [NotMapped]
        public bool? IsLiveWithBlockingGap
        {
            get => F.AsBool(F.Memo(this, "IsLiveWithBlockingGap", () => F.And(F.Bool3(F.Of(this.IsLive)), F.Bool3(F.Of(this.HasOpenBlockingGap))))); set { }
        }

        // Formula ShouldNotBeExecutable (rulebook: =AND({{IsReadyForExecution}}, {{HasOpenBlockingGap}}))
        [NotMapped]
        public bool? ShouldNotBeExecutable
        {
            get => F.AsBool(F.Memo(this, "ShouldNotBeExecutable", () => F.And(F.Bool3(F.Of(this.IsReadyForExecution)), F.Bool3(F.Of(this.HasOpenBlockingGap))))); set { }
        }

        // Formula CountOfUnapprovedRelianceFragments (rulebook: =COUNTIFS(KnowledgeFragments!{{IsUnapprovedButReliedOn}}, TRUE))
        [NotMapped]
        public int? CountOfUnapprovedRelianceFragments
        {
            get => F.AsInt(F.Memo(this, "CountOfUnapprovedRelianceFragments", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritLiteral(F.Of(__r.IsUnapprovedButReliedOn), F.B(true))))))); set { }
        }

        // Formula RunsOnUnapprovedKnowledge (rulebook: ={{CountOfUnapprovedRelianceFragments}} > 0)
        [NotMapped]
        public bool? RunsOnUnapprovedKnowledge
        {
            get => F.AsBool(F.Memo(this, "RunsOnUnapprovedKnowledge", () => F.Cmp(F.Of(this.CountOfUnapprovedRelianceFragments), ">", F.I(0)))); set { }
        }

        // Formula CountOfOverdueGaps (rulebook: =COUNTIFS(KnowledgeGaps!{{IsOverdueGap}}, TRUE))
        [NotMapped]
        public int? CountOfOverdueGaps
        {
            get => F.AsInt(F.Memo(this, "CountOfOverdueGaps", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeGap>(base.SoAContext, "KnowledgeGaps", __c => __c.KnowledgeGaps), __r => F.CritLiteral(F.Of(__r.IsOverdueGap), F.B(true))))))); set { }
        }

        // Formula CountOfChangeRequests (rulebook: =COUNTIFS(ChangeRequests!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}))
        [NotMapped]
        public int? CountOfChangeRequests
        {
            get => F.AsInt(F.Memo(this, "CountOfChangeRequests", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeRequest>(base.SoAContext, "ChangeRequests", __c => __c.ChangeRequests), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula CountOfReviewEvents (rulebook: =COUNTIFS(ReviewEvents!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}))
        [NotMapped]
        public int? CountOfReviewEvents
        {
            get => F.AsInt(F.Memo(this, "CountOfReviewEvents", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ReviewEvent>(base.SoAContext, "ReviewEvents", __c => __c.ReviewEvents), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula HasGovernanceRecord (rulebook: =OR({{CountOfChangeRequests}} > 0, {{CountOfReviewEvents}} > 0))
        [NotMapped]
        public bool? HasGovernanceRecord
        {
            get => F.AsBool(F.Memo(this, "HasGovernanceRecord", () => F.Or(F.Bool3(F.Cmp(F.Of(this.CountOfChangeRequests), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.CountOfReviewEvents), ">", F.I(0)))))); set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula DaysSinceModified (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{ModifiedAt}}, "days"))
        [NotMapped]
        public int? DaysSinceModified
        {
            get => F.AsInt(F.Memo(this, "DaysSinceModified", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.ModifiedAt), F.S("days"))))); set { }
        }

        // Formula DaysSinceLastReview (rulebook: =DATETIME_DIFF({{AsOfInstant}}, MAXIFS(ReviewEvents!{{ReviewedAt}}, ReviewEvents!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}), "days"))
        [NotMapped]
        public int? DaysSinceLastReview
        {
            get => F.AsInt(F.Memo(this, "DaysSinceLastReview", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<ReviewEvent>(base.SoAContext, "ReviewEvents", __c => __c.ReviewEvents), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)), __r => F.Of(__r.ReviewedAt))), F.S("days"))))); set { }
        }

        // Formula WasModifiedSinceLastReview (rulebook: ={{DaysSinceModified}} < {{DaysSinceLastReview}})
        [NotMapped]
        public bool? WasModifiedSinceLastReview
        {
            get => F.AsBool(F.Memo(this, "WasModifiedSinceLastReview", () => F.Cmp(F.Of(this.DaysSinceModified), "<", F.Of(this.DaysSinceLastReview)))); set { }
        }

        // Formula ModifierIsAuthority (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{ModifiedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? ModifierIsAuthority
        {
            get => F.AsString(F.Memo(this, "ModifierIsAuthority", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.ModifiedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula HasUnwitnessedChange (rulebook: =AND({{IsLive}}, {{WasModifiedSinceLastReview}}))
        [NotMapped]
        public bool? HasUnwitnessedChange
        {
            get => F.AsBool(F.Memo(this, "HasUnwitnessedChange", () => F.And(F.Bool3(F.Of(this.IsLive)), F.Bool3(F.Of(this.WasModifiedSinceLastReview))))); set { }
        }

        // Formula CountOfStaleFragments (rulebook: =COUNTIFS(KnowledgeFragments!{{ExceedsOwningCadence}}, TRUE))
        [NotMapped]
        public int? CountOfStaleFragments
        {
            get => F.AsInt(F.Memo(this, "CountOfStaleFragments", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritLiteral(F.Of(__r.ExceedsOwningCadence), F.B(true))))))); set { }
        }

        // Formula KnowledgeIsStalerThanCadence (rulebook: ={{CountOfStaleFragments}} > 0)
        [NotMapped]
        public bool? KnowledgeIsStalerThanCadence
        {
            get => F.AsBool(F.Memo(this, "KnowledgeIsStalerThanCadence", () => F.Cmp(F.Of(this.CountOfStaleFragments), ">", F.I(0)))); set { }
        }

        // Formula CompoundFragileFragmentCount (rulebook: =COUNTIFS(KnowledgeFragments!{{CompoundFragileVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? CompoundFragileFragmentCount
        {
            get => F.AsDecimal(F.Memo(this, "CompoundFragileFragmentCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritField(F.Of(__r.CompoundFragileVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula RestsOnCompoundFragileKnowledge (rulebook: =AND({{IsLive}}, {{CompoundFragileFragmentCount}} > 0))
        [NotMapped]
        public bool? RestsOnCompoundFragileKnowledge
        {
            get => F.AsBool(F.Memo(this, "RestsOnCompoundFragileKnowledge", () => F.And(F.Bool3(F.Of(this.IsLive)), F.Bool3(F.Cmp(F.Of(this.CompoundFragileFragmentCount), ">", F.I(0)))))); set { }
        }

        // Formula ConcentratedWitnessSessionCount (rulebook: =COUNTIFS(ElicitationSessions!{{ConcentratedSessionVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? ConcentratedWitnessSessionCount
        {
            get => F.AsDecimal(F.Memo(this, "ConcentratedWitnessSessionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationSession>(base.SoAContext, "ElicitationSessions", __c => __c.ElicitationSessions), __r => F.CritField(F.Of(__r.ConcentratedSessionVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula KnowledgeBaseIsConcentrated (rulebook: =AND({{IsLive}}, {{ConcentratedWitnessSessionCount}} > 0))
        [NotMapped]
        public bool? KnowledgeBaseIsConcentrated
        {
            get => F.AsBool(F.Memo(this, "KnowledgeBaseIsConcentrated", () => F.And(F.Bool3(F.Of(this.IsLive)), F.Bool3(F.Cmp(F.Of(this.ConcentratedWitnessSessionCount), ">", F.I(0)))))); set { }
        }

        // Formula MachineConsumedUnapprovedCount (rulebook: =COUNTIFS(KnowledgeFragments!{{MachineConsumedUnapprovedVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? MachineConsumedUnapprovedCount
        {
            get => F.AsDecimal(F.Memo(this, "MachineConsumedUnapprovedCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritField(F.Of(__r.MachineConsumedUnapprovedVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula FeedsUnapprovedKnowledgeToMachines (rulebook: =AND({{IsLive}}, {{MachineConsumedUnapprovedCount}} > 0))
        [NotMapped]
        public bool? FeedsUnapprovedKnowledgeToMachines
        {
            get => F.AsBool(F.Memo(this, "FeedsUnapprovedKnowledgeToMachines", () => F.And(F.Bool3(F.Of(this.IsLive)), F.Bool3(F.Cmp(F.Of(this.MachineConsumedUnapprovedCount), ">", F.I(0)))))); set { }
        }

        // Formula GenuinelyOverdueFragmentCount (rulebook: =COUNTIFS(KnowledgeFragments!{{GenuinelyOverdueVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? GenuinelyOverdueFragmentCount
        {
            get => F.AsDecimal(F.Memo(this, "GenuinelyOverdueFragmentCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritField(F.Of(__r.GenuinelyOverdueVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula AwaitedDecisionCount (rulebook: =COUNTIFS(ChangeRequests!{{BacklogVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? AwaitedDecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "AwaitedDecisionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeRequest>(base.SoAContext, "ChangeRequests", __c => __c.ChangeRequests), __r => F.CritField(F.Of(__r.BacklogVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula ScopedOpenBlockingGapCount (rulebook: =COUNTIFS(KnowledgeGaps!{{OpenBlockingGapVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? ScopedOpenBlockingGapCount
        {
            get => F.AsDecimal(F.Memo(this, "ScopedOpenBlockingGapCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeGap>(base.SoAContext, "KnowledgeGaps", __c => __c.KnowledgeGaps), __r => F.CritField(F.Of(__r.OpenBlockingGapVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula IsBlockedOnPendingDecision (rulebook: =AND({{AwaitedDecisionCount}} > 0, {{ScopedOpenBlockingGapCount}} > 0))
        [NotMapped]
        public bool? IsBlockedOnPendingDecision
        {
            get => F.AsBool(F.Memo(this, "IsBlockedOnPendingDecision", () => F.And(F.Bool3(F.Cmp(F.Of(this.AwaitedDecisionCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.ScopedOpenBlockingGapCount), ">", F.I(0)))))); set { }
        }

        // Formula UnexercisedHumanGateCount (rulebook: =COUNTIFS(Steps!{{UnexercisedGateVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? UnexercisedHumanGateCount
        {
            get => F.AsDecimal(F.Memo(this, "UnexercisedHumanGateCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.UnexercisedGateVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula AiBoundaryIsUnevidenced (rulebook: =AND({{IsLive}}, {{UnexercisedHumanGateCount}} > 0))
        [NotMapped]
        public bool? AiBoundaryIsUnevidenced
        {
            get => F.AsBool(F.Memo(this, "AiBoundaryIsUnevidenced", () => F.And(F.Bool3(F.Of(this.IsLive)), F.Bool3(F.Cmp(F.Of(this.UnexercisedHumanGateCount), ">", F.I(0)))))); set { }
        }

        // Formula LoadBearingUnapprovedCount (rulebook: =COUNTIFS(KnowledgeFragments!{{UnapprovedLoadBearingVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? LoadBearingUnapprovedCount
        {
            get => F.AsDecimal(F.Memo(this, "LoadBearingUnapprovedCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritField(F.Of(__r.UnapprovedLoadBearingVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula UnlandedDecisionCount (rulebook: =COUNTIFS(ChangeRequests!{{UnlandedVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? UnlandedDecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "UnlandedDecisionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeRequest>(base.SoAContext, "ChangeRequests", __c => __c.ChangeRequests), __r => F.CritField(F.Of(__r.UnlandedVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula UnrehearsedControlEntryCount (rulebook: =COUNTIFS(StepTransitions!{{UnrehearsedControlVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? UnrehearsedControlEntryCount
        {
            get => F.AsDecimal(F.Memo(this, "UnrehearsedControlEntryCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepTransition>(base.SoAContext, "StepTransitions", __c => __c.StepTransitions), __r => F.CritField(F.Of(__r.UnrehearsedControlVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula HasUnrehearsedControlEntry (rulebook: ={{UnrehearsedControlEntryCount}} > 0)
        [NotMapped]
        public bool? HasUnrehearsedControlEntry
        {
            get => F.AsBool(F.Memo(this, "HasUnrehearsedControlEntry", () => F.Cmp(F.Of(this.UnrehearsedControlEntryCount), ">", F.I(0)))); set { }
        }

        // Formula IsLiveWithUnrehearsedControl (rulebook: =AND({{IsLive}}, {{HasUnrehearsedControlEntry}}))
        [NotMapped]
        public bool? IsLiveWithUnrehearsedControl
        {
            get => F.AsBool(F.Memo(this, "IsLiveWithUnrehearsedControl", () => F.And(F.Bool3(F.Of(this.IsLive)), F.Bool3(F.Of(this.HasUnrehearsedControlEntry))))); set { }
        }

        // Formula CadenceBreachCount (rulebook: =COUNTIFS(ReviewEvents!{{CadenceBreachVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? CadenceBreachCount
        {
            get => F.AsDecimal(F.Memo(this, "CadenceBreachCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ReviewEvent>(base.SoAContext, "ReviewEvents", __c => __c.ReviewEvents), __r => F.CritField(F.Of(__r.CadenceBreachVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula IsInCadenceBreach (rulebook: ={{CadenceBreachCount}} > 0)
        [NotMapped]
        public bool? IsInCadenceBreach
        {
            get => F.AsBool(F.Memo(this, "IsInCadenceBreach", () => F.Cmp(F.Of(this.CadenceBreachCount), ">", F.I(0)))); set { }
        }

        // Formula HasDecisionInFlight (rulebook: ={{OpenChangeRequestCount}} > 0)
        [NotMapped]
        public bool? HasDecisionInFlight
        {
            get => F.AsBool(F.Memo(this, "HasDecisionInFlight", () => F.Cmp(F.Of(this.OpenChangeRequestCount), ">", F.I(0)))); set { }
        }

        // Formula IsUnremediatedCadenceBreach (rulebook: =AND({{IsInCadenceBreach}}, NOT({{HasDecisionInFlight}})))
        [NotMapped]
        public bool? IsUnremediatedCadenceBreach
        {
            get => F.AsBool(F.Memo(this, "IsUnremediatedCadenceBreach", () => F.And(F.Bool3(F.Of(this.IsInCadenceBreach)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasDecisionInFlight))))))); set { }
        }

        // Formula IsManagedCadenceBreach (rulebook: =AND({{IsInCadenceBreach}}, {{HasDecisionInFlight}}))
        [NotMapped]
        public bool? IsManagedCadenceBreach
        {
            get => F.AsBool(F.Memo(this, "IsManagedCadenceBreach", () => F.And(F.Bool3(F.Of(this.IsInCadenceBreach)), F.Bool3(F.Of(this.HasDecisionInFlight))))); set { }
        }

        // Formula GovernanceIsSilent (rulebook: =AND({{IsLive}}, NOT({{HasGovernanceRecord}})))
        [NotMapped]
        public bool? GovernanceIsSilent
        {
            get => F.AsBool(F.Memo(this, "GovernanceIsSilent", () => F.And(F.Bool3(F.Of(this.IsLive)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasGovernanceRecord))))))); set { }
        }

        // Formula ValidFragmentCount (rulebook: =COUNTIFS(KnowledgeFragments!{{ValidFragmentVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? ValidFragmentCount
        {
            get => F.AsDecimal(F.Memo(this, "ValidFragmentCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritField(F.Of(__r.ValidFragmentVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula StillOwnsValidKnowledge (rulebook: ={{ValidFragmentCount}} > 0)
        [NotMapped]
        public bool? StillOwnsValidKnowledge
        {
            get => F.AsBool(F.Memo(this, "StillOwnsValidKnowledge", () => F.Cmp(F.Of(this.ValidFragmentCount), ">", F.I(0)))); set { }
        }

        // Formula IncomingSupersessionCount (rulebook: =COUNTIFS(ProcedureVersionLinks!{{SupersededVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? IncomingSupersessionCount
        {
            get => F.AsDecimal(F.Memo(this, "IncomingSupersessionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureVersionLink>(base.SoAContext, "ProcedureVersionLinks", __c => __c.ProcedureVersionLinks), __r => F.CritField(F.Of(__r.SupersededVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula IsStillReferenced (rulebook: ={{IncomingSupersessionCount}} > 0)
        [NotMapped]
        public bool? IsStillReferenced
        {
            get => F.AsBool(F.Memo(this, "IsStillReferenced", () => F.Cmp(F.Of(this.IncomingSupersessionCount), ">", F.I(0)))); set { }
        }

        // Formula IsLoadBearingOrphan (rulebook: =AND({{IsUnstewarded}}, OR({{StillOwnsValidKnowledge}}, {{IsStillReferenced}})))
        [NotMapped]
        public bool? IsLoadBearingOrphan
        {
            get => F.AsBool(F.Memo(this, "IsLoadBearingOrphan", () => F.And(F.Bool3(F.Of(this.IsUnstewarded)), F.Bool3(F.Or(F.Bool3(F.Of(this.StillOwnsValidKnowledge)), F.Bool3(F.Of(this.IsStillReferenced))))))); set { }
        }

        // Formula IsCleanlyRetired (rulebook: =AND({{IsUnstewarded}}, NOT({{StillOwnsValidKnowledge}}), NOT({{IsStillReferenced}})))
        [NotMapped]
        public bool? IsCleanlyRetired
        {
            get => F.AsBool(F.Memo(this, "IsCleanlyRetired", () => F.And(F.Bool3(F.Of(this.IsUnstewarded)), F.Bool3(F.Not(F.Bool3(F.Of(this.StillOwnsValidKnowledge)))), F.Bool3(F.Not(F.Bool3(F.Of(this.IsStillReferenced))))))); set { }
        }

        // Formula StalledImplementationCount (rulebook: =COUNTIFS(ChangeRequests!{{StalledImplementationVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? StalledImplementationCount
        {
            get => F.AsDecimal(F.Memo(this, "StalledImplementationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeRequest>(base.SoAContext, "ChangeRequests", __c => __c.ChangeRequests), __r => F.CritField(F.Of(__r.StalledImplementationVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula IsHeldUnfitByLandedDecisions (rulebook: =AND(NOT({{IsFitToExecute}}), {{StalledImplementationCount}} > 0))
        [NotMapped]
        public bool? IsHeldUnfitByLandedDecisions
        {
            get => F.AsBool(F.Memo(this, "IsHeldUnfitByLandedDecisions", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsFitToExecute)))), F.Bool3(F.Cmp(F.Of(this.StalledImplementationCount), ">", F.I(0)))))); set { }
        }

        // Formula UndeclaredControlKindCount (rulebook: =COUNTIFS(Steps!{{UndeclaredControlVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? UndeclaredControlKindCount
        {
            get => F.AsDecimal(F.Memo(this, "UndeclaredControlKindCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.UndeclaredControlVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula ControlTaxonomyIsIncomplete (rulebook: ={{UndeclaredControlKindCount}} > 0)
        [NotMapped]
        public bool? ControlTaxonomyIsIncomplete
        {
            get => F.AsBool(F.Memo(this, "ControlTaxonomyIsIncomplete", () => F.Cmp(F.Of(this.UndeclaredControlKindCount), ">", F.I(0)))); set { }
        }

        // Formula HasApprovedChangeRequest (rulebook: ={{ApprovedChangeRequestCount}} > 0)
        [NotMapped]
        public bool? HasApprovedChangeRequest
        {
            get => F.AsBool(F.Memo(this, "HasApprovedChangeRequest", () => F.Cmp(F.Of(this.ApprovedChangeRequestCount), ">", F.I(0)))); set { }
        }

        // Formula ApprovedChangeRequestCount (rulebook: =COUNTIFS(ChangeRequests!{{ApprovedVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? ApprovedChangeRequestCount
        {
            get => F.AsDecimal(F.Memo(this, "ApprovedChangeRequestCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeRequest>(base.SoAContext, "ChangeRequests", __c => __c.ChangeRequests), __r => F.CritField(F.Of(__r.ApprovedVersionKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula UnwatchedUnownedControlCount (rulebook: =COUNTIFS(Requirements!{{UnwatchedUnownedFlag}}, "unwatched-unowned"))
        [NotMapped]
        public decimal? UnwatchedUnownedControlCount
        {
            get => F.AsDecimal(F.Memo(this, "UnwatchedUnownedControlCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Requirement>(base.SoAContext, "Requirements", __c => __c.Requirements), __r => F.CritLiteral(F.Of(__r.UnwatchedUnownedFlag), F.S("unwatched-unowned")))))); set { }
        }

        // Formula MiningRunCount (rulebook: =COUNTIFS(ProcessMiningRuns!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? MiningRunCount
        {
            get => F.AsDecimal(F.Memo(this, "MiningRunCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcessMiningRun>(base.SoAContext, "ProcessMiningRuns", __c => __c.ProcessMiningRuns), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula DriftedMiningRunCount (rulebook: =COUNTIFS(ProcessMiningRuns!{{DriftedMiningRunKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public decimal? DriftedMiningRunCount
        {
            get => F.AsDecimal(F.Memo(this, "DriftedMiningRunCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcessMiningRun>(base.SoAContext, "ProcessMiningRuns", __c => __c.ProcessMiningRuns), __r => F.CritField(F.Of(__r.DriftedMiningRunKey), F.Of(this.ProcedureVersionId)))))); set { }
        }

        // Formula HasUnresolvedMiningDrift (rulebook: ={{DriftedMiningRunCount}} > 0)
        [NotMapped]
        public bool? HasUnresolvedMiningDrift
        {
            get => F.AsBool(F.Memo(this, "HasUnresolvedMiningDrift", () => F.Cmp(F.Of(this.DriftedMiningRunCount), ">", F.I(0)))); set { }
        }

        // Formula EntryStepId (rulebook: =MAXIFS(Steps!{{EntryStepKey}}, Steps!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}))
        [NotMapped]
        public string? EntryStepId
        {
            get => F.AsString(F.Memo(this, "EntryStepId", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)), __r => F.Of(__r.EntryStepKey))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
        public string? CreatedByAgent { get; set; }
        public string? ModifiedByAgent { get; set; }
        public string? EvaluationContext { get; set; }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("CreatedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(CreatedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. CreatedByAgent: " + CreatedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(CreatedByAgent);
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
                        CreatedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("ModifiedByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(ModifiedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. ModifiedByAgent: " + ModifiedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(ModifiedByAgent);
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
                        ModifiedByAgent = _agentRef.AgentId;
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

        private ObservableCollection<ProcedureVersionLink> _previousProcedureVersionProcedureVersionLinks;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ProcedureVersionLink> PreviousProcedureVersionProcedureVersionLinks
        {
            get
            {
                if (_previousProcedureVersionProcedureVersionLinks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PreviousProcedureVersionProcedureVersionLinks - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _previousProcedureVersionProcedureVersionLinks = new ObservableCollection<ProcedureVersionLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureVersionLinks.Where(x => x.PreviousProcedureVersion == this.ProcedureVersionId).ToList<ProcedureVersionLink>();
                        _previousProcedureVersionProcedureVersionLinks = new ObservableCollection<ProcedureVersionLink>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _previousProcedureVersionProcedureVersionLinks.CollectionChanged += PreviousProcedureVersionProcedureVersionLinks_CollectionChanged;
                }
                return _previousProcedureVersionProcedureVersionLinks;
            }
            private set
            {
                if (_previousProcedureVersionProcedureVersionLinks != null)
                {
                    _previousProcedureVersionProcedureVersionLinks.CollectionChanged -= PreviousProcedureVersionProcedureVersionLinks_CollectionChanged;
                }
                _previousProcedureVersionProcedureVersionLinks = value;
                if (_previousProcedureVersionProcedureVersionLinks != null)
                {
                    _previousProcedureVersionProcedureVersionLinks.CollectionChanged += PreviousProcedureVersionProcedureVersionLinks_CollectionChanged;
                }
            }
        }

        private void PreviousProcedureVersionProcedureVersionLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureVersionLink>())
                {
                    item.PreviousProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ProcedureVersionLink> _nextProcedureVersionProcedureVersionLinks;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<ProcedureVersionLink> NextProcedureVersionProcedureVersionLinks
        {
            get
            {
                if (_nextProcedureVersionProcedureVersionLinks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access NextProcedureVersionProcedureVersionLinks - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _nextProcedureVersionProcedureVersionLinks = new ObservableCollection<ProcedureVersionLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureVersionLinks.Where(x => x.NextProcedureVersion == this.ProcedureVersionId).ToList<ProcedureVersionLink>();
                        _nextProcedureVersionProcedureVersionLinks = new ObservableCollection<ProcedureVersionLink>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _nextProcedureVersionProcedureVersionLinks.CollectionChanged += NextProcedureVersionProcedureVersionLinks_CollectionChanged;
                }
                return _nextProcedureVersionProcedureVersionLinks;
            }
            private set
            {
                if (_nextProcedureVersionProcedureVersionLinks != null)
                {
                    _nextProcedureVersionProcedureVersionLinks.CollectionChanged -= NextProcedureVersionProcedureVersionLinks_CollectionChanged;
                }
                _nextProcedureVersionProcedureVersionLinks = value;
                if (_nextProcedureVersionProcedureVersionLinks != null)
                {
                    _nextProcedureVersionProcedureVersionLinks.CollectionChanged += NextProcedureVersionProcedureVersionLinks_CollectionChanged;
                }
            }
        }

        private void NextProcedureVersionProcedureVersionLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureVersionLink>())
                {
                    item.NextProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ProcedureStatusChange> _procedureStatusChanges;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<ProcedureStatusChange> ProcedureStatusChanges
        {
            get
            {
                if (_procedureStatusChanges == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureStatusChanges - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _procedureStatusChanges = new ObservableCollection<ProcedureStatusChange>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureStatusChanges.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ProcedureStatusChange>();
                        _procedureStatusChanges = new ObservableCollection<ProcedureStatusChange>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureStatusChanges.CollectionChanged += ProcedureStatusChanges_CollectionChanged;
                }
                return _procedureStatusChanges;
            }
            private set
            {
                if (_procedureStatusChanges != null)
                {
                    _procedureStatusChanges.CollectionChanged -= ProcedureStatusChanges_CollectionChanged;
                }
                _procedureStatusChanges = value;
                if (_procedureStatusChanges != null)
                {
                    _procedureStatusChanges.CollectionChanged += ProcedureStatusChanges_CollectionChanged;
                }
            }
        }

        private void ProcedureStatusChanges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureStatusChange>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<Step> _steps;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<Step> Steps
        {
            get
            {
                if (_steps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Steps - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _steps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = base.SoAContext.Steps.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<Step>();
                        _steps = new ObservableCollection<Step>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _steps.CollectionChanged += Steps_CollectionChanged;
                }
                return _steps;
            }
            private set
            {
                if (_steps != null)
                {
                    _steps.CollectionChanged -= Steps_CollectionChanged;
                }
                _steps = value;
                if (_steps != null)
                {
                    _steps.CollectionChanged += Steps_CollectionChanged;
                }
            }
        }

        private void Steps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Step>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<StepTransition> _stepTransitions;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<StepTransition> StepTransitions
        {
            get
            {
                if (_stepTransitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepTransitions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _stepTransitions = new ObservableCollection<StepTransition>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepTransitions.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<StepTransition>();
                        _stepTransitions = new ObservableCollection<StepTransition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepTransitions.CollectionChanged += StepTransitions_CollectionChanged;
                }
                return _stepTransitions;
            }
            private set
            {
                if (_stepTransitions != null)
                {
                    _stepTransitions.CollectionChanged -= StepTransitions_CollectionChanged;
                }
                _stepTransitions = value;
                if (_stepTransitions != null)
                {
                    _stepTransitions.CollectionChanged += StepTransitions_CollectionChanged;
                }
            }
        }

        private void StepTransitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepTransition>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<Rationale> _rationales;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<Rationale> Rationales
        {
            get
            {
                if (_rationales == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Rationales - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _rationales = new ObservableCollection<Rationale>();
                    }
                    else
                    {
                        var items = base.SoAContext.Rationales.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<Rationale>();
                        _rationales = new ObservableCollection<Rationale>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _rationales.CollectionChanged += Rationales_CollectionChanged;
                }
                return _rationales;
            }
            private set
            {
                if (_rationales != null)
                {
                    _rationales.CollectionChanged -= Rationales_CollectionChanged;
                }
                _rationales = value;
                if (_rationales != null)
                {
                    _rationales.CollectionChanged += Rationales_CollectionChanged;
                }
            }
        }

        private void Rationales_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Rationale>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<Exception> _exceptions;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<Exception> Exceptions
        {
            get
            {
                if (_exceptions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Exceptions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _exceptions = new ObservableCollection<Exception>();
                    }
                    else
                    {
                        var items = base.SoAContext.Exceptions.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<Exception>();
                        _exceptions = new ObservableCollection<Exception>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _exceptions.CollectionChanged += Exceptions_CollectionChanged;
                }
                return _exceptions;
            }
            private set
            {
                if (_exceptions != null)
                {
                    _exceptions.CollectionChanged -= Exceptions_CollectionChanged;
                }
                _exceptions = value;
                if (_exceptions != null)
                {
                    _exceptions.CollectionChanged += Exceptions_CollectionChanged;
                }
            }
        }

        private void Exceptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Exception>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ProcedureResource> _procedureResources;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<ProcedureResource> ProcedureResources
        {
            get
            {
                if (_procedureResources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureResources - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _procedureResources = new ObservableCollection<ProcedureResource>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureResources.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ProcedureResource>();
                        _procedureResources = new ObservableCollection<ProcedureResource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureResources.CollectionChanged += ProcedureResources_CollectionChanged;
                }
                return _procedureResources;
            }
            private set
            {
                if (_procedureResources != null)
                {
                    _procedureResources.CollectionChanged -= ProcedureResources_CollectionChanged;
                }
                _procedureResources = value;
                if (_procedureResources != null)
                {
                    _procedureResources.CollectionChanged += ProcedureResources_CollectionChanged;
                }
            }
        }

        private void ProcedureResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureResource>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ElicitationSession> _elicitationSessions;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<ElicitationSession> ElicitationSessions
        {
            get
            {
                if (_elicitationSessions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ElicitationSessions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _elicitationSessions = new ObservableCollection<ElicitationSession>();
                    }
                    else
                    {
                        var items = base.SoAContext.ElicitationSessions.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ElicitationSession>();
                        _elicitationSessions = new ObservableCollection<ElicitationSession>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _elicitationSessions.CollectionChanged += ElicitationSessions_CollectionChanged;
                }
                return _elicitationSessions;
            }
            private set
            {
                if (_elicitationSessions != null)
                {
                    _elicitationSessions.CollectionChanged -= ElicitationSessions_CollectionChanged;
                }
                _elicitationSessions = value;
                if (_elicitationSessions != null)
                {
                    _elicitationSessions.CollectionChanged += ElicitationSessions_CollectionChanged;
                }
            }
        }

        private void ElicitationSessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ElicitationSession>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<KnowledgeFragment> _knowledgeFragments;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<KnowledgeFragment> KnowledgeFragments
        {
            get
            {
                if (_knowledgeFragments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeFragments.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<KnowledgeFragment>();
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
                return _knowledgeFragments;
            }
            private set
            {
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged -= KnowledgeFragments_CollectionChanged;
                }
                _knowledgeFragments = value;
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
            }
        }

        private void KnowledgeFragments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeFragment>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<KnowledgeGap> _knowledgeGaps;

        [InverseProperty("ProcedureVersionRef")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeGaps - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeGaps.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<KnowledgeGap>();
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
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<FAQ> _fAQs;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<FAQ> FAQs
        {
            get
            {
                if (_fAQs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FAQs - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _fAQs = new ObservableCollection<FAQ>();
                    }
                    else
                    {
                        var items = base.SoAContext.FAQs.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<FAQ>();
                        _fAQs = new ObservableCollection<FAQ>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fAQs.CollectionChanged += FAQs_CollectionChanged;
                }
                return _fAQs;
            }
            private set
            {
                if (_fAQs != null)
                {
                    _fAQs.CollectionChanged -= FAQs_CollectionChanged;
                }
                _fAQs = value;
                if (_fAQs != null)
                {
                    _fAQs.CollectionChanged += FAQs_CollectionChanged;
                }
            }
        }

        private void FAQs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FAQ>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<Explanation> _explanations;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<Explanation> Explanations
        {
            get
            {
                if (_explanations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Explanations - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _explanations = new ObservableCollection<Explanation>();
                    }
                    else
                    {
                        var items = base.SoAContext.Explanations.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<Explanation>();
                        _explanations = new ObservableCollection<Explanation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _explanations.CollectionChanged += Explanations_CollectionChanged;
                }
                return _explanations;
            }
            private set
            {
                if (_explanations != null)
                {
                    _explanations.CollectionChanged -= Explanations_CollectionChanged;
                }
                _explanations = value;
                if (_explanations != null)
                {
                    _explanations.CollectionChanged += Explanations_CollectionChanged;
                }
            }
        }

        private void Explanations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Explanation>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ProcedureExecution> _procedureExecutions;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<ProcedureExecution> ProcedureExecutions
        {
            get
            {
                if (_procedureExecutions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecutions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureExecutions.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ProcedureExecution>();
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureExecutions.CollectionChanged += ProcedureExecutions_CollectionChanged;
                }
                return _procedureExecutions;
            }
            private set
            {
                if (_procedureExecutions != null)
                {
                    _procedureExecutions.CollectionChanged -= ProcedureExecutions_CollectionChanged;
                }
                _procedureExecutions = value;
                if (_procedureExecutions != null)
                {
                    _procedureExecutions.CollectionChanged += ProcedureExecutions_CollectionChanged;
                }
            }
        }

        private void ProcedureExecutions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureExecution>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<StewardshipAssignment> _stewardshipAssignments;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<StewardshipAssignment> StewardshipAssignments
        {
            get
            {
                if (_stewardshipAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StewardshipAssignments - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _stewardshipAssignments = new ObservableCollection<StewardshipAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.StewardshipAssignments.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<StewardshipAssignment>();
                        _stewardshipAssignments = new ObservableCollection<StewardshipAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stewardshipAssignments.CollectionChanged += StewardshipAssignments_CollectionChanged;
                }
                return _stewardshipAssignments;
            }
            private set
            {
                if (_stewardshipAssignments != null)
                {
                    _stewardshipAssignments.CollectionChanged -= StewardshipAssignments_CollectionChanged;
                }
                _stewardshipAssignments = value;
                if (_stewardshipAssignments != null)
                {
                    _stewardshipAssignments.CollectionChanged += StewardshipAssignments_CollectionChanged;
                }
            }
        }

        private void StewardshipAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StewardshipAssignment>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ChangeRequest> _changeRequests;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<ChangeRequest> ChangeRequests
        {
            get
            {
                if (_changeRequests == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeRequests - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _changeRequests = new ObservableCollection<ChangeRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeRequests.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ChangeRequest>();
                        _changeRequests = new ObservableCollection<ChangeRequest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _changeRequests.CollectionChanged += ChangeRequests_CollectionChanged;
                }
                return _changeRequests;
            }
            private set
            {
                if (_changeRequests != null)
                {
                    _changeRequests.CollectionChanged -= ChangeRequests_CollectionChanged;
                }
                _changeRequests = value;
                if (_changeRequests != null)
                {
                    _changeRequests.CollectionChanged += ChangeRequests_CollectionChanged;
                }
            }
        }

        private void ChangeRequests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeRequest>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ReviewEvent> _reviewEvents;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<ReviewEvent> ReviewEvents
        {
            get
            {
                if (_reviewEvents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ReviewEvents - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _reviewEvents = new ObservableCollection<ReviewEvent>();
                    }
                    else
                    {
                        var items = base.SoAContext.ReviewEvents.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ReviewEvent>();
                        _reviewEvents = new ObservableCollection<ReviewEvent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _reviewEvents.CollectionChanged += ReviewEvents_CollectionChanged;
                }
                return _reviewEvents;
            }
            private set
            {
                if (_reviewEvents != null)
                {
                    _reviewEvents.CollectionChanged -= ReviewEvents_CollectionChanged;
                }
                _reviewEvents = value;
                if (_reviewEvents != null)
                {
                    _reviewEvents.CollectionChanged += ReviewEvents_CollectionChanged;
                }
            }
        }

        private void ReviewEvents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ReviewEvent>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<LearningActivity> _learningActivities;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<LearningActivity> LearningActivities
        {
            get
            {
                if (_learningActivities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LearningActivities - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _learningActivities = new ObservableCollection<LearningActivity>();
                    }
                    else
                    {
                        var items = base.SoAContext.LearningActivities.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<LearningActivity>();
                        _learningActivities = new ObservableCollection<LearningActivity>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _learningActivities.CollectionChanged += LearningActivities_CollectionChanged;
                }
                return _learningActivities;
            }
            private set
            {
                if (_learningActivities != null)
                {
                    _learningActivities.CollectionChanged -= LearningActivities_CollectionChanged;
                }
                _learningActivities = value;
                if (_learningActivities != null)
                {
                    _learningActivities.CollectionChanged += LearningActivities_CollectionChanged;
                }
            }
        }

        private void LearningActivities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<LearningActivity>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<OperationalBinding> _operationalBindings;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<OperationalBinding> OperationalBindings
        {
            get
            {
                if (_operationalBindings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OperationalBindings - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _operationalBindings = new ObservableCollection<OperationalBinding>();
                    }
                    else
                    {
                        var items = base.SoAContext.OperationalBindings.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<OperationalBinding>();
                        _operationalBindings = new ObservableCollection<OperationalBinding>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _operationalBindings.CollectionChanged += OperationalBindings_CollectionChanged;
                }
                return _operationalBindings;
            }
            private set
            {
                if (_operationalBindings != null)
                {
                    _operationalBindings.CollectionChanged -= OperationalBindings_CollectionChanged;
                }
                _operationalBindings = value;
                if (_operationalBindings != null)
                {
                    _operationalBindings.CollectionChanged += OperationalBindings_CollectionChanged;
                }
            }
        }

        private void OperationalBindings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<OperationalBinding>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<CommunicationPolicy> _communicationPolicies;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<CommunicationPolicy> CommunicationPolicies
        {
            get
            {
                if (_communicationPolicies == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunicationPolicies - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _communicationPolicies = new ObservableCollection<CommunicationPolicy>();
                    }
                    else
                    {
                        var items = base.SoAContext.CommunicationPolicies.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<CommunicationPolicy>();
                        _communicationPolicies = new ObservableCollection<CommunicationPolicy>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _communicationPolicies.CollectionChanged += CommunicationPolicies_CollectionChanged;
                }
                return _communicationPolicies;
            }
            private set
            {
                if (_communicationPolicies != null)
                {
                    _communicationPolicies.CollectionChanged -= CommunicationPolicies_CollectionChanged;
                }
                _communicationPolicies = value;
                if (_communicationPolicies != null)
                {
                    _communicationPolicies.CollectionChanged += CommunicationPolicies_CollectionChanged;
                }
            }
        }

        private void CommunicationPolicies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CommunicationPolicy>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ProcessMiningRun> _processMiningRuns;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<ProcessMiningRun> ProcessMiningRuns
        {
            get
            {
                if (_processMiningRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessMiningRuns - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _processMiningRuns = new ObservableCollection<ProcessMiningRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessMiningRuns.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ProcessMiningRun>();
                        _processMiningRuns = new ObservableCollection<ProcessMiningRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _processMiningRuns.CollectionChanged += ProcessMiningRuns_CollectionChanged;
                }
                return _processMiningRuns;
            }
            private set
            {
                if (_processMiningRuns != null)
                {
                    _processMiningRuns.CollectionChanged -= ProcessMiningRuns_CollectionChanged;
                }
                _processMiningRuns = value;
                if (_processMiningRuns != null)
                {
                    _processMiningRuns.CollectionChanged += ProcessMiningRuns_CollectionChanged;
                }
            }
        }

        private void ProcessMiningRuns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessMiningRun>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.EvaluationContextRef;
            _ = this.PreviousProcedureVersionProcedureVersionLinks;
            _ = this.NextProcedureVersionProcedureVersionLinks;
            _ = this.ProcedureStatusChanges;
            _ = this.Steps;
            _ = this.StepTransitions;
            _ = this.Rationales;
            _ = this.Exceptions;
            _ = this.ProcedureResources;
            _ = this.ElicitationSessions;
            _ = this.KnowledgeFragments;
            _ = this.KnowledgeGaps;
            _ = this.FAQs;
            _ = this.Explanations;
            _ = this.ProcedureExecutions;
            _ = this.StewardshipAssignments;
            _ = this.ChangeRequests;
            _ = this.ReviewEvents;
            _ = this.LearningActivities;
            _ = this.OperationalBindings;
            _ = this.CommunicationPolicies;
            _ = this.ProcessMiningRuns;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
