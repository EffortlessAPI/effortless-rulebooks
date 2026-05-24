
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("CoreDataFlows")]
    public class CoreDataFlowBase : SoAEntityBase
    {
        [Key]
        public string FlowId { get; set; }

        public string Name { get; set; }
        public string Steps { get; set; }
        public string? Triggers { get; set; }
        public string? Outputs { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
