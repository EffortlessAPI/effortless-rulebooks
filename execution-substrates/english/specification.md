# Specification Document for Jessica Talisman - BASIC Ontology Parts 1-3

## Overview
This specification document outlines the calculated fields present in the rulebook derived from the Airtable base "Jessica Talisman - BASIC Ontology Parts 1-3." It provides detailed instructions on how to compute these fields based on the raw input data available in the respective entities.

## Workflows

### Input Fields
1. **WorkflowId**
   - **Type:** String
   - **Description:** Unique identifier for the workflow.

2. **DisplayName**
   - **Type:** String
   - **Description:** Human-readable name for the workflow.

3. **WorkflowSteps**
   - **Type:** String
   - **Description:** Reference to workflow steps associated with this workflow.

### Calculated Fields
1. **Name**
   - **Description:** A machine-friendly name for the workflow, used for programmatic reference and URL slug generation.
   - **Computation:** Convert the `DisplayName` to lowercase and replace spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** For `DisplayName` "Employee Onboarding", the computed `Name` would be "employee-onboarding".

2. **CountOfSteps**
   - **Description:** The total number of steps associated with this workflow.
   - **Computation:** Count the number of entries in the `WorkflowSteps` that match the current `WorkflowId`.
   - **Formula:** `=COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}})`
   - **Example:** If there are two steps linked to the workflow with `WorkflowId` "asset-management", the `CountOfSteps` would be 2.

3. **HasMoreThan1Step**
   - **Description:** A boolean value indicating whether the workflow has more than one step.
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
   - **Description:** Human-readable name for the workflow step.

3. **Workflow**
   - **Type:** String
   - **Description:** Reference to the parent workflow associated with this step.

### Calculated Fields
1. **Name**
   - **Description:** A machine-friendly name for the workflow step, used for programmatic reference.
   - **Computation:** Convert the `DisplayName` to lowercase and replace spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** For `DisplayName` "Submit Request", the computed `Name` would be "submit-request".

## ApprovalGates

### Input Fields
1. **ApprovalGateId**
   - **Type:** String
   - **Description:** Unique identifier for the approval gate.

2. **DisplayName**
   - **Type:** String
   - **Description:** Human-readable name for the approval gate.

### Calculated Fields
1. **Name**
   - **Description:** A machine-friendly name for the approval gate, used for programmatic reference.
   - **Computation:** Convert the `DisplayName` to lowercase and replace spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** For `DisplayName` "Manager Approval", the computed `Name` would be "manager-approval".

## PrecedesSteps

### Input Fields
1. **PrecedesStepId**
   - **Type:** String
   - **Description:** Unique identifier for the precedes step.

2. **StepNumber**
   - **Type:** Integer
   - **Description:** The ordinal position of the step in the sequence.

### Calculated Fields
1. **DisplayName**
   - **Description:** A formatted string that represents the step number in a user-friendly way.
   - **Computation:** Concatenate the string "Step-" with the `StepNumber`.
   - **Formula:** `="Step-" & {{StepNumber}}`
   - **Example:** If `StepNumber` is 1, the computed `DisplayName` would be "Step-1".

## Roles

### Input Fields
1. **RoleId**
   - **Type:** String
   - **Description:** Unique identifier for the role.

2. **DisplayName**
   - **Type:** String
   - **Description:** Human-readable name for the role.

### Calculated Fields
1. **Name**
   - **Description:** A machine-friendly name for the role, used for programmatic reference.
   - **Computation:** Convert the `DisplayName` to lowercase.
   - **Formula:** `=LOWER({{DisplayName}})`
   - **Example:** For `DisplayName` "Admin", the computed `Name` would be "admin".

## Departments

### Input Fields
1. **DepartmentId**
   - **Type:** String
   - **Description:** Unique identifier for the department.

2. **DisplayName**
   - **Type:** String
   - **Description:** Human-readable name for the department.

### Calculated Fields
1. **Name**
   - **Description:** A machine-friendly name for the department, used for programmatic reference.
   - **Computation:** Convert the `DisplayName` to lowercase and replace spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** For `DisplayName` "Human Resources", the computed `Name` would be "human-resources".

This document provides a comprehensive guide to computing the calculated fields in the Jessica Talisman - BASIC Ontology Parts 1-3 rulebook. By following the outlined steps and examples, users can accurately derive the necessary values from the raw input data.