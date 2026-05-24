
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ProjectMetadata")]
    public class ProjectMetadataBase : SoAEntityBase
    {
        [Key]
        public string ProjectId { get; set; }

        public string Name { get; set; }
        public string Purpose { get; set; }
        public string? Architecture { get; set; }
        public string? RepositoryRoot { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
