
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("RoleQuestions")]
    public class RoleQuestionBase : SoAEntityBase
    {
        [Key]
        public string RoleQuestionId { get; set; }

        // Formula Name (rulebook: ={{AskingRole}} & ": " & LEFT({{QuestionText}}, 60))
        public string? Name
        {
            get => this.AskingRole + ": " + LEFT(this.QuestionText, 60); set { }
        }

        public string? QuestionText { get; set; }
        public string? WhyItMatters { get; set; }
        public bool? AnswerableBefore { get; set; }
        // Formula PredicateCount (rulebook: =COUNTIFS(RulebookFields!{{InventedForQuestion}}, {{RoleQuestionId}}))
        public decimal? PredicateCount
        {
            get => COUNTIFS(RulebookFields!this.InventedForQuestion, this.RoleQuestionId); set { }
        }

        // Formula IsAnswered (rulebook: ={{PredicateCount}} > 0)
        public bool? IsAnswered
        {
            get => this.PredicateCount > 0; set { }
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AskingRole: " + AskingRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(AskingRole);
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
                    AskingRole = _role == null ? default : _role.RoleId;
                }
            }
        }

        private WitnessLoop _witnessLoop;

        [ForeignKey("WitnessLoop")]
        public virtual WitnessLoop WitnessLoop
        {
            get
            {
                if (_witnessLoop == null && !string.IsNullOrEmpty(WitnessLoop))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WitnessLoop - no database context is set. WitnessLoop: " + WitnessLoop + ".");
                        }
                        return null;
                    }
                    _witnessLoop = Context.WitnessLoops.Find(WitnessLoop);
                    if (_witnessLoop != null)
                    {
                        Context.Attach(_witnessLoop);
                    }
                }
                return _witnessLoop;
            }
            set
            {
                if (_witnessLoop != value)
                {
                    _witnessLoop = value;
                    WitnessLoop = _witnessLoop == null ? default : _witnessLoop.WitnessLoopId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookFields - no database context is set. RoleQuestionId: " + this.RoleQuestionId + ".");
                        }
                        _rulebookFields = new ObservableCollection<RulebookField>();
                    }
                    else
                    {
                        var items = Context.RulebookFields.Where(x => x.InventedForQuestion == this.RoleQuestionId).ToList<RulebookField>();
                        _rulebookFields = new ObservableCollection<RulebookField>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TestCases - no database context is set. RoleQuestionId: " + this.RoleQuestionId + ".");
                        }
                        _testCases = new ObservableCollection<TestCase>();
                    }
                    else
                    {
                        var items = Context.TestCases.Where(x => x.DefendsQuestion == this.RoleQuestionId).ToList<TestCase>();
                        _testCases = new ObservableCollection<TestCase>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRouteQuestions - no database context is set. RoleQuestionId: " + this.RoleQuestionId + ".");
                        }
                        _appRouteQuestions = new ObservableCollection<AppRouteQuestion>();
                    }
                    else
                    {
                        var items = Context.AppRouteQuestions.Where(x => x.Question == this.RoleQuestionId).ToList<AppRouteQuestion>();
                        _appRouteQuestions = new ObservableCollection<AppRouteQuestion>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.WitnessLoop;
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
