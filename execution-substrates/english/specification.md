# ACME, LLC Rulebook Specification

## Overview
This rulebook outlines the schema and calculations for the ACME, LLC customer management system. It includes the definitions of various entities, their fields, and how to compute derived fields based on the input data. The primary focus is on the Customers, ERBVersions, and ERBCustomizations tables.

---

## Customers Table

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
   - **Description:** An identifier for the customers, derived from their email address.
   - **Computation:** Replace the "@" symbol in the EmailAddress with a "-" to create a unique identifier.
   - **Original Formula:** `=SUBSTITUTE({{EmailAddress}}, "@", "-")`
   - **Example:** For the email `jane.smith@email.com`, the Name would be `jane.smith-email.com`.

2. **FullName**
   - **Type:** Calculated
   - **Description:** The full name of the customer, formatted as "LastName, FirstName".
   - **Computation:** Concatenate the LastName and FirstName fields, separated by a comma and a space.
   - **Original Formula:** `={{LastName}} & ", " & {{FirstName}}`
   - **Example:** For FirstName `Bobby` and LastName `Smith`, the FullName would be `Smith, Bobby`.

---

## ERBVersions Table

### Input Fields
1. **ERBVersionId**
   - **Type:** String
   - **Description:** Unique identifier for the ERB version. This field is mandatory.

2. **BaseId**
   - **Type:** String
   - **Description:** Identifier for the base associated with this ERB version. This field is optional.

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
   - **Description:** Date and time of the commit for this ERB version. This field is optional.

7. **IsPublished**
   - **Type:** Boolean
   - **Description:** Indicates whether this ERB version is published. This field is optional.

### Derived Fields
- **No derived fields are defined for the ERBVersions table.**

---

## ERBCustomizations Table

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
   - **Description:** SQL code associated with the customization. This field is optional.

5. **SQLTarget**
   - **Type:** String
   - **Description:** The target database for the SQL code. This field is optional.

6. **CustomizationType**
   - **Type:** String
   - **Description:** Type of customization (e.g., Schema, Functions, Views, RLS, Data). This field is optional.

### Derived Fields
- **No derived fields are defined for the ERBCustomizations table.**

---

This specification provides a clear understanding of how to compute derived fields within the ACME, LLC rulebook, ensuring accurate data representation and processing.