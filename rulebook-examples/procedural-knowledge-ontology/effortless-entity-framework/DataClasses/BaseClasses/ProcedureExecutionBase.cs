
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
    [Table("ProcedureExecutions")]
    public class ProcedureExecutionBase : SoAEntityBase
    {
        [Key]
        public string ProcedureExecutionId { get; set; }

        // Formula Name (rulebook: ={{ProcedureVersion}} & " / " & {{Context}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProcedureVersion)), F.S(" / "), F.Text(F.Of(this.Context))))); set { }
        }

        public string? ExecutionStatus { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }
        public string? Context { get; set; }
        public string? OperationalRecordUri { get; set; }
        // Formula ExpectedStepCount (rulebook: =INDEX(ProcedureVersions!{{SpecifiedStepCount}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public decimal? ExpectedStepCount
        {
            get => F.AsDecimal(F.Memo(this, "ExpectedStepCount", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.SpecifiedStepCount), () => F.Of(new ProcedureVersion().SpecifiedStepCount)))); set { }
        }

        // Formula CompletedStepCount (rulebook: =COUNTIFS(StepExecutions!{{CompletedExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? CompletedStepCount
        {
            get => F.AsDecimal(F.Memo(this, "CompletedStepCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.CompletedExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula ControlBreachCount (rulebook: =COUNTIFS(StepExecutions!{{ControlBreachExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? ControlBreachCount
        {
            get => F.AsDecimal(F.Memo(this, "ControlBreachCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.ControlBreachExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula LateStepCount (rulebook: =COUNTIFS(StepExecutions!{{LateExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? LateStepCount
        {
            get => F.AsDecimal(F.Memo(this, "LateStepCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.LateExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula IsStructurallyComplete (rulebook: ={{CompletedStepCount}} >= {{ExpectedStepCount}})
        [NotMapped]
        public bool? IsStructurallyComplete
        {
            get => F.AsBool(F.Memo(this, "IsStructurallyComplete", () => F.Cmp(F.Of(this.CompletedStepCount), ">=", F.Of(this.ExpectedStepCount)))); set { }
        }

        // Formula DivergedFromSpecification (rulebook: =OR(NOT({{IsStructurallyComplete}}), {{ControlBreachCount}} > 0))
        [NotMapped]
        public bool? DivergedFromSpecification
        {
            get => F.AsBool(F.Memo(this, "DivergedFromSpecification", () => F.Or(F.Bool3(F.Not(F.Bool3(F.Of(this.IsStructurallyComplete)))), F.Bool3(F.Cmp(F.Of(this.ControlBreachCount), ">", F.I(0)))))); set { }
        }

        // Formula AllBlockingControlsEvaluated (rulebook: ={{UnevaluatedBlockingTotal}} = 0)
        [NotMapped]
        public bool? AllBlockingControlsEvaluated
        {
            get => F.AsBool(F.Memo(this, "AllBlockingControlsEvaluated", () => F.Eq(F.Of(this.UnevaluatedBlockingTotal), F.I(0)))); set { }
        }

        // Formula UnevaluatedBlockingTotal (rulebook: =COUNTIFS(StepExecutions!{{UnevaluatedBlockingExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? UnevaluatedBlockingTotal
        {
            get => F.AsDecimal(F.Memo(this, "UnevaluatedBlockingTotal", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.UnevaluatedBlockingExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula SeparationOfDutiesHeld (rulebook: ={{SeparationViolationCount}} = 0)
        [NotMapped]
        public bool? SeparationOfDutiesHeld
        {
            get => F.AsBool(F.Memo(this, "SeparationOfDutiesHeld", () => F.Eq(F.Of(this.SeparationViolationCount), F.I(0)))); set { }
        }

        // Formula SeparationViolationCount (rulebook: =COUNTIFS(StepExecutions!{{SeparationViolationExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? SeparationViolationCount
        {
            get => F.AsDecimal(F.Memo(this, "SeparationViolationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.SeparationViolationExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula IsAttestationReady (rulebook: =AND({{IsStructurallyComplete}}, NOT({{DivergedFromSpecification}}), {{AllBlockingControlsEvaluated}}, {{SeparationOfDutiesHeld}}))
        [NotMapped]
        public bool? IsAttestationReady
        {
            get => F.AsBool(F.Memo(this, "IsAttestationReady", () => F.And(F.Bool3(F.Of(this.IsStructurallyComplete)), F.Bool3(F.Not(F.Bool3(F.Of(this.DivergedFromSpecification)))), F.Bool3(F.Of(this.AllBlockingControlsEvaluated)), F.Bool3(F.Of(this.SeparationOfDutiesHeld))))); set { }
        }

        // Formula AttestationBlockerSummary (rulebook: =IF({{IsAttestationReady}}, "", IF(NOT({{IsStructurallyComplete}}), "Incomplete: specified steps did not all complete.", IF({{SeparationViolationCount}} > 0, "Segregation of duties violated.", IF({{UnevaluatedBlockingTotal}} > 0, "Blocking controls were never evaluated.", "Control breach recorded on one or more steps.")))))
        [NotMapped]
        public string? AttestationBlockerSummary
        {
            get => F.AsString(F.Memo(this, "AttestationBlockerSummary", () => (F.Truthy(F.Bool3(F.Of(this.IsAttestationReady))) ? F.S("") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.Of(this.IsStructurallyComplete))))) ? F.S("Incomplete: specified steps did not all complete.") : (F.Truthy(F.Bool3(F.Cmp(F.Of(this.SeparationViolationCount), ">", F.I(0)))) ? F.S("Segregation of duties violated.") : (F.Truthy(F.Bool3(F.Cmp(F.Of(this.UnevaluatedBlockingTotal), ">", F.I(0)))) ? F.S("Blocking controls were never evaluated.") : F.S("Control breach recorded on one or more steps."))))))); set { }
        }

        // Formula ExecutedVersionIsFit (rulebook: =INDEX(ProcedureVersions!{{IsFitToExecute}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public bool? ExecutedVersionIsFit
        {
            get => F.AsBool(F.Memo(this, "ExecutedVersionIsFit", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.IsFitToExecute), () => F.Of(new ProcedureVersion().IsFitToExecute)))); set { }
        }

        // Formula SignedAgainstUnfitVersion (rulebook: =AND({{ExecutionStatus}} = "Completed", NOT({{ExecutedVersionIsFit}})))
        [NotMapped]
        public bool? SignedAgainstUnfitVersion
        {
            get => F.AsBool(F.Memo(this, "SignedAgainstUnfitVersion", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ExecutionStatus)), F.S("Completed"))), F.Bool3(F.Not(F.Bool3(F.Of(this.ExecutedVersionIsFit))))))); set { }
        }

        // Formula AssertedOnlyControlCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{AssertedOnlyExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? AssertedOnlyControlCount
        {
            get => F.AsDecimal(F.Memo(this, "AssertedOnlyControlCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.AssertedOnlyExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula AssuranceIsMostlyAsserted (rulebook: ={{AssertedOnlyControlCount}} > 0)
        [NotMapped]
        public bool? AssuranceIsMostlyAsserted
        {
            get => F.AsBool(F.Memo(this, "AssuranceIsMostlyAsserted", () => F.Cmp(F.Of(this.AssertedOnlyControlCount), ">", F.I(0)))); set { }
        }

        // Formula UnreachableHandlingFailureCount (rulebook: =COUNTIFS(MessageDeliveries!{{UnreachableFailureKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? UnreachableHandlingFailureCount
        {
            get => F.AsDecimal(F.Memo(this, "UnreachableHandlingFailureCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MessageDelivery>(base.SoAContext, "MessageDeliveries", __c => __c.MessageDeliveries), __r => F.CritField(F.Of(__r.UnreachableFailureKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula RetentionBreachCount (rulebook: =COUNTIFS(MessageDeliveries!{{RetentionBreachExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? RetentionBreachCount
        {
            get => F.AsDecimal(F.Memo(this, "RetentionBreachCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MessageDelivery>(base.SoAContext, "MessageDeliveries", __c => __c.MessageDeliveries), __r => F.CritField(F.Of(__r.RetentionBreachExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula ClearedLegalReviewCount (rulebook: =COUNTIFS(StepExecutions!{{ClearedLegalReviewKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? ClearedLegalReviewCount
        {
            get => F.AsDecimal(F.Memo(this, "ClearedLegalReviewCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.ClearedLegalReviewKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula HasClearedLegalReview (rulebook: ={{ClearedLegalReviewCount}} > 0)
        [NotMapped]
        public bool? HasClearedLegalReview
        {
            get => F.AsBool(F.Memo(this, "HasClearedLegalReview", () => F.Cmp(F.Of(this.ClearedLegalReviewCount), ">", F.I(0)))); set { }
        }

        // Formula AbandonedFailureCount (rulebook: =COUNTIFS(MessageDeliveries!{{AbandonedFailureExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? AbandonedFailureCount
        {
            get => F.AsDecimal(F.Memo(this, "AbandonedFailureCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MessageDelivery>(base.SoAContext, "MessageDeliveries", __c => __c.MessageDeliveries), __r => F.CritField(F.Of(__r.AbandonedFailureExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula DeliveredCount (rulebook: =COUNTIFS(MessageDeliveries!{{ReachedExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? DeliveredCount
        {
            get => F.AsDecimal(F.Memo(this, "DeliveredCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MessageDelivery>(base.SoAContext, "MessageDeliveries", __c => __c.MessageDeliveries), __r => F.CritField(F.Of(__r.ReachedExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula TotalDeliveryAttemptCount (rulebook: =COUNTIFS(MessageDeliveries!{{ProcedureExecution}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? TotalDeliveryAttemptCount
        {
            get => F.AsDecimal(F.Memo(this, "TotalDeliveryAttemptCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MessageDelivery>(base.SoAContext, "MessageDeliveries", __c => __c.MessageDeliveries), __r => F.CritField(F.Of(__r.ProcedureExecution), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula HasAbandonedFailures (rulebook: ={{AbandonedFailureCount}} > 0)
        [NotMapped]
        public bool? HasAbandonedFailures
        {
            get => F.AsBool(F.Memo(this, "HasAbandonedFailures", () => F.Cmp(F.Of(this.AbandonedFailureCount), ">", F.I(0)))); set { }
        }

        // Formula MishandledRefusalCount (rulebook: =COUNTIFS(SendIntents!{{RefusalFailureExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? MishandledRefusalCount
        {
            get => F.AsDecimal(F.Memo(this, "MishandledRefusalCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SendIntent>(base.SoAContext, "SendIntents", __c => __c.SendIntents), __r => F.CritField(F.Of(__r.RefusalFailureExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula UncleanStepCount (rulebook: =COUNTIFS(StepExecutions!{{ProcedureExecutionWhenUnclean}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? UncleanStepCount
        {
            get => F.AsDecimal(F.Memo(this, "UncleanStepCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.ProcedureExecutionWhenUnclean), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula RanClean (rulebook: ={{UncleanStepCount}} = 0)
        [NotMapped]
        public bool? RanClean
        {
            get => F.AsBool(F.Memo(this, "RanClean", () => F.Eq(F.Of(this.UncleanStepCount), F.I(0)))); set { }
        }

        // Formula CountOfApprovalExecutions (rulebook: =COUNTIFS(StepExecutions!{{IsApprovalExecution}}, TRUE))
        [NotMapped]
        public int? CountOfApprovalExecutions
        {
            get => F.AsInt(F.Memo(this, "CountOfApprovalExecutions", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritLiteral(F.Of(__r.IsApprovalExecution), F.B(true))))))); set { }
        }

        // Formula HasHumanApproval (rulebook: ={{CountOfApprovalExecutions}} > 0)
        [NotMapped]
        public bool? HasHumanApproval
        {
            get => F.AsBool(F.Memo(this, "HasHumanApproval", () => F.Cmp(F.Of(this.CountOfApprovalExecutions), ">", F.I(0)))); set { }
        }

        // Formula CountOfDeliveryExecutions (rulebook: =COUNTIFS(StepExecutions!{{Step}}, "policy-07"))
        [NotMapped]
        public int? CountOfDeliveryExecutions
        {
            get => F.AsInt(F.Memo(this, "CountOfDeliveryExecutions", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritLiteral(F.Of(__r.Step), F.S("policy-07"))))))); set { }
        }

        // Formula HasDelivered (rulebook: ={{CountOfDeliveryExecutions}} > 0)
        [NotMapped]
        public bool? HasDelivered
        {
            get => F.AsBool(F.Memo(this, "HasDelivered", () => F.Cmp(F.Of(this.CountOfDeliveryExecutions), ">", F.I(0)))); set { }
        }

        // Formula DeliveredWithoutApproval (rulebook: =AND({{HasDelivered}}, NOT({{HasHumanApproval}})))
        [NotMapped]
        public bool? DeliveredWithoutApproval
        {
            get => F.AsBool(F.Memo(this, "DeliveredWithoutApproval", () => F.And(F.Bool3(F.Of(this.HasDelivered)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasHumanApproval))))))); set { }
        }

        // Formula InvalidApprovalCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{RunWhenInvalidApproval}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? InvalidApprovalCount
        {
            get => F.AsDecimal(F.Memo(this, "InvalidApprovalCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.RunWhenInvalidApproval), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula ApprovalChainIsComplete (rulebook: ={{InvalidApprovalCount}} = 0)
        [NotMapped]
        public bool? ApprovalChainIsComplete
        {
            get => F.AsBool(F.Memo(this, "ApprovalChainIsComplete", () => F.Eq(F.Of(this.InvalidApprovalCount), F.I(0)))); set { }
        }

        // Formula VacuouslyCleanStepCount (rulebook: =COUNTIFS(StepExecutions!{{VacuouslyCleanExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? VacuouslyCleanStepCount
        {
            get => F.AsDecimal(F.Memo(this, "VacuouslyCleanStepCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.VacuouslyCleanExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula PreparationStepCount (rulebook: =COUNTIFS(StepExecutions!{{PreparationExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? PreparationStepCount
        {
            get => F.AsDecimal(F.Memo(this, "PreparationStepCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.PreparationExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula ApprovalStepCount (rulebook: =COUNTIFS(StepExecutions!{{ApprovalExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? ApprovalStepCount
        {
            get => F.AsDecimal(F.Memo(this, "ApprovalStepCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.ApprovalExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula SeparationWasTestable (rulebook: =AND({{PreparationStepCount}} > 0, {{ApprovalStepCount}} > 0))
        [NotMapped]
        public bool? SeparationWasTestable
        {
            get => F.AsBool(F.Memo(this, "SeparationWasTestable", () => F.And(F.Bool3(F.Cmp(F.Of(this.PreparationStepCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.ApprovalStepCount), ">", F.I(0)))))); set { }
        }

        // Formula SeparationHeldUnderTest (rulebook: =AND({{SeparationWasTestable}}, {{SeparationOfDutiesHeld}}))
        [NotMapped]
        public bool? SeparationHeldUnderTest
        {
            get => F.AsBool(F.Memo(this, "SeparationHeldUnderTest", () => F.And(F.Bool3(F.Of(this.SeparationWasTestable)), F.Bool3(F.Of(this.SeparationOfDutiesHeld))))); set { }
        }

        // Formula SeparationIsVacuouslyGreen (rulebook: =AND({{SeparationOfDutiesHeld}}, NOT({{SeparationWasTestable}})))
        [NotMapped]
        public bool? SeparationIsVacuouslyGreen
        {
            get => F.AsBool(F.Memo(this, "SeparationIsVacuouslyGreen", () => F.And(F.Bool3(F.Of(this.SeparationOfDutiesHeld)), F.Bool3(F.Not(F.Bool3(F.Of(this.SeparationWasTestable))))))); set { }
        }

        // Formula SeparationAssuranceNote (rulebook: =IF({{SeparationViolationCount}} > 0, "Violated: same agent prepared and approved.", IF({{SeparationIsVacuouslyGreen}}, "Not tested: this run had no preparation/approval pair.", "Held under test.")))
        [NotMapped]
        public string? SeparationAssuranceNote
        {
            get => F.AsString(F.Memo(this, "SeparationAssuranceNote", () => (F.Truthy(F.Bool3(F.Cmp(F.Of(this.SeparationViolationCount), ">", F.I(0)))) ? F.S("Violated: same agent prepared and approved.") : (F.Truthy(F.Bool3(F.Of(this.SeparationIsVacuouslyGreen))) ? F.S("Not tested: this run had no preparation/approval pair.") : F.S("Held under test."))))); set { }
        }

        // Formula UngovernedDivergenceCount (rulebook: =COUNTIFS(StepExecutions!{{UngovernedDivergenceExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? UngovernedDivergenceCount
        {
            get => F.AsDecimal(F.Memo(this, "UngovernedDivergenceCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.UngovernedDivergenceExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula DivergenceWasFullyGoverned (rulebook: =AND({{DivergedFromSpecification}}, {{UngovernedDivergenceCount}} = 0))
        [NotMapped]
        public bool? DivergenceWasFullyGoverned
        {
            get => F.AsBool(F.Memo(this, "DivergenceWasFullyGoverned", () => F.And(F.Bool3(F.Of(this.DivergedFromSpecification)), F.Bool3(F.Eq(F.Of(this.UngovernedDivergenceCount), F.I(0)))))); set { }
        }

        // Formula ComputedlyWitnessedControlCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{ComputedWitnessExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? ComputedlyWitnessedControlCount
        {
            get => F.AsDecimal(F.Memo(this, "ComputedlyWitnessedControlCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.ComputedWitnessExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula EvaluatedControlCount (rulebook: ={{ComputedlyWitnessedControlCount}} + {{AssertedOnlyControlCount}})
        [NotMapped]
        public decimal? EvaluatedControlCount
        {
            get => F.AsDecimal(F.Memo(this, "EvaluatedControlCount", () => F.Add(F.Of(this.ComputedlyWitnessedControlCount), F.Of(this.AssertedOnlyControlCount)))); set { }
        }

        // Formula ComputedAssuranceRatio (rulebook: =IF({{EvaluatedControlCount}} = 0, 0, {{ComputedlyWitnessedControlCount}} / {{EvaluatedControlCount}}))
        [NotMapped]
        public decimal? ComputedAssuranceRatio
        {
            get => F.AsDecimal(F.Memo(this, "ComputedAssuranceRatio", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.EvaluatedControlCount), F.I(0)))) ? F.I(0) : F.Div(F.Of(this.ComputedlyWitnessedControlCount), F.Of(this.EvaluatedControlCount))))); set { }
        }

        // Formula InterestedPartyAssertionCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{InterestedAssertionExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? InterestedPartyAssertionCount
        {
            get => F.AsDecimal(F.Memo(this, "InterestedPartyAssertionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.InterestedAssertionExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula AssuranceGrade (rulebook: =IF({{EvaluatedControlCount}} = 0, "None: no blocking control was evaluated.", IF({{InterestedPartyAssertionCount}} > 0, "Weak: at least one control rests on an interested-party assertion.", IF({{ComputedAssuranceRatio}} < 0.5, "Thin: most controls rest on human assertion.", IF({{ComputedAssuranceRatio}} < 1, "Mixed: computed and asserted controls.", "Computed: every evaluated control has a witness.")))))
        [NotMapped]
        public string? AssuranceGrade
        {
            get => F.AsString(F.Memo(this, "AssuranceGrade", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.EvaluatedControlCount), F.I(0)))) ? F.S("None: no blocking control was evaluated.") : (F.Truthy(F.Bool3(F.Cmp(F.Of(this.InterestedPartyAssertionCount), ">", F.I(0)))) ? F.S("Weak: at least one control rests on an interested-party assertion.") : (F.Truthy(F.Bool3(F.Cmp(F.Of(this.ComputedAssuranceRatio), "<", F.D(0.5)))) ? F.S("Thin: most controls rest on human assertion.") : (F.Truthy(F.Bool3(F.Cmp(F.Of(this.ComputedAssuranceRatio), "<", F.I(1)))) ? F.S("Mixed: computed and asserted controls.") : F.S("Computed: every evaluated control has a witness."))))))); set { }
        }

        // Formula AttestationWouldBeWeaklyBased (rulebook: =AND({{IsAttestationReady}}, OR({{InterestedPartyAssertionCount}} > 0, {{ComputedAssuranceRatio}} < 0.5)))
        [NotMapped]
        public bool? AttestationWouldBeWeaklyBased
        {
            get => F.AsBool(F.Memo(this, "AttestationWouldBeWeaklyBased", () => F.And(F.Bool3(F.Of(this.IsAttestationReady)), F.Bool3(F.Or(F.Bool3(F.Cmp(F.Of(this.InterestedPartyAssertionCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.ComputedAssuranceRatio), "<", F.D(0.5)))))))); set { }
        }

        // Formula IndependentHumanObservationCount (rulebook: =COUNTIFS(VerificationOutcomes!{{IndependentObservationExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? IndependentHumanObservationCount
        {
            get => F.AsDecimal(F.Memo(this, "IndependentHumanObservationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<VerificationOutcome>(base.SoAContext, "VerificationOutcomes", __c => __c.VerificationOutcomes), __r => F.CritField(F.Of(__r.IndependentObservationExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula HasAnyIndependentObservation (rulebook: ={{IndependentHumanObservationCount}} > 0)
        [NotMapped]
        public bool? HasAnyIndependentObservation
        {
            get => F.AsBool(F.Memo(this, "HasAnyIndependentObservation", () => F.Cmp(F.Of(this.IndependentHumanObservationCount), ">", F.I(0)))); set { }
        }

        // Formula SelfAttestedApprovalCount (rulebook: =COUNTIFS(StepExecutions!{{SelfAttestedApprovalExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? SelfAttestedApprovalCount
        {
            get => F.AsDecimal(F.Memo(this, "SelfAttestedApprovalCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.SelfAttestedApprovalExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula AssuranceChainIsCircular (rulebook: =AND({{SelfAttestedApprovalCount}} > 0, NOT({{HasAnyIndependentObservation}})))
        [NotMapped]
        public bool? AssuranceChainIsCircular
        {
            get => F.AsBool(F.Memo(this, "AssuranceChainIsCircular", () => F.And(F.Bool3(F.Cmp(F.Of(this.SelfAttestedApprovalCount), ">", F.I(0))), F.Bool3(F.Not(F.Bool3(F.Of(this.HasAnyIndependentObservation))))))); set { }
        }

        // Formula LatestAttestationInstant (rulebook: =MAXIFS(Attestations!{{SignedAt}}, Attestations!{{ProcedureExecution}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public DateTimeOffset? LatestAttestationInstant
        {
            get => F.AsDateTime(F.Memo(this, "LatestAttestationInstant", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<Attestation>(base.SoAContext, "Attestations", __c => __c.Attestations), __r => F.CritField(F.Of(__r.ProcedureExecution), F.Of(this.ProcedureExecutionId)), __r => F.Of(__r.SignedAt))))); set { }
        }

        // Formula HasBeenAttested (rulebook: ={{AttestationCount}} > 0)
        [NotMapped]
        public bool? HasBeenAttested
        {
            get => F.AsBool(F.Memo(this, "HasBeenAttested", () => F.Cmp(F.Of(this.AttestationCount), ">", F.I(0)))); set { }
        }

        // Formula AttestationCount (rulebook: =COUNTIFS(Attestations!{{ProcedureExecution}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? AttestationCount
        {
            get => F.AsDecimal(F.Memo(this, "AttestationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Attestation>(base.SoAContext, "Attestations", __c => __c.Attestations), __r => F.CritField(F.Of(__r.ProcedureExecution), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula PostAttestationScoreCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{PostAttestationScoreExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? PostAttestationScoreCount
        {
            get => F.AsDecimal(F.Memo(this, "PostAttestationScoreCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.PostAttestationScoreExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula BasisChangedAfterSignature (rulebook: =AND({{HasBeenAttested}}, {{PostAttestationScoreCount}} > 0))
        [NotMapped]
        public bool? BasisChangedAfterSignature
        {
            get => F.AsBool(F.Memo(this, "BasisChangedAfterSignature", () => F.And(F.Bool3(F.Of(this.HasBeenAttested)), F.Bool3(F.Cmp(F.Of(this.PostAttestationScoreCount), ">", F.I(0)))))); set { }
        }

        // Formula RequiresReAttestation (rulebook: =AND({{BasisChangedAfterSignature}}, NOT({{IsAttestationReady}})))
        [NotMapped]
        public bool? RequiresReAttestation
        {
            get => F.AsBool(F.Memo(this, "RequiresReAttestation", () => F.And(F.Bool3(F.Of(this.BasisChangedAfterSignature)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsAttestationReady))))))); set { }
        }

        // Formula IntendedRecipientCount (rulebook: =COUNTIFS(SendIntents!{{IntentExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? IntendedRecipientCount
        {
            get => F.AsDecimal(F.Memo(this, "IntendedRecipientCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SendIntent>(base.SoAContext, "SendIntents", __c => __c.SendIntents), __r => F.CritField(F.Of(__r.IntentExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula ReachedRecipientCount (rulebook: =COUNTIFS(SendIntents!{{DeliveredIntentExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? ReachedRecipientCount
        {
            get => F.AsDecimal(F.Memo(this, "ReachedRecipientCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SendIntent>(base.SoAContext, "SendIntents", __c => __c.SendIntents), __r => F.CritField(F.Of(__r.DeliveredIntentExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula SilentlyDroppedCount (rulebook: =COUNTIFS(SendIntents!{{DroppedIntentExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? SilentlyDroppedCount
        {
            get => F.AsDecimal(F.Memo(this, "SilentlyDroppedCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SendIntent>(base.SoAContext, "SendIntents", __c => __c.SendIntents), __r => F.CritField(F.Of(__r.DroppedIntentExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula DeliveryYieldPercent (rulebook: =IF({{IntendedRecipientCount}} > 0, {{ReachedRecipientCount}} * 100 / {{IntendedRecipientCount}}, 0))
        [NotMapped]
        public decimal? DeliveryYieldPercent
        {
            get => F.AsDecimal(F.Memo(this, "DeliveryYieldPercent", () => (F.Truthy(F.Bool3(F.Cmp(F.Of(this.IntendedRecipientCount), ">", F.I(0)))) ? F.Div(F.Mul(F.Of(this.ReachedRecipientCount), F.I(100)), F.Of(this.IntendedRecipientCount)) : F.I(0)))); set { }
        }

        // Formula CampaignSilentlyLostAudience (rulebook: =({{SilentlyDroppedCount}} > 0))
        [NotMapped]
        public bool? CampaignSilentlyLostAudience
        {
            get => F.AsBool(F.Memo(this, "CampaignSilentlyLostAudience", () => F.Cmp(F.Of(this.SilentlyDroppedCount), ">", F.I(0)))); set { }
        }

        // Formula UnrecordedRefusalCount (rulebook: =COUNTIFS(SendIntents!{{UnrecordedRefusalExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? UnrecordedRefusalCount
        {
            get => F.AsDecimal(F.Memo(this, "UnrecordedRefusalCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SendIntent>(base.SoAContext, "SendIntents", __c => __c.SendIntents), __r => F.CritField(F.Of(__r.UnrecordedRefusalExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula HasUnrecordedRefusals (rulebook: =({{UnrecordedRefusalCount}} > 0))
        [NotMapped]
        public bool? HasUnrecordedRefusals
        {
            get => F.AsBool(F.Memo(this, "HasUnrecordedRefusals", () => F.Cmp(F.Of(this.UnrecordedRefusalCount), ">", F.I(0)))); set { }
        }

        // Formula IndependentlyConfirmedIntentCount (rulebook: =COUNTIFS(SendIntents!{{IndependentlyConfirmedExecutionKey}}, {{ProcedureExecutionId}}))
        [NotMapped]
        public decimal? IndependentlyConfirmedIntentCount
        {
            get => F.AsDecimal(F.Memo(this, "IndependentlyConfirmedIntentCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SendIntent>(base.SoAContext, "SendIntents", __c => __c.SendIntents), __r => F.CritField(F.Of(__r.IndependentlyConfirmedExecutionKey), F.Of(this.ProcedureExecutionId)))))); set { }
        }

        // Formula SendDecisionsAreEntirelySelfWitnessed (rulebook: =AND({{IntendedRecipientCount}} > 0, {{IndependentlyConfirmedIntentCount}} = 0))
        [NotMapped]
        public bool? SendDecisionsAreEntirelySelfWitnessed
        {
            get => F.AsBool(F.Memo(this, "SendDecisionsAreEntirelySelfWitnessed", () => F.And(F.Bool3(F.Cmp(F.Of(this.IntendedRecipientCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.IndependentlyConfirmedIntentCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? ExecutedByAgent { get; set; }

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

        private Agent _agent;

        [ForeignKey("ExecutedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ExecutedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ExecutedByAgent: " + ExecutedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ExecutedByAgent);
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
                        ExecutedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private ObservableCollection<StepExecution> _stepExecutions;

        [InverseProperty("ProcedureExecutionRef")]
        public virtual ObservableCollection<StepExecution> StepExecutions
        {
            get
            {
                if (_stepExecutions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecutions - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _stepExecutions = new ObservableCollection<StepExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepExecutions.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<StepExecution>();
                        _stepExecutions = new ObservableCollection<StepExecution>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepExecutions.CollectionChanged += StepExecutions_CollectionChanged;
                }
                return _stepExecutions;
            }
            private set
            {
                if (_stepExecutions != null)
                {
                    _stepExecutions.CollectionChanged -= StepExecutions_CollectionChanged;
                }
                _stepExecutions = value;
                if (_stepExecutions != null)
                {
                    _stepExecutions.CollectionChanged += StepExecutions_CollectionChanged;
                }
            }
        }

        private void StepExecutions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepExecution>())
                {
                    item.ProcedureExecution = this.ProcedureExecutionId;
                }
            }
        }

        private ObservableCollection<UserFeedback> _userFeedback;

        [InverseProperty("ProcedureExecutionRef")]
        public virtual ObservableCollection<UserFeedback> UserFeedback
        {
            get
            {
                if (_userFeedback == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserFeedback - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _userFeedback = new ObservableCollection<UserFeedback>();
                    }
                    else
                    {
                        var items = base.SoAContext.UserFeedback.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<UserFeedback>();
                        _userFeedback = new ObservableCollection<UserFeedback>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _userFeedback.CollectionChanged += UserFeedback_CollectionChanged;
                }
                return _userFeedback;
            }
            private set
            {
                if (_userFeedback != null)
                {
                    _userFeedback.CollectionChanged -= UserFeedback_CollectionChanged;
                }
                _userFeedback = value;
                if (_userFeedback != null)
                {
                    _userFeedback.CollectionChanged += UserFeedback_CollectionChanged;
                }
            }
        }

        private void UserFeedback_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<UserFeedback>())
                {
                    item.ProcedureExecution = this.ProcedureExecutionId;
                }
            }
        }

        private ObservableCollection<ObservedTransition> _observedTransitions;

        [InverseProperty("ProcedureExecutionRef")]
        public virtual ObservableCollection<ObservedTransition> ObservedTransitions
        {
            get
            {
                if (_observedTransitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ObservedTransitions - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _observedTransitions = new ObservableCollection<ObservedTransition>();
                    }
                    else
                    {
                        var items = base.SoAContext.ObservedTransitions.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<ObservedTransition>();
                        _observedTransitions = new ObservableCollection<ObservedTransition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _observedTransitions.CollectionChanged += ObservedTransitions_CollectionChanged;
                }
                return _observedTransitions;
            }
            private set
            {
                if (_observedTransitions != null)
                {
                    _observedTransitions.CollectionChanged -= ObservedTransitions_CollectionChanged;
                }
                _observedTransitions = value;
                if (_observedTransitions != null)
                {
                    _observedTransitions.CollectionChanged += ObservedTransitions_CollectionChanged;
                }
            }
        }

        private void ObservedTransitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ObservedTransition>())
                {
                    item.ProcedureExecution = this.ProcedureExecutionId;
                }
            }
        }

        private ObservableCollection<MessageDelivery> _messageDeliveries;

        [InverseProperty("ProcedureExecutionRef")]
        public virtual ObservableCollection<MessageDelivery> MessageDeliveries
        {
            get
            {
                if (_messageDeliveries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = base.SoAContext.MessageDeliveries.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<MessageDelivery>();
                        _messageDeliveries = new ObservableCollection<MessageDelivery>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _messageDeliveries.CollectionChanged += MessageDeliveries_CollectionChanged;
                }
                return _messageDeliveries;
            }
            private set
            {
                if (_messageDeliveries != null)
                {
                    _messageDeliveries.CollectionChanged -= MessageDeliveries_CollectionChanged;
                }
                _messageDeliveries = value;
                if (_messageDeliveries != null)
                {
                    _messageDeliveries.CollectionChanged += MessageDeliveries_CollectionChanged;
                }
            }
        }

        private void MessageDeliveries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MessageDelivery>())
                {
                    item.ProcedureExecution = this.ProcedureExecutionId;
                }
            }
        }

        private ObservableCollection<SendIntent> _sendIntents;

        [InverseProperty("ProcedureExecutionRef")]
        public virtual ObservableCollection<SendIntent> SendIntents
        {
            get
            {
                if (_sendIntents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = base.SoAContext.SendIntents.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<SendIntent>();
                        _sendIntents = new ObservableCollection<SendIntent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _sendIntents.CollectionChanged += SendIntents_CollectionChanged;
                }
                return _sendIntents;
            }
            private set
            {
                if (_sendIntents != null)
                {
                    _sendIntents.CollectionChanged -= SendIntents_CollectionChanged;
                }
                _sendIntents = value;
                if (_sendIntents != null)
                {
                    _sendIntents.CollectionChanged += SendIntents_CollectionChanged;
                }
            }
        }

        private void SendIntents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SendIntent>())
                {
                    item.ProcedureExecution = this.ProcedureExecutionId;
                }
            }
        }

        private ObservableCollection<DeliveredCommunication> _deliveredCommunications;

        [InverseProperty("ProcedureExecutionRef")]
        public virtual ObservableCollection<DeliveredCommunication> DeliveredCommunications
        {
            get
            {
                if (_deliveredCommunications == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DeliveredCommunications - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _deliveredCommunications = new ObservableCollection<DeliveredCommunication>();
                    }
                    else
                    {
                        var items = base.SoAContext.DeliveredCommunications.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<DeliveredCommunication>();
                        _deliveredCommunications = new ObservableCollection<DeliveredCommunication>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _deliveredCommunications.CollectionChanged += DeliveredCommunications_CollectionChanged;
                }
                return _deliveredCommunications;
            }
            private set
            {
                if (_deliveredCommunications != null)
                {
                    _deliveredCommunications.CollectionChanged -= DeliveredCommunications_CollectionChanged;
                }
                _deliveredCommunications = value;
                if (_deliveredCommunications != null)
                {
                    _deliveredCommunications.CollectionChanged += DeliveredCommunications_CollectionChanged;
                }
            }
        }

        private void DeliveredCommunications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DeliveredCommunication>())
                {
                    item.ProcedureExecution = this.ProcedureExecutionId;
                }
            }
        }

        private ObservableCollection<Attestation> _attestations;

        [InverseProperty("ProcedureExecutionRef")]
        public virtual ObservableCollection<Attestation> Attestations
        {
            get
            {
                if (_attestations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Attestations - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _attestations = new ObservableCollection<Attestation>();
                    }
                    else
                    {
                        var items = base.SoAContext.Attestations.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<Attestation>();
                        _attestations = new ObservableCollection<Attestation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _attestations.CollectionChanged += Attestations_CollectionChanged;
                }
                return _attestations;
            }
            private set
            {
                if (_attestations != null)
                {
                    _attestations.CollectionChanged -= Attestations_CollectionChanged;
                }
                _attestations = value;
                if (_attestations != null)
                {
                    _attestations.CollectionChanged += Attestations_CollectionChanged;
                }
            }
        }

        private void Attestations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Attestation>())
                {
                    item.ProcedureExecution = this.ProcedureExecutionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.Agent;
            _ = this.StepExecutions;
            _ = this.UserFeedback;
            _ = this.ObservedTransitions;
            _ = this.MessageDeliveries;
            _ = this.SendIntents;
            _ = this.DeliveredCommunications;
            _ = this.Attestations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
