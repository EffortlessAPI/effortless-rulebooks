
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
    [Table("ProcessInterdependencies")]
    public class ProcessInterdependencyBase : SoAEntityBase
    {
        [Key]
        public string ProcessInterdependencyId { get; set; }

        // Formula Name (rulebook: ={{FromProcedure}} & " " & {{Effect}} & " " & {{ToProcedure}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.FromProcedure)), F.S(" "), F.Text(F.Of(this.Effect)), F.S(" "), F.Text(F.Of(this.ToProcedure))))); set { }
        }

        public string? Effect { get; set; }
        public string? Mechanism { get; set; }
        public string? EvidenceNote { get; set; }
        // Formula IsHinderingDependency (rulebook: ={{Effect}} = "Hinders")
        [NotMapped]
        public bool? IsHinderingDependency
        {
            get => F.AsBool(F.Memo(this, "IsHinderingDependency", () => F.Eq(F.Nullif(F.Of(this.Effect)), F.S("Hinders")))); set { }
        }

        // Formula HinderedProcedureKey (rulebook: =IF({{Effect}} = "Hinders", {{ToProcedure}}, ""))
        [NotMapped]
        public string? HinderedProcedureKey
        {
            get => F.AsString(F.Memo(this, "HinderedProcedureKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Effect)), F.S("Hinders")))) ? F.Of(this.ToProcedure) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? FromProcedure { get; set; }
        public string? ToProcedure { get; set; }

        private Procedure _procedure;

        [ForeignKey("FromProcedure")]
        public virtual Procedure Procedure
        {
            get
            {
                if (_procedure == null && !string.IsNullOrEmpty(FromProcedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedure - no database context is set. FromProcedure: " + FromProcedure + ".");
                        }
                        return null;
                    }
                    _procedure = base.SoAContext.Procedures.Find(FromProcedure);
                    if (_procedure != null)
                    {
                        base.SoAContext.Attach(_procedure);
                    }
                }
                return _procedure;
            }
            set
            {
                if (_procedure != value)
                {
                    _procedure = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedure != null)
                    {
                        FromProcedure = _procedure.ProcedureId;
                    }
                }
            }
        }

        private Procedure _procedureRef;

        [ForeignKey("ToProcedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(ToProcedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. ToProcedure: " + ToProcedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(ToProcedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        ToProcedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Procedure;
            _ = this.ProcedureRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
