
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("RulebookReleases")]
    public class RulebookReleaseBase : SoAEntityBase
    {
        [Key]
        public string RulebookReleaseId { get; set; }

        // Formula Name (rulebook: ={{RulebookVersion}} & " / PKO " & {{PkoCoreVersionIri}})
        public string? Name
        {
            get => this.RulebookVersion + " / PKO " + this.PkoCoreVersionIri; set { }
        }

        public string? RulebookVersion { get; set; }
        public string? ProfileVersion { get; set; }
        public string? ProfileSchemaPath { get; set; }
        public string? PkoCoreVersionIri { get; set; }
        public string? PkoIndustryVersionIri { get; set; }
        public DateTime? IssuedAt { get; set; }
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
