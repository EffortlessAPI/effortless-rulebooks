
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("TestCases")]
    public class TestCaseBase : SoAEntityBase
    {
        [Key]
        public string TestCaseId { get; set; }

        // Formula Name (rulebook: ={{TestKind}} & ": " & {{Subject}})
        public string? Name
        {
            get => this.TestKind + ": " + this.Subject; set { }
        }

        public string? TestKind { get; set; }
        public string? Subject { get; set; }
        public string? TargetTable { get; set; }
        public string? TargetField { get; set; }
        public string? Assertion { get; set; }
        public string? Severity { get; set; }
        // Formula IsBlocking (rulebook: ={{Severity}} = "blocking")
        public bool? IsBlocking
        {
            get => this.Severity = "blocking"; set { }
        }

        public string? LastOutcome { get; set; }
        public string? LastDetail { get; set; }
        public DateTime? LastRunAt { get; set; }
        // Formula IsPassing (rulebook: ={{LastOutcome}} = "PASS")
        public bool? IsPassing
        {
            get => this.LastOutcome = "PASS"; set { }
        }

        // Formula IsFailing (rulebook: ={{LastOutcome}} = "FAIL")
        public bool? IsFailing
        {
            get => this.LastOutcome = "FAIL"; set { }
        }

        // Formula NeedsAttention (rulebook: =AND({{IsFailing}}, {{IsBlocking}}))
        public bool? NeedsAttention
        {
            get => AND(this.IsFailing, this.IsBlocking); set { }
        }

        // Formula PassingSuiteKey (rulebook: =IF({{IsPassing}}, {{Suite}}, ""))
        public string? PassingSuiteKey
        {
            get => IF(this.IsPassing, this.Suite, ""); set { }
        }

        // Formula NeedsAttentionSuiteKey (rulebook: =IF({{NeedsAttention}}, {{Suite}}, ""))
        public string? NeedsAttentionSuiteKey
        {
            get => IF(this.NeedsAttention, this.Suite, ""); set { }
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestion - no database context is set. DefendsQuestion: " + DefendsQuestion + ".");
                        }
                        return null;
                    }
                    _roleQuestion = Context.RoleQuestions.Find(DefendsQuestion);
                    if (_roleQuestion != null)
                    {
                        Context.Attach(_roleQuestion);
                    }
                }
                return _roleQuestion;
            }
            set
            {
                if (_roleQuestion != value)
                {
                    _roleQuestion = value;
                    DefendsQuestion = _roleQuestion == null ? default : _roleQuestion.RoleQuestionId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TestSuite - no database context is set. Suite: " + Suite + ".");
                        }
                        return null;
                    }
                    _testSuite = Context.TestSuites.Find(Suite);
                    if (_testSuite != null)
                    {
                        Context.Attach(_testSuite);
                    }
                }
                return _testSuite;
            }
            set
            {
                if (_testSuite != value)
                {
                    _testSuite = value;
                    Suite = _testSuite == null ? default : _testSuite.TestSuiteId;
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
