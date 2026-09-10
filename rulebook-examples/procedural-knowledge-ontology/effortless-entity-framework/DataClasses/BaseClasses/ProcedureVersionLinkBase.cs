
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ProcedureVersionLinks")]
    public class ProcedureVersionLinkBase : SoAEntityBase
    {
        [Key]
        public string ProcedureVersionLinkId { get; set; }

        // Formula Name (rulebook: ={{PreviousProcedureVersion}} & " -> " & {{NextProcedureVersion}})
        public string? Name
        {
            get => this.PreviousProcedureVersion + " -> " + this.NextProcedureVersion; set { }
        }

        public string? RelationIri { get; set; }
        public string? ChangeSummary { get; set; }
        // Formula SupersededVersionKey (rulebook: =IF({{RelationIri}} = "https://w3id.org/pko#nextVersion", {{PreviousProcedureVersion}}, ""))
        public string? SupersededVersionKey
        {
            get => IF(this.RelationIri = "https://w3id.org/pko#nextVersion", this.PreviousProcedureVersion, ""); set { }
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. PreviousProcedureVersion: " + PreviousProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = Context.ProcedureVersions.Find(PreviousProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        Context.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    PreviousProcedureVersion = _procedureVersion == null ? default : _procedureVersion.ProcedureVersionId;
                }
            }
        }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("NextProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(NextProcedureVersion))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. NextProcedureVersion: " + NextProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = Context.ProcedureVersions.Find(NextProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        Context.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    NextProcedureVersion = _procedureVersion == null ? default : _procedureVersion.ProcedureVersionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.ProcedureVersion;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
