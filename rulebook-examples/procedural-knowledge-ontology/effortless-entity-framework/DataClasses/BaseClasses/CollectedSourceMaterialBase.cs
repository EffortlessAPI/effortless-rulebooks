
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
    [Table("CollectedSourceMaterials")]
    public class CollectedSourceMaterialBase : SoAEntityBase
    {
        [Key]
        public string CollectedSourceMaterialId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? MaterialKind { get; set; }
        public DateTimeOffset? CollectedAt { get; set; }
        public DateTimeOffset? OrganizedAt { get; set; }
        public DateTimeOffset? EncodedAt { get; set; }
        // Formula IsModeledBeforeOrganized (rulebook: =AND({{EncodedIntoVersion}} <> "", OR({{OrganizedAt}} = "", {{EncodedAt}} < {{OrganizedAt}})))
        [NotMapped]
        public bool? IsModeledBeforeOrganized
        {
            get => F.AsBool(F.Memo(this, "IsModeledBeforeOrganized", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.EncodedIntoVersion))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.OrganizedAt))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.EncodedAt)), "<", F.Nullif(F.Of(this.OrganizedAt))))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        // Formula SourceDocumentRevisedAt (rulebook: =INDEX(Resources!{{ModifiedAt}}, MATCH({{SourceDocument}}, Resources!{{ResourceId}}, 0)))
        [NotMapped]
        public DateTimeOffset? SourceDocumentRevisedAt
        {
            get => F.AsDateTime(F.Memo(this, "SourceDocumentRevisedAt", () => F.Lookup<Resource>(this, "Resources", "ResourceId", __c => __c.Resources, __r => F.Of(__r.ResourceId), F.Of(this.SourceDocument), __r => F.Of(__r.ModifiedAt), () => F.Of(new Resource().ModifiedAt)))); set { }
        }

        // Formula IsDocumentSource (rulebook: ={{MaterialKind}} = "DocumentExcerpt")
        [NotMapped]
        public bool? IsDocumentSource
        {
            get => F.AsBool(F.Memo(this, "IsDocumentSource", () => F.Eq(F.Nullif(F.Of(this.MaterialKind)), F.S("DocumentExcerpt")))); set { }
        }

        // Formula IsPeopleCapture (rulebook: =OR({{MaterialKind}} = "Transcript", {{MaterialKind}} = "FieldNotes"))
        [NotMapped]
        public bool? IsPeopleCapture
        {
            get => F.AsBool(F.Memo(this, "IsPeopleCapture", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.MaterialKind)), F.S("Transcript"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.MaterialKind)), F.S("FieldNotes")))))); set { }
        }

        // Formula IsPracticeEvidence (rulebook: =OR({{MaterialKind}} = "FieldNotes", {{MaterialKind}} = "MinedEventTrace"))
        [NotMapped]
        public bool? IsPracticeEvidence
        {
            get => F.AsBool(F.Memo(this, "IsPracticeEvidence", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.MaterialKind)), F.S("FieldNotes"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.MaterialKind)), F.S("MinedEventTrace")))))); set { }
        }

        public bool? HoldsReasoningOrTacitKnowledge { get; set; }
        // Formula IsCapturedInFlowOfWork (rulebook: ={{CapturedDuringExecution}} <> "")
        [NotMapped]
        public bool? IsCapturedInFlowOfWork
        {
            get => F.AsBool(F.Memo(this, "IsCapturedInFlowOfWork", () => F.IsNotBlank(F.Of(this.CapturedDuringExecution)))); set { }
        }

        public decimal? ExpertEffortHours { get; set; }
        // Formula DependentTraceCount (rulebook: =COUNTIFS(KnowledgeTraces!{{SourceMaterial}}, {{CollectedSourceMaterialId}}))
        [NotMapped]
        public int? DependentTraceCount
        {
            get => F.AsInt(F.Memo(this, "DependentTraceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTrace>(base.SoAContext, "KnowledgeTraces", __c => __c.KnowledgeTraces), __r => F.CritField(F.Of(__r.SourceMaterial), F.Of(this.CollectedSourceMaterialId))))))); set { }
        }

        // Formula IsDependencyInvisibleToChange (rulebook: =AND({{EncodedIntoVersion}} <> "", {{DependentTraceCount}} = 0))
        [NotMapped]
        public bool? IsDependencyInvisibleToChange
        {
            get => F.AsBool(F.Memo(this, "IsDependencyInvisibleToChange", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.EncodedIntoVersion))), F.Bool3(F.Eq(F.Of(this.DependentTraceCount), F.I(0)))))); set { }
        }

        // Formula ChangedDependentCount (rulebook: =COUNTIFS(KnowledgeTraces!{{SourceMaterial}}, {{CollectedSourceMaterialId}}, KnowledgeTraces!{{IsSourceChangedSinceTaken}}, TRUE))
        [NotMapped]
        public int? ChangedDependentCount
        {
            get => F.AsInt(F.Memo(this, "ChangedDependentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTrace>(base.SoAContext, "KnowledgeTraces", __c => __c.KnowledgeTraces), __r => F.CritField(F.Of(__r.SourceMaterial), F.Of(this.CollectedSourceMaterialId)) && F.CritLiteral(F.Of(__r.IsSourceChangedSinceTaken), F.B(true))))))); set { }
        }

        // Formula HasKnowledgeAffectedBySourceChange (rulebook: ={{ChangedDependentCount}} > 0)
        [NotMapped]
        public bool? HasKnowledgeAffectedBySourceChange
        {
            get => F.AsBool(F.Memo(this, "HasKnowledgeAffectedBySourceChange", () => F.Cmp(F.Of(this.ChangedDependentCount), ">", F.I(0)))); set { }
        }


        public string? Procedure { get; set; }
        public string? OrganizedIntoScheme { get; set; }
        public string? EncodedIntoVersion { get; set; }
        public string? SourceDocument { get; set; }
        public string? ComplementsMiningRun { get; set; }
        public string? CapturedDuringExecution { get; set; }
        public string? CollectedAtOccasion { get; set; }
        public string? PromptedByFeedback { get; set; }
        public string? ProducedByMethodApplication { get; set; }
        public string? ContributingExpert { get; set; }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
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
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }

        private Vocabulary _vocabulary;

        [ForeignKey("OrganizedIntoScheme")]
        public virtual Vocabulary Vocabulary
        {
            get
            {
                if (_vocabulary == null && !string.IsNullOrEmpty(OrganizedIntoScheme))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Vocabulary - no database context is set. OrganizedIntoScheme: " + OrganizedIntoScheme + ".");
                        }
                        return null;
                    }
                    _vocabulary = base.SoAContext.Vocabularies.Find(OrganizedIntoScheme);
                    if (_vocabulary != null)
                    {
                        base.SoAContext.Attach(_vocabulary);
                    }
                }
                return _vocabulary;
            }
            set
            {
                if (_vocabulary != value)
                {
                    _vocabulary = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabulary != null)
                    {
                        OrganizedIntoScheme = _vocabulary.VocabularyId;
                    }
                }
            }
        }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("EncodedIntoVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(EncodedIntoVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. EncodedIntoVersion: " + EncodedIntoVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = base.SoAContext.ProcedureVersions.Find(EncodedIntoVersion);
                    if (_procedureVersion != null)
                    {
                        base.SoAContext.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersion != null)
                    {
                        EncodedIntoVersion = _procedureVersion.ProcedureVersionId;
                    }
                }
            }
        }

        private Resource _resource;

        [ForeignKey("SourceDocument")]
        public virtual Resource Resource
        {
            get
            {
                if (_resource == null && !string.IsNullOrEmpty(SourceDocument))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. SourceDocument: " + SourceDocument + ".");
                        }
                        return null;
                    }
                    _resource = base.SoAContext.Resources.Find(SourceDocument);
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
                        SourceDocument = _resource.ResourceId;
                    }
                }
            }
        }

        private ProcessMiningRun _processMiningRun;

        [ForeignKey("ComplementsMiningRun")]
        public virtual ProcessMiningRun ProcessMiningRun
        {
            get
            {
                if (_processMiningRun == null && !string.IsNullOrEmpty(ComplementsMiningRun))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessMiningRun - no database context is set. ComplementsMiningRun: " + ComplementsMiningRun + ".");
                        }
                        return null;
                    }
                    _processMiningRun = base.SoAContext.ProcessMiningRuns.Find(ComplementsMiningRun);
                    if (_processMiningRun != null)
                    {
                        base.SoAContext.Attach(_processMiningRun);
                    }
                }
                return _processMiningRun;
            }
            set
            {
                if (_processMiningRun != value)
                {
                    _processMiningRun = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_processMiningRun != null)
                    {
                        ComplementsMiningRun = _processMiningRun.ProcessMiningRunId;
                    }
                }
            }
        }

        private ProcedureExecution _procedureExecution;

        [ForeignKey("CapturedDuringExecution")]
        public virtual ProcedureExecution ProcedureExecution
        {
            get
            {
                if (_procedureExecution == null && !string.IsNullOrEmpty(CapturedDuringExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecution - no database context is set. CapturedDuringExecution: " + CapturedDuringExecution + ".");
                        }
                        return null;
                    }
                    _procedureExecution = base.SoAContext.ProcedureExecutions.Find(CapturedDuringExecution);
                    if (_procedureExecution != null)
                    {
                        base.SoAContext.Attach(_procedureExecution);
                    }
                }
                return _procedureExecution;
            }
            set
            {
                if (_procedureExecution != value)
                {
                    _procedureExecution = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureExecution != null)
                    {
                        CapturedDuringExecution = _procedureExecution.ProcedureExecutionId;
                    }
                }
            }
        }

        private CollectionOccasion _collectionOccasion;

        [ForeignKey("CollectedAtOccasion")]
        public virtual CollectionOccasion CollectionOccasion
        {
            get
            {
                if (_collectionOccasion == null && !string.IsNullOrEmpty(CollectedAtOccasion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CollectionOccasion - no database context is set. CollectedAtOccasion: " + CollectedAtOccasion + ".");
                        }
                        return null;
                    }
                    _collectionOccasion = base.SoAContext.CollectionOccasions.Find(CollectedAtOccasion);
                    if (_collectionOccasion != null)
                    {
                        base.SoAContext.Attach(_collectionOccasion);
                    }
                }
                return _collectionOccasion;
            }
            set
            {
                if (_collectionOccasion != value)
                {
                    _collectionOccasion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_collectionOccasion != null)
                    {
                        CollectedAtOccasion = _collectionOccasion.CollectionOccasionId;
                    }
                }
            }
        }

        private UserFeedback _userFeedback;

        [ForeignKey("PromptedByFeedback")]
        public virtual UserFeedback UserFeedback
        {
            get
            {
                if (_userFeedback == null && !string.IsNullOrEmpty(PromptedByFeedback))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserFeedback - no database context is set. PromptedByFeedback: " + PromptedByFeedback + ".");
                        }
                        return null;
                    }
                    _userFeedback = base.SoAContext.UserFeedback.Find(PromptedByFeedback);
                    if (_userFeedback != null)
                    {
                        base.SoAContext.Attach(_userFeedback);
                    }
                }
                return _userFeedback;
            }
            set
            {
                if (_userFeedback != value)
                {
                    _userFeedback = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_userFeedback != null)
                    {
                        PromptedByFeedback = _userFeedback.UserFeedbackId;
                    }
                }
            }
        }

        private MethodApplication _methodApplication;

        [ForeignKey("ProducedByMethodApplication")]
        public virtual MethodApplication MethodApplication
        {
            get
            {
                if (_methodApplication == null && !string.IsNullOrEmpty(ProducedByMethodApplication))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MethodApplication - no database context is set. ProducedByMethodApplication: " + ProducedByMethodApplication + ".");
                        }
                        return null;
                    }
                    _methodApplication = base.SoAContext.MethodApplications.Find(ProducedByMethodApplication);
                    if (_methodApplication != null)
                    {
                        base.SoAContext.Attach(_methodApplication);
                    }
                }
                return _methodApplication;
            }
            set
            {
                if (_methodApplication != value)
                {
                    _methodApplication = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_methodApplication != null)
                    {
                        ProducedByMethodApplication = _methodApplication.MethodApplicationId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("ContributingExpert")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ContributingExpert))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ContributingExpert: " + ContributingExpert + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ContributingExpert);
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
                        ContributingExpert = _agent.AgentId;
                    }
                }
            }
        }

        private ObservableCollection<SchemeRefinement> _schemeRefinements;

        [InverseProperty("CollectedSourceMaterial")]
        public virtual ObservableCollection<SchemeRefinement> SchemeRefinements
        {
            get
            {
                if (_schemeRefinements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SchemeRefinements - no database context is set. CollectedSourceMaterialId: " + this.CollectedSourceMaterialId + ".");
                        }
                        _schemeRefinements = new ObservableCollection<SchemeRefinement>();
                    }
                    else
                    {
                        var items = base.SoAContext.SchemeRefinements.Where(x => x.TriggeredByMaterial == this.CollectedSourceMaterialId).ToList<SchemeRefinement>();
                        _schemeRefinements = new ObservableCollection<SchemeRefinement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _schemeRefinements.CollectionChanged += SchemeRefinements_CollectionChanged;
                }
                return _schemeRefinements;
            }
            private set
            {
                if (_schemeRefinements != null)
                {
                    _schemeRefinements.CollectionChanged -= SchemeRefinements_CollectionChanged;
                }
                _schemeRefinements = value;
                if (_schemeRefinements != null)
                {
                    _schemeRefinements.CollectionChanged += SchemeRefinements_CollectionChanged;
                }
            }
        }

        private void SchemeRefinements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SchemeRefinement>())
                {
                    item.TriggeredByMaterial = this.CollectedSourceMaterialId;
                }
            }
        }

        private ObservableCollection<SourceTermMention> _sourceTermMentions;

        [InverseProperty("CollectedSourceMaterial")]
        public virtual ObservableCollection<SourceTermMention> SourceTermMentions
        {
            get
            {
                if (_sourceTermMentions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SourceTermMentions - no database context is set. CollectedSourceMaterialId: " + this.CollectedSourceMaterialId + ".");
                        }
                        _sourceTermMentions = new ObservableCollection<SourceTermMention>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourceTermMentions.Where(x => x.SourceMaterial == this.CollectedSourceMaterialId).ToList<SourceTermMention>();
                        _sourceTermMentions = new ObservableCollection<SourceTermMention>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _sourceTermMentions.CollectionChanged += SourceTermMentions_CollectionChanged;
                }
                return _sourceTermMentions;
            }
            private set
            {
                if (_sourceTermMentions != null)
                {
                    _sourceTermMentions.CollectionChanged -= SourceTermMentions_CollectionChanged;
                }
                _sourceTermMentions = value;
                if (_sourceTermMentions != null)
                {
                    _sourceTermMentions.CollectionChanged += SourceTermMentions_CollectionChanged;
                }
            }
        }

        private void SourceTermMentions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SourceTermMention>())
                {
                    item.SourceMaterial = this.CollectedSourceMaterialId;
                }
            }
        }

        private ObservableCollection<KnowledgeTrace> _knowledgeTraces;

        [InverseProperty("CollectedSourceMaterial")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeTraces - no database context is set. CollectedSourceMaterialId: " + this.CollectedSourceMaterialId + ".");
                        }
                        _knowledgeTraces = new ObservableCollection<KnowledgeTrace>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTraces.Where(x => x.SourceMaterial == this.CollectedSourceMaterialId).ToList<KnowledgeTrace>();
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
                    item.SourceMaterial = this.CollectedSourceMaterialId;
                }
            }
        }

        private ObservableCollection<StakeholderPerspectif> _stakeholderPerspectives;

        [InverseProperty("CollectedSourceMaterial")]
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
                            throw new InvalidOperationException("Cannot access StakeholderPerspectives - no database context is set. CollectedSourceMaterialId: " + this.CollectedSourceMaterialId + ".");
                        }
                        _stakeholderPerspectives = new ObservableCollection<StakeholderPerspectif>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderPerspectives.Where(x => x.SourceMaterial == this.CollectedSourceMaterialId).ToList<StakeholderPerspectif>();
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
                    item.SourceMaterial = this.CollectedSourceMaterialId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.Vocabulary;
            _ = this.ProcedureVersion;
            _ = this.Resource;
            _ = this.ProcessMiningRun;
            _ = this.ProcedureExecution;
            _ = this.CollectionOccasion;
            _ = this.UserFeedback;
            _ = this.MethodApplication;
            _ = this.Agent;
            _ = this.SchemeRefinements;
            _ = this.SourceTermMentions;
            _ = this.KnowledgeTraces;
            _ = this.StakeholderPerspectives;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
