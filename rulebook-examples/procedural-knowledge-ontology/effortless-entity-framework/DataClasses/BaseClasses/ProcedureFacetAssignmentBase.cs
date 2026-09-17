
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
    [Table("ProcedureFacetAssignments")]
    public class ProcedureFacetAssignmentBase : SoAEntityBase
    {
        [Key]
        public string ProcedureFacetAssignmentId { get; set; }

        // Formula Name (rulebook: ={{Procedure}} & " " & {{Facet}} & "=" & {{FacetValue}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Procedure)), F.S(" "), F.Text(F.Of(this.Facet)), F.S("="), F.Text(F.Of(this.FacetValue))))); set { }
        }

        public string? FacetValue { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
        public string? Facet { get; set; }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }

        private ClassificationFacet _classificationFacet;

        [ForeignKey("Facet")]
        public virtual ClassificationFacet ClassificationFacet
        {
            get
            {
                if (_classificationFacet == null && !string.IsNullOrEmpty(Facet))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ClassificationFacet - no database context is set. Facet: " + Facet + ".");
                        }
                        return null;
                    }
                    _classificationFacet = base.SoAContext.ClassificationFacets.Find(Facet);
                    if (_classificationFacet != null)
                    {
                        base.SoAContext.Attach(_classificationFacet);
                    }
                }
                return _classificationFacet;
            }
            set
            {
                if (_classificationFacet != value)
                {
                    _classificationFacet = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_classificationFacet != null)
                    {
                        Facet = _classificationFacet.ClassificationFacetId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.ClassificationFacet;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
