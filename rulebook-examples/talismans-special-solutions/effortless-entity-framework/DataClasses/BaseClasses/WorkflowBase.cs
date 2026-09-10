
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
    [Table("Workflows")]
    public class WorkflowBase : SoAEntityBase
    {
        [Key]
        public string WorkflowId { get; set; }

        // Formula RelativePath (rulebook: ="workflows/" & {{WorkflowId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("workflows/"), F.TextOr(F.Of(this.WorkflowId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        // Formula Name (rulebook: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-"))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Substitute(F.Lower(F.Of(this.DisplayName)), F.S(" "), F.S("-")))); set { }
        }

        public string? DisplayName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Identifier { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public DateTimeOffset? Created { get; set; }
        public int? StalenessThresholdMonths { get; set; }
        // Formula CountOfNonProposedSteps (rulebook: =COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}}))
        [NotMapped]
        public int? CountOfNonProposedSteps
        {
            get => F.AsInt(F.Memo(this, "CountOfNonProposedSteps", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowStep>(base.SoAContext, "WorkflowSteps", __c => __c.WorkflowSteps), __r => F.CritField(F.Of(__r.Workflow), F.Of(this.WorkflowId))))))); set { }
        }

        // Formula HasMoreThan1Step (rulebook: ={{CountOfNonProposedSteps}} > 1)
        [NotMapped]
        public bool? HasMoreThan1Step
        {
            get => F.AsBool(F.Memo(this, "HasMoreThan1Step", () => F.Cmp(F.Of(this.CountOfNonProposedSteps), ">", F.I(1)))); set { }
        }

        // Formula CountAISteps (rulebook: =COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}}, WorkflowSteps!{{IsExecutedByAI}}, TRUE))
        [NotMapped]
        public int? CountAISteps
        {
            get => F.AsInt(F.Memo(this, "CountAISteps", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowStep>(base.SoAContext, "WorkflowSteps", __c => __c.WorkflowSteps), __r => F.CritField(F.Of(__r.Workflow), F.Of(this.WorkflowId)) && F.CritLiteral(F.Of(__r.IsExecutedByAI), F.B(true))))))); set { }
        }

        // Formula CountHumanSteps (rulebook: =COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}}, WorkflowSteps!{{IsExecutedByHuman}}, TRUE))
        [NotMapped]
        public int? CountHumanSteps
        {
            get => F.AsInt(F.Memo(this, "CountHumanSteps", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowStep>(base.SoAContext, "WorkflowSteps", __c => __c.WorkflowSteps), __r => F.CritField(F.Of(__r.Workflow), F.Of(this.WorkflowId)) && F.CritLiteral(F.Of(__r.IsExecutedByHuman), F.B(true))))))); set { }
        }

        // Formula CountHumanRequiredSteps (rulebook: =COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}}, WorkflowSteps!{{RequiresHumanApproval}}, TRUE))
        [NotMapped]
        public int? CountHumanRequiredSteps
        {
            get => F.AsInt(F.Memo(this, "CountHumanRequiredSteps", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowStep>(base.SoAContext, "WorkflowSteps", __c => __c.WorkflowSteps), __r => F.CritField(F.Of(__r.Workflow), F.Of(this.WorkflowId)) && F.CritLiteral(F.Of(__r.RequiresHumanApproval), F.B(true))))))); set { }
        }

        // Formula CountApprovalConsistencyViolations (rulebook: =COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}}, WorkflowSteps!{{ApprovalConsistencyViolation}}, TRUE))
        [NotMapped]
        public int? CountApprovalConsistencyViolations
        {
            get => F.AsInt(F.Memo(this, "CountApprovalConsistencyViolations", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowStep>(base.SoAContext, "WorkflowSteps", __c => __c.WorkflowSteps), __r => F.CritField(F.Of(__r.Workflow), F.Of(this.WorkflowId)) && F.CritLiteral(F.Of(__r.ApprovalConsistencyViolation), F.B(true))))))); set { }
        }

        // Formula HasConsistencyViolation (rulebook: ={{CountApprovalConsistencyViolations}} > 0)
        [NotMapped]
        public bool? HasConsistencyViolation
        {
            get => F.AsBool(F.Memo(this, "HasConsistencyViolation", () => F.Cmp(F.Of(this.CountApprovalConsistencyViolations), ">", F.I(0)))); set { }
        }

        // Formula HasAIAgentStep (rulebook: ={{CountAISteps}} > 0)
        [NotMapped]
        public bool? HasAIAgentStep
        {
            get => F.AsBool(F.Memo(this, "HasAIAgentStep", () => F.Cmp(F.Of(this.CountAISteps), ">", F.I(0)))); set { }
        }

        // Formula MonthsSinceModified (rulebook: =DATETIME_DIFF(NOW(), {{Modified}}, "months"))
        [NotMapped]
        public int? MonthsSinceModified
        {
            get => F.AsInt(F.Memo(this, "MonthsSinceModified", () => F.Integer(F.DatetimeDiff(F.Now(), F.Of(this.Modified), F.S("months"))))); set { }
        }

        // Formula IsStale (rulebook: ={{MonthsSinceModified}} > {{StalenessThresholdMonths}})
        [NotMapped]
        public bool? IsStale
        {
            get => F.AsBool(F.Memo(this, "IsStale", () => F.Cmp(F.Of(this.MonthsSinceModified), ">", F.Nullif(F.Of(this.StalenessThresholdMonths))))); set { }
        }

        // Formula IsStaleAndHasAIAgent (rulebook: =AND({{IsStale}}, {{HasAIAgentStep}}))
        [NotMapped]
        public bool? IsStaleAndHasAIAgent
        {
            get => F.AsBool(F.Memo(this, "IsStaleAndHasAIAgent", () => F.And(F.Bool3(F.Of(this.IsStale)), F.Bool3(F.Of(this.HasAIAgentStep))))); set { }
        }

        // Formula CountDerivationLinks (rulebook: =COUNTIFS(WorkflowArtifacts!{{ProducedByWorkflow}}, Workflows!{{WorkflowId}}, WorkflowArtifacts!{{HasDerivationParent}}, TRUE))
        [NotMapped]
        public int? CountDerivationLinks
        {
            get => F.AsInt(F.Memo(this, "CountDerivationLinks", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowArtifact>(base.SoAContext, "WorkflowArtifacts", __c => __c.WorkflowArtifacts), __r => F.CritField(F.Of(__r.ProducedByWorkflow), F.Of(this.WorkflowId)) && F.CritLiteral(F.Of(__r.HasDerivationParent), F.B(true))))))); set { }
        }

        // Formula CountLegalOwnedSteps (rulebook: =COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}}, WorkflowSteps!{{IsLegalOwned}}, TRUE))
        [NotMapped]
        public int? CountLegalOwnedSteps
        {
            get => F.AsInt(F.Memo(this, "CountLegalOwnedSteps", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowStep>(base.SoAContext, "WorkflowSteps", __c => __c.WorkflowSteps), __r => F.CritField(F.Of(__r.Workflow), F.Of(this.WorkflowId)) && F.CritLiteral(F.Of(__r.IsLegalOwned), F.B(true))))))); set { }
        }

        // Formula CountEngineeringOwnedSteps (rulebook: =COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}}, WorkflowSteps!{{IsEngineeringOwned}}, TRUE))
        [NotMapped]
        public int? CountEngineeringOwnedSteps
        {
            get => F.AsInt(F.Memo(this, "CountEngineeringOwnedSteps", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowStep>(base.SoAContext, "WorkflowSteps", __c => __c.WorkflowSteps), __r => F.CritField(F.Of(__r.Workflow), F.Of(this.WorkflowId)) && F.CritLiteral(F.Of(__r.IsEngineeringOwned), F.B(true))))))); set { }
        }

        // Formula InvolvesEngineeringAndLegal (rulebook: =AND({{CountEngineeringOwnedSteps}} > 0, {{CountLegalOwnedSteps}} > 0))
        [NotMapped]
        public bool? InvolvesEngineeringAndLegal
        {
            get => F.AsBool(F.Memo(this, "InvolvesEngineeringAndLegal", () => F.And(F.Bool3(F.Cmp(F.Of(this.CountEngineeringOwnedSteps), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.CountLegalOwnedSteps), ">", F.I(0)))))); set { }
        }

        // Formula CountInferredPrecedencePairs (rulebook: =COUNTIFS(vw_step_precedence_closure!{{IsInferred}}, TRUE()))
        [NotMapped]
        public int? CountInferredPrecedencePairs
        {
            get => throw new System.NotSupportedException("Workflows.CountInferredPrecedencePairs: could not translate formula =COUNTIFS(vw_step_precedence_closure!{{IsInferred}}, TRUE()): no table named vw_step_precedence_closure"); set { }
        }

        // Formula CountAssertedPrecedencePairs (rulebook: =COUNTIFS(vw_step_precedence_closure!{{IsInferred}}, FALSE()))
        [NotMapped]
        public int? CountAssertedPrecedencePairs
        {
            get => throw new System.NotSupportedException("Workflows.CountAssertedPrecedencePairs: could not translate formula =COUNTIFS(vw_step_precedence_closure!{{IsInferred}}, FALSE()): no table named vw_step_precedence_closure"); set { }
        }

        // Formula CountOfPrecedenceClosurePairs (rulebook: ={{CountAssertedPrecedencePairs}} + {{CountInferredPrecedencePairs}})
        [NotMapped]
        public int? CountOfPrecedenceClosurePairs
        {
            get => F.AsInt(F.Memo(this, "CountOfPrecedenceClosurePairs", () => F.Integer(F.Add(F.Of(this.CountAssertedPrecedencePairs), F.Of(this.CountInferredPrecedencePairs))))); set { }
        }

        // Formula CountRolesWithBadFillerCardinality (rulebook: =COUNTIFS(Roles!{{HasExactlyOneFiller}}, FALSE()))
        [NotMapped]
        public int? CountRolesWithBadFillerCardinality
        {
            get => F.AsInt(F.Memo(this, "CountRolesWithBadFillerCardinality", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Role>(base.SoAContext, "Roles", __c => __c.Roles), __r => F.CritLiteral(F.Of(__r.HasExactlyOneFiller), F.B(false))))))); set { }
        }

        // Formula CountAgentTypeChanges (rulebook: =COUNTIFS(RoleAssignments!{{IsAgentTypeChange}}, TRUE))
        [NotMapped]
        public int? CountAgentTypeChanges
        {
            get => F.AsInt(F.Memo(this, "CountAgentTypeChanges", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritLiteral(F.Of(__r.IsAgentTypeChange), F.B(true))))))); set { }
        }

        // Formula CountComplianceAuditChanges (rulebook: =COUNTIFS(RoleAssignments!{{RequiresComplianceAudit}}, TRUE))
        [NotMapped]
        public int? CountComplianceAuditChanges
        {
            get => F.AsInt(F.Memo(this, "CountComplianceAuditChanges", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritLiteral(F.Of(__r.RequiresComplianceAudit), F.B(true))))))); set { }
        }

        // Formula CountApprovalGateSteps (rulebook: =COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}}, WorkflowSteps!{{IsApprovalGate}}, TRUE))
        [NotMapped]
        public int? CountApprovalGateSteps
        {
            get => F.AsInt(F.Memo(this, "CountApprovalGateSteps", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowStep>(base.SoAContext, "WorkflowSteps", __c => __c.WorkflowSteps), __r => F.CritField(F.Of(__r.Workflow), F.Of(this.WorkflowId)) && F.CritLiteral(F.Of(__r.IsApprovalGate), F.B(true))))))); set { }
        }

        // Formula CountGatesWithoutHumanApprover (rulebook: =COUNTIFS(ApprovalGates!{{HasHumanApprover}}, FALSE))
        [NotMapped]
        public int? CountGatesWithoutHumanApprover
        {
            get => F.AsInt(F.Memo(this, "CountGatesWithoutHumanApprover", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ApprovalGate>(base.SoAContext, "ApprovalGates", __c => __c.ApprovalGates), __r => F.CritLiteral(F.Of(__r.HasHumanApprover), F.B(false))))))); set { }
        }

        // Formula CountWorkflowArtifacts (rulebook: =COUNTIFS(WorkflowArtifacts!{{ProducedByWorkflow}}, Workflows!{{WorkflowId}}))
        [NotMapped]
        public int? CountWorkflowArtifacts
        {
            get => F.AsInt(F.Memo(this, "CountWorkflowArtifacts", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowArtifact>(base.SoAContext, "WorkflowArtifacts", __c => __c.WorkflowArtifacts), __r => F.CritField(F.Of(__r.ProducedByWorkflow), F.Of(this.WorkflowId))))))); set { }
        }

        // Formula CountRolesWithEscalationViolation (rulebook: =COUNTIFS(Roles!{{EscalationViolation}}, TRUE))
        [NotMapped]
        public int? CountRolesWithEscalationViolation
        {
            get => F.AsInt(F.Memo(this, "CountRolesWithEscalationViolation", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Role>(base.SoAContext, "Roles", __c => __c.Roles), __r => F.CritLiteral(F.Of(__r.EscalationViolation), F.B(true))))))); set { }
        }

        // Formula CountUnconsumedDatasets (rulebook: =COUNTIFS(Datasets!{{IsConsumed}}, FALSE))
        [NotMapped]
        public int? CountUnconsumedDatasets
        {
            get => F.AsInt(F.Memo(this, "CountUnconsumedDatasets", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Dataset>(base.SoAContext, "Datasets", __c => __c.Datasets), __r => F.CritLiteral(F.Of(__r.IsConsumed), F.B(false))))))); set { }
        }

        // Formula Cq1Satisfied (rulebook: ={{CountOfPrecedenceClosurePairs}} = {{CountOfNonProposedSteps}} * ({{CountOfNonProposedSteps}} - 1) / 2)
        [NotMapped]
        public bool? Cq1Satisfied
        {
            get => F.AsBool(F.Memo(this, "Cq1Satisfied", () => F.Eq(F.Of(this.CountOfPrecedenceClosurePairs), F.Div(F.Mul(F.Of(this.CountOfNonProposedSteps), F.Sub(F.Of(this.CountOfNonProposedSteps), F.I(1))), F.I(2))))); set { }
        }

        // Formula Cq2Satisfied (rulebook: =AND({{CountApprovalGateSteps}} > 0, {{CountGatesWithoutHumanApprover}} = 0))
        [NotMapped]
        public bool? Cq2Satisfied
        {
            get => F.AsBool(F.Memo(this, "Cq2Satisfied", () => F.And(F.Bool3(F.Cmp(F.Of(this.CountApprovalGateSteps), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.CountGatesWithoutHumanApprover), F.I(0)))))); set { }
        }

        // Formula Cq3Satisfied (rulebook: =NOT({{HasConsistencyViolation}}))
        [NotMapped]
        public bool? Cq3Satisfied
        {
            get => F.AsBool(F.Memo(this, "Cq3Satisfied", () => F.Not(F.Bool3(F.Of(this.HasConsistencyViolation))))); set { }
        }

        // Formula Cq4Satisfied (rulebook: ={{CountDerivationLinks}} = {{CountWorkflowArtifacts}} - 1)
        [NotMapped]
        public bool? Cq4Satisfied
        {
            get => F.AsBool(F.Memo(this, "Cq4Satisfied", () => F.Eq(F.Of(this.CountDerivationLinks), F.Sub(F.Of(this.CountWorkflowArtifacts), F.I(1))))); set { }
        }

        // Formula Cq5Satisfied (rulebook: =NOT({{IsStale}}))
        [NotMapped]
        public bool? Cq5Satisfied
        {
            get => F.AsBool(F.Memo(this, "Cq5Satisfied", () => F.Not(F.Bool3(F.Of(this.IsStale))))); set { }
        }

        // Formula Cq6Satisfied (rulebook: ={{CountRolesWithEscalationViolation}} = 0)
        [NotMapped]
        public bool? Cq6Satisfied
        {
            get => F.AsBool(F.Memo(this, "Cq6Satisfied", () => F.Eq(F.Of(this.CountRolesWithEscalationViolation), F.I(0)))); set { }
        }

        // Formula Cq7Satisfied (rulebook: ={{InvolvesEngineeringAndLegal}})
        [NotMapped]
        public bool? Cq7Satisfied
        {
            get => F.AsBool(F.Memo(this, "Cq7Satisfied", () => F.Of(this.InvolvesEngineeringAndLegal))); set { }
        }

        // Formula Cq8Satisfied (rulebook: ={{CountUnconsumedDatasets}} = 0)
        [NotMapped]
        public bool? Cq8Satisfied
        {
            get => F.AsBool(F.Memo(this, "Cq8Satisfied", () => F.Eq(F.Of(this.CountUnconsumedDatasets), F.I(0)))); set { }
        }


        public string? WorkflowStatus { get; set; }
        public string? WorkflowSteps { get; set; }

        private WorkflowStatusConcept _workflowStatusConcept;

        [ForeignKey("WorkflowStatus")]
        public virtual WorkflowStatusConcept WorkflowStatusConcept
        {
            get
            {
                if (_workflowStatusConcept == null && !string.IsNullOrEmpty(WorkflowStatus))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowStatusConcept - no database context is set. WorkflowStatus: " + WorkflowStatus + ".");
                        }
                        return null;
                    }
                    _workflowStatusConcept = base.SoAContext.WorkflowStatusConcepts.Find(WorkflowStatus);
                    if (_workflowStatusConcept != null)
                    {
                        base.SoAContext.Attach(_workflowStatusConcept);
                    }
                }
                return _workflowStatusConcept;
            }
            set
            {
                if (_workflowStatusConcept != value)
                {
                    _workflowStatusConcept = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_workflowStatusConcept != null)
                    {
                        WorkflowStatus = _workflowStatusConcept.ConceptId;
                    }
                }
            }
        }

        private WorkflowStep _workflowStep;

        [ForeignKey("WorkflowSteps")]
        public virtual WorkflowStep WorkflowStep
        {
            get
            {
                if (_workflowStep == null && !string.IsNullOrEmpty(WorkflowSteps))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowStep - no database context is set. WorkflowSteps: " + WorkflowSteps + ".");
                        }
                        return null;
                    }
                    _workflowStep = base.SoAContext.WorkflowSteps.Find(WorkflowSteps);
                    if (_workflowStep != null)
                    {
                        base.SoAContext.Attach(_workflowStep);
                    }
                }
                return _workflowStep;
            }
            set
            {
                if (_workflowStep != value)
                {
                    _workflowStep = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_workflowStep != null)
                    {
                        WorkflowSteps = _workflowStep.WorkflowStepId;
                    }
                }
            }
        }

        private ObservableCollection<WorkflowStep> _workflowWorkflowSteps;

        [InverseProperty("WorkflowRef")]
        public virtual ObservableCollection<WorkflowStep> WorkflowWorkflowSteps
        {
            get
            {
                if (_workflowWorkflowSteps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowWorkflowSteps - no database context is set. WorkflowId: " + this.WorkflowId + ".");
                        }
                        _workflowWorkflowSteps = new ObservableCollection<WorkflowStep>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowSteps.Where(x => x.Workflow == this.WorkflowId).ToList<WorkflowStep>();
                        _workflowWorkflowSteps = new ObservableCollection<WorkflowStep>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _workflowWorkflowSteps.CollectionChanged += WorkflowWorkflowSteps_CollectionChanged;
                }
                return _workflowWorkflowSteps;
            }
            private set
            {
                if (_workflowWorkflowSteps != null)
                {
                    _workflowWorkflowSteps.CollectionChanged -= WorkflowWorkflowSteps_CollectionChanged;
                }
                _workflowWorkflowSteps = value;
                if (_workflowWorkflowSteps != null)
                {
                    _workflowWorkflowSteps.CollectionChanged += WorkflowWorkflowSteps_CollectionChanged;
                }
            }
        }

        private void WorkflowWorkflowSteps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowStep>())
                {
                    item.Workflow = this.WorkflowId;
                }
            }
        }

        private ObservableCollection<WorkflowStatusConcept> _workflowStatusConcepts;

        [InverseProperty("Workflow")]
        public virtual ObservableCollection<WorkflowStatusConcept> WorkflowStatusConcepts
        {
            get
            {
                if (_workflowStatusConcepts == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowStatusConcepts - no database context is set. WorkflowId: " + this.WorkflowId + ".");
                        }
                        _workflowStatusConcepts = new ObservableCollection<WorkflowStatusConcept>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowStatusConcepts.Where(x => x.Workflows == this.WorkflowId).ToList<WorkflowStatusConcept>();
                        _workflowStatusConcepts = new ObservableCollection<WorkflowStatusConcept>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _workflowStatusConcepts.CollectionChanged += WorkflowStatusConcepts_CollectionChanged;
                }
                return _workflowStatusConcepts;
            }
            private set
            {
                if (_workflowStatusConcepts != null)
                {
                    _workflowStatusConcepts.CollectionChanged -= WorkflowStatusConcepts_CollectionChanged;
                }
                _workflowStatusConcepts = value;
                if (_workflowStatusConcepts != null)
                {
                    _workflowStatusConcepts.CollectionChanged += WorkflowStatusConcepts_CollectionChanged;
                }
            }
        }

        private void WorkflowStatusConcepts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowStatusConcept>())
                {
                    item.Workflows = this.WorkflowId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.WorkflowStatusConcept;
            _ = this.WorkflowStep;
            _ = this.WorkflowWorkflowSteps;
            _ = this.WorkflowStatusConcepts;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
