
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
    [Table("PromptTemplates")]
    public class PromptTemplateBase : SoAEntityBase
    {
        [Key]
        public string PromptTemplateId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Layer { get; set; }
        public string? LibraryStatus { get; set; }
        public bool? CarriesProcedureInstructions { get; set; }
        public bool? IncludesRareDetail { get; set; }
        public int? TokenBudget { get; set; }
        public int? EstimatedTokens { get; set; }
        public int? SourceTokenCount { get; set; }
        // Formula ChildTemplateCount (rulebook: =COUNTIFS(PromptTemplates!{{ParentTemplate}}, PromptTemplates!{{PromptTemplateId}}))
        [NotMapped]
        public int? ChildTemplateCount
        {
            get => F.AsInt(F.Memo(this, "ChildTemplateCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<PromptTemplate>(base.SoAContext, "PromptTemplates", __c => __c.PromptTemplates), __r => F.CritField(F.Of(__r.ParentTemplate), F.Of(this.PromptTemplateId))))))); set { }
        }

        // Formula IsDisconnectedPromptKnowledge (rulebook: =AND({{CarriesProcedureInstructions}}, {{SourceProcedureVersion}} = ""))
        [NotMapped]
        public bool? IsDisconnectedPromptKnowledge
        {
            get => F.AsBool(F.Memo(this, "IsDisconnectedPromptKnowledge", () => F.And(F.IsTrueV(F.Of(this.CarriesProcedureInstructions)), F.Bool3(F.IsBlank(F.Of(this.SourceProcedureVersion)))))); set { }
        }

        // Formula IsUnmanagedPromptInUse (rulebook: =AND({{UsedByAgent}} <> "", OR({{LibraryStatus}} <> "Maintained", {{MaintainedByRole}} = "")))
        [NotMapped]
        public bool? IsUnmanagedPromptInUse
        {
            get => F.AsBool(F.Memo(this, "IsUnmanagedPromptInUse", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.UsedByAgent))), F.Bool3(F.Or(F.Bool3(F.Ne(F.Nullif(F.Of(this.LibraryStatus)), F.S("Maintained"))), F.Bool3(F.IsBlank(F.Of(this.MaintainedByRole)))))))); set { }
        }

        // Formula SpendsBudgetOnRareDetail (rulebook: =AND({{IncludesRareDetail}}, {{EstimatedTokens}} > {{TokenBudget}}))
        [NotMapped]
        public bool? SpendsBudgetOnRareDetail
        {
            get => F.AsBool(F.Memo(this, "SpendsBudgetOnRareDetail", () => F.And(F.IsTrueV(F.Of(this.IncludesRareDetail)), F.Bool3(F.Cmp(F.Nullif(F.Of(this.EstimatedTokens)), ">", F.Nullif(F.Of(this.TokenBudget))))))); set { }
        }

        // Formula CondensationPercent (rulebook: =IF({{SourceTokenCount}} = 0, 0, ROUND(100 * {{EstimatedTokens}} / {{SourceTokenCount}}, 1)))
        [NotMapped]
        public decimal? CondensationPercent
        {
            get => F.AsDecimal(F.Memo(this, "CondensationPercent", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.SourceTokenCount)), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.EstimatedTokens)), F.Of(this.SourceTokenCount)), F.I(1))))); set { }
        }

        // Formula IsVerbatimDump (rulebook: =AND({{SourceTokenCount}} > 0, {{CondensationPercent}} >= 90))
        [NotMapped]
        public bool? IsVerbatimDump
        {
            get => F.AsBool(F.Memo(this, "IsVerbatimDump", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.SourceTokenCount)), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.CondensationPercent), ">=", F.I(90)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ParentTemplate { get; set; }
        public string? SourceProcedureVersion { get; set; }
        public string? MaintainedByRole { get; set; }
        public string? UsedByAgent { get; set; }

        private PromptTemplate _promptTemplate;

        [ForeignKey("ParentTemplate")]
        public virtual PromptTemplate PromptTemplate
        {
            get
            {
                if (_promptTemplate == null && !string.IsNullOrEmpty(ParentTemplate))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PromptTemplate - no database context is set. ParentTemplate: " + ParentTemplate + ".");
                        }
                        return null;
                    }
                    _promptTemplate = base.SoAContext.PromptTemplates.Find(ParentTemplate);
                    if (_promptTemplate != null)
                    {
                        base.SoAContext.Attach(_promptTemplate);
                    }
                }
                return _promptTemplate;
            }
            set
            {
                if (_promptTemplate != value)
                {
                    _promptTemplate = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_promptTemplate != null)
                    {
                        ParentTemplate = _promptTemplate.PromptTemplateId;
                    }
                }
            }
        }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("SourceProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(SourceProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. SourceProcedureVersion: " + SourceProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = base.SoAContext.ProcedureVersions.Find(SourceProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        base.SoAContext.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersion != null)
                    {
                        SourceProcedureVersion = _procedureVersion.ProcedureVersionId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("MaintainedByRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(MaintainedByRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. MaintainedByRole: " + MaintainedByRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(MaintainedByRole);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        MaintainedByRole = _role.RoleId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("UsedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(UsedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. UsedByAgent: " + UsedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(UsedByAgent);
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
                        UsedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private ObservableCollection<PromptTemplate> _promptTemplates;

        [InverseProperty("PromptTemplate")]
        public virtual ObservableCollection<PromptTemplate> PromptTemplates
        {
            get
            {
                if (_promptTemplates == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PromptTemplates - no database context is set. PromptTemplateId: " + this.PromptTemplateId + ".");
                        }
                        _promptTemplates = new ObservableCollection<PromptTemplate>();
                    }
                    else
                    {
                        var items = base.SoAContext.PromptTemplates.Where(x => x.ParentTemplate == this.PromptTemplateId).ToList<PromptTemplate>();
                        _promptTemplates = new ObservableCollection<PromptTemplate>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _promptTemplates.CollectionChanged += PromptTemplates_CollectionChanged;
                }
                return _promptTemplates;
            }
            private set
            {
                if (_promptTemplates != null)
                {
                    _promptTemplates.CollectionChanged -= PromptTemplates_CollectionChanged;
                }
                _promptTemplates = value;
                if (_promptTemplates != null)
                {
                    _promptTemplates.CollectionChanged += PromptTemplates_CollectionChanged;
                }
            }
        }

        private void PromptTemplates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<PromptTemplate>())
                {
                    item.ParentTemplate = this.PromptTemplateId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.PromptTemplate;
            _ = this.ProcedureVersion;
            _ = this.Role;
            _ = this.Agent;
            _ = this.PromptTemplates;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
