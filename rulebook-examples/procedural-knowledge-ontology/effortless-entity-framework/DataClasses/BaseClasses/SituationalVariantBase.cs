
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
    [Table("SituationalVariants")]
    public class SituationalVariantBase : SoAEntityBase
    {
        [Key]
        public string SituationalVariantId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Situation { get; set; }
        public string? Adaptation { get; set; }
        // Formula ScopeDimensionCount (rulebook: =INDEX(ApplicabilityScopes!{{DimensionCount}}, MATCH({{ApplicabilityScope}}, ApplicabilityScopes!{{ApplicabilityScopeId}}, 0)))
        [NotMapped]
        public int? ScopeDimensionCount
        {
            get => F.AsInt(F.Memo(this, "ScopeDimensionCount", () => F.Integer(F.Lookup<ApplicabilityScope>(this, "ApplicabilityScopes", "ApplicabilityScopeId", __c => __c.ApplicabilityScopes, __r => F.Of(__r.ApplicabilityScopeId), F.Of(this.ApplicabilityScope), __r => F.Of(__r.DimensionCount), () => F.Of(new ApplicabilityScope().DimensionCount))))); set { }
        }

        // Formula ScopeStatesConditions (rulebook: =INDEX(ApplicabilityScopes!{{StatesConditions}}, MATCH({{ApplicabilityScope}}, ApplicabilityScopes!{{ApplicabilityScopeId}}, 0)))
        [NotMapped]
        public bool? ScopeStatesConditions
        {
            get => F.AsBool(F.Memo(this, "ScopeStatesConditions", () => F.Lookup<ApplicabilityScope>(this, "ApplicabilityScopes", "ApplicabilityScopeId", __c => __c.ApplicabilityScopes, __r => F.Of(__r.ApplicabilityScopeId), F.Of(this.ApplicabilityScope), __r => F.Of(__r.StatesConditions), () => F.Of(new ApplicabilityScope().StatesConditions)))); set { }
        }

        // Formula HasNoApplicabilityDimension (rulebook: =OR({{ApplicabilityScope}} = "", {{ScopeDimensionCount}} = 0))
        [NotMapped]
        public bool? HasNoApplicabilityDimension
        {
            get => F.AsBool(F.Memo(this, "HasNoApplicabilityDimension", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.ApplicabilityScope))), F.Bool3(F.Eq(F.Of(this.ScopeDimensionCount), F.I(0)))))); set { }
        }

        // Formula DivergesWithoutStatedConditions (rulebook: =AND({{Adaptation}} <> "", OR({{ApplicabilityScope}} = "", {{ScopeStatesConditions}} = FALSE)))
        [NotMapped]
        public bool? DivergesWithoutStatedConditions
        {
            get => F.AsBool(F.Memo(this, "DivergesWithoutStatedConditions", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.Adaptation))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ApplicabilityScope))), F.Bool3(F.Eq(F.Of(this.ScopeStatesConditions), F.B(false)))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
        public string? ApplicabilityScope { get; set; }
        public string? ExpertAgent { get; set; }

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

        private ApplicabilityScope _applicabilityScopeRef;

        [ForeignKey("ApplicabilityScope")]
        public virtual ApplicabilityScope ApplicabilityScopeRef
        {
            get
            {
                if (_applicabilityScopeRef == null && !string.IsNullOrEmpty(ApplicabilityScope))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApplicabilityScopeRef - no database context is set. ApplicabilityScope: " + ApplicabilityScope + ".");
                        }
                        return null;
                    }
                    _applicabilityScopeRef = base.SoAContext.ApplicabilityScopes.Find(ApplicabilityScope);
                    if (_applicabilityScopeRef != null)
                    {
                        base.SoAContext.Attach(_applicabilityScopeRef);
                    }
                }
                return _applicabilityScopeRef;
            }
            set
            {
                if (_applicabilityScopeRef != value)
                {
                    _applicabilityScopeRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_applicabilityScopeRef != null)
                    {
                        ApplicabilityScope = _applicabilityScopeRef.ApplicabilityScopeId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("ExpertAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ExpertAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ExpertAgent: " + ExpertAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ExpertAgent);
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
                        ExpertAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.ApplicabilityScopeRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
