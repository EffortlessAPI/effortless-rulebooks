# Specification Document for Jessica Talisman - BASIC Ontology Parts 1-3

## Overview
This document provides a detailed specification for calculating fields within the rulebook "Jessica Talisman - BASIC Ontology Parts 1-3." The rulebook is structured around workflows and their associated steps, including various calculated fields that derive values from raw input data. This specification outlines how to compute these calculated fields based on the provided raw input fields.

## Workflows

### Input Fields
1. **WorkflowId**
   - **Type:** string
   - **Description:** Unique identifier for the workflow.

2. **DisplayName**
   - **Type:** string
   - **Description:** A human-readable name for the workflow.

3. **WorkflowSteps**
   - **Type:** string
   - **Description:** A reference to the workflow steps associated with this workflow.

### Calculated Fields

#### 1. Name
- **Description:** A machine-friendly name for the workflow, used for programmatic reference and URL slug generation.
- **Calculation Method:** 
  - Convert the `DisplayName` to lowercase.
  - Replace spaces with hyphens.
- **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
- **Example:** 
  - If `DisplayName` is "Performance Review", then:
    - Lowercase: "performance review"
    - Replace spaces: "performance-review"
  - Result: `performance-review`

#### 2. CountOfNonProposedSteps
- **Description:** The count of workflow steps associated with this workflow, useful for analyzing workflow complexity.
- **Calculation Method:** 
  - Count the number of entries in `WorkflowSteps` that match the current `WorkflowId`.
- **Formula:** `=COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}})`
- **Example:** 
  - For the workflow with `WorkflowId` "performance-review" and `WorkflowSteps` containing "system-notification-sent, step-2, recwwXHLqxKPhj6Mt", the count is 3.
  - Result: `3`

#### 3. HasMoreThan1Step
- **Description:** A boolean indicating whether the workflow has more than one step.
- **Calculation Method:** 
  - Check if `CountOfNonProposedSteps` is greater than 1.
- **Formula:** `={{CountOfNonProposedSteps}} > 1`
- **Example:** 
  - If `CountOfNonProposedSteps` is `3`, then:
    - `3 > 1` evaluates to `true`.
  - Result: `true`

## Workflow Steps

### Input Fields
1. **WorkflowStepId**
   - **Type:** string
   - **Description:** Unique identifier for the workflow step.

2. **DisplayName**
   - **Type:** string
   - **Description:** A human-readable name for the workflow step.

3. **Workflow**
   - **Type:** string
   - **Description:** Foreign key to the parent workflow.

### Calculated Fields

#### 1. Name
- **Description:** A machine-friendly name for the workflow step, used for programmatic reference.
- **Calculation Method:** 
  - Convert the `DisplayName` to lowercase.
  - Replace spaces with hyphens.
- **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
- **Example:** 
  - If `DisplayName` is "Submit Request", then:
    - Lowercase: "submit request"
    - Replace spaces: "submit-request"
  - Result: `submit-request`

## Approval Gates

### Input Fields
1. **ApprovalGateId**
   - **Type:** string
   - **Description:** Unique identifier for the approval gate.

2. **DisplayName**
   - **Type:** string
   - **Description:** A human-readable name for the approval gate.

### Calculated Fields

#### 1. Name
- **Description:** A machine-friendly name for the approval gate, used for programmatic reference.
- **Calculation Method:** 
  - Convert the `DisplayName` to lowercase.
  - Replace spaces with hyphens.
- **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
- **Example:** 
  - If `DisplayName` is "Initial Review", then:
    - Lowercase: "initial review"
    - Replace spaces: "initial-review"
  - Result: `initial-review`

## Conclusion
This specification provides a clear guide on how to compute the calculated fields within the "Jessica Talisman - BASIC Ontology Parts 1-3" rulebook. By following the outlined methods and examples, users can derive the necessary values without needing to reference the original formulas directly.