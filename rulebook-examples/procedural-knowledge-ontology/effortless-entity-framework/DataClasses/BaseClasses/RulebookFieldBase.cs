
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("RulebookFields")]
    public class RulebookFieldBase : SoAEntityBase
    {
        [Key]
        public string RulebookFieldId { get; set; }

        // Formula Name (rulebook: ={{TargetTable}} & "." & {{FieldName}})
        public string? Name
        {
            get => this.TargetTable + "." + this.FieldName; set { }
        }

        public string? TargetTable { get; set; }
        public string? FieldName { get; set; }
        public string? FieldType { get; set; }
        public string? Datatype { get; set; }
        public string? Formula { get; set; }
        // Formula IsDerived (rulebook: =OR({{FieldType}} = "calculated", {{FieldType}} = "lookup", {{FieldType}} = "aggregation"))
        public bool? IsDerived
        {
            get => OR(this.FieldType = "calculated", this.FieldType = "lookup", this.FieldType = "aggregation"); set { }
        }

        // Formula IsWitness (rulebook: ={{InventedForQuestion}} <> "")
        public bool? IsWitness
        {
            get => this.InventedForQuestion <> ""; set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? InventedForQuestion { get; set; }

        private RoleQuestion _roleQuestion;

        [ForeignKey("InventedForQuestion")]
        public virtual RoleQuestion RoleQuestion
        {
            get
            {
                if (_roleQuestion == null && !string.IsNullOrEmpty(InventedForQuestion))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestion - no database context is set. InventedForQuestion: " + InventedForQuestion + ".");
                        }
                        return null;
                    }
                    _roleQuestion = Context.RoleQuestions.Find(InventedForQuestion);
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
                    InventedForQuestion = _roleQuestion == null ? default : _roleQuestion.RoleQuestionId;
                }
            }
        }

        private ObservableCollection<FieldGrant> _fieldGrants;

        [InverseProperty("RulebookField")]
        public virtual ObservableCollection<FieldGrant> FieldGrants
        {
            get
            {
                if (_fieldGrants == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FieldGrants - no database context is set. RulebookFieldId: " + this.RulebookFieldId + ".");
                        }
                        _fieldGrants = new ObservableCollection<FieldGrant>();
                    }
                    else
                    {
                        var items = Context.FieldGrants.Where(x => x.TargetField == this.RulebookFieldId).ToList<FieldGrant>();
                        _fieldGrants = new ObservableCollection<FieldGrant>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _fieldGrants.CollectionChanged += FieldGrants_CollectionChanged;
                }
                return _fieldGrants;
            }
            private set
            {
                if (_fieldGrants != null)
                {
                    _fieldGrants.CollectionChanged -= FieldGrants_CollectionChanged;
                }
                _fieldGrants = value;
                if (_fieldGrants != null)
                {
                    _fieldGrants.CollectionChanged += FieldGrants_CollectionChanged;
                }
            }
        }

        private void FieldGrants_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FieldGrant>())
                {
                    item.TargetField = this.RulebookFieldId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RoleQuestion;
            _ = this.FieldGrants;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
