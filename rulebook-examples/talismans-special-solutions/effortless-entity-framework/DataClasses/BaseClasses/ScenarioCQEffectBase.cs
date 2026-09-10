
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
    [Table("ScenarioCQEffects")]
    public class ScenarioCQEffectBase : SoAEntityBase
    {
        [Key]
        public string ScenarioCQEffectId { get; set; }

        // Formula RelativePath (rulebook: ="scenario-cq-effects/" & {{ScenarioCQEffectId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("scenario-cq-effects/"), F.TextOr(F.Of(this.ScenarioCQEffectId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        // Formula Name (rulebook: =SUBSTITUTE(LOWER({{ScenarioCQEffectId}}), " ", "-"))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Substitute(F.Lower(F.Of(this.ScenarioCQEffectId)), F.S(" "), F.S("-")))); set { }
        }

        public string EffectKind { get; set; }
        public string? Note { get; set; }
        public int? SortOrder { get; set; }

        public string Scenario { get; set; }
        public string CompetencyQuestion { get; set; }

        private Scenario _scenarioRef;

        [ForeignKey("Scenario")]
        public virtual Scenario ScenarioRef
        {
            get
            {
                if (_scenarioRef == null && !string.IsNullOrEmpty(Scenario))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ScenarioRef - no database context is set. Scenario: " + Scenario + ".");
                        }
                        return null;
                    }
                    _scenarioRef = base.SoAContext.Scenarios.Find(Scenario);
                    if (_scenarioRef != null)
                    {
                        base.SoAContext.Attach(_scenarioRef);
                    }
                }
                return _scenarioRef;
            }
            set
            {
                if (_scenarioRef != value)
                {
                    _scenarioRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_scenarioRef != null)
                    {
                        Scenario = _scenarioRef.ScenarioId;
                    }
                }
            }
        }

        private CompetencyQuestion _competencyQuestionRef;

        [ForeignKey("CompetencyQuestion")]
        public virtual CompetencyQuestion CompetencyQuestionRef
        {
            get
            {
                if (_competencyQuestionRef == null && !string.IsNullOrEmpty(CompetencyQuestion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CompetencyQuestionRef - no database context is set. CompetencyQuestion: " + CompetencyQuestion + ".");
                        }
                        return null;
                    }
                    _competencyQuestionRef = base.SoAContext.CompetencyQuestions.Find(CompetencyQuestion);
                    if (_competencyQuestionRef != null)
                    {
                        base.SoAContext.Attach(_competencyQuestionRef);
                    }
                }
                return _competencyQuestionRef;
            }
            set
            {
                if (_competencyQuestionRef != value)
                {
                    _competencyQuestionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_competencyQuestionRef != null)
                    {
                        CompetencyQuestion = _competencyQuestionRef.CompetencyQuestionId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ScenarioRef;
            _ = this.CompetencyQuestionRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
