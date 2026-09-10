
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("SemanticMappings")]
    public class SemanticMappingBase : SoAEntityBase
    {
        [Key]
        public string SemanticMappingId { get; set; }

        // Formula Name (rulebook: ={{SourcePath}} & " -> " & {{TargetIri}})
        public string? Name
        {
            get => this.SourcePath + " -> " + this.TargetIri; set { }
        }

        public string? SourcePath { get; set; }
        public string? MappingKind { get; set; }
        public string? TargetIri { get; set; }
        public string? MappingRelation { get; set; }
        public string? Notes { get; set; }

        public string? OntologyProfile { get; set; }

        private OntologyProfile _ontologyProfile;

        [ForeignKey("OntologyProfile")]
        public virtual OntologyProfile OntologyProfile
        {
            get
            {
                if (_ontologyProfile == null && !string.IsNullOrEmpty(OntologyProfile))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OntologyProfile - no database context is set. OntologyProfile: " + OntologyProfile + ".");
                        }
                        return null;
                    }
                    _ontologyProfile = Context.OntologyProfiles.Find(OntologyProfile);
                    if (_ontologyProfile != null)
                    {
                        Context.Attach(_ontologyProfile);
                    }
                }
                return _ontologyProfile;
            }
            set
            {
                if (_ontologyProfile != value)
                {
                    _ontologyProfile = value;
                    OntologyProfile = _ontologyProfile == null ? default : _ontologyProfile.OntologyProfileId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OntologyProfile;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
