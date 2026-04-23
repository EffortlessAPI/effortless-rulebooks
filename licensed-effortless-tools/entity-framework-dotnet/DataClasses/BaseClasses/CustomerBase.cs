
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Customers")]
    public class CustomerBase : SoAEntityBase
    {
        [Key]
        public string CustomerId { get; set; }

        public string? Customer { get; set; }
        public string? EmailAddress { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        // Formula FullName (rulebook: ={{LastName}} & ", " & {{FirstName}})
        public string? FullName
        {
            get => this.LastName + ", " + this.FirstName; set { }
        }




        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.CustomerId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
