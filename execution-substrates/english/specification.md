# Specification Document for Jessica Talisman - BASIC Ontology Parts 1-3 Rulebook

## Overview
This document provides a detailed specification for the calculated fields within the "Jessica Talisman - BASIC Ontology Parts 1-3" rulebook. The rulebook is structured to manage workflows, workflow steps, approval gates, roles, and other related entities. The focus is on how to compute specific calculated fields based on raw input data.

## Workflows

### Input Fields
1. **WorkflowId**
   - **Type:** String
   - **Description:** Unique identifier for the workflow.

2. **DisplayName**
   - **Type:** String
   - **Description:** Human-readable name of the workflow.

### Calculated Fields
1. **Name**
   - **Description:** A machine-friendly name for the workflow, used for programmatic reference and URL slug generation.
   - **Computation:** Convert the `DisplayName` to lowercase, replace spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** If `DisplayName` is "Employee Onboarding", then `Name` would be "employee-onboarding".

2. **CountOfSteps**
   - **Description:** The total number of steps associated with this workflow.
   - **Computation:** Count the number of entries in the `WorkflowSteps` table that reference this workflow by matching `WorkflowSteps.Workflow` with `WorkflowId`.
   - **Formula:** `=COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}})`
   - **Example:** If there are 2 steps linked to the workflow with `WorkflowId` "onboarding", then `CountOfSteps` would be 2.

3. **HasMoreThan1Step**
   - **Description:** A boolean indicating if the workflow has more than one step.
   - **Computation:** Check if `CountOfSteps` is greater than 1.
   - **Formula:** `={{CountOfSteps}} > 1`
   - **Example:** If `CountOfSteps` is 2, then `HasMoreThan1Step` would be `true`. If `CountOfSteps` is 1, it would be `false`.

## WorkflowSteps

### Input Fields
1. **WorkflowStepId**
   - **Type:** String
   - **Description:** Unique identifier for the workflow step.

2. **DisplayName**
   - **Type:** String
   - **Description:** Human-readable name of the workflow step.

3. **Workflow**
   - **Type:** String
   - **Description:** Foreign key to the parent workflow.

### Calculated Fields
1. **Name**
   - **Description:** A machine-friendly name for the workflow step.
   - **Computation:** Convert the `DisplayName` to lowercase, replace spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** If `DisplayName` is "Submit Request", then `Name` would be "submit-request".

## ApprovalGates

### Input Fields
1. **ApprovalGateId**
   - **Type:** String
   - **Description:** Unique identifier for the approval gate.

2. **DisplayName**
   - **Type:** String
   - **Description:** Human-readable name of the approval gate.

### Calculated Fields
1. **Name**
   - **Description:** A machine-friendly name for the approval gate.
   - **Computation:** Convert the `DisplayName` to lowercase, replace spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** If `DisplayName` is "Manager Approval", then `Name` would be "manager-approval".

## PrecedesSteps

### Input Fields
1. **PrecedesStepId**
   - **Type:** String
   - **Description:** Unique identifier for the precedes step.

2. **StepNumber**
   - **Type:** Integer
   - **Description:** Ordinal position of the step within its workflow.

### Calculated Fields
1. **DisplayName**
   - **Description:** A formatted string representing the step number.
   - **Computation:** Concatenate "Step-" with the `StepNumber`.
   - **Formula:** `="Step-" & {{StepNumber}}`
   - **Example:** If `StepNumber` is 1, then `DisplayName` would be "Step-1".

## Roles

### Input Fields
1. **RoleId**
   - **Type:** String
   - **Description:** Unique identifier for the role.

2. **DisplayName**
   - **Type:** String
   - **Description:** Human-readable name of the role.

### Calculated Fields
1. **Name**
   - **Description:** A machine-friendly name for the role.
   - **Computation:** Convert the `DisplayName` to lowercase.
   - **Formula:** `=LOWER({{DisplayName}})`
   - **Example:** If `DisplayName` is "Administrator", then `Name` would be "administrator".

## Conclusion
This specification outlines the necessary steps to compute calculated fields within the "Jessica Talisman - BASIC Ontology Parts 1-3" rulebook. By following the provided computations and examples, one can accurately derive the values for each calculated field based on the raw input data.