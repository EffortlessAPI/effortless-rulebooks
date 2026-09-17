
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
    [Table("Vocabularies")]
    public class VocabularyBase : SoAEntityBase
    {
        [Key]
        public string VocabularyId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Title))); set { }
        }

        public string? Title { get; set; }
        public string? SchemeUri { get; set; }
        // Formula TermCount (rulebook: =COUNTIFS(VocabularyTerms!{{Vocabulary}}, {{VocabularyId}}))
        [NotMapped]
        public decimal? TermCount
        {
            get => F.AsDecimal(F.Memo(this, "TermCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<VocabularyTerm>(base.SoAContext, "VocabularyTerms", __c => __c.VocabularyTerms), __r => F.CritField(F.Of(__r.Vocabulary), F.Of(this.VocabularyId)))))); set { }
        }

        // Formula OrphanTermCount (rulebook: =COUNTIFS(VocabularyTerms!{{OrphanTermVocabularyKey}}, {{VocabularyId}}))
        [NotMapped]
        public decimal? OrphanTermCount
        {
            get => F.AsDecimal(F.Memo(this, "OrphanTermCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<VocabularyTerm>(base.SoAContext, "VocabularyTerms", __c => __c.VocabularyTerms), __r => F.CritField(F.Of(__r.OrphanTermVocabularyKey), F.Of(this.VocabularyId)))))); set { }
        }

        // Formula HasOrphanTerms (rulebook: ={{OrphanTermCount}} > 0)
        [NotMapped]
        public bool? HasOrphanTerms
        {
            get => F.AsBool(F.Memo(this, "HasOrphanTerms", () => F.Cmp(F.Of(this.OrphanTermCount), ">", F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        public string? SchemeKind { get; set; }
        public string? ModelLayer { get; set; }
        public string? GovernedDimension { get; set; }
        public string? PublicationFormat { get; set; }
        public string? Prefix { get; set; }
        public DateTimeOffset? EstablishedAt { get; set; }
        public DateTimeOffset? OntologyModelingStartedAt { get; set; }
        // Formula IsMachineAccessible (rulebook: =AND({{SchemeUri}} <> "", {{PublicationFormat}} = "SKOS Turtle"))
        [NotMapped]
        public bool? IsMachineAccessible
        {
            get => F.AsBool(F.Memo(this, "IsMachineAccessible", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.SchemeUri))), F.Bool3(F.Eq(F.Nullif(F.Of(this.PublicationFormat)), F.S("SKOS Turtle")))))); set { }
        }

        // Formula ManagedSchemeProcedureKey (rulebook: =IF(AND({{GoverningRole}} <> "", {{EstablishedAt}} <> ""), {{GovernsProcedure}}, ""))
        [NotMapped]
        public string? ManagedSchemeProcedureKey
        {
            get => F.AsString(F.Memo(this, "ManagedSchemeProcedureKey", () => (F.Truthy(F.Bool3(F.And(F.Bool3(F.IsNotBlank(F.Of(this.GoverningRole))), F.Bool3(F.IsNotBlank(F.Of(this.EstablishedAt)))))) ? F.Of(this.GovernsProcedure) : F.S("")))); set { }
        }

        // Formula OrganizedTranscriptCount (rulebook: =COUNTIFS(CollectedSourceMaterials!{{OrganizedIntoScheme}}, {{VocabularyId}}, CollectedSourceMaterials!{{MaterialKind}}, "Transcript"))
        [NotMapped]
        public int? OrganizedTranscriptCount
        {
            get => F.AsInt(F.Memo(this, "OrganizedTranscriptCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CollectedSourceMaterial>(base.SoAContext, "CollectedSourceMaterials", __c => __c.CollectedSourceMaterials), __r => F.CritField(F.Of(__r.OrganizedIntoScheme), F.Of(this.VocabularyId)) && F.CritLiteral(F.Of(__r.MaterialKind), F.S("Transcript"))))))); set { }
        }

        // Formula OrganizedFieldNotesCount (rulebook: =COUNTIFS(CollectedSourceMaterials!{{OrganizedIntoScheme}}, {{VocabularyId}}, CollectedSourceMaterials!{{MaterialKind}}, "FieldNotes"))
        [NotMapped]
        public int? OrganizedFieldNotesCount
        {
            get => F.AsInt(F.Memo(this, "OrganizedFieldNotesCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CollectedSourceMaterial>(base.SoAContext, "CollectedSourceMaterials", __c => __c.CollectedSourceMaterials), __r => F.CritField(F.Of(__r.OrganizedIntoScheme), F.Of(this.VocabularyId)) && F.CritLiteral(F.Of(__r.MaterialKind), F.S("FieldNotes"))))))); set { }
        }

        // Formula OrganizedProcessMapCount (rulebook: =COUNTIFS(CollectedSourceMaterials!{{OrganizedIntoScheme}}, {{VocabularyId}}, CollectedSourceMaterials!{{MaterialKind}}, "ProcessMap"))
        [NotMapped]
        public int? OrganizedProcessMapCount
        {
            get => F.AsInt(F.Memo(this, "OrganizedProcessMapCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CollectedSourceMaterial>(base.SoAContext, "CollectedSourceMaterials", __c => __c.CollectedSourceMaterials), __r => F.CritField(F.Of(__r.OrganizedIntoScheme), F.Of(this.VocabularyId)) && F.CritLiteral(F.Of(__r.MaterialKind), F.S("ProcessMap"))))))); set { }
        }

        // Formula OrganizedMinedEventTraceCount (rulebook: =COUNTIFS(CollectedSourceMaterials!{{OrganizedIntoScheme}}, {{VocabularyId}}, CollectedSourceMaterials!{{MaterialKind}}, "MinedEventTrace"))
        [NotMapped]
        public int? OrganizedMinedEventTraceCount
        {
            get => F.AsInt(F.Memo(this, "OrganizedMinedEventTraceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CollectedSourceMaterial>(base.SoAContext, "CollectedSourceMaterials", __c => __c.CollectedSourceMaterials), __r => F.CritField(F.Of(__r.OrganizedIntoScheme), F.Of(this.VocabularyId)) && F.CritLiteral(F.Of(__r.MaterialKind), F.S("MinedEventTrace"))))))); set { }
        }

        // Formula OrganizedDocumentExcerptCount (rulebook: =COUNTIFS(CollectedSourceMaterials!{{OrganizedIntoScheme}}, {{VocabularyId}}, CollectedSourceMaterials!{{MaterialKind}}, "DocumentExcerpt"))
        [NotMapped]
        public int? OrganizedDocumentExcerptCount
        {
            get => F.AsInt(F.Memo(this, "OrganizedDocumentExcerptCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CollectedSourceMaterial>(base.SoAContext, "CollectedSourceMaterials", __c => __c.CollectedSourceMaterials), __r => F.CritField(F.Of(__r.OrganizedIntoScheme), F.Of(this.VocabularyId)) && F.CritLiteral(F.Of(__r.MaterialKind), F.S("DocumentExcerpt"))))))); set { }
        }

        // Formula OrganizedMaterialCount (rulebook: =COUNTIFS(CollectedSourceMaterials!{{OrganizedIntoScheme}}, {{VocabularyId}}))
        [NotMapped]
        public int? OrganizedMaterialCount
        {
            get => F.AsInt(F.Memo(this, "OrganizedMaterialCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CollectedSourceMaterial>(base.SoAContext, "CollectedSourceMaterials", __c => __c.CollectedSourceMaterials), __r => F.CritField(F.Of(__r.OrganizedIntoScheme), F.Of(this.VocabularyId))))))); set { }
        }

        // Formula OrganizedKindCount (rulebook: =IF({{OrganizedTranscriptCount}} > 0, 1, 0) + IF({{OrganizedFieldNotesCount}} > 0, 1, 0) + IF({{OrganizedProcessMapCount}} > 0, 1, 0) + IF({{OrganizedMinedEventTraceCount}} > 0, 1, 0) + IF({{OrganizedDocumentExcerptCount}} > 0, 1, 0))
        [NotMapped]
        public int? OrganizedKindCount
        {
            get => F.AsInt(F.Memo(this, "OrganizedKindCount", () => F.Integer(F.Add(F.Add(F.Add(F.Add((F.Truthy(F.Bool3(F.Cmp(F.Of(this.OrganizedTranscriptCount), ">", F.I(0)))) ? F.I(1) : F.I(0)), (F.Truthy(F.Bool3(F.Cmp(F.Of(this.OrganizedFieldNotesCount), ">", F.I(0)))) ? F.I(1) : F.I(0))), (F.Truthy(F.Bool3(F.Cmp(F.Of(this.OrganizedProcessMapCount), ">", F.I(0)))) ? F.I(1) : F.I(0))), (F.Truthy(F.Bool3(F.Cmp(F.Of(this.OrganizedMinedEventTraceCount), ">", F.I(0)))) ? F.I(1) : F.I(0))), (F.Truthy(F.Bool3(F.Cmp(F.Of(this.OrganizedDocumentExcerptCount), ">", F.I(0)))) ? F.I(1) : F.I(0)))))); set { }
        }

        // Formula IsSingleKindFrame (rulebook: =AND({{OrganizedMaterialCount}} >= 2, {{OrganizedKindCount}} = 1))
        [NotMapped]
        public bool? IsSingleKindFrame
        {
            get => F.AsBool(F.Memo(this, "IsSingleKindFrame", () => F.And(F.Bool3(F.Cmp(F.Of(this.OrganizedMaterialCount), ">=", F.I(2))), F.Bool3(F.Eq(F.Of(this.OrganizedKindCount), F.I(1)))))); set { }
        }

        // Formula LatestOrganizedMaterialAt (rulebook: =MAXIFS(CollectedSourceMaterials!{{CollectedAt}}, CollectedSourceMaterials!{{OrganizedIntoScheme}}, {{VocabularyId}}))
        [NotMapped]
        public DateTimeOffset? LatestOrganizedMaterialAt
        {
            get => F.AsDateTime(F.Memo(this, "LatestOrganizedMaterialAt", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<CollectedSourceMaterial>(base.SoAContext, "CollectedSourceMaterials", __c => __c.CollectedSourceMaterials), __r => F.CritField(F.Of(__r.OrganizedIntoScheme), F.Of(this.VocabularyId)), __r => F.Of(__r.CollectedAt))))); set { }
        }

        // Formula RefinementCount (rulebook: =COUNTIFS(SchemeRefinements!{{Vocabulary}}, {{VocabularyId}}))
        [NotMapped]
        public int? RefinementCount
        {
            get => F.AsInt(F.Memo(this, "RefinementCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SchemeRefinement>(base.SoAContext, "SchemeRefinements", __c => __c.SchemeRefinements), __r => F.CritField(F.Of(__r.Vocabulary), F.Of(this.VocabularyId))))))); set { }
        }

        // Formula IsFrozenDespiteNewCollection (rulebook: =AND({{EstablishedAt}} <> "", {{LatestOrganizedMaterialAt}} <> "", {{LatestOrganizedMaterialAt}} > {{EstablishedAt}}, {{RefinementCount}} = 0))
        [NotMapped]
        public bool? IsFrozenDespiteNewCollection
        {
            get => F.AsBool(F.Memo(this, "IsFrozenDespiteNewCollection", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.EstablishedAt))), F.Bool3(F.IsNotBlank(F.Of(this.LatestOrganizedMaterialAt))), F.Bool3(F.Cmp(F.Of(this.LatestOrganizedMaterialAt), ">", F.Nullif(F.Of(this.EstablishedAt)))), F.Bool3(F.Eq(F.Of(this.RefinementCount), F.I(0)))))); set { }
        }

        // Formula OntologyPrecededVocabularyControl (rulebook: =AND({{OntologyModelingStartedAt}} <> "", OR({{EstablishedAt}} = "", {{OntologyModelingStartedAt}} < {{EstablishedAt}})))
        [NotMapped]
        public bool? OntologyPrecededVocabularyControl
        {
            get => F.AsBool(F.Memo(this, "OntologyPrecededVocabularyControl", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.OntologyModelingStartedAt))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.EstablishedAt))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.OntologyModelingStartedAt)), "<", F.Nullif(F.Of(this.EstablishedAt))))))))); set { }
        }


        public string? GoverningRole { get; set; }
        public string? GovernsProcedure { get; set; }

        private Role _role;

        [ForeignKey("GoverningRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(GoverningRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. GoverningRole: " + GoverningRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(GoverningRole);
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
                        GoverningRole = _role.RoleId;
                    }
                }
            }
        }

        private Procedure _procedure;

        [ForeignKey("GovernsProcedure")]
        public virtual Procedure Procedure
        {
            get
            {
                if (_procedure == null && !string.IsNullOrEmpty(GovernsProcedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedure - no database context is set. GovernsProcedure: " + GovernsProcedure + ".");
                        }
                        return null;
                    }
                    _procedure = base.SoAContext.Procedures.Find(GovernsProcedure);
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
                        GovernsProcedure = _procedure.ProcedureId;
                    }
                }
            }
        }

        private ObservableCollection<CommunitiesOfPractice> _communitiesOfPractice;

        [InverseProperty("Vocabulary")]
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
                            throw new InvalidOperationException("Cannot access CommunitiesOfPractice - no database context is set. VocabularyId: " + this.VocabularyId + ".");
                        }
                        _communitiesOfPractice = new ObservableCollection<CommunitiesOfPractice>();
                    }
                    else
                    {
                        var items = base.SoAContext.CommunitiesOfPractice.Where(x => x.OwnVocabulary == this.VocabularyId).ToList<CommunitiesOfPractice>();
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
                    item.OwnVocabulary = this.VocabularyId;
                }
            }
        }

        private ObservableCollection<VocabularyTerm> _vocabularyTerms;

        [InverseProperty("VocabularyRef")]
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
                            throw new InvalidOperationException("Cannot access VocabularyTerms - no database context is set. VocabularyId: " + this.VocabularyId + ".");
                        }
                        _vocabularyTerms = new ObservableCollection<VocabularyTerm>();
                    }
                    else
                    {
                        var items = base.SoAContext.VocabularyTerms.Where(x => x.Vocabulary == this.VocabularyId).ToList<VocabularyTerm>();
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
                    item.Vocabulary = this.VocabularyId;
                }
            }
        }

        private ObservableCollection<KnowledgeBrokerLink> _seekerVocabularyKnowledgeBrokerLinks;

        [InverseProperty("Vocabulary")]
        public virtual ObservableCollection<KnowledgeBrokerLink> SeekerVocabularyKnowledgeBrokerLinks
        {
            get
            {
                if (_seekerVocabularyKnowledgeBrokerLinks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SeekerVocabularyKnowledgeBrokerLinks - no database context is set. VocabularyId: " + this.VocabularyId + ".");
                        }
                        _seekerVocabularyKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeBrokerLinks.Where(x => x.SeekerVocabulary == this.VocabularyId).ToList<KnowledgeBrokerLink>();
                        _seekerVocabularyKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _seekerVocabularyKnowledgeBrokerLinks.CollectionChanged += SeekerVocabularyKnowledgeBrokerLinks_CollectionChanged;
                }
                return _seekerVocabularyKnowledgeBrokerLinks;
            }
            private set
            {
                if (_seekerVocabularyKnowledgeBrokerLinks != null)
                {
                    _seekerVocabularyKnowledgeBrokerLinks.CollectionChanged -= SeekerVocabularyKnowledgeBrokerLinks_CollectionChanged;
                }
                _seekerVocabularyKnowledgeBrokerLinks = value;
                if (_seekerVocabularyKnowledgeBrokerLinks != null)
                {
                    _seekerVocabularyKnowledgeBrokerLinks.CollectionChanged += SeekerVocabularyKnowledgeBrokerLinks_CollectionChanged;
                }
            }
        }

        private void SeekerVocabularyKnowledgeBrokerLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeBrokerLink>())
                {
                    item.SeekerVocabulary = this.VocabularyId;
                }
            }
        }

        private ObservableCollection<KnowledgeBrokerLink> _holderVocabularyKnowledgeBrokerLinks;

        [InverseProperty("VocabularyRef")]
        public virtual ObservableCollection<KnowledgeBrokerLink> HolderVocabularyKnowledgeBrokerLinks
        {
            get
            {
                if (_holderVocabularyKnowledgeBrokerLinks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access HolderVocabularyKnowledgeBrokerLinks - no database context is set. VocabularyId: " + this.VocabularyId + ".");
                        }
                        _holderVocabularyKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeBrokerLinks.Where(x => x.HolderVocabulary == this.VocabularyId).ToList<KnowledgeBrokerLink>();
                        _holderVocabularyKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _holderVocabularyKnowledgeBrokerLinks.CollectionChanged += HolderVocabularyKnowledgeBrokerLinks_CollectionChanged;
                }
                return _holderVocabularyKnowledgeBrokerLinks;
            }
            private set
            {
                if (_holderVocabularyKnowledgeBrokerLinks != null)
                {
                    _holderVocabularyKnowledgeBrokerLinks.CollectionChanged -= HolderVocabularyKnowledgeBrokerLinks_CollectionChanged;
                }
                _holderVocabularyKnowledgeBrokerLinks = value;
                if (_holderVocabularyKnowledgeBrokerLinks != null)
                {
                    _holderVocabularyKnowledgeBrokerLinks.CollectionChanged += HolderVocabularyKnowledgeBrokerLinks_CollectionChanged;
                }
            }
        }

        private void HolderVocabularyKnowledgeBrokerLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeBrokerLink>())
                {
                    item.HolderVocabulary = this.VocabularyId;
                }
            }
        }

        private ObservableCollection<CollectedSourceMaterial> _collectedSourceMaterials;

        [InverseProperty("Vocabulary")]
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
                            throw new InvalidOperationException("Cannot access CollectedSourceMaterials - no database context is set. VocabularyId: " + this.VocabularyId + ".");
                        }
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>();
                    }
                    else
                    {
                        var items = base.SoAContext.CollectedSourceMaterials.Where(x => x.OrganizedIntoScheme == this.VocabularyId).ToList<CollectedSourceMaterial>();
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
                    item.OrganizedIntoScheme = this.VocabularyId;
                }
            }
        }

        private ObservableCollection<SchemeRefinement> _schemeRefinements;

        [InverseProperty("VocabularyRef")]
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
                            throw new InvalidOperationException("Cannot access SchemeRefinements - no database context is set. VocabularyId: " + this.VocabularyId + ".");
                        }
                        _schemeRefinements = new ObservableCollection<SchemeRefinement>();
                    }
                    else
                    {
                        var items = base.SoAContext.SchemeRefinements.Where(x => x.Vocabulary == this.VocabularyId).ToList<SchemeRefinement>();
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
                    item.Vocabulary = this.VocabularyId;
                }
            }
        }

        private ObservableCollection<AiLabelingRun> _aiLabelingRuns;

        [InverseProperty("Vocabulary")]
        public virtual ObservableCollection<AiLabelingRun> AiLabelingRuns
        {
            get
            {
                if (_aiLabelingRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiLabelingRuns - no database context is set. VocabularyId: " + this.VocabularyId + ".");
                        }
                        _aiLabelingRuns = new ObservableCollection<AiLabelingRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiLabelingRuns.Where(x => x.GroundingScheme == this.VocabularyId).ToList<AiLabelingRun>();
                        _aiLabelingRuns = new ObservableCollection<AiLabelingRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiLabelingRuns.CollectionChanged += AiLabelingRuns_CollectionChanged;
                }
                return _aiLabelingRuns;
            }
            private set
            {
                if (_aiLabelingRuns != null)
                {
                    _aiLabelingRuns.CollectionChanged -= AiLabelingRuns_CollectionChanged;
                }
                _aiLabelingRuns = value;
                if (_aiLabelingRuns != null)
                {
                    _aiLabelingRuns.CollectionChanged += AiLabelingRuns_CollectionChanged;
                }
            }
        }

        private void AiLabelingRuns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiLabelingRun>())
                {
                    item.GroundingScheme = this.VocabularyId;
                }
            }
        }

        private ObservableCollection<SourceTermMention> _sourceTermMentions;

        [InverseProperty("Vocabulary")]
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
                            throw new InvalidOperationException("Cannot access SourceTermMentions - no database context is set. VocabularyId: " + this.VocabularyId + ".");
                        }
                        _sourceTermMentions = new ObservableCollection<SourceTermMention>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourceTermMentions.Where(x => x.ConceptScheme == this.VocabularyId).ToList<SourceTermMention>();
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
                    item.ConceptScheme = this.VocabularyId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.Procedure;
            _ = this.CommunitiesOfPractice;
            _ = this.VocabularyTerms;
            _ = this.SeekerVocabularyKnowledgeBrokerLinks;
            _ = this.HolderVocabularyKnowledgeBrokerLinks;
            _ = this.CollectedSourceMaterials;
            _ = this.SchemeRefinements;
            _ = this.AiLabelingRuns;
            _ = this.SourceTermMentions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
