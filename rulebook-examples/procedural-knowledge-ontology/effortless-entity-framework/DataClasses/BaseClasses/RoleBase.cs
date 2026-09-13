
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
    [Table("Roles")]
    public class RoleBase : SoAEntityBase
    {
        [Key]
        public string RoleId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        // Formula CurrentAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{CurrentAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? CurrentAgentKind
        {
            get => F.AsString(F.Memo(this, "CurrentAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.CurrentAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        public string? Responsibility { get; set; }
        // Formula ActiveAssignmentCount (rulebook: =COUNTIFS(RoleAssignments!{{Role}}, {{RoleId}}))
        [NotMapped]
        public decimal? ActiveAssignmentCount
        {
            get => F.AsDecimal(F.Memo(this, "ActiveAssignmentCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.Role), F.Of(this.RoleId)))))); set { }
        }

        // Formula CurrentlyCoveredAssignmentCount (rulebook: =COUNTIFS(RoleAssignments!{{RoleWhenCovering}}, {{RoleId}}))
        [NotMapped]
        public decimal? CurrentlyCoveredAssignmentCount
        {
            get => F.AsDecimal(F.Memo(this, "CurrentlyCoveredAssignmentCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.RoleWhenCovering), F.Of(this.RoleId)))))); set { }
        }

        // Formula HasNoCurrentHolder (rulebook: ={{CurrentlyCoveredAssignmentCount}} = 0)
        [NotMapped]
        public bool? HasNoCurrentHolder
        {
            get => F.AsBool(F.Memo(this, "HasNoCurrentHolder", () => F.Eq(F.Of(this.CurrentlyCoveredAssignmentCount), F.I(0)))); set { }
        }

        // Formula CountOfAwaitedDecisions (rulebook: =COUNTIFS(ChangeRequests!{{AuthorityRole}}, Roles!{{RoleId}}))
        [NotMapped]
        public int? CountOfAwaitedDecisions
        {
            get => F.AsInt(F.Memo(this, "CountOfAwaitedDecisions", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeRequest>(base.SoAContext, "ChangeRequests", __c => __c.ChangeRequests), __r => F.CritField(F.Of(__r.AuthorityRole), F.Of(this.RoleId))))))); set { }
        }

        public string? CurrentAssignment { get; set; }
        // Formula CurrentAssignmentValidFrom (rulebook: =INDEX(RoleAssignments!{{ValidFrom}}, MATCH({{CurrentAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public DateTimeOffset? CurrentAssignmentValidFrom
        {
            get => F.AsDateTime(F.Memo(this, "CurrentAssignmentValidFrom", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.CurrentAssignment), __r => F.Of(__r.ValidFrom), () => F.Of(new RoleAssignment().ValidFrom)))); set { }
        }

        // Formula IsNonHumanHeld (rulebook: =NOT({{CurrentAgentKind}} = "Human"))
        [NotMapped]
        public bool? IsNonHumanHeld
        {
            get => F.AsBool(F.Memo(this, "IsNonHumanHeld", () => F.Not(F.Bool3(F.Eq(F.Of(this.CurrentAgentKind), F.S("Human")))))); set { }
        }

        // Formula IsUngovernedNonHumanRole (rulebook: =AND({{IsNonHumanHeld}}, {{HasNoCurrentHolder}}))
        [NotMapped]
        public bool? IsUngovernedNonHumanRole
        {
            get => F.AsBool(F.Memo(this, "IsUngovernedNonHumanRole", () => F.And(F.Bool3(F.Of(this.IsNonHumanHeld)), F.Bool3(F.Of(this.HasNoCurrentHolder))))); set { }
        }

        // Formula DepartedAssignmentCount (rulebook: =COUNTIFS(RoleAssignments!{{DepartedRoleKey}}, {{RoleId}}))
        [NotMapped]
        public decimal? DepartedAssignmentCount
        {
            get => F.AsDecimal(F.Memo(this, "DepartedAssignmentCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.DepartedRoleKey), F.Of(this.RoleId)))))); set { }
        }

        // Formula HasLostAHolder (rulebook: ={{DepartedAssignmentCount}} > 0)
        [NotMapped]
        public bool? HasLostAHolder
        {
            get => F.AsBool(F.Memo(this, "HasLostAHolder", () => F.Cmp(F.Of(this.DepartedAssignmentCount), ">", F.I(0)))); set { }
        }

        // Formula IsVacatedRole (rulebook: =AND({{HasLostAHolder}}, {{HasNoCurrentHolder}}))
        [NotMapped]
        public bool? IsVacatedRole
        {
            get => F.AsBool(F.Memo(this, "IsVacatedRole", () => F.And(F.Bool3(F.Of(this.HasLostAHolder)), F.Bool3(F.Of(this.HasNoCurrentHolder))))); set { }
        }

        // Formula UngroundedBoundaryCount (rulebook: =COUNTIFS(AuthorityBoundaries!{{ConstrainedRoleAssignmentKey}}, {{RoleId}}))
        [NotMapped]
        public decimal? UngroundedBoundaryCount
        {
            get => F.AsDecimal(F.Memo(this, "UngroundedBoundaryCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AuthorityBoundary>(base.SoAContext, "AuthorityBoundaries", __c => __c.AuthorityBoundaries), __r => F.CritField(F.Of(__r.ConstrainedRoleAssignmentKey), F.Of(this.RoleId)))))); set { }
        }

        // Formula IsGovernedByLapsedAuthority (rulebook: =({{UngroundedBoundaryCount}} > 0))
        [NotMapped]
        public bool? IsGovernedByLapsedAuthority
        {
            get => F.AsBool(F.Memo(this, "IsGovernedByLapsedAuthority", () => F.Cmp(F.Of(this.UngroundedBoundaryCount), ">", F.I(0)))); set { }
        }

        // Formula UnescalatedRefusalCount (rulebook: =COUNTIFS(SendIntents!{{UnescalatedRefusalRoleKey}}, {{RoleId}}))
        [NotMapped]
        public decimal? UnescalatedRefusalCount
        {
            get => F.AsDecimal(F.Memo(this, "UnescalatedRefusalCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SendIntent>(base.SoAContext, "SendIntents", __c => __c.SendIntents), __r => F.CritField(F.Of(__r.UnescalatedRefusalRoleKey), F.Of(this.RoleId)))))); set { }
        }

        // Formula UnauthorizedEnforcementAssignmentCount (rulebook: =COUNTIFS(RoleAssignments!{{UnauthorizedEnforcementRoleKey}}, {{RoleId}}))
        [NotMapped]
        public decimal? UnauthorizedEnforcementAssignmentCount
        {
            get => F.AsDecimal(F.Memo(this, "UnauthorizedEnforcementAssignmentCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.UnauthorizedEnforcementRoleKey), F.Of(this.RoleId)))))); set { }
        }

        // Formula IsUngovernedEnforcementRole (rulebook: =({{UnauthorizedEnforcementAssignmentCount}} > 0))
        [NotMapped]
        public bool? IsUngovernedEnforcementRole
        {
            get => F.AsBool(F.Memo(this, "IsUngovernedEnforcementRole", () => F.Cmp(F.Of(this.UnauthorizedEnforcementAssignmentCount), ">", F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }
        public string? CurrentAgent { get; set; }

        private Organization _organizationRef;

        [ForeignKey("Organization")]
        public virtual Organization OrganizationRef
        {
            get
            {
                if (_organizationRef == null && !string.IsNullOrEmpty(Organization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRef - no database context is set. Organization: " + Organization + ".");
                        }
                        return null;
                    }
                    _organizationRef = base.SoAContext.Organizations.Find(Organization);
                    if (_organizationRef != null)
                    {
                        base.SoAContext.Attach(_organizationRef);
                    }
                }
                return _organizationRef;
            }
            set
            {
                if (_organizationRef != value)
                {
                    _organizationRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organizationRef != null)
                    {
                        Organization = _organizationRef.OrganizationId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("CurrentAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(CurrentAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. CurrentAgent: " + CurrentAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(CurrentAgent);
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
                        CurrentAgent = _agent.AgentId;
                    }
                }
            }
        }

        private ObservableCollection<RoleAssignment> _roleRoleAssignments;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<RoleAssignment> RoleRoleAssignments
        {
            get
            {
                if (_roleRoleAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleRoleAssignments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _roleRoleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignments.Where(x => x.Role == this.RoleId).ToList<RoleAssignment>();
                        _roleRoleAssignments = new ObservableCollection<RoleAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roleRoleAssignments.CollectionChanged += RoleRoleAssignments_CollectionChanged;
                }
                return _roleRoleAssignments;
            }
            private set
            {
                if (_roleRoleAssignments != null)
                {
                    _roleRoleAssignments.CollectionChanged -= RoleRoleAssignments_CollectionChanged;
                }
                _roleRoleAssignments = value;
                if (_roleRoleAssignments != null)
                {
                    _roleRoleAssignments.CollectionChanged += RoleRoleAssignments_CollectionChanged;
                }
            }
        }

        private void RoleRoleAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleAssignment>())
                {
                    item.Role = this.RoleId;
                }
            }
        }

        private ObservableCollection<RoleAssignment> _approvingAuthorityRoleRoleAssignments;

        [InverseProperty("RoleRefRef")]
        public virtual ObservableCollection<RoleAssignment> ApprovingAuthorityRoleRoleAssignments
        {
            get
            {
                if (_approvingAuthorityRoleRoleAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApprovingAuthorityRoleRoleAssignments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _approvingAuthorityRoleRoleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignments.Where(x => x.ApprovingAuthorityRole == this.RoleId).ToList<RoleAssignment>();
                        _approvingAuthorityRoleRoleAssignments = new ObservableCollection<RoleAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _approvingAuthorityRoleRoleAssignments.CollectionChanged += ApprovingAuthorityRoleRoleAssignments_CollectionChanged;
                }
                return _approvingAuthorityRoleRoleAssignments;
            }
            private set
            {
                if (_approvingAuthorityRoleRoleAssignments != null)
                {
                    _approvingAuthorityRoleRoleAssignments.CollectionChanged -= ApprovingAuthorityRoleRoleAssignments_CollectionChanged;
                }
                _approvingAuthorityRoleRoleAssignments = value;
                if (_approvingAuthorityRoleRoleAssignments != null)
                {
                    _approvingAuthorityRoleRoleAssignments.CollectionChanged += ApprovingAuthorityRoleRoleAssignments_CollectionChanged;
                }
            }
        }

        private void ApprovingAuthorityRoleRoleAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleAssignment>())
                {
                    item.ApprovingAuthorityRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<CommunitiesOfPractice> _communitiesOfPractice;

        [InverseProperty("Role")]
        public virtual ObservableCollection<CommunitiesOfPractice> CommunitiesOfPractice
        {
            get
            {
                if (_communitiesOfPractice == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunitiesOfPractice - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _communitiesOfPractice = new ObservableCollection<CommunitiesOfPractice>();
                    }
                    else
                    {
                        var items = base.SoAContext.CommunitiesOfPractice.Where(x => x.StewardRole == this.RoleId).ToList<CommunitiesOfPractice>();
                        _communitiesOfPractice = new ObservableCollection<CommunitiesOfPractice>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _communitiesOfPractice.CollectionChanged += CommunitiesOfPractice_CollectionChanged;
                }
                return _communitiesOfPractice;
            }
            private set
            {
                if (_communitiesOfPractice != null)
                {
                    _communitiesOfPractice.CollectionChanged -= CommunitiesOfPractice_CollectionChanged;
                }
                _communitiesOfPractice = value;
                if (_communitiesOfPractice != null)
                {
                    _communitiesOfPractice.CollectionChanged += CommunitiesOfPractice_CollectionChanged;
                }
            }
        }

        private void CommunitiesOfPractice_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CommunitiesOfPractice>())
                {
                    item.StewardRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<Step> _steps;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access Steps - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _steps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = base.SoAContext.Steps.Where(x => x.AssignedRole == this.RoleId).ToList<Step>();
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
                    item.AssignedRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<Requirement> _requirements;

        [InverseProperty("Role")]
        public virtual ObservableCollection<Requirement> Requirements
        {
            get
            {
                if (_requirements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Requirements - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _requirements = new ObservableCollection<Requirement>();
                    }
                    else
                    {
                        var items = base.SoAContext.Requirements.Where(x => x.AccountableRole == this.RoleId).ToList<Requirement>();
                        _requirements = new ObservableCollection<Requirement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _requirements.CollectionChanged += Requirements_CollectionChanged;
                }
                return _requirements;
            }
            private set
            {
                if (_requirements != null)
                {
                    _requirements.CollectionChanged -= Requirements_CollectionChanged;
                }
                _requirements = value;
                if (_requirements != null)
                {
                    _requirements.CollectionChanged += Requirements_CollectionChanged;
                }
            }
        }

        private void Requirements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Requirement>())
                {
                    item.AccountableRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<Rationale> _rationales;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access Rationales - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _rationales = new ObservableCollection<Rationale>();
                    }
                    else
                    {
                        var items = base.SoAContext.Rationales.Where(x => x.AuthorityRole == this.RoleId).ToList<Rationale>();
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
                    item.AuthorityRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<Exception> _approvalRoleExceptions;

        [InverseProperty("Role")]
        public virtual ObservableCollection<Exception> ApprovalRoleExceptions
        {
            get
            {
                if (_approvalRoleExceptions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApprovalRoleExceptions - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _approvalRoleExceptions = new ObservableCollection<Exception>();
                    }
                    else
                    {
                        var items = base.SoAContext.Exceptions.Where(x => x.ApprovalRole == this.RoleId).ToList<Exception>();
                        _approvalRoleExceptions = new ObservableCollection<Exception>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _approvalRoleExceptions.CollectionChanged += ApprovalRoleExceptions_CollectionChanged;
                }
                return _approvalRoleExceptions;
            }
            private set
            {
                if (_approvalRoleExceptions != null)
                {
                    _approvalRoleExceptions.CollectionChanged -= ApprovalRoleExceptions_CollectionChanged;
                }
                _approvalRoleExceptions = value;
                if (_approvalRoleExceptions != null)
                {
                    _approvalRoleExceptions.CollectionChanged += ApprovalRoleExceptions_CollectionChanged;
                }
            }
        }

        private void ApprovalRoleExceptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Exception>())
                {
                    item.ApprovalRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<Exception> _fallbackRoleExceptions;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<Exception> FallbackRoleExceptions
        {
            get
            {
                if (_fallbackRoleExceptions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FallbackRoleExceptions - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _fallbackRoleExceptions = new ObservableCollection<Exception>();
                    }
                    else
                    {
                        var items = base.SoAContext.Exceptions.Where(x => x.FallbackRole == this.RoleId).ToList<Exception>();
                        _fallbackRoleExceptions = new ObservableCollection<Exception>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fallbackRoleExceptions.CollectionChanged += FallbackRoleExceptions_CollectionChanged;
                }
                return _fallbackRoleExceptions;
            }
            private set
            {
                if (_fallbackRoleExceptions != null)
                {
                    _fallbackRoleExceptions.CollectionChanged -= FallbackRoleExceptions_CollectionChanged;
                }
                _fallbackRoleExceptions = value;
                if (_fallbackRoleExceptions != null)
                {
                    _fallbackRoleExceptions.CollectionChanged += FallbackRoleExceptions_CollectionChanged;
                }
            }
        }

        private void FallbackRoleExceptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Exception>())
                {
                    item.FallbackRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<KnowledgeFragment> _knowledgeFragments;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeFragments.Where(x => x.OwnerRole == this.RoleId).ToList<KnowledgeFragment>();
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
                    item.OwnerRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<KnowledgeGap> _knowledgeGaps;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeGaps - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeGaps.Where(x => x.OwnerRole == this.RoleId).ToList<KnowledgeGap>();
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
                    item.OwnerRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<StewardshipAssignment> _stewardRoleStewardshipAssignments;

        [InverseProperty("Role")]
        public virtual ObservableCollection<StewardshipAssignment> StewardRoleStewardshipAssignments
        {
            get
            {
                if (_stewardRoleStewardshipAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StewardRoleStewardshipAssignments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _stewardRoleStewardshipAssignments = new ObservableCollection<StewardshipAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.StewardshipAssignments.Where(x => x.StewardRole == this.RoleId).ToList<StewardshipAssignment>();
                        _stewardRoleStewardshipAssignments = new ObservableCollection<StewardshipAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stewardRoleStewardshipAssignments.CollectionChanged += StewardRoleStewardshipAssignments_CollectionChanged;
                }
                return _stewardRoleStewardshipAssignments;
            }
            private set
            {
                if (_stewardRoleStewardshipAssignments != null)
                {
                    _stewardRoleStewardshipAssignments.CollectionChanged -= StewardRoleStewardshipAssignments_CollectionChanged;
                }
                _stewardRoleStewardshipAssignments = value;
                if (_stewardRoleStewardshipAssignments != null)
                {
                    _stewardRoleStewardshipAssignments.CollectionChanged += StewardRoleStewardshipAssignments_CollectionChanged;
                }
            }
        }

        private void StewardRoleStewardshipAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StewardshipAssignment>())
                {
                    item.StewardRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<StewardshipAssignment> _authorityRoleStewardshipAssignments;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<StewardshipAssignment> AuthorityRoleStewardshipAssignments
        {
            get
            {
                if (_authorityRoleStewardshipAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorityRoleStewardshipAssignments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _authorityRoleStewardshipAssignments = new ObservableCollection<StewardshipAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.StewardshipAssignments.Where(x => x.AuthorityRole == this.RoleId).ToList<StewardshipAssignment>();
                        _authorityRoleStewardshipAssignments = new ObservableCollection<StewardshipAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _authorityRoleStewardshipAssignments.CollectionChanged += AuthorityRoleStewardshipAssignments_CollectionChanged;
                }
                return _authorityRoleStewardshipAssignments;
            }
            private set
            {
                if (_authorityRoleStewardshipAssignments != null)
                {
                    _authorityRoleStewardshipAssignments.CollectionChanged -= AuthorityRoleStewardshipAssignments_CollectionChanged;
                }
                _authorityRoleStewardshipAssignments = value;
                if (_authorityRoleStewardshipAssignments != null)
                {
                    _authorityRoleStewardshipAssignments.CollectionChanged += AuthorityRoleStewardshipAssignments_CollectionChanged;
                }
            }
        }

        private void AuthorityRoleStewardshipAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StewardshipAssignment>())
                {
                    item.AuthorityRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<ChangeRequest> _changeRequests;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access ChangeRequests - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _changeRequests = new ObservableCollection<ChangeRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeRequests.Where(x => x.AuthorityRole == this.RoleId).ToList<ChangeRequest>();
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
                    item.AuthorityRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<CommunicationPolicy> _communicationPolicies;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access CommunicationPolicies - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _communicationPolicies = new ObservableCollection<CommunicationPolicy>();
                    }
                    else
                    {
                        var items = base.SoAContext.CommunicationPolicies.Where(x => x.ApprovalRole == this.RoleId).ToList<CommunicationPolicy>();
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
                    item.ApprovalRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<RoleQuestion> _roleQuestions;

        [InverseProperty("Role")]
        public virtual ObservableCollection<RoleQuestion> RoleQuestions
        {
            get
            {
                if (_roleQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestions - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _roleQuestions = new ObservableCollection<RoleQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleQuestions.Where(x => x.AskingRole == this.RoleId).ToList<RoleQuestion>();
                        _roleQuestions = new ObservableCollection<RoleQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roleQuestions.CollectionChanged += RoleQuestions_CollectionChanged;
                }
                return _roleQuestions;
            }
            private set
            {
                if (_roleQuestions != null)
                {
                    _roleQuestions.CollectionChanged -= RoleQuestions_CollectionChanged;
                }
                _roleQuestions = value;
                if (_roleQuestions != null)
                {
                    _roleQuestions.CollectionChanged += RoleQuestions_CollectionChanged;
                }
            }
        }

        private void RoleQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleQuestion>())
                {
                    item.AskingRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<TemplateApproval> _templateApprovals;

        [InverseProperty("Role")]
        public virtual ObservableCollection<TemplateApproval> TemplateApprovals
        {
            get
            {
                if (_templateApprovals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TemplateApprovals - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _templateApprovals = new ObservableCollection<TemplateApproval>();
                    }
                    else
                    {
                        var items = base.SoAContext.TemplateApprovals.Where(x => x.DecidedInRole == this.RoleId).ToList<TemplateApproval>();
                        _templateApprovals = new ObservableCollection<TemplateApproval>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _templateApprovals.CollectionChanged += TemplateApprovals_CollectionChanged;
                }
                return _templateApprovals;
            }
            private set
            {
                if (_templateApprovals != null)
                {
                    _templateApprovals.CollectionChanged -= TemplateApprovals_CollectionChanged;
                }
                _templateApprovals = value;
                if (_templateApprovals != null)
                {
                    _templateApprovals.CollectionChanged += TemplateApprovals_CollectionChanged;
                }
            }
        }

        private void TemplateApprovals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TemplateApproval>())
                {
                    item.DecidedInRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<SendIntent> _sendIntents;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = base.SoAContext.SendIntents.Where(x => x.RefusalNotifiedRole == this.RoleId).ToList<SendIntent>();
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
                    item.RefusalNotifiedRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<AuthorityBoundary> _authorityBoundaries;

        [InverseProperty("Role")]
        public virtual ObservableCollection<AuthorityBoundary> AuthorityBoundaries
        {
            get
            {
                if (_authorityBoundaries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorityBoundaries - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>();
                    }
                    else
                    {
                        var items = base.SoAContext.AuthorityBoundaries.Where(x => x.AuthorityRole == this.RoleId).ToList<AuthorityBoundary>();
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.AuthorityRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<AppRoleProfile> _appRoleProfiles;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<AppRoleProfile> AppRoleProfiles
        {
            get
            {
                if (_appRoleProfiles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRoleProfiles - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _appRoleProfiles = new ObservableCollection<AppRoleProfile>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppRoleProfiles.Where(x => x.Role == this.RoleId).ToList<AppRoleProfile>();
                        _appRoleProfiles = new ObservableCollection<AppRoleProfile>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _appRoleProfiles.CollectionChanged += AppRoleProfiles_CollectionChanged;
                }
                return _appRoleProfiles;
            }
            private set
            {
                if (_appRoleProfiles != null)
                {
                    _appRoleProfiles.CollectionChanged -= AppRoleProfiles_CollectionChanged;
                }
                _appRoleProfiles = value;
                if (_appRoleProfiles != null)
                {
                    _appRoleProfiles.CollectionChanged += AppRoleProfiles_CollectionChanged;
                }
            }
        }

        private void AppRoleProfiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppRoleProfile>())
                {
                    item.Role = this.RoleId;
                }
            }
        }

        private ObservableCollection<AppRoute> _appRoutes;

        [InverseProperty("Role")]
        public virtual ObservableCollection<AppRoute> AppRoutes
        {
            get
            {
                if (_appRoutes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRoutes - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _appRoutes = new ObservableCollection<AppRoute>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppRoutes.Where(x => x.OwningRole == this.RoleId).ToList<AppRoute>();
                        _appRoutes = new ObservableCollection<AppRoute>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _appRoutes.CollectionChanged += AppRoutes_CollectionChanged;
                }
                return _appRoutes;
            }
            private set
            {
                if (_appRoutes != null)
                {
                    _appRoutes.CollectionChanged -= AppRoutes_CollectionChanged;
                }
                _appRoutes = value;
                if (_appRoutes != null)
                {
                    _appRoutes.CollectionChanged += AppRoutes_CollectionChanged;
                }
            }
        }

        private void AppRoutes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppRoute>())
                {
                    item.OwningRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<AccessPrincipal> _accessPrincipals;

        [InverseProperty("Role")]
        public virtual ObservableCollection<AccessPrincipal> AccessPrincipals
        {
            get
            {
                if (_accessPrincipals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPrincipals - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _accessPrincipals = new ObservableCollection<AccessPrincipal>();
                    }
                    else
                    {
                        var items = base.SoAContext.AccessPrincipals.Where(x => x.DomainRole == this.RoleId).ToList<AccessPrincipal>();
                        _accessPrincipals = new ObservableCollection<AccessPrincipal>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _accessPrincipals.CollectionChanged += AccessPrincipals_CollectionChanged;
                }
                return _accessPrincipals;
            }
            private set
            {
                if (_accessPrincipals != null)
                {
                    _accessPrincipals.CollectionChanged -= AccessPrincipals_CollectionChanged;
                }
                _accessPrincipals = value;
                if (_accessPrincipals != null)
                {
                    _accessPrincipals.CollectionChanged += AccessPrincipals_CollectionChanged;
                }
            }
        }

        private void AccessPrincipals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AccessPrincipal>())
                {
                    item.DomainRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<Vocabulary> _vocabularies;

        [InverseProperty("Role")]
        public virtual ObservableCollection<Vocabulary> Vocabularies
        {
            get
            {
                if (_vocabularies == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Vocabularies - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _vocabularies = new ObservableCollection<Vocabulary>();
                    }
                    else
                    {
                        var items = base.SoAContext.Vocabularies.Where(x => x.GoverningRole == this.RoleId).ToList<Vocabulary>();
                        _vocabularies = new ObservableCollection<Vocabulary>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _vocabularies.CollectionChanged += Vocabularies_CollectionChanged;
                }
                return _vocabularies;
            }
            private set
            {
                if (_vocabularies != null)
                {
                    _vocabularies.CollectionChanged -= Vocabularies_CollectionChanged;
                }
                _vocabularies = value;
                if (_vocabularies != null)
                {
                    _vocabularies.CollectionChanged += Vocabularies_CollectionChanged;
                }
            }
        }

        private void Vocabularies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Vocabulary>())
                {
                    item.GoverningRole = this.RoleId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.Agent;
            _ = this.RoleRoleAssignments;
            _ = this.ApprovingAuthorityRoleRoleAssignments;
            _ = this.CommunitiesOfPractice;
            _ = this.Steps;
            _ = this.Requirements;
            _ = this.Rationales;
            _ = this.ApprovalRoleExceptions;
            _ = this.FallbackRoleExceptions;
            _ = this.KnowledgeFragments;
            _ = this.KnowledgeGaps;
            _ = this.StewardRoleStewardshipAssignments;
            _ = this.AuthorityRoleStewardshipAssignments;
            _ = this.ChangeRequests;
            _ = this.CommunicationPolicies;
            _ = this.RoleQuestions;
            _ = this.TemplateApprovals;
            _ = this.SendIntents;
            _ = this.AuthorityBoundaries;
            _ = this.AppRoleProfiles;
            _ = this.AppRoutes;
            _ = this.AccessPrincipals;
            _ = this.Vocabularies;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
