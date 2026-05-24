# ACME, LLC Rulebook Specification Document

## Overview
This document provides a detailed specification for the ACME, LLC rulebook, which outlines the schema and calculations for customer data management. The rulebook defines how to derive calculated fields from raw input data, ensuring clarity in data processing and reporting.

## Customers Table

### Input Fields
1. **CustomerId**
   - **Type:** String
   - **Description:** Unique identifier for each customer. This field is mandatory.

2. **EmailAddress**
   - **Type:** String
   - **Description:** The customer's email address. This field is optional.

3. **FirstName**
   - **Type:** String
   - **Description:** The first name of the customer. This field is optional.

4. **LastName**
   - **Type:** String
   - **Description:** The last name of the customer. This field is optional.

### Calculated Fields
1. **Name**
   - **Type:** Calculated
   - **Description:** A unique identifier for the customers, derived from their email address.
   - **Computation:** The Name is computed by replacing the "@" symbol in the EmailAddress with a hyphen ("-"). 
   - **Original Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example:** For a customer with the EmailAddress `jane.smith@email.com`, the Name would be `jane.smith-email.com`.

2. **FullName**
   - **Type:** Calculated
   - **Description:** The full name of the customer, formatted as "LastName, FirstName".
   - **Computation:** The FullName is constructed by concatenating the LastName and FirstName fields, separated by a comma and a space.
   - **Original Formula:** `={{LastName}} & ", " & {{FirstName}}`
   - **Example:** For a customer with LastName `Smith` and FirstName `Bobby`, the FullName would be `Smith, Bobby`.

### Example Data
| CustomerId                  | EmailAddress          | FirstName | LastName | Name                     | FullName       |
|-----------------------------|-----------------------|-----------|----------|--------------------------|-----------------|
| jane-smith-email-com        | jane.smith@email.com  | Bobby     | Smith    | jane.smith-email.com     | Smith, Bobby    |
| john-doe-email-com         | john.doe@email.com    | Jimmy     | Doe      | john.doe-email.com       | Doe, Jimmy      |
| emily-jones-email-com       | emily.jones@email.com | Mary      | Jones    | emily.jones-email.com    | Jones, Mary     |

## ERBVersions Table

### Input Fields
1. **ERBVersionId**
   - **Type:** String
   - **Description:** Unique identifier for each ERB version. This field is mandatory.

2. **BaseId**
   - **Type:** String
   - **Description:** Identifier for the base associated with the ERB version. This field is optional.

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
   - **Description:** The date and time when the ERB version was committed. This field is optional.

7. **IsPublished**
   - **Type:** Boolean
   - **Description:** Indicates whether the ERB version is published. This field is optional.

### Calculated Fields
*No calculated fields are defined in the ERBVersions table.*

## ERBCustomizations Table

### Input Fields
1. **ERBCustomizationId**
   - **Type:** String
   - **Description:** Unique identifier for each ERB customization. This field is mandatory.

2. **Name**
   - **Type:** String
   - **Description:** Name of the ERB customization. This field is optional.

3. **Title**
   - **Type:** String
   - **Description:** Title of the ERB customization. This field is optional.

4. **SQLCode**
   - **Type:** String
   - **Description:** SQL code associated with the customization. This field is optional.

5. **SQLTarget**
   - **Type:** String
   - **Description:** The target database for the SQL code. This field is optional.

6. **CustomizationType**
   - **Type:** String
   - **Description:** Type of customization (e.g., Schema, Functions, Views, RLS, Data). This field is optional.

### Calculated Fields
*No calculated fields are defined in the ERBCustomizations table.*

## Conclusion
This specification document provides a comprehensive overview of how to compute derived fields within the ACME, LLC rulebook. By following the outlined computations, users can accurately derive customer names and full names from the provided raw input fields.