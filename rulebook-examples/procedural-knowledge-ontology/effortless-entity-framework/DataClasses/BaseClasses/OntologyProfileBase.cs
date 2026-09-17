
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
    [Table("OntologyProfiles")]
    public class OntologyProfileBase : SoAEntityBase
    {
        [Key]
        public string OntologyProfileId { get; set; }

        // Formula Name (rulebook: ={{Label}} & " " & {{Version}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Label)), F.S(" "), F.Text(F.Of(this.Version))))); set { }
        }

        public string? Label { get; set; }
        public string? Version { get; set; }
        public string? VersionIri { get; set; }
        public string? NamespaceIri { get; set; }
        public string? License { get; set; }
        public string? Scope { get; set; }
        // Formula MappingCount (rulebook: =COUNTIFS(SemanticMappings!{{OntologyProfile}}, {{OntologyProfileId}}))
        [NotMapped]
        public int? MappingCount
        {
            get => F.AsInt(F.Memo(this, "MappingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SemanticMapping>(base.SoAContext, "SemanticMappings", __c => __c.SemanticMappings), __r => F.CritField(F.Of(__r.OntologyProfile), F.Of(this.OntologyProfileId))))))); set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        public DateTimeOffset? LastRevisedAt { get; set; }
        public DateTimeOffset? LastMajorRevisionAt { get; set; }
        public DateTimeOffset? DependencyReviewedAt { get; set; }
        // Formula DaysSinceLastRevision (rulebook: =IF({{LastRevisedAt}} = "", 0, DATETIME_DIFF({{AsOfInstant}}, {{LastRevisedAt}}, "days")))
        [NotMapped]
        public int? DaysSinceLastRevision
        {
            get => F.AsInt(F.Memo(this, "DaysSinceLastRevision", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.LastRevisedAt)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.LastRevisedAt), F.S("days")))))); set { }
        }

        // Formula DaysSinceMajorRevision (rulebook: =IF({{LastMajorRevisionAt}} = "", 0, DATETIME_DIFF({{AsOfInstant}}, {{LastMajorRevisionAt}}, "days")))
        [NotMapped]
        public int? DaysSinceMajorRevision
        {
            get => F.AsInt(F.Memo(this, "DaysSinceMajorRevision", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.LastMajorRevisionAt)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.LastMajorRevisionAt), F.S("days")))))); set { }
        }

        // Formula DaysSinceDependencyReviewed (rulebook: =IF({{DependencyReviewedAt}} = "", 0, DATETIME_DIFF({{AsOfInstant}}, {{DependencyReviewedAt}}, "days")))
        [NotMapped]
        public int? DaysSinceDependencyReviewed
        {
            get => F.AsInt(F.Memo(this, "DaysSinceDependencyReviewed", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.DependencyReviewedAt)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.DependencyReviewedAt), F.S("days")))))); set { }
        }

        // Formula RecentDeprecationCount (rulebook: =COUNTIFS(ExternalStandardTerms!{{OntologyProfile}}, {{OntologyProfileId}}, ExternalStandardTerms!{{IsRecentDeprecation}}, TRUE))
        [NotMapped]
        public int? RecentDeprecationCount
        {
            get => F.AsInt(F.Memo(this, "RecentDeprecationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ExternalStandardTerm>(base.SoAContext, "ExternalStandardTerms", __c => __c.ExternalStandardTerms), __r => F.CritField(F.Of(__r.OntologyProfile), F.Of(this.OntologyProfileId)) && F.CritLiteral(F.Of(__r.IsRecentDeprecation), F.B(true))))))); set { }
        }

        // Formula RequiresFrequentReview (rulebook: =OR({{RecentDeprecationCount}} >= 2, AND({{LastMajorRevisionAt}} <> "", {{DaysSinceMajorRevision}} < 730)))
        [NotMapped]
        public bool? RequiresFrequentReview
        {
            get => F.AsBool(F.Memo(this, "RequiresFrequentReview", () => F.Or(F.Bool3(F.Cmp(F.Of(this.RecentDeprecationCount), ">=", F.I(2))), F.Bool3(F.And(F.Bool3(F.IsNotBlank(F.Of(this.LastMajorRevisionAt))), F.Bool3(F.Cmp(F.Of(this.DaysSinceMajorRevision), "<", F.I(730)))))))); set { }
        }

        // Formula ChangeRateProfile (rulebook: =IF({{RecentDeprecationCount}} >= 2, "FrequentDeprecation", IF(AND({{LastMajorRevisionAt}} <> "", {{DaysSinceMajorRevision}} < 730), "RecentMajorRevision", IF(AND({{LastRevisedAt}} <> "", {{DaysSinceLastRevision}} > 3650), "Dormant", "Unassessed"))))
        [NotMapped]
        public string? ChangeRateProfile
        {
            get => F.AsString(F.Memo(this, "ChangeRateProfile", () => (F.Truthy(F.Bool3(F.Cmp(F.Of(this.RecentDeprecationCount), ">=", F.I(2)))) ? F.S("FrequentDeprecation") : (F.Truthy(F.Bool3(F.And(F.Bool3(F.IsNotBlank(F.Of(this.LastMajorRevisionAt))), F.Bool3(F.Cmp(F.Of(this.DaysSinceMajorRevision), "<", F.I(730)))))) ? F.S("RecentMajorRevision") : (F.Truthy(F.Bool3(F.And(F.Bool3(F.IsNotBlank(F.Of(this.LastRevisedAt))), F.Bool3(F.Cmp(F.Of(this.DaysSinceLastRevision), ">", F.I(3650)))))) ? F.S("Dormant") : F.S("Unassessed")))))); set { }
        }

        // Formula IsReviewOverdueForChangeRate (rulebook: =AND({{RequiresFrequentReview}}, OR({{DependencyReviewedAt}} = "", {{DaysSinceDependencyReviewed}} > 180)))
        [NotMapped]
        public bool? IsReviewOverdueForChangeRate
        {
            get => F.AsBool(F.Memo(this, "IsReviewOverdueForChangeRate", () => F.And(F.Bool3(F.Of(this.RequiresFrequentReview)), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.DependencyReviewedAt))), F.Bool3(F.Cmp(F.Of(this.DaysSinceDependencyReviewed), ">", F.I(180)))))))); set { }
        }

        public int? AdoptionStage { get; set; }
        public DateTimeOffset? AdoptedAt { get; set; }
        // Formula PrerequisiteAdoptedAt (rulebook: =INDEX(OntologyProfiles!{{AdoptedAt}}, MATCH({{PrerequisiteProfile}}, OntologyProfiles!{{OntologyProfileId}}, 0)))
        [NotMapped]
        public DateTimeOffset? PrerequisiteAdoptedAt
        {
            get => F.AsDateTime(F.Memo(this, "PrerequisiteAdoptedAt", () => F.Lookup<OntologyProfile>(this, "OntologyProfiles", "OntologyProfileId", __c => __c.OntologyProfiles, __r => F.Of(__r.OntologyProfileId), F.Of(this.PrerequisiteProfile), __r => F.Of(__r.AdoptedAt), () => F.Of(new OntologyProfile().AdoptedAt)))); set { }
        }

        // Formula SkipsAdoptionPath (rulebook: =AND({{AdoptionStage}} > 1, OR({{PrerequisiteProfile}} = "", {{PrerequisiteAdoptedAt}} = "", {{PrerequisiteAdoptedAt}} > {{AdoptedAt}})))
        [NotMapped]
        public bool? SkipsAdoptionPath
        {
            get => F.AsBool(F.Memo(this, "SkipsAdoptionPath", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.AdoptionStage)), ">", F.I(1))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.PrerequisiteProfile))), F.Bool3(F.IsBlank(F.Of(this.PrerequisiteAdoptedAt))), F.Bool3(F.Cmp(F.Of(this.PrerequisiteAdoptedAt), ">", F.Nullif(F.Of(this.AdoptedAt))))))))); set { }
        }


        public string? EvaluationContext { get; set; }
        public string? PrerequisiteProfile { get; set; }

        private EvaluationContext _evaluationContextRef;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContextRef
        {
            get
            {
                if (_evaluationContextRef == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContextRef - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContextRef = base.SoAContext.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContextRef != null)
                    {
                        base.SoAContext.Attach(_evaluationContextRef);
                    }
                }
                return _evaluationContextRef;
            }
            set
            {
                if (_evaluationContextRef != value)
                {
                    _evaluationContextRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_evaluationContextRef != null)
                    {
                        EvaluationContext = _evaluationContextRef.EvaluationContextId;
                    }
                }
            }
        }

        private OntologyProfile _ontologyProfile;

        [ForeignKey("PrerequisiteProfile")]
        public virtual OntologyProfile OntologyProfile
        {
            get
            {
                if (_ontologyProfile == null && !string.IsNullOrEmpty(PrerequisiteProfile))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OntologyProfile - no database context is set. PrerequisiteProfile: " + PrerequisiteProfile + ".");
                        }
                        return null;
                    }
                    _ontologyProfile = base.SoAContext.OntologyProfiles.Find(PrerequisiteProfile);
                    if (_ontologyProfile != null)
                    {
                        base.SoAContext.Attach(_ontologyProfile);
                    }
                }
                return _ontologyProfile;
            }
            set
            {
                if (_ontologyProfile != value)
                {
                    _ontologyProfile = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_ontologyProfile != null)
                    {
                        PrerequisiteProfile = _ontologyProfile.OntologyProfileId;
                    }
                }
            }
        }

        private ObservableCollection<OntologyProfile> _ontologyProfiles;

        [InverseProperty("OntologyProfile")]
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
                            throw new InvalidOperationException("Cannot access OntologyProfiles - no database context is set. OntologyProfileId: " + this.OntologyProfileId + ".");
                        }
                        _ontologyProfiles = new ObservableCollection<OntologyProfile>();
                    }
                    else
                    {
                        var items = base.SoAContext.OntologyProfiles.Where(x => x.PrerequisiteProfile == this.OntologyProfileId).ToList<OntologyProfile>();
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
                    item.PrerequisiteProfile = this.OntologyProfileId;
                }
            }
        }

        private ObservableCollection<SemanticMapping> _semanticMappings;

        [InverseProperty("OntologyProfileRef")]
        public virtual ObservableCollection<SemanticMapping> SemanticMappings
        {
            get
            {
                if (_semanticMappings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SemanticMappings - no database context is set. OntologyProfileId: " + this.OntologyProfileId + ".");
                        }
                        _semanticMappings = new ObservableCollection<SemanticMapping>();
                    }
                    else
                    {
                        var items = base.SoAContext.SemanticMappings.Where(x => x.OntologyProfile == this.OntologyProfileId).ToList<SemanticMapping>();
                        _semanticMappings = new ObservableCollection<SemanticMapping>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _semanticMappings.CollectionChanged += SemanticMappings_CollectionChanged;
                }
                return _semanticMappings;
            }
            private set
            {
                if (_semanticMappings != null)
                {
                    _semanticMappings.CollectionChanged -= SemanticMappings_CollectionChanged;
                }
                _semanticMappings = value;
                if (_semanticMappings != null)
                {
                    _semanticMappings.CollectionChanged += SemanticMappings_CollectionChanged;
                }
            }
        }

        private void SemanticMappings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SemanticMapping>())
                {
                    item.OntologyProfile = this.OntologyProfileId;
                }
            }
        }

        private ObservableCollection<ClaimEvidence> _claimEvidence;

        [InverseProperty("OntologyProfileRef")]
        public virtual ObservableCollection<ClaimEvidence> ClaimEvidence
        {
            get
            {
                if (_claimEvidence == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ClaimEvidence - no database context is set. OntologyProfileId: " + this.OntologyProfileId + ".");
                        }
                        _claimEvidence = new ObservableCollection<ClaimEvidence>();
                    }
                    else
                    {
                        var items = base.SoAContext.ClaimEvidence.Where(x => x.OntologyProfile == this.OntologyProfileId).ToList<ClaimEvidence>();
                        _claimEvidence = new ObservableCollection<ClaimEvidence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _claimEvidence.CollectionChanged += ClaimEvidence_CollectionChanged;
                }
                return _claimEvidence;
            }
            private set
            {
                if (_claimEvidence != null)
                {
                    _claimEvidence.CollectionChanged -= ClaimEvidence_CollectionChanged;
                }
                _claimEvidence = value;
                if (_claimEvidence != null)
                {
                    _claimEvidence.CollectionChanged += ClaimEvidence_CollectionChanged;
                }
            }
        }

        private void ClaimEvidence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ClaimEvidence>())
                {
                    item.OntologyProfile = this.OntologyProfileId;
                }
            }
        }

        private ObservableCollection<ExternalStandardTerm> _externalStandardTerms;

        [InverseProperty("OntologyProfileRef")]
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
                            throw new InvalidOperationException("Cannot access ExternalStandardTerms - no database context is set. OntologyProfileId: " + this.OntologyProfileId + ".");
                        }
                        _externalStandardTerms = new ObservableCollection<ExternalStandardTerm>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExternalStandardTerms.Where(x => x.OntologyProfile == this.OntologyProfileId).ToList<ExternalStandardTerm>();
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
                    item.OntologyProfile = this.OntologyProfileId;
                }
            }
        }

        private ObservableCollection<ExternalDependencyRevision> _externalDependencyRevisions;

        [InverseProperty("OntologyProfileRef")]
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
                            throw new InvalidOperationException("Cannot access ExternalDependencyRevisions - no database context is set. OntologyProfileId: " + this.OntologyProfileId + ".");
                        }
                        _externalDependencyRevisions = new ObservableCollection<ExternalDependencyRevision>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExternalDependencyRevisions.Where(x => x.OntologyProfile == this.OntologyProfileId).ToList<ExternalDependencyRevision>();
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
                    item.OntologyProfile = this.OntologyProfileId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.EvaluationContextRef;
            _ = this.OntologyProfile;
            _ = this.OntologyProfiles;
            _ = this.SemanticMappings;
            _ = this.ClaimEvidence;
            _ = this.ExternalStandardTerms;
            _ = this.ExternalDependencyRevisions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
