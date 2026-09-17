
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
    [Table("ExecutionEntities")]
    public class ExecutionEntityBase : SoAEntityBase
    {
        [Key]
        public string ExecutionEntityId { get; set; }

        // Formula Name (rulebook: ={{StepExecution}} & " " & {{Usage}} & " " & {{EntityLabel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.StepExecution)), F.S(" "), F.Text(F.Of(this.Usage)), F.S(" "), F.Text(F.Of(this.EntityLabel))))); set { }
        }

        public string? Usage { get; set; }
        public string? EntityLabel { get; set; }
        public string? EntityUri { get; set; }
        // Formula VariableDirection (rulebook: =INDEX(StepVariables!{{Direction}}, MATCH({{StepVariable}}, StepVariables!{{StepVariableId}}, 0)))
        [NotMapped]
        public string? VariableDirection
        {
            get => F.AsString(F.Memo(this, "VariableDirection", () => F.Lookup<StepVariable>(this, "StepVariables", "StepVariableId", __c => __c.StepVariables, __r => F.Of(__r.StepVariableId), F.Of(this.StepVariable), __r => F.Of(__r.Direction), () => F.Of(new StepVariable().Direction)))); set { }
        }

        // Formula VariableStep (rulebook: =INDEX(StepVariables!{{Step}}, MATCH({{StepVariable}}, StepVariables!{{StepVariableId}}, 0)))
        [NotMapped]
        public string? VariableStep
        {
            get => F.AsString(F.Memo(this, "VariableStep", () => F.Lookup<StepVariable>(this, "StepVariables", "StepVariableId", __c => __c.StepVariables, __r => F.Of(__r.StepVariableId), F.Of(this.StepVariable), __r => F.Of(__r.Step), () => F.Of(new StepVariable().Step)))); set { }
        }

        // Formula ExecutedStep (rulebook: =INDEX(StepExecutions!{{Step}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? ExecutedStep
        {
            get => F.AsString(F.Memo(this, "ExecutedStep", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.Step), () => F.Of(new StepExecution().Step)))); set { }
        }

        // Formula IsUsageDirectionMismatch (rulebook: =OR(AND({{Usage}} = "Used", {{VariableDirection}} <> "Input"), AND({{Usage}} = "Generated", {{VariableDirection}} <> "Output")))
        [NotMapped]
        public bool? IsUsageDirectionMismatch
        {
            get => F.AsBool(F.Memo(this, "IsUsageDirectionMismatch", () => F.Or(F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Usage)), F.S("Used"))), F.Bool3(F.Ne(F.Of(this.VariableDirection), F.S("Input"))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Usage)), F.S("Generated"))), F.Bool3(F.Ne(F.Of(this.VariableDirection), F.S("Output")))))))); set { }
        }

        // Formula IsForeignVariable (rulebook: ={{VariableStep}} <> {{ExecutedStep}})
        [NotMapped]
        public bool? IsForeignVariable
        {
            get => F.AsBool(F.Memo(this, "IsForeignVariable", () => F.Ne(F.Of(this.VariableStep), F.Of(this.ExecutedStep)))); set { }
        }

        // Formula UsedExecutionKey (rulebook: =IF({{Usage}} = "Used", {{StepExecution}}, ""))
        [NotMapped]
        public string? UsedExecutionKey
        {
            get => F.AsString(F.Memo(this, "UsedExecutionKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Usage)), F.S("Used")))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        // Formula GeneratedExecutionKey (rulebook: =IF({{Usage}} = "Generated", {{StepExecution}}, ""))
        [NotMapped]
        public string? GeneratedExecutionKey
        {
            get => F.AsString(F.Memo(this, "GeneratedExecutionKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Usage)), F.S("Generated")))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        public string? RecordedDatatype { get; set; }
        public string? RecordedFormat { get; set; }
        // Formula VariableDatatype (rulebook: =INDEX(StepVariables!{{Datatype}}, MATCH({{StepVariable}}, StepVariables!{{StepVariableId}}, 0)))
        [NotMapped]
        public string? VariableDatatype
        {
            get => F.AsString(F.Memo(this, "VariableDatatype", () => F.Lookup<StepVariable>(this, "StepVariables", "StepVariableId", __c => __c.StepVariables, __r => F.Of(__r.StepVariableId), F.Of(this.StepVariable), __r => F.Of(__r.Datatype), () => F.Of(new StepVariable().Datatype)))); set { }
        }

        // Formula VariableExpectedFormat (rulebook: =INDEX(StepVariables!{{ExpectedFormat}}, MATCH({{StepVariable}}, StepVariables!{{StepVariableId}}, 0)))
        [NotMapped]
        public string? VariableExpectedFormat
        {
            get => F.AsString(F.Memo(this, "VariableExpectedFormat", () => F.Lookup<StepVariable>(this, "StepVariables", "StepVariableId", __c => __c.StepVariables, __r => F.Of(__r.StepVariableId), F.Of(this.StepVariable), __r => F.Of(__r.ExpectedFormat), () => F.Of(new StepVariable().ExpectedFormat)))); set { }
        }

        // Formula ViolatesDeclaredDatatypeOrFormat (rulebook: =OR(AND({{RecordedDatatype}} <> "", {{RecordedDatatype}} <> {{VariableDatatype}}), AND({{RecordedFormat}} <> "", {{VariableExpectedFormat}} <> "", {{RecordedFormat}} <> {{VariableExpectedFormat}})))
        [NotMapped]
        public bool? ViolatesDeclaredDatatypeOrFormat
        {
            get => F.AsBool(F.Memo(this, "ViolatesDeclaredDatatypeOrFormat", () => F.Or(F.Bool3(F.And(F.Bool3(F.IsNotBlank(F.Of(this.RecordedDatatype))), F.Bool3(F.Ne(F.Nullif(F.Of(this.RecordedDatatype)), F.Of(this.VariableDatatype))))), F.Bool3(F.And(F.Bool3(F.IsNotBlank(F.Of(this.RecordedFormat))), F.Bool3(F.IsNotBlank(F.Of(this.VariableExpectedFormat))), F.Bool3(F.Ne(F.Nullif(F.Of(this.RecordedFormat)), F.Of(this.VariableExpectedFormat)))))))); set { }
        }

        // Formula GeneratingAgent (rulebook: =INDEX(StepExecutions!{{ExecutedByAgent}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? GeneratingAgent
        {
            get => F.AsString(F.Memo(this, "GeneratingAgent", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.ExecutedByAgent), () => F.Of(new StepExecution().ExecutedByAgent)))); set { }
        }

        // Formula AttributedToAgent (rulebook: =IF({{Usage}} = "Generated", {{GeneratingAgent}}, ""))
        [NotMapped]
        public string? AttributedToAgent
        {
            get => F.AsString(F.Memo(this, "AttributedToAgent", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Usage)), F.S("Generated")))) ? F.Of(this.GeneratingAgent) : F.S("")))); set { }
        }


        public string? StepExecution { get; set; }
        public string? StepVariable { get; set; }

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

        private StepVariable _stepVariableRef;

        [ForeignKey("StepVariable")]
        public virtual StepVariable StepVariableRef
        {
            get
            {
                if (_stepVariableRef == null && !string.IsNullOrEmpty(StepVariable))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepVariableRef - no database context is set. StepVariable: " + StepVariable + ".");
                        }
                        return null;
                    }
                    _stepVariableRef = base.SoAContext.StepVariables.Find(StepVariable);
                    if (_stepVariableRef != null)
                    {
                        base.SoAContext.Attach(_stepVariableRef);
                    }
                }
                return _stepVariableRef;
            }
            set
            {
                if (_stepVariableRef != value)
                {
                    _stepVariableRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepVariableRef != null)
                    {
                        StepVariable = _stepVariableRef.StepVariableId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecutionRef;
            _ = this.StepVariableRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
