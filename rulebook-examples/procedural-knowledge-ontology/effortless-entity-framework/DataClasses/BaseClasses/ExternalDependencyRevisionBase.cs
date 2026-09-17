
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
    [Table("ExternalDependencyRevisions")]
    public class ExternalDependencyRevisionBase : SoAEntityBase
    {
        [Key]
        public string ExternalDependencyRevisionId { get; set; }

        // Formula Name (rulebook: ={{OntologyProfile}} & " " & {{RevisionLabel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.OntologyProfile)), F.S(" "), F.Text(F.Of(this.RevisionLabel))))); set { }
        }

        public string? RevisionLabel { get; set; }
        public string? RevisionKind { get; set; }
        public DateTimeOffset? PublishedAt { get; set; }
        // Formula AffectedMappingCount (rulebook: =INDEX(OntologyProfiles!{{MappingCount}}, MATCH({{OntologyProfile}}, OntologyProfiles!{{OntologyProfileId}}, 0)))
        [NotMapped]
        public int? AffectedMappingCount
        {
            get => F.AsInt(F.Memo(this, "AffectedMappingCount", () => F.Integer(F.Lookup<OntologyProfile>(this, "OntologyProfiles", "OntologyProfileId", __c => __c.OntologyProfiles, __r => F.Of(__r.OntologyProfileId), F.Of(this.OntologyProfile), __r => F.Of(__r.MappingCount), () => F.Of(new OntologyProfile().MappingCount))))); set { }
        }

        public DateTimeOffset? TrackedAt { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula DaysSincePublished (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{PublishedAt}}, "days"))
        [NotMapped]
        public int? DaysSincePublished
        {
            get => F.AsInt(F.Memo(this, "DaysSincePublished", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.PublishedAt), F.S("days"))))); set { }
        }

        // Formula IsUntrackedRevision (rulebook: =AND({{TrackedAt}} = "", {{AffectedMappingCount}} > 0, {{DaysSincePublished}} > 30))
        [NotMapped]
        public bool? IsUntrackedRevision
        {
            get => F.AsBool(F.Memo(this, "IsUntrackedRevision", () => F.And(F.Bool3(F.IsBlank(F.Of(this.TrackedAt))), F.Bool3(F.Cmp(F.Of(this.AffectedMappingCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.DaysSincePublished), ">", F.I(30)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? OntologyProfile { get; set; }
        public string? TrackedByAgent { get; set; }
        public string? EvaluationContext { get; set; }

        private OntologyProfile _ontologyProfileRef;

        [ForeignKey("OntologyProfile")]
        public virtual OntologyProfile OntologyProfileRef
        {
            get
            {
                if (_ontologyProfileRef == null && !string.IsNullOrEmpty(OntologyProfile))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OntologyProfileRef - no database context is set. OntologyProfile: " + OntologyProfile + ".");
                        }
                        return null;
                    }
                    _ontologyProfileRef = base.SoAContext.OntologyProfiles.Find(OntologyProfile);
                    if (_ontologyProfileRef != null)
                    {
                        base.SoAContext.Attach(_ontologyProfileRef);
                    }
                }
                return _ontologyProfileRef;
            }
            set
            {
                if (_ontologyProfileRef != value)
                {
                    _ontologyProfileRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_ontologyProfileRef != null)
                    {
                        OntologyProfile = _ontologyProfileRef.OntologyProfileId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("TrackedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(TrackedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. TrackedByAgent: " + TrackedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(TrackedByAgent);
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
                        TrackedByAgent = _agent.AgentId;
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
            _ = this.OntologyProfileRef;
            _ = this.Agent;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
