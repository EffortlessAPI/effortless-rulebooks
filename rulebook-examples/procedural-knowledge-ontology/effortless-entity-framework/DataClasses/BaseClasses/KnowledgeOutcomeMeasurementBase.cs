
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
    [Table("KnowledgeOutcomeMeasurements")]
    public class KnowledgeOutcomeMeasurementBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeOutcomeMeasurementId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? MeasuredGroup { get; set; }
        public string? KnowledgeScope { get; set; }
        public DateTimeOffset? MeasuredAt { get; set; }
        public decimal? KnowledgeAccessScore { get; set; }
        public decimal? ErrorRatePercent { get; set; }
        public decimal? MinutesPerRun { get; set; }
        public decimal? SatisfactionScore { get; set; }
        public decimal? EfficiencyGainPercent { get; set; }
        public decimal? TargetErrorRatePercent { get; set; }
        public string? InformedInvestmentDecision { get; set; }
        // Formula BaselineAccessScore (rulebook: =INDEX(KnowledgeOutcomeMeasurements!{{KnowledgeAccessScore}}, MATCH({{ComparisonBaseline}}, KnowledgeOutcomeMeasurements!{{KnowledgeOutcomeMeasurementId}}, 0)))
        [NotMapped]
        public decimal? BaselineAccessScore
        {
            get => F.AsDecimal(F.Memo(this, "BaselineAccessScore", () => F.Lookup<KnowledgeOutcomeMeasurement>(this, "KnowledgeOutcomeMeasurements", "KnowledgeOutcomeMeasurementId", __c => __c.KnowledgeOutcomeMeasurements, __r => F.Of(__r.KnowledgeOutcomeMeasurementId), F.Of(this.ComparisonBaseline), __r => F.Of(__r.KnowledgeAccessScore), () => F.Of(new KnowledgeOutcomeMeasurement().KnowledgeAccessScore)))); set { }
        }

        // Formula BaselineErrorRate (rulebook: =INDEX(KnowledgeOutcomeMeasurements!{{ErrorRatePercent}}, MATCH({{ComparisonBaseline}}, KnowledgeOutcomeMeasurements!{{KnowledgeOutcomeMeasurementId}}, 0)))
        [NotMapped]
        public decimal? BaselineErrorRate
        {
            get => F.AsDecimal(F.Memo(this, "BaselineErrorRate", () => F.Lookup<KnowledgeOutcomeMeasurement>(this, "KnowledgeOutcomeMeasurements", "KnowledgeOutcomeMeasurementId", __c => __c.KnowledgeOutcomeMeasurements, __r => F.Of(__r.KnowledgeOutcomeMeasurementId), F.Of(this.ComparisonBaseline), __r => F.Of(__r.ErrorRatePercent), () => F.Of(new KnowledgeOutcomeMeasurement().ErrorRatePercent)))); set { }
        }

        // Formula BaselineMinutesPerRun (rulebook: =INDEX(KnowledgeOutcomeMeasurements!{{MinutesPerRun}}, MATCH({{ComparisonBaseline}}, KnowledgeOutcomeMeasurements!{{KnowledgeOutcomeMeasurementId}}, 0)))
        [NotMapped]
        public decimal? BaselineMinutesPerRun
        {
            get => F.AsDecimal(F.Memo(this, "BaselineMinutesPerRun", () => F.Lookup<KnowledgeOutcomeMeasurement>(this, "KnowledgeOutcomeMeasurements", "KnowledgeOutcomeMeasurementId", __c => __c.KnowledgeOutcomeMeasurements, __r => F.Of(__r.KnowledgeOutcomeMeasurementId), F.Of(this.ComparisonBaseline), __r => F.Of(__r.MinutesPerRun), () => F.Of(new KnowledgeOutcomeMeasurement().MinutesPerRun)))); set { }
        }

        // Formula BaselineSatisfaction (rulebook: =INDEX(KnowledgeOutcomeMeasurements!{{SatisfactionScore}}, MATCH({{ComparisonBaseline}}, KnowledgeOutcomeMeasurements!{{KnowledgeOutcomeMeasurementId}}, 0)))
        [NotMapped]
        public decimal? BaselineSatisfaction
        {
            get => F.AsDecimal(F.Memo(this, "BaselineSatisfaction", () => F.Lookup<KnowledgeOutcomeMeasurement>(this, "KnowledgeOutcomeMeasurements", "KnowledgeOutcomeMeasurementId", __c => __c.KnowledgeOutcomeMeasurements, __r => F.Of(__r.KnowledgeOutcomeMeasurementId), F.Of(this.ComparisonBaseline), __r => F.Of(__r.SatisfactionScore), () => F.Of(new KnowledgeOutcomeMeasurement().SatisfactionScore)))); set { }
        }

        // Formula HigherAccessFewerErrors (rulebook: =AND({{ComparisonBaseline}} <> "", {{KnowledgeAccessScore}} > {{BaselineAccessScore}}, {{ErrorRatePercent}} < {{BaselineErrorRate}}))
        [NotMapped]
        public bool? HigherAccessFewerErrors
        {
            get => F.AsBool(F.Memo(this, "HigherAccessFewerErrors", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ComparisonBaseline))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.KnowledgeAccessScore)), ">", F.Of(this.BaselineAccessScore))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ErrorRatePercent)), "<", F.Of(this.BaselineErrorRate)))))); set { }
        }

        // Formula HigherAccessMoreEfficient (rulebook: =AND({{ComparisonBaseline}} <> "", {{KnowledgeAccessScore}} > {{BaselineAccessScore}}, {{MinutesPerRun}} < {{BaselineMinutesPerRun}}))
        [NotMapped]
        public bool? HigherAccessMoreEfficient
        {
            get => F.AsBool(F.Memo(this, "HigherAccessMoreEfficient", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ComparisonBaseline))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.KnowledgeAccessScore)), ">", F.Of(this.BaselineAccessScore))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.MinutesPerRun)), "<", F.Of(this.BaselineMinutesPerRun)))))); set { }
        }

        // Formula HigherAccessMoreSatisfied (rulebook: =AND({{ComparisonBaseline}} <> "", {{KnowledgeAccessScore}} > {{BaselineAccessScore}}, {{SatisfactionScore}} > {{BaselineSatisfaction}}))
        [NotMapped]
        public bool? HigherAccessMoreSatisfied
        {
            get => F.AsBool(F.Memo(this, "HigherAccessMoreSatisfied", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ComparisonBaseline))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.KnowledgeAccessScore)), ">", F.Of(this.BaselineAccessScore))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.SatisfactionScore)), ">", F.Of(this.BaselineSatisfaction)))))); set { }
        }

        // Formula IsUnactedAdverseOutcome (rulebook: =AND({{ErrorRatePercent}} > {{TargetErrorRatePercent}}, {{InformedChangeRequest}} = "", {{InformedInvestmentDecision}} = ""))
        [NotMapped]
        public bool? IsUnactedAdverseOutcome
        {
            get => F.AsBool(F.Memo(this, "IsUnactedAdverseOutcome", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ErrorRatePercent)), ">", F.Nullif(F.Of(this.TargetErrorRatePercent)))), F.Bool3(F.IsBlank(F.Of(this.InformedChangeRequest))), F.Bool3(F.IsBlank(F.Of(this.InformedInvestmentDecision)))))); set { }
        }

        // Formula IsGainOutsideProceduralScope (rulebook: =AND({{EfficiencyGainPercent}} > 0, {{KnowledgeScope}} <> "Procedural"))
        [NotMapped]
        public bool? IsGainOutsideProceduralScope
        {
            get => F.AsBool(F.Memo(this, "IsGainOutsideProceduralScope", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.EfficiencyGainPercent)), ">", F.I(0))), F.Bool3(F.Ne(F.Nullif(F.Of(this.KnowledgeScope)), F.S("Procedural")))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Facility { get; set; }
        public string? ProcedureVersion { get; set; }
        public string? AiInitiative { get; set; }
        public string? InformedChangeRequest { get; set; }
        public string? ComparisonBaseline { get; set; }

        private Facility _facilityRef;

        [ForeignKey("Facility")]
        public virtual Facility FacilityRef
        {
            get
            {
                if (_facilityRef == null && !string.IsNullOrEmpty(Facility))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FacilityRef - no database context is set. Facility: " + Facility + ".");
                        }
                        return null;
                    }
                    _facilityRef = base.SoAContext.Facilities.Find(Facility);
                    if (_facilityRef != null)
                    {
                        base.SoAContext.Attach(_facilityRef);
                    }
                }
                return _facilityRef;
            }
            set
            {
                if (_facilityRef != value)
                {
                    _facilityRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_facilityRef != null)
                    {
                        Facility = _facilityRef.FacilityId;
                    }
                }
            }
        }

        private ProcedureVersion _procedureVersionRef;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersionRef
        {
            get
            {
                if (_procedureVersionRef == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionRef - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersionRef = base.SoAContext.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersionRef != null)
                    {
                        base.SoAContext.Attach(_procedureVersionRef);
                    }
                }
                return _procedureVersionRef;
            }
            set
            {
                if (_procedureVersionRef != value)
                {
                    _procedureVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersionRef != null)
                    {
                        ProcedureVersion = _procedureVersionRef.ProcedureVersionId;
                    }
                }
            }
        }

        private AiAdoptionInitiatif _aiAdoptionInitiatif;

        [ForeignKey("AiInitiative")]
        public virtual AiAdoptionInitiatif AiAdoptionInitiatif
        {
            get
            {
                if (_aiAdoptionInitiatif == null && !string.IsNullOrEmpty(AiInitiative))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiAdoptionInitiatif - no database context is set. AiInitiative: " + AiInitiative + ".");
                        }
                        return null;
                    }
                    _aiAdoptionInitiatif = base.SoAContext.AiAdoptionInitiatives.Find(AiInitiative);
                    if (_aiAdoptionInitiatif != null)
                    {
                        base.SoAContext.Attach(_aiAdoptionInitiatif);
                    }
                }
                return _aiAdoptionInitiatif;
            }
            set
            {
                if (_aiAdoptionInitiatif != value)
                {
                    _aiAdoptionInitiatif = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_aiAdoptionInitiatif != null)
                    {
                        AiInitiative = _aiAdoptionInitiatif.AiAdoptionInitiativeId;
                    }
                }
            }
        }

        private ChangeRequest _changeRequest;

        [ForeignKey("InformedChangeRequest")]
        public virtual ChangeRequest ChangeRequest
        {
            get
            {
                if (_changeRequest == null && !string.IsNullOrEmpty(InformedChangeRequest))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeRequest - no database context is set. InformedChangeRequest: " + InformedChangeRequest + ".");
                        }
                        return null;
                    }
                    _changeRequest = base.SoAContext.ChangeRequests.Find(InformedChangeRequest);
                    if (_changeRequest != null)
                    {
                        base.SoAContext.Attach(_changeRequest);
                    }
                }
                return _changeRequest;
            }
            set
            {
                if (_changeRequest != value)
                {
                    _changeRequest = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_changeRequest != null)
                    {
                        InformedChangeRequest = _changeRequest.ChangeRequestId;
                    }
                }
            }
        }

        private KnowledgeOutcomeMeasurement _knowledgeOutcomeMeasurement;

        [ForeignKey("ComparisonBaseline")]
        public virtual KnowledgeOutcomeMeasurement KnowledgeOutcomeMeasurement
        {
            get
            {
                if (_knowledgeOutcomeMeasurement == null && !string.IsNullOrEmpty(ComparisonBaseline))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeOutcomeMeasurement - no database context is set. ComparisonBaseline: " + ComparisonBaseline + ".");
                        }
                        return null;
                    }
                    _knowledgeOutcomeMeasurement = base.SoAContext.KnowledgeOutcomeMeasurements.Find(ComparisonBaseline);
                    if (_knowledgeOutcomeMeasurement != null)
                    {
                        base.SoAContext.Attach(_knowledgeOutcomeMeasurement);
                    }
                }
                return _knowledgeOutcomeMeasurement;
            }
            set
            {
                if (_knowledgeOutcomeMeasurement != value)
                {
                    _knowledgeOutcomeMeasurement = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeOutcomeMeasurement != null)
                    {
                        ComparisonBaseline = _knowledgeOutcomeMeasurement.KnowledgeOutcomeMeasurementId;
                    }
                }
            }
        }

        private ObservableCollection<KnowledgeOutcomeMeasurement> _knowledgeOutcomeMeasurements;

        [InverseProperty("KnowledgeOutcomeMeasurement")]
        public virtual ObservableCollection<KnowledgeOutcomeMeasurement> KnowledgeOutcomeMeasurements
        {
            get
            {
                if (_knowledgeOutcomeMeasurements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeOutcomeMeasurements - no database context is set. KnowledgeOutcomeMeasurementId: " + this.KnowledgeOutcomeMeasurementId + ".");
                        }
                        _knowledgeOutcomeMeasurements = new ObservableCollection<KnowledgeOutcomeMeasurement>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeOutcomeMeasurements.Where(x => x.ComparisonBaseline == this.KnowledgeOutcomeMeasurementId).ToList<KnowledgeOutcomeMeasurement>();
                        _knowledgeOutcomeMeasurements = new ObservableCollection<KnowledgeOutcomeMeasurement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeOutcomeMeasurements.CollectionChanged += KnowledgeOutcomeMeasurements_CollectionChanged;
                }
                return _knowledgeOutcomeMeasurements;
            }
            private set
            {
                if (_knowledgeOutcomeMeasurements != null)
                {
                    _knowledgeOutcomeMeasurements.CollectionChanged -= KnowledgeOutcomeMeasurements_CollectionChanged;
                }
                _knowledgeOutcomeMeasurements = value;
                if (_knowledgeOutcomeMeasurements != null)
                {
                    _knowledgeOutcomeMeasurements.CollectionChanged += KnowledgeOutcomeMeasurements_CollectionChanged;
                }
            }
        }

        private void KnowledgeOutcomeMeasurements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeOutcomeMeasurement>())
                {
                    item.ComparisonBaseline = this.KnowledgeOutcomeMeasurementId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.FacilityRef;
            _ = this.ProcedureVersionRef;
            _ = this.AiAdoptionInitiatif;
            _ = this.ChangeRequest;
            _ = this.KnowledgeOutcomeMeasurement;
            _ = this.KnowledgeOutcomeMeasurements;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
