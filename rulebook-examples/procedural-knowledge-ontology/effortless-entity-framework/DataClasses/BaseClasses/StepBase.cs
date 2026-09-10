
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Steps")]
    public class StepBase : SoAEntityBase
    {
        [Key]
        public string StepId { get; set; }

        // Formula Name (rulebook: ={{StepNumber}} & ". " & {{Title}})
        public string? Name
        {
            get => this.StepNumber + ". " + this.Title; set { }
        }

        public string? StepNumber { get; set; }
        public string? Title { get; set; }
        public string? StepKind { get; set; }
        // Formula AssignedRoleLabel (rulebook: =INDEX(Roles!{{Label}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        public string? AssignedRoleLabel
        {
            get => INDEX(Roles!this.Label, MATCH(this.AssignedRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula AssignedAgentKind (rulebook: =INDEX(Roles!{{CurrentAgentKind}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        public string? AssignedAgentKind
        {
            get => INDEX(Roles!this.CurrentAgentKind, MATCH(this.AssignedRole, Roles!this.RoleId, 0)); set { }
        }

        public string? Instruction { get; set; }
        public int? ExpectedDurationMinutes { get; set; }
        public string? ExpertiseLevel { get; set; }
        public bool? RequiresHumanConfirmation { get; set; }
        // Formula BlockingRequirementCount (rulebook: =COUNTIFS(StepRequirements!{{BlockingStepKey}}, {{StepId}}))
        public decimal? BlockingRequirementCount
        {
            get => COUNTIFS(StepRequirements!this.BlockingStepKey, this.StepId); set { }
        }

        // Formula StaleBindingCount (rulebook: =COUNTIFS(OperationalBindings!{{StaleBindingStepKey}}, {{StepId}}))
        public decimal? StaleBindingCount
        {
            get => COUNTIFS(OperationalBindings!this.StaleBindingStepKey, this.StepId); set { }
        }

        // Formula AuthoritativeStaleCount (rulebook: =COUNTIFS(OperationalBindings!{{AuthoritativeStaleStepKey}}, {{StepId}}))
        public decimal? AuthoritativeStaleCount
        {
            get => COUNTIFS(OperationalBindings!this.AuthoritativeStaleStepKey, this.StepId); set { }
        }

        // Formula AvailableExceptionCount (rulebook: =COUNTIFS(Exceptions!{{ActiveExceptionStepKey}}, {{StepId}}))
        public decimal? AvailableExceptionCount
        {
            get => COUNTIFS(Exceptions!this.ActiveExceptionStepKey, this.StepId); set { }
        }

        // Formula DeclaredVerificationCount (rulebook: =COUNTIFS(StepVerifications!{{Step}}, {{StepId}}))
        public decimal? DeclaredVerificationCount
        {
            get => COUNTIFS(StepVerifications!this.Step, this.StepId); set { }
        }

        // Formula IsPreparationStep (rulebook: =OR({{AssignedRole}} = "finance-analyst", {{AssignedRole}} = "variance-review-agent"))
        public bool? IsPreparationStep
        {
            get => OR(this.AssignedRole = "finance-analyst", this.AssignedRole = "variance-review-agent"); set { }
        }

        // Formula IsApprovalStep (rulebook: =OR({{AssignedRole}} = "controller", {{AssignedRole}} = "cfo"))
        public bool? IsApprovalStep
        {
            get => OR(this.AssignedRole = "controller", this.AssignedRole = "cfo"); set { }
        }

        // Formula StaleAuthoritativeBindingCount (rulebook: =COUNTIFS(OperationalBindings!{{StepWhenStale}}, {{StepId}}))
        public decimal? StaleAuthoritativeBindingCount
        {
            get => COUNTIFS(OperationalBindings!this.StepWhenStale, this.StepId); set { }
        }

        // Formula InputsAreFresh (rulebook: ={{StaleAuthoritativeBindingCount}} = 0)
        public bool? InputsAreFresh
        {
            get => this.StaleAuthoritativeBindingCount = 0; set { }
        }

        // Formula IsSoftwareAssigned (rulebook: =OR({{AssignedAgentKind}} = "AIAgent", {{AssignedAgentKind}} = "AutomatedPipeline"))
        public bool? IsSoftwareAssigned
        {
            get => OR(this.AssignedAgentKind = "AIAgent", this.AssignedAgentKind = "AutomatedPipeline"); set { }
        }

        // Formula IsHumanApprovalGate (rulebook: =AND(NOT({{IsSoftwareAssigned}}), OR({{StepId}} = "policy-05", {{StepId}} = "close-06")))
        public bool? IsHumanApprovalGate
        {
            get => AND(NOT(this.IsSoftwareAssigned), OR(this.StepId = "policy-05", this.StepId = "close-06")); set { }
        }

        // Formula GateHeldByHuman (rulebook: =AND({{IsHumanApprovalGate}}, {{AssignedAgentKind}} = "Human"))
        public bool? GateHeldByHuman
        {
            get => AND(this.IsHumanApprovalGate, this.AssignedAgentKind = "Human"); set { }
        }

        // Formula BindingBoundaryCount (rulebook: =COUNTIFS(AuthorityBoundaries!{{StepWhenBinding}}, {{StepId}}))
        public decimal? BindingBoundaryCount
        {
            get => COUNTIFS(AuthorityBoundaries!this.StepWhenBinding, this.StepId); set { }
        }

        // Formula AssignedRoleIsUngoverned (rulebook: =INDEX(Roles!{{IsUngovernedNonHumanRole}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        public bool? AssignedRoleIsUngoverned
        {
            get => INDEX(Roles!this.IsUngovernedNonHumanRole, MATCH(this.AssignedRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula UnusableBindingCount (rulebook: =COUNTIFS(OperationalBindings!{{StepWhenUnusable}}, {{StepId}}))
        public decimal? UnusableBindingCount
        {
            get => COUNTIFS(OperationalBindings!this.StepWhenUnusable, this.StepId); set { }
        }

        // Formula AllSourcesUsable (rulebook: ={{UnusableBindingCount}} = 0)
        public bool? AllSourcesUsable
        {
            get => this.UnusableBindingCount = 0; set { }
        }

        public string? ControlKind { get; set; }
        // Formula UnwarrantedBoundaryCount (rulebook: =COUNTIFS(AuthorityBoundaries!{{UnwarrantedBoundaryStepKey}}, {{StepId}}))
        public decimal? UnwarrantedBoundaryCount
        {
            get => COUNTIFS(AuthorityBoundaries!this.UnwarrantedBoundaryStepKey, this.StepId); set { }
        }

        // Formula IsGovernedByUnwarrantedBoundary (rulebook: ={{UnwarrantedBoundaryCount}} > 0)
        public bool? IsGovernedByUnwarrantedBoundary
        {
            get => this.UnwarrantedBoundaryCount > 0; set { }
        }

        // Formula SoftwareExecutionCount (rulebook: =COUNTIFS(StepExecutions!{{SoftwareExecutionStepKey}}, {{StepId}}))
        public decimal? SoftwareExecutionCount
        {
            get => COUNTIFS(StepExecutions!this.SoftwareExecutionStepKey, this.StepId); set { }
        }

        // Formula HasBeenApproachedBySoftware (rulebook: ={{SoftwareExecutionCount}} > 0)
        public bool? HasBeenApproachedBySoftware
        {
            get => this.SoftwareExecutionCount > 0; set { }
        }

        // Formula IsUnexercisedHumanGate (rulebook: =AND({{IsHumanApprovalGate}}, NOT({{HasBeenApproachedBySoftware}})))
        public bool? IsUnexercisedHumanGate
        {
            get => AND(this.IsHumanApprovalGate, NOT(this.HasBeenApproachedBySoftware)); set { }
        }

        // Formula IsDemonstratedHumanGate (rulebook: =AND({{IsHumanApprovalGate}}, {{HasBeenApproachedBySoftware}}, {{GateHeldByHuman}}))
        public bool? IsDemonstratedHumanGate
        {
            get => AND(this.IsHumanApprovalGate, this.HasBeenApproachedBySoftware, this.GateHeldByHuman); set { }
        }

        // Formula UnexercisedGateVersionKey (rulebook: =IF({{IsUnexercisedHumanGate}}, {{ProcedureVersion}}, ""))
        public string? UnexercisedGateVersionKey
        {
            get => IF(this.IsUnexercisedHumanGate, this.ProcedureVersion, ""); set { }
        }

        // Formula HasDeclaredControlKind (rulebook: ={{ControlKind}} <> "")
        public bool? HasDeclaredControlKind
        {
            get => this.ControlKind <> ""; set { }
        }

        // Formula UndeclaredControlVersionKey (rulebook: =IF({{HasDeclaredControlKind}}, "", {{ProcedureVersion}}))
        public string? UndeclaredControlVersionKey
        {
            get => IF(this.HasDeclaredControlKind, "", this.ProcedureVersion); set { }
        }

        // Formula ApprovalStepIsSoftwareAssigned (rulebook: =AND({{ControlKind}} = "Approval", {{IsSoftwareAssigned}}))
        public bool? ApprovalStepIsSoftwareAssigned
        {
            get => AND(this.ControlKind = "Approval", this.IsSoftwareAssigned); set { }
        }

        // Formula UnwitnessedBlockingCount (rulebook: =COUNTIFS(StepRequirements!{{UnwitnessedStepKey}}, {{StepId}}))
        public decimal? UnwitnessedBlockingCount
        {
            get => COUNTIFS(StepRequirements!this.UnwitnessedStepKey, this.StepId); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? AssignedRole { get; set; }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = Context.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        Context.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    ProcedureVersion = _procedureVersion == null ? default : _procedureVersion.ProcedureVersionId;
                }
            }
        }

        private Role _role;

        [ForeignKey("AssignedRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(AssignedRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AssignedRole: " + AssignedRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(AssignedRole);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    AssignedRole = _role == null ? default : _role.RoleId;
                }
            }
        }

        private ObservableCollection<StepTransition> _stepTransitions;

        [InverseProperty("Step")]
        public virtual ObservableCollection<StepTransition> StepTransitions
        {
            get
            {
                if (_stepTransitions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepTransitions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepTransitions = new ObservableCollection<StepTransition>();
                    }
                    else
                    {
                        var items = Context.StepTransitions.Where(x => x.FromStep == this.StepId).ToList<StepTransition>();
                        _stepTransitions = new ObservableCollection<StepTransition>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _stepTransitions.CollectionChanged += StepTransitions_CollectionChanged;
                }
                return _stepTransitions;
            }
            private set
            {
                if (_stepTransitions != null)
                {
                    _stepTransitions.CollectionChanged -= StepTransitions_CollectionChanged;
                }
                _stepTransitions = value;
                if (_stepTransitions != null)
                {
                    _stepTransitions.CollectionChanged += StepTransitions_CollectionChanged;
                }
            }
        }

        private void StepTransitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepTransition>())
                {
                    item.FromStep = this.StepId;
                }
            }
        }

        private ObservableCollection<StepTransition> _stepTransitions;

        [InverseProperty("Step")]
        public virtual ObservableCollection<StepTransition> StepTransitions
        {
            get
            {
                if (_stepTransitions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepTransitions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepTransitions = new ObservableCollection<StepTransition>();
                    }
                    else
                    {
                        var items = Context.StepTransitions.Where(x => x.ToStep == this.StepId).ToList<StepTransition>();
                        _stepTransitions = new ObservableCollection<StepTransition>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _stepTransitions.CollectionChanged += StepTransitions_CollectionChanged;
                }
                return _stepTransitions;
            }
            private set
            {
                if (_stepTransitions != null)
                {
                    _stepTransitions.CollectionChanged -= StepTransitions_CollectionChanged;
                }
                _stepTransitions = value;
                if (_stepTransitions != null)
                {
                    _stepTransitions.CollectionChanged += StepTransitions_CollectionChanged;
                }
            }
        }

        private void StepTransitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepTransition>())
                {
                    item.ToStep = this.StepId;
                }
            }
        }

        private ObservableCollection<StepAction> _stepActions;

        [InverseProperty("Step")]
        public virtual ObservableCollection<StepAction> StepActions
        {
            get
            {
                if (_stepActions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepActions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepActions = new ObservableCollection<StepAction>();
                    }
                    else
                    {
                        var items = Context.StepActions.Where(x => x.Step == this.StepId).ToList<StepAction>();
                        _stepActions = new ObservableCollection<StepAction>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _stepActions.CollectionChanged += StepActions_CollectionChanged;
                }
                return _stepActions;
            }
            private set
            {
                if (_stepActions != null)
                {
                    _stepActions.CollectionChanged -= StepActions_CollectionChanged;
                }
                _stepActions = value;
                if (_stepActions != null)
                {
                    _stepActions.CollectionChanged += StepActions_CollectionChanged;
                }
            }
        }

        private void StepActions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepAction>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepFunction> _stepFunctions;

        [InverseProperty("Step")]
        public virtual ObservableCollection<StepFunction> StepFunctions
        {
            get
            {
                if (_stepFunctions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepFunctions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepFunctions = new ObservableCollection<StepFunction>();
                    }
                    else
                    {
                        var items = Context.StepFunctions.Where(x => x.Step == this.StepId).ToList<StepFunction>();
                        _stepFunctions = new ObservableCollection<StepFunction>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _stepFunctions.CollectionChanged += StepFunctions_CollectionChanged;
                }
                return _stepFunctions;
            }
            private set
            {
                if (_stepFunctions != null)
                {
                    _stepFunctions.CollectionChanged -= StepFunctions_CollectionChanged;
                }
                _stepFunctions = value;
                if (_stepFunctions != null)
                {
                    _stepFunctions.CollectionChanged += StepFunctions_CollectionChanged;
                }
            }
        }

        private void StepFunctions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepFunction>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepTool> _stepTools;

        [InverseProperty("Step")]
        public virtual ObservableCollection<StepTool> StepTools
        {
            get
            {
                if (_stepTools == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepTools - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepTools = new ObservableCollection<StepTool>();
                    }
                    else
                    {
                        var items = Context.StepTools.Where(x => x.Step == this.StepId).ToList<StepTool>();
                        _stepTools = new ObservableCollection<StepTool>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _stepTools.CollectionChanged += StepTools_CollectionChanged;
                }
                return _stepTools;
            }
            private set
            {
                if (_stepTools != null)
                {
                    _stepTools.CollectionChanged -= StepTools_CollectionChanged;
                }
                _stepTools = value;
                if (_stepTools != null)
                {
                    _stepTools.CollectionChanged += StepTools_CollectionChanged;
                }
            }
        }

        private void StepTools_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepTool>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepRequirement> _stepRequirements;

        [InverseProperty("Step")]
        public virtual ObservableCollection<StepRequirement> StepRequirements
        {
            get
            {
                if (_stepRequirements == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRequirements - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepRequirements = new ObservableCollection<StepRequirement>();
                    }
                    else
                    {
                        var items = Context.StepRequirements.Where(x => x.Step == this.StepId).ToList<StepRequirement>();
                        _stepRequirements = new ObservableCollection<StepRequirement>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _stepRequirements.CollectionChanged += StepRequirements_CollectionChanged;
                }
                return _stepRequirements;
            }
            private set
            {
                if (_stepRequirements != null)
                {
                    _stepRequirements.CollectionChanged -= StepRequirements_CollectionChanged;
                }
                _stepRequirements = value;
                if (_stepRequirements != null)
                {
                    _stepRequirements.CollectionChanged += StepRequirements_CollectionChanged;
                }
            }
        }

        private void StepRequirements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepRequirement>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepVerification> _stepVerifications;

        [InverseProperty("Step")]
        public virtual ObservableCollection<StepVerification> StepVerifications
        {
            get
            {
                if (_stepVerifications == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepVerifications - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepVerifications = new ObservableCollection<StepVerification>();
                    }
                    else
                    {
                        var items = Context.StepVerifications.Where(x => x.Step == this.StepId).ToList<StepVerification>();
                        _stepVerifications = new ObservableCollection<StepVerification>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _stepVerifications.CollectionChanged += StepVerifications_CollectionChanged;
                }
                return _stepVerifications;
            }
            private set
            {
                if (_stepVerifications != null)
                {
                    _stepVerifications.CollectionChanged -= StepVerifications_CollectionChanged;
                }
                _stepVerifications = value;
                if (_stepVerifications != null)
                {
                    _stepVerifications.CollectionChanged += StepVerifications_CollectionChanged;
                }
            }
        }

        private void StepVerifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepVerification>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<Rationale> _rationales;

        [InverseProperty("Step")]
        public virtual ObservableCollection<Rationale> Rationales
        {
            get
            {
                if (_rationales == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Rationales - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _rationales = new ObservableCollection<Rationale>();
                    }
                    else
                    {
                        var items = Context.Rationales.Where(x => x.Step == this.StepId).ToList<Rationale>();
                        _rationales = new ObservableCollection<Rationale>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _rationales.CollectionChanged += Rationales_CollectionChanged;
                }
                return _rationales;
            }
            private set
            {
                if (_rationales != null)
                {
                    _rationales.CollectionChanged -= Rationales_CollectionChanged;
                }
                _rationales = value;
                if (_rationales != null)
                {
                    _rationales.CollectionChanged += Rationales_CollectionChanged;
                }
            }
        }

        private void Rationales_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Rationale>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<Exception> _exceptions;

        [InverseProperty("Step")]
        public virtual ObservableCollection<Exception> Exceptions
        {
            get
            {
                if (_exceptions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Exceptions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _exceptions = new ObservableCollection<Exception>();
                    }
                    else
                    {
                        var items = Context.Exceptions.Where(x => x.TriggerStep == this.StepId).ToList<Exception>();
                        _exceptions = new ObservableCollection<Exception>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _exceptions.CollectionChanged += Exceptions_CollectionChanged;
                }
                return _exceptions;
            }
            private set
            {
                if (_exceptions != null)
                {
                    _exceptions.CollectionChanged -= Exceptions_CollectionChanged;
                }
                _exceptions = value;
                if (_exceptions != null)
                {
                    _exceptions.CollectionChanged += Exceptions_CollectionChanged;
                }
            }
        }

        private void Exceptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Exception>())
                {
                    item.TriggerStep = this.StepId;
                }
            }
        }

        private ObservableCollection<KnowledgeFragment> _knowledgeFragments;

        [InverseProperty("Step")]
        public virtual ObservableCollection<KnowledgeFragment> KnowledgeFragments
        {
            get
            {
                if (_knowledgeFragments == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = Context.KnowledgeFragments.Where(x => x.Step == this.StepId).ToList<KnowledgeFragment>();
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
                return _knowledgeFragments;
            }
            private set
            {
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged -= KnowledgeFragments_CollectionChanged;
                }
                _knowledgeFragments = value;
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
            }
        }

        private void KnowledgeFragments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeFragment>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<KnowledgeGap> _knowledgeGaps;

        [InverseProperty("Step")]
        public virtual ObservableCollection<KnowledgeGap> KnowledgeGaps
        {
            get
            {
                if (_knowledgeGaps == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeGaps - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>();
                    }
                    else
                    {
                        var items = Context.KnowledgeGaps.Where(x => x.Step == this.StepId).ToList<KnowledgeGap>();
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _knowledgeGaps.CollectionChanged += KnowledgeGaps_CollectionChanged;
                }
                return _knowledgeGaps;
            }
            private set
            {
                if (_knowledgeGaps != null)
                {
                    _knowledgeGaps.CollectionChanged -= KnowledgeGaps_CollectionChanged;
                }
                _knowledgeGaps = value;
                if (_knowledgeGaps != null)
                {
                    _knowledgeGaps.CollectionChanged += KnowledgeGaps_CollectionChanged;
                }
            }
        }

        private void KnowledgeGaps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeGap>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<FAQ> _fAQs;

        [InverseProperty("Step")]
        public virtual ObservableCollection<FAQ> FAQs
        {
            get
            {
                if (_fAQs == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FAQs - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _fAQs = new ObservableCollection<FAQ>();
                    }
                    else
                    {
                        var items = Context.FAQs.Where(x => x.Step == this.StepId).ToList<FAQ>();
                        _fAQs = new ObservableCollection<FAQ>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _fAQs.CollectionChanged += FAQs_CollectionChanged;
                }
                return _fAQs;
            }
            private set
            {
                if (_fAQs != null)
                {
                    _fAQs.CollectionChanged -= FAQs_CollectionChanged;
                }
                _fAQs = value;
                if (_fAQs != null)
                {
                    _fAQs.CollectionChanged += FAQs_CollectionChanged;
                }
            }
        }

        private void FAQs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FAQ>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<Explanation> _explanations;

        [InverseProperty("Step")]
        public virtual ObservableCollection<Explanation> Explanations
        {
            get
            {
                if (_explanations == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Explanations - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _explanations = new ObservableCollection<Explanation>();
                    }
                    else
                    {
                        var items = Context.Explanations.Where(x => x.Step == this.StepId).ToList<Explanation>();
                        _explanations = new ObservableCollection<Explanation>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _explanations.CollectionChanged += Explanations_CollectionChanged;
                }
                return _explanations;
            }
            private set
            {
                if (_explanations != null)
                {
                    _explanations.CollectionChanged -= Explanations_CollectionChanged;
                }
                _explanations = value;
                if (_explanations != null)
                {
                    _explanations.CollectionChanged += Explanations_CollectionChanged;
                }
            }
        }

        private void Explanations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Explanation>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepExecution> _stepExecutions;

        [InverseProperty("Step")]
        public virtual ObservableCollection<StepExecution> StepExecutions
        {
            get
            {
                if (_stepExecutions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecutions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepExecutions = new ObservableCollection<StepExecution>();
                    }
                    else
                    {
                        var items = Context.StepExecutions.Where(x => x.Step == this.StepId).ToList<StepExecution>();
                        _stepExecutions = new ObservableCollection<StepExecution>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _stepExecutions.CollectionChanged += StepExecutions_CollectionChanged;
                }
                return _stepExecutions;
            }
            private set
            {
                if (_stepExecutions != null)
                {
                    _stepExecutions.CollectionChanged -= StepExecutions_CollectionChanged;
                }
                _stepExecutions = value;
                if (_stepExecutions != null)
                {
                    _stepExecutions.CollectionChanged += StepExecutions_CollectionChanged;
                }
            }
        }

        private void StepExecutions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepExecution>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<OperationalBinding> _operationalBindings;

        [InverseProperty("Step")]
        public virtual ObservableCollection<OperationalBinding> OperationalBindings
        {
            get
            {
                if (_operationalBindings == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OperationalBindings - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _operationalBindings = new ObservableCollection<OperationalBinding>();
                    }
                    else
                    {
                        var items = Context.OperationalBindings.Where(x => x.Step == this.StepId).ToList<OperationalBinding>();
                        _operationalBindings = new ObservableCollection<OperationalBinding>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _operationalBindings.CollectionChanged += OperationalBindings_CollectionChanged;
                }
                return _operationalBindings;
            }
            private set
            {
                if (_operationalBindings != null)
                {
                    _operationalBindings.CollectionChanged -= OperationalBindings_CollectionChanged;
                }
                _operationalBindings = value;
                if (_operationalBindings != null)
                {
                    _operationalBindings.CollectionChanged += OperationalBindings_CollectionChanged;
                }
            }
        }

        private void OperationalBindings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<OperationalBinding>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<AuthorityBoundary> _authorityBoundaries;

        [InverseProperty("Step")]
        public virtual ObservableCollection<AuthorityBoundary> AuthorityBoundaries
        {
            get
            {
                if (_authorityBoundaries == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorityBoundaries - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>();
                    }
                    else
                    {
                        var items = Context.AuthorityBoundaries.Where(x => x.Step == this.StepId).ToList<AuthorityBoundary>();
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _authorityBoundaries.CollectionChanged += AuthorityBoundaries_CollectionChanged;
                }
                return _authorityBoundaries;
            }
            private set
            {
                if (_authorityBoundaries != null)
                {
                    _authorityBoundaries.CollectionChanged -= AuthorityBoundaries_CollectionChanged;
                }
                _authorityBoundaries = value;
                if (_authorityBoundaries != null)
                {
                    _authorityBoundaries.CollectionChanged += AuthorityBoundaries_CollectionChanged;
                }
            }
        }

        private void AuthorityBoundaries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AuthorityBoundary>())
                {
                    item.Step = this.StepId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.Role;
            _ = this.StepTransitions;
            _ = this.StepTransitions;
            _ = this.StepActions;
            _ = this.StepFunctions;
            _ = this.StepTools;
            _ = this.StepRequirements;
            _ = this.StepVerifications;
            _ = this.Rationales;
            _ = this.Exceptions;
            _ = this.KnowledgeFragments;
            _ = this.KnowledgeGaps;
            _ = this.FAQs;
            _ = this.Explanations;
            _ = this.StepExecutions;
            _ = this.OperationalBindings;
            _ = this.AuthorityBoundaries;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
