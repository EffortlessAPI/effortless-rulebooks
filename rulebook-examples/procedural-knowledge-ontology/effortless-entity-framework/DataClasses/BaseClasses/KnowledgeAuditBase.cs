
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
    [Table("KnowledgeAudits")]
    public class KnowledgeAuditBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeAuditId { get; set; }

        // Formula Name (rulebook: ={{Organization}} & ": " & {{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Organization)), F.S(": "), F.Text(F.Of(this.Title))))); set { }
        }

        public string? Title { get; set; }
        public DateTimeOffset? ConductedAt { get; set; }
        public string? ProblemFraming { get; set; }
        // Formula FindingCount (rulebook: =COUNTIFS(KnowledgeAuditItems!{{KnowledgeAudit}}, {{KnowledgeAuditId}}, KnowledgeAuditItems!{{HasInternalShortfall}}, TRUE))
        [NotMapped]
        public int? FindingCount
        {
            get => F.AsInt(F.Memo(this, "FindingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeAuditItem>(base.SoAContext, "KnowledgeAuditItems", __c => __c.KnowledgeAuditItems), __r => F.CritField(F.Of(__r.KnowledgeAudit), F.Of(this.KnowledgeAuditId)) && F.CritLiteral(F.Of(__r.HasInternalShortfall), F.B(true))))))); set { }
        }

        // Formula TreatsDeficitAsCostProblem (rulebook: =AND({{FindingCount}} > 0, {{ProblemFraming}} <> "KnowledgeDeficit"))
        [NotMapped]
        public bool? TreatsDeficitAsCostProblem
        {
            get => F.AsBool(F.Memo(this, "TreatsDeficitAsCostProblem", () => F.And(F.Bool3(F.Cmp(F.Of(this.FindingCount), ">", F.I(0))), F.Bool3(F.Ne(F.Nullif(F.Of(this.ProblemFraming)), F.S("KnowledgeDeficit")))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }
        public string? ConductedByAgent { get; set; }

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

        private Agent _agent;

        [ForeignKey("ConductedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ConductedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ConductedByAgent: " + ConductedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ConductedByAgent);
                    if (_agent != null)
                    {
                        base.SoAContext.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agent != null)
                    {
                        ConductedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private ObservableCollection<KnowledgeAuditItem> _knowledgeAuditItems;

        [InverseProperty("KnowledgeAuditRef")]
        public virtual ObservableCollection<KnowledgeAuditItem> KnowledgeAuditItems
        {
            get
            {
                if (_knowledgeAuditItems == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeAuditItems - no database context is set. KnowledgeAuditId: " + this.KnowledgeAuditId + ".");
                        }
                        _knowledgeAuditItems = new ObservableCollection<KnowledgeAuditItem>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeAuditItems.Where(x => x.KnowledgeAudit == this.KnowledgeAuditId).ToList<KnowledgeAuditItem>();
                        _knowledgeAuditItems = new ObservableCollection<KnowledgeAuditItem>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeAuditItems.CollectionChanged += KnowledgeAuditItems_CollectionChanged;
                }
                return _knowledgeAuditItems;
            }
            private set
            {
                if (_knowledgeAuditItems != null)
                {
                    _knowledgeAuditItems.CollectionChanged -= KnowledgeAuditItems_CollectionChanged;
                }
                _knowledgeAuditItems = value;
                if (_knowledgeAuditItems != null)
                {
                    _knowledgeAuditItems.CollectionChanged += KnowledgeAuditItems_CollectionChanged;
                }
            }
        }

        private void KnowledgeAuditItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeAuditItem>())
                {
                    item.KnowledgeAudit = this.KnowledgeAuditId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.Agent;
            _ = this.KnowledgeAuditItems;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
