
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AppUsers")]
    public class AppUserBase : SoAEntityBase
    {
        [Key]
        public string AppUserId { get; set; }

        // Formula Name (rulebook: ={{DisplayName}})
        public string? Name
        {
            get => this.DisplayName; set { }
        }

        public string? EmailAddress { get; set; }
        public string? DisplayName { get; set; }
        public bool? IsEnabled { get; set; }
        // Formula AgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{LinkedAgent}}, Agents!{{AgentId}}, 0)))
        public string? AgentKind
        {
            get => INDEX(Agents!this.AgentKind, MATCH(this.LinkedAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula Organization (rulebook: =INDEX(Agents!{{Organization}}, MATCH({{LinkedAgent}}, Agents!{{AgentId}}, 0)))
        public string? Organization
        {
            get => INDEX(Agents!this.Organization, MATCH(this.LinkedAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula AssignmentCount (rulebook: =COUNTIFS(PrincipalAssignments!{{AppUser}}, {{AppUserId}}))
        public decimal? AssignmentCount
        {
            get => COUNTIFS(PrincipalAssignments!this.AppUser, this.AppUserId); set { }
        }

        // Formula HasNoPrincipal (rulebook: ={{AssignmentCount}} = 0)
        public bool? HasNoPrincipal
        {
            get => this.AssignmentCount = 0; set { }
        }

        // Formula HoldsMultiplePrincipals (rulebook: ={{AssignmentCount}} > 1)
        public bool? HoldsMultiplePrincipals
        {
            get => this.AssignmentCount > 1; set { }
        }

        // Formula IsNonHumanSignIn (rulebook: =OR({{AgentKind}} = "AIAgent", {{AgentKind}} = "AutomatedPipeline"))
        public bool? IsNonHumanSignIn
        {
            get => OR(this.AgentKind = "AIAgent", this.AgentKind = "AutomatedPipeline"); set { }
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. LinkedAgent: " + LinkedAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(LinkedAgent);
                    if (_agent != null)
                    {
                        Context.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    LinkedAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private ObservableCollection<PrincipalAssignment> _principalAssignments;

        [InverseProperty("AppUser")]
        public virtual ObservableCollection<PrincipalAssignment> PrincipalAssignments
        {
            get
            {
                if (_principalAssignments == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PrincipalAssignments - no database context is set. AppUserId: " + this.AppUserId + ".");
                        }
                        _principalAssignments = new ObservableCollection<PrincipalAssignment>();
                    }
                    else
                    {
                        var items = Context.PrincipalAssignments.Where(x => x.AppUser == this.AppUserId).ToList<PrincipalAssignment>();
                        _principalAssignments = new ObservableCollection<PrincipalAssignment>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("AppUser")]
        public virtual ObservableCollection<IssuedToken> IssuedTokens
        {
            get
            {
                if (_issuedTokens == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IssuedTokens - no database context is set. AppUserId: " + this.AppUserId + ".");
                        }
                        _issuedTokens = new ObservableCollection<IssuedToken>();
                    }
                    else
                    {
                        var items = Context.IssuedTokens.Where(x => x.AppUser == this.AppUserId).ToList<IssuedToken>();
                        _issuedTokens = new ObservableCollection<IssuedToken>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
