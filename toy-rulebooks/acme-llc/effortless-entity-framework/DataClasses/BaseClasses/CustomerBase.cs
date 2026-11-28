
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;
using F = SqlOnAir.DotNet.Lib.DataClasses.Formulas.EfFormulaFns;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Customers")]
    public class CustomerBase : SoAEntityBase
    {
        [Key]
        public string CustomerId { get; set; }

        // Formula Name (rulebook: =SUBSTITUTE({{EmailAddress}}, "@", "-"))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Substitute(F.Of(this.EmailAddress), F.S("@"), F.S("-")))); set { }
        }

        public string? EmailAddress { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        // Formula FullName (rulebook: ={{FirstName}} & " " & {{LastName}})
        [NotMapped]
        public string? FullName
        {
            get => F.AsString(F.Memo(this, "FullName", () => F.Concat(F.Text(F.Of(this.FirstName)), F.S(" "), F.Text(F.Of(this.LastName))))); set { }
        }




        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
