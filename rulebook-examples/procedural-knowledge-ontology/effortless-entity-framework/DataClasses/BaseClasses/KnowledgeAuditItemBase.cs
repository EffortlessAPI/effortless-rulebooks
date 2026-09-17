
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
    [Table("KnowledgeAuditItems")]
    public class KnowledgeAuditItemBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeAuditItemId { get; set; }

        // Formula Name (rulebook: ={{KnowledgeAudit}} & " / " & {{KnowledgeArea}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.KnowledgeAudit)), F.S(" / "), F.Text(F.Of(this.KnowledgeArea))))); set { }
        }

        public string? KnowledgeArea { get; set; }
        public string? KnowledgeKind { get; set; }
        public int? NeededLevel { get; set; }
        public int? HeldInternalLevel { get; set; }
        public int? ProviderHeldLevel { get; set; }
        public int? InternalHoldingTeamCount { get; set; }
        // Formula ClientOrganization (rulebook: =INDEX(KnowledgeAudits!{{Organization}}, MATCH({{KnowledgeAudit}}, KnowledgeAudits!{{KnowledgeAuditId}}, 0)))
        [NotMapped]
        public string? ClientOrganization
        {
            get => F.AsString(F.Memo(this, "ClientOrganization", () => F.Lookup<KnowledgeAudit>(this, "KnowledgeAudits", "KnowledgeAuditId", __c => __c.KnowledgeAudits, __r => F.Of(__r.KnowledgeAuditId), F.Of(this.KnowledgeAudit), __r => F.Of(__r.Organization), () => F.Of(new KnowledgeAudit().Organization)))); set { }
        }

        // Formula HasInternalShortfall (rulebook: ={{HeldInternalLevel}} < {{NeededLevel}})
        [NotMapped]
        public bool? HasInternalShortfall
        {
            get => F.AsBool(F.Memo(this, "HasInternalShortfall", () => F.Cmp(F.Nullif(F.Of(this.HeldInternalLevel)), "<", F.Nullif(F.Of(this.NeededLevel))))); set { }
        }

        // Formula IsKnowledgeDependency (rulebook: =AND({{HasInternalShortfall}}, {{ProviderHoldingKnowledge}} <> "", {{ProviderHeldLevel}} >= {{NeededLevel}}))
        [NotMapped]
        public bool? IsKnowledgeDependency
        {
            get => F.AsBool(F.Memo(this, "IsKnowledgeDependency", () => F.And(F.Bool3(F.Of(this.HasInternalShortfall)), F.Bool3(F.IsNotBlank(F.Of(this.ProviderHoldingKnowledge))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ProviderHeldLevel)), ">=", F.Nullif(F.Of(this.NeededLevel))))))); set { }
        }

        // Formula IsCoverageGap (rulebook: =AND({{HasInternalShortfall}}, NOT({{IsKnowledgeDependency}})))
        [NotMapped]
        public bool? IsCoverageGap
        {
            get => F.AsBool(F.Memo(this, "IsCoverageGap", () => F.And(F.Bool3(F.Of(this.HasInternalShortfall)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsKnowledgeDependency))))))); set { }
        }

        // Formula IsUnnamedFinding (rulebook: =AND({{HasInternalShortfall}}, {{NamedKnowledgeGap}} = ""))
        [NotMapped]
        public bool? IsUnnamedFinding
        {
            get => F.AsBool(F.Memo(this, "IsUnnamedFinding", () => F.And(F.Bool3(F.Of(this.HasInternalShortfall)), F.Bool3(F.IsBlank(F.Of(this.NamedKnowledgeGap)))))); set { }
        }

        // Formula IsSingleTeamSilo (rulebook: =AND({{HeldInternalLevel}} >= {{NeededLevel}}, {{InternalHoldingTeamCount}} = 1))
        [NotMapped]
        public bool? IsSingleTeamSilo
        {
            get => F.AsBool(F.Memo(this, "IsSingleTeamSilo", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.HeldInternalLevel)), ">=", F.Nullif(F.Of(this.NeededLevel)))), F.Bool3(F.Eq(F.Nullif(F.Of(this.InternalHoldingTeamCount)), F.I(1)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? KnowledgeAudit { get; set; }
        public string? SourcingFunction { get; set; }
        public string? ProviderHoldingKnowledge { get; set; }
        public string? NamedKnowledgeGap { get; set; }

        private KnowledgeAudit _knowledgeAuditRef;

        [ForeignKey("KnowledgeAudit")]
        public virtual KnowledgeAudit KnowledgeAuditRef
        {
            get
            {
                if (_knowledgeAuditRef == null && !string.IsNullOrEmpty(KnowledgeAudit))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeAuditRef - no database context is set. KnowledgeAudit: " + KnowledgeAudit + ".");
                        }
                        return null;
                    }
                    _knowledgeAuditRef = base.SoAContext.KnowledgeAudits.Find(KnowledgeAudit);
                    if (_knowledgeAuditRef != null)
                    {
                        base.SoAContext.Attach(_knowledgeAuditRef);
                    }
                }
                return _knowledgeAuditRef;
            }
            set
            {
                if (_knowledgeAuditRef != value)
                {
                    _knowledgeAuditRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeAuditRef != null)
                    {
                        KnowledgeAudit = _knowledgeAuditRef.KnowledgeAuditId;
                    }
                }
            }
        }

        private SourcingFunction _sourcingFunctionRef;

        [ForeignKey("SourcingFunction")]
        public virtual SourcingFunction SourcingFunctionRef
        {
            get
            {
                if (_sourcingFunctionRef == null && !string.IsNullOrEmpty(SourcingFunction))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SourcingFunctionRef - no database context is set. SourcingFunction: " + SourcingFunction + ".");
                        }
                        return null;
                    }
                    _sourcingFunctionRef = base.SoAContext.SourcingFunctions.Find(SourcingFunction);
                    if (_sourcingFunctionRef != null)
                    {
                        base.SoAContext.Attach(_sourcingFunctionRef);
                    }
                }
                return _sourcingFunctionRef;
            }
            set
            {
                if (_sourcingFunctionRef != value)
                {
                    _sourcingFunctionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_sourcingFunctionRef != null)
                    {
                        SourcingFunction = _sourcingFunctionRef.SourcingFunctionId;
                    }
                }
            }
        }

        private Organization _organization;

        [ForeignKey("ProviderHoldingKnowledge")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(ProviderHoldingKnowledge))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. ProviderHoldingKnowledge: " + ProviderHoldingKnowledge + ".");
                        }
                        return null;
                    }
                    _organization = base.SoAContext.Organizations.Find(ProviderHoldingKnowledge);
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
                        ProviderHoldingKnowledge = _organization.OrganizationId;
                    }
                }
            }
        }

        private KnowledgeGap _knowledgeGap;

        [ForeignKey("NamedKnowledgeGap")]
        public virtual KnowledgeGap KnowledgeGap
        {
            get
            {
                if (_knowledgeGap == null && !string.IsNullOrEmpty(NamedKnowledgeGap))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeGap - no database context is set. NamedKnowledgeGap: " + NamedKnowledgeGap + ".");
                        }
                        return null;
                    }
                    _knowledgeGap = base.SoAContext.KnowledgeGaps.Find(NamedKnowledgeGap);
                    if (_knowledgeGap != null)
                    {
                        base.SoAContext.Attach(_knowledgeGap);
                    }
                }
                return _knowledgeGap;
            }
            set
            {
                if (_knowledgeGap != value)
                {
                    _knowledgeGap = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeGap != null)
                    {
                        NamedKnowledgeGap = _knowledgeGap.KnowledgeGapId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.KnowledgeAuditRef;
            _ = this.SourcingFunctionRef;
            _ = this.Organization;
            _ = this.KnowledgeGap;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
