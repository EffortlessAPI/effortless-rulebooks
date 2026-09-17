
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
        // Formula ExecutionCount (rulebook: =COUNTIFS(ProcedureExecutions!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? ExecutionCount
        {
            get => F.AsInt(F.Memo(this, "ExecutionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureExecution>(base.SoAContext, "ProcedureExecutions", __c => __c.ProcedureExecutions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        public decimal? ExpectedDurationValue { get; set; }
        public string? ExpectedDurationUnit { get; set; }
        // Formula StatusIsPko (rulebook: =INDEX(LifecycleStatuses!{{IsPkoStatus}}, MATCH({{Status}}, LifecycleStatuses!{{LifecycleStatusId}}, 0)))
        [NotMapped]
        public bool? StatusIsPko
        {
            get => F.AsBool(F.Memo(this, "StatusIsPko", () => F.Lookup<LifecycleStatuse>(this, "LifecycleStatuses", "LifecycleStatusId", __c => __c.LifecycleStatuses, __r => F.Of(__r.LifecycleStatusId), F.Of(this.Status), __r => F.Of(__r.IsPkoStatus), () => F.Of(new LifecycleStatuse().IsPkoStatus)))); set { }
        }

        // Formula UsesNonPkoStatus (rulebook: ={{StatusIsPko}} = FALSE)
        [NotMapped]
        public bool? UsesNonPkoStatus
        {
            get => F.AsBool(F.Memo(this, "UsesNonPkoStatus", () => F.Eq(F.Of(this.StatusIsPko), F.B(false)))); set { }
        }

        // Formula ExceptionCount (rulebook: =COUNTIFS(Exceptions!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? ExceptionCount
        {
            get => F.AsInt(F.Memo(this, "ExceptionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Exception>(base.SoAContext, "Exceptions", __c => __c.Exceptions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula FallbackTransitionCount (rulebook: =COUNTIFS(StepTransitions!{{ProcedureVersion}}, {{ProcedureVersionId}}, StepTransitions!{{TransitionKind}}, "Fallback"))
        [NotMapped]
        public int? FallbackTransitionCount
        {
            get => F.AsInt(F.Memo(this, "FallbackTransitionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepTransition>(base.SoAContext, "StepTransitions", __c => __c.StepTransitions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.TransitionKind), F.S("Fallback"))))))); set { }
        }

        // Formula AlternativeTransitionCount (rulebook: =COUNTIFS(StepTransitions!{{ProcedureVersion}}, {{ProcedureVersionId}}, StepTransitions!{{TransitionKind}}, "Alternative"))
        [NotMapped]
        public int? AlternativeTransitionCount
        {
            get => F.AsInt(F.Memo(this, "AlternativeTransitionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepTransition>(base.SoAContext, "StepTransitions", __c => __c.StepTransitions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.TransitionKind), F.S("Alternative"))))))); set { }
        }

        // Formula HasNoExceptionHandling (rulebook: =AND({{CountOfSteps}} > 0, {{ExceptionCount}} = 0, {{FallbackTransitionCount}} = 0, {{AlternativeTransitionCount}} = 0))
        [NotMapped]
        public bool? HasNoExceptionHandling
        {
            get => F.AsBool(F.Memo(this, "HasNoExceptionHandling", () => F.And(F.Bool3(F.Cmp(F.Of(this.CountOfSteps), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.ExceptionCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.FallbackTransitionCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.AlternativeTransitionCount), F.I(0)))))); set { }
        }

        // Formula LatestSourceDocumentModifiedAt (rulebook: =MAXIFS(ProcedureResources!{{ResourceModifiedAt}}, ProcedureResources!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public DateTimeOffset? LatestSourceDocumentModifiedAt
        {
            get => F.AsDateTime(F.Memo(this, "LatestSourceDocumentModifiedAt", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<ProcedureResource>(base.SoAContext, "ProcedureResources", __c => __c.ProcedureResources), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)), __r => F.Of(__r.ResourceModifiedAt))))); set { }
        }

        // Formula DaysDocumentTrailsVersion (rulebook: =IF({{LatestSourceDocumentModifiedAt}} = "", 0, DATETIME_DIFF({{ModifiedAt}}, {{LatestSourceDocumentModifiedAt}}, "days")))
        [NotMapped]
        public int? DaysDocumentTrailsVersion
        {
            get => F.AsInt(F.Memo(this, "DaysDocumentTrailsVersion", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.LatestSourceDocumentModifiedAt)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.ModifiedAt), F.Of(this.LatestSourceDocumentModifiedAt), F.S("days")))))); set { }
        }

        // Formula DocumentLagsPractice (rulebook: =AND({{LatestSourceDocumentModifiedAt}} <> "", {{DaysDocumentTrailsVersion}} > 60))
        [NotMapped]
        public bool? DocumentLagsPractice
        {
            get => F.AsBool(F.Memo(this, "DocumentLagsPractice", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.LatestSourceDocumentModifiedAt))), F.Bool3(F.Cmp(F.Of(this.DaysDocumentTrailsVersion), ">", F.I(60)))))); set { }
        }

        // Formula HumanStepCount (rulebook: =COUNTIFS(Steps!{{ProcedureVersion}}, {{ProcedureVersionId}}, Steps!{{AssignedAgentKind}}, "Human"))
        [NotMapped]
        public int? HumanStepCount
        {
            get => F.AsInt(F.Memo(this, "HumanStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.AssignedAgentKind), F.S("Human"))))))); set { }
        }

        // Formula NonHumanStepCount (rulebook: ={{CountOfSteps}} - {{HumanStepCount}})
        [NotMapped]
        public int? NonHumanStepCount
        {
            get => F.AsInt(F.Memo(this, "NonHumanStepCount", () => F.Integer(F.Sub(F.Of(this.CountOfSteps), F.Of(this.HumanStepCount))))); set { }
        }

        // Formula MixesHumanAndSoftwareSteps (rulebook: =AND({{HumanStepCount}} > 0, {{NonHumanStepCount}} > 0))
        [NotMapped]
        public bool? MixesHumanAndSoftwareSteps
        {
            get => F.AsBool(F.Memo(this, "MixesHumanAndSoftwareSteps", () => F.And(F.Bool3(F.Cmp(F.Of(this.HumanStepCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.NonHumanStepCount), ">", F.I(0)))))); set { }
        }

        // Formula DayRunCount (rulebook: =COUNTIFS(ProcedureExecutions!{{ProcedureVersion}}, {{ProcedureVersionId}}, ProcedureExecutions!{{Shift}}, "Day"))
        [NotMapped]
        public int? DayRunCount
        {
            get => F.AsInt(F.Memo(this, "DayRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureExecution>(base.SoAContext, "ProcedureExecutions", __c => __c.ProcedureExecutions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.Shift), F.S("Day"))))))); set { }
        }

        // Formula NightRunCount (rulebook: =COUNTIFS(ProcedureExecutions!{{ProcedureVersion}}, {{ProcedureVersionId}}, ProcedureExecutions!{{Shift}}, "Night"))
        [NotMapped]
        public int? NightRunCount
        {
            get => F.AsInt(F.Memo(this, "NightRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureExecution>(base.SoAContext, "ProcedureExecutions", __c => __c.ProcedureExecutions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.Shift), F.S("Night"))))))); set { }
        }

        // Formula DayDeviatingRunCount (rulebook: =COUNTIFS(ProcedureExecutions!{{DeviatingDayVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? DayDeviatingRunCount
        {
            get => F.AsInt(F.Memo(this, "DayDeviatingRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureExecution>(base.SoAContext, "ProcedureExecutions", __c => __c.ProcedureExecutions), __r => F.CritField(F.Of(__r.DeviatingDayVersionKey), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula NightDeviatingRunCount (rulebook: =COUNTIFS(ProcedureExecutions!{{DeviatingNightVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? NightDeviatingRunCount
        {
            get => F.AsInt(F.Memo(this, "NightDeviatingRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureExecution>(base.SoAContext, "ProcedureExecutions", __c => __c.ProcedureExecutions), __r => F.CritField(F.Of(__r.DeviatingNightVersionKey), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula IsInconsistentAcrossShifts (rulebook: =AND({{DayRunCount}} > 0, {{NightRunCount}} > 0, OR(AND({{DayDeviatingRunCount}} = 0, {{NightDeviatingRunCount}} > 0), AND({{DayDeviatingRunCount}} > 0, {{NightDeviatingRunCount}} = 0))))
        [NotMapped]
        public bool? IsInconsistentAcrossShifts
        {
            get => F.AsBool(F.Memo(this, "IsInconsistentAcrossShifts", () => F.And(F.Bool3(F.Cmp(F.Of(this.DayRunCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.NightRunCount), ">", F.I(0))), F.Bool3(F.Or(F.Bool3(F.And(F.Bool3(F.Eq(F.Of(this.DayDeviatingRunCount), F.I(0))), F.Bool3(F.Cmp(F.Of(this.NightDeviatingRunCount), ">", F.I(0))))), F.Bool3(F.And(F.Bool3(F.Cmp(F.Of(this.DayDeviatingRunCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.NightDeviatingRunCount), F.I(0)))))))))); set { }
        }

        // Formula OverlapsRelationCount (rulebook: =COUNTIFS(ActivityRelations!{{OverlapsVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? OverlapsRelationCount
        {
            get => F.AsInt(F.Memo(this, "OverlapsRelationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ActivityRelation>(base.SoAContext, "ActivityRelations", __c => __c.ActivityRelations), __r => F.CritField(F.Of(__r.OverlapsVersionKey), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula EnablesRelationCount (rulebook: =COUNTIFS(ActivityRelations!{{EnablesVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? EnablesRelationCount
        {
            get => F.AsInt(F.Memo(this, "EnablesRelationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ActivityRelation>(base.SoAContext, "ActivityRelations", __c => __c.ActivityRelations), __r => F.CritField(F.Of(__r.EnablesVersionKey), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula PreventsRelationCount (rulebook: =COUNTIFS(ActivityRelations!{{PreventsVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? PreventsRelationCount
        {
            get => F.AsInt(F.Memo(this, "PreventsRelationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ActivityRelation>(base.SoAContext, "ActivityRelations", __c => __c.ActivityRelations), __r => F.CritField(F.Of(__r.PreventsVersionKey), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula StepsWithoutOntologyTypeCount (rulebook: =COUNTIFS(Steps!{{UntypedVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? StepsWithoutOntologyTypeCount
        {
            get => F.AsInt(F.Memo(this, "StepsWithoutOntologyTypeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.UntypedVersionKey), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula RestsOnNotationOnly (rulebook: ={{StepsWithoutOntologyTypeCount}} > 0)
        [NotMapped]
        public bool? RestsOnNotationOnly
        {
            get => F.AsBool(F.Memo(this, "RestsOnNotationOnly", () => F.Cmp(F.Of(this.StepsWithoutOntologyTypeCount), ">", F.I(0)))); set { }
        }

        // Formula IsCurrentWithoutMotivation (rulebook: =AND({{IsCurrent}}, {{NewVersionMotivation}} = ""))
        [NotMapped]
        public bool? IsCurrentWithoutMotivation
        {
            get => F.AsBool(F.Memo(this, "IsCurrentWithoutMotivation", () => F.And(F.IsTrueV(F.Of(this.IsCurrent)), F.Bool3(F.IsBlank(F.Of(this.NewVersionMotivation)))))); set { }
        }

        // Formula ConditionlessStepCount (rulebook: =COUNTIFS(Steps!{{ConditionlessVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? ConditionlessStepCount
        {
            get => F.AsInt(F.Memo(this, "ConditionlessStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ConditionlessVersionKey), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula IsUnderSpecifiedForExecution (rulebook: =OR({{CountOfSteps}} = 0, {{ConditionlessStepCount}} = {{CountOfSteps}}, {{HasNoExceptionHandling}}))
        [NotMapped]
        public bool? IsUnderSpecifiedForExecution
        {
            get => F.AsBool(F.Memo(this, "IsUnderSpecifiedForExecution", () => F.Or(F.Bool3(F.Eq(F.Of(this.CountOfSteps), F.I(0))), F.Bool3(F.Eq(F.Of(this.ConditionlessStepCount), F.Of(this.CountOfSteps))), F.Bool3(F.Of(this.HasNoExceptionHandling))))); set { }
        }

        // Formula CoarseTopLevelStepCount (rulebook: =COUNTIFS(Steps!{{CoarseTopLevelVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? CoarseTopLevelStepCount
        {
            get => F.AsInt(F.Memo(this, "CoarseTopLevelStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.CoarseTopLevelVersionKey), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula FineTopLevelStepCount (rulebook: =COUNTIFS(Steps!{{FineTopLevelVersionKey}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? FineTopLevelStepCount
        {
            get => F.AsInt(F.Memo(this, "FineTopLevelStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.FineTopLevelVersionKey), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula MixesGranularityAtOneLevel (rulebook: =AND({{CoarseTopLevelStepCount}} > 0, {{FineTopLevelStepCount}} > 0))
        [NotMapped]
        public bool? MixesGranularityAtOneLevel
        {
            get => F.AsBool(F.Memo(this, "MixesGranularityAtOneLevel", () => F.And(F.Bool3(F.Cmp(F.Of(this.CoarseTopLevelStepCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.FineTopLevelStepCount), ">", F.I(0)))))); set { }
        }

        // Formula ProcedureTypeOfVersion (rulebook: =INDEX(Procedures!{{ProcedureType}}, MATCH({{Procedure}}, Procedures!{{ProcedureId}}, 0)))
        [NotMapped]
        public string? ProcedureTypeOfVersion
        {
            get => F.AsString(F.Memo(this, "ProcedureTypeOfVersion", () => F.Lookup<Procedure>(this, "Procedures", "ProcedureId", __c => __c.Procedures, __r => F.Of(__r.ProcedureId), F.Of(this.Procedure), __r => F.Of(__r.ProcedureType), () => F.Of(new Procedure().ProcedureType)))); set { }
        }

        // Formula CreatedByAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{CreatedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? CreatedByAgentKind
        {
            get => F.AsString(F.Memo(this, "CreatedByAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.CreatedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula ElicitationSessionCount (rulebook: =COUNTIFS(ElicitationSessions!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? ElicitationSessionCount
        {
            get => F.AsInt(F.Memo(this, "ElicitationSessionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationSession>(base.SoAContext, "ElicitationSessions", __c => __c.ElicitationSessions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula ExpertCaptureCount (rulebook: =COUNTIFS(AuthoringSubmissions!{{ProcedureVersion}}, {{ProcedureVersionId}}, AuthoringSubmissions!{{SubmitterExpertise}}, "DomainExpert"))
        [NotMapped]
        public int? ExpertCaptureCount
        {
            get => F.AsInt(F.Memo(this, "ExpertCaptureCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AuthoringSubmission>(base.SoAContext, "AuthoringSubmissions", __c => __c.AuthoringSubmissions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.SubmitterExpertise), F.S("DomainExpert"))))))); set { }
        }

        // Formula ElicitationEvidenceCount (rulebook: ={{ElicitationSessionCount}} + {{ExpertCaptureCount}})
        [NotMapped]
        public int? ElicitationEvidenceCount
        {
            get => F.AsInt(F.Memo(this, "ElicitationEvidenceCount", () => F.Integer(F.Add(F.Of(this.ElicitationSessionCount), F.Of(this.ExpertCaptureCount))))); set { }
        }

        // Formula IndexedSegmentCount (rulebook: =COUNTIFS(RetrievalSegments!{{ProcedureVersion}}, {{ProcedureVersionId}}, RetrievalSegments!{{IsIndexed}}, TRUE))
        [NotMapped]
        public int? IndexedSegmentCount
        {
            get => F.AsInt(F.Memo(this, "IndexedSegmentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RetrievalSegment>(base.SoAContext, "RetrievalSegments", __c => __c.RetrievalSegments), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.IsIndexed), F.B(true))))))); set { }
        }

        // Formula SearchCount (rulebook: =COUNTIFS(KnowledgeSearchEvents!{{SoughtProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? SearchCount
        {
            get => F.AsInt(F.Memo(this, "SearchCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeSearchEvent>(base.SoAContext, "KnowledgeSearchEvents", __c => __c.KnowledgeSearchEvents), __r => F.CritField(F.Of(__r.SoughtProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula SuccessfulSearchCount (rulebook: =COUNTIFS(KnowledgeSearchEvents!{{SoughtProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeSearchEvents!{{FoundNothingUseful}}, FALSE))
        [NotMapped]
        public int? SuccessfulSearchCount
        {
            get => F.AsInt(F.Memo(this, "SuccessfulSearchCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeSearchEvent>(base.SoAContext, "KnowledgeSearchEvents", __c => __c.KnowledgeSearchEvents), __r => F.CritField(F.Of(__r.SoughtProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.FoundNothingUseful), F.B(false))))))); set { }
        }

        // Formula SearchSuccessPercent (rulebook: =IF({{SearchCount}} = 0, 0, ROUND(100 * {{SuccessfulSearchCount}} / {{SearchCount}}, 1)))
        [NotMapped]
        public decimal? SearchSuccessPercent
        {
            get => F.AsDecimal(F.Memo(this, "SearchSuccessPercent", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.SearchCount), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.SuccessfulSearchCount)), F.Of(this.SearchCount)), F.I(1))))); set { }
        }

        // Formula OpenQuestionAnnotationCount (rulebook: =COUNTIFS(ModelAnnotations!{{ProcedureVersion}}, {{ProcedureVersionId}}, ModelAnnotations!{{AnnotationKind}}, "Question", ModelAnnotations!{{Status}}, "Open"))
        [NotMapped]
        public int? OpenQuestionAnnotationCount
        {
            get => F.AsInt(F.Memo(this, "OpenQuestionAnnotationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelAnnotation>(base.SoAContext, "ModelAnnotations", __c => __c.ModelAnnotations), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.AnnotationKind), F.S("Question")) && F.CritLiteral(F.Of(__r.Status), F.S("Open"))))))); set { }
        }

        // Formula IsInadequateForUse (rulebook: =OR({{IndexedSegmentCount}} = 0, AND({{SearchCount}} > 0, {{SearchSuccessPercent}} < 50), {{OpenQuestionAnnotationCount}} >= 2))
        [NotMapped]
        public bool? IsInadequateForUse
        {
            get => F.AsBool(F.Memo(this, "IsInadequateForUse", () => F.Or(F.Bool3(F.Eq(F.Of(this.IndexedSegmentCount), F.I(0))), F.Bool3(F.And(F.Bool3(F.Cmp(F.Of(this.SearchCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.SearchSuccessPercent), "<", F.I(50))))), F.Bool3(F.Cmp(F.Of(this.OpenQuestionAnnotationCount), ">=", F.I(2)))))); set { }
        }

        // Formula ServedAssertionCount (rulebook: =COUNTIFS(SnapshotAssertions!{{SourceProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? ServedAssertionCount
        {
            get => F.AsInt(F.Memo(this, "ServedAssertionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SnapshotAssertion>(base.SoAContext, "SnapshotAssertions", __c => __c.SnapshotAssertions), __r => F.CritField(F.Of(__r.SourceProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula LacksMachineInterpretableEncoding (rulebook: =AND({{IsCurrent}}, OR({{IndexedSegmentCount}} = 0, {{ServedAssertionCount}} = 0)))
        [NotMapped]
        public bool? LacksMachineInterpretableEncoding
        {
            get => F.AsBool(F.Memo(this, "LacksMachineInterpretableEncoding", () => F.And(F.IsTrueV(F.Of(this.IsCurrent)), F.Bool3(F.Or(F.Bool3(F.Eq(F.Of(this.IndexedSegmentCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.ServedAssertionCount), F.I(0)))))))); set { }
        }

        // Formula StructuredQueryCount (rulebook: =COUNTIFS(KnowledgeQueryDefinitions!{{TargetProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? StructuredQueryCount
        {
            get => F.AsInt(F.Memo(this, "StructuredQueryCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeQueryDefinition>(base.SoAContext, "KnowledgeQueryDefinitions", __c => __c.KnowledgeQueryDefinitions), __r => F.CritField(F.Of(__r.TargetProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula ProfileValidatedSubmissionCount (rulebook: =COUNTIFS(AuthoringSubmissions!{{ProcedureVersion}}, {{ProcedureVersionId}}, AuthoringSubmissions!{{ProfileValidationPassed}}, TRUE))
        [NotMapped]
        public int? ProfileValidatedSubmissionCount
        {
            get => F.AsInt(F.Memo(this, "ProfileValidatedSubmissionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AuthoringSubmission>(base.SoAContext, "AuthoringSubmissions", __c => __c.AuthoringSubmissions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.ProfileValidationPassed), F.B(true))))))); set { }
        }

        // Formula ReasonedAssertionCount (rulebook: =COUNTIFS(SnapshotAssertions!{{SourceProcedureVersion}}, {{ProcedureVersionId}}, SnapshotAssertions!{{SnapshotIsReasoned}}, TRUE))
        [NotMapped]
        public int? ReasonedAssertionCount
        {
            get => F.AsInt(F.Memo(this, "ReasonedAssertionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SnapshotAssertion>(base.SoAContext, "SnapshotAssertions", __c => __c.SnapshotAssertions), __r => F.CritField(F.Of(__r.SourceProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.SnapshotIsReasoned), F.B(true))))))); set { }
        }

        // Formula IsNotQueryValidateReasonReady (rulebook: =AND({{IsCurrent}}, OR({{StructuredQueryCount}} = 0, {{ProfileValidatedSubmissionCount}} = 0, {{ReasonedAssertionCount}} = 0)))
        [NotMapped]
        public bool? IsNotQueryValidateReasonReady
        {
            get => F.AsBool(F.Memo(this, "IsNotQueryValidateReasonReady", () => F.And(F.IsTrueV(F.Of(this.IsCurrent)), F.Bool3(F.Or(F.Bool3(F.Eq(F.Of(this.StructuredQueryCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.ProfileValidatedSubmissionCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.ReasonedAssertionCount), F.I(0)))))))); set { }
        }

        // Formula PublishedProjectionCount (rulebook: =COUNTIFS(KnowledgeProjections!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeProjections!{{IsPublished}}, TRUE))
        [NotMapped]
        public int? PublishedProjectionCount
        {
            get => F.AsInt(F.Memo(this, "PublishedProjectionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeProjection>(base.SoAContext, "KnowledgeProjections", __c => __c.KnowledgeProjections), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.IsPublished), F.B(true))))))); set { }
        }

        // Formula ConsumerSyncCount (rulebook: =COUNTIFS(ConsumerSystemSyncs!{{LoadedVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? ConsumerSyncCount
        {
            get => F.AsInt(F.Memo(this, "ConsumerSyncCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ConsumerSystemSync>(base.SoAContext, "ConsumerSystemSyncs", __c => __c.ConsumerSystemSyncs), __r => F.CritField(F.Of(__r.LoadedVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula IsUnreachableKnowledge (rulebook: =AND({{IsCurrent}}, {{PublishedProjectionCount}} = 0, {{ConsumerSyncCount}} = 0))
        [NotMapped]
        public bool? IsUnreachableKnowledge
        {
            get => F.AsBool(F.Memo(this, "IsUnreachableKnowledge", () => F.And(F.IsTrueV(F.Of(this.IsCurrent)), F.Bool3(F.Eq(F.Of(this.PublishedProjectionCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.ConsumerSyncCount), F.I(0)))))); set { }
        }

        // Formula HumanSyncCount (rulebook: =COUNTIFS(ConsumerSystemSyncs!{{LoadedVersion}}, {{ProcedureVersionId}}, ConsumerSystemSyncs!{{ReachesHumans}}, TRUE))
        [NotMapped]
        public int? HumanSyncCount
        {
            get => F.AsInt(F.Memo(this, "HumanSyncCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ConsumerSystemSync>(base.SoAContext, "ConsumerSystemSyncs", __c => __c.ConsumerSystemSyncs), __r => F.CritField(F.Of(__r.LoadedVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.ReachesHumans), F.B(true))))))); set { }
        }

        // Formula MachineSyncCount (rulebook: =COUNTIFS(ConsumerSystemSyncs!{{LoadedVersion}}, {{ProcedureVersionId}}, ConsumerSystemSyncs!{{ReachesMachines}}, TRUE))
        [NotMapped]
        public int? MachineSyncCount
        {
            get => F.AsInt(F.Memo(this, "MachineSyncCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ConsumerSystemSync>(base.SoAContext, "ConsumerSystemSyncs", __c => __c.ConsumerSystemSyncs), __r => F.CritField(F.Of(__r.LoadedVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.ReachesMachines), F.B(true))))))); set { }
        }

        // Formula HumanChannelCount (rulebook: ={{PublishedProjectionCount}} + {{HumanSyncCount}})
        [NotMapped]
        public int? HumanChannelCount
        {
            get => F.AsInt(F.Memo(this, "HumanChannelCount", () => F.Integer(F.Add(F.Of(this.PublishedProjectionCount), F.Of(this.HumanSyncCount))))); set { }
        }

        // Formula MachineChannelCount (rulebook: ={{MachineSyncCount}} + {{ServedAssertionCount}})
        [NotMapped]
        public int? MachineChannelCount
        {
            get => F.AsInt(F.Memo(this, "MachineChannelCount", () => F.Integer(F.Add(F.Of(this.MachineSyncCount), F.Of(this.ServedAssertionCount))))); set { }
        }

        // Formula ServesOnlyHumansOrOnlyMachines (rulebook: =AND({{IsCurrent}}, ({{HumanChannelCount}} + {{MachineChannelCount}}) > 0, OR({{HumanChannelCount}} = 0, {{MachineChannelCount}} = 0)))
        [NotMapped]
        public bool? ServesOnlyHumansOrOnlyMachines
        {
            get => F.AsBool(F.Memo(this, "ServesOnlyHumansOrOnlyMachines", () => F.And(F.IsTrueV(F.Of(this.IsCurrent)), F.Bool3(F.Cmp(F.Add(F.Of(this.HumanChannelCount), F.Of(this.MachineChannelCount)), ">", F.I(0))), F.Bool3(F.Or(F.Bool3(F.Eq(F.Of(this.HumanChannelCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.MachineChannelCount), F.I(0)))))))); set { }
        }

        // Formula FreshMiningRunCount (rulebook: =COUNTIFS(ProcessMiningRuns!{{ProcedureVersion}}, {{ProcedureVersionId}}, ProcessMiningRuns!{{IsStaleMiningEvidence}}, FALSE))
        [NotMapped]
        public int? FreshMiningRunCount
        {
            get => F.AsInt(F.Memo(this, "FreshMiningRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcessMiningRun>(base.SoAContext, "ProcessMiningRuns", __c => __c.ProcessMiningRuns), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.IsStaleMiningEvidence), F.B(false))))))); set { }
        }

        // Formula LacksContinuousDriftDetection (rulebook: =AND({{IsCurrent}}, {{ExecutionCount}} > 0, {{FreshMiningRunCount}} = 0))
        [NotMapped]
        public bool? LacksContinuousDriftDetection
        {
            get => F.AsBool(F.Memo(this, "LacksContinuousDriftDetection", () => F.And(F.IsTrueV(F.Of(this.IsCurrent)), F.Bool3(F.Cmp(F.Of(this.ExecutionCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.FreshMiningRunCount), F.I(0)))))); set { }
        }

        // Formula OutcomeMeasurementCount (rulebook: =COUNTIFS(KnowledgeOutcomeMeasurements!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? OutcomeMeasurementCount
        {
            get => F.AsInt(F.Memo(this, "OutcomeMeasurementCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeOutcomeMeasurement>(base.SoAContext, "KnowledgeOutcomeMeasurements", __c => __c.KnowledgeOutcomeMeasurements), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula IsDisconnectedFromOutcomes (rulebook: =AND({{IsCurrent}}, {{ExecutionCount}} > 0, {{OutcomeMeasurementCount}} = 0))
        [NotMapped]
        public bool? IsDisconnectedFromOutcomes
        {
            get => F.AsBool(F.Memo(this, "IsDisconnectedFromOutcomes", () => F.And(F.IsTrueV(F.Of(this.IsCurrent)), F.Bool3(F.Cmp(F.Of(this.ExecutionCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.OutcomeMeasurementCount), F.I(0)))))); set { }
        }

        // Formula AiContributionCount (rulebook: =COUNTIFS(AiInsightProposals!{{TargetProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? AiContributionCount
        {
            get => F.AsInt(F.Memo(this, "AiContributionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AiInsightProposal>(base.SoAContext, "AiInsightProposals", __c => __c.AiInsightProposals), __r => F.CritField(F.Of(__r.TargetProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula AiConsumptionCount (rulebook: ={{ServedAssertionCount}} + {{IndexedSegmentCount}})
        [NotMapped]
        public int? AiConsumptionCount
        {
            get => F.AsInt(F.Memo(this, "AiConsumptionCount", () => F.Integer(F.Add(F.Of(this.ServedAssertionCount), F.Of(this.IndexedSegmentCount))))); set { }
        }

        // Formula UsesAiInOneDirectionOnly (rulebook: =OR(AND({{AiContributionCount}} > 0, {{AiConsumptionCount}} = 0), AND({{AiContributionCount}} = 0, {{AiConsumptionCount}} > 0)))
        [NotMapped]
        public bool? UsesAiInOneDirectionOnly
        {
            get => F.AsBool(F.Memo(this, "UsesAiInOneDirectionOnly", () => F.Or(F.Bool3(F.And(F.Bool3(F.Cmp(F.Of(this.AiContributionCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.AiConsumptionCount), F.I(0))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Of(this.AiContributionCount), F.I(0))), F.Bool3(F.Cmp(F.Of(this.AiConsumptionCount), ">", F.I(0)))))))); set { }
        }

        // Formula IsUnmodifiedForTwelveMonths (rulebook: =AND({{IsCurrent}}, {{DaysSinceModified}} > 365))
        [NotMapped]
        public bool? IsUnmodifiedForTwelveMonths
        {
            get => F.AsBool(F.Memo(this, "IsUnmodifiedForTwelveMonths", () => F.And(F.IsTrueV(F.Of(this.IsCurrent)), F.Bool3(F.Cmp(F.Of(this.DaysSinceModified), ">", F.I(365)))))); set { }
        }

        // Formula DesignDecisionCount (rulebook: =COUNTIFS(ProcessDesignDecisions!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? DesignDecisionCount
        {
            get => F.AsInt(F.Memo(this, "DesignDecisionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcessDesignDecision>(base.SoAContext, "ProcessDesignDecisions", __c => __c.ProcessDesignDecisions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula IsLiveWithoutRecordedDecisions (rulebook: =AND({{IsLive}}, {{DesignDecisionCount}} = 0))
        [NotMapped]
        public bool? IsLiveWithoutRecordedDecisions
        {
            get => F.AsBool(F.Memo(this, "IsLiveWithoutRecordedDecisions", () => F.And(F.Bool3(F.Of(this.IsLive)), F.Bool3(F.Eq(F.Of(this.DesignDecisionCount), F.I(0)))))); set { }
        }

        // Formula AiArtifactConsumingInputCount (rulebook: =COUNTIFS(StepVariables!{{ConsumerWorkflow}}, {{ProcedureVersionId}}, StepVariables!{{IsInputFromAiArtifact}}, TRUE))
        [NotMapped]
        public int? AiArtifactConsumingInputCount
        {
            get => F.AsInt(F.Memo(this, "AiArtifactConsumingInputCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepVariable>(base.SoAContext, "StepVariables", __c => __c.StepVariables), __r => F.CritField(F.Of(__r.ConsumerWorkflow), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.IsInputFromAiArtifact), F.B(true))))))); set { }
        }

        // Formula ContainsStepsAffectedByAiAgentChange (rulebook: ={{AiArtifactConsumingInputCount}} > 0)
        [NotMapped]
        public bool? ContainsStepsAffectedByAiAgentChange
        {
            get => F.AsBool(F.Memo(this, "ContainsStepsAffectedByAiAgentChange", () => F.Cmp(F.Of(this.AiArtifactConsumingInputCount), ">", F.I(0)))); set { }
        }

        // Formula InterviewSessionCount (rulebook: =COUNTIFS(ElicitationSessions!{{ProcedureVersion}}, {{ProcedureVersionId}}, ElicitationSessions!{{ElicitationMode}}, "Interview"))
        [NotMapped]
        public int? InterviewSessionCount
        {
            get => F.AsInt(F.Memo(this, "InterviewSessionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationSession>(base.SoAContext, "ElicitationSessions", __c => __c.ElicitationSessions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.ElicitationMode), F.S("Interview"))))))); set { }
        }

        // Formula ObservationSessionCount (rulebook: =COUNTIFS(ElicitationSessions!{{ProcedureVersion}}, {{ProcedureVersionId}}, ElicitationSessions!{{ElicitationMode}}, "Observation"))
        [NotMapped]
        public int? ObservationSessionCount
        {
            get => F.AsInt(F.Memo(this, "ObservationSessionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationSession>(base.SoAContext, "ElicitationSessions", __c => __c.ElicitationSessions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.ElicitationMode), F.S("Observation"))))))); set { }
        }

        // Formula WorkshopSessionCount (rulebook: =COUNTIFS(ElicitationSessions!{{ProcedureVersion}}, {{ProcedureVersionId}}, ElicitationSessions!{{ElicitationMode}}, "Workshop"))
        [NotMapped]
        public int? WorkshopSessionCount
        {
            get => F.AsInt(F.Memo(this, "WorkshopSessionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationSession>(base.SoAContext, "ElicitationSessions", __c => __c.ElicitationSessions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.ElicitationMode), F.S("Workshop"))))))); set { }
        }

        // Formula ProtocolSessionCount (rulebook: =COUNTIFS(ElicitationSessions!{{ProcedureVersion}}, {{ProcedureVersionId}}, ElicitationSessions!{{ElicitationMode}}, "Protocol"))
        [NotMapped]
        public int? ProtocolSessionCount
        {
            get => F.AsInt(F.Memo(this, "ProtocolSessionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationSession>(base.SoAContext, "ElicitationSessions", __c => __c.ElicitationSessions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.ElicitationMode), F.S("Protocol"))))))); set { }
        }

        // Formula IncidentSessionCount (rulebook: =COUNTIFS(ElicitationSessions!{{ProcedureVersion}}, {{ProcedureVersionId}}, ElicitationSessions!{{ElicitationMode}}, "Incident"))
        [NotMapped]
        public int? IncidentSessionCount
        {
            get => F.AsInt(F.Memo(this, "IncidentSessionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationSession>(base.SoAContext, "ElicitationSessions", __c => __c.ElicitationSessions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.ElicitationMode), F.S("Incident"))))))); set { }
        }

        // Formula ReconciledDivergenceCount (rulebook: =COUNTIFS(WorkflowViewDivergences!{{ProcedureVersion}}, {{ProcedureVersionId}}, WorkflowViewDivergences!{{IsReconciled}}, TRUE))
        [NotMapped]
        public int? ReconciledDivergenceCount
        {
            get => F.AsInt(F.Memo(this, "ReconciledDivergenceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowViewDivergence>(base.SoAContext, "WorkflowViewDivergences", __c => __c.WorkflowViewDivergences), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.IsReconciled), F.B(true))))))); set { }
        }

        // Formula ComplementaryMethodCount (rulebook: =IF({{InterviewSessionCount}} > 0, 1, 0) + IF({{ObservationSessionCount}} > 0, 1, 0) + IF({{WorkshopSessionCount}} > 0, 1, 0) + IF({{ProtocolSessionCount}} > 0, 1, 0) + IF({{IncidentSessionCount}} > 0, 1, 0))
        [NotMapped]
        public int? ComplementaryMethodCount
        {
            get => F.AsInt(F.Memo(this, "ComplementaryMethodCount", () => F.Integer(F.Add(F.Add(F.Add(F.Add((F.Truthy(F.Bool3(F.Cmp(F.Of(this.InterviewSessionCount), ">", F.I(0)))) ? F.I(1) : F.I(0)), (F.Truthy(F.Bool3(F.Cmp(F.Of(this.ObservationSessionCount), ">", F.I(0)))) ? F.I(1) : F.I(0))), (F.Truthy(F.Bool3(F.Cmp(F.Of(this.WorkshopSessionCount), ">", F.I(0)))) ? F.I(1) : F.I(0))), (F.Truthy(F.Bool3(F.Cmp(F.Of(this.ProtocolSessionCount), ">", F.I(0)))) ? F.I(1) : F.I(0))), (F.Truthy(F.Bool3(F.Cmp(F.Of(this.IncidentSessionCount), ">", F.I(0)))) ? F.I(1) : F.I(0)))))); set { }
        }

        // Formula ReliesOnSingleMethod (rulebook: ={{ComplementaryMethodCount}} = 1)
        [NotMapped]
        public bool? ReliesOnSingleMethod
        {
            get => F.AsBool(F.Memo(this, "ReliesOnSingleMethod", () => F.Eq(F.Of(this.ComplementaryMethodCount), F.I(1)))); set { }
        }

        // Formula MissesARequiredElicitationMode (rulebook: =AND({{ComplementaryMethodCount}} > 0, OR({{WorkshopSessionCount}} = 0, {{InterviewSessionCount}} = 0, {{ObservationSessionCount}} = 0, {{ReconciledDivergenceCount}} = 0)))
        [NotMapped]
        public bool? MissesARequiredElicitationMode
        {
            get => F.AsBool(F.Memo(this, "MissesARequiredElicitationMode", () => F.And(F.Bool3(F.Cmp(F.Of(this.ComplementaryMethodCount), ">", F.I(0))), F.Bool3(F.Or(F.Bool3(F.Eq(F.Of(this.WorkshopSessionCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.InterviewSessionCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.ObservationSessionCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.ReconciledDivergenceCount), F.I(0)))))))); set { }
        }

        // Formula CriticalIncidentCount (rulebook: =COUNTIFS(CriticalIncidents!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public int? CriticalIncidentCount
        {
            get => F.AsInt(F.Memo(this, "CriticalIncidentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CriticalIncident>(base.SoAContext, "CriticalIncidents", __c => __c.CriticalIncidents), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId))))))); set { }
        }

        // Formula JudgmentUnprobedByIncidents (rulebook: =AND({{TacitJudgmentFragmentCount}} > 0, {{CriticalIncidentCount}} = 0))
        [NotMapped]
        public bool? JudgmentUnprobedByIncidents
        {
            get => F.AsBool(F.Memo(this, "JudgmentUnprobedByIncidents", () => F.And(F.Bool3(F.Cmp(F.Of(this.TacitJudgmentFragmentCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.CriticalIncidentCount), F.I(0)))))); set { }
        }

        // Formula SmeApprovalCount (rulebook: =COUNTIFS(RepresentationReviews!{{ProcedureVersion}}, {{ProcedureVersionId}}, RepresentationReviews!{{ReviewPurpose}}, "RepresentationApproval", RepresentationReviews!{{ReviewerIsAffectedSme}}, TRUE, RepresentationReviews!{{Decision}}, "Approved"))
        [NotMapped]
        public int? SmeApprovalCount
        {
            get => F.AsInt(F.Memo(this, "SmeApprovalCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RepresentationReview>(base.SoAContext, "RepresentationReviews", __c => __c.RepresentationReviews), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.ReviewPurpose), F.S("RepresentationApproval")) && F.CritLiteral(F.Of(__r.ReviewerIsAffectedSme), F.B(true)) && F.CritLiteral(F.Of(__r.Decision), F.S("Approved"))))))); set { }
        }

        // Formula SmeAiEvaluationCount (rulebook: =COUNTIFS(RepresentationReviews!{{ProcedureVersion}}, {{ProcedureVersionId}}, RepresentationReviews!{{ReviewPurpose}}, "AiEvaluation", RepresentationReviews!{{ReviewerIsAffectedSme}}, TRUE))
        [NotMapped]
        public int? SmeAiEvaluationCount
        {
            get => F.AsInt(F.Memo(this, "SmeAiEvaluationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RepresentationReview>(base.SoAContext, "RepresentationReviews", __c => __c.RepresentationReviews), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.ReviewPurpose), F.S("AiEvaluation")) && F.CritLiteral(F.Of(__r.ReviewerIsAffectedSme), F.B(true))))))); set { }
        }

        // Formula IsApprovedWithoutSmeSignoff (rulebook: =AND({{Status}} = "Approved", {{SmeApprovalCount}} = 0))
        [NotMapped]
        public bool? IsApprovedWithoutSmeSignoff
        {
            get => F.AsBool(F.Memo(this, "IsApprovedWithoutSmeSignoff", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved"))), F.Bool3(F.Eq(F.Of(this.SmeApprovalCount), F.I(0)))))); set { }
        }

        // Formula ExpertsEvaluateAiNotRepresentation (rulebook: =AND({{SmeAiEvaluationCount}} > 0, {{SmeApprovalCount}} = 0))
        [NotMapped]
        public bool? ExpertsEvaluateAiNotRepresentation
        {
            get => F.AsBool(F.Memo(this, "ExpertsEvaluateAiNotRepresentation", () => F.And(F.Bool3(F.Cmp(F.Of(this.SmeAiEvaluationCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.SmeApprovalCount), F.I(0)))))); set { }
        }

        // Formula KeSessionCount (rulebook: =COUNTIFS(ElicitationSessions!{{ProcedureVersion}}, {{ProcedureVersionId}}, ElicitationSessions!{{FacilitatorIsKnowledgeEngineer}}, TRUE))
        [NotMapped]
        public int? KeSessionCount
        {
            get => F.AsInt(F.Memo(this, "KeSessionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationSession>(base.SoAContext, "ElicitationSessions", __c => __c.ElicitationSessions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.FacilitatorIsKnowledgeEngineer), F.B(true))))))); set { }
        }

        // Formula KeFieldSessionCount (rulebook: =COUNTIFS(ElicitationSessions!{{ProcedureVersion}}, {{ProcedureVersionId}}, ElicitationSessions!{{IsKeFieldSession}}, TRUE))
        [NotMapped]
        public int? KeFieldSessionCount
        {
            get => F.AsInt(F.Memo(this, "KeFieldSessionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationSession>(base.SoAContext, "ElicitationSessions", __c => __c.ElicitationSessions), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.IsKeFieldSession), F.B(true))))))); set { }
        }

        // Formula IsStudiedOnlyFromTheDesk (rulebook: =AND({{KeSessionCount}} > 0, {{KeFieldSessionCount}} = 0))
        [NotMapped]
        public bool? IsStudiedOnlyFromTheDesk
        {
            get => F.AsBool(F.Memo(this, "IsStudiedOnlyFromTheDesk", () => F.And(F.Bool3(F.Cmp(F.Of(this.KeSessionCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.KeFieldSessionCount), F.I(0)))))); set { }
        }

        // Formula JudgmentHeldOutsideSopCount (rulebook: =COUNTIFS(KnowledgeHoldings!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeHoldings!{{IsJudgmentOutsideDocument}}, TRUE))
        [NotMapped]
        public int? JudgmentHeldOutsideSopCount
        {
            get => F.AsInt(F.Memo(this, "JudgmentHeldOutsideSopCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeHolding>(base.SoAContext, "KnowledgeHoldings", __c => __c.KnowledgeHoldings), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.IsJudgmentOutsideDocument), F.B(true))))))); set { }
        }

        // Formula TacitHoldingCount (rulebook: =COUNTIFS(KnowledgeHoldings!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeHoldings!{{KnowledgeForm}}, "Tacit"))
        [NotMapped]
        public int? TacitHoldingCount
        {
            get => F.AsInt(F.Memo(this, "TacitHoldingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeHolding>(base.SoAContext, "KnowledgeHoldings", __c => __c.KnowledgeHoldings), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.KnowledgeForm), F.S("Tacit"))))))); set { }
        }

        // Formula ExplicitHoldingCount (rulebook: =COUNTIFS(KnowledgeHoldings!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeHoldings!{{KnowledgeForm}}, "Explicit"))
        [NotMapped]
        public int? ExplicitHoldingCount
        {
            get => F.AsInt(F.Memo(this, "ExplicitHoldingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeHolding>(base.SoAContext, "KnowledgeHoldings", __c => __c.KnowledgeHoldings), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.KnowledgeForm), F.S("Explicit"))))))); set { }
        }

        // Formula TacitShareExceedsExplicit (rulebook: ={{TacitHoldingCount}} > {{ExplicitHoldingCount}})
        [NotMapped]
        public bool? TacitShareExceedsExplicit
        {
            get => F.AsBool(F.Memo(this, "TacitShareExceedsExplicit", () => F.Cmp(F.Of(this.TacitHoldingCount), ">", F.Of(this.ExplicitHoldingCount)))); set { }
        }

        // Formula HandsHeldCount (rulebook: =COUNTIFS(KnowledgeHoldings!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeHoldings!{{Carrier}}, "OperatorMemory", KnowledgeHoldings!{{IsUnformalizedProcessKnowledge}}, TRUE))
        [NotMapped]
        public int? HandsHeldCount
        {
            get => F.AsInt(F.Memo(this, "HandsHeldCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeHolding>(base.SoAContext, "KnowledgeHoldings", __c => __c.KnowledgeHoldings), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.Carrier), F.S("OperatorMemory")) && F.CritLiteral(F.Of(__r.IsUnformalizedProcessKnowledge), F.B(true))))))); set { }
        }

        // Formula NegotiatedPracticeCount (rulebook: =COUNTIFS(KnowledgeHoldings!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeHoldings!{{Carrier}}, "UnspokenCrewAgreement", KnowledgeHoldings!{{IsUnformalizedProcessKnowledge}}, TRUE))
        [NotMapped]
        public int? NegotiatedPracticeCount
        {
            get => F.AsInt(F.Memo(this, "NegotiatedPracticeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeHolding>(base.SoAContext, "KnowledgeHoldings", __c => __c.KnowledgeHoldings), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.Carrier), F.S("UnspokenCrewAgreement")) && F.CritLiteral(F.Of(__r.IsUnformalizedProcessKnowledge), F.B(true))))))); set { }
        }

        // Formula LivesInHandsSilenceAndNegotiation (rulebook: =AND({{HandsHeldCount}} > 0, {{JudgmentHeldOutsideSopCount}} > 0, {{NegotiatedPracticeCount}} > 0))
        [NotMapped]
        public bool? LivesInHandsSilenceAndNegotiation
        {
            get => F.AsBool(F.Memo(this, "LivesInHandsSilenceAndNegotiation", () => F.And(F.Bool3(F.Cmp(F.Of(this.HandsHeldCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.JudgmentHeldOutsideSopCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.NegotiatedPracticeCount), ">", F.I(0)))))); set { }
        }

        // Formula ProcessModelTraceCount (rulebook: =COUNTIFS(KnowledgeTraces!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeTraces!{{TargetKind}}, "ProcessModel", KnowledgeTraces!{{TraceRole}}, "Origin"))
        [NotMapped]
        public int? ProcessModelTraceCount
        {
            get => F.AsInt(F.Memo(this, "ProcessModelTraceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTrace>(base.SoAContext, "KnowledgeTraces", __c => __c.KnowledgeTraces), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.TargetKind), F.S("ProcessModel")) && F.CritLiteral(F.Of(__r.TraceRole), F.S("Origin"))))))); set { }
        }

        // Formula IsLiveModelUntraced (rulebook: =AND({{IsLive}}, {{ProcessModelTraceCount}} = 0))
        [NotMapped]
        public bool? IsLiveModelUntraced
        {
            get => F.AsBool(F.Memo(this, "IsLiveModelUntraced", () => F.And(F.Bool3(F.Of(this.IsLive)), F.Bool3(F.Eq(F.Of(this.ProcessModelTraceCount), F.I(0)))))); set { }
        }

        // Formula TrailingPracticeTraceCount (rulebook: =COUNTIFS(KnowledgeTraces!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeTraces!{{IsDocumentTrailingPractice}}, TRUE))
        [NotMapped]
        public int? TrailingPracticeTraceCount
        {
            get => F.AsInt(F.Memo(this, "TrailingPracticeTraceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTrace>(base.SoAContext, "KnowledgeTraces", __c => __c.KnowledgeTraces), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.IsDocumentTrailingPractice), F.B(true))))))); set { }
        }

        // Formula IsDocumentedBehindPractice (rulebook: ={{TrailingPracticeTraceCount}} > 0)
        [NotMapped]
        public bool? IsDocumentedBehindPractice
        {
            get => F.AsBool(F.Memo(this, "IsDocumentedBehindPractice", () => F.Cmp(F.Of(this.TrailingPracticeTraceCount), ">", F.I(0)))); set { }
        }

        public string? StandardizationDriver { get; set; }
        // Formula TacitFragmentCount (rulebook: =COUNTIFS(KnowledgeFragments!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeFragments!{{KnowledgeForm}}, "Tacit"))
        [NotMapped]
        public int? TacitFragmentCount
        {
            get => F.AsInt(F.Memo(this, "TacitFragmentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.KnowledgeForm), F.S("Tacit"))))))); set { }
        }

        // Formula IsStandardizedWithoutTacitCapture (rulebook: =AND({{StandardizationDriver}} = "Compliance", {{TacitFragmentCount}} = 0))
        [NotMapped]
        public bool? IsStandardizedWithoutTacitCapture
        {
            get => F.AsBool(F.Memo(this, "IsStandardizedWithoutTacitCapture", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.StandardizationDriver)), F.S("Compliance"))), F.Bool3(F.Eq(F.Of(this.TacitFragmentCount), F.I(0)))))); set { }
        }

        // Formula TacitFormFragmentCount (rulebook: =COUNTIFS(KnowledgeFragments!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeFragments!{{KnowledgeForm}}, "Tacit"))
        [NotMapped]
        public int? TacitFormFragmentCount
        {
            get => F.AsInt(F.Memo(this, "TacitFormFragmentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.KnowledgeForm), F.S("Tacit"))))))); set { }
        }

        // Formula SituatedJudgmentFragmentCount (rulebook: =COUNTIFS(KnowledgeFragments!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeFragments!{{KnowledgeForm}}, "SituatedJudgment"))
        [NotMapped]
        public int? SituatedJudgmentFragmentCount
        {
            get => F.AsInt(F.Memo(this, "SituatedJudgmentFragmentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.KnowledgeForm), F.S("SituatedJudgment"))))))); set { }
        }

        // Formula TacitJudgmentFragmentCount (rulebook: ={{TacitFormFragmentCount}} + {{SituatedJudgmentFragmentCount}})
        [NotMapped]
        public int? TacitJudgmentFragmentCount
        {
            get => F.AsInt(F.Memo(this, "TacitJudgmentFragmentCount", () => F.Integer(F.Add(F.Of(this.TacitFormFragmentCount), F.Of(this.SituatedJudgmentFragmentCount))))); set { }
        }

        // Formula FirstStep (rulebook: =MAXIFS(Steps!{{DeclaredFirstStepKey}}, Steps!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public string? FirstStep
        {
            get => F.AsString(F.Memo(this, "FirstStep", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)), __r => F.Of(__r.DeclaredFirstStepKey))))); set { }
        }

        // Formula FallbackStep (rulebook: =MAXIFS(Steps!{{DeclaredFallbackStepKey}}, Steps!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        [NotMapped]
        public string? FallbackStep
        {
            get => F.AsString(F.Memo(this, "FallbackStep", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)), __r => F.Of(__r.DeclaredFallbackStepKey))))); set { }
        }

        // Formula FirstStepDisagreesWithGraph (rulebook: =AND({{DeclaredFirstStepCount}} > 0, {{GraphEntryStepCount}} > 0, {{FirstStep}} <> {{EntryStepId}}))
        [NotMapped]
        public bool? FirstStepDisagreesWithGraph
        {
            get => F.AsBool(F.Memo(this, "FirstStepDisagreesWithGraph", () => F.And(F.Bool3(F.Cmp(F.Of(this.DeclaredFirstStepCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.GraphEntryStepCount), ">", F.I(0))), F.Bool3(F.Ne(F.Of(this.FirstStep), F.Of(this.EntryStepId)))))); set { }
        }

        // Formula DeclaredFirstStepCount (rulebook: =COUNTIFS(Steps!{{ProcedureVersion}}, {{ProcedureVersionId}}, Steps!{{IsDeclaredFirstStep}}, TRUE))
        [NotMapped]
        public int? DeclaredFirstStepCount
        {
            get => F.AsInt(F.Memo(this, "DeclaredFirstStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.IsDeclaredFirstStep), F.B(true))))))); set { }
        }

        // Formula GraphEntryStepCount (rulebook: =COUNTIFS(Steps!{{ProcedureVersion}}, {{ProcedureVersionId}}, Steps!{{IsEntryStep}}, TRUE))
        [NotMapped]
        public int? GraphEntryStepCount
        {
            get => F.AsInt(F.Memo(this, "GraphEntryStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.ProcedureVersionId)) && F.CritLiteral(F.Of(__r.IsEntryStep), F.B(true))))))); set { }
        }


        public string? Procedure { get; set; }
        public string? Status { get; set; }
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

        private LifecycleStatuse _lifecycleStatuse;

        [ForeignKey("Status")]
        public virtual LifecycleStatuse LifecycleStatuse
        {
            get
            {
                if (_lifecycleStatuse == null && !string.IsNullOrEmpty(Status))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LifecycleStatuse - no database context is set. Status: " + Status + ".");
                        }
                        return null;
                    }
                    _lifecycleStatuse = base.SoAContext.LifecycleStatuses.Find(Status);
                    if (_lifecycleStatuse != null)
                    {
                        base.SoAContext.Attach(_lifecycleStatuse);
                    }
                }
                return _lifecycleStatuse;
            }
            set
            {
                if (_lifecycleStatuse != value)
                {
                    _lifecycleStatuse = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_lifecycleStatuse != null)
                    {
                        Status = _lifecycleStatuse.LifecycleStatusId;
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

        private ObservableCollection<RoleAssignment> _roleAssignments;

        [InverseProperty("ProcedureVersion")]
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
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignments.Where(x => x.ForProcedureVersion == this.ProcedureVersionId).ToList<RoleAssignment>();
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
                    item.ForProcedureVersion = this.ProcedureVersionId;
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

        private ObservableCollection<Machine> _machines;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<Machine> Machines
        {
            get
            {
                if (_machines == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Machines - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _machines = new ObservableCollection<Machine>();
                    }
                    else
                    {
                        var items = base.SoAContext.Machines.Where(x => x.GoverningProcedureVersion == this.ProcedureVersionId).ToList<Machine>();
                        _machines = new ObservableCollection<Machine>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _machines.CollectionChanged += Machines_CollectionChanged;
                }
                return _machines;
            }
            private set
            {
                if (_machines != null)
                {
                    _machines.CollectionChanged -= Machines_CollectionChanged;
                }
                _machines = value;
                if (_machines != null)
                {
                    _machines.CollectionChanged += Machines_CollectionChanged;
                }
            }
        }

        private void Machines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Machine>())
                {
                    item.GoverningProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<AuthoringSubmission> _authoringSubmissions;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<AuthoringSubmission> AuthoringSubmissions
        {
            get
            {
                if (_authoringSubmissions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthoringSubmissions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _authoringSubmissions = new ObservableCollection<AuthoringSubmission>();
                    }
                    else
                    {
                        var items = base.SoAContext.AuthoringSubmissions.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<AuthoringSubmission>();
                        _authoringSubmissions = new ObservableCollection<AuthoringSubmission>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _authoringSubmissions.CollectionChanged += AuthoringSubmissions_CollectionChanged;
                }
                return _authoringSubmissions;
            }
            private set
            {
                if (_authoringSubmissions != null)
                {
                    _authoringSubmissions.CollectionChanged -= AuthoringSubmissions_CollectionChanged;
                }
                _authoringSubmissions = value;
                if (_authoringSubmissions != null)
                {
                    _authoringSubmissions.CollectionChanged += AuthoringSubmissions_CollectionChanged;
                }
            }
        }

        private void AuthoringSubmissions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AuthoringSubmission>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ProcessStage> _processStages;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<ProcessStage> ProcessStages
        {
            get
            {
                if (_processStages == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessStages - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _processStages = new ObservableCollection<ProcessStage>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessStages.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ProcessStage>();
                        _processStages = new ObservableCollection<ProcessStage>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _processStages.CollectionChanged += ProcessStages_CollectionChanged;
                }
                return _processStages;
            }
            private set
            {
                if (_processStages != null)
                {
                    _processStages.CollectionChanged -= ProcessStages_CollectionChanged;
                }
                _processStages = value;
                if (_processStages != null)
                {
                    _processStages.CollectionChanged += ProcessStages_CollectionChanged;
                }
            }
        }

        private void ProcessStages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessStage>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ProcedureLensView> _procedureLensViews;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ProcedureLensView> ProcedureLensViews
        {
            get
            {
                if (_procedureLensViews == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureLensViews - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _procedureLensViews = new ObservableCollection<ProcedureLensView>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureLensViews.Where(x => x.ProjectsVersion == this.ProcedureVersionId).ToList<ProcedureLensView>();
                        _procedureLensViews = new ObservableCollection<ProcedureLensView>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureLensViews.CollectionChanged += ProcedureLensViews_CollectionChanged;
                }
                return _procedureLensViews;
            }
            private set
            {
                if (_procedureLensViews != null)
                {
                    _procedureLensViews.CollectionChanged -= ProcedureLensViews_CollectionChanged;
                }
                _procedureLensViews = value;
                if (_procedureLensViews != null)
                {
                    _procedureLensViews.CollectionChanged += ProcedureLensViews_CollectionChanged;
                }
            }
        }

        private void ProcedureLensViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureLensView>())
                {
                    item.ProjectsVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<CollectedSourceMaterial> _collectedSourceMaterials;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<CollectedSourceMaterial> CollectedSourceMaterials
        {
            get
            {
                if (_collectedSourceMaterials == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CollectedSourceMaterials - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>();
                    }
                    else
                    {
                        var items = base.SoAContext.CollectedSourceMaterials.Where(x => x.EncodedIntoVersion == this.ProcedureVersionId).ToList<CollectedSourceMaterial>();
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _collectedSourceMaterials.CollectionChanged += CollectedSourceMaterials_CollectionChanged;
                }
                return _collectedSourceMaterials;
            }
            private set
            {
                if (_collectedSourceMaterials != null)
                {
                    _collectedSourceMaterials.CollectionChanged -= CollectedSourceMaterials_CollectionChanged;
                }
                _collectedSourceMaterials = value;
                if (_collectedSourceMaterials != null)
                {
                    _collectedSourceMaterials.CollectionChanged += CollectedSourceMaterials_CollectionChanged;
                }
            }
        }

        private void CollectedSourceMaterials_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CollectedSourceMaterial>())
                {
                    item.EncodedIntoVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ConsumerSystemSync> _consumerSystemSyncs;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ConsumerSystemSync> ConsumerSystemSyncs
        {
            get
            {
                if (_consumerSystemSyncs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConsumerSystemSyncs - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _consumerSystemSyncs = new ObservableCollection<ConsumerSystemSync>();
                    }
                    else
                    {
                        var items = base.SoAContext.ConsumerSystemSyncs.Where(x => x.LoadedVersion == this.ProcedureVersionId).ToList<ConsumerSystemSync>();
                        _consumerSystemSyncs = new ObservableCollection<ConsumerSystemSync>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _consumerSystemSyncs.CollectionChanged += ConsumerSystemSyncs_CollectionChanged;
                }
                return _consumerSystemSyncs;
            }
            private set
            {
                if (_consumerSystemSyncs != null)
                {
                    _consumerSystemSyncs.CollectionChanged -= ConsumerSystemSyncs_CollectionChanged;
                }
                _consumerSystemSyncs = value;
                if (_consumerSystemSyncs != null)
                {
                    _consumerSystemSyncs.CollectionChanged += ConsumerSystemSyncs_CollectionChanged;
                }
            }
        }

        private void ConsumerSystemSyncs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ConsumerSystemSync>())
                {
                    item.LoadedVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<SnapshotAssertion> _snapshotAssertions;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<SnapshotAssertion> SnapshotAssertions
        {
            get
            {
                if (_snapshotAssertions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SnapshotAssertions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _snapshotAssertions = new ObservableCollection<SnapshotAssertion>();
                    }
                    else
                    {
                        var items = base.SoAContext.SnapshotAssertions.Where(x => x.SourceProcedureVersion == this.ProcedureVersionId).ToList<SnapshotAssertion>();
                        _snapshotAssertions = new ObservableCollection<SnapshotAssertion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _snapshotAssertions.CollectionChanged += SnapshotAssertions_CollectionChanged;
                }
                return _snapshotAssertions;
            }
            private set
            {
                if (_snapshotAssertions != null)
                {
                    _snapshotAssertions.CollectionChanged -= SnapshotAssertions_CollectionChanged;
                }
                _snapshotAssertions = value;
                if (_snapshotAssertions != null)
                {
                    _snapshotAssertions.CollectionChanged += SnapshotAssertions_CollectionChanged;
                }
            }
        }

        private void SnapshotAssertions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SnapshotAssertion>())
                {
                    item.SourceProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<RetrievalSegment> _retrievalSegments;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<RetrievalSegment> RetrievalSegments
        {
            get
            {
                if (_retrievalSegments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RetrievalSegments - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _retrievalSegments = new ObservableCollection<RetrievalSegment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RetrievalSegments.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<RetrievalSegment>();
                        _retrievalSegments = new ObservableCollection<RetrievalSegment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _retrievalSegments.CollectionChanged += RetrievalSegments_CollectionChanged;
                }
                return _retrievalSegments;
            }
            private set
            {
                if (_retrievalSegments != null)
                {
                    _retrievalSegments.CollectionChanged -= RetrievalSegments_CollectionChanged;
                }
                _retrievalSegments = value;
                if (_retrievalSegments != null)
                {
                    _retrievalSegments.CollectionChanged += RetrievalSegments_CollectionChanged;
                }
            }
        }

        private void RetrievalSegments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RetrievalSegment>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<KnowledgeQueryDefinition> _knowledgeQueryDefinitions;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<KnowledgeQueryDefinition> KnowledgeQueryDefinitions
        {
            get
            {
                if (_knowledgeQueryDefinitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeQueryDefinitions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _knowledgeQueryDefinitions = new ObservableCollection<KnowledgeQueryDefinition>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeQueryDefinitions.Where(x => x.TargetProcedureVersion == this.ProcedureVersionId).ToList<KnowledgeQueryDefinition>();
                        _knowledgeQueryDefinitions = new ObservableCollection<KnowledgeQueryDefinition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeQueryDefinitions.CollectionChanged += KnowledgeQueryDefinitions_CollectionChanged;
                }
                return _knowledgeQueryDefinitions;
            }
            private set
            {
                if (_knowledgeQueryDefinitions != null)
                {
                    _knowledgeQueryDefinitions.CollectionChanged -= KnowledgeQueryDefinitions_CollectionChanged;
                }
                _knowledgeQueryDefinitions = value;
                if (_knowledgeQueryDefinitions != null)
                {
                    _knowledgeQueryDefinitions.CollectionChanged += KnowledgeQueryDefinitions_CollectionChanged;
                }
            }
        }

        private void KnowledgeQueryDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeQueryDefinition>())
                {
                    item.TargetProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<PromptTemplate> _promptTemplates;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<PromptTemplate> PromptTemplates
        {
            get
            {
                if (_promptTemplates == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PromptTemplates - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _promptTemplates = new ObservableCollection<PromptTemplate>();
                    }
                    else
                    {
                        var items = base.SoAContext.PromptTemplates.Where(x => x.SourceProcedureVersion == this.ProcedureVersionId).ToList<PromptTemplate>();
                        _promptTemplates = new ObservableCollection<PromptTemplate>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _promptTemplates.CollectionChanged += PromptTemplates_CollectionChanged;
                }
                return _promptTemplates;
            }
            private set
            {
                if (_promptTemplates != null)
                {
                    _promptTemplates.CollectionChanged -= PromptTemplates_CollectionChanged;
                }
                _promptTemplates = value;
                if (_promptTemplates != null)
                {
                    _promptTemplates.CollectionChanged += PromptTemplates_CollectionChanged;
                }
            }
        }

        private void PromptTemplates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<PromptTemplate>())
                {
                    item.SourceProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<KnowledgeProjection> _knowledgeProjections;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<KnowledgeProjection> KnowledgeProjections
        {
            get
            {
                if (_knowledgeProjections == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeProjections - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _knowledgeProjections = new ObservableCollection<KnowledgeProjection>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeProjections.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<KnowledgeProjection>();
                        _knowledgeProjections = new ObservableCollection<KnowledgeProjection>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeProjections.CollectionChanged += KnowledgeProjections_CollectionChanged;
                }
                return _knowledgeProjections;
            }
            private set
            {
                if (_knowledgeProjections != null)
                {
                    _knowledgeProjections.CollectionChanged -= KnowledgeProjections_CollectionChanged;
                }
                _knowledgeProjections = value;
                if (_knowledgeProjections != null)
                {
                    _knowledgeProjections.CollectionChanged += KnowledgeProjections_CollectionChanged;
                }
            }
        }

        private void KnowledgeProjections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeProjection>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ModelAnnotation> _modelAnnotations;

        [InverseProperty("ProcedureVersionRef")]
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
                            throw new InvalidOperationException("Cannot access ModelAnnotations - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _modelAnnotations = new ObservableCollection<ModelAnnotation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelAnnotations.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ModelAnnotation>();
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
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<KnowledgeSearchEvent> _knowledgeSearchEvents;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<KnowledgeSearchEvent> KnowledgeSearchEvents
        {
            get
            {
                if (_knowledgeSearchEvents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeSearchEvents - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _knowledgeSearchEvents = new ObservableCollection<KnowledgeSearchEvent>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeSearchEvents.Where(x => x.SoughtProcedureVersion == this.ProcedureVersionId).ToList<KnowledgeSearchEvent>();
                        _knowledgeSearchEvents = new ObservableCollection<KnowledgeSearchEvent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeSearchEvents.CollectionChanged += KnowledgeSearchEvents_CollectionChanged;
                }
                return _knowledgeSearchEvents;
            }
            private set
            {
                if (_knowledgeSearchEvents != null)
                {
                    _knowledgeSearchEvents.CollectionChanged -= KnowledgeSearchEvents_CollectionChanged;
                }
                _knowledgeSearchEvents = value;
                if (_knowledgeSearchEvents != null)
                {
                    _knowledgeSearchEvents.CollectionChanged += KnowledgeSearchEvents_CollectionChanged;
                }
            }
        }

        private void KnowledgeSearchEvents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeSearchEvent>())
                {
                    item.SoughtProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<AiAdoptionInitiatif> _aiAdoptionInitiatives;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<AiAdoptionInitiatif> AiAdoptionInitiatives
        {
            get
            {
                if (_aiAdoptionInitiatives == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiAdoptionInitiatives - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _aiAdoptionInitiatives = new ObservableCollection<AiAdoptionInitiatif>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiAdoptionInitiatives.Where(x => x.TargetVersion == this.ProcedureVersionId).ToList<AiAdoptionInitiatif>();
                        _aiAdoptionInitiatives = new ObservableCollection<AiAdoptionInitiatif>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiAdoptionInitiatives.CollectionChanged += AiAdoptionInitiatives_CollectionChanged;
                }
                return _aiAdoptionInitiatives;
            }
            private set
            {
                if (_aiAdoptionInitiatives != null)
                {
                    _aiAdoptionInitiatives.CollectionChanged -= AiAdoptionInitiatives_CollectionChanged;
                }
                _aiAdoptionInitiatives = value;
                if (_aiAdoptionInitiatives != null)
                {
                    _aiAdoptionInitiatives.CollectionChanged += AiAdoptionInitiatives_CollectionChanged;
                }
            }
        }

        private void AiAdoptionInitiatives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiAdoptionInitiatif>())
                {
                    item.TargetVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<KnowledgeOutcomeMeasurement> _knowledgeOutcomeMeasurements;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<KnowledgeOutcomeMeasurement> KnowledgeOutcomeMeasurements
        {
            get
            {
                if (_knowledgeOutcomeMeasurements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeOutcomeMeasurements - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _knowledgeOutcomeMeasurements = new ObservableCollection<KnowledgeOutcomeMeasurement>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeOutcomeMeasurements.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<KnowledgeOutcomeMeasurement>();
                        _knowledgeOutcomeMeasurements = new ObservableCollection<KnowledgeOutcomeMeasurement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeOutcomeMeasurements.CollectionChanged += KnowledgeOutcomeMeasurements_CollectionChanged;
                }
                return _knowledgeOutcomeMeasurements;
            }
            private set
            {
                if (_knowledgeOutcomeMeasurements != null)
                {
                    _knowledgeOutcomeMeasurements.CollectionChanged -= KnowledgeOutcomeMeasurements_CollectionChanged;
                }
                _knowledgeOutcomeMeasurements = value;
                if (_knowledgeOutcomeMeasurements != null)
                {
                    _knowledgeOutcomeMeasurements.CollectionChanged += KnowledgeOutcomeMeasurements_CollectionChanged;
                }
            }
        }

        private void KnowledgeOutcomeMeasurements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeOutcomeMeasurement>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<AiInsightProposal> _aiInsightProposals;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<AiInsightProposal> AiInsightProposals
        {
            get
            {
                if (_aiInsightProposals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiInsightProposals - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _aiInsightProposals = new ObservableCollection<AiInsightProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiInsightProposals.Where(x => x.TargetProcedureVersion == this.ProcedureVersionId).ToList<AiInsightProposal>();
                        _aiInsightProposals = new ObservableCollection<AiInsightProposal>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiInsightProposals.CollectionChanged += AiInsightProposals_CollectionChanged;
                }
                return _aiInsightProposals;
            }
            private set
            {
                if (_aiInsightProposals != null)
                {
                    _aiInsightProposals.CollectionChanged -= AiInsightProposals_CollectionChanged;
                }
                _aiInsightProposals = value;
                if (_aiInsightProposals != null)
                {
                    _aiInsightProposals.CollectionChanged += AiInsightProposals_CollectionChanged;
                }
            }
        }

        private void AiInsightProposals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiInsightProposal>())
                {
                    item.TargetProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ProcessDesignDecision> _processDesignDecisions;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<ProcessDesignDecision> ProcessDesignDecisions
        {
            get
            {
                if (_processDesignDecisions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessDesignDecisions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _processDesignDecisions = new ObservableCollection<ProcessDesignDecision>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessDesignDecisions.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ProcessDesignDecision>();
                        _processDesignDecisions = new ObservableCollection<ProcessDesignDecision>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _processDesignDecisions.CollectionChanged += ProcessDesignDecisions_CollectionChanged;
                }
                return _processDesignDecisions;
            }
            private set
            {
                if (_processDesignDecisions != null)
                {
                    _processDesignDecisions.CollectionChanged -= ProcessDesignDecisions_CollectionChanged;
                }
                _processDesignDecisions = value;
                if (_processDesignDecisions != null)
                {
                    _processDesignDecisions.CollectionChanged += ProcessDesignDecisions_CollectionChanged;
                }
            }
        }

        private void ProcessDesignDecisions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessDesignDecision>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<DriftObservation> _driftObservations;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<DriftObservation> DriftObservations
        {
            get
            {
                if (_driftObservations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DriftObservations - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _driftObservations = new ObservableCollection<DriftObservation>();
                    }
                    else
                    {
                        var items = base.SoAContext.DriftObservations.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<DriftObservation>();
                        _driftObservations = new ObservableCollection<DriftObservation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _driftObservations.CollectionChanged += DriftObservations_CollectionChanged;
                }
                return _driftObservations;
            }
            private set
            {
                if (_driftObservations != null)
                {
                    _driftObservations.CollectionChanged -= DriftObservations_CollectionChanged;
                }
                _driftObservations = value;
                if (_driftObservations != null)
                {
                    _driftObservations.CollectionChanged += DriftObservations_CollectionChanged;
                }
            }
        }

        private void DriftObservations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DriftObservation>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<CriticalIncident> _criticalIncidents;

        [InverseProperty("ProcedureVersionRef")]
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
                            throw new InvalidOperationException("Cannot access CriticalIncidents - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _criticalIncidents = new ObservableCollection<CriticalIncident>();
                    }
                    else
                    {
                        var items = base.SoAContext.CriticalIncidents.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<CriticalIncident>();
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
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<RepresentationReview> _representationReviews;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<RepresentationReview> RepresentationReviews
        {
            get
            {
                if (_representationReviews == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RepresentationReviews - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _representationReviews = new ObservableCollection<RepresentationReview>();
                    }
                    else
                    {
                        var items = base.SoAContext.RepresentationReviews.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<RepresentationReview>();
                        _representationReviews = new ObservableCollection<RepresentationReview>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _representationReviews.CollectionChanged += RepresentationReviews_CollectionChanged;
                }
                return _representationReviews;
            }
            private set
            {
                if (_representationReviews != null)
                {
                    _representationReviews.CollectionChanged -= RepresentationReviews_CollectionChanged;
                }
                _representationReviews = value;
                if (_representationReviews != null)
                {
                    _representationReviews.CollectionChanged += RepresentationReviews_CollectionChanged;
                }
            }
        }

        private void RepresentationReviews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RepresentationReview>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<WorkflowViewDivergence> _workflowViewDivergences;

        [InverseProperty("ProcedureVersionRef")]
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
                            throw new InvalidOperationException("Cannot access WorkflowViewDivergences - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _workflowViewDivergences = new ObservableCollection<WorkflowViewDivergence>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowViewDivergences.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<WorkflowViewDivergence>();
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
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<KnowledgeHolding> _knowledgeHoldings;

        [InverseProperty("ProcedureVersionRef")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeHoldings - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _knowledgeHoldings = new ObservableCollection<KnowledgeHolding>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeHoldings.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<KnowledgeHolding>();
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
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<KnowledgeTrace> _knowledgeTraces;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<KnowledgeTrace> KnowledgeTraces
        {
            get
            {
                if (_knowledgeTraces == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeTraces - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _knowledgeTraces = new ObservableCollection<KnowledgeTrace>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTraces.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<KnowledgeTrace>();
                        _knowledgeTraces = new ObservableCollection<KnowledgeTrace>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeTraces.CollectionChanged += KnowledgeTraces_CollectionChanged;
                }
                return _knowledgeTraces;
            }
            private set
            {
                if (_knowledgeTraces != null)
                {
                    _knowledgeTraces.CollectionChanged -= KnowledgeTraces_CollectionChanged;
                }
                _knowledgeTraces = value;
                if (_knowledgeTraces != null)
                {
                    _knowledgeTraces.CollectionChanged += KnowledgeTraces_CollectionChanged;
                }
            }
        }

        private void KnowledgeTraces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeTrace>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<StakeholderPerspectif> _stakeholderPerspectives;

        [InverseProperty("ProcedureVersionRef")]
        public virtual ObservableCollection<StakeholderPerspectif> StakeholderPerspectives
        {
            get
            {
                if (_stakeholderPerspectives == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StakeholderPerspectives - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _stakeholderPerspectives = new ObservableCollection<StakeholderPerspectif>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderPerspectives.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<StakeholderPerspectif>();
                        _stakeholderPerspectives = new ObservableCollection<StakeholderPerspectif>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stakeholderPerspectives.CollectionChanged += StakeholderPerspectives_CollectionChanged;
                }
                return _stakeholderPerspectives;
            }
            private set
            {
                if (_stakeholderPerspectives != null)
                {
                    _stakeholderPerspectives.CollectionChanged -= StakeholderPerspectives_CollectionChanged;
                }
                _stakeholderPerspectives = value;
                if (_stakeholderPerspectives != null)
                {
                    _stakeholderPerspectives.CollectionChanged += StakeholderPerspectives_CollectionChanged;
                }
            }
        }

        private void StakeholderPerspectives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StakeholderPerspectif>())
                {
                    item.ProcedureVersion = this.ProcedureVersionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.LifecycleStatuse;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.EvaluationContextRef;
            _ = this.RoleAssignments;
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
            _ = this.Machines;
            _ = this.AuthoringSubmissions;
            _ = this.ProcessStages;
            _ = this.ProcedureLensViews;
            _ = this.CollectedSourceMaterials;
            _ = this.ConsumerSystemSyncs;
            _ = this.SnapshotAssertions;
            _ = this.RetrievalSegments;
            _ = this.KnowledgeQueryDefinitions;
            _ = this.PromptTemplates;
            _ = this.KnowledgeProjections;
            _ = this.ModelAnnotations;
            _ = this.KnowledgeSearchEvents;
            _ = this.AiAdoptionInitiatives;
            _ = this.KnowledgeOutcomeMeasurements;
            _ = this.AiInsightProposals;
            _ = this.ProcessDesignDecisions;
            _ = this.DriftObservations;
            _ = this.CriticalIncidents;
            _ = this.RepresentationReviews;
            _ = this.WorkflowViewDivergences;
            _ = this.KnowledgeHoldings;
            _ = this.KnowledgeTraces;
            _ = this.StakeholderPerspectives;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
