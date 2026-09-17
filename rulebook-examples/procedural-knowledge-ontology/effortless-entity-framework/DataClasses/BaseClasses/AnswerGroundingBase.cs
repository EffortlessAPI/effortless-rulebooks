
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
    [Table("AnswerGroundings")]
    public class AnswerGroundingBase : SoAEntityBase
    {
        [Key]
        public string AnswerGroundingId { get; set; }

        // Formula Name (rulebook: ={{AssistantAnswer}} & " <- " & {{SnapshotAssertion}} & {{RetrievalSegment}} & {{ExternalSourceUri}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.AssistantAnswer)), F.S(" <- "), F.Text(F.Of(this.SnapshotAssertion)), F.Text(F.Of(this.RetrievalSegment)), F.Text(F.Of(this.ExternalSourceUri))))); set { }
        }

        public string? ExternalSourceUri { get; set; }
        public bool? CitedToUser { get; set; }
        // Formula IsFromOwnKnowledge (rulebook: =AND({{ExternalSourceUri}} = "", OR({{SnapshotAssertion}} <> "", {{RetrievalSegment}} <> "")))
        [NotMapped]
        public bool? IsFromOwnKnowledge
        {
            get => F.AsBool(F.Memo(this, "IsFromOwnKnowledge", () => F.And(F.Bool3(F.IsBlank(F.Of(this.ExternalSourceUri))), F.Bool3(F.Or(F.Bool3(F.IsNotBlank(F.Of(this.SnapshotAssertion))), F.Bool3(F.IsNotBlank(F.Of(this.RetrievalSegment)))))))); set { }
        }

        // Formula AssertionIsStale (rulebook: =INDEX(SnapshotAssertions!{{IsStaleRoleAssertion}}, MATCH({{SnapshotAssertion}}, SnapshotAssertions!{{SnapshotAssertionId}}, 0)))
        [NotMapped]
        public bool? AssertionIsStale
        {
            get => F.AsBool(F.Memo(this, "AssertionIsStale", () => F.Lookup<SnapshotAssertion>(this, "SnapshotAssertions", "SnapshotAssertionId", __c => __c.SnapshotAssertions, __r => F.Of(__r.SnapshotAssertionId), F.Of(this.SnapshotAssertion), __r => F.Of(__r.IsStaleRoleAssertion), () => F.Of(new SnapshotAssertion().IsStaleRoleAssertion)))); set { }
        }

        // Formula AssertionPresentsDeprecated (rulebook: =INDEX(SnapshotAssertions!{{PresentsDeprecatedAsCurrent}}, MATCH({{SnapshotAssertion}}, SnapshotAssertions!{{SnapshotAssertionId}}, 0)))
        [NotMapped]
        public bool? AssertionPresentsDeprecated
        {
            get => F.AsBool(F.Memo(this, "AssertionPresentsDeprecated", () => F.Lookup<SnapshotAssertion>(this, "SnapshotAssertions", "SnapshotAssertionId", __c => __c.SnapshotAssertions, __r => F.Of(__r.SnapshotAssertionId), F.Of(this.SnapshotAssertion), __r => F.Of(__r.PresentsDeprecatedAsCurrent), () => F.Of(new SnapshotAssertion().PresentsDeprecatedAsCurrent)))); set { }
        }

        // Formula GroundsOnStaleAssertion (rulebook: =AND({{SnapshotAssertion}} <> "", OR({{AssertionIsStale}}, {{AssertionPresentsDeprecated}})))
        [NotMapped]
        public bool? GroundsOnStaleAssertion
        {
            get => F.AsBool(F.Memo(this, "GroundsOnStaleAssertion", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.SnapshotAssertion))), F.Bool3(F.Or(F.Bool3(F.Of(this.AssertionIsStale)), F.Bool3(F.Of(this.AssertionPresentsDeprecated))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? AssistantAnswer { get; set; }
        public string? SnapshotAssertion { get; set; }
        public string? RetrievalSegment { get; set; }

        private AssistantAnswer _assistantAnswerRef;

        [ForeignKey("AssistantAnswer")]
        public virtual AssistantAnswer AssistantAnswerRef
        {
            get
            {
                if (_assistantAnswerRef == null && !string.IsNullOrEmpty(AssistantAnswer))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssistantAnswerRef - no database context is set. AssistantAnswer: " + AssistantAnswer + ".");
                        }
                        return null;
                    }
                    _assistantAnswerRef = base.SoAContext.AssistantAnswers.Find(AssistantAnswer);
                    if (_assistantAnswerRef != null)
                    {
                        base.SoAContext.Attach(_assistantAnswerRef);
                    }
                }
                return _assistantAnswerRef;
            }
            set
            {
                if (_assistantAnswerRef != value)
                {
                    _assistantAnswerRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_assistantAnswerRef != null)
                    {
                        AssistantAnswer = _assistantAnswerRef.AssistantAnswerId;
                    }
                }
            }
        }

        private SnapshotAssertion _snapshotAssertionRef;

        [ForeignKey("SnapshotAssertion")]
        public virtual SnapshotAssertion SnapshotAssertionRef
        {
            get
            {
                if (_snapshotAssertionRef == null && !string.IsNullOrEmpty(SnapshotAssertion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SnapshotAssertionRef - no database context is set. SnapshotAssertion: " + SnapshotAssertion + ".");
                        }
                        return null;
                    }
                    _snapshotAssertionRef = base.SoAContext.SnapshotAssertions.Find(SnapshotAssertion);
                    if (_snapshotAssertionRef != null)
                    {
                        base.SoAContext.Attach(_snapshotAssertionRef);
                    }
                }
                return _snapshotAssertionRef;
            }
            set
            {
                if (_snapshotAssertionRef != value)
                {
                    _snapshotAssertionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_snapshotAssertionRef != null)
                    {
                        SnapshotAssertion = _snapshotAssertionRef.SnapshotAssertionId;
                    }
                }
            }
        }

        private RetrievalSegment _retrievalSegmentRef;

        [ForeignKey("RetrievalSegment")]
        public virtual RetrievalSegment RetrievalSegmentRef
        {
            get
            {
                if (_retrievalSegmentRef == null && !string.IsNullOrEmpty(RetrievalSegment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RetrievalSegmentRef - no database context is set. RetrievalSegment: " + RetrievalSegment + ".");
                        }
                        return null;
                    }
                    _retrievalSegmentRef = base.SoAContext.RetrievalSegments.Find(RetrievalSegment);
                    if (_retrievalSegmentRef != null)
                    {
                        base.SoAContext.Attach(_retrievalSegmentRef);
                    }
                }
                return _retrievalSegmentRef;
            }
            set
            {
                if (_retrievalSegmentRef != value)
                {
                    _retrievalSegmentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_retrievalSegmentRef != null)
                    {
                        RetrievalSegment = _retrievalSegmentRef.RetrievalSegmentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AssistantAnswerRef;
            _ = this.SnapshotAssertionRef;
            _ = this.RetrievalSegmentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
