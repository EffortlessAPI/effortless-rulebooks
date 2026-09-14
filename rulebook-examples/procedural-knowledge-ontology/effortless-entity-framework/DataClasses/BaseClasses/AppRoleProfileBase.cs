
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
    [Table("AppRoleProfiles")]
    public class AppRoleProfileBase : SoAEntityBase
    {
        [Key]
        public string AppRoleProfileId { get; set; }

        // Formula Name (rulebook: ={{DisplayLabel}} & " (" & {{RoleKind}} & ")")
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.DisplayLabel)), F.S(" ("), F.Text(F.Of(this.RoleKind)), F.S(")")))); set { }
        }

        public string? DisplayLabel { get; set; }
        public string? RoleKind { get; set; }
        public string? AccentColor { get; set; }
        public string? IconMark { get; set; }
        public string? IconPngBase64 { get; set; }
        public string? Pitch { get; set; }
        public decimal? SortOrder { get; set; }
        // Formula RouteCount (rulebook: =COUNTIFS(AppRoutes!{{OwningRole}}, {{Role}}))
        [NotMapped]
        public decimal? RouteCount
        {
            get => F.AsDecimal(F.Memo(this, "RouteCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AppRoute>(base.SoAContext, "AppRoutes", __c => __c.AppRoutes), __r => F.CritField(F.Of(__r.OwningRole), F.Of(this.Role)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Role { get; set; }

        private Role _roleRef;

        [ForeignKey("Role")]
        public virtual Role RoleRef
        {
            get
            {
                if (_roleRef == null && !string.IsNullOrEmpty(Role))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleRef - no database context is set. Role: " + Role + ".");
                        }
                        return null;
                    }
                    _roleRef = base.SoAContext.Roles.Find(Role);
                    if (_roleRef != null)
                    {
                        base.SoAContext.Attach(_roleRef);
                    }
                }
                return _roleRef;
            }
            set
            {
                if (_roleRef != value)
                {
                    _roleRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleRef != null)
                    {
                        Role = _roleRef.RoleId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RoleRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
