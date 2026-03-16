# JTO - v3 Rulebook Specification Document

## Overview
This document outlines the specifications for the JTO - v3 rulebook, which is designed to manage workflows, workflow steps, roles, and human agents within a structured framework. The rulebook includes various entities, each with defined fields and relationships, and specifies how to compute certain calculated fields based on raw input data.

## Entities with Calculated Fields

### 1. Workflows
#### Input Fields
- **WorkflowId**: `string` - Unique identifier for the workflow.
- **Title**: `string` - Human-readable name for the workflow.
- **Description**: `string` - Detailed description of the workflow's purpose and scope.
- **Created**: `datetime` - Date the workflow was created.
- **Modified**: `datetime` - Date the workflow was last modified.
- **Identifier**: `string` - External reference identifier (e.g., ticket number).
- **WorkflowSteps**: `string` - Steps contained in this workflow.

#### Calculated Field
- **CountOfWorkflowSteps**: `integer`
  - **Description**: This field counts the number of steps associated with the workflow by checking how many steps have the current workflow's ID as their parent.
  - **How to Compute**: To compute this value, you will count all entries in the `WorkflowSteps` table where the `IsStepOf` field matches the current workflow's `WorkflowId`.
  - **Formula**: `=COUNTIFS(WorkflowSteps!{{IsStepOf}}, Workflows!{{WorkflowId}})`
  - **Example**: For the workflow with `WorkflowId` "production-deployment-workflow", there are three steps: "legal-review", "risk-assessment", and "release-approval". Thus, `CountOfWorkflowSteps` would be 3.

---

### 2. WorkflowSteps
#### Input Fields
- **WorkflowStepId**: `string` - Unique identifier for the workflow step.
- **Label**: `string` - Human-readable name for the step.
- **SequencePosition**: `integer` - Ordinal position in the workflow sequence.
- **RequiresHumanApproval**: `boolean` - Indicates if this step requires human approval.
- **IsStepOf**: `string` - Parent workflow containing this step.
- **AssignedRole**: `string` - Role responsible for this step.

#### Calculated Fields
- **IsStepOfTitle**: `string`
  - **Description**: This field retrieves the title of the parent workflow associated with this step.
  - **How to Compute**: Use the `MATCH` function to find the row in the `Workflows` table where the `WorkflowId` matches the `IsStepOf` field, then use `INDEX` to get the corresponding `Title`.
  - **Formula**: `=INDEX(Workflows!{{Title}}, MATCH(WorkflowSteps!{{IsStepOf}}, Workflows!{{WorkflowId}}, 0))`
  - **Example**: For the step with `IsStepOf` "production-deployment-workflow", the `IsStepOfTitle` would return "Production Deployment Workflow".

- **IsStepOfDescription**: `string`
  - **Description**: This field retrieves the description of the parent workflow associated with this step.
  - **How to Compute**: Similar to `IsStepOfTitle`, use `MATCH` to find the row in the `Workflows` table and then `INDEX` to get the `Description`.
  - **Formula**: `=INDEX(Workflows!{{Description}}, MATCH(WorkflowSteps!{{IsStepOf}}, Workflows!{{WorkflowId}}, 0))`
  - **Example**: For the step with `IsStepOf` "production-deployment-workflow", the `IsStepOfDescription` would return "End-to-end workflow for deploying software releases to production, including risk analysis, legal clearance, and release approval."

- **IsStepOfIdentifier**: `string`
  - **Description**: This field retrieves the external identifier of the parent workflow associated with this step.
  - **How to Compute**: Use `MATCH` to find the row in the `Workflows` table where the `WorkflowId` matches the `IsStepOf` field, then use `INDEX` to get the corresponding `Identifier`.
  - **Formula**: `=INDEX(Workflows!{{Identifier}}, MATCH(WorkflowSteps!{{IsStepOf}}, Workflows!{{WorkflowId}}, 0))`
  - **Example**: For the step with `IsStepOf` "production-deployment-workflow", the `IsStepOfIdentifier` would return "WF-PROD-001".

- **AssignedRoleLabel**: `string`
  - **Description**: This field retrieves the label of the role assigned to this step.
  - **How to Compute**: Use `MATCH` to find the row in the `Roles` table where the `RoleId` matches the `AssignedRole` field, then use `INDEX` to get the corresponding `Label`.
  - **Formula**: `=INDEX(Roles!{{Label}}, MATCH(WorkflowSteps!{{AssignedRole}}, Roles!{{RoleId}}, 0))`
  - **Example**: For the step with `AssignedRole` "risk-analyst", the `AssignedRoleLabel` would return "Risk Analyst".

- **AssignedRoleComment**: `string`
  - **Description**: This field retrieves the comment or description of the assigned role.
  - **How to Compute**: Similar to `AssignedRoleLabel`, use `MATCH` to find the row in the `Roles` table and then `INDEX` to get the `Comment`.
  - **Formula**: `=INDEX(Roles!{{Comment}}, MATCH(WorkflowSteps!{{AssignedRole}}, Roles!{{RoleId}}, 0))`
  - **Example**: For the step with `AssignedRole` "risk-analyst", the `AssignedRoleComment` would return "Role responsible for risk assessment. In full ontology, filled by AI agent."

- **AssignedRoleFilledBy**: `string`
  - **Description**: This field retrieves the agent currently filling the assigned role.
  - **How to Compute**: Use `MATCH` to find the row in the `Roles` table where the `RoleId` matches the `AssignedRole`, then use `INDEX` to get the corresponding `FilledBy`.
  - **Formula**: `=INDEX(Roles!{{FilledBy}}, MATCH(WorkflowSteps!{{AssignedRole}}, Roles!{{RoleId}}, 0))`
  - **Example**: For the step with `AssignedRole` "release-manager", the `AssignedRoleFilledBy` would return "maria-gonzalez".

---

### 3. Roles
#### Input Fields
- **RoleId**: `string` - Unique identifier for the role.
- **Label**: `string` - Human-readable name for the role.
- **Comment**: `string` - Description of the role's responsibilities.
- **FilledBy**: `string` - Agent currently filling this role.

#### Calculated Fields
- **CountOfWorkflowSteps**: `integer`
  - **Description**: This field counts the number of workflow steps assigned to this role.
  - **How to Compute**: Count all entries in the `WorkflowSteps` table where the `AssignedRole` matches the current role's `RoleId`.
  - **Formula**: `=COUNTIFS(WorkflowSteps!{{AssignedRole}}, Roles!{{RoleId}})`
  - **Example**: For the role with `RoleId` "release-manager", there is one step assigned: "release-approval". Thus, `CountOfWorkflowSteps` would be 1.

- **FilledByName**: `string`
  - **Description**: This field retrieves the name of the agent filling this role.
  - **How to Compute**: Use `MATCH` to find the row in the `HumanAgents` table where the `HumanAgentId` matches the `FilledBy` field, then use `INDEX` to get the corresponding `Name`.
  - **Formula**: `=INDEX(HumanAgents!{{Name}}, MATCH(Roles!{{FilledBy}}, HumanAgents!{{HumanAgentId}}, 0))`
  - **Example**: For the role with `FilledBy` "maria-gonzalez", the `FilledByName` would return "Maria Gonzalez".

- **FilledByMBox**: `string`
  - **Description**: This field retrieves the email address of the agent filling this role.
  - **How to Compute**: Similar to `FilledByName`, use `MATCH` to find the row in the `HumanAgents` table and then `INDEX` to get the `Mbox`.
  - **Formula**: `=INDEX(HumanAgents!{{Mbox}}, MATCH(Roles!{{FilledBy}}, HumanAgents!{{HumanAgentId}}, 0))`
  - **Example**: For the role with `FilledBy` "maria-gonzalez", the `FilledByMBox` would return "maria.gonzalez@specialsolutions.example".

---

### 4. HumanAgents
#### Input Fields
- **HumanAgentId**: `string` - Unique identifier for the human agent.
- **Name**: `string` - Full name of the person.
- **Mbox**: `string` - Email address of the person.

#### Calculated Field
- **CountOfRoles**: `integer`
  - **Description**: This field counts the number of roles currently filled by this agent.
  - **How to Compute**: Count all entries in the `Roles` table where the `FilledBy` matches the current agent's `HumanAgentId`.
  - **Formula**: `=COUNTIFS(Roles!{{FilledBy}}, HumanAgents!{{HumanAgentId}})`
  - **Example**: For the agent with `HumanAgentId` "maria-gonzalez", the `CountOfRoles` would be 1, as she is filling the "release-manager" role.

---

This specification document provides a comprehensive guide to computing calculated fields within the JTO - v3 rulebook, ensuring clarity and consistency in data management across workflows, steps, roles, and agents.