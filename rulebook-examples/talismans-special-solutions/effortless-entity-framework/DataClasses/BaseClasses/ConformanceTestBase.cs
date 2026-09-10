
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
    [Table("ConformanceTests")]
    public class ConformanceTestBase : SoAEntityBase
    {
        [Key]
        public string ConformanceTestId { get; set; }

        // Formula RelativePath (rulebook: ="conformance-tests/" & {{ConformanceTestId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("conformance-tests/"), F.TextOr(F.Of(this.ConformanceTestId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        // Formula Name (rulebook: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-"))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Substitute(F.Lower(F.Of(this.DisplayName)), F.S(" "), F.S("-")))); set { }
        }

        public string DisplayName { get; set; }
        public string? FeatureRef { get; set; }
        public string Section { get; set; }
        public string TestKind { get; set; }
        public string? TargetRef { get; set; }
        public string? Expect { get; set; }
        public string? Explanation { get; set; }
        public int SortOrder { get; set; }
        public bool IsEnabled { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
