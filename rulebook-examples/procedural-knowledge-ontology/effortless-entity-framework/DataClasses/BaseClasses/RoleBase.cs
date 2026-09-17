
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
        public string? SeniorityLevel { get; set; }
        public string? RoleFamily { get; set; }
        // Formula SpecializedRoleFamily (rulebook: =INDEX(Roles!{{RoleFamily}}, MATCH({{SpecializesRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? SpecializedRoleFamily
        {
            get => F.AsString(F.Memo(this, "SpecializedRoleFamily", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.SpecializesRole), __r => F.Of(__r.RoleFamily), () => F.Of(new Role().RoleFamily)))); set { }
        }

        // Formula SpecializationCount (rulebook: =COUNTIFS(Roles!{{SpecializesRole}}, Roles!{{RoleId}}))
        [NotMapped]
        public int? SpecializationCount
        {
            get => F.AsInt(F.Memo(this, "SpecializationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Role>(base.SoAContext, "Roles", __c => __c.Roles), __r => F.CritField(F.Of(__r.SpecializesRole), F.Of(this.RoleId))))))); set { }
        }

        // Formula HasSpecializations (rulebook: ={{SpecializationCount}} > 0)
        [NotMapped]
        public bool? HasSpecializations
        {
            get => F.AsBool(F.Memo(this, "HasSpecializations", () => F.Cmp(F.Of(this.SpecializationCount), ">", F.I(0)))); set { }
        }

        // Formula IsSeniorVariantNotSpecialization (rulebook: =AND({{SeniorityLevel}} = "Senior", OR({{SpecializesRole}} = "", {{SpecializedRoleFamily}} <> {{RoleFamily}})))
        [NotMapped]
        public bool? IsSeniorVariantNotSpecialization
        {
            get => F.AsBool(F.Memo(this, "IsSeniorVariantNotSpecialization", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.SeniorityLevel)), F.S("Senior"))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.SpecializesRole))), F.Bool3(F.Ne(F.Of(this.SpecializedRoleFamily), F.Nullif(F.Of(this.RoleFamily))))))))); set { }
        }

        // Formula OrganizationType (rulebook: =INDEX(Organizations!{{OrganizationType}}, MATCH({{Organization}}, Organizations!{{OrganizationId}}, 0)))
        [NotMapped]
        public string? OrganizationType
        {
            get => F.AsString(F.Memo(this, "OrganizationType", () => F.Lookup<Organization>(this, "Organizations", "OrganizationId", __c => __c.Organizations, __r => F.Of(__r.OrganizationId), F.Of(this.Organization), __r => F.Of(__r.OrganizationType), () => F.Of(new Organization().OrganizationType)))); set { }
        }

        // Formula IsNotHousedInDepartment (rulebook: ={{OrganizationType}} <> "Department")
        [NotMapped]
        public bool? IsNotHousedInDepartment
        {
            get => F.AsBool(F.Memo(this, "IsNotHousedInDepartment", () => F.Ne(F.Of(this.OrganizationType), F.S("Department")))); set { }
        }

        // Formula CapabilityTagCount (rulebook: =COUNTIFS(RoleCapabilityTags!{{Role}}, {{RoleId}}))
        [NotMapped]
        public int? CapabilityTagCount
        {
            get => F.AsInt(F.Memo(this, "CapabilityTagCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleCapabilityTag>(base.SoAContext, "RoleCapabilityTags", __c => __c.RoleCapabilityTags), __r => F.CritField(F.Of(__r.Role), F.Of(this.RoleId))))))); set { }
        }

        // Formula ComplianceReviewTagCount (rulebook: =COUNTIFS(RoleCapabilityTags!{{Role}}, {{RoleId}}, RoleCapabilityTags!{{CapabilityTerm}}, "vt-cap-compliance-review"))
        [NotMapped]
        public int? ComplianceReviewTagCount
        {
            get => F.AsInt(F.Memo(this, "ComplianceReviewTagCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleCapabilityTag>(base.SoAContext, "RoleCapabilityTags", __c => __c.RoleCapabilityTags), __r => F.CritField(F.Of(__r.Role), F.Of(this.RoleId)) && F.CritLiteral(F.Of(__r.CapabilityTerm), F.S("vt-cap-compliance-review"))))))); set { }
        }

        // Formula HasComplianceReviewCapability (rulebook: ={{ComplianceReviewTagCount}} > 0)
        [NotMapped]
        public bool? HasComplianceReviewCapability
        {
            get => F.AsBool(F.Memo(this, "HasComplianceReviewCapability", () => F.Cmp(F.Of(this.ComplianceReviewTagCount), ">", F.I(0)))); set { }
        }

        // Formula RoleMentionCount (rulebook: =COUNTIFS(SourceTermMentions!{{IntendedTermRole}}, {{RoleId}}))
        [NotMapped]
        public int? RoleMentionCount
        {
            get => F.AsInt(F.Memo(this, "RoleMentionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SourceTermMention>(base.SoAContext, "SourceTermMentions", __c => __c.SourceTermMentions), __r => F.CritField(F.Of(__r.IntendedTermRole), F.Of(this.RoleId))))))); set { }
        }

        // Formula UnresolvedRoleMentionCount (rulebook: =COUNTIFS(SourceTermMentions!{{UnresolvedRoleKey}}, {{RoleId}}))
        [NotMapped]
        public int? UnresolvedRoleMentionCount
        {
            get => F.AsInt(F.Memo(this, "UnresolvedRoleMentionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SourceTermMention>(base.SoAContext, "SourceTermMentions", __c => __c.SourceTermMentions), __r => F.CritField(F.Of(__r.UnresolvedRoleKey), F.Of(this.RoleId))))))); set { }
        }

        // Formula IsMissedByPhraseQuery (rulebook: ={{UnresolvedRoleMentionCount}} > 0)
        [NotMapped]
        public bool? IsMissedByPhraseQuery
        {
            get => F.AsBool(F.Memo(this, "IsMissedByPhraseQuery", () => F.Cmp(F.Of(this.UnresolvedRoleMentionCount), ">", F.I(0)))); set { }
        }

        public string? PreferredKnowledgeForm { get; set; }
        // Formula CurrentHolderName (rulebook: =INDEX(Agents!{{DisplayName}}, MATCH({{CurrentAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? CurrentHolderName
        {
            get => F.AsString(F.Memo(this, "CurrentHolderName", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.CurrentAgent), __r => F.Of(__r.DisplayName), () => F.Of(new Agent().DisplayName)))); set { }
        }

        // Formula BackupRoleHolder (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{EscalationBackupRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? BackupRoleHolder
        {
            get => F.AsString(F.Memo(this, "BackupRoleHolder", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.EscalationBackupRole), __r => F.Of(__r.CurrentAgent), () => F.Of(new Role().CurrentAgent)))); set { }
        }

        // Formula HasEscalationBackup (rulebook: ={{EscalationBackupRole}} <> "")
        [NotMapped]
        public bool? HasEscalationBackup
        {
            get => F.AsBool(F.Memo(this, "HasEscalationBackup", () => F.IsNotBlank(F.Of(this.EscalationBackupRole)))); set { }
        }

        // Formula HasUnfilledEscalationBackup (rulebook: =AND({{EscalationBackupRole}} <> "", {{BackupRoleHolder}} = ""))
        [NotMapped]
        public bool? HasUnfilledEscalationBackup
        {
            get => F.AsBool(F.Memo(this, "HasUnfilledEscalationBackup", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.EscalationBackupRole))), F.Bool3(F.IsBlank(F.Of(this.BackupRoleHolder)))))); set { }
        }

        // Formula ReleaseApprovalStepCount (rulebook: =COUNTIFS(Steps!{{AssignedRole}}, {{RoleId}}, Steps!{{IsReleaseApprovalGate}}, TRUE))
        [NotMapped]
        public int? ReleaseApprovalStepCount
        {
            get => F.AsInt(F.Memo(this, "ReleaseApprovalStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.AssignedRole), F.Of(this.RoleId)) && F.CritLiteral(F.Of(__r.IsReleaseApprovalGate), F.B(true))))))); set { }
        }

        // Formula IsProductionReleaseApprover (rulebook: ={{ReleaseApprovalStepCount}} > 0)
        [NotMapped]
        public bool? IsProductionReleaseApprover
        {
            get => F.AsBool(F.Memo(this, "IsProductionReleaseApprover", () => F.Cmp(F.Of(this.ReleaseApprovalStepCount), ">", F.I(0)))); set { }
        }

        // Formula ApprovalStepCount (rulebook: =COUNTIFS(Steps!{{AssignedRole}}, {{RoleId}}, Steps!{{ControlKind}}, "Approval"))
        [NotMapped]
        public int? ApprovalStepCount
        {
            get => F.AsInt(F.Memo(this, "ApprovalStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.AssignedRole), F.Of(this.RoleId)) && F.CritLiteral(F.Of(__r.ControlKind), F.S("Approval"))))))); set { }
        }


        public string? Organization { get; set; }
        public string? CurrentAgent { get; set; }
        public string? SpecializesRole { get; set; }
        public string? EscalationBackupRole { get; set; }

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

        private Role _role;

        [ForeignKey("SpecializesRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(SpecializesRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. SpecializesRole: " + SpecializesRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(SpecializesRole);
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
                        SpecializesRole = _role.RoleId;
                    }
                }
            }
        }

        private Role _roleRef;

        [ForeignKey("EscalationBackupRole")]
        public virtual Role RoleRef
        {
            get
            {
                if (_roleRef == null && !string.IsNullOrEmpty(EscalationBackupRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleRef - no database context is set. EscalationBackupRole: " + EscalationBackupRole + ".");
                        }
                        return null;
                    }
                    _roleRef = base.SoAContext.Roles.Find(EscalationBackupRole);
                    if (_roleRef != null)
                    {
                        base.SoAContext.Attach(_roleRef);
                    }
                }
                return _roleRef;
            }
            set
            {
                if (_roleRef != value)
                {
                    _roleRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleRef != null)
                    {
                        EscalationBackupRole = _roleRef.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<Role> _specializesRoleRoles;

        [InverseProperty("Role")]
        public virtual ObservableCollection<Role> SpecializesRoleRoles
        {
            get
            {
                if (_specializesRoleRoles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SpecializesRoleRoles - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _specializesRoleRoles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.SpecializesRole == this.RoleId).ToList<Role>();
                        _specializesRoleRoles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _specializesRoleRoles.CollectionChanged += SpecializesRoleRoles_CollectionChanged;
                }
                return _specializesRoleRoles;
            }
            private set
            {
                if (_specializesRoleRoles != null)
                {
                    _specializesRoleRoles.CollectionChanged -= SpecializesRoleRoles_CollectionChanged;
                }
                _specializesRoleRoles = value;
                if (_specializesRoleRoles != null)
                {
                    _specializesRoleRoles.CollectionChanged += SpecializesRoleRoles_CollectionChanged;
                }
            }
        }

        private void SpecializesRoleRoles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.SpecializesRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<Role> _escalationBackupRoleRoles;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<Role> EscalationBackupRoleRoles
        {
            get
            {
                if (_escalationBackupRoleRoles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EscalationBackupRoleRoles - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _escalationBackupRoleRoles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.EscalationBackupRole == this.RoleId).ToList<Role>();
                        _escalationBackupRoleRoles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _escalationBackupRoleRoles.CollectionChanged += EscalationBackupRoleRoles_CollectionChanged;
                }
                return _escalationBackupRoleRoles;
            }
            private set
            {
                if (_escalationBackupRoleRoles != null)
                {
                    _escalationBackupRoleRoles.CollectionChanged -= EscalationBackupRoleRoles_CollectionChanged;
                }
                _escalationBackupRoleRoles = value;
                if (_escalationBackupRoleRoles != null)
                {
                    _escalationBackupRoleRoles.CollectionChanged += EscalationBackupRoleRoles_CollectionChanged;
                }
            }
        }

        private void EscalationBackupRoleRoles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.EscalationBackupRole = this.RoleId;
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

        private ObservableCollection<VocabularyTerm> _vocabularyTerms;

        [InverseProperty("Role")]
        public virtual ObservableCollection<VocabularyTerm> VocabularyTerms
        {
            get
            {
                if (_vocabularyTerms == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerms - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _vocabularyTerms = new ObservableCollection<VocabularyTerm>();
                    }
                    else
                    {
                        var items = base.SoAContext.VocabularyTerms.Where(x => x.RepresentsRole == this.RoleId).ToList<VocabularyTerm>();
                        _vocabularyTerms = new ObservableCollection<VocabularyTerm>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _vocabularyTerms.CollectionChanged += VocabularyTerms_CollectionChanged;
                }
                return _vocabularyTerms;
            }
            private set
            {
                if (_vocabularyTerms != null)
                {
                    _vocabularyTerms.CollectionChanged -= VocabularyTerms_CollectionChanged;
                }
                _vocabularyTerms = value;
                if (_vocabularyTerms != null)
                {
                    _vocabularyTerms.CollectionChanged += VocabularyTerms_CollectionChanged;
                }
            }
        }

        private void VocabularyTerms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<VocabularyTerm>())
                {
                    item.RepresentsRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<FailureMode> _failureModes;

        [InverseProperty("Role")]
        public virtual ObservableCollection<FailureMode> FailureModes
        {
            get
            {
                if (_failureModes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FailureModes - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _failureModes = new ObservableCollection<FailureMode>();
                    }
                    else
                    {
                        var items = base.SoAContext.FailureModes.Where(x => x.EscalateToRole == this.RoleId).ToList<FailureMode>();
                        _failureModes = new ObservableCollection<FailureMode>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _failureModes.CollectionChanged += FailureModes_CollectionChanged;
                }
                return _failureModes;
            }
            private set
            {
                if (_failureModes != null)
                {
                    _failureModes.CollectionChanged -= FailureModes_CollectionChanged;
                }
                _failureModes = value;
                if (_failureModes != null)
                {
                    _failureModes.CollectionChanged += FailureModes_CollectionChanged;
                }
            }
        }

        private void FailureModes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FailureMode>())
                {
                    item.EscalateToRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<StepCue> _stepCues;

        [InverseProperty("Role")]
        public virtual ObservableCollection<StepCue> StepCues
        {
            get
            {
                if (_stepCues == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepCues - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _stepCues = new ObservableCollection<StepCue>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepCues.Where(x => x.EscalateToRole == this.RoleId).ToList<StepCue>();
                        _stepCues = new ObservableCollection<StepCue>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepCues.CollectionChanged += StepCues_CollectionChanged;
                }
                return _stepCues;
            }
            private set
            {
                if (_stepCues != null)
                {
                    _stepCues.CollectionChanged -= StepCues_CollectionChanged;
                }
                _stepCues = value;
                if (_stepCues != null)
                {
                    _stepCues.CollectionChanged += StepCues_CollectionChanged;
                }
            }
        }

        private void StepCues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepCue>())
                {
                    item.EscalateToRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<BusinessOutcome> _businessOutcomes;

        [InverseProperty("Role")]
        public virtual ObservableCollection<BusinessOutcome> BusinessOutcomes
        {
            get
            {
                if (_businessOutcomes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access BusinessOutcomes - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _businessOutcomes = new ObservableCollection<BusinessOutcome>();
                    }
                    else
                    {
                        var items = base.SoAContext.BusinessOutcomes.Where(x => x.OwnerRole == this.RoleId).ToList<BusinessOutcome>();
                        _businessOutcomes = new ObservableCollection<BusinessOutcome>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _businessOutcomes.CollectionChanged += BusinessOutcomes_CollectionChanged;
                }
                return _businessOutcomes;
            }
            private set
            {
                if (_businessOutcomes != null)
                {
                    _businessOutcomes.CollectionChanged -= BusinessOutcomes_CollectionChanged;
                }
                _businessOutcomes = value;
                if (_businessOutcomes != null)
                {
                    _businessOutcomes.CollectionChanged += BusinessOutcomes_CollectionChanged;
                }
            }
        }

        private void BusinessOutcomes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<BusinessOutcome>())
                {
                    item.OwnerRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<ProcessStage> _processStages;

        [InverseProperty("Role")]
        public virtual ObservableCollection<ProcessStage> ProcessStages
        {
            get
            {
                if (_processStages == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessStages - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _processStages = new ObservableCollection<ProcessStage>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessStages.Where(x => x.OwnerRole == this.RoleId).ToList<ProcessStage>();
                        _processStages = new ObservableCollection<ProcessStage>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _processStages.CollectionChanged += ProcessStages_CollectionChanged;
                }
                return _processStages;
            }
            private set
            {
                if (_processStages != null)
                {
                    _processStages.CollectionChanged -= ProcessStages_CollectionChanged;
                }
                _processStages = value;
                if (_processStages != null)
                {
                    _processStages.CollectionChanged += ProcessStages_CollectionChanged;
                }
            }
        }

        private void ProcessStages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessStage>())
                {
                    item.OwnerRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<StakeholderLense> _stakeholderLenses;

        [InverseProperty("Role")]
        public virtual ObservableCollection<StakeholderLense> StakeholderLenses
        {
            get
            {
                if (_stakeholderLenses == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StakeholderLenses - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _stakeholderLenses = new ObservableCollection<StakeholderLense>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderLenses.Where(x => x.ExemplarRole == this.RoleId).ToList<StakeholderLense>();
                        _stakeholderLenses = new ObservableCollection<StakeholderLense>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stakeholderLenses.CollectionChanged += StakeholderLenses_CollectionChanged;
                }
                return _stakeholderLenses;
            }
            private set
            {
                if (_stakeholderLenses != null)
                {
                    _stakeholderLenses.CollectionChanged -= StakeholderLenses_CollectionChanged;
                }
                _stakeholderLenses = value;
                if (_stakeholderLenses != null)
                {
                    _stakeholderLenses.CollectionChanged += StakeholderLenses_CollectionChanged;
                }
            }
        }

        private void StakeholderLenses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StakeholderLense>())
                {
                    item.ExemplarRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<RoleCapabilityTag> _roleCapabilityTags;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<RoleCapabilityTag> RoleCapabilityTags
        {
            get
            {
                if (_roleCapabilityTags == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleCapabilityTags - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _roleCapabilityTags = new ObservableCollection<RoleCapabilityTag>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleCapabilityTags.Where(x => x.Role == this.RoleId).ToList<RoleCapabilityTag>();
                        _roleCapabilityTags = new ObservableCollection<RoleCapabilityTag>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roleCapabilityTags.CollectionChanged += RoleCapabilityTags_CollectionChanged;
                }
                return _roleCapabilityTags;
            }
            private set
            {
                if (_roleCapabilityTags != null)
                {
                    _roleCapabilityTags.CollectionChanged -= RoleCapabilityTags_CollectionChanged;
                }
                _roleCapabilityTags = value;
                if (_roleCapabilityTags != null)
                {
                    _roleCapabilityTags.CollectionChanged += RoleCapabilityTags_CollectionChanged;
                }
            }
        }

        private void RoleCapabilityTags_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleCapabilityTag>())
                {
                    item.Role = this.RoleId;
                }
            }
        }

        private ObservableCollection<GroundingSnapshot> _groundingSnapshots;

        [InverseProperty("Role")]
        public virtual ObservableCollection<GroundingSnapshot> GroundingSnapshots
        {
            get
            {
                if (_groundingSnapshots == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GroundingSnapshots - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _groundingSnapshots = new ObservableCollection<GroundingSnapshot>();
                    }
                    else
                    {
                        var items = base.SoAContext.GroundingSnapshots.Where(x => x.StewardRole == this.RoleId).ToList<GroundingSnapshot>();
                        _groundingSnapshots = new ObservableCollection<GroundingSnapshot>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _groundingSnapshots.CollectionChanged += GroundingSnapshots_CollectionChanged;
                }
                return _groundingSnapshots;
            }
            private set
            {
                if (_groundingSnapshots != null)
                {
                    _groundingSnapshots.CollectionChanged -= GroundingSnapshots_CollectionChanged;
                }
                _groundingSnapshots = value;
                if (_groundingSnapshots != null)
                {
                    _groundingSnapshots.CollectionChanged += GroundingSnapshots_CollectionChanged;
                }
            }
        }

        private void GroundingSnapshots_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<GroundingSnapshot>())
                {
                    item.StewardRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<RetrievalSegment> _retrievalSegments;

        [InverseProperty("Role")]
        public virtual ObservableCollection<RetrievalSegment> RetrievalSegments
        {
            get
            {
                if (_retrievalSegments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RetrievalSegments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _retrievalSegments = new ObservableCollection<RetrievalSegment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RetrievalSegments.Where(x => x.AccountableRole == this.RoleId).ToList<RetrievalSegment>();
                        _retrievalSegments = new ObservableCollection<RetrievalSegment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _retrievalSegments.CollectionChanged += RetrievalSegments_CollectionChanged;
                }
                return _retrievalSegments;
            }
            private set
            {
                if (_retrievalSegments != null)
                {
                    _retrievalSegments.CollectionChanged -= RetrievalSegments_CollectionChanged;
                }
                _retrievalSegments = value;
                if (_retrievalSegments != null)
                {
                    _retrievalSegments.CollectionChanged += RetrievalSegments_CollectionChanged;
                }
            }
        }

        private void RetrievalSegments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RetrievalSegment>())
                {
                    item.AccountableRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<PromptTemplate> _promptTemplates;

        [InverseProperty("Role")]
        public virtual ObservableCollection<PromptTemplate> PromptTemplates
        {
            get
            {
                if (_promptTemplates == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PromptTemplates - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _promptTemplates = new ObservableCollection<PromptTemplate>();
                    }
                    else
                    {
                        var items = base.SoAContext.PromptTemplates.Where(x => x.MaintainedByRole == this.RoleId).ToList<PromptTemplate>();
                        _promptTemplates = new ObservableCollection<PromptTemplate>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _promptTemplates.CollectionChanged += PromptTemplates_CollectionChanged;
                }
                return _promptTemplates;
            }
            private set
            {
                if (_promptTemplates != null)
                {
                    _promptTemplates.CollectionChanged -= PromptTemplates_CollectionChanged;
                }
                _promptTemplates = value;
                if (_promptTemplates != null)
                {
                    _promptTemplates.CollectionChanged += PromptTemplates_CollectionChanged;
                }
            }
        }

        private void PromptTemplates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<PromptTemplate>())
                {
                    item.MaintainedByRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<KnowledgeProjection> _knowledgeProjections;

        [InverseProperty("Role")]
        public virtual ObservableCollection<KnowledgeProjection> KnowledgeProjections
        {
            get
            {
                if (_knowledgeProjections == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeProjections - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _knowledgeProjections = new ObservableCollection<KnowledgeProjection>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeProjections.Where(x => x.AudienceRole == this.RoleId).ToList<KnowledgeProjection>();
                        _knowledgeProjections = new ObservableCollection<KnowledgeProjection>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeProjections.CollectionChanged += KnowledgeProjections_CollectionChanged;
                }
                return _knowledgeProjections;
            }
            private set
            {
                if (_knowledgeProjections != null)
                {
                    _knowledgeProjections.CollectionChanged -= KnowledgeProjections_CollectionChanged;
                }
                _knowledgeProjections = value;
                if (_knowledgeProjections != null)
                {
                    _knowledgeProjections.CollectionChanged += KnowledgeProjections_CollectionChanged;
                }
            }
        }

        private void KnowledgeProjections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeProjection>())
                {
                    item.AudienceRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<GovernedModel> _governedModels;

        [InverseProperty("Role")]
        public virtual ObservableCollection<GovernedModel> GovernedModels
        {
            get
            {
                if (_governedModels == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModels - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _governedModels = new ObservableCollection<GovernedModel>();
                    }
                    else
                    {
                        var items = base.SoAContext.GovernedModels.Where(x => x.ToolingOwnerRole == this.RoleId).ToList<GovernedModel>();
                        _governedModels = new ObservableCollection<GovernedModel>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _governedModels.CollectionChanged += GovernedModels_CollectionChanged;
                }
                return _governedModels;
            }
            private set
            {
                if (_governedModels != null)
                {
                    _governedModels.CollectionChanged -= GovernedModels_CollectionChanged;
                }
                _governedModels = value;
                if (_governedModels != null)
                {
                    _governedModels.CollectionChanged += GovernedModels_CollectionChanged;
                }
            }
        }

        private void GovernedModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<GovernedModel>())
                {
                    item.ToolingOwnerRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<ModelCharter> _stewardRoleModelCharters;

        [InverseProperty("Role")]
        public virtual ObservableCollection<ModelCharter> StewardRoleModelCharters
        {
            get
            {
                if (_stewardRoleModelCharters == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StewardRoleModelCharters - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _stewardRoleModelCharters = new ObservableCollection<ModelCharter>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelCharters.Where(x => x.StewardRole == this.RoleId).ToList<ModelCharter>();
                        _stewardRoleModelCharters = new ObservableCollection<ModelCharter>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stewardRoleModelCharters.CollectionChanged += StewardRoleModelCharters_CollectionChanged;
                }
                return _stewardRoleModelCharters;
            }
            private set
            {
                if (_stewardRoleModelCharters != null)
                {
                    _stewardRoleModelCharters.CollectionChanged -= StewardRoleModelCharters_CollectionChanged;
                }
                _stewardRoleModelCharters = value;
                if (_stewardRoleModelCharters != null)
                {
                    _stewardRoleModelCharters.CollectionChanged += StewardRoleModelCharters_CollectionChanged;
                }
            }
        }

        private void StewardRoleModelCharters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelCharter>())
                {
                    item.StewardRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<ModelCharter> _authorityRoleModelCharters;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<ModelCharter> AuthorityRoleModelCharters
        {
            get
            {
                if (_authorityRoleModelCharters == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorityRoleModelCharters - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _authorityRoleModelCharters = new ObservableCollection<ModelCharter>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelCharters.Where(x => x.AuthorityRole == this.RoleId).ToList<ModelCharter>();
                        _authorityRoleModelCharters = new ObservableCollection<ModelCharter>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _authorityRoleModelCharters.CollectionChanged += AuthorityRoleModelCharters_CollectionChanged;
                }
                return _authorityRoleModelCharters;
            }
            private set
            {
                if (_authorityRoleModelCharters != null)
                {
                    _authorityRoleModelCharters.CollectionChanged -= AuthorityRoleModelCharters_CollectionChanged;
                }
                _authorityRoleModelCharters = value;
                if (_authorityRoleModelCharters != null)
                {
                    _authorityRoleModelCharters.CollectionChanged += AuthorityRoleModelCharters_CollectionChanged;
                }
            }
        }

        private void AuthorityRoleModelCharters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelCharter>())
                {
                    item.AuthorityRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<ChangeAuthorityRule> _permittedRoleChangeAuthorityRules;

        [InverseProperty("Role")]
        public virtual ObservableCollection<ChangeAuthorityRule> PermittedRoleChangeAuthorityRules
        {
            get
            {
                if (_permittedRoleChangeAuthorityRules == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PermittedRoleChangeAuthorityRules - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _permittedRoleChangeAuthorityRules = new ObservableCollection<ChangeAuthorityRule>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeAuthorityRules.Where(x => x.PermittedRole == this.RoleId).ToList<ChangeAuthorityRule>();
                        _permittedRoleChangeAuthorityRules = new ObservableCollection<ChangeAuthorityRule>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _permittedRoleChangeAuthorityRules.CollectionChanged += PermittedRoleChangeAuthorityRules_CollectionChanged;
                }
                return _permittedRoleChangeAuthorityRules;
            }
            private set
            {
                if (_permittedRoleChangeAuthorityRules != null)
                {
                    _permittedRoleChangeAuthorityRules.CollectionChanged -= PermittedRoleChangeAuthorityRules_CollectionChanged;
                }
                _permittedRoleChangeAuthorityRules = value;
                if (_permittedRoleChangeAuthorityRules != null)
                {
                    _permittedRoleChangeAuthorityRules.CollectionChanged += PermittedRoleChangeAuthorityRules_CollectionChanged;
                }
            }
        }

        private void PermittedRoleChangeAuthorityRules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeAuthorityRule>())
                {
                    item.PermittedRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<ChangeAuthorityRule> _approvalRoleChangeAuthorityRules;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<ChangeAuthorityRule> ApprovalRoleChangeAuthorityRules
        {
            get
            {
                if (_approvalRoleChangeAuthorityRules == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApprovalRoleChangeAuthorityRules - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _approvalRoleChangeAuthorityRules = new ObservableCollection<ChangeAuthorityRule>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeAuthorityRules.Where(x => x.ApprovalRole == this.RoleId).ToList<ChangeAuthorityRule>();
                        _approvalRoleChangeAuthorityRules = new ObservableCollection<ChangeAuthorityRule>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _approvalRoleChangeAuthorityRules.CollectionChanged += ApprovalRoleChangeAuthorityRules_CollectionChanged;
                }
                return _approvalRoleChangeAuthorityRules;
            }
            private set
            {
                if (_approvalRoleChangeAuthorityRules != null)
                {
                    _approvalRoleChangeAuthorityRules.CollectionChanged -= ApprovalRoleChangeAuthorityRules_CollectionChanged;
                }
                _approvalRoleChangeAuthorityRules = value;
                if (_approvalRoleChangeAuthorityRules != null)
                {
                    _approvalRoleChangeAuthorityRules.CollectionChanged += ApprovalRoleChangeAuthorityRules_CollectionChanged;
                }
            }
        }

        private void ApprovalRoleChangeAuthorityRules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeAuthorityRule>())
                {
                    item.ApprovalRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<ModelConsumer> _modelConsumers;

        [InverseProperty("Role")]
        public virtual ObservableCollection<ModelConsumer> ModelConsumers
        {
            get
            {
                if (_modelConsumers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelConsumers - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _modelConsumers = new ObservableCollection<ModelConsumer>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelConsumers.Where(x => x.OwnerRole == this.RoleId).ToList<ModelConsumer>();
                        _modelConsumers = new ObservableCollection<ModelConsumer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelConsumers.CollectionChanged += ModelConsumers_CollectionChanged;
                }
                return _modelConsumers;
            }
            private set
            {
                if (_modelConsumers != null)
                {
                    _modelConsumers.CollectionChanged -= ModelConsumers_CollectionChanged;
                }
                _modelConsumers = value;
                if (_modelConsumers != null)
                {
                    _modelConsumers.CollectionChanged += ModelConsumers_CollectionChanged;
                }
            }
        }

        private void ModelConsumers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelConsumer>())
                {
                    item.OwnerRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<KnowledgeWorkforcePosition> _knowledgeWorkforcePositions;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<KnowledgeWorkforcePosition> KnowledgeWorkforcePositions
        {
            get
            {
                if (_knowledgeWorkforcePositions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeWorkforcePositions - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _knowledgeWorkforcePositions = new ObservableCollection<KnowledgeWorkforcePosition>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeWorkforcePositions.Where(x => x.Role == this.RoleId).ToList<KnowledgeWorkforcePosition>();
                        _knowledgeWorkforcePositions = new ObservableCollection<KnowledgeWorkforcePosition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeWorkforcePositions.CollectionChanged += KnowledgeWorkforcePositions_CollectionChanged;
                }
                return _knowledgeWorkforcePositions;
            }
            private set
            {
                if (_knowledgeWorkforcePositions != null)
                {
                    _knowledgeWorkforcePositions.CollectionChanged -= KnowledgeWorkforcePositions_CollectionChanged;
                }
                _knowledgeWorkforcePositions = value;
                if (_knowledgeWorkforcePositions != null)
                {
                    _knowledgeWorkforcePositions.CollectionChanged += KnowledgeWorkforcePositions_CollectionChanged;
                }
            }
        }

        private void KnowledgeWorkforcePositions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeWorkforcePosition>())
                {
                    item.Role = this.RoleId;
                }
            }
        }

        private ObservableCollection<AssignmentUpdatePolicy> _assignmentUpdatePolicies;

        [InverseProperty("Role")]
        public virtual ObservableCollection<AssignmentUpdatePolicy> AssignmentUpdatePolicies
        {
            get
            {
                if (_assignmentUpdatePolicies == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssignmentUpdatePolicies - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _assignmentUpdatePolicies = new ObservableCollection<AssignmentUpdatePolicy>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssignmentUpdatePolicies.Where(x => x.TriggerOwnerRole == this.RoleId).ToList<AssignmentUpdatePolicy>();
                        _assignmentUpdatePolicies = new ObservableCollection<AssignmentUpdatePolicy>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assignmentUpdatePolicies.CollectionChanged += AssignmentUpdatePolicies_CollectionChanged;
                }
                return _assignmentUpdatePolicies;
            }
            private set
            {
                if (_assignmentUpdatePolicies != null)
                {
                    _assignmentUpdatePolicies.CollectionChanged -= AssignmentUpdatePolicies_CollectionChanged;
                }
                _assignmentUpdatePolicies = value;
                if (_assignmentUpdatePolicies != null)
                {
                    _assignmentUpdatePolicies.CollectionChanged += AssignmentUpdatePolicies_CollectionChanged;
                }
            }
        }

        private void AssignmentUpdatePolicies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssignmentUpdatePolicy>())
                {
                    item.TriggerOwnerRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<RoleAssignmentUpdateTask> _roleAssignmentUpdateTasks;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<RoleAssignmentUpdateTask> RoleAssignmentUpdateTasks
        {
            get
            {
                if (_roleAssignmentUpdateTasks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignmentUpdateTasks - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _roleAssignmentUpdateTasks = new ObservableCollection<RoleAssignmentUpdateTask>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignmentUpdateTasks.Where(x => x.Role == this.RoleId).ToList<RoleAssignmentUpdateTask>();
                        _roleAssignmentUpdateTasks = new ObservableCollection<RoleAssignmentUpdateTask>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roleAssignmentUpdateTasks.CollectionChanged += RoleAssignmentUpdateTasks_CollectionChanged;
                }
                return _roleAssignmentUpdateTasks;
            }
            private set
            {
                if (_roleAssignmentUpdateTasks != null)
                {
                    _roleAssignmentUpdateTasks.CollectionChanged -= RoleAssignmentUpdateTasks_CollectionChanged;
                }
                _roleAssignmentUpdateTasks = value;
                if (_roleAssignmentUpdateTasks != null)
                {
                    _roleAssignmentUpdateTasks.CollectionChanged += RoleAssignmentUpdateTasks_CollectionChanged;
                }
            }
        }

        private void RoleAssignmentUpdateTasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleAssignmentUpdateTask>())
                {
                    item.Role = this.RoleId;
                }
            }
        }

        private ObservableCollection<StakeholderPerspectif> _stakeholderPerspectives;

        [InverseProperty("Role")]
        public virtual ObservableCollection<StakeholderPerspectif> StakeholderPerspectives
        {
            get
            {
                if (_stakeholderPerspectives == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StakeholderPerspectives - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _stakeholderPerspectives = new ObservableCollection<StakeholderPerspectif>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderPerspectives.Where(x => x.HolderRole == this.RoleId).ToList<StakeholderPerspectif>();
                        _stakeholderPerspectives = new ObservableCollection<StakeholderPerspectif>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stakeholderPerspectives.CollectionChanged += StakeholderPerspectives_CollectionChanged;
                }
                return _stakeholderPerspectives;
            }
            private set
            {
                if (_stakeholderPerspectives != null)
                {
                    _stakeholderPerspectives.CollectionChanged -= StakeholderPerspectives_CollectionChanged;
                }
                _stakeholderPerspectives = value;
                if (_stakeholderPerspectives != null)
                {
                    _stakeholderPerspectives.CollectionChanged += StakeholderPerspectives_CollectionChanged;
                }
            }
        }

        private void StakeholderPerspectives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StakeholderPerspectif>())
                {
                    item.HolderRole = this.RoleId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.Agent;
            _ = this.Role;
            _ = this.RoleRef;
            _ = this.SpecializesRoleRoles;
            _ = this.EscalationBackupRoleRoles;
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
            _ = this.VocabularyTerms;
            _ = this.FailureModes;
            _ = this.StepCues;
            _ = this.BusinessOutcomes;
            _ = this.ProcessStages;
            _ = this.StakeholderLenses;
            _ = this.RoleCapabilityTags;
            _ = this.GroundingSnapshots;
            _ = this.RetrievalSegments;
            _ = this.PromptTemplates;
            _ = this.KnowledgeProjections;
            _ = this.GovernedModels;
            _ = this.StewardRoleModelCharters;
            _ = this.AuthorityRoleModelCharters;
            _ = this.PermittedRoleChangeAuthorityRules;
            _ = this.ApprovalRoleChangeAuthorityRules;
            _ = this.ModelConsumers;
            _ = this.KnowledgeWorkforcePositions;
            _ = this.AssignmentUpdatePolicies;
            _ = this.RoleAssignmentUpdateTasks;
            _ = this.StakeholderPerspectives;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
