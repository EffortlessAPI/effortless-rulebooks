
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
    [Table("StepProtectiveEquipment")]
    public class StepProtectiveEquipmentBase : SoAEntityBase
    {
        [Key]
        public string StepProtectiveEquipmentId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{ProtectiveEquipment}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(" / "), F.Text(F.Of(this.ProtectiveEquipment))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Step { get; set; }
        public string? ProtectiveEquipment { get; set; }

        private Step _stepRef;

        [ForeignKey("Step")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(Step))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(Step);
                    if (_stepRef != null)
                    {
                        base.SoAContext.Attach(_stepRef);
                    }
                }
                return _stepRef;
            }
            set
            {
                if (_stepRef != value)
                {
                    _stepRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepRef != null)
                    {
                        Step = _stepRef.StepId;
                    }
                }
            }
        }

        private ProtectiveEquipment _protectiveEquipmentRef;

        [ForeignKey("ProtectiveEquipment")]
        public virtual ProtectiveEquipment ProtectiveEquipmentRef
        {
            get
            {
                if (_protectiveEquipmentRef == null && !string.IsNullOrEmpty(ProtectiveEquipment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProtectiveEquipmentRef - no database context is set. ProtectiveEquipment: " + ProtectiveEquipment + ".");
                        }
                        return null;
                    }
                    _protectiveEquipmentRef = base.SoAContext.ProtectiveEquipment.Find(ProtectiveEquipment);
                    if (_protectiveEquipmentRef != null)
                    {
                        base.SoAContext.Attach(_protectiveEquipmentRef);
                    }
                }
                return _protectiveEquipmentRef;
            }
            set
            {
                if (_protectiveEquipmentRef != value)
                {
                    _protectiveEquipmentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_protectiveEquipmentRef != null)
                    {
                        ProtectiveEquipment = _protectiveEquipmentRef.ProtectiveEquipmentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.ProtectiveEquipmentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
