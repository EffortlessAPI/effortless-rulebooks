
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
    [Table("GovernanceRoles")]
    public class GovernanceRoleBase : SoAEntityBase
    {
        [Key]
        public string GovernanceRoleId { get; set; }

        // Formula RelativePath (rulebook: ="governance-roles/" & {{GovernanceRoleId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("governance-roles/"), F.TextOr(F.Of(this.GovernanceRoleId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        // Formula Name (rulebook: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-"))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Substitute(F.Lower(F.Of(this.DisplayName)), F.S(" "), F.S("-")))); set { }
        }

        public string? DisplayName { get; set; }
        public string? Kind { get; set; }
        public string? Responsibilities { get; set; }
        public string? ApprovalScope { get; set; }
        public string? HeldBy { get; set; }
        // Formula CanApproveChanges (rulebook: ={{Kind}} = "Authority")
        [NotMapped]
        public bool? CanApproveChanges
        {
            get => F.AsBool(F.Memo(this, "CanApproveChanges", () => F.Eq(F.Nullif(F.Of(this.Kind)), F.S("Authority")))); set { }
        }


        public string? ApprovedChanges { get; set; }

        private ChangeLog _changeLog;

        [ForeignKey("ApprovedChanges")]
        public virtual ChangeLog ChangeLog
        {
            get
            {
                if (_changeLog == null && !string.IsNullOrEmpty(ApprovedChanges))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeLog - no database context is set. ApprovedChanges: " + ApprovedChanges + ".");
                        }
                        return null;
                    }
                    _changeLog = base.SoAContext.ChangeLog.Find(ApprovedChanges);
                    if (_changeLog != null)
                    {
                        base.SoAContext.Attach(_changeLog);
                    }
                }
                return _changeLog;
            }
            set
            {
                if (_changeLog != value)
                {
                    _changeLog = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_changeLog != null)
                    {
                        ApprovedChanges = _changeLog.ChangeLogId;
                    }
                }
            }
        }

        private ObservableCollection<ChangeLog> _approvedByChangeLog;

        [InverseProperty("GovernanceRole")]
        public virtual ObservableCollection<ChangeLog> ApprovedByChangeLog
        {
            get
            {
                if (_approvedByChangeLog == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApprovedByChangeLog - no database context is set. GovernanceRoleId: " + this.GovernanceRoleId + ".");
                        }
                        _approvedByChangeLog = new ObservableCollection<ChangeLog>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeLog.Where(x => x.ApprovedBy == this.GovernanceRoleId).ToList<ChangeLog>();
                        _approvedByChangeLog = new ObservableCollection<ChangeLog>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _approvedByChangeLog.CollectionChanged += ApprovedByChangeLog_CollectionChanged;
                }
                return _approvedByChangeLog;
            }
            private set
            {
                if (_approvedByChangeLog != null)
                {
                    _approvedByChangeLog.CollectionChanged -= ApprovedByChangeLog_CollectionChanged;
                }
                _approvedByChangeLog = value;
                if (_approvedByChangeLog != null)
                {
                    _approvedByChangeLog.CollectionChanged += ApprovedByChangeLog_CollectionChanged;
                }
            }
        }

        private void ApprovedByChangeLog_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeLog>())
                {
                    item.ApprovedBy = this.GovernanceRoleId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ChangeLog;
            _ = this.ApprovedByChangeLog;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
