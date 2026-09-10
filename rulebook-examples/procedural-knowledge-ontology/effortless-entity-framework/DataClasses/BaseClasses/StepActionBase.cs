
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("StepActions")]
    public class StepActionBase : SoAEntityBase
    {
        [Key]
        public string StepActionId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{Action}})
        public string? Name
        {
            get => this.Step + " / " + this.Action; set { }
        }


        public string? Step { get; set; }
        public string? Action { get; set; }

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

        private Action _action;

        [ForeignKey("Action")]
        public virtual Action Action
        {
            get
            {
                if (_action == null && !string.IsNullOrEmpty(Action))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Action - no database context is set. Action: " + Action + ".");
                        }
                        return null;
                    }
                    _action = Context.Actions.Find(Action);
                    if (_action != null)
                    {
                        Context.Attach(_action);
                    }
                }
                return _action;
            }
            set
            {
                if (_action != value)
                {
                    _action = value;
                    Action = _action == null ? default : _action.ActionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Step;
            _ = this.Action;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
