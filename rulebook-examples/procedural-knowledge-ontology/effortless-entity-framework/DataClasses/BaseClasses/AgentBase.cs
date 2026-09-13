
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
    [Table("Agents")]
    public class AgentBase : SoAEntityBase
    {
        [Key]
        public string AgentId { get; set; }

        // Formula Name (rulebook: ={{DisplayName}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.DisplayName))); set { }
        }

        public string? DisplayName { get; set; }
        public string? AgentKind { get; set; }
        public string? ContactAddress { get; set; }
        public string? VersionOrEmploymentKey { get; set; }
        // Formula CountOfCurrentRoleAssignments (rulebook: =COUNTIFS(RoleAssignments!{{CurrentAgentKey}}, {{AgentId}}))
        [NotMapped]
        public int? CountOfCurrentRoleAssignments
        {
            get => F.AsInt(F.Memo(this, "CountOfCurrentRoleAssignments", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.CurrentAgentKey), F.Of(this.AgentId))))))); set { }
        }

        // Formula IsStillEngaged (rulebook: ={{CountOfCurrentRoleAssignments}} > 0)
        [NotMapped]
        public bool? IsStillEngaged
        {
            get => F.AsBool(F.Memo(this, "IsStillEngaged", () => F.Cmp(F.Of(this.CountOfCurrentRoleAssignments), ">", F.I(0)))); set { }
        }

        // Formula DecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{DecidingAgent}}, {{AgentId}}))
        [NotMapped]
        public decimal? DecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "DecisionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.DecidingAgent), F.Of(this.AgentId)))))); set { }
        }

        // Formula OverriddenDecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{DecidingAgentWhenOverridden}}, {{AgentId}}))
        [NotMapped]
        public decimal? OverriddenDecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "OverriddenDecisionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.DecidingAgentWhenOverridden), F.Of(this.AgentId)))))); set { }
        }

        // Formula OverrideRatePercent (rulebook: =IF({{DecisionCount}} = 0, 0, ({{OverriddenDecisionCount}} * 100) / {{DecisionCount}}))
        [NotMapped]
        public decimal? OverrideRatePercent
        {
            get => F.AsDecimal(F.Memo(this, "OverrideRatePercent", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.DecisionCount), F.I(0)))) ? F.I(0) : F.Div(F.Mul(F.Of(this.OverriddenDecisionCount), F.I(100)), F.Of(this.DecisionCount))))); set { }
        }

        // Formula IsNonHuman (rulebook: =NOT({{AgentKind}} = "Human"))
        [NotMapped]
        public bool? IsNonHuman
        {
            get => F.AsBool(F.Memo(this, "IsNonHuman", () => F.Not(F.Bool3(F.Eq(F.Nullif(F.Of(this.AgentKind)), F.S("Human")))))); set { }
        }

        // Formula BoundaryViolationCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{AgentWhenBoundaryViolated}}, {{AgentId}}))
        [NotMapped]
        public decimal? BoundaryViolationCount
        {
            get => F.AsDecimal(F.Memo(this, "BoundaryViolationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.AgentWhenBoundaryViolated), F.Of(this.AgentId)))))); set { }
        }

        // Formula IsOperatingOutsideBoundary (rulebook: ={{BoundaryViolationCount}} > 0)
        [NotMapped]
        public bool? IsOperatingOutsideBoundary
        {
            get => F.AsBool(F.Memo(this, "IsOperatingOutsideBoundary", () => F.Cmp(F.Of(this.BoundaryViolationCount), ">", F.I(0)))); set { }
        }

        // Formula DraftDecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{AgentWhenDraft}}, {{AgentId}}))
        [NotMapped]
        public decimal? DraftDecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "DraftDecisionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.AgentWhenDraft), F.Of(this.AgentId)))))); set { }
        }

        // Formula OverriddenDraftCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{AgentWhenDraftOverridden}}, {{AgentId}}))
        [NotMapped]
        public decimal? OverriddenDraftCount
        {
            get => F.AsDecimal(F.Memo(this, "OverriddenDraftCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.AgentWhenDraftOverridden), F.Of(this.AgentId)))))); set { }
        }

        // Formula DraftRewriteRatePercent (rulebook: =IF({{DraftDecisionCount}} = 0, 0, ({{OverriddenDraftCount}} * 100) / {{DraftDecisionCount}}))
        [NotMapped]
        public decimal? DraftRewriteRatePercent
        {
            get => F.AsDecimal(F.Memo(this, "DraftRewriteRatePercent", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.DraftDecisionCount), F.I(0)))) ? F.I(0) : F.Div(F.Mul(F.Of(this.OverriddenDraftCount), F.I(100)), F.Of(this.DraftDecisionCount))))); set { }
        }

        // Formula TimesNamedAsBroker (rulebook: =COUNTIFS(KnowledgeBrokerLinks!{{ActiveRelianceBrokerKey}}, {{AgentId}}))
        [NotMapped]
        public decimal? TimesNamedAsBroker
        {
            get => F.AsDecimal(F.Memo(this, "TimesNamedAsBroker", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeBrokerLink>(base.SoAContext, "KnowledgeBrokerLinks", __c => __c.KnowledgeBrokerLinks), __r => F.CritField(F.Of(__r.ActiveRelianceBrokerKey), F.Of(this.AgentId)))))); set { }
        }

        // Formula IsRecognizedBroker (rulebook: ={{TimesNamedAsBroker}} >= 3)
        [NotMapped]
        public bool? IsRecognizedBroker
        {
            get => F.AsBool(F.Memo(this, "IsRecognizedBroker", () => F.Cmp(F.Of(this.TimesNamedAsBroker), ">=", F.I(3)))); set { }
        }

        // Formula AtRiskRelianceCount (rulebook: =COUNTIFS(KnowledgeBrokerLinks!{{AtRiskBrokerKey}}, {{AgentId}}))
        [NotMapped]
        public decimal? AtRiskRelianceCount
        {
            get => F.AsDecimal(F.Memo(this, "AtRiskRelianceCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeBrokerLink>(base.SoAContext, "KnowledgeBrokerLinks", __c => __c.KnowledgeBrokerLinks), __r => F.CritField(F.Of(__r.AtRiskBrokerKey), F.Of(this.AgentId)))))); set { }
        }

        // Formula HasAtRiskKnowledgeReliance (rulebook: ={{AtRiskRelianceCount}} > 0)
        [NotMapped]
        public bool? HasAtRiskKnowledgeReliance
        {
            get => F.AsBool(F.Memo(this, "HasAtRiskKnowledgeReliance", () => F.Cmp(F.Of(this.AtRiskRelianceCount), ">", F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }

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

        private ObservableCollection<Role> _roles;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<Role> Roles
        {
            get
            {
                if (_roles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Roles - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _roles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.CurrentAgent == this.AgentId).ToList<Role>();
                        _roles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
                return _roles;
            }
            private set
            {
                if (_roles != null)
                {
                    _roles.CollectionChanged -= Roles_CollectionChanged;
                }
                _roles = value;
                if (_roles != null)
                {
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
            }
        }

        private void Roles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.CurrentAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<RoleAssignment> _roleAssignments;

        [InverseProperty("AgentRef")]
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
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignments.Where(x => x.Agent == this.AgentId).ToList<RoleAssignment>();
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
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<Mentorship> _mentorAgentMentorships;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<Mentorship> MentorAgentMentorships
        {
            get
            {
                if (_mentorAgentMentorships == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MentorAgentMentorships - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _mentorAgentMentorships = new ObservableCollection<Mentorship>();
                    }
                    else
                    {
                        var items = base.SoAContext.Mentorships.Where(x => x.MentorAgent == this.AgentId).ToList<Mentorship>();
                        _mentorAgentMentorships = new ObservableCollection<Mentorship>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _mentorAgentMentorships.CollectionChanged += MentorAgentMentorships_CollectionChanged;
                }
                return _mentorAgentMentorships;
            }
            private set
            {
                if (_mentorAgentMentorships != null)
                {
                    _mentorAgentMentorships.CollectionChanged -= MentorAgentMentorships_CollectionChanged;
                }
                _mentorAgentMentorships = value;
                if (_mentorAgentMentorships != null)
                {
                    _mentorAgentMentorships.CollectionChanged += MentorAgentMentorships_CollectionChanged;
                }
            }
        }

        private void MentorAgentMentorships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Mentorship>())
                {
                    item.MentorAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<Mentorship> _learnerAgentMentorships;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<Mentorship> LearnerAgentMentorships
        {
            get
            {
                if (_learnerAgentMentorships == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LearnerAgentMentorships - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _learnerAgentMentorships = new ObservableCollection<Mentorship>();
                    }
                    else
                    {
                        var items = base.SoAContext.Mentorships.Where(x => x.LearnerAgent == this.AgentId).ToList<Mentorship>();
                        _learnerAgentMentorships = new ObservableCollection<Mentorship>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _learnerAgentMentorships.CollectionChanged += LearnerAgentMentorships_CollectionChanged;
                }
                return _learnerAgentMentorships;
            }
            private set
            {
                if (_learnerAgentMentorships != null)
                {
                    _learnerAgentMentorships.CollectionChanged -= LearnerAgentMentorships_CollectionChanged;
                }
                _learnerAgentMentorships = value;
                if (_learnerAgentMentorships != null)
                {
                    _learnerAgentMentorships.CollectionChanged += LearnerAgentMentorships_CollectionChanged;
                }
            }
        }

        private void LearnerAgentMentorships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Mentorship>())
                {
                    item.LearnerAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcedureVersion> _createdByAgentProcedureVersions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ProcedureVersion> CreatedByAgentProcedureVersions
        {
            get
            {
                if (_createdByAgentProcedureVersions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CreatedByAgentProcedureVersions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _createdByAgentProcedureVersions = new ObservableCollection<ProcedureVersion>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureVersions.Where(x => x.CreatedByAgent == this.AgentId).ToList<ProcedureVersion>();
                        _createdByAgentProcedureVersions = new ObservableCollection<ProcedureVersion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _createdByAgentProcedureVersions.CollectionChanged += CreatedByAgentProcedureVersions_CollectionChanged;
                }
                return _createdByAgentProcedureVersions;
            }
            private set
            {
                if (_createdByAgentProcedureVersions != null)
                {
                    _createdByAgentProcedureVersions.CollectionChanged -= CreatedByAgentProcedureVersions_CollectionChanged;
                }
                _createdByAgentProcedureVersions = value;
                if (_createdByAgentProcedureVersions != null)
                {
                    _createdByAgentProcedureVersions.CollectionChanged += CreatedByAgentProcedureVersions_CollectionChanged;
                }
            }
        }

        private void CreatedByAgentProcedureVersions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureVersion>())
                {
                    item.CreatedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcedureVersion> _modifiedByAgentProcedureVersions;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ProcedureVersion> ModifiedByAgentProcedureVersions
        {
            get
            {
                if (_modifiedByAgentProcedureVersions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModifiedByAgentProcedureVersions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _modifiedByAgentProcedureVersions = new ObservableCollection<ProcedureVersion>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureVersions.Where(x => x.ModifiedByAgent == this.AgentId).ToList<ProcedureVersion>();
                        _modifiedByAgentProcedureVersions = new ObservableCollection<ProcedureVersion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modifiedByAgentProcedureVersions.CollectionChanged += ModifiedByAgentProcedureVersions_CollectionChanged;
                }
                return _modifiedByAgentProcedureVersions;
            }
            private set
            {
                if (_modifiedByAgentProcedureVersions != null)
                {
                    _modifiedByAgentProcedureVersions.CollectionChanged -= ModifiedByAgentProcedureVersions_CollectionChanged;
                }
                _modifiedByAgentProcedureVersions = value;
                if (_modifiedByAgentProcedureVersions != null)
                {
                    _modifiedByAgentProcedureVersions.CollectionChanged += ModifiedByAgentProcedureVersions_CollectionChanged;
                }
            }
        }

        private void ModifiedByAgentProcedureVersions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureVersion>())
                {
                    item.ModifiedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcedureStatusChange> _procedureStatusChanges;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ProcedureStatusChange> ProcedureStatusChanges
        {
            get
            {
                if (_procedureStatusChanges == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureStatusChanges - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _procedureStatusChanges = new ObservableCollection<ProcedureStatusChange>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureStatusChanges.Where(x => x.ChangedByAgent == this.AgentId).ToList<ProcedureStatusChange>();
                        _procedureStatusChanges = new ObservableCollection<ProcedureStatusChange>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.ChangedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ElicitationSession> _practitionerAgentElicitationSessions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ElicitationSession> PractitionerAgentElicitationSessions
        {
            get
            {
                if (_practitionerAgentElicitationSessions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PractitionerAgentElicitationSessions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _practitionerAgentElicitationSessions = new ObservableCollection<ElicitationSession>();
                    }
                    else
                    {
                        var items = base.SoAContext.ElicitationSessions.Where(x => x.PractitionerAgent == this.AgentId).ToList<ElicitationSession>();
                        _practitionerAgentElicitationSessions = new ObservableCollection<ElicitationSession>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _practitionerAgentElicitationSessions.CollectionChanged += PractitionerAgentElicitationSessions_CollectionChanged;
                }
                return _practitionerAgentElicitationSessions;
            }
            private set
            {
                if (_practitionerAgentElicitationSessions != null)
                {
                    _practitionerAgentElicitationSessions.CollectionChanged -= PractitionerAgentElicitationSessions_CollectionChanged;
                }
                _practitionerAgentElicitationSessions = value;
                if (_practitionerAgentElicitationSessions != null)
                {
                    _practitionerAgentElicitationSessions.CollectionChanged += PractitionerAgentElicitationSessions_CollectionChanged;
                }
            }
        }

        private void PractitionerAgentElicitationSessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ElicitationSession>())
                {
                    item.PractitionerAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ElicitationSession> _facilitatorAgentElicitationSessions;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ElicitationSession> FacilitatorAgentElicitationSessions
        {
            get
            {
                if (_facilitatorAgentElicitationSessions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FacilitatorAgentElicitationSessions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _facilitatorAgentElicitationSessions = new ObservableCollection<ElicitationSession>();
                    }
                    else
                    {
                        var items = base.SoAContext.ElicitationSessions.Where(x => x.FacilitatorAgent == this.AgentId).ToList<ElicitationSession>();
                        _facilitatorAgentElicitationSessions = new ObservableCollection<ElicitationSession>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _facilitatorAgentElicitationSessions.CollectionChanged += FacilitatorAgentElicitationSessions_CollectionChanged;
                }
                return _facilitatorAgentElicitationSessions;
            }
            private set
            {
                if (_facilitatorAgentElicitationSessions != null)
                {
                    _facilitatorAgentElicitationSessions.CollectionChanged -= FacilitatorAgentElicitationSessions_CollectionChanged;
                }
                _facilitatorAgentElicitationSessions = value;
                if (_facilitatorAgentElicitationSessions != null)
                {
                    _facilitatorAgentElicitationSessions.CollectionChanged += FacilitatorAgentElicitationSessions_CollectionChanged;
                }
            }
        }

        private void FacilitatorAgentElicitationSessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ElicitationSession>())
                {
                    item.FacilitatorAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeFragment> _knowledgeFragments;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeFragments.Where(x => x.SourceAgent == this.AgentId).ToList<KnowledgeFragment>();
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
                    item.SourceAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcedureExecution> _procedureExecutions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ProcedureExecution> ProcedureExecutions
        {
            get
            {
                if (_procedureExecutions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecutions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureExecutions.Where(x => x.ExecutedByAgent == this.AgentId).ToList<ProcedureExecution>();
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.ExecutedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<StepExecution> _stepExecutions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<StepExecution> StepExecutions
        {
            get
            {
                if (_stepExecutions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecutions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _stepExecutions = new ObservableCollection<StepExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepExecutions.Where(x => x.ExecutedByAgent == this.AgentId).ToList<StepExecution>();
                        _stepExecutions = new ObservableCollection<StepExecution>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.ExecutedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<RequirementSatisfaction> _requirementSatisfactions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<RequirementSatisfaction> RequirementSatisfactions
        {
            get
            {
                if (_requirementSatisfactions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequirementSatisfactions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>();
                    }
                    else
                    {
                        var items = base.SoAContext.RequirementSatisfactions.Where(x => x.EvaluatedByAgent == this.AgentId).ToList<RequirementSatisfaction>();
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.EvaluatedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<IssueOccurrence> _issueOccurrences;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access IssueOccurrences - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>();
                    }
                    else
                    {
                        var items = base.SoAContext.IssueOccurrences.Where(x => x.EncounteredByAgent == this.AgentId).ToList<IssueOccurrence>();
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
                    item.EncounteredByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<UserQuestion> _userQuestions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<UserQuestion> UserQuestions
        {
            get
            {
                if (_userQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserQuestions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _userQuestions = new ObservableCollection<UserQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.UserQuestions.Where(x => x.AskedByAgent == this.AgentId).ToList<UserQuestion>();
                        _userQuestions = new ObservableCollection<UserQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.AskedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<UserFeedback> _userFeedback;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<UserFeedback> UserFeedback
        {
            get
            {
                if (_userFeedback == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserFeedback - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _userFeedback = new ObservableCollection<UserFeedback>();
                    }
                    else
                    {
                        var items = base.SoAContext.UserFeedback.Where(x => x.ProvidedByAgent == this.AgentId).ToList<UserFeedback>();
                        _userFeedback = new ObservableCollection<UserFeedback>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.ProvidedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ChangeRequest> _changeRequests;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access ChangeRequests - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _changeRequests = new ObservableCollection<ChangeRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeRequests.Where(x => x.RequestedByAgent == this.AgentId).ToList<ChangeRequest>();
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
                    item.RequestedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ReviewEvent> _reviewEvents;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access ReviewEvents - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _reviewEvents = new ObservableCollection<ReviewEvent>();
                    }
                    else
                    {
                        var items = base.SoAContext.ReviewEvents.Where(x => x.ReviewedByAgent == this.AgentId).ToList<ReviewEvent>();
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
                    item.ReviewedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<LearningActivity> _learningActivities;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<LearningActivity> LearningActivities
        {
            get
            {
                if (_learningActivities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LearningActivities - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _learningActivities = new ObservableCollection<LearningActivity>();
                    }
                    else
                    {
                        var items = base.SoAContext.LearningActivities.Where(x => x.FacilitatorAgent == this.AgentId).ToList<LearningActivity>();
                        _learningActivities = new ObservableCollection<LearningActivity>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.FacilitatorAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ExceptionInvocation> _invokedByAgentExceptionInvocations;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ExceptionInvocation> InvokedByAgentExceptionInvocations
        {
            get
            {
                if (_invokedByAgentExceptionInvocations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access InvokedByAgentExceptionInvocations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _invokedByAgentExceptionInvocations = new ObservableCollection<ExceptionInvocation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExceptionInvocations.Where(x => x.InvokedByAgent == this.AgentId).ToList<ExceptionInvocation>();
                        _invokedByAgentExceptionInvocations = new ObservableCollection<ExceptionInvocation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _invokedByAgentExceptionInvocations.CollectionChanged += InvokedByAgentExceptionInvocations_CollectionChanged;
                }
                return _invokedByAgentExceptionInvocations;
            }
            private set
            {
                if (_invokedByAgentExceptionInvocations != null)
                {
                    _invokedByAgentExceptionInvocations.CollectionChanged -= InvokedByAgentExceptionInvocations_CollectionChanged;
                }
                _invokedByAgentExceptionInvocations = value;
                if (_invokedByAgentExceptionInvocations != null)
                {
                    _invokedByAgentExceptionInvocations.CollectionChanged += InvokedByAgentExceptionInvocations_CollectionChanged;
                }
            }
        }

        private void InvokedByAgentExceptionInvocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExceptionInvocation>())
                {
                    item.InvokedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ExceptionInvocation> _approvedByAgentExceptionInvocations;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ExceptionInvocation> ApprovedByAgentExceptionInvocations
        {
            get
            {
                if (_approvedByAgentExceptionInvocations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApprovedByAgentExceptionInvocations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _approvedByAgentExceptionInvocations = new ObservableCollection<ExceptionInvocation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExceptionInvocations.Where(x => x.ApprovedByAgent == this.AgentId).ToList<ExceptionInvocation>();
                        _approvedByAgentExceptionInvocations = new ObservableCollection<ExceptionInvocation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _approvedByAgentExceptionInvocations.CollectionChanged += ApprovedByAgentExceptionInvocations_CollectionChanged;
                }
                return _approvedByAgentExceptionInvocations;
            }
            private set
            {
                if (_approvedByAgentExceptionInvocations != null)
                {
                    _approvedByAgentExceptionInvocations.CollectionChanged -= ApprovedByAgentExceptionInvocations_CollectionChanged;
                }
                _approvedByAgentExceptionInvocations = value;
                if (_approvedByAgentExceptionInvocations != null)
                {
                    _approvedByAgentExceptionInvocations.CollectionChanged += ApprovedByAgentExceptionInvocations_CollectionChanged;
                }
            }
        }

        private void ApprovedByAgentExceptionInvocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExceptionInvocation>())
                {
                    item.ApprovedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<VerificationOutcome> _verificationOutcomes;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<VerificationOutcome> VerificationOutcomes
        {
            get
            {
                if (_verificationOutcomes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VerificationOutcomes - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>();
                    }
                    else
                    {
                        var items = base.SoAContext.VerificationOutcomes.Where(x => x.ObservedByAgent == this.AgentId).ToList<VerificationOutcome>();
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.ObservedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<MessageDelivery> _messageDeliveries;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<MessageDelivery> MessageDeliveries
        {
            get
            {
                if (_messageDeliveries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = base.SoAContext.MessageDeliveries.Where(x => x.SentByAgent == this.AgentId).ToList<MessageDelivery>();
                        _messageDeliveries = new ObservableCollection<MessageDelivery>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.SentByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<TemplateApproval> _templateApprovals;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access TemplateApprovals - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _templateApprovals = new ObservableCollection<TemplateApproval>();
                    }
                    else
                    {
                        var items = base.SoAContext.TemplateApprovals.Where(x => x.DecidedByAgent == this.AgentId).ToList<TemplateApproval>();
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
                    item.DecidedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AgentDecisionRecord> _decidingAgentAgentDecisionRecords;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AgentDecisionRecord> DecidingAgentAgentDecisionRecords
        {
            get
            {
                if (_decidingAgentAgentDecisionRecords == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DecidingAgentAgentDecisionRecords - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _decidingAgentAgentDecisionRecords = new ObservableCollection<AgentDecisionRecord>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentDecisionRecords.Where(x => x.DecidingAgent == this.AgentId).ToList<AgentDecisionRecord>();
                        _decidingAgentAgentDecisionRecords = new ObservableCollection<AgentDecisionRecord>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _decidingAgentAgentDecisionRecords.CollectionChanged += DecidingAgentAgentDecisionRecords_CollectionChanged;
                }
                return _decidingAgentAgentDecisionRecords;
            }
            private set
            {
                if (_decidingAgentAgentDecisionRecords != null)
                {
                    _decidingAgentAgentDecisionRecords.CollectionChanged -= DecidingAgentAgentDecisionRecords_CollectionChanged;
                }
                _decidingAgentAgentDecisionRecords = value;
                if (_decidingAgentAgentDecisionRecords != null)
                {
                    _decidingAgentAgentDecisionRecords.CollectionChanged += DecidingAgentAgentDecisionRecords_CollectionChanged;
                }
            }
        }

        private void DecidingAgentAgentDecisionRecords_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentDecisionRecord>())
                {
                    item.DecidingAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AgentDecisionRecord> _reviewedByAgentAgentDecisionRecords;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<AgentDecisionRecord> ReviewedByAgentAgentDecisionRecords
        {
            get
            {
                if (_reviewedByAgentAgentDecisionRecords == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ReviewedByAgentAgentDecisionRecords - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _reviewedByAgentAgentDecisionRecords = new ObservableCollection<AgentDecisionRecord>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentDecisionRecords.Where(x => x.ReviewedByAgent == this.AgentId).ToList<AgentDecisionRecord>();
                        _reviewedByAgentAgentDecisionRecords = new ObservableCollection<AgentDecisionRecord>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _reviewedByAgentAgentDecisionRecords.CollectionChanged += ReviewedByAgentAgentDecisionRecords_CollectionChanged;
                }
                return _reviewedByAgentAgentDecisionRecords;
            }
            private set
            {
                if (_reviewedByAgentAgentDecisionRecords != null)
                {
                    _reviewedByAgentAgentDecisionRecords.CollectionChanged -= ReviewedByAgentAgentDecisionRecords_CollectionChanged;
                }
                _reviewedByAgentAgentDecisionRecords = value;
                if (_reviewedByAgentAgentDecisionRecords != null)
                {
                    _reviewedByAgentAgentDecisionRecords.CollectionChanged += ReviewedByAgentAgentDecisionRecords_CollectionChanged;
                }
            }
        }

        private void ReviewedByAgentAgentDecisionRecords_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentDecisionRecord>())
                {
                    item.ReviewedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<Attestation> _attestations;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<Attestation> Attestations
        {
            get
            {
                if (_attestations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Attestations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _attestations = new ObservableCollection<Attestation>();
                    }
                    else
                    {
                        var items = base.SoAContext.Attestations.Where(x => x.SignedByAgent == this.AgentId).ToList<Attestation>();
                        _attestations = new ObservableCollection<Attestation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.SignedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AppUser> _appUsers;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AppUser> AppUsers
        {
            get
            {
                if (_appUsers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppUsers - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _appUsers = new ObservableCollection<AppUser>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppUsers.Where(x => x.LinkedAgent == this.AgentId).ToList<AppUser>();
                        _appUsers = new ObservableCollection<AppUser>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _appUsers.CollectionChanged += AppUsers_CollectionChanged;
                }
                return _appUsers;
            }
            private set
            {
                if (_appUsers != null)
                {
                    _appUsers.CollectionChanged -= AppUsers_CollectionChanged;
                }
                _appUsers = value;
                if (_appUsers != null)
                {
                    _appUsers.CollectionChanged += AppUsers_CollectionChanged;
                }
            }
        }

        private void AppUsers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppUser>())
                {
                    item.LinkedAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeBrokerLink> _seekerKnowledgeBrokerLinks;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<KnowledgeBrokerLink> SeekerKnowledgeBrokerLinks
        {
            get
            {
                if (_seekerKnowledgeBrokerLinks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SeekerKnowledgeBrokerLinks - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _seekerKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeBrokerLinks.Where(x => x.Seeker == this.AgentId).ToList<KnowledgeBrokerLink>();
                        _seekerKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _seekerKnowledgeBrokerLinks.CollectionChanged += SeekerKnowledgeBrokerLinks_CollectionChanged;
                }
                return _seekerKnowledgeBrokerLinks;
            }
            private set
            {
                if (_seekerKnowledgeBrokerLinks != null)
                {
                    _seekerKnowledgeBrokerLinks.CollectionChanged -= SeekerKnowledgeBrokerLinks_CollectionChanged;
                }
                _seekerKnowledgeBrokerLinks = value;
                if (_seekerKnowledgeBrokerLinks != null)
                {
                    _seekerKnowledgeBrokerLinks.CollectionChanged += SeekerKnowledgeBrokerLinks_CollectionChanged;
                }
            }
        }

        private void SeekerKnowledgeBrokerLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeBrokerLink>())
                {
                    item.Seeker = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeBrokerLink> _brokerKnowledgeBrokerLinks;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<KnowledgeBrokerLink> BrokerKnowledgeBrokerLinks
        {
            get
            {
                if (_brokerKnowledgeBrokerLinks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access BrokerKnowledgeBrokerLinks - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _brokerKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeBrokerLinks.Where(x => x.Broker == this.AgentId).ToList<KnowledgeBrokerLink>();
                        _brokerKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _brokerKnowledgeBrokerLinks.CollectionChanged += BrokerKnowledgeBrokerLinks_CollectionChanged;
                }
                return _brokerKnowledgeBrokerLinks;
            }
            private set
            {
                if (_brokerKnowledgeBrokerLinks != null)
                {
                    _brokerKnowledgeBrokerLinks.CollectionChanged -= BrokerKnowledgeBrokerLinks_CollectionChanged;
                }
                _brokerKnowledgeBrokerLinks = value;
                if (_brokerKnowledgeBrokerLinks != null)
                {
                    _brokerKnowledgeBrokerLinks.CollectionChanged += BrokerKnowledgeBrokerLinks_CollectionChanged;
                }
            }
        }

        private void BrokerKnowledgeBrokerLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeBrokerLink>())
                {
                    item.Broker = this.AgentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.Roles;
            _ = this.RoleAssignments;
            _ = this.MentorAgentMentorships;
            _ = this.LearnerAgentMentorships;
            _ = this.CreatedByAgentProcedureVersions;
            _ = this.ModifiedByAgentProcedureVersions;
            _ = this.ProcedureStatusChanges;
            _ = this.PractitionerAgentElicitationSessions;
            _ = this.FacilitatorAgentElicitationSessions;
            _ = this.KnowledgeFragments;
            _ = this.ProcedureExecutions;
            _ = this.StepExecutions;
            _ = this.RequirementSatisfactions;
            _ = this.IssueOccurrences;
            _ = this.UserQuestions;
            _ = this.UserFeedback;
            _ = this.ChangeRequests;
            _ = this.ReviewEvents;
            _ = this.LearningActivities;
            _ = this.InvokedByAgentExceptionInvocations;
            _ = this.ApprovedByAgentExceptionInvocations;
            _ = this.VerificationOutcomes;
            _ = this.MessageDeliveries;
            _ = this.TemplateApprovals;
            _ = this.DecidingAgentAgentDecisionRecords;
            _ = this.ReviewedByAgentAgentDecisionRecords;
            _ = this.Attestations;
            _ = this.AppUsers;
            _ = this.SeekerKnowledgeBrokerLinks;
            _ = this.BrokerKnowledgeBrokerLinks;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
