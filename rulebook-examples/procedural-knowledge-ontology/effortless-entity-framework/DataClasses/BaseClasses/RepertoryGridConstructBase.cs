
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
    [Table("RepertoryGridConstructs")]
    public class RepertoryGridConstructBase : SoAEntityBase
    {
        [Key]
        public string RepertoryGridConstructId { get; set; }

        // Formula Name (rulebook: ={{Dimension}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Dimension))); set { }
        }

        public string? SituationsCompared { get; set; }
        public string? PoleA { get; set; }
        public string? PoleB { get; set; }
        public bool? WasStatedUnprompted { get; set; }
        public bool? SeparatesSituations { get; set; }
        // Formula Dimension (rulebook: ={{PoleA}} & " vs " & {{PoleB}})
        [NotMapped]
        public string? Dimension
        {
            get => F.AsString(F.Memo(this, "Dimension", () => F.Concat(F.Text(F.Of(this.PoleA)), F.S(" vs "), F.Text(F.Of(this.PoleB))))); set { }
        }

        // Formula IsNeverStatedDimension (rulebook: =AND({{PoleA}} <> "", NOT({{WasStatedUnprompted}})))
        [NotMapped]
        public bool? IsNeverStatedDimension
        {
            get => F.AsBool(F.Memo(this, "IsNeverStatedDimension", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PoleA))), F.Bool3(F.Not(F.IsTrueV(F.Of(this.WasStatedUnprompted))))))); set { }
        }

        // Formula IsRecordedDiscriminatingDimension (rulebook: =AND({{PoleA}} <> "", {{SeparatesSituations}}))
        [NotMapped]
        public bool? IsRecordedDiscriminatingDimension
        {
            get => F.AsBool(F.Memo(this, "IsRecordedDiscriminatingDimension", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PoleA))), F.IsTrueV(F.Of(this.SeparatesSituations))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ElicitationSession { get; set; }
        public string? Agent { get; set; }

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


        protected override void LazyLoadProperties()
        {
            _ = this.ElicitationSessionRef;
            _ = this.AgentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
