
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("KnowledgeFragments")]
    public class KnowledgeFragmentBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeFragmentId { get; set; }

        // Formula Name (rulebook: ={{KnowledgeForm}} & ": " & LEFT({{Statement}}, 60))
        public string? Name
        {
            get => this.KnowledgeForm + ": " + LEFT(this.Statement, 60); set { }
        }

        public string? KnowledgeForm { get; set; }
        public string? Statement { get; set; }
        public string? Confidence { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string? Status { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula IsCurrentlyValid (rulebook: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}}), {{Status}} = "Approved"))
        public bool? IsCurrentlyValid
        {
            get => AND(this.ValidFrom <= this.AsOfInstant, OR(this.ValidTo = "", this.ValidTo > this.AsOfInstant), this.Status = "Approved"); set { }
        }

        // Formula SourceAgentIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{SourceAgent}}, Agents!{{AgentId}}, 0)))
        public bool? SourceAgentIsStillEngaged
        {
            get => INDEX(Agents!this.IsStillEngaged, MATCH(this.SourceAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula SourceAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{SourceAgent}}, Agents!{{AgentId}}, 0)))
        public string? SourceAgentKind
        {
            get => INDEX(Agents!this.AgentKind, MATCH(this.SourceAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula HasHumanSource (rulebook: ={{SourceAgentKind}} = "Human")
        public bool? HasHumanSource
        {
            get => this.SourceAgentKind = "Human"; set { }
        }

        // Formula HasOrphanedProvenance (rulebook: =AND({{IsCurrentlyValid}}, NOT({{SourceAgentIsStillEngaged}})))
        public bool? HasOrphanedProvenance
        {
            get => AND(this.IsCurrentlyValid, NOT(this.SourceAgentIsStillEngaged)); set { }
        }

        // Formula IsUndefendableTacitClaim (rulebook: =AND({{HasOrphanedProvenance}}, OR({{KnowledgeForm}} = "Tacit", {{KnowledgeForm}} = "SituatedJudgment")))
        public bool? IsUndefendableTacitClaim
        {
            get => AND(this.HasOrphanedProvenance, OR(this.KnowledgeForm = "Tacit", this.KnowledgeForm = "SituatedJudgment")); set { }
        }

        // Formula IsApproved (rulebook: ={{Status}} = "Approved")
        public bool? IsApproved
        {
            get => this.Status = "Approved"; set { }
        }

        // Formula IsWithinValidityWindow (rulebook: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        public bool? IsWithinValidityWindow
        {
            get => AND(this.ValidFrom <= this.AsOfInstant, OR(this.ValidTo = "", this.ValidTo > this.AsOfInstant)); set { }
        }

        // Formula IsReliedUpon (rulebook: =AND({{Step}} <> "", {{IsWithinValidityWindow}}))
        public bool? IsReliedUpon
        {
            get => AND(this.Step <> "", this.IsWithinValidityWindow); set { }
        }

        // Formula StepProcedureVersionStatus (rulebook: =INDEX(Steps!{{ProcedureVersion}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public string? StepProcedureVersionStatus
        {
            get => INDEX(Steps!this.ProcedureVersion, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula IsAttachedToLiveVersion (rulebook: =INDEX(ProcedureVersions!{{IsLive}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        public bool? IsAttachedToLiveVersion
        {
            get => INDEX(ProcedureVersions!this.IsLive, MATCH(this.ProcedureVersion, ProcedureVersions!this.ProcedureVersionId, 0)); set { }
        }

        // Formula IsUnapprovedButReliedOn (rulebook: =AND({{IsReliedUpon}}, {{IsAttachedToLiveVersion}}, NOT({{IsApproved}})))
        public bool? IsUnapprovedButReliedOn
        {
            get => AND(this.IsReliedUpon, this.IsAttachedToLiveVersion, NOT(this.IsApproved)); set { }
        }

        // Formula EvidenceAgeDays (rulebook: =INDEX(ElicitationSessions!{{DaysSinceElicited}}, MATCH({{ElicitationSession}}, ElicitationSessions!{{ElicitationSessionId}}, 0)))
        public int? EvidenceAgeDays
        {
            get => INDEX(ElicitationSessions!this.DaysSinceElicited, MATCH(this.ElicitationSession, ElicitationSessions!this.ElicitationSessionId, 0)); set { }
        }

        // Formula HasRecordedElicitation (rulebook: ={{ElicitationSession}} <> "")
        public bool? HasRecordedElicitation
        {
            get => this.ElicitationSession <> ""; set { }
        }

        // Formula IsFromSingleWitness (rulebook: =INDEX(ElicitationSessions!{{IsSingleWitnessMethod}}, MATCH({{ElicitationSession}}, ElicitationSessions!{{ElicitationSessionId}}, 0)))
        public bool? IsFromSingleWitness
        {
            get => INDEX(ElicitationSessions!this.IsSingleWitnessMethod, MATCH(this.ElicitationSession, ElicitationSessions!this.ElicitationSessionId, 0)); set { }
        }

        // Formula EvidenceExpiryDays (rulebook: =IF({{IsFromSingleWitness}}, 180, 365))
        public int? EvidenceExpiryDays
        {
            get => IF(this.IsFromSingleWitness, 180, 365); set { }
        }

        // Formula EvidenceHasExpired (rulebook: =AND({{HasRecordedElicitation}}, {{EvidenceAgeDays}} > {{EvidenceExpiryDays}}))
        public bool? EvidenceHasExpired
        {
            get => AND(this.HasRecordedElicitation, this.EvidenceAgeDays > this.EvidenceExpiryDays); set { }
        }

        // Formula OwnerAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        public string? OwnerAgent
        {
            get => INDEX(Roles!this.CurrentAgent, MATCH(this.OwnerRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula IsAwaitingApproval (rulebook: ={{Status}} = "Reviewed")
        public bool? IsAwaitingApproval
        {
            get => this.Status = "Reviewed"; set { }
        }

        // Formula OwnerIsMe (rulebook: ={{OwnerRole}} = "hr-policy-owner")
        public bool? OwnerIsMe
        {
            get => this.OwnerRole = "hr-policy-owner"; set { }
        }

        // Formula IsMyUnfinishedApproval (rulebook: =AND({{OwnerIsMe}}, {{IsAwaitingApproval}}))
        public bool? IsMyUnfinishedApproval
        {
            get => AND(this.OwnerIsMe, this.IsAwaitingApproval); set { }
        }

        // Formula IsInvokedByAnException (rulebook: =COUNTIFS(Exceptions!{{TriggerStep}}, KnowledgeFragments!{{Step}}))
        public int? IsInvokedByAnException
        {
            get => this.Exceptions == null ? 0 : this.Exceptions.Count; set { }
        }

        // Formula HasOperationalReliance (rulebook: ={{IsInvokedByAnException}} > 0)
        public bool? HasOperationalReliance
        {
            get => this.IsInvokedByAnException > 0; set { }
        }

        // Formula IsUnapprovedAndOperationallyLive (rulebook: =AND({{IsMyUnfinishedApproval}}, {{HasOperationalReliance}}))
        public bool? IsUnapprovedAndOperationallyLive
        {
            get => AND(this.IsMyUnfinishedApproval, this.HasOperationalReliance); set { }
        }

        // Formula AgeDays (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days"))
        public int? AgeDays
        {
            get => DATETIME_DIFF(this.AsOfInstant, this.ValidFrom, "days"); set { }
        }

        // Formula IsLowConfidence (rulebook: =OR({{Confidence}} = "Medium", {{Confidence}} = "Low"))
        public bool? IsLowConfidence
        {
            get => OR(this.Confidence = "Medium", this.Confidence = "Low"); set { }
        }

        // Formula OwningVersionCadenceDays (rulebook: =INDEX(ProcedureVersions!{{StewardReviewCadenceDays}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        public int? OwningVersionCadenceDays
        {
            get => INDEX(ProcedureVersions!this.StewardReviewCadenceDays, MATCH(this.ProcedureVersion, ProcedureVersions!this.ProcedureVersionId, 0)); set { }
        }

        // Formula ExceedsOwningCadence (rulebook: ={{AgeDays}} > {{OwningVersionCadenceDays}})
        public bool? ExceedsOwningCadence
        {
            get => this.AgeDays > this.OwningVersionCadenceDays; set { }
        }

        // Formula IsAgingLowConfidenceClaim (rulebook: =AND({{ExceedsOwningCadence}}, {{IsLowConfidence}}))
        public bool? IsAgingLowConfidenceClaim
        {
            get => AND(this.ExceedsOwningCadence, this.IsLowConfidence); set { }
        }

        // Formula OwnerRoleAgentKind (rulebook: =INDEX(Roles!{{CurrentAgentKind}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        public string? OwnerRoleAgentKind
        {
            get => INDEX(Roles!this.CurrentAgentKind, MATCH(this.OwnerRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula IsHumanOwned (rulebook: ={{OwnerRoleAgentKind}} = "Human")
        public bool? IsHumanOwned
        {
            get => this.OwnerRoleAgentKind = "Human"; set { }
        }

        // Formula IsAiValidatedByAi (rulebook: =AND(NOT({{SourceAgentKind}} = "Human"), NOT({{IsHumanOwned}})))
        public bool? IsAiValidatedByAi
        {
            get => AND(NOT(this.SourceAgentKind = "Human"), NOT(this.IsHumanOwned)); set { }
        }

        // Formula ReviewCadenceDays (rulebook: =INDEX(ProcedureVersions!{{StewardReviewCadenceDays}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        public int? ReviewCadenceDays
        {
            get => INDEX(ProcedureVersions!this.StewardReviewCadenceDays, MATCH(this.ProcedureVersion, ProcedureVersions!this.ProcedureVersionId, 0)); set { }
        }

        // Formula IsOverdueForReview (rulebook: =AND({{IsCurrentlyValid}}, {{AgeDays}} > {{ReviewCadenceDays}}))
        public bool? IsOverdueForReview
        {
            get => AND(this.IsCurrentlyValid, this.AgeDays > this.ReviewCadenceDays); set { }
        }

        // Formula PredatesCurrentRoleHolder (rulebook: =AND({{OwnerRoleAgentKind}} <> "", {{ValidFrom}} < {{OwnerRoleAssignmentValidFrom}}))
        public bool? PredatesCurrentRoleHolder
        {
            get => AND(this.OwnerRoleAgentKind <> "", this.ValidFrom < this.OwnerRoleAssignmentValidFrom); set { }
        }

        // Formula OwnerRoleAssignmentValidFrom (rulebook: =INDEX(Roles!{{CurrentAssignmentValidFrom}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        public DateTime? OwnerRoleAssignmentValidFrom
        {
            get => INDEX(Roles!this.CurrentAssignmentValidFrom, MATCH(this.OwnerRole, Roles!this.RoleId, 0)); set { }
        }

        public DateTime? LastReviewedAt { get; set; }
        // Formula FragilitySignalCount (rulebook: =IF({{IsFromSingleWitness}}, 1, 0) + IF({{IsOverdueForReview}}, 1, 0) + IF({{IsLowConfidence}}, 1, 0) + IF({{HasOperationalReliance}}, 1, 0))
        public int? FragilitySignalCount
        {
            get => IF(this.IsFromSingleWitness, 1, 0) + IF(this.IsOverdueForReview, 1, 0) + IF(this.IsLowConfidence, 1, 0) + IF(this.HasOperationalReliance, 1, 0); set { }
        }

        // Formula IsCompoundFragile (rulebook: ={{FragilitySignalCount}} >= 3)
        public bool? IsCompoundFragile
        {
            get => this.FragilitySignalCount >= 3; set { }
        }

        // Formula IsSinglePointOfFailure (rulebook: =AND({{IsFromSingleWitness}}, {{HasOperationalReliance}}))
        public bool? IsSinglePointOfFailure
        {
            get => AND(this.IsFromSingleWitness, this.HasOperationalReliance); set { }
        }

        // Formula IsExpiringSinglePointOfFailure (rulebook: =AND({{IsSinglePointOfFailure}}, {{IsOverdueForReview}}))
        public bool? IsExpiringSinglePointOfFailure
        {
            get => AND(this.IsSinglePointOfFailure, this.IsOverdueForReview); set { }
        }

        // Formula CompoundFragileVersionKey (rulebook: =IF({{IsCompoundFragile}}, {{ProcedureVersion}}, ""))
        public string? CompoundFragileVersionKey
        {
            get => IF(this.IsCompoundFragile, this.ProcedureVersion, ""); set { }
        }

        // Formula ValidFragmentSessionKey (rulebook: =IF({{IsCurrentlyValid}}, {{ElicitationSession}}, ""))
        public string? ValidFragmentSessionKey
        {
            get => IF(this.IsCurrentlyValid, this.ElicitationSession, ""); set { }
        }

        // Formula ConsumingStepIsSoftwareAssigned (rulebook: =INDEX(Steps!{{IsSoftwareAssigned}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public bool? ConsumingStepIsSoftwareAssigned
        {
            get => INDEX(Steps!this.IsSoftwareAssigned, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula ConsumingStepAgentKind (rulebook: =INDEX(Steps!{{AssignedAgentKind}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        public string? ConsumingStepAgentKind
        {
            get => INDEX(Steps!this.AssignedAgentKind, MATCH(this.Step, Steps!this.StepId, 0)); set { }
        }

        // Formula IsUnapprovedAndMachineConsumed (rulebook: =AND({{IsUnapprovedButReliedOn}}, {{ConsumingStepIsSoftwareAssigned}}))
        public bool? IsUnapprovedAndMachineConsumed
        {
            get => AND(this.IsUnapprovedButReliedOn, this.ConsumingStepIsSoftwareAssigned); set { }
        }

        // Formula IsUnapprovedAndHumanConsumed (rulebook: =AND({{IsUnapprovedButReliedOn}}, NOT({{ConsumingStepIsSoftwareAssigned}})))
        public bool? IsUnapprovedAndHumanConsumed
        {
            get => AND(this.IsUnapprovedButReliedOn, NOT(this.ConsumingStepIsSoftwareAssigned)); set { }
        }

        // Formula MachineConsumedUnapprovedVersionKey (rulebook: =IF({{IsUnapprovedAndMachineConsumed}}, {{ProcedureVersion}}, ""))
        public string? MachineConsumedUnapprovedVersionKey
        {
            get => IF(this.IsUnapprovedAndMachineConsumed, this.ProcedureVersion, ""); set { }
        }

        // Formula HasReviewRecord (rulebook: ={{LastReviewedAt}} <> "")
        public bool? HasReviewRecord
        {
            get => this.LastReviewedAt <> ""; set { }
        }

        // Formula DaysSinceActualReview (rulebook: =IF({{HasReviewRecord}}, DATETIME_DIFF({{AsOfInstant}}, {{LastReviewedAt}}, "days"), 0))
        public int? DaysSinceActualReview
        {
            get => IF(this.HasReviewRecord, DATETIME_DIFF(this.AsOfInstant, this.LastReviewedAt, "days"), 0); set { }
        }

        // Formula IsUnreviewedSinceAuthoring (rulebook: =AND({{IsCurrentlyValid}}, NOT({{HasReviewRecord}})))
        public bool? IsUnreviewedSinceAuthoring
        {
            get => AND(this.IsCurrentlyValid, NOT(this.HasReviewRecord)); set { }
        }

        // Formula IsGenuinelyOverdue (rulebook: =AND({{IsCurrentlyValid}}, {{HasReviewRecord}}, {{DaysSinceActualReview}} > {{ReviewCadenceDays}}))
        public bool? IsGenuinelyOverdue
        {
            get => AND(this.IsCurrentlyValid, this.HasReviewRecord, this.DaysSinceActualReview > this.ReviewCadenceDays); set { }
        }

        // Formula ReviewRecencyIsInferred (rulebook: =AND({{IsOverdueForReview}}, NOT({{HasReviewRecord}})))
        public bool? ReviewRecencyIsInferred
        {
            get => AND(this.IsOverdueForReview, NOT(this.HasReviewRecord)); set { }
        }

        // Formula InferenceDisagreesWithRecord (rulebook: =AND({{HasReviewRecord}}, {{IsOverdueForReview}}, NOT({{IsGenuinelyOverdue}})))
        public bool? InferenceDisagreesWithRecord
        {
            get => AND(this.HasReviewRecord, this.IsOverdueForReview, NOT(this.IsGenuinelyOverdue)); set { }
        }

        // Formula GenuinelyOverdueVersionKey (rulebook: =IF({{IsGenuinelyOverdue}}, {{ProcedureVersion}}, ""))
        public string? GenuinelyOverdueVersionKey
        {
            get => IF(this.IsGenuinelyOverdue, this.ProcedureVersion, ""); set { }
        }

        // Formula RatifiedBoundaryCount (rulebook: =COUNTIFS(AuthorityBoundaries!{{RatifyingFragmentKey}}, {{KnowledgeFragmentId}}))
        public decimal? RatifiedBoundaryCount
        {
            get => COUNTIFS(AuthorityBoundaries!this.RatifyingFragmentKey, this.KnowledgeFragmentId); set { }
        }

        // Formula RelianceSurfaceCount (rulebook: ={{IsInvokedByAnException}} + {{RatifiedBoundaryCount}})
        public int? RelianceSurfaceCount
        {
            get => this.IsInvokedByAnException + this.RatifiedBoundaryCount; set { }
        }

        // Formula DaysAwaitingMyApproval (rulebook: =IF({{IsMyUnfinishedApproval}}, DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days"), 0))
        public int? DaysAwaitingMyApproval
        {
            get => IF(this.IsMyUnfinishedApproval, DATETIME_DIFF(this.AsOfInstant, this.ValidFrom, "days"), 0); set { }
        }

        // Formula IsHighBlastRadiusUnapproved (rulebook: =AND({{IsUnapprovedAndOperationallyLive}}, {{RelianceSurfaceCount}} > 1))
        public bool? IsHighBlastRadiusUnapproved
        {
            get => AND(this.IsUnapprovedAndOperationallyLive, this.RelianceSurfaceCount > 1); set { }
        }

        // Formula IsLongUnapproved (rulebook: =AND({{IsMyUnfinishedApproval}}, {{DaysAwaitingMyApproval}} > 30))
        public bool? IsLongUnapproved
        {
            get => AND(this.IsMyUnfinishedApproval, this.DaysAwaitingMyApproval > 30); set { }
        }

        // Formula UnapprovedLoadBearingVersionKey (rulebook: =IF({{IsHighBlastRadiusUnapproved}}, {{ProcedureVersion}}, ""))
        public string? UnapprovedLoadBearingVersionKey
        {
            get => IF(this.IsHighBlastRadiusUnapproved, this.ProcedureVersion, ""); set { }
        }

        // Formula OwnerRoleIsVacated (rulebook: =INDEX(Roles!{{IsVacatedRole}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        public bool? OwnerRoleIsVacated
        {
            get => INDEX(Roles!this.IsVacatedRole, MATCH(this.OwnerRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula IsOrphanedByRole (rulebook: =AND({{IsCurrentlyValid}}, {{OwnerRoleIsVacated}}))
        public bool? IsOrphanedByRole
        {
            get => AND(this.IsCurrentlyValid, this.OwnerRoleIsVacated); set { }
        }

        // Formula ValidFragmentVersionKey (rulebook: =IF({{IsCurrentlyValid}}, {{ProcedureVersion}}, ""))
        public string? ValidFragmentVersionKey
        {
            get => IF(this.IsCurrentlyValid, this.ProcedureVersion, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? ElicitationSession { get; set; }
        public string? SourceAgent { get; set; }
        public string? OwnerRole { get; set; }
        public string? EvaluationContext { get; set; }

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

        private ElicitationSession _elicitationSession;

        [ForeignKey("ElicitationSession")]
        public virtual ElicitationSession ElicitationSession
        {
            get
            {
                if (_elicitationSession == null && !string.IsNullOrEmpty(ElicitationSession))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ElicitationSession - no database context is set. ElicitationSession: " + ElicitationSession + ".");
                        }
                        return null;
                    }
                    _elicitationSession = Context.ElicitationSessions.Find(ElicitationSession);
                    if (_elicitationSession != null)
                    {
                        Context.Attach(_elicitationSession);
                    }
                }
                return _elicitationSession;
            }
            set
            {
                if (_elicitationSession != value)
                {
                    _elicitationSession = value;
                    ElicitationSession = _elicitationSession == null ? default : _elicitationSession.ElicitationSessionId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. SourceAgent: " + SourceAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(SourceAgent);
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
                    SourceAgent = _agent == null ? default : _agent.AgentId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. OwnerRole: " + OwnerRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(OwnerRole);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OwnerRole = _role == null ? default : _role.RoleId;
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

        private ObservableCollection<AuthorityBoundary> _authorityBoundaries;

        [InverseProperty("KnowledgeFragment")]
        public virtual ObservableCollection<AuthorityBoundary> AuthorityBoundaries
        {
            get
            {
                if (_authorityBoundaries == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorityBoundaries - no database context is set. KnowledgeFragmentId: " + this.KnowledgeFragmentId + ".");
                        }
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>();
                    }
                    else
                    {
                        var items = Context.AuthorityBoundaries.Where(x => x.RatifiedByKnowledgeFragment == this.KnowledgeFragmentId).ToList<AuthorityBoundary>();
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.Step;
            _ = this.ElicitationSession;
            _ = this.Agent;
            _ = this.Role;
            _ = this.EvaluationContext;
            _ = this.AuthorityBoundaries;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
