
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
    [Table("CueObservations")]
    public class CueObservationBase : SoAEntityBase
    {
        [Key]
        public string CueObservationId { get; set; }

        // Formula Name (rulebook: ={{StepExecution}} & " / " & {{StepCue}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.StepExecution)), F.S(" / "), F.Text(F.Of(this.StepCue))))); set { }
        }

        public DateTimeOffset? ObservedAt { get; set; }
        public bool? WasEscalated { get; set; }
        // Formula CueRequiresEscalation (rulebook: =INDEX(StepCues!{{RequiresEscalation}}, MATCH({{StepCue}}, StepCues!{{StepCueId}}, 0)))
        [NotMapped]
        public bool? CueRequiresEscalation
        {
            get => F.AsBool(F.Memo(this, "CueRequiresEscalation", () => F.Lookup<StepCue>(this, "StepCues", "StepCueId", __c => __c.StepCues, __r => F.Of(__r.StepCueId), F.Of(this.StepCue), __r => F.Of(__r.RequiresEscalation), () => F.Of(new StepCue().RequiresEscalation)))); set { }
        }

        // Formula IsUnescalatedDangerCue (rulebook: =AND({{CueRequiresEscalation}}, {{WasEscalated}} = FALSE))
        [NotMapped]
        public bool? IsUnescalatedDangerCue
        {
            get => F.AsBool(F.Memo(this, "IsUnescalatedDangerCue", () => F.And(F.Bool3(F.Of(this.CueRequiresEscalation)), F.Bool3(F.Eq(F.Nullif(F.Of(this.WasEscalated)), F.B(false)))))); set { }
        }

        // Formula UnescalatedCueKey (rulebook: =IF({{IsUnescalatedDangerCue}}, {{StepCue}}, ""))
        [NotMapped]
        public string? UnescalatedCueKey
        {
            get => F.AsString(F.Memo(this, "UnescalatedCueKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnescalatedDangerCue))) ? F.Of(this.StepCue) : F.S("")))); set { }
        }

        // Formula UnescalatedExecutionKey (rulebook: =IF({{IsUnescalatedDangerCue}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? UnescalatedExecutionKey
        {
            get => F.AsString(F.Memo(this, "UnescalatedExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnescalatedDangerCue))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        // Formula OwnerOrganization (rulebook: =INDEX(StepExecutions!{{OwnerOrganization}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? OwnerOrganization
        {
            get => F.AsString(F.Memo(this, "OwnerOrganization", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.OwnerOrganization), () => F.Of(new StepExecution().OwnerOrganization)))); set { }
        }

        // Formula CueSignalsIncompleteStep (rulebook: =INDEX(StepCues!{{SignalsIncompleteStep}}, MATCH({{StepCue}}, StepCues!{{StepCueId}}, 0)))
        [NotMapped]
        public bool? CueSignalsIncompleteStep
        {
            get => F.AsBool(F.Memo(this, "CueSignalsIncompleteStep", () => F.Lookup<StepCue>(this, "StepCues", "StepCueId", __c => __c.StepCues, __r => F.Of(__r.StepCueId), F.Of(this.StepCue), __r => F.Of(__r.SignalsIncompleteStep), () => F.Of(new StepCue().SignalsIncompleteStep)))); set { }
        }

        public DateTimeOffset? AcknowledgedAt { get; set; }
        // Formula IsAwaitingAcknowledgement (rulebook: =AND({{WasEscalated}} = TRUE, {{AcknowledgedAt}} = ""))
        [NotMapped]
        public bool? IsAwaitingAcknowledgement
        {
            get => F.AsBool(F.Memo(this, "IsAwaitingAcknowledgement", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.WasEscalated)), F.B(true))), F.Bool3(F.IsBlank(F.Of(this.AcknowledgedAt)))))); set { }
        }


        public string? StepExecution { get; set; }
        public string? StepCue { get; set; }
        public string? ObservedByAgent { get; set; }
        public string? EscalatedToAgent { get; set; }

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

        private StepCue _stepCueRef;

        [ForeignKey("StepCue")]
        public virtual StepCue StepCueRef
        {
            get
            {
                if (_stepCueRef == null && !string.IsNullOrEmpty(StepCue))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepCueRef - no database context is set. StepCue: " + StepCue + ".");
                        }
                        return null;
                    }
                    _stepCueRef = base.SoAContext.StepCues.Find(StepCue);
                    if (_stepCueRef != null)
                    {
                        base.SoAContext.Attach(_stepCueRef);
                    }
                }
                return _stepCueRef;
            }
            set
            {
                if (_stepCueRef != value)
                {
                    _stepCueRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepCueRef != null)
                    {
                        StepCue = _stepCueRef.StepCueId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("ObservedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ObservedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ObservedByAgent: " + ObservedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ObservedByAgent);
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
                        ObservedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("EscalatedToAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(EscalatedToAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. EscalatedToAgent: " + EscalatedToAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(EscalatedToAgent);
                    if (_agentRef != null)
                    {
                        base.SoAContext.Attach(_agentRef);
                    }
                }
                return _agentRef;
            }
            set
            {
                if (_agentRef != value)
                {
                    _agentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRef != null)
                    {
                        EscalatedToAgent = _agentRef.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecutionRef;
            _ = this.StepCueRef;
            _ = this.Agent;
            _ = this.AgentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
