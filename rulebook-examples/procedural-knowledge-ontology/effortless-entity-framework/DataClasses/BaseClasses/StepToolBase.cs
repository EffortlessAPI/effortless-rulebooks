
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("StepTools")]
    public class StepToolBase : SoAEntityBase
    {
        [Key]
        public string StepToolId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{Tool}})
        public string? Name
        {
            get => this.Step + " / " + this.Tool; set { }
        }


        public string? Step { get; set; }
        public string? Tool { get; set; }

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

        private Tool _tool;

        [ForeignKey("Tool")]
        public virtual Tool Tool
        {
            get
            {
                if (_tool == null && !string.IsNullOrEmpty(Tool))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Tool - no database context is set. Tool: " + Tool + ".");
                        }
                        return null;
                    }
                    _tool = Context.Tools.Find(Tool);
                    if (_tool != null)
                    {
                        Context.Attach(_tool);
                    }
                }
                return _tool;
            }
            set
            {
                if (_tool != value)
                {
                    _tool = value;
                    Tool = _tool == null ? default : _tool.ToolId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Step;
            _ = this.Tool;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
