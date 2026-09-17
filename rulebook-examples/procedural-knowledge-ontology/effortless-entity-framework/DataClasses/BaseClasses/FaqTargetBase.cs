
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
    [Table("FaqTargets")]
    public class FaqTargetBase : SoAEntityBase
    {
        [Key]
        public string FaqTargetId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        // Formula FaqCount (rulebook: =COUNTIFS(FAQs!{{TargetKind}}, {{FaqTargetId}}))
        [NotMapped]
        public int? FaqCount
        {
            get => F.AsInt(F.Memo(this, "FaqCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<FAQ>(base.SoAContext, "FAQs", __c => __c.FAQs), __r => F.CritField(F.Of(__r.TargetKind), F.Of(this.FaqTargetId))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<FAQ> _fAQs;

        [InverseProperty("FaqTarget")]
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
                            throw new InvalidOperationException("Cannot access FAQs - no database context is set. FaqTargetId: " + this.FaqTargetId + ".");
                        }
                        _fAQs = new ObservableCollection<FAQ>();
                    }
                    else
                    {
                        var items = base.SoAContext.FAQs.Where(x => x.TargetKind == this.FaqTargetId).ToList<FAQ>();
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
                    item.TargetKind = this.FaqTargetId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.FAQs;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
