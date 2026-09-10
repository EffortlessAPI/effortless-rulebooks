
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ChangeRequests")]
    public class ChangeRequestBase : SoAEntityBase
    {
        [Key]
        public string ChangeRequestId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        public string? Name
        {
            get => this.Title; set { }
        }

        public string? Title { get; set; }
        public string? ChangeKind { get; set; }
        public string? Status { get; set; }
        public DateTime? RequestedAt { get; set; }
        public DateTime? DecidedAt { get; set; }
        public string? ImpactAssessment { get; set; }
        // Formula IsOpen (rulebook: =AND(OR({{Status}} = "Draft", {{Status}} = "UnderReview", {{Status}} = "Approved"), {{ImplementedAt}} = ""))
        public bool? IsOpen
        {
            get => AND(OR(this.Status = "Draft", this.Status = "UnderReview", this.Status = "Approved"), this.ImplementedAt = ""); set { }
        }

        // Formula OpenChangeVersionKey (rulebook: =IF({{IsOpen}}, {{ProcedureVersion}}, ""))
        public string? OpenChangeVersionKey
        {
            get => IF(this.IsOpen, this.ProcedureVersion, ""); set { }
        }

        // Formula IsDecided (rulebook: ={{DecidedAt}} <> "")
        public bool? IsDecided
        {
            get => this.DecidedAt <> ""; set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula DaysPending (rulebook: =IF({{IsDecided}}, DATETIME_DIFF({{DecidedAt}}, {{RequestedAt}}, "days"), DATETIME_DIFF({{AsOfInstant}}, {{RequestedAt}}, "days")))
        public int? DaysPending
        {
            get => IF(this.IsDecided, DATETIME_DIFF(this.DecidedAt, this.RequestedAt, "days"), DATETIME_DIFF(this.AsOfInstant, this.RequestedAt, "days")); set { }
        }

        // Formula IsStillPending (rulebook: =AND({{IsOpen}}, NOT({{IsDecided}})))
        public bool? IsStillPending
        {
            get => AND(this.IsOpen, NOT(this.IsDecided)); set { }
        }

        // Formula IsStalled (rulebook: =AND({{IsStillPending}}, {{DaysPending}} > 14))
        public bool? IsStalled
        {
            get => AND(this.IsStillPending, this.DaysPending > 14); set { }
        }

        // Formula AuthorityAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{AuthorityRole}}, Roles!{{RoleId}}, 0)))
        public string? AuthorityAgent
        {
            get => INDEX(Roles!this.CurrentAgent, MATCH(this.AuthorityRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula RequesterIsAuthority (rulebook: ={{RequestedByAgent}} = {{AuthorityAgent}})
        public bool? RequesterIsAuthority
        {
            get => this.RequestedByAgent = this.AuthorityAgent; set { }
        }

        // Formula AwaitsAuthorityDecision (rulebook: =AND({{Status}} = "UnderReview", NOT({{IsDecided}})))
        public bool? AwaitsAuthorityDecision
        {
            get => AND(this.Status = "UnderReview", NOT(this.IsDecided)); set { }
        }

        // Formula AuthorityRoleLabel (rulebook: =INDEX(Roles!{{Label}}, MATCH({{AuthorityRole}}, Roles!{{RoleId}}, 0)))
        public string? AuthorityRoleLabel
        {
            get => INDEX(Roles!this.Label, MATCH(this.AuthorityRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula TouchesLiveVersion (rulebook: =INDEX(ProcedureVersions!{{IsLive}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        public bool? TouchesLiveVersion
        {
            get => INDEX(ProcedureVersions!this.IsLive, MATCH(this.ProcedureVersion, ProcedureVersions!this.ProcedureVersionId, 0)); set { }
        }

        // Formula IsLiveDecisionBacklog (rulebook: =AND({{AwaitsAuthorityDecision}}, {{TouchesLiveVersion}}))
        public bool? IsLiveDecisionBacklog
        {
            get => AND(this.AwaitsAuthorityDecision, this.TouchesLiveVersion); set { }
        }

        // Formula BlocksAnOpenGap (rulebook: =AND({{IsLiveDecisionBacklog}}, {{ChangeKind}} = "Enhancement"))
        public bool? BlocksAnOpenGap
        {
            get => AND(this.IsLiveDecisionBacklog, this.ChangeKind = "Enhancement"); set { }
        }

        public DateTime? ImplementedAt { get; set; }
        // Formula BacklogVersionKey (rulebook: =IF({{IsLiveDecisionBacklog}}, {{ProcedureVersion}}, ""))
        public string? BacklogVersionKey
        {
            get => IF(this.IsLiveDecisionBacklog, this.ProcedureVersion, ""); set { }
        }

        // Formula IsMyPendingDecision (rulebook: =AND({{AuthorityRole}} = "hr-policy-owner", {{AwaitsAuthorityDecision}}))
        public bool? IsMyPendingDecision
        {
            get => AND(this.AuthorityRole = "hr-policy-owner", this.AwaitsAuthorityDecision); set { }
        }

        // Formula IsMyBlockingBacklog (rulebook: =AND({{IsMyPendingDecision}}, {{BlocksAnOpenGap}}))
        public bool? IsMyBlockingBacklog
        {
            get => AND(this.IsMyPendingDecision, this.BlocksAnOpenGap); set { }
        }

        // Formula IsMyOverdueBacklog (rulebook: =AND({{IsMyBlockingBacklog}}, {{DaysPending}} > 14))
        public bool? IsMyOverdueBacklog
        {
            get => AND(this.IsMyBlockingBacklog, this.DaysPending > 14); set { }
        }

        // Formula IsImplemented (rulebook: ={{ImplementedAt}} <> "")
        public bool? IsImplemented
        {
            get => this.ImplementedAt <> ""; set { }
        }

        // Formula IsMyDecidedRequest (rulebook: =AND({{AuthorityRole}} = "hr-policy-owner", {{IsDecided}}))
        public bool? IsMyDecidedRequest
        {
            get => AND(this.AuthorityRole = "hr-policy-owner", this.IsDecided); set { }
        }

        // Formula IsMyDecidedButUnlanded (rulebook: =AND({{IsMyDecidedRequest}}, NOT({{IsImplemented}})))
        public bool? IsMyDecidedButUnlanded
        {
            get => AND(this.IsMyDecidedRequest, NOT(this.IsImplemented)); set { }
        }

        // Formula DecisionLatencyDays (rulebook: =IF({{IsDecided}}, DATETIME_DIFF({{DecidedAt}}, {{RequestedAt}}, "days"), 0))
        public int? DecisionLatencyDays
        {
            get => IF(this.IsDecided, DATETIME_DIFF(this.DecidedAt, this.RequestedAt, "days"), 0); set { }
        }

        // Formula ImplementationLatencyDays (rulebook: =IF({{IsImplemented}}, DATETIME_DIFF({{ImplementedAt}}, {{DecidedAt}}, "days"), 0))
        public int? ImplementationLatencyDays
        {
            get => IF(this.IsImplemented, DATETIME_DIFF(this.ImplementedAt, this.DecidedAt, "days"), 0); set { }
        }

        // Formula DelayIsDownstreamOfMe (rulebook: =AND({{IsMyDecidedButUnlanded}}, {{DecisionLatencyDays}} <= 14))
        public bool? DelayIsDownstreamOfMe
        {
            get => AND(this.IsMyDecidedButUnlanded, this.DecisionLatencyDays <= 14); set { }
        }

        // Formula UnlandedVersionKey (rulebook: =IF({{IsMyDecidedButUnlanded}}, {{ProcedureVersion}}, ""))
        public string? UnlandedVersionKey
        {
            get => IF(this.IsMyDecidedButUnlanded, this.ProcedureVersion, ""); set { }
        }

        // Formula IsApprovedNotImplemented (rulebook: =AND({{Status}} = "Approved", NOT({{IsImplemented}})))
        public bool? IsApprovedNotImplemented
        {
            get => AND(this.Status = "Approved", NOT(this.IsImplemented)); set { }
        }

        // Formula DaysSinceApproval (rulebook: =IF({{IsDecided}}, DATETIME_DIFF({{AsOfInstant}}, {{DecidedAt}}, "days"), 0))
        public int? DaysSinceApproval
        {
            get => IF(this.IsDecided, DATETIME_DIFF(this.AsOfInstant, this.DecidedAt, "days"), 0); set { }
        }

        // Formula IsStalledImplementation (rulebook: =AND({{IsApprovedNotImplemented}}, {{DaysSinceApproval}} > 14))
        public bool? IsStalledImplementation
        {
            get => AND(this.IsApprovedNotImplemented, this.DaysSinceApproval > 14); set { }
        }

        // Formula StalledImplementationVersionKey (rulebook: =IF({{IsStalledImplementation}}, {{ProcedureVersion}}, ""))
        public string? StalledImplementationVersionKey
        {
            get => IF(this.IsStalledImplementation, this.ProcedureVersion, ""); set { }
        }

        // Formula ApprovedVersionKey (rulebook: =IF({{IsApprovedDecision}}, {{ProcedureVersion}}, ""))
        public string? ApprovedVersionKey
        {
            get => IF(this.IsApprovedDecision, this.ProcedureVersion, ""); set { }
        }

        // Formula IsApprovedDecision (rulebook: ={{Status}} = "Approved")
        public bool? IsApprovedDecision
        {
            get => this.Status = "Approved"; set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? RequestedByAgent { get; set; }
        public string? AuthorityRole { get; set; }
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

        private Agent _agent;

        [ForeignKey("RequestedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(RequestedByAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. RequestedByAgent: " + RequestedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(RequestedByAgent);
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
                    RequestedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private Role _role;

        [ForeignKey("AuthorityRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(AuthorityRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AuthorityRole: " + AuthorityRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(AuthorityRole);
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
                    AuthorityRole = _role == null ? default : _role.RoleId;
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

        private ObservableCollection<RoleAssignment> _roleAssignments;

        [InverseProperty("ChangeRequest")]
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
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. ChangeRequestId: " + this.ChangeRequestId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = Context.RoleAssignments.Where(x => x.AuthorizingChangeRequest == this.ChangeRequestId).ToList<RoleAssignment>();
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
                    item.AuthorizingChangeRequest = this.ChangeRequestId;
                }
            }
        }

        private ObservableCollection<ReviewEvent> _reviewEvents;

        [InverseProperty("ChangeRequest")]
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
                            throw new InvalidOperationException("Cannot access ReviewEvents - no database context is set. ChangeRequestId: " + this.ChangeRequestId + ".");
                        }
                        _reviewEvents = new ObservableCollection<ReviewEvent>();
                    }
                    else
                    {
                        var items = Context.ReviewEvents.Where(x => x.RelatedChangeRequest == this.ChangeRequestId).ToList<ReviewEvent>();
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
                    item.RelatedChangeRequest = this.ChangeRequestId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.Agent;
            _ = this.Role;
            _ = this.EvaluationContext;
            _ = this.RoleAssignments;
            _ = this.ReviewEvents;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
