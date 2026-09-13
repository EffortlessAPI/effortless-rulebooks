
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
    [Table("RulebookReleases")]
    public class RulebookReleaseBase : SoAEntityBase
    {
        [Key]
        public string RulebookReleaseId { get; set; }

        // Formula Name (rulebook: ={{RulebookVersion}} & " / PKO " & {{PkoCoreVersionIri}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.RulebookVersion)), F.S(" / PKO "), F.TextOr(F.Of(this.PkoCoreVersionIri))))); set { }
        }

        public string? RulebookVersion { get; set; }
        public string? ProfileVersion { get; set; }
        public string? ProfileSchemaPath { get; set; }
        public string? PkoCoreVersionIri { get; set; }
        public string? PkoIndustryVersionIri { get; set; }
        public DateTimeOffset? IssuedAt { get; set; }
        public string? Status { get; set; }
        public string? Changelog { get; set; }
        public bool? IsCurrent { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
