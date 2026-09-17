
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
    [Table("DomainCoverageAreas")]
    public class DomainCoverageAreaBase : SoAEntityBase
    {
        [Key]
        public string DomainCoverageAreaId { get; set; }

        // Formula Name (rulebook: ={{AreaLabel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.AreaLabel))); set { }
        }

        public string? AreaLabel { get; set; }
        public string? RequiredConcept { get; set; }
        // Formula IsUncoveredArea (rulebook: ={{CoveringTable}} = "")
        [NotMapped]
        public bool? IsUncoveredArea
        {
            get => F.AsBool(F.Memo(this, "IsUncoveredArea", () => F.IsBlank(F.Of(this.CoveringTable)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? CoveringTable { get; set; }

        private GovernedModel _governedModelRef;

        [ForeignKey("GovernedModel")]
        public virtual GovernedModel GovernedModelRef
        {
            get
            {
                if (_governedModelRef == null && !string.IsNullOrEmpty(GovernedModel))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModelRef - no database context is set. GovernedModel: " + GovernedModel + ".");
                        }
                        return null;
                    }
                    _governedModelRef = base.SoAContext.GovernedModels.Find(GovernedModel);
                    if (_governedModelRef != null)
                    {
                        base.SoAContext.Attach(_governedModelRef);
                    }
                }
                return _governedModelRef;
            }
            set
            {
                if (_governedModelRef != value)
                {
                    _governedModelRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_governedModelRef != null)
                    {
                        GovernedModel = _governedModelRef.GovernedModelId;
                    }
                }
            }
        }

        private RulebookTable _rulebookTable;

        [ForeignKey("CoveringTable")]
        public virtual RulebookTable RulebookTable
        {
            get
            {
                if (_rulebookTable == null && !string.IsNullOrEmpty(CoveringTable))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTable - no database context is set. CoveringTable: " + CoveringTable + ".");
                        }
                        return null;
                    }
                    _rulebookTable = base.SoAContext.RulebookTables.Find(CoveringTable);
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
                        CoveringTable = _rulebookTable.RulebookTableId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.RulebookTable;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
