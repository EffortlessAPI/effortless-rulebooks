
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("RoleSchemas")]
    public class RoleSchemaBase : SoAEntityBase
    {
        [Key]
        public string RoleSchemaId { get; set; }

        // Formula Name (rulebook: ={{SchemaName}})
        public string? Name
        {
            get => this.SchemaName; set { }
        }

        public string? SchemaName { get; set; }
        // Formula SearchPath (rulebook: ={{SchemaName}})
        public string? SearchPath
        {
            get => this.SchemaName; set { }
        }

        public bool? IsSealed { get; set; }
        // Formula ViewCount (rulebook: =COUNTIFS(RoleSchemaViews!{{RoleSchema}}, {{RoleSchemaId}}))
        public decimal? ViewCount
        {
            get => COUNTIFS(RoleSchemaViews!this.RoleSchema, this.RoleSchemaId); set { }
        }

        // Formula IsEmptySchema (rulebook: ={{ViewCount}} = 0)
        public bool? IsEmptySchema
        {
            get => this.ViewCount = 0; set { }
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPrincipal - no database context is set. Principal: " + Principal + ".");
                        }
                        return null;
                    }
                    _accessPrincipal = Context.AccessPrincipals.Find(Principal);
                    if (_accessPrincipal != null)
                    {
                        Context.Attach(_accessPrincipal);
                    }
                }
                return _accessPrincipal;
            }
            set
            {
                if (_accessPrincipal != value)
                {
                    _accessPrincipal = value;
                    Principal = _accessPrincipal == null ? default : _accessPrincipal.AccessPrincipalId;
                }
            }
        }

        private ObservableCollection<RoleSchemaView> _roleSchemaViews;

        [InverseProperty("RoleSchema")]
        public virtual ObservableCollection<RoleSchemaView> RoleSchemaViews
        {
            get
            {
                if (_roleSchemaViews == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleSchemaViews - no database context is set. RoleSchemaId: " + this.RoleSchemaId + ".");
                        }
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>();
                    }
                    else
                    {
                        var items = Context.RoleSchemaViews.Where(x => x.RoleSchema == this.RoleSchemaId).ToList<RoleSchemaView>();
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
