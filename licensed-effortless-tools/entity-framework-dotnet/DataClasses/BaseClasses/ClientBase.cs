
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Client")]
    public class ClientBase : SoAEntityBase
    {
        [Key]
        public string ClientId { get; set; }

        public string? Customer { get; set; }
        public string? EmailAddress { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        // Formula FullName (rulebook: ={{FirstName}} & " " & {{LastName}})
        public string? FullName
        {
            get => this.FirstName + " " + this.LastName; set { }
        }




        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.ClientId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
