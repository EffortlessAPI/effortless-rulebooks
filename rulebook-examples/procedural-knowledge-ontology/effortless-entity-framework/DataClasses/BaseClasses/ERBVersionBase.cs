
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ERBVersions")]
    public class ERBVersionBase : SoAEntityBase
    {
        [Key]
        public string ERBVersionId { get; set; }

        public string? BaseId { get; set; }
        public string? Name { get; set; }
        public string? Message { get; set; }
        public string? Notes { get; set; }
        public DateTime? CommitDate { get; set; }
        public bool? IsPublished { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
