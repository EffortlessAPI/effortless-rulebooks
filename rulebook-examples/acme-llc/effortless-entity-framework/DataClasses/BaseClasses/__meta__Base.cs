
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("__meta__")]
    public class __meta__Base : SoAEntityBase
    {
        public string MetaKey { get; set; }
        // Formula Name (rulebook: ={{MetaKey}})
        public string Name
        {
            get => this.MetaKey; set { }
        }

        public string ValueType { get; set; }
        public string? StringValue { get; set; }
        public string? JsonValue { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
