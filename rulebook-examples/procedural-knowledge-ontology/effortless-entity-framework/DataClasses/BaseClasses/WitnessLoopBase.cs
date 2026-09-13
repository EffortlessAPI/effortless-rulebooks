
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
    [Table("WitnessLoops")]
    public class WitnessLoopBase : SoAEntityBase
    {
        [Key]
        public string WitnessLoopId { get; set; }

        // Formula Name (rulebook: ="Loop " & {{LoopNumber}} & ": " & {{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.S("Loop "), F.TextOr(F.Of(this.LoopNumber)), F.S(": "), F.TextOr(F.Of(this.Title))))); set { }
        }

        public decimal LoopNumber { get; set; }
        public string? Title { get; set; }
        public string? Premise { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        // Formula QuestionCount (rulebook: =COUNTIFS(RoleQuestions!{{WitnessLoop}}, {{WitnessLoopId}}))
        [NotMapped]
        public decimal? QuestionCount
        {
            get => F.AsDecimal(F.Memo(this, "QuestionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleQuestion>(base.SoAContext, "RoleQuestions", __c => __c.RoleQuestions), __r => F.CritField(F.Of(__r.WitnessLoop), F.Of(this.WitnessLoopId)))))); set { }
        }

        // Formula IsComplete (rulebook: ={{CompletedAt}} <> "")
        [NotMapped]
        public bool? IsComplete
        {
            get => F.AsBool(F.Memo(this, "IsComplete", () => F.IsNotBlank(F.Of(this.CompletedAt)))); set { }
        }

        public decimal? FieldsAfter { get; set; }
        public decimal? DerivedAfter { get; set; }
        public decimal? WitnessedAfter { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<RoleQuestion> _roleQuestions;

        [InverseProperty("WitnessLoopRef")]
        public virtual ObservableCollection<RoleQuestion> RoleQuestions
        {
            get
            {
                if (_roleQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestions - no database context is set. WitnessLoopId: " + this.WitnessLoopId + ".");
                        }
                        _roleQuestions = new ObservableCollection<RoleQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleQuestions.Where(x => x.WitnessLoop == this.WitnessLoopId).ToList<RoleQuestion>();
                        _roleQuestions = new ObservableCollection<RoleQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
