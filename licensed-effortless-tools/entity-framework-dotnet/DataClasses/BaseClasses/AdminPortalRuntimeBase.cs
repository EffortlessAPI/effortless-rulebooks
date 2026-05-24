
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AdminPortalRuntime")]
    public class AdminPortalRuntimeBase : SoAEntityBase
    {
        [Key]
        public string ProcessId { get; set; }

        public string Name { get; set; }
        public string Command { get; set; }
        public decimal? Port { get; set; }
        public string? DependsOn { get; set; }
        public bool AutoRestart { get; set; }
        public string Purpose { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
