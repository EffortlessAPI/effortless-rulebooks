
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
    [Table("OntologySupportProgrammes")]
    public class OntologySupportProgrammeBase : SoAEntityBase
    {
        [Key]
        public string OntologySupportProgrammeId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Acronym { get; set; }
        public string? Funder { get; set; }
        public string? GrantReference { get; set; }
        public string? ProgrammeIri { get; set; }
        public string? Coordinator { get; set; }
        public DateTimeOffset? StartedOn { get; set; }
        public DateTimeOffset? EndsOn { get; set; }
        public bool? IsIndustryFocused { get; set; }
        public string? OurSuccessorSteward { get; set; }
        public string? WhyRecorded { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula SupportedProfileCount (rulebook: =COUNTIFS(OntologyProfiles!{{SupportingProgramme}}, {{OntologySupportProgrammeId}}))
        [NotMapped]
        public int? SupportedProfileCount
        {
            get => F.AsInt(F.Memo(this, "SupportedProfileCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<OntologyProfile>(base.SoAContext, "OntologyProfiles", __c => __c.OntologyProfiles), __r => F.CritField(F.Of(__r.SupportingProgramme), F.Of(this.OntologySupportProgrammeId))))))); set { }
        }

        // Formula HasEnded (rulebook: =AND({{EndsOn}} <> "", {{EndsOn}} < {{AsOfInstant}}))
        [NotMapped]
        public bool? HasEnded
        {
            get => F.AsBool(F.Memo(this, "HasEnded", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.EndsOn))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.EndsOn)), "<", F.Of(this.AsOfInstant)))))); set { }
        }

        // Formula DaysUntilProgrammeEnds (rulebook: =IF({{EndsOn}} = "", 0, DATETIME_DIFF({{EndsOn}}, {{AsOfInstant}}, "days")))
        [NotMapped]
        public int? DaysUntilProgrammeEnds
        {
            get => F.AsInt(F.Memo(this, "DaysUntilProgrammeEnds", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.EndsOn)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.EndsOn), F.Of(this.AsOfInstant), F.S("days")))))); set { }
        }

        // Formula IsEndedWithNoStewardNamed (rulebook: =AND({{HasEnded}}, {{OurSuccessorSteward}} = ""))
        [NotMapped]
        public bool? IsEndedWithNoStewardNamed
        {
            get => F.AsBool(F.Memo(this, "IsEndedWithNoStewardNamed", () => F.And(F.Bool3(F.Of(this.HasEnded)), F.Bool3(F.IsBlank(F.Of(this.OurSuccessorSteward)))))); set { }
        }

        // Formula IsEndingSoonWithNoStewardNamed (rulebook: =AND({{HasEnded}} = FALSE, {{EndsOn}} <> "", {{DaysUntilProgrammeEnds}} <= 180, {{OurSuccessorSteward}} = ""))
        [NotMapped]
        public bool? IsEndingSoonWithNoStewardNamed
        {
            get => F.AsBool(F.Memo(this, "IsEndingSoonWithNoStewardNamed", () => F.And(F.Bool3(F.Eq(F.Of(this.HasEnded), F.B(false))), F.Bool3(F.IsNotBlank(F.Of(this.EndsOn))), F.Bool3(F.Cmp(F.Of(this.DaysUntilProgrammeEnds), "<=", F.I(180))), F.Bool3(F.IsBlank(F.Of(this.OurSuccessorSteward)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? EvaluationContext { get; set; }

        private EvaluationContext _evaluationContextRef;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContextRef
        {
            get
            {
                if (_evaluationContextRef == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContextRef - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContextRef = base.SoAContext.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContextRef != null)
                    {
                        base.SoAContext.Attach(_evaluationContextRef);
                    }
                }
                return _evaluationContextRef;
            }
            set
            {
                if (_evaluationContextRef != value)
                {
                    _evaluationContextRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_evaluationContextRef != null)
                    {
                        EvaluationContext = _evaluationContextRef.EvaluationContextId;
                    }
                }
            }
        }

        private ObservableCollection<OntologyProfile> _ontologyProfiles;

        [InverseProperty("OntologySupportProgramme")]
        public virtual ObservableCollection<OntologyProfile> OntologyProfiles
        {
            get
            {
                if (_ontologyProfiles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OntologyProfiles - no database context is set. OntologySupportProgrammeId: " + this.OntologySupportProgrammeId + ".");
                        }
                        _ontologyProfiles = new ObservableCollection<OntologyProfile>();
                    }
                    else
                    {
                        var items = base.SoAContext.OntologyProfiles.Where(x => x.SupportingProgramme == this.OntologySupportProgrammeId).ToList<OntologyProfile>();
                        _ontologyProfiles = new ObservableCollection<OntologyProfile>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _ontologyProfiles.CollectionChanged += OntologyProfiles_CollectionChanged;
                }
                return _ontologyProfiles;
            }
            private set
            {
                if (_ontologyProfiles != null)
                {
                    _ontologyProfiles.CollectionChanged -= OntologyProfiles_CollectionChanged;
                }
                _ontologyProfiles = value;
                if (_ontologyProfiles != null)
                {
                    _ontologyProfiles.CollectionChanged += OntologyProfiles_CollectionChanged;
                }
            }
        }

        private void OntologyProfiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<OntologyProfile>())
                {
                    item.SupportingProgramme = this.OntologySupportProgrammeId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.EvaluationContextRef;
            _ = this.OntologyProfiles;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
