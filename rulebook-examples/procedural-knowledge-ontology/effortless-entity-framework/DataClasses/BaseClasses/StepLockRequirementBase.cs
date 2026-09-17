
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
    [Table("StepLockRequirements")]
    public class StepLockRequirementBase : SoAEntityBase
    {
        [Key]
        public string StepLockRequirementId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{LockDevice}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(" / "), F.Text(F.Of(this.LockDevice))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Step { get; set; }
        public string? LockDevice { get; set; }

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

        private LockDevice _lockDeviceRef;

        [ForeignKey("LockDevice")]
        public virtual LockDevice LockDeviceRef
        {
            get
            {
                if (_lockDeviceRef == null && !string.IsNullOrEmpty(LockDevice))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LockDeviceRef - no database context is set. LockDevice: " + LockDevice + ".");
                        }
                        return null;
                    }
                    _lockDeviceRef = base.SoAContext.LockDevices.Find(LockDevice);
                    if (_lockDeviceRef != null)
                    {
                        base.SoAContext.Attach(_lockDeviceRef);
                    }
                }
                return _lockDeviceRef;
            }
            set
            {
                if (_lockDeviceRef != value)
                {
                    _lockDeviceRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_lockDeviceRef != null)
                    {
                        LockDevice = _lockDeviceRef.LockDeviceId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.LockDeviceRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
