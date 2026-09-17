
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
    [Table("KnowledgeDeliverables")]
    public class KnowledgeDeliverableBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeDeliverableId { get; set; }

        // Formula Name (rulebook: ={{ProviderEngagement}} & " " & {{Direction}} & ": " & {{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProviderEngagement)), F.S(" "), F.Text(F.Of(this.Direction)), F.S(": "), F.Text(F.Of(this.Title))))); set { }
        }

        public string? Direction { get; set; }
        public string? Title { get; set; }
        public DateTimeOffset? DueAt { get; set; }
        public DateTimeOffset? DeliveredAt { get; set; }
        // Formula IsDelivered (rulebook: ={{DeliveredAt}} <> "")
        [NotMapped]
        public bool? IsDelivered
        {
            get => F.AsBool(F.Memo(this, "IsDelivered", () => F.IsNotBlank(F.Of(this.DeliveredAt)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProviderEngagement { get; set; }

        private ProviderEngagement _providerEngagementRef;

        [ForeignKey("ProviderEngagement")]
        public virtual ProviderEngagement ProviderEngagementRef
        {
            get
            {
                if (_providerEngagementRef == null && !string.IsNullOrEmpty(ProviderEngagement))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProviderEngagementRef - no database context is set. ProviderEngagement: " + ProviderEngagement + ".");
                        }
                        return null;
                    }
                    _providerEngagementRef = base.SoAContext.ProviderEngagements.Find(ProviderEngagement);
                    if (_providerEngagementRef != null)
                    {
                        base.SoAContext.Attach(_providerEngagementRef);
                    }
                }
                return _providerEngagementRef;
            }
            set
            {
                if (_providerEngagementRef != value)
                {
                    _providerEngagementRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_providerEngagementRef != null)
                    {
                        ProviderEngagement = _providerEngagementRef.ProviderEngagementId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProviderEngagementRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
