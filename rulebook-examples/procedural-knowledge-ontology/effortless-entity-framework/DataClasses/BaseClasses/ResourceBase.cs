
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Resources")]
    public class ResourceBase : SoAEntityBase
    {
        [Key]
        public string ResourceId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        public string? Name
        {
            get => this.Title; set { }
        }

        public string? Title { get; set; }
        public string? ResourceKind { get; set; }
        public string? ExternalUri { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? Description { get; set; }
        public string? ApprovalStatus { get; set; }
        // Formula IsApprovedSource (rulebook: ={{ApprovalStatus}} = "Approved")
        public bool? IsApprovedSource
        {
            get => this.ApprovalStatus = "Approved"; set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<ProcedureResource> _procedureResources;

        [InverseProperty("Resource")]
        public virtual ObservableCollection<ProcedureResource> ProcedureResources
        {
            get
            {
                if (_procedureResources == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureResources - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _procedureResources = new ObservableCollection<ProcedureResource>();
                    }
                    else
                    {
                        var items = Context.ProcedureResources.Where(x => x.Resource == this.ResourceId).ToList<ProcedureResource>();
                        _procedureResources = new ObservableCollection<ProcedureResource>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _procedureResources.CollectionChanged += ProcedureResources_CollectionChanged;
                }
                return _procedureResources;
            }
            private set
            {
                if (_procedureResources != null)
                {
                    _procedureResources.CollectionChanged -= ProcedureResources_CollectionChanged;
                }
                _procedureResources = value;
                if (_procedureResources != null)
                {
                    _procedureResources.CollectionChanged += ProcedureResources_CollectionChanged;
                }
            }
        }

        private void ProcedureResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureResource>())
                {
                    item.Resource = this.ResourceId;
                }
            }
        }

        private ObservableCollection<UserQuestion> _userQuestions;

        [InverseProperty("Resource")]
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
                            throw new InvalidOperationException("Cannot access UserQuestions - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _userQuestions = new ObservableCollection<UserQuestion>();
                    }
                    else
                    {
                        var items = Context.UserQuestions.Where(x => x.AddressedByResource == this.ResourceId).ToList<UserQuestion>();
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
                    item.AddressedByResource = this.ResourceId;
                }
            }
        }

        private ObservableCollection<LearningActivity> _learningActivities;

        [InverseProperty("Resource")]
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
                            throw new InvalidOperationException("Cannot access LearningActivities - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _learningActivities = new ObservableCollection<LearningActivity>();
                    }
                    else
                    {
                        var items = Context.LearningActivities.Where(x => x.EvidenceResource == this.ResourceId).ToList<LearningActivity>();
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
                    item.EvidenceResource = this.ResourceId;
                }
            }
        }

        private ObservableCollection<OperationalBinding> _operationalBindings;

        [InverseProperty("Resource")]
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
                            throw new InvalidOperationException("Cannot access OperationalBindings - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _operationalBindings = new ObservableCollection<OperationalBinding>();
                    }
                    else
                    {
                        var items = Context.OperationalBindings.Where(x => x.Resource == this.ResourceId).ToList<OperationalBinding>();
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
                    item.Resource = this.ResourceId;
                }
            }
        }

        private ObservableCollection<MessageTemplate> _messageTemplates;

        [InverseProperty("Resource")]
        public virtual ObservableCollection<MessageTemplate> MessageTemplates
        {
            get
            {
                if (_messageTemplates == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageTemplates - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _messageTemplates = new ObservableCollection<MessageTemplate>();
                    }
                    else
                    {
                        var items = Context.MessageTemplates.Where(x => x.Resource == this.ResourceId).ToList<MessageTemplate>();
                        _messageTemplates = new ObservableCollection<MessageTemplate>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _messageTemplates.CollectionChanged += MessageTemplates_CollectionChanged;
                }
                return _messageTemplates;
            }
            private set
            {
                if (_messageTemplates != null)
                {
                    _messageTemplates.CollectionChanged -= MessageTemplates_CollectionChanged;
                }
                _messageTemplates = value;
                if (_messageTemplates != null)
                {
                    _messageTemplates.CollectionChanged += MessageTemplates_CollectionChanged;
                }
            }
        }

        private void MessageTemplates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MessageTemplate>())
                {
                    item.Resource = this.ResourceId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureResources;
            _ = this.UserQuestions;
            _ = this.LearningActivities;
            _ = this.OperationalBindings;
            _ = this.MessageTemplates;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
