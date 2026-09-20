
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
    [Table("AbundantKnowledgeGaps")]
    public class AbundantKnowledgeGapBase : SoAEntityBase
    {
        [Key]
        public string AbundantKnowledgeGapId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? GapKind { get; set; }
        public string? Description { get; set; }
        public string? WhyAbundanceDoesNotSupplyIt { get; set; }
        // Formula RepresentingTableRowCount (rulebook: =INDEX(RulebookTables!{{MeasuredRowCount}}, MATCH({{RepresentedByTable}}, RulebookTables!{{RulebookTableId}}, 0)))
        [NotMapped]
        public int? RepresentingTableRowCount
        {
            get => F.AsInt(F.Memo(this, "RepresentingTableRowCount", () => F.Integer(F.Lookup<RulebookTable>(this, "RulebookTables", "RulebookTableId", __c => __c.RulebookTables, __r => F.Of(__r.RulebookTableId), F.Of(this.RepresentedByTable), __r => F.Of(__r.MeasuredRowCount), () => F.Of(new RulebookTable().MeasuredRowCount))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? RepresentedByTable { get; set; }

        private RulebookTable _rulebookTable;

        [ForeignKey("RepresentedByTable")]
        public virtual RulebookTable RulebookTable
        {
            get
            {
                if (_rulebookTable == null && !string.IsNullOrEmpty(RepresentedByTable))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTable - no database context is set. RepresentedByTable: " + RepresentedByTable + ".");
                        }
                        return null;
                    }
                    _rulebookTable = base.SoAContext.RulebookTables.Find(RepresentedByTable);
                    if (_rulebookTable != null)
                    {
                        base.SoAContext.Attach(_rulebookTable);
                    }
                }
                return _rulebookTable;
            }
            set
            {
                if (_rulebookTable != value)
                {
                    _rulebookTable = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookTable != null)
                    {
                        RepresentedByTable = _rulebookTable.RulebookTableId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RulebookTable;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
