
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ProcedureResources")]
    public class ProcedureResourceBase : SoAEntityBase
    {
        [Key]
        public string ProcedureResourceId { get; set; }

        // Formula Name (rulebook: ={{ProcedureVersion}} & " / " & {{Resource}})
        public string? Name
        {
            get => this.ProcedureVersion + " / " + this.Resource; set { }
        }

        public string? Relation { get; set; }
        // Formula RelationIri (rulebook: =IF({{Relation}} = "wasExtractedFrom", "https://w3id.org/pko#wasExtractedFrom", "http://purl.org/dc/terms/references"))
        public string? RelationIri
        {
            get => IF(this.Relation = "wasExtractedFrom", "https://w3id.org/pko#wasExtractedFrom", "http://purl.org/dc/terms/references"); set { }
        }


        public string? ProcedureVersion { get; set; }
        public string? Resource { get; set; }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = Context.ProcedureVersions.Find(ProcedureVersion);
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
                    ProcedureVersion = _procedureVersion == null ? default : _procedureVersion.ProcedureVersionId;
                }
            }
        }

        private Resource _resource;

        [ForeignKey("Resource")]
        public virtual Resource Resource
        {
            get
            {
                if (_resource == null && !string.IsNullOrEmpty(Resource))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. Resource: " + Resource + ".");
                        }
                        return null;
                    }
                    _resource = Context.Resources.Find(Resource);
                    if (_resource != null)
                    {
                        Context.Attach(_resource);
                    }
                }
                return _resource;
            }
            set
            {
                if (_resource != value)
                {
                    _resource = value;
                    Resource = _resource == null ? default : _resource.ResourceId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.Resource;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
