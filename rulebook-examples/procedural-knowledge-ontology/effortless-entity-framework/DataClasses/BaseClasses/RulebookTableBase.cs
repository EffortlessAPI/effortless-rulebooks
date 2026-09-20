
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
    [Table("RulebookTables")]
    public class RulebookTableBase : SoAEntityBase
    {
        [Key]
        public string RulebookTableId { get; set; }

        public string TableName { get; set; }
        // Formula Name (rulebook: ={{TableName}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.TableName))); set { }
        }

        public string? PhysicalTable { get; set; }
        public string? PhysicalView { get; set; }
        public string? SubjectArea { get; set; }
        public bool? IsExtension { get; set; }
        // Formula FieldCount (rulebook: =COUNTIFS(RulebookFields!{{TargetTable}}, {{RulebookTableId}}))
        [NotMapped]
        public decimal? FieldCount
        {
            get => F.AsDecimal(F.Memo(this, "FieldCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RulebookField>(base.SoAContext, "RulebookFields", __c => __c.RulebookFields), __r => F.CritField(F.Of(__r.TargetTable), F.Of(this.RulebookTableId)))))); set { }
        }

        // Formula PolicyCount (rulebook: =COUNTIFS(AccessPolicies!{{TargetTable}}, {{RulebookTableId}}))
        [NotMapped]
        public decimal? PolicyCount
        {
            get => F.AsDecimal(F.Memo(this, "PolicyCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AccessPolicy>(base.SoAContext, "AccessPolicies", __c => __c.AccessPolicies), __r => F.CritField(F.Of(__r.TargetTable), F.Of(this.RulebookTableId)))))); set { }
        }

        // Formula IsUnsecured (rulebook: ={{PolicyCount}} = 0)
        [NotMapped]
        public bool? IsUnsecured
        {
            get => F.AsBool(F.Memo(this, "IsUnsecured", () => F.Eq(F.Of(this.PolicyCount), F.I(0)))); set { }
        }

        // Formula DisagreeingSubstrateCount (rulebook: =COUNTIFS(TableConformance!{{ImperfectTableKey}}, {{RulebookTableId}}))
        [NotMapped]
        public decimal? DisagreeingSubstrateCount
        {
            get => F.AsDecimal(F.Memo(this, "DisagreeingSubstrateCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TableConformance>(base.SoAContext, "TableConformance", __c => __c.TableConformance), __r => F.CritField(F.Of(__r.ImperfectTableKey), F.Of(this.RulebookTableId)))))); set { }
        }

        public int? MeasuredRowCount { get; set; }
        // Formula HasMeasuredRows (rulebook: ={{MeasuredRowCount}} > 0)
        [NotMapped]
        public bool? HasMeasuredRows
        {
            get => F.AsBool(F.Memo(this, "HasMeasuredRows", () => F.Cmp(F.Nullif(F.Of(this.MeasuredRowCount)), ">", F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        public string? OrganizationLevel { get; set; }
        // Formula SemanticMappingCount (rulebook: =COUNTIFS(SemanticMappings!{{SourcePath}}, {{RulebookTableId}}))
        [NotMapped]
        public int? SemanticMappingCount
        {
            get => F.AsInt(F.Memo(this, "SemanticMappingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SemanticMapping>(base.SoAContext, "SemanticMappings", __c => __c.SemanticMappings), __r => F.CritField(F.Of(__r.SourcePath), F.Of(this.RulebookTableId))))))); set { }
        }

        // Formula MeaningIsOnlyTabular (rulebook: ={{SemanticMappingCount}} = 0)
        [NotMapped]
        public bool? MeaningIsOnlyTabular
        {
            get => F.AsBool(F.Memo(this, "MeaningIsOnlyTabular", () => F.Eq(F.Of(this.SemanticMappingCount), F.I(0)))); set { }
        }

        // Formula ExactMappingCount (rulebook: =COUNTIFS(SemanticMappings!{{SourcePath}}, {{RulebookTableId}}, SemanticMappings!{{MappingRelation}}, "exact"))
        [NotMapped]
        public int? ExactMappingCount
        {
            get => F.AsInt(F.Memo(this, "ExactMappingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SemanticMapping>(base.SoAContext, "SemanticMappings", __c => __c.SemanticMappings), __r => F.CritField(F.Of(__r.SourcePath), F.Of(this.RulebookTableId)) && F.CritLiteral(F.Of(__r.MappingRelation), F.S("exact"))))))); set { }
        }

        // Formula AlignedMappingCount (rulebook: =COUNTIFS(SemanticMappings!{{SourcePath}}, {{RulebookTableId}}, SemanticMappings!{{MappingRelation}}, "aligned"))
        [NotMapped]
        public int? AlignedMappingCount
        {
            get => F.AsInt(F.Memo(this, "AlignedMappingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SemanticMapping>(base.SoAContext, "SemanticMappings", __c => __c.SemanticMappings), __r => F.CritField(F.Of(__r.SourcePath), F.Of(this.RulebookTableId)) && F.CritLiteral(F.Of(__r.MappingRelation), F.S("aligned"))))))); set { }
        }

        // Formula IsUnalignedToStandard (rulebook: =({{ExactMappingCount}} + {{AlignedMappingCount}}) = 0)
        [NotMapped]
        public bool? IsUnalignedToStandard
        {
            get => F.AsBool(F.Memo(this, "IsUnalignedToStandard", () => F.Eq(F.Add(F.Of(this.ExactMappingCount), F.Of(this.AlignedMappingCount)), F.I(0)))); set { }
        }

        // Formula SemanticTypeIriFieldCount (rulebook: =COUNTIFS(RulebookFields!{{TargetTable}}, {{RulebookTableId}}, RulebookFields!{{FieldName}}, "SemanticTypeIri"))
        [NotMapped]
        public int? SemanticTypeIriFieldCount
        {
            get => F.AsInt(F.Memo(this, "SemanticTypeIriFieldCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RulebookField>(base.SoAContext, "RulebookFields", __c => __c.RulebookFields), __r => F.CritField(F.Of(__r.TargetTable), F.Of(this.RulebookTableId)) && F.CritLiteral(F.Of(__r.FieldName), F.S("SemanticTypeIri"))))))); set { }
        }

        // Formula LacksSemanticTypeConvention (rulebook: ={{SemanticTypeIriFieldCount}} = 0)
        [NotMapped]
        public bool? LacksSemanticTypeConvention
        {
            get => F.AsBool(F.Memo(this, "LacksSemanticTypeConvention", () => F.Eq(F.Of(this.SemanticTypeIriFieldCount), F.I(0)))); set { }
        }

        // Formula IsUnsecuredGovernanceRecord (rulebook: =AND({{SubjectArea}} = "governance", {{PolicyCount}} = 0))
        [NotMapped]
        public bool? IsUnsecuredGovernanceRecord
        {
            get => F.AsBool(F.Memo(this, "IsUnsecuredGovernanceRecord", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.SubjectArea)), F.S("governance"))), F.Bool3(F.Eq(F.Of(this.PolicyCount), F.I(0)))))); set { }
        }

        // Formula UnrestrictedNonAdminPolicyCount (rulebook: =COUNTIFS(AccessPolicies!{{TargetTable}}, {{RulebookTableId}}, AccessPolicies!{{IsUnrestrictedNonAdminGrant}}, TRUE))
        [NotMapped]
        public int? UnrestrictedNonAdminPolicyCount
        {
            get => F.AsInt(F.Memo(this, "UnrestrictedNonAdminPolicyCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AccessPolicy>(base.SoAContext, "AccessPolicies", __c => __c.AccessPolicies), __r => F.CritField(F.Of(__r.TargetTable), F.Of(this.RulebookTableId)) && F.CritLiteral(F.Of(__r.IsUnrestrictedNonAdminGrant), F.B(true))))))); set { }
        }

        // Formula RestrictedNonAdminPolicyCount (rulebook: =COUNTIFS(AccessPolicies!{{TargetTable}}, {{RulebookTableId}}, AccessPolicies!{{IsUnrestrictedNonAdminGrant}}, FALSE, AccessPolicies!{{PrincipalIsAdmin}}, FALSE))
        [NotMapped]
        public int? RestrictedNonAdminPolicyCount
        {
            get => F.AsInt(F.Memo(this, "RestrictedNonAdminPolicyCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AccessPolicy>(base.SoAContext, "AccessPolicies", __c => __c.AccessPolicies), __r => F.CritField(F.Of(__r.TargetTable), F.Of(this.RulebookTableId)) && F.CritLiteral(F.Of(__r.IsUnrestrictedNonAdminGrant), F.B(false)) && F.CritLiteral(F.Of(__r.PrincipalIsAdmin), F.B(false))))))); set { }
        }

        // Formula IsReadableInFullByNonAdmin (rulebook: ={{UnrestrictedNonAdminPolicyCount}} > 0)
        [NotMapped]
        public bool? IsReadableInFullByNonAdmin
        {
            get => F.AsBool(F.Memo(this, "IsReadableInFullByNonAdmin", () => F.Cmp(F.Of(this.UnrestrictedNonAdminPolicyCount), ">", F.I(0)))); set { }
        }

        // Formula IsControlledForEveryNonAdmin (rulebook: =AND({{RestrictedNonAdminPolicyCount}} > 0, {{UnrestrictedNonAdminPolicyCount}} = 0))
        [NotMapped]
        public bool? IsControlledForEveryNonAdmin
        {
            get => F.AsBool(F.Memo(this, "IsControlledForEveryNonAdmin", () => F.And(F.Bool3(F.Cmp(F.Of(this.RestrictedNonAdminPolicyCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.UnrestrictedNonAdminPolicyCount), F.I(0)))))); set { }
        }



        private ObservableCollection<AccessPolicy> _accessPolicies;

        [InverseProperty("RulebookTable")]
        public virtual ObservableCollection<AccessPolicy> AccessPolicies
        {
            get
            {
                if (_accessPolicies == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPolicies - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _accessPolicies = new ObservableCollection<AccessPolicy>();
                    }
                    else
                    {
                        var items = base.SoAContext.AccessPolicies.Where(x => x.TargetTable == this.RulebookTableId).ToList<AccessPolicy>();
                        _accessPolicies = new ObservableCollection<AccessPolicy>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _accessPolicies.CollectionChanged += AccessPolicies_CollectionChanged;
                }
                return _accessPolicies;
            }
            private set
            {
                if (_accessPolicies != null)
                {
                    _accessPolicies.CollectionChanged -= AccessPolicies_CollectionChanged;
                }
                _accessPolicies = value;
                if (_accessPolicies != null)
                {
                    _accessPolicies.CollectionChanged += AccessPolicies_CollectionChanged;
                }
            }
        }

        private void AccessPolicies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AccessPolicy>())
                {
                    item.TargetTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<RoleSchemaView> _roleSchemaViews;

        [InverseProperty("RulebookTable")]
        public virtual ObservableCollection<RoleSchemaView> RoleSchemaViews
        {
            get
            {
                if (_roleSchemaViews == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleSchemaViews - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleSchemaViews.Where(x => x.TargetTable == this.RulebookTableId).ToList<RoleSchemaView>();
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roleSchemaViews.CollectionChanged += RoleSchemaViews_CollectionChanged;
                }
                return _roleSchemaViews;
            }
            private set
            {
                if (_roleSchemaViews != null)
                {
                    _roleSchemaViews.CollectionChanged -= RoleSchemaViews_CollectionChanged;
                }
                _roleSchemaViews = value;
                if (_roleSchemaViews != null)
                {
                    _roleSchemaViews.CollectionChanged += RoleSchemaViews_CollectionChanged;
                }
            }
        }

        private void RoleSchemaViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleSchemaView>())
                {
                    item.TargetTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<AccessDenialTest> _accessDenialTests;

        [InverseProperty("RulebookTable")]
        public virtual ObservableCollection<AccessDenialTest> AccessDenialTests
        {
            get
            {
                if (_accessDenialTests == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessDenialTests - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>();
                    }
                    else
                    {
                        var items = base.SoAContext.AccessDenialTests.Where(x => x.TargetTable == this.RulebookTableId).ToList<AccessDenialTest>();
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    item.TargetTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<TableConformance> _tableConformance;

        [InverseProperty("RulebookTableRef")]
        public virtual ObservableCollection<TableConformance> TableConformance
        {
            get
            {
                if (_tableConformance == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TableConformance - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _tableConformance = new ObservableCollection<TableConformance>();
                    }
                    else
                    {
                        var items = base.SoAContext.TableConformance.Where(x => x.RulebookTable == this.RulebookTableId).ToList<TableConformance>();
                        _tableConformance = new ObservableCollection<TableConformance>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _tableConformance.CollectionChanged += TableConformance_CollectionChanged;
                }
                return _tableConformance;
            }
            private set
            {
                if (_tableConformance != null)
                {
                    _tableConformance.CollectionChanged -= TableConformance_CollectionChanged;
                }
                _tableConformance = value;
                if (_tableConformance != null)
                {
                    _tableConformance.CollectionChanged += TableConformance_CollectionChanged;
                }
            }
        }

        private void TableConformance_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TableConformance>())
                {
                    item.RulebookTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<ClaimEvidence> _claimEvidence;

        [InverseProperty("RulebookTableRef")]
        public virtual ObservableCollection<ClaimEvidence> ClaimEvidence
        {
            get
            {
                if (_claimEvidence == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ClaimEvidence - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _claimEvidence = new ObservableCollection<ClaimEvidence>();
                    }
                    else
                    {
                        var items = base.SoAContext.ClaimEvidence.Where(x => x.RulebookTable == this.RulebookTableId).ToList<ClaimEvidence>();
                        _claimEvidence = new ObservableCollection<ClaimEvidence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _claimEvidence.CollectionChanged += ClaimEvidence_CollectionChanged;
                }
                return _claimEvidence;
            }
            private set
            {
                if (_claimEvidence != null)
                {
                    _claimEvidence.CollectionChanged -= ClaimEvidence_CollectionChanged;
                }
                _claimEvidence = value;
                if (_claimEvidence != null)
                {
                    _claimEvidence.CollectionChanged += ClaimEvidence_CollectionChanged;
                }
            }
        }

        private void ClaimEvidence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ClaimEvidence>())
                {
                    item.RulebookTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<ChangeImpactFinding> _changeImpactFindings;

        [InverseProperty("RulebookTable")]
        public virtual ObservableCollection<ChangeImpactFinding> ChangeImpactFindings
        {
            get
            {
                if (_changeImpactFindings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeImpactFindings - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _changeImpactFindings = new ObservableCollection<ChangeImpactFinding>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeImpactFindings.Where(x => x.AffectedTable == this.RulebookTableId).ToList<ChangeImpactFinding>();
                        _changeImpactFindings = new ObservableCollection<ChangeImpactFinding>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _changeImpactFindings.CollectionChanged += ChangeImpactFindings_CollectionChanged;
                }
                return _changeImpactFindings;
            }
            private set
            {
                if (_changeImpactFindings != null)
                {
                    _changeImpactFindings.CollectionChanged -= ChangeImpactFindings_CollectionChanged;
                }
                _changeImpactFindings = value;
                if (_changeImpactFindings != null)
                {
                    _changeImpactFindings.CollectionChanged += ChangeImpactFindings_CollectionChanged;
                }
            }
        }

        private void ChangeImpactFindings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeImpactFinding>())
                {
                    item.AffectedTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<ExpansionConceptFit> _expansionConceptFits;

        [InverseProperty("RulebookTable")]
        public virtual ObservableCollection<ExpansionConceptFit> ExpansionConceptFits
        {
            get
            {
                if (_expansionConceptFits == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExpansionConceptFits - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _expansionConceptFits = new ObservableCollection<ExpansionConceptFit>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExpansionConceptFits.Where(x => x.CoveringTable == this.RulebookTableId).ToList<ExpansionConceptFit>();
                        _expansionConceptFits = new ObservableCollection<ExpansionConceptFit>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _expansionConceptFits.CollectionChanged += ExpansionConceptFits_CollectionChanged;
                }
                return _expansionConceptFits;
            }
            private set
            {
                if (_expansionConceptFits != null)
                {
                    _expansionConceptFits.CollectionChanged -= ExpansionConceptFits_CollectionChanged;
                }
                _expansionConceptFits = value;
                if (_expansionConceptFits != null)
                {
                    _expansionConceptFits.CollectionChanged += ExpansionConceptFits_CollectionChanged;
                }
            }
        }

        private void ExpansionConceptFits_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExpansionConceptFit>())
                {
                    item.CoveringTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<TermDefinition> _termDefinitions;

        [InverseProperty("RulebookTableRef")]
        public virtual ObservableCollection<TermDefinition> TermDefinitions
        {
            get
            {
                if (_termDefinitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TermDefinitions - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _termDefinitions = new ObservableCollection<TermDefinition>();
                    }
                    else
                    {
                        var items = base.SoAContext.TermDefinitions.Where(x => x.RulebookTable == this.RulebookTableId).ToList<TermDefinition>();
                        _termDefinitions = new ObservableCollection<TermDefinition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _termDefinitions.CollectionChanged += TermDefinitions_CollectionChanged;
                }
                return _termDefinitions;
            }
            private set
            {
                if (_termDefinitions != null)
                {
                    _termDefinitions.CollectionChanged -= TermDefinitions_CollectionChanged;
                }
                _termDefinitions = value;
                if (_termDefinitions != null)
                {
                    _termDefinitions.CollectionChanged += TermDefinitions_CollectionChanged;
                }
            }
        }

        private void TermDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TermDefinition>())
                {
                    item.RulebookTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<DomainCoverageArea> _domainCoverageAreas;

        [InverseProperty("RulebookTable")]
        public virtual ObservableCollection<DomainCoverageArea> DomainCoverageAreas
        {
            get
            {
                if (_domainCoverageAreas == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DomainCoverageAreas - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _domainCoverageAreas = new ObservableCollection<DomainCoverageArea>();
                    }
                    else
                    {
                        var items = base.SoAContext.DomainCoverageAreas.Where(x => x.CoveringTable == this.RulebookTableId).ToList<DomainCoverageArea>();
                        _domainCoverageAreas = new ObservableCollection<DomainCoverageArea>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _domainCoverageAreas.CollectionChanged += DomainCoverageAreas_CollectionChanged;
                }
                return _domainCoverageAreas;
            }
            private set
            {
                if (_domainCoverageAreas != null)
                {
                    _domainCoverageAreas.CollectionChanged -= DomainCoverageAreas_CollectionChanged;
                }
                _domainCoverageAreas = value;
                if (_domainCoverageAreas != null)
                {
                    _domainCoverageAreas.CollectionChanged += DomainCoverageAreas_CollectionChanged;
                }
            }
        }

        private void DomainCoverageAreas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DomainCoverageArea>())
                {
                    item.CoveringTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<ModelChangeLogEntry> _modelChangeLogEntries;

        [InverseProperty("RulebookTable")]
        public virtual ObservableCollection<ModelChangeLogEntry> ModelChangeLogEntries
        {
            get
            {
                if (_modelChangeLogEntries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelChangeLogEntries - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _modelChangeLogEntries = new ObservableCollection<ModelChangeLogEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeLogEntries.Where(x => x.AffectedTable == this.RulebookTableId).ToList<ModelChangeLogEntry>();
                        _modelChangeLogEntries = new ObservableCollection<ModelChangeLogEntry>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelChangeLogEntries.CollectionChanged += ModelChangeLogEntries_CollectionChanged;
                }
                return _modelChangeLogEntries;
            }
            private set
            {
                if (_modelChangeLogEntries != null)
                {
                    _modelChangeLogEntries.CollectionChanged -= ModelChangeLogEntries_CollectionChanged;
                }
                _modelChangeLogEntries = value;
                if (_modelChangeLogEntries != null)
                {
                    _modelChangeLogEntries.CollectionChanged += ModelChangeLogEntries_CollectionChanged;
                }
            }
        }

        private void ModelChangeLogEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelChangeLogEntry>())
                {
                    item.AffectedTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<AppAction> _appActions;

        [InverseProperty("RulebookTable")]
        public virtual ObservableCollection<AppAction> AppActions
        {
            get
            {
                if (_appActions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppActions - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _appActions = new ObservableCollection<AppAction>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppActions.Where(x => x.TargetTable == this.RulebookTableId).ToList<AppAction>();
                        _appActions = new ObservableCollection<AppAction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _appActions.CollectionChanged += AppActions_CollectionChanged;
                }
                return _appActions;
            }
            private set
            {
                if (_appActions != null)
                {
                    _appActions.CollectionChanged -= AppActions_CollectionChanged;
                }
                _appActions = value;
                if (_appActions != null)
                {
                    _appActions.CollectionChanged += AppActions_CollectionChanged;
                }
            }
        }

        private void AppActions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppAction>())
                {
                    item.TargetTable = this.RulebookTableId;
                }
            }
        }

        private ObservableCollection<AbundantKnowledgeGap> _abundantKnowledgeGaps;

        [InverseProperty("RulebookTable")]
        public virtual ObservableCollection<AbundantKnowledgeGap> AbundantKnowledgeGaps
        {
            get
            {
                if (_abundantKnowledgeGaps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AbundantKnowledgeGaps - no database context is set. RulebookTableId: " + this.RulebookTableId + ".");
                        }
                        _abundantKnowledgeGaps = new ObservableCollection<AbundantKnowledgeGap>();
                    }
                    else
                    {
                        var items = base.SoAContext.AbundantKnowledgeGaps.Where(x => x.RepresentedByTable == this.RulebookTableId).ToList<AbundantKnowledgeGap>();
                        _abundantKnowledgeGaps = new ObservableCollection<AbundantKnowledgeGap>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _abundantKnowledgeGaps.CollectionChanged += AbundantKnowledgeGaps_CollectionChanged;
                }
                return _abundantKnowledgeGaps;
            }
            private set
            {
                if (_abundantKnowledgeGaps != null)
                {
                    _abundantKnowledgeGaps.CollectionChanged -= AbundantKnowledgeGaps_CollectionChanged;
                }
                _abundantKnowledgeGaps = value;
                if (_abundantKnowledgeGaps != null)
                {
                    _abundantKnowledgeGaps.CollectionChanged += AbundantKnowledgeGaps_CollectionChanged;
                }
            }
        }

        private void AbundantKnowledgeGaps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AbundantKnowledgeGap>())
                {
                    item.RepresentedByTable = this.RulebookTableId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AccessPolicies;
            _ = this.RoleSchemaViews;
            _ = this.AccessDenialTests;
            _ = this.TableConformance;
            _ = this.ClaimEvidence;
            _ = this.ChangeImpactFindings;
            _ = this.ExpansionConceptFits;
            _ = this.TermDefinitions;
            _ = this.DomainCoverageAreas;
            _ = this.ModelChangeLogEntries;
            _ = this.AppActions;
            _ = this.AbundantKnowledgeGaps;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
