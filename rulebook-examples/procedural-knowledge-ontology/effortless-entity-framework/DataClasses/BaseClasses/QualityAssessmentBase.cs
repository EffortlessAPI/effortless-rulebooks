
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
    [Table("QualityAssessments")]
    public class QualityAssessmentBase : SoAEntityBase
    {
        [Key]
        public string QualityAssessmentId { get; set; }

        // Formula Name (rulebook: ={{RulebookRelease}} & " / " & {{QualityCriterion}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.RulebookRelease)), F.S(" / "), F.Text(F.Of(this.QualityCriterion))))); set { }
        }

        public int? Score { get; set; }
        public string? Method { get; set; }
        public DateTimeOffset? AssessedAt { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? RulebookRelease { get; set; }
        public string? QualityCriterion { get; set; }
        public string? AssessedByAgent { get; set; }

        private RulebookRelease _rulebookReleaseRef;

        [ForeignKey("RulebookRelease")]
        public virtual RulebookRelease RulebookReleaseRef
        {
            get
            {
                if (_rulebookReleaseRef == null && !string.IsNullOrEmpty(RulebookRelease))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookReleaseRef - no database context is set. RulebookRelease: " + RulebookRelease + ".");
                        }
                        return null;
                    }
                    _rulebookReleaseRef = base.SoAContext.RulebookReleases.Find(RulebookRelease);
                    if (_rulebookReleaseRef != null)
                    {
                        base.SoAContext.Attach(_rulebookReleaseRef);
                    }
                }
                return _rulebookReleaseRef;
            }
            set
            {
                if (_rulebookReleaseRef != value)
                {
                    _rulebookReleaseRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookReleaseRef != null)
                    {
                        RulebookRelease = _rulebookReleaseRef.RulebookReleaseId;
                    }
                }
            }
        }

        private QualityCriteria _qualityCriteria;

        [ForeignKey("QualityCriterion")]
        public virtual QualityCriteria QualityCriteria
        {
            get
            {
                if (_qualityCriteria == null && !string.IsNullOrEmpty(QualityCriterion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access QualityCriteria - no database context is set. QualityCriterion: " + QualityCriterion + ".");
                        }
                        return null;
                    }
                    _qualityCriteria = base.SoAContext.QualityCriteria.Find(QualityCriterion);
                    if (_qualityCriteria != null)
                    {
                        base.SoAContext.Attach(_qualityCriteria);
                    }
                }
                return _qualityCriteria;
            }
            set
            {
                if (_qualityCriteria != value)
                {
                    _qualityCriteria = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_qualityCriteria != null)
                    {
                        QualityCriterion = _qualityCriteria.QualityCriterionId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("AssessedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(AssessedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. AssessedByAgent: " + AssessedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(AssessedByAgent);
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
                        AssessedByAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RulebookReleaseRef;
            _ = this.QualityCriteria;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
