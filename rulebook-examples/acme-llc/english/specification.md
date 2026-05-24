# ACME, LLC Rulebook Specification Document

## Overview
This document outlines the specifications for the ACME, LLC rulebook, which is generated from an Airtable base. It describes the structure of the data, including input fields and how to compute derived fields for the Customers, ERBVersions, and ERBCustomizations entities.

## Customers Entity

### Input Fields
1. **CustomerId**
   - **Type**: String (raw)
   - **Description**: Unique identifier for each customer.

2. **EmailAddress**
   - **Type**: String (raw)
   - **Description**: The customer's email address.

3. **FirstName**
   - **Type**: String (raw)
   - **Description**: The first name of the customer.

4. **LastName**
   - **Type**: String (raw)
   - **Description**: The last name of the customer.

### Derived Fields

1. **Name**
   - **Type**: String (calculated)
   - **Description**: An identifier for the customers, derived from their email address.
   - **Computation**: Replace the "@" symbol in the EmailAddress with a "-" to create a unique identifier.
   - **Original Formula**: `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example**: For the email `jane.smith@email.com`, the computed Name would be `jane.smith-email.com`.

2. **FullName**
   - **Type**: String (calculated)
   - **Description**: The full name of the customer, formatted as "LastName, FirstName".
   - **Computation**: Concatenate the LastName and FirstName fields with a comma and a space in between.
   - **Original Formula**: `={{LastName}} & ", " & {{FirstName}}`
   - **Example**: For FirstName `Bobby` and LastName `Smith`, the computed FullName would be `Smith, Bobby`.

## ERBVersions Entity

### Input Fields
1. **ERBVersionId**
   - **Type**: String (raw)
   - **Description**: Unique identifier for each ERB version.

2. **BaseId**
   - **Type**: String (raw)
   - **Description**: Identifier for the base associated with the ERB version.

3. **Name**
   - **Type**: String (raw)
   - **Description**: Name of the ERB version.

4. **Message**
   - **Type**: String (raw)
   - **Description**: Message associated with the ERB version.

5. **Notes**
   - **Type**: String (raw)
   - **Description**: Additional notes related to the ERB version.

6. **CommitDate**
   - **Type**: Datetime (raw)
   - **Description**: The date and time when the ERB version was committed.

7. **IsPublished**
   - **Type**: Boolean (raw)
   - **Description**: Indicates whether the ERB version is published.

### Derived Fields
There are no derived fields in the ERBVersions entity.

## ERBCustomizations Entity

### Input Fields
1. **ERBCustomizationId**
   - **Type**: String (raw)
   - **Description**: Unique identifier for each ERB customization.

2. **Name**
   - **Type**: String (raw)
   - **Description**: Name of the ERB customization.

3. **Title**
   - **Type**: String (raw)
   - **Description**: Title of the ERB customization.

4. **SQLCode**
   - **Type**: String (raw)
   - **Description**: SQL code associated with the customization.

5. **SQLTarget**
   - **Type**: String (raw)
   - **Description**: The target database for the SQL code.

6. **CustomizationType**
   - **Type**: String (raw)
   - **Description**: Type of customization (e.g., Schema, Functions, Views, RLS, Data).

### Derived Fields
There are no derived fields in the ERBCustomizations entity.

## Summary
This specification document provides a detailed description of how to compute derived fields for the Customers entity, including the input fields and their descriptions. The ERBVersions and ERBCustomizations entities contain only raw input fields without derived fields. This guide should enable users to accurately compute the necessary values based on the provided rules and examples.