
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("UserRoles")]
    public class UserRoleBase : SoAEntityBase
    {
        [Key]
        public string RoleId { get; set; }

        public string Name { get; set; }
        public string AccessLevel { get; set; }
        public bool CanEditRulebook { get; set; }
        public bool CanRunBuilds { get; set; }
        public bool CanAccessTechTools { get; set; }
        public bool CanSwitchProjects { get; set; }
        public bool CanManageUsers { get; set; }
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
