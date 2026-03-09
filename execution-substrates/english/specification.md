# Specification Document for Jessica Talisman - Ontology Parts 1-3 Rulebook

## Overview
This specification document outlines the calculated fields present in the "Jessica Talisman - Ontology Parts 1-3" rulebook. It provides detailed instructions on how to compute these fields based on the input data available in the rulebook. The rulebook is structured around workflows, workflow steps, approval gates, and roles, each containing specific fields that contribute to the overall functionality of the system.

## Workflows

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the workflow.

### Calculated Fields
1. **Name**
   - **Description:** This field generates a machine-friendly name for the workflow, which is used for programmatic reference and URL slug generation.
   - **Computation:** Convert the `DisplayName` to lowercase and replace spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:** 
     - If `DisplayName` is "Onboarding", then:
       - `Name` = `SUBSTITUTE(LOWER("Onboarding"), " ", "-")` = "onboarding"

2. **CountOfSteps**
   - **Description:** This field counts the number of workflow steps associated with this workflow.
   - **Computation:** Count the number of entries in the `WorkflowSteps` table where the `Workflow` matches the current workflow's `WorkflowId`.
   - **Formula:** `=COUNTIFS(WorkflowSteps!{{Workflow}}, Workflows!{{WorkflowId}})`
   - **Example:** 
     - If there is 1 step associated with the workflow "onboarding", then:
       - `CountOfSteps` = 1

3. **HasMoreThan1Step**
   - **Description:** This field indicates whether the workflow has more than one step.
   - **Computation:** Check if `CountOfSteps` is greater than 1.
   - **Formula:** `={{CountOfSteps}} > 1`
   - **Example:** 
     - If `CountOfSteps` is 1, then:
       - `HasMoreThan1Step` = `1 > 1` = false

## WorkflowSteps

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the workflow step.

### Calculated Fields
1. **Name**
   - **Description:** This field generates a machine-friendly name for the workflow step, used for programmatic reference.
   - **Computation:** Convert the `DisplayName` to lowercase and replace spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:**
     - If `DisplayName` is "Submit Request", then:
       - `Name` = `SUBSTITUTE(LOWER("Submit Request"), " ", "-")` = "submit-request"

## ApprovalGates

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the approval gate.

### Calculated Fields
1. **Name**
   - **Description:** This field generates a machine-friendly name for the approval gate.
   - **Computation:** Convert the `DisplayName` to lowercase and replace spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:**
     - If `DisplayName` is "Initial Review", then:
       - `Name` = `SUBSTITUTE(LOWER("Initial Review"), " ", "-")` = "initial-review"

## Roles

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the role.

### Calculated Fields
1. **Name**
   - **Description:** This field generates a lowercase name for the role.
   - **Computation:** Convert the `DisplayName` to lowercase.
   - **Formula:** `=LOWER({{DisplayName}})`
   - **Example:**
     - If `DisplayName` is "Admin", then:
       - `Name` = `LOWER("Admin")` = "admin"

## Departments

### Input Fields
1. **DisplayName**
   - **Type:** String (raw)
   - **Description:** A human-readable name for the department.

### Calculated Fields
1. **Name**
   - **Description:** This field generates a machine-friendly name for the department.
   - **Computation:** Convert the `DisplayName` to lowercase and replace spaces with hyphens.
   - **Formula:** `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example:**
     - If `DisplayName` is "Human Resources", then:
       - `Name` = `SUBSTITUTE(LOWER("Human Resources"), " ", "-")` = "human-resources"

## Conclusion
This document provides a comprehensive guide to calculating the fields in the "Jessica Talisman - Ontology Parts 1-3" rulebook. By following the outlined procedures, users can accurately compute the necessary values based on the raw input data provided in the rulebook.