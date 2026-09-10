
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Agents")]
    public class AgentBase : SoAEntityBase
    {
        [Key]
        public string AgentId { get; set; }

        // Formula Name (rulebook: ={{DisplayName}})
        public string? Name
        {
            get => this.DisplayName; set { }
        }

        public string? DisplayName { get; set; }
        public string? AgentKind { get; set; }
        public string? ContactAddress { get; set; }
        public string? VersionOrEmploymentKey { get; set; }
        // Formula CountOfCurrentRoleAssignments (rulebook: =COUNTIFS(RoleAssignments!{{CurrentAgentKey}}, {{AgentId}}))
        public int? CountOfCurrentRoleAssignments
        {
            get => COUNTIFS(RoleAssignments!this.CurrentAgentKey, this.AgentId); set { }
        }

        // Formula IsStillEngaged (rulebook: ={{CountOfCurrentRoleAssignments}} > 0)
        public bool? IsStillEngaged
        {
            get => this.CountOfCurrentRoleAssignments > 0; set { }
        }

        // Formula DecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{DecidingAgent}}, {{AgentId}}))
        public decimal? DecisionCount
        {
            get => COUNTIFS(AgentDecisionRecords!this.DecidingAgent, this.AgentId); set { }
        }

        // Formula OverriddenDecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{DecidingAgentWhenOverridden}}, {{AgentId}}))
        public decimal? OverriddenDecisionCount
        {
            get => COUNTIFS(AgentDecisionRecords!this.DecidingAgentWhenOverridden, this.AgentId); set { }
        }

        // Formula OverrideRatePercent (rulebook: =IF({{DecisionCount}} = 0, 0, ({{OverriddenDecisionCount}} * 100) / {{DecisionCount}}))
        public decimal? OverrideRatePercent
        {
            get => IF(this.DecisionCount = 0, 0, (this.OverriddenDecisionCount * 100) / this.DecisionCount); set { }
        }

        // Formula IsNonHuman (rulebook: =NOT({{AgentKind}} = "Human"))
        public bool? IsNonHuman
        {
            get => NOT(this.AgentKind = "Human"); set { }
        }

        // Formula BoundaryViolationCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{AgentWhenBoundaryViolated}}, {{AgentId}}))
        public decimal? BoundaryViolationCount
        {
            get => COUNTIFS(AgentDecisionRecords!this.AgentWhenBoundaryViolated, this.AgentId); set { }
        }

        // Formula IsOperatingOutsideBoundary (rulebook: ={{BoundaryViolationCount}} > 0)
        public bool? IsOperatingOutsideBoundary
        {
            get => this.BoundaryViolationCount > 0; set { }
        }

        // Formula DraftDecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{AgentWhenDraft}}, {{AgentId}}))
        public decimal? DraftDecisionCount
        {
            get => COUNTIFS(AgentDecisionRecords!this.AgentWhenDraft, this.AgentId); set { }
        }

        // Formula OverriddenDraftCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{AgentWhenDraftOverridden}}, {{AgentId}}))
        public decimal? OverriddenDraftCount
        {
            get => COUNTIFS(AgentDecisionRecords!this.AgentWhenDraftOverridden, this.AgentId); set { }
        }

        // Formula DraftRewriteRatePercent (rulebook: =IF({{DraftDecisionCount}} = 0, 0, ({{OverriddenDraftCount}} * 100) / {{DraftDecisionCount}}))
        public decimal? DraftRewriteRatePercent
        {
            get => IF(this.DraftDecisionCount = 0, 0, (this.OverriddenDraftCount * 100) / this.DraftDecisionCount); set { }
        }

        // Formula TimesNamedAsBroker (rulebook: =COUNTIFS(KnowledgeBrokerLinks!{{ActiveRelianceBrokerKey}}, {{AgentId}}))
        public decimal? TimesNamedAsBroker
        {
            get => COUNTIFS(KnowledgeBrokerLinks!this.ActiveRelianceBrokerKey, this.AgentId); set { }
        }

        // Formula IsRecognizedBroker (rulebook: ={{TimesNamedAsBroker}} >= 3)
        public bool? IsRecognizedBroker
        {
            get => this.TimesNamedAsBroker >= 3; set { }
        }

        // Formula AtRiskRelianceCount (rulebook: =COUNTIFS(KnowledgeBrokerLinks!{{AtRiskBrokerKey}}, {{AgentId}}))
        public decimal? AtRiskRelianceCount
        {
            get => COUNTIFS(KnowledgeBrokerLinks!this.AtRiskBrokerKey, this.AgentId); set { }
        }

        // Formula HasAtRiskKnowledgeReliance (rulebook: ={{AtRiskRelianceCount}} > 0)
        public bool? HasAtRiskKnowledgeReliance
        {
            get => this.AtRiskRelianceCount > 0; set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }

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

        private ObservableCollection<Role> _roles;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<Role> Roles
        {
            get
            {
                if (_roles == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Roles - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _roles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = Context.Roles.Where(x => x.CurrentAgent == this.AgentId).ToList<Role>();
                        _roles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = Context.RoleAssignments.Where(x => x.Agent == this.AgentId).ToList<RoleAssignment>();
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
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<Mentorship> _mentorships;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<Mentorship> Mentorships
        {
            get
            {
                if (_mentorships == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Mentorships - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _mentorships = new ObservableCollection<Mentorship>();
                    }
                    else
                    {
                        var items = Context.Mentorships.Where(x => x.MentorAgent == this.AgentId).ToList<Mentorship>();
                        _mentorships = new ObservableCollection<Mentorship>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _mentorships.CollectionChanged += Mentorships_CollectionChanged;
                }
                return _mentorships;
            }
            private set
            {
                if (_mentorships != null)
                {
                    _mentorships.CollectionChanged -= Mentorships_CollectionChanged;
                }
                _mentorships = value;
                if (_mentorships != null)
                {
                    _mentorships.CollectionChanged += Mentorships_CollectionChanged;
                }
            }
        }

        private void Mentorships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Mentorship>())
                {
                    item.MentorAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<Mentorship> _mentorships;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<Mentorship> Mentorships
        {
            get
            {
                if (_mentorships == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Mentorships - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _mentorships = new ObservableCollection<Mentorship>();
                    }
                    else
                    {
                        var items = Context.Mentorships.Where(x => x.LearnerAgent == this.AgentId).ToList<Mentorship>();
                        _mentorships = new ObservableCollection<Mentorship>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _mentorships.CollectionChanged += Mentorships_CollectionChanged;
                }
                return _mentorships;
            }
            private set
            {
                if (_mentorships != null)
                {
                    _mentorships.CollectionChanged -= Mentorships_CollectionChanged;
                }
                _mentorships = value;
                if (_mentorships != null)
                {
                    _mentorships.CollectionChanged += Mentorships_CollectionChanged;
                }
            }
        }

        private void Mentorships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Mentorship>())
                {
                    item.LearnerAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcedureVersion> _procedureVersions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ProcedureVersion> ProcedureVersions
        {
            get
            {
                if (_procedureVersions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _procedureVersions = new ObservableCollection<ProcedureVersion>();
                    }
                    else
                    {
                        var items = Context.ProcedureVersions.Where(x => x.CreatedByAgent == this.AgentId).ToList<ProcedureVersion>();
                        _procedureVersions = new ObservableCollection<ProcedureVersion>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _procedureVersions.CollectionChanged += ProcedureVersions_CollectionChanged;
                }
                return _procedureVersions;
            }
            private set
            {
                if (_procedureVersions != null)
                {
                    _procedureVersions.CollectionChanged -= ProcedureVersions_CollectionChanged;
                }
                _procedureVersions = value;
                if (_procedureVersions != null)
                {
                    _procedureVersions.CollectionChanged += ProcedureVersions_CollectionChanged;
                }
            }
        }

        private void ProcedureVersions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureVersion>())
                {
                    item.CreatedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcedureVersion> _procedureVersions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ProcedureVersion> ProcedureVersions
        {
            get
            {
                if (_procedureVersions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _procedureVersions = new ObservableCollection<ProcedureVersion>();
                    }
                    else
                    {
                        var items = Context.ProcedureVersions.Where(x => x.ModifiedByAgent == this.AgentId).ToList<ProcedureVersion>();
                        _procedureVersions = new ObservableCollection<ProcedureVersion>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _procedureVersions.CollectionChanged += ProcedureVersions_CollectionChanged;
                }
                return _procedureVersions;
            }
            private set
            {
                if (_procedureVersions != null)
                {
                    _procedureVersions.CollectionChanged -= ProcedureVersions_CollectionChanged;
                }
                _procedureVersions = value;
                if (_procedureVersions != null)
                {
                    _procedureVersions.CollectionChanged += ProcedureVersions_CollectionChanged;
                }
            }
        }

        private void ProcedureVersions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureStatusChanges - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _procedureStatusChanges = new ObservableCollection<ProcedureStatusChange>();
                    }
                    else
                    {
                        var items = Context.ProcedureStatusChanges.Where(x => x.ChangedByAgent == this.AgentId).ToList<ProcedureStatusChange>();
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
                    item.ChangedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ElicitationSession> _elicitationSessions;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access ElicitationSessions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _elicitationSessions = new ObservableCollection<ElicitationSession>();
                    }
                    else
                    {
                        var items = Context.ElicitationSessions.Where(x => x.PractitionerAgent == this.AgentId).ToList<ElicitationSession>();
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
                    item.PractitionerAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ElicitationSession> _elicitationSessions;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access ElicitationSessions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _elicitationSessions = new ObservableCollection<ElicitationSession>();
                    }
                    else
                    {
                        var items = Context.ElicitationSessions.Where(x => x.FacilitatorAgent == this.AgentId).ToList<ElicitationSession>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = Context.KnowledgeFragments.Where(x => x.SourceAgent == this.AgentId).ToList<KnowledgeFragment>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecutions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>();
                    }
                    else
                    {
                        var items = Context.ProcedureExecutions.Where(x => x.ExecutedByAgent == this.AgentId).ToList<ProcedureExecution>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecutions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _stepExecutions = new ObservableCollection<StepExecution>();
                    }
                    else
                    {
                        var items = Context.StepExecutions.Where(x => x.ExecutedByAgent == this.AgentId).ToList<StepExecution>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequirementSatisfactions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>();
                    }
                    else
                    {
                        var items = Context.RequirementSatisfactions.Where(x => x.EvaluatedByAgent == this.AgentId).ToList<RequirementSatisfaction>();
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IssueOccurrences - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>();
                    }
                    else
                    {
                        var items = Context.IssueOccurrences.Where(x => x.EncounteredByAgent == this.AgentId).ToList<IssueOccurrence>();
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserQuestions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _userQuestions = new ObservableCollection<UserQuestion>();
                    }
                    else
                    {
                        var items = Context.UserQuestions.Where(x => x.AskedByAgent == this.AgentId).ToList<UserQuestion>();
                        _userQuestions = new ObservableCollection<UserQuestion>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserFeedback - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _userFeedback = new ObservableCollection<UserFeedback>();
                    }
                    else
                    {
                        var items = Context.UserFeedback.Where(x => x.ProvidedByAgent == this.AgentId).ToList<UserFeedback>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeRequests - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _changeRequests = new ObservableCollection<ChangeRequest>();
                    }
                    else
                    {
                        var items = Context.ChangeRequests.Where(x => x.RequestedByAgent == this.AgentId).ToList<ChangeRequest>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ReviewEvents - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _reviewEvents = new ObservableCollection<ReviewEvent>();
                    }
                    else
                    {
                        var items = Context.ReviewEvents.Where(x => x.ReviewedByAgent == this.AgentId).ToList<ReviewEvent>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LearningActivities - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _learningActivities = new ObservableCollection<LearningActivity>();
                    }
                    else
                    {
                        var items = Context.LearningActivities.Where(x => x.FacilitatorAgent == this.AgentId).ToList<LearningActivity>();
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
                    item.FacilitatorAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ExceptionInvocation> _exceptionInvocations;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ExceptionInvocation> ExceptionInvocations
        {
            get
            {
                if (_exceptionInvocations == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExceptionInvocations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _exceptionInvocations = new ObservableCollection<ExceptionInvocation>();
                    }
                    else
                    {
                        var items = Context.ExceptionInvocations.Where(x => x.InvokedByAgent == this.AgentId).ToList<ExceptionInvocation>();
                        _exceptionInvocations = new ObservableCollection<ExceptionInvocation>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _exceptionInvocations.CollectionChanged += ExceptionInvocations_CollectionChanged;
                }
                return _exceptionInvocations;
            }
            private set
            {
                if (_exceptionInvocations != null)
                {
                    _exceptionInvocations.CollectionChanged -= ExceptionInvocations_CollectionChanged;
                }
                _exceptionInvocations = value;
                if (_exceptionInvocations != null)
                {
                    _exceptionInvocations.CollectionChanged += ExceptionInvocations_CollectionChanged;
                }
            }
        }

        private void ExceptionInvocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExceptionInvocation>())
                {
                    item.InvokedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ExceptionInvocation> _exceptionInvocations;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ExceptionInvocation> ExceptionInvocations
        {
            get
            {
                if (_exceptionInvocations == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExceptionInvocations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _exceptionInvocations = new ObservableCollection<ExceptionInvocation>();
                    }
                    else
                    {
                        var items = Context.ExceptionInvocations.Where(x => x.ApprovedByAgent == this.AgentId).ToList<ExceptionInvocation>();
                        _exceptionInvocations = new ObservableCollection<ExceptionInvocation>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _exceptionInvocations.CollectionChanged += ExceptionInvocations_CollectionChanged;
                }
                return _exceptionInvocations;
            }
            private set
            {
                if (_exceptionInvocations != null)
                {
                    _exceptionInvocations.CollectionChanged -= ExceptionInvocations_CollectionChanged;
                }
                _exceptionInvocations = value;
                if (_exceptionInvocations != null)
                {
                    _exceptionInvocations.CollectionChanged += ExceptionInvocations_CollectionChanged;
                }
            }
        }

        private void ExceptionInvocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VerificationOutcomes - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>();
                    }
                    else
                    {
                        var items = Context.VerificationOutcomes.Where(x => x.ObservedByAgent == this.AgentId).ToList<VerificationOutcome>();
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = Context.MessageDeliveries.Where(x => x.SentByAgent == this.AgentId).ToList<MessageDelivery>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TemplateApprovals - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _templateApprovals = new ObservableCollection<TemplateApproval>();
                    }
                    else
                    {
                        var items = Context.TemplateApprovals.Where(x => x.DecidedByAgent == this.AgentId).ToList<TemplateApproval>();
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
                    item.DecidedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AgentDecisionRecord> _agentDecisionRecords;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access AgentDecisionRecords - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _agentDecisionRecords = new ObservableCollection<AgentDecisionRecord>();
                    }
                    else
                    {
                        var items = Context.AgentDecisionRecords.Where(x => x.DecidingAgent == this.AgentId).ToList<AgentDecisionRecord>();
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
                    item.DecidingAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AgentDecisionRecord> _agentDecisionRecords;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access AgentDecisionRecords - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _agentDecisionRecords = new ObservableCollection<AgentDecisionRecord>();
                    }
                    else
                    {
                        var items = Context.AgentDecisionRecords.Where(x => x.ReviewedByAgent == this.AgentId).ToList<AgentDecisionRecord>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Attestations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _attestations = new ObservableCollection<Attestation>();
                    }
                    else
                    {
                        var items = Context.Attestations.Where(x => x.SignedByAgent == this.AgentId).ToList<Attestation>();
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppUsers - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _appUsers = new ObservableCollection<AppUser>();
                    }
                    else
                    {
                        var items = Context.AppUsers.Where(x => x.LinkedAgent == this.AgentId).ToList<AppUser>();
                        _appUsers = new ObservableCollection<AppUser>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        private ObservableCollection<KnowledgeBrokerLink> _knowledgeBrokerLinks;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<KnowledgeBrokerLink> KnowledgeBrokerLinks
        {
            get
            {
                if (_knowledgeBrokerLinks == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeBrokerLinks - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = Context.KnowledgeBrokerLinks.Where(x => x.Seeker == this.AgentId).ToList<KnowledgeBrokerLink>();
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _knowledgeBrokerLinks.CollectionChanged += KnowledgeBrokerLinks_CollectionChanged;
                }
                return _knowledgeBrokerLinks;
            }
            private set
            {
                if (_knowledgeBrokerLinks != null)
                {
                    _knowledgeBrokerLinks.CollectionChanged -= KnowledgeBrokerLinks_CollectionChanged;
                }
                _knowledgeBrokerLinks = value;
                if (_knowledgeBrokerLinks != null)
                {
                    _knowledgeBrokerLinks.CollectionChanged += KnowledgeBrokerLinks_CollectionChanged;
                }
            }
        }

        private void KnowledgeBrokerLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeBrokerLink>())
                {
                    item.Seeker = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeBrokerLink> _knowledgeBrokerLinks;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<KnowledgeBrokerLink> KnowledgeBrokerLinks
        {
            get
            {
                if (_knowledgeBrokerLinks == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeBrokerLinks - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = Context.KnowledgeBrokerLinks.Where(x => x.Broker == this.AgentId).ToList<KnowledgeBrokerLink>();
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _knowledgeBrokerLinks.CollectionChanged += KnowledgeBrokerLinks_CollectionChanged;
                }
                return _knowledgeBrokerLinks;
            }
            private set
            {
                if (_knowledgeBrokerLinks != null)
                {
                    _knowledgeBrokerLinks.CollectionChanged -= KnowledgeBrokerLinks_CollectionChanged;
                }
                _knowledgeBrokerLinks = value;
                if (_knowledgeBrokerLinks != null)
                {
                    _knowledgeBrokerLinks.CollectionChanged += KnowledgeBrokerLinks_CollectionChanged;
                }
            }
        }

        private void KnowledgeBrokerLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            _ = this.Organization;
            _ = this.Roles;
            _ = this.RoleAssignments;
            _ = this.Mentorships;
            _ = this.Mentorships;
            _ = this.ProcedureVersions;
            _ = this.ProcedureVersions;
            _ = this.ProcedureStatusChanges;
            _ = this.ElicitationSessions;
            _ = this.ElicitationSessions;
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
            _ = this.ExceptionInvocations;
            _ = this.ExceptionInvocations;
            _ = this.VerificationOutcomes;
            _ = this.MessageDeliveries;
            _ = this.TemplateApprovals;
            _ = this.AgentDecisionRecords;
            _ = this.AgentDecisionRecords;
            _ = this.Attestations;
            _ = this.AppUsers;
            _ = this.KnowledgeBrokerLinks;
            _ = this.KnowledgeBrokerLinks;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
