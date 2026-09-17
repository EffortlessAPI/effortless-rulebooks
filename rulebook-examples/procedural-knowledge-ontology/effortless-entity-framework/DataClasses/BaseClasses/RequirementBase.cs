
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
    [Table("Requirements")]
    public class RequirementBase : SoAEntityBase
    {
        [Key]
        public string RequirementId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? RequirementType { get; set; }
        public string? Statement { get; set; }
        public string? Rationale { get; set; }
        public bool? IsBlocking { get; set; }
        // Formula SatisfactionRecordCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{Requirement}}, {{RequirementId}}))
        [NotMapped]
        public decimal? SatisfactionRecordCount
        {
            get => F.AsDecimal(F.Memo(this, "SatisfactionRecordCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.Requirement), F.Of(this.RequirementId)))))); set { }
        }

        // Formula StepBindingCount (rulebook: =COUNTIFS(StepRequirements!{{Requirement}}, {{RequirementId}}))
        [NotMapped]
        public decimal? StepBindingCount
        {
            get => F.AsDecimal(F.Memo(this, "StepBindingCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepRequirement>(base.SoAContext, "StepRequirements", __c => __c.StepRequirements), __r => F.CritField(F.Of(__r.Requirement), F.Of(this.RequirementId)))))); set { }
        }

        // Formula IsBoundToAnyStep (rulebook: ={{StepBindingCount}} > 0)
        [NotMapped]
        public bool? IsBoundToAnyStep
        {
            get => F.AsBool(F.Memo(this, "IsBoundToAnyStep", () => F.Cmp(F.Of(this.StepBindingCount), ">", F.I(0)))); set { }
        }

        // Formula HasEverBeenEvaluated (rulebook: ={{SatisfactionRecordCount}} > 0)
        [NotMapped]
        public bool? HasEverBeenEvaluated
        {
            get => F.AsBool(F.Memo(this, "HasEverBeenEvaluated", () => F.Cmp(F.Of(this.SatisfactionRecordCount), ">", F.I(0)))); set { }
        }

        // Formula NegativeOutcomeCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{NegativeOutcomeRequirementKey}}, {{RequirementId}}))
        [NotMapped]
        public decimal? NegativeOutcomeCount
        {
            get => F.AsDecimal(F.Memo(this, "NegativeOutcomeCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RequirementSatisfaction>(base.SoAContext, "RequirementSatisfactions", __c => __c.RequirementSatisfactions), __r => F.CritField(F.Of(__r.NegativeOutcomeRequirementKey), F.Of(this.RequirementId)))))); set { }
        }

        // Formula IsInoperativeControl (rulebook: =AND({{IsBlocking}}, {{IsBoundToAnyStep}}, NOT({{HasEverBeenEvaluated}})))
        [NotMapped]
        public bool? IsInoperativeControl
        {
            get => F.AsBool(F.Memo(this, "IsInoperativeControl", () => F.And(F.IsTrueV(F.Of(this.IsBlocking)), F.Bool3(F.Of(this.IsBoundToAnyStep)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasEverBeenEvaluated))))))); set { }
        }

        // Formula IsDecorativeControl (rulebook: =AND({{IsBlocking}}, NOT({{IsBoundToAnyStep}})))
        [NotMapped]
        public bool? IsDecorativeControl
        {
            get => F.AsBool(F.Memo(this, "IsDecorativeControl", () => F.And(F.IsTrueV(F.Of(this.IsBlocking)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsBoundToAnyStep))))))); set { }
        }

        public bool? HasComputedWitness { get; set; }
        public string? WitnessFieldName { get; set; }
        // Formula HasEverProducedNegative (rulebook: ={{NegativeOutcomeCount}} > 0)
        [NotMapped]
        public bool? HasEverProducedNegative
        {
            get => F.AsBool(F.Memo(this, "HasEverProducedNegative", () => F.Cmp(F.Of(this.NegativeOutcomeCount), ">", F.I(0)))); set { }
        }

        // Formula IsUnfalsifiedControl (rulebook: =AND({{IsBlocking}}, {{HasEverBeenEvaluated}}, NOT({{HasEverProducedNegative}})))
        [NotMapped]
        public bool? IsUnfalsifiedControl
        {
            get => F.AsBool(F.Memo(this, "IsUnfalsifiedControl", () => F.And(F.IsTrueV(F.Of(this.IsBlocking)), F.Bool3(F.Of(this.HasEverBeenEvaluated)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasEverProducedNegative))))))); set { }
        }

        // Formula ClaimsAWitnessField (rulebook: ={{WitnessFieldName}} <> "")
        [NotMapped]
        public bool? ClaimsAWitnessField
        {
            get => F.AsBool(F.Memo(this, "ClaimsAWitnessField", () => F.IsNotBlank(F.Of(this.WitnessFieldName)))); set { }
        }

        // Formula NamedWitnessFieldExists (rulebook: =INDEX(RulebookFields!{{IsDerived}}, MATCH({{WitnessFieldName}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public bool? NamedWitnessFieldExists
        {
            get => F.AsBool(F.Memo(this, "NamedWitnessFieldExists", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.WitnessFieldName), __r => F.Of(__r.IsDerived), () => F.Of(new RulebookField().IsDerived)))); set { }
        }

        // Formula DerivedHasComputedWitness (rulebook: =AND({{ClaimsAWitnessField}}, {{NamedWitnessFieldExists}}))
        [NotMapped]
        public bool? DerivedHasComputedWitness
        {
            get => F.AsBool(F.Memo(this, "DerivedHasComputedWitness", () => F.And(F.Bool3(F.Of(this.ClaimsAWitnessField)), F.Bool3(F.Of(this.NamedWitnessFieldExists))))); set { }
        }

        // Formula WitnessClaimIsUnverified (rulebook: =NOT({{HasComputedWitness}} = {{DerivedHasComputedWitness}}))
        [NotMapped]
        public bool? WitnessClaimIsUnverified
        {
            get => F.AsBool(F.Memo(this, "WitnessClaimIsUnverified", () => F.Not(F.Bool3(F.Eq(F.Nullif(F.Of(this.HasComputedWitness)), F.Of(this.DerivedHasComputedWitness)))))); set { }
        }

        // Formula IsUnwitnessedBlockingControl (rulebook: =AND({{IsBlocking}}, NOT({{DerivedHasComputedWitness}})))
        [NotMapped]
        public bool? IsUnwitnessedBlockingControl
        {
            get => F.AsBool(F.Memo(this, "IsUnwitnessedBlockingControl", () => F.And(F.IsTrueV(F.Of(this.IsBlocking)), F.Bool3(F.Not(F.Bool3(F.Of(this.DerivedHasComputedWitness))))))); set { }
        }

        // Formula WitnessFireCount (rulebook: ={{NegativeOutcomeCount}})
        [NotMapped]
        public decimal? WitnessFireCount
        {
            get => F.AsDecimal(F.Memo(this, "WitnessFireCount", () => F.Of(this.NegativeOutcomeCount))); set { }
        }

        // Formula WitnessHasNeverFired (rulebook: =AND({{HasComputedWitness}}, {{WitnessFireCount}} = 0))
        [NotMapped]
        public bool? WitnessHasNeverFired
        {
            get => F.AsBool(F.Memo(this, "WitnessHasNeverFired", () => F.And(F.IsTrueV(F.Of(this.HasComputedWitness)), F.Bool3(F.Eq(F.Of(this.WitnessFireCount), F.I(0)))))); set { }
        }

        // Formula EvaluationSampleSize (rulebook: ={{SatisfactionRecordCount}})
        [NotMapped]
        public decimal? EvaluationSampleSize
        {
            get => F.AsDecimal(F.Memo(this, "EvaluationSampleSize", () => F.Of(this.SatisfactionRecordCount))); set { }
        }

        // Formula HasMeaningfulSample (rulebook: ={{EvaluationSampleSize}} >= {{MinimumSampleForAssurance}})
        [NotMapped]
        public bool? HasMeaningfulSample
        {
            get => F.AsBool(F.Memo(this, "HasMeaningfulSample", () => F.Cmp(F.Of(this.EvaluationSampleSize), ">=", F.Nullif(F.Of(this.MinimumSampleForAssurance))))); set { }
        }

        public int? MinimumSampleForAssurance { get; set; }
        // Formula IsUntestedWitness (rulebook: =AND({{WitnessHasNeverFired}}, NOT({{HasMeaningfulSample}})))
        [NotMapped]
        public bool? IsUntestedWitness
        {
            get => F.AsBool(F.Memo(this, "IsUntestedWitness", () => F.And(F.Bool3(F.Of(this.WitnessHasNeverFired)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasMeaningfulSample))))))); set { }
        }

        // Formula IsEvidencedHoldingControl (rulebook: =AND({{WitnessHasNeverFired}}, {{HasMeaningfulSample}}))
        [NotMapped]
        public bool? IsEvidencedHoldingControl
        {
            get => F.AsBool(F.Memo(this, "IsEvidencedHoldingControl", () => F.And(F.Bool3(F.Of(this.WitnessHasNeverFired)), F.Bool3(F.Of(this.HasMeaningfulSample))))); set { }
        }

        // Formula ControlAssuranceState (rulebook: =IF(NOT({{IsBoundToAnyStep}}), "Decorative", IF(NOT({{HasEverBeenEvaluated}}), "Inoperative", IF(NOT({{HasComputedWitness}}), "Asserted", IF({{WitnessFireCount}} > 0, "Demonstrated", IF({{HasMeaningfulSample}}, "Holding", "Untested"))))))
        [NotMapped]
        public string? ControlAssuranceState
        {
            get => F.AsString(F.Memo(this, "ControlAssuranceState", () => (F.Truthy(F.Bool3(F.Not(F.Bool3(F.Of(this.IsBoundToAnyStep))))) ? F.S("Decorative") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.Of(this.HasEverBeenEvaluated))))) ? F.S("Inoperative") : (F.Truthy(F.Bool3(F.Not(F.IsTrueV(F.Of(this.HasComputedWitness))))) ? F.S("Asserted") : (F.Truthy(F.Bool3(F.Cmp(F.Of(this.WitnessFireCount), ">", F.I(0)))) ? F.S("Demonstrated") : (F.Truthy(F.Bool3(F.Of(this.HasMeaningfulSample))) ? F.S("Holding") : F.S("Untested")))))))); set { }
        }

        // Formula UnexercisedBindingCount (rulebook: =COUNTIFS(StepRequirements!{{UnexercisedBindingRequirementKey}}, {{RequirementId}}))
        [NotMapped]
        public decimal? UnexercisedBindingCount
        {
            get => F.AsDecimal(F.Memo(this, "UnexercisedBindingCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepRequirement>(base.SoAContext, "StepRequirements", __c => __c.StepRequirements), __r => F.CritField(F.Of(__r.UnexercisedBindingRequirementKey), F.Of(this.RequirementId)))))); set { }
        }

        // Formula WitnessIsPartiallyScoped (rulebook: =AND({{HasComputedWitness}}, {{UnexercisedBindingCount}} > 0))
        [NotMapped]
        public bool? WitnessIsPartiallyScoped
        {
            get => F.AsBool(F.Memo(this, "WitnessIsPartiallyScoped", () => F.And(F.IsTrueV(F.Of(this.HasComputedWitness)), F.Bool3(F.Cmp(F.Of(this.UnexercisedBindingCount), ">", F.I(0)))))); set { }
        }

        // Formula AccountableAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{AccountableRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? AccountableAgent
        {
            get => F.AsString(F.Memo(this, "AccountableAgent", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AccountableRole), __r => F.Of(__r.CurrentAgent), () => F.Of(new Role().CurrentAgent)))); set { }
        }

        // Formula HasNamedOwner (rulebook: ={{AccountableRole}} <> "")
        [NotMapped]
        public bool? HasNamedOwner
        {
            get => F.AsBool(F.Memo(this, "HasNamedOwner", () => F.IsNotBlank(F.Of(this.AccountableRole)))); set { }
        }

        // Formula IsOrphanedBlockingControl (rulebook: =AND({{IsBlocking}}, NOT({{HasNamedOwner}})))
        [NotMapped]
        public bool? IsOrphanedBlockingControl
        {
            get => F.AsBool(F.Memo(this, "IsOrphanedBlockingControl", () => F.And(F.IsTrueV(F.Of(this.IsBlocking)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasNamedOwner))))))); set { }
        }

        // Formula IsUnwatchedAndUnowned (rulebook: =AND({{IsBlocking}}, NOT({{HasComputedWitness}}), NOT({{HasNamedOwner}})))
        [NotMapped]
        public bool? IsUnwatchedAndUnowned
        {
            get => F.AsBool(F.Memo(this, "IsUnwatchedAndUnowned", () => F.And(F.IsTrueV(F.Of(this.IsBlocking)), F.Bool3(F.Not(F.IsTrueV(F.Of(this.HasComputedWitness)))), F.Bool3(F.Not(F.Bool3(F.Of(this.HasNamedOwner))))))); set { }
        }

        // Formula AttestationExposureNote (rulebook: =IF(NOT({{IsBlocking}}), "", IF({{IsUnwatchedAndUnowned}}, "Unwatched and unowned: exposure defaults to the signatory.", IF({{IsOrphanedBlockingControl}}, "Witnessed but unowned: no named accountability.", IF(NOT({{HasComputedWitness}}), "Owned but unwitnessed: rests on human judgement.", "")))))
        [NotMapped]
        public string? AttestationExposureNote
        {
            get => F.AsString(F.Memo(this, "AttestationExposureNote", () => (F.Truthy(F.Bool3(F.Not(F.IsTrueV(F.Of(this.IsBlocking))))) ? F.S("") : (F.Truthy(F.Bool3(F.Of(this.IsUnwatchedAndUnowned))) ? F.S("Unwatched and unowned: exposure defaults to the signatory.") : (F.Truthy(F.Bool3(F.Of(this.IsOrphanedBlockingControl))) ? F.S("Witnessed but unowned: no named accountability.") : (F.Truthy(F.Bool3(F.Not(F.IsTrueV(F.Of(this.HasComputedWitness))))) ? F.S("Owned but unwitnessed: rests on human judgement.") : F.S(""))))))); set { }
        }

        // Formula UnwatchedUnownedFlag (rulebook: =IF({{IsUnwatchedAndUnowned}}, "unwatched-unowned", ""))
        [NotMapped]
        public string? UnwatchedUnownedFlag
        {
            get => F.AsString(F.Memo(this, "UnwatchedUnownedFlag", () => (F.Truthy(F.Bool3(F.Of(this.IsUnwatchedAndUnowned))) ? F.S("unwatched-unowned") : F.S("")))); set { }
        }

        // Formula UsesControlledVocabulary (rulebook: ={{ControlledTerm}} <> "")
        [NotMapped]
        public bool? UsesControlledVocabulary
        {
            get => F.AsBool(F.Memo(this, "UsesControlledVocabulary", () => F.IsNotBlank(F.Of(this.ControlledTerm)))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        // Formula IsRegulatoryRequirement (rulebook: ={{RegulatoryFramework}} <> "")
        [NotMapped]
        public bool? IsRegulatoryRequirement
        {
            get => F.AsBool(F.Memo(this, "IsRegulatoryRequirement", () => F.IsNotBlank(F.Of(this.RegulatoryFramework)))); set { }
        }

        // Formula ConstraintTraceCount (rulebook: =COUNTIFS(KnowledgeTraces!{{Requirement}}, {{RequirementId}}, KnowledgeTraces!{{TargetKind}}, "Constraint"))
        [NotMapped]
        public int? ConstraintTraceCount
        {
            get => F.AsInt(F.Memo(this, "ConstraintTraceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTrace>(base.SoAContext, "KnowledgeTraces", __c => __c.KnowledgeTraces), __r => F.CritField(F.Of(__r.Requirement), F.Of(this.RequirementId)) && F.CritLiteral(F.Of(__r.TargetKind), F.S("Constraint"))))))); set { }
        }

        // Formula IsUntracedBoundConstraint (rulebook: =AND({{IsBoundToAnyStep}}, {{ConstraintTraceCount}} = 0))
        [NotMapped]
        public bool? IsUntracedBoundConstraint
        {
            get => F.AsBool(F.Memo(this, "IsUntracedBoundConstraint", () => F.And(F.Bool3(F.Of(this.IsBoundToAnyStep)), F.Bool3(F.Eq(F.Of(this.ConstraintTraceCount), F.I(0)))))); set { }
        }


        public string? AccountableRole { get; set; }
        public string? ControlledTerm { get; set; }
        public string? RegulatoryFramework { get; set; }

        private Role _role;

        [ForeignKey("AccountableRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(AccountableRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AccountableRole: " + AccountableRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(AccountableRole);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        AccountableRole = _role.RoleId;
                    }
                }
            }
        }

        private VocabularyTerm _vocabularyTerm;

        [ForeignKey("ControlledTerm")]
        public virtual VocabularyTerm VocabularyTerm
        {
            get
            {
                if (_vocabularyTerm == null && !string.IsNullOrEmpty(ControlledTerm))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerm - no database context is set. ControlledTerm: " + ControlledTerm + ".");
                        }
                        return null;
                    }
                    _vocabularyTerm = base.SoAContext.VocabularyTerms.Find(ControlledTerm);
                    if (_vocabularyTerm != null)
                    {
                        base.SoAContext.Attach(_vocabularyTerm);
                    }
                }
                return _vocabularyTerm;
            }
            set
            {
                if (_vocabularyTerm != value)
                {
                    _vocabularyTerm = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabularyTerm != null)
                    {
                        ControlledTerm = _vocabularyTerm.VocabularyTermId;
                    }
                }
            }
        }

        private RegulatoryFramework _regulatoryFrameworkRef;

        [ForeignKey("RegulatoryFramework")]
        public virtual RegulatoryFramework RegulatoryFrameworkRef
        {
            get
            {
                if (_regulatoryFrameworkRef == null && !string.IsNullOrEmpty(RegulatoryFramework))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RegulatoryFrameworkRef - no database context is set. RegulatoryFramework: " + RegulatoryFramework + ".");
                        }
                        return null;
                    }
                    _regulatoryFrameworkRef = base.SoAContext.RegulatoryFrameworks.Find(RegulatoryFramework);
                    if (_regulatoryFrameworkRef != null)
                    {
                        base.SoAContext.Attach(_regulatoryFrameworkRef);
                    }
                }
                return _regulatoryFrameworkRef;
            }
            set
            {
                if (_regulatoryFrameworkRef != value)
                {
                    _regulatoryFrameworkRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_regulatoryFrameworkRef != null)
                    {
                        RegulatoryFramework = _regulatoryFrameworkRef.RegulatoryFrameworkId;
                    }
                }
            }
        }

        private ObservableCollection<StepRequirement> _stepRequirements;

        [InverseProperty("RequirementRef")]
        public virtual ObservableCollection<StepRequirement> StepRequirements
        {
            get
            {
                if (_stepRequirements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRequirements - no database context is set. RequirementId: " + this.RequirementId + ".");
                        }
                        _stepRequirements = new ObservableCollection<StepRequirement>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepRequirements.Where(x => x.Requirement == this.RequirementId).ToList<StepRequirement>();
                        _stepRequirements = new ObservableCollection<StepRequirement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepRequirements.CollectionChanged += StepRequirements_CollectionChanged;
                }
                return _stepRequirements;
            }
            private set
            {
                if (_stepRequirements != null)
                {
                    _stepRequirements.CollectionChanged -= StepRequirements_CollectionChanged;
                }
                _stepRequirements = value;
                if (_stepRequirements != null)
                {
                    _stepRequirements.CollectionChanged += StepRequirements_CollectionChanged;
                }
            }
        }

        private void StepRequirements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepRequirement>())
                {
                    item.Requirement = this.RequirementId;
                }
            }
        }

        private ObservableCollection<RequirementSatisfaction> _requirementSatisfactions;

        [InverseProperty("RequirementRef")]
        public virtual ObservableCollection<RequirementSatisfaction> RequirementSatisfactions
        {
            get
            {
                if (_requirementSatisfactions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequirementSatisfactions - no database context is set. RequirementId: " + this.RequirementId + ".");
                        }
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>();
                    }
                    else
                    {
                        var items = base.SoAContext.RequirementSatisfactions.Where(x => x.Requirement == this.RequirementId).ToList<RequirementSatisfaction>();
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _requirementSatisfactions.CollectionChanged += RequirementSatisfactions_CollectionChanged;
                }
                return _requirementSatisfactions;
            }
            private set
            {
                if (_requirementSatisfactions != null)
                {
                    _requirementSatisfactions.CollectionChanged -= RequirementSatisfactions_CollectionChanged;
                }
                _requirementSatisfactions = value;
                if (_requirementSatisfactions != null)
                {
                    _requirementSatisfactions.CollectionChanged += RequirementSatisfactions_CollectionChanged;
                }
            }
        }

        private void RequirementSatisfactions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RequirementSatisfaction>())
                {
                    item.Requirement = this.RequirementId;
                }
            }
        }

        private ObservableCollection<AuthorityBoundary> _authorityBoundaries;

        [InverseProperty("Requirement")]
        public virtual ObservableCollection<AuthorityBoundary> AuthorityBoundaries
        {
            get
            {
                if (_authorityBoundaries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorityBoundaries - no database context is set. RequirementId: " + this.RequirementId + ".");
                        }
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>();
                    }
                    else
                    {
                        var items = base.SoAContext.AuthorityBoundaries.Where(x => x.EnforcingRequirement == this.RequirementId).ToList<AuthorityBoundary>();
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _authorityBoundaries.CollectionChanged += AuthorityBoundaries_CollectionChanged;
                }
                return _authorityBoundaries;
            }
            private set
            {
                if (_authorityBoundaries != null)
                {
                    _authorityBoundaries.CollectionChanged -= AuthorityBoundaries_CollectionChanged;
                }
                _authorityBoundaries = value;
                if (_authorityBoundaries != null)
                {
                    _authorityBoundaries.CollectionChanged += AuthorityBoundaries_CollectionChanged;
                }
            }
        }

        private void AuthorityBoundaries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AuthorityBoundary>())
                {
                    item.EnforcingRequirement = this.RequirementId;
                }
            }
        }

        private ObservableCollection<AnswerRequirementCheck> _answerRequirementChecks;

        [InverseProperty("RequirementRef")]
        public virtual ObservableCollection<AnswerRequirementCheck> AnswerRequirementChecks
        {
            get
            {
                if (_answerRequirementChecks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AnswerRequirementChecks - no database context is set. RequirementId: " + this.RequirementId + ".");
                        }
                        _answerRequirementChecks = new ObservableCollection<AnswerRequirementCheck>();
                    }
                    else
                    {
                        var items = base.SoAContext.AnswerRequirementChecks.Where(x => x.Requirement == this.RequirementId).ToList<AnswerRequirementCheck>();
                        _answerRequirementChecks = new ObservableCollection<AnswerRequirementCheck>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _answerRequirementChecks.CollectionChanged += AnswerRequirementChecks_CollectionChanged;
                }
                return _answerRequirementChecks;
            }
            private set
            {
                if (_answerRequirementChecks != null)
                {
                    _answerRequirementChecks.CollectionChanged -= AnswerRequirementChecks_CollectionChanged;
                }
                _answerRequirementChecks = value;
                if (_answerRequirementChecks != null)
                {
                    _answerRequirementChecks.CollectionChanged += AnswerRequirementChecks_CollectionChanged;
                }
            }
        }

        private void AnswerRequirementChecks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AnswerRequirementCheck>())
                {
                    item.Requirement = this.RequirementId;
                }
            }
        }

        private ObservableCollection<KnowledgeTrace> _knowledgeTraces;

        [InverseProperty("RequirementRef")]
        public virtual ObservableCollection<KnowledgeTrace> KnowledgeTraces
        {
            get
            {
                if (_knowledgeTraces == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeTraces - no database context is set. RequirementId: " + this.RequirementId + ".");
                        }
                        _knowledgeTraces = new ObservableCollection<KnowledgeTrace>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTraces.Where(x => x.Requirement == this.RequirementId).ToList<KnowledgeTrace>();
                        _knowledgeTraces = new ObservableCollection<KnowledgeTrace>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeTraces.CollectionChanged += KnowledgeTraces_CollectionChanged;
                }
                return _knowledgeTraces;
            }
            private set
            {
                if (_knowledgeTraces != null)
                {
                    _knowledgeTraces.CollectionChanged -= KnowledgeTraces_CollectionChanged;
                }
                _knowledgeTraces = value;
                if (_knowledgeTraces != null)
                {
                    _knowledgeTraces.CollectionChanged += KnowledgeTraces_CollectionChanged;
                }
            }
        }

        private void KnowledgeTraces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeTrace>())
                {
                    item.Requirement = this.RequirementId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.VocabularyTerm;
            _ = this.RegulatoryFrameworkRef;
            _ = this.StepRequirements;
            _ = this.RequirementSatisfactions;
            _ = this.AuthorityBoundaries;
            _ = this.AnswerRequirementChecks;
            _ = this.KnowledgeTraces;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
