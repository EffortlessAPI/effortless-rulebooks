
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
    [Table("ChangeLog")]
    public class ChangeLogBase : SoAEntityBase
    {
        [Key]
        public string ChangeLogId { get; set; }

        // Formula RelativePath (rulebook: ="change-log/" & {{ChangeLogId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("change-log/"), F.TextOr(F.Of(this.ChangeLogId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        // Formula Name (rulebook: ={{Version}} & " (" & {{ChangeDate}} & ")")
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.Version)), F.S(" ("), F.TextOr(F.Of(this.ChangeDate)), F.S(")")))); set { }
        }

        public string? Version { get; set; }
        public DateOnly? ChangeDate { get; set; }
        public string? ChangeKind { get; set; }
        public string? MotivatingQuestion { get; set; }
        public string? TermsAffected { get; set; }
        public string? Rationale { get; set; }
        // Formula IsBreakingChange (rulebook: ={{ChangeKind}} = "major")
        [NotMapped]
        public bool? IsBreakingChange
        {
            get => F.AsBool(F.Memo(this, "IsBreakingChange", () => F.Eq(F.Nullif(F.Of(this.ChangeKind)), F.S("major")))); set { }
        }

        // Formula IsBackwardCompatible (rulebook: =OR({{ChangeKind}} = "patch", {{ChangeKind}} = "minor"))
        [NotMapped]
        public bool? IsBackwardCompatible
        {
            get => F.AsBool(F.Memo(this, "IsBackwardCompatible", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeKind)), F.S("patch"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeKind)), F.S("minor")))))); set { }
        }


        public string? ApprovedBy { get; set; }

        private GovernanceRole _governanceRole;

        [ForeignKey("ApprovedBy")]
        public virtual GovernanceRole GovernanceRole
        {
            get
            {
                if (_governanceRole == null && !string.IsNullOrEmpty(ApprovedBy))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernanceRole - no database context is set. ApprovedBy: " + ApprovedBy + ".");
                        }
                        return null;
                    }
                    _governanceRole = base.SoAContext.GovernanceRoles.Find(ApprovedBy);
                    if (_governanceRole != null)
                    {
                        base.SoAContext.Attach(_governanceRole);
                    }
                }
                return _governanceRole;
            }
            set
            {
                if (_governanceRole != value)
                {
                    _governanceRole = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_governanceRole != null)
                    {
                        ApprovedBy = _governanceRole.GovernanceRoleId;
                    }
                }
            }
        }

        private ObservableCollection<GovernanceRole> _governanceRoles;

        [InverseProperty("ChangeLog")]
        public virtual ObservableCollection<GovernanceRole> GovernanceRoles
        {
            get
            {
                if (_governanceRoles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernanceRoles - no database context is set. ChangeLogId: " + this.ChangeLogId + ".");
                        }
                        _governanceRoles = new ObservableCollection<GovernanceRole>();
                    }
                    else
                    {
                        var items = base.SoAContext.GovernanceRoles.Where(x => x.ApprovedChanges == this.ChangeLogId).ToList<GovernanceRole>();
                        _governanceRoles = new ObservableCollection<GovernanceRole>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _governanceRoles.CollectionChanged += GovernanceRoles_CollectionChanged;
                }
                return _governanceRoles;
            }
            private set
            {
                if (_governanceRoles != null)
                {
                    _governanceRoles.CollectionChanged -= GovernanceRoles_CollectionChanged;
                }
                _governanceRoles = value;
                if (_governanceRoles != null)
                {
                    _governanceRoles.CollectionChanged += GovernanceRoles_CollectionChanged;
                }
            }
        }

        private void GovernanceRoles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<GovernanceRole>())
                {
                    item.ApprovedChanges = this.ChangeLogId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernanceRole;
            _ = this.GovernanceRoles;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
