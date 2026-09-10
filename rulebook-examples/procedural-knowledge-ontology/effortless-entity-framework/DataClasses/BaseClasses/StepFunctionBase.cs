
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("StepFunctions")]
    public class StepFunctionBase : SoAEntityBase
    {
        [Key]
        public string StepFunctionId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{Function}})
        public string? Name
        {
            get => this.Step + " / " + this.Function; set { }
        }


        public string? Step { get; set; }
        public string? Function { get; set; }

        private Step _step;

        [ForeignKey("Step")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(Step))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _step = Context.Steps.Find(Step);
                    if (_step != null)
                    {
                        Context.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    Step = _step == null ? default : _step.StepId;
                }
            }
        }

        private Function _function;

        [ForeignKey("Function")]
        public virtual Function Function
        {
            get
            {
                if (_function == null && !string.IsNullOrEmpty(Function))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Function - no database context is set. Function: " + Function + ".");
                        }
                        return null;
                    }
                    _function = Context.Functions.Find(Function);
                    if (_function != null)
                    {
                        Context.Attach(_function);
                    }
                }
                return _function;
            }
            set
            {
                if (_function != value)
                {
                    _function = value;
                    Function = _function == null ? default : _function.FunctionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Step;
            _ = this.Function;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
