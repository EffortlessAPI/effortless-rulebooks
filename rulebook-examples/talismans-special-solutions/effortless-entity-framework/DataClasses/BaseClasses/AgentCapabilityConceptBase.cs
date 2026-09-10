
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
    [Table("AgentCapabilityConcepts")]
    public class AgentCapabilityConceptBase : SoAEntityBase
    {
        [Key]
        public string ConceptId { get; set; }

        // Formula RelativePath (rulebook: ="concepts/agent-capability/" & {{ConceptId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("concepts/agent-capability/"), F.TextOr(F.Of(this.ConceptId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        public string PrefLabel { get; set; }
        public string? AltLabel { get; set; }
        public string? Definition { get; set; }
        public string? ScopeNote { get; set; }

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

        private ObservableCollection<Role> _hasCapabilityRoles;

        [InverseProperty("AgentCapabilityConcept")]
        public virtual ObservableCollection<Role> HasCapabilityRoles
        {
            get
            {
                if (_hasCapabilityRoles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access HasCapabilityRoles - no database context is set. ConceptId: " + this.ConceptId + ".");
                        }
                        _hasCapabilityRoles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.HasCapability == this.ConceptId).ToList<Role>();
                        _hasCapabilityRoles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _hasCapabilityRoles.CollectionChanged += HasCapabilityRoles_CollectionChanged;
                }
                return _hasCapabilityRoles;
            }
            private set
            {
                if (_hasCapabilityRoles != null)
                {
                    _hasCapabilityRoles.CollectionChanged -= HasCapabilityRoles_CollectionChanged;
                }
                _hasCapabilityRoles = value;
                if (_hasCapabilityRoles != null)
                {
                    _hasCapabilityRoles.CollectionChanged += HasCapabilityRoles_CollectionChanged;
                }
            }
        }

        private void HasCapabilityRoles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.HasCapability = this.ConceptId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.HasCapabilityRoles;
        }

        public override string ToString()
        {
            return this.ConceptId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
