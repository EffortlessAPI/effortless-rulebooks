
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
    [Table("CapabilityDeclines")]
    public class CapabilityDeclineBase : SoAEntityBase
    {
        [Key]
        public string CapabilityDeclineId { get; set; }

        // Formula Name (rulebook: ={{Organization}} & ": " & {{Stage}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Organization)), F.S(": "), F.Text(F.Of(this.Stage))))); set { }
        }

        public string? Stage { get; set; }
        public DateTimeOffset? DeclineStartedAt { get; set; }
        public string? Evidence { get; set; }
        // Formula PrecedingStage (rulebook: =INDEX(CapabilityDeclines!{{Stage}}, MATCH({{PrecedingStageDecline}}, CapabilityDeclines!{{CapabilityDeclineId}}, 0)))
        [NotMapped]
        public string? PrecedingStage
        {
            get => F.AsString(F.Memo(this, "PrecedingStage", () => F.Lookup<CapabilityDecline>(this, "CapabilityDeclines", "CapabilityDeclineId", __c => __c.CapabilityDeclines, __r => F.Of(__r.CapabilityDeclineId), F.Of(this.PrecedingStageDecline), __r => F.Of(__r.Stage), () => F.Of(new CapabilityDecline().Stage)))); set { }
        }

        // Formula PrecedingDeclineStartedAt (rulebook: =INDEX(CapabilityDeclines!{{DeclineStartedAt}}, MATCH({{PrecedingStageDecline}}, CapabilityDeclines!{{CapabilityDeclineId}}, 0)))
        [NotMapped]
        public DateTimeOffset? PrecedingDeclineStartedAt
        {
            get => F.AsDateTime(F.Memo(this, "PrecedingDeclineStartedAt", () => F.Lookup<CapabilityDecline>(this, "CapabilityDeclines", "CapabilityDeclineId", __c => __c.CapabilityDeclines, __r => F.Of(__r.CapabilityDeclineId), F.Of(this.PrecedingStageDecline), __r => F.Of(__r.DeclineStartedAt), () => F.Of(new CapabilityDecline().DeclineStartedAt)))); set { }
        }

        // Formula FollowsPrecedingStageDecline (rulebook: =AND({{PrecedingStageDecline}} <> "", {{PrecedingDeclineStartedAt}} < {{DeclineStartedAt}}, OR(AND({{Stage}} = "EngineerTraining", {{PrecedingStage}} = "FacilityInvestment"), AND({{Stage}} = "KnowHowUpkeep", {{PrecedingStage}} = "EngineerTraining"))))
        [NotMapped]
        public bool? FollowsPrecedingStageDecline
        {
            get => F.AsBool(F.Memo(this, "FollowsPrecedingStageDecline", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PrecedingStageDecline))), F.Bool3(F.Cmp(F.Of(this.PrecedingDeclineStartedAt), "<", F.Nullif(F.Of(this.DeclineStartedAt)))), F.Bool3(F.Or(F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Stage)), F.S("EngineerTraining"))), F.Bool3(F.Eq(F.Of(this.PrecedingStage), F.S("FacilityInvestment"))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Stage)), F.S("KnowHowUpkeep"))), F.Bool3(F.Eq(F.Of(this.PrecedingStage), F.S("EngineerTraining")))))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }
        public string? PrecedingStageDecline { get; set; }

        private Organization _organizationRef;

        [ForeignKey("Organization")]
        public virtual Organization OrganizationRef
        {
            get
            {
                if (_organizationRef == null && !string.IsNullOrEmpty(Organization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRef - no database context is set. Organization: " + Organization + ".");
                        }
                        return null;
                    }
                    _organizationRef = base.SoAContext.Organizations.Find(Organization);
                    if (_organizationRef != null)
                    {
                        base.SoAContext.Attach(_organizationRef);
                    }
                }
                return _organizationRef;
            }
            set
            {
                if (_organizationRef != value)
                {
                    _organizationRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organizationRef != null)
                    {
                        Organization = _organizationRef.OrganizationId;
                    }
                }
            }
        }

        private CapabilityDecline _capabilityDecline;

        [ForeignKey("PrecedingStageDecline")]
        public virtual CapabilityDecline CapabilityDecline
        {
            get
            {
                if (_capabilityDecline == null && !string.IsNullOrEmpty(PrecedingStageDecline))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CapabilityDecline - no database context is set. PrecedingStageDecline: " + PrecedingStageDecline + ".");
                        }
                        return null;
                    }
                    _capabilityDecline = base.SoAContext.CapabilityDeclines.Find(PrecedingStageDecline);
                    if (_capabilityDecline != null)
                    {
                        base.SoAContext.Attach(_capabilityDecline);
                    }
                }
                return _capabilityDecline;
            }
            set
            {
                if (_capabilityDecline != value)
                {
                    _capabilityDecline = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_capabilityDecline != null)
                    {
                        PrecedingStageDecline = _capabilityDecline.CapabilityDeclineId;
                    }
                }
            }
        }

        private ObservableCollection<CapabilityDecline> _capabilityDeclines;

        [InverseProperty("CapabilityDecline")]
        public virtual ObservableCollection<CapabilityDecline> CapabilityDeclines
        {
            get
            {
                if (_capabilityDeclines == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CapabilityDeclines - no database context is set. CapabilityDeclineId: " + this.CapabilityDeclineId + ".");
                        }
                        _capabilityDeclines = new ObservableCollection<CapabilityDecline>();
                    }
                    else
                    {
                        var items = base.SoAContext.CapabilityDeclines.Where(x => x.PrecedingStageDecline == this.CapabilityDeclineId).ToList<CapabilityDecline>();
                        _capabilityDeclines = new ObservableCollection<CapabilityDecline>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _capabilityDeclines.CollectionChanged += CapabilityDeclines_CollectionChanged;
                }
                return _capabilityDeclines;
            }
            private set
            {
                if (_capabilityDeclines != null)
                {
                    _capabilityDeclines.CollectionChanged -= CapabilityDeclines_CollectionChanged;
                }
                _capabilityDeclines = value;
                if (_capabilityDeclines != null)
                {
                    _capabilityDeclines.CollectionChanged += CapabilityDeclines_CollectionChanged;
                }
            }
        }

        private void CapabilityDeclines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CapabilityDecline>())
                {
                    item.PrecedingStageDecline = this.CapabilityDeclineId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.CapabilityDecline;
            _ = this.CapabilityDeclines;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
