
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
    [Table("RoleSchemas")]
    public class RoleSchemaBase : SoAEntityBase
    {
        [Key]
        public string RoleSchemaId { get; set; }

        // Formula Name (rulebook: ={{SchemaName}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.SchemaName))); set { }
        }

        public string? SchemaName { get; set; }
        // Formula SearchPath (rulebook: ={{SchemaName}})
        [NotMapped]
        public string? SearchPath
        {
            get => F.AsString(F.Memo(this, "SearchPath", () => F.Of(this.SchemaName))); set { }
        }

        public bool? IsSealed { get; set; }
        // Formula ViewCount (rulebook: =COUNTIFS(RoleSchemaViews!{{RoleSchema}}, {{RoleSchemaId}}))
        [NotMapped]
        public decimal? ViewCount
        {
            get => F.AsDecimal(F.Memo(this, "ViewCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleSchemaView>(base.SoAContext, "RoleSchemaViews", __c => __c.RoleSchemaViews), __r => F.CritField(F.Of(__r.RoleSchema), F.Of(this.RoleSchemaId)))))); set { }
        }

        // Formula IsEmptySchema (rulebook: ={{ViewCount}} = 0)
        [NotMapped]
        public bool? IsEmptySchema
        {
            get => F.AsBool(F.Memo(this, "IsEmptySchema", () => F.Eq(F.Of(this.ViewCount), F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Principal { get; set; }

        private AccessPrincipal _accessPrincipal;

        [ForeignKey("Principal")]
        public virtual AccessPrincipal AccessPrincipal
        {
            get
            {
                if (_accessPrincipal == null && !string.IsNullOrEmpty(Principal))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPrincipal - no database context is set. Principal: " + Principal + ".");
                        }
                        return null;
                    }
                    _accessPrincipal = base.SoAContext.AccessPrincipals.Find(Principal);
                    if (_accessPrincipal != null)
                    {
                        base.SoAContext.Attach(_accessPrincipal);
                    }
                }
                return _accessPrincipal;
            }
            set
            {
                if (_accessPrincipal != value)
                {
                    _accessPrincipal = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_accessPrincipal != null)
                    {
                        Principal = _accessPrincipal.AccessPrincipalId;
                    }
                }
            }
        }

        private ObservableCollection<RoleSchemaView> _roleSchemaViews;

        [InverseProperty("RoleSchemaRef")]
        public virtual ObservableCollection<RoleSchemaView> RoleSchemaViews
        {
            get
            {
                if (_roleSchemaViews == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleSchemaViews - no database context is set. RoleSchemaId: " + this.RoleSchemaId + ".");
                        }
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleSchemaViews.Where(x => x.RoleSchema == this.RoleSchemaId).ToList<RoleSchemaView>();
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roleSchemaViews.CollectionChanged += RoleSchemaViews_CollectionChanged;
                }
                return _roleSchemaViews;
            }
            private set
            {
                if (_roleSchemaViews != null)
                {
                    _roleSchemaViews.CollectionChanged -= RoleSchemaViews_CollectionChanged;
                }
                _roleSchemaViews = value;
                if (_roleSchemaViews != null)
                {
                    _roleSchemaViews.CollectionChanged += RoleSchemaViews_CollectionChanged;
                }
            }
        }

        private void RoleSchemaViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleSchemaView>())
                {
                    item.RoleSchema = this.RoleSchemaId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AccessPrincipal;
            _ = this.RoleSchemaViews;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
