
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
    [Table("CompetencyQuestions")]
    public class CompetencyQuestionBase : SoAEntityBase
    {
        [Key]
        public string CompetencyQuestionId { get; set; }

        // Formula RelativePath (rulebook: ="competency-questions/" & {{CompetencyQuestionId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("competency-questions/"), F.TextOr(F.Of(this.CompetencyQuestionId))))); set { }
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

        public decimal Number { get; set; }
        public string DisplayName { get; set; }
        public string QuestionText { get; set; }
        public string TargetTable { get; set; }
        public string TargetField { get; set; }
        public string AnswerKind { get; set; }
        public string ExpectedAnswer { get; set; }
        public string? SatisfiedField { get; set; }
        public string? Explanation { get; set; }
        public decimal? SortOrder { get; set; }
        public bool? IsActive { get; set; }

        public string? SimulateScenario { get; set; }

        private Scenario _scenario;

        [ForeignKey("SimulateScenario")]
        public virtual Scenario Scenario
        {
            get
            {
                if (_scenario == null && !string.IsNullOrEmpty(SimulateScenario))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Scenario - no database context is set. SimulateScenario: " + SimulateScenario + ".");
                        }
                        return null;
                    }
                    _scenario = base.SoAContext.Scenarios.Find(SimulateScenario);
                    if (_scenario != null)
                    {
                        base.SoAContext.Attach(_scenario);
                    }
                }
                return _scenario;
            }
            set
            {
                if (_scenario != value)
                {
                    _scenario = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_scenario != null)
                    {
                        SimulateScenario = _scenario.ScenarioId;
                    }
                }
            }
        }

        private ObservableCollection<ScenarioCQEffect> _scenarioCQEffects;

        [InverseProperty("CompetencyQuestionRef")]
        public virtual ObservableCollection<ScenarioCQEffect> ScenarioCQEffects
        {
            get
            {
                if (_scenarioCQEffects == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ScenarioCQEffects - no database context is set. CompetencyQuestionId: " + this.CompetencyQuestionId + ".");
                        }
                        _scenarioCQEffects = new ObservableCollection<ScenarioCQEffect>();
                    }
                    else
                    {
                        var items = base.SoAContext.ScenarioCQEffects.Where(x => x.CompetencyQuestion == this.CompetencyQuestionId).ToList<ScenarioCQEffect>();
                        _scenarioCQEffects = new ObservableCollection<ScenarioCQEffect>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _scenarioCQEffects.CollectionChanged += ScenarioCQEffects_CollectionChanged;
                }
                return _scenarioCQEffects;
            }
            private set
            {
                if (_scenarioCQEffects != null)
                {
                    _scenarioCQEffects.CollectionChanged -= ScenarioCQEffects_CollectionChanged;
                }
                _scenarioCQEffects = value;
                if (_scenarioCQEffects != null)
                {
                    _scenarioCQEffects.CollectionChanged += ScenarioCQEffects_CollectionChanged;
                }
            }
        }

        private void ScenarioCQEffects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ScenarioCQEffect>())
                {
                    item.CompetencyQuestion = this.CompetencyQuestionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Scenario;
            _ = this.ScenarioCQEffects;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
