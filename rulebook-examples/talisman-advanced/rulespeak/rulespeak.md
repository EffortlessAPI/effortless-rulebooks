# 📘 Talisman: Advanced — RuleSpeak

_Workflow / approval ontology with cross-entity lookups and multi-step derivation._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Workflow** | A workflow tracked by the business. |
| Count of Steps | Calculated count of workflow steps in this workflow. Useful for workflow complexity analysis and reporting. |
| Has More Than1 Step | True when the count of steps is greater than 1. |
| **Workflow Step** | A workflow step tracked by the business. |
| Execution Actor Type | Computed: `If(AssignedRoleDepartment = "HumanAgent", "HumanAgent", If(AssignedRoleDepartment = "AIAgent", "AIAgent", If(AssignedRoleDepartment = "AutomatedPipeline", "AutomatedPipeline", Blank())))`. |
| Assigned Role Department | The owned by of the workflow step's assigned role. |
| Approval Gate Escalation Threshold Hours | The escalation threshold hours of the workflow step's approval gate. |
| **Approval** | An approval tracked by the business. |
| **Precedes Step** | A precedes step tracked by the business. |
| Display Name | Computed as the literal “Step-”, followed by the step number. |
| **Role** | A role tracked by the business. |
| **Department** | A department tracked by the business. |
| **Human Agent** | A human agent tracked by the business. |
| **AI Agent** | An AI agent tracked by the business. |
| **Automated Pipeline** | An automated pipeline tracked by the business. |
| **Sample Data Governance** | Governance rules for which sample-data rows belong in this rulebook. The Talisman ADVANCED demo is supposed to mirror the worked ABox in the source article — the Production Deployment Workflow at Special Solutions — not a generic catalog of every workflow type. Each row below records one keep/remove decision with a reason and source citation. Used as a checklist when curating seed data; rows here are NOT consumed by substrate code, only by the data-curation process. Originated from LEGACY_PLANs/PLAN-to-cleanup-ADVANCED-table.md (now relocated to this project as PLAN-to-cleanup-ADVANCED-table.md). |

## 2 Fact Types

- a **workflow step** may reference one **workflow**
- a **workflow step** may reference one **role**
- a **workflow step** may reference one **approval**
- a **workflow step** may reference one **precedes step**
- a **precedes step** may reference one **workflow step**
- a **role** may reference one **human agent**
- a **role** may reference one **AI agent**
- a **role** may reference one **automated pipeline**
- a **role** may reference one **department**
- a **role** may reference one **role**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Count of Steps** | A workflow's count of steps is the number of workflow steps related to the workflow. |
| **DR-2 Has More Than1 Step** | A workflow is considered to have a more than1 step if the count of steps is greater than 1. |
| **DR-3 Execution Actor Type** | The workflow step's execution actor type is determined by the following priority:<br>1. the literal “HumanAgent”, if the assigned role department is the literal “HumanAgent”;<br>2. the literal “AIAgent”, if the assigned role department is the literal “AIAgent”;<br>3. the literal “AutomatedPipeline”, if the assigned role department is the literal “AutomatedPipeline”;<br>4. otherwise `Blank()`. |
| **DR-4 Assigned Role Department** | A workflow step's assigned role department is the owned by of the workflow step's assigned role. |
| **DR-5 Approval Gate Escalation Threshold Hours** | A workflow step's approval gate escalation threshold hours is the escalation threshold hours of the workflow step's approval gate. |
| **DR-6 Display Name** | A precedes step's display name is computed as the literal “Step-”, followed by the step number. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Workflows.CountOfSteps** | rollup | `Count(WorkflowSteps via Workflow)` |
| **Workflows.HasMoreThan1Step** | formula | `CountOfSteps > 1` |
| **WorkflowSteps.ExecutionActorType** | formula | `If(AssignedRoleDepartment = "HumanAgent", "HumanAgent", If(AssignedRoleDepartment = "AIAgent", "AIAgent", If(AssignedRoleDepartment = "AutomatedPipeline", "AutomatedPipeline", Blank())))` |
| **WorkflowSteps.AssignedRoleDepartment** | lookup | `Lookup(Roles.OwnedBy via AssignedRole)` |
| **WorkflowSteps.ApprovalGateEscalationThresholdHours** | lookup | `Lookup(Approvals.EscalationThresholdHours via ApprovalGate)` |
| **PrecedesSteps.DisplayName** | formula | `"Step-" & StepNumber` |
