
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Roles")]
    public class RoleBase : SoAEntityBase
    {
        [Key]
        public string RoleId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        public string? Name
        {
            get => this.Label; set { }
        }

        public string? Label { get; set; }
        // Formula CurrentAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{CurrentAgent}}, Agents!{{AgentId}}, 0)))
        public string? CurrentAgentKind
        {
            get => INDEX(Agents!this.AgentKind, MATCH(this.CurrentAgent, Agents!this.AgentId, 0)); set { }
        }

        public string? Responsibility { get; set; }
        // Formula ActiveAssignmentCount (rulebook: =COUNTIFS(RoleAssignments!{{Role}}, {{RoleId}}))
        public decimal? ActiveAssignmentCount
        {
            get => COUNTIFS(RoleAssignments!this.Role, this.RoleId); set { }
        }

        // Formula CurrentlyCoveredAssignmentCount (rulebook: =COUNTIFS(RoleAssignments!{{RoleWhenCovering}}, {{RoleId}}))
        public decimal? CurrentlyCoveredAssignmentCount
        {
            get => COUNTIFS(RoleAssignments!this.RoleWhenCovering, this.RoleId); set { }
        }

        // Formula HasNoCurrentHolder (rulebook: ={{CurrentlyCoveredAssignmentCount}} = 0)
        public bool? HasNoCurrentHolder
        {
            get => this.CurrentlyCoveredAssignmentCount = 0; set { }
        }

        // Formula CountOfAwaitedDecisions (rulebook: =COUNTIFS(ChangeRequests!{{AuthorityRole}}, Roles!{{RoleId}}))
        public int? CountOfAwaitedDecisions
        {
            get => this.ChangeRequests == null ? 0 : this.ChangeRequests.Count; set { }
        }

        public string? CurrentAssignment { get; set; }
        // Formula CurrentAssignmentValidFrom (rulebook: =INDEX(RoleAssignments!{{ValidFrom}}, MATCH({{CurrentAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        public DateTime? CurrentAssignmentValidFrom
        {
            get => INDEX(RoleAssignments!this.ValidFrom, MATCH(this.CurrentAssignment, RoleAssignments!this.RoleAssignmentId, 0)); set { }
        }

        // Formula IsNonHumanHeld (rulebook: =NOT({{CurrentAgentKind}} = "Human"))
        public bool? IsNonHumanHeld
        {
            get => NOT(this.CurrentAgentKind = "Human"); set { }
        }

        // Formula IsUngovernedNonHumanRole (rulebook: =AND({{IsNonHumanHeld}}, {{HasNoCurrentHolder}}))
        public bool? IsUngovernedNonHumanRole
        {
            get => AND(this.IsNonHumanHeld, this.HasNoCurrentHolder); set { }
        }

        // Formula DepartedAssignmentCount (rulebook: =COUNTIFS(RoleAssignments!{{DepartedRoleKey}}, {{RoleId}}))
        public decimal? DepartedAssignmentCount
        {
            get => COUNTIFS(RoleAssignments!this.DepartedRoleKey, this.RoleId); set { }
        }

        // Formula HasLostAHolder (rulebook: ={{DepartedAssignmentCount}} > 0)
        public bool? HasLostAHolder
        {
            get => this.DepartedAssignmentCount > 0; set { }
        }

        // Formula IsVacatedRole (rulebook: =AND({{HasLostAHolder}}, {{HasNoCurrentHolder}}))
        public bool? IsVacatedRole
        {
            get => AND(this.HasLostAHolder, this.HasNoCurrentHolder); set { }
        }

        // Formula UngroundedBoundaryCount (rulebook: =COUNTIFS(AuthorityBoundaries!{{ConstrainedRoleAssignmentKey}}, {{RoleId}}))
        public decimal? UngroundedBoundaryCount
        {
            get => COUNTIFS(AuthorityBoundaries!this.ConstrainedRoleAssignmentKey, this.RoleId); set { }
        }

        // Formula IsGovernedByLapsedAuthority (rulebook: =({{UngroundedBoundaryCount}} > 0))
        public bool? IsGovernedByLapsedAuthority
        {
            get => (this.UngroundedBoundaryCount > 0); set { }
        }

        // Formula UnescalatedRefusalCount (rulebook: =COUNTIFS(SendIntents!{{UnescalatedRefusalRoleKey}}, {{RoleId}}))
        public decimal? UnescalatedRefusalCount
        {
            get => COUNTIFS(SendIntents!this.UnescalatedRefusalRoleKey, this.RoleId); set { }
        }

        // Formula UnauthorizedEnforcementAssignmentCount (rulebook: =COUNTIFS(RoleAssignments!{{UnauthorizedEnforcementRoleKey}}, {{RoleId}}))
        public decimal? UnauthorizedEnforcementAssignmentCount
        {
            get => COUNTIFS(RoleAssignments!this.UnauthorizedEnforcementRoleKey, this.RoleId); set { }
        }

        // Formula IsUngovernedEnforcementRole (rulebook: =({{UnauthorizedEnforcementAssignmentCount}} > 0))
        public bool? IsUngovernedEnforcementRole
        {
            get => (this.UnauthorizedEnforcementAssignmentCount > 0); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }
        public string? CurrentAgent { get; set; }

        private Organization _organization;

        [ForeignKey("Organization")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(Organization))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. Organization: " + Organization + ".");
                        }
                        return null;
                    }
                    _organization = Context.Organizations.Find(Organization);
                    if (_organization != null)
                    {
                        Context.Attach(_organization);
                    }
                }
                return _organization;
            }
            set
            {
                if (_organization != value)
                {
                    _organization = value;
                    Organization = _organization == null ? default : _organization.OrganizationId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. CurrentAgent: " + CurrentAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(CurrentAgent);
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
                    CurrentAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private ObservableCollection<RoleAssignment> _roleAssignments;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = Context.RoleAssignments.Where(x => x.Role == this.RoleId).ToList<RoleAssignment>();
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
                    item.Role = this.RoleId;
                }
            }
        }

        private ObservableCollection<RoleAssignment> _roleAssignments;

        [InverseProperty("ApprovingAuthorityRole")]
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
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = Context.RoleAssignments.Where(x => x.ApprovingAuthorityRole == this.RoleId).ToList<RoleAssignment>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunitiesOfPractice - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _communitiesOfPractice = new ObservableCollection<CommunitiesOfPractice>();
                    }
                    else
                    {
                        var items = Context.CommunitiesOfPractice.Where(x => x.StewardRole == this.RoleId).ToList<CommunitiesOfPractice>();
                        _communitiesOfPractice = new ObservableCollection<CommunitiesOfPractice>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Steps - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _steps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = Context.Steps.Where(x => x.AssignedRole == this.RoleId).ToList<Step>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Requirements - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _requirements = new ObservableCollection<Requirement>();
                    }
                    else
                    {
                        var items = Context.Requirements.Where(x => x.AccountableRole == this.RoleId).ToList<Requirement>();
                        _requirements = new ObservableCollection<Requirement>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Rationales - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _rationales = new ObservableCollection<Rationale>();
                    }
                    else
                    {
                        var items = Context.Rationales.Where(x => x.AuthorityRole == this.RoleId).ToList<Rationale>();
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
                    item.AuthorityRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<Exception> _exceptions;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access Exceptions - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _exceptions = new ObservableCollection<Exception>();
                    }
                    else
                    {
                        var items = Context.Exceptions.Where(x => x.ApprovalRole == this.RoleId).ToList<Exception>();
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
                    item.ApprovalRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<Exception> _exceptions;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access Exceptions - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _exceptions = new ObservableCollection<Exception>();
                    }
                    else
                    {
                        var items = Context.Exceptions.Where(x => x.FallbackRole == this.RoleId).ToList<Exception>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = Context.KnowledgeFragments.Where(x => x.OwnerRole == this.RoleId).ToList<KnowledgeFragment>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeGaps - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>();
                    }
                    else
                    {
                        var items = Context.KnowledgeGaps.Where(x => x.OwnerRole == this.RoleId).ToList<KnowledgeGap>();
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
                    item.OwnerRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<StewardshipAssignment> _stewardshipAssignments;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access StewardshipAssignments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _stewardshipAssignments = new ObservableCollection<StewardshipAssignment>();
                    }
                    else
                    {
                        var items = Context.StewardshipAssignments.Where(x => x.StewardRole == this.RoleId).ToList<StewardshipAssignment>();
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
                    item.StewardRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<StewardshipAssignment> _stewardshipAssignments;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access StewardshipAssignments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _stewardshipAssignments = new ObservableCollection<StewardshipAssignment>();
                    }
                    else
                    {
                        var items = Context.StewardshipAssignments.Where(x => x.AuthorityRole == this.RoleId).ToList<StewardshipAssignment>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeRequests - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _changeRequests = new ObservableCollection<ChangeRequest>();
                    }
                    else
                    {
                        var items = Context.ChangeRequests.Where(x => x.AuthorityRole == this.RoleId).ToList<ChangeRequest>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunicationPolicies - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _communicationPolicies = new ObservableCollection<CommunicationPolicy>();
                    }
                    else
                    {
                        var items = Context.CommunicationPolicies.Where(x => x.ApprovalRole == this.RoleId).ToList<CommunicationPolicy>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestions - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _roleQuestions = new ObservableCollection<RoleQuestion>();
                    }
                    else
                    {
                        var items = Context.RoleQuestions.Where(x => x.AskingRole == this.RoleId).ToList<RoleQuestion>();
                        _roleQuestions = new ObservableCollection<RoleQuestion>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TemplateApprovals - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _templateApprovals = new ObservableCollection<TemplateApproval>();
                    }
                    else
                    {
                        var items = Context.TemplateApprovals.Where(x => x.DecidedInRole == this.RoleId).ToList<TemplateApproval>();
                        _templateApprovals = new ObservableCollection<TemplateApproval>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = Context.SendIntents.Where(x => x.RefusalNotifiedRole == this.RoleId).ToList<SendIntent>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorityBoundaries - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>();
                    }
                    else
                    {
                        var items = Context.AuthorityBoundaries.Where(x => x.AuthorityRole == this.RoleId).ToList<AuthorityBoundary>();
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
                    item.AuthorityRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<AppRoleProfile> _appRoleProfiles;

        [InverseProperty("Role")]
        public virtual ObservableCollection<AppRoleProfile> AppRoleProfiles
        {
            get
            {
                if (_appRoleProfiles == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRoleProfiles - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _appRoleProfiles = new ObservableCollection<AppRoleProfile>();
                    }
                    else
                    {
                        var items = Context.AppRoleProfiles.Where(x => x.Role == this.RoleId).ToList<AppRoleProfile>();
                        _appRoleProfiles = new ObservableCollection<AppRoleProfile>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRoutes - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _appRoutes = new ObservableCollection<AppRoute>();
                    }
                    else
                    {
                        var items = Context.AppRoutes.Where(x => x.OwningRole == this.RoleId).ToList<AppRoute>();
                        _appRoutes = new ObservableCollection<AppRoute>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPrincipals - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _accessPrincipals = new ObservableCollection<AccessPrincipal>();
                    }
                    else
                    {
                        var items = Context.AccessPrincipals.Where(x => x.DomainRole == this.RoleId).ToList<AccessPrincipal>();
                        _accessPrincipals = new ObservableCollection<AccessPrincipal>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Vocabularies - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _vocabularies = new ObservableCollection<Vocabulary>();
                    }
                    else
                    {
                        var items = Context.Vocabularies.Where(x => x.GoverningRole == this.RoleId).ToList<Vocabulary>();
                        _vocabularies = new ObservableCollection<Vocabulary>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.Organization;
            _ = this.Agent;
            _ = this.RoleAssignments;
            _ = this.RoleAssignments;
            _ = this.CommunitiesOfPractice;
            _ = this.Steps;
            _ = this.Requirements;
            _ = this.Rationales;
            _ = this.Exceptions;
            _ = this.Exceptions;
            _ = this.KnowledgeFragments;
            _ = this.KnowledgeGaps;
            _ = this.StewardshipAssignments;
            _ = this.StewardshipAssignments;
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
