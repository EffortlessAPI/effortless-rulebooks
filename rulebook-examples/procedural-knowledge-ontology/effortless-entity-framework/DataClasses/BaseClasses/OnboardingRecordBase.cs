
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
    [Table("OnboardingRecords")]
    public class OnboardingRecordBase : SoAEntityBase
    {
        [Key]
        public string OnboardingRecordId { get; set; }

        // Formula Name (rulebook: ={{NewStarter}} & " on " & {{Procedure}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.NewStarter)), F.S(" on "), F.Text(F.Of(this.Procedure))))); set { }
        }

        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? ProficientAt { get; set; }
        public bool? UsedCapturedKnowledge { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula IsProficient (rulebook: ={{ProficientAt}} <> "")
        [NotMapped]
        public bool? IsProficient
        {
            get => F.AsBool(F.Memo(this, "IsProficient", () => F.IsNotBlank(F.Of(this.ProficientAt)))); set { }
        }

        // Formula DaysToProficiency (rulebook: =IF(OR({{StartedAt}} = "", {{ProficientAt}} = ""), 0, DATETIME_DIFF({{ProficientAt}}, {{StartedAt}}, "days")))
        [NotMapped]
        public int? DaysToProficiency
        {
            get => F.AsInt(F.Memo(this, "DaysToProficiency", () => F.Integer((F.Truthy(F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.StartedAt))), F.Bool3(F.IsBlank(F.Of(this.ProficientAt)))))) ? F.I(0) : F.DatetimeDiff(F.Of(this.ProficientAt), F.Of(this.StartedAt), F.S("days")))))); set { }
        }

        // Formula DaysSinceStart (rulebook: =IF({{StartedAt}} = "", 99999, DATETIME_DIFF({{AsOfInstant}}, {{StartedAt}}, "days")))
        [NotMapped]
        public int? DaysSinceStart
        {
            get => F.AsInt(F.Memo(this, "DaysSinceStart", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.StartedAt)))) ? F.I(99999) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.StartedAt), F.S("days")))))); set { }
        }

        // Formula IsRecentStart (rulebook: =AND({{DaysSinceStart}} >= 0, {{DaysSinceStart}} <= 180))
        [NotMapped]
        public bool? IsRecentStart
        {
            get => F.AsBool(F.Memo(this, "IsRecentStart", () => F.And(F.Bool3(F.Cmp(F.Of(this.DaysSinceStart), ">=", F.I(0))), F.Bool3(F.Cmp(F.Of(this.DaysSinceStart), "<=", F.I(180)))))); set { }
        }

        // Formula ProcedureRepositoryEntryCount (rulebook: =INDEX(Procedures!{{RepositoryEntryCount}}, MATCH({{Procedure}}, Procedures!{{ProcedureId}}, 0)))
        [NotMapped]
        public int? ProcedureRepositoryEntryCount
        {
            get => F.AsInt(F.Memo(this, "ProcedureRepositoryEntryCount", () => F.Integer(F.Lookup<Procedure>(this, "Procedures", "ProcedureId", __c => __c.Procedures, __r => F.Of(__r.ProcedureId), F.Of(this.Procedure), __r => F.Of(__r.RepositoryEntryCount), () => F.Of(new Procedure().RepositoryEntryCount))))); set { }
        }

        // Formula ProcedureDepartedOnlyCount (rulebook: =INDEX(Procedures!{{DepartedOnlyKnowHowCount}}, MATCH({{Procedure}}, Procedures!{{ProcedureId}}, 0)))
        [NotMapped]
        public int? ProcedureDepartedOnlyCount
        {
            get => F.AsInt(F.Memo(this, "ProcedureDepartedOnlyCount", () => F.Integer(F.Lookup<Procedure>(this, "Procedures", "ProcedureId", __c => __c.Procedures, __r => F.Of(__r.ProcedureId), F.Of(this.Procedure), __r => F.Of(__r.DepartedOnlyKnowHowCount), () => F.Of(new Procedure().DepartedOnlyKnowHowCount))))); set { }
        }

        // Formula IsStartingFromNothing (rulebook: =AND({{ProcedureRepositoryEntryCount}} = 0, {{ProcedureDepartedOnlyCount}} > 0))
        [NotMapped]
        public bool? IsStartingFromNothing
        {
            get => F.AsBool(F.Memo(this, "IsStartingFromNothing", () => F.And(F.Bool3(F.Eq(F.Of(this.ProcedureRepositoryEntryCount), F.I(0))), F.Bool3(F.Cmp(F.Of(this.ProcedureDepartedOnlyCount), ">", F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? NewStarter { get; set; }
        public string? Procedure { get; set; }
        public string? EvaluationContext { get; set; }

        private Agent _agent;

        [ForeignKey("NewStarter")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(NewStarter))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. NewStarter: " + NewStarter + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(NewStarter);
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
                        NewStarter = _agent.AgentId;
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

        private EvaluationContext _evaluationContextRef;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContextRef
        {
            get
            {
                if (_evaluationContextRef == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContextRef - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContextRef = base.SoAContext.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContextRef != null)
                    {
                        base.SoAContext.Attach(_evaluationContextRef);
                    }
                }
                return _evaluationContextRef;
            }
            set
            {
                if (_evaluationContextRef != value)
                {
                    _evaluationContextRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_evaluationContextRef != null)
                    {
                        EvaluationContext = _evaluationContextRef.EvaluationContextId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Agent;
            _ = this.ProcedureRef;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
