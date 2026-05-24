
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AppUsers")]
    public class AppUserBase : SoAEntityBase
    {
        [Key]
        public string UserId { get; set; }

        public string Email { get; set; }
        public string DisplayName { get; set; }
        // Formula RoleId
        public string RoleId
        {
            get => default /* TODO: translate formula:  */; set { }
        }

        public bool IsDefault { get; set; }
        public string? Notes { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.DisplayName?.ToString() ?? base.ToString() ?? "";
        }
    }
}
