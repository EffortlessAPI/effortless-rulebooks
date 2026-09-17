
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
    [Table("ArticleClaims")]
    public class ArticleClaimBase : SoAEntityBase
    {
        [Key]
        public string ArticleClaimId { get; set; }

        // Formula Name (rulebook: ={{ArticleClaimId}} & ": " & LEFT({{ClaimText}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ArticleClaimId)), F.S(": "), F.Text(F.Left(F.Of(this.ClaimText), F.I(60)))))); set { }
        }

        public string? ClaimKind { get; set; }
        public string? SectionRef { get; set; }
        public string? ClaimText { get; set; }
        // Formula RequiredEvidence (rulebook: =IF({{ClaimKind}} = "Concept", "a table with rows or a field with data", IF({{ClaimKind}} = "CompetencyQuestion", "an answered role question with a computed answer", IF({{ClaimKind}} = "Standard", "a mapped ontology profile or an applied knowledge method", IF({{ClaimKind}} = "Scenario", "a procedure with recorded executions", "a discriminating witness invented for a role question")))))
        [NotMapped]
        public string? RequiredEvidence
        {
            get => F.AsString(F.Memo(this, "RequiredEvidence", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.ClaimKind)), F.S("Concept")))) ? F.S("a table with rows or a field with data") : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.ClaimKind)), F.S("CompetencyQuestion")))) ? F.S("an answered role question with a computed answer") : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.ClaimKind)), F.S("Standard")))) ? F.S("a mapped ontology profile or an applied knowledge method") : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.ClaimKind)), F.S("Scenario")))) ? F.S("a procedure with recorded executions") : F.S("a discriminating witness invented for a role question"))))))); set { }
        }

        // Formula EvidenceCount (rulebook: =COUNTIFS(ClaimEvidence!{{ArticleClaim}}, {{ArticleClaimId}}))
        [NotMapped]
        public int? EvidenceCount
        {
            get => F.AsInt(F.Memo(this, "EvidenceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ClaimEvidence>(base.SoAContext, "ClaimEvidence", __c => __c.ClaimEvidence), __r => F.CritField(F.Of(__r.ArticleClaim), F.Of(this.ArticleClaimId))))))); set { }
        }

        // Formula ValidEvidenceCount (rulebook: =COUNTIFS(ClaimEvidence!{{ArticleClaim}}, {{ArticleClaimId}}, ClaimEvidence!{{IsValid}}, TRUE))
        [NotMapped]
        public int? ValidEvidenceCount
        {
            get => F.AsInt(F.Memo(this, "ValidEvidenceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ClaimEvidence>(base.SoAContext, "ClaimEvidence", __c => __c.ClaimEvidence), __r => F.CritField(F.Of(__r.ArticleClaim), F.Of(this.ArticleClaimId)) && F.CritLiteral(F.Of(__r.IsValid), F.B(true))))))); set { }
        }

        // Formula AgreedEvidenceCount (rulebook: =COUNTIFS(ClaimEvidence!{{ArticleClaim}}, {{ArticleClaimId}}, ClaimEvidence!{{IsAgreedEvidence}}, TRUE))
        [NotMapped]
        public int? AgreedEvidenceCount
        {
            get => F.AsInt(F.Memo(this, "AgreedEvidenceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ClaimEvidence>(base.SoAContext, "ClaimEvidence", __c => __c.ClaimEvidence), __r => F.CritField(F.Of(__r.ArticleClaim), F.Of(this.ArticleClaimId)) && F.CritLiteral(F.Of(__r.IsAgreedEvidence), F.B(true))))))); set { }
        }

        // Formula IsCovered (rulebook: ={{ValidEvidenceCount}} > 0)
        [NotMapped]
        public bool? IsCovered
        {
            get => F.AsBool(F.Memo(this, "IsCovered", () => F.Cmp(F.Of(this.ValidEvidenceCount), ">", F.I(0)))); set { }
        }

        // Formula IsAgreed (rulebook: ={{AgreedEvidenceCount}} > 0)
        [NotMapped]
        public bool? IsAgreed
        {
            get => F.AsBool(F.Memo(this, "IsAgreed", () => F.Cmp(F.Of(this.AgreedEvidenceCount), ">", F.I(0)))); set { }
        }

        // Formula HasRejectedEvidence (rulebook: =AND({{EvidenceCount}} > 0, {{ValidEvidenceCount}} = 0))
        [NotMapped]
        public bool? HasRejectedEvidence
        {
            get => F.AsBool(F.Memo(this, "HasRejectedEvidence", () => F.And(F.Bool3(F.Cmp(F.Of(this.EvidenceCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.ValidEvidenceCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? SourceArticle { get; set; }

        private SourceArticle _sourceArticleRef;

        [ForeignKey("SourceArticle")]
        public virtual SourceArticle SourceArticleRef
        {
            get
            {
                if (_sourceArticleRef == null && !string.IsNullOrEmpty(SourceArticle))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SourceArticleRef - no database context is set. SourceArticle: " + SourceArticle + ".");
                        }
                        return null;
                    }
                    _sourceArticleRef = base.SoAContext.SourceArticles.Find(SourceArticle);
                    if (_sourceArticleRef != null)
                    {
                        base.SoAContext.Attach(_sourceArticleRef);
                    }
                }
                return _sourceArticleRef;
            }
            set
            {
                if (_sourceArticleRef != value)
                {
                    _sourceArticleRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_sourceArticleRef != null)
                    {
                        SourceArticle = _sourceArticleRef.SourceArticleId;
                    }
                }
            }
        }

        private ObservableCollection<ClaimEvidence> _claimEvidence;

        [InverseProperty("ArticleClaimRef")]
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
                            throw new InvalidOperationException("Cannot access ClaimEvidence - no database context is set. ArticleClaimId: " + this.ArticleClaimId + ".");
                        }
                        _claimEvidence = new ObservableCollection<ClaimEvidence>();
                    }
                    else
                    {
                        var items = base.SoAContext.ClaimEvidence.Where(x => x.ArticleClaim == this.ArticleClaimId).ToList<ClaimEvidence>();
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
                    item.ArticleClaim = this.ArticleClaimId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.SourceArticleRef;
            _ = this.ClaimEvidence;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
