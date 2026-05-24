
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ExecutionSubstrates")]
    public class ExecutionSubstrateBase : SoAEntityBase
    {
        [Key]
        public string SubstrateId { get; set; }

        public string Name { get; set; }
        public string Technology { get; set; }
        public string RelativePath { get; set; }
        public string InjectorScript { get; set; }
        public string? TestScript { get; set; }
        public bool IsProduction { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
