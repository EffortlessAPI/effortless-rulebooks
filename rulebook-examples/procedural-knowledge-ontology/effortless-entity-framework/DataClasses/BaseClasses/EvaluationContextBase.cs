
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("EvaluationContexts")]
    public class EvaluationContextBase : SoAEntityBase
    {
        [Key]
        public string EvaluationContextId { get; set; }

        // Formula Name (rulebook: ={{Label}} & " @ " & {{AsOfInstant}})
        public string? Name
        {
            get => this.Label + " @ " + this.AsOfInstant; set { }
        }

        public string? Label { get; set; }
        public DateTime AsOfInstant { get; set; }
        public bool? IsCurrent { get; set; }
        public string? Rationale { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<RoleAssignment> _roleAssignments;

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = Context.RoleAssignments.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<RoleAssignment>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ProcedureVersion> _procedureVersions;

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access ProcedureVersions - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _procedureVersions = new ObservableCollection<ProcedureVersion>();
                    }
                    else
                    {
                        var items = Context.ProcedureVersions.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ProcedureVersion>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ElicitationSession> _elicitationSessions;

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access ElicitationSessions - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _elicitationSessions = new ObservableCollection<ElicitationSession>();
                    }
                    else
                    {
                        var items = Context.ElicitationSessions.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ElicitationSession>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<KnowledgeFragment> _knowledgeFragments;

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = Context.KnowledgeFragments.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<KnowledgeFragment>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<KnowledgeGap> _knowledgeGaps;

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeGaps - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>();
                    }
                    else
                    {
                        var items = Context.KnowledgeGaps.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<KnowledgeGap>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<StewardshipAssignment> _stewardshipAssignments;

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access StewardshipAssignments - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _stewardshipAssignments = new ObservableCollection<StewardshipAssignment>();
                    }
                    else
                    {
                        var items = Context.StewardshipAssignments.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<StewardshipAssignment>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ChangeRequest> _changeRequests;

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access ChangeRequests - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _changeRequests = new ObservableCollection<ChangeRequest>();
                    }
                    else
                    {
                        var items = Context.ChangeRequests.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ChangeRequest>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ReviewEvent> _reviewEvents;

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access ReviewEvents - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _reviewEvents = new ObservableCollection<ReviewEvent>();
                    }
                    else
                    {
                        var items = Context.ReviewEvents.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ReviewEvent>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<OperationalBinding> _operationalBindings;

        [InverseProperty("EvaluationContext")]
        public virtual ObservableCollection<OperationalBinding> OperationalBindings
        {
            get
            {
                if (_operationalBindings == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OperationalBindings - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _operationalBindings = new ObservableCollection<OperationalBinding>();
                    }
                    else
                    {
                        var items = Context.OperationalBindings.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<OperationalBinding>();
                        _operationalBindings = new ObservableCollection<OperationalBinding>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = Context.MessageDeliveries.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<MessageDelivery>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<SendIntent> _sendIntents;

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = Context.SendIntents.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<SendIntent>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<AuthorityBoundary> _authorityBoundaries;

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access AuthorityBoundaries - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>();
                    }
                    else
                    {
                        var items = Context.AuthorityBoundaries.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<AuthorityBoundary>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }

        private ObservableCollection<ProcessMiningRun> _processMiningRuns;

        [InverseProperty("EvaluationContext")]
        public virtual ObservableCollection<ProcessMiningRun> ProcessMiningRuns
        {
            get
            {
                if (_processMiningRuns == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessMiningRuns - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _processMiningRuns = new ObservableCollection<ProcessMiningRun>();
                    }
                    else
                    {
                        var items = Context.ProcessMiningRuns.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<ProcessMiningRun>();
                        _processMiningRuns = new ObservableCollection<ProcessMiningRun>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("EvaluationContext")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeBrokerLinks - no database context is set. EvaluationContextId: " + this.EvaluationContextId + ".");
                        }
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = Context.KnowledgeBrokerLinks.Where(x => x.EvaluationContext == this.EvaluationContextId).ToList<KnowledgeBrokerLink>();
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
                    item.EvaluationContext = this.EvaluationContextId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RoleAssignments;
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
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
