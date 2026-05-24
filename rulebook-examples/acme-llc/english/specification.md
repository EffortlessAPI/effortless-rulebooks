# ACME, LLC Rulebook Specification Document

## Overview
This document provides a detailed specification for the ACME, LLC rulebook, which outlines the structure and calculations for customer data management. The rulebook includes definitions for various entities, their fields, and how to compute derived fields based on raw inputs.

## Entity: Customers

### Input Fields
1. **CustomerId**
   - **Type:** String (raw)
   - **Description:** Unique identifier for the customer. This field is mandatory.

2. **EmailAddress**
   - **Type:** String (raw)
   - **Description:** The customer's email address. This field is optional.

3. **FirstName**
   - **Type:** String (raw)
   - **Description:** The first name of the customer. This field is optional.

4. **LastName**
   - **Type:** String (raw)
   - **Description:** The last name of the customer. This field is optional.

### Derived Fields
1. **Name**
   - **Type:** String (calculated)
   - **Description:** Identifier for the customers, derived from the EmailAddress.
   - **Computation:** The Name is computed by replacing the "@" symbol in the EmailAddress with a hyphen ("-"). 
   - **Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example:** For the EmailAddress `jane.smith@email.com`, the computed Name would be `jane.smith-email.com`.

2. **FullName**
   - **Type:** String (calculated)
   - **Description:** The full name of the customer, formatted as "LastName, FirstName".
   - **Computation:** The FullName is created by concatenating the LastName and FirstName with a comma and a space in between.
   - **Formula:** `={{LastName}} & ", " & {{FirstName}}`
   - **Example:** For a customer with LastName `Smith` and FirstName `Bobby`, the FullName would be `Smith, Bobby`.

### Example Data
Here are a few examples of how the derived fields are computed based on the provided data:

- For the customer with:
  - **EmailAddress:** `jane.smith@email.com`
  - **FirstName:** `Bobby`
  - **LastName:** `Smith`
  
  The computed fields would be:
  - **Name:** `jane.smith-email.com`
  - **FullName:** `Smith, Bobby`

- For the customer with:
  - **EmailAddress:** `john.doe@email.com`
  - **FirstName:** `Jimmy`
  - **LastName:** `Doe`
  
  The computed fields would be:
  - **Name:** `john.doe-email.com`
  - **FullName:** `Doe, Jimmy`

- For the customer with:
  - **EmailAddress:** `emily.jones@email.com`
  - **FirstName:** `Mary`
  - **LastName:** `Jones`
  
  The computed fields would be:
  - **Name:** `emily.jones-email.com`
  - **FullName:** `Jones, Mary`

## Entity: ERBVersions
### Input Fields
1. **ERBVersionId**
   - **Type:** String (raw)
   - **Description:** Unique identifier for the ERB version. This field is mandatory.

2. **BaseId**
   - **Type:** String (raw)
   - **Description:** Identifier for the base. This field is optional.

3. **Name**
   - **Type:** String (raw)
   - **Description:** Name of the ERB version. This field is optional.

4. **Message**
   - **Type:** String (raw)
   - **Description:** Message associated with the ERB version. This field is optional.

5. **Notes**
   - **Type:** String (raw)
   - **Description:** Additional notes for the ERB version. This field is optional.

6. **CommitDate**
   - **Type:** Datetime (raw)
   - **Description:** Date and time of the commit. This field is optional.

7. **IsPublished**
   - **Type:** Boolean (raw)
   - **Description:** Indicates whether the ERB version is published. This field is optional.

### Derived Fields
*There are no derived fields in the ERBVersions entity as per the provided rulebook.*

## Entity: ERBCustomizations
### Input Fields
1. **ERBCustomizationId**
   - **Type:** String (raw)
   - **Description:** Unique identifier for the ERB customization. This field is mandatory.

2. **Name**
   - **Type:** String (raw)
   - **Description:** Name of the ERB customization. This field is optional.

3. **Title**
   - **Type:** String (raw)
   - **Description:** Title of the ERB customization. This field is optional.

4. **SQLCode**
   - **Type:** String (raw)
   - **Description:** SQL code associated with the customization. This field is optional.

5. **SQLTarget**
   - **Type:** String (raw)
   - **Description:** Target database for the SQL code. This field is optional.

6. **CustomizationType**
   - **Type:** String (raw)
   - **Description:** Type of customization (e.g., Schema, Functions, Views, RLS, Data). This field is optional.

### Derived Fields
*There are no derived fields in the ERBCustomizations entity as per the provided rulebook.*

## Conclusion
This specification document outlines the structure and computation methods for the ACME, LLC rulebook. By following the provided descriptions and examples, one can accurately compute the derived fields for the Customers entity and understand the input structure for the ERBVersions and ERBCustomizations entities.