
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
    [Table("StakeholderLenses")]
    public class StakeholderLenseBase : SoAEntityBase
    {
        [Key]
        public string StakeholderLensId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public bool? NeedsStepGuidance { get; set; }
        public bool? NeedsMetrics { get; set; }
        public bool? NeedsExceptionHandling { get; set; }
        public bool? NeedsComplianceEvidence { get; set; }
        public bool? NeedsStructuredConstraints { get; set; }
        public string? RequiredGranularity { get; set; }
        public string? PreferredForm { get; set; }
        // Formula GranularityRank (rulebook: =IF({{RequiredGranularity}} = "Step", 3, IF({{RequiredGranularity}} = "Stage", 2, IF({{RequiredGranularity}} = "Category", 1, 0))))
        [NotMapped]
        public int? GranularityRank
        {
            get => F.AsInt(F.Memo(this, "GranularityRank", () => F.Integer((F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.RequiredGranularity)), F.S("Step")))) ? F.I(3) : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.RequiredGranularity)), F.S("Stage")))) ? F.I(2) : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.RequiredGranularity)), F.S("Category")))) ? F.I(1) : F.I(0))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ExemplarRole { get; set; }

        private Role _role;

        [ForeignKey("ExemplarRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(ExemplarRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. ExemplarRole: " + ExemplarRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(ExemplarRole);
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
                        ExemplarRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<ProcedureLensView> _procedureLensViews;

        [InverseProperty("StakeholderLense")]
        public virtual ObservableCollection<ProcedureLensView> ProcedureLensViews
        {
            get
            {
                if (_procedureLensViews == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureLensViews - no database context is set. StakeholderLensId: " + this.StakeholderLensId + ".");
                        }
                        _procedureLensViews = new ObservableCollection<ProcedureLensView>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureLensViews.Where(x => x.StakeholderLens == this.StakeholderLensId).ToList<ProcedureLensView>();
                        _procedureLensViews = new ObservableCollection<ProcedureLensView>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureLensViews.CollectionChanged += ProcedureLensViews_CollectionChanged;
                }
                return _procedureLensViews;
            }
            private set
            {
                if (_procedureLensViews != null)
                {
                    _procedureLensViews.CollectionChanged -= ProcedureLensViews_CollectionChanged;
                }
                _procedureLensViews = value;
                if (_procedureLensViews != null)
                {
                    _procedureLensViews.CollectionChanged += ProcedureLensViews_CollectionChanged;
                }
            }
        }

        private void ProcedureLensViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureLensView>())
                {
                    item.StakeholderLens = this.StakeholderLensId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.ProcedureLensViews;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
