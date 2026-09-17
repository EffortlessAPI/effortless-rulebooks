
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
    [Table("CorporateGovernancePrograms")]
    public class CorporateGovernanceProgramBase : SoAEntityBase
    {
        [Key]
        public string CorporateGovernanceProgramId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? SponsorDiscipline { get; set; }
        public DateTimeOffset? LaunchedAt { get; set; }
        public bool? CoversCompliance { get; set; }
        public bool? CoversDataQuality { get; set; }
        public bool? CoversInformationManagement { get; set; }
        public bool? CoversKnowledgeManagement { get; set; }
        // Formula IsComplianceOnly (rulebook: =AND({{CoversCompliance}}, {{CoversDataQuality}} = FALSE, {{CoversInformationManagement}} = FALSE, {{CoversKnowledgeManagement}} = FALSE))
        [NotMapped]
        public bool? IsComplianceOnly
        {
            get => F.AsBool(F.Memo(this, "IsComplianceOnly", () => F.And(F.IsTrueV(F.Of(this.CoversCompliance)), F.Bool3(F.Eq(F.Nullif(F.Of(this.CoversDataQuality)), F.B(false))), F.Bool3(F.Eq(F.Nullif(F.Of(this.CoversInformationManagement)), F.B(false))), F.Bool3(F.Eq(F.Nullif(F.Of(this.CoversKnowledgeManagement)), F.B(false)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }

        private Organization _organizationRef;

        [ForeignKey("Organization")]
        public virtual Organization OrganizationRef
        {
            get
            {
                if (_organizationRef == null && !string.IsNullOrEmpty(Organization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRef - no database context is set. Organization: " + Organization + ".");
                        }
                        return null;
                    }
                    _organizationRef = base.SoAContext.Organizations.Find(Organization);
                    if (_organizationRef != null)
                    {
                        base.SoAContext.Attach(_organizationRef);
                    }
                }
                return _organizationRef;
            }
            set
            {
                if (_organizationRef != value)
                {
                    _organizationRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organizationRef != null)
                    {
                        Organization = _organizationRef.OrganizationId;
                    }
                }
            }
        }

        private ObservableCollection<RecordsRetentionPolicy> _recordsRetentionPolicies;

        [InverseProperty("CorporateGovernanceProgram")]
        public virtual ObservableCollection<RecordsRetentionPolicy> RecordsRetentionPolicies
        {
            get
            {
                if (_recordsRetentionPolicies == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RecordsRetentionPolicies - no database context is set. CorporateGovernanceProgramId: " + this.CorporateGovernanceProgramId + ".");
                        }
                        _recordsRetentionPolicies = new ObservableCollection<RecordsRetentionPolicy>();
                    }
                    else
                    {
                        var items = base.SoAContext.RecordsRetentionPolicies.Where(x => x.GovernanceProgram == this.CorporateGovernanceProgramId).ToList<RecordsRetentionPolicy>();
                        _recordsRetentionPolicies = new ObservableCollection<RecordsRetentionPolicy>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _recordsRetentionPolicies.CollectionChanged += RecordsRetentionPolicies_CollectionChanged;
                }
                return _recordsRetentionPolicies;
            }
            private set
            {
                if (_recordsRetentionPolicies != null)
                {
                    _recordsRetentionPolicies.CollectionChanged -= RecordsRetentionPolicies_CollectionChanged;
                }
                _recordsRetentionPolicies = value;
                if (_recordsRetentionPolicies != null)
                {
                    _recordsRetentionPolicies.CollectionChanged += RecordsRetentionPolicies_CollectionChanged;
                }
            }
        }

        private void RecordsRetentionPolicies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RecordsRetentionPolicy>())
                {
                    item.GovernanceProgram = this.CorporateGovernanceProgramId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.RecordsRetentionPolicies;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
