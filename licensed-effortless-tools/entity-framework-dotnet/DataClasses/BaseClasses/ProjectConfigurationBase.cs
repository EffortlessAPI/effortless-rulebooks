
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ProjectConfiguration")]
    public class ProjectConfigurationBase : SoAEntityBase
    {
        [Key]
        public string ConfigId { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Format { get; set; }
        public string Purpose { get; set; }
        public string? MaintainedBy { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.ConfigId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
