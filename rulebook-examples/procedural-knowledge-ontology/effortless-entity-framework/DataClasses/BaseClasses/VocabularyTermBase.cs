
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
    [Table("VocabularyTerms")]
    public class VocabularyTermBase : SoAEntityBase
    {
        [Key]
        public string VocabularyTermId { get; set; }

        // Formula Name (rulebook: ={{PrefLabel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.PrefLabel))); set { }
        }

        public string? PrefLabel { get; set; }
        public string? AltLabels { get; set; }
        public string? Definition { get; set; }
        // Formula UsageCount (rulebook: =COUNTIFS(Requirements!{{ControlledTerm}}, {{VocabularyTermId}}))
        [NotMapped]
        public decimal? UsageCount
        {
            get => F.AsDecimal(F.Memo(this, "UsageCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Requirement>(base.SoAContext, "Requirements", __c => __c.Requirements), __r => F.CritField(F.Of(__r.ControlledTerm), F.Of(this.VocabularyTermId)))))); set { }
        }

        // Formula IsOrphanTerm (rulebook: ={{UsageCount}} = 0)
        [NotMapped]
        public bool? IsOrphanTerm
        {
            get => F.AsBool(F.Memo(this, "IsOrphanTerm", () => F.Eq(F.Of(this.UsageCount), F.I(0)))); set { }
        }

        // Formula IsWidelyAdoptedTerm (rulebook: ={{UsageCount}} >= 2)
        [NotMapped]
        public bool? IsWidelyAdoptedTerm
        {
            get => F.AsBool(F.Memo(this, "IsWidelyAdoptedTerm", () => F.Cmp(F.Of(this.UsageCount), ">=", F.I(2)))); set { }
        }

        // Formula OrphanTermVocabularyKey (rulebook: =IF({{IsOrphanTerm}}, {{Vocabulary}}, ""))
        [NotMapped]
        public string? OrphanTermVocabularyKey
        {
            get => F.AsString(F.Memo(this, "OrphanTermVocabularyKey", () => (F.Truthy(F.Bool3(F.Of(this.IsOrphanTerm))) ? F.Of(this.Vocabulary) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        public string? ScopeNote { get; set; }
        // Formula BroaderTermParent (rulebook: =INDEX(VocabularyTerms!{{BroaderTerm}}, MATCH({{BroaderTerm}}, VocabularyTerms!{{VocabularyTermId}}, 0)))
        [NotMapped]
        public string? BroaderTermParent
        {
            get => F.AsString(F.Memo(this, "BroaderTermParent", () => F.Lookup<VocabularyTerm>(this, "VocabularyTerms", "VocabularyTermId", __c => __c.VocabularyTerms, __r => F.Of(__r.VocabularyTermId), F.Of(this.BroaderTerm), __r => F.Of(__r.BroaderTerm), () => F.Of(new VocabularyTerm().BroaderTerm)))); set { }
        }

        // Formula SchemeGovernedDimension (rulebook: =INDEX(Vocabularies!{{GovernedDimension}}, MATCH({{Vocabulary}}, Vocabularies!{{VocabularyId}}, 0)))
        [NotMapped]
        public string? SchemeGovernedDimension
        {
            get => F.AsString(F.Memo(this, "SchemeGovernedDimension", () => F.Lookup<Vocabulary>(this, "Vocabularies", "VocabularyId", __c => __c.Vocabularies, __r => F.Of(__r.VocabularyId), F.Of(this.Vocabulary), __r => F.Of(__r.GovernedDimension), () => F.Of(new Vocabulary().GovernedDimension)))); set { }
        }

        public string? ConceptIri { get; set; }
        public string? NamespaceIri { get; set; }
        public string? SameAsIri { get; set; }
        // Formula IntroducedReleaseIssuedAt (rulebook: =INDEX(RulebookReleases!{{IssuedAt}}, MATCH({{IntroducedInRelease}}, RulebookReleases!{{RulebookReleaseId}}, 0)))
        [NotMapped]
        public DateTimeOffset? IntroducedReleaseIssuedAt
        {
            get => F.AsDateTime(F.Memo(this, "IntroducedReleaseIssuedAt", () => F.Lookup<RulebookRelease>(this, "RulebookReleases", "RulebookReleaseId", __c => __c.RulebookReleases, __r => F.Of(__r.RulebookReleaseId), F.Of(this.IntroducedInRelease), __r => F.Of(__r.IssuedAt), () => F.Of(new RulebookRelease().IssuedAt)))); set { }
        }

        public DateTimeOffset? DefinitionRevisedAt { get; set; }
        // Formula LatestMeaningChangeAt (rulebook: =MAXIFS(TermMeaningChanges!{{ChangedAt}}, TermMeaningChanges!{{VocabularyTerm}}, {{VocabularyTermId}}))
        [NotMapped]
        public DateTimeOffset? LatestMeaningChangeAt
        {
            get => F.AsDateTime(F.Memo(this, "LatestMeaningChangeAt", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<TermMeaningChange>(base.SoAContext, "TermMeaningChanges", __c => __c.TermMeaningChanges), __r => F.CritField(F.Of(__r.VocabularyTerm), F.Of(this.VocabularyTermId)), __r => F.Of(__r.ChangedAt))))); set { }
        }

        // Formula HasStaleDefinition (rulebook: =AND({{LatestMeaningChangeAt}} <> "", OR({{DefinitionRevisedAt}} = "", {{DefinitionRevisedAt}} < {{LatestMeaningChangeAt}})))
        [NotMapped]
        public bool? HasStaleDefinition
        {
            get => F.AsBool(F.Memo(this, "HasStaleDefinition", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.LatestMeaningChangeAt))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.DefinitionRevisedAt))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.DefinitionRevisedAt)), "<", F.Of(this.LatestMeaningChangeAt)))))))); set { }
        }

        // Formula StructuralShiftCount (rulebook: =COUNTIFS(TermMeaningChanges!{{VocabularyTerm}}, {{VocabularyTermId}}, TermMeaningChanges!{{IsStructuralChange}}, TRUE, TermMeaningChanges!{{SpanDays}}, ">=700"))
        [NotMapped]
        public int? StructuralShiftCount
        {
            get => F.AsInt(F.Memo(this, "StructuralShiftCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TermMeaningChange>(base.SoAContext, "TermMeaningChanges", __c => __c.TermMeaningChanges), __r => F.CritField(F.Of(__r.VocabularyTerm), F.Of(this.VocabularyTermId)) && F.CritLiteral(F.Of(__r.IsStructuralChange), F.B(true)) && F.CritOp(F.Of(__r.SpanDays), ">=", F.I(700))))))); set { }
        }

        // Formula HasStructuralSenseShiftAcrossYears (rulebook: ={{StructuralShiftCount}} > 0)
        [NotMapped]
        public bool? HasStructuralSenseShiftAcrossYears
        {
            get => F.AsBool(F.Memo(this, "HasStructuralSenseShiftAcrossYears", () => F.Cmp(F.Of(this.StructuralShiftCount), ">", F.I(0)))); set { }
        }

        // Formula PrefLabelPractitionerMentionCount (rulebook: =SUMIFS(TermLabelVariants!{{PractitionerMentionCount}}, TermLabelVariants!{{VocabularyTerm}}, {{VocabularyTermId}}, TermLabelVariants!{{LabelKind}}, "pref"))
        [NotMapped]
        public int? PrefLabelPractitionerMentionCount
        {
            get => F.AsInt(F.Memo(this, "PrefLabelPractitionerMentionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<TermLabelVariant>(base.SoAContext, "TermLabelVariants", __c => __c.TermLabelVariants), __r => F.CritField(F.Of(__r.VocabularyTerm), F.Of(this.VocabularyTermId)) && F.CritLiteral(F.Of(__r.LabelKind), F.S("pref")), __r => F.Of(__r.PractitionerMentionCount), null))))); set { }
        }

        // Formula AltLabelPractitionerMentionCount (rulebook: =SUMIFS(TermLabelVariants!{{PractitionerMentionCount}}, TermLabelVariants!{{VocabularyTerm}}, {{VocabularyTermId}}, TermLabelVariants!{{LabelKind}}, "alt"))
        [NotMapped]
        public int? AltLabelPractitionerMentionCount
        {
            get => F.AsInt(F.Memo(this, "AltLabelPractitionerMentionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<TermLabelVariant>(base.SoAContext, "TermLabelVariants", __c => __c.TermLabelVariants), __r => F.CritField(F.Of(__r.VocabularyTerm), F.Of(this.VocabularyTermId)) && F.CritLiteral(F.Of(__r.LabelKind), F.S("alt")), __r => F.Of(__r.PractitionerMentionCount), null))))); set { }
        }

        // Formula IsOrganizedAroundOfficialTerm (rulebook: =AND({{AltLabelPractitionerMentionCount}} >= 2, {{AltLabelPractitionerMentionCount}} > {{PrefLabelPractitionerMentionCount}}))
        [NotMapped]
        public bool? IsOrganizedAroundOfficialTerm
        {
            get => F.AsBool(F.Memo(this, "IsOrganizedAroundOfficialTerm", () => F.And(F.Bool3(F.Cmp(F.Of(this.AltLabelPractitionerMentionCount), ">=", F.I(2))), F.Bool3(F.Cmp(F.Of(this.AltLabelPractitionerMentionCount), ">", F.Of(this.PrefLabelPractitionerMentionCount)))))); set { }
        }

        // Formula SourcePhrasingCount (rulebook: =COUNTIFS(SourceTermMentions!{{IntendedTerm}}, {{VocabularyTermId}}))
        [NotMapped]
        public int? SourcePhrasingCount
        {
            get => F.AsInt(F.Memo(this, "SourcePhrasingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SourceTermMention>(base.SoAContext, "SourceTermMentions", __c => __c.SourceTermMentions), __r => F.CritField(F.Of(__r.IntendedTerm), F.Of(this.VocabularyTermId))))))); set { }
        }

        // Formula UnreconciledPhrasingCount (rulebook: =COUNTIFS(SourceTermMentions!{{UnresolvedIntendedTermKey}}, {{VocabularyTermId}}))
        [NotMapped]
        public int? UnreconciledPhrasingCount
        {
            get => F.AsInt(F.Memo(this, "UnreconciledPhrasingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SourceTermMention>(base.SoAContext, "SourceTermMentions", __c => __c.SourceTermMentions), __r => F.CritField(F.Of(__r.UnresolvedIntendedTermKey), F.Of(this.VocabularyTermId))))))); set { }
        }

        // Formula HasUnreconciledVariantPhrasings (rulebook: =AND({{SourcePhrasingCount}} >= 2, {{UnreconciledPhrasingCount}} > 0))
        [NotMapped]
        public bool? HasUnreconciledVariantPhrasings
        {
            get => F.AsBool(F.Memo(this, "HasUnreconciledVariantPhrasings", () => F.And(F.Bool3(F.Cmp(F.Of(this.SourcePhrasingCount), ">=", F.I(2))), F.Bool3(F.Cmp(F.Of(this.UnreconciledPhrasingCount), ">", F.I(0)))))); set { }
        }


        public string? Vocabulary { get; set; }
        public string? BroaderTerm { get; set; }
        public string? IntroducedInRelease { get; set; }
        public string? RepresentsRole { get; set; }

        private Vocabulary _vocabularyRef;

        [ForeignKey("Vocabulary")]
        public virtual Vocabulary VocabularyRef
        {
            get
            {
                if (_vocabularyRef == null && !string.IsNullOrEmpty(Vocabulary))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyRef - no database context is set. Vocabulary: " + Vocabulary + ".");
                        }
                        return null;
                    }
                    _vocabularyRef = base.SoAContext.Vocabularies.Find(Vocabulary);
                    if (_vocabularyRef != null)
                    {
                        base.SoAContext.Attach(_vocabularyRef);
                    }
                }
                return _vocabularyRef;
            }
            set
            {
                if (_vocabularyRef != value)
                {
                    _vocabularyRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabularyRef != null)
                    {
                        Vocabulary = _vocabularyRef.VocabularyId;
                    }
                }
            }
        }

        private VocabularyTerm _vocabularyTerm;

        [ForeignKey("BroaderTerm")]
        public virtual VocabularyTerm VocabularyTerm
        {
            get
            {
                if (_vocabularyTerm == null && !string.IsNullOrEmpty(BroaderTerm))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerm - no database context is set. BroaderTerm: " + BroaderTerm + ".");
                        }
                        return null;
                    }
                    _vocabularyTerm = base.SoAContext.VocabularyTerms.Find(BroaderTerm);
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
                        BroaderTerm = _vocabularyTerm.VocabularyTermId;
                    }
                }
            }
        }

        private RulebookRelease _rulebookRelease;

        [ForeignKey("IntroducedInRelease")]
        public virtual RulebookRelease RulebookRelease
        {
            get
            {
                if (_rulebookRelease == null && !string.IsNullOrEmpty(IntroducedInRelease))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookRelease - no database context is set. IntroducedInRelease: " + IntroducedInRelease + ".");
                        }
                        return null;
                    }
                    _rulebookRelease = base.SoAContext.RulebookReleases.Find(IntroducedInRelease);
                    if (_rulebookRelease != null)
                    {
                        base.SoAContext.Attach(_rulebookRelease);
                    }
                }
                return _rulebookRelease;
            }
            set
            {
                if (_rulebookRelease != value)
                {
                    _rulebookRelease = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookRelease != null)
                    {
                        IntroducedInRelease = _rulebookRelease.RulebookReleaseId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("RepresentsRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(RepresentsRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. RepresentsRole: " + RepresentsRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(RepresentsRole);
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
                        RepresentsRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<Requirement> _requirements;

        [InverseProperty("VocabularyTerm")]
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
                            throw new InvalidOperationException("Cannot access Requirements - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _requirements = new ObservableCollection<Requirement>();
                    }
                    else
                    {
                        var items = base.SoAContext.Requirements.Where(x => x.ControlledTerm == this.VocabularyTermId).ToList<Requirement>();
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
                    item.ControlledTerm = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<Resource> _resources;

        [InverseProperty("VocabularyTerm")]
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
                            throw new InvalidOperationException("Cannot access Resources - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _resources = new ObservableCollection<Resource>();
                    }
                    else
                    {
                        var items = base.SoAContext.Resources.Where(x => x.ArtifactTypeConcept == this.VocabularyTermId).ToList<Resource>();
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
                    item.ArtifactTypeConcept = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<VocabularyTerm> _vocabularyTerms;

        [InverseProperty("VocabularyTerm")]
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
                            throw new InvalidOperationException("Cannot access VocabularyTerms - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _vocabularyTerms = new ObservableCollection<VocabularyTerm>();
                    }
                    else
                    {
                        var items = base.SoAContext.VocabularyTerms.Where(x => x.BroaderTerm == this.VocabularyTermId).ToList<VocabularyTerm>();
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
                    item.BroaderTerm = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<KnowledgeBrokerLink> _knowledgeBrokerLinks;

        [InverseProperty("VocabularyTerm")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeBrokerLinks - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeBrokerLinks.Where(x => x.Topic == this.VocabularyTermId).ToList<KnowledgeBrokerLink>();
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
                    item.Topic = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<LifecycleStatuse> _lifecycleStatuses;

        [InverseProperty("VocabularyTerm")]
        public virtual ObservableCollection<LifecycleStatuse> LifecycleStatuses
        {
            get
            {
                if (_lifecycleStatuses == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LifecycleStatuses - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _lifecycleStatuses = new ObservableCollection<LifecycleStatuse>();
                    }
                    else
                    {
                        var items = base.SoAContext.LifecycleStatuses.Where(x => x.WorkflowStatusConcept == this.VocabularyTermId).ToList<LifecycleStatuse>();
                        _lifecycleStatuses = new ObservableCollection<LifecycleStatuse>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _lifecycleStatuses.CollectionChanged += LifecycleStatuses_CollectionChanged;
                }
                return _lifecycleStatuses;
            }
            private set
            {
                if (_lifecycleStatuses != null)
                {
                    _lifecycleStatuses.CollectionChanged -= LifecycleStatuses_CollectionChanged;
                }
                _lifecycleStatuses = value;
                if (_lifecycleStatuses != null)
                {
                    _lifecycleStatuses.CollectionChanged += LifecycleStatuses_CollectionChanged;
                }
            }
        }

        private void LifecycleStatuses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<LifecycleStatuse>())
                {
                    item.WorkflowStatusConcept = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<TermLabelVariant> _termLabelVariants;

        [InverseProperty("VocabularyTermRef")]
        public virtual ObservableCollection<TermLabelVariant> TermLabelVariants
        {
            get
            {
                if (_termLabelVariants == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TermLabelVariants - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _termLabelVariants = new ObservableCollection<TermLabelVariant>();
                    }
                    else
                    {
                        var items = base.SoAContext.TermLabelVariants.Where(x => x.VocabularyTerm == this.VocabularyTermId).ToList<TermLabelVariant>();
                        _termLabelVariants = new ObservableCollection<TermLabelVariant>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _termLabelVariants.CollectionChanged += TermLabelVariants_CollectionChanged;
                }
                return _termLabelVariants;
            }
            private set
            {
                if (_termLabelVariants != null)
                {
                    _termLabelVariants.CollectionChanged -= TermLabelVariants_CollectionChanged;
                }
                _termLabelVariants = value;
                if (_termLabelVariants != null)
                {
                    _termLabelVariants.CollectionChanged += TermLabelVariants_CollectionChanged;
                }
            }
        }

        private void TermLabelVariants_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TermLabelVariant>())
                {
                    item.VocabularyTerm = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<SourceTermMention> _sourceTermMentions;

        [InverseProperty("VocabularyTerm")]
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
                            throw new InvalidOperationException("Cannot access SourceTermMentions - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _sourceTermMentions = new ObservableCollection<SourceTermMention>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourceTermMentions.Where(x => x.IntendedTerm == this.VocabularyTermId).ToList<SourceTermMention>();
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
                    item.IntendedTerm = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<TermRelation> _fromTermTermRelations;

        [InverseProperty("VocabularyTerm")]
        public virtual ObservableCollection<TermRelation> FromTermTermRelations
        {
            get
            {
                if (_fromTermTermRelations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FromTermTermRelations - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _fromTermTermRelations = new ObservableCollection<TermRelation>();
                    }
                    else
                    {
                        var items = base.SoAContext.TermRelations.Where(x => x.FromTerm == this.VocabularyTermId).ToList<TermRelation>();
                        _fromTermTermRelations = new ObservableCollection<TermRelation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fromTermTermRelations.CollectionChanged += FromTermTermRelations_CollectionChanged;
                }
                return _fromTermTermRelations;
            }
            private set
            {
                if (_fromTermTermRelations != null)
                {
                    _fromTermTermRelations.CollectionChanged -= FromTermTermRelations_CollectionChanged;
                }
                _fromTermTermRelations = value;
                if (_fromTermTermRelations != null)
                {
                    _fromTermTermRelations.CollectionChanged += FromTermTermRelations_CollectionChanged;
                }
            }
        }

        private void FromTermTermRelations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TermRelation>())
                {
                    item.FromTerm = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<TermRelation> _toTermTermRelations;

        [InverseProperty("VocabularyTermRef")]
        public virtual ObservableCollection<TermRelation> ToTermTermRelations
        {
            get
            {
                if (_toTermTermRelations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ToTermTermRelations - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _toTermTermRelations = new ObservableCollection<TermRelation>();
                    }
                    else
                    {
                        var items = base.SoAContext.TermRelations.Where(x => x.ToTerm == this.VocabularyTermId).ToList<TermRelation>();
                        _toTermTermRelations = new ObservableCollection<TermRelation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _toTermTermRelations.CollectionChanged += ToTermTermRelations_CollectionChanged;
                }
                return _toTermTermRelations;
            }
            private set
            {
                if (_toTermTermRelations != null)
                {
                    _toTermTermRelations.CollectionChanged -= ToTermTermRelations_CollectionChanged;
                }
                _toTermTermRelations = value;
                if (_toTermTermRelations != null)
                {
                    _toTermTermRelations.CollectionChanged += ToTermTermRelations_CollectionChanged;
                }
            }
        }

        private void ToTermTermRelations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TermRelation>())
                {
                    item.ToTerm = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<TermMeaningChange> _termMeaningChanges;

        [InverseProperty("VocabularyTermRef")]
        public virtual ObservableCollection<TermMeaningChange> TermMeaningChanges
        {
            get
            {
                if (_termMeaningChanges == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TermMeaningChanges - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _termMeaningChanges = new ObservableCollection<TermMeaningChange>();
                    }
                    else
                    {
                        var items = base.SoAContext.TermMeaningChanges.Where(x => x.VocabularyTerm == this.VocabularyTermId).ToList<TermMeaningChange>();
                        _termMeaningChanges = new ObservableCollection<TermMeaningChange>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _termMeaningChanges.CollectionChanged += TermMeaningChanges_CollectionChanged;
                }
                return _termMeaningChanges;
            }
            private set
            {
                if (_termMeaningChanges != null)
                {
                    _termMeaningChanges.CollectionChanged -= TermMeaningChanges_CollectionChanged;
                }
                _termMeaningChanges = value;
                if (_termMeaningChanges != null)
                {
                    _termMeaningChanges.CollectionChanged += TermMeaningChanges_CollectionChanged;
                }
            }
        }

        private void TermMeaningChanges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TermMeaningChange>())
                {
                    item.VocabularyTerm = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<ExternalStandardTerm> _externalStandardTerms;

        [InverseProperty("VocabularyTerm")]
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
                            throw new InvalidOperationException("Cannot access ExternalStandardTerms - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _externalStandardTerms = new ObservableCollection<ExternalStandardTerm>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExternalStandardTerms.Where(x => x.RehomedAsTerm == this.VocabularyTermId).ToList<ExternalStandardTerm>();
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
                    item.RehomedAsTerm = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<RoleCapabilityTag> _roleCapabilityTags;

        [InverseProperty("VocabularyTerm")]
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
                            throw new InvalidOperationException("Cannot access RoleCapabilityTags - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _roleCapabilityTags = new ObservableCollection<RoleCapabilityTag>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleCapabilityTags.Where(x => x.CapabilityTerm == this.VocabularyTermId).ToList<RoleCapabilityTag>();
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
                    item.CapabilityTerm = this.VocabularyTermId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.VocabularyRef;
            _ = this.VocabularyTerm;
            _ = this.RulebookRelease;
            _ = this.Role;
            _ = this.Requirements;
            _ = this.Resources;
            _ = this.VocabularyTerms;
            _ = this.KnowledgeBrokerLinks;
            _ = this.LifecycleStatuses;
            _ = this.TermLabelVariants;
            _ = this.SourceTermMentions;
            _ = this.FromTermTermRelations;
            _ = this.ToTermTermRelations;
            _ = this.TermMeaningChanges;
            _ = this.ExternalStandardTerms;
            _ = this.RoleCapabilityTags;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
