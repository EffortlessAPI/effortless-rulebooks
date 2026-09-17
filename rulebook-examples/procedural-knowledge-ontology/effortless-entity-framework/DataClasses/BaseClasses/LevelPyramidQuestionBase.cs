
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
    [Table("LevelPyramidQuestions")]
    public class LevelPyramidQuestionBase : SoAEntityBase
    {
        [Key]
        public string LevelPyramidQuestionId { get; set; }

        // Formula Name (rulebook: ={{Level}} & " answers " & {{QuestionKind}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Level)), F.S(" answers "), F.Text(F.Of(this.QuestionKind))))); set { }
        }

        public string? QuestionKind { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? Level { get; set; }

        private ProcessKnowledgeLevel _processKnowledgeLevel;

        [ForeignKey("Level")]
        public virtual ProcessKnowledgeLevel ProcessKnowledgeLevel
        {
            get
            {
                if (_processKnowledgeLevel == null && !string.IsNullOrEmpty(Level))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessKnowledgeLevel - no database context is set. Level: " + Level + ".");
                        }
                        return null;
                    }
                    _processKnowledgeLevel = base.SoAContext.ProcessKnowledgeLevels.Find(Level);
                    if (_processKnowledgeLevel != null)
                    {
                        base.SoAContext.Attach(_processKnowledgeLevel);
                    }
                }
                return _processKnowledgeLevel;
            }
            set
            {
                if (_processKnowledgeLevel != value)
                {
                    _processKnowledgeLevel = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_processKnowledgeLevel != null)
                    {
                        Level = _processKnowledgeLevel.ProcessKnowledgeLevelId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcessKnowledgeLevel;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
