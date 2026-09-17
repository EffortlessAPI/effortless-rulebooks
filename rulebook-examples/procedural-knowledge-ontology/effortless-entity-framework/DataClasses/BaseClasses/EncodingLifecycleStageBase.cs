
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
    [Table("EncodingLifecycleStages")]
    public class EncodingLifecycleStageBase : SoAEntityBase
    {
        [Key]
        public string EncodingLifecycleStageId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public int? StageOrder { get; set; }
        public string? Activity { get; set; }
        // Formula AnnotationCount (rulebook: =COUNTIFS(ModelAnnotations!{{LifecycleStage}}, {{EncodingLifecycleStageId}}))
        [NotMapped]
        public int? AnnotationCount
        {
            get => F.AsInt(F.Memo(this, "AnnotationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelAnnotation>(base.SoAContext, "ModelAnnotations", __c => __c.ModelAnnotations), __r => F.CritField(F.Of(__r.LifecycleStage), F.Of(this.EncodingLifecycleStageId))))))); set { }
        }

        // Formula IsStageWithoutFeedback (rulebook: ={{AnnotationCount}} = 0)
        [NotMapped]
        public bool? IsStageWithoutFeedback
        {
            get => F.AsBool(F.Memo(this, "IsStageWithoutFeedback", () => F.Eq(F.Of(this.AnnotationCount), F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<ModelAnnotation> _modelAnnotations;

        [InverseProperty("EncodingLifecycleStage")]
        public virtual ObservableCollection<ModelAnnotation> ModelAnnotations
        {
            get
            {
                if (_modelAnnotations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelAnnotations - no database context is set. EncodingLifecycleStageId: " + this.EncodingLifecycleStageId + ".");
                        }
                        _modelAnnotations = new ObservableCollection<ModelAnnotation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelAnnotations.Where(x => x.LifecycleStage == this.EncodingLifecycleStageId).ToList<ModelAnnotation>();
                        _modelAnnotations = new ObservableCollection<ModelAnnotation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelAnnotations.CollectionChanged += ModelAnnotations_CollectionChanged;
                }
                return _modelAnnotations;
            }
            private set
            {
                if (_modelAnnotations != null)
                {
                    _modelAnnotations.CollectionChanged -= ModelAnnotations_CollectionChanged;
                }
                _modelAnnotations = value;
                if (_modelAnnotations != null)
                {
                    _modelAnnotations.CollectionChanged += ModelAnnotations_CollectionChanged;
                }
            }
        }

        private void ModelAnnotations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelAnnotation>())
                {
                    item.LifecycleStage = this.EncodingLifecycleStageId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ModelAnnotations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
