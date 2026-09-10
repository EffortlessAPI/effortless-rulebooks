
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AppRoleProfiles")]
    public class AppRoleProfileBase : SoAEntityBase
    {
        [Key]
        public string AppRoleProfileId { get; set; }

        // Formula Name (rulebook: ={{DisplayLabel}} & " (" & {{RoleKind}} & ")")
        public string? Name
        {
            get => this.DisplayLabel + " (" + this.RoleKind + ")"; set { }
        }

        public string? DisplayLabel { get; set; }
        public string? RoleKind { get; set; }
        public string? AccentColor { get; set; }
        public string? IconMark { get; set; }
        public string? IconPngBase64 { get; set; }
        public string? Pitch { get; set; }
        public decimal? SortOrder { get; set; }
        // Formula RouteCount (rulebook: =COUNTIFS(AppRoutes!{{OwningRole}}, {{Role}}))
        public decimal? RouteCount
        {
            get => COUNTIFS(AppRoutes!this.OwningRole, this.Role); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Role { get; set; }

        private Role _role;

        [ForeignKey("Role")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(Role))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. Role: " + Role + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(Role);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    Role = _role == null ? default : _role.RoleId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
