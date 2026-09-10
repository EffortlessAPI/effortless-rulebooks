
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ProcedureTypes")]
    public class ProcedureTypeBase : SoAEntityBase
    {
        [Key]
        public string ProcedureTypeId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        public string? Name
        {
            get => this.Label; set { }
        }

        public string? Label { get; set; }
        public string? Definition { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<Procedure> _procedures;

        [InverseProperty("ProcedureType")]
        public virtual ObservableCollection<Procedure> Procedures
        {
            get
            {
                if (_procedures == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedures - no database context is set. ProcedureTypeId: " + this.ProcedureTypeId + ".");
                        }
                        _procedures = new ObservableCollection<Procedure>();
                    }
                    else
                    {
                        var items = Context.Procedures.Where(x => x.ProcedureType == this.ProcedureTypeId).ToList<Procedure>();
                        _procedures = new ObservableCollection<Procedure>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
                return _procedures;
            }
            private set
            {
                if (_procedures != null)
                {
                    _procedures.CollectionChanged -= Procedures_CollectionChanged;
                }
                _procedures = value;
                if (_procedures != null)
                {
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
            }
        }

        private void Procedures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Procedure>())
                {
                    item.ProcedureType = this.ProcedureTypeId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Procedures;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
