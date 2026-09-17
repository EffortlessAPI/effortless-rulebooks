
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
    [Table("DecisionPoints")]
    public class DecisionPointBase : SoAEntityBase
    {
        [Key]
        public string DecisionPointId { get; set; }

        // Formula Name (rulebook: ={{Step}} & ": " & LEFT({{Question}}, 50))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(": "), F.Text(F.Left(F.Of(this.Question), F.I(50)))))); set { }
        }

        public string? Question { get; set; }
        public string? DecidingFactors { get; set; }
        public string? DefaultOutcome { get; set; }
        public string? DmnDecisionKey { get; set; }
        // Formula HasNoDecidingFactors (rulebook: ={{DecidingFactors}} = "")
        [NotMapped]
        public bool? HasNoDecidingFactors
        {
            get => F.AsBool(F.Memo(this, "HasNoDecidingFactors", () => F.IsBlank(F.Of(this.DecidingFactors)))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        // Formula IsDmnEncoded (rulebook: ={{DmnDecisionKey}} <> "")
        [NotMapped]
        public bool? IsDmnEncoded
        {
            get => F.AsBool(F.Memo(this, "IsDmnEncoded", () => F.IsNotBlank(F.Of(this.DmnDecisionKey)))); set { }
        }


        public string? Step { get; set; }
        public string? GoverningTransition { get; set; }

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

        private StepTransition _stepTransition;

        [ForeignKey("GoverningTransition")]
        public virtual StepTransition StepTransition
        {
            get
            {
                if (_stepTransition == null && !string.IsNullOrEmpty(GoverningTransition))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepTransition - no database context is set. GoverningTransition: " + GoverningTransition + ".");
                        }
                        return null;
                    }
                    _stepTransition = base.SoAContext.StepTransitions.Find(GoverningTransition);
                    if (_stepTransition != null)
                    {
                        base.SoAContext.Attach(_stepTransition);
                    }
                }
                return _stepTransition;
            }
            set
            {
                if (_stepTransition != value)
                {
                    _stepTransition = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepTransition != null)
                    {
                        GoverningTransition = _stepTransition.StepTransitionId;
                    }
                }
            }
        }

        private ObservableCollection<RetrievalSegment> _retrievalSegments;

        [InverseProperty("DecisionPointRef")]
        public virtual ObservableCollection<RetrievalSegment> RetrievalSegments
        {
            get
            {
                if (_retrievalSegments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RetrievalSegments - no database context is set. DecisionPointId: " + this.DecisionPointId + ".");
                        }
                        _retrievalSegments = new ObservableCollection<RetrievalSegment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RetrievalSegments.Where(x => x.DecisionPoint == this.DecisionPointId).ToList<RetrievalSegment>();
                        _retrievalSegments = new ObservableCollection<RetrievalSegment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _retrievalSegments.CollectionChanged += RetrievalSegments_CollectionChanged;
                }
                return _retrievalSegments;
            }
            private set
            {
                if (_retrievalSegments != null)
                {
                    _retrievalSegments.CollectionChanged -= RetrievalSegments_CollectionChanged;
                }
                _retrievalSegments = value;
                if (_retrievalSegments != null)
                {
                    _retrievalSegments.CollectionChanged += RetrievalSegments_CollectionChanged;
                }
            }
        }

        private void RetrievalSegments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RetrievalSegment>())
                {
                    item.DecisionPoint = this.DecisionPointId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.StepTransition;
            _ = this.RetrievalSegments;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
