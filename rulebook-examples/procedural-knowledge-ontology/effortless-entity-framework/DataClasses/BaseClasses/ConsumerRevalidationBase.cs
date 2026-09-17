
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
    [Table("ConsumerRevalidations")]
    public class ConsumerRevalidationBase : SoAEntityBase
    {
        [Key]
        public string ConsumerRevalidationId { get; set; }

        // Formula Name (rulebook: ={{RulebookRelease}} & " / " & {{ModelConsumer}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.RulebookRelease)), F.S(" / "), F.Text(F.Of(this.ModelConsumer))))); set { }
        }

        public bool? WasNotified { get; set; }
        public DateTimeOffset? NotifiedAt { get; set; }
        public bool? PassedRevalidation { get; set; }
        public DateTimeOffset? RevalidatedAt { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? RulebookRelease { get; set; }
        public string? ModelConsumer { get; set; }

        private RulebookRelease _rulebookReleaseRef;

        [ForeignKey("RulebookRelease")]
        public virtual RulebookRelease RulebookReleaseRef
        {
            get
            {
                if (_rulebookReleaseRef == null && !string.IsNullOrEmpty(RulebookRelease))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookReleaseRef - no database context is set. RulebookRelease: " + RulebookRelease + ".");
                        }
                        return null;
                    }
                    _rulebookReleaseRef = base.SoAContext.RulebookReleases.Find(RulebookRelease);
                    if (_rulebookReleaseRef != null)
                    {
                        base.SoAContext.Attach(_rulebookReleaseRef);
                    }
                }
                return _rulebookReleaseRef;
            }
            set
            {
                if (_rulebookReleaseRef != value)
                {
                    _rulebookReleaseRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookReleaseRef != null)
                    {
                        RulebookRelease = _rulebookReleaseRef.RulebookReleaseId;
                    }
                }
            }
        }

        private ModelConsumer _modelConsumerRef;

        [ForeignKey("ModelConsumer")]
        public virtual ModelConsumer ModelConsumerRef
        {
            get
            {
                if (_modelConsumerRef == null && !string.IsNullOrEmpty(ModelConsumer))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelConsumerRef - no database context is set. ModelConsumer: " + ModelConsumer + ".");
                        }
                        return null;
                    }
                    _modelConsumerRef = base.SoAContext.ModelConsumers.Find(ModelConsumer);
                    if (_modelConsumerRef != null)
                    {
                        base.SoAContext.Attach(_modelConsumerRef);
                    }
                }
                return _modelConsumerRef;
            }
            set
            {
                if (_modelConsumerRef != value)
                {
                    _modelConsumerRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_modelConsumerRef != null)
                    {
                        ModelConsumer = _modelConsumerRef.ModelConsumerId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RulebookReleaseRef;
            _ = this.ModelConsumerRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
