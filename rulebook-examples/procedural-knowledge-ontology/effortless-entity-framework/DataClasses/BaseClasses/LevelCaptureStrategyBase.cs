
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
    [Table("LevelCaptureStrategies")]
    public class LevelCaptureStrategyBase : SoAEntityBase
    {
        [Key]
        public string LevelCaptureStrategyId { get; set; }

        // Formula Name (rulebook: ={{Level}} & " " & {{KnowledgeForm}} & ": " & LEFT({{Description}}, 40))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Level)), F.S(" "), F.Text(F.Of(this.KnowledgeForm)), F.S(": "), F.Text(F.Left(F.Of(this.Description), F.I(40)))))); set { }
        }

        public string? KnowledgeForm { get; set; }
        public string? TransferMode { get; set; }
        public string? Description { get; set; }
        // Formula ContradictsKnowledgeForm (rulebook: =OR(AND({{KnowledgeForm}} = "Tacit", {{TransferMode}} = "Codified"), AND({{KnowledgeForm}} = "Explicit", {{TransferMode}} = "InPerson")))
        [NotMapped]
        public bool? ContradictsKnowledgeForm
        {
            get => F.AsBool(F.Memo(this, "ContradictsKnowledgeForm", () => F.Or(F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.KnowledgeForm)), F.S("Tacit"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.TransferMode)), F.S("Codified"))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.KnowledgeForm)), F.S("Explicit"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.TransferMode)), F.S("InPerson")))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Level { get; set; }
        public string? KnowledgeMethod { get; set; }

        private ProcessKnowledgeLevel _processKnowledgeLevel;

        [ForeignKey("Level")]
        public virtual ProcessKnowledgeLevel ProcessKnowledgeLevel
        {
            get
            {
                if (_processKnowledgeLevel == null && !string.IsNullOrEmpty(Level))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessKnowledgeLevel - no database context is set. Level: " + Level + ".");
                        }
                        return null;
                    }
                    _processKnowledgeLevel = base.SoAContext.ProcessKnowledgeLevels.Find(Level);
                    if (_processKnowledgeLevel != null)
                    {
                        base.SoAContext.Attach(_processKnowledgeLevel);
                    }
                }
                return _processKnowledgeLevel;
            }
            set
            {
                if (_processKnowledgeLevel != value)
                {
                    _processKnowledgeLevel = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_processKnowledgeLevel != null)
                    {
                        Level = _processKnowledgeLevel.ProcessKnowledgeLevelId;
                    }
                }
            }
        }

        private KnowledgeMethod _knowledgeMethodRef;

        [ForeignKey("KnowledgeMethod")]
        public virtual KnowledgeMethod KnowledgeMethodRef
        {
            get
            {
                if (_knowledgeMethodRef == null && !string.IsNullOrEmpty(KnowledgeMethod))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeMethodRef - no database context is set. KnowledgeMethod: " + KnowledgeMethod + ".");
                        }
                        return null;
                    }
                    _knowledgeMethodRef = base.SoAContext.KnowledgeMethods.Find(KnowledgeMethod);
                    if (_knowledgeMethodRef != null)
                    {
                        base.SoAContext.Attach(_knowledgeMethodRef);
                    }
                }
                return _knowledgeMethodRef;
            }
            set
            {
                if (_knowledgeMethodRef != value)
                {
                    _knowledgeMethodRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeMethodRef != null)
                    {
                        KnowledgeMethod = _knowledgeMethodRef.KnowledgeMethodId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcessKnowledgeLevel;
            _ = this.KnowledgeMethodRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
