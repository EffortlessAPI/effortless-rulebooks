
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("RulebookSourceSpokes")]
    public class RulebookSourceSpokeBase : SoAEntityBase
    {
        [Key]
        public string SpokeId { get; set; }

        public string Name { get; set; }
        public string Kind { get; set; }
        public string Direction { get; set; }
        public bool Required { get; set; }
        public string Purpose { get; set; }
        public string? Authority { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
