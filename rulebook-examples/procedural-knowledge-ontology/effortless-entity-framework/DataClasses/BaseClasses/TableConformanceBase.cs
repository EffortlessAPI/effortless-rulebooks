
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
    [Table("TableConformance")]
    public class TableConformanceBase : SoAEntityBase
    {
        [Key]
        public string TableConformanceId { get; set; }

        // Formula Name (rulebook: =CONCAT({{Substrate}}, " / ", {{RulebookTable}}))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Substrate)), F.S(" / "), F.Text(F.Of(this.RulebookTable))))); set { }
        }

        public decimal? RecordCount { get; set; }
        public decimal? DerivedFieldCount { get; set; }
        public decimal? CellsTested { get; set; }
        public decimal? CellsPassed { get; set; }
        public bool? IsMissingAnswerFile { get; set; }
        // Formula CellsFailed (rulebook: ={{CellsTested}} - {{CellsPassed}})
        [NotMapped]
        public decimal? CellsFailed
        {
            get => F.AsDecimal(F.Memo(this, "CellsFailed", () => F.Sub(F.Of(this.CellsTested), F.Of(this.CellsPassed)))); set { }
        }

        // Formula Score (rulebook: =IF({{CellsTested}} = 0, 0, ROUND(100 * {{CellsPassed}} / {{CellsTested}}, 2)))
        [NotMapped]
        public decimal? Score
        {
            get => F.AsDecimal(F.Memo(this, "Score", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.CellsTested)), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.CellsPassed)), F.Of(this.CellsTested)), F.I(2))))); set { }
        }

        // Formula IsPerfect (rulebook: ={{CellsFailed}} = 0)
        [NotMapped]
        public bool? IsPerfect
        {
            get => F.AsBool(F.Memo(this, "IsPerfect", () => F.Eq(F.Of(this.CellsFailed), F.I(0)))); set { }
        }

        // Formula ImperfectSubstrateKey (rulebook: =IF({{IsPerfect}}, "", {{Substrate}}))
        [NotMapped]
        public string? ImperfectSubstrateKey
        {
            get => F.AsString(F.Memo(this, "ImperfectSubstrateKey", () => (F.Truthy(F.Bool3(F.Of(this.IsPerfect))) ? F.S("") : F.Of(this.Substrate)))); set { }
        }

        // Formula ImperfectTableKey (rulebook: =IF({{IsPerfect}}, "", {{RulebookTable}}))
        [NotMapped]
        public string? ImperfectTableKey
        {
            get => F.AsString(F.Memo(this, "ImperfectTableKey", () => (F.Truthy(F.Bool3(F.Of(this.IsPerfect))) ? F.S("") : F.Of(this.RulebookTable)))); set { }
        }

        // Formula DisagreeingFieldCount (rulebook: =COUNTIFS(FieldDisagreements!{{TableConformance}}, {{TableConformanceId}}))
        [NotMapped]
        public decimal? DisagreeingFieldCount
        {
            get => F.AsDecimal(F.Memo(this, "DisagreeingFieldCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<FieldDisagreement>(base.SoAContext, "FieldDisagreements", __c => __c.FieldDisagreements), __r => F.CritField(F.Of(__r.TableConformance), F.Of(this.TableConformanceId)))))); set { }
        }

        // Formula SubstrateLabel (rulebook: =INDEX(ConformanceSubstrates!{{Label}}, MATCH({{Substrate}}, ConformanceSubstrates!{{ConformanceSubstrateId}}, 0)))
        [NotMapped]
        public string? SubstrateLabel
        {
            get => F.AsString(F.Memo(this, "SubstrateLabel", () => F.Lookup<ConformanceSubstrate>(this, "ConformanceSubstrates", "ConformanceSubstrateId", __c => __c.ConformanceSubstrates, __r => F.Of(__r.ConformanceSubstrateId), F.Of(this.Substrate), __r => F.Of(__r.Label), () => F.Of(new ConformanceSubstrate().Label)))); set { }
        }

        // Formula SubjectArea (rulebook: =INDEX(RulebookTables!{{SubjectArea}}, MATCH({{RulebookTable}}, RulebookTables!{{RulebookTableId}}, 0)))
        [NotMapped]
        public string? SubjectArea
        {
            get => F.AsString(F.Memo(this, "SubjectArea", () => F.Lookup<RulebookTable>(this, "RulebookTables", "RulebookTableId", __c => __c.RulebookTables, __r => F.Of(__r.RulebookTableId), F.Of(this.RulebookTable), __r => F.Of(__r.SubjectArea), () => F.Of(new RulebookTable().SubjectArea)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Run { get; set; }
        public string? Substrate { get; set; }
        public string? RulebookTable { get; set; }

        private ConformanceRun _conformanceRun;

        [ForeignKey("Run")]
        public virtual ConformanceRun ConformanceRun
        {
            get
            {
                if (_conformanceRun == null && !string.IsNullOrEmpty(Run))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConformanceRun - no database context is set. Run: " + Run + ".");
                        }
                        return null;
                    }
                    _conformanceRun = base.SoAContext.ConformanceRuns.Find(Run);
                    if (_conformanceRun != null)
                    {
                        base.SoAContext.Attach(_conformanceRun);
                    }
                }
                return _conformanceRun;
            }
            set
            {
                if (_conformanceRun != value)
                {
                    _conformanceRun = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_conformanceRun != null)
                    {
                        Run = _conformanceRun.ConformanceRunId;
                    }
                }
            }
        }

        private ConformanceSubstrate _conformanceSubstrate;

        [ForeignKey("Substrate")]
        public virtual ConformanceSubstrate ConformanceSubstrate
        {
            get
            {
                if (_conformanceSubstrate == null && !string.IsNullOrEmpty(Substrate))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConformanceSubstrate - no database context is set. Substrate: " + Substrate + ".");
                        }
                        return null;
                    }
                    _conformanceSubstrate = base.SoAContext.ConformanceSubstrates.Find(Substrate);
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
                        Substrate = _conformanceSubstrate.ConformanceSubstrateId;
                    }
                }
            }
        }

        private RulebookTable _rulebookTableRef;

        [ForeignKey("RulebookTable")]
        public virtual RulebookTable RulebookTableRef
        {
            get
            {
                if (_rulebookTableRef == null && !string.IsNullOrEmpty(RulebookTable))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTableRef - no database context is set. RulebookTable: " + RulebookTable + ".");
                        }
                        return null;
                    }
                    _rulebookTableRef = base.SoAContext.RulebookTables.Find(RulebookTable);
                    if (_rulebookTableRef != null)
                    {
                        base.SoAContext.Attach(_rulebookTableRef);
                    }
                }
                return _rulebookTableRef;
            }
            set
            {
                if (_rulebookTableRef != value)
                {
                    _rulebookTableRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookTableRef != null)
                    {
                        RulebookTable = _rulebookTableRef.RulebookTableId;
                    }
                }
            }
        }

        private ObservableCollection<FieldDisagreement> _fieldDisagreements;

        [InverseProperty("TableConformanceRef")]
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
                            throw new InvalidOperationException("Cannot access FieldDisagreements - no database context is set. TableConformanceId: " + this.TableConformanceId + ".");
                        }
                        _fieldDisagreements = new ObservableCollection<FieldDisagreement>();
                    }
                    else
                    {
                        var items = base.SoAContext.FieldDisagreements.Where(x => x.TableConformance == this.TableConformanceId).ToList<FieldDisagreement>();
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
                    item.TableConformance = this.TableConformanceId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ConformanceRun;
            _ = this.ConformanceSubstrate;
            _ = this.RulebookTableRef;
            _ = this.FieldDisagreements;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
