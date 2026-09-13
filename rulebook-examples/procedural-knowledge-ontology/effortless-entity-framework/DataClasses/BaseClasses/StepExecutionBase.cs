
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
    [Table("StepExecutions")]
    public class StepExecutionBase : SoAEntityBase
    {
        [Key]
        public string StepExecutionId { get; set; }

        // Formula Name (rulebook: ={{ProcedureExecution}} & " / " & {{Step}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.ProcedureExecution)), F.S(" / "), F.TextOr(F.Of(this.Step))))); set { }
        }

        public string? ExecutionStatus { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }
        public string? VerificationResult { get; set; }
        public string? Deviation { get; set; }
        // Formula ActualDurationMinutes (rulebook: =IF({{EndedAt}} = "", 0, DATETIME_DIFF({{EndedAt}}, {{StartedAt}}, "minutes")))
        [NotMapped]
        public int? ActualDurationMinutes
        {
            get => F.AsInt(F.Memo(this, "ActualDurationMinutes", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.EndedAt)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.EndedAt), F.Of(this.StartedAt), F.S("minutes")))))); set { }
        }

        // Formula ExpectedDurationMinutes (rulebook: =INDEX(Steps!{{ExpectedDurationMinutes}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public int? ExpectedDurationMinutes
        {
            get => F.AsInt(F.Memo(this, "ExpectedDurationMinutes", () => F.Integer(F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.ExpectedDurationMinutes), () => F.Of(new Step().ExpectedDurationMinutes))))); set { }
        }

        // Formula IsLate (rulebook: ={{ActualDurationMinutes}} > {{ExpectedDurationMinutes}})
        [NotMapped]
        public bool? IsLate
        {
            get => F.AsBool(F.Memo(this, "IsLate", () => F.Cmp(F.Of(this.ActualDurationMinutes), ">", F.Of(this.ExpectedDurationMinutes)))); set { }
        }

        // Formula BlockingUnmetCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{StepExecution}}, {{StepExecutionId}}, RequirementSatisfactions!{{IsBlockingAndUnmet}}, TRUE))
        [NotMapped]
        public decimal? BlockingUnmetCount
        {
            get => F.AsDecimal(F.Memo(this, "BlockingUnmetCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.StepExecution), F.Of(this.StepExecutionId)) && F.CritLiteral(F.Of(__r.IsBlockingAndUnmet), F.B(true)))))); set { }
        }

        // Formula BlockingUnmetCountSafe (rulebook: =COUNTIFS(RequirementSatisfactions!{{BlockingUnmetStepKey}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? BlockingUnmetCountSafe
        {
            get => F.AsDecimal(F.Memo(this, "BlockingUnmetCountSafe", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.BlockingUnmetStepKey), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula ProceededPastBlockingControl (rulebook: =AND({{ExecutionStatus}} = "Completed", {{BlockingUnmetCountSafe}} > 0))
        [NotMapped]
        public bool? ProceededPastBlockingControl
        {
            get => F.AsBool(F.Memo(this, "ProceededPastBlockingControl", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ExecutionStatus)), F.S("Completed"))), F.Bool3(F.Cmp(F.Of(this.BlockingUnmetCountSafe), ">", F.I(0)))))); set { }
        }

        // Formula ExpectedBlockingCount (rulebook: =INDEX(Steps!{{BlockingRequirementCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public decimal? ExpectedBlockingCount
        {
            get => F.AsDecimal(F.Memo(this, "ExpectedBlockingCount", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.BlockingRequirementCount), () => F.Of(new Step().BlockingRequirementCount)))); set { }
        }

        // Formula EvaluatedBlockingCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{BlockingSatisfactionStepKey}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? EvaluatedBlockingCount
        {
            get => F.AsDecimal(F.Memo(this, "EvaluatedBlockingCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.BlockingSatisfactionStepKey), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula UnevaluatedBlockingCount (rulebook: ={{ExpectedBlockingCount}} - {{EvaluatedBlockingCount}})
        [NotMapped]
        public decimal? UnevaluatedBlockingCount
        {
            get => F.AsDecimal(F.Memo(this, "UnevaluatedBlockingCount", () => F.Sub(F.Of(this.ExpectedBlockingCount), F.Of(this.EvaluatedBlockingCount)))); set { }
        }

        // Formula HasUnevaluatedBlockingControl (rulebook: ={{UnevaluatedBlockingCount}} > 0)
        [NotMapped]
        public bool? HasUnevaluatedBlockingControl
        {
            get => F.AsBool(F.Memo(this, "HasUnevaluatedBlockingControl", () => F.Cmp(F.Of(this.UnevaluatedBlockingCount), ">", F.I(0)))); set { }
        }

        // Formula StaleAuthoritativeSourceCount (rulebook: =INDEX(Steps!{{AuthoritativeStaleCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public decimal? StaleAuthoritativeSourceCount
        {
            get => F.AsDecimal(F.Memo(this, "StaleAuthoritativeSourceCount", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.AuthoritativeStaleCount), () => F.Of(new Step().AuthoritativeStaleCount)))); set { }
        }

        // Formula RanOnStaleAuthoritativeSource (rulebook: ={{StaleAuthoritativeSourceCount}} > 0)
        [NotMapped]
        public bool? RanOnStaleAuthoritativeSource
        {
            get => F.AsBool(F.Memo(this, "RanOnStaleAuthoritativeSource", () => F.Cmp(F.Of(this.StaleAuthoritativeSourceCount), ">", F.I(0)))); set { }
        }

        // Formula HasDeviationNote (rulebook: ={{Deviation}} <> "")
        [NotMapped]
        public bool? HasDeviationNote
        {
            get => F.AsBool(F.Memo(this, "HasDeviationNote", () => F.IsNotBlank(F.Of(this.Deviation)))); set { }
        }

        // Formula IsLateAndUnexplained (rulebook: =AND({{IsLate}}, NOT({{HasDeviationNote}})))
        [NotMapped]
        public bool? IsLateAndUnexplained
        {
            get => F.AsBool(F.Memo(this, "IsLateAndUnexplained", () => F.And(F.Bool3(F.Of(this.IsLate)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasDeviationNote))))))); set { }
        }

        // Formula AvailableExceptionCountForStep (rulebook: =INDEX(Steps!{{AvailableExceptionCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public decimal? AvailableExceptionCountForStep
        {
            get => F.AsDecimal(F.Memo(this, "AvailableExceptionCountForStep", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.AvailableExceptionCount), () => F.Of(new Step().AvailableExceptionCount)))); set { }
        }

        // Formula HadUninvokedExceptionAvailable (rulebook: =AND({{IsLateAndUnexplained}}, {{AvailableExceptionCountForStep}} > 0))
        [NotMapped]
        public bool? HadUninvokedExceptionAvailable
        {
            get => F.AsBool(F.Memo(this, "HadUninvokedExceptionAvailable", () => F.And(F.Bool3(F.Of(this.IsLateAndUnexplained)), F.Bool3(F.Cmp(F.Of(this.AvailableExceptionCountForStep), ">", F.I(0)))))); set { }
        }

        // Formula ExpectedVerificationCount (rulebook: =INDEX(Steps!{{DeclaredVerificationCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public decimal? ExpectedVerificationCount
        {
            get => F.AsDecimal(F.Memo(this, "ExpectedVerificationCount", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.DeclaredVerificationCount), () => F.Of(new Step().DeclaredVerificationCount)))); set { }
        }

        // Formula PerformedVerificationCount (rulebook: =COUNTIFS(VerificationOutcomes!{{StepExecution}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? PerformedVerificationCount
        {
            get => F.AsDecimal(F.Memo(this, "PerformedVerificationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<VerificationOutcome>(base.SoAContext, "VerificationOutcomes", __c => __c.VerificationOutcomes), __r => F.CritField(F.Of(__r.StepExecution), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula SkippedVerificationCount (rulebook: ={{ExpectedVerificationCount}} - {{PerformedVerificationCount}})
        [NotMapped]
        public decimal? SkippedVerificationCount
        {
            get => F.AsDecimal(F.Memo(this, "SkippedVerificationCount", () => F.Sub(F.Of(this.ExpectedVerificationCount), F.Of(this.PerformedVerificationCount)))); set { }
        }

        // Formula HasSkippedVerification (rulebook: ={{SkippedVerificationCount}} > 0)
        [NotMapped]
        public bool? HasSkippedVerification
        {
            get => F.AsBool(F.Memo(this, "HasSkippedVerification", () => F.Cmp(F.Of(this.SkippedVerificationCount), ">", F.I(0)))); set { }
        }

        // Formula ClaimsPassWithoutEvidence (rulebook: =AND({{VerificationResult}} = "PASS", {{HasSkippedVerification}}))
        [NotMapped]
        public bool? ClaimsPassWithoutEvidence
        {
            get => F.AsBool(F.Memo(this, "ClaimsPassWithoutEvidence", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.VerificationResult)), F.S("PASS"))), F.Bool3(F.Of(this.HasSkippedVerification))))); set { }
        }

        // Formula StepIsPreparation (rulebook: =INDEX(Steps!{{IsPreparationStep}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? StepIsPreparation
        {
            get => F.AsBool(F.Memo(this, "StepIsPreparation", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.IsPreparationStep), () => F.Of(new Step().IsPreparationStep)))); set { }
        }

        // Formula StepIsApproval (rulebook: =INDEX(Steps!{{IsApprovalStep}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? StepIsApproval
        {
            get => F.AsBool(F.Memo(this, "StepIsApproval", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.IsApprovalStep), () => F.Of(new Step().IsApprovalStep)))); set { }
        }

        // Formula PreparerAgentKey (rulebook: =IF({{StepIsPreparation}}, {{ProcedureExecution}} & "|" & {{ExecutedByAgent}}, ""))
        [NotMapped]
        public string? PreparerAgentKey
        {
            get => F.AsString(F.Memo(this, "PreparerAgentKey", () => (F.Truthy(F.Bool3(F.Of(this.StepIsPreparation))) ? F.Concat(F.TextOr(F.Of(this.ProcedureExecution)), F.S("|"), F.TextOr(F.Of(this.ExecutedByAgent))) : F.S("")))); set { }
        }

        // Formula ApproverAgentKey (rulebook: =IF({{StepIsApproval}}, {{ProcedureExecution}} & "|" & {{ExecutedByAgent}}, ""))
        [NotMapped]
        public string? ApproverAgentKey
        {
            get => F.AsString(F.Memo(this, "ApproverAgentKey", () => (F.Truthy(F.Bool3(F.Of(this.StepIsApproval))) ? F.Concat(F.TextOr(F.Of(this.ProcedureExecution)), F.S("|"), F.TextOr(F.Of(this.ExecutedByAgent))) : F.S("")))); set { }
        }

        // Formula PreparedByThisAgentCount (rulebook: =COUNTIFS(StepExecutions!{{PreparerAgentKey}}, {{ApproverAgentKey}}))
        [NotMapped]
        public decimal? PreparedByThisAgentCount
        {
            get => F.AsDecimal(F.Memo(this, "PreparedByThisAgentCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.PreparerAgentKey), F.Of(this.ApproverAgentKey)))))); set { }
        }

        // Formula ViolatesSeparationOfDuties (rulebook: =AND({{StepIsApproval}}, {{PreparedByThisAgentCount}} > 0))
        [NotMapped]
        public bool? ViolatesSeparationOfDuties
        {
            get => F.AsBool(F.Memo(this, "ViolatesSeparationOfDuties", () => F.And(F.Bool3(F.Of(this.StepIsApproval)), F.Bool3(F.Cmp(F.Of(this.PreparedByThisAgentCount), ">", F.I(0)))))); set { }
        }

        // Formula RequiredRoleForStep (rulebook: =INDEX(Steps!{{AssignedRole}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? RequiredRoleForStep
        {
            get => F.AsString(F.Memo(this, "RequiredRoleForStep", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.AssignedRole), () => F.Of(new Step().AssignedRole)))); set { }
        }

        // Formula ExecutorRoleKey (rulebook: ={{ExecutedByAgent}} & "|" & {{RequiredRoleForStep}})
        [NotMapped]
        public string? ExecutorRoleKey
        {
            get => F.AsString(F.Memo(this, "ExecutorRoleKey", () => F.Concat(F.TextOr(F.Of(this.ExecutedByAgent)), F.S("|"), F.TextOr(F.Of(this.RequiredRoleForStep))))); set { }
        }

        // Formula ExecutorAuthorityCount (rulebook: =COUNTIFS(RoleAssignments!{{AgentRoleKey}}, {{ExecutorRoleKey}}))
        [NotMapped]
        public decimal? ExecutorAuthorityCount
        {
            get => F.AsDecimal(F.Memo(this, "ExecutorAuthorityCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.AgentRoleKey), F.Of(this.ExecutorRoleKey)))))); set { }
        }

        // Formula ExecutorHeldRequiredRole (rulebook: ={{ExecutorAuthorityCount}} > 0)
        [NotMapped]
        public bool? ExecutorHeldRequiredRole
        {
            get => F.AsBool(F.Memo(this, "ExecutorHeldRequiredRole", () => F.Cmp(F.Of(this.ExecutorAuthorityCount), ">", F.I(0)))); set { }
        }

        // Formula IsUnauthorizedApproval (rulebook: =AND({{StepIsApproval}}, NOT({{ExecutorHeldRequiredRole}})))
        [NotMapped]
        public bool? IsUnauthorizedApproval
        {
            get => F.AsBool(F.Memo(this, "IsUnauthorizedApproval", () => F.And(F.Bool3(F.Of(this.StepIsApproval)), F.Bool3(F.Not(F.Bool3(F.Of(this.ExecutorHeldRequiredRole))))))); set { }
        }

        // Formula CompletedExecutionKey (rulebook: =IF({{ExecutionStatus}} = "Completed", {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? CompletedExecutionKey
        {
            get => F.AsString(F.Memo(this, "CompletedExecutionKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.ExecutionStatus)), F.S("Completed")))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula ControlBreachExecutionKey (rulebook: =IF(OR({{ProceededPastBlockingControl}}, {{ViolatesSeparationOfDuties}}, {{IsUnauthorizedApproval}}, {{ClaimsPassWithoutEvidence}}), {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? ControlBreachExecutionKey
        {
            get => F.AsString(F.Memo(this, "ControlBreachExecutionKey", () => (F.Truthy(F.Bool3(F.Or(F.Bool3(F.Of(this.ProceededPastBlockingControl)), F.Bool3(F.Of(this.ViolatesSeparationOfDuties)), F.Bool3(F.Of(this.IsUnauthorizedApproval)), F.Bool3(F.Of(this.ClaimsPassWithoutEvidence))))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula LateExecutionKey (rulebook: =IF({{IsLate}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? LateExecutionKey
        {
            get => F.AsString(F.Memo(this, "LateExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsLate))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula ExecutorAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{ExecutedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? ExecutorAgentKind
        {
            get => F.AsString(F.Memo(this, "ExecutorAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.ExecutedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula ExecutorIsHuman (rulebook: ={{ExecutorAgentKind}} = "Human")
        [NotMapped]
        public bool? ExecutorIsHuman
        {
            get => F.AsBool(F.Memo(this, "ExecutorIsHuman", () => F.Eq(F.Of(this.ExecutorAgentKind), F.S("Human")))); set { }
        }

        // Formula StepRequiresHumanConfirmation (rulebook: =INDEX(Steps!{{RequiresHumanConfirmation}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? StepRequiresHumanConfirmation
        {
            get => F.AsBool(F.Memo(this, "StepRequiresHumanConfirmation", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.RequiresHumanConfirmation), () => F.Of(new Step().RequiresHumanConfirmation)))); set { }
        }

        // Formula NonHumanRanHumanStep (rulebook: =AND({{StepRequiresHumanConfirmation}}, NOT({{ExecutorIsHuman}})))
        [NotMapped]
        public bool? NonHumanRanHumanStep
        {
            get => F.AsBool(F.Memo(this, "NonHumanRanHumanStep", () => F.And(F.Bool3(F.Of(this.StepRequiresHumanConfirmation)), F.Bool3(F.Not(F.Bool3(F.Of(this.ExecutorIsHuman))))))); set { }
        }

        // Formula NonHumanApproval (rulebook: =AND({{StepIsApproval}}, NOT({{ExecutorIsHuman}})))
        [NotMapped]
        public bool? NonHumanApproval
        {
            get => F.AsBool(F.Memo(this, "NonHumanApproval", () => F.And(F.Bool3(F.Of(this.StepIsApproval)), F.Bool3(F.Not(F.Bool3(F.Of(this.ExecutorIsHuman))))))); set { }
        }

        // Formula UnevaluatedBlockingExecutionKey (rulebook: =IF({{HasUnevaluatedBlockingControl}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? UnevaluatedBlockingExecutionKey
        {
            get => F.AsString(F.Memo(this, "UnevaluatedBlockingExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.HasUnevaluatedBlockingControl))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula SeparationViolationExecutionKey (rulebook: =IF({{ViolatesSeparationOfDuties}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? SeparationViolationExecutionKey
        {
            get => F.AsString(F.Memo(this, "SeparationViolationExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.ViolatesSeparationOfDuties))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula SelfWitnessedVerificationCount (rulebook: =COUNTIFS(VerificationOutcomes!{{SelfWitnessedStepKey}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? SelfWitnessedVerificationCount
        {
            get => F.AsDecimal(F.Memo(this, "SelfWitnessedVerificationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<VerificationOutcome>(base.SoAContext, "VerificationOutcomes", __c => __c.VerificationOutcomes), __r => F.CritField(F.Of(__r.SelfWitnessedStepKey), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula UnbackedVerificationCount (rulebook: =COUNTIFS(VerificationOutcomes!{{UnbackedStepKey}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? UnbackedVerificationCount
        {
            get => F.AsDecimal(F.Memo(this, "UnbackedVerificationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<VerificationOutcome>(base.SoAContext, "VerificationOutcomes", __c => __c.VerificationOutcomes), __r => F.CritField(F.Of(__r.UnbackedStepKey), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula ApprovalRestsOnSelfAttestation (rulebook: =AND({{StepIsApproval}}, OR({{SelfWitnessedVerificationCount}} > 0, {{HasSkippedVerification}})))
        [NotMapped]
        public bool? ApprovalRestsOnSelfAttestation
        {
            get => F.AsBool(F.Memo(this, "ApprovalRestsOnSelfAttestation", () => F.And(F.Bool3(F.Of(this.StepIsApproval)), F.Bool3(F.Or(F.Bool3(F.Cmp(F.Of(this.SelfWitnessedVerificationCount), ">", F.I(0))), F.Bool3(F.Of(this.HasSkippedVerification))))))); set { }
        }

        // Formula ExceptionInvocationCount (rulebook: =COUNTIFS(ExceptionInvocations!{{StepExecution}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? ExceptionInvocationCount
        {
            get => F.AsDecimal(F.Memo(this, "ExceptionInvocationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ExceptionInvocation>(base.SoAContext, "ExceptionInvocations", __c => __c.ExceptionInvocations), __r => F.CritField(F.Of(__r.StepExecution), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula RanUnderException (rulebook: ={{ExceptionInvocationCount}} > 0)
        [NotMapped]
        public bool? RanUnderException
        {
            get => F.AsBool(F.Memo(this, "RanUnderException", () => F.Cmp(F.Of(this.ExceptionInvocationCount), ">", F.I(0)))); set { }
        }

        // Formula IsCompleted (rulebook: ={{ExecutionStatus}} = "Completed")
        [NotMapped]
        public bool? IsCompleted
        {
            get => F.AsBool(F.Memo(this, "IsCompleted", () => F.Eq(F.Nullif(F.Of(this.ExecutionStatus)), F.S("Completed")))); set { }
        }

        // Formula IsVerificationPassed (rulebook: ={{VerificationResult}} = "PASS")
        [NotMapped]
        public bool? IsVerificationPassed
        {
            get => F.AsBool(F.Memo(this, "IsVerificationPassed", () => F.Eq(F.Nullif(F.Of(this.VerificationResult)), F.S("PASS")))); set { }
        }

        // Formula IsLegalReviewStep (rulebook: ={{Step}} = "policy-04")
        [NotMapped]
        public bool? IsLegalReviewStep
        {
            get => F.AsBool(F.Memo(this, "IsLegalReviewStep", () => F.Eq(F.Nullif(F.Of(this.Step)), F.S("policy-04")))); set { }
        }

        // Formula ClearedLegalReviewKey (rulebook: =IF(AND({{IsLegalReviewStep}}, {{IsVerificationPassed}}), {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? ClearedLegalReviewKey
        {
            get => F.AsString(F.Memo(this, "ClearedLegalReviewKey", () => (F.Truthy(F.Bool3(F.And(F.Bool3(F.Of(this.IsLegalReviewStep)), F.Bool3(F.Of(this.IsVerificationPassed))))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula AssignedRole (rulebook: =INDEX(Steps!{{AssignedRole}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? AssignedRole
        {
            get => F.AsString(F.Memo(this, "AssignedRole", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.AssignedRole), () => F.Of(new Step().AssignedRole)))); set { }
        }

        // Formula RoleCurrentAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? RoleCurrentAgent
        {
            get => F.AsString(F.Memo(this, "RoleCurrentAgent", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AssignedRole), __r => F.Of(__r.CurrentAgent), () => F.Of(new Role().CurrentAgent)))); set { }
        }

        // Formula ExecutorIsDesignatedAgent (rulebook: ={{ExecutedByAgent}} = {{RoleCurrentAgent}})
        [NotMapped]
        public bool? ExecutorIsDesignatedAgent
        {
            get => F.AsBool(F.Memo(this, "ExecutorIsDesignatedAgent", () => F.Eq(F.Nullif(F.Of(this.ExecutedByAgent)), F.Of(this.RoleCurrentAgent)))); set { }
        }

        // Formula InputsWereFreshAtRun (rulebook: =INDEX(Steps!{{InputsAreFresh}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? InputsWereFreshAtRun
        {
            get => F.AsBool(F.Memo(this, "InputsWereFreshAtRun", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.InputsAreFresh), () => F.Of(new Step().InputsAreFresh)))); set { }
        }

        // Formula RanOnStaleInputs (rulebook: =AND({{ExecutionStatus}} = "Completed", NOT({{InputsWereFreshAtRun}})))
        [NotMapped]
        public bool? RanOnStaleInputs
        {
            get => F.AsBool(F.Memo(this, "RanOnStaleInputs", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ExecutionStatus)), F.S("Completed"))), F.Bool3(F.Not(F.Bool3(F.Of(this.InputsWereFreshAtRun))))))); set { }
        }

        // Formula UnresolvedIssueCount (rulebook: =COUNTIFS(IssueOccurrences!{{StepExecutionWhenUnresolved}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? UnresolvedIssueCount
        {
            get => F.AsDecimal(F.Memo(this, "UnresolvedIssueCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<IssueOccurrence>(base.SoAContext, "IssueOccurrences", __c => __c.IssueOccurrences), __r => F.CritField(F.Of(__r.StepExecutionWhenUnresolved), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula HasDeviation (rulebook: ={{Deviation}} <> "")
        [NotMapped]
        public bool? HasDeviation
        {
            get => F.AsBool(F.Memo(this, "HasDeviation", () => F.IsNotBlank(F.Of(this.Deviation)))); set { }
        }

        // Formula IsClean (rulebook: =AND({{VerificationResult}} = "PASS", NOT({{HasDeviation}}), {{UnresolvedIssueCount}} = 0, NOT({{IsLate}})))
        [NotMapped]
        public bool? IsClean
        {
            get => F.AsBool(F.Memo(this, "IsClean", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.VerificationResult)), F.S("PASS"))), F.Bool3(F.Not(F.Bool3(F.Of(this.HasDeviation)))), F.Bool3(F.Eq(F.Of(this.UnresolvedIssueCount), F.I(0))), F.Bool3(F.Not(F.Bool3(F.Of(this.IsLate))))))); set { }
        }

        // Formula ProcedureExecutionWhenUnclean (rulebook: =IF({{IsClean}}, "", {{ProcedureExecution}}))
        [NotMapped]
        public string? ProcedureExecutionWhenUnclean
        {
            get => F.AsString(F.Memo(this, "ProcedureExecutionWhenUnclean", () => (F.Truthy(F.Bool3(F.Of(this.IsClean))) ? F.S("") : F.Of(this.ProcedureExecution)))); set { }
        }

        // Formula EvaluatedRequirementCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{StepExecutionWhenScored}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? EvaluatedRequirementCount
        {
            get => F.AsDecimal(F.Memo(this, "EvaluatedRequirementCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.StepExecutionWhenScored), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula RequiredBlockingCount (rulebook: =INDEX(Steps!{{BlockingRequirementCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public int? RequiredBlockingCount
        {
            get => F.AsInt(F.Memo(this, "RequiredBlockingCount", () => F.Integer(F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.BlockingRequirementCount), () => F.Of(new Step().BlockingRequirementCount))))); set { }
        }

        // Formula HasUnevaluatedBlockingRequirement (rulebook: ={{EvaluatedRequirementCount}} < {{RequiredBlockingCount}})
        [NotMapped]
        public bool? HasUnevaluatedBlockingRequirement
        {
            get => F.AsBool(F.Memo(this, "HasUnevaluatedBlockingRequirement", () => F.Cmp(F.Of(this.EvaluatedRequirementCount), "<", F.Of(this.RequiredBlockingCount)))); set { }
        }

        // Formula ExecutingAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{ExecutedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? ExecutingAgentKind
        {
            get => F.AsString(F.Memo(this, "ExecutingAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.ExecutedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula WasExecutedBySoftware (rulebook: =OR({{ExecutingAgentKind}} = "AIAgent", {{ExecutingAgentKind}} = "AutomatedPipeline"))
        [NotMapped]
        public bool? WasExecutedBySoftware
        {
            get => F.AsBool(F.Memo(this, "WasExecutedBySoftware", () => F.Or(F.Bool3(F.Eq(F.Of(this.ExecutingAgentKind), F.S("AIAgent"))), F.Bool3(F.Eq(F.Of(this.ExecutingAgentKind), F.S("AutomatedPipeline")))))); set { }
        }

        // Formula StepIsSoftwareAssigned (rulebook: =INDEX(Steps!{{IsSoftwareAssigned}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? StepIsSoftwareAssigned
        {
            get => F.AsBool(F.Memo(this, "StepIsSoftwareAssigned", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.IsSoftwareAssigned), () => F.Of(new Step().IsSoftwareAssigned)))); set { }
        }

        // Formula SoftwareDidHumanWork (rulebook: =AND({{WasExecutedBySoftware}}, NOT({{StepIsSoftwareAssigned}})))
        [NotMapped]
        public bool? SoftwareDidHumanWork
        {
            get => F.AsBool(F.Memo(this, "SoftwareDidHumanWork", () => F.And(F.Bool3(F.Of(this.WasExecutedBySoftware)), F.Bool3(F.Not(F.Bool3(F.Of(this.StepIsSoftwareAssigned))))))); set { }
        }

        // Formula IsApprovalExecution (rulebook: =INDEX(Steps!{{IsHumanApprovalGate}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? IsApprovalExecution
        {
            get => F.AsBool(F.Memo(this, "IsApprovalExecution", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.IsHumanApprovalGate), () => F.Of(new Step().IsHumanApprovalGate)))); set { }
        }

        // Formula IsVerified (rulebook: =AND({{VerificationResult}} <> "", {{VerificationResult}} <> "PENDING", {{VerificationResult}} <> "FAIL"))
        [NotMapped]
        public bool? IsVerified
        {
            get => F.AsBool(F.Memo(this, "IsVerified", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.VerificationResult))), F.Bool3(F.Ne(F.Nullif(F.Of(this.VerificationResult)), F.S("PENDING"))), F.Bool3(F.Ne(F.Nullif(F.Of(this.VerificationResult)), F.S("FAIL")))))); set { }
        }

        // Formula UnconfirmedNonHumanDecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{StepExecutionWhenUnconfirmed}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? UnconfirmedNonHumanDecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "UnconfirmedNonHumanDecisionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.StepExecutionWhenUnconfirmed), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula RequiresHumanConfirmation (rulebook: =INDEX(Steps!{{RequiresHumanConfirmation}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? RequiresHumanConfirmation
        {
            get => F.AsBool(F.Memo(this, "RequiresHumanConfirmation", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.RequiresHumanConfirmation), () => F.Of(new Step().RequiresHumanConfirmation)))); set { }
        }

        // Formula HumanConfirmationMissing (rulebook: =AND({{RequiresHumanConfirmation}}, {{UnconfirmedNonHumanDecisionCount}} > 0))
        [NotMapped]
        public bool? HumanConfirmationMissing
        {
            get => F.AsBool(F.Memo(this, "HumanConfirmationMissing", () => F.And(F.Bool3(F.Of(this.RequiresHumanConfirmation)), F.Bool3(F.Cmp(F.Of(this.UnconfirmedNonHumanDecisionCount), ">", F.I(0)))))); set { }
        }

        // Formula DraftedFromUnusableSource (rulebook: =AND({{ExecutionStatus}} = "Completed", NOT({{InputsWereUsable}})))
        [NotMapped]
        public bool? DraftedFromUnusableSource
        {
            get => F.AsBool(F.Memo(this, "DraftedFromUnusableSource", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ExecutionStatus)), F.S("Completed"))), F.Bool3(F.Not(F.Bool3(F.Of(this.InputsWereUsable))))))); set { }
        }

        // Formula InputsWereUsable (rulebook: =INDEX(Steps!{{AllSourcesUsable}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? InputsWereUsable
        {
            get => F.AsBool(F.Memo(this, "InputsWereUsable", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.AllSourcesUsable), () => F.Of(new Step().AllSourcesUsable)))); set { }
        }

        // Formula SoftwareExecutionStepKey (rulebook: =IF({{WasExecutedBySoftware}}, {{Step}}, ""))
        [NotMapped]
        public string? SoftwareExecutionStepKey
        {
            get => F.AsString(F.Memo(this, "SoftwareExecutionStepKey", () => (F.Truthy(F.Bool3(F.Of(this.WasExecutedBySoftware))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula StepControlKind (rulebook: =INDEX(Steps!{{ControlKind}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? StepControlKind
        {
            get => F.AsString(F.Memo(this, "StepControlKind", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.ControlKind), () => F.Of(new Step().ControlKind)))); set { }
        }

        // Formula UnfalsifiedClearanceCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{UnfalsifiedClearanceStepKey}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? UnfalsifiedClearanceCount
        {
            get => F.AsDecimal(F.Memo(this, "UnfalsifiedClearanceCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.UnfalsifiedClearanceStepKey), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula AllClearancesAreUnfalsified (rulebook: =AND({{EvaluatedBlockingCount}} > 0, {{UnfalsifiedClearanceCount}} >= {{EvaluatedBlockingCount}}))
        [NotMapped]
        public bool? AllClearancesAreUnfalsified
        {
            get => F.AsBool(F.Memo(this, "AllClearancesAreUnfalsified", () => F.And(F.Bool3(F.Cmp(F.Of(this.EvaluatedBlockingCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.UnfalsifiedClearanceCount), ">=", F.Of(this.EvaluatedBlockingCount)))))); set { }
        }

        // Formula StaleAtRunCount (rulebook: =COUNTIFS(BindingObservations!{{StaleAtRunStepKey}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? StaleAtRunCount
        {
            get => F.AsDecimal(F.Memo(this, "StaleAtRunCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<BindingObservation>(base.SoAContext, "BindingObservations", __c => __c.BindingObservations), __r => F.CritField(F.Of(__r.StaleAtRunStepKey), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula WasStaleWhenIRanIt (rulebook: ={{StaleAtRunCount}} > 0)
        [NotMapped]
        public bool? WasStaleWhenIRanIt
        {
            get => F.AsBool(F.Memo(this, "WasStaleWhenIRanIt", () => F.Cmp(F.Of(this.StaleAtRunCount), ">", F.I(0)))); set { }
        }

        // Formula StalenessAnswerIsTenseDependent (rulebook: =NOT({{WasStaleWhenIRanIt}} = {{RanOnStaleAuthoritativeSource}}))
        [NotMapped]
        public bool? StalenessAnswerIsTenseDependent
        {
            get => F.AsBool(F.Memo(this, "StalenessAnswerIsTenseDependent", () => F.Not(F.Bool3(F.Eq(F.Of(this.WasStaleWhenIRanIt), F.Of(this.RanOnStaleAuthoritativeSource)))))); set { }
        }

        // Formula HasAnyDeclaredCheck (rulebook: =OR({{ExpectedVerificationCount}} > 0, {{ExpectedBlockingCount}} > 0))
        [NotMapped]
        public bool? HasAnyDeclaredCheck
        {
            get => F.AsBool(F.Memo(this, "HasAnyDeclaredCheck", () => F.Or(F.Bool3(F.Cmp(F.Of(this.ExpectedVerificationCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.ExpectedBlockingCount), ">", F.I(0)))))); set { }
        }

        // Formula PerformedCheckCount (rulebook: ={{PerformedVerificationCount}} + {{EvaluatedBlockingCount}})
        [NotMapped]
        public decimal? PerformedCheckCount
        {
            get => F.AsDecimal(F.Memo(this, "PerformedCheckCount", () => F.Add(F.Of(this.PerformedVerificationCount), F.Of(this.EvaluatedBlockingCount)))); set { }
        }

        // Formula DeclaredCheckCount (rulebook: ={{ExpectedVerificationCount}} + {{ExpectedBlockingCount}})
        [NotMapped]
        public decimal? DeclaredCheckCount
        {
            get => F.AsDecimal(F.Memo(this, "DeclaredCheckCount", () => F.Add(F.Of(this.ExpectedVerificationCount), F.Of(this.ExpectedBlockingCount)))); set { }
        }

        // Formula IsUncheckedByDesign (rulebook: ={{DeclaredCheckCount}} = 0)
        [NotMapped]
        public bool? IsUncheckedByDesign
        {
            get => F.AsBool(F.Memo(this, "IsUncheckedByDesign", () => F.Eq(F.Of(this.DeclaredCheckCount), F.I(0)))); set { }
        }

        // Formula IsVacuouslyClean (rulebook: =AND({{IsClean}}, {{IsUncheckedByDesign}}))
        [NotMapped]
        public bool? IsVacuouslyClean
        {
            get => F.AsBool(F.Memo(this, "IsVacuouslyClean", () => F.And(F.Bool3(F.Of(this.IsClean)), F.Bool3(F.Of(this.IsUncheckedByDesign))))); set { }
        }

        // Formula IsSubstantivelyClean (rulebook: =AND({{IsClean}}, {{PerformedCheckCount}} >= {{DeclaredCheckCount}}, {{DeclaredCheckCount}} > 0))
        [NotMapped]
        public bool? IsSubstantivelyClean
        {
            get => F.AsBool(F.Memo(this, "IsSubstantivelyClean", () => F.And(F.Bool3(F.Of(this.IsClean)), F.Bool3(F.Cmp(F.Of(this.PerformedCheckCount), ">=", F.Of(this.DeclaredCheckCount))), F.Bool3(F.Cmp(F.Of(this.DeclaredCheckCount), ">", F.I(0)))))); set { }
        }

        // Formula VacuouslyCleanExecutionKey (rulebook: =IF({{IsVacuouslyClean}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? VacuouslyCleanExecutionKey
        {
            get => F.AsString(F.Memo(this, "VacuouslyCleanExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsVacuouslyClean))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula UncorroboratedPassCount (rulebook: =COUNTIFS(VerificationOutcomes!{{UncorroboratedPassStepKey}}, {{StepExecutionId}}))
        [NotMapped]
        public decimal? UncorroboratedPassCount
        {
            get => F.AsDecimal(F.Memo(this, "UncorroboratedPassCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<VerificationOutcome>(base.SoAContext, "VerificationOutcomes", __c => __c.VerificationOutcomes), __r => F.CritField(F.Of(__r.UncorroboratedPassStepKey), F.Of(this.StepExecutionId)))))); set { }
        }

        // Formula EvidencePositionIsWeak (rulebook: =AND({{PerformedVerificationCount}} > 0, {{UncorroboratedPassCount}} >= {{PerformedVerificationCount}}))
        [NotMapped]
        public bool? EvidencePositionIsWeak
        {
            get => F.AsBool(F.Memo(this, "EvidencePositionIsWeak", () => F.And(F.Bool3(F.Cmp(F.Of(this.PerformedVerificationCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.UncorroboratedPassCount), ">=", F.Of(this.PerformedVerificationCount)))))); set { }
        }

        // Formula PreparationExecutionKey (rulebook: =IF({{StepIsPreparation}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? PreparationExecutionKey
        {
            get => F.AsString(F.Memo(this, "PreparationExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.StepIsPreparation))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula ApprovalExecutionKey (rulebook: =IF({{StepIsApproval}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? ApprovalExecutionKey
        {
            get => F.AsString(F.Memo(this, "ApprovalExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.StepIsApproval))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula HasGoverningInstrument (rulebook: =OR({{RanUnderException}}, {{HasApprovedChangeCoverage}}))
        [NotMapped]
        public bool? HasGoverningInstrument
        {
            get => F.AsBool(F.Memo(this, "HasGoverningInstrument", () => F.Or(F.Bool3(F.Of(this.RanUnderException)), F.Bool3(F.Of(this.HasApprovedChangeCoverage))))); set { }
        }

        // Formula HasApprovedChangeCoverage (rulebook: =INDEX(ProcedureVersions!{{HasApprovedChangeRequest}}, MATCH({{VersionOfStep}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public bool? HasApprovedChangeCoverage
        {
            get => F.AsBool(F.Memo(this, "HasApprovedChangeCoverage", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.VersionOfStep), __r => F.Of(__r.HasApprovedChangeRequest), () => F.Of(new ProcedureVersion().HasApprovedChangeRequest)))); set { }
        }

        // Formula VersionOfStep (rulebook: =INDEX(Steps!{{ProcedureVersion}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? VersionOfStep
        {
            get => F.AsString(F.Memo(this, "VersionOfStep", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.ProcedureVersion), () => F.Of(new Step().ProcedureVersion)))); set { }
        }

        // Formula IsUngovernedDivergence (rulebook: =AND(OR({{HasDeviation}}, {{IsLate}}, {{ProceededPastBlockingControl}}), NOT({{HasGoverningInstrument}})))
        [NotMapped]
        public bool? IsUngovernedDivergence
        {
            get => F.AsBool(F.Memo(this, "IsUngovernedDivergence", () => F.And(F.Bool3(F.Or(F.Bool3(F.Of(this.HasDeviation)), F.Bool3(F.Of(this.IsLate)), F.Bool3(F.Of(this.ProceededPastBlockingControl)))), F.Bool3(F.Not(F.Bool3(F.Of(this.HasGoverningInstrument))))))); set { }
        }

        // Formula UngovernedDivergenceExecutionKey (rulebook: =IF({{IsUngovernedDivergence}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? UngovernedDivergenceExecutionKey
        {
            get => F.AsString(F.Memo(this, "UngovernedDivergenceExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUngovernedDivergence))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula SelfAttestedApprovalExecutionKey (rulebook: =IF({{ApprovalRestsOnSelfAttestation}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? SelfAttestedApprovalExecutionKey
        {
            get => F.AsString(F.Memo(this, "SelfAttestedApprovalExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.ApprovalRestsOnSelfAttestation))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureExecution { get; set; }
        public string? Step { get; set; }
        public string? ExecutedByAgent { get; set; }

        private ProcedureExecution _procedureExecutionRef;

        [ForeignKey("ProcedureExecution")]
        public virtual ProcedureExecution ProcedureExecutionRef
        {
            get
            {
                if (_procedureExecutionRef == null && !string.IsNullOrEmpty(ProcedureExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecutionRef - no database context is set. ProcedureExecution: " + ProcedureExecution + ".");
                        }
                        return null;
                    }
                    _procedureExecutionRef = base.SoAContext.ProcedureExecutions.Find(ProcedureExecution);
                    if (_procedureExecutionRef != null)
                    {
                        base.SoAContext.Attach(_procedureExecutionRef);
                    }
                }
                return _procedureExecutionRef;
            }
            set
            {
                if (_procedureExecutionRef != value)
                {
                    _procedureExecutionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureExecutionRef != null)
                    {
                        ProcedureExecution = _procedureExecutionRef.ProcedureExecutionId;
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

        private ObservableCollection<RequirementSatisfaction> _requirementSatisfactions;

        [InverseProperty("StepExecutionRef")]
        public virtual ObservableCollection<RequirementSatisfaction> RequirementSatisfactions
        {
            get
            {
                if (_requirementSatisfactions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequirementSatisfactions - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>();
                    }
                    else
                    {
                        var items = base.SoAContext.RequirementSatisfactions.Where(x => x.StepExecution == this.StepExecutionId).ToList<RequirementSatisfaction>();
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _requirementSatisfactions.CollectionChanged += RequirementSatisfactions_CollectionChanged;
                }
                return _requirementSatisfactions;
            }
            private set
            {
                if (_requirementSatisfactions != null)
                {
                    _requirementSatisfactions.CollectionChanged -= RequirementSatisfactions_CollectionChanged;
                }
                _requirementSatisfactions = value;
                if (_requirementSatisfactions != null)
                {
                    _requirementSatisfactions.CollectionChanged += RequirementSatisfactions_CollectionChanged;
                }
            }
        }

        private void RequirementSatisfactions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RequirementSatisfaction>())
                {
                    item.StepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<IssueOccurrence> _issueOccurrences;

        [InverseProperty("StepExecutionRef")]
        public virtual ObservableCollection<IssueOccurrence> IssueOccurrences
        {
            get
            {
                if (_issueOccurrences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IssueOccurrences - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>();
                    }
                    else
                    {
                        var items = base.SoAContext.IssueOccurrences.Where(x => x.StepExecution == this.StepExecutionId).ToList<IssueOccurrence>();
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _issueOccurrences.CollectionChanged += IssueOccurrences_CollectionChanged;
                }
                return _issueOccurrences;
            }
            private set
            {
                if (_issueOccurrences != null)
                {
                    _issueOccurrences.CollectionChanged -= IssueOccurrences_CollectionChanged;
                }
                _issueOccurrences = value;
                if (_issueOccurrences != null)
                {
                    _issueOccurrences.CollectionChanged += IssueOccurrences_CollectionChanged;
                }
            }
        }

        private void IssueOccurrences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<IssueOccurrence>())
                {
                    item.StepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<UserQuestion> _userQuestions;

        [InverseProperty("StepExecutionRef")]
        public virtual ObservableCollection<UserQuestion> UserQuestions
        {
            get
            {
                if (_userQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserQuestions - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _userQuestions = new ObservableCollection<UserQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.UserQuestions.Where(x => x.StepExecution == this.StepExecutionId).ToList<UserQuestion>();
                        _userQuestions = new ObservableCollection<UserQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _userQuestions.CollectionChanged += UserQuestions_CollectionChanged;
                }
                return _userQuestions;
            }
            private set
            {
                if (_userQuestions != null)
                {
                    _userQuestions.CollectionChanged -= UserQuestions_CollectionChanged;
                }
                _userQuestions = value;
                if (_userQuestions != null)
                {
                    _userQuestions.CollectionChanged += UserQuestions_CollectionChanged;
                }
            }
        }

        private void UserQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<UserQuestion>())
                {
                    item.StepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<ExceptionInvocation> _exceptionInvocations;

        [InverseProperty("StepExecutionRef")]
        public virtual ObservableCollection<ExceptionInvocation> ExceptionInvocations
        {
            get
            {
                if (_exceptionInvocations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExceptionInvocations - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _exceptionInvocations = new ObservableCollection<ExceptionInvocation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExceptionInvocations.Where(x => x.StepExecution == this.StepExecutionId).ToList<ExceptionInvocation>();
                        _exceptionInvocations = new ObservableCollection<ExceptionInvocation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _exceptionInvocations.CollectionChanged += ExceptionInvocations_CollectionChanged;
                }
                return _exceptionInvocations;
            }
            private set
            {
                if (_exceptionInvocations != null)
                {
                    _exceptionInvocations.CollectionChanged -= ExceptionInvocations_CollectionChanged;
                }
                _exceptionInvocations = value;
                if (_exceptionInvocations != null)
                {
                    _exceptionInvocations.CollectionChanged += ExceptionInvocations_CollectionChanged;
                }
            }
        }

        private void ExceptionInvocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExceptionInvocation>())
                {
                    item.StepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<VerificationOutcome> _verificationOutcomes;

        [InverseProperty("StepExecutionRef")]
        public virtual ObservableCollection<VerificationOutcome> VerificationOutcomes
        {
            get
            {
                if (_verificationOutcomes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VerificationOutcomes - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>();
                    }
                    else
                    {
                        var items = base.SoAContext.VerificationOutcomes.Where(x => x.StepExecution == this.StepExecutionId).ToList<VerificationOutcome>();
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _verificationOutcomes.CollectionChanged += VerificationOutcomes_CollectionChanged;
                }
                return _verificationOutcomes;
            }
            private set
            {
                if (_verificationOutcomes != null)
                {
                    _verificationOutcomes.CollectionChanged -= VerificationOutcomes_CollectionChanged;
                }
                _verificationOutcomes = value;
                if (_verificationOutcomes != null)
                {
                    _verificationOutcomes.CollectionChanged += VerificationOutcomes_CollectionChanged;
                }
            }
        }

        private void VerificationOutcomes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<VerificationOutcome>())
                {
                    item.StepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<ObservedTransition> _observedTransitions;

        [InverseProperty("StepExecution")]
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
                            throw new InvalidOperationException("Cannot access ObservedTransitions - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _observedTransitions = new ObservableCollection<ObservedTransition>();
                    }
                    else
                    {
                        var items = base.SoAContext.ObservedTransitions.Where(x => x.ArrivingStepExecution == this.StepExecutionId).ToList<ObservedTransition>();
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
                    item.ArrivingStepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<MessageDelivery> _messageDeliveries;

        [InverseProperty("StepExecutionRef")]
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
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = base.SoAContext.MessageDeliveries.Where(x => x.StepExecution == this.StepExecutionId).ToList<MessageDelivery>();
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
                    item.StepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<SendIntent> _sendIntents;

        [InverseProperty("StepExecutionRef")]
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
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = base.SoAContext.SendIntents.Where(x => x.StepExecution == this.StepExecutionId).ToList<SendIntent>();
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
                    item.StepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<AgentDecisionRecord> _agentDecisionRecords;

        [InverseProperty("StepExecutionRef")]
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
                            throw new InvalidOperationException("Cannot access AgentDecisionRecords - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _agentDecisionRecords = new ObservableCollection<AgentDecisionRecord>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentDecisionRecords.Where(x => x.StepExecution == this.StepExecutionId).ToList<AgentDecisionRecord>();
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
                    item.StepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<DeliveredCommunication> _sendingStepExecutionDeliveredCommunications;

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<DeliveredCommunication> SendingStepExecutionDeliveredCommunications
        {
            get
            {
                if (_sendingStepExecutionDeliveredCommunications == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SendingStepExecutionDeliveredCommunications - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _sendingStepExecutionDeliveredCommunications = new ObservableCollection<DeliveredCommunication>();
                    }
                    else
                    {
                        var items = base.SoAContext.DeliveredCommunications.Where(x => x.SendingStepExecution == this.StepExecutionId).ToList<DeliveredCommunication>();
                        _sendingStepExecutionDeliveredCommunications = new ObservableCollection<DeliveredCommunication>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _sendingStepExecutionDeliveredCommunications.CollectionChanged += SendingStepExecutionDeliveredCommunications_CollectionChanged;
                }
                return _sendingStepExecutionDeliveredCommunications;
            }
            private set
            {
                if (_sendingStepExecutionDeliveredCommunications != null)
                {
                    _sendingStepExecutionDeliveredCommunications.CollectionChanged -= SendingStepExecutionDeliveredCommunications_CollectionChanged;
                }
                _sendingStepExecutionDeliveredCommunications = value;
                if (_sendingStepExecutionDeliveredCommunications != null)
                {
                    _sendingStepExecutionDeliveredCommunications.CollectionChanged += SendingStepExecutionDeliveredCommunications_CollectionChanged;
                }
            }
        }

        private void SendingStepExecutionDeliveredCommunications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DeliveredCommunication>())
                {
                    item.SendingStepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<DeliveredCommunication> _authorizingStepExecutionDeliveredCommunications;

        [InverseProperty("StepExecutionRef")]
        public virtual ObservableCollection<DeliveredCommunication> AuthorizingStepExecutionDeliveredCommunications
        {
            get
            {
                if (_authorizingStepExecutionDeliveredCommunications == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorizingStepExecutionDeliveredCommunications - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _authorizingStepExecutionDeliveredCommunications = new ObservableCollection<DeliveredCommunication>();
                    }
                    else
                    {
                        var items = base.SoAContext.DeliveredCommunications.Where(x => x.AuthorizingStepExecution == this.StepExecutionId).ToList<DeliveredCommunication>();
                        _authorizingStepExecutionDeliveredCommunications = new ObservableCollection<DeliveredCommunication>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _authorizingStepExecutionDeliveredCommunications.CollectionChanged += AuthorizingStepExecutionDeliveredCommunications_CollectionChanged;
                }
                return _authorizingStepExecutionDeliveredCommunications;
            }
            private set
            {
                if (_authorizingStepExecutionDeliveredCommunications != null)
                {
                    _authorizingStepExecutionDeliveredCommunications.CollectionChanged -= AuthorizingStepExecutionDeliveredCommunications_CollectionChanged;
                }
                _authorizingStepExecutionDeliveredCommunications = value;
                if (_authorizingStepExecutionDeliveredCommunications != null)
                {
                    _authorizingStepExecutionDeliveredCommunications.CollectionChanged += AuthorizingStepExecutionDeliveredCommunications_CollectionChanged;
                }
            }
        }

        private void AuthorizingStepExecutionDeliveredCommunications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DeliveredCommunication>())
                {
                    item.AuthorizingStepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<BindingObservation> _bindingObservations;

        [InverseProperty("StepExecutionRef")]
        public virtual ObservableCollection<BindingObservation> BindingObservations
        {
            get
            {
                if (_bindingObservations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access BindingObservations - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _bindingObservations = new ObservableCollection<BindingObservation>();
                    }
                    else
                    {
                        var items = base.SoAContext.BindingObservations.Where(x => x.StepExecution == this.StepExecutionId).ToList<BindingObservation>();
                        _bindingObservations = new ObservableCollection<BindingObservation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _bindingObservations.CollectionChanged += BindingObservations_CollectionChanged;
                }
                return _bindingObservations;
            }
            private set
            {
                if (_bindingObservations != null)
                {
                    _bindingObservations.CollectionChanged -= BindingObservations_CollectionChanged;
                }
                _bindingObservations = value;
                if (_bindingObservations != null)
                {
                    _bindingObservations.CollectionChanged += BindingObservations_CollectionChanged;
                }
            }
        }

        private void BindingObservations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<BindingObservation>())
                {
                    item.StepExecution = this.StepExecutionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureExecutionRef;
            _ = this.StepRef;
            _ = this.Agent;
            _ = this.RequirementSatisfactions;
            _ = this.IssueOccurrences;
            _ = this.UserQuestions;
            _ = this.ExceptionInvocations;
            _ = this.VerificationOutcomes;
            _ = this.ObservedTransitions;
            _ = this.MessageDeliveries;
            _ = this.SendIntents;
            _ = this.AgentDecisionRecords;
            _ = this.SendingStepExecutionDeliveredCommunications;
            _ = this.AuthorizingStepExecutionDeliveredCommunications;
            _ = this.BindingObservations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
