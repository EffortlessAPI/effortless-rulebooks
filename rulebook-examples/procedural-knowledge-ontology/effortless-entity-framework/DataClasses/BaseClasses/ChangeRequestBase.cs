
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
    [Table("ChangeRequests")]
    public class ChangeRequestBase : SoAEntityBase
    {
        [Key]
        public string ChangeRequestId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Title))); set { }
        }

        public string? Title { get; set; }
        public string? ChangeKind { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset? RequestedAt { get; set; }
        public DateTimeOffset? DecidedAt { get; set; }
        public string? ImpactAssessment { get; set; }
        // Formula IsOpen (rulebook: =AND(OR({{Status}} = "Draft", {{Status}} = "UnderReview", {{Status}} = "Approved"), {{ImplementedAt}} = ""))
        [NotMapped]
        public bool? IsOpen
        {
            get => F.AsBool(F.Memo(this, "IsOpen", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Draft"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("UnderReview"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved"))))), F.Bool3(F.IsBlank(F.Of(this.ImplementedAt)))))); set { }
        }

        // Formula OpenChangeVersionKey (rulebook: =IF({{IsOpen}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? OpenChangeVersionKey
        {
            get => F.AsString(F.Memo(this, "OpenChangeVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsOpen))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula IsDecided (rulebook: ={{DecidedAt}} <> "")
        [NotMapped]
        public bool? IsDecided
        {
            get => F.AsBool(F.Memo(this, "IsDecided", () => F.IsNotBlank(F.Of(this.DecidedAt)))); set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula DaysPending (rulebook: =IF({{IsDecided}}, DATETIME_DIFF({{DecidedAt}}, {{RequestedAt}}, "days"), DATETIME_DIFF({{AsOfInstant}}, {{RequestedAt}}, "days")))
        [NotMapped]
        public int? DaysPending
        {
            get => F.AsInt(F.Memo(this, "DaysPending", () => F.Integer((F.Truthy(F.Bool3(F.Of(this.IsDecided))) ? F.DatetimeDiff(F.Of(this.DecidedAt), F.Of(this.RequestedAt), F.S("days")) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.RequestedAt), F.S("days")))))); set { }
        }

        // Formula IsStillPending (rulebook: =AND({{IsOpen}}, NOT({{IsDecided}})))
        [NotMapped]
        public bool? IsStillPending
        {
            get => F.AsBool(F.Memo(this, "IsStillPending", () => F.And(F.Bool3(F.Of(this.IsOpen)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsDecided))))))); set { }
        }

        // Formula IsStalled (rulebook: =AND({{IsStillPending}}, {{DaysPending}} > 14))
        [NotMapped]
        public bool? IsStalled
        {
            get => F.AsBool(F.Memo(this, "IsStalled", () => F.And(F.Bool3(F.Of(this.IsStillPending)), F.Bool3(F.Cmp(F.Of(this.DaysPending), ">", F.I(14)))))); set { }
        }

        // Formula AuthorityAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{AuthorityRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? AuthorityAgent
        {
            get => F.AsString(F.Memo(this, "AuthorityAgent", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AuthorityRole), __r => F.Of(__r.CurrentAgent), () => F.Of(new Role().CurrentAgent)))); set { }
        }

        // Formula RequesterIsAuthority (rulebook: ={{RequestedByAgent}} = {{AuthorityAgent}})
        [NotMapped]
        public bool? RequesterIsAuthority
        {
            get => F.AsBool(F.Memo(this, "RequesterIsAuthority", () => F.Eq(F.Nullif(F.Of(this.RequestedByAgent)), F.Of(this.AuthorityAgent)))); set { }
        }

        // Formula AwaitsAuthorityDecision (rulebook: =AND({{Status}} = "UnderReview", NOT({{IsDecided}})))
        [NotMapped]
        public bool? AwaitsAuthorityDecision
        {
            get => F.AsBool(F.Memo(this, "AwaitsAuthorityDecision", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("UnderReview"))), F.Bool3(F.Not(F.Bool3(F.Of(this.IsDecided))))))); set { }
        }

        // Formula AuthorityRoleLabel (rulebook: =INDEX(Roles!{{Label}}, MATCH({{AuthorityRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? AuthorityRoleLabel
        {
            get => F.AsString(F.Memo(this, "AuthorityRoleLabel", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AuthorityRole), __r => F.Of(__r.Label), () => F.Of(new Role().Label)))); set { }
        }

        // Formula TouchesLiveVersion (rulebook: =INDEX(ProcedureVersions!{{IsLive}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public bool? TouchesLiveVersion
        {
            get => F.AsBool(F.Memo(this, "TouchesLiveVersion", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.IsLive), () => F.Of(new ProcedureVersion().IsLive)))); set { }
        }

        // Formula IsLiveDecisionBacklog (rulebook: =AND({{AwaitsAuthorityDecision}}, {{TouchesLiveVersion}}))
        [NotMapped]
        public bool? IsLiveDecisionBacklog
        {
            get => F.AsBool(F.Memo(this, "IsLiveDecisionBacklog", () => F.And(F.Bool3(F.Of(this.AwaitsAuthorityDecision)), F.Bool3(F.Of(this.TouchesLiveVersion))))); set { }
        }

        // Formula BlocksAnOpenGap (rulebook: =AND({{IsLiveDecisionBacklog}}, {{ChangeKind}} = "Enhancement"))
        [NotMapped]
        public bool? BlocksAnOpenGap
        {
            get => F.AsBool(F.Memo(this, "BlocksAnOpenGap", () => F.And(F.Bool3(F.Of(this.IsLiveDecisionBacklog)), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeKind)), F.S("Enhancement")))))); set { }
        }

        public DateTimeOffset? ImplementedAt { get; set; }
        // Formula BacklogVersionKey (rulebook: =IF({{IsLiveDecisionBacklog}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? BacklogVersionKey
        {
            get => F.AsString(F.Memo(this, "BacklogVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsLiveDecisionBacklog))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula IsMyPendingDecision (rulebook: =AND({{AuthorityRole}} = "hr-policy-owner", {{AwaitsAuthorityDecision}}))
        [NotMapped]
        public bool? IsMyPendingDecision
        {
            get => F.AsBool(F.Memo(this, "IsMyPendingDecision", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AuthorityRole)), F.S("hr-policy-owner"))), F.Bool3(F.Of(this.AwaitsAuthorityDecision))))); set { }
        }

        // Formula IsMyBlockingBacklog (rulebook: =AND({{IsMyPendingDecision}}, {{BlocksAnOpenGap}}))
        [NotMapped]
        public bool? IsMyBlockingBacklog
        {
            get => F.AsBool(F.Memo(this, "IsMyBlockingBacklog", () => F.And(F.Bool3(F.Of(this.IsMyPendingDecision)), F.Bool3(F.Of(this.BlocksAnOpenGap))))); set { }
        }

        // Formula IsMyOverdueBacklog (rulebook: =AND({{IsMyBlockingBacklog}}, {{DaysPending}} > 14))
        [NotMapped]
        public bool? IsMyOverdueBacklog
        {
            get => F.AsBool(F.Memo(this, "IsMyOverdueBacklog", () => F.And(F.Bool3(F.Of(this.IsMyBlockingBacklog)), F.Bool3(F.Cmp(F.Of(this.DaysPending), ">", F.I(14)))))); set { }
        }

        // Formula IsImplemented (rulebook: ={{ImplementedAt}} <> "")
        [NotMapped]
        public bool? IsImplemented
        {
            get => F.AsBool(F.Memo(this, "IsImplemented", () => F.IsNotBlank(F.Of(this.ImplementedAt)))); set { }
        }

        // Formula IsMyDecidedRequest (rulebook: =AND({{AuthorityRole}} = "hr-policy-owner", {{IsDecided}}))
        [NotMapped]
        public bool? IsMyDecidedRequest
        {
            get => F.AsBool(F.Memo(this, "IsMyDecidedRequest", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AuthorityRole)), F.S("hr-policy-owner"))), F.Bool3(F.Of(this.IsDecided))))); set { }
        }

        // Formula IsMyDecidedButUnlanded (rulebook: =AND({{IsMyDecidedRequest}}, NOT({{IsImplemented}})))
        [NotMapped]
        public bool? IsMyDecidedButUnlanded
        {
            get => F.AsBool(F.Memo(this, "IsMyDecidedButUnlanded", () => F.And(F.Bool3(F.Of(this.IsMyDecidedRequest)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsImplemented))))))); set { }
        }

        // Formula DecisionLatencyDays (rulebook: =IF({{IsDecided}}, DATETIME_DIFF({{DecidedAt}}, {{RequestedAt}}, "days"), 0))
        [NotMapped]
        public int? DecisionLatencyDays
        {
            get => F.AsInt(F.Memo(this, "DecisionLatencyDays", () => F.Integer((F.Truthy(F.Bool3(F.Of(this.IsDecided))) ? F.DatetimeDiff(F.Of(this.DecidedAt), F.Of(this.RequestedAt), F.S("days")) : F.I(0))))); set { }
        }

        // Formula ImplementationLatencyDays (rulebook: =IF({{IsImplemented}}, DATETIME_DIFF({{ImplementedAt}}, {{DecidedAt}}, "days"), 0))
        [NotMapped]
        public int? ImplementationLatencyDays
        {
            get => F.AsInt(F.Memo(this, "ImplementationLatencyDays", () => F.Integer((F.Truthy(F.Bool3(F.Of(this.IsImplemented))) ? F.DatetimeDiff(F.Of(this.ImplementedAt), F.Of(this.DecidedAt), F.S("days")) : F.I(0))))); set { }
        }

        // Formula DelayIsDownstreamOfMe (rulebook: =AND({{IsMyDecidedButUnlanded}}, {{DecisionLatencyDays}} <= 14))
        [NotMapped]
        public bool? DelayIsDownstreamOfMe
        {
            get => F.AsBool(F.Memo(this, "DelayIsDownstreamOfMe", () => F.And(F.Bool3(F.Of(this.IsMyDecidedButUnlanded)), F.Bool3(F.Cmp(F.Of(this.DecisionLatencyDays), "<=", F.I(14)))))); set { }
        }

        // Formula UnlandedVersionKey (rulebook: =IF({{IsMyDecidedButUnlanded}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? UnlandedVersionKey
        {
            get => F.AsString(F.Memo(this, "UnlandedVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsMyDecidedButUnlanded))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula IsApprovedNotImplemented (rulebook: =AND({{Status}} = "Approved", NOT({{IsImplemented}})))
        [NotMapped]
        public bool? IsApprovedNotImplemented
        {
            get => F.AsBool(F.Memo(this, "IsApprovedNotImplemented", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved"))), F.Bool3(F.Not(F.Bool3(F.Of(this.IsImplemented))))))); set { }
        }

        // Formula DaysSinceApproval (rulebook: =IF({{IsDecided}}, DATETIME_DIFF({{AsOfInstant}}, {{DecidedAt}}, "days"), 0))
        [NotMapped]
        public int? DaysSinceApproval
        {
            get => F.AsInt(F.Memo(this, "DaysSinceApproval", () => F.Integer((F.Truthy(F.Bool3(F.Of(this.IsDecided))) ? F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.DecidedAt), F.S("days")) : F.I(0))))); set { }
        }

        // Formula IsStalledImplementation (rulebook: =AND({{IsApprovedNotImplemented}}, {{DaysSinceApproval}} > 14))
        [NotMapped]
        public bool? IsStalledImplementation
        {
            get => F.AsBool(F.Memo(this, "IsStalledImplementation", () => F.And(F.Bool3(F.Of(this.IsApprovedNotImplemented)), F.Bool3(F.Cmp(F.Of(this.DaysSinceApproval), ">", F.I(14)))))); set { }
        }

        // Formula StalledImplementationVersionKey (rulebook: =IF({{IsStalledImplementation}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? StalledImplementationVersionKey
        {
            get => F.AsString(F.Memo(this, "StalledImplementationVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsStalledImplementation))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula ApprovedVersionKey (rulebook: =IF({{IsApprovedDecision}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? ApprovedVersionKey
        {
            get => F.AsString(F.Memo(this, "ApprovedVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsApprovedDecision))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula IsApprovedDecision (rulebook: ={{Status}} = "Approved")
        [NotMapped]
        public bool? IsApprovedDecision
        {
            get => F.AsBool(F.Memo(this, "IsApprovedDecision", () => F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? RequestedByAgent { get; set; }
        public string? AuthorityRole { get; set; }
        public string? EvaluationContext { get; set; }

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

        [ForeignKey("RequestedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(RequestedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. RequestedByAgent: " + RequestedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(RequestedByAgent);
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
                        RequestedByAgent = _agent.AgentId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AuthorityRole: " + AuthorityRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(AuthorityRole);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        AuthorityRole = _role.RoleId;
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

        [InverseProperty("ChangeRequest")]
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
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. ChangeRequestId: " + this.ChangeRequestId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignments.Where(x => x.AuthorizingChangeRequest == this.ChangeRequestId).ToList<RoleAssignment>();
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
                    item.AuthorizingChangeRequest = this.ChangeRequestId;
                }
            }
        }

        private ObservableCollection<IssueOccurrence> _issueOccurrences;

        [InverseProperty("ChangeRequest")]
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
                            throw new InvalidOperationException("Cannot access IssueOccurrences - no database context is set. ChangeRequestId: " + this.ChangeRequestId + ".");
                        }
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>();
                    }
                    else
                    {
                        var items = base.SoAContext.IssueOccurrences.Where(x => x.RedesignChangeRequest == this.ChangeRequestId).ToList<IssueOccurrence>();
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
                    item.RedesignChangeRequest = this.ChangeRequestId;
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ReviewEvents - no database context is set. ChangeRequestId: " + this.ChangeRequestId + ".");
                        }
                        _reviewEvents = new ObservableCollection<ReviewEvent>();
                    }
                    else
                    {
                        var items = base.SoAContext.ReviewEvents.Where(x => x.RelatedChangeRequest == this.ChangeRequestId).ToList<ReviewEvent>();
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
                    item.RelatedChangeRequest = this.ChangeRequestId;
                }
            }
        }

        private ObservableCollection<KnowledgeOutcomeMeasurement> _knowledgeOutcomeMeasurements;

        [InverseProperty("ChangeRequest")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeOutcomeMeasurements - no database context is set. ChangeRequestId: " + this.ChangeRequestId + ".");
                        }
                        _knowledgeOutcomeMeasurements = new ObservableCollection<KnowledgeOutcomeMeasurement>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeOutcomeMeasurements.Where(x => x.InformedChangeRequest == this.ChangeRequestId).ToList<KnowledgeOutcomeMeasurement>();
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
                    item.InformedChangeRequest = this.ChangeRequestId;
                }
            }
        }

        private ObservableCollection<AiInsightProposal> _aiInsightProposals;

        [InverseProperty("ChangeRequest")]
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
                            throw new InvalidOperationException("Cannot access AiInsightProposals - no database context is set. ChangeRequestId: " + this.ChangeRequestId + ".");
                        }
                        _aiInsightProposals = new ObservableCollection<AiInsightProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiInsightProposals.Where(x => x.FoldedIntoChangeRequest == this.ChangeRequestId).ToList<AiInsightProposal>();
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
                    item.FoldedIntoChangeRequest = this.ChangeRequestId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.Agent;
            _ = this.Role;
            _ = this.EvaluationContextRef;
            _ = this.RoleAssignments;
            _ = this.IssueOccurrences;
            _ = this.ReviewEvents;
            _ = this.KnowledgeOutcomeMeasurements;
            _ = this.AiInsightProposals;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
