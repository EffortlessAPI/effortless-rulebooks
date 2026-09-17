
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
    [Table("SourceRelationships")]
    public class SourceRelationshipBase : SoAEntityBase
    {
        [Key]
        public string SourceRelationshipId { get; set; }

        // Formula Name (rulebook: ={{KnowledgeEngineer}} & " with " & {{SourceAgent}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.KnowledgeEngineer)), F.S(" with "), F.Text(F.Of(this.SourceAgent))))); set { }
        }

        public string? SourceStanding { get; set; }
        public string? PowerDynamic { get; set; }
        public string? NegotiatedAgreement { get; set; }
        public string? TrustLevel { get; set; }
        public string? TrustBuildingPractice { get; set; }
        public bool? WithholdingObserved { get; set; }
        public string? WithholdingMotive { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        // Formula IsUnnegotiatedPowerGap (rulebook: =AND({{PowerDynamic}} <> "", {{PowerDynamic}} <> "Balanced", {{NegotiatedAgreement}} = ""))
        [NotMapped]
        public bool? IsUnnegotiatedPowerGap
        {
            get => F.AsBool(F.Memo(this, "IsUnnegotiatedPowerGap", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PowerDynamic))), F.Bool3(F.Ne(F.Nullif(F.Of(this.PowerDynamic)), F.S("Balanced"))), F.Bool3(F.IsBlank(F.Of(this.NegotiatedAgreement)))))); set { }
        }

        // Formula IsExtractiveRelationship (rulebook: =AND({{TrustLevel}} <> "", {{TrustLevel}} <> "Established", {{TrustBuildingPractice}} = ""))
        [NotMapped]
        public bool? IsExtractiveRelationship
        {
            get => F.AsBool(F.Memo(this, "IsExtractiveRelationship", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.TrustLevel))), F.Bool3(F.Ne(F.Nullif(F.Of(this.TrustLevel)), F.S("Established"))), F.Bool3(F.IsBlank(F.Of(this.TrustBuildingPractice)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? KnowledgeEngineer { get; set; }
        public string? SourceAgent { get; set; }
        public string? Procedure { get; set; }

        private Agent _agent;

        [ForeignKey("KnowledgeEngineer")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(KnowledgeEngineer))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. KnowledgeEngineer: " + KnowledgeEngineer + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(KnowledgeEngineer);
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
                        KnowledgeEngineer = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("SourceAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(SourceAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. SourceAgent: " + SourceAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(SourceAgent);
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
                        SourceAgent = _agentRef.AgentId;
                    }
                }
            }
        }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.ProcedureRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
