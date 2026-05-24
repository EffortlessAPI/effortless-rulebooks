
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("BuildPipeline")]
    public class BuildPipelineBase : SoAEntityBase
    {
        [Key]
        public string AspectId { get; set; }

        public string Aspect { get; set; }
        public string? PortalLocation { get; set; }
        public string? CliEquivalent { get; set; }
        public string Authority { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.AspectId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
