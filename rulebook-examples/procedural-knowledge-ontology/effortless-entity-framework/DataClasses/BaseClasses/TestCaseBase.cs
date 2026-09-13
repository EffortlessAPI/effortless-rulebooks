
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
    [Table("TestCases")]
    public class TestCaseBase : SoAEntityBase
    {
        [Key]
        public string TestCaseId { get; set; }

        // Formula Name (rulebook: ={{TestKind}} & ": " & {{Subject}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.TestKind)), F.S(": "), F.TextOr(F.Of(this.Subject))))); set { }
        }

        public string? TestKind { get; set; }
        public string? Subject { get; set; }
        public string? TargetTable { get; set; }
        public string? TargetField { get; set; }
        public string? Assertion { get; set; }
        public string? Severity { get; set; }
        // Formula IsBlocking (rulebook: ={{Severity}} = "blocking")
        [NotMapped]
        public bool? IsBlocking
        {
            get => F.AsBool(F.Memo(this, "IsBlocking", () => F.Eq(F.Nullif(F.Of(this.Severity)), F.S("blocking")))); set { }
        }

        public string? LastOutcome { get; set; }
        public string? LastDetail { get; set; }
        public DateTimeOffset? LastRunAt { get; set; }
        // Formula IsPassing (rulebook: ={{LastOutcome}} = "PASS")
        [NotMapped]
        public bool? IsPassing
        {
            get => F.AsBool(F.Memo(this, "IsPassing", () => F.Eq(F.Nullif(F.Of(this.LastOutcome)), F.S("PASS")))); set { }
        }

        // Formula IsFailing (rulebook: ={{LastOutcome}} = "FAIL")
        [NotMapped]
        public bool? IsFailing
        {
            get => F.AsBool(F.Memo(this, "IsFailing", () => F.Eq(F.Nullif(F.Of(this.LastOutcome)), F.S("FAIL")))); set { }
        }

        // Formula NeedsAttention (rulebook: =AND({{IsFailing}}, {{IsBlocking}}))
        [NotMapped]
        public bool? NeedsAttention
        {
            get => F.AsBool(F.Memo(this, "NeedsAttention", () => F.And(F.Bool3(F.Of(this.IsFailing)), F.Bool3(F.Of(this.IsBlocking))))); set { }
        }

        // Formula PassingSuiteKey (rulebook: =IF({{IsPassing}}, {{Suite}}, ""))
        [NotMapped]
        public string? PassingSuiteKey
        {
            get => F.AsString(F.Memo(this, "PassingSuiteKey", () => (F.Truthy(F.Bool3(F.Of(this.IsPassing))) ? F.Of(this.Suite) : F.S("")))); set { }
        }

        // Formula NeedsAttentionSuiteKey (rulebook: =IF({{NeedsAttention}}, {{Suite}}, ""))
        [NotMapped]
        public string? NeedsAttentionSuiteKey
        {
            get => F.AsString(F.Memo(this, "NeedsAttentionSuiteKey", () => (F.Truthy(F.Bool3(F.Of(this.NeedsAttention))) ? F.Of(this.Suite) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? DefendsQuestion { get; set; }
        public string? Suite { get; set; }

        private RoleQuestion _roleQuestion;

        [ForeignKey("DefendsQuestion")]
        public virtual RoleQuestion RoleQuestion
        {
            get
            {
                if (_roleQuestion == null && !string.IsNullOrEmpty(DefendsQuestion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestion - no database context is set. DefendsQuestion: " + DefendsQuestion + ".");
                        }
                        return null;
                    }
                    _roleQuestion = base.SoAContext.RoleQuestions.Find(DefendsQuestion);
                    if (_roleQuestion != null)
                    {
                        base.SoAContext.Attach(_roleQuestion);
                    }
                }
                return _roleQuestion;
            }
            set
            {
                if (_roleQuestion != value)
                {
                    _roleQuestion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleQuestion != null)
                    {
                        DefendsQuestion = _roleQuestion.RoleQuestionId;
                    }
                }
            }
        }

        private TestSuite _testSuite;

        [ForeignKey("Suite")]
        public virtual TestSuite TestSuite
        {
            get
            {
                if (_testSuite == null && !string.IsNullOrEmpty(Suite))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TestSuite - no database context is set. Suite: " + Suite + ".");
                        }
                        return null;
                    }
                    _testSuite = base.SoAContext.TestSuites.Find(Suite);
                    if (_testSuite != null)
                    {
                        base.SoAContext.Attach(_testSuite);
                    }
                }
                return _testSuite;
            }
            set
            {
                if (_testSuite != value)
                {
                    _testSuite = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_testSuite != null)
                    {
                        Suite = _testSuite.TestSuiteId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RoleQuestion;
            _ = this.TestSuite;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
