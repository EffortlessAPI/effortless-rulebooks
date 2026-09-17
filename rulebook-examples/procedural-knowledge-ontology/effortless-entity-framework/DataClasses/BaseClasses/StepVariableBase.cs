
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
    [Table("StepVariables")]
    public class StepVariableBase : SoAEntityBase
    {
        [Key]
        public string StepVariableId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " " & {{Direction}} & " " & {{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(" "), F.Text(F.Of(this.Direction)), F.S(" "), F.Text(F.Of(this.Label))))); set { }
        }

        public string? Direction { get; set; }
        public string? Label { get; set; }
        public string? Datatype { get; set; }
        public bool? IsExternalInput { get; set; }
        // Formula SourceDirection (rulebook: =INDEX(StepVariables!{{Direction}}, MATCH({{SourceVariable}}, StepVariables!{{StepVariableId}}, 0)))
        [NotMapped]
        public string? SourceDirection
        {
            get => F.AsString(F.Memo(this, "SourceDirection", () => F.Lookup<StepVariable>(this, "StepVariables", "StepVariableId", __c => __c.StepVariables, __r => F.Of(__r.StepVariableId), F.Of(this.SourceVariable), __r => F.Of(__r.Direction), () => F.Of(new StepVariable().Direction)))); set { }
        }

        // Formula SourceStep (rulebook: =INDEX(StepVariables!{{Step}}, MATCH({{SourceVariable}}, StepVariables!{{StepVariableId}}, 0)))
        [NotMapped]
        public string? SourceStep
        {
            get => F.AsString(F.Memo(this, "SourceStep", () => F.Lookup<StepVariable>(this, "StepVariables", "StepVariableId", __c => __c.StepVariables, __r => F.Of(__r.StepVariableId), F.Of(this.SourceVariable), __r => F.Of(__r.Step), () => F.Of(new StepVariable().Step)))); set { }
        }

        // Formula IsDanglingInput (rulebook: =AND({{Direction}} = "Input", {{SourceVariable}} = "", {{IsExternalInput}} = FALSE))
        [NotMapped]
        public bool? IsDanglingInput
        {
            get => F.AsBool(F.Memo(this, "IsDanglingInput", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Direction)), F.S("Input"))), F.Bool3(F.IsBlank(F.Of(this.SourceVariable))), F.Bool3(F.Eq(F.Nullif(F.Of(this.IsExternalInput)), F.B(false)))))); set { }
        }

        // Formula IsMiswiredSource (rulebook: =AND({{SourceVariable}} <> "", {{SourceDirection}} <> "Output"))
        [NotMapped]
        public bool? IsMiswiredSource
        {
            get => F.AsBool(F.Memo(this, "IsMiswiredSource", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.SourceVariable))), F.Bool3(F.Ne(F.Of(this.SourceDirection), F.S("Output")))))); set { }
        }

        // Formula ConsumerCount (rulebook: =COUNTIFS(StepVariables!{{SourceVariable}}, StepVariables!{{StepVariableId}}))
        [NotMapped]
        public int? ConsumerCount
        {
            get => F.AsInt(F.Memo(this, "ConsumerCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepVariable>(base.SoAContext, "StepVariables", __c => __c.StepVariables), __r => F.CritField(F.Of(__r.SourceVariable), F.Of(this.StepVariableId))))))); set { }
        }

        // Formula InputStepKey (rulebook: =IF({{Direction}} = "Input", {{Step}}, ""))
        [NotMapped]
        public string? InputStepKey
        {
            get => F.AsString(F.Memo(this, "InputStepKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Direction)), F.S("Input")))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula OutputStepKey (rulebook: =IF({{Direction}} = "Output", {{Step}}, ""))
        [NotMapped]
        public string? OutputStepKey
        {
            get => F.AsString(F.Memo(this, "OutputStepKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Direction)), F.S("Output")))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        public string? ExpectedFormat { get; set; }
        // Formula StepAccountableAgent (rulebook: =INDEX(Steps!{{AccountableAgent}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? StepAccountableAgent
        {
            get => F.AsString(F.Memo(this, "StepAccountableAgent", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.AccountableAgent), () => F.Of(new Step().AccountableAgent)))); set { }
        }

        // Formula StepAgentKind (rulebook: =INDEX(Steps!{{AssignedAgentKind}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? StepAgentKind
        {
            get => F.AsString(F.Memo(this, "StepAgentKind", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.AssignedAgentKind), () => F.Of(new Step().AssignedAgentKind)))); set { }
        }

        // Formula ConsumerRole (rulebook: =INDEX(Steps!{{AssignedRole}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? ConsumerRole
        {
            get => F.AsString(F.Memo(this, "ConsumerRole", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.AssignedRole), () => F.Of(new Step().AssignedRole)))); set { }
        }

        // Formula ConsumerWorkflow (rulebook: =INDEX(Steps!{{ProcedureVersion}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? ConsumerWorkflow
        {
            get => F.AsString(F.Memo(this, "ConsumerWorkflow", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.ProcedureVersion), () => F.Of(new Step().ProcedureVersion)))); set { }
        }

        // Formula SourceStepAgent (rulebook: =INDEX(Steps!{{AccountableAgent}}, MATCH({{SourceStep}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? SourceStepAgent
        {
            get => F.AsString(F.Memo(this, "SourceStepAgent", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.SourceStep), __r => F.Of(__r.AccountableAgent), () => F.Of(new Step().AccountableAgent)))); set { }
        }

        // Formula SourceStepAgentKind (rulebook: =INDEX(Steps!{{AssignedAgentKind}}, MATCH({{SourceStep}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? SourceStepAgentKind
        {
            get => F.AsString(F.Memo(this, "SourceStepAgentKind", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.SourceStep), __r => F.Of(__r.AssignedAgentKind), () => F.Of(new Step().AssignedAgentKind)))); set { }
        }

        // Formula IsInputFromAiArtifact (rulebook: =AND({{Direction}} = "Input", {{SourceStepAgentKind}} = "AIAgent"))
        [NotMapped]
        public bool? IsInputFromAiArtifact
        {
            get => F.AsBool(F.Memo(this, "IsInputFromAiArtifact", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Direction)), F.S("Input"))), F.Bool3(F.Eq(F.Of(this.SourceStepAgentKind), F.S("AIAgent")))))); set { }
        }

        // Formula AiArtifactConsumerHasNoAccountableAgent (rulebook: =AND({{IsInputFromAiArtifact}}, {{StepAccountableAgent}} = ""))
        [NotMapped]
        public bool? AiArtifactConsumerHasNoAccountableAgent
        {
            get => F.AsBool(F.Memo(this, "AiArtifactConsumerHasNoAccountableAgent", () => F.And(F.Bool3(F.Of(this.IsInputFromAiArtifact)), F.Bool3(F.IsBlank(F.Of(this.StepAccountableAgent)))))); set { }
        }

        // Formula AiBlastRadiusPath (rulebook: =IF({{IsInputFromAiArtifact}}, {{SourceStepAgent}} & " produces " & {{Label}} & " for " & {{Step}} & "; role " & {{ConsumerRole}} & "; agent " & {{StepAccountableAgent}} & "; workflow " & {{ConsumerWorkflow}}, ""))
        [NotMapped]
        public string? AiBlastRadiusPath
        {
            get => F.AsString(F.Memo(this, "AiBlastRadiusPath", () => (F.Truthy(F.Bool3(F.Of(this.IsInputFromAiArtifact))) ? F.Concat(F.Text(F.Of(this.SourceStepAgent)), F.S(" produces "), F.Text(F.Of(this.Label)), F.S(" for "), F.Text(F.Of(this.Step)), F.S("; role "), F.Text(F.Of(this.ConsumerRole)), F.S("; agent "), F.Text(F.Of(this.StepAccountableAgent)), F.S("; workflow "), F.Text(F.Of(this.ConsumerWorkflow))) : F.S("")))); set { }
        }


        public string? Step { get; set; }
        public string? SourceVariable { get; set; }

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

        private StepVariable _stepVariable;

        [ForeignKey("SourceVariable")]
        public virtual StepVariable StepVariable
        {
            get
            {
                if (_stepVariable == null && !string.IsNullOrEmpty(SourceVariable))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepVariable - no database context is set. SourceVariable: " + SourceVariable + ".");
                        }
                        return null;
                    }
                    _stepVariable = base.SoAContext.StepVariables.Find(SourceVariable);
                    if (_stepVariable != null)
                    {
                        base.SoAContext.Attach(_stepVariable);
                    }
                }
                return _stepVariable;
            }
            set
            {
                if (_stepVariable != value)
                {
                    _stepVariable = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepVariable != null)
                    {
                        SourceVariable = _stepVariable.StepVariableId;
                    }
                }
            }
        }

        private ObservableCollection<StepVariable> _stepVariables;

        [InverseProperty("StepVariable")]
        public virtual ObservableCollection<StepVariable> StepVariables
        {
            get
            {
                if (_stepVariables == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepVariables - no database context is set. StepVariableId: " + this.StepVariableId + ".");
                        }
                        _stepVariables = new ObservableCollection<StepVariable>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepVariables.Where(x => x.SourceVariable == this.StepVariableId).ToList<StepVariable>();
                        _stepVariables = new ObservableCollection<StepVariable>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepVariables.CollectionChanged += StepVariables_CollectionChanged;
                }
                return _stepVariables;
            }
            private set
            {
                if (_stepVariables != null)
                {
                    _stepVariables.CollectionChanged -= StepVariables_CollectionChanged;
                }
                _stepVariables = value;
                if (_stepVariables != null)
                {
                    _stepVariables.CollectionChanged += StepVariables_CollectionChanged;
                }
            }
        }

        private void StepVariables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepVariable>())
                {
                    item.SourceVariable = this.StepVariableId;
                }
            }
        }

        private ObservableCollection<ExecutionEntity> _executionEntities;

        [InverseProperty("StepVariableRef")]
        public virtual ObservableCollection<ExecutionEntity> ExecutionEntities
        {
            get
            {
                if (_executionEntities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExecutionEntities - no database context is set. StepVariableId: " + this.StepVariableId + ".");
                        }
                        _executionEntities = new ObservableCollection<ExecutionEntity>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExecutionEntities.Where(x => x.StepVariable == this.StepVariableId).ToList<ExecutionEntity>();
                        _executionEntities = new ObservableCollection<ExecutionEntity>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _executionEntities.CollectionChanged += ExecutionEntities_CollectionChanged;
                }
                return _executionEntities;
            }
            private set
            {
                if (_executionEntities != null)
                {
                    _executionEntities.CollectionChanged -= ExecutionEntities_CollectionChanged;
                }
                _executionEntities = value;
                if (_executionEntities != null)
                {
                    _executionEntities.CollectionChanged += ExecutionEntities_CollectionChanged;
                }
            }
        }

        private void ExecutionEntities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExecutionEntity>())
                {
                    item.StepVariable = this.StepVariableId;
                }
            }
        }

        private ObservableCollection<ArtifactHandoff> _artifactHandoffs;

        [InverseProperty("StepVariableRef")]
        public virtual ObservableCollection<ArtifactHandoff> ArtifactHandoffs
        {
            get
            {
                if (_artifactHandoffs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ArtifactHandoffs - no database context is set. StepVariableId: " + this.StepVariableId + ".");
                        }
                        _artifactHandoffs = new ObservableCollection<ArtifactHandoff>();
                    }
                    else
                    {
                        var items = base.SoAContext.ArtifactHandoffs.Where(x => x.StepVariable == this.StepVariableId).ToList<ArtifactHandoff>();
                        _artifactHandoffs = new ObservableCollection<ArtifactHandoff>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _artifactHandoffs.CollectionChanged += ArtifactHandoffs_CollectionChanged;
                }
                return _artifactHandoffs;
            }
            private set
            {
                if (_artifactHandoffs != null)
                {
                    _artifactHandoffs.CollectionChanged -= ArtifactHandoffs_CollectionChanged;
                }
                _artifactHandoffs = value;
                if (_artifactHandoffs != null)
                {
                    _artifactHandoffs.CollectionChanged += ArtifactHandoffs_CollectionChanged;
                }
            }
        }

        private void ArtifactHandoffs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ArtifactHandoff>())
                {
                    item.StepVariable = this.StepVariableId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.StepVariable;
            _ = this.StepVariables;
            _ = this.ExecutionEntities;
            _ = this.ArtifactHandoffs;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
