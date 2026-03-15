# Specification Document for Jessica Talisman - BASIC Ontology Parts 1-3

## Overview
This document provides a detailed specification for the rulebook derived from the Airtable base "Jessica Talisman - BASIC Ontology Parts 1-3". It outlines how to compute calculated fields for various entities within the rulebook, specifically focusing on workflows and their associated steps.

---

## Workflows

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the workflow.

### Calculated Fields

#### 1. Name
- **Description:** This field provides a machine-friendly name for the workflow, which is used for programmatic reference and URL slug generation.
- **Computation:** To compute the `Name`, take the `DisplayName`, convert it to lowercase, and replace spaces with hyphens.
- **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
- **Example:** 
  - If `DisplayName` is "Performance Review", then:
  - `Name` = "performance-review"

---

### Input Fields
1. **WorkflowSteps**
   - **Type:** String (raw)
   - **Description:** Reference to workflow steps associated with this workflow.

### Calculated Fields

#### 2. CountOfNonProposedSteps
- **Description:** This field counts the number of workflow steps associated with the workflow, which helps in analyzing workflow complexity.
- **Computation:** Count the number of entries in `WorkflowSteps` that match the current `WorkflowId`.
- **Formula:** `=COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}})`
- **Example:**
  - If `WorkflowSteps` contains "system-notification-sent, step-2, recwwXHLqxKPhj6Mt" for the workflow with `WorkflowId` "performance-review", then:
  - `CountOfNonProposedSteps` = 3

#### 3. HasMoreThan1Step
- **Description:** This boolean field indicates whether the workflow has more than one step.
- **Computation:** Check if `CountOfNonProposedSteps` is greater than 1.
- **Formula:** `={{CountOfNonProposedSteps}} > 1`
- **Example:**
  - If `CountOfNonProposedSteps` is 3, then:
  - `HasMoreThan1Step` = true

---

## WorkflowSteps

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the workflow step.

### Calculated Fields

#### 1. Name
- **Description:** This field provides a machine-friendly name for the workflow step, similar to the `Name` field in workflows.
- **Computation:** To compute the `Name`, take the `DisplayName`, convert it to lowercase, and replace spaces with hyphens.
- **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
- **Example:**
  - If `DisplayName` is "Submit Request", then:
  - `Name` = "submit-request"

---

## ApprovalGates

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the approval gate.

### Calculated Fields

#### 1. Name
- **Description:** This field provides a machine-friendly name for the approval gate.
- **Computation:** To compute the `Name`, take the `DisplayName`, convert it to lowercase, and replace spaces with hyphens.
- **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
- **Example:**
  - If `DisplayName` is "Manager Approval", then:
  - `Name` = "manager-approval"

---

## PrecedesSteps

### Input Fields
1. **StepNumber**
   - **Type:** Integer (raw)
   - **Description:** The ordinal position of the step in the workflow.

### Calculated Fields

#### 1. DisplayName
- **Description:** This field generates a display name for the step based on its sequence number.
- **Computation:** Concatenate the string "Step-" with the `StepNumber`.
- **Formula:** `="Step-" & {{StepNumber}}`
- **Example:**
  - If `StepNumber` is 1, then:
  - `DisplayName` = "Step-1"

---

## Roles

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the role.

### Calculated Fields

#### 1. Name
- **Description:** This field provides a machine-friendly name for the role.
- **Computation:** To compute the `Name`, take the `DisplayName` and convert it to lowercase.
- **Formula:** `=LOWER({{DisplayName}})`
- **Example:**
  - If `DisplayName` is "Admin", then:
  - `Name` = "admin"

---

## Conclusion
This specification outlines the necessary steps to compute calculated fields for workflows, workflow steps, approval gates, precedes steps, and roles within the Jessica Talisman - BASIC Ontology Parts 1-3 rulebook. By following the provided formulas and examples, users can accurately derive the calculated values as intended.