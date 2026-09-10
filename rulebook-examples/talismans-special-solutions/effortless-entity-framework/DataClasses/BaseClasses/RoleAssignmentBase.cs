
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
    [Table("RoleAssignments")]
    public class RoleAssignmentBase : SoAEntityBase
    {
        [Key]
        public string RoleAssignmentId { get; set; }

        // Formula ParentPath (rulebook: =INDEX(Roles!{{RelativePath}}, MATCH({{Role}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? ParentPath
        {
            get => F.AsString(F.Memo(this, "ParentPath", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.Role), __r => F.Of(__r.RelativePath), () => F.Of(new Role().RelativePath)))); set { }
        }

        // Formula RelativePath (rulebook: ={{ParentPath}} & "/assignments/" & {{RoleAssignmentId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.TextOr(F.Of(this.ParentPath)), F.S("/assignments/"), F.TextOr(F.Of(this.RoleAssignmentId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        // Formula Name (rulebook: ={{Role}} & " [" & {{ValidFrom}} & " -> " & IF(ISBLANK({{ValidTo}}), "open", {{ValidTo}}) & "]")
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.Role)), F.S(" ["), F.TextOr(F.Of(this.ValidFrom)), F.S(" -> "), F.TextNotNull((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.ValidTo)))) ? F.S("open") : F.Of(this.ValidTo))), F.S("]")))); set { }
        }

        public DateOnly ValidFrom { get; set; }
        public DateOnly? ValidTo { get; set; }
        public string? Reason { get; set; }
        public string? PriorFillerType { get; set; }
        // Formula FillerType (rulebook: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", ""))))
        [NotMapped]
        public string? FillerType
        {
            get => F.AsString(F.Memo(this, "FillerType", () => (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.FilledByHumanAgent)))))) ? F.S("HumanAgent") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.FilledByAIAgent)))))) ? F.S("AIAgent") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.FilledByAutomatedPipeline)))))) ? F.S("AutomatedPipeline") : F.S("")))))); set { }
        }

        // Formula IsCurrent (rulebook: =ISBLANK({{ValidTo}}))
        [NotMapped]
        public bool? IsCurrent
        {
            get => F.AsBool(F.Memo(this, "IsCurrent", () => F.IsBlank(F.Of(this.ValidTo)))); set { }
        }

        // Formula WasActiveAsOfAuditDate (rulebook: =AND({{ValidFrom}} <= "2026-03-01", OR(ISBLANK({{ValidTo}}), {{ValidTo}} > "2026-03-01")))
        [NotMapped]
        public bool? WasActiveAsOfAuditDate
        {
            get => F.AsBool(F.Memo(this, "WasActiveAsOfAuditDate", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidFrom)), "<=", F.S("2026-03-01"))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ValidTo))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidTo)), ">", F.S("2026-03-01")))))))); set { }
        }

        // Formula IsAgentTypeChange (rulebook: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} <> {{FillerType}}))
        [NotMapped]
        public bool? IsAgentTypeChange
        {
            get => F.AsBool(F.Memo(this, "IsAgentTypeChange", () => F.And(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.PriorFillerType))))), F.Bool3(F.Ne(F.Nullif(F.Of(this.PriorFillerType)), F.Of(this.FillerType)))))); set { }
        }

        // Formula RequiresComplianceAudit (rulebook: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} = "AIAgent", {{FillerType}} = "HumanAgent"))
        [NotMapped]
        public bool? RequiresComplianceAudit
        {
            get => F.AsBool(F.Memo(this, "RequiresComplianceAudit", () => F.And(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.PriorFillerType))))), F.Bool3(F.Eq(F.Nullif(F.Of(this.PriorFillerType)), F.S("AIAgent"))), F.Bool3(F.Eq(F.Of(this.FillerType), F.S("HumanAgent")))))); set { }
        }


        public string Role { get; set; }
        public string? FilledByHumanAgent { get; set; }
        public string? FilledByAIAgent { get; set; }
        public string? FilledByAutomatedPipeline { get; set; }

        private Role _roleRef;

        [ForeignKey("Role")]
        public virtual Role RoleRef
        {
            get
            {
                if (_roleRef == null && !string.IsNullOrEmpty(Role))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleRef - no database context is set. Role: " + Role + ".");
                        }
                        return null;
                    }
                    _roleRef = base.SoAContext.Roles.Find(Role);
                    if (_roleRef != null)
                    {
                        base.SoAContext.Attach(_roleRef);
                    }
                }
                return _roleRef;
            }
            set
            {
                if (_roleRef != value)
                {
                    _roleRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleRef != null)
                    {
                        Role = _roleRef.RoleId;
                    }
                }
            }
        }

        private HumanAgent _humanAgent;

        [ForeignKey("FilledByHumanAgent")]
        public virtual HumanAgent HumanAgent
        {
            get
            {
                if (_humanAgent == null && !string.IsNullOrEmpty(FilledByHumanAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access HumanAgent - no database context is set. FilledByHumanAgent: " + FilledByHumanAgent + ".");
                        }
                        return null;
                    }
                    _humanAgent = base.SoAContext.HumanAgents.Find(FilledByHumanAgent);
                    if (_humanAgent != null)
                    {
                        base.SoAContext.Attach(_humanAgent);
                    }
                }
                return _humanAgent;
            }
            set
            {
                if (_humanAgent != value)
                {
                    _humanAgent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_humanAgent != null)
                    {
                        FilledByHumanAgent = _humanAgent.HumanAgentId;
                    }
                }
            }
        }

        private AIAgent _aIAgent;

        [ForeignKey("FilledByAIAgent")]
        public virtual AIAgent AIAgent
        {
            get
            {
                if (_aIAgent == null && !string.IsNullOrEmpty(FilledByAIAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AIAgent - no database context is set. FilledByAIAgent: " + FilledByAIAgent + ".");
                        }
                        return null;
                    }
                    _aIAgent = base.SoAContext.AIAgents.Find(FilledByAIAgent);
                    if (_aIAgent != null)
                    {
                        base.SoAContext.Attach(_aIAgent);
                    }
                }
                return _aIAgent;
            }
            set
            {
                if (_aIAgent != value)
                {
                    _aIAgent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_aIAgent != null)
                    {
                        FilledByAIAgent = _aIAgent.AIAgentId;
                    }
                }
            }
        }

        private AutomatedPipeline _automatedPipeline;

        [ForeignKey("FilledByAutomatedPipeline")]
        public virtual AutomatedPipeline AutomatedPipeline
        {
            get
            {
                if (_automatedPipeline == null && !string.IsNullOrEmpty(FilledByAutomatedPipeline))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AutomatedPipeline - no database context is set. FilledByAutomatedPipeline: " + FilledByAutomatedPipeline + ".");
                        }
                        return null;
                    }
                    _automatedPipeline = base.SoAContext.AutomatedPipelines.Find(FilledByAutomatedPipeline);
                    if (_automatedPipeline != null)
                    {
                        base.SoAContext.Attach(_automatedPipeline);
                    }
                }
                return _automatedPipeline;
            }
            set
            {
                if (_automatedPipeline != value)
                {
                    _automatedPipeline = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_automatedPipeline != null)
                    {
                        FilledByAutomatedPipeline = _automatedPipeline.AutomatedPipelineId;
                    }
                }
            }
        }

        private ObservableCollection<Role> _roles;

        [InverseProperty("RoleAssignment")]
        public virtual ObservableCollection<Role> Roles
        {
            get
            {
                if (_roles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Roles - no database context is set. RoleAssignmentId: " + this.RoleAssignmentId + ".");
                        }
                        _roles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.RoleAssignments == this.RoleAssignmentId).ToList<Role>();
                        _roles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
                return _roles;
            }
            private set
            {
                if (_roles != null)
                {
                    _roles.CollectionChanged -= Roles_CollectionChanged;
                }
                _roles = value;
                if (_roles != null)
                {
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
            }
        }

        private void Roles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.RoleAssignments = this.RoleAssignmentId;
                }
            }
        }

        private ObservableCollection<HumanAgent> _humanAgents;

        [InverseProperty("RoleAssignment")]
        public virtual ObservableCollection<HumanAgent> HumanAgents
        {
            get
            {
                if (_humanAgents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access HumanAgents - no database context is set. RoleAssignmentId: " + this.RoleAssignmentId + ".");
                        }
                        _humanAgents = new ObservableCollection<HumanAgent>();
                    }
                    else
                    {
                        var items = base.SoAContext.HumanAgents.Where(x => x.RoleAssignments == this.RoleAssignmentId).ToList<HumanAgent>();
                        _humanAgents = new ObservableCollection<HumanAgent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _humanAgents.CollectionChanged += HumanAgents_CollectionChanged;
                }
                return _humanAgents;
            }
            private set
            {
                if (_humanAgents != null)
                {
                    _humanAgents.CollectionChanged -= HumanAgents_CollectionChanged;
                }
                _humanAgents = value;
                if (_humanAgents != null)
                {
                    _humanAgents.CollectionChanged += HumanAgents_CollectionChanged;
                }
            }
        }

        private void HumanAgents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<HumanAgent>())
                {
                    item.RoleAssignments = this.RoleAssignmentId;
                }
            }
        }

        private ObservableCollection<AIAgent> _aIAgents;

        [InverseProperty("RoleAssignment")]
        public virtual ObservableCollection<AIAgent> AIAgents
        {
            get
            {
                if (_aIAgents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AIAgents - no database context is set. RoleAssignmentId: " + this.RoleAssignmentId + ".");
                        }
                        _aIAgents = new ObservableCollection<AIAgent>();
                    }
                    else
                    {
                        var items = base.SoAContext.AIAgents.Where(x => x.RoleAssignments == this.RoleAssignmentId).ToList<AIAgent>();
                        _aIAgents = new ObservableCollection<AIAgent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aIAgents.CollectionChanged += AIAgents_CollectionChanged;
                }
                return _aIAgents;
            }
            private set
            {
                if (_aIAgents != null)
                {
                    _aIAgents.CollectionChanged -= AIAgents_CollectionChanged;
                }
                _aIAgents = value;
                if (_aIAgents != null)
                {
                    _aIAgents.CollectionChanged += AIAgents_CollectionChanged;
                }
            }
        }

        private void AIAgents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AIAgent>())
                {
                    item.RoleAssignments = this.RoleAssignmentId;
                }
            }
        }

        private ObservableCollection<AutomatedPipeline> _automatedPipelines;

        [InverseProperty("RoleAssignment")]
        public virtual ObservableCollection<AutomatedPipeline> AutomatedPipelines
        {
            get
            {
                if (_automatedPipelines == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AutomatedPipelines - no database context is set. RoleAssignmentId: " + this.RoleAssignmentId + ".");
                        }
                        _automatedPipelines = new ObservableCollection<AutomatedPipeline>();
                    }
                    else
                    {
                        var items = base.SoAContext.AutomatedPipelines.Where(x => x.RoleAssignments == this.RoleAssignmentId).ToList<AutomatedPipeline>();
                        _automatedPipelines = new ObservableCollection<AutomatedPipeline>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _automatedPipelines.CollectionChanged += AutomatedPipelines_CollectionChanged;
                }
                return _automatedPipelines;
            }
            private set
            {
                if (_automatedPipelines != null)
                {
                    _automatedPipelines.CollectionChanged -= AutomatedPipelines_CollectionChanged;
                }
                _automatedPipelines = value;
                if (_automatedPipelines != null)
                {
                    _automatedPipelines.CollectionChanged += AutomatedPipelines_CollectionChanged;
                }
            }
        }

        private void AutomatedPipelines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AutomatedPipeline>())
                {
                    item.RoleAssignments = this.RoleAssignmentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RoleRef;
            _ = this.HumanAgent;
            _ = this.AIAgent;
            _ = this.AutomatedPipeline;
            _ = this.Roles;
            _ = this.HumanAgents;
            _ = this.AIAgents;
            _ = this.AutomatedPipelines;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
