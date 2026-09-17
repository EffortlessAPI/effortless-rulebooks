
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
    [Table("ProcedureTypes")]
    public class ProcedureTypeBase : SoAEntityBase
    {
        [Key]
        public string ProcedureTypeId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Definition { get; set; }
        public string? SemanticTypeIri { get; set; }
        public string? TaxonomyRank { get; set; }
        public string? DistinguishingValue { get; set; }
        // Formula NarrowerTypeCount (rulebook: =COUNTIFS(ProcedureTypes!{{BroaderProcedureType}}, ProcedureTypes!{{ProcedureTypeId}}))
        [NotMapped]
        public int? NarrowerTypeCount
        {
            get => F.AsInt(F.Memo(this, "NarrowerTypeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureType>(base.SoAContext, "ProcedureTypes", __c => __c.ProcedureTypes), __r => F.CritField(F.Of(__r.BroaderProcedureType), F.Of(this.ProcedureTypeId))))))); set { }
        }

        // Formula HasNarrowerTypes (rulebook: ={{NarrowerTypeCount}} > 0)
        [NotMapped]
        public bool? HasNarrowerTypes
        {
            get => F.AsBool(F.Memo(this, "HasNarrowerTypes", () => F.Cmp(F.Of(this.NarrowerTypeCount), ">", F.I(0)))); set { }
        }

        // Formula IsDetachedFromTaxonomy (rulebook: =AND({{TaxonomyRank}} <> "FunctionalDomain", {{BroaderProcedureType}} = ""))
        [NotMapped]
        public bool? IsDetachedFromTaxonomy
        {
            get => F.AsBool(F.Memo(this, "IsDetachedFromTaxonomy", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.TaxonomyRank)), F.S("FunctionalDomain"))), F.Bool3(F.IsBlank(F.Of(this.BroaderProcedureType)))))); set { }
        }

        // Formula BroaderIsDetached (rulebook: =INDEX(ProcedureTypes!{{IsDetachedFromTaxonomy}}, MATCH({{BroaderProcedureType}}, ProcedureTypes!{{ProcedureTypeId}}, 0)))
        [NotMapped]
        public bool? BroaderIsDetached
        {
            get => F.AsBool(F.Memo(this, "BroaderIsDetached", () => F.Lookup<ProcedureType>(this, "ProcedureTypes", "ProcedureTypeId", __c => __c.ProcedureTypes, __r => F.Of(__r.ProcedureTypeId), F.Of(this.BroaderProcedureType), __r => F.Of(__r.IsDetachedFromTaxonomy), () => F.Of(new ProcedureType().IsDetachedFromTaxonomy)))); set { }
        }

        // Formula IsUnreachableByNavigation (rulebook: =OR({{IsDetachedFromTaxonomy}}, AND({{BroaderProcedureType}} <> "", {{BroaderIsDetached}} = TRUE)))
        [NotMapped]
        public bool? IsUnreachableByNavigation
        {
            get => F.AsBool(F.Memo(this, "IsUnreachableByNavigation", () => F.Or(F.Bool3(F.Of(this.IsDetachedFromTaxonomy)), F.Bool3(F.And(F.Bool3(F.IsNotBlank(F.Of(this.BroaderProcedureType))), F.Bool3(F.Eq(F.Of(this.BroaderIsDetached), F.B(true)))))))); set { }
        }

        // Formula MemberCount (rulebook: =COUNTIFS(Procedures!{{ProcedureType}}, {{ProcedureTypeId}}))
        [NotMapped]
        public int? MemberCount
        {
            get => F.AsInt(F.Memo(this, "MemberCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Procedure>(base.SoAContext, "Procedures", __c => __c.Procedures), __r => F.CritField(F.Of(__r.ProcedureType), F.Of(this.ProcedureTypeId))))))); set { }
        }

        // Formula MembersLackingDistinctionCount (rulebook: =COUNTIFS(Procedures!{{LacksDistinctionTypeKey}}, {{ProcedureTypeId}}))
        [NotMapped]
        public int? MembersLackingDistinctionCount
        {
            get => F.AsInt(F.Memo(this, "MembersLackingDistinctionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Procedure>(base.SoAContext, "Procedures", __c => __c.Procedures), __r => F.CritField(F.Of(__r.LacksDistinctionTypeKey), F.Of(this.ProcedureTypeId))))))); set { }
        }

        // Formula IsArbitraryGrouping (rulebook: =AND({{MemberCount}} > 0, OR({{DistinguishingFacet}} = "", {{MembersLackingDistinctionCount}} = {{MemberCount}})))
        [NotMapped]
        public bool? IsArbitraryGrouping
        {
            get => F.AsBool(F.Memo(this, "IsArbitraryGrouping", () => F.And(F.Bool3(F.Cmp(F.Of(this.MemberCount), ">", F.I(0))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.DistinguishingFacet))), F.Bool3(F.Eq(F.Of(this.MembersLackingDistinctionCount), F.Of(this.MemberCount)))))))); set { }
        }


        public string? BroaderProcedureType { get; set; }
        public string? DistinguishingFacet { get; set; }

        private ProcedureType _procedureType;

        [ForeignKey("BroaderProcedureType")]
        public virtual ProcedureType ProcedureType
        {
            get
            {
                if (_procedureType == null && !string.IsNullOrEmpty(BroaderProcedureType))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureType - no database context is set. BroaderProcedureType: " + BroaderProcedureType + ".");
                        }
                        return null;
                    }
                    _procedureType = base.SoAContext.ProcedureTypes.Find(BroaderProcedureType);
                    if (_procedureType != null)
                    {
                        base.SoAContext.Attach(_procedureType);
                    }
                }
                return _procedureType;
            }
            set
            {
                if (_procedureType != value)
                {
                    _procedureType = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureType != null)
                    {
                        BroaderProcedureType = _procedureType.ProcedureTypeId;
                    }
                }
            }
        }

        private ClassificationFacet _classificationFacet;

        [ForeignKey("DistinguishingFacet")]
        public virtual ClassificationFacet ClassificationFacet
        {
            get
            {
                if (_classificationFacet == null && !string.IsNullOrEmpty(DistinguishingFacet))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ClassificationFacet - no database context is set. DistinguishingFacet: " + DistinguishingFacet + ".");
                        }
                        return null;
                    }
                    _classificationFacet = base.SoAContext.ClassificationFacets.Find(DistinguishingFacet);
                    if (_classificationFacet != null)
                    {
                        base.SoAContext.Attach(_classificationFacet);
                    }
                }
                return _classificationFacet;
            }
            set
            {
                if (_classificationFacet != value)
                {
                    _classificationFacet = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_classificationFacet != null)
                    {
                        DistinguishingFacet = _classificationFacet.ClassificationFacetId;
                    }
                }
            }
        }

        private ObservableCollection<ProcedureType> _procedureTypes;

        [InverseProperty("ProcedureType")]
        public virtual ObservableCollection<ProcedureType> ProcedureTypes
        {
            get
            {
                if (_procedureTypes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureTypes - no database context is set. ProcedureTypeId: " + this.ProcedureTypeId + ".");
                        }
                        _procedureTypes = new ObservableCollection<ProcedureType>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureTypes.Where(x => x.BroaderProcedureType == this.ProcedureTypeId).ToList<ProcedureType>();
                        _procedureTypes = new ObservableCollection<ProcedureType>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureTypes.CollectionChanged += ProcedureTypes_CollectionChanged;
                }
                return _procedureTypes;
            }
            private set
            {
                if (_procedureTypes != null)
                {
                    _procedureTypes.CollectionChanged -= ProcedureTypes_CollectionChanged;
                }
                _procedureTypes = value;
                if (_procedureTypes != null)
                {
                    _procedureTypes.CollectionChanged += ProcedureTypes_CollectionChanged;
                }
            }
        }

        private void ProcedureTypes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureType>())
                {
                    item.BroaderProcedureType = this.ProcedureTypeId;
                }
            }
        }

        private ObservableCollection<Procedure> _procedures;

        [InverseProperty("ProcedureTypeRef")]
        public virtual ObservableCollection<Procedure> Procedures
        {
            get
            {
                if (_procedures == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedures - no database context is set. ProcedureTypeId: " + this.ProcedureTypeId + ".");
                        }
                        _procedures = new ObservableCollection<Procedure>();
                    }
                    else
                    {
                        var items = base.SoAContext.Procedures.Where(x => x.ProcedureType == this.ProcedureTypeId).ToList<Procedure>();
                        _procedures = new ObservableCollection<Procedure>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
                return _procedures;
            }
            private set
            {
                if (_procedures != null)
                {
                    _procedures.CollectionChanged -= Procedures_CollectionChanged;
                }
                _procedures = value;
                if (_procedures != null)
                {
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
            }
        }

        private void Procedures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Procedure>())
                {
                    item.ProcedureType = this.ProcedureTypeId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureType;
            _ = this.ClassificationFacet;
            _ = this.ProcedureTypes;
            _ = this.Procedures;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
