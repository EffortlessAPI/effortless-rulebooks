
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
    [Table("StepFunctions")]
    public class StepFunctionBase : SoAEntityBase
    {
        [Key]
        public string StepFunctionId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{Function}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(" / "), F.Text(F.Of(this.Function))))); set { }
        }


        public string? Step { get; set; }
        public string? Function { get; set; }

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

        private Function _functionRef;

        [ForeignKey("Function")]
        public virtual Function FunctionRef
        {
            get
            {
                if (_functionRef == null && !string.IsNullOrEmpty(Function))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FunctionRef - no database context is set. Function: " + Function + ".");
                        }
                        return null;
                    }
                    _functionRef = base.SoAContext.Functions.Find(Function);
                    if (_functionRef != null)
                    {
                        base.SoAContext.Attach(_functionRef);
                    }
                }
                return _functionRef;
            }
            set
            {
                if (_functionRef != value)
                {
                    _functionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_functionRef != null)
                    {
                        Function = _functionRef.FunctionId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.FunctionRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
