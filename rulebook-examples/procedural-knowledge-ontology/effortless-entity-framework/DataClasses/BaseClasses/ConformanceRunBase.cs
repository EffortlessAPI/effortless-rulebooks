
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
    [Table("ConformanceRuns")]
    public class ConformanceRunBase : SoAEntityBase
    {
        [Key]
        public string ConformanceRunId { get; set; }

        // Formula Name (rulebook: ={{ConformanceRunId}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.ConformanceRunId))); set { }
        }

        public DateTimeOffset? RanOn { get; set; }
        public string? RulebookCommit { get; set; }
        public bool? IsLatest { get; set; }
        public string? Notes { get; set; }
        // Formula SubstrateCount (rulebook: =COUNTIFS(SubstrateRunScores!{{Run}}, {{ConformanceRunId}}))
        [NotMapped]
        public decimal? SubstrateCount
        {
            get => F.AsDecimal(F.Memo(this, "SubstrateCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SubstrateRunScore>(base.SoAContext, "SubstrateRunScores", __c => __c.SubstrateRunScores), __r => F.CritField(F.Of(__r.Run), F.Of(this.ConformanceRunId)))))); set { }
        }

        // Formula PerfectSubstrateCount (rulebook: =COUNTIFS(SubstrateRunScores!{{PerfectRunKey}}, {{ConformanceRunId}}))
        [NotMapped]
        public decimal? PerfectSubstrateCount
        {
            get => F.AsDecimal(F.Memo(this, "PerfectSubstrateCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SubstrateRunScore>(base.SoAContext, "SubstrateRunScores", __c => __c.SubstrateRunScores), __r => F.CritField(F.Of(__r.PerfectRunKey), F.Of(this.ConformanceRunId)))))); set { }
        }

        // Formula CellsTested (rulebook: =SUMIFS(SubstrateRunScores!{{CellsTested}}, SubstrateRunScores!{{Run}}, {{ConformanceRunId}}))
        [NotMapped]
        public decimal? CellsTested
        {
            get => F.AsDecimal(F.Memo(this, "CellsTested", () => (base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<SubstrateRunScore>(base.SoAContext, "SubstrateRunScores", __c => __c.SubstrateRunScores), __r => F.CritField(F.Of(__r.Run), F.Of(this.ConformanceRunId)), __r => F.Of(__r.CellsTested), null)))); set { }
        }

        // Formula CellsPassed (rulebook: =SUMIFS(SubstrateRunScores!{{CellsPassed}}, SubstrateRunScores!{{Run}}, {{ConformanceRunId}}))
        [NotMapped]
        public decimal? CellsPassed
        {
            get => F.AsDecimal(F.Memo(this, "CellsPassed", () => (base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<SubstrateRunScore>(base.SoAContext, "SubstrateRunScores", __c => __c.SubstrateRunScores), __r => F.CritField(F.Of(__r.Run), F.Of(this.ConformanceRunId)), __r => F.Of(__r.CellsPassed), null)))); set { }
        }

        // Formula CellsFailed (rulebook: ={{CellsTested}} - {{CellsPassed}})
        [NotMapped]
        public decimal? CellsFailed
        {
            get => F.AsDecimal(F.Memo(this, "CellsFailed", () => F.Sub(F.Of(this.CellsTested), F.Of(this.CellsPassed)))); set { }
        }

        // Formula OverallScore (rulebook: =IF({{CellsTested}} = 0, 0, ROUND(100 * {{CellsPassed}} / {{CellsTested}}, 2)))
        [NotMapped]
        public decimal? OverallScore
        {
            get => F.AsDecimal(F.Memo(this, "OverallScore", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.CellsTested), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.CellsPassed)), F.Of(this.CellsTested)), F.I(2))))); set { }
        }

        // Formula ImperfectSubstrateCount (rulebook: ={{SubstrateCount}} - {{PerfectSubstrateCount}})
        [NotMapped]
        public decimal? ImperfectSubstrateCount
        {
            get => F.AsDecimal(F.Memo(this, "ImperfectSubstrateCount", () => F.Sub(F.Of(this.SubstrateCount), F.Of(this.PerfectSubstrateCount)))); set { }
        }

        // Formula IsFullyConformant (rulebook: =AND({{SubstrateCount}} > 0, {{ImperfectSubstrateCount}} = 0))
        [NotMapped]
        public bool? IsFullyConformant
        {
            get => F.AsBool(F.Memo(this, "IsFullyConformant", () => F.And(F.Bool3(F.Cmp(F.Of(this.SubstrateCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.ImperfectSubstrateCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? AnswerKeyAuthor { get; set; }

        private ConformanceSubstrate _conformanceSubstrate;

        [ForeignKey("AnswerKeyAuthor")]
        public virtual ConformanceSubstrate ConformanceSubstrate
        {
            get
            {
                if (_conformanceSubstrate == null && !string.IsNullOrEmpty(AnswerKeyAuthor))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConformanceSubstrate - no database context is set. AnswerKeyAuthor: " + AnswerKeyAuthor + ".");
                        }
                        return null;
                    }
                    _conformanceSubstrate = base.SoAContext.ConformanceSubstrates.Find(AnswerKeyAuthor);
                    if (_conformanceSubstrate != null)
                    {
                        base.SoAContext.Attach(_conformanceSubstrate);
                    }
                }
                return _conformanceSubstrate;
            }
            set
            {
                if (_conformanceSubstrate != value)
                {
                    _conformanceSubstrate = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_conformanceSubstrate != null)
                    {
                        AnswerKeyAuthor = _conformanceSubstrate.ConformanceSubstrateId;
                    }
                }
            }
        }

        private ObservableCollection<SubstrateRunScore> _substrateRunScores;

        [InverseProperty("ConformanceRun")]
        public virtual ObservableCollection<SubstrateRunScore> SubstrateRunScores
        {
            get
            {
                if (_substrateRunScores == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SubstrateRunScores - no database context is set. ConformanceRunId: " + this.ConformanceRunId + ".");
                        }
                        _substrateRunScores = new ObservableCollection<SubstrateRunScore>();
                    }
                    else
                    {
                        var items = base.SoAContext.SubstrateRunScores.Where(x => x.Run == this.ConformanceRunId).ToList<SubstrateRunScore>();
                        _substrateRunScores = new ObservableCollection<SubstrateRunScore>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _substrateRunScores.CollectionChanged += SubstrateRunScores_CollectionChanged;
                }
                return _substrateRunScores;
            }
            private set
            {
                if (_substrateRunScores != null)
                {
                    _substrateRunScores.CollectionChanged -= SubstrateRunScores_CollectionChanged;
                }
                _substrateRunScores = value;
                if (_substrateRunScores != null)
                {
                    _substrateRunScores.CollectionChanged += SubstrateRunScores_CollectionChanged;
                }
            }
        }

        private void SubstrateRunScores_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SubstrateRunScore>())
                {
                    item.Run = this.ConformanceRunId;
                }
            }
        }

        private ObservableCollection<TableConformance> _tableConformance;

        [InverseProperty("ConformanceRun")]
        public virtual ObservableCollection<TableConformance> TableConformance
        {
            get
            {
                if (_tableConformance == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TableConformance - no database context is set. ConformanceRunId: " + this.ConformanceRunId + ".");
                        }
                        _tableConformance = new ObservableCollection<TableConformance>();
                    }
                    else
                    {
                        var items = base.SoAContext.TableConformance.Where(x => x.Run == this.ConformanceRunId).ToList<TableConformance>();
                        _tableConformance = new ObservableCollection<TableConformance>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _tableConformance.CollectionChanged += TableConformance_CollectionChanged;
                }
                return _tableConformance;
            }
            private set
            {
                if (_tableConformance != null)
                {
                    _tableConformance.CollectionChanged -= TableConformance_CollectionChanged;
                }
                _tableConformance = value;
                if (_tableConformance != null)
                {
                    _tableConformance.CollectionChanged += TableConformance_CollectionChanged;
                }
            }
        }

        private void TableConformance_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TableConformance>())
                {
                    item.Run = this.ConformanceRunId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ConformanceSubstrate;
            _ = this.SubstrateRunScores;
            _ = this.TableConformance;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
