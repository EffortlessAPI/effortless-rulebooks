
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ProcedureVersions")]
    public class ProcedureVersionBase : SoAEntityBase
    {
        [Key]
        public string ProcedureVersionId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        public string? Name
        {
            get => this.Title; set { }
        }

        public string? VersionNumber { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? NewVersionMotivation { get; set; }
        public string? ChangelogDescription { get; set; }
        public bool? IsCurrent { get; set; }
        // Formula CountOfSteps (rulebook: =COUNTIFS(Steps!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}))
        public int? CountOfSteps
        {
            get => this.Steps == null ? 0 : this.Steps.Count; set { }
        }

        // Formula CountOfOpenKnowledgeGaps (rulebook: =COUNTIFS(KnowledgeGaps!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, KnowledgeGaps!{{Status}}, "Open"))
        public int? CountOfOpenKnowledgeGaps
        {
            get => COUNTIFS(KnowledgeGaps!this.ProcedureVersion, ProcedureVersions!this.ProcedureVersionId, KnowledgeGaps!this.Status, "Open"); set { }
        }

        // Formula IsReadyForExecution (rulebook: =AND({{Status}} = "Approved", {{CountOfSteps}} > 0, {{CountOfOpenKnowledgeGaps}} = 0))
        public bool? IsReadyForExecution
        {
            get => AND(this.Status = "Approved", this.CountOfSteps > 0, this.CountOfOpenKnowledgeGaps = 0); set { }
        }

        // Formula SpecifiedStepCount (rulebook: =COUNTIFS(Steps!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        public decimal? SpecifiedStepCount
        {
            get => COUNTIFS(Steps!this.ProcedureVersion, this.ProcedureVersionId); set { }
        }

        // Formula OverdueReviewCount (rulebook: =COUNTIFS(ReviewEvents!{{OverdueVersionKey}}, {{ProcedureVersionId}}))
        public decimal? OverdueReviewCount
        {
            get => COUNTIFS(ReviewEvents!this.OverdueVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula OpenChangeRequestCount (rulebook: =COUNTIFS(ChangeRequests!{{OpenChangeVersionKey}}, {{ProcedureVersionId}}))
        public decimal? OpenChangeRequestCount
        {
            get => COUNTIFS(ChangeRequests!this.OpenChangeVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula OpenHighSeverityGapCount (rulebook: =COUNTIFS(KnowledgeGaps!{{OpenGapVersionKey}}, {{ProcedureVersionId}}))
        public decimal? OpenHighSeverityGapCount
        {
            get => COUNTIFS(KnowledgeGaps!this.OpenGapVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula IsFitToExecute (rulebook: =AND({{Status}} = "Approved", {{OverdueReviewCount}} = 0, {{OpenChangeRequestCount}} = 0, {{OpenHighSeverityGapCount}} = 0))
        public bool? IsFitToExecute
        {
            get => AND(this.Status = "Approved", this.OverdueReviewCount = 0, this.OpenChangeRequestCount = 0, this.OpenHighSeverityGapCount = 0); set { }
        }

        // Formula StewardReviewCadenceDays (rulebook: =SUMIFS(StewardshipAssignments!{{ReviewCadenceDays}}, StewardshipAssignments!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        public decimal? StewardReviewCadenceDays
        {
            get => SUMIFS(StewardshipAssignments!this.ReviewCadenceDays, StewardshipAssignments!this.ProcedureVersion, this.ProcedureVersionId); set { }
        }

        // Formula CountOfStewardshipAssignments (rulebook: =COUNTIFS(StewardshipAssignments!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}))
        public int? CountOfStewardshipAssignments
        {
            get => this.StewardshipAssignments == null ? 0 : this.StewardshipAssignments.Count; set { }
        }

        // Formula HasAnySteward (rulebook: ={{CountOfStewardshipAssignments}} > 0)
        public bool? HasAnySteward
        {
            get => this.CountOfStewardshipAssignments > 0; set { }
        }

        // Formula IsLive (rulebook: =OR({{Status}} = "Approved", {{Status}} = "Published"))
        public bool? IsLive
        {
            get => OR(this.Status = "Approved", this.Status = "Published"); set { }
        }

        // Formula IsUnstewarded (rulebook: =NOT({{HasAnySteward}}))
        public bool? IsUnstewarded
        {
            get => NOT(this.HasAnySteward); set { }
        }

        // Formula IsLiveAndUnstewarded (rulebook: =AND({{IsLive}}, {{IsUnstewarded}}))
        public bool? IsLiveAndUnstewarded
        {
            get => AND(this.IsLive, this.IsUnstewarded); set { }
        }

        // Formula CountOfOpenBlockingGaps (rulebook: =COUNTIFS(KnowledgeGaps!{{IsOpenAndBlocking}}, TRUE))
        public int? CountOfOpenBlockingGaps
        {
            get => COUNTIFS(KnowledgeGaps!this.IsOpenAndBlocking, TRUE); set { }
        }

        // Formula HasOpenBlockingGap (rulebook: ={{CountOfOpenBlockingGaps}} > 0)
        public bool? HasOpenBlockingGap
        {
            get => this.CountOfOpenBlockingGaps > 0; set { }
        }

        // Formula IsLiveWithBlockingGap (rulebook: =AND({{IsLive}}, {{HasOpenBlockingGap}}))
        public bool? IsLiveWithBlockingGap
        {
            get => AND(this.IsLive, this.HasOpenBlockingGap); set { }
        }

        // Formula ShouldNotBeExecutable (rulebook: =AND({{IsReadyForExecution}}, {{HasOpenBlockingGap}}))
        public bool? ShouldNotBeExecutable
        {
            get => AND(this.IsReadyForExecution, this.HasOpenBlockingGap); set { }
        }

        // Formula CountOfUnapprovedRelianceFragments (rulebook: =COUNTIFS(KnowledgeFragments!{{IsUnapprovedButReliedOn}}, TRUE))
        public int? CountOfUnapprovedRelianceFragments
        {
            get => COUNTIFS(KnowledgeFragments!this.IsUnapprovedButReliedOn, TRUE); set { }
        }

        // Formula RunsOnUnapprovedKnowledge (rulebook: ={{CountOfUnapprovedRelianceFragments}} > 0)
        public bool? RunsOnUnapprovedKnowledge
        {
            get => this.CountOfUnapprovedRelianceFragments > 0; set { }
        }

        // Formula CountOfOverdueGaps (rulebook: =COUNTIFS(KnowledgeGaps!{{IsOverdueGap}}, TRUE))
        public int? CountOfOverdueGaps
        {
            get => COUNTIFS(KnowledgeGaps!this.IsOverdueGap, TRUE); set { }
        }

        // Formula CountOfChangeRequests (rulebook: =COUNTIFS(ChangeRequests!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}))
        public int? CountOfChangeRequests
        {
            get => this.ChangeRequests == null ? 0 : this.ChangeRequests.Count; set { }
        }

        // Formula CountOfReviewEvents (rulebook: =COUNTIFS(ReviewEvents!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}))
        public int? CountOfReviewEvents
        {
            get => this.ReviewEvents == null ? 0 : this.ReviewEvents.Count; set { }
        }

        // Formula HasGovernanceRecord (rulebook: =OR({{CountOfChangeRequests}} > 0, {{CountOfReviewEvents}} > 0))
        public bool? HasGovernanceRecord
        {
            get => OR(this.CountOfChangeRequests > 0, this.CountOfReviewEvents > 0); set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula DaysSinceModified (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{ModifiedAt}}, "days"))
        public int? DaysSinceModified
        {
            get => DATETIME_DIFF(this.AsOfInstant, this.ModifiedAt, "days"); set { }
        }

        // Formula DaysSinceLastReview (rulebook: =DATETIME_DIFF({{AsOfInstant}}, MAXIFS(ReviewEvents!{{ReviewedAt}}, ReviewEvents!{{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}), "days"))
        public int? DaysSinceLastReview
        {
            get => DATETIME_DIFF(this.AsOfInstant, MAXIFS(ReviewEvents!this.ReviewedAt, ReviewEvents!this.ProcedureVersion, ProcedureVersions!this.ProcedureVersionId), "days"); set { }
        }

        // Formula WasModifiedSinceLastReview (rulebook: ={{DaysSinceModified}} < {{DaysSinceLastReview}})
        public bool? WasModifiedSinceLastReview
        {
            get => this.DaysSinceModified < this.DaysSinceLastReview; set { }
        }

        // Formula ModifierIsAuthority (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{ModifiedByAgent}}, Agents!{{AgentId}}, 0)))
        public string? ModifierIsAuthority
        {
            get => INDEX(Agents!this.AgentKind, MATCH(this.ModifiedByAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula HasUnwitnessedChange (rulebook: =AND({{IsLive}}, {{WasModifiedSinceLastReview}}))
        public bool? HasUnwitnessedChange
        {
            get => AND(this.IsLive, this.WasModifiedSinceLastReview); set { }
        }

        // Formula CountOfStaleFragments (rulebook: =COUNTIFS(KnowledgeFragments!{{ExceedsOwningCadence}}, TRUE))
        public int? CountOfStaleFragments
        {
            get => COUNTIFS(KnowledgeFragments!this.ExceedsOwningCadence, TRUE); set { }
        }

        // Formula KnowledgeIsStalerThanCadence (rulebook: ={{CountOfStaleFragments}} > 0)
        public bool? KnowledgeIsStalerThanCadence
        {
            get => this.CountOfStaleFragments > 0; set { }
        }

        // Formula CompoundFragileFragmentCount (rulebook: =COUNTIFS(KnowledgeFragments!{{CompoundFragileVersionKey}}, {{ProcedureVersionId}}))
        public decimal? CompoundFragileFragmentCount
        {
            get => COUNTIFS(KnowledgeFragments!this.CompoundFragileVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula RestsOnCompoundFragileKnowledge (rulebook: =AND({{IsLive}}, {{CompoundFragileFragmentCount}} > 0))
        public bool? RestsOnCompoundFragileKnowledge
        {
            get => AND(this.IsLive, this.CompoundFragileFragmentCount > 0); set { }
        }

        // Formula ConcentratedWitnessSessionCount (rulebook: =COUNTIFS(ElicitationSessions!{{ConcentratedSessionVersionKey}}, {{ProcedureVersionId}}))
        public decimal? ConcentratedWitnessSessionCount
        {
            get => COUNTIFS(ElicitationSessions!this.ConcentratedSessionVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula KnowledgeBaseIsConcentrated (rulebook: =AND({{IsLive}}, {{ConcentratedWitnessSessionCount}} > 0))
        public bool? KnowledgeBaseIsConcentrated
        {
            get => AND(this.IsLive, this.ConcentratedWitnessSessionCount > 0); set { }
        }

        // Formula MachineConsumedUnapprovedCount (rulebook: =COUNTIFS(KnowledgeFragments!{{MachineConsumedUnapprovedVersionKey}}, {{ProcedureVersionId}}))
        public decimal? MachineConsumedUnapprovedCount
        {
            get => COUNTIFS(KnowledgeFragments!this.MachineConsumedUnapprovedVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula FeedsUnapprovedKnowledgeToMachines (rulebook: =AND({{IsLive}}, {{MachineConsumedUnapprovedCount}} > 0))
        public bool? FeedsUnapprovedKnowledgeToMachines
        {
            get => AND(this.IsLive, this.MachineConsumedUnapprovedCount > 0); set { }
        }

        // Formula GenuinelyOverdueFragmentCount (rulebook: =COUNTIFS(KnowledgeFragments!{{GenuinelyOverdueVersionKey}}, {{ProcedureVersionId}}))
        public decimal? GenuinelyOverdueFragmentCount
        {
            get => COUNTIFS(KnowledgeFragments!this.GenuinelyOverdueVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula AwaitedDecisionCount (rulebook: =COUNTIFS(ChangeRequests!{{BacklogVersionKey}}, {{ProcedureVersionId}}))
        public decimal? AwaitedDecisionCount
        {
            get => COUNTIFS(ChangeRequests!this.BacklogVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula ScopedOpenBlockingGapCount (rulebook: =COUNTIFS(KnowledgeGaps!{{OpenBlockingGapVersionKey}}, {{ProcedureVersionId}}))
        public decimal? ScopedOpenBlockingGapCount
        {
            get => COUNTIFS(KnowledgeGaps!this.OpenBlockingGapVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula IsBlockedOnPendingDecision (rulebook: =AND({{AwaitedDecisionCount}} > 0, {{ScopedOpenBlockingGapCount}} > 0))
        public bool? IsBlockedOnPendingDecision
        {
            get => AND(this.AwaitedDecisionCount > 0, this.ScopedOpenBlockingGapCount > 0); set { }
        }

        // Formula UnexercisedHumanGateCount (rulebook: =COUNTIFS(Steps!{{UnexercisedGateVersionKey}}, {{ProcedureVersionId}}))
        public decimal? UnexercisedHumanGateCount
        {
            get => COUNTIFS(Steps!this.UnexercisedGateVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula AiBoundaryIsUnevidenced (rulebook: =AND({{IsLive}}, {{UnexercisedHumanGateCount}} > 0))
        public bool? AiBoundaryIsUnevidenced
        {
            get => AND(this.IsLive, this.UnexercisedHumanGateCount > 0); set { }
        }

        // Formula LoadBearingUnapprovedCount (rulebook: =COUNTIFS(KnowledgeFragments!{{UnapprovedLoadBearingVersionKey}}, {{ProcedureVersionId}}))
        public decimal? LoadBearingUnapprovedCount
        {
            get => COUNTIFS(KnowledgeFragments!this.UnapprovedLoadBearingVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula UnlandedDecisionCount (rulebook: =COUNTIFS(ChangeRequests!{{UnlandedVersionKey}}, {{ProcedureVersionId}}))
        public decimal? UnlandedDecisionCount
        {
            get => COUNTIFS(ChangeRequests!this.UnlandedVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula UnrehearsedControlEntryCount (rulebook: =COUNTIFS(StepTransitions!{{UnrehearsedControlVersionKey}}, {{ProcedureVersionId}}))
        public decimal? UnrehearsedControlEntryCount
        {
            get => COUNTIFS(StepTransitions!this.UnrehearsedControlVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula HasUnrehearsedControlEntry (rulebook: ={{UnrehearsedControlEntryCount}} > 0)
        public bool? HasUnrehearsedControlEntry
        {
            get => this.UnrehearsedControlEntryCount > 0; set { }
        }

        // Formula IsLiveWithUnrehearsedControl (rulebook: =AND({{IsLive}}, {{HasUnrehearsedControlEntry}}))
        public bool? IsLiveWithUnrehearsedControl
        {
            get => AND(this.IsLive, this.HasUnrehearsedControlEntry); set { }
        }

        // Formula CadenceBreachCount (rulebook: =COUNTIFS(ReviewEvents!{{CadenceBreachVersionKey}}, {{ProcedureVersionId}}))
        public decimal? CadenceBreachCount
        {
            get => COUNTIFS(ReviewEvents!this.CadenceBreachVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula IsInCadenceBreach (rulebook: ={{CadenceBreachCount}} > 0)
        public bool? IsInCadenceBreach
        {
            get => this.CadenceBreachCount > 0; set { }
        }

        // Formula HasDecisionInFlight (rulebook: ={{OpenChangeRequestCount}} > 0)
        public bool? HasDecisionInFlight
        {
            get => this.OpenChangeRequestCount > 0; set { }
        }

        // Formula IsUnremediatedCadenceBreach (rulebook: =AND({{IsInCadenceBreach}}, NOT({{HasDecisionInFlight}})))
        public bool? IsUnremediatedCadenceBreach
        {
            get => AND(this.IsInCadenceBreach, NOT(this.HasDecisionInFlight)); set { }
        }

        // Formula IsManagedCadenceBreach (rulebook: =AND({{IsInCadenceBreach}}, {{HasDecisionInFlight}}))
        public bool? IsManagedCadenceBreach
        {
            get => AND(this.IsInCadenceBreach, this.HasDecisionInFlight); set { }
        }

        // Formula GovernanceIsSilent (rulebook: =AND({{IsLive}}, NOT({{HasGovernanceRecord}})))
        public bool? GovernanceIsSilent
        {
            get => AND(this.IsLive, NOT(this.HasGovernanceRecord)); set { }
        }

        // Formula ValidFragmentCount (rulebook: =COUNTIFS(KnowledgeFragments!{{ValidFragmentVersionKey}}, {{ProcedureVersionId}}))
        public decimal? ValidFragmentCount
        {
            get => COUNTIFS(KnowledgeFragments!this.ValidFragmentVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula StillOwnsValidKnowledge (rulebook: ={{ValidFragmentCount}} > 0)
        public bool? StillOwnsValidKnowledge
        {
            get => this.ValidFragmentCount > 0; set { }
        }

        // Formula IncomingSupersessionCount (rulebook: =COUNTIFS(ProcedureVersionLinks!{{SupersededVersionKey}}, {{ProcedureVersionId}}))
        public decimal? IncomingSupersessionCount
        {
            get => COUNTIFS(ProcedureVersionLinks!this.SupersededVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula IsStillReferenced (rulebook: ={{IncomingSupersessionCount}} > 0)
        public bool? IsStillReferenced
        {
            get => this.IncomingSupersessionCount > 0; set { }
        }

        // Formula IsLoadBearingOrphan (rulebook: =AND({{IsUnstewarded}}, OR({{StillOwnsValidKnowledge}}, {{IsStillReferenced}})))
        public bool? IsLoadBearingOrphan
        {
            get => AND(this.IsUnstewarded, OR(this.StillOwnsValidKnowledge, this.IsStillReferenced)); set { }
        }

        // Formula IsCleanlyRetired (rulebook: =AND({{IsUnstewarded}}, NOT({{StillOwnsValidKnowledge}}), NOT({{IsStillReferenced}})))
        public bool? IsCleanlyRetired
        {
            get => AND(this.IsUnstewarded, NOT(this.StillOwnsValidKnowledge), NOT(this.IsStillReferenced)); set { }
        }

        // Formula StalledImplementationCount (rulebook: =COUNTIFS(ChangeRequests!{{StalledImplementationVersionKey}}, {{ProcedureVersionId}}))
        public decimal? StalledImplementationCount
        {
            get => COUNTIFS(ChangeRequests!this.StalledImplementationVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula IsHeldUnfitByLandedDecisions (rulebook: =AND(NOT({{IsFitToExecute}}), {{StalledImplementationCount}} > 0))
        public bool? IsHeldUnfitByLandedDecisions
        {
            get => AND(NOT(this.IsFitToExecute), this.StalledImplementationCount > 0); set { }
        }

        // Formula UndeclaredControlKindCount (rulebook: =COUNTIFS(Steps!{{UndeclaredControlVersionKey}}, {{ProcedureVersionId}}))
        public decimal? UndeclaredControlKindCount
        {
            get => COUNTIFS(Steps!this.UndeclaredControlVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula ControlTaxonomyIsIncomplete (rulebook: ={{UndeclaredControlKindCount}} > 0)
        public bool? ControlTaxonomyIsIncomplete
        {
            get => this.UndeclaredControlKindCount > 0; set { }
        }

        // Formula HasApprovedChangeRequest (rulebook: ={{ApprovedChangeRequestCount}} > 0)
        public bool? HasApprovedChangeRequest
        {
            get => this.ApprovedChangeRequestCount > 0; set { }
        }

        // Formula ApprovedChangeRequestCount (rulebook: =COUNTIFS(ChangeRequests!{{ApprovedVersionKey}}, {{ProcedureVersionId}}))
        public decimal? ApprovedChangeRequestCount
        {
            get => COUNTIFS(ChangeRequests!this.ApprovedVersionKey, this.ProcedureVersionId); set { }
        }

        // Formula UnwatchedUnownedControlCount (rulebook: =COUNTIFS(Requirements!{{UnwatchedUnownedFlag}}, "unwatched-unowned"))
        public decimal? UnwatchedUnownedControlCount
        {
            get => COUNTIFS(Requirements!this.UnwatchedUnownedFlag, "unwatched-unowned"); set { }
        }

        // Formula MiningRunCount (rulebook: =COUNTIFS(ProcessMiningRuns!{{ProcedureVersion}}, {{ProcedureVersionId}}))
        public decimal? MiningRunCount
        {
            get => COUNTIFS(ProcessMiningRuns!this.ProcedureVersion, this.ProcedureVersionId); set { }
        }

        // Formula DriftedMiningRunCount (rulebook: =COUNTIFS(ProcessMiningRuns!{{DriftedMiningRunKey}}, {{ProcedureVersionId}}))
        public decimal? DriftedMiningRunCount
        {
            get => COUNTIFS(ProcessMiningRuns!this.DriftedMiningRunKey, this.ProcedureVersionId); set { }
        }

        // Formula HasUnresolvedMiningDrift (rulebook: ={{DriftedMiningRunCount}} > 0)
        public bool? HasUnresolvedMiningDrift
        {
            get => this.DriftedMiningRunCount > 0; set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
        public string? CreatedByAgent { get; set; }
        public string? ModifiedByAgent { get; set; }
        public string? EvaluationContext { get; set; }

        private Procedure _procedure;

        [ForeignKey("Procedure")]
        public virtual Procedure Procedure
        {
            get
            {
                if (_procedure == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedure - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedure = Context.Procedures.Find(Procedure);
                    if (_procedure != null)
                    {
                        Context.Attach(_procedure);
                    }
                }
                return _procedure;
            }
            set
            {
                if (_procedure != value)
                {
                    _procedure = value;
                    Procedure = _procedure == null ? default : _procedure.ProcedureId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. CreatedByAgent: " + CreatedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(CreatedByAgent);
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
                    CreatedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private Agent _agent;

        [ForeignKey("ModifiedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ModifiedByAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ModifiedByAgent: " + ModifiedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(ModifiedByAgent);
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
                    ModifiedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private EvaluationContext _evaluationContext;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContext
        {
            get
            {
                if (_evaluationContext == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContext - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContext = Context.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContext != null)
                    {
                        Context.Attach(_evaluationContext);
                    }
                }
                return _evaluationContext;
            }
            set
            {
                if (_evaluationContext != value)
                {
                    _evaluationContext = value;
                    EvaluationContext = _evaluationContext == null ? default : _evaluationContext.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ProcedureVersionLink> _procedureVersionLinks;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ProcedureVersionLink> ProcedureVersionLinks
        {
            get
            {
                if (_procedureVersionLinks == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionLinks - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _procedureVersionLinks = new ObservableCollection<ProcedureVersionLink>();
                    }
                    else
                    {
                        var items = Context.ProcedureVersionLinks.Where(x => x.PreviousProcedureVersion == this.ProcedureVersionId).ToList<ProcedureVersionLink>();
                        _procedureVersionLinks = new ObservableCollection<ProcedureVersionLink>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _procedureVersionLinks.CollectionChanged += ProcedureVersionLinks_CollectionChanged;
                }
                return _procedureVersionLinks;
            }
            private set
            {
                if (_procedureVersionLinks != null)
                {
                    _procedureVersionLinks.CollectionChanged -= ProcedureVersionLinks_CollectionChanged;
                }
                _procedureVersionLinks = value;
                if (_procedureVersionLinks != null)
                {
                    _procedureVersionLinks.CollectionChanged += ProcedureVersionLinks_CollectionChanged;
                }
            }
        }

        private void ProcedureVersionLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureVersionLink>())
                {
                    item.PreviousProcedureVersion = this.ProcedureVersionId;
                }
            }
        }

        private ObservableCollection<ProcedureVersionLink> _procedureVersionLinks;

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ProcedureVersionLink> ProcedureVersionLinks
        {
            get
            {
                if (_procedureVersionLinks == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionLinks - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _procedureVersionLinks = new ObservableCollection<ProcedureVersionLink>();
                    }
                    else
                    {
                        var items = Context.ProcedureVersionLinks.Where(x => x.NextProcedureVersion == this.ProcedureVersionId).ToList<ProcedureVersionLink>();
                        _procedureVersionLinks = new ObservableCollection<ProcedureVersionLink>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _procedureVersionLinks.CollectionChanged += ProcedureVersionLinks_CollectionChanged;
                }
                return _procedureVersionLinks;
            }
            private set
            {
                if (_procedureVersionLinks != null)
                {
                    _procedureVersionLinks.CollectionChanged -= ProcedureVersionLinks_CollectionChanged;
                }
                _procedureVersionLinks = value;
                if (_procedureVersionLinks != null)
                {
                    _procedureVersionLinks.CollectionChanged += ProcedureVersionLinks_CollectionChanged;
                }
            }
        }

        private void ProcedureVersionLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ProcedureStatusChange> ProcedureStatusChanges
        {
            get
            {
                if (_procedureStatusChanges == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureStatusChanges - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _procedureStatusChanges = new ObservableCollection<ProcedureStatusChange>();
                    }
                    else
                    {
                        var items = Context.ProcedureStatusChanges.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ProcedureStatusChange>();
                        _procedureStatusChanges = new ObservableCollection<ProcedureStatusChange>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<Step> Steps
        {
            get
            {
                if (_steps == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Steps - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _steps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = Context.Steps.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<Step>();
                        _steps = new ObservableCollection<Step>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<StepTransition> StepTransitions
        {
            get
            {
                if (_stepTransitions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepTransitions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _stepTransitions = new ObservableCollection<StepTransition>();
                    }
                    else
                    {
                        var items = Context.StepTransitions.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<StepTransition>();
                        _stepTransitions = new ObservableCollection<StepTransition>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<Rationale> Rationales
        {
            get
            {
                if (_rationales == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Rationales - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _rationales = new ObservableCollection<Rationale>();
                    }
                    else
                    {
                        var items = Context.Rationales.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<Rationale>();
                        _rationales = new ObservableCollection<Rationale>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<Exception> Exceptions
        {
            get
            {
                if (_exceptions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Exceptions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _exceptions = new ObservableCollection<Exception>();
                    }
                    else
                    {
                        var items = Context.Exceptions.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<Exception>();
                        _exceptions = new ObservableCollection<Exception>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ProcedureResource> ProcedureResources
        {
            get
            {
                if (_procedureResources == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureResources - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _procedureResources = new ObservableCollection<ProcedureResource>();
                    }
                    else
                    {
                        var items = Context.ProcedureResources.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ProcedureResource>();
                        _procedureResources = new ObservableCollection<ProcedureResource>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ElicitationSession> ElicitationSessions
        {
            get
            {
                if (_elicitationSessions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ElicitationSessions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _elicitationSessions = new ObservableCollection<ElicitationSession>();
                    }
                    else
                    {
                        var items = Context.ElicitationSessions.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ElicitationSession>();
                        _elicitationSessions = new ObservableCollection<ElicitationSession>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<KnowledgeFragment> KnowledgeFragments
        {
            get
            {
                if (_knowledgeFragments == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = Context.KnowledgeFragments.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<KnowledgeFragment>();
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<KnowledgeGap> KnowledgeGaps
        {
            get
            {
                if (_knowledgeGaps == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeGaps - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>();
                    }
                    else
                    {
                        var items = Context.KnowledgeGaps.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<KnowledgeGap>();
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<FAQ> FAQs
        {
            get
            {
                if (_fAQs == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FAQs - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _fAQs = new ObservableCollection<FAQ>();
                    }
                    else
                    {
                        var items = Context.FAQs.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<FAQ>();
                        _fAQs = new ObservableCollection<FAQ>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<Explanation> Explanations
        {
            get
            {
                if (_explanations == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Explanations - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _explanations = new ObservableCollection<Explanation>();
                    }
                    else
                    {
                        var items = Context.Explanations.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<Explanation>();
                        _explanations = new ObservableCollection<Explanation>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ProcedureExecution> ProcedureExecutions
        {
            get
            {
                if (_procedureExecutions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecutions - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>();
                    }
                    else
                    {
                        var items = Context.ProcedureExecutions.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ProcedureExecution>();
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<StewardshipAssignment> StewardshipAssignments
        {
            get
            {
                if (_stewardshipAssignments == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StewardshipAssignments - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _stewardshipAssignments = new ObservableCollection<StewardshipAssignment>();
                    }
                    else
                    {
                        var items = Context.StewardshipAssignments.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<StewardshipAssignment>();
                        _stewardshipAssignments = new ObservableCollection<StewardshipAssignment>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ChangeRequest> ChangeRequests
        {
            get
            {
                if (_changeRequests == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeRequests - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _changeRequests = new ObservableCollection<ChangeRequest>();
                    }
                    else
                    {
                        var items = Context.ChangeRequests.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ChangeRequest>();
                        _changeRequests = new ObservableCollection<ChangeRequest>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ReviewEvent> ReviewEvents
        {
            get
            {
                if (_reviewEvents == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ReviewEvents - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _reviewEvents = new ObservableCollection<ReviewEvent>();
                    }
                    else
                    {
                        var items = Context.ReviewEvents.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ReviewEvent>();
                        _reviewEvents = new ObservableCollection<ReviewEvent>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<LearningActivity> LearningActivities
        {
            get
            {
                if (_learningActivities == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LearningActivities - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _learningActivities = new ObservableCollection<LearningActivity>();
                    }
                    else
                    {
                        var items = Context.LearningActivities.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<LearningActivity>();
                        _learningActivities = new ObservableCollection<LearningActivity>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<OperationalBinding> OperationalBindings
        {
            get
            {
                if (_operationalBindings == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OperationalBindings - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _operationalBindings = new ObservableCollection<OperationalBinding>();
                    }
                    else
                    {
                        var items = Context.OperationalBindings.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<OperationalBinding>();
                        _operationalBindings = new ObservableCollection<OperationalBinding>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<CommunicationPolicy> CommunicationPolicies
        {
            get
            {
                if (_communicationPolicies == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunicationPolicies - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _communicationPolicies = new ObservableCollection<CommunicationPolicy>();
                    }
                    else
                    {
                        var items = Context.CommunicationPolicies.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<CommunicationPolicy>();
                        _communicationPolicies = new ObservableCollection<CommunicationPolicy>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("ProcedureVersion")]
        public virtual ObservableCollection<ProcessMiningRun> ProcessMiningRuns
        {
            get
            {
                if (_processMiningRuns == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessMiningRuns - no database context is set. ProcedureVersionId: " + this.ProcedureVersionId + ".");
                        }
                        _processMiningRuns = new ObservableCollection<ProcessMiningRun>();
                    }
                    else
                    {
                        var items = Context.ProcessMiningRuns.Where(x => x.ProcedureVersion == this.ProcedureVersionId).ToList<ProcessMiningRun>();
                        _processMiningRuns = new ObservableCollection<ProcessMiningRun>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.Procedure;
            _ = this.Agent;
            _ = this.Agent;
            _ = this.EvaluationContext;
            _ = this.ProcedureVersionLinks;
            _ = this.ProcedureVersionLinks;
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
