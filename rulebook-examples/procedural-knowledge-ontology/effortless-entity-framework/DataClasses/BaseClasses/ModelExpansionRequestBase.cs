
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
    [Table("ModelExpansionRequests")]
    public class ModelExpansionRequestBase : SoAEntityBase
    {
        [Key]
        public string ModelExpansionRequestId { get; set; }

        // Formula Name (rulebook: =LEFT({{WorkflowDescription}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Left(F.Of(this.WorkflowDescription), F.I(60)))); set { }
        }

        public string? WorkflowDescription { get; set; }
        public DateTimeOffset? RequestedAt { get; set; }
        public string? FitDecision { get; set; }
        public DateTimeOffset? DecidedAt { get; set; }
        // Formula ModelDomainOwner (rulebook: =INDEX(GovernedModels!{{DomainOwningOrganization}}, MATCH({{GovernedModel}}, GovernedModels!{{GovernedModelId}}, 0)))
        [NotMapped]
        public string? ModelDomainOwner
        {
            get => F.AsString(F.Memo(this, "ModelDomainOwner", () => F.Lookup<GovernedModel>(this, "GovernedModels", "GovernedModelId", __c => __c.GovernedModels, __r => F.Of(__r.GovernedModelId), F.Of(this.GovernedModel), __r => F.Of(__r.DomainOwningOrganization), () => F.Of(new GovernedModel().DomainOwningOrganization)))); set { }
        }

        // Formula ConceptCount (rulebook: =COUNTIFS(ExpansionConceptFits!{{ModelExpansionRequest}}, {{ModelExpansionRequestId}}))
        [NotMapped]
        public int? ConceptCount
        {
            get => F.AsInt(F.Memo(this, "ConceptCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ExpansionConceptFit>(base.SoAContext, "ExpansionConceptFits", __c => __c.ExpansionConceptFits), __r => F.CritField(F.Of(__r.ModelExpansionRequest), F.Of(this.ModelExpansionRequestId))))))); set { }
        }

        // Formula UncoveredConceptCount (rulebook: =COUNTIFS(ExpansionConceptFits!{{ModelExpansionRequest}}, {{ModelExpansionRequestId}}, ExpansionConceptFits!{{IsUncovered}}, TRUE))
        [NotMapped]
        public int? UncoveredConceptCount
        {
            get => F.AsInt(F.Memo(this, "UncoveredConceptCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ExpansionConceptFit>(base.SoAContext, "ExpansionConceptFits", __c => __c.ExpansionConceptFits), __r => F.CritField(F.Of(__r.ModelExpansionRequest), F.Of(this.ModelExpansionRequestId)) && F.CritLiteral(F.Of(__r.IsUncovered), F.B(true))))))); set { }
        }

        // Formula IsCrossFunctionExpansion (rulebook: =AND({{RequestingOrganization}} <> "", {{ModelDomainOwner}} <> "", {{RequestingOrganization}} <> {{ModelDomainOwner}}))
        [NotMapped]
        public bool? IsCrossFunctionExpansion
        {
            get => F.AsBool(F.Memo(this, "IsCrossFunctionExpansion", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.RequestingOrganization))), F.Bool3(F.IsNotBlank(F.Of(this.ModelDomainOwner))), F.Bool3(F.Ne(F.Nullif(F.Of(this.RequestingOrganization)), F.Of(this.ModelDomainOwner)))))); set { }
        }

        // Formula RequiresSchemaExtension (rulebook: ={{UncoveredConceptCount}} > 0)
        [NotMapped]
        public bool? RequiresSchemaExtension
        {
            get => F.AsBool(F.Memo(this, "RequiresSchemaExtension", () => F.Cmp(F.Of(this.UncoveredConceptCount), ">", F.I(0)))); set { }
        }

        // Formula FitDecisionContradictsConceptFit (rulebook: =AND({{FitDecision}} = "FitsExistingSchema", {{UncoveredConceptCount}} > 0))
        [NotMapped]
        public bool? FitDecisionContradictsConceptFit
        {
            get => F.AsBool(F.Memo(this, "FitDecisionContradictsConceptFit", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.FitDecision)), F.S("FitsExistingSchema"))), F.Bool3(F.Cmp(F.Of(this.UncoveredConceptCount), ">", F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? RequestingOrganization { get; set; }
        public string? RequestedByAgent { get; set; }
        public string? DecidedByAgent { get; set; }

        private GovernedModel _governedModelRef;

        [ForeignKey("GovernedModel")]
        public virtual GovernedModel GovernedModelRef
        {
            get
            {
                if (_governedModelRef == null && !string.IsNullOrEmpty(GovernedModel))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModelRef - no database context is set. GovernedModel: " + GovernedModel + ".");
                        }
                        return null;
                    }
                    _governedModelRef = base.SoAContext.GovernedModels.Find(GovernedModel);
                    if (_governedModelRef != null)
                    {
                        base.SoAContext.Attach(_governedModelRef);
                    }
                }
                return _governedModelRef;
            }
            set
            {
                if (_governedModelRef != value)
                {
                    _governedModelRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_governedModelRef != null)
                    {
                        GovernedModel = _governedModelRef.GovernedModelId;
                    }
                }
            }
        }

        private Organization _organization;

        [ForeignKey("RequestingOrganization")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(RequestingOrganization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. RequestingOrganization: " + RequestingOrganization + ".");
                        }
                        return null;
                    }
                    _organization = base.SoAContext.Organizations.Find(RequestingOrganization);
                    if (_organization != null)
                    {
                        base.SoAContext.Attach(_organization);
                    }
                }
                return _organization;
            }
            set
            {
                if (_organization != value)
                {
                    _organization = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organization != null)
                    {
                        RequestingOrganization = _organization.OrganizationId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("RequestedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(RequestedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. RequestedByAgent: " + RequestedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(RequestedByAgent);
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
                        RequestedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("DecidedByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(DecidedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. DecidedByAgent: " + DecidedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(DecidedByAgent);
                    if (_agentRef != null)
                    {
                        base.SoAContext.Attach(_agentRef);
                    }
                }
                return _agentRef;
            }
            set
            {
                if (_agentRef != value)
                {
                    _agentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRef != null)
                    {
                        DecidedByAgent = _agentRef.AgentId;
                    }
                }
            }
        }

        private ObservableCollection<ExpansionConceptFit> _expansionConceptFits;

        [InverseProperty("ModelExpansionRequestRef")]
        public virtual ObservableCollection<ExpansionConceptFit> ExpansionConceptFits
        {
            get
            {
                if (_expansionConceptFits == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExpansionConceptFits - no database context is set. ModelExpansionRequestId: " + this.ModelExpansionRequestId + ".");
                        }
                        _expansionConceptFits = new ObservableCollection<ExpansionConceptFit>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExpansionConceptFits.Where(x => x.ModelExpansionRequest == this.ModelExpansionRequestId).ToList<ExpansionConceptFit>();
                        _expansionConceptFits = new ObservableCollection<ExpansionConceptFit>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _expansionConceptFits.CollectionChanged += ExpansionConceptFits_CollectionChanged;
                }
                return _expansionConceptFits;
            }
            private set
            {
                if (_expansionConceptFits != null)
                {
                    _expansionConceptFits.CollectionChanged -= ExpansionConceptFits_CollectionChanged;
                }
                _expansionConceptFits = value;
                if (_expansionConceptFits != null)
                {
                    _expansionConceptFits.CollectionChanged += ExpansionConceptFits_CollectionChanged;
                }
            }
        }

        private void ExpansionConceptFits_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExpansionConceptFit>())
                {
                    item.ModelExpansionRequest = this.ModelExpansionRequestId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.Organization;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.ExpansionConceptFits;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
