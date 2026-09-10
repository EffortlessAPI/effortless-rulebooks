
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("WitnessLoops")]
    public class WitnessLoopBase : SoAEntityBase
    {
        [Key]
        public string WitnessLoopId { get; set; }

        // Formula Name (rulebook: ="Loop " & {{LoopNumber}} & ": " & {{Title}})
        public string? Name
        {
            get => "Loop " + this.LoopNumber + ": " + this.Title; set { }
        }

        public decimal LoopNumber { get; set; }
        public string? Title { get; set; }
        public string? Premise { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        // Formula QuestionCount (rulebook: =COUNTIFS(RoleQuestions!{{WitnessLoop}}, {{WitnessLoopId}}))
        public decimal? QuestionCount
        {
            get => COUNTIFS(RoleQuestions!this.WitnessLoop, this.WitnessLoopId); set { }
        }

        // Formula IsComplete (rulebook: ={{CompletedAt}} <> "")
        public bool? IsComplete
        {
            get => this.CompletedAt <> ""; set { }
        }

        public decimal? FieldsAfter { get; set; }
        public decimal? DerivedAfter { get; set; }
        public decimal? WitnessedAfter { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<RoleQuestion> _roleQuestions;

        [InverseProperty("WitnessLoop")]
        public virtual ObservableCollection<RoleQuestion> RoleQuestions
        {
            get
            {
                if (_roleQuestions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestions - no database context is set. WitnessLoopId: " + this.WitnessLoopId + ".");
                        }
                        _roleQuestions = new ObservableCollection<RoleQuestion>();
                    }
                    else
                    {
                        var items = Context.RoleQuestions.Where(x => x.WitnessLoop == this.WitnessLoopId).ToList<RoleQuestion>();
                        _roleQuestions = new ObservableCollection<RoleQuestion>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _roleQuestions.CollectionChanged += RoleQuestions_CollectionChanged;
                }
                return _roleQuestions;
            }
            private set
            {
                if (_roleQuestions != null)
                {
                    _roleQuestions.CollectionChanged -= RoleQuestions_CollectionChanged;
                }
                _roleQuestions = value;
                if (_roleQuestions != null)
                {
                    _roleQuestions.CollectionChanged += RoleQuestions_CollectionChanged;
                }
            }
        }

        private void RoleQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleQuestion>())
                {
                    item.WitnessLoop = this.WitnessLoopId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RoleQuestions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
