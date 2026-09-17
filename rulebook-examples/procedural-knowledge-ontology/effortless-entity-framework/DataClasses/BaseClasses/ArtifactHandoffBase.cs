
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
    [Table("ArtifactHandoffs")]
    public class ArtifactHandoffBase : SoAEntityBase
    {
        [Key]
        public string ArtifactHandoffId { get; set; }

        // Formula Name (rulebook: ={{FromStep}} & " -> " & {{ToStep}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.FromStep)), F.S(" -> "), F.Text(F.Of(this.ToStep))))); set { }
        }

        // Formula DeclaredSourceStep (rulebook: =INDEX(StepVariables!{{SourceStep}}, MATCH({{StepVariable}}, StepVariables!{{StepVariableId}}, 0)))
        [NotMapped]
        public string? DeclaredSourceStep
        {
            get => F.AsString(F.Memo(this, "DeclaredSourceStep", () => F.Lookup<StepVariable>(this, "StepVariables", "StepVariableId", __c => __c.StepVariables, __r => F.Of(__r.StepVariableId), F.Of(this.StepVariable), __r => F.Of(__r.SourceStep), () => F.Of(new StepVariable().SourceStep)))); set { }
        }

        // Formula DeclaredConsumerStep (rulebook: =INDEX(StepVariables!{{Step}}, MATCH({{StepVariable}}, StepVariables!{{StepVariableId}}, 0)))
        [NotMapped]
        public string? DeclaredConsumerStep
        {
            get => F.AsString(F.Memo(this, "DeclaredConsumerStep", () => F.Lookup<StepVariable>(this, "StepVariables", "StepVariableId", __c => __c.StepVariables, __r => F.Of(__r.StepVariableId), F.Of(this.StepVariable), __r => F.Of(__r.Step), () => F.Of(new StepVariable().Step)))); set { }
        }

        // Formula DisagreesWithDeclaredVariable (rulebook: =OR({{FromStep}} <> {{DeclaredSourceStep}}, {{ToStep}} <> {{DeclaredConsumerStep}}))
        [NotMapped]
        public bool? DisagreesWithDeclaredVariable
        {
            get => F.AsBool(F.Memo(this, "DisagreesWithDeclaredVariable", () => F.Or(F.Bool3(F.Ne(F.Nullif(F.Of(this.FromStep)), F.Of(this.DeclaredSourceStep))), F.Bool3(F.Ne(F.Nullif(F.Of(this.ToStep)), F.Of(this.DeclaredConsumerStep)))))); set { }
        }

        public string? HandoffClosure { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? StepVariable { get; set; }
        public string? FromStep { get; set; }
        public string? ToStep { get; set; }

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

        private Step _step;

        [ForeignKey("FromStep")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(FromStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. FromStep: " + FromStep + ".");
                        }
                        return null;
                    }
                    _step = base.SoAContext.Steps.Find(FromStep);
                    if (_step != null)
                    {
                        base.SoAContext.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_step != null)
                    {
                        FromStep = _step.StepId;
                    }
                }
            }
        }

        private Step _stepRef;

        [ForeignKey("ToStep")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(ToStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. ToStep: " + ToStep + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(ToStep);
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
                        ToStep = _stepRef.StepId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepVariableRef;
            _ = this.Step;
            _ = this.StepRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
