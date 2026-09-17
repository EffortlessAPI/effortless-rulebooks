
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
    [Table("ProcessKnowledgeLevels")]
    public class ProcessKnowledgeLevelBase : SoAEntityBase
    {
        [Key]
        public string ProcessKnowledgeLevelId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? PlanningHorizon { get; set; }
        // Formula TacitStrategyCount (rulebook: =COUNTIFS(LevelCaptureStrategies!{{Level}}, {{ProcessKnowledgeLevelId}}, LevelCaptureStrategies!{{KnowledgeForm}}, "Tacit"))
        [NotMapped]
        public int? TacitStrategyCount
        {
            get => F.AsInt(F.Memo(this, "TacitStrategyCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<LevelCaptureStrategy>(base.SoAContext, "LevelCaptureStrategies", __c => __c.LevelCaptureStrategies), __r => F.CritField(F.Of(__r.Level), F.Of(this.ProcessKnowledgeLevelId)) && F.CritLiteral(F.Of(__r.KnowledgeForm), F.S("Tacit"))))))); set { }
        }

        // Formula ExplicitStrategyCount (rulebook: =COUNTIFS(LevelCaptureStrategies!{{Level}}, {{ProcessKnowledgeLevelId}}, LevelCaptureStrategies!{{KnowledgeForm}}, "Explicit"))
        [NotMapped]
        public int? ExplicitStrategyCount
        {
            get => F.AsInt(F.Memo(this, "ExplicitStrategyCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<LevelCaptureStrategy>(base.SoAContext, "LevelCaptureStrategies", __c => __c.LevelCaptureStrategies), __r => F.CritField(F.Of(__r.Level), F.Of(this.ProcessKnowledgeLevelId)) && F.CritLiteral(F.Of(__r.KnowledgeForm), F.S("Explicit"))))))); set { }
        }

        // Formula LacksCaptureStrategyForEitherForm (rulebook: =OR({{TacitStrategyCount}} = 0, {{ExplicitStrategyCount}} = 0))
        [NotMapped]
        public bool? LacksCaptureStrategyForEitherForm
        {
            get => F.AsBool(F.Memo(this, "LacksCaptureStrategyForEitherForm", () => F.Or(F.Bool3(F.Eq(F.Of(this.TacitStrategyCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.ExplicitStrategyCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<LevelCaptureStrategy> _levelCaptureStrategies;

        [InverseProperty("ProcessKnowledgeLevel")]
        public virtual ObservableCollection<LevelCaptureStrategy> LevelCaptureStrategies
        {
            get
            {
                if (_levelCaptureStrategies == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LevelCaptureStrategies - no database context is set. ProcessKnowledgeLevelId: " + this.ProcessKnowledgeLevelId + ".");
                        }
                        _levelCaptureStrategies = new ObservableCollection<LevelCaptureStrategy>();
                    }
                    else
                    {
                        var items = base.SoAContext.LevelCaptureStrategies.Where(x => x.Level == this.ProcessKnowledgeLevelId).ToList<LevelCaptureStrategy>();
                        _levelCaptureStrategies = new ObservableCollection<LevelCaptureStrategy>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _levelCaptureStrategies.CollectionChanged += LevelCaptureStrategies_CollectionChanged;
                }
                return _levelCaptureStrategies;
            }
            private set
            {
                if (_levelCaptureStrategies != null)
                {
                    _levelCaptureStrategies.CollectionChanged -= LevelCaptureStrategies_CollectionChanged;
                }
                _levelCaptureStrategies = value;
                if (_levelCaptureStrategies != null)
                {
                    _levelCaptureStrategies.CollectionChanged += LevelCaptureStrategies_CollectionChanged;
                }
            }
        }

        private void LevelCaptureStrategies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<LevelCaptureStrategy>())
                {
                    item.Level = this.ProcessKnowledgeLevelId;
                }
            }
        }

        private ObservableCollection<LevelPyramidQuestion> _levelPyramidQuestions;

        [InverseProperty("ProcessKnowledgeLevel")]
        public virtual ObservableCollection<LevelPyramidQuestion> LevelPyramidQuestions
        {
            get
            {
                if (_levelPyramidQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LevelPyramidQuestions - no database context is set. ProcessKnowledgeLevelId: " + this.ProcessKnowledgeLevelId + ".");
                        }
                        _levelPyramidQuestions = new ObservableCollection<LevelPyramidQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.LevelPyramidQuestions.Where(x => x.Level == this.ProcessKnowledgeLevelId).ToList<LevelPyramidQuestion>();
                        _levelPyramidQuestions = new ObservableCollection<LevelPyramidQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _levelPyramidQuestions.CollectionChanged += LevelPyramidQuestions_CollectionChanged;
                }
                return _levelPyramidQuestions;
            }
            private set
            {
                if (_levelPyramidQuestions != null)
                {
                    _levelPyramidQuestions.CollectionChanged -= LevelPyramidQuestions_CollectionChanged;
                }
                _levelPyramidQuestions = value;
                if (_levelPyramidQuestions != null)
                {
                    _levelPyramidQuestions.CollectionChanged += LevelPyramidQuestions_CollectionChanged;
                }
            }
        }

        private void LevelPyramidQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<LevelPyramidQuestion>())
                {
                    item.Level = this.ProcessKnowledgeLevelId;
                }
            }
        }

        private ObservableCollection<ProcessLevelStatement> _processLevelStatements;

        [InverseProperty("ProcessKnowledgeLevel")]
        public virtual ObservableCollection<ProcessLevelStatement> ProcessLevelStatements
        {
            get
            {
                if (_processLevelStatements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessLevelStatements - no database context is set. ProcessKnowledgeLevelId: " + this.ProcessKnowledgeLevelId + ".");
                        }
                        _processLevelStatements = new ObservableCollection<ProcessLevelStatement>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessLevelStatements.Where(x => x.Level == this.ProcessKnowledgeLevelId).ToList<ProcessLevelStatement>();
                        _processLevelStatements = new ObservableCollection<ProcessLevelStatement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _processLevelStatements.CollectionChanged += ProcessLevelStatements_CollectionChanged;
                }
                return _processLevelStatements;
            }
            private set
            {
                if (_processLevelStatements != null)
                {
                    _processLevelStatements.CollectionChanged -= ProcessLevelStatements_CollectionChanged;
                }
                _processLevelStatements = value;
                if (_processLevelStatements != null)
                {
                    _processLevelStatements.CollectionChanged += ProcessLevelStatements_CollectionChanged;
                }
            }
        }

        private void ProcessLevelStatements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessLevelStatement>())
                {
                    item.Level = this.ProcessKnowledgeLevelId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.LevelCaptureStrategies;
            _ = this.LevelPyramidQuestions;
            _ = this.ProcessLevelStatements;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
