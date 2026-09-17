
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
    [Table("SemanticMappings")]
    public class SemanticMappingBase : SoAEntityBase
    {
        [Key]
        public string SemanticMappingId { get; set; }

        // Formula Name (rulebook: ={{SourcePath}} & " -> " & {{TargetIri}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.SourcePath)), F.S(" -> "), F.Text(F.Of(this.TargetIri))))); set { }
        }

        public string? SourcePath { get; set; }
        public string? MappingKind { get; set; }
        public string? TargetIri { get; set; }
        public string? MappingRelation { get; set; }
        public string? Notes { get; set; }
        public string? AvailableStandardIri { get; set; }
        // Formula ReinventsStandardTerm (rulebook: =AND({{MappingRelation}} = "extension", {{AvailableStandardIri}} <> ""))
        [NotMapped]
        public bool? ReinventsStandardTerm
        {
            get => F.AsBool(F.Memo(this, "ReinventsStandardTerm", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.MappingRelation)), F.S("extension"))), F.Bool3(F.IsNotBlank(F.Of(this.AvailableStandardIri)))))); set { }
        }

        // Formula IsNonResolvableTermIri (rulebook: =AND({{MappingRelation}} = "extension", LEFT({{TargetIri}}, 4) <> "http"))
        [NotMapped]
        public bool? IsNonResolvableTermIri
        {
            get => F.AsBool(F.Memo(this, "IsNonResolvableTermIri", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.MappingRelation)), F.S("extension"))), F.Bool3(F.Ne(F.Left(F.Of(this.TargetIri), F.I(4)), F.S("http")))))); set { }
        }


        public string? OntologyProfile { get; set; }

        private OntologyProfile _ontologyProfileRef;

        [ForeignKey("OntologyProfile")]
        public virtual OntologyProfile OntologyProfileRef
        {
            get
            {
                if (_ontologyProfileRef == null && !string.IsNullOrEmpty(OntologyProfile))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OntologyProfileRef - no database context is set. OntologyProfile: " + OntologyProfile + ".");
                        }
                        return null;
                    }
                    _ontologyProfileRef = base.SoAContext.OntologyProfiles.Find(OntologyProfile);
                    if (_ontologyProfileRef != null)
                    {
                        base.SoAContext.Attach(_ontologyProfileRef);
                    }
                }
                return _ontologyProfileRef;
            }
            set
            {
                if (_ontologyProfileRef != value)
                {
                    _ontologyProfileRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_ontologyProfileRef != null)
                    {
                        OntologyProfile = _ontologyProfileRef.OntologyProfileId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OntologyProfileRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
