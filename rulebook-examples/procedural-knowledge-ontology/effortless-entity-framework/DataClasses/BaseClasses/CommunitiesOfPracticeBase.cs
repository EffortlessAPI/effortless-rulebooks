
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
    [Table("CommunitiesOfPractice")]
    public class CommunitiesOfPracticeBase : SoAEntityBase
    {
        [Key]
        public string CommunityOfPracticeId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Purpose { get; set; }
        public string? Cadence { get; set; }
        public string? SemanticTypeIri { get; set; }
        public string? Norms { get; set; }
        public string? Origin { get; set; }
        public int? OpenApprenticeshipPlaces { get; set; }
        public bool? IsColocatedTrade { get; set; }
        // Formula HasOwnVocabularyAndNorms (rulebook: =AND({{OwnVocabulary}} <> "", {{Norms}} <> ""))
        [NotMapped]
        public bool? HasOwnVocabularyAndNorms
        {
            get => F.AsBool(F.Memo(this, "HasOwnVocabularyAndNorms", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.OwnVocabulary))), F.Bool3(F.IsNotBlank(F.Of(this.Norms)))))); set { }
        }

        // Formula SharingEventCount (rulebook: =COUNTIFS(KnowledgeTransfers!{{CommunityOfPractice}}, {{CommunityOfPracticeId}}))
        [NotMapped]
        public int? SharingEventCount
        {
            get => F.AsInt(F.Memo(this, "SharingEventCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTransfer>(base.SoAContext, "KnowledgeTransfers", __c => __c.KnowledgeTransfers), __r => F.CritField(F.Of(__r.CommunityOfPractice), F.Of(this.CommunityOfPracticeId))))))); set { }
        }

        // Formula IsMandatedWithoutSharingNorm (rulebook: =AND({{Origin}} <> "", {{Origin}} <> "Grassroots", {{SharingEventCount}} = 0))
        [NotMapped]
        public bool? IsMandatedWithoutSharingNorm
        {
            get => F.AsBool(F.Memo(this, "IsMandatedWithoutSharingNorm", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.Origin))), F.Bool3(F.Ne(F.Nullif(F.Of(this.Origin)), F.S("Grassroots"))), F.Bool3(F.Eq(F.Of(this.SharingEventCount), F.I(0)))))); set { }
        }

        // Formula InterconnectedKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{CommunityOfPractice}}, {{CommunityOfPracticeId}}, KnowHowCarriers!{{BuildsOnSameCommunityKnowHow}}, TRUE))
        [NotMapped]
        public int? InterconnectedKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "InterconnectedKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.CommunityOfPractice), F.Of(this.CommunityOfPracticeId)) && F.CritLiteral(F.Of(__r.BuildsOnSameCommunityKnowHow), F.B(true))))))); set { }
        }

        // Formula PhysicalKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{CommunityOfPractice}}, {{CommunityOfPracticeId}}, KnowHowCarriers!{{WorkMedium}}, "Physical"))
        [NotMapped]
        public int? PhysicalKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "PhysicalKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.CommunityOfPractice), F.Of(this.CommunityOfPracticeId)) && F.CritLiteral(F.Of(__r.WorkMedium), F.S("Physical"))))))); set { }
        }

        // Formula DigitalKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{CommunityOfPractice}}, {{CommunityOfPracticeId}}, KnowHowCarriers!{{WorkMedium}}, "Digital"))
        [NotMapped]
        public int? DigitalKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "DigitalKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.CommunityOfPractice), F.Of(this.CommunityOfPracticeId)) && F.CritLiteral(F.Of(__r.WorkMedium), F.S("Digital"))))))); set { }
        }

        // Formula PersonCarriedKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{CommunityOfPractice}}, {{CommunityOfPracticeId}}, KnowHowCarriers!{{CarrierKind}}, "Person"))
        [NotMapped]
        public int? PersonCarriedKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "PersonCarriedKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.CommunityOfPractice), F.Of(this.CommunityOfPracticeId)) && F.CritLiteral(F.Of(__r.CarrierKind), F.S("Person"))))))); set { }
        }

        // Formula SpansPhysicalAndDigitalWithHumans (rulebook: =AND({{PhysicalKnowHowCount}} > 0, {{DigitalKnowHowCount}} > 0, {{PersonCarriedKnowHowCount}} > 0))
        [NotMapped]
        public bool? SpansPhysicalAndDigitalWithHumans
        {
            get => F.AsBool(F.Memo(this, "SpansPhysicalAndDigitalWithHumans", () => F.And(F.Bool3(F.Cmp(F.Of(this.PhysicalKnowHowCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.DigitalKnowHowCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.PersonCarriedKnowHowCount), ">", F.I(0)))))); set { }
        }

        // Formula ExternalMemberCount (rulebook: =COUNTIFS(CommunityMemberships!{{CommunityOfPractice}}, {{CommunityOfPracticeId}}, CommunityMemberships!{{IsExternalMember}}, TRUE))
        [NotMapped]
        public int? ExternalMemberCount
        {
            get => F.AsInt(F.Memo(this, "ExternalMemberCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CommunityMembership>(base.SoAContext, "CommunityMemberships", __c => __c.CommunityMemberships), __r => F.CritField(F.Of(__r.CommunityOfPractice), F.Of(this.CommunityOfPracticeId)) && F.CritLiteral(F.Of(__r.IsExternalMember), F.B(true))))))); set { }
        }

        // Formula SpecialistMemberCount (rulebook: =COUNTIFS(CommunityMemberships!{{CommunityOfPractice}}, {{CommunityOfPracticeId}}, CommunityMemberships!{{IsSpecialist}}, TRUE))
        [NotMapped]
        public int? SpecialistMemberCount
        {
            get => F.AsInt(F.Memo(this, "SpecialistMemberCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CommunityMembership>(base.SoAContext, "CommunityMemberships", __c => __c.CommunityMemberships), __r => F.CritField(F.Of(__r.CommunityOfPractice), F.Of(this.CommunityOfPracticeId)) && F.CritLiteral(F.Of(__r.IsSpecialist), F.B(true))))))); set { }
        }

        // Formula EmployerMoveCount (rulebook: =COUNTIFS(KnowledgeTransfers!{{CommunityOfPractice}}, {{CommunityOfPracticeId}}, KnowledgeTransfers!{{Channel}}, "EmployerChange"))
        [NotMapped]
        public int? EmployerMoveCount
        {
            get => F.AsInt(F.Memo(this, "EmployerMoveCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTransfer>(base.SoAContext, "KnowledgeTransfers", __c => __c.KnowledgeTransfers), __r => F.CritField(F.Of(__r.CommunityOfPractice), F.Of(this.CommunityOfPracticeId)) && F.CritLiteral(F.Of(__r.Channel), F.S("EmployerChange"))))))); set { }
        }

        // Formula IsCrossFirmPracticeCluster (rulebook: =AND({{ExternalMemberCount}} >= 2, {{EmployerMoveCount}} >= 1, {{SpecialistMemberCount}} >= 1))
        [NotMapped]
        public bool? IsCrossFirmPracticeCluster
        {
            get => F.AsBool(F.Memo(this, "IsCrossFirmPracticeCluster", () => F.And(F.Bool3(F.Cmp(F.Of(this.ExternalMemberCount), ">=", F.I(2))), F.Bool3(F.Cmp(F.Of(this.EmployerMoveCount), ">=", F.I(1))), F.Bool3(F.Cmp(F.Of(this.SpecialistMemberCount), ">=", F.I(1)))))); set { }
        }

        // Formula RecentApprenticeshipCount (rulebook: =COUNTIFS(Mentorships!{{CommunityOfPractice}}, {{CommunityOfPracticeId}}, Mentorships!{{IsRecentApprenticeship}}, TRUE))
        [NotMapped]
        public int? RecentApprenticeshipCount
        {
            get => F.AsInt(F.Memo(this, "RecentApprenticeshipCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Mentorship>(base.SoAContext, "Mentorships", __c => __c.Mentorships), __r => F.CritField(F.Of(__r.CommunityOfPractice), F.Of(this.CommunityOfPracticeId)) && F.CritLiteral(F.Of(__r.IsRecentApprenticeship), F.B(true))))))); set { }
        }

        // Formula IsCirculationEndingForLackOfApprentices (rulebook: =AND({{EmployerMoveCount}} >= 1, {{OpenApprenticeshipPlaces}} > 0, {{RecentApprenticeshipCount}} = 0))
        [NotMapped]
        public bool? IsCirculationEndingForLackOfApprentices
        {
            get => F.AsBool(F.Memo(this, "IsCirculationEndingForLackOfApprentices", () => F.And(F.Bool3(F.Cmp(F.Of(this.EmployerMoveCount), ">=", F.I(1))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.OpenApprenticeshipPlaces)), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.RecentApprenticeshipCount), F.I(0)))))); set { }
        }

        // Formula AmbientAbsorptionCount (rulebook: =COUNTIFS(KnowledgeTransfers!{{CommunityOfPractice}}, {{CommunityOfPracticeId}}, KnowledgeTransfers!{{IsAmbientAbsorptionByNonPractitioner}}, TRUE))
        [NotMapped]
        public int? AmbientAbsorptionCount
        {
            get => F.AsInt(F.Memo(this, "AmbientAbsorptionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTransfer>(base.SoAContext, "KnowledgeTransfers", __c => __c.KnowledgeTransfers), __r => F.CritField(F.Of(__r.CommunityOfPractice), F.Of(this.CommunityOfPracticeId)) && F.CritLiteral(F.Of(__r.IsAmbientAbsorptionByNonPractitioner), F.B(true))))))); set { }
        }

        // Formula HasAmbientTradeKnowHow (rulebook: =AND({{IsColocatedTrade}}, {{AmbientAbsorptionCount}} >= 1))
        [NotMapped]
        public bool? HasAmbientTradeKnowHow
        {
            get => F.AsBool(F.Memo(this, "HasAmbientTradeKnowHow", () => F.And(F.IsTrueV(F.Of(this.IsColocatedTrade)), F.Bool3(F.Cmp(F.Of(this.AmbientAbsorptionCount), ">=", F.I(1)))))); set { }
        }


        public string? Organization { get; set; }
        public string? StewardRole { get; set; }
        public string? OwnVocabulary { get; set; }

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

        private Role _role;

        [ForeignKey("StewardRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(StewardRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. StewardRole: " + StewardRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(StewardRole);
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
                        StewardRole = _role.RoleId;
                    }
                }
            }
        }

        private Vocabulary _vocabulary;

        [ForeignKey("OwnVocabulary")]
        public virtual Vocabulary Vocabulary
        {
            get
            {
                if (_vocabulary == null && !string.IsNullOrEmpty(OwnVocabulary))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Vocabulary - no database context is set. OwnVocabulary: " + OwnVocabulary + ".");
                        }
                        return null;
                    }
                    _vocabulary = base.SoAContext.Vocabularies.Find(OwnVocabulary);
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
                        OwnVocabulary = _vocabulary.VocabularyId;
                    }
                }
            }
        }

        private ObservableCollection<Mentorship> _mentorships;

        [InverseProperty("CommunitiesOfPractice")]
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
                            throw new InvalidOperationException("Cannot access Mentorships - no database context is set. CommunityOfPracticeId: " + this.CommunityOfPracticeId + ".");
                        }
                        _mentorships = new ObservableCollection<Mentorship>();
                    }
                    else
                    {
                        var items = base.SoAContext.Mentorships.Where(x => x.CommunityOfPractice == this.CommunityOfPracticeId).ToList<Mentorship>();
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
                    item.CommunityOfPractice = this.CommunityOfPracticeId;
                }
            }
        }

        private ObservableCollection<LearningActivity> _learningActivities;

        [InverseProperty("CommunitiesOfPractice")]
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
                            throw new InvalidOperationException("Cannot access LearningActivities - no database context is set. CommunityOfPracticeId: " + this.CommunityOfPracticeId + ".");
                        }
                        _learningActivities = new ObservableCollection<LearningActivity>();
                    }
                    else
                    {
                        var items = base.SoAContext.LearningActivities.Where(x => x.CommunityOfPractice == this.CommunityOfPracticeId).ToList<LearningActivity>();
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
                    item.CommunityOfPractice = this.CommunityOfPracticeId;
                }
            }
        }

        private ObservableCollection<KnowHowCarrier> _knowHowCarriers;

        [InverseProperty("CommunitiesOfPractice")]
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
                            throw new InvalidOperationException("Cannot access KnowHowCarriers - no database context is set. CommunityOfPracticeId: " + this.CommunityOfPracticeId + ".");
                        }
                        _knowHowCarriers = new ObservableCollection<KnowHowCarrier>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowHowCarriers.Where(x => x.CommunityOfPractice == this.CommunityOfPracticeId).ToList<KnowHowCarrier>();
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
                    item.CommunityOfPractice = this.CommunityOfPracticeId;
                }
            }
        }

        private ObservableCollection<KnowledgeTransfer> _knowledgeTransfers;

        [InverseProperty("CommunitiesOfPractice")]
        public virtual ObservableCollection<KnowledgeTransfer> KnowledgeTransfers
        {
            get
            {
                if (_knowledgeTransfers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeTransfers - no database context is set. CommunityOfPracticeId: " + this.CommunityOfPracticeId + ".");
                        }
                        _knowledgeTransfers = new ObservableCollection<KnowledgeTransfer>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTransfers.Where(x => x.CommunityOfPractice == this.CommunityOfPracticeId).ToList<KnowledgeTransfer>();
                        _knowledgeTransfers = new ObservableCollection<KnowledgeTransfer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeTransfers.CollectionChanged += KnowledgeTransfers_CollectionChanged;
                }
                return _knowledgeTransfers;
            }
            private set
            {
                if (_knowledgeTransfers != null)
                {
                    _knowledgeTransfers.CollectionChanged -= KnowledgeTransfers_CollectionChanged;
                }
                _knowledgeTransfers = value;
                if (_knowledgeTransfers != null)
                {
                    _knowledgeTransfers.CollectionChanged += KnowledgeTransfers_CollectionChanged;
                }
            }
        }

        private void KnowledgeTransfers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeTransfer>())
                {
                    item.CommunityOfPractice = this.CommunityOfPracticeId;
                }
            }
        }

        private ObservableCollection<CommunityMembership> _communityMemberships;

        [InverseProperty("CommunitiesOfPractice")]
        public virtual ObservableCollection<CommunityMembership> CommunityMemberships
        {
            get
            {
                if (_communityMemberships == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunityMemberships - no database context is set. CommunityOfPracticeId: " + this.CommunityOfPracticeId + ".");
                        }
                        _communityMemberships = new ObservableCollection<CommunityMembership>();
                    }
                    else
                    {
                        var items = base.SoAContext.CommunityMemberships.Where(x => x.CommunityOfPractice == this.CommunityOfPracticeId).ToList<CommunityMembership>();
                        _communityMemberships = new ObservableCollection<CommunityMembership>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _communityMemberships.CollectionChanged += CommunityMemberships_CollectionChanged;
                }
                return _communityMemberships;
            }
            private set
            {
                if (_communityMemberships != null)
                {
                    _communityMemberships.CollectionChanged -= CommunityMemberships_CollectionChanged;
                }
                _communityMemberships = value;
                if (_communityMemberships != null)
                {
                    _communityMemberships.CollectionChanged += CommunityMemberships_CollectionChanged;
                }
            }
        }

        private void CommunityMemberships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CommunityMembership>())
                {
                    item.CommunityOfPractice = this.CommunityOfPracticeId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.Role;
            _ = this.Vocabulary;
            _ = this.Mentorships;
            _ = this.LearningActivities;
            _ = this.KnowHowCarriers;
            _ = this.KnowledgeTransfers;
            _ = this.CommunityMemberships;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
