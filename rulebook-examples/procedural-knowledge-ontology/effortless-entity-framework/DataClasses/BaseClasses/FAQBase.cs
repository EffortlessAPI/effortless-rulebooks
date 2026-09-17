
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
    [Table("FAQs")]
    public class FAQBase : SoAEntityBase
    {
        [Key]
        public string FaqId { get; set; }

        // Formula Name (rulebook: ={{Question}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Question))); set { }
        }

        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? SemanticTypeIri { get; set; }
        // Formula ResolutionCount (rulebook: =COUNTIFS(UserQuestions!{{ResolvedByFaq}}, {{FaqId}}))
        [NotMapped]
        public int? ResolutionCount
        {
            get => F.AsInt(F.Memo(this, "ResolutionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<UserQuestion>(base.SoAContext, "UserQuestions", __c => __c.UserQuestions), __r => F.CritField(F.Of(__r.ResolvedByFaq), F.Of(this.FaqId))))))); set { }
        }

        // Formula IsUnusedFaq (rulebook: ={{ResolutionCount}} = 0)
        [NotMapped]
        public bool? IsUnusedFaq
        {
            get => F.AsBool(F.Memo(this, "IsUnusedFaq", () => F.Eq(F.Of(this.ResolutionCount), F.I(0)))); set { }
        }


        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? Category { get; set; }
        public string? TargetKind { get; set; }
        public string? Resource { get; set; }

        private ProcedureVersion _procedureVersionRef;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersionRef
        {
            get
            {
                if (_procedureVersionRef == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionRef - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersionRef = base.SoAContext.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersionRef != null)
                    {
                        base.SoAContext.Attach(_procedureVersionRef);
                    }
                }
                return _procedureVersionRef;
            }
            set
            {
                if (_procedureVersionRef != value)
                {
                    _procedureVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersionRef != null)
                    {
                        ProcedureVersion = _procedureVersionRef.ProcedureVersionId;
                    }
                }
            }
        }

        private Step _stepRef;

        [ForeignKey("Step")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(Step))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(Step);
                    if (_stepRef != null)
                    {
                        base.SoAContext.Attach(_stepRef);
                    }
                }
                return _stepRef;
            }
            set
            {
                if (_stepRef != value)
                {
                    _stepRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepRef != null)
                    {
                        Step = _stepRef.StepId;
                    }
                }
            }
        }

        private FaqCategory _faqCategory;

        [ForeignKey("Category")]
        public virtual FaqCategory FaqCategory
        {
            get
            {
                if (_faqCategory == null && !string.IsNullOrEmpty(Category))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FaqCategory - no database context is set. Category: " + Category + ".");
                        }
                        return null;
                    }
                    _faqCategory = base.SoAContext.FaqCategories.Find(Category);
                    if (_faqCategory != null)
                    {
                        base.SoAContext.Attach(_faqCategory);
                    }
                }
                return _faqCategory;
            }
            set
            {
                if (_faqCategory != value)
                {
                    _faqCategory = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_faqCategory != null)
                    {
                        Category = _faqCategory.FaqCategoryId;
                    }
                }
            }
        }

        private FaqTarget _faqTarget;

        [ForeignKey("TargetKind")]
        public virtual FaqTarget FaqTarget
        {
            get
            {
                if (_faqTarget == null && !string.IsNullOrEmpty(TargetKind))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FaqTarget - no database context is set. TargetKind: " + TargetKind + ".");
                        }
                        return null;
                    }
                    _faqTarget = base.SoAContext.FaqTargets.Find(TargetKind);
                    if (_faqTarget != null)
                    {
                        base.SoAContext.Attach(_faqTarget);
                    }
                }
                return _faqTarget;
            }
            set
            {
                if (_faqTarget != value)
                {
                    _faqTarget = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_faqTarget != null)
                    {
                        TargetKind = _faqTarget.FaqTargetId;
                    }
                }
            }
        }

        private Resource _resourceRef;

        [ForeignKey("Resource")]
        public virtual Resource ResourceRef
        {
            get
            {
                if (_resourceRef == null && !string.IsNullOrEmpty(Resource))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ResourceRef - no database context is set. Resource: " + Resource + ".");
                        }
                        return null;
                    }
                    _resourceRef = base.SoAContext.Resources.Find(Resource);
                    if (_resourceRef != null)
                    {
                        base.SoAContext.Attach(_resourceRef);
                    }
                }
                return _resourceRef;
            }
            set
            {
                if (_resourceRef != value)
                {
                    _resourceRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_resourceRef != null)
                    {
                        Resource = _resourceRef.ResourceId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserQuestions - no database context is set. FaqId: " + this.FaqId + ".");
                        }
                        _userQuestions = new ObservableCollection<UserQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.UserQuestions.Where(x => x.ResolvedByFaq == this.FaqId).ToList<UserQuestion>();
                        _userQuestions = new ObservableCollection<UserQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
            _ = this.ProcedureVersionRef;
            _ = this.StepRef;
            _ = this.FaqCategory;
            _ = this.FaqTarget;
            _ = this.ResourceRef;
            _ = this.UserQuestions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
