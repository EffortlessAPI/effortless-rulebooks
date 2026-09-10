
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
    [Table("Scenarios")]
    public class ScenarioBase : SoAEntityBase
    {
        [Key]
        public string ScenarioId { get; set; }

        // Formula RelativePath (rulebook: ="scenarios/" & {{ScenarioId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("scenarios/"), F.TextOr(F.Of(this.ScenarioId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        // Formula Name (rulebook: =SUBSTITUTE(LOWER({{Label}}), " ", "-"))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Substitute(F.Lower(F.Of(this.Label)), F.S(" "), F.S("-")))); set { }
        }

        public string Label { get; set; }
        public string? Icon { get; set; }
        public string? Explanation { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsReset { get; set; }
        public string Edits { get; set; }


        private ObservableCollection<CompetencyQuestion> _competencyQuestions;

        [InverseProperty("Scenario")]
        public virtual ObservableCollection<CompetencyQuestion> CompetencyQuestions
        {
            get
            {
                if (_competencyQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CompetencyQuestions - no database context is set. ScenarioId: " + this.ScenarioId + ".");
                        }
                        _competencyQuestions = new ObservableCollection<CompetencyQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.CompetencyQuestions.Where(x => x.SimulateScenario == this.ScenarioId).ToList<CompetencyQuestion>();
                        _competencyQuestions = new ObservableCollection<CompetencyQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _competencyQuestions.CollectionChanged += CompetencyQuestions_CollectionChanged;
                }
                return _competencyQuestions;
            }
            private set
            {
                if (_competencyQuestions != null)
                {
                    _competencyQuestions.CollectionChanged -= CompetencyQuestions_CollectionChanged;
                }
                _competencyQuestions = value;
                if (_competencyQuestions != null)
                {
                    _competencyQuestions.CollectionChanged += CompetencyQuestions_CollectionChanged;
                }
            }
        }

        private void CompetencyQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CompetencyQuestion>())
                {
                    item.SimulateScenario = this.ScenarioId;
                }
            }
        }

        private ObservableCollection<ScenarioCQEffect> _scenarioCQEffects;

        [InverseProperty("ScenarioRef")]
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
                            throw new InvalidOperationException("Cannot access ScenarioCQEffects - no database context is set. ScenarioId: " + this.ScenarioId + ".");
                        }
                        _scenarioCQEffects = new ObservableCollection<ScenarioCQEffect>();
                    }
                    else
                    {
                        var items = base.SoAContext.ScenarioCQEffects.Where(x => x.Scenario == this.ScenarioId).ToList<ScenarioCQEffect>();
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
                    item.Scenario = this.ScenarioId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.CompetencyQuestions;
            _ = this.ScenarioCQEffects;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
