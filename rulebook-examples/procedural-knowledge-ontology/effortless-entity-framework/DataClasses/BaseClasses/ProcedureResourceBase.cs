
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
    [Table("ProcedureResources")]
    public class ProcedureResourceBase : SoAEntityBase
    {
        [Key]
        public string ProcedureResourceId { get; set; }

        // Formula Name (rulebook: ={{ProcedureVersion}} & " / " & {{Resource}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.ProcedureVersion)), F.S(" / "), F.TextOr(F.Of(this.Resource))))); set { }
        }

        public string? Relation { get; set; }
        // Formula RelationIri (rulebook: =IF({{Relation}} = "wasExtractedFrom", "https://w3id.org/pko#wasExtractedFrom", "http://purl.org/dc/terms/references"))
        [NotMapped]
        public string? RelationIri
        {
            get => F.AsString(F.Memo(this, "RelationIri", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Relation)), F.S("wasExtractedFrom")))) ? F.S("https://w3id.org/pko#wasExtractedFrom") : F.S("http://purl.org/dc/terms/references")))); set { }
        }


        public string? ProcedureVersion { get; set; }
        public string? Resource { get; set; }

        private ProcedureVersion _procedureVersionRef;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersionRef
        {
            get
            {
                if (_procedureVersionRef == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionRef - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersionRef = base.SoAContext.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersionRef != null)
                    {
                        base.SoAContext.Attach(_procedureVersionRef);
                    }
                }
                return _procedureVersionRef;
            }
            set
            {
                if (_procedureVersionRef != value)
                {
                    _procedureVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersionRef != null)
                    {
                        ProcedureVersion = _procedureVersionRef.ProcedureVersionId;
                    }
                }
            }
        }

        private Resource _resourceRef;

        [ForeignKey("Resource")]
        public virtual Resource ResourceRef
        {
            get
            {
                if (_resourceRef == null && !string.IsNullOrEmpty(Resource))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ResourceRef - no database context is set. Resource: " + Resource + ".");
                        }
                        return null;
                    }
                    _resourceRef = base.SoAContext.Resources.Find(Resource);
                    if (_resourceRef != null)
                    {
                        base.SoAContext.Attach(_resourceRef);
                    }
                }
                return _resourceRef;
            }
            set
            {
                if (_resourceRef != value)
                {
                    _resourceRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_resourceRef != null)
                    {
                        Resource = _resourceRef.ResourceId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.ResourceRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
