
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
    [Table("ClassificationFacets")]
    public class ClassificationFacetBase : SoAEntityBase
    {
        [Key]
        public string ClassificationFacetId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? DimensionDescription { get; set; }
        // Formula AssignmentCount (rulebook: =COUNTIFS(ProcedureFacetAssignments!{{Facet}}, {{ClassificationFacetId}}))
        [NotMapped]
        public int? AssignmentCount
        {
            get => F.AsInt(F.Memo(this, "AssignmentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureFacetAssignment>(base.SoAContext, "ProcedureFacetAssignments", __c => __c.ProcedureFacetAssignments), __r => F.CritField(F.Of(__r.Facet), F.Of(this.ClassificationFacetId))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<ProcedureType> _procedureTypes;

        [InverseProperty("ClassificationFacet")]
        public virtual ObservableCollection<ProcedureType> ProcedureTypes
        {
            get
            {
                if (_procedureTypes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureTypes - no database context is set. ClassificationFacetId: " + this.ClassificationFacetId + ".");
                        }
                        _procedureTypes = new ObservableCollection<ProcedureType>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureTypes.Where(x => x.DistinguishingFacet == this.ClassificationFacetId).ToList<ProcedureType>();
                        _procedureTypes = new ObservableCollection<ProcedureType>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureTypes.CollectionChanged += ProcedureTypes_CollectionChanged;
                }
                return _procedureTypes;
            }
            private set
            {
                if (_procedureTypes != null)
                {
                    _procedureTypes.CollectionChanged -= ProcedureTypes_CollectionChanged;
                }
                _procedureTypes = value;
                if (_procedureTypes != null)
                {
                    _procedureTypes.CollectionChanged += ProcedureTypes_CollectionChanged;
                }
            }
        }

        private void ProcedureTypes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureType>())
                {
                    item.DistinguishingFacet = this.ClassificationFacetId;
                }
            }
        }

        private ObservableCollection<ProcedureFacetAssignment> _procedureFacetAssignments;

        [InverseProperty("ClassificationFacet")]
        public virtual ObservableCollection<ProcedureFacetAssignment> ProcedureFacetAssignments
        {
            get
            {
                if (_procedureFacetAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureFacetAssignments - no database context is set. ClassificationFacetId: " + this.ClassificationFacetId + ".");
                        }
                        _procedureFacetAssignments = new ObservableCollection<ProcedureFacetAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureFacetAssignments.Where(x => x.Facet == this.ClassificationFacetId).ToList<ProcedureFacetAssignment>();
                        _procedureFacetAssignments = new ObservableCollection<ProcedureFacetAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureFacetAssignments.CollectionChanged += ProcedureFacetAssignments_CollectionChanged;
                }
                return _procedureFacetAssignments;
            }
            private set
            {
                if (_procedureFacetAssignments != null)
                {
                    _procedureFacetAssignments.CollectionChanged -= ProcedureFacetAssignments_CollectionChanged;
                }
                _procedureFacetAssignments = value;
                if (_procedureFacetAssignments != null)
                {
                    _procedureFacetAssignments.CollectionChanged += ProcedureFacetAssignments_CollectionChanged;
                }
            }
        }

        private void ProcedureFacetAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureFacetAssignment>())
                {
                    item.Facet = this.ClassificationFacetId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureTypes;
            _ = this.ProcedureFacetAssignments;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
