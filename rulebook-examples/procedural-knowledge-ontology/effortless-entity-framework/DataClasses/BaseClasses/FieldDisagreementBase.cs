
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
    [Table("FieldDisagreements")]
    public class FieldDisagreementBase : SoAEntityBase
    {
        [Key]
        public string FieldDisagreementId { get; set; }

        // Formula Name (rulebook: =CONCAT({{Substrate}}, " / ", {{RulebookField}}))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Substrate)), F.S(" / "), F.Text(F.Of(this.RulebookField))))); set { }
        }

        public string? FieldClass { get; set; }
        public decimal? CellsFailed { get; set; }
        public string? DominantReason { get; set; }
        // Formula SampledCellCount (rulebook: =COUNTIFS(CellDisagreements!{{FieldDisagreement}}, {{FieldDisagreementId}}))
        [NotMapped]
        public decimal? SampledCellCount
        {
            get => F.AsDecimal(F.Memo(this, "SampledCellCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CellDisagreement>(base.SoAContext, "CellDisagreements", __c => __c.CellDisagreements), __r => F.CritField(F.Of(__r.FieldDisagreement), F.Of(this.FieldDisagreementId)))))); set { }
        }

        // Formula IsFullySampled (rulebook: ={{SampledCellCount}} = {{CellsFailed}})
        [NotMapped]
        public bool? IsFullySampled
        {
            get => F.AsBool(F.Memo(this, "IsFullySampled", () => F.Eq(F.Of(this.SampledCellCount), F.Nullif(F.Of(this.CellsFailed))))); set { }
        }

        // Formula Formula (rulebook: =INDEX(RulebookFields!{{Formula}}, MATCH({{RulebookField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public string? Formula
        {
            get => F.AsString(F.Memo(this, "Formula", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.RulebookField), __r => F.Of(__r.Formula), () => F.Of(new RulebookField().Formula)))); set { }
        }

        // Formula SubstrateLabel (rulebook: =INDEX(ConformanceSubstrates!{{Label}}, MATCH({{Substrate}}, ConformanceSubstrates!{{ConformanceSubstrateId}}, 0)))
        [NotMapped]
        public string? SubstrateLabel
        {
            get => F.AsString(F.Memo(this, "SubstrateLabel", () => F.Lookup<ConformanceSubstrate>(this, "ConformanceSubstrates", "ConformanceSubstrateId", __c => __c.ConformanceSubstrates, __r => F.Of(__r.ConformanceSubstrateId), F.Of(this.Substrate), __r => F.Of(__r.Label), () => F.Of(new ConformanceSubstrate().Label)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Substrate { get; set; }
        public string? RulebookField { get; set; }
        public string? TableConformance { get; set; }

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

        private RulebookField _rulebookFieldRef;

        [ForeignKey("RulebookField")]
        public virtual RulebookField RulebookFieldRef
        {
            get
            {
                if (_rulebookFieldRef == null && !string.IsNullOrEmpty(RulebookField))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookFieldRef - no database context is set. RulebookField: " + RulebookField + ".");
                        }
                        return null;
                    }
                    _rulebookFieldRef = base.SoAContext.RulebookFields.Find(RulebookField);
                    if (_rulebookFieldRef != null)
                    {
                        base.SoAContext.Attach(_rulebookFieldRef);
                    }
                }
                return _rulebookFieldRef;
            }
            set
            {
                if (_rulebookFieldRef != value)
                {
                    _rulebookFieldRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookFieldRef != null)
                    {
                        RulebookField = _rulebookFieldRef.RulebookFieldId;
                    }
                }
            }
        }

        private TableConformance _tableConformanceRef;

        [ForeignKey("TableConformance")]
        public virtual TableConformance TableConformanceRef
        {
            get
            {
                if (_tableConformanceRef == null && !string.IsNullOrEmpty(TableConformance))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TableConformanceRef - no database context is set. TableConformance: " + TableConformance + ".");
                        }
                        return null;
                    }
                    _tableConformanceRef = base.SoAContext.TableConformance.Find(TableConformance);
                    if (_tableConformanceRef != null)
                    {
                        base.SoAContext.Attach(_tableConformanceRef);
                    }
                }
                return _tableConformanceRef;
            }
            set
            {
                if (_tableConformanceRef != value)
                {
                    _tableConformanceRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_tableConformanceRef != null)
                    {
                        TableConformance = _tableConformanceRef.TableConformanceId;
                    }
                }
            }
        }

        private ObservableCollection<CellDisagreement> _cellDisagreements;

        [InverseProperty("FieldDisagreementRef")]
        public virtual ObservableCollection<CellDisagreement> CellDisagreements
        {
            get
            {
                if (_cellDisagreements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CellDisagreements - no database context is set. FieldDisagreementId: " + this.FieldDisagreementId + ".");
                        }
                        _cellDisagreements = new ObservableCollection<CellDisagreement>();
                    }
                    else
                    {
                        var items = base.SoAContext.CellDisagreements.Where(x => x.FieldDisagreement == this.FieldDisagreementId).ToList<CellDisagreement>();
                        _cellDisagreements = new ObservableCollection<CellDisagreement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _cellDisagreements.CollectionChanged += CellDisagreements_CollectionChanged;
                }
                return _cellDisagreements;
            }
            private set
            {
                if (_cellDisagreements != null)
                {
                    _cellDisagreements.CollectionChanged -= CellDisagreements_CollectionChanged;
                }
                _cellDisagreements = value;
                if (_cellDisagreements != null)
                {
                    _cellDisagreements.CollectionChanged += CellDisagreements_CollectionChanged;
                }
            }
        }

        private void CellDisagreements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CellDisagreement>())
                {
                    item.FieldDisagreement = this.FieldDisagreementId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ConformanceSubstrate;
            _ = this.RulebookFieldRef;
            _ = this.TableConformanceRef;
            _ = this.CellDisagreements;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
