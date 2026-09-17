
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
    [Table("QualityCriteria")]
    public class QualityCriteriaBase : SoAEntityBase
    {
        [Key]
        public string QualityCriterionId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Definition { get; set; }
        public bool? IsStated { get; set; }
        // Formula AssessmentCount (rulebook: =COUNTIFS(QualityAssessments!{{QualityCriterion}}, {{QualityCriterionId}}))
        [NotMapped]
        public int? AssessmentCount
        {
            get => F.AsInt(F.Memo(this, "AssessmentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<QualityAssessment>(base.SoAContext, "QualityAssessments", __c => __c.QualityAssessments), __r => F.CritField(F.Of(__r.QualityCriterion), F.Of(this.QualityCriterionId))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<QualityAssessment> _qualityAssessments;

        [InverseProperty("QualityCriteria")]
        public virtual ObservableCollection<QualityAssessment> QualityAssessments
        {
            get
            {
                if (_qualityAssessments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access QualityAssessments - no database context is set. QualityCriterionId: " + this.QualityCriterionId + ".");
                        }
                        _qualityAssessments = new ObservableCollection<QualityAssessment>();
                    }
                    else
                    {
                        var items = base.SoAContext.QualityAssessments.Where(x => x.QualityCriterion == this.QualityCriterionId).ToList<QualityAssessment>();
                        _qualityAssessments = new ObservableCollection<QualityAssessment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _qualityAssessments.CollectionChanged += QualityAssessments_CollectionChanged;
                }
                return _qualityAssessments;
            }
            private set
            {
                if (_qualityAssessments != null)
                {
                    _qualityAssessments.CollectionChanged -= QualityAssessments_CollectionChanged;
                }
                _qualityAssessments = value;
                if (_qualityAssessments != null)
                {
                    _qualityAssessments.CollectionChanged += QualityAssessments_CollectionChanged;
                }
            }
        }

        private void QualityAssessments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<QualityAssessment>())
                {
                    item.QualityCriterion = this.QualityCriterionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.QualityAssessments;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
