
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
    [Table("ExpertCognitions")]
    public class ExpertCognitionBase : SoAEntityBase
    {
        [Key]
        public string ExpertCognitionId { get; set; }

        // Formula Name (rulebook: ={{CognitionKind}} & ": " & LEFT({{Statement}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.CognitionKind)), F.S(": "), F.Text(F.Left(F.Of(this.Statement), F.I(60)))))); set { }
        }

        public string? CognitionKind { get; set; }
        public string? Statement { get; set; }
        public bool? AppliedAutomatically { get; set; }
        public bool? ExpertStatedExceptions { get; set; }
        public string? CapturedExceptions { get; set; }
        // Formula IsMentalModel (rulebook: ={{CognitionKind}} = "MentalModel")
        [NotMapped]
        public bool? IsMentalModel
        {
            get => F.AsBool(F.Memo(this, "IsMentalModel", () => F.Eq(F.Nullif(F.Of(this.CognitionKind)), F.S("MentalModel")))); set { }
        }

        // Formula IsAutomaticHeuristic (rulebook: =AND({{CognitionKind}} = "DecisionHeuristic", {{AppliedAutomatically}}))
        [NotMapped]
        public bool? IsAutomaticHeuristic
        {
            get => F.AsBool(F.Memo(this, "IsAutomaticHeuristic", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.CognitionKind)), F.S("DecisionHeuristic"))), F.IsTrueV(F.Of(this.AppliedAutomatically))))); set { }
        }

        // Formula IsHeuristicOversimplified (rulebook: =AND({{CognitionKind}} = "DecisionHeuristic", {{ExpertStatedExceptions}}, {{CapturedExceptions}} = ""))
        [NotMapped]
        public bool? IsHeuristicOversimplified
        {
            get => F.AsBool(F.Memo(this, "IsHeuristicOversimplified", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.CognitionKind)), F.S("DecisionHeuristic"))), F.IsTrueV(F.Of(this.ExpertStatedExceptions)), F.Bool3(F.IsBlank(F.Of(this.CapturedExceptions)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Agent { get; set; }
        public string? Step { get; set; }
        public string? ElicitationSession { get; set; }

        private Agent _agentRef;

        [ForeignKey("Agent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(Agent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. Agent: " + Agent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(Agent);
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
                        Agent = _agentRef.AgentId;
                    }
                }
            }
        }

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

        private ElicitationSession _elicitationSessionRef;

        [ForeignKey("ElicitationSession")]
        public virtual ElicitationSession ElicitationSessionRef
        {
            get
            {
                if (_elicitationSessionRef == null && !string.IsNullOrEmpty(ElicitationSession))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ElicitationSessionRef - no database context is set. ElicitationSession: " + ElicitationSession + ".");
                        }
                        return null;
                    }
                    _elicitationSessionRef = base.SoAContext.ElicitationSessions.Find(ElicitationSession);
                    if (_elicitationSessionRef != null)
                    {
                        base.SoAContext.Attach(_elicitationSessionRef);
                    }
                }
                return _elicitationSessionRef;
            }
            set
            {
                if (_elicitationSessionRef != value)
                {
                    _elicitationSessionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_elicitationSessionRef != null)
                    {
                        ElicitationSession = _elicitationSessionRef.ElicitationSessionId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AgentRef;
            _ = this.StepRef;
            _ = this.ElicitationSessionRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
