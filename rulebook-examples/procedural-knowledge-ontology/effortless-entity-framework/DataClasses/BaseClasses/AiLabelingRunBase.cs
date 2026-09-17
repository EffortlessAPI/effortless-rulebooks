
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
    [Table("AiLabelingRuns")]
    public class AiLabelingRunBase : SoAEntityBase
    {
        [Key]
        public string AiLabelingRunId { get; set; }

        // Formula Name (rulebook: ={{Agent}} & " " & {{RunAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Agent)), F.S(" "), F.DatetimeText(F.Of(this.RunAt))))); set { }
        }

        public DateTimeOffset? RunAt { get; set; }
        public string? TaskDescription { get; set; }
        // Formula OutputCount (rulebook: =COUNTIFS(SourceTermMentions!{{AiLabelingRun}}, {{AiLabelingRunId}}))
        [NotMapped]
        public int? OutputCount
        {
            get => F.AsInt(F.Memo(this, "OutputCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SourceTermMention>(base.SoAContext, "SourceTermMentions", __c => __c.SourceTermMentions), __r => F.CritField(F.Of(__r.AiLabelingRun), F.Of(this.AiLabelingRunId))))))); set { }
        }

        // Formula NonCanonicalOutputCount (rulebook: =COUNTIFS(SourceTermMentions!{{AiLabelingRun}}, {{AiLabelingRunId}}, SourceTermMentions!{{IsNonCanonicalGeneratedValue}}, TRUE))
        [NotMapped]
        public int? NonCanonicalOutputCount
        {
            get => F.AsInt(F.Memo(this, "NonCanonicalOutputCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SourceTermMention>(base.SoAContext, "SourceTermMentions", __c => __c.SourceTermMentions), __r => F.CritField(F.Of(__r.AiLabelingRun), F.Of(this.AiLabelingRunId)) && F.CritLiteral(F.Of(__r.IsNonCanonicalGeneratedValue), F.B(true))))))); set { }
        }

        // Formula GroundingSchemeIsMachineAccessible (rulebook: =INDEX(Vocabularies!{{IsMachineAccessible}}, MATCH({{GroundingScheme}}, Vocabularies!{{VocabularyId}}, 0)))
        [NotMapped]
        public bool? GroundingSchemeIsMachineAccessible
        {
            get => F.AsBool(F.Memo(this, "GroundingSchemeIsMachineAccessible", () => F.Lookup<Vocabulary>(this, "Vocabularies", "VocabularyId", __c => __c.Vocabularies, __r => F.Of(__r.VocabularyId), F.Of(this.GroundingScheme), __r => F.Of(__r.IsMachineAccessible), () => F.Of(new Vocabulary().IsMachineAccessible)))); set { }
        }

        // Formula IsUngroundedSynonymSprawl (rulebook: =AND({{GroundingScheme}} = "", {{NonCanonicalOutputCount}} > 0))
        [NotMapped]
        public bool? IsUngroundedSynonymSprawl
        {
            get => F.AsBool(F.Memo(this, "IsUngroundedSynonymSprawl", () => F.And(F.Bool3(F.IsBlank(F.Of(this.GroundingScheme))), F.Bool3(F.Cmp(F.Of(this.NonCanonicalOutputCount), ">", F.I(0)))))); set { }
        }

        // Formula GroundedInNonMachineReadableScheme (rulebook: =AND({{GroundingScheme}} <> "", {{GroundingSchemeIsMachineAccessible}} = FALSE))
        [NotMapped]
        public bool? GroundedInNonMachineReadableScheme
        {
            get => F.AsBool(F.Memo(this, "GroundedInNonMachineReadableScheme", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.GroundingScheme))), F.Bool3(F.Eq(F.Of(this.GroundingSchemeIsMachineAccessible), F.B(false)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Agent { get; set; }
        public string? GroundingScheme { get; set; }

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

        private Vocabulary _vocabulary;

        [ForeignKey("GroundingScheme")]
        public virtual Vocabulary Vocabulary
        {
            get
            {
                if (_vocabulary == null && !string.IsNullOrEmpty(GroundingScheme))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Vocabulary - no database context is set. GroundingScheme: " + GroundingScheme + ".");
                        }
                        return null;
                    }
                    _vocabulary = base.SoAContext.Vocabularies.Find(GroundingScheme);
                    if (_vocabulary != null)
                    {
                        base.SoAContext.Attach(_vocabulary);
                    }
                }
                return _vocabulary;
            }
            set
            {
                if (_vocabulary != value)
                {
                    _vocabulary = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabulary != null)
                    {
                        GroundingScheme = _vocabulary.VocabularyId;
                    }
                }
            }
        }

        private ObservableCollection<SourceTermMention> _sourceTermMentions;

        [InverseProperty("AiLabelingRunRef")]
        public virtual ObservableCollection<SourceTermMention> SourceTermMentions
        {
            get
            {
                if (_sourceTermMentions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SourceTermMentions - no database context is set. AiLabelingRunId: " + this.AiLabelingRunId + ".");
                        }
                        _sourceTermMentions = new ObservableCollection<SourceTermMention>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourceTermMentions.Where(x => x.AiLabelingRun == this.AiLabelingRunId).ToList<SourceTermMention>();
                        _sourceTermMentions = new ObservableCollection<SourceTermMention>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _sourceTermMentions.CollectionChanged += SourceTermMentions_CollectionChanged;
                }
                return _sourceTermMentions;
            }
            private set
            {
                if (_sourceTermMentions != null)
                {
                    _sourceTermMentions.CollectionChanged -= SourceTermMentions_CollectionChanged;
                }
                _sourceTermMentions = value;
                if (_sourceTermMentions != null)
                {
                    _sourceTermMentions.CollectionChanged += SourceTermMentions_CollectionChanged;
                }
            }
        }

        private void SourceTermMentions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SourceTermMention>())
                {
                    item.AiLabelingRun = this.AiLabelingRunId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AgentRef;
            _ = this.Vocabulary;
            _ = this.SourceTermMentions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
