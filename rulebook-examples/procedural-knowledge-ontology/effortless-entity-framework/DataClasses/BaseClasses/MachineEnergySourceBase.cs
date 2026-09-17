
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
    [Table("MachineEnergySources")]
    public class MachineEnergySourceBase : SoAEntityBase
    {
        [Key]
        public string MachineEnergySourceId { get; set; }

        // Formula Name (rulebook: ={{Machine}} & " / " & {{EnergySource}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Machine)), F.S(" / "), F.Text(F.Of(this.EnergySource))))); set { }
        }

        // Formula MachineProcedureVersion (rulebook: =INDEX(Machines!{{GoverningProcedureVersion}}, MATCH({{Machine}}, Machines!{{MachineId}}, 0)))
        [NotMapped]
        public string? MachineProcedureVersion
        {
            get => F.AsString(F.Memo(this, "MachineProcedureVersion", () => F.Lookup<Machine>(this, "Machines", "MachineId", __c => __c.Machines, __r => F.Of(__r.MachineId), F.Of(this.Machine), __r => F.Of(__r.GoverningProcedureVersion), () => F.Of(new Machine().GoverningProcedureVersion)))); set { }
        }

        // Formula IsolationStepCount (rulebook: =COUNTIFS(Steps!{{IsolatesEnergySource}}, {{EnergySource}}, Steps!{{ProcedureVersion}}, {{MachineProcedureVersion}}))
        [NotMapped]
        public int? IsolationStepCount
        {
            get => F.AsInt(F.Memo(this, "IsolationStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.IsolatesEnergySource), F.Of(this.EnergySource)) && F.CritField(F.Of(__r.ProcedureVersion), F.Of(this.MachineProcedureVersion))))))); set { }
        }

        // Formula IsUnisolatedEnergySource (rulebook: ={{IsolationStepCount}} = 0)
        [NotMapped]
        public bool? IsUnisolatedEnergySource
        {
            get => F.AsBool(F.Memo(this, "IsUnisolatedEnergySource", () => F.Eq(F.Of(this.IsolationStepCount), F.I(0)))); set { }
        }

        // Formula UnisolatedMachineKey (rulebook: =IF({{IsUnisolatedEnergySource}}, {{Machine}}, ""))
        [NotMapped]
        public string? UnisolatedMachineKey
        {
            get => F.AsString(F.Memo(this, "UnisolatedMachineKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnisolatedEnergySource))) ? F.Of(this.Machine) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Machine { get; set; }
        public string? EnergySource { get; set; }

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

        private EnergySource _energySourceRef;

        [ForeignKey("EnergySource")]
        public virtual EnergySource EnergySourceRef
        {
            get
            {
                if (_energySourceRef == null && !string.IsNullOrEmpty(EnergySource))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EnergySourceRef - no database context is set. EnergySource: " + EnergySource + ".");
                        }
                        return null;
                    }
                    _energySourceRef = base.SoAContext.EnergySources.Find(EnergySource);
                    if (_energySourceRef != null)
                    {
                        base.SoAContext.Attach(_energySourceRef);
                    }
                }
                return _energySourceRef;
            }
            set
            {
                if (_energySourceRef != value)
                {
                    _energySourceRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_energySourceRef != null)
                    {
                        EnergySource = _energySourceRef.EnergySourceId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.MachineRef;
            _ = this.EnergySourceRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
