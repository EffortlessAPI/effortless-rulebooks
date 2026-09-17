
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
    [Table("AgentUpgradeAssessments")]
    public class AgentUpgradeAssessmentBase : SoAEntityBase
    {
        [Key]
        public string AgentUpgradeAssessmentId { get; set; }

        // Formula Name (rulebook: ={{CurrentAgent}} & " -> " & {{CandidateAgent}} & " (" & {{AssessmentMethod}} & ")")
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.CurrentAgent)), F.S(" -> "), F.Text(F.Of(this.CandidateAgent)), F.S(" ("), F.Text(F.Of(this.AssessmentMethod)), F.S(")")))); set { }
        }

        public DateTimeOffset? AssessedAt { get; set; }
        public string? AssessmentMethod { get; set; }
        public int? ListedAffectedStepCount { get; set; }
        public string? ListedAffectedStepsNote { get; set; }
        // Formula AttributedArtifactCount (rulebook: =INDEX(Agents!{{AttributedArtifactCount}}, MATCH({{CurrentAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public int? AttributedArtifactCount
        {
            get => F.AsInt(F.Memo(this, "AttributedArtifactCount", () => F.Integer(F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.CurrentAgent), __r => F.Of(__r.AttributedArtifactCount), () => F.Of(new Agent().AttributedArtifactCount))))); set { }
        }

        // Formula TraversedDownstreamStepCount (rulebook: =INDEX(Agents!{{ArtifactBlastRadiusStepCount}}, MATCH({{CurrentAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public int? TraversedDownstreamStepCount
        {
            get => F.AsInt(F.Memo(this, "TraversedDownstreamStepCount", () => F.Integer(F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.CurrentAgent), __r => F.Of(__r.ArtifactBlastRadiusStepCount), () => F.Of(new Agent().ArtifactBlastRadiusStepCount))))); set { }
        }

        // Formula MissedTraversedImpact (rulebook: ={{ListedAffectedStepCount}} < {{TraversedDownstreamStepCount}})
        [NotMapped]
        public bool? MissedTraversedImpact
        {
            get => F.AsBool(F.Memo(this, "MissedTraversedImpact", () => F.Cmp(F.Nullif(F.Of(this.ListedAffectedStepCount)), "<", F.Of(this.TraversedDownstreamStepCount)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? CurrentAgent { get; set; }
        public string? CandidateAgent { get; set; }
        public string? AssessedByAgent { get; set; }

        private Agent _agent;

        [ForeignKey("CurrentAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(CurrentAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. CurrentAgent: " + CurrentAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(CurrentAgent);
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
                        CurrentAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("CandidateAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(CandidateAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. CandidateAgent: " + CandidateAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(CandidateAgent);
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
                        CandidateAgent = _agentRef.AgentId;
                    }
                }
            }
        }

        private Agent _agentRefRef;

        [ForeignKey("AssessedByAgent")]
        public virtual Agent AgentRefRef
        {
            get
            {
                if (_agentRefRef == null && !string.IsNullOrEmpty(AssessedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRefRef - no database context is set. AssessedByAgent: " + AssessedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRefRef = base.SoAContext.Agents.Find(AssessedByAgent);
                    if (_agentRefRef != null)
                    {
                        base.SoAContext.Attach(_agentRefRef);
                    }
                }
                return _agentRefRef;
            }
            set
            {
                if (_agentRefRef != value)
                {
                    _agentRefRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRefRef != null)
                    {
                        AssessedByAgent = _agentRefRef.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.AgentRefRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
