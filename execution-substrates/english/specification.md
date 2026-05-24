# ACME, LLC Rulebook Specification

## Overview
This specification document outlines the structure and computation methods for the rulebook generated from the Airtable base "ACME, LLC (template)." It details the entities involved, their input fields, and the derived fields, including how to compute each derived field based on the raw input data.

## Entity: Customers

### Input Fields
1. **CustomerId**
   - **Type:** String
   - **Description:** Unique identifier for the customer. This field is mandatory.

2. **EmailAddress**
   - **Type:** String
   - **Description:** The customer's email address. This field is optional.

3. **FirstName**
   - **Type:** String
   - **Description:** The first name of the customer. This field is optional.

4. **LastName**
   - **Type:** String
   - **Description:** The last name of the customer. This field is optional.

### Derived Fields
1. **Name**
   - **Type:** Calculated
   - **Description:** This field serves as an identifier for the customers.
   - **Computation:** The Name is derived by replacing the "@" symbol in the EmailAddress with a hyphen ("-"). 
   - **Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example:** For the customer with EmailAddress `jane.smith@email.com`, the Name would be `jane.smith-email.com`.

2. **FullName**
   - **Type:** Calculated
   - **Description:** The full name is constructed from the FirstName and LastName of the customer.
   - **Computation:** The FullName is created by concatenating the LastName, a comma and space, and the FirstName.
   - **Formula:** `={{LastName}} & ", " & {{FirstName}}`
   - **Example:** For a customer with LastName `Smith` and FirstName `Bobby`, the FullName would be `Smith, Bobby`.

## Entity: ERBVersions

### Input Fields
1. **ERBVersionId**
   - **Type:** String
   - **Description:** Unique identifier for the ERB version. This field is mandatory.

2. **BaseId**
   - **Type:** String
   - **Description:** Identifier for the base. This field is optional.

3. **Name**
   - **Type:** String
   - **Description:** Name of the ERB version. This field is optional.

4. **Message**
   - **Type:** String
   - **Description:** Message associated with the ERB version. This field is optional.

5. **Notes**
   - **Type:** String
   - **Description:** Additional notes regarding the ERB version. This field is optional.

6. **CommitDate**
   - **Type:** Datetime
   - **Description:** Date and time of the commit. This field is optional.

7. **IsPublished**
   - **Type:** Boolean
   - **Description:** Indicates whether the ERB version is published. This field is optional.

### Derived Fields
- There are no derived fields in the ERBVersions entity.

## Entity: ERBCustomizations

### Input Fields
1. **ERBCustomizationId**
   - **Type:** String
   - **Description:** Unique identifier for the ERB customization. This field is mandatory.

2. **Name**
   - **Type:** String
   - **Description:** Name of the ERB customization. This field is optional.

3. **Title**
   - **Type:** String
   - **Description:** Title of the ERB customization. This field is optional.

4. **SQLCode**
   - **Type:** String
   - **Description:** SQL code for the customization. This field is optional.

5. **SQLTarget**
   - **Type:** String
   - **Description:** Target database for the SQL code. This field is optional.

6. **CustomizationType**
   - **Type:** String
   - **Description:** Type of customization (e.g., Schema, Functions, Views, RLS, Data). This field is optional.

### Derived Fields
- There are no derived fields in the ERBCustomizations entity.

## Summary
This document provides a comprehensive overview of the input and derived fields within the ACME, LLC rulebook. The derived fields are computed based on specific formulas that utilize the raw input fields, ensuring clarity in how to derive each value. The examples provided illustrate the expected outcomes based on the given data.