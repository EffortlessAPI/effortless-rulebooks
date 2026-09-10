
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("FAQs")]
    public class FAQBase : SoAEntityBase
    {
        [Key]
        public string FaqId { get; set; }

        // Formula Name (rulebook: ={{Question}})
        public string? Name
        {
            get => this.Question; set { }
        }

        public string? Category { get; set; }
        public string? TargetKind { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = Context.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        Context.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    ProcedureVersion = _procedureVersion == null ? default : _procedureVersion.ProcedureVersionId;
                }
            }
        }

        private Step _step;

        [ForeignKey("Step")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(Step))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _step = Context.Steps.Find(Step);
                    if (_step != null)
                    {
                        Context.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    Step = _step == null ? default : _step.StepId;
                }
            }
        }

        private ObservableCollection<UserQuestion> _userQuestions;

        [InverseProperty("FAQ")]
        public virtual ObservableCollection<UserQuestion> UserQuestions
        {
            get
            {
                if (_userQuestions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserQuestions - no database context is set. FaqId: " + this.FaqId + ".");
                        }
                        _userQuestions = new ObservableCollection<UserQuestion>();
                    }
                    else
                    {
                        var items = Context.UserQuestions.Where(x => x.ResolvedByFaq == this.FaqId).ToList<UserQuestion>();
                        _userQuestions = new ObservableCollection<UserQuestion>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _userQuestions.CollectionChanged += UserQuestions_CollectionChanged;
                }
                return _userQuestions;
            }
            private set
            {
                if (_userQuestions != null)
                {
                    _userQuestions.CollectionChanged -= UserQuestions_CollectionChanged;
                }
                _userQuestions = value;
                if (_userQuestions != null)
                {
                    _userQuestions.CollectionChanged += UserQuestions_CollectionChanged;
                }
            }
        }

        private void UserQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<UserQuestion>())
                {
                    item.ResolvedByFaq = this.FaqId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.Step;
            _ = this.UserQuestions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
