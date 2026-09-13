
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
    [Table("AppUsers")]
    public class AppUserBase : SoAEntityBase
    {
        [Key]
        public string AppUserId { get; set; }

        // Formula Name (rulebook: ={{DisplayName}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.DisplayName))); set { }
        }

        public string? EmailAddress { get; set; }
        public string? DisplayName { get; set; }
        public bool? IsEnabled { get; set; }
        // Formula AgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{LinkedAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? AgentKind
        {
            get => F.AsString(F.Memo(this, "AgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.LinkedAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula Organization (rulebook: =INDEX(Agents!{{Organization}}, MATCH({{LinkedAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? Organization
        {
            get => F.AsString(F.Memo(this, "Organization", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.LinkedAgent), __r => F.Of(__r.Organization), () => F.Of(new Agent().Organization)))); set { }
        }

        // Formula AssignmentCount (rulebook: =COUNTIFS(PrincipalAssignments!{{AppUser}}, {{AppUserId}}))
        [NotMapped]
        public decimal? AssignmentCount
        {
            get => F.AsDecimal(F.Memo(this, "AssignmentCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<PrincipalAssignment>(base.SoAContext, "PrincipalAssignments", __c => __c.PrincipalAssignments), __r => F.CritField(F.Of(__r.AppUser), F.Of(this.AppUserId)))))); set { }
        }

        // Formula HasNoPrincipal (rulebook: ={{AssignmentCount}} = 0)
        [NotMapped]
        public bool? HasNoPrincipal
        {
            get => F.AsBool(F.Memo(this, "HasNoPrincipal", () => F.Eq(F.Of(this.AssignmentCount), F.I(0)))); set { }
        }

        // Formula HoldsMultiplePrincipals (rulebook: ={{AssignmentCount}} > 1)
        [NotMapped]
        public bool? HoldsMultiplePrincipals
        {
            get => F.AsBool(F.Memo(this, "HoldsMultiplePrincipals", () => F.Cmp(F.Of(this.AssignmentCount), ">", F.I(1)))); set { }
        }

        // Formula IsNonHumanSignIn (rulebook: =OR({{AgentKind}} = "AIAgent", {{AgentKind}} = "AutomatedPipeline"))
        [NotMapped]
        public bool? IsNonHumanSignIn
        {
            get => F.AsBool(F.Memo(this, "IsNonHumanSignIn", () => F.Or(F.Bool3(F.Eq(F.Of(this.AgentKind), F.S("AIAgent"))), F.Bool3(F.Eq(F.Of(this.AgentKind), F.S("AutomatedPipeline")))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? LinkedAgent { get; set; }

        private Agent _agent;

        [ForeignKey("LinkedAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(LinkedAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. LinkedAgent: " + LinkedAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(LinkedAgent);
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
                        LinkedAgent = _agent.AgentId;
                    }
                }
            }
        }

        private ObservableCollection<PrincipalAssignment> _principalAssignments;

        [InverseProperty("AppUserRef")]
        public virtual ObservableCollection<PrincipalAssignment> PrincipalAssignments
        {
            get
            {
                if (_principalAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PrincipalAssignments - no database context is set. AppUserId: " + this.AppUserId + ".");
                        }
                        _principalAssignments = new ObservableCollection<PrincipalAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.PrincipalAssignments.Where(x => x.AppUser == this.AppUserId).ToList<PrincipalAssignment>();
                        _principalAssignments = new ObservableCollection<PrincipalAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _principalAssignments.CollectionChanged += PrincipalAssignments_CollectionChanged;
                }
                return _principalAssignments;
            }
            private set
            {
                if (_principalAssignments != null)
                {
                    _principalAssignments.CollectionChanged -= PrincipalAssignments_CollectionChanged;
                }
                _principalAssignments = value;
                if (_principalAssignments != null)
                {
                    _principalAssignments.CollectionChanged += PrincipalAssignments_CollectionChanged;
                }
            }
        }

        private void PrincipalAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<PrincipalAssignment>())
                {
                    item.AppUser = this.AppUserId;
                }
            }
        }

        private ObservableCollection<IssuedToken> _issuedTokens;

        [InverseProperty("AppUserRef")]
        public virtual ObservableCollection<IssuedToken> IssuedTokens
        {
            get
            {
                if (_issuedTokens == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IssuedTokens - no database context is set. AppUserId: " + this.AppUserId + ".");
                        }
                        _issuedTokens = new ObservableCollection<IssuedToken>();
                    }
                    else
                    {
                        var items = base.SoAContext.IssuedTokens.Where(x => x.AppUser == this.AppUserId).ToList<IssuedToken>();
                        _issuedTokens = new ObservableCollection<IssuedToken>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _issuedTokens.CollectionChanged += IssuedTokens_CollectionChanged;
                }
                return _issuedTokens;
            }
            private set
            {
                if (_issuedTokens != null)
                {
                    _issuedTokens.CollectionChanged -= IssuedTokens_CollectionChanged;
                }
                _issuedTokens = value;
                if (_issuedTokens != null)
                {
                    _issuedTokens.CollectionChanged += IssuedTokens_CollectionChanged;
                }
            }
        }

        private void IssuedTokens_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<IssuedToken>())
                {
                    item.AppUser = this.AppUserId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Agent;
            _ = this.PrincipalAssignments;
            _ = this.IssuedTokens;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
