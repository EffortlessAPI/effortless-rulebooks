
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
    [Table("RulebookTables")]
    public class RulebookTableBase : SoAEntityBase
    {
        [Key]
        public string RulebookTableId { get; set; }

        public string TableName { get; set; }
        // Formula Name (rulebook: ={{TableName}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.TableName))); set { }
        }

        public string? PhysicalTable { get; set; }
        public string? PhysicalView { get; set; }
        public string? SubjectArea { get; set; }
        public bool? IsExtension { get; set; }
        // Formula FieldCount (rulebook: =COUNTIFS(RulebookFields!{{TargetTable}}, {{RulebookTableId}}))
        [NotMapped]
        public decimal? FieldCount
        {
            get => F.AsDecimal(F.Memo(this, "FieldCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RulebookField>(base.SoAContext, "RulebookFields", __c => __c.RulebookFields), __r => F.CritField(F.Of(__r.TargetTable), F.Of(this.RulebookTableId)))))); set { }
        }

        // Formula PolicyCount (rulebook: =COUNTIFS(AccessPolicies!{{TargetTable}}, {{RulebookTableId}}))
        [NotMapped]
        public decimal? PolicyCount
        {
            get => F.AsDecimal(F.Memo(this, "PolicyCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AccessPolicy>(base.SoAContext, "AccessPolicies", __c => __c.AccessPolicies), __r => F.CritField(F.Of(__r.TargetTable), F.Of(this.RulebookTableId)))))); set { }
        }

        // Formula IsUnsecured (rulebook: ={{PolicyCount}} = 0)
        [NotMapped]
        public bool? IsUnsecured
        {
            get => F.AsBool(F.Memo(this, "IsUnsecured", () => F.Eq(F.Of(this.PolicyCount), F.I(0)))); set { }
        }

        // Formula DisagreeingSubstrateCount (rulebook: =COUNTIFS(TableConformance!{{ImperfectTableKey}}, {{RulebookTableId}}))
        [NotMapped]
        public decimal? DisagreeingSubstrateCount
        {
            get => F.AsDecimal(F.Memo(this, "DisagreeingSubstrateCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TableConformance>(base.SoAContext, "TableConformance", __c => __c.TableConformance), __r => F.CritField(F.Of(__r.ImperfectTableKey), F.Of(this.RulebookTableId)))))); set { }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPolicies - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _accessPolicies = new ObservableCollection<AccessPolicy>();
                    }
                    else
                    {
                        var items = base.SoAContext.AccessPolicies.Where(x => x.TargetTable == this.RulebookTableId).ToList<AccessPolicy>();
                        _accessPolicies = new ObservableCollection<AccessPolicy>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleSchemaViews - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleSchemaViews.Where(x => x.TargetTable == this.RulebookTableId).ToList<RoleSchemaView>();
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessDenialTests - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>();
                    }
                    else
                    {
                        var items = base.SoAContext.AccessDenialTests.Where(x => x.TargetTable == this.RulebookTableId).ToList<AccessDenialTest>();
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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

        private ObservableCollection<TableConformance> _tableConformance;

        [InverseProperty("RulebookTableRef")]
        public virtual ObservableCollection<TableConformance> TableConformance
        {
            get
            {
                if (_tableConformance == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TableConformance - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _tableConformance = new ObservableCollection<TableConformance>();
                    }
                    else
                    {
                        var items = base.SoAContext.TableConformance.Where(x => x.RulebookTable == this.RulebookTableId).ToList<TableConformance>();
                        _tableConformance = new ObservableCollection<TableConformance>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _tableConformance.CollectionChanged += TableConformance_CollectionChanged;
                }
                return _tableConformance;
            }
            private set
            {
                if (_tableConformance != null)
                {
                    _tableConformance.CollectionChanged -= TableConformance_CollectionChanged;
                }
                _tableConformance = value;
                if (_tableConformance != null)
                {
                    _tableConformance.CollectionChanged += TableConformance_CollectionChanged;
                }
            }
        }

        private void TableConformance_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TableConformance>())
                {
                    item.RulebookTable = this.RulebookTableId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AccessPolicies;
            _ = this.RoleSchemaViews;
            _ = this.AccessDenialTests;
            _ = this.TableConformance;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
