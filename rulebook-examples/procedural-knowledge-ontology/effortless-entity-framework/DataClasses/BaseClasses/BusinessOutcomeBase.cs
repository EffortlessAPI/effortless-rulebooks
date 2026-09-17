
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
    [Table("BusinessOutcomes")]
    public class BusinessOutcomeBase : SoAEntityBase
    {
        [Key]
        public string BusinessOutcomeId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? StrategicObjective { get; set; }
        // Formula LinkedMeasureCount (rulebook: =COUNTIFS(ProcessOutcomeMeasures!{{BusinessOutcome}}, {{BusinessOutcomeId}}))
        [NotMapped]
        public int? LinkedMeasureCount
        {
            get => F.AsInt(F.Memo(this, "LinkedMeasureCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcessOutcomeMeasure>(base.SoAContext, "ProcessOutcomeMeasures", __c => __c.ProcessOutcomeMeasures), __r => F.CritField(F.Of(__r.BusinessOutcome), F.Of(this.BusinessOutcomeId))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? OwnerRole { get; set; }

        private Role _role;

        [ForeignKey("OwnerRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(OwnerRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. OwnerRole: " + OwnerRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(OwnerRole);
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
                        OwnerRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<ProcessOutcomeMeasure> _processOutcomeMeasures;

        [InverseProperty("BusinessOutcomeRef")]
        public virtual ObservableCollection<ProcessOutcomeMeasure> ProcessOutcomeMeasures
        {
            get
            {
                if (_processOutcomeMeasures == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessOutcomeMeasures - no database context is set. BusinessOutcomeId: " + this.BusinessOutcomeId + ".");
                        }
                        _processOutcomeMeasures = new ObservableCollection<ProcessOutcomeMeasure>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessOutcomeMeasures.Where(x => x.BusinessOutcome == this.BusinessOutcomeId).ToList<ProcessOutcomeMeasure>();
                        _processOutcomeMeasures = new ObservableCollection<ProcessOutcomeMeasure>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _processOutcomeMeasures.CollectionChanged += ProcessOutcomeMeasures_CollectionChanged;
                }
                return _processOutcomeMeasures;
            }
            private set
            {
                if (_processOutcomeMeasures != null)
                {
                    _processOutcomeMeasures.CollectionChanged -= ProcessOutcomeMeasures_CollectionChanged;
                }
                _processOutcomeMeasures = value;
                if (_processOutcomeMeasures != null)
                {
                    _processOutcomeMeasures.CollectionChanged += ProcessOutcomeMeasures_CollectionChanged;
                }
            }
        }

        private void ProcessOutcomeMeasures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessOutcomeMeasure>())
                {
                    item.BusinessOutcome = this.BusinessOutcomeId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.ProcessOutcomeMeasures;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
