
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AddToolCatalog")]
    public class AddToolCatalogBase : SoAEntityBase
    {
        [Key]
        public string ToolId { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }
        public string Source { get; set; }
        public string InstallUrl { get; set; }
        public string? OutputPath { get; set; }
        // Formula SubstrateId
        public string? SubstrateId
        {
            get => default /* TODO: translate formula:  */; set { }
        }

        public string Description { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
