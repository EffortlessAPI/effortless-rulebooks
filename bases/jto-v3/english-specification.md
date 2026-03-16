# JTO - v3 Rulebook Specification

## Overview
The JTO - v3 rulebook defines workflows, workflow steps, roles, and human agents for managing various business processes. This document outlines how to compute calculated fields based on the input data provided in the rulebook.

## Entities with Calculated Fields

### 1. Workflows
#### Input Fields
- **WorkflowId** (string): Unique identifier for the workflow.
- **Title** (string): Title of the workflow.
- **Description** (string): Description of the workflow's purpose.
- **Created** (datetime): Date when the workflow was created.
- **Modified** (datetime): Date when the workflow was last modified.
- **Identifier** (string): A specific code representing the workflow.
- **WorkflowSteps** (string): Identifier for the related workflow steps.

#### Calculated Fields
Currently, there are no calculated fields defined in the Workflows entity.

---

### 2. WorkflowSteps
#### Input Fields
- **WorkflowStepId** (string): Unique identifier for the workflow step.
- **Label** (string): Label describing the workflow step.
- **SequencePosition** (integer): Position of the step in the workflow sequence.
- **RequiresHumanApproval** (boolean): Indicates if human approval is required for this step.
- **IsStepOf** (string): Identifier for the workflow this step is associated with.
- **AssignedRole** (string): Role assigned to this workflow step.

#### Calculated Fields
Currently, there are no calculated fields defined in the WorkflowSteps entity.

---

### 3. Roles
#### Input Fields
- **RoleId** (string): Unique identifier for the role.
- **Label** (string): Name of the role.
- **Comment** (string): Description of the role's responsibilities.
- **FilledBy** (string): Identifier for the human agent filling this role.
- **WorkflowSteps** (string): Identifier for the related workflow steps.

#### Calculated Fields
Currently, there are no calculated fields defined in the Roles entity.

---

### 4. HumanAgents
#### Input Fields
- **HumanAgentId** (string): Unique identifier for the human agent.
- **Name** (string): Name of the human agent.
- **Mbox** (string): Email address of the human agent.
- **Roles** (string): Identifier for the role assigned to the human agent.

#### Calculated Fields
Currently, there are no calculated fields defined in the HumanAgents entity.

---

## Summary
The JTO - v3 rulebook provides a structured framework for managing workflows, workflow steps, roles, and human agents. While the entities defined in this rulebook do not currently include any calculated fields, the input fields are essential for understanding the relationships and responsibilities within the workflows. This specification serves as a guide for future enhancements where calculated fields may be introduced.