
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("StepExecutions")]
    public class StepExecutionBase : SoAEntityBase
    {
        [Key]
        public string StepExecutionId { get; set; }

        // Formula Name (rulebook: ={{ProcedureExecution}} & " / " & {{Step}})
        public string? Name
        {
            get => this.ProcedureExecution + " / " + this.Step; set { }
        }

        public string? ExecutionStatus { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? VerificationResult { get; set; }
        public string? Deviation { get; set; }
        // Formula ActualDurationMinutes (rulebook: =IF({{EndedAt}} = "", 0, DATETIME_DIFF({{EndedAt}}, {{StartedAt}}, "minutes")))
        public int? ActualDurationMinutes
        {
            get => IF(this.EndedAt = "", 0, DATETIME_DIFF(this.EndedAt, this.StartedAt, "minutes")); set { }
        }

        // Formula ExpectedDurationMinutes (rulebook: =INDEX(Steps!{{ExpectedDurationMinutes}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public int? ExpectedDurationMinutes
        {
            get => INDEX(Steps!this.ExpectedDurationMinutes, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula IsLate (rulebook: ={{ActualDurationMinutes}} > {{ExpectedDurationMinutes}})
        public bool? IsLate
        {
            get => this.ActualDurationMinutes > this.ExpectedDurationMinutes; set { }
        }

        // Formula BlockingUnmetCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{StepExecution}}, {{StepExecutionId}}, RequirementSatisfactions!{{IsBlockingAndUnmet}}, TRUE))
        public decimal? BlockingUnmetCount
        {
            get => COUNTIFS(RequirementSatisfactions!this.StepExecution, this.StepExecutionId, RequirementSatisfactions!this.IsBlockingAndUnmet, TRUE); set { }
        }

        // Formula BlockingUnmetCountSafe (rulebook: =COUNTIFS(RequirementSatisfactions!{{BlockingUnmetStepKey}}, {{StepExecutionId}}))
        public decimal? BlockingUnmetCountSafe
        {
            get => COUNTIFS(RequirementSatisfactions!this.BlockingUnmetStepKey, this.StepExecutionId); set { }
        }

        // Formula ProceededPastBlockingControl (rulebook: =AND({{ExecutionStatus}} = "Completed", {{BlockingUnmetCountSafe}} > 0))
        public bool? ProceededPastBlockingControl
        {
            get => AND(this.ExecutionStatus = "Completed", this.BlockingUnmetCountSafe > 0); set { }
        }

        // Formula ExpectedBlockingCount (rulebook: =INDEX(Steps!{{BlockingRequirementCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public decimal? ExpectedBlockingCount
        {
            get => INDEX(Steps!this.BlockingRequirementCount, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula EvaluatedBlockingCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{BlockingSatisfactionStepKey}}, {{StepExecutionId}}))
        public decimal? EvaluatedBlockingCount
        {
            get => COUNTIFS(RequirementSatisfactions!this.BlockingSatisfactionStepKey, this.StepExecutionId); set { }
        }

        // Formula UnevaluatedBlockingCount (rulebook: ={{ExpectedBlockingCount}} - {{EvaluatedBlockingCount}})
        public decimal? UnevaluatedBlockingCount
        {
            get => this.ExpectedBlockingCount - this.EvaluatedBlockingCount; set { }
        }

        // Formula HasUnevaluatedBlockingControl (rulebook: ={{UnevaluatedBlockingCount}} > 0)
        public bool? HasUnevaluatedBlockingControl
        {
            get => this.UnevaluatedBlockingCount > 0; set { }
        }

        // Formula StaleAuthoritativeSourceCount (rulebook: =INDEX(Steps!{{AuthoritativeStaleCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public decimal? StaleAuthoritativeSourceCount
        {
            get => INDEX(Steps!this.AuthoritativeStaleCount, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula RanOnStaleAuthoritativeSource (rulebook: ={{StaleAuthoritativeSourceCount}} > 0)
        public bool? RanOnStaleAuthoritativeSource
        {
            get => this.StaleAuthoritativeSourceCount > 0; set { }
        }

        // Formula HasDeviationNote (rulebook: ={{Deviation}} <> "")
        public bool? HasDeviationNote
        {
            get => this.Deviation <> ""; set { }
        }

        // Formula IsLateAndUnexplained (rulebook: =AND({{IsLate}}, NOT({{HasDeviationNote}})))
        public bool? IsLateAndUnexplained
        {
            get => AND(this.IsLate, NOT(this.HasDeviationNote)); set { }
        }

        // Formula AvailableExceptionCountForStep (rulebook: =INDEX(Steps!{{AvailableExceptionCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public decimal? AvailableExceptionCountForStep
        {
            get => INDEX(Steps!this.AvailableExceptionCount, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula HadUninvokedExceptionAvailable (rulebook: =AND({{IsLateAndUnexplained}}, {{AvailableExceptionCountForStep}} > 0))
        public bool? HadUninvokedExceptionAvailable
        {
            get => AND(this.IsLateAndUnexplained, this.AvailableExceptionCountForStep > 0); set { }
        }

        // Formula ExpectedVerificationCount (rulebook: =INDEX(Steps!{{DeclaredVerificationCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public decimal? ExpectedVerificationCount
        {
            get => INDEX(Steps!this.DeclaredVerificationCount, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula PerformedVerificationCount (rulebook: =COUNTIFS(VerificationOutcomes!{{StepExecution}}, {{StepExecutionId}}))
        public decimal? PerformedVerificationCount
        {
            get => COUNTIFS(VerificationOutcomes!this.StepExecution, this.StepExecutionId); set { }
        }

        // Formula SkippedVerificationCount (rulebook: ={{ExpectedVerificationCount}} - {{PerformedVerificationCount}})
        public decimal? SkippedVerificationCount
        {
            get => this.ExpectedVerificationCount - this.PerformedVerificationCount; set { }
        }

        // Formula HasSkippedVerification (rulebook: ={{SkippedVerificationCount}} > 0)
        public bool? HasSkippedVerification
        {
            get => this.SkippedVerificationCount > 0; set { }
        }

        // Formula ClaimsPassWithoutEvidence (rulebook: =AND({{VerificationResult}} = "PASS", {{HasSkippedVerification}}))
        public bool? ClaimsPassWithoutEvidence
        {
            get => AND(this.VerificationResult = "PASS", this.HasSkippedVerification); set { }
        }

        // Formula StepIsPreparation (rulebook: =INDEX(Steps!{{IsPreparationStep}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public bool? StepIsPreparation
        {
            get => INDEX(Steps!this.IsPreparationStep, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula StepIsApproval (rulebook: =INDEX(Steps!{{IsApprovalStep}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public bool? StepIsApproval
        {
            get => INDEX(Steps!this.IsApprovalStep, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula PreparerAgentKey (rulebook: =IF({{StepIsPreparation}}, {{ProcedureExecution}} & "|" & {{ExecutedByAgent}}, ""))
        public string? PreparerAgentKey
        {
            get => IF(this.StepIsPreparation, this.ProcedureExecution + "|" + this.ExecutedByAgent, ""); set { }
        }

        // Formula ApproverAgentKey (rulebook: =IF({{StepIsApproval}}, {{ProcedureExecution}} & "|" & {{ExecutedByAgent}}, ""))
        public string? ApproverAgentKey
        {
            get => IF(this.StepIsApproval, this.ProcedureExecution + "|" + this.ExecutedByAgent, ""); set { }
        }

        // Formula PreparedByThisAgentCount (rulebook: =COUNTIFS(StepExecutions!{{PreparerAgentKey}}, {{ApproverAgentKey}}))
        public decimal? PreparedByThisAgentCount
        {
            get => COUNTIFS(StepExecutions!this.PreparerAgentKey, this.ApproverAgentKey); set { }
        }

        // Formula ViolatesSeparationOfDuties (rulebook: =AND({{StepIsApproval}}, {{PreparedByThisAgentCount}} > 0))
        public bool? ViolatesSeparationOfDuties
        {
            get => AND(this.StepIsApproval, this.PreparedByThisAgentCount > 0); set { }
        }

        // Formula RequiredRoleForStep (rulebook: =INDEX(Steps!{{AssignedRole}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public string? RequiredRoleForStep
        {
            get => INDEX(Steps!this.AssignedRole, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula ExecutorRoleKey (rulebook: ={{ExecutedByAgent}} & "|" & {{RequiredRoleForStep}})
        public string? ExecutorRoleKey
        {
            get => this.ExecutedByAgent + "|" + this.RequiredRoleForStep; set { }
        }

        // Formula ExecutorAuthorityCount (rulebook: =COUNTIFS(RoleAssignments!{{AgentRoleKey}}, {{ExecutorRoleKey}}))
        public decimal? ExecutorAuthorityCount
        {
            get => COUNTIFS(RoleAssignments!this.AgentRoleKey, this.ExecutorRoleKey); set { }
        }

        // Formula ExecutorHeldRequiredRole (rulebook: ={{ExecutorAuthorityCount}} > 0)
        public bool? ExecutorHeldRequiredRole
        {
            get => this.ExecutorAuthorityCount > 0; set { }
        }

        // Formula IsUnauthorizedApproval (rulebook: =AND({{StepIsApproval}}, NOT({{ExecutorHeldRequiredRole}})))
        public bool? IsUnauthorizedApproval
        {
            get => AND(this.StepIsApproval, NOT(this.ExecutorHeldRequiredRole)); set { }
        }

        // Formula CompletedExecutionKey (rulebook: =IF({{ExecutionStatus}} = "Completed", {{ProcedureExecution}}, ""))
        public string? CompletedExecutionKey
        {
            get => IF(this.ExecutionStatus = "Completed", this.ProcedureExecution, ""); set { }
        }

        // Formula ControlBreachExecutionKey (rulebook: =IF(OR({{ProceededPastBlockingControl}}, {{ViolatesSeparationOfDuties}}, {{IsUnauthorizedApproval}}, {{ClaimsPassWithoutEvidence}}), {{ProcedureExecution}}, ""))
        public string? ControlBreachExecutionKey
        {
            get => IF(OR(this.ProceededPastBlockingControl, this.ViolatesSeparationOfDuties, this.IsUnauthorizedApproval, this.ClaimsPassWithoutEvidence), this.ProcedureExecution, ""); set { }
        }

        // Formula LateExecutionKey (rulebook: =IF({{IsLate}}, {{ProcedureExecution}}, ""))
        public string? LateExecutionKey
        {
            get => IF(this.IsLate, this.ProcedureExecution, ""); set { }
        }

        // Formula ExecutorAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{ExecutedByAgent}}, Agents!{{AgentId}}, 0)))
        public string? ExecutorAgentKind
        {
            get => INDEX(Agents!this.AgentKind, MATCH(this.ExecutedByAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula ExecutorIsHuman (rulebook: ={{ExecutorAgentKind}} = "Human")
        public bool? ExecutorIsHuman
        {
            get => this.ExecutorAgentKind = "Human"; set { }
        }

        // Formula StepRequiresHumanConfirmation (rulebook: =INDEX(Steps!{{RequiresHumanConfirmation}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public bool? StepRequiresHumanConfirmation
        {
            get => INDEX(Steps!this.RequiresHumanConfirmation, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula NonHumanRanHumanStep (rulebook: =AND({{StepRequiresHumanConfirmation}}, NOT({{ExecutorIsHuman}})))
        public bool? NonHumanRanHumanStep
        {
            get => AND(this.StepRequiresHumanConfirmation, NOT(this.ExecutorIsHuman)); set { }
        }

        // Formula NonHumanApproval (rulebook: =AND({{StepIsApproval}}, NOT({{ExecutorIsHuman}})))
        public bool? NonHumanApproval
        {
            get => AND(this.StepIsApproval, NOT(this.ExecutorIsHuman)); set { }
        }

        // Formula UnevaluatedBlockingExecutionKey (rulebook: =IF({{HasUnevaluatedBlockingControl}}, {{ProcedureExecution}}, ""))
        public string? UnevaluatedBlockingExecutionKey
        {
            get => IF(this.HasUnevaluatedBlockingControl, this.ProcedureExecution, ""); set { }
        }

        // Formula SeparationViolationExecutionKey (rulebook: =IF({{ViolatesSeparationOfDuties}}, {{ProcedureExecution}}, ""))
        public string? SeparationViolationExecutionKey
        {
            get => IF(this.ViolatesSeparationOfDuties, this.ProcedureExecution, ""); set { }
        }

        // Formula SelfWitnessedVerificationCount (rulebook: =COUNTIFS(VerificationOutcomes!{{SelfWitnessedStepKey}}, {{StepExecutionId}}))
        public decimal? SelfWitnessedVerificationCount
        {
            get => COUNTIFS(VerificationOutcomes!this.SelfWitnessedStepKey, this.StepExecutionId); set { }
        }

        // Formula UnbackedVerificationCount (rulebook: =COUNTIFS(VerificationOutcomes!{{UnbackedStepKey}}, {{StepExecutionId}}))
        public decimal? UnbackedVerificationCount
        {
            get => COUNTIFS(VerificationOutcomes!this.UnbackedStepKey, this.StepExecutionId); set { }
        }

        // Formula ApprovalRestsOnSelfAttestation (rulebook: =AND({{StepIsApproval}}, OR({{SelfWitnessedVerificationCount}} > 0, {{HasSkippedVerification}})))
        public bool? ApprovalRestsOnSelfAttestation
        {
            get => AND(this.StepIsApproval, OR(this.SelfWitnessedVerificationCount > 0, this.HasSkippedVerification)); set { }
        }

        // Formula ExceptionInvocationCount (rulebook: =COUNTIFS(ExceptionInvocations!{{StepExecution}}, {{StepExecutionId}}))
        public decimal? ExceptionInvocationCount
        {
            get => COUNTIFS(ExceptionInvocations!this.StepExecution, this.StepExecutionId); set { }
        }

        // Formula RanUnderException (rulebook: ={{ExceptionInvocationCount}} > 0)
        public bool? RanUnderException
        {
            get => this.ExceptionInvocationCount > 0; set { }
        }

        // Formula IsCompleted (rulebook: ={{ExecutionStatus}} = "Completed")
        public bool? IsCompleted
        {
            get => this.ExecutionStatus = "Completed"; set { }
        }

        // Formula IsVerificationPassed (rulebook: ={{VerificationResult}} = "PASS")
        public bool? IsVerificationPassed
        {
            get => this.VerificationResult = "PASS"; set { }
        }

        // Formula IsLegalReviewStep (rulebook: ={{Step}} = "policy-04")
        public bool? IsLegalReviewStep
        {
            get => this.Step = "policy-04"; set { }
        }

        // Formula ClearedLegalReviewKey (rulebook: =IF(AND({{IsLegalReviewStep}}, {{IsVerificationPassed}}), {{ProcedureExecution}}, ""))
        public string? ClearedLegalReviewKey
        {
            get => IF(AND(this.IsLegalReviewStep, this.IsVerificationPassed), this.ProcedureExecution, ""); set { }
        }

        // Formula AssignedRole (rulebook: =INDEX(Steps!{{AssignedRole}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public string? AssignedRole
        {
            get => INDEX(Steps!this.AssignedRole, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula RoleCurrentAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        public string? RoleCurrentAgent
        {
            get => INDEX(Roles!this.CurrentAgent, MATCH(this.AssignedRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula ExecutorIsDesignatedAgent (rulebook: ={{ExecutedByAgent}} = {{RoleCurrentAgent}})
        public bool? ExecutorIsDesignatedAgent
        {
            get => this.ExecutedByAgent = this.RoleCurrentAgent; set { }
        }

        // Formula InputsWereFreshAtRun (rulebook: =INDEX(Steps!{{InputsAreFresh}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public bool? InputsWereFreshAtRun
        {
            get => INDEX(Steps!this.InputsAreFresh, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula RanOnStaleInputs (rulebook: =AND({{ExecutionStatus}} = "Completed", NOT({{InputsWereFreshAtRun}})))
        public bool? RanOnStaleInputs
        {
            get => AND(this.ExecutionStatus = "Completed", NOT(this.InputsWereFreshAtRun)); set { }
        }

        // Formula UnresolvedIssueCount (rulebook: =COUNTIFS(IssueOccurrences!{{StepExecutionWhenUnresolved}}, {{StepExecutionId}}))
        public decimal? UnresolvedIssueCount
        {
            get => COUNTIFS(IssueOccurrences!this.StepExecutionWhenUnresolved, this.StepExecutionId); set { }
        }

        // Formula HasDeviation (rulebook: ={{Deviation}} <> "")
        public bool? HasDeviation
        {
            get => this.Deviation <> ""; set { }
        }

        // Formula IsClean (rulebook: =AND({{VerificationResult}} = "PASS", NOT({{HasDeviation}}), {{UnresolvedIssueCount}} = 0, NOT({{IsLate}})))
        public bool? IsClean
        {
            get => AND(this.VerificationResult = "PASS", NOT(this.HasDeviation), this.UnresolvedIssueCount = 0, NOT(this.IsLate)); set { }
        }

        // Formula ProcedureExecutionWhenUnclean (rulebook: =IF({{IsClean}}, "", {{ProcedureExecution}}))
        public string? ProcedureExecutionWhenUnclean
        {
            get => IF(this.IsClean, "", this.ProcedureExecution); set { }
        }

        // Formula EvaluatedRequirementCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{StepExecutionWhenScored}}, {{StepExecutionId}}))
        public decimal? EvaluatedRequirementCount
        {
            get => COUNTIFS(RequirementSatisfactions!this.StepExecutionWhenScored, this.StepExecutionId); set { }
        }

        // Formula RequiredBlockingCount (rulebook: =INDEX(Steps!{{BlockingRequirementCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public int? RequiredBlockingCount
        {
            get => INDEX(Steps!this.BlockingRequirementCount, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula HasUnevaluatedBlockingRequirement (rulebook: ={{EvaluatedRequirementCount}} < {{RequiredBlockingCount}})
        public bool? HasUnevaluatedBlockingRequirement
        {
            get => this.EvaluatedRequirementCount < this.RequiredBlockingCount; set { }
        }

        // Formula ExecutingAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{ExecutedByAgent}}, Agents!{{AgentId}}, 0)))
        public string? ExecutingAgentKind
        {
            get => INDEX(Agents!this.AgentKind, MATCH(this.ExecutedByAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula WasExecutedBySoftware (rulebook: =OR({{ExecutingAgentKind}} = "AIAgent", {{ExecutingAgentKind}} = "AutomatedPipeline"))
        public bool? WasExecutedBySoftware
        {
            get => OR(this.ExecutingAgentKind = "AIAgent", this.ExecutingAgentKind = "AutomatedPipeline"); set { }
        }

        // Formula StepIsSoftwareAssigned (rulebook: =INDEX(Steps!{{IsSoftwareAssigned}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public bool? StepIsSoftwareAssigned
        {
            get => INDEX(Steps!this.IsSoftwareAssigned, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula SoftwareDidHumanWork (rulebook: =AND({{WasExecutedBySoftware}}, NOT({{StepIsSoftwareAssigned}})))
        public bool? SoftwareDidHumanWork
        {
            get => AND(this.WasExecutedBySoftware, NOT(this.StepIsSoftwareAssigned)); set { }
        }

        // Formula IsApprovalExecution (rulebook: =INDEX(Steps!{{IsHumanApprovalGate}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public bool? IsApprovalExecution
        {
            get => INDEX(Steps!this.IsHumanApprovalGate, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula IsVerified (rulebook: =AND({{VerificationResult}} <> "", {{VerificationResult}} <> "PENDING", {{VerificationResult}} <> "FAIL"))
        public bool? IsVerified
        {
            get => AND(this.VerificationResult <> "", this.VerificationResult <> "PENDING", this.VerificationResult <> "FAIL"); set { }
        }

        // Formula UnconfirmedNonHumanDecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{StepExecutionWhenUnconfirmed}}, {{StepExecutionId}}))
        public decimal? UnconfirmedNonHumanDecisionCount
        {
            get => COUNTIFS(AgentDecisionRecords!this.StepExecutionWhenUnconfirmed, this.StepExecutionId); set { }
        }

        // Formula RequiresHumanConfirmation (rulebook: =INDEX(Steps!{{RequiresHumanConfirmation}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public bool? RequiresHumanConfirmation
        {
            get => INDEX(Steps!this.RequiresHumanConfirmation, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula HumanConfirmationMissing (rulebook: =AND({{RequiresHumanConfirmation}}, {{UnconfirmedNonHumanDecisionCount}} > 0))
        public bool? HumanConfirmationMissing
        {
            get => AND(this.RequiresHumanConfirmation, this.UnconfirmedNonHumanDecisionCount > 0); set { }
        }

        // Formula DraftedFromUnusableSource (rulebook: =AND({{ExecutionStatus}} = "Completed", NOT({{InputsWereUsable}})))
        public bool? DraftedFromUnusableSource
        {
            get => AND(this.ExecutionStatus = "Completed", NOT(this.InputsWereUsable)); set { }
        }

        // Formula InputsWereUsable (rulebook: =INDEX(Steps!{{AllSourcesUsable}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public bool? InputsWereUsable
        {
            get => INDEX(Steps!this.AllSourcesUsable, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula SoftwareExecutionStepKey (rulebook: =IF({{WasExecutedBySoftware}}, {{Step}}, ""))
        public string? SoftwareExecutionStepKey
        {
            get => IF(this.WasExecutedBySoftware, this.Step, ""); set { }
        }

        // Formula StepControlKind (rulebook: =INDEX(Steps!{{ControlKind}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public string? StepControlKind
        {
            get => INDEX(Steps!this.ControlKind, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula UnfalsifiedClearanceCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{UnfalsifiedClearanceStepKey}}, {{StepExecutionId}}))
        public decimal? UnfalsifiedClearanceCount
        {
            get => COUNTIFS(RequirementSatisfactions!this.UnfalsifiedClearanceStepKey, this.StepExecutionId); set { }
        }

        // Formula AllClearancesAreUnfalsified (rulebook: =AND({{EvaluatedBlockingCount}} > 0, {{UnfalsifiedClearanceCount}} >= {{EvaluatedBlockingCount}}))
        public bool? AllClearancesAreUnfalsified
        {
            get => AND(this.EvaluatedBlockingCount > 0, this.UnfalsifiedClearanceCount >= this.EvaluatedBlockingCount); set { }
        }

        // Formula StaleAtRunCount (rulebook: =COUNTIFS(BindingObservations!{{StaleAtRunStepKey}}, {{StepExecutionId}}))
        public decimal? StaleAtRunCount
        {
            get => COUNTIFS(BindingObservations!this.StaleAtRunStepKey, this.StepExecutionId); set { }
        }

        // Formula WasStaleWhenIRanIt (rulebook: ={{StaleAtRunCount}} > 0)
        public bool? WasStaleWhenIRanIt
        {
            get => this.StaleAtRunCount > 0; set { }
        }

        // Formula StalenessAnswerIsTenseDependent (rulebook: =NOT({{WasStaleWhenIRanIt}} = {{RanOnStaleAuthoritativeSource}}))
        public bool? StalenessAnswerIsTenseDependent
        {
            get => NOT(this.WasStaleWhenIRanIt = this.RanOnStaleAuthoritativeSource); set { }
        }

        // Formula HasAnyDeclaredCheck (rulebook: =OR({{ExpectedVerificationCount}} > 0, {{ExpectedBlockingCount}} > 0))
        public bool? HasAnyDeclaredCheck
        {
            get => OR(this.ExpectedVerificationCount > 0, this.ExpectedBlockingCount > 0); set { }
        }

        // Formula PerformedCheckCount (rulebook: ={{PerformedVerificationCount}} + {{EvaluatedBlockingCount}})
        public decimal? PerformedCheckCount
        {
            get => this.PerformedVerificationCount + this.EvaluatedBlockingCount; set { }
        }

        // Formula DeclaredCheckCount (rulebook: ={{ExpectedVerificationCount}} + {{ExpectedBlockingCount}})
        public decimal? DeclaredCheckCount
        {
            get => this.ExpectedVerificationCount + this.ExpectedBlockingCount; set { }
        }

        // Formula IsUncheckedByDesign (rulebook: ={{DeclaredCheckCount}} = 0)
        public bool? IsUncheckedByDesign
        {
            get => this.DeclaredCheckCount = 0; set { }
        }

        // Formula IsVacuouslyClean (rulebook: =AND({{IsClean}}, {{IsUncheckedByDesign}}))
        public bool? IsVacuouslyClean
        {
            get => AND(this.IsClean, this.IsUncheckedByDesign); set { }
        }

        // Formula IsSubstantivelyClean (rulebook: =AND({{IsClean}}, {{PerformedCheckCount}} >= {{DeclaredCheckCount}}, {{DeclaredCheckCount}} > 0))
        public bool? IsSubstantivelyClean
        {
            get => AND(this.IsClean, this.PerformedCheckCount >= this.DeclaredCheckCount, this.DeclaredCheckCount > 0); set { }
        }

        // Formula VacuouslyCleanExecutionKey (rulebook: =IF({{IsVacuouslyClean}}, {{ProcedureExecution}}, ""))
        public string? VacuouslyCleanExecutionKey
        {
            get => IF(this.IsVacuouslyClean, this.ProcedureExecution, ""); set { }
        }

        // Formula UncorroboratedPassCount (rulebook: =COUNTIFS(VerificationOutcomes!{{UncorroboratedPassStepKey}}, {{StepExecutionId}}))
        public decimal? UncorroboratedPassCount
        {
            get => COUNTIFS(VerificationOutcomes!this.UncorroboratedPassStepKey, this.StepExecutionId); set { }
        }

        // Formula EvidencePositionIsWeak (rulebook: =AND({{PerformedVerificationCount}} > 0, {{UncorroboratedPassCount}} >= {{PerformedVerificationCount}}))
        public bool? EvidencePositionIsWeak
        {
            get => AND(this.PerformedVerificationCount > 0, this.UncorroboratedPassCount >= this.PerformedVerificationCount); set { }
        }

        // Formula PreparationExecutionKey (rulebook: =IF({{StepIsPreparation}}, {{ProcedureExecution}}, ""))
        public string? PreparationExecutionKey
        {
            get => IF(this.StepIsPreparation, this.ProcedureExecution, ""); set { }
        }

        // Formula ApprovalExecutionKey (rulebook: =IF({{StepIsApproval}}, {{ProcedureExecution}}, ""))
        public string? ApprovalExecutionKey
        {
            get => IF(this.StepIsApproval, this.ProcedureExecution, ""); set { }
        }

        // Formula HasGoverningInstrument (rulebook: =OR({{RanUnderException}}, {{HasApprovedChangeCoverage}}))
        public bool? HasGoverningInstrument
        {
            get => OR(this.RanUnderException, this.HasApprovedChangeCoverage); set { }
        }

        // Formula HasApprovedChangeCoverage (rulebook: =INDEX(ProcedureVersions!{{HasApprovedChangeRequest}}, MATCH({{VersionOfStep}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        public bool? HasApprovedChangeCoverage
        {
            get => INDEX(ProcedureVersions!this.HasApprovedChangeRequest, MATCH(this.VersionOfStep, ProcedureVersions!this.ProcedureVersionId, 0)); set { }
        }

        // Formula VersionOfStep (rulebook: =INDEX(Steps!{{ProcedureVersion}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public string? VersionOfStep
        {
            get => INDEX(Steps!this.ProcedureVersion, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula IsUngovernedDivergence (rulebook: =AND(OR({{HasDeviation}}, {{IsLate}}, {{ProceededPastBlockingControl}}), NOT({{HasGoverningInstrument}})))
        public bool? IsUngovernedDivergence
        {
            get => AND(OR(this.HasDeviation, this.IsLate, this.ProceededPastBlockingControl), NOT(this.HasGoverningInstrument)); set { }
        }

        // Formula UngovernedDivergenceExecutionKey (rulebook: =IF({{IsUngovernedDivergence}}, {{ProcedureExecution}}, ""))
        public string? UngovernedDivergenceExecutionKey
        {
            get => IF(this.IsUngovernedDivergence, this.ProcedureExecution, ""); set { }
        }

        // Formula SelfAttestedApprovalExecutionKey (rulebook: =IF({{ApprovalRestsOnSelfAttestation}}, {{ProcedureExecution}}, ""))
        public string? SelfAttestedApprovalExecutionKey
        {
            get => IF(this.ApprovalRestsOnSelfAttestation, this.ProcedureExecution, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureExecution { get; set; }
        public string? Step { get; set; }
        public string? ExecutedByAgent { get; set; }

        private ProcedureExecution _procedureExecution;

        [ForeignKey("ProcedureExecution")]
        public virtual ProcedureExecution ProcedureExecution
        {
            get
            {
                if (_procedureExecution == null && !string.IsNullOrEmpty(ProcedureExecution))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecution - no database context is set. ProcedureExecution: " + ProcedureExecution + ".");
                        }
                        return null;
                    }
                    _procedureExecution = Context.ProcedureExecutions.Find(ProcedureExecution);
                    if (_procedureExecution != null)
                    {
                        Context.Attach(_procedureExecution);
                    }
                }
                return _procedureExecution;
            }
            set
            {
                if (_procedureExecution != value)
                {
                    _procedureExecution = value;
                    ProcedureExecution = _procedureExecution == null ? default : _procedureExecution.ProcedureExecutionId;
                }
            }
        }

        private Step _step;

        [ForeignKey("Step")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(Step))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _step = Context.Steps.Find(Step);
                    if (_step != null)
                    {
                        Context.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    Step = _step == null ? default : _step.StepId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ExecutedByAgent: " + ExecutedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(ExecutedByAgent);
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
                    ExecutedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private ObservableCollection<RequirementSatisfaction> _requirementSatisfactions;

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<RequirementSatisfaction> RequirementSatisfactions
        {
            get
            {
                if (_requirementSatisfactions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequirementSatisfactions - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>();
                    }
                    else
                    {
                        var items = Context.RequirementSatisfactions.Where(x => x.StepExecution == this.StepExecutionId).ToList<RequirementSatisfaction>();
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<IssueOccurrence> IssueOccurrences
        {
            get
            {
                if (_issueOccurrences == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IssueOccurrences - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>();
                    }
                    else
                    {
                        var items = Context.IssueOccurrences.Where(x => x.StepExecution == this.StepExecutionId).ToList<IssueOccurrence>();
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<UserQuestion> UserQuestions
        {
            get
            {
                if (_userQuestions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserQuestions - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _userQuestions = new ObservableCollection<UserQuestion>();
                    }
                    else
                    {
                        var items = Context.UserQuestions.Where(x => x.StepExecution == this.StepExecutionId).ToList<UserQuestion>();
                        _userQuestions = new ObservableCollection<UserQuestion>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<ExceptionInvocation> ExceptionInvocations
        {
            get
            {
                if (_exceptionInvocations == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExceptionInvocations - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _exceptionInvocations = new ObservableCollection<ExceptionInvocation>();
                    }
                    else
                    {
                        var items = Context.ExceptionInvocations.Where(x => x.StepExecution == this.StepExecutionId).ToList<ExceptionInvocation>();
                        _exceptionInvocations = new ObservableCollection<ExceptionInvocation>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<VerificationOutcome> VerificationOutcomes
        {
            get
            {
                if (_verificationOutcomes == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VerificationOutcomes - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>();
                    }
                    else
                    {
                        var items = Context.VerificationOutcomes.Where(x => x.StepExecution == this.StepExecutionId).ToList<VerificationOutcome>();
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ObservedTransitions - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _observedTransitions = new ObservableCollection<ObservedTransition>();
                    }
                    else
                    {
                        var items = Context.ObservedTransitions.Where(x => x.ArrivingStepExecution == this.StepExecutionId).ToList<ObservedTransition>();
                        _observedTransitions = new ObservableCollection<ObservedTransition>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<MessageDelivery> MessageDeliveries
        {
            get
            {
                if (_messageDeliveries == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = Context.MessageDeliveries.Where(x => x.StepExecution == this.StepExecutionId).ToList<MessageDelivery>();
                        _messageDeliveries = new ObservableCollection<MessageDelivery>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<SendIntent> SendIntents
        {
            get
            {
                if (_sendIntents == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = Context.SendIntents.Where(x => x.StepExecution == this.StepExecutionId).ToList<SendIntent>();
                        _sendIntents = new ObservableCollection<SendIntent>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<AgentDecisionRecord> AgentDecisionRecords
        {
            get
            {
                if (_agentDecisionRecords == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentDecisionRecords - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _agentDecisionRecords = new ObservableCollection<AgentDecisionRecord>();
                    }
                    else
                    {
                        var items = Context.AgentDecisionRecords.Where(x => x.StepExecution == this.StepExecutionId).ToList<AgentDecisionRecord>();
                        _agentDecisionRecords = new ObservableCollection<AgentDecisionRecord>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        private ObservableCollection<DeliveredCommunication> _deliveredCommunications;

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<DeliveredCommunication> DeliveredCommunications
        {
            get
            {
                if (_deliveredCommunications == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DeliveredCommunications - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _deliveredCommunications = new ObservableCollection<DeliveredCommunication>();
                    }
                    else
                    {
                        var items = Context.DeliveredCommunications.Where(x => x.SendingStepExecution == this.StepExecutionId).ToList<DeliveredCommunication>();
                        _deliveredCommunications = new ObservableCollection<DeliveredCommunication>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    item.SendingStepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<DeliveredCommunication> _deliveredCommunications;

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<DeliveredCommunication> DeliveredCommunications
        {
            get
            {
                if (_deliveredCommunications == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DeliveredCommunications - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _deliveredCommunications = new ObservableCollection<DeliveredCommunication>();
                    }
                    else
                    {
                        var items = Context.DeliveredCommunications.Where(x => x.AuthorizingStepExecution == this.StepExecutionId).ToList<DeliveredCommunication>();
                        _deliveredCommunications = new ObservableCollection<DeliveredCommunication>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    item.AuthorizingStepExecution = this.StepExecutionId;
                }
            }
        }

        private ObservableCollection<BindingObservation> _bindingObservations;

        [InverseProperty("StepExecution")]
        public virtual ObservableCollection<BindingObservation> BindingObservations
        {
            get
            {
                if (_bindingObservations == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access BindingObservations - no database context is set. StepExecutionId: " + this.StepExecutionId + ".");
                        }
                        _bindingObservations = new ObservableCollection<BindingObservation>();
                    }
                    else
                    {
                        var items = Context.BindingObservations.Where(x => x.StepExecution == this.StepExecutionId).ToList<BindingObservation>();
                        _bindingObservations = new ObservableCollection<BindingObservation>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.ProcedureExecution;
            _ = this.Step;
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
            _ = this.DeliveredCommunications;
            _ = this.DeliveredCommunications;
            _ = this.BindingObservations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
