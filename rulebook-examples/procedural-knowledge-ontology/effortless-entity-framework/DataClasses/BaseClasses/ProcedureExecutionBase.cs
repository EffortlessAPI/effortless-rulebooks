
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ProcedureExecutions")]
    public class ProcedureExecutionBase : SoAEntityBase
    {
        [Key]
        public string ProcedureExecutionId { get; set; }

        // Formula Name (rulebook: ={{ProcedureVersion}} & " / " & {{Context}})
        public string? Name
        {
            get => this.ProcedureVersion + " / " + this.Context; set { }
        }

        public string? ExecutionStatus { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? Context { get; set; }
        public string? OperationalRecordUri { get; set; }
        // Formula ExpectedStepCount (rulebook: =INDEX(ProcedureVersions!{{SpecifiedStepCount}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        public decimal? ExpectedStepCount
        {
            get => INDEX(ProcedureVersions!this.SpecifiedStepCount, MATCH(this.ProcedureVersion, ProcedureVersions!this.ProcedureVersionId, 0)); set { }
        }

        // Formula CompletedStepCount (rulebook: =COUNTIFS(StepExecutions!{{CompletedExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? CompletedStepCount
        {
            get => COUNTIFS(StepExecutions!this.CompletedExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula ControlBreachCount (rulebook: =COUNTIFS(StepExecutions!{{ControlBreachExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? ControlBreachCount
        {
            get => COUNTIFS(StepExecutions!this.ControlBreachExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula LateStepCount (rulebook: =COUNTIFS(StepExecutions!{{LateExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? LateStepCount
        {
            get => COUNTIFS(StepExecutions!this.LateExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula IsStructurallyComplete (rulebook: ={{CompletedStepCount}} >= {{ExpectedStepCount}})
        public bool? IsStructurallyComplete
        {
            get => this.CompletedStepCount >= this.ExpectedStepCount; set { }
        }

        // Formula DivergedFromSpecification (rulebook: =OR(NOT({{IsStructurallyComplete}}), {{ControlBreachCount}} > 0))
        public bool? DivergedFromSpecification
        {
            get => OR(NOT(this.IsStructurallyComplete), this.ControlBreachCount > 0); set { }
        }

        // Formula AllBlockingControlsEvaluated (rulebook: ={{UnevaluatedBlockingTotal}} = 0)
        public bool? AllBlockingControlsEvaluated
        {
            get => this.UnevaluatedBlockingTotal = 0; set { }
        }

        // Formula UnevaluatedBlockingTotal (rulebook: =COUNTIFS(StepExecutions!{{UnevaluatedBlockingExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? UnevaluatedBlockingTotal
        {
            get => COUNTIFS(StepExecutions!this.UnevaluatedBlockingExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula SeparationOfDutiesHeld (rulebook: ={{SeparationViolationCount}} = 0)
        public bool? SeparationOfDutiesHeld
        {
            get => this.SeparationViolationCount = 0; set { }
        }

        // Formula SeparationViolationCount (rulebook: =COUNTIFS(StepExecutions!{{SeparationViolationExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? SeparationViolationCount
        {
            get => COUNTIFS(StepExecutions!this.SeparationViolationExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula IsAttestationReady (rulebook: =AND({{IsStructurallyComplete}}, NOT({{DivergedFromSpecification}}), {{AllBlockingControlsEvaluated}}, {{SeparationOfDutiesHeld}}))
        public bool? IsAttestationReady
        {
            get => AND(this.IsStructurallyComplete, NOT(this.DivergedFromSpecification), this.AllBlockingControlsEvaluated, this.SeparationOfDutiesHeld); set { }
        }

        // Formula AttestationBlockerSummary (rulebook: =IF({{IsAttestationReady}}, "", IF(NOT({{IsStructurallyComplete}}), "Incomplete: specified steps did not all complete.", IF({{SeparationViolationCount}} > 0, "Segregation of duties violated.", IF({{UnevaluatedBlockingTotal}} > 0, "Blocking controls were never evaluated.", "Control breach recorded on one or more steps.")))))
        public string? AttestationBlockerSummary
        {
            get => IF(this.IsAttestationReady, "", IF(NOT(this.IsStructurallyComplete), "Incomplete: specified steps did not all complete.", IF(this.SeparationViolationCount > 0, "Segregation of duties violated.", IF(this.UnevaluatedBlockingTotal > 0, "Blocking controls were never evaluated.", "Control breach recorded on one or more steps.")))); set { }
        }

        // Formula ExecutedVersionIsFit (rulebook: =INDEX(ProcedureVersions!{{IsFitToExecute}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        public bool? ExecutedVersionIsFit
        {
            get => INDEX(ProcedureVersions!this.IsFitToExecute, MATCH(this.ProcedureVersion, ProcedureVersions!this.ProcedureVersionId, 0)); set { }
        }

        // Formula SignedAgainstUnfitVersion (rulebook: =AND({{ExecutionStatus}} = "Completed", NOT({{ExecutedVersionIsFit}})))
        public bool? SignedAgainstUnfitVersion
        {
            get => AND(this.ExecutionStatus = "Completed", NOT(this.ExecutedVersionIsFit)); set { }
        }

        // Formula AssertedOnlyControlCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{AssertedOnlyExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? AssertedOnlyControlCount
        {
            get => COUNTIFS(RequirementSatisfactions!this.AssertedOnlyExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula AssuranceIsMostlyAsserted (rulebook: ={{AssertedOnlyControlCount}} > 0)
        public bool? AssuranceIsMostlyAsserted
        {
            get => this.AssertedOnlyControlCount > 0; set { }
        }

        // Formula UnreachableHandlingFailureCount (rulebook: =COUNTIFS(MessageDeliveries!{{UnreachableFailureKey}}, {{ProcedureExecutionId}}))
        public decimal? UnreachableHandlingFailureCount
        {
            get => COUNTIFS(MessageDeliveries!this.UnreachableFailureKey, this.ProcedureExecutionId); set { }
        }

        // Formula RetentionBreachCount (rulebook: =COUNTIFS(MessageDeliveries!{{RetentionBreachExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? RetentionBreachCount
        {
            get => COUNTIFS(MessageDeliveries!this.RetentionBreachExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula ClearedLegalReviewCount (rulebook: =COUNTIFS(StepExecutions!{{ClearedLegalReviewKey}}, {{ProcedureExecutionId}}))
        public decimal? ClearedLegalReviewCount
        {
            get => COUNTIFS(StepExecutions!this.ClearedLegalReviewKey, this.ProcedureExecutionId); set { }
        }

        // Formula HasClearedLegalReview (rulebook: ={{ClearedLegalReviewCount}} > 0)
        public bool? HasClearedLegalReview
        {
            get => this.ClearedLegalReviewCount > 0; set { }
        }

        // Formula AbandonedFailureCount (rulebook: =COUNTIFS(MessageDeliveries!{{AbandonedFailureExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? AbandonedFailureCount
        {
            get => COUNTIFS(MessageDeliveries!this.AbandonedFailureExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula DeliveredCount (rulebook: =COUNTIFS(MessageDeliveries!{{ReachedExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? DeliveredCount
        {
            get => COUNTIFS(MessageDeliveries!this.ReachedExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula TotalDeliveryAttemptCount (rulebook: =COUNTIFS(MessageDeliveries!{{ProcedureExecution}}, {{ProcedureExecutionId}}))
        public decimal? TotalDeliveryAttemptCount
        {
            get => COUNTIFS(MessageDeliveries!this.ProcedureExecution, this.ProcedureExecutionId); set { }
        }

        // Formula HasAbandonedFailures (rulebook: ={{AbandonedFailureCount}} > 0)
        public bool? HasAbandonedFailures
        {
            get => this.AbandonedFailureCount > 0; set { }
        }

        // Formula MishandledRefusalCount (rulebook: =COUNTIFS(SendIntents!{{RefusalFailureExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? MishandledRefusalCount
        {
            get => COUNTIFS(SendIntents!this.RefusalFailureExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula UncleanStepCount (rulebook: =COUNTIFS(StepExecutions!{{ProcedureExecutionWhenUnclean}}, {{ProcedureExecutionId}}))
        public decimal? UncleanStepCount
        {
            get => COUNTIFS(StepExecutions!this.ProcedureExecutionWhenUnclean, this.ProcedureExecutionId); set { }
        }

        // Formula RanClean (rulebook: ={{UncleanStepCount}} = 0)
        public bool? RanClean
        {
            get => this.UncleanStepCount = 0; set { }
        }

        // Formula CountOfApprovalExecutions (rulebook: =COUNTIFS(StepExecutions!{{IsApprovalExecution}}, TRUE))
        public int? CountOfApprovalExecutions
        {
            get => COUNTIFS(StepExecutions!this.IsApprovalExecution, TRUE); set { }
        }

        // Formula HasHumanApproval (rulebook: ={{CountOfApprovalExecutions}} > 0)
        public bool? HasHumanApproval
        {
            get => this.CountOfApprovalExecutions > 0; set { }
        }

        // Formula CountOfDeliveryExecutions (rulebook: =COUNTIFS(StepExecutions!{{Step}}, "policy-07"))
        public int? CountOfDeliveryExecutions
        {
            get => COUNTIFS(StepExecutions!this.Step, "policy-07"); set { }
        }

        // Formula HasDelivered (rulebook: ={{CountOfDeliveryExecutions}} > 0)
        public bool? HasDelivered
        {
            get => this.CountOfDeliveryExecutions > 0; set { }
        }

        // Formula DeliveredWithoutApproval (rulebook: =AND({{HasDelivered}}, NOT({{HasHumanApproval}})))
        public bool? DeliveredWithoutApproval
        {
            get => AND(this.HasDelivered, NOT(this.HasHumanApproval)); set { }
        }

        // Formula InvalidApprovalCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{RunWhenInvalidApproval}}, {{ProcedureExecutionId}}))
        public decimal? InvalidApprovalCount
        {
            get => COUNTIFS(RequirementSatisfactions!this.RunWhenInvalidApproval, this.ProcedureExecutionId); set { }
        }

        // Formula ApprovalChainIsComplete (rulebook: ={{InvalidApprovalCount}} = 0)
        public bool? ApprovalChainIsComplete
        {
            get => this.InvalidApprovalCount = 0; set { }
        }

        // Formula VacuouslyCleanStepCount (rulebook: =COUNTIFS(StepExecutions!{{VacuouslyCleanExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? VacuouslyCleanStepCount
        {
            get => COUNTIFS(StepExecutions!this.VacuouslyCleanExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula PreparationStepCount (rulebook: =COUNTIFS(StepExecutions!{{PreparationExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? PreparationStepCount
        {
            get => COUNTIFS(StepExecutions!this.PreparationExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula ApprovalStepCount (rulebook: =COUNTIFS(StepExecutions!{{ApprovalExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? ApprovalStepCount
        {
            get => COUNTIFS(StepExecutions!this.ApprovalExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula SeparationWasTestable (rulebook: =AND({{PreparationStepCount}} > 0, {{ApprovalStepCount}} > 0))
        public bool? SeparationWasTestable
        {
            get => AND(this.PreparationStepCount > 0, this.ApprovalStepCount > 0); set { }
        }

        // Formula SeparationHeldUnderTest (rulebook: =AND({{SeparationWasTestable}}, {{SeparationOfDutiesHeld}}))
        public bool? SeparationHeldUnderTest
        {
            get => AND(this.SeparationWasTestable, this.SeparationOfDutiesHeld); set { }
        }

        // Formula SeparationIsVacuouslyGreen (rulebook: =AND({{SeparationOfDutiesHeld}}, NOT({{SeparationWasTestable}})))
        public bool? SeparationIsVacuouslyGreen
        {
            get => AND(this.SeparationOfDutiesHeld, NOT(this.SeparationWasTestable)); set { }
        }

        // Formula SeparationAssuranceNote (rulebook: =IF({{SeparationViolationCount}} > 0, "Violated: same agent prepared and approved.", IF({{SeparationIsVacuouslyGreen}}, "Not tested: this run had no preparation/approval pair.", "Held under test.")))
        public string? SeparationAssuranceNote
        {
            get => IF(this.SeparationViolationCount > 0, "Violated: same agent prepared and approved.", IF(this.SeparationIsVacuouslyGreen, "Not tested: this run had no preparation/approval pair.", "Held under test.")); set { }
        }

        // Formula UngovernedDivergenceCount (rulebook: =COUNTIFS(StepExecutions!{{UngovernedDivergenceExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? UngovernedDivergenceCount
        {
            get => COUNTIFS(StepExecutions!this.UngovernedDivergenceExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula DivergenceWasFullyGoverned (rulebook: =AND({{DivergedFromSpecification}}, {{UngovernedDivergenceCount}} = 0))
        public bool? DivergenceWasFullyGoverned
        {
            get => AND(this.DivergedFromSpecification, this.UngovernedDivergenceCount = 0); set { }
        }

        // Formula ComputedlyWitnessedControlCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{ComputedWitnessExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? ComputedlyWitnessedControlCount
        {
            get => COUNTIFS(RequirementSatisfactions!this.ComputedWitnessExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula EvaluatedControlCount (rulebook: ={{ComputedlyWitnessedControlCount}} + {{AssertedOnlyControlCount}})
        public decimal? EvaluatedControlCount
        {
            get => this.ComputedlyWitnessedControlCount + this.AssertedOnlyControlCount; set { }
        }

        // Formula ComputedAssuranceRatio (rulebook: =IF({{EvaluatedControlCount}} = 0, 0, {{ComputedlyWitnessedControlCount}} / {{EvaluatedControlCount}}))
        public decimal? ComputedAssuranceRatio
        {
            get => IF(this.EvaluatedControlCount = 0, 0, this.ComputedlyWitnessedControlCount / this.EvaluatedControlCount); set { }
        }

        // Formula InterestedPartyAssertionCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{InterestedAssertionExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? InterestedPartyAssertionCount
        {
            get => COUNTIFS(RequirementSatisfactions!this.InterestedAssertionExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula AssuranceGrade (rulebook: =IF({{EvaluatedControlCount}} = 0, "None: no blocking control was evaluated.", IF({{InterestedPartyAssertionCount}} > 0, "Weak: at least one control rests on an interested-party assertion.", IF({{ComputedAssuranceRatio}} < 0.5, "Thin: most controls rest on human assertion.", IF({{ComputedAssuranceRatio}} < 1, "Mixed: computed and asserted controls.", "Computed: every evaluated control has a witness.")))))
        public string? AssuranceGrade
        {
            get => IF(this.EvaluatedControlCount = 0, "None: no blocking control was evaluated.", IF(this.InterestedPartyAssertionCount > 0, "Weak: at least one control rests on an interested-party assertion.", IF(this.ComputedAssuranceRatio < 0.5, "Thin: most controls rest on human assertion.", IF(this.ComputedAssuranceRatio < 1, "Mixed: computed and asserted controls.", "Computed: every evaluated control has a witness.")))); set { }
        }

        // Formula AttestationWouldBeWeaklyBased (rulebook: =AND({{IsAttestationReady}}, OR({{InterestedPartyAssertionCount}} > 0, {{ComputedAssuranceRatio}} < 0.5)))
        public bool? AttestationWouldBeWeaklyBased
        {
            get => AND(this.IsAttestationReady, OR(this.InterestedPartyAssertionCount > 0, this.ComputedAssuranceRatio < 0.5)); set { }
        }

        // Formula IndependentHumanObservationCount (rulebook: =COUNTIFS(VerificationOutcomes!{{IndependentObservationExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? IndependentHumanObservationCount
        {
            get => COUNTIFS(VerificationOutcomes!this.IndependentObservationExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula HasAnyIndependentObservation (rulebook: ={{IndependentHumanObservationCount}} > 0)
        public bool? HasAnyIndependentObservation
        {
            get => this.IndependentHumanObservationCount > 0; set { }
        }

        // Formula SelfAttestedApprovalCount (rulebook: =COUNTIFS(StepExecutions!{{SelfAttestedApprovalExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? SelfAttestedApprovalCount
        {
            get => COUNTIFS(StepExecutions!this.SelfAttestedApprovalExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula AssuranceChainIsCircular (rulebook: =AND({{SelfAttestedApprovalCount}} > 0, NOT({{HasAnyIndependentObservation}})))
        public bool? AssuranceChainIsCircular
        {
            get => AND(this.SelfAttestedApprovalCount > 0, NOT(this.HasAnyIndependentObservation)); set { }
        }

        // Formula LatestAttestationInstant (rulebook: =MAXIFS(Attestations!{{SignedAt}}, Attestations!{{ProcedureExecution}}, {{ProcedureExecutionId}}))
        public DateTime? LatestAttestationInstant
        {
            get => MAXIFS(Attestations!this.SignedAt, Attestations!this.ProcedureExecution, this.ProcedureExecutionId); set { }
        }

        // Formula HasBeenAttested (rulebook: ={{AttestationCount}} > 0)
        public bool? HasBeenAttested
        {
            get => this.AttestationCount > 0; set { }
        }

        // Formula AttestationCount (rulebook: =COUNTIFS(Attestations!{{ProcedureExecution}}, {{ProcedureExecutionId}}))
        public decimal? AttestationCount
        {
            get => COUNTIFS(Attestations!this.ProcedureExecution, this.ProcedureExecutionId); set { }
        }

        // Formula PostAttestationScoreCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{PostAttestationScoreExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? PostAttestationScoreCount
        {
            get => COUNTIFS(RequirementSatisfactions!this.PostAttestationScoreExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula BasisChangedAfterSignature (rulebook: =AND({{HasBeenAttested}}, {{PostAttestationScoreCount}} > 0))
        public bool? BasisChangedAfterSignature
        {
            get => AND(this.HasBeenAttested, this.PostAttestationScoreCount > 0); set { }
        }

        // Formula RequiresReAttestation (rulebook: =AND({{BasisChangedAfterSignature}}, NOT({{IsAttestationReady}})))
        public bool? RequiresReAttestation
        {
            get => AND(this.BasisChangedAfterSignature, NOT(this.IsAttestationReady)); set { }
        }

        // Formula IntendedRecipientCount (rulebook: =COUNTIFS(SendIntents!{{IntentExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? IntendedRecipientCount
        {
            get => COUNTIFS(SendIntents!this.IntentExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula ReachedRecipientCount (rulebook: =COUNTIFS(SendIntents!{{DeliveredIntentExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? ReachedRecipientCount
        {
            get => COUNTIFS(SendIntents!this.DeliveredIntentExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula SilentlyDroppedCount (rulebook: =COUNTIFS(SendIntents!{{DroppedIntentExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? SilentlyDroppedCount
        {
            get => COUNTIFS(SendIntents!this.DroppedIntentExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula DeliveryYieldPercent (rulebook: =IF({{IntendedRecipientCount}} > 0, {{ReachedRecipientCount}} * 100 / {{IntendedRecipientCount}}, 0))
        public decimal? DeliveryYieldPercent
        {
            get => IF(this.IntendedRecipientCount > 0, this.ReachedRecipientCount * 100 / this.IntendedRecipientCount, 0); set { }
        }

        // Formula CampaignSilentlyLostAudience (rulebook: =({{SilentlyDroppedCount}} > 0))
        public bool? CampaignSilentlyLostAudience
        {
            get => (this.SilentlyDroppedCount > 0); set { }
        }

        // Formula UnrecordedRefusalCount (rulebook: =COUNTIFS(SendIntents!{{UnrecordedRefusalExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? UnrecordedRefusalCount
        {
            get => COUNTIFS(SendIntents!this.UnrecordedRefusalExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula HasUnrecordedRefusals (rulebook: =({{UnrecordedRefusalCount}} > 0))
        public bool? HasUnrecordedRefusals
        {
            get => (this.UnrecordedRefusalCount > 0); set { }
        }

        // Formula IndependentlyConfirmedIntentCount (rulebook: =COUNTIFS(SendIntents!{{IndependentlyConfirmedExecutionKey}}, {{ProcedureExecutionId}}))
        public decimal? IndependentlyConfirmedIntentCount
        {
            get => COUNTIFS(SendIntents!this.IndependentlyConfirmedExecutionKey, this.ProcedureExecutionId); set { }
        }

        // Formula SendDecisionsAreEntirelySelfWitnessed (rulebook: =AND({{IntendedRecipientCount}} > 0, {{IndependentlyConfirmedIntentCount}} = 0))
        public bool? SendDecisionsAreEntirelySelfWitnessed
        {
            get => AND(this.IntendedRecipientCount > 0, this.IndependentlyConfirmedIntentCount = 0); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? ExecutedByAgent { get; set; }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = Context.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        Context.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    ProcedureVersion = _procedureVersion == null ? default : _procedureVersion.ProcedureVersionId;
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

        private ObservableCollection<StepExecution> _stepExecutions;

        [InverseProperty("ProcedureExecution")]
        public virtual ObservableCollection<StepExecution> StepExecutions
        {
            get
            {
                if (_stepExecutions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecutions - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _stepExecutions = new ObservableCollection<StepExecution>();
                    }
                    else
                    {
                        var items = Context.StepExecutions.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<StepExecution>();
                        _stepExecutions = new ObservableCollection<StepExecution>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureExecution")]
        public virtual ObservableCollection<UserFeedback> UserFeedback
        {
            get
            {
                if (_userFeedback == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserFeedback - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _userFeedback = new ObservableCollection<UserFeedback>();
                    }
                    else
                    {
                        var items = Context.UserFeedback.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<UserFeedback>();
                        _userFeedback = new ObservableCollection<UserFeedback>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureExecution")]
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
                            throw new InvalidOperationException("Cannot access ObservedTransitions - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _observedTransitions = new ObservableCollection<ObservedTransition>();
                    }
                    else
                    {
                        var items = Context.ObservedTransitions.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<ObservedTransition>();
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
                    item.ProcedureExecution = this.ProcedureExecutionId;
                }
            }
        }

        private ObservableCollection<MessageDelivery> _messageDeliveries;

        [InverseProperty("ProcedureExecution")]
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
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = Context.MessageDeliveries.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<MessageDelivery>();
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
                    item.ProcedureExecution = this.ProcedureExecutionId;
                }
            }
        }

        private ObservableCollection<SendIntent> _sendIntents;

        [InverseProperty("ProcedureExecution")]
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
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = Context.SendIntents.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<SendIntent>();
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
                    item.ProcedureExecution = this.ProcedureExecutionId;
                }
            }
        }

        private ObservableCollection<DeliveredCommunication> _deliveredCommunications;

        [InverseProperty("ProcedureExecution")]
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
                            throw new InvalidOperationException("Cannot access DeliveredCommunications - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _deliveredCommunications = new ObservableCollection<DeliveredCommunication>();
                    }
                    else
                    {
                        var items = Context.DeliveredCommunications.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<DeliveredCommunication>();
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
                    item.ProcedureExecution = this.ProcedureExecutionId;
                }
            }
        }

        private ObservableCollection<Attestation> _attestations;

        [InverseProperty("ProcedureExecution")]
        public virtual ObservableCollection<Attestation> Attestations
        {
            get
            {
                if (_attestations == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Attestations - no database context is set. ProcedureExecutionId: " + this.ProcedureExecutionId + ".");
                        }
                        _attestations = new ObservableCollection<Attestation>();
                    }
                    else
                    {
                        var items = Context.Attestations.Where(x => x.ProcedureExecution == this.ProcedureExecutionId).ToList<Attestation>();
                        _attestations = new ObservableCollection<Attestation>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.ProcedureVersion;
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
