
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
    [Table("ProcedureVersionLinks")]
    public class ProcedureVersionLinkBase : SoAEntityBase
    {
        [Key]
        public string ProcedureVersionLinkId { get; set; }

        // Formula Name (rulebook: ={{PreviousProcedureVersion}} & " -> " & {{NextProcedureVersion}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.PreviousProcedureVersion)), F.S(" -> "), F.Text(F.Of(this.NextProcedureVersion))))); set { }
        }

        public string? RelationIri { get; set; }
        public string? ChangeSummary { get; set; }
        // Formula SupersededVersionKey (rulebook: =IF({{RelationIri}} = "https://w3id.org/pko#nextVersion", {{PreviousProcedureVersion}}, ""))
        [NotMapped]
        public string? SupersededVersionKey
        {
            get => F.AsString(F.Memo(this, "SupersededVersionKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.RelationIri)), F.S("https://w3id.org/pko#nextVersion")))) ? F.Of(this.PreviousProcedureVersion) : F.S("")))); set { }
        }


        public string? PreviousProcedureVersion { get; set; }
        public string? NextProcedureVersion { get; set; }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("PreviousProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(PreviousProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. PreviousProcedureVersion: " + PreviousProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = base.SoAContext.ProcedureVersions.Find(PreviousProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        base.SoAContext.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersion != null)
                    {
                        PreviousProcedureVersion = _procedureVersion.ProcedureVersionId;
                    }
                }
            }
        }

        private ProcedureVersion _procedureVersionRef;

        [ForeignKey("NextProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersionRef
        {
            get
            {
                if (_procedureVersionRef == null && !string.IsNullOrEmpty(NextProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionRef - no database context is set. NextProcedureVersion: " + NextProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersionRef = base.SoAContext.ProcedureVersions.Find(NextProcedureVersion);
                    if (_procedureVersionRef != null)
                    {
                        base.SoAContext.Attach(_procedureVersionRef);
                    }
                }
                return _procedureVersionRef;
            }
            set
            {
                if (_procedureVersionRef != value)
                {
                    _procedureVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersionRef != null)
                    {
                        NextProcedureVersion = _procedureVersionRef.ProcedureVersionId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.ProcedureVersionRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
