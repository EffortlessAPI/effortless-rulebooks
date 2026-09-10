
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
    [Table("VocabularyReconciliations")]
    public class VocabularyReconciliationBase : SoAEntityBase
    {
        [Key]
        public string ReconciliationId { get; set; }

        // Formula RelativePath (rulebook: ="reconciliations/" & {{ReconciliationId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("reconciliations/"), F.TextOr(F.Of(this.ReconciliationId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        // Formula Name (rulebook: ={{DeprecatedTerm}} & " owl:sameAs " & {{ReplacementTerm}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.DeprecatedTerm)), F.S(" owl:sameAs "), F.TextOr(F.Of(this.ReplacementTerm))))); set { }
        }

        public string? DeprecatedTerm { get; set; }
        public string? ReplacementTerm { get; set; }
        public string? ReconciliationRelation { get; set; }
        public string? SourceStandard { get; set; }
        public string? IntroducedInVersion { get; set; }
        public string? Rationale { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
