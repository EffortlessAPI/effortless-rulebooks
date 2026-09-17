
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
    [Table("ProcedureTargets")]
    public class ProcedureTargetBase : SoAEntityBase
    {
        [Key]
        public string ProcedureTargetId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? TargetKind { get; set; }
        // Formula MachineIsNonStandard (rulebook: =INDEX(Machines!{{IsNonStandardConfiguration}}, MATCH({{Machine}}, Machines!{{MachineId}}, 0)))
        [NotMapped]
        public bool? MachineIsNonStandard
        {
            get => F.AsBool(F.Memo(this, "MachineIsNonStandard", () => F.Lookup<Machine>(this, "Machines", "MachineId", __c => __c.Machines, __r => F.Of(__r.MachineId), F.Of(this.Machine), __r => F.Of(__r.IsNonStandardConfiguration), () => F.Of(new Machine().IsNonStandardConfiguration)))); set { }
        }

        // Formula HandlingFailureModeCount (rulebook: =COUNTIFS(FailureModes!{{ProcedureTarget}}, {{ProcedureTargetId}}))
        [NotMapped]
        public int? HandlingFailureModeCount
        {
            get => F.AsInt(F.Memo(this, "HandlingFailureModeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<FailureMode>(base.SoAContext, "FailureModes", __c => __c.FailureModes), __r => F.CritField(F.Of(__r.ProcedureTarget), F.Of(this.ProcedureTargetId))))))); set { }
        }

        // Formula IsUnhandledNonStandardTarget (rulebook: =AND({{MachineIsNonStandard}}, {{HandlingFailureModeCount}} = 0))
        [NotMapped]
        public bool? IsUnhandledNonStandardTarget
        {
            get => F.AsBool(F.Memo(this, "IsUnhandledNonStandardTarget", () => F.And(F.Bool3(F.Of(this.MachineIsNonStandard)), F.Bool3(F.Eq(F.Of(this.HandlingFailureModeCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
        public string? Machine { get; set; }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }

        private Machine _machineRef;

        [ForeignKey("Machine")]
        public virtual Machine MachineRef
        {
            get
            {
                if (_machineRef == null && !string.IsNullOrEmpty(Machine))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MachineRef - no database context is set. Machine: " + Machine + ".");
                        }
                        return null;
                    }
                    _machineRef = base.SoAContext.Machines.Find(Machine);
                    if (_machineRef != null)
                    {
                        base.SoAContext.Attach(_machineRef);
                    }
                }
                return _machineRef;
            }
            set
            {
                if (_machineRef != value)
                {
                    _machineRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_machineRef != null)
                    {
                        Machine = _machineRef.MachineId;
                    }
                }
            }
        }

        private ObservableCollection<FailureMode> _failureModes;

        [InverseProperty("ProcedureTargetRef")]
        public virtual ObservableCollection<FailureMode> FailureModes
        {
            get
            {
                if (_failureModes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FailureModes - no database context is set. ProcedureTargetId: " + this.ProcedureTargetId + ".");
                        }
                        _failureModes = new ObservableCollection<FailureMode>();
                    }
                    else
                    {
                        var items = base.SoAContext.FailureModes.Where(x => x.ProcedureTarget == this.ProcedureTargetId).ToList<FailureMode>();
                        _failureModes = new ObservableCollection<FailureMode>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _failureModes.CollectionChanged += FailureModes_CollectionChanged;
                }
                return _failureModes;
            }
            private set
            {
                if (_failureModes != null)
                {
                    _failureModes.CollectionChanged -= FailureModes_CollectionChanged;
                }
                _failureModes = value;
                if (_failureModes != null)
                {
                    _failureModes.CollectionChanged += FailureModes_CollectionChanged;
                }
            }
        }

        private void FailureModes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FailureMode>())
                {
                    item.ProcedureTarget = this.ProcedureTargetId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.MachineRef;
            _ = this.FailureModes;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
