# ACME, LLC Rulebook Specification Document

## Overview
This document provides a detailed specification for the ACME, LLC rulebook, which defines the schema and calculations for customer data, ERB versions, and ERB customizations. The rulebook includes raw input fields as well as derived fields that are calculated based on the input data.

## Entity: Customers

### Input Fields
1. **CustomerId**
   - **Type**: String
   - **Description**: Unique identifier for the customer. This field is mandatory.

2. **EmailAddress**
   - **Type**: String
   - **Description**: The customer's email address. This field is optional.

3. **FirstName**
   - **Type**: String
   - **Description**: The first name of the customer. This field is optional.

4. **LastName**
   - **Type**: String
   - **Description**: The last name of the customer. This field is optional.

### Derived Fields
1. **Name**
   - **Type**: Calculated
   - **Description**: This field serves as an identifier for the customers, derived from the customer's email address.
   - **Computation**: The `Name` is computed by replacing the "@" symbol in the `EmailAddress` with a hyphen ("-"). 
   - **Formula**: `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example**: For `EmailAddress` "jane.smith@email.com", the computed `Name` would be "jane.smith-email.com".

2. **FullName**
   - **Type**: Calculated
   - **Description**: This field represents the full name of the customer, constructed from their first and last names.
   - **Computation**: The `FullName` is created by concatenating the `LastName`, a comma and a space, and the `FirstName`.
   - **Formula**: `={{LastName}} & ", " & {{FirstName}}`
   - **Example**: For `FirstName` "Bobby" and `LastName` "Smith", the computed `FullName` would be "Smith, Bobby".

## Entity: ERBVersions

### Input Fields
1. **ERBVersionId**
   - **Type**: String
   - **Description**: Unique identifier for the ERB version. This field is mandatory.

2. **BaseId**
   - **Type**: String
   - **Description**: Identifier for the base associated with the ERB version. This field is optional.

3. **Name**
   - **Type**: String
   - **Description**: Name of the ERB version. This field is optional.

4. **Message**
   - **Type**: String
   - **Description**: Message associated with the ERB version. This field is optional.

5. **Notes**
   - **Type**: String
   - **Description**: Additional notes related to the ERB version. This field is optional.

6. **CommitDate**
   - **Type**: Datetime
   - **Description**: Date and time when the ERB version was committed. This field is optional.

7. **IsPublished**
   - **Type**: Boolean
   - **Description**: Indicates whether the ERB version is published. This field is optional.

### Derived Fields
*There are no derived fields in the ERBVersions entity.*

## Entity: ERBCustomizations

### Input Fields
1. **ERBCustomizationId**
   - **Type**: String
   - **Description**: Unique identifier for the ERB customization. This field is mandatory.

2. **Name**
   - **Type**: String
   - **Description**: Name of the ERB customization. This field is optional.

3. **Title**
   - **Type**: String
   - **Description**: Title of the ERB customization. This field is optional.

4. **SQLCode**
   - **Type**: String
   - **Description**: SQL code associated with the ERB customization. This field is optional.

5. **SQLTarget**
   - **Type**: String
   - **Description**: Target database for the SQL code. This field is optional.

6. **CustomizationType**
   - **Type**: String
   - **Description**: Type of customization (e.g., Schema, Functions, Views, RLS, Data). This field is optional.

### Derived Fields
*There are no derived fields in the ERBCustomizations entity.*

## Summary
This specification document outlines the input fields and derived fields for the ACME, LLC rulebook. The calculations for the derived fields are clearly defined, allowing for accurate computation based on the provided input data.