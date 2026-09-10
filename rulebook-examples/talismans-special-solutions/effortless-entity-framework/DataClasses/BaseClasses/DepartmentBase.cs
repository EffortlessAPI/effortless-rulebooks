
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
    [Table("Departments")]
    public class DepartmentBase : SoAEntityBase
    {
        [Key]
        public string DepartmentId { get; set; }

        // Formula RelativePath (rulebook: ="departments/" & {{DepartmentId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("departments/"), F.TextOr(F.Of(this.DepartmentId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        // Formula Name (rulebook: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-"))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Substitute(F.Lower(F.Of(this.DisplayName)), F.S(" "), F.S("-")))); set { }
        }

        public string? Title { get; set; }
        public string? DisplayName { get; set; }

        public string? Roles { get; set; }

        private Role _role;

        [ForeignKey("Roles")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(Roles))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. Roles: " + Roles + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(Roles);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        Roles = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<Role> _ownedByRoles;

        [InverseProperty("Department")]
        public virtual ObservableCollection<Role> OwnedByRoles
        {
            get
            {
                if (_ownedByRoles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OwnedByRoles - no database context is set. DepartmentId: " + this.DepartmentId + ".");
                        }
                        _ownedByRoles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.OwnedBy == this.DepartmentId).ToList<Role>();
                        _ownedByRoles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _ownedByRoles.CollectionChanged += OwnedByRoles_CollectionChanged;
                }
                return _ownedByRoles;
            }
            private set
            {
                if (_ownedByRoles != null)
                {
                    _ownedByRoles.CollectionChanged -= OwnedByRoles_CollectionChanged;
                }
                _ownedByRoles = value;
                if (_ownedByRoles != null)
                {
                    _ownedByRoles.CollectionChanged += OwnedByRoles_CollectionChanged;
                }
            }
        }

        private void OwnedByRoles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.OwnedBy = this.DepartmentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.OwnedByRoles;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
