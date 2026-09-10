
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("StepVerifications")]
    public class StepVerificationBase : SoAEntityBase
    {
        [Key]
        public string StepVerificationId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{VerificationKind}})
        public string? Name
        {
            get => this.Step + " / " + this.VerificationKind; set { }
        }

        public string? VerificationKind { get; set; }
        public string? SignalIdentifier { get; set; }
        public string? ExpectedSignalValue { get; set; }
        public string? Instruction { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? Step { get; set; }

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

        private ObservableCollection<VerificationOutcome> _verificationOutcomes;

        [InverseProperty("StepVerification")]
        public virtual ObservableCollection<VerificationOutcome> VerificationOutcomes
        {
            get
            {
                if (_verificationOutcomes == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VerificationOutcomes - no database context is set. StepVerificationId: " + this.StepVerificationId + ".");
                        }
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>();
                    }
                    else
                    {
                        var items = Context.VerificationOutcomes.Where(x => x.StepVerification == this.StepVerificationId).ToList<VerificationOutcome>();
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.Step;
            _ = this.VerificationOutcomes;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
