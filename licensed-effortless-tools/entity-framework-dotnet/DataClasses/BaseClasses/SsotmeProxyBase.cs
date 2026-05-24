
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("SsotmeProxy")]
    public class SsotmeProxyBase : SoAEntityBase
    {
        [Key]
        public string RouteId { get; set; }

        public string Route { get; set; }
        public string? SubstrateId { get; set; }
        public string? InjectorScript { get; set; }
        public string Description { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.RouteId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
