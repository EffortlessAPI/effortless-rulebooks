
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
    [Table("Resources")]
    public class ResourceBase : SoAEntityBase
    {
        [Key]
        public string ResourceId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Title))); set { }
        }

        public string? Title { get; set; }
        public string? ResourceKind { get; set; }
        public string? ExternalUri { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public string? Description { get; set; }
        public string? ApprovalStatus { get; set; }
        // Formula IsApprovedSource (rulebook: ={{ApprovalStatus}} = "Approved")
        [NotMapped]
        public bool? IsApprovedSource
        {
            get => F.AsBool(F.Memo(this, "IsApprovedSource", () => F.Eq(F.Nullif(F.Of(this.ApprovalStatus)), F.S("Approved")))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        public string? Language { get; set; }
        public string? Keywords { get; set; }
        public string? Format { get; set; }
        // Formula SourceModifiedAt (rulebook: =INDEX(Resources!{{ModifiedAt}}, MATCH({{ExtractedFromResource}}, Resources!{{ResourceId}}, 0)))
        [NotMapped]
        public DateTimeOffset? SourceModifiedAt
        {
            get => F.AsDateTime(F.Memo(this, "SourceModifiedAt", () => F.Lookup<Resource>(this, "Resources", "ResourceId", __c => __c.Resources, __r => F.Of(__r.ResourceId), F.Of(this.ExtractedFromResource), __r => F.Of(__r.ModifiedAt), () => F.Of(new Resource().ModifiedAt)))); set { }
        }

        // Formula IsStaleExtraction (rulebook: =AND({{ExtractedFromResource}} <> "", {{SourceModifiedAt}} > {{ModifiedAt}}))
        [NotMapped]
        public bool? IsStaleExtraction
        {
            get => F.AsBool(F.Memo(this, "IsStaleExtraction", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ExtractedFromResource))), F.Bool3(F.Cmp(F.Of(this.SourceModifiedAt), ">", F.Nullif(F.Of(this.ModifiedAt))))))); set { }
        }

        // Formula ReferencingStepCount (rulebook: =COUNTIFS(StepResources!{{Resource}}, {{ResourceId}}))
        [NotMapped]
        public int? ReferencingStepCount
        {
            get => F.AsInt(F.Memo(this, "ReferencingStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepResource>(base.SoAContext, "StepResources", __c => __c.StepResources), __r => F.CritField(F.Of(__r.Resource), F.Of(this.ResourceId))))))); set { }
        }

        // Formula ReferencingVersionCount (rulebook: =COUNTIFS(ProcedureResources!{{Resource}}, {{ResourceId}}))
        [NotMapped]
        public int? ReferencingVersionCount
        {
            get => F.AsInt(F.Memo(this, "ReferencingVersionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureResource>(base.SoAContext, "ProcedureResources", __c => __c.ProcedureResources), __r => F.CritField(F.Of(__r.Resource), F.Of(this.ResourceId))))))); set { }
        }

        // Formula IsUnusedResource (rulebook: =AND({{ReferencingStepCount}} = 0, {{ReferencingVersionCount}} = 0))
        [NotMapped]
        public bool? IsUnusedResource
        {
            get => F.AsBool(F.Memo(this, "IsUnusedResource", () => F.And(F.Bool3(F.Eq(F.Of(this.ReferencingStepCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.ReferencingVersionCount), F.I(0)))))); set { }
        }

        // Formula IsContentWithoutOrganization (rulebook: =AND({{Description}} <> "", OR({{ArtifactTypeConcept}} = "", {{Keywords}} = "", ({{ReferencingStepCount}} + {{ReferencingVersionCount}}) = 0)))
        [NotMapped]
        public bool? IsContentWithoutOrganization
        {
            get => F.AsBool(F.Memo(this, "IsContentWithoutOrganization", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.Description))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ArtifactTypeConcept))), F.Bool3(F.IsBlank(F.Of(this.Keywords))), F.Bool3(F.Eq(F.Add(F.Of(this.ReferencingStepCount), F.Of(this.ReferencingVersionCount)), F.I(0)))))))); set { }
        }

        // Formula TrailingPracticeCount (rulebook: =COUNTIFS(KnowledgeTraces!{{ContradictedDocument}}, {{ResourceId}}, KnowledgeTraces!{{IsDocumentTrailingPractice}}, TRUE))
        [NotMapped]
        public int? TrailingPracticeCount
        {
            get => F.AsInt(F.Memo(this, "TrailingPracticeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTrace>(base.SoAContext, "KnowledgeTraces", __c => __c.KnowledgeTraces), __r => F.CritField(F.Of(__r.ContradictedDocument), F.Of(this.ResourceId)) && F.CritLiteral(F.Of(__r.IsDocumentTrailingPractice), F.B(true))))))); set { }
        }

        // Formula IsBehindCurrentPractice (rulebook: ={{TrailingPracticeCount}} > 0)
        [NotMapped]
        public bool? IsBehindCurrentPractice
        {
            get => F.AsBool(F.Memo(this, "IsBehindCurrentPractice", () => F.Cmp(F.Of(this.TrailingPracticeCount), ">", F.I(0)))); set { }
        }


        public string? CreatedByAgent { get; set; }
        public string? ModifiedByAgent { get; set; }
        public string? ExtractedFromResource { get; set; }
        public string? ArtifactTypeConcept { get; set; }
        public string? CatalogEntryFor { get; set; }
        public string? ComplianceRecordFor { get; set; }

        private Agent _agent;

        [ForeignKey("CreatedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(CreatedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. CreatedByAgent: " + CreatedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(CreatedByAgent);
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
                        CreatedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("ModifiedByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(ModifiedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. ModifiedByAgent: " + ModifiedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(ModifiedByAgent);
                    if (_agentRef != null)
                    {
                        base.SoAContext.Attach(_agentRef);
                    }
                }
                return _agentRef;
            }
            set
            {
                if (_agentRef != value)
                {
                    _agentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRef != null)
                    {
                        ModifiedByAgent = _agentRef.AgentId;
                    }
                }
            }
        }

        private Resource _resource;

        [ForeignKey("ExtractedFromResource")]
        public virtual Resource Resource
        {
            get
            {
                if (_resource == null && !string.IsNullOrEmpty(ExtractedFromResource))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. ExtractedFromResource: " + ExtractedFromResource + ".");
                        }
                        return null;
                    }
                    _resource = base.SoAContext.Resources.Find(ExtractedFromResource);
                    if (_resource != null)
                    {
                        base.SoAContext.Attach(_resource);
                    }
                }
                return _resource;
            }
            set
            {
                if (_resource != value)
                {
                    _resource = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_resource != null)
                    {
                        ExtractedFromResource = _resource.ResourceId;
                    }
                }
            }
        }

        private VocabularyTerm _vocabularyTerm;

        [ForeignKey("ArtifactTypeConcept")]
        public virtual VocabularyTerm VocabularyTerm
        {
            get
            {
                if (_vocabularyTerm == null && !string.IsNullOrEmpty(ArtifactTypeConcept))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerm - no database context is set. ArtifactTypeConcept: " + ArtifactTypeConcept + ".");
                        }
                        return null;
                    }
                    _vocabularyTerm = base.SoAContext.VocabularyTerms.Find(ArtifactTypeConcept);
                    if (_vocabularyTerm != null)
                    {
                        base.SoAContext.Attach(_vocabularyTerm);
                    }
                }
                return _vocabularyTerm;
            }
            set
            {
                if (_vocabularyTerm != value)
                {
                    _vocabularyTerm = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabularyTerm != null)
                    {
                        ArtifactTypeConcept = _vocabularyTerm.VocabularyTermId;
                    }
                }
            }
        }

        private Procedure _procedure;

        [ForeignKey("CatalogEntryFor")]
        public virtual Procedure Procedure
        {
            get
            {
                if (_procedure == null && !string.IsNullOrEmpty(CatalogEntryFor))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedure - no database context is set. CatalogEntryFor: " + CatalogEntryFor + ".");
                        }
                        return null;
                    }
                    _procedure = base.SoAContext.Procedures.Find(CatalogEntryFor);
                    if (_procedure != null)
                    {
                        base.SoAContext.Attach(_procedure);
                    }
                }
                return _procedure;
            }
            set
            {
                if (_procedure != value)
                {
                    _procedure = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedure != null)
                    {
                        CatalogEntryFor = _procedure.ProcedureId;
                    }
                }
            }
        }

        private Procedure _procedureRef;

        [ForeignKey("ComplianceRecordFor")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(ComplianceRecordFor))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. ComplianceRecordFor: " + ComplianceRecordFor + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(ComplianceRecordFor);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        ComplianceRecordFor = _procedureRef.ProcedureId;
                    }
                }
            }
        }

        private ObservableCollection<Resource> _resources;

        [InverseProperty("Resource")]
        public virtual ObservableCollection<Resource> Resources
        {
            get
            {
                if (_resources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resources - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _resources = new ObservableCollection<Resource>();
                    }
                    else
                    {
                        var items = base.SoAContext.Resources.Where(x => x.ExtractedFromResource == this.ResourceId).ToList<Resource>();
                        _resources = new ObservableCollection<Resource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _resources.CollectionChanged += Resources_CollectionChanged;
                }
                return _resources;
            }
            private set
            {
                if (_resources != null)
                {
                    _resources.CollectionChanged -= Resources_CollectionChanged;
                }
                _resources = value;
                if (_resources != null)
                {
                    _resources.CollectionChanged += Resources_CollectionChanged;
                }
            }
        }

        private void Resources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Resource>())
                {
                    item.ExtractedFromResource = this.ResourceId;
                }
            }
        }

        private ObservableCollection<ProcedureResource> _procedureResources;

        [InverseProperty("ResourceRef")]
        public virtual ObservableCollection<ProcedureResource> ProcedureResources
        {
            get
            {
                if (_procedureResources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureResources - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _procedureResources = new ObservableCollection<ProcedureResource>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureResources.Where(x => x.Resource == this.ResourceId).ToList<ProcedureResource>();
                        _procedureResources = new ObservableCollection<ProcedureResource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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

        private ObservableCollection<FAQ> _fAQs;

        [InverseProperty("ResourceRef")]
        public virtual ObservableCollection<FAQ> FAQs
        {
            get
            {
                if (_fAQs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FAQs - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _fAQs = new ObservableCollection<FAQ>();
                    }
                    else
                    {
                        var items = base.SoAContext.FAQs.Where(x => x.Resource == this.ResourceId).ToList<FAQ>();
                        _fAQs = new ObservableCollection<FAQ>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fAQs.CollectionChanged += FAQs_CollectionChanged;
                }
                return _fAQs;
            }
            private set
            {
                if (_fAQs != null)
                {
                    _fAQs.CollectionChanged -= FAQs_CollectionChanged;
                }
                _fAQs = value;
                if (_fAQs != null)
                {
                    _fAQs.CollectionChanged += FAQs_CollectionChanged;
                }
            }
        }

        private void FAQs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FAQ>())
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserQuestions - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _userQuestions = new ObservableCollection<UserQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.UserQuestions.Where(x => x.AddressedByResource == this.ResourceId).ToList<UserQuestion>();
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LearningActivities - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _learningActivities = new ObservableCollection<LearningActivity>();
                    }
                    else
                    {
                        var items = base.SoAContext.LearningActivities.Where(x => x.EvidenceResource == this.ResourceId).ToList<LearningActivity>();
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
                    item.EvidenceResource = this.ResourceId;
                }
            }
        }

        private ObservableCollection<OperationalBinding> _operationalBindings;

        [InverseProperty("ResourceRef")]
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
                            throw new InvalidOperationException("Cannot access OperationalBindings - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _operationalBindings = new ObservableCollection<OperationalBinding>();
                    }
                    else
                    {
                        var items = base.SoAContext.OperationalBindings.Where(x => x.Resource == this.ResourceId).ToList<OperationalBinding>();
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
                    item.Resource = this.ResourceId;
                }
            }
        }

        private ObservableCollection<MessageTemplate> _messageTemplates;

        [InverseProperty("ResourceRef")]
        public virtual ObservableCollection<MessageTemplate> MessageTemplates
        {
            get
            {
                if (_messageTemplates == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageTemplates - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _messageTemplates = new ObservableCollection<MessageTemplate>();
                    }
                    else
                    {
                        var items = base.SoAContext.MessageTemplates.Where(x => x.Resource == this.ResourceId).ToList<MessageTemplate>();
                        _messageTemplates = new ObservableCollection<MessageTemplate>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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

        private ObservableCollection<StepResource> _stepResources;

        [InverseProperty("ResourceRef")]
        public virtual ObservableCollection<StepResource> StepResources
        {
            get
            {
                if (_stepResources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepResources - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _stepResources = new ObservableCollection<StepResource>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepResources.Where(x => x.Resource == this.ResourceId).ToList<StepResource>();
                        _stepResources = new ObservableCollection<StepResource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepResources.CollectionChanged += StepResources_CollectionChanged;
                }
                return _stepResources;
            }
            private set
            {
                if (_stepResources != null)
                {
                    _stepResources.CollectionChanged -= StepResources_CollectionChanged;
                }
                _stepResources = value;
                if (_stepResources != null)
                {
                    _stepResources.CollectionChanged += StepResources_CollectionChanged;
                }
            }
        }

        private void StepResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepResource>())
                {
                    item.Resource = this.ResourceId;
                }
            }
        }

        private ObservableCollection<CollectedSourceMaterial> _collectedSourceMaterials;

        [InverseProperty("Resource")]
        public virtual ObservableCollection<CollectedSourceMaterial> CollectedSourceMaterials
        {
            get
            {
                if (_collectedSourceMaterials == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CollectedSourceMaterials - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>();
                    }
                    else
                    {
                        var items = base.SoAContext.CollectedSourceMaterials.Where(x => x.SourceDocument == this.ResourceId).ToList<CollectedSourceMaterial>();
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _collectedSourceMaterials.CollectionChanged += CollectedSourceMaterials_CollectionChanged;
                }
                return _collectedSourceMaterials;
            }
            private set
            {
                if (_collectedSourceMaterials != null)
                {
                    _collectedSourceMaterials.CollectionChanged -= CollectedSourceMaterials_CollectionChanged;
                }
                _collectedSourceMaterials = value;
                if (_collectedSourceMaterials != null)
                {
                    _collectedSourceMaterials.CollectionChanged += CollectedSourceMaterials_CollectionChanged;
                }
            }
        }

        private void CollectedSourceMaterials_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CollectedSourceMaterial>())
                {
                    item.SourceDocument = this.ResourceId;
                }
            }
        }

        private ObservableCollection<ConsumerSystemSync> _consumerSystemSyncs;

        [InverseProperty("Resource")]
        public virtual ObservableCollection<ConsumerSystemSync> ConsumerSystemSyncs
        {
            get
            {
                if (_consumerSystemSyncs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConsumerSystemSyncs - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _consumerSystemSyncs = new ObservableCollection<ConsumerSystemSync>();
                    }
                    else
                    {
                        var items = base.SoAContext.ConsumerSystemSyncs.Where(x => x.SourceResource == this.ResourceId).ToList<ConsumerSystemSync>();
                        _consumerSystemSyncs = new ObservableCollection<ConsumerSystemSync>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _consumerSystemSyncs.CollectionChanged += ConsumerSystemSyncs_CollectionChanged;
                }
                return _consumerSystemSyncs;
            }
            private set
            {
                if (_consumerSystemSyncs != null)
                {
                    _consumerSystemSyncs.CollectionChanged -= ConsumerSystemSyncs_CollectionChanged;
                }
                _consumerSystemSyncs = value;
                if (_consumerSystemSyncs != null)
                {
                    _consumerSystemSyncs.CollectionChanged += ConsumerSystemSyncs_CollectionChanged;
                }
            }
        }

        private void ConsumerSystemSyncs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ConsumerSystemSync>())
                {
                    item.SourceResource = this.ResourceId;
                }
            }
        }

        private ObservableCollection<RetrievalSegment> _retrievalSegments;

        [InverseProperty("Resource")]
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
                            throw new InvalidOperationException("Cannot access RetrievalSegments - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _retrievalSegments = new ObservableCollection<RetrievalSegment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RetrievalSegments.Where(x => x.SourceResource == this.ResourceId).ToList<RetrievalSegment>();
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
                    item.SourceResource = this.ResourceId;
                }
            }
        }

        private ObservableCollection<ModelProposal> _modelProposals;

        [InverseProperty("Resource")]
        public virtual ObservableCollection<ModelProposal> ModelProposals
        {
            get
            {
                if (_modelProposals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelProposals - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _modelProposals = new ObservableCollection<ModelProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelProposals.Where(x => x.SourceDocument == this.ResourceId).ToList<ModelProposal>();
                        _modelProposals = new ObservableCollection<ModelProposal>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelProposals.CollectionChanged += ModelProposals_CollectionChanged;
                }
                return _modelProposals;
            }
            private set
            {
                if (_modelProposals != null)
                {
                    _modelProposals.CollectionChanged -= ModelProposals_CollectionChanged;
                }
                _modelProposals = value;
                if (_modelProposals != null)
                {
                    _modelProposals.CollectionChanged += ModelProposals_CollectionChanged;
                }
            }
        }

        private void ModelProposals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelProposal>())
                {
                    item.SourceDocument = this.ResourceId;
                }
            }
        }

        private ObservableCollection<KnowledgeTrace> _knowledgeTraces;

        [InverseProperty("Resource")]
        public virtual ObservableCollection<KnowledgeTrace> KnowledgeTraces
        {
            get
            {
                if (_knowledgeTraces == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeTraces - no database context is set. ResourceId: " + this.ResourceId + ".");
                        }
                        _knowledgeTraces = new ObservableCollection<KnowledgeTrace>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTraces.Where(x => x.ContradictedDocument == this.ResourceId).ToList<KnowledgeTrace>();
                        _knowledgeTraces = new ObservableCollection<KnowledgeTrace>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeTraces.CollectionChanged += KnowledgeTraces_CollectionChanged;
                }
                return _knowledgeTraces;
            }
            private set
            {
                if (_knowledgeTraces != null)
                {
                    _knowledgeTraces.CollectionChanged -= KnowledgeTraces_CollectionChanged;
                }
                _knowledgeTraces = value;
                if (_knowledgeTraces != null)
                {
                    _knowledgeTraces.CollectionChanged += KnowledgeTraces_CollectionChanged;
                }
            }
        }

        private void KnowledgeTraces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeTrace>())
                {
                    item.ContradictedDocument = this.ResourceId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.Resource;
            _ = this.VocabularyTerm;
            _ = this.Procedure;
            _ = this.ProcedureRef;
            _ = this.Resources;
            _ = this.ProcedureResources;
            _ = this.FAQs;
            _ = this.UserQuestions;
            _ = this.LearningActivities;
            _ = this.OperationalBindings;
            _ = this.MessageTemplates;
            _ = this.StepResources;
            _ = this.CollectedSourceMaterials;
            _ = this.ConsumerSystemSyncs;
            _ = this.RetrievalSegments;
            _ = this.ModelProposals;
            _ = this.KnowledgeTraces;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
