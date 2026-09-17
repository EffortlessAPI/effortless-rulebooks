
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
    [Table("DepartmentProcessAccounts")]
    public class DepartmentProcessAccountBase : SoAEntityBase
    {
        [Key]
        public string DepartmentProcessAccountId { get; set; }

        // Formula Name (rulebook: ={{Department}} & " on " & {{Procedure}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Department)), F.S(" on "), F.Text(F.Of(this.Procedure))))); set { }
        }

        public DateTimeOffset? EngagedAt { get; set; }
        public string? AccountSummary { get; set; }
        public string? ShapingInterest { get; set; }
        public string? ResolutionPractice { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
        // Formula ConflictingDepartment (rulebook: =INDEX(DepartmentProcessAccounts!{{Department}}, MATCH({{ConflictsWithAccount}}, DepartmentProcessAccounts!{{DepartmentProcessAccountId}}, 0)))
        [NotMapped]
        public string? ConflictingDepartment
        {
            get => F.AsString(F.Memo(this, "ConflictingDepartment", () => F.Lookup<DepartmentProcessAccount>(this, "DepartmentProcessAccounts", "DepartmentProcessAccountId", __c => __c.DepartmentProcessAccounts, __r => F.Of(__r.DepartmentProcessAccountId), F.Of(this.ConflictsWithAccount), __r => F.Of(__r.Department), () => F.Of(new DepartmentProcessAccount().Department)))); set { }
        }

        // Formula IsAwaitingEngagement (rulebook: ={{EngagedAt}} = "")
        [NotMapped]
        public bool? IsAwaitingEngagement
        {
            get => F.AsBool(F.Memo(this, "IsAwaitingEngagement", () => F.IsBlank(F.Of(this.EngagedAt)))); set { }
        }

        // Formula IsConflictingAccount (rulebook: =AND({{ConflictsWithAccount}} <> "", {{ConflictingDepartment}} <> {{Department}}))
        [NotMapped]
        public bool? IsConflictingAccount
        {
            get => F.AsBool(F.Memo(this, "IsConflictingAccount", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ConflictsWithAccount))), F.Bool3(F.Ne(F.Of(this.ConflictingDepartment), F.Nullif(F.Of(this.Department))))))); set { }
        }

        // Formula IsUnresolvedDisagreement (rulebook: =AND({{IsConflictingAccount}}, {{ResolutionPractice}} = ""))
        [NotMapped]
        public bool? IsUnresolvedDisagreement
        {
            get => F.AsBool(F.Memo(this, "IsUnresolvedDisagreement", () => F.And(F.Bool3(F.Of(this.IsConflictingAccount)), F.Bool3(F.IsBlank(F.Of(this.ResolutionPractice)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
        public string? Department { get; set; }
        public string? StakeholderAgent { get; set; }
        public string? ConflictsWithAccount { get; set; }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }

        private Organization _organization;

        [ForeignKey("Department")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(Department))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. Department: " + Department + ".");
                        }
                        return null;
                    }
                    _organization = base.SoAContext.Organizations.Find(Department);
                    if (_organization != null)
                    {
                        base.SoAContext.Attach(_organization);
                    }
                }
                return _organization;
            }
            set
            {
                if (_organization != value)
                {
                    _organization = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organization != null)
                    {
                        Department = _organization.OrganizationId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("StakeholderAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(StakeholderAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. StakeholderAgent: " + StakeholderAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(StakeholderAgent);
                    if (_agent != null)
                    {
                        base.SoAContext.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agent != null)
                    {
                        StakeholderAgent = _agent.AgentId;
                    }
                }
            }
        }

        private DepartmentProcessAccount _departmentProcessAccount;

        [ForeignKey("ConflictsWithAccount")]
        public virtual DepartmentProcessAccount DepartmentProcessAccount
        {
            get
            {
                if (_departmentProcessAccount == null && !string.IsNullOrEmpty(ConflictsWithAccount))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DepartmentProcessAccount - no database context is set. ConflictsWithAccount: " + ConflictsWithAccount + ".");
                        }
                        return null;
                    }
                    _departmentProcessAccount = base.SoAContext.DepartmentProcessAccounts.Find(ConflictsWithAccount);
                    if (_departmentProcessAccount != null)
                    {
                        base.SoAContext.Attach(_departmentProcessAccount);
                    }
                }
                return _departmentProcessAccount;
            }
            set
            {
                if (_departmentProcessAccount != value)
                {
                    _departmentProcessAccount = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_departmentProcessAccount != null)
                    {
                        ConflictsWithAccount = _departmentProcessAccount.DepartmentProcessAccountId;
                    }
                }
            }
        }

        private ObservableCollection<DepartmentProcessAccount> _departmentProcessAccounts;

        [InverseProperty("DepartmentProcessAccount")]
        public virtual ObservableCollection<DepartmentProcessAccount> DepartmentProcessAccounts
        {
            get
            {
                if (_departmentProcessAccounts == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DepartmentProcessAccounts - no database context is set. DepartmentProcessAccountId: " + this.DepartmentProcessAccountId + ".");
                        }
                        _departmentProcessAccounts = new ObservableCollection<DepartmentProcessAccount>();
                    }
                    else
                    {
                        var items = base.SoAContext.DepartmentProcessAccounts.Where(x => x.ConflictsWithAccount == this.DepartmentProcessAccountId).ToList<DepartmentProcessAccount>();
                        _departmentProcessAccounts = new ObservableCollection<DepartmentProcessAccount>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _departmentProcessAccounts.CollectionChanged += DepartmentProcessAccounts_CollectionChanged;
                }
                return _departmentProcessAccounts;
            }
            private set
            {
                if (_departmentProcessAccounts != null)
                {
                    _departmentProcessAccounts.CollectionChanged -= DepartmentProcessAccounts_CollectionChanged;
                }
                _departmentProcessAccounts = value;
                if (_departmentProcessAccounts != null)
                {
                    _departmentProcessAccounts.CollectionChanged += DepartmentProcessAccounts_CollectionChanged;
                }
            }
        }

        private void DepartmentProcessAccounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DepartmentProcessAccount>())
                {
                    item.ConflictsWithAccount = this.DepartmentProcessAccountId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.Organization;
            _ = this.Agent;
            _ = this.DepartmentProcessAccount;
            _ = this.DepartmentProcessAccounts;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
