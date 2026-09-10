
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Rationales")]
    public class RationaleBase : SoAEntityBase
    {
        [Key]
        public string RationaleId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        public string? Name
        {
            get => this.Title; set { }
        }

        public string? Title { get; set; }
        public string? Statement { get; set; }
        public string? Status { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? AuthorityRole { get; set; }

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

        private Step _step;

        [ForeignKey("Step")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(Step))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _step = Context.Steps.Find(Step);
                    if (_step != null)
                    {
                        Context.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    Step = _step == null ? default : _step.StepId;
                }
            }
        }

        private Role _role;

        [ForeignKey("AuthorityRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(AuthorityRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AuthorityRole: " + AuthorityRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(AuthorityRole);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    AuthorityRole = _role == null ? default : _role.RoleId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.Step;
            _ = this.Role;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
