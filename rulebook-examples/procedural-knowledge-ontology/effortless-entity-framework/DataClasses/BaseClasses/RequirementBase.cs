
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Requirements")]
    public class RequirementBase : SoAEntityBase
    {
        [Key]
        public string RequirementId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        public string? Name
        {
            get => this.Label; set { }
        }

        public string? Label { get; set; }
        public string? RequirementType { get; set; }
        public string? Statement { get; set; }
        public string? Rationale { get; set; }
        public bool? IsBlocking { get; set; }
        // Formula SatisfactionRecordCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{Requirement}}, {{RequirementId}}))
        public decimal? SatisfactionRecordCount
        {
            get => COUNTIFS(RequirementSatisfactions!this.Requirement, this.RequirementId); set { }
        }

        // Formula StepBindingCount (rulebook: =COUNTIFS(StepRequirements!{{Requirement}}, {{RequirementId}}))
        public decimal? StepBindingCount
        {
            get => COUNTIFS(StepRequirements!this.Requirement, this.RequirementId); set { }
        }

        // Formula IsBoundToAnyStep (rulebook: ={{StepBindingCount}} > 0)
        public bool? IsBoundToAnyStep
        {
            get => this.StepBindingCount > 0; set { }
        }

        // Formula HasEverBeenEvaluated (rulebook: ={{SatisfactionRecordCount}} > 0)
        public bool? HasEverBeenEvaluated
        {
            get => this.SatisfactionRecordCount > 0; set { }
        }

        // Formula NegativeOutcomeCount (rulebook: =COUNTIFS(RequirementSatisfactions!{{NegativeOutcomeRequirementKey}}, {{RequirementId}}))
        public decimal? NegativeOutcomeCount
        {
            get => COUNTIFS(RequirementSatisfactions!this.NegativeOutcomeRequirementKey, this.RequirementId); set { }
        }

        // Formula IsInoperativeControl (rulebook: =AND({{IsBlocking}}, {{IsBoundToAnyStep}}, NOT({{HasEverBeenEvaluated}})))
        public bool? IsInoperativeControl
        {
            get => AND(this.IsBlocking, this.IsBoundToAnyStep, NOT(this.HasEverBeenEvaluated)); set { }
        }

        // Formula IsDecorativeControl (rulebook: =AND({{IsBlocking}}, NOT({{IsBoundToAnyStep}})))
        public bool? IsDecorativeControl
        {
            get => AND(this.IsBlocking, NOT(this.IsBoundToAnyStep)); set { }
        }

        public bool? HasComputedWitness { get; set; }
        public string? WitnessFieldName { get; set; }
        // Formula HasEverProducedNegative (rulebook: ={{NegativeOutcomeCount}} > 0)
        public bool? HasEverProducedNegative
        {
            get => this.NegativeOutcomeCount > 0; set { }
        }

        // Formula IsUnfalsifiedControl (rulebook: =AND({{IsBlocking}}, {{HasEverBeenEvaluated}}, NOT({{HasEverProducedNegative}})))
        public bool? IsUnfalsifiedControl
        {
            get => AND(this.IsBlocking, this.HasEverBeenEvaluated, NOT(this.HasEverProducedNegative)); set { }
        }

        // Formula ClaimsAWitnessField (rulebook: ={{WitnessFieldName}} <> "")
        public bool? ClaimsAWitnessField
        {
            get => this.WitnessFieldName <> ""; set { }
        }

        // Formula NamedWitnessFieldExists (rulebook: =INDEX(RulebookFields!{{IsDerived}}, MATCH({{WitnessFieldName}}, RulebookFields!{{RulebookFieldId}}, 0)))
        public bool? NamedWitnessFieldExists
        {
            get => INDEX(RulebookFields!this.IsDerived, MATCH(this.WitnessFieldName, RulebookFields!this.RulebookFieldId, 0)); set { }
        }

        // Formula DerivedHasComputedWitness (rulebook: =AND({{ClaimsAWitnessField}}, {{NamedWitnessFieldExists}}))
        public bool? DerivedHasComputedWitness
        {
            get => AND(this.ClaimsAWitnessField, this.NamedWitnessFieldExists); set { }
        }

        // Formula WitnessClaimIsUnverified (rulebook: =NOT({{HasComputedWitness}} = {{DerivedHasComputedWitness}}))
        public bool? WitnessClaimIsUnverified
        {
            get => NOT(this.HasComputedWitness = this.DerivedHasComputedWitness); set { }
        }

        // Formula IsUnwitnessedBlockingControl (rulebook: =AND({{IsBlocking}}, NOT({{DerivedHasComputedWitness}})))
        public bool? IsUnwitnessedBlockingControl
        {
            get => AND(this.IsBlocking, NOT(this.DerivedHasComputedWitness)); set { }
        }

        // Formula WitnessFireCount (rulebook: ={{NegativeOutcomeCount}})
        public decimal? WitnessFireCount
        {
            get => this.NegativeOutcomeCount; set { }
        }

        // Formula WitnessHasNeverFired (rulebook: =AND({{HasComputedWitness}}, {{WitnessFireCount}} = 0))
        public bool? WitnessHasNeverFired
        {
            get => AND(this.HasComputedWitness, this.WitnessFireCount = 0); set { }
        }

        // Formula EvaluationSampleSize (rulebook: ={{SatisfactionRecordCount}})
        public decimal? EvaluationSampleSize
        {
            get => this.SatisfactionRecordCount; set { }
        }

        // Formula HasMeaningfulSample (rulebook: ={{EvaluationSampleSize}} >= {{MinimumSampleForAssurance}})
        public bool? HasMeaningfulSample
        {
            get => this.EvaluationSampleSize >= this.MinimumSampleForAssurance; set { }
        }

        public int? MinimumSampleForAssurance { get; set; }
        // Formula IsUntestedWitness (rulebook: =AND({{WitnessHasNeverFired}}, NOT({{HasMeaningfulSample}})))
        public bool? IsUntestedWitness
        {
            get => AND(this.WitnessHasNeverFired, NOT(this.HasMeaningfulSample)); set { }
        }

        // Formula IsEvidencedHoldingControl (rulebook: =AND({{WitnessHasNeverFired}}, {{HasMeaningfulSample}}))
        public bool? IsEvidencedHoldingControl
        {
            get => AND(this.WitnessHasNeverFired, this.HasMeaningfulSample); set { }
        }

        // Formula ControlAssuranceState (rulebook: =IF(NOT({{IsBoundToAnyStep}}), "Decorative", IF(NOT({{HasEverBeenEvaluated}}), "Inoperative", IF(NOT({{HasComputedWitness}}), "Asserted", IF({{WitnessFireCount}} > 0, "Demonstrated", IF({{HasMeaningfulSample}}, "Holding", "Untested"))))))
        public string? ControlAssuranceState
        {
            get => IF(NOT(this.IsBoundToAnyStep), "Decorative", IF(NOT(this.HasEverBeenEvaluated), "Inoperative", IF(NOT(this.HasComputedWitness), "Asserted", IF(this.WitnessFireCount > 0, "Demonstrated", IF(this.HasMeaningfulSample, "Holding", "Untested"))))); set { }
        }

        // Formula UnexercisedBindingCount (rulebook: =COUNTIFS(StepRequirements!{{UnexercisedBindingRequirementKey}}, {{RequirementId}}))
        public decimal? UnexercisedBindingCount
        {
            get => COUNTIFS(StepRequirements!this.UnexercisedBindingRequirementKey, this.RequirementId); set { }
        }

        // Formula WitnessIsPartiallyScoped (rulebook: =AND({{HasComputedWitness}}, {{UnexercisedBindingCount}} > 0))
        public bool? WitnessIsPartiallyScoped
        {
            get => AND(this.HasComputedWitness, this.UnexercisedBindingCount > 0); set { }
        }

        // Formula AccountableAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{AccountableRole}}, Roles!{{RoleId}}, 0)))
        public string? AccountableAgent
        {
            get => INDEX(Roles!this.CurrentAgent, MATCH(this.AccountableRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula HasNamedOwner (rulebook: ={{AccountableRole}} <> "")
        public bool? HasNamedOwner
        {
            get => this.AccountableRole <> ""; set { }
        }

        // Formula IsOrphanedBlockingControl (rulebook: =AND({{IsBlocking}}, NOT({{HasNamedOwner}})))
        public bool? IsOrphanedBlockingControl
        {
            get => AND(this.IsBlocking, NOT(this.HasNamedOwner)); set { }
        }

        // Formula IsUnwatchedAndUnowned (rulebook: =AND({{IsBlocking}}, NOT({{HasComputedWitness}}), NOT({{HasNamedOwner}})))
        public bool? IsUnwatchedAndUnowned
        {
            get => AND(this.IsBlocking, NOT(this.HasComputedWitness), NOT(this.HasNamedOwner)); set { }
        }

        // Formula AttestationExposureNote (rulebook: =IF(NOT({{IsBlocking}}), "", IF({{IsUnwatchedAndUnowned}}, "Unwatched and unowned: exposure defaults to the signatory.", IF({{IsOrphanedBlockingControl}}, "Witnessed but unowned: no named accountability.", IF(NOT({{HasComputedWitness}}), "Owned but unwitnessed: rests on human judgement.", "")))))
        public string? AttestationExposureNote
        {
            get => IF(NOT(this.IsBlocking), "", IF(this.IsUnwatchedAndUnowned, "Unwatched and unowned: exposure defaults to the signatory.", IF(this.IsOrphanedBlockingControl, "Witnessed but unowned: no named accountability.", IF(NOT(this.HasComputedWitness), "Owned but unwitnessed: rests on human judgement.", "")))); set { }
        }

        // Formula UnwatchedUnownedFlag (rulebook: =IF({{IsUnwatchedAndUnowned}}, "unwatched-unowned", ""))
        public string? UnwatchedUnownedFlag
        {
            get => IF(this.IsUnwatchedAndUnowned, "unwatched-unowned", ""); set { }
        }

        // Formula UsesControlledVocabulary (rulebook: ={{ControlledTerm}} <> "")
        public bool? UsesControlledVocabulary
        {
            get => this.ControlledTerm <> ""; set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? AccountableRole { get; set; }
        public string? ControlledTerm { get; set; }

        private Role _role;

        [ForeignKey("AccountableRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(AccountableRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AccountableRole: " + AccountableRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(AccountableRole);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    AccountableRole = _role == null ? default : _role.RoleId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerm - no database context is set. ControlledTerm: " + ControlledTerm + ".");
                        }
                        return null;
                    }
                    _vocabularyTerm = Context.VocabularyTerms.Find(ControlledTerm);
                    if (_vocabularyTerm != null)
                    {
                        Context.Attach(_vocabularyTerm);
                    }
                }
                return _vocabularyTerm;
            }
            set
            {
                if (_vocabularyTerm != value)
                {
                    _vocabularyTerm = value;
                    ControlledTerm = _vocabularyTerm == null ? default : _vocabularyTerm.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<StepRequirement> _stepRequirements;

        [InverseProperty("Requirement")]
        public virtual ObservableCollection<StepRequirement> StepRequirements
        {
            get
            {
                if (_stepRequirements == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRequirements - no database context is set. RequirementId: " + this.RequirementId + ".");
                        }
                        _stepRequirements = new ObservableCollection<StepRequirement>();
                    }
                    else
                    {
                        var items = Context.StepRequirements.Where(x => x.Requirement == this.RequirementId).ToList<StepRequirement>();
                        _stepRequirements = new ObservableCollection<StepRequirement>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("Requirement")]
        public virtual ObservableCollection<RequirementSatisfaction> RequirementSatisfactions
        {
            get
            {
                if (_requirementSatisfactions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequirementSatisfactions - no database context is set. RequirementId: " + this.RequirementId + ".");
                        }
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>();
                    }
                    else
                    {
                        var items = Context.RequirementSatisfactions.Where(x => x.Requirement == this.RequirementId).ToList<RequirementSatisfaction>();
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorityBoundaries - no database context is set. RequirementId: " + this.RequirementId + ".");
                        }
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>();
                    }
                    else
                    {
                        var items = Context.AuthorityBoundaries.Where(x => x.EnforcingRequirement == this.RequirementId).ToList<AuthorityBoundary>();
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.VocabularyTerm;
            _ = this.StepRequirements;
            _ = this.RequirementSatisfactions;
            _ = this.AuthorityBoundaries;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
