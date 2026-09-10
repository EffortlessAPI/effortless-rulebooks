
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AccessDenialTests")]
    public class AccessDenialTestBase : SoAEntityBase
    {
        [Key]
        public string AccessDenialTestId { get; set; }

        // Formula Name (rulebook: ={{Principal}} & " must not see " & {{ForbiddenRowId}})
        public string? Name
        {
            get => this.Principal + " must not see " + this.ForbiddenRowId; set { }
        }

        public string? ForbiddenRowId { get; set; }
        public bool? ExpectedVisible { get; set; }
        public bool? ObservedVisible { get; set; }
        public DateTime? LastRunAt { get; set; }
        // Formula HasRun (rulebook: ={{LastRunAt}} <> "")
        public bool? HasRun
        {
            get => this.LastRunAt <> ""; set { }
        }

        // Formula IsPassing (rulebook: ={{ObservedVisible}} = {{ExpectedVisible}})
        public bool? IsPassing
        {
            get => this.ObservedVisible = this.ExpectedVisible; set { }
        }

        // Formula IsLeak (rulebook: =AND(NOT({{ExpectedVisible}}), {{ObservedVisible}}))
        public bool? IsLeak
        {
            get => AND(NOT(this.ExpectedVisible), this.ObservedVisible); set { }
        }

        // Formula IsUnproven (rulebook: =NOT({{HasRun}}))
        public bool? IsUnproven
        {
            get => NOT(this.HasRun); set { }
        }

        public string? Rationale { get; set; }
        // Formula IsPositiveControl (rulebook: ={{ExpectedVisible}})
        public bool? IsPositiveControl
        {
            get => this.ExpectedVisible; set { }
        }

        public string? ForbiddenTable { get; set; }
        public string? ForbiddenColumn { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? TargetPolicy { get; set; }
        public string? Principal { get; set; }
        public string? TargetTable { get; set; }

        private AccessPolicy _accessPolicy;

        [ForeignKey("TargetPolicy")]
        public virtual AccessPolicy AccessPolicy
        {
            get
            {
                if (_accessPolicy == null && !string.IsNullOrEmpty(TargetPolicy))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPolicy - no database context is set. TargetPolicy: " + TargetPolicy + ".");
                        }
                        return null;
                    }
                    _accessPolicy = Context.AccessPolicies.Find(TargetPolicy);
                    if (_accessPolicy != null)
                    {
                        Context.Attach(_accessPolicy);
                    }
                }
                return _accessPolicy;
            }
            set
            {
                if (_accessPolicy != value)
                {
                    _accessPolicy = value;
                    TargetPolicy = _accessPolicy == null ? default : _accessPolicy.AccessPolicyId;
                }
            }
        }

        private AccessPrincipal _accessPrincipal;

        [ForeignKey("Principal")]
        public virtual AccessPrincipal AccessPrincipal
        {
            get
            {
                if (_accessPrincipal == null && !string.IsNullOrEmpty(Principal))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPrincipal - no database context is set. Principal: " + Principal + ".");
                        }
                        return null;
                    }
                    _accessPrincipal = Context.AccessPrincipals.Find(Principal);
                    if (_accessPrincipal != null)
                    {
                        Context.Attach(_accessPrincipal);
                    }
                }
                return _accessPrincipal;
            }
            set
            {
                if (_accessPrincipal != value)
                {
                    _accessPrincipal = value;
                    Principal = _accessPrincipal == null ? default : _accessPrincipal.AccessPrincipalId;
                }
            }
        }

        private RulebookTable _rulebookTable;

        [ForeignKey("TargetTable")]
        public virtual RulebookTable RulebookTable
        {
            get
            {
                if (_rulebookTable == null && !string.IsNullOrEmpty(TargetTable))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTable - no database context is set. TargetTable: " + TargetTable + ".");
                        }
                        return null;
                    }
                    _rulebookTable = Context.RulebookTables.Find(TargetTable);
                    if (_rulebookTable != null)
                    {
                        Context.Attach(_rulebookTable);
                    }
                }
                return _rulebookTable;
            }
            set
            {
                if (_rulebookTable != value)
                {
                    _rulebookTable = value;
                    TargetTable = _rulebookTable == null ? default : _rulebookTable.RulebookTableId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AccessPolicy;
            _ = this.AccessPrincipal;
            _ = this.RulebookTable;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
