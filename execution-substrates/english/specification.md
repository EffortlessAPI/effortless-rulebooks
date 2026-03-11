# Specification Document for Jessica Talisman - BASIC Ontology Parts 1-3

## Overview
This specification document outlines the calculated fields within the rulebook "Jessica Talisman - BASIC Ontology Parts 1-3." The rulebook is structured to manage workflows, workflow steps, approval gates, and other related entities. It provides a clear understanding of how to compute calculated fields based on raw input data.

## Workflows

### Input Fields
1. **DisplayName**
   - **Type**: String (raw)
   - **Description**: The human-readable name of the workflow, used for display purposes.

### Calculated Fields
1. **Name**
   - **Description**: This field generates a machine-friendly name for the workflow that can be used for programmatic references and URL slug generation.
   - **Computation**: To compute the `Name`, take the `DisplayName`, convert it to lowercase, and replace spaces with hyphens.
   - **Formula**: `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example**: If `DisplayName` is "Performance Review", the computed `Name` will be "performance-review".

2. **HasMoreThan1Step**
   - **Description**: This field indicates whether the workflow has more than one step based on the count of non-proposed steps.
   - **Computation**: Check if `CountOfNonProposedSteps` is greater than 1.
   - **Formula**: `={{CountOfNonProposedSteps}} > 1`
   - **Example**: If `CountOfNonProposedSteps` is 0, then `HasMoreThan1Step` will evaluate to `false`.

## Workflow Steps

### Input Fields
1. **DisplayName**
   - **Type**: String (raw)
   - **Description**: The human-readable name of the workflow step.

### Calculated Fields
1. **Name**
   - **Description**: This field generates a machine-friendly name for the workflow step that can be used for programmatic references.
   - **Computation**: To compute the `Name`, take the `DisplayName`, convert it to lowercase, and replace spaces with hyphens.
   - **Formula**: `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example**: If `DisplayName` is "Submit Request", the computed `Name` will be "submit-request".

## Approval Gates

### Input Fields
1. **DisplayName**
   - **Type**: String (raw)
   - **Description**: The human-readable name of the approval gate.

### Calculated Fields
1. **Name**
   - **Description**: This field generates a machine-friendly name for the approval gate that can be used for programmatic references.
   - **Computation**: To compute the `Name`, take the `DisplayName`, convert it to lowercase, and replace spaces with hyphens.
   - **Formula**: `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example**: If `DisplayName` is "Manager Approval", the computed `Name` will be "manager-approval".

## Precedes Steps

### Input Fields
1. **StepNumber**
   - **Type**: Integer (raw)
   - **Description**: The ordinal position of the step within its workflow.

### Calculated Fields
1. **DisplayName**
   - **Description**: This field generates a display name for the precedes step based on its step number.
   - **Computation**: To compute the `DisplayName`, concatenate the string "Step-" with the `StepNumber`.
   - **Formula**: `="Step-" & {{StepNumber}}`
   - **Example**: If `StepNumber` is 1, the computed `DisplayName` will be "Step-1".

## Roles

### Input Fields
1. **DisplayName**
   - **Type**: String (raw)
   - **Description**: The human-readable name of the role.

### Calculated Fields
1. **Name**
   - **Description**: This field generates a lowercase name for the role that can be used for programmatic references.
   - **Computation**: To compute the `Name`, convert the `DisplayName` to lowercase.
   - **Formula**: `=LOWER({{DisplayName}})`
   - **Example**: If `DisplayName` is "Administrator", the computed `Name` will be "administrator".

## Departments

### Input Fields
1. **DisplayName**
   - **Type**: String (raw)
   - **Description**: The human-readable name of the department.

### Calculated Fields
1. **Name**
   - **Description**: This field generates a machine-friendly name for the department that can be used for programmatic references.
   - **Computation**: To compute the `Name`, take the `DisplayName`, convert it to lowercase, and replace spaces with hyphens.
   - **Formula**: `=SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")`
   - **Example**: If `DisplayName` is "Human Resources", the computed `Name` will be "human-resources".

This document provides a comprehensive guide to computing the calculated fields in the "Jessica Talisman - BASIC Ontology Parts 1-3" rulebook. Each calculation is derived from specific input fields, ensuring clarity and accuracy in the computation process.