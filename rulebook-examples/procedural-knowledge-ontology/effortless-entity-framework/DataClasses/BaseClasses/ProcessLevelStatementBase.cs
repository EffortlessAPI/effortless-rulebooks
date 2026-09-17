
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
    [Table("ProcessLevelStatements")]
    public class ProcessLevelStatementBase : SoAEntityBase
    {
        [Key]
        public string ProcessLevelStatementId { get; set; }

        // Formula Name (rulebook: ={{Procedure}} & " " & {{Level}} & " " & {{QuestionKind}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Procedure)), F.S(" "), F.Text(F.Of(this.Level)), F.S(" "), F.Text(F.Of(this.QuestionKind))))); set { }
        }

        public string? QuestionKind { get; set; }
        public string? Statement { get; set; }
        // Formula LevelQuestionKey (rulebook: ={{Level}} & "|" & {{QuestionKind}})
        [NotMapped]
        public string? LevelQuestionKey
        {
            get => F.AsString(F.Memo(this, "LevelQuestionKey", () => F.Concat(F.Text(F.Of(this.Level)), F.S("|"), F.Text(F.Of(this.QuestionKind))))); set { }
        }

        // Formula PyramidMatchCount (rulebook: =COUNTIFS(LevelPyramidQuestions!{{LevelPyramidQuestionId}}, {{LevelQuestionKey}}))
        [NotMapped]
        public int? PyramidMatchCount
        {
            get => F.AsInt(F.Memo(this, "PyramidMatchCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<LevelPyramidQuestion>(base.SoAContext, "LevelPyramidQuestions", __c => __c.LevelPyramidQuestions), __r => F.CritField(F.Of(__r.LevelPyramidQuestionId), F.Of(this.LevelQuestionKey))))))); set { }
        }

        // Formula IsFiledAtWrongLevel (rulebook: ={{PyramidMatchCount}} = 0)
        [NotMapped]
        public bool? IsFiledAtWrongLevel
        {
            get => F.AsBool(F.Memo(this, "IsFiledAtWrongLevel", () => F.Eq(F.Of(this.PyramidMatchCount), F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
        public string? Level { get; set; }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }

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
            _ = this.ProcedureRef;
            _ = this.ProcessKnowledgeLevel;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
