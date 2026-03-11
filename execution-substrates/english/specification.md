# Specification Document for Jessica Talisman - BASIC Ontology Parts 1-3

## Overview
This document outlines the specifications for calculating fields within the rulebook "Jessica Talisman - BASIC Ontology Parts 1-3." The rulebook is designed to manage workflows, workflow steps, approval gates, roles, departments, human agents, AI agents, and automated pipelines. It provides a structured way to compute specific fields based on raw input data.

## Workflows

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the workflow.

### Calculated Fields
1. **Name**
   - **Description:** This field generates a machine-friendly name for the workflow by converting the `DisplayName` to lowercase and replacing spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** For a `DisplayName` of "Employee Onboarding", the computed `Name` would be "employee-onboarding".

2. **CountOfSteps**
   - **Description:** This field counts the number of steps associated with the workflow by checking how many entries in the `WorkflowSteps` table reference the current `WorkflowId`.
   - **Formula:** `=COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}})`
   - **Example:** If there is one step associated with the workflow "onboarding", the `CountOfSteps` would be 1.

3. **HasMoreThan1Step**
   - **Description:** This boolean field indicates whether the workflow contains more than one step by comparing the `CountOfSteps` to 1.
   - **Formula:** `={{CountOfSteps}} > 1`
   - **Example:** If `CountOfSteps` is 1, then `HasMoreThan1Step` would be false.

## WorkflowSteps

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the workflow step.

### Calculated Fields
1. **Name**
   - **Description:** This field generates a machine-friendly name for the workflow step by converting the `DisplayName` to lowercase and replacing spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** For a `DisplayName` of "Submit Request", the computed `Name` would be "submit-request".

## ApprovalGates

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the approval gate.

### Calculated Fields
1. **Name**
   - **Description:** This field generates a machine-friendly name for the approval gate by converting the `DisplayName` to lowercase and replacing spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** For a `DisplayName` of "Initial Review", the computed `Name` would be "initial-review".

## PrecedesSteps

### Input Fields
1. **StepNumber**
   - **Type:** Integer (raw)
   - **Description:** The ordinal position of the step within its workflow.

### Calculated Fields
1. **DisplayName**
   - **Description:** This field generates a display name for the precedes step in the format "Step-X", where X is the `StepNumber`.
   - **Formula:** `="Step-" & {{StepNumber}}`
   - **Example:** If `StepNumber` is 1, the computed `DisplayName` would be "Step-1".

## Roles

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the role.

### Calculated Fields
1. **Name**
   - **Description:** This field generates a lowercase version of the `DisplayName` for programmatic reference.
   - **Formula:** `=LOWER({{DisplayName}})`
   - **Example:** For a `DisplayName` of "Administrator", the computed `Name` would be "administrator".

## Departments

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the department.

### Calculated Fields
1. **Name**
   - **Description:** This field generates a machine-friendly name for the department by converting the `DisplayName` to lowercase and replacing spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** For a `DisplayName` of "Human Resources", the computed `Name` would be "human-resources".

## Conclusion
This specification document provides a detailed guide on how to compute calculated fields based on raw input fields within the specified rulebook. By following these instructions, users can derive the correct values for each calculated field without needing to reference the original formulas directly.