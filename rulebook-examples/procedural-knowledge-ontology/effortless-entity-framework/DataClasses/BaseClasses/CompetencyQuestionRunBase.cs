
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
    [Table("CompetencyQuestionRuns")]
    public class CompetencyQuestionRunBase : SoAEntityBase
    {
        [Key]
        public string CompetencyQuestionRunId { get; set; }

        // Formula Name (rulebook: ={{CqSetEntry}} & " @ " & {{RulebookRelease}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.CqSetEntry)), F.S(" @ "), F.Text(F.Of(this.RulebookRelease))))); set { }
        }

        public DateTimeOffset? RanAt { get; set; }
        public bool? WasAnswerable { get; set; }
        public string? AnswerOutcome { get; set; }
        // Formula EntryIsOriginal (rulebook: =INDEX(CompetencyQuestionSetEntries!{{IsOriginalBaseline}}, MATCH({{CqSetEntry}}, CompetencyQuestionSetEntries!{{CompetencyQuestionSetEntryId}}, 0)))
        [NotMapped]
        public bool? EntryIsOriginal
        {
            get => F.AsBool(F.Memo(this, "EntryIsOriginal", () => F.Lookup<CompetencyQuestionSetEntry>(this, "CompetencyQuestionSetEntries", "CompetencyQuestionSetEntryId", __c => __c.CompetencyQuestionSetEntries, __r => F.Of(__r.CompetencyQuestionSetEntryId), F.Of(this.CqSetEntry), __r => F.Of(__r.IsOriginalBaseline), () => F.Of(new CompetencyQuestionSetEntry().IsOriginalBaseline)))); set { }
        }

        // Formula PriorWasAnswerable (rulebook: =INDEX(CompetencyQuestionRuns!{{WasAnswerable}}, MATCH({{PriorRun}}, CompetencyQuestionRuns!{{CompetencyQuestionRunId}}, 0)))
        [NotMapped]
        public bool? PriorWasAnswerable
        {
            get => F.AsBool(F.Memo(this, "PriorWasAnswerable", () => F.Lookup<CompetencyQuestionRun>(this, "CompetencyQuestionRuns", "CompetencyQuestionRunId", __c => __c.CompetencyQuestionRuns, __r => F.Of(__r.CompetencyQuestionRunId), F.Of(this.PriorRun), __r => F.Of(__r.WasAnswerable), () => F.Of(new CompetencyQuestionRun().WasAnswerable)))); set { }
        }

        // Formula IsBaselineRegression (rulebook: =AND({{EntryIsOriginal}}, {{PriorRun}} <> "", {{PriorWasAnswerable}}, {{WasAnswerable}} = FALSE))
        [NotMapped]
        public bool? IsBaselineRegression
        {
            get => F.AsBool(F.Memo(this, "IsBaselineRegression", () => F.And(F.Bool3(F.Of(this.EntryIsOriginal)), F.Bool3(F.IsNotBlank(F.Of(this.PriorRun))), F.Bool3(F.Of(this.PriorWasAnswerable)), F.Bool3(F.Eq(F.Nullif(F.Of(this.WasAnswerable)), F.B(false)))))); set { }
        }

        // Formula IsUnfixedWrongAnswer (rulebook: =AND({{AnswerOutcome}} = "Wrong", {{DefectChangeRequest}} = ""))
        [NotMapped]
        public bool? IsUnfixedWrongAnswer
        {
            get => F.AsBool(F.Memo(this, "IsUnfixedWrongAnswer", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AnswerOutcome)), F.S("Wrong"))), F.Bool3(F.IsBlank(F.Of(this.DefectChangeRequest)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? CqSetEntry { get; set; }
        public string? RulebookRelease { get; set; }
        public string? PriorRun { get; set; }
        public string? DefectChangeRequest { get; set; }

        private CompetencyQuestionSetEntry _competencyQuestionSetEntry;

        [ForeignKey("CqSetEntry")]
        public virtual CompetencyQuestionSetEntry CompetencyQuestionSetEntry
        {
            get
            {
                if (_competencyQuestionSetEntry == null && !string.IsNullOrEmpty(CqSetEntry))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CompetencyQuestionSetEntry - no database context is set. CqSetEntry: " + CqSetEntry + ".");
                        }
                        return null;
                    }
                    _competencyQuestionSetEntry = base.SoAContext.CompetencyQuestionSetEntries.Find(CqSetEntry);
                    if (_competencyQuestionSetEntry != null)
                    {
                        base.SoAContext.Attach(_competencyQuestionSetEntry);
                    }
                }
                return _competencyQuestionSetEntry;
            }
            set
            {
                if (_competencyQuestionSetEntry != value)
                {
                    _competencyQuestionSetEntry = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_competencyQuestionSetEntry != null)
                    {
                        CqSetEntry = _competencyQuestionSetEntry.CompetencyQuestionSetEntryId;
                    }
                }
            }
        }

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

        private CompetencyQuestionRun _competencyQuestionRun;

        [ForeignKey("PriorRun")]
        public virtual CompetencyQuestionRun CompetencyQuestionRun
        {
            get
            {
                if (_competencyQuestionRun == null && !string.IsNullOrEmpty(PriorRun))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CompetencyQuestionRun - no database context is set. PriorRun: " + PriorRun + ".");
                        }
                        return null;
                    }
                    _competencyQuestionRun = base.SoAContext.CompetencyQuestionRuns.Find(PriorRun);
                    if (_competencyQuestionRun != null)
                    {
                        base.SoAContext.Attach(_competencyQuestionRun);
                    }
                }
                return _competencyQuestionRun;
            }
            set
            {
                if (_competencyQuestionRun != value)
                {
                    _competencyQuestionRun = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_competencyQuestionRun != null)
                    {
                        PriorRun = _competencyQuestionRun.CompetencyQuestionRunId;
                    }
                }
            }
        }

        private ModelChangeRequest _modelChangeRequest;

        [ForeignKey("DefectChangeRequest")]
        public virtual ModelChangeRequest ModelChangeRequest
        {
            get
            {
                if (_modelChangeRequest == null && !string.IsNullOrEmpty(DefectChangeRequest))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelChangeRequest - no database context is set. DefectChangeRequest: " + DefectChangeRequest + ".");
                        }
                        return null;
                    }
                    _modelChangeRequest = base.SoAContext.ModelChangeRequests.Find(DefectChangeRequest);
                    if (_modelChangeRequest != null)
                    {
                        base.SoAContext.Attach(_modelChangeRequest);
                    }
                }
                return _modelChangeRequest;
            }
            set
            {
                if (_modelChangeRequest != value)
                {
                    _modelChangeRequest = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_modelChangeRequest != null)
                    {
                        DefectChangeRequest = _modelChangeRequest.ModelChangeRequestId;
                    }
                }
            }
        }

        private ObservableCollection<CompetencyQuestionRun> _competencyQuestionRuns;

        [InverseProperty("CompetencyQuestionRun")]
        public virtual ObservableCollection<CompetencyQuestionRun> CompetencyQuestionRuns
        {
            get
            {
                if (_competencyQuestionRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CompetencyQuestionRuns - no database context is set. CompetencyQuestionRunId: " + this.CompetencyQuestionRunId + ".");
                        }
                        _competencyQuestionRuns = new ObservableCollection<CompetencyQuestionRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.CompetencyQuestionRuns.Where(x => x.PriorRun == this.CompetencyQuestionRunId).ToList<CompetencyQuestionRun>();
                        _competencyQuestionRuns = new ObservableCollection<CompetencyQuestionRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _competencyQuestionRuns.CollectionChanged += CompetencyQuestionRuns_CollectionChanged;
                }
                return _competencyQuestionRuns;
            }
            private set
            {
                if (_competencyQuestionRuns != null)
                {
                    _competencyQuestionRuns.CollectionChanged -= CompetencyQuestionRuns_CollectionChanged;
                }
                _competencyQuestionRuns = value;
                if (_competencyQuestionRuns != null)
                {
                    _competencyQuestionRuns.CollectionChanged += CompetencyQuestionRuns_CollectionChanged;
                }
            }
        }

        private void CompetencyQuestionRuns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CompetencyQuestionRun>())
                {
                    item.PriorRun = this.CompetencyQuestionRunId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.CompetencyQuestionSetEntry;
            _ = this.RulebookReleaseRef;
            _ = this.CompetencyQuestionRun;
            _ = this.ModelChangeRequest;
            _ = this.CompetencyQuestionRuns;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
