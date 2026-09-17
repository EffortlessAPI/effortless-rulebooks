
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
    [Table("KnowledgeSearchEvents")]
    public class KnowledgeSearchEventBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeSearchEventId { get; set; }

        // Formula Name (rulebook: ={{SearchedByAgent}} & ": " & LEFT({{QueryText}}, 40))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.SearchedByAgent)), F.S(": "), F.Text(F.Left(F.Of(this.QueryText), F.I(40)))))); set { }
        }

        public DateTimeOffset? SearchedAt { get; set; }
        public string? Channel { get; set; }
        public string? QueryText { get; set; }
        public int? ResultCount { get; set; }
        public bool? WasAbandoned { get; set; }
        public decimal? SecondsBeforeAbandoning { get; set; }
        public decimal? DwellSeconds { get; set; }
        public string? LinkedUsabilityBarrier { get; set; }
        // Formula FoundNothingUseful (rulebook: =OR({{ResultCount}} = 0, AND({{OpenedSegment}} = "", {{OpenedProjection}} = "")))
        [NotMapped]
        public bool? FoundNothingUseful
        {
            get => F.AsBool(F.Memo(this, "FoundNothingUseful", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.ResultCount)), F.I(0))), F.Bool3(F.And(F.Bool3(F.IsBlank(F.Of(this.OpenedSegment))), F.Bool3(F.IsBlank(F.Of(this.OpenedProjection)))))))); set { }
        }

        // Formula GaveUpAfterSeeingResults (rulebook: =AND({{WasAbandoned}}, {{ResultCount}} > 0))
        [NotMapped]
        public bool? GaveUpAfterSeeingResults
        {
            get => F.AsBool(F.Memo(this, "GaveUpAfterSeeingResults", () => F.And(F.IsTrueV(F.Of(this.WasAbandoned)), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ResultCount)), ">", F.I(0)))))); set { }
        }

        // Formula IsUnlinkedFailedSearch (rulebook: =AND({{FoundNothingUseful}}, {{LinkedKnowledgeGap}} = "", {{LinkedUsabilityBarrier}} = ""))
        [NotMapped]
        public bool? IsUnlinkedFailedSearch
        {
            get => F.AsBool(F.Memo(this, "IsUnlinkedFailedSearch", () => F.And(F.Bool3(F.Of(this.FoundNothingUseful)), F.Bool3(F.IsBlank(F.Of(this.LinkedKnowledgeGap))), F.Bool3(F.IsBlank(F.Of(this.LinkedUsabilityBarrier)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? SearchedByAgent { get; set; }
        public string? SoughtProcedureVersion { get; set; }
        public string? OpenedSegment { get; set; }
        public string? OpenedProjection { get; set; }
        public string? LinkedKnowledgeGap { get; set; }

        private Agent _agent;

        [ForeignKey("SearchedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(SearchedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. SearchedByAgent: " + SearchedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(SearchedByAgent);
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
                        SearchedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("SoughtProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(SoughtProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. SoughtProcedureVersion: " + SoughtProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = base.SoAContext.ProcedureVersions.Find(SoughtProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        base.SoAContext.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersion != null)
                    {
                        SoughtProcedureVersion = _procedureVersion.ProcedureVersionId;
                    }
                }
            }
        }

        private RetrievalSegment _retrievalSegment;

        [ForeignKey("OpenedSegment")]
        public virtual RetrievalSegment RetrievalSegment
        {
            get
            {
                if (_retrievalSegment == null && !string.IsNullOrEmpty(OpenedSegment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RetrievalSegment - no database context is set. OpenedSegment: " + OpenedSegment + ".");
                        }
                        return null;
                    }
                    _retrievalSegment = base.SoAContext.RetrievalSegments.Find(OpenedSegment);
                    if (_retrievalSegment != null)
                    {
                        base.SoAContext.Attach(_retrievalSegment);
                    }
                }
                return _retrievalSegment;
            }
            set
            {
                if (_retrievalSegment != value)
                {
                    _retrievalSegment = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_retrievalSegment != null)
                    {
                        OpenedSegment = _retrievalSegment.RetrievalSegmentId;
                    }
                }
            }
        }

        private KnowledgeProjection _knowledgeProjection;

        [ForeignKey("OpenedProjection")]
        public virtual KnowledgeProjection KnowledgeProjection
        {
            get
            {
                if (_knowledgeProjection == null && !string.IsNullOrEmpty(OpenedProjection))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeProjection - no database context is set. OpenedProjection: " + OpenedProjection + ".");
                        }
                        return null;
                    }
                    _knowledgeProjection = base.SoAContext.KnowledgeProjections.Find(OpenedProjection);
                    if (_knowledgeProjection != null)
                    {
                        base.SoAContext.Attach(_knowledgeProjection);
                    }
                }
                return _knowledgeProjection;
            }
            set
            {
                if (_knowledgeProjection != value)
                {
                    _knowledgeProjection = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeProjection != null)
                    {
                        OpenedProjection = _knowledgeProjection.KnowledgeProjectionId;
                    }
                }
            }
        }

        private KnowledgeGap _knowledgeGap;

        [ForeignKey("LinkedKnowledgeGap")]
        public virtual KnowledgeGap KnowledgeGap
        {
            get
            {
                if (_knowledgeGap == null && !string.IsNullOrEmpty(LinkedKnowledgeGap))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeGap - no database context is set. LinkedKnowledgeGap: " + LinkedKnowledgeGap + ".");
                        }
                        return null;
                    }
                    _knowledgeGap = base.SoAContext.KnowledgeGaps.Find(LinkedKnowledgeGap);
                    if (_knowledgeGap != null)
                    {
                        base.SoAContext.Attach(_knowledgeGap);
                    }
                }
                return _knowledgeGap;
            }
            set
            {
                if (_knowledgeGap != value)
                {
                    _knowledgeGap = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeGap != null)
                    {
                        LinkedKnowledgeGap = _knowledgeGap.KnowledgeGapId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Agent;
            _ = this.ProcedureVersion;
            _ = this.RetrievalSegment;
            _ = this.KnowledgeProjection;
            _ = this.KnowledgeGap;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
