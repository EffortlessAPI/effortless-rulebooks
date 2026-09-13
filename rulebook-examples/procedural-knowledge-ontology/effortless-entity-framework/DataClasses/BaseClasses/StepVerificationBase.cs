
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
    [Table("StepVerifications")]
    public class StepVerificationBase : SoAEntityBase
    {
        [Key]
        public string StepVerificationId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{VerificationKind}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.Step)), F.S(" / "), F.TextOr(F.Of(this.VerificationKind))))); set { }
        }

        public string? VerificationKind { get; set; }
        public string? SignalIdentifier { get; set; }
        public string? ExpectedSignalValue { get; set; }
        public string? Instruction { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? Step { get; set; }

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

        private ObservableCollection<VerificationOutcome> _verificationOutcomes;

        [InverseProperty("StepVerificationRef")]
        public virtual ObservableCollection<VerificationOutcome> VerificationOutcomes
        {
            get
            {
                if (_verificationOutcomes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VerificationOutcomes - no database context is set. StepVerificationId: " + this.StepVerificationId + ".");
                        }
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>();
                    }
                    else
                    {
                        var items = base.SoAContext.VerificationOutcomes.Where(x => x.StepVerification == this.StepVerificationId).ToList<VerificationOutcome>();
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _verificationOutcomes.CollectionChanged += VerificationOutcomes_CollectionChanged;
                }
                return _verificationOutcomes;
            }
            private set
            {
                if (_verificationOutcomes != null)
                {
                    _verificationOutcomes.CollectionChanged -= VerificationOutcomes_CollectionChanged;
                }
                _verificationOutcomes = value;
                if (_verificationOutcomes != null)
                {
                    _verificationOutcomes.CollectionChanged += VerificationOutcomes_CollectionChanged;
                }
            }
        }

        private void VerificationOutcomes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<VerificationOutcome>())
                {
                    item.StepVerification = this.StepVerificationId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.VerificationOutcomes;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
