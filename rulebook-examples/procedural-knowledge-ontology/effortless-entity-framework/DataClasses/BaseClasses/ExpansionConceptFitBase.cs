
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
    [Table("ExpansionConceptFits")]
    public class ExpansionConceptFitBase : SoAEntityBase
    {
        [Key]
        public string ExpansionConceptFitId { get; set; }

        // Formula Name (rulebook: ={{ModelExpansionRequest}} & ": " & {{ConceptLabel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ModelExpansionRequest)), F.S(": "), F.Text(F.Of(this.ConceptLabel))))); set { }
        }

        public string? ConceptLabel { get; set; }
        // Formula IsUncovered (rulebook: ={{CoveringTable}} = "")
        [NotMapped]
        public bool? IsUncovered
        {
            get => F.AsBool(F.Memo(this, "IsUncovered", () => F.IsBlank(F.Of(this.CoveringTable)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ModelExpansionRequest { get; set; }
        public string? CoveringTable { get; set; }

        private ModelExpansionRequest _modelExpansionRequestRef;

        [ForeignKey("ModelExpansionRequest")]
        public virtual ModelExpansionRequest ModelExpansionRequestRef
        {
            get
            {
                if (_modelExpansionRequestRef == null && !string.IsNullOrEmpty(ModelExpansionRequest))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelExpansionRequestRef - no database context is set. ModelExpansionRequest: " + ModelExpansionRequest + ".");
                        }
                        return null;
                    }
                    _modelExpansionRequestRef = base.SoAContext.ModelExpansionRequests.Find(ModelExpansionRequest);
                    if (_modelExpansionRequestRef != null)
                    {
                        base.SoAContext.Attach(_modelExpansionRequestRef);
                    }
                }
                return _modelExpansionRequestRef;
            }
            set
            {
                if (_modelExpansionRequestRef != value)
                {
                    _modelExpansionRequestRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_modelExpansionRequestRef != null)
                    {
                        ModelExpansionRequest = _modelExpansionRequestRef.ModelExpansionRequestId;
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
            _ = this.ModelExpansionRequestRef;
            _ = this.RulebookTable;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
