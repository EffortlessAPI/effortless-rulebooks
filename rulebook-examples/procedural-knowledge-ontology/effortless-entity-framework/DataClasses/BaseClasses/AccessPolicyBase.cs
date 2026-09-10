
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AccessPolicies")]
    public class AccessPolicyBase : SoAEntityBase
    {
        [Key]
        public string AccessPolicyId { get; set; }

        // Formula Name (rulebook: ={{Principal}} & " " & {{Command}} & " " & {{TargetTable}})
        public string? Name
        {
            get => this.Principal + " " + this.Command + " " + this.TargetTable; set { }
        }

        public string? Command { get; set; }
        public string? RowPredicate { get; set; }
        public string? CheckPredicate { get; set; }
        public string? Rationale { get; set; }
        public bool? ReferencesInference { get; set; }
        // Formula IsWriteCommand (rulebook: =OR({{Command}} = "INSERT", {{Command}} = "UPDATE", {{Command}} = "DELETE", {{Command}} = "ALL"))
        public bool? IsWriteCommand
        {
            get => OR(this.Command = "INSERT", this.Command = "UPDATE", this.Command = "DELETE", this.Command = "ALL"); set { }
        }

        // Formula IsUnrestricted (rulebook: ={{RowPredicate}} = "")
        public bool? IsUnrestricted
        {
            get => this.RowPredicate = ""; set { }
        }

        // Formula PrincipalIsAdmin (rulebook: =INDEX(AccessPrincipals!{{IsAdministrator}}, MATCH({{Principal}}, AccessPrincipals!{{AccessPrincipalId}}, 0)))
        public bool? PrincipalIsAdmin
        {
            get => INDEX(AccessPrincipals!this.IsAdministrator, MATCH(this.Principal, AccessPrincipals!this.AccessPrincipalId, 0)); set { }
        }

        // Formula IsUnrestrictedNonAdminGrant (rulebook: =AND({{IsUnrestricted}}, NOT({{PrincipalIsAdmin}})))
        public bool? IsUnrestrictedNonAdminGrant
        {
            get => AND(this.IsUnrestricted, NOT(this.PrincipalIsAdmin)); set { }
        }

        // Formula IsUnwitnessedWrite (rulebook: =AND({{IsWriteCommand}}, {{DenialTestCount}} = 0))
        public bool? IsUnwitnessedWrite
        {
            get => AND(this.IsWriteCommand, this.DenialTestCount = 0); set { }
        }

        // Formula DenialTestCount (rulebook: =COUNTIFS(AccessDenialTests!{{TargetPolicy}}, {{AccessPolicyId}}))
        public decimal? DenialTestCount
        {
            get => COUNTIFS(AccessDenialTests!this.TargetPolicy, this.AccessPolicyId); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Principal { get; set; }
        public string? TargetTable { get; set; }

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

        private ObservableCollection<AccessDenialTest> _accessDenialTests;

        [InverseProperty("AccessPolicy")]
        public virtual ObservableCollection<AccessDenialTest> AccessDenialTests
        {
            get
            {
                if (_accessDenialTests == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessDenialTests - no database context is set. AccessPolicyId: " + this.AccessPolicyId + ".");
                        }
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>();
                    }
                    else
                    {
                        var items = Context.AccessDenialTests.Where(x => x.TargetPolicy == this.AccessPolicyId).ToList<AccessDenialTest>();
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _accessDenialTests.CollectionChanged += AccessDenialTests_CollectionChanged;
                }
                return _accessDenialTests;
            }
            private set
            {
                if (_accessDenialTests != null)
                {
                    _accessDenialTests.CollectionChanged -= AccessDenialTests_CollectionChanged;
                }
                _accessDenialTests = value;
                if (_accessDenialTests != null)
                {
                    _accessDenialTests.CollectionChanged += AccessDenialTests_CollectionChanged;
                }
            }
        }

        private void AccessDenialTests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AccessDenialTest>())
                {
                    item.TargetPolicy = this.AccessPolicyId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AccessPrincipal;
            _ = this.RulebookTable;
            _ = this.AccessDenialTests;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
