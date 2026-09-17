
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
    [Table("ApplicabilityScopes")]
    public class ApplicabilityScopeBase : SoAEntityBase
    {
        [Key]
        public string ApplicabilityScopeId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Geography { get; set; }
        public string? CustomerSegment { get; set; }
        public string? AppliesWhen { get; set; }
        public string? ExclusionCondition { get; set; }
        public string? NamedGraphIri { get; set; }
        // Formula DimensionCount (rulebook: =IF({{BusinessUnit}} <> "", 1, 0) + IF({{Geography}} <> "", 1, 0) + IF({{CustomerSegment}} <> "", 1, 0) + IF({{RegulatoryRegime}} <> "", 1, 0))
        [NotMapped]
        public int? DimensionCount
        {
            get => F.AsInt(F.Memo(this, "DimensionCount", () => F.Integer(F.Add(F.Add(F.Add((F.Truthy(F.Bool3(F.IsNotBlank(F.Of(this.BusinessUnit)))) ? F.I(1) : F.I(0)), (F.Truthy(F.Bool3(F.IsNotBlank(F.Of(this.Geography)))) ? F.I(1) : F.I(0))), (F.Truthy(F.Bool3(F.IsNotBlank(F.Of(this.CustomerSegment)))) ? F.I(1) : F.I(0))), (F.Truthy(F.Bool3(F.IsNotBlank(F.Of(this.RegulatoryRegime)))) ? F.I(1) : F.I(0)))))); set { }
        }

        // Formula StatesConditions (rulebook: =OR({{AppliesWhen}} <> "", {{ExclusionCondition}} <> ""))
        [NotMapped]
        public bool? StatesConditions
        {
            get => F.AsBool(F.Memo(this, "StatesConditions", () => F.Or(F.Bool3(F.IsNotBlank(F.Of(this.AppliesWhen))), F.Bool3(F.IsNotBlank(F.Of(this.ExclusionCondition)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? BusinessUnit { get; set; }
        public string? RegulatoryRegime { get; set; }

        private Organization _organization;

        [ForeignKey("BusinessUnit")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(BusinessUnit))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. BusinessUnit: " + BusinessUnit + ".");
                        }
                        return null;
                    }
                    _organization = base.SoAContext.Organizations.Find(BusinessUnit);
                    if (_organization != null)
                    {
                        base.SoAContext.Attach(_organization);
                    }
                }
                return _organization;
            }
            set
            {
                if (_organization != value)
                {
                    _organization = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organization != null)
                    {
                        BusinessUnit = _organization.OrganizationId;
                    }
                }
            }
        }

        private RegulatoryFramework _regulatoryFramework;

        [ForeignKey("RegulatoryRegime")]
        public virtual RegulatoryFramework RegulatoryFramework
        {
            get
            {
                if (_regulatoryFramework == null && !string.IsNullOrEmpty(RegulatoryRegime))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RegulatoryFramework - no database context is set. RegulatoryRegime: " + RegulatoryRegime + ".");
                        }
                        return null;
                    }
                    _regulatoryFramework = base.SoAContext.RegulatoryFrameworks.Find(RegulatoryRegime);
                    if (_regulatoryFramework != null)
                    {
                        base.SoAContext.Attach(_regulatoryFramework);
                    }
                }
                return _regulatoryFramework;
            }
            set
            {
                if (_regulatoryFramework != value)
                {
                    _regulatoryFramework = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_regulatoryFramework != null)
                    {
                        RegulatoryRegime = _regulatoryFramework.RegulatoryFrameworkId;
                    }
                }
            }
        }

        private ObservableCollection<StepContextSensitivity> _stepContextSensitivities;

        [InverseProperty("ApplicabilityScopeRef")]
        public virtual ObservableCollection<StepContextSensitivity> StepContextSensitivities
        {
            get
            {
                if (_stepContextSensitivities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepContextSensitivities - no database context is set. ApplicabilityScopeId: " + this.ApplicabilityScopeId + ".");
                        }
                        _stepContextSensitivities = new ObservableCollection<StepContextSensitivity>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepContextSensitivities.Where(x => x.ApplicabilityScope == this.ApplicabilityScopeId).ToList<StepContextSensitivity>();
                        _stepContextSensitivities = new ObservableCollection<StepContextSensitivity>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepContextSensitivities.CollectionChanged += StepContextSensitivities_CollectionChanged;
                }
                return _stepContextSensitivities;
            }
            private set
            {
                if (_stepContextSensitivities != null)
                {
                    _stepContextSensitivities.CollectionChanged -= StepContextSensitivities_CollectionChanged;
                }
                _stepContextSensitivities = value;
                if (_stepContextSensitivities != null)
                {
                    _stepContextSensitivities.CollectionChanged += StepContextSensitivities_CollectionChanged;
                }
            }
        }

        private void StepContextSensitivities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepContextSensitivity>())
                {
                    item.ApplicabilityScope = this.ApplicabilityScopeId;
                }
            }
        }

        private ObservableCollection<SituationalVariant> _situationalVariants;

        [InverseProperty("ApplicabilityScopeRef")]
        public virtual ObservableCollection<SituationalVariant> SituationalVariants
        {
            get
            {
                if (_situationalVariants == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SituationalVariants - no database context is set. ApplicabilityScopeId: " + this.ApplicabilityScopeId + ".");
                        }
                        _situationalVariants = new ObservableCollection<SituationalVariant>();
                    }
                    else
                    {
                        var items = base.SoAContext.SituationalVariants.Where(x => x.ApplicabilityScope == this.ApplicabilityScopeId).ToList<SituationalVariant>();
                        _situationalVariants = new ObservableCollection<SituationalVariant>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _situationalVariants.CollectionChanged += SituationalVariants_CollectionChanged;
                }
                return _situationalVariants;
            }
            private set
            {
                if (_situationalVariants != null)
                {
                    _situationalVariants.CollectionChanged -= SituationalVariants_CollectionChanged;
                }
                _situationalVariants = value;
                if (_situationalVariants != null)
                {
                    _situationalVariants.CollectionChanged += SituationalVariants_CollectionChanged;
                }
            }
        }

        private void SituationalVariants_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SituationalVariant>())
                {
                    item.ApplicabilityScope = this.ApplicabilityScopeId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Organization;
            _ = this.RegulatoryFramework;
            _ = this.StepContextSensitivities;
            _ = this.SituationalVariants;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
