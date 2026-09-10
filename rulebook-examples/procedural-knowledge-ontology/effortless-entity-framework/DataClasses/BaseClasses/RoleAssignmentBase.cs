
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("RoleAssignments")]
    public class RoleAssignmentBase : SoAEntityBase
    {
        [Key]
        public string RoleAssignmentId { get; set; }

        // Formula Name (rulebook: ={{Role}} & " @ " & {{ValidFrom}})
        public string? Name
        {
            get => this.Role + " @ " + this.ValidFrom; set { }
        }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula IsCurrent (rulebook: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        public bool? IsCurrent
        {
            get => AND(this.ValidFrom <= this.AsOfInstant, OR(this.ValidTo = "", this.ValidTo > this.AsOfInstant)); set { }
        }

        // Formula CurrentAgentKey (rulebook: =IF({{IsCurrent}}, {{Agent}}, ""))
        public string? CurrentAgentKey
        {
            get => IF(this.IsCurrent, this.Agent, ""); set { }
        }

        // Formula IsCurrentlyValid (rulebook: =AND({{Status}} = "Active", OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        public bool? IsCurrentlyValid
        {
            get => AND(this.Status = "Active", OR(this.ValidTo = "", this.ValidTo > this.AsOfInstant)); set { }
        }

        // Formula AgentRoleKey (rulebook: =IF({{IsCurrentlyValid}}, {{Agent}} & "|" & {{Role}}, ""))
        public string? AgentRoleKey
        {
            get => IF(this.IsCurrentlyValid, this.Agent + "|" + this.Role, ""); set { }
        }

        // Formula HasDeparted (rulebook: =AND({{ValidTo}} <> "", {{ValidTo}} <= {{AsOfInstant}}))
        public bool? HasDeparted
        {
            get => AND(this.ValidTo <> "", this.ValidTo <= this.AsOfInstant); set { }
        }

        // Formula CoversNow (rulebook: =AND({{Status}} = "Active", {{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        public bool? CoversNow
        {
            get => AND(this.Status = "Active", this.ValidFrom <= this.AsOfInstant, OR(this.ValidTo = "", this.ValidTo > this.AsOfInstant)); set { }
        }

        // Formula RoleWhenCovering (rulebook: =IF({{CoversNow}}, {{Role}}, ""))
        public string? RoleWhenCovering
        {
            get => IF(this.CoversNow, this.Role, ""); set { }
        }

        // Formula AgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{Agent}}, Agents!{{AgentId}}, 0)))
        public string? AgentKind
        {
            get => INDEX(Agents!this.AgentKind, MATCH(this.Agent, Agents!this.AgentId, 0)); set { }
        }

        // Formula IsNonHumanAssignment (rulebook: =NOT({{AgentKind}} = "Human"))
        public bool? IsNonHumanAssignment
        {
            get => NOT(this.AgentKind = "Human"); set { }
        }

        // Formula PredecessorAgentKind (rulebook: =INDEX(RoleAssignments!{{AgentKind}}, MATCH({{SupersedesAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        public string? PredecessorAgentKind
        {
            get => INDEX(RoleAssignments!this.AgentKind, MATCH(this.SupersedesAssignment, RoleAssignments!this.RoleAssignmentId, 0)); set { }
        }

        // Formula IsHumanToNonHumanHandover (rulebook: =AND({{PredecessorAgentKind}} = "Human", {{IsNonHumanAssignment}}))
        public bool? IsHumanToNonHumanHandover
        {
            get => AND(this.PredecessorAgentKind = "Human", this.IsNonHumanAssignment); set { }
        }

        // Formula IsUnauthorizedNonHumanAssignment (rulebook: =AND({{IsNonHumanAssignment}}, NOT({{HasApprovingAuthority}})))
        public bool? IsUnauthorizedNonHumanAssignment
        {
            get => AND(this.IsNonHumanAssignment, NOT(this.HasApprovingAuthority)); set { }
        }

        // Formula WasAuthorizedByChangeRequest (rulebook: =AND({{HasApprovingAuthority}}, {{AuthorizingChangeRequest}} <> ""))
        public bool? WasAuthorizedByChangeRequest
        {
            get => AND(this.HasApprovingAuthority, this.AuthorizingChangeRequest <> ""); set { }
        }

        // Formula DecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{RoleAssignmentWhenScored}}, {{RoleAssignmentId}}))
        public decimal? DecisionCount
        {
            get => COUNTIFS(AgentDecisionRecords!this.RoleAssignmentWhenScored, this.RoleAssignmentId); set { }
        }

        // Formula OverriddenDecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{RoleAssignmentWhenOverridden}}, {{RoleAssignmentId}}))
        public decimal? OverriddenDecisionCount
        {
            get => COUNTIFS(AgentDecisionRecords!this.RoleAssignmentWhenOverridden, this.RoleAssignmentId); set { }
        }

        // Formula OverrideRatePercent (rulebook: =IF({{DecisionCount}} = 0, 0, ({{OverriddenDecisionCount}} * 100) / {{DecisionCount}}))
        public decimal? OverrideRatePercent
        {
            get => IF(this.DecisionCount = 0, 0, (this.OverriddenDecisionCount * 100) / this.DecisionCount); set { }
        }

        // Formula PredecessorOverrideRatePercent (rulebook: =INDEX(RoleAssignments!{{OverrideRatePercent}}, MATCH({{SupersedesAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        public decimal? PredecessorOverrideRatePercent
        {
            get => INDEX(RoleAssignments!this.OverrideRatePercent, MATCH(this.SupersedesAssignment, RoleAssignments!this.RoleAssignmentId, 0)); set { }
        }

        // Formula QualityRegressedVsPredecessor (rulebook: =AND({{SupersedesAssignment}} <> "", {{OverrideRatePercent}} > {{PredecessorOverrideRatePercent}}))
        public bool? QualityRegressedVsPredecessor
        {
            get => AND(this.SupersedesAssignment <> "", this.OverrideRatePercent > this.PredecessorOverrideRatePercent); set { }
        }

        // Formula DepartedRoleKey (rulebook: =IF({{HasDeparted}}, {{Role}}, ""))
        public string? DepartedRoleKey
        {
            get => IF(this.HasDeparted, this.Role, ""); set { }
        }

        public int? MinimumDecisionsForComparison { get; set; }
        // Formula PredecessorDecisionCount (rulebook: =INDEX(RoleAssignments!{{DecisionCount}}, MATCH({{SupersedesAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        public decimal? PredecessorDecisionCount
        {
            get => INDEX(RoleAssignments!this.DecisionCount, MATCH(this.SupersedesAssignment, RoleAssignments!this.RoleAssignmentId, 0)); set { }
        }

        // Formula HasSufficientSample (rulebook: ={{DecisionCount}} >= {{MinimumDecisionsForComparison}})
        public bool? HasSufficientSample
        {
            get => this.DecisionCount >= this.MinimumDecisionsForComparison; set { }
        }

        // Formula PredecessorHasSufficientSample (rulebook: ={{PredecessorDecisionCount}} >= {{MinimumDecisionsForComparison}})
        public bool? PredecessorHasSufficientSample
        {
            get => this.PredecessorDecisionCount >= this.MinimumDecisionsForComparison; set { }
        }

        // Formula ComparisonIsEvidentiallySound (rulebook: =AND({{HasSufficientSample}}, {{PredecessorHasSufficientSample}}))
        public bool? ComparisonIsEvidentiallySound
        {
            get => AND(this.HasSufficientSample, this.PredecessorHasSufficientSample); set { }
        }

        // Formula SingleOverrideSwingPercent (rulebook: =IF({{DecisionCount}} > 0, 100 / {{DecisionCount}}, 0))
        public decimal? SingleOverrideSwingPercent
        {
            get => IF(this.DecisionCount > 0, 100 / this.DecisionCount, 0); set { }
        }

        // Formula QualityVerdictIsUnsupported (rulebook: =AND(NOT({{ComparisonIsEvidentiallySound}}), NOT({{QualityRegressedVsPredecessor}})))
        public bool? QualityVerdictIsUnsupported
        {
            get => AND(NOT(this.ComparisonIsEvidentiallySound), NOT(this.QualityRegressedVsPredecessor)); set { }
        }

        // Formula IsUnmeasuredAutomationHandover (rulebook: =AND({{IsHumanToNonHumanHandover}}, NOT({{ComparisonIsEvidentiallySound}})))
        public bool? IsUnmeasuredAutomationHandover
        {
            get => AND(this.IsHumanToNonHumanHandover, NOT(this.ComparisonIsEvidentiallySound)); set { }
        }

        // Formula ErrorCorrectionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{ErrorCorrectionRoleAssignmentKey}}, {{RoleAssignmentId}}))
        public decimal? ErrorCorrectionCount
        {
            get => COUNTIFS(AgentDecisionRecords!this.ErrorCorrectionRoleAssignmentKey, this.RoleAssignmentId); set { }
        }

        // Formula ErrorRatePercent (rulebook: =IF({{DecisionCount}} > 0, {{ErrorCorrectionCount}} * 100 / {{DecisionCount}}, 0))
        public decimal? ErrorRatePercent
        {
            get => IF(this.DecisionCount > 0, this.ErrorCorrectionCount * 100 / this.DecisionCount, 0); set { }
        }

        public DateTime? AuthorizationDecidedAt { get; set; }
        public DateTime? AuthorizationReviewedAt { get; set; }
        public int? AuthorizationReviewCadenceDays { get; set; }
        // Formula HasDatedAuthorization (rulebook: =AND({{ApprovingAuthorityRole}} <> "", {{AuthorizationDecidedAt}} <> ""))
        public bool? HasDatedAuthorization
        {
            get => AND(this.ApprovingAuthorityRole <> "", this.AuthorizationDecidedAt <> ""); set { }
        }

        // Formula DaysSinceAuthorizationReview (rulebook: =IF({{AuthorizationReviewedAt}} <> "", DATETIME_DIFF({{AsOfInstant}}, {{AuthorizationReviewedAt}}, "days"), DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days")))
        public int? DaysSinceAuthorizationReview
        {
            get => IF(this.AuthorizationReviewedAt <> "", DATETIME_DIFF(this.AsOfInstant, this.AuthorizationReviewedAt, "days"), DATETIME_DIFF(this.AsOfInstant, this.ValidFrom, "days")); set { }
        }

        // Formula AuthorizationIsOverdueForReview (rulebook: =AND({{AuthorizationReviewCadenceDays}} > 0, {{DaysSinceAuthorizationReview}} > {{AuthorizationReviewCadenceDays}}))
        public bool? AuthorizationIsOverdueForReview
        {
            get => AND(this.AuthorizationReviewCadenceDays > 0, this.DaysSinceAuthorizationReview > this.AuthorizationReviewCadenceDays); set { }
        }

        // Formula IsStandingUnreviewedAutomation (rulebook: =AND({{CoversNow}}, AND({{IsNonHumanAssignment}}, {{AuthorizationIsOverdueForReview}})))
        public bool? IsStandingUnreviewedAutomation
        {
            get => AND(this.CoversNow, AND(this.IsNonHumanAssignment, this.AuthorizationIsOverdueForReview)); set { }
        }

        // Formula IsUnconditionedAutomationHandover (rulebook: =AND({{IsHumanToNonHumanHandover}}, {{AuthorizationReviewCadenceDays}} = 0))
        public bool? IsUnconditionedAutomationHandover
        {
            get => AND(this.IsHumanToNonHumanHandover, this.AuthorizationReviewCadenceDays = 0); set { }
        }

        public int? MaxTolerableErrorRatePercent { get; set; }
        // Formula ExceedsTolerableErrorRate (rulebook: =AND({{MaxTolerableErrorRatePercent}} > 0, {{ErrorRatePercent}} >= {{MaxTolerableErrorRatePercent}}))
        public bool? ExceedsTolerableErrorRate
        {
            get => AND(this.MaxTolerableErrorRatePercent > 0, this.ErrorRatePercent >= this.MaxTolerableErrorRatePercent); set { }
        }

        // Formula BoundaryViolationCountForAssignment (rulebook: =COUNTIFS(AgentDecisionRecords!{{BoundaryViolationRoleAssignmentKey}}, {{RoleAssignmentId}}))
        public decimal? BoundaryViolationCountForAssignment
        {
            get => COUNTIFS(AgentDecisionRecords!this.BoundaryViolationRoleAssignmentKey, this.RoleAssignmentId); set { }
        }

        // Formula HasAnyBoundaryViolation (rulebook: =({{BoundaryViolationCountForAssignment}} > 0))
        public bool? HasAnyBoundaryViolation
        {
            get => (this.BoundaryViolationCountForAssignment > 0); set { }
        }

        // Formula HasUngroundedGoverningBoundary (rulebook: =INDEX(Roles!{{IsGovernedByLapsedAuthority}}, MATCH({{Role}}, Roles!{{RoleId}}, 0)))
        public bool? HasUngroundedGoverningBoundary
        {
            get => INDEX(Roles!this.IsGovernedByLapsedAuthority, MATCH(this.Role, Roles!this.RoleId, 0)); set { }
        }

        // Formula SuspensionConditionMet (rulebook: =OR({{ExceedsTolerableErrorRate}}, OR({{HasAnyBoundaryViolation}}, {{HasUngroundedGoverningBoundary}})))
        public bool? SuspensionConditionMet
        {
            get => OR(this.ExceedsTolerableErrorRate, OR(this.HasAnyBoundaryViolation, this.HasUngroundedGoverningBoundary)); set { }
        }

        // Formula IsOperatingUnderMetSuspensionCondition (rulebook: =AND({{SuspensionConditionMet}}, AND({{CoversNow}}, {{IsNonHumanAssignment}})))
        public bool? IsOperatingUnderMetSuspensionCondition
        {
            get => AND(this.SuspensionConditionMet, AND(this.CoversNow, this.IsNonHumanAssignment)); set { }
        }

        // Formula HasDeclaredSuspensionCondition (rulebook: ={{MaxTolerableErrorRatePercent}} > 0)
        public bool? HasDeclaredSuspensionCondition
        {
            get => this.MaxTolerableErrorRatePercent > 0; set { }
        }

        // Formula HasApprovingAuthority (rulebook: ={{ApprovingAuthorityRole}} <> "")
        public bool? HasApprovingAuthority
        {
            get => this.ApprovingAuthorityRole <> ""; set { }
        }

        // Formula HasAuthorizingChangeRequest (rulebook: ={{AuthorizingChangeRequest}} <> "")
        public bool? HasAuthorizingChangeRequest
        {
            get => this.AuthorizingChangeRequest <> ""; set { }
        }

        public bool? IsEnforcementRole { get; set; }
        // Formula IsUnauthorizedEnforcementAgent (rulebook: =AND({{IsEnforcementRole}}, {{IsUnauthorizedNonHumanAssignment}}))
        public bool? IsUnauthorizedEnforcementAgent
        {
            get => AND(this.IsEnforcementRole, this.IsUnauthorizedNonHumanAssignment); set { }
        }

        // Formula GovernanceEvidenceCount (rulebook: =IF({{HasApprovingAuthority}}, 1, 0) + IF({{HasAuthorizingChangeRequest}}, 1, 0))
        public int? GovernanceEvidenceCount
        {
            get => IF(this.HasApprovingAuthority, 1, 0) + IF(this.HasAuthorizingChangeRequest, 1, 0); set { }
        }

        // Formula UnauthorizedEnforcementRoleKey (rulebook: =IF({{IsUnauthorizedNonHumanAssignment}}, {{Role}}, ""))
        public string? UnauthorizedEnforcementRoleKey
        {
            get => IF(this.IsUnauthorizedNonHumanAssignment, this.Role, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Role { get; set; }
        public string? Agent { get; set; }
        public string? EvaluationContext { get; set; }
        public string? SupersedesAssignment { get; set; }
        public string? ApprovingAuthorityRole { get; set; }
        public string? AuthorizingChangeRequest { get; set; }

        private Role _role;

        [ForeignKey("Role")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(Role))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. Role: " + Role + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(Role);
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
                    Role = _role == null ? default : _role.RoleId;
                }
            }
        }

        private Agent _agent;

        [ForeignKey("Agent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(Agent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. Agent: " + Agent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(Agent);
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
                    Agent = _agent == null ? default : _agent.AgentId;
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

        private RoleAssignment _roleAssignment;

        [ForeignKey("SupersedesAssignment")]
        public virtual RoleAssignment RoleAssignment
        {
            get
            {
                if (_roleAssignment == null && !string.IsNullOrEmpty(SupersedesAssignment))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignment - no database context is set. SupersedesAssignment: " + SupersedesAssignment + ".");
                        }
                        return null;
                    }
                    _roleAssignment = Context.RoleAssignments.Find(SupersedesAssignment);
                    if (_roleAssignment != null)
                    {
                        Context.Attach(_roleAssignment);
                    }
                }
                return _roleAssignment;
            }
            set
            {
                if (_roleAssignment != value)
                {
                    _roleAssignment = value;
                    SupersedesAssignment = _roleAssignment == null ? default : _roleAssignment.RoleAssignmentId;
                }
            }
        }

        private Role _approvingAuthorityRole;

        [ForeignKey("ApprovingAuthorityRole")]
        public virtual Role ApprovingAuthorityRole
        {
            get
            {
                if (_approvingAuthorityRole == null && !string.IsNullOrEmpty(ApprovingAuthorityRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApprovingAuthorityRole - no database context is set. ApprovingAuthorityRole: " + ApprovingAuthorityRole + ".");
                        }
                        return null;
                    }
                    _approvingAuthorityRole = Context.Roles.Find(ApprovingAuthorityRole);
                    if (_approvingAuthorityRole != null)
                    {
                        Context.Attach(_approvingAuthorityRole);
                    }
                }
                return _approvingAuthorityRole;
            }
            set
            {
                if (_approvingAuthorityRole != value)
                {
                    _approvingAuthorityRole = value;
                    ApprovingAuthorityRole = _approvingAuthorityRole == null ? default : _approvingAuthorityRole.RoleId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeRequest - no database context is set. AuthorizingChangeRequest: " + AuthorizingChangeRequest + ".");
                        }
                        return null;
                    }
                    _changeRequest = Context.ChangeRequests.Find(AuthorizingChangeRequest);
                    if (_changeRequest != null)
                    {
                        Context.Attach(_changeRequest);
                    }
                }
                return _changeRequest;
            }
            set
            {
                if (_changeRequest != value)
                {
                    _changeRequest = value;
                    AuthorizingChangeRequest = _changeRequest == null ? default : _changeRequest.ChangeRequestId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. RoleAssignmentId: " + this.RoleAssignmentId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = Context.RoleAssignments.Where(x => x.SupersedesAssignment == this.RoleAssignmentId).ToList<RoleAssignment>();
                        _roleAssignments = new ObservableCollection<RoleAssignment>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentDecisionRecords - no database context is set. RoleAssignmentId: " + this.RoleAssignmentId + ".");
                        }
                        _agentDecisionRecords = new ObservableCollection<AgentDecisionRecord>();
                    }
                    else
                    {
                        var items = Context.AgentDecisionRecords.Where(x => x.UnderRoleAssignment == this.RoleAssignmentId).ToList<AgentDecisionRecord>();
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
                    item.UnderRoleAssignment = this.RoleAssignmentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.Agent;
            _ = this.EvaluationContext;
            _ = this.RoleAssignment;
            _ = this.ApprovingAuthorityRole;
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
