
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("RulebookDomains")]
    public class RulebookDomainBase : SoAEntityBase
    {
        [Key]
        public string DomainId { get; set; }

        public string DomainName { get; set; }
        public string RelativePath { get; set; }
        public string RulebookPath { get; set; }
        public string? ComplexityLevel { get; set; }
        public decimal? TableCount { get; set; }
        public string? KeyFeatures { get; set; }
        public string? Purpose { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.DomainId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
