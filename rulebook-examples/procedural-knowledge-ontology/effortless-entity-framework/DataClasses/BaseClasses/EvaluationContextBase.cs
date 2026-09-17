
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
    [Table("EvaluationContexts")]
    public class EvaluationContextBase : SoAEntityBase
    {
        [Key]
        public string EvaluationContextId { get; set; }

        // Formula Name (rulebook: ={{Label}} & " @ " & {{AsOfInstant}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Label)), F.S(" @ "), F.DatetimeText(F.Of(this.AsOfInstant))))); set { }
        }

        public string? Label { get; set; }
        public DateTimeOffset AsOfInstant { get; set; }
        public bool? IsCurrent { get; set; }
        public string? Rationale { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<OntologyProfile> _ontologyProfiles;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<OntologyProfile> OntologyProfiles
        {
            get
            {
                if (_ontologyProfiles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OntologyProfiles - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _ontologyProfiles = new ObservableCollection<OntologyProfile>();
                    }
                    else
                    {
                        var items = base.SoAContext.OntologyProfiles.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<OntologyProfile>();
                        _ontologyProfiles = new ObservableCollection<OntologyProfile>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _ontologyProfiles.CollectionChanged += OntologyProfiles_CollectionChanged;
                }
                return _ontologyProfiles;
            }
            private set
            {
                if (_ontologyProfiles != null)
                {
                    _ontologyProfiles.CollectionChanged -= OntologyProfiles_CollectionChanged;
                }
                _ontologyProfiles = value;
                if (_ontologyProfiles != null)
                {
                    _ontologyProfiles.CollectionChanged += OntologyProfiles_CollectionChanged;
                }
            }
        }

        private void OntologyProfiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<OntologyProfile>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<RoleAssignment> _roleAssignments;

        [InverseProperty("EvaluationContextRef")]
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
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignments.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<RoleAssignment>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<Mentorship> _mentorships;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<Mentorship> Mentorships
        {
            get
            {
                if (_mentorships == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Mentorships - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _mentorships = new ObservableCollection<Mentorship>();
                    }
                    else
                    {
                        var items = base.SoAContext.Mentorships.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<Mentorship>();
                        _mentorships = new ObservableCollection<Mentorship>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ProcedureVersion> _procedureVersions;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<ProcedureVersion> ProcedureVersions
        {
            get
            {
                if (_procedureVersions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersions - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _procedureVersions = new ObservableCollection<ProcedureVersion>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureVersions.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ProcedureVersion>();
                        _procedureVersions = new ObservableCollection<ProcedureVersion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ElicitationSession> _elicitationSessions;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<ElicitationSession> ElicitationSessions
        {
            get
            {
                if (_elicitationSessions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ElicitationSessions - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _elicitationSessions = new ObservableCollection<ElicitationSession>();
                    }
                    else
                    {
                        var items = base.SoAContext.ElicitationSessions.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ElicitationSession>();
                        _elicitationSessions = new ObservableCollection<ElicitationSession>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<KnowledgeFragment> _knowledgeFragments;

        [InverseProperty("EvaluationContextRef")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeFragments.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<KnowledgeFragment>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<KnowledgeGap> _knowledgeGaps;

        [InverseProperty("EvaluationContextRef")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeGaps - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeGaps.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<KnowledgeGap>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<StewardshipAssignment> _stewardshipAssignments;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<StewardshipAssignment> StewardshipAssignments
        {
            get
            {
                if (_stewardshipAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StewardshipAssignments - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _stewardshipAssignments = new ObservableCollection<StewardshipAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.StewardshipAssignments.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<StewardshipAssignment>();
                        _stewardshipAssignments = new ObservableCollection<StewardshipAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ChangeRequest> _changeRequests;

        [InverseProperty("EvaluationContextRef")]
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
                            throw new InvalidOperationException("Cannot access ChangeRequests - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _changeRequests = new ObservableCollection<ChangeRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeRequests.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ChangeRequest>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ReviewEvent> _reviewEvents;

        [InverseProperty("EvaluationContextRef")]
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
                            throw new InvalidOperationException("Cannot access ReviewEvents - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _reviewEvents = new ObservableCollection<ReviewEvent>();
                    }
                    else
                    {
                        var items = base.SoAContext.ReviewEvents.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ReviewEvent>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<OperationalBinding> _operationalBindings;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<OperationalBinding> OperationalBindings
        {
            get
            {
                if (_operationalBindings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OperationalBindings - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _operationalBindings = new ObservableCollection<OperationalBinding>();
                    }
                    else
                    {
                        var items = base.SoAContext.OperationalBindings.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<OperationalBinding>();
                        _operationalBindings = new ObservableCollection<OperationalBinding>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<MessageDelivery> _messageDeliveries;

        [InverseProperty("EvaluationContextRef")]
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
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = base.SoAContext.MessageDeliveries.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<MessageDelivery>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<SendIntent> _sendIntents;

        [InverseProperty("EvaluationContextRef")]
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
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = base.SoAContext.SendIntents.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<SendIntent>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<AuthorityBoundary> _authorityBoundaries;

        [InverseProperty("EvaluationContextRef")]
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
                            throw new InvalidOperationException("Cannot access AuthorityBoundaries - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>();
                    }
                    else
                    {
                        var items = base.SoAContext.AuthorityBoundaries.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<AuthorityBoundary>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ProcessMiningRun> _processMiningRuns;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<ProcessMiningRun> ProcessMiningRuns
        {
            get
            {
                if (_processMiningRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessMiningRuns - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _processMiningRuns = new ObservableCollection<ProcessMiningRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessMiningRuns.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ProcessMiningRun>();
                        _processMiningRuns = new ObservableCollection<ProcessMiningRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<KnowledgeBrokerLink> _knowledgeBrokerLinks;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<KnowledgeBrokerLink> KnowledgeBrokerLinks
        {
            get
            {
                if (_knowledgeBrokerLinks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeBrokerLinks - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeBrokerLinks.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<KnowledgeBrokerLink>();
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ExternalStandardTerm> _externalStandardTerms;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<ExternalStandardTerm> ExternalStandardTerms
        {
            get
            {
                if (_externalStandardTerms == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExternalStandardTerms - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _externalStandardTerms = new ObservableCollection<ExternalStandardTerm>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExternalStandardTerms.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ExternalStandardTerm>();
                        _externalStandardTerms = new ObservableCollection<ExternalStandardTerm>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _externalStandardTerms.CollectionChanged += ExternalStandardTerms_CollectionChanged;
                }
                return _externalStandardTerms;
            }
            private set
            {
                if (_externalStandardTerms != null)
                {
                    _externalStandardTerms.CollectionChanged -= ExternalStandardTerms_CollectionChanged;
                }
                _externalStandardTerms = value;
                if (_externalStandardTerms != null)
                {
                    _externalStandardTerms.CollectionChanged += ExternalStandardTerms_CollectionChanged;
                }
            }
        }

        private void ExternalStandardTerms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExternalStandardTerm>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<AiAdoptionInitiatif> _aiAdoptionInitiatives;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<AiAdoptionInitiatif> AiAdoptionInitiatives
        {
            get
            {
                if (_aiAdoptionInitiatives == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiAdoptionInitiatives - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _aiAdoptionInitiatives = new ObservableCollection<AiAdoptionInitiatif>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiAdoptionInitiatives.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<AiAdoptionInitiatif>();
                        _aiAdoptionInitiatives = new ObservableCollection<AiAdoptionInitiatif>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiAdoptionInitiatives.CollectionChanged += AiAdoptionInitiatives_CollectionChanged;
                }
                return _aiAdoptionInitiatives;
            }
            private set
            {
                if (_aiAdoptionInitiatives != null)
                {
                    _aiAdoptionInitiatives.CollectionChanged -= AiAdoptionInitiatives_CollectionChanged;
                }
                _aiAdoptionInitiatives = value;
                if (_aiAdoptionInitiatives != null)
                {
                    _aiAdoptionInitiatives.CollectionChanged += AiAdoptionInitiatives_CollectionChanged;
                }
            }
        }

        private void AiAdoptionInitiatives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiAdoptionInitiatif>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<GovernedModel> _governedModels;

        [InverseProperty("EvaluationContextRef")]
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
                            throw new InvalidOperationException("Cannot access GovernedModels - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _governedModels = new ObservableCollection<GovernedModel>();
                    }
                    else
                    {
                        var items = base.SoAContext.GovernedModels.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<GovernedModel>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ModelCharter> _modelCharters;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<ModelCharter> ModelCharters
        {
            get
            {
                if (_modelCharters == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelCharters - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _modelCharters = new ObservableCollection<ModelCharter>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelCharters.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ModelCharter>();
                        _modelCharters = new ObservableCollection<ModelCharter>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelCharters.CollectionChanged += ModelCharters_CollectionChanged;
                }
                return _modelCharters;
            }
            private set
            {
                if (_modelCharters != null)
                {
                    _modelCharters.CollectionChanged -= ModelCharters_CollectionChanged;
                }
                _modelCharters = value;
                if (_modelCharters != null)
                {
                    _modelCharters.CollectionChanged += ModelCharters_CollectionChanged;
                }
            }
        }

        private void ModelCharters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelCharter>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ExternalDependencyRevision> _externalDependencyRevisions;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<ExternalDependencyRevision> ExternalDependencyRevisions
        {
            get
            {
                if (_externalDependencyRevisions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExternalDependencyRevisions - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _externalDependencyRevisions = new ObservableCollection<ExternalDependencyRevision>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExternalDependencyRevisions.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ExternalDependencyRevision>();
                        _externalDependencyRevisions = new ObservableCollection<ExternalDependencyRevision>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _externalDependencyRevisions.CollectionChanged += ExternalDependencyRevisions_CollectionChanged;
                }
                return _externalDependencyRevisions;
            }
            private set
            {
                if (_externalDependencyRevisions != null)
                {
                    _externalDependencyRevisions.CollectionChanged -= ExternalDependencyRevisions_CollectionChanged;
                }
                _externalDependencyRevisions = value;
                if (_externalDependencyRevisions != null)
                {
                    _externalDependencyRevisions.CollectionChanged += ExternalDependencyRevisions_CollectionChanged;
                }
            }
        }

        private void ExternalDependencyRevisions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExternalDependencyRevision>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<StakeholderQuestion> _stakeholderQuestions;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<StakeholderQuestion> StakeholderQuestions
        {
            get
            {
                if (_stakeholderQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StakeholderQuestions - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _stakeholderQuestions = new ObservableCollection<StakeholderQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderQuestions.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<StakeholderQuestion>();
                        _stakeholderQuestions = new ObservableCollection<StakeholderQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stakeholderQuestions.CollectionChanged += StakeholderQuestions_CollectionChanged;
                }
                return _stakeholderQuestions;
            }
            private set
            {
                if (_stakeholderQuestions != null)
                {
                    _stakeholderQuestions.CollectionChanged -= StakeholderQuestions_CollectionChanged;
                }
                _stakeholderQuestions = value;
                if (_stakeholderQuestions != null)
                {
                    _stakeholderQuestions.CollectionChanged += StakeholderQuestions_CollectionChanged;
                }
            }
        }

        private void StakeholderQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StakeholderQuestion>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<CompetencyQuestionSetEntry> _competencyQuestionSetEntries;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<CompetencyQuestionSetEntry> CompetencyQuestionSetEntries
        {
            get
            {
                if (_competencyQuestionSetEntries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CompetencyQuestionSetEntries - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _competencyQuestionSetEntries = new ObservableCollection<CompetencyQuestionSetEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.CompetencyQuestionSetEntries.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<CompetencyQuestionSetEntry>();
                        _competencyQuestionSetEntries = new ObservableCollection<CompetencyQuestionSetEntry>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _competencyQuestionSetEntries.CollectionChanged += CompetencyQuestionSetEntries_CollectionChanged;
                }
                return _competencyQuestionSetEntries;
            }
            private set
            {
                if (_competencyQuestionSetEntries != null)
                {
                    _competencyQuestionSetEntries.CollectionChanged -= CompetencyQuestionSetEntries_CollectionChanged;
                }
                _competencyQuestionSetEntries = value;
                if (_competencyQuestionSetEntries != null)
                {
                    _competencyQuestionSetEntries.CollectionChanged += CompetencyQuestionSetEntries_CollectionChanged;
                }
            }
        }

        private void CompetencyQuestionSetEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CompetencyQuestionSetEntry>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<AiModelDeployment> _aiModelDeployments;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<AiModelDeployment> AiModelDeployments
        {
            get
            {
                if (_aiModelDeployments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiModelDeployments - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _aiModelDeployments = new ObservableCollection<AiModelDeployment>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiModelDeployments.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<AiModelDeployment>();
                        _aiModelDeployments = new ObservableCollection<AiModelDeployment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiModelDeployments.CollectionChanged += AiModelDeployments_CollectionChanged;
                }
                return _aiModelDeployments;
            }
            private set
            {
                if (_aiModelDeployments != null)
                {
                    _aiModelDeployments.CollectionChanged -= AiModelDeployments_CollectionChanged;
                }
                _aiModelDeployments = value;
                if (_aiModelDeployments != null)
                {
                    _aiModelDeployments.CollectionChanged += AiModelDeployments_CollectionChanged;
                }
            }
        }

        private void AiModelDeployments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiModelDeployment>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<AiAgentAccountability> _aiAgentAccountabilities;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<AiAgentAccountability> AiAgentAccountabilities
        {
            get
            {
                if (_aiAgentAccountabilities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiAgentAccountabilities - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _aiAgentAccountabilities = new ObservableCollection<AiAgentAccountability>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiAgentAccountabilities.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<AiAgentAccountability>();
                        _aiAgentAccountabilities = new ObservableCollection<AiAgentAccountability>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiAgentAccountabilities.CollectionChanged += AiAgentAccountabilities_CollectionChanged;
                }
                return _aiAgentAccountabilities;
            }
            private set
            {
                if (_aiAgentAccountabilities != null)
                {
                    _aiAgentAccountabilities.CollectionChanged -= AiAgentAccountabilities_CollectionChanged;
                }
                _aiAgentAccountabilities = value;
                if (_aiAgentAccountabilities != null)
                {
                    _aiAgentAccountabilities.CollectionChanged += AiAgentAccountabilities_CollectionChanged;
                }
            }
        }

        private void AiAgentAccountabilities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiAgentAccountability>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<RoleAssignmentUpdateTask> _roleAssignmentUpdateTasks;

        [InverseProperty("EvaluationContextRef")]
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
                            throw new InvalidOperationException("Cannot access RoleAssignmentUpdateTasks - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _roleAssignmentUpdateTasks = new ObservableCollection<RoleAssignmentUpdateTask>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignmentUpdateTasks.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<RoleAssignmentUpdateTask>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<KnowHowCarrier> _knowHowCarriers;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<KnowHowCarrier> KnowHowCarriers
        {
            get
            {
                if (_knowHowCarriers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowHowCarriers - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _knowHowCarriers = new ObservableCollection<KnowHowCarrier>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowHowCarriers.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<KnowHowCarrier>();
                        _knowHowCarriers = new ObservableCollection<KnowHowCarrier>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowHowCarriers.CollectionChanged += KnowHowCarriers_CollectionChanged;
                }
                return _knowHowCarriers;
            }
            private set
            {
                if (_knowHowCarriers != null)
                {
                    _knowHowCarriers.CollectionChanged -= KnowHowCarriers_CollectionChanged;
                }
                _knowHowCarriers = value;
                if (_knowHowCarriers != null)
                {
                    _knowHowCarriers.CollectionChanged += KnowHowCarriers_CollectionChanged;
                }
            }
        }

        private void KnowHowCarriers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowHowCarrier>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<KnowledgeRepositoryEntry> _knowledgeRepositoryEntries;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<KnowledgeRepositoryEntry> KnowledgeRepositoryEntries
        {
            get
            {
                if (_knowledgeRepositoryEntries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeRepositoryEntries - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _knowledgeRepositoryEntries = new ObservableCollection<KnowledgeRepositoryEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeRepositoryEntries.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<KnowledgeRepositoryEntry>();
                        _knowledgeRepositoryEntries = new ObservableCollection<KnowledgeRepositoryEntry>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeRepositoryEntries.CollectionChanged += KnowledgeRepositoryEntries_CollectionChanged;
                }
                return _knowledgeRepositoryEntries;
            }
            private set
            {
                if (_knowledgeRepositoryEntries != null)
                {
                    _knowledgeRepositoryEntries.CollectionChanged -= KnowledgeRepositoryEntries_CollectionChanged;
                }
                _knowledgeRepositoryEntries = value;
                if (_knowledgeRepositoryEntries != null)
                {
                    _knowledgeRepositoryEntries.CollectionChanged += KnowledgeRepositoryEntries_CollectionChanged;
                }
            }
        }

        private void KnowledgeRepositoryEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeRepositoryEntry>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<OnboardingRecord> _onboardingRecords;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<OnboardingRecord> OnboardingRecords
        {
            get
            {
                if (_onboardingRecords == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OnboardingRecords - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _onboardingRecords = new ObservableCollection<OnboardingRecord>();
                    }
                    else
                    {
                        var items = base.SoAContext.OnboardingRecords.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<OnboardingRecord>();
                        _onboardingRecords = new ObservableCollection<OnboardingRecord>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _onboardingRecords.CollectionChanged += OnboardingRecords_CollectionChanged;
                }
                return _onboardingRecords;
            }
            private set
            {
                if (_onboardingRecords != null)
                {
                    _onboardingRecords.CollectionChanged -= OnboardingRecords_CollectionChanged;
                }
                _onboardingRecords = value;
                if (_onboardingRecords != null)
                {
                    _onboardingRecords.CollectionChanged += OnboardingRecords_CollectionChanged;
                }
            }
        }

        private void OnboardingRecords_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<OnboardingRecord>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<CollectionOccasion> _collectionOccasions;

        [InverseProperty("EvaluationContextRef")]
        public virtual ObservableCollection<CollectionOccasion> CollectionOccasions
        {
            get
            {
                if (_collectionOccasions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CollectionOccasions - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _collectionOccasions = new ObservableCollection<CollectionOccasion>();
                    }
                    else
                    {
                        var items = base.SoAContext.CollectionOccasions.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<CollectionOccasion>();
                        _collectionOccasions = new ObservableCollection<CollectionOccasion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _collectionOccasions.CollectionChanged += CollectionOccasions_CollectionChanged;
                }
                return _collectionOccasions;
            }
            private set
            {
                if (_collectionOccasions != null)
                {
                    _collectionOccasions.CollectionChanged -= CollectionOccasions_CollectionChanged;
                }
                _collectionOccasions = value;
                if (_collectionOccasions != null)
                {
                    _collectionOccasions.CollectionChanged += CollectionOccasions_CollectionChanged;
                }
            }
        }

        private void CollectionOccasions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CollectionOccasion>())
                {
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OntologyProfiles;
            _ = this.RoleAssignments;
            _ = this.Mentorships;
            _ = this.ProcedureVersions;
            _ = this.ElicitationSessions;
            _ = this.KnowledgeFragments;
            _ = this.KnowledgeGaps;
            _ = this.StewardshipAssignments;
            _ = this.ChangeRequests;
            _ = this.ReviewEvents;
            _ = this.OperationalBindings;
            _ = this.MessageDeliveries;
            _ = this.SendIntents;
            _ = this.AuthorityBoundaries;
            _ = this.ProcessMiningRuns;
            _ = this.KnowledgeBrokerLinks;
            _ = this.ExternalStandardTerms;
            _ = this.AiAdoptionInitiatives;
            _ = this.GovernedModels;
            _ = this.ModelCharters;
            _ = this.ExternalDependencyRevisions;
            _ = this.StakeholderQuestions;
            _ = this.CompetencyQuestionSetEntries;
            _ = this.AiModelDeployments;
            _ = this.AiAgentAccountabilities;
            _ = this.RoleAssignmentUpdateTasks;
            _ = this.KnowHowCarriers;
            _ = this.KnowledgeRepositoryEntries;
            _ = this.OnboardingRecords;
            _ = this.CollectionOccasions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
