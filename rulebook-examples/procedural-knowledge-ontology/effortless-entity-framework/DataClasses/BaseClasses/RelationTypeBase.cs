
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
    [Table("RelationTypes")]
    public class RelationTypeBase : SoAEntityBase
    {
        [Key]
        public string RelationTypeId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Family { get; set; }
        public string? InverseLabel { get; set; }
        public bool? IsDefined { get; set; }
        public string? RealizedBy { get; set; }
        // Formula UsageCount (rulebook: =COUNTIFS(ActivityRelations!{{RelationType}}, {{RelationTypeId}}))
        [NotMapped]
        public int? UsageCount
        {
            get => F.AsInt(F.Memo(this, "UsageCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ActivityRelation>(base.SoAContext, "ActivityRelations", __c => __c.ActivityRelations), __r => F.CritField(F.Of(__r.RelationType), F.Of(this.RelationTypeId))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<ActivityRelation> _activityRelations;

        [InverseProperty("RelationTypeRef")]
        public virtual ObservableCollection<ActivityRelation> ActivityRelations
        {
            get
            {
                if (_activityRelations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ActivityRelations - no database context is set. RelationTypeId: " + this.RelationTypeId + ".");
                        }
                        _activityRelations = new ObservableCollection<ActivityRelation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ActivityRelations.Where(x => x.RelationType == this.RelationTypeId).ToList<ActivityRelation>();
                        _activityRelations = new ObservableCollection<ActivityRelation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _activityRelations.CollectionChanged += ActivityRelations_CollectionChanged;
                }
                return _activityRelations;
            }
            private set
            {
                if (_activityRelations != null)
                {
                    _activityRelations.CollectionChanged -= ActivityRelations_CollectionChanged;
                }
                _activityRelations = value;
                if (_activityRelations != null)
                {
                    _activityRelations.CollectionChanged += ActivityRelations_CollectionChanged;
                }
            }
        }

        private void ActivityRelations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ActivityRelation>())
                {
                    item.RelationType = this.RelationTypeId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ActivityRelations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
