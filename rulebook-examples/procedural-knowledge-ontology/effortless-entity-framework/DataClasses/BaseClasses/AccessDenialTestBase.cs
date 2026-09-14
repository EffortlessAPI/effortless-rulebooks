
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
    [Table("AccessDenialTests")]
    public class AccessDenialTestBase : SoAEntityBase
    {
        [Key]
        public string AccessDenialTestId { get; set; }

        // Formula Name (rulebook: ={{Principal}} & " must not see " & {{ForbiddenRowId}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Principal)), F.S(" must not see "), F.Text(F.Of(this.ForbiddenRowId))))); set { }
        }

        public string? ForbiddenRowId { get; set; }
        public bool? ExpectedVisible { get; set; }
        public bool? ObservedVisible { get; set; }
        public DateTimeOffset? LastRunAt { get; set; }
        // Formula HasRun (rulebook: ={{LastRunAt}} <> "")
        [NotMapped]
        public bool? HasRun
        {
            get => F.AsBool(F.Memo(this, "HasRun", () => F.IsNotBlank(F.Of(this.LastRunAt)))); set { }
        }

        // Formula IsPassing (rulebook: ={{ObservedVisible}} = {{ExpectedVisible}})
        [NotMapped]
        public bool? IsPassing
        {
            get => F.AsBool(F.Memo(this, "IsPassing", () => F.Eq(F.Nullif(F.Of(this.ObservedVisible)), F.Nullif(F.Of(this.ExpectedVisible))))); set { }
        }

        // Formula IsLeak (rulebook: =AND(NOT({{ExpectedVisible}}), {{ObservedVisible}}))
        [NotMapped]
        public bool? IsLeak
        {
            get => F.AsBool(F.Memo(this, "IsLeak", () => F.And(F.Bool3(F.Not(F.IsTrueV(F.Of(this.ExpectedVisible)))), F.IsTrueV(F.Of(this.ObservedVisible))))); set { }
        }

        // Formula IsUnproven (rulebook: =NOT({{HasRun}}))
        [NotMapped]
        public bool? IsUnproven
        {
            get => F.AsBool(F.Memo(this, "IsUnproven", () => F.Not(F.Bool3(F.Of(this.HasRun))))); set { }
        }

        public string? Rationale { get; set; }
        // Formula IsPositiveControl (rulebook: ={{ExpectedVisible}})
        [NotMapped]
        public bool? IsPositiveControl
        {
            get => F.AsBool(F.Memo(this, "IsPositiveControl", () => F.Of(this.ExpectedVisible))); set { }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPolicy - no database context is set. TargetPolicy: " + TargetPolicy + ".");
                        }
                        return null;
                    }
                    _accessPolicy = base.SoAContext.AccessPolicies.Find(TargetPolicy);
                    if (_accessPolicy != null)
                    {
                        base.SoAContext.Attach(_accessPolicy);
                    }
                }
                return _accessPolicy;
            }
            set
            {
                if (_accessPolicy != value)
                {
                    _accessPolicy = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_accessPolicy != null)
                    {
                        TargetPolicy = _accessPolicy.AccessPolicyId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPrincipal - no database context is set. Principal: " + Principal + ".");
                        }
                        return null;
                    }
                    _accessPrincipal = base.SoAContext.AccessPrincipals.Find(Principal);
                    if (_accessPrincipal != null)
                    {
                        base.SoAContext.Attach(_accessPrincipal);
                    }
                }
                return _accessPrincipal;
            }
            set
            {
                if (_accessPrincipal != value)
                {
                    _accessPrincipal = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_accessPrincipal != null)
                    {
                        Principal = _accessPrincipal.AccessPrincipalId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTable - no database context is set. TargetTable: " + TargetTable + ".");
                        }
                        return null;
                    }
                    _rulebookTable = base.SoAContext.RulebookTables.Find(TargetTable);
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
                        TargetTable = _rulebookTable.RulebookTableId;
                    }
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
