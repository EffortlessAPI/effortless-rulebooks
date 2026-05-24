
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ERBCustomizations")]
    public class ERBCustomizationBase : SoAEntityBase
    {
        [Key]
        public string ERBCustomizationId { get; set; }

        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? SQLCode { get; set; }
        public string? SQLTarget { get; set; }
        public string? CustomizationType { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
