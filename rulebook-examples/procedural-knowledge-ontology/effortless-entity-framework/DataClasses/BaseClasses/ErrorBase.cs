
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Errors")]
    public class ErrorBase : SoAEntityBase
    {
        [Key]
        public string ErrorId { get; set; }

        // Formula Name (rulebook: ={{ErrorCode}} & " - " & {{Label}})
        public string? Name
        {
            get => this.ErrorCode + " - " + this.Label; set { }
        }

        public string? Label { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorCause { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<IssueOccurrence> _issueOccurrences;

        [InverseProperty("Error")]
        public virtual ObservableCollection<IssueOccurrence> IssueOccurrences
        {
            get
            {
                if (_issueOccurrences == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IssueOccurrences - no database context is set. ErrorId: " + this.ErrorId + ".");
                        }
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>();
                    }
                    else
                    {
                        var items = Context.IssueOccurrences.Where(x => x.Error == this.ErrorId).ToList<IssueOccurrence>();
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.IssueOccurrences;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
