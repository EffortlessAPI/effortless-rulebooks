
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
    [Table("AiToolInvocations")]
    public class AiToolInvocationBase : SoAEntityBase
    {
        [Key]
        public string AiToolInvocationId { get; set; }

        // Formula Name (rulebook: ={{InvokingAgent}} & " -> " & {{Function}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.InvokingAgent)), F.S(" -> "), F.Text(F.Of(this.Function))))); set { }
        }

        public DateTimeOffset? InvokedAt { get; set; }
        public int? SuppliedInputCount { get; set; }
        // Formula ExecutedStep (rulebook: =INDEX(StepExecutions!{{Step}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? ExecutedStep
        {
            get => F.AsString(F.Memo(this, "ExecutedStep", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.Step), () => F.Of(new StepExecution().Step)))); set { }
        }

        // Formula DeclaredFunctionCount (rulebook: =COUNTIFS(StepFunctions!{{Step}}, {{ExecutedStep}}, StepFunctions!{{Function}}, {{Function}}))
        [NotMapped]
        public int? DeclaredFunctionCount
        {
            get => F.AsInt(F.Memo(this, "DeclaredFunctionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepFunction>(base.SoAContext, "StepFunctions", __c => __c.StepFunctions), __r => F.CritField(F.Of(__r.Step), F.Of(this.ExecutedStep)) && F.CritField(F.Of(__r.Function), F.Of(this.Function))))))); set { }
        }

        // Formula IsUndeclaredToolUse (rulebook: ={{DeclaredFunctionCount}} = 0)
        [NotMapped]
        public bool? IsUndeclaredToolUse
        {
            get => F.AsBool(F.Memo(this, "IsUndeclaredToolUse", () => F.Eq(F.Of(this.DeclaredFunctionCount), F.I(0)))); set { }
        }

        // Formula DeclaredInputCount (rulebook: =INDEX(StepExecutions!{{StepInputVariableCount}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public int? DeclaredInputCount
        {
            get => F.AsInt(F.Memo(this, "DeclaredInputCount", () => F.Integer(F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.StepInputVariableCount), () => F.Of(new StepExecution().StepInputVariableCount))))); set { }
        }

        // Formula ActedWithoutDeclaredContext (rulebook: =AND({{DeclaredInputCount}} > 0, {{SuppliedInputCount}} < {{DeclaredInputCount}}))
        [NotMapped]
        public bool? ActedWithoutDeclaredContext
        {
            get => F.AsBool(F.Memo(this, "ActedWithoutDeclaredContext", () => F.And(F.Bool3(F.Cmp(F.Of(this.DeclaredInputCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.SuppliedInputCount)), "<", F.Of(this.DeclaredInputCount)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? InvokingAgent { get; set; }
        public string? StepExecution { get; set; }
        public string? Function { get; set; }

        private Agent _agent;

        [ForeignKey("InvokingAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(InvokingAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. InvokingAgent: " + InvokingAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(InvokingAgent);
                    if (_agent != null)
                    {
                        base.SoAContext.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agent != null)
                    {
                        InvokingAgent = _agent.AgentId;
                    }
                }
            }
        }

        private StepExecution _stepExecutionRef;

        [ForeignKey("StepExecution")]
        public virtual StepExecution StepExecutionRef
        {
            get
            {
                if (_stepExecutionRef == null && !string.IsNullOrEmpty(StepExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecutionRef - no database context is set. StepExecution: " + StepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecutionRef = base.SoAContext.StepExecutions.Find(StepExecution);
                    if (_stepExecutionRef != null)
                    {
                        base.SoAContext.Attach(_stepExecutionRef);
                    }
                }
                return _stepExecutionRef;
            }
            set
            {
                if (_stepExecutionRef != value)
                {
                    _stepExecutionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepExecutionRef != null)
                    {
                        StepExecution = _stepExecutionRef.StepExecutionId;
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
            _ = this.Agent;
            _ = this.StepExecutionRef;
            _ = this.FunctionRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
