
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AppNavGroups")]
    public class AppNavGroupBase : SoAEntityBase
    {
        [Key]
        public string AppNavGroupId { get; set; }

        // Formula Name (rulebook: ={{GroupLabel}})
        public string? Name
        {
            get => this.GroupLabel; set { }
        }

        public string? GroupLabel { get; set; }
        // Formula RouteCount (rulebook: =COUNTIFS(AppRoutes!{{NavGroup}}, {{AppNavGroupId}}))
        public decimal? RouteCount
        {
            get => COUNTIFS(AppRoutes!this.NavGroup, this.AppNavGroupId); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<AppRoute> _appRoutes;

        [InverseProperty("AppNavGroup")]
        public virtual ObservableCollection<AppRoute> AppRoutes
        {
            get
            {
                if (_appRoutes == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRoutes - no database context is set. AppNavGroupId: " + this.AppNavGroupId + ".");
                        }
                        _appRoutes = new ObservableCollection<AppRoute>();
                    }
                    else
                    {
                        var items = Context.AppRoutes.Where(x => x.NavGroup == this.AppNavGroupId).ToList<AppRoute>();
                        _appRoutes = new ObservableCollection<AppRoute>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _appRoutes.CollectionChanged += AppRoutes_CollectionChanged;
                }
                return _appRoutes;
            }
            private set
            {
                if (_appRoutes != null)
                {
                    _appRoutes.CollectionChanged -= AppRoutes_CollectionChanged;
                }
                _appRoutes = value;
                if (_appRoutes != null)
                {
                    _appRoutes.CollectionChanged += AppRoutes_CollectionChanged;
                }
            }
        }

        private void AppRoutes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppRoute>())
                {
                    item.NavGroup = this.AppNavGroupId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AppRoutes;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
