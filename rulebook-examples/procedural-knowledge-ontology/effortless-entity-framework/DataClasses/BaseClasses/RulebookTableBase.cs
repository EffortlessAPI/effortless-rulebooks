
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("RulebookTables")]
    public class RulebookTableBase : SoAEntityBase
    {
        [Key]
        public string RulebookTableId { get; set; }

        public string TableName { get; set; }
        // Formula Name (rulebook: ={{TableName}})
        public string? Name
        {
            get => this.TableName; set { }
        }

        public string? PhysicalTable { get; set; }
        public string? PhysicalView { get; set; }
        public string? SubjectArea { get; set; }
        public bool? IsExtension { get; set; }
        // Formula FieldCount (rulebook: =COUNTIFS(RulebookFields!{{TargetTable}}, {{RulebookTableId}}))
        public decimal? FieldCount
        {
            get => COUNTIFS(RulebookFields!this.TargetTable, this.RulebookTableId); set { }
        }

        // Formula PolicyCount (rulebook: =COUNTIFS(AccessPolicies!{{TargetTable}}, {{RulebookTableId}}))
        public decimal? PolicyCount
        {
            get => COUNTIFS(AccessPolicies!this.TargetTable, this.RulebookTableId); set { }
        }

        // Formula IsUnsecured (rulebook: ={{PolicyCount}} = 0)
        public bool? IsUnsecured
        {
            get => this.PolicyCount = 0; set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<AccessPolicy> _accessPolicies;

        [InverseProperty("RulebookTable")]
        public virtual ObservableCollection<AccessPolicy> AccessPolicies
        {
            get
            {
                if (_accessPolicies == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPolicies - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _accessPolicies = new ObservableCollection<AccessPolicy>();
                    }
                    else
                    {
                        var items = Context.AccessPolicies.Where(x => x.TargetTable == this.RulebookTableId).ToList<AccessPolicy>();
                        _accessPolicies = new ObservableCollection<AccessPolicy>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _accessPolicies.CollectionChanged += AccessPolicies_CollectionChanged;
                }
                return _accessPolicies;
            }
            private set
            {
                if (_accessPolicies != null)
                {
                    _accessPolicies.CollectionChanged -= AccessPolicies_CollectionChanged;
                }
                _accessPolicies = value;
                if (_accessPolicies != null)
                {
                    _accessPolicies.CollectionChanged += AccessPolicies_CollectionChanged;
                }
            }
        }

        private void AccessPolicies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AccessPolicy>())
                {
                    item.TargetTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<RoleSchemaView> _roleSchemaViews;

        [InverseProperty("RulebookTable")]
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
                            throw new InvalidOperationException("Cannot access RoleSchemaViews - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>();
                    }
                    else
                    {
                        var items = Context.RoleSchemaViews.Where(x => x.TargetTable == this.RulebookTableId).ToList<RoleSchemaView>();
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
                    item.TargetTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<AccessDenialTest> _accessDenialTests;

        [InverseProperty("RulebookTable")]
        public virtual ObservableCollection<AccessDenialTest> AccessDenialTests
        {
            get
            {
                if (_accessDenialTests == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessDenialTests - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>();
                    }
                    else
                    {
                        var items = Context.AccessDenialTests.Where(x => x.TargetTable == this.RulebookTableId).ToList<AccessDenialTest>();
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _accessDenialTests.CollectionChanged += AccessDenialTests_CollectionChanged;
                }
                return _accessDenialTests;
            }
            private set
            {
                if (_accessDenialTests != null)
                {
                    _accessDenialTests.CollectionChanged -= AccessDenialTests_CollectionChanged;
                }
                _accessDenialTests = value;
                if (_accessDenialTests != null)
                {
                    _accessDenialTests.CollectionChanged += AccessDenialTests_CollectionChanged;
                }
            }
        }

        private void AccessDenialTests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AccessDenialTest>())
                {
                    item.TargetTable = this.RulebookTableId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AccessPolicies;
            _ = this.RoleSchemaViews;
            _ = this.AccessDenialTests;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
