
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AppPermissions")]
    public class AppPermissionBase : SoAEntityBase
    {
        [Key]
        public string PermissionId { get; set; }

        // Formula RoleId
        public string RoleId
        {
            get => default /* TODO: translate formula:  */; set { }
        }

        public string Resource { get; set; }
        public string Action { get; set; }
        public bool Allow { get; set; }
        public string? RlsPredicate { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.PermissionId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
