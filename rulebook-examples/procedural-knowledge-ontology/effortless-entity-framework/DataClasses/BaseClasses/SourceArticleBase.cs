
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
    [Table("SourceArticles")]
    public class SourceArticleBase : SoAEntityBase
    {
        [Key]
        public string SourceArticleId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Title))); set { }
        }

        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Series { get; set; }
        public DateOnly? PublishedOn { get; set; }
        public string? LocalFileName { get; set; }
        public string? Thesis { get; set; }
        // Formula ClaimCount (rulebook: =COUNTIFS(ArticleClaims!{{SourceArticle}}, {{SourceArticleId}}))
        [NotMapped]
        public int? ClaimCount
        {
            get => F.AsInt(F.Memo(this, "ClaimCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ArticleClaim>(base.SoAContext, "ArticleClaims", __c => __c.ArticleClaims), __r => F.CritField(F.Of(__r.SourceArticle), F.Of(this.SourceArticleId))))))); set { }
        }

        // Formula CoveredClaimCount (rulebook: =COUNTIFS(ArticleClaims!{{SourceArticle}}, {{SourceArticleId}}, ArticleClaims!{{IsCovered}}, TRUE))
        [NotMapped]
        public int? CoveredClaimCount
        {
            get => F.AsInt(F.Memo(this, "CoveredClaimCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ArticleClaim>(base.SoAContext, "ArticleClaims", __c => __c.ArticleClaims), __r => F.CritField(F.Of(__r.SourceArticle), F.Of(this.SourceArticleId)) && F.CritLiteral(F.Of(__r.IsCovered), F.B(true))))))); set { }
        }

        // Formula AgreedClaimCount (rulebook: =COUNTIFS(ArticleClaims!{{SourceArticle}}, {{SourceArticleId}}, ArticleClaims!{{IsAgreed}}, TRUE))
        [NotMapped]
        public int? AgreedClaimCount
        {
            get => F.AsInt(F.Memo(this, "AgreedClaimCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ArticleClaim>(base.SoAContext, "ArticleClaims", __c => __c.ArticleClaims), __r => F.CritField(F.Of(__r.SourceArticle), F.Of(this.SourceArticleId)) && F.CritLiteral(F.Of(__r.IsAgreed), F.B(true))))))); set { }
        }

        // Formula UncoveredClaimCount (rulebook: ={{ClaimCount}} - {{CoveredClaimCount}})
        [NotMapped]
        public int? UncoveredClaimCount
        {
            get => F.AsInt(F.Memo(this, "UncoveredClaimCount", () => F.Integer(F.Sub(F.Of(this.ClaimCount), F.Of(this.CoveredClaimCount))))); set { }
        }

        // Formula CoveragePercent (rulebook: =IF({{ClaimCount}} = 0, 0, ROUND(100 * {{CoveredClaimCount}} / {{ClaimCount}}, 1)))
        [NotMapped]
        public decimal? CoveragePercent
        {
            get => F.AsDecimal(F.Memo(this, "CoveragePercent", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.ClaimCount), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.CoveredClaimCount)), F.Of(this.ClaimCount)), F.I(1))))); set { }
        }

        // Formula AgreedCoveragePercent (rulebook: =IF({{ClaimCount}} = 0, 0, ROUND(100 * {{AgreedClaimCount}} / {{ClaimCount}}, 1)))
        [NotMapped]
        public decimal? AgreedCoveragePercent
        {
            get => F.AsDecimal(F.Memo(this, "AgreedCoveragePercent", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.ClaimCount), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.AgreedClaimCount)), F.Of(this.ClaimCount)), F.I(1))))); set { }
        }

        // Formula IsFullyCovered (rulebook: =AND({{ClaimCount}} > 0, {{UncoveredClaimCount}} = 0))
        [NotMapped]
        public bool? IsFullyCovered
        {
            get => F.AsBool(F.Memo(this, "IsFullyCovered", () => F.And(F.Bool3(F.Cmp(F.Of(this.ClaimCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.UncoveredClaimCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<ArticleClaim> _articleClaims;

        [InverseProperty("SourceArticleRef")]
        public virtual ObservableCollection<ArticleClaim> ArticleClaims
        {
            get
            {
                if (_articleClaims == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ArticleClaims - no database context is set. SourceArticleId: " + this.SourceArticleId + ".");
                        }
                        _articleClaims = new ObservableCollection<ArticleClaim>();
                    }
                    else
                    {
                        var items = base.SoAContext.ArticleClaims.Where(x => x.SourceArticle == this.SourceArticleId).ToList<ArticleClaim>();
                        _articleClaims = new ObservableCollection<ArticleClaim>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _articleClaims.CollectionChanged += ArticleClaims_CollectionChanged;
                }
                return _articleClaims;
            }
            private set
            {
                if (_articleClaims != null)
                {
                    _articleClaims.CollectionChanged -= ArticleClaims_CollectionChanged;
                }
                _articleClaims = value;
                if (_articleClaims != null)
                {
                    _articleClaims.CollectionChanged += ArticleClaims_CollectionChanged;
                }
            }
        }

        private void ArticleClaims_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ArticleClaim>())
                {
                    item.SourceArticle = this.SourceArticleId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ArticleClaims;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
