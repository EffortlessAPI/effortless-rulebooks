
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
    [Table("TermMeaningChanges")]
    public class TermMeaningChangeBase : SoAEntityBase
    {
        [Key]
        public string TermMeaningChangeId { get; set; }

        // Formula Name (rulebook: ={{VocabularyTerm}} & " meaning changed " & {{ChangedAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.VocabularyTerm)), F.S(" meaning changed "), F.DatetimeText(F.Of(this.ChangedAt))))); set { }
        }

        public DateTimeOffset? PriorMeaningSince { get; set; }
        public DateTimeOffset? ChangedAt { get; set; }
        public string? PriorMeaning { get; set; }
        public string? NewMeaning { get; set; }
        public bool? IsStructuralChange { get; set; }
        // Formula SpanDays (rulebook: =IF({{PriorMeaningSince}} = "", 0, DATETIME_DIFF({{ChangedAt}}, {{PriorMeaningSince}}, "days")))
        [NotMapped]
        public int? SpanDays
        {
            get => F.AsInt(F.Memo(this, "SpanDays", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.PriorMeaningSince)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.ChangedAt), F.Of(this.PriorMeaningSince), F.S("days")))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? VocabularyTerm { get; set; }
        public string? RecordedByAgent { get; set; }

        private VocabularyTerm _vocabularyTermRef;

        [ForeignKey("VocabularyTerm")]
        public virtual VocabularyTerm VocabularyTermRef
        {
            get
            {
                if (_vocabularyTermRef == null && !string.IsNullOrEmpty(VocabularyTerm))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTermRef - no database context is set. VocabularyTerm: " + VocabularyTerm + ".");
                        }
                        return null;
                    }
                    _vocabularyTermRef = base.SoAContext.VocabularyTerms.Find(VocabularyTerm);
                    if (_vocabularyTermRef != null)
                    {
                        base.SoAContext.Attach(_vocabularyTermRef);
                    }
                }
                return _vocabularyTermRef;
            }
            set
            {
                if (_vocabularyTermRef != value)
                {
                    _vocabularyTermRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabularyTermRef != null)
                    {
                        VocabularyTerm = _vocabularyTermRef.VocabularyTermId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("RecordedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(RecordedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. RecordedByAgent: " + RecordedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(RecordedByAgent);
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
                        RecordedByAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.VocabularyTermRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
