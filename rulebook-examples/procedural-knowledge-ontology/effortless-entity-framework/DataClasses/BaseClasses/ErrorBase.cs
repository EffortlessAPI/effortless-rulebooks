
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
    [Table("Errors")]
    public class ErrorBase : SoAEntityBase
    {
        [Key]
        public string ErrorId { get; set; }

        // Formula Name (rulebook: ={{ErrorCode}} & " - " & {{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ErrorCode)), F.S(" - "), F.Text(F.Of(this.Label))))); set { }
        }

        public string? Label { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorCause { get; set; }
        public string? SemanticTypeIri { get; set; }
        public string? Description { get; set; }
        // Formula RemedyStepCount (rulebook: =COUNTIFS(Steps!{{RemedyForError}}, {{ErrorId}}))
        [NotMapped]
        public int? RemedyStepCount
        {
            get => F.AsInt(F.Memo(this, "RemedyStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.RemedyForError), F.Of(this.ErrorId))))))); set { }
        }

        // Formula OccurrenceCount (rulebook: =COUNTIFS(IssueOccurrences!{{Error}}, {{ErrorId}}))
        [NotMapped]
        public int? OccurrenceCount
        {
            get => F.AsInt(F.Memo(this, "OccurrenceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<IssueOccurrence>(base.SoAContext, "IssueOccurrences", __c => __c.IssueOccurrences), __r => F.CritField(F.Of(__r.Error), F.Of(this.ErrorId))))))); set { }
        }

        // Formula HasNoRemedyStep (rulebook: =AND({{OccurrenceCount}} > 0, {{RemedyStepCount}} = 0))
        [NotMapped]
        public bool? HasNoRemedyStep
        {
            get => F.AsBool(F.Memo(this, "HasNoRemedyStep", () => F.And(F.Bool3(F.Cmp(F.Of(this.OccurrenceCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.RemedyStepCount), F.I(0)))))); set { }
        }



        private ObservableCollection<Step> _steps;

        [InverseProperty("Error")]
        public virtual ObservableCollection<Step> Steps
        {
            get
            {
                if (_steps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Steps - no database context is set. ErrorId: " + this.ErrorId + ".");
                        }
                        _steps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = base.SoAContext.Steps.Where(x => x.RemedyForError == this.ErrorId).ToList<Step>();
                        _steps = new ObservableCollection<Step>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _steps.CollectionChanged += Steps_CollectionChanged;
                }
                return _steps;
            }
            private set
            {
                if (_steps != null)
                {
                    _steps.CollectionChanged -= Steps_CollectionChanged;
                }
                _steps = value;
                if (_steps != null)
                {
                    _steps.CollectionChanged += Steps_CollectionChanged;
                }
            }
        }

        private void Steps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Step>())
                {
                    item.RemedyForError = this.ErrorId;
                }
            }
        }

        private ObservableCollection<IssueOccurrence> _issueOccurrences;

        [InverseProperty("ErrorRef")]
        public virtual ObservableCollection<IssueOccurrence> IssueOccurrences
        {
            get
            {
                if (_issueOccurrences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IssueOccurrences - no database context is set. ErrorId: " + this.ErrorId + ".");
                        }
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>();
                    }
                    else
                    {
                        var items = base.SoAContext.IssueOccurrences.Where(x => x.Error == this.ErrorId).ToList<IssueOccurrence>();
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _issueOccurrences.CollectionChanged += IssueOccurrences_CollectionChanged;
                }
                return _issueOccurrences;
            }
            private set
            {
                if (_issueOccurrences != null)
                {
                    _issueOccurrences.CollectionChanged -= IssueOccurrences_CollectionChanged;
                }
                _issueOccurrences = value;
                if (_issueOccurrences != null)
                {
                    _issueOccurrences.CollectionChanged += IssueOccurrences_CollectionChanged;
                }
            }
        }

        private void IssueOccurrences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<IssueOccurrence>())
                {
                    item.Error = this.ErrorId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Steps;
            _ = this.IssueOccurrences;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
