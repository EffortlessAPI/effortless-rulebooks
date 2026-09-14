
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
    [Table("RoleQuestions")]
    public class RoleQuestionBase : SoAEntityBase
    {
        [Key]
        public string RoleQuestionId { get; set; }

        // Formula Name (rulebook: ={{AskingRole}} & ": " & LEFT({{QuestionText}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.AskingRole)), F.S(": "), F.Text(F.Left(F.Of(this.QuestionText), F.I(60)))))); set { }
        }

        public string? QuestionText { get; set; }
        public string? WhyItMatters { get; set; }
        public bool? AnswerableBefore { get; set; }
        // Formula PredicateCount (rulebook: =COUNTIFS(RulebookFields!{{InventedForQuestion}}, {{RoleQuestionId}}))
        [NotMapped]
        public decimal? PredicateCount
        {
            get => F.AsDecimal(F.Memo(this, "PredicateCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RulebookField>(base.SoAContext, "RulebookFields", __c => __c.RulebookFields), __r => F.CritField(F.Of(__r.InventedForQuestion), F.Of(this.RoleQuestionId)))))); set { }
        }

        // Formula IsAnswered (rulebook: ={{PredicateCount}} > 0)
        [NotMapped]
        public bool? IsAnswered
        {
            get => F.AsBool(F.Memo(this, "IsAnswered", () => F.Cmp(F.Of(this.PredicateCount), ">", F.I(0)))); set { }
        }

        public string? WitnessedAnswer { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? AskingRole { get; set; }
        public string? WitnessLoop { get; set; }

        private Role _role;

        [ForeignKey("AskingRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(AskingRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AskingRole: " + AskingRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(AskingRole);
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
                        AskingRole = _role.RoleId;
                    }
                }
            }
        }

        private WitnessLoop _witnessLoopRef;

        [ForeignKey("WitnessLoop")]
        public virtual WitnessLoop WitnessLoopRef
        {
            get
            {
                if (_witnessLoopRef == null && !string.IsNullOrEmpty(WitnessLoop))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WitnessLoopRef - no database context is set. WitnessLoop: " + WitnessLoop + ".");
                        }
                        return null;
                    }
                    _witnessLoopRef = base.SoAContext.WitnessLoops.Find(WitnessLoop);
                    if (_witnessLoopRef != null)
                    {
                        base.SoAContext.Attach(_witnessLoopRef);
                    }
                }
                return _witnessLoopRef;
            }
            set
            {
                if (_witnessLoopRef != value)
                {
                    _witnessLoopRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_witnessLoopRef != null)
                    {
                        WitnessLoop = _witnessLoopRef.WitnessLoopId;
                    }
                }
            }
        }

        private ObservableCollection<RulebookField> _rulebookFields;

        [InverseProperty("RoleQuestion")]
        public virtual ObservableCollection<RulebookField> RulebookFields
        {
            get
            {
                if (_rulebookFields == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookFields - no database context is set. RoleQuestionId: " + this.RoleQuestionId + ".");
                        }
                        _rulebookFields = new ObservableCollection<RulebookField>();
                    }
                    else
                    {
                        var items = base.SoAContext.RulebookFields.Where(x => x.InventedForQuestion == this.RoleQuestionId).ToList<RulebookField>();
                        _rulebookFields = new ObservableCollection<RulebookField>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _rulebookFields.CollectionChanged += RulebookFields_CollectionChanged;
                }
                return _rulebookFields;
            }
            private set
            {
                if (_rulebookFields != null)
                {
                    _rulebookFields.CollectionChanged -= RulebookFields_CollectionChanged;
                }
                _rulebookFields = value;
                if (_rulebookFields != null)
                {
                    _rulebookFields.CollectionChanged += RulebookFields_CollectionChanged;
                }
            }
        }

        private void RulebookFields_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RulebookField>())
                {
                    item.InventedForQuestion = this.RoleQuestionId;
                }
            }
        }

        private ObservableCollection<TestCase> _testCases;

        [InverseProperty("RoleQuestion")]
        public virtual ObservableCollection<TestCase> TestCases
        {
            get
            {
                if (_testCases == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TestCases - no database context is set. RoleQuestionId: " + this.RoleQuestionId + ".");
                        }
                        _testCases = new ObservableCollection<TestCase>();
                    }
                    else
                    {
                        var items = base.SoAContext.TestCases.Where(x => x.DefendsQuestion == this.RoleQuestionId).ToList<TestCase>();
                        _testCases = new ObservableCollection<TestCase>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _testCases.CollectionChanged += TestCases_CollectionChanged;
                }
                return _testCases;
            }
            private set
            {
                if (_testCases != null)
                {
                    _testCases.CollectionChanged -= TestCases_CollectionChanged;
                }
                _testCases = value;
                if (_testCases != null)
                {
                    _testCases.CollectionChanged += TestCases_CollectionChanged;
                }
            }
        }

        private void TestCases_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TestCase>())
                {
                    item.DefendsQuestion = this.RoleQuestionId;
                }
            }
        }

        private ObservableCollection<AppRouteQuestion> _appRouteQuestions;

        [InverseProperty("RoleQuestion")]
        public virtual ObservableCollection<AppRouteQuestion> AppRouteQuestions
        {
            get
            {
                if (_appRouteQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRouteQuestions - no database context is set. RoleQuestionId: " + this.RoleQuestionId + ".");
                        }
                        _appRouteQuestions = new ObservableCollection<AppRouteQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppRouteQuestions.Where(x => x.Question == this.RoleQuestionId).ToList<AppRouteQuestion>();
                        _appRouteQuestions = new ObservableCollection<AppRouteQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _appRouteQuestions.CollectionChanged += AppRouteQuestions_CollectionChanged;
                }
                return _appRouteQuestions;
            }
            private set
            {
                if (_appRouteQuestions != null)
                {
                    _appRouteQuestions.CollectionChanged -= AppRouteQuestions_CollectionChanged;
                }
                _appRouteQuestions = value;
                if (_appRouteQuestions != null)
                {
                    _appRouteQuestions.CollectionChanged += AppRouteQuestions_CollectionChanged;
                }
            }
        }

        private void AppRouteQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppRouteQuestion>())
                {
                    item.Question = this.RoleQuestionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.WitnessLoopRef;
            _ = this.RulebookFields;
            _ = this.TestCases;
            _ = this.AppRouteQuestions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
