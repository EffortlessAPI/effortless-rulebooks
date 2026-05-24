
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Dependencies")]
    public class DependencyBase : SoAEntityBase
    {
        [Key]
        public string DependencyId { get; set; }

        public string Name { get; set; }
        public string? Version { get; set; }
        public string Type { get; set; }
        public string Purpose { get; set; }
        public bool Required { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
