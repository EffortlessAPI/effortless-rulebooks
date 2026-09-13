
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
    [Table("ConformanceSubstrates")]
    public class ConformanceSubstrateBase : SoAEntityBase
    {
        [Key]
        public string ConformanceSubstrateId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Transpiler { get; set; }
        public string? OutputFolder { get; set; }
        public string? Engine { get; set; }
        public string? HowItComputes { get; set; }
        public string? Role { get; set; }
        public decimal? SortOrder { get; set; }
        // Formula IsGraded (rulebook: ={{Role}} = "graded")
        [NotMapped]
        public bool? IsGraded
        {
            get => F.AsBool(F.Memo(this, "IsGraded", () => F.Eq(F.Nullif(F.Of(this.Role)), F.S("graded")))); set { }
        }

        // Formula RunCount (rulebook: =COUNTIFS(SubstrateRunScores!{{Substrate}}, {{ConformanceSubstrateId}}))
        [NotMapped]
        public decimal? RunCount
        {
            get => F.AsDecimal(F.Memo(this, "RunCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SubstrateRunScore>(base.SoAContext, "SubstrateRunScores", __c => __c.SubstrateRunScores), __r => F.CritField(F.Of(__r.Substrate), F.Of(this.ConformanceSubstrateId)))))); set { }
        }

        // Formula LatestCellsTested (rulebook: =SUMIFS(SubstrateRunScores!{{LatestCellsTested}}, SubstrateRunScores!{{Substrate}}, {{ConformanceSubstrateId}}))
        [NotMapped]
        public decimal? LatestCellsTested
        {
            get => F.AsDecimal(F.Memo(this, "LatestCellsTested", () => (base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<SubstrateRunScore>(base.SoAContext, "SubstrateRunScores", __c => __c.SubstrateRunScores), __r => F.CritField(F.Of(__r.Substrate), F.Of(this.ConformanceSubstrateId)), __r => F.Of(__r.LatestCellsTested), null)))); set { }
        }

        // Formula LatestCellsPassed (rulebook: =SUMIFS(SubstrateRunScores!{{LatestCellsPassed}}, SubstrateRunScores!{{Substrate}}, {{ConformanceSubstrateId}}))
        [NotMapped]
        public decimal? LatestCellsPassed
        {
            get => F.AsDecimal(F.Memo(this, "LatestCellsPassed", () => (base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<SubstrateRunScore>(base.SoAContext, "SubstrateRunScores", __c => __c.SubstrateRunScores), __r => F.CritField(F.Of(__r.Substrate), F.Of(this.ConformanceSubstrateId)), __r => F.Of(__r.LatestCellsPassed), null)))); set { }
        }

        // Formula LatestHarnessErrors (rulebook: =SUMIFS(SubstrateRunScores!{{LatestErrorFlag}}, SubstrateRunScores!{{Substrate}}, {{ConformanceSubstrateId}}))
        [NotMapped]
        public decimal? LatestHarnessErrors
        {
            get => F.AsDecimal(F.Memo(this, "LatestHarnessErrors", () => (base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<SubstrateRunScore>(base.SoAContext, "SubstrateRunScores", __c => __c.SubstrateRunScores), __r => F.CritField(F.Of(__r.Substrate), F.Of(this.ConformanceSubstrateId)), __r => F.Of(__r.LatestErrorFlag), null)))); set { }
        }

        // Formula LatestCellsFailed (rulebook: ={{LatestCellsTested}} - {{LatestCellsPassed}})
        [NotMapped]
        public decimal? LatestCellsFailed
        {
            get => F.AsDecimal(F.Memo(this, "LatestCellsFailed", () => F.Sub(F.Of(this.LatestCellsTested), F.Of(this.LatestCellsPassed)))); set { }
        }

        // Formula LatestScore (rulebook: =IF({{LatestCellsTested}} = 0, 0, ROUND(100 * {{LatestCellsPassed}} / {{LatestCellsTested}}, 2)))
        [NotMapped]
        public decimal? LatestScore
        {
            get => F.AsDecimal(F.Memo(this, "LatestScore", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.LatestCellsTested), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.LatestCellsPassed)), F.Of(this.LatestCellsTested)), F.I(2))))); set { }
        }

        // Formula DisagreeingFieldCount (rulebook: =COUNTIFS(FieldDisagreements!{{Substrate}}, {{ConformanceSubstrateId}}))
        [NotMapped]
        public decimal? DisagreeingFieldCount
        {
            get => F.AsDecimal(F.Memo(this, "DisagreeingFieldCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<FieldDisagreement>(base.SoAContext, "FieldDisagreements", __c => __c.FieldDisagreements), __r => F.CritField(F.Of(__r.Substrate), F.Of(this.ConformanceSubstrateId)))))); set { }
        }

        // Formula DisagreeingTableCount (rulebook: =COUNTIFS(TableConformance!{{ImperfectSubstrateKey}}, {{ConformanceSubstrateId}}))
        [NotMapped]
        public decimal? DisagreeingTableCount
        {
            get => F.AsDecimal(F.Memo(this, "DisagreeingTableCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TableConformance>(base.SoAContext, "TableConformance", __c => __c.TableConformance), __r => F.CritField(F.Of(__r.ImperfectSubstrateKey), F.Of(this.ConformanceSubstrateId)))))); set { }
        }

        // Formula IsFullyConformant (rulebook: =AND({{LatestCellsTested}} > 0, {{LatestCellsFailed}} = 0, {{LatestHarnessErrors}} = 0))
        [NotMapped]
        public bool? IsFullyConformant
        {
            get => F.AsBool(F.Memo(this, "IsFullyConformant", () => F.And(F.Bool3(F.Cmp(F.Of(this.LatestCellsTested), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.LatestCellsFailed), F.I(0))), F.Bool3(F.Eq(F.Of(this.LatestHarnessErrors), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<ConformanceRun> _conformanceRuns;

        [InverseProperty("ConformanceSubstrate")]
        public virtual ObservableCollection<ConformanceRun> ConformanceRuns
        {
            get
            {
                if (_conformanceRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConformanceRuns - no database context is set. ConformanceSubstrateId: " + this.ConformanceSubstrateId + ".");
                        }
                        _conformanceRuns = new ObservableCollection<ConformanceRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.ConformanceRuns.Where(x => x.AnswerKeyAuthor == this.ConformanceSubstrateId).ToList<ConformanceRun>();
                        _conformanceRuns = new ObservableCollection<ConformanceRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _conformanceRuns.CollectionChanged += ConformanceRuns_CollectionChanged;
                }
                return _conformanceRuns;
            }
            private set
            {
                if (_conformanceRuns != null)
                {
                    _conformanceRuns.CollectionChanged -= ConformanceRuns_CollectionChanged;
                }
                _conformanceRuns = value;
                if (_conformanceRuns != null)
                {
                    _conformanceRuns.CollectionChanged += ConformanceRuns_CollectionChanged;
                }
            }
        }

        private void ConformanceRuns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ConformanceRun>())
                {
                    item.AnswerKeyAuthor = this.ConformanceSubstrateId;
                }
            }
        }

        private ObservableCollection<SubstrateRunScore> _substrateRunScores;

        [InverseProperty("ConformanceSubstrate")]
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
                            throw new InvalidOperationException("Cannot access SubstrateRunScores - no database context is set. ConformanceSubstrateId: " + this.ConformanceSubstrateId + ".");
                        }
                        _substrateRunScores = new ObservableCollection<SubstrateRunScore>();
                    }
                    else
                    {
                        var items = base.SoAContext.SubstrateRunScores.Where(x => x.Substrate == this.ConformanceSubstrateId).ToList<SubstrateRunScore>();
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
                    item.Substrate = this.ConformanceSubstrateId;
                }
            }
        }

        private ObservableCollection<TableConformance> _tableConformance;

        [InverseProperty("ConformanceSubstrate")]
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
                            throw new InvalidOperationException("Cannot access TableConformance - no database context is set. ConformanceSubstrateId: " + this.ConformanceSubstrateId + ".");
                        }
                        _tableConformance = new ObservableCollection<TableConformance>();
                    }
                    else
                    {
                        var items = base.SoAContext.TableConformance.Where(x => x.Substrate == this.ConformanceSubstrateId).ToList<TableConformance>();
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
                    item.Substrate = this.ConformanceSubstrateId;
                }
            }
        }

        private ObservableCollection<FieldDisagreement> _fieldDisagreements;

        [InverseProperty("ConformanceSubstrate")]
        public virtual ObservableCollection<FieldDisagreement> FieldDisagreements
        {
            get
            {
                if (_fieldDisagreements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FieldDisagreements - no database context is set. ConformanceSubstrateId: " + this.ConformanceSubstrateId + ".");
                        }
                        _fieldDisagreements = new ObservableCollection<FieldDisagreement>();
                    }
                    else
                    {
                        var items = base.SoAContext.FieldDisagreements.Where(x => x.Substrate == this.ConformanceSubstrateId).ToList<FieldDisagreement>();
                        _fieldDisagreements = new ObservableCollection<FieldDisagreement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fieldDisagreements.CollectionChanged += FieldDisagreements_CollectionChanged;
                }
                return _fieldDisagreements;
            }
            private set
            {
                if (_fieldDisagreements != null)
                {
                    _fieldDisagreements.CollectionChanged -= FieldDisagreements_CollectionChanged;
                }
                _fieldDisagreements = value;
                if (_fieldDisagreements != null)
                {
                    _fieldDisagreements.CollectionChanged += FieldDisagreements_CollectionChanged;
                }
            }
        }

        private void FieldDisagreements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FieldDisagreement>())
                {
                    item.Substrate = this.ConformanceSubstrateId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ConformanceRuns;
            _ = this.SubstrateRunScores;
            _ = this.TableConformance;
            _ = this.FieldDisagreements;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
